using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.Cache
{
    public class CacheConfig : ConfigEntityBase
    {
        #region Membership Variables        

        /// <summary>
        /// 构造函数
        /// </summary>
        public CacheConfig()
        {
            KeySuffix = string.Empty;
            DefaultExpireTime = 2;
        }

        #endregion


        /// <summary>
        /// 默认过期时间
        /// 单位毫秒
        /// </summary>
        public ulong DefaultExpireTime
        {
            get
            {
                return GetValue<ulong>("DefaultExpireTime");
            }
            set
            {
                SetValue("DefaultExpireTime", value);
            }
        }

        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeySuffix
        {
            get
            {
                return GetValue<string>("KeySuffix");
            }
            set
            {
                SetValue("KeySuffix", value);
            }
        }



        public string Provider
        {
            get
            {
                return GetValue<string>("Provider");
            }
            set
            {
                SetValue("Provider", value);
            }
        }

        public CacheProvider ProviderInstance
        {
            get {
                if (!IsLock)
                {
                    return null;
                }
                var result = (CacheProvider)Provider.CreateInstance();
                result.Initialize(this);
                return result;
            }
        }

        static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }
    }


}
