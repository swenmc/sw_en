using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class DataExportTables
    {
        int m_ID;
        string m_Description_ENU_USA;
        string m_Description_CSY;
        string m_Description_SVK;
        string m_Symbol;
        string m_Identificator;
        string m_Unit;
        string m_UnitIdentificator;

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

        public string Description_ENU_USA
        {
            get
            {
                return m_Description_ENU_USA;
            }

            set
            {
                m_Description_ENU_USA = value;
            }
        }

        public string Description_CSY
        {
            get
            {
                return m_Description_CSY;
            }

            set
            {
                m_Description_CSY = value;
            }
        }

        public string Description_SVK
        {
            get
            {
                return m_Description_SVK;
            }

            set
            {
                m_Description_SVK = value;
            }
        }

        public string Symbol
        {
            get
            {
                return m_Symbol;
            }

            set
            {
                m_Symbol = value;
            }
        }

        public string Identificator
        {
            get
            {
                return m_Identificator;
            }

            set
            {
                m_Identificator = value;
            }
        }

        public string Unit
        {
            get
            {
                return m_Unit;
            }

            set
            {
                m_Unit = value;
            }
        }
        
        public string UnitIdentificator
        {
            get
            {
                return m_UnitIdentificator;
            }

            set
            {
                m_UnitIdentificator = value;
            }
        }

        public DataExportTables() { }
    }
}
