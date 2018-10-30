using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATH
{
    public class Vector2D
    {
        public double X;
        public double Y;

        // Constructors.
        public Vector2D(double x, double y) { X = x; Y = y; }
        public Vector2D() : this(double.NaN, double.NaN) { }

        public static Vector2D operator -(Vector2D v, Vector2D w)
        {
            return new Vector2D(v.X - w.X, v.Y - w.Y);
        }

        public static Vector2D operator +(Vector2D v, Vector2D w)
        {
            return new Vector2D(v.X + w.X, v.Y + w.Y);
        }

        public static double operator *(Vector2D v, Vector2D w)
        {
            return v.X * w.X + v.Y * w.Y;
        }

        public static Vector2D operator *(Vector2D v, double mult)
        {
            return new Vector2D(v.X * mult, v.Y * mult);
        }

        public static Vector2D operator *(double mult, Vector2D v)
        {
            return new Vector2D(v.X * mult, v.Y * mult);
        }

        public double Cross(Vector2D v)
        {
            return X * v.Y - Y * v.X;
        }

        public override bool Equals(object obj)
        {
            var v = (Vector2D)obj;
            return (X - v.X).IsZero() && (Y - v.Y).IsZero();
        }
    }

    public static class Extensions
    {
        private const double Epsilon = 1e-10;

        public static bool IsZero(this double d)
        {
            return Math.Abs(d) < Epsilon;
        }
    }


}
