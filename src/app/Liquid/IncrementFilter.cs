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
    public static class IncrementFilter
    {
        public static string increment(string input)
        {
            var i = int.Parse(input);
            i++;
            input = i.ToString();
            return input;

        }
    }
}