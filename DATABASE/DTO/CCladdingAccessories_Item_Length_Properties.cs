using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CCladdingAccessories_Item_Length_Properties
    {
        private int m_ID;
        private string m_Name;
        private string m_code;
        private string m_Standard;
        private double m_mass_kg_per_m;
        private double m_price_PPLM_NZD;
        private double m_price_PPKG_NZD;
        private int m_GCD_page;
        private string m_Note;

        private int[] m_fixingIDsArray;

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

        public string Code
        {
            get
            {
                return m_code;
            }

            set
            {
                m_code = value;
            }
        }

        public string Standard
        {
            get
            {
                return m_Standard;
            }

            set
            {
                m_Standard = value;
            }
        }

        public double Mass_kg_per_m
        {
            get
            {
                return m_mass_kg_per_m;
            }

            set
            {
                m_mass_kg_per_m = value;
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

        public int GCD_page
        {
            get
            {
                return m_GCD_page;
            }

            set
            {
                m_GCD_page = value;
            }
        }

        public string Note
        {
            get
            {
                return m_Note;
            }

            set
            {
                m_Note = value;
            }
        }

        public CCladdingAccessories_Item_Length_Properties(){}
    }
}