using CommandLine.Infrastructure;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
namespace CommandLine.Text
{
	public class CopyrightInfo
	{
		private const string DefaultCopyrightWord = "Copyright";
		private const string SymbolLower = "(c)";
		private const string SymbolUpper = "(C)";
		private readonly AssemblyCopyrightAttribute _attribute;
		private readonly bool _isSymbolUpper;
		private readonly int[] _copyrightYears;
		private readonly string _author;
		private readonly int _builderSize;
		public static CopyrightInfo Default
		{
			get
			{
				AssemblyCopyrightAttribute attribute = ReflectionHelper.GetAttribute<AssemblyCopyrightAttribute>();
				if (attribute != null)
				{
					return new CopyrightInfo(attribute);
				}
				AssemblyCompanyAttribute attribute2 = ReflectionHelper.GetAttribute<AssemblyCompanyAttribute>();
				if (attribute2 != null)
				{
					return new CopyrightInfo(attribute2.Company, DateTime.Now.Year);
				}
				throw new InvalidOperationException("CopyrightInfo::Default requires that you define AssemblyCopyrightAttribute or AssemblyCompanyAttribute.");
			}
		}
		protected virtual string CopyrightWord
		{
			get
			{
				return "Copyright";
			}
		}
		public CopyrightInfo(string author, int year) : this(true, author, new int[]
		{
			year
		})
		{
		}
		public CopyrightInfo(string author, params int[] years) : this(true, author, years)
		{
		}
		public CopyrightInfo(bool isSymbolUpper, string author, params int[] copyrightYears)
		{
			Assumes.NotNullOrEmpty(author, "author");
			Assumes.NotZeroLength<int>(copyrightYears, "copyrightYears");
			this._isSymbolUpper = isSymbolUpper;
			this._author = author;
			this._copyrightYears = copyrightYears;
			this._builderSize = 12 + author.Length + 4 * copyrightYears.Length + 10;
		}
		protected CopyrightInfo()
		{
		}
		private CopyrightInfo(AssemblyCopyrightAttribute attribute)
		{
			this._attribute = attribute;
		}
		public static implicit operator string(CopyrightInfo info)
		{
			return info.ToString();
		}
		public override string ToString()
		{
			if (this._attribute != null)
			{
				return this._attribute.Copyright;
			}
			StringBuilder stringBuilder = new StringBuilder(this._builderSize);
			stringBuilder.Append(this.CopyrightWord);
			stringBuilder.Append(' ');
			stringBuilder.Append(this._isSymbolUpper ? "(C)" : "(c)");
			stringBuilder.Append(' ');
			stringBuilder.Append(this.FormatYears(this._copyrightYears));
			stringBuilder.Append(' ');
			stringBuilder.Append(this._author);
			return stringBuilder.ToString();
		}
		protected virtual string FormatYears(int[] years)
		{
			if (years.Length == 1)
			{
				return years[0].ToString(CultureInfo.InvariantCulture);
			}
			StringBuilder stringBuilder = new StringBuilder(years.Length * 6);
			for (int i = 0; i < years.Length; i++)
			{
				stringBuilder.Append(years[i].ToString(CultureInfo.InvariantCulture));
				int num = i + 1;
				if (num < years.Length)
				{
					stringBuilder.Append((years[num] - years[i] > 1) ? " - " : ", ");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
