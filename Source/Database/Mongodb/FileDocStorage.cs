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
    /// MongoDb文件存储
    /// </summary>
    public class FileDocStorage : ObjectStorageBase, IFileStorage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        public FileDocStorage(string configName)
            : base(configName)
        {

        }

        /// <summary>
        /// 插入文件
        /// </summary>
        /// <param name="context">文件流</param>
        /// <param name="entity">文件Meata对象</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>插入成功，返回对象<see cref="ObjectId"/></returns>
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
        /// 更新文件
        /// </summary>
        /// <param name="entity">文件Meata对象</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
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

            throw new InstrumentationException("更新文件元数据出错。");
        }

        /// <summary>
        /// 通过ObjectId删除文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        public void Delete<T>(ObjectId id) where T : IMetaEntity, new()
        {
            if (ParseWriteConcernResult(GetCollection<T>().Remove(Query<T>.Where(p => p.Id == id))))
            {
                DataBase.GridFS.DeleteById(id);
            }
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
        /// 通过ObjectId查询单个文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>返回的文件Meata信息</returns>
        public T FindById<T>(ObjectId id) where T : IMetaEntity, new()
        {
            return GetCollection<T>().FindOneById(id);
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
                var query = new QueryDocument(DictionaryWarp(condition));
                return GetCollection<T>().FindOne(query);
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
                var query = new QueryDocument(DictionaryWarp(condition));
                return GetCollection<T>().Find(query).Any();
            }
                );
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
                var query = GetCollection<T>().Find(new QueryDocument(dic));
                sortList.ToList().ForEach(p => query = query.SetSortOrder(new SortByDocument(p.Key, new BsonInt32(p.Value ? 1 : -1))));
                return query.ToList();
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
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {

            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(where, orderby, p => p, isAsc);
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
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(index, pageSize, where, orderby, p => p.Select(p1 => p1).ToList(), isAsc);
            });
        }

        /// <summary>
        /// 通过ObjectId下载文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <returns>返回流</returns>
        /// <exception cref="NotImplementedException">未实现异常</exception>
        public Stream DownLoad(ObjectId id)
        {
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


        /// <summary>
        /// 删除所有文件
        /// </summary>
        /// <typeparam name="T">文件继承Meata类型</typeparam>
        /// <returns>true成功，false失败</returns>
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
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="id">标识ObjectId</param>
        /// <param name="saveFile">保存文件路径</param>
        public void DownLoad(ObjectId id, string saveFile)
        {
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
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("此方法在文件文档元数据存储中无法实现");
        }


        /// <summary>
        /// 分页查询文件列表
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new()
        {
            throw new InstrumentationException("此方法在文件文档元数据存储中无法实现");
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
            throw new InstrumentationException("此方法在文件文档元数据存储中无法实现");
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
            throw new InstrumentationException("此方法在文件文档元数据存储中无法实现");
        }

        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
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
        /// 分页查询文件列表
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        public PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var query1 = query(GetCollection<T>().AsQueryable());
                return query1.Query(index, pageSize, null, orderby, p => p.Select(p1 => p1).ToList());
            });
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <returns>返回下载流数据</returns>
        public Stream DownLoad(IDictionary<string, object> condition)
        {
            throw new InstrumentationException("此方法在Doc元数据存储中无法实现，请使用泛型方法");
        }
        private T GetEntity<T>(ObjectId id) where T : IMetaEntity, new()
        {
            var info = GetCollection<T>().FindOneById(id);
            if (info == null)
            {
                throw new FileNotFoundException(string.Format("标识为{0}文件没有找到。", id));
            }
            return info;
        }

        private MongoGridFSFileInfo GetMongoGridFsFileInfo<T>(T entity) where T : IMetaEntity
        {
            var fs = GetDataBase(entity.Database).GridFS.FindOneById(entity.Id);
            if (fs == null)
            {
                throw new MongoException(string.Format("标识为{0}文件在数据库{1}中没有找到。", entity.Id, entity.Database));
            }
            return fs;
        }
        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
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
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <param name="saveFile">保存文件到指定目录</param>
        public void DownLoad(IDictionary<string, object> condition, string saveFile)
        {
            throw new InstrumentationException("此方法在Doc元数据存储中无法实现，请使用泛型方法");
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <param name="saveFile">保存文件到指定目录</param>

        public void DownLoad<T>(IDictionary<string, object> condition, string saveFile) where T : IMetaEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                throw new InstrumentationException("条件condition不能为null或键数据为0。");
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
                throw new MongoException("文件不存在。");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, saveFile);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <returns>返回下载流数据</returns>
        public Stream DownLoad<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                throw new InstrumentationException("条件condition不能为null或键数据为0。");
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
                throw new MongoException("文件不存在。");
            }

            var stream = new MemoryStream();
            DataBase.GridFS.Download(stream, info);

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// 查询字典数据转换程序
        /// 核心加入前缀和替换Id为_Id
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> DictionaryWarp(IEnumerable<KeyValuePair<string, object>> dic)
        {
            return dic.ToDictionary(kv => (kv.Key == "Id" ? "_id" : kv.Key), kv => kv.Value);
        }
    }
}