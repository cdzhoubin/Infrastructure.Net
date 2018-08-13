using System;
using System.Linq;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.Repositories.EventArgs;
using Zhoubin.Infrastructure.Common.Repositories.EventHandler;

namespace Zhoubin.Infrastructure.Common.Repositories.Mongo
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public class MongoUnitOfWork : IUnitOfWork
    {
        /// <inheritdoc />
        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        /// <inheritdoc />
        public event AccessEventHandler CanCommit;
        /// <inheritdoc />
        public event CommitEventHandler Committed;
        /// <inheritdoc />
        public event CommitEventHandler Committing;
        /// <inheritdoc />
        public event CommitEventHandler CommittingFailed;

        /// <inheritdoc />
        public int Commit()
        {
            CommitEventHandler committing = Committing;
            if (committing != null)
            {
                committing.Invoke(this, new CommitEventArgs());
            }
            CommitEventHandler committed = Committed;
            if (committed != null)
            {
                committed.Invoke(this,new CommitEventArgs());
            }
            return 0;
        }

        /// <summary>
        /// 创建仓储模式
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>返回创建成功的仓储对象</returns>
        public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class,IDocumentEntity, new()
        {
            return UnitOfWork.Resolver.Resolve<IRepository<TEntity>>();
        }


        /// <inheritdoc />
        IRepository<TEntity> IUnitOfWork.CreateRepository<TEntity>()
        {
            var type = typeof(TEntity);
            if (!type.GetInterfaces().Contains(typeof(IDocumentEntity)))
            {
                throw new ArgumentException(type.FullName + "没有实现接口："+typeof(IDocumentEntity).FullName);
            }
            if (!type.GetConstructors().Any(p => p.IsPublic && !p.GetParameters().Any()))
            {
                throw new ArgumentException(type.FullName + "没有默认构造函数。");
            }
            return UnitOfWork.Resolver.Resolve<IRepository<TEntity>>();
        }


        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _isDisposed = true;
        }

        private bool _isDisposed;
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        public MongoUnitOfWork()
        {
            _context = Factory.CreateObjectStorage();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="storeName">创建DbContent回调</param>
        public MongoUnitOfWork(string storeName)
        {
            _context = string.IsNullOrEmpty(storeName) ? Factory.CreateObjectStorage(): Factory.CreateObjectStorage(storeName);
        }

        #endregion
        #region Context 

        /// <summary>
        /// 
        /// </summary>
        private readonly IObjectStorage _context;

        /// <summary>
        /// 当前Content对象
        /// </summary>
        public IObjectStorage Context
        {
            get
            {
                return _context;
            }
        }

        #endregion
    }
}