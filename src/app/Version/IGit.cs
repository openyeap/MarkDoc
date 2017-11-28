namespace Bzway.Writer.App
{
    public interface IGit
    {
        /// <summary>
        /// 本地两个版本的比较
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2">被比较的版本，默认本地最新</param>
        void Diff(string version1, string version2 = "");
        /// <summary>
        /// 本地得到指定版本
        /// </summary>
        /// <param name="version">默认本地最新</param>
        void Get(string version = "");
        /// <summary>
        /// 本地工作目录中的变化提交到指定版本上
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="version">默认本地最新</param>
        void Set(string comments, string version = "");
        /// <summary>
        /// 从远程目录更新到本地
        /// </summary>
        /// <param name="url"></param>
        void Pull(string url);
        /// <summary>
        /// 将本地版本推送到远程
        /// </summary>
        /// <param name="url"></param>
        /// <param name="version">默认本地最新</param>
        void Push(string url, string version = "");

    }
    public interface ISite
    {
        string Upsert(string name);
        void Generate();
    }
}