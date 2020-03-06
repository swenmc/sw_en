using _3DTools;
using System;
using System.Linq;
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

        // Base Plate to Edge Distances

        float m_fx_minus_plateEdge_to_pad;

        public float x_minus_plateEdge_to_pad
        {
            get
            {
                return m_fx_minus_plateEdge_to_pad;
            }

            set
            {
                m_fx_minus_plateEdge_to_pad = value;
            }
        }

        float m_fy_minus_plateEdge_to_pad;

        public float y_minus_plateEdge_to_pad
        {
            get
            {
                return m_fy_minus_plateEdge_to_pad;
            }

            set
            {
                m_fy_minus_plateEdge_to_pad = value;
            }
        }

        float m_fx_plus_plateEdge_to_pad;

        public float x_plus_plateEdge_to_pad
        {
            get
            {
                return m_fx_plus_plateEdge_to_pad;
            }

            set
            {
                m_fx_plus_plateEdge_to_pad = value;
            }
        }

        float m_fy_plus_plateEdge_to_pad;

        public float y_plus_plateEdge_to_pad
        {
            get
            {
                return m_fy_plus_plateEdge_to_pad;
            }

            set
            {
                m_fy_plus_plateEdge_to_pad = value;
            }
        }

        private float m_fe_min_y; // Minimalna vzdialenost skrutiek - smer y

        public float e_min_y
        {
            get
            {
                return m_fe_min_y;
            }

            set
            {
                m_fe_min_y = value;
            }
        }

        private float m_fe_min_z; // Minimalna vzdialenost skrutiek - smer x

        public float e_min_z
        {
            get
            {
                return m_fe_min_z;
            }

            set
            {
                m_fe_min_z = value;
            }
        }

        public CConCom_Plate_B_basic()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_B;
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
            CScrewArrangement screwArrangement_temp)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_B;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            bool uniformDistributionOfShear = false; // TODO - Todo by malo prist z nastavenia design options
            AnchorArrangement = new CAnchorArrangement_BB_BG(Name, referenceAnchor_temp, uniformDistributionOfShear);

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

            UpdatePlateData(screwArrangement_temp/*, AnchorArrangement*/);
        }

        public override void UpdatePlateData(CScrewArrangement screwArrangement/*, CAnchorArrangement AnchorArrangmement*/)
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

        public void UpdatePlateData(CAnchorArrangement_BB_BG anchorArrangement)
        {
            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            if (ScrewArrangement != null)
            {
                arrConnectorControlPoints3D = new Point3D[ScrewArrangement.IHolesNumber];
            }

            // Calculate point positions
            Calc_Coord2D();

            // Calculate parameters of arrangement depending on plate geometry
            if (anchorArrangement != null)
            {
                anchorArrangement.Calc_BasePlateData(Fb_X, m_flZ, Fh_Y, Ft);
                AnchorArrangement = anchorArrangement;
            }

            Calc_Coord3D(); // Tato funckia potrebuje aby boli inicializovany objekt AnchorArrangement - vykresluju sa podla toho otvory v plechu (vypocet suradnic dier)

            if (ScrewArrangement != null)
            {
                ScrewArrangement.Calc_BasePlateData(Fb_X, m_flZ, Fh_Y, Ft);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(ScrewArrangement);
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = Fb_X;
            Height_hy = Fh_Y;
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = Fb_X + 2 * m_flZ; // Total width
            Height_hy_Stretched = Fh_Y;
            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

            fA_g = Get_A_rect(2 * Ft, Fh_Y);
            int iNumberOfScrewsInSection = 6; // Jedna strana plechu TODO, temporary - zavisi na rozmiestneni skrutiek

            if (screwArrangement is CScrewArrangement_BX)
            {
                CScrewSequenceGroup gr = screwArrangement.ListOfSequenceGroups.FirstOrDefault();
                if (gr != null)
                {
                    iNumberOfScrewsInSection = 0;
                    foreach (CConnectorSequence sc in gr.ListSequence)
                    {
                        iNumberOfScrewsInSection += ((CScrewRectSequence)sc).NumberOfScrewsInColumn_yDirection;
                    }
                }
            }
            //if (screwArrangement is CScrewArrangement_BX_1)
            //    iNumberOfScrewsInSection = ((CScrewArrangement_BX_1)screwArrangement).RectSequences[0].NumberOfScrewsInColumn_yDirection + ((CScrewArrangement_BX_1)screwArrangement).RectSequences[1].NumberOfScrewsInColumn_yDirection;

            //if (screwArrangement is CScrewArrangement_BX_2)
            //    iNumberOfScrewsInSection = ((CScrewArrangement_BX_2)screwArrangement).RectSequences[0].NumberOfScrewsInColumn_yDirection + ((CScrewArrangement_BX_2)screwArrangement).RectSequences[1].NumberOfScrewsInColumn_yDirection + ((CScrewArrangement_BX_2)screwArrangement).RectSequences[2].NumberOfScrewsInColumn_yDirection;

            fA_n = fA_g;

            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * 2 * Ft;
            }

            fA_v_zv = Get_A_rect(2 * Ft, Fh_Y);

            fA_vn_zv = fA_v_zv;

            if (screwArrangement != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * 2 * Ft;
            }

            fI_yu = 2 * Get_I_yu_rect(Ft, Fh_Y); // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, Fh_Y); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        public void SetMinimumScrewToEdgeDistances(CScrewArrangement screwArrangement)
        {
            e_min_y = 0;
            e_min_z = 0;

            if (screwArrangement != null && screwArrangement.HolesCentersPoints2D != null && screwArrangement.HolesCentersPoints2D.Length > 0)
            {
                // Minimum edge distances - zadane v suradnicovom smere plechu
                e_min_y = (float)screwArrangement.HolesCentersPoints2D[0].Y;
                e_min_z = (float)screwArrangement.HolesCentersPoints2D[0].X;
            }
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic refPlate = (CConCom_Plate_B_basic)plate;
                this.m_flZ = refPlate.m_flZ;
                this.AnchorArrangement = refPlate.AnchorArrangement;
                this.m_fx_minus_plateEdge_to_pad = refPlate.m_fx_minus_plateEdge_to_pad;
                this.m_fy_minus_plateEdge_to_pad = refPlate.m_fy_minus_plateEdge_to_pad;
                this.m_fx_plus_plateEdge_to_pad = refPlate.m_fx_plus_plateEdge_to_pad;
                this.m_fy_plus_plateEdge_to_pad = refPlate.m_fy_plus_plateEdge_to_pad;
                this.m_fe_min_y = refPlate.m_fe_min_y;
                this.m_fe_min_z = refPlate.m_fe_min_z;
            }
        }
    }
}
