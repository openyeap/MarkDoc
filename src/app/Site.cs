using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DotLiquid;
using System.Diagnostics;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.FileProviders;

namespace Bzway.Writer.App
{
    public partial class Site
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
                return this.configuration.GetSection("tools").GetValue<string>("browser", "iexplore.exe");
            }
        }
        public string Editor
        {
            get
            {
                return this.configuration.GetSection("tools").GetValue<string>("editor", "notepad.exe");
            }
        }
        public string ProcessFile
        {
            get
            {
                return Path.Combine(this.AppDirectory, "writer.pid");
            }
        }

        public readonly IConfigurationRoot configuration;

        private readonly string DocDirectory;

        private readonly string AppDirectory;

        private readonly Hash globalHash;
        private string doc_dir
        {
            get
            {
                return Path.Combine(this.DocDirectory, globalHash.Get<string>("doc_dir", "doc"));
            }
        }
        private string public_dir
        {
            get
            {
                return Path.Combine(this.DocDirectory, globalHash.Get<string>("public_dir", "public"));
            }
        }
        private string themes_dir
        {
            get
            {
                return Path.Combine(this.DocDirectory, globalHash.Get<string>("themes_dir", "themes"));
            }
        }
        private string default_themes
        {
            get
            {
                return this.globalHash.Get<string>("default_themes", "default");
            }
        }

        public Site()
        {
            this.AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(this.AppDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
            this.configuration = builder.Build();

            this.DocDirectory = Directory.GetCurrentDirectory();

            var configFile = Path.Combine(this.DocDirectory, "config.yml");
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (File.Exists(configFile))
            {
                foreach (var item in ConfigFileHelp.Default.ParseYaml(File.ReadAllText(configFile)))
                {
                    if (dict.ContainsKey(item.Key))
                    {
                        dict[item.Key] = item.Value;
                    }
                    else
                    {
                        dict.Add(item.Key, item.Value);
                    }
                }
            }
            configFile = Path.Combine(this.DocDirectory, "config.json");
            if (File.Exists(configFile))
            {
                foreach (var item in ConfigFileHelp.Default.ParseJson(File.ReadAllText(configFile)))
                {
                    if (dict.ContainsKey(item.Key))
                    {
                        dict[item.Key] = item.Value;
                    }
                    else
                    {
                        dict.Add(item.Key, item.Value);
                    }
                }
            }
            this.globalHash = Hash.FromDictionary(dict);
        }
        List<Page> LoadPages(Theme theme)
        {
            List<Page> list = new List<Page>();
            try
            {
                foreach (var path in Directory.GetFiles(this.doc_dir, "*.*", SearchOption.AllDirectories))
                {
                    list.Add(new Page(this.doc_dir, path, this.globalHash, theme));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }
        public string Create(string name)
        {
            var path = Path.Combine(this.doc_dir, name + ".md");
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            if (!fi.Exists)
            {
                fi.Create();
            }
            Console.WriteLine("created new page: " + fi.FullName);
            return fi.FullName;
        }
        public void Clean()
        {
            Directory.Delete(this.public_dir, true);
        }
        public void Generate()
        {
            var themePath = Path.Combine(this.themes_dir, this.default_themes);
            Template.FileSystem = new LayoutFileSystem(new PhysicalFileProvider(this.doc_dir), new PhysicalFileProvider(themePath));
            var theme = new Theme(themePath);
            foreach (var item in this.LoadPages(theme))
            {
                item.Save(this.public_dir);
            }
            foreach (var path in theme.Assets)
            {
                var destFileName = Path.Combine(this.public_dir, path.Remove(0, theme.Root.Length + 1));
                var fi = new FileInfo(destFileName);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                File.Copy(path, destFileName, true);
            }
        }
    }
}