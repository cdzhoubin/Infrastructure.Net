using System;
using Microsoft.Extensions.Logging;
using Zhoubin.Infrastructure.Common.CoreConsole;

namespace Common.CoreConsole.Sample
{
    public class SampleSingleService : HostedServiceBase<SampleSingleService>
    {
        public SampleSingleService(ILogger<SampleSingleService> logger) : base(logger, 0)
        {
        }

        protected override void DoWork()
        {
            Logger.LogInformation("DoWork:" + DateTime.Now.ToString());
        }
    }
}
