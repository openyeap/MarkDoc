using CommandLine.Infrastructure;
using System;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class VerbOptionAttribute : BaseOptionAttribute
	{
		public override char? ShortName
		{
			get
			{
				return null;
			}
			internal set
			{
				throw new InvalidOperationException("Verb commands do not support short name by design.");
			}
		}
		public override bool Required
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException("Verb commands cannot be mandatory since are mutually exclusive by design.");
			}
		}
		public VerbOptionAttribute(string longName) : base(null, longName)
		{
			Assumes.NotNullOrEmpty(longName, "longName");
		}
	}
}
