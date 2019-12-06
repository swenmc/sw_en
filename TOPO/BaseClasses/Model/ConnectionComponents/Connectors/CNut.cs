using System;
using _3DTools;
using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;

namespace BaseClasses
{
    [Serializable]
    public class CNut : CConnectionComponentEntity3D
    {
        private string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }

        private float m_fPitch_coarse;
        public float Pitch_coarse
        {
            get
            {
                return m_fPitch_coarse;
            }

            set
            {
                m_fPitch_coarse = value;
            }
        }

        private float m_fSizeAcrossFlats_max;
        public float SizeAcrossFlats_max
        {
            get
            {
                return m_fSizeAcrossFlats_max;
            }

            set
            {
                m_fSizeAcrossFlats_max = value;
            }
        }

        private float m_fSizeAcrossFlats_min;
        public float SizeAcrossFlats_min
        {
            get
            {
                return m_fSizeAcrossFlats_min;
            }

            set
            {
                m_fSizeAcrossFlats_min = value;
            }
        }

        private float m_fSizeAcrossCorners;
        public float SizeAcrossCorners
        {
            get
            {
                return m_fSizeAcrossCorners;
            }

            set
            {
                m_fSizeAcrossCorners = value;
            }
        }

        private float m_fThickness_max;
        public float Thickness_max
        {
            get
            {
                return m_fThickness_max;
            }

            set
            {
                m_fThickness_max = value;
            }
        }

        private float m_fThickness_min;
        public float Thickness_min
        {
            get
            {
                return m_fThickness_min;
            }

            set
            {
                m_fThickness_min = value;
            }
        }

        private float m_fMass;
        public float Mass
        {
            get
            {
                return m_fMass;
            }

            set
            {
                m_fMass = value;
            }
        }

        private float m_fPrice_PPKG_NZD;
        public float Price_PPKG_NZD
        {
            get
            {
                return m_fPrice_PPKG_NZD;
            }

            set
            {
                m_fPrice_PPKG_NZD = value;
            }
        }

        private float m_fPrice_PPP_NZD;
        public float Price_PPP_NZD
        {
            get
            {
                return m_fPrice_PPP_NZD;
            }

            set
            {
                m_fPrice_PPP_NZD = value;
            }
        }

        [NonSerialized]
        public DiffuseMaterial m_DiffuseMat;
        [NonSerialized]
        public GeometryModel3D Visual_Nut;

        public float m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg;

        public CNut() : base()
        {
        }

        public CNut(string name_temp, string nameMaterial_temp, Point3D controlpoint, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Prefix = "Nut";
            m_Name = name_temp;

            m_pControlPoint = controlpoint;

            CBoltNutProperties properties = CBoltNutsManager.GetBoltNutProperties(Name, "Nuts");

            m_fPitch_coarse = (float)properties.Pitch_coarse;
            m_fSizeAcrossFlats_max = (float)properties.SizeAcrossFlats_max;
            m_fSizeAcrossFlats_min = (float)properties.SizeAcrossFlats_min;
            m_fSizeAcrossCorners = (float)properties.SizeAcrossCorners;
            m_fThickness_max = (float)properties.Thickness_max;
            m_fThickness_min = (float)properties.Thickness_min;
            m_fMass = (float)properties.Mass;
            m_fPrice_PPKG_NZD = (float)properties.Price_PPKG_NZD;
            m_fPrice_PPP_NZD = (float)properties.Price_PPP_NZD;

            m_Mat.Name = nameMaterial_temp;
            ((CMat_03_00)m_Mat).m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            ((CMat_03_00)m_Mat).m_ff_yk = new float[1] { (float)materialProperties.Fy };
            ((CMat_03_00)m_Mat).m_ff_u = new float[1] { (float)materialProperties.Fu };

            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
        }

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

        public override void loadWireFrameIndices()
        { }
    }
}
