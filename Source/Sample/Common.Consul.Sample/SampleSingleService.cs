using Microsoft.Extensions.Logging;
using System;
using Zhoubin.Infrastructure.Common.CoreConsole;

namespace Common.Consul.Sample
{
    public class SampleSingleService : HostedServiceBase<SampleSingleService>
    {
        public SampleSingleService(ILogger<SampleSingleService> logger) : base(logger, 1000)
        {
        }

        protected override void DoWork()
        {
           if(Program.consulLeaderService.IsLeader)
            {
                Logger.LogInformation("Run Leader DoWork:" + DateTime.Now.ToString());

            } else
            {
                Logger.LogInformation("DoWork:" + DateTime.Now.ToString());
            }
            
        }
    }
}
