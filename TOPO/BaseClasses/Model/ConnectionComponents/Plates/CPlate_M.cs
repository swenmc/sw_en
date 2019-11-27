using _3DTools;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_M : CPlate
    {
        private float m_fbX1;

        public float Fb_X1
        {
            get
            {
                return m_fbX1;
            }

            set
            {
                m_fbX1 = value;
            }
        }

        private float m_fbX2;

        public float Fb_X2
        {
            get
            {
                return m_fbX2;
            }

            set
            {
                m_fbX2 = value;
            }
        }

        private float m_fbX3;

        public float Fb_X3
        {
            get
            {
                return m_fbX3;
            }

            set
            {
                m_fbX3 = value;
            }
        }

        private float m_fhY;

        public float Fh_Y
        {
            get
            {
                return m_fhY;
            }

            set
            {
                m_fhY = value;
            }
        }

        public CConCom_Plate_M()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_M;
            BIsDisplayed = true;
        }

        public CConCom_Plate_M(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fbX3_temp,
            float fhY_temp,
            float ft_platethickness,
            int iNumberOfHoles,
            float fbX2_temp, // Wind post width
            float fRoofPitch_rad,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_N;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;
        }
        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = 0;
            PointsOut2D[1].Y = m_fbX1;

            PointsOut2D[2].X = PointsOut2D[1].X + m_fbX2;
            PointsOut2D[2].Y = PointsOut2D[1].Y;

            PointsOut2D[3].X = PointsOut2D[2].X + m_fbX3;
            PointsOut2D[3].Y = PointsOut2D[0].Y;

            PointsOut2D[4].X = PointsOut2D[3].X;
            PointsOut2D[4].Y = PointsOut2D[0].Y + m_fhY;

            PointsOut2D[5].X = PointsOut2D[2].X;
            PointsOut2D[5].Y = PointsOut2D[4].Y;

            PointsOut2D[6].X = PointsOut2D[1].X;
            PointsOut2D[6].Y = PointsOut2D[4].Y;

            PointsOut2D[7].X = PointsOut2D[0].X;
            PointsOut2D[7].Y = PointsOut2D[4].Y;
        }
    }
}
