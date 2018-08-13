using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Element;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 内容基类
    /// </summary>
    /// <typeparam name="T">内容类型</typeparam>
    public abstract class ContentEntity<T> : IContentEntity
    {
        /// <summary>
        /// 内容
        /// </summary>
        public virtual T Content { get; set; }
        /// <summary>
        /// 添加内容后，加入追加空行
        /// </summary>
        public bool AppendNewLine { get; set; }
        /// <summary>
        /// 字体设置
        /// </summary>
        /// <typeparam name="TContent">内容类型</typeparam>
        /// <param name="font">字体定义</param>
        /// <param name="obj">对象</param>
        protected static void SetFont<TContent>(ContentFont font, ElementPropertyContainer<TContent> obj) where TContent : IPropertyContainer
        {
            if (font != null)
            {
                if (font.Style == null)
                {
                    obj.SetFont(PdfFontFactory.CreateRegisteredFont(font.Name, PdfEncodings.IDENTITY_H, font.Embedded, true));
                }
                else
                {
                    obj.SetFont(PdfFontFactory.CreateRegisteredFont(font.Name, PdfEncodings.IDENTITY_H, font.Embedded, font.Style.Value, true));
                }
                if (font.Size != null)
                {
                    obj.SetFontSize(font.Size.Value);
                }
                if (font.Color != null)
                {
                    obj.SetFontColor(font.Color);
                }
            }
        }
    }
}