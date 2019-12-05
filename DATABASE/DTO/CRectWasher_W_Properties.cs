using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CRectWasher_W_Properties
    {
        private int m_ID;
        private string m_Name;
        private double m_dim1x;
        private double m_dim2y;
        private double m_thickness;
        private int m_iNumberOfHoles;
        private double m_area;
        private double m_volume;
        private double m_mass;
        private double m_price_PPSM_NZD;
        private double m_price_PPKG_NZD;
        private double m_price_PPP_NZD;

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

        public double dim1x
        {
            get
            {
                return m_dim1x;
            }

            set
            {
                m_dim1x = value;
            }
        }

        public double dim2y
        {
            get
            {
                return m_dim2y;
            }

            set
            {
                m_dim2y = value;
            }
        }

        public double thickness
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

        public int NumberOfHoles
        {
            get
            {
                return m_iNumberOfHoles;
            }

            set
            {
                m_iNumberOfHoles = value;
            }
        }

        public double Area
        {
            get
            {
                return m_area;
            }

            set
            {
                m_area = value;
            }
        }

        public double Volume
        {
            get
            {
                return m_volume;
            }

            set
            {
                m_volume = value;
            }
        }

        public double Mass
        {
            get
            {
                return m_mass;
            }

            set
            {
                m_mass = value;
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

        public CRectWasher_W_Properties() { }
    }
}
