using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace BaseClasses
{
    public abstract class CPlate : CConnectionComponentEntity3D
    {
        public ESerieTypePlate m_ePlateSerieType_FS; // Type of plate - FormSteel
        public float fWidth_bx;
        public float fHeight_hy;
        public float fThickness_tz;
        public float fArea;
        public CConnector[] m_arrPlateConnectors;

        public float m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg;

        public CPlate()
        {
            BIsDisplayed = true;
            m_Mat = new CMat();
        }

        public CPlate(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
            m_Mat = new CMat();
        }

        protected override void loadIndices()
        { }

        public int ITotNoPointsin3D; // Number of all points in 3D (excluding auxiliary)
        public int ITotNoPointsin2D; // Number of all points in 2D (excluding auxiliary)
        //public float[,] PointsOut2D; // Array of points coordinates of plate outline in 2D used for DXF
        public int IHolesNumber;   // Number of holes
        public float[,] HolesCentersPoints2D; // Array of points coordinates of holes centers
        public float FHoleDiameter;
        public int INumberOfPointsOfHole = 12; // Have to be Even - Todo funguje pre 12 bodov, napr. pre 24 je tam chyba, je potrebne "doladit"
        public Point3D [] arrConnectorControlPoints3D; // Array of control points for inserting connectors (bolts, screws, anchors, ...)

        public int INoPoints2Dfor3D; // Number of points in one surface used for 3D model (holes lines are divided to the straight segments)

        // TODO - zjednotit funkcie s triedou CCRSC

        // Draw Hexagon / Add hexagon indices - clockwise CW numbering of input points 1,2,3,4,5,6 (see scheme)
        // Add in order 1,2,3,4,5,6
        protected void AddHexagonIndices_CW_123456(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4,
              int point5, int point6)
        {
            // Main numbering is clockwise

            //   6  _  1
            // 5  /   \  2
            //   |_____|
            // 4         3

            // Triangles Numbering is Counterclockwise

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point5);
            Indices.Add(point4);

            Indices.Add(point1);
            Indices.Add(point6);
            Indices.Add(point5);
        }

        // Draw Hexagon / Add hexagon indices - countrer-clockwise CCW numbering of input points 1,2,3,4,5,6 (see scheme)
        // Add in order 1,6,5,4,3,2
        protected void AddHexagonIndices_CCW_123456(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4,
              int point5, int point6)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            //   6  _  1
            // 5  /   \  2
            //   |_____|
            // 4         3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point4);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point5);

            Indices.Add(point1);
            Indices.Add(point5);
            Indices.Add(point6);
        }

        // Draw Penthagon / Add penthagon indices - clockwise CW numbering of input points 1,2,3,4,5 (see scheme)
        // Add in order 1,2,3,4,5
        protected void AddPenthagonIndices_CW_12345(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4, int point5)
        {
            // Main numbering is clockwise

            //     1
            // 5  / \  2
            //   |___|
            // 4       3

            // Triangles Numbering is Counterclockwise

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point5);
            Indices.Add(point4);
        }

        // Draw Penthagon / Add pengthagon indices - countrer-clockwise CCW numbering of input points 1,2,3,4,5 (see scheme)
        // Add in order 1,5,4,3,2
        protected void AddPenthagonIndices_CCW_12345(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4, int point5)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            //     1
            // 5  / \  2
            //   |___|
            // 4       3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point4);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point5);
        }

        // Draw Rectangle / Add rectangle indices - clockwise CW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,2,3,4
        protected void AddRectangleIndices_CW_1234(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4)
        {
            // Main numbering is clockwise

            // 1  _______  2
            //   |_______| 
            // 4           3

            // Triangles Numbering is Counterclockwise
            // Top Right
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);

            // Bottom Left
            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point3);
        }

        // Draw Rectangle / Add rectangle indices - countrer-clockwise CCW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,4,3,2
        protected void AddRectangleIndices_CCW_1234(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            // 1  _______  2
            //   |_______| 
            // 4           3

            // Triangles Numbering is Clockwise
            // Top Right
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);

            // Bottom Left
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point4);
        }

        // Draw Triangle / Add triangle indices - clockwise CW numbering of input points 1,2,3 (see scheme)
        // Add in order 1,2,3,4
        protected void AddTriangleIndices_CW_123(Int32Collection Indices,
              int point1, int point2,
              int point3)
        {
            // Main numbering is clockwise

            // 1  _______  2
            //           | 
            //             3

            // Triangle Numbering is Counterclockwise
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);
        }

        // Draw Triangle / Add triangle indices - countrer-clockwise CCW numbering of input points 1,2,3 (see scheme)
        // Add in order 1,3,2
        protected void AddTriangleIndices_CCW_123(Int32Collection Indices,
              int point1, int point2,
              int point3)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            // 1  _______  2
            //           | 
            //             3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre kontinualne pravidelne cislovanie bodov
        protected void DrawCaraLaterals_CW(int secNum, Int32Collection Indices)
        {
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CW_1234(Indices, i, secNum + i, secNum + i + 1, i + 1);
                else
                    AddRectangleIndices_CW_1234(Indices, i, secNum + i, secNum, 0); // Last Element
            }
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre pravidelne cislovanie bodov s vynechanim pociatocnych uzlov - pomocne
        protected void DrawCaraLaterals_CW(int iAuxNum, int secNum, Int32Collection Indices)
        {
            // iAuxNum - number of auxiliary points - start ofset
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy (tento pocet neobsahuje pomocne body iAuxNum)

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum + i + 1, iAuxNum + i + 1);
                else
                    AddRectangleIndices_CW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum, iAuxNum + 0); // Last Element
            }
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre kontinualne pravidelne cislovanie bodov
        protected void DrawCaraLaterals_CCW(int secNum, Int32Collection Indices)
        {
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CCW_1234(Indices, i, secNum + i, secNum + i + 1, i + 1);
                else
                    AddRectangleIndices_CCW_1234(Indices, i, secNum + i, secNum, 0); // Last Element
            }
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre pravidelne cislovanie bodov s vynechanim pociatocnych uzlov - pomocne
        protected void DrawCaraLaterals_CCW(int iAuxNum, int secNum, Int32Collection Indices)
        {
            // iAuxNum - number of auxiliary points - start ofset
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy (tento pocet neobsahuje pomocne body iAuxNum)

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CCW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum + i + 1, iAuxNum + i + 1);
                else
                    AddRectangleIndices_CCW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum, iAuxNum + 0); // Last Element
            }
        }

        public void TransformPlateCoord(GeometryModel3D model)
        {
            model.Transform = CreateTransformCoordGroup();
        }

        public void TransformPlateCoord(ScreenSpaceLines3D wireframeModel)
        {
            wireframeModel.Transform = CreateTransformCoordGroup();
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate Plate from its cs to joint cs system in LCS of member or GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), m_fRotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_fRotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ_deg); // Rotation in degrees

            // Move 0,0,0 to control point in LCS of member or GCS
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
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

            TransformPlateCoord(model);

            return model;
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D ssl3D = new ScreenSpaceLines3D();
            return ssl3D;
        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            foreach (Point3D point in arrPoints3D)
                pMeshPositions.Add(point);

            return pMeshPositions;
        }

        public void ChangeIndices(Int32Collection TriangleIndices)
        {
            if (TriangleIndices != null && TriangleIndices.Count > 0)
            {
                int iSecond = 1;
                int iThird = 2;

                int iTIcount = TriangleIndices.Count;
                for (int i = 0; i < iTIcount / 3; i++)
                {
                    int iTI_2 = TriangleIndices[iSecond];
                    int iTI_3 = TriangleIndices[iThird];

                    TriangleIndices[iThird] = iTI_2;
                    TriangleIndices[iSecond] = iTI_3;

                    iSecond += 3;
                    iThird += 3;
                }
            }
        }

        // Return the polygon's area in "square units."
        // The value will be negative if the polygon is
        // oriented clockwise.

        private float SignedPolygonArea()
        {
            // Add the first point to the end.
            int num_points = PointsOut2D.Length / 2;
            Point[] pts = new Point[num_points + 1];

            for (int i = 0; i < num_points; i++)
            {
                pts[i].X = PointsOut2D[i, 0];
                pts[i].Y = PointsOut2D[i, 1];
            }

            pts[num_points].X = PointsOut2D[0, 0];
            pts[num_points].Y = PointsOut2D[0, 1];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area += (float)(
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2);
            }

            // Return the result.
            return area;
        }

        // Return the polygon's area in "square units."
        public float PolygonArea()
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polyogn is
            // oriented clockwise.
            return Math.Abs(SignedPolygonArea());
        }
    }
}
