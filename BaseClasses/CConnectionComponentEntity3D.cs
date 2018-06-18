using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

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

        public EConnectionComponentType eConnComponentType;

        public CConnectionComponentEntity3D() { }
        protected abstract void loadIndices();
        protected abstract Point3DCollection GetDefinitionPoints();
        public abstract GeometryModel3D CreateGeomModel3D(SolidColorBrush brush);
        public abstract ScreenSpaceLines3D CreateWireFrameModel();
    }
}
