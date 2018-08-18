using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace Zhoubin.Infrastructure.Common.MessageQueue.ActiveMQ
{
    
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public class ActiveMQNet : MessageQueueAbstract<IConnection>
    // ReSharper restore InconsistentNaming
    {
        
        IConnectionFactory _factory;
        private IConnection _connection;
        
        protected override void Initiate()
        {
            if (!ConfigEntity.ExtentProperty.ContainsKey("Url"))
            {
                throw new MessageQueueException("配置中Url为空。");
            }
            _factory = new ConnectionFactory(ConfigEntity.ExtentProperty["Url"]);
        }

        public override int Open()
        {
            _connection = CreateMessageQueue();
            return 0;
        }

        public override void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        protected override int Send(string message)
        {
            using (ISession session = _connection.CreateSession())
            {
                //Create the Producer for the topic/queue   
                var prod = session.CreateProducer(CreateDestination(session));
                var msg = prod.CreateTextMessage(message);
                prod.Send(msg, ConfigEntity.ExtentProperty.ContainsKey("Storage") && ConfigEntity.ExtentProperty["Storage"] == "true" ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
            }

            return 0;
        }

        private IDestination CreateDestination(ISession session)
        {
            return Apache.NMS.Util.SessionUtil.GetDestination(session, ConfigEntity.QueueName);
        }
        // ReSharper disable once InconsistentNaming
        protected override void Retrieve(IConnection mq, Func<string, bool> func, bool noACK)
        {
            using (ISession session = mq.CreateSession())
            {
                //Create the Producer for the topic/queue   
                var prod = session.CreateConsumer(CreateDestination(session));
                var msg = prod.Receive() as ITextMessage;
                bool result = func(msg.Text);
                if (!noACK)
                {
                    //todo:如果支持确认逻辑，在此处加入
                }
            }
        }

        protected override IConnection CreateMessageQueue()
        {
            var connection = !ConfigEntity.ExtentProperty.ContainsKey("UserName")
                        ? _factory.CreateConnection()
                        : _factory.CreateConnection(ConfigEntity.ExtentProperty["UserName"], ConfigEntity.ExtentProperty["Password"]);
            connection.Start();
            return connection;
        }

        protected override IConnection DefaultMessageQueue
        {
            get { return _connection; }
        }
    }
}
