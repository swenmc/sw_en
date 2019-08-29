using MATH;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    public class CHollowCylinder
    {
        Int32Collection TriangleIndices;

        public float m_fr_in;
        public float m_fr_out;
        public float m_fL;
        DiffuseMaterial m_mat;

        short iTotNoPoints = 73; // 1 auxialiary node in centroid / stredovy bod

        public CHollowCylinder(float fr_in, float fr_out, float fL, DiffuseMaterial mat)
        {
            m_fr_in = fr_in;
            m_fr_out = fr_out;
            m_fL = fL;
            m_mat = mat;
        }

        public CHollowCylinder(short iNumberOfPointsWithCentroid, float fr_in, float fr_out, float fL, DiffuseMaterial mat)
        {
            iTotNoPoints = iNumberOfPointsWithCentroid; // Odd number - include centroid
            m_fr_in = fr_in;
            m_fr_out = fr_out;
            m_fL = fL;
            m_mat = mat;
        }

        public GeometryModel3D CreateM_G_M_3D_Volume(Point3D solidControlEdge, float fr_in, float fr_out, float fL, DiffuseMaterial mat, int iPrimaryModelDirection = 2)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            if (m_fr_out <= 0f)
            {
                // Exception
                //return;
            }

            // Suradnice v 2D

            // Create Array - allocate memory
            float[,] PointsOut = new float[iTotNoPoints - 1, 2];

            // Outside Points Coordinates
            PointsOut = Geom2D.GetCirclePointCoordArray_CW(m_fr_out, iTotNoPoints - 1);

            float[,] PointsIn = new float[iTotNoPoints - 1, 2];

            // Inside Points
            PointsIn = Geom2D.GetCirclePointCoordArray_CW(m_fr_in, iTotNoPoints - 1);

            meshGeom3D.Positions = new Point3DCollection();

            // Vytvorime valec, ktory ma vysku v smere suradnice h

            // Bottom  h = 0
            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                Point3D pPrimary = GetPointinLCS(iPrimaryModelDirection, PointsOut[i, 0], PointsOut[i, 1], 0);
                Point3D p = new Point3D(solidControlEdge.X + pPrimary.X, solidControlEdge.Y + pPrimary.Y, solidControlEdge.Z + pPrimary.Z);
                meshGeom3D.Positions.Add(p);
            }

            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                Point3D pPrimary = GetPointinLCS(iPrimaryModelDirection, PointsIn[i, 0], PointsIn[i, 1], 0);
                Point3D p = new Point3D(solidControlEdge.X + pPrimary.X, solidControlEdge.Y + pPrimary.Y, solidControlEdge.Z + pPrimary.Z);
                meshGeom3D.Positions.Add(p);
            }

            // Top L = xxx
            for (int i = 0; i < iTotNoPoints-1; i++)
            {
                Point3D pPrimary = GetPointinLCS(iPrimaryModelDirection, PointsOut[i, 0], PointsOut[i, 1], fL);
                Point3D p = new Point3D(solidControlEdge.X + pPrimary.X, solidControlEdge.Y + pPrimary.Y, solidControlEdge.Z + pPrimary.Z);
                meshGeom3D.Positions.Add(p);
            }

            for (int i = 0; i < iTotNoPoints - 1; i++)
            {
                Point3D pPrimary = GetPointinLCS(iPrimaryModelDirection, PointsIn[i, 0], PointsIn[i, 1], fL);
                Point3D p = new Point3D(solidControlEdge.X + pPrimary.X, solidControlEdge.Y + pPrimary.Y, solidControlEdge.Z + pPrimary.Z);
                meshGeom3D.Positions.Add(p);
            }

            loadCrScIndices_26_28(iTotNoPoints - 1, 0);

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }

        // Refaktorovat s StraightLineArrow3D
        private static Point3D GetPointinLCS(int iPrimaryModelDirection, double dCoordX, double dCoordY, double dCoordZ)
        {
            Point3D p = new Point3D();
            // Nastavi suradnice uzla podla toho v akom smere sa ma valec primarne vykreslit

            // iPrimaryModelDirection Kod pre smer modelu valca v LCS(0 - X, 1 - Y, 2 - Z - default
            if (iPrimaryModelDirection == 0)
            {
                p.X = dCoordZ;
                p.Y = dCoordX;
                p.Z = dCoordY;
            }
            else if (iPrimaryModelDirection == 1)
            {
                p.X = dCoordX;
                p.Y = dCoordZ;
                p.Z = dCoordY;
            }
            else //if (iPrimaryModelDirection == 2)// Default (valec v smere Z)
            {
                p.X = dCoordX;
                p.Y = dCoordY;
                p.Z = dCoordZ;
            }

            return p;
        }

        // TOTO JE VSETKO SKOPIROVANE Z PRIEREZOV
        // Auxiliary, used for also other cross-sections

        public void loadCrScIndices_26_28(int iNoPoints, int iAux)
        {
            // iAux - number of auxiliary points in inside/outside collection of points
            // iNoPoints - numer of real points in inside/outside collection of points
            // iAux + iNoPoints - total number of points in inside/outside collection of section

            TriangleIndices = new Int32Collection(iNoPoints * 24);

            // Front Side / Forehead

            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, iAux + i + 1, iAux + i + (iAux + iNoPoints) + 1, iAux + i + (iAux + iNoPoints));
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, iAux + 0, iAux + i + iAux + 1, iAux + i + (iAux + iNoPoints)); // Last Element
            }

            // Back Side
            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints + 1, 2 * (iAux + iNoPoints) + iAux + i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints, 2 * (iAux + iNoPoints) + i + 2 * iAux + 1, 2 * (iAux + iNoPoints) + iAux + 0); // Last Element
            }

            // Shell Surface OutSide
            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + iAux + i + 1, iAux + i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + iAux, iAux + 0); // Last Element
            }

            // Shell Surface Inside
            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + iNoPoints + iAux + i, iAux + iNoPoints + iAux + i + 1, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints + 1, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * (iAux + iNoPoints) + 2 * iAux + i + 1, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints, iAux + iNoPoints + iAux + i, 2 * iAux + iNoPoints); // Last Element
            }
        }

        // Draw Rectangle / Add rectangle indices - countrer-clockwise CCW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,4,3,2
        void AddRectangleIndices_CCW_1234(Int32Collection Indices,
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

        // Draw Rectangle / Add rectangle indices - clockwise CW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,2,3,4
        void AddRectangleIndices_CW_1234(Int32Collection Indices,
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
    }
}