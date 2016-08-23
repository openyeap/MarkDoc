using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bzway.DotNetBook.ePub.Entities
{
    public class EpubByteContentFile : EpubContentFile
    {
        public byte[] Content { get; set; }
    }
}
