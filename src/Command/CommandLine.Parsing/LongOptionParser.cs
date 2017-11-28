using System;
using System.Collections.Generic;
namespace CommandLine.Parsing
{
	internal sealed class LongOptionParser : ArgumentParser
	{
		private readonly bool _ignoreUnkwnownArguments;
		public LongOptionParser(bool ignoreUnkwnownArguments)
		{
			this._ignoreUnkwnownArguments = ignoreUnkwnownArguments;
		}
		public override PresentParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options)
		{
			string[] array = argumentEnumerator.Current.Substring(2).Split(new char[]
			{
				'='
			}, 2);
			OptionInfo optionInfo = map[array[0]];
			if (optionInfo == null)
			{
				if (!this._ignoreUnkwnownArguments)
				{
					return PresentParserState.Failure;
				}
				return PresentParserState.MoveOnNextElement;
			}
			else
			{
				optionInfo.IsDefined = true;
				ArgumentParser.EnsureOptionArrayAttributeIsNotBoundToScalar(optionInfo);
				if (!optionInfo.IsBoolean)
				{
					if (array.Length == 1 && (argumentEnumerator.IsLast || !ArgumentParser.IsInputValue(argumentEnumerator.Next)))
					{
						return PresentParserState.Failure;
					}
					if (array.Length == 2)
					{
						bool flag;
						if (!optionInfo.IsArray)
						{
							flag = optionInfo.SetValue(array[1], options);
							if (!flag)
							{
								base.DefineOptionThatViolatesFormat(optionInfo);
							}
							return ArgumentParser.BooleanToParserState(flag);
						}
						ArgumentParser.EnsureOptionAttributeIsArrayCompatible(optionInfo);
						IList<string> nextInputValues = ArgumentParser.GetNextInputValues(argumentEnumerator);
						nextInputValues.Insert(0, array[1]);
						flag = optionInfo.SetValue(nextInputValues, options);
						if (!flag)
						{
							base.DefineOptionThatViolatesFormat(optionInfo);
						}
						return ArgumentParser.BooleanToParserState(flag);
					}
					else
					{
						bool flag;
						if (!optionInfo.IsArray)
						{
							flag = optionInfo.SetValue(argumentEnumerator.Next, options);
							if (!flag)
							{
								base.DefineOptionThatViolatesFormat(optionInfo);
							}
							return ArgumentParser.BooleanToParserState(flag, true);
						}
						ArgumentParser.EnsureOptionAttributeIsArrayCompatible(optionInfo);
						IList<string> nextInputValues2 = ArgumentParser.GetNextInputValues(argumentEnumerator);
						flag = optionInfo.SetValue(nextInputValues2, options);
						if (!flag)
						{
							base.DefineOptionThatViolatesFormat(optionInfo);
						}
						return ArgumentParser.BooleanToParserState(flag);
					}
				}
				else
				{
					if (array.Length == 2)
					{
						return PresentParserState.Failure;
					}
					bool flag = optionInfo.SetValue(true, options);
					if (!flag)
					{
						base.DefineOptionThatViolatesFormat(optionInfo);
					}
					return ArgumentParser.BooleanToParserState(flag);
				}
			}
		}
	}
}
