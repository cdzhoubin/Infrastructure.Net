namespace Zhoubin.Infrastructure.Common.Log.NLog
{
    /// <summary>
    /// NLog  日志记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NLogHandler<T> : LoggerBase<T> where T:LogEntityBase
    {
        private readonly global::NLog.Logger _logger = global::NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="info"></param>
        protected override void Write(T info)
        {
            switch (info.Sevenrity)
            {
                case Sevenrity.Debug:
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.Debug(info);
                    }
                    break;
                case Sevenrity.Error:
                    if (_logger.IsErrorEnabled)
                    {
                        _logger.Error(info);
                    }
                    break;
                case Sevenrity.Fatal:
                    if (_logger.IsFatalEnabled)
                    {
                        _logger.Fatal(info);
                    }
                    break;
                case Sevenrity.Info:
                    if (_logger.IsInfoEnabled)
                    {
                        _logger.Info(info);
                    }
                    break;
                case Sevenrity.Trace:
                    if (_logger.IsWarnEnabled)
                    {
                        _logger.Info(info);
                    }
                    break;
                case Sevenrity.Warn:
                    if (_logger.IsWarnEnabled)
                    {
                        _logger.Warn(info);
                    }
                    break;
            }
        }

        /// <summary>
        /// 写入字符串日志
        /// </summary>
        /// <param name="info"></param>
        protected override void Write(string info)
        {
            _logger.Info(info);
        }

        /// <summary>
        /// 关闭日志处理器
        /// </summary>
        public override void ShutDown()
        {
            
        }
    }
    
}
