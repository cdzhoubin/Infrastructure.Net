using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Zhoubin.Infrastructure.Common.MessageQueue.RabbitMQ
{
   
    // ReSharper disable once InconsistentNaming
    public class RabbitMQNet : MessageQueueAbstract<IConnection>
    {
        static RabbitMQNet()
        {
            Cf = new ConnectionFactory();
        }


        static readonly ConnectionFactory Cf;
        private IConnection _connection;

        // ReSharper disable once InconsistentNaming
        protected override void Retrieve(IConnection mq, Func<string, bool> func, bool noAck)
        {
            using (var ch = DefaultMessageQueue.CreateModel())
            {
                var message = ch.BasicGet(ConfigEntity.QueueName, noAck);
                if (message == null)
                {
                    return;
                }

                var result = func(Encoding.UTF8.GetString(message.Body));
                if (!noAck)
                {
                    if (result)
                    {
                        ch.BasicAck(message.DeliveryTag, false);
                    }
                }
            }
        }

        protected override IConnection CreateMessageQueue()
        {
            return Cf.CreateConnection();
        }

        protected override IConnection DefaultMessageQueue
        {
            get { return _connection; }
        }

        protected override int Send(string message)
        {
            using (var conn = CreateMessageQueue())
            {
                using (var ch = conn.CreateModel())
                {
                    string finalName = ch.QueueDeclare(ConfigEntity.QueueName, ConfigEntity.ExtentProperty["Durable"] == "true",
                                                       ConfigEntity.ExtentProperty["Exclusive"] == "true", false, new Dictionary<string, object>());

                    var property = new BasicProperties();

                    ch.TxSelect();
                    try
                    {
                        ch.BasicPublish("", finalName, property, Encoding.UTF8.GetBytes(message));
                    }
                    catch (Exception exception)
                    {
                        ch.TxRollback();
                        throw new MessageQueueException("发送消息异常，详细内容请查看内联异常信息", exception);
                    }

                    ch.TxCommit();
                    return 0;
                }
            }
        }

        protected override void Initiate()
        {
            Cf.Uri = new Uri(ConfigEntity.ExtentProperty["Url"]);
        }

        public override int Open()
        {
            _connection = Cf.CreateConnection();
            return 0;
        }

        public override void Close()
        {
            if (_connection != null)
                _connection.Close();
        }
    }
}
