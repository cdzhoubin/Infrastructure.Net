using System;

namespace Zhoubin.Infrastructure.Common
{
    /// <summary>
    /// 基础架构异常
    /// </summary>
    public class InfrastructureException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">信息</param>
        public InfrastructureException(string message) : base(message) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        public InfrastructureException() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="innerException">内联异常</param>
        public InfrastructureException(string message, Exception innerException) : base(message, innerException) { }

    }
}
