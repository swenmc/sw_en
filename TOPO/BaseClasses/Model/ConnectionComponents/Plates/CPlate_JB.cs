﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using _3DTools;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_JB : CPlate_Frame
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

        public CConCom_Plate_JB()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;
        }

        public CConCom_Plate_JB(string sName_temp,
            Point3D controlpoint,
            float fb_temp,
            float fh_1_temp,
            float fh_2_temp,
            float fL_temp,
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

            ITotNoPointsin2D = 12;
            ITotNoPointsin3D = 26;

            m_pControlPoint = controlpoint;
            m_fbX = fb_temp;
            m_fhY1 = fh_1_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fL_temp;
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
                screwArrangement.Calc_ApexPlateData(0, m_fbX, m_flZ, m_fhY1, Ft, m_fSlope_rad, m_bScrewInPlusZDirection);
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
            Width_bx = m_fbX;
            Height_hy = Math.Max(m_fhY1, m_fhY2);
            SetFlatedPlateDimensions();
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

            fA_g = Get_A_channel(m_flZ, Ft, Ft, m_fhY1);
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

            fI_yu = Get_I_yu_channel(m_flZ, Ft, Ft, m_fhY1);  // TODO - bug - v strede je horna pasnica rozdelena, takze to musi byt tvar L, nie U // Nesymetricky prierez// Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY1); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            float fx_temp = m_flZ * (float)Math.Sin(m_fSlope_rad);
            float fy_temp = m_flZ * (float)Math.Cos(m_fSlope_rad);
            float fx_temp2 = Ft * (float)Math.Sin(m_fSlope_rad);

            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX;
            PointsOut2D[1].Y = PointsOut2D[0].Y;

            PointsOut2D[2].X = PointsOut2D[1].X;
            PointsOut2D[2].Y = m_flZ;

            PointsOut2D[3].X = PointsOut2D[1].X;
            PointsOut2D[3].Y = PointsOut2D[2].Y + m_fhY1;

            PointsOut2D[4].X = PointsOut2D[3].X + fx_temp;
            PointsOut2D[4].Y = PointsOut2D[3].Y + fy_temp;

            PointsOut2D[5].X = 0.5f * m_fbX + fx_temp2 + fx_temp;
            PointsOut2D[5].Y = m_flZ + m_fhY2 + fy_temp;

            PointsOut2D[6].X = 0.5f * m_fbX + fx_temp2;
            PointsOut2D[6].Y = m_flZ + m_fhY2;

            PointsOut2D[7].X = 0.5f * m_fbX - fx_temp2;
            PointsOut2D[7].Y = PointsOut2D[6].Y;

            PointsOut2D[8].X = PointsOut2D[7].X - fx_temp2 - fx_temp;
            PointsOut2D[8].Y = PointsOut2D[5].Y;

            PointsOut2D[9].X = PointsOut2D[0].X - fx_temp;
            PointsOut2D[9].Y = PointsOut2D[4].Y;

            PointsOut2D[10].X = PointsOut2D[0].X;
            PointsOut2D[10].Y = PointsOut2D[3].Y;

            PointsOut2D[11].X = PointsOut2D[0].X;
            PointsOut2D[11].Y = PointsOut2D[2].Y;
        }

        public override void Calc_Coord3D()
        {
            float fx_temp = m_flZ * (float)Math.Sin(m_fSlope_rad);
            float fy_temp = m_flZ * (float)Math.Cos(m_fSlope_rad);
            float fx_temp2 = Ft * (float)Math.Sin(m_fSlope_rad);
            float fy_temp2 = Ft * (float)Math.Cos(m_fSlope_rad);

            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = m_fbX;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = arrPoints3D[0].Z;

            arrPoints3D[2].X = arrPoints3D[1].X;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = m_fhY1 + Ft + fy_temp2;
            arrPoints3D[3].Z = arrPoints3D[2].Z;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = arrPoints3D[3].Y;
            arrPoints3D[4].Z = arrPoints3D[1].Z;

            arrPoints3D[5].X = 0.5f * m_fbX + 2 * fx_temp2;
            arrPoints3D[5].Y = m_fhY2 + Ft + fy_temp2;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = arrPoints3D[5].X;
            arrPoints3D[6].Y = arrPoints3D[5].Y;
            arrPoints3D[6].Z = 0;

            arrPoints3D[7].X = 0.5f * m_fbX + fx_temp2;
            arrPoints3D[7].Y = m_fhY2 + 2 * Ft;
            arrPoints3D[7].Z = arrPoints3D[2].Z;

            arrPoints3D[8].X = 0.5f * m_fbX - fx_temp2;
            arrPoints3D[8].Y = arrPoints3D[7].Y;
            arrPoints3D[8].Z = arrPoints3D[7].Z;

            arrPoints3D[9].X = 0.5f * m_fbX - 2 * fx_temp2;
            arrPoints3D[9].Y = arrPoints3D[6].Y;
            arrPoints3D[9].Z = arrPoints3D[2].Z;

            arrPoints3D[10].X = arrPoints3D[9].X;
            arrPoints3D[10].Y = arrPoints3D[9].Y;
            arrPoints3D[10].Z = arrPoints3D[5].Z;

            arrPoints3D[11].X = arrPoints3D[0].X;
            arrPoints3D[11].Y = arrPoints3D[4].Y;
            arrPoints3D[11].Z = arrPoints3D[4].Z;

            arrPoints3D[12].X = arrPoints3D[0].X;
            arrPoints3D[12].Y = arrPoints3D[3].Y;
            arrPoints3D[12].Z = arrPoints3D[3].Z;

            arrPoints3D[13].X = arrPoints3D[0].X;
            arrPoints3D[13].Y = arrPoints3D[0].Y;
            arrPoints3D[13].Z = arrPoints3D[2].Z;

            int i_temp = 14;  // Number of point in first layer

            // Second layer
            arrPoints3D[i_temp + 0].X = 0;
            arrPoints3D[i_temp + 0].Y = Ft;
            arrPoints3D[i_temp + 0].Z = m_flZ;

            arrPoints3D[i_temp + 1].X = m_fbX;
            arrPoints3D[i_temp + 1].Y = Ft;
            arrPoints3D[i_temp + 1].Z = arrPoints3D[0].Z;

            arrPoints3D[i_temp + 2].X = arrPoints3D[1].X;
            arrPoints3D[i_temp + 2].Y = Ft;
            arrPoints3D[i_temp + 2].Z = Ft;

            arrPoints3D[i_temp + 3].X = arrPoints3D[2].X;
            arrPoints3D[i_temp + 3].Y = m_fhY1 + Ft;
            arrPoints3D[i_temp + 3].Z = Ft;

            arrPoints3D[i_temp + 4].X = arrPoints3D[1].X;
            arrPoints3D[i_temp + 4].Y = arrPoints3D[17].Y;
            arrPoints3D[i_temp + 4].Z = arrPoints3D[1].Z;

            arrPoints3D[i_temp + 5].X = 0.5f * m_fbX + fx_temp2;
            arrPoints3D[i_temp + 5].Y = m_fhY2 + Ft;
            arrPoints3D[i_temp + 5].Z = arrPoints3D[0].Z;

            arrPoints3D[i_temp + 6].X = arrPoints3D[19].X;
            arrPoints3D[i_temp + 6].Y = arrPoints3D[19].Y;
            arrPoints3D[i_temp + 6].Z = Ft;

            arrPoints3D[i_temp + 7].X = 0.5f * m_fbX - fx_temp2;
            arrPoints3D[i_temp + 7].Y = m_fhY2 + Ft;
            arrPoints3D[i_temp + 7].Z = arrPoints3D[20].Z;

            arrPoints3D[i_temp + 8].X = arrPoints3D[21].X;
            arrPoints3D[i_temp + 8].Y = arrPoints3D[21].Y;
            arrPoints3D[i_temp + 8].Z = arrPoints3D[19].Z;

            arrPoints3D[i_temp + 9].X = arrPoints3D[12].X;
            arrPoints3D[i_temp + 9].Y = arrPoints3D[18].Y;
            arrPoints3D[i_temp + 9].Z = arrPoints3D[18].Z;

            arrPoints3D[i_temp + 10].X = arrPoints3D[23].X;
            arrPoints3D[i_temp + 10].Y = arrPoints3D[17].Y;
            arrPoints3D[i_temp + 10].Z = arrPoints3D[17].Z;

            arrPoints3D[i_temp + 11].X = arrPoints3D[14].X;
            arrPoints3D[i_temp + 11].Y = arrPoints3D[14].Y;
            arrPoints3D[i_temp + 11].Z = arrPoints3D[16].Z;
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 5;
            Dimensions = new CDimension[iNumberOfDimensions + 1];

            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], true, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], true, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], true, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], true, true, 40);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[4], PointsOut2D[5], true, true);

            Dimensions[5] = new CDimensionArc(plateCenter, new Point(PointsOut2D[7].X, PointsOut2D[3].Y), PointsOut2D[7], PointsOut2D[10]);
        }

        public override void Set_MemberOutlinePoints2D()
        {
            int iNumberOfLines = 3;
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
            float fy1 = Fl_Z + fc1_cut * (float)Math.Sin(FSlope_rad);

            // Urcenie suradnic bodu na hornej hrane plechu
            float fdepth = Fh_Y1 * (float)Math.Cos(FSlope_rad);

            float fx2 = fx1 - fdepth * (float)Math.Sin(FSlope_rad);
            float fy2 = fy1 + fdepth * (float)Math.Cos(FSlope_rad);

            // Body su nezavisle na bodoch outline aj ked maju rovnake suradnice
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[11].X, PointsOut2D[11].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));
            MemberOutlines[2] = new CLine2D(new Point(fx2, fy2), new Point(PointsOut2D[10].X, PointsOut2D[10].Y));

            MemberOutlines = AddMirroredLinesAboutY(0.5f * Fb_X, MemberOutlines);
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 3;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[2], PointsOut2D[11]);
            BendLines[1] = new CLine2D(PointsOut2D[3], PointsOut2D[6]);
            BendLines[2] = new CLine2D(PointsOut2D[7], PointsOut2D[10]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Back Side
            AddHexagonIndices_CCW_123456(TriangleIndices, 2, 13, 12, 8, 7, 3);
            AddTriangleIndices_CCW_123(TriangleIndices, 12, 9, 8);
            AddTriangleIndices_CCW_123(TriangleIndices, 3, 7, 6);

            // Front Side
            AddHexagonIndices_CW_123456(TriangleIndices, 16,25,24,21,20,17);

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 15, 14);
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 19, 18);
            AddRectangleIndices_CCW_1234(TriangleIndices, 10, 11, 23, 22);

            // Shell Surface
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 16, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 17, 16);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 18, 17);

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 14, 25, 13);
            AddRectangleIndices_CCW_1234(TriangleIndices, 13, 25, 24, 12);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 12, 24, 23);

            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 10, 22);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7, 19, 5, 6);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7, 8, 21, 20);

            // Top / Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 6, 5, 4);
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 12, 11, 10);

            AddRectangleIndices_CCW_1234(TriangleIndices, 17, 18, 19, 20);
            AddRectangleIndices_CCW_1234(TriangleIndices, 21, 22, 23, 24);

            AddRectangleIndices_CCW_1234(TriangleIndices, 14, 15, 16, 25);
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 13, 2, 1);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            for (int i = 0; i < 14; i++)
            {
                if (i < (14) - 1)
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

            for (int i = 0; i < 12; i++)
            {
                if (i < (12) - 1)
                {
                    pi = arrPoints3D[14 + i];
                    pj = arrPoints3D[14 + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[14 + i];
                    pj = arrPoints3D[14];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Front
            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[17]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[21]);
            wireFrame.Points.Add(arrPoints3D[24]);

            // Back
            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[12]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[21]);

            return wireFrame;
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_JB)
            {
                //ak by bolo viac parametrov tak takto
                CConCom_Plate_JB refPlate = (CConCom_Plate_JB)plate;
                this.m_fbX = refPlate.m_fbX;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fhY2 = refPlate.m_fhY2;
                this.m_flZ = refPlate.m_flZ;
                this.m_fSlope_rad = refPlate.m_fSlope_rad;                
                //this.m_bScrewInPlusZDirection = refPlate.m_bScrewInPlusZDirection;  //toto kopirovat nechceme
            }
        }
    }
}
