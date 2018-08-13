using System;

namespace Zhoubin.Infrastructure.Common.MessageQueue
{
    /// <summary>
    /// 消息接收处理器
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">接收到的消息</param>
    /// <typeparam name="T">事件数据</typeparam>
    public delegate void MessageArrivalHandler<T>(object sender,MessageArrivalEventArgs<T> args);

    /// <summary>
    /// 消息到达通知事件参数
    /// </summary>
    /// <typeparam name="T">事件数据</typeparam>
    public class MessageArrivalEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="content">消息内容</param>
        public MessageArrivalEventArgs(Message<T> content)
        {
            Content = content;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public Message<T> Content { get; private set; }
    }
}