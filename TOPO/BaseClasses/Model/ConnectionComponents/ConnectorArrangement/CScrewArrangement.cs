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

        // TO Ondrej - mam tu pole m_arrPlateScrews ale aj zoznam List<CScrewSequenceGroup> m_listOfSequenceGroups
        // To je v istom zmysle duplicita, v tom zozname su jednotlive skupiny skrutiek, v nich sekvencie a v tych sekvenciach by mali byt skrutky
        // V tom poli su objeky skrutiek zo vsetkych skupin a vsetkych sekvencii

        // TODO - chcelo by to refaktorovat hierarchiu tych objektov a dat tomu hlavu a patu :)

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
            float fOffset_x,
            float fbX,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad)
        { }
        public virtual void Calc_HolesCentersCoord2DApexPlate(
            float fOffset_x,
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
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i].X - fx; // Odpocitat hodnotu flZ pridanu pre 2D zobrazenie (knee plate alebo apex JC)
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i].Y - fy; // Odpocitat hodnotu flZ pridanu pre 2D zobrazenie (apex plate)
                arrConnectorControlPoints3D[i].Z = -0.8*ft; // TODO Position depends on screw length // Zaciatok skrutky vycnieva 80% z hrubky plechu (dopracovat tak ze budeme poznat dlzku skrutky a pozicia zaciatku bude urcena podla hrubky plechu a dlzky skrutky
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

        // FACE PLATE "O"- KNEE JOINT
        public virtual void Calc_FacePlateData(
            float fbX1,
            float fbX2,
            float fhY1,
            float ft)
        { }

        public virtual void Calc_HolesCentersCoord2DFacePlate(
            float fbX1,
            float fbX2,
            float fhY1)
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

        public override void FillArrayOfHolesCentersInWholeArrangement()
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
    }
}
