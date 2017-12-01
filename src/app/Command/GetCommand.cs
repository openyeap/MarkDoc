using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class GetCommand : ICommand
    {
        public string Name => "get";
        public string Usage => @"
Command: get version
    -v, --version                The version of source to get from local, latest version is default
";
        public void Execute(IEnumerable<string> args)
        {
            var version = args.LastOrDefault();
            if (string.IsNullOrEmpty(version))
            {
                Console.WriteLine(this.Usage);
                return;
            }
            var git = new Git();
            git.Get(version);
        }
    }
}