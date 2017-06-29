namespace Zhoubin.Infrastructure.Log
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="info">日志信息</param>
        void Write(object info);

        /// <summary>
        /// 关闭日志写入
        /// </summary>
        void ShutDown();
    }
}