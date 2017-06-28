using System;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// 滑动缓存实现基类
    /// </summary>
    public abstract class SlidingCacheWaperProvider : CacheProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheCatName">缓存分类分类名</param>
        protected SlidingCacheWaperProvider(string cacheCatName) : base(cacheCatName)
        {
            KeySuffix = string.Empty;
            DefaultExpireTime = 2000;
        }
        /// <summary>
        /// 创建存储包装类
        /// </summary>
        /// <param name="value">待包装对象</param>
        /// <param name="slidingExpiration">没有滑动时间表示绝对时间</param>
        /// <returns>返回成功创建的<see cref="SlidingCacheWraper"/></returns>
        protected SlidingCacheWraper CreateValueWraper(object value, TimeSpan slidingExpiration)
        {
            return new SlidingCacheWraper(value, slidingExpiration);
        }
        /// <summary>
        /// 创建存储包装类
        /// </summary>
        /// <param name="value">待包装对象</param>
        /// <returns>返回成功创建的&lt;see cref="SlidingCacheWraper"/&gt;</returns>
        protected SlidingCacheWraper CreateValueWraper(object value)
        {
            return CreateValueWraper(value, TimeSpan.Zero);
        }
        /// <summary>
        /// 使用包装器实现滑动
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="result">解析结果</param>
        /// <param name="strKey">键值</param>
        /// <param name="action">重新存储算法实现</param>
        /// <returns>返回定的类型，当前result为空时，返回默认默认值：default(T)</returns>
        protected T GetValue<T>(SlidingCacheWraper result, string strKey, Action<string, object, TimeSpan> action)
        {
            if (result == null)
            {
                return default(T);
            }
            if (result.GetSlidingExpiration() == TimeSpan.Zero)
            {
                return result.GetCacheContent<T>();
            }
            action(strKey, result, result.GetSlidingExpiration());
            return result.GetCacheContent<T>();
        }
    }
}
