using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CLengthItemProperties
    {
        private int m_ID;
        private string m_Name;
        private double m_thickness;
        private double m_width_total;
        private double m_mass_kg_m2;
        private double m_mass_kg_lm;
        private double m_price_PPLM_NZD;
        private double m_price_PPSM_NZD;
        private double m_price_PPKG_NZD;
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

        public double Width_total
        {
            get
            {
                return m_width_total;
            }

            set
            {
                m_width_total = value;
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

        public double Mass_kg_lm
        {
            get
            {
                return m_mass_kg_lm;
            }

            set
            {
                m_mass_kg_lm = value;
            }
        }

        public double Price_PPLM_NZD
        {
            get
            {
                return m_price_PPLM_NZD;
            }

            set
            {
                m_price_PPLM_NZD = value;
            }
        }

        public double Price_PPSM_NZD
        {
            get
            {
                return m_price_PPSM_NZD;
            }

            set
            {
                m_price_PPSM_NZD = value;
            }
        }

        public double Price_PPKG_NZD
        {
            get
            {
                return m_price_PPKG_NZD;
            }

            set
            {
                m_price_PPKG_NZD = value;
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

        public CLengthItemProperties() { }
    }
}
