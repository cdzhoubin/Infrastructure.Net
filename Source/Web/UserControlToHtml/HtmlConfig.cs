using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Web.UserControlToHtml
{
    /// <summary>
    /// 用户控件生成Html代码
    /// 配置读取类
    /// </summary>
    public sealed class HtmlConfig:ConfigHelper<HtmlEntity>
    {
        /// <summary>
        /// Html配置读取构造函数
        /// </summary>
        /// <remarks>默认从默认的配置文件中读取，如web项目从web.config，winfrom程序从app.config，配置的区域名称为：HtmlEntities</remarks>
        /// <example>配置示例：
        /// 
        /// 
        /// 
        /// 
        /// </example>
        public HtmlConfig():base("HtmlEntities",null){}

        /// <summary>
        /// Html配置读取构造函数
        /// </summary>
        /// <param name="configFile">配置文件路径</param>
        /// <param name="sectionName">配置区域名称，默认值为：HtmlEntities</param>
        public HtmlConfig(string configFile, string sectionName = "HtmlEntities") : base(sectionName, configFile) { }
    }
}
