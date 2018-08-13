using System;

namespace Zhoubin.Infrastructure.Common.Repositories.Mongo
{
    /// <summary>
    /// Mongodb单元创建工厂
    /// </summary>
    public class MongoUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        public MongoUnitOfWorkFactory()
        { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">创建DbContent回调</param>
        public MongoUnitOfWorkFactory(string context)
        {
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        private static string _context;
        #endregion

        /// <inheritdoc />
        public IUnitOfWork Create()
        {
            return new MongoUnitOfWork(_context);
        }

        #region IDisposable Implementation

        private bool _disposed;


        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="disposing">释放中</param>
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
        ~MongoUnitOfWorkFactory()
        {
            Dispose(false);
        }

        #endregion
    }
}