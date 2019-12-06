using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CBoltNutProperties
    {
        private int m_ID;
        private string m_Name;
        private string m_Standard;
        private double m_Pitch_coarse;
        private double m_SizeAcrossFlats_max;
        private double m_SizeAcrossFlats_min;
        private double m_SizeAcrossCorners;
        private double m_Thickness_max;
        private double m_Thickness_min;
        private double m_Mass_kg;
        private double m_Price_PPKG_NZD;
        private double m_Price_PPP_NZD;

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

        public double Pitch_coarse
        {
            get
            {
                return m_Pitch_coarse;
            }

            set
            {
                m_Pitch_coarse = value;
            }
        }

        public double SizeAcrossFlats_max
        {
            get
            {
                return m_SizeAcrossFlats_max;
            }

            set
            {
                m_SizeAcrossFlats_max = value;
            }
        }

        public double SizeAcrossFlats_min
        {
            get
            {
                return m_SizeAcrossFlats_min;
            }

            set
            {
                m_SizeAcrossFlats_min = value;
            }
        }

        public double SizeAcrossCorners
        {
            get
            {
                return m_SizeAcrossCorners;
            }

            set
            {
                m_SizeAcrossCorners = value;
            }
        }

        public double Thickness_max
        {
            get
            {
                return m_Thickness_max;
            }

            set
            {
                m_Thickness_max = value;
            }
        }

        public double Thickness_min
        {
            get
            {
                return m_Thickness_min;
            }

            set
            {
                m_Thickness_min = value;
            }
        }

        public double Mass
        {
            get
            {
                return m_Mass_kg;
            }

            set
            {
                m_Mass_kg = value;
            }
        }

        public double Price_PPKG_NZD
        {
            get
            {
                return m_Price_PPKG_NZD;
            }

            set
            {
                m_Price_PPKG_NZD = value;
            }
        }

        public double Price_PPP_NZD
        {
            get
            {
                return m_Price_PPP_NZD;
            }

            set
            {
                m_Price_PPP_NZD = value;
            }
        }

        public CBoltNutProperties() { }
    }
}
