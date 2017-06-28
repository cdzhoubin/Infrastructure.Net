using System;
using System.Web;
using System.Web.UI.WebControls;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Web.Validate
{
    /// <summary>
    /// 数据检验基类
    /// </summary>
    public  abstract class NetBaseValidator:RegularExpressionValidator
    {
        /// <summary>
        /// 配置内容
        /// </summary>
        protected static readonly ConfigHelper<ValidateEntity> ConfigContent;
        static NetBaseValidator()
        {
            var configFilePath = System.Configuration.ConfigurationManager.AppSettings["NetConfigPath"];
            if (!string.IsNullOrEmpty(configFilePath))
            {
                if (HttpContext.Current != null)//httpcontent，配置虚拟目录
                {
                    configFilePath = HttpContext.Current.Server.MapPath(configFilePath);
                }
            }

            ConfigContent = new ConfigHelper<ValidateEntity>("ValidateEntities", configFilePath);
        }
        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <returns>返回正则表达式</returns>
        protected abstract string GetRegularExpression();
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>错误信息</returns>
        protected abstract string GetErrorMessage();

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.Init"/> 事件。
        /// </summary>
        /// <param name="e">一个包含事件数据的 <see cref="T:System.EventArgs"/>。</param>
        protected override void OnInit(EventArgs e)
        {
            ValidationExpression = GetRegularExpression();
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = GetErrorMessage();
            }
            base.OnInit(e);
        }
    }
}
