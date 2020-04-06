using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace TwoOpt
{
    public class ModelSync
    {        
        private readonly CoordMatrix _coordMatrix = new CoordMatrix();
        public Tour _tour, _newTour;
        private double _width, _height;
        //public string Title { get; set; }

        public bool AlgorithmEnded;

        //public List<Pair> DisplayCoords = new List<Pair>();
        public List<Pair> TourCoords = new List<Pair>();

        //public event EventHandler<EventArgs<Tuple<double, int>>> TourUpdated;

        public ModelSync()
        {
            AlgorithmEnded = false;
        }

        public void Run(List<Point> routePoints, double width, double height)
        {
            _width = width;
            _height = height;
            InitMatrix(routePoints);

            Initialize();            
            _tour.CreateRandomTour();
            SetTourCoords();

            AlgorithmEnded = false;

            TwoOpt();
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
            var bestDistance = _tour.TourDistance();
            while (improve < 20)
            {
                bestDistance = _tour.TourDistance();

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

                        //TourUpdated.Raise(this, new Tuple<double, int>(bestDistance, iteration));
                    }
                }

                improve++;
            }

            AlgorithmEnded = true;
            //TourUpdated.Raise(this, new Tuple<double, int>(bestDistance, iteration));
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
        
        public void InitMatrix(List<Point> routePoints)
        {
            _coordMatrix.ClearCoordinates();

            foreach (Point p in routePoints)
            {
                _coordMatrix.AddCoordinate(p.X, p.Y);
            }
            _coordMatrix.SetDistanceMatrix();
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

        

        public void Initialize()
        {
            _tour = new Tour();
            _newTour = new Tour();
            _tour.SetMatrix(_coordMatrix);
            _newTour.SetMatrix(_coordMatrix);
            _newTour.CreateTour();
        }
    }
}