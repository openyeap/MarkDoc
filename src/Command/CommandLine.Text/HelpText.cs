using CommandLine.Extensions;
using CommandLine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace CommandLine.Text
{
	public class HelpText
	{
		private const int BuilderCapacity = 128;
		private const int DefaultMaximumLength = 80;
		private const string DefaultRequiredWord = "Required.";
		private readonly StringBuilder _preOptionsHelp;
		private readonly StringBuilder _postOptionsHelp;
		private readonly BaseSentenceBuilder _sentenceBuilder;
		private int? _maximumDisplayWidth;
		private string _heading;
		private string _copyright;
		private bool _additionalNewLineAfterOption;
		private StringBuilder _optionsHelp;
		private bool _addDashesToOption;
		public event EventHandler<FormatOptionHelpTextEventArgs> FormatOptionHelpText;
		public string Heading
		{
			get
			{
				return this._heading;
			}
			set
			{
				Assumes.NotNullOrEmpty(value, "value");
				this._heading = value;
			}
		}
		public string Copyright
		{
			get
			{
				return this._heading;
			}
			set
			{
				Assumes.NotNullOrEmpty(value, "value");
				this._copyright = value;
			}
		}
		public int MaximumDisplayWidth
		{
			get
			{
				if (!this._maximumDisplayWidth.HasValue)
				{
					return 80;
				}
				return this._maximumDisplayWidth.Value;
			}
			set
			{
				this._maximumDisplayWidth = new int?(value);
			}
		}
		public bool AddDashesToOption
		{
			get
			{
				return this._addDashesToOption;
			}
			set
			{
				this._addDashesToOption = value;
			}
		}
		public bool AdditionalNewLineAfterOption
		{
			get
			{
				return this._additionalNewLineAfterOption;
			}
			set
			{
				this._additionalNewLineAfterOption = value;
			}
		}
		public BaseSentenceBuilder SentenceBuilder
		{
			get
			{
				return this._sentenceBuilder;
			}
		}
		public HelpText()
		{
			this._preOptionsHelp = new StringBuilder(128);
			this._postOptionsHelp = new StringBuilder(128);
			this._sentenceBuilder = BaseSentenceBuilder.CreateBuiltIn();
		}
		public HelpText(BaseSentenceBuilder sentenceBuilder) : this()
		{
			Assumes.NotNull<BaseSentenceBuilder>(sentenceBuilder, "sentenceBuilder");
			this._sentenceBuilder = sentenceBuilder;
		}
		public HelpText(string heading) : this()
		{
			Assumes.NotNullOrEmpty(heading, "heading");
			this._heading = heading;
		}
		public HelpText(BaseSentenceBuilder sentenceBuilder, string heading) : this(heading)
		{
			Assumes.NotNull<BaseSentenceBuilder>(sentenceBuilder, "sentenceBuilder");
			this._sentenceBuilder = sentenceBuilder;
		}
		public HelpText(string heading, string copyright) : this()
		{
			Assumes.NotNullOrEmpty(heading, "heading");
			Assumes.NotNullOrEmpty(copyright, "copyright");
			this._heading = heading;
			this._copyright = copyright;
		}
		public HelpText(BaseSentenceBuilder sentenceBuilder, string heading, string copyright) : this(heading, copyright)
		{
			Assumes.NotNull<BaseSentenceBuilder>(sentenceBuilder, "sentenceBuilder");
			this._sentenceBuilder = sentenceBuilder;
		}
		public HelpText(string heading, string copyright, object options) : this()
		{
			Assumes.NotNullOrEmpty(heading, "heading");
			Assumes.NotNullOrEmpty(copyright, "copyright");
			Assumes.NotNull<object>(options, "options");
			this._heading = heading;
			this._copyright = copyright;
			this.DoAddOptions(options, "Required.", this.MaximumDisplayWidth, false);
		}
		public HelpText(BaseSentenceBuilder sentenceBuilder, string heading, string copyright, object options) : this(heading, copyright, options)
		{
			Assumes.NotNull<BaseSentenceBuilder>(sentenceBuilder, "sentenceBuilder");
			this._sentenceBuilder = sentenceBuilder;
		}
		public static HelpText AutoBuild(object options)
		{
			return HelpText.AutoBuild(options, null, false);
		}
		public static HelpText AutoBuild(object options, Action<HelpText> onError, bool verbsIndex = false)
		{
			HelpText helpText = new HelpText
			{
				Heading = HeadingInfo.Default,
				Copyright = CopyrightInfo.Default,
				AdditionalNewLineAfterOption = true,
				AddDashesToOption = !verbsIndex
			};
			if (onError != null)
			{
				IList<Pair<PropertyInfo, ParserStateAttribute>> list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
				if (list != null)
				{
					onError(helpText);
				}
			}
			AssemblyLicenseAttribute attribute = ReflectionHelper.GetAttribute<AssemblyLicenseAttribute>();
			if (attribute != null)
			{
				attribute.AddToHelpText(helpText, true);
			}
			AssemblyUsageAttribute attribute2 = ReflectionHelper.GetAttribute<AssemblyUsageAttribute>();
			if (attribute2 != null)
			{
				attribute2.AddToHelpText(helpText, true);
			}
			helpText.AddOptions(options);
			return helpText;
		}
		public static HelpText AutoBuild(object options, string verb)
		{
			bool flag;
			object obj = Parser.InternalGetVerbOptionsInstanceByName(verb, options, out flag);
			bool flag2 = verb == null || !flag;
			object target = flag2 ? options : obj;
			return HelpText.AutoBuild(target, delegate(HelpText current)
			{
				HelpText.DefaultParsingErrorsHandler(target, current);
			}, flag2);
		}
		public static void DefaultParsingErrorsHandler(object options, HelpText current)
		{
			IList<Pair<PropertyInfo, ParserStateAttribute>> list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
			if (list.Count == 0)
			{
				return;
			}
			IParserState parserState = (IParserState)list[0].Left.GetValue(options, null);
			if (parserState == null || parserState.Errors.Count == 0)
			{
				return;
			}
			string text = current.RenderParsingErrorsText(options, 2);
			if (!string.IsNullOrEmpty(text))
			{
				current.AddPreOptionsLine(Environment.NewLine + current.SentenceBuilder.ErrorsHeadingText);
				string[] array = text.Split(new string[]
				{
					Environment.NewLine
				}, StringSplitOptions.None);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string value = array2[i];
					current.AddPreOptionsLine(value);
				}
			}
		}
		public static implicit operator string(HelpText info)
		{
			return info.ToString();
		}
		public void AddPreOptionsLine(string value)
		{
			this.AddPreOptionsLine(value, this.MaximumDisplayWidth);
		}
		public void AddPostOptionsLine(string value)
		{
			this.AddLine(this._postOptionsHelp, value);
		}
		public void AddOptions(object options)
		{
			this.AddOptions(options, "Required.");
		}
		public void AddOptions(object options, string requiredWord)
		{
			Assumes.NotNull<object>(options, "options");
			Assumes.NotNullOrEmpty(requiredWord, "requiredWord");
			this.AddOptions(options, requiredWord, this.MaximumDisplayWidth);
		}
		public void AddOptions(object options, string requiredWord, int maximumLength)
		{
			Assumes.NotNull<object>(options, "options");
			Assumes.NotNullOrEmpty(requiredWord, "requiredWord");
			this.DoAddOptions(options, requiredWord, maximumLength, true);
		}
		public string RenderParsingErrorsText(object options, int indent)
		{
			IList<Pair<PropertyInfo, ParserStateAttribute>> list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
			if (list.Count == 0)
			{
				return string.Empty;
			}
			IParserState parserState = (IParserState)list[0].Left.GetValue(options, null);
			if (parserState == null || parserState.Errors.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ParsingError current in parserState.Errors)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(indent.Spaces());
				char? shortName = current.BadOption.ShortName;
				if ((shortName.HasValue ? new int?((int)shortName.GetValueOrDefault()) : null).HasValue)
				{
					stringBuilder2.Append('-');
					stringBuilder2.Append(current.BadOption.ShortName);
					if (!string.IsNullOrEmpty(current.BadOption.LongName))
					{
						stringBuilder2.Append('/');
					}
				}
				if (!string.IsNullOrEmpty(current.BadOption.LongName))
				{
					stringBuilder2.Append("--");
					stringBuilder2.Append(current.BadOption.LongName);
				}
				stringBuilder2.Append(" ");
				stringBuilder2.Append(current.ViolatesRequired ? this._sentenceBuilder.RequiredOptionMissingText : this._sentenceBuilder.OptionWord);
				if (current.ViolatesFormat)
				{
					stringBuilder2.Append(" ");
					stringBuilder2.Append(this._sentenceBuilder.ViolatesFormatText);
				}
				if (current.ViolatesMutualExclusiveness)
				{
					if (current.ViolatesFormat || current.ViolatesRequired)
					{
						stringBuilder2.Append(" ");
						stringBuilder2.Append(this._sentenceBuilder.AndWord);
					}
					stringBuilder2.Append(" ");
					stringBuilder2.Append(this._sentenceBuilder.ViolatesMutualExclusivenessText);
				}
				stringBuilder2.Append('.');
				stringBuilder.AppendLine(stringBuilder2.ToString());
			}
			return stringBuilder.ToString();
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(HelpText.GetLength(this._heading) + HelpText.GetLength(this._copyright) + HelpText.GetLength(this._preOptionsHelp) + HelpText.GetLength(this._optionsHelp) + 10);
			stringBuilder.Append(this._heading);
			if (!string.IsNullOrEmpty(this._copyright))
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this._copyright);
			}
			if (this._preOptionsHelp.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this._preOptionsHelp);
			}
			if (this._optionsHelp != null && this._optionsHelp.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this._optionsHelp);
			}
			if (this._postOptionsHelp.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this._postOptionsHelp);
			}
			return stringBuilder.ToString();
		}
		protected virtual void OnFormatOptionHelpText(FormatOptionHelpTextEventArgs e)
		{
			EventHandler<FormatOptionHelpTextEventArgs> formatOptionHelpText = this.FormatOptionHelpText;
			if (formatOptionHelpText != null)
			{
				formatOptionHelpText(this, e);
			}
		}
		private static int GetLength(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return value.Length;
		}
		private static int GetLength(StringBuilder value)
		{
			if (value == null)
			{
				return 0;
			}
			return value.Length;
		}
		private static void AddLine(StringBuilder builder, string value, int maximumLength)
		{
			Assumes.NotNull<string>(value, "value");
			if (builder.Length > 0)
			{
				builder.Append(Environment.NewLine);
			}
			do
			{
				int num = 0;
				string[] array = value.Split(new char[]
				{
					' '
				});
				int i = 0;
				while (i < array.Length)
				{
					if (array[i].Length < maximumLength - num)
					{
						builder.Append(array[i]);
						num += array[i].Length;
						if (maximumLength - num > 1 && i != array.Length - 1)
						{
							builder.Append(" ");
							num++;
						}
						i++;
					}
					else
					{
						if (array[i].Length >= maximumLength && num == 0)
						{
							builder.Append(array[i].Substring(0, maximumLength));
							num = maximumLength;
							break;
						}
						break;
					}
				}
				value = value.Substring(Math.Min(num, value.Length));
				if (value.Length > 0)
				{
					builder.Append(Environment.NewLine);
				}
			}
			while (value.Length > maximumLength);
			builder.Append(value);
		}
		private void DoAddOptions(object options, string requiredWord, int maximumLength, bool fireEvent = true)
		{
			IList<BaseOptionAttribute> list = ReflectionHelper.RetrievePropertyAttributeList<BaseOptionAttribute>(options);
			HelpOptionAttribute helpOptionAttribute = ReflectionHelper.RetrieveMethodAttributeOnly<HelpOptionAttribute>(options);
			if (helpOptionAttribute != null)
			{
				list.Add(helpOptionAttribute);
			}
			if (list.Count == 0)
			{
				return;
			}
			int maxLength = this.GetMaxLength(list);
			this._optionsHelp = new StringBuilder(128);
			int widthOfHelpText = maximumLength - (maxLength + 6);
			foreach (BaseOptionAttribute current in list)
			{
				this.AddOption(requiredWord, maxLength, current, widthOfHelpText, fireEvent);
			}
		}
		private void AddPreOptionsLine(string value, int maximumLength)
		{
			HelpText.AddLine(this._preOptionsHelp, value, maximumLength);
		}
		private void AddOption(string requiredWord, int maxLength, BaseOptionAttribute option, int widthOfHelpText, bool fireEvent = true)
		{
			this._optionsHelp.Append("  ");
			StringBuilder stringBuilder = new StringBuilder(maxLength);
			if (option.HasShortName)
			{
				if (this._addDashesToOption)
				{
					stringBuilder.Append('-');
				}
				stringBuilder.AppendFormat("{0}", option.ShortName);
				if (option.HasMetaValue)
				{
					stringBuilder.AppendFormat(" {0}", option.MetaValue);
				}
				if (option.HasLongName)
				{
					stringBuilder.Append(", ");
				}
			}
			if (option.HasLongName)
			{
				if (this._addDashesToOption)
				{
					stringBuilder.Append("--");
				}
				stringBuilder.AppendFormat("{0}", option.LongName);
				if (option.HasMetaValue)
				{
					stringBuilder.AppendFormat("={0}", option.MetaValue);
				}
			}
			this._optionsHelp.Append((stringBuilder.Length < maxLength) ? stringBuilder.ToString().PadRight(maxLength) : stringBuilder.ToString());
			this._optionsHelp.Append("    ");
			if (option.HasDefaultValue)
			{
				option.HelpText = "(Default: {0}) ".FormatLocal(new object[]
				{
					option.DefaultValue
				}) + option.HelpText;
			}
			if (option.Required)
			{
				option.HelpText = "{0} ".FormatInvariant(new object[]
				{
					requiredWord
				}) + option.HelpText;
			}
			if (fireEvent)
			{
				FormatOptionHelpTextEventArgs formatOptionHelpTextEventArgs = new FormatOptionHelpTextEventArgs(option);
				this.OnFormatOptionHelpText(formatOptionHelpTextEventArgs);
				option.HelpText = formatOptionHelpTextEventArgs.Option.HelpText;
			}
			if (!string.IsNullOrEmpty(option.HelpText))
			{
				do
				{
					int num = 0;
					string[] array = option.HelpText.Split(new char[]
					{
						' '
					});
					int i = 0;
					while (i < array.Length)
					{
						if (array[i].Length < widthOfHelpText - num)
						{
							this._optionsHelp.Append(array[i]);
							num += array[i].Length;
							if (widthOfHelpText - num > 1 && i != array.Length - 1)
							{
								this._optionsHelp.Append(" ");
								num++;
							}
							i++;
						}
						else
						{
							if (array[i].Length >= widthOfHelpText && num == 0)
							{
								this._optionsHelp.Append(array[i].Substring(0, widthOfHelpText));
								num = widthOfHelpText;
								break;
							}
							break;
						}
					}
					option.HelpText = option.HelpText.Substring(Math.Min(num, option.HelpText.Length)).Trim();
					if (option.HelpText.Length > 0)
					{
						this._optionsHelp.Append(Environment.NewLine);
						this._optionsHelp.Append(new string(' ', maxLength + 6));
					}
				}
				while (option.HelpText.Length > widthOfHelpText);
			}
			this._optionsHelp.Append(option.HelpText);
			this._optionsHelp.Append(Environment.NewLine);
			if (this._additionalNewLineAfterOption)
			{
				this._optionsHelp.Append(Environment.NewLine);
			}
		}
		private void AddLine(StringBuilder builder, string value)
		{
			Assumes.NotNull<string>(value, "value");
			HelpText.AddLine(builder, value, this.MaximumDisplayWidth);
		}
		private int GetMaxLength(IEnumerable<BaseOptionAttribute> optionList)
		{
			int num = 0;
			foreach (BaseOptionAttribute current in optionList)
			{
				int num2 = 0;
				bool hasShortName = current.HasShortName;
				bool hasLongName = current.HasLongName;
				int num3 = 0;
				if (current.HasMetaValue)
				{
					num3 = current.MetaValue.Length + 1;
				}
				if (hasShortName)
				{
					num2++;
					if (this.AddDashesToOption)
					{
						num2++;
					}
					num2 += num3;
				}
				if (hasLongName)
				{
					num2 += current.LongName.Length;
					if (this.AddDashesToOption)
					{
						num2 += 2;
					}
					num2 += num3;
				}
				if (hasShortName && hasLongName)
				{
					num2 += 2;
				}
				num = Math.Max(num, num2);
			}
			return num;
		}
	}
}
