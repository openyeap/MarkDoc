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
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class Page
    {
        static readonly Regex regex = new Regex("<!---(?<input>[\\s\\S]*?)--->", RegexOptions.Multiline);
        public string Source { get; set; }
        readonly Theme theme;
        readonly Hash pageData;
        readonly string pagePath;
        public Page(string root, string filePath, Hash siteData, Theme theme)
        {
            this.pageData = new Hash();
            this.theme = theme;
            this.pagePath = filePath.Substring(root.Length + 1, filePath.Length - filePath.LastIndexOf('.')) + ".html";
            var input = File.ReadAllText(filePath);
            StringBuilder setting = new StringBuilder();
            foreach (Match match in regex.Matches(input))
            {
                setting.AppendLine(match.Groups["input"].Value);
            }
            if (filePath.EndsWith(".md") || filePath.EndsWith(".markdown"))
            {
                Markdown md = new Markdown();
                this.Source = md.Transform(regex.Replace(input, ""));
            }
            else
            {
                this.Source = regex.Replace(input, "");
            }

            var dict = ConfigFileHelp.Default.ParseYaml(setting.ToString());
            this.pageData = Hash.FromDictionary(dict);

            string layout = pageData.Get<string>("layout", pageData.Get<string>("default_layout", "Index"));

            if (this.theme.Layout.ContainsKey(layout))
            {
                this.Source = this.theme.Layout[layout].Source.Replace("{{ body }}", this.Source);
            }
            var template = Template.Parse(this.Source);         
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