using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DotLiquid;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Bzway.Writer.App
{
    public class Server
    {
        const string processFile = "/writer.pid";
        public string HostUrl { get; private set; }
        public string BroswerPath { get; private set; }
        public string ProcessFile { get; private set; }
        public Server()
        {
            var applicationPath = Assembly.GetEntryAssembly().Location;
            var root = new FileInfo(applicationPath).DirectoryName;
            this.ProcessFile = root + processFile;

            var builder = new ConfigurationBuilder()
                 .SetBasePath(root)
                 .AddJsonFile("config.json", optional: false, reloadOnChange: false)
                 .AddEnvironmentVariables();
            var config = builder.Build();
            this.HostUrl = config.GetValue<string>("HostUrl", "http://localhost:9999");
            this.BroswerPath = config.GetValue<string>("BroswerPath", @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
        }
    }
}