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
    public abstract class CAnchorArrangement : CConnectorArrangement
    {
        private List<CAnchorSequenceGroup> m_listOfSequenceGroups;
        public List<CAnchorSequenceGroup> ListOfSequenceGroups
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

        private CAnchor m_referenceAnchor;

        public CAnchor referenceAnchor
        {
            get
            {
                return m_referenceAnchor;
            }

            set
            {
                m_referenceAnchor = value;
            }
        }

        private CAnchor[] m_arrPlateAnchors;

        public CAnchor[] Anchors
        {
            get
            {
                return m_arrPlateAnchors;
            }

            set
            {
                m_arrPlateAnchors = value;
            }
        }

        public Point3D[] arrConnectorControlPoints3D; // Array of control points for inserting connectors (bolts, screws, anchors, ...)
        public Point[] holesCentersPointsfor3D;

        private float m_fHoleRadius; // Hole radius

        public float HoleRadius
        {
            get
            {
                return m_fHoleRadius;
            }

            set
            {
                m_fHoleRadius = value;
            }
        }

        private int m_iRadiusAngle;

        public int RadiusAngle
        {
            get
            {
                return m_iRadiusAngle;
            }

            set
            {
                m_iRadiusAngle = value;
            }
        }

        public CAnchorArrangement()
        { }

        public CAnchorArrangement(int iAnchorNumber_temp, CAnchor referenceAnchor_temp) : base(iAnchorNumber_temp)
        {
            IHolesNumber = iAnchorNumber_temp;
            referenceAnchor = referenceAnchor_temp;
            holesCentersPointsfor3D = new Point[IHolesNumber];

            RadiusAngle = 360; // Circle total angle to generate holes
        }

        // BASE PLATE
        public virtual void Calc_BasePlateData(
            float fbX,
            float flZ,
            float fhY,
            float ft)
        { }

        public virtual void Calc_HolesCentersCoord2D(
            float fbX,
            float flZ,
            float fhY)
        { }

        public virtual void UpdateArrangmentData() { }

        public virtual void RecalculateTotalNumberOfAnchors()
        {
            // TODO - Malo by sa refaktorovat pre skrutky a kotvy a preniest priamo do connectors
            // Celkovy pocet kotiev, pocet moze byt v kazdej sekvencii rozny
            IHolesNumber = 0;

            foreach (CAnchorSequenceGroup group in ListOfSequenceGroups)
            {
                foreach (CAnchorSequence seq in group.ListSequence)
                    IHolesNumber += seq.INumberOfConnectors;
            }

            // Validation
            if (IHolesNumber < 0)
                IHolesNumber = 0;
        }

        public virtual void SetEdgeDistances(CConCom_Plate_B_basic plate, CFoundation pad, float fx_plateEdge_to_pad, float fy_plateEdge_to_pad)
        { }

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
