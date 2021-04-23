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
                //bool changedValue = m_iGauge != value;
                m_iGauge = value;

                //if(changedValue) UpdateAllValuesOnGaugeChange();
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

        private float m_ft_ha_headAgonThickness;
        public float T_ha_headAgonThickness
        {
            get
            {
                return m_ft_ha_headAgonThickness;
            }

            set
            {
                m_ft_ha_headAgonThickness = value;
            }
        }

        private float m_ft_ht_headTotalThickness;
        public float T_ht_headTotalThickness
        {
            get
            {
                return m_ft_ht_headTotalThickness;
            }

            set
            {
                m_ft_ht_headTotalThickness = value;
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
        public float D_predrillholediameter
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

        public CScrew(string sName_temp, string gauge)
        {
            m_Mat.Name = "Class 3 / 4 / B8"; // TODO - pripravit databazu materialov pre skrutky
            Prefix = "TEK"; // TODO - Urcit co je Prefix a co je Name u skrutiek

            Name = sName_temp;
            CTEKScrewProp screwProperties = CTEKScrewsManager.GetScrewProperties2(gauge);
            SetScrewValuesFromDatabase(screwProperties);
        }

        public CScrew(CScrew referenceScrew, Point3D controlpoint, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg)
        {
            m_Mat.Name = referenceScrew.m_Mat.Name;
            Prefix = referenceScrew.Prefix;
            Name = referenceScrew.Name;
            m_iGauge = referenceScrew.Gauge;
            Diameter_thread = referenceScrew.Diameter_thread;
            Diameter_shank = referenceScrew.Diameter_shank;

            Diameter_hole = GetDiameter_Hole();
            //threadType1;
            //threadsPerInch1;
            //threadType2;
            //threadsPerInch2;
            //threadType3;
            //threadsPerInch3;
            //headSizeInch;
            m_fd_h_headdiameter = referenceScrew.D_h_headdiameter;
            m_ft_ha_headAgonThickness = referenceScrew.T_ha_headAgonThickness;
            m_ft_ht_headTotalThickness = referenceScrew.T_ht_headTotalThickness;
            m_fd_w_washerdiameter = referenceScrew.D_w_washerdiameter;
            m_ft_w_washerthickness = referenceScrew.T_w_washerthickness;
            m_fd_predrillholediameter = referenceScrew.D_predrillholediameter;
            m_fShearStrength_nominal = referenceScrew.ShearStrength_nominal;
            m_fAxialTensileStrength_nominal = referenceScrew.AxialTensileStrength_nominal;
            m_fTorsionalStrength_nominal = referenceScrew.TorsionalStrength_nominal;
            Mass = referenceScrew.Mass;
            Price_PPP_NZD = referenceScrew.Price_PPP_NZD;
            Price_PPKG_NZD = referenceScrew.Price_PPKG_NZD;

            Length = referenceScrew.Length;

            ControlPoint = controlpoint;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
        }

        public void SetScrewValuesFromDatabase(CTEKScrewProp properties)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            m_iGauge = properties.gauge;  // Gauge - hodnota int
            Diameter_thread = (float)properties.threadDiameter;
            Diameter_shank = (float)properties.shankDiameter;
            float shankLength = (float)properties.shankLength;

            Diameter_hole = GetDiameter_Hole();
            //threadType1;
            //threadsPerInch1;
            //threadType2;
            //threadsPerInch2;
            //threadType3;
            //threadsPerInch3;
            //headSizeInch;
            m_fd_h_headdiameter = (float)properties.headSize;
            m_ft_ha_headAgonThickness = (float)properties.headAgonThickness;
            m_ft_ht_headTotalThickness = (float)properties.headTotalThickness;
            m_fd_w_washerdiameter = (float)properties.washerSize;
            m_ft_w_washerthickness = (float)properties.washerThickness;
            m_fd_predrillholediameter = (float)properties.preDrillHoleDiameter_3mmthickness;
            m_fShearStrength_nominal = (float)properties.shearStrength_N;
            m_fAxialTensileStrength_nominal = (float)properties.axialTensileStrength_N;
            m_fTorsionalStrength_nominal = (float)properties.torsionalStrength_Nm;
            Mass = (float)properties.mass_kg;
            Price_PPP_NZD = (float)properties.price_PPP_NZD;
            Price_PPKG_NZD = (float)properties.price_PPKG_NZD;

            Length = shankLength + m_ft_ht_headTotalThickness; // Celkova dlzka vratane hlavy
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


        public void UpdateAllValuesOnGaugeChange()
        {
            CTEKScrewProp screwProperties = CTEKScrewsManager.GetScrewProperties2(m_iGauge.ToString()); // !!! Ak je to int tak sa uvazuje ID, ak je to string tak gauge
            SetScrewValuesFromDatabase(screwProperties);

            System.Diagnostics.Trace.WriteLine("UpdateAllValuesOnGaugeChange call " + DateTime.Now.ToLongTimeString());
        }
    }
}
