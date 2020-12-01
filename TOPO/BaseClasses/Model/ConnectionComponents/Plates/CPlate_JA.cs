﻿using _3DTools;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_JA : CPlate_Frame
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

        public CConCom_Plate_JA()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;
        }

        public CConCom_Plate_JA(string sName_temp,
            Point3D controlpoint,
            float fb_temp,
            float fh_1_temp,
            float fh_2_temp,
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

            ITotNoPointsin2D = 5;
            ITotNoPointsin3D = 10;

            m_pControlPoint = controlpoint;
            m_fbX = fb_temp;
            m_fhY1 = fh_1_temp;
            m_fhY2 = fh_2_temp;
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
                m_fSlope_rad = (float)Math.Atan((Fh_Y2 - Fh_Y1) / (0.5 * Fb_X));
            else
                Fh_Y2 = Fh_Y1 + ((float)Math.Tan(m_fSlope_rad) * (0.5f * Fb_X));

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
                screwArrangement.Calc_ApexPlateData(0, m_fbX, 0, m_fhY1, Ft, m_fSlope_rad, ScrewInPlusZDirection);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = m_fbX;
            Height_hy = Math.Max(m_fhY1, m_fhY2);
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = m_fbX;
            Height_hy_Stretched = Math.Max(m_fhY1, m_fhY2);
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

            fA_g = Get_A_rect(Ft, m_fhY1);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;
            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fhY1);

            fA_vn_zv = fA_v_zv;
            if (screwArrangement != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fhY1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY1); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX;
            PointsOut2D[1].Y = PointsOut2D[0].Y;

            PointsOut2D[2].X = PointsOut2D[1].X;
            PointsOut2D[2].Y = m_fhY1;

            PointsOut2D[3].X = 0.5f * m_fbX;
            PointsOut2D[3].Y = m_fhY2;

            PointsOut2D[4].X = PointsOut2D[0].X;
            PointsOut2D[4].Y = PointsOut2D[2].Y;
        }

        public override void Calc_Coord3D()
        {
            for (int i = 0; i < 2; i++) // 2 cycles - front and back surface
            {
                // One Side
                for (int j = 0; j < ITotNoPointsin2D; j++)
                {
                    arrPoints3D[(i * ITotNoPointsin2D) + j].X = PointsOut2D[j].X;
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Y = PointsOut2D[j].Y;
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Z = i * Ft;
                }
            }
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 3;
            Dimensions = new CDimension[iNumberOfDimensions+1];

            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], true, true);

            Dimensions[3] = new CDimensionArc(plateCenter, new Point(PointsOut2D[3].X, PointsOut2D[2].Y), PointsOut2D[3], PointsOut2D[4]);
        }

        public override void Set_MemberOutlinePoints2D()
        {
            int iNumberOfLines = 2;
            MemberOutlines = new CLine2D[iNumberOfLines];

            // TODO - refaktorovat pre plechy JA a JB

            // Skratenie pruta v smere pruta (5 mm)
            float fcut = 0.005f;
            // Dlzka prepony trojuholnika vratane skratenia (od bodu 0 az po stred plechu)
            float fb1 = 0.5f * Fb_X;
            float fc1 = fb1 / (float)Math.Cos(FSlope_rad);
            // Skratenie prepony c1 o fcut
            float fc1_cut = fc1 - fcut;
            // Urcenie suradnic koncoveho bodu prepony
            float fx1 = fc1_cut * (float)Math.Cos(FSlope_rad);
            float fy1 = fc1_cut * (float)Math.Sin(FSlope_rad);

            // Urcenie suradnic bodu na hornej hrane plechu
            float fdepth = Fh_Y1 * (float)Math.Cos(FSlope_rad);

            float fx2 = fx1 - fdepth * (float)Math.Sin(FSlope_rad);
            float fy2 = fy1 + fdepth * (float)Math.Cos(FSlope_rad);

            // Body su nezavisle na bodoch outline aj ked maju rovnake suradnice
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[0].X, PointsOut2D[0].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));

            MemberOutlines = AddMirroredLinesAboutY(0.5f * Fb_X, MemberOutlines);
        }

        protected override void loadIndices()
        {
            int secNum = 5;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddPenthagonIndices_CW_12345(TriangleIndices, 0, 1, 2, 3, 4);

            // Back Side
            AddPenthagonIndices_CCW_12345(TriangleIndices, 5, 6, 7, 8, 9);

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
                pi = arrPoints3D[i];
                pj = arrPoints3D[ITotNoPointsin2D + i];

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            return wireFrame;
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA refPlate = (CConCom_Plate_JA)plate;
                this.m_fbX = refPlate.m_fbX;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fhY2 = refPlate.m_fhY2;
                this.m_fSlope_rad = refPlate.m_fSlope_rad;                
                //this.m_bScrewInPlusZDirection = refPlate.m_bScrewInPlusZDirection;  //toto kopirovat nechceme
            }
        }
    }
}
