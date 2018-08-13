using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.Cryptography
{
    /// <summary>
    ///解密数据
    /// </summary>
    public class Decryption
    {
        /// <summary>
        /// decrypt the string data asymmetrically using the security profile
        /// information stored in the configuration file
        /// </summary>
        /// <param name="data">encrypted string data</param>
        /// <param name="profile">security profile name</param>
        /// <param name="key">encrypted secret key</param>
        /// <param name="iv">genrated iv</param>
        /// <param name="signature">generated signature</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string data, string profile, byte[] key, byte[] iv, byte[] signature)
        {
            //convert the string to stream
            var original = new MemoryStream(Base64ToBytes(data));
            var decryptedStream = Decrypt(original, profile, key, iv, signature);
            var decryptedData = new Byte[decryptedStream.Length];
            decryptedStream.Read(decryptedData, 0, decryptedData.Length);
            return Encoding.UTF8.GetString(decryptedData);

        }


        /// <summary>
        /// decrypt the string data asymmetrically using the security profile
        /// information stored in the configuration file
        /// </summary>
        /// <param name="data">encrypted string data</param>
        /// <param name="profile">security profile name</param>
        /// <param name="key">encrypted secret key</param>
        /// <param name="iv">genrated iv</param>
        /// <param name="signature">generated signature</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string data, string profile, string key, string iv, string signature)
        {
            //convert the string to stream
            var original = new MemoryStream(Base64ToBytes(data));
            var decryptedStream = Decrypt(original, profile,key, iv, signature);
            var decryptedData = new Byte[decryptedStream.Length];
            decryptedStream.Read(decryptedData, 0, decryptedData.Length);
            return Encoding.UTF8.GetString(decryptedData);

        }

        /// <summary>
        /// decrypt the string data asymmetrically using the security profile
        /// information stored in the configuration file
        /// </summary>
        /// <param name="data">encrypted string data</param>
        /// <param name="profile">security profile name</param>
        /// <param name="key">encrypted secret key</param>
        /// <param name="iv">genrated iv</param>
        /// <param name="signature">generated signature</param>
        /// <returns>Decrypted string</returns>
        public static Stream Decrypt(Stream data, string profile, string key, string iv, string signature)
        {
            //convert the string to stream
            return Decrypt(data, profile, Convert.FromBase64String(key), Convert.FromBase64String(iv), Convert.FromBase64String(signature));
        }

        /// <summary>
        /// decrypt the stream data asymmetrically using the security profile
        /// information stored in the configuration file
        /// </summary>
        /// <param name="data">encrypted stream data</param>
        /// <param name="profile">security profile name</param>
        /// <param name="key">generated key</param>
        /// <param name="iv">generated iv</param>
        /// <param name="signature">generated signature</param>
        /// <returns>decrypted stream</returns>
        public static Stream Decrypt(Stream data, string profile, byte[] key, byte[] iv, byte[] signature)
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
            var senderCertificate = fromFile ? Certificate.LoadCertificateByFile(senderCert, password) : Certificate.SearchCertificateBySubjectName( senderCert,sendCertStore,sendCertStoreName);
            var receiverCertificate = fromFile ? Certificate.LoadCertificateByFile(receiverCert, null) : Certificate.SearchCertificateBySubjectName(receiverCert, receiverCertStore, receiverCertStoreName);

            string senderPrivateKey = senderCertificate.GetKeyAlgorithmParametersString();
            string receiverPublicKey = receiverCertificate.GetPublicKeyString();

            if (string.IsNullOrEmpty(senderPrivateKey) || string.IsNullOrEmpty(receiverPublicKey))
            {
                throw new Exception("用于签名的私有Key或公有Key为空。");
            }

            //import the public key information to verify the data
            var provider = (RSACryptoServiceProvider)receiverCertificate.PublicKey.Key;

            var ms = new MemoryStream();
            var buffer = new Byte[1024];
            int count;
            while ((count = data.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, count);
            }

            var encryptedData = ms.ToArray();
            //data.Position = 0 ;
            //data.Read(encryptedData,0,encryptedData.Length);
            //verify if the data has been tempered with	
            var v = provider.VerifyData(encryptedData, new SHA1CryptoServiceProvider(), signature);
            if (v == false)
            {
                throw new CryptographicException();
            }
            //import the private key information to decrypt data
            var provider2 = (RSACryptoServiceProvider)senderCertificate.PrivateKey;
            //decrypt the secret key and iv
            var decryptedkey = provider2.Decrypt(key, false);
            var decryptediv = provider2.Decrypt(iv, false);

            var symmProvider = SymmetricAlgorithm.Create(cm.AlgorithmProvider);
            symmProvider.Key = decryptedkey;
            symmProvider.IV = decryptediv;
            var decryptor = symmProvider.CreateDecryptor();
            ms.Position = 0;
            //decrypt the stream			
            var decStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            var decrypted = new MemoryStream();
            while ((count = decStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                decrypted.Write(buffer, 0, count);
            }
            decrypted.Position = 0;
            return decrypted;
        }

        /// <summary>
        /// decrypt the stream data symmetrically using the security profile
        /// information stored in the configuration file
        /// </summary>
        /// <param name="data">stream data</param>
        /// <param name="profile">security profile name</param>
        /// <returns>decrypted stream</returns>
        public static Stream Decrypt(Stream data, string profile)
        {
            var cm = new EncryptionConfigHelper()[profile];
            return Decrypt(data, cm);
        }

        /// <summary>
        /// decrypt the string data symmetrically using the security profile
        /// information stored in the configuration file
        /// </summary>
        /// <param name="data">encrypted string data</param>
        /// <param name="profile">security profile name</param>
        /// <returns>decrypted string</returns>
        public static string Decrypt(string data, string profile)
        {
            var cm = new EncryptionConfigHelper()[profile];
            return Decrypt(data,cm);
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data">待解密字符串</param>
        /// <param name="proEntity">解密配置</param>
        /// <returns>解密后字符串</returns>
        public static string Decrypt(string data, EncryptionConfigEntity proEntity)
        {
            //convert the string to stream
            var inputByteArray = Base64ToBytes(data);
            var original = new MemoryStream(inputByteArray);
            //MemoryStream original = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var decryptedStream = Decrypt(original, proEntity);
            var decryptedData = new Byte[decryptedStream.Length];
            decryptedStream.Read(decryptedData, 0, decryptedData.Length);
            //convert the stream to  string
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data">待解密流数据</param>
        /// <param name="proEntity">解密配置</param>
        /// <returns>解密后流数据，返回MemoryStream</returns>
        /// <exception cref="ArgumentNullException">当解密配置为null时抛出此异常</exception>
        /// <exception cref="Exception">此方法只支持对称解密，如果使用非对称配置，抛出此异常</exception>
        public static Stream Decrypt(Stream data, EncryptionConfigEntity proEntity)
        {
            if (proEntity == null)
            {
                throw new ArgumentNullException("proEntity");
            }

            if (proEntity.SymmetricAlgorithm != true)
            {
                throw new Exception("This method id not intended for asymmetric  encryption");
            }
            //retrieve the secret key and iv from the configuration file			
            var provider = proEntity.AlgorithmProvider.CreateInstance<SymmetricAlgorithm>();
            var key = proEntity.ExtentProperty["Key"];
            var iv = proEntity.ExtentProperty["IV"];

            provider.Key = Decryption.Base64ToBytes(key);
            provider.IV = Decryption.Base64ToBytes(iv);
            var decryptor = provider.CreateDecryptor();
            //decrypt the stream 
            var decStream = new CryptoStream(data, decryptor, CryptoStreamMode.Read);
            var decrypted = new MemoryStream();
            var buffer = new byte[2048];
            int count;

            while ((count = decStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                decrypted.Write(buffer, 0, count);
            }

            decrypted.Position = 0;
            return decrypted;
        }
        internal static byte[] Base64ToBytes(string data)
        {
            return Convert.FromBase64String(data);
            //var inputByteArray = new byte[data.Length / 2];
            //for (var x = 0; x < data.Length / 2; x++)
            //{
            //    var i = (Convert.ToInt32(data.Substring(x * 2, 2), 16));
            //    inputByteArray[x] = (byte)i;
            //}

            //return inputByteArray;
        }

        internal static string BytesToBase64(byte[] encryptedData)
        {
           return  Convert.ToBase64String(encryptedData);
            //var ret = new StringBuilder();
            //foreach (var b in encryptedData)
            //{
            //    ret.AppendFormat("{0:X2}", b);
            //}
            //return ret.ToString();
        }
    }
}