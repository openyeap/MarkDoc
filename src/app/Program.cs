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
using CommandLine;
using CommandLine.Text;

namespace Bzway.Writer.App
{
    public class EditOptions
    {
        [Option('p', "path", MetaValue = "Path", Required = false, HelpText = "输入文件的相对路径")]
        public string Path { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, "mdoc edit").ToString();
        }

    }

    public class EditCommand : ICommand
    {
        public string Name => "edit";

        public object Options { get { return this.options; } }

        private EditOptions options = new EditOptions();

        public void Execute(IEnumerable<string> args)
        {
            var root = Directory.GetCurrentDirectory();
            var cmd = args.FirstOrDefault();
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            var docPath = cmd.Trim('/', '\\');
            var site = new Site();
            var filePath = site.Upsert(docPath);
            Site server = new Site();
            Process.Start(server.Editor, filePath);
        }
    }
    public class Program
    {
        static Dictionary<string, ICommand> dictionary;

        static IDictionary<string, ICommand> GetCommands
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(m => m.IsAssignableFrom(typeof(ICommand))))
                    {
                        var cmd = (ICommand)Activator.CreateInstance(item);
                        if (cmd != null)
                        {
                            dictionary.Add(cmd.Name, cmd);
                        }
                    }
                }
                return dictionary;
            }
        }

        public static void Main(string[] args)
        {
            var cmd = dictionary[args.FirstOrDefault()]; 

            if (Parser.Default.ParseArguments(args.Skip(1).ToArray(), cmd.Options))
            {
                cmd.Execute(args);
            }

        }
        public static void Generate(string[] args)
        {
            Site site = new Site();
            site.Generate();
            Console.WriteLine("generate site OK");
        }
        public static void View(string[] args)
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

        public static void Edit(string[] args)
        {
            var root = Directory.GetCurrentDirectory();
            var cmd = args.FirstOrDefault();
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            var docPath = cmd.Trim('/', '\\');
            var site = new Site();
            var filePath = site.Upsert(docPath);
            Site server = new Site();
            Process.Start(server.Editor, filePath);
        }
        public static void Run(string[] args)
        {
            var root = Directory.GetCurrentDirectory();
            var server = new Site();
            bool canRun = !File.Exists(server.ProcessFile);
            if (!canRun)
            {
                using (var stream = File.OpenText(server.ProcessFile))
                {
                    int pid = 0;
                    if (int.TryParse(stream.ReadLine(), out pid))
                    {
                        var process = Process.GetProcesses().FirstOrDefault(m => m.Id == pid);
                        canRun = (process == null);
                    }
                }
            }
            if (canRun)
            {
                using (var stream = File.CreateText(server.ProcessFile))
                {
                    stream.Write(Process.GetCurrentProcess().Id);
                };
                var site = new Site();
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls(server.HostUrl)
                    .UseContentRoot(root)
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();
                host.Run();
                File.Delete(server.ProcessFile);
            }
        }
    }
}