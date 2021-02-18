using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CAreaPolygonal : CArea
    {
        //public int m_iPointsCollection = new Int32Collection(); // List / Collection of points IDs

        public int[] m_iPointsCollection; // List / Collection of points IDs

        public List<Point3D> m_EdgePointList;

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
            m_EdgePointList = edgePointList;
            FTime = fTime;
        }

        public GeometryModel3D CreateArea(bool useTextures, DiffuseMaterial material)
        {
            if (m_EdgePointList == null) return null;

            MeshGeometry3D mesh = new MeshGeometry3D();

            foreach (Point3D p in m_EdgePointList)
                mesh.Positions.Add(p);

            for (int i = 0; i < m_EdgePointList.Count - 1; i++)
            {
                mesh.TriangleIndices.Add(0); // Vsetky trojuholniky zacinaju v nule, uvazujeme konvexny polygon
                mesh.TriangleIndices.Add(i + 1);
                mesh.TriangleIndices.Add(i + 2);
            }

            if (useTextures)
            {
                if (m_EdgePointList.Count == 3 || m_EdgePointList.Count == 4)
                {
                    mesh.TextureCoordinates.Add(new Point(0, 0));
                    mesh.TextureCoordinates.Add(new Point(1, 0));
                    mesh.TextureCoordinates.Add(new Point(1, 1));
                    mesh.TextureCoordinates.Add(new Point(0, 1));
                }
                else if(m_EdgePointList.Count == 5)
                {
                    // Top Tip relative x-position
                    // Body su v rovine XZ
                    double xTopTip_PointNo4 = Math.Abs((m_EdgePointList[3].X - m_EdgePointList[0].X) / (m_EdgePointList[1].X - m_EdgePointList[0].X));

                    // Body su v rovine YZ
                    if(MATH.MathF.d_equal(m_EdgePointList[0].X, m_EdgePointList[1].X))
                        xTopTip_PointNo4 = Math.Abs((m_EdgePointList[3].Y - m_EdgePointList[0].Y) / (m_EdgePointList[1].Y - m_EdgePointList[0].Y));

                    mesh.TextureCoordinates.Add(new Point(0, 1));
                    mesh.TextureCoordinates.Add(new Point(1, 1));
                    mesh.TextureCoordinates.Add(new Point(1, 1 - (m_EdgePointList[2].Z / m_EdgePointList[3].Z)));
                    mesh.TextureCoordinates.Add(new Point(xTopTip_PointNo4, 0));
                    mesh.TextureCoordinates.Add(new Point(0, 1 - (m_EdgePointList[2].Z / m_EdgePointList[3].Z)));
                }
                else
                {
                    throw new Exception("Not implemented.");
                }
            }

            return new GeometryModel3D(mesh, material);
        }
    }
}