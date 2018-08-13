using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// MongoDb�ļ��洢
    /// </summary>
    public class FileDocStorage : ObjectStorageBase, IFileStorage
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="configName">�����ļ�</param>
        public FileDocStorage(string configName)
            : base(configName)
        {

        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="context">�ļ���</param>
        /// <param name="entity">�ļ�Meata����</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <returns>����ɹ������ض���<see cref="ObjectId"/></returns>
        public ObjectId Insert<T>(Stream context, T entity) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                entity.FileSize = context.Length;
                context.Position = 0;
                var fsInfo = DataBase.GridFS.Upload(context, entity.FileName);
                entity.Id = new ObjectId(fsInfo.Id.ToString());
                entity.HashCode = fsInfo.MD5;
                context.Dispose();
                context = null;
                GetCollection<T>().Insert(entity);
                return entity.Id;
            });
        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="entity">�ļ�Meata����</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        public void Update<T>(T entity) where T : IMetaEntity, new()
        {
            var info = GetCollection<T>().FindOneById(entity.Id);
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (info == null)
            {
                return;
            }

            info.Fill(entity);

            var result = GetCollection<T>().Save(info);
            if (ParseWriteConcernResult(result))
            {
                return;
            }

            throw new InstrumentationException("�����ļ�Ԫ���ݳ���");
        }

        /// <summary>
        /// ͨ��ObjectIdɾ���ļ�
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        public void Delete<T>(ObjectId id) where T : IMetaEntity, new()
        {
            if (ParseWriteConcernResult(GetCollection<T>().Remove(Query<T>.Where(p => p.Id == id))))
            {
                DataBase.GridFS.DeleteById(id);
            }
        }

        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <param name="condition">����</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        public void Delete<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            Excute(() =>
                    {
                        var query = new QueryDocument(DictionaryWarp(condition));
                        var list = GetCollection<T>().Find(query).AsQueryable().Select(p => p.Id).ToList();
                        if (list.Count > 0)
                        {
                            GetCollection<T>().Remove(new QueryDocument(DictionaryWarp(condition)));
                            list.ForEach(p => DataBase.GridFS.DeleteById(p));
                        }

                    }
                );
        }

        /// <summary>
        /// ͨ��ObjectId��ѯ�����ļ�
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <returns>���ص��ļ�Meata��Ϣ</returns>
        public T FindById<T>(ObjectId id) where T : IMetaEntity, new()
        {
            return GetCollection<T>().FindOneById(id);
        }

        /// <summary>
        /// ���ҵ����ļ�
        /// </summary>
        /// <param name="condition">��ѯ����</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <returns>���ص��ļ�Meata��Ϣ</returns>
        public T FindOneByCondition<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query = new QueryDocument(DictionaryWarp(condition));
                return GetCollection<T>().FindOne(query);
            }
                );
        }

        /// <summary>
        /// ����Ƿ����ĳ���ļ�
        /// </summary>
        /// <param name="condition">��ѯ����</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <returns>true��ʾ�У�false��ʾ��</returns>
        public bool Any<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query = new QueryDocument(DictionaryWarp(condition));
                return GetCollection<T>().Find(query).Any();
            }
                );
        }

        /// <summary>
        /// ͨ��������ȡ�ļ�
        /// </summary>
        /// <param name="condition">key-valse���Ͳ�ѯ����</param>
        /// <param name="sortList">key-value�������</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <returns>ͨ��������ȡ�ļ�</returns>
        public List<T> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var dic = DictionaryWarp(condition);
                var query = GetCollection<T>().Find(new QueryDocument(dic));
                sortList.ToList().ForEach(p => query = query.SetSortOrder(new SortByDocument(p.Key, new BsonInt32(p.Value ? 1 : -1))));
                return query.ToList();
            }
                );
        }

        /// <summary>
        /// ͨ��������ȡ�ļ�
        /// </summary>
        /// <param name="where">��ķ���ѯ����</param>
        /// <param name="orderby">��ķ���������</param>
        /// <param name="isAsc">true��������false��������</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <typeparam name="TOrderBy">��ķ�ﷵ��������</typeparam>
        /// <returns>�ļ����󼯺�</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {

            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(where, orderby, p => p, isAsc);
            });
        }



        /// <summary>
        /// ��ҳ��ȡ�ļ�
        /// </summary>
        /// <param name="index">��ǰҳ</param>
        /// <param name="pageSize">ÿҳ��С</param>
        /// <param name="where">��ķ���ѯ����</param>
        /// <param name="orderby">��ķ���������</param>
        /// <param name="isAsc">true��������false��������</param>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <typeparam name="TOrderBy">��ķ�ﷵ��������</typeparam>
        /// <returns>�ļ����󼯺�</returns>
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(index, pageSize, where, orderby, p => p.Select(p1 => p1).ToList(), isAsc);
            });
        }

        /// <summary>
        /// ͨ��ObjectId�����ļ�
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <returns>������</returns>
        /// <exception cref="NotImplementedException">δʵ���쳣</exception>
        public Stream DownLoad(ObjectId id)
        {
            MongoGridFSFileInfo info = DataBase.GridFS.FindOneById(id);
            if (info == null)
            {
                throw new MongoException("�ļ������ڡ�");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, info);

            stream.Position = 0;
            return stream;

        }


        /// <summary>
        /// ɾ�������ļ�
        /// </summary>
        /// <typeparam name="T">�ļ��̳�Meata����</typeparam>
        /// <returns>true�ɹ���falseʧ��</returns>
        public bool DeleteAll<T>() where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var list = GetCollection<T>().AsQueryable().Select(p => p.Id).ToList();
                list.ForEach(p =>
                {
                    if (ParseWriteConcernResult(GetCollection<T>().Remove(Query.EQ("_id", p))))
                    {
                        DataBase.GridFS.DeleteById(p);
                    }
                });

                return true;
            }
                );
        }


        /// <summary>
        /// ����ָ����ʶ���ļ�
        /// </summary>
        /// <param name="id">��ʶObjectId</param>
        /// <param name="saveFile">�����ļ�·��</param>
        public void DownLoad(ObjectId id, string saveFile)
        {
            MongoGridFSFileInfo info = DataBase.GridFS.FindOneById(id);
            if (info == null)
            {
                throw new MongoException("�ļ������ڡ�");
            }

            DataBase.GridFS.Download(saveFile, info);
        }


        /// <summary>
        /// ��ѯ�ļ������б�
        /// </summary>
        /// <param name="where">��ѯ����</param>
        /// <param name="orderby">��������</param>
        /// <param name="isAsc">�Ƿ�����</param>
        /// <typeparam name="T">��������</typeparam>
        /// <typeparam name="TOrderBy">��������</typeparam>
        /// <returns>����ָ�����Ͷ���</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("�˷������ļ��ĵ�Ԫ���ݴ洢���޷�ʵ��");
        }


        /// <summary>
        /// ��ҳ��ѯ�ļ��б�
        /// </summary>
        /// <param name="index">����</param>
        /// <param name="pageSize">��ҳ��С</param>
        /// <param name="where">��ѯ����</param>
        /// <param name="orderby">��������</param>
        /// <param name="isAsc">�Ƿ�����</param>
        /// <typeparam name="T">��������</typeparam>
        /// <typeparam name="TOrderBy">��������</typeparam>
        /// <returns>����ָ�����Ͷ���</returns>
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("�˷������ļ��ĵ�Ԫ���ݴ洢���޷�ʵ��");
        }

        /// <summary>
        /// ��ѯ�ļ������б�
        /// </summary>
        /// <param name="query">��ѯ����</param>
        /// <param name="orderby">��������</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>����ָ�����Ͷ���</returns>
        public List<T> AdvanceQuery<T>(Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> query, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> orderby) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("�˷������ļ��ĵ�Ԫ���ݴ洢���޷�ʵ��");
        }

        /// <summary>
        /// ��ҳ��ѯ�ļ��б�
        /// </summary>
        /// <param name="index">����</param>
        /// <param name="pageSize">��ҳ��С</param>
        /// <param name="query">��ѯ����</param>
        /// <param name="orderby">��������</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>����ָ�����Ͷ���</returns>
        public PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> query, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> orderby) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("�˷������ļ��ĵ�Ԫ���ݴ洢���޷�ʵ��");
        }

        /// <summary>
        /// ��ѯ�ļ������б�
        /// </summary>
        /// <param name="query">��ѯ����</param>
        /// <param name="orderby">��������</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>����ָ�����Ͷ���</returns>
        public List<T> AdvanceQuery<T>(Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query1 = query(GetCollection<T>().AsQueryable());
                if (orderby != null)
                {
                    query1 = orderby(query1);
                }

                return query1.Select(p1 => p1).ToList();
            });
        }

        /// <summary>
        /// ��ҳ��ѯ�ļ��б�
        /// </summary>
        /// <param name="index">����</param>
        /// <param name="pageSize">��ҳ��С</param>
        /// <param name="query">��ѯ����</param>
        /// <param name="orderby">��������</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>����ָ�����Ͷ���</returns>
        public PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query1 = query(GetCollection<T>().AsQueryable());
                return query1.Query(index, pageSize, null, orderby, p => p.Select(p1 => p1).ToList());
            });
        }


        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="condition">�����ֵ�����</param>
        /// <returns>��������������</returns>
        public Stream DownLoad(IDictionary<string, object> condition)
        {
            throw new InstrumentationException("�˷�����DocԪ���ݴ洢���޷�ʵ�֣���ʹ�÷��ͷ���");
        }
        private T GetEntity<T>(ObjectId id) where T : IMetaEntity, new()
        {
            var info = GetCollection<T>().FindOneById(id);
            if (info == null)
            {
                throw new FileNotFoundException(string.Format("��ʶΪ{0}�ļ�û���ҵ���", id));
            }
            return info;
        }

        private MongoGridFSFileInfo GetMongoGridFsFileInfo<T>(T entity) where T : IMetaEntity
        {
            var fs = GetDataBase(entity.Database).GridFS.FindOneById(entity.Id);
            if (fs == null)
            {
                throw new MongoException(string.Format("��ʶΪ{0}�ļ������ݿ�{1}��û���ҵ���", entity.Id, entity.Database));
            }
            return fs;
        }
        /// <summary>
        /// ����ָ����ʶ���ļ�
        /// </summary>
        /// <typeparam name="T">�ļ�Ԫ��������</typeparam>
        /// <param name="id">��ʶ</param>
        /// <returns>������</returns>
        public Stream DownLoad<T>(ObjectId id) where T : IMetaEntity, new()
        {
            var info = GetEntity<T>(id);
            var fs = GetMongoGridFsFileInfo(info);
            var stream = new MemoryStream();
            GetDataBase(info.Database).GridFS.Download(stream,fs);
            return stream;
        }

        public void DownLoad<T>(ObjectId id, string saveFile) where T : IMetaEntity, new()
        {
            var info = GetEntity<T>(id);
            GetMongoGridFsFileInfo(info);
            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, saveFile);
        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="condition">�����ֵ�����</param>
        /// <param name="saveFile">�����ļ���ָ��Ŀ¼</param>
        public void DownLoad(IDictionary<string, object> condition, string saveFile)
        {
            throw new InstrumentationException("�˷�����DocԪ���ݴ洢���޷�ʵ�֣���ʹ�÷��ͷ���");
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="condition">�����ֵ�����</param>
        /// <param name="saveFile">�����ļ���ָ��Ŀ¼</param>

        public void DownLoad<T>(IDictionary<string, object> condition, string saveFile) where T : IMetaEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                throw new InstrumentationException("����condition����Ϊnull�������Ϊ0��");
            }

            var query = new QueryDocument(DictionaryWarp(condition));
            var id = GetCollection<T>().Find(query).AsQueryable().Select(p => p.Id).FirstOrDefault();
            if (id == ObjectId.Empty)
            {
                throw new FileNotFoundException();
            }

            MongoGridFSFileInfo info = DataBase.GridFS.FindOneById(id);
            if (info == null)
            {
                throw new MongoException("�ļ������ڡ�");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, saveFile);
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="condition">�����ֵ�����</param>
        /// <returns>��������������</returns>
        public Stream DownLoad<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                throw new InstrumentationException("����condition����Ϊnull�������Ϊ0��");
            }

            var query = new QueryDocument(DictionaryWarp(condition));
            var id = GetCollection<T>().Find(query).AsQueryable().Select(p => p.Id).FirstOrDefault();
            if (id == ObjectId.Empty)
            {
                throw new FileNotFoundException();
            }

            MongoGridFSFileInfo info = DataBase.GridFS.FindOneById(id);
            if (info == null)
            {
                throw new MongoException("�ļ������ڡ�");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, info);

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// ��ѯ�ֵ�����ת������
        /// ���ļ���ǰ׺���滻IdΪ_Id
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> DictionaryWarp(IEnumerable<KeyValuePair<string, object>> dic)
        {
            return dic.ToDictionary(kv => (kv.Key == "Id" ? "_id" : kv.Key), kv => kv.Value);
        }
    }
}