using System;

namespace Zhoubin.Infrastructure.Common.Repositories
{
    /// <summary>
    /// 访问出现异常
    /// </summary>
    [Serializable]
    public class AccessException : InfrastructureException
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        public AccessException()
            : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常内容</param>
        public AccessException(string message)
            : base(message)
        {

        }

        #endregion
    }
}
