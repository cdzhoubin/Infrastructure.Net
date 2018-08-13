using System;
using System.Data;
using System.IO;
using System.Text;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.Document
{
    public class CsvDocumnet : DocumentBase<DataTable>
    {
        private Encoding _encoding;
        private string _file;
        private bool _firstRowForTitle;
        public CsvDocumnet(string file,Encoding encoding,bool firstRowForTitle) : base(true)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(file);
            }
            _file = file;
            _firstRowForTitle = firstRowForTitle;
            _encoding = encoding;
        }
        public CsvDocumnet(DataTable csvData, Encoding encoding, bool firstRowForTitle) : base(false)
        {
            if (csvData == null)
            {
                throw new ArgumentNullException(nameof(csvData));
            }
            _csvData = csvData;
            _firstRowForTitle = firstRowForTitle;
            _encoding = encoding;
        }
        protected override void DocumentInitialize()
        {
            
        }
        DataTable _csvData;
        protected override DataTable DocumentRead()
        {
            return _file.CsvToDataTable(_encoding, _firstRowForTitle);
        }

        protected override void DocumentOpen()
        {
            
        }

        protected override void DocumentSave(Stream stream)
        {
            StringBuilder builder = new StringBuilder();
            if (_firstRowForTitle)
            {
                AppendLine(builder, _csvData.Columns, str => str);
            }
            if (_csvData.Rows.Count > 0)
            {
                builder.AppendLine();
            }
            for (int i = 0; i < _csvData.Rows.Count; i++)
            {
                AppendLine(builder, _csvData.Columns, str => _csvData.Rows[i][str].ToString());
                if (i < _csvData.Rows.Count - 1)
                {
                    builder.AppendLine();
                }
            }
            byte[] buffer = _encoding.GetBytes(builder.ToString());
            stream.Write(buffer, 0, buffer.Length);
        }

        private void AppendLine(StringBuilder sb, DataColumnCollection dcc, Func<string, string> func)
        {
            bool firstColumn = true;
            for (int j = 0; j < dcc.Count; j++)
            {
                if (!firstColumn)
                    sb.Append(',');
                sb.Append(CreateValue(func(dcc[j].ColumnName)));
                firstColumn = false;
            }
        }


        private string CreateValue(string str)
        {
            if (str.IndexOfAny(new char[] {'"', ','}) != -1)
            {
                return string.Format("\"{0}\"", str.Replace("\"", "\"\""));
            }
            return str;
        }
    }
}