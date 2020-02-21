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
            float[] farr_DistancesOfPointsX = new float[1] { fDistanceOfPointsX };
            float[] farr_DistancesOfPointsY = new float[1] { fDistanceOfPointsY };

            return GetRegularArrayOfPointsInCartesianCoordinates(refPoint, iNumberOfPointsInXDirection, iNumberOfPointsInYDirection, farr_DistancesOfPointsX, farr_DistancesOfPointsY);
        }

        public Point[] GetRegularArrayOfPointsInCartesianCoordinates(Point refPoint, int iNumberOfPointsInXDirection, int iNumberOfPointsInYDirection, float[] farr_DistancesOfPointsX, float[] farr_DistancesOfPointsY)
        {
            Point[] array = new Point[iNumberOfPointsInXDirection * iNumberOfPointsInYDirection];

            double lastY = refPoint.Y;

            for (int i = 0; i < iNumberOfPointsInYDirection; i++) // Rows
            {
                double lastX = refPoint.X;

                for (int j = 0; j < iNumberOfPointsInXDirection; j++) // Columns
                {
                    // Regular distances between rows / columns
                    if (farr_DistancesOfPointsX.Length == 1 && farr_DistancesOfPointsY.Length == 1)
                    {
                        array[i * iNumberOfPointsInXDirection + j].X = refPoint.X + j * farr_DistancesOfPointsX[0]; // Fill items in row [i], column [j]
                        array[i * iNumberOfPointsInXDirection + j].Y = refPoint.Y + i * farr_DistancesOfPointsY[0]; // Fill items in row [i], column [j]
                    }
                    else if (farr_DistancesOfPointsX.Length == 1 && farr_DistancesOfPointsY.Length != 1)
                    {
                        array[i * iNumberOfPointsInXDirection + j].X = refPoint.X + j * farr_DistancesOfPointsX[0]; // Fill items in row [i], column [j]
                        array[i * iNumberOfPointsInXDirection + j].Y = lastY + (i > 0 ? farr_DistancesOfPointsY[i - 1] : 0); // Fill items in row [i], column [j]

                        lastX += 0; // Ak je v poli pre vzdialenosti x len jedna hodnota index nenavysujeme
                    }
                    else if (farr_DistancesOfPointsX.Length != 1 && farr_DistancesOfPointsY.Length == 1)
                    {
                        array[i * iNumberOfPointsInXDirection + j].X = lastX + (j > 0 ? farr_DistancesOfPointsX[j - 1] : 0); // Fill items in row [i], column [j]
                        array[i * iNumberOfPointsInXDirection + j].Y = refPoint.Y + i * farr_DistancesOfPointsY[0]; // Fill items in row [i], column [j]

                        lastX += j > 0 ? farr_DistancesOfPointsX[j - 1] : 0;
                    }
                    else
                    {
                        // Pre prvy index v rade alebo stplci pripocitavame nulu pretoze uvazujeme priamo hodnoty refPoint.X a refPoint.Y
                        array[i * iNumberOfPointsInXDirection + j].X = lastX + (j > 0 ? farr_DistancesOfPointsX[j - 1] : 0); // Fill items in row [i], column [j]
                        array[i * iNumberOfPointsInXDirection + j].Y = lastY + (i > 0 ? farr_DistancesOfPointsY[i - 1] : 0); // Fill items in row [i], column [j]

                        lastX += j > 0 ? farr_DistancesOfPointsX[j - 1] : 0;
                    }
                }

                if (i < farr_DistancesOfPointsY.Length)
                    lastY += i > 0 ? farr_DistancesOfPointsY[i - 1] : 0; // Pripocitat len ak je index v poli validny, pre rovnake vzdialenosti medzi bodmi by to nefungovalo
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

        public int GetNumberOfEquallySpacedConnectors(float fDimension, float fSpacing)
        {
            return (int)(fDimension / fSpacing) - 1; // Vypocitame pocet medzier a jednu odpocitame jednu medzeru, takze dostaneme pocet skrutiek
        }

        public float GetEdgeDistanceOfEquallySpacedConnectors(float fDimension, float fSpacing, int iNumberOfConnectors)
        {
            return (fDimension - (iNumberOfConnectors - 1) * fSpacing) / 2f;
        }
    }
}
