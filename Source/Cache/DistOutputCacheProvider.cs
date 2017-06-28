using System;
using System.Web.Caching;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// asp.net输出缓存
    /// memcachep实现
    /// </summary>
    public class DistOutputCacheProvider : OutputCacheProvider
    {
        //public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        //{                   
        //    base.Initialize(name, config);
        //}
        
        /// <summary>
        /// 新增缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entry">对象</param>
        /// <param name="utcExpiry">utc过期时间</param>
        /// <returns>返回加入的缓存对象</returns>
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            return DistCache.Add(key, entry, GetTimeSpan(utcExpiry)) ? entry : null;
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回获取对象，获取失败返回null</returns>
        public override object Get(string key)
        {
            return DistCache.Get(key);
        }

        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="key">键</param>
        public override void Remove(string key)
        {
            DistCache.Remove(key);
        }

        /// <summary>
        /// 更新缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entry">对象</param>
        /// <param name="utcExpiry">utc过期时间</param>
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            DistCache.Add(key, entry, GetTimeSpan(utcExpiry));
        }

        private TimeSpan GetTimeSpan(DateTime utcExpiry)
        {
            if (utcExpiry == DateTime.MaxValue)//当设置过期时间为日期最大值时，自动设置为10年
            {
                utcExpiry = DateTime.UtcNow.AddYears(10);
            }
            return new TimeSpan((utcExpiry - DateTime.UtcNow).Ticks);
        }
    }
}
