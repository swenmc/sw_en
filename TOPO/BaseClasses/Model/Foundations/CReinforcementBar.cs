using _3DTools;
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
    // Class CReinforcementBar
    [Serializable]
    public class CReinforcementBar : Cylinder
    {
        private Point3D m_StartPoint;
        private Point3D m_EndPoint;
        private float m_diameter;
        private float m_fArea_As_1;
        private float m_totalLength;

        private bool m_BarIsInXDirection; // TODO - toto urobit nejako krajsie (pouzit napriklad nejaky enum X,Y,Z)

        private bool m_bIsStraight;
        private bool m_bIsTop_U;

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

         public bool IsStraight
        {
            get
            {
                return m_bIsStraight;
            }

            set
            {
                m_bIsStraight = value;
            }
        }

        public bool IsTop_U
        {
            get
            {
                return m_bIsTop_U;
            }

            set
            {
                m_bIsTop_U = value;
            }
        }

        public CReinforcementBar()
        {
        }

        // TODO Ondrej - nahradit CVolume triedou Cylinder (zrusit dedenie od CVolume) a refaktorovat s CConnector, pripravit wireframe model pre reinforcement bars
        public CReinforcementBar(int iBar_ID, string materialName, string barName, bool bBarIsInXDirection_temp, Point3D pControlEdgePoint, float fLength, float fDiameter, /*Color volColor,*/ float fvolOpacity, bool bIsStraight, bool bIsTop_U, bool bIsDisplayed, float fTime)
        {
            ID = iBar_ID;
            Name = barName;
            BarIsInXDirection = bBarIsInXDirection_temp;
            m_pControlPoint = pControlEdgePoint;
            m_StartPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);
            //m_EndPoint - závisi od pootocenia

            m_EndPoint = new Point3D(m_pControlPoint.X + fLength, m_pControlPoint.Y, m_pControlPoint.Z);
            if (!BarIsInXDirection)
            {
                m_EndPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y + fLength, m_pControlPoint.Z);
            }

            m_fr = 0.5f * fDiameter;
            m_fL = fLength;
            //m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            m_bIsStraight = bIsStraight;
            m_bIsTop_U = bIsTop_U;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            m_diameter = fDiameter;
            m_totalLength = fLength;

            //m_eShapeType = EVolumeShapeType.eShape3D_Cylinder;

            m_fArea_As_1 = MathF.fPI * MathF.Pow2(m_diameter) / 4f; // Reinforcement bar cross-sectional area

            SetMaterialPropertiesFromDatabase(materialName);
        }

        public void SetMaterialPropertiesFromDatabase(string matName)
        {
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

        // TODO Ondrej - nahradit CVolume triedou Cylinder (zrusit dedenie od CVolume) a refaktorovat s CConnector, pripravit wireframe model pre reinforcement bars

        public /*override*/ Model3DGroup CreateModel3DGroup(float fOpacity, Transform3DGroup temp)
        {
            SolidColorBrush brush = new SolidColorBrush(m_volColor_2);
            brush.Opacity = fOpacity;

            return CreateModel3DGroup(brush, temp);
        }

        public /*override*/ Model3DGroup CreateModel3DGroup(Color colorBrush, float fOpacity, Transform3DGroup temp)
        {
            SolidColorBrush brush = new SolidColorBrush(colorBrush);
            brush.Opacity = fOpacity;
            Model3DGroup Visual_Object = CreateModel3DGroup(brush, temp);
            return Visual_Object;
        }

        public /*override*/ Model3DGroup CreateModel3DGroup(SolidColorBrush brush, Transform3DGroup temp)
        {
            Model3DGroup modelGroup = new Model3DGroup();

            DiffuseMaterial mat = new DiffuseMaterial(brush);

            // Vytvorime model ktory smeruje v ose X
            if (m_bIsStraight) // Priamy prut
            {
                GeometryModel3D model = new GeometryModel3D();
                model = CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), 12 + 1, m_fr, m_fL, new DiffuseMaterial(brush), 0);
                modelGroup.Children.Add(model);
            }
            else
            {
                CSolidCircleBar_U bar = new CSolidCircleBar_U(m_BarIsInXDirection, m_pControlPoint, m_diameter, 0.03f, m_bIsTop_U, new DiffuseMaterial(brush));
                modelGroup = bar.CreateM_3D_G_Volume_U_Bar(new Point3D(0, 0, 0), 12 + 1);
            }

            double dRotationAngle_deg = 0;

            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            if(!m_bIsStraight && m_bIsTop_U) // Is U shape and upper reinforcement
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
            TranslateTransform3D myTranslateTransform3D = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

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
            myTransform3DGroup.Children.Add(temp);

            // Set the Transform property of the modelGroup to the Transform3DGroup which includes 
            // both transformations. The 3D object now has two Transformations applied to it.
            modelGroup.Transform = myTransform3DGroup;
            //---------------------------------------------------------------------------

            return modelGroup;
        }
    }
}
