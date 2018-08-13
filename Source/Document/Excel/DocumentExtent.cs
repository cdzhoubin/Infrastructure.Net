using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace Zhoubin.Infrastructure.Common.Document.Excel
{
    /// <summary>
    /// Excel类扩展方法
    /// </summary>
    public static class DocumentExtent
    {
        /// <summary>
        /// 写入Table数据
        /// </summary>
        /// <param name="sheet">待写入Sheet</param>
        /// <param name="dt">数据表</param>
        /// <returns>返回<see cref="ISheet"/>对象</returns>
        public static ISheet Write(this ISheet sheet, DataTable dt)
        {
            if (dt != null)
            {
                var captionMap = new Dictionary<string, string>();
                foreach (DataColumn column in dt.Columns)
                    captionMap.Add(string.IsNullOrEmpty(column.Caption) ? column.ColumnName : column.Caption, column.ColumnName);
                Write(sheet, dt, captionMap);
            }
            return sheet;
        }

        /// <summary>
        /// 写入Table数据
        /// </summary>
        /// <param name="sheet">待写入Sheet</param>
        /// <param name="dt">数据表</param>
        /// <param name="captionMap">标题影射</param>
        /// <returns>返回<see cref="ISheet"/>对象</returns>
        public static ISheet Write(this ISheet sheet, DataTable dt,Dictionary<string,string> captionMap)
        {
            if (dt.Columns.Count != captionMap.Count)
            {
                throw new Exception("表格数据列和标题列表数量不相同。");
            }

            if (dt.Rows.Count > 65535)
            {
                throw new Exception("单个Sheet最大仅运行65535列");
            }

            int i = 0,j=0;
            var row = sheet.CreateRow(i);
            captionMap.Keys.ToList().ForEach(p =>
            {
                row.CreateCell(j).SetCellValue(p);
                j++;

            });

            i++;
            foreach (DataRow dr in dt.Rows)
            {
                row = sheet.CreateRow(i);
                j = 0;

                foreach (KeyValuePair<string, string> pair in captionMap)
                {
                    var cell = row.CreateCell(j);
                    if (dr[pair.Value] is DateTime)
                    {
                       cell.SetCellValue(Convert<DateTime>(dr[pair.Value]));
                       
                       cell.CellStyle = GetDateTimeStyle(sheet.Workbook);
                    }
                    else if (dr[pair.Value] is bool)
                    {
                        cell.SetCellValue(Convert<bool>(dr[pair.Value]));
                        cell.SetCellType(CellType.Boolean);
                    }
                    else if (dr[pair.Value] is double  || dr[pair.Value] is float)
                    {
                        cell.SetCellValue(Convert<double>(dr[pair.Value]));
                        cell.SetCellType(CellType.Numeric);
                    }
                    else if (dr[pair.Value] is Int || dr[pair.Value] is Int16 || dr[pair.Value] is Int32)
                    {
                        cell.SetCellValue(Convert<Int32>(dr[pair.Value]));
                        cell.SetCellType(CellType.Numeric);
                    }
                    else if (dr[pair.Value] is long)
                    {
                        cell.SetCellValue(Convert<long>(dr[pair.Value]));
                        cell.SetCellType(CellType.Numeric);
                    }
                    else if (dr[pair.Value] is decimal)
                    {
                        cell.SetCellValue(double.Parse(Convert<string>(dr[pair.Value])));
                        cell.SetCellType(CellType.Numeric);
                    }
                    else 
                    {
                        cell.SetCellValue(Convert<string>(dr[pair.Value]));
                        cell.SetCellType(CellType.String);
                    }
                    j++;
                    
                }

                i++;
            }
            

            return sheet;
        }

        private static ICellStyle _dateStyle;
        static ICellStyle GetDateTimeStyle(IWorkbook workbook)
        {
            if (_dateStyle == null)
            {
                _dateStyle = workbook.CreateCellStyle();
                _dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy-MM-dd HH:mm");
                _dateStyle.ShrinkToFit = true;
            }

            return _dateStyle;
        }
        private static T Convert<T>(object ob)
        {
            if (ob == null || ob == DBNull.Value)
            {
                return default(T);
            }

            return (T) ob;
        }
    }
}
