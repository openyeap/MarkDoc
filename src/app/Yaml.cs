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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bzway.Writer.App
{
    public class ConfigFileHelp
    {
        private ConfigFileHelp()
        {
        }
        static ConfigFileHelp yaml = new ConfigFileHelp();
        public static ConfigFileHelp Default { get { return yaml; } }
        private Deserializer deserializer = new Deserializer();
        public Dictionary<string, object> ParseYaml(string input)
        {
            try
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
            catch
            {
                return new Dictionary<string, object>();
            }
        }


        public Dictionary<string, object> ParseJson(string input)
        {
            var values2 = new Dictionary<string, object>();
            try
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);
                foreach (KeyValuePair<string, object> d in values)
                {
                    if (d.Value is JObject)
                    {
                        values2.Add(d.Key, ParseJson(d.Value.ToString()));
                    }
                    else
                    {
                        values2.Add(d.Key, d.Value);
                    }
                }

            }
            catch { }
            return values2;
        }
    }
}