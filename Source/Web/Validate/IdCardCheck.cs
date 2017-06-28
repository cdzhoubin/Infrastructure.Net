using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Zhoubin.Infrastructure.Web.Validate
{
    /// <summary>
    /// 中华人民共和国身份证验证
    /// </summary>
    [ToolboxData("<{0}:ChinaIdCardValidator runat=\"server\"></{0}:ChinaIdCardValidator>")]
    public class ChinaIdCardValidator : CustomValidator
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChinaIdCardValidator()
        {
            ClientValidationFunction = "ValidateChinaIdCard";
            ErrorMessage = Properties.Resources.IdCardTipMessage;
        }

        /// <summary>
        /// 为 <see cref="T:System.Web.UI.WebControls.CustomValidator"/> 控件引发 <see cref="E:System.Web.UI.WebControls.CustomValidator.ServerValidate"/> 事件。
        /// </summary>
        /// <returns>
        /// 如果 <paramref name="value"/> 参数所指定的值通过验证，则为 true；否则为 false。
        /// </returns>
        /// <param name="value">要验证的值。</param>
        protected override bool OnServerValidate(string value)
        {
            string msg;
            var result = CheckIdCard.CheckCard(value, out msg);
            ErrorMessage = result ? CheckIdCard.ErrorIdCardMessage : msg;
            return result;
        }

        /// <summary>
        /// 引发 <see cref="E:System.Web.UI.Control.PreRender"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.EventArgs"/>。</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (Page != null)
            {
                CheckIdCard.RegisterScript();
            }

            base.OnPreRender(e);
        }
    }
}
