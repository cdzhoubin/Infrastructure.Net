﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Zhoubin.Infrastructure.Common.Config;
using Zhoubin.Infrastructure.Common.Cryptography;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 邮件发送类
    /// 此类提供发送带附件类
    /// 支持文本和超文本两种格式
    /// 同时支持内嵌图片（此功能需要使用html全部文本)
    /// </summary>
    public static class EmailHelper
    {

        /// <summary>
        /// 同步发送邮件
        /// </summary>
        /// <param name="mailFrom">发送者邮件地址</param>
        /// <param name="mailDisplayName">发送者名称</param>
        /// <param name="mailSubject">主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="mailTo">收件人</param>
        /// <param name="isHtml">是否超文本邮件</param>
        /// <param name="mailCc">抄送者</param>
        /// <param name="mailBcc">密送者</param>
        /// <param name="mailAttachments">附件</param>
        /// <returns>发送成功返回true</returns>
        public static bool SendMail(string mailFrom, string mailDisplayName, string mailSubject, string mailBody
            , List<string> mailTo, bool isHtml = true, List<string> mailCc = null, List<string> mailBcc = null, List<IEmailAttachment> mailAttachments = null)
        {
            var message = CreateMailMessage(mailFrom, mailDisplayName, mailSubject, mailBody, mailTo, isHtml, mailCc,
                mailBcc, mailAttachments);


            return SendMail(message, false);
        }

        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="mailFrom">发送者邮件地址</param>
        /// <param name="mailDisplayName">发送者名称</param>
        /// <param name="mailSubject">主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="mailTo">收件人</param>
        /// <param name="isHtml">是否超文本邮件</param>
        /// <param name="userState">异步状态标识</param>
        /// <param name="completeHandler">发送回调</param>
        /// <param name="mailCc">抄送者</param>
        /// <param name="mailBcc">密送者</param>
        /// <param name="mailAttachments">附件</param>
        /// <returns>成功返回true,其它失败</returns>
        public static bool SendMail(string mailFrom, string mailDisplayName, string mailSubject, string mailBody
            , List<string> mailTo, object userState, SendCompletedEventHandler completeHandler = null, bool isHtml = true, List<string> mailCc = null, List<string> mailBcc = null, List<IEmailAttachment> mailAttachments = null)
        {
            var message = CreateMailMessage(mailFrom, mailDisplayName, mailSubject, mailBody, mailTo, isHtml, mailCc,
                mailBcc, mailAttachments);
            return SendMail(message, true, userState, completeHandler);
        }

        private static MailMessage CreateMailMessage(string mailFrom, string mailDisplayName, string mailSubject, string mailBody
            , List<string> mailTo, bool isHtml, List<string> mailCc = null, List<string> mailBcc = null, List<IEmailAttachment> mailAttachments = null)
        {
            var encoding = Encoding.UTF8;
            var message = new MailMessage
            {
                From = new MailAddress(mailFrom, mailDisplayName ?? mailFrom, encoding),
                Subject = mailSubject,
                SubjectEncoding = encoding,
                BodyEncoding = encoding,
                Body = mailBody,
                IsBodyHtml = isHtml
            };

            if (mailTo != null && mailTo.Count > 0)
            {
                mailTo.Where(p => !string.IsNullOrEmpty(p)).ToList().ForEach(message.To.Add);
            }

            if (mailCc != null && mailCc.Count > 0)
            {
                mailCc.Where(p => !string.IsNullOrEmpty(p)).ToList().ForEach(message.CC.Add);
            }

            if (mailBcc != null && mailBcc.Count > 0)
            {
                mailBcc.Where(p => !string.IsNullOrEmpty(p)).ToList().ForEach(message.Bcc.Add);
            }

            if (mailAttachments != null && mailAttachments.Count > 0)
            {
                mailAttachments.ForEach(p => message.Attachments.Add(p.CreateAttachment()));
            }

            return message;
        }

        private static SmtpClient CreateSmtpClient()
        {
            MailConfig helper = new MailConfigHelper().DefaultConfig;
            var smtpMail = new SmtpClient(helper.Server,helper.Port);
            smtpMail.Credentials = new NetworkCredential(helper.User,helper.Password);
            smtpMail.EnableSsl = helper.EnableSsl;
            return smtpMail;
        }
        private static bool SendMail(MailMessage message, bool isAsync, object userState = null, SendCompletedEventHandler completeHandler = null)
        {
            var smtpMail = CreateSmtpClient();
            smtpMail.SendCompleted += completeHandler ?? smtpMail_SendCompleted;

            try
            {
                if (!isAsync)
                {
                    smtpMail.Send(message);
                }
                else
                {
                    userState = userState ?? Guid.NewGuid();
                    smtpMail.SendAsync(message, userState);
                }
            }
            catch (SmtpFailedRecipientException exception)
            {
                throw new InfrastructureException("发送邮件出错，详细信息参考内部错误。", exception);
            }
            finally
            {
                if (!isAsync)
                {
                    smtpMail.Dispose();
                }
            }

            return true;
        }

        static void smtpMail_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var client = sender as SmtpClient;
            if (client != null)
            {
                client.Dispose();
            }
        }
    }

    public class MailConfigHelper : ConfigHelper<MailConfig>
    {
        public MailConfigHelper() : base("MailSetting", false)
        {
        }
    }

    public class MailConfig : ConfigEntityBase
    {
        public MailConfig()
        {
            Port = 25;
            Default = true;
        }
        public string Server
        {
            get
            {
                return GetValue<string>("Server");
            }
            set
            {
                SetValue("Server", value);
            }
        }
        public int Port
        {
            get
            {
                return GetValue<int>("Port");
            }
            set
            {
                SetValue("Port", value);
            }
        }
        public bool EnableSsl
        {
            get
            {
                return GetValue<bool>("EnableSsl");
            }
            set
            {
                SetValue("EnableSsl", value);
            }
        }
        public string User
        {
            get
            {
                return GetValue<string>("User");
            }
            set
            {
                SetValue("User", value);
            }
        }
        public string Password
        {
            get
            {
                return GetValue<string>("Password");
            }
            set
            {
                SetValue("Password", value);
            }
        }
    }
}
