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

namespace Bzway.Writer.App
{
    public class Site
    {
        private readonly string root;
        private readonly Hash globalHash;
        private readonly string doc_dir;
        private readonly string public_dir;
        private readonly string themes_dir;
        private readonly string default_themes;
        private readonly string default_category;
        private readonly string date_format;
        private readonly string time_format;
        private readonly Theme theme;
        private readonly List<Page> pages;
        public Site(string root)
        {
            this.root = root;
            this.globalHash = LoadGlobalSettings();
            this.doc_dir = globalHash.Get<string>("doc_dir", "doc");
            this.public_dir = globalHash.Get<string>("public_dir", "public");
            this.themes_dir = globalHash.Get<string>("themes_dir", "themes");
            this.default_themes = globalHash.Get<string>("default_themes", "default");
            this.default_category = globalHash.Get<string>("default_category", "home");
            this.date_format = globalHash.Get<string>("date_format", "yyyy-MM-dd");
            this.time_format = globalHash.Get<string>("time_format", "HH:mm:ss");

            var path = Path.Combine(this.root, this.themes_dir, this.default_themes);
            this.theme = new Theme(path);
            this.pages = this.LoadPages();
            Template.FileSystem = new LayoutFileSystem();
        }
        List<Page> LoadPages()
        {
            List<Page> list = new List<Page>();
            try
            {
                foreach (var path in Directory.GetFiles(this.doc_dir, "*.*", SearchOption.AllDirectories))
                {
                    list.Add(new Page(Path.Combine(this.root, this.doc_dir), path, this.globalHash, this.theme));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }

        Hash LoadGlobalSettings()
        {
            var configFile = Path.Combine(root, "config.yml");
            if (!File.Exists(configFile))
            {
                return new Hash();
            }
            var deserializer = new Deserializer();
            var yamlObject = (Dictionary<object, object>)deserializer.Deserialize(new StreamReader(configFile));
            var dict = new Dictionary<string, object>();
            foreach (var item in yamlObject)
            {
                dict.Add(item.Key.ToString(), item.Value);
            }
            return Hash.FromDictionary(dict);
        }
        public void Create(string name)
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
        }
        public void Clean()
        {
            Directory.Delete(this.public_dir, true);
        }
        public void Generate()
        {
            foreach (var item in this.pages)
            {
                item.Save(this.public_dir);
            }
            foreach (var path in this.theme.Assets)
            {

                var destFileName = Path.Combine(this.public_dir, path.Remove(0, this.theme.Root.Length + 1));
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