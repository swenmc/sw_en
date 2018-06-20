using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using MATH;

namespace BaseClasses
{
    public class CConCom_Plate_BB_BG : CPlate
    {
        public float m_fbX;
        public float m_fhY;
        public float m_flZ; // Not used in 2D model
        public float m_ft; // Not used in 2D model

        float m_fRotationX_deg = 0;
        float m_fRotationY_deg = 0;
        float m_fRotationZ_deg = 0;

        float m_fDistanceBetweenHoles;

        int iNoPoints2Dfor3D;

        public CConCom_Plate_BB_BG()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_BB_BG(GraphObj.CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, int iHolesNumber_temp, float fHoleDiameter_temp, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            m_pControlPoint = controlpoint;
            m_fbX = fbX_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber_temp = 2;
            FHoleDiameter = fHoleDiameter_temp;

            m_fDistanceBetweenHoles = 0.5f * m_fhY;
            ITotNoPointsin2D = 8;

            iNoPoints2Dfor3D = ITotNoPointsin2D + IHolesNumber * 4 + IHolesNumber * INumberOfPointsOfHole;
            ITotNoPointsin3D = 2 * iNoPoints2Dfor3D;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber, 2];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D();

            // Fill list of indices for drawing of surface
            loadIndices();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_flZ;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = PointsOut2D[1, 0] + m_fbX;
            PointsOut2D[2, 1] = 0;

            PointsOut2D[3, 0] = PointsOut2D[2, 0] + m_flZ;
            PointsOut2D[3, 1] = 0;

            PointsOut2D[4, 0] = PointsOut2D[3, 0];
            PointsOut2D[4, 1] = m_fhY;

            PointsOut2D[5, 0] = PointsOut2D[2, 0];
            PointsOut2D[5, 1] = m_fhY;

            PointsOut2D[6, 0] = PointsOut2D[1, 0];
            PointsOut2D[6, 1] = m_fhY;

            PointsOut2D[7, 0] = PointsOut2D[0, 0];
            PointsOut2D[7, 1] = m_fhY;
        }

        void Calc_Coord3D()
        {
            float[,] holesCentersPointsfor3D = new float[IHolesNumber, 2];

            holesCentersPointsfor3D[0, 0] = 0.5f * m_fbX;
            holesCentersPointsfor3D[0, 1] = 0.5f * m_fhY - 0.5f * m_fDistanceBetweenHoles;

            holesCentersPointsfor3D[1, 0] = 0.5f * m_fbX;
            holesCentersPointsfor3D[1, 1] = 0.5f * m_fhY + 0.5f * m_fDistanceBetweenHoles;

            float fradius = 0.5f * FHoleDiameter;
            int iRadiusAngle = 360; // Angle


            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = arrPoints3D[0].X;
            arrPoints3D[1].Y = arrPoints3D[0].Y;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX;
            arrPoints3D[2].Y = arrPoints3D[0].Y;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[0].Y;
            arrPoints3D[3].Z = arrPoints3D[0].Z;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[1].X;
            arrPoints3D[6].Y = arrPoints3D[4].Y;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = arrPoints3D[4].Y;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            // Points in holes square edges

            arrPoints3D[8].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[8].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[8].Z = 0;

            arrPoints3D[9].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[9].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[9].Z = 0;

            arrPoints3D[10].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[10].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[10].Z = 0;

            arrPoints3D[11].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[11].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[11].Z = 0;

            arrPoints3D[12].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[12].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[12].Z = 0;

            arrPoints3D[13].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[13].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[13].Z = 0;

            arrPoints3D[14].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[14].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[14].Z = 0;

            arrPoints3D[15].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[15].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[15].Z = 0;

            // Holes 1
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].X = holesCentersPointsfor3D[0, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].Y = holesCentersPointsfor3D[0, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].Z = 0;
            }

            // Hole 2
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].X = holesCentersPointsfor3D[1, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Y = holesCentersPointsfor3D[1, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Z = 0;
            }


            arrPoints3D[iNoPoints2Dfor3D + 0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = arrPoints3D[0].X;
            arrPoints3D[1].Y = arrPoints3D[0].Y;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX;
            arrPoints3D[2].Y = arrPoints3D[0].Y;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[0].Y;
            arrPoints3D[3].Z = arrPoints3D[0].Z;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[1].X;
            arrPoints3D[6].Y = arrPoints3D[4].Y;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = arrPoints3D[4].Y;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            // Points in holes square edges

            arrPoints3D[8].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[8].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[8].Z = 0;

            arrPoints3D[9].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[9].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[9].Z = 0;

            arrPoints3D[10].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[10].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[10].Z = 0;

            arrPoints3D[11].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[11].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[11].Z = 0;

            arrPoints3D[12].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[12].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[12].Z = 0;

            arrPoints3D[13].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[13].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[13].Z = 0;

            arrPoints3D[14].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[14].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[14].Z = 0;

            arrPoints3D[15].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[15].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[15].Z = 0;

            // Holes 1
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].X = holesCentersPointsfor3D[0, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].Y = holesCentersPointsfor3D[0, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].Z = 0;
            }

            // Hole 2
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].X = holesCentersPointsfor3D[1, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Y = holesCentersPointsfor3D[1, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Z = 0;
            }
        }

        void Calc_HolesCentersCoord2D()
        {
            HolesCentersPoints2D[0, 0] = m_flZ + 0.5f * m_fbX;
            HolesCentersPoints2D[0, 1] = 0.5f * m_fhY - 0.5f * m_fDistanceBetweenHoles;

            HolesCentersPoints2D[1, 0] = HolesCentersPoints2D[0, 0];
            HolesCentersPoints2D[1, 1] = 0.5f * m_fhY + 0.5f * m_fDistanceBetweenHoles;
        }

        protected override void loadIndices()
        {
            int secNum = 6;
            TriangleIndices = new Int32Collection();

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 5, 4, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 3, 4);
            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 10, 11);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7, 8, 9, 10);

            // Shell Surface
            DrawCaraLaterals_CW(secNum, TriangleIndices);
        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            foreach (Point3D point in arrPoints3D)
                pMeshPositions.Add(point);

            return pMeshPositions;
        }

        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;

            model.Material = new DiffuseMaterial(brush);  // Set Model Material

            TransformPlateCoord(model, m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg); // Not used now

            return model;
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
