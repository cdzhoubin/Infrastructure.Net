using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.Extent
{
    static class Utility
    {
        private static bool? _customAlert;
        /// <summary>
        /// AppSetting中读取自定义弹出消息框使用模式
        /// Key:CustomAlert,1表示启用，其它表示使用alter
        /// </summary>
        internal static bool CustomAlert
        {
            get
            {
                if (_customAlert == null)
                {
                    _customAlert = System.Configuration.ConfigurationManager.AppSettings["CustomAlert"] == "1";
                }

                return _customAlert.Value;
            }
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="ob">待截取对象</param>
        /// <param name="length">待截取长度</param>
        /// <returns>当对象为null时，返回String.Empty;当字符串长度小于等于待截取长度时，返回原字符串；当字符串大于截取长度时，返回待截取长度减去2的字符串</returns>
        internal static string GetSubString(object ob, int length)
        {
            if (ob == null)
            {
                return string.Empty;
            }

            var str = ob.ToString().Trim();
            return str.Length > length ? string.Format("{0}...", str.Substring(0, length - 2)) : str;
        }
        internal static void ShowMessage(Page page, Type type, string message, string url = null)
        {
            RegisterScript(page, type, GetAlertScript(message, url));
        }

        internal static void ShowMessage(UpdatePanel updatePanel, Type type, string message, string url = null)
        {
            RegisterScript(updatePanel, type, GetAlertScript(message, url));
        }

        internal static void ShowMessageAndAutoClose(Page page, Type type, string message, string url = null)
        {
            RegisterScript(page, type, GetAutoCloseScript(message, url,5));
        }

        internal static void ShowMessageAndAutoClose(UpdatePanel updatePanel, Type type, string message, string url = null)
        {
            RegisterScript(updatePanel, type, GetAutoCloseScript(message, url,5));
        }

        internal static void ShowMessageAndAutoClose(Page page, Type type, string message,int timeOut, string url)
        {
            RegisterScript(page, type, GetAutoCloseScript(message, url,timeOut));
        }


        internal static void ShowMessageAndAutoClose(UpdatePanel updatePanel, Type type, string message, int timeOut, string url)
        {
            RegisterScript(updatePanel, type, GetAutoCloseScript(message, url, timeOut));
        }

        internal static void ShowMessageAndAutoClose(Page page, Type type, string message, int timeOut=5)
        {
            RegisterScript(page, type, GetAutoCloseScript(message, null, timeOut));
        }


        internal static void ShowMessageAndAutoClose(UpdatePanel updatePanel, Type type, string message, int timeOut=5)
        {
            RegisterScript(updatePanel, type, GetAutoCloseScript(message, null, timeOut));
        }

        static string GetAutoCloseScript(string message, string url,int timeOut)
        {
            if (!CustomAlert)
            {
                throw new Exception("此模块只能自定义Alert模式下才能使用。");
            }

            return string.IsNullOrEmpty(url) ? string.Format("smoke.signal('{0}', function(e){{}}, {1})", message.Replace("'", "\\'"), timeOut*1000)
                : string.Format("smoke.signal('{0}', function(e){{window.location.href='{1}';}}, {2})", message.Replace("'", "\\'"), url, timeOut * 1000);
        }

        static string GetAlertScript(string message, string url)
        {
            return string.IsNullOrEmpty(url) ? string.Format(CustomAlert ? "smoke.alert('{0}', function(e){{}},{{ok: \"{1}\"}})" : "alert('{0}');", message.Replace("'", "\\'"), Common.Properties.Resources.Ok)
                : string.Format(CustomAlert ? "smoke.alert('{0}', function(e){{window.location.href='{1}';}}, {{ok: \"{2}\"}})" : "alert('{0}');window.location.href='{1}';", message.Replace("'", "\\'"), url, Common.Properties.Resources.Ok);
        }

        internal static void RegisterScript(UpdatePanel panel, Type type, string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                return;
            }

            ScriptManager.RegisterStartupScript(panel, type, Guid.NewGuid().ToString().Replace("-", ""), script, true);
        }

        internal static void RegisterScript(Page page, Type type, string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                return;
            }

            page.ClientScript.RegisterStartupScript(type, Guid.NewGuid().ToString().Replace("-", ""), script, true);
        }

        internal static void DownloadFile(HttpResponse response, Stream stream, string fileName, string contentType = null)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            try
            {
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName));
                response.AddHeader("Content-Transfer-Encoding", "binary");
                response.ContentType = string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType;
                long total = 0;
                const int bufferSize = 1024 * 10;
                var buffer = new byte[bufferSize];
                stream.Position = 0;
                while (total < stream.Length && response.IsClientConnected)
                {
                    var count = stream.Read(buffer, 0, bufferSize);
                    response.OutputStream.Write(buffer, 0, count);
                    total += count;
                    response.Flush();
                }
            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// 多线程文件下载
        /// 支持限制速度
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="stream">文件流</param>
        /// <param name="fileMd5">文件Hash值</param>
        /// <param name="fileName">下载文件名</param>
        /// <param name="lastModifyTime">最后修改时间</param>
        /// <param name="speed">每秒字节数,单位KB</param>
        /// <returns></returns>
        internal static bool DownloadFile(HttpRequest request, HttpResponse response, Stream stream, string fileMd5, string fileName, DateTime lastModifyTime, int speed)
        {
            //方法来源：http://www.cnblogs.com/gjahead/archive/2007/06/18/787654.html
            //调整了其中的方法，限制速度的单位为KB
            #region 验证：HttpMethod，请求的文件是否存在
            switch (request.HttpMethod.ToUpper())//目前只支持GET和HEAD方法
            {
                case "GET":
                case "HEAD":
                    break;
                default:
                    response.StatusCode = 501;
                    return false;
            }
            if (stream == null)
            {
                response.StatusCode = 404;
                return false;
            }
            #endregion

            #region 定义局部变量
            long startBytes = 0;
            const int packSize = 1024 * 10; //分块读取，每块10K bytes

            using (var br = new BinaryReader(stream))
            {
                var fileLength = stream.Length;

                var sleep = (int)Math.Ceiling(1.0 * packSize / speed); //毫秒数：读取下一数据块的时间间隔

            #endregion

                #region 验证：文件是否太大，是否是续传，且在上次被请求的日期之后是否被修改过--------------

                if (stream.Length > Int32.MaxValue)
                {
                    response.StatusCode = 413; //请求实体太大
                    return false;
                }

                if (request.Headers["If-Range"] != null) //对应响应头ETag：文件md5在码
                {
                    //----------上次被请求的日期之后被修改过--------------
                    if (request.Headers["If-Range"].Replace("\"", "") != fileMd5)//文件修改过
                    {
                        response.StatusCode = 412; //预处理失败
                        return false;
                    }
                }

                var fileNameEncoding = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                if (string.IsNullOrEmpty(fileNameEncoding))
                {
                    response.StatusCode = 412; //预处理失败
                    return false;
                }
                #endregion

                try
                {
                    #region -------添加重要响应头、解析请求头、相关验证-------------------


                    response.Clear();
                    response.Buffer = false;
                    response.AddHeader("Content-MD5", fileMd5); //用于验证文件
                    response.AddHeader("Accept-Ranges", "bytes"); //重要：续传必须
                    response.AppendHeader("ETag", "\"" + fileMd5 + "\""); //重要：续传必须
                    //response.AppendHeader("Last-Modified", lastUpdateTiemStr);//把最后修改日期写入响应 
                    response.ContentType = "application/octet-stream"; //MIME类型：匹配任意文件类型
                    response.AddHeader("Content-Disposition", "attachment;filename=" +
                                                              fileNameEncoding.Replace("+", "%20"));
                    response.AddHeader("Content-Length", (fileLength - startBytes).ToString(CultureInfo.InvariantCulture));
                    response.AddHeader("Connection", "Keep-Alive");
                    response.ContentEncoding = Encoding.UTF8;
                    if (request.Headers["Range"] != null)
                    {
                        //------如果是续传请求，则获取续传的起始位置，即已经下载到客户端的字节数------
                        response.StatusCode = 206; //重要：续传必须，表示局部范围响应。初始下载时默认为200
                        string[] range = request.Headers["Range"].Split(new[] { '=', '-' }); //"bytes=1474560-"
                        startBytes = Convert.ToInt64(range[1]); //已经下载的字节数，即本次下载的开始位置 
                        if (startBytes < 0 || startBytes >= fileLength)
                        {
                            //无效的起始位置
                            return false;
                        }
                    }
                    if (startBytes > 0)
                    {
                        //------如果是续传请求，告诉客户端本次的开始字节数，总长度，以便客户端将续传数据追加到startBytes位置后----------
                        response.AddHeader("Content-Range",
                                           string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }

                    #endregion

                    #region -------向客户端发送数据块-------------------

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    var maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize); //分块下载，剩余部分可分成的块数
                    for (int i = 0; i < maxCount && response.IsClientConnected; i++)//客户端中断连接，则暂停
                    {

                        response.BinaryWrite(br.ReadBytes(packSize));
                        response.Flush();
                        if (sleep > 1)
                            Thread.Sleep(sleep);
                    }

                    #endregion
                }
                catch
                {
                    return false;
                }
                finally
                {
                    stream.Dispose();
                }
            }

            return true;
        }
    }
}