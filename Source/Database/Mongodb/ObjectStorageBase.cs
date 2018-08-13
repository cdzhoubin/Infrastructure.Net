using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Log;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 对象存储基类
    /// </summary>
    public abstract class ObjectStorageBase : ObjectStorageBase<MongoDatabase>
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        protected ObjectStorageBase(string configName):base(configName)
        {
            AutoLoadNavProperty = false;
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
        protected override MongoDatabase CreateDataBase(MongoClient mongoClient, string name)
        {
            var server = mongoClient.GetServer();
            return server.GetDatabase(name);
        }
        /// <summary>
        /// 创建表对象
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回表对象Collection</returns>
        protected new MongoCollection<T> GetCollection<T>() where T : IEntity, new()
        {
            return DataBase.GetCollection<T>(GetCollectionName<T>());
        }

        

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <param name="ob">待加载导航属性对象</param>
        /// <typeparam name="T">导航属性类型</typeparam>
        protected void LoadNavProperty<T>(T ob) where T : IEntity, new()
        {
            foreach (var val in ob.GetType().GetProperties().Select(p => p.GetValue(ob, null)).OfType<IDbRef>())
            {
                val.LoadRef(DataBase);
            }
        }

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <param name="obs">待加载导航属性数组</param>
        /// <typeparam name="T">导航属性类型</typeparam>
        protected void LoadNavProperty<T>(List<T> obs) where T : IEntity, new()
        {
            foreach (var val in obs.SelectMany(ob => ob.GetType().GetProperties().Select(p => p.GetValue(ob, null)).OfType<IDbRef>()))
            {
                val.LoadRef(DataBase);
            }
        }

        /// <summary>
        /// 自动加载导航属性
        /// </summary>
        public bool AutoLoadNavProperty { get; set; }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (DataBase != null && DataBase.Server != null)
            {
                DataBase.Server.Disconnect();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool ParseWriteConcernResult(WriteConcernResult result)
        {
            if (result == null)//连接字符串加入w=0时，此返回值为空
            {
                return true;
            }
            if (result.HasLastErrorMessage)
            {
                LogFactory.GetDefaultLogger().Write(result.LastErrorMessage);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 对象存储基类
    /// </summary>
    public abstract class ObjectStorageBase<TMongoDatabase> : IDisposable
    {

        private readonly string _configName;

        protected string ConfigName { get { return _configName; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        protected ObjectStorageBase(string configName)
        {
            _configName = configName ?? ConfigHelper.GetAppSettings("DefaultConfigName");
            _config = new Config();
        }

        private TMongoDatabase _db;
        // ReSharper disable StaticFieldInGenericType
        private static readonly Dictionary<Type, string> TypeDic = new Dictionary<Type, string>();
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// 数据库
        /// </summary>
        protected TMongoDatabase DataBase
        {
            get { return _db == null ? (_db = GetDataBase()):_db; }
        }

        private Config _config;
        MongoClient _mongoClient;
        protected MongoClient CurrentMongoClient
        {
            get
            {
                return _mongoClient ?? (_mongoClient = GetMongoClient());
            }
        }
        private MongoClient GetMongoClient()
        {
            var entity = _config[ConfigName];
            return new MongoClient(new MongoUrl(entity.ConnectionString));
        }
        /// <summary>
        /// 获取指定的数据库
        /// </summary>
        /// <param name="name">数据为兄名称，如果为空，表示取配置中的数据库名称</param>
        /// <returns>返回数据库对象</returns>
        protected TMongoDatabase GetDataBase(string name = null)
        {
            var entity = _config[ConfigName];
            return CreateDataBase(CurrentMongoClient, name ?? entity.DataBase);
        }

        /// <summary>
        /// 创建Mongo数据库对象
        /// </summary>
        /// <param name="mongoClient">Mongo客户端对象</param>
        /// <returns>返回创建对象</returns>
        protected abstract TMongoDatabase CreateDataBase(MongoClient mongoClient, string name);
        /// <summary>
        /// 创建表对象
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回表对象Collection</returns>
        protected object GetCollection<T>() where T : IEntity, new()
        {
            return null;
        }

        private static readonly Object CollectionNameGetLock = new object();
        /// <summary>
        /// 根据表类型获取表对象名
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回表名</returns>
        protected string GetCollectionName<T>() where T : IEntity, new()
        {
            var type = typeof(T);
            if (!TypeDic.ContainsKey(type))
            {
                lock (CollectionNameGetLock)
                {
                    if (!TypeDic.ContainsKey(type))
                    {
                        var t = new T();
                        TypeDic.Add(type, t.CollectionName);
                    }
                }
            }

            return TypeDic[type];
        }

        /// <summary>
        /// 对你转换成表对象
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <param name="entity">对象</param>
        /// <returns>表实例</returns>
        protected virtual T GetDocumentObject<T>(object entity)
        {
            if (entity == null)
            {
                return default(T);
            }

            if (entity.GetType().IsAssignableFrom(typeof(T)))
            {
                return (T)entity;
            }

            throw new Exception("Error Object Type.");
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="action">操作</param>
        protected void Excute(Action action)
        {
            action();
        }

        /// <summary>
        /// 操作操作
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <param name="action">操作</param>
        /// <param name="entity">实体</param>
        protected void Excute<T>(Action<T> action, T entity)
        {
            action(GetDocumentObject<T>(entity));
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="action">操作</param>
        /// <param name="entity">类型</param>
        /// <returns>返回结果</returns>
        protected TResult Excute<T, TResult>(Func<T, TResult> action, T entity)
        {
            return action(GetDocumentObject<T>(entity));
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="action">函数委托</param>
        /// <returns>返回TResult结果类型</returns>
        protected TResult Excute<TResult>(Func<TResult> action)
        {
            return action();
        }
        

        public abstract void Dispose();
    }
}