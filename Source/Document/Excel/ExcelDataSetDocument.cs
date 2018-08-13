using System.Data;
using NPOI.SS.UserModel;

namespace Zhoubin.Infrastructure.Common.Document.Excel
{
    /// <summary>
    /// DataSet和Excel互转换
    /// </summary>
    public sealed class ExcelDataSetDocument : ExcelDocumentBase<DataSet>
    {
        private readonly bool _firstRowCaption;
        /// <summary>
        /// 写入excel构造函数
        /// </summary>
        /// <param name="ds"></param>
        /// <exception cref="InfrastructureException"></exception>
        public ExcelDataSetDocument(DataSet ds) : base(ds)
        {
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
            {
                throw new InfrastructureException("没有有效数据输出");
            }
            bool thorwResult = true;
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    thorwResult = false;
                }
            }
            if (thorwResult)
            {
                throw new InfrastructureException("没有有效数据输出");
            }
        }
        /// <summary>
        /// 读取Excel构造函数
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="firstRowCaption"></param>
        public ExcelDataSetDocument(string fileName, bool firstRowCaption = true) : base(fileName)
        {
            _firstRowCaption = firstRowCaption;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void Write()
        {
            int i = 0;
            foreach (DataTable dt in InputData.Tables)
            {
                ISheet sheet = GetSheet(string.IsNullOrEmpty(dt.TableName) ? "Table" + (i++).ToString(): dt.TableName);
                sheet.Write(dt);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DataSet DocumentRead(IWorkbook hssfworkbook)
        {
            DataSet ds = new DataSet();
            for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
            {
                ISheet sheet = hssfworkbook.GetSheetAt(i);
                if (sheet.LastRowNum == 0)
                {
                    continue;
                }
                DataTable dt = new DataTable(sheet.SheetName);
                int j = 0;
                if (_firstRowCaption)
                {
                    IRow row = sheet.GetRow(j);
                    for (int k = 0; k < row.Cells.Count; k++)
                    {
                        DataColumn dc = new DataColumn(row.Cells[k].StringCellValue) {DataType = typeof(string)};
                        dt.Columns.Add(dc);
                    }
                    j = j + 1;
                }
                else
                {
                    IRow row = sheet.GetRow(j);
                    for (int k = 0; k < row.Cells.Count; k++)
                    {
                        DataColumn dc = new DataColumn("Column" + k) {DataType = typeof(string)};
                        dt.Columns.Add(dc);
                    }
                }
                for (;j <= sheet.LastRowNum; j++)
                {
                    IRow row = sheet.GetRow(j);
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < row.Cells.Count; k++)
                    {
                        ICell cell = row.Cells[k];
                        string value = "";
                        switch (cell.CellType)
                        {
                            case CellType.Blank:
                                break;
                            case CellType.Boolean:
                                value = cell.BooleanCellValue.ToString().ToLower();
                                break;
                            case CellType.Error:
                                value = null;
                                break;
                            case CellType.Formula:
                                value = cell.CellFormula;
                                break;
                            case CellType.Numeric:
                                value = cell.NumericCellValue.ToString("F2");
                                break;
                            case CellType.String:
                                value = cell.StringCellValue;
                                break;
                            default:
                                value = null;
                                break;
                        }
                        dr[k] = value;
                    }
                    dt.Rows.Add(dr);
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }
    }
}