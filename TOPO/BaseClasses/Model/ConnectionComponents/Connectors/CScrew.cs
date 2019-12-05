using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Globalization;
using MATERIAL;
using DATABASE;
using DATABASE.DTO;
using System;

namespace BaseClasses
{
    [Serializable]
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

        private int m_iGauge;  // Gauge v mm
        public int Gauge
        {
            get
            {
                return m_iGauge;
            }

            set
            {
                m_iGauge = value;
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

        private float m_fd_predrillholediameter;
        public float D_holediameter
        {
            get
            {
                return m_fd_predrillholediameter;
            }

            set
            {
                m_fd_predrillholediameter = value;
            }
        }

        private float m_fShearStrength_nominal;
        public float ShearStrength_nominal
        {
            get
            {
                return m_fShearStrength_nominal;
            }

            set
            {
                m_fShearStrength_nominal = value;
            }
        }

        private float m_fAxialTensileStrength_nominal;
        public float AxialTensileStrength_nominal
        {
            get
            {
                return m_fAxialTensileStrength_nominal;
            }

            set
            {
                m_fAxialTensileStrength_nominal = value;
            }
        }

        private float m_fTorsionalStrength_nominal;
        public float TorsionalStrength_nominal
        {
            get
            {
                return m_fTorsionalStrength_nominal;
            }

            set
            {
                m_fTorsionalStrength_nominal = value;
            }
        }

        public CScrew()
        { }

        public CScrew(string sName_temp, Point3D controlpoint, int iGauge_temp, float fDiameter_thread_temp, float fHeadDiameter_temp, float fWasherDiameter_temp, float fWasherThickness_temp, float fLength_temp, float fMass_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            m_Mat.Name = "Class 3 / 4 / B8"; // TODO - pripravit databazu materialov pre skrutky
            Prefix = "TEK"; // TODO - Urcit co je Prefix a co je Name u skrutiek

            Name = sName_temp;
            m_pControlPoint = controlpoint;
            m_iGauge = iGauge_temp;
            Diameter_thread = fDiameter_thread_temp;

            D_h_headdiameter = fHeadDiameter_temp;
            D_w_washerdiameter = fWasherDiameter_temp;
            T_w_washerthickness = fWasherThickness_temp;

            Length = fLength_temp;
            Mass = fMass_temp;
            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
        }

        public CScrew(string sName_temp, string gauge)
        {
            Name = sName_temp;
            CTEKScrewProp screwProperties = CTEKScrewsManager.GetScrewProperties2(gauge);
            SetScrewValuesFromDatabase(screwProperties);
        }

        public void SetScrewValuesFromDatabase(CTEKScrewProp properties)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            m_iGauge = properties.gauge;  // Gauge v mm
            Diameter_thread = (float)properties.threadDiameter;
            Diameter_shank = (float)properties.shankDiameter;
            Length = (float)properties.shankLength;
            //threadType1;
            //threadsPerInch1;
            //threadType2;
            //threadsPerInch2;
            //threadType3;
            //threadsPerInch3;
            //headSizeInch;
            m_fd_h_headdiameter = (float)properties.headSize;
            //headThicknessmm;
            m_fd_w_washerdiameter = (float)properties.washerSize;
            m_ft_w_washerthickness = (float)properties.washerThickness;
            m_fd_predrillholediameter = (float)properties.preDrillHoleDiameter_3mmthickness;
            m_fShearStrength_nominal = (float)properties.shearStrength_N;
            m_fAxialTensileStrength_nominal = (float)properties.axialTensileStrength_N;
            m_fTorsionalStrength_nominal = (float)properties.torsionalStrength_Nm;
            Mass = (float)properties.mass_kg;
            Price_PPP_NZD = (float)properties.price_PPP_NZD;
            Price_PPKG_NZD = (float)properties.price_PPKG_NZD;
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
