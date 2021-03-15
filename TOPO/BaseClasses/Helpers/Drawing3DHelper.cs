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
    public static class Drawing3DHelper
    {
        //public static void TransformPoints(List<Point3D> points, Transform3DGroup transform)
        //{            
        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        points[i] = transform.Transform(points[i]);
        //    }
        //}
        public static void TransformPoints(List<Point3D> points, Transform3D transform)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = transform.Transform(points[i]);
            }
        }


    }
}
