using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace BaseClasses
{
    public class CConCom_Plate_LL : CPlate
    {
        public float m_fbX1;
        public float m_fbX2;
        public float m_fhY;
        public float m_flZ; // Not used in 2D model
        public float m_ft; // Not used in 2D model
        public int m_iHolesNumber = 0;

        public CConCom_Plate_LL()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_LL(GraphObj.CPoint controlpoint, float fbX1_temp, float fbX2_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, int iHolesNumber, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 12;
            ITotNoPointsin3D = 26;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
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

            PointsOut2D[1, 0] = 0;
            PointsOut2D[1, 1] = m_fbX1;

            PointsOut2D[2, 0] = PointsOut2D[1, 0] + m_fbX2;
            PointsOut2D[2, 1] = PointsOut2D[1, 1];

            PointsOut2D[3, 0] = PointsOut2D[2, 0];
            PointsOut2D[3, 1] = PointsOut2D[0, 1];

            PointsOut2D[4, 0] = PointsOut2D[3, 0] + m_fhY - m_ft;
            PointsOut2D[4, 1] = PointsOut2D[3, 1];

            PointsOut2D[5, 0] = PointsOut2D[4, 0];
            PointsOut2D[5, 1] = PointsOut2D[2, 1];

            PointsOut2D[6, 0] = PointsOut2D[5, 0];
            PointsOut2D[6, 1] = PointsOut2D[5, 1] + m_flZ;

            PointsOut2D[7, 0] = PointsOut2D[2, 0];
            PointsOut2D[7, 1] = PointsOut2D[6, 1];

            PointsOut2D[8, 0] = PointsOut2D[0, 0];
            PointsOut2D[8, 1] = PointsOut2D[7, 1];

            PointsOut2D[9, 0] = PointsOut2D[0, 0] - (m_fhY - m_ft);
            PointsOut2D[9, 1] = PointsOut2D[6, 1];

            PointsOut2D[10, 0] = PointsOut2D[9, 0];
            PointsOut2D[10, 1] = PointsOut2D[1, 1];

            PointsOut2D[11, 0] = PointsOut2D[10, 0];
            PointsOut2D[11, 1] = PointsOut2D[0, 1];
        }

        void Calc_Coord3D()
        {
            // First layer

            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = arrPoints3D[1].X + m_fbX2;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X + m_fbX1;
            arrPoints3D[3].Y = 0;
            arrPoints3D[3].Z = 0;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[2].X;
            arrPoints3D[6].Y = m_ft;
            arrPoints3D[6].Z = arrPoints3D[2].Z;

            arrPoints3D[7].X = arrPoints3D[1].X;
            arrPoints3D[7].Y = m_ft;
            arrPoints3D[7].Z = arrPoints3D[1].Z;

            arrPoints3D[8].X = arrPoints3D[1].X;
            arrPoints3D[8].Y = m_fhY;
            arrPoints3D[8].Z = arrPoints3D[1].Z;

            arrPoints3D[9].X = arrPoints3D[0].X;
            arrPoints3D[9].Y = m_fhY;
            arrPoints3D[9].Z = arrPoints3D[0].Z;

            // Second layer

            arrPoints3D[10].X = arrPoints3D[0].X;
            arrPoints3D[10].Y = arrPoints3D[0].Y;
            arrPoints3D[10].Z = m_ft;

            arrPoints3D[11].X = m_fbX1 - m_ft;
            arrPoints3D[11].Y = arrPoints3D[10].X;
            arrPoints3D[11].Z = m_ft;

            arrPoints3D[12].X = m_fbX1 + m_fbX2 + m_ft;
            arrPoints3D[12].Y = arrPoints3D[11].Y;
            arrPoints3D[12].Z = m_ft;

            arrPoints3D[13].X = arrPoints3D[3].X;
            arrPoints3D[13].Y = arrPoints3D[3].Y;
            arrPoints3D[13].Z = m_ft;

            arrPoints3D[14].X = arrPoints3D[4].X;
            arrPoints3D[14].Y = arrPoints3D[4].Y;
            arrPoints3D[14].Z = m_ft;

            arrPoints3D[15].X = arrPoints3D[12].X;
            arrPoints3D[15].Y = arrPoints3D[5].Y;
            arrPoints3D[15].Z = m_ft;

            arrPoints3D[16].X = arrPoints3D[11].X;
            arrPoints3D[16].Y = arrPoints3D[8].Y;
            arrPoints3D[16].Z = m_ft;

            arrPoints3D[17].X = arrPoints3D[9].X;
            arrPoints3D[17].Y = arrPoints3D[9].Y;
            arrPoints3D[17].Z = m_ft;

            // Third layer

            arrPoints3D[18].X = arrPoints3D[11].X;
            arrPoints3D[18].Y = arrPoints3D[11].Y;
            arrPoints3D[18].Z = m_flZ;

            arrPoints3D[19].X = arrPoints3D[12].X;
            arrPoints3D[19].Y = arrPoints3D[12].Y;
            arrPoints3D[19].Z = m_flZ;

            arrPoints3D[20].X = arrPoints3D[15].X;
            arrPoints3D[20].Y = arrPoints3D[15].Y;
            arrPoints3D[20].Z = m_flZ;

            arrPoints3D[21].X = arrPoints3D[5].X;
            arrPoints3D[21].Y = arrPoints3D[5].Y;
            arrPoints3D[21].Z = m_flZ;

            arrPoints3D[22].X = arrPoints3D[6].X;
            arrPoints3D[22].Y = arrPoints3D[6].Y;
            arrPoints3D[22].Z = m_flZ;

            arrPoints3D[23].X = arrPoints3D[7].X;
            arrPoints3D[23].Y = arrPoints3D[7].Y;
            arrPoints3D[23].Z = m_flZ;

            arrPoints3D[24].X = arrPoints3D[8].X;
            arrPoints3D[24].Y = arrPoints3D[8].Y;
            arrPoints3D[24].Z = m_flZ;

            arrPoints3D[25].X = arrPoints3D[16].X;
            arrPoints3D[25].Y = arrPoints3D[16].Y;
            arrPoints3D[25].Z = m_flZ;
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 11, 10);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 12, 11);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 13, 12);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 12, 19, 18);

            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 23, 22);

            // Top
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 17, 16, 8);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 16, 25, 24);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 21, 20, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 15, 14, 4);

            // Front
            AddRectangleIndices_CCW_1234(TriangleIndices, 10, 11, 16, 17);
            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 13, 14, 15);

            AddRectangleIndices_CCW_1234(TriangleIndices, 18, 23, 24, 25);
            AddRectangleIndices_CCW_1234(TriangleIndices, 18, 19, 22, 23);
            AddRectangleIndices_CCW_1234(TriangleIndices, 19, 20, 21, 22);

            // Back
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 2, 3);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 0, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 1, 2);

            // Side
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 10, 17, 9);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 18, 25, 16);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 24, 23, 7);

            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 6, 22, 21);
            AddRectangleIndices_CCW_1234(TriangleIndices, 20, 19, 12, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 14, 13);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            // z = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[0]);

            // z = t
            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[16]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[15]);

            // z = L
            wireFrame.Points.Add(arrPoints3D[18]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[19]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[20]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[21]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[22]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[23]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[24]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[25]);
            wireFrame.Points.Add(arrPoints3D[18]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[23]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[17]);

            return wireFrame;
        }
    }
}
