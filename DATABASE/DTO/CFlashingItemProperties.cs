using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    [Serializable]
    public class CFlashingItemProperties
    {
        private int m_ID;
        private string m_prefix;
        private int m_type_ID;
        private string m_type_Name;
        private int m_group_ID;
        private string m_group_Name;
        private string m_elements_snakeModel_deg_mm;
        private double [] m_ArrElements_snakeModel_deg_mm;
        private double m_width_total;
        private double m_thickness;
        private double m_mass_kg_lm;
        private double m_price_PPLM_NZD;
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

        public string Prefix
        {
            get
            {
                return m_prefix;
            }

            set
            {
                m_prefix = value;
            }
        }

        public int Type_ID
        {
            get
            {
                return m_type_ID;
            }

            set
            {
                m_type_ID = value;
            }
        }

        public string Type_Name
        {
            get
            {
                return m_type_Name;
            }

            set
            {
                m_type_Name = value;
            }
        }

        public int Group_ID
        {
            get
            {
                return m_group_ID;
            }

            set
            {
                m_group_ID = value;
            }
        }

        public string Group_Name
        {
            get
            {
                return m_group_Name;
            }

            set
            {
                m_group_Name = value;
            }
        }

        public string Elements_snakeModel_deg_mm
        {
            get
            {
                return m_elements_snakeModel_deg_mm;
            }

            set
            {
                m_elements_snakeModel_deg_mm = value;
            }
        }

        public double[] ArrElements_snakeModel_deg_mm
        {
            get
            {
                return m_ArrElements_snakeModel_deg_mm;
            }

            set
            {
                m_ArrElements_snakeModel_deg_mm = value;
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

        public CFlashingItemProperties() { }
    }
}