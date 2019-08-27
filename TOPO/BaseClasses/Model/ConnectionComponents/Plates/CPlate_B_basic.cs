using _3DTools;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_B_basic : CConCom_Plate_Q_T_Y
    {
        /*
        private float m_fbX;

        public float Fb_X
        {
            get
            {
                return m_fbX;
            }

            set
            {
                m_fbX = value;
            }
        }

        private float m_fhY;

        public float Fh_Y
        {
            get
            {
                return m_fhY;
            }

            set
            {
                m_fhY = value;
            }
        }
        */

        private float m_flZ; // Not used in 2D model

        public float Fl_Z
        {
            get
            {
                return m_flZ;
            }

            set
            {
                m_flZ = value;
            }
        }

        private CAnchorArrangement_BB_BG m_anchorArrangement;

        public CAnchorArrangement_BB_BG AnchorArrangement
        {
            get
            {
                return m_anchorArrangement;
            }

            set
            {
                m_anchorArrangement = value;
            }
        }

        public CConCom_Plate_B_basic()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_B_basic(string sName_temp,
            Point3D controlpoint,
            float fbX_temp,
            float fhY_temp,
            float fl_Z_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CAnchor referenceAnchor_temp,
            CScrewArrangement screwArrangement_temp,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_B;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            AnchorArrangement = new CAnchorArrangement_BB_BG(Name, referenceAnchor_temp);

            m_pControlPoint = controlpoint;
            Fb_X = fbX_temp;
            Fh_Y = fhY_temp;
            m_flZ = fl_Z_temp;
            m_flZ2 = m_flZ1 = m_flZ; // Same
            Ft = ft_platethickness;
            m_iHolesNumber = 0; // Nepodporujeme otvory

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement_temp);
        }

        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            if (screwArrangement != null)
            {
                arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber];
            }

            // Calculate point positions
            Calc_Coord2D();

            // Calculate parameters of arrangement depending on plate geometry
            if (AnchorArrangement != null)
            {
                AnchorArrangement.Calc_BasePlateData(Fb_X, m_flZ, Fh_Y, Ft);
            }

            Calc_Coord3D(); // Tato funckia potrebuje aby boli inicializovany objekt AnchorArrangement - vykresluju sa podla toho otvory v plechu (vypocet suradnic dier)

            if (screwArrangement != null)
            {
                screwArrangement.Calc_BasePlateData(Fb_X, m_flZ, Fh_Y, Ft);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = Fb_X + 2 * m_flZ; // Total width
            Height_hy = Fh_Y;
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(2 * Ft, Fh_Y);
            int iNumberOfScrewsInSection = 6; // Jedna strana plechu TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;

            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * 2 * Ft;
            }

            fA_v_zv = Get_A_rect(2 * Ft, Fh_Y);

            fA_vn_zv = fA_v_zv;

            if (screwArrangement != null)
            {
                fA_v_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * 2 * Ft;
            }

            fI_yu = 2 * Get_I_yu_rect(Ft, Fh_Y); // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, Fh_Y); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }
    }
}
