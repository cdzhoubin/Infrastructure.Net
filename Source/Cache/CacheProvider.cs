using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics;

namespace Zhoubin.Infrastructure.Cache
{
    /// <summary>
    /// CacheProvider基类
    /// 所有实现都从此基类继承
    /// </summary>
    public abstract class CacheProvider : ProviderBase,IDisposable
    {
        #region Membership Variables

        /// <summary>
        /// 缓存性能计数器分类名称
        /// </summary>
        protected string CacheCatName { get; private set; }
        private const string StrTotalOpName = "# operations executed";
        private const string StrOpPerSecName = "# operations / sec";
        private const string StrAddOpName = "# of add operations executed";
        private const string StrGetOpName = "# of get operations executed";
        private const string StrAddOpPerSecName = "# of add operations / sec";
        private const string StrGetOpPerSecName = "# of get operations / sec";

        private PerformanceCounter _objAddOperations;
        private PerformanceCounter _objAddPerSecondOperations;
        private PerformanceCounter _objGetOperations;
        private PerformanceCounter _objGetPerSecondOperations;
        private PerformanceCounter _objOperationsPerSecond;
        private PerformanceCounter _objTotalOperations;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheCatName">缓存分类分类名</param>
        protected CacheProvider(string cacheCatName)
        {
            CacheCatName = cacheCatName;
            KeySuffix = string.Empty;
            DefaultExpireTime = 2000;
        }

        #endregion


        /// <summary>
        /// 是否启用性能计数器
        /// </summary>
        public bool EnablePerformance { get; private set; }

        /// <summary>
        /// 默认过期时间
        /// 单位毫秒
        /// </summary>
        public ulong DefaultExpireTime { get; set; }

        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeySuffix { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <param name="config">配置数据</param>
        /// <exception cref="ArgumentNullException">当参数config为null时抛出此异常</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from Web.config.
            if (null == config)
            {
                throw (new ArgumentNullException("config"));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "HttpCacheProvider.CacheProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Http Cache Provider");
            }
            // Call the base class implementation.
            base.Initialize(name, config);

            // Load configuration data.            
            DefaultExpireTime =
                Convert.ToUInt64(GetConfigValue(config["defaultExpireTime"], "2000"));

            KeySuffix =
                GetConfigValue(config["keySuffix"], string.Empty);

            EnablePerformance = Boolean.Parse(GetConfigValue(config["enablePerformance"], "false"));


        }

        static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }

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


        #region Performance Counter Methods

        /// <summary>
        /// 性能计数器
        /// </summary>
        public virtual void CheckPerformanceCounterCategories()
        {
            if (!PerformanceCounterCategory.Exists(CacheCatName))
            {
                var counters = new CounterCreationDataCollection();

                var totalOps = new CounterCreationData
                {
                    CounterName = StrTotalOpName,
                    CounterHelp = "Total number of operations executed",
                    CounterType = PerformanceCounterType.NumberOfItems32
                };
                counters.Add(totalOps);

                var opsPerSecond = new CounterCreationData
                {
                    CounterName = StrOpPerSecName,
                    CounterHelp = "Number of operations executed per second",
                    CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                };
                counters.Add(opsPerSecond);

                var addOps = new CounterCreationData
                {
                    CounterName = StrAddOpName,
                    CounterHelp = "Number of add operations execution",
                    CounterType = PerformanceCounterType.NumberOfItems32
                };
                counters.Add(addOps);

                var addOpsPerSec = new CounterCreationData
                {
                    CounterName = StrAddOpPerSecName,
                    CounterHelp = "Number of add operations per second",
                    CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                };
                counters.Add(addOpsPerSec);

                var getOps = new CounterCreationData
                {
                    CounterName = StrGetOpName,
                    CounterHelp = "Number of get operations execution",
                    CounterType = PerformanceCounterType.NumberOfItems32
                };
                counters.Add(getOps);

                var getOpsPerSec = new CounterCreationData
                {
                    CounterName = StrGetOpPerSecName,
                    CounterHelp = "Number of get operations per second",
                    CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                };
                counters.Add(getOpsPerSec);

                // create new category with the counters above
                PerformanceCounterCategory.Create(CacheCatName,
                                                  "Memcached Cache Provider Performance Counter",
                                                  PerformanceCounterCategoryType.SingleInstance, counters);
            }

            // create counters to work with
            _objTotalOperations = new PerformanceCounter
            {
                CategoryName = CacheCatName,
                CounterName = StrTotalOpName,
                MachineName = ".",
                ReadOnly = false,
                RawValue = 0
            };

            _objOperationsPerSecond = new PerformanceCounter
            {
                CategoryName = CacheCatName,
                CounterName = StrOpPerSecName,
                MachineName = ".",
                ReadOnly = false,
                RawValue = 0
            };

            _objAddOperations = new PerformanceCounter
            {
                CategoryName = CacheCatName,
                CounterName = StrAddOpName,
                MachineName = ".",
                ReadOnly = false,
                RawValue = 0
            };

            _objAddPerSecondOperations = new PerformanceCounter
            {
                CategoryName = CacheCatName,
                CounterName = StrAddOpPerSecName,
                MachineName = ".",
                ReadOnly = false,
                RawValue = 0
            };

            _objGetOperations = new PerformanceCounter
            {
                CategoryName = CacheCatName,
                CounterName = StrGetOpName,
                MachineName = ".",
                ReadOnly = false,
                RawValue = 0
            };

            _objGetPerSecondOperations = new PerformanceCounter
            {
                CategoryName = CacheCatName,
                CounterName = StrGetOpPerSecName,
                MachineName = ".",
                ReadOnly = false,
                RawValue = 0
            };
        }

        /// <summary>
        /// 总操作数增加
        /// </summary>
        protected void IncrementTotalOperPc()
        {
            if (_objTotalOperations != null)
                _objTotalOperations.Increment();
            if (_objOperationsPerSecond != null)
                _objOperationsPerSecond.Increment();
        }

        /// <summary>
        /// 新增操作数据增加
        /// </summary>
        protected void IncrementAddOperPc()
        {
            if (_objAddOperations != null)
                _objAddOperations.Increment();
            if (_objAddPerSecondOperations != null)
                _objAddPerSecondOperations.Increment();
        }

        /// <summary>
        /// 读取操作数增加
        /// </summary>
        protected void IncrementGetOperPc()
        {
            if (_objGetOperations != null)
                _objGetOperations.Increment();
            if (_objGetPerSecondOperations != null)
                _objGetPerSecondOperations.Increment();
        }

        #endregion

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
