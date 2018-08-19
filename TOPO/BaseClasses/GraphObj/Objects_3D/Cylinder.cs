using MATH;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    // Cylinder in local x-axis
    public class Cylinder
    {
        public float m_fDim1_r;
        public float m_fDim2_L;
        DiffuseMaterial m_mat;

        short iTotNoPoints = 73; // 1 auxialiary node in centroid / stredovy bod

        public Cylinder()
        { }
        public Cylinder(float fDim1_r, float fDim2_L, DiffuseMaterial mat)
        {
            m_fDim1_r = fDim1_r;
            m_fDim2_L = fDim2_L;
            m_mat = mat;
        }


        public Cylinder(short iNumberOfPointsWithCentroid, float fDim1_r, float fDim2_L, DiffuseMaterial mat)
        {
            iTotNoPoints = iNumberOfPointsWithCentroid; // Odd number - include centroid
            m_fDim1_r = fDim1_r;
            m_fDim2_L = fDim2_L;
            m_mat = mat;
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of cylinder
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateM_G_M_3D_Volume_Cylinder(Point3D solidControlEdge, short iNumberPoints, float fDim1_r, float fDim2_L, DiffuseMaterial mat)
        {
            iTotNoPoints = iNumberPoints;  // Odd number - include centroid
            return CreateM_G_M_3D_Volume_Cylinder(solidControlEdge, fDim1_r, fDim2_L, mat);
        }
        public GeometryModel3D CreateM_G_M_3D_Volume_Cylinder(Point3D solidControlEdge, float fDim1_r, float fDim2_L, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            if (fDim1_r <= 0f)
            {
                // Exception
                //return;
            }

            // Create Array - allocate memory
            float[,] PointsOut = new float[iTotNoPoints - 1, 2];

            // Outside Points Coordinates
            PointsOut = Geom2D.GetCirclePointCoord(fDim1_r, iTotNoPoints - 1);

            // TODO - potrebujeme zmenit velkost dvojrozmerneho pola a pridat don posledny bod - stredovy bod kruhu
            float[,] PointsOutTemp = PointsOut;

            PointsOut = new float[iTotNoPoints, 2];

            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                PointsOut[i, 0] = PointsOutTemp[i, 0];
                PointsOut[i, 1] = PointsOutTemp[i, 1];
            }

            // Centroid
            PointsOut[iTotNoPoints - 1, 0] = 0f;
            PointsOut[iTotNoPoints - 1, 1] = 0f;

            meshGeom3D.Positions = new Point3DCollection();

            // TODO Ondrej 15/07/2018
            // Vytvorime valec, ktory ma vysku v smere suradnice x

            // Bottom  x = 0
            for (int i = 0; i < iTotNoPoints; i++)
            {
                Point3D p = new Point3D(0, PointsOut[i, 0],  PointsOut[i, 1]);
                meshGeom3D.Positions.Add(p);
            }

            // Top L = xxx
            for (int i = 0; i < iTotNoPoints; i++)
            {
                Point3D p = new Point3D(fDim2_L, PointsOut[i, 0], PointsOut[i, 1]);
                meshGeom3D.Positions.Add(p);
            }

            Int32Collection TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                if (i < iTotNoPoints - 2)
                {
                    TriangleIndices.Add(i + 1);
                    TriangleIndices.Add(iTotNoPoints - 1);
                    TriangleIndices.Add(i);
                }
                else // Last Element
                {
                    TriangleIndices.Add(0);
                    TriangleIndices.Add(iTotNoPoints - 1);
                    TriangleIndices.Add(i);
                }
            }

            // Back Side
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                if (i < iTotNoPoints - 2)
                {
                    TriangleIndices.Add(iTotNoPoints + iTotNoPoints - 1);
                    TriangleIndices.Add(iTotNoPoints + i + 1);
                    TriangleIndices.Add(iTotNoPoints + i);
                }
                else // Last Element
                {
                    TriangleIndices.Add(iTotNoPoints + iTotNoPoints - 1);
                    TriangleIndices.Add(iTotNoPoints);
                    TriangleIndices.Add(iTotNoPoints + i);
                }
            }

            // Shell Surface OutSide
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                if (i < iTotNoPoints - 2)
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, iTotNoPoints + i, iTotNoPoints + i + 1, i + 1);
                else
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, iTotNoPoints + i, iTotNoPoints, 0); // Last Element

            }

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            TranslateTransform3D translate = new TranslateTransform3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z);

            geomModel3D.Transform = translate;

            return geomModel3D;
        }
        public Int32Collection GetWireFrameIndices_Cylinder(short nPoints)
        {
            Int32Collection wireFrameIndices = new Int32Collection();

            short iTotNoPoints = nPoints; // 73 // 1 auxialiary node in centroid / stredovy bod

            // Front Side / Forehead
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                if (i < iTotNoPoints - 2)
                {
                    wireFrameIndices.Add(i + 1);
                    wireFrameIndices.Add(i);
                }
                else // Last Element
                {
                    wireFrameIndices.Add(i);
                    wireFrameIndices.Add(0);
                }
            }

            // Back Side
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                if (i < iTotNoPoints - 2)
                {
                    wireFrameIndices.Add(iTotNoPoints + i + 1);
                    wireFrameIndices.Add(iTotNoPoints + i);
                }
                else // Last Element
                {
                    wireFrameIndices.Add(iTotNoPoints);
                    wireFrameIndices.Add(iTotNoPoints + i);
                }
            }

            // Shell Surface OutSide
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                wireFrameIndices.Add(i);
                wireFrameIndices.Add(iTotNoPoints + i);
            }

            return wireFrameIndices;
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
    }
}
