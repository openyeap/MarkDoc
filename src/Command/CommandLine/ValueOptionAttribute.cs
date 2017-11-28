using System;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValueOptionAttribute : Attribute
	{
		private readonly int _index;
		public int Index
		{
			get
			{
				return this._index;
			}
		}
		public ValueOptionAttribute(int index)
		{
			this._index = index;
		}
	}
}
