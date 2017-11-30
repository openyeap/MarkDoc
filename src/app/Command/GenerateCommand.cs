using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class GenerateCommand : ICommand
    {
        public string Name => "g|Generate";

        public string Usage => string.Empty;

        public void Execute(IEnumerable<string> args)
        {
            Site site = new Site();
            site.Generate();
            Console.WriteLine("generate site OK");
        }
    }
}