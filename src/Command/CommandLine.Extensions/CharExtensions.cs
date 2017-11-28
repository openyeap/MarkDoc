using System;
namespace CommandLine.Extensions
{
	internal static class CharExtensions
	{
		public static bool IsWhiteSpace(this char c)
		{
			switch (c)
			{
			case '\t':
			case '\v':
			case '\f':
				break;
			case '\n':
				goto IL_22;
			default:
				if (c != ' ')
				{
					goto IL_22;
				}
				break;
			}
			return true;
			IL_22:
			return c > '\u007f' && char.IsWhiteSpace(c);
		}
		public static bool IsLineTerminator(this char c)
		{
			if (c != '\n' && c != '\r')
			{
				switch (c)
				{
				case '\u2028':
				case '\u2029':
					break;
				default:
					return false;
				}
			}
			return true;
		}
	}
}
