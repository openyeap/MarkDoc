using Bzway.DotNetBook.ePub.Schema.Navigation;
using Bzway.DotNetBook.ePub.Schema.Opf;

namespace Bzway.DotNetBook.ePub.Entities
{
    public class EpubSchema
    {
        public EpubPackage Package { get; set; }
        public EpubNavigation Navigation { get; set; }
        public string ContentDirectoryPath { get; set; }
    }
}
