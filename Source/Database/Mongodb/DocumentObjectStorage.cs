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
    /// Mongodb����洢
    /// </summary>
    public sealed class DocumentObjectStorage : ObjectStorageBase, IObjectStorage
    {
        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="configName">������������</param>
        public DocumentObjectStorage(string configName)
            : base(configName)
        {

        }
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="entity">�������ĵ�����</param>
        /// <typeparam name="T">�ɽ����ĵ���������</typeparam>
        /// <returns>����ɹ�MongoDB���ɵ�ObjectId</returns>
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
        /// ͨ��ObjectIdɾ������
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">��ɾ���ĵ���������</typeparam>
        /// <returns>true�ɹ�,falseʧ��</returns>
        public bool Delete<T>(string id) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var result = GetCollection<T>().Remove(Query.EQ("_id", new ObjectId(id)));
                return ParseWriteConcernResult(result);
            });
        }

        /// <summary>
        /// ͨ������ɾ������
        /// </summary>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <param name="queryDocument">��ѯ����</param>
        /// <returns>true�ɹ�,falseʧ��</returns>
        private bool Delete<T>(QueryDocument queryDocument) where T : IDocumentEntity, new()
        {
            var result = GetCollection<T>().Remove(queryDocument);
            return ParseWriteConcernResult(result);
        }
        
        /// <summary>
        /// ͨ���ֵ�����ɾ������
        /// </summary>
        /// <param name="condition">key-value�ֵ�����</param>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <returns>true�ɹ�,falseʧ��</returns>
        public bool Delete<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            if (condition == null || condition.Count == 0)
            {
                return true;
            }

            return Excute(() => Delete<T>(new QueryDocument(condition)));
        }

        /// <summary>
        /// ��������ɾ������
        /// </summary>
        /// <param name="where">����</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>ɾ���ɹ�����true,����false</returns>
        public bool Delete<T>(Expression<Func<T, bool>> @where) where T : IDocumentEntity, new()
        {
            if (@where == null)
            {
                return true;
            }

            return Excute(() => ParseWriteConcernResult(GetCollection<T>().Remove(Query<T>.Where(@where))));
        }

        /// <summary>
        /// �����ĵ�����
        /// </summary>
        /// <param name="entity">�����¶���</param>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <returns>true�ɹ�,falseʧ��</returns>
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
        /// ͨ��ObjectId���ҵ�������
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <typeparam name="T">�ĵ�����</typeparam>
        /// <returns>���ҵ��Ķ���</returns>
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
        /// ͨ��������ѯ��������
        /// </summary>
        /// <param name="condition">key-value����</param>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <returns>���ҵ��Ķ���</returns>
        /// <exception cref="ArgumentNullException">condition����Ϊ���쳣</exception>
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
        /// ����������ѯ������¼
        /// </summary>
        /// <param name="where">����</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>�������Ϊnull��ֱ�ӷ���null,�������ز�ѯ���</returns>
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
        /// �Ƿ����������������
        /// </summary>
        /// <param name="condition">key-value����</param>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <returns>true�������������Ķ���false�򲻰���</returns>
        /// <exception cref="ArgumentNullException">�������쳣</exception>
        public bool Any<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            return Excute(() => GetCollection<T>().Find(new QueryDocument(condition)).AsQueryable().Any());
        }

        /// <summary>
        /// �Ƿ����������������
        /// </summary>
        /// <param name="where">��������</param>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <returns>true�������������Ķ���false�򲻰���</returns>
        public bool Any<T>(Expression<Func<T, bool>> where) where T : IDocumentEntity, new()
        {
            return Excute(() =>where == null ?  GetCollection<T>().AsQueryable().Any():GetCollection<T>().AsQueryable().Any(where));
        }

        /// <summary>
        /// �������з��������Ķ��󼯺�
        /// </summary>
        /// <param name="condition">key-value����</param>
        /// <param name="sortList">���������</param>
        /// <typeparam name="T">�ĵ���������</typeparam>
        /// <returns>���ҵ��ļ���</returns>
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
        /// �������з��������Ķ��󼯺�
        /// </summary>
        /// <param name="where">��ķ����ʽ����</param>
        /// <param name="orderby">��ķ����ʽ������</param>
        /// <param name="isAsc">true��ʾ����false��ʾ����</param>
        /// <typeparam name="T">�ĵ�����</typeparam>
        /// <typeparam name="TOrderBy">������</typeparam>
        /// <returns>���ҵ��ļ���</returns>
        public List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
                {
                    var query = GetCollection<T>().AsQueryable();
                    return query.Query(where, orderby, p => p, isAsc);
                });
        }


      
        /// <summary>
        /// ͨ����ҳ��ѯ���������ļ���
        /// </summary>
        /// <param name="index">��ǰҳ��</param>
        /// <param name="pageSize">ÿҳ��С</param>
        /// <param name="where">��ķ����ʽ����</param>
        /// <param name="orderby">��ķ����ʽ������</param>
        /// <param name="isAsc">true��ʾ����false��ʾ����</param>
        /// <typeparam name="T">�ĵ�����</typeparam>
        /// <typeparam name="TOrderBy">������</typeparam>
        /// <returns>���ط�ҳ����</returns>
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
        /// ɾ�����ж�������
        /// </summary>
        /// <typeparam name="T">�ĵ�����</typeparam>
        /// <returns>true��ʾ�ɹ���flase��ʾʧ��</returns>
        public bool DeleteAll<T>() where T : IDocumentEntity, new()
        {
            return Excute(() =>
                {
                    var result = GetCollection<T>().RemoveAll();
                    return ParseWriteConcernResult(result);
                });
        }


        /// <summary>
        /// ��ѯ���������Ķ��󼯺�����
        /// </summary>
        /// <param name="condition">key-value����</param>
        /// <typeparam name="T">�ĵ�����</typeparam>
        /// <returns>����������������</returns>
        public long Count<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new()
        {
            return Excute(() => condition == null || condition.Count == 0 ? GetCollection<T>().Count() : GetCollection<T>().Count(new QueryDocument(condition)));
        }

        /// <summary>
        /// ͳ������
        /// </summary>
        /// <param name="where">����</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>��������</returns>
        public long Count<T>(Expression<Func<T, bool>> @where) where T : IDocumentEntity, new()
        {
            if (@where == null)
            {
                return 0;
            }

            return Excute(() => GetCollection<T>().Count(Query<T>.Where(@where)));
        }

        /// <summary>
        /// �������ö���
        /// </summary>
        /// <param name="dbRef">���ö���</param>
        /// <typeparam name="T">���ö�������</typeparam>
        /// <returns>���ؼ��ص����ö���</returns>
        public T LoadRef<T>(MongoDBRef dbRef) where T : IDocumentEntity, new()
        {
            if (dbRef == null)
            {
                return default(T);
            }

            return Excute(() => DataBase.FetchDBRefAs<T>(dbRef));
        }

        /// <summary>
        /// ��ѯָ����������������
        /// </summary>
        /// <param name="condition">�ֵ�����</param>
        /// <param name="sortList">��������</param>
        /// <param name="selectorFunc">ѡ����</param>
        /// <typeparam name="T">������</typeparam>
        /// <returns>���ز�ѯ������</returns>
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
        /// ��ѯָ����������������
        /// </summary>
        /// <param name="where">����</param>
        /// <param name="orderby">��������</param>
        /// <param name="isAsc">����</param>
        /// <param name="selectorFunc">ѡ��������</param>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="TOrderBy">��������</typeparam>
        /// <returns>���ز�ѯ������</returns>
        public List<object> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> @where, Expression<Func<T, TOrderBy>> @orderby, bool isAsc, Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query( where, orderby, selectorFunc, isAsc);
            });
        }

        /// <summary>
        /// ��ҳ��ѯ
        /// </summary>
        /// <param name="index">ҳ����</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <param name="where">����</param>
        /// <param name="orderby">��������</param>
        /// <param name="isAsc">����</param>
        /// <param name="selectorFunc">ѡ��������</param>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="TOrderBy">��������</typeparam>
        /// <returns>���ط�ҳ���ݶ���</returns>
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
        /// ��ѯ
        /// </summary>
        /// <param name="condition">����</param>
        /// <param name="sortList">����</param>
        /// <param name="selectorFunc">��ѡ����</param>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="TReturn">��������</typeparam>
        /// <returns>���ز�ѯ���</returns>
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
        /// ��ѯ�б�
        /// </summary>
        /// <param name="where">����</param>
        /// <param name="orderby">���򱸼�</param>
        /// <param name="isAsc">����</param>
        /// <param name="selectorFunc">��ѡ����</param>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="TOrderBy">��������</typeparam>
        /// <typeparam name="TReturn">��������</typeparam>
        /// <returns>��ѯ���</returns>
        public List<TReturn> FindByQuery<T, TOrderBy, TReturn>(Expression<Func<T, bool>> @where, Expression<Func<T, TOrderBy>> @orderby, bool isAsc, Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new()
        {
            return Excute(() =>
            {
                var query = GetCollection<T>().AsQueryable();
                return query.Query(where, orderby, selectorFunc, isAsc);
            });
        }

        /// <summary>
        /// ��ѯ
        /// </summary>
        /// <param name="index">����</param>
        /// <param name="pageSize">��ҳ��С</param>
        /// <param name="where">����</param>
        /// <param name="orderby">���򱸼�</param>
        /// <param name="isAsc">����</param>
        /// <param name="selectorFunc">��ѡ����</param>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="TOrderBy">��������</typeparam>
        /// <typeparam name="TReturn">��������</typeparam>
        /// <returns>��ѯ���</returns>
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
        /// �߼���ѯ
        /// </summary>
        /// <param name="query">��ѯִ��Action</param>
        /// <typeparam name="T">���ʱ�����</typeparam>
        public void AdvanceQuery<T>(Action<IQueryable<T>> query) where T : IDocumentEntity, new()
        {
            Excute(() =>
            {
                var queryable = GetCollection<T>().AsQueryable();
                query(queryable);
            });
        }
        /// <summary>
        /// ����Queryable����
        /// </summary>
        /// <typeparam name="T">�洢��������</typeparam>
        /// <returns>���ش�������</returns>
        public IQueryable<T> CreateQueryable<T>() where T : IDocumentEntity, new()
        {
            return GetCollection<T>().AsQueryable();
        }
    }
}