using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Zhoubin.Infrastructure.Common.Config
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
        public LogConfigHelper() : base("LogService")
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

    /// <summary>
    /// 日志配置实体
    /// </summary>
    public sealed class LogConfigEntity:ConfigEntityBase
    {
        /// <summary>
        /// 日志处理器
        /// </summary>
        public string HandleType
        {
            get
            {
                return GetValue<string>("HandleType");
            }
            set
            {
                SetValue("HandleType", value);
            }
        }
    }
}
