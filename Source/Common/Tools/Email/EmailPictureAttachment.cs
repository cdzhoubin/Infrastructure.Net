using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace Zhoubin.Infrastructure.Common.Tools.Email
{
    /// <summary>
    /// 图片附件用于内联内容发送
    /// </summary>
    public class EmailPictureAttachment : EmailAttachment
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="content">文件路径</param>
        /// <param name="contentId">自定义标识</param>
        public EmailPictureAttachment(string content, string contentId)
            : base(content, MediaTypeNames.Image.Jpeg)
        {
            ContentId = contentId;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="content">文件流</param>
        /// <param name="contentId">自定义标识</param>
        public EmailPictureAttachment(Stream content, string contentId)
            : base(content, MediaTypeNames.Image.Jpeg)
        {
            ContentId = contentId;
        }

        /// <summary>
        /// 附件内容附件属性填充
        /// </summary>
        /// <param name="attachment">待填充附件实体</param>
        protected override void FillAttachment(Attachment attachment)
        {
            base.FillAttachment(attachment);
            attachment.ContentId = ContentId;
            attachment.ContentDisposition.Inline = true;
            if (IsFileContent)
            {
                var info = new FileInfo(FileContent);
                attachment.ContentDisposition.CreationDate = info.CreationTime;
                attachment.ContentDisposition.ModificationDate = info.LastWriteTime;
                attachment.ContentDisposition.ReadDate = info.LastAccessTime;
                attachment.ContentDisposition.FileName = info.Name;
            }
            else
            {
                attachment.ContentDisposition.CreationDate = DateTime.Now;
                attachment.ContentDisposition.ModificationDate = DateTime.Now;
                attachment.ContentDisposition.ReadDate = DateTime.Now;
                attachment.ContentDisposition.FileName = ContentId + ".jpg";
            }

        }

        /// <summary>
        /// 图片编号
        /// </summary>
        public String ContentId { get; set; }

        /// <summary>
        /// 创建外链链接地址
        /// </summary>
        /// <returns>返回当前对象的外链地址</returns>
        public string CreateLink()
        {
            return string.Format("cid:{0}", ContentId);
        }
    }
}