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
using CENEX;
using _3DTools;

namespace sw_en_GUI
{
	/// <summary>
	/// Interaction logic for Viewer3D.xaml
	/// </summary>
	public partial class Viewer3D : Window
	{
		private Int32Collection M_FBProfileTriangelsIndices;
		private Trackball _trackball = new Trackball();
		ScreenSpaceLines3D wireframe = new ScreenSpaceLines3D();
		ScreenSpaceLines3D point = new ScreenSpaceLines3D();
		
		public Trackball Trackball
		{
			get { return _trackball; }
			set { _trackball = value; }
		}

		//------------------------------------------------------------------------------------------------
		public Viewer3D()
		{
			InitializeComponent();

			this.Camera.Transform = _trackball.Transform;
			this.Headlight.Transform = _trackball.Transform;
			this.PointsVisual3D.Transform = _trackball.Transform;

			this.Camera.Position = new Point3D(0, 0, 400);

			loadFBProfileTriangelIndices();
			Model3DGroup gr = new Model3DGroup();
			//gr.Children.Add(new AmbientLight());

			GeometryModel3D model = new GeometryModel3D();
			MeshGeometry3D mesh = new MeshGeometry3D();
			mesh.Positions = new Point3DCollection();
			CTest1 test1 = new CTest1();

			// Number of Points per section
			int iNoCrScPoints2D = 4; // Depends on Section Type - nacitavat priamo z objektu objCrSc // I,U,Z,HL, L, ....

			// Points 2D Coordinate Array
            float[,] res = test1.objCrScWF.CrScPointsOut; // I,U,Z,HL, L, ....


			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Tube !!!
			//float[,] res1 = test1.objCrSc.m_CrScPointOut; // RB, TU
			//float[,] res2 = test1.objCrSc.m_CrScPointIn; // RB, TU
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


			// Length of Element
			float fELength = -500; // Temporary load flor Member Segment geometry

			// Fill Mesh Positions for Start and End Section of Element - Defines Edge Points of Element

			// I,U,Z,HL, L, ....
			for (int j = 0; j < iNoCrScPoints2D; j++)
			{
				mesh.Positions.Add(new Point3D(res[j, 0], res[j, 1], 0));
			}
			for (int j = 0; j < iNoCrScPoints2D; j++)
			{
				mesh.Positions.Add(new Point3D(res[j, 0], res[j, 1], fELength));
			}
			mesh.TriangleIndices = M_FBProfileTriangelsIndices;
			model.Geometry = mesh;
			SolidColorBrush br = new SolidColorBrush(Colors.Red);
			model.Material = new DiffuseMaterial(br);
			gr.Children.Add(model);
			

			Root.Content = (Model3D)gr;

			wireframe.Color = Colors.Aquamarine;
			wireframe.Thickness = 1;
			makeWireframe(mesh.Positions);

			PointsVisual3D.Content = CreateCube(3, 3, 3);
			
			Viewport.Children.Add(wireframe);
			Viewport.Children.Add(point);


			drawAxis(-100, -50, 0);

		}

		private void drawAxis(double x, double y, double z) 
		{
			ScreenSpaceLines3D x_axis = new ScreenSpaceLines3D();
			ScreenSpaceLines3D y_axis = new ScreenSpaceLines3D();
			ScreenSpaceLines3D z_axis = new ScreenSpaceLines3D();
			x_axis.Color = Colors.Red;
			y_axis.Color = Colors.Green;
			z_axis.Color = Colors.Blue;

			x_axis.Points.Add(new Point3D(x, y, z));
			x_axis.Points.Add(new Point3D(x+20, y, z));

			y_axis.Points.Add(new Point3D(x, y, z));
			y_axis.Points.Add(new Point3D(x, y+20, z));

			z_axis.Points.Add(new Point3D(x, y, z));
			z_axis.Points.Add(new Point3D(x, y, z+20));

			Viewport.Children.Add(x_axis);
			Viewport.Children.Add(y_axis);
			Viewport.Children.Add(z_axis);
		}

		private void makeWireframe(Point3DCollection collection) 
		{
			//point
			point.Thickness = 5;
			point.Color = Colors.Yellow;
			point.Points.Add(collection[0]);
			point.Points.Add(new Point3D(collection[0].X + 0.1, collection[0].Y + 0.1, collection[0].Z + 0.1));

			//front rectangle
			wireframe.Points.Add(collection[0]);
			wireframe.Points.Add(collection[1]);

			wireframe.Points.Add(collection[1]);
			wireframe.Points.Add(collection[2]);

			wireframe.Points.Add(collection[2]);
			wireframe.Points.Add(collection[3]);

			wireframe.Points.Add(collection[3]);
			wireframe.Points.Add(collection[0]);

			//back rectangle
			wireframe.Points.Add(collection[4]);
			wireframe.Points.Add(collection[5]);

			wireframe.Points.Add(collection[5]);
			wireframe.Points.Add(collection[6]);

			wireframe.Points.Add(collection[6]);
			wireframe.Points.Add(collection[7]);

			wireframe.Points.Add(collection[7]);
			wireframe.Points.Add(collection[4]);


			// 4 side edges
			wireframe.Points.Add(collection[0]);
			wireframe.Points.Add(collection[4]);

			wireframe.Points.Add(collection[1]);
			wireframe.Points.Add(collection[5]);

			wireframe.Points.Add(collection[2]);
			wireframe.Points.Add(collection[6]);

			wireframe.Points.Add(collection[3]);
			wireframe.Points.Add(collection[7]);

		}



		//------------------------------------------------------------------------------------------------
		private void DrawRectangleIndices(Int32Collection Indices,
			  int point1, int point2,
			  int point3, int point4)
		{
			// Main Numbering is clockwise

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

		//------------------------------------------------------------------------------------------------
		private void loadFBProfileTriangelIndices()
		{
			// const int secNum = 4;  // Number of points in section (2D)
			M_FBProfileTriangelsIndices = new Int32Collection();

			// Front Side / Forehead
			DrawRectangleIndices(M_FBProfileTriangelsIndices, 0, 1, 2, 3);

			// Back Side 
			DrawRectangleIndices(M_FBProfileTriangelsIndices, 4, 7, 6, 5);

			// Shell Surface
			DrawRectangleIndices(M_FBProfileTriangelsIndices, 0, 4, 5, 1);
			DrawRectangleIndices(M_FBProfileTriangelsIndices, 1, 5, 6, 2);
			DrawRectangleIndices(M_FBProfileTriangelsIndices, 2, 6, 7, 3);
			DrawRectangleIndices(M_FBProfileTriangelsIndices, 3, 7, 4, 0);
		}

		//--------------------------------------------------------------------------------------------
		public Model3DGroup CreateCube(double x, double y, double z)
		{
			Model3DGroup models = new Model3DGroup();

			Point3D p0 = new Point3D(x-1, y-1, z-1);
			Point3D p1 = new Point3D(x+1, y-1, z-1);
			Point3D p2 = new Point3D(x+1, y-1, z+1);
			Point3D p3 = new Point3D(x+-1, y-1, z+1);
			Point3D p4 = new Point3D(x-1, y+1, z-1);
			Point3D p5 = new Point3D(x+1, y+1, z-1);
			Point3D p6 = new Point3D(x+1, y+1, z+1);
			Point3D p7 = new Point3D(x-1, y+1, z+1);

			models.Children.Add(CreateRectangle(p3, p2, p6, p7, Brushes.Yellow));  // front
			models.Children.Add(CreateRectangle(p2, p1, p5, p6, Brushes.Yellow));  // right
			models.Children.Add(CreateRectangle(p1, p0, p4, p5, Brushes.Yellow));  // back
			models.Children.Add(CreateRectangle(p0, p3, p7, p4, Brushes.Yellow));  // left
			models.Children.Add(CreateRectangle(p7, p6, p5, p4, Brushes.Yellow));  // top
			models.Children.Add(CreateRectangle(p2, p3, p0, p1, Brushes.Yellow));  // bottom

			return models;
		}

		//--------------------------------------------------------------------------------------------
		public GeometryModel3D CreateRectangle(
			  Point3D point1, Point3D point2,
			  Point3D point3, Point3D point4,
			  Brush brush)
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

			return new GeometryModel3D(mesh,
				new DiffuseMaterial(brush));
		}

		//------------------------------------------------------------------------------------------------
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_trackball.EventSource = CaptureBorder;
		}

		//------------------------------------------------------------------------------------------------
		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.LeftCtrl: _trackball.IsCtrlDown = e.IsDown; break;

			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.LeftCtrl: _trackball.IsCtrlDown = e.IsDown; break;
			}
		}


		//------------------------------------------------------------------------------------------------
		
	}
}
