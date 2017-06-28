using System;
using System.Collections;
using System.Threading;
using System.Web;
using Zhoubin.Infrastructure.Common.Ioc;

namespace Zhoubin.Infrastructure.Common.Repositories
{
    /// <summary>
    /// 工作单元工厂
    /// </summary>
    public static class UnitOfWork
    {
        #region Factory

        /// <summary>
        /// 工作单元工厂
        /// </summary>
        private static IUnitOfWorkFactory _factory;

        /// <summary>
        /// 工作单元工厂
        /// </summary>
        private static IUnitOfWorkFactory Factory
        {
            get
            {
                return _factory;
            }
            set
            {
                if (value != _factory)
                {
                    _factory = value;
                }
            }
        }

        #endregion

        #region Threads

        /// <summary>
        /// 线程表
        /// </summary>
        private static Hashtable _threads;

        /// <summary>
        /// 线程表
        /// </summary>
        private static Hashtable Threads
        {
            get
            {
                if (_threads == null)
                {
                    _threads = new Hashtable();
                }

                return _threads;
            }
        }

        #endregion

        #region Current

        /// <summary>
        /// 
        /// </summary>
        private const string HttpContextKey = "System.Data.Entity.Extensions.UnitOfWork.Key";

        /// <summary>
        /// 获取当前工作单元
        /// </summary>
        public static IUnitOfWork Current
        {

            get
            {
                IUnitOfWork unitOfWork = Get();

                if (unitOfWork != null && unitOfWork.IsDisposed)
                {
                    unitOfWork = null;
                }

                if (unitOfWork == null) //当前工作单元为空的情况下，使用工作方法创建工作单元
                {
                    //Factory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
                    Factory = Resolver.Resolve<IUnitOfWorkFactory>();
                    unitOfWork = Factory.Create();
                    Save(unitOfWork);
                }

                return unitOfWork;
            }
        }

        /// <summary>
        /// 获取默认工作单元
        /// </summary>
        /// <returns>获取当前工作单元，如果不存在就创建一个新的返回</returns>
        private static IUnitOfWork Get()
        {
            if (HttpContext.Current == null) //非Web环境实现，与线程相关
            {
                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                //如果当前线程没有名称，就没有工作单元，设置名称
                {
                    Thread.CurrentThread.Name = Guid.NewGuid().ToString();
                    return null;
                }

                lock (Threads.SyncRoot) //根据工作线程获取工作单元
                {
                    if (Thread.CurrentThread.Name != null && Threads.ContainsKey(Thread.CurrentThread.Name))
                    {
                        return (IUnitOfWork)Threads[Thread.CurrentThread.Name];
                    }
                }
            }
            else
            {
                if (HttpContext.Current.Items.Contains(HttpContextKey))
                {
                    return (IUnitOfWork)HttpContext.Current.Items[HttpContextKey];
                }
            }
            return null;
        }

        /// <summary>
        /// 保存创建好的工作单元
        /// </summary>
        /// <param name="unitOfWork">保存工作单元</param>
        private static void Save(IUnitOfWork unitOfWork)
        {
            if (HttpContext.Current == null)
            {
                if (Thread.CurrentThread.Name == null)
                {
                    return;
                }
                lock (Threads.SyncRoot)
                {
                    Threads[Thread.CurrentThread.Name] = unitOfWork;
                }
            }
            else
            {
                HttpContext.Current.Items[HttpContextKey] = unitOfWork;
            }
        }

        #endregion

        #region IUnitOfWork

        /// <summary>
        /// 提交工作单元
        /// </summary>
        public static void Commit()
        {
            IUnitOfWork unitOfWork = Get();

            if (unitOfWork != null)
            {
                unitOfWork.Commit();
            }
        }

        #endregion

        #region Ioc

        private static IResolver _resolver;
        /// <summary>
        /// 注册Resolver接口
        /// </summary>
        /// <param name="resolver">Ioc接口</param>
        public static void RegisterResolver(IResolver resolver)
        {
            _resolver = resolver;
        }

        /// <summary>
        /// 查询当前的Ioc接口
        /// </summary>
        /// <exception cref="InfrastructureException">Ioc没有初始化时，抛出此异常</exception>
        public static IResolver Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    throw new InfrastructureException("Ioc接口没有初始化，使用前，请先调用RegisterResolver");
                }

                return _resolver;
            }
        }
        #endregion
    }
}
