using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Log;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// ����洢����
    /// </summary>
    public abstract class ObjectStorageBase : ObjectStorageBase<MongoDatabase>
    {

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="configName">�����ļ�</param>
        protected ObjectStorageBase(string configName):base(configName)
        {
            AutoLoadNavProperty = false;
        }

        //private MongoDatabase _db;
        // ReSharper disable StaticFieldInGenericType
        private static readonly Dictionary<Type, string> TypeDic = new Dictionary<Type, string>();
        // ReSharper restore StaticFieldInGenericType

        

        /// <summary>
        /// ��ȡָ�������ݿ�
        /// </summary>
        /// <param name="name">����Ϊ�����ƣ����Ϊ�գ���ʾȡ�����е����ݿ�����</param>
        /// <returns>�������ݿ����</returns>
        protected override MongoDatabase CreateDataBase(MongoClient mongoClient, string name)
        {
            var server = mongoClient.GetServer();
            return server.GetDatabase(name);
        }
        /// <summary>
        /// ���������
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <returns>���ر����Collection</returns>
        protected new MongoCollection<T> GetCollection<T>() where T : IEntity, new()
        {
            return DataBase.GetCollection<T>(GetCollectionName<T>());
        }

        

        /// <summary>
        /// ���ص�������
        /// </summary>
        /// <param name="ob">�����ص������Զ���</param>
        /// <typeparam name="T">������������</typeparam>
        protected void LoadNavProperty<T>(T ob) where T : IEntity, new()
        {
            foreach (var val in ob.GetType().GetProperties().Select(p => p.GetValue(ob, null)).OfType<IDbRef>())
            {
                val.LoadRef(DataBase);
            }
        }

        /// <summary>
        /// ���ص�������
        /// </summary>
        /// <param name="obs">�����ص�����������</param>
        /// <typeparam name="T">������������</typeparam>
        protected void LoadNavProperty<T>(List<T> obs) where T : IEntity, new()
        {
            foreach (var val in obs.SelectMany(ob => ob.GetType().GetProperties().Select(p => p.GetValue(ob, null)).OfType<IDbRef>()))
            {
                val.LoadRef(DataBase);
            }
        }

        /// <summary>
        /// �Զ����ص�������
        /// </summary>
        public bool AutoLoadNavProperty { get; set; }

        /// <summary>
        /// ִ�����ͷŻ����÷��й���Դ��ص�Ӧ�ó����������
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