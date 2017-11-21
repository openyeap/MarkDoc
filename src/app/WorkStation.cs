using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Linq;
namespace Bzway
{
    public class WorkStation
    {
        public static void Register()
        {
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
        public static void Unregister(string fileType, string shellKeyName)
        {
            //var path = "Folder\\shell\\NetDisk|directory\\Background\\shell\\NetDisk".Split('|');
            //foreach (var item in path)
            //{
            //    Registry.ClassesRoot.DeleteSubKeyTree(item);
            //}

        }
    }
}