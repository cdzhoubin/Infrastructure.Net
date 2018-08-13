namespace Zhoubin.Infrastructure.Common.Log
{
    /// <summary>
    /// 日志实体
    /// 默认日志Sevenrity = Sevenrity.Info
    /// </summary>
    public class LogEntity : LogEntityBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LogEntity()
        {
            Sevenrity = Sevenrity.Info;
        }
    }
}