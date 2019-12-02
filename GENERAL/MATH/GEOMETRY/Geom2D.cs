using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MATH
{
    public static class Geom2D
    {
        // Docasne kym sa neprevedu vsetky funkcie z float[,] na Point[]
        public static Point[] GetConvertedFloatToPointArray(float[,] fArray)
        {
            Point[] pArray = new Point[fArray.Length / 2];
            for (int i = 0; i < fArray.Length / 2; i++)
            {
                pArray[i].X = fArray[i, 0];
                pArray[i].Y = fArray[i, 1];
            }

            return pArray;
        }

        public static float[,] m_ArrfPointsCoord2D;
        public static List<Point> m_ListPointsCoord2D;

        // Transformation of coordinates
        // Polar to Carthesian, Input angle in degrees
        public static float GetPositionX_deg(float radius, float theta_deg)
        {
            return radius * (float)Math.Cos(theta_deg * Math.PI / 180);
        }

        public static float GetPositionY_CW_deg(float radius, float theta_deg)
        {
            // Clock-wise (for counterclock-wise change sign for vertical coordinate)
            return - radius * (float)Math.Sin(theta_deg * Math.PI / 180);
        }

        public static float GetPositionY_CCW_deg(float radius, float theta_deg)
        {
            // Counter Clock-wise
            return -GetPositionY_CW_deg(radius, theta_deg);
        }

        // Polar to Carthesian, Input angle in rad
        public static float GetPositionX_rad(float radius, float theta_rad)
        {
            return radius * (float)Math.Cos(theta_rad);
        }

        public static float GetPositionY_CW_rad(float radius, float theta_rad)
        {
            // Clock-wise (for counterclock-wise change sign for vertical coordinate)
            return -radius * (float)Math.Sin(theta_rad);
        }

        public static float GetPositionY_CCW_rad(float radius, float theta_rad)
        {
            // Counter Clock-wise
            return -GetPositionY_CW_rad(radius, theta_rad);
        }

        // Get rotated position of point
        // Input angle in radians
        public static float GetRotatedPosition_x_CCW_rad(float x, float y, double theta_rad)
        {
            return (float)(x * Math.Cos(theta_rad) - y * Math.Sin(theta_rad));
        }
        public static double GetRotatedPosition_x_CCW_rad(double x, double y, double theta_rad)
        {
            return (x * Math.Cos(theta_rad) - y * Math.Sin(theta_rad));
        }

        public static float GetRotatedPosition_y_CCW_rad(float x, float y, double theta_rad)
        {
            return (float)(x * Math.Sin(theta_rad) + y * Math.Cos(theta_rad));
        }
        public static double GetRotatedPosition_y_CCW_rad(double x, double y, double theta_rad)
        {
            return (x * Math.Sin(theta_rad) + y * Math.Cos(theta_rad));
        }

        public static float GetRotatedPosition_x_CW_rad(float x, float y, double theta_rad)
        {
            return (float)(x * Math.Cos(theta_rad) + y * Math.Sin(theta_rad));
        }

        public static float GetRotatedPosition_y_CW_rad(float x, float y, double theta_rad)
        {
            return (float)(x * - Math.Sin(theta_rad) + y * Math.Cos(theta_rad));
        }

        // Trasnform point
        public static void TransformPositions_CCW_deg(float x_centerOfRotation, float y_centerOfRotation, double theta_deg, ref Point[] pArray)
        {
            for (int i = 0; i < pArray.Length; i++)
                TransformPositions_CCW_deg(x_centerOfRotation, y_centerOfRotation, theta_deg, ref pArray[i]);
        }

        public static void TransformPositions_CCW_deg(float x_centerOfRotation, float y_centerOfRotation, double theta_deg, ref Point p)
        {
            TransformPositions_CCW_rad(x_centerOfRotation, y_centerOfRotation, theta_deg / 180f * Math.PI, ref p);
        }

        public static void TransformPositions_CCW_deg(Point centerOfRotation, double theta_deg, ref Point p)
        {
            TransformPositions_CCW_rad(centerOfRotation, theta_deg / 180f * Math.PI, ref p);
        }

        public static void TransformPositions_CCW_rad(Point centerOfRotation, double theta_rad, ref Point p)
        {
            TransformPositions_CCW_rad((float)centerOfRotation.X, (float)centerOfRotation.Y, theta_rad, ref p);
        }

        public static void TransformPositions_CCW_rad(float x_centerOfRotation, float y_centerOfRotation, double theta_rad, ref Point [] pArray)
        {
            for(int i = 0; i < pArray.Length; i++)
                TransformPositions_CCW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, ref pArray[i]);
        }

        public static void TransformPositions_CCW_rad(float x_centerOfRotation, float y_centerOfRotation, double theta_rad, ref Point p)
        {
            float px = (float)p.X;
            float py = (float)p.Y;
            TransformPositions_CCW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, ref px, ref py);

            p.X = px;
            p.Y = py;
        }

        public static void TransformPositions_CCW_rad(float x_centerOfRotation, float y_centerOfRotation, double theta_rad, ref float x, ref float y)
        {
            float px;
            float py;

            if (!MathF.d_equal(theta_rad, 0)) // Translate and rotate
            {
                px = (float)(Math.Cos(theta_rad) * (x - x_centerOfRotation) - Math.Sin(theta_rad) * (y - y_centerOfRotation) + x_centerOfRotation);
                py = (float)(Math.Sin(theta_rad) * (x - x_centerOfRotation) + Math.Cos(theta_rad) * (y - y_centerOfRotation) + y_centerOfRotation);
            }
            else // Only translate
            {
                px = x + x_centerOfRotation;
                py = y + y_centerOfRotation;
            }

            x = px;
            y = py;
        }
        public static void TransformPositions_CCW_deg(float x_centerOfRotation, float y_centerOfRotation, double theta_deg, ref double x, ref double y)
        {
            TransformPositions_CCW_rad(x_centerOfRotation, y_centerOfRotation, theta_deg / 180f * Math.PI, ref x, ref y);
        }

        public static void TransformPositions_CCW_rad(double x_centerOfRotation, double y_centerOfRotation, double theta_rad, ref double x, ref double y)
        {
            double px;
            double py;

            if (!MathF.d_equal(theta_rad, 0)) // Translate and rotate
            {
                px = (float)(Math.Cos(theta_rad) * (x - x_centerOfRotation) - Math.Sin(theta_rad) * (y - y_centerOfRotation) + x_centerOfRotation);
                py = (float)(Math.Sin(theta_rad) * (x - x_centerOfRotation) + Math.Cos(theta_rad) * (y - y_centerOfRotation) + y_centerOfRotation);
            }
            else // Only translate
            {
                px = x + x_centerOfRotation;
                py = y + y_centerOfRotation;
            }

            x = px;
            y = py;
        }

        public static void TransformPositions_CW_deg(double x_centerOfRotation, double y_centerOfRotation, double theta_deg, ref Point p)
        {
            double x = p.X;
            double y = p.Y;

            TransformPositions_CW_deg(x_centerOfRotation, y_centerOfRotation, theta_deg, ref x, ref y);

            p.X = x;
            p.Y = y;
        }
        public static Point TransformPositions_CW_deg(double x_centerOfRotation, double y_centerOfRotation, double theta_deg, Point p)
        {
            double x = p.X;
            double y = p.Y;

            TransformPositions_CW_deg(x_centerOfRotation, y_centerOfRotation, theta_deg, ref x, ref y);
            return new Point(x, y);
        }
        public static void TransformPositions_CW_deg(double x_centerOfRotation, double y_centerOfRotation, double theta_deg, ref double x, ref double y)
        {
            TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_deg / 180f * Math.PI, ref x, ref y);
        }

        public static void TransformPositions_CW_rad(double x_centerOfRotation, double y_centerOfRotation, double theta_rad, ref double x, ref double y)
        {
            double px;
            double py;

            if (!MathF.d_equal(theta_rad, 0)) // Translate and rotate
            {
                px = (Math.Cos(theta_rad) * (x - x_centerOfRotation) + Math.Sin(theta_rad) * (y - y_centerOfRotation) + x_centerOfRotation);
                py = (-Math.Sin(theta_rad) * (x - x_centerOfRotation) + Math.Cos(theta_rad) * (y - y_centerOfRotation) + y_centerOfRotation);
            }
            else  // Only translate
            {
                px = x + x_centerOfRotation;
                py = y + y_centerOfRotation;
            }

            x = px;
            y = py;
        }
        public static Point GetTransformedPoint_CW_rad(double x_centerOfRotation, double y_centerOfRotation, double theta_rad, Point p)
        {
            double px = p.X;
            double py = p.Y;

            TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, ref px, ref py);

            return new Point(px, py);
        }

        public static void TransformPositions_CW_rad(double x_centerOfRotation, double y_centerOfRotation, double theta_rad, ref Point p)
        {
            double px = p.X;
            double py = p.Y;

            TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, ref px, ref py);

            p.X = px;
            p.Y = py;
        }

        // Transform array
        //public static void TransformPositions_CW_deg(float x_centerOfRotation, float y_centerOfRotation, double theta_deg, ref float[,] array)
        //{
        //    TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_deg / 180f * Math.PI, ref array);
        //}
        //public static void TransformPositions_CW_deg(Point centerOfRotation, double theta_deg, ref List<Point> points)
        //{
        //    TransformPositions_CW_deg((float)centerOfRotation.X, (float)centerOfRotation.Y, theta_deg, ref points);
        //}
        public static void TransformPositions_CW_deg(float x_centerOfRotation, float y_centerOfRotation, double theta_deg, ref List<Point> points)
        {
            TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_deg / 180f * Math.PI, ref points);
        }
        public static void TransformPositions_CW_deg(float x_centerOfRotation, float y_centerOfRotation, double theta_deg, ref Point[] points)
        {
            TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_deg / 180f * Math.PI, ref points);
        }

        //public static void TransformPositions_CW_rad(double x_centerOfRotation, double y_centerOfRotation, double theta_rad, ref float [,] array)
        //{
        //    if (array != null)
        //    {
        //        for (int i = 0; i < array.Length / 2; i++)
        //            TransformPositions_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, ref array[i, 0], ref array[i, 1]);
        //    }
        //}
        public static void TransformPositions_CW_rad(float x_centerOfRotation, float y_centerOfRotation, double theta_rad, ref List<Point> points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = GetTransformedPoint_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, points[i]);
                }
            }
        }
        public static void TransformPositions_CW_rad(float x_centerOfRotation, float y_centerOfRotation, double theta_rad, ref Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = GetTransformedPoint_CW_rad(x_centerOfRotation, y_centerOfRotation, theta_rad, points[i]);
                }
            }
        }        

        // Mirror
        //public static void MirrorAboutY_ChangeXCoordinatesArray(ref float[,] array)
        //{
        //    if (array != null)
        //    {
        //        for (int i = 0; i < array.Length / 2; i++)
        //            array[i, 0] *= -1;
        //    }
        //}

        //public static void MirrorAboutX_ChangeYCoordinatesArray(ref float[,] array)
        //{
        //    if (array != null)
        //    {
        //        for (int i = 0; i < array.Length / 2; i++)
        //            array[i, 1] *= -1;
        //    }
        //}

        public static void MirrorAboutY_ChangeXCoordinates(ref List<Point> points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Point p = points[i];
                    p.X *= -1;
                    points[i] = p;
                }
            }
        }
        public static void MirrorAboutY_ChangeXCoordinates(ref Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X *= -1;
                }
            }
        }

        public static void MirrorAboutX_ChangeYCoordinates(ref List<Point> points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Point p = points[i];
                    p.Y *= -1;
                    points[i] = p;
                }
            }
        }
        public static void MirrorAboutX_ChangeYCoordinates(ref Point point)
        {
            point.Y *= -1;
        }
        public static List<Point> MirrorAboutX_ChangeYCoordinates(List<Point> points)
        {
            List<Point> mirroredPoints = new List<Point>();

            if (points != null)
            {
                foreach (Point p in points)
                {   
                    mirroredPoints.Add(new Point(p.X, p.Y * -1));
                }
            }
            return mirroredPoints;
        }
        public static List<Point> MirrorAboutX_ChangeYCoordinates(Point[] points)
        {
            List<Point> mirroredPoints = new List<Point>();

            if (points != null)
            {
                foreach (Point p in points)
                {
                    mirroredPoints.Add(new Point(p.X, p.Y * -1));
                }
            }
            return mirroredPoints;
        }
        
        

        public static void MirrorAboutX_ChangeYCoordinates(ref Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].Y *= -1;
                }
            }
        }

        // TODO - Ondrej - zjednotit typ objeku pouzivany pre 2D, asi by to mal byt Point (nie array [,] a pod)

        // Different data types transformation
        public static float[,] TransformPointToArrayCoord(List<Point> Points_input)
        {
            if (Points_input == null)
                throw new ArgumentNullException("Not inicialized list of points!");

            float[,] array = new float[Points_input.Count, 2];

            for (int i = 0; i < Points_input.Count; i++)
            {
                array[i, 0] = (float)Points_input[i].X;
                array[i, 1] = (float)Points_input[i].Y;
            }

            return array;
        }

        //public static List<Point> TransformArrayToPointCoord(float [,] array_input)
        //{
        //    if (array_input == null)
        //        throw new ArgumentNullException("Not inicialized array of point coordinates!");

        //    List <Point> listPoints = new List<Point>(array_input.Length / 2);

        //    for (int i = 0; i < array_input.Length / 2; i++)
        //    {
        //        listPoints.Add(new Point(array_input[i, 0], array_input[i, 1]));
        //    }

        //    return listPoints;
        //}

        public static List<Point> TransformArrayToList(Point[] array_input)
        {
            if (array_input == null) return null;
                //throw new ArgumentNullException("Not inicialized array of point coordinates!");

            List<Point> listPoints = new List<Point>(array_input.Length);

            for (int i = 0; i < array_input.Length; i++)
            {
                listPoints.Add(array_input[i]);
            }

            return listPoints;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get Basic 2D Shapes Coordinates

        // List
        // semicircle shape,
        // incomplete circle shape,
        // half rounded rectangle shape,
        // right triangle shape,
        // isosceles triangle shape,
        // equilateral triangle shape,
        // trapezium shape,
        // square shape,
        // pentagon shape,
        // hexagon shape,
        // heptagon shape,
        // octagon shape,
        // nonagon shape,
        // decagon shape, 
        // rhombus shape,
        // isosceles trapezium shape,
        // circle shape,
        // semicircle curve shape,
        // curve shape,
        // incomplete circle shape,
        // empty semicircle shape,
        // right trapezium shape


        #region Circle
        // Circle
        // Get Points Coordinates
        public static float[,] GetCirclePointCoordArray_CW(float fr, int iNumber)
        {
            m_ArrfPointsCoord2D = new float[iNumber, 2];

            for (int i = 0; i < iNumber; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX_deg(fr, i * 360 / iNumber);  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CW_deg(fr, i * 360 / iNumber);  // z
            }

            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetCirclePointCoordArray_CCW(float fr, int iNumber)
        {
            m_ArrfPointsCoord2D = new float[iNumber, 2];

            for (int i = 0; i < iNumber; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX_deg(fr, i * 360 / iNumber);  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CCW_deg(fr, i * 360 / iNumber);  // z
            }

            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetCirclePointCoord_CW(float fr, int iNumber)
        {
            m_ListPointsCoord2D = new List<Point>(iNumber);
            for (int i = 0; i < iNumber; i++)
            {
                m_ListPointsCoord2D.Add(new Point(GetPositionX_deg(fr, i * 360 / iNumber), GetPositionY_CW_deg(fr, i * 360 / iNumber)));                
            }
            return m_ListPointsCoord2D;
        }
        public static List<Point> GetCirclePointCoord_CCW(float fr, int iNumber)
        {
            m_ListPointsCoord2D = new List<Point>(iNumber);
            for (int i = 0; i < iNumber; i++)
            {
                m_ListPointsCoord2D.Add(new Point(GetPositionX_deg(fr, i * 360 / iNumber), GetPositionY_CCW_deg(fr, i * 360 / iNumber)));                
            }
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Arc
        // Arc
        public static float[,] GetArcPointCoordArray_CW_deg(float fr, float fStartAngle_deg, float fEndAngle_deg, int iNumber, bool bIncludeCentroid = true)
        {
            if (bIncludeCentroid)
            {
                // iNumber - number of points of section (arc + centroid)
                // iNumber - 1 - number of points arc
                m_ArrfPointsCoord2D = new float[iNumber, 2];  // Allocate Memory for whole section (all section points including centroid)

                // Decrease Number
                --iNumber;
                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }
            else
            {
                m_ArrfPointsCoord2D = new float[iNumber, 2];  // Allocate Memory for whole section (all section points excluding centroid)

                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }

            for (int i = 0; i < iNumber; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CW_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // z
            }

            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetArcPointCoordArray_CCW_deg(float fr, float fStartAngle_deg, float fEndAngle_deg, int iNumber, bool bIncludeCentroid = true)
        {
            if (bIncludeCentroid)
            {
                // iNumber - number of points of section (arc + centroid)
                // iNumber - 1 - number of points arc
                m_ArrfPointsCoord2D = new float[iNumber, 2];  // Allocate Memory for whole section (all section points including centroid)

                // Decrease Number
                --iNumber;
                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }
            else
            {
                m_ArrfPointsCoord2D = new float[iNumber, 2];  // Allocate Memory for whole section (all section points excluding centroid)

                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }

            for (int i = 0; i < iNumber; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CCW_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // z
            }

            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetArcPointCoord_CW_deg(float fr, float fStartAngle_deg, float fEndAngle_deg, int iNumber, bool bIncludeCentroid = true)
        {
            if (bIncludeCentroid)
            {
                // iNumber - number of points of section (arc + centroid)
                // iNumber - 1 - number of points arc
                m_ListPointsCoord2D = new List<Point>(iNumber);  // Allocate Memory for whole section (all section points including centroid)

                // Decrease Number
                --iNumber;
                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }
            else
            {
                m_ListPointsCoord2D = new List<Point>(iNumber);  // Allocate Memory for whole section (all section points excluding centroid)

                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }

            
            for (int i = 0; i < iNumber; i++)
            {
                Point p = new Point();
                p.X = GetPositionX_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // y
                p.Y = GetPositionY_CW_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // z
                m_ListPointsCoord2D.Add(p);
            }

            return m_ListPointsCoord2D;
        }
        public static List<Point> GetArcPointCoord_CCW_deg(float fr, float fStartAngle_deg, float fEndAngle_deg, int iNumber, bool bIncludeCentroid = true)
        {
            if (bIncludeCentroid)
            {
                // iNumber - number of points of section (arc + centroid)
                // iNumber - 1 - number of points arc
                m_ListPointsCoord2D = new List<Point>(iNumber);  // Allocate Memory for whole section (all section points including centroid)

                // Decrease Number
                --iNumber;
                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }
            else
            {
                m_ListPointsCoord2D = new List<Point>(iNumber);  // Allocate Memory for whole section (all section points excluding centroid)

                // iNumber - number of points of arc
                // iNumber - 1 - number of segments of arc
            }

            for (int i = 0; i < iNumber; i++)
            {
                Point p = new Point();
                p.X = GetPositionX_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // y
                p.Y = GetPositionY_CCW_deg(fr, fStartAngle_deg + i * (fEndAngle_deg - fStartAngle_deg) / (iNumber - 1));  // z
                m_ListPointsCoord2D.Add(p); 
            }

            return m_ListPointsCoord2D;
        }
        /*
         * Calculates the angle (in radians) between two vectors pointing outward from one center
         *
         * @param p0 first point
         * @param p1 second point
         * @param c center point
         */
        public static double GetAngle_rad(Point p0, Point p1, Point c)
        {
            var p0c = Math.Sqrt(Math.Pow(c.X - p0.X, 2) +
                                Math.Pow(c.Y - p0.Y, 2)); // p0->c (b)
            var p1c = Math.Sqrt(Math.Pow(c.X - p1.X, 2) +
                                Math.Pow(c.Y - p1.Y, 2)); // p1->c (a)
            var p0p1 = Math.Sqrt(Math.Pow(p1.X - p0.X, 2) +
                                 Math.Pow(p1.Y - p0.Y, 2)); // p0->p1 (c)
            return Math.Acos((p1c * p1c + p0c * p0c - p0p1 * p0p1) / (2 * p1c * p0c));
        }
        #endregion
        #region Ellipse
        // Ellipse
        // Get Points Coordinates
        /*
        * This functions returns an array containing 36 points to draw an
        * ellipse.
        *
        * @param fx {float} X coordinate
        * @param fy {float} Y coordinate
        * @param fa {float} Semimajor axis
        * @param fb {float} Semiminor axis
        * @param fangle {float} Angle of the ellipse in Degrees

        * Read more: http://www.answers.com/topic/ellipse#ixzz1UN0OIaGS
        */
        public static float[,] GetEllipsePointsArray(float fx, float fy, float fa, float fb, float fAngle, short isteps)
        {
            //if (isteps == null)
            //    isteps = 36;
            //if (fangle == null)
            //    fangle = 0f;

            m_ArrfPointsCoord2D = new float[isteps, 2];

            // Angle is given by Degree Value
            float fBeta = -fAngle * (MathF.fPI / 180f); //(Math.PI/180) converts Degree Value into Radians
            float fsinbeta = (float)Math.Sin(fBeta);
            float fcosbeta = (float)Math.Cos(fBeta);

            int iNodeTemp = 0; // Temporary Number of Current Point

            for (int i = 0; i < 360; i += 360 / isteps)
            {
                float falpha = i * (MathF.fPI / 180);
                float fsinalpha = (float)Math.Sin(falpha);
                float fcosalpha = (float)Math.Cos(falpha);

                // Clock-wise (for counterclock-wise change sign for vertical coordinate)
                m_ArrfPointsCoord2D[iNodeTemp, 0] = fx + (fa * fcosalpha * fcosbeta - fb * fsinalpha * fsinbeta);      // Point x-coordinate
                m_ArrfPointsCoord2D[iNodeTemp, 1] = fy - (fa * fcosalpha * fsinbeta + fb * fsinalpha * fcosbeta);      // Point y-coordinate

                iNodeTemp++;
            }
            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetEllipsePointCoordArray(float fa, float fb, float fAngle, short isteps)
        {
            //if (isteps == null)
            //    isteps = 36;

            m_ArrfPointsCoord2D = GetEllipsePointsArray(0.0f, 0.0f, fa, fb, fAngle, isteps);
            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetEllipsePointCoordArray(float fa, float fb, float fAngle)
        {
            m_ArrfPointsCoord2D = GetEllipsePointCoordArray(fa, fb, fAngle, 72);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetEllipsePoints(float fx, float fy, float fa, float fb, float fAngle, short isteps)
        {
            m_ListPointsCoord2D = new List<Point>(isteps);

            // Angle is given by Degree Value
            float fBeta = -fAngle * (MathF.fPI / 180f); //(Math.PI/180) converts Degree Value into Radians
            double fsinbeta = Math.Sin(fBeta);
            double fcosbeta = Math.Cos(fBeta);

            int iNodeTemp = 0; // Temporary Number of Current Point

            for (int i = 0; i < 360; i += 360 / isteps)
            {
                float falpha = i * (MathF.fPI / 180);
                double fsinalpha = Math.Sin(falpha);
                double fcosalpha = Math.Cos(falpha);

                // Clock-wise (for counterclock-wise change sign for vertical coordinate)
                m_ListPointsCoord2D.Add(new Point(fx + (fa * fcosalpha * fcosbeta - fb * fsinalpha * fsinbeta), fy - (fa * fcosalpha * fsinbeta + fb * fsinalpha * fcosbeta)));
                iNodeTemp++;
            }
            return m_ListPointsCoord2D;
        }
        public static List<Point> GetEllipsePointCoord(float fa, float fb, float fAngle, short isteps)
        {
            m_ListPointsCoord2D = GetEllipsePoints(0.0f, 0.0f, fa, fb, fAngle, isteps);
            return m_ListPointsCoord2D;
        }
        public static List<Point> GetEllipsePointCoord(float fa, float fb, float fAngle)
        {
            m_ListPointsCoord2D = GetEllipsePointCoord(fa, fb, fAngle, 72);
            return m_ListPointsCoord2D;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////

        public static float GetRadiusfromSideLength(float fa, short iNumEdges)
        {
            return fa / 2f * 1f / ((float)Math.Sin((2 * MathF.fPI) / (2 * iNumEdges)));
        }

        public static float GetIntRadiusfromSideLength(float fa, short iNumEdges)
        {
            return fa / 2f * 1f / ((float)Math.Tan((2 * MathF.fPI) / (2 * iNumEdges)));
        }

        public static float GetRegularPolygonInternalSideLength(float fa1, float fr1, float fr2)
        {
            return fa1 / fr1 * fr2;
        }

        // Regular Polygonal Shapes

        #region n-Polygon
        // (n) polygon
        public static float[,] GetPolygonPointCoordArray(float fa, short iNumEdges)
        {
            m_ArrfPointsCoord2D = new float[iNumEdges, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            for (int i = 0; i < iNumEdges; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX_deg(GetRadiusfromSideLength(fa, iNumEdges), i * 360 / iNumEdges);  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CW_deg(GetRadiusfromSideLength(fa, iNumEdges), i * 360 / iNumEdges);  // z
            }

            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetPolygonPointCoord(float fa, short iNumEdges)
        {
            m_ListPointsCoord2D = new List<Point>(iNumEdges);
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            for (int i = 0; i < iNumEdges; i++)
            {
                m_ListPointsCoord2D.Add(new Point(GetPositionX_deg(GetRadiusfromSideLength(fa, iNumEdges), i * 360 / iNumEdges), GetPositionY_CW_deg(GetRadiusfromSideLength(fa, iNumEdges), i * 360 / iNumEdges)));                
            }

            return m_ListPointsCoord2D;
        }
        #endregion
        #region Triangle
        // Triangle
        public static float[,] GetTrianEqLatPointCoord1Array(float fa)
        {
            m_ArrfPointsCoord2D = new float[3, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ArrfPointsCoord2D[0, 0] = 0f;                                   // y
            m_ArrfPointsCoord2D[0, 1] = 2f / 3f * (fa / 2f) * MathF.fSqrt3;   // z

            // Point No. 2
            m_ArrfPointsCoord2D[1, 0] = fa / 2f;                              // y
            m_ArrfPointsCoord2D[1, 1] = -1f / 3f * (fa / 2f) * MathF.fSqrt3;  // z

            // Point No. 3
            m_ArrfPointsCoord2D[2, 0] = -m_ArrfPointsCoord2D[1, 0];           // y
            m_ArrfPointsCoord2D[2, 1] = m_ArrfPointsCoord2D[1, 1];            // z

            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetTrianEqLatPointCoord2Array(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 3);
            return m_ArrfPointsCoord2D;
        }
        // Isosceles
        public static float[,] GetTrianIsosCelPointCoordArray(float fh, float fb)
        {
            m_ArrfPointsCoord2D = new float[3, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ArrfPointsCoord2D[0, 0] = 0f;               // y
            m_ArrfPointsCoord2D[0, 1] = 2f / 3f * fh;     // z

            // Point No. 2
            m_ArrfPointsCoord2D[1, 0] = fb / 2f;          // y
            m_ArrfPointsCoord2D[1, 1] = -1f / 3f * fh;    // z

            // Point No. 3
            m_ArrfPointsCoord2D[2, 0] = -m_ArrfPointsCoord2D[1, 0];  // y
            m_ArrfPointsCoord2D[2, 1] = m_ArrfPointsCoord2D[1, 1];   // z

            return m_ArrfPointsCoord2D;
        }
        // Right triangle (right-angled triangle, rectangled triangle)
        public static float[,] GetTrianRightAngPointCoordArray(float fh, float fb)
        {
            m_ArrfPointsCoord2D = new float[3, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ArrfPointsCoord2D[0, 0] = -fb / 3f;        // y
            m_ArrfPointsCoord2D[0, 1] = 2f / 3f * fh;     // z

            // Point No. 2
            m_ArrfPointsCoord2D[1, 0] = 2f / 3f * fb;     // y
            m_ArrfPointsCoord2D[1, 1] = -1f / 3f * fh;    // z

            // Point No. 3
            m_ArrfPointsCoord2D[2, 0] = m_ArrfPointsCoord2D[0, 0];  // y
            m_ArrfPointsCoord2D[2, 1] = m_ArrfPointsCoord2D[1, 1];  // z

            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetTrianEqLatPointCoord1(double fa)
        {
            m_ListPointsCoord2D = new List<Point>(3);

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ListPointsCoord2D.Add(new Point(0, 2.0 / 3.0 * (fa / 2.0) * MathF.fSqrt3));

            // Point No. 2
            m_ListPointsCoord2D.Add(new Point(fa / 2.0, -1.0 / 3.0 * (fa / 2.0) * MathF.fSqrt3));

            // Point No. 3
            m_ListPointsCoord2D.Add(new Point(-m_ListPointsCoord2D[1].X, m_ListPointsCoord2D[1].Y));

            return m_ListPointsCoord2D;
        }
        public static List<Point> GetTrianEqLatPointCoord2(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 3);
            return m_ListPointsCoord2D;
        }
        // Isosceles
        public static List<Point> GetTrianIsosCelPointCoord(float fh, float fb)
        {
            List<Point> m_ListPointsCoord2D = new List<Point>(3);

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ListPointsCoord2D.Add(new Point(0.0, 2f / 3f * fh));

            // Point No. 2            
            m_ListPointsCoord2D.Add(new Point(fb / 2f, -1f / 3f * fh));

            // Point No. 3            
            m_ListPointsCoord2D.Add(new Point(-m_ListPointsCoord2D[1].X, m_ListPointsCoord2D[1].Y));

            return m_ListPointsCoord2D;
        }
        // Right triangle (right-angled triangle, rectangled triangle)
        public static List<Point> GetTrianRightAngPointCoord(float fh, float fb)
        {
            m_ListPointsCoord2D = new List<Point>(3);
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ListPointsCoord2D.Add(new Point(-fb / 3f, 2f / 3f * fh));

            // Point No. 2            
            m_ListPointsCoord2D.Add(new Point(2f / 3f * fb, -1f / 3f * fh));

            // Point No. 3            
            m_ListPointsCoord2D.Add(new Point(m_ListPointsCoord2D[0].X, m_ListPointsCoord2D[1].Y));
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Square
        // Square (4)
        public static float[,] GetSquarePointCoord1Array(float fa)
        {
            m_ArrfPointsCoord2D = new float[4, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ArrfPointsCoord2D[0, 0] = -fa/2f;      // y
            m_ArrfPointsCoord2D[0, 1] = fa / 2f;     // z

            // Point No. 2
            m_ArrfPointsCoord2D[1, 0] = -m_ArrfPointsCoord2D[0, 0];   // y
            m_ArrfPointsCoord2D[1, 1] = m_ArrfPointsCoord2D[0, 1];    // z

            // Point No. 3
            m_ArrfPointsCoord2D[2, 0] = -m_ArrfPointsCoord2D[0, 0];  // y
            m_ArrfPointsCoord2D[2, 1] = -m_ArrfPointsCoord2D[0, 1];  // z

            // Point No. 4
            m_ArrfPointsCoord2D[3, 0] = m_ArrfPointsCoord2D[0, 0];   // y
            m_ArrfPointsCoord2D[3, 1] = -m_ArrfPointsCoord2D[0, 1];  // z

            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetSquarePointCoord2Array(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 4);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetSquarePointCoord1(float fa)
        {
            m_ListPointsCoord2D = new List<Point>(4);

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ListPointsCoord2D.Add(new Point(-fa / 2f, fa / 2f));
            // Point No. 2            
            m_ListPointsCoord2D.Add(new Point(-m_ListPointsCoord2D[0].X, m_ListPointsCoord2D[0].Y));

            // Point No. 3            
            m_ListPointsCoord2D.Add(new Point(-m_ListPointsCoord2D[0].X, -m_ListPointsCoord2D[0].Y));

            // Point No. 4            
            m_ListPointsCoord2D.Add(new Point(m_ListPointsCoord2D[0].X, -m_ListPointsCoord2D[0].Y));

            return m_ListPointsCoord2D;
        }
        public static List<Point> GetSquarePointCoord2(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 4);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Rectangle
        // Rectangle (4)
        public static float[,] GetRectanglePointCoordArray(float fh, float fb)
        {
            m_ArrfPointsCoord2D = new float[4, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ArrfPointsCoord2D[0, 0] = -fb / 2f;      // y
            m_ArrfPointsCoord2D[0, 1] = fh / 2f;       // z

            // Point No. 2
            m_ArrfPointsCoord2D[1, 0] = -m_ArrfPointsCoord2D[0, 0];   // y
            m_ArrfPointsCoord2D[1, 1] = m_ArrfPointsCoord2D[0, 1];    // z

            // Point No. 3
            m_ArrfPointsCoord2D[2, 0] = -m_ArrfPointsCoord2D[0, 0];  // y
            m_ArrfPointsCoord2D[2, 1] = -m_ArrfPointsCoord2D[0, 1];  // z

            // Point No. 4
            m_ArrfPointsCoord2D[3, 0] = m_ArrfPointsCoord2D[0, 0];   // y
            m_ArrfPointsCoord2D[3, 1] = -m_ArrfPointsCoord2D[0, 1];  // z

            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetRectanglePointCoord(float fh, float fb)
        {
            m_ListPointsCoord2D = new List<Point>(4);

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            m_ListPointsCoord2D.Add(new Point(-fb / 2f, fh / 2f));

            // Point No. 2            
            m_ListPointsCoord2D.Add(new Point(-m_ListPointsCoord2D[0].X, m_ListPointsCoord2D[0].Y));

            // Point No. 3            
            m_ListPointsCoord2D.Add(new Point(-m_ListPointsCoord2D[0].X, -m_ListPointsCoord2D[0].Y));

            // Point No. 4            
            m_ListPointsCoord2D.Add(new Point(m_ListPointsCoord2D[0].X, -m_ListPointsCoord2D[0].Y));

            return m_ListPointsCoord2D;
        }
        #endregion
        #region Pentagon
        // Pentagon (5)
        public static float[,] GetPentagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 5);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetPentagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 5);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Hexagon
        // Hexafgon (6)
        public static float[,] GetHexagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 6);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetHexagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 6);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Heptagon
        // Heptagon (7)
        public static float[,] GetHeptagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 7);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetHeptagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 7);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Octagon
        // Octagon  (8)
        public static float[,] GetOctagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 8);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetOctagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 8);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Nonagon
        // Nonagon  (9)
        public static float[,] GetNonagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 9);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetNonagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 9);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Decagon
        // Decagon  (10)
        public static float[,] GetDecagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 10);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetDecagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 10);
            return m_ListPointsCoord2D;
        }
        #endregion        
        #region Hendecagon
        // Hendecagon(11)
        public static float[,] GetHendecagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 11);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetHendecagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 11);
            return m_ListPointsCoord2D;
        }
        #endregion
        #region Dodecagon
        // Dodecagon (12)
        public static float[,] GetDodecagonPointCoordArray(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoordArray(fa, 12);
            return m_ArrfPointsCoord2D;
        }
        public static List<Point> GetDodecagonPointCoord(float fa)
        {
            m_ListPointsCoord2D = GetPolygonPointCoord(fa, 12);
            return m_ListPointsCoord2D;
        }
        #endregion

        #region AddCentroidPosition_Zero
        public static float[,] AddCentroidPosition_ZeroArray(float[,] fArrayEdgePoints)
        {
            float[,] fArrayEdgePointsAndCentroid = new float[fArrayEdgePoints.Length / 2 + 1, 2];

            for (int i = 0; i < fArrayEdgePoints.Length / 2; i++)
            {
                fArrayEdgePointsAndCentroid[i, 0] = fArrayEdgePoints[i, 0];
                fArrayEdgePointsAndCentroid[i, 1] = fArrayEdgePoints[i, 1];
            }

            // Centroid
            fArrayEdgePointsAndCentroid[fArrayEdgePoints.Length / 2, 0] = 0f;
            fArrayEdgePointsAndCentroid[fArrayEdgePoints.Length / 2, 1] = 0f;

            return fArrayEdgePointsAndCentroid;
        }
        public static List<Point> AddCentroidPosition_Zero(List<Point> EdgePoints)
        {
            List<Point> EdgePointsAndCentroid = new List<Point>(EdgePoints.Count);

            for (int i = 0; i < EdgePoints.Count; i++)
            {
                EdgePointsAndCentroid.Add(EdgePoints[i]);
            }
            // Centroid
            EdgePointsAndCentroid.Add(new Point(0, 0));

            return EdgePointsAndCentroid;
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Returns polygonal area - consider orientation (positive or negative)
        public static float SignedPolygonArea(Point [] Points2D)
        {
            // Add the first point to the end.
            int num_points = Points2D.Length;
            Point[] pts = new Point[num_points + 1];

            for (int i = 0; i < num_points; i++)
            {
                pts[i].X = Points2D[i].X;
                pts[i].Y = Points2D[i].Y;
            }

            pts[num_points].X = Points2D[0].X;
            pts[num_points].Y = Points2D[0].Y;

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area += (float)(
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2);
            }

            // Return the result.
            return area;
        }

        // Return the polygon's area in "square units."
        public static float PolygonArea(Point[] Points2D)
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polyogn is
            // oriented clockwise.
            return Math.Abs(SignedPolygonArea(Points2D));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get rotation angle (degree) in 2D environment

        public static float GetAlpha2D_CW(float fCoordStart1, float fCoordEnd1, float fCoordStart2, float fCoordEnd2)
        {
            // Clockwise rotation in 2D environment

            ///////////////////////////////////////////////////////////////////
            // len rozpracovane , nutne skontrolovat znamienka a vylepsit 
            ///////////////////////////////////////////////////////////////////

            if ((fCoordEnd1 >= fCoordStart1) && (fCoordEnd2 >= fCoordStart2))
            {
                // 1st Quadrant (0-90 degrees / resp. 0 - 0.5PI)
                return (float)Math.Atan((fCoordEnd2 - fCoordStart2) / (fCoordEnd1 - fCoordStart1));
            }
            else if ((fCoordEnd1 <= fCoordStart1) && (fCoordEnd2 >= fCoordStart2))
            {
                // 2nd Quadrant (90-180 degrees / resp. 0.5PI - PI)
                return (float)Math.PI / 2 + (float)Math.Atan((fCoordStart1 - fCoordEnd1) / (fCoordEnd2 - fCoordStart2));
            }
            else if ((fCoordEnd1 <= fCoordStart1) && (fCoordEnd2 <= fCoordStart2))
            {
                // 3rd Quadrant (180-270 degrees / resp. PI - 1.5PI)
                return (float)Math.PI + (float)Math.Atan((fCoordStart2 - fCoordEnd2) / (fCoordStart1 - fCoordEnd1));
            }
            else /*((fCoordEnd1 >= fCoordStart1) && (fCoordEnd2 <= fCoordStart2))*/
            {
                // 4th Quadrant (270-360 degrees / resp. 1.5PI - 2PI)
                return (1.5f * (float)Math.PI) + (float)Math.Atan((fCoordEnd1 - fCoordStart1) / (fCoordStart2 - fCoordEnd2));
            }
        }

        public static double GetAlpha2D_CW(double dLength_A, double dLength_B)
        {
            // Clockwise rotation in 2D environment
            // Left-handed system

            ///////////////////////////////////////////////////////////////////
            // len rozpracovane , nutne skontrolovat znamienka a vylepsit 
            ///////////////////////////////////////////////////////////////////

            // A - adjacent side of triangle
            // B - opposite side of triangle
            // C - hypotenuse

            double dLength_C = Math.Sqrt(Math.Pow(dLength_A, 2) + Math.Pow(dLength_B, 2));// length hypotenuse

             if(MathF.d_equal(dLength_A, 0.0) && MathF.d_equal(dLength_B, 0.0))
                return 0.0f;

            if ((dLength_A >= 0.0) && (dLength_B >= 0.0))
            {
                // 1st Quadrant (0-90 degrees / resp. 0 - 0.5PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return Math.PI / 2.0;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return 0.0;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return Math.Atan((dLength_B) / (dLength_A)); // Atan vrati kladny uhol
                else
                    return Math.Acos(dLength_B / dLength_C);
            }
            else if ((dLength_A <= 0.0) && (dLength_B >= 0.0))
            {
                // 2nd Quadrant (90-180 degrees / resp. 0.5PI - PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return Math.PI / 2.0;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return Math.PI;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return Math.PI + Math.Atan((dLength_B) / (dLength_A)); // Atan vrati zaporny uhol
                else
                    return Math.PI + Math.Acos(dLength_B / dLength_C);
            }
            else if ((dLength_A <= 0.0) && (dLength_B <= 0.0))
            {
                // 3rd Quadrant (180-270 degrees / resp. PI - 1.5PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return 1.5 * Math.PI;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return Math.PI;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return Math.PI + Math.Atan((dLength_B) / (dLength_A)); // Atan vrati kladny uhol
                else
                    return Math.PI + Math.Acos(dLength_B / dLength_C);
            }
            else /*((fdLength_A >= 0.0) && (dLength_B <= 0.0))*/
            {
                // 4th Quadrant (270-360 degrees / resp. 1.5PI - 2PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return 1.5 * Math.PI;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return 2.0 * Math.PI;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return (2.0 * Math.PI) + Math.Atan((dLength_B) / (dLength_A)); // Atan vrati zaporny uhol
                else
                    return (2.0 * Math.PI) + Math.Acos(dLength_B / dLength_C);
            }

            // Ukazka vypoctu uhlov
            /*

            A	    B	    B/A	      Gama rad	    Gama deg
            1	    1	    1	        0,785	    45
            -1	    1	    -1	        -0,785	    -45
            -1	    -1	    1       	0,785	    45
            1	    -1	    -1	        -0,785  	-45

            1	    0,5 	0,5     	0,464	    26,57
            -0,5    1       -2	        -1,107	    -63,43
            -1	    -0,5    0,5     	0,464	    26,57
            0,5	    -1	    -2	        -1,107  	-63,43
            */
        }

        public static double GetAlpha2D_CW_2(double dLength_A, double dLength_B)
        {
            // Clockwise rotation in 2D environment
            // Left-handed system
            // For rotation about Y-axis

            ///////////////////////////////////////////////////////////////////
            // len rozpracovane , nutne skontrolovat znamienka a vylepsit 
            ///////////////////////////////////////////////////////////////////

            // A - adjacent side of triangle
            // B - opposite side of triangle
            // C - hypotenuse

            double dLength_C = Math.Sqrt(Math.Pow(dLength_A, 2) + Math.Pow(dLength_B, 2));// length hypotenuse

            if (MathF.d_equal(dLength_A, 0.0) && MathF.d_equal(dLength_B, 0.0))
                return 0.0f;

            if ((dLength_A >= 0.0) && (dLength_B <= 0.0))
            {
                // 1st Quadrant (0-90 degrees / resp. 0 - 0.5PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return Math.PI / 2.0;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return 0.0;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return (- Math.Atan((dLength_B) / (dLength_A))); // Atan vrati zaporny uhol
                else
                    return (- Math.Acos(dLength_B / dLength_C));
            }
            else if ((dLength_A <= 0.0) && (dLength_B <= 0.0))
            {
                // 2nd Quadrant (90-180 degrees / resp. 0.5PI - PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return Math.PI / 2.0;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return Math.PI;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return Math.PI - Math.Atan((dLength_B) / (dLength_A)); // Atan vrati kladny uhol
                else
                    return Math.PI - Math.Acos(dLength_B / dLength_C);
            }
            else if ((dLength_A <= 0.0) && (dLength_B >= 0.0))
            {
                // 3rd Quadrant (180-270 degrees / resp. PI - 1.5PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return 1.5 * Math.PI;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return Math.PI;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return Math.PI - Math.Atan((dLength_B) / (dLength_A)); // Atan vrati zaporny uhol
                else
                    return Math.PI - Math.Acos(dLength_B / dLength_C);
            }
            else /*((fdLength_A >= 0.0) && (dLength_B >= 0.0))*/
            {
                // 4th Quadrant (270-360 degrees / resp. 1.5PI - 2PI)
                if (MathF.d_equal(dLength_A, 0.0))
                    return 1.5 * Math.PI;
                else if (MathF.d_equal(dLength_B, 0.0))
                    return 2.0 * Math.PI;
                else if (!MathF.d_equal(dLength_A, 0.0))
                    return (2.0 * Math.PI) - Math.Atan((dLength_B) / (dLength_A)); // Atan vrati kladny uhol
                else
                    return (2.0 * Math.PI) - Math.Acos(dLength_B / dLength_C);
            }
        }

        public static double GetAlpha2D_CW_3(double dLength_A, double dLength_B, double dLength_C)
        {
            // Clockwise rotation in 2D environment
            // Left-handed system
            // For rotation about Y-axis - graphics

            ///////////////////////////////////////////////////////////////////
            // len rozpracovane , nutne skontrolovat znamienka a vylepsit 
            ///////////////////////////////////////////////////////////////////

            // A - x axis
            // B - z axis           - opposite side of triangle
            // C - length of member - hypotenuse

            if (MathF.d_equal(dLength_A, 0.0) && MathF.d_equal(dLength_B, 0.0))
                return 0.0f;

            if ((dLength_A >= 0.0) && (dLength_B <= 0.0))
            {
                // 1st Quadrant (0-90 degrees / resp. 0 - 0.5PI)
                if (MathF.d_equal(dLength_B, 0.0))
                    return 0.0;
                else
                    return (-Math.Asin(dLength_B / dLength_C)); // Asin vrati zaporny uhol
            }
            else if ((dLength_A <= 0.0) && (dLength_B <= 0.0))
            {
                // 2nd Quadrant (90-180 degrees / resp. 0.5PI - PI)
                if (MathF.d_equal(dLength_B, 0.0))
                    return Math.PI;
                else
                    return Math.PI + Math.Asin(dLength_B / dLength_C); // Asin vrati zaporny uhol
            }
            else if ((dLength_A <= 0.0) && (dLength_B >= 0.0))
            {
                // 3rd Quadrant (180-270 degrees / resp. PI - 1.5PI)
                if (MathF.d_equal(dLength_B, 0.0))
                    return Math.PI;
                else
                    return Math.PI + Math.Asin(dLength_B / dLength_C); // Asin vrati kladny uhol
            }
            else /*((fdLength_A >= 0.0) && (dLength_B >= 0.0))*/
            {
                // 4th Quadrant (270-360 degrees / resp. 1.5PI - 2PI)
                if (MathF.d_equal(dLength_B, 0.0))
                    return 2.0 * Math.PI;
                else
                    return (2.0 * Math.PI) - Math.Asin(dLength_B / dLength_C);  // Asin vrati kladny uhol
            }
        }



        /// <summary>
        /// Converts an angle in decimal degress to radians.
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees to convert.</param>
        /// <returns>Angle in radians</returns>
        public static double DegreesToRadians(double angleInDegrees)
        {
            return angleInDegrees * (Math.PI / 180);
        }

        /// <summary>
        /// Converts an angle in radians tp decimal degress.
        /// </summary>
        /// <param name="angleInRadians">The angle in radians to convert.</param>
        /// <returns>Angle in radians</returns>
        public static double RadiansToDegrees(double angleInRadians)
        {
            return angleInRadians * 180 / Math.PI;            
        }

        /// <summary>
        /// Rotates a point around the origin
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        public static Point RotatePoint(Point pointToRotate, double angleInDegrees)
        {
            return RotatePoint(pointToRotate, new Point(0, 0), angleInDegrees);
        }        

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The center point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = DegreesToRadians(angleInDegrees);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        /// <summary>
        /// Test whether two line segments intersect. If so, calculate the intersection point.
        /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
        /// </summary>
        /// <param name="p">Vector to the start point of p.</param>
        /// <param name="p2">Vector to the end point of p.</param>
        /// <param name="q">Vector to the start point of q.</param>
        /// <param name="q2">Vector to the end point of q.</param>
        /// <param name="intersection">The point of intersection, if any.</param>
        /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
        /// </param>
        /// <returns>True if an intersection point was found.</returns>
        public static bool LineSegementsIntersect(Vector2D p, Vector2D p2, Vector2D q, Vector2D q2,
            out Vector2D intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new Vector2D();

            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }
    }
}
