using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CPlate_LL_Properties
    {
        private int m_ID;
        private string m_Name;
        private double m_dim11;
        private double m_dim12;
        private double m_dim2y;
        private double m_dim3;
        private double m_thickness;
        private int m_iNumberOfHolesScrews;
        private double m_totalDim_x;
        private double m_totalDim_y;
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

        public double dim11
        {
            get
            {
                return m_dim11;
            }

            set
            {
                m_dim11 = value;
            }
        }

        public double dim12
        {
            get
            {
                return m_dim12;
            }

            set
            {
                m_dim12 = value;
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

        public double dim3
        {
            get
            {
                return m_dim3;
            }

            set
            {
                m_dim3 = value;
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

        public int NumberOfHolesScrews
        {
            get
            {
                return m_iNumberOfHolesScrews;
            }

            set
            {
                m_iNumberOfHolesScrews = value;
            }
        }

        public double TotalDim_x
        {
            get
            {
                return m_totalDim_x;
            }

            set
            {
                m_totalDim_x = value;
            }
        }

        public double TotalDim_y
        {
            get
            {
                return m_totalDim_y;
            }

            set
            {
                m_totalDim_y = value;
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

        public CPlate_LL_Properties() { }
    }
}
