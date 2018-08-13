using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 新行内容
    /// </summary>
    public class NewLineContent : ContentEntity<string>, IPDFBlockElement
    {
        /// <summary>
        /// 换行内容
        /// </summary>
        public override string Content
        {
            get
            {
                return Environment.NewLine;
            }
            set { }
        }

        /// <summary>
        /// 换行次数
        /// </summary>
        public int Count { get; set; }
        public virtual IBlockElement CreateBlock()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                sb.Append(Content);
            }
            Paragraph p = new Paragraph(sb.ToString());
            return p;
        }
    }

    public sealed class ListContent : ContentTextFont<List<List<IContentEntity>>>, IPDFBlockElement
    {
        public ListContent()
        {
            ListType = ListType.DECIMAL;
            Content = new List<List<IContentEntity>>();
        }
        public ListType ListType { get; set; }
        public IBlockElement CreateBlock()
        {
            List list = new List((iText.Layout.Properties.ListNumberingType)((int)ListType));
            SetFont(Font, list);
            foreach (var item in Content)
            {
                ListItem li = new ListItem();
                foreach (var s in item)
                {
                    if (s is IPDFBlockElement)
                    {
                        li.Add((s as IPDFBlockElement).CreateBlock());
                    }
                    else
                    if (s is IPDFLeafElement)
                    {
                        var image = ((IPDFLeafElement)s).CreateLeaf();
                        if (image is Image)
                            li.Add((Image)image);
                        else if (image is AreaBreak)
                        {
                            li.Add((AreaBreak)image);
                        }
                        else
                        {
                            throw new System.Exception("不支持的类型：" + item.GetType().FullName);
                        }
                    }
                    else
                        throw new System.Exception("不支持的类型：" + item.GetType().FullName);
                }
                list.Add(li);
            }

            return list;
        }
    }
    public enum ListType
    {
        DECIMAL = 0,
        DECIMAL_LEADING_ZERO = 1,
        ROMAN_LOWER = 2,
        ROMAN_UPPER = 3,
        ENGLISH_LOWER = 4,
        ENGLISH_UPPER = 5,
        GREEK_LOWER = 6,
        GREEK_UPPER = 7,
        ZAPF_DINGBATS_1 = 8,
        ZAPF_DINGBATS_2 = 9,
        ZAPF_DINGBATS_3 = 10,
        ZAPF_DINGBATS_4 = 11
    }
}