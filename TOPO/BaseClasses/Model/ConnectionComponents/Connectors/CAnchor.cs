using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    public class CAnchor : CConnector
    {
        // Anchor to plate edge distances
        private float m_fx_pe_minus;
        private float m_fx_pe_plus;
        private float m_fy_pe_minus;
        private float m_fy_pe_plus;

        private float m_fx_pe_min;
        private float m_fy_pe_min;
        private float m_fx_pe_max;
        private float m_fy_pe_max;

        // Anchor to foundation edge distances
        private float m_fx_fe_minus;
        private float m_fx_fe_plus;
        private float m_fy_fe_minus;
        private float m_fy_fe_plus;

        private float m_fx_fe_min;
        private float m_fy_fe_min;
        private float m_fx_fe_max;
        private float m_fy_fe_max;

        // Washer size
        // Plate washer
        private float m_fx_washer_plate;
        private float m_fy_washer_plate;

        // Bearing washer
        private float m_fx_washer_bearing;
        private float m_fy_washer_bearing;

        private float m_fh_effective; // Effective Depth

        //-------------------------------------------------------------------------------------------------------------
        public float x_pe_minus
        {
            get
            {
                return m_fx_pe_minus;
            }

            set
            {
                m_fx_pe_minus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_pe_plus
        {
            get
            {
                return m_fx_pe_plus;
            }

            set
            {
                m_fx_pe_plus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_pe_minus
        {
            get
            {
                return m_fy_pe_minus;
            }

            set
            {
                m_fy_pe_minus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_pe_plus
        {
            get
            {
                return m_fy_pe_plus;
            }

            set
            {
                m_fy_pe_plus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_pe_min
        {
            get
            {
                return m_fx_pe_min;
            }

            set
            {
                m_fx_pe_min = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_pe_max
        {
            get
            {
                return m_fx_pe_max;
            }

            set
            {
                m_fx_pe_max = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_pe_min
        {
            get
            {
                return m_fy_pe_min;
            }

            set
            {
                m_fy_pe_min = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_pe_max
        {
            get
            {
                return m_fy_pe_max;
            }

            set
            {
                m_fy_pe_max = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_fe_minus
        {
            get
            {
                return m_fx_fe_minus;
            }

            set
            {
                m_fx_fe_minus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_fe_plus
        {
            get
            {
                return m_fx_fe_plus;
            }

            set
            {
                m_fx_fe_plus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_fe_minus
        {
            get
            {
                return m_fy_fe_minus;
            }

            set
            {
                m_fy_fe_minus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_fe_plus
        {
            get
            {
                return m_fy_fe_plus;
            }

            set
            {
                m_fy_fe_plus = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_fe_min
        {
            get
            {
                return m_fx_fe_min;
            }

            set
            {
                m_fx_fe_min = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_fe_max
        {
            get
            {
                return m_fx_fe_max;
            }

            set
            {
                m_fx_fe_max = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_fe_min
        {
            get
            {
                return m_fy_fe_min;
            }

            set
            {
                m_fy_fe_min = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_fe_max
        {
            get
            {
                return m_fy_fe_max;
            }

            set
            {
                m_fy_fe_max = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_washer_plate
        {
            get
            {
                return m_fx_washer_plate;
            }

            set
            {
                m_fx_washer_plate = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_washer_plate
        {
            get
            {
                return m_fy_washer_plate;
            }

            set
            {
                m_fy_washer_plate = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float x_washer_bearing
        {
            get
            {
                return m_fx_washer_bearing;
            }

            set
            {
                m_fx_washer_bearing = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float y_washer_bearing
        {
            get
            {
                return m_fy_washer_bearing;
            }

            set
            {
                m_fy_washer_bearing = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float h_effective
        {
            get
            {
                return m_fh_effective;
            }

            set
            {
                m_fh_effective = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private float m_fDiameter_pitch;
        public float Diameter_pitch
        {
            get
            {
                return m_fDiameter_pitch;
            }

            set
            {
                m_fDiameter_pitch = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private float m_Area_p_pitch;
        public float Area_p_pitch
        {
            get
            {
                return m_Area_p_pitch;
            }

            set
            {
                m_Area_p_pitch = value;
            }
        }

        public CAnchor() : base()
        {
        }

        public CAnchor(string name_temp, float fLength_temp, bool bIsDisplayed)
        {
            Prefix = "Anchor";
            Name = name_temp;
            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            Length = fLength_temp;

            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name);

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            // Washer size
            // Plate washer
            // TODO - zavisi od rozmerov plechu
            x_washer_plate = 0.08f; // 80 mm
            y_washer_plate = 0.08f; // 80 mm

            // Bearing washer
            x_washer_bearing = 0.06f; // 60 mm
            y_washer_bearing = 0.06f; // 60 mm

            h_effective = 0.330f; // 330 mm (efektivna dlzka tyce zabetonovana v zaklade)

            m_Mat.Name = "8.8";
            m_Mat.m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            m_Mat.m_ff_yk = new float[1] { (float)materialProperties.Fy };
            m_Mat.m_ff_u = new float[1] { (float)materialProperties.Fu };

            Mass = GetMass();

            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = 0;
            m_fRotationY_deg = 90;
            m_fRotationZ_deg = 0;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);
        }

        public CAnchor(string name_temp, string nameMaterial_temp, float fLength_temp, float fh_eff_temp, bool bIsDisplayed)
        {
            Prefix = "Anchor";
            Name = name_temp;
            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            Length = fLength_temp;

            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name);

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            // Washer size
            // Plate washer
            // TODO - zavisi od rozmerov plechu
            x_washer_plate = 0.08f; // 80 mm
            y_washer_plate = 0.08f; // 80 mm

            // Bearing washer
            x_washer_bearing = 0.06f; // 60 mm
            y_washer_bearing = 0.06f; // 60 mm

            h_effective = fh_eff_temp; // Efektivna dlzka tyce zabetonovana v zaklade

            m_Mat.Name = nameMaterial_temp;
            m_Mat.m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            m_Mat.m_ff_yk = new float[1] { (float)materialProperties.Fy };
            m_Mat.m_ff_u = new float[1] { (float)materialProperties.Fu };

            Mass = GetMass();

            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = 0;
            m_fRotationY_deg = 90;
            m_fRotationZ_deg = 0;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);
        }

        public CAnchor(string name_temp, string nameMaterial_temp, CPoint controlpoint, float fLength_temp, float fh_eff_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Prefix = "Anchor";
            Name = name_temp;
            m_pControlPoint = controlpoint;
            Length = fLength_temp;

            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name);

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            // Washer size
            // Plate washer
            // TODO - zavisi od rozmerov plechu
            x_washer_plate = 0.08f; // 80 mm
            y_washer_plate = 0.08f; // 80 mm

            // Bearing washer
            x_washer_bearing = 0.06f; // 60 mm
            y_washer_bearing = 0.06f; // 60 mm

            h_effective = fh_eff_temp; // Efektivna dlzka tyce zabetonovana v zaklade

            m_Mat.Name = nameMaterial_temp;
            m_Mat.m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            m_Mat.m_ff_yk = new float[1] { (float)materialProperties.Fy };
            m_Mat.m_ff_u = new float[1] { (float)materialProperties.Fu };

            Mass = GetMass();

            BIsDisplayed = bIsDisplayed;

            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);
        }

        public float GetMass()
        {
            return Area_p_pitch * Length * m_Mat.m_fRho;
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
