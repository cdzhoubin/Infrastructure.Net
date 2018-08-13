using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.Cryptography
{
    /// <summary>
    /// It consists methods to encrypt string and stream.
    /// </summary>
    public class Encryption
    {
        /// <summary>
        /// 非对称加密算法
        /// </summary>
        /// <param name="data">字符串</param>
        /// <param name="profile">配置文件</param>
        /// <param name="key">output parameter containing the generated secret key </param>
        /// <param name="iv">output parameter containing the generated iv key</param>
        /// <param name="signature">数字签名</param>
        /// <returns>dencrypted string</returns>
        public static string Encrypt(string data, string profile, out byte[] key, out byte[] iv, out byte[] signature)
        {
            //convert the string into stream
            var original = new MemoryStream(Encoding.UTF8.GetBytes(data));
            //encrypte the stream
            var encryptedStream = Encrypt(original, profile, out key, out iv, out signature);
            //convert the encrypted into back to string
            var encryptedData = new byte[encryptedStream.Length];
            encryptedStream.Read(encryptedData, 0, encryptedData.Length);
            return Decryption.BytesToBase64(encryptedData);

        }

        /// <summary>
        /// 非对称加密算法
        /// </summary>
        /// <param name="data">字符串</param>
        /// <param name="profile">配置文件</param>
        /// <param name="key">output parameter containing the generated secret key </param>
        /// <param name="iv">output parameter containing the generated iv key</param>
        /// <param name="signature">数字签名</param>
        /// <returns>dencrypted string</returns>
        public static string Encrypt(string data, string profile, out string key, out string iv, out string signature)
        {
            //convert the string into stream
            var original = new MemoryStream(Encoding.UTF8.GetBytes(data));
            //encrypte the stream
            var encryptedStream = Encrypt(original, profile, out key, out iv, out signature);
            //convert the encrypted into back to string
            var encryptedData = new byte[encryptedStream.Length];
            encryptedStream.Read(encryptedData, 0, encryptedData.Length);
            return Decryption.BytesToBase64(encryptedData);
        }
        /// <summary>
        /// 异步加密算法
        /// of specific security profile in the configuration file
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="profile">配置名</param>
        /// <param name="key">output parameter for generated secret key</param>
        /// <param name="iv">output parameter for generated iv</param>
        /// <param name="signature">out parameters for the digital signature</param>
        /// <returns>stream data</returns>
        public static Stream Encrypt(Stream data, string profile, out string key, out string iv, out string signature)
        {
            byte[] key1, iv1, signature1;
            var result = Encrypt(data, profile, out key1, out iv1, out signature1);
            iv = Convert.ToBase64String(iv1);
            key = Convert.ToBase64String(key1);
            signature = Convert.ToBase64String(signature1);
            return result;
        }

        /// <summary>
        /// 异步加密算法
        /// of specific security profile in the configuration file
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="profile">配置名</param>
        /// <param name="key">output parameter for generated secret key</param>
        /// <param name="iv">output parameter for generated iv</param>
        /// <param name="signature">out parameters for the digital signature</param>
        /// <returns>stream data</returns>
        public static Stream Encrypt(Stream data, string profile, out byte[] key, out byte[] iv, out byte[] signature)
        {
            var helper = new EncryptionConfigHelper();
            var cm = helper[profile];


            if (cm.SymmetricAlgorithm)
            {
                throw new Exception("This method id not intended for symmetric  encryption");
            }

            //retireve the sneder and receiver's certification information for encryption
            var senderCert = cm.ExtentProperty["SenderCertificate"];
            string sendCertStore = null;
            string sendCertStoreName = null;
            string password = null;

            var receiverCert = cm.ExtentProperty["ReceiverCertificate"];
            string receiverCertStore = null;
            string receiverCertStoreName = null;

            var fromFile = cm.ExtentProperty["CertificateFile"] == "true";
            if (!fromFile)
            {
                if (cm.ExtentProperty.ContainsKey("SenderCertificateStore"))
                    sendCertStore = cm.ExtentProperty["SenderCertificateStore"];
                if (cm.ExtentProperty.ContainsKey("SenderCertificateStoreName"))
                    sendCertStoreName = cm.ExtentProperty["SenderCertificateStoreName"];

                if (cm.ExtentProperty.ContainsKey("ReceiverCertificateStore"))
                    receiverCertStore = cm.ExtentProperty["ReceiverCertificateStore"];

                if (cm.ExtentProperty.ContainsKey("ReceiverCertificateStoreName"))
                    receiverCertStoreName = cm.ExtentProperty["ReceiverCertificateStoreName"];
            }
            else
            {
                password = cm.ExtentProperty["SenderCertificatePassword"];
            }

            //obtain the X509 certificate object for the sender and receiver
            var senderCertificate = fromFile ? Certificate.LoadCertificateByFile(senderCert, password) : Certificate.SearchCertificateBySubjectName(senderCert, sendCertStore, sendCertStoreName);
            var receiverCertificate = fromFile ? Certificate.LoadCertificateByFile(receiverCert, null) : Certificate.SearchCertificateBySubjectName(receiverCert, receiverCertStore, receiverCertStoreName);


            var symmProvider = cm.AlgorithmProvider.CreateInstance<SymmetricAlgorithm>();

            ICryptoTransform encryptor = symmProvider.CreateEncryptor();

            var encStream = new CryptoStream(data, encryptor, CryptoStreamMode.Read);
            var encrypted = new MemoryStream();
            var buffer = new byte[1024];
            int count;
            while ((count = encStream.Read(buffer, 0, 1024)) > 0)
            {
                encrypted.Write(buffer, 0, count);
            }
            //encrypt the screte key, iv key using receiver's public key
            //that are used to decrypt the data
            var provider = (RSACryptoServiceProvider)receiverCertificate.PublicKey.Key;

            key = provider.Encrypt(symmProvider.Key, false);
            iv = provider.Encrypt(symmProvider.IV, false);

            //sign the data with sender's private key
            var provider2 = (RSACryptoServiceProvider)senderCertificate.PrivateKey;
            signature = provider2.SignData(encrypted.ToArray(), new SHA1CryptoServiceProvider());
            encrypted.Position = 0;
            return encrypted;
        }

        /// <summary>
        /// 同步加密算法
        /// </summary>
        /// <param name="data">stream data</param>
        /// <param name="profile">profile name</param>
        /// <returns>stream data</returns>
        public static Stream Encrypt(Stream data, string profile)
        {
            var cm = new EncryptionConfigHelper()[profile];
            return Encrypt(data, cm);
        }

        /// <summary>
        ///  同步加密算法
        /// </summary>
        /// <param name="data">string data</param>
        /// <param name="profile">security profile name</param>
        /// <returns>encrypted string</returns>
        public static string Encrypt(string data, string profile)
        {
            var cm = new EncryptionConfigHelper()[profile];
            return Encrypt(data, cm);
        }

        /// <summary>
        /// 流数据加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="proEntity">加密配置</param>
        /// <returns>加密后流数据，返回MemoryStream</returns>
        /// <exception cref="ArgumentNullException">当加密配置为null时抛出此异常</exception>
        /// <exception cref="Exception">此方法只支持对称加密，如果使用非对称配置，抛出此异常</exception>
        public static Stream Encrypt(Stream data, EncryptionConfigEntity proEntity)
        {
            if (proEntity == null)
            {
                throw new ArgumentNullException("proEntity");
            }

            if (proEntity.SymmetricAlgorithm == false)
            {
                throw new Exception("This method id not intended for asymmetric  encryption");
            }

            //retrive the secret key and iv information
            var provider = proEntity.AlgorithmProvider.CreateInstance<SymmetricAlgorithm>();
            provider.Key = Decryption.Base64ToBytes(proEntity.ExtentProperty["Key"]);
            provider.IV = Decryption.Base64ToBytes(proEntity.ExtentProperty["IV"]);

            var encryptor = provider.CreateEncryptor();
            var encrypted = new MemoryStream();
            //encrypt the stream symmetrically
            var encStream = new CryptoStream(encrypted, encryptor, CryptoStreamMode.Write);

            var buffer = new byte[1024];
            int count;
            while ((count = data.Read(buffer, 0, 1024)) > 0)
            {
                encStream.Write(buffer, 0, count);
            }
            encStream.FlushFinalBlock();
            encrypted.Position = 0;
            return encrypted;
        }

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="data">待加密数据</param>
        /// <param name="proEntity">加密配置</param>
        /// <returns>返回加密后数据</returns>
        public static string Encrypt(string data, EncryptionConfigEntity proEntity)
        {
            var original = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var encryptedStream = Encrypt(original, proEntity);
            var encryptedData = new Byte[encryptedStream.Length];
            encryptedStream.Read(encryptedData, 0, encryptedData.Length);
            //convert the encrytped stream to string
            return Decryption.BytesToBase64(encryptedData);
        }
    }


}
