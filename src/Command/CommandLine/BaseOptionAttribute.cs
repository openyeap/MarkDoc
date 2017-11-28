using CommandLine.Extensions;
using System;
namespace CommandLine
{
	public abstract class BaseOptionAttribute : Attribute
	{
		internal const string DefaultMutuallyExclusiveSet = "Default";
		private char? _shortName;
		private object _defaultValue;
		private string _metaValue;
		private bool _hasMetaValue;
		private string _mutuallyExclusiveSet;
		public virtual char? ShortName
		{
			get
			{
				return this._shortName;
			}
			internal set
			{
				this._shortName = value;
			}
		}
		public string LongName
		{
			get;
			internal set;
		}
		public string MutuallyExclusiveSet
		{
			get
			{
				return this._mutuallyExclusiveSet;
			}
			set
			{
				this._mutuallyExclusiveSet = (string.IsNullOrEmpty(value) ? "Default" : value);
			}
		}
		public virtual bool Required
		{
			get;
			set;
		}
		public virtual object DefaultValue
		{
			get
			{
				return this._defaultValue;
			}
			set
			{
				this._defaultValue = value;
				this.HasDefaultValue = true;
			}
		}
		public virtual string MetaValue
		{
			get
			{
				return this._metaValue;
			}
			set
			{
				this._metaValue = value;
				this._hasMetaValue = !string.IsNullOrEmpty(this._metaValue);
			}
		}
		public string HelpText
		{
			get;
			set;
		}
		internal string UniqueName
		{
			get;
			private set;
		}
		internal bool HasShortName
		{
			get
			{
				char? shortName = this._shortName;
				return (shortName.HasValue ? new int?((int)shortName.GetValueOrDefault()) : null).HasValue;
			}
		}
		internal bool HasLongName
		{
			get
			{
				return !string.IsNullOrEmpty(this.LongName);
			}
		}
		internal bool HasDefaultValue
		{
			get;
			private set;
		}
		internal bool HasMetaValue
		{
			get
			{
				return this._hasMetaValue;
			}
		}
		internal bool AutoLongName
		{
			get;
			set;
		}
		protected BaseOptionAttribute()
		{
		}
		protected BaseOptionAttribute(char shortName, string longName)
		{
			this._shortName = new char?(shortName);
			if (this._shortName.Value.IsWhiteSpace() || this._shortName.Value.IsLineTerminator())
			{
				throw new ArgumentException("shortName with whitespace or line terminator character is not allowed.", "shortName");
			}
			this.UniqueName = new string(shortName, 1);
			this.LongName = longName;
		}
		protected BaseOptionAttribute(char? shortName, string longName)
		{
			this._shortName = shortName;
			char? shortName2 = this._shortName;
			if ((shortName2.HasValue ? new int?((int)shortName2.GetValueOrDefault()) : null).HasValue)
			{
				if (this._shortName.Value.IsWhiteSpace() || this._shortName.Value.IsLineTerminator())
				{
					throw new ArgumentException("shortName with whitespace or line terminator character is not allowed.", "shortName");
				}
				this.UniqueName = new string(this._shortName.Value, 1);
			}
			this.LongName = longName;
			if (this.UniqueName != null)
			{
				return;
			}
			if (this.LongName == null)
			{
				throw new ArgumentNullException("longName", "The option must have short, long name or both.");
			}
			this.UniqueName = this.LongName;
		}
	}
}
