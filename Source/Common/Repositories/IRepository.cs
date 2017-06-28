using System;
using System.Collections.Generic;
using System.Linq;
using Zhoubin.Infrastructure.Common.Repositories.EventHandler;

namespace Zhoubin.Infrastructure.Common.Repositories
{
    /// <summary>
    /// 仓储模式
    /// </summary>
    /// <typeparam name="TEntity">仓储实体</typeparam>
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        #region Create

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <returns>返回创建好的实体</returns>
        TEntity Create();

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="TDerivedEntity">派生类型</typeparam>
        /// <returns>返回创建好的派生实体</returns>
        TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity;
        
        /// <summary>
        /// 创建实体成功事件
        /// </summary>
        event ActionEventHandler<TEntity> Created;

        #endregion

        #region Insert

        /// <summary>
        /// 可以插入实例事件
        /// </summary>
        event AccessEventHandler CanInsert;

        /// <summary>
        /// 插入前事件
        /// </summary>
        event ActionEventHandler<TEntity> Inserting;

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">待插入对象</param>
        /// <returns>返回插入成功后的对象</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 插入成功事件
        /// </summary>
        event ActionEventHandler<TEntity> Inserted;

        #endregion

        #region Select
        
        /// <summary>
        /// 是否可读事件
        /// </summary>
        event AccessEventHandler CanRead;

        /// <summary>
        /// 查询过虑事件
        /// </summary>
        event FilterEventHandler<TEntity> Filter;

        /// <summary>
        /// 根据Key获取实体
        /// </summary>
        /// <param name="keyValue">实体键</param>
        /// <returns>返回查询结果</returns>
        TEntity GetByKey(object keyValue);

        /// <summary>
        /// 根据一组key获取实体
        /// </summary>
        /// <param name="keyValues">一组查询条件</param>
        /// <returns>返回查询到的实例</returns>
        TEntity GetByKeys(params object[] keyValues);

        /// <summary>
        /// 获取所有实例数组
        /// </summary>
        /// <returns>返回所有实体</returns>
        IEnumerable<TEntity> Get();

        /// <summary>
        /// 获取指定过虑条件的实体数组
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns>查询结果</returns>
        IEnumerable<TEntity> Get(Func<TEntity, bool> filter);
        
        /// <summary>
        /// 根据指定排序获取所有实体
        /// </summary>
        /// <param name="order">排序条件</param>
        /// <returns>查询结果</returns>
        IEnumerable<TEntity> Get(Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> order);

        /// <summary>
        /// 根据指定过虑条件、排序获取所有实体
        /// </summary>
        /// <param name="filter">过虑条件</param>
        /// <param name="order">排序</param>
        /// <returns>返回迭代对象</returns>
        IEnumerable<TEntity> Get(Func<TEntity, bool> filter, Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> order);

        /// <summary>
        /// 查询Queryable对象，此对象用于高级查询实现
        /// </summary>
        IQueryable<TEntity> Queryable
        {
            get;
        }

        #endregion

        #region Update

        /// <summary>
        /// 能否更新事件
        /// </summary>
        event AccessEventHandler CanUpdate;

        /// <summary>
        /// 正在更新事件
        /// </summary>
        event ActionEventHandler<TEntity> Updating;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">更新对象</param>
        /// <returns>更新成功后，返回更新后的对象</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更新完成事件
        /// </summary>
        event ActionEventHandler<TEntity> Updated;

        #endregion
        
        #region Delete

        /// <summary>
        /// 是否能删除事件
        /// </summary>
        event AccessEventHandler CanDelete;

        /// <summary>
        /// 删除前事件
        /// </summary>
        event ActionEventHandler<TEntity> Deleting;

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">键</param>
        /// <returns>返回删除成功的对象</returns>
        TEntity Delete(object keyValue);

        /// <summary>
        /// 删除一组Key实体
        /// </summary>
        /// <param name="keyValues">一组键</param>
        /// <returns>返回成功删除的对象</returns>
        TEntity Delete(params object[] keyValues);

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回删除成功的实体</returns>
        TEntity Delete(TEntity entity);

        /// <summary>
        /// 删除成功事件
        /// </summary>
        event ActionEventHandler<TEntity> Deleted;

        #endregion
    }
}
