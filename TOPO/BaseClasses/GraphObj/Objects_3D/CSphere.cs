using System;
using MATH;
using System.Windows;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class CSphere : CVolume
    {
        // Sphere
        public CSphere(int iSphere_ID, Point3D pControlCenterPoint, float fr, DiffuseMaterial volMat1, bool bIsDisplayed, float fTime)
        {
            ID = iSphere_ID;
            m_pControlPoint = pControlCenterPoint;
            m_fDim1 = fr;
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            // Set same properties for both materials
            m_Material_2 = volMat1;
            m_volColor_2 = volMat1.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            /*MObject3DModel =*/
            CreateM_3D_G_Volume_Sphere(new Point3D(pControlCenterPoint.X, pControlCenterPoint.Y, pControlCenterPoint.Z), fr, volMat1);
        }

        //--------------------------------------------------------------------------------------------
        // Generate 3D volume geometry of sphere
        // Each side can be other material / color
        //--------------------------------------------------------------------------------------------
        public static Model3DGroup CreateM_3D_G_Volume_Sphere(Point3D solidControlPoint, float fr, DiffuseMaterial mat1)
        {
            Model3DGroup models = new Model3DGroup();

            SphereMeshGenerator objSphere = new SphereMeshGenerator(solidControlPoint, fr);
            GeometryModel3D sphereModel3D = new GeometryModel3D(objSphere.Geometry, mat1);

            models.Children.Add((Model3D)sphereModel3D);

            return models;
        }
    }

    public class SphereMeshGenerator
    {
        private int _slices = 32;
        private int _stacks = 16;
        private Point3D _center = new Point3D();
        private double _radius = 0.5;

        public int Slices
        {
            get { return _slices; }
            set { _slices = value; }
        }

        public int Stacks
        {
            get { return _stacks; }
            set { _stacks = value; }
        }

        public Point3D Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public double Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public MeshGeometry3D Geometry
        {
            get
            {
                return CalculateMesh();
            }
        }

        public SphereMeshGenerator(Point3D c, float fr)
        {
            Center = c; // Set Center
            Radius = fr;
        }

        private MeshGeometry3D CalculateMesh()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int stack = 0; stack <= Stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / Stacks; // kut koji zamisljeni pravac povucen iz sredista koordinatnog sustava zatvara sa XZ ravninom. 
                double y = _radius * Math.Sin(phi); // Odredi poziciju Y koordinate. 
                double scale = -_radius * Math.Cos(phi);

                for (int slice = 0; slice <= Slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / Slices; // Kada gledamo 2D koordinatni sustav osi X i Z... ovo je kut koji zatvara zamisljeni pravac povucen iz sredista koordinatnog sustava sa Z osi ( Z = Y ). 
                    double x = scale * Math.Sin(theta); // Odredi poziciju X koordinate. Uoči da je scale = -_radius * Math.Cos(phi)
                    double z = scale * Math.Cos(theta); // Odredi poziciju Z koordinate. Uoči da je scale = -_radius * Math.Cos(phi)

                    Vector3D normal = new Vector3D(x, y, z); // Normala je vektor koji je okomit na površinu. U ovom slučaju normala je vektor okomit na trokut plohu trokuta. 
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add(normal + Center);     // Positions dobiva vrhove trokuta. 
                    mesh.TextureCoordinates.Add(new Point((double)slice / Slices, (double)stack / Stacks));
                    // TextureCoordinates kaže gdje će se neka točka iz 2D-a preslikati u 3D svijet. 
                }
            }

            for (int stack = 0; stack <= Stacks; stack++)
            {
                int top = (stack + 0) * (Slices + 1);
                int bot = (stack + 1) * (Slices + 1);

                for (int slice = 0; slice < Slices; slice++)
                {
                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add(top + slice);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(top + slice + 1);
                    }

                    if (stack != Stacks - 1)
                    {
                        mesh.TriangleIndices.Add(top + slice + 1);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(bot + slice + 1);
                    }
                }
            }

            return mesh;
        }
    }
}
