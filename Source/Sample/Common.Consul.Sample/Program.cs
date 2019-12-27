using Consul;
using System;
using System.Threading.Tasks;
using Zhoubin.Infrastructure.Common.Consul.Leader;
using Zhoubin.Infrastructure.Common.CoreConsole;

namespace Common.Consul.Sample
{
    /// <summary>
    /// 主从选举事例
    /// 采用命令行本机启用Consul服务，命令如下：
    /// consul.exe agent --ui --dev
    /// </summary>
    class Program
    {
        public static ConsulLeaderService consulLeaderService;
        static void Main(string[] args)
        {
            using (IConsulClient client = new ConsulClient())
            {
                consulLeaderService = new ConsulLeaderService(client, "Sample");
                consulLeaderService.Start();
                HostTool.Host<SampleSingleService>(args);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
