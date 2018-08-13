using System.Configuration;
using Zhoubin.Infrastructure.Common.MessageQueue;

namespace Zhoubin.Infrastructure.Common.Log.Queue
{
    /// <summary>
    /// 消息队列日志记录器
    /// </summary>
    public sealed class QueueMessageHandler<T> : LoggerBase<T> where T : LogEntityBase
    {
        private readonly IMessageQueue _messagQueueLogger
            = MessageQueueFactory.GetMessageQueue(string.IsNullOrEmpty(Config.ConfigHelper.GetAppSettings("LogMessageQueue")) ? "LogMessageQueue" : Config.ConfigHelper.GetAppSettings("LogMessageQueue"));

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="info"></param>
        protected override void Write(T info)
        {
            _messagQueueLogger.Send(new Message<T>(info.Title, info, (int)info.Sevenrity));
        }

        /// <summary>
        /// 写入字符串日志
        /// </summary>
        /// <param name="info"></param>
        protected override void Write(string info)
        {
            _messagQueueLogger.Send(new Message<string>("", info, 0));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ShutDown()
        {
        }
    }
}
