using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;

namespace Excel2Protobuf
{
    public class ClassInfo
    {
        public string Name{get; set;}

        public List<FieldInfo> Fields{get; set;}
        private List<FieldInfo> m_FieldList = new List<FieldInfo>(); 
        public void AddField(FieldInfo info)
        {
            m_FieldList.Add(info);
        }

        public FieldInfo[] GetAllFields()
        {
            return m_FieldList.ToArray();
        }

        public string Serialize()
        {
            var str = string.Format("Message {0} {{0}}", Name);
            var define = new StringBuilder();
            var index = 1;
            foreach(var fieldInfo in m_FieldList)
            {
                define.AppendFormat(fieldInfo.Serialize(), index);
                define.Append(";");
                define.Append(Environment.NewLine);
            }

            return string.Format(str, define.ToString());

        }
    }
    public class FieldInfo
    {
        public string Name { get; set; }
        //public int ColumnIndex { get; set; }
        public string TypeName { get; set; }

        public string Serialize()
        {
            var format = "";
            switch(TypeName)
            {
                case "int":
                    format = "sint32";
                    break;
                case "long":
                    format = "sint64";
                    break;
                case "ulong":
                    format = "uint64";
                    break;
                case "double":
                    format = "double";
                    break;
                case "bool":
                    format = "bool";
                    break;
                case "string":
                default:
                    format = "string";
                    break;

            }

            return String.Format("{0} {1} ={{0}}", format, Name);
        }
    }

    class SerializeToPb
    {
        private FileStream mStream;
        private IExcelDataReader mReader;
        public SerializeToPb(string path)
        {
            mStream = File.Open(path, FileMode.Open, FileAccess.Read);
            mReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
            mReader.IsFirstRowAsColumnNames = true;
        }

        public void ReadTables()
        {
            do
            {
                var cls = new ClassInfo();
                cls.Name = mReader.Name;
                ClassInfoMgr.GetIt().AddClass(cls);
                ReadColumnHeader();
            } while(!mReader.NextResult());
        }

        private void ReadColumnHeader()
        {
            //get max column used num
            mReader.Read();
            ReadFieldName();

            mReader.Read();
            GetMessageTypeName();
        }

        private void ReadFieldName()
        {
            var clsname = mReader.Name;
            var cls = ClassInfoMgr.GetIt().GetClass(clsname);
            var ColNum = mReader.FieldCount;
            for(int i = 0; i < ColNum; i++)
            {
                var str = mReader.GetString(i);
                if (string.IsNullOrEmpty(str) == false)
                {
                    var info = new FieldInfo();
                    info.Name = str;
                    //info.ColumnIndex = i;
                    cls.AddField(info);
                }
            }
           
        }

        private void GetMessageTypeName()
        {
            var clsname = mReader.Name;
            var cls = ClassInfoMgr.GetIt().GetClass(clsname);

            var fields = cls.GetAllFields();
            foreach(var field in fields)
            {
                var index = mReader.GetOrdinal(field.Name);
                field.TypeName = mReader.GetString(index);
            }
        }

        public void GeneratorPb()
        {
            var cls_list = ClassInfoMgr.GetIt().GetAllClass();
            foreach(var cls in cls_list)
            {
                FileStream file = new FileStream(cls.Name, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(file);
                var content = cls.Serialize();
                writer.Write(content);
                writer.Close();
            }
        }
    }
}
