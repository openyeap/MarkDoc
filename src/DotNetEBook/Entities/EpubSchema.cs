using Bzway.EBook.Reader.Epub.Schema.Navigation;
using Bzway.EBook.Reader.Epub.Schema.Opf;

namespace Bzway.EBook.Reader.Epub.Entities
{
    public class EpubSchema
    {
        public EpubPackage Package { get; set; }
        public EpubNavigation Navigation { get; set; }
        public string ContentDirectoryPath { get; set; }
    }
}
