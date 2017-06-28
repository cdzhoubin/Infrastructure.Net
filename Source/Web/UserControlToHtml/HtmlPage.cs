using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.UserControlToHtml
{
    /// <summary>
    /// 此类为生成Html代码
    /// 页面工具类
    /// </summary>
    sealed class  HtmlPage:Page
    {
        protected override void InitOutputCache(OutputCacheParameters cacheSettings)
        {
            cacheSettings.Enabled = true;
            base.InitOutputCache(cacheSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {
            
        }
    }
}