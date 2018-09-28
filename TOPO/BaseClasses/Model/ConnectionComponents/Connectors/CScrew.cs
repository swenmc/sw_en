using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Globalization;
using MATERIAL;
using DATABASE;
using DATABASE.DTO;

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

        private int m_iGauge;
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

        public CScrew()
        { }

        public CScrew(string sName_temp, CPoint controlpoint, int iGauge_temp, float fDiameter_thread_temp, float fHeadDiameter_temp, float fWasherDiameter_temp, float fWasherThickness_temp, float fLength_temp, float fWeight_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            m_Mat = new CMat_03_00(); // Todo - material ako parameter
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
            Weight = fWeight_temp;
            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
        }

        public CScrew(string sName_temp, CTEKScrewProperties screwproperties)
        {
            Name = sName_temp;
            SetScrewValuesFromDatabase(screwproperties);
        }

        public CScrew(string sName_temp, string gauge)
        {
            CTEKScrewProperties screwProperties = CTEKScrewsManager.LoadScrewProperties(gauge);
            SetScrewValuesFromDatabase(screwProperties);
        }

        public void SetScrewValuesFromDatabase(CTEKScrewProperties properties)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            Gauge = int.Parse(properties.gauge, nfi);
            try
            {
                Diameter_thread = float.Parse(properties.threadDiameter, nfi) / 1000f;
            }
            catch
            {
                Diameter_thread = 0;
            }

            try
            {
                Diameter_shank = float.Parse(properties.shankDiameter, nfi) / 1000f;
            }
            catch
            {
                Diameter_shank = 0;
            }

            /*
            properties.threadType1
            properties.threadsPerInch1
            properties.threadType2
            properties.threadsPerInch2
            properties.threadType3
            properties.threadsPerInch3
            properties.headSizeInch
            */


            // Temporary - chybajuce data v databaze

            try
            {
                D_h_headdiameter = float.Parse(properties.headSizemm, nfi) / 1000f;
            }
            catch
            {
                D_h_headdiameter = 0;
            }

            try
            {
                D_w_washerdiameter = float.Parse(properties.washerSizemm, nfi) / 1000f;
            }
            catch
            {
                D_w_washerdiameter = 0;
            }

            try
            {
                T_w_washerthickness = float.Parse(properties.washerThicknessmm, nfi) / 1000f;
            }
            catch
            {
                T_w_washerthickness = 0;
            }

            // Default
            Length = 0.009f; // m
            Weight = 0.015f; // kg
        }

        //float fScrewWeight = 0.012f;

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
