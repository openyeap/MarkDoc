using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DotLiquid;
using HeyRed.MarkdownSharp;
using Microsoft.Extensions.FileProviders;
using YamlDotNet.Serialization;

namespace Bzway.Writer.App
{
    public class Page
    {
        public string Source { get; set; }
        readonly Theme theme;
        readonly Hash pageData;
        readonly string pagePath;
        readonly FileInfo fileInfo;
        public Page(string root, string filePath, Hash siteData, Theme theme)
        {
            this.pageData = new Hash();
            this.theme = theme;
            this.fileInfo = new FileInfo(filePath);
            this.pagePath = fileInfo.FullName.Remove(0, root.Length + 1).Replace(fileInfo.Extension, ".html");

            StringBuilder source = new StringBuilder();
            StringBuilder setting = new StringBuilder();
            bool isSetting = false;
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.Equals("---"))
                {
                    isSetting = !isSetting;
                    continue;
                }
                if (isSetting)
                {
                    setting.AppendLine(line);
                }
                else
                {
                    source.AppendLine(line);
                }
            }

            if (this.fileInfo.Extension.Equals(".md") || this.fileInfo.Extension.Equals(".markdown"))
            {
                Markdown md = new Markdown();
                this.Source = md.Transform(source.ToString());
            }
            else
            {
                this.Source = source.ToString();
            }

            var deserializer = new Deserializer();
            var yamlObject = (Dictionary<object, object>)deserializer.Deserialize(new StringReader(setting.ToString()));
            var dict = new Dictionary<string, object>();
            foreach (var item in yamlObject)
            {
                dict.Add(item.Key.ToString(), item.Value);
            }
            this.pageData = Hash.FromDictionary(dict);

            string layout = pageData.Get<string>("layout", pageData.Get<string>("default_layout", "Index"));

            if (this.theme.Layout.ContainsKey(layout))
            {
                this.Source = this.theme.Layout[layout].Source.Replace("{{ body }}", this.Source);
            }
            var template = Template.Parse(this.Source);
            template.Registers.Add("layout_dir", new PhysicalFileProvider(this.theme.Root));
            template.Registers.Add("page_dir", new PhysicalFileProvider(fileInfo.DirectoryName));
            Hash hash = new Hash();
            hash.Add("site", siteData);
            hash.Add("page", pageData);
            this.Source = template.Render(hash);
        }
        public void Save(string publicRoot)
        {
            string path = Path.Combine(publicRoot, pagePath);
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
            using (var steam = fileInfo.Create())
            {
                var buffer = Encoding.UTF8.GetBytes(this.Source);
                steam.Write(buffer, 0, buffer.Length);
            }
        }
    }
}