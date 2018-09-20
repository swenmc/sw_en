using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class CScrewSequence
    {
        private int m_iNumberOfScrews;

        public int INumberOfScrews
        {
            get
            {
                return m_iNumberOfScrews;
            }

            set
            {
                m_iNumberOfScrews = value;
            }
        }

        private Point m_RefPoint;

        public Point ReferencePoint
        {
            get
            {
                return m_RefPoint;
            }

            set
            {
                m_RefPoint = value;
            }
        }

        private Point[] m_holesCentersPoints;

        public Point[] HolesCentersPoints
        {
            get
            {
                //if (m_holesCentersPoints == null) m_holesCentersPoints = new Point[0];
                return m_holesCentersPoints;
            }

            set
            {
                m_holesCentersPoints = value;
            }
        }

        public CScrewSequence() { }
    }
}
