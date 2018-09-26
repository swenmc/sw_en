using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public class CScrewArrangement : CConnectorArrangement
    {
        private List<CScrewSequenceGroup> m_listOfSequenceGroups;
        public List<CScrewSequenceGroup> ListOfSequenceGroups
        {
            get
            {
                return m_listOfSequenceGroups;
            }

            set
            {
                m_listOfSequenceGroups = value;
            }
        }

        private CScrew m_referenceScrew;

        public CScrew referenceScrew
        {
            get
            {
                return m_referenceScrew;
            }

            set
            {
                m_referenceScrew = value;
            }
        }

        private CScrew[] m_arrPlateScrews;

        public CScrew[] Screws
        {
            get
            {
                return m_arrPlateScrews;
            }

            set
            {
                m_arrPlateScrews = value;
            }
        }

        public Point3D[] arrConnectorControlPoints3D; // Array of control points for inserting connectors (bolts, screws, anchors, ...)

        public CScrewArrangement()
        { }

        public CScrewArrangement(int iScrewsNumber_temp, CScrew referenceScrew_temp) : base(iScrewsNumber_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
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

        //public List<Point> GetHolesCentersPoints2D()
        //{
        //    List<Point> points = null;
        //    if (this.HolesCentersPoints2D != null)
        //    {
        //        points = new List<Point>(this.HolesCentersPoints2D.Length);
        //        for (int i = 0; i < this.HolesCentersPoints2D.Length; i++)
        //        {
        //            points.Add(new Point(this.HolesCentersPoints2D[i].X, this.HolesCentersPoints2D[i].Y));
        //        }
        //    }
        //    return points;
        //}

        public void FillArrayOfHolesCentersInWholeArrangement()
        {
            // Fill array of holes centers - whole arrangement
            int iPointIndex = 0;
            for (int i = 0; i < this.ListOfSequenceGroups.Count; i++) // Add each group
            {
                for (int j = 0; j < this.ListOfSequenceGroups[i].ListScrewSequence.Count; j++) // Add each sequence in group
                {
                    for (int k = 0; k < this.ListOfSequenceGroups[i].ListScrewSequence[j].HolesCentersPoints.Length; k++) // Add each point in the sequence
                    {
                        HolesCentersPoints2D[iPointIndex + k].X = this.ListOfSequenceGroups[i].ListScrewSequence[j].HolesCentersPoints[k].X;
                        HolesCentersPoints2D[iPointIndex + k].Y = this.ListOfSequenceGroups[i].ListScrewSequence[j].HolesCentersPoints[k].Y;
                    }

                    iPointIndex += this.ListOfSequenceGroups[i].ListScrewSequence[j].HolesCentersPoints.Length;
                }
            }
        }
    }
}
