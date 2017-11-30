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
    public partial class Site : ISite
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

        private readonly string workDirectory;

        private readonly string AppDirectory;

        private readonly Hash siteData;
        private string PostDirectory
        {
            get
            {
                return Path.Combine(this.workDirectory, siteData.Get<string>("post_dir", "post"));
            }
        }
        public string PublicDirectory
        {
            get
            {
                return Path.Combine(this.workDirectory, siteData.Get<string>("public_dir", "public"));
            }
        }
        private string ThemesDirectory
        {
            get
            {
                return Path.Combine(this.workDirectory, siteData.Get<string>("themes_dir", "themes"));
            }
        }
        private string DataDirectory
        {
            get
            {
                return Path.Combine(this.workDirectory, siteData.Get<string>("data_dir", "data"));
            }
        }
        private string DefaultTheme
        {
            get
            {
                return this.siteData.Get<string>("default_theme", "default");
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

            this.workDirectory = Directory.GetCurrentDirectory();

            var configFile = Path.Combine(this.workDirectory, "config.yml");
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
            configFile = Path.Combine(this.workDirectory, "config.json");
            if (File.Exists(configFile))
            {
                var jsonObject = ConfigFileHelp.Default.ParseJson(File.ReadAllText(configFile));
                if (jsonObject != null && jsonObject is Dictionary<string, object>)
                {
                    var list = (Dictionary<string, object>)jsonObject;
                    foreach (var item in list)
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
            }
            this.siteData = Hash.FromDictionary(dict);
        }
        List<Page> LoadPages()
        {
            List<Page> list = new List<Page>();
            try
            {
                foreach (var path in Directory.GetFiles(this.workDirectory, "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (path.EndsWith(".json") || path.EndsWith(".yaml"))
                    {
                        continue;
                    }
                    list.Add(new Page(this.workDirectory, path));
                }
                foreach (var path in Directory.GetFiles(this.PostDirectory, "*.*", SearchOption.AllDirectories))
                {
                    list.Add(new Page(this.PostDirectory, path));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }
        public string Upsert(string name)
        {
            var path = Path.Combine(this.PostDirectory, name + ".md");
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            if (!fi.Exists)
            {
                File.WriteAllText(fi.FullName, "Hello World", Encoding.UTF8);
            }
            Console.WriteLine("created new page: " + fi.FullName);
            return fi.FullName;
        }
        public void Clean()
        {
            Directory.Delete(this.PublicDirectory, true);
        }

        public string Generate(string url = "")
        {
            var returnValue = string.Empty;
            var themePath = Path.Combine(this.ThemesDirectory, this.DefaultTheme);

            Template.FileSystem = new LayoutFileSystem(new PhysicalFileProvider(this.PostDirectory), new PhysicalFileProvider(themePath));
            Template.RegisterFilter(typeof(IncrementFilter));
            var theme = new Theme(themePath);

            foreach (var path in theme.Assets)
            {
                var destFileName = Path.Combine(this.PublicDirectory, path.Remove(0, theme.Root.Length + 1));
                var fi = new FileInfo(destFileName);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                File.Copy(path, destFileName, true);
            }

            var pageList = this.LoadPages();
            if (this.siteData.ContainsKey("pages"))
            {
                this.siteData.Remove("pages");
            }
            this.siteData.Add("pages", pageList);

            Hash data = new Hash();
            Dictionary<string, object> dict;
            foreach (var path in Directory.GetFiles(this.DataDirectory, "*.*", SearchOption.TopDirectoryOnly))
            {
                FileInfo fileInfo = new FileInfo(path);
                var key = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
                switch (fileInfo.Extension)
                {
                    case ".json":
                        var jsonObject = ConfigFileHelp.Default.ParseJson(File.ReadAllText(path));
                        if (jsonObject == null)
                        {
                            continue;
                        }
                        if (jsonObject is List<Dictionary<string, object>>)
                        {
                            data.Add(key, jsonObject);
                        }
                        else
                        {
                            var list = (Dictionary<string, object>)jsonObject;
                            dict = new Dictionary<string, object>();
                            foreach (var item in list)
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
                            data.Add(key, Hash.FromDictionary(dict));
                        }
                        break;
                    case ".yaml":
                        dict = new Dictionary<string, object>();
                        foreach (var item in ConfigFileHelp.Default.ParseYaml(File.ReadAllText(path)))
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
                        data.Add(key, Hash.FromDictionary(dict));
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in pageList)
            {
                item.Save(this.PublicDirectory, this.siteData, data, theme);
                if (item.url == url)
                {
                    returnValue = item.Source;
                }
            }
            return returnValue;
        }
    }


    public class menu : DotLiquid.Drop
    {
        public string name { get; set; }

        public string url { get; set; }
        public string target { get; set; }
    }
}