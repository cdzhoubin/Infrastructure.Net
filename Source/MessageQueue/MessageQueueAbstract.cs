using System;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// 消息队列辅助泛型实现类
    /// </summary>
    /// <typeparam name="TQueue">队列类型</typeparam>
    public abstract class MessageQueueAbstract<TQueue> : MessageQueueBase where TQueue : IDisposable
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">待发送消息</param>
        /// <typeparam name="T">消息内容类型</typeparam>
        /// <returns>返回0发送成功，其它发送失败</returns>
        public override int Send<T>(Message<T> message)
        {
            if (message == null)
            {
                return 0;
            }

            return Send(message.SerializeObject());
        }

        

        /// <summary>
        /// 发送消息信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>成功返回0，其它表示出错</returns>
        protected abstract int Send(string message);


        /// <summary>
        /// 消息接收
        /// </summary>
        /// <param name="func">消息处理函数</param>
        /// <param name="autoHandle">自动处理</param>
        /// <param name="noAck">不应答</param>
        /// <typeparam name="T">消息内容类型</typeparam>
        // ReSharper disable once InconsistentNaming
        protected override void Retrieve<T>(Func<Message<T>, bool> func, bool autoHandle,bool noAck)
        {
            try
            {

                if (autoHandle)
                {// ReSharper disable once CompareNonConstrainedGenericWithNull
                    if (DefaultMessageQueue == null)
                    {
                        throw new MessageQueueNotOpenException();
                    }

                    Retrieve(DefaultMessageQueue, p=>func(p.DeserializeObject<Message<T>>()), noAck);
                }
                else
                {
                    using (var mq = CreateMessageQueue())
                    {
                        Retrieve(mq, p => func(p.DeserializeObject<Message<T>>()), noAck);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Write(new Log.LogExceptionEntity(exception));
                if (ConfigEntity.ErrorThrowException)
                {
                    throw new MessageQueueException(exception);
                }
            }
        }


        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="mq">消息队列</param>
        /// <param name="func">消息处理函数回调</param>
        /// <param name="noAck">不应答</param>
// ReSharper disable once InconsistentNaming
        protected abstract void Retrieve(TQueue mq, Func<string, bool> func,bool noAck);

        /// <summary>
        /// 创建消息队列
        /// </summary>
        /// <returns>返回创建好的消息队列对象</returns>
        protected abstract TQueue CreateMessageQueue();
        /// <summary>
        /// 默认消息队列
        /// </summary>
        protected abstract TQueue DefaultMessageQueue { get; }

    }
}