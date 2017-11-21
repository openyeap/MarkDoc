using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using DotLiquid;
using DotLiquid.FileSystems;
using Microsoft.Extensions.FileProviders;
using System.Text;
using HeyRed.MarkdownSharp; 

namespace Bzway.Writer.App
{ 
    public class LiquidViewResult
    {
        private static Dictionary<int, Template> cache = new Dictionary<int, Template>();
        private readonly string[] exetensions = new string[] { "", ".html", ".htm", ".md", ".markdown" };
        private readonly string path;
        private readonly int hashCode;
        private readonly string root;
        public IDictionary<string, object> MapData { get; private set; }

        public LiquidViewResult(string root, string path)
        {
            this.root = root;
            this.path = path;
            this.hashCode = (this.root + this.path).GetHashCode();

        }


        public Task<string> Render()
        {
            return Task.Run<string>(() =>
            {
                if (!cache.ContainsKey(this.hashCode))
                {
                    IFileProvider fileProvider = new PhysicalFileProvider(this.root);

                    foreach (var item in exetensions)
                    {
                        var fileInfo = fileProvider.GetFileInfo(path + item);
                        if (fileInfo.Exists)
                        {
                            int count = (int)fileInfo.Length;
                            byte[] buffer = new byte[count];
                            using (var reader = fileInfo.CreateReadStream())
                            {
                                reader.Read(buffer, 0, count);
                            }
                            var source = Encoding.UTF8.GetString(buffer);
                            Markdown markdown = new Markdown();
                            source = markdown.Transform(source);
                            var template = Template.Parse(source);
                            template.Registers.Add("file_system", new TemplateFileSystem(fileProvider));
                            cache.Add(this.hashCode, template);
                            break;
                        }
                    }
                    if (!cache.ContainsKey(this.hashCode))
                    {
                        cache.Add(this.hashCode, Template.Parse("NO Found"));
                    }
                }
                return cache[this.hashCode].Render();
            });
        }
    }
    public class TemplateFileSystem : IFileSystem
    {
        readonly IFileProvider fileProvider;
        public TemplateFileSystem(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }
        public string ReadTemplateFile(Context context, string templateName)
        {
            var path = context[templateName].ToString();

            var fileInfo = fileProvider.GetFileInfo(path);

            if (fileInfo.Exists)
            {
                int count = (int)fileInfo.Length;
                byte[] buffer = new byte[count];
                using (var reader = fileInfo.CreateReadStream())
                {
                    reader.Read(buffer, 0, count);
                }
                var source = Encoding.UTF8.GetString(buffer);
                return source;
            }
            fileInfo = this.fileProvider.GetFileInfo(context["area"].ToString() + path);
            if (fileInfo.Exists)
            {
                int count = (int)fileInfo.Length;
                byte[] buffer = new byte[count];
                using (var reader = fileInfo.CreateReadStream())
                {
                    reader.Read(buffer, 0, count);
                }
                var source = Encoding.UTF8.GetString(buffer);
                return source;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}