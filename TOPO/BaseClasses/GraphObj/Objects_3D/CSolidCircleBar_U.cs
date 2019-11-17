using MATH;
using System;
using System.Collections.Generic;
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

        public float cylinderVerticalLeft_Length;
        public float cylinderHorizontal_Length;
        public float cylinderVerticalRight_Length;

        List<Point3D> arcLeft_wireFramePoints = new List<Point3D>(); // TODO Ondrej - toto by trebalo nejako rozumne usporiadat, teraz taham body nekonzistentne pre priame casti a pre obluciky
        List<Point3D> arcRight_wireFramePoints = new List<Point3D>(); // TODO Ondrej - toto by trebalo nejako rozumne usporiadat, teraz taham body nekonzistentne pre priame casti a pre obluciky

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

            /*
            float cylinderVerticalLeft_Length = 0.075f;
            float cylinderHorizontal_Length = 0.6f;
            float cylinderVerticalRight_Length = 0.075f;
            float arcRadius = 0.03f;
            */

            // Kreslime v rovine XZ
            Point3D cylinderVerticalLeft_ControlPoint = new Point3D(0, 0, -cylinderVerticalLeft_Length);
            Point3D arcLeft_ControlPoint = new Point3D(arcRadius, 0, cylinderVerticalLeft_ControlPoint.Z);
            Point3D cylinderHorizontal_ControlPoint = new Point3D(arcRadius, 0, -cylinderVerticalLeft_Length - arcRadius);
            Point3D arcRight_ControlPoint = new Point3D(cylinderHorizontal_ControlPoint.X + cylinderHorizontal_Length, 0, cylinderHorizontal_ControlPoint.Z + arcRadius);
            Point3D cylinderVerticalRight_ControlPoint = new Point3D(arcRight_ControlPoint.X + arcRadius, 0, arcRight_ControlPoint.Z);

            GeometryModel3D cylinderVerticalLeft = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalLeft_ControlPoint, 13, 0.5f * m_fDiameter, cylinderVerticalLeft_Length, mat, 2, true, false);
            GeometryModel3D leftArc = GetTorusGeometryModel3D(arcRadius, 0.5f * m_fDiameter, Math.PI, 1.5 * Math.PI, mat, arcLeft_ControlPoint, out arcLeft_wireFramePoints);
            GeometryModel3D cylinderHorizontal = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderHorizontal_ControlPoint, 13, 0.5f * m_fDiameter, cylinderHorizontal_Length, mat,0, false, false);
            GeometryModel3D rightArc = GetTorusGeometryModel3D(arcRadius, 0.5f * m_fDiameter, 1.5 * Math.PI, 2 * Math.PI, mat, arcRight_ControlPoint, out arcRight_wireFramePoints);
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

        public static GeometryModel3D GetTorusGeometryModel3D(float fLineRadius, float fRadius, double fAngle_min_rad, double fAngle_max_rad, DiffuseMaterial mat, Point3D pCenter, out List<Point3D> wireFramePoints)
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

            wireFramePoints = ps.WireframePoints;

            return ps.GeometryModel3D;
        }

        public void SetSegmentLengths(float diameterOfBarInYdirectionTop, float diameterOfBarInYdirectionBottom, CFoundation pad)
        {
            cylinderVerticalLeft_Length =  pad.m_fDim3 - 2 * pad.ConcreteCover - 0.5f * m_fDiameter - arcRadius;

            if(bBarIsInXDirection)
                cylinderVerticalLeft_Length = pad.m_fDim3 - 2 * pad.ConcreteCover - diameterOfBarInYdirectionTop - diameterOfBarInYdirectionBottom - 0.5f * m_fDiameter - arcRadius;

            if (bBarIsInXDirection)
                cylinderHorizontal_Length = pad.m_fDim1 - 2 * (pad.ConcreteCover + 0.5f * m_fDiameter + arcRadius);
            else
                cylinderHorizontal_Length = pad.m_fDim2 - 2 * (pad.ConcreteCover + 0.5f * m_fDiameter + arcRadius);

            cylinderVerticalRight_Length = pad.m_fDim3 - 2 * pad.ConcreteCover - 0.5f * m_fDiameter - arcRadius;

            if (bBarIsInXDirection)
                cylinderVerticalRight_Length = pad.m_fDim3 - 2 * pad.ConcreteCover - diameterOfBarInYdirectionTop - diameterOfBarInYdirectionBottom - 0.5f * m_fDiameter - arcRadius;
        }

        public List<Point3D> GetWireFramePoints_Volume(Model3DGroup volumeModel)
        {
            List<Point3D> models = new List<Point3D>();

            // TODO Ondrej - pripravit wireframe model pre cely tvar U (zlucit wireframe poinst z 5 objektov)
            // Urobil som to zatial takto ale nepaci sa mi to, lebo je to dost divoke a nejednotne pre priame casti a obluciky

            models.AddRange(CVolume.GetWireFramePoints_Volume((GeometryModel3D)volumeModel.Children[0], true));
            models.AddRange(arcLeft_wireFramePoints);
            models.AddRange(CVolume.GetWireFramePoints_Volume((GeometryModel3D)volumeModel.Children[2], true));
            models.AddRange(arcRight_wireFramePoints);
            models.AddRange(CVolume.GetWireFramePoints_Volume((GeometryModel3D)volumeModel.Children[4], true));

            return models;
        }
    }
}
