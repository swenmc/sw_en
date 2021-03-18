using BaseClasses.Helpers;
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
    public class CCladding : CEntity3D
    {
        EModelType_FS eModelType;
        BuildingGeometryDataInput sBuildingGeomInputData;
        System.Collections.ObjectModel.ObservableCollection<CCanopiesInfo> canopyCollection;
        System.Collections.ObjectModel.ObservableCollection<CBayInfo> bayWidthCollection;
        System.Collections.ObjectModel.ObservableCollection<FibreglassProperties> fibreglassSheetCollection;
        System.Collections.ObjectModel.ObservableCollection<DoorProperties> doorPropCollection;
        System.Collections.ObjectModel.ObservableCollection<WindowProperties> windowPropCollection;

        float m_fFrontColumnDistance; // Vzdialenost front columns
        float m_fBackColumnDistance; // Vzdialenost back columns

        double claddingHeight_Wall; // z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m v tabulkach tableSections_m alebo trapezoidalSheeting_m
        double claddingHeight_Roof; // z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m
        double claddingWidthRibModular_Wall;
        double claddingWidthRibModular_Roof;
        double claddingWidthModular_Wall;
        double claddingWidthModular_Roof;

        double column_crsc_z_plus;
        double column_crsc_y_minus;
        double column_crsc_y_plus;

        string m_claddingShape_Wall;
        string m_claddingCoatingType_Wall;
        string m_claddingShape_Roof;
        string m_claddingCoatingType_Roof;

        string m_ColorNameWall;
        string m_ColorNameRoof;

        Color m_ColorWall;
        Color m_ColorRoof;

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
        double claddingWidthRibModular_Roof_FG;
        double claddingWidthModular_Roof_FG;

        string m_ColorNameWall_FG;
        string m_claddingShape_Wall_FG;
        string m_claddingCoatingType_Wall_FG;
        Color m_ColorWall_FG;
        double claddingWidthRibModular_Wall_FG;
        double claddingWidthModular_Wall_FG;

        bool bIndividualCladdingSheets;

        bool bGenerateLeftSideCladding = true;
        bool bGenerateFrontSideCladding = true;
        bool bGenerateRightSideCladding = true;
        bool bGenerateBackSideCladding = true;
        bool bGenerateRoofCladding = true;

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

        private List<Point3D> m_WireFramePoints;
        public List<Point3D> WireFramePoints
        {
            get
            {
                if (m_WireFramePoints == null) m_WireFramePoints = new List<Point3D>();
                return m_WireFramePoints;
            }

            set
            {
                m_WireFramePoints = value;
            }
        }

        public CCladding()
        {

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
            string colorName_Wall, string colorName_Roof, string claddingShape_Wall, string claddingCoatingType_Wall, string claddingShape_Roof, string claddingCoatingType_Roof,
            Color colorWall, Color colorRoof,
            Color colorWall_FG, Color colorRoof_FG,
            string colorWall_FG_Name, string colorRoof_FG_Name,
            bool bIsDisplayed, int fTime,
            double wallCladdingHeight,
            double roofCladdingHeight,
            double wallCladdingWidthRib,
            double roofCladdingWidthRib,
            float wallCladdingWidthModular,
            float roofCladdingWidthModular,
            float fRoofEdgeOverHang_FB_Y,
            float fRoofEdgeOverHang_LR_X,
            float fCanopyRoofEdgeOverHang_LR_X,
            float fWallBottomOffset_Z,
            bool bConsiderRoofCladdingFor_FB_WallHeight)
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
            column_crsc_z_plus = columnSection.z_max;
            column_crsc_y_minus = columnSection.y_min;
            column_crsc_y_plus = columnSection.y_max;
            m_fFrontColumnDistance = fFrontColumnDistance;
            m_fBackColumnDistance = fBackColumnDistance;
            m_ColorNameWall = colorName_Wall;
            m_ColorNameRoof = colorName_Roof;
            m_claddingShape_Wall = claddingShape_Wall;
            m_claddingCoatingType_Wall = claddingCoatingType_Wall;
            m_claddingShape_Roof = claddingShape_Roof;
            m_claddingCoatingType_Roof = claddingCoatingType_Roof;

            SetCladdingGenerateProperties(componentList);

            m_ColorWall = colorWall;
            m_ColorRoof = colorRoof;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            claddingHeight_Wall = wallCladdingHeight;
            claddingHeight_Roof = roofCladdingHeight;
            claddingWidthRibModular_Wall = wallCladdingWidthRib;
            claddingWidthRibModular_Roof = roofCladdingWidthRib;
            claddingWidthModular_Wall = wallCladdingWidthModular;
            claddingWidthModular_Roof = roofCladdingWidthModular;

            roofEdgeOverhang_Y = fRoofEdgeOverHang_FB_Y;
            roofEdgeOverhang_X = fRoofEdgeOverHang_LR_X;
            canopyOverhangOffset_x = fCanopyRoofEdgeOverHang_LR_X;
            bottomEdge_z = fWallBottomOffset_Z;
            considerRoofCladdingFor_FB_WallHeight = bConsiderRoofCladdingFor_FB_WallHeight;

            canopyOverhangOffset_y = roofEdgeOverhang_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

            m_ColorNameRoof_FG = colorRoof_FG_Name;
            m_claddingShape_Roof_FG = m_claddingShape_Roof;
            m_claddingCoatingType_Roof_FG = "";
            m_ColorRoof_FG = colorRoof_FG; 
            claddingWidthRibModular_Roof_FG = roofCladdingWidthRib;
            claddingWidthModular_Roof_FG = claddingWidthModular_Roof;

            m_ColorNameWall_FG = colorWall_FG_Name;
            m_claddingShape_Wall_FG = m_claddingShape_Wall;
            m_claddingCoatingType_Wall_FG = "";
            m_ColorWall_FG = colorWall_FG; 
            claddingWidthRibModular_Wall_FG = wallCladdingWidthRib;
            claddingWidthModular_Wall_FG = claddingWidthModular_Wall;
        }

        public Model3DGroup GetCladdingModel(DisplayOptions options)
        {
            ControlPoint = new Point3D(0, 0, 0);

            Model3DGroup model_gr = new Model3DGroup();
            WireFramePoints = new List<Point3D>();

            // Vytvorime model v GCS [0,0,0] je uvazovana v bode m_ControlPoint

            double additionalOffset = 0.005;  // 5 mm Aby nekolidovali plochy cladding s members
            double additionalOffsetRoof = 0.010; // Aby nekolidovali plochy cladding s members (cross-bracing) na streche

            // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
            double column_crsc_y_minus_temp = column_crsc_y_minus - additionalOffset;
            double column_crsc_y_plus_temp = column_crsc_y_plus + additionalOffset;
            double column_crsc_z_plus_temp = column_crsc_z_plus + additionalOffset;

            //----------------------------------------
            // To Ondrej - toto by sme mali nahradit funkciou
            // CalculateWallHeightsForCladding
            double height_1_final = sBuildingGeomInputData.fH_1_centerline + column_crsc_z_plus / Math.Cos(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
            double height_2_final = sBuildingGeomInputData.fH_2_centerline + column_crsc_z_plus / Math.Cos(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon
            
            double height_1_final_edge_LR_Wall = height_1_final - column_crsc_z_plus_temp * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            double height_2_final_edge_LR_Wall = height_2_final;
            
            double height_1_final_edge_Roof = height_1_final + additionalOffsetRoof - (column_crsc_z_plus_temp + roofEdgeOverhang_X) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            double height_2_final_edge_Roof = height_2_final + additionalOffsetRoof;
            
            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                height_2_final_edge_LR_Wall = height_2_final + column_crsc_z_plus_temp * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                height_2_final_edge_Roof = height_2_final + additionalOffsetRoof + (column_crsc_z_plus_temp + roofEdgeOverhang_X) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            }
            
            // Nastavime rovnaku vysku hornej hrany
            double height_1_final_edge_FB_Wall = height_1_final_edge_LR_Wall;
            double height_2_final_edge_FB_Wall = height_2_final_edge_LR_Wall;
            
            if (considerRoofCladdingFor_FB_WallHeight)
            {
                height_1_final_edge_FB_Wall = height_1_final_edge_FB_Wall + claddingHeight_Roof * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                height_2_final_edge_FB_Wall = height_2_final_edge_FB_Wall + claddingHeight_Roof * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            
                if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                    height_2_final_edge_FB_Wall = height_2_final + (column_crsc_z_plus_temp + claddingHeight_Roof) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            }
            //----------------------------------------

            // Wall Cladding Edges

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, bottomEdge_z);
            Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, bottomEdge_z);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, bottomEdge_z);
            Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, bottomEdge_z);

            Brush solidBrushFront = new SolidColorBrush(m_ColorWall);
            Brush solidBrushSide = new SolidColorBrush(m_ColorWall);
            Brush solidBrushRoof = new SolidColorBrush(m_ColorRoof);

            Brush solidBrushWall_FG = new SolidColorBrush(m_ColorWall_FG);
            Brush solidBrushRoof_FG = new SolidColorBrush(m_ColorRoof_FG);

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

            double wpWidth = 0;
            double wpHeight = 0;

            if (options.bUseTextures)
            {
                //CS ENDURA® a CS MAXX® maju rovnake farby, takze pre nich coating type v nazve suboru nepouzivam
                string uriString_Wall = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + m_ColorNameWall + ".jpg";

                if (m_claddingCoatingType_Wall == "FORMCLAD®")
                {
                    string claddingCoatingType_Wall_string = "FORMCLAD";
                    uriString_Wall = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + claddingCoatingType_Wall_string + "_" + m_ColorNameWall + ".jpg";
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
                string uriString_Roof = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + m_ColorNameRoof + ".jpg";

                if (m_claddingCoatingType_Roof == "FORMCLAD®")
                {
                    string claddingCoatingType_Roof_string = "FORMCLAD";
                    uriString_Roof = "pack://application:,,,/Resources/Textures/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + claddingCoatingType_Roof_string + "_" + m_ColorNameRoof + ".jpg";
                }

                brushRoof = new ImageBrush();
                brushRoof.ImageSource = new BitmapImage(new Uri(uriString_Roof, UriKind.RelativeOrAbsolute));
                brushRoof.TileMode = TileMode.Tile;
                brushRoof.ViewportUnits = BrushMappingMode.Absolute;
                brushRoof.Stretch = Stretch.Fill;
                brushRoof.Opacity = options.fRoofCladdingOpacity;

                // Fibreglass
                //string uriString_FG_Wall = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + m_ColorNameWall_FG + ".jpg";
                string uriString_FG_Wall = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Wall + "/" + m_claddingShape_Wall + "_" + "White" + ".jpg";

                brushWall_FG = new ImageBrush();
                brushWall_FG.ImageSource = new BitmapImage(new Uri(uriString_FG_Wall, UriKind.RelativeOrAbsolute));
                brushWall_FG.TileMode = TileMode.Tile;
                brushWall_FG.ViewportUnits = BrushMappingMode.Absolute;
                brushWall_FG.Stretch = Stretch.Fill;
                brushWall_FG.Opacity = options.fFibreglassOpacity;

                //string uriString_FG_Roof = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + m_ColorNameRoof_FG + ".jpg";
                string uriString_FG_Roof = "pack://application:,,,/Resources/Textures/Fibreglass/" + m_claddingShape_Roof + "/" + m_claddingShape_Roof + "_" + "White" + ".jpg";

                brushRoof_FG = new ImageBrush();
                brushRoof_FG.ImageSource = new BitmapImage(new Uri(uriString_FG_Roof, UriKind.RelativeOrAbsolute));
                brushRoof_FG.TileMode = TileMode.Tile;
                brushRoof_FG.ViewportUnits = BrushMappingMode.Absolute;
                brushRoof_FG.Stretch = Stretch.Fill;
                brushRoof_FG.Opacity = options.fFibreglassOpacity;
            }

            // Wall Points
            Point3D pLRWall_front2_heightright = new Point3D();
            Point3D pLRWall_back2_heightright = new Point3D();
            Point3D pLRWall_front3_heightleft = new Point3D();
            Point3D pLRWall_back3_heightleft = new Point3D();
            Point3D pLRWall_front4_top = new Point3D();
            Point3D pLRWall_back4_top = new Point3D();

            Point3D pFBWall_front2_heightright = new Point3D();
            Point3D pFBWall_back2_heightright = new Point3D();
            Point3D pFBWall_front3_heightleft = new Point3D();
            Point3D pFBWall_back3_heightleft = new Point3D();
            Point3D pFBWall_front4_top = new Point3D();
            Point3D pFBWall_back4_top = new Point3D();

            // Roof Points - oddelene pretoze strecha ma presahy
            Point3D pRoof_front2_heightright = new Point3D();
            Point3D pRoof_back2_heightright = new Point3D();
            Point3D pRoof_front3_heightleft = new Point3D();
            Point3D pRoof_back3_heightleft = new Point3D();
            Point3D pRoof_front4_top = new Point3D();
            Point3D pRoof_back4_top = new Point3D();

            // Roof edge offset from centerline in Y-direction
            float fRoofEdgeOffsetFromCenterline = -(float)column_crsc_y_minus_temp + (float)roofEdgeOverhang_Y;

            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                // Monopitch Roof

                // Wall
                pLRWall_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_2_final_edge_FB_Wall);
                pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_FB_Wall);

                pLRWall_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_2_final_edge_FB_Wall);
                pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_FB_Wall);

                pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, height_2_final_edge_FB_Wall);
                pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, height_2_final_edge_FB_Wall);
                pFBWall_front3_heightleft = new Point3D(pLRWall_front3_heightleft.X, pLRWall_front3_heightleft.Y, height_1_final_edge_FB_Wall);
                pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, height_1_final_edge_FB_Wall);

                // Roof
                pRoof_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_2_final_edge_Roof);
                pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_1_final_edge_Roof);

                pRoof_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_2_final_edge_Roof);
                pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_1_final_edge_Roof);

                if (options.bUseTextures) // Pouzijeme len ak vykreslujeme textury, inak sa pouzije material vytvoreny z SolidColorBrush podla vybranej farby cladding v GUI
                {
                    wpWidth = claddingWidthRibModular_Wall / (pfront1_baseright.X - pfront0_baseleft.X);
                    wpHeight = claddingWidthRibModular_Wall / (pFBWall_front2_heightright.Z - pfront1_baseright.Z);
                    brushFront.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_FrontBackWall = new DiffuseMaterial(brushFront);
                }

                // Front Wall
                if (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pFBWall_front2_heightright, pFBWall_front3_heightleft }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }

                // Back Wall
                if (bGenerateBackSideCladding && options.bDisplayCladdingBackWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pFBWall_back3_heightleft, pFBWall_back2_heightright }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }
                    

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                    wpHeight = claddingWidthRibModular_Wall / (pLRWall_back2_heightright.Z - pback0_baseleft.Z);
                    brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_SideWall = new DiffuseMaterial(brushSide);
                }

                // Left Wall
                if (bGenerateLeftSideCladding && options.bDisplayCladdingLeftWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pLRWall_front3_heightleft, pLRWall_back3_heightleft }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }

                // Right Wall
                if (bGenerateRightSideCladding && options.bDisplayCladdingRightWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pLRWall_back2_heightright, pLRWall_front2_heightright }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }                    

                if (options.bUseTextures)
                {
                    double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pRoof_front2_heightright, pRoof_front3_heightleft); // Rovina XZ
                    wpWidth = claddingWidthRibModular_Roof / (pRoof_back2_heightright.Y - pRoof_front2_heightright.Y);
                    wpHeight = claddingWidthRibModular_Roof / poinstsDist;
                    brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_Roof = new DiffuseMaterial(brushRoof);
                }

                // Roof
                if (bGenerateRoofCladding && options.bDisplayCladdingRoof && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(4, new List<Point3D>() { pRoof_front2_heightright, pRoof_back2_heightright, pRoof_back3_heightleft, pRoof_front3_heightleft }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_Roof));
                    WireFramePoints.AddRange(area.GetWireFrame());

                    // Canopies
                    foreach (CCanopiesInfo canopy in canopyCollection)
                    {
                        int iAreaIndex = 5;

                        if (canopy.Left)
                        {
                            // 2 ______ 1
                            //  |      |
                            //  |      |
                            //  |______|
                            // 3        0

                            bool hasNextCanopy = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                            bool hasPreviousCanopy = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                            float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                            float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                            float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                            float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                            float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
                            int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                            double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                            double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                            float fCanopyCladdingWidth = (float)canopy.WidthLeft + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                            float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                            Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                            Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                            Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
                            Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

                            if (options.bUseTextures)
                            {
                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);
                                wpWidth = claddingWidthRibModular_Roof / (pback_left.Y - pfront_left.Y);
                                wpHeight = claddingWidthRibModular_Roof / poinstsDist;

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

                                ImageBrush brushRoofCanopy = brushRoof.Clone();
                                System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                                r.Location = new System.Windows.Point(-wpWidthOffset, 0);
                                brushRoofCanopy.Viewport = r;
                                material_Roof = new DiffuseMaterial(brushRoofCanopy);
                            }

                            CAreaPolygonal areaCL = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
                            model_gr.Children.Add(areaCL.CreateArea(options.bUseTextures, material_Roof));
                            WireFramePoints.AddRange(areaCL.GetWireFrame());
                            iAreaIndex++;
                        }

                        if (canopy.Right)
                        {
                            bool hasNextCanopy = ModelHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                            bool hasPreviousCanopy = ModelHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                            float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                            float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                            float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                            float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                            float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
                            int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                            double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                            double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                            float fCanopyCladdingWidth = (float)canopy.WidthRight + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                            float fCanopy_EdgeCoordinate_z = (float)height_2_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                            Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_2_final_edge_Roof);
                            Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_2_final_edge_Roof);
                            Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
                            Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

                            if (options.bUseTextures)
                            {
                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);
                                wpWidth = claddingWidthRibModular_Roof / (pback_left.Y - pfront_left.Y);
                                wpHeight = claddingWidthRibModular_Roof / poinstsDist;

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

                                ImageBrush brushRoofCanopy = brushRoof.Clone();
                                System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                                r.Location = new System.Windows.Point(-wpWidthOffset, 0);
                                brushRoofCanopy.Viewport = r;
                                material_Roof = new DiffuseMaterial(brushRoofCanopy);
                            }

                            CAreaPolygonal areaCR = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
                            model_gr.Children.Add(areaCR.CreateArea(options.bUseTextures, material_Roof));
                            WireFramePoints.AddRange(areaCR.GetWireFrame());
                            iAreaIndex++;
                        }
                    }
                }
            }
            else if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Gable Roof Building

                // Wall
                pLRWall_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_LR_Wall);
                pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, height_1_final_edge_LR_Wall);
                pLRWall_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp, height_2_final_edge_LR_Wall);

                pLRWall_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_LR_Wall);
                pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_1_final_edge_LR_Wall);
                pLRWall_back4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, height_2_final_edge_LR_Wall);

                pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, height_1_final_edge_FB_Wall);
                pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, height_1_final_edge_FB_Wall);
                pFBWall_front3_heightleft = new Point3D(pLRWall_front3_heightleft.X, pLRWall_front3_heightleft.Y, height_1_final_edge_FB_Wall);
                pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, height_1_final_edge_FB_Wall);
                pFBWall_front4_top = new Point3D(pLRWall_front4_top.X, pLRWall_front4_top.Y, height_2_final_edge_FB_Wall);
                pFBWall_back4_top = new Point3D(pLRWall_back4_top.X, pLRWall_back4_top.Y, height_2_final_edge_FB_Wall);

                // Roof
                pRoof_front2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_front4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp - roofEdgeOverhang_Y, height_2_final_edge_Roof);

                pRoof_back2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_temp - roofEdgeOverhang_X, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_1_final_edge_Roof);
                pRoof_back4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + roofEdgeOverhang_Y, height_2_final_edge_Roof);

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pfront1_baseright.X - pfront0_baseleft.X);
                    wpHeight = claddingWidthRibModular_Wall / (pFBWall_front4_top.Z - pfront1_baseright.Z);
                    brushFront.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_FrontBackWall = new DiffuseMaterial(brushFront);
                }

                // Front Wall
                if (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pFBWall_front2_heightright, pFBWall_front4_top, pFBWall_front3_heightleft }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }

                // Back Wall
                if (bGenerateBackSideCladding && options.bDisplayCladdingBackWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pFBWall_back3_heightleft, pFBWall_back4_top, pFBWall_back2_heightright }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_FrontBackWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }
                    

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                    wpHeight = claddingWidthRibModular_Wall / (pLRWall_back3_heightleft.Z - pback0_baseleft.Z);
                    brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_SideWall = new DiffuseMaterial(brushSide);
                }

                // Left Wall
                if (bGenerateLeftSideCladding && options.bDisplayCladdingLeftWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pLRWall_front3_heightleft, pLRWall_back3_heightleft }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }
                    
                // Right Wall
                if (bGenerateRightSideCladding && options.bDisplayCladdingRightWall && !bIndividualCladdingSheets)
                {
                    CAreaPolygonal area = new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pLRWall_back2_heightright, pLRWall_front2_heightright }, 0);
                    model_gr.Children.Add(area.CreateArea(options.bUseTextures, material_SideWall));
                    WireFramePoints.AddRange(area.GetWireFrame());
                }

                if (options.bUseTextures)
                {
                    double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pRoof_front4_top, pRoof_front3_heightleft); // Rovina XZ
                    wpWidth = claddingWidthRibModular_Roof / (pRoof_back4_top.Y - pRoof_front4_top.Y);
                    wpHeight = claddingWidthRibModular_Roof / poinstsDist;
                    brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_Roof = new DiffuseMaterial(brushRoof);
                }

                if (bGenerateRoofCladding && options.bDisplayCladdingRoof && !bIndividualCladdingSheets)
                {
                    // Roof - Left Side
                    CAreaPolygonal areaL = new CAreaPolygonal(4, new List<Point3D>() { pRoof_front4_top, pRoof_back4_top, pRoof_back3_heightleft, pRoof_front3_heightleft }, 0);
                    model_gr.Children.Add(areaL.CreateArea(options.bUseTextures, material_Roof));
                    WireFramePoints.AddRange(areaL.GetWireFrame());
                    // Roof - Right Side
                    CAreaPolygonal areaR = new CAreaPolygonal(5, new List<Point3D>() { pRoof_front2_heightright, pRoof_back2_heightright, pRoof_back4_top, pRoof_front4_top }, 0);
                    model_gr.Children.Add(areaR.CreateArea(options.bUseTextures, material_Roof));
                    WireFramePoints.AddRange(areaR.GetWireFrame());

                    // Canopies
                    foreach (CCanopiesInfo canopy in canopyCollection)
                    {
                        int iAreaIndex = 6;

                        if (canopy.Left)
                        {
                            bool hasNextCanopy = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                            bool hasPreviousCanopy = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                            float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                            float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                            float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                            float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                            float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
                            int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                            double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                            double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                            float fCanopyCladdingWidth = (float)canopy.WidthLeft + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                            float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                            Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                            Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                            Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
                            Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

                            if (options.bUseTextures)
                            {
                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);
                                wpWidth = claddingWidthRibModular_Roof / (pback_left.Y - pfront_left.Y);
                                wpHeight = claddingWidthRibModular_Roof / poinstsDist;

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

                                ImageBrush brushRoofCanopy = brushRoof.Clone();
                                System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                                r.Location = new System.Windows.Point(-wpWidthOffset, 0);
                                brushRoofCanopy.Viewport = r;
                                material_Roof = new DiffuseMaterial(brushRoofCanopy);
                            }

                            CAreaPolygonal areaCL = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
                            model_gr.Children.Add(areaCL.CreateArea(options.bUseTextures, material_Roof));
                            WireFramePoints.AddRange(areaCL.GetWireFrame());

                            iAreaIndex++;
                        }

                        if (canopy.Right)
                        {
                            bool hasNextCanopy = ModelHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                            bool hasPreviousCanopy = ModelHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                            float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                            float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                            float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffset;
                            float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffset;

                            float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
                            int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                            double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                            double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                            float fCanopyCladdingWidth = (float)canopy.WidthRight + (float)canopyOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                            float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                            Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_1_final_edge_Roof);
                            Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_1_final_edge_Roof);
                            Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
                            Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

                            if (options.bUseTextures)
                            {
                                double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront_right, pfront_left);
                                wpWidth = claddingWidthRibModular_Roof / (pback_left.Y - pfront_left.Y);
                                wpHeight = claddingWidthRibModular_Roof / poinstsDist;

                                double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y);

                                ImageBrush brushRoofCanopy = brushRoof.Clone();
                                System.Windows.Rect r = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                                r.Location = new System.Windows.Point(-wpWidthOffset, 0);
                                brushRoofCanopy.Viewport = r;
                                material_Roof = new DiffuseMaterial(brushRoofCanopy);
                            }
                            CAreaPolygonal areaCR = new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0);
                            model_gr.Children.Add(areaCR.CreateArea(options.bUseTextures, material_Roof));
                            WireFramePoints.AddRange(areaCR.GetWireFrame());
                            iAreaIndex++;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Not implemented kitset type.");
            }

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
            bool bUseTop20Colors = true; // Pouzit vsetky farby v zozname (141) alebo striedat len vybrane (20)

            // Left Wall
            // Total Wall Width
            double width = pback0_baseleft.Y - pfront0_baseleft.Y;
            double height_left_basic = -bottomEdge_z + height_1_final_edge_LR_Wall;
            Point3D pControlPoint_LeftWall = new Point3D(pback0_baseleft.X, pback0_baseleft.Y, pback0_baseleft.Z);

            int iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            double dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            double dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            int iNumberOfSheets = iNumberOfWholeSheets + 1;

            listOfFibreGlassSheetsWallLeft = new List<CCladdingOrFibreGlassSheet>();

            foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
            {
                if (fgsp.Side == "Left")
                {
                    listOfFibreGlassSheetsWallLeft.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                        pControlPoint_LeftWall, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : claddingWidthModular_Wall_FG, fgsp.Length, fgsp.Length, 0, 0,
                        m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                    iSheet_FG_Index++;
                }
            }

            List<COpening> listOfOpeningsLeftWall_All = null;
            GenerateCladdingOpenings(listOfFibreGlassSheetsWallLeft, "Left", pControlPoint_LeftWall, width, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
            ref iOpeningIndex, out listOfOpeningsLeftWall_All);

            if (bGenerateLeftSideCladding && bIndividualCladdingSheets)
            {
                GenerateCladdingSheets(options.bCladdingSheetColoursByID, bUseTop20Colors, "Left", pControlPoint_LeftWall, m_ColorNameWall,
                m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity, width,
                claddingWidthRibModular_Wall, claddingWidthModular_Wall, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_left_basic,
                listOfOpeningsLeftWall_All, ref iSheetIndex, out listOfCladdingSheetsLeftWall);
            }

            // Front Wall
            // Total Wall Width
            width = pfront1_baseright.X - pfront0_baseleft.X;
            height_left_basic = -bottomEdge_z + height_1_final_edge_FB_Wall;

            Point3D pControlPoint_FrontWall = new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z);
            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            listOfFibreGlassSheetsWallFront = new List<CCladdingOrFibreGlassSheet>();

            foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
            {
                if (fgsp.Side == "Front")
                {
                    listOfFibreGlassSheetsWallFront.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                        pControlPoint_FrontWall, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : claddingWidthModular_Wall_FG, fgsp.Length, fgsp.Length, 0, 0,
                        m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                    iSheet_FG_Index++;
                }
            }

            List<COpening> listOfOpeningsFrontWall_All = null;
            GenerateCladdingOpenings(listOfFibreGlassSheetsWallFront, "Front", pControlPoint_FrontWall, width, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
            ref iOpeningIndex, out listOfOpeningsFrontWall_All);

            if (bGenerateFrontSideCladding &&  bIndividualCladdingSheets)
            {
                GenerateCladdingSheets(options.bCladdingSheetColoursByID, bUseTop20Colors, "Front", pControlPoint_FrontWall, m_ColorNameWall,
                m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, width,
                claddingWidthRibModular_Wall, claddingWidthModular_Wall, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_2_final_edge_FB_Wall,
                listOfOpeningsFrontWall_All, ref iSheetIndex, out listOfCladdingSheetsFrontWall);
            }

            // Right Wall
            // Total Wall Width
            width = pback1_baseright.Y - pfront1_baseright.Y;
            height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_LR_Wall : -bottomEdge_z + height_2_final_edge_LR_Wall;
            Point3D pControlPoint_RightWall = new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z);

            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            listOfFibreGlassSheetsWallRight = new List<CCladdingOrFibreGlassSheet>();

            foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
            {
                if (fgsp.Side == "Right")
                {
                    listOfFibreGlassSheetsWallRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                pControlPoint_RightWall, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : claddingWidthModular_Wall_FG, fgsp.Length, fgsp.Length, 0, 0,
                m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                    iSheet_FG_Index++;
                }
            }

            List<COpening> listOfOpeningsRightWall_All = null;
            GenerateCladdingOpenings(listOfFibreGlassSheetsWallRight, "Right", pControlPoint_RightWall, width, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
            ref iOpeningIndex, out listOfOpeningsRightWall_All);

            if (bGenerateRightSideCladding && bIndividualCladdingSheets)
            {
                GenerateCladdingSheets(options.bCladdingSheetColoursByID, bUseTop20Colors, "Right", pControlPoint_RightWall, m_ColorNameWall,
                m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity, width,
                claddingWidthRibModular_Wall, claddingWidthModular_Wall, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_left_basic,
                listOfOpeningsRightWall_All, ref iSheetIndex, out listOfCladdingSheetsRightWall);
            }

            // Back Wall
            // Total Wall Width
            width = pback1_baseright.X - pback0_baseleft.X;
            height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_FB_Wall : -bottomEdge_z + height_2_final_edge_FB_Wall;

            Point3D pControlPoint_BackWall = new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z);
            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            listOfFibreGlassSheetsWallBack = new List<CCladdingOrFibreGlassSheet>();

            foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
            {
                if (fgsp.Side == "Back")
                {
                    listOfFibreGlassSheetsWallBack.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                         pControlPoint_BackWall, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : claddingWidthModular_Wall_FG, fgsp.Length, fgsp.Length, 0, 0,
                         m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, options.fFibreglassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                    iSheet_FG_Index++;
                }
            }

            List<COpening> listOfOpeningsBackWall_All = null;
            GenerateCladdingOpenings(listOfFibreGlassSheetsWallBack, "Back", pControlPoint_BackWall, width, iNumberOfEdges_FG_D_W, column_crsc_y_minus_temp, column_crsc_z_plus_temp,
            ref iOpeningIndex, out listOfOpeningsBackWall_All);

            if (bGenerateBackSideCladding && bIndividualCladdingSheets)
            {
                GenerateCladdingSheets(options.bCladdingSheetColoursByID, bUseTop20Colors, "Back", pControlPoint_BackWall, m_ColorNameWall,
                m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, width,
                claddingWidthRibModular_Wall, claddingWidthModular_Wall, iNumberOfSheets, dPartialSheet_End, height_left_basic, height_2_final_edge_FB_Wall,
                listOfOpeningsBackWall_All, ref iSheetIndex, out listOfCladdingSheetsBackWall);
            }

            // Roof - Right Side
            // Total Width
            width = pRoof_back2_heightright.Y - pRoof_front2_heightright.Y;

            double length_left_basic;

            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
                length_left_basic = Drawing3D.GetPoint3DDistanceDouble(pRoof_front2_heightright, pRoof_front4_top);
            else
                length_left_basic = Drawing3D.GetPoint3DDistanceDouble(pRoof_front3_heightleft, pRoof_front2_heightright);

            Point3D pControlPoint_RoofRight = new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z);
            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Roof);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Roof;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            listOfFibreGlassSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();

            foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
            {
                if (fgsp.Side == "Roof" || fgsp.Side == "Roof-Right Side")
                {
                    listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_D_W, fgsp.X, fgsp.Y,
                         pControlPoint_RoofRight, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : claddingWidthModular_Roof_FG, fgsp.Length, fgsp.Length, 0, 0,
                         m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, options.fFibreglassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                    iSheet_FG_Index++;
                }
            }

            if (bGenerateRoofCladding && bIndividualCladdingSheets)
            {
                GenerateCladdingSheets(options.bCladdingSheetColoursByID, bUseTop20Colors, "Roof-right", pControlPoint_RoofRight, m_ColorNameRoof,
                m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, width,
                claddingWidthRibModular_Roof, claddingWidthModular_Roof, iNumberOfSheets, dPartialSheet_End, length_left_basic, length_left_basic,
                SheetListToOpeningListConverter(listOfFibreGlassSheetsRoofRight), ref iSheetIndex, out listOfCladdingSheetsRoofRight);

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
                            bool hasNextCanopyRight = ModelHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                            bool hasPreviousCanopyRight = ModelHelper.IsNeighboringRightCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

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

                                    originalsheet.CoordinateInPlane_y -= CanopyCladdingWidth_Right;
                                    originalsheet.LengthTopLeft += CanopyCladdingWidth_Right;
                                    originalsheet.LengthTopRight += CanopyCladdingWidth_Right;
                                    //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                    originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);

                                    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed || (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && !canopy.Left))
                                        breakIndex = cIndex + 1;
                                }
                            }

                            // Pre monopitch upravujeme aj lavu stranu plechu
                            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                            {
                                bool hasNextCanopyLeft = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                                bool hasPreviousCanopyLeft = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

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

                                        //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth; // Ostava povodne
                                        originalsheet.LengthTopLeft += CanopyCladdingWidth_Left;
                                        originalsheet.LengthTopRight += CanopyCladdingWidth_Left;
                                        //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                        originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);

                                        breakIndex = cIndex + 1;
                                    }
                                }
                            }
                            if (cIndex == breakIndex) break;
                            cIndex++;
                        }
                    }
                }
            }
            // Roof - Left Side

            listOfFibreGlassSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();

            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Roof - Left Side
                // Total Width
                width = pRoof_back3_heightleft.Y - pRoof_front3_heightleft.Y;

                Point3D pControlPoint_RoofLeft = new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z);
                iNumberOfWholeSheets = (int)(width / claddingWidthModular_Roof);
                dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Roof;
                dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                iNumberOfSheets = iNumberOfWholeSheets + 1;

                foreach (FibreglassProperties fgsp in fibreglassSheetCollection)
                {
                    if (fgsp.Side == "Roof-Left Side")
                    {
                        length_left_basic = Drawing3D.GetPoint3DDistanceDouble(pRoof_front3_heightleft, pRoof_front4_top);

                        // Pre Left side prevratime suradnice v LCS y, aby boli vstupy na oboch stranach brane od spodnej hrany H1
                        double Position_y = length_left_basic - fgsp.Y - fgsp.Length;

                        listOfFibreGlassSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_D_W, fgsp.X, Position_y,
                            pControlPoint_RoofLeft, fgsp.X >= dWidthOfWholeSheets ? dPartialSheet_End : claddingWidthModular_Roof_FG, fgsp.Length, fgsp.Length, 0, 0,
                            m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, options.fFibreglassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                        iSheet_FG_Index++;
                    }
                }

                listOfCladdingSheetsRoofLeft = null;
                if (bGenerateRoofCladding && bIndividualCladdingSheets)
                {
                    GenerateCladdingSheets(options.bCladdingSheetColoursByID, bUseTop20Colors, "Roof -left", pControlPoint_RoofLeft, m_ColorNameRoof,
                    m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, width,
                    claddingWidthRibModular_Roof, claddingWidthModular_Roof, iNumberOfSheets, dPartialSheet_End, length_left_basic, length_left_basic,
                    SheetListToOpeningListConverter(listOfFibreGlassSheetsRoofLeft), ref iSheetIndex, out listOfCladdingSheetsRoofLeft);

                    // TODO - upravit plechy pre canopies
                    if (canopyCollection != null)
                    {
                        for (int i = 0; i < listOfCladdingSheetsRoofLeft.Count; i++)
                        {
                            CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofLeft[i];

                            // Musime menit len tie sheets ktore maju koniec na hrane strechy
                            if (MATH.MathF.d_equal(originalsheet.CoordinateInPlane_y + originalsheet.LengthTotal, length_left_basic, 0.002))
                            {
                                foreach (CCanopiesInfo canopy in canopyCollection)
                                {
                                    bool hasNextCanopyLeft = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex + 1));
                                    bool hasPreviousCanopyLeft = ModelHelper.IsNeighboringLeftCanopy(canopyCollection.ElementAtOrDefault(canopy.BayIndex - 1));

                                    float fCanopyBayStartOffsetLeft = hasPreviousCanopyLeft ? 0f : ((canopy.BayIndex == 0 ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                                    float fCanopyBayEndOffsetLeft = hasNextCanopyLeft ? 0f : (((canopy.BayIndex == canopyCollection.Count - 1) ? (float)roofEdgeOverhang_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                                    float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, bayWidthCollection) - fCanopyBayStartOffsetLeft;
                                    float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, bayWidthCollection) + fCanopyBayEndOffsetLeft;

                                    // Zistime ci je plocha originalsheet v kolizii s nejakym canopy - right
                                    // Myslim ze mame niekde uz funkcie ktore vedia skontrolovat ci sa dve plochy prekryvaju

                                    // Zistime ci je canopy v kolizii s plechom
                                    // Ak ano upravime koncove lokalne suradnice plechu y na suradnice canopy a nastavime nove dlzky plechu
                                    if (canopy.Left && (
                                        (fBayStartCoordinate_Y_Left + fCanopyBayStartOffsetLeft <= originalsheet.CoordinateInPlane_x &&
                                        fBayEndCoordinate_Y_Left + fCanopyBayStartOffsetLeft >= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                                        (fBayStartCoordinate_Y_Left + fCanopyBayStartOffsetLeft >= originalsheet.CoordinateInPlane_x &&
                                        fBayStartCoordinate_Y_Left + fCanopyBayStartOffsetLeft <= (originalsheet.CoordinateInPlane_x + originalsheet.Width)) ||
                                        (fBayEndCoordinate_Y_Left + fCanopyBayStartOffsetLeft >= originalsheet.CoordinateInPlane_x &&
                                        fBayEndCoordinate_Y_Left + fCanopyBayStartOffsetLeft <= (originalsheet.CoordinateInPlane_x + originalsheet.Width))))
                                    {
                                        double CanopyCladdingWidth_Left = canopy.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X;

                                        //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth_Left;
                                        originalsheet.LengthTopLeft += CanopyCladdingWidth_Left;
                                        originalsheet.LengthTopRight += CanopyCladdingWidth_Left;
                                        //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                        originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Vytvorime geometry model

            // TODO - osetrit pripady ked moze byt list null

            double outOffPlaneOffset_FG = -0.010; // Pokial kreslime cladding ako jednoliatu plochu na celu stenu alebo strechu, nastavime offset, aby sa fibreglasa nevnarali do cladding
            if (bIndividualCladdingSheets)
            {
                // Ak kreslime individualne sheets pre cladding nepotrebujeme offset
                outOffPlaneOffset_FG = 0.000;
                //To Mato - toto je nutne? To sa neda oddelit do 2 roznych metod? Ak to spravne chapem,tak 1.(to hore) sa vytvara stale aj ked to netreba
                //To Ondrej - asi sa to da oddelit, len neviem kolko je tam zavislosti. Dost vela veci bude treba vytiahnut este pred tie metody
                //Najprv som urobil tu prvu cast a potom chceli samostatne sheets tak som pridal dalsiu, ako som Ti už skôr pisal, trebalo by to poriadne refaktorovať
                model_gr = new Model3DGroup(); // Vyprazdnime model group s povodnym cladding 
                WireFramePoints = new List<Point3D>(); //toto asi musim spravit,lebo inak by to bolo dvojmo aj pre cene aj pre osobitne sheets
            }

            bool claddingWireframe = options.bDisplayWireFrameModel && options.bDisplayCladdingWireFrame;
            bool fibreglassWireframe = options.bDisplayWireFrameModel && options.bDisplayCladdingWireFrame && options.bDisplayFibreglassWireFrame;
            if (bGenerateLeftSideCladding && options.bDisplayCladdingLeftWall && bIndividualCladdingSheets)
               AddSheet3DModelsToModelGroup(listOfCladdingSheetsLeftWall, options, brushSide, material_SideWall, claddingWidthRibModular_Wall, 0, 0, -90, ref model_gr, claddingWireframe);
 
            if (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame)
               AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallLeft, options, brushWall_FG, material_Wall_FG, claddingWidthRibModular_Wall_FG, 0, 0, -90, ref model_gr, fibreglassWireframe, outOffPlaneOffset_FG);

            if (bGenerateFrontSideCladding && options.bDisplayCladdingFrontWall && bIndividualCladdingSheets)
               AddSheet3DModelsToModelGroup(listOfCladdingSheetsFrontWall, options, brushFront, material_FrontBackWall, claddingWidthRibModular_Wall, 0, 0, 0, ref model_gr, claddingWireframe);

            if (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame)
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallFront, options, brushWall_FG, material_Wall_FG, claddingWidthRibModular_Wall_FG, 0, 0, 0, ref model_gr, fibreglassWireframe, outOffPlaneOffset_FG);

            if (bGenerateRightSideCladding && options.bDisplayCladdingRightWall && bIndividualCladdingSheets)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsRightWall, options, brushSide, material_SideWall, claddingWidthRibModular_Wall, 0, 0, 90, ref model_gr, claddingWireframe);

            if (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame)
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallRight, options, brushWall_FG, material_Wall_FG, claddingWidthRibModular_Wall_FG, 0, 0, 90, ref model_gr, fibreglassWireframe, outOffPlaneOffset_FG);

            if (bGenerateBackSideCladding && options.bDisplayCladdingBackWall && bIndividualCladdingSheets)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsBackWall, options, brushFront, material_FrontBackWall, claddingWidthRibModular_Wall, 0, 0, 180, ref model_gr, claddingWireframe);

            if (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame)
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsWallBack, options, brushWall_FG, material_Wall_FG, claddingWidthRibModular_Wall_FG, 0, 0, 180, ref model_gr, fibreglassWireframe, outOffPlaneOffset_FG);
 
            // Set rotation about GCS X-axis - Roof - Right Side (Gable Roof) and Monopitch Roof
            double rotationAboutX = -90f + (eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? sBuildingGeomInputData.fRoofPitch_deg : -sBuildingGeomInputData.fRoofPitch_deg);

            if (bGenerateRoofCladding && options.bDisplayCladdingRoof && bIndividualCladdingSheets)
                AddSheet3DModelsToModelGroup(listOfCladdingSheetsRoofRight, options, brushRoof, material_Roof, claddingWidthRibModular_Roof, rotationAboutX, 0, 90, ref model_gr, claddingWireframe);

            if (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame)
                AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsRoofRight, options, brushRoof_FG, material_Roof_FG, claddingWidthRibModular_Roof_FG, rotationAboutX, 0, 90, ref model_gr, fibreglassWireframe, outOffPlaneOffset_FG);

            if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Set rotation about GCS X-axis - Roof - Left Side (Gable Roof)
                rotationAboutX = -90f - sBuildingGeomInputData.fRoofPitch_deg;

                if (bGenerateRoofCladding && options.bDisplayCladdingRoof && bIndividualCladdingSheets)
                    AddSheet3DModelsToModelGroup(listOfCladdingSheetsRoofLeft, options, brushRoof, material_Roof, claddingWidthRibModular_Roof, rotationAboutX, 0, 90, ref model_gr, claddingWireframe);

                if (options.bDisplayFibreglass || options.bDisplayFibreglassWireFrame)
                    AddSheet3DModelsToModelGroup(listOfFibreGlassSheetsRoofLeft, options, brushRoof_FG, material_Roof_FG, claddingWidthRibModular_Roof_FG, rotationAboutX, 0, 90, ref model_gr, fibreglassWireframe, outOffPlaneOffset_FG);
            }
            return model_gr;
        }

        public void GenerateCladdingSheets(bool bCladdingSheetColoursByID,
            bool bUseTop20Colors,
            string side,
            Point3D pControlPoint,
            string colorName,
            string claddingShape,
            string claddingCoatingType,
            Color color,
            float fOpacity,
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

            for (int i = 0; i < iNumberOfOriginalSheetsOnSide; i++)
            {
                if (bCladdingSheetColoursByID)
                    color = ColorsHelper.GetColorWithIndex(i, bUseTop20Colors);

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

                if(listOfOpenings != null && listOfOpenings.Count > 0) // Nejake opening su zadefinovane, takze ma zmysel hladat kolizie
                   objectInColision_In_Local_x = listOfOpenings.Where(o => (o.CoordinateInPlane_x <= originalsheetCoordinateInPlane_x + dLimit && (o.CoordinateInPlane_x + o.Width) >= (originalsheetCoordinateInPlane_x - dLimit + originalsheetWidth))).ToList();

                // Ak neexistuju objekty v kolizii s originalsheet mozeme opustit funkciu
                if (objectInColision_In_Local_x == null || objectInColision_In_Local_x.Count == 0)
                {
                    // Nie je potrebne delit sheet - pridame teda "originalsheet"
                    listOfSheets.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, originalsheetNumberOfEdges, originalsheetCoordinateInPlane_x, originalsheetCoordinateInPlane_y,
                    originalsheetControlPoint, originalsheetWidth, originalsheetLengthTopLeft, originalsheetLengthTopRight, originalsheetTipCoordinate_x, originalsheetLengthTopTip,
                    colorName, claddingShape, claddingCoatingType, color, fOpacity, claddingWidthRibModular, true, 0));
                    iSheetIndex++;
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

                    // Skontrolovat podla suradnic ci objekt zacina alebo konci priamo na hrane a podla toho upravit pocet novych, ktore treba vytvorit
                    foreach(COpening o in objectInColision_In_Local_x)
                    {
                        if (o.CoordinateInPlane_y >= (originalsheetCoordinateInPlane_y + originalsheetLengthTotal) ||
                            ((o.CoordinateInPlane_y + o.LengthTotal) >= (originalsheetCoordinateInPlane_y + originalsheetLengthTotal)))
                            iNumberOfNewSheets--;
                    }

                    // 5. Pridame nove sheets do zoznamu
                    for (int j = 0; j < iNumberOfNewSheets; j++)
                    {
                        if (j == iNumberOfNewSheets - 1) // Last segment of original sheet
                        {
                            listOfSheets.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, originalsheetNumberOfEdges,
                            originalsheetCoordinateInPlane_x,
                            objectInColision_In_Local_x[j - 1].CoordinateInPlane_y + objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheetControlPoint, originalsheetWidth,
                            originalsheetLengthTopLeft - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheetLengthTopRight - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheetTipCoordinate_x,
                            originalsheetLengthTopTip - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - objectInColision_In_Local_x[j - 1].LengthTotal,
                            colorName, claddingShape, claddingCoatingType,
                            color, fOpacity, claddingWidthRibModular, true, 0));
                            iSheetIndex++;
                        }
                        else
                        {
                            double coordinate_y = 0; // Zacat od okraja  !!! - je potrebne zmenit pre doors a zacat nad dverami

                            if (j > 0)
                                coordinate_y = objectInColision_In_Local_x[j - 1].CoordinateInPlane_y + objectInColision_In_Local_x[j - 1].LengthTotal;

                            iNumberOfEdges = 4;
                            listOfSheets.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, iNumberOfEdges,
                            originalsheetCoordinateInPlane_x,
                            coordinate_y,
                            originalsheetControlPoint, originalsheetWidth,
                            objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinate_y,
                            objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinate_y,
                            0,
                            0,
                            colorName, claddingShape, claddingCoatingType,
                            color, fOpacity, claddingWidthRibModular, true, 0));
                            iSheetIndex++;
                        }
                    }
                }
            }
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
            foreach (DoorProperties door in doorPropCollection)
            {
                if (door.sBuildingSide == side)
                {
                    // TODO - vypocitat presnu poziciu otvoru dveri od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)

                    double doorPosition_x_Input_GUI;

                if(side == "Left" || side == "Right")
                    doorPosition_x_Input_GUI = -column_crsc_y_minus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * bayWidthCollection[door.iBayNumber - 1].Width);
                else
                    doorPosition_x_Input_GUI = column_crsc_z_plus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * (door.sBuildingSide == "Front" ? m_fFrontColumnDistance : m_fBackColumnDistance));

                double doorPosition_x = doorPosition_x_Input_GUI;

                if (door.sBuildingSide == "Left" || door.sBuildingSide == "Back") // Reverse x-direction in GUI
                    doorPosition_x = width - doorPosition_x_Input_GUI - door.fDoorsWidth;

                    listOfOpenings_All.Add(new COpening(iOpeningIndex + 1, iNumberOfEdges_FG_D_W, doorPosition_x, 0,
                    pControlPoint, door.fDoorsWidth, door.fDoorsHeight, door.fDoorsHeight, 0, 0, true, 0));
                    iOpeningIndex++;
                }
            }

            // Do zoznamu pridame otvory pre windows
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
                    windowPosition_x_Input_GUI = column_crsc_z_plus_temp + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * (window.sBuildingSide == "Front" ? m_fFrontColumnDistance : m_fBackColumnDistance));

                double windowPosition_x = windowPosition_x_Input_GUI;

                if (window.sBuildingSide == "Left" || window.sBuildingSide == "Back") // Reverse x-direction in GUI
                    windowPosition_x = width - windowPosition_x_Input_GUI - window.fWindowsWidth;

                    listOfOpenings_All.Add(new COpening(iOpeningIndex + 1, iNumberOfEdges_FG_D_W, windowPosition_x, -bottomEdge_z + window.fWindowCoordinateZinBay,
                    pControlPoint, window.fWindowsWidth, window.fWindowsHeight, window.fWindowsHeight, 0, 0, true, 0));
                    iOpeningIndex++;
                }
            }
        }

        // TO Ondrej - je nejaka krasia cesta ako prevadzat medzi sebou objekty potomkov spolocneho predka, ak chceme pouzit len parametre predka??
       public List<COpening> SheetListToOpeningListConverter (List<CCladdingOrFibreGlassSheet> input)
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
            bool createWireframe,
            double outOffPlaneOffset = 0)
        {
            if (listOfsheets != null || listOfsheets.Count > 0)
            {
                double wpWidth = 0, wpHeight = 0;

                for (int i = 0; i < listOfsheets.Count; i++)
                {
                    if (options.bUseTextures)
                    {
                        if (i == 0)
                        {
                            double poinstsDist = listOfsheets[i].LengthTotal;
                            wpWidth = widthRibModular / listOfsheets[i].Width;
                            wpHeight = widthRibModular / poinstsDist;
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

                    listOfsheets[i].RotationX = rotationX;
                    listOfsheets[i].RotationY = rotationY;
                    listOfsheets[i].RotationZ = rotationZ;

                    // Pridame sheet do model group
                    GeometryModel3D sheetModel = listOfsheets[i].GetCladdingSheetModel(options, material, createWireframe, outOffPlaneOffset);
                    sheetModel.Transform = listOfsheets[i].GetTransformGroup();
                    //listOfsheets[i].PointText = sheetModel.Transform.Transform(listOfsheets[i].PointText); //transformPoint
                    if (createWireframe)
                    {
                        Drawing3DHelper.TransformPoints(listOfsheets[i].WireFramePoints, sheetModel.Transform);
                        WireFramePoints.AddRange(listOfsheets[i].WireFramePoints);
                    }
                    modelGroup.Children.Add(sheetModel);
                }
            }
        }

        public void SetCladdingGenerateProperties(IList<CComponentInfo> componentList)
        {
            CComponentInfo girtL = componentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (girtL != null && girtL.Generate.HasValue)
            {
                bGenerateLeftSideCladding = girtL.Generate.Value;
            }

            CComponentInfo girtR = componentList.Last(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (girtR != null && girtR.Generate.HasValue)
            {
                bGenerateRightSideCladding = girtR.Generate.Value;
            }

            CComponentInfo girtFront = componentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide);
            if (girtFront != null && girtFront.Generate.HasValue)
            {
                bGenerateFrontSideCladding = girtFront.Generate.Value;
            }

            CComponentInfo girtBack = componentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.GirtBackSide);
            if (girtBack != null && girtBack.Generate.HasValue)
            {
                bGenerateBackSideCladding = girtBack.Generate.Value;
            }

            CComponentInfo purlin = componentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
            if (purlin != null && purlin.Generate.HasValue)
            {
                bGenerateRoofCladding = purlin.Generate.Value;
            }
        }
    }
}
