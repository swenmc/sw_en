using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CBolt : CConnector
    {
        public CBolt()
        { }

        public CBolt(CPoint controlpoint, int iGauge_temp, float fDiameter_temp, float fLength_temp, float fWeight_temp, bool bIsDisplayed)
        {
            m_pControlPoint = controlpoint;
            BIsDisplayed = bIsDisplayed;
            Length = fLength_temp;
            Diameter_thread = fDiameter_temp;
            Weight = fWeight_temp;

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
