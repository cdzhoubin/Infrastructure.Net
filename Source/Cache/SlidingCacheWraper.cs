using System;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// 缓存包装类
    /// </summary>
    [Serializable]
    public class SlidingCacheWraper
    {
        /// <summary>
        /// 当前缓存对象，转移为指定类型
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>返回转换成功的对象，如果当前对象为null,就返回默认值</returns>

        public T GetCacheContent<T>()
        {
            if (CacheContent == null)
            {
                return default(T);
            }
            return (T)CacheContent;
        }
        /// <summary>
        /// 缓存
        /// </summary>

        public object CacheContent { get; set; }
        /// <summary>
        /// 滑动时间
        /// </summary>
        public long SlidingExpiration { get; set; }
        /// <summary>
        /// 获取时间片
        /// </summary>
        /// <returns>返回滑动时间值，如果小于等于0，就返回0</returns>

        public TimeSpan GetSlidingExpiration()
        {
            if (SlidingExpiration <= 0)
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromTicks(SlidingExpiration);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cache">缓存对象</param>
        /// <param name="slidingExpiration">滑动时间</param>
        public SlidingCacheWraper(object cache, TimeSpan slidingExpiration)
        {
            CacheContent = cache;
            SlidingExpiration = slidingExpiration.Ticks;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SlidingCacheWraper() { }
    }
}
