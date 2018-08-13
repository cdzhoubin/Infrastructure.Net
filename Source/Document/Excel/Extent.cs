using System;
using System.Data;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Zhoubin.Infrastructure.Common.Document.Excel
{
    /// <summary>
    /// NPOI相关扩展方法
    /// </summary>
    public static class Extent
    {
        /// <summary>  
        /// DataTable导出到Excel文件  
        /// </summary>  
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="strHeaderText">表头文本</param>  
        /// <param name="strFileName">保存位置</param>  
        public static void Export(this DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (var ms = Export(dtSource, strHeaderText))
            {
                using (var fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>  
        /// DataTable导出到Excel的MemoryStream  
        /// </summary>  
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="strHeaderText">表头文本</param>  
        public static MemoryStream Export(this DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 取得每列的列宽（最大宽度）
            var arrColWidth = CreateColWidth(dtSource);

            #endregion

            int rowIndex = 0;
            ISheet sheet = null;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex % 65535 == 0)
                {
                    sheet = workbook.CreateSheet();
                    #region 表头及样式 会自动增加一行，索引自动加一
                    CreateHeaderAndStyle(dtSource.Columns.Count, strHeaderText, sheet);
                    #endregion

                    #region 列头及样式
                    CreateColHeaderAndStyle(dtSource, sheet, arrColWidth);
                    #endregion

                    rowIndex++;
                }
                #endregion


                #region 填充内容
                FillData(sheet, rowIndex % 65535, row);

                #endregion

                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet  
                return ms;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="row"></param>
        private static void FillData(ISheet sheet, int rowIndex, DataRow row)
        {
            ICellStyle contentStyle = sheet.Workbook.CreateCellStyle();
            contentStyle.Alignment = HorizontalAlignment.Left;
            IRow dataRow = sheet.CreateRow(rowIndex);
            foreach (DataColumn column in row.Table.Columns)
            {
                ICell newCell = dataRow.CreateCell(column.Ordinal);
                newCell.CellStyle = contentStyle;

                string drValue = row[column].ToString();

                switch (column.DataType.ToString())
                {
                    case "System.String": //字符串类型  
                        newCell.SetCellValue(drValue);
                        break;
                    case "System.DateTime": //日期类型  
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(dateV);

                        IDataFormat format = sheet.Workbook.CreateDataFormat();
                        ICellStyle dateStyle = sheet.Workbook.CreateCellStyle();
                        dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
                        newCell.CellStyle = dateStyle; //格式化显示  
                        break;
                    case "System.Boolean": //布尔型  
                        bool boolV;
                        bool.TryParse(drValue, out boolV);
                        newCell.SetCellValue(boolV);
                        break;
                    case "System.Int16": //整型  
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV;
                        int.TryParse(drValue, out intV);
                        newCell.SetCellValue(intV);
                        break;
                    case "System.Decimal": //浮点型  
                    case "System.Double":
                        double doubV;
                        double.TryParse(drValue, out doubV);
                        newCell.SetCellValue(doubV);
                        break;
                    case "System.DBNull": //空值处理  
                        newCell.SetCellValue("");
                        break;
                    default:
                        newCell.SetCellValue("");
                        break;
                }
            }
        }


        /// <summary>
        /// 创建列表头和样式
        /// </summary>
        /// <param name="dtSource">表格数据</param>
        /// <param name="sheet">工作表</param>
        /// <param name="arrColWidth">列宽度列表</param>
        private static void CreateColHeaderAndStyle(DataTable dtSource, ISheet sheet, int[] arrColWidth)
        {
            IRow headerRow = sheet.CreateRow(1);
            ICellStyle headStyle = sheet.Workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            IFont font = sheet.Workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = 700;
            headStyle.SetFont(font);


            foreach (DataColumn column in dtSource.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                //设置列宽  
                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
            }
        }

        /// <summary>
        /// 创建表头和样式
        /// </summary>
        /// <param name="columnCount">列数</param>
        /// <param name="strHeaderText">表头名称</param>
        /// <param name="sheet">工作表</param>
        private static void CreateHeaderAndStyle(int columnCount, string strHeaderText, ISheet sheet)
        {
            IRow headerRow = sheet.CreateRow(0);
            headerRow.HeightInPoints = 25;
            headerRow.CreateCell(0).SetCellValue(strHeaderText);

            ICellStyle headStyle = sheet.Workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            IFont font = sheet.Workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Boldweight = 700;
            headStyle.SetFont(font);

            headerRow.GetCell(0).CellStyle = headStyle;

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, columnCount - 1));
        }

        private static int[] CreateColWidth(DataTable dtSource)
        {
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                //GBK对应的code page是CP936
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            return arrColWidth;
        }
        

        /// <summary>读取excel  
        /// 默认第一行为标头  
        /// </summary>  
        /// <param name="strFileName">excel文档路径</param>  
        /// <returns></returns>  
        public static DataTable ImportToDataTable(this string strFileName)
        {
            DataTable dt = new DataTable();

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
    }
}
