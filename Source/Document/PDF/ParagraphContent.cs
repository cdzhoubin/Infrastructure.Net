using System;
using System.Collections.Generic;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 段落对象
    /// </summary>
    public sealed class ParagraphContent : ContentTextFont<List<IContentEntity>>, IPDFBlockElement
    {
        /// <summary>
        /// 段落构造函数
        /// 
        /// </summary>
        public ParagraphContent()
        {
            Leading = 2 * 15.5f;
            Content = new List<IContentEntity>();
        }
        /// <summary>
        /// 对象方式
        /// </summary>
        public int? Align { get; set; }
        /// <summary>
        /// 左缩进
        /// </summary>
        public int? IndentLeft { get; set; }
        /// <summary>
        /// 首先缩进
        /// </summary>
        public int? IndentFirst { get; set; }

        /// <summary>
        /// 默认行间距为31
        /// </summary>
        public float Leading { get; set; }

        public IBlockElement CreateBlock()
        {
            Paragraph p = new Paragraph();
            SetFont(Font, p);
            p.SetTextAlignment(Align != null ? (TextAlignment)Align.Value : TextAlignment.JUSTIFIED);
            p.SetFixedLeading(Leading);
            if (IndentLeft != null)
            {
                //p.setIn = content.IndentLeft.Value * content.Font.Size;
            }

            if (IndentFirst != null)
            {
                p.SetFirstLineIndent(IndentFirst.Value * Font.Size.Value);
            }
            foreach (var content in Content)
            {
                if (content is IPDFBlockElement)
                {
                    p.Add(((IPDFBlockElement)content).CreateBlock());
                }
                else
                if (content is IPDFLeafElement)
                {
                    p.Add(((IPDFLeafElement)content).CreateLeaf());
                }
                else
                    throw new System.Exception("不支持的类型：" + content.GetType().FullName);
            }

            if (AppendNewLine)
            {
                p.Add(Environment.NewLine);
            }
            return p;
        }
    }
}