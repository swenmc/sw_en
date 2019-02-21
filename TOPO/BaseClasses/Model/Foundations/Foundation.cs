using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // Class CFoundation
    [Serializable]
    public class CFoundation : CVolume
    {
        public EFoundationType eFoundationType;

        // Rectangular prism
        public CFoundation(int iFoundation_ID, EFoundationType eType, CPoint pControlEdgePoint, float fX, float fY, float fZ, Color volColor, float fvolOpacity, bool bIsDisplayed, float fTime)
        {
            ID = iFoundation_ID;
            eFoundationType = eType;
            m_pControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
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
            if (eFoundationType == EFoundationType.ePad)
            {
                CVolume volume = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, m_pControlPoint, m_fDim1, m_fDim2, m_fDim3, new DiffuseMaterial(brush), true, 0);
                model = volume.CreateGM_3D_Volume_8Edges(volume);
            }
            else
            {
                throw new NotImplementedException();
            }

            return model;
        }
    }
}
