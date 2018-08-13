using iText.Layout;
using iText.Layout.Element;
using System;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 文本对象
    /// </summary>
    public abstract class ContentTextFont<T> : ContentEntity<T>
    {
        /// <summary>
        /// 字体
        /// </summary>
        public ContentFont Font { get; set; }
    }
    /// <summary>
    /// 文本对象
    /// </summary>
    public class ContentText : ContentTextFont<string>, IPDFLeafElement
    {
        public ILeafElement CreateLeaf()
        {
            Text p = new Text(AppendNewLine ? Content + Environment.NewLine:Content);
            SetFont(Font, p);
            return p;
        }
    }
}