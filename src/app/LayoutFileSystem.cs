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
    public class LayoutFileSystem : IFileSystem
    {
        public LayoutFileSystem()
        {

        }
        public string ReadTemplateFile(Context context, string templateName)
        {
            var path = context[templateName].ToString();
            var fileProvider = context.Registers.Get<IFileProvider>("page_dir");
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
            fileProvider = context.Registers.Get<IFileProvider>("layout_dir");
            if (fileProvider == null)
            {
                return string.Empty;
            }
            fileInfo = fileProvider.GetFileInfo(path);
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
            return string.Empty;
        }
    }
}