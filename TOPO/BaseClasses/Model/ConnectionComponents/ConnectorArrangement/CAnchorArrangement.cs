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
    }
}
