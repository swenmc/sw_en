using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CPlaneItemProperties
    {
        private int m_ID;
        private string m_Name;
        private double m_thickness;
        private double m_mass_kg_m2;
        private double m_price1_PPSM_NZD;
        private double m_price1_PPKG_NZD;
        private double m_price2_PPSM_NZD;
        private double m_price2_PPKG_NZD;
        private double m_price3_PPSM_NZD;
        private double m_price3_PPKG_NZD;
        private string m_note;

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

        public double Thickness
        {
            get
            {
                return m_thickness;
            }

            set
            {
                m_thickness = value;
            }
        }

        public double Mass_kg_m2
        {
            get
            {
                return m_mass_kg_m2;
            }

            set
            {
                m_mass_kg_m2 = value;
            }
        }

        public double Price1_PPSM_NZD
        {
            get
            {
                return m_price1_PPSM_NZD;
            }

            set
            {
                m_price1_PPSM_NZD = value;
            }
        }

        public double Price1_PPKG_NZD
        {
            get
            {
                return m_price1_PPKG_NZD;
            }

            set
            {
                m_price1_PPKG_NZD = value;
            }
        }

        public double Price2_PPSM_NZD
        {
            get
            {
                return m_price2_PPSM_NZD;
            }

            set
            {
                m_price2_PPSM_NZD = value;
            }
        }

        public double Price2_PPKG_NZD
        {
            get
            {
                return m_price2_PPKG_NZD;
            }

            set
            {
                m_price2_PPKG_NZD = value;
            }
        }

        public double Price3_PPSM_NZD
        {
            get
            {
                return m_price3_PPSM_NZD;
            }

            set
            {
                m_price3_PPSM_NZD = value;
            }
        }

        public double Price3_PPKG_NZD
        {
            get
            {
                return m_price3_PPKG_NZD;
            }

            set
            {
                m_price3_PPKG_NZD = value;
            }
        }

        public string Note
        {
            get
            {
                return m_note;
            }

            set
            {
                m_note = value;
            }
        }

        public CPlaneItemProperties() { }
    }
}
