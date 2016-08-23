using Bzway.EPubBook.Reader.Schema.Navigation;
using Bzway.EPubBook.Reader.Schema.Opf;

namespace Bzway.EPubBook.Reader.Entities
{
    public class EpubSchema
    {
        public EpubPackage Package { get; set; }
        public EpubNavigation Navigation { get; set; }
        public string ContentDirectoryPath { get; set; }
    }
}
