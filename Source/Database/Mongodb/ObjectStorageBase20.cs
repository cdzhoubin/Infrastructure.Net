using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 对象存储基类
    /// </summary>
    public abstract class ObjectStorageBase20 : ObjectStorageBase<IMongoDatabase>
    {
        /// <summary>
        /// 自动加载导航属性
        /// </summary>
        public  bool AutoLoadNavProperty { get { return false; } set { } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        protected ObjectStorageBase20(string configName) : base(configName)
        {
        }

        //private MongoDatabase _db;
        // ReSharper disable StaticFieldInGenericType
        private static readonly Dictionary<Type, string> TypeDic = new Dictionary<Type, string>();
        // ReSharper restore StaticFieldInGenericType



        /// <summary>
        /// 获取指定的数据库
        /// </summary>
        /// <param name="name">数据为兄名称，如果为空，表示取配置中的数据库名称</param>
        /// <returns>返回数据库对象</returns>
        protected override IMongoDatabase CreateDataBase(MongoClient mongoClient, string name)
        {
            return mongoClient.GetDatabase(name);
        }

        GridFSBucket _gridFSBucket;
        protected GridFSBucket GridFS
        {
            get
            {
                return _gridFSBucket ?? (_gridFSBucket = new GridFSBucket(DataBase, null));
            }
        }

        /// <summary>
        /// 获取指定的数据库
        /// </summary>
        /// <param name="name">数据为兄名称，如果为空，表示取配置中的数据库名称</param>
        /// <returns>返回数据库对象</returns>
        protected GridFSBucket GetGridFS(string name = null)
        {
            return new GridFSBucket(GetDataBase(name),null);
        }
        /// <summary>
        /// 创建表对象
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回表对象Collection</returns>
        protected new IMongoCollection<T> GetCollection<T>() where T : IEntity, new()
        {
            return DataBase.GetCollection<T>(GetCollectionName<T>());
        }       

        

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
        }
    }
}