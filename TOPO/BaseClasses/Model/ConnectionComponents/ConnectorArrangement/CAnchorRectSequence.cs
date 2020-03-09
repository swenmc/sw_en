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
        private int m_iNumberOfAnchorsInColumn_yDirection;
        private float m_fDistanceOfPointsX;
        private float m_fDistanceOfPointsY;

        double m_RefPointX;
        double m_RefPointY;

        

        private bool m_SameDistancesX;
        private bool m_SameDistancesY;
        private List<float> m_DistancesOfPointsX;
        private List<float> m_DistancesOfPointsY;

        public int NumberOfAnchorsInRow_xDirection
        {
            get
            {
                return m_iNumberOfAnchorsInRow_xDirection;
            }

            set
            {
                m_iNumberOfAnchorsInRow_xDirection = value;
                INumberOfConnectors = m_iNumberOfAnchorsInRow_xDirection * m_iNumberOfAnchorsInColumn_yDirection;
                SetDistancesX();
            }
        }

        

        public int NumberOfAnchorsInColumn_yDirection
        {
            get
            {
                return m_iNumberOfAnchorsInColumn_yDirection;
            }

            set
            {
                m_iNumberOfAnchorsInColumn_yDirection = value;
                INumberOfConnectors = m_iNumberOfAnchorsInRow_xDirection * m_iNumberOfAnchorsInColumn_yDirection;
                SetDistancesY();
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
                SetDistancesX();
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
                SetDistancesY();
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
                //ReferencePoint = new Point(RefPointX, RefPointY);
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
                //ReferencePoint = new Point(RefPointX, RefPointY);
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
                if (m_SameDistancesX == false) SetDistancesX();
                else SetDistanceX();
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
                if (m_SameDistancesY == false) SetDistancesY();
                else SetDistanceY();
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
                if (m_DistancesOfPointsX != null)
                {
                    if (m_DistancesOfPointsX.Count > 0) m_fDistanceOfPointsX = m_DistancesOfPointsX.First();
                }
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
                if (m_DistancesOfPointsY != null)
                {
                    if (m_DistancesOfPointsY.Count > 0) m_fDistanceOfPointsY = m_DistancesOfPointsY.First();
                }
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


        private void SetDistancesX()
        {
            m_DistancesOfPointsX = new List<float>();
            for (int i = 0; i < m_iNumberOfAnchorsInRow_xDirection - 1; i++)
            {
                m_DistancesOfPointsX.Add(m_fDistanceOfPointsX);
            }
        }
        private void SetDistancesY()
        {
            m_DistancesOfPointsY = new List<float>();
            for (int i = 0; i < m_iNumberOfAnchorsInColumn_yDirection - 1; i++)
            {
                m_DistancesOfPointsY.Add(m_fDistanceOfPointsY);
            }
        }
        private void SetDistanceX()
        {
            if (m_DistancesOfPointsX != null && m_DistancesOfPointsX.Count > 0) DistanceOfPointsX = m_DistancesOfPointsX.First();
        }
        private void SetDistanceY()
        {
            if (m_DistancesOfPointsY != null && m_DistancesOfPointsY.Count > 0) DistanceOfPointsY = m_DistancesOfPointsY.First();
        }
    }
}
