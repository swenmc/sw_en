using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CMatPropertiesBOLT
    {
        // Bolts / Anchors
        private int m_ID;
        private string m_Standard;
        private string m_Grade;
        private double m_E;
        private double m_G;
        private double m_Nu;

        private double m_fy;
        private double m_fu;
        private double m_Rho;
        private double m_Alpha;

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

        public string Grade
        {
            get
            {
                return m_Grade;
            }

            set
            {
                m_Grade = value;
            }
        }

        public double E
        {
            get
            {
                return m_E;
            }

            set
            {
                m_E = value;
            }
        }

        public double G
        {
            get
            {
                return m_G;
            }

            set
            {
                m_G = value;
            }
        }

        public double Nu
        {
            get
            {
                return m_Nu;
            }

            set
            {
                m_Nu = value;
            }
        }

        public double Fy
        {
            get
            {
                return m_fy;
            }

            set
            {
                m_fy = value;
            }
        }

        public double Fu
        {
            get
            {
                return m_fu;
            }

            set
            {
                m_fu = value;
            }
        }

        public double Rho
        {
            get
            {
                return m_Rho;
            }

            set
            {
                m_Rho = value;
            }
        }

        public double Alpha
        {
            get
            {
                return m_Alpha;
            }

            set
            {
                m_Alpha = value;
            }
        }

        public CMatPropertiesBOLT() { }
    }
}
