using log4net;
using log4net.Config;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Log.Log4Net
{
    /// <summary>
    /// Log4Net 日志记录器
    /// </summary>
    public sealed class Log4NetHandler<T> : LoggerBase<T> where T : LogEntityBase
    {
        static Log4NetHandler()
        {
            var repository = LogManager.CreateRepository("Log4NetHandler");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            // 默认简单配置，输出至控制台
            BasicConfigurator.Configure(repository);
            _logger = LogManager.GetLogger(repository.Name, "Log4NetHandler");
        }
        private static readonly log4net.ILog _logger;

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
        /// 
        /// </summary>
        public override void ShutDown()
        {
            _logger.Logger.Repository.Shutdown();
        }
    }
}
