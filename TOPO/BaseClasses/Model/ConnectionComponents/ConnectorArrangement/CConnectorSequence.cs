using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public abstract class CConnectorSequence
    {
        private int m_iNumberOfConnectors;

        public int INumberOfConnectors
        {
            get
            {
                return m_iNumberOfConnectors;
            }

            set
            {
                m_iNumberOfConnectors = value;
            }
        }

        /*
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
        }*/

        private Point[] m_holesCentersPoints;

        public Point[] HolesCentersPoints
        {
            get
            {
                return m_holesCentersPoints;
            }

            set
            {
                m_holesCentersPoints = value;
            }
        }

        public CConnectorSequence() { }
    }
}
