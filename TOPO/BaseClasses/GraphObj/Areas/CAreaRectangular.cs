using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Linq;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CAreaRectangular : CAreaPolygonal // TO Ondrej - doriesit ci je potrebna samostatna trieda a ake by malo byt dedenie
    {
        private double m_x; // Width of rectangle

        public double x
        {
            get
            {
                return m_x;
            }

            set
            {
                m_x = value;
            }
        }

        private double m_z; // Height of rectangle

        public double z
        {
            get
            {
                return m_z;
            }

            set
            {
                m_z = value;
            }
        }

        private double m_OutOffPlaneOffset_y;

        public double OutOffPlaneOffset_y
        {
            get
            {
                return m_OutOffPlaneOffset_y;
            }

            set
            {
                m_OutOffPlaneOffset_y = value;
            }
        }

        // Constructor 1
        public CAreaRectangular()
        {

        }

        // Constructor 2
        public CAreaRectangular(int iArea_ID, int[] iPCollection, int fTime)
        {
            ID = iArea_ID;
            m_iPointsCollection = iPCollection;
            FTime = fTime;
        }

        // Constructor 3
        public CAreaRectangular(int iArea_ID, List<Point3D> edgePointList, int fTime)
        {
            ID = iArea_ID;
            EdgePointList = edgePointList;
            FTime = fTime;
        }

        // Constructor 4
        public CAreaRectangular(int iArea_ID, Point pOrigin, double width_x, double height_z, int fTime, double offset_y = 0)
        {
            ID = iArea_ID;

            x = width_x;
            z = height_z;
            OutOffPlaneOffset_y = offset_y;

            // Edge in local coordinates
            Point3D p0_baseleft = new Point3D(0, OutOffPlaneOffset_y, 0);
            Point3D p1_baseright = new Point3D(x, OutOffPlaneOffset_y, 0);
            Point3D p2_topright = new Point3D(x, OutOffPlaneOffset_y, z);
            Point3D p3_topleft = new Point3D(0, OutOffPlaneOffset_y, z);

            // Local in-plane offset

            p0_baseleft.X += pOrigin.X;
            p0_baseleft.Z += pOrigin.Y;

            p1_baseright.X += pOrigin.X;
            p1_baseright.Z += pOrigin.Y;

            p2_topright.X += pOrigin.X;
            p2_topright.Z += pOrigin.Y;

            p3_topleft.X += pOrigin.X;
            p3_topleft.Z += pOrigin.Y;

            EdgePointList = new List<Point3D> { p0_baseleft, p1_baseright, p2_topright, p3_topleft };

            FTime = fTime;
        }

        public GeometryModel3D CreateArea(DiffuseMaterial material, bool useTextures = false, bool setBackMaterial = true)
        {
            if (EdgePointList == null) return null;

            MeshGeometry3D mesh = new MeshGeometry3D();

            foreach (Point3D p in EdgePointList)
                mesh.Positions.Add(p);

            for (int i = 0; i < EdgePointList.Count - 1; i++)
            {
                mesh.TriangleIndices.Add(0); // Vsetky trojuholniky zacinaju v nule, uvazujeme konvexny polygon
                mesh.TriangleIndices.Add(i + 1);
                mesh.TriangleIndices.Add(i + 2);
            }

            if (useTextures)
            {
                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(1, 0));
                mesh.TextureCoordinates.Add(new Point(1, 1));
                mesh.TextureCoordinates.Add(new Point(0, 1));
            }

            GeometryModel3D model3D = new GeometryModel3D(mesh, material);
            if (setBackMaterial) model3D.BackMaterial = material;
            return model3D;
        }
    }
}
