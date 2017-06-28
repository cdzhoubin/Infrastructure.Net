using System;
using System.IO;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.Extent
{
    /// <summary>
    /// asp.net页面扩展方法
    /// </summary>
    public static class PageExtent
    {
        /// <summary>
        /// 用JS显示消息
        /// 使用JavaScript中的alert方法
        /// </summary>
        /// <param name="page">页面实例</param>
        /// <param name="type">控件类型</param>
        /// <param name="message">消息内容</param>
        /// <param name="url">如果不为空，显示提示信息，自动跳转页面到此地址</param>
        public static void ShowMessage(this Page page, Type type, string message, string url = null)
        {
            Utility.ShowMessage(page, type, message, url);
        }

        /// <summary>
        /// 注册一段脚本到页面
        /// </summary>
        /// <param name="page">页面实例</param>
        /// <param name="type">控件类型</param>
        /// <param name="script">脚本内容</param>
        public static void RegisterScript(this Page page, Type type, string script)
        {
            Utility.RegisterScript(page, type, script);
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="page">页面实例</param>
        /// <param name="stream">文件流，下载完成后，自动关闭流</param>
        /// <param name="fileName">文件名，包括扩展名</param>
        /// <param name="contentType">内容类型，如为null或空，就默认为：application/octet-stream</param>
        public static void DownloadFile(this Page page, Stream stream, string fileName, string contentType = null)
        {
            Utility.DownloadFile(page.Response, stream, fileName, contentType);
        }

        /// <summary>
        /// 截取指定长度的子串
        /// </summary>
        /// <param name="page">页面实例</param>
        /// <param name="ob">截取对象</param>
        /// <param name="length">截取长度</param>
        /// <returns>返回截取后的子串，如果小于截取长度，返回全部字符串内容</returns>
        public static string GetSubString(this Page page, object ob, int length)
        {
            return Utility.GetSubString(ob, length);
        }

        /// <summary>
        /// 多线程文件下载
        /// </summary>
        /// <param name="page">页面实例</param>
        /// <param name="stream">文件流，下载完成后，自动关闭流</param>
        /// <param name="fileMd5">文件唯一编码，如md5</param>
        /// <param name="fileName">客户下载时显示的名称</param>
        /// <param name="lastModifyTime">文件最近修改的时间</param>
        /// <param name="speed">下载速度限制，0表示不限制速度</param>
        public static void DownloadFile(this Page page, Stream stream, string fileMd5, string fileName,
                                        DateTime lastModifyTime, int speed)
        {
            Utility.DownloadFile(page.Request, page.Response, stream, fileMd5, fileName, lastModifyTime, speed);
        }
    }
}
