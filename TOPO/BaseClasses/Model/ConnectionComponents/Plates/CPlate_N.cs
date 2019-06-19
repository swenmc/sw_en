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
    public class CConCom_Plate_N : CPlate
    {
        private float m_fbX1; // Width of flat part connected to rafter

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

        private float m_fbX2; // Diagonal part in slope

        private float m_fbX3; // Width of flat part connected to colum

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

        public float m_fZ; // Column cross-section depth - Not used in 2D model

        public float FZ
        {
            get
            {
                return m_fZ;
            }

            set
            {
                m_fZ = value;
            }
        }

        private float m_alpha1_rad;

        public float Alpha1_rad
        {
            get
            {
                return m_alpha1_rad;
            }

            set
            {
                m_alpha1_rad = value;
            }
        }

        private float alpha2_rad;
        private float x_a;

        public int m_iHolesNumber = 0;

        public CConCom_Plate_N()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_N;
            BIsDisplayed = true;
        }

        public CConCom_Plate_N(string sName_temp,
            CPoint controlpoint,
            float fbX1_temp,
            float fbX3_temp,
            float fhY_temp,
            float f_Z_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_N screwArrangement_temp,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_N;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 12;
            ITotNoPointsin3D = 24;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX3 = fbX3_temp;
            m_fhY = fhY_temp;
            m_fZ = f_Z_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            alpha2_rad = MathF.fPI / 8f; // 22.5 deg
            m_alpha1_rad = MathF.fPI / 2f - alpha2_rad; // 67.5 deg
            x_a = (float)Math.Tan(alpha2_rad) * m_fZ;
            m_fbX2 = m_fZ / (float)Math.Sin(m_alpha1_rad);

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement_temp != null)
            {
                screwArrangement_temp.Calc_HolesCentersCoord2D(Fb_X1, m_fbX2, Fb_X3, Fh_Y);
                arrConnectorControlPoints3D = new Point3D[screwArrangement_temp.IHolesNumber];
                Calc_HolesControlPointsCoord3D(screwArrangement_temp);
                GenerateConnectors(screwArrangement_temp);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            Width_bx = 2 * m_fbX1 + 2 * m_fbX2 + m_fbX3;
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
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = PointsOut2D[2].X + m_fbX3;
            PointsOut2D[3].Y = 0;

            PointsOut2D[4].X = PointsOut2D[3].X + m_fbX2;
            PointsOut2D[4].Y = 0;

            PointsOut2D[5].X = PointsOut2D[4].X + m_fbX1;
            PointsOut2D[5].Y = 0;

            PointsOut2D[6].X = PointsOut2D[5].X;
            PointsOut2D[6].Y = m_fhY;

            PointsOut2D[7].X = PointsOut2D[4].X;
            PointsOut2D[7].Y = m_fhY;

            PointsOut2D[8].X = PointsOut2D[3].X;
            PointsOut2D[8].Y = m_fhY;

            PointsOut2D[9].X = PointsOut2D[2].X;
            PointsOut2D[9].Y = m_fhY;

            PointsOut2D[10].X = PointsOut2D[1].X;
            PointsOut2D[10].Y = m_fhY;

            PointsOut2D[11].X = PointsOut2D[0].X;
            PointsOut2D[11].Y = m_fhY;
        }

        public override void Calc_Coord3D()
        {
            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = arrPoints3D[1].X + x_a;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = m_fZ;

            arrPoints3D[3].X = arrPoints3D[2].X + m_fbX3;
            arrPoints3D[3].Y = 0;
            arrPoints3D[3].Z = m_fZ;

            arrPoints3D[4].X = arrPoints3D[3].X + x_a;
            arrPoints3D[4].Y = 0;
            arrPoints3D[4].Z = 0;

            arrPoints3D[5].X = arrPoints3D[4].X + m_fbX1;
            arrPoints3D[5].Y = 0;
            arrPoints3D[5].Z = 0;

            arrPoints3D[6].X = arrPoints3D[5].X;
            arrPoints3D[6].Y = m_fhY;
            arrPoints3D[6].Z = 0;

            arrPoints3D[7].X = arrPoints3D[4].X;
            arrPoints3D[7].Y = m_fhY;
            arrPoints3D[7].Z = 0;

            arrPoints3D[8].X = arrPoints3D[3].X;
            arrPoints3D[8].Y = m_fhY;
            arrPoints3D[8].Z = m_fZ;

            arrPoints3D[9].X = arrPoints3D[2].X;
            arrPoints3D[9].Y = m_fhY;
            arrPoints3D[9].Z = m_fZ;

            arrPoints3D[10].X = arrPoints3D[1].X;
            arrPoints3D[10].Y = m_fhY;
            arrPoints3D[10].Z = 0;

            arrPoints3D[11].X = arrPoints3D[0].X;
            arrPoints3D[11].Y = m_fhY;
            arrPoints3D[11].Z = 0;

            // Second layer
            float fangle_aux_rad = (MathF.fPI / 2f + alpha2_rad) / 2f; // Polovica uhla ktory zvieraju strana bx1 a bx2 (celna a sikma)
            float fx_aux = Ft / (float)Math.Tan(fangle_aux_rad);

            arrPoints3D[12].X = arrPoints3D[0].X;
            arrPoints3D[12].Y = arrPoints3D[0].Y;
            arrPoints3D[12].Z = arrPoints3D[0].Z + Ft;

            arrPoints3D[13].X = arrPoints3D[1].X - fx_aux;
            arrPoints3D[13].Y = arrPoints3D[1].Y;
            arrPoints3D[13].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[14].X = arrPoints3D[2].X - fx_aux;
            arrPoints3D[14].Y = arrPoints3D[2].Y;
            arrPoints3D[14].Z = arrPoints3D[2].Z + Ft;

            arrPoints3D[15].X = arrPoints3D[3].X + fx_aux;
            arrPoints3D[15].Y = arrPoints3D[3].Y;
            arrPoints3D[15].Z = arrPoints3D[3].Z + Ft;

            arrPoints3D[16].X = arrPoints3D[4].X + fx_aux;
            arrPoints3D[16].Y = arrPoints3D[4].Y;
            arrPoints3D[16].Z = arrPoints3D[4].Z + Ft;

            arrPoints3D[17].X = arrPoints3D[5].X;
            arrPoints3D[17].Y = arrPoints3D[5].Y;
            arrPoints3D[17].Z = arrPoints3D[5].Z + Ft;

            arrPoints3D[18].X = arrPoints3D[6].X;
            arrPoints3D[18].Y = arrPoints3D[6].Y;
            arrPoints3D[18].Z = arrPoints3D[6].Z + Ft;

            arrPoints3D[19].X = arrPoints3D[7].X + fx_aux;
            arrPoints3D[19].Y = arrPoints3D[7].Y;
            arrPoints3D[19].Z = arrPoints3D[7].Z + Ft;

            arrPoints3D[20].X = arrPoints3D[8].X + fx_aux;
            arrPoints3D[20].Y = arrPoints3D[8].Y;
            arrPoints3D[20].Z = arrPoints3D[8].Z + Ft;

            arrPoints3D[21].X = arrPoints3D[9].X - fx_aux;
            arrPoints3D[21].Y = arrPoints3D[9].Y;
            arrPoints3D[21].Z = arrPoints3D[9].Z + Ft;

            arrPoints3D[22].X = arrPoints3D[10].X - fx_aux;
            arrPoints3D[22].Y = arrPoints3D[10].Y;
            arrPoints3D[22].Z = arrPoints3D[10].Z + Ft;

            arrPoints3D[23].X = arrPoints3D[11].X;
            arrPoints3D[23].Y = arrPoints3D[11].Y;
            arrPoints3D[23].Z = arrPoints3D[11].Z + Ft;
        }

        void Calc_HolesControlPointsCoord3D(CScrewArrangement screwArrangement)
        {
            // 3 x 4 screws = 12 screws in the plate (square arrangement 4 screws in group)

            float fx_edge = 0.020f;
            float fy_edge = 0.020f;

            // Left back
            arrConnectorControlPoints3D[0].X = fx_edge;
            arrConnectorControlPoints3D[0].Y = fy_edge;
            arrConnectorControlPoints3D[0].Z = -Ft; // TODO Position depends on screw length

            arrConnectorControlPoints3D[1].X = m_fbX1 - fx_edge;
            arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y;
            arrConnectorControlPoints3D[1].Z = arrConnectorControlPoints3D[0].Z;

            arrConnectorControlPoints3D[2].X = arrConnectorControlPoints3D[0].X;
            arrConnectorControlPoints3D[2].Y = m_fhY - fy_edge;
            arrConnectorControlPoints3D[2].Z = arrConnectorControlPoints3D[0].Z;

            arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[1].X;
            arrConnectorControlPoints3D[3].Y = arrConnectorControlPoints3D[2].Y;
            arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[0].Z;

            // Right back
            arrConnectorControlPoints3D[4].X = m_fbX1 + 2 * x_a + m_fbX3 + fx_edge;
            arrConnectorControlPoints3D[4].Y = arrConnectorControlPoints3D[0].Y;
            arrConnectorControlPoints3D[4].Z = arrConnectorControlPoints3D[0].Z;

            arrConnectorControlPoints3D[5].X = 2 * m_fbX1 + 2 * x_a + m_fbX3 - fx_edge;
            arrConnectorControlPoints3D[5].Y = arrConnectorControlPoints3D[4].Y;
            arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[0].Z;

            arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[4].X;
            arrConnectorControlPoints3D[6].Y = arrConnectorControlPoints3D[2].Y;
            arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[0].Z;

            arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[5].X;
            arrConnectorControlPoints3D[7].Y = arrConnectorControlPoints3D[2].Y;
            arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[0].Z;

            // Middle front
            arrConnectorControlPoints3D[8].X = m_fbX1 + x_a + fx_edge;
            arrConnectorControlPoints3D[8].Y = arrConnectorControlPoints3D[0].Y;
            arrConnectorControlPoints3D[8].Z = m_fZ - Ft;

            arrConnectorControlPoints3D[9].X = m_fbX1 + x_a + m_fbX3 - fx_edge;
            arrConnectorControlPoints3D[9].Y = arrConnectorControlPoints3D[0].Y;
            arrConnectorControlPoints3D[9].Z = arrConnectorControlPoints3D[8].Z;

            arrConnectorControlPoints3D[10].X = arrConnectorControlPoints3D[8].X;
            arrConnectorControlPoints3D[10].Y = arrConnectorControlPoints3D[2].Y;
            arrConnectorControlPoints3D[10].Z = arrConnectorControlPoints3D[8].Z;

            arrConnectorControlPoints3D[11].X = arrConnectorControlPoints3D[9].X;
            arrConnectorControlPoints3D[11].Y = arrConnectorControlPoints3D[2].Y;
            arrConnectorControlPoints3D[11].Z = arrConnectorControlPoints3D[8].Z;
        }

        void GenerateConnectors(CScrewArrangement screwArrangement)
        {
            if (screwArrangement.IHolesNumber > 0)
            {
                screwArrangement.Screws = new CScrew[screwArrangement.IHolesNumber];

                for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        screwArrangement.Screws[i] = new CScrew("TEK", controlpoint, screwArrangement.referenceScrew.Gauge, screwArrangement.referenceScrew.Diameter_thread, screwArrangement.referenceScrew.D_h_headdiameter, screwArrangement.referenceScrew.D_w_washerdiameter, screwArrangement.referenceScrew.T_w_washerthickness, screwArrangement.referenceScrew.Length, screwArrangement.referenceScrew.Mass, 0, -90, 0, true);
                }
            }
        }

        protected override void loadIndices()
        {
            int secNum = 12;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 13, 22, 23);
            AddRectangleIndices_CCW_1234(TriangleIndices, 13, 14, 21, 22);
            AddRectangleIndices_CCW_1234(TriangleIndices, 14, 15, 20, 21);
            AddRectangleIndices_CCW_1234(TriangleIndices, 15, 16, 19, 20);
            AddRectangleIndices_CCW_1234(TriangleIndices, 16, 17, 18, 19);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 10, 11);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 9, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 8, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 7, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 6, 7);

            // Shell Surface
            DrawCaraLaterals_CW(secNum, TriangleIndices);
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

            wireFrame.Points.Add(arrPoints3D[13]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[15]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[19]);

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

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[7]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[23]);

            return wireFrame;
        }
    }
}
