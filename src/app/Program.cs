using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DotLiquid;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(string.Join(';', args));
            WorkStation.Register();
            var key = args.FirstOrDefault();
            if (string.IsNullOrEmpty(key))
            {
                key = "help";
            }
            var cmd = CommandHelper.Default[key];
            cmd.Execute(args.Skip(1));
        }
    }
}