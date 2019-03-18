using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class ComboItem
    {
        private int m_ID;
        private string m_Name;

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

        public ComboItem() { }

        public ComboItem(int id, string name)
        {
            m_ID = id;
            m_Name = name;
        }
    }
}
