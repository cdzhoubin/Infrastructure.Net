using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zhoubin.Infrastructure.Common.Console.Core
{
    public abstract class HostedServiceBase<T> : IHostedService, IDisposable
        where T : class, IHostedService
    {
        public ILogger Logger { get { return _logger; } }
        private readonly ILogger _logger;
        private int _sleepTime;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="sleepTime">休眠时间，如果为0表示执行单次，大于0表示循环执行的</param>
        protected HostedServiceBase(ILogger<T> logger, int sleepTime)
        {
            _logger = logger;
            _sleepTime = sleepTime < 0 ? 0 : sleepTime;
        }
        private Task _task;
        public void Dispose()
        {
            if (_task != null)
            {
                _task.Dispose();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");
            _task = CreateWorkTask(cancellationToken);
            _task.Start();
            _logger.LogInformation("Started");
            return _task;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping.");
            _isRunning = false;
            _logger.LogInformation("Stopped.");
            return Task.CompletedTask; ;
        }
        protected virtual Task CreateWorkTask(CancellationToken cancellationToken)
        {
            Task task = _sleepTime == 0 ? new Task(DoWork, cancellationToken): new Task(DoWhileWork, cancellationToken);

            return task;
        }

        private void DoWhileWork()
        {
            _isRunning = true;
            while (_isRunning)
            {
                try
                {
                    _logger.LogInformation("任务执行开始...");
                    DoWork();
                    _logger.LogInformation("任务执行完成.");
                }
                catch (ThreadAbortException)
                {
                    _isRunning = false;
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "执行任务出错。");
                }
                try
                {
                    Thread.Sleep((int)_sleepTime);
                }
                catch (ThreadAbortException)
                {
                    _isRunning = false;
                }
            }
        }
        protected abstract void DoWork();


        private bool _isRunning;
    }
}
