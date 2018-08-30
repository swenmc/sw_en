using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace TwoOpt
{
   public class Model
   {
      private readonly BackgroundWorker _worker = new BackgroundWorker();
      private readonly CoordMatrix _coordMatrix = new CoordMatrix();
      private Tour _tour, _newTour;
      private double _width, _height;
      public string Title { get; set; }

      public List<Pair> DisplayCoords = new List<Pair>();
      public List<Pair> TourCoords = new List<Pair>();

      public event EventHandler<EventArgs<Tuple<double,int>>> TourUpdated;

      public Model()
      {
         Title = "Two-Opt Algorithm: ";

         _worker.DoWork += UpdateTour;
         _worker.WorkerSupportsCancellation = true;
      }

      public void CancelJobs()
      {
         _worker.CancelAsync();
      }

      public void Run()
      {        
         Initialize(1000);        
         _tour.CreateNearestNeighbourTour();
         SetTourCoords();

         if (!_worker.IsBusy) _worker.RunWorkerAsync();
      }

      // Do all 2-opt combinations
      private void TwoOpt()
      {
         // Get tour size
         var size = _tour.TourSize();

         //CHECK THIS!!		
         for (var i = 0; i < size; i++)  //changed to i = 1 
         {
            _newTour.SetCity(i, _tour.GetCity(i));
         }

         // repeat until no improvement is made 
         var improve = 0;
         var iteration = 0;

         while (improve < 500)
         {
            var bestDistance = _tour.TourDistance();

            for (var i = 1; i < size - 1; i++)
            {
               for (var k = i + 1; k < size; k++)
               {
                  TwoOptSwap(i, k);
                  iteration++;

                  var newDistance = _newTour.TourDistance();

                  if (!(newDistance < bestDistance)) continue;

                  // Improvement found so reset
                  improve = 0;

                  for (var j = 0; j < size; j++)
                  {
                     _tour.SetCity(j, _newTour.GetCity(j));
                  }

                  bestDistance = newDistance;

                  // Update the display
                  SetTourCoords();

                  TourUpdated.Raise(this, new Tuple<double, int>( bestDistance, iteration));
               }
            }

            improve++;
         }         
      }

      private void TwoOptSwap(int i, int k)
      {
         var size = _tour.TourSize();

         // 1. take route[0] to route[i-1] and add them in order to new_route
         for (var c = 0; c <= i - 1; ++c)
         {
            _newTour.SetCity(c, _tour.GetCity(c));
         }

         // 2. take route[i] to route[k] and add them in reverse order to new_route
         var dec = 0;
         for (var c = i; c <= k; ++c)
         {
            _newTour.SetCity(c, _tour.GetCity(k - dec));
            dec++;
         }

         // 3. take route[k+1] to end and add them in order to new_route
         for (var c = k + 1; c < size; ++c)
         {
            _newTour.SetCity(c, _tour.GetCity(c));
         }
      }

      public void InitMatrix(Stream stream, double height, double width)
      {
         using (var reader = new StreamReader(stream))
         {
            var lines = reader.ReadToEnd()
               .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            _coordMatrix.ClearCoordinates();

            var lineCount = 0;
            foreach (var line in lines)
            {              
               lineCount++;
               UpdateMatrix(line, lineCount);
            }

            _coordMatrix.SetDistanceMatrix();
            SetDisplayCoords(height, width);
            Title = _coordMatrix.GetTitle();
         }
      }
        public void InitMatrix(List<Point> routePoints, double height, double width)
        {
            _coordMatrix.ClearCoordinates();

            foreach (Point p in routePoints)
            {
                _coordMatrix.AddCoordinate(p.X, p.Y);
            }
            _coordMatrix.SetDistanceMatrix();
            SetDisplayCoords(height, width);
            Title = _coordMatrix.GetTitle();
        }


        private void SetTourCoords()
      {
         var maxX = _coordMatrix.MaxX();
         var minX = _coordMatrix.MinX();
         var maxY = _coordMatrix.MaxY();
         var minY = _coordMatrix.MinY();

         TourCoords.Clear();

         foreach (var city in _tour.GetCities())
         {
            var coord = _coordMatrix.GetCoordinate(city);

            var xc1 = coord.X();
            var yc1 = coord.Y();

            var xn1 = (xc1 - minX) / (float)(maxX - minX);
            var yn1 = 1.0 - (yc1 - minY) / (float)(maxY - minY);

            var xcoord1 = (xn1 * Math.Abs(_width));
            var ycoord1 = (yn1 * Math.Abs(_height));

            var newCoord = new Pair(xcoord1, ycoord1);

            TourCoords.Add(newCoord);
         }         
      }

      private void SetDisplayCoords(double height, double width)
      {
         _width = width;
         _height = height;

         var maxX = _coordMatrix.MaxX();
         var minX = _coordMatrix.MinX();
         var maxY = _coordMatrix.MaxY();
         var minY = _coordMatrix.MinY();

         DisplayCoords.Clear();

         for (var i = 0; i < _coordMatrix.Size(); ++i)
         {
            var coord = _coordMatrix.GetCoordinate(i);

            var xc1 = coord.X();
            var yc1 = coord.Y();

            var xn1 = (xc1 - minX)/(float) (maxX - minX);
            var yn1 = 1.0 - (yc1 - minY)/(float) (maxY - minY);

            var xcoord1 = (xn1*Math.Abs(width));
            var ycoord1 = (yn1*Math.Abs(height));

            var newCoord = new Pair(xcoord1, ycoord1);

            DisplayCoords.Add(newCoord);
         }
      }

      public void Initialize(int iter)
      {
         _tour = new Tour();
         _newTour = new Tour();
         _tour.SetMatrix(_coordMatrix);
         _newTour.SetMatrix(_coordMatrix);
         _newTour.CreateTour();        
      }

      private void UpdateTour(object sender, DoWorkEventArgs e)
      {
         TwoOpt();
      }

      public void UpdateMatrix(string strLine, int line)
      {
         _coordMatrix.UpdateMatrix(strLine, line);
      }
   }
}