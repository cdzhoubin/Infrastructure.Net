using System;
using System.Configuration.Provider;
using Zhoubin.Infrastructure.Common.Properties;

namespace Zhoubin.Infrastructure.Cache
{    

    /// <summary>
    /// ����ʵ��Collection��
    /// </summary>
    public class CacheProviderCollection : ProviderCollection
    {
        /// <summary>
        /// �������ƻ�ȡ����ʵ��
        /// </summary>
        /// <param name="name"></param>
        public new CacheProvider this[string name]
        {
            get
            {
                return (CacheProvider)base[name];
            }
        }

        /// <summary>
        /// ��ӻ���ʵ��
        /// </summary>
        /// <param name="provider">����ʵ��</param>
        /// <exception cref="ArgumentNullException">��providerΪnullʱ���׳����쳣</exception>
        /// <exception cref="ArgumentException">��provider����CacheProvider������ʱ���׳����쳣</exception>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is CacheProvider))
                throw new ArgumentException(Resources.InvalidProviderType, "provider");

            base.Add(provider);
        }
    }    
}
