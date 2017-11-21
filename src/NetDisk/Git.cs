using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Bzway
{

    public class Git : IGit
    {

        public void Set(string root, string auth, string comments)
        {
            #region 1. 遍历工作目录中所有文件
            var files = this.GetFiles(root);
            #endregion

            #region 2.生成新版本快照
            var stageVersionId = this.GetStageVersionId(files, root);
            #endregion

            #region 3.与最近版本比较差异
            var listOfVersion = GetVersions(root);
            var lastVersion = listOfVersion.LastOrDefault();
            if (lastVersion == null || lastVersion.Id != stageVersionId)
            {
                listOfVersion.Add(new Version()
                {
                    Id = stageVersionId,
                    PId = lastVersion == null ? null : lastVersion.Id,
                    Time = DateTime.UtcNow,
                    Auth = auth,
                    Comments = comments,
                });
                #region 4.如果有差异，则提交新版本号（版本树）到本地库
                SaveVersion(root, listOfVersion);
                #endregion
            }
            #endregion

        }
        public void Get(string root, string version)
        {
            var listOfVersion = GetVersions(root).Where(m => m.Id.Contains(version)).ToArray();
            if (listOfVersion.Length != 1)
            {
                foreach (var item in GetVersions(root))
                {
                    Console.WriteLine(item.Id);
                }
                return;
            }
            Dictionary<string, string> dictionary;
            var dataFilePath = Path.Combine(root, ".git", "data", listOfVersion[0].Id);

            using (Stream stream = new FileStream(dataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                dictionary = (Dictionary<string, string>)formatter.Deserialize(stream);
            }
            if (dictionary == null)
            {
                return;
            }
            foreach (var item in dictionary.Keys)
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(root, item));
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                if (!fileInfo.Exists)
                {
                    using (var inputStream = fileInfo.OpenWrite())
                    {
                        using (var outStream = File.OpenRead(Path.Combine(root, ".git", "data", dictionary[item])))
                        {
                            outStream.CopyTo(inputStream);
                        }
                    }
                }
            }
        }
        public void Push(string root, string url)
        {

        }
        public void Pull(string root, string url)
        {

        }

        public void Diff(string root, string version1, string version2)
        {
            var listOfVersion = GetVersions(root);
            var v1 = listOfVersion.Where(m => m.Id.Contains(version1)).FirstOrDefault();
            var v2 = listOfVersion.Where(m => m.Id.Contains(version2)).FirstOrDefault();

            if (v1 == null || v2 == null)
            {
                return;
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
        }
        private List<FileInfo> GetFiles(string root)
        {
            List<FileInfo> list = new List<FileInfo>();
            foreach (var item in Directory.GetFiles(root, "*.*", SearchOption.TopDirectoryOnly))
            {
                list.Add(new FileInfo(item));
            }
            foreach (var item in Directory.GetDirectories(root, "*.*", SearchOption.TopDirectoryOnly))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                if (directoryInfo.Name == ".git")
                {
                    continue;
                }
                foreach (var file in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    list.Add(file);
                }
            }
            return list;
        }
        private string GetStageVersionId(List<FileInfo> files, string root)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var file in files)
            {
                var path = file.FullName.Remove(0, root.Length + 1).Replace("\\", "/");
                var data = GetFileData(root, file);
                dictionary.Add(path, data);
            }
            var tempPath = Path.GetTempFileName();
            using (Stream tempStream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(tempStream, dictionary);
                tempStream.Flush();
                tempStream.Close();
            }

            var id = GetFileData(root, new FileInfo(tempPath));
            return id;
        }
        private void SaveVersion(string root, List<Version> list)
        {
            var versionFilePath = Path.Combine(root, ".git", "version");
            try
            {
                using (var versionFileStream = File.OpenWrite(versionFilePath))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(versionFileStream, list);
                }
            }
            catch { }
        }
        private List<Version> GetVersions(string root)
        {
            var versionFilePath = Path.Combine(root, ".git", "version");
            var o = new List<Version>();
            try
            {
                using (var versionFileStream = File.OpenRead(versionFilePath))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    o = (List<Version>)formatter.Deserialize(versionFileStream);
                }
            }
            catch { }
            return o;
        }
        private string GetFileData(string root, FileInfo file, bool zip = false)
        {
            byte[] buffer;
            using (var stream = file.OpenRead())
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            if (zip)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    {
                        gZipStream.Write(buffer, 0, buffer.Length);
                        gZipStream.Flush();
                    }
                    memoryStream.Position = 0;
                    buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                }
            }

            var hashData = sha1(buffer);
            var dataFilePath = Path.Combine(root, ".git", "data", hashData);
            FileInfo dataFileInfo = new FileInfo(dataFilePath);
            if (!dataFileInfo.Exists)
            {
                if (!dataFileInfo.Directory.Exists)
                {
                    dataFileInfo.Directory.Create();
                }
                using (var stream = dataFileInfo.Create())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            return hashData;
        }
        public string sha1(byte[] data)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] hash = sha1.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        public string sha1(string input)
        {
            input = input.Replace("\r\n", "\n");
            input = string.Format("blob {0}{1}{2}", input.Length, Convert.ToChar(0x0), input);
            var data = Encoding.UTF8.GetBytes(input);
            return this.sha1(data);
        }
        public void Diff(string version1, string version2)
        {
            throw new NotImplementedException();
        }

        public void Get(string version)
        {
            throw new NotImplementedException();
        }

        public void Set(string comments, string version)
        {
            throw new NotImplementedException();
        }
    }
}