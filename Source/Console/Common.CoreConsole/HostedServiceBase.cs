using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zhoubin.Infrastructure.Common.CoreConsole
{
    /// <summary>
    /// 服务配置基类
    /// 实现基本的独立执行和循环执行
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    public abstract class HostedServiceBase<T> : IHostedService, IDisposable
        where T : class, IHostedService
    {
        public ILogger Logger { get; }

        public bool IsRunning { get; set; }

        private readonly int _sleepTime;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="sleepTime">休眠时间，如果为0表示执行单次，大于0表示循环执行的</param>
        protected HostedServiceBase(ILogger<T> logger, int sleepTime)
        {
            Logger = logger;
            _sleepTime = sleepTime < 0 ? 0 : sleepTime;
        }
        private Task _task;
        private CancellationToken _cancellationToken;
        public void Dispose()
        {
            if (_task != null)
            {
                _task.Dispose();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting");
            _task = CreateWorkTask(cancellationToken);
            _task.Start();
            Logger.LogInformation("Started");
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping.");
            IsRunning = false;
            Logger.LogInformation("Stopped.");
            return Task.CompletedTask;
        }
        protected virtual Task CreateWorkTask(CancellationToken cancellationToken)
        {
            Task task = _sleepTime == 0 ? new Task(DoWork, cancellationToken) : new Task(DoWhileWork, cancellationToken);
            _cancellationToken = cancellationToken;
            return task;
        }

        private void DoWhileWork()
        {
            IsRunning = true;
            while (IsRunning && !_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Logger.LogInformation("任务执行开始...");
                    DoWork();
                    Logger.LogInformation("任务执行完成.");
                }
                catch (ThreadAbortException)
                {
                    IsRunning = false;
                    continue;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "执行任务出错。");
                }
                Task.Delay(TimeSpan.FromMilliseconds(_sleepTime), _cancellationToken);
            }
        }
        protected abstract void DoWork();
    }
}
