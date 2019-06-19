using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 分库文档存储实现
    /// </summary>
    public class ShardingFileDocStorage : ObjectStorageBase, IFileStorage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configName">配置文件</param>
        public ShardingFileDocStorage(string configName)
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
                var id = GetGridFS(entity.Database).UploadFromStream(entity.FileName, context);
                var fsInfo = CheckGridFsFileExists(GetGridFS(entity.Database), id);
                entity.Id = id;
                entity.HashCode = fsInfo.MD5;
                context.Dispose();
                context = null;
                GetCollection<T>().InsertOne(entity);
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
            var info = CheckFileExists<T>(entity.Id);

            info.Fill(entity);

            var result = GetCollection<T>().ReplaceOne(Builders<T>.Filter.Where(p => p.Id == entity.Id), info);
            if (result.IsAcknowledged && result.ModifiedCount == 1)
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
            var info = CheckFileExists<T>(id);
            var result = GetCollection<T>().DeleteOne(Builders<T>.Filter.Where(p => p.Id == id));
            if (result.IsAcknowledged && result.DeletedCount == 0)
            {
                GetGridFS(info.Database).Delete(id);
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
                var query = ConvertDictionaryToFilterDefinition<T>(DictionaryWarp(condition));
                var list = GetCollection<T>().Find(query).Project(p => new { p.Id, p.Database }).ToList();
                if (list.Count > 0)
                {
                    GetCollection<T>().DeleteMany(ConvertDictionaryToFilterDefinition<T>(DictionaryWarp(condition)));
                    list.ForEach(p => GetGridFS(p.Database).Delete(p.Id));
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
            return GetCollection<T>().Find(Builders<T>.Filter.Where(p => p.Id == id)).FirstOrDefault();
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
                var query = ConvertDictionaryToFilterDefinition<T>(DictionaryWarp(condition));
                return GetCollection<T>().Find(query).FirstOrDefault();
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
                var query = ConvertDictionaryToFilterDefinition<T>(DictionaryWarp(condition));
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
                var query = GetCollection<T>().Find(ConvertDictionaryToFilterDefinition<T>(dic));
                var sort = ConvertDictionaryToSortDefinition<T>(sortList);
                if (sort != null)
                {
                    query = query.Sort(sort);
                }
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
        /// 此实现只能查询未分库的文件
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <returns>返回流</returns>
        [Obsolete("当前方法只适用于查询当前库文件下载，如果文件使用了归档功能，会出现文件不存在异常。")]
        public Stream DownLoad(ObjectId id)
        {
            GridFSFileInfo info = CheckGridFsFileExists(GridFS, id);

            var stream = new MemoryStream();
            GridFS.DownloadToStream(id, stream);

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
                var list = GetCollection<T>().AsQueryable().Select(p => new { p.Id, p.Database }).ToList();
                list.ForEach(p =>
                {
                    var result = GetCollection<T>().DeleteOne(Builders<T>.Filter.Eq("_id", p.Id));
                    if (result.IsAcknowledged && result.DeletedCount == 1)
                    {
                        GetGridFS(p.Database).Delete(p.Id);
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
        [Obsolete("当前方法只适用于查询当前库文件下载，如果文件使用了归档功能，会出现文件不存在异常。")]
        public void DownLoad(ObjectId id, string saveFile)
        {

            GridFSFileInfo info = CheckGridFsFileExists(GridFS, id);
            using (var stream = new FileStream(saveFile, FileMode.OpenOrCreate))
                GridFS.DownloadToStream(id, stream);
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
        /// <param name="saveFile">保存文件到指定目录</param>

        public void DownLoad<T>(IDictionary<string, object> condition, string saveFile) where T : IMetaEntity, new()
        {
            using (var stream = new FileStream(saveFile, FileMode.OpenOrCreate))
                DownLoad<T>(condition, stream);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <returns>返回下载流数据</returns>
        public Stream DownLoad<T>(IDictionary<string, object> condition) where T : IMetaEntity, new()
        {

            var stream = new MemoryStream();
            DownLoad<T>(condition, stream);

            stream.Position = 0;
            return stream;
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="condition">下载字典条件</param>
        /// <returns>返回下载流数据</returns>
        void DownLoad<T>(IDictionary<string, object> condition, Stream stream) where T : IMetaEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                throw new InfrastructureException("条件condition不能为null或键数据为0。");
            }

            var query = ConvertDictionaryToFilterDefinition<T>(DictionaryWarp(condition));
            var entity = GetCollection<T>().Find(query).FirstOrDefault();
            if (entity == null)
            {
                throw new FileNotFoundException();
            }
            var gridfs = GetGridFSForEntity(entity);
            GridFSFileInfo info = CheckGridFsFileExists(gridfs, entity.Id);


            gridfs.DownloadToStream(info.Id, stream);
        }

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
        public Stream DownLoad<T>(ObjectId id) where T : IMetaEntity, new()
        {
            var info = CheckFileExists<T>(id);
            var gridfs = GetGridFSForEntity(info);
            var fs = CheckGridFsFileExists(gridfs, id);
            var stream = new MemoryStream();
            gridfs.DownloadToStream(fs.Id, stream);
            stream.Position = 0;
            return stream;
        }


        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <param name="saveFile">保存文件路径</param>
        public void DownLoad<T>(ObjectId id, string saveFile) where T : IMetaEntity, new()
        {
            var info = CheckFileExists<T>(id);
            var gridfs = GetGridFSForEntity(info);
            CheckGridFsFileExists(gridfs, info.Id);
            using (var stream = new FileStream(saveFile, FileMode.OpenOrCreate))
                gridfs.DownloadToStream(id, stream);
        }

        private GridFSBucket GetGridFSForEntity<T>(T entity) where T : IMetaEntity, new()
        {
            return string.IsNullOrEmpty(entity.Database) ? GridFS : GetGridFS(entity.Database);
        }
        private GridFSFileInfo CheckGridFsFileExists(GridFSBucket gridFS, ObjectId id)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Where(p => p.Id == id);
            var fs = gridFS.Find(filter).FirstOrDefault();
            if (fs == null)
            {
                throw new MongoException(string.Format("标识为{0}文件在数据库{1}中没有找到。", id, gridFS.Database.DatabaseNamespace.DatabaseName));
            }
            return fs;
        }
        private T CheckFileExists<T>(ObjectId id) where T : IMetaEntity, new()
        {
            var filter = Builders<T>.Filter.Where(p => p.Id == id);
            var fs = GetCollection<T>().Find(filter).FirstOrDefault();
            if (fs == null)
            {
                throw new MongoException(string.Format("标识为{0}文件在数据库中没有找到。", id));
            }
            return fs;
        }
        private void EnsureIdNotNull(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
        }
        public T FindById<T>(string id) where T : IMetaEntity, new()
        {
            EnsureIdNotNull(id);
            return FindById<T>(new ObjectId(id));
        }

        public bool Any<T>(Expression<Func<T, bool>> where) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                return GetCollection<T>().AsQueryable().Where(where).Any();
            });
        }

        public List<T> FindByQuery<T>(Expression<Func<T, bool>> where) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                return GetCollection<T>().AsQueryable().Where(where).Select(p => p).ToList();
            });
        }

        public void DownLoad<T>(Expression<Func<T, bool>> where, string saveFile) where T : IMetaEntity, new()
        {
            Excute(() =>
            {
                var entity = GetCollection<T>().AsQueryable().Where(where).Select(p => p).FirstOrDefault();
                if (entity == null)
                {
                    throw new FileNotFoundException("文件不存在。");
                }
                using (var stream = new FileStream(saveFile, FileMode.OpenOrCreate))
                {
                    var gridfs = GetGridFSForEntity(entity);
                    GridFSFileInfo info = CheckGridFsFileExists(gridfs, entity.Id);

                    gridfs.DownloadToStream(info.Id, stream);
                }
            });
        }

        public Stream DownLoad<T>(Expression<Func<T, bool>> where) where T : IMetaEntity, new()
        {
            return Excute(() =>
            {
                var entity = GetCollection<T>().AsQueryable().Where(where).Select(p => p).FirstOrDefault();
                if (entity == null)
                {
                    throw new FileNotFoundException("文件不存在。");
                }
                var stream = new MemoryStream();

                var gridfs = GetGridFSForEntity(entity);
                GridFSFileInfo info = CheckGridFsFileExists(gridfs, entity.Id);

                gridfs.DownloadToStream(info.Id, stream);
                stream.Position = 0;
                return stream;

            });
        }

        public Stream DownLoad<T>(string id) where T : IMetaEntity, new()
        {
            EnsureIdNotNull(id);
            return DownLoad<T>(new ObjectId(id));
        }

        public void DownLoad<T>(string id, string saveFile) where T : IMetaEntity, new()
        {
            EnsureIdNotNull(id);
            DownLoad<T>(new ObjectId(id), saveFile);
        }

        [Obsolete("当前方法只适用于查询当前库文件下载，如果文件使用了归档功能，会出现文件不存在异常。")]
        public Stream DownLoad(string id)
        {
            EnsureIdNotNull(id);
            return DownLoad(new ObjectId(id));
        }
    }
}
