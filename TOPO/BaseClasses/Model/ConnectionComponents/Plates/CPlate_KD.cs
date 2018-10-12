﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public class CConCom_Plate_KD : CPlate
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

        public CConCom_Plate_KD()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KD(string sName_temp,
            CPoint controlpoint,
            float fb_1_temp,
            float fh_1_temp,
            float fb_2_temp,
            float fh_2_temp,
            float fl_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement screwArrangement,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 8;
            INoPoints2Dfor3D = 10;
            ITotNoPointsin3D = 19;

            m_pControlPoint = controlpoint;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fl_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

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
                // Parameter flZ - // Distance from the left edge is used for KC and KD plates)
                screwArrangement.Calc_KneePlateData(m_fbX1, m_fbX2, m_flZ, m_fhY1, Ft, m_fSlope_rad);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_channel(m_flZ, Ft, Ft, m_fbX1);
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

            fI_yu = Get_I_yu_channel(m_flZ, Ft, Ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp = m_flZ * (float)Math.Cos(fBeta);
            float fy_temp = m_flZ * (float)Math.Sin(fBeta);

            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_flZ;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = m_flZ + m_fbX1;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = m_flZ + m_fbX1 + fx_temp;
            PointsOut2D[3].Y = -fy_temp;

            PointsOut2D[4].X = m_flZ + m_fbX2 + fx_temp;
            PointsOut2D[4].Y = m_fhY2 - fy_temp;

            PointsOut2D[5].X = m_flZ + m_fbX2;
            PointsOut2D[5].Y = m_fhY2;

            PointsOut2D[6].X = m_flZ;
            PointsOut2D[6].Y = m_fhY1;

            PointsOut2D[7].X = 0;
            PointsOut2D[7].Y = m_fhY1;
        }

        void Calc_Coord3D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp2 = Ft * (float)Math.Cos(fBeta);
            float fy_temp2 = Ft * (float)Math.Sin(fBeta);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX1;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = m_fbX1 + fx_temp2;
            arrPoints3D[3].Y = -fy_temp2;
            arrPoints3D[3].Z = arrPoints3D[1].Z;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = arrPoints3D[3].Y;
            arrPoints3D[4].Z = m_flZ;

            arrPoints3D[5].X = m_fbX2 + fx_temp2;
            arrPoints3D[5].Y = m_fhY2 - fy_temp2;
            arrPoints3D[5].Z = arrPoints3D[4].Z;

            arrPoints3D[6].X = arrPoints3D[5].X;
            arrPoints3D[6].Y = arrPoints3D[5].Y;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = m_fbX2;
            arrPoints3D[7].Y = m_fhY2;
            arrPoints3D[7].Z = arrPoints3D[1].Z;

            arrPoints3D[8].X = Ft;
            arrPoints3D[8].Y = m_fhY1;
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            arrPoints3D[9].X = arrPoints3D[1].X;
            arrPoints3D[9].Y = arrPoints3D[8].Y;
            arrPoints3D[9].Z = arrPoints3D[1].Z;

            arrPoints3D[10].X = arrPoints3D[1].X;
            arrPoints3D[10].Y = arrPoints3D[9].Y;
            arrPoints3D[10].Z = arrPoints3D[0].Z;

            // Second layer
            // INoPoints2Dfor3D = 10
            arrPoints3D[INoPoints2Dfor3D + 1].X = arrPoints3D[8].X;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = arrPoints3D[0].Y;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 3].X = arrPoints3D[2].X;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = arrPoints3D[2].Y;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = m_flZ;

            arrPoints3D[INoPoints2Dfor3D + 5].X = arrPoints3D[7].X;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = arrPoints3D[7].Y;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[INoPoints2Dfor3D + 4].Z;

            arrPoints3D[INoPoints2Dfor3D + 6].X = arrPoints3D[7].X;
            arrPoints3D[INoPoints2Dfor3D + 6].Y = arrPoints3D[7].Y;
            arrPoints3D[INoPoints2Dfor3D + 6].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 7].X = arrPoints3D[8].X;
            arrPoints3D[INoPoints2Dfor3D + 7].Y = arrPoints3D[8].Y;
            arrPoints3D[INoPoints2Dfor3D + 7].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 8].X = arrPoints3D[INoPoints2Dfor3D + 7].X;
            arrPoints3D[INoPoints2Dfor3D + 8].Y = arrPoints3D[INoPoints2Dfor3D + 7].Y;
            arrPoints3D[INoPoints2Dfor3D + 8].Z = arrPoints3D[11].Z;
        }

        void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 6;
            Dimensions = new CDimension[iNumberOfDimensions + 1];
            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1]);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2]);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], true, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[5], PointsOut2D[4], true, false);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[6], PointsOut2D[5], true, false);
            Dimensions[5] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[7], true, false);

            Dimensions[6] = new CDimensionArc(new Point(PointsOut2D[2].X, PointsOut2D[6].Y), PointsOut2D[5], PointsOut2D[6]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 13, 16, 17);
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 11, 18, 10);
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 15, 14);

            // Back Side
            AddPenthagonIndices_CCW_12345(TriangleIndices, 1, 9, 8, 7, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 7, 6, 3);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 18, 10, 9, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 16, 17, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 5, 15, 7, 6);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 11, 12, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 12, 13, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 14, 4, 3);

            // Side Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 9, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 18, 17, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 16, 15, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 5, 6);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // BackSide
            for (int i = 0; i < INoPoints2Dfor3D + 1; i++)
            {
                if (i < 9)
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
            for (int i = 0; i < INoPoints2Dfor3D - 2; i++)
            {
                if (i < INoPoints2Dfor3D - 3)
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
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[13]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[9]);

            return wireFrame;
        }
    }
}
