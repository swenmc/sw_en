using _3DTools;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATERIAL;
using System;

namespace BaseClasses
{
    [Serializable]
    public abstract class CConnectionComponentEntity3D : CEntity3D
    {
        [NonSerialized]
        private Int32Collection m_TriangleIndices;

        public Int32Collection TriangleIndices
        {
            get { return m_TriangleIndices; }
            set { m_TriangleIndices = value; }
        }        
        public Point[] PointsOut2D;
        /*
        public Point[] m_arrPoints2D;
        public Point[] PointsOut2D
        {
            get { return m_arrPoints2D; }
            set { m_arrPoints2D = value; }
        }
        */
        [NonSerialized]
        private Point3D[] m_arrPoints3D;
        
        public Point3D[] arrPoints3D
        {
            get { return m_arrPoints3D; }
            set { m_arrPoints3D = value; }
        }

        [NonSerialized]
        public EConnectionComponentType eConnComponentType;

        //[NonSerialized]
        //public new CMat_03_00 m_Mat; // Pre Connection Component predefinovat material z obecneho na STEEL 03_00

        public CConnectionComponentEntity3D()
        {
            m_Mat = new CMat_03_00();
        }
        protected abstract void loadIndices();
        //public abstract void loadWireFrameIndices();

        protected abstract Point3DCollection GetDefinitionPoints();
        public abstract GeometryModel3D CreateGeomModel3D(SolidColorBrush brush);
        public abstract ScreenSpaceLines3D CreateWireFrameModel();

        public void LoadIndicesPrismWithOpening(int iNumberOfPointsInExternalOutline)
        {
            // TODO - premenovat premenne a toto nastavovanie odstranit
            int secNum = iNumberOfPointsInExternalOutline;
            int INoPoints2Dfor3D = 2 * secNum;

            TriangleIndices = new Int32Collection();

            // Bottom Side
            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, i, secNum + i, secNum + i + 1, i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, i, secNum + i, secNum + 0, 0);
            }

            // Top Side
            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CCW_1234(TriangleIndices, INoPoints2Dfor3D + i, INoPoints2Dfor3D + secNum + i, INoPoints2Dfor3D + secNum + i + 1, INoPoints2Dfor3D + i + 1);
                else
                    AddRectangleIndices_CCW_1234(TriangleIndices, INoPoints2Dfor3D + i, INoPoints2Dfor3D + secNum + i, INoPoints2Dfor3D + secNum + 0, INoPoints2Dfor3D + 0);
            }

            // Shell Surface
            // External surface
            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, 2 * secNum + i, 2 * secNum + i + 1, i + 1);
                else
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, 2 * secNum + i, 2 * secNum + 0, 0);
            }

            // Internal Surface
            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, secNum + i, secNum + 2 * secNum + i, secNum + 2 * secNum + i + 1, secNum + i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, secNum + i, secNum + 2 * secNum + i, secNum + 2 * secNum + 0, secNum + 0);
            }
        }

        public ScreenSpaceLines3D CreateWireFrameModel(int iEdgesOutBasic, int iNumberOfSegmentsPerSideOut, bool bApplyFirstPointsTransfromation)
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            int INoPoints2Dfor3D = 2 * iEdgesOutBasic * iNumberOfSegmentsPerSideOut; // Pocet bodov v zakladni hranola (vnutorne aj vonkajsie)

            int iNumberOfPointsPerPolygon = INoPoints2Dfor3D / 2;

            // z = 0
            // External Outline
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                if (i < iNumberOfPointsPerPolygon - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[i]);
                    wireFrame.Points.Add(arrPoints3D[i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[i]);
                    wireFrame.Points.Add(arrPoints3D[0]);
                }
            }

            // Internal Outline
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                if (i < iNumberOfPointsPerPolygon - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[iNumberOfPointsPerPolygon + i]);
                    wireFrame.Points.Add(arrPoints3D[iNumberOfPointsPerPolygon + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[iNumberOfPointsPerPolygon + i]);
                    wireFrame.Points.Add(arrPoints3D[iNumberOfPointsPerPolygon + 0]);
                }
            }

            // z = t
            // External Outline
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                if (i < iNumberOfPointsPerPolygon - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + i]);
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + i]);
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + 0]);
                }
            }

            // Internal Outline
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                if (i < iNumberOfPointsPerPolygon - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + iNumberOfPointsPerPolygon + i]);
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + iNumberOfPointsPerPolygon + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + iNumberOfPointsPerPolygon + i]);
                    wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + iNumberOfPointsPerPolygon + 0]);
                }
            }

            // Lateral
            // External Outline
            int iPosunIndexovBodov = bApplyFirstPointsTransfromation ? iNumberOfSegmentsPerSideOut / 2 : 0; // Posunieme indexy o polovicu poctu bodov na strane

            for (int i = 0; i < iEdgesOutBasic; i++) // Len 4 rohove body
            {
                wireFrame.Points.Add(arrPoints3D[i * iNumberOfSegmentsPerSideOut + iPosunIndexovBodov]);
                wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + (i * iNumberOfSegmentsPerSideOut + iPosunIndexovBodov)]);
            }

            // Internal Outline
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                wireFrame.Points.Add(arrPoints3D[iNumberOfPointsPerPolygon + i]);
                wireFrame.Points.Add(arrPoints3D[INoPoints2Dfor3D + iNumberOfPointsPerPolygon + i]);
            }

            return wireFrame;
        }

        // POMOCNE FUNKCIE
        // REFAKTOROVAT NAPRIEC KODOM S CRSC, VOLUME ATD

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
    }
}
