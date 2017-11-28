using System;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionArrayAttribute : BaseOptionAttribute
	{
		public OptionArrayAttribute()
		{
			base.AutoLongName = true;
		}
		public OptionArrayAttribute(char shortName) : base(shortName, null)
		{
		}
		public OptionArrayAttribute(string longName) : base(null, longName)
		{
		}
		public OptionArrayAttribute(char shortName, string longName) : base(shortName, longName)
		{
		}
	}
}
