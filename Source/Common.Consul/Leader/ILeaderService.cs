using System;

namespace Zhoubin.Infrastructure.Common.Consul.Leader
{
    /// <summary>
    /// Leader选举接口
    /// </summary>
    public interface ILeaderService : IDisposable
    {
        /// <summary>
        /// Leader标识，当选举为leader时，返回true,其他返回false
        /// </summary>
        bool IsLeader { get; }
        /// <summary>
        /// 启动选举
        /// </summary>
        void Start();
    }
}
