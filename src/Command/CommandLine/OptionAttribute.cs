using CommandLine.Parsing;
using System;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionAttribute : BaseOptionAttribute
	{
		public OptionAttribute()
		{
			base.AutoLongName = true;
		}
		public OptionAttribute(char shortName) : base(shortName, null)
		{
		}
		public OptionAttribute(string longName) : base(null, longName)
		{
		}
		public OptionAttribute(char shortName, string longName) : base(shortName, longName)
		{
		}
		internal OptionInfo CreateOptionInfo()
		{
			return new OptionInfo(this.ShortName, base.LongName);
		}
	}
}
