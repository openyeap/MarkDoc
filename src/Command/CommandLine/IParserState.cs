using System;
using System.Collections.Generic;
namespace CommandLine
{
	public interface IParserState
	{
		IList<ParsingError> Errors
		{
			get;
		}
	}
}
