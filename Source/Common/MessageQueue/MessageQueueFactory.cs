using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MessageQueue
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
            return CreateMessageQueue(new MessageQueueConfigHelper().Default);
        }

        /// <summary>
        /// 获取消息处理器
        /// </summary>
        /// <param name="queryName">处理器名称</param>
        /// <returns>返回消息处理器,创建失败返回null</returns>
        public static IMessageQueue GetMessageQueue(string queryName)
        {
            var heper = new MessageQueueConfigHelper();
            return CreateMessageQueue(heper[queryName]);
        }

        /// <summary>
        /// 创建消息队列实例
        /// </summary>
        /// <param name="entity">队列配置</param>
        /// <returns>返回创建成功的队列实例，返回null表示创建失败</returns>
        public static IMessageQueue CreateMessageQueue(MessageQueueEntity entity)
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
        public static IAutoReveiveMessage GetDefaultAutoReceiveQueue()
        {
            return CreateAutoReceiveMessage(new MessageQueueConfigHelper().Default);
        }

        /// <summary>
        /// 获取自动消息处理器
        /// </summary>
        /// <param name="queryName">处理器名称</param>
        /// <returns>返回消息处理器,创建失败返回null</returns>
        public static IAutoReveiveMessage GetAutoReceiveQueue(string queryName)
        {
            var heper = new MessageQueueConfigHelper();
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