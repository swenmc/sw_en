using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CCladdingAccessories_Item_Properties
    {
        private int m_ID;
        private string m_Name;
        private string m_code;
        private double m_default_spacing_m;
        private string m_Standard;
        private bool m_isFixingItem;
        private string m_fixingIDs;
        private double m_mass_kg;
        private double m_price_PPP_NZD;
        private double m_price_PPKG_NZD;
        private int m_GCD_page;
        private string m_Note1;
        private string m_Note2;

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

        public double Default_spacing_m
        {
            get
            {
                return m_default_spacing_m;
            }

            set
            {
                m_default_spacing_m = value;
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

        public bool IsFixingItem
        {
            get
            {
                return m_isFixingItem;
            }

            set
            {
                m_isFixingItem = value;
            }
        }

        public string FixingIDs
        {
            get
            {
                return m_fixingIDs;
            }

            set
            {
                m_fixingIDs = value;
            }
        }

        public double Mass_kg
        {
            get
            {
                return m_mass_kg;
            }

            set
            {
                m_mass_kg = value;
            }
        }

        public double Price_PPP_NZD
        {
            get
            {
                return m_price_PPP_NZD;
            }

            set
            {
                m_price_PPP_NZD = value;
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

        public string Note1
        {
            get
            {
                return m_Note1;
            }

            set
            {
                m_Note1 = value;
            }
        }

        public string Note2
        {
            get
            {
                return m_Note2;
            }

            set
            {
                m_Note2 = value;
            }
        }

        public int [] FixingIDsArray
        {
            get
            {
                return m_fixingIDsArray;
            }

            set
            {
                m_fixingIDsArray = value;
            }
        }

        public CCladdingAccessories_Item_Properties(){}
    }
}