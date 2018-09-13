using _3DTools;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_JA : CPlate
    {
        private float m_fbX;

        public float Fb_X
        {
            get
            {
                return m_fbX;
            }

            set
            {
                m_fbX = value;
            }
        }

        private float m_fhY1;

        public float Fh_Y1
        {
            get
            {
                return m_fhY1;
            }

            set
            {
                m_fhY1 = value;
            }
        }

        private float m_fhY2;

        public float Fh_Y2
        {
            get
            {
                return m_fhY2;
            }

            set
            {
                m_fhY2 = value;
            }
        }

        float m_fSlope_rad;
        public float[] HolesCenterRadii;

        public new CScrewArrangementCircleApexOrKnee screwArrangement;

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
            CScrewArrangementCircleApexOrKnee screwArrangement_temp,
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
            m_fhY1 = fh_1_temp;
            m_fhY2 = fh_2_temp;
            Ft = ft_platethickness;
            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / (0.5 * fb_temp));
            screwArrangement = screwArrangement_temp;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[screwArrangement.IHolesNumber + (screwArrangement.BUseAdditionalCornerScrews ? screwArrangement.IAdditionalConnectorNumber : 0), 2];
            HolesCenterRadii = new float[HolesCentersPoints2D.Length / 2];
            arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber + (screwArrangement.BUseAdditionalCornerScrews ? screwArrangement.IAdditionalConnectorNumber : 0)];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            screwArrangement.Calc_HolesCentersCoord2D(m_fbX,
                0,
                m_fhY1,
                m_fSlope_rad,
                screwArrangement.BUseAdditionalCornerScrews,
                screwArrangement.IAdditionalConnectorNumber,
                screwArrangement.FCrscWebStraightDepth,
                screwArrangement.FStiffenerSize,
                ref HolesCentersPoints2D);

            screwArrangement.Calc_HolesControlPointsCoord3D(0, Ft);

            // Fill list of indices for drawing of surface
            loadIndices();

            screwArrangement.GenerateConnectors();

            fWidth_bx = m_fbX;
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fWeight = GetWeightIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY1);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            fA_v_zv = Get_A_rect(Ft, m_fhY1);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            fI_yu = Get_I_yu_rect( Ft, m_fhY1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY1); // Elastic section modulus
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
            CScrewArrangementCircleApexOrKnee screwArrangement_temp,
            bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 5;
            ITotNoPointsin3D = 10;

            m_pControlPoint = controlpoint;
            m_fbX = fb_temp;
            m_fhY1 = fh_1_temp;
            m_fhY2 = fh_2_temp;
            Ft = ft_platethickness;
            screwArrangement = screwArrangement_temp;

            m_fSlope_rad = fSLope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[screwArrangement.IHolesNumber + (screwArrangement.BUseAdditionalCornerScrews ? screwArrangement.IAdditionalConnectorNumber : 0), 2];
            HolesCenterRadii = new float[HolesCentersPoints2D.Length / 2];
            arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber + (screwArrangement.BUseAdditionalCornerScrews ? screwArrangement.IAdditionalConnectorNumber : 0)];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            screwArrangement.Calc_HolesCentersCoord2D(m_fbX,
                0,
                m_fhY1,
                m_fSlope_rad,
                screwArrangement.BUseAdditionalCornerScrews,
                screwArrangement.IAdditionalConnectorNumber,
                screwArrangement.FCrscWebStraightDepth,
                screwArrangement.FStiffenerSize,
                ref HolesCentersPoints2D);

            screwArrangement.Calc_HolesControlPointsCoord3D(0, Ft);

            // Fill list of indices for drawing of surface
            loadIndices();

            screwArrangement.GenerateConnectors();

            fWidth_bx = m_fbX;
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fWeight = GetWeightIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY1);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            fA_v_zv = Get_A_rect(Ft, m_fhY1);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            fI_yu = Get_I_yu_rect(Ft, m_fhY1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY1); // Elastic section modulus
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_fbX;
            PointsOut2D[1, 1] = PointsOut2D[0, 1];

            PointsOut2D[2, 0] = PointsOut2D[1, 0];
            PointsOut2D[2, 1] = m_fhY1;

            PointsOut2D[3, 0] = 0.5f * m_fbX;
            PointsOut2D[3, 1] = m_fhY2;

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
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Z = i * Ft;
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
