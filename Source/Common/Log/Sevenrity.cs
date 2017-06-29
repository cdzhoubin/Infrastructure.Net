namespace Zhoubin.Infrastructure.Log
{
    /// <summary>
    /// 日志信息分类
    /// </summary>
    public enum Sevenrity
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug = 0,
        /// <summary>
        /// 提示
        /// </summary>
        Info = 1,
        /// <summary>
        /// 警告
        /// </summary>
        Warn = 2,
        /// <summary>
        /// 跟踪
        /// </summary>
        Trace = 3,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 4,
        /// <summary>
        /// 致命错误
        /// </summary>
        Fatal = 5
    }
}