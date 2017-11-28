using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2Protobuf
{
    public class ClassInfoMgr
    {
        private static ClassInfoMgr sm_mgr;

        private List<ClassInfo> m_InfoList = new List<ClassInfo>();
        public static ClassInfoMgr GetIt()
        {
            if(sm_mgr == null)
            {
                sm_mgr = new ClassInfoMgr();
            }

            return sm_mgr;
        }

        public void AddClass(ClassInfo info)
        {
            m_InfoList.Add(info);
        }

        public ClassInfo[] GetAllClass()
        {
            return m_InfoList.ToArray();
        }

        public ClassInfo GetClass(string name)
        {
            var cls = m_InfoList.Find(c => c.Name == name);
            return cls;
        }
    }
}
