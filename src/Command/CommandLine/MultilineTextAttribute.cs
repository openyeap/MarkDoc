using CommandLine.Infrastructure;
using CommandLine.Text;
using System;
using System.Text;
namespace CommandLine
{
	public abstract class MultilineTextAttribute : Attribute
	{
		private readonly string _line1;
		private readonly string _line2;
		private readonly string _line3;
		private readonly string _line4;
		private readonly string _line5;
		public virtual string Value
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(string.Empty);
				string[] array = new string[]
				{
					this._line1,
					this._line2,
					this._line3,
					this._line4,
					this._line5
				};
				for (int i = 0; i < this.GetLastLineWithText(array); i++)
				{
					stringBuilder.AppendLine(array[i]);
				}
				return stringBuilder.ToString();
			}
		}
		public string Line1
		{
			get
			{
				return this._line1;
			}
		}
		public string Line2
		{
			get
			{
				return this._line2;
			}
		}
		public string Line3
		{
			get
			{
				return this._line3;
			}
		}
		public string Line4
		{
			get
			{
				return this._line4;
			}
		}
		public string Line5
		{
			get
			{
				return this._line5;
			}
		}
		protected MultilineTextAttribute(string line1)
		{
			Assumes.NotNullOrEmpty(line1, "line1");
			this._line1 = line1;
		}
		protected MultilineTextAttribute(string line1, string line2) : this(line1)
		{
			Assumes.NotNullOrEmpty(line2, "line2");
			this._line2 = line2;
		}
		protected MultilineTextAttribute(string line1, string line2, string line3) : this(line1, line2)
		{
			Assumes.NotNullOrEmpty(line3, "line3");
			this._line3 = line3;
		}
		protected MultilineTextAttribute(string line1, string line2, string line3, string line4) : this(line1, line2, line3)
		{
			Assumes.NotNullOrEmpty(line4, "line4");
			this._line4 = line4;
		}
		protected MultilineTextAttribute(string line1, string line2, string line3, string line4, string line5) : this(line1, line2, line3, line4)
		{
			Assumes.NotNullOrEmpty(line5, "line5");
			this._line5 = line5;
		}
		internal void AddToHelpText(Action<string> action)
		{
			string[] array = new string[]
			{
				this._line1,
				this._line2,
				this._line3,
				this._line4,
				this._line5
			};
			Array.ForEach<string>(array, delegate(string line)
			{
				if (!string.IsNullOrEmpty(line))
				{
					action(line);
				}
			});
		}
		internal void AddToHelpText(HelpText helpText, bool before)
		{
			if (before)
			{
				this.AddToHelpText(new Action<string>(helpText.AddPreOptionsLine));
				return;
			}
			this.AddToHelpText(new Action<string>(helpText.AddPostOptionsLine));
		}
		protected virtual int GetLastLineWithText(string[] value)
		{
			int num = Array.FindLastIndex<string>(value, (string str) => !string.IsNullOrEmpty(str));
			return num + 1;
		}
	}
}
