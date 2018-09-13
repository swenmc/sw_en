using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MATH;


namespace BaseClasses
{
    public class CConnectorArrangement
    {
        private int m_iHolesNumber;

        public int IHolesNumber
        {
            get
            {
                return m_iHolesNumber;
            }

            set
            {
                m_iHolesNumber = value;
            }
        }

        private float[,] m_HolesCentersPoints2D;

        public float[,] HolesCentersPoints2D
        {
            get
            {
                return m_HolesCentersPoints2D;
            }

            set
            {
                m_HolesCentersPoints2D = value;
            }
        }

        public CConnectorArrangement()
        {}

        public CConnectorArrangement(int iHolesNumber)
        {
            HolesCentersPoints2D = new float[IHolesNumber, 2];
        }
    }
}
