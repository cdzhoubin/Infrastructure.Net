using System.ComponentModel;
using System.Web.UI;
using Zhoubin.Infrastructure.Common.Properties;

namespace Zhoubin.Infrastructure.Web.Validate
{
    /// <summary>
    /// 数值数据校验
    /// </summary>
    [ToolboxData("<{0}:NumberValidator runat=\"server\"></{0}:NumberValidator>")]
    public class NumberValidator : NetBaseValidator
    {
        /// <summary>
        /// 整数部分长度
        /// </summary>
// ReSharper disable LocalizableElement
        [DisplayName("整数长度")]
// ReSharper restore LocalizableElement
        public int IntegerLength { get; set; }
        /// <summary>
        /// 小数部分长度
        /// </summary>
// ReSharper disable LocalizableElement
        [DisplayName("小数长度")]
// ReSharper restore LocalizableElement
        public int DecimalLength { get; set; }

        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <returns>返回正则表达式</returns>
        protected override string GetRegularExpression()
        {
            var str = string.Format("\\d{{{0}}}", IntegerLength);
            return DecimalLength > 0 ? string.Format("{0}(.\\d{{{1}}})", str, DecimalLength) : str;
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>错误信息</returns>
        protected override string GetErrorMessage()
        {
            return DecimalLength > 0 ?
                       string.Format(Resources.NumberCheckMessage, IntegerLength, DecimalLength)
                       : string.Format(Resources.NumberCheckMessageSecond, IntegerLength);
        }
    }
}