using System;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.Extent
{
    /// <summary>
    /// UpdatePanel 扩展方法
    /// </summary>
    public static class UpdatePanelExtent
    {
        /// <summary>
        /// 用JS显示消息
        /// 使用JavaScript中的alert方法
        /// </summary>
        /// <param name="updatePanel">页面实例</param>
        /// <param name="type">控件类型</param>
        /// <param name="message">消息内容</param>
        /// <param name="url">如果不为空，显示提示信息，自动跳转页面到此地址</param>
        public static void ShowMessage(this UpdatePanel updatePanel, Type type, string message, string url = null)
        {
            Utility.ShowMessage(updatePanel, type, message, url);
        }
        /// <summary>
        /// 注册一段脚本到页面
        /// </summary>
        /// <param name="panel">页面实例</param>
        /// <param name="type">控件类型</param>
        /// <param name="script">脚本内容</param>
        public static void RegisterScript(this UpdatePanel panel, Type type, string script)
        {
            Utility.RegisterScript(panel, type, script);
        }
    }
}