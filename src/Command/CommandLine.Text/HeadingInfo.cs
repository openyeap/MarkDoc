using CommandLine.Infrastructure;
using System;
using System.IO;
using System.Reflection;
using System.Text;
namespace CommandLine.Text
{
	public class HeadingInfo
	{
		private readonly string _programName;
		private readonly string _version;
		public static HeadingInfo Default
		{
			get
			{
				AssemblyTitleAttribute attribute = ReflectionHelper.GetAttribute<AssemblyTitleAttribute>();
				string programName = (attribute == null) ? ReflectionHelper.AssemblyFromWhichToPullInformation.GetName().Name : Path.GetFileNameWithoutExtension(attribute.Title);
				AssemblyInformationalVersionAttribute attribute2 = ReflectionHelper.GetAttribute<AssemblyInformationalVersionAttribute>();
				string version = (attribute2 == null) ? ReflectionHelper.AssemblyFromWhichToPullInformation.GetName().Version.ToString() : attribute2.InformationalVersion;
				return new HeadingInfo(programName, version);
			}
		}
		public HeadingInfo(string programName) : this(programName, null)
		{
		}
		public HeadingInfo(string programName, string version)
		{
			Assumes.NotNullOrEmpty(programName, "programName");
			this._programName = programName;
			this._version = version;
		}
		public static implicit operator string(HeadingInfo info)
		{
			return info.ToString();
		}
		public override string ToString()
		{
			bool flag = string.IsNullOrEmpty(this._version);
			StringBuilder stringBuilder = new StringBuilder(this._programName.Length + ((!flag) ? (this._version.Length + 1) : 0));
			stringBuilder.Append(this._programName);
			if (!flag)
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(this._version);
			}
			return stringBuilder.ToString();
		}
		public void WriteMessage(string message, TextWriter writer)
		{
			Assumes.NotNullOrEmpty(message, "message");
			Assumes.NotNull<TextWriter>(writer, "writer");
			StringBuilder stringBuilder = new StringBuilder(this._programName.Length + message.Length + 2);
			stringBuilder.Append(this._programName);
			stringBuilder.Append(": ");
			stringBuilder.Append(message);
			writer.WriteLine(stringBuilder.ToString());
		}
		public void WriteMessage(string message)
		{
			this.WriteMessage(message, Console.Out);
		}
		public void WriteError(string message)
		{
			this.WriteMessage(message, Console.Error);
		}
	}
}
