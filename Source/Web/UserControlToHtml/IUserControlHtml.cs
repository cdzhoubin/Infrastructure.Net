using System.Web;

namespace Zhoubin.Infrastructure.Web.UserControlToHtml
{
    /// <summary>
    /// 控件生成Html代码实现接口
    /// 所有要使用控件生成Html的用户控件都要生成实现此接口
    /// </summary>
    public interface IUserControlHtml
    {
        /// <summary>
        /// 对用户控件进行初始化
        /// </summary>
        /// <param name="context">上下文</param>
        void InitUserControl(HttpContext context);
    }
}