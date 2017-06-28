using System.ComponentModel;
using System.Web.UI;

namespace Zhoubin.Infrastructure.Web.Validate
{
    /// <summary>
    /// 基本类型校验
    /// </summary>
    [ToolboxData("<{0}:NetValidate runat=\"server\"></{0}:NetValidate>")]
    public class NetValidate : NetBaseValidator
    {
        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <returns>返回正则表达式</returns>
        protected override string GetRegularExpression()
        {
            return ConfigContent[ValidateType.ToString()].RegularExpression;
        }


        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>错误信息</returns>
        protected override string GetErrorMessage()
        {
            return ConfigContent[ValidateType.ToString()].ErrorMessage;
        }

        /// <summary>
        /// 数据校验类型
        /// </summary>
        [DisplayName(@"校验类型")]
        public NetValidateType ValidateType { get; set; }
    }
}
