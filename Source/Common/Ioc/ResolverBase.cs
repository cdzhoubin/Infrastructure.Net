using System;
using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common.Ioc
{
    /// <summary>
    /// Ioc����ʵ��
    /// </summary>
    public abstract class ResolverBase : IResolver
    {
        static readonly Dictionary<Type,ResolverBase> ResolverDictionary = new Dictionary<Type, ResolverBase>();

        /// <summary>
        /// Ioc��������ע��ӿ�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="name">��������</param>
        /// <returns>�����ɹ�����</returns>
        public abstract T Resolve<T>(string name = null);
        /// <summary>
        /// ����Iocʵ��
        /// ʹ�ô˷���������������
        /// </summary>
        /// <typeparam name="T">Ioc����</typeparam>
        /// <returns>���ش����ɹ�ʵ��</returns>

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
        /// ioc��ʼ��
        /// </summary>
        protected abstract void Init();
    }
}