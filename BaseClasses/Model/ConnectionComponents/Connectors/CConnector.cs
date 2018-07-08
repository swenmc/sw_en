using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    public class CConnector : CConnectionComponentEntity3D
    {
        public float m_fLength;
        public float m_fDiameterOrGauge;
        public float m_fWeight;
        public float m_iNumberOfThreads;
        public DiffuseMaterial m_DiffuseMat;
        public Cylinder m_cylinder;

        public CConnector()
        {
            BIsDisplayed = true;
            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial();
            m_cylinder = new Cylinder();
        }

        public CConnector(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial();
            m_cylinder = new Cylinder();
        }

        public CConnector(bool bIsDisplayed, float fDiameter_temp, float fLength_temp, float fWeight_temp)
        {
            BIsDisplayed = bIsDisplayed;
            m_fLength = fDiameter_temp;
            m_fDiameterOrGauge = fLength_temp;
            m_fWeight = fWeight_temp;

            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * m_fDiameterOrGauge, m_fLength, m_DiffuseMat);
        }

        protected override void loadIndices()
        {

        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pointCollection = new Point3DCollection();

            return pointCollection;
        }

        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            m_DiffuseMat = new DiffuseMaterial(brush);
            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel = m_cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z), 0.5f * m_fDiameterOrGauge, m_fLength, m_DiffuseMat);
            return geometryModel;
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D geometryWireFrameModel = new ScreenSpaceLines3D();
            return geometryWireFrameModel;
        }
    }
}