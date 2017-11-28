using System;
namespace CommandLine.Infrastructure
{
	internal static class SR
	{
		public const string ArgumentNullException_ArgsStringArrayCannotBeNull = "The arguments string array cannot be null.";
		public const string ArgumentNullException_OptionsInstanceCannotBeNull = "The target options instance cannot be null.";
		public const string ArgumentNullException_ParserSettingsInstanceCannotBeNull = "The command line parser settings instance cannot be null.";
		public const string ArgumentNullException_AttributeCannotBeNull = "The attribute is mandatory.";
		public const string ArgumentNullException_PropertyCannotBeNull = "The property is mandatory.";
		public const string CommandLineParserException_CannotCreateInstanceForVerbCommand = "Instance defined for verb command could not be created.";
		public const string ArgumentException_NoWhiteSpaceOrLineTerminatorInShortName = "shortName with whitespace or line terminator character is not allowed.";
		public const string ArgumentNullException_LongNameCannotBeNullWhenShortNameIsUndefined = "The option must have short, long name or both.";
		public const string MemberAccessException_BadSignatureForHelpVerbOptionAttribute = "{0} has an incorrect signature. Help verb command requires a method that accepts and returns a string.";
		public const string InvalidOperationException_DoNotUseShortNameForVerbCommands = "Verb commands do not support short name by design.";
		public const string InvalidOperationException_DoNotSetRequiredPropertyForVerbCommands = "Verb commands cannot be mandatory since are mutually exclusive by design.";
		public const string CommandLineParserException_IncompatibleTypes = "The types are incompatible.";
		public const string InvalidOperationException_CopyrightInfoRequiresAssemblyCopyrightAttributeOrAssemblyCompanyAttribute = "CopyrightInfo::Default requires that you define AssemblyCopyrightAttribute or AssemblyCompanyAttribute.";
		public const string ArgumentNullException_OnVerbDelegateCannotBeNull = "Delegate executed to capture verb command instance reference cannot be null.";
		public const string ArgumentNullException_ParserSettingsDelegateCannotBeNull = "The command line parser settings delegate cannot be null.";
		public const string InvalidOperationException_ParserSettingsInstanceCanBeUsedOnce = "The command line parserSettings instance cannnot be used more than once.";
		public const string InvalidOperationException_ParserStateInstanceCannotBeNotNull = "ParserState instance cannot be supplied.";
		public const string InvalidOperationException_ParserStateInstanceBadApplied = "Cannot apply ParserStateAttribute to a property that does not implement IParserState or is not accessible.";
	}
}
