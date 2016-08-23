using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bzway.EBook.Reader.Epub.Schema.Navigation
{
    public class EpubNavigation
    {
        public EpubNavigationHead Head { get; set; }
        public EpubNavigationDocTitle DocTitle { get; set; }
        public List<EpubNavigationDocAuthor> DocAuthors { get; set; }
        public EpubNavigationMap NavMap { get; set; }
        public EpubNavigationPageList PageList { get; set; }
        public List<EpubNavigationList> NavLists { get; set; }
    }
}
