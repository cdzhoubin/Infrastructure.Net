using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Cryptography
{
    /// <summary>
    /// 签名数据
    /// </summary>
    public class SignData
    {
        /// <summary>
        /// 签名数据的类型
        /// </summary>
        public class SignType
        {
            readonly string _name = "";
            private SignType(string name)
            {
                _name = name;
            }
            /// <summary>
            /// SHA1签名算法
            /// </summary>
// ReSharper disable once InconsistentNaming
            public static SignType SHA1 = new SignType("SHA1");
            //public static SignType SHA1Managed = new SignType("SHA1Managed");
            /// <summary>
            /// SHA256签名算法
            /// </summary>
// ReSharper disable once InconsistentNaming
            public static SignType SHA256 = new SignType("SHA256");
            //public static SignType SHA256Managed = new SignType("SHA256Managed");
            /// <summary>
            /// SHA384签名算法
            /// </summary>
// ReSharper disable once InconsistentNaming
            public static SignType SHA384 = new SignType("SHA384");
            // public static SignType SHA384Managed = new SignType("SHA384Managed");
            /// <summary>
            /// SHA512签名算法
            /// </summary>
// ReSharper disable once InconsistentNaming
            public static SignType SHA512 = new SignType("SHA512");
            // public static SignType SHA512Managed = new SignType("SHA512Managed");
            /// <summary>
            /// MD5签名算法
            /// </summary>
// ReSharper disable once InconsistentNaming
            public static SignType MD5 = new SignType("MD5");

            /// <summary>
            /// 确定指定的 <see cref="T:System.Object"/> 是否等于当前的 <see cref="T:System.Object"/>。
            /// </summary>
            /// <returns>
            /// 如果指定的 <see cref="T:System.Object"/> 等于当前的 <see cref="T:System.Object"/>，则为 true；否则为 false。
            /// </returns>
            /// <param name="obj">与当前的 <see cref="T:System.Object"/> 进行比较的 <see cref="T:System.Object"/>。</param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                var sign = obj as SignType;
                if (sign == null)
                    return false;
                return _name == sign._name;
            }

            /// <summary>
            /// 用作特定类型的哈希函数。
            /// </summary>
            /// <returns>
            /// 当前 <see cref="T:System.Object"/> 的哈希代码。
            /// </returns>
            public override int GetHashCode()
            {
                return _name.GetHashCode();
            }
            /// <summary>
            /// 比较两个签名算法类型是否相同
            /// </summary>
            /// <param name="ob1">对象一</param>
            /// <param name="ob2">对象二</param>
            /// <returns>名称相同，或者都为null时相同，其它不同</returns>
            public static bool operator ==(SignType ob1, SignType ob2)
            {
                if (ob1 as object == null && ob2 as object == null)
                {
                    return true;
                }

                if (ob1 as object == null || ob2 as object == null)
                {
                    return false;
                }

                if (ob1._name == ob2._name)
                    return true;
                return false;
            }
            /// <summary>
            /// 不等比较
            /// </summary>
            /// <param name="ob1">对象一</param>
            /// <param name="ob2">对象二</param>
            /// <returns>当对象不同时为null或同时不为null,但名称不同时，为true,其它为false</returns>
            public static bool operator !=(SignType ob1, SignType ob2)
            {
                return (!(ob1 == ob2));
            }

            /// <summary>
            /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
            /// </summary>
            /// <returns>
            /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override string ToString()
            {
                return _name;
            }
        }

        /// <summary>
        /// MD5签名数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>签名数据</returns>
        public static string Sign(string data)
        {
            return Sign(data, SignType.MD5);
        }
        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">签名数据的类型</param>
        /// <returns>签名数据</returns>
        public static string Sign(string data, SignType type)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var result = Sign(new MemoryStream(bytes), type);
            var ret = new StringBuilder();
            foreach (var b in result)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }
        /// <summary>
        /// 签名流数据
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="type">签名算法</param>
        /// <returns>签名数据</returns>
        public static string Sign(Stream data, SignType type)
        {
            var result = SignStream(data, type);
            if(result == null)
            {
                return null;
            }
            var ret = new StringBuilder();
            foreach (var b in result)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }
        //private static IEnumerable<byte> Sign(byte[] data, SignType type)
        //{
        //    var hashAlgorithm = HashAlgorithm.Create(type.ToString());
        //    if (hashAlgorithm != null)
        //        return hashAlgorithm.ComputeHash(data);
        //    return null;
        //}

        private static IEnumerable<byte> SignStream(Stream data, SignType type)
        {
            var hashAlgorithm = HashAlgorithm.Create(type.ToString());
            if (hashAlgorithm != null)
                return hashAlgorithm.ComputeHash(data);
            return null;
        }

        /// <summary>
        /// MD5检查数据签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="checkData">校验数据</param>
        /// <returns>校验通过，返回true,其它返回false</returns>
        public static bool CheckSign(string data, string checkData)
        {
            return (checkData == Sign(data));
        }
        /// <summary>
        /// 检查数据签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="checkData">校验数据</param>
        /// <param name="type">签名数据的类型</param>
        /// <returns>校验通过，返回true,其它返回false</returns>
        public static bool CheckSign(string data, string checkData, SignType type)
        {
            return (checkData == Sign(data, type));
        }
        /// <summary>
        /// 签名流数据
        /// </summary>
        /// <param name="data">签名数据</param>
        /// <param name="checkData">检验数据</param>
        /// <param name="type">检验签名类型</param>
        /// <returns>校验通过，返回true,其它返回false</returns>
        public static bool CheckSign(Stream data, string checkData, SignType type)
        {
            return (checkData == Sign(data, type));
        }
    }
}