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
        public string Name => "help";
        public void Execute(IEnumerable<string> args)
        {
           var helpText = @"
Usage:
mdoc command [options]

      --help                 Display help mode
      --debug                Enable debugging
      --safe                 Disable custom plugins

Command: Edit
  -p, --path VALUE           The path of file to create or modify

Command: Setting
  -t, --theme VALUE         The theme which to be used on the site

Command: create
  -t, --template = VALUE       The templating engine to use
      --destination = VALUE    The path to the destination site(default _site)
      --author = VALUE

Command: version
  Display current version.

Command: View
  - p, --path = VALUE           The related path to view
                               virtual directory
Command: Run
  - t, --template = VALUE       The templating engine to use
  - p, --port = VALUE           The port to test the site locally
       --cleantarget          Delete the target directory(_site by default)
       --vDir = VALUE           Rewrite url's to work inside the specified virtual directory
";
            Console.Write(helpText);
        }
    }
}
