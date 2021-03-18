using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Linq;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CAreaPolygonal : CArea
    {
        //public int m_iPointsCollection = new Int32Collection(); // List / Collection of points IDs

        public int[] m_iPointsCollection; // List / Collection of points IDs

        private List<Point3D> m_EdgePointList;

        public CAreaPolygonal()
        {

        }

        // Constructor 2
        public CAreaPolygonal(int iArea_ID, int[] iPCollection, int fTime)
        {
            ID = iArea_ID;
            m_iPointsCollection = iPCollection;
            FTime = fTime;
        }

        // Constructor 3
        public CAreaPolygonal(int iArea_ID, List<Point3D> edgePointList, int fTime)
        {
            ID = iArea_ID;
            EdgePointList = edgePointList;
            FTime = fTime;
        }

        public List<Point3D> EdgePointList
        {
            get
            {
                return m_EdgePointList;
            }

            set
            {
                m_EdgePointList = value;
            }
        }

        public GeometryModel3D CreateArea(bool useTextures, DiffuseMaterial material, bool setBackMaterial = true)
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
                if (EdgePointList.Count == 3 || EdgePointList.Count == 4)
                {
                    mesh.TextureCoordinates.Add(new Point(0, 0));
                    mesh.TextureCoordinates.Add(new Point(1, 0));
                    mesh.TextureCoordinates.Add(new Point(1, 1));
                    mesh.TextureCoordinates.Add(new Point(0, 1));
                }
                else if(EdgePointList.Count == 5)
                {
                    // Top Tip relative x-position
                    // Body su v rovine XZ
                    double xTopTip_PointNo4 = Math.Abs((EdgePointList[3].X - EdgePointList[0].X) / (EdgePointList[1].X - EdgePointList[0].X));

                    // Body su v rovine YZ
                    if(MATH.MathF.d_equal(EdgePointList[0].X, EdgePointList[1].X))
                        xTopTip_PointNo4 = Math.Abs((EdgePointList[3].Y - EdgePointList[0].Y) / (EdgePointList[1].Y - EdgePointList[0].Y));

                    mesh.TextureCoordinates.Add(new Point(0, 1));
                    mesh.TextureCoordinates.Add(new Point(1, 1));
                    mesh.TextureCoordinates.Add(new Point(1, 1 - (EdgePointList[2].Z / EdgePointList[3].Z)));
                    mesh.TextureCoordinates.Add(new Point(xTopTip_PointNo4, 0));
                    mesh.TextureCoordinates.Add(new Point(0, 1 - (EdgePointList[2].Z / EdgePointList[3].Z)));
                }
                else
                {
                    throw new Exception("Not implemented.");
                }
            }

            GeometryModel3D model3D = new GeometryModel3D(mesh, material);
            if(setBackMaterial) model3D.BackMaterial = material;
            return model3D;
        }

        private static List<Point3D> GetWireFramePointsFromGeometryPositions(Point3DCollection positions)
        {
            List<Point3D> wireframePoints = new List<Point3D>();
            for (int i = 0; i < positions.Count - 1; i++)
            {
                wireframePoints.Add(positions[i]);
                wireframePoints.Add(positions[i + 1]);
            }
            return wireframePoints;
        }

        public List<Point3D> GetWireFrame()
        {            
            List<Point3D> wireframe = new List<Point3D>();
            if (EdgePointList == null || EdgePointList.Count < 2) return wireframe;

            for (int i = 0; i < EdgePointList.Count - 1; i++)
            {
                wireframe.Add(EdgePointList[i]);
                wireframe.Add(EdgePointList[i + 1]);
            }
            wireframe.Add(EdgePointList.Last());
            wireframe.Add(EdgePointList.First());
            return wireframe;
        }

        public Point3D GetCenterPoint()
        {
            Point3D centreP = new Point3D(0, 0, 0);
            if (EdgePointList == null || EdgePointList.Count < 1) return centreP;

            double x = EdgePointList.Average(p => p.X);
            double y = EdgePointList.Average(p => p.Y);
            double z = EdgePointList.Average(p => p.Z);
            centreP = new Point3D(x, y, z);
            return centreP;            
        }
    }
}