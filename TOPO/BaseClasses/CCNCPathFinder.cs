using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class CCNCPathFinder
    {
        List<CLine2D> MLines;
        List<Point> MPoints;

        public List<CLine2D> Lines
        {
            get
            {
                if (MLines == null) MLines = new List<CLine2D>();
                return MLines;
            }

            set
            {
                MLines = value;
            }
        }

        public List<Point> Points
        {
            get
            {
                if (MPoints == null) MPoints = new List<Point>();
                return MPoints;
            }

            set
            {
                MPoints = value;
            }
        }

        public CCNCPathFinder()
        { }

        public CCNCPathFinder(CPlate plate)
        {
            if (plate.HolesCentersPoints2D != null)
            {
                for (int i = 0; i < plate.HolesCentersPoints2D.Length / 2; i++)
                {
                    Points.Add(new Point(plate.HolesCentersPoints2D[i, 0], plate.HolesCentersPoints2D[i, 1]));
                }
            }
            
            /*
            Points.Add(new Point(1, 1));
            Points.Add(new Point(2, 2));
            Points.Add(new Point(3, 3));
            Points.Add(new Point(4, 4));
            Points.Add(new Point(4, 5));
            Points.Add(new Point(3, 6));
            Points.Add(new Point(3, 1));
            Points.Add(new Point(1, 3));
            */
            IEnumerable<IEnumerable<Point>> res = GetPermutations(Points, Points.Count);


            List<List<CLine2D>> allLines = new List<List<CLine2D>>();
            int count = 0;
            foreach (IEnumerable<Point> ie in res)
            {
                allLines.Add(GetLinesFromPoints(ie.ToList()));                
                //String s = "";
                //foreach (Point p in ie)
                //{                    
                //    s += string.Format("[{0},{1}],", p.X, p.Y);
                //}
                //Console.WriteLine(s);
                count++;
            }

            List<List<CLine2D>> noIntersectionPolyLines = GetAllPolylinesWithNoIntersection(allLines);
            List<CLine2D>  shortestPolyline = GetShortestPolyLine(noIntersectionPolyLines);
            Console.WriteLine("Number of rows: "+ count);
            Console.ReadLine();
        }

        private static List<CLine2D> GetLinesFromPoints(List<Point> points)
        {
            List<CLine2D> lines = new List<CLine2D>();
            for (int i = 1; i < points.Count; i++)
            {
                lines.Add(new CLine2D(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y));
            }
            return lines;
        }

        

        private static List<List<CLine2D>> GetAllPolylinesWithNoIntersection(List<List<CLine2D>> allLines)
        {
            List<List<CLine2D>> polyLinesNoIntersection = new List<List<CLine2D>>();
            foreach (List<CLine2D> polyline in allLines)
            {
                if (!LinesIntersects(polyline)) polyLinesNoIntersection.Add(polyline);
            }
            return polyLinesNoIntersection;
        }
        private static List<CLine2D> GetShortestPolyLine(List<List<CLine2D>> polylines)
        {
            List<CLine2D> shortestPolyLine = null;
            double shortestDistance = double.MaxValue;
            foreach (List<CLine2D> polyline in polylines)
            {
                double d = GetPolyLineLength(polyline);
                Console.WriteLine("Distance: " + d);
                if (d < shortestDistance) shortestPolyLine = polyline;
            }
            return shortestPolyLine;
        }

        private static double GetPolyLineLength(List<CLine2D> polyline)
        {
            double length = 0;
            foreach (CLine2D l in polyline)
            {
                length += l.GetLineLength();
            }
            return length;
        }

        private static bool LinesIntersects(List<CLine2D> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[i].IntersectsLine(lines[j])) return true;
                }
            }
            return false;
        }

        public static List<List<T>> GetAllCombos<T>(List<T> list)
        {
            List<List<T>> result = new List<List<T>>();
            // head
            result.Add(new List<T>());
            result.Last().Add(list[0]);
            if (list.Count == 1)
                return result;
            // tail
            List<List<T>> tailCombos = GetAllCombos(list.Skip(1).ToList());
            tailCombos.ForEach(combo =>
            {
                result.Add(new List<T>(combo));
                combo.Add(list[0]);
                result.Add(new List<T>(combo));
            });
            return result;
        }

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }


    }
}
