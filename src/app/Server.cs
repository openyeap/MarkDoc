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

        public string HostUrl
        {
            get
            {

                return this.configuration.GetValue<string>("applicationUrl", "http://localhost:9999");
            }
        }
        public string Broswer
        {
            get
            {
                return this.configuration.GetSection("Tools").GetValue<string>("Broswer", "iexplore.exe");
            }
        }
        public string Editor
        {
            get
            {
                return this.configuration.GetSection("Tools").GetValue<string>("Editor", "notepad.exe");
            }
        }
        public string ProcessFile
        {
            get
            {
                return Path.Combine(this.root, "writer.pid");
            }
        }

        private readonly string root;
        private readonly IConfigurationRoot configuration;
        public Server()
        {
            this.root = AppDomain.CurrentDomain.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(root)
                .AddJsonFile("config.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
            this.configuration = builder.Build();

        }
    }
}