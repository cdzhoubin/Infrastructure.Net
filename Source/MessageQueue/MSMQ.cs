using System;
using System.Messaging;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// Msmq消息队列实现
    /// </summary>
    public class Msmq : MessageQueueAbstract<System.Messaging.MessageQueue>
    {
        private System.Messaging.MessageQueue _mq;

        private MessageQueueTransactionType _transactionType;


        /// <summary>
        /// 发送消息信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>成功返回0，其它表示出错</returns>
        protected override int Send(string message)
        {
            
            var m = new System.Messaging.Message
            {
                Label = Guid.NewGuid().ToString(),
                Body = message,
                Recoverable = ConfigEntity.ExtentProperty["Storage"] == "true",
            };

            _mq.Send(m, _transactionType);
            return 0;
        }


// ReSharper disable once InconsistentNaming
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="mq">消息队列</param>
        /// <param name="func">消息处理函数回调</param>
        /// <param name="noAck">不应答</param>
// ReSharper disable once InconsistentNaming
        protected override void Retrieve(System.Messaging.MessageQueue mq, Func<string, bool> func, bool noAck)
        {
            var message1 = mq.Receive(ConfigEntity.Timeout, _transactionType);
            if (message1 == null)
            {
                return;
            }

            var m = message1.Body.ToString();
           
            if (func(m))
            {
                //todo:处理成功确认
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Initiate()
        {

            if (!System.Messaging.MessageQueue.Exists(ConfigEntity.FormatName))
            {
                System.Messaging.MessageQueue.Create(ConfigEntity.FormatName);
            }

            _transactionType = (MessageQueueTransactionType)Enum.Parse(typeof(MessageQueueTransactionType), ConfigEntity.TransactionType);
        }

        /// <summary>
        /// 打开消息队列
        /// </summary>
        /// <returns>返回0打开成功，返回其它打开出错</returns>
        public override int Open()
        {
            if (_mq != null)
            {
                return 1;//队列已经打开
            }

            try
            {
                _mq = CreateMessageQueue();
            }
            catch (Exception exception)
            {
                throw new MessageQueueException("初始化消息队列出错。", exception);
            }

            return 0;
        }

        /// <summary>
        /// 创建消息队列
        /// </summary>
        /// <returns>返回创建好的消息队列对象</returns>
        protected override System.Messaging.MessageQueue CreateMessageQueue()
        {
            var mq = new System.Messaging.MessageQueue(ConfigEntity.FormatName);
            if (string.IsNullOrEmpty(ConfigEntity.MessageFormatter))
            {
                ((XmlMessageFormatter)mq.Formatter).TargetTypeNames = new string[ConfigEntity.SupportedTypes.Count];
                ConfigEntity.SupportedTypes.CopyTo(((XmlMessageFormatter)mq.Formatter).TargetTypeNames, 0);
            }
            else
            {
                mq.Formatter = ConfigEntity.MessageFormatter.CreateInstance<IMessageFormatter>();
            }

            return mq;
        }

        /// <summary>
        /// 关闭消息队列
        /// </summary>
        public override void Close()
        {
            if (_mq != null)
            {
                _mq.Close();
                _mq.Dispose();
            }
        }

        /// <summary>
        /// 默认消息队列
        /// </summary>
        protected override System.Messaging.MessageQueue DefaultMessageQueue
        {
            get { return _mq; }
        }
    }
}
