using System;
using System.ComponentModel;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.Controls
{
    /// <summary>
    /// 多文件上传
    /// </summary>
    [DefaultProperty("HandlerPath")]
    [ToolboxData("<{0}:MultFileUpload runat=server></{0}:MultFileUpload>")]
    public class MultFileUpload : FileUploadBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MultFileUpload()
            : base(false)
        {
        }

        private const string JavaScriptTemplate = @"<script type='text/javascript'>
var {0};
$(function () {{
{0}Load();
}});
function {0}Load() {{
var {0}Config = new UploadConfig();
{0}Config.ContainerId = '{1}'; 
{0}Config.Url = '{2}?{3}={4}'; 
{0}Config.RunTimeTypes = '{5}'; 
{0}Config.MaxFileSize = '{6}'; 
{0}Config.ChunkSize = '{7}';
{0}Config.Width = {8};
{0}Config.Heigh = {9};
{0}Config.PlUploadFolder = '{10}'; 
{0}Config.UniqueNames = {11}; 
{0}Config.Filters = {12}; 
{0}Config.CompleteEvent = {13};
{0}Config.ErrorEvent = {14};
{0}Config.ProgressEvent = {15};
{0}Config.AddedFileEvent = {16};
{0} =new PluploadProcessFile({0}Config);
}}
        </script>";


        /// <summary>
        /// 输出控件内容
        /// </summary>
        /// <param name="output">输出对象</param>
        protected override void WriteContents(HtmlTextWriter output)
        {
            output.Write(JavaScriptTemplate, FileUploadObjectName, ContainerId, Entity.HandlerPath, Entity.FolderId, UploadPath, Entity.RunTypes, Entity.MaxFileSize, Entity.ChunkSize, Entity.ImageUpload ? Entity.ImageWidth : 0, Entity.ImageUpload ? Entity.ImageHeight : 0, Entity.PlUploadFloder, "false", Entity.FileTypeList, string.IsNullOrEmpty(UploadCompleteCallBack) ? "null" : UploadCompleteCallBack, string.IsNullOrEmpty(UploadErrorCallBack) ? "null" : UploadErrorCallBack, string.IsNullOrEmpty(UploadProgressCallBack) ? "null" : UploadProgressCallBack, string.IsNullOrEmpty(AddFileCallBack) ? "null" : AddFileCallBack);
            output.Write(Entity.MoreTemplate, ContainerId);
        }


        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.PreRender"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnPreRender(EventArgs e)
        {
            RegisterScript();
            base.OnPreRender(e);
        }

        #region 文件



        /// <summary>
        /// 文件列表编号
        /// </summary>
        public string FileListId { get; set; }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public string BrowButtonId { get; set; }

        #endregion

        /// <summary>
        /// 注册脚本
        /// </summary>
        protected override void RegisterScript()
        {
            
        }
    }

    /// <summary>
    /// Plupload2.0控件封装实现
    /// </summary>
    [DefaultProperty("HandlerPath")]
    [ToolboxData("<{0}:MultFileUpload2 runat=server></{0}:MultFileUpload2>")]
    public class MultFileUpload2 : FileUpload2Base
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MultFileUpload2()
            : base(false)
        {
        }

        private const string JavaScriptTemplate = @"<script type='text/javascript'>
var {0};
$(function () {{
{0}Load();
}});
function {0}Load() {{
var {0}Config = new UploadConfig();
{0}Config.ContainerId = '{1}'; 
{0}Config.Url = '{2}?{3}={4}'; 
{0}Config.RunTimeTypes = '{5}'; 
{0}Config.MaxFileSize = '{6}'; 
{0}Config.ChunkSize = '{7}';
{0}Config.Width = {8};
{0}Config.Heigh = {9};
{0}Config.PlUploadFolder = '{10}'; 
{0}Config.UniqueNames = {11}; 
{0}Config.Filters = {12}; 
{0}Config.CompleteEvent = {13};
{0}Config.ErrorEvent = {14};
{0}Config.ProgressEvent = {15};
{0}Config.AddedFileEvent = {16};
{0} =new PluploadProcessFile({0}Config);
}}
        </script>";

        /// <summary>
        /// 输出控件内容
        /// </summary>
        /// <param name="output">输出对象</param>
        protected override void WriteContents(HtmlTextWriter output)
        {
            output.Write(JavaScriptTemplate, FileUploadObjectName, ContainerId
                , Entity.HandlerPath, Entity.FolderId, UploadPath, Entity.RunTypes
                , Entity.MaxFileSize, Entity.ChunkSize, Entity.ImageUpload ? Entity.ImageWidth : 0
                , Entity.ImageUpload ? Entity.ImageHeight : 0, Entity.PlUploadFloder, "false"
                , Entity.FileTypeList, string.IsNullOrEmpty(UploadCompleteCallBack) ? "null" : UploadCompleteCallBack
                , string.IsNullOrEmpty(UploadErrorCallBack) ? "null" : UploadErrorCallBack
                , string.IsNullOrEmpty(UploadProgressCallBack) ? "null" : UploadProgressCallBack
                , string.IsNullOrEmpty(AddFileCallBack) ? "null" : AddFileCallBack);
            output.Write(Entity.MoreTemplate, ContainerId);
        }


        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.PreRender"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnPreRender(EventArgs e)
        {
            RegisterScript();
            base.OnPreRender(e);
        }

        #region 文件



        /// <summary>
        /// 文件列表编号
        /// </summary>
        public string FileListId { get; set; }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public string BrowButtonId { get; set; }

        #endregion

        /// <summary>
        /// 注册脚本
        /// </summary>
        protected override void RegisterScript()
        {
            
        }
    }
}