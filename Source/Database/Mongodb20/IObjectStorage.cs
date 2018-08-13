using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 对象存储接口
    /// </summary>
    public interface IObjectStorage:IDisposable
    {
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity">待插入对象</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回对象标识</returns>
        string Insert<T>(T entity) where T : IDocumentEntity, new();
        
        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity">待更新对象</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>更新成功返回true,其它false</returns>
        bool Update<T>(T entity) where T : IDocumentEntity, new();
        
        /// <summary>
        /// 删除指定记录
        /// </summary>
        /// <param name="id">标识</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>成功返回true,其它false</returns>
        bool Delete<T>(string id) where T : IDocumentEntity, new();
        /// <summary>
        /// 删除指定条件记录
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>成功返回true,其它false</returns>
        bool Delete<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new();

        /// <summary>
        /// 删除指定条件记录
        /// </summary>
        /// <param name="where">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>成功返回true,其它false</returns>
        bool Delete<T>(Expression<Func<T, bool>> where) where T : IDocumentEntity, new();
        /// <summary>
        /// 删除所有记录
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>成功返回true,其它false</returns>
        bool DeleteAll<T>() where T:IDocumentEntity,new ();

        /// <summary>
        /// 根据Id查找实体
        /// </summary>
        /// <param name="id">键</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询到的实体</returns>
        T FindById<T>(string id) where T : IDocumentEntity, new();
        /// <summary>
        /// 根据条件查询实体
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询到的实体</returns>
        T FindOneByCondition<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new();

        /// <summary>
        /// 查询指定条件记录
        /// </summary>
        /// <param name="where">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询到的实体</returns>
        T FindOneByCondition<T>(Expression<Func<T, bool>> where) where T : IDocumentEntity, new();

        /// <summary>
        /// 检查指定条件的对象是否存在
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>true包含符合条件的对象，false则不包含</returns>
        bool Any<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new();
        /// <summary>
        /// 是否包含符合条件对象
        /// </summary>
        /// <param name="where">过虑条件</param>
        /// <typeparam name="T">文档对象类型</typeparam>
        /// <returns>true包含符合条件的对象，false则不包含</returns>
        bool Any<T>(Expression<Func<T, bool>> where) where T : IDocumentEntity, new();
        /// <summary>
        /// 查询指定条件的对象
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="sortList">排序条件，如果为null忽略</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询结果</returns>
        List<T> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList) where T : IDocumentEntity, new();

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">升序</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>查询结果</returns>
        List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby,
                                         bool isAsc) where T : IDocumentEntity, new();
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">升序</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>查询结果</returns>
        PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where, 
                                             Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IDocumentEntity, new();
        /// <summary>
        /// 统计数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回统计数量</returns>
        long Count<T>(IDictionary<string, object> condition) where T : IDocumentEntity, new();

        /// <summary>
        /// 查询指定条件记录数量
        /// </summary>
        /// <param name="where">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回统计数量</returns>
        long Count<T>(Expression<Func<T, bool>> where) where T : IDocumentEntity, new();

        /// <summary>
        /// 加载引用对象
        /// </summary>
        /// <param name="dbRef">引用对象</param>
        /// <typeparam name="T">引用对象类型</typeparam>
        /// <returns>返回导航属性对象</returns>
        T LoadRef<T>(MongoDBRef dbRef) where T : IDocumentEntity, new();

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="sortList">排序</param>
        /// <param name="selectorFunc">列选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询结果</returns>
        List<object> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList, Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new();

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序备件</param>
        /// <param name="isAsc">升序</param>
        /// <param name="selectorFunc">列选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>查询结果</returns>
        List<object> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby,
                                         bool isAsc, Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new();

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
        /// <returns>查询结果</returns>
        PageInfo<object> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where,
                                             Expression<Func<T, TOrderBy>> orderby, bool isAsc, Func<IQueryable<T>, List<object>> selectorFunc) where T : IDocumentEntity, new();

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="sortList">排序</param>
        /// <param name="selectorFunc">列选择器</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <returns>返回查询结果</returns>
        List<TReturn> FindByCondition<T,TReturn>(IDictionary<string, object> condition, IDictionary<string, bool> sortList, Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new();

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
        List<TReturn> FindByQuery<T, TOrderBy, TReturn>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby,
                                         bool isAsc, Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new();

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
        PageInfo<TReturn> FindByQuery<T, TOrderBy, TReturn>(int index, int pageSize, Expression<Func<T, bool>> where,
                                             Expression<Func<T, TOrderBy>> orderby, bool isAsc, Func<IQueryable<T>, List<TReturn>> selectorFunc) where T : IDocumentEntity, new();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="where"></param>
        ///// <param name="grouprby"></param>
        ///// <param name="selector"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TGroupBy"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        //List<TResult> GroupBy<T, TGroupBy, TResult>(Expression<Func<T, bool>> where,
        //                                     Expression<Func<T, TGroupBy>> grouprby, Expression<Func<IGrouping<TGroupBy, T>, TResult>> selector) where T : IDocumentEntity, new();
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="where"></param>
        ///// <param name="grouprby"></param>
        ///// <param name="selector"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TGroupBy"></typeparam>
        ///// <returns></returns>
        //List<object> GroupBy<T, TGroupBy>(Expression<Func<T, bool>> where,
        //                                     Expression<Func<T, TGroupBy>> grouprby, Expression<Func<IGrouping<TGroupBy, T>, object>> selector) where T : IDocumentEntity, new();

        /// <summary>
        /// 高级查询方法
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <typeparam name="T">表类型</typeparam>
        void AdvanceQuery<T>(Action<IQueryable<T>> query) where T : IDocumentEntity, new();
        /// <summary>
        /// 创建Queryable对象
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns></returns>
        IQueryable<T> CreateQueryable<T>() where T : IDocumentEntity, new();

        void Advance<T>(Action<IMongoCollection<T>> action) where T : IDocumentEntity, new();

    }
}