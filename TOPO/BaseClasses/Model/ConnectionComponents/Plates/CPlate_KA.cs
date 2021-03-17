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
    public class CConCom_Plate_KA : CPlate_Frame
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

        public CConCom_Plate_KA()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
        }

        public CConCom_Plate_KA(string sName_temp,
            Point3D controlpoint,
            float fb_1_temp,
            float fh_1_temp,
            float fb_2_temp,
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
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;

            ITotNoPointsin2D = 4;
            ITotNoPointsin3D = 8;

            ControlPoint = controlpoint;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
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
            if (MathF.d_equal(FSlope_rad, 0))
                FSlope_rad = (float)Math.Atan((Fh_Y2 - Fh_Y1) / Fb_X2);
            else
                Fh_Y2 = Fh_Y1 + ((float)Math.Tan(FSlope_rad) * Fb_X2);

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
                screwArrangement.Calc_KneePlateData(m_fbX1, m_fbX2, 0, m_fhY1, Ft, FSlope_rad, ScrewInPlusZDirection);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = Math.Max(m_fbX1, m_fbX2);
            Height_hy = Math.Max(m_fhY1, m_fhY2);
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = Math.Max(m_fbX1, m_fbX2);
            Height_hy_Stretched = Math.Max(m_fhY1, m_fhY2);
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

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

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX1;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = m_fbX2;
            PointsOut2D[2].Y = m_fhY2;

            PointsOut2D[3].X = 0;
            PointsOut2D[3].Y = m_fhY1;
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
            int iNumberOfDimensions = 6;
            Dimensions = new CDimension[iNumberOfDimensions+1];
            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[2], true, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[3], true, true);

            Dimensions[4] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[2].X, PointsOut2D[1].Y), PointsOut2D[2], false, true, 50);
            Dimensions[5] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[3].X, Math.Max(PointsOut2D[3].Y, PointsOut2D[2].Y)), new Point(PointsOut2D[2].X, Math.Max(PointsOut2D[3].Y, PointsOut2D[2].Y)), true, true, 50);

            Dimensions[6] = new CDimensionArc(plateCenter, new Point(PointsOut2D[1].X, PointsOut2D[3].Y), PointsOut2D[2], PointsOut2D[3]);
        }

        public override void Set_MemberOutlinePoints2D()
        {
            int iNumberOfLines = 4;
            MemberOutlines = new CLine2D[iNumberOfLines];

            // TODO - refaktorovat pre plechy KA az KE

            // Skratenie pruta v smere pruta (5 mm)
            float fcut = 0.005f;

            float fdepth = Fb_X1; // ???

            float fx1 = fdepth;
            float fy1 = fdepth - fcut;

            float fx2 = 0;
            float fy2 = fy1;

            float fb1_y = (float)PointsOut2D[3].Y - fdepth * (float)Math.Cos(FSlope_rad); // Teoreticky bod, kde sa stretne rafter a column ak neuvazujeme skratenie prutov
            float fb1_x = fdepth * (float)Math.Sin(FSlope_rad);

            float faux_x = fcut * (float)Math.Cos(FSlope_rad);
            float faux_y = fcut * (float)Math.Sin(FSlope_rad);

            float fx3 = (float)PointsOut2D[3].X + faux_x; // Vlavo hore
            float fy3 = (float)PointsOut2D[3].Y + faux_y;

            float fx4 = fx3 + fdepth * (float)Math.Sin(FSlope_rad);
            float fy4 = fy3 - fdepth * (float)Math.Cos(FSlope_rad);

            if (FSlope_rad < 0) // Falling knee
            {
                float fxb3_temp = fdepth * (float)Math.Sin(-FSlope_rad);
                float fyb3_temp = fxb3_temp * (float)Math.Tan(-FSlope_rad);

                float faux = fxb3_temp / (float)Math.Cos(-FSlope_rad); // Dlzka odrezanej hrany vlavo hore

                faux_x = fcut * (float)Math.Cos(-FSlope_rad);
                faux_y = fcut * (float)Math.Sin(-FSlope_rad);

                fx3 = (float)PointsOut2D[3].X + fxb3_temp + faux_x; // Hore
                fy3 = (float)PointsOut2D[3].Y - fyb3_temp - faux_y;

                fx4 = fx3 - fdepth * (float)Math.Sin(-FSlope_rad); // Vlavo
                fy4 = fy3 - fdepth * (float)Math.Cos(-FSlope_rad);

                float fb4_x = (float)PointsOut2D[3].X;
                float fb4_y = (float)PointsOut2D[3].Y - fyb3_temp - fdepth * (float)Math.Cos(-FSlope_rad);

                fb1_x = (float)PointsOut2D[3].X + fdepth;
                fb1_y = fb4_y - fdepth * (float)Math.Tan(-FSlope_rad);

                fx1 = (float)PointsOut2D[3].X + fdepth;
                fy1 = fb1_y - fcut;

                fx2 = (float)PointsOut2D[3].X;
                fy2 = fy1;
            }

            bool considerCollinearOverlapAsIntersect = true;

            Vector2D intersection;

            Geom2D.LineSegementsIntersect(
                  new Vector2D(fb1_x, fb1_y),
                  new Vector2D(10, fb1_y + 10 * Math.Tan(FSlope_rad)),
                  new Vector2D(PointsOut2D[1].X, PointsOut2D[1].Y),
                  new Vector2D(PointsOut2D[2].X, PointsOut2D[2].Y),
                  out intersection,
                  considerCollinearOverlapAsIntersect);

            float fx5 = (float)intersection.X;
            float fy5 = (float)intersection.Y;

            // Body su nezavisle na bodoch outline aj ked maju rovnake suradnice
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[1].X, PointsOut2D[1].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));
            MemberOutlines[2] = new CLine2D(new Point(fx3, fy3), new Point(fx4, fy4));
            MemberOutlines[3] = new CLine2D(new Point(fx4, fy4), new Point(fx5, fy5));
        }

        protected override void loadIndices()
        {
            int secNum = 4;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 3);

            // Back Side
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 6, 7);

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

            if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA refPlate = (CConCom_Plate_KA)plate;

                //skusam 557
                this.Ft = refPlate.Ft;

                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fhY2 = refPlate.m_fhY2;
                this.FSlope_rad = refPlate.FSlope_rad;
                //this.m_bScrewInPlusZDirection = refPlate.m_bScrewInPlusZDirection;  //toto kopirovat nechceme
            }
        }
    }
}
