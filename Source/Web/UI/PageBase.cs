using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Zhoubin.Infrastructure.Common;
using Zhoubin.Infrastructure.Web.Extent;

namespace Zhoubin.Infrastructure.Web.UI
{
    /// <summary>
    /// 公共页面基类，提供相关ShowMessage方法
    /// 所有同步异步方法都统一到ShowMessage名称
    /// </summary>
    public abstract class PageBase : Page
    {
        /// <summary>
        /// 当前ScriptManager 实例
        /// 
        /// </summary>
        protected ScriptManager CurrentScriptManager { 
            get 
        { return ScriptManager.GetCurrent(this); } }

        /// <summary>
        /// 当前是否处于异步加载模式
        /// </summary>
        public bool IsInAsyncPostBack
        {
            get
            {
                ScriptManager manage = ScriptManager.GetCurrent(this);
                if (manage != null)
                {
                    return manage.IsInAsyncPostBack;
                }
                return false;
            }
        }

        /// <summary>
        /// 启用登录用户令牌检查
        /// </summary>
        protected virtual bool EnableLoginCheck { get { return true; } }
        /// <summary>
        /// 创建用户登录令牌
        /// </summary>
        /// <param name="request">HttpRequest对象</param>
        /// <returns>返回令牌字符串</returns>
        protected virtual string CreateLoginToken(HttpRequest request)
        {
            return Page.User.Identity.Name;
        }

        /// <summary>
        /// 检查用户登录令牌
        /// </summary>
        /// <param name="request">HttpRequest对象</param>
        /// <returns>检查成功返回true</returns>
        protected virtual bool CheckLoginToken(HttpRequest request)
        {
            return true;
        }

        void ProcessRequest(bool isRequest)
        {
            if (!EnableLoginCheck)
            {
                return ;
            }

            var request = Request;
            if (!CheckLoginToken(request))
            {
                return;
            }

            if (isRequest)
            {
                if (request.HttpMethod == "GET")
                {
                    return;
                }

                string tokenKey = GetLoginToken();
                if (string.IsNullOrEmpty(tokenKey))
                {
                    throw new InfrastructureException("启用登录Token的情况下，TokenKey为空；如果禁用Token，请设置属性EnableLoginCheck为false。");
                }

                if (CreateLoginToken(request) != tokenKey)
                {
                    throw new InfrastructureException("页面内容已经变更，此页面已经失效。");
                }
            }
            else
            {
                SaveLoginToken();
            }
        }

        /// <summary>
        /// 保存登录令牌
        /// </summary>
        protected virtual void SaveLoginToken()
        {
            if (EnableViewState)
            {
                ViewState[TokenKey] = CreateLoginToken(Request);
            }

            ClientScript.RegisterHiddenField(TokenKey, CreateLoginToken(Request));
        }

        /// <summary>
        /// 获取登录令牌
        /// </summary>
        /// <returns>返回令牌</returns>
        protected virtual string GetLoginToken()
        {
            if (EnableViewState)
            {
                return ViewState[TokenKey] == null ? null : ViewState[TokenKey].ToString();
            }

            return Request.Form.Get(TokenKey);
        }

        /// <summary>
        /// 登录令牌Key
        /// </summary>
        protected  virtual string TokenKey { get { return "LoginToken"; } }

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.Init"/> 事件以对页进行初始化。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.EventArgs"/>。</param>
        protected override void OnInit(EventArgs e)
        {
            
            base.OnInit(e);
        }

        /// <summary>
        /// 在回发数据已加载到页服务器控件之后但在 <see cref="M:System.Web.UI.Control.OnLoad(System.EventArgs)"/> 事件之前，引发 <see cref="E:System.Web.UI.Page.PreLoad"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.EventArgs"/>。</param>
        protected override void OnPreLoad(EventArgs e)
        {
            ProcessRequest(true);
            base.OnPreLoad(e);
        }

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.PreRender"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/> 对象。</param>
        protected override void OnPreRender(EventArgs e)
        {
            ProcessRequest(false);
            base.OnPreRender(e);
        }

        /// <summary>
        /// 自定义弹出框脚本目录
        /// </summary>
        protected virtual string SmokeFolder
        {
            get { return ""; }
        }
        /// <summary>
        /// 添加CSS
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="page">页面</param>
        static void AddCss(string path, Page page)
        {
            var cssFile = new Literal { Text = @"<link href=""" + page.ResolveUrl(path) + @""" type=""text/css"" rel=""stylesheet"" />" };
            page.Header.Controls.Add(cssFile);
        }

        /// <summary>
        /// 添加JavaScript
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="page">页面</param>
        static void AddJavaScript(string path, Page page)
        {
            var cssFile = new Literal { Text = @"<script src=""" + page.ResolveUrl(path) + @""" type=""text/javascript""/>" };
            page.Header.Controls.Add(cssFile);
        }

        /// <summary>
        /// 在 <see cref="M:System.Web.UI.Page.OnPreRenderComplete(System.EventArgs)"/> 事件之后但在呈现页之前引发 <see cref="E:System.Web.UI.Page.PreRenderComplete"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.EventArgs"/>。</param>
        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (Utility.CustomAlert)
            {
                AddCss(
                    string.IsNullOrEmpty(SmokeFolder)
                        ? ClientScript.GetWebResourceUrl(typeof(PageBase), ConstHelper.AlertCssPath)
                        : string.Format("{0}/{1}", SmokeFolder.TrimEnd('/'), "smoke.css"), this);

                ClientScript.RegisterClientScriptResource(typeof(PageBase), ConstHelper.AlertJavaScriptFilePath);
            }
            base.OnPreRenderComplete(e);
        }

        /// <summary>
        /// 显示异步消息
        /// </summary>
        /// <param name="updatePanel">UpdatePanel对象</param>
        /// <param name="type">类型</param>
        /// <param name="message">消息内容</param>
        /// <param name="url">跳转地址</param>
        protected void ShowMessage(UpdatePanel updatePanel, Type type, string message, string url = null)
        {
            updatePanel.ShowMessage(type, message, url);
        }

        /// <summary>
        /// 自动显示信息并关闭
        /// </summary>
        /// <param name="updatePanel">UpdatePanel对象</param>
        /// <param name="type">类型</param>
        /// <param name="message">消息内容</param>
        /// <param name="timeOut">延时关闭时间，单位：秒</param>
        /// <param name="url">跳转Url</param>
        protected void AutoCloseMessage(UpdatePanel updatePanel, Type type, string message, int timeOut, string url = null)
        {
            Utility.ShowMessageAndAutoClose(updatePanel, type, message, url);
        }

        /// <summary>
        /// 自动显示信息并关闭
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="timeOut">延时关闭时间，单位：秒</param>
        /// <param name="url">跳转Url</param>
        protected void AutoCloseMessage(string message, int timeOut, string url = null)
        {
            Utility.ShowMessageAndAutoClose(this, Page.GetType(), message, url);
        }
        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="panel">UpdatePanel对象</param>
        /// <param name="type">类型</param>
        /// <param name="script">脚本</param>
        protected void RegisterScript(UpdatePanel panel, Type type, string script)
        {
            panel.RegisterScript(type, script);
        }
        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="url">跳转Url</param>
        protected void ShowMessage(string message, string url = null)
        {
            Page.ShowMessage(Page.GetType(), message, url);
        }
        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="script">脚本</param>
        protected void RegisterScript(string script)
        {
            Page.RegisterScript(Page.GetType(), script);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        protected void DownloadFile(Stream stream, string fileName, string contentType = null)
        {
            Page.DownloadFile(stream, fileName, contentType);
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="ob">待截取对象</param>
        /// <param name="length">截取长度</param>
        /// <returns>当对象为null时，返回String.Empty;当字符串长度小于等于待截取长度时，返回原字符串；当字符串大于截取长度时，返回待截取长度减去2的字符串</returns>
        protected string GetSubString(object ob, int length)
        {
            return Page.GetSubString(ob, length);
        }
    }
}
