using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 压缩文件帮助类
    /// 此类用于简化生成压缩文件
    /// </summary>
    public static class ZipHelper
    {
        /// <summary>
        /// 压缩文件到指定的流
        /// </summary>
        /// <param name="zipStreams">待压缩流字典</param>
        /// <param name="password">压缩密码，默认不设置密码</param>
        /// <returns>返回压缩生成的流</returns>
        public static Stream Zip(Dictionary<string, Stream> zipStreams, string password = null)
        {
            var stream = new MemoryStream();
            using (var zip = new ZipFile())
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }
                foreach (var str in zipStreams.Keys)
                {
                    zip.AddEntry(str, zipStreams[str]);
                }

                zip.Save(stream);
            }

            return stream;
        }
        /// <summary>
        /// 解压缩文件流
        /// </summary>
        /// <param name="stream">待解压文件流</param>
        /// <param name="password">密码</param>
        /// <returns>解压后的流字典数据</returns>
        public static Dictionary<string, Stream> UnZip(Stream stream, string password = null)
        {
            using (var zip = ZipFile.Read(stream))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }

                return zip.ToDictionary<ZipEntry, string, Stream>(
                    entry => entry.FileName, entry =>
                    {
                        var stream1 = new MemoryStream();
                        entry.Extract(stream1);
                        return stream1;
                    });
            }
        }

        /// <summary>
        /// 压缩指定的目录
        /// </summary>
        /// <param name="folder">目录名称</param>
        /// <param name="fileName">生成的文件名称</param>
        /// <param name="includeSubFolder">是否包括子目录，默认值不包括</param>
        /// <param name="password">压缩密码</param>
        /// <exception cref="DirectoryNotFoundException">目录不存在抛出此异常</exception>
        /// <exception cref="FileLoadException">文件已经存在抛出此异常</exception>
        public static void ZipFolder(string folder, string fileName, bool includeSubFolder = false, string password = null)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException(folder);
            }

            folder = new DirectoryInfo(folder).FullName;
            if (File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            var store = new FileInfo(fileName).DirectoryName;
            if (store == null)
            {
                throw new FileLoadException("文件路径不正确。");
            }
            if (!Directory.Exists(store))
            {
                Directory.CreateDirectory(store);
            }

            using (var zip = new ZipFile())
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }

                ZipFolder(zip, folder, includeSubFolder);
                zip.Save(fileName);
            }
        }

        static void ZipFolder(ZipFile zip, string folder, bool includeSubFolder, string baseFoler = null)
        {
            baseFoler = baseFoler ?? folder;
            var dir = new DirectoryInfo(folder);
            foreach (var file in dir.GetFiles())
            {
                zip.AddEntry(file.FullName.Replace(baseFoler, ""),
                    new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
            }

            if (includeSubFolder)
            {
                foreach (var directory in dir.GetDirectories())
                {
                    ZipFolder(zip, directory.FullName, true, baseFoler);
                }
            }
        }

        /// <summary>
        /// 解压文件到指定目录
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="folder">目标目录</param>
        /// <param name="password">密码</param>
        /// <exception cref="FileNotFoundException">文件不存在时，抛出此异常</exception>
        public static void UnZipFolder(string fileName, string folder, string password = null)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (var zip = ZipFile.Read(fileName))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }

                foreach (var entry in zip)
                {
                    entry.Extract(folder);
                }
            }
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="password">密码</param>
        /// <returns>返回压缩后的字符串</returns>
        public static string Zip(string content, string password = null)
        {
            using (var zip = new ZipFile())
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }

                zip.AddEntry("content", new MemoryStream(Encoding.UTF8.GetBytes(content)));
                var stream = new MemoryStream();
                zip.Save(stream);
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }


        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="password">密码</param>
        /// <returns>返回解压后的字符串</returns>
        public static string UnZip(string content, string password = null)
        {
            using (var zip = ZipFile.Read(new MemoryStream(System.Convert.FromBase64String(content))))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }

                var entry = zip.FirstOrDefault();
                if (entry != null && entry.FileName == "content")
                {
                    var stream = new MemoryStream();
                    entry.Extract(stream);
                    return Encoding.UTF8.GetString(stream.GetBuffer()).TrimEnd('\0');
                }

                return null;
            }
        }
    }
}
