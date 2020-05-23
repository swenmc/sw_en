using _3DTools;
using BaseClasses.GraphObj;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_F_or_L : CPlate
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

        private float m_fbX2;

        public float Fb_X2
        {
            get
            {
                return m_fbX2;
            }

            set
            {
                m_fbX2 = value;
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

        private int m_iLeftRightIndex; // F - plate 0 - left, 1 - right

        public int LeftRightIndex // Toto potrebujeme ako public, inak sa pri klonovani nenastavi spravne a plate sa zle vykresluje
        {
            get
            {
                return m_iLeftRightIndex;
            }

            set
            {
                m_iLeftRightIndex = value;
            }
        }

        private float m_e_min_x_LeftLeg;

        public float e_min_x_LeftLeg
        {
            get
            {
                return m_e_min_x_LeftLeg;
            }

            set
            {
                m_e_min_x_LeftLeg = value;
            }
        }

        private float m_e_min_y_LeftLeg;

        public float e_min_y_LeftLeg
        {
            get
            {
                return m_e_min_y_LeftLeg;
            }

            set
            {
                m_e_min_y_LeftLeg = value;
            }
        }

        private float m_e_min_z_RightLeg;

        public float e_min_z_RightLeg
        {
            get
            {
                return m_e_min_z_RightLeg;
            }

            set
            {
                m_e_min_z_RightLeg = value;
            }
        }

        private float m_e_min_y_RightLeg;

        public float e_min_y_RightLeg
        {
            get
            {
                return m_e_min_y_RightLeg;
            }

            set
            {
                m_e_min_y_RightLeg = value;
            }
        }

        float m_fConnectedSectionDepth;

        public float ConnectedSectionDepth
        {
            get
            {
                return m_fConnectedSectionDepth;
            }

            set
            {
                m_fConnectedSectionDepth = value;
            }
        }

        public CConCom_Plate_F_or_L()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
        }

        // L with or without holes
        public CConCom_Plate_F_or_L(string sName_temp,
            Point3D controlpoint,
            float fbX_temp,
            float fhY_temp,
            float fl_Z_temp,
            float ft_platethickness,
            float fConnectedSectionDepth,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_L screwArrangement)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_L;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_iLeftRightIndex = 0; // Side index -0 - left(original)
            m_fbX1 = fbX_temp;
            m_fbX2 = m_fbX1; // L Series - without slope
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            Ft = ft_platethickness;
            m_fConnectedSectionDepth = fConnectedSectionDepth; // Vyska pripojeneho prierezu (zvycajne je to secondary member)
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement);
        }

        // F - with or without holes
        public CConCom_Plate_F_or_L(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fbX2_temp,
            float fhY_temp,
            float fl_Z_temp,
            float ft_platethickness,
            float fConnectedSectionDepth,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_F screwArrangement)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_F;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_iLeftRightIndex = sName_temp.Substring(5, 2) == "LH" ? 0 : 1; // Side index - 0 - left (original), 1 - right
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            Ft = ft_platethickness;
            m_fConnectedSectionDepth = fConnectedSectionDepth; // Vyska pripojeneho prierezu (zvycajne je to secondary member)
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement);
        }

        //----------------------------------------------------------------------------
        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            if (screwArrangement != null)
            {
                arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber];
            }

            /*
            if (m_ePlateSerieType_FS == ESerieTypePlate.eSerie_L)
            {
                m_fbX2 = m_fbX1;
            }*/

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement != null)
            {
                if (screwArrangement is CScrewArrangement_L)
                {
                    ((CScrewArrangement_L)screwArrangement).Calc_HolesCentersCoord2D(Fb_X1, Fh_Y, Fl_Z);
                }
                else //if (screwArrangement is CScrewArrangement_F)
                {
                    ((CScrewArrangement_F)screwArrangement).Calc_HolesCentersCoord2D(Fb_X1, Fb_X2, Fh_Y, Fl_Z);
                }

                Calc_HolesControlPointsCoord3D(screwArrangement);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();

            Set_BendLinesPoints2D();

            bool bChangeRotationAngle_MirroredPlate = false;

            if (m_iLeftRightIndex % 2 != 0) // Change x-coordinates for odd index (RH)
            {
                bChangeRotationAngle_MirroredPlate = true; // Change rotation angle (about vertical axis Y) of screws in the left leg

                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i].X *= -1;
                }

                if (screwArrangement != null)
                {
                    for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                    {
                        screwArrangement.HolesCentersPoints2D[i].X *= -1;
                        arrConnectorControlPoints3D[i].X *= -1;
                    }
                }

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].X *= -1;
                }
            }

            if (screwArrangement != null)
            {
                GenerateConnectors(screwArrangement, bChangeRotationAngle_MirroredPlate);
            }
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            if (m_ePlateSerieType_FS == ESerieTypePlate.eSerie_L)
                Width_bx = m_fbX1;
            else
                Width_bx = m_fbX2; //Math.Max(m_fbX1, m_fbX2);

            Height_hy = m_fhY;

            if (m_ePlateSerieType_FS == ESerieTypePlate.eSerie_L)
                Width_bx_Stretched = m_fbX1 + m_flZ; // Total width
            else
                Width_bx_Stretched = m_fbX2 + m_flZ; // Total width

            Height_hy_Stretched = m_fhY;

            //SetFlatedPlateDimensions();

            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY);

            int iNumberOfScrewsInSection; // TODO, temporary - zavisi na rozmiestneni skrutiek

            if (screwArrangement is CScrewArrangement_L)
                iNumberOfScrewsInSection = 4;
            else
                iNumberOfScrewsInSection = 8;

            fA_n = fA_g;
            if (screwArrangement != null)
            {
                fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fhY);

            fA_vn_zv = fA_v_zv;
            if (screwArrangement != null)
            {
                fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fhY);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_flZ;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X + m_fbX1;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = PointsOut2D[1].X + m_fbX2;
            PointsOut2D[3].Y = m_fhY;

            PointsOut2D[4].X = PointsOut2D[1].X;
            PointsOut2D[4].Y = m_fhY;

            PointsOut2D[5].X = PointsOut2D[0].X;
            PointsOut2D[5].Y = m_fhY;
        }

        public override void Calc_Coord3D()
        {
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX1;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = m_fbX2;
            arrPoints3D[3].Y = m_fhY;
            arrPoints3D[3].Z = arrPoints3D[2].Z;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[1].Z;

            arrPoints3D[5].X = arrPoints3D[0].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = arrPoints3D[0].X + Ft;
            arrPoints3D[6].Y = arrPoints3D[0].Y;
            arrPoints3D[6].Z = arrPoints3D[0].Z;

            arrPoints3D[7].X = arrPoints3D[1].X + Ft;
            arrPoints3D[7].Y = arrPoints3D[1].Y;
            arrPoints3D[7].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[8].X = arrPoints3D[2].X;
            arrPoints3D[8].Y = arrPoints3D[2].Y;
            arrPoints3D[8].Z = arrPoints3D[2].Z + Ft;

            arrPoints3D[9].X = arrPoints3D[3].X;
            arrPoints3D[9].Y = arrPoints3D[3].Y;
            arrPoints3D[9].Z = arrPoints3D[3].Z + Ft;

            arrPoints3D[10].X = arrPoints3D[7].X;
            arrPoints3D[10].Y = arrPoints3D[7].Y + m_fhY;
            arrPoints3D[10].Z = arrPoints3D[7].Z;

            arrPoints3D[11].X = arrPoints3D[6].X;
            arrPoints3D[11].Y = arrPoints3D[6].Y + m_fhY;
            arrPoints3D[11].Z = arrPoints3D[6].Z;
        }

        void Calc_HolesControlPointsCoord3D(CScrewArrangement screwArrangement)
        {
            m_e_min_x_LeftLeg = 0.010f; // Left leg
            m_e_min_z_RightLeg = 0.010f; // Right leg

            m_e_min_y_LeftLeg = m_e_min_y_RightLeg = 0.010f;

            float fy_edge2 = 0.030f;

            float fScrewOffset = screwArrangement.referenceScrew.T_ht_headTotalThickness;

            if (screwArrangement is CScrewArrangement_L)
            {
                float fy_edge3 = 0.120f;

                // TODO nahradit enumom a switchom

                if (screwArrangement.IHolesNumber == 16) // LH, LI, LK
                {
                    // Left Leg

                    arrConnectorControlPoints3D[0].X = Ft + fScrewOffset;
                    arrConnectorControlPoints3D[0].Y = m_e_min_y_LeftLeg;
                    arrConnectorControlPoints3D[0].Z = m_flZ - m_e_min_x_LeftLeg;

                    arrConnectorControlPoints3D[1].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y;
                    arrConnectorControlPoints3D[1].Z = m_e_min_x_LeftLeg;

                    arrConnectorControlPoints3D[2].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[2].Y = fy_edge2;
                    arrConnectorControlPoints3D[2].Z = 0.5f * m_flZ;

                    arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[3].Y = fy_edge3;
                    arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[2].Z;

                    arrConnectorControlPoints3D[4].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[4].Y = m_fhY - fy_edge3;
                    arrConnectorControlPoints3D[4].Z = arrConnectorControlPoints3D[2].Z;

                    arrConnectorControlPoints3D[5].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[5].Y = m_fhY - fy_edge2;
                    arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[2].Z;

                    arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[6].Y = m_fhY - m_e_min_y_LeftLeg;
                    arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[0].Z;

                    arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[7].Y = m_fhY - m_e_min_y_LeftLeg;
                    arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[1].Z;

                    // Right Leg

                    arrConnectorControlPoints3D[8].X = m_e_min_x_LeftLeg;
                    arrConnectorControlPoints3D[8].Y = m_fhY - m_e_min_y_RightLeg;
                    arrConnectorControlPoints3D[8].Z = Ft + fScrewOffset;  // TODO Position depends on screw length

                    arrConnectorControlPoints3D[9].X = m_fbX1 - m_e_min_x_LeftLeg;
                    arrConnectorControlPoints3D[9].Y = arrConnectorControlPoints3D[8].Y;
                    arrConnectorControlPoints3D[9].Z = arrConnectorControlPoints3D[8].Z;

                    arrConnectorControlPoints3D[10].X = 0.5f * m_fbX1;
                    arrConnectorControlPoints3D[10].Y = m_fhY - fy_edge2;
                    arrConnectorControlPoints3D[10].Z = arrConnectorControlPoints3D[8].Z;

                    arrConnectorControlPoints3D[11].X = arrConnectorControlPoints3D[10].X;
                    arrConnectorControlPoints3D[11].Y = m_fhY - fy_edge3;
                    arrConnectorControlPoints3D[11].Z = arrConnectorControlPoints3D[8].Z;

                    arrConnectorControlPoints3D[12].X = arrConnectorControlPoints3D[10].X;
                    arrConnectorControlPoints3D[12].Y = fy_edge3;
                    arrConnectorControlPoints3D[12].Z = arrConnectorControlPoints3D[8].Z;

                    arrConnectorControlPoints3D[13].X = arrConnectorControlPoints3D[10].X;
                    arrConnectorControlPoints3D[13].Y = fy_edge2;
                    arrConnectorControlPoints3D[13].Z = arrConnectorControlPoints3D[8].Z;

                    arrConnectorControlPoints3D[14].X = arrConnectorControlPoints3D[8].X;
                    arrConnectorControlPoints3D[14].Y = m_e_min_y_RightLeg;
                    arrConnectorControlPoints3D[14].Z = arrConnectorControlPoints3D[8].Z;

                    arrConnectorControlPoints3D[15].X = arrConnectorControlPoints3D[9].X;
                    arrConnectorControlPoints3D[15].Y = m_e_min_y_RightLeg;
                    arrConnectorControlPoints3D[15].Z = arrConnectorControlPoints3D[8].Z;
                }
                else if (screwArrangement.IHolesNumber == 8) // LJ
                {
                    // Left Leg

                    arrConnectorControlPoints3D[0].X = Ft + fScrewOffset; // TODO Position depends on screw length
                    arrConnectorControlPoints3D[0].Y = m_e_min_y_LeftLeg;
                    arrConnectorControlPoints3D[0].Z = m_flZ - m_e_min_x_LeftLeg;

                    arrConnectorControlPoints3D[1].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y;
                    arrConnectorControlPoints3D[1].Z = m_e_min_x_LeftLeg;

                    arrConnectorControlPoints3D[2].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[2].Y = m_fhY - m_e_min_y_LeftLeg;
                    arrConnectorControlPoints3D[2].Z = arrConnectorControlPoints3D[0].Z;

                    arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[0].X;
                    arrConnectorControlPoints3D[3].Y = arrConnectorControlPoints3D[2].Y;
                    arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[1].Z;

                    // Right Leg

                    arrConnectorControlPoints3D[4].X = m_e_min_x_LeftLeg;
                    arrConnectorControlPoints3D[4].Y = m_fhY - m_e_min_y_RightLeg;
                    arrConnectorControlPoints3D[4].Z = Ft + fScrewOffset; // TODO Position depends on screw length

                    arrConnectorControlPoints3D[5].X = m_fbX1 - m_e_min_x_LeftLeg;
                    arrConnectorControlPoints3D[5].Y = arrConnectorControlPoints3D[4].Y;
                    arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[4].Z;

                    arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[4].X;
                    arrConnectorControlPoints3D[6].Y = m_e_min_y_RightLeg;
                    arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[4].Z;

                    arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[5].X;
                    arrConnectorControlPoints3D[7].Y = arrConnectorControlPoints3D[6].Y;
                    arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[4].Z;
                }
                else
                {
                    // Not defined expected number of holes for L or F plate
                }
            }
            else
            {
                m_e_min_x_LeftLeg = m_flZ * 0.5f; // Middle of left leg

                float fx_edge21 = (Fb_X2 - 2 * m_e_min_z_RightLeg) / 3f; // Todo - v rade su 4 skrutky a 3 medzery, bolo by super urobit to dynamicke
                float fb_temp_rightBottomRow = Fb_X1 + ((Fb_X2 - Fb_X1) / Fh_Y * (Fh_Y - m_fConnectedSectionDepth + m_e_min_y_RightLeg));
                float fx_edge22 = (fb_temp_rightBottomRow - 2 * m_e_min_z_RightLeg) / 1f; // Todo - v rade su 2 skrutky a 1 medzera, bolo by super urobit to dynamicke

                // Left Leg
                // Bottom
                arrConnectorControlPoints3D[0].X = Ft + fScrewOffset; // TODO Position depends on screw length
                arrConnectorControlPoints3D[0].Y = m_e_min_y_LeftLeg;
                arrConnectorControlPoints3D[0].Z = m_flZ - m_e_min_x_LeftLeg;

                arrConnectorControlPoints3D[1].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y + fy_edge2;
                arrConnectorControlPoints3D[1].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[2].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[2].Y = arrConnectorControlPoints3D[1].Y + fy_edge2;
                arrConnectorControlPoints3D[2].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[3].Y = arrConnectorControlPoints3D[2].Y + fy_edge2;
                arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[0].Z;

                // Upper
                arrConnectorControlPoints3D[4].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[4].Y = m_fhY - m_e_min_y_LeftLeg;
                arrConnectorControlPoints3D[4].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[5].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[5].Y = arrConnectorControlPoints3D[4].Y - fy_edge2;
                arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[6].Y = arrConnectorControlPoints3D[5].Y - fy_edge2;
                arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[7].Y = arrConnectorControlPoints3D[6].Y - fy_edge2;
                arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[0].Z;

                // Right Leg
                // Bottom row
                arrConnectorControlPoints3D[8].X = m_e_min_z_RightLeg;
                arrConnectorControlPoints3D[8].Y = m_fhY - m_fConnectedSectionDepth  + m_e_min_y_RightLeg;
                arrConnectorControlPoints3D[8].Z = Ft + fScrewOffset; // TODO Position depends on screw length

                arrConnectorControlPoints3D[9].X = arrConnectorControlPoints3D[8].X + fx_edge22;
                arrConnectorControlPoints3D[9].Y = arrConnectorControlPoints3D[8].Y;
                arrConnectorControlPoints3D[9].Z = arrConnectorControlPoints3D[8].Z;

                // Upper row
                arrConnectorControlPoints3D[10].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[10].Y = m_fhY - m_e_min_y_RightLeg;
                arrConnectorControlPoints3D[10].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[11].X = arrConnectorControlPoints3D[10].X + fx_edge21;
                arrConnectorControlPoints3D[11].Y = arrConnectorControlPoints3D[10].Y;
                arrConnectorControlPoints3D[11].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[12].X = arrConnectorControlPoints3D[11].X + fx_edge21;
                arrConnectorControlPoints3D[12].Y = arrConnectorControlPoints3D[10].Y;
                arrConnectorControlPoints3D[12].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[13].X = arrConnectorControlPoints3D[12].X + fx_edge21;
                arrConnectorControlPoints3D[13].Y = arrConnectorControlPoints3D[10].Y;
                arrConnectorControlPoints3D[13].Z = arrConnectorControlPoints3D[8].Z;
            }
        }

        void GenerateConnectors(CScrewArrangement screwArrangement, bool bChangeRotationAngle_MirroredPlate)
        {
            if (screwArrangement.IHolesNumber > 0)
            {
                screwArrangement.Screws = new CScrew[screwArrangement.IHolesNumber];

                // TODO Ondrej 15/07/2018
                // Tu sa pridava sktrutka do plechu, vklada sa do pozicie na plechu v suradnicovom systeme plechu (controlpoint) a otoci sa do pozicie v LCS plechu
                // Potom je potrebne pri posune a otoceni plechu otocit aj vsetky skrutky, ktore k nemu patria
                // Povodne suradnice skrutky su ako suradnice valca kde x je v smere vysky valca

                // Update 1
                // Po tomto vlozeni skrutiek do plechu by sa mali suradnice skrutiek prepocitat z povodnych v ktorych su zadane do suradnicoveho systemu plechu a ulozit

                int iNumberOfScrewsInLeftLeg = screwArrangement.IHolesNumber / 2;

                float fRotationAngleAboutYAxis = 180; // Vertical Axis

                if (bChangeRotationAngle_MirroredPlate)
                    fRotationAngleAboutYAxis = 0;

                if (screwArrangement is CScrewArrangement_F)
                    iNumberOfScrewsInLeftLeg = 8; // TODO - umoznit nastavovat dynamicky

                for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                {
                    if (i < iNumberOfScrewsInLeftLeg) // Left Leg
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, fRotationAngleAboutYAxis, 0);
                    }
                    else
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, 90, 0);
                    }
                }
            }
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 1;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[1], PointsOut2D[4]);
        }

        protected override void loadIndices()
        {
            int secNum = 6;
            TriangleIndices = new Int32Collection();

            if (m_iLeftRightIndex == 0) // Left
            {
                AddRectangleIndices_CCW_1234(TriangleIndices, 0, 5, 4, 1);
                AddRectangleIndices_CCW_1234(TriangleIndices, 1, 4, 3, 2);
                AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 10, 11);
                AddRectangleIndices_CCW_1234(TriangleIndices, 7, 8, 9, 10);

                // Shell Surface
                DrawCaraLaterals_CW(secNum, TriangleIndices);
            }
            else if(m_iLeftRightIndex == 1) // Right
            {
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 5, 4, 1);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 4, 3, 2);
                AddRectangleIndices_CW_1234(TriangleIndices, 6, 7, 10, 11);
                AddRectangleIndices_CW_1234(TriangleIndices, 7, 8, 9, 10);

                // Shell Surface
                DrawCaraLaterals_CCW(secNum, TriangleIndices);
            }
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            // y = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[0]);

            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[8]);

            // y = m_fhY
            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[11]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[4]);

            return wireFrame;
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_F_or_L)
            {
                CConCom_Plate_F_or_L refPlate = (CConCom_Plate_F_or_L)plate;
                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fhY = refPlate.m_fhY;
                this.m_flZ = refPlate.m_flZ;
                this.m_iLeftRightIndex = refPlate.m_iLeftRightIndex;
                this.m_e_min_x_LeftLeg = refPlate.m_e_min_x_LeftLeg;
                this.m_e_min_y_LeftLeg = refPlate.m_e_min_y_LeftLeg;
                this.m_e_min_z_RightLeg = refPlate.m_e_min_z_RightLeg;
                this.m_e_min_y_RightLeg = refPlate.m_e_min_y_RightLeg;
                this.m_fConnectedSectionDepth = refPlate.m_fConnectedSectionDepth;
            }
        }
    }
}
