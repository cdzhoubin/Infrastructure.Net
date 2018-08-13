using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.Cryptography
{
    /// <summary>
    /// 加密配置实体
    /// </summary>
    public sealed class EncryptionConfigEntity : ConfigEntityBase
    {
        /// <summary>
        /// 根据指定算法和Key创建加密配置
        /// </summary>
        /// <param name="algorithmProvider">算法</param>
        /// <param name="symmetricAlgorithm">同步异步算法</param>
        /// <param name="extentProperty">扩展数据</param>
        /// <returns>返回<see cref="EncryptionConfigEntity"/></returns>
        public static EncryptionConfigEntity CreateEncryptionConfigEntity(string algorithmProvider, bool symmetricAlgorithm, Dictionary<string, string> extentProperty)
        {
            var entity = new EncryptionConfigEntity
            {
                AlgorithmProvider = algorithmProvider,
                SymmetricAlgorithm = symmetricAlgorithm,
                ExtentProperty = extentProperty
            };
            return entity;
        }
        /// <summary>
        /// 配置构造函数
        /// </summary>
        public EncryptionConfigEntity()
        {
        }
        /// <summary>
        /// 日志处理器
        /// </summary>
        public string AlgorithmProvider { get { return GetValue<string>("AlgorithmProvider"); } set { SetValue("AlgorithmProvider", value); } }


        /// <summary>
        /// 同步算法
        /// </summary>
        public bool SymmetricAlgorithm { get { return GetValue<bool>("SymmetricAlgorithm"); } set { SetValue("SymmetricAlgorithm", value); } }



        /// <summary>
        /// 解密，算法实现和扩展属性
        /// 暂时支持对称算法
        /// </summary>
        /// <param name="entity">待解密对象</param>
        /// <returns>返回解密后对象</returns>
        internal protected override void Decrypt()
        {
            base.Decrypt();

            AlgorithmProvider = Decrypt(AlgorithmProvider);
            foreach (var keyValuePair in ExtentProperty)
            {
                ExtentProperty[keyValuePair.Key] = Decrypt(keyValuePair.Value);
            }
        }
    }
}
