using System;
using System.Collections.Generic;

namespace TwoOpt
{
   public class Tour
   {
      static readonly Random Random = new Random();

      private readonly List<int> _cities = new List<int>();
      private CoordMatrix _mat = new CoordMatrix();

      public void SetMatrix(CoordMatrix mat)
      {
         _mat = mat;
      }

      public void Reset()
      {
         _cities.Clear();
      }

      // Implementing Fisher–Yates shuffle	
      private static void Shuffle<T>(IList<T> array)
      {
         var n = array.Count;
         for (var i = 1; i < n; i++)  //changed to 1
         {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            var r = i + Random.Next(n - i);
            var t = array[r];
            array[r] = array[i];
            array[i] = t;
         }
      }

      // Generate arbitrary tour
      public void CreateRandomTour()
      {
         Reset();

         var cityArray = new int[_mat.Size()];

         for (var i = 0; i < _mat.Size(); i++)
         {
            cityArray[i] = i;
         }

         Shuffle(cityArray);

         for (var i = 0; i < _mat.Size(); i++)
         {
            _cities.Add(cityArray[i]);
         }
      }

      public int TourSize()
      {
         return _cities.Count;
      }

      public List<int> GetCities()
      {
         return _cities;
      }

      public void SetCity(int index, int value)
      {
         _cities[index] = value;
      }

      // Get the tour city, given the index passed to it
      public int GetCity(int index)
      {
         var node = _cities[index];
         return node;
      }
       
      // Get total distance of tour
      public double TourDistance()
      {
         var dist = 0.0;

         var size = _cities.Count;
         int c1, c2;

         for (var i = 0; i < size - 1; i++)
         {
            c1 = _cities[i];
            c2 = _cities[i + 1];
            dist += Distance(c1, c2);
         }

         //We do not need to go back to beginning city
         // And back to the beginning city
         //c1 = _cities[size - 1];
         //c2 = _cities[0];
         //dist += Distance(c1, c2);

         return dist;
      }

      public void CreateTour()
      {
         for (var i = 0; i < _mat.Size(); i++)
         {
            _cities.Add(i);
         }
      }

      public void SetTour(IReadOnlyList<int> cities)
      {
         var size = _cities.Count;

         for (var i = 0; i < size; ++i)
         {           
            _cities[i] = cities[i];
         }
      }

      // Get tour from the set of nearest neighbours
      public void CreateNearestNeighbourTour()
      {
         Reset();

         var citySet = new HashSet<int>();

         for (var i = 0; i < _mat.Size(); i++)
         {
            citySet.Add(i);
         }

         var city = 0;

         for (var i = 1; i <= _mat.Size(); i++)
         {
            // Add city to tour
            _cities.Add(city);

            // Remove city from unique set
            citySet.Remove(city);
            city = GetNearestNeighbour(city, citySet);
         }
      }

      public int GetNearestNeighbour(int c, HashSet<int> cset)
      {
         var city = 0;

         // Get minimum distance node
         double minDist = _mat.Infinity;

         foreach (var n in cset)
         {
            if (n == c) continue;

            var dist = Distance(c, n);

            if (dist < minDist)
            {
               city = n;
               minDist = dist;
            }
         }

         return city;
      }

      // Get distance bewteen selected cities
      private double Distance(int c1, int c2)
      {
         return _mat.Distance(c1, c2);
      }
   }
}