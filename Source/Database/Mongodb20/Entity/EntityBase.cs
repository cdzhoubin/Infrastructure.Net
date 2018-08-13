using System;

namespace Zhoubin.Infrastructure.Common.MongoDb.Entity
{
    /// <summary>
    /// 存储实体基类
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// 存储集名称
        /// </summary>
        public abstract string CollectionName { get; }
    }
}
