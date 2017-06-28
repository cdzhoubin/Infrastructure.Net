using System.Configuration;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// 缓存配置区类
    /// </summary>
    public class CacheProviderSection : ConfigurationSection
    {
        /// <summary>
        /// 缓存实现类
        /// </summary>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        /// <summary>
        /// 默认缓存实现
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider", DefaultValue = "MemcachedCacheProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }

        /// <summary>
        /// 是否启用性能计数器
        /// </summary>
        [RegexStringValidator("^(true|false)$")]
        [ConfigurationProperty("enablePerformance", DefaultValue = "false")]
        public string EnablePerformance
        {
            get { return (string)base["enablePerformance"]; }
            set { base["enablePerformance"] = value; }
        }
    }
}
