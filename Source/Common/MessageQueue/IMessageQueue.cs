using System;

namespace Zhoubin.Infrastructure.Common.MessageQueue
{
    /// <summary>
    /// 消息队列接口
    /// </summary>
    public interface IMessageQueue : IDisposable
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">待发送消息</param>
        /// <typeparam name="T">消息数据类型</typeparam>
        /// <returns>返回0发送成功，其它发送失败</returns>
        int Send<T>(Message<T> message);
        
        /// <summary>
        /// 接收消息
        /// 返回单条消息，在需要应答方式下使用
        /// </summary>
        /// <param name="func">接收消息回调</param>
        /// <param name="noAck">不应答</param>
        /// <typeparam name="T">消息数据类型</typeparam>
        /// <returns>返回接收到的消息内容</returns>
        void Retrieve<T>(Func<Message<T>, bool> func, bool noAck);

        /// <summary>
        /// 初始化消息队列
        /// </summary>
        /// <param name="configEntity">队列配置信息</param>
        void Initiate(MessageQueueEntity configEntity);
        /// <summary>
        /// 打开消息队列
        /// </summary>
        /// <returns>返回0打开成功，返回其它打开出错</returns>
        int Open();
        /// <summary>
        /// 关闭消息队列
        /// </summary>
        void Close();
    }
}
