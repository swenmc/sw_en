using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MATH;

namespace BaseClasses
{
    public abstract class CConnectorArrangement
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

        private Point[] m_HolesCentersPoints2D;

        public Point[] HolesCentersPoints2D
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
            HolesCentersPoints2D = new Point[IHolesNumber];
        }
    }
}
