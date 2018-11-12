using _3DTools;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATERIAL;
using System;

namespace BaseClasses
{
    [Serializable]
    public abstract class CConnectionComponentEntity3D : CEntity3D
    {
        [NonSerialized]
        private Int32Collection m_TriangleIndices;
                
        public Int32Collection TriangleIndices
        {
            get { return m_TriangleIndices; }
            set { m_TriangleIndices = value; }
        }        
        public Point[] PointsOut2D;
        /*
        public Point[] m_arrPoints2D;
        public Point[] PointsOut2D
        {
            get { return m_arrPoints2D; }
            set { m_arrPoints2D = value; }
        }
        */
        [NonSerialized]
        private Point3D[] m_arrPoints3D;
        
        public Point3D[] arrPoints3D
        {
            get { return m_arrPoints3D; }
            set { m_arrPoints3D = value; }
        }

        [NonSerialized]
        public new CMat_03_00 m_Mat;
        [NonSerialized]
        public EConnectionComponentType eConnComponentType;

        public CConnectionComponentEntity3D() { }
        protected abstract void loadIndices();
        public abstract void loadWireFrameIndices();

        protected abstract Point3DCollection GetDefinitionPoints();
        public abstract GeometryModel3D CreateGeomModel3D(SolidColorBrush brush);
        public abstract ScreenSpaceLines3D CreateWireFrameModel();
    }
}
