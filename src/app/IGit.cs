namespace Bzway
{
    public interface IGit
    {
        void Diff(string version1, string version2);
        /// <summary>
        /// 得到指定版本
        /// </summary>
        /// <param name="version"></param>
        void Get(string version);
        /// <summary>
        /// 在指定版本上设计新版本
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="version"></param>
        void Set(string comments, string version);

        void Pull(string root, string url);
        void Push(string root, string url);

    }
}