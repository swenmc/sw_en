using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CSectionPropertiesText
    {
        private int m_ID;
        private string m_text;
        private string m_symbol;
        private string m_name;
        private string m_value;
        private string m_unit_SI;
        private string m_unit_NcmkPa;
        private string m_unit_NmmMpa;

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

        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                m_text = value;
            }
        }

        public string Symbol
        {
            get
            {
                return m_symbol;
            }

            set
            {
                m_symbol = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }

            set
            {
                m_name = value;
            }
        }

        public string Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }

        public string Unit_SI
        {
            get
            {
                return m_unit_SI;
            }

            set
            {
                m_unit_SI = value;
            }
        }

        public string Unit_NcmkPa
        {
            get
            {
                return m_unit_NcmkPa;
            }

            set
            {
                m_unit_NcmkPa = value;
            }
        }

        public string Unit_NmmMpa
        {
            get
            {
                return m_unit_NmmMpa;
            }

            set
            {
                m_unit_NmmMpa = value;
            }
        }

        public CSectionPropertiesText() { }
    }
}
