using MATH;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CVolume : CEntity3D
    {
        //public int[] m_iPointsCollection; // List / Collection of points IDs
        public int[] m_iPointsCollection; // List / Collection of points IDs

        public EVolumeShapeType m_eShapeType;

        public float m_fvolOpacity;
        public Color m_volColor_1 = new Color(); // Default
        public Color m_volColor_2 = new Color();

        [NonSerialized]
        public DiffuseMaterial m_Material_1 = new DiffuseMaterial();
        [NonSerialized]
        public DiffuseMaterial m_Material_2 = new DiffuseMaterial();
        [NonSerialized]
        public GeometryModel3D Visual_Object;

        public float m_fDim1;
        public float m_fDim2;
        public float m_fDim3;
        public float m_fDim4;

        public float m_fVolume;

        private List<Point3D> m_WireFramePoints;
        public List<Point3D> WireFramePoints
        {
            get
            {
                if (m_WireFramePoints == null) m_WireFramePoints = new List<Point3D>();
                return m_WireFramePoints;
            }

            set
            {
                m_WireFramePoints = value;
            }
        }

        // Constructor 1
        public CVolume()
        {


        }

        // Constructor 2
        public CVolume(int iVolume_ID, int[] iPCollection, int fTime)
        {
            ID = iVolume_ID;
            m_iPointsCollection = iPCollection;
            FTime = fTime;
        }

        // Constructor 3
        // Rectangular Prism 8 Edges
        public CVolume(int iVolume_ID, EVolumeShapeType iShapeType, Point3D pControlEdgePoint, float fX, float fY, float fZ, Color volColor, float fvolOpacity, bool bIsDisplayed, float fTime)
        {
            ID = iVolume_ID;
            m_eShapeType = iShapeType;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            m_Material_2 = new DiffuseMaterial(new SolidColorBrush(m_volColor_2));
        }

        // Constructor 4
        // Rectangular Prism 8 Edges
        public CVolume(int iVolume_ID, EVolumeShapeType iShapeType, Point3D pControlEdgePoint, float fX, float fY, float fZ, DiffuseMaterial volMat1, DiffuseMaterial volMat2, bool bIsDisplayed, float fTime)
        {
            ID = iVolume_ID;
            m_eShapeType = iShapeType;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_fVolume = fX * fY * fZ; // !!! plati len pre kvader
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            m_Material_2 = volMat2;
            m_volColor_2 = volMat2.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            /*MObject3DModel = */CreateM_3D_G_Volume_8Edges(new Point3D(pControlEdgePoint.X, pControlEdgePoint.Y, pControlEdgePoint.Z), fX, fY, fZ, volMat1, volMat2);
        }

        // Constructor 5a
        // Rectangular Prism 8 Edges
        public CVolume(int iVolume_ID, EVolumeShapeType iShapeType, Point3D pControlEdgePoint, float fX, float fY, float fZ, DiffuseMaterial volMat1, bool bIsDisplayed, float fTime)
        {
            ID = iVolume_ID;
            m_eShapeType = iShapeType;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_fVolume = fX * fY * fZ; // !!! plati len pre kvader
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            // Set same properties for both materials
            m_Material_2 = volMat1;
            m_volColor_2 = volMat1.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            /*MObject3DModel =*/ CreateM_3D_G_Volume_8Edges(new Point3D(pControlEdgePoint.X, pControlEdgePoint.Y,pControlEdgePoint.Z), fX, fY, fZ, volMat1, volMat1);
        }

        // Constructor 5b
        public CVolume(int iVolume_ID, EVolumeShapeType iShapeType, Point3D pControlEdgePoint, float fX, float fY, float fZ, MaterialGroup matGroup, bool bIsDisplayed, float fTime)
        {
            ID = iVolume_ID;
            m_eShapeType = iShapeType;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_fVolume = fX * fY * fZ; // !!! plati len pre kvader
            //m_Material_1 = volMat1;
            //m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            // Set same properties for both materials
            //m_Material_2 = volMat1;
            //m_volColor_2 = volMat1.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            /*MObject3DModel =*/
            CreateM_3D_G_Volume_8Edges(new Point3D(pControlEdgePoint.X, pControlEdgePoint.Y, pControlEdgePoint.Z), fX, fY, fZ, matGroup);
        }

        // Constructor 7
        // SquarePyramid
        public CVolume(int iVolume_ID, EVolumeShapeType iShapeType, Point3D pControlCenterPoint, float fa, float fh, DiffuseMaterial volMat1, bool bIsDisplayed, float fTime)
        {
            ID = iVolume_ID;
            m_eShapeType = iShapeType;
            ControlPoint = pControlCenterPoint;
            m_fDim1 = fa;
            m_fDim2 = fh;
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            // Set same properties for both materials
            m_Material_2 = volMat1;
            m_volColor_2 = volMat1.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            CreateM_G_M_3D_Volume_5Edges(new Point3D(pControlCenterPoint.X, pControlCenterPoint.Y, pControlCenterPoint.Z), fa, fh, volMat1);
        }

        //--------------------------------------------------------------------------------------------
        // Create 2D rectangle as GeometryModel3D - one material of rectangle can be defined
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateRectangle(Point3D point1, Point3D point2, Point3D point3, Point3D point4, DiffuseMaterial DiffMat)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);
            mesh.Positions.Add(point4);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            return new GeometryModel3D(mesh, DiffMat);
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
        public static void AddRectangleIndices_CCW_1234(Int32Collection Indices,
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

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of rectangular prism
        // Only one material of solid - NEFUNGUJE SPRAVNE TEXTURA
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateGM_3D_Volume_8EdgesOld(Point3D solidControlEdge, float fDim1, float fDim2, float fDim3, DiffuseMaterial mat)
        {
            Point3D p0 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p2 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p3 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p4 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p5 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p6 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);
            Point3D p7 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);

            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            meshGeom3D.Positions = new Point3DCollection();

            meshGeom3D.Positions.Add(p0);
            meshGeom3D.Positions.Add(p1);
            meshGeom3D.Positions.Add(p2);
            meshGeom3D.Positions.Add(p3);
            meshGeom3D.Positions.Add(p4);
            meshGeom3D.Positions.Add(p5);
            meshGeom3D.Positions.Add(p6);
            meshGeom3D.Positions.Add(p7);

            Int32Collection TriangleIndices = new Int32Collection();

            //Bottom
            TriangleIndices.Add(3);
            TriangleIndices.Add(2);
            TriangleIndices.Add(1);

            TriangleIndices.Add(3);
            TriangleIndices.Add(1);
            TriangleIndices.Add(0);


            // Top
            TriangleIndices.Add(4);
            TriangleIndices.Add(5);
            TriangleIndices.Add(6);

            TriangleIndices.Add(4);
            TriangleIndices.Add(6);
            TriangleIndices.Add(7);

            // Side
            TriangleIndices.Add(0);
            TriangleIndices.Add(1);
            TriangleIndices.Add(5);

            TriangleIndices.Add(0);
            TriangleIndices.Add(5);
            TriangleIndices.Add(4);

            TriangleIndices.Add(1);
            TriangleIndices.Add(2);
            TriangleIndices.Add(6);

            TriangleIndices.Add(1);
            TriangleIndices.Add(6);
            TriangleIndices.Add(5);

            TriangleIndices.Add(2);
            TriangleIndices.Add(3);
            TriangleIndices.Add(7);

            TriangleIndices.Add(2);
            TriangleIndices.Add(7);
            TriangleIndices.Add(6);

            TriangleIndices.Add(3);
            TriangleIndices.Add(0);
            TriangleIndices.Add(4);

            TriangleIndices.Add(3);
            TriangleIndices.Add(4);
            TriangleIndices.Add(7);

            meshGeom3D.TriangleIndices = TriangleIndices;

            Point p2D_1 = new Point(0, 1);
            Point p2D_2 = new Point(1, 1);
            Point p2D_3 = new Point(1, 0);
            Point p2D_4 = new Point(0, 0);

            // Top and bottom
            /*for (int i = 0; i < 2; i++) // side of solid
            {
                meshGeom3D.TextureCoordinates.Add(p2D_1);
                meshGeom3D.TextureCoordinates.Add(p2D_2);
                meshGeom3D.TextureCoordinates.Add(p2D_3);
                meshGeom3D.TextureCoordinates.Add(p2D_4);
            }

            // Sides
            for (int i = 0; i < 4; i++) // side of solid
            {
                meshGeom3D.TextureCoordinates.Add(p2D_4);
                meshGeom3D.TextureCoordinates.Add(p2D_1);
                meshGeom3D.TextureCoordinates.Add(p2D_2);
                meshGeom3D.TextureCoordinates.Add(p2D_3);
            }*/


            // NEFUNGUJE SPRAVNE TEXTURA

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));
            meshGeom3D.TextureCoordinates.Add(new Point(0, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));
            meshGeom3D.TextureCoordinates.Add(new Point(0, 0));

            // Side

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));
            meshGeom3D.TextureCoordinates.Add(new Point(0, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));
            meshGeom3D.TextureCoordinates.Add(new Point(0, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));
            meshGeom3D.TextureCoordinates.Add(new Point(0, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));

            meshGeom3D.TextureCoordinates.Add(new Point(0, 1));
            meshGeom3D.TextureCoordinates.Add(new Point(1, 0));
            meshGeom3D.TextureCoordinates.Add(new Point(0, 0));


            Vector3D n = new Vector3D(0, 0, 1);

            for (int i = 0; i < 36; i++)
                meshGeom3D.Normals.Add(n);

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }
        public GeometryModel3D CreateGM_3D_Volume_8Edges(CVolume volume)
        {
            Point3D solidControlEdge = new Point3D(volume.ControlPoint.X, volume.ControlPoint.Y, volume.ControlPoint.Z);

            /*
            Point3D p0 = new Point3D(solidControlEdge.X               , solidControlEdge.Y               , solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X + volume.m_fDim1, solidControlEdge.Y               , solidControlEdge.Z);
            Point3D p2 = new Point3D(solidControlEdge.X + volume.m_fDim1, solidControlEdge.Y + volume.m_fDim2, solidControlEdge.Z);
            Point3D p3 = new Point3D(solidControlEdge.X               , solidControlEdge.Y + volume.m_fDim2, solidControlEdge.Z);
            Point3D p4 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z + volume.m_fDim3);
            Point3D p5 = new Point3D(solidControlEdge.X + volume.m_fDim1, solidControlEdge.Y, solidControlEdge.Z + volume.m_fDim3);
            Point3D p6 = new Point3D(solidControlEdge.X + volume.m_fDim1, solidControlEdge.Y + volume.m_fDim2, solidControlEdge.Z + volume.m_fDim3);
            Point3D p7 = new Point3D(solidControlEdge.X, solidControlEdge.Y + volume.m_fDim2, solidControlEdge.Z + volume.m_fDim3);
            */

            return CreateGM_3D_Volume_8EdgesOld(solidControlEdge, volume.m_fDim1, volume.m_fDim2, volume.m_fDim3, volume.m_Material_1);
        }
        public static List<Point3D> GetWireFramePoints_Volume(GeometryModel3D volumeModel, bool bIsPointOnBase = false)
        {
            // Funguje len pre prizmaticke prvky pravidelneho tvaru (rovnaky pocet bodov na hornej aj spodnej podstave)

            List<Point3D> wireframePoints = new List<Point3D>();
            MeshGeometry3D geom = volumeModel.Geometry as MeshGeometry3D;

            List<Point3D> transformedPoints = new List<Point3D>(); // Pole transformovanych bodov

            foreach (Point3D p in geom.Positions)
            {
                Point3D transdformed = volumeModel.Transform.Transform(p); // Transformujeme povodny bod
                transformedPoints.Add(transdformed);
            }

            int iNumberOfEdgesTotal = transformedPoints.Count;
            int iNumberOfEdges_Base = iNumberOfEdgesTotal / 2; // Plati pre pravidelne hranoly, valce atd, celkovy pocet bodov je vzdy parny

            if (!bIsPointOnBase)
            {
                // Spodna podstava - Bottom Base
                for (int i = 0; i < iNumberOfEdges_Base; i++)
                {
                    if (i < iNumberOfEdges_Base - 1)
                    {
                        wireframePoints.Add(transformedPoints[i]);
                        wireframePoints.Add(transformedPoints[i + 1]);
                    }
                    else
                    {
                        wireframePoints.Add(transformedPoints[i]);
                        wireframePoints.Add(transformedPoints[0]);
                    }
                }

                // Horna podstava - Top Base
                for (int i = 0; i < iNumberOfEdges_Base; i++)
                {
                    if (i < iNumberOfEdges_Base - 1)
                    {
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i]);
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i + 1]);
                    }
                    else
                    {
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i]);
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + 0]);
                    }
                }

                // Plast hranola
                for (int i = 0; i < iNumberOfEdges_Base; i++)
                {
                    wireframePoints.Add(transformedPoints[i]);
                    wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i]);
                }
            }
            else
            {
                int iNumberOfEdges_BaseWithoutMiddle = iNumberOfEdges_Base - 1;

                // Spodna podstava - Bottom Base
                for (int i = 0; i < iNumberOfEdges_BaseWithoutMiddle; i++)
                {
                    if (i < iNumberOfEdges_BaseWithoutMiddle - 1)
                    {
                        wireframePoints.Add(transformedPoints[i]);
                        wireframePoints.Add(transformedPoints[i + 1]);
                    }
                    else
                    {
                        wireframePoints.Add(transformedPoints[i]);
                        wireframePoints.Add(transformedPoints[0]);
                    }
                }

                // Horna podstava - Top Base
                for (int i = 0; i < iNumberOfEdges_BaseWithoutMiddle; i++)
                {
                    if (i < iNumberOfEdges_BaseWithoutMiddle - 1)
                    {
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i]);
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i + 1]);
                    }
                    else
                    {
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i]);
                        wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + 0]);
                    }
                }

                // Plast hranola
                for (int i = 0; i < iNumberOfEdges_BaseWithoutMiddle; i++)
                {
                    wireframePoints.Add(transformedPoints[i]);
                    wireframePoints.Add(transformedPoints[iNumberOfEdges_Base + i]);
                }
            }

            return wireframePoints;
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of square pyramide
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateM_G_M_3D_Volume_5Edges(Point3D solidControlEdge, float fDim1_a, float fDim2_h, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            Point3D p0 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X - 0.5f * fDim1_a, solidControlEdge.Y - 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);
            Point3D p2 = new Point3D(solidControlEdge.X + 0.5f * fDim1_a, solidControlEdge.Y - 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);
            Point3D p3 = new Point3D(solidControlEdge.X + 0.5f * fDim1_a, solidControlEdge.Y + 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);
            Point3D p4 = new Point3D(solidControlEdge.X - 0.5f * fDim1_a, solidControlEdge.Y + 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);

            meshGeom3D.Positions = new Point3DCollection();

            meshGeom3D.Positions.Add(p0);
            meshGeom3D.Positions.Add(p1);
            meshGeom3D.Positions.Add(p2);
            meshGeom3D.Positions.Add(p3);
            meshGeom3D.Positions.Add(p4);

            Int32Collection TriangleIndices = new Int32Collection();

            // Sides
            TriangleIndices.Add(0);
            TriangleIndices.Add(1);
            TriangleIndices.Add(2);

            TriangleIndices.Add(0);
            TriangleIndices.Add(2);
            TriangleIndices.Add(3);

            TriangleIndices.Add(0);
            TriangleIndices.Add(3);
            TriangleIndices.Add(4);

            TriangleIndices.Add(0);
            TriangleIndices.Add(4);
            TriangleIndices.Add(1);

            // Bottom
            TriangleIndices.Add(1);
            TriangleIndices.Add(4);
            TriangleIndices.Add(2);

            TriangleIndices.Add(2);
            TriangleIndices.Add(4);
            TriangleIndices.Add(3);

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of triangular prism - just for nodal supports
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateM_G_M_3D_Volume_6Edges_CN(Point3D solidControlEdge, float fDim1_a, float fDim2_h, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            Point3D p0 = new Point3D(solidControlEdge.X, solidControlEdge.Y - 0.5f * fDim1_a, solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X, solidControlEdge.Y + 0.5f * fDim1_a, solidControlEdge.Z);
            Point3D p2 = new Point3D(solidControlEdge.X - 0.5f * fDim1_a, solidControlEdge.Y - 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);
            Point3D p3 = new Point3D(solidControlEdge.X + 0.5f * fDim1_a, solidControlEdge.Y - 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);
            Point3D p4 = new Point3D(solidControlEdge.X + 0.5f * fDim1_a, solidControlEdge.Y + 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);
            Point3D p5 = new Point3D(solidControlEdge.X - 0.5f * fDim1_a, solidControlEdge.Y + 0.5f * fDim1_a, solidControlEdge.Z - fDim2_h);

            meshGeom3D.Positions = new Point3DCollection();

            meshGeom3D.Positions.Add(p0);
            meshGeom3D.Positions.Add(p1);
            meshGeom3D.Positions.Add(p2);
            meshGeom3D.Positions.Add(p3);
            meshGeom3D.Positions.Add(p4);
            meshGeom3D.Positions.Add(p5);

            Int32Collection TriangleIndices = new Int32Collection();

            // Sides
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 4, 3);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 0, 2, 5);

            TriangleIndices.Add(0);
            TriangleIndices.Add(2);
            TriangleIndices.Add(3);

            TriangleIndices.Add(1);
            TriangleIndices.Add(4);
            TriangleIndices.Add(5);

            // Bottom
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of polygonal prism
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateM_G_M_3D_Volume_nEdges(Point3D solidControlEdge, Point3DCollection points_z0, float fDim2_h, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh
            meshGeom3D.Positions = new Point3DCollection();

            // Base (0)
            foreach (Point3D point in points_z0)
            {
                Point3D point_temp = point;
                point_temp.X += solidControlEdge.X;
                point_temp.Y += solidControlEdge.Y;
                point_temp.Z += solidControlEdge.Z;
                meshGeom3D.Positions.Add(point_temp);
            }

            // Top (h)
            foreach (Point3D point in points_z0)
            {
                Point3D point_temp = point;
                point_temp.X += solidControlEdge.X;
                point_temp.Y += solidControlEdge.Y;
                point_temp.Z += solidControlEdge.Z;
                point_temp.Z += fDim2_h;
                meshGeom3D.Positions.Add(point_temp);
            }

            Int32Collection TriangleIndices = new Int32Collection();

            // Bottom
            for (int i = 0; i < points_z0.Count - 2; i++)
            {
                TriangleIndices.Add(0);
                TriangleIndices.Add(i + 2);
                TriangleIndices.Add(i + 1);
            }

            // Top
            for (int i = 0; i < points_z0.Count - 2; i++)
            {
                TriangleIndices.Add(points_z0.Count + 0);
                TriangleIndices.Add(points_z0.Count + i + 1);
                TriangleIndices.Add(points_z0.Count + i + 2);
            }

            // Sides
            for (int i = 0; i < points_z0.Count; i++)
            {
                if (i < points_z0.Count-1)
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, i + 1, points_z0.Count + i + 1, points_z0.Count + i);
                else // Last rectangle
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, 0, points_z0.Count, points_z0.Count + i);
            }

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }

        public GeometryModel3D CreateM_G_M_3D_Volume_nEdges(Point3D solidControlEdge, Point3DCollection points_z0, Point3DCollection points_zh, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh
            meshGeom3D.Positions = new Point3DCollection();

            // Base (0)
            foreach (Point3D point in points_z0)
            {
                Point3D point_temp = point;
                point_temp.X += solidControlEdge.X;
                point_temp.Y += solidControlEdge.Y;
                point_temp.Z += solidControlEdge.Z;
                meshGeom3D.Positions.Add(point_temp);
            }

            // Top (h)
            foreach (Point3D point in points_zh)
            {
                Point3D point_temp = point;
                point_temp.X += solidControlEdge.X;
                point_temp.Y += solidControlEdge.Y;
                point_temp.Z += solidControlEdge.Z;
                meshGeom3D.Positions.Add(point_temp);
            }

            Int32Collection TriangleIndices = new Int32Collection();

            // Bottom
            for (int i = 0; i < points_z0.Count - 2; i++)
            {
                TriangleIndices.Add(0);
                TriangleIndices.Add(i + 2);
                TriangleIndices.Add(i + 1);
            }

            // Top
            for (int i = 0; i < points_zh.Count - 2; i++)
            {
                TriangleIndices.Add(points_z0.Count + 0);
                TriangleIndices.Add(points_z0.Count + i + 1);
                TriangleIndices.Add(points_z0.Count + i + 2);
            }

            // Sides
            for (int i = 0; i < points_z0.Count; i++)
            {
                if (i < points_z0.Count - 1)
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, i + 1, points_z0.Count + i + 1, points_z0.Count + i);
                else // Last rectangle
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, 0, points_z0.Count, points_z0.Count + i);
            }

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }

        // Refaktorovat s StraightLineArrow3D
        public static Point3D GetPointinLCS(int iPrimaryModelDirection, double dCoordX, double dCoordY, double dCoordZ)
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

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of rectangular prism
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateM_3D_G_Volume_8Edges(float fDim1, float fDim2, float fDim3, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            Point3D p0 = new Point3D(0, 0, 0);
            Point3D p1 = new Point3D(fDim1, 0, 0);
            Point3D p2 = new Point3D(fDim1, fDim2, 0);
            Point3D p3 = new Point3D(0, fDim2, 0);
            Point3D p4 = new Point3D(0, 0, fDim3);
            Point3D p5 = new Point3D(fDim1, 0, fDim3);
            Point3D p6 = new Point3D(fDim1, fDim2, fDim3);
            Point3D p7 = new Point3D(0, fDim2, fDim3);

            meshGeom3D.Positions = new Point3DCollection();

            meshGeom3D.Positions.Add(p0);
            meshGeom3D.Positions.Add(p1);
            meshGeom3D.Positions.Add(p2);
            meshGeom3D.Positions.Add(p3);
            meshGeom3D.Positions.Add(p4);
            meshGeom3D.Positions.Add(p5);
            meshGeom3D.Positions.Add(p6);
            meshGeom3D.Positions.Add(p7);

            Int32Collection TriangleIndices = new Int32Collection();

            // Sides
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 2, 1, 0); // Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 6, 7); // Top
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 5, 4); // Sides
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 6, 5);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 7, 6);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 0, 4, 7);

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }
        public GeometryModel3D CreateM_3D_G_Volume_8Edges(Point3D solidControlEdge, float fDim1, float fDim2, float fDim3, MaterialGroup matGroup)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            Point3D p0 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p2 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p3 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p4 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p5 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p6 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);
            Point3D p7 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);

            meshGeom3D.Positions = new Point3DCollection();

            meshGeom3D.Positions.Add(p0);
            meshGeom3D.Positions.Add(p1);
            meshGeom3D.Positions.Add(p2);
            meshGeom3D.Positions.Add(p3);
            meshGeom3D.Positions.Add(p4);
            meshGeom3D.Positions.Add(p5);
            meshGeom3D.Positions.Add(p6);
            meshGeom3D.Positions.Add(p7);

            Int32Collection TriangleIndices = new Int32Collection();

            // Sides
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 2, 1, 0); // Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 6, 7); // Top
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 5, 4); // Sides
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 6, 5);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 7, 6);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 0, 4, 7);

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = matGroup;

            return geomModel3D;
        }
        public GeometryModel3D CreateM_3D_G_Volume_8Edges(Point3D solidControlEdge, float fDim1, float fDim2, float fDim3, DiffuseMaterial mat)
        {
            MeshGeometry3D meshGeom3D = new MeshGeometry3D(); // Create geometry mesh

            Point3D p0 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p2 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p3 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p4 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p5 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p6 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);
            Point3D p7 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);

            meshGeom3D.Positions = new Point3DCollection();

            meshGeom3D.Positions.Add(p0);
            meshGeom3D.Positions.Add(p1);
            meshGeom3D.Positions.Add(p2);
            meshGeom3D.Positions.Add(p3);
            meshGeom3D.Positions.Add(p4);
            meshGeom3D.Positions.Add(p5);
            meshGeom3D.Positions.Add(p6);
            meshGeom3D.Positions.Add(p7);

            Int32Collection TriangleIndices = new Int32Collection();

            // Sides
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 2, 1, 0); // Bottom
            AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 6, 7); // Top
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 5, 4); // Sides
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 6, 5);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 7, 6);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 0, 4, 7);

            meshGeom3D.TriangleIndices = TriangleIndices;

            GeometryModel3D geomModel3D = new GeometryModel3D();

            geomModel3D.Geometry = meshGeom3D; // Set mesh to model

            geomModel3D.Material = mat;

            return geomModel3D;
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of rectangular prism
        // Each side can be other material / color
        //--------------------------------------------------------------------------------------------
        public Model3DGroup CreateM_3D_G_Volume_8Edges(Point3D solidControlEdge, float fDim1, float fDim2, float fDim3, DiffuseMaterial mat1, DiffuseMaterial mat2)
        {
            Model3DGroup models = new Model3DGroup();

            Point3D p0 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p1 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z);
            Point3D p2 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p3 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z);
            Point3D p4 = new Point3D(solidControlEdge.X, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p5 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y, solidControlEdge.Z + fDim3);
            Point3D p6 = new Point3D(solidControlEdge.X + fDim1, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);
            Point3D p7 = new Point3D(solidControlEdge.X, solidControlEdge.Y + fDim2, solidControlEdge.Z + fDim3);

            /*
            DiffuseMaterial DiffMat1 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(255,255,120)));

            BitmapImage brickjpg = new BitmapImage();
            brickjpg.BeginInit();
            brickjpg.UriSource = new Uri(@"brick.jpg", UriKind.RelativeOrAbsolute);
            brickjpg.EndInit();

            DiffuseMaterial DiffMat2 = new DiffuseMaterial(new ImageBrush(brickjpg));
            */

            models.Children.Add(CreateRectangle(p3, p2, p1, p0, mat1)); // Bottom
            models.Children.Add(CreateRectangle(p4, p5, p6, p7, mat1)); // Top
            models.Children.Add(CreateRectangle(p0, p1, p5, p4, mat2)); // Sides
            models.Children.Add(CreateRectangle(p1, p2, p6, p5, mat2));
            models.Children.Add(CreateRectangle(p2, p3, p7, p6, mat2));
            models.Children.Add(CreateRectangle(p3, p0, p4, p7, mat2));

            //To Mato - tu niekde by sme mohli vytvarat aj wireframe
            //pripadne aj napojene na nejaky bool ci to treba
            WireFramePoints = new List<Point3D>() { p0, p1, p1, p2, p2, p3, p3, p0, p4, p5, p5, p6, p6, p7, p7, p4, p0, p4, p1, p5, p2, p6, p3, p7 };            

            return models;
        }
        public Model3DGroup CreateM_3D_G_Volume_8Edges(CVolume volume)
        {
            Model3DGroup m3Dg = new Model3DGroup();

            Point3D solidControlEdge = new Point3D(volume.ControlPoint.X, volume.ControlPoint.Y, volume.ControlPoint.Z);

            m3Dg.Children.Add(CreateM_3D_G_Volume_8Edges(solidControlEdge, volume.m_fDim1, volume.m_fDim2, volume.m_fDim3, volume.m_Material_1, volume.m_Material_2));

            return m3Dg;
        }
        public Model3DGroup CreateM_3D_G_Volume_8Edges()
        {
            Model3DGroup m3Dg = new Model3DGroup();

            Point3D solidControlEdge = new Point3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            m3Dg.Children.Add(CreateM_3D_G_Volume_8Edges(solidControlEdge, m_fDim1, m_fDim2, m_fDim3, m_Material_1, m_Material_2));

            return m3Dg;
        }
    }
}
