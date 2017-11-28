using CommandLine.Infrastructure;
using System;
namespace CommandLine.Parsing
{
	internal sealed class OneCharStringEnumerator : IArgumentEnumerator
	{
		private readonly string _data;
		private string _currentElement;
		private int _index;
		public string Current
		{
			get
			{
				if (this._index == -1)
				{
					throw new InvalidOperationException();
				}
				if (this._index >= this._data.Length)
				{
					throw new InvalidOperationException();
				}
				return this._currentElement;
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
				if (this._index > this._data.Length)
				{
					throw new InvalidOperationException();
				}
				if (this.IsLast)
				{
					return null;
				}
				return this._data.Substring(this._index + 1, 1);
			}
		}
		public bool IsLast
		{
			get
			{
				return this._index == this._data.Length - 1;
			}
		}
		public OneCharStringEnumerator(string value)
		{
			Assumes.NotNullOrEmpty(value, "value");
			this._data = value;
			this._index = -1;
		}
		public bool MoveNext()
		{
			if (this._index < this._data.Length - 1)
			{
				this._index++;
				this._currentElement = this._data.Substring(this._index, 1);
				return true;
			}
			this._index = this._data.Length;
			return false;
		}
		public string GetRemainingFromNext()
		{
			if (this._index == -1)
			{
				throw new InvalidOperationException();
			}
			if (this._index > this._data.Length)
			{
				throw new InvalidOperationException();
			}
			return this._data.Substring(this._index + 1);
		}
		public bool MovePrevious()
		{
			throw new NotSupportedException();
		}
	}
}
