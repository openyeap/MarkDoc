using CommandLine.Infrastructure;
using System;
using System.Reflection;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class HelpOptionAttribute : BaseOptionAttribute
	{
		private const string DefaultHelpText = "Display this help screen.";
		public override bool Required
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}
		public HelpOptionAttribute() : this("help")
		{
			base.HelpText = "Display this help screen.";
		}
		public HelpOptionAttribute(char shortName) : base(shortName, null)
		{
			base.HelpText = "Display this help screen.";
		}
		public HelpOptionAttribute(string longName) : base(null, longName)
		{
			base.HelpText = "Display this help screen.";
		}
		public HelpOptionAttribute(char shortName, string longName) : base(shortName, longName)
		{
			base.HelpText = "Display this help screen.";
		}
		internal static void InvokeMethod(object target, Pair<MethodInfo, HelpOptionAttribute> pair, out string text)
		{
			text = null;
			MethodInfo left = pair.Left;
			if (!HelpOptionAttribute.CheckMethodSignature(left))
			{
				throw new MemberAccessException();
			}
			text = (string)left.Invoke(target, null);
		}
		private static bool CheckMethodSignature(MethodInfo value)
		{
			return value.ReturnType == typeof(string) && value.GetParameters().Length == 0;
		}
	}
}
