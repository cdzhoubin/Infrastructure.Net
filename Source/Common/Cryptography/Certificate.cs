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
        /// <param name="name">�洢��������</param>
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
        /// ���ļ�����֤��
        /// </summary>
        /// <param name="file">�ļ�·��</param>
        /// <param name="password">����</param>
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
    /// ֤�������
    /// </summary>
    public static class CertificateHelper
    {

        /// <summary>
        /// ֤��洢����ת��Ϊ�洢ö��
        /// </summary>
        /// <param name="storeName">�洢��</param>
        /// <returns>���ش洢������</returns>
        /// <exception cref="ArgumentNullException">storeNameΪnull���ʱ�׳����쳣</exception>
        /// <exception cref="ArgumentException">ת������ʱ���׳����쳣</exception>
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

            throw new ArgumentException("ת���洢���Ƴ���"+storeName);
        }

        /// <summary>
        /// �洢·��ת��Ϊ�洢·��ö��
        /// </summary>
        /// <param name="storeLocation">�洢·���ַ���</param>
        /// <returns>���ش洢·��ö��</returns>
        /// <exception cref="ArgumentNullException">��storeLocationΪnull���ʱ���׳����쳣</exception>
        /// <exception cref="ArgumentException">ת������ʱ���׳����쳣</exception>
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

            throw new ArgumentException("ת���洢���Ƴ���" + storeLocation);
        }

        /// <summary>
        /// ���ݱ����ѯ֤��
        /// </summary>
        /// <param name="subjectName">֤������</param>
        /// <param name="name">�洢��</param>
        /// <param name="location">�洢·��</param>
        /// <returns>����x509֤�����</returns>
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

