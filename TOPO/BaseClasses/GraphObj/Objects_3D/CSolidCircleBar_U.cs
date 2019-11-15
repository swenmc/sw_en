﻿using MATH;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class CSolidCircleBar_U
    {
        public float m_fDiameter;
        DiffuseMaterial m_mat;

        //short iTotNoPoints = 13; // 1 auxialiary node in centroid / stredovy bod

        public CSolidCircleBar_U()
        { }

        public static Model3DGroup CreateM_3D_G_Volume_U_Bar(Point3D solidControlEdge, short nPoints, float fDiameter, DiffuseMaterial mat)
        {
            Model3DGroup models = new Model3DGroup();

            float cylinderVerticalLeft_Length = 0.075f;
            float cylinderHorizontal_Length = 0.6f;
            float cylinderVerticalRight_Length = 0.075f;
            float arcRadius = 0.03f;

            // Kreslime v rovine XZ
            Point3D cylinderVerticalLeft_ControlPoint = new Point3D(0, 0, -cylinderVerticalLeft_Length);
            Point3D arcLeft_ControlPoint = new Point3D(arcRadius, 0, cylinderVerticalLeft_ControlPoint.Y - arcRadius);
            Point3D cylinderHorizontal_ControlPoint = new Point3D(0, 0, -cylinderVerticalLeft_Length - arcRadius);
            Point3D arcRight_ControlPoint = new Point3D(cylinderHorizontal_ControlPoint.X + cylinderHorizontal_Length + arcRadius, 0, cylinderHorizontal_ControlPoint.Y + arcRadius);
            Point3D cylinderVerticalRight_ControlPoint = new Point3D(arcRight_ControlPoint.X + arcRadius, 0, arcRight_ControlPoint.Y);

            GeometryModel3D cylinderVerticalLeft = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalLeft_ControlPoint, 13, 0.5f * fDiameter, cylinderVerticalLeft_Length, mat, 2);
            Model3DGroup leftArc = GetTorus3DGroup(arcRadius, 0.5f * fDiameter, 0, Math.PI / 3, Colors.Red /*mat.Color*/, 1, arcLeft_ControlPoint /*new Point3D(0, 0, 0)*/);
            GeometryModel3D cylinderHorizontal = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderHorizontal_ControlPoint, 13, 0.5f * fDiameter, cylinderHorizontal_Length, mat,0, false, false);
            Model3DGroup rightArc = GetTorus3DGroup(arcRadius, 0.5f * fDiameter,0, Math.PI / 4, mat.Color, 1, arcRight_ControlPoint /*new Point3D(0, 0, 0)*/);
            GeometryModel3D cylinderVerticalRight = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalRight_ControlPoint, 13, 0.5f * fDiameter, cylinderVerticalRight_Length, mat, 2);

            // Add particular segments to the group
            models.Children.Add((Model3D)cylinderVerticalLeft);
            models.Children.Add((Model3D)leftArc);
            models.Children.Add((Model3D)cylinderHorizontal);
            models.Children.Add((Model3D)rightArc);
            models.Children.Add((Model3D)cylinderVerticalRight);

            return models;
        }

        // Refaktorovat s CurvedLineArrow3D

        public static Model3DGroup GetTorus3DGroup(float fLineRadius, float fRadius, double fAngle_min_rad, double fAngle_max_rad, Color SurfaceColor, float fOpacity, Point3D pCenter)
        {
            ParametricSurface ps = new ParametricSurface(fLineRadius, fRadius, SurfaceColor, fOpacity, pCenter);

            ps.Umin = fAngle_min_rad;
            ps.Umax = fAngle_max_rad; // hlavny uhol
            ps.Vmin = 0;
            ps.Vmax = 2 * Math.PI;
            ps.Nu = 24; // delenie
            ps.Nv = 12;
            ps.CreateSurface();

            return ps.Group3D;
        }
    }
}
