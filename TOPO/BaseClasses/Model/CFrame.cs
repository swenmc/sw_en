using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CFrame
    {
        List<CMember> m_Members;
        public List<CMember> Members
        {
            get
            {
                return m_Members;
            }

            set
            {
                m_Members = value;
            }
        }
        public CFrame()
        {
            m_Members = new List<CMember>();
        }

        
    }
}
