using System;
using System.Threading;
using Zhoubin.Infrastructure.Log;
using Zhoubin.Infrastructure.MessageQueue.Config;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// 消息队列辅助实现类
    /// </summary>
    public abstract class MessageQueueBase : IMessageQueue, IAutoReveiveMessage
    {
        private static ILogger logger = LogFactory.GetDefaultLogger();
        /// <summary>
        /// 队列配置
        /// </summary>
        protected MessageQueueEntity ConfigEntity { get; private set; }
        /// <summary>
        /// 日志记录器
        /// </summary>
        protected static ILogger Logger
        {
            get
            {
                return logger;
            }

            set
            {
                logger = value;
            }
        }

        //protected MessageArrivalHandler ArrivalHandler;
        //protected bool QueueListenerStarted;
        //Thread _thread;

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">待发送消息</param>
        /// <typeparam name="T">消息内容类型</typeparam>
        /// <returns>返回0发送成功，其它发送失败</returns>
        public abstract int Send<T>(Message<T> message);


        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="func">消息处理函数</param>
        /// <param name="noAck">不应答</param>
        /// <typeparam name="T">消息内容类型</typeparam>
        /// <returns>返回接收到的消息内容</returns>
        // ReSharper disable once InconsistentNaming
        public void Retrieve<T>(Func<Message<T>, bool> func, bool noAck)
        {
            //if (ArrivalHandler != null && ArrivalHandler.GetInvocationList().Length > 0)
            //{
            //    throw new NotAllowHandleMessageOnAutoModel();
            //}

            Retrieve(func, false, noAck);
        }


        /// <summary>
        /// 消息接收
        /// </summary>
        /// <param name="func">消息处理函数</param>
        /// <param name="autoHandle">自动处理</param>
        /// <param name="noAck">不应答</param>
        /// <typeparam name="T">消息内容类型</typeparam>
        // ReSharper disable once InconsistentNaming
        protected abstract void Retrieve<T>(Func<Message<T>, bool> func, bool autoHandle, bool noAck);

        /// <summary>
        /// 初始化消息队列
        /// </summary>
        /// <param name="configEntity">消息配置</param>
        public void Initiate(MessageQueueEntity configEntity)
        {
            ConfigEntity = configEntity;
            Initiate();
        }

        /// <summary>
        ///  初始化
        /// </summary>
        protected abstract void Initiate();

        /// <summary>
        /// 打开消息队列
        /// </summary>
        /// <returns>返回0打开成功，返回其它打开出错</returns>
        public abstract int Open();

        /// <summary>
        /// 关闭消息队列
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Close();
            Stop();
        }

        /// <summary>
        /// 开始自动处理消息
        /// </summary>
        /// <param name="processed">消息处理器</param>
        /// <param name="autoAck">自动应答</param>
        /// <typeparam name="T">消息类型</typeparam>
        public void Start<T>(Func<Message<T>, bool> processed, bool autoAck)
        {
            if (QueueListenerStarted)
            {
                throw new MessageQueueException("侦听线程已经启动，请不要重复启动。");
            }

            Open();
            ThreadStart ts = () =>
            {
                QueueListenerStarted = true;
                try
                {
                    //listen to the queue continusly through loop
                    while (QueueListenerStarted)
                    {
                        Thread.Sleep(ConfigEntity.SleepTime);
                        Retrieve(processed, true, autoAck);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(new Log.LogExceptionEntity(exception));
                }
            };

            _thread = new Thread(ts);
            _thread.Start();
        }

        /// <summary>
        /// 停止消息接收
        /// </summary>
        public void Stop()
        {
            Close();
            QueueListenerStarted = false;
            Thread.Sleep(2000);
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
            }

            _thread = null;
        }

        /// <summary>
        /// 监听是否开始
        /// </summary>
        protected bool QueueListenerStarted;
        Thread _thread;
    }
}