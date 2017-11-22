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
            root += "\\doc";
            app.Run(async (context) =>
            {
                var path = context.Request.Path.Value;
                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(path.Split('/').LastOrDefault()))
                {
                    path += "index";
                }
                var view = new LiquidViewResult(root, path);
                var result = view.Render().Result;
                await context.Response.WriteAsync(result);
            });
        }
    }
}