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

        private float m_fRoofPitch_rad;

        public float RoofPitch_rad
        {
            get
            {
                return m_fRoofPitch_rad;
            }

            set
            {
                m_fRoofPitch_rad = value;
            }
        }

        private float m_fGamma1_rad; // Uhol medzi vonkajsou hranou plechu a hranou prierezu

        public float Gamma1_rad
        {
            get
            {
                return m_fGamma1_rad;
            }

            set
            {
                m_fGamma1_rad = value;
            }
        }

        private float m_fe_min_x; // Minimalna vzdialenost skrutiek - smer x

        public float e_min_x
        {
            get
            {
                return m_fe_min_x;
            }

            set
            {
                m_fe_min_x = value;
            }
        }

        private float m_fe_min_y; // Minimalna vzdialenost skrutiek - smer y

        public float e_min_y
        {
            get
            {
                return m_fe_min_y;
            }

            set
            {
                m_fe_min_y = value;
            }
        }

        public CConCom_Plate_M()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_M;
        }

        public CConCom_Plate_M(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fbX3_temp,
            float fhY_temp,
            float ft_platethickness,
            float fbX2_temp, // Wind post width
            float fRoofPitch_rad,
            float fGamma_1_rad,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_M screwArrangement_temp)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_M;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX3 = fbX3_temp;
            m_fhY = fhY_temp;
            Ft = ft_platethickness; //0.02f;
            m_fbX2 = fbX2_temp;
            m_fRoofPitch_rad = fRoofPitch_rad;
            m_fGamma1_rad = fGamma_1_rad;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement_temp != null)
            {
                arrConnectorControlPoints3D = new Point3D[screwArrangement_temp.IHolesNumber];
                screwArrangement_temp.Calc_HolesCentersCoord2D(Ft, Fb_X1, Fb_X2, Fb_X3, Fh_Y);
                Calc_HolesControlPointsCoord3D(screwArrangement_temp);
                GenerateConnectors(screwArrangement_temp);
            }

            Width_bx = m_fbX1 + m_fbX2 + m_fbX3;
            Height_hy = m_fhY;
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = m_fbX1 + m_fbX2 + m_fbX3;
            Height_hy_Stretched = m_fhY;

            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY);
            int iNumberOfScrewsInSection = 2; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;

            if (screwArrangement_temp != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fhY);

            fA_vn_zv = fA_v_zv;

            if (screwArrangement_temp != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fhY);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY); // Elastic section modulus

            ScrewArrangement = screwArrangement_temp;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX1;
            PointsOut2D[1].Y = 0;

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

        public override void Calc_Coord3D()
        {
            // Auxiliary values;
            float fGamma2 = 0.5f * MathF.fPI - m_fGamma1_rad;

            //Priemetry
            // x priblizne ???
            float fx1 = m_fbX1 * (float)Math.Sin(m_fGamma1_rad);
            float fx3 = m_fbX3 * (float)Math.Sin(m_fGamma1_rad);

            float fy1 = fx1 * (float)Math.Tan(m_fRoofPitch_rad);
            float fy3 = fx3 * (float)Math.Tan(m_fRoofPitch_rad);

            float fz1 = m_fbX1 * (float)Math.Cos(m_fGamma1_rad);
            float fz3 = m_fbX3 * (float)Math.Cos(m_fGamma1_rad);

            float fx11 = m_fhY * (float)Math.Cos(m_fGamma1_rad);
            float fx31 = m_fhY * (float)Math.Cos(m_fGamma1_rad);

            float fz11 = m_fhY * (float)Math.Sin(m_fGamma1_rad);
            float fz31 = m_fhY * (float)Math.Sin(m_fGamma1_rad);

            float fx12 = fx1 - fx11;
            float fx32 = fx3 - fx31;

            float fy11 = fx11 * (float)Math.Tan(m_fRoofPitch_rad);
            float fy31 = fx31 * (float)Math.Tan(m_fRoofPitch_rad);

            float fy12 = fy1 - fy11;
            float fy32 = fy3 - fy31;

            float fy2_right = 0, fy2_left = 0;

            if (m_fRoofPitch_rad > 0)
                fy2_right = m_fbX2 * (float)Math.Tan(m_fRoofPitch_rad);
            else if (m_fRoofPitch_rad < 0)
                fy2_left = m_fbX2 * (float)Math.Tan(m_fRoofPitch_rad);
            else
                fy2_right = fy2_left = 0;

            float ft_x = Ft * (float)Math.Sin(m_fRoofPitch_rad);
            float ft_y = Ft * (float)Math.Cos(m_fRoofPitch_rad);

            // First layer
            arrPoints3D[0].X = - 0.5 * m_fbX2;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = + 0.5 * m_fbX2;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = + 0.5 * m_fbX2 + fx32 - ft_x;
            arrPoints3D[2].Y = m_fhY + fy2_right + fy32 + ft_y;
            arrPoints3D[2].Z = - fz3 - fz31;

            arrPoints3D[3].X = arrPoints3D[2].X + fx31 - ft_x;
            arrPoints3D[3].Y = m_fhY + fy2_right + fy3 + ft_y;
            arrPoints3D[3].Z = -fz3;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[1].Z;

            arrPoints3D[5].X = arrPoints3D[0].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = arrPoints3D[0].X - fx1 - ft_x;
            arrPoints3D[6].Y = m_fhY - fy2_left - fy1 + ft_y;
            arrPoints3D[6].Z = - fz1;

            arrPoints3D[7].X = arrPoints3D[0].X - fx12 - ft_x;
            arrPoints3D[7].Y = m_fhY - fy2_left - fy12 + ft_y;
            arrPoints3D[7].Z = -fz1 - fz11;

            // Second layer
            arrPoints3D[8].X = arrPoints3D[7].X + ft_x;
            arrPoints3D[8].Y = arrPoints3D[7].Y - ft_y;
            arrPoints3D[8].Z = -fz1 - fz11;

            arrPoints3D[9].X = arrPoints3D[0].X - Ft * Math.Tan(fGamma2);
            arrPoints3D[9].Y = 0;
            arrPoints3D[9].Z = Ft;

            arrPoints3D[10].X = arrPoints3D[1].X + Ft * Math.Tan(fGamma2);
            arrPoints3D[10].Y = 0;
            arrPoints3D[10].Z = Ft;

            arrPoints3D[11].X = arrPoints3D[2].X + ft_x;
            arrPoints3D[11].Y = arrPoints3D[2].Y - ft_y;
            arrPoints3D[11].Z = arrPoints3D[2].Z;

            arrPoints3D[12].X = arrPoints3D[3].X + ft_x;
            arrPoints3D[12].Y = arrPoints3D[3].Y - ft_y;
            arrPoints3D[12].Z = arrPoints3D[3].Z;

            arrPoints3D[13].X = arrPoints3D[10].X;
            arrPoints3D[13].Y = m_fhY;
            arrPoints3D[13].Z = arrPoints3D[10].Z;

            arrPoints3D[14].X = arrPoints3D[9].X;
            arrPoints3D[14].Y = m_fhY;
            arrPoints3D[14].Z = arrPoints3D[9].Z;

            arrPoints3D[15].X = arrPoints3D[6].X + ft_x;
            arrPoints3D[15].Y = arrPoints3D[6].Y - ft_y;
            arrPoints3D[15].Z = arrPoints3D[6].Z;
        }

        void Calc_HolesControlPointsCoord3D(CScrewArrangement screwArrangement)
        {
            // TODO - nie je to velmi presne spocitane, ale nechcem s tym stravit vela casu

            // 3 x 2 screws = 6 screws in the plate
            float fScrewOffset = screwArrangement.referenceScrew.T_ht_headTotalThickness;

            m_fe_min_x = 0.015f;  // x-direction
            m_fe_min_y = 0.015f;  // y-direction in 2D

            // Middle
            arrConnectorControlPoints3D[0].X = 0;
            arrConnectorControlPoints3D[0].Y = e_min_y;
            arrConnectorControlPoints3D[0].Z =  Ft + fScrewOffset;

            arrConnectorControlPoints3D[1].X = 0;
            arrConnectorControlPoints3D[1].Y = m_fhY - e_min_y;
            arrConnectorControlPoints3D[1].Z = arrConnectorControlPoints3D[0].Z;

            // Auxiliary calculations
            // Left

            float fS3OffSetGCS_X, fS3OffSetGCS_Yb, fS3OffSetGCS_Yabove, fS3OffSetGCS_Z;
            GetScrewPositionCoordinates(m_fe_min_x, e_min_y, fScrewOffset, m_fGamma1_rad, out fS3OffSetGCS_X, out fS3OffSetGCS_Yb, out fS3OffSetGCS_Yabove, out fS3OffSetGCS_Z);

            float fS4OffSetGCS_X, fS4OffSetGCS_Yb, fS4OffSetGCS_Yabove, fS4OffSetGCS_Z;
            GetScrewPositionCoordinates(m_fe_min_x, m_fhY - e_min_y, fScrewOffset, m_fGamma1_rad, out fS4OffSetGCS_X, out fS4OffSetGCS_Yb, out fS4OffSetGCS_Yabove, out fS4OffSetGCS_Z);

            arrConnectorControlPoints3D[2].X = arrPoints3D[15].X + fS3OffSetGCS_X;
            arrConnectorControlPoints3D[2].Y = arrPoints3D[15].Y + fS3OffSetGCS_Yb + fS3OffSetGCS_Yabove;
            arrConnectorControlPoints3D[2].Z = arrPoints3D[15].Z + fS3OffSetGCS_Z;

            arrConnectorControlPoints3D[3].X = arrPoints3D[15].X + fS4OffSetGCS_X;
            arrConnectorControlPoints3D[3].Y = arrPoints3D[15].Y + fS4OffSetGCS_Yb + fS4OffSetGCS_Yabove;
            arrConnectorControlPoints3D[3].Z = arrPoints3D[15].Z + fS4OffSetGCS_Z;

            // Right
            float fS5OffSetGCS_X, fS5OffSetGCS_Yb, fS5OffSetGCS_Yabove, fS5OffSetGCS_Z;
            GetScrewPositionCoordinates(m_fe_min_x, m_fe_min_y, fScrewOffset, m_fGamma1_rad, out fS5OffSetGCS_X, out fS5OffSetGCS_Yb, out fS5OffSetGCS_Yabove, out fS5OffSetGCS_Z);

            float fS6OffSetGCS_X, fS6OffSetGCS_Yb, fS6OffSetGCS_Yabove, fS6OffSetGCS_Z;
            GetScrewPositionCoordinates(m_fe_min_x, m_fhY - m_fe_min_y, fScrewOffset, m_fGamma1_rad, out fS6OffSetGCS_X, out fS6OffSetGCS_Yb, out fS6OffSetGCS_Yabove, out fS6OffSetGCS_Z);

            arrConnectorControlPoints3D[4].X = arrPoints3D[12].X - fS5OffSetGCS_X;
            arrConnectorControlPoints3D[4].Y = arrPoints3D[12].Y - fS5OffSetGCS_Yb + fS5OffSetGCS_Yabove;
            arrConnectorControlPoints3D[4].Z = arrPoints3D[12].Z + fS5OffSetGCS_Z;

            arrConnectorControlPoints3D[5].X = arrPoints3D[12].X - fS6OffSetGCS_X;
            arrConnectorControlPoints3D[5].Y = arrPoints3D[12].Y - fS6OffSetGCS_Yb + fS6OffSetGCS_Yabove;
            arrConnectorControlPoints3D[5].Z = arrPoints3D[12].Z + fS6OffSetGCS_Z;
        }

        void GetScrewPositionCoordinates(
            float fx_edge,
            float fy_position,
            float fOffSetOfScrewAndPlate,
            float fGamma,
            out float fOffSetGCS_X,
            out float fOffSetGCS_Ybasic,
            out float fOffSetGCS_YabovePlate,
            out float fOffSetGCS_Z
            )
        {
            float fGamma_s11 = (float)Math.Atan(fy_position / fx_edge);
            float fGamma_s12 = 0.5f * MathF.fPI - fGamma_s11 - fGamma;
            float fc_s1 = MathF.Sqrt(MathF.Pow2(fx_edge) + MathF.Pow2(fy_position));

            float fx_s1 = fc_s1 * (float)Math.Cos(fGamma_s12);
            float fy_s1 = fc_s1 * (float)Math.Sin(fGamma_s12);

            float fz_s1 = fx_s1 * (float)Math.Sin(m_fRoofPitch_rad);
            float fx_s1_p = fx_s1 * (float)Math.Cos(m_fRoofPitch_rad);

            float fz_s1_a = (fOffSetOfScrewAndPlate + Ft) * (float)Math.Cos(m_fRoofPitch_rad);
            float fx_s1_p_a = (fOffSetOfScrewAndPlate + Ft) * (float)Math.Sin(m_fRoofPitch_rad);

            fOffSetGCS_X = fx_s1_p - fx_s1_p_a;
            fOffSetGCS_Ybasic = fz_s1;
            fOffSetGCS_YabovePlate = fz_s1_a;
            fOffSetGCS_Z = fy_s1;
        }

        void GenerateConnectors(CScrewArrangement screwArrangement)
        {
            if (screwArrangement.IHolesNumber > 0)
            {
                screwArrangement.Screws = new CScrew[screwArrangement.IHolesNumber];

                for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                {
                    Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);

                    float fAngle_y_deg = 90; // Default 1 a 2 skrutka v strede
                    float fAngle_z_deg = 0;

                    if (i > 1) // 3-6 skrutka
                    {
                        fAngle_y_deg = 0;
                        fAngle_z_deg = -90 + (float)Geom2D.RadiansToDegrees(m_fRoofPitch_rad);
                    }

                    screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, fAngle_y_deg, fAngle_z_deg);
                }
            }
        }

        protected override void loadIndices()
        {
            int secNum = 9;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 10, 13, 14);
            AddRectangleIndices_CCW_1234(TriangleIndices, 10, 11, 12, 13);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 14, 15);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 4, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 3, 4);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 5, 6, 7);

            // Shell Surface
            for (int i = 0; i < secNum - 3; i++)
            {
                AddRectangleIndices_CW_1234(TriangleIndices, i, secNum + i, secNum + i + 1, i + 1);
            }

            AddRectangleIndices_CW_1234(TriangleIndices, 6, 15, 8, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 8, 9, 0);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // Front Side
            for (int i = 0; i < PointsOut2D.Length; i++)
            {
                if (i < (PointsOut2D.Length) - 1)
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[i + 1];
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
            for (int i = 0; i < PointsOut2D.Length; i++)
            {
                if (i < (PointsOut2D.Length) - 1)
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
            for (int i = 0; i < PointsOut2D.Length; i++)
            {
                if (i < PointsOut2D.Length - 1)
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[i + ITotNoPointsin2D + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[ITotNoPointsin2D];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[13]);

            return wireFrame;
        }


        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_M)
            {
                CConCom_Plate_M refPlate = (CConCom_Plate_M)plate;
                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fbX3 = refPlate.m_fbX3;
                this.m_fhY = refPlate.m_fhY;
                this.m_fRoofPitch_rad = refPlate.m_fRoofPitch_rad;
                this.m_fGamma1_rad = refPlate.m_fGamma1_rad;
                this.m_fe_min_x = refPlate.m_fe_min_x;
                this.m_fe_min_y = refPlate.m_fe_min_y;
            }
        }
    }
}
