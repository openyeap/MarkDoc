using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class SetCommand : ICommand
    {
        public string Name => "set";
        public string Usage => @"
Command: set comments [--version]
   -c, --comments                The comments of the setting
   -v, --version                 The version of source to set to, latest version is default
";
        public void Execute(IEnumerable<string> args)
        {
            var comments = args.FirstOrDefault();
            var git = new Git();
            git.Set( comments, args.Skip(1).LastOrDefault());
        }
    }
}