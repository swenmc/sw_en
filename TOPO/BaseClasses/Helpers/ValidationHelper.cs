using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BaseClasses.Helpers
{
    public static class GeometryHelper
    {
        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        public static double FindDistanceToSegment(
            PointF pt, PointF p1, PointF p2, out PointF closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }


        //zdroj: https://stackoverflow.com/questions/4858264/find-the-distance-from-a-3d-point-to-a-line-segment
        /**
         * Calculates the euclidean distance from a point to a line segment.
         *
         * @param v     the point
         * @param a     start of line segment
         * @param b     end of line segment 
         * @return      distance from v to line segment [a,b]
         *
         * @author      Afonso Santos
         */
        public static double distanceToSegment(Vector3D v, Vector3D a, Vector3D b)
        {

            Vector3D ab = Vector3D.Subtract(b, a);
            Vector3D av = Vector3D.Subtract(v, a);

            if (Vector3D.DotProduct(av, ab) <= 0.0)           // Point is lagging behind start of the segment, so perpendicular distance is not viable.
                return  modulus(av);         // Use distance to start of segment instead.

            Vector3D bv = Vector3D.Subtract(v, b);

            if (Vector3D.DotProduct(bv, ab) >= 0.0)           // Point is advanced past the end of the segment, so perpendicular distance is not viable.
                return modulus(bv);         // Use distance to end of the segment instead.

            //(ab.cross(av)).modulus() / ab.modulus();       // Perpendicular distance of point to segment.
            return modulus(Vector3D.CrossProduct(ab, av)) / modulus(ab);       // Perpendicular distance of point to segment.
        }

        private static double modulus(Vector3D v)
        {
            //zeby to bolo v.LengthSquared ???
            return Math.Sqrt(Vector3D.DotProduct(v,v));
        }

    }
}
