using System.Configuration;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// ������������
    /// </summary>
    public class CacheProviderSection : ConfigurationSection
    {
        /// <summary>
        /// ����ʵ����
        /// </summary>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        /// <summary>
        /// Ĭ�ϻ���ʵ��
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider", DefaultValue = "MemcachedCacheProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }

        /// <summary>
        /// �Ƿ��������ܼ�����
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
