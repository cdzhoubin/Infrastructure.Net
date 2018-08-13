using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout;
using Zhoubin.Infrastructure.Common.Document;
using Zhoubin.Infrastructure.Common.Document.Pdf;

namespace DocumentSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            PdfSample doc = new PdfSample();

            doc.Initialize();
            doc.Open();
            using (var stream = new FileStream("e:\\6.pdf", FileMode.OpenOrCreate))
                doc.Save(stream);
        }
    }
    public class PdfSample : PdfDocumentBase<PdfSample>
    {
        protected override void DocumentInitialize()
        {
            FontFiles.Add("c:/windows/fonts/simsun.ttc");
            
            base.DocumentInitialize();
        }

        ContentFont font = new ContentFont() { Name = "宋体" };
        ContentFont fontYellow = new ContentFont() { Name = "宋体", Color = ColorConstants.YELLOW };
        ContentFont fontGreen = new ContentFont() { Name = "宋体", Color = ColorConstants.GREEN };
        protected override void WriteDocument(Document document)
        {
            document.Write(new ContentString { Content = "示例pdf文件生成，我的颜色是黄色", Font = fontYellow });
            document.Write(new ContentString { Content = "示例pdf文件生成，我的颜色是绿色", Font = fontGreen });
            document.Write(new NewLineContent { Count = 5 });
            document.Write(new ContentString { Content = "文本段落示例：", Font = font });

            ParagraphContent paragraphContent = new ParagraphContent();
            paragraphContent.Content.Add(new ContentString { Content = text, Font = fontGreen });
            paragraphContent.Content.Add(new NewLineContent { Count = 1 });
            paragraphContent.Content.Add(new PictureContent { Content = File.ReadAllBytes("E:\\1.jpg"), Width = 500 });
            //   paragraphContent.Content.Add(new ContentString { Content = contetText, Font = font });
            paragraphContent.Content.Add(new ContentString { Content = text, Font = fontYellow });
            paragraphContent.Content.Add(new NewLineContent { Count = 1 }); paragraphContent.Content.Add(new PictureContent { Content = File.ReadAllBytes("E:\\2.jpg"), Width = 500 });
            //paragraphContent.Content.Add(new ContentString { Content = contetText.Replace("\n", ""), Font = font });
            document.Write(paragraphContent);

            //AddList(document);

            AddTable(document);

        }

        private void AddTable(Document document)
        {
            TableContent tableContent = new TableContent() { Content = CreateDataTable() };
            tableContent.HeaderFont = fontYellow;
            tableContent.ContentFont = font;
            tableContent.ShowHeader = true;
            tableContent.RelativeWidths = new List<float> { 0.25f,0.25f,0.25f,0.25f};
            tableContent.Width = 500;
            document.Write(tableContent);
        }
        List<string> names = new List<string> { "张三", "李四", "王五", "司马迁", "岳飞" };
        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn { ColumnName = "Name", Caption = "名称" });
            dt.Columns.Add(new DataColumn { ColumnName = "Age", Caption = "年龄" });
            dt.Columns.Add(new DataColumn { ColumnName = "Birday", Caption = "生日" });
            dt.Columns.Add(new DataColumn { ColumnName = "Sex", Caption = "性别" });
            for (int i = 1; i <= 100; i++)
            {
                var row = dt.NewRow();
                row["Name"] = names[random.Next(0,4)];
                row["Age"] = random.Next(18,29);
                row["Birday"] = DateTime.Now.AddDays(-random.Next(180,10000)).ToString("yyyy-MM-dd");
                row["Sex"] = random.Next(0,1) == 1 ? "男":"女";
                dt.Rows.Add(row);
            }
            return dt;
        }
        static Random random = new Random();
        private void AddList(Document document)
        {
            ListType listType = ListType.DECIMAL;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));

            listType = ListType.DECIMAL_LEADING_ZERO;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ENGLISH_LOWER;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ENGLISH_UPPER;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));

            listType = ListType.GREEK_LOWER;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));

            listType = ListType.GREEK_UPPER;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ROMAN_LOWER;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ROMAN_UPPER;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ZAPF_DINGBATS_1;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ZAPF_DINGBATS_2;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ZAPF_DINGBATS_3;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
            listType = ListType.ZAPF_DINGBATS_4;
            document.Write(new ContentString() { Content = "列表示例：" + listType, Font = font, AppendNewLine = true });
            document.Write(CreateList(listType));
        }
        private ListContent CreateList(ListType listType)
        {
            ListContent list = new ListContent() {
            ListType = listType
            };
            for (int i = 0; i < 5; i++)
            {
                List<IContentEntity> itemList = new List<IContentEntity>();
                ParagraphContent content = new ParagraphContent();
                for (int j = 0; j < 5; j++)
                {
                    content.Content.Add(new ContentText() { Content = "列表项" + j, Font = j % 2 == 1 ? font : fontYellow });
                }
                content.Content.Add(new ContentText() { Content = i.ToString(), Font = i % 2 == 1 ? font : fontYellow });
                itemList.Add(content);
                list.Content.Add(itemList);
            }
            return list;
        }
        string text = "原因";
        
    }
}
