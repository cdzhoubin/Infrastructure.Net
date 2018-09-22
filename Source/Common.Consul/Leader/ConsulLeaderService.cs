using Consul;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Zhoubin.Infrastructure.Common.Consul.Leader
{
    public class ConsulLeaderService : LeaderServiceBase
    {
        IConsulClient _client;
        IDistributedLock _lock;
        public ConsulLeaderService(IConsulClient client, string leaderName, string basePath = null) : base(leaderName, basePath)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_lock != null && _lock.IsHeld)
            {
                _lock.Release();
            }
        }

        protected override bool CheckLeader()
        {
            try
            {
                return _lock.IsHeld;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }


        protected override void SelectLeader()
        {
            try
            {
                var task = _lock.Acquire();
                Task.WaitAll(task);
            }
            catch (AggregateException) { }
            catch (LockMaxAttemptsReachedException)
            {
                // Ignore because lock delay might be in effect.
            }
        }
        protected override void Init(string leaderGroupPath)
        {
            _lock = _client.CreateLock(leaderGroupPath);
        }
    }
}
