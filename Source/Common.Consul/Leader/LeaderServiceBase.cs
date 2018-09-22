using System;
using System.Diagnostics;
using System.Threading;

namespace Zhoubin.Infrastructure.Common.Consul.Leader
{
    /// <summary>
    /// Leader选举基类
    /// </summary>
    public abstract class LeaderServiceBase : ILeaderService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="leaderGroupName">分组名称</param>
        /// <param name="basePath">选择基路径，默认为null</param>
        protected LeaderServiceBase(string leaderGroupName, string basePath = null)
        {
            if (string.IsNullOrWhiteSpace(leaderGroupName))
            {
                throw new ArgumentNullException(nameof(leaderGroupName));
            }
            _leaderGroupName = leaderGroupName;
            _basePath = basePath;
        }

        readonly string _leaderGroupName;
        readonly string _basePath;
        private Thread _thread;
        private bool _running;
        private bool _isDisposed = false;
        /// <summary>
        /// 服务组路径模板
        /// </summary>
        protected virtual string ServicePathTemplate { get { return "services/{0}/leader"; } }

        /// <summary>
        /// Leader标识，当选举为leader时，返回true,其他返回false
        /// </summary>
        public bool IsLeader
        {
            get
            {
                if (!_running)
                {
                    throw new Exception("请启动选举后，再检查。");
                }
                return CheckLeader();
            }
        }
        /// <summary>
        /// 检查leader状态
        /// </summary>
        /// <returns>当选举为leader时，返回true,其他返回false</returns>
        protected abstract bool CheckLeader();
        /// <summary>
        /// 启动选举
        /// </summary>
        public void Start()
        {
            _running = true;
            _thread = new Thread(new ThreadStart(Work));
            _thread.Start();
        }
        /// <summary>
        /// 初始化选举
        /// </summary>
        /// <param name="leaderGroupPath">选举组路径</param>
        protected virtual void Init(string leaderGroupPath)
        {

        }
        /// <summary>
        /// leader选举
        /// </summary>
        protected abstract void SelectLeader();
        private void Work()
        {
            var leaderGroupPath = string.Format(ServicePathTemplate, _leaderGroupName);
            if (!string.IsNullOrEmpty(_basePath))
            {
                leaderGroupPath = string.Format("{0}/{1}", _basePath, leaderGroupPath);
            }
            Init(leaderGroupPath);
            while (_running)
            {
                try
                {
                    SelectLeader();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            _running = false;
            int count = 5;
            while (_thread.IsAlive && count > 0)
            {
                Thread.Sleep(1000);
                count--;
            }
            _thread.Abort();
        }
    }
}
