using Ninject;

namespace Zhoubin.Infrastructure.Common.Ioc.Ninject
{
    /// <summary>
    /// Ninject Ioc实现类
    /// </summary>
    public abstract class NinjectResolverBase : ResolverBase
    {
        private IKernel _kernel;

        /// <summary>
        /// 获取IKernel实例
        /// </summary>
        /// <returns>每次调用都创建新的</returns>
        protected virtual IKernel GetKernel()
        {
            return new StandardKernel();
        }

        /// <summary>
        /// Ioc创建对象注入接口
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">配置名称</param>
        /// <returns>创建成功类型</returns>
        public override T Resolve<T>(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return _kernel.Get<T>();
            }
            return _kernel.Get<T>(name);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            _kernel = GetKernel();
            Init(_kernel);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="kernel">ioc实例</param>
        protected abstract void Init(IKernel kernel);
    }
}
