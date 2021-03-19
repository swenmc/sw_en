﻿using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    [Serializable]
    public class CReinforcementBar : CVolume
    {
        private Point3D m_StartPoint;
        private Point3D m_EndPoint;
        private float m_diameter;
        private float m_fArea_As_1;
        private float m_projectionLength;
        private float m_totalLength;

        private bool m_BarIsInXDirection; // TODO - toto urobit nejako krajsie (pouzit napriklad nejaky enum X,Y,Z)

        private float m_fOpacity;

        private Color m_Color;

        public float Diameter
        {
            get
            {
                return m_diameter;
            }

            set
            {
                m_diameter = value;
            }
        }

        public float Area_As_1
        {
            get
            {
                return m_fArea_As_1;
            }

            set
            {
                m_fArea_As_1 = value;
            }
        }

        public float ProjectionLength
        {
            get
            {
                return m_projectionLength;
            }

            set
            {
                m_projectionLength = value;
            }
        }

        public float TotalLength
        {
            get
            {
                return m_totalLength;
            }

            set
            {
                m_totalLength = value;
            }
        }

        public Point3D StartPoint
        {
            get
            {
                return m_StartPoint;
            }

            set
            {
                m_StartPoint = value;
            }
        }

        public Point3D EndPoint
        {
            get
            {
                return m_EndPoint;
            }

            set
            {
                m_EndPoint = value;
            }
        }

        public bool BarIsInXDirection // TODO - toto urobit nejako krajsie (pouzit napriklad nejaky enum X,Y,Z)
        {
            get
            {
                return m_BarIsInXDirection;
            }

            set
            {
                m_BarIsInXDirection = value;
            }
        }

        public float Opacity
        {
            get
            {
                return m_fOpacity;
            }

            set
            {
                m_fOpacity = value;
            }
        }

        public Color ColorBar
        {
            get
            {
                return m_Color;
            }

            set
            {
                m_Color = value;
            }
        }

        [NonSerialized]
        public Model3DGroup Visual_Object;

        public void SetMaterialPropertiesFromDatabase(string matName)
        {
            if (string.IsNullOrEmpty(matName)) return; 
            // Vytvorim material typu steel
            MATERIAL.CMat_03_00 mat = new MATERIAL.CMat_03_00();

            // Nastavim vlastnosti
            mat.Name = matName;
            CMatPropertiesRF prop = CMaterialManager.LoadMaterialPropertiesRF(matName);
            mat.m_fE = (float)prop.E;
            mat.m_fG = (float)prop.G;
            mat.m_fNu = (float)prop.Nu;
            mat.m_fAlpha_T = (float)prop.Alpha;
            mat.m_fRho = (float)prop.Rho;
            mat.m_ft_interval = new float[1] { 0.1f };
            mat.m_ff_yk = new float[1] { (float)prop.Ry };

            // Nastavim vytvoreny material typu steel do vlastnosti objektu
            m_Mat = mat;
        }

        public /*override*/ Model3DGroup CreateModel3DGroup(float fOpacity, Transform3DGroup temp, CFoundation pad /* docasne*/)
        {
            SolidColorBrush brush = new SolidColorBrush(ColorBar);
            brush.Opacity = fOpacity;

            return CreateModel3DGroup(brush, temp, pad /* docasne*/);
        }

        public /*override*/ Model3DGroup CreateModel3DGroup(Color colorBrush, float fOpacity, Transform3DGroup temp, CFoundation pad /* docasne*/)
        {
            SolidColorBrush brush = new SolidColorBrush(colorBrush);
            brush.Opacity = fOpacity;
            Visual_Object = CreateModel3DGroup(brush, temp, pad /* docasne*/);
            return Visual_Object;
        }

        //tato metoda by mohla vracat GeometryModel3D
        public /*override*/ Model3DGroup CreateModel3DGroup(SolidColorBrush brush, Transform3DGroup temp, CFoundation pad /* docasne*/)
        {
            Model3DGroup modelGroup = new Model3DGroup();
            //GeometryModel3D model;
            //DiffuseMaterial mat = new DiffuseMaterial(brush);

            // Vytvorime model ktory smeruje v ose X
            if (this is CReinforcementBarStraight) // Priamy prut
            {
                GeometryModel3D model = new GeometryModel3D();
                model = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), 6 + 1, 0.5f * m_diameter, m_projectionLength, new DiffuseMaterial(brush), 0);
                modelGroup.Children.Add(model);

                SetWireFramePoints_Volume(model, true);
            }
            else
            {
                CReinforcementBar_U bar = (CReinforcementBar_U)this;
                CSolidCircleBar_U barModel = new CSolidCircleBar_U(BarIsInXDirection, ControlPoint, Diameter, bar.ArcRadiusNet, bar.IsTop_U, new DiffuseMaterial(brush));
                float diameterOfBarInYdirection_Top = 0.012f; // TODO - pre U bar v smere X sem potrebujeme dostat aj priemer vystuze v smere Y resp. upravit cover (smer X sa nachadza vertikalne blizsie k stredu patky)
                float diameterOfBarInYdirection_Bottom = 0.012f; // TODO - pre U bar v smere X sem potrebujeme dostat aj priemer vystuze v smere Y resp. upravit cover (smer X sa nachadza vertikalne blizsie k stredu patky)
                barModel.SetSegmentLengths(diameterOfBarInYdirection_Top, diameterOfBarInYdirection_Bottom, pad);
                TotalLength = bar.TotalLength = barModel.m_fTotalLength;
                modelGroup = barModel.CreateM_3D_G_Volume_U_Bar(new Point3D(0, 0, 0), 6 + 1);

                WireFramePoints = barModel.GetWireFramePoints_Volume(modelGroup);
            }

            // Transformacia vyztuze vratane transformacie zakladu v GCS
            Transform3DGroup myTransform3DGroup = GetTransformGroup(temp);
            
            // Set the Transform property of the modelGroup to the Transform3DGroup which includes 
            // both transformations. The 3D object now has two Transformations applied to it.
            modelGroup.Transform = myTransform3DGroup;
            //---------------------------------------------------------------------------

            Visual_Object = modelGroup;
            return modelGroup;
        }

        public Transform3DGroup GetTransformGroup(Transform3DGroup foundationTransGroup)
        {
            double dRotationAngle_deg = 0;

            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            if (this is CReinforcementBar_U && ((CReinforcementBar_U)this).IsTop_U) // Is U shape and upper reinforcement
            {
                // Model Rotation
                // Rotate about X-axis
                // Create and apply a transformation that rotates the object.
                RotateTransform3D myRotateTransform3D = new RotateTransform3D();
                AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
                myAxisAngleRotation3d.Axis = new Vector3D(1, 0, 0); // Rotate about X
                myAxisAngleRotation3d.Angle = 180;
                myRotateTransform3D.Rotation = myAxisAngleRotation3d;

                // Add the rotation transform to Transform3DGroup
                myTransform3DGroup.Children.Add(myRotateTransform3D);
            }

            if (!BarIsInXDirection) // Rotate from X to Y (90 deg about Z)
            {
                dRotationAngle_deg = 90; // Prut smeruje v ose y

                // Model Rotation
                // Rotate about Z-axis
                // Create and apply a transformation that rotates the object.
                RotateTransform3D myRotateTransform3D = new RotateTransform3D();
                AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
                myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 1); // Rotate about Z
                myAxisAngleRotation3d.Angle = dRotationAngle_deg;
                myRotateTransform3D.Rotation = myAxisAngleRotation3d;

                // Add the rotation transform to Transform3DGroup
                myTransform3DGroup.Children.Add(myRotateTransform3D);
            }

            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D);

            // Set the Transform property of the modelGroup to the Transform3DGroup which includes 
            // both transformations. The 3D object now has two Transformations applied to it.
            //model.Transform = myTransform3DGroup;

            //---------------------------------------------------------------------------
            // TO Ondrej - toto je tu docasne, transformujem kazdy prut samostatne do GCS tak ako je umiestneny zaklad
            // Treba to prerobit tak ze sa budu transformovat vsetky objekty vyztuze v hornej aj dolnej vrstve spolu, asi na konci metody
            // CreateReinforcementBars() v CFoundation
            // Je tu podobna vec ako bola pri plechoch a spojoch, musime si zapamatat body po trasnformaciach line 152-168 a na tie body potom urobit tuto transformaciu
            myTransform3DGroup.Children.Add(foundationTransGroup);

            return myTransform3DGroup;
        }

        public List<Point3D> GetWireFramePoints_Volume(Transform3D trans)
        {
            return TransformListOfPoints(trans, WireFramePoints);
        }

        private List<Point3D> TransformListOfPoints(Transform3D trans, List<Point3D> list)
        {
            List<Point3D> transformedPoints = new List<Point3D>();

            foreach (Point3D p in list)
                transformedPoints.Add(trans.Transform(p));

            return transformedPoints;
        }
    }
}
