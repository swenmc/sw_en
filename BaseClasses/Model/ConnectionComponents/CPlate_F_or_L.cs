using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace BaseClasses
{
    public class CConCom_Plate_F_or_L : CPlate
    {
        public float m_fbX1;
        public float m_fbX2;
        public float m_fhY;
        public float m_flZ; // Not used in 2D model
        public float m_ft; // Not used in 2D model
        public int m_iHolesNumber = 0;

        public CConCom_Plate_F_or_L()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_F_or_L(string sName_temp, GraphObj.CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX_temp;
            m_fbX2 = m_fbX1; // L Series - without slope
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory
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

        public CConCom_Plate_F_or_L(string sName_temp, GraphObj.CPoint controlpoint, int iLeftRightIndex, float fbX1_temp, float fbX2_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory
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

            if (iLeftRightIndex % 2 != 0) // Change y-coordinates for odd index (RH)
            {
                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i, 0] *= -1;
                }

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].X *= -1;
                }

                // Change indices
                ChangeIndices(TriangleIndices); // Todo - takto to nefunguje :-)
            }
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_flZ;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = PointsOut2D[1, 0] + m_fbX1;
            PointsOut2D[2, 1] = 0;

            PointsOut2D[3, 0] = PointsOut2D[1, 0] + m_fbX2;
            PointsOut2D[3, 1] = m_fhY;

            PointsOut2D[4, 0] = PointsOut2D[1, 0];
            PointsOut2D[4, 1] = m_fhY;

            PointsOut2D[5, 0] = PointsOut2D[0, 0];
            PointsOut2D[5, 1] = m_fhY;
        }

        void Calc_Coord3D()
        {
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX1;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = m_fbX2;
            arrPoints3D[3].Y = m_fhY;
            arrPoints3D[3].Z = arrPoints3D[2].Z;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[1].Z;

            arrPoints3D[5].X = arrPoints3D[0].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = arrPoints3D[0].X + m_ft;
            arrPoints3D[6].Y = arrPoints3D[0].Y;
            arrPoints3D[6].Z = arrPoints3D[0].Z;

            arrPoints3D[7].X = arrPoints3D[1].X + +m_ft;
            arrPoints3D[7].Y = arrPoints3D[1].Y;
            arrPoints3D[7].Z = arrPoints3D[1].Z + m_ft;

            arrPoints3D[8].X = arrPoints3D[2].X;
            arrPoints3D[8].Y = arrPoints3D[2].Y;
            arrPoints3D[8].Z = arrPoints3D[2].Z + m_ft;

            arrPoints3D[9].X = arrPoints3D[3].X;
            arrPoints3D[9].Y = arrPoints3D[3].Y;
            arrPoints3D[9].Z = arrPoints3D[3].Z + m_ft;

            arrPoints3D[10].X = arrPoints3D[7].X;
            arrPoints3D[10].Y = arrPoints3D[7].Y + m_fhY;
            arrPoints3D[10].Z = arrPoints3D[7].Z;

            arrPoints3D[11].X = arrPoints3D[6].X;
            arrPoints3D[11].Y = arrPoints3D[6].Y + m_fhY;
            arrPoints3D[11].Z = arrPoints3D[6].Z;
        }

        protected override void loadIndices()
        {
            int secNum = 6;
            TriangleIndices = new Int32Collection();

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 5, 4, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 4, 3, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 10, 11);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7, 8, 9, 10);

            // Shell Surface
            DrawCaraLaterals_CW(secNum, TriangleIndices);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            // y = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[0]);

            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[8]);

            // y = m_fhY
            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[11]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[4]);

            return wireFrame;
        }


    }
}
