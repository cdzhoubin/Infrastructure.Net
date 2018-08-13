using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 文本水印
    /// </summary>
    public sealed class TextWatermarker : IEventHandler
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TextWatermarker()
            : this("四川日报招标比选网")
        {

        }
        private readonly string _phrase;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="waterMaker">水印文本</param>
        public TextWatermarker(string waterMaker)
        {
            _phrase = waterMaker;
        }
        

        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdf = docEvent.GetDocument();

            PdfPage page = docEvent.GetPage();
            //PdfFont font = null;
            //try
            //{
            //    //"华文宋体", BaseFont.IDENTITY_H, 55, Font.NORMAL, new GrayColor(0.85f)
            //    font = PdfFontFactory.CreateFont("华文宋体");

            //}
            //catch (IOException)
            //{
            //    return;
            //}
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);
            new Canvas(canvas, pdf, page.GetPageSize())
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(60)
                   // .SetFont(font)
                    .ShowTextAligned(_phrase, 298, 421, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 
                    pdf.GetPageNumber(page) % 2 == 1 ? 45 : -45);
        }
    }
}