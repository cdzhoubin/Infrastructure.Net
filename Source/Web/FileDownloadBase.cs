using System;
using System.IO;
using System.Text;
using System.Web;
using Zhoubin.Infrastructure.Common.Extent;
using Zhoubin.Infrastructure.Web.Extent;

namespace Zhoubin.Infrastructure.Web
{
    /// <summary>
    /// 文件下载HttpHandler基类
    /// </summary>
    public abstract class FileDownloadBase : IHttpHandler
    {
        /// <summary>
        /// 获取一个值，该值指示其他请求是否可以使用 <see cref="T:System.Web.IHttpHandler"/> 实例。
        /// </summary>
        /// <returns>
        /// 如果 <see cref="T:System.Web.IHttpHandler"/> 实例可再次使用，则为 true；否则为 false。
        /// </returns>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// 单线程下载
        /// </summary>
        public abstract bool SingleThread { get; set; }
        /// <summary>
        /// 上下文
        /// </summary>
        protected HttpContext Context { get; set; }
        /// <summary>
        /// 处理下载请求
        /// </summary>
        /// <param name="context">上下文</param>
        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            if (!CheckParams())
            {
                context.Response.StatusCode = 400;
                return;
            }

            if (!Check())
            {
                context.Response.StatusCode = 401;
                return;
            }
            using (var stream = GetStream())
            {
                if (SingleThread)
                {
                    context.DownloadFile(stream, GetFileName());
                }
                else
                {
                    context.DownloadFile(stream, GetFileMd5Hash(stream), GetFileName(), GetLastModifyTime(), GetSpeed());
                }
            }

        }

        /// <summary>
        /// 检查参数
        /// </summary>
        /// <returns>检查参数通过返回true</returns>
        protected virtual bool CheckParams()
        {
            return true;
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <returns>权限检查通过返回true</returns>
        protected virtual bool Check()
        {
            return true;
        }

        /// <summary>
        /// 下载的文件名
        /// </summary>
        /// <returns>获取下载中设置的文件名</returns>
        protected abstract string GetFileName();

        /// <summary>
        /// 文件流
        /// </summary>
        /// <returns>获取下载文件流</returns>
        protected abstract Stream GetStream();

        /// <summary>
        /// 文件Hash编码
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>返回文件流md5</returns>
        protected virtual string GetFileMd5Hash(Stream stream)
        {
            var calculator = System.Security.Cryptography.MD5.Create();
            var buffer = calculator.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var b in buffer)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 文件最近修改的时间
        /// </summary>
        /// <returns>返回修改时间</returns>
        protected virtual DateTime GetLastModifyTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 下载速度设置
        /// 多线程适用
        /// </summary>
        /// <returns>返回下载速度</returns>
        protected virtual int GetSpeed()
        {
            return 100;
        }
        
    }
}
