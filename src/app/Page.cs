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

    public class Page : ILiquidizable
    {
        static readonly Regex regex = new Regex("<!---(?<input>[\\s\\S]*?)--->", RegexOptions.Multiline);
        public string Source { get; private set; }

        public string url
        {
            get
            {
                return this.pageData.Get<string>("url");
            }
        }
        public string title
        {
            get
            {
                return this.pageData.Get<string>("title", "标题");
            }
        }
        readonly Hash pageData;
        public Page(string root, string filePath)
        {
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
            if (!this.pageData.ContainsKey("url"))
            {
                var url = filePath.Substring(root.Length + 1, filePath.LastIndexOf('.') - root.Length - 1) + ".html";
                this.pageData.Add("url", url);
            }
            FileInfo fileInfo = new FileInfo(filePath);
            if (!this.pageData.ContainsKey("CreationTime"))
            {
                this.pageData.Add("CreationTime", fileInfo.CreationTimeUtc);
            }
            if (!this.pageData.ContainsKey("DateTime"))
            {
                this.pageData.Add("DateTime", fileInfo.LastWriteTimeUtc);
            }
            if (!this.pageData.ContainsKey("Title"))
            {
                this.pageData.Add("Title", Path.GetFileNameWithoutExtension(filePath));
            }
        }
        public void Save(string publicRoot, Hash siteData, Theme theme)
        {
            string layout = pageData.Get<string>("layout", pageData.Get<string>("default_layout", "Index"));
            if (theme.Layout.ContainsKey(layout))
            {
                this.Source = theme.Layout[layout].Source.Replace("{{ body }}", this.Source);
            }
            var template = Template.Parse(this.Source);
            Hash hash = new Hash();
            hash.Add("site", siteData);
            hash.Add("page", pageData);
            this.Source = template.Render(hash);
            string path = Path.Combine(publicRoot, this.url);
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
        public object ToLiquid()
        {
            return Hash.FromAnonymousObject(this);
        }
    }
}