using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Web
{
    /// <summary>
    /// 用于动态替换Header中的CSS和JS加入动态版本号
    /// 版本号自动使用AppSettings中设置的项：ResourceVersion
    /// 如果未设置，自动取当前时间生成一个版本
    /// 备注：只能替换在header中注入的脚本和CSS文件
    /// </summary>
    public class ScriptCssPageAdapter : System.Web.UI.Adapters.PageAdapter
    {
        static ScriptCssPageAdapter()
        {
            Version = System.Configuration.ConfigurationManager.AppSettings["ResourceVersion"];
            if (string.IsNullOrEmpty(Version))
            {
                Version = DateTime.Now.Date.Ticks.ToString(CultureInfo.InvariantCulture);
            }
        }
        private static readonly string Version;

        /// <summary>
        /// 重写关联控件的 <see cref="M:System.Web.UI.Control.OnPreRender(System.EventArgs)"/> 方法。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.EventArgs"/>。</param>
        protected override void OnPreRender(EventArgs e)//css,htmllink 
        {
            foreach (var link in Page.Header.Controls.OfType<HtmlLink>().ToList()
                .Where(link => link.Attributes["type"].EqualsIgnoreCase("text/css"))
                .Where(link => link.Attributes["href"].ContainsIgnoreCase("/App_Themes/{0}/".Fill(Page.Theme))))
            {
                link.Href += string.Format("?t={0}", Version);
            }

            foreach (var link in Page.Header.Controls.OfType<LiteralControl>().ToList()) //script,LiteralControl 
            {
                if (link.Text.ContainsIgnoreCase("text/javascript"))
                    if (link.Text.ContainsIgnoreCase("<script"))
                        link.Text = link.Text.ReplaceIgnoreCase(".js", ".js?t={0}".Fill(Version));
            }

            base.OnPreRender(e);
        }

    }
}
