using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
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

        public CScrewRectSequence()
        { }
    }
}
