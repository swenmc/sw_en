using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CAnchorRectSequence : CAnchorSequence
    {
        private int m_iNumberOfAnchorsInRow_xDirection;

        public int NumberOfAnchorsInRow_xDirection
        {
            get
            {
                return m_iNumberOfAnchorsInRow_xDirection;
            }

            set
            {
                m_iNumberOfAnchorsInRow_xDirection = value;
            }
        }

        private int m_iNumberOfAnchorsInColumn_yDirection;

        public int NumberOfAnchorsInColumn_yDirection
        {
            get
            {
                return m_iNumberOfAnchorsInColumn_yDirection;
            }

            set
            {
                m_iNumberOfAnchorsInColumn_yDirection = value;
            }
        }

        private float m_fDistanceOfPointsX;

        public float DistanceOfPointsX
        {
            get
            {
                return m_fDistanceOfPointsX;
            }

            set
            {
                m_fDistanceOfPointsX = value;
            }
        }

        private float m_fDistanceOfPointsY;

        public float DistanceOfPointsY
        {
            get
            {
                return m_fDistanceOfPointsY;
            }

            set
            {
                m_fDistanceOfPointsY = value;
            }
        }

        public CAnchorRectSequence()
        { }

        public CAnchorRectSequence(int iNumberOfAnchorsInRow_xDirection_temp, int iNumberOfAnchorsInColumn_yDirection_temp)
        {
            NumberOfAnchorsInRow_xDirection = iNumberOfAnchorsInRow_xDirection_temp;
            NumberOfAnchorsInColumn_yDirection = iNumberOfAnchorsInColumn_yDirection_temp;
            INumberOfConnectors = NumberOfAnchorsInRow_xDirection * NumberOfAnchorsInColumn_yDirection;
            HolesCentersPoints = new Point[INumberOfConnectors];
        }
    }
}
