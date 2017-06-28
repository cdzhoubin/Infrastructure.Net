using System;
using Zhoubin.Infrastructure.Common;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// 消息队列异常
    /// </summary>
    public class MessageQueueException : InfrastructureException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageQueueException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常内容</param>
        public MessageQueueException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常内容</param>
        /// <param name="exception">包装异常</param>
        public MessageQueueException(string message, Exception exception)
            : base(message, exception)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exception">包装异常</param>
        public MessageQueueException(Exception exception)
            : base("消息队列出现错误，详情请查看内联异常信息。", exception)
        {

        }
    }

    /// <summary>
    /// 队列打开没有打开异常
    /// </summary>
    public class MessageQueueNotOpenException : MessageQueueException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageQueueNotOpenException() : base("消息队列没有打开.") { }
    }

    /// <summary>
    /// 自动消息接收模式下，不允许手动接收消息。
    /// </summary>
    public class NotAllowHandleMessageOnAutoModel : MessageQueueException
    {
        /// <summary>
        /// 自动消息接收模式下，不允许手动接收消息。
        /// </summary>
        public NotAllowHandleMessageOnAutoModel() : base("自动消息接收模式下，不允许手动接收消息。") { }
    }
}
