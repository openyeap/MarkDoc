using System;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionListAttribute : BaseOptionAttribute
	{
		private const char DefaultSeparator = ':';
		public char Separator
		{
			get;
			set;
		}
		public OptionListAttribute()
		{
			base.AutoLongName = true;
			this.Separator = ':';
		}
		public OptionListAttribute(char shortName) : base(shortName, null)
		{
		}
		public OptionListAttribute(string longName) : base(null, longName)
		{
		}
		public OptionListAttribute(char shortName, string longName) : base(shortName, longName)
		{
			this.Separator = ':';
		}
		public OptionListAttribute(char shortName, string longName, char separator) : base(shortName, longName)
		{
			this.Separator = separator;
		}
	}
}
