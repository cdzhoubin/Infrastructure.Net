using RdKafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zhoubin.Infrastructure.Common.Extent;
using Zhoubin.Infrastructure.Common.Log;

namespace Zhoubin.Infrastructure.Common.MessageQueue.Kafka
{
    public class KafkaQueue : MessageQueueAbstract<BrokerRouter>
    {
        protected override BrokerRouter DefaultMessageQueue
        {
            get
            {
                return CreateMessageQueue();
            }
        }

        public override int Open()
        {
            return 0;
        }

        BrokerRouter _router;
        public override void Close()
        {
            _router.Dispose();
        }

        protected override BrokerRouter CreateMessageQueue()
        {
            if(_router == null)
            {
                _router = new BrokerRouter { BrokerServer = _serverlist };
            }

            return _router;
        }

        private string _serverlist;
        protected override void Initiate()
        {
            string urls = ConfigEntity.ExtentProperty["Url"];
            if (string.IsNullOrEmpty(urls))
            {
                throw new MessageQueueException("初始化消息队列异常，服务器地址url为空,正确为：10.10.9.1:8081,10.10.9.3:9091");
            }
            _serverlist = urls;
        }
        /// <summary>
        /// 客户端分组
        /// 如果不配置，默认值为：Consumer
        /// </summary>
        protected string Consumer
        {
            get
            {
                if (ConfigEntity.ExtentProperty.ContainsKey(ConsumerName))
                    return ConfigEntity.ExtentProperty[ConsumerName];
                return ConsumerName;
            }
        }
       
        private const string ConsumerName = "Consumer";
        protected override void Retrieve(BrokerRouter mq, Func<string, bool> func, bool noAck)
        {
            var config = new RdKafka.Config() { GroupId = Consumer };
            using (var consumer = new EventConsumer(config, CreateMessageQueue().BrokerServer))
            {

                //注册一个事件
                consumer.OnMessage += (obj, msg) =>
                {
                    try
                    {
                        //msg.TopicPartitionOffset
                        func(msg.Payload.ToUtf8String());
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(new LogExceptionEntity(ex));
                        if (ConfigEntity.ErrorThrowException)
                        {
                            throw new MessageQueueException(ex);
                        }
                    }
                    
                    //Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {text}");
                };

                //订阅一个或者多个Topic
                consumer.Subscribe(ConfigEntity.ExtentProperty[TopicNameKey].Split(',').ToList());

                //启动
                consumer.Start();

                //Console.WriteLine("Started consumer, press enter to stop consuming");
                //Console.ReadLine();
            }
        }

        private const string TopicNameKey = "TopicName";
        protected override int Send(string message)
        {
            using (var client = new Producer(CreateMessageQueue().BrokerServer))
            {
                using (Topic topic = client.Topic(ConfigEntity.ExtentProperty[TopicNameKey]))
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    var proecess = topic.Produce(data);
                    

                   // Console.WriteLine($"发送到分区：{proecess.Result.Partition}, Offset 为: {proecess.Result.Offset}");

                    return proecess.IsFaulted ? 1 : 0;
                }
            }
        }
    }

    public class BrokerRouter : IDisposable
    {
        public string BrokerServer { get; set; }
        public void Dispose()
        {
        }
    }
}
