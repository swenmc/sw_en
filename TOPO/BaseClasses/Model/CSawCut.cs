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
    public class CSawCut : CEntity3D
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

        private Point3D m_PointText;

        public Point3D PointText
        {
            get { return m_PointText; }
            set { m_PointText = value; }
        }

        private float m_fLength;

        public float Length
        {
            get { return m_fLength; }
            set { m_fLength = value; }
        }

        private string m_Text;

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        private float m_CutWidth;

        public float CutWidth
        {
            get { return m_CutWidth; }
            set { m_CutWidth = value; }
        }

        private float m_CutDepth;

        public float CutDepth
        {
            get { return m_CutDepth; }
            set { m_CutDepth = value; }
        }

        // TO Ondrej - ak maju podla teba tie premenne zmysel tak z nich treba urobit properties, mozno by sa dalo riesit priamo v tomto objekte aky je smer textu
        bool bTextAboveLine; // true - text je medzi nad liniou (alebo nalavo od nej), false - text je na opacnej strane (pod liniou) alebo napravo od nej
        public int iVectorOverFactor_LCS;
        public int iVectorUpFactor_LCS;

        public Transform3DGroup TransformGr;

        public CSawCut(int id, Point3D start, Point3D end, float cutWidth, float cutDepth,  bool bIsDiplayed_temp, int fTime)
        {
            ID = id;
            m_PointStart = start;
            m_PointEnd = end;
            m_CutWidth = cutWidth;
            m_CutDepth = cutDepth;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;

            m_Text = "SAW CUT";

            Length = (float)Math.Sqrt((float)Math.Pow(m_PointEnd.X - m_PointStart.X, 2f) + (float)Math.Pow(m_PointEnd.Y - m_PointStart.Y, 2f) + (float)Math.Pow(m_PointEnd.Z - m_PointStart.Z, 2f));

            SetTextPointInLCS(); // Text v LCS
        }

        // TODO Ondrej - Refaktorovat
        public void SetTextPointInLCS()
        {
            bTextAboveLine = false;

            iVectorOverFactor_LCS = -1;
            iVectorUpFactor_LCS = -1;

            float fOffsetFromLine;
            float fOffsetFromPlane = 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

            if (bTextAboveLine)
                fOffsetFromLine = 0.1f; // Mezera medzi ciarou a textom (kladna - text nad ciarou (+y), zaporna, text pod ciarou (-y))
            else
                fOffsetFromLine = -0.1f;

            m_PointText = new Point3D()
            {
                X = 0.3 * m_fLength, // Kreslime v 30% dlzky od zaciatku
                Y = fOffsetFromLine,
                Z = fOffsetFromPlane
            };
        }

        public GeometryModel3D GetSawCutModel(System.Windows.Media.Color color)
        {
            GeometryModel3D model = new GeometryModel3D();

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color)); // TODO Ondrej - urobit nastavitelnu hrubku a farbu

            float fLineThickness = 0.002f; // hrubka = priemer pre export do 2D (2 x polomer valca)
            float fLineCylinderRadius = 0.005f; //0.005f * fLength; // Nastavovat ! polomer valca, co najmensi ale viditelny - 3D

            // LCS - line in x-direction
            model = CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0,0,0), 13, fLineCylinderRadius, m_fLength, material, 0);

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

            // To Ondrej - toto treba nejako normalnejsie poskladat a refaktorovat s CDimensionLineLinear3D a CControlJoint
            if(dGammaZ > 0) // Otacame proti smeru hodinovych ruciciek - text chceme mat citatelny horizontalne takze musime prehodit vektor
            {
                bTextAboveLine = true;

                iVectorOverFactor_LCS = 1;
                iVectorUpFactor_LCS = 1;

                float fOffsetFromLine = 0.1f;
                float fOffsetFromPlane = 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

                m_PointText = new Point3D()
                {
                    X = 0.2 * m_fLength, // Kreslime v 20% dlzky od zaciatku
                    Y = fOffsetFromLine,
                    Z = fOffsetFromPlane
                };
            }

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
