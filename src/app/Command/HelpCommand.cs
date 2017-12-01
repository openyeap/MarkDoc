using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class HelpCommand : ICommand
    {
        public string Name => "?|--help|help";

        public string Usage => @"
Usage:
mdoc command [options]
        --help                   Display help mode
        --debug                  Enable debugging
        --safe                   Disable custom plugins

Command: create
    -t, --template = VALUE       The templating engine to use
        --destination = VALUE    The path to the destination site(default _site)
        --author = VALUE

Command: Setting
    -t, --theme VALUE            The theme which to be used on the site

Command: version
    Display current version.
";

        public void Execute(IEnumerable<string> args)
        {
            foreach (var item in CommandHelper.Default)
            {
                Console.WriteLine(item.Usage);
            }
        }
    }
}
