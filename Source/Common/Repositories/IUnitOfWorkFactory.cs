using System;

namespace Zhoubin.Infrastructure.Common.Repositories
{
    /// <summary>
    /// 工作单元工厂
    /// </summary>
    public interface IUnitOfWorkFactory : IDisposable
    {
        /// <summary>
        /// 创建工作单元
        /// </summary>
        /// <returns>返回创建成功的工作单元</returns>
        IUnitOfWork Create();
    }
}
