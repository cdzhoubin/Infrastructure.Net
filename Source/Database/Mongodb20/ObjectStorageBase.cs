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
    /// ����洢����
    /// </summary>
    public abstract class ObjectStorageBase : ObjectStorageBase<IMongoDatabase>
    {
        /// <summary>
        /// ��ѯ�ֵ�����ת������
        /// ���ļ���ǰ׺���滻IdΪ_Id
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        protected IDictionary<string, object> DictionaryWarp(IDictionary<string, object> dic)
        {
            return dic.ToDictionary(kv => (kv.Key == "Id" ? "_id" : kv.Key), kv => kv.Value);
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="configName">�����ļ�</param>
        protected ObjectStorageBase(string configName) : base(configName)
        {
        }

        private IMongoDatabase _db;
        // ReSharper disable StaticFieldInGenericType
        private static readonly Dictionary<Type, string> TypeDic = new Dictionary<Type, string>();
        // ReSharper restore StaticFieldInGenericType



        /// <summary>
        /// ��ȡָ�������ݿ�
        /// </summary>
        /// <param name="name">����Ϊ�����ƣ����Ϊ�գ���ʾȡ�����е����ݿ�����</param>
        /// <returns>�������ݿ����</returns>
        protected override IMongoDatabase CreateDataBase(MongoClient mongoClient, string name)
        {
            return mongoClient.GetDatabase(name);
        }
        /// <summary>
        /// ���������
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <returns>���ر����Collection</returns>
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
        /// ��ȡָ�������ݿ�
        /// </summary>
        /// <param name="name">����Ϊ�����ƣ����Ϊ�գ���ʾȡ�����е����ݿ�����</param>
        /// <returns>�������ݿ����</returns>
        protected GridFSBucket GetGridFS(string name = null)
        {
            return new GridFSBucket(GetDataBase(name), null);
        }


        protected GridFSFileInfo GetGridFsFile(GridFSBucket gridFS, ObjectId id, bool nullThrowException = true)
        {
            var fs = gridFS.OpenDownloadStream(id);            
            //var filter = Builders<GridFSFileInfo>.Filter.Where(p => p.Id == id);
            //var fs = gridFS.Find(filter).FirstOrDefault();
            if ((fs == null || fs.FileInfo == null) && nullThrowException == true)
            {
                throw new MongoException(string.Format(FileNotExistsErrorMessage, id, gridFS.Database.DatabaseNamespace.DatabaseName));
            }
            return fs.FileInfo;
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
                return "��ʶΪ{0}�ļ������ݿ�{1}��û���ҵ���";
            }
        }
        protected virtual string DocumentNotExistsErrorMessage
        {
            get
            {
                return "��ʶΪ{0}�����ݿ���û���ҵ���";
            }
        }

        protected FilterDefinition<T> ConvertDictionaryToFilterDefinition<T>(IDictionary<string, object> dic)
        {
            if (dic == null && dic.Count == 0)
            {
                return Builders<T>.Filter.Empty;
            }
            if (dic.Count == 1)
            {
                return Builders<T>.Filter.Eq(dic.Keys.First(), dic[dic.Keys.First()]);
            }
            List<FilterDefinition<T>> list = new List<FilterDefinition<T>>();
            foreach (var key in dic.Keys)
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
                return dic[dic.Keys.First()] ? Builders<T>.Sort.Ascending(dic.Keys.First()) : Builders<T>.Sort.Descending(dic.Keys.First());
            }
            List<SortDefinition<T>> list = new List<SortDefinition<T>>();
            foreach (var key in dic.Keys)
            {
                list.Add(dic[key] ? Builders<T>.Sort.Ascending(key) : Builders<T>.Sort.Descending(key));
            }
            return Builders<T>.Sort.Combine(list);
        }
        /// <summary>
        /// ִ�����ͷŻ����÷��й���Դ��ص�Ӧ�ó����������
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
            if (result == null)//�����ַ�������w=0ʱ���˷���ֵΪ��
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
    /// ����洢����
    /// </summary>
    public abstract class ObjectStorageBase<TMongoDatabase> : IDisposable
    {

        private readonly string _configName;

        protected string ConfigName { get { return _configName; } }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="configName">�����ļ�</param>
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
        /// ���ݿ�
        /// </summary>
        protected TMongoDatabase DataBase
        {
            get { return _db == null ? (_db = GetDataBase()) : _db; }
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
        /// ��ȡָ�������ݿ�
        /// </summary>
        /// <param name="name">����Ϊ�����ƣ����Ϊ�գ���ʾȡ�����е����ݿ�����</param>
        /// <returns>�������ݿ����</returns>
        protected TMongoDatabase GetDataBase(string name = null)
        {
            var entity = _config[ConfigName];
            return CreateDataBase(CurrentMongoClient, name ?? entity.DataBase);
        }

        /// <summary>
        /// ����Mongo���ݿ����
        /// </summary>
        /// <param name="mongoClient">Mongo�ͻ��˶���</param>
        /// <returns>���ش�������</returns>
        protected abstract TMongoDatabase CreateDataBase(MongoClient mongoClient, string name);
        /// <summary>
        /// ���������
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <returns>���ر����Collection</returns>
        protected object GetCollection<T>() where T : IEntity, new()
        {
            return null;
        }

        private static readonly Object CollectionNameGetLock = new object();
        /// <summary>
        /// ���ݱ����ͻ�ȡ�������
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <returns>���ر���</returns>
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
        /// ����ת���ɱ����
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <param name="entity">����</param>
        /// <returns>��ʵ��</returns>
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
        /// ִ�в���
        /// </summary>
        /// <param name="action">����</param>
        protected void Excute(Action action)
        {
            action();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <param name="action">����</param>
        /// <param name="entity">ʵ��</param>
        protected void Excute<T>(Action<T> action, T entity)
        {
            action(GetDocumentObject<T>(entity));
        }

        /// <summary>
        /// ִ�в���
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="TResult">���ؽ������</typeparam>
        /// <param name="action">����</param>
        /// <param name="entity">����</param>
        /// <returns>���ؽ��</returns>
        protected TResult Excute<T, TResult>(Func<T, TResult> action, T entity)
        {
            return action(GetDocumentObject<T>(entity));
        }

        /// <summary>
        /// ִ�в���
        /// </summary>
        /// <typeparam name="TResult">�������</typeparam>
        /// <param name="action">����ί��</param>
        /// <returns>����TResult�������</returns>
        protected TResult Excute<TResult>(Func<TResult> action)
        {
            return action();
        }


        public abstract void Dispose();
    }
}