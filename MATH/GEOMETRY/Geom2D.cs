using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    public static class Geom2D
    {
        public static float[,] m_ArrfPointsCoord2D;

        // Transformation of coordinates
        // Polar to Carthesian, Input angle in degrees
        public static float GetPositionX(float radius, float theta)
        {
            return radius * (float)Math.Cos(theta * Math.PI / 180);
        }

        public static float GetPositionY_CW(float radius, float theta)
        {
            // Clock-wise (for counterclock-wise change sign for vertical coordinate)
            return -radius * (float)Math.Sin(theta * Math.PI / 180);
        }

        public static float GetPositionY_CCW(float radius, float theta)
        {
            // Counter Clock-wise
            return - GetPositionY_CW(radius, theta);
        }

        // Get rotated position of point
        public static float GetRotatedPosition_x_CCW(float x, float y, double theta)
        {
            return (float)(x * Math.Cos(theta) - y * Math.Sin(theta));
        }

        public static float GetRotatedPosition_y_CCW(float x, float y, double theta)
        {
            return (float)(x * Math.Sin(theta) + y * Math.Cos(theta));
        }

        public static float GetRotatedPosition_x_CW(float x, float y, double theta)
        {
            return (float)(x * Math.Cos(theta) + y * Math.Sin(theta));
        }

        public static float GetRotatedPosition_y_CW(float x, float y, double theta)
        {
            return (float)(x * - Math.Sin(theta) + y * Math.Cos(theta));
        }

        public static void TransformPositions_CCW(float x, float y, float x_centerOfRotation, float y_centerOfRotation, double theta)
        {
            float px;
            float py;

            if (!MathF.d_equal(theta, 0)) // Translate and rotate
            {
                px = (float)(Math.Cos(theta) * (x - x_centerOfRotation) - Math.Sin(theta) * (y - y_centerOfRotation) + x_centerOfRotation);
                py = (float)(Math.Sin(theta) * (x - x_centerOfRotation) + Math.Cos(theta) * (y - y_centerOfRotation) + y_centerOfRotation);
            }
            else // Only translate
            {
                px = x + x_centerOfRotation;
                py = y + y_centerOfRotation;
            }

            x = px;
            y = py;
        }

        public static void TransformPositions_CW(float x, float y, float x_centerOfRotation, float y_centerOfRotation, double theta)
        {
            float px;
            float py;

            if (!MathF.d_equal(theta, 0)) // Translate and rotate
            {
                px = (float)(Math.Cos(theta) * (x - x_centerOfRotation) + Math.Sin(theta) * (y - y_centerOfRotation) + x_centerOfRotation);
                py = (float)(-Math.Sin(theta) * (x - x_centerOfRotation) + Math.Cos(theta) * (y - y_centerOfRotation) + y_centerOfRotation);
            }
            else  // Only translate
            {
                px = x + x_centerOfRotation;
                py = y + y_centerOfRotation;
            }

            x = px;
            y = py;
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
        public static float[,] GetCirclePointCoord(float fr, int iNumber)
        {
            m_ArrfPointsCoord2D = new float[iNumber, 2];

            for (int i = 0; i < iNumber; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX(fr, i * 360 / iNumber);  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CW(fr, i * 360 / iNumber);  // z
            }

            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Arc
        // Arc
        public static float[,] GetArcPointCoord(float fr, int fStartAngle, int fEndAngle, int iNumber)
        {
            // iNumber - number of points of section (arc + centroid)
            // iNumber - 1 - number of points arc
            m_ArrfPointsCoord2D = new float[iNumber, 2];  // Allocate Memory for whole section (all section points including centroid)

            // Decrease Number
            --iNumber;
            // iNumber - number of points of arc
            // iNumber - 1 - number of segments of arc

            for (int i = 0; i < iNumber; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX(fr, fStartAngle + i * (fEndAngle - fStartAngle) / (iNumber - 1));  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CW(fr, fStartAngle + i * (fEndAngle - fStartAngle) / (iNumber - 1));  // z
            }

            return m_ArrfPointsCoord2D;
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
        public static float[,] GetEllipsePoints(float fx, float fy, float fa, float fb, float fAngle, short isteps)
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
        public static float[,] GetEllipsePointCoord(float fa, float fb, float fAngle, short isteps)
        {
            //if (isteps == null)
            //    isteps = 36;

            m_ArrfPointsCoord2D = GetEllipsePoints(0.0f, 0.0f, fa, fb, fAngle, isteps);
            return m_ArrfPointsCoord2D;
        }
        public static float[,] GetEllipsePointCoord(float fa, float fb, float fAngle)
        {
            m_ArrfPointsCoord2D = GetEllipsePointCoord(fa, fb, fAngle, 72);
            return m_ArrfPointsCoord2D;
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
        public static float[,] GetPolygonPointCoord(float fa, short iNumEdges)
        {
            m_ArrfPointsCoord2D = new float[iNumEdges, 2];

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            for (int i = 0; i < iNumEdges; i++)
            {
                m_ArrfPointsCoord2D[i, 0] = GetPositionX(GetRadiusfromSideLength(fa, iNumEdges), i * 360 / iNumEdges);  // y
                m_ArrfPointsCoord2D[i, 1] = GetPositionY_CW(GetRadiusfromSideLength(fa, iNumEdges), i * 360 / iNumEdges);  // z
            }

            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Triangle
        // Triangle
        public static float[,] GetTrianEqLatPointCoord1(float fa)
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
        public static float[,] GetTrianEqLatPointCoord2(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 3);
            return m_ArrfPointsCoord2D;
        }
        // Isosceles
        public static float[,] GetTrianIsosCelPointCoord(float fh, float fb)
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
        public static float[,] GetTrianRightAngPointCoord(float fh, float fb)
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
        #endregion
        #region Square
        // Square (4)
        public static float[,] GetSquarePointCoord1(float fa)
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
        public static float[,] GetSquarePointCoord2(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 4);
            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Rectangle
        // Rectangle (4)
        public static float[,] GetRectanglePointCoord(float fh, float fb)
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
        #endregion
        #region Pentagon
        // Pentagon (5)
        public static float[,] GetPentagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 5);
            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Hexagon
        // Hexafgon (6)
        public static float[,] GetHexagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 6);
            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Heptagon
        // Heptagon (7)
        public static float[,] GetHeptagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 7);
            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Octagon
        // Octagon  (8)
        public static float[,] GetOctagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 8);
            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Nonagon
        // Nonagon  (9)
        public static float[,] GetNonagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 9);
            return m_ArrfPointsCoord2D;
        }
        #endregion
        #region Decagon
        // Decagon  (10)
        public static float[,] GetDecagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 10);
            return m_ArrfPointsCoord2D;
        }
        #endregion        
        #region Hendecagon
        // Hendecagon(11)
        public static float[,] GetHendecagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 11);
            return m_ArrfPointsCoord2D;
        }
        #endregion        
        #region Dodecagon
        // Dodecagon (12)
        public static float[,] GetDodecagonPointCoord(float fa)
        {
            m_ArrfPointsCoord2D = GetPolygonPointCoord(fa, 12);
            return m_ArrfPointsCoord2D;
        }
        #endregion

        #region AddCentroidPosition_Zero
        public static float[,] AddCentroidPosition_Zero(float[,] fArrayEdgePoints)
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
        #endregion

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
    }
}
