using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// Pdf文档生成对象基类
    /// </summary>
    /// <typeparam name="T">文档类型</typeparam>
    public abstract class PdfDocumentBase<T> : DocumentBase<T> where T : PdfDocumentBase<T>
    {
        /// <summary>
        /// 启用中文支持
        /// </summary>
        protected bool EnableAsian { get { return true; } }

        private PdfDocument _pdfDocument;
        private iText.Layout.Document _document;

        private PageSize _pageSize = PageSize.A4;
        private PdfWriter _writer;
        float _marginLeft = 90;
        float _marginRigth = 70f;
        float _marginTop = 50f;
        float _marginBottom = 50f;

        private string _tempFileName;
        // ReSharper disable StaticFieldInGenericType
        static volatile object _loadSync = new object();
        static readonly List<string> RegisterFontFiles = new List<string>();
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// 构造函数
        /// </summary>
        protected PdfDocumentBase() : base(false)
        {
            FontFiles = new List<string>();
            PageEvent = new TextWatermarker();
        }

        static PdfDocumentBase()
        {
           // PdfFontFactory.RegisterSystemDirectories();
        }

        /// <summary>
        /// 创建字体
        /// </summary>
        /// <param name="fontName">字体名</param>
        /// <param name="size">字号</param>
        /// <param name="style">样式</param>
        /// <param name="color">颜色</param>
        /// <returns>返回创建成功的字体对象</returns>
        protected PdfFont CreateFont(string fontName, float size, int style, Color color)
        {
            var font = PdfFontFactory.CreateRegisteredFont(fontName, PdfEncodings.IDENTITY_H,false,style);
            
            return font;
        }

        /// <summary>
        /// 创建字体
        /// </summary>
        /// <param name="fontName">字体名</param>
        /// <param name="size">字号</param>
        /// <param name="style">样式</param>
        /// <returns>返回创建成功的字体对象</returns>
        protected PdfFont CreateFont(string fontName, float size, int style)
        {
            return PdfFontFactory.CreateRegisteredFont(fontName, PdfEncodings.IDENTITY_H,false,style);
        }

        /// <summary>
        /// 创建字体
        /// </summary>
        /// <param name="fontName">字体名</param>
        /// <param name="size">字号</param>
        /// <returns>返回创建成功的字体对象</returns>
        protected PdfFont CreateFont(string fontName, float size)
        {
            return PdfFontFactory.CreateRegisteredFont(fontName, PdfEncodings.IDENTITY_H,false);
        }

        /// <summary>
        /// 创建字体
        /// </summary>
        /// <param name="fontName">字体名</param>
        /// <returns>返回创建成功的字体对象</returns>
        protected PdfFont CreateFont(string fontName)
        {
            return PdfFontFactory.CreateRegisteredFont(fontName, PdfEncodings.IDENTITY_H);
        }


        /// <summary>
        /// 文档事件，用于写入水印和其它一些特殊需求使用
        /// </summary>
        protected virtual IEventHandler PageEvent { get; private set; }
        /// <summary>
        /// 初始化文档
        /// </summary>
        /// <param name="pageSize">页面，默认为A4</param>
        /// <param name="marginLeft">左间距</param>
        /// <param name="marginRigth">右间距</param>
        /// <param name="marginTop">顶部间距</param>
        /// <param name="marginBottom">底部间距</param>
        public virtual void Initialize(PageSize pageSize, int marginLeft, int marginRigth, int marginTop, int marginBottom)
        {
            _pageSize = pageSize;
            _marginLeft = marginLeft;
            _marginRigth = marginRigth;
            _marginTop = marginTop;
            _marginBottom = marginBottom;
            Initialize();
        }

        /// <summary>
        /// 文档初始
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void DocumentInitialize()
        {
            lock (_loadSync)
            {

                FontFiles.ForEach(p =>
                            {
                                if (File.Exists(p) && !RegisterFontFiles.Contains(p))
                                {
                                    PdfFontFactory.Register(p);
                                    RegisterFontFiles.Add(p);
                                }
                            });
            }
        }

        /// <summary>
        /// 注册字体文件
        /// </summary>
        protected List<string> FontFiles { get; private set; }
        /// <summary>
        /// 文档打开
        /// </summary>
        /// <exception cref="Exception">当文档对象未初始化时，抛出此异常</exception>
        protected override void DocumentOpen()
        {
            _tempFileName = System.IO.Path.GetTempFileName();
            _writer = new PdfWriter(new FileStream(_tempFileName, FileMode.OpenOrCreate));
            _pdfDocument = new PdfDocument(_writer);
            _pdfDocument.SetDefaultPageSize(PageSize.A4);
            _pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, PageEvent);
            //_writer = PdfWriter.GetInstance(_document, );
            //_writer.PageEvent = PageEvent;
            //_document.Open();
            //var doc = _pdfDocument.AddNewPage();
            
            _document = new iText.Layout.Document(_pdfDocument);
            _document.SetMargins(_marginLeft, _marginRigth, _marginTop, _marginBottom);
        }

        /// <summary>
        /// 文档生成
        /// </summary>
        /// <param name="stream">输出流</param>
        protected override void DocumentSave(Stream stream)
        {
            WriteDocument(_document, _writer);
            _pdfDocument.Close();


            using (var tmp = new FileStream(_tempFileName, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[2048];
                int count;
                while ((count = tmp.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, count);
                }
            }

            stream.Position = 0;
        }


        /// <summary>
        /// 文档内容写入
        /// </summary>
        /// <param name="document">文档对象</param>
        /// <param name="writer">PdfWriter对象，用于高级设置如水印等等</param>
        protected virtual void WriteDocument(iText.Layout.Document document, PdfWriter writer)
        {
            WriteDocument(document);
        }

        /// <summary>
        /// 文档内容写入
        /// </summary>
        /// <param name="document">文档对象</param>
        protected abstract void WriteDocument(iText.Layout.Document document);

        /// <summary>
        /// 自动回收
        /// </summary>
        public override void Dispose()
        {
            if (_pdfDocument != null)
            {
                _pdfDocument = null;
            }

            if (_writer != null)
            {
                _writer.Dispose();
            }

            if (File.Exists(_tempFileName))
            {
                File.Delete(_tempFileName);
            }
            base.Dispose();
        }
    }
}
