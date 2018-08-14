using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace Zhoubin.Infrastructure.Common.Console.Core
{
    public abstract class HostedServiceBase<T,TConfig> : HostedServiceBase<T> where T:class, IHostedService
        where TConfig:TaskSetting,new()
    {
        TConfig setting;
        public HostedServiceBase(ILogger<T> logger, IOptions<TConfig> appConfig) : base(logger,appConfig.Value.SleepTime)
        {
            setting = appConfig.Value;
        }
        protected override void DoWork()
        {
            DoWork(setting);
        }
        protected abstract void DoWork(TConfig config);
    }

    public class TaskSetting
    {
        public int SleepTime { get; set; }
    }
}
