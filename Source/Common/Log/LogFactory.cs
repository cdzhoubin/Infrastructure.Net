using System.Collections.Generic;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Extent;
using Zhoubin.Infrastructure.Log.Config;

namespace Zhoubin.Infrastructure.Log
{
    /// <summary>
    /// 日志工厂类
    /// </summary>
    public static class LogFactory
    {
        // ReSharper disable once UnusedMember.Local
        static readonly Dictionary<string, ILogger> DictionaryLoggers = new Dictionary<string, ILogger>();
        static readonly object SyncLogger = new object();

        /// <summary>
        /// 获取默认日志处理器
        /// </summary>
        /// <param name="configName">配置文件路径，全路径</param>
        /// <returns>返回日志处理器</returns>
        public static ILogger GetDefaultLogger(string configName = null)
        {
            return CreateLogger(new LogConfigHelper(configName).Default);
        }

        /// <summary>
        /// 获取日志处理器
        /// </summary>
        /// <param name="loggerName">处理器名称</param>
        /// <param name="configName">配置文件路径，全路径</param>
        /// <returns>返回日志处理器</returns>
        public static ILogger GetLogger(string loggerName, string configName = null)
        {
            var heper = new LogConfigHelper(configName);
            return CreateLogger(heper[loggerName]);
        }

        private static ILogger CreateLogger(LogConfigEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            lock (SyncLogger)
            {
                if (DictionaryLoggers.ContainsKey(entity.Name))
                {
                    return DictionaryLoggers[entity.Name];
                }
                var logger = entity.HandleType.CreateInstance<ILogger>();
                if (logger == null)
                {
                    return null;
                }

                DictionaryLoggers.Add(entity.Name, logger);

                return DictionaryLoggers[entity.Name];
            }
        }

        /// <summary>
        /// 关于日志handle
        /// </summary>
        public static void ShutDown()
        {
            if (DictionaryLoggers.Keys.Count == 0)
            {
                return;
            }

            lock (SyncLogger)
            {
                if (DictionaryLoggers.Keys.Count <= 0)
                {
                    return;
                }

                foreach (var logger in DictionaryLoggers.Values)
                {
                    logger.ShutDown();
                }

                DictionaryLoggers.Clear();
            }
        }
    }
}