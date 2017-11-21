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
        static WorkStation _me;
        private WorkStation()
        {
        }
        public static WorkStation Instance
        {
            get
            {
                if (_me == null)
                {
                    _me = new WorkStation();
                }
                return _me;
            }
        }

        public void LockWorkStation(bool Block)
        {
            try
            {
                BlockInput(Block);
                LockCtrlAltDelete(Block);
            }
            catch (Exception ex)
            {
                var msss = ex.Message;

            }
        }

        [DllImport("user32.dll")]
        static extern void BlockInput(bool Block);

        void LockCtrlAltDelete(bool Block)
        {

        }

    }
}