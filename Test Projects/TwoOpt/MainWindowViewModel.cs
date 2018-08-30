using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace TwoOpt
{
   public class MainWindowViewModel : BaseViewModel
   {
      public readonly Model _model;
      private ICommand _openCommand;
      private ICommand _runCommand;

      public List<Point> RoutePoints;

      public MainWindowViewModel()
      {
         _model = new Model();
         _model.TourUpdated += ModelOnTourUpdated;
      }

      public IMainWindow MainWindow { get; set; }

      public string Title
      {
         get { return _model.Title; }
         set
         {
            _model.Title = value;
            OnPropertyChanged("Title");
         }
      }

      public ICommand OpenCommand
      {
            //get
            //{
            //   return _openCommand ?? (_openCommand = new RelayCommand(
            //             x =>
            //             {
            //                var openFileDialog1 = new OpenFileDialog
            //                {
            //                   InitialDirectory = "c:\\",
            //                   Filter = @"txt files (*.tsp)|*.tsp|All files (*.*)|*.*",
            //                   FilterIndex = 2,
            //                   RestoreDirectory = true
            //                };

            //                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            //                try
            //                {
            //                   Stream stream;
            //                   // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            //                   if ((stream = openFileDialog1.OpenFile()) == null) return;

            //                   var height = MainWindow.GridHeight();
            //                   var width = MainWindow.GridWidth();

            //                   _model.CancelJobs();
            //                   _model.InitMatrix(stream, height, width);
            //                   Title = _model.Title;
            //                   DisplayNodes();
            //                   MainWindow.UpdateIteration(0, 0, null);
            //                }
            //                catch (Exception ex)
            //                {
            //                   MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            //                }
            //             }));
            //}
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(
                          x =>
                          {
                              var height = MainWindow.GridHeight();
                              var width = MainWindow.GridWidth();

                              _model.CancelJobs();
                              _model.InitMatrix(RoutePoints, height, width);
                              Title = _model.Title;
                              DisplayNodes();
                              MainWindow.UpdateIteration(0, 0, null);

                          }));
            }
        }

      public ICommand RunCommand
      {
         get
         {
            return _runCommand ?? (_runCommand = new RelayCommand(
                      x => { _model.Run(); }));
         }
      }

      public List<Pair> Lines => _model.TourCoords;

      private void ModelOnTourUpdated(object sender, EventArgs<Tuple<double, int>> eventArgs)
      {
         var iter = eventArgs.Value.Item2;
         var best = eventArgs.Value.Item1;

         Update(best, iter);
      }

      public IEnumerable<string> ReadLines(Func<Stream> streamProvider,
         Encoding encoding)
      {
         using (var stream = streamProvider())
         {
            using (var reader = new StreamReader(stream, encoding))
            {
               string line;
               while ((line = reader.ReadLine()) != null)
                  yield return line;
            }
         }
      }

      private void DisplayNodes()
      {
         MainWindow?.PlotNodes(_model.DisplayCoords);
      }

      private void Update(double best, int iter)
      {
         MainWindow.UpdateIteration(best, iter, _model.TourCoords);
      }
   }
}