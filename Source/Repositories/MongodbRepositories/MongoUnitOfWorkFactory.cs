using System;

namespace Zhoubin.Infrastructure.Common.Repositories.Mongo
{
    /// <summary>
    /// Mongodb��Ԫ��������
    /// </summary>
    public class MongoUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Constructor

        /// <summary>
        /// ���캯��
        /// </summary>
        public MongoUnitOfWorkFactory()
        { }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="context">����DbContent�ص�</param>
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


        /// <summary>ִ�����ͷŻ����÷��й���Դ������Ӧ�ó����������</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// �ͷŶ���
        /// </summary>
        /// <param name="disposing">�ͷ���</param>
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
        /// ��������
        /// </summary>
        ~MongoUnitOfWorkFactory()
        {
            Dispose(false);
        }

        #endregion
    }
}