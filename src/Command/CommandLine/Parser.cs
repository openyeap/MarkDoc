using CommandLine.Infrastructure;
using CommandLine.Parsing;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
namespace CommandLine
{
	public sealed class Parser : IDisposable
	{
		public const int DefaultExitCodeFail = 1;
		private static readonly Parser DefaultParser = new Parser(true);
		private readonly ParserSettings _settings;
		private bool _disposed;
		public static Parser Default
		{
			get
			{
				return Parser.DefaultParser;
			}
		}
		public ParserSettings Settings
		{
			get
			{
				return this._settings;
			}
		}
		public Parser()
		{
			this._settings = new ParserSettings
			{
				Consumed = true
			};
		}
		[Obsolete("Use constructor that accepts Action<ParserSettings>.")]
		public Parser(ParserSettings settings)
		{
			Assumes.NotNull<ParserSettings>(settings, "settings", "The command line parser settings instance cannot be null.");
			if (settings.Consumed)
			{
				throw new InvalidOperationException("The command line parserSettings instance cannnot be used more than once.");
			}
			this._settings = settings;
			this._settings.Consumed = true;
		}
		public Parser(Action<ParserSettings> configuration)
		{
			Assumes.NotNull<Action<ParserSettings>>(configuration, "configuration", "The command line parser settings delegate cannot be null.");
			this._settings = new ParserSettings();
			configuration(this.Settings);
			this._settings.Consumed = true;
		}
		private Parser(bool singleton) : this(delegate(ParserSettings with)
		{
			with.CaseSensitive = false;
			with.MutuallyExclusive = false;
			with.HelpWriter = Console.Error;
			with.ParsingCulture = CultureInfo.InvariantCulture;
		})
		{
		}
		~Parser()
		{
			this.Dispose(false);
		}
		public bool ParseArguments(string[] args, object options)
		{
			Assumes.NotNull<string[]>(args, "args", "The arguments string array cannot be null.");
			Assumes.NotNull<object>(options, "options", "The target options instance cannot be null.");
			return this.DoParseArguments(args, options);
		}
		public bool ParseArguments(string[] args, object options, Action<string, object> onVerbCommand)
		{
			Assumes.NotNull<string[]>(args, "args", "The arguments string array cannot be null.");
			Assumes.NotNull<object>(options, "options", "The target options instance cannot be null.");
			Assumes.NotNull<object>(options, "onVerbCommand", "Delegate executed to capture verb command instance reference cannot be null.");
			object obj = null;
			bool flag = this.DoParseArgumentsVerbs(args, options, ref obj);
			onVerbCommand(args.FirstOrDefault<string>() ?? string.Empty, flag ? obj : null);
			return flag;
		}
		public bool ParseArgumentsStrict(string[] args, object options, Action onFail = null)
		{
			Assumes.NotNull<string[]>(args, "args", "The arguments string array cannot be null.");
			Assumes.NotNull<object>(options, "options", "The target options instance cannot be null.");
			if (!this.DoParseArguments(args, options))
			{
				this.InvokeAutoBuildIfNeeded(options);
				if (onFail == null)
				{
					Environment.Exit(1);
				}
				else
				{
					onFail();
				}
				return false;
			}
			return true;
		}
		public bool ParseArgumentsStrict(string[] args, object options, Action<string, object> onVerbCommand, Action onFail = null)
		{
			Assumes.NotNull<string[]>(args, "args", "The arguments string array cannot be null.");
			Assumes.NotNull<object>(options, "options", "The target options instance cannot be null.");
			Assumes.NotNull<object>(options, "onVerbCommand", "Delegate executed to capture verb command instance reference cannot be null.");
			object arg = null;
			if (!this.DoParseArgumentsVerbs(args, options, ref arg))
			{
				onVerbCommand(args.FirstOrDefault<string>() ?? string.Empty, null);
				this.InvokeAutoBuildIfNeeded(options);
				if (onFail == null)
				{
					Environment.Exit(1);
				}
				else
				{
					onFail();
				}
				return false;
			}
			onVerbCommand(args.FirstOrDefault<string>() ?? string.Empty, arg);
			return true;
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		internal static object InternalGetVerbOptionsInstanceByName(string verb, object target, out bool found)
		{
			found = false;
			if (string.IsNullOrEmpty(verb))
			{
				return target;
			}
			Pair<PropertyInfo, VerbOptionAttribute> pair = ReflectionHelper.RetrieveOptionProperty<VerbOptionAttribute>(target, verb);
			found = (pair != null);
			if (!found)
			{
				return target;
			}
			return pair.Left.GetValue(target, null);
		}
		private static void SetParserStateIfNeeded(object options, IEnumerable<ParsingError> errors)
		{
			if (!options.CanReceiveParserState())
			{
				return;
			}
			PropertyInfo left = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options)[0].Left;
			object value = left.GetValue(options, null);
			if (value != null)
			{
				if (!(value is IParserState))
				{
					throw new InvalidOperationException("Cannot apply ParserStateAttribute to a property that does not implement IParserState or is not accessible.");
				}
				if (!(value is ParserState))
				{
					throw new InvalidOperationException("ParserState instance cannot be supplied.");
				}
			}
			else
			{
				try
				{
					left.SetValue(options, new ParserState(), null);
				}
				catch (Exception innerException)
				{
					throw new InvalidOperationException("Cannot apply ParserStateAttribute to a property that does not implement IParserState or is not accessible.", innerException);
				}
			}
			IParserState parserState = (IParserState)left.GetValue(options, null);
			foreach (ParsingError current in errors)
			{
				parserState.Errors.Add(current);
			}
		}
		private static StringComparison GetStringComparison(ParserSettings settings)
		{
			if (!settings.CaseSensitive)
			{
				return StringComparison.OrdinalIgnoreCase;
			}
			return StringComparison.Ordinal;
		}
		private bool DoParseArguments(string[] args, object options)
		{
			Pair<MethodInfo, HelpOptionAttribute> pair = ReflectionHelper.RetrieveMethod<HelpOptionAttribute>(options);
			TextWriter helpWriter = this._settings.HelpWriter;
			if (pair == null || helpWriter == null)
			{
				return this.DoParseArgumentsCore(args, options);
			}
			if (this.ParseHelp(args, pair.Right) || !this.DoParseArgumentsCore(args, options))
			{
				string value;
				HelpOptionAttribute.InvokeMethod(options, pair, out value);
				helpWriter.Write(value);
				return false;
			}
			return true;
		}
		private bool DoParseArgumentsCore(string[] args, object options)
		{
			bool flag = false;
			OptionMap optionMap = OptionMap.Create(options, this._settings);
			optionMap.SetDefaults();
			ValueMapper valueMapper = new ValueMapper(options, this._settings.ParsingCulture);
			StringArrayEnumerator stringArrayEnumerator = new StringArrayEnumerator(args);
			while (stringArrayEnumerator.MoveNext())
			{
				string current = stringArrayEnumerator.Current;
				if (!string.IsNullOrEmpty(current))
				{
					ArgumentParser argumentParser = ArgumentParser.Create(current, this._settings.IgnoreUnknownArguments);
					if (argumentParser != null)
					{
						PresentParserState presentParserState = argumentParser.Parse(stringArrayEnumerator, optionMap, options);
						if ((ushort)(presentParserState & PresentParserState.Failure) == 2)
						{
							Parser.SetParserStateIfNeeded(options, argumentParser.PostParsingState);
							flag = true;
						}
						else
						{
							if ((ushort)(presentParserState & PresentParserState.MoveOnNextElement) == 4)
							{
								stringArrayEnumerator.MoveNext();
							}
						}
					}
					else
					{
						if (valueMapper.CanReceiveValues && !valueMapper.MapValueItem(current))
						{
							flag = true;
						}
					}
				}
			}
			flag |= !optionMap.EnforceRules();
			return !flag;
		}
		private bool DoParseArgumentsVerbs(string[] args, object options, ref object verbInstance)
		{
			IList<Pair<PropertyInfo, VerbOptionAttribute>> verbs = ReflectionHelper.RetrievePropertyList<VerbOptionAttribute>(options);
			Pair<MethodInfo, HelpVerbOptionAttribute> pair = ReflectionHelper.RetrieveMethod<HelpVerbOptionAttribute>(options);
			if (args.Length == 0)
			{
				if (pair != null || this._settings.HelpWriter != null)
				{
					this.DisplayHelpVerbText(options, pair, null);
				}
				return false;
			}
			OptionMap optionMap = OptionMap.Create(options, verbs, this._settings);
			if (this.TryParseHelpVerb(args, options, pair, optionMap))
			{
				return false;
			}
			OptionInfo optionInfo = optionMap[args.First<string>()];
			if (optionInfo == null)
			{
				if (pair != null)
				{
					this.DisplayHelpVerbText(options, pair, null);
				}
				return false;
			}
			verbInstance = optionInfo.GetValue(options);
			if (verbInstance == null)
			{
				verbInstance = optionInfo.CreateInstance(options);
			}
			bool flag = this.DoParseArgumentsCore(args.Skip(1).ToArray<string>(), verbInstance);
			if (!flag && pair != null)
			{
				this.DisplayHelpVerbText(options, pair, args.First<string>());
			}
			return flag;
		}
		private bool ParseHelp(string[] args, HelpOptionAttribute helpOption)
		{
			bool caseSensitive = this._settings.CaseSensitive;
			int i = 0;
			while (i < args.Length)
			{
				string argument = args[i];
				char? shortName = helpOption.ShortName;
				bool result;
				if ((shortName.HasValue ? new int?((int)shortName.GetValueOrDefault()) : null).HasValue && ArgumentParser.CompareShort(argument, helpOption.ShortName, caseSensitive))
				{
					result = true;
				}
				else
				{
					if (string.IsNullOrEmpty(helpOption.LongName) || !ArgumentParser.CompareLong(argument, helpOption.LongName, caseSensitive))
					{
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			return false;
		}
		private bool TryParseHelpVerb(string[] args, object options, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, OptionMap optionMap)
		{
			TextWriter helpWriter = this._settings.HelpWriter;
			if (helpInfo != null && helpWriter != null && string.Compare(args[0], helpInfo.Right.LongName, Parser.GetStringComparison(this._settings)) == 0)
			{
				string text = args.FirstOrDefault<string>();
				if (text != null)
				{
					OptionInfo optionInfo = optionMap[text];
					if (optionInfo != null && optionInfo.GetValue(options) == null)
					{
						optionInfo.CreateInstance(options);
					}
				}
				this.DisplayHelpVerbText(options, helpInfo, text);
				return true;
			}
			return false;
		}
		private void DisplayHelpVerbText(object options, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, string verb)
		{
			string value;
			if (verb == null)
			{
				HelpVerbOptionAttribute.InvokeMethod(options, helpInfo, null, out value);
			}
			else
			{
				HelpVerbOptionAttribute.InvokeMethod(options, helpInfo, verb, out value);
			}
			if (this._settings.HelpWriter != null)
			{
				this._settings.HelpWriter.Write(value);
			}
		}
		private void InvokeAutoBuildIfNeeded(object options)
		{
			if (this._settings.HelpWriter == null || options.HasHelp() || options.HasVerbHelp())
			{
				return;
			}
			this._settings.HelpWriter.Write(HelpText.AutoBuild(options, delegate(HelpText current)
			{
				HelpText.DefaultParsingErrorsHandler(options, current);
			}, options.HasVerbs()));
		}
		private void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing)
			{
				if (this._settings != null)
				{
					this._settings.Dispose();
				}
				this._disposed = true;
			}
		}
	}
}
