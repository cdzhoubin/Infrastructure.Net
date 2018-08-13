using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.Cryptography
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
        public EncryptionConfigHelper()
            : base("EncryptionService")
        {
        }
    }
}
