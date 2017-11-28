using CommandLine.Extensions;
using CommandLine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace CommandLine.Parsing
{
	internal sealed class OptionMap
	{
		private sealed class MutuallyExclusiveInfo
		{
			private int _count;
			public OptionInfo BadOption
			{
				get;
				private set;
			}
			public int Occurrence
			{
				get
				{
					return this._count;
				}
			}
			public MutuallyExclusiveInfo(OptionInfo option)
			{
				this.BadOption = option;
			}
			public void IncrementOccurrence()
			{
				this._count++;
			}
		}
		private readonly ParserSettings _settings;
		private readonly Dictionary<string, string> _names;
		private readonly Dictionary<string, OptionInfo> _map;
		private readonly Dictionary<string, OptionMap.MutuallyExclusiveInfo> _mutuallyExclusiveSetMap;
		internal object RawOptions
		{
			private get;
			set;
		}
		public OptionInfo this[string key]
		{
			get
			{
				OptionInfo result = null;
				if (this._map.ContainsKey(key))
				{
					result = this._map[key];
				}
				else
				{
					if (this._names.ContainsKey(key))
					{
						string key2 = this._names[key];
						result = this._map[key2];
					}
				}
				return result;
			}
			set
			{
				this._map[key] = value;
				if (value.HasBothNames)
				{
					this._names[value.LongName] = new string(value.ShortName.Value, 1);
				}
			}
		}
		internal OptionMap(int capacity, ParserSettings settings)
		{
			this._settings = settings;
			IEqualityComparer<string> comparer = this._settings.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			this._names = new Dictionary<string, string>(capacity, comparer);
			this._map = new Dictionary<string, OptionInfo>(capacity * 2, comparer);
			if (this._settings.MutuallyExclusive)
			{
				this._mutuallyExclusiveSetMap = new Dictionary<string, OptionMap.MutuallyExclusiveInfo>(capacity, StringComparer.OrdinalIgnoreCase);
			}
		}
		public static OptionMap Create(object target, ParserSettings settings)
		{
			IList<Pair<PropertyInfo, BaseOptionAttribute>> list = ReflectionHelper.RetrievePropertyList<BaseOptionAttribute>(target);
			if (list == null)
			{
				return null;
			}
			OptionMap optionMap = new OptionMap(list.Count, settings);
			foreach (Pair<PropertyInfo, BaseOptionAttribute> current in list)
			{
				if (current.Left != null && current.Right != null)
				{
					string text;
					if (current.Right.AutoLongName)
					{
						text = current.Left.Name.ToLowerInvariant();
						current.Right.LongName = text;
					}
					else
					{
						text = current.Right.UniqueName;
					}
					optionMap[text] = new OptionInfo(current.Right, current.Left, settings.ParsingCulture);
				}
			}
			optionMap.RawOptions = target;
			return optionMap;
		}
		public static OptionMap Create(object target, IList<Pair<PropertyInfo, VerbOptionAttribute>> verbs, ParserSettings settings)
		{
			OptionMap optionMap = new OptionMap(verbs.Count, settings);
			foreach (Pair<PropertyInfo, VerbOptionAttribute> current in verbs)
			{
				OptionInfo optionInfo = new OptionInfo(current.Right, current.Left, settings.ParsingCulture)
				{
					HasParameterLessCtor = current.Left.PropertyType.GetConstructor(Type.EmptyTypes) != null
				};
				if (!optionInfo.HasParameterLessCtor && current.Left.GetValue(target, null) == null)
				{
					throw new ParserException("Type {0} must have a parameterless constructor or" + " be already initialized to be used as a verb command.".FormatInvariant(new object[]
					{
						current.Left.PropertyType
					}));
				}
				optionMap[current.Right.UniqueName] = optionInfo;
			}
			optionMap.RawOptions = target;
			return optionMap;
		}
		public bool EnforceRules()
		{
			return this.EnforceMutuallyExclusiveMap() && this.EnforceRequiredRule();
		}
		public void SetDefaults()
		{
			foreach (OptionInfo current in this._map.Values)
			{
				current.SetDefault(this.RawOptions);
			}
		}
		private static void SetParserStateIfNeeded(object options, OptionInfo option, bool? required, bool? mutualExclusiveness)
		{
			IList<Pair<PropertyInfo, ParserStateAttribute>> list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
			if (list.Count == 0)
			{
				return;
			}
			PropertyInfo left = list[0].Left;
			if (left.GetValue(options, null) == null)
			{
				left.SetValue(options, new ParserState(), null);
			}
			IParserState parserState = (IParserState)left.GetValue(options, null);
			if (parserState == null)
			{
				return;
			}
			ParsingError parsingError = new ParsingError
			{
				BadOption = 
				{
					ShortName = option.ShortName,
					LongName = option.LongName
				}
			};
			if (required.HasValue)
			{
				parsingError.ViolatesRequired = required.Value;
			}
			if (mutualExclusiveness.HasValue)
			{
				parsingError.ViolatesMutualExclusiveness = mutualExclusiveness.Value;
			}
			parserState.Errors.Add(parsingError);
		}
		private bool EnforceRequiredRule()
		{
			bool result = true;
			foreach (OptionInfo current in this._map.Values)
			{
				if (current.Required && (!current.IsDefined || !current.ReceivedValue))
				{
					OptionMap.SetParserStateIfNeeded(this.RawOptions, current, new bool?(true), null);
					result = false;
				}
			}
			return result;
		}
		private bool EnforceMutuallyExclusiveMap()
		{
			if (!this._settings.MutuallyExclusive)
			{
				return true;
			}
			foreach (OptionInfo current in this._map.Values)
			{
				if (current.IsDefined && current.MutuallyExclusiveSet != null)
				{
					this.BuildMutuallyExclusiveMap(current);
				}
			}
			foreach (OptionMap.MutuallyExclusiveInfo current2 in this._mutuallyExclusiveSetMap.Values)
			{
				if (current2.Occurrence > 1)
				{
					OptionMap.SetParserStateIfNeeded(this.RawOptions, current2.BadOption, null, new bool?(true));
					return false;
				}
			}
			return true;
		}
		private void BuildMutuallyExclusiveMap(OptionInfo option)
		{
			string mutuallyExclusiveSet = option.MutuallyExclusiveSet;
			if (!this._mutuallyExclusiveSetMap.ContainsKey(mutuallyExclusiveSet))
			{
				this._mutuallyExclusiveSetMap.Add(mutuallyExclusiveSet, new OptionMap.MutuallyExclusiveInfo(option));
			}
			this._mutuallyExclusiveSetMap[mutuallyExclusiveSet].IncrementOccurrence();
		}
	}
}
