using CommandLine.Infrastructure;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
namespace CommandLine
{
	public sealed class ParserSettings
	{
		private const bool CaseSensitiveDefault = true;
		private bool _disposed;
		private bool _caseSensitive;
		private bool _mutuallyExclusive;
		private bool _ignoreUnknownArguments;
		private TextWriter _helpWriter;
		private CultureInfo _parsingCulture;
		public bool CaseSensitive
		{
			get
			{
				return this._caseSensitive;
			}
			set
			{
				PopsicleSetter.Set<bool>(this.Consumed, ref this._caseSensitive, value);
			}
		}
		public bool MutuallyExclusive
		{
			get
			{
				return this._mutuallyExclusive;
			}
			set
			{
				PopsicleSetter.Set<bool>(this.Consumed, ref this._mutuallyExclusive, value);
			}
		}
		public TextWriter HelpWriter
		{
			get
			{
				return this._helpWriter;
			}
			set
			{
				PopsicleSetter.Set<TextWriter>(this.Consumed, ref this._helpWriter, value);
			}
		}
		public bool IgnoreUnknownArguments
		{
			get
			{
				return this._ignoreUnknownArguments;
			}
			set
			{
				PopsicleSetter.Set<bool>(this.Consumed, ref this._ignoreUnknownArguments, value);
			}
		}
		public CultureInfo ParsingCulture
		{
			get
			{
				return this._parsingCulture;
			}
			set
			{
				PopsicleSetter.Set<CultureInfo>(this.Consumed, ref this._parsingCulture, value);
			}
		}
		internal bool Consumed
		{
			get;
			set;
		}
		public ParserSettings() : this(true, false, false, null)
		{
		}
		public ParserSettings(bool caseSensitive) : this(caseSensitive, false, false, null)
		{
		}
		public ParserSettings(TextWriter helpWriter) : this(true, false, false, helpWriter)
		{
		}
		public ParserSettings(bool caseSensitive, TextWriter helpWriter) : this(caseSensitive, false, false, helpWriter)
		{
		}
		public ParserSettings(bool caseSensitive, bool mutuallyExclusive) : this(caseSensitive, mutuallyExclusive, false, null)
		{
		}
		public ParserSettings(bool caseSensitive, bool mutuallyExclusive, TextWriter helpWriter) : this(caseSensitive, mutuallyExclusive, false, helpWriter)
		{
		}
		public ParserSettings(bool caseSensitive, bool mutuallyExclusive, bool ignoreUnknownArguments, TextWriter helpWriter)
		{
			this.CaseSensitive = caseSensitive;
			this.MutuallyExclusive = mutuallyExclusive;
			this.HelpWriter = helpWriter;
			this.IgnoreUnknownArguments = ignoreUnknownArguments;
			this.ParsingCulture = Thread.CurrentThread.CurrentCulture;
		}
		~ParserSettings()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing)
			{
				if (this._helpWriter != null)
				{
					this._helpWriter.Dispose();
					this._helpWriter = null;
				}
				this._disposed = true;
			}
		}
	}
}
