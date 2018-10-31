using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_KDS : CConCom_Plate_KD
    {
        public CConCom_Plate_KDS()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KDS(string sName_temp,
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
            Fb_X1 = fb_1_temp;
            Fh_Y1 = fh_1_temp;
            Fb_X2 = fb_2_temp;
            Fh_Y2 = fh_2_temp;
            Fl_Z = fl_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement);
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

            PointsOut2D[4].X = Fl_Z + Fb_X2 + (FSlope_rad > 0 ? fx_temp : fx_temp4);
            PointsOut2D[4].Y = Fh_Y2 - (FSlope_rad > 0 ? fy_temp : fy_temp4);

            PointsOut2D[5].X = Fl_Z + Fb_X2;
            PointsOut2D[5].Y = Fh_Y2;

            PointsOut2D[6].X = Fl_Z;
            PointsOut2D[6].Y = Fh_Y1;

            PointsOut2D[7].X = 0;
            PointsOut2D[7].Y = Fh_Y1 - (FSlope_rad > 0 ? fy_temp2 : 0);
        }

        public override void Calc_Coord3D()
        {
            float fBeta = (float)Math.Atan((Fb_X2 - Fb_X1) / Fh_Y2);
            float fx_temp2 = Ft * (float)Math.Cos(fBeta);
            float fy_temp2 = Ft * (float)Math.Sin(fBeta);

            float fx_temp3 = Ft / (float)Math.Cos(fBeta);

            float fy_temp4 = (Fl_Z-Ft) * (float)Math.Tan(FSlope_rad);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = Fl_Z;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = Fb_X1 + fx_temp2;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[2].Y;
            arrPoints3D[3].Z = Fl_Z;

            arrPoints3D[4].X = Fb_X2 + fx_temp2;
            arrPoints3D[4].Y = Fh_Y2 - fy_temp2;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[4].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[1].Z;

            arrPoints3D[6].X = Fb_X2;
            arrPoints3D[6].Y = Fh_Y2;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = Ft;
            arrPoints3D[7].Y = Fh_Y1;
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            arrPoints3D[8].X = arrPoints3D[1].X;
            arrPoints3D[8].Y = arrPoints3D[7].Y;
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            arrPoints3D[9].X = arrPoints3D[1].X;
            arrPoints3D[9].Y = arrPoints3D[8].Y;
            arrPoints3D[9].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[10].X = arrPoints3D[1].X;
            arrPoints3D[10].Y = arrPoints3D[9].Y - fy_temp4;
            arrPoints3D[10].Z = arrPoints3D[0].Z;

            // Second layer
            // INoPoints2Dfor3D = 10
            arrPoints3D[INoPoints2Dfor3D + 1].X = arrPoints3D[7].X;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = arrPoints3D[0].Y;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 3].X = arrPoints3D[2].X - fx_temp3;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = arrPoints3D[2].Y;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = Fl_Z;

            arrPoints3D[INoPoints2Dfor3D + 5].X = arrPoints3D[6].X;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = arrPoints3D[6].Y;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[INoPoints2Dfor3D + 4].Z;

            arrPoints3D[INoPoints2Dfor3D + 6].X = arrPoints3D[6].X;
            arrPoints3D[INoPoints2Dfor3D + 6].Y = arrPoints3D[6].Y;
            arrPoints3D[INoPoints2Dfor3D + 6].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 7].X = arrPoints3D[7].X;
            arrPoints3D[INoPoints2Dfor3D + 7].Y = arrPoints3D[7].Y;
            arrPoints3D[INoPoints2Dfor3D + 7].Z = Ft;

            arrPoints3D[INoPoints2Dfor3D + 8].X = arrPoints3D[INoPoints2Dfor3D + 7].X;
            arrPoints3D[INoPoints2Dfor3D + 8].Y = arrPoints3D[10].Y;
            arrPoints3D[INoPoints2Dfor3D + 8].Z = arrPoints3D[10].Z;
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 11;
            Dimensions = new CDimension[iNumberOfDimensions + 1];
            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1]);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2]);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], true, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[4], PointsOut2D[5], true, true);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[5], PointsOut2D[6], true, true);
            Dimensions[5] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[7], true, true);
            Dimensions[6] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[3], true, true, 50);
            Dimensions[7] = new CDimensionLinear(plateCenter, PointsOut2D[7], PointsOut2D[5], true, true, 50);

            // Tip before cutting off
            float pTipX;
            float pTipY;

            if (FSlope_rad > 0)
            {
                float fBeta = (float)Math.Atan((Fb_X2 - Fb_X1) / Fh_Y2);

                float fc = Fl_Z / (float)Math.Cos(fBeta + FSlope_rad);
                float fa = Fl_Z * (float)Math.Tan(fBeta + FSlope_rad);

                pTipX = (float)PointsOut2D[5].X + fc * (float)Math.Cos(FSlope_rad);
                pTipY = (float)PointsOut2D[5].Y + fc * (float)Math.Sin(FSlope_rad);
            }
            else
            {
                pTipX = (float)PointsOut2D[7].X;
                pTipY = (float)PointsOut2D[7].Y + Fl_Z * (float)Math.Tan(-FSlope_rad);
            }

            Point pTip = new Point(pTipX, pTipY);

            if (FSlope_rad > 0)
                Dimensions[8] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[6], true, true, 70);
            else
                Dimensions[8] = new CDimensionLinear(plateCenter, PointsOut2D[0], pTip, true, true, 60);

            Dimensions[9] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[4].X, PointsOut2D[3].Y), PointsOut2D[4], true, true, 50);

            if (FSlope_rad > 0)
                Dimensions[10] = new CDimensionLinear(plateCenter, new Point(pTip.X, PointsOut2D[3].Y), pTip, true, true, 70);
            else
                Dimensions[10] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[3].X, PointsOut2D[2].Y), PointsOut2D[3], true, true, 70);

            Dimensions[11] = new CDimensionArc(plateCenter, new Point(PointsOut2D[2].X, PointsOut2D[6].Y), PointsOut2D[5], PointsOut2D[6]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 13, 16, 17);
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 11, 18, 10);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 15, 14);

            // Back Side
            AddHexagonIndices_CCW_123456(TriangleIndices, 1, 8, 7, 6, 5, 2);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 18, 10, 9, 17);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 8, 7, 17);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 16, 17, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 15, 6, 5);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 11, 12, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 12, 13, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 13, 14, 3);

            // Side Surface
            AddPenthagonIndices_CW_12345(TriangleIndices, 0, 1,8, 9, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 18, 17, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 16, 15, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

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

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[13]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[8]);

            return wireFrame;
        }
    }
}
