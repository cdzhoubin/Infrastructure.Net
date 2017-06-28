using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Zhoubin.Infrastructure.Common.Extent
{
    /// <summary>
    /// Linq扩展方法
    /// </summary>
    public static class LinqExtent
    {
        /// <summary>
        /// 查询扩展方法一
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="where">过滤条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <param name="isAsc">升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实例类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <typeparam name="TResult">返回结果集类型</typeparam>
        /// <returns>返回列表对象</returns>
        ///  <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<TResult> Query<TEntity, TOrderBy, TResult>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Expression<Func<TEntity, TResult>> selector,
            bool isAsc)
        {
            return Query(query,new List<Expression<Func<TEntity, bool>>>{where}, p => isAsc ? p.OrderBy(orderby) : p.OrderByDescending(orderby), selector);
            }

        /// <summary>
        /// 查询扩展方法一
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="wheres">过滤条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <param name="isAsc">升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实例类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <typeparam name="TResult">返回结果集类型</typeparam>
        /// <returns>返回列表对象</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<TResult> Query<TEntity, TOrderBy, TResult>(this IQueryable<TEntity> query,
            List<Expression<Func<TEntity, bool>>> wheres,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Expression<Func<TEntity, TResult>> selector,
            bool isAsc)
        {

            return Query(query,wheres, p => isAsc ? p.OrderBy(orderby) : p.OrderByDescending(orderby), selector);
            }

        /// <summary>
        /// 查询方法扩展二
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="where">过滤条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <param name="isAsc">升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <returns>返回列表对象</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<object> Query<TEntity, TOrderBy>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<object>> selector,
            bool isAsc)
        {

            return Query(query, where, p => isAsc ? p.OrderBy(orderby) : p.OrderByDescending(orderby), selector);
        }

        /// <summary>
        /// 查询方法扩展二
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="where">过滤条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <param name="isAsc">升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <returns>返回列表对象</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<TReturn> Query<TEntity, TOrderBy,TReturn>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<TReturn>> selector,
            bool isAsc)
        {

            return Query(query, where, p => isAsc ? p.OrderBy(orderby) : p.OrderByDescending(orderby), selector);
        }
        /// <summary>
        /// 查询方法扩展二
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="where">过滤条件</param>
        /// <param name="funOrderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <returns>返回匿名对象List</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<object> Query<TEntity>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> where,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> funOrderby,
            Func<IQueryable<TEntity>, List<object>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (funOrderby == null)
            {
                throw new ArgumentNullException("funOrderby");
            }

            var queryable = query;
            if (where != null)
            {
                queryable = queryable.Where(where);
            }

            queryable = funOrderby(queryable);

            return selector(queryable);
        }

        /// <summary>
        /// 查询方法扩展二
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="where">过滤条件</param>
        /// <param name="funOrderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <returns>返回匿名对象List</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<TReturn> Query<TEntity, TReturn>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> where,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> funOrderby,
            Func<IQueryable<TEntity>, List<TReturn>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (funOrderby == null)
            {
                throw new ArgumentNullException("funOrderby");
            }

            var queryable = query;
            if (where != null)
            {
                queryable = queryable.Where(where);
            }

            queryable = funOrderby(queryable);

            return selector(queryable);
        }

        /// <summary>
        /// 查询扩展方法一
        /// 可以指定条件、排序条件（只支持单个条件），选择指定的列
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="wheres">过滤条件</param>
        /// <param name="funOrderby">排序条件</param>
        /// <param name="selector">列表选择条件</param>
        /// <typeparam name="TEntity">业务实例类型</typeparam>
        /// <typeparam name="TResult">返回结果集类型</typeparam>
        /// <returns>返回对象List</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static List<TResult> Query<TEntity, TResult>(this IQueryable<TEntity> query,
            List<Expression<Func<TEntity, bool>>> wheres,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> funOrderby,
            Expression<Func<TEntity, TResult>> selector)
            {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (funOrderby == null)
            {
                throw new ArgumentNullException("funOrderby");
            }

            var queryable = query;
            if (wheres != null)
            {
                wheres.ForEach(p =>
                {
                    if (p != null)
                    {
                        queryable = queryable.Where(p);
                    }
                });
            }

            queryable = funOrderby(queryable);
            return queryable.Select(selector).ToList();
        }


        /// <summary>
        /// 分页查询方法扩展一
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">过滤表达式</param>
        /// <param name="orderby">排序表达式</param>
        /// <param name="selector">选择表达式</param>
        /// <param name="isAsc">排序方式：升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <returns>返回分页数据</returns>
        public static PageInfo<object> Query<TEntity, TOrderBy>(this IQueryable<TEntity> query, int index, int pageSize,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<object>> selector,
            bool isAsc)
        {
            return Query(query, index, pageSize, where == null ? new List<Expression<Func<TEntity, bool>>>() : new List<Expression<Func<TEntity, bool>>> { where }, orderby, selector,
                isAsc);
        }

        /// <summary>
        /// 分页查询方法扩展一
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">过滤表达式</param>
        /// <param name="orderbys">排序表达式</param>
        /// <param name="selector">选择表达式</param>
        /// <param name="isAscs">排序方式：升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <returns>返回分页数据</returns>
        public static PageInfo<object> Query<TEntity, TOrderBy>(this IQueryable<TEntity> query, int index, int pageSize,
            Expression<Func<TEntity, bool>> where,
            List<Expression<Func<TEntity, TOrderBy>>> orderbys,
            Func<IQueryable<TEntity>, List<object>> selector,
            List<bool> isAscs)
        {
            return Query(query, index, pageSize, where != null ? new List<Expression<Func<TEntity, bool>>> { where } : new List<Expression<Func<TEntity, bool>>>(), orderbys, selector,
                isAscs);
        }

        /// <summary>
        /// 分页查询方法扩展二
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="wheres">过滤表达式列表</param>
        /// <param name="orderby">排序表达式</param>
        /// <param name="selector">选择表达式</param>
        /// <param name="isAsc">排序方式：升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <returns>返回分页数据</returns>
        public static PageInfo<object> Query<TEntity, TOrderBy>(this IQueryable<TEntity> query, int index, int pageSize,
            List<Expression<Func<TEntity, bool>>> wheres,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<object>> selector, bool isAsc)
        {
            return Query<TEntity, TOrderBy, object>(query, index, pageSize, wheres, orderby, selector, isAsc);
        }

        /// <summary>
        /// 分页查询方法扩展二
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="wheres">过滤表达式</param>
        /// <param name="orderby">排序表达式</param>
        /// <param name="selector">选择表达式</param>
        /// <param name="isAsc">排序方式：升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <typeparam name="TResult">结果集类型</typeparam>
        /// <returns>返回分页数据</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static PageInfo<TResult> Query<TEntity, TOrderBy, TResult>(this IQueryable<TEntity> query, int index,
            int pageSize,
            List<Expression<Func<TEntity, bool>>> wheres,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<TResult>> selector, bool isAsc)
        {
            return Query(query, index, pageSize, wheres,
                orderby == null
                    ? new List<Expression<Func<TEntity, TOrderBy>>>()
                    : new List<Expression<Func<TEntity, TOrderBy>>> { orderby }, selector,
                new List<bool> { isAsc });
        }

        /// <summary>
        /// 分页查询方法扩展二
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="wheres">过滤表达式</param>
        /// <param name="orderbys">排序表达式,支持多重排序</param>
        /// <param name="selector">选择表达式</param>
        /// <param name="isAscss">排序方式：升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <typeparam name="TResult">结果集类型</typeparam>
        /// <returns>返回分页数据</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static PageInfo<TResult> Query<TEntity, TOrderBy, TResult>(this IQueryable<TEntity> query, int index,
            int pageSize,
            List<Expression<Func<TEntity, bool>>> wheres,
            List<Expression<Func<TEntity, TOrderBy>>> orderbys,
            Func<IQueryable<TEntity>, List<TResult>> selector, List<bool> isAscss)
        {
            return Query(query, index, pageSize, wheres, p =>
            {
                if (orderbys != null && orderbys.Count > 0)
                {
                    for (int i = 0; i < orderbys.Count; i++)
                    {
                        p = isAscss[i]
                            ? p.OrderBy(orderbys[i])
                            : p.OrderByDescending(orderbys[i]);
                }

            }

                return p;
            }, selector);
        }

        /// <summary>
        /// 分页查询方法扩展二
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">过滤表达式</param>
        /// <param name="orderby">排序表达式</param>
        /// <param name="selector">选择表达式</param>
        /// <param name="isAsc">排序方式：升序为true,降序为false</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TOrderBy">排序条件类型</typeparam>
        /// <typeparam name="TResult">结果集类型</typeparam>
        /// <returns>返回分页数据</returns>
        public static PageInfo<TResult> Query<TEntity, TOrderBy, TResult>(this IQueryable<TEntity> query, int index,
            int pageSize,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<TResult>> selector,
            bool isAsc)
        {
            return Query(query, index, pageSize, new List<Expression<Func<TEntity, bool>>> { where }, orderby, selector,
                isAsc);
        }

        /// <summary>
        /// 分页查询方法扩展二
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <param name="index">索引，最小值为1</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="wheres">过滤表达式</param>
        /// <param name="funOrderby">排序表达式,支持多重排序,此表达式使用，扩展的方式进行</param>
        /// <param name="selector">选择表达式</param>
        /// <typeparam name="TEntity">业务实体类型</typeparam>
        /// <typeparam name="TResult">结果集类型</typeparam>
        /// <returns>返回分页数据</returns>
        /// <exception cref="ArgumentNullException">当选择条件selector为null时，抛出此异常</exception>
        public static PageInfo<TResult> Query<TEntity, TResult>(this IQueryable<TEntity> query, int index,
            int pageSize,
            List<Expression<Func<TEntity, bool>>> wheres,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> funOrderby,
            Func<IQueryable<TEntity>, List<TResult>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (funOrderby == null)
            {
                throw new ArgumentNullException("funOrderby");
            }

            PageInfo.CheckPageIndexAndSize(ref index, ref pageSize);
            IQueryable<TEntity> queryable = query;
            if (wheres != null)
            {
                wheres.ForEach(p =>
                {
                    if (p != null)
                    {
                        queryable = queryable.Where(p);
                    }
                });
            }

            int count = queryable.Count();
            PageInfo.CheckPageIndexAndSize(ref index, pageSize, count);
            if (count > 0)
            {
                queryable = funOrderby(queryable);
                if (index != 1)
                {
                    queryable = queryable.Skip((index - 1) * pageSize);
                }

                queryable = queryable.Take(pageSize);
                return new PageInfo<TResult>(index, pageSize, count, selector(queryable));
            }

            return new PageInfo<TResult>(index, pageSize, count, new List<TResult>());
        }


        /// <summary>
        /// 指事务隔离级别执行相关查询
        /// </summary>
        /// <param name="query">待查询对象</param>
        /// <param name="action">查询方法</param>
        /// <param name="isolationLevel">默认读未提交</param>
        /// <param name="timeOut">事务溢出时间</param>
        /// <typeparam name="TEntity">查询对象类型</typeparam>
        public static void QueryWithTransactions<TEntity>(this IQueryable<TEntity> query, Action action, System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, TimeSpan? timeOut = null)
        {
            if (action == null)
            {
                return;
            }

            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = isolationLevel
            };

            if (timeOut != null)
            {
                transactionOptions.Timeout = timeOut.Value;
            }

            using (var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    action();
                }
                finally
                {
                    transactionScope.Complete();
                }

            }
        }

        /// <summary>
        /// Linq中使用读未提交进行查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <param name="action">执行方法</param>
        /// <param name="timeOut">事务溢出时间</param>
        /// <typeparam name="TEntity">查询对象类型</typeparam>
        public static void QueryWithNoLock<TEntity>(this IQueryable<TEntity> query, Action action,
            TimeSpan? timeOut = null)
        {
            QueryWithTransactions(query, action, System.Transactions.IsolationLevel.ReadUncommitted, timeOut);
        }
       
    }
}
