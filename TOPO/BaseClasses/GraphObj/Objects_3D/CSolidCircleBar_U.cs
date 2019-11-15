using MATH;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class CSolidCircleBar_U
    {
        public bool bBarIsInXDirection;
        public float m_fDiameter;
        public float arcRadius;
        public float arcRadiusNet;

        public Point3D m_pControlPoint;

        DiffuseMaterial mat;
        public bool m_bIsTop_U;

        //short iTotNoPoints = 13; // 1 auxialiary node in centroid / stredovy bod

        public CSolidCircleBar_U(bool bBarIsInXDirection_temp, Point3D pControlEdgePoint, float fDiameter, float farcRadiusNet, bool bIsTop_U_temp, DiffuseMaterial mat_temp)
        {
            bBarIsInXDirection = bBarIsInXDirection_temp;
            m_fDiameter = fDiameter;
            arcRadiusNet = farcRadiusNet;
            arcRadius = farcRadiusNet + 0.5f * fDiameter;
            m_pControlPoint = pControlEdgePoint;
            m_bIsTop_U = bIsTop_U_temp;

            mat = mat_temp;
        }

        public Model3DGroup CreateM_3D_G_Volume_U_Bar(Point3D solidControlEdge, short nPoints)
        {
            Model3DGroup models = new Model3DGroup();

            float cylinderVerticalLeft_Length = 0.075f;
            float cylinderHorizontal_Length = 0.6f;
            float cylinderVerticalRight_Length = 0.075f;
            float arcRadius = 0.03f;

            // Kreslime v rovine XZ
            Point3D cylinderVerticalLeft_ControlPoint = new Point3D(0, 0, -cylinderVerticalLeft_Length);
            Point3D arcLeft_ControlPoint = new Point3D(arcRadius, 0, cylinderVerticalLeft_ControlPoint.Z);
            Point3D cylinderHorizontal_ControlPoint = new Point3D(arcRadius, 0, -cylinderVerticalLeft_Length - arcRadius);
            Point3D arcRight_ControlPoint = new Point3D(cylinderHorizontal_ControlPoint.X + cylinderHorizontal_Length, 0, cylinderHorizontal_ControlPoint.Z + arcRadius);
            Point3D cylinderVerticalRight_ControlPoint = new Point3D(arcRight_ControlPoint.X + arcRadius, 0, arcRight_ControlPoint.Z);

            GeometryModel3D cylinderVerticalLeft = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalLeft_ControlPoint, 13, 0.5f * m_fDiameter, cylinderVerticalLeft_Length, mat, 2, true, false);
            GeometryModel3D leftArc = GetTorusGeometryModel3D(arcRadius, 0.5f * m_fDiameter, Math.PI, 1.5 * Math.PI, mat, arcLeft_ControlPoint);
            GeometryModel3D cylinderHorizontal = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderHorizontal_ControlPoint, 13, 0.5f * m_fDiameter, cylinderHorizontal_Length, mat,0, false, false);
            GeometryModel3D rightArc = GetTorusGeometryModel3D(arcRadius, 0.5f * m_fDiameter, 1.5 * Math.PI, 2 * Math.PI, mat, arcRight_ControlPoint);
            GeometryModel3D cylinderVerticalRight = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalRight_ControlPoint, 13, 0.5f * m_fDiameter, cylinderVerticalRight_Length, mat, 2, true, false);

            // Add particular segments to the group
            models.Children.Add((Model3D)cylinderVerticalLeft);
            models.Children.Add((Model3D)leftArc);
            models.Children.Add((Model3D)cylinderHorizontal);
            models.Children.Add((Model3D)rightArc);
            models.Children.Add((Model3D)cylinderVerticalRight);

            return models;
        }

        // Refaktorovat s CurvedLineArrow3D

        public static GeometryModel3D GetTorusGeometryModel3D(float fLineRadius, float fRadius, double fAngle_min_rad, double fAngle_max_rad, DiffuseMaterial mat, Point3D pCenter)
        {
            // Torus sa defaultne kresli do roviny XZ

            ParametricSurface ps = new ParametricSurface(fLineRadius, fRadius, pCenter);

            ps.Umin = fAngle_min_rad;
            ps.Umax = fAngle_max_rad; // hlavny uhol
            ps.Vmin = 0;
            ps.Vmax = 2 * Math.PI;
            ps.Nu = 24; // delenie
            ps.Nv = 12;
            ps.CreateSurface(mat);

            return ps.GeometryModel3D;
        }

        public void GetDefinitionLengths(
            float diameterOfBarInYdirection,
            CFoundation pad,
            out float cylinderVerticalLeft_Length,
            out float cylinderHorizontal_Length,
            out float cylinderVerticalRight_Length
            )
        {
            // IN WORK

            cylinderVerticalLeft_Length =  pad.m_fDim3 - pad.ConcreteCover - 0.5f * m_fDiameter - arcRadius;

            if(bBarIsInXDirection)
                cylinderVerticalLeft_Length = pad.m_fDim3 - pad.ConcreteCover - diameterOfBarInYdirection - 0.5f * m_fDiameter - arcRadius;
            /*
            Point leftArcStart = new Point(start.X, start.Y - (pad.m_fDim3 - pad.ConcreteCover - pad.ConcreteCover - 0.5f * fBarDiameter - fArcRadius));
            Point leftArcEnd = new Point(start.X + fArcRadius, leftArcStart.Y - fArcRadius);
            Point rightArcStart = new Point(leftArcEnd.X + fHorizontalStraightPartLength, leftArcEnd.Y);
            Point rightArcEnd = new Point(rightArcStart.X + fArcRadius, rightArcStart.Y + fArcRadius);
            Point end = new Point(pad.m_fDim2 - pad.ConcreteCover - 0.5 * fBarDiameter, start.Y);
            */

            cylinderVerticalLeft_Length = 0.075f;
            cylinderHorizontal_Length = 0.6f;
            cylinderVerticalRight_Length = 0.075f;
        }
    }
}
