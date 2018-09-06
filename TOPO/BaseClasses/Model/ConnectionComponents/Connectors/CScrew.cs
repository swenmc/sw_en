using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CScrew : CConnector
    {
        private EScrewTypes m_eType;
        public EScrewTypes Type
        {
            get
            {
                return m_eType;
            }

            set
            {
                m_eType = value;
            }
        }

        private float m_fd_h_headdiameter;
        public float D_h_headdiameter
        {
            get
            {
                return m_fd_h_headdiameter;
            }

            set
            {
                m_fd_h_headdiameter = value;
            }
        }

        private float m_fd_w_washerdiameter;
        public float D_w_washerdiameter
        {
            get
            {
                return m_fd_w_washerdiameter;
            }

            set
            {
                m_fd_w_washerdiameter = value;
            }
        }

        private float m_ft_w_washerthickness;
        public float T_w_washerthickness
        {
            get
            {
                return m_ft_w_washerthickness;
            }

            set
            {
                m_ft_w_washerthickness = value;
            }
        }

        public CScrew()
        { }

        public CScrew(string sName_temp, CPoint controlpoint, int iGauge_temp, float fDiameter_temp, float fLength_temp, float fWeight_temp, bool bIsDisplayed)
        {
            Name = sName_temp;
            m_pControlPoint = controlpoint;
            BIsDisplayed = bIsDisplayed;
            m_fLength = fLength_temp;
            m_iGauge = iGauge_temp;
            m_fDiameter = fDiameter_temp;
            m_fWeight = fWeight_temp;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * m_fDiameter, m_fLength, m_DiffuseMat);
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
