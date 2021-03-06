﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Web.Tools
{
    /// <summary>
    /// Plupload组件文件上传辅助实现类
    /// </summary>
    public abstract class PluploadHandler
    {
        private static string GetAssemblyPath()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
            path = path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return path + "temp" + Path.DirectorySeparatorChar;

        }
        /// <summary>
        /// 默认文件上传根路径 ，默认值为当前程序集所有目录下的temp目录
        /// </summary>
        public static string RootPath { get; set; } = GetAssemblyPath();
        /// <summary>
        /// 临时文件夹名
        /// </summary>
        protected abstract string TempFileName { get; }
        /// <summary>
        /// 检验文件扩展名是否有效
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>有效返回true</returns>
        protected virtual bool IsValid(string fileName)
        {
            string[] strs = fileName.ToLower().Split('.');
            if (strs.Length >= 2)
            {
                return AllowExtension.Contains(strs[strs.Length - 1]);
            }
            return false;
        }

        /// <summary>
        /// 允许上传的文件扩展名定义
        /// 注意，正确格式如下：jpg,png
        /// 错误方式：.jpg,.png,.gif
        /// 区别大小写
        /// </summary>
        protected virtual List<string> AllowExtension
        {
            get
            {
                return new List<string> { "jpg", "png", "gif" };
            }
        }
        /// <summary>
        /// 文件夹名
        /// </summary>
        protected virtual string FolderId
        {
            get { return "f"; }
        }


        /// <summary>
        /// 通过实现 <see cref="T:System.Web.IHttpHandler"/> 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context"><see cref="T:System.Web.HttpContext"/> 对象，它提供对用于为 HTTP 请求提供服务的内部服务器对象（如 Request、Response、Session 和 Server）的引用。</param>
        /// <returns>返回 0 保存成功，返回 1表示保存成功并且文件上传完成，返回小于0表示出错：-1参数不正确，-2 文件不满足要求 -3 出现异常</returns>
        protected virtual int ProcessRequest(HttpContext context, string folder = null)
        {
            context.Response.ContentType = "text/plain";
            //context.Response. = "utf-8";
            context.Response.Headers.Add("charset", "utf-8");

            var request = context.Request;
            int chunk;
            int chunks;
            if (string.IsNullOrEmpty(folder))
            {
                folder = request.Query[FolderId];
            }
            string fileName = null;

            if (!CheckParams(request, out chunk, out chunks, ref fileName))
            {
                return -1;
            }

            if (!IsValid(fileName))
            {
                return -2;
            }

            try
            {
                var strServerPath = CreateFilePath(folder, fileName);
                SaveContent(request, strServerPath, chunk, chunks);
                return chunk + 1 == chunks ? 1 : 0;
            }
            catch (IOException ioEx)
            {
                LogException("该目录只读", ioEx);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogException("该路径下无安全权限", accessEx);
            }
            return -3;
        }
        #region 静态类辅助方法
        private volatile static ConcurrentDictionary<Type, List<string>> _dicAllowExtension = new ConcurrentDictionary<Type, List<string>>();
        private volatile static ConcurrentDictionary<Type, string> _dicTemplateFoler = new ConcurrentDictionary<Type, string>();
        private static readonly object ObSync = new object();
        private static string[] GetFiles<T>(string targetDir, out string targetFolder) where T : PluploadHandler, new()
        {
            Type type = typeof(T);
            if (!_dicAllowExtension.ContainsKey(type))
            {
                lock (ObSync)
                {
                    if (!_dicAllowExtension.ContainsKey(type))
                    {
                        var list = new T();
                        _dicTemplateFoler.AddOrUpdate(type, list.TempFileName, (key, value) => value);
                        _dicAllowExtension.AddOrUpdate(type, list.AllowExtension, (key, value) => value);
                    }
                }
            }

            var list1 = new List<string>();
            var targetFolder1 = CreateTargetDir(_dicTemplateFoler[type], targetDir);
            if (Directory.Exists(targetFolder1))
            {
                _dicAllowExtension[type].ForEach(p => list1.AddRange(Directory.GetFiles(targetFolder1, "*." + p)));
            }

            targetFolder = targetFolder1;
            return list1.ToArray();
        }

        private static void DeleteDirectory(string targetDir, Action<Exception> errorAction)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(p =>
            {
                try
                {
                    if (Directory.Exists(targetDir))
                    {
                        Directory.Delete(targetDir, true);
                    }
                }
                catch (Exception ioEx)//IO错误，直接忽略
                {
                    if (errorAction != null)
                    {
                        errorAction(ioEx);
                    }
                }
            });
        }
        /// <summary>
        /// 创建目标目录
        /// </summary>
        /// <param name="request">上下文</param>
        /// <param name="tempFloder">临时目录名</param>
        /// <param name="targetDir">目标路径</param>
        /// <returns>返回拼接好的目录路径。</returns>
        protected static string CreateTargetDir(string tempFloder, string targetDir)
        {
            return string.Format("{0}{1}{2}{3}{2}", RootPath, tempFloder,
                Path.DirectorySeparatorChar, targetDir);
        }
        #endregion
        /// <summary>
        /// 处理上传文件
        /// </summary>
        /// <param name="fileUploadDelegate">文件处理回调</param>
        /// <param name="targetDir">文件目录</param>
        /// <typeparam name="T">Handle类型</typeparam>
        public static void UploadFile<T>(Action<string[]> fileUploadDelegate, string targetDir) where T : PluploadHandler, new()
        {
            UploadFile<T>(fileUploadDelegate, targetDir, null);
        }

        /// <summary>
        /// 处理上传文件
        /// </summary>
        /// <param name="fileUploadDelegate">文件处理回调</param>
        /// <param name="targetDir">文件目录</param>
        /// <param name="errorAction">出错Action</param>
        /// <typeparam name="T">Handle类型</typeparam>
        public static void UploadFile<T>(Action<string[]> fileUploadDelegate, string targetDir, Action<Exception> errorAction) where T : PluploadHandler, new()
        {
            string targetFloder;
            var list = GetFiles<T>(targetDir, out targetFloder);
            if (fileUploadDelegate != null)
            {
                try
                {
                    fileUploadDelegate(list);
                }
                finally
                {
                    DeleteDirectory(targetFloder, errorAction);
                }
            }
            else
            {
                DeleteDirectory(targetFloder, errorAction);
            }

        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="folder">如果输入值为空，表示使用folderId从Query中取参数</param>
        /// <returns></returns>
        public static int ProcessRequest<T>(HttpContext context, string folder = null) where T : PluploadHandler, new()
        {
            T handler = new T();
            return handler.ProcessRequest(context, folder);
        }

        #region 私有方法

        private string CreateFilePath(string targetDir, string fileName)
        {
            var strServerDir = string.Format("{0}{1}{2}{3}{2}", RootPath, TempFileName, Path.DirectorySeparatorChar, targetDir);
            if (!Directory.Exists(strServerDir))
            {
                var dirInfo = Directory.CreateDirectory(strServerDir);
                dirInfo.Attributes = FileAttributes.Hidden;
            }

            return Path.Combine(strServerDir, fileName);
        }
        private static void SaveContent(HttpRequest request, string filePath, int chunk, int chunks)
        {
            var source = GetSourceStream(request);

            if (source == null)
            {
                return;
            }

            using (var fs = new FileStream(chunk == -1 ? filePath : GetTempFileName(filePath), chunk == 0 || chunk == -1 ? FileMode.Create : FileMode.Append))
            {
                CopyStream(source, fs);
            }

            if (chunks == chunk + 1)//上传完成，修正文件名称
            {
                File.Move(GetTempFileName(filePath), filePath);
            }
        }

        private static Stream GetSourceStream(HttpRequest request)
        {
            if (request.ContentType == "application/octet-stream" && request.ContentLength > 0)
            {
                return request.Body;
            }

            if (request.ContentType.Contains("multipart/form-data") && request.Form.Files.Count > 0 && request.Form.Files[0].Length > 0)
            {
                return request.Form.Files[0].OpenReadStream();
            }

            return null;
        }

        private static void CopyStream(Stream source, Stream target)
        {
            var buffer = new byte[1024 * 1024];
            while (true)
            {
                var currentReadLength = source.Read(buffer, 0, buffer.Length);
                if (currentReadLength <= 0)
                {
                    break;
                }

                target.Write(buffer, 0, currentReadLength);
            }
        }
        private static bool CheckParams(HttpRequest request, out int chunk, out int chunks, ref string name)
        {

            if (!int.TryParse(request.Form["chunk"], out chunk))
            {
                chunk = -1;
            }

            if (!int.TryParse(request.Form["chunks"], out chunks))
            {
                chunks = -2;
            }

            if (chunk < 0 || chunks < 0 || chunk >= chunks)
            {
                return false;
            }

            if (string.IsNullOrEmpty(request.Form["name"]))
            {
                return false;
            }

            name = request.Form["name"];
            return true;
        }
        private static string GetTempFileName(string filePath)
        {
            return filePath + ".part";
        }
        #endregion

        /// <summary>
        /// 记录日志，默认实现不写入日志
        /// </summary>
        /// <param name="exDescription">异常描述</param>
        /// <param name="ex">异常</param>
        protected virtual void LogException(string exDescription, Exception ex)
        {

        }
    }
}
