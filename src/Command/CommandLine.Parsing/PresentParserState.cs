using System;
namespace CommandLine.Parsing
{
	[Flags]
	internal enum PresentParserState : ushort
	{
		Undefined = 0,
		Success = 1,
		Failure = 2,
		MoveOnNextElement = 4
	}
}
