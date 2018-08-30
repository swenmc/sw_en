using System;
using System.Collections.Generic;

namespace TwoOpt
{
   public class CoordMatrix
   {
      public enum EdgeWeightType
      {
         Att,
         Euc_2D
      }

      public readonly List<Pair> _coords = new List<Pair>();
      private readonly Dictionary<string, double> _distMatrix = new Dictionary<string, double>();
      private EdgeWeightType _edgeWeightType;
      private double _maxx;
      private double _maxy;
      private double _minx;
      private double _miny;

      private string _title;
      public double Infinity = double.MaxValue;

      public CoordMatrix()
      {
         _minx = Infinity;
         _miny = Infinity;
         _maxx = -Infinity;
         _maxy = -Infinity;
         _edgeWeightType = EdgeWeightType.Euc_2D;

         _title = "";
      }

      public void UpdateMatrix(string strLine, int line)
      {
         strLine = strLine.Trim();
      
         var tokens = strLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

         // Read file title
         if (tokens[0].ToUpper().Contains("NAME"))
         {           
            if (tokens.Length < 3)
            {
               SetTitle(tokens[1]);
            }
            else
            {
               SetTitle(tokens[2]);
            }
         }

         // Read edge weight type
         if (tokens[0].ToUpper().Contains("EDGE_WEIGHT_TYPE"))
         {
            var length = tokens.Length;

            if (length > 2)
            {
               if (tokens[2].ToUpper().Equals("ATT"))
               {
                  SetEdgeWeightType("ATT");
               }
               else if (tokens[2].Equals("EUC_2D"))
               {
                  SetEdgeWeightType("EUC_2D");
               }
            }
            else if (length > 1)
            {
               if (tokens[1].ToUpper().Equals("ATT"))
               {
                  SetEdgeWeightType("ATT");
               }
               else if (tokens[1].Equals("EUC_2D"))
               {
                  SetEdgeWeightType("EUC_2D");
               }
            }
           
         }

         if (line < 7 || (tokens.Length == 1 && strLine.ToUpper() != "EOF"))
         {
            return;
         }

         if (strLine.ToUpper() == "EOF")
         {
            return;
         }

         // Fill in city data
         //int node = Integer.parseInt( tokens[0] );
         int x = (int) double.Parse(tokens[1]);        
         int y = (int) double.Parse(tokens[2]);

         AddCoordinate(x, y);
      }

      public double MaxX()
      {
         return _maxx;
      }

      public double MinX()
      {
         return _minx;
      }

      public double MaxY()
      {
         return _maxy;
      }

      public double MinY()
      {
         return _miny;
      }

      // Tokenize the input string
      public void SetTitle(string title)
      {
         _title = title;
      }

      public string GetTitle()
      {
         return _title;
      }

      public void SetEdgeWeightType(string type)
      {
         _edgeWeightType = EdgeWeightType.Att;

         if (type == "ATT")
            _edgeWeightType = EdgeWeightType.Att;
         else if (type == "EUC_2D")
            _edgeWeightType = EdgeWeightType.Euc_2D;
      }

      public void ClearCoordinates()
      {
         _coords.Clear();

         _minx = Infinity;
         _miny = Infinity;
         _maxx = -Infinity;
         _maxy = -Infinity;
      }

      public void AddCoordinate(double x, double y)
      {
         _coords.Add(new Pair(x, y));

         // Get node extremities
         if (x > _maxx) _maxx = x;
         if (y > _maxy) _maxy = y;
         if (x < _minx) _minx = x;
         if (y < _miny) _miny = y;
      }

      public Pair GetCoordinate(int index)
      {
         return _coords[index];
      }

      public int Size()
      {
         return _coords.Count;
      }

      // Set up distance matrix between node pairs
      public void SetDistanceMatrix()
      {
         for (var i = 0; i < (int) _coords.Count - 1; i++)
         {
            // Get first node coordinate
            var p1 = _coords[i];
            var x1 = p1.X();
            var y1 = p1.Y();

            for (var j = i + 1; j < (int) _coords.Count; j++)
            {
               // Get second node coordinate
               var p2 = _coords[j];
               var x2 = p2.X();
               var y2 = p2.Y();

               // Get Euclidean distance between nodes
               var dist = 0.0;
               if (_edgeWeightType == EdgeWeightType.Att)
                  dist = CalcPseudoEuclidDistance(x1, x2, y1, y2);
               else
                  dist = (double) Math.Sqrt((double) (x1 - x2)*(x1 - x2) +
                                            (double) (y1 - y2)*(y1 - y2));

               // Map the distance to node pair				
               var s = i + "," + j;
               _distMatrix[s] = dist;
            }
         }
      }

      // For edge weight type 'ATT'
      // Pseudo Euclidean distance
      private int CalcPseudoEuclidDistance(double x1, double x2, double y1, double y2)
      {
         int dij;

         var xd = x1 - x2;
         var yd = y1 - y2;

         double rij = Math.Sqrt((xd*xd + yd*yd)/10.0);

         var tij = Round(rij);

         if (tij < rij)
            dij = tij + 1;
         else
            dij = tij;

         return dij;
      }

      private static int Round(double n)
      {
         return (int) (n + 0.5);
      }

      // Get Euclidean distance between two cities
      public double Distance(int c1, int c2)
      {
         // Ensure node ids in ascending order
         if (c1 < c2)
         {
            var s = c1 + "," + c2;
            var dist = _distMatrix[s];
            return dist;
         }
         else
         {
            var s = c2 + "," + c1;
            var dist = _distMatrix[s];
            return dist;
         }
      }
   }
}