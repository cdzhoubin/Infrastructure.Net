using System.Linq;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.MessageQueue.Config
{
    /// <summary>
    /// 日志配置帮助类
    /// </summary>
    public sealed class MessageQueueConfigHelper : ConfigHelper<MessageQueueEntity>
    {
        /// <summary>
        /// 日志配置帮助类
        /// </summary>
        /// <param name="configFile">配置文件</param>
        public MessageQueueConfigHelper(string configFile)
            : base("MessageQueue", configFile)
        {
        }

        /// <summary>
        /// 默认日志处理器
        /// </summary>
        public MessageQueueEntity Default
        {
            get
            {
                return Section.First(p => p.Default);
            }
        }
    }
}
