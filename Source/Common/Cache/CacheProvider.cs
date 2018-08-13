using System;
using System.Collections.Generic;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.Cache
{

    /// <summary>
    /// CacheProvider基类
    /// 所有实现都从此基类继承
    /// </summary>
    public abstract class CacheProvider :IDisposable
    {
        #region Membership Variables        

        /// <summary>
        /// 构造函数
        /// </summary>
        protected CacheProvider(string providerName)
        {
            ProviderName = providerName;
            KeySuffix = string.Empty;
            DefaultExpireTime = 2;
        }

        #endregion
        
        public string ProviderName { get; private set; }
        /// <summary>
        /// 默认过期时间
        /// 单位秒
        /// </summary>
        public ulong DefaultExpireTime { get; private set; }

        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeySuffix { get; private set; }

        static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }
        protected  CacheConfig CacheConfig { get; private set; }
        public void Initialize(CacheConfig config)
        {
            CacheConfig = config;
            KeySuffix = config.KeySuffix;
            DefaultExpireTime = config.DefaultExpireTime;
            Init(config);
        }

        protected abstract void Init(CacheConfig config);
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <returns>成功返回true,其它返回false</returns>
        public abstract bool Add(string strKey, object objValue);
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="bDefaultExpire">是否默认过期时间</param>
        /// <returns>成功返回true,其它返回false</returns>
        public bool Add(string strKey, object objValue, bool bDefaultExpire)
        {
            if (!bDefaultExpire)
            {
                return Add(strKey, objValue);
            }

            return Add(strKey, objValue, TimeSpan.FromSeconds(DefaultExpireTime));
        }
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="lNumofMilliSeconds">过期时间，在当前日期的基础上加上此时间，单位毫秒</param>
        /// <returns>成功返回true,其它返回false</returns>
        public bool Add(string strKey, object objValue, long lNumofMilliSeconds)
        {
            return Add(strKey, objValue,TimeSpan.FromMilliseconds(lNumofMilliSeconds));
        }
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="timeSpan">过期时间，在当前日期的基础上加上此时间</param>
        /// <returns>成功返回true,其它返回false</returns>
        public abstract bool Add(string strKey, object objValue, TimeSpan timeSpan);

        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="time">绝对过期日期</param>
        /// <returns>成功返回true,其它返回false</returns>
        public bool Add(string strKey, object objValue, DateTime time)
        {
            return Add(strKey, objValue, TimeSpan.FromTicks((time.ToUniversalTime() - DateTime.UtcNow).Ticks));
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>返回缓存对象</returns>
        public abstract object Get(string strKey);
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <returns>成功返回读取到的对象,失败返回默认值</returns>
        public abstract T Get<T>(string strKey);
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="keys">一组Key</param>
        /// <returns>返回一组数据组对象</returns>
        public abstract IDictionary<string, object> Get(params string[] keys);
        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public abstract void RemoveAll();
        /// <summary>
        /// 移除指定键缓存
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>成功返回true,其它返回false</returns>
        public abstract bool Remove(string strKey);
        /// <summary>
        /// 增加缓存时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">时间</param>
        /// <returns>返回增加后的时间</returns>
        public abstract ulong Increment(string key, ulong amount);
        /// <summary>
        /// 减少缓存时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">时间</param>
        /// <returns>返回减少后的缓存时间</returns>
        public abstract ulong Decrement(string key, ulong amount);


        

        /// <summary>
        /// 释放对象
        /// </summary>
        public abstract void Dispose();

        #region 滑动日期实现
        /// <summary>
        /// 新增默认日期滑动缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <returns>成功返回true,其它返回false</returns>
        public bool AddForSliding(string strKey, object objValue)
        {
            return AddForSliding(strKey, objValue, TimeSpan.FromMilliseconds(DefaultExpireTime));
        }
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="lNumofMilliSeconds">过期时间，在当前日期的基础上加上此时间，单位毫秒</param>
        /// <returns>成功返回true,其它返回false</returns>
        public bool AddForSliding(string strKey, object objValue, long lNumofMilliSeconds)
        {
            return AddForSliding(strKey, objValue, TimeSpan.FromMilliseconds(lNumofMilliSeconds));
        }
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="timeSpan">过期时间，在当前日期的基础上加上此时间</param>
        /// <returns>成功返回true,其它返回false</returns>
        public abstract bool AddForSliding(string strKey, object objValue, TimeSpan timeSpan);
        #endregion
    }


}
