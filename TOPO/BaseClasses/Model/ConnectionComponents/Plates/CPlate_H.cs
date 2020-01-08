using _3DTools;
using BaseClasses.GraphObj;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_H : CPlate
    {
        private float m_fbX1;

        public float Fb_X1
        {
            get
            {
                return m_fbX1;
            }

            set
            {
                m_fbX1 = value;
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

        private int iLeftRightIndex; // plate 0 - left, 1 - right 

        public CConCom_Plate_H()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_H(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fhY_temp,
            float fl_Z_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_H screwArrangement_temp,  // TODO
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_H;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 7;
            ITotNoPointsin3D = 14;

            m_pControlPoint = controlpoint;
            iLeftRightIndex = sName_temp.Substring(4, 2) == "LH" ? 0 : 1; // Side index - 0 - left (original), 1 - right
            m_fbX1 = fbX1_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            arrConnectorControlPoints3D = new Point3D[screwArrangement_temp.IHolesNumber];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            screwArrangement_temp.Calc_HolesCentersCoord2D(Fb_X1, Fh_Y, Fl_Z);
            Calc_HolesControlPointsCoord3D(screwArrangement_temp);

            // Fill list of indices for drawing of surface
            loadIndices();

            bool bChangeRotationAngle_MirroredPlate = false;

            if (iLeftRightIndex % 2 != 0) // Change x-coordinates for odd index (RH)
            {
                bChangeRotationAngle_MirroredPlate = true; // Change rotation angle (about vertical axis Y) of screws in the left leg

                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i].X *= -1;
                }

                if (screwArrangement_temp != null)
                {
                    for (int i = 0; i < screwArrangement_temp.IHolesNumber; i++)
                    {
                        screwArrangement_temp.HolesCentersPoints2D[i].X *= -1;
                        arrConnectorControlPoints3D[i].X *= -1;
                    }
                }

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].X *= -1;
                }
            }

            GenerateConnectors(screwArrangement_temp, bChangeRotationAngle_MirroredPlate);

            Width_bx = m_fbX1 + m_flZ;
            Height_hy = m_fhY;
            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY);
            int iNumberOfScrewsInSection = 8; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            fA_v_zv = Get_A_rect(Ft, m_fhY);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            fI_yu = Get_I_yu_rect(Ft, m_fhY);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY); // Elastic section modulus

            ScrewArrangement = screwArrangement_temp;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;
        }

        public override void Calc_Coord3D()
        {
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;
        }

        void Calc_HolesControlPointsCoord3D(CScrewArrangement screwArrangement)
        {
        }

        void GenerateConnectors(CScrewArrangement screwArrangement, bool bChangeRotationAngle_MirroredPlate)
        {
        }

        protected override void loadIndices()
        {
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();


            return wireFrame;
        }
    }
}
