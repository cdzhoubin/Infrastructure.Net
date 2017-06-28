using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Zhoubin.Infrastructure.Web.Config;

namespace Zhoubin.Infrastructure.Web.Controls
{
    /// <summary>
    /// 文件上传基类
    /// </summary>
    public abstract class FileUploadBase : WebControl
    {
        private readonly bool _singleFile;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="singleFile">是否单个文件</param>
        protected FileUploadBase(bool singleFile)
        {
            _singleFile = singleFile;
        }
        readonly FileUploadHelper _helper = new FileUploadHelper();
        /// <summary>
        /// 上传配置
        /// </summary>
        protected UploadEntity Entity { get { return _helper.UploadConfig.UploadList.FirstOrDefault(p => p.Name == ConfigName); } }


        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.Init"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnInit(EventArgs e)
        {
            Page.PreLoad += Page_PreLoad;
        }

        void Page_PreLoad(object sender, EventArgs e)
        {
           if(!_singleFile)
           {
               AddLink(string.Format("{0}/jquery.plupload.queue/css/jquery.plupload.queue.css", Entity.PlUploadFloder));
           }
        }

        private void AddLink(string url)
        {
            var link = new HtmlLink { Href = url };
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            Page.Header.Controls.Add(link);
        }
        /// <summary>
        /// 配置名称，必须设置
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 上传控件窗口编号
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        /// 文件上传对象JS对象名称
        /// </summary>
        public string FileUploadObjectName
        {
            get { return string.Format("{0}fileupload", ClientID); }
        }

        /// <summary>
        /// 初始化控件JS函数名称
        /// </summary>
        public string InitFunctionName
        {
            get { return string.Format("{0}fileupload{1}", ClientID,"Load"); }
        }
        /// <summary>
        /// 文件上传路径（目录）
        /// </summary>
        public string UploadPath { get; set; }


        /// <summary>
        /// 添加文件回调
        /// </summary>
        public string AddFileCallBack { get; set; }
        /// <summary>
        /// 进度回调
        /// </summary>
        public string UploadProgressCallBack { get; set; }
        /// <summary>
        /// 完成回调
        /// </summary>
        public string UploadCompleteCallBack { get; set; }
        /// <summary>
        /// 错误回调
        /// </summary>
        public string UploadErrorCallBack { get; set; }

        /// <summary>
        /// 将控件的内容呈现到指定的编写器中。此方法主要由控件开发人员使用。
        /// </summary>
        /// <param name="output"><see cref="T:System.Web.UI.HtmlTextWriter"/>，表示要在客户端呈现 HTML 内容的输出流。</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (DesignMode)
            {
                output.Write("{0}文件上传：{1}", _singleFile ? "单" : "多", ID);
                return;

            }

            if (Entity == null)
            {
                throw new Exception("文件上传配置名称不存在。");
            }
            WriteContents(output);
        }


        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.PreRender"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("Plupload", string.Format("{0}/plupload.full.js", Entity.PlUploadFloder));
            Page.ClientScript.RegisterClientScriptInclude("PluploadLang", string.Format("{0}/cn.js", Entity.PlUploadFloder));
            if(!_singleFile)
            {
                
                //Page.ClientScript.RegisterClientScriptInclude("PluploadUI", string.Format("{0}/jquery.ui.plupload/jquery.ui.plupload.js", Entity.PlUploadFloder));
                Page.ClientScript.RegisterClientScriptInclude("PluploadQuery", string.Format("{0}/jquery.plupload.queue/jquery.plupload.queue.js", Entity.PlUploadFloder));
            }

            Page.ClientScript.RegisterClientScriptResource(typeof(FileUpload), ConstHelper.FileUploadPath);
            RegisterScript();
            base.OnPreRender(e);
        }

        /// <summary>
        /// 注册脚本
        /// </summary>
        protected abstract void RegisterScript();

        /// <summary>
        /// 输出控件内容
        /// </summary>
        /// <param name="output">输出对象</param>
        protected abstract void WriteContents(HtmlTextWriter output);
    }

    /// <summary>
    /// 文件上传基类
    /// </summary>
    public abstract class FileUpload2Base : WebControl
    {
        private readonly bool _singleFile;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="singleFile">是否单个文件上传</param>
        protected FileUpload2Base(bool singleFile)
        {
            _singleFile = singleFile;
        }
        readonly FileUploadHelper _helper = new FileUploadHelper();

        /// <summary>
        /// 上传配置
        /// </summary>
        protected UploadEntity Entity
        {
            get
            {
                return _helper.UploadConfig.UploadList.FirstOrDefault(p => p.Name == ConfigName);
            }
        }


        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.Init"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnInit(EventArgs e)
        {
            Page.PreLoad += Page_PreLoad;
        }

        void Page_PreLoad(object sender, EventArgs e)
        {
            if (!_singleFile)
            {
                AddLink(string.Format("{0}/jquery.plupload.queue/css/jquery.plupload.queue.css", Entity.PlUploadFloder));
            }
        }

        private void AddLink(string url)
        {
            var link = new HtmlLink { Href = url };
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            Page.Header.Controls.Add(link);
        }
        /// <summary>
        /// 配置名称，必须设置
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 上传控件窗口编号
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        /// 文件上传对象JS对象名称
        /// </summary>
        public string FileUploadObjectName
        {
            get { return string.Format("{0}fileupload", ClientID); }
        }

        /// <summary>
        /// 初始化控件JS函数名称
        /// </summary>
        public string InitFunctionName
        {
            get { return string.Format("{0}fileupload{1}", ClientID, "Load"); }
        }
        /// <summary>
        /// 文件上传路径（目录）
        /// </summary>
        public string UploadPath { get; set; }


        /// <summary>
        /// 添加文件回调
        /// </summary>
        public string AddFileCallBack { get; set; }
        /// <summary>
        /// 进度回调
        /// </summary>
        public string UploadProgressCallBack { get; set; }
        /// <summary>
        /// 完成回调
        /// </summary>
        public string UploadCompleteCallBack { get; set; }
        /// <summary>
        /// 错误回调
        /// </summary>
        public string UploadErrorCallBack { get; set; }

        /// <summary>
        /// 将控件的内容呈现到指定的编写器中。此方法主要由控件开发人员使用。
        /// </summary>
        /// <param name="output"><see cref="T:System.Web.UI.HtmlTextWriter"/>，表示要在客户端呈现 HTML 内容的输出流。</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (DesignMode)
            {
                output.Write("{0}文件上传：{1}", _singleFile ? "单" : "多", ID);
                return;

            }

            if (Entity == null)
            {
                throw new Exception("文件上传配置名称不存在。");
            }
            WriteContents(output);
        }

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.PreRender"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("Plupload", string.Format("{0}/plupload.full.min.js", Entity.PlUploadFloder));
            Page.ClientScript.RegisterClientScriptInclude("PluploadLang", string.Format("{0}/i18n/zh_CN.js", Entity.PlUploadFloder));
            if (!_singleFile)
            {

                //Page.ClientScript.RegisterClientScriptInclude("PluploadUI", string.Format("{0}/jquery.ui.plupload/jquery.ui.plupload.js", Entity.PlUploadFloder));
                Page.ClientScript.RegisterClientScriptInclude("PluploadQuery", string.Format("{0}/jquery.plupload.queue/jquery.plupload.queue.min.js", Entity.PlUploadFloder));
            }

            Page.ClientScript.RegisterClientScriptResource(typeof(FileUpload), ConstHelper.FileUploadPath2);
            RegisterScript();
            base.OnPreRender(e);
        }

        /// <summary>
        /// 注册脚本
        /// </summary>
        protected abstract void RegisterScript();

        /// <summary>
        /// 输出控件内容
        /// </summary>
        /// <param name="output">输出对象</param>
        protected abstract void WriteContents(HtmlTextWriter output);
    }
}