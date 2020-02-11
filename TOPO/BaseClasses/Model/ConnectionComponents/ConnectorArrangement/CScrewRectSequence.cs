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
        private int m_iNumberOfScrewsInColumn_yDirection;
        
        private double m_RefPointX;
        private double m_RefPointY;        
        private float m_fDistanceOfPointsX;
        private float m_fDistanceOfPointsY;

        private bool m_SameDistancesX;
        private bool m_SameDistancesY;
        private List<float> m_DistancesOfPointsX;
        private List<float> m_DistancesOfPointsY;
        

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

        public bool SameDistancesX
        {
            get
            {
                return m_SameDistancesX;
            }

            set
            {
                m_SameDistancesX = value;
            }
        }

        public bool SameDistancesY
        {
            get
            {
                return m_SameDistancesY;
            }

            set
            {
                m_SameDistancesY = value;
            }
        }

        public List<float> DistancesOfPointsX
        {
            get
            {
                if (m_DistancesOfPointsX == null) SetDistancesX();
                return m_DistancesOfPointsX;
            }

            set
            {
                m_DistancesOfPointsX = value;
            }
        }

        public List<float> DistancesOfPointsY
        {
            get
            {
                if (m_DistancesOfPointsY == null) SetDistancesY();
                return m_DistancesOfPointsY;
            }

            set
            {
                m_DistancesOfPointsY = value;
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

        private void SetDistancesX()
        {
            m_DistancesOfPointsX = new List<float>();
            for (int i = 0; i < m_iNumberOfScrewsInRow_xDirection - 1; i++)
            {
                m_DistancesOfPointsX.Add(m_fDistanceOfPointsX);
            }
        }
        private void SetDistancesY()
        {
            m_DistancesOfPointsY = new List<float>();
            for (int i = 0; i < m_iNumberOfScrewsInColumn_yDirection - 1; i++)
            {
                m_DistancesOfPointsY.Add(m_fDistanceOfPointsY);
            }
        }

    }
}
