using BaseClasses.Helpers;
using DATABASE.DTO;
using MATERIAL;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    // TO Ondrej - z tejto triedy by som chcel urobit skor nejaky "manager", ktory bude pripravovať data a funkcie pre generovanie samotnych objektov sheets
    //To Mato - nechavame stale strasne vela komentovaneho kodu, mam chut to vsetko pomazat
    public class CCladding : CEntity3D
    {
        public EModelType_FS eModelType;
        public BuildingGeometryDataInput sBuildingGeomInputData;
        public System.Collections.ObjectModel.ObservableCollection<CCanopiesInfo> canopyCollection;
        public System.Collections.ObjectModel.ObservableCollection<CBayInfo> bayWidthCollection;
        public System.Collections.ObjectModel.ObservableCollection<FibreglassProperties> fibreglassSheetCollection;
        public System.Collections.ObjectModel.ObservableCollection<DoorProperties> doorPropCollection;
        public System.Collections.ObjectModel.ObservableCollection<WindowProperties> windowPropCollection;

        float m_fFrontColumnDistance; // Vzdialenost front columns
        float m_fBackColumnDistance; // Vzdialenost back columns

        double m_column_crsc_z_plus;
        double m_column_crsc_y_minus;
        double m_column_crsc_y_plus;

        string m_claddingShape_Wall;
        string m_claddingCoatingType_Wall;
        string m_claddingShape_Roof;
        string m_claddingCoatingType_Roof;

        string m_ColorNameWall;
        string m_ColorNameRoof;

        Color m_ColorWall;
        Color m_ColorRoof;

        MATERIAL.CMat m_MaterialCladding_Wall;
        MATERIAL.CMat m_MaterialCladding_Roof;

        MATERIAL.CMat m_MaterialFibreglass_Wall;
        MATERIAL.CMat m_MaterialFibreglass_Roof;

        DATABASE.DTO.CTS_CrscProperties m_WallProps;
        DATABASE.DTO.CTS_CrscProperties m_RoofProps;
        DATABASE.DTO.CTS_CoilProperties m_WallCoilProps;
        DATABASE.DTO.CTS_CoilProperties m_RoofCoilProps;

        // Consider roof cladding height for front and back wall
        bool considerRoofCladdingFor_FB_WallHeight;
        double bottomEdge_z;// = -0.05; // Offset pod spodnu uroven podlahy, default -50 mm, limit <-500mm, 0>
        double roofEdgeOverhang_X; // = 0.150; // Presah okraja strechy, default 150 mm, limit <0, 500mm>
        double roofEdgeOverhang_Y; // = 0.000; // Presah okraja strechy, default 0 mm limit <0, 300mm>
        double canopyOverhangOffset_x;
        double canopyOverhangOffset_y;

        // Fibre glass properties
        string m_ColorNameRoof_FG;
        string m_claddingShape_Roof_FG;
        string m_claddingCoatingType_Roof_FG;
        Color m_ColorRoof_FG;
        DATABASE.DTO.CFibreglassProperties m_RoofProps_FG;

        string m_ColorNameWall_FG;
        string m_claddingShape_Wall_FG;
        string m_claddingCoatingType_Wall_FG;
        Color m_ColorWall_FG;
        DATABASE.DTO.CFibreglassProperties m_WallProps_FG;

        bool bIndividualCladdingSheets;

        bool bGenerateLeftSideCladding = true;
        bool bGenerateFrontSideCladding = true;
        bool bGenerateRightSideCladding = true;
        bool bGenerateBackSideCladding = true;
        bool bGenerateRoofCladding = true;

        bool bGenerateLeftSideFibreglass = true;
        bool bGenerateFrontSideFibreglass = true;
        bool bGenerateRightSideFibreglass = true;
        bool bGenerateBackSideFibreglass = true;
        bool bGenerateRoofFibreglass = true;

        public List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallLeft = null;
        public List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsLeftWall = null;
        public List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallFront = null;
        public List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsFrontWall = null;
        public List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallRight = null;
        public List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRightWall = null;
        public List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallBack = null;
        public List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsBackWall = null;
        public List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsRoofRight = null;
        public List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRoofRight = null;
        public List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsRoofLeft = null;
        public List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRoofLeft = null;

        float maxSheetLegth_RoofCladding;
        float maxSheetLegth_WallCladding;
        float maxSheetLegth_RoofFibreglass;
        float maxSheetLegth_WallFibreglass;

        float overlap_RoofCladding;
        float overlap_WallCladding;
        float overlap_RoofFibreglass;
        float overlap_WallFibreglass;

        bool bUseTop20Colors = true; // Pouzit vsetky farby v zozname (141) alebo striedat len vybrane (20)

        public double dRoofSide_length; // Roof Side Length

        public double Column_crsc_z_plus
        {
            get
            {
                return m_column_crsc_z_plus;
            }

            set
            {
                m_column_crsc_z_plus = value;
            }
        }

        public double Column_crsc_y_minus
        {
            get
            {
                return m_column_crsc_y_minus;
            }

            set
            {
                m_column_crsc_y_minus = value;
            }
        }

        public double Column_crsc_y_plus
        {
            get
            {
                return m_column_crsc_y_plus;
            }

            set
            {
                m_column_crsc_y_plus = value;
            }
        }

        public CTS_CrscProperties WallProps
        {
            get
            {
                return m_WallProps;
            }

            set
            {
                m_WallProps = value;
            }
        }

        public CTS_CrscProperties RoofProps
        {
            get
            {
                return m_RoofProps;
            }

            set
            {
                m_RoofProps = value;
            }
        }

        public CTS_CoilProperties WallCoilProps
        {
            get
            {
                return m_WallCoilProps;
            }

            set
            {
                m_WallCoilProps = value;
            }
        }

        public CTS_CoilProperties RoofCoilProps
        {
            get
            {
                return m_RoofCoilProps;
            }

            set
            {
                m_RoofCoilProps = value;
            }
        }

        public CFibreglassProperties RoofProps_FG
        {
            get
            {
                return m_RoofProps_FG;
            }

            set
            {
                m_RoofProps_FG = value;
            }
        }

        public CFibreglassProperties WallProps_FG
        {
            get
            {
                return m_WallProps_FG;
            }

            set
            {
                m_WallProps_FG = value;
            }
        }

        public string ColorNameWall
        {
            get
            {
                return m_ColorNameWall;
            }

            set
            {
                m_ColorNameWall = value;
            }
        }

        public string ColorNameRoof
        {
            get
            {
                return m_ColorNameRoof;
            }

            set
            {
                m_ColorNameRoof = value;
            }
        }

        public string ColorNameRoof_FG
        {
            get
            {
                return m_ColorNameRoof_FG;
            }

            set
            {
                m_ColorNameRoof_FG = value;
            }
        }

        public string CladdingShape_Roof_FG
        {
            get
            {
                return m_claddingShape_Roof_FG;
            }

            set
            {
                m_claddingShape_Roof_FG = value;
            }
        }

        public string CladdingCoatingType_Roof_FG
        {
            get
            {
                return m_claddingCoatingType_Roof_FG;
            }

            set
            {
                m_claddingCoatingType_Roof_FG = value;
            }
        }

        public string ColorNameWall_FG
        {
            get
            {
                return m_ColorNameWall_FG;
            }

            set
            {
                m_ColorNameWall_FG = value;
            }
        }

        public string CladdingShape_Wall_FG
        {
            get
            {
                return m_claddingShape_Wall_FG;
            }

            set
            {
                m_claddingShape_Wall_FG = value;
            }
        }

        public string CladdingCoatingType_Wall_FG
        {
            get
            {
                return m_claddingCoatingType_Wall_FG;
            }

            set
            {
                m_claddingCoatingType_Wall_FG = value;
            }
        }

        public Color ColorWall
        {
            get
            {
                return m_ColorWall;
            }

            set
            {
                m_ColorWall = value;
            }
        }

        public Color ColorRoof
        {
            get
            {
                return m_ColorRoof;
            }

            set
            {
                m_ColorRoof = value;
            }
        }

        public Color ColorRoof_FG
        {
            get
            {
                return m_ColorRoof_FG;
            }

            set
            {
                m_ColorRoof_FG = value;
            }
        }

        public Color ColorWall_FG
        {
            get
            {
                return m_ColorWall_FG;
            }

            set
            {
                m_ColorWall_FG = value;
            }
        }

        public string CladdingShape_Wall
        {
            get
            {
                return m_claddingShape_Wall;
            }

            set
            {
                m_claddingShape_Wall = value;
            }
        }

        public string CladdingCoatingType_Wall
        {
            get
            {
                return m_claddingCoatingType_Wall;
            }

            set
            {
                m_claddingCoatingType_Wall = value;
            }
        }

        public string CladdingShape_Roof
        {
            get
            {
                return m_claddingShape_Roof;
            }

            set
            {
                m_claddingShape_Roof = value;
            }
        }

        public string CladdingCoatingType_Roof
        {
            get
            {
                return m_claddingCoatingType_Roof;
            }

            set
            {
                m_claddingCoatingType_Roof = value;
            }
        }

        public CMat MaterialCladding_Wall
        {
            get
            {
                return m_MaterialCladding_Wall;
            }

            set
            {
                m_MaterialCladding_Wall = value;
            }
        }

        public CMat MaterialCladding_Roof
        {
            get
            {
                return m_MaterialCladding_Roof;
            }

            set
            {
                m_MaterialCladding_Roof = value;
            }
        }

        public CMat MaterialFibreglass_Wall
        {
            get
            {
                return m_MaterialFibreglass_Wall;
            }

            set
            {
                m_MaterialFibreglass_Wall = value;
            }
        }

        public CMat MaterialFibreglass_Roof
        {
            get
            {
                return m_MaterialFibreglass_Roof;
            }

            set
            {
                m_MaterialFibreglass_Roof = value;
            }
        }

        public float FFrontColumnDistance
        {
            get
            {
                return m_fFrontColumnDistance;
            }

            set
            {
                m_fFrontColumnDistance = value;
            }
        }

        public float FBackColumnDistance
        {
            get
            {
                return m_fBackColumnDistance;
            }

            set
            {
                m_fBackColumnDistance = value;
            }
        }

        public CCladding()
        {
            canopyCollection = new System.Collections.ObjectModel.ObservableCollection<CCanopiesInfo>(); //nechce sa nam stale kontrolovat na null
            fibreglassSheetCollection = new System.Collections.ObjectModel.ObservableCollection<FibreglassProperties>(); //nechce sa nam stale kontrolovat na null
            bayWidthCollection = new System.Collections.ObjectModel.ObservableCollection<CBayInfo>();//nechce sa nam stale kontrolovat na null
            doorPropCollection = new System.Collections.ObjectModel.ObservableCollection<DoorProperties>();//nechce sa nam stale kontrolovat na null
            windowPropCollection = new System.Collections.ObjectModel.ObservableCollection<WindowProperties>();//nechce sa nam stale kontrolovat na null
        }

        // Constructor 2
        public CCladding(int iCladding_ID, EModelType_FS modelType_FS, BuildingGeometryDataInput sGeometryInputData,
            bool bIndividualSheets,
            IList<CComponentInfo> componentList,
            System.Collections.ObjectModel.ObservableCollection<CCanopiesInfo> canopies,
            System.Collections.ObjectModel.ObservableCollection<CBayInfo> bayWidths,
            System.Collections.ObjectModel.ObservableCollection<FibreglassProperties> fibreglassProp,
            System.Collections.ObjectModel.ObservableCollection<DoorProperties> doorProp,
            System.Collections.ObjectModel.ObservableCollection<WindowProperties> windowProp,
            CRSC.CCrSc_TW columnSection,
            float fFrontColumnDistance, float fBackColumnDistance,
            string colorName_Wall, string colorName_Roof,
            string claddingShape_Wall, string claddingCoatingType_Wall,
            string claddingShape_Roof, string claddingCoatingType_Roof,
            Color colorWall, Color colorRoof,
            Color colorWall_FG, Color colorRoof_FG,
            string colorWall_FG_Name, string colorRoof_FG_Name,
            string thicknessName_Wall_FG, string thicknessName_Roof_FG,
            bool bIsDisplayed, int fTime,
            DATABASE.DTO.CTS_CrscProperties wallProps,
            DATABASE.DTO.CTS_CrscProperties roofProps,
            DATABASE.DTO.CTS_CoilProperties wallCoilProps,
            DATABASE.DTO.CTS_CoilProperties roofCoilProps,
            float fRoofEdgeOverHang_FB_Y,
            float fRoofEdgeOverHang_LR_X,
            float fCanopyRoofEdgeOverHang_LR_X,
            float fWallBottomOffset_Z,
            bool bConsiderRoofCladdingFor_FB_WallHeight,
            float fMaxSheetLegth_RoofCladding, float fMaxSheetLegth_WallCladding, float fMaxSheetLegth_RoofFibreglass, float fMaxSheetLegth_WallFibreglass,
            float fOverlap_RoofCladding, float fOverlap_WallCladding, float fOverlap_RoofFibreglass, float fOverlap_WallFibreglass,
            DisplayOptions opts
            )
        {
            ID = iCladding_ID;
            eModelType = modelType_FS;
            sBuildingGeomInputData = sGeometryInputData;
            bIndividualCladdingSheets = bIndividualSheets;
            canopyCollection = canopies;
            bayWidthCollection = bayWidths;
            fibreglassSheetCollection = fibreglassProp;
            doorPropCollection = doorProp;
            windowPropCollection = windowProp;
            Column_crsc_z_plus = columnSection.z_max;
            Column_crsc_y_minus = columnSection.y_min;
            Column_crsc_y_plus = columnSection.y_max;
            FFrontColumnDistance = fFrontColumnDistance;
            FBackColumnDistance = fBackColumnDistance;

            maxSheetLegth_RoofCladding = fMaxSheetLegth_RoofCladding;
            maxSheetLegth_WallCladding = fMaxSheetLegth_WallCladding;
            maxSheetLegth_RoofFibreglass = fMaxSheetLegth_RoofFibreglass;
            maxSheetLegth_WallFibreglass = fMaxSheetLegth_WallFibreglass;
            overlap_RoofCladding = fOverlap_RoofCladding;
            overlap_WallCladding = fOverlap_WallCladding;
            overlap_RoofFibreglass = fOverlap_RoofFibreglass;
            overlap_WallFibreglass = fOverlap_WallFibreglass;

            ColorNameWall = colorName_Wall;
            ColorNameRoof = colorName_Roof;
            CladdingShape_Wall = claddingShape_Wall;
            CladdingCoatingType_Wall = claddingCoatingType_Wall;
            CladdingShape_Roof = claddingShape_Roof;
            CladdingCoatingType_Roof = claddingCoatingType_Roof;

            SetCladdingGenerateProperties(componentList);

            ColorWall = colorWall;
            ColorRoof = colorRoof;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            WallProps = wallProps;
            RoofProps = roofProps;
            WallCoilProps = wallCoilProps;
            RoofCoilProps = roofCoilProps;

            roofEdgeOverhang_Y = fRoofEdgeOverHang_FB_Y;
            roofEdgeOverhang_X = fRoofEdgeOverHang_LR_X;
            canopyOverhangOffset_x = fCanopyRoofEdgeOverHang_LR_X;
            bottomEdge_z = fWallBottomOffset_Z;
            considerRoofCladdingFor_FB_WallHeight = bConsiderRoofCladdingFor_FB_WallHeight;

            canopyOverhangOffset_y = roofEdgeOverhang_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

            ColorNameRoof_FG = colorRoof_FG_Name;
            CladdingShape_Roof_FG = CladdingShape_Roof;
            CladdingCoatingType_Roof_FG = "";
            ColorRoof_FG = colorRoof_FG;
            RoofProps_FG = DATABASE.CFibreglassManager.LoadFibreglassProperties_meters(claddingShape_Roof + "-" + thicknessName_Roof_FG); // TODO Presunut definiciu do VM

            ColorNameWall_FG = colorWall_FG_Name;
            CladdingShape_Wall_FG = CladdingShape_Wall;
            CladdingCoatingType_Wall_FG = "";
            ColorWall_FG = colorWall_FG;
            WallProps_FG = DATABASE.CFibreglassManager.LoadFibreglassProperties_meters(claddingShape_Wall + "-" + thicknessName_Wall_FG); // TODO Presunut definiciu do VM

            MaterialCladding_Wall = new MATERIAL.CMat_03_00(wallProps.material_Name, 200e+9f, 80e+9f, 0.3f, 7850f);
            MaterialCladding_Roof = new MATERIAL.CMat_03_00(roofProps.material_Name, 200e+9f, 80e+9f, 0.3f, 7850f);
            MaterialFibreglass_Wall = new MATERIAL.CMat_10_00("default"); // Default - TODO dopracovat databazu materialov fibreglass, pripade do GUI moznost volby materialu
            MaterialFibreglass_Roof = new MATERIAL.CMat_10_00("default"); // Default - TODO dopracovat databazu materialov fibreglass, pripade do GUI moznost volby materialu

            GenerateCladding(opts);
        }

        public void GenerateCladding(DisplayOptions options)
        {
            // Vytvorime model v GCS [0,0,0] je uvazovana v bode m_ControlPoint
            ControlPoint = new Point3D(0, 0, 0);

            // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
            // Start
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
            double additionalOffset = 0.010;  // 10 mm Aby nekolidovali plochy cladding s members
            double additionalOffsetRoof = 0.010; // 10 mm Aby nekolidovali plochy cladding s members (cross-bracing) na streche

            // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
            double column_crsc_y_minus_withAddOffset = Column_crsc_y_minus - additionalOffset;
            double column_crsc_y_plus_withAddOffset = Column_crsc_y_plus + additionalOffset;
            double column_crsc_z_plus_withAddOffset = Column_crsc_z_plus + additionalOffset;

            //----------------------------------------
            // To Ondrej - toto by sme mali nahradit funkciou
            // CalculateWallHeightsForCladding
            double height_1_final = sBuildingGeomInputData.fH_1_centerline + Column_crsc_z_plus / Math.Cos(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
            double height_2_final = sBuildingGeomInputData.fH_2_centerline + Column_crsc_z_plus / Math.Cos(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon

            double height_1_final_edge_LR_Wall = height_1_final - column_crsc_z_plus_withAddOffset * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            double height_2_final_edge_LR_Wall = height_2_final;

            double height_1_final_edge_Roof = height_1_final + additionalOffsetRoof - (column_crsc_z_plus_withAddOffset + roofEdgeOverhang_X) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            double height_2_final_edge_Roof = height_2_final + additionalOffsetRoof;

            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                height_2_final_edge_LR_Wall = height_2_final + column_crsc_z_plus_withAddOffset * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                height_2_final_edge_Roof = height_2_final + additionalOffsetRoof + (column_crsc_z_plus_withAddOffset + roofEdgeOverhang_X) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            }

            // Nastavime rovnaku vysku hornej hrany
            double height_1_final_edge_FB_Wall = height_1_final_edge_LR_Wall;
            double height_2_final_edge_FB_Wall = height_2_final_edge_LR_Wall;

            if (considerRoofCladdingFor_FB_WallHeight)
            {
                height_1_final_edge_FB_Wall = height_1_final_edge_FB_Wall + RoofProps.height_m * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                height_2_final_edge_FB_Wall = height_2_final_edge_FB_Wall + RoofProps.height_m * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                    height_2_final_edge_FB_Wall = height_2_final + (column_crsc_z_plus_withAddOffset + RoofProps.height_m) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            }
            //----------------------------------------

            // Wall Cladding Edges

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus_withAddOffset, column_crsc_y_minus_withAddOffset, bottomEdge_z);
            Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_withAddOffset, column_crsc_y_minus_withAddOffset, bottomEdge_z);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus_withAddOffset, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset, bottomEdge_z);
            Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_withAddOffset, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset, bottomEdge_z);

            // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
            // End
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------

            // TODO - mohlo by prist z Main Window, rozdiel je ze pridavame nejake odstadenie od hrany profilu, aby s nim plech nekolidoval v 3D
            double baseWidth_Overall_for3D = sBuildingGeomInputData.fWidth_overall + 2 * additionalOffset;
            // TODO - mohlo by prist z Main Window, rozdiel je ze pridavame nejake odstadenie od hrany profilu, aby s nim plech nekolidoval v 3D
            double baseLength_Overall_for3D = sBuildingGeomInputData.fLength_overall + 2 * additionalOffset;

            // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
            // Start
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
            // Wall Points
            //Point3D pLRWall_front2_heightright = new Point3D();
            //Point3D pLRWall_back2_heightright = new Point3D();
            //Point3D pLRWall_front3_heightleft = new Point3D();
            //Point3D pLRWall_back3_heightleft = new Point3D();
            //Point3D pLRWall_front4_top = new Point3D();
            //Point3D pLRWall_back4_top = new Point3D();

            //Point3D pFBWall_front2_heightright = new Point3D();
            //Point3D pFBWall_back2_heightright = new Point3D();
            //Point3D pFBWall_front3_heightleft = new Point3D();
            //Point3D pFBWall_back3_heightleft = new Point3D();
            //Point3D pFBWall_front4_top = new Point3D();
            //Point3D pFBWall_back4_top = new Point3D();

            // Roof Points - oddelene pretoze strecha ma presahy
            Point3D pRoof_front2_heightright = new Point3D();
            Point3D pRoof_back2_heightright = new Point3D();
            Point3D pRoof_front3_heightleft = new Point3D();
            Point3D pRoof_back3_heightleft = new Point3D();
            Point3D pRoof_front4_top = new Point3D();
            Point3D pRoof_back4_top = new Point3D();

            // Roof edge offset from centerline in Y-direction
            float fRoofEdgeOffsetFromCenterline = -(float)column_crsc_y_minus_withAddOffset + (float)roofEdgeOverhang_Y;

            // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
            // End
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------

            int iNumberOfFrontBackWallEdges = 4;

            //Point3D pControlPoint_FrontWall = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z);
            //Point3D pControlPoint_BackWall = new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z);
            //Point3D pControlPoint_LeftWall = new Point3D(pback0_baseleft.X, pback0_baseleft.Y, pback0_baseleft.Z);
            //Point3D pControlPoint_RightWall = new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z);

            Point3D pControlPoint_RoofRight;
            Point3D pControlPoint_RoofLeft;

            // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
            // Start
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
            // Nastavenie bodov suradnic hornych bodov stien a bodov strechy pre monopitch a gable roof model
            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                // Monopitch Roof

                // Wall
                iNumberOfFrontBackWallEdges = 4;
                //pLRWall_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_2_final_edge_FB_Wall);
                //pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_FB_Wall);

                //pLRWall_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_2_final_edge_FB_Wall);
                //pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_FB_Wall);

                //pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, height_2_final_edge_FB_Wall);
                //pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, height_2_final_edge_FB_Wall);
                //pFBWall_front3_heightleft = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, height_1_final_edge_FB_Wall);
                //pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, height_1_final_edge_FB_Wall);

                // Roof
                pRoof_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_withAddOffset + roofEdgeOverhang_X, column_crsc_y_minus_withAddOffset - roofEdgeOverhang_Y, height_2_final_edge_Roof);
                pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_withAddOffset - roofEdgeOverhang_X, column_crsc_y_minus_withAddOffset - roofEdgeOverhang_Y, height_1_final_edge_Roof);

                pRoof_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_withAddOffset + roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset + roofEdgeOverhang_Y, height_2_final_edge_Roof);
                pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_withAddOffset - roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset + roofEdgeOverhang_Y, height_1_final_edge_Roof);

            }
            else if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Gable Roof Building

                // Wall
                iNumberOfFrontBackWallEdges = 5;
                //pLRWall_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_LR_Wall);
                //pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_LR_Wall);
                //pLRWall_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp, height_2_final_edge_LR_Wall);

                //pLRWall_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_LR_Wall);
                //pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_LR_Wall);
                //pLRWall_back4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_2_final_edge_LR_Wall);

                //pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, height_1_final_edge_FB_Wall);
                //pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, height_1_final_edge_FB_Wall);
                //pFBWall_front3_heightleft = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, height_1_final_edge_FB_Wall);
                //pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, height_1_final_edge_FB_Wall);
                //pFBWall_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp, height_2_final_edge_FB_Wall);
                //pFBWall_back4_top = new Point3D(pLRWall_back4_top.X, pLRWall_back4_top.Y, height_2_final_edge_FB_Wall);

                // Roof
                pRoof_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_withAddOffset + roofEdgeOverhang_X, column_crsc_y_minus_withAddOffset - roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_withAddOffset - roofEdgeOverhang_X, column_crsc_y_minus_withAddOffset - roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_withAddOffset - roofEdgeOverhang_Y, height_2_final_edge_Roof);

                pRoof_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_withAddOffset + roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset + roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_withAddOffset - roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset + roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_back4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset + roofEdgeOverhang_Y, height_2_final_edge_Roof);

            }
            else
            {
                throw new Exception("Not implemented kitset type.");
            }

            // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
            // End
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------

            // Ak je bDisplayIndividualCladdingSheets = false
            // Zobrazime len jednoliatu plochu s farbou alebo texturou, nad nou mozeme zobrazit fibreglass sheet s offsetom aby sa nevnarali do cladding
            // Bude to podobne ako door a window, takze sa nebudu kreslit realne otvory len sa nad plochu strechy dokresli fibreglass sheet
            // V takom pripade nebudeme generovat cladding sheet material list ani cladding sheet layout pattern
            // Len spocitame plochu otvorov a odratame ju z celkovej plochy cladding a to bude v Quotation

            // Ak je bDisplayIndividualCladdingSheets = true
            // Generujeme jednotlive plechy, tie rozdelime podla toho ako koliduju s otvormi
            // V mieste otvorov pre fibreglass, door, windows sa nebudú generovat cladding sheets, ktore su otvorom rozdelene po celej sirke
            // Cladding sheets, ktore sa s otvorom prekryvaju len ciastocne vykreslime v 3D cele
            // Tuto zostavu cladding sheets, fibreglass sheets, doors, windows vieme pekne vykreslit do layouts ako 2D pohlady na jednotlive steny
            // Do vykazu materialov mozeme potom uviest jednotlive dlzky cladding sheets

            // Particular Cladding Sheet Model

            // Prva uroven, stany budovy alebo strechy, left, right, front, back, roof left roof right
            // Druha uroven jednotlive sheet nachadzajuce sa v jednej rovine
            //List<List<CCladdingOrFibreGlassSheet>> listOfCladdingSheets = new List<List<CCladdingOrFibreGlassSheet>>();

            int iSheetIndex = 0;
            int iSheet_FG_Index = 0;
            int iOpeningIndex = 0;
            int iNumberOfEdges_FG_D_W = 4; // Number of edges - fibreglass shet, door, window opening


            // Left Wall
            double height_left_basic = -bottomEdge_z + height_1_final_edge_LR_Wall;

            int iNumberOfWholeSheets = (int)(baseLength_Overall_for3D / WallProps.widthModular_m);
            double dWidthOfWholeSheets = iNumberOfWholeSheets * WallProps.widthModular_m;
            double dPartialSheet_End = baseLength_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
            int iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<COpening> listOfOpeningsLeftWall_All = null;
            List<CCladdingOrFibreGlassSheet> listOfFibreGlassOpenings = null;

            if (bGenerateLeftSideFibreglass)
            {
                listOfFibreGlassSheetsWallLeft = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

                foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                {
                    if (fgsp.Side == "Left")
                    {
                        CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFL", "Fibreglass - Left Wall", fgsp.Location, MaterialFibreglass_Wall,
                            WallProps_FG.thickness_m, WallProps_FG.widthCoil_m, WallProps_FG.flatsheet_mass_kg_m2, WallProps_FG.price_PPSM_NZD * WallProps_FG.widthModular_m / WallProps_FG.widthCoil_m, WallProps_FG.widthModular_m,
                            iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                            pback0_baseleft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
                            ColorNameWall_FG, CladdingShape_Wall_FG, CladdingCoatingType_Wall_FG, ColorWall_FG, options.fFibreglassOpacity, WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));

                        listOfFibreGlassOpenings.Add(sheet);
                        List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                        listOfFibreGlassSheetsWallLeft.AddRange(sheets);
                        iSheet_FG_Index += sheets.Count;
                    }
                }
                GenerateCladdingOpenings(listOfFibreGlassOpenings, "Left", pback0_baseleft, baseLength_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_withAddOffset, column_crsc_z_plus_withAddOffset,
                ref iOpeningIndex, out listOfOpeningsLeftWall_All);
            }

            if (bGenerateLeftSideCladding)
            {
                if (bIndividualCladdingSheets)
                    GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_withAddOffset, "Left", EBuildingSide.Left, "WCL", "Cladding - Left Wall", MaterialCladding_Wall, pback0_baseleft, ColorNameWall,
                    CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fLeftCladdingOpacity,
                    WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, baseLength_Overall_for3D,
                    WallProps.widthRib_m, WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_left_basic,
                    listOfOpeningsLeftWall_All, ref iSheetIndex, out listOfCladdingSheetsLeftWall);
                else // (bGenerateLeftSideCladding && !bIndividualCladdingSheets)
                {
                    listOfCladdingSheetsLeftWall = new List<CCladdingOrFibreGlassSheet>();
                    listOfCladdingSheetsLeftWall.Add(new CCladdingOrFibreGlassSheet(1, "WCL", "Cladding - Left Wall", EBuildingSide.Left, MaterialCladding_Wall,
                     WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, WallProps.widthModular_m,
                        4, 0, 0,
                        pback0_baseleft, baseLength_Overall_for3D, height_left_basic, height_left_basic, 0.5 * baseLength_Overall_for3D, height_left_basic,
                        ColorNameWall, CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fLeftCladdingOpacity,
                        WallProps.widthRib_m, options.bDisplayCladdingLeftWall, 0, false));

                }
            }

            // Front Wall
            height_left_basic = -bottomEdge_z + height_1_final_edge_FB_Wall;

            iNumberOfWholeSheets = (int)(baseWidth_Overall_for3D / WallProps.widthModular_m);
            dWidthOfWholeSheets = iNumberOfWholeSheets * WallProps.widthModular_m;
            dPartialSheet_End = baseWidth_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<COpening> listOfOpeningsFrontWall_All = null;
            if (bGenerateFrontSideFibreglass)
            {
                listOfFibreGlassSheetsWallFront = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

                foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                {
                    if (fgsp.Side == "Front")
                    {
                        CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFF", "Fibreglass - Front Wall", fgsp.Location, MaterialFibreglass_Wall,
                            WallProps_FG.thickness_m, WallProps_FG.widthCoil_m, WallProps_FG.flatsheet_mass_kg_m2, WallProps_FG.price_PPSM_NZD * WallProps_FG.widthModular_m / WallProps_FG.widthCoil_m, WallProps_FG.widthModular_m,
                            iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                            pfront0_baseleft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
                            ColorNameWall_FG, CladdingShape_Wall_FG, CladdingCoatingType_Wall_FG, ColorWall_FG, options.fFibreglassOpacity, WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
                        listOfFibreGlassOpenings.Add(sheet);

                        List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                        listOfFibreGlassSheetsWallFront.AddRange(sheets);
                        iSheet_FG_Index += sheets.Count;
                    }
                }

                GenerateCladdingOpenings(listOfFibreGlassOpenings, "Front", pfront0_baseleft, baseWidth_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_withAddOffset, column_crsc_z_plus_withAddOffset,
                ref iOpeningIndex, out listOfOpeningsFrontWall_All);
            }

            if (bGenerateFrontSideCladding)
            {
                if (bIndividualCladdingSheets)
                    GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_withAddOffset, "Front", EBuildingSide.Front, "WCF", "Cladding - Front Wall", MaterialCladding_Wall, pfront0_baseleft, ColorNameWall,
                    CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fFrontCladdingOpacity,
                    WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, baseWidth_Overall_for3D,
                    WallProps.widthRib_m, WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_2_final_edge_FB_Wall,
                    listOfOpeningsFrontWall_All, ref iSheetIndex, out listOfCladdingSheetsFrontWall);
                else // (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall && !bIndividualCladdingSheets)
                {
                    double height_right_basic = height_left_basic;
                    double height_middle_basic = -bottomEdge_z + height_2_final_edge_FB_Wall;

                    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                    {
                        height_right_basic = -bottomEdge_z + height_2_final_edge_FB_Wall;
                        height_middle_basic = 0.5 * (height_left_basic + height_right_basic);
                    }

                    listOfCladdingSheetsFrontWall = new List<CCladdingOrFibreGlassSheet>();
                    listOfCladdingSheetsFrontWall.Add(new CCladdingOrFibreGlassSheet(2, "WCF", "Cladding - Front Wall", EBuildingSide.Front, MaterialCladding_Wall,
                        WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, WallProps.widthModular_m,
                        iNumberOfFrontBackWallEdges, 0, 0,
                        pfront0_baseleft, baseWidth_Overall_for3D, height_left_basic, height_right_basic, 0.5 * baseWidth_Overall_for3D, height_middle_basic,
                        ColorNameWall, CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fFrontCladdingOpacity, WallProps.widthRib_m,
                        options.bDisplayCladdingFrontWall, 0, false));

                }
            }

            // Right Wall
            height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_LR_Wall : -bottomEdge_z + height_2_final_edge_LR_Wall;

            iNumberOfWholeSheets = (int)(baseLength_Overall_for3D / WallProps.widthModular_m);
            dWidthOfWholeSheets = iNumberOfWholeSheets * WallProps.widthModular_m;
            dPartialSheet_End = baseLength_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<COpening> listOfOpeningsRightWall_All = null;

            if (bGenerateRightSideFibreglass)
            {
                listOfFibreGlassSheetsWallRight = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

                foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                {
                    if (fgsp.Side == "Right")
                    {
                        CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFR", "Fibreglass - Right Wall", fgsp.Location, MaterialFibreglass_Wall,
                            WallProps_FG.thickness_m, WallProps_FG.widthCoil_m, WallProps_FG.flatsheet_mass_kg_m2, WallProps_FG.price_PPSM_NZD * WallProps_FG.widthModular_m / WallProps_FG.widthCoil_m, WallProps_FG.widthModular_m,
                            iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                            pfront1_baseright, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
                            ColorNameWall_FG, CladdingShape_Wall_FG, CladdingCoatingType_Wall_FG, ColorWall_FG, options.fFibreglassOpacity, WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
                        listOfFibreGlassOpenings.Add(sheet);

                        List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                        //CountRealLenghts(sheets, height_left_basic);
                        listOfFibreGlassSheetsWallRight.AddRange(sheets);
                        iSheet_FG_Index += sheets.Count;
                    }
                }
                GenerateCladdingOpenings(listOfFibreGlassOpenings, "Right", pfront1_baseright, baseLength_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_withAddOffset, column_crsc_z_plus_withAddOffset,
                ref iOpeningIndex, out listOfOpeningsRightWall_All);
            }

            if (bGenerateRightSideCladding)
            {
                if (bIndividualCladdingSheets)
                    GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_withAddOffset, "Right", EBuildingSide.Right, "WCR", "Cladding - Right Wall", MaterialCladding_Wall, pfront1_baseright, ColorNameWall,
                    CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fLeftCladdingOpacity,
                    WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, baseLength_Overall_for3D,
                    WallProps.widthRib_m, WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_left_basic,
                    listOfOpeningsRightWall_All, ref iSheetIndex, out listOfCladdingSheetsRightWall);
                else
                {
                    listOfCladdingSheetsRightWall = new List<CCladdingOrFibreGlassSheet>();
                    listOfCladdingSheetsRightWall.Add(new CCladdingOrFibreGlassSheet(3, "WCR", "Cladding - Right Wall", EBuildingSide.Right, MaterialCladding_Wall,
                        WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, WallProps.widthModular_m,
                        4, 0, 0,
                        pfront1_baseright, baseLength_Overall_for3D, height_left_basic, height_left_basic, 0.5 * baseLength_Overall_for3D, height_left_basic,
                        ColorNameWall, CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fLeftCladdingOpacity,
                        WallProps.widthRib_m, options.bDisplayCladdingRightWall, 0, false, false));

                }
            }

            // Back Wall
            height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_FB_Wall : -bottomEdge_z + height_2_final_edge_FB_Wall;

            iNumberOfWholeSheets = (int)(baseWidth_Overall_for3D / WallProps.widthModular_m);
            dWidthOfWholeSheets = iNumberOfWholeSheets * WallProps.widthModular_m;
            dPartialSheet_End = baseWidth_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<COpening> listOfOpeningsBackWall_All = null;

            if (bGenerateBackSideFibreglass)
            {
                listOfFibreGlassSheetsWallBack = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

                foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                {
                    if (fgsp.Side == "Back")
                    {
                        CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFB", "Fibreglass - Back Wall", fgsp.Location, MaterialFibreglass_Wall,
                            WallProps_FG.thickness_m, WallProps_FG.widthCoil_m, WallProps_FG.flatsheet_mass_kg_m2, WallProps_FG.price_PPSM_NZD * WallProps_FG.widthModular_m / WallProps_FG.widthCoil_m, WallProps_FG.widthModular_m,
                            iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                            pback1_baseright, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
                            ColorNameWall_FG, CladdingShape_Wall_FG, CladdingCoatingType_Wall_FG, ColorWall_FG, options.fFibreglassOpacity, WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
                        listOfFibreGlassOpenings.Add(sheet);

                        List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                        //CountRealLenghts(sheets, height_left_basic);
                        listOfFibreGlassSheetsWallBack.AddRange(sheets);
                        iSheet_FG_Index += sheets.Count;
                    }
                }
                GenerateCladdingOpenings(listOfFibreGlassOpenings, "Back", pback1_baseright, baseWidth_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_withAddOffset, column_crsc_z_plus_withAddOffset,
                ref iOpeningIndex, out listOfOpeningsBackWall_All);
            }

            if (bGenerateBackSideCladding)
            {
                if (bIndividualCladdingSheets)
                    GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_withAddOffset, "Back", EBuildingSide.Back, "WCB", "Cladding - Back Wall", MaterialCladding_Wall, pback1_baseright, ColorNameWall,
                    CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fFrontCladdingOpacity,
                    WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, baseWidth_Overall_for3D,
                    WallProps.widthRib_m, WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_2_final_edge_FB_Wall,
                    listOfOpeningsBackWall_All, ref iSheetIndex, out listOfCladdingSheetsBackWall);
                else //(bGenerateBackSideCladding && !bIndividualCladdingSheets)
                {
                    double height_right_basic = height_left_basic;
                    double height_middle_basic = -bottomEdge_z + height_2_final_edge_FB_Wall;

                    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                    {
                        height_right_basic = -bottomEdge_z + height_1_final_edge_FB_Wall;
                        height_middle_basic = 0.5 * (height_left_basic + height_right_basic);
                    }

                    listOfCladdingSheetsBackWall = new List<CCladdingOrFibreGlassSheet>();
                    listOfCladdingSheetsBackWall.Add(new CCladdingOrFibreGlassSheet(4, "WCB", "Cladding - Back Wall", EBuildingSide.Back, MaterialCladding_Wall,
                        WallProps.thicknessCore_m, WallCoilProps.widthCoil, WallCoilProps.coilmass_kg_m2, WallCoilProps.price_PPSM_NZD, WallProps.widthModular_m,
                        iNumberOfFrontBackWallEdges, 0, 0,
                        pback1_baseright, baseWidth_Overall_for3D, height_left_basic, height_right_basic, 0.5 * baseWidth_Overall_for3D, height_middle_basic,
                        ColorNameWall, CladdingShape_Wall, CladdingCoatingType_Wall, ColorWall, options.fFrontCladdingOpacity, WallProps.widthRib_m, options.bDisplayCladdingBackWall, 0, false));

                }
            }

            // TODO - vstup z main Window
            float RoofLength_Y = (float)(fRoofEdgeOffsetFromCenterline + sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_withAddOffset + roofEdgeOverhang_Y);

            // Roof - Right Side

            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
                dRoofSide_length = Drawing3D.GetPoint3DDistanceDouble(pRoof_front2_heightright, pRoof_front4_top);
            else
                dRoofSide_length = Drawing3D.GetPoint3DDistanceDouble(pRoof_front3_heightleft, pRoof_front2_heightright);

            pControlPoint_RoofRight = new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z);

            iNumberOfWholeSheets = (int)(RoofLength_Y / RoofProps.widthModular_m);
            dWidthOfWholeSheets = iNumberOfWholeSheets * RoofProps.widthModular_m;
            dPartialSheet_End = RoofLength_Y - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            if (bGenerateRoofFibreglass)
            {
                listOfFibreGlassSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

                foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                {
                    if (fgsp.Side == "Roof" || fgsp.Side == "Roof-Right Side")
                    {
                        CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "RF", "Fibreglass - Roof-Right Side", fgsp.Location, MaterialFibreglass_Roof,
                             RoofProps_FG.thickness_m, RoofProps_FG.widthCoil_m, RoofProps_FG.flatsheet_mass_kg_m2, RoofProps_FG.price_PPSM_NZD * RoofProps_FG.widthModular_m / RoofProps_FG.widthCoil_m, RoofProps_FG.widthModular_m,
                             iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                             pControlPoint_RoofRight, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : RoofProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
                             ColorNameRoof_FG, CladdingShape_Roof_FG, CladdingCoatingType_Roof_FG, ColorRoof_FG, options.fFibreglassOpacity, RoofProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
                        listOfFibreGlassOpenings.Add(sheet);

                        if (!sheet.HasOverlap && IsAnyCanopyOnSide(fgsp.Location, sheet.ConvertToOpening(), column_crsc_y_minus_withAddOffset, column_crsc_y_plus_withAddOffset, column_crsc_z_plus_withAddOffset, fRoofEdgeOffsetFromCenterline, dRoofSide_length))
                            sheet.HasOverlap = true;

                        List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                        listOfFibreGlassSheetsRoofRight.AddRange(sheets);
                        iSheet_FG_Index += sheets.Count;
                    }
                }
            }

            bool isOrigSheetLast = true;
            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && sBuildingGeomInputData.fRoofPitch_deg > 0) isOrigSheetLast = false;

            if (bGenerateRoofCladding)
            {
                if (bIndividualCladdingSheets)
                {
                    EBuildingSide sheetLoc = EBuildingSide.Roof_Right_Side;
                    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed) sheetLoc = EBuildingSide.Roof;

                    GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_withAddOffset, "Roof-right", sheetLoc, "RC", "Cladding - Roof-Right Side", MaterialCladding_Roof, pControlPoint_RoofRight, ColorNameRoof,
                    CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity,
                    RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofLength_Y,
                    RoofProps.widthRib_m, RoofProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, dRoofSide_length, dRoofSide_length,
                    SheetListToOpeningListConverter(listOfFibreGlassOpenings), ref iSheetIndex, out listOfCladdingSheetsRoofRight);

                    // TODO - upravit plechy pre canopies
                    if (canopyCollection != null)
                    {
                        for (int i = 0; i < listOfCladdingSheetsRoofRight.Count; i++)
                        {
                            CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofRight[i];
                            int cIndex = 0;
                            int breakIndex = canopyCollection.Count;
                            foreach (CCanopiesInfo canopy in canopyCollection)
                            {
                                double CanopyCladdingWidth_Right = 0;
                                if (RightSheetNeedsToBeExtendedToCanopy(originalsheet, canopy, column_crsc_y_minus_withAddOffset, column_crsc_y_plus_withAddOffset, column_crsc_z_plus_withAddOffset, fRoofEdgeOffsetFromCenterline, out CanopyCladdingWidth_Right))
                                {

                                    // Tu sa upravia dlzky pre sheet ktory zasahuje do canopy
                                    // Je potrebne nasledne este sheet rozdelit podla jeho maximalnej dlzky a rozdelenym sheet nastavit overlap (okrem krajneho)
                                    originalsheet.CoordinateInPlane_y = 0;
                                    originalsheet.CoordinateInPlane_y -= CanopyCladdingWidth_Right;
                                    originalsheet.LengthTopLeft += CanopyCladdingWidth_Right;
                                    originalsheet.LengthTopRight += CanopyCladdingWidth_Right;
                                    //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                    originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);
                                    originalsheet.Update();

                                    CutCanopySheet(originalsheet, false, ref iSheetIndex, dRoofSide_length, isOrigSheetLast);

                                    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed || (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && !canopy.Left))
                                        breakIndex = cIndex + 1;
                                }

                                // Pre monopitch upravujeme aj lavu stranu plechu
                                if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                                {
                                    double CanopyCladdingWidth_Left = 0;
                                    if (LeftSheetNeedsToBeExtendedToCanopy(originalsheet, canopy, column_crsc_y_minus_withAddOffset, column_crsc_y_plus_withAddOffset, column_crsc_z_plus_withAddOffset, fRoofEdgeOffsetFromCenterline, dRoofSide_length, out CanopyCladdingWidth_Left))
                                    {
                                        //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth; // Ostava povodne
                                        originalsheet.LengthTopLeft += CanopyCladdingWidth_Left;
                                        originalsheet.LengthTopRight += CanopyCladdingWidth_Left;
                                        //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                        originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);
                                        originalsheet.Update();

                                        CutCanopySheet(originalsheet, false, ref iSheetIndex, dRoofSide_length, isOrigSheetLast);

                                        breakIndex = cIndex + 1;
                                    }
                                }
                                if (cIndex == breakIndex) break;
                                cIndex++;
                            }
                        }
                    }
                }
                else
                {
                    listOfCladdingSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();
                    listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(5, "RC", "Cladding - Roof", EBuildingSide.Roof, MaterialCladding_Roof,
                        RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofProps.widthModular_m,
                        4, 0, 0,
                        pControlPoint_RoofRight, RoofLength_Y, dRoofSide_length, dRoofSide_length, 0.5 * RoofLength_Y, dRoofSide_length,
                        ColorNameRoof, CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity, RoofProps.widthRib_m, options.bDisplayCladdingRoof, 0, false));

                    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                    {
                        // Canopies
                        foreach (CCanopiesInfo canopy in canopyCollection)
                        {
                            double width_temp;
                            int iAreaIndex = 6;

                            if (canopy.Right)
                            {
                                bool hasNextCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                                bool hasPreviousCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                                float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_withAddOffset); // Positive value
                                float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_withAddOffset);

                                float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                                float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                                float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
                                int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / RoofProps.widthRib_m);
                                double dWidthOfWholeRibs = iNumberOfWholeRibs * RoofProps.widthRib_m;
                                double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                                float fCanopyCladdingWidth = (float)canopy.WidthRight + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X;
                                float fCanopy_EdgeCoordinate_z = (float)height_2_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                                Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_2_final_edge_Roof);
                                Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_2_final_edge_Roof);
                                Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_withAddOffset + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
                                Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_withAddOffset + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

                                width_temp = pback_left.Y - pfront_left.Y;

                                listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof, MaterialCladding_Roof,
                                    RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofProps.widthModular_m,
                                    4, 0, 0,
                                    pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
                                    ColorNameRoof, CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity, RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

                                iAreaIndex++;
                            }

                            if (canopy.Left)
                            {
                                // 2 ______ 1
                                //  |      |
                                //  |      |
                                //  |______|
                                // 3        0

                                bool hasNextCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                                bool hasPreviousCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                                float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_withAddOffset); // Positive value
                                float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_withAddOffset);

                                float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                                float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                                float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
                                int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / RoofProps.widthRib_m);
                                double dWidthOfWholeRibs = iNumberOfWholeRibs * RoofProps.widthRib_m;
                                double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                                float fCanopyCladdingWidth = (float)canopy.WidthLeft + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X;
                                float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                                Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                                Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                                Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
                                Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne                                
                                width_temp = pback_left.Y - pfront_left.Y;

                                listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof, MaterialCladding_Roof,
                                    RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofProps.widthModular_m,
                                    4, 0, 0,
                                    pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
                                    ColorNameRoof, CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity, RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

                                iAreaIndex++;
                            }
                        }
                    }
                }
            }

            // Roof - Left Side

            listOfFibreGlassSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();

            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Roof - Left Side
                pControlPoint_RoofLeft = new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z);
                iNumberOfWholeSheets = (int)(RoofLength_Y / RoofProps.widthModular_m);
                dWidthOfWholeSheets = iNumberOfWholeSheets * RoofProps.widthModular_m;
                dPartialSheet_End = RoofLength_Y - dWidthOfWholeSheets; // Last Sheet
                iNumberOfSheets = iNumberOfWholeSheets + 1;

                if (bGenerateRoofFibreglass)
                {
                    listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

                    foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                    {
                        if (fgsp.Side == "Roof-Left Side")
                        {
                            dRoofSide_length = Drawing3D.GetPoint3DDistanceDouble(pRoof_front3_heightleft, pRoof_front4_top);

                            // Pre Left side prevratime suradnice v LCS y, aby boli vstupy na oboch stranach brane od spodnej hrany H1
                            double Position_y = dRoofSide_length - fgsp.Y - fgsp.Length;

                            CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "RF", "Fibreglass - Roof-Left Side", fgsp.Location, MaterialFibreglass_Roof,
                                RoofProps_FG.thickness_m, RoofProps_FG.widthCoil_m, RoofProps_FG.flatsheet_mass_kg_m2, RoofProps_FG.price_PPSM_NZD * RoofProps_FG.widthModular_m / RoofProps_FG.widthCoil_m, RoofProps_FG.widthModular_m,
                                iNumberOfEdges_FG_D_W, fgsp.X, Position_y,
                                pControlPoint_RoofLeft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : RoofProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
                                ColorNameRoof_FG, CladdingShape_Roof_FG, CladdingCoatingType_Roof_FG, ColorRoof_FG, options.fFibreglassOpacity, RoofProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
                            listOfFibreGlassOpenings.Add(sheet);

                            if (!sheet.HasOverlap && IsAnyCanopyOnSide(fgsp.Location, sheet.ConvertToOpening(), column_crsc_y_minus_withAddOffset, column_crsc_y_plus_withAddOffset, column_crsc_z_plus_withAddOffset, fRoofEdgeOffsetFromCenterline, dRoofSide_length))
                                sheet.HasOverlap = true;

                            List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                            listOfFibreGlassSheetsRoofLeft.AddRange(sheets);
                            iSheet_FG_Index += sheets.Count;
                        }
                    }
                }

                if (bGenerateRoofCladding)
                {
                    if (bIndividualCladdingSheets)
                    {
                        GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_withAddOffset, "Roof-left", EBuildingSide.Roof_Left_Side, "RC", "Cladding - Roof-Left Side", MaterialCladding_Roof,
                        pControlPoint_RoofLeft, ColorNameRoof,
                        CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity,
                        RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD,
                        RoofLength_Y,
                        RoofProps.widthRib_m, RoofProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, dRoofSide_length, dRoofSide_length,
                        SheetListToOpeningListConverter(listOfFibreGlassOpenings), ref iSheetIndex, out listOfCladdingSheetsRoofLeft);

                        // TODO - upravit plechy pre canopies
                        if (canopyCollection != null)
                        {
                            for (int i = 0; i < listOfCladdingSheetsRoofLeft.Count; i++)
                            {
                                CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofLeft[i];

                                int cIndex = 0;
                                int breakIndex = canopyCollection.Count;
                                foreach (CCanopiesInfo canopy in canopyCollection)
                                {
                                    double CanopyCladdingWidth_Left = 0;
                                    if (LeftSheetNeedsToBeExtendedToCanopy(originalsheet, canopy, column_crsc_y_minus_withAddOffset, column_crsc_y_plus_withAddOffset, column_crsc_z_plus_withAddOffset, fRoofEdgeOffsetFromCenterline, dRoofSide_length, out CanopyCladdingWidth_Left))
                                    {
                                        //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth; // Ostava povodne
                                        originalsheet.LengthTopLeft += CanopyCladdingWidth_Left;
                                        originalsheet.LengthTopRight += CanopyCladdingWidth_Left;
                                        //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                        originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);
                                        originalsheet.Update();

                                        CutCanopySheet(originalsheet, true, ref iSheetIndex, dRoofSide_length, isOrigSheetLast);

                                        breakIndex = cIndex + 1;
                                    }

                                    if (cIndex == breakIndex) break;
                                    cIndex++;
                                }

                            }
                        }
                    }
                    else
                    {
                        listOfCladdingSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();
                        listOfCladdingSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(6, "RC", "Cladding - Roof-Left Side", EBuildingSide.Roof_Left_Side, MaterialCladding_Roof,
                            RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofProps.widthModular_m,
                            4, 0, 0,
                            pControlPoint_RoofLeft, RoofLength_Y, dRoofSide_length, dRoofSide_length, 0.5 * RoofLength_Y, dRoofSide_length,
                            ColorNameRoof, CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity, RoofProps.widthRib_m, options.bDisplayCladdingRoof, 0, false));

                        // Canopies
                        foreach (CCanopiesInfo canopy in canopyCollection)
                        {
                            int iAreaIndex = 7;

                            double width_temp;

                            if (canopy.Right)
                            {
                                bool hasNextCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                                bool hasPreviousCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                                float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_withAddOffset); // Positive value
                                float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_withAddOffset);

                                float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                                float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                                float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
                                int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / RoofProps.widthRib_m);
                                double dWidthOfWholeRibs = iNumberOfWholeRibs * RoofProps.widthRib_m;
                                double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                                float fCanopyCladdingWidth = (float)canopy.WidthRight + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X;
                                float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                                Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_1_final_edge_Roof);
                                Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_1_final_edge_Roof);
                                Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_withAddOffset + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
                                Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_withAddOffset + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y);
                                width_temp = pback_left.Y - pfront_left.Y;

                                listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof_Right_Side, MaterialCladding_Roof,
                                    RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofProps.widthModular_m,
                                    4, 0, 0,
                                    pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
                                    ColorNameRoof, CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity, RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

                                iAreaIndex++;
                            }

                            if (canopy.Left)
                            {
                                bool hasNextCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                                bool hasPreviousCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                                float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_withAddOffset); // Positive value
                                float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_withAddOffset);

                                float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                                float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                                float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
                                int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / RoofProps.widthRib_m);
                                double dWidthOfWholeRibs = iNumberOfWholeRibs * RoofProps.widthRib_m;
                                double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                                float fCanopyCladdingWidth = (float)canopy.WidthLeft + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X;
                                float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                                Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                                Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_withAddOffset - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                                Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
                                Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne                                
                                width_temp = pback_left.Y - pfront_left.Y;

                                listOfCladdingSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof_Left_Side, MaterialCladding_Roof,
                                    RoofProps.thicknessCore_m, RoofCoilProps.widthCoil, RoofCoilProps.coilmass_kg_m2, RoofCoilProps.price_PPSM_NZD, RoofProps.widthModular_m,
                                    4, 0, 0,
                                    pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
                                    ColorNameRoof, CladdingShape_Roof, CladdingCoatingType_Roof, ColorRoof, options.fRoofCladdingOpacity, RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

                                iAreaIndex++;
                            }
                        }
                    }
                }
            }

            CountRealLenghts();
        }

        public Model3DGroup GetCladdingModel(DisplayOptions options)
        {
            Model3DGroup model_gr = new Model3DGroup();
            WireFramePoints = new List<Point3D>();

            Brush solidBrushFront = new SolidColorBrush(ColorWall);
            Brush solidBrushSide = new SolidColorBrush(ColorWall);
            Brush solidBrushRoof = new SolidColorBrush(ColorRoof);
            Brush solidBrushWall_FG = new SolidColorBrush(ColorWall_FG);
            Brush solidBrushRoof_FG = new SolidColorBrush(ColorRoof_FG);

            solidBrushFront.Opacity = options.fFrontCladdingOpacity;
            solidBrushSide.Opacity = options.fLeftCladdingOpacity;
            solidBrushRoof.Opacity = options.fRoofCladdingOpacity;
            solidBrushWall_FG.Opacity = options.fFibreglassOpacity;
            solidBrushRoof_FG.Opacity = options.fFibreglassOpacity;

            DiffuseMaterial material_SideWall = new DiffuseMaterial(solidBrushSide); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_FrontBackWall = new DiffuseMaterial(solidBrushFront); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_Roof = new DiffuseMaterial(solidBrushRoof); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_Wall_FG = new DiffuseMaterial(solidBrushWall_FG);
            DiffuseMaterial material_Roof_FG = new DiffuseMaterial(solidBrushRoof_FG);

            ImageBrush brushFront = null;
            ImageBrush brushSide = null;
            ImageBrush brushRoof = null;
            ImageBrush brushWall_FG = null;
            ImageBrush brushRoof_FG = null;

            if (options.bUseTextures && options.bUseTexturesCladding)
            {
                //CS ENDURA® a CS MAXX® maju rovnake farby, takze pre nich coating type v nazve suboru nepouzivam
                string uriString_Wall = "pack://application:,,,/Resources/Textures/" + CladdingShape_Wall + "/" + CladdingShape_Wall + "_" + ColorNameWall + ".jpg";

                if (CladdingCoatingType_Wall == "FORMCLAD®")
                {
                    string claddingCoatingType_Wall_string = "FORMCLAD";
                    uriString_Wall = "pack://application:,,,/Resources/Textures/" + CladdingShape_Wall + "/" + CladdingShape_Wall + "_" + claddingCoatingType_Wall_string + "_" + ColorNameWall + ".jpg";
                }

                brushFront = new ImageBrush();
                brushFront.ImageSource = new BitmapImage(new Uri(uriString_Wall, UriKind.RelativeOrAbsolute));
                brushFront.TileMode = TileMode.Tile;
                brushFront.ViewportUnits = BrushMappingMode.Absolute;
                brushFront.Stretch = Stretch.Fill;
                brushFront.Opacity = options.fFrontCladdingOpacity;

                brushSide = new ImageBrush();
                brushSide.ImageSource = new BitmapImage(new Uri(uriString_Wall, UriKind.RelativeOrAbsolute));
                brushSide.TileMode = TileMode.Tile;
                brushSide.ViewportUnits = BrushMappingMode.Absolute;
                brushSide.Stretch = Stretch.Fill;
                brushSide.Opacity = options.fLeftCladdingOpacity;

                // CS ENDURA® a CS MAXX® maju rovnake farby, takze pre nich coating type v nazve suboru nepouzivam
                string uriString_Roof = "pack://application:,,,/Resources/Textures/" + CladdingShape_Roof + "/" + CladdingShape_Roof + "_" + ColorNameRoof + ".jpg";

                if (CladdingCoatingType_Roof == "FORMCLAD®")
                {
                    string claddingCoatingType_Roof_string = "FORMCLAD";
                    uriString_Roof = "pack://application:,,,/Resources/Textures/" + CladdingShape_Roof + "/" + CladdingShape_Roof + "_" + claddingCoatingType_Roof_string + "_" + ColorNameRoof + ".jpg";
                }

                brushRoof = new ImageBrush();
                brushRoof.ImageSource = new BitmapImage(new Uri(uriString_Roof, UriKind.RelativeOrAbsolute));
                brushRoof.TileMode = TileMode.Tile;
                brushRoof.ViewportUnits = BrushMappingMode.Absolute;
                brushRoof.Stretch = Stretch.Fill;
                brushRoof.Opacity = options.fRoofCladdingOpacity;

                // Fibreglass
                //string uriString_FG_Wall = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + m_ColorNameWall_FG + ".jpg";
                string uriString_FG_Wall = "pack://application:,,,/Resources/Textures/Fibreglass/" + CladdingShape_Wall + "/" + CladdingShape_Wall + "_" + "White" + ".jpg";

                brushWall_FG = new ImageBrush();
                brushWall_FG.ImageSource = new BitmapImage(new Uri(uriString_FG_Wall, UriKind.RelativeOrAbsolute));
                brushWall_FG.TileMode = TileMode.Tile;
                brushWall_FG.ViewportUnits = BrushMappingMode.Absolute;
                brushWall_FG.Stretch = Stretch.Fill;
                brushWall_FG.Opacity = options.fFibreglassOpacity;

                //string uriString_FG_Roof = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + m_ColorNameRoof_FG + ".jpg";
                string uriString_FG_Roof = "pack://application:,,,/Resources/Textures/Fibreglass/" + CladdingShape_Roof + "/" + CladdingShape_Roof + "_" + "White" + ".jpg";

                brushRoof_FG = new ImageBrush();
                brushRoof_FG.ImageSource = new BitmapImage(new Uri(uriString_FG_Roof, UriKind.RelativeOrAbsolute));
                brushRoof_FG.TileMode = TileMode.Tile;
                brushRoof_FG.ViewportUnits = BrushMappingMode.Absolute;
                brushRoof_FG.Stretch = Stretch.Fill;
                brushRoof_FG.Opacity = options.fFibreglassOpacity;
            }

            // Vytvorime geometry model
            double outOffPlaneOffset_FG = -0.005; // Pokial kreslime cladding ako jednoliatu plochu na celu stenu alebo strechu, nastavime offset, aby sa fibreglasa nevnarali do cladding
            if (bIndividualCladdingSheets)
            {
                // Ak kreslime individualne sheets pre cladding nepotrebujeme offset
                outOffPlaneOffset_FG = 0.000;
            }

            //bool claddingWireframe = options.bDisplayWireFrameModel && options.bDisplayCladdingWireFrame;
            bool fibreglassWireframe = options.bDisplayWireFrameModel && options.bDisplayCladdingWireFrame && options.bDisplayFibreglassWireFrame;

            if (bGenerateLeftSideCladding && options.bDisplayCladdingLeftWall)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsLeftWall, options, brushSide, material_SideWall, WallProps.widthRib_m, 0, 0, -90, ref model_gr);

            if (bGenerateLeftSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallLeft, options, brushWall_FG, material_Wall_FG, WallProps_FG.widthRib_m, 0, 0, -90, ref model_gr, outOffPlaneOffset_FG);

            if (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsFrontWall, options, brushFront, material_FrontBackWall, WallProps.widthRib_m, 0, 0, 0, ref model_gr);

            if (bGenerateFrontSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallFront, options, brushWall_FG, material_Wall_FG, WallProps_FG.widthRib_m, 0, 0, 0, ref model_gr, outOffPlaneOffset_FG);

            if (bGenerateRightSideCladding && options.bDisplayCladdingRightWall)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsRightWall, options, brushSide, material_SideWall, WallProps.widthRib_m, 0, 0, 90, ref model_gr);

            if (bGenerateRightSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallRight, options, brushWall_FG, material_Wall_FG, WallProps_FG.widthRib_m, 0, 0, 90, ref model_gr, outOffPlaneOffset_FG);

            if (bGenerateBackSideCladding && options.bDisplayCladdingBackWall)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsBackWall, options, brushFront, material_FrontBackWall, WallProps.widthRib_m, 0, 0, 180, ref model_gr);

            if (bGenerateBackSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallBack, options, brushWall_FG, material_Wall_FG, WallProps_FG.widthRib_m, 0, 0, 180, ref model_gr, outOffPlaneOffset_FG);

            // Set rotation about GCS X-axis - Roof - Right Side (Gable Roof) and Monopitch Roof
            double rotationAboutX = -90f + (eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? sBuildingGeomInputData.fRoofPitch_deg : -sBuildingGeomInputData.fRoofPitch_deg);

            if (bGenerateRoofCladding && options.bDisplayCladdingRoof)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsRoofRight, options, brushRoof, material_Roof, RoofProps.widthRib_m, rotationAboutX, 0, 90, ref model_gr);

            if (bGenerateRoofFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsRoofRight, options, brushRoof_FG, material_Roof_FG, RoofProps_FG.widthRib_m, rotationAboutX, 0, 90, ref model_gr, outOffPlaneOffset_FG);

            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Set rotation about GCS X-axis - Roof - Left Side (Gable Roof)
                rotationAboutX = -90f - sBuildingGeomInputData.fRoofPitch_deg;

                if (bGenerateRoofCladding && options.bDisplayCladdingRoof)
                    AddSheet3DModelsToModelGroup(listOfCladdingSheetsRoofLeft, options, brushRoof, material_Roof, RoofProps.widthRib_m, rotationAboutX, 0, 90, ref model_gr);

                if (bGenerateRoofFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
                    AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsRoofLeft, options, brushRoof_FG, material_Roof_FG, RoofProps_FG.widthRib_m, rotationAboutX, 0, 90, ref model_gr, outOffPlaneOffset_FG);
            }

            return model_gr;
        }


        public bool RightSheetNeedsToBeExtendedToCanopy(CCladdingOrFibreGlassSheet originalsheet, CCanopiesInfo canopy, double column_crsc_y_minus_temp, double column_crsc_y_plus_temp, double column_crsc_z_plus_temp, float fRoofEdgeOffsetFromCenterline, out double maxCanopyCladdingWidth_Right)
        {
            maxCanopyCladdingWidth_Right = 0;
            bool needsToBeExtended = false;

            bool hasNextCanopyRight = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
            bool hasPreviousCanopyRight = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

            float fCanopyBayStartOffsetRight = hasPreviousCanopyRight ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
            float fCanopyBayEndOffsetRight = hasNextCanopyRight ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

            float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetRight;
            float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetRight;

            // Musime menit len tie sheets ktore maju zaciatok na hrane strechy
            if (MATH.MathF.d_equal(originalsheet.CoordinateInPlane_y, 0, 0.002))
            {
                // Zistime ci je plocha originalsheet v kolizii s nejakym canopy - right
                // Myslim ze mame niekde uz funkcie ktore vedia skontrolovat ci sa dve plochy prekryvaju

                // Zistime ci je canopy v kolizii s plechom
                // Ak ano upravime koncove lokalne suradnice plechu y na suradnice canopy a nastavime nove dlzky plechu
                if (canopy.Right && (
                   (fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= originalsheet.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                   (fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                   fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                   (fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width))))
                {
                    double CanopyCladdingWidth_Right = canopy.WidthRight + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X;
                    maxCanopyCladdingWidth_Right = CanopyCladdingWidth_Right;
                    needsToBeExtended = true;
                }

                if (hasNextCanopyRight)
                {
                    CCanopiesInfo canopyNext = canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1); //go to next canopy
                    hasNextCanopyRight = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopyNext.BayIndex + 1));
                    hasPreviousCanopyRight = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopyNext.BayIndex - 1));

                    fCanopyBayStartOffsetRight = hasPreviousCanopyRight ? 0f : ((canopyNext.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                    fCanopyBayEndOffsetRight = hasNextCanopyRight ? 0f : (((canopyNext.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                    fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopyNext.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetRight;
                    fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopyNext.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetRight;

                    if (canopyNext.Right && (
                       (fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= originalsheet.CoordinateInPlane_x &&
                       fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                       (fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                       fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                       (fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                       fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width))))
                    {
                        double CanopyCladdingWidth_Right = canopyNext.WidthRight + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X;
                        if (maxCanopyCladdingWidth_Right < CanopyCladdingWidth_Right) maxCanopyCladdingWidth_Right = CanopyCladdingWidth_Right;
                    }
                }
            }
            return needsToBeExtended;
        }

        public bool LeftSheetNeedsToBeExtendedToCanopy(CCladdingOrFibreGlassSheet originalsheet, CCanopiesInfo canopy, double column_crsc_y_minus_temp, double column_crsc_y_plus_temp, double column_crsc_z_plus_temp, float fRoofEdgeOffsetFromCenterline, double length_left_basic, out double maxCanopyCladdingWidth_Left)
        {
            maxCanopyCladdingWidth_Left = 0;
            bool needsToBeExtended = false;

            bool hasNextCanopyLeft = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
            bool hasPreviousCanopyLeft = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

            float fCanopyBayStartOffsetLeft = hasPreviousCanopyLeft ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
            float fCanopyBayEndOffsetLeft = hasNextCanopyLeft ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

            float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetLeft;
            float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetLeft;

            // Musime menit len tie sheets ktore maju koniec na hrane strechy
            if (MATH.MathF.d_equal(originalsheet.CoordinateInPlane_y + originalsheet.LengthTotal, length_left_basic, 0.002))
            {
                // Zistime ci je canopy v kolizii s plechom
                // Ak ano upravime koncove lokalne suradnice plechu y na suradnice canopy a nastavime nove dlzky plechu
                if (canopy.Left && (
                   (fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= originalsheet.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                   (fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                   fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                   (fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width))))
                {
                    double CanopyCladdingWidth_Left = canopy.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X;
                    maxCanopyCladdingWidth_Left = CanopyCladdingWidth_Left;
                    needsToBeExtended = true;
                }

                if (hasNextCanopyLeft)
                {
                    CCanopiesInfo canopyNext = canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1); //go to next canopy
                    hasNextCanopyLeft = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopyNext.BayIndex + 1));
                    hasPreviousCanopyLeft = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopyNext.BayIndex - 1));

                    fCanopyBayStartOffsetLeft = hasPreviousCanopyLeft ? 0f : ((canopyNext.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                    fCanopyBayEndOffsetLeft = hasNextCanopyLeft ? 0f : (((canopyNext.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                    fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopyNext.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetLeft;
                    fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopyNext.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetLeft;

                    if (canopyNext.Left && (
                   (fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= originalsheet.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                   (fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                   fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                   (fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= originalsheet.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= (originalsheet.CoordinateInPlane_x + originalsheet.Width))))
                    {
                        double CanopyCladdingWidth_Left = canopyNext.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X;
                        if (maxCanopyCladdingWidth_Left < CanopyCladdingWidth_Left) maxCanopyCladdingWidth_Left = CanopyCladdingWidth_Left;
                    }
                }
            }
            return needsToBeExtended;
        }

        public bool IsAnyCanopyOnSide(EBuildingSide side, COpening opening, double column_crsc_y_minus_temp, double column_crsc_y_plus_temp, double column_crsc_z_plus_temp, float fRoofEdgeOffsetFromCenterline, double length_left_basic)
        {
            foreach (CCanopiesInfo canopy in canopyCollection)
            {
                if (IsCanopyOnSide(side, opening, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, length_left_basic))
                    return true;
            }
            return false;
        }
        private bool IsCanopyOnSide(EBuildingSide side, COpening opening, CCanopiesInfo canopy, double column_crsc_y_minus_temp, double column_crsc_y_plus_temp, double column_crsc_z_plus_temp, float fRoofEdgeOffsetFromCenterline, double length_left_basic)
        {
            if (side == EBuildingSide.Roof_Right_Side) return IsCanopyOnRight(opening, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline);
            else if (side == EBuildingSide.Roof_Left_Side) return IsCanopyOnLeft(opening, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, length_left_basic);
            else if (side == EBuildingSide.Roof)
            {
                if (MathF.d_equal(opening.CoordinateInPlane_y, 0, 0.002))
                {
                    return IsCanopyOnRight(opening, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline);
                }
                return IsCanopyOnLeft(opening, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, length_left_basic);
            }
            else return false;  //throw new Exception("Error in IsCanopyOnSide.");

        }

        private bool IsCanopyOnRight(COpening opening, CCanopiesInfo canopy, double column_crsc_y_minus_temp, double column_crsc_y_plus_temp, double column_crsc_z_plus_temp, float fRoofEdgeOffsetFromCenterline)
        {
            bool hasNextCanopyRight = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
            bool hasPreviousCanopyRight = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

            float fCanopyBayStartOffsetRight = hasPreviousCanopyRight ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
            float fCanopyBayEndOffsetRight = hasNextCanopyRight ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

            float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetRight;
            float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetRight;

            // Musime menit len tie sheets ktore maju zaciatok na hrane strechy
            if (MathF.d_equal(opening.CoordinateInPlane_y, 0, 0.002))
            {
                // Zistime ci je plocha originalsheet v kolizii s nejakym canopy - right
                if (canopy.Right && (
                   (fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= opening.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= (opening.CoordinateInPlane_x + opening.Width)) ||
                   (fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= opening.CoordinateInPlane_x &&
                   fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= (opening.CoordinateInPlane_x + opening.Width)) ||
                   (fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline >= opening.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline <= (opening.CoordinateInPlane_x + opening.Width))))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsCanopyOnLeft(COpening opening, CCanopiesInfo canopy, double column_crsc_y_minus_temp, double column_crsc_y_plus_temp, double column_crsc_z_plus_temp, float fRoofEdgeOffsetFromCenterline, double length_left_basic)
        {
            bool hasNextCanopyLeft = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
            bool hasPreviousCanopyLeft = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

            float fCanopyBayStartOffsetLeft = hasPreviousCanopyLeft ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
            float fCanopyBayEndOffsetLeft = hasNextCanopyLeft ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

            float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetLeft;
            float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetLeft;

            // Musime menit len tie sheets ktore maju koniec na hrane strechy
            if (MATH.MathF.d_equal(opening.CoordinateInPlane_y + opening.LengthTotal, length_left_basic, 0.002))
            {
                // Zistime ci je canopy v kolizii s plechom
                if (canopy.Left && (
                   (fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= opening.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= (opening.CoordinateInPlane_x + opening.Width)) ||
                   (fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= opening.CoordinateInPlane_x &&
                   fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= (opening.CoordinateInPlane_x + opening.Width)) ||
                   (fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline >= opening.CoordinateInPlane_x &&
                   fBayEndCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline <= (opening.CoordinateInPlane_x + opening.Width))))
                {
                    return true;
                }
            }
            return false;
        }

        public void GenerateCladdingSheets(bool bCladdingSheetColoursByID,
            double column_crsc_y_minus_temp,
            string side,
            EBuildingSide location,
            string prefix,
            string name,
            MATERIAL.CMat material, // Vseobecny lebo FG nemusi byt steel
            Point3D pControlPoint,
            string colorName,
            string claddingShape,
            string claddingCoatingType,
            Color color,
            float fOpacity,
            double thicknessCore_m,
            double widthCoil,
            double coilMass_kg_m2,
            double coilPrice_PPSM_NZD,
            double width,
            double claddingWidthRibModular,
            double claddingWidthModular,
            //int iNumberOfWholeSheets,
            int iNumberOfOriginalSheetsOnSide,
            //double dWidthOfWholeSheets,
            double dLastSheetWidth,
            double height_left_basic,
            double height_middle_basic_aux, // height_2_final_edge_FB_Wall
            List<COpening> listOfOpenings,
            ref int iSheetIndex, out List<CCladdingOrFibreGlassSheet> listOfSheets)
        {
            listOfSheets = new List<CCladdingOrFibreGlassSheet>(); // Pole kombinovane z povodnych aj nadelenych sheets
            double eq_limit = 0.001; // 1 mm

            // Roof edge offset from centerline in Y-direction
            float fRoofEdgeOffsetFromCenterline = -(float)column_crsc_y_minus_temp + (float)roofEdgeOverhang_Y;

            for (int i = 0; i < iNumberOfOriginalSheetsOnSide; i++)
            {
                // Zakladne hodnoty pre obdlznik
                int iNumberOfEdges = 4;
                // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                double height_left = height_left_basic;
                double height_right = height_left_basic;
                double height_toptip = height_left_basic;
                double tipCoordinate_x = 0.5 * (i == iNumberOfOriginalSheetsOnSide - 1 ? dLastSheetWidth : claddingWidthModular);

                if (side == "Front" || side == "Back")
                {
                    // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                    height_left = ModelHelper.GetVerticalCoordinate(side, eModelType, width, height_left_basic, i * claddingWidthModular, sBuildingGeomInputData.fRoofPitch_deg);
                    height_right = ModelHelper.GetVerticalCoordinate(side, eModelType, width, height_left_basic, (i + 1) * claddingWidthModular, sBuildingGeomInputData.fRoofPitch_deg);
                    height_toptip = 0.5 * (height_left + height_right);
                    tipCoordinate_x = 0.5 * claddingWidthModular;

                    if (i == iNumberOfOriginalSheetsOnSide - 1)
                    {
                        // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                        height_right = ModelHelper.GetVerticalCoordinate(side, eModelType, width, height_left_basic, width, sBuildingGeomInputData.fRoofPitch_deg);
                        height_toptip = 0.5 * (height_left + height_right);
                        tipCoordinate_x = 0.5 * dLastSheetWidth;
                    }

                    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed &&
                       i * claddingWidthModular < 0.5 * width &&
                       (i + 1) * claddingWidthModular > 0.5 * width)
                    {
                        iNumberOfEdges = 5;
                        // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                        height_toptip = -bottomEdge_z + height_middle_basic_aux; // Stred budovy gable roof - roof ridge
                        tipCoordinate_x = 0.5 * width - i * claddingWidthModular;
                    }
                }

                // Nastavime parametre originalneho sheet, v pripade ze sheet sa nedeli pouziju sa tieto parametre priamo pre vygenerovanie objektu
                // V pripade ze sheet sa deli, su tieto parametre pouzite pre generovanie objektov nadelenych plechov

                double originalsheetLengthTotal = Math.Max(Math.Max(height_left, height_right), height_toptip);

                int originalsheetNumberOfEdges = iNumberOfEdges;
                double originalsheetCoordinateInPlane_x = i * claddingWidthModular;
                double originalsheetCoordinateInPlane_y = 0;
                Point3D originalsheetControlPoint = pControlPoint;
                double originalsheetWidth = i == iNumberOfOriginalSheetsOnSide - 1 ? dLastSheetWidth : claddingWidthModular;
                double originalsheetLengthTopLeft = height_left;
                double originalsheetLengthTopRight = height_right;
                double originalsheetTipCoordinate_x = tipCoordinate_x;
                double originalsheetLengthTopTip = height_toptip;

                // Tieto parametre nepotrebujeme prevadzat lebo sa pre original sheet a nadele sheets nemenia, takze sa mozu pouzit priamo parametre funkcie
                //string originalsheetColorName = colorName;
                //string originalsheetCladdingShape = claddingShape;
                //string originalsheetCladdingCoatingType = claddingCoatingType;
                //Color originalsheetColor = color;
                //float originalsheetOpacity = fOpacity;
                //double originalsheetCladdingWidthRibModular = claddingWidthModular;
                //bool originalsheetIsDisplayed = true;
                //float originalsheetTime = 0;

                // listOfOpenings // TODO - zapracovat aj doors a windows, vyrobit vseobecneho predka pre fibreglassSheet, door, window, ktory bude reprezentovat otvor v cladding

                // 1. Zistime ci lezi originalsheet v rovine s nejakym objektom listOfOpenings
                // Bod 1 mozeme preskocit ak vieme na ktorej strane resp v ktorej stresnej rovine sa nachadzame, bod 1 teda ignorujeme, lebo do funkcie vstupuju uz roztriedene objekty podla stran

                // 2. Zistime ci je plocha originalsheet v kolizii s nejakym objektom listOfOpenings
                // Myslim ze mame niekde uz funkcie ktore vedia skontrolovat ci sa dve plochy prekryvaju

                // Zoznam objektov ktore su v kolizii
                double dLimit = 0.020; // 20 mm // Ak otvor zacina 20 mm za okrajom plechu a 20 mm pred koncom plechu, tak uvazujeme ze cely plech je otvorom rozdeleny 

                List<COpening> objectInColision_In_Local_x = null;

                if (listOfOpenings != null && listOfOpenings.Count > 0) // Nejake opening su zadefinovane, takze ma zmysel hladat kolizie
                    objectInColision_In_Local_x = listOfOpenings.Where(o => (o.CoordinateInPlane_x <= originalsheetCoordinateInPlane_x + dLimit && (o.CoordinateInPlane_x + o.Width) >= (originalsheetCoordinateInPlane_x - dLimit + originalsheetWidth))).ToList();


                // Ak neexistuju objekty v kolizii s originalsheet mozeme opustit funkciu
                if (objectInColision_In_Local_x == null || objectInColision_In_Local_x.Count == 0)
                {
                    CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheetIndex + 1, prefix, name, location, material,
                    thicknessCore_m, widthCoil, coilMass_kg_m2, coilPrice_PPSM_NZD, claddingWidthModular,
                    originalsheetNumberOfEdges, originalsheetCoordinateInPlane_x, originalsheetCoordinateInPlane_y,
                    originalsheetControlPoint, originalsheetWidth, originalsheetLengthTopLeft, originalsheetLengthTopRight, originalsheetTipCoordinate_x, originalsheetLengthTopTip,
                    colorName, claddingShape, claddingCoatingType, color, fOpacity, claddingWidthRibModular, true, 0, false);

                    List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
                    //CountRealLenghts(sheets, height_left_basic);
                    listOfSheets.AddRange(sheets);

                    // Nie je potrebne delit sheet - pridame teda "originalsheet"
                    //listOfSheets.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, prefix, name, material,
                    //thicknessCore_m, widthCoil, coilMass_kg_m2, coilPrice_PPSM_NZD, claddingWidthModular,
                    //originalsheetNumberOfEdges, originalsheetCoordinateInPlane_x, originalsheetCoordinateInPlane_y,
                    //originalsheetControlPoint, originalsheetWidth, originalsheetLengthTopLeft, originalsheetLengthTopRight, originalsheetTipCoordinate_x, originalsheetLengthTopTip,
                    //colorName, claddingShape, claddingCoatingType, color, fOpacity, claddingWidthRibModular, true, 0));
                    iSheetIndex += sheets.Count;

                    //  ---- real lengths su rovnake
                }
                else if (objectInColision_In_Local_x.Count == 1 &&
                         objectInColision_In_Local_x[0].CoordinateInPlane_y <= originalsheetCoordinateInPlane_y &&
                         objectInColision_In_Local_x[0].LengthTotal >= originalsheetLengthTotal) // Otvor je dlhsi ako cely povodny sheet, odstranime len originalsheet a nic nepridavame
                {
                    // TODO - este by sa mohlo stat ze openings je sice viac nadefinovanych priamo vedla seba, ale tvoria jeden velky otvor
                    // Asi je to nezmyselne zadanie, ale malo by to byt osetrene
                    // Do not add sheet to the list - whole original sheet is substituted by opening
                }
                else
                {
                    // Predpokladame ze samotne objekty v listOfOpenings sa neprekryvaju
                    // 3. Zoradime objekty podla lokalnej suradnice y
                    // Bug 745 - To Ondrej // Moze to vyzerat takto ze prava strana sa zoradi a potom sa to priradi do toho isteho zoznamu???
                    objectInColision_In_Local_x = objectInColision_In_Local_x.OrderBy(o => o.CoordinateInPlane_y).ToList();

                    // 4. Podla poctu objektov v objectInColision_In_Local_x a ich suradnic vieme na kolko casti budeme originalsheet delit
                    int iNumberOfNewSheets = objectInColision_In_Local_x.Count + 1;

                    bool isFibreglassFirst = false;
                    bool isFibreglassLast = false;

                    // Skontrolovat podla suradnic ci objekt zacina alebo konci priamo na hrane a podla toho upravit pocet novych, ktore treba vytvorit
                    foreach (COpening o in objectInColision_In_Local_x)
                    {
                        if (MathF.d_equal(o.CoordinateInPlane_y, 0, eq_limit) || MathF.d_equal(o.CoordinateInPlane_y + o.LengthTotal, height_left_basic, eq_limit))
                        {
                            bool isCanopyOnSide = IsAnyCanopyOnSide(location, o, Column_crsc_y_minus, Column_crsc_y_plus, Column_crsc_z_plus, fRoofEdgeOffsetFromCenterline, height_left_basic);

                            if (MathF.d_equal(o.CoordinateInPlane_y, 0, eq_limit) && !isCanopyOnSide) { iNumberOfNewSheets--; isFibreglassFirst = true; }
                            else if (MathF.d_equal(o.CoordinateInPlane_y + o.LengthTotal, height_left_basic, eq_limit) && !isCanopyOnSide) { iNumberOfNewSheets--; isFibreglassLast = true; }
                        }
                    }

                    List<CCladdingOrFibreGlassSheet> sheets = new List<CCladdingOrFibreGlassSheet>();
                    int openingIndex = 0;
                    bool hasOverlap = true;
                    // 5. Pridame nove sheets do zoznamu
                    for (int j = 0; j < iNumberOfNewSheets; j++)
                    {
                        if (location == EBuildingSide.Roof_Left_Side || //lava roof je proste speci
                            location == EBuildingSide.Roof && sBuildingGeomInputData.fRoofPitch_deg > 0) //monopitch roof a roofPitch deg > 0
                        {
                            if (j == iNumberOfNewSheets - 1 && !isFibreglassLast) hasOverlap = false;
                            else hasOverlap = true;
                        }
                        else
                        {
                            if (j == 0 && !isFibreglassFirst) hasOverlap = false;
                            else hasOverlap = true;
                        }

                        if (j == iNumberOfNewSheets - 1) // Last segment of original sheet or first segment of original sheet (Nastane vtedy ked zaciname s FG)
                        {
                            if (isFibreglassFirst || isFibreglassLast) { openingIndex = j; }
                            else openingIndex = j - 1;

                            double coordinateInPlane_y = objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y + objectInColision_In_Local_x[openingIndex].LengthTotal;
                            double lengthTopLeft = originalsheetLengthTopLeft - objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                            double lengthTopRight = originalsheetLengthTopRight - objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                            double lengthTopTip = originalsheetLengthTopTip - objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;

                            if (isFibreglassLast) // Koncime s FG sheet
                            {
                                iNumberOfEdges = 4;
                                coordinateInPlane_y = height_left_basic - objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                lengthTopLeft = originalsheetLengthTopLeft - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                lengthTopRight = originalsheetLengthTopRight - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                lengthTopTip = originalsheetLengthTopTip - objectInColision_In_Local_x[openingIndex].LengthTotal;

                                if (!isFibreglassFirst && objectInColision_In_Local_x.Count > 1)
                                {
                                    coordinateInPlane_y = objectInColision_In_Local_x[openingIndex - 1].CoordinateInPlane_y + objectInColision_In_Local_x[openingIndex - 1].LengthTotal;
                                    lengthTopLeft = originalsheetLengthTopLeft - coordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                    lengthTopRight = originalsheetLengthTopRight - coordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                    lengthTopTip = originalsheetLengthTopTip - coordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal; // ??? Pre reverzny smer je to asi nezmyselne
                                }

                                if (isFibreglassFirst && isFibreglassLast) // Dva otvory - first a last, jeden cladding sheet medzi nimi
                                {
                                    iNumberOfEdges = 4;
                                    coordinateInPlane_y = objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y + objectInColision_In_Local_x[openingIndex].LengthTotal;
                                    lengthTopLeft = objectInColision_In_Local_x[openingIndex + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                    lengthTopRight = objectInColision_In_Local_x[openingIndex + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                    lengthTopTip = objectInColision_In_Local_x[openingIndex + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                }
                            }

                            // To Ondrej, mrkni na tieto dve podmienky ci sa to neda zapisat nejako jednoduchsie
                            if (side == "Roof-left" && isFibreglassLast)
                            {
                                iNumberOfEdges = 4;
                                coordinateInPlane_y = height_left_basic - objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                lengthTopLeft = originalsheetLengthTopLeft - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                lengthTopRight = originalsheetLengthTopRight - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                lengthTopTip = originalsheetLengthTopTip - objectInColision_In_Local_x[openingIndex].LengthTotal; // ??? Pre reverzny smer je to asi nezmyselne

                                if (!isFibreglassFirst && objectInColision_In_Local_x.Count > 1)
                                {
                                    coordinateInPlane_y = objectInColision_In_Local_x[openingIndex - 1].CoordinateInPlane_y + objectInColision_In_Local_x[openingIndex - 1].LengthTotal;
                                    lengthTopLeft = originalsheetLengthTopLeft - coordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                    lengthTopRight = originalsheetLengthTopRight - coordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal;
                                    lengthTopTip = originalsheetLengthTopTip - coordinateInPlane_y - objectInColision_In_Local_x[openingIndex].LengthTotal; // ??? Pre reverzny smer je to asi nezmyselne
                                }

                                if (isFibreglassFirst && isFibreglassLast) // Dva otvory - first a last, jeden cladding sheet medzi nimi
                                {
                                    iNumberOfEdges = 4;
                                    coordinateInPlane_y = objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y + objectInColision_In_Local_x[openingIndex].LengthTotal;
                                    lengthTopLeft = objectInColision_In_Local_x[openingIndex + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                    lengthTopRight = objectInColision_In_Local_x[openingIndex + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                    lengthTopTip = objectInColision_In_Local_x[openingIndex + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                }
                            }

                            if (objectInColision_In_Local_x[openingIndex].OpeningType == EOpeningType.Doors || objectInColision_In_Local_x[openingIndex].OpeningType == EOpeningType.Windows) hasOverlap = false;

                            CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheetIndex + 1, prefix, name, location, material,
                            thicknessCore_m, widthCoil, coilMass_kg_m2, coilPrice_PPSM_NZD, claddingWidthModular,
                            originalsheetNumberOfEdges, // 4 alebo 5 vrcholov
                            originalsheetCoordinateInPlane_x,
                            coordinateInPlane_y,
                            originalsheetControlPoint, originalsheetWidth,
                            lengthTopLeft,
                            lengthTopRight,
                            originalsheetTipCoordinate_x,
                            lengthTopTip,
                            colorName, claddingShape, claddingCoatingType,
                            color, fOpacity, claddingWidthRibModular, true, 0, hasOverlap);

                            List<CCladdingOrFibreGlassSheet> cuttedSheets = CutSheetAccordingToMaxLength(sheet);
                            sheets.AddRange(cuttedSheets);
                            iSheetIndex += cuttedSheets.Count;

                            //listOfSheets.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, prefix, name, material,
                            //thicknessCore_m, widthCoil, coilMass_kg_m2, coilPrice_PPSM_NZD, claddingWidthModular,
                            //originalsheetNumberOfEdges,
                            //originalsheetCoordinateInPlane_x,
                            //objectInColision_In_Local_x[j - 1].CoordinateInPlane_y + objectInColision_In_Local_x[j - 1].LengthTotal,
                            //originalsheetControlPoint, originalsheetWidth,
                            //originalsheetLengthTopLeft - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - objectInColision_In_Local_x[j - 1].LengthTotal,
                            //originalsheetLengthTopRight - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - objectInColision_In_Local_x[j - 1].LengthTotal,
                            //originalsheetTipCoordinate_x,
                            //originalsheetLengthTopTip - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - objectInColision_In_Local_x[j - 1].LengthTotal,
                            //colorName, claddingShape, claddingCoatingType,
                            //color, fOpacity, claddingWidthRibModular, true, 0));
                            //iSheetIndex++;
                        }
                        else
                        {
                            double coordinateInPlane_y = 0; // Zacat od okraja  !!! - je potrebne zmenit pre doors a zacat nad dverami

                            if (j == 0 && isFibreglassFirst)
                                coordinateInPlane_y = (double)objectInColision_In_Local_x[0].LengthTotal;

                            if (isFibreglassFirst) openingIndex = j;
                            else openingIndex = j - 1;

                            if (openingIndex < 0) openingIndex = 0;

                            if (j > 0)
                                coordinateInPlane_y = objectInColision_In_Local_x[openingIndex].CoordinateInPlane_y + objectInColision_In_Local_x[openingIndex].LengthTotal;

                            double lengthTopLeft = objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinateInPlane_y;
                            double lengthTopRight = objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinateInPlane_y;

                            if (isFibreglassFirst && objectInColision_In_Local_x.Count > 1)
                            {
                                lengthTopLeft = objectInColision_In_Local_x[j + 1].CoordinateInPlane_y - coordinateInPlane_y;
                                lengthTopRight = objectInColision_In_Local_x[j + 1].CoordinateInPlane_y - coordinateInPlane_y;
                            }

                            if (side == "Roof-left" && isFibreglassLast && !isFibreglassFirst)
                            {
                                if (j > 0)
                                    coordinateInPlane_y = objectInColision_In_Local_x[j - 1].CoordinateInPlane_y + objectInColision_In_Local_x[j - 1].LengthTotal;

                                if (j < iNumberOfNewSheets - 1)
                                {
                                    lengthTopLeft = objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinateInPlane_y;
                                    lengthTopRight = objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinateInPlane_y;
                                }
                                else
                                {
                                    // TO Ondrej - toto je asi zbytocne, sem by to nemalo vojst, lebo posledny cladding sheet sa riesi v hornom bloku if
                                    lengthTopLeft = height_left_basic - coordinateInPlane_y;
                                    lengthTopRight = height_left_basic - coordinateInPlane_y;
                                }
                            }

                            if (objectInColision_In_Local_x[openingIndex].OpeningType == EOpeningType.Doors || objectInColision_In_Local_x[openingIndex].OpeningType == EOpeningType.Windows) hasOverlap = false;

                            iNumberOfEdges = 4;
                            CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheetIndex + 1, prefix, name, location, material,
                                thicknessCore_m, widthCoil, coilMass_kg_m2, coilPrice_PPSM_NZD, claddingWidthModular,
                                iNumberOfEdges, // 4 vrcholy
                                originalsheetCoordinateInPlane_x,
                                coordinateInPlane_y,
                                originalsheetControlPoint, originalsheetWidth,
                                lengthTopLeft,  //tu Mato nebude problem,ze tam ide stale objectInColision_In_Local_x[j]?
                                lengthTopRight,
                                0,
                                0,
                                colorName, claddingShape, claddingCoatingType,
                                color, fOpacity, claddingWidthRibModular, true, 0, hasOverlap);

                            List<CCladdingOrFibreGlassSheet> cuttedSheets = CutSheetAccordingToMaxLength(sheet);
                            sheets.AddRange(cuttedSheets);
                            iSheetIndex += cuttedSheets.Count;

                            //iNumberOfEdges = 4;
                            //listOfSheets.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, prefix, name, material,
                            //thicknessCore_m, widthCoil, coilMass_kg_m2, coilPrice_PPSM_NZD, claddingWidthModular,
                            //iNumberOfEdges,
                            //originalsheetCoordinateInPlane_x,
                            //coordinate_y,
                            //originalsheetControlPoint, originalsheetWidth,
                            //objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinate_y,
                            //objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinate_y,
                            //0,
                            //0,
                            //colorName, claddingShape, claddingCoatingType,
                            //color, fOpacity, claddingWidthRibModular, true, 0));
                            //iSheetIndex++;
                        }
                    }
                    //CountRealLenghts(sheets, height_left_basic);  //az na konci sa prepocitaju 
                    listOfSheets.AddRange(sheets); //a potom sa pridaju do kolekcie listOfSheets
                }
            }
        }

        private List<CCladdingOrFibreGlassSheet> CutSheetAccordingToMaxLength(CCladdingOrFibreGlassSheet orig_sheet)
        {
            CCladdingOrFibreGlassSheet sheet = orig_sheet.Clone();
            List<CCladdingOrFibreGlassSheet> sheets = new List<CCladdingOrFibreGlassSheet>();

            if (sheet.IsFibreglass)
            {
                if (sheet.IsRoofFibreglass)
                {
                    while (sheet.LengthTotal > maxSheetLegth_RoofFibreglass)
                    {
                        CCladdingOrFibreGlassSheet cuttedSheet = GetCuttedSheetAndShortenOriginal(ref sheet, maxSheetLegth_RoofFibreglass);
                        sheets.Add(cuttedSheet);
                    }
                }
                else if (sheet.IsWalllFibreglass)
                {
                    while (sheet.LengthTotal > maxSheetLegth_WallFibreglass)
                    {
                        CCladdingOrFibreGlassSheet cuttedSheet = GetCuttedSheetAndShortenOriginal(ref sheet, maxSheetLegth_WallFibreglass);
                        sheets.Add(cuttedSheet);
                    }
                }
            }
            else
            {
                if (sheet.IsRoofCladding)
                {
                    while (sheet.LengthTotal > maxSheetLegth_RoofCladding)
                    {
                        CCladdingOrFibreGlassSheet cuttedSheet = GetCuttedSheetAndShortenOriginal(ref sheet, maxSheetLegth_RoofCladding);
                        sheets.Add(cuttedSheet);
                    }
                }
                else if (sheet.IsWallCladding)
                {
                    while (sheet.LengthTotal > maxSheetLegth_WallCladding)
                    {
                        CCladdingOrFibreGlassSheet cuttedSheet = GetCuttedSheetAndShortenOriginal(ref sheet, maxSheetLegth_WallCladding);
                        sheets.Add(cuttedSheet);
                    }
                }
            }

            sheet.Update();

            if (sheet.Location == EBuildingSide.Roof)
            {
                if (sBuildingGeomInputData.fRoofPitch_deg > 0) sheets.Insert(0, sheet);
                else sheets.Add(sheet);
            }
            else if (sheet.IsFibreglass && sheet.Location == EBuildingSide.Roof_Left_Side) sheets.Insert(0, sheet);
            else sheets.Add(sheet);

            if (!orig_sheet.HasOverlap) SetCuttedSheetsOverlaps(sheets);

            return sheets;
        }

        private void SetCuttedSheetsOverlaps(List<CCladdingOrFibreGlassSheet> sheets)
        {
            for (int i = 0; i < sheets.Count; i++)
            {
                if (sheets[i].Name == "Cladding - Roof-Left Side"/* || sheets[i].IsCanopy*/)
                {
                    if (i == sheets.Count - 1) sheets[i].HasOverlap = false;
                    else sheets[i].HasOverlap = true;
                }
                else
                {
                    if (i == 0) sheets[i].HasOverlap = false;
                    else sheets[i].HasOverlap = true;
                }

            }
        }

        private void CutCanopySheet(CCladdingOrFibreGlassSheet originalsheet, bool isRoofLeft, ref int iSheetIndex, double length_left_basic, bool isOrigSheetLast)
        {
            List<CCladdingOrFibreGlassSheet> sheets = new List<CCladdingOrFibreGlassSheet>();
            while (originalsheet.LengthTotal > maxSheetLegth_RoofCladding)
            {
                CCladdingOrFibreGlassSheet cuttedSheet = GetCuttedSheetAndShortenOriginal(ref originalsheet, maxSheetLegth_RoofCladding, false);
                iSheetIndex++;
                cuttedSheet.ID = iSheetIndex;
                sheets.Add(cuttedSheet);

                if (isRoofLeft) listOfCladdingSheetsRoofLeft.Add(cuttedSheet);
                else listOfCladdingSheetsRoofRight.Add(cuttedSheet);
            }
            originalsheet.Update();

            if (isOrigSheetLast) sheets.Add(originalsheet);
            else sheets.Insert(0, originalsheet);

            if (!originalsheet.HasOverlap) SetCuttedSheetsOverlaps(sheets);

            //CountRealLenghts(sheets, length_left_basic);
        }

        private bool SheetHasOverlap(FibreglassProperties fgsp)
        {
            if (fgsp.Location == EBuildingSide.Roof && sBuildingGeomInputData.fRoofPitch_deg > 0)
            {
                //tolerancia 1mm
                if (MathF.d_equal(fgsp.Y + fgsp.Length, fgsp.MaxHeight, 0.001)) return false;
                else return true;
            }
            else
            {
                if (MathF.d_equal(fgsp.Y, 0)) return false;
                else return true;
            }
        }

        private void CountRealLenghts()
        {
            CountRealLenghts(GetCladdingSheets());
            CountRealLenghts(GetFibreglassSheets());
        }
        private void CountRealLenghts(List<CCladdingOrFibreGlassSheet> sheets)
        {
            float overlap = 0f;
            foreach (CCladdingOrFibreGlassSheet sheet in sheets)
            {
                if (sheet.HasOverlap)
                {
                    if (sheet.IsWallCladding) overlap = overlap_WallCladding;
                    else if (sheet.IsRoofCladding) overlap = overlap_RoofCladding;
                    else if (sheet.IsWalllFibreglass) overlap = overlap_WallFibreglass;
                    else if (sheet.IsRoofFibreglass) overlap = overlap_RoofFibreglass;

                    sheet.LengthTopLeft_Real = sheet.LengthTopLeft + overlap;
                    sheet.LengthTopRight_Real = sheet.LengthTopRight + overlap;
                    sheet.LengthTopTip_Real = sheet.LengthTopTip + overlap;
                    sheet.LengthTotal_Real = sheet.LengthTotal + overlap;
                }
                else
                {
                    sheet.LengthTopLeft_Real = sheet.LengthTopLeft;
                    sheet.LengthTopRight_Real = sheet.LengthTopRight;
                    sheet.LengthTopTip_Real = sheet.LengthTopTip;
                    sheet.LengthTotal_Real = sheet.LengthTotal;
                }
            }

            //refaktoring
            //float overlap = 0f;
            //for (int i = 0; i < sheets.Count; i++)
            //{
            //    
            //    // Je potrebne doladit podla specifickych podmienok kedy sa nema jednat o overlap

            //    // TO Ondrej - pre canopies je potrebne uvazovat okraje canopy, teda suradnice 0 a height_left_basic je potrebne upravovat podla sirky canopies a skutocnej suradnice y zaciatku prveho a konca posledneho sheet na canopy
            //    if (    /*i == 0*/
            //        (eModelType == EModelType_FS.eKitsetGableRoofEnclosed && (sheets[i].Name != "Fibreglass - Roof-Left Side" && sheets[i].Name != "Cladding - Roof-Left Side" && MATH.MathF.d_equal(sheets[i].CoordinateInPlane_y, 0)) ||
            //        ((sheets[i].Name == "Fibreglass - Roof-Left Side" || sheets[i].Name == "Cladding - Roof-Left Side") && MATH.MathF.d_equal(sheets[i].CoordinateInPlane_y + sheets[i].LengthTotal, height_left_basic))) ||
            //        (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && 
            //            (sheets[i].Name != "Fibreglass - Roof-Right Side" && sheets[i].Name != "Cladding - Roof-Right Side" && MATH.MathF.d_equal(sheets[i].CoordinateInPlane_y, 0) ||
            //            ((sheets[i].Name == "Fibreglass - Roof-Right Side" || sheets[i].Name == "Cladding - Roof-Right Side") && sBuildingGeomInputData.fRoofPitch_deg < 0 && MATH.MathF.d_equal(sheets[i].CoordinateInPlane_y, 0)) ||
            //            ((sheets[i].Name == "Fibreglass - Roof-Right Side" || sheets[i].Name == "Cladding - Roof-Right Side") && sBuildingGeomInputData.fRoofPitch_deg > 0 && MATH.MathF.d_equal(sheets[i].CoordinateInPlane_y + sheets[i].LengthTotal, height_left_basic))))
            //       )
            //    {
            //        // Nemusi platit pre prvy sheet zo zoznamu, ale skor by tu mala byt podmienka pre sheet, ktory je na spodnom okraji steny, pripadne na okraji strechy kde je gutter
            //        // alebo je to prvy sheet nad otvorom v stene
            //        // Pre sheets ktore su na gable roof, left side sa ma uvazovat ze nie prvy ale posledny sheet v smere y je pri gutter a nema mat pripocitane overlap
            //        // podobne pre monopitch roof treba zohladnit sklon strechy
            //        sheets[i].LengthTopLeft_Real = sheets[i].LengthTopLeft;
            //        sheets[i].LengthTopRight_Real = sheets[i].LengthTopRight;
            //        sheets[i].LengthTopTip_Real = sheets[i].LengthTopTip;
            //        sheets[i].LengthTotal_Real = sheets[i].LengthTotal;
            //    }
            //    else
            //    {
            //        if (sheets[i].IsWallCladding) overlap = overlap_WallCladding;
            //        else if (sheets[i].IsRoofCladding) overlap = overlap_RoofCladding;
            //        else if (sheets[i].IsWalllFibreglass) overlap = overlap_WallFibreglass;
            //        else if (sheets[i].IsRoofFibreglass) overlap = overlap_RoofFibreglass;

            //        sheets[i].LengthTopLeft_Real = sheets[i].LengthTopLeft + overlap;
            //        sheets[i].LengthTopRight_Real = sheets[i].LengthTopRight + overlap;
            //        sheets[i].LengthTopTip_Real = sheets[i].LengthTopTip + overlap;
            //        sheets[i].LengthTotal_Real = sheets[i].LengthTotal + overlap;
            //    }
            //}
        }

        private CCladdingOrFibreGlassSheet GetCuttedSheetAndShortenOriginal(ref CCladdingOrFibreGlassSheet originalSheet, float maxLength, bool changeOriginalID = true)
        {
            CCladdingOrFibreGlassSheet cuttedSheet = originalSheet.Clone();
            cuttedSheet.NumberOfEdges = 4;
            cuttedSheet.LengthTopTip = maxLength;
            cuttedSheet.LengthTopRight = maxLength;
            cuttedSheet.LengthTopLeft = maxLength;
            cuttedSheet.LengthTotal = maxLength;
            cuttedSheet.Update();

            originalSheet.CoordinateInPlane_y += maxLength;
            if (changeOriginalID) originalSheet.ID++;
            originalSheet.LengthTopTip -= maxLength;
            originalSheet.LengthTopRight -= maxLength;
            originalSheet.LengthTopLeft -= maxLength;
            originalSheet.LengthTotal -= maxLength;

            return cuttedSheet;
        }

        public void GenerateCladdingOpenings(List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheets,
            string side,
            Point3D pControlPoint,
            double width,
            int iNumberOfEdges_FG_D_W, // Number of opening edges
            double column_crsc_y_minus_temp,
            double column_crsc_z_plus_temp,
            ref int iOpeningIndex,
            out List<COpening> listOfOpenings_All)
        {
            listOfOpenings_All = new List<COpening>(); // Zoznam otvorov v cladding

            // Skonvertujeme zoznam fibreglass na otvory (ak nie je prazdny) a nastavime / pridame ho do zoznamu otvorov v cladding
            if (listOfFibreGlassSheets != null && listOfFibreGlassSheets.Count > 0)
                listOfOpenings_All = SheetListToOpeningListConverter(listOfFibreGlassSheets);

            // Do zoznamu pridame otvory pre doors
            if (doorPropCollection != null)
            {
                foreach (DoorProperties door in doorPropCollection)
                {
                    if (door.sBuildingSide == side)
                    {
                        // TODO - vypocitat presnu poziciu otvoru dveri od laveho okraja steny
                        // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)

                        double doorPosition_x_Input_GUI;

                        if (side == "Left" || side == "Right")
                            doorPosition_x_Input_GUI = -column_crsc_y_minus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * bayWidthCollection[door.iBayNumber - 1].Width);
                        else
                            doorPosition_x_Input_GUI = column_crsc_z_plus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * (door.sBuildingSide == "Front" ? FFrontColumnDistance : FBackColumnDistance));

                        double doorPosition_x = doorPosition_x_Input_GUI;

                        if (door.sBuildingSide == "Left" || door.sBuildingSide == "Back") // Reverse x-direction in GUI
                            doorPosition_x = width - doorPosition_x_Input_GUI - door.fDoorsWidth;

                        listOfOpenings_All.Add(new COpening(iOpeningIndex + 1, iNumberOfEdges_FG_D_W, doorPosition_x, 0,
                        pControlPoint, door.fDoorsWidth, door.fDoorsHeight, door.fDoorsHeight, 0, 0, true, 0, EOpeningType.Doors));
                        iOpeningIndex++;
                    }
                }
            }

            // Do zoznamu pridame otvory pre windows
            if (windowPropCollection != null)
            {
                foreach (WindowProperties window in windowPropCollection)
                {
                    if (window.sBuildingSide == side)
                    {
                        // TODO - vypocitat presnu poziciu otvoru okna od laveho okraja steny
                        // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                        double windowPosition_x_Input_GUI;

                        if (side == "Left" || side == "Right")
                            windowPosition_x_Input_GUI = -column_crsc_y_minus_temp + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * bayWidthCollection[window.iBayNumber - 1].Width);
                        else
                            windowPosition_x_Input_GUI = column_crsc_z_plus_temp + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * (window.sBuildingSide == "Front" ? FFrontColumnDistance : FBackColumnDistance));

                        double windowPosition_x = windowPosition_x_Input_GUI;

                        if (window.sBuildingSide == "Left" || window.sBuildingSide == "Back") // Reverse x-direction in GUI
                            windowPosition_x = width - windowPosition_x_Input_GUI - window.fWindowsWidth;

                        listOfOpenings_All.Add(new COpening(iOpeningIndex + 1, iNumberOfEdges_FG_D_W, windowPosition_x, -bottomEdge_z + window.fWindowCoordinateZinBay,
                        pControlPoint, window.fWindowsWidth, window.fWindowsHeight, window.fWindowsHeight, 0, 0, true, 0, EOpeningType.Windows));
                        iOpeningIndex++;
                    }
                }
            }
        }

        public List<COpening> SheetListToOpeningListConverter(List<CCladdingOrFibreGlassSheet> input)
        {
            List<COpening> output = null;

            if (input != null && input.Count > 0)
            {
                output = new List<COpening>();

                foreach (CCladdingOrFibreGlassSheet sheet in input)
                    output.Add(sheet.ConvertToOpening());
            }

            return output;
        }

        public void AddSheet3DModelsToModelGroup(List<CCladdingOrFibreGlassSheet> listOfsheets,
            DisplayOptions options,
            ImageBrush brush,
            DiffuseMaterial material,
            double widthRibModular,
            double rotationX,
            double rotationY,
            double rotationZ,
            ref Model3DGroup modelGroup,
            //bool createWireframe,
            double outOffPlaneOffset = 0)
        {
            if (listOfsheets != null && listOfsheets.Count > 0)
            {

                double wpWidth = 0, wpHeight = 0;

                for (int i = 0; i < listOfsheets.Count; i++)
                {
                    if (options.bUseTextures && options.bUseTexturesCladding)
                    {
                        if (listOfsheets[i].IsCanopy)
                        {
                            wpWidth = widthRibModular / listOfsheets[i].Width;
                            wpHeight = widthRibModular / listOfsheets[i].LengthTotal;
                            ImageBrush brushCanopy = brush.Clone();
                            System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                            r.Location = new System.Windows.Point(-listOfsheets[i].WpWidthOffset, 0);
                            brushCanopy.Viewport = r;
                            material = new DiffuseMaterial(brushCanopy); //tu dufam,ze neprepise material ten ktory sa nastavil
                        }
                        else
                        {
                            if (i == 0)
                            {
                                wpWidth = widthRibModular / listOfsheets[i].Width;
                                wpHeight = widthRibModular / listOfsheets[i].LengthTotal;
                                brush.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                                material = new DiffuseMaterial(brush);
                            }
                            else if (i == listOfsheets.Count - 1)
                            {
                                wpWidth = widthRibModular / listOfsheets[i].Width;
                                ImageBrush brush_Last = brush.Clone();
                                brush_Last.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                                material = new DiffuseMaterial(brush_Last);
                            }
                        }
                    }
                    else if (options.bCladdingSheetColoursByID)
                    {
                        listOfsheets[i].Color = ColorsHelper.GetColorWithIndex(i, bUseTop20Colors);
                    }

                    listOfsheets[i].RotationX = rotationX;
                    listOfsheets[i].RotationY = rotationY;
                    listOfsheets[i].RotationZ = rotationZ;

                    // Pridame sheet do model group
                    GeometryModel3D sheetModel = listOfsheets[i].GetCladdingSheetModel(options, material, outOffPlaneOffset);
                    sheetModel.Transform = listOfsheets[i].GetTransformGroup();
                    //listOfsheets[i].PointText = sheetModel.Transform.Transform(listOfsheets[i].PointText); //transformPoint
                    //if (createWireframe)
                    //{
                    listOfsheets[i].SetWireFramePoints();
                    List<Point3D> wireframe = listOfsheets[i].WireFramePoints; // Vytvorime docasny zoznam bodov pre jeden sheet
                    Drawing3DHelper.TransformPoints(wireframe, sheetModel.Transform); // Transformujeme body wireframe z LCS do GCS
                    WireFramePoints.AddRange(wireframe); // Pridame body do zoznamu
                    //}

                    modelGroup.Children.Add(sheetModel);
                }
            }
        }

        public void SetCladdingGenerateProperties(IList<CComponentInfo> componentList)
        {
            CComponentInfo girtL = componentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (girtL != null && girtL.Generate.HasValue)
            {
                bGenerateLeftSideCladding = girtL.Generate.Value;
                bGenerateLeftSideFibreglass = girtL.Generate.Value;
            }

            CComponentInfo girtR = componentList.LastOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (girtR != null && girtR.Generate.HasValue)
            {
                bGenerateRightSideCladding = girtR.Generate.Value;
                bGenerateRightSideFibreglass = girtR.Generate.Value;
            }

            CComponentInfo girtFront = componentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide);
            if (girtFront != null && girtFront.Generate.HasValue)
            {
                bGenerateFrontSideCladding = girtFront.Generate.Value;
                bGenerateFrontSideFibreglass = girtFront.Generate.Value;
            }

            CComponentInfo girtBack = componentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtBackSide);
            if (girtBack != null && girtBack.Generate.HasValue)
            {
                bGenerateBackSideCladding = girtBack.Generate.Value;
                bGenerateBackSideFibreglass = girtBack.Generate.Value;
            }

            CComponentInfo purlin = componentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
            if (purlin != null && purlin.Generate.HasValue)
            {
                bGenerateRoofCladding = purlin.Generate.Value;
                bGenerateRoofFibreglass = purlin.Generate.Value;
            }
        }

        public void SetCladdingWireframePoints()
        {
            WireFramePoints = new List<Point3D>();
            if (listOfCladdingSheetsLeftWall != null) foreach (CCladdingOrFibreGlassSheet s in listOfCladdingSheetsLeftWall) WireFramePoints.AddRange(s.WireFramePoints);
            if (listOfCladdingSheetsRightWall != null) foreach (CCladdingOrFibreGlassSheet s in listOfCladdingSheetsRightWall) WireFramePoints.AddRange(s.WireFramePoints);
            if (listOfCladdingSheetsFrontWall != null) foreach (CCladdingOrFibreGlassSheet s in listOfCladdingSheetsFrontWall) WireFramePoints.AddRange(s.WireFramePoints);
            if (listOfCladdingSheetsBackWall != null) foreach (CCladdingOrFibreGlassSheet s in listOfCladdingSheetsBackWall) WireFramePoints.AddRange(s.WireFramePoints);
            if (listOfCladdingSheetsRoofLeft != null) foreach (CCladdingOrFibreGlassSheet s in listOfCladdingSheetsRoofLeft) WireFramePoints.AddRange(s.WireFramePoints);
            if (listOfCladdingSheetsRoofRight != null) foreach (CCladdingOrFibreGlassSheet s in listOfCladdingSheetsRoofRight) WireFramePoints.AddRange(s.WireFramePoints);
        }

        public List<CCladdingOrFibreGlassSheet> GetCladdingSheets()
        {
            List<CCladdingOrFibreGlassSheet> list = new List<CCladdingOrFibreGlassSheet>();
            if (listOfCladdingSheetsLeftWall != null) list.AddRange(listOfCladdingSheetsLeftWall);
            if (listOfCladdingSheetsFrontWall != null) list.AddRange(listOfCladdingSheetsFrontWall);
            if (listOfCladdingSheetsRightWall != null) list.AddRange(listOfCladdingSheetsRightWall);
            if (listOfCladdingSheetsBackWall != null) list.AddRange(listOfCladdingSheetsBackWall);
            if (listOfCladdingSheetsRoofRight != null) list.AddRange(listOfCladdingSheetsRoofRight);
            if (listOfCladdingSheetsRoofLeft != null) list.AddRange(listOfCladdingSheetsRoofLeft);

            return list;
        }
        public List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Wall()
        {
            List<CCladdingOrFibreGlassSheet> list = new List<CCladdingOrFibreGlassSheet>();
            if (listOfCladdingSheetsLeftWall != null) list.AddRange(listOfCladdingSheetsLeftWall);
            if (listOfCladdingSheetsFrontWall != null) list.AddRange(listOfCladdingSheetsFrontWall);
            if (listOfCladdingSheetsRightWall != null) list.AddRange(listOfCladdingSheetsRightWall);
            if (listOfCladdingSheetsBackWall != null) list.AddRange(listOfCladdingSheetsBackWall);

            return list;
        }
        public List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Roof()
        {
            List<CCladdingOrFibreGlassSheet> list = new List<CCladdingOrFibreGlassSheet>();
            if (listOfCladdingSheetsRoofRight != null) list.AddRange(listOfCladdingSheetsRoofRight);
            if (listOfCladdingSheetsRoofLeft != null) list.AddRange(listOfCladdingSheetsRoofLeft);

            return list;
        }

        public List<CCladdingOrFibreGlassSheet> GetFibreglassSheets()
        {
            List<CCladdingOrFibreGlassSheet> list = new List<CCladdingOrFibreGlassSheet>();
            if (listOfFibreGlassSheetsWallLeft != null) list.AddRange(listOfFibreGlassSheetsWallLeft);
            if (listOfFibreGlassSheetsWallFront != null) list.AddRange(listOfFibreGlassSheetsWallFront);
            if (listOfFibreGlassSheetsWallRight != null) list.AddRange(listOfFibreGlassSheetsWallRight);
            if (listOfFibreGlassSheetsWallBack != null) list.AddRange(listOfFibreGlassSheetsWallBack);
            if (listOfFibreGlassSheetsRoofRight != null) list.AddRange(listOfFibreGlassSheetsRoofRight);
            if (listOfFibreGlassSheetsRoofLeft != null) list.AddRange(listOfFibreGlassSheetsRoofLeft);

            return list;
        }
        public List<CCladdingOrFibreGlassSheet> GetFibreglassSheets_Roof()
        {
            List<CCladdingOrFibreGlassSheet> list = new List<CCladdingOrFibreGlassSheet>();
            if (listOfFibreGlassSheetsRoofRight != null) list.AddRange(listOfFibreGlassSheetsRoofRight);
            if (listOfFibreGlassSheetsRoofLeft != null) list.AddRange(listOfFibreGlassSheetsRoofLeft);

            return list;
        }
        public List<CCladdingOrFibreGlassSheet> GetFibreglassSheets_Wall()
        {
            List<CCladdingOrFibreGlassSheet> list = new List<CCladdingOrFibreGlassSheet>();
            if (listOfFibreGlassSheetsWallLeft != null) list.AddRange(listOfFibreGlassSheetsWallLeft);
            if (listOfFibreGlassSheetsWallFront != null) list.AddRange(listOfFibreGlassSheetsWallFront);
            if (listOfFibreGlassSheetsWallRight != null) list.AddRange(listOfFibreGlassSheetsWallRight);
            if (listOfFibreGlassSheetsWallBack != null) list.AddRange(listOfFibreGlassSheetsWallBack);

            return list;
        }

        public bool HasCladdingSheets()
        {
            return HasCladdingSheets_Roof() || HasCladdingSheets_Wall();
        }
        public bool HasCladdingSheets_Roof()
        {
            if (listOfCladdingSheetsRoofRight != null && listOfCladdingSheetsRoofRight.Count > 0) return true;
            if (listOfCladdingSheetsRoofLeft != null && listOfCladdingSheetsRoofLeft.Count > 0) return true;

            return false;
        }
        public bool HasCladdingSheets_RoofRight()
        {
            if (listOfCladdingSheetsRoofRight != null && listOfCladdingSheetsRoofRight.Count > 0) return true;

            return false;
        }
        public bool HasCladdingSheets_RoofLeft()
        {
            if (listOfCladdingSheetsRoofLeft != null && listOfCladdingSheetsRoofLeft.Count > 0) return true;

            return false;
        }

        public bool HasCladdingSheets_Wall()
        {
            if (listOfCladdingSheetsLeftWall != null && listOfCladdingSheetsLeftWall.Count > 0) return true;
            if (listOfCladdingSheetsFrontWall != null && listOfCladdingSheetsFrontWall.Count > 0) return true;
            if (listOfCladdingSheetsRightWall != null && listOfCladdingSheetsRightWall.Count > 0) return true;
            if (listOfCladdingSheetsBackWall != null && listOfCladdingSheetsBackWall.Count > 0) return true;

            return false;
        }
        public bool HasCladdingSheets_WallLeft()
        {
            if (listOfCladdingSheetsLeftWall != null && listOfCladdingSheetsLeftWall.Count > 0) return true;

            return false;
        }
        public bool HasCladdingSheets_WallFront()
        {
            if (listOfCladdingSheetsFrontWall != null && listOfCladdingSheetsFrontWall.Count > 0) return true;

            return false;
        }
        public bool HasCladdingSheets_WallRight()
        {
            if (listOfCladdingSheetsRightWall != null && listOfCladdingSheetsRightWall.Count > 0) return true;

            return false;
        }
        public bool HasCladdingSheets_WallBack()
        {
            if (listOfCladdingSheetsBackWall != null && listOfCladdingSheetsBackWall.Count > 0) return true;

            return false;
        }

        public bool HasFibreglassSheets()
        {
            return HasFibreglassSheets_Wall() || HasFibreglassSheets_Roof();
        }
        public bool HasFibreglassSheets_Roof()
        {
            if (listOfFibreGlassSheetsRoofRight != null && listOfFibreGlassSheetsRoofRight.Count > 0) return true;
            if (listOfFibreGlassSheetsRoofLeft != null && listOfFibreGlassSheetsRoofLeft.Count > 0) return true;

            return false;
        }
        public bool HasFibreglassSheets_RoofRight()
        {
            if (listOfFibreGlassSheetsRoofRight != null && listOfFibreGlassSheetsRoofRight.Count > 0) return true;

            return false;
        }
        public bool HasFibreglassSheets_RoofLeft()
        {
            if (listOfFibreGlassSheetsRoofLeft != null && listOfFibreGlassSheetsRoofLeft.Count > 0) return true;

            return false;
        }

        public bool HasFibreglassSheets_Wall()
        {
            if (listOfFibreGlassSheetsWallLeft != null && listOfFibreGlassSheetsWallLeft.Count > 0) return true;
            if (listOfFibreGlassSheetsWallFront != null && listOfFibreGlassSheetsWallFront.Count > 0) return true;
            if (listOfFibreGlassSheetsWallRight != null && listOfFibreGlassSheetsWallRight.Count > 0) return true;
            if (listOfFibreGlassSheetsWallBack != null && listOfFibreGlassSheetsWallBack.Count > 0) return true;

            return false;
        }
        public bool HasFibreglass_WallLeft()
        {
            if (listOfFibreGlassSheetsWallLeft != null && listOfFibreGlassSheetsWallLeft.Count > 0) return true;

            return false;
        }
        public bool HasFibreglass_WallFront()
        {
            if (listOfFibreGlassSheetsWallFront != null && listOfFibreGlassSheetsWallFront.Count > 0) return true;

            return false;
        }
        public bool HasFibreglass_WallRight()
        {
            if (listOfFibreGlassSheetsWallRight != null && listOfFibreGlassSheetsWallRight.Count > 0) return true;

            return false;
        }
        public bool HasFibreglass_WallBack()
        {
            if (listOfFibreGlassSheetsWallBack != null && listOfFibreGlassSheetsWallBack.Count > 0) return true;

            return false;
        }

        public double GetSheetCollectionLongitudinalIntersectionLength(List<CCladdingOrFibreGlassSheet> sheets, List<float> positions_X, float sideLength)
        {
            positions_X.Add(sideLength); // Pridame koncovu suradnicu

            double totalIntersectionLength = 0;

            for (int i = 0; i < positions_X.Count; i++) // Kazde x miesto
            {
                List<CInterval> intervals_Y = new List<CInterval>();

                for (int j = 0; j < sheets.Count; j++) // Kazdy sheet
                {
                    // sheet konci alebo zacina v pozicii X
                    if (MathF.d_equal(sheets[j].CoordinateInPlane_x, positions_X[i]) ||
                      MathF.d_equal(sheets[j].CoordinateInPlane_x + sheets[j].Width, positions_X[i]))
                        intervals_Y.Add(new CInterval(sheets[j].CoordinateInPlane_y, sheets[j].CoordinateInPlane_y + sheets[j].LengthTotal));
                }

                // Get intersection length for in current X for intervals Y
                double intersectionLength_inXCoord = GetIntersectionLength(intervals_Y);
                // Pridame dlzku presahov v mieste X k celkovej dlzke pre celu kolekciu
                totalIntersectionLength += intersectionLength_inXCoord;
            }

            return totalIntersectionLength;
        }

        private double GetIntersectionLength(List<CInterval> intervals)
        {
            double length = 0;

            if (intervals == null || intervals.Count < 2)
                return length;

            double l; double r;
            for (int i = 0; i < intervals.Count; i++)
            {
                l = intervals[i].S;
                r = intervals[i].E;

                for (int j = i + 1; j < intervals.Count; j++)
                {
                    if (intervals[j].S > r || intervals[j].E < l)
                    {
                        // No intersection;
                    }
                    else
                    {
                        length += Math.Abs(Math.Min(r, intervals[j].E) - Math.Max(l, intervals[j].S));
                    }
                }
            }
            return length;
        }

        public double GetMaxItemLength()
        {
            double maxLen = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in GetCladdingSheets())
            {
                if (maxLen < sheet.LengthTotal_Real) maxLen = sheet.LengthTotal_Real;
            }
            foreach (CCladdingOrFibreGlassSheet sheet in GetFibreglassSheets())
            {
                if (maxLen < sheet.LengthTotal_Real) maxLen = sheet.LengthTotal_Real;
            }
            return maxLen;
        }

        public double GetMaxItemLength_RoofFibreglass()
        {
            double maxLen = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in GetFibreglassSheets_Roof())
            {
                if (maxLen < sheet.LengthTotal_Real) maxLen = sheet.LengthTotal_Real;
            }
            return maxLen;
        }

        public double GetMaxItemLength_WallFibreglass()
        {
            double maxLen = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in GetFibreglassSheets_Wall())
            {
                if (maxLen < sheet.LengthTotal_Real) maxLen = sheet.LengthTotal_Real;
            }
            return maxLen;
        }

        public double GetSheet_OverlappingWidth(CCladdingOrFibreGlassSheet sheet, double side_length)
        {
            // Funkcia vrati kontaktnu sirku pre sheet, ktory je v kontakte s inym sheet
            // To znamena ze do sirky sa nezahrna rozmer width ak je zaciatok alebo koniec na okraji strechy alebo steny
            // Defaultne je width pre overlap 2x width
            double dLimitLength = 0.001; // 1 mm

            double doverlapwidth = 2 * sheet.Width;

            if (sheet.Width > sheet.CladdingWidthRibModular)
            {
                // End without overlap with other sheet - edge of side
                if (MathF.d_equal(sheet.CoordinateInPlane_y + sheet.LengthTotal, side_length, dLimitLength))
                    doverlapwidth -= sheet.Width;

                // Start without overlap with other sheet - edge of side
                if (MathF.d_equal(sheet.CoordinateInPlane_y, 0, dLimitLength))
                    doverlapwidth -= sheet.Width;

                // Podmienka neplati ak su na cladding sheet len 2 rebra
                if (sheet.HasOverlap && MathF.d_equal(doverlapwidth, 0.0001))
                    throw new Exception("Incompatible sheet properties."); // Overlap width nie je nulova ale sheet ma overlap = true ????

                return doverlapwidth;
            }
            else
                return 0;
        }

        public int GetSheet_OverlappingWidthFixingPoints(CCladdingOrFibreGlassSheet sheet, double side_length)
        {
            // Funkcia vrati kontaktnu sirku pre sheet, ktory je v kontakte s inym sheet
            // To znamena ze do sirky sa nezahrna rozmer width ak je zaciatok alebo koniec na okraji strechy alebo steny
            // Defaultne je width pre overlap 2x width
            double dLimitLength = 0.001; // 1 mm

            int doverlapwidthFixingPoints = 2 * Math.Max(0, (int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 - 2);

            if (sheet.Width > sheet.CladdingWidthRibModular)
            {
                // End without overlap with other sheet - edge of side
                if (MathF.d_equal(sheet.CoordinateInPlane_y + sheet.LengthTotal, side_length, dLimitLength))
                    doverlapwidthFixingPoints -= Math.Max(0, (int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 - 2);

                // Start without overlap with other sheet - edge of side
                if (MathF.d_equal(sheet.CoordinateInPlane_y, 0, dLimitLength))
                    doverlapwidthFixingPoints -= Math.Max(0, (int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 - 2);

                // Podmienka neplati ak su na cladding sheet len 2 rebra
                // Tato kontrola nefunguje uplne dobre, treba to este skontrolovat pre sheets ktore su nad doors alebo v okoli windows.
                //if (sheet.HasOverlap && doverlapwidthFixingPoints < 1)
                //    throw new Exception("Incompatible sheet properties."); // Overlap width fixing points je nula ale sheet ma overlap = true ????

                return doverlapwidthFixingPoints;
            }
            else
                return 0;
        }

        public void Clear()
        {
            listOfFibreGlassSheetsWallLeft = null;
            listOfCladdingSheetsLeftWall = null;
            listOfFibreGlassSheetsWallFront = null;
            listOfCladdingSheetsFrontWall = null;
            listOfFibreGlassSheetsWallRight = null;
            listOfCladdingSheetsRightWall = null;
            listOfFibreGlassSheetsWallBack = null;
            listOfCladdingSheetsBackWall = null;
            listOfFibreGlassSheetsRoofRight = null;
            listOfCladdingSheetsRoofRight = null;
            listOfFibreGlassSheetsRoofLeft = null;
            listOfCladdingSheetsRoofLeft = null;
        }
    }

    public class CInterval
    {
        public double S;
        public double E;
        public CInterval(double start, double end)
        {
            S = start;
            E = end;
        }
    }
}





//zaloha pre refaktoring 4.6.2021
//public Model3DGroup GetCladdingModel(DisplayOptions options)
//{
//    ControlPoint = new Point3D(0, 0, 0);

//    Model3DGroup model_gr = new Model3DGroup();
//    WireFramePoints = new List<Point3D>();

//    // Vytvorime model v GCS [0,0,0] je uvazovana v bode m_ControlPoint

//    // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
//    // Start
//    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
//    double additionalOffset = 0.010;  // 10 mm Aby nekolidovali plochy cladding s members
//    double additionalOffsetRoof = 0.010; // 10 mm Aby nekolidovali plochy cladding s members (cross-bracing) na streche

//    // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
//    double column_crsc_y_minus_temp = Column_crsc_y_minus - additionalOffset;
//    double column_crsc_y_plus_temp = Column_crsc_y_plus + additionalOffset;
//    double column_crsc_z_plus_temp = Column_crsc_z_plus + additionalOffset;

//    //----------------------------------------
//    // To Ondrej - toto by sme mali nahradit funkciou
//    // CalculateWallHeightsForCladding
//    double height_1_final = sBuildingGeomInputData.fH_1_centerline + Column_crsc_z_plus / Math.Cos(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
//    double height_2_final = sBuildingGeomInputData.fH_2_centerline + Column_crsc_z_plus / Math.Cos(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon

//    double height_1_final_edge_LR_Wall = height_1_final - column_crsc_z_plus_temp * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
//    double height_2_final_edge_LR_Wall = height_2_final;

//    double height_1_final_edge_Roof = height_1_final + additionalOffsetRoof - (column_crsc_z_plus_temp + roofEdgeOverhang_X) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
//    double height_2_final_edge_Roof = height_2_final + additionalOffsetRoof;

//    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//    {
//        height_2_final_edge_LR_Wall = height_2_final + column_crsc_z_plus_temp * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
//        height_2_final_edge_Roof = height_2_final + additionalOffsetRoof + (column_crsc_z_plus_temp + roofEdgeOverhang_X) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
//    }

//    // Nastavime rovnaku vysku hornej hrany
//    double height_1_final_edge_FB_Wall = height_1_final_edge_LR_Wall;
//    double height_2_final_edge_FB_Wall = height_2_final_edge_LR_Wall;

//    if (considerRoofCladdingFor_FB_WallHeight)
//    {
//        height_1_final_edge_FB_Wall = height_1_final_edge_FB_Wall + m_RoofProps.height_m * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
//        height_2_final_edge_FB_Wall = height_2_final_edge_FB_Wall + m_RoofProps.height_m * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

//        if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//            height_2_final_edge_FB_Wall = height_2_final + (column_crsc_z_plus_temp + m_RoofProps.height_m) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
//    }
//    //----------------------------------------

//    // Wall Cladding Edges

//    Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, bottomEdge_z);
//    Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, bottomEdge_z);

//    Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, bottomEdge_z);
//    Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, bottomEdge_z);

//    // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
//    // End
//    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

//    // TODO - mohlo by prist z Main Window, rozdiel je ze pridavame nejake odstadenie od hrany profilu, aby s nim plech nekolidoval v 3D
//    double baseWidth_Overall_for3D = sBuildingGeomInputData.fWidth_overall + 2 * additionalOffset;
//    // TODO - mohlo by prist z Main Window, rozdiel je ze pridavame nejake odstadenie od hrany profilu, aby s nim plech nekolidoval v 3D
//    double baseLength_Overall_for3D = sBuildingGeomInputData.fLength_overall + 2 * additionalOffset;

//    Brush solidBrushFront = new SolidColorBrush(m_ColorWall);
//    Brush solidBrushSide = new SolidColorBrush(m_ColorWall);
//    Brush solidBrushRoof = new SolidColorBrush(m_ColorRoof);

//    Brush solidBrushWall_FG = new SolidColorBrush(m_ColorWall_FG);
//    Brush solidBrushRoof_FG = new SolidColorBrush(m_ColorRoof_FG);

//    solidBrushFront.Opacity = options.fFrontCladdingOpacity;
//    solidBrushSide.Opacity = options.fLeftCladdingOpacity;
//    solidBrushRoof.Opacity = options.fRoofCladdingOpacity;

//    solidBrushWall_FG.Opacity = options.fFibreglassOpacity;
//    solidBrushRoof_FG.Opacity = options.fFibreglassOpacity;

//    DiffuseMaterial material_SideWall = new DiffuseMaterial(solidBrushSide); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
//    DiffuseMaterial material_FrontBackWall = new DiffuseMaterial(solidBrushFront); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
//    DiffuseMaterial material_Roof = new DiffuseMaterial(solidBrushRoof); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export

//    DiffuseMaterial material_Wall_FG = new DiffuseMaterial(solidBrushWall_FG);
//    DiffuseMaterial material_Roof_FG = new DiffuseMaterial(solidBrushRoof_FG);

//    ImageBrush brushFront = null;
//    ImageBrush brushSide = null;
//    ImageBrush brushRoof = null;

//    ImageBrush brushWall_FG = null;
//    ImageBrush brushRoof_FG = null;

//    if (options.bUseTextures && options.bUseTexturesCladding)
//    {
//        //CS ENDURA® a CS MAXX® maju rovnake farby, takze pre nich coating type v nazve suboru nepouzivam
//        string uriString_Wall = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + m_ColorNameWall + ".jpg";

//        if (m_claddingCoatingType_Wall == "FORMCLAD®")
//        {
//            string claddingCoatingType_Wall_string = "FORMCLAD";
//            uriString_Wall = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + claddingCoatingType_Wall_string + "_" + m_ColorNameWall + ".jpg";
//        }

//        brushFront = new ImageBrush();
//        brushFront.ImageSource = new BitmapImage(new Uri(uriString_Wall, UriKind.RelativeOrAbsolute));
//        brushFront.TileMode = TileMode.Tile;
//        brushFront.ViewportUnits = BrushMappingMode.Absolute;
//        brushFront.Stretch = Stretch.Fill;
//        brushFront.Opacity = options.fFrontCladdingOpacity;

//        brushSide = new ImageBrush();
//        brushSide.ImageSource = new BitmapImage(new Uri(uriString_Wall, UriKind.RelativeOrAbsolute));
//        brushSide.TileMode = TileMode.Tile;
//        brushSide.ViewportUnits = BrushMappingMode.Absolute;
//        brushSide.Stretch = Stretch.Fill;
//        brushSide.Opacity = options.fLeftCladdingOpacity;

//        // CS ENDURA® a CS MAXX® maju rovnake farby, takze pre nich coating type v nazve suboru nepouzivam
//        string uriString_Roof = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + m_ColorNameRoof + ".jpg";

//        if (m_claddingCoatingType_Roof == "FORMCLAD®")
//        {
//            string claddingCoatingType_Roof_string = "FORMCLAD";
//            uriString_Roof = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + claddingCoatingType_Roof_string + "_" + m_ColorNameRoof + ".jpg";
//        }

//        brushRoof = new ImageBrush();
//        brushRoof.ImageSource = new BitmapImage(new Uri(uriString_Roof, UriKind.RelativeOrAbsolute));
//        brushRoof.TileMode = TileMode.Tile;
//        brushRoof.ViewportUnits = BrushMappingMode.Absolute;
//        brushRoof.Stretch = Stretch.Fill;
//        brushRoof.Opacity = options.fRoofCladdingOpacity;

//        // Fibreglass
//        //string uriString_FG_Wall = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + m_ColorNameWall_FG + ".jpg";
//        string uriString_FG_Wall = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + "White" + ".jpg";

//        brushWall_FG = new ImageBrush();
//        brushWall_FG.ImageSource = new BitmapImage(new Uri(uriString_FG_Wall, UriKind.RelativeOrAbsolute));
//        brushWall_FG.TileMode = TileMode.Tile;
//        brushWall_FG.ViewportUnits = BrushMappingMode.Absolute;
//        brushWall_FG.Stretch = Stretch.Fill;
//        brushWall_FG.Opacity = options.fFibreglassOpacity;

//        //string uriString_FG_Roof = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + m_ColorNameRoof_FG + ".jpg";
//        string uriString_FG_Roof = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + "White" + ".jpg";

//        brushRoof_FG = new ImageBrush();
//        brushRoof_FG.ImageSource = new BitmapImage(new Uri(uriString_FG_Roof, UriKind.RelativeOrAbsolute));
//        brushRoof_FG.TileMode = TileMode.Tile;
//        brushRoof_FG.ViewportUnits = BrushMappingMode.Absolute;
//        brushRoof_FG.Stretch = Stretch.Fill;
//        brushRoof_FG.Opacity = options.fFibreglassOpacity;
//    }


//    // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
//    // Start
//    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
//    // Wall Points
//    //Point3D pLRWall_front2_heightright = new Point3D();
//    //Point3D pLRWall_back2_heightright = new Point3D();
//    //Point3D pLRWall_front3_heightleft = new Point3D();
//    //Point3D pLRWall_back3_heightleft = new Point3D();
//    //Point3D pLRWall_front4_top = new Point3D();
//    //Point3D pLRWall_back4_top = new Point3D();

//    //Point3D pFBWall_front2_heightright = new Point3D();
//    //Point3D pFBWall_back2_heightright = new Point3D();
//    //Point3D pFBWall_front3_heightleft = new Point3D();
//    //Point3D pFBWall_back3_heightleft = new Point3D();
//    //Point3D pFBWall_front4_top = new Point3D();
//    //Point3D pFBWall_back4_top = new Point3D();

//    // Roof Points - oddelene pretoze strecha ma presahy
//    Point3D pRoof_front2_heightright = new Point3D();
//    Point3D pRoof_back2_heightright = new Point3D();
//    Point3D pRoof_front3_heightleft = new Point3D();
//    Point3D pRoof_back3_heightleft = new Point3D();
//    Point3D pRoof_front4_top = new Point3D();
//    Point3D pRoof_back4_top = new Point3D();

//    // Roof edge offset from centerline in Y-direction
//    float fRoofEdgeOffsetFromCenterline = -(float)column_crsc_y_minus_temp + (float)roofEdgeOverhang_Y;

//    // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
//    // End
//    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

//    int iNumberOfFrontBackWallEdges = 4;

//    //Point3D pControlPoint_FrontWall = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z);
//    //Point3D pControlPoint_BackWall = new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z);
//    //Point3D pControlPoint_LeftWall = new Point3D(pback0_baseleft.X, pback0_baseleft.Y, pback0_baseleft.Z);
//    //Point3D pControlPoint_RightWall = new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z);

//    Point3D pControlPoint_RoofRight;
//    Point3D pControlPoint_RoofLeft;

//    // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
//    // Start
//    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
//    // Nastavenie bodov suradnic hornych bodov stien a bodov strechy pre monopitch a gable roof model
//    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//    {
//        // Monopitch Roof

//        // Wall
//        iNumberOfFrontBackWallEdges = 4;
//        //pLRWall_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_2_final_edge_FB_Wall);
//        //pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_FB_Wall);

//        //pLRWall_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_2_final_edge_FB_Wall);
//        //pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_FB_Wall);

//        //pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, height_2_final_edge_FB_Wall);
//        //pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, height_2_final_edge_FB_Wall);
//        //pFBWall_front3_heightleft = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, height_1_final_edge_FB_Wall);
//        //pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, height_1_final_edge_FB_Wall);

//        // Roof
//        pRoof_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_2_final_edge_Roof);
//        pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_1_final_edge_Roof);

//        pRoof_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_2_final_edge_Roof);
//        pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_1_final_edge_Roof);

//        /*
//        if (options.bUseTextures) // Pouzijeme len ak vykreslujeme textury, inak sa pouzije material vytvoreny z SolidColorBrush podla vybranej farby cladding v GUI
//        {
//            wpWidth = m_WallProps.widthRib_m / (pfront1_baseright.X - pfront0_baseleft.X);
//            wpHeight = m_WallProps.widthRib_m / (pFBWall_front2_heightright.Z - pfront1_baseright.Z);
//            brushFront.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//            material_FrontBackWall = new DiffuseMaterial(brushFront);
//        }

//        if (options.bUseTextures)
//        {
//            wpWidth = m_WallProps.widthRib_m / (pback0_baseleft.Y - pfront0_baseleft.Y);
//            wpHeight = m_WallProps.widthRib_m / (pLRWall_back2_heightright.Z - pback0_baseleft.Z);
//            brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//            material_SideWall = new DiffuseMaterial(brushSide);
//        }

//        if (options.bUseTextures)
//        {
//            double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pRoof_front2_heightright, pRoof_front3_heightleft); // Rovina XZ
//            wpWidth = m_RoofProps.widthRib_m / (pRoof_back2_heightright.Y - pRoof_front2_heightright.Y);
//            wpHeight = m_RoofProps.widthRib_m / poinstsDist;
//            brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//            material_Roof = new DiffuseMaterial(brushRoof);
//        }*/
//    }
//    else if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
//    {
//        // Gable Roof Building

//        // Wall
//        iNumberOfFrontBackWallEdges = 5;
//        //pLRWall_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_LR_Wall);
//        //pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_LR_Wall);
//        //pLRWall_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp, height_2_final_edge_LR_Wall);

//        //pLRWall_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_LR_Wall);
//        //pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_LR_Wall);
//        //pLRWall_back4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_2_final_edge_LR_Wall);

//        //pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, height_1_final_edge_FB_Wall);
//        //pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, height_1_final_edge_FB_Wall);
//        //pFBWall_front3_heightleft = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, height_1_final_edge_FB_Wall);
//        //pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, height_1_final_edge_FB_Wall);
//        //pFBWall_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp, height_2_final_edge_FB_Wall);
//        //pFBWall_back4_top = new Point3D(pLRWall_back4_top.X, pLRWall_back4_top.Y, height_2_final_edge_FB_Wall);

//        // Roof
//        pRoof_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_1_final_edge_Roof);
//        pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_1_final_edge_Roof);
//        pRoof_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_2_final_edge_Roof);

//        pRoof_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_1_final_edge_Roof);
//        pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_1_final_edge_Roof);
//        pRoof_back4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_2_final_edge_Roof);

//        /*
//        if (options.bUseTextures)
//        {
//            wpWidth = m_WallProps.widthRib_m / (pfront1_baseright.X - pfront0_baseleft.X);
//            wpHeight = m_WallProps.widthRib_m / (pFBWall_front4_top.Z - pfront1_baseright.Z);
//            brushFront.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//            material_FrontBackWall = new DiffuseMaterial(brushFront);
//        }

//        if (options.bUseTextures)
//        {
//            wpWidth = m_WallProps.widthRib_m / (pback0_baseleft.Y - pfront0_baseleft.Y);
//            wpHeight = m_WallProps.widthRib_m / (pLRWall_back3_heightleft.Z - pback0_baseleft.Z);
//            brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//            material_SideWall = new DiffuseMaterial(brushSide);
//        }

//        if (options.bUseTextures)
//        {
//            double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pRoof_front4_top, pRoof_front3_heightleft); // Rovina XZ
//            wpWidth = m_RoofProps.widthRib_m / (pRoof_back4_top.Y - pRoof_front4_top.Y);
//            wpHeight = m_RoofProps.widthRib_m / poinstsDist;
//            brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//            material_Roof = new DiffuseMaterial(brushRoof);
//        }
//        */
//    }
//    else
//    {
//        throw new Exception("Not implemented kitset type.");
//    }

//    // TO Ondrej - toto cele by sme potrebovali dat do CPFDViewModel, resp do nejakeho helpera
//    // End
//    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

//    // Ak je bDisplayIndividualCladdingSheets = false
//    // Zobrazime len jednoliatu plochu s farbou alebo texturou, nad nou mozeme zobrazit fibreglass sheet s offsetom aby sa nevnarali do cladding
//    // Bude to podobne ako door a window, takze sa nebudu kreslit realne otvory len sa nad plochu strechy dokresli fibreglass sheet
//    // V takom pripade nebudeme generovat cladding sheet material list ani cladding sheet layout pattern
//    // Len spocitame plochu otvorov a odratame ju z celkovej plochy cladding a to bude v Quotation

//    // Ak je bDisplayIndividualCladdingSheets = true
//    // Generujeme jednotlive plechy, tie rozdelime podla toho ako koliduju s otvormi
//    // V mieste otvorov pre fibreglass, door, windows sa nebudú generovat cladding sheets, ktore su otvorom rozdelene po celej sirke
//    // Cladding sheets, ktore sa s otvorom prekryvaju len ciastocne vykreslime v 3D cele
//    // Tuto zostavu cladding sheets, fibreglass sheets, doors, windows vieme pekne vykreslit do layouts ako 2D pohlady na jednotlive steny
//    // Do vykazu materialov mozeme potom uviest jednotlive dlzky cladding sheets

//    // Particular Cladding Sheet Model

//    // Prva uroven, stany budovy alebo strechy, left, right, front, back, roof left roof right
//    // Druha uroven jednotlive sheet nachadzajuce sa v jednej rovine
//    //List<List<CCladdingOrFibreGlassSheet>> listOfCladdingSheets = new List<List<CCladdingOrFibreGlassSheet>>();

//    int iSheetIndex = 0;
//    int iSheet_FG_Index = 0;
//    int iOpeningIndex = 0;
//    int iNumberOfEdges_FG_D_W = 4; // Number of edges - fibreglass shet, door, window opening


//    // Left Wall
//    double height_left_basic = -bottomEdge_z + height_1_final_edge_LR_Wall;

//    int iNumberOfWholeSheets = (int)(baseLength_Overall_for3D / m_WallProps.widthModular_m);
//    double dWidthOfWholeSheets = iNumberOfWholeSheets * m_WallProps.widthModular_m;
//    double dPartialSheet_End = baseLength_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
//    int iNumberOfSheets = iNumberOfWholeSheets + 1;

//    List<COpening> listOfOpeningsLeftWall_All = null;
//    List<CCladdingOrFibreGlassSheet> listOfFibreGlassOpenings = null;

//    if (bGenerateLeftSideFibreglass)
//    {
//        listOfFibreGlassSheetsWallLeft = new List<CCladdingOrFibreGlassSheet>();
//        listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

//        foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
//        {
//            if (fgsp.Side == "Left")
//            {
//                CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFL", "Fibreglass - Left Wall", fgsp.Location, m_MaterialFibreglass_Wall,
//                    m_WallProps_FG.thickness_m, m_WallProps_FG.widthCoil_m, m_WallProps_FG.flatsheet_mass_kg_m2, m_WallProps_FG.price_PPSM_NZD * m_WallProps_FG.widthModular_m / m_WallProps_FG.widthCoil_m, m_WallProps_FG.widthModular_m,
//                    iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
//                    pback0_baseleft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : m_WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
//                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, m_WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));

//                listOfFibreGlassOpenings.Add(sheet);
//                List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
//                //CountRealLenghts(sheets);
//                listOfFibreGlassSheetsWallLeft.AddRange(sheets);
//                iSheet_FG_Index += sheets.Count;
//            }
//        }
//        GenerateCladdingOpenings(listOfFibreGlassOpenings, "Left", pback0_baseleft, baseLength_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
//        ref iOpeningIndex, out listOfOpeningsLeftWall_All);
//    }

//    if (bGenerateLeftSideCladding)
//    {
//        if (bIndividualCladdingSheets)
//            GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_temp, "Left", EBuildingSide.Left, "WCL", "Cladding - Left Wall", m_MaterialCladding_Wall, pback0_baseleft, m_ColorNameWall,
//            m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity,
//            m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, baseLength_Overall_for3D,
//            m_WallProps.widthRib_m, m_WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_left_basic,
//            listOfOpeningsLeftWall_All, ref iSheetIndex, out listOfCladdingSheetsLeftWall);
//        else // (bGenerateLeftSideCladding && !bIndividualCladdingSheets)
//        {
//            listOfCladdingSheetsLeftWall = new List<CCladdingOrFibreGlassSheet>();
//            listOfCladdingSheetsLeftWall.Add(new CCladdingOrFibreGlassSheet(1, "WCL", "Cladding - Left Wall", EBuildingSide.Left, m_MaterialCladding_Wall,
//             m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, m_WallProps.widthModular_m,
//                4, 0, 0,
//                pback0_baseleft, baseLength_Overall_for3D, height_left_basic, height_left_basic, 0.5 * baseLength_Overall_for3D, height_left_basic,
//                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity,
//                m_WallProps.widthRib_m, options.bDisplayCladdingLeftWall, 0, false));

//            /* Mono
//            CAreaPolygonal area = new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pLRWall_front3_heightleft, pLRWall_back3_heightleft }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */

//            /* Gable
//            CAreaPolygonal area = new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pLRWall_front3_heightleft, pLRWall_back3_heightleft }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */
//        }
//    }

//    // Front Wall
//    height_left_basic = -bottomEdge_z + height_1_final_edge_FB_Wall;

//    iNumberOfWholeSheets = (int)(baseWidth_Overall_for3D / m_WallProps.widthModular_m);
//    dWidthOfWholeSheets = iNumberOfWholeSheets * m_WallProps.widthModular_m;
//    dPartialSheet_End = baseWidth_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
//    iNumberOfSheets = iNumberOfWholeSheets + 1;

//    List<COpening> listOfOpeningsFrontWall_All = null;
//    if (bGenerateFrontSideFibreglass)
//    {
//        listOfFibreGlassSheetsWallFront = new List<CCladdingOrFibreGlassSheet>();
//        listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

//        foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
//        {
//            if (fgsp.Side == "Front")
//            {
//                CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFF", "Fibreglass - Front Wall", fgsp.Location, m_MaterialFibreglass_Wall,
//                    m_WallProps_FG.thickness_m, m_WallProps_FG.widthCoil_m, m_WallProps_FG.flatsheet_mass_kg_m2, m_WallProps_FG.price_PPSM_NZD * m_WallProps_FG.widthModular_m / m_WallProps_FG.widthCoil_m, m_WallProps_FG.widthModular_m,
//                    iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
//                    pfront0_baseleft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : m_WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
//                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, m_WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
//                listOfFibreGlassOpenings.Add(sheet);

//                List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
//                //CountRealLenghts(sheets, height_left_basic);
//                listOfFibreGlassSheetsWallFront.AddRange(sheets);
//                iSheet_FG_Index += sheets.Count;
//            }
//        }

//        GenerateCladdingOpenings(listOfFibreGlassOpenings, "Front", pfront0_baseleft, baseWidth_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
//        ref iOpeningIndex, out listOfOpeningsFrontWall_All);
//    }

//    if (bGenerateFrontSideCladding)
//    {
//        if (bIndividualCladdingSheets)
//            GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_temp, "Front", EBuildingSide.Front, "WCF", "Cladding - Front Wall", m_MaterialCladding_Wall, pfront0_baseleft, m_ColorNameWall,
//            m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity,
//            m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, baseWidth_Overall_for3D,
//            m_WallProps.widthRib_m, m_WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_2_final_edge_FB_Wall,
//            listOfOpeningsFrontWall_All, ref iSheetIndex, out listOfCladdingSheetsFrontWall);
//        else // (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall && !bIndividualCladdingSheets)
//        {
//            double height_right_basic = height_left_basic;
//            double height_middle_basic = -bottomEdge_z + height_2_final_edge_FB_Wall;

//            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//            {
//                height_right_basic = -bottomEdge_z + height_2_final_edge_FB_Wall;
//                height_middle_basic = 0.5 * (height_left_basic + height_right_basic);
//            }

//            listOfCladdingSheetsFrontWall = new List<CCladdingOrFibreGlassSheet>();
//            listOfCladdingSheetsFrontWall.Add(new CCladdingOrFibreGlassSheet(2, "WCF", "Cladding - Front Wall", EBuildingSide.Front, m_MaterialCladding_Wall,
//                m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, m_WallProps.widthModular_m,
//                iNumberOfFrontBackWallEdges, 0, 0,
//                pfront0_baseleft, baseWidth_Overall_for3D, height_left_basic, height_right_basic, 0.5 * baseWidth_Overall_for3D, height_middle_basic,
//                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, m_WallProps.widthRib_m,
//                options.bDisplayCladdingFrontWall, 0, false));

//            /* Mono
//            CAreaPolygonal area = new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pFBWall_front2_heightright, pFBWall_front3_heightleft }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */

//            /* Gable
//            CAreaPolygonal area = new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pFBWall_front2_heightright, pFBWall_front4_top, pFBWall_front3_heightleft }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */
//        }
//    }

//    // Right Wall
//    height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_LR_Wall : -bottomEdge_z + height_2_final_edge_LR_Wall;

//    iNumberOfWholeSheets = (int)(baseLength_Overall_for3D / m_WallProps.widthModular_m);
//    dWidthOfWholeSheets = iNumberOfWholeSheets * m_WallProps.widthModular_m;
//    dPartialSheet_End = baseLength_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
//    iNumberOfSheets = iNumberOfWholeSheets + 1;

//    List<COpening> listOfOpeningsRightWall_All = null;

//    if (bGenerateRightSideFibreglass)
//    {
//        listOfFibreGlassSheetsWallRight = new List<CCladdingOrFibreGlassSheet>();
//        listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

//        foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
//        {
//            if (fgsp.Side == "Right")
//            {
//                CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFR", "Fibreglass - Right Wall", fgsp.Location, m_MaterialFibreglass_Wall,
//                    m_WallProps_FG.thickness_m, m_WallProps_FG.widthCoil_m, m_WallProps_FG.flatsheet_mass_kg_m2, m_WallProps_FG.price_PPSM_NZD * m_WallProps_FG.widthModular_m / m_WallProps_FG.widthCoil_m, m_WallProps_FG.widthModular_m,
//                    iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
//                    pfront1_baseright, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : m_WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
//                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, m_WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
//                listOfFibreGlassOpenings.Add(sheet);

//                List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
//                //CountRealLenghts(sheets, height_left_basic);
//                listOfFibreGlassSheetsWallRight.AddRange(sheets);
//                iSheet_FG_Index += sheets.Count;
//            }
//        }
//        GenerateCladdingOpenings(listOfFibreGlassOpenings, "Right", pfront1_baseright, baseLength_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
//        ref iOpeningIndex, out listOfOpeningsRightWall_All);
//    }

//    if (bGenerateRightSideCladding)
//    {
//        if (bIndividualCladdingSheets)
//            GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_temp, "Right", EBuildingSide.Right, "WCR", "Cladding - Right Wall", m_MaterialCladding_Wall, pfront1_baseright, m_ColorNameWall,
//            m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity,
//            m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, baseLength_Overall_for3D,
//            m_WallProps.widthRib_m, m_WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_left_basic,
//            listOfOpeningsRightWall_All, ref iSheetIndex, out listOfCladdingSheetsRightWall);
//        else
//        {
//            listOfCladdingSheetsRightWall = new List<CCladdingOrFibreGlassSheet>();
//            listOfCladdingSheetsRightWall.Add(new CCladdingOrFibreGlassSheet(3, "WCR", "Cladding - Right Wall", EBuildingSide.Right, m_MaterialCladding_Wall,
//                m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, m_WallProps.widthModular_m,
//                4, 0, 0,
//                pfront1_baseright, baseLength_Overall_for3D, height_left_basic, height_left_basic, 0.5 * baseLength_Overall_for3D, height_left_basic,
//                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity,
//                m_WallProps.widthRib_m, options.bDisplayCladdingRightWall, 0, false, false));

//            /* Monopitch
//            CAreaPolygonal area = new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pLRWall_back2_heightright, pLRWall_front2_heightright }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */

//            /* Gable
//            CAreaPolygonal area = new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pLRWall_back2_heightright, pLRWall_front2_heightright }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */
//        }
//    }

//    // Back Wall
//    height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_FB_Wall : -bottomEdge_z + height_2_final_edge_FB_Wall;

//    iNumberOfWholeSheets = (int)(baseWidth_Overall_for3D / m_WallProps.widthModular_m);
//    dWidthOfWholeSheets = iNumberOfWholeSheets * m_WallProps.widthModular_m;
//    dPartialSheet_End = baseWidth_Overall_for3D - dWidthOfWholeSheets; // Last Sheet
//    iNumberOfSheets = iNumberOfWholeSheets + 1;

//    List<COpening> listOfOpeningsBackWall_All = null;

//    if (bGenerateBackSideFibreglass)
//    {
//        listOfFibreGlassSheetsWallBack = new List<CCladdingOrFibreGlassSheet>();
//        listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

//        foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
//        {
//            if (fgsp.Side == "Back")
//            {
//                CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "WFB", "Fibreglass - Back Wall", fgsp.Location, m_MaterialFibreglass_Wall,
//                    m_WallProps_FG.thickness_m, m_WallProps_FG.widthCoil_m, m_WallProps_FG.flatsheet_mass_kg_m2, m_WallProps_FG.price_PPSM_NZD * m_WallProps_FG.widthModular_m / m_WallProps_FG.widthCoil_m, m_WallProps_FG.widthModular_m,
//                    iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
//                    pback1_baseright, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : m_WallProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
//                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, m_WallProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
//                listOfFibreGlassOpenings.Add(sheet);

//                List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
//                //CountRealLenghts(sheets, height_left_basic);
//                listOfFibreGlassSheetsWallBack.AddRange(sheets);
//                iSheet_FG_Index += sheets.Count;
//            }
//        }
//        GenerateCladdingOpenings(listOfFibreGlassOpenings, "Back", pback1_baseright, baseWidth_Overall_for3D, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
//        ref iOpeningIndex, out listOfOpeningsBackWall_All);
//    }

//    if (bGenerateBackSideCladding)
//    {
//        if (bIndividualCladdingSheets)
//            GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_temp, "Back", EBuildingSide.Back, "WCB", "Cladding - Back Wall", m_MaterialCladding_Wall, pback1_baseright, m_ColorNameWall,
//            m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity,
//            m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, baseWidth_Overall_for3D,
//            m_WallProps.widthRib_m, m_WallProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_2_final_edge_FB_Wall,
//            listOfOpeningsBackWall_All, ref iSheetIndex, out listOfCladdingSheetsBackWall);
//        else //(bGenerateBackSideCladding && !bIndividualCladdingSheets)
//        {
//            double height_right_basic = height_left_basic;
//            double height_middle_basic = -bottomEdge_z + height_2_final_edge_FB_Wall;

//            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//            {
//                height_right_basic = -bottomEdge_z + height_1_final_edge_FB_Wall;
//                height_middle_basic = 0.5 * (height_left_basic + height_right_basic);
//            }

//            listOfCladdingSheetsBackWall = new List<CCladdingOrFibreGlassSheet>();
//            listOfCladdingSheetsBackWall.Add(new CCladdingOrFibreGlassSheet(4, "WCB", "Cladding - Back Wall", EBuildingSide.Back, m_MaterialCladding_Wall,
//                m_WallProps.thicknessCore_m, m_WallCoilProps.widthCoil, m_WallCoilProps.coilmass_kg_m2, m_WallCoilProps.price_PPSM_NZD, m_WallProps.widthModular_m,
//                iNumberOfFrontBackWallEdges, 0, 0,
//                pback1_baseright, baseWidth_Overall_for3D, height_left_basic, height_right_basic, 0.5 * baseWidth_Overall_for3D, height_middle_basic,
//                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, m_WallProps.widthRib_m, options.bDisplayCladdingBackWall, 0, false));

//            /* Mono
//            CAreaPolygonal area = new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pFBWall_back3_heightleft, pFBWall_back2_heightright }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */

//            /* Gable
//            CAreaPolygonal area = new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pFBWall_back3_heightleft, pFBWall_back4_top, pFBWall_back2_heightright }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */
//        }
//    }

//    // TODO - vstup z main Window
//    float RoofLength_Y = (float)(fRoofEdgeOffsetFromCenterline + sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y);

//    // Roof - Right Side

//    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
//        dRoofSide_length = Drawing3D.GetPoint3DDistanceDouble(pRoof_front2_heightright, pRoof_front4_top);
//    else
//        dRoofSide_length = Drawing3D.GetPoint3DDistanceDouble(pRoof_front3_heightleft, pRoof_front2_heightright);

//    pControlPoint_RoofRight = new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z);

//    iNumberOfWholeSheets = (int)(RoofLength_Y / m_RoofProps.widthModular_m);
//    dWidthOfWholeSheets = iNumberOfWholeSheets * m_RoofProps.widthModular_m;
//    dPartialSheet_End = RoofLength_Y - dWidthOfWholeSheets; // Last Sheet
//    iNumberOfSheets = iNumberOfWholeSheets + 1;

//    if (bGenerateRoofFibreglass)
//    {
//        listOfFibreGlassSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();
//        listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

//        foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
//        {
//            if (fgsp.Side == "Roof" || fgsp.Side == "Roof-Right Side")
//            {
//                CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "RF", "Fibreglass - Roof-Right Side", fgsp.Location, m_MaterialFibreglass_Roof,
//                     m_RoofProps_FG.thickness_m, m_RoofProps_FG.widthCoil_m, m_RoofProps_FG.flatsheet_mass_kg_m2, m_RoofProps_FG.price_PPSM_NZD * m_RoofProps_FG.widthModular_m / m_RoofProps_FG.widthCoil_m, m_RoofProps_FG.widthModular_m,
//                     iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
//                     pControlPoint_RoofRight, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : m_RoofProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
//                     m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, options.fFibreglassOpacity, m_RoofProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
//                listOfFibreGlassOpenings.Add(sheet);

//                if (!sheet.HasOverlap && IsAnyCanopyOnSide(fgsp.Location, sheet.ConvertToOpening(), column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, dRoofSide_length))
//                    sheet.HasOverlap = true;

//                List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
//                //CountRealLenghts(sheets, dRoofSide_length);
//                listOfFibreGlassSheetsRoofRight.AddRange(sheets);
//                iSheet_FG_Index += sheets.Count;
//            }
//        }
//    }

//    bool isOrigSheetLast = true;
//    if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && sBuildingGeomInputData.fRoofPitch_deg > 0) isOrigSheetLast = false;

//    if (bGenerateRoofCladding)
//    {
//        if (bIndividualCladdingSheets)
//        {
//            EBuildingSide sheetLoc = EBuildingSide.Roof_Right_Side;
//            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed) sheetLoc = EBuildingSide.Roof;

//            GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_temp, "Roof-right", sheetLoc, "RC", "Cladding - Roof-Right Side", m_MaterialCladding_Roof, pControlPoint_RoofRight, m_ColorNameRoof,
//            m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity,
//            m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, RoofLength_Y,
//            m_RoofProps.widthRib_m, m_RoofProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, dRoofSide_length, dRoofSide_length,
//            SheetListToOpeningListConverter(listOfFibreGlassOpenings), ref iSheetIndex, out listOfCladdingSheetsRoofRight);

//            // TODO - upravit plechy pre canopies
//            if (canopyCollection != null)
//            {
//                for (int i = 0; i < listOfCladdingSheetsRoofRight.Count; i++)
//                {
//                    CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofRight[i];
//                    int cIndex = 0;
//                    int breakIndex = canopyCollection.Count;
//                    foreach (CCanopiesInfo canopy in canopyCollection)
//                    {
//                        double CanopyCladdingWidth_Right = 0;
//                        if (RightSheetNeedsToBeExtendedToCanopy(originalsheet, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, out CanopyCladdingWidth_Right))
//                        {

//                            // Tu sa upravia dlzky pre sheet ktory zasahuje do canopy
//                            // Je potrebne nasledne este sheet rozdelit podla jeho maximalnej dlzky a rozdelenym sheet nastavit overlap (okrem krajneho)
//                            originalsheet.CoordinateInPlane_y = 0;
//                            originalsheet.CoordinateInPlane_y -= CanopyCladdingWidth_Right;
//                            originalsheet.LengthTopLeft += CanopyCladdingWidth_Right;
//                            originalsheet.LengthTopRight += CanopyCladdingWidth_Right;
//                            //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
//                            originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);
//                            originalsheet.Update();

//                            CutCanopySheet(originalsheet, false, ref iSheetIndex, dRoofSide_length, isOrigSheetLast);


//                            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed || (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && !canopy.Left))
//                                breakIndex = cIndex + 1;
//                        }

//                        // Pre monopitch upravujeme aj lavu stranu plechu
//                        if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//                        {
//                            double CanopyCladdingWidth_Left = 0;
//                            if (LeftSheetNeedsToBeExtendedToCanopy(originalsheet, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, dRoofSide_length, out CanopyCladdingWidth_Left))
//                            {
//                                //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth; // Ostava povodne
//                                originalsheet.LengthTopLeft += CanopyCladdingWidth_Left;
//                                originalsheet.LengthTopRight += CanopyCladdingWidth_Left;
//                                //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
//                                originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);
//                                originalsheet.Update();

//                                CutCanopySheet(originalsheet, false, ref iSheetIndex, dRoofSide_length, isOrigSheetLast);

//                                breakIndex = cIndex + 1;
//                            }
//                        }
//                        if (cIndex == breakIndex) break;
//                        cIndex++;
//                    }
//                }
//            }
//        }
//        else
//        {
//            listOfCladdingSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();
//            listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(5, "RC", "Cladding - Roof", EBuildingSide.Roof, m_MaterialCladding_Roof,
//                m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, m_RoofProps.widthModular_m,
//                4, 0, 0,
//                pControlPoint_RoofRight, RoofLength_Y, dRoofSide_length, dRoofSide_length, 0.5 * RoofLength_Y, dRoofSide_length,
//                m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, m_RoofProps.widthRib_m, options.bDisplayCladdingRoof, 0, false));
//            /* Mono
//            CAreaPolygonal area = new CAreaPolygonal(4, new List<Point3D>() { pRoof_front2_heightright, pRoof_back2_heightright, pRoof_back3_heightleft, pRoof_front3_heightleft }, 0);
//            model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_Roof));
//            area.SetWireFramePoints();
//            WireFramePoints.AddRange(area.WireFramePoints);
//            */

//            /* Gable
//            CAreaPolygonal areaR = new CAreaPolygonal(4, new List<Point3D>() { pRoof_front2_heightright, pRoof_back2_heightright, pRoof_back4_top, pRoof_front4_top }, 0);
//            model_gr.Children.Add(areaR.CreateArea(options.bUseTextures, material_Roof));
//            areaR.SetWireFramePoints();
//            WireFramePoints.AddRange(areaR.WireFramePoints);
//            */

//            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
//            {
//                // Canopies
//                foreach (CCanopiesInfo canopy in canopyCollection)
//                {
//                    double width_temp;
//                    int iAreaIndex = 6;

//                    if (canopy.Right)
//                    {
//                        bool hasNextCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
//                        bool hasPreviousCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

//                        float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
//                        float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

//                        float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
//                        float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

//                        float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
//                        int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / m_RoofProps.widthRib_m);
//                        double dWidthOfWholeRibs = iNumberOfWholeRibs * m_RoofProps.widthRib_m;
//                        double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

//                        float fCanopyCladdingWidth = (float)canopy.WidthRight + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
//                        float fCanopy_EdgeCoordinate_z = (float)height_2_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

//                        Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_2_final_edge_Roof);
//                        Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_2_final_edge_Roof);
//                        Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
//                        Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

//                        double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

//                        double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

//                        //if (options.bUseTextures)
//                        //{
//                        //    wpWidth = m_RoofProps.widthRib_m / (pback_left.Y - pfront_left.Y);
//                        //    wpHeight = m_RoofProps.widthRib_m / poinstsDist;

//                        //    wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

//                        //    ImageBrush brushRoofCanopy = brushRoof.Clone();
//                        //    System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//                        //    r.Location = new System.Windows.Point(-wpWidthOffset, 0);
//                        //    brushRoofCanopy.Viewport = r;
//                        //    material_Roof = new DiffuseMaterial(brushRoofCanopy);
//                        //}

//                        width_temp = pback_left.Y - pfront_left.Y;

//                        listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof, m_MaterialCladding_Roof,
//                            m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, m_RoofProps.widthModular_m,
//                            4, 0, 0,
//                            pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
//                            m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, m_RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

//                        /*
//                        CAreaPolygonal areaCR = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
//                        model_gr.Children.Add(areaCR.CreateArea(options.bUseTextures, material_Roof));
//                        areaCR.SetWireFramePoints();
//                        WireFramePoints.AddRange(areaCR.WireFramePoints);
//                        */
//                        iAreaIndex++;
//                    }

//                    if (canopy.Left)
//                    {
//                        // 2 ______ 1
//                        //  |      |
//                        //  |      |
//                        //  |______|
//                        // 3        0

//                        bool hasNextCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
//                        bool hasPreviousCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

//                        float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
//                        float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

//                        float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
//                        float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

//                        float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
//                        int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / m_RoofProps.widthRib_m);
//                        double dWidthOfWholeRibs = iNumberOfWholeRibs * m_RoofProps.widthRib_m;
//                        double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

//                        float fCanopyCladdingWidth = (float)canopy.WidthLeft + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
//                        float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

//                        Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
//                        Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
//                        Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
//                        Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

//                        double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

//                        double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne
//                        //if (options.bUseTextures)
//                        //{
//                        //    wpWidth = m_RoofProps.widthRib_m / (pback_left.Y - pfront_left.Y);
//                        //    wpHeight = m_RoofProps.widthRib_m / poinstsDist;

//                        //    double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

//                        //    ImageBrush brushRoofCanopy = brushRoof.Clone();
//                        //    System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//                        //    r.Location = new System.Windows.Point(-wpWidthOffset, 0);
//                        //    brushRoofCanopy.Viewport = r;
//                        //    material_Roof = new DiffuseMaterial(brushRoofCanopy);
//                        //}

//                        width_temp = pback_left.Y - pfront_left.Y;

//                        listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof, m_MaterialCladding_Roof,
//                            m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, m_RoofProps.widthModular_m,
//                            4, 0, 0,
//                            pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
//                            m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, m_RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

//                        /*
//                        CAreaPolygonal areaCL = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
//                        model_gr.Children.Add(areaCL.CreateArea(options.bUseTextures, material_Roof));
//                        areaCL.SetWireFramePoints();
//                        WireFramePoints.AddRange(areaCL.WireFramePoints);
//                        */
//                        iAreaIndex++;
//                    }
//                }
//            }
//        }
//    }

//    // Roof - Left Side

//    listOfFibreGlassSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();

//    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
//    {
//        // Roof - Left Side
//        pControlPoint_RoofLeft = new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z);
//        iNumberOfWholeSheets = (int)(RoofLength_Y / m_RoofProps.widthModular_m);
//        dWidthOfWholeSheets = iNumberOfWholeSheets * m_RoofProps.widthModular_m;
//        dPartialSheet_End = RoofLength_Y - dWidthOfWholeSheets; // Last Sheet
//        iNumberOfSheets = iNumberOfWholeSheets + 1;

//        if (bGenerateRoofFibreglass)
//        {
//            listOfFibreGlassOpenings = new List<CCladdingOrFibreGlassSheet>();

//            foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
//            {
//                if (fgsp.Side == "Roof-Left Side")
//                {
//                    dRoofSide_length = Drawing3D.GetPoint3DDistanceDouble(pRoof_front3_heightleft, pRoof_front4_top);

//                    // Pre Left side prevratime suradnice v LCS y, aby boli vstupy na oboch stranach brane od spodnej hrany H1
//                    double Position_y = dRoofSide_length - fgsp.Y - fgsp.Length;

//                    CCladdingOrFibreGlassSheet sheet = new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, "RF", "Fibreglass - Roof-Left Side", fgsp.Location, m_MaterialFibreglass_Roof,
//                        m_RoofProps_FG.thickness_m, m_RoofProps_FG.widthCoil_m, m_RoofProps_FG.flatsheet_mass_kg_m2, m_RoofProps_FG.price_PPSM_NZD * m_RoofProps_FG.widthModular_m / m_RoofProps_FG.widthCoil_m, m_RoofProps_FG.widthModular_m,
//                        iNumberOfEdges_FG_D_W, fgsp.X, Position_y,
//                        pControlPoint_RoofLeft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : m_RoofProps_FG.widthModular_m, fgsp.Length, fgsp.Length, 0, 0,
//                        m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, options.fFibreglassOpacity, m_RoofProps_FG.widthRib_m, true, 0, SheetHasOverlap(fgsp));
//                    listOfFibreGlassOpenings.Add(sheet);

//                    if (!sheet.HasOverlap && IsAnyCanopyOnSide(fgsp.Location, sheet.ConvertToOpening(), column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, dRoofSide_length))
//                        sheet.HasOverlap = true;

//                    List<CCladdingOrFibreGlassSheet> sheets = CutSheetAccordingToMaxLength(sheet);
//                    //CountRealLenghts(sheets, dRoofSide_length);
//                    listOfFibreGlassSheetsRoofLeft.AddRange(sheets);
//                    iSheet_FG_Index += sheets.Count;
//                }
//            }
//        }

//        if (bGenerateRoofCladding)
//        {
//            if (bIndividualCladdingSheets)
//            {
//                GenerateCladdingSheets(options.bCladdingSheetColoursByID, column_crsc_y_minus_temp, "Roof-left", EBuildingSide.Roof_Left_Side, "RC", "Cladding - Roof-Left Side", m_MaterialCladding_Roof,
//                pControlPoint_RoofLeft, m_ColorNameRoof,
//                m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity,
//                m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD,
//                RoofLength_Y,
//                m_RoofProps.widthRib_m, m_RoofProps.widthModular_m, iNumberOfSheets, dPartialSheet_End, dRoofSide_length, dRoofSide_length,
//                SheetListToOpeningListConverter(listOfFibreGlassOpenings), ref iSheetIndex, out listOfCladdingSheetsRoofLeft);

//                // TODO - upravit plechy pre canopies
//                if (canopyCollection != null)
//                {
//                    for (int i = 0; i < listOfCladdingSheetsRoofLeft.Count; i++)
//                    {
//                        CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofLeft[i];

//                        int cIndex = 0;
//                        int breakIndex = canopyCollection.Count;
//                        foreach (CCanopiesInfo canopy in canopyCollection)
//                        {
//                            double CanopyCladdingWidth_Left = 0;
//                            if (LeftSheetNeedsToBeExtendedToCanopy(originalsheet, canopy, column_crsc_y_minus_temp, column_crsc_y_plus_temp, column_crsc_z_plus_temp, fRoofEdgeOffsetFromCenterline, dRoofSide_length, out CanopyCladdingWidth_Left))
//                            {
//                                //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth; // Ostava povodne
//                                originalsheet.LengthTopLeft += CanopyCladdingWidth_Left;
//                                originalsheet.LengthTopRight += CanopyCladdingWidth_Left;
//                                //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
//                                originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);
//                                originalsheet.Update();

//                                CutCanopySheet(originalsheet, true, ref iSheetIndex, dRoofSide_length, isOrigSheetLast);

//                                breakIndex = cIndex + 1;
//                            }

//                            if (cIndex == breakIndex) break;
//                            cIndex++;
//                        }

//                    }
//                }
//            }
//            else
//            {
//                listOfCladdingSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();
//                listOfCladdingSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(6, "RC", "Cladding - Roof-Left Side", EBuildingSide.Roof_Left_Side, m_MaterialCladding_Roof,
//                    m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, m_RoofProps.widthModular_m,
//                    4, 0, 0,
//                    pControlPoint_RoofLeft, RoofLength_Y, dRoofSide_length, dRoofSide_length, 0.5 * RoofLength_Y, dRoofSide_length,
//                    m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, m_RoofProps.widthRib_m, options.bDisplayCladdingRoof, 0, false));

//                /*
//                CAreaPolygonal areaL = new CAreaPolygonal(5, new List<Point3D>() { pRoof_front4_top, pRoof_back4_top, pRoof_back3_heightleft, pRoof_front3_heightleft }, 0);
//                model_gr.Children.Add(areaL.CreateArea(options.bUseTextures, material_Roof));
//                areaL.SetWireFramePoints();
//                WireFramePoints.AddRange(areaL.WireFramePoints);
//                */

//                // Canopies
//                foreach (CCanopiesInfo canopy in canopyCollection)
//                {
//                    int iAreaIndex = 7;

//                    double width_temp;

//                    if (canopy.Right)
//                    {
//                        bool hasNextCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
//                        bool hasPreviousCanopy = CanopiesHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

//                        float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
//                        float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

//                        float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
//                        float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

//                        float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
//                        int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / m_RoofProps.widthRib_m);
//                        double dWidthOfWholeRibs = iNumberOfWholeRibs * m_RoofProps.widthRib_m;
//                        double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

//                        float fCanopyCladdingWidth = (float)canopy.WidthRight + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
//                        float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

//                        Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_1_final_edge_Roof);
//                        Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_1_final_edge_Roof);
//                        Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
//                        Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

//                        double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

//                        double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y);
//                        //if (options.bUseTextures)
//                        //{
//                        //    wpWidth = m_RoofProps.widthRib_m / (pback_left.Y - pfront_left.Y);
//                        //    wpHeight = m_RoofProps.widthRib_m / poinstsDist;

//                        //    double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y);

//                        //    ImageBrush brushRoofCanopy = brushRoof.Clone();
//                        //    System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//                        //    r.Location = new System.Windows.Point(-wpWidthOffset, 0);
//                        //    brushRoofCanopy.Viewport = r;
//                        //    material_Roof = new DiffuseMaterial(brushRoofCanopy);
//                        //}

//                        width_temp = pback_left.Y - pfront_left.Y;

//                        listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof_Right_Side, m_MaterialCladding_Roof,
//                            m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, m_RoofProps.widthModular_m,
//                            4, 0, 0,
//                            pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
//                            m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, m_RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

//                        /*
//                        CAreaPolygonal areaCR = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
//                        model_gr.Children.Add(areaCR.CreateArea(options.bUseTextures, material_Roof));
//                        areaCR.SetWireFramePoints();
//                        WireFramePoints.AddRange(areaCR.WireFramePoints);
//                        */
//                        iAreaIndex++;
//                    }

//                    if (canopy.Left)
//                    {
//                        bool hasNextCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
//                        bool hasPreviousCanopy = CanopiesHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

//                        float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
//                        float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

//                        float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
//                        float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

//                        float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
//                        int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / m_RoofProps.widthRib_m);
//                        double dWidthOfWholeRibs = iNumberOfWholeRibs * m_RoofProps.widthRib_m;
//                        double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

//                        float fCanopyCladdingWidth = (float)canopy.WidthLeft + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
//                        float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

//                        Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
//                        Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
//                        Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
//                        Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

//                        double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);

//                        double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne
//                        //if (options.bUseTextures)
//                        //{
//                        //    wpWidth = m_RoofProps.widthRib_m / (pback_left.Y - pfront_left.Y);
//                        //    wpHeight = m_RoofProps.widthRib_m / poinstsDist;

//                        //    double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

//                        //    ImageBrush brushRoofCanopy = brushRoof.Clone();
//                        //    System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
//                        //    r.Location = new System.Windows.Point(-wpWidthOffset, 0);
//                        //    brushRoofCanopy.Viewport = r;
//                        //    material_Roof = new DiffuseMaterial(brushRoofCanopy);
//                        //}

//                        width_temp = pback_left.Y - pfront_left.Y;

//                        listOfCladdingSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iAreaIndex, "RC", "Cladding - Roof", EBuildingSide.Roof_Left_Side, m_MaterialCladding_Roof,
//                            m_RoofProps.thicknessCore_m, m_RoofCoilProps.widthCoil, m_RoofCoilProps.coilmass_kg_m2, m_RoofCoilProps.price_PPSM_NZD, m_RoofProps.widthModular_m,
//                            4, 0, 0,
//                            pfront_right, width_temp, poinstsDist, poinstsDist, 0.5 * width_temp, poinstsDist,
//                            m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, m_RoofProps.widthRib_m, true, 0, false, true, wpWidthOffset));

//                        /*
//                        CAreaPolygonal areaCL = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
//                        model_gr.Children.Add(areaCL.CreateArea(options.bUseTextures, material_Roof));
//                        areaCL.SetWireFramePoints();
//                        WireFramePoints.AddRange(areaCL.WireFramePoints);
//                        */

//                        iAreaIndex++;
//                    }
//                }
//            }
//        }
//    }

//    // Vytvorime geometry model
//    double outOffPlaneOffset_FG = -0.005; // Pokial kreslime cladding ako jednoliatu plochu na celu stenu alebo strechu, nastavime offset, aby sa fibreglasa nevnarali do cladding
//    if (bIndividualCladdingSheets)
//    {
//        // Ak kreslime individualne sheets pre cladding nepotrebujeme offset
//        outOffPlaneOffset_FG = 0.000;
//    }

//    bool claddingWireframe = options.bDisplayWireFrameModel && options.bDisplayCladdingWireFrame;
//    bool fibreglassWireframe = options.bDisplayWireFrameModel && options.bDisplayCladdingWireFrame && options.bDisplayFibreglassWireFrame;

//    if (bGenerateLeftSideCladding && options.bDisplayCladdingLeftWall/* && bIndividualCladdingSheets*/)
//        AddSheet3DModelsToModelGroup(listOfCladdingSheetsLeftWall, options, brushSide, material_SideWall, m_WallProps.widthRib_m, 0, 0, -90, ref model_gr/*, claddingWireframe*/);

//    if (bGenerateLeftSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
//        AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallLeft, options, brushWall_FG, material_Wall_FG, m_WallProps_FG.widthRib_m, 0, 0, -90, ref model_gr/*, fibreglassWireframe*/, outOffPlaneOffset_FG);

//    if (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall/* && bIndividualCladdingSheets*/)
//        AddSheet3DModelsToModelGroup(listOfCladdingSheetsFrontWall, options, brushFront, material_FrontBackWall, m_WallProps.widthRib_m, 0, 0, 0, ref model_gr/*, claddingWireframe*/);

//    if (bGenerateFrontSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
//        AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallFront, options, brushWall_FG, material_Wall_FG, m_WallProps_FG.widthRib_m, 0, 0, 0, ref model_gr/*, fibreglassWireframe*/, outOffPlaneOffset_FG);

//    if (bGenerateRightSideCladding && options.bDisplayCladdingRightWall/* && bIndividualCladdingSheets*/)
//        AddSheet3DModelsToModelGroup(listOfCladdingSheetsRightWall, options, brushSide, material_SideWall, m_WallProps.widthRib_m, 0, 0, 90, ref model_gr/*, claddingWireframe*/);

//    if (bGenerateRightSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
//        AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallRight, options, brushWall_FG, material_Wall_FG, m_WallProps_FG.widthRib_m, 0, 0, 90, ref model_gr/*, fibreglassWireframe*/, outOffPlaneOffset_FG);

//    if (bGenerateBackSideCladding && options.bDisplayCladdingBackWall/* && bIndividualCladdingSheets*/)
//        AddSheet3DModelsToModelGroup(listOfCladdingSheetsBackWall, options, brushFront, material_FrontBackWall, m_WallProps.widthRib_m, 0, 0, 180, ref model_gr/*, claddingWireframe*/);

//    if (bGenerateBackSideFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
//        AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallBack, options, brushWall_FG, material_Wall_FG, m_WallProps_FG.widthRib_m, 0, 0, 180, ref model_gr/*, fibreglassWireframe*/, outOffPlaneOffset_FG);

//    // Set rotation about GCS X-axis - Roof - Right Side (Gable Roof) and Monopitch Roof
//    double rotationAboutX = -90f + (eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? sBuildingGeomInputData.fRoofPitch_deg : -sBuildingGeomInputData.fRoofPitch_deg);

//    if (bGenerateRoofCladding && options.bDisplayCladdingRoof/* && bIndividualCladdingSheets*/)
//        AddSheet3DModelsToModelGroup(listOfCladdingSheetsRoofRight, options, brushRoof, material_Roof, m_RoofProps.widthRib_m, rotationAboutX, 0, 90, ref model_gr/*, claddingWireframe*/);

//    if (bGenerateRoofFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
//        AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsRoofRight, options, brushRoof_FG, material_Roof_FG, m_RoofProps_FG.widthRib_m, rotationAboutX, 0, 90, ref model_gr/*, fibreglassWireframe*/, outOffPlaneOffset_FG);

//    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
//    {
//        // Set rotation about GCS X-axis - Roof - Left Side (Gable Roof)
//        rotationAboutX = -90f - sBuildingGeomInputData.fRoofPitch_deg;

//        if (bGenerateRoofCladding && options.bDisplayCladdingRoof/* && bIndividualCladdingSheets*/)
//            AddSheet3DModelsToModelGroup(listOfCladdingSheetsRoofLeft, options, brushRoof, material_Roof, m_RoofProps.widthRib_m, rotationAboutX, 0, 90, ref model_gr/*, claddingWireframe*/);

//        if (bGenerateRoofFibreglass && (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame))
//            AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsRoofLeft, options, brushRoof_FG, material_Roof_FG, m_RoofProps_FG.widthRib_m, rotationAboutX, 0, 90, ref model_gr/*, fibreglassWireframe*/, outOffPlaneOffset_FG);
//    }
//    CountRealLenghts();
//    return model_gr;
//}