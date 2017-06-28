using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Zhoubin.Infrastructure.Common.Cryptography
{
    /// <summary>
    /// It provides certificate search ability with 
    /// a given location and subject.
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// Retrieve the X509 certificate for a given subject name and location
        /// </summary>
        /// <param name="location">either CurrentUser store or LocalMachine store</param>
        /// <param name="subjectName">subject name</param>
        /// <param name="name">存储区域名称</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 SearchCertificateBySubjectName(string subjectName,string location,  string name)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                return null;
            }

            return subjectName.ToCertificate(string.IsNullOrEmpty(name) ? StoreName.My : name.ToStoreName(), string.IsNullOrEmpty(location) ? StoreLocation.LocalMachine : location.ToStoreLocation());
        }

        /// <summary>
        /// 从文件加载证书
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <param name="password">密码</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 LoadCertificateByFile(string file,string password)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            if (password == null)
            {
                return new X509Certificate2(file);
            }

            return new X509Certificate2(file,password);
        }
    }

    /// <summary>
    /// 证书帮助类
    /// </summary>
    public static class CertificateHelper
    {

        /// <summary>
        /// 证书存储名称转换为存储枚举
        /// </summary>
        /// <param name="storeName">存储名</param>
        /// <returns>返回存储名对象</returns>
        /// <exception cref="ArgumentNullException">storeName为null或空时抛出此异常</exception>
        /// <exception cref="ArgumentException">转换出错时，抛出此异常</exception>
        public static StoreName ToStoreName(this string storeName)
        {
            if (string.IsNullOrEmpty(storeName))
            {
                throw new ArgumentNullException("storeName");
            }

            StoreName name;
            if (Enum.TryParse(storeName, true, out name))
            {
                return name;
            }

            throw new ArgumentException("转换存储名称出错："+storeName);
        }

        /// <summary>
        /// 存储路径转换为存储路径枚举
        /// </summary>
        /// <param name="storeLocation">存储路径字符串</param>
        /// <returns>返回存储路径枚举</returns>
        /// <exception cref="ArgumentNullException">当storeLocation为null或空时，抛出此异常</exception>
        /// <exception cref="ArgumentException">转换出错时，抛出此异常</exception>
        public static StoreLocation ToStoreLocation(this string storeLocation)
        {
            if (string.IsNullOrEmpty(storeLocation))
            {
                throw new ArgumentNullException("storeLocation");
            }

            StoreLocation name;
            if (Enum.TryParse(storeLocation, true, out name))
            {
                return name;
            }

            throw new ArgumentException("转换存储名称出错：" + storeLocation);
        }

        /// <summary>
        /// 根据标题查询证书
        /// </summary>
        /// <param name="subjectName">证书主题</param>
        /// <param name="name">存储名</param>
        /// <param name="location">存储路径</param>
        /// <returns>返回x509证书对象</returns>
        public static X509Certificate2 ToCertificate(this string subjectName,
                                                        StoreName name = StoreName.My,
                                                            StoreLocation location = StoreLocation.LocalMachine)
        {
            var store = new X509Store(name, location);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var cert = store.Certificates.OfType<X509Certificate2>()
                            .FirstOrDefault(c => c.SubjectName.Name != null && c.SubjectName.Name.Equals(subjectName,
                                StringComparison.OrdinalIgnoreCase));

                return (cert != null) ? new X509Certificate2(cert) : null;
            }
            finally
            {
                store.Certificates.OfType<X509Certificate2>().ToList().ForEach(c => c.Reset());
                store.Close();
            }
        }
    }
}

