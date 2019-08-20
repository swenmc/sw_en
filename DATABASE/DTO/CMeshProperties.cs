using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CMeshProperties
    {
        private int m_ID;
        private string m_Name;
        private string m_MaterialName;
        private double m_wireDiameter;
        private double m_centersDistance;

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

        public string MaterialName
        {
            get
            {
                return m_MaterialName;
            }

            set
            {
                m_MaterialName = value;
            }
        }

        public double WireDiameter
        {
            get
            {
                return m_wireDiameter;
            }

            set
            {
                m_wireDiameter = value;
            }
        }

        public double CentersDistance
        {
            get
            {
                return m_centersDistance;
            }

            set
            {
                m_centersDistance = value;
            }
        }

        public CMeshProperties() { }
    }
}
