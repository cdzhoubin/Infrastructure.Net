using iText.Kernel.Font;
using iText.Layout.Element;
using System;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 字符串对象
    /// </summary>
    public class ContentString : ContentTextFont<string>, IPDFBlockElement
    {
        public virtual IBlockElement CreateBlock()
        {
            Paragraph p = new Paragraph(Content);
            if (AppendNewLine)
            {
                p.Add(Environment.NewLine);
            }
            SetFont(Font, p);
            return p;
        }        
    }
}