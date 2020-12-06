﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using BaseClasses.GraphObj;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_KB : CPlate_Frame
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

        public Point pTip;

        public CConCom_Plate_KB()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
        }

        public CConCom_Plate_KB(string sName_temp,
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

            ITotNoPointsin2D = 6;
            INoPoints2Dfor3D = 8;
            ITotNoPointsin3D = 14;

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
                // Parameter flZ - // Distance from the left edge is the same as KA plate (lz is used for KC and KD plates)
                screwArrangement.Calc_KneePlateData(m_fbX1, m_fbX2, 0, m_fhY1, Ft, FSlope_rad, ScrewInPlusZDirection);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            // Tip before cutting off - only on the right side (no theroretical tip point for falling knee)
            if (FSlope_rad > 0)
            {
                float fBeta = (float)Math.Atan((Fb_X2 - Fb_X1) / Fh_Y2);

                float fc = Fl_Z / (float)Math.Cos(fBeta + FSlope_rad);
                float fa = Fl_Z * (float)Math.Tan(fBeta + FSlope_rad);

                float pTipX = (float)PointsOut2D[4].X + fc * (float)Math.Cos(FSlope_rad);
                float pTipY = (float)PointsOut2D[4].Y + fc * (float)Math.Sin(FSlope_rad);

                pTip = new Point(pTipX, pTipY);
            }

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

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp = m_flZ * (float)Math.Cos(fBeta);
            float fy_temp = m_flZ * (float)Math.Sin(fBeta);

            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX1;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = m_fbX1 + fx_temp;
            PointsOut2D[2].Y = - fy_temp;

            PointsOut2D[3].X = m_fbX2 + fx_temp;
            PointsOut2D[3].Y = m_fhY2 - fy_temp;

            PointsOut2D[4].X = m_fbX2;
            PointsOut2D[4].Y = m_fhY2;

            PointsOut2D[5].X = 0;
            PointsOut2D[5].Y = m_fhY1;
        }

        public override void Calc_Coord3D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp2 = Ft * (float)Math.Cos(fBeta);
            float fy_temp2 = Ft * (float)Math.Sin(fBeta);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = arrPoints3D[0].Z;

            arrPoints3D[2].X = m_fbX1 + fx_temp2;
            arrPoints3D[2].Y = -fy_temp2;
            arrPoints3D[2].Z = arrPoints3D[0].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[2].Y;
            arrPoints3D[3].Z = m_flZ;

            arrPoints3D[4].X = m_fbX2 + fx_temp2;
            arrPoints3D[4].Y = m_fhY2 - fy_temp2;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[4].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = m_fbX2;
            arrPoints3D[6].Y = m_fhY2;
            arrPoints3D[6].Z = arrPoints3D[0].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = m_fhY1;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            // Second layer
            arrPoints3D[INoPoints2Dfor3D + 0].X = 0;
            arrPoints3D[INoPoints2Dfor3D + 0].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 0].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 1].X = m_fbX1;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = arrPoints3D[INoPoints2Dfor3D + 1].Y;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = m_flZ;

            arrPoints3D[INoPoints2Dfor3D + 3].X = m_fbX2;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = m_fhY2;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 5].X = arrPoints3D[INoPoints2Dfor3D + 0].X;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = m_fhY1;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 11;
            Dimensions = new CDimension[iNumberOfDimensions + 1];
            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            // Bottom
            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1]);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], new Point(PointsOut2D[2].X, PointsOut2D[1].Y));
            Dimensions[2] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[0].X, PointsOut2D[2].Y), PointsOut2D[2], true, true, 53);

            // Top
            if (FSlope_rad > 0)
            {
                Dimensions[3] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[4].X, pTip.Y), new Point(PointsOut2D[5].X, pTip.Y));
                Dimensions[4] = new CDimensionLinear(plateCenter, pTip, new Point(PointsOut2D[4].X, pTip.Y));
                Dimensions[5] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[5].X, pTip.Y), pTip, true, true, 53);
            }
            else
            {
                Dimensions[3] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[4].X, PointsOut2D[5].Y), PointsOut2D[5]);
                Dimensions[4] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[3].X, PointsOut2D[5].Y), new Point(PointsOut2D[4].X, PointsOut2D[5].Y));
                Dimensions[5] = new CDimensionLinear(plateCenter, PointsOut2D[5], new Point(PointsOut2D[3].X, PointsOut2D[5].Y), true, true, 53);
            }

            // Vertical
            Dimensions[6] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[5], true, true);

            if (FSlope_rad > 0)
            {
                // Left
                Dimensions[7] = new CDimensionLinear(plateCenter, PointsOut2D[5], new Point(PointsOut2D[5].X, pTip.Y), true, true);

                // Right
                Dimensions[8] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[3].X, PointsOut2D[2].Y), PointsOut2D[3], true, true);
                Dimensions[9] = new CDimensionLinear(plateCenter, PointsOut2D[3], new Point(PointsOut2D[3].X, pTip.Y), true, true);
                Dimensions[10] = new CDimensionLinear(plateCenter, new Point(pTip.X, PointsOut2D[2].Y), pTip, true, true, 55);
            }
            else
            {
                // Left
                //Dimensions[7] = Dimensions[6]; // Identicka, inak treba osetrit rozny pocet kot pre falling knee
                Dimensions[7] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[5], true, true);

                // Right
                Dimensions[8] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[3].X, PointsOut2D[2].Y), PointsOut2D[3], true, true);
                Dimensions[9] = new CDimensionLinear(plateCenter, PointsOut2D[3], new Point(PointsOut2D[3].X, PointsOut2D[5].Y), true, true);
                Dimensions[10] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[3].X, PointsOut2D[2].Y), new Point(PointsOut2D[3].X, PointsOut2D[5].Y), true, true, 55);
            }

            Dimensions[11] = new CDimensionArc(plateCenter, new Point(PointsOut2D[1].X, PointsOut2D[5].Y), PointsOut2D[4], PointsOut2D[5]);
        }

        public override void Set_MemberOutlinePoints2D()
        {
            int iNumberOfLines = 4 + (FSlope_rad > 0 ? 2 :0);
            MemberOutlines = new CLine2D[iNumberOfLines];

            // TODO - refaktorovat pre plechy KA az KE

            // Skratenie pruta v smere pruta (5 mm)
            float fcut = 0.005f;

            float fdepth = Fb_X1; // ???

            float fx1 = fdepth;
            float fy1 = fdepth - fcut;

            float fx2 = 0;
            float fy2 = fy1;

            float fb1_y = (float)PointsOut2D[5].Y - fdepth * (float)Math.Cos(FSlope_rad); // Teoreticky bod, kde sa stretne rafter a column ak neuvazujeme skratenie prutov
            float fb1_x = fdepth * (float)Math.Sin(FSlope_rad);

            float faux_x = fcut * (float)Math.Cos(FSlope_rad);
            float faux_y = fcut * (float)Math.Sin(FSlope_rad);

            float fx3 = (float)PointsOut2D[5].X + faux_x; // Vlavo hore
            float fy3 = (float)PointsOut2D[5].Y + faux_y;

            float fx4 = fx3 + fdepth * (float)Math.Sin(FSlope_rad);
            float fy4 = fy3 - fdepth * (float)Math.Cos(FSlope_rad);

            // Theoretical tip point - 2 lines
            float fx6 = (float)PointsOut2D[3].X;
            float fy6 = (float)PointsOut2D[3].Y;

            float fx7 = (float)PointsOut2D[4].X;
            float fy7 = (float)PointsOut2D[4].Y;

            if (FSlope_rad < 0) // Falling knee
            {
                float fxb3_temp = fdepth * (float)Math.Sin(-FSlope_rad);
                float fyb3_temp = fxb3_temp * (float)Math.Tan(-FSlope_rad);

                float faux = fxb3_temp / (float)Math.Cos(-FSlope_rad); // Dlzka odrezanej hrany vlavo hore

                faux_x = fcut * (float)Math.Cos(-FSlope_rad);
                faux_y = fcut * (float)Math.Sin(-FSlope_rad);

                fx3 = (float)PointsOut2D[5].X + fxb3_temp + faux_x; // Hore
                fy3 = (float)PointsOut2D[5].Y - fyb3_temp - faux_y;

                fx4 = fx3 - fdepth * (float)Math.Sin(-FSlope_rad); // Vlavo
                fy4 = fy3 - fdepth * (float)Math.Cos(-FSlope_rad);

                float fb4_x = (float)PointsOut2D[5].X;
                float fb4_y = (float)PointsOut2D[5].Y - fyb3_temp - fdepth * (float)Math.Cos(-FSlope_rad);

                fb1_x = (float)PointsOut2D[5].X + fdepth;
                fb1_y = fb4_y - fdepth * (float)Math.Tan(-FSlope_rad);

                fx1 = (float)PointsOut2D[5].X + fdepth;
                fy1 = fb1_y - fcut;

                fx2 = (float)PointsOut2D[5].X;
                fy2 = fy1;
            }

            bool considerCollinearOverlapAsIntersect = true;

            Vector2D intersection;

            Geom2D.LineSegementsIntersect(
                  new Vector2D(fb1_x, fb1_y),
                  new Vector2D(10, fb1_y + 10 * Math.Tan(FSlope_rad)),
                  new Vector2D(PointsOut2D[1].X, PointsOut2D[1].Y),
                  new Vector2D(PointsOut2D[4].X, PointsOut2D[4].Y),
                  out intersection,
                  considerCollinearOverlapAsIntersect);

            float fx5 = (float)intersection.X;
            float fy5 = (float)intersection.Y;

            // Body su nezavisle na bodoch outline aj ked maju rovnake suradnice
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[1].X, PointsOut2D[1].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));
            MemberOutlines[2] = new CLine2D(new Point(fx3, fy3), new Point(fx4, fy4));
            MemberOutlines[3] = new CLine2D(new Point(fx4, fy4), new Point(fx5, fy5));

            // Theoretical tip point - 2 lines
            if (FSlope_rad > 0)
            {
                MemberOutlines[4] = new CLine2D(new Point(fx6, fy6), new Point(pTip.X, pTip.Y));
                MemberOutlines[5] = new CLine2D(new Point(fx7, fy7), new Point(pTip.X, pTip.Y));
            }
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 1;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[1], PointsOut2D[4]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 12, 13, 8);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 11, 10);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 6, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 5, 6);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 6, 12, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 11, 6, 5);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 8, 9, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 10, 3, 2);

            // Side Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 13, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 12, 11, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // BackSide
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                if (i < INoPoints2Dfor3D - 1)
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
            for (int i = 0; i < ITotNoPointsin2D; i++)
            {
                if (i < (ITotNoPointsin2D) - 1)
                {
                    pi = arrPoints3D[INoPoints2Dfor3D + i];
                    pj = arrPoints3D[INoPoints2Dfor3D + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[INoPoints2Dfor3D + i];
                    pj = arrPoints3D[INoPoints2Dfor3D + 0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[8]);
            
            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[12]);

            return wireFrame;
        }


        public override void MirrorPlateAboutX()
        {
            pTip.Y *= -1;

            base.MirrorPlateAboutX();
        }

        public override void MirrorPlateAboutY()
        {
            pTip.X *= -1;

            base.MirrorPlateAboutY();
        }

        public override void RotatePlateAboutZ_CW(float fTheta_deg)
        {
            pTip = Geom2D.TransformPositions_CW_deg(0, 0, fTheta_deg, pTip);

            base.RotatePlateAboutZ_CW(fTheta_deg);
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_KB)
            {
                CConCom_Plate_KB refPlate = (CConCom_Plate_KB)plate;
                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fhY2 = refPlate.m_fhY2;
                this.m_flZ = refPlate.m_flZ;
                this.FSlope_rad = refPlate.FSlope_rad;
                //this.m_bScrewInPlusZDirection = refPlate.m_bScrewInPlusZDirection;  //toto kopirovat nechceme
                this.pTip = refPlate.pTip;
            }
        }
    }
}
