using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class EditCommand : ICommand
    {
        Regex regex = new Regex(@"(^-p|-path|) (?<path>[\s\S]*$?)", RegexOptions.Singleline);
        IEnumerable<string> args;
        string Path
        {
            get
            {
                var path = string.Join(" ", args.ToArray());
                foreach (Match match in regex.Matches(path))
                {
                    return (match.Groups["path"].Value);
                }

                return path.Trim();
            }
        }

        public string Name => "edit";
        public void Execute(IEnumerable<string> args)
        {
            this.args = args;
            if (string.IsNullOrEmpty(this.Path))
            {
                Console.WriteLine(this.Usage);
                return;
            }
            var root = Directory.GetCurrentDirectory();
            var docPath = this.Path.Trim('/', '\\');
            var site = new Site();
            var filePath = site.Upsert(docPath);
            Process.Start(site.Editor, filePath);
        }
        public string Usage { get { return "mdoc edit [-p|-path] value"; } }
    }
}