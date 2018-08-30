using _3DTools;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATERIAL;

namespace BaseClasses
{
    public abstract class CConnectionComponentEntity3D : CEntity3D
    {
        private Int32Collection m_TriangleIndices;

        public Int32Collection TriangleIndices
        {
            get { return m_TriangleIndices; }
            set { m_TriangleIndices = value; }
        }

        public new CMat_03_00 m_Mat;

        public float[,] PointsOut2D;
        public Point3D[] arrPoints3D;

        public EConnectionComponentType eConnComponentType;

        public CConnectionComponentEntity3D() { }
        protected abstract void loadIndices();
        public abstract void loadWireFrameIndices();

        protected abstract Point3DCollection GetDefinitionPoints();
        public abstract GeometryModel3D CreateGeomModel3D(SolidColorBrush brush);
        public abstract ScreenSpaceLines3D CreateWireFrameModel();
    }
}
