using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BaseClasses;

namespace PFD
{
    /// <summary>
    /// Interaction logic for FrameInternalForces_2D.xaml
    /// </summary>
    public partial class FrameInternalForces_2D : Window
    {
        public FrameInternalForces_2D()
        {
            InitializeComponent();

            Line a = new Line();

            List<Point> listMemberPoints = new List<Point>(2);
            listMemberPoints.Add(new Point(10, 20));
            listMemberPoints.Add(new Point(200, 200));

            listMemberPoints.Add(new Point(50, 50));
            listMemberPoints.Add(new Point(200, -200));

            Drawing2D.DrawPolyLine(false, listMemberPoints, 10, 10, 5, 5, 1, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, Canvas_InternalForceDiagram);

        }
    }
}
