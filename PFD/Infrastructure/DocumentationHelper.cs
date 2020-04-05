using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace PFD
{
    public static class DocumentationHelper
    {
        public static DirectoryInfo CreateDocumentationFolder(string parent_folder, string projectNumber)
        {
            int version = 1;
            string documentationFolder = $"{projectNumber}_version_{version}";
            while (Directory.Exists($"{parent_folder}\\{documentationFolder}"))
            {
                version++;
                documentationFolder = $"{projectNumber}_version_{version}";
            }

            DirectoryInfo di = Directory.CreateDirectory($"{parent_folder}\\{documentationFolder}");
            return di;
        }

        public static void FindCNCPath(CPlate plate)
        {
            List<Point> points = null;

            if (plate.ScrewArrangement == null) return; // Screw arrangmenet must exists

            points = plate.ScrewArrangement.HolesCentersPoints2D.ToList();
            
            if (points == null || points.Count == 0)
            {                
                return;
            }

            // Calculate size of plate and width to height ratio to set size of "salesman" algorthim window
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
            Drawing2D.CalculateModelLimits(plate.ScrewArrangement.HolesCentersPoints2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            double fWidth = fTempMax_X - fTempMin_X;
            double fHeigth = fTempMax_Y - fTempMin_Y;
            double fHeightToWidthRatio = fHeigth / fWidth;

            // Add coordinates of drilling machine start point
            points.Insert(0, new System.Windows.Point(0, 0));

            TwoOpt.WindowRunSalesman w = new TwoOpt.WindowRunSalesman(points, fHeightToWidthRatio);
            TwoOpt.MainWindowViewModel viewModel = w.DataContext as TwoOpt.MainWindowViewModel;

            viewModel.OpenCommand.Execute(null);
            viewModel.RunCommand.Execute(null);
            while (!viewModel._model.AlgorithmEnded) { Thread.Sleep(1000); }

            MessageBox.Show("END.");
            


            //w.Show();
            //w.Closing += Salesman_Closing;
        }

        //private static void Salesman_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    TwoOpt.WindowRunSalesman w = sender as TwoOpt.WindowRunSalesman;
        //    TwoOpt.MainWindowViewModel viewModel = w.DataContext as TwoOpt.MainWindowViewModel;

        //    List<System.Windows.Point> PathPoints = new List<System.Windows.Point>(viewModel.RoutePoints.Count);
        //    for (int i = 0; i < viewModel.RoutePoints.Count; i++)
        //    {
        //        PathPoints.Add(viewModel.RoutePoints[viewModel._model._tour.GetCities()[i]]);
        //    }

            

        //    Frame2DWidth = Frame2D.ActualWidth;
        //    Frame2DHeight = Frame2D.ActualHeight;
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

        //    if (vm.ComponentTypeIndex == 1) //ma to vyznam iba pre Plates
        //    {
        //        // Set drilling route points
        //        vm.DrillingRoutePoints = PathPoints;
        //        // Enable button to display of CNC drilling file
        //        BtnShowCNCDrillingFile.IsEnabled = true;
        //        // Update plate data and display
        //        UpdateAndDisplayPlate();
        //        vm.SetComponentProperties(plate);
        //    }

        //}
    }
}
