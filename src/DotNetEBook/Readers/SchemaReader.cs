using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bzway.DotNetBook.ePub.Entities;
using Bzway.DotNetBook.ePub.Schema.Navigation;
using Bzway.DotNetBook.ePub.Schema.Opf;
using Bzway.DotNetBook.ePub.Utils;

namespace Bzway.DotNetBook.ePub.Readers
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
