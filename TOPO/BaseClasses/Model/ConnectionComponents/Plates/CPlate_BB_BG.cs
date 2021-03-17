using _3DTools;
using MATH;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_BB_BG : CConCom_Plate_B_basic
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
        }*/

        int iNoPoints2Dfor3D; // Pomocny bod

        /*
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
        */

        public CConCom_Plate_BB_BG()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_B;
        }

        public CConCom_Plate_BB_BG(string sName_temp,
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

            ControlPoint = controlpoint;
            Fb_X = fbX_temp;
            Fh_Y = fhY_temp;
            Fl_Z = fl_Z_temp;
            Ft = ft_platethickness;

            ITotNoPointsin2D = 8;

            bool uniformDistributionOfShear = false; // Todo by malo prist z nastaveni Design Options

            AnchorArrangement = new CAnchorArrangement_BB_BG(Name, referenceAnchor_temp, uniformDistributionOfShear);

            iNoPoints2Dfor3D = ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + AnchorArrangement.IHolesNumber * INumberOfPointsOfHole;
            ITotNoPointsin3D = 2 * iNoPoints2Dfor3D;

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
                AnchorArrangement.Calc_BasePlateData(Fb_X, Fl_Z, Fh_Y, Ft);
            }

            Calc_Coord3D(); // Tato funckia potrebuje aby boli inicializovany objekt AnchorArrangement - vykresluju sa podla toho otvory v plechu (vypocet suradnic dier)

            if (screwArrangement != null)
            {
                screwArrangement.Calc_BasePlateData(Fb_X, Fl_Z, Fh_Y, Ft);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            //Set_DimensionPoints2D();

            //Set_MemberOutlinePoints2D();

            Set_BendLinesPoints2D();
        }

        public void UpdatePlateData_Basic( CScrewArrangement screwArrangement)
        {
            Width_bx = Fb_X;
            Height_hy = Fh_Y;
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = Fb_X + 2 * Fl_Z; // Total width
            Height_hy_Stretched = Fh_Y;
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Minimum edge distances - zadane v suradnicovom smere plechu
            SetMinimumScrewToEdgeDistances(screwArrangement);

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
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * 2 * Ft;
            }

            fI_yu = 2 * Get_I_yu_rect(Ft, Fh_Y); // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, Fh_Y); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = Fl_Z;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X + Fb_X;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = PointsOut2D[2].X + Fl_Z;
            PointsOut2D[3].Y = 0;

            PointsOut2D[4].X = PointsOut2D[3].X;
            PointsOut2D[4].Y = Fh_Y;

            PointsOut2D[5].X = PointsOut2D[2].X;
            PointsOut2D[5].Y = Fh_Y;

            PointsOut2D[6].X = PointsOut2D[1].X;
            PointsOut2D[6].Y = Fh_Y;

            PointsOut2D[7].X = PointsOut2D[0].X;
            PointsOut2D[7].Y = Fh_Y;
        }

        public override void Calc_Coord3D()
        {
            // First layer

            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = Fl_Z;

            arrPoints3D[1].X = arrPoints3D[0].X;
            arrPoints3D[1].Y = arrPoints3D[0].Y;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = Fb_X + 2 * Ft;
            arrPoints3D[2].Y = arrPoints3D[0].Y;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[0].Y;
            arrPoints3D[3].Z = arrPoints3D[0].Z;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = Fh_Y;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[1].X;
            arrPoints3D[6].Y = arrPoints3D[4].Y;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = arrPoints3D[4].Y;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            // Points in holes square edges

            arrPoints3D[8].X = AnchorArrangement.holesCentersPointsfor3D[0].X - AnchorArrangement.HoleRadius;
            arrPoints3D[8].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y + AnchorArrangement.HoleRadius;;
            arrPoints3D[8].Z = 0;

            arrPoints3D[9].X = AnchorArrangement.holesCentersPointsfor3D[0].X - AnchorArrangement.HoleRadius;
            arrPoints3D[9].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[9].Z = 0;

            arrPoints3D[10].X = AnchorArrangement.holesCentersPointsfor3D[0].X + AnchorArrangement.HoleRadius;
            arrPoints3D[10].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[10].Z = 0;

            arrPoints3D[11].X = AnchorArrangement.holesCentersPointsfor3D[0].X + AnchorArrangement.HoleRadius;
            arrPoints3D[11].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[11].Z = 0;

            arrPoints3D[12].X = AnchorArrangement.holesCentersPointsfor3D[1].X - AnchorArrangement.HoleRadius;
            arrPoints3D[12].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[12].Z = 0;

            arrPoints3D[13].X = AnchorArrangement.holesCentersPointsfor3D[1].X - AnchorArrangement.HoleRadius;
            arrPoints3D[13].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[13].Z = 0;

            arrPoints3D[14].X = AnchorArrangement.holesCentersPointsfor3D[1].X + AnchorArrangement.HoleRadius;
            arrPoints3D[14].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[14].Z = 0;

            arrPoints3D[15].X = AnchorArrangement.holesCentersPointsfor3D[1].X + AnchorArrangement.HoleRadius;
            arrPoints3D[15].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[15].Z = 0;

            // Hole 1 - bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i].X = AnchorArrangement.holesCentersPointsfor3D[0].X + Geom2D.GetPositionX_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y + Geom2D.GetPositionY_CCW_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i].Z = 0;
            }

            // Hole 2 - upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i].X = AnchorArrangement.holesCentersPointsfor3D[1].X + Geom2D.GetPositionX_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y + Geom2D.GetPositionY_CCW_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i].Z = 0;
            }

            // Second layer

            arrPoints3D[iNoPoints2Dfor3D + 0].X = Ft;
            arrPoints3D[iNoPoints2Dfor3D + 0].Y = 0;
            arrPoints3D[iNoPoints2Dfor3D + 0].Z = Fl_Z;

            arrPoints3D[iNoPoints2Dfor3D + 1].X = arrPoints3D[iNoPoints2Dfor3D + 0].X;
            arrPoints3D[iNoPoints2Dfor3D + 1].Y = arrPoints3D[0].Y;
            arrPoints3D[iNoPoints2Dfor3D + 1].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 2].X = Fb_X + Ft;
            arrPoints3D[iNoPoints2Dfor3D + 2].Y = arrPoints3D[iNoPoints2Dfor3D + 0].Y;
            arrPoints3D[iNoPoints2Dfor3D + 2].Z = arrPoints3D[iNoPoints2Dfor3D + 1].Z;

            arrPoints3D[iNoPoints2Dfor3D + 3].X = arrPoints3D[iNoPoints2Dfor3D + 2].X;
            arrPoints3D[iNoPoints2Dfor3D + 3].Y = arrPoints3D[iNoPoints2Dfor3D + 0].Y;
            arrPoints3D[iNoPoints2Dfor3D + 3].Z = arrPoints3D[iNoPoints2Dfor3D + 0].Z;

            arrPoints3D[iNoPoints2Dfor3D + 4].X = arrPoints3D[iNoPoints2Dfor3D + 3].X;
            arrPoints3D[iNoPoints2Dfor3D + 4].Y = Fh_Y;
            arrPoints3D[iNoPoints2Dfor3D + 4].Z = arrPoints3D[iNoPoints2Dfor3D + 3].Z;

            arrPoints3D[iNoPoints2Dfor3D + 5].X = arrPoints3D[iNoPoints2Dfor3D + 2].X;
            arrPoints3D[iNoPoints2Dfor3D + 5].Y = arrPoints3D[iNoPoints2Dfor3D + 4].Y;
            arrPoints3D[iNoPoints2Dfor3D + 5].Z = arrPoints3D[iNoPoints2Dfor3D + 2].Z;

            arrPoints3D[iNoPoints2Dfor3D + 6].X = arrPoints3D[iNoPoints2Dfor3D + 1].X;
            arrPoints3D[iNoPoints2Dfor3D + 6].Y = arrPoints3D[iNoPoints2Dfor3D + 4].Y;
            arrPoints3D[iNoPoints2Dfor3D + 6].Z = arrPoints3D[iNoPoints2Dfor3D + 1].Z;

            arrPoints3D[iNoPoints2Dfor3D + 7].X = arrPoints3D[iNoPoints2Dfor3D + 0].X;
            arrPoints3D[iNoPoints2Dfor3D + 7].Y = arrPoints3D[iNoPoints2Dfor3D + 4].Y;
            arrPoints3D[iNoPoints2Dfor3D + 7].Z = arrPoints3D[iNoPoints2Dfor3D + 0].Z;

            // Points in holes square edges

            arrPoints3D[iNoPoints2Dfor3D + 8].X = AnchorArrangement.holesCentersPointsfor3D[0].X - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 8].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 8].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 9].X = AnchorArrangement.holesCentersPointsfor3D[0].X - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 9].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 9].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 10].X = AnchorArrangement.holesCentersPointsfor3D[0].X + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 10].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 10].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 11].X = AnchorArrangement.holesCentersPointsfor3D[0].X + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 11].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 11].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 12].X = AnchorArrangement.holesCentersPointsfor3D[1].X - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 12].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 12].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 13].X = AnchorArrangement.holesCentersPointsfor3D[1].X - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 13].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 13].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 14].X = AnchorArrangement.holesCentersPointsfor3D[1].X + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 14].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y - AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 14].Z = Ft;

            arrPoints3D[iNoPoints2Dfor3D + 15].X = AnchorArrangement.holesCentersPointsfor3D[1].X + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 15].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y + AnchorArrangement.HoleRadius;
            arrPoints3D[iNoPoints2Dfor3D + 15].Z = Ft;

            // Holes 1 - bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i].X = AnchorArrangement.holesCentersPointsfor3D[0].X + Geom2D.GetPositionX_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i].Y = AnchorArrangement.holesCentersPointsfor3D[0].Y + Geom2D.GetPositionY_CCW_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i].Z = Ft;
            }

            // Hole 2 - upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i].X = AnchorArrangement.holesCentersPointsfor3D[1].X + Geom2D.GetPositionX_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i].Y = AnchorArrangement.holesCentersPointsfor3D[1].Y + Geom2D.GetPositionY_CCW_deg(AnchorArrangement.HoleRadius, 90 + i * AnchorArrangement.RadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i].Z = Ft;
            }
        }

        protected override void loadIndices()
        {
            int iNumber_of_quaters = 4;
            int iNumber_of_hole_points = 12;
            int iNoTrianglesInquater = iNumber_of_hole_points / iNumber_of_quaters;

            TriangleIndices = new Int32Collection();

            // First layer
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 7, 6, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 5, 4, 3);

            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 6, 12, 9);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 10, 15, 5);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 9, 10, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 15, 12, 6);

            // Middle
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 13, 14, 11);

            // Hole 1 - bottom
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i + 1) * (j + 1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, i + ITotNoPointsin2D, ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, i + ITotNoPointsin2D, ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4);
                    }
                }
            }

            // Hole 2 - upper
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i+1) * (j+1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, 4 + i + ITotNoPointsin2D, INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, 4 + i + ITotNoPointsin2D, INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4);
                    }
                }
            }

            // Second layer
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 0, iNoPoints2Dfor3D + 7, iNoPoints2Dfor3D + 6, iNoPoints2Dfor3D + 1);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 2, iNoPoints2Dfor3D + 5, iNoPoints2Dfor3D + 4, iNoPoints2Dfor3D + 3);

            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D +1, iNoPoints2Dfor3D + 6, iNoPoints2Dfor3D + 12, iNoPoints2Dfor3D + 9);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D +2, iNoPoints2Dfor3D + 10, iNoPoints2Dfor3D + 15, iNoPoints2Dfor3D + 5);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D +1, iNoPoints2Dfor3D + 9, iNoPoints2Dfor3D + 10, iNoPoints2Dfor3D + 2);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 5, iNoPoints2Dfor3D + 15, iNoPoints2Dfor3D + 12, iNoPoints2Dfor3D + 6);

            // Middle
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 8, iNoPoints2Dfor3D + 13, iNoPoints2Dfor3D + 14, iNoPoints2Dfor3D + 11);

            // Hole 1 - bottom
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i + 1) * (j + 1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + i + ITotNoPointsin2D, iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + i + ITotNoPointsin2D, iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4);
                    }
                }
            }

            // Hole 2 - upper
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i + 1) * (j + 1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + 4 + i + ITotNoPointsin2D, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + 4 + i + ITotNoPointsin2D, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4);
                    }
                }
            }

            // Between first and second layer

            // Holes

            // Bottom

            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < (INumberOfPointsOfHole-1))
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + i +1, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + i + 1);
                }
                else // Last rectangle
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber);
                }
            }

            // Upper


            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < (INumberOfPointsOfHole - 1))
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole + i +1, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole + i + 1);
                }
                else // Last rectangle
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber +  INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole, ITotNoPointsin2D + 4 * AnchorArrangement.IHolesNumber + INumberOfPointsOfHole);
                }
            }


            for (int i = 0; i < ITotNoPointsin2D; i++)
            {
                if (i < (ITotNoPointsin2D - 1))
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, i + 1, iNoPoints2Dfor3D + i + 1, iNoPoints2Dfor3D + i);
                else
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, 0, iNoPoints2Dfor3D, iNoPoints2Dfor3D + i);
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
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 3]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 3]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 2]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 2]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 1]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 1]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 0]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 0]);
            wireFrame.Points.Add(arrPoints3D[0]);

            // y = m_fhY
            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 4]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 4]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 5]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 5]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 6]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 6]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 7]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 7]);
            wireFrame.Points.Add(arrPoints3D[7]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 0]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 7]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 1]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 6]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 2]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 5]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 3]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 4]);

            // Holes

            // Bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4]);
                }
            }

            // Upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole]);
                }
            }

            // Bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i+1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4]);
                }
            }

            // Upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i+1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole]);
                }
            }

            // Lateral holes

            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i]);
                wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + i]);
            }

            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {

                wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + AnchorArrangement.IHolesNumber * 4 + INumberOfPointsOfHole + i]);

            }

            return wireFrame;
        }


        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            //if (plate is CConCom_Plate_BB_BG)
            //{
            //    this.AnchorArrangement = ((CConCom_Plate_BB_BG)plate).AnchorArrangement;
            //}
        }
    }
}
