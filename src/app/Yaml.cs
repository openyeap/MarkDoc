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
using YamlDotNet.RepresentationModel;

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
        public Dictionary<string, object> ParseYaml(string text)
        {
            var results = new Dictionary<string, object>();

            var input = new StringReader(text);

            var yaml = new YamlStream();
            yaml.Load(input);

            if (yaml.Documents.Count == 0)
            {
                return results;
            }

            var root = yaml.Documents[0].RootNode;

            var collection = root as YamlMappingNode;
            if (collection != null)
            {
                foreach (var entry in collection.Children)
                {
                    var node = entry.Key as YamlScalarNode;
                    if (node != null)
                    {
                        results.Add(node.Value, GetValue(entry.Value));
                    }
                }
            }
            return results;
        }

        private static object GetValue(YamlNode value)
        {
            var collection = value as YamlMappingNode;
            if (collection != null)
            {
                var results = new Dictionary<string, object>();
                foreach (var entry in collection.Children)
                {
                    var node = entry.Key as YamlScalarNode;
                    if (node != null)
                    {
                        results.Add(node.Value, GetValue(entry.Value));
                    }
                }

                return results;
            }

            var list = value as YamlSequenceNode;
            if (list != null)
            {
                if (list.Children.All(_ => _ is YamlScalarNode))
                {
                    var listString = new List<string>();
                    foreach (var entry in list.Children)
                    {
                        var node = entry as YamlScalarNode;
                        if (node != null)
                        {
                            listString.Add(node.Value);
                        }
                    }
                    return listString;
                }
                else
                {
                    var listResults = new List<object>();
                    foreach (var entry in list.Children)
                    {
                        listResults.Add(GetValue(entry));
                    }
                    return listResults;
                }
            }

            bool valueBool;
            if (bool.TryParse(value.ToString(), out valueBool))
            {
                return valueBool;
            }

            return value.ToString();
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