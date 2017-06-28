using System;
using System.IO;
using System.Web;

namespace Zhoubin.Infrastructure.Web.Extent
{
    /// <summary>
    /// HttpContent扩展方法
    /// </summary>
    public static class HttpContentExtent
    {
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="context">Http上下文环境</param>
        /// <param name="stream">文件流，下载完成后，自动关闭流</param>
        /// <param name="fileName">客户下载时显示的名称</param>
        /// <param name="contentType">内容类型，默认值：application/octet-stream</param>
        public static void DownloadFile(this HttpContext context, Stream stream, string fileName, string contentType = null)
        {
            Utility.DownloadFile(context.Response, stream, fileName, contentType);
        }

        /// <summary>
        /// 多线程文件下载
        /// </summary>
        /// <param name="context">Http上下文环境</param>
        /// <param name="stream">文件流，下载完成后，自动关闭流</param>
        /// <param name="fileMd5">文件唯一编码，如md5</param>
        /// <param name="fileName">客户下载时显示的名称</param>
        /// <param name="lastModifyTime">文件最近修改的时间</param>
        /// <param name="speed">下载速度限制</param>
        public static void DownloadFile(this HttpContext context, Stream stream, string fileMd5, string fileName,
                                        DateTime lastModifyTime, int speed)
        {
            Utility.DownloadFile(context.Request, context.Response, stream, fileMd5, fileName, lastModifyTime, speed);
        }
    }
}