using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Zhoubin.Infrastructure.Common.Log;
using Zhoubin.Infrastructure.Common.Tools;

namespace Zhoubin.Infrastructure.Common.Identity.MessageService
{
    /// <summary>
    /// 邮件发送服务
    /// </summary>
    public class EmailService : IEmailSender
    {
        private readonly string _sender;
        private readonly string _senderName;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="senderName">发送者名称</param>
        /// <exception cref="ArgumentNullException">当参数为null时抛出此异常</exception>
        public EmailService(string sender, string senderName)
        {
            if (string.IsNullOrEmpty(sender))
            {
                throw new ArgumentNullException("sender");
            }
            if (string.IsNullOrEmpty(senderName))
            {
                throw new ArgumentNullException("senderName");
            }
            _sender = sender;
            _senderName = senderName;

        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="message">邮件消息</param>
        /// <returns></returns>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.Run(() =>
            {
                bool result = EmailHelper.SendMail(_sender, _senderName,subject, htmlMessage, new List<string>() { email });
                if (!result)
                {
                    LogFactory.GetDefaultLogger().Write(new LogEntity() { Content = string.Format("邮件发送失败，接收人：{0},主题：{1}内容：{2}", email, subject, htmlMessage) });
                }
            });
        }
    }
}
