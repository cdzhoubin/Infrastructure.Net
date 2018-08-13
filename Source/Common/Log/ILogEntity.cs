using System;

namespace Zhoubin.Infrastructure.Common.Log
{
    /// <summary>
    /// 日志实体接口
    /// </summary>
    public interface ILogEntity
    {
        /// <summary>
        /// 日志类别
        /// </summary>
        Sevenrity Sevenrity { get; set; }
        /// <summary>
        /// 事件编号
        /// </summary>
        string EventId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// IP地址
        /// </summary>
// ReSharper disable once InconsistentNaming
        string IPAddress { get; }
        /// <summary>
        /// 程序名称
        /// </summary>
        string FileName { get; }


        /// <summary>
        /// 应用程序域名
        /// </summary>
        string AppDomainName { get; }
        /// <summary>
        /// 进程编号
        /// </summary>
        int PorcessId { get; }
        /// <summary>
        /// 进程名称
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// 机器名称
        /// </summary>
        string MachineName { get; }

        /// <summary>
        /// 线程编号
        /// </summary>
        int ThreadId { get; }
    }
}
