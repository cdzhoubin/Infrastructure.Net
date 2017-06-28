using System;
using Zhoubin.Infrastructure.Common.Repositories.EventHandler;

namespace Zhoubin.Infrastructure.Common.Repositories
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region Commit

        /// <summary>
        /// 能提交事件
        /// </summary>
        event AccessEventHandler CanCommit;

        /// <summary>
        /// 提交前事件
        /// </summary>
        event CommitEventHandler Committing;

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns>返回0成功，其它失败</returns>
        int Commit();

        /// <summary>
        /// 提交失败事件
        /// </summary>
        event CommitEventHandler CommittingFailed;

        /// <summary>
        /// 提交成功事件
        /// </summary>
        event CommitEventHandler Committed;

        #endregion

        /// <summary>
        /// 创建仓储模式
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>返回创建成功的仓储对象</returns>
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// 标记当前单元是否已经释放
        /// </summary>
        bool IsDisposed { get; }
    }
}
