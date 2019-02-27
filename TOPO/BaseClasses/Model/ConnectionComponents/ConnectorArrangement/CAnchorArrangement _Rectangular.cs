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
    public abstract class CAnchorArrangement_Rectangular : CAnchorArrangement
    {
        private int m_iNumberOfAnchorsInYDirection;

        public int NumberOfAnchorsInYDirection // Cross-section geometrical y-axis (horizontal)
        {
            get
            {
                return m_iNumberOfAnchorsInYDirection;
            }

            set
            {
                m_iNumberOfAnchorsInYDirection = value;
            }
        }

        private int m_iNumberOfAnchorsInZDirection;

        public int NumberOfAnchorsInZDirection // Cross-section geometrical z-axis (vertical)
        {
            get
            {
                return m_iNumberOfAnchorsInZDirection;
            }

            set
            {
                m_iNumberOfAnchorsInZDirection = value;
            }
        }

        public CAnchorArrangement_Rectangular()
        { }

        public CAnchorArrangement_Rectangular(int iNumberInYDirection_temp, int iNumberInZDirection_temp, CAnchor referenceAnchor_temp) : base(iNumberInYDirection_temp * iNumberInZDirection_temp, referenceAnchor_temp)
        {
            NumberOfAnchorsInYDirection = iNumberInYDirection_temp;
            NumberOfAnchorsInZDirection = iNumberInZDirection_temp;
            referenceAnchor = referenceAnchor_temp;
            holesCentersPointsfor3D = new Point[IHolesNumber];

            IHolesNumber = iNumberInYDirection_temp * iNumberInZDirection_temp;

            RadiusAngle = 360; // Circle total angle to generate holes
        }
    }
}
