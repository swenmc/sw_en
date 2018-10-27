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
    [Serializable]
    public abstract class CScrewArrangement : CConnectorArrangement
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
            for (int i = 0; i < ListOfSequenceGroups.Count; i++) // Add each group
            {
                for (int j = 0; j < ListOfSequenceGroups[i].ListSequence.Count; j++) // Add each sequence in group
                {
                    if (ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints == null) continue;

                    for (int k = 0; k < ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints.Length; k++) // Add each point in the sequence
                    {
                        HolesCentersPoints2D[iPointIndex + k].X = ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints[k].X;
                        HolesCentersPoints2D[iPointIndex + k].Y = ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints[k].Y;
                    }

                    iPointIndex += ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints.Length;
                }
            }
        }

        public virtual void Calc_KneePlateData(
            float fbX1,
            float fbX2,
            float flZ,
            float fhY1,
            float ft,
            float fSlope_rad)
        { }

        public virtual void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        { }

        public virtual void Calc_ApexPlateData(
            float fbX,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad)
        { }
        public virtual void Calc_HolesCentersCoord2DApexPlate(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        { }

        public virtual void UpdateArrangmentData() { }

        public virtual void Calc_HolesControlPointsCoord3D_FlatPlate(float fx, float fy, float ft)
        {
            for (int i = 0; i < IHolesNumber; i++)
            {
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i].X - fx; // Odpocitat hodnotu flZ pridanu pre 2D zobrazenie (knee plate)
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i].Y - fy; // Odpocitat hodnotu flZ pridanu pre 2D zobrazenie (apex plate)
                arrConnectorControlPoints3D[i].Z = -ft; // TODO Position depends on screw length;
            }
        }

        public virtual void GenerateConnectors_FlatPlate()
        {
            Screws = new CScrew[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                Screws[i] = new CScrew(referenceScrew.Name, controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Mass, 0, -90, 0, true);
            }
        }

        // BASE PLATE
        public virtual void Calc_BasePlateData(
            float fbX,
            float flZ,
            float fhY,
            float ft)
        { }

        public virtual void Calc_HolesCentersCoord2DBasePlate(
            float fbX,
            float flZ,
            float fhY)
        { }

        public virtual void RecalculateTotalNumberOfScrews()
        {
            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            IHolesNumber = 0;

            foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
            {
                foreach (CScrewSequence seq in group.ListSequence)
                    IHolesNumber += seq.INumberOfConnectors;
            }

            // Validation
            if (IHolesNumber < 0)
                IHolesNumber = 0;
        }
    }
}
