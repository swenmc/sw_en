﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CControlJoint : CEntity3D
    {
        private Point3D m_PointStart;

        public Point3D PointStart
        {
            get { return m_PointStart; }
            set { m_PointStart = value; }
        }
        private Point3D m_PointEnd;

        public Point3D PointEnd
        {
            get { return m_PointEnd; }
            set { m_PointEnd = value; }
        }

        private float m_fLength;

        public float Length
        {
            get { return m_fLength; }
            set { m_fLength = value; }
        }

        public Transform3DGroup TransformGr;

        public CControlJoint(int id, Point3D start, Point3D end, bool bIsDiplayed_temp, int fTime)
        {
            ID = id;
            PointStart = start;
            PointEnd = end;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;

            Length = (float)Math.Sqrt((float)Math.Pow(m_PointEnd.X - m_PointStart.X, 2f) + (float)Math.Pow(m_PointEnd.Y - m_PointStart.Y, 2f) + (float)Math.Pow(m_PointEnd.Z - m_PointStart.Z, 2f));
        }

        public GeometryModel3D GetControlJointModel(System.Windows.Media.Color color)
        {
            GeometryModel3D model = new GeometryModel3D();

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color)); // TODO Ondrej - urobit nastavitelnu hrubku a farbu

            float fLineThickness = 0.002f; // hrubka = priemer pre export do 2D (2 x polomer valca)
            float fLineCylinderRadius = 0.005f; //0.005f * fLength; // Nastavovat ! polomer valca, co najmensi ale viditelny - 3D

            // LCS - line in x-direction
            model = CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), 13, fLineCylinderRadius, m_fLength, material, 0);

            // Spocitame priemety
            double dDeltaX = m_PointEnd.X - m_PointStart.X;
            double dDeltaY = m_PointEnd.Y - m_PointStart.Y;
            double dDeltaZ = m_PointEnd.Z - m_PointStart.Z;

            // Returns transformed coordinates of member nodes
            // Angles
            double dAlphaX = 0, dBetaY = 0, dGammaZ = 0;

            // Uhly pootocenia LCS okolo osi GCS
            // Angles
            dAlphaX = Geom2D.GetAlpha2D_CW(dDeltaY, dDeltaZ);
            dBetaY = Geom2D.GetAlpha2D_CW_2(dDeltaX, dDeltaZ); // !!! Pre pootocenie okolo Y su pouzite ine kvadranty !!!
            dGammaZ = Geom2D.GetAlpha2D_CW(dDeltaX, dDeltaY);

            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            // About Z - plane XY
            AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
            axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
            axisAngleRotation3dZ.Angle = Geom2D.RadiansToDegrees(dGammaZ);
            rotateZ.Rotation = axisAngleRotation3dZ;

            TranslateTransform3D translateOrigin = new TranslateTransform3D(m_PointStart.X, m_PointStart.Y, m_PointStart.Z);

            TransformGr = new Transform3DGroup();
            TransformGr.Children.Add(rotateX);
            TransformGr.Children.Add(rotateY);
            TransformGr.Children.Add(rotateZ);
            TransformGr.Children.Add(translateOrigin); // Presun celej koty v ramci GCS

            model.Transform = TransformGr;

            return model;
        }
    }
}
