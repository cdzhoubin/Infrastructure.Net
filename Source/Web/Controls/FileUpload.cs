using System.ComponentModel;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.Controls
{
    /// <summary>
    /// 单个文件上传控件
    /// </summary>
    [DefaultProperty("FileListId")]
    [ToolboxData("<{0}:FileUpload runat=server></{0}:FileUpload>")]
    public class FileUpload : FileUploadBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileUpload()
            : base(true)
        {
        }

        private const string JavaScriptTemplate = @"<script type='text/javascript'>
var {0};
$(function(){{{0}Load();}});
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
{0}Config.BrowButtonId = '{17}';
{0}Config.FileListId = '{18}';
{0} =new SingleFileUpload({0}Config);
}}
        </script>";
        
        /// <summary>
        /// 输出控件内容
        /// </summary>
        /// <param name="output">输出对象</param>
        protected override void WriteContents(HtmlTextWriter output)
        {
            output.Write(JavaScriptTemplate, FileUploadObjectName, ContainerId, Entity.HandlerPath, Entity.FolderId, UploadPath, Entity.RunTypes, Entity.MaxFileSize, Entity.ChunkSize, Entity.ImageUpload ? Entity.ImageWidth : 0, Entity.ImageUpload ? Entity.ImageHeight : 0, Entity.PlUploadFloder, "false", Entity.FileTypeList, string.IsNullOrEmpty(UploadCompleteCallBack) ? "null" : UploadCompleteCallBack, string.IsNullOrEmpty(UploadErrorCallBack) ? "null" : UploadErrorCallBack, string.IsNullOrEmpty(UploadProgressCallBack) ? "null" : UploadProgressCallBack, string.IsNullOrEmpty(AddFileCallBack) ? "null" : AddFileCallBack, BrowButtonId, FileListId);
            output.Write(Entity.SingleTemplate, ContainerId, BrowButtonId, FileListId);

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
            Page.ClientScript.RegisterClientScriptInclude("PluploadQuery", string.Format("{0}/jquery.plupload.queue/jquery.plupload.queue.js", Entity.PlUploadFloder));
        }
    }

    /// <summary>
    /// 文件上传控件2.0
    /// </summary>
    /// 
    [DefaultProperty("FileListId")]
    [ToolboxData("<{0}:FileUpload2 runat=server></{0}:FileUpload2>")]
    public class FileUpload2 : FileUpload2Base
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileUpload2()
            : base(true)
        {
        }

        private const string JavaScriptTemplate = @"<script type='text/javascript'>
var {0};
$(function(){{{0}Load();}});
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
{0}Config.BrowButtonId = '{17}';
{0}Config.FileListId = '{18}';
{0} =new SingleFileUpload({0}Config);
}}
        </script>";
        /// <summary>
        /// 输出控件内容
        /// </summary>
        /// <param name="output">输出对象</param>
        protected override void WriteContents(HtmlTextWriter output)
        {
            output.Write(JavaScriptTemplate, FileUploadObjectName, ContainerId, Entity.HandlerPath, Entity.FolderId, UploadPath, Entity.RunTypes, Entity.MaxFileSize, Entity.ChunkSize, Entity.ImageUpload ? Entity.ImageWidth : 0, Entity.ImageUpload ? Entity.ImageHeight : 0, Entity.PlUploadFloder, "false", Entity.FileTypeList, string.IsNullOrEmpty(UploadCompleteCallBack) ? "null" : UploadCompleteCallBack, string.IsNullOrEmpty(UploadErrorCallBack) ? "null" : UploadErrorCallBack, string.IsNullOrEmpty(UploadProgressCallBack) ? "null" : UploadProgressCallBack, string.IsNullOrEmpty(AddFileCallBack) ? "null" : AddFileCallBack, BrowButtonId, FileListId);
            output.Write(Entity.SingleTemplate, ContainerId, BrowButtonId, FileListId);

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
            Page.ClientScript.RegisterClientScriptInclude("PluploadQuery", string.Format("{0}/jquery.plupload.queue/jquery.plupload.queue.min.js", Entity.PlUploadFloder));
        }
    }
}
