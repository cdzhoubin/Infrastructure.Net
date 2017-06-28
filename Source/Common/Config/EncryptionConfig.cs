using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common.Config
{
    /// <summary>
    /// 加密配置帮助类
    /// </summary>
    public sealed class EncryptionConfigHelper : ConfigHelper<EncryptionConfigEntity>
    {
        /// <summary>
        /// 日志配置帮助类
        /// </summary>
        /// <param name="configFile">配置文件</param>
        public EncryptionConfigHelper(string configFile)
            : base("EncryptionService", configFile)
        {
        }
    }

    /// <summary>
    /// 加密配置实体
    /// </summary>
    public sealed class EncryptionConfigEntity : ConfigEntityBase<EncryptionConfigEntity>
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
            ExtentProperty = new Dictionary<string, string>();
        }
        /// <summary>
        /// 日志处理器
        /// </summary>
        public string AlgorithmProvider { get; private set; }


        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, string> ExtentProperty { get; private set; }

        /// <summary>
        /// 同步算法
        /// </summary>
        public bool SymmetricAlgorithm { get; private set; }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="node">结点</param>
        protected override void SetProperty(EncryptionConfigEntity entity, System.Xml.XmlNode node)
        {
            switch (node.Name)
            {
                case "AlgorithmProvider":
                    entity.AlgorithmProvider = node.InnerText;
                    break;
                case "SymmetricAlgorithm":
                    entity.SymmetricAlgorithm = node.InnerText.ToLower() == "true";
                    break;
                default:
                    entity.ExtentProperty.Add(node.Name, node.InnerText);
                    break;
            }
        }

        /// <summary>
        /// 解密，算法实现和扩展属性
        /// 暂时支持对称算法
        /// </summary>
        /// <param name="entity">待解密对象</param>
        /// <returns>返回解密后对象</returns>
        protected override EncryptionConfigEntity Decrypt(EncryptionConfigEntity entity)
        {
            var ent = base.Decrypt(entity);

            ent.AlgorithmProvider = Decrypt(ent.AlgorithmProvider);
            foreach (var keyValuePair in ent.ExtentProperty)
            {
                ent.ExtentProperty[keyValuePair.Key] = Decrypt(keyValuePair.Value);
            }

            return ent;
        }
    }

    /// <summary>
    /// 加密服务配置读取SectionHandle
    /// </summary>
    public sealed class EncryptionService : ConfigurationSectionHandlerHelper<EncryptionConfigEntity>
    {

    }
}
