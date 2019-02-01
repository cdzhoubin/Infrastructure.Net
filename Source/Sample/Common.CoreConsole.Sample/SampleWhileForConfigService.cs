using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zhoubin.Infrastructure.Common.CoreConsole;

namespace Common.CoreConsole.Sample
{
    public class SampleWhileForConfigService : HostedServiceBase<SampleWhileService,SampleTaskSetting>
    {
        public SampleWhileForConfigService(ILogger<SampleWhileService> logger, IOptions<SampleTaskSetting> appConfig) : base(logger, appConfig)
        {
        }
        int i = 1;
        protected override void DoWork(SampleTaskSetting setting)
        {
            Logger.LogInformation(string.Format("{0} DoWork:{1},Url:{2},Time:{3}"
                , i.ToString().PadLeft(5, ' '), DateTime.Now,setting.Url,setting.SleepTime));
            i++;
        }
    }
}
