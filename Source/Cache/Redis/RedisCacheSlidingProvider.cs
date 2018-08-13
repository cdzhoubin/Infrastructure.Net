using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Zhoubin.Infrastructure.Common.Cache.Redis
{
    /// <summary>
    /// RedisCache滑动实现
    /// </summary>
    public class RedisCacheSlidingProvider : SlidingCacheWaperProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RedisCacheSlidingProvider()
            : base("Redis Cache Sliding Provider")
        {

        }

        #region Membership Variables
        private IRedisClient _client;

        #endregion

        #region ProviderBase Methods

        


        #endregion

        #region Cache Provider


        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <returns>成功返回true,其它返回false</returns>
        public override bool Add(string strKey, object objValue)
        {
            return _client.Set(KeySuffix + strKey, CreateValueWraper(objValue));
        }


        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>返回缓存对象</returns>
        public override object Get(string strKey)
        {
            var result = _client.Get<SlidingCacheWraper>(KeySuffix + strKey);

            return GetValue<object>(result, KeySuffix + strKey);
        }


        /// <inheritdoc />
        public override IDictionary<string, object> Get(params string[] keys)
        {
            IList<string> keysList = keys.Select(str => KeySuffix + str).ToList();

            var ret = _client.GetByIds<object>(keysList.ToArray());
            IDictionary<string, object> retVal = new Dictionary<string, object>();

            for (var i = 0; i < keysList.Count; i++)
            {
                string str = keysList[i];
                retVal.Add(str.Remove(0, KeySuffix.Length), GetValue<object>((SlidingCacheWraper)ret[i], str));
            }

            return retVal;
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public override void RemoveAll()
        {
            _client.RemoveAll(_client.GetAllKeys());
        }


        /// <inheritdoc />
        public override bool Remove(string strKey)
        {
            return _client.Remove(KeySuffix + strKey);
        }


        /// <inheritdoc />
        public override T Get<T>(string strKey)
        {
            return GetValue<T>(_client.Get<SlidingCacheWraper>(KeySuffix + strKey), KeySuffix + strKey);
        }


        /// <inheritdoc />
        public override ulong Increment(string key, ulong amount)
        {
            unchecked
            {
                return (ulong)_client.Increment(KeySuffix + key, (uint)amount);
            }

        }


        /// <inheritdoc />
        public override ulong Decrement(string key, ulong amount)
        {
            return (ulong)_client.Decrement(KeySuffix + key, (uint)amount);
        }

        #endregion

        ///// <summary>
        ///// 
        ///// </summary>
        //public MemcachedCacheProvider()
        //{
        //    _objTotalOperations = new PerformanceCounter(StrCacheCatName, StrTotalOpName);
        //    _objOperationsPerSecond = new PerformanceCounter(StrCacheCatName, StrOpPerSecName);
        //    _objAddOperations = new PerformanceCounter(StrCacheCatName, StrAddOpName);
        //    _objGetOperations = new PerformanceCounter(StrCacheCatName, StrGetOpName);
        //    _objAddPerSecondOperations = new PerformanceCounter(StrCacheCatName, StrAddOpName);
        //    _objGetPerSecondOperations = new PerformanceCounter(StrCacheCatName, StrGetOpPerSecName);
        //}


        /// <inheritdoc />
        public override void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }

        /// <inheritdoc />
        public override bool AddForSliding(string strKey, object objValue, TimeSpan timeSpan)
        {
            return _client.Set(KeySuffix + strKey, new SlidingCacheWraper(objValue, timeSpan), timeSpan);
        }
        /// <summary>
        /// 使用包装器实现滑动
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="result">解析结果</param>
        /// <param name="strKey">键值</param>
        /// <returns></returns>
        private T GetValue<T>(SlidingCacheWraper result, string strKey)
        {
            return GetValue<T>(result, strKey, (p, v, t) =>
            {
                _client.Set(p, v, t);
            });
        }


        /// <inheritdoc />
        public override bool Add(string strKey, object objValue, TimeSpan time)
        {
            return _client.Set(KeySuffix + strKey, CreateValueWraper(objValue), time);
        }
        static List<Type> TypeList = new List<Type>();
        protected override void Init(CacheConfig config)
        {
            _client = new RedisClient(config.ExtentProperty["Servers"]);
            var str = config.ExtentProperty["SerializeTypes"];
            if (!string.IsNullOrEmpty(str))
            {
                foreach (string item in str.Split(';'))
                {
                    if (string.IsNullOrEmpty(item.Trim()))
                    {
                        continue;
                    }
                    TypeList.Add(Type.GetType(item));
                }
            }
            if (TypeList.Count > 0)
            {
                JsConfig.AllowRuntimeType = type => TypeList.Contains(type);
            }
        }
    }

}
