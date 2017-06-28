using System.IO;
using System.Web;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.UserControlToHtml
{
    /// <summary>
    /// 控件生成Html帮助类
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    public class  ViewManager<T> where T:UserControl
    {
        private Page _pageHolder;

        /// <summary>
        /// 加载控件
        /// </summary>
        /// <param name="path">控件路径</param>
        /// <returns>返回创建的控件实例</returns>
        public T LoadViewControl(string path)
        {
            _pageHolder = new HtmlPage();
            return (T) _pageHolder.LoadControl(path);
        }

        /// <summary>
        /// 转换控件内容为Html字符串
        /// </summary>
        /// <param name="control">控件实例</param>
        /// <returns>返回生成的Html字符串</returns>
        public string RenderView(T control)
        {
            var output = new StringWriter();
            _pageHolder.Controls.Add(control);
            HttpContext.Current.Server.Execute(_pageHolder,output,false);
            return output.ToString();
        }
    }
}