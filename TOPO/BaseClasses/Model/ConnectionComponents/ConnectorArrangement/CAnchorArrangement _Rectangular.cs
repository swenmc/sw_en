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

        public override void SetEdgeDistances(CConCom_Plate_B_basic plate, CFoundation pad)
        {
            float fpadX = pad.m_fDim1; // X
            float fpadY = pad.m_fDim2; // Y

            for (int i = 0; i < Anchors.Length; i++) // For each anchor
            {
                float fAnchorPositionOnPlate_x = (float)holesCentersPointsfor3D[i].X;
                float fAnchorPositionOnPlate_y = (float)holesCentersPointsfor3D[i].Y;

                // Anchor to plate edge distances
                Anchors[i].x_pe_minus = fAnchorPositionOnPlate_x;
                Anchors[i].x_pe_plus = plate.Fb_X - fAnchorPositionOnPlate_x;
                Anchors[i].y_pe_minus = fAnchorPositionOnPlate_y;
                Anchors[i].y_pe_plus = plate.Fh_Y - fAnchorPositionOnPlate_y;

                Anchors[i].x_pe_min = Math.Min(Anchors[i].x_pe_minus, Anchors[i].x_pe_plus);
                Anchors[i].y_pe_min = Math.Min(Anchors[i].y_pe_minus, Anchors[i].y_pe_plus);
                Anchors[i].x_pe_max = Math.Max(Anchors[i].x_pe_minus, Anchors[i].x_pe_plus);
                Anchors[i].y_pe_max = Math.Max(Anchors[i].y_pe_minus, Anchors[i].y_pe_plus);

                // Anchor to foundation edge distances
                Anchors[i].x_fe_minus = plate.x_plateEdge_to_pad + Anchors[i].x_pe_minus;
                Anchors[i].x_fe_plus = fpadX - Anchors[i].x_fe_minus;
                Anchors[i].y_fe_minus = plate.y_plateEdge_to_pad + Anchors[i].y_pe_minus;
                Anchors[i].y_fe_plus = fpadY - Anchors[i].y_fe_minus;

                Anchors[i].x_fe_min = Math.Min(Anchors[i].x_fe_minus, Anchors[i].x_fe_plus);
                Anchors[i].y_fe_min = Math.Min(Anchors[i].y_fe_minus, Anchors[i].y_fe_plus);
                Anchors[i].x_fe_max = Math.Max(Anchors[i].x_fe_minus, Anchors[i].x_fe_plus);
                Anchors[i].y_fe_max = Math.Max(Anchors[i].y_fe_minus, Anchors[i].y_fe_plus);
            }
        }
    }
}
