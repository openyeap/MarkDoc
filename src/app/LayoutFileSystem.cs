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
        public IList<IFileProvider> fileProviders = new List<IFileProvider>();
        public LayoutFileSystem(params IFileProvider[] fileProviders)
        {
            foreach (var item in fileProviders)
            {
                this.fileProviders.Add(item);
            }
        }
        public string ReadTemplateFile(Context context, string templateName)
        {
            try
            {
                var path = templateName;
                if (context[templateName] != null)
                {
                    path = context[templateName].ToString();
                }

                foreach (var fileProvider in this.fileProviders)
                {
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
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
    }
}