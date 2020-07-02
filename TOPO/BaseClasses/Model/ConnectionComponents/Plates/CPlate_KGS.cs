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
    public class CConCom_Plate_KGS : CConCom_Plate_KHS
    {
        public CConCom_Plate_KGS()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
        }

        public CConCom_Plate_KGS(string sName_temp,
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
            INoPoints2Dfor3D = 11;
            ITotNoPointsin3D = 26;

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
            ScrewInPlusZDirection = bScrewInPlusZDirection;

            UpdatePlateData(screwArrangement);
        }

        //----------------------------------------------------------------------------

        public override void Calc_Coord3D()
        {
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
            arrPoints3D[0].Z = Fl_Z;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = Fb_X1;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X + fx_temp9;
            arrPoints3D[3].Y = arrPoints3D[2].Y + fy_temp9;
            arrPoints3D[3].Z = -Fl_Z;

            arrPoints3D[5].X = (Fb_X2) - fc1_temp * (float)Math.Sin(fBeta);
            arrPoints3D[5].Y = Fh_Y2 - fc1_temp * (float)Math.Cos(fBeta);
            arrPoints3D[5].Z = 0;

            arrPoints3D[4].X = arrPoints3D[5].X;
            arrPoints3D[4].Y = arrPoints3D[5].Y;
            arrPoints3D[4].Z = -Fl_Z;

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

            arrPoints3D[8].X = Ft;
            arrPoints3D[8].Y = Fh_Y1;
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            arrPoints3D[9].X = arrPoints3D[1].X;
            arrPoints3D[9].Y = arrPoints3D[8].Y;
            arrPoints3D[9].Z = arrPoints3D[1].Z;

            arrPoints3D[10].X = arrPoints3D[1].X;
            arrPoints3D[10].Y = arrPoints3D[9].Y;
            arrPoints3D[10].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[11].X = arrPoints3D[1].X;
            arrPoints3D[11].Y = arrPoints3D[10].Y - (FSlope_rad > 0 ? fy_temp4 : 0);
            arrPoints3D[11].Z = arrPoints3D[0].Z;

            // Second layer
            // INoPoints2Dfor3D = 12
            arrPoints3D[INoPoints2Dfor3D + 1].X = Ft;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = arrPoints3D[0].Y;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = arrPoints3D[0].Y;
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
            arrPoints3D[INoPoints2Dfor3D + 6].Z = arrPoints3D[3].Z;

            arrPoints3D[INoPoints2Dfor3D + 7].X = arrPoints3D[4].X + fx_temp2; // Opravit
            arrPoints3D[INoPoints2Dfor3D + 7].Y = arrPoints3D[4].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 7].Z = arrPoints3D[3].Z;

            arrPoints3D[INoPoints2Dfor3D + 8].X = arrPoints3D[5].X + fx_temp2;
            arrPoints3D[INoPoints2Dfor3D + 8].Y = arrPoints3D[5].Y - fy_temp2;
            arrPoints3D[INoPoints2Dfor3D + 8].Z = arrPoints3D[1].Z;

            arrPoints3D[INoPoints2Dfor3D + 9].X = arrPoints3D[INoPoints2Dfor3D + 8].X;
            arrPoints3D[INoPoints2Dfor3D + 9].Y = arrPoints3D[INoPoints2Dfor3D + 8].Y;
            arrPoints3D[INoPoints2Dfor3D + 9].Z = arrPoints3D[INoPoints2Dfor3D + 4].Z;

            arrPoints3D[INoPoints2Dfor3D + 10].X = arrPoints3D[5].X;
            arrPoints3D[INoPoints2Dfor3D + 10].Y = arrPoints3D[5].Y;
            arrPoints3D[INoPoints2Dfor3D + 10].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[INoPoints2Dfor3D + 11].X = arrPoints3D[6].X;
            arrPoints3D[INoPoints2Dfor3D + 11].Y = arrPoints3D[6].Y;
            arrPoints3D[INoPoints2Dfor3D + 11].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 12].X = arrPoints3D[7].X;
            arrPoints3D[INoPoints2Dfor3D + 12].Y = arrPoints3D[7].Y;
            arrPoints3D[INoPoints2Dfor3D + 12].Z = arrPoints3D[10].Z;

            arrPoints3D[INoPoints2Dfor3D + 13].X = arrPoints3D[8].X;
            arrPoints3D[INoPoints2Dfor3D + 13].Y = arrPoints3D[8].Y;
            arrPoints3D[INoPoints2Dfor3D + 13].Z = arrPoints3D[10].Z;

            arrPoints3D[INoPoints2Dfor3D + 14].X = arrPoints3D[12].X;
            arrPoints3D[INoPoints2Dfor3D + 14].Y = arrPoints3D[11].Y;
            arrPoints3D[INoPoints2Dfor3D + 14].Z = arrPoints3D[0].Z;
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();
 
            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 24, 21, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 21, 24, 23, 22);
            AddRectangleIndices_CW_1234(TriangleIndices, 14, 21, 20, 15);

            // Back Side
            AddPenthagonIndices_CW_12345(TriangleIndices, 1, 2, 5, 8, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 5, 6, 7, 8);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 23, 24, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 10, 9, 8, 24);
            AddRectangleIndices_CW_1234(TriangleIndices, 10, 24, 25, 11);

            AddRectangleIndices_CW_1234(TriangleIndices, 5, 19, 20, 21);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 18, 19, 5);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 12, 13, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 13, 14, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 14, 15, 16, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 16, 17, 3);

            AddRectangleIndices_CW_1234(TriangleIndices, 5, 21, 22, 6);

            // Side Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 11, 25, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 12, 25, 24, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 22, 23, 7, 6);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
            AddHexagonIndices_CW_123456(TriangleIndices, 15, 20, 19, 18, 17, 16);
            AddPenthagonIndices_CW_12345(TriangleIndices, 0, 1, 9, 10, 11);
            AddRectangleIndices_CW_1234(TriangleIndices, 17, 18, 4, 3);
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
            for (int i = 0; i < 14; i++)
            {
                if (i < 14 - 1)
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
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[16]);
 
            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[13]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[15]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[9]);

            return wireFrame;
        }
    }
}
