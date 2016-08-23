namespace Bzway.DotNetBook.ePub.Entities
{
    public abstract class EpubContentFile
    {
        public string FileName { get; set; }
        public EpubContentType ContentType { get; set; }
        public string ContentMimeType { get; set; }
        public byte[] Content { get; set; }
    }
}
