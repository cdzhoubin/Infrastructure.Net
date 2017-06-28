using System;
using System.Configuration.Provider;
using Zhoubin.Infrastructure.Common.Properties;

namespace Zhoubin.Infrastructure.Cache
{    

    /// <summary>
    /// 缓存实现Collection类
    /// </summary>
    public class CacheProviderCollection : ProviderCollection
    {
        /// <summary>
        /// 根据名称获取缓存实现
        /// </summary>
        /// <param name="name"></param>
        public new CacheProvider this[string name]
        {
            get
            {
                return (CacheProvider)base[name];
            }
        }

        /// <summary>
        /// 添加缓存实现
        /// </summary>
        /// <param name="provider">缓存实现</param>
        /// <exception cref="ArgumentNullException">当provider为null时，抛出此异常</exception>
        /// <exception cref="ArgumentException">当provider不是CacheProvider的子类时，抛出此异常</exception>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is CacheProvider))
                throw new ArgumentException(Resources.InvalidProviderType, "provider");

            base.Add(provider);
        }
    }    
}
