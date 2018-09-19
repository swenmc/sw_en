using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TwoOpt
{
    /// <summary>
    ///    Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WindowRunSalesman : IMainWindow
    {
        public WindowRunSalesman(List<Point> Points, double fHeightToWidthRatio = 1)
        {
            InitializeComponent();

            // Set window size according to the ratio of (ymax - ymin) / (xmax - xmin) calculated from input point coordinates
            // window height 892
            // window width 800
            // canvas height 692
            // canvas width 765
            float fVerticalDifference = 200;
            float fHorizontalDifference = 35;

            if (fHeightToWidthRatio < 1)
                Height = (fHeightToWidthRatio * (Width - fHorizontalDifference)) + fVerticalDifference;
            else
                Width = ((1 / fHeightToWidthRatio) * (Height - fVerticalDifference)) + fHorizontalDifference;

            var mainWindowViewModel = DataContext as MainWindowViewModel;

            if (mainWindowViewModel != null)
                mainWindowViewModel.MainWindow = this;

            mainWindowViewModel.RoutePoints = Points;
        }

        public void PlotNodes(List<Pair> displayCoords)
        {
            CanvasGrid.Children.Clear();

            InvalidateVisual();

            int count = 0;

            foreach (var coord in displayCoords)
            {
                var ellipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    Fill = (count == 0 ? Brushes.Red: Brushes.Blue)
                };

                Canvas.SetLeft(ellipse, coord.X());
                Canvas.SetTop(ellipse, coord.Y());

                CanvasGrid.Children.Add(ellipse);
                count++;
            }
        }
        public void DisplayNodes(List<Pair> displayCoords)
        {
            int count = 0;
            foreach (var coord in displayCoords)
            {
                var ellipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    Fill = (count == 0 ? Brushes.Red : Brushes.Blue)
                };

                Canvas.SetLeft(ellipse, coord.X());
                Canvas.SetTop(ellipse, coord.Y());

                CanvasGrid.Children.Add(ellipse);
                count++;
            }
        }



        public double GridHeight()
        {
            return DisplayGrid.ActualHeight;
        }

        public double GridWidth()
        {
            return DisplayGrid.ActualWidth;
        }

        public void UpdateIteration(double best, int iter, List<Pair> tourCoords)
        {
            Dispatcher.Invoke(() =>
            {
                IterationLabel.Content = "Iteration: " + iter;
                BestLabel.Content = "Best distance: " + Math.Round(best, 4);

                var mainWindowViewModel = DataContext as MainWindowViewModel;

                if (mainWindowViewModel._model.AlgorithmEnded) WaitingLabel.Content = "Route found. Close the window please.";
                else WaitingLabel.Content = "Recalculating... Please Wait.";

                if (tourCoords == null) return;

                var size = tourCoords.Count;
                var startCoord = tourCoords[0];
                CanvasGrid.Children.Clear();

                for (var i = 1; i < size; ++i)
                {
                    var endCoord = tourCoords[i];

                    var link = new Line
                    {
                        X1 = startCoord.X() + 2,
                        Y1 = startCoord.Y() + 2,
                        X2 = endCoord.X() + 2,
                        Y2 = endCoord.Y() + 2,

                        Stroke = Brushes.Blue
                    };

                    CanvasGrid.Children.Add(link);
                    startCoord = tourCoords[i];
                }

                var finalLink = new Line
                {
                    X1 = tourCoords[0].X() + 2,
                    Y1 = tourCoords[0].Y() + 2,
                    X2 = tourCoords[size - 1].X() + 2,
                    Y2 = tourCoords[size - 1].Y() + 2,
                    Stroke = Brushes.Blue
                };

                CanvasGrid.Children.Add(finalLink);
                DisplayNodes(tourCoords);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CanvasGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindowViewModel = DataContext as MainWindowViewModel;
            mainWindowViewModel.OpenCommand.Execute(null);
            mainWindowViewModel.RunCommand.Execute(null);
        }

        private void DisplayGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}