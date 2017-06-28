using System;

namespace Zhoubin.Infrastructure.Web.Controls
{
    /// <summary>
    /// CachePanel
    /// 事件参数
    /// </summary>
    public class CacheEventArgs : EventArgs
    {
        private readonly CachePanel _panel;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="panel">缓存Panel</param>
        public CacheEventArgs(CachePanel panel)
        {
            if (panel == null)
            {
                throw new ArgumentNullException("panel");
            }

            _panel = panel;
        }


        /// <summary>
        /// 缓存时间
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _panel.Duration;
            }
            set
            {
                _panel.Duration = value;
            }
        }


        /// <summary>
        /// 缓存Key
        /// </summary>
        public string CacheKey
        {
            get
            {
                return _panel.CacheKey;
            }
            set
            {
                _panel.CacheKey = value;
            }
        }
    }
}