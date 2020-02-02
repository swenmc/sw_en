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
    public class CConCom_Plate_JBS : CConCom_Plate_JB
    {
        public CConCom_Plate_JBS()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_J;
        }

        public CConCom_Plate_JBS(string sName_temp,
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

            ITotNoPointsin2D = 11;
            ITotNoPointsin3D = 27;

            m_pControlPoint = controlpoint;
            Fb_X = fb_temp;
            Fh_Y1 = fh_1_temp;
            Fh_Y2 = fh_2_temp;
            Fl_Z = fL_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;
            ScrewInPlusZDirection = bScrewInPlusZDirection;

            UpdatePlateData(screwArrangement);
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            float fx_temp = Fl_Z * (float)Math.Sin(FSlope_rad);
            float fy_temp = Fl_Z * (float)Math.Cos(FSlope_rad);
            float fx_temp2 = Ft * (float)Math.Sin(FSlope_rad);

            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = Fb_X;
            PointsOut2D[1].Y = PointsOut2D[0].Y;

            PointsOut2D[2].X = PointsOut2D[1].X;
            PointsOut2D[2].Y = Fl_Z;

            PointsOut2D[3].X = PointsOut2D[1].X;
            PointsOut2D[3].Y = PointsOut2D[2].Y + Fh_Y1;

            PointsOut2D[4].X = PointsOut2D[3].X;
            PointsOut2D[4].Y = PointsOut2D[3].Y + fy_temp;

            PointsOut2D[5].X = 0.5f * Fb_X + fx_temp2 + fx_temp;
            PointsOut2D[5].Y = Fl_Z + Fh_Y2 + fy_temp;

            PointsOut2D[6].X = 0.5f * Fb_X;
            PointsOut2D[6].Y = Fl_Z + Fh_Y2;

            PointsOut2D[7].X = PointsOut2D[6].X - fx_temp2 - fx_temp;
            PointsOut2D[7].Y = PointsOut2D[5].Y;

            PointsOut2D[8].X = PointsOut2D[0].X;
            PointsOut2D[8].Y = PointsOut2D[4].Y;

            PointsOut2D[9].X = PointsOut2D[0].X;
            PointsOut2D[9].Y = PointsOut2D[3].Y;

            PointsOut2D[10].X = PointsOut2D[0].X;
            PointsOut2D[10].Y = PointsOut2D[2].Y;
        }

        public override void Calc_Coord3D()
        {
            float fx_temp = Fl_Z * (float)Math.Sin(FSlope_rad);
            float fy_temp = Fl_Z * (float)Math.Cos(FSlope_rad);
            float fx_temp2 = Ft * (float)Math.Sin(FSlope_rad);
            float fy_temp2 = Ft * (float)Math.Cos(FSlope_rad);

            float fy_temp3 = fx_temp * (float)Math.Tan(FSlope_rad);

            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = Fl_Z;

            arrPoints3D[1].X = Fb_X;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = arrPoints3D[0].Z;

            arrPoints3D[2].X = arrPoints3D[1].X;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = Fh_Y1 + fy_temp2;
            arrPoints3D[3].Z = arrPoints3D[2].Z;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = arrPoints3D[3].Y;
            arrPoints3D[4].Z = Ft;

            arrPoints3D[5].X = arrPoints3D[1].X - fx_temp;
            arrPoints3D[5].Y = arrPoints3D[4].Y + fy_temp3;
            arrPoints3D[5].Z = arrPoints3D[1].Z;

            arrPoints3D[6].X = 0.5f * Fb_X + fx_temp2;
            arrPoints3D[6].Y = Fh_Y2 + fy_temp2;
            arrPoints3D[6].Z = arrPoints3D[0].Z;

            arrPoints3D[7].X = arrPoints3D[6].X;
            arrPoints3D[7].Y = arrPoints3D[6].Y;
            arrPoints3D[7].Z = 0;

            arrPoints3D[8].X = 0.5f * Fb_X;
            arrPoints3D[8].Y = Fh_Y2;
            arrPoints3D[8].Z = arrPoints3D[2].Z;

            arrPoints3D[9].X = arrPoints3D[8].X - fx_temp2;
            arrPoints3D[9].Y = arrPoints3D[7].Y;
            arrPoints3D[9].Z = arrPoints3D[7].Z;

            arrPoints3D[10].X = arrPoints3D[9].X;
            arrPoints3D[10].Y = arrPoints3D[6].Y;
            arrPoints3D[10].Z = arrPoints3D[6].Z;

            arrPoints3D[11].X = arrPoints3D[0].X + fx_temp;
            arrPoints3D[11].Y = arrPoints3D[5].Y;
            arrPoints3D[11].Z = arrPoints3D[5].Z;

            arrPoints3D[12].X = arrPoints3D[0].X;
            arrPoints3D[12].Y = arrPoints3D[4].Y;
            arrPoints3D[12].Z = arrPoints3D[4].Z;

            arrPoints3D[13].X = arrPoints3D[0].X;
            arrPoints3D[13].Y = arrPoints3D[3].Y;
            arrPoints3D[13].Z = arrPoints3D[3].Z;

            arrPoints3D[14].X = arrPoints3D[0].X;
            arrPoints3D[14].Y = arrPoints3D[0].Y;
            arrPoints3D[14].Z = arrPoints3D[2].Z;

            int i_temp = 15;  // Number of point in first layer

            // Second layer
            arrPoints3D[i_temp + 0].X = 0;
            arrPoints3D[i_temp + 0].Y = Ft;
            arrPoints3D[i_temp + 0].Z = Fl_Z;

            arrPoints3D[i_temp + 1].X = Fb_X;
            arrPoints3D[i_temp + 1].Y = Ft;
            arrPoints3D[i_temp + 1].Z = arrPoints3D[0].Z;

            arrPoints3D[i_temp + 2].X = arrPoints3D[1].X;
            arrPoints3D[i_temp + 2].Y = Ft;
            arrPoints3D[i_temp + 2].Z = Ft;

            arrPoints3D[i_temp + 3].X = arrPoints3D[2].X;
            arrPoints3D[i_temp + 3].Y = Fh_Y1;
            arrPoints3D[i_temp + 3].Z = Ft;

            arrPoints3D[i_temp + 4].X = arrPoints3D[5].X;
            arrPoints3D[i_temp + 4].Y = arrPoints3D[5].Y - fy_temp2;
            arrPoints3D[i_temp + 4].Z = arrPoints3D[1].Z;

            arrPoints3D[i_temp + 5].X = 0.5f * Fb_X;
            arrPoints3D[i_temp + 5].Y = Fh_Y2;
            arrPoints3D[i_temp + 5].Z = arrPoints3D[0].Z;

            arrPoints3D[i_temp + 6].X = arrPoints3D[20].X;
            arrPoints3D[i_temp + 6].Y = arrPoints3D[20].Y;
            arrPoints3D[i_temp + 6].Z = Ft;

            arrPoints3D[i_temp + 7].X = arrPoints3D[i_temp + 6].X;
            arrPoints3D[i_temp + 7].Y = arrPoints3D[i_temp + 6].Y;
            arrPoints3D[i_temp + 7].Z = arrPoints3D[i_temp + 6].Z;

            arrPoints3D[i_temp + 8].X = arrPoints3D[i_temp + 5].X;
            arrPoints3D[i_temp + 8].Y = arrPoints3D[i_temp + 5].Y;
            arrPoints3D[i_temp + 8].Z = arrPoints3D[i_temp + 5].Z;

            arrPoints3D[i_temp + 9].X = arrPoints3D[11].X;
            arrPoints3D[i_temp + 9].Y = arrPoints3D[19].Y;
            arrPoints3D[i_temp + 9].Z = arrPoints3D[19].Z;

            arrPoints3D[i_temp + 10].X = arrPoints3D[0].X;
            arrPoints3D[i_temp + 10].Y = arrPoints3D[18].Y;
            arrPoints3D[i_temp + 10].Z = arrPoints3D[18].Z;

            arrPoints3D[i_temp + 11].X = arrPoints3D[14].X;
            arrPoints3D[i_temp + 11].Y = arrPoints3D[17].Y;
            arrPoints3D[i_temp + 11].Z = arrPoints3D[17].Z;
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 8;
            Dimensions = new CDimension[iNumberOfDimensions + 1];

            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], false, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], false, true);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[8], true, true);
            Dimensions[5] = new CDimensionLinear(plateCenter, PointsOut2D[8], new Point(PointsOut2D[8].X, PointsOut2D[7].Y), true, true);
            Dimensions[6] = new CDimensionLinear(plateCenter, PointsOut2D[0], new Point(PointsOut2D[0].X, PointsOut2D[7].Y), true, true, 50);
            Dimensions[7] = new CDimensionLinear(plateCenter, PointsOut2D[4], PointsOut2D[5], true, true);

            float fx_temp = Fl_Z * (float)Math.Sin(FSlope_rad); // nahradny bod 4
            float fy_temp = Fl_Z * (float)Math.Cos(FSlope_rad);
            Point p4 = new Point(PointsOut2D[3].X + fx_temp, PointsOut2D[3].Y + fy_temp);

            // Tip
            Point pTip = new Point(0.5f * Fb_X, PointsOut2D[0].Y + Fh_Y2 + Fl_Z + fy_temp);

            //Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[3], p4, true, true, 40);
            //Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[4], PointsOut2D[5], true, true);
            //Dimensions[6] = new CDimensionLinear(plateCenter, PointsOut2D[0], new Point(PointsOut2D[0].X, pTip.Y), true, true, 50);
            //Dimensions[7] = new CDimensionLinear(plateCenter, PointsOut2D[8], pTip, true, true);
            //Dimensions[8] = new CDimensionLinear(plateCenter, new Point(PointsOut2D[8].X, PointsOut2D[7].Y), PointsOut2D[7], true, true, 40);

            Dimensions[8] = new CDimensionArc(plateCenter, new Point(PointsOut2D[6].X, PointsOut2D[3].Y), PointsOut2D[6], PointsOut2D[9]);
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
            MemberOutlines[0] = new CLine2D(new Point(PointsOut2D[10].X, PointsOut2D[10].Y), new Point(fx1, fy1));
            MemberOutlines[1] = new CLine2D(new Point(fx1, fy1), new Point(fx2, fy2));
            MemberOutlines[2] = new CLine2D(new Point(fx2, fy2), new Point(PointsOut2D[9].X, PointsOut2D[9].Y));

            MemberOutlines = AddMirroredLinesAboutY(0.5f * Fb_X, MemberOutlines);
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 3;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[2], PointsOut2D[10]);
            BendLines[1] = new CLine2D(PointsOut2D[3], PointsOut2D[6]);
            BendLines[2] = new CLine2D(PointsOut2D[6], PointsOut2D[9]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Back Side
            AddPenthagonIndices_CCW_12345(TriangleIndices, 2, 14, 13, 8, 3);
            AddTriangleIndices_CCW_123(TriangleIndices, 13, 9, 8);
            AddTriangleIndices_CCW_123(TriangleIndices, 3, 8, 7);

            // Front Side
            AddPenthagonIndices_CW_12345(TriangleIndices, 17, 26, 25, 21, 18);

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 16, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 6, 20, 19);
            AddRectangleIndices_CCW_1234(TriangleIndices, 10, 11, 24, 23);

            // Shell Surface
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 17, 16);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 4, 17);
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 19, 18);

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 15, 26, 14);
            AddRectangleIndices_CCW_1234(TriangleIndices, 14, 26, 12, 13);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 12, 25, 24);

            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 10, 23);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 20, 6, 7);

            // Top / Bottom
            AddPenthagonIndices_CCW_12345(TriangleIndices, 3, 7, 6, 5, 4);
            AddPenthagonIndices_CCW_12345(TriangleIndices, 9, 13, 12, 11, 10);

            AddRectangleIndices_CCW_1234(TriangleIndices, 18, 19, 20, 21);
            AddRectangleIndices_CCW_1234(TriangleIndices, 22, 23, 24, 25);

            AddRectangleIndices_CCW_1234(TriangleIndices, 15, 16, 17, 26);
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 14, 2, 1);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            for (int i = 0; i < 15; i++)
            {
                if (i < (15) - 1)
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

            for (int i = 0; i < 12; i++)
            {
                if (i < (12) - 1)
                {
                    pi = arrPoints3D[15 + i];
                    pj = arrPoints3D[15 + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[15 + i];
                    pj = arrPoints3D[15];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Front
            wireFrame.Points.Add(arrPoints3D[17]);
            wireFrame.Points.Add(arrPoints3D[26]);

            wireFrame.Points.Add(arrPoints3D[18]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[22]);
            wireFrame.Points.Add(arrPoints3D[25]);

            // Back
            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[13]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[25]);

            return wireFrame;
        }
    }
}
