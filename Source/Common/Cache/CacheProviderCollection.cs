using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.Cache
{    

    /// <summary>
    /// 缓存实现Collection类
    /// </summary>
    public class CacheProviderCollection : ConfigHelper<CacheConfig>
    {
        public CacheProviderCollection() : base("CacheProvider")
        {
        }

        /// <summary>
        /// 根据名称获取缓存实现
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
