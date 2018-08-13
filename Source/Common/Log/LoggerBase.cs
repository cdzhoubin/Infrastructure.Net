namespace Zhoubin.Infrastructure.Common.Log
{
    /// <summary>
    /// 日志接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public abstract class LoggerBase<T>:ILogger where T : ILogEntity
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="info">日志对象</param>
        protected  abstract void Write(T info);

        /// <summary>
        /// 写入字符串日志
        /// </summary>
        /// <param name="info">日志字符串</param>
        protected abstract void Write(string info);

        /// <summary>
        /// 写入对象信息
        /// </summary>
        /// <param name="info">待写入对象</param>
        public void Write(object info)
        {
            if (info == null)
            {
                return;
            }

            if (info is ILogEntity)
            {
                Write((T)info);
                return;
            }

            Write(info.ToString());
        }

        /// <summary>
        /// 关闭日志写入
        /// </summary>
        public abstract void ShutDown();
    }
}