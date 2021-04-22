using System;
using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System.Collections.Generic;

namespace BaseClasses
{
    [Serializable]
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

        private float m_fh_effective; // Effective Depth
        private float m_fWasherBearing_OffsetFromBottom; // Vzdialenost od spodnej hrany kotvy po spodnu hranu washer

        private bool m_bIsActiveInTension; // TODO - toto by mohla byt univerzalna vlastnost pre predka CConnector
        private bool m_bIsActiveInShear; // TODO - toto by mohla byt univerzalna vlastnost pre predka CConnector

        private List<CNut> m_Nuts;

        private float fOffsetFor3D = 0.0001f; // Offset medzi washer a nut pre krajsiu 3D grafiku

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
        public float WasherBearing_OffsetFromBottom
        {
            get
            {
                return m_fWasherBearing_OffsetFromBottom;
            }

            set
            {
                m_fWasherBearing_OffsetFromBottom = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsActiveInTension
        {
            get
            {
                return m_bIsActiveInTension;
            }

            set
            {
                m_bIsActiveInTension = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsActiveInShear
        {
            get
            {
                return m_bIsActiveInShear;
            }

            set
            {
                m_bIsActiveInShear = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<CNut> Nuts
        {
            get
            {
                return m_Nuts;
            }

            set
            {
                m_Nuts = value;
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

        //-------------------------------------------------------------------------------------------------------------
        private float m_Price_PPLM_NZD;
        public float Price_PPLM_NZD
        {
            get
            {
                return m_Price_PPLM_NZD;
            }

            set
            {
                m_Price_PPLM_NZD = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        private float m_fPortionOtAnchorAbovePlate_abs; // Vzdialenost horneho okraja kotvy od spodnej hrany plechu (povrchu betonoveho zakladu)
        public float PortionOtAnchorAbovePlate_abs
        {
            get
            {
                return m_fPortionOtAnchorAbovePlate_abs;
            }

            set
            {
                m_fPortionOtAnchorAbovePlate_abs = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private CWasher_W m_WasherPlateTop;
                
        public CWasher_W WasherPlateTop
        {
            get
            {
                return m_WasherPlateTop;
            }

            set
            {
                m_WasherPlateTop = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private CWasher_W m_WasherBearing;
        public CWasher_W WasherBearing
        {
            get
            {
                return m_WasherBearing;
            }

            set
            {
                m_WasherBearing = value;
            }
        }

        public CAnchor() : base()
        {
        }

        public CAnchor(string name_temp, float fLength_temp /*CWasher_W washerPlateTop, CWasher_W washerBearing,*/)
        {
            Prefix = "Anchor";
            Name = name_temp;
            ControlPoint = new Point3D(0, 0, 0);
            Length = fLength_temp;

            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name, "ThreadedBars");

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Diameter_hole = GetDiameter_Hole();

            Price_PPKG_NZD = (float)properties.Price_PPKG_NZD;
            Price_PPLM_NZD = (float)properties.Price_PPLM_NZD;
            Price_PPP_NZD = (float)properties.Price_PPLM_NZD * fLength_temp;
            Mass = (float)properties.Mass_kg_LM * fLength_temp;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            h_effective = 0.90909f * fLength_temp; // 300 mm (efektivna dlzka tyce zabetonovana v zaklade)

            m_bIsActiveInTension = true; // Default
            m_bIsActiveInShear = true; // Default

            ((CMat_03_00)m_Mat).Name = "8.8";
            ((CMat_03_00)m_Mat).m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            ((CMat_03_00)m_Mat).m_ff_yk = new float[1] { (float)materialProperties.Fy };
            ((CMat_03_00)m_Mat).m_ff_u = new float[1] { (float)materialProperties.Fu };

            //Mass = GetMass();

            m_fRotationX_deg = 0;
            m_fRotationY_deg = 90;
            m_fRotationZ_deg = 0;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);

            SetPortionOtAnchorAbovePlateDefault();
        }

        public CAnchor(string name_temp, string nameMaterial_temp, float fLength_temp, float fh_eff_temp, /*CWasher_W washerPlateTop, CWasher_W washerBearing,*/ bool bIsDisplayed)
        {
            Prefix = "Anchor";
            Name = name_temp;
            ControlPoint = new Point3D(0, 0, 0);
            Length = fLength_temp;

            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name, "ThreadedBars");

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Diameter_hole = GetDiameter_Hole();

            Price_PPKG_NZD = (float)properties.Price_PPKG_NZD;
            Price_PPLM_NZD = (float)properties.Price_PPLM_NZD;
            Price_PPP_NZD = (float)properties.Price_PPLM_NZD * fLength_temp;
            Mass = (float)properties.Mass_kg_LM * fLength_temp;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            h_effective = fh_eff_temp; // Efektivna dlzka tyce zabetonovana v zaklade

            m_bIsActiveInTension = true; // Default
            m_bIsActiveInShear = true; // Default

            m_Mat.Name = nameMaterial_temp;
            ((CMat_03_00)m_Mat).m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            ((CMat_03_00)m_Mat).m_ff_yk = new float[1] { (float)materialProperties.Fy };
            ((CMat_03_00)m_Mat).m_ff_u = new float[1] { (float)materialProperties.Fu };

            //Mass = GetMass();

            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = 0;
            m_fRotationY_deg = 90;
            m_fRotationZ_deg = 0;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);

            SetPortionOtAnchorAbovePlateDefault();
        }

        // Tento konstruktor sa pouziva pre vytvorenie realnych kotiev - vstupne parametre su parametre referencnej kotvy
        // TODO Ondrej - mozno by sme mali mat konstruktor kde do CAnchor vstupuje iny objekt CAnchor (referencna kotva) a potom sa v konsruktore len vybrane parametre
        // (okrem control point a natocenia) skopiruju zo vstupneho objektu do properties vytvaraneho objektu
        //
        public CAnchor(string name_temp, string nameMaterial_temp, Point3D controlpoint, float fLength_temp, float fh_eff_temp, float fPortionOtAnchorAbovePlate_abs_temp, CWasher_W washerPlateTop, CWasher_W washerBearing, /*bool bIsActiveInTension, bool bIsActiveInShear, */float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Prefix = "Anchor";
            Name = name_temp;
            ControlPoint = controlpoint;
            Length = fLength_temp;

            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name, "ThreadedBars");

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Diameter_hole = GetDiameter_Hole();

            Price_PPKG_NZD = (float)properties.Price_PPKG_NZD;
            Price_PPLM_NZD = (float)properties.Price_PPLM_NZD;
            Price_PPP_NZD = (float)properties.Price_PPLM_NZD * fLength_temp;
            Mass = (float)properties.Mass_kg_LM * fLength_temp;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            m_fh_effective = fh_eff_temp; // Efektivna dlzka tyce zabetonovana v zaklade
            m_fPortionOtAnchorAbovePlate_abs = fPortionOtAnchorAbovePlate_abs_temp;

            m_bIsActiveInTension = true; // Default
            m_bIsActiveInShear = true; // Default

            m_Mat.Name = nameMaterial_temp;
            ((CMat_03_00)m_Mat).m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            ((CMat_03_00)m_Mat).m_ff_yk = new float[1] { (float)materialProperties.Fy };
            ((CMat_03_00)m_Mat).m_ff_u = new float[1] { (float)materialProperties.Fu };

            //Mass = GetMass();

            BIsDisplayed = bIsDisplayed;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);

            // Tato funkcia sa nevola, kedze fPortionOtAnchorAbovePlate je vstupny parameter
            // Mozeme ju vsak pre istotu zavolat ak je vstup nevalidny - horny okraj kotvy by bol pod urovnou betonu
            if(fPortionOtAnchorAbovePlate_abs_temp <= 0)
                SetPortionOtAnchorAbovePlateDefault();

            // Washer size
            // Plate washer
            if (washerPlateTop != null)
            {
                m_WasherPlateTop = washerPlateTop;

                // Urcime pozicie washer a nuts v LCS kotvy - LCS kotvy smeruje v smere x
                float fPlateThickness = 0.003f; // TODO - zavisi od hrubky plechu base plate - napojit 
                m_WasherPlateTop.ControlPoint.X = m_fPortionOtAnchorAbovePlate_abs - fPlateThickness;

                m_Nuts = new List<CNut>();

                CNut nut = new CNut(name_temp, nameMaterial_temp, new Point3D(0, 0, 0), 0, -90, 0);
                float fWasherTopPlateNutPosition = m_fPortionOtAnchorAbovePlate_abs - fPlateThickness - m_WasherPlateTop.Ft - fOffsetFor3D;
                nut.ControlPoint.X = fWasherTopPlateNutPosition;

                m_Nuts.Add(nut);
            }

            // Bearing washer
            if (washerBearing != null)
            {
                m_WasherBearing = washerBearing;

                if (m_Nuts == null)
                    m_Nuts = new List<CNut>();

                CNut nutTop = new CNut(name_temp, nameMaterial_temp, new Point3D(0, 0, 0), 0, -90, 0);
                CNut nutBottom = new CNut(name_temp, nameMaterial_temp, new Point3D(0, 0, 0), 0, -90, 0);

                // Urcime pozicie washer a nuts v LCS kotvy - LCS kotvy smeruje v smere x
                m_fWasherBearing_OffsetFromBottom = nutBottom.Thickness_max + 0.02f; // vyska matice + 20 mm
                m_WasherBearing.ControlPoint.X = m_fPortionOtAnchorAbovePlate_abs + (Length - m_fPortionOtAnchorAbovePlate_abs - m_fWasherBearing_OffsetFromBottom);

                float fWasherBearingTopNutPosition = (float)m_WasherBearing.ControlPoint.X - m_WasherBearing.Ft - fOffsetFor3D;
                float fWasherBearingBottomNutPosition = (float)m_WasherBearing.ControlPoint.X + nutBottom.Thickness_max + fOffsetFor3D;
                nutTop.ControlPoint.X = fWasherBearingTopNutPosition;
                nutBottom.ControlPoint.X = fWasherBearingBottomNutPosition;

                m_Nuts.Add(nutTop);
                m_Nuts.Add(nutBottom);
            }
        }

        //public float GetMass()
        //{
        //    return Area_p_pitch * Length * m_Mat.m_fRho;
        //}

        private void SetPortionOtAnchorAbovePlateDefault()
        {
            // TODO - Tuto vzdialenost mozeme urcovat rozne, ako parameter kolko ma byt kotva nad plechom / betonom alebo ako parameter dlzka kotvy - kolko ma byt kotevna dlzka (dlzka zabetonovanej casti kotvy)
            float fPortionOtAnchorAbovePlate_rel = 0.09f; // [-] // Suradnica konca kotvy nad plechom (maximum z 9% dlzky kotvy, 1.8x priemer kotvy alebo 20 mm)
            m_fPortionOtAnchorAbovePlate_abs = MathF.Max(fPortionOtAnchorAbovePlate_rel * Length, 1.8f * Diameter_shank, 0.02f); // [m]
        }

        public void UpdateControlPoint()
        {
            m_fPortionOtAnchorAbovePlate_abs = Length - m_fh_effective - (m_WasherBearing != null ? m_WasherBearing.Ft : 0) - m_fWasherBearing_OffsetFromBottom;

            this.ControlPoint.Z = m_fPortionOtAnchorAbovePlate_abs; // Globalny system

            if (WasherPlateTop != null)
            {
                // TODO Ondrej // Refaktorovat s konstruktorom
                // m_Nuts sa pregeneruje uplne, ak chces mozes sa s tym vyhrat, aby sa len updatovali pozicie control point CNut

                // Urcime pozicie washer a nuts v LCS kotvy - LCS kotvy smeruje v smere x
                float fPlateThickness = 0.003f; // TODO - zavisi od hrubky plechu base plate - napojit
                m_WasherPlateTop.ControlPoint.X = m_fPortionOtAnchorAbovePlate_abs - fPlateThickness;

                m_Nuts = new List<CNut>();

                CNut nut = new CNut(Name, m_Mat.Name, new Point3D(0, 0, 0), 0, -90, 0);
                float fWasherTopPlateNutPosition = m_fPortionOtAnchorAbovePlate_abs - fPlateThickness - m_WasherPlateTop.Ft - fOffsetFor3D;
                nut.ControlPoint.X = fWasherTopPlateNutPosition;

                m_Nuts.Add(nut);
            }

            if (WasherBearing != null)
            {
                // TODO Ondrej // Refaktorovat s konstruktorom
                // m_Nuts sa pregeneruje uplne, ak chces mozes sa s tym vyhrat, aby sa len updatovali pozicie control point CNut

                if (m_Nuts == null)
                    m_Nuts = new List<CNut>();

                CNut nutTop = new CNut(Name, m_Mat.Name, new Point3D(0, 0, 0), 0, -90, 0);
                CNut nutBottom = new CNut(Name, m_Mat.Name, new Point3D(0, 0, 0), 0, -90, 0);

                // Urcime pozicie washer a nuts v LCS kotvy - LCS kotvy smeruje v smere x
                m_fWasherBearing_OffsetFromBottom = nutBottom.Thickness_max + 0.02f; // vyska matice + 20 mm
                m_WasherBearing.ControlPoint.X = m_fPortionOtAnchorAbovePlate_abs + (Length - m_fPortionOtAnchorAbovePlate_abs - m_fWasherBearing_OffsetFromBottom);

                float fWasherBearingTopNutPosition = (float)m_WasherBearing.ControlPoint.X - m_WasherBearing.Ft - fOffsetFor3D;
                float fWasherBearingBottomNutPosition = (float)m_WasherBearing.ControlPoint.X + nutBottom.Thickness_max + fOffsetFor3D;
                nutTop.ControlPoint.X = fWasherBearingTopNutPosition;
                nutBottom.ControlPoint.X = fWasherBearingBottomNutPosition;

                m_Nuts.Add(nutTop);
                m_Nuts.Add(nutBottom);
            }
        }

        public void UpdateAnchorOnNameChanged()
        {
            CBoltProperties properties = CBoltsManager.GetBoltProperties(Name, "ThreadedBars");

            Diameter_shank = (float)properties.ShankDiameter;
            Diameter_thread = (float)properties.ThreadDiameter;
            Diameter_pitch = (float)properties.PitchDiameter;

            Diameter_hole = GetDiameter_Hole();

            Price_PPKG_NZD = (float)properties.Price_PPKG_NZD;
            Price_PPLM_NZD = (float)properties.Price_PPLM_NZD;
            Price_PPP_NZD = (float)properties.Price_PPLM_NZD * Length;
            Mass = (float)properties.Mass_kg_LM * Length;

            Area_c_thread = MathF.fPI * MathF.Pow2(Diameter_thread) / 4f; // Core / thread area
            Area_o_shank = MathF.fPI * MathF.Pow2(Diameter_shank) / 4f; // Shank area
            Area_p_pitch = MathF.fPI * MathF.Pow2(Diameter_pitch) / 4f; // Pitch diameter area

            if (m_fPortionOtAnchorAbovePlate_abs <= 0)
                SetPortionOtAnchorAbovePlateDefault();
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
