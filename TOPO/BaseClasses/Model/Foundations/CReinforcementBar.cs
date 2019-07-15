using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // Class CReinforcementBar
    [Serializable]
    public class CReinforcementBar : CVolume
    {
        private Point3D m_StartPoint;
        private Point3D m_EndPoint;

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

        bool BarIsInXDirection;

        public CReinforcementBar()
        {
        }

        public CReinforcementBar(int iBar_ID, CPoint pControlEdgePoint, Point3D startPoint, Point3D endPoint, float fLength, float fDiameter, Color volColor, float fvolOpacity, bool bIsDisplayed, float fTime)
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
        }

        public CReinforcementBar(int iBar_ID, bool bBarIsInXDirection, CPoint pControlEdgePoint, float fLength, float fDiameter, Color volColor, float fvolOpacity, bool bIsDisplayed, float fTime)
        {
            ID = iBar_ID;
            BarIsInXDirection = bBarIsInXDirection;
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

            double dRotationAboutX = 0;
            double dRotationAboutY = 0;

            if (BarIsInXDirection) // Rotate from Z to X (90 deg about Y)
                dRotationAboutY = 90;
            else // Rotate from Z to Y (90 deg about X)
                dRotationAboutX = 90;

            CVolume volume = new CVolume(1, EVolumeShapeType.eShape3D_Cylinder, m_pControlPoint, m_fDim1, m_fDim2, m_fDim3, new DiffuseMaterial(brush), true, 0);
            model = volume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z), m_fDim1, m_fDim2, new DiffuseMaterial(brush));

            return model;
        }
    }
}
