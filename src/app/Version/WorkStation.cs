using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace Bzway.Writer.App
{
    public class WorkStation
    {
        public static object Registry { get; private set; }

        public static void Register()
        {
            try
            {
                var path = Environment.GetEnvironmentVariable("Path");
                var file = Assembly.GetEntryAssembly().Location;
                if (!path.Contains(file))
                {
                    Environment.SetEnvironmentVariable("Path", path + ";" + file);
                }
                  path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "install.reg");
                Process.Start(path);

                //var path = "Folder\\shell\\NetDisk|directory\\Background\\shell\\NetDisk".Split('|');

                //foreach (var item in path)
                //{
                //    using (RegistryKey mainMenu = Registry.ClassesRoot.CreateSubKey(item))
                //    {
                //        mainMenu.SetValue("Icon", "shell32.dll,5");
                //        mainMenu.SetValue("MUIVerb", "NetDisk");
                //        mainMenu.SetValue("SubCommands", "");
                //        var menus = "get|set|diff|push|pull".Split('|');

                //        for (int i = 0; i < menus.Length; i++)
                //        {
                //            var m = menus[i];
                //            using (var subMenu = mainMenu.CreateSubKey(string.Format("shell\\menu{0}", i)))
                //            {
                //                subMenu.SetValue("Icon", "shell32.dll,6");
                //                subMenu.SetValue("", m);
                //            }
                //            using (var subMenu = mainMenu.CreateSubKey(string.Format("shell\\menu{0}\\command", i)))
                //            {
                //                subMenu.SetValue("", string.Format("{0} {1}", Application.ExecutablePath, m));
                //            }
                //        }
                //    }
                //}
            }
            catch { }

        }
        public static void Unregister(string fileType, string shellKeyName)
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uninstall.reg");
                Process.Start(path);
                //var path = "Folder\\shell\\NetDisk|directory\\Background\\shell\\NetDisk".Split('|');
                //foreach (var item in path)
                //{
                //    Registry.ClassesRoot.DeleteSubKeyTree(item);
                //}

            }
            catch { }

        }
    }
}