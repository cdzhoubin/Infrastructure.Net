using System;

namespace Zhoubin.Infrastructure.Common.MessageQueue
{
    /// <summary>
    /// 自动接收队列消息接口
    /// </summary>
    public interface IAutoReveiveMessage : IDisposable
    {
        /// <summary>
        /// 开始自动处理消息
        /// </summary>
        /// <param name="processed">消息处理器</param>
        /// <param name="autoAck">自动应答</param>
        /// <typeparam name="T">消息类型</typeparam>
        void Start<T>(Func<Message<T>, bool> processed, bool autoAck);

        /// <summary>
        /// 停止消息接收
        /// </summary>
        void Stop();

        /// <summary>
        /// 初始化消息队列
        /// </summary>
        /// <param name="configEntity">消息队列配置</param>
        void Initiate(MessageQueueEntity configEntity);
    }
}