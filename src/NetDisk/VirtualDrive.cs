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
    /// <summary>
    /// Creation and deletion of non-persistent virtual drives.
    /// The drive has to be restored after a reboot of the system.
    /// Does not appear in the registry key: HKLM\System\MountedDevices
    /// 
    /// VirtualDrive - © Konstantin Gross
    /// </summary>
    public class VirtualDrive
    {
        #region Win32
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool DefineDosDevice(int dwFlags, string lpDeviceName, string lpTargetPath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetDriveType(string lpRootPathName);

        private const int DDD_RAW_TARGET_PATH = 0x00000001;
        private const int DDD_REMOVE_DEFINITION = 0x00000002;
        private const int DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;

        private const int DRIVE_UNKNOWN = 0;
        private const int DRIVE_NO_ROOT_DIR = 1;
        private const int DRIVE_FIXED = 3;
        #endregion // Win32

        #region Public methods

        #region Create
        /// <summary>
        /// Creation of a non-persistent drive.
        /// </summary>
        /// <param name="driveChar">Drive letter</param>
        /// <param name="path">Path to the folder to be linked</param>
        /// <returns>True/False on attempt to create the drive</returns>
        public static bool Create(char driveChar, string path)
        {
            return DDDOperation(driveChar, path, true);
        }
        #endregion // Create

        #region Delete
        /// <summary>
        /// Deletion of a non-persistent drive.
        /// </summary>
        /// <param name="driveChar">Drive letter</param>
        /// <param name="path">Path to the folder to be linked</param>
        /// <returns>True/False on attempt to delete the drive</returns>
        public static bool Delete(char driveChar, string path)
        {
            return DDDOperation(driveChar, path, false);
        }
        #endregion // Delete

        #endregion // Public methods

        #region Private methods

        #region DDDOperations
        private static bool DDDOperation(char driveChar, string path, bool create)
        {
            //Valid directory?
            if (!Directory.Exists(path))
            {
                return false;
            }
            string drive = string.Format("{0}:", driveChar.ToString().ToUpper());

            //Does the volume exist?
            int type = GetDriveType(string.Format("{0}{1}", drive, Path.DirectorySeparatorChar));

            //Hint: Type of a created virtual drive is DRIVE_FIXED
            if ((create && type != DRIVE_UNKNOWN && type != DRIVE_NO_ROOT_DIR) || (!create && type != DRIVE_FIXED))
            {
                return false;
            }
            int flags = DDD_RAW_TARGET_PATH;

            if (!create)
            {
                flags |= (DDD_REMOVE_DEFINITION | DDD_EXACT_MATCH_ON_REMOVE);
            }
            return DefineDosDevice(flags, drive, string.Format("{0}??{0}{1}", Path.DirectorySeparatorChar, path));
        }
        #endregion // DDDOperations

        #endregion // Private methods
    }
}