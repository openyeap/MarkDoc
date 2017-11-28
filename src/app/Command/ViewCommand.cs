using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class ViewCommand : ICommand
    {
        public string Name => "View";
        public void Execute(IEnumerable<string> args)
        {
            var server = new Site();
            var filePath = args.FirstOrDefault();
            if (string.IsNullOrEmpty(filePath))
            {
                Process.Start(server.Broswer, server.HostUrl);
                return;
            }
            Process.Start(server.Broswer, server.HostUrl + filePath);
        }
    }
}