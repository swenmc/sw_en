﻿using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    // Class CReinforcementBar
    [Serializable]
    public class CReinforcementBar : CVolume
    {
        private Point3D m_StartPoint;
        private Point3D m_EndPoint;

        private bool m_BarIsInXDirection; // TODO - toto urobit nejako krajsie (pouzit napriklad nejaky enum X,Y,Z)

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

        public new MATERIAL.CMat_03_00 m_Mat; // Pre Bar predefinovat material z obecneho na STEEL 03_00

        public CReinforcementBar()
        {
        }

        /*
        public CReinforcementBar(int iBar_ID, string materialName, CPoint pControlEdgePoint, Point3D startPoint, Point3D endPoint, float fLength, float fDiameter, Color volColor, float fvolOpacity, bool bIsDisplayed, float fTime)
        {
            ID = iBar_ID;
            m_pControlPoint = pControlEdgePoint;
            m_StartPoint = startPoint;
            m_EndPoint = endPoint;
            m_fDim1 = 0.5f * fDiameter;
            m_fDim2 = fLength;
            m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            m_eShapeType = EVolumeShapeType.eShape3D_Cylinder;

            SetMaterialPropertiesFromDatabase(materialName);
        }
        */

        public CReinforcementBar(int iBar_ID, string materialName, string barName, bool bBarIsInXDirection_temp, CPoint pControlEdgePoint, float fLength, float fDiameter, Color volColor, float fvolOpacity, bool bIsDisplayed, float fTime)
        {
            ID = iBar_ID;
            Name = barName;
            BarIsInXDirection = bBarIsInXDirection_temp;
            m_pControlPoint = pControlEdgePoint;
            m_StartPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);
            //m_EndPoint - závisi od pootocenia
            m_fDim1 = 0.5f * fDiameter;
            m_fDim2 = fLength;
            m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            m_eShapeType = EVolumeShapeType.eShape3D_Cylinder;

            SetMaterialPropertiesFromDatabase(materialName);
        }

        public void SetMaterialPropertiesFromDatabase(string matName)
        {
            m_Mat = new MATERIAL.CMat_03_00();
            m_Mat.Name = matName;
            CMatPropertiesRF prop = DATABASE.CMaterialManager.LoadMaterialPropertiesRF(matName);
            m_Mat.m_fE = (float)prop.E;
            m_Mat.m_fG = (float)prop.G;
            m_Mat.m_fNu = (float)prop.Nu;
            m_Mat.m_fAlpha_T = (float)prop.Alpha;
            m_Mat.m_fRho = (float)prop.Rho;
            m_Mat.m_ft_interval = new float[1] { 0.1f };
            m_Mat.m_ff_yk = new float[1] { (float)prop.Ry };
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D()
        {
            return CreateGeomModel3D(new SolidColorBrush(m_volColor_2));
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(Color colorBrush)
        {
            return CreateGeomModel3D(new SolidColorBrush(colorBrush));
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            DiffuseMaterial mat = new DiffuseMaterial(brush);

            double dRotationAngle_deg = 0;

            if (BarIsInXDirection) // Rotate from Z to X (90 deg about Y)
                dRotationAngle_deg = 90; // Prut smeruje v ose x
            else // Rotate from Z to Y (90 deg about X)
                dRotationAngle_deg = -90; // Prut smeruje v ose y

            CVolume volume = new CVolume(1, EVolumeShapeType.eShape3D_Cylinder, m_pControlPoint, m_fDim1, m_fDim2, m_fDim3, new DiffuseMaterial(brush), true, 0);
            model = volume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0,0,0) /*new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z)*/, m_fDim1, m_fDim2, new DiffuseMaterial(brush));

            // Model Transformation
            // Rotate about X or Y axis

            // Apply multiple transformations to the object.
            // Create and apply a transformation that rotates the object.
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(BarIsInXDirection ? 0 : 1, BarIsInXDirection ? 1 : 0, 0); // Rotate about X or Y
            myAxisAngleRotation3d.Angle = dRotationAngle_deg;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            // Add the rotation transform to a Transform3DGroup
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();
            myTransform3DGroup.Children.Add(myRotateTransform3D);

            // Create and apply translation

            //TranslateTransform3D myTranslateTransform3D = new TranslateTransform3D(0,0,-0.4f);
            TranslateTransform3D myTranslateTransform3D = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D);

            // Set the Transform property of the GeometryModel to the Transform3DGroup which includes 
            // both transformations. The 3D object now has two Transformations applied to it.
            model.Transform = myTransform3DGroup;

            return model;
        }
    }
}