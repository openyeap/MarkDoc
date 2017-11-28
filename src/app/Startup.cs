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
using Microsoft.Extensions.Configuration;

namespace Bzway.Writer.App
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var root = env.ContentRootPath;
            app.Run(async (context) =>
            {
                try
                {
                    var path = context.Request.Path.Value;
                    if (string.IsNullOrEmpty(path) || path.EndsWith('/'))
                    {
                        path += "index";
                    }
                    path = path.Trim('/') + ".html";
                    Site site = context.RequestServices.GetService<Site>();
                    var result = site.View(path);
                    if (!string.IsNullOrEmpty(result))
                    {
                        await context.Response.WriteAsync(result);
                    }
                    else
                    {
                        PhysicalFileProvider physicalFile = new PhysicalFileProvider(root);
                        var file = physicalFile.GetFileInfo(context.Request.Path.Value);
                        using (var stream = file.CreateReadStream())
                        {
                            byte[] buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            result = Encoding.UTF8.GetString(buffer);
                        }
                        await context.Response.WriteAsync(result);
                    }
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync(ex.Message);
                }
            });
        }
    }
}