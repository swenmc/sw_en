using _3DTools;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_JA : CPlate
    {
        float m_fbX;
        float m_fhY_1;
        float m_fhY_2;
        float m_ft;
        float m_fSlope_rad;

        float m_fCrscWebStraightDepth;
        float m_fStiffenerSize; // Middle cross-section stiffener dimension (without screws)
        bool m_bUseAdditionalCornerScrews;
        int m_iAdditionalConnectorNumber;

        public float[] HolesCenterRadii;
        public int INumberOfCircleJoints = 2;

        private float fConnectorLength;

        public float FConnectorLength
        {
            get { return fConnectorLength; }
            set { fConnectorLength = value; }
        }

        public CConCom_Plate_JA()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;
            BIsDisplayed = true;
        }

        public CConCom_Plate_JA(string sName_temp,
            GraphObj.CPoint controlpoint,
            float fb_temp,
            float fh_1_temp,
            float fh_2_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            int iHolesNumber,
            float fHoleDiameter_temp,
            float fConnectorLength_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorNumber_temp,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 5;
            ITotNoPointsin3D = 10;

            m_pControlPoint = controlpoint;
            m_fbX = fb_temp;
            m_fhY_1 = fh_1_temp;
            m_fhY_2 = fh_2_temp;
            m_ft = ft_platethickness;
            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / (0.5 * fb_temp));
            IHolesNumber = iHolesNumber;
            FHoleDiameter = fHoleDiameter_temp;
            FConnectorLength = fConnectorLength_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_bUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            m_iAdditionalConnectorNumber = iAdditionalConnectorNumber_temp;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0), 2];
            HolesCenterRadii = new float[HolesCentersPoints2D.Length / 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0)];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D_ApexPlate(m_fbX,
                0,
                m_fhY_1,
                m_fSlope_rad,
                m_bUseAdditionalCornerScrews,
                INumberOfCircleJoints,
                m_iAdditionalConnectorNumber,
                m_fCrscWebStraightDepth,
                m_fStiffenerSize);

            Calc_HolesControlPointsCoord3D_ApexOrKneePlate(0, m_ft);

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors_ApexOrKneePlate(14, FConnectorLength, 0.015f);

            fWidth_bx = m_fbX;
            fHeight_hy = Math.Max(m_fhY_1, m_fhY_2);
            fThickness_tz = m_ft;
            fArea = PolygonArea();
            fWeight = GetPlateWeight();
        }

        public CConCom_Plate_JA(GraphObj.CPoint controlpoint,
            float fb_temp,
            float fh_1_temp,
            float fh_2_temp,
            float ft_platethickness,
            float fSLope_rad_temp,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            int iHolesNumber,
            float fHoleDiameter_temp,
            float fConnectorLength_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorNumber_temp,
            bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 5;
            ITotNoPointsin3D = 10;

            m_pControlPoint = controlpoint;
            m_fbX = fb_temp;
            m_fhY_1 = fh_1_temp;
            m_fhY_2 = fh_2_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            FHoleDiameter = fHoleDiameter_temp;
            FConnectorLength = fConnectorLength_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_bUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            m_iAdditionalConnectorNumber = iAdditionalConnectorNumber_temp;
            m_fSlope_rad = fSLope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0), 2];
            HolesCenterRadii = new float[HolesCentersPoints2D.Length / 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0)];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D_ApexPlate(m_fbX,
                0,
                m_fhY_1,
                m_fSlope_rad,
                m_bUseAdditionalCornerScrews,
                INumberOfCircleJoints,
                m_iAdditionalConnectorNumber,
                m_fCrscWebStraightDepth,
                m_fStiffenerSize);

            Calc_HolesControlPointsCoord3D_ApexOrKneePlate(0, m_ft);

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors_ApexOrKneePlate(14, FConnectorLength, 0.015f);

            fWidth_bx = m_fbX;
            fHeight_hy = Math.Max(m_fhY_1, m_fhY_2);
            fThickness_tz = m_ft;
            fArea = PolygonArea();
            fWeight = GetPlateWeight();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_fbX;
            PointsOut2D[1, 1] = PointsOut2D[0, 1];

            PointsOut2D[2, 0] = PointsOut2D[1, 0];
            PointsOut2D[2, 1] = m_fhY_1;

            PointsOut2D[3, 0] = 0.5f * m_fbX;
            PointsOut2D[3, 1] = m_fhY_2;

            PointsOut2D[4, 0] = PointsOut2D[0, 0];
            PointsOut2D[4, 1] = PointsOut2D[2, 1];
        }

        void Calc_Coord3D()
        {
            for (int i = 0; i < 2; i++) // 2 cycles - front and back surface
            {
                // One Side
                for (int j = 0; j < ITotNoPointsin2D; j++)
                {
                    arrPoints3D[(i * ITotNoPointsin2D) + j].X = PointsOut2D[j, 0];
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Y = PointsOut2D[j, 1];
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Z = i * m_ft;
                }
            }
        }

        protected override void loadIndices()
        {
            int secNum = 5;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddPenthagonIndices_CCW_12345(TriangleIndices, 0, 1, 2, 3, 4);

            // Back Side
            AddPenthagonIndices_CW_12345(TriangleIndices, 5, 6, 7, 8, 9);

            // Shell Surface
            DrawCaraLaterals_CCW(secNum, TriangleIndices);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // Front Side
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                if (i < (PointsOut2D.Length / 2) - 1)
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[i+1];
                }
                else // Last line
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // BackSide
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                if (i < (PointsOut2D.Length / 2) - 1)
                {
                    pi = arrPoints3D[ITotNoPointsin2D + i];
                    pj = arrPoints3D[ITotNoPointsin2D + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[ITotNoPointsin2D + i];
                    pj = arrPoints3D[ITotNoPointsin2D + 0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Lateral
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                pi = arrPoints3D[i];
                pj = arrPoints3D[ITotNoPointsin2D + i];

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            return wireFrame;
        }
    }
}
