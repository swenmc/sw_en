using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class ParametricSurface : CEntity3D
    {
        private int nu = 30;
        private int nv = 30;
        private double radius = 0.02;
        private double mainradius = 0.4f;
        private double umin = -3;
        private double umax = 3;
        private double vmin = -8;
        private double vmax = 8;
        private double xmin = -1;
        private double xmax = 1;
        private double ymin = -1;
        private double ymax = 1;
        private double zmin = -1;
        private double zmax = 1;
        private Color lineColor = Colors.Black;
        //private Color surfaceColor = Colors.White;
        //private float fOpacity;
        private Point3D center = new Point3D();
        private bool isHiddenLine = false;
        private bool isWireframe = true;
        //private Viewport3D viewport3d = new Viewport3D();
        private GeometryModel3D m_geometryModel3D;

        public bool IsWireframe
        {
            get { return isWireframe; }
            set { isWireframe = value; }
        }
        public bool IsHiddenLine
        {
            get { return isHiddenLine; }
            set { isHiddenLine = value; }
        }
        public double Mainradius
        {
            get { return mainradius; }
            set { mainradius = value; }
        }
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }
        //public Color SurfaceColor
        //{
        //    get { return surfaceColor; }
        //    set { surfaceColor = value; }
        //}
        //public float FOpacity
        //{
        //    get { return fOpacity; }
        //    set { fOpacity = value; }
        //}
        public double Umin
        {
            get { return umin; }
            set { umin = value; }
        }
        public double Umax
        {
            get { return umax; }
            set { umax = value; }
        }
        public double Vmin
        {
            get { return vmin; }
            set { vmin = value; }
        }
        public double Vmax
        {
            get { return vmax; }
            set { vmax = value; }
        }
        public int Nu
        {
            get { return nu; }
            set { nu = value; }
        }
        public int Nv
        {
            get { return nv; }
            set { nv = value; }
        }
        public double Xmin
        {
            get { return xmin; }
            set { xmin = value; }
        }

        public double Xmax
        {
            get { return xmax; }
            set { xmax = value; }
        }
        public double Ymin
        {
            get { return ymin; }
            set { ymin = value; }
        }
        public double Ymax
        {
            get { return ymax; }
            set { ymax = value; }
        }
        public double Zmin
        {
            get { return zmin; }
            set { zmin = value; }
        }
        public double Zmax
        {
            get { return zmax; }
            set { zmax = value; }
        }
        public Point3D Center
        {
            get { return center; }
            set { center = value; }
        }
        //public Viewport3D Viewport3d
        //{
        //    get { return viewport3d; }
        //    set { viewport3d = value; }
        //}
        
        public GeometryModel3D GeometryModel3D
        {
            get { return m_geometryModel3D; }
            set { m_geometryModel3D = value; }
        }

        public ParametricSurface(double mainradius, double radius, Point3D pCenterPoint)
        {
            //SurfaceColor = cSurfaceColor;
            //FOpacity = fOpacity;

            Mainradius = mainradius;
            Radius = radius;
            Center = pCenterPoint;
        }

        public Point3D Torus(double u, double v, double mainradius, double radius)
        {
            double x = (mainradius + radius * Math.Cos(v)) * Math.Cos(u);
            double z = (mainradius + radius * Math.Cos(v)) * Math.Sin(u);
            double y = radius * Math.Sin(v);
            return new Point3D(x, y, z);
        }

        public void CreateSurface(DiffuseMaterial mat)
        {
            double du = (Umax - Umin) / (Nu - 1);
            double dv = (Vmax - Vmin) / (Nv - 1);
            if (Nu < 2 || Nv < 2)
                return;
            Point3D[,] pts = new Point3D[Nu, Nv];
            for (int i = 0; i < Nu; i++)
            {
                double u = Umin + i * du;

                for (int j = 0; j < Nv; j++)
                {
                    double v = Vmin + j * dv;
                    pts[i, j] = Torus(u, v, mainradius, radius);
                    pts[i, j] += (Vector3D)Center;
                    pts[i, j] = Utility.GetNormalize(
                    pts[i, j], Xmin, Xmax,
                    Ymin, Ymax, Zmin, Zmax);
                }
            }
            Point3D[] p = new Point3D[4];
            List<Point3D> positions = new List<Point3D>();
            List<int> triangleIndices = new List<int>();
            WireFramePoints = new List<Point3D>();
            int index = 0;
            for (int i = 0; i < Nu - 1; i++)
            {
                for (int j = 0; j < Nv - 1; j++)
                {
                    p[0] = pts[i, j];
                    p[1] = pts[i, j + 1];
                    p[2] = pts[i + 1, j + 1];
                    p[3] = pts[i + 1, j];
                    
                    Utility.CreateRectangleFace(p[0], p[1], p[2], p[3], index, positions, triangleIndices);
                    index = index + 4;

                    // Create wireframe
                    if (IsWireframe == true) Utility.CreateWireframe(p[0], p[1], p[2], p[3], WireFramePoints);
                }
            }

            MeshGeometry3D meshGeom3D = new MeshGeometry3D();
            meshGeom3D.Positions = new Point3DCollection(positions);
            meshGeom3D.TriangleIndices = new Int32Collection(triangleIndices);

            m_geometryModel3D = new GeometryModel3D();
            m_geometryModel3D.Geometry = meshGeom3D; // Set mesh to model
            m_geometryModel3D.Material = mat;
        }

    }
}
