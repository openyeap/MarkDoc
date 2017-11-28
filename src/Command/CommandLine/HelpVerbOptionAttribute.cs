using CommandLine.Extensions;
using CommandLine.Infrastructure;
using System;
using System.Reflection;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class HelpVerbOptionAttribute : BaseOptionAttribute
	{
		private const string DefaultHelpText = "Display more information on a specific command.";
		public override char? ShortName
		{
			get
			{
				return null;
			}
			internal set
			{
				throw new InvalidOperationException("Verb commands do not support short name by design.");
			}
		}
		public override bool Required
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException("Verb commands cannot be mandatory since are mutually exclusive by design.");
			}
		}
		public HelpVerbOptionAttribute() : this("help")
		{
			base.HelpText = "Display more information on a specific command.";
		}
		public HelpVerbOptionAttribute(string longName) : base(null, longName)
		{
			base.HelpText = "Display more information on a specific command.";
		}
		internal static void InvokeMethod(object target, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, string verb, out string text)
		{
			text = null;
			MethodInfo left = helpInfo.Left;
			if (!HelpVerbOptionAttribute.CheckMethodSignature(left))
			{
				throw new MemberAccessException("{0} has an incorrect signature. Help verb command requires a method that accepts and returns a string.".FormatInvariant(new object[]
				{
					left.Name
				}));
			}
			text = (string)left.Invoke(target, new object[]
			{
				verb
			});
		}
		private static bool CheckMethodSignature(MethodInfo value)
		{
			return value.ReturnType == typeof(string) && value.GetParameters().Length == 1 && value.GetParameters()[0].ParameterType == typeof(string);
		}
	}
}
