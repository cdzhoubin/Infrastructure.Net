using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// Mongodb对象存储
    /// </summary>
    public sealed class DocumentObjectStorage : ObjectStorageBase, IObjectStorage
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configName">程序集配置名称</param>
        public DocumentObjectStorage(string configName)
            : base(configName)
        {

        }
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="entity">待插入文档对象</param>
        /// <typeparam name="T">可接受文档对象类型</typeparam>
        /// <returns>插入成功MongoDB生成的ObjectId</returns>
        public string Insert<T>(T entity) where T : IDocumentEntity, new()
        {
            return Excute(() =>
                {
                    GetCollection<T>().Insert(entity);
// ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    return entity.Id.ToString();
                });
        }

        /// <summary>
        /// 通过ObjectId删除对象
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">待删除文档对象类型</typeparam>
        /// <returns>true成功,false失败</returns>
        public bool Delete<T>(string id) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var result = GetCollection<T>().Remove(Query.EQ("_id", new ObjectId(id)));
                return ParseWriteConcernResult(result);
            });
        }

        /// <summary>
        /// 通过条件删除对象
        /// </summary>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <param name="queryDocument">查询条件</param>
        /// <returns>true成功,false失败</returns>
        private bool Delete<T>(QueryDocument queryDocument) where T : IDocumentEntity, new()
        {
            var result = GetCollection<T>().Remove(queryDocument);
            return ParseWriteConcernResult(result);
        }
        
        /// <summary>
        /// 通过字典条件删除对象
        /// </summary>
        /// <param name="condition">key-value字典条件</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>true成功,false失败</returns>
        public bool Delete<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                return true;
            }

            return Excute(() => Delete<T>(new QueryDocument(condition)));
        }

        /// <summary>
        /// 根据条件删除对象
        /// </summary>
        /// <param name="where">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>删除成功返回true,其他false</returns>
        public bool Delete<T>(Expression<Func<T, bool>> @where) where T : IDocumentEntity, new()
        {
            if (@where == null)
            {
                return true;
            }

            return Excute(() => ParseWriteConcernResult(GetCollection<T>().Remove(Query<T>.Where(@where))));
        }

        /// <summary>
        /// 更新文档对象
        /// </summary>
        /// <param name="entity">待更新对象</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>true成功,false失败</returns>
        public bool Update<T>(T entity) where T : IDocumentEntity, new()
        {
            return Excute(() =>
                  {
                      var p1 = GetCollection<T>().FindOneById(entity.Id);
                      // ReSharper disable CompareNonConstrainedGenericWithNull
                      if (null == p1)
                      // ReSharper restore CompareNonConstrainedGenericWithNull
                      {
                          return false;
                      }

                      p1.Fill(entity);
                      var result = GetCollection<T>().Save(p1);
                      return ParseWriteConcernResult(result);
                  });
        }

        /// <summary>
        /// 通过ObjectId查找单个对象
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">文档类型</typeparam>
        /// <returns>查找到的对象</returns>
        public T FindById<T>(string id) where T : IDocumentEntity, new()
        {
            var result = Excute(() => GetCollection<T>().FindOneById(new ObjectId(id)));
            if (AutoLoadNavProperty)
            {
                LoadNavProperty(result);
            }

            return result;
        }

        /// <summary>
        /// 通过条件查询单个对象
        /// </summary>
        /// <param name="condition">key-value条件</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>查找到的对象</returns>
        /// <exception cref="ArgumentNullException">condition条件为空异常</exception>
        public T FindOneByCondition<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            var result = Excute(() => GetCollection<T>().FindOne(new QueryDocument(condition)));
            if (AutoLoadNavProperty)
            {
                LoadNavProperty(result);
            }

            return result;
        }

        /// <summary>
        /// 根据条件查询单条记录
        /// </summary>
        /// <param name="where">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>如果条件为null，直接返回null,其他返回查询结果</returns>
        public T FindOneByCondition<T>(Expression<Func<T, bool>> @where) where T : IDocumentEntity, new()
        {
            if (@where == null)
            {
                return default(T);
            }

            var result = Excute(() => GetCollection<T>().FindOne(Query<T>.Where(@where)));
            if (AutoLoadNavProperty)
            {
                LoadNavProperty(result);
            }

            return result;
        }

        /// <summary>
        /// 是否包含符合条件对象
        /// </summary>
        /// <param name="condition">key-value对象</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>true包含符合条件的对象，false则不包含</returns>
        /// <exception cref="ArgumentNullException">条件空异常</exception>
        public bool Any<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            return Excute(() => GetCollection<T>().Find(new QueryDocument(condition)).AsQueryable().Any());
        }

        /// <summary>
        /// 是否包含符合条件对象
        /// </summary>
        /// <param name="where">过虑条件</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>true包含符合条件的对象，false则不包含</returns>
        public bool Any<T>(Expression<Func<T, bool>> where) where T : IDocumentEntity, new()
        {
            return Excute(() =>where == null ?  GetCollection<T>().AsQueryable().Any():GetCollection<T>().AsQueryable().Any(where));
        }

        /// <summary>
        /// 查找所有符合条件的对象集合
        /// </summary>
        /// <param name="condition">key-value条件</param>
        /// <param name="sortList">排序的属性</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>查找到的集合</returns>
        public List<T> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList) where T : IDocumentEntity, new()
        {
            var result = Excute(() =>
                {
                    var collection = GetCollection<T>();
                    var query = collection.Find(new QueryDocument(condition));
                    sortList.ToList().ForEach(p => query = query.SetSortOrder(new SortByDocument(p.Key,new BsonInt32(p.Value ? 1 : -1))));
                    return query.ToList();
                });
            if (AutoLoadNavProperty)
            {
                LoadNavProperty(result);
            }

            return result;
        }

        /// <summary>
        /// 查找所有符合条件的对象集合
        /// </summary>
        /// <param name="where">拉姆达表达式条件</param>
        /// <param name="orderby">拉姆达表达式排序列</param>
        /// <param name="isAsc">true标示升序，false标示降序</param>
        /// <typeparam name="T">文档类型</typeparam>
        /// <typeparam name="TOrderBy">排序列</typeparam>
        /// <returns>查找到的集合</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
                {
                    var query = GetCollection<T>().AsQueryable();
                    return query.Query(where, orderby, p => p, isAsc);
                });
        }


      
        /// <summary>
        /// 通过分页查询符合条件的集合
        /// </summary>
        /// <param name="index">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">拉姆达表达式条件</param>
        /// <param name="orderby">拉姆达表达式排序列</param>
        /// <param name="isAsc">true表示升序，false表示降序</param>
        /// <typeparam name="T">文档类型</typeparam>
        /// <typeparam name="TOrderBy">排序列</typeparam>
        /// <returns>返回分页数据</returns>
        public PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
                {
                    var query = GetCollection<T>().AsQueryable();
                    Func<IQueryable<T>, List<T>> selector = p => p.ToList();
                    return query.Query(index, pageSize, where, orderby, selector, isAsc);
                });
        }


        /// <summary>
        /// 删除所有对象数据
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <returns>true表示成功，flase表示失败</returns>
        public bool DeleteAll<T>() where T : IDocumentEntity, new()
        {
            return Excute(() =>
                {
                    var result = GetCollection<T>().RemoveAll();
                    return ParseWriteConcernResult(result);
                });
        }


        /// <summary>
        /// 查询符合条件的对象集合总数
        /// </summary>
        /// <param name="condition">key-value条件</param>
        /// <typeparam name="T">文档对象</typeparam>
        /// <returns>符合条件的总条数</returns>
        public long Count<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            return Excute(() => condition == null || condition.Count == 0 ? GetCollection<T>().Count() : GetCollection<T>().Count(new QueryDocument(condition)));
        }

        /// <summary>
        /// 统计数量
        /// </summary>
        /// <param name="where">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回数量</returns>
        public long Count<T>(Expression<Func<T, bool>> @where) where T : IDocumentEntity, new()
        {
            if (@where == null)
            {
                return 0;
            }

            return Excute(() => GetCollection<T>().Count(Query<T>.Where(@where)));
        }

        /// <summary>
        /// 加载引用对象
        /// </summary>
        /// <param name="dbRef">引用对象</param>
        /// <typeparam name="T">引用对象类型</typeparam>
        /// <returns>返回加载的引用对象</returns>
        public T LoadRef<T>(MongoDBRef dbRef) where T : IDocumentEntity, new()
        {
            if (dbRef == null)
            {
                return default(T);
            }

            return Excute(() => DataBase.FetchDBRefAs<T>(dbRef));
        }

        /// <summary>
        /// 查询指定条件的所有数据
        /// </summary>
        /// <param name="condition">字典条件</param>
        /// <param name="sortList">排序条件</param>
        /// <param name="selectorFunc">选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询到数据</returns>
        public List<object> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList, Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var collection = GetCollection<T>();
                var query = collection.Find(new QueryDocument(condition));
                sortList.ToList().ForEach(p => query = query.SetSortOrder(new SortByDocument(p.Key,new BsonInt32(p.Value ? 1 : -1))));
               
                return selectorFunc(query.AsQueryable());
            });
        }

        /// <summary>
        /// 查询指定条件的所有数据
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">升序</param>
        /// <param name="selectorFunc">选择数据器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回查询到数据</returns>
        public List<object> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> @where, Expression<Func<T, TOrderBy>> @orderby, bool isAsc, Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query( where, orderby, selectorFunc, isAsc);
            });
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="index">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">升序</param>
        /// <param name="selectorFunc">选择数据器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回分页数据对象</returns>
        public PageInfo<object> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TOrderBy>> @orderby, bool isAsc,
            Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(index, pageSize, where, orderby, selectorFunc, isAsc);
            });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="sortList">排序</param>
        /// <param name="selectorFunc">列选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <returns>返回查询结果</returns>
        public List<TReturn> FindByCondition<T, TReturn>(IDictionary<string, object> condition, IDictionary<string, bool> sortList, Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var collection = GetCollection<T>();
                var query = collection.Find(new QueryDocument(condition));
                sortList.ToList().ForEach(p => query = query.SetSortOrder(new SortByDocument(p.Key, new BsonInt32(p.Value ? 1 : -1))));

                return selectorFunc(query.AsQueryable());
            });
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序备件</param>
        /// <param name="isAsc">升序</param>
        /// <param name="selectorFunc">列选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <returns>查询结果</returns>
        public List<TReturn> FindByQuery<T, TOrderBy, TReturn>(Expression<Func<T, bool>> @where, Expression<Func<T, TOrderBy>> @orderby, bool isAsc, Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(where, orderby, selectorFunc, isAsc);
            });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序备件</param>
        /// <param name="isAsc">升序</param>
        /// <param name="selectorFunc">列选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <returns>查询结果</returns>
        public PageInfo<TReturn> FindByQuery<T, TOrderBy, TReturn>(int index, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TOrderBy>> @orderby, bool isAsc,
            Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(index, pageSize, where, orderby, selectorFunc, isAsc);
            });
        }


        //public List<TResult> GroupBy<T, TGroupBy, TResult>(Expression<Func<T, bool>> where, Expression<Func<T, TGroupBy>> grouprby, Expression<Func<IGrouping<TGroupBy, T>, TResult>> selector)
        //where T : IDocumentEntity, new()
        //{
        //    return Excute(() =>
        //    {
        //        var query = GetCollection<T>().AsQueryable();
        //        if (where != null)
        //        {
        //            query = query.Where(where);
        //        }

        //        return query.GroupBy(grouprby).Select(selector).ToList();
        //    });
        //}

        //public List<object> GroupBy<T, TGroupBy>(Expression<Func<T, bool>> where, Expression<Func<T, TGroupBy>> grouprby, Expression<Func<IGrouping<TGroupBy, T>,object>> selector)
        //where T : IDocumentEntity, new()
        //{
        //    return Excute(() =>
        //    {
        //        var query = GetCollection<T>().AsQueryable();
        //        if (where != null)
        //        {
        //            query = query.Where(where);
        //        }

        //        return query.GroupBy(grouprby).Select(selector).ToList();
        //    });
        //}


        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="query">查询执行Action</param>
        /// <typeparam name="T">访问表类型</typeparam>
        public void AdvanceQuery<T>(Action<IQueryable<T>> query) where T : IDocumentEntity, new()
        {
            Excute(() =>
            {
                var queryable = GetCollection<T>().AsQueryable();
                query(queryable);
            });
        }
        /// <summary>
        /// 创建Queryable对象
        /// </summary>
        /// <typeparam name="T">存储数据类型</typeparam>
        /// <returns>返回创建对象</returns>
        public IQueryable<T> CreateQueryable<T>() where T : IDocumentEntity, new()
        {
            return GetCollection<T>().AsQueryable();
        }
    }
}