using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Zhoubin.Infrastructure.Common.Document.Excel
{
    /// <summary>
    /// Excel文件处理基类
    /// </summary>
    /// <typeparam name="T">实例类型</typeparam>
    public abstract class ExcelDocumentBase<T> : DocumentBase<T>
    {
        IWorkbook _hssfworkbook;
        /// <summary>
        /// 写入文件输入数据
        /// </summary>
        protected T InputData { get; private set; }

        /// <summary>
        /// 构造函数
        /// 创建Excel文件
        /// </summary>
        /// <param name="ds">待转换数据</param>
        protected ExcelDocumentBase(T ds):base(false)
        {
            InputData = ds;
        }

        /// <summary>
        /// 构造函数
        /// 用于读取excel文件
        /// </summary>
        /// <param name="fileName">打开的文件名</param>
        /// <exception cref="FileNotFoundException">当文件不存在时，抛出此异常。</exception>
        protected ExcelDocumentBase(string fileName):base(true)
        {
            _fileName = fileName;
            if (!File.Exists(_fileName))
            {
                throw new FileNotFoundException("文件没有找到。",_fileName);
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        private readonly string _fileName;

        /// <inheritdoc />
        protected override void DocumentInitialize()
        {
            var hssfworkbook1 = string.IsNullOrEmpty(_fileName) ? new HSSFWorkbook() : new HSSFWorkbook(new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "软件爱好者：周彬";
            hssfworkbook1.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "";

            hssfworkbook1.SummaryInformation = si;
            _hssfworkbook = hssfworkbook1;
        }


        /// <inheritdoc />
        protected override void DocumentOpen()
        {
            
        }


        /// <inheritdoc />
        protected override void DocumentSave(Stream stream)
        {
            Write();
            SetPrintRegion();
            _hssfworkbook.Write(stream);
        }


        /// <summary>
        /// 设置打印区域
        /// </summary>
        protected virtual void SetPrintRegion()
        {
            
        }

        /// <summary>
        /// 创建文档内容
        /// </summary>
        protected abstract void Write();
        

        /// <summary>
        /// 获取指定的Sheet
        /// 如果不存在就创建
        /// </summary>
        /// <param name="index">sheet索引编号</param>
        /// <returns>返回指定的sheet对象</returns>
        protected ISheet GetSheet(int index)
        {
            if (_hssfworkbook.NumberOfSheets < index)
            {
                int i = index - _hssfworkbook.NumberOfSheets + 1;
                while (i>0)
                {
                    _hssfworkbook.CreateSheet();
                    i--;
                }
                
            }

            
            return _hssfworkbook.GetSheetAt(index);
        }

        /// <summary>
        /// 获取指定的Sheet
        /// 如果不存在就创建
        /// </summary>
        /// <param name="name">sheet名称</param>
        /// <returns>返回指定的sheet对象</returns>
        protected ISheet GetSheet(string name)
        {
            return _hssfworkbook.GetSheet(name) ?? _hssfworkbook.CreateSheet(name);
        }

        protected override T DocumentRead()
        {
            return DocumentRead(_hssfworkbook);
        }
        /// <summary>
        /// 6读取文件
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <returns></returns>
        protected abstract T DocumentRead(IWorkbook hssfworkbook);
    }
}
