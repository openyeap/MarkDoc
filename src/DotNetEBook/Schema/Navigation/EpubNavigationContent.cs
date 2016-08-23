using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bzway.EPubBook.Reader.Schema.Navigation
{
    public class EpubNavigationContent
    {
        public string Id { get; set; }
        public string Source { get; set; }

        public override string ToString()
        {
            return String.Concat("Source: " + Source);
        }
    }
}
