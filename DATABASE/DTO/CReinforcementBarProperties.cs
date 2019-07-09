using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CReinforcementBarProperties
    {
        private int m_ID;
        private float m_Diameter_mm;
        private float m_Diameter_m;
        private float m_Area_As1;
        private float m_Mass;

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

        public float Diameter_mm
        {
            get
            {
                return m_Diameter_mm;
            }

            set
            {
                m_Diameter_mm = value;
            }
        }

        public float Diameter_m
        {
            get
            {
                return m_Diameter_m;
            }

            set
            {
                m_Diameter_m = value;
            }
        }

        public float Area_As1
        {
            get
            {
                return m_Area_As1;
            }

            set
            {
                m_Area_As1 = value;
            }
        }

        public float Mass // kg/m
        {
            get
            {
                return m_Mass;
            }

            set
            {
                m_Mass = value;
            }
        }


        public CReinforcementBarProperties() { }
    }
}
