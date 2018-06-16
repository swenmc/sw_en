using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        //--------------------------------------------------------------------------------------------
        public Window3()
        {
            InitializeComponent();

            Model3DGroup model = CreateCube();
            //Model3DGroup modelsgroup2 = CreateCube(3, 3, 3);

            //Model3DGroup modelsgroup = new Model3DGroup();

            //modelsgroup.Children.Add(modelsgroup1);
            //modelsgroup.Children.Add(modelsgroup2);

            _trackport.Model = (Model3D)model;

            _trackport.Trackball.TranslateScale = 1000;   //step for moving object (panning)

            _trackport.SetupScene();
        }

        //--------------------------------------------------------------------------------------------
        public Model3DGroup CreateCube()
        {
            Model3DGroup models = new Model3DGroup();

            Point3D p0 = new Point3D(0, 0, 0);
            Point3D p1 = new Point3D(1, 0, 0);
            Point3D p2 = new Point3D(1, 1, 0);
            Point3D p3 = new Point3D(0, 1, 0);
            Point3D p4 = new Point3D(0, 0, 1);
            Point3D p5 = new Point3D(1, 0, 1);
            Point3D p6 = new Point3D(1, 1, 1);
            Point3D p7 = new Point3D(0, 1, 1);

            ImageBrush myBrush = new ImageBrush(new BitmapImage(new Uri(@"brick.jpg", UriKind.RelativeOrAbsolute)));
            myBrush.TileMode = TileMode.Tile;

            //BitmapImage brickjpg = new BitmapImage();
            //brickjpg.BeginInit();
            //brickjpg.UriSource = new Uri(@"brick.jpg", UriKind.RelativeOrAbsolute);
            //brickjpg.EndInit();
            //DiffuseMaterial DiffMat = new DiffuseMaterial(new ImageBrush(brickjpg));

            DiffuseMaterial DiffMat = new DiffuseMaterial(myBrush);

            models.Children.Add(CreateRectangle(p3, p2, p1, p0, DiffMat));
            models.Children.Add(CreateRectangle(p4, p5, p6, p7, DiffMat));
            models.Children.Add(CreateRectangle(p0, p1, p5, p4, DiffMat));
            models.Children.Add(CreateRectangle(p1, p2, p6, p5, DiffMat));
            models.Children.Add(CreateRectangle(p2, p3, p7, p6, DiffMat));
            models.Children.Add(CreateRectangle(p3, p0, p4, p7, DiffMat));

            return models;
        }

        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateRectangle(
              Point3D point1, Point3D point2,
              Point3D point3, Point3D point4,
              DiffuseMaterial DiffMat)
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
        //--------------------------------------------------------------------------------------------
        private void LookAt(PerspectiveCamera camera, Point3D lookAtPoint)
        {
            camera.LookDirection = lookAtPoint - camera.Position;
        }
    }
}
