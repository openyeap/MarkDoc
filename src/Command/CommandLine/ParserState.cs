using System;
using System.Collections.Generic;
namespace CommandLine
{
	public sealed class ParserState : IParserState
	{
		public IList<ParsingError> Errors
		{
			get;
			private set;
		}
		internal ParserState()
		{
			this.Errors = new List<ParsingError>();
		}
	}
}
