using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zhoubin.Infrastructure.Common.CoreConsole
{
    /// <summary>
    /// 控制台启动应用工具
    /// </summary>
    public static class HostTool
    {
        /// <summary>
        /// 使用方法
        ///public static async Task Main(string[] args)
        ///{
        ///    await HostTool.Host<T>(args);
        ///}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Host<T>(string[] args) where T : class, IHostedService
        {
            var builder = CreateHostBuilder<T, TaskSetting>(args, null);
            await builder.RunConsoleAsync();
        }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Host<T,TSetting>(string[] args,string settingName) 
            where T : class, IHostedService
            where TSetting:class
        {
            var builder = CreateHostBuilder<T,TSetting>(args,settingName);

            await builder.RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder<T, TSetting>(string[] args, string settingName)
            where T : class, IHostedService
            where TSetting : class
        {
          return  new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }

                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var configFile = "appsettings.json";
                    if (!string.IsNullOrEmpty(environmentName))
                    {
                        configFile = string.Format("appsettings.{0}.json", environmentName);
                    }
                    config.AddJsonFile(configFile, optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    if (!string.IsNullOrEmpty(settingName))
                    {
                        services.Configure<TSetting>(hostContext.Configuration.GetSection(settingName));
                    }
                    services.AddSingleton<IHostedService, T>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });
        }
    }
}
