using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CAnchor : CConnector
    {
        public CAnchor() : base()
        {
        }

        public CAnchor(float fDiameter_temp, float fLength_temp, float fMass_temp, bool bIsDisplayed)
        {
            m_pControlPoint = new CPoint(0,0,0,0,0);
            BIsDisplayed = bIsDisplayed;
            Length = fLength_temp;
            Diameter_thread = fDiameter_temp;
            Mass = fMass_temp;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
        }

        public CAnchor(CPoint controlpoint, float fDiameter_temp, float fLength_temp, float fMass_temp, bool bIsDisplayed)
        {
            m_pControlPoint = controlpoint;
            BIsDisplayed = bIsDisplayed;
            Length = fLength_temp;
            Diameter_thread = fDiameter_temp;
            Mass = fMass_temp;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
        }

        /*
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
            GeometryModel3D geometryModel = new GeometryModel3D();
            return geometryModel;
        }
        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D geometryWireFrameModel = new ScreenSpaceLines3D();
            return geometryWireFrameModel;
        }
        */
    }
}
