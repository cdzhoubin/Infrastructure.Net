using System.Configuration;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Cryptography
{
    /// <summary>
    /// 加密配置文件读取辅助类
    /// </summary>
    static class CryptionConfig
    {
        static CryptionConfig()
        {
            ConfigFile = ConfigurationManager.AppSettings["CryptographyConfigFile"];
            if (string.IsNullOrEmpty(ConfigFile))
            {
                if (!File.Exists(ConfigFile))
                {
                    ConfigFile = null;
                }
            }
            else
            {
                ConfigFile = null;
            }
        }
        public static string ConfigFile { get; private set; }
    }
}