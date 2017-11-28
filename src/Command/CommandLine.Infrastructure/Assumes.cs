using System;
namespace CommandLine.Infrastructure
{
	internal static class Assumes
	{
		public static void NotNull<T>(T value, string paramName) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}
		public static void NotNull<T>(T value, string paramName, string message) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName, message);
			}
		}
		public static void NotNullOrEmpty(string value, string paramName)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(paramName);
			}
		}
		public static void NotZeroLength<T>(T[] array, string paramName)
		{
			if (array.Length == 0)
			{
				throw new ArgumentOutOfRangeException(paramName);
			}
		}
	}
}
