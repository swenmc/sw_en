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

        public CBolt(Point3D controlpoint, float fDiameter_shank, float fLength_temp, float fMass_temp)
        {
            ControlPoint = controlpoint;
            Length = fLength_temp;
            Diameter_shank = fDiameter_shank;

            Diameter_hole = GetDiameter_Hole();

            Mass = fMass_temp;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
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
