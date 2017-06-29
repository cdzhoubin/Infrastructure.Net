using Zhoubin.Infrastructure.Common.Extent;
using Zhoubin.Infrastructure.MessageQueue.Config;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// 消息队列工厂
    /// </summary>
    public static class MessageQueueFactory
    {
        /// <summary>
        /// 获取默认消息处理器
        /// </summary>
        /// <param name="configFileName">配置文件路径，全路径</param>
        /// <returns>返回消息处理器,创建失败返回null</returns>
        public static IMessageQueue GetDefaultMessageQueue(string configFileName = null)
        {
            return CreateMessageQueue(new MessageQueueConfigHelper(configFileName).Default);
        }

        /// <summary>
        /// 获取消息处理器
        /// </summary>
        /// <param name="queryName">处理器名称</param>
        /// <param name="configFileName">配置文件路径，全路径</param>
        /// <returns>返回消息处理器,创建失败返回null</returns>
        public static IMessageQueue GetMessageQueue(string queryName, string configFileName = null)
        {
            var heper = new MessageQueueConfigHelper(configFileName);
            return CreateMessageQueue(heper[queryName]);
        }

        private static IMessageQueue CreateMessageQueue(MessageQueueEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(entity.HandleType))
            {
                return null;
            }

            var query = entity.HandleType.CreateInstance<IMessageQueue>();
            query.Initiate(entity);
            return query;
        }


        /// <summary>
        /// 获取默认自动消息处理器
        /// </summary>
        /// <param name="configFileName">配置文件路径，全路径</param>
        /// <returns>返回消息处理器,创建失败返回null</returns>
        public static IAutoReveiveMessage GetDefaultAutoReceiveQueue(string configFileName = null)
        {
            return CreateAutoReceiveMessage(new MessageQueueConfigHelper(configFileName).Default);
        }

        /// <summary>
        /// 获取自动消息处理器
        /// </summary>
        /// <param name="queryName">处理器名称</param>
        /// <param name="configFileName">配置文件路径，全路径</param>
        /// <returns>返回消息处理器,创建失败返回null</returns>
        public static IAutoReveiveMessage GetAutoReceiveQueue(string queryName, string configFileName = null)
        {
            var heper = new MessageQueueConfigHelper(configFileName);
            return CreateAutoReceiveMessage(heper[queryName]);
        }

        private static IAutoReveiveMessage CreateAutoReceiveMessage(MessageQueueEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(entity.AutoHandleType))
            {
                return null;
            }

            var query = entity.AutoHandleType.CreateInstance<IAutoReveiveMessage>();
            query.Initiate(entity);
            return query;
        }
    }
}