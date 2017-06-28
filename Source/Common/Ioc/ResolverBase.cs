using System;
using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common.Ioc
{
    /// <summary>
    /// Ioc基类实现
    /// </summary>
    public abstract class ResolverBase : IResolver
    {
        static readonly Dictionary<Type,ResolverBase> ResolverDictionary = new Dictionary<Type, ResolverBase>();

        /// <summary>
        /// Ioc创建对象注入接口
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">配置名称</param>
        /// <returns>创建成功类型</returns>
        public abstract T Resolve<T>(string name = null);
        /// <summary>
        /// 创建Ioc实例
        /// 使用此方法创建单例对象
        /// </summary>
        /// <typeparam name="T">Ioc类型</typeparam>
        /// <returns>返回创建成功实例</returns>

        public static IResolver GetResolver<T>() where T : ResolverBase, new()
        {
            Type type = typeof (T);
            if (!ResolverDictionary.ContainsKey(type))
            {
                lock (typeof(ResolverBase))
                {
                    if (!ResolverDictionary.ContainsKey(type))
                    {
                        var resolver = new T();
                        resolver.Init();
                        ResolverDictionary.Add(type,resolver);
                    }
                }
            }
            return ResolverDictionary[type];
        }
        /// <summary>
        /// ioc初始化
        /// </summary>
        protected abstract void Init();
    }
}