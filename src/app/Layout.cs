using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DotLiquid;
using System.Text.RegularExpressions;

namespace Bzway.Writer.App
{
    public class Theme
    {
        public string Root { get; private set; }
        public List<string> Assets { get; set; }
        public Dictionary<string, Layout> Layout { get; set; }
        public Theme(string root)
        {
            this.Root = root;
            this.Layout = new Dictionary<string, Layout>(StringComparer.OrdinalIgnoreCase);
            this.Assets = new List<string>();

            if (!Directory.Exists(this.Root))
            {
                Directory.CreateDirectory(this.Root);
                return;
            }
            foreach (var item in Directory.GetFiles(this.Root, "*.html", SearchOption.TopDirectoryOnly))
            {
                FileInfo file = new FileInfo(item);

                this.Layout.Add(Path.GetFileNameWithoutExtension(file.FullName), new Layout() { Source = File.ReadAllText(item) });
            }
            List<string> regList = new List<string>();
            var ignoreFile = Path.Combine(root, "ignore.txt");
            if (File.Exists(ignoreFile))
            {
                var ignores = File.ReadLines(Path.Combine(root, "ignore.txt")).Where(m => !m.StartsWith("#") && !string.IsNullOrEmpty(m.Trim())).Select(m => m.Trim()).ToList();
                var rules = ignores.Where(m => m.StartsWith("*.")).Select(m => m.Remove(0, 2)).ToArray();
                if (rules.Length > 0)
                {
                    regList.Add(string.Format(@"^.+\.({0})$", string.Join("|", rules)));
                }
                rules = ignores.Where(m => m.StartsWith("/")).Select(m => m.Remove(0, 1)).ToArray();
                if (rules.Length > 0)
                {
                    regList.Add(string.Format(@"^\/({0})\/.+$", string.Join("|", rules)));
                }
                rules = ignores.Where(m => m.EndsWith("/")).Select(m => m.Remove(m.Length - 1)).ToArray();
                if (rules.Length > 0)
                {
                    regList.Add(string.Format(@"^.?\/({0})\/.+$", string.Join("|", rules)));
                }
                rules = ignores.Where(m => !m.StartsWith("*.") & !m.StartsWith("/") && !m.EndsWith("/")).ToArray();
                if (rules.Length > 0)
                {
                    regList.Add(string.Format(@"^.+({0}).+$", string.Join("|", rules)));
                }
            }

            var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories).Where(file =>
            {
                if (file.EndsWith("ignore.txt"))
                {
                    return false;
                }
                var name = file.Remove(0, root.Length).Replace("\\", "/");
                var result = true;
                foreach (var reg in regList)
                {
                    result = Regex.IsMatch(name, reg);
                    if (result)
                    {
                        return false;
                    }
                }
                return true;
            }).ToList();
            foreach (var item in files)
            {
                if (item.EndsWith(".html"))
                {
                    continue;
                }
                this.Assets.Add(item);
            }
        }
    }
    public class Layout
    {
        public string Source { get; set; }

    }
}