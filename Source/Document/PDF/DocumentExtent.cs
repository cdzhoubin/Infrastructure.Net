using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// Document对象扩展方法
    /// </summary>
    public static class DocumentExtent
    {
        /// <summary>
        /// 字符串对象写入
        /// </summary>
        /// <param name="document">itext Document对象</param>
        /// <param name="contentEntity">内容对象</param>
        /// <returns>itext Document对象</returns>
        public static iText.Layout.Document Write<T>(this iText.Layout.Document document, T content)
            where T:IPDFBlockElement,IContentEntity
        {
            if(content == null)
            {
                return document;
            }
            document.Add(content.CreateBlock());
            return document;
        }
    }
}
