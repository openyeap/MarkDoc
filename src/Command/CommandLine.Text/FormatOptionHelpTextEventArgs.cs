using System;
namespace CommandLine.Text
{
	public class FormatOptionHelpTextEventArgs : EventArgs
	{
		private readonly BaseOptionAttribute _option;
		public BaseOptionAttribute Option
		{
			get
			{
				return this._option;
			}
		}
		public FormatOptionHelpTextEventArgs(BaseOptionAttribute option)
		{
			this._option = option;
		}
	}
}
