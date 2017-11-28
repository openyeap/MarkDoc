using System;
namespace CommandLine.Text
{
	public abstract class BaseSentenceBuilder
	{
		public abstract string OptionWord
		{
			get;
		}
		public abstract string AndWord
		{
			get;
		}
		public abstract string RequiredOptionMissingText
		{
			get;
		}
		public abstract string ViolatesFormatText
		{
			get;
		}
		public abstract string ViolatesMutualExclusivenessText
		{
			get;
		}
		public abstract string ErrorsHeadingText
		{
			get;
		}
		public static BaseSentenceBuilder CreateBuiltIn()
		{
			return new EnglishSentenceBuilder();
		}
	}
}
