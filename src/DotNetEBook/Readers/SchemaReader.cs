using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bzway.EBook.Reader.Epub.Entities;
using Bzway.EBook.Reader.Epub.Schema.Navigation;
using Bzway.EBook.Reader.Epub.Schema.Opf;
using Bzway.EBook.Reader.Epub.Utils;

namespace Bzway.EBook.Reader.Epub.Readers
{
    internal static class SchemaReader
    {
        public static EpubSchema ReadSchema(ZipArchive epubArchive)
        {
            EpubSchema result = new EpubSchema();
            string rootFilePath = RootFilePathReader.GetRootFilePath(epubArchive);
            string contentDirectoryPath = ZipPathUtils.GetDirectoryPath(rootFilePath);
            result.ContentDirectoryPath = contentDirectoryPath;
            EpubPackage package = PackageReader.ReadPackage(epubArchive, rootFilePath);
            result.Package = package;
            EpubNavigation navigation = NavigationReader.ReadNavigation(epubArchive, contentDirectoryPath, package);
            result.Navigation = navigation;
            return result;
        }
    }
}
