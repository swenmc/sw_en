using InteractiveDataDisplay.WPF;
using System;
using System.Collections;
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

namespace PFD
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow(IEnumerable x_values, IEnumerable fArr_AxialForceValuesN, IEnumerable fArr_ShearForceValuesVx, 
            IEnumerable fArr_ShearForceValuesVy, IEnumerable fArr_TorsionMomentValuesT, 
            IEnumerable fArr_BendingMomentValuesMx, IEnumerable fArr_BendingMomentValuesMy)
        {
            InitializeComponent();

            //var x = Enumerable.Range(0, 1001).Select(i => i / 10.0).ToArray();
            //var y = x.Select(v => Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v).ToArray();
            //linegraph.Plot(x, y);

            //var y2 = new List<double> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            //var x2 = new List<double> { 1, 1, 2, 2, 2, 3, 3, 4, 5 };


            //ShowGraph(x_values, y_values);

            if (x_values == null) return;
            if (fArr_AxialForceValuesN == null) return;
            if (fArr_ShearForceValuesVx == null) return;
            if (fArr_ShearForceValuesVy == null) return;
            if (fArr_TorsionMomentValuesT == null) return;
            if (fArr_BendingMomentValuesMx == null) return;
            if (fArr_BendingMomentValuesMy == null) return;

            linegraph1.Plot(x_values, fArr_AxialForceValuesN);
            linegraph2.Plot(x_values, fArr_ShearForceValuesVx);
            linegraph3.Plot(x_values, fArr_ShearForceValuesVy);
            linegraph4.Plot(x_values, fArr_TorsionMomentValuesT);
            linegraph5.Plot(x_values, fArr_BendingMomentValuesMx);
            linegraph6.Plot(x_values, fArr_BendingMomentValuesMy);


            //double[] x = new double[200];
            //for (int i = 0; i < x.Length; i++)
            //    x[i] = 3.1415 * i / (x.Length - 1);

            //for (int i = 0; i < 25; i++)
            //{
            //    var lg = new LineGraph();
            //    lines.Children.Add(lg);
            //    lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(i * 10), 0));
            //    lg.Description = String.Format("Data series {0}", i + 1);
            //    lg.StrokeThickness = 2;
            //    lg.Plot(x, x.Select(v => Math.Sin(v + i / 10.0)).ToArray());
            //}
        }

        //public void ShowGraph(IEnumerable x_values, IEnumerable y_values)
        //{
        //    if (x_values == null) return;
        //    if (y_values == null) return;
        //    linegraph.Plot(x_values, y_values);
            
        //}

        

    }

    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
