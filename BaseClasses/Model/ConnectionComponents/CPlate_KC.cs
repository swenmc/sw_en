using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace BaseClasses
{
    public class CConCom_Plate_KC : CPlate
    {
        float m_fbXR; // Rafter Width
        float m_fbX1;
        float m_fhY1;
        float m_fbX2;
        float m_fhY2;
        float m_flZ;
        float m_ft;
        float m_fSlope_rad;
        public float m_fRotationZ;

        public CConCom_Plate_KC()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KC(string sName_temp, GraphObj.CPoint controlpoint, float fb_R_temp, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float fl_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 12;
            INoPoints2Dfor3D = 16;
            ITotNoPointsin3D = 30;

            m_pControlPoint = controlpoint;
            m_fbXR = fb_R_temp;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fl_temp;
            m_ft = ft_platethickness;
            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / fb_2_temp);
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();
        }

        public CConCom_Plate_KC(GraphObj.CPoint controlpoint, float fb_R_temp, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float fl_temp, float ft_platethickness, float fSLope_rad_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 12;
            INoPoints2Dfor3D = 16;
            ITotNoPointsin3D = 30;

            m_pControlPoint = controlpoint;
            m_fbXR = fb_R_temp;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fl_temp;
            m_ft = ft_platethickness;
            m_fSlope_rad = fSLope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp = m_flZ * (float)Math.Cos(fBeta);
            float fy_temp = m_flZ * (float)Math.Sin(fBeta);

            PointsOut2D[0, 0] = 0.5f * m_fbXR;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = 0.5f * m_fbXR + m_fbX1;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = 0.5f * m_fbXR + m_fbX1 + fx_temp;
            PointsOut2D[2, 1] = - fy_temp;

            PointsOut2D[3, 0] = 0.5f * m_fbXR + m_fbX2 + fx_temp;
            PointsOut2D[3, 1] = m_fhY2 - fy_temp;

            PointsOut2D[4, 0] = 0.5f * m_fbXR + m_fbX2;
            PointsOut2D[4, 1] = m_fhY2;

            PointsOut2D[5, 0] = 0.5f * m_fbXR;
            PointsOut2D[5, 1] = m_fhY1;

            // Copy - symmetry about y-axis
            for(int i = 0; i < ITotNoPointsin2D / 2; i++)
            {
                PointsOut2D[ITotNoPointsin2D - i - 1, 0] = -PointsOut2D[i, 0];  // Change sign - Negative coordinates "x"
                PointsOut2D[ITotNoPointsin2D - i - 1, 1] = PointsOut2D[i, 1];
            }
        }

        void Calc_Coord3D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp2 = m_ft * (float)Math.Cos(fBeta);
            float fy_temp2 = m_ft * (float)Math.Sin(fBeta);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0.5f * m_fbXR;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = arrPoints3D[0].Z;

            arrPoints3D[2].X = m_fbX1 + fx_temp2;
            arrPoints3D[2].Y = -fy_temp2;
            arrPoints3D[2].Z = arrPoints3D[0].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[2].Y;
            arrPoints3D[3].Z = arrPoints3D[0].Z + m_flZ;

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

            // Copy - symmetry about y-axis
            for (int i = 0; i < INoPoints2Dfor3D / 2; i++)
            {
                arrPoints3D[INoPoints2Dfor3D - i - 1].X = arrPoints3D[i].X;
                arrPoints3D[INoPoints2Dfor3D - i - 1].Y = arrPoints3D[i].Y;
                arrPoints3D[INoPoints2Dfor3D - i - 1].Z = -arrPoints3D[i].Z; // Change sign - Negative coordinates "z"
            }

            // Second layer
            arrPoints3D[INoPoints2Dfor3D + 0].X = -m_ft;
            arrPoints3D[INoPoints2Dfor3D + 0].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 0].Z = 0.5f * m_fbXR + m_ft;

            arrPoints3D[INoPoints2Dfor3D + 1].X = m_fbX1;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = arrPoints3D[INoPoints2Dfor3D + 1].Y;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = 0.5f * m_fbXR + m_flZ;

            arrPoints3D[INoPoints2Dfor3D + 3].X = m_fbX2;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = m_fhY2;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 5].X = 0;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = m_fhY1;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 6].X = -m_ft;
            arrPoints3D[INoPoints2Dfor3D + 6].Y = m_fhY1;
            arrPoints3D[INoPoints2Dfor3D + 6].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            // Copy - symmetry about y-axis
            for (int i = 0; i < 14; i++)
            {
                arrPoints3D[ITotNoPointsin3D - i - 1].X = arrPoints3D[INoPoints2Dfor3D + i].X;
                arrPoints3D[ITotNoPointsin3D - i - 1].Y = arrPoints3D[INoPoints2Dfor3D + i].Y;
                arrPoints3D[ITotNoPointsin3D - i - 1].Z = -arrPoints3D[INoPoints2Dfor3D + i].Z; // Change sign - Negative coordinates "z"
            }
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / BackSide
            AddPenthagonIndices_CCW_12345(TriangleIndices, 17, 20, 21, 22,16);
            AddPenthagonIndices_CCW_12345(TriangleIndices, 25, 24, 23, 29, 28);

            AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 6, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 6, 5, 2);

            AddRectangleIndices_CW_1234(TriangleIndices, 8, 9, 14, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 10, 13, 14);

            AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 19, 18);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 12, 27, 26);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 19, 6, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 20, 21, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 21, 22, 23, 24);
            AddRectangleIndices_CW_1234(TriangleIndices, 8, 24, 25, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 26, 11, 10);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 18, 3, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 16, 17, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 15, 29, 16);
            AddRectangleIndices_CW_1234(TriangleIndices, 14, 28, 29, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 12, 27, 14, 13);

            // Side Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 10, 11, 12, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 8, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 16, 29, 23, 22);

            AddRectangleIndices_CW_1234(TriangleIndices, 17, 20, 19, 18);
            AddRectangleIndices_CW_1234(TriangleIndices, 25, 28, 27, 26);

            AddRectangleIndices_CW_1234(TriangleIndices, 3, 18, 19, 4);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 26, 27, 12);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // BackSide
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                if (i < (INoPoints2Dfor3D - 1))
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
                if (i < (14) - 1)
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
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[15]);
            
            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[23]);
            wireFrame.Points.Add(arrPoints3D[29]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[26]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[27]);

            wireFrame.Points.Add(arrPoints3D[17]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[25]);
            wireFrame.Points.Add(arrPoints3D[28]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[28]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[20]);

            return wireFrame;
        }
    }
}
