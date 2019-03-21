using System;
using System.Threading.Tasks;
using Zhoubin.Infrastructure.Common.CoreConsole;

namespace Common.CoreConsole.Sample
{
    class Program
    {
        public static void Main(string[] args)
        {
            //循环执行任务并读取配置信息示例
          HostTool.Host<SampleWhileForConfigService, SampleTaskSetting>(args, "TaskSetting");
            
            //只执行一次示例
            //HostTool.Host<SampleSingleService>(args);
            //只执行一次示例
            //HostTool.Host<SampleWhileService>(args);
            //Console.ReadKey();
        }
    }
}
