using System;
using System.Text;

namespace Zhoubin.Infrastructure.Log
{
    /// <summary>
    /// 异常日志实体
    /// </summary>
    public sealed class LogExceptionEntity : LogEntity
    {
        private readonly Exception _ex;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex">异常</param>
        public LogExceptionEntity(Exception ex):this()
        {
            Title = ex.Message;
            Content = ex.StackTrace;
            _ex = ex;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogExceptionEntity()
        {
            Sevenrity = Sevenrity.Error;
        }

        /// <summary>
        /// 转换日志到异常字符串
        /// 序列化到指定的<see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">序列化到指定的<see cref="StringBuilder"/></param>
        protected override void ToString(StringBuilder sb)
        {
            base.ToString(sb);
            
            if (_ex != null)
            {
                sb.AppendFormat("\n【Exception】{0}", _ex);
                var ex = _ex.InnerException;
                while (ex != null)
                {
                    sb.AppendFormat("\n【InnerException】{0}", ex);
                    ex = ex.InnerException;
                }
            }
        }
    }
}