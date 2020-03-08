using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using _3DTools;

namespace BaseClasses
{
    [Serializable]
    public class CWasher_W : CPlate
    {
        List<Point> pointsIn_2D;
        List<Point> pointsOut_2D;

        private float m_fHoleDiameter; // Priemer otvoru vo washer - priemer kotvy pre < 20 mm je to 16 mm + 2 mm = 18 mm, pre >= 20 mm je to napr. 20 + 3 mm = 23 mm

        public float HoleDiameter
        {
            get
            {
                return m_fHoleDiameter;
            }

            set
            {
                m_fHoleDiameter = value;
            }
        }

        short iEdgesOutBasic = 4;
        short iNumberOfSegmentsPerSideOut = 4;

        public CWasher_W()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_W;
        }

        public CWasher_W(string sName_temp,
        Point3D controlpoint,
        //float fbx_1_temp,
        //float fhy_1_temp,
        //float ft_platethickness,
        float fHoleDiameter,
        float fRotation_x_deg,
        float fRotation_y_deg,
        float fRotation_z_deg)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_W;

            ITotNoPointsin2D = 4;
            INoPoints2Dfor3D = 16 + 16;
            ITotNoPointsin3D = 32 + 32;

            m_pControlPoint = controlpoint;
            //m_fbX1 = fbx_1_temp;
            //m_fhY1 = fhy_1_temp;
            //Ft = ft_platethickness;
            m_fHoleDiameter = fHoleDiameter;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // TO Ondrej - tu nacitavam velkosti a dalsie parametre washer podla Name z databazy
            // Ak by sme teda chceli aby sa dali menit rozmery v GUI, tak by sa to pri update uz podla mena neprepisovala databazovymi hodnotami
            // Load washer properties from the database
            if (Name != null && Name.Length > 1)
            {
                DATABASE.DTO.CRectWasher_W_Properties prop = DATABASE.CWashersManager.GetPlate_W_Properties(Name);

                Width_bx = (float)prop.dim1x;
                Height_hy = (float)prop.dim2y;
                Ft = (float)prop.thickness;
                //fArea = (float)prop.Area;
                //fCuttingRouteDistance = 2 * Width_bx + 2 * Height_hy;
                //fSurface = 2 * fArea + fCuttingRouteDistance * Ft;
                //fVolume = (float)prop.Volume;
                //fMass = (float)prop.Mass;
                Price_PPKG_NZD = prop.Price_PPKG_NZD;
                //Price_PPP_NZD = prop.Price_PPP_NZD;
            }

            UpdatePlateData();
        }

        public void UpdatePlateData()
        {
            m_Mat.Name = "Q235"; // ???? TODO zapracovat do databazy ??? // AS/NZS1111

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(/*null*/);

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();
        }

        public void UpdatePlateData_Basic(/*CScrewArrangement screwArrangement*/)
        {
            //TODO - obdlznikova geometria nie je implementovana pre 3D vykreslenie

            //SetFlatedPlateDimensions();
            Width_bx_Stretched = Width_bx;
            Height_hy_Stretched = Height_hy;
            fArea = Width_bx * Height_hy; //fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = 2* Width_bx + 2 * Height_hy;// fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = 2 * fArea + fCuttingRouteDistance * Ft; // fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();
            //Price_PPKG_NZD - database
            Price_PPP_NZD = fMass * Price_PPKG_NZD;

            // Minimum edge distances - zadane v suradnicovom smere plechu
            //SetMinimumScrewToEdgeDistances(screwArrangement);

            fA_g = Get_A_rect(Ft, Height_hy);
            int iNumberOfSHolesInSection = 1; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;
            if (iNumberOfSHolesInSection > 0)
            {
                fA_n -= iNumberOfSHolesInSection * m_fHoleDiameter * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, Height_hy);

            fA_vn_zv = fA_v_zv;
            if (iNumberOfSHolesInSection > 0)
            {
                fA_vn_zv -= iNumberOfSHolesInSection * m_fHoleDiameter * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, Height_hy);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, Height_hy); // Elastic section modulus

            //ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        // TODO Ondrej Refactoring CNut
        public override void Calc_Coord2D()
        {
            //TODO - obdlznikova geometria nie je implementovana pre 3D vykreslenie

            // Outline Points
            // Outline Edges
            float fRadiusOut = Width_bx * (float)MathF.dSqrt2 / 2;  // Ma sa nastavit ako polovica dlzky uhlopriecky // Plati len pre stvorcove podlozky, je potrebne doimplementovat obdlznikove
            float fRadiusIn = 0.5f * m_fHoleDiameter;

            int iEdgeOut = iEdgesOutBasic * iNumberOfSegmentsPerSideOut;
            int iEdgesInBasic = iEdgeOut;

            float fAngleBasic_rad = MathF.fPI / iEdgesOutBasic;

            List<Point>pointsOutBasic_2D = Geom2D.GetPolygonPointCoord_RadiusInput_CW(fRadiusOut, iEdgesOutBasic);
            pointsIn_2D = Geom2D.GetPolygonPointCoord_RadiusInput_CW(fRadiusIn, (short)iEdgesInBasic); // Kruh vo vnutri
            pointsOut_2D = Geom2D.GetPolygonPointsIncludingIntermediateOnSides_CW(fRadiusOut, iEdgesOutBasic, iNumberOfSegmentsPerSideOut); // Stvorec (TODO - zamysliet sa ako dorobit vynimku pre obldznik, uhol medzi uhloprieckami nie je rovnaky)

            // Pre kombinaciu stvorca a kruhu musime body stvorca pootocit tak, aby bol prvy bod v y = 0 a uprostred strany stvorca, nie rohovy bod
            // Naprv presunieme body tak aby bol prvy bod zoznamu bod ktory je uprostred strany stvorca
            ChangeFirstNodeOfinList(2, ref pointsOut_2D);

            // Nasledne pootocime body okolo [0,0] o 45 stupnov proti smeru hodinovych ruciciek
            Geom2D.TransformPositions_CCW_rad(0, 0, fAngleBasic_rad, ref pointsOut_2D);

            // !!! Ostatne plates maju body definovane proti smeru hodinovych ruciciek s [0,0] vlavo dole, toto je v smere hodinovych ruciciek a [0,0] je v strede
            ChangeFirstNodeOfinList(2, ref pointsOutBasic_2D);
            // Nasledne pootocime body okolo [0,0] o 45 stupnov proti smeru hodinovych ruciciek
            Geom2D.TransformPositions_CCW_rad(0, 0, fAngleBasic_rad, ref pointsOutBasic_2D);

            PointsOut2D = pointsOutBasic_2D.ToArray();
        }

        // TODO Ondrej Refactoring CNut
        public override void Calc_Coord3D()
        {
            //TODO - obdlznikova geometria nie je implementovana pre 3D vykreslenie

            int iNumberOfPointsPerPolygon = INoPoints2Dfor3D / 2;

            // First layer
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                // Vonkajsi obvod
                arrPoints3D[i].X = pointsOut_2D[i].X;
                arrPoints3D[i].Y = pointsOut_2D[i].Y;
                arrPoints3D[i].Z = 0;

                // Vnutorny kruh
                arrPoints3D[iNumberOfPointsPerPolygon + i].X = pointsIn_2D[i].X;
                arrPoints3D[iNumberOfPointsPerPolygon + i].Y = pointsIn_2D[i].Y;
                arrPoints3D[iNumberOfPointsPerPolygon + i].Z = 0;
            }

            // Second layer
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                arrPoints3D[INoPoints2Dfor3D + i].X = arrPoints3D[i].X;
                arrPoints3D[INoPoints2Dfor3D + i].Y = arrPoints3D[i].Y;
                arrPoints3D[INoPoints2Dfor3D + i].Z = Ft;
            }
        }

        protected override void loadIndices()
        {
            LoadIndicesPrismWithOpening(iEdgesOutBasic * iNumberOfSegmentsPerSideOut);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            return CreateWireFrameModel(iEdgesOutBasic, iNumberOfSegmentsPerSideOut, true);
        }

        // TODO Ondrej, pozri sa na to - asi to vies urobit lepsie
        public void ChangeFirstNodeOfinList(int iOffsetCount, ref List<Point> points)
        {
            List<Point> offSet = points.GetRange(0, iOffsetCount); // Zo zaciatku vyberieme prvky v pocte iOffsetCount a presunieme ich na koniec listu
            points.AddRange(offSet); // Pridame polozky na koniec zoznamu
            points.RemoveRange(0, iOffsetCount); // Vymazeme polozky zo zaciatku zoznamu
        }

        //public override void loadWireFrameIndices() { }


        //Ak by nahodou trebalo aj pre washer
        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CWasher_W)
            {
                CWasher_W refPlate = (CWasher_W)plate;
                this.pointsIn_2D = refPlate.pointsIn_2D;
                this.pointsOut_2D = refPlate.pointsOut_2D;
                this.iEdgesOutBasic = refPlate.iEdgesOutBasic;
                this.iNumberOfSegmentsPerSideOut = refPlate.iNumberOfSegmentsPerSideOut;
                this.m_fHoleDiameter = refPlate.m_fHoleDiameter;
            }
        }
    }
}
