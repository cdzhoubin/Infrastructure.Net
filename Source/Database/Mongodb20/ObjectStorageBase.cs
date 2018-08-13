using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Log;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 对象存储基类
    /// </summary>
    public abstract class ObjectStorageBase : ObjectStorageBase<IMongoDatabase>
    {
        /// <summary>
        /// 查询字典数据转换程序
        /// 核心加入前缀和替换Id为_Id
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        protected IDictionary<string, object> DictionaryWarp(IDictionary<string, object> dic)
        {
            return dic.ToDictionary(kv => (kv.Key == "Id" ? "_id" : kv.Key), kv => kv.Value);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        protected ObjectStorageBase(string configName) : base(configName)
        {
        }

        private IMongoDatabase _db;
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
        /// <summary>
        /// 创建表对象
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回表对象Collection</returns>
        protected new IMongoCollection<T> GetCollection<T>() where T : IEntity, new()
        {
            return DataBase.GetCollection<T>(GetCollectionName<T>());
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
            return new GridFSBucket(GetDataBase(name), null);
        }


        protected GridFSFileInfo GetGridFsFile(GridFSBucket gridFS, ObjectId id, bool nullThrowException = true)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Where(p => p.Id == id);
            var fs = gridFS.Find(filter).FirstOrDefault();
            if (fs == null && nullThrowException == true)
            {
                throw new MongoException(string.Format(FileNotExistsErrorMessage, id, gridFS.Database.DatabaseNamespace.DatabaseName));
            }
            return fs;
        }
        protected T GetDocument<T>(ObjectId id, bool nullThrowException = true) where T : IMetaEntity, new()
        {
            var filter = Builders<T>.Filter.Where(p => p.Id == id);
            var fs = GetCollection<T>().Find(filter).FirstOrDefault();
            if (fs == null && nullThrowException)
            {
                throw new MongoException(string.Format(DocumentNotExistsErrorMessage, id));
            }
            return fs;
        }

        protected virtual string FileNotExistsErrorMessage
        {
            get
            {
                return "标识为{0}文件在数据库{1}中没有找到。";
            }
        }
        protected virtual string DocumentNotExistsErrorMessage{
        get {
                return "标识为{0}在数据库中没有找到。";
            }
        }

        protected FilterDefinition<T> ConvertDictionaryToFilterDefinition<T>( IDictionary<string, object> dic)
        {
            if(dic == null && dic.Count == 0)
            {
                return Builders<T>.Filter.Empty;
            }
            if(dic.Count == 1)
            {
                return Builders<T>.Filter.Eq(dic.Keys.First(), dic[dic.Keys.First()]);
            }
            List<FilterDefinition<T>> list = new List<FilterDefinition<T>>();
            foreach(var key in dic.Keys)
            {
                list.Add(Builders<T>.Filter.Eq(key, dic[key]));
            }
            return Builders<T>.Filter.And(list);
        }

        protected SortDefinition<T> ConvertDictionaryToSortDefinition<T>(IDictionary<string, bool> dic)
        {
            if (dic == null && dic.Count == 0)
            {
                return null;
            }
            if (dic.Count == 1)
            {
                return dic[dic.Keys.First()] ? Builders<T>.Sort.Ascending(dic.Keys.First()): Builders<T>.Sort.Descending(dic.Keys.First());
            }
            List<SortDefinition<T>> list = new List<SortDefinition<T>>();
            foreach (var key in dic.Keys)
            {
                list.Add(dic[key] ? Builders<T>.Sort.Ascending(key) : Builders<T>.Sort.Descending(key));
            }
            return Builders<T>.Sort.Combine(list);
        }
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
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