using System;
namespace CommandLine.Text
{
	public class EnglishSentenceBuilder : BaseSentenceBuilder
	{
		public override string OptionWord
		{
			get
			{
				return "option";
			}
		}
		public override string AndWord
		{
			get
			{
				return "and";
			}
		}
		public override string RequiredOptionMissingText
		{
			get
			{
				return "required option is missing";
			}
		}
		public override string ViolatesFormatText
		{
			get
			{
				return "violates format";
			}
		}
		public override string ViolatesMutualExclusivenessText
		{
			get
			{
				return "violates mutual exclusiveness";
			}
		}
		public override string ErrorsHeadingText
		{
			get
			{
				return "ERROR(S):";
			}
		}
	}
}
