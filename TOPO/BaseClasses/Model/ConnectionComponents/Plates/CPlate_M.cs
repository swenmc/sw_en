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
            float fbX2_temp, // Wind post width
            float fRoofPitch_rad,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_M screwArrangement_temp,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_M;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX3 = fbX3_temp;
            m_fhY = fhY_temp;
            Ft = ft_platethickness; //0.02f;
            m_fbX2 = fbX2_temp;
            m_fRoofPitch_rad = fRoofPitch_rad;
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
                //arrConnectorControlPoints3D = new Point3D[screwArrangement_temp.IHolesNumber];
                screwArrangement_temp.Calc_HolesCentersCoord2D(Ft, Fb_X1, Fb_X2, Fb_X3, Fh_Y);
                //Calc_HolesControlPointsCoord3D(screwArrangement_temp);
                //GenerateConnectors(screwArrangement_temp);
            }

            Width_bx = 2 * m_fbX1 + 2 * m_fbX2 + 2 * m_fbX3;
            Height_hy = m_fhY;
            fArea = PolygonArea();
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
                fA_v_zv -= iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
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
            float fGamma1 = MathF.fPI / 6f;
            float fGamma2 = 0.5f * MathF.fPI - fGamma1;

            //Priemetry
            // x priblizne ???
            float fx1 = m_fbX1 * (float)Math.Sin(fGamma1);
            float fx3 = m_fbX3 * (float)Math.Sin(fGamma1);

            float fy1 = fx1 * (float)Math.Tan(m_fRoofPitch_rad);
            float fy3 = fx3 * (float)Math.Tan(m_fRoofPitch_rad);

            float fz1 = MathF.Sqrt(MathF.Pow2(m_fbX1) + MathF.Pow2(fy1));
            float fz3 = MathF.Sqrt(MathF.Pow2(m_fbX3) + MathF.Pow2(fy3));

            float fx11 = m_fhY * (float)Math.Cos(fGamma1);
            float fx31 = m_fhY * (float)Math.Cos(fGamma1);

            float fz11 = m_fhY * (float)Math.Sin(fGamma1);
            float fz31 = m_fhY * (float)Math.Sin(fGamma1);

            float fx12 = fx1 - fx11;
            float fx32 = fx3 - fx31;

            float fy11 = fx11 * (float)Math.Tan(m_fRoofPitch_rad);
            float fy31 = fx31 * (float)Math.Tan(m_fRoofPitch_rad);

            float fy12 = fy1 - fy11;
            float fy32 = fy3 - fy31;

            float fy2 = m_fbX2 * (float)Math.Tan(m_fRoofPitch_rad);

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
            arrPoints3D[2].Y = m_fhY + fy2 + fy32 + ft_y;
            arrPoints3D[2].Z = - fz3 - fz31;

            arrPoints3D[3].X = arrPoints3D[2].X + fx31 - ft_x;
            arrPoints3D[3].Y = m_fhY + fy2 + fy3 + ft_y;
            arrPoints3D[3].Z = -fz3;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[1].Z;

            arrPoints3D[5].X = arrPoints3D[0].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = arrPoints3D[0].X - fx1 - ft_x;
            arrPoints3D[6].Y = m_fhY - fy1 + ft_y;
            arrPoints3D[6].Z = - fz1;

            arrPoints3D[7].X = arrPoints3D[0].X - fx12 - ft_x;
            arrPoints3D[7].Y = m_fhY - fy12 + ft_y;
            arrPoints3D[7].Z = -fz1 - fz11;

            // Second layer
            arrPoints3D[8].X = arrPoints3D[0].X - fx12;
            arrPoints3D[8].Y = m_fhY - fy12;
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
    }
}
