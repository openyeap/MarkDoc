using CommandLine.Infrastructure;
using System;
namespace CommandLine.Parsing
{
	internal sealed class StringArrayEnumerator : IArgumentEnumerator
	{
		private readonly int _endIndex;
		private readonly string[] _data;
		private int _index;
		public string Current
		{
			get
			{
				if (this._index == -1)
				{
					throw new InvalidOperationException();
				}
				if (this._index >= this._endIndex)
				{
					throw new InvalidOperationException();
				}
				return this._data[this._index];
			}
		}
		public string Next
		{
			get
			{
				if (this._index == -1)
				{
					throw new InvalidOperationException();
				}
				if (this._index > this._endIndex)
				{
					throw new InvalidOperationException();
				}
				if (this.IsLast)
				{
					return null;
				}
				return this._data[this._index + 1];
			}
		}
		public bool IsLast
		{
			get
			{
				return this._index == this._endIndex - 1;
			}
		}
		public StringArrayEnumerator(string[] value)
		{
			Assumes.NotNull<string[]>(value, "value");
			this._data = value;
			this._index = -1;
			this._endIndex = value.Length;
		}
		public bool MoveNext()
		{
			if (this._index < this._endIndex)
			{
				this._index++;
				return this._index < this._endIndex;
			}
			return false;
		}
		public string GetRemainingFromNext()
		{
			throw new NotSupportedException();
		}
		public bool MovePrevious()
		{
			if (this._index <= 0)
			{
				throw new InvalidOperationException();
			}
			if (this._index <= this._endIndex)
			{
				this._index--;
				return this._index <= this._endIndex;
			}
			return false;
		}
	}
}
