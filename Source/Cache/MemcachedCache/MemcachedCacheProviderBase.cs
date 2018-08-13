

using System;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.Extensions.Configuration;

namespace Zhoubin.Infrastructure.Common.Cache.MemcachedCache
{
    internal class MemcachedCacheProviderHelper
    {
        protected static ClientConfiguration ClientConfiguration { get; private set; }
        static MemcachedCacheProviderHelper()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");

            var jsonConfiguration = builder.Build();
            var definition = new CouchbaseClientDefinition();
            jsonConfiguration.GetSection("Couchbase").Bind(definition);
            ClientConfiguration = new ClientConfiguration(definition);
        }
        internal static T Excute<T>(Func<IBucket, T> func, string bucketName)
        {
            using (Cluster _cluster = new Cluster(ClientConfiguration))
            using (var bucket = _cluster.OpenBucket(bucketName))
            {
                return func(bucket);
            }
        }
        internal static void Excute(Action<IBucket> action, string bucketName)
        {
            using (Cluster _cluster = new Cluster(ClientConfiguration))
            using (var bucket = _cluster.OpenBucket(bucketName))
            {
                action(bucket);
            }
        }
    }
    public abstract class MemcachedCacheProviderBase : CacheProvider
    {
        protected MemcachedCacheProviderBase(string providerName) : base(providerName)
        {
        }



        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">配置数据</param>
        /// <exception cref="ArgumentNullException">当参数config为null时抛出此异常</exception>
        protected override void Init(CacheConfig config)
        {
            BucketName = config.ExtentProperty["BucketName"] ?? "default";
        }

        protected string BucketName { get; private set; }
        
        protected T Excute<T>(Func<IBucket, T> func)
        {
                return MemcachedCacheProviderHelper.Excute<T>(func, BucketName);
        }
        protected void Excute(Action<IBucket> action)
        {
            MemcachedCacheProviderHelper.Excute(action, BucketName);
        }
    }
}