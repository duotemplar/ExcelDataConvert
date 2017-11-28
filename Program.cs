using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;
using ProtoBuf;
using Sprache;

namespace Excel2Protobuf
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var pb = "message SearchRequest { string query = 1; int32 page_number = 2; int32 result_per_page = 3; }";
            var parser = Postal.ProtoBuf.MessageParser.ParseText(pb);
            var def = Postal.ProtoBuf.MessageParser._fieldParser;
            var field = def.Parse(pb);
            var name2 = field.Name;

            var format = "what {0} {{0}}";

            var test = string.Format(format, "hi");
            var test2 = string.Format(test, "world");
            FileStream stream = File.Open("test.xlsx", FileMode.Open, FileAccess.Read);

            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //4. DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();
            var count = result.Tables.Count;

            var name = excelReader.Name;
            var r = excelReader.ResultsCount;
            excelReader.NextResult();
            //5. Data Reader methods
            while (excelReader.Read())
            {
                var num = excelReader.FieldCount;
                var str = excelReader.GetString(0);
                str = excelReader.GetString(3);
                var d = excelReader.Depth;
                Console.WriteLine(str);
            }


            //6. Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();

            Console.ReadLine();
        }
    }
}
