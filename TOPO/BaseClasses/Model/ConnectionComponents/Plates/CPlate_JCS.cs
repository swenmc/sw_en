using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using _3DTools;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_JCS : CPlate_Frame
    {
        private float m_fd_crsc;

        public float Fd_crsc
        {
            get
            {
                return m_fd_crsc;
            }

            set
            {
                m_fd_crsc = value;
            }
        }

        private float m_fw_apexHalfLength;

        public float Fw_apexHalfLength
        {
            get
            {
                return m_fw_apexHalfLength;
            }

            set
            {
                m_fw_apexHalfLength = value;
            }
        }

        private float m_flZ;

        public float Fl_Z
        {
            get
            {
                return m_flZ;
            }

            set
            {
                m_flZ = value;
            }
        }

        float m_fSlope_rad;

        public float FSlope_rad
        {
            get
            {
                return m_fSlope_rad;
            }

            set
            {
                m_fSlope_rad = value;
            }
        }

        bool m_bScrewInPlusZDirection;

        public bool ScrewInPlusZDirection
        {
            get
            {
                return m_bScrewInPlusZDirection;
            }

            set
            {
                m_bScrewInPlusZDirection = value;
            }
        }

        // Private
        private float m_fbX1;

        /*
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
        }*/

        private float m_fbX1_AndLips;

        /*
        public float Fb_X1_AndLips
        {
            get
            {
                return m_fbX1_AndLips;
            }

            set
            {
                m_fbX1_AndLips = value;
            }
        }*/

        private float m_fbX2_AndLips;

        /*
        public float Fb_X2_AndLips
        {
            get
            {
                return m_fbX2_AndLips;
            }

            set
            {
                m_fbX2_AndLips = value;
            }
        }*/

        private float m_fhY1;

        /*
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
        }*/

        private float m_fbX2;

        /*
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
        }*/

        private float m_fhY2;

        /*
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
        }*/

        private float m_fLipBase_dim_x;

        public CConCom_Plate_JCS()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;
        }

        public CConCom_Plate_JCS(string sName_temp,
            Point3D controlpoint,
            //float fb_temp,
            //float fh_1_temp,
            //float fh_2_temp,
            float fd_crsc,
            float fw_apexHalfLength,
            float fL_temp,
            float fSlope_rad,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            bool bScrewInPlusZDirection,
            CScrewArrangement screwArrangement)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;

            ITotNoPointsin2D = 9;
            ITotNoPointsin3D = 26;

            m_pControlPoint = controlpoint;

            Fd_crsc = fd_crsc;
            Fw_apexHalfLength = fw_apexHalfLength;
            Fl_Z = fL_temp;
            FSlope_rad = fSlope_rad;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;
            ScrewInPlusZDirection = bScrewInPlusZDirection;

            UpdatePlateData(screwArrangement);
        }

        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            if (MathF.d_equal(m_fSlope_rad, 0))
                m_fSlope_rad = (float)Math.Atan((m_fhY2 - m_fhY1) / (0.5 * m_fbX2));
            //else
                //m_fhY2 = m_fhY1 + ((float)Math.Tan(m_fSlope_rad) * (0.5f * m_fbX2)); // ! not implemented

            // Recalculate dimensions
            m_fhY1 = m_fd_crsc * (float)Math.Cos(m_fSlope_rad);
            m_fbX2 = 2 * m_fw_apexHalfLength * (float)Math.Cos(m_fSlope_rad);
            m_fbX1 = m_fbX2 - 2 * m_fd_crsc * (float)Math.Sin(m_fSlope_rad);
            m_fhY2 = m_fhY1 + m_fw_apexHalfLength * (float)Math.Sin(m_fSlope_rad);

            m_fLipBase_dim_x = m_flZ / (float)Math.Cos(m_fSlope_rad);
            m_fbX1_AndLips = 2 * m_fLipBase_dim_x + m_fbX1;
            m_fbX2_AndLips = 2 * m_fLipBase_dim_x + m_fbX2;

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            if (screwArrangement != null)
            {
                arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber];
            }

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement != null)
            {
                screwArrangement.Calc_ApexPlateData(m_fLipBase_dim_x, m_fbX1_AndLips, 0, m_fhY1, Ft, m_fSlope_rad, ScrewInPlusZDirection);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();

            Set_BendLinesPoints2D();
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = m_fbX2;
            Height_hy = Math.Max(m_fhY1, m_fhY2);
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = m_fbX2_AndLips; // Total width
            Height_hy_Stretched = m_fhY2;
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

            fA_g = Get_A_rect(Ft, m_fd_crsc);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;
            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fd_crsc);

            fA_vn_zv = fA_v_zv;
            if (screwArrangement != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fd_crsc);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fd_crsc); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        public override void SetMinimumScrewToEdgeDistances(CScrewArrangement screwArrangement)
        {
            SetMinimumScrewToEdgeDistances_Basic(screwArrangement);

            e_min_x -= m_fLipBase_dim_x; // Odpocitame sirku laveho ohybu
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            float fx1_temp = m_flZ * (float)Math.Cos(m_fSlope_rad);
            float fy1_temp = m_flZ * (float)Math.Sin(m_fSlope_rad);

            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fLipBase_dim_x;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X + m_fbX1;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = PointsOut2D[2].X + m_fLipBase_dim_x;
            PointsOut2D[3].Y = 0;

            float fy_5_temp = m_fhY1 - fy1_temp;
            PointsOut2D[4].X = PointsOut2D[3].X + fy_5_temp * Math.Tan(m_fSlope_rad);
            PointsOut2D[4].Y = fy_5_temp;

            PointsOut2D[5].X = PointsOut2D[4].X - fx1_temp;
            PointsOut2D[5].Y = m_fhY1;

            PointsOut2D[6].X = PointsOut2D[5].X - 0.5f * m_fbX2;
            PointsOut2D[6].Y = m_fhY2;

            PointsOut2D[7].X = PointsOut2D[6].X - 0.5f * m_fbX2;
            PointsOut2D[7].Y = PointsOut2D[5].Y;

            PointsOut2D[8].X = PointsOut2D[7].X - fx1_temp;
            PointsOut2D[8].Y = PointsOut2D[4].Y;
        }

        public override void Calc_Coord3D()
        {
            float fx_temp = Fl_Z * (float)Math.Cos(FSlope_rad);
            float fy_temp = Fl_Z * (float)Math.Sin(FSlope_rad);
            float fx_temp2 = Ft * (float)Math.Cos(FSlope_rad);
            float fy_temp2 = Ft * (float)Math.Sin(FSlope_rad);

            float fy_temp3 = fx_temp * (float)Math.Tan(FSlope_rad);

            float f_lip_cut = Fl_Z * (float)Math.Tan(FSlope_rad);
            float fx_temp4 = f_lip_cut * (float)Math.Sin(FSlope_rad);
            float fy_temp4 = f_lip_cut * (float)Math.Cos(FSlope_rad);

            float ftemp6 = Ft * (float)Math.Tan(FSlope_rad);

            float f_lip_cut2 = f_lip_cut - ftemp6;
            float fx_temp5 = f_lip_cut2 * (float)Math.Sin(FSlope_rad);
            float fy_temp5 = f_lip_cut2 * (float)Math.Cos(FSlope_rad);

            float fx_temp6 = Ft / (float)Math.Cos(FSlope_rad);

            float fy_temp6 = Ft / (float)Math.Sin(FSlope_rad);

            float fx_temp7 = (Fd_crsc - ftemp6) * (float)Math.Sin(FSlope_rad);
            float fy_temp7 = (Fd_crsc - ftemp6) * (float)Math.Cos(FSlope_rad);

            arrPoints3D[0].X = -fx_temp5;
            arrPoints3D[0].Y = fy_temp5;
            arrPoints3D[0].Z = Fl_Z;

            arrPoints3D[1].X = 0; // Mala by byt ina
            arrPoints3D[1].Y = 0; // Mala by byt ina
            arrPoints3D[1].Z = Ft;

            arrPoints3D[2].X = 0; // Mala by byt ina
            arrPoints3D[2].Y = 0; // Mala by byt ina
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = fx_temp2;
            arrPoints3D[3].Y = 0;
            arrPoints3D[3].Z = 0;

            arrPoints3D[4].X = m_fbX1 - fx_temp2;
            arrPoints3D[4].Y = 0;
            arrPoints3D[4].Z = 0;

            arrPoints3D[5].X = m_fbX1;
            arrPoints3D[5].Y = 0;
            arrPoints3D[5].Z = 0;

            arrPoints3D[6].X = arrPoints3D[5].X;
            arrPoints3D[6].Y = arrPoints3D[1].Y;
            arrPoints3D[6].Z = Ft;

            arrPoints3D[7].X = m_fbX1 + fx_temp5;
            arrPoints3D[7].Y = arrPoints3D[0].Y;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            arrPoints3D[8].X = m_fbX1 + fx_temp7;
            arrPoints3D[8].Y = fy_temp7;
            arrPoints3D[8].Z = arrPoints3D[0].Z;

            arrPoints3D[9].X = arrPoints3D[8].X;
            arrPoints3D[9].Y = arrPoints3D[8].Y;
            arrPoints3D[9].Z = arrPoints3D[1].Z;

            arrPoints3D[10].X = arrPoints3D[8].X;
            arrPoints3D[10].Y = arrPoints3D[8].Y;
            arrPoints3D[10].Z = arrPoints3D[2].Z;

            arrPoints3D[11].X = arrPoints3D[10].X - fx_temp2;
            arrPoints3D[11].Y = arrPoints3D[10].Y + fy_temp2;
            arrPoints3D[11].Z = arrPoints3D[10].Z;

            arrPoints3D[12].X = 0.5f * m_fbX1;
            arrPoints3D[12].Y = m_fhY2;
            arrPoints3D[12].Z = arrPoints3D[2].Z;

            arrPoints3D[13].X = arrPoints3D[10].X - 2 * fx_temp7 - m_fbX1 + fx_temp2;
            arrPoints3D[13].Y = arrPoints3D[11].Y;
            arrPoints3D[13].Z = arrPoints3D[11].Z;

            arrPoints3D[14].X = arrPoints3D[13].X - fx_temp2;
            arrPoints3D[14].Y = arrPoints3D[10].Y;
            arrPoints3D[14].Z = arrPoints3D[10].Z;

            arrPoints3D[15].X = arrPoints3D[14].X;
            arrPoints3D[15].Y = arrPoints3D[14].Y;
            arrPoints3D[15].Z = arrPoints3D[9].Z;

            arrPoints3D[16].X = arrPoints3D[15].X;
            arrPoints3D[16].Y = arrPoints3D[15].Y;
            arrPoints3D[16].Z = arrPoints3D[8].Z;

            int i_temp = 17;  // Number of point in first layer

            // Second layer
            arrPoints3D[i_temp + 0].X = arrPoints3D[0].X + fx_temp2;
            arrPoints3D[i_temp + 0].Y = arrPoints3D[0].Y + fy_temp2;
            arrPoints3D[i_temp + 0].Z = Fl_Z;

            arrPoints3D[i_temp + 1].X = fx_temp6;
            arrPoints3D[i_temp + 1].Y = 0;
            arrPoints3D[i_temp + 1].Z = arrPoints3D[1].Z;

            arrPoints3D[i_temp + 2].X = arrPoints3D[6].X - fx_temp6;
            arrPoints3D[i_temp + 2].Y = 0;
            arrPoints3D[i_temp + 2].Z = arrPoints3D[1].Z;

            arrPoints3D[i_temp + 3].X = arrPoints3D[7].X - fx_temp2;
            arrPoints3D[i_temp + 3].Y = arrPoints3D[7].Y + fy_temp2;
            arrPoints3D[i_temp + 3].Z = Fl_Z;

            arrPoints3D[i_temp + 4].X = arrPoints3D[8].X - fx_temp2;
            arrPoints3D[i_temp + 4].Y = arrPoints3D[8].Y + fy_temp2;
            arrPoints3D[i_temp + 4].Z = Fl_Z;

            arrPoints3D[i_temp + 5].X = arrPoints3D[i_temp + 4].X;
            arrPoints3D[i_temp + 5].Y = arrPoints3D[i_temp + 4].Y;
            arrPoints3D[i_temp + 5].Z = arrPoints3D[1].Z;

            arrPoints3D[i_temp + 6].X = arrPoints3D[12].X;
            arrPoints3D[i_temp + 6].Y = arrPoints3D[12].Y;
            arrPoints3D[i_temp + 6].Z = arrPoints3D[1].Z;

            arrPoints3D[i_temp + 7].X = arrPoints3D[13].X;
            arrPoints3D[i_temp + 7].Y = arrPoints3D[i_temp + 5].Y;
            arrPoints3D[i_temp + 7].Z = arrPoints3D[i_temp + 5].Z;

            arrPoints3D[i_temp + 8].X = arrPoints3D[i_temp + 7].X;
            arrPoints3D[i_temp + 8].Y = arrPoints3D[i_temp + 4].Y;
            arrPoints3D[i_temp + 8].Z = arrPoints3D[i_temp + 4].Z;
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 13;
            Dimensions = new CDimension[iNumberOfDimensions + 1];

            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            // Bottom
            Dimensions[0] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, PointsOut2D[0].Y), PointsOut2D[0], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], false, true);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[3], new Point(PointsOut2D[4].X, PointsOut2D[3].Y), false, true);

            // Left side
            Dimensions[5] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[7], false, true, 80);

            // Right side
            Dimensions[6] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[4].X, PointsOut2D[3].Y), PointsOut2D[4], true, true);
            Dimensions[7] = new CDimensionLinear(plateCenter, PointsOut2D[4], new Point(PointsOut2D[4].X, PointsOut2D[6].Y), true, true);

            // Top
            Dimensions[8] = new CDimensionLinear(plateCenter, PointsOut2D[6], new Point(PointsOut2D[4].X, PointsOut2D[6].Y), true, true);
            Dimensions[9] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, PointsOut2D[6].Y), PointsOut2D[6], true, true);

            // Top Input Width
            Dimensions[10] = new CDimensionLinear(plateCenter, PointsOut2D[7], PointsOut2D[6], true, true, 70);

            // Overall
            Dimensions[11] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, PointsOut2D[0].Y), new Point(PointsOut2D[4].X, PointsOut2D[0].Y), false, true, 50);
            Dimensions[12] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[4].X, PointsOut2D[3].Y), new Point(PointsOut2D[4].X, PointsOut2D[6].Y), true, true, 50);

            // Slope
            Dimensions[13] = new CDimensionArc(plateCenter, new Point(PointsOut2D[6].X, PointsOut2D[7].Y), PointsOut2D[6], PointsOut2D[7]);
        }

        public override void Set_MemberOutlinePoints2D()
        {
            int iNumberOfLines = 3;
            MemberOutlines = new CLine2D[iNumberOfLines];

            // Skratenie pruta v smere pruta (5 mm)
            float fcut = 0.005f;
            // Dlzka prepony trojuholnika vratane skratenia (od bodu 0 az po stred plechu)
            float fb1 = 0.5f * m_fbX1;
            float fc1 = fb1 / (float)Math.Cos(FSlope_rad);
            // Skratenie prepony c1 o fcut
            float fc1_cut = fc1 - fcut;
            // Urcenie suradnic koncoveho bodu prepony
            float fx1 = m_fLipBase_dim_x + fc1_cut * (float)Math.Cos(FSlope_rad);
            float fy1 = fc1_cut * (float)Math.Sin(FSlope_rad);

            // Urcenie suradnic bodu na hornej hrane plechu
            float fdepth = m_fd_crsc; // Vyska prierezu sa preberie z GUI // m_fhY1 * (float)Math.Cos(FSlope_rad);

            float fx2 = fx1 - fdepth * (float)Math.Sin(FSlope_rad);
            float fy2 = fy1 + fdepth * (float)Math.Cos(FSlope_rad);

            // Body su nezavisle na bodoch outline aj ked maju rovnake suradnice
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[1].X, PointsOut2D[1].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));
            MemberOutlines[2] = new CLine2D(new Point(fx2, fy2), new Point(PointsOut2D[7].X, PointsOut2D[7].Y));

            MemberOutlines = AddMirroredLinesAboutY(0.5f * m_fbX1_AndLips, MemberOutlines);
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 2;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[1], PointsOut2D[7]);
            BendLines[1] = new CLine2D(PointsOut2D[2], PointsOut2D[5]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Back Side
            AddPenthagonIndices_CCW_12345(TriangleIndices, 2, 13, 12, 11, 4);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 14, 13, 3);
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 11, 10,5);

            // Front Side
            AddPenthagonIndices_CW_12345(TriangleIndices, 18, 24, 23, 22, 19);

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 17, 25, 16);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7,8,21, 20);

            // Shell Surface
            AddPenthagonIndices_CCW_12345(TriangleIndices, 0, 16,14,2,1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 17, 18, 24, 25);

            AddPenthagonIndices_CCW_12345(TriangleIndices, 5, 10, 8,7,6);
            AddRectangleIndices_CCW_1234(TriangleIndices, 19, 20, 21, 22);

            // Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 18, 17);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 3, 18);

            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 19, 18);

            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 6, 19);
            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 20, 19);

            // Top
            AddRectangleIndices_CCW_1234(TriangleIndices, 15, 16, 25, 24);
            AddRectangleIndices_CCW_1234(TriangleIndices, 13, 14, 15, 24);
            AddRectangleIndices_CCW_1234(TriangleIndices, 13, 24, 23, 12);

            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 23, 22, 11);
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 10, 11, 22);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 22, 21);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            for (int i = 0; i < 17; i++)
            {
                if (i < (17) - 1)
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

            for (int i = 0; i < 9; i++)
            {
                if (i < (9) - 1)
                {
                    pi = arrPoints3D[17 + i];
                    pj = arrPoints3D[17 + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[17 + i];
                    pj = arrPoints3D[17];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Front
            wireFrame.Points.Add(arrPoints3D[18]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[19]);
            wireFrame.Points.Add(arrPoints3D[22]);

            // Back
            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[10]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[25]);

             wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[23]);

            return wireFrame;
        }
    }
}
