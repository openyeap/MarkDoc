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
        public string Name => "edit";
        public void Execute(IEnumerable<string> args)
        {
            CommandOptions editOptions = new CommandOptions(string.Join(" ", args.ToArray()));
            if (string.IsNullOrEmpty(editOptions.Path))
            {
                Console.WriteLine(editOptions.GetUsage());
                return;
            }
            var root = Directory.GetCurrentDirectory();
            var docPath = editOptions.Path.Trim('/', '\\');
            var site = new Site();
            var filePath = site.Upsert(docPath);
            Process.Start(site.Editor, filePath);
        }
    }
}
