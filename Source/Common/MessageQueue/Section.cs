using System;
using System.Collections.Generic;
using System.Linq;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.MessageQueue
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
        public MessageQueueConfigHelper()
            : base("MessageQueue")
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
    /// 消息队列，配置说明
    /// </summary>
    public class MessageQueueEntity : ConfigEntityBase
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
        public string HandleType
        {
            get
            {
                return GetValue<string>("HandleType");
            }
            set
            {
                SetValue("HandleType", value);
            }
        }

        /// <summary>
        /// 自动处理器
        /// </summary>
        public string AutoHandleType
        {
            get
            {
                return GetValue<string>("AutoHandleType");
            }
            set
            {
                SetValue("AutoHandleType", value);
            }
        }


        /// <summary>
        /// 队列名称，默认值为：MessageQueue
        /// </summary>
        public string QueueName
        {
            get
            {
                return GetValue<string>("QueueName");
            }
            set
            {
                SetValue("QueueName", value);
            }
        }

        /// <summary>
        /// 休眠时间
        /// </summary>
        public int SleepTime
        {
            get
            {
                return GetValue<int>("SleepTime");
            }
            set
            {
                SetValue("SleepTime", value);
            }
        }

        /// <summary>
        /// 队列路径
        /// </summary>
        public string FormatName
        {
            get
            {
                return GetValue<string>("FormatName");
            }
            set
            {
                SetValue("FormatName", value);
            }
        }

        /// <summary>
        /// 支持序列化的类型
        /// 字符串类型自动加入支持，其它类型要手动加入
        /// </summary>
        public List<string> SupportedTypes
        {
            get
            {
                return GetValue<List<string>>("SupportedTypes");
            }
            set
            {
                SetValue("SupportedTypes", value);
            }
        }

        /// <summary>
        /// 消息序列化器
        /// </summary>
        public string MessageFormatter
        {
            get
            {
                return GetValue<string>("MessageFormatter");
            }
            set
            {
                SetValue("MessageFormatter", value);
            }
        }

        /// <summary>
        /// 事务类型
        /// </summary>
        public string TransactionType
        {
            get
            {
                return GetValue<string>("TransactionType");
            }
            set
            {
                SetValue("TransactionType", value);
            }
        }

        /// <summary>
        /// 读取消息超时时间
        /// 默认时间3秒
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return GetValue<TimeSpan>("Timeout");
            }
            set
            {
                SetValue("Timeout", value);
            }
        }

        /// <summary>
        /// 消息确认
        /// </summary>
        public bool Acknowledge
        {
            get
            {
                return GetValue<bool>("Acknowledge");
            }
            set
            {
                SetValue("Acknowledge", value);
            }
        }
        /// <summary>
        /// 处理错误抛出异常
        /// 默认值:true
        /// </summary>
        public bool ErrorThrowException
        {
            get
            {
                return GetValue<bool>("ErrorThrowException");
            }
            set
            {
                SetValue("ErrorThrowException", value);
            }
        }
    }
}
