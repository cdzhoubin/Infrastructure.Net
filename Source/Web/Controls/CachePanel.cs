using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.UI;
using Control = System.Web.UI.Control;

namespace Zhoubin.Infrastructure.Web.Controls
{
    /// <summary>
    /// 缓存Panel
    /// 可以缓存panel中的所有内容
    /// </summary>
    [DefaultProperty("Duration")]
    [ToolboxData("<{0}:CachePanel runat=server></{0}:CachePanel>")]
    public class CachePanel : Control
    {
        /// <summary>
        /// 缓存时间
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                if (value ==TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value","缓存设置必须大于0");
                }

                _duration = value;
            }
        }


        /// <summary>
        /// 缓存Key
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// PostBack是否加载数据
        /// </summary>
        public bool PostBackLoadData { get; set; }

        private EventHandler<CacheEventArgs> _resolveCache;
        /// <summary>
        /// 缓存设置
        /// </summary>
        public event EventHandler<CacheEventArgs> ResolveCache
        {
            add
            {
                _resolveCache += value;
            }
            remove
            {
                // ReSharper disable once DelegateSubtraction
                _resolveCache -= value;
            }

        }

        private EventHandler _loadData;
        /// <summary>
        /// 数据加载事件
        /// </summary>
        public event EventHandler LoadDataEvent
        {
            add
            {
                _loadData += value;
            }
            remove
            {
                // ReSharper disable once DelegateSubtraction
                _loadData -= value;
            }

        }

        /// <summary>
        /// 是否命中缓存
        /// </summary>
        public bool CacheHit { get; private set; }

        private string _mCacheKey;
        private string _mCachedContent;
        private TimeSpan _duration;

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.Load"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack || PostBackLoadData)
            {
                var loadData = _loadData;
                if (loadData != null)
                {
                    loadData(this, EventArgs.Empty);
                }
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.Init"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnInit(EventArgs e)
        {
            var resolveCacheKey = _resolveCache;
            if (resolveCacheKey != null)
            {
                resolveCacheKey(this, new CacheEventArgs(this));
            }

            CacheHit = LoadCache();
            base.OnInit(e);
        }

        private bool LoadCache()
        {
            _mCacheKey = GetCacheKey();
            _mCachedContent = Context.Cache.Get(_mCacheKey) as string;
            CacheHit = _mCachedContent != null;
            if (CacheHit)
            {
                Controls.Clear();
            }

            return CacheHit;
        }
        /// <summary>
        /// 获取缓存Key
        /// </summary>
        /// <returns></returns>
        string GetCacheKey()
        {
            var cacheKeyBase = CacheKey ?? GetDefaultCacheKeyBase();
            return "$CachePanel$" + cacheKeyBase;
        }

        private string GetDefaultCacheKeyBase()
        {
            return Context.Request.AppRelativeCurrentExecutionFilePath + "_" + UniqueID;
        }

        /// <summary>
        /// 将服务器控件子级的内容输出到提供的 <see cref="T:System.Web.UI.HtmlTextWriter"/> 对象，此对象编写将在客户端呈现的内容。
        /// </summary>
        /// <param name="writer">接收呈现内容的 <see cref="T:System.Web.UI.HtmlTextWriter"/> 对象。</param>
        protected override void RenderChildren(HtmlTextWriter writer)
        {
            if (_mCachedContent == null)
            {
                lock (typeof(CachePanel))
                {
                    if (!LoadCache())
                    {
                        var sb = new StringBuilder();
                        var innerWriter = new HtmlTextWriter(new StringWriter(sb));
                        base.RenderChildren(innerWriter);

                        _mCachedContent = sb.ToString();
                        Context.Cache.Insert(_mCacheKey, _mCachedContent, null,
                            DateTime.Now.Add(Duration), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                }
            }

            writer.Write(_mCachedContent);
        }
    }
}
