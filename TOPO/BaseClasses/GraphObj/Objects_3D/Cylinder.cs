using MATH;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    // Cylinder in local x-axis
    public class Cylinder : CVolume
    {
        public float m_fDim1_r;
        public float m_fDim2_L;
        DiffuseMaterial m_mat;

        //short iTotNoPoints = 13; // 1 auxialiary node in centroid / stredovy bod

        public Cylinder()
        { }
        public Cylinder(float fDim1_r, float fDim2_L, DiffuseMaterial mat)
        {
            m_fDim1_r = fDim1_r;
            m_fDim2_L = fDim2_L;
            m_mat = mat;
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of cylinder
        //--------------------------------------------------------------------------------------------
        public static Int32Collection GetWireFrameIndices_Cylinder(short nPoints)
        {
            Int32Collection wireFrameIndices = new Int32Collection();

            short iTotNoPoints = nPoints; // 13 // 1 auxialiary node in centroid / stredovy bod

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
        public static GeometryModel3D CreateM_G_M_3D_Volume_Cylinder(Point3D solidControlEdge, short nPoints, float fDim1_r, float fDim2_h, DiffuseMaterial mat, int iPrimaryModelDirection = 2, bool bDrawTop = true, bool bDrawBottom = true)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            short iTotNoPoints = nPoints; // 73 // 1 auxialiary node in centroid / stredovy bod

            if (fDim1_r <= 0f)
            {
                // Exception
                //return;
            }

            // Create Array - allocate memory
            float[,] PointsOut = new float[iTotNoPoints - 1, 2];

            // Outside Points Coordinates
            PointsOut = Geom2D.GetCirclePointCoordArray_CW(fDim1_r, iTotNoPoints - 1);

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

            // Bottom  h = 0
            for (int i = 0; i < iTotNoPoints; i++)
            {
                Point3D pPrimary = CVolume.GetPointinLCS(iPrimaryModelDirection, PointsOut[i, 0], PointsOut[i, 1], 0);
                Point3D p = new Point3D(solidControlEdge.X + pPrimary.X, solidControlEdge.Y + pPrimary.Y, solidControlEdge.Z + pPrimary.Z);
                meshGeom3D.Positions.Add(p);
            }

            // Top h = xxx
            for (int i = 0; i < iTotNoPoints; i++)
            {
                Point3D pPrimary = CVolume.GetPointinLCS(iPrimaryModelDirection, PointsOut[i, 0], PointsOut[i, 1], fDim2_h);
                Point3D p = new Point3D(solidControlEdge.X + pPrimary.X, solidControlEdge.Y + pPrimary.Y, solidControlEdge.Z + pPrimary.Z);
                meshGeom3D.Positions.Add(p);
            }

            Int32Collection TriangleIndices = new Int32Collection();

            if (bDrawBottom == true)
            {
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
            }

            if (bDrawTop == true)
            {
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

            return geomModel3D;
        }
    }
}
