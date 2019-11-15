using _3DTools;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class Utility
    {
        public static void CreateRectangleFace_old(
        Point3D p0, Point3D p1, Point3D p2, Point3D p3,
        Color surfaceColor, Viewport3D viewport)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = surfaceColor;
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D geometry =
            new GeometryModel3D(mesh, material);
            ModelVisual3D model = new ModelVisual3D();
            model.Content = geometry;
            viewport.Children.Add(model);
        }

        public static void CreateWireframe(Point3D p0, Point3D p1, Point3D p2, Point3D p3, List<Point3D> wireframePoints)
        {
            wireframePoints.Add(p0);
            wireframePoints.Add(p1);
            wireframePoints.Add(p1);
            wireframePoints.Add(p2);
            wireframePoints.Add(p2);
            wireframePoints.Add(p3);
            wireframePoints.Add(p3);
            wireframePoints.Add(p0);            
        }
        public static Point3D GetNormalize(Point3D pt,
        double xmin, double xmax,
        double ymin, double ymax,
        double zmin, double zmax)
        {
            pt.X = -1 + 2 * (pt.X - xmin) / (xmax - xmin);
            pt.Y = -1 + 2 * (pt.Y - ymin) / (ymax - ymin);
            pt.Z = -1 + 2 * (pt.Z - zmin) / (zmax - zmin);
            return pt;
        }


        public static void CreateRectangleFace(Point3D p0, Point3D p1, Point3D p2, Point3D p3, int index, List<Point3D> positions, List<int> triangleIndices)
        {
            positions.Add(p0);
            positions.Add(p1);
            positions.Add(p2);
            positions.Add(p3);
            triangleIndices.Add(index + 0);
            triangleIndices.Add(index + 1);
            triangleIndices.Add(index + 2);
            triangleIndices.Add(index + 2);
            triangleIndices.Add(index + 3);
            triangleIndices.Add(index + 0);
        }

        //    public static void CreateRectangleFaceMartin(
        //Point3D p0, Point3D p1, Point3D p2, Point3D p3,
        //Color surfaceColor, float fOpacity, Model3DGroup gr)
        //    {
        //        // TODO - Ondrej performance - tu pre kazdy stvorcek robim novy GeometryModel3D
        //        // a pridavam to do model group, asi by bolo lepsie mat to vsetko v jednom GeometryModel3D, pochybujem ze by sme potrebovali kazdemu stvorceku definovat iny material
        //        MeshGeometry3D mesh = new MeshGeometry3D();
        //        mesh.Positions.Add(p0);
        //        mesh.Positions.Add(p1);
        //        mesh.Positions.Add(p2);
        //        mesh.Positions.Add(p3);
        //        mesh.TriangleIndices.Add(0);
        //        mesh.TriangleIndices.Add(1);
        //        mesh.TriangleIndices.Add(2);
        //        mesh.TriangleIndices.Add(2);
        //        mesh.TriangleIndices.Add(3);
        //        mesh.TriangleIndices.Add(0);
        //        SolidColorBrush brush = new SolidColorBrush();
        //        brush.Color = surfaceColor;
        //        brush.Opacity = fOpacity;
        //        Material material = new DiffuseMaterial(brush);
        //        GeometryModel3D geometry = new GeometryModel3D(mesh, material);
        //        gr.Children.Add(geometry);
        //    }
    }
}
