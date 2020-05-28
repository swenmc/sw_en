using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using BaseClasses.GraphObj;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_KHS : CPlate_Frame
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

        public CConCom_Plate_KHS()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
        }

        public CConCom_Plate_KHS(string sName_temp,
            Point3D controlpoint,
            float fb_1_temp,
            float fh_1_temp,
            float fb_2_temp,
            float fh_2_temp,
            float fl_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            bool bScrewInPlusZDirection,
            CScrewArrangement screwArrangement)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;

            ITotNoPointsin2D = 11;
            INoPoints2Dfor3D = 10;
            ITotNoPointsin3D = 23;

            m_pControlPoint = controlpoint;
            Fb_X1 = fb_1_temp;
            Fh_Y1 = fh_1_temp;
            Fb_X2 = fb_2_temp;
            Fh_Y2 = fh_2_temp;
            Fl_Z = fl_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;
            m_bScrewInPlusZDirection = bScrewInPlusZDirection;

            UpdatePlateData(screwArrangement);
        }

        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            if (MathF.d_equal(m_fSlope_rad, 0))
                m_fSlope_rad = (float)Math.Atan((Fh_Y2 - Fh_Y1) / Fb_X2);
            else
                Fh_Y2 = Fh_Y1 + ((float)Math.Tan(m_fSlope_rad) * Fb_X2);

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
                // Parameter flZ - // Distance from the left edge is the same as KC or KD plate (lz is used for KC and KD plates)
                screwArrangement.Calc_KneePlateData(m_fbX1, m_fbX2, m_flZ, m_fhY1, Ft, m_fSlope_rad, m_bScrewInPlusZDirection);
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
            Width_bx = Math.Max(m_fbX1, m_fbX2);
            Height_hy = Math.Max(m_fhY1, m_fhY2);
            SetFlatedPlateDimensions();
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

            // Konzervativne, vynechana pasnica
            fA_g = Get_A_rect(Ft, m_fbX1);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;

            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fbX1);

            fA_vn_zv = fA_v_zv;

            if (screwArrangement != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        public override void SetMinimumScrewToEdgeDistances(CScrewArrangement screwArrangement)
        {
            SetMinimumScrewToEdgeDistances_Basic(screwArrangement);

            e_min_x -= m_flZ; // Odpocitame sirku laveho ohybu
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            float fBeta = (float)Math.Atan((Fb_X2 - Fb_X1) / Fh_Y2);
            float fx_temp = Fl_Z * (float)Math.Cos(fBeta);
            float fy_temp = Fl_Z * (float)Math.Sin(fBeta);

            float fy_temp2 = Fl_Z * (float)Math.Tan(FSlope_rad);

            float fx_temp3 = Fl_Z / (float)Math.Cos(fBeta);

            // Falling knee
            float fx_temp4 = Fl_Z * (float)Math.Cos(-FSlope_rad);
            float fy_temp4 = Fl_Z * (float)Math.Sin(-FSlope_rad);

            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = Fl_Z;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = Fl_Z + Fb_X1;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = Fl_Z + Fb_X1 + fx_temp3;
            PointsOut2D[3].Y = 0;

            float fc1_temp =  Fb_X1 / (float)Math.Cos(FSlope_rad + fBeta);

            PointsOut2D[5].X = (Fl_Z + Fb_X2) - fc1_temp * (float)Math.Sin(fBeta);
            PointsOut2D[5].Y = Fh_Y2 - fc1_temp * (float)Math.Cos(fBeta);

            float fc3_temp = Fh_Y2 / (float)Math.Cos(fBeta);
            float fc2_temp = fc3_temp - fc1_temp;
            float fc4_temp = fc2_temp - fy_temp2;

            PointsOut2D[4].X = PointsOut2D[5].X + fx_temp;
            PointsOut2D[4].Y = PointsOut2D[5].Y - fy_temp;

            float fc5_temp = Fl_Z * (float)Math.Tan(FSlope_rad + fBeta);

            float fx_temp5 = fc5_temp * (float)Math.Sin(fBeta);
            float fy_temp5 = fc5_temp * (float)Math.Cos(fBeta);

            PointsOut2D[6].X = PointsOut2D[4].X + (FSlope_rad > 0 ? fx_temp5 : 0);
            PointsOut2D[6].Y = PointsOut2D[4].Y + (FSlope_rad > 0 ? fy_temp5 : 0);

            PointsOut2D[8].X = Fl_Z + Fb_X2;
            PointsOut2D[8].Y = Fh_Y2;

            float fc6_temp = Fl_Z / (float)Math.Cos(FSlope_rad + fBeta);

            float fx_temp6 = fc6_temp * (float)Math.Cos(FSlope_rad);
            float fy_temp6 = fc6_temp * (float)Math.Sin(FSlope_rad);

            PointsOut2D[7].X = Fl_Z + Fb_X2 + (FSlope_rad > 0 ? fx_temp6 : fx_temp4);
            PointsOut2D[7].Y = Fh_Y2 + (FSlope_rad > 0 ? fy_temp6 : - fy_temp4);

            PointsOut2D[9].X = Fl_Z;
            PointsOut2D[9].Y = Fh_Y1;

            PointsOut2D[10].X = 0;
            PointsOut2D[10].Y = Fh_Y1 - (FSlope_rad > 0 ? fy_temp2 : -fy_temp4);
        }

        public override void Calc_Coord3D()
        {
            /*
            float fBeta = (float)Math.Atan((Fb_X2 - Fb_X1) / Fh_Y2);

            float fx_temp = Fl_Z * (float)Math.Cos(fBeta);
            float fy_temp = Fl_Z * (float)Math.Sin(fBeta);

            float fx_temp2 = Ft * (float)Math.Cos(fBeta);
            float fy_temp2 = Ft * (float)Math.Sin(fBeta);

            float fx_temp3 = Ft / (float)Math.Cos(fBeta);

            float fy_temp4 = Fl_Z * (float)Math.Tan(FSlope_rad);

            float fy_temp5 = Fl_Z * (float)Math.Tan(fBeta);

            float fx_temp8 = fy_temp4 * (float)Math.Sin(fBeta);
            float fy_temp8 = fy_temp4 * (float)Math.Cos(fBeta);

            float fx_temp9 = fy_temp5 * (float)Math.Sin(fBeta);
            float fy_temp9 = fy_temp5 * (float)Math.Cos(fBeta);

            // Falling knee
            float fx_temp42 = Fl_Z * (float)Math.Cos(-FSlope_rad);
            float fy_temp42 = Fl_Z * (float)Math.Sin(-FSlope_rad);

            float fc1_temp = Fb_X1 / (float)Math.Cos(FSlope_rad + fBeta);
            float fc3_temp = Fh_Y2 / (float)Math.Cos(fBeta);
            float fc2_temp = fc3_temp - fc1_temp;

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = -m_flZ;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX1;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X + fx_temp9;
            arrPoints3D[3].Y = arrPoints3D[2].Y + fy_temp9;
            arrPoints3D[3].Z = arrPoints3D[0].Z;

            arrPoints3D[5].X = (Fb_X2) - fc1_temp * (float)Math.Sin(fBeta);
            arrPoints3D[5].Y = Fh_Y2 - fc1_temp * (float)Math.Cos(fBeta);
            arrPoints3D[5].Z = 0;

            arrPoints3D[4].X = arrPoints3D[5].X;
            arrPoints3D[4].Y = arrPoints3D[5].Y;
            arrPoints3D[4].Z = arrPoints3D[0].Z;

            // Point [4] in 2D
            float fx4_2D = (float)arrPoints3D[5].X + fx_temp;
            float fy4_2D = (float)arrPoints3D[5].Y - fy_temp;

            float fc5_temp = Fl_Z * (float)Math.Tan(FSlope_rad + fBeta);

            float fx_temp52 = fc5_temp * (float)Math.Sin(fBeta);
            float fy_temp52 = fc5_temp * (float)Math.Cos(fBeta);

            arrPoints3D[6].X = fx4_2D + (FSlope_rad > 0 ? fx_temp52 : 0);
            arrPoints3D[6].Y = fy4_2D + (FSlope_rad > 0 ? fy_temp52 : 0);
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            float fc6_temp = Fl_Z / (float)Math.Cos(FSlope_rad + fBeta);

            float fx_temp6 = fc6_temp * (float)Math.Cos(FSlope_rad);
            float fy_temp6 = fc6_temp * (float)Math.Sin(FSlope_rad);

            arrPoints3D[7].X = Fb_X2 + (FSlope_rad > 0 ? fx_temp6 : fx_temp42);
            arrPoints3D[7].Y = Fh_Y2 + (FSlope_rad > 0 ? fy_temp6 : -fy_temp42);
            arrPoints3D[7].Z = arrPoints3D[1].Z;

            arrPoints3D[8].X = -Fl_Z;
            arrPoints3D[8].Y = Fh_Y1 - (FSlope_rad > 0 ? fy_temp4 : - fy_temp42);
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            float fy_temp7 = Fb_X1 / (float)Math.Cos(FSlope_rad);

            arrPoints3D[11].X = 0;
            arrPoints3D[11].Y = Fh_Y1 - fy_temp7;
            arrPoints3D[11].Z = arrPoints3D[1].Z;

            arrPoints3D[10].X = -Ft;
            arrPoints3D[10].Y = arrPoints3D[11].Y;
            arrPoints3D[10].Z = arrPoints3D[1].Z;

            arrPoints3D[9].X = arrPoints3D[8].X;
            arrPoints3D[9].Y = PointsOut2D[11].Y + (FSlope_rad > 0 ? 0 : fy_temp42);
            arrPoints3D[9].Z = arrPoints3D[1].Z;

            arrPoints3D[12].X = 0;
            arrPoints3D[12].Y = arrPoints3D[10].Y;
            arrPoints3D[12].Z = arrPoints3D[0].Z;

            // Second layer
            // INoPoints2Dfor3D = 12
            arrPoints3D[INoPoints2Dfor3D + 1].X = -Ft;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = arrPoints3D[0].Y;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 3].X = arrPoints3D[2].X;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = arrPoints3D[2].Y;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = Ft;


            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X + fx_temp2;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = arrPoints3D[INoPoints2Dfor3D + 3].Z;

            arrPoints3D[INoPoints2Dfor3D + 5].X = arrPoints3D[2].X + fx_temp2;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = arrPoints3D[2].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[1].Z;

            arrPoints3D[INoPoints2Dfor3D + 6].X = arrPoints3D[3].X + fx_temp2; // Opravit
            arrPoints3D[INoPoints2Dfor3D + 6].Y = arrPoints3D[3].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 6].Z = arrPoints3D[0].Z;

            arrPoints3D[INoPoints2Dfor3D + 7].X = arrPoints3D[4].X + fx_temp2; // Opravit
            arrPoints3D[INoPoints2Dfor3D + 7].Y = arrPoints3D[4].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 7].Z = arrPoints3D[0].Z;

            arrPoints3D[INoPoints2Dfor3D + 8].X = arrPoints3D[5].X + fx_temp2;
            arrPoints3D[INoPoints2Dfor3D + 8].Y = arrPoints3D[5].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 8].Z = arrPoints3D[1].Z;

            arrPoints3D[INoPoints2Dfor3D + 9].X = arrPoints3D[INoPoints2Dfor3D + 8].X;
            arrPoints3D[INoPoints2Dfor3D + 9].Y = arrPoints3D[INoPoints2Dfor3D + 8].Y;
            arrPoints3D[INoPoints2Dfor3D + 9].Z = arrPoints3D[INoPoints2Dfor3D + 4].Z;

            arrPoints3D[INoPoints2Dfor3D + 10].X = arrPoints3D[5].X;
            arrPoints3D[INoPoints2Dfor3D + 10].Y = arrPoints3D[5].Y;
            arrPoints3D[INoPoints2Dfor3D + 10].Z = arrPoints3D[INoPoints2Dfor3D + 3].Z;

            arrPoints3D[INoPoints2Dfor3D + 11].X = arrPoints3D[6].X;
            arrPoints3D[INoPoints2Dfor3D + 11].Y = arrPoints3D[6].Y;
            arrPoints3D[INoPoints2Dfor3D + 11].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 12].X = arrPoints3D[7].X;
            arrPoints3D[INoPoints2Dfor3D + 12].Y = arrPoints3D[7].Y;
            arrPoints3D[INoPoints2Dfor3D + 12].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 13].X = arrPoints3D[8].X;
            arrPoints3D[INoPoints2Dfor3D + 13].Y = arrPoints3D[8].Y;
            arrPoints3D[INoPoints2Dfor3D + 13].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 14].X = arrPoints3D[9].X;
            arrPoints3D[INoPoints2Dfor3D + 14].Y = arrPoints3D[9].Y;
            arrPoints3D[INoPoints2Dfor3D + 14].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 15].X = arrPoints3D[10].X;
            arrPoints3D[INoPoints2Dfor3D + 15].Y = arrPoints3D[10].Y;
            arrPoints3D[INoPoints2Dfor3D + 15].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 16].X = arrPoints3D[10].X;
            arrPoints3D[INoPoints2Dfor3D + 16].Y = arrPoints3D[10].Y;
            arrPoints3D[INoPoints2Dfor3D + 16].Z = arrPoints3D[10].Z;

            arrPoints3D[INoPoints2Dfor3D + 17].X = arrPoints3D[13].X;
            arrPoints3D[INoPoints2Dfor3D + 17].Y = arrPoints3D[12].Y;
            arrPoints3D[INoPoints2Dfor3D + 17].Z = arrPoints3D[13].Z;
            */
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 16;
            Dimensions = new CDimension[iNumberOfDimensions + 1];
            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            // Bottom
            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], false, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[3], false, true, 53);

            // Top
            double yPosition = Math.Max(PointsOut2D[7].Y, PointsOut2D[10].Y);
            Dimensions[4] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[10].X, yPosition), new Point(PointsOut2D[7].X, yPosition),true, true, 53);
            Dimensions[5] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[10].X, yPosition), new Point(PointsOut2D[9].X, yPosition));
            Dimensions[6] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[9].X, yPosition), new Point(PointsOut2D[8].X, yPosition));
            Dimensions[7] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, yPosition), new Point(PointsOut2D[7].X, yPosition));

            // Vertical
            if (FSlope_rad > 0)
            {
                Dimensions[8] = new CDimensionLinear(plateCenter, PointsOut2D[10], new Point(PointsOut2D[10].X, PointsOut2D[7].Y), true, true);
                Dimensions[9] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[10], true, true);

                Dimensions[10] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], false, true);
                Dimensions[11] = new CDimensionLinear(plateCenter, PointsOut2D[4], PointsOut2D[6], false, true);
                Dimensions[12] = new CDimensionLinear(plateCenter, PointsOut2D[6], PointsOut2D[7], false, true);

                Dimensions[13] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, PointsOut2D[3].Y), PointsOut2D[8], false, true, 63);
                Dimensions[14] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[7].X, PointsOut2D[3].Y), PointsOut2D[7], false, true, 75);

                Dimensions[15] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[9], true, true, 95);
            }
            else
            {
                Dimensions[8] = new CDimensionLinear(plateCenter, PointsOut2D[11], PointsOut2D[10], true, true);
                Dimensions[9] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[10], true, true, 95);

                Dimensions[10] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], false, true);
                Dimensions[11] = new CDimensionLinear(plateCenter, PointsOut2D[6], PointsOut2D[7], false, true);
                Dimensions[13] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, PointsOut2D[3].Y), PointsOut2D[8], false, true, 63);
                Dimensions[14] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[7].X, PointsOut2D[3].Y), PointsOut2D[7], false, true, 75);

                Dimensions[14] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[9], true, true, 80);

                Dimensions[15] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[10], true, true, 95); // Kopia kvoli rovnakemu poctu kot, prerobit na iny pocet kot pre falling knee
            }

            Dimensions[16] = new CDimensionArc(plateCenter, new Point(PointsOut2D[2].X, PointsOut2D[9].Y), PointsOut2D[8], PointsOut2D[9]);
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 2;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[1], PointsOut2D[9]);
            BendLines[1] = new CLine2D(PointsOut2D[2], PointsOut2D[5]);
        }

        public override void Set_MemberOutlinePoints2D()
        {
            int iNumberOfLines = 2 + 1;
            MemberOutlines = new CLine2D[iNumberOfLines];

            // Skratenie pruta v smere pruta (5 mm)
            float fcut = 0.005f;

            float fdepth = Fb_X1; // ???

            float fx1 = Fl_Z + fdepth;
            float fy1 = fdepth - fcut;

            float fx2 = Fl_Z;
            float fy2 = fy1;

            float fy3 = fdepth * (float)Math.Cos(m_fSlope_rad);

            // Body su nezavisle na bodoch outline aj ked maju rovnake suradnice
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[2].X, PointsOut2D[2].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));
            MemberOutlines[2] = new CLine2D(new Point(PointsOut2D[9].X, PointsOut2D[9].Y - fy3), new Point(PointsOut2D[5].X, PointsOut2D[5].Y));
        }

        protected override void loadIndices()
        {
            /*
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 14, 27, 22, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 15, 22, 21, 16);
            AddHexagonIndices_CCW_123456(TriangleIndices, 22, 23, 24, 25, 26, 27);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 5, 11);
            AddHexagonIndices_CW_123456(TriangleIndices, 5, 6, 7, 8, 9, 11);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 24, 25, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 28, 29, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 5, 20, 21, 22);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 19, 20, 5);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 13, 14, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 14, 15, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 15, 16, 17);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 17, 18, 3);

            AddRectangleIndices_CW_1234(TriangleIndices, 9, 26, 27, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 5, 22, 23, 6);

            // Side Surface
            AddHexagonIndices_CW_123456(TriangleIndices, 16, 21, 20, 19, 18, 17);
            AddRectangleIndices_CW_1234(TriangleIndices, 3, 18, 19, 4);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 23, 24, 7);

            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 11, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 12, 29, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 29, 27, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 8, 25, 26, 9);
            */
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();
            /*
            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // BackSide
            for (int i = 0; i < INoPoints2Dfor3D + 1; i++)
            {
                if (i < INoPoints2Dfor3D)
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

            // FrontSide
            for (int i = 0; i < 17; i++)
            {
                if (i < 17 - 1)
                {
                    pi = arrPoints3D[INoPoints2Dfor3D + 1 + i];
                    pj = arrPoints3D[INoPoints2Dfor3D + 1 + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[INoPoints2Dfor3D + 1 + i];
                    pj = arrPoints3D[INoPoints2Dfor3D + 1 + 0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[17]);
 
            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[19]);

            //wireFrame.Points.Add(arrPoints3D[5]); // Nie je tam zlom, kedze je ohyb v pravom uhle
            //wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[26]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[27]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[28]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[29]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[27]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[21]);
*/
            return wireFrame;
            
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_KHS)
            {
                CConCom_Plate_KHS refPlate = (CConCom_Plate_KHS)plate;
                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fhY2 = refPlate.m_fhY2;
                this.m_flZ = refPlate.m_flZ;
                this.m_fSlope_rad = refPlate.m_fSlope_rad;
                this.m_bScrewInPlusZDirection = refPlate.m_bScrewInPlusZDirection;
                //this.pTip = refPlate.pTip;
            }
        }
    }
}
