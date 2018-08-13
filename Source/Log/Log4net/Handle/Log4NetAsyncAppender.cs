using System;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace Zhoubin.Infrastructure.Common.Log.Log4Net.Handle
{
    /// <summary>
    /// Appender that forwards LoggingEvents asynchronously
    /// </summary>
    /// <remarks>
    /// This appender forwards LoggingEvents to a list of attached appenders.
    /// The events are forwarded asynchronously using the ThreadPool.
    /// This allows the calling thread to be released quickly, however it does
    /// not guarantee the ordering of events delivered to the attached appenders.
    /// </remarks>
    public sealed class AsyncAppender : IBulkAppender, IOptionHandler, IAppenderAttachable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 激活选项
        /// </summary>
        public void ActivateOptions()
        {
        }



        /// <summary>
        /// 修正标志
        /// </summary>
        public FixFlags Fix
        {
            get { return _fixFlags; }
            set { _fixFlags = value; }
        }



        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            // Remove all the attached appenders
            lock (this)
            {

                if (_mAppenderAttachedImpl != null)
                {

                    _mAppenderAttachedImpl.RemoveAllAppenders();

                }

            }

        }



        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="loggingEvent"></param>
        public void DoAppend(LoggingEvent loggingEvent)
        {
            loggingEvent.Fix = _fixFlags;
            ThreadPool.QueueUserWorkItem(AsyncAppend, loggingEvent);
        }



        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="loggingEvents"></param>
        public void DoAppend(LoggingEvent[] loggingEvents)
        {
            foreach (var loggingEvent in loggingEvents)
            {
                loggingEvent.Fix = _fixFlags;
            }

            ThreadPool.QueueUserWorkItem(AsyncAppend, loggingEvents);
        }


        /// <summary>
        /// 异步写入日志方法
        /// </summary>
        /// <param name="state"></param>
        private void AsyncAppend(object state)
        {
            if (_mAppenderAttachedImpl != null)
            {
                var loggingEvent = state as LoggingEvent;
                if (loggingEvent != null)
                {
                    _mAppenderAttachedImpl.AppendLoopOnAppenders(loggingEvent);
                }
                else
                {
                    var loggingEvents = state as LoggingEvent[];

                    if (loggingEvents != null)
                    {
                        _mAppenderAttachedImpl.AppendLoopOnAppenders(loggingEvents);
                    }
                }

            }

        }



        #region IAppenderAttachable Members



        /// <summary>
        /// 增加Appender
        /// </summary>
        /// <param name="newAppender"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddAppender(IAppender newAppender)
        {

            if (newAppender == null)
            {
                throw new ArgumentNullException("newAppender");
            }

            lock (this)
            {
                if (_mAppenderAttachedImpl == null)
                {
                    _mAppenderAttachedImpl = new AppenderAttachedImpl();
                }

                _mAppenderAttachedImpl.AddAppender(newAppender);
            }

        }



        /// <summary>
        /// 日志记录器集合
        /// </summary>
        public AppenderCollection Appenders
        {
            get
            {
                lock (this)
                {
                    if (_mAppenderAttachedImpl == null)
                    {
                        return AppenderCollection.EmptyCollection;
                    }

                    return _mAppenderAttachedImpl.Appenders;
                }
            }

        }



        /// <summary>
        /// 根据名称获取日志记录器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IAppender GetAppender(string name)
        {

            lock (this)
            {
                if (_mAppenderAttachedImpl == null || name == null)
                {
                    return null;
                }

                return _mAppenderAttachedImpl.GetAppender(name);
            }

        }



        /// <summary>
        /// 删除所有日志记录器
        /// </summary>
        public void RemoveAllAppenders()
        {
            lock (this)
            {
                if (_mAppenderAttachedImpl != null)
                {
                    _mAppenderAttachedImpl.RemoveAllAppenders();
                    _mAppenderAttachedImpl = null;
                }
            }
        }



        /// <summary>
        /// 删除指定日志记录器
        /// </summary>
        /// <param name="appender"></param>
        /// <returns></returns>
        public IAppender RemoveAppender(IAppender appender)
        {

            lock (this)
            {
                if (appender != null && _mAppenderAttachedImpl != null)
                {
                    return _mAppenderAttachedImpl.RemoveAppender(appender);
                }
            }

            return null;
        }


        /// <summary>
        /// 删除指定名称日志记录器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IAppender RemoveAppender(string name)
        {
            lock (this)
            {
                if (name != null && _mAppenderAttachedImpl != null)
                {
                    return _mAppenderAttachedImpl.RemoveAppender(name);
                }
            }

            return null;
        }
        #endregion

        private AppenderAttachedImpl _mAppenderAttachedImpl;
        private FixFlags _fixFlags = FixFlags.All;
    }
}
