using System;
namespace CommandLine
{
	public sealed class ParsingError
	{
		public BadOptionInfo BadOption
		{
			get;
			private set;
		}
		public bool ViolatesRequired
		{
			get;
			set;
		}
		public bool ViolatesFormat
		{
			get;
			set;
		}
		public bool ViolatesMutualExclusiveness
		{
			get;
			set;
		}
		internal ParsingError()
		{
			this.BadOption = new BadOptionInfo();
		}
		internal ParsingError(char? shortName, string longName, bool format)
		{
			this.BadOption = new BadOptionInfo(shortName, longName);
			this.ViolatesFormat = format;
		}
	}
}
