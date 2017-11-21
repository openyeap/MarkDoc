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
    [Serializable]
    public class Version
    {
        public string Id { get; set; }
        public string PId { get; set; }
        public string ChildId { get; set; }
        public string Auth { get; set; }
        public DateTime Time { get; set; }
        public string Comments { get; set; }
    }

    public class VersionHistory
    {

    }
}