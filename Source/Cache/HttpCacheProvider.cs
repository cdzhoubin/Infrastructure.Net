using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// 根据HttpRunTime.Cache实现的缓存Provider
    /// </summary>
    public class HttpCacheProvider : CacheProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpCacheProvider()
            : base("Http Cache Provider")
        {
            
        }


        /// <summary>
        /// 名称
        /// </summary>
        public override string Name
        {
            get { return "HttpCacheProvider"; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public override string Description
        {
            get { return "HttpCacheProvider"; }
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="strKey">键</param>
        /// <param name="objValue">对象</param>
        /// <returns>成功返回true,其它返回false</returns>
        public override bool Add(string strKey, object objValue)
        {
            IncrementTotalOperPc();
            IncrementAddOperPc();
            HttpRuntime.Cache.Insert(KeySuffix + strKey, objValue);
            return true;
        }

        

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>返回缓存对象</returns>
        public override object Get(string strKey)
        {
            IncrementTotalOperPc();
            IncrementGetOperPc();
            return HttpRuntime.Cache.Get(KeySuffix + strKey);
        }


        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <returns>成功返回读取到的对象,失败返回默认值</returns>
        public override T Get<T>(string strKey)
        {
            IncrementTotalOperPc();
            IncrementGetOperPc();

            return (T)HttpRuntime.Cache.Get(KeySuffix + strKey);
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="keys">一组Key</param>
        /// <returns>返回一组数据组对象</returns>
        public override IDictionary<string, object> Get(params string[] keys)
        {
            IncrementTotalOperPc();
            IncrementGetOperPc();

            IDictionary<string, object> ret = new Dictionary<string, object>();
            foreach (var key in keys.ToList())
            {
                ret.Add(key, HttpRuntime.Cache.Get(KeySuffix + key));
            }

            return ret;
        }


        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public override void RemoveAll()
        {
            IncrementTotalOperPc();
            var itemsInCache = HttpRuntime.Cache.GetEnumerator();
            while (itemsInCache.MoveNext())
            {

                HttpRuntime.Cache.Remove(itemsInCache.Key.ToString());
            }
        }


        /// <summary>
        /// 移除指定键缓存
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns>成功返回true,其它返回false</returns>
        public override bool Remove(string strKey)
        {
            IncrementTotalOperPc();
            HttpRuntime.Cache.Remove(KeySuffix + strKey);
            return true;
        }


        /// <summary>
        /// 增加缓存时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">时间</param>
        /// <returns>返回增加后的时间</returns>
        public override ulong Increment(string key, ulong amount)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 减少缓存时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">时间</param>
        /// <returns>返回减少后的缓存时间</returns>
        public override ulong Decrement(string key, ulong amount)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
        }
        
        /// <summary>
        /// 新增滑动缓存
        /// </summary>
        /// <param name="strKey">key</param>
        /// <param name="objValue">缓存值</param>
        /// <param name="timeSpan">滑动时间</param>
        /// <returns>成功返回true，其他返回false</returns>
        public override bool AddForSliding(string strKey, object objValue, TimeSpan timeSpan)
        {
            IncrementTotalOperPc();
            IncrementAddOperPc();

            HttpRuntime.Cache.Insert(KeySuffix + strKey, objValue, null, System.Web.Caching.Cache.NoAbsoluteExpiration, timeSpan);
            return true;
        }
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <param name="objValue">Value</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns>成功返回true,其它返回false</returns>
        public override bool Add(string strKey, object objValue, TimeSpan timeSpan)
        {
            IncrementTotalOperPc();
            IncrementAddOperPc();

            HttpRuntime.Cache.Insert(KeySuffix + strKey, objValue, null, DateTime.Now.AddTicks(timeSpan.Ticks), System.Web.Caching.Cache.NoSlidingExpiration);
            return true;
        }
    }
}
