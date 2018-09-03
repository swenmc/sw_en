using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MATH
{
    public static class Geom3D
    {
        public static void TransformPositionsAboutZ_CW_deg(Point3D centerOfRotation, double theta_deg, ref Point3D[] array)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    float fX = (float)array[i].X;
                    float fY = (float)array[i].Y;
                    Geom2D.TransformPositions_CW_deg((float)centerOfRotation.X, (float)centerOfRotation.Y, theta_deg / 180f * Math.PI, ref fX, ref fY);

                    // Set new coordinates
                    array[i].X = fX;
                    array[i].Y = fY;
                    //array[i].Z - same
                }
            }
        }

        public static void MirrorAboutY_ChangeXCoordinates(ref Point3D[] array)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                    array[i].X *= -1;
            }
        }

        public static void MirrorAboutX_ChangeYCoordinates(ref Point3D[] array)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                    array[i].Y *= -1;
            }
        }
    }
}
