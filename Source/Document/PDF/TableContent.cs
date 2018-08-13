using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Data;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 表格内容
    /// </summary>
    public class TableContent : ContentEntity<DataTable>, IPDFBlockElement
    {
        public int Width { get; set; }
        /// <summary>
        /// 标题字体
        /// </summary>
        public ContentFont HeaderFont { get; set; }

        /// <summary>
        /// 内容字体
        /// </summary>
        public ContentFont ContentFont { get; set; }

        /// <summary>
        /// 列宽度
        /// </summary>
        public List<float> RelativeWidths { get; set; }
        /// <summary>
        /// 是是否显示标题
        /// </summary>
        public bool ShowHeader { get; set; }

        public IBlockElement CreateBlock()
        {
            var table = CreateTable();
            if (AppendNewLine)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Add(table);
                paragraph.Add(Environment.NewLine);
                return paragraph;
            }

            return table;
        }

        private Table CreateTable()
        {
            var table = RelativeWidths == null || RelativeWidths.Count ==0 
                ? new Table(Content.Columns.Count):new Table(RelativeWidths.ToArray());
            if(Width > 0)
            {
                table.SetWidth(Width);
            }
            if (ShowHeader)
            {
                var headcells = new List<Cell>();
                foreach (DataColumn column in Content.Columns)
                {
                    var cell = new Cell();
                    var p = CreateParagraph(column.Caption, 0.4f, HeaderFont);
                    cell.Add(p);
                    table.AddCell(cell);
                }
            }

            foreach (DataRow dr in Content.Rows)
            {
                foreach (DataColumn column in Content.Columns)
                {
                    var cell = new Cell();
                    var p = CreateParagraph(
                        dr[column.ColumnName] == null ? "" : dr[column.ColumnName].ToString()
                        , 0.3f, ContentFont);
                    cell.Add(p);
                    table.AddCell(cell);
                }
            }
            return table;
        }

        private Paragraph CreateParagraph(string caption, float padding, ContentFont font)
        {
            var p = new Paragraph(caption);
            SetFont(font, p);
            p.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            p.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            p.SetPadding(padding);
            return p;
        }
    }
}