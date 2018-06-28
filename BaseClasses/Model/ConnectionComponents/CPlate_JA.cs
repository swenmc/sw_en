using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace BaseClasses
{
    public class CConCom_Plate_JA : CPlate
    {
        float m_fb;
        float m_fh_1;
        float m_fh_2;
        float m_ft;
        float m_fSlope_rad;
        public float m_fRotationZ;

        public CConCom_Plate_JA()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_JA(GraphObj.CPoint controlpoint, float fb_temp, float fh_1_temp, float fh_2_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 5;
            ITotNoPointsin3D = 10;

            m_pControlPoint = controlpoint;
            m_fb = fb_temp;
            m_fh_1 = fh_1_temp;
            m_fh_2 = fh_2_temp;
            m_ft = ft_platethickness;
            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / (0.5 * fb_temp));
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

        public CConCom_Plate_JA(GraphObj.CPoint controlpoint, float fb_temp, float fh_1_temp, float fh_2_temp, float ft_platethickness, float fSLope_rad_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 5;
            ITotNoPointsin3D = 10;

            m_pControlPoint = controlpoint;
            m_fb = fb_temp;
            m_fh_1 = fh_1_temp;
            m_fh_2 = fh_2_temp;
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
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_fb;
            PointsOut2D[1, 1] = PointsOut2D[0, 1];

            PointsOut2D[2, 0] = PointsOut2D[1, 0];
            PointsOut2D[2, 1] = m_fh_1;

            PointsOut2D[3, 0] = 0.5f * m_fb;
            PointsOut2D[3, 1] = m_fh_2;

            PointsOut2D[4, 0] = PointsOut2D[0, 0];
            PointsOut2D[4, 1] = PointsOut2D[2, 1];
        }

        void Calc_Coord3D()
        {
            for (int i = 0; i < 2; i++) // 2 cycles - front and back surface
            {
                // One Side
                for (int j = 0; j < ITotNoPointsin2D; j++)
                {
                    arrPoints3D[(i * ITotNoPointsin2D) + j].X = PointsOut2D[j, 0];
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Y = PointsOut2D[j, 1];
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Z = i * m_ft;
                }
            }
        }

        protected override void loadIndices()
        {
            int secNum = 5;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddPenthagonIndices_CCW_12345(TriangleIndices, 0, 1, 2, 3, 4);

            // Back Side
            AddPenthagonIndices_CW_12345(TriangleIndices, 5, 6, 7, 8, 9);

            // Shell Surface
            DrawCaraLaterals_CCW(secNum, TriangleIndices);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // Front Side
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                if (i < (PointsOut2D.Length / 2) - 1)
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
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                if (i < (PointsOut2D.Length / 2) - 1)
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
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                pi = arrPoints3D[i];
                pj = arrPoints3D[ITotNoPointsin2D + i];

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            return wireFrame;
        }
    }
}
