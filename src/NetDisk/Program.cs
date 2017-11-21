using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Bzway
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {
            Git aaa = new Git();
            while (true)
            {
                var input = File.ReadAllText(@"D:\lua\foo3.txt", Encoding.UTF8);

                var aaaaaa = aaa.sha1(input);

            }

            if (arg.Length == 0)
            {
                try
                {
                    Register();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            try
            {
                var root = Directory.GetCurrentDirectory();
                VirtualDrive.Create('N', root);
                var git = new Git();
                git.Set(root, "", "");
                var version = Console.ReadLine();
                while (!string.IsNullOrEmpty(version))
                {
                    git.Get(root, version);
                    version = Console.ReadLine();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            return;
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //var form = new MainForm();

            //if (arg.Length == 1)
            //{
            //    double minutes;
            //    if (double.TryParse(arg[0], out minutes))
            //    {
            //        form.workMinutes = minutes;
            //    }
            //}
            //Application.Run(form);
        }

}