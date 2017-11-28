using System;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ParserStateAttribute : Attribute
	{
	}
}
