using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// MongoDb文件存储
    /// </summary>
    public class FileStorage : ObjectStorageBase, IFileStorage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        public FileStorage(string configName)
            : base(configName)
        {

        }

        /// <summary>
        /// 插入文件
        /// </summary>
        /// <param name="context">文件流</param>
        /// <param name="entity">文件Meata对象</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>成功返回对象标识</returns>
        public ObjectId Insert<T>(Stream context, T entity) where T : IMetaEntity, new()
        {
            return Excute(() =>
                {
                    entity.FileSize = context.Length;
                    context.Position = 0;
                    var fsInfo = DataBase.GridFS.Upload(context, entity.FileName);
                    // ReSharper disable SpecifyACultureInStringConversionExplicitly
                    entity.Id = new ObjectId(fsInfo.Id.ToString());
                    // ReSharper restore SpecifyACultureInStringConversionExplicitly
                    entity.HashCode = fsInfo.MD5;
                    context.Dispose();
                    context = null;
                    var bson = entity.ToBsonDocument();
                    DataBase.GridFS.SetMetadata(fsInfo, bson);
                    return entity.Id;
                });
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        /// <param name="entity">文件Meata对象</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        public void Update<T>(T entity) where T : IMetaEntity, new()
        {
            var info = DataBase.GridFS.FindOneById(entity.Id);
            if (info == null)
            {
                return;
            }

            var bson = entity.ToBsonDocument();
            DataBase.GridFS.SetMetadata(info, bson);
        }

        /// <summary>
        /// 通过ObjectId删除文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        public void Delete<T>(ObjectId id) where T : IMetaEntity, new()
        {
            DataBase.GridFS.DeleteById(id);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        public void Delete<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            Excute(() =>
                {
                    var dic = DictionaryWarp(condition);
                    var query = new QueryDocument(dic);
                    DataBase.GridFS.Delete(query);
                }
                );
        }

        /// <summary>
        /// 通过ObjectId查询单个文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>返回的文件Meata信息</returns>
        public T FindById<T>(ObjectId id) where T : IMetaEntity, new()
        {
            var info = DataBase.GridFS.FindOneById(id);
            return Deseralize<T>(info);
        }

        /// <summary>
        /// 查找单个文件
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>返回的文件Meata信息</returns>
        public T FindOneByCondition<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var dic = DictionaryWarp(condition);
                var query = new QueryDocument(dic);
                var info = DataBase.GridFS.FindOne(query);
                return Deseralize<T>(info);
            }
               );
        }

        /// <summary>
        /// 检查是否包含某个文件
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>true表示有，false表示无</returns>
        public bool Any<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var dic = DictionaryWarp(condition);
                var query = new QueryDocument(dic);
                return DataBase.GridFS.Find(query).Any();
            }
               );
        }

        /// <summary>
        /// 字典数据前缀设置
        /// </summary>
       const  string KeyPre ="metadata."; 
        /// <summary>
        /// 查询字典数据转换程序
        /// 核心加入前缀和替换Id为_Id
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> DictionaryWarp(IEnumerable<KeyValuePair<string, object>> dic)
        {
            return dic.ToDictionary(kv => KeyPre + (kv.Key == "Id" ? "_id" : kv.Key), kv => kv.Value);
        }

        /// <summary>
        /// 通过条件获取文件
        /// </summary>
        /// <param name="condition">key-valse类型查询条件</param>
        /// <param name="sortList">key-value排序规则</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>通过条件获取文件</returns>
        public List<T> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var dic = DictionaryWarp(condition);
                var query = DataBase.GridFS.Find(new QueryDocument(dic));
                if (sortList != null)
                {
                    sortList.ToList().ForEach(p => query = query.SetSortOrder(new SortByDocument("metadata." + p.Key, new BsonInt32(p.Value ? 1 : -1))));
                }
                return query.ToList().ConvertAll(Deseralize<T>);
            }
               );
        }

        /// <summary>
        /// 通过条件获取文件
        /// </summary>
        /// <param name="where">拉姆达查询条件</param>
        /// <param name="orderby">拉姆达排序规则</param>
        /// <param name="isAsc">true升序排序，false降序排序</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <typeparam name="TOrderBy">拉姆达返回排序列</typeparam>
        /// <returns>文件对象集合</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {

            return Excute(() =>
            {
                var query = DataBase.GridFS.FindAll().AsQueryable();
                return query.Query(where, orderby, p => Deseralize<T>(p), isAsc).ToList();
            });
        }



        /// <summary>
        /// 分页获取文件
        /// </summary>
        /// <param name="index">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">拉姆达查询条件</param>
        /// <param name="orderby">拉姆达排序规则</param>
        /// <param name="isAsc">true升序排序，false降序排序</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <typeparam name="TOrderBy">拉姆达返回排序列</typeparam>
        /// <returns>文件对象集合</returns>
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query = DataBase.GridFS.FindAll().AsQueryable();
                return query.Query(index, pageSize, where, orderby, p => p.Select(p1 => Deseralize<T>(p1)).ToList(), isAsc);
            });
        }


        /// <summary>
        /// 通过条件获取文件
        /// </summary>
        /// <param name="where">拉姆达查询条件</param>
        /// <param name="orderby">拉姆达排序规则</param>
        /// <param name="isAsc">true升序排序，false降序排序</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <typeparam name="TOrderBy">拉姆达返回排序列</typeparam>
        /// <returns>文件对象集合</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("此方法在File元数据存储中无法实现");
        }


        /// <summary>
        /// 分页获取文件
        /// </summary>
        /// <param name="index">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">拉姆达查询条件</param>
        /// <param name="orderby">拉姆达排序规则</param>
        /// <param name="isAsc">true升序排序，false降序排序</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <typeparam name="TOrderBy">拉姆达返回排序列</typeparam>
        /// <returns>文件对象集合</returns>
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("此方法在File元数据存储中无法实现");
        }

        /// <summary>
        /// 通过ObjectId下载文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <returns>返回流</returns>
        /// <exception cref="NotImplementedException">未实现异常</exception>
        public Stream DownLoad(ObjectId id)
        {
            if (id == ObjectId.Empty)
            {
                throw new ArgumentNullException("id");
            }

            MongoGridFSFileInfo info = DataBase.GridFS.FindOneById(id);
            if (info == null)
            {
                throw new MongoException("文件不存在。");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, info);

            stream.Position = 0;
            return stream;
        }

        private static T Deseralize<T>(MongoGridFSFileInfo doc) where T : IEntity
        {
            var result = BsonSerializer.Deserialize<T>(doc.Metadata);
            return result;
        }


        /// <summary>
        /// 删除所有文件
        /// </summary>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>true成功，false失败</returns>
        public bool DeleteAll<T>() where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                DataBase.GridFS.Delete(Query.EQ("metadata.EntityHash", GetCollectionName<T>().GetHashCode()));
                return true;
            }
               );
        }


        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <param name="saveFile">保存文件路径</param>
        /// <returns></returns>
        public void DownLoad(ObjectId id, string saveFile)
        {
            if (id == ObjectId.Empty)
            {
                throw new ArgumentNullException("id");
            }

            if (string.IsNullOrEmpty(saveFile))
            {
                throw new ArgumentNullException("saveFile");
            }

            MongoGridFSFileInfo info = DataBase.GridFS.FindOneById(id);
            if (info == null)
            {
                throw new MongoException("文件不存在。");
            }

            DataBase.GridFS.Download(saveFile, info);
        }


        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public List<T> AdvanceQuery<T>(Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> query, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> orderby) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query1 = query(DataBase.GridFS.FindAll().AsQueryable());
                if (orderby != null)
                {
                    query1 = orderby(query1);
                }

                return query1.Select(p1 => Deseralize<T>(p1)).ToList();
            });
        }

        /// <summary>
        /// 分页查询文件列表
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> query, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> orderby) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query1 = query(DataBase.GridFS.FindAll().AsQueryable());
                return query1.Query(index, pageSize, null, orderby, p => p.Select(p1 => Deseralize<T>(p1)).ToList());
            });
        }

        /// <summary>
        /// 查询文件对象列表
        /// 此方法在File元数据存储中无法实现
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public List<T> AdvanceQuery<T>(Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("此方法在File元数据存储中无法实现");
        }

        /// <summary>
        /// 分页查询文件列表
        /// 此方法在File元数据存储中无法实现
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("此方法在File元数据存储中无法实现");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <returns>返回下载流数据</returns>
        public Stream DownLoad(IDictionary<string, object> condition)
        {
            if (condition == null || condition.Count == 0)
            {
                throw new InstrumentationException("条件condition不能为null或键数据为0。");
            }

            var query = new QueryDocument(DictionaryWarp(condition));
            MongoGridFSFileInfo info = DataBase.GridFS.FindOne(query);
            if (info == null)
            {
                throw new MongoException("文件不存在。");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, info);

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
        public Stream DownLoad<T>(ObjectId id) where T : IMetaEntity, new()
        {
            return DownLoad(id);
        }

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <param name="saveFile">保存文件路径</param>
        public void DownLoad<T>(ObjectId id, string saveFile) where T : IMetaEntity, new()
        {
            DownLoad(id,saveFile);
        }

        /// <summary>
        /// 下载指定条件的文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="saveFile">保存文件路径</param>
        public void DownLoad(IDictionary<string, object> condition, string saveFile)
        {
            if (string.IsNullOrEmpty(saveFile))
            {
                throw new ArgumentNullException("saveFile");
            }

            if (condition == null || condition.Count == 0)
            {
                throw new InstrumentationException("条件condition不能为null或键数据为0。");
            }

            var query = new QueryDocument(DictionaryWarp(condition));
            MongoGridFSFileInfo info = DataBase.GridFS.FindOne(query);
            if (info == null)
            {
                throw new MongoException("文件不存在。");
            }

            DataBase.GridFS.Download(saveFile, info);
        }


        /// <summary>
        /// 下载指定条件的文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="saveFile">保存文件路径</param>
        public void DownLoad<T>(IDictionary<string, object> condition, string saveFile) where T : IMetaEntity, new()
        {
            DownLoad(condition,saveFile);
        }

        /// <summary>
        /// 根据指定条件下载查询到的第一个文件
        /// 用于文档型数据查询
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>true:存在</returns>
        public Stream DownLoad<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            return DownLoad(condition);
        }
    }
}
