using System.Collections.Generic;
using System.Drawing;
using Bzway.EBook.Reader.Epub.Entities;

namespace Bzway.EBook.Reader.Epub
{
    public partial class EPubBook
    {
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public List<string> AuthorList { get; set; }
        public EpubSchema Schema { get; set; }
        public EpubContent Content { get; set; }
        public Image CoverImage { get; set; }
        public List<EpubChapter> Chapters { get; set; }
    }
}
