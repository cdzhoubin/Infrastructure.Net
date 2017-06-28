using System.Net.Mail;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 邮件附件接口
    /// </summary>
    public interface IEmailAttachment
    {
        /// <summary>
        /// 创建附件
        /// </summary>
        /// <returns>返回创建好的附件对象</returns>
        Attachment CreateAttachment();
    }
}