using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewRectSequence : CScrewSequence
    {
        private int m_iNumberOfScrewsInRow_xDirection;

        public int NumberOfScrewsInRow_xDirection
        {
            get
            {
                return m_iNumberOfScrewsInRow_xDirection;
            }

            set
            {
                m_iNumberOfScrewsInRow_xDirection = value;
            }
        }

        private int m_iNumberOfScrewsInColumn_yDirection;

        public int NumberOfScrewsInColumn_yDirection
        {
            get
            {
                return m_iNumberOfScrewsInColumn_yDirection;
            }

            set
            {
                m_iNumberOfScrewsInColumn_yDirection = value;
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

        private double m_RefPointX;
        private double m_RefPointY;

        public double RefPointX
        {
            get
            {
                return m_RefPointX;
            }

            set
            {
                m_RefPointX = value;
                ReferencePoint = new Point(RefPointX, RefPointY);
            }
        }

        public double RefPointY
        {
            get
            {
                return m_RefPointY;
            }

            set
            {
                m_RefPointY = value;
                ReferencePoint = new Point(RefPointX, RefPointY);
            }
        }

        

        public CScrewRectSequence()
        { }

        public CScrewRectSequence(int iNumberOfScrewsInRow_xDirection_temp, int iNumberOfScrewsInColumn_yDirection_temp)
        {
            NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_temp;
            NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_temp;
            INumberOfConnectors = NumberOfScrewsInRow_xDirection * NumberOfScrewsInColumn_yDirection;
            HolesCentersPoints = new Point[INumberOfConnectors];            
        }

        public CScrewRectSequence(int iNumberOfScrewsInRow_xDirection, int iNumberOfScrewsInColumn_yDirection, float refPointX, float refPointY, float distanceOfPointsX, float distanceOfPointsY)
        {
            NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection;
            NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection;

            m_RefPointX = refPointX;
            m_RefPointY = refPointY;
            m_fDistanceOfPointsX = distanceOfPointsX;
            m_fDistanceOfPointsY = distanceOfPointsY;

            INumberOfConnectors = NumberOfScrewsInRow_xDirection * NumberOfScrewsInColumn_yDirection;
            HolesCentersPoints = new Point[INumberOfConnectors];
        }
        
    }
}
