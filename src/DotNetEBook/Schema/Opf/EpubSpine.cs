using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bzway.DotNetBook.ePub.Schema.Opf
{
    public class EpubSpine : List<EpubSpineItemRef>
    {
        public string Toc { get; set; }
    }
}
