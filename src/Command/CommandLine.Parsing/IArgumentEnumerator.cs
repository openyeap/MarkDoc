using System;
namespace CommandLine.Parsing
{
	internal interface IArgumentEnumerator
	{
		string Current
		{
			get;
		}
		string Next
		{
			get;
		}
		bool IsLast
		{
			get;
		}
		bool MoveNext();
		bool MovePrevious();
		string GetRemainingFromNext();
	}
}
