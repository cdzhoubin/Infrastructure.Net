using System;
using System.Collections.Generic;
using System.Linq;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// 日志配置帮助类
    /// </summary>
    public sealed class MessageQueueConfigHelper : ConfigHelper<MessageQueueEntity>
    {
        /// <summary>
        /// 日志配置帮助类
        /// </summary>
        /// <param name="configFile">配置文件</param>
        public MessageQueueConfigHelper(string configFile)
            : base("MessageQueue", configFile)
        {
        }

        /// <summary>
        /// 默认日志处理器
        /// </summary>
        public MessageQueueEntity Default
        {
            get
            {
                return Section.First(p => p.Default);
            }
        }
    }

    /// <summary>
    /// 消息队列配置区
    /// </summary>
    public class MessageQueueSection : ConfigurationSectionHandlerHelper<MessageQueueEntity>
    {
    }

    /// <summary>
    /// 消息队列，配置说明
    /// </summary>
    public class MessageQueueEntity : ConfigEntityBase<MessageQueueEntity>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageQueueEntity()
        {
            QueueName = "MessageQueue";
            SleepTime = 2000;
            SupportedTypes = new List<string> { typeof(string).FullName };
            ExtentProperty = new Dictionary<string, string>();
            TransactionType = "None";
            Timeout = TimeSpan.FromSeconds(3);
            ErrorThrowException = true;
        }
        /// <summary>
        /// 处理器
        /// </summary>
        public string HandleType { get; set; }

        /// <summary>
        /// 自动处理器
        /// </summary>
        public string AutoHandleType { get; set; }

        /// <summary>
        /// 默认处理配置
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// 队列名称，默认值为：MessageQueue
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 休眠时间
        /// </summary>
        public int SleepTime { get; set; }

        /// <summary>
        /// 队列路径
        /// </summary>
        public string FormatName { get; set; }

        /// <summary>
        /// 支持序列化的类型
        /// 字符串类型自动加入支持，其它类型要手动加入
        /// </summary>
        public List<string> SupportedTypes { get; set; }

        /// <summary>
        /// 消息序列化器
        /// </summary>
        public string MessageFormatter { get; set; }


        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, string> ExtentProperty { get; set; }

        /// <summary>
        /// 事务类型
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// 读取消息超时时间
        /// 默认时间3秒
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// 消息确认
        /// </summary>
        public bool Acknowledge { get; set; }
        /// <summary>
        /// 处理错误抛出异常
        /// 默认值:true
        /// </summary>
        public bool ErrorThrowException { get; set; }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="node">结点</param>
        protected override void SetProperty(MessageQueueEntity entity, System.Xml.XmlNode node)
        {
            switch (node.Name)
            {
                case "ErrorThrowException":
                    entity.ErrorThrowException = bool.TrueString == node.InnerText;
                    break;
                case "QueueName":
                    entity.QueueName = node.InnerText;
                    break;
                case "AutoHandleType":
                    entity.AutoHandleType = node.InnerText;
                    break;
                case "HandleType":
                    entity.HandleType = node.InnerText;
                    break;
                case "Default":
                    entity.Default = node.InnerText == "true";
                    break;
                case "SupportedTypes":
                    foreach (System.Xml.XmlNode childNode in node.ChildNodes)
                    {
                        entity.SupportedTypes.Add(childNode.InnerText);
                    }
                    break;
                case "MessageFormatter":
                    entity.MessageFormatter = node.InnerText;
                    break;
                case "FormatName":
                    entity.FormatName = node.InnerText;
                    break;
                case "SleepTime":
                    entity.SleepTime = int.Parse(node.InnerText);
                    break;
                case "TransactionType":
                    entity.TransactionType = node.InnerText;
                    break;
                case "Timeout":
                    entity.Timeout = TimeSpan.FromSeconds(double.Parse(node.InnerText));
                    break;
                case "Acknowledge":
                    entity.Acknowledge = node.InnerText.ToLower() == "true";
                    break;
                default:
                    entity.ExtentProperty.Add(node.Name, node.ChildNodes.Count > 0 ? node.InnerXml : node.InnerText);
                    break;
            }
        }
    }
}
