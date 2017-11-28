using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    internal class CommandOptions
    {
        Regex regex = new Regex(@"(^-p|-path) (?<path>[\s\S]*$?)", RegexOptions.Singleline);
        string args;

        public CommandOptions(string args)
        {
            this.args = args;
        }

        public string Path
        {
            get
            {
                foreach (Match match in regex.Matches(args))
                {
                    return (match.Groups["path"].Value);
                }
                return string.Empty;
            }
        }
        public string GetUsage()
        {
            return ("mdoc edit -p|-path").ToString();
        }
    }
}