using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CConnectionDescription
    {
        private int m_ID;
        private string m_Name;
        private string m_JoinType;
        
        public int ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }

        public string JoinType
        {
            get
            {
                return m_JoinType;
            }

            set
            {
                m_JoinType = value;
            }
        }

        public CConnectionDescription() { }
    }
}
