using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CDownpipeProperties
    {
        private int m_ID;
        private string m_Name;
        private string m_Shape;
        private double m_diameter;
        private double m_length;
        private double m_mass_kg_lm;
        private double m_mass_kg_piece;
        private double m_price_PPLM_NZD;
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

        public string Shape
        {
            get
            {
                return m_Shape;
            }

            set
            {
                m_Shape = value;
            }
        }

        public double Diameter
        {
            get
            {
                return m_diameter;
            }

            set
            {
                m_diameter = value;
            }
        }

        public double Length
        {
            get
            {
                return m_length;
            }

            set
            {
                m_length = value;
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

        public double Mass_kg_piece
        {
            get
            {
                return m_mass_kg_piece;
            }

            set
            {
                m_mass_kg_piece = value;
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

        public CDownpipeProperties() { }
    }
}
