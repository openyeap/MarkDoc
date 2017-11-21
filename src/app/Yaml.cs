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
using YamlDotNet.Serialization;

namespace Bzway.Writer.App
{
    public class Yaml
    {
        private Yaml()
        {
        }
        static Yaml yaml = new Yaml();
        public static Yaml Default { get { return yaml; } }
        private Deserializer deserializer = new Deserializer();
        public Dictionary<string, object> Parse(string input)
        {
            var yamlObject = (Dictionary<object, object>)deserializer.Deserialize(new StringReader(input));
            if (yamlObject == null)
            {
                return new Dictionary<string, object>();
            }
            var dict = new Dictionary<string, object>();
            foreach (var item in yamlObject)
            {
                dict.Add(item.Key.ToString(), item.Value);
            }
            return dict;
        }
    }
}