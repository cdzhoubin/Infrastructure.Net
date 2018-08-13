

using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase.Core;

namespace Zhoubin.Infrastructure.Common.Cache.MemcachedCache
{
    /// <summary>
    /// Couchbase缓存实现
    /// </summary>
    public class MemcachedCacheProvider : MemcachedCacheProviderBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MemcachedCacheProvider()
            : base("Memcached Cache Provider")
        {

        }


        #region Cache Provider


        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <returns>成功返回true,其它返回false</returns>
        public override bool Add(string strKey, object objValue)
        {
            return Excute(bucket =>
            {
                var result = bucket.Insert(KeySuffix + strKey, objValue);
                return result.Success;
            });
        }

        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="timeSpan">过期时间，在当前日期的基础上加上此时间</param>
        /// <returns>成功返回true,其它返回false</returns>
        public override bool Add(string strKey, object objValue, TimeSpan timeSpan)
        {
            return Excute(bucket => bucket.Insert(KeySuffix + strKey, objValue, timeSpan).Success);
        }


        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>返回缓存对象</returns>
        public override object Get(string strKey)
        {
            return Excute(bucket => {
                var result = bucket.Get<object>(KeySuffix + strKey);
                if (result.Success)
                {
                    return result.Value;
                }
                return null;
            });
        }


        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="keys">一组Key</param>
        /// <returns>返回一组数据组对象</returns>
        public override IDictionary<string, object> Get(params string[] keys)
        {

            IList<string> keysList = keys.Select(str => KeySuffix + str).ToList();

            return Excute(bucket =>
            {
                IDictionary<string, object> retVal = new Dictionary<string, object>();
                foreach (var key in keysList)
                {
                    var task = bucket.GetAsync<object>(key);
                    if (task.Result.Success)
                    {
                        retVal.Add(key.Remove(0, KeySuffix.Length), task.Result.Value);
                    }
                }

                return retVal;
            });
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public override void RemoveAll()
        {
            Action<IBucket> func = bucket =>
            {
                var user = CacheConfig.ExtentProperty["User"];
                var manager = string.IsNullOrEmpty(user) ? bucket.CreateManager() : bucket.CreateManager(user, CacheConfig.ExtentProperty["Password"]);
                var result = manager.Flush();
            };
            Excute(func);
        }

        /// <summary>
        /// 移除指定键缓存
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>成功返回true</returns>
        public override bool Remove(string strKey)
        {
            return Excute(bucket => bucket.Remove(KeySuffix + strKey).Success);
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <returns>成功返回对象</returns>
        public override T Get<T>(string strKey)
        {
            return Excute(bucket =>
            {
                if(typeof(T) == typeof(string))
                {
                    var result1 = bucket.Get<object>(KeySuffix + strKey);
                    if (result1.Success)
                    {
                        return (T)result1.Value;
                    }
                    return default(T);
                }
                var result = bucket.Get<T>(KeySuffix + strKey);
                if (result.Success)
                {
                    return result.Value;
                }
                return default(T);
            });
        }


        /// <summary>
        /// 增加缓存时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">时间</param>
        /// <returns>返回增加后的时间</returns>
        public override ulong Increment(string key, ulong amount)
        {
            return Excute(bucket => bucket.Increment(KeySuffix + key, DefaultExpireTime, amount).Value);
        }


        /// <summary>
        /// 减少缓存时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">时间</param>
        /// <returns>返回减少后的缓存时间</returns>
        public override ulong Decrement(string key, ulong amount)
        {
            return Excute(bucket => bucket.Decrement(KeySuffix + key, DefaultExpireTime, amount).Value);
        }

        #endregion



        /// <inheritdoc />
        public override void Dispose()
        {

        }

        /// <inheritdoc />
        public override bool AddForSliding(string strKey, object objValue, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}