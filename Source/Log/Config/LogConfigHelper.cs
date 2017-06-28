using System.Linq;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Log.Config
{
    /// <summary>
    /// 日志配置帮助类
    /// </summary>
    public sealed class LogConfigHelper : ConfigHelper<LogConfigEntity>
    {
        /// <summary>
        /// 日志配置帮助类
        /// </summary>
        /// <param name="configFile">配置文件路径</param>
        public LogConfigHelper(string configFile) : base("LogService", configFile)
        {
        }

        /// <summary>
        /// 默认日志处理器
        /// </summary>
        public LogConfigEntity Default
        {
            get
            {
                return Section.First(p => p.Default);
            }
        }
    }
}
