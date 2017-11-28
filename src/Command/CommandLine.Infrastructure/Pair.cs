using System;
namespace CommandLine.Infrastructure
{
	internal sealed class Pair<TLeft, TRight> where TLeft : class where TRight : class
	{
		private readonly TLeft _left;
		private readonly TRight _right;
		public TLeft Left
		{
			get
			{
				return this._left;
			}
		}
		public TRight Right
		{
			get
			{
				return this._right;
			}
		}
		public Pair(TLeft left, TRight right)
		{
			this._left = left;
			this._right = right;
		}
		public override int GetHashCode()
		{
			int arg_24_0;
			if (this._left != null)
			{
				TLeft left = this._left;
				arg_24_0 = left.GetHashCode();
			}
			else
			{
				arg_24_0 = 0;
			}
			int num = arg_24_0;
			int arg_49_0;
			if (this._right != null)
			{
				TRight right = this._right;
				arg_49_0 = right.GetHashCode();
			}
			else
			{
				arg_49_0 = 0;
			}
			int num2 = arg_49_0;
			return num ^ num2;
		}
		public override bool Equals(object obj)
		{
			Pair<TLeft, TRight> pair = obj as Pair<TLeft, TRight>;
			return pair != null && object.Equals(this._left, pair._left) && object.Equals(this._right, pair._right);
		}
	}
}
