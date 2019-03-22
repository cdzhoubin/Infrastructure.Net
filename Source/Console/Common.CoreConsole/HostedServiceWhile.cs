using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace Zhoubin.Infrastructure.Common.CoreConsole
{
    /// <summary>
    /// 服务实现基类
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <typeparam name="TConfig">服务配置数据</typeparam>
    public abstract class HostedServiceBase<T,TConfig> : HostedServiceBase<T> where T:class, IHostedService
        where TConfig:TaskSetting,new()
    {
        readonly TConfig setting;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appConfig"></param>
        protected HostedServiceBase(ILogger<T> logger, IOptions<TConfig> appConfig) : base(logger,appConfig.Value.SleepTime)
        {
            setting = appConfig.Value;
        }
        protected override void DoWork()
        {
            DoWork(setting);
        }
        protected abstract void DoWork(TConfig config);

        protected override void Init()
        {
            Init(setting);
        }
        protected virtual void Init(TConfig config)
        {
            
        }
    }

    public class TaskSetting
    {
        public int SleepTime { get; set; }
    }
}
