using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Tools.Email
{
    /// <summary>
    /// 普通邮件附件
    /// </summary>
    public class EmailAttachment : IEmailAttachment
    {
        private readonly Stream _streamContent;
        private readonly string _fileContent;
        private readonly bool _isFileContent;


        /// <summary>
        /// 文件附件
        /// </summary>
        /// <param name="content">文件路径</param>
        /// <param name="contentType">内容Mime类型</param>
        public EmailAttachment(string content, string contentType)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content");
            }

            if (!File.Exists(content))
            {
                throw new FileNotFoundException("文件不存在。", content);
            }

            ContentType = contentType;
            _fileContent = content;
            _isFileContent = true;
        }

        /// <summary>
        /// 文件流附件
        /// </summary>
        /// <param name="content">文件流内容</param>
        /// <param name="contentType">内容Mime类型</param>
        public EmailAttachment(Stream content, string contentType)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            _streamContent = content;
            ContentType = contentType;
            _isFileContent = false;
            _streamContent.Position = 0;
        }
        /// <summary>
        /// 创建邮件发送使用的附件实体
        /// </summary>
        /// <returns>返回创建好的附件对象</returns>
        public Attachment CreateAttachment()
        {
            var att = _isFileContent ? new Attachment(_fileContent, ContentType) : new Attachment(_streamContent, new ContentType(ContentType));
            att.NameEncoding = Encoding.UTF8;
            FillAttachment(att);
            return att;
        }

        /// <summary>
        /// 填充附件
        /// </summary>
        /// <param name="attachment">附件对象</param>
        protected virtual void FillAttachment(Attachment attachment)
        {

        }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// 文件内容时此属性为true,
        /// 文件流时此属性为false
        /// </summary>
        protected bool IsFileContent
        {
            get { return _isFileContent; }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        protected string FileContent
        {
            get { return _fileContent; }
        }

        /// <summary>
        /// 文件流
        /// </summary>
        protected Stream StreamContent
        {
            get { return _streamContent; }
        }
    }
}