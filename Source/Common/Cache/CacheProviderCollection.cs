using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.Cache
{    

    /// <summary>
    /// ����ʵ��Collection��
    /// </summary>
    public class CacheProviderCollection : ConfigHelper<CacheConfig>
    {
        public CacheProviderCollection() : base("CacheProvider")
        {
        }

        /// <summary>
        /// �������ƻ�ȡ����ʵ��
        /// </summary>
        /// <param name="name"></param>
        public new CacheProvider this[string name]
        {
            get
            {
                return base[name].ProviderInstance;
            }
        }
    }    
}
