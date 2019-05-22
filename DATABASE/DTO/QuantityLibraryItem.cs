using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class QuantityLibraryItem
    {
        int m_ID;
        string m_UnitIdentificator;
        string m_BasicUnit;
        string m_GUIUnit;
        float m_GUIUnitFactor;
        int m_GUIDecimalPlaces;
        string m_ReportUnit;
        float m_ReportUnitFactor;
        int m_ReportDecimalPlaces;

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

        public string BasicUnit
        {
            get
            {
                return m_BasicUnit;
            }

            set
            {
                m_BasicUnit = value;
            }
        }

        public string GUIUnit
        {
            get
            {
                return m_GUIUnit;
            }

            set
            {
                m_GUIUnit = value;
            }
        }

        public float GUIUnitFactor
        {
            get
            {
                return m_GUIUnitFactor;
            }

            set
            {
                m_GUIUnitFactor = value;
            }
        }

        public int GUIDecimalPlaces
        {
            get
            {
                return m_GUIDecimalPlaces;
            }

            set
            {
                m_GUIDecimalPlaces = value;
            }
        }

        public string ReportUnit
        {
            get
            {
                return m_ReportUnit;
            }

            set
            {
                m_ReportUnit = value;
            }
        }

        public float ReportUnitFactor
        {
            get
            {
                return m_ReportUnitFactor;
            }

            set
            {
                m_ReportUnitFactor = value;
            }
        }

        public int ReportDecimalPlaces
        {
            get
            {
                return m_ReportDecimalPlaces;
            }

            set
            {
                m_ReportDecimalPlaces = value;
            }
        }

        public QuantityLibraryItem() { }
    }
}
