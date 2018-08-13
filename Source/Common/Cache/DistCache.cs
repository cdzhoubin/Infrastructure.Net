using System;
using System.Collections.Generic;
using System.Linq;

namespace Zhoubin.Infrastructure.Common.Cache
{
    /// <summary>
    /// �ֲ�ʽ����ʵ��
    /// </summary>
    public static class DistCache
    {
        static DistCache()
        {
            LoadProvider();
        }
        private static volatile CacheProvider _objProvider;
        private static CacheProviderCollection _objProviders;
        private static readonly object Lock = new object();

        /// <summary>
        /// �������ƻ�ȡ����ʵ��
        /// </summary>
        /// <param name="providerName">��������</param>
        /// <exception cref="ProviderException">��providerName��Ӧ��&lt;see cref="CacheProvider"/&gt;������ʱ���׳����쳣</exception>
        /// <returns>���ش����ɹ���&lt;see cref="CacheProvider"/&gt;</returns>
        public static CacheProvider GetCacheProvider(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return _objProvider;
            }
            var provider = _objProviders[providerName];
            if (provider == null)
            {
                throw new CacheProviderException("Unable to load " + providerName + " cache provider");
            }
            return provider;

        }

        /// <summary>
        /// �޸�Ĭ��ʹ�õ�Provider
        /// </summary>
        /// <param name="providerName">����</param>
        /// <exception cref="ProviderException">��providerδ�ҵ�ʱ�׳����쳣��</exception>
        public static void ChangeProvider(string providerName)
        {
            lock (Lock)
            {
                _objProvider = _objProviders[providerName];
            }
            if (_objProvider == null)
            {
                throw new CacheProviderException("Unable to load " + providerName + " cache provider");
            }
        }

        /// <summary>
        /// ��Ĭ�������ļ��м��ػ���������Ϣ
        /// </summary>
        private static void LoadProvider()
        {
            // Avoid claiming lock if provider already loaded
            if (_objProvider != null)
            {
                return;
            }

            lock (Lock)
            {
                //make sure _provider is still null
                if (_objProvider != null)
                {
                    return;
                }

                
                //Load registered providers and point _objProvider to the default provider
                _objProviders = new CacheProviderCollection();
                _objProvider = _objProviders.DefaultConfig.ProviderInstance;

                if (_objProvider == null)
                {
                    throw new CacheProviderException("Unable to load default cache provider");
                }
            }
        }

        

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="objValue">����</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ�����true,��������false</returns>
        public static bool Add(string strKey, object objValue, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="objValue">����</param>
        /// <param name="bDefaultTimeSpan">ʹ��Ĭ�Ϲ���ʱ��</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ�����true,��������false</returns>
        public static bool Add(string strKey, object objValue, bool bDefaultTimeSpan, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue, bDefaultTimeSpan);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="objValue">����</param>
        /// <param name="lNumofMilliSeconds">����ʱ��,��λ:����</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ�����true,��������false</returns>
        public static bool Add(string strKey, object objValue, long lNumofMilliSeconds, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue, lNumofMilliSeconds);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="objValue">����</param>
        /// <param name="tspan">����ʱ��</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ�����true,��������false</returns>
        public static bool Add(string strKey, object objValue, TimeSpan tspan, string providerName = null)
        {
            return GetCacheProvider(providerName).Add(strKey, objValue, tspan);
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="objValue">����</param>
        /// <param name="tspan">����ʱ��</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ�����true,��������false</returns>
        public static bool AddForSliding(string strKey, object objValue, TimeSpan tspan, string providerName = null)
        {
            return GetCacheProvider(providerName).AddForSliding(strKey, objValue, tspan);
        }

        /// <summary>
        /// ��ȡָ����ֵ
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ����ػ�ȡ����ֵ</returns>
        public static object Get(string strKey, string providerName = null)
        {
            return GetCacheProvider(providerName).Get(strKey);
        }

        /// <summary>
        /// ��ȡָ������ָ������
        /// </summary>
        /// <param name="strKey">��</param>
        /// <typeparam name="T">ֵ����</typeparam>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ����ض���,ʧ�ܷ���null</returns>
        public static T Get<T>(string strKey, string providerName = null)
        {
            return GetCacheProvider(providerName).Get<T>(strKey);
        }

        /// <summary>
        /// ��ȡһ�黺������
        /// </summary>
        /// <param name="keys">������</param>
        /// <returns>�ɹ������ֵ�����</returns>
        [Obsolete("��ʹ�÷���IDictionary<string, object> Get(IEnumerable<string> keys, string providerName = null)�滻")]
        public static IDictionary<string, object> Get(params string[] keys)
        {
            return _objProvider.Get(keys);
        }
        /// <summary>
        /// ��ȡһ�黺������
        /// </summary>
        /// <param name="keys">������</param>
        /// <param name="providerName">����</param>
        /// <returns>�ɹ������ֵ�����</returns>
        public static IDictionary<string, object> Get(IEnumerable<string> keys, string providerName = null)
        {
            return GetCacheProvider(providerName).Get(keys.ToArray());
        }

        /// <summary>
        /// �Ƴ�ָ����ֵ��������
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="providerName">����</param>
        /// <returns>����ɾ����Ķ���</returns>
        public static object Remove(string strKey, string providerName = null)
        {
            return GetCacheProvider(providerName).Remove(strKey);
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="providerName">����</param>
        public static void RemoveAll(string providerName = null)
        {
            GetCacheProvider(providerName).RemoveAll();
        }

        /// <summary>
        /// ���ӻ����ֵ����ʱ��
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="lAmount">���ӻ���ʱ�䣬��λ������</param>
        /// <param name="providerName">����</param>
        /// <returns>�������Ӻ�Ļ���ʱ��</returns>
        public static ulong Increment(string strKey, ulong lAmount, string providerName = null)
        {
            return GetCacheProvider(providerName).Increment(strKey, lAmount);
        }

        /// <summary>
        /// ���ٻ����ֵ����ʱ��
        /// </summary>
        /// <param name="strKey">��</param>
        /// <param name="lAmount">����ʱ�䣬��λ������</param>
        /// <param name="providerName">����</param>
        /// <returns>���ؼ��ٺ�Ļ���ʱ��</returns>
        public static ulong Decrement(string strKey, ulong lAmount, string providerName = null)
        {
            return GetCacheProvider(providerName).Decrement(strKey, lAmount);
        }

    }
}
