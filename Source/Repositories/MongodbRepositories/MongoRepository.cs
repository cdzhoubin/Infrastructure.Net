using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.Repositories.EventArgs;
using Zhoubin.Infrastructure.Common.Repositories.EventHandler;

namespace Zhoubin.Infrastructure.Common.Repositories.Mongo
{
    /// <summary>
    /// Mogodb仓储实现
    /// </summary>
    /// <typeparam name="TEntity">类型</typeparam>
    public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : class, IDocumentEntity, new()
    {
        #region Constructor

        #endregion

        #region IDisposable Implementation

        private bool _disposed;

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="disposing">对象释放中</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MongoRepository()
        {
            Dispose(false);
        }

        #endregion

        #region IRepository<TEntity> Implementation

        #region UnitOfWork

        /// <summary>
        /// 工作单元
        /// </summary>
        private MongoUnitOfWork _unitOfWork;

        /// <summary>
        /// 当前工作单元，当前线程或当前请求
        /// </summary>
        public MongoUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork ??
                       (_unitOfWork = (MongoUnitOfWork) Repositories.UnitOfWork.Current);
            }
        }

        #endregion

        #region Context

        /// <summary>
        /// 
        /// </summary>
        private IObjectStorage _context;

        /// <summary>
        /// 当前IObjectStorage
        /// </summary>
        public IObjectStorage Context
        {
            get { return _context ?? (_context = UnitOfWork.Context); }
        }

        #endregion

        #region Create

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns>返回创建后的实体</returns>
        public virtual TEntity Create()
        {
            TEntity entity = new TEntity();
            OnCreated(new ActionEventArgs<TEntity>(entity));
            return entity;
        }


        /// <inheritdoc />
        public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            TDerivedEntity derivedEntity = Activator.CreateInstance(typeof(TDerivedEntity)) as TDerivedEntity;

            OnCreated(new ActionEventArgs<TEntity>(derivedEntity));
            return derivedEntity;
        }


        /// <inheritdoc />
        public event ActionEventHandler<TEntity> Created;

        /// <summary>
        /// 触发创建成功事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnCreated(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Created;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Insert

        /// <inheritdoc />
        public event AccessEventHandler CanInsert;

        
        /// <summary>
        /// 允许新增事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnCanInsert(AccessEventArgs e)
        {
            AccessEventHandler handler = CanInsert;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <inheritdoc />
        public event ActionEventHandler<TEntity> Inserting;

        /// <summary>
        /// 触发新增前事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnInserting(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Inserting;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">待插入对象</param>
        /// <returns>返回插入成功后的对象</returns>
        /// <exception cref="AccessException">不允许新增时，抛出此异常</exception>
        public virtual TEntity Insert(TEntity entity)
        {
            if (CheckAccess(OnCanInsert))
            {
                OnInserting(new ActionEventArgs<TEntity>(entity));
                Invoke(() =>
                {
                    entity.Id = new ObjectId(Context.Insert(entity));
                    //UnitOfWork.Commit();
                });

                OnInserted(new ActionEventArgs<TEntity>(entity));
            }
            else
            {
                throw new AccessException("CanInsert");
            }

            return entity;
        }


        /// <inheritdoc />
        public event ActionEventHandler<TEntity> Inserted;

        /// <summary>
        /// 触发新增成功事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnInserted(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Inserted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Select

        /// <inheritdoc />
        public event AccessEventHandler CanRead;

        /// <summary>
        /// 触发允许读取事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnCanRead(AccessEventArgs e)
        {
            AccessEventHandler handler = CanRead;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <inheritdoc />
        public event FilterEventHandler<TEntity> Filter;

        /// <summary>
        /// 触发过虑事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnFilter(FilterEventArgs<TEntity> e)
        {
            FilterEventHandler<TEntity> handler = Filter;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// 根据Key获取实体
        /// </summary>
        /// <param name="keyValue">实体键</param>
        /// <returns>返回查询结果</returns>
        /// <exception cref="AccessException">不允许读取时抛出此异常</exception>
        public virtual TEntity GetByKey(object keyValue)
        {
            if (CheckAccess(OnCanRead))
            {
                TEntity entity = null;
                Invoke(() => entity = _context.FindById<TEntity>(keyValue.ToString()));
                return entity;
            }

            throw new AccessException("CanRead");
        }


        /// <summary>
        /// 根据一组key获取实体
        /// </summary>
        /// <param name="keyValues">一组查询条件</param>
        /// <returns>返回查询到的实例</returns>
        /// <exception cref="System.InvalidOperationException">查询时可能会抛出异常</exception>
        /// <exception cref="AccessException">不允许读取时抛出此异常</exception>
        public virtual TEntity GetByKeys(params object[] keyValues)
        {
            if (CheckAccess(OnCanRead))
            {
                TEntity entity = null;
                Invoke(() =>
                {
                    foreach (object t in keyValues)
                    {
                        entity = Context.FindById<TEntity>(t.ToString());
                        if (entity != null)
                        {
                            break;
                        }
                    }
                });
                return entity;
            }

            throw new AccessException("CanRead");
        }


        /// <summary>
        /// 获取所有实例数组
        /// </summary>
        /// <returns>返回所有实体</returns>
        /// <exception cref="System.ArgumentNullException">查询时可能会抛出异常</exception>
        /// <exception cref="AccessException">不允许读取时抛出此异常</exception>
        public virtual IEnumerable<TEntity> Get()
        {
            return Get(null, null);
        }


        /// <summary>
        /// 获取指定过虑条件的实体数组
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns>查询结果</returns>
        /// <exception cref="System.ArgumentNullException">查询过虑时可能抛出此异常</exception>
        /// <exception cref="AccessException">不允许读取时抛出此异常</exception>
        public virtual IEnumerable<TEntity> Get(Func<TEntity, bool> filter)
        {
            return Get(filter, null);
        }


        /// <summary>
        /// 根据指定排序获取所有实体
        /// </summary>
        /// <param name="order">排序条件</param>
        /// <returns>查询结果</returns>
        /// <exception cref="System.ArgumentNullException">查询过虑时可能抛出此异常</exception>
        /// <exception cref="AccessException">不允许读取时抛出此异常</exception>
        public virtual IEnumerable<TEntity> Get(Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> order)
        {
            return Get(null, order);
        }


        /// <summary>
        /// 根据指定过虑条件、排序获取所有实体
        /// </summary>
        /// <param name="filter">过虑条件</param>
        /// <param name="order">排序</param>
        /// <returns>返回迭代对象</returns>
        /// <exception cref="System.ArgumentNullException">查询过虑时可能抛出此异常</exception>
        /// <exception cref="AccessException">不允许读取时抛出此异常</exception>
        public virtual IEnumerable<TEntity> Get(Func<TEntity, bool> filter,
            Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> order)
        {
            if (CheckAccess(OnCanRead))
            {

                IEnumerable<TEntity> result = null;
                Invoke((query) =>
                {
                    result = query.Where(FilterEntity);
                    if (filter != null)
                    {
                        result = result.Where(filter);
                    }

                    result = order == null ? result.ToList() : order(result).ToList();
                });
                return result;
            }

            throw new AccessException("CanRead");
        }

        #endregion

        #region Update

        /// <inheritdoc />
        public event AccessEventHandler CanUpdate;

        /// <summary>
        /// 触发是否允许更新事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnCanUpdate(AccessEventArgs e)
        {
            AccessEventHandler handler = CanUpdate;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <inheritdoc />
        public event ActionEventHandler<TEntity> Updating;

        /// <summary>
        /// 触发更新前事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnUpdating(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Updating;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">更新对象</param>
        /// <returns>更新成功后，返回更新后的对象</returns>
        /// <exception cref="AccessException">不允许更新时抛出此异常</exception>
        public virtual TEntity Update(TEntity entity)
        {
            if (CheckAccess(OnCanUpdate))
            {
                OnUpdating(new ActionEventArgs<TEntity>(entity));
                Invoke(() =>
                {
                    _context.Update(entity);
                });

                OnUpdated(new ActionEventArgs<TEntity>(entity));
                return entity;
            }

            throw new AccessException("CanUpdate");
        }

        /// <summary>
        /// 更新完成事件
        /// </summary>
        public event ActionEventHandler<TEntity> Updated;

        /// <summary>
        /// 触发更新完成事件
        /// </summary>
        /// <param name="e">事件</param>
        protected virtual void OnUpdated(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Updated;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        public event AccessEventHandler CanDelete;

        /// <summary>
        /// 触发删除事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnCanDelete(AccessEventArgs e)
        {
            AccessEventHandler handler = CanDelete;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 删除前事件
        /// </summary>
        public event ActionEventHandler<TEntity> Deleting;

        /// <summary>
        /// 触发删除前事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnDeleting(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Deleting;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">键</param>
        /// <returns>返回删除成功的对象</returns>
        public virtual TEntity Delete(object keyValue)
        {
            TEntity result = null;
            Invoke(() =>
            {
                result = _context.FindById<TEntity>(keyValue.ToString());

                if (result != null)
                {
                    Delete(result); 
                }
            });
            return result;
        }


        /// <summary>
        /// 删除一组Key实体
        /// </summary>
        /// <param name="keyValues">一组键</param>
        /// <returns>返回成功删除的对象</returns>
        /// <exception cref="System.InvalidOperationException">查询时可能会抛出此异常</exception>
        /// <exception cref="AccessException">不允许删除时抛出此异常</exception>
        public virtual TEntity Delete(params object[] keyValues)
        {
            TEntity result = null;
            Invoke(() =>
            {
                foreach (object t in keyValues)
                {
                    result = Context.FindById<TEntity>(t.ToString());
                    if (result != null)
                    {
                        break;
                    }
                }
            });

            result = Delete(result);
            return result;
        }


        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回删除成功的实体</returns>
        /// <exception cref="AccessException">不允许删除时抛出此异常</exception>
        public virtual TEntity Delete(TEntity entity)
        {
            if (CheckAccess(OnCanDelete))
            {
                OnDeleting(new ActionEventArgs<TEntity>(entity));
                Invoke(() =>
                {
                    _context.Delete<TEntity>(p => p.Id == entity.Id);
                    //UnitOfWork.Commit();
                });

                OnDeleted(new ActionEventArgs<TEntity>(entity));
                return entity;
            }

            throw new AccessException("CanDelete");
        }


        /// <inheritdoc />
        public event ActionEventHandler<TEntity> Deleted;

        /// <summary>
        /// 触发删除成功事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnDeleted(ActionEventArgs<TEntity> e)
        {
            ActionEventHandler<TEntity> handler = Deleted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// 权限访问检查
        /// </summary>
        /// <param name="accessAction">访问权限检查Action</param>
        /// <returns>有权限返回true</returns>
        /// <exception cref="System.ArgumentNullException">accessAction== null 抛出此异常</exception>
        protected bool CheckAccess(Action<AccessEventArgs> accessAction)
        {
            if (accessAction == null)
            {
                throw new ArgumentNullException("accessAction");
            }

            var accessEventArgs = new AccessEventArgs(true);
            Invoke(() => accessAction.Invoke(accessEventArgs));
            return accessEventArgs.Result;
        }

        /// <summary>
        /// 过虑实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>过虑成功返回true</returns>
        protected bool FilterEntity(TEntity entity)
        {
            var filterEventArgs = new FilterEventArgs<TEntity>(entity, true);
            Invoke(() => OnFilter(filterEventArgs));
            return filterEventArgs.Result;
        }

        #endregion

        #endregion
        /// <summary>
        /// 高级查询对象
        /// </summary>
        public IQueryable<TEntity> Queryable
        {
            get
            {
                return _context.CreateQueryable<TEntity>();
            }
        }

        private void Invoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw new Exception("数据访问出现异常，详细信息请查看内部异常", ex);
            }
        }
        private void Invoke(Action<IQueryable<TEntity>>  action)
        {
            try
            {
                _context.AdvanceQuery(action);
            }
            catch (Exception ex)
            {
                throw new Exception("数据访问出现异常，详细信息请查看内部异常", ex);
            }
        }
    }
}
