using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;
using Zhoubin.Infrastructure.Cache;

namespace Zhoubin.Infrastructure.Web.Cache
{
    /// <summary>
    /// 分布式缓存Session提供者
    /// </summary>
    public class DistCacheSessionStateProvider : SessionStateStoreProviderBase
    {
        private static CacheProvider _cacheProvider;
        /// <summary>
        /// 释放由 <see cref="T:System.Web.SessionState.SessionStateStoreProviderBase"/> 实现使用的所有资源。
        /// </summary>
        public override void Dispose()
        {

        }

        /// <summary>
        /// 设置对 Global.asax 文件中定义的 Session_OnEnd 事件的 <see cref="T:System.Web.SessionState.SessionStateItemExpireCallback"/> 委托的引用。
        /// </summary>
        /// <returns>
        /// 如果会话状态存储提供程序支持调用 Session_OnEnd 事件，则为 true；否则为 false。
        /// </returns>
        /// <param name="expireCallback">对 Global.asax 文件中定义的 Session_OnEnd 事件的 <see cref="T:System.Web.SessionState.SessionStateItemExpireCallback"/> 委托。</param>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        /// <summary>
        /// 由 <see cref="T:System.Web.SessionState.SessionStateModule"/> 对象调用，以便进行每次请求初始化。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param>
        public override void InitializeRequest(HttpContext context)
        {

        }

        /// <summary>
        /// 初始化提供程序。
        /// </summary>
        /// <param name="name">该提供程序的友好名称。</param><param name="config">名称/值对的集合，表示在配置中为该提供程序指定的、提供程序特定的特性。</param><exception cref="T:System.ArgumentNullException">提供程序的名称是 null。</exception><exception cref="T:System.ArgumentException">提供程序的名称长度为零。</exception><exception cref="T:System.InvalidOperationException">提供程序初始化完成后，将尝试调用该提供程序上的 <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/>。</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            _cacheProvider = DistCache.GetCacheProvider(config.Get("CacheProviderName"));
            base.Initialize(name, config);
        }

        /// <summary>
        /// 从会话数据存储区中返回只读会话状态数据。
        /// </summary>
        /// <returns>
        /// 使用会话数据存储区中的会话值和信息填充的 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>。
        /// </returns>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的 <see cref="P:System.Web.SessionState.HttpSessionState.SessionID"/>。</param>
        /// <param name="locked">当此方法返回时，如果请求的会话项在会话数据存储区被锁定，请包含一个设置为 true 的布尔值；否则请包含一个设置为 false 的布尔值。</param>
        /// <param name="lockAge">当此方法返回时，请包含一个设置为会话数据存储区中的项锁定时间的 <see cref="T:System.TimeSpan"/> 对象。</param><param name="lockId">当此方法返回时，请包含一个设置为当前请求的锁定标识符的对象。有关锁定标识符的详细信息，请参见 <see cref="T:System.Web.SessionState.SessionStateStoreProviderBase"/> 类摘要中的“锁定会话存储区数据”。</param><param name="actions">当此方法返回时，请包含 <see cref="T:System.Web.SessionState.SessionStateActions"/> 值之一，指示当前会话是否为未初始化的无 Cookie 会话。</param>
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId,
            out SessionStateActions actions)
        {
            var e = Get(false, id, out locked, out lockAge, out lockId, out actions);

            return e?.ToStoreData(context);
        }
        static SessionStateItem Get(bool acquireLock, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            locked = false;
            lockId = null;
            lockAge = TimeSpan.Zero;
            actions = SessionStateActions.None;

            var e = _cacheProvider.Get<SessionStateItem>(id);
            if (e == null)
                return null;

            if (acquireLock)
            {
                // repeat until we can update the retrieved 
                // item (i.e. nobody changes it between the 
                // time we get it from the store and updates it s attributes)
                // Save() will return false if Cas() fails
                while (true)
                {
                    if (e.LockId > 0)
                        break;

                    actions = e.Flag;

                    e.LockId = 0;
                    e.LockTime = DateTime.UtcNow;
                    e.Flag = SessionStateActions.None;

                    // try to update the item in the store
                    if (_cacheProvider.Add(id,e,TimeSpan.FromMinutes(e.Timeout)))
                    {
                        locked = true;
                        lockId = e.LockId;

                        return e;
                    }

                    // it has been modified between we loaded and tried to save it
                    e = _cacheProvider.Get<SessionStateItem>(id);
                    if (e == null)
                        return null;
                }
            }

            locked = true;
            lockAge = DateTime.UtcNow - e.LockTime;
            lockId = e.LockId;
            actions = SessionStateActions.None;

            return acquireLock ? null : e;
        }
        /// <summary>
        /// 从会话数据存储区中返回只读会话状态数据。
        /// </summary>
        /// <returns>
        /// 使用会话数据存储区中的会话值和信息填充的 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>。
        /// </returns>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param><param name="id">当前请求的 <see cref="P:System.Web.SessionState.HttpSessionState.SessionID"/>。</param><param name="locked">当此方法返回时，如果成功获得锁定，请包含一个设置为 true 的布尔值；否则请包含一个设置为 false 的布尔值。</param><param name="lockAge">当此方法返回时，请包含一个设置为会话数据存储区中的项锁定时间的 <see cref="T:System.TimeSpan"/> 对象。</param><param name="lockId">当此方法返回时，请包含一个设置为当前请求的锁定标识符的对象。有关锁定标识符的详细信息，请参见 <see cref="T:System.Web.SessionState.SessionStateStoreProviderBase"/> 类摘要中的“锁定会话存储区数据”。</param><param name="actions">当此方法返回时，请包含 <see cref="T:System.Web.SessionState.SessionStateActions"/> 值之一，指示当前会话是否为未初始化的无 Cookie 会话。</param>
        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge,
            out object lockId, out SessionStateActions actions)
        {
            var e = Get(true, id, out locked, out lockAge, out lockId, out actions);

            return e?.ToStoreData(context);
        }

        /// <summary>
        /// 释放对会话数据存储区中项的锁定。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的会话标识符。</param>
        /// <param name="lockId">当前请求的锁定标识符。</param>
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            var tmp = (ulong)lockId;
            SessionStateItem e;
            do
            {
                // Load the header for the item with CAS
                e = _cacheProvider.Get<SessionStateItem>(id);

                // Bail if the entry does not exist, or the lock ID does not match our lock ID
                if (e == null || e.LockId != tmp)
                {
                    break;
                }

                // Attempt to clear the lock for this item and loop around until we succeed
                e.LockId = 0;
                e.LockTime = DateTime.MinValue;
            } while (!_cacheProvider.Add(id, e, TimeSpan.FromMinutes(e.Timeout)));
        }

        /// <summary>
        /// 使用当前请求中的值更新会话状态数据存储区中的会话项信息，并清除对数据的锁定。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param><param name="id">当前请求的会话标识符。</param><param name="item">包含要存储的当前会话值的 <see cref="T:System.Web.SessionState.SessionStateStoreData"/> 对象。</param><param name="lockId">当前请求的锁定标识符。</param><param name="newItem">如果为 true，则将会话项标识为新项；如果为 false，则将会话项标识为现有的项。</param>
        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            SessionStateItem e;
            do
            {
                if (!newItem)
                {
                    var tmp = (ulong)lockId;

                    // Load the entire item with CAS (need the DataCas value also for the save)
                    e = _cacheProvider.Get<SessionStateItem>(id);

                    // if we're expecting an existing item, but
                    // it's not in the cache
                    // or it's locked by someone else, then quit
                    if (e == null || e.LockId != tmp)
                    {
                        return;
                    }
                }
                else
                {
                    // Create a new item if it requested
                    e = new SessionStateItem();
                }

                // Set the new data and reset the locks
                e.Timeout = item.Timeout;
                e.Data = (SessionStateItemCollection)item.Items;
                e.Flag = SessionStateActions.None;
                e.LockId = 0;
                e.LockTime = DateTime.MinValue;

                // Attempt to save with CAS and loop around if it fails
            } while (!_cacheProvider.Add(id, e, TimeSpan.FromMinutes(e.Timeout)));
        }

        /// <summary>
        /// 删除会话数据存储区中的项数据。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param><param name="id">当前请求的会话标识符。</param><param name="lockId">当前请求的锁定标识符。</param><param name="item">表示将从数据存储区中删除的项的 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>。</param>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            var tmp = (ulong)lockId;
            var e = _cacheProvider.Get<SessionStateItem>(id);

            if (e != null && e.LockId == tmp)
            {
                _cacheProvider.Remove(id);
            }
        }

        /// <summary>
        /// 更新会话数据存储区中的项的到期日期和时间。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param><param name="id">当前请求的会话标识符。</param>
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            SessionStateItem e;
            do
            {
                // Load the item with CAS
                e = _cacheProvider.Get<SessionStateItem>(id);
                if (e == null)
                {
                    break;
                }

                // Try to save with CAS, and loop around until we succeed
            } while (!_cacheProvider.Add(id, e, TimeSpan.FromMinutes(e.Timeout)));
        }

        /// <summary>
        /// 创建要用于当前请求的新 <see cref="T:System.Web.SessionState.SessionStateStoreData"/> 对象。
        /// </summary>
        /// <returns>
        /// 当前请求的新 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>。
        /// </returns>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param><param name="timeout">新 <see cref="T:System.Web.SessionState.SessionStateStoreData"/> 的会话状态 <see cref="P:System.Web.SessionState.HttpSessionState.Timeout"/> 值。</param>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        /// <summary>
        /// 将新的会话状态项添加到数据存储区中。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的 <see cref="P:System.Web.SessionState.HttpSessionState.SessionID"/>。</param>
        /// <param name="timeout">当前请求的会话 <see cref="P:System.Web.SessionState.HttpSessionState.Timeout"/>。</param>
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            var e = new SessionStateItem
            {
                Data = new SessionStateItemCollection(),
                Flag = SessionStateActions.InitializeItem,
                LockId = 0,
                Timeout = timeout
            };


            _cacheProvider.Add(id, e, TimeSpan.FromMinutes(timeout));
        }

        /// <summary>
        /// 在请求结束时由 <see cref="T:System.Web.SessionState.SessionStateModule"/> 对象调用。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="T:System.Web.HttpContext"/>。</param>
        public override void EndRequest(HttpContext context)
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SessionStateItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public SessionStateStoreData ToStoreData(HttpContext context)
        {
            return new SessionStateStoreData(Data, SessionStateUtility.GetSessionStaticObjects(context), Timeout);
        }

        private SessionStateItemCollection _data;
        private SessionStateActions _flag;
        private ulong _lockId;
        private DateTime _lockTime;

        // this is in minutes
        private int _timeout;

        /// <summary>
        /// 
        /// </summary>
        public SessionStateItemCollection Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionStateActions Flag
        {
            get
            {
                return _flag;
            }

            set
            {
                _flag = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ulong LockId
        {
            get
            {
                return _lockId;
            }

            set
            {
                _lockId = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LockTime
        {
            get
            {
                return _lockTime;
            }

            set
            {
                _lockTime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }

            set
            {
                _timeout = value;
            }
        }
    }



}
