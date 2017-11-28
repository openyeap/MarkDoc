using CommandLine.Infrastructure;
using System;
namespace CommandLine.Parsing
{
	internal static class TargetCapabilitiesExtensions
	{
		public static bool HasVerbs(this object target)
		{
			return ReflectionHelper.RetrievePropertyList<VerbOptionAttribute>(target).Count > 0;
		}
		public static bool HasHelp(this object target)
		{
			return ReflectionHelper.RetrieveMethod<HelpOptionAttribute>(target) != null;
		}
		public static bool HasVerbHelp(this object target)
		{
			return ReflectionHelper.RetrieveMethod<HelpVerbOptionAttribute>(target) != null;
		}
		public static bool CanReceiveParserState(this object target)
		{
			return ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(target).Count > 0;
		}
	}
}
