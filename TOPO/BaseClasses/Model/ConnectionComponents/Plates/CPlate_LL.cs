using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using _3DTools;

namespace BaseClasses
{
    public class CConCom_Plate_LL : CPlate
    {
        public float m_fbX1; // Long vertical part in connection (member z-direction)
        public float m_fbX2; // short horizontal part in connection (member y-direction)
        public float m_fhY;  // Member x-direction
        public float m_flZ;  // Not used in 2D model
        public float m_ft;   // Not used in 2D model

        public CConCom_Plate_LL()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_LL;
            BIsDisplayed = true;
        }

        public CConCom_Plate_LL(string sName_temp, CPoint controlpoint, float fbX1_temp, float fbX2_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, CScrew referenceScrew_temp, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_LL;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 12;
            ITotNoPointsin3D = 26;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            referenceScrew = referenceScrew_temp;
            // m_arrPlateScrews = arrPlateConnectors_temp; // Generate
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber, 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D();
            Calc_HolesControlPointsCoord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors();

            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = m_fhY;
            fThickness_tz = m_ft;
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fWeight = GetWeightIgnoringHoles();

            fA_g = Get_A_rect(2 * m_ft, m_fbX1);
            int iNumberOfScrewsInSection = 8; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * referenceScrew.Diameter_thread;
            fA_v_zv = Get_A_rect(2 * m_ft, m_fbX1);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * referenceScrew.Diameter_thread;
            fI_yu = 2 * Get_I_yu_rect(m_ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = 0;
            PointsOut2D[1, 1] = m_fbX1;

            PointsOut2D[2, 0] = PointsOut2D[1, 0] + m_fbX2;
            PointsOut2D[2, 1] = PointsOut2D[1, 1];

            PointsOut2D[3, 0] = PointsOut2D[2, 0];
            PointsOut2D[3, 1] = PointsOut2D[0, 1];

            PointsOut2D[4, 0] = PointsOut2D[3, 0] + m_fhY - m_ft;
            PointsOut2D[4, 1] = PointsOut2D[3, 1];

            PointsOut2D[5, 0] = PointsOut2D[4, 0];
            PointsOut2D[5, 1] = PointsOut2D[2, 1];

            PointsOut2D[6, 0] = PointsOut2D[5, 0];
            PointsOut2D[6, 1] = PointsOut2D[5, 1] + m_flZ;

            PointsOut2D[7, 0] = PointsOut2D[2, 0];
            PointsOut2D[7, 1] = PointsOut2D[6, 1];

            PointsOut2D[8, 0] = PointsOut2D[0, 0];
            PointsOut2D[8, 1] = PointsOut2D[7, 1];

            PointsOut2D[9, 0] = PointsOut2D[0, 0] - (m_fhY - m_ft);
            PointsOut2D[9, 1] = PointsOut2D[6, 1];

            PointsOut2D[10, 0] = PointsOut2D[9, 0];
            PointsOut2D[10, 1] = PointsOut2D[1, 1];

            PointsOut2D[11, 0] = PointsOut2D[10, 0];
            PointsOut2D[11, 1] = PointsOut2D[0, 1];
        }

        void Calc_Coord3D()
        {
            // First layer

            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = arrPoints3D[1].X + m_fbX2;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X + m_fbX1;
            arrPoints3D[3].Y = 0;
            arrPoints3D[3].Z = 0;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[2].X;
            arrPoints3D[6].Y = m_ft;
            arrPoints3D[6].Z = arrPoints3D[2].Z;

            arrPoints3D[7].X = arrPoints3D[1].X;
            arrPoints3D[7].Y = m_ft;
            arrPoints3D[7].Z = arrPoints3D[1].Z;

            arrPoints3D[8].X = arrPoints3D[1].X;
            arrPoints3D[8].Y = m_fhY;
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            arrPoints3D[9].X = arrPoints3D[0].X;
            arrPoints3D[9].Y = m_fhY;
            arrPoints3D[9].Z = arrPoints3D[0].Z;

            // Second layer

            arrPoints3D[10].X = arrPoints3D[0].X;
            arrPoints3D[10].Y = arrPoints3D[0].Y;
            arrPoints3D[10].Z = m_ft;

            arrPoints3D[11].X = m_fbX1 - m_ft;
            arrPoints3D[11].Y = arrPoints3D[10].X;
            arrPoints3D[11].Z = m_ft;

            arrPoints3D[12].X = m_fbX1 + m_fbX2 + m_ft;
            arrPoints3D[12].Y = arrPoints3D[11].Y;
            arrPoints3D[12].Z = m_ft;

            arrPoints3D[13].X = arrPoints3D[3].X;
            arrPoints3D[13].Y = arrPoints3D[3].Y;
            arrPoints3D[13].Z = m_ft;

            arrPoints3D[14].X = arrPoints3D[4].X;
            arrPoints3D[14].Y = arrPoints3D[4].Y;
            arrPoints3D[14].Z = m_ft;

            arrPoints3D[15].X = arrPoints3D[12].X;
            arrPoints3D[15].Y = arrPoints3D[5].Y;
            arrPoints3D[15].Z = m_ft;

            arrPoints3D[16].X = arrPoints3D[11].X;
            arrPoints3D[16].Y = arrPoints3D[8].Y;
            arrPoints3D[16].Z = m_ft;

            arrPoints3D[17].X = arrPoints3D[9].X;
            arrPoints3D[17].Y = arrPoints3D[9].Y;
            arrPoints3D[17].Z = m_ft;

            // Third layer

            arrPoints3D[18].X = arrPoints3D[11].X;
            arrPoints3D[18].Y = arrPoints3D[11].Y;
            arrPoints3D[18].Z = m_flZ;

            arrPoints3D[19].X = arrPoints3D[12].X;
            arrPoints3D[19].Y = arrPoints3D[12].Y;
            arrPoints3D[19].Z = m_flZ;

            arrPoints3D[20].X = arrPoints3D[15].X;
            arrPoints3D[20].Y = arrPoints3D[15].Y;
            arrPoints3D[20].Z = m_flZ;

            arrPoints3D[21].X = arrPoints3D[5].X;
            arrPoints3D[21].Y = arrPoints3D[5].Y;
            arrPoints3D[21].Z = m_flZ;

            arrPoints3D[22].X = arrPoints3D[6].X;
            arrPoints3D[22].Y = arrPoints3D[6].Y;
            arrPoints3D[22].Z = m_flZ;

            arrPoints3D[23].X = arrPoints3D[7].X;
            arrPoints3D[23].Y = arrPoints3D[7].Y;
            arrPoints3D[23].Z = m_flZ;

            arrPoints3D[24].X = arrPoints3D[8].X;
            arrPoints3D[24].Y = arrPoints3D[8].Y;
            arrPoints3D[24].Z = m_flZ;

            arrPoints3D[25].X = arrPoints3D[16].X;
            arrPoints3D[25].Y = arrPoints3D[16].Y;
            arrPoints3D[25].Z = m_flZ;
        }

        void Calc_HolesCentersCoord2D()
        {
            float fx_edge = 0.010f;  // y-direction
            float fy_edge1 = 0.010f; // x-direction
            float fy_edge2 = 0.030f; // x-direction
            float fy_edge3 = 0.120f; // x-direction

            // TODO nahradit enumom a switchom

            // TODO OPRAVIT SURADNICE

            if (IHolesNumber == 32) // LLH, LLK
            {
                HolesCentersPoints2D[0, 0] = - m_fhY + m_ft + fy_edge1;
                HolesCentersPoints2D[0, 1] = fx_edge;

                HolesCentersPoints2D[1, 0] = HolesCentersPoints2D[0, 0];
                HolesCentersPoints2D[1, 1] = m_fbX1 - fx_edge;

                HolesCentersPoints2D[2, 0] = -m_fhY + m_ft + fy_edge2;
                HolesCentersPoints2D[2, 1] = 0.5f * m_fbX1;

                HolesCentersPoints2D[3, 0] = -m_fhY + m_ft + fy_edge3;
                HolesCentersPoints2D[3, 1] = HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[4, 0] = -fy_edge3;
                HolesCentersPoints2D[4, 1] = HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[5, 0] = -fy_edge2;
                HolesCentersPoints2D[5, 1] = HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[6, 0] = -fy_edge1;
                HolesCentersPoints2D[6, 1] = HolesCentersPoints2D[0, 1];

                HolesCentersPoints2D[7, 0] = HolesCentersPoints2D[6, 0];
                HolesCentersPoints2D[7, 1] = HolesCentersPoints2D[1, 1];

                float fTemp = (0.5f * m_fbX1 + 0.5f * m_flZ);

                HolesCentersPoints2D[8, 0] = HolesCentersPoints2D[7, 0];
                HolesCentersPoints2D[8, 1] = fTemp + HolesCentersPoints2D[7, 1];

                HolesCentersPoints2D[9, 0] = HolesCentersPoints2D[6, 0];
                HolesCentersPoints2D[9, 1] = fTemp + HolesCentersPoints2D[6, 1];

                HolesCentersPoints2D[10, 0] = HolesCentersPoints2D[5, 0];
                HolesCentersPoints2D[10, 1] = fTemp + HolesCentersPoints2D[5, 1];

                HolesCentersPoints2D[11, 0] = HolesCentersPoints2D[4, 0];
                HolesCentersPoints2D[11, 1] = fTemp + HolesCentersPoints2D[4, 1];

                HolesCentersPoints2D[12, 0] = HolesCentersPoints2D[3, 0];
                HolesCentersPoints2D[12, 1] = fTemp + HolesCentersPoints2D[3, 1];

                HolesCentersPoints2D[13, 0] = HolesCentersPoints2D[2, 0];
                HolesCentersPoints2D[13, 1] = fTemp + HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[14, 0] = HolesCentersPoints2D[0, 0];
                HolesCentersPoints2D[14, 1] = fTemp + HolesCentersPoints2D[1, 1];

                HolesCentersPoints2D[15, 0] = HolesCentersPoints2D[1, 0];
                HolesCentersPoints2D[15, 1] = fTemp + HolesCentersPoints2D[0, 1];

                for (int i = 0; i < IHolesNumber / 2; i++)
                {
                    HolesCentersPoints2D[IHolesNumber / 2 + i, 0] = m_fbX2 -HolesCentersPoints2D[(IHolesNumber / 2 - i - 1), 0];
                    HolesCentersPoints2D[IHolesNumber / 2 + i, 1] = HolesCentersPoints2D[(IHolesNumber / 2 - i - 1), 1];
                }
            }
            else
            {
                // Not defined expected number of holes for LL plate
            }
        }

        void Calc_HolesControlPointsCoord3D()
        {
            // TODO UPRAVIT SURADNICE

            float fx_edge = 0.010f;
            float fy_edge1 = 0.010f;
            float fy_edge2 = 0.030f;
            float fy_edge3 = 0.120f;

            // TODO nahradit enumom a switchom

            if (IHolesNumber == 32) // LLH, LLK
            {
                arrConnectorControlPoints3D[0].X = fx_edge;
                arrConnectorControlPoints3D[0].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[0].Z = - m_ft; // TODO Position depends on screw length

                arrConnectorControlPoints3D[1].X = m_fbX1 - fx_edge;
                arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y;
                arrConnectorControlPoints3D[1].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[2].X = 0.5f * m_fbX1;
                arrConnectorControlPoints3D[2].Y = m_fhY - fy_edge2;
                arrConnectorControlPoints3D[2].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[2].X;
                arrConnectorControlPoints3D[3].Y = m_fhY - fy_edge3;
                arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[4].X = arrConnectorControlPoints3D[2].X;
                arrConnectorControlPoints3D[4].Y = fy_edge3;
                arrConnectorControlPoints3D[4].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[5].X = arrConnectorControlPoints3D[2].X;
                arrConnectorControlPoints3D[5].Y = fy_edge2;
                arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[6].Y = fy_edge1;
                arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[1].X;
                arrConnectorControlPoints3D[7].Y = fy_edge1;
                arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[8].X = m_fbX1 - 2 * m_ft; // TODO Position depends on screw length
                arrConnectorControlPoints3D[8].Y = fy_edge1;
                arrConnectorControlPoints3D[8].Z = m_flZ - fx_edge;

                arrConnectorControlPoints3D[9].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[9].Y = arrConnectorControlPoints3D[8].Y;
                arrConnectorControlPoints3D[9].Z = fx_edge;

                arrConnectorControlPoints3D[10].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[10].Y = fy_edge2;
                arrConnectorControlPoints3D[10].Z = 0.5f * m_flZ;

                arrConnectorControlPoints3D[11].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[11].Y = fy_edge3;
                arrConnectorControlPoints3D[11].Z = arrConnectorControlPoints3D[10].Z;

                arrConnectorControlPoints3D[12].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[12].Y = m_fhY - fy_edge3;
                arrConnectorControlPoints3D[12].Z = arrConnectorControlPoints3D[10].Z;

                arrConnectorControlPoints3D[13].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[13].Y = m_fhY - fy_edge2;
                arrConnectorControlPoints3D[13].Z = arrConnectorControlPoints3D[10].Z;

                arrConnectorControlPoints3D[14].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[14].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[14].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[15].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[15].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[15].Z = arrConnectorControlPoints3D[9].Z;

                float fb_temp = m_fbX1 * 2 + m_fbX2;

                for (int i = 0; i < IHolesNumber / 2; i++)
                {
                    arrConnectorControlPoints3D[IHolesNumber / 2 + i].X = fb_temp - arrConnectorControlPoints3D[(IHolesNumber / 2 - i - 1)].X;
                    arrConnectorControlPoints3D[IHolesNumber / 2 + i].Y = arrConnectorControlPoints3D[(IHolesNumber / 2 - i - 1)].Y;
                    arrConnectorControlPoints3D[IHolesNumber / 2 + i].Z = arrConnectorControlPoints3D[(IHolesNumber / 2 - i - 1)].Z;
                }
            }
            else
            {
                // Not defined expected number of holes for LL plate
            }
        }

        void GenerateConnectors()
        {
            if (IHolesNumber > 0)
            {
                m_arrPlateScrews = new CScrew[IHolesNumber];

                // TODO Ondrej 15/07/2018
                // Tu sa pridava sktrutka do plechu, v klada sa do pozicie na plechu v suradnicovom systeme plechu (controlpoint) a otoci sa do pozicie v LCS plechu
                // Potom je potrebne pri posune a otoceni plechu otocit aj vsetky skrutky, ktore k nemu patria
                // Povodne suradnice skrutky su ako suradnice valca kde x je v smere vysky valca

                // Update 1
                // Po tomto vlozeni skrutiek do plechu by sa mali suradnice skrutiek prepocitat z povodnych, v ktorych su zadane do suradnicoveho systemu plechu a ulozit

                
                for (int i = 0; i < IHolesNumber; i++)
                {
                    if (i < IHolesNumber / 4) // Left
                    {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        m_arrPlateScrews[i] = new CScrew("TEK", controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Weight, 0, -90, 0, true);
                    }
                    else if (i < IHolesNumber * 2 / 4) // Front Left
                    {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        m_arrPlateScrews[i] = new CScrew("TEK", controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Weight, 0, 0, 0, true);
                    }
                    else if (i < IHolesNumber * 3 / 4) // Front Right
                    {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        m_arrPlateScrews[i] = new CScrew("TEK", controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Weight, 0, 180, 0, true);
                    }
                    else // Right
                    {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        m_arrPlateScrews[i] = new CScrew("TEK", controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Weight, 0, -90, 0, true);
                    }
                }
            }
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 11, 10);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 12, 11);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 13, 12);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 12, 19, 18);

            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 23, 22);

            // Top
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 17, 16, 8);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 16, 25, 24);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 21, 20, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 15, 14, 4);

            // Front
            AddRectangleIndices_CCW_1234(TriangleIndices, 10, 11, 16, 17);
            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 13, 14, 15);

            AddRectangleIndices_CCW_1234(TriangleIndices, 18, 23, 24, 25);
            AddRectangleIndices_CCW_1234(TriangleIndices, 18, 19, 22, 23);
            AddRectangleIndices_CCW_1234(TriangleIndices, 19, 20, 21, 22);

            // Back
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 4, 3, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 8, 1, 0);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7, 6, 2, 1);

            // Side
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 10, 17, 9);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 18, 25, 16);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 24, 23, 7);

            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 6, 22, 21);
            AddRectangleIndices_CCW_1234(TriangleIndices, 20, 19, 12, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 14, 13);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            // z = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[0]);

            // z = t
            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[13]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[15]);

            // z = L
            wireFrame.Points.Add(arrPoints3D[18]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[19]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[20]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[21]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[22]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[23]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[24]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[25]);
            wireFrame.Points.Add(arrPoints3D[18]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[15]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[17]);

            return wireFrame;
        }

        // TODO Ondrej - Indices pre wireframe plechu
        public override void loadWireFrameIndices()
        {
            Int32Collection wireFrameIndices = new Int32Collection();

            // z = 0
            wireFrameIndices.Add(0);
            wireFrameIndices.Add(1);

            wireFrameIndices.Add(1);
            wireFrameIndices.Add(2);

            wireFrameIndices.Add(2);
            wireFrameIndices.Add(3);

            wireFrameIndices.Add(3);
            wireFrameIndices.Add(4);

            wireFrameIndices.Add(4);
            wireFrameIndices.Add(5);

            wireFrameIndices.Add(5);
            wireFrameIndices.Add(6);

            wireFrameIndices.Add(6);
            wireFrameIndices.Add(7);

            wireFrameIndices.Add(7);
            wireFrameIndices.Add(8);

            wireFrameIndices.Add(8);
            wireFrameIndices.Add(9);

            wireFrameIndices.Add(9);
            wireFrameIndices.Add(0);

            // z = t
            wireFrameIndices.Add(10);
            wireFrameIndices.Add(11);

            wireFrameIndices.Add(16);
            wireFrameIndices.Add(17);

            wireFrameIndices.Add(12);
            wireFrameIndices.Add(13);

            wireFrameIndices.Add(13);
            wireFrameIndices.Add(14);

            wireFrameIndices.Add(14);
            wireFrameIndices.Add(15);

            wireFrameIndices.Add(10);
            wireFrameIndices.Add(17);

            wireFrameIndices.Add(11);
            wireFrameIndices.Add(16);

            wireFrameIndices.Add(12);
            wireFrameIndices.Add(15);

            // z = L
            wireFrameIndices.Add(18);
            wireFrameIndices.Add(19);

            wireFrameIndices.Add(19);
            wireFrameIndices.Add(20);

            wireFrameIndices.Add(20);
            wireFrameIndices.Add(21);

            wireFrameIndices.Add(21);
            wireFrameIndices.Add(22);

            wireFrameIndices.Add(22);
            wireFrameIndices.Add(23);

            wireFrameIndices.Add(23);
            wireFrameIndices.Add(24);

            wireFrameIndices.Add(24);
            wireFrameIndices.Add(25);

            wireFrameIndices.Add(25);
            wireFrameIndices.Add(18);

            // Lateral
            wireFrameIndices.Add(0);
            wireFrameIndices.Add(10);

            wireFrameIndices.Add(3);
            wireFrameIndices.Add(13);

            wireFrameIndices.Add(4);
            wireFrameIndices.Add(14);

            wireFrameIndices.Add(15);
            wireFrameIndices.Add(20);

            wireFrameIndices.Add(12);
            wireFrameIndices.Add(19);

            wireFrameIndices.Add(5);
            wireFrameIndices.Add(21);

            wireFrameIndices.Add(6);
            wireFrameIndices.Add(22);

            wireFrameIndices.Add(7);
            wireFrameIndices.Add(23);

            wireFrameIndices.Add(8);
            wireFrameIndices.Add(24);

            wireFrameIndices.Add(16);
            wireFrameIndices.Add(25);

            wireFrameIndices.Add(11);
            wireFrameIndices.Add(18);

            wireFrameIndices.Add(9);
            wireFrameIndices.Add(17);
        }
    }
}
