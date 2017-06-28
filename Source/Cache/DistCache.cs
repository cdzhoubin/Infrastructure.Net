using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// 分布式缓存实现
    /// </summary>
    public static class DistCache
    {
        static DistCache()
        {
            LoadProvider();
        }
        private static volatile CacheProvider _objProvider;
        private static CacheProviderCollection _objProviders;
        private static readonly object Lock = new object();

        /// <summary>
        /// 根据名称获取缓存实例
        /// </summary>
        /// <param name="providerName">配置名称</param>
        /// <exception cref="ProviderException">当providerName对应的&lt;see cref="CacheProvider"/&gt;不存在时，抛出此异常</exception>
        /// <returns>返回创建成功的&lt;see cref="CacheProvider"/&gt;</returns>
        public static CacheProvider GetCacheProvider(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return _objProvider;
            }
            var provider = _objProviders[providerName];
            if (provider == null)
            {
                throw new ProviderException("Unable to load " + providerName + " cache provider");
            }
            return provider;

        }

        /// <summary>
        /// 修改默认使用的Provider
        /// </summary>
        /// <param name="providerName">名称</param>
        /// <exception cref="ProviderException">当provider未找到时抛出此异常。</exception>
        public static void ChangeProvider(string providerName)
        {
            lock (Lock)
            {
                _objProvider = _objProviders[providerName];
            }
            if (_objProvider == null)
            {
                throw new ProviderException("Unable to load " + providerName + " cache provider");
            }
        }

        /// <summary>
        /// 从默认配置文件中加载缓存配置信息
        /// </summary>
        private static void LoadProvider()
        {
            // Avoid claiming lock if provider already loaded
            if (_objProvider != null)
            {
                return;
            }

            lock (Lock)
            {
                //make sure _provider is still null
                if (_objProvider != null)
                {
                    return;
                }

                //Get a reference to the <cacheProvider> section 
                var objSection = (CacheProviderSection)
                                 WebConfigurationManager.GetSection("cacheProvider");

                //Load registered providers and point _objProvider to the default provider
                _objProviders = new CacheProviderCollection();
                ProvidersHelper.InstantiateProviders
                    (objSection.Providers, _objProviders, typeof(CacheProvider));
                _objProvider = _objProviders[objSection.DefaultProvider];

                if (_objProvider == null)
                {
                    throw new ProviderException("Unable to load default cache provider");
                }

                //creating performance counter categories
                if (_objProvider.EnablePerformance)
                {
                    _objProvider.CheckPerformanceCounterCategories();
                }
            }
        }

        /// <summary>
        /// 默认过期时间
        /// </summary>
        public static ulong DefaultExpireTime
        {
            get
            {
                return _objProvider.DefaultExpireTime;
            }

            set
            {
                _objProvider.DefaultExpireTime = value;
            }
        }

        /// <summary>
        /// Key前缘
        /// </summary>
        public static string KeySuffix
        {
            get
            {
                return _objProvider.KeySuffix;
            }

            set
            {
                _objProvider.KeySuffix = value;
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="objValue">对象</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回true,其它返回false</returns>
        public static bool Add(string strKey, object objValue, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="objValue">对象</param>
        /// <param name="bDefaultTimeSpan">使用默认过期时间</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回true,其它返回false</returns>
        public static bool Add(string strKey, object objValue, bool bDefaultTimeSpan, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue, bDefaultTimeSpan);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="objValue">对象</param>
        /// <param name="lNumofMilliSeconds">过期时间,单位:毫秒</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回true,其它返回false</returns>
        public static bool Add(string strKey, object objValue, long lNumofMilliSeconds, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue, lNumofMilliSeconds);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="objValue">对象</param>
        /// <param name="tspan">过期时间</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回true,其它返回false</returns>
        public static bool Add(string strKey, object objValue, TimeSpan tspan, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue, tspan);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="objValue">对象</param>
        /// <param name="tspan">过期时间</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回true,其它返回false</returns>
        public static bool AddForSliding(string strKey, object objValue, TimeSpan tspan, string providerName = null)
        {
            return GetCacheProvider(providerName).AddForSliding(strKey, objValue, tspan);
        }

        /// <summary>
        /// 获取指定键值
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回获取到的值</returns>
        public static object Get(string strKey, string providerName = null)
        {
            return GetCacheProvider(providerName).Get(strKey);
        }

        /// <summary>
        /// 获取指定键的指定类型
        /// </summary>
        /// <param name="strKey">键</param>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回对象,失败返回null</returns>
        public static T Get<T>(string strKey, string providerName = null)
        {
            return GetCacheProvider(providerName).Get<T>(strKey);
        }

        /// <summary>
        /// 获取一组缓存数据
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <returns>成功返回字典数据</returns>
        [Obsolete("请使用方法IDictionary<string, object> Get(IEnumerable<string> keys, string providerName = null)替换")]
        public static IDictionary<string, object> Get(params string[] keys)
        {
            return _objProvider.Get(keys);
        }
        /// <summary>
        /// 获取一组缓存数据
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="providerName">名称</param>
        /// <returns>成功返回字典数据</returns>
        public static IDictionary<string, object> Get(IEnumerable<string> keys, string providerName = null)
        {
            return GetCacheProvider(providerName).Get(keys.ToArray());
        }

        /// <summary>
        /// 移除指定键值缓存数据
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="providerName">名称</param>
        /// <returns>返回删除后的对象</returns>
        public static object Remove(string strKey, string providerName = null)
        {
            return GetCacheProvider(providerName).Remove(strKey);
        }

        /// <summary>
        /// 移除所有
        /// </summary>
        /// <param name="providerName">名称</param>
        public static void RemoveAll(string providerName = null)
        {
            GetCacheProvider(providerName).RemoveAll();
        }

        /// <summary>
        /// 增加缓存键值保留时间
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="lAmount">增加缓存时间，单位：毫秒</param>
        /// <param name="providerName">名称</param>
        /// <returns>返回增加后的缓存时间</returns>
        public static ulong Increment(string strKey, ulong lAmount, string providerName = null)
        {
            return GetCacheProvider(providerName).Increment(strKey, lAmount);
        }

        /// <summary>
        /// 减少缓存键值保留时间
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="lAmount">减少时间，单位：毫秒</param>
        /// <param name="providerName">名称</param>
        /// <returns>返回减少后的缓存时间</returns>
        public static ulong Decrement(string strKey, ulong lAmount, string providerName = null)
        {
            return GetCacheProvider(providerName).Decrement(strKey, lAmount);
        }

    }
}
