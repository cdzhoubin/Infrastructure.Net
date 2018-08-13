using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 对象实体基类
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 存储的集合名称，类似sql表名
        /// </summary>
        string CollectionName { get; }
    }

    /// <summary>
    /// 文档对象接口
    /// </summary>
    public interface IDocumentEntity:IEntity
    {
        /// <summary>
        /// ObjectId  MongoDb生成的唯一标示
        /// </summary>
        ObjectId Id { get; set; }
        /// <summary>
        /// 填充方法
        /// </summary>
        /// <param name="entity">填充对象</param>
        void Fill(IEntity entity);
        
    }

    /// <summary>
    /// 文件Meata接口
    /// </summary>
    public interface IMetaEntity:IEntity
    {
        /// <summary>
        /// 填充方法
        /// </summary>
        /// <param name="entity">填充对象</param>
        void Fill(IEntity entity);

        /// <summary>
        /// ObjectId转换为字符串
        /// </summary>
        ObjectId Id { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        string ContentType { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        long FileSize { get; set; }
        /// <summary>
        /// 文件哈希值
        /// </summary>
        string HashCode { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 文件描述
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 文件存储的数据库名称，默认值为空，表示存储当前数据库
        /// </summary>
        string Database { get; set; }
    }

    /// <summary>
    /// 对象引用加载接口
    /// </summary>
    [Obsolete("此方法不再使用")]
    public interface IDbRef
    {
        /// <summary>
        /// 加载引用对象
        /// </summary>
        /// <param name="mongoDatabase">数据库对象</param>
        void LoadRef(MongoDatabase mongoDatabase);
    }
    
}