using System;
using Microsoft.Extensions.Logging;
using Zhoubin.Infrastructure.Common.CoreConsole;

namespace Common.CoreConsole.Sample
{
    public class SampleWhileService : HostedServiceBase<SampleWhileService>
    {
        public SampleWhileService(ILogger<SampleWhileService> logger) : base(logger, 2000)
        {
        }
        int i = 1;
        protected override void DoWork()
        {
            Logger.LogInformation(string.Format("{0} DoWork:{1}",i.ToString().PadLeft(5,' '), DateTime.Now));
            i++;
        }
    }
}
