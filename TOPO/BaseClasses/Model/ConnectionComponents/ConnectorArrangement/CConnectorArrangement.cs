using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MATH;

namespace BaseClasses
{
    [Serializable]
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

        public Point[] GetRegularArrayOfPointsInCartesianCoordinates(Point refPoint, int iNumberOfPointsInXDirection, int iNumberOfPointsInYDirection, float fDistanceOfPointsX, float fDistanceOfPointsY)
        {
            Point[] array = new Point[iNumberOfPointsInXDirection * iNumberOfPointsInYDirection];

            for (int i = 0; i < iNumberOfPointsInYDirection; i++) // Rows
            {
                for (int j = 0; j < iNumberOfPointsInXDirection; j++) // Columns
                {
                    array[i * iNumberOfPointsInXDirection + j].X = refPoint.X + j * fDistanceOfPointsX; // Fill items in row [i], column [j]
                    array[i * iNumberOfPointsInXDirection + j].Y = refPoint.Y + i * fDistanceOfPointsY; // Fill items in row [i], column [j]
                }
            }

            return array;
        }

        public void TranslateSequence(float fPoint_x, float fPoint_y, CConnectorSequence sequence)
        {
            Point[] seqPoints = sequence.HolesCentersPoints;
            Geom2D.TransformPositions_CCW_rad(fPoint_x, fPoint_y, 0, ref seqPoints);
            sequence.HolesCentersPoints = seqPoints; // je to potrebne takto nastavovat lebo nie je mozne volat [ref sequence.HolesCentersPoints]
        }

        public virtual void FillArrayOfHolesCentersInWholeArrangement()
        {

        }
    }
}
