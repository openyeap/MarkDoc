using CommandLine.Extensions;
using System;
using System.Collections.Generic;
namespace CommandLine.Parsing
{
	internal abstract class ArgumentParser
	{
		public List<ParsingError> PostParsingState
		{
			get;
			private set;
		}
		protected ArgumentParser()
		{
			this.PostParsingState = new List<ParsingError>();
		}
		public static bool CompareShort(string argument, char? option, bool caseSensitive)
		{
			return string.Compare(argument, ArgumentParser.ToOption(option), caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
		}
		public static bool CompareLong(string argument, string option, bool caseSensitive)
		{
			return string.Compare(argument, ArgumentParser.ToOption(option), caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
		}
		public static ArgumentParser Create(string argument, bool ignoreUnknownArguments = false)
		{
			if (argument.IsNumeric())
			{
				return null;
			}
			if (ArgumentParser.IsDash(argument))
			{
				return null;
			}
			if (ArgumentParser.IsLongOption(argument))
			{
				return new LongOptionParser(ignoreUnknownArguments);
			}
			if (ArgumentParser.IsShortOption(argument))
			{
				return new OptionGroupParser(ignoreUnknownArguments);
			}
			return null;
		}
		public static bool IsInputValue(string argument)
		{
			return argument.IsNumeric() || argument.Length <= 0 || ArgumentParser.IsDash(argument) || !ArgumentParser.IsShortOption(argument);
		}
		public abstract PresentParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options);
		internal static IList<string> InternalWrapperOfGetNextInputValues(IArgumentEnumerator ae)
		{
			return ArgumentParser.GetNextInputValues(ae);
		}
		protected static IList<string> GetNextInputValues(IArgumentEnumerator ae)
		{
			IList<string> list = new List<string>();
			while (ae.MoveNext() && ArgumentParser.IsInputValue(ae.Current))
			{
				list.Add(ae.Current);
			}
			if (!ae.MovePrevious())
			{
				throw new ParserException();
			}
			return list;
		}
		protected static PresentParserState BooleanToParserState(bool value)
		{
			return ArgumentParser.BooleanToParserState(value, false);
		}
		protected static PresentParserState BooleanToParserState(bool value, bool addMoveNextIfTrue)
		{
			if (value && !addMoveNextIfTrue)
			{
				return PresentParserState.Success;
			}
			if (value)
			{
				return PresentParserState.Success | PresentParserState.MoveOnNextElement;
			}
			return PresentParserState.Failure;
		}
		protected static void EnsureOptionAttributeIsArrayCompatible(OptionInfo option)
		{
			if (!option.IsAttributeArrayCompatible)
			{
				throw new ParserException();
			}
		}
		protected static void EnsureOptionArrayAttributeIsNotBoundToScalar(OptionInfo option)
		{
			if (!option.IsArray && option.IsAttributeArrayCompatible)
			{
				throw new ParserException();
			}
		}
		protected void DefineOptionThatViolatesFormat(OptionInfo option)
		{
			this.PostParsingState.Add(new ParsingError(option.ShortName, option.LongName, true));
		}
		private static string ToOption(string value)
		{
			return "--" + value;
		}
		private static string ToOption(char? value)
		{
			return "-" + value;
		}
		private static bool IsDash(string value)
		{
			return string.CompareOrdinal(value, "-") == 0;
		}
		private static bool IsShortOption(string value)
		{
			return value[0] == '-';
		}
		private static bool IsLongOption(string value)
		{
			return value[0] == '-' && value[1] == '-';
		}
	}
}
