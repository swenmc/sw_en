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
using BaseClasses.GraphObj;
using BaseClasses;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Collections.ObjectModel;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowHelixTrial.xaml
    /// </summary>
    public partial class WindowHelixTrial : Window
    {
        private ObservableCollection<Point3D> points = new ObservableCollection<Point3D>();


        public ObservableCollection<Point3D> Points
        {
            get
            {
                return this.points;
            }
        }

        public WindowHelixTrial()
        {
            Points.Add(new Point3D(0, 0, 0));
            Points.Add(new Point3D(1, 0, 0));
            Points.Add(new Point3D(0, 1, 0));
            Points.Add(new Point3D(0, 0, 1));

            Points.Add(new Point3D(1, 0, 1));
            Points.Add(new Point3D(1, 1, 0));
            Points.Add(new Point3D(0, 1, 1));
            Points.Add(new Point3D(1, 1, 1));

            InitializeComponent();

            
            

            CPoint[] m_arrGOPoints = new BaseClasses.GraphObj.CPoint[1];
            CArea[] m_arrGOAreas = new CArea[0];
            CVolume[] m_arrGOVolumes = new CVolume[1];

            float x = 0.0f;
            float y = 0.0f;
            float z = 10.0f;

            m_arrGOPoints[00] = new CPoint(1, x, y, z, 0);

            DiffuseMaterial DiffMat1 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(255, 255, 120)));

            BitmapImage brickjpg = new BitmapImage();
            brickjpg.BeginInit();
            brickjpg.UriSource = new Uri(@"brick.jpg", UriKind.RelativeOrAbsolute);
            brickjpg.EndInit();

            ImageBrush brickIB = new ImageBrush(brickjpg);
            brickIB.TileMode = TileMode.Tile;
            brickIB.Stretch = Stretch.None;

            DiffuseMaterial DiffMat2 = new DiffuseMaterial(brickIB);

            BaseClasses.GraphObj.CVolume v = new CVolume(001, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[000], 1, 1, 1, DiffMat1, DiffMat2, true, 0);

            Model3DGroup mod3dGr = v.CreateM_3D_G_Volume_8Edges();
            //PointsVisual3D p = new PointsVisual3D();
            //p.Content = new Point3D(0,0,0);
            //mod3dGr.Children.Add(p);

            

            foo.Content = mod3dGr;
        }

        //public Point3D[] GeneratePoints(int n, double time)
        //109	+        {
        //110	+            var result = new Point3D[n];
        //111	+            double R = 2;
        //112	+            double r = 0.5;
        //113	+            for (int i = 0; i < n; i++)
        //114	+            {
        //115	+                double t = Math.PI * 2 * i / (n - 1);
        //116	+                double u = t * 24 + time * 5;
        //117	+                var pt = new Point3D(Math.Cos(t) * (R + r * Math.Cos(u)), Math.Sin(t) * (R + r * Math.Cos(u)), r * Math.Sin(u));
        //118	+                result[i] =pt;
        //119	+                if (i > 0 && i < n - 1)
        //120	+                    result[i] =pt;
        //121	+            }
        //122	+            return result;
        //123	+        }
    }
}
