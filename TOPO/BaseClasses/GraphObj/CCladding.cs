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
        System.Collections.ObjectModel.ObservableCollection<DoorProperties> doorPropCollection;
        System.Collections.ObjectModel.ObservableCollection<WindowProperties> windowPropCollection;

        float m_fFrontColumnDistance; // Vzdialenost front columns
        float m_fBackColumnDistance; // Vzdialenost back columns

        double claddingHeight_Wall = 0.030; // z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m v tabulkach tableSections_m alebo trapezoidalSheeting_m
        double claddingHeight_Roof = 0.075; // z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m

        double claddingWidthRibModular_Wall = 0.190; // m // z databazy cladding MDBTrapezoidalSheeting widthRib_m
        double claddingWidthRibModular_Roof = 0.300; // m // z databazy cladding MDBTrapezoidalSheeting widthRib_m
        float claddingWidthModular_Wall = 0.6f; // TODO 719 - Input z databazy
        float claddingWidthModular_Roof = 0.7f; // TODO 719 - Input z databazy

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

        // Fibre glass properties - TODO - Input from GUI
        string m_ColorNameRoof_FG;
        string m_claddingShape_Roof_FG;
        string m_claddingCoatingType_Roof_FG;
        Color m_ColorRoof_FG;
        double claddingWidthRibModular_Roof_FG;
        float claddingWidthModular_Roof_FG;

        string m_ColorNameWall_FG;
        string m_claddingShape_Wall_FG;
        string m_claddingCoatingType_Wall_FG;
        Color m_ColorWall_FG;
        double claddingWidthRibModular_Wall_FG;
        float claddingWidthModular_Wall_FG;

        float fFibreGlassOpacity = 0.9f; // TODO 719 - napojit na GUI
        float fOpeningOpacity = 0.02f;
        bool bDistinguishedSheetColor = true; // TODO 719 - Option v GUI - Display Options

        public CCladding()
        {

        }

        // Constructor 2
        public CCladding(int iCladding_ID, EModelType_FS modelType_FS, BuildingGeometryDataInput sGeometryInputData,
            System.Collections.ObjectModel.ObservableCollection<CCanopiesInfo> canopies,
            System.Collections.ObjectModel.ObservableCollection<CBayInfo> bayWidths,
            System.Collections.ObjectModel.ObservableCollection<DoorProperties> doorProp,
            System.Collections.ObjectModel.ObservableCollection<WindowProperties> windowProp,
            CRSC.CCrSc_TW columnSection,
            float fFrontColumnDistance, float fBackColumnDistance,
            string colorName_Wall, string colorName_Roof, string claddingShape_Wall, string claddingCoatingType_Wall, string claddingShape_Roof, string claddingCoatingType_Roof,
            Color colorWall, Color colorRoof,
            bool bIsDisplayed, int fTime, double wallCladdingHeight, double roofCladdingHeight, double wallCladdingWidthRib, double roofCladdingWidthRib)
        {
            ID = iCladding_ID;
            eModelType = modelType_FS;
            sBuildingGeomInputData = sGeometryInputData;
            canopyCollection = canopies;
            bayWidthCollection = bayWidths;
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

            m_ColorWall = colorWall;
            m_ColorRoof = colorRoof;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            claddingHeight_Wall = wallCladdingHeight;
            claddingHeight_Roof = roofCladdingHeight;
            claddingWidthRibModular_Wall = wallCladdingWidthRib;
            claddingWidthRibModular_Roof = roofCladdingWidthRib;

            // TODO 719 - Implementovat a napojit z GUI
            m_ColorNameRoof_FG = "LightCyan"; // TODO 719 GUI INPUT
            m_claddingShape_Roof_FG = m_claddingShape_Roof;
            m_claddingCoatingType_Roof_FG = "";
            m_ColorRoof_FG = Colors.LightCyan; // TODO 719 GUI INPUT
            claddingWidthRibModular_Roof_FG = roofCladdingWidthRib;
            claddingWidthModular_Roof_FG = claddingWidthModular_Roof;

            m_ColorNameWall_FG = "LightBlue"; // TODO 719 GUI INPUT
            m_claddingShape_Wall_FG = m_claddingShape_Wall;
            m_claddingCoatingType_Wall_FG = "";
            m_ColorWall_FG = Colors.LightBlue; // TODO 719 GUI INPUT
            claddingWidthRibModular_Wall_FG = wallCladdingWidthRib;
            claddingWidthModular_Wall_FG = claddingWidthModular_Wall;
        }

        public Model3DGroup GetCladdingModel(DisplayOptions options)
        {
            m_pControlPoint = new Point3D(0, 0, 0);

            Model3DGroup model_gr = new Model3DGroup();

            // Vytvorime model v GCS [0,0,0] je uvazovana v bode m_ControlPoint

            // Consider roof cladding height for front and back wall
            bool bConsiderRoofCladdingFor_FB_WallHeight = true; // TODO 719 - napojit na GUI // Default true

            double bottomEdge_z = -0.05; // Offset pod spodnu uroven podlahy // TODO 719 - napojit na GUI, default -50 mm, limit <-500mm, 0>

            double roofEdgeOverhang_X = 0.150; // Presah okraja strechy // TODO 719 - napojit na GUI, default 150 mm, limit <0, 600mm>
            double roofEdgeOverhang_Y = 0.000; // Presah okraja strechy // TODO 719 - napojit na GUI, default 0 mm limit <0, 300mm>

            if (bConsiderRoofCladdingFor_FB_WallHeight && roofEdgeOverhang_Y > 0)
                throw new Exception("Invalid input. Roof cladding is in the collision with front/back wall cladding.");

            double additionalOffset = 0.010;  // 10 mm Aby nekolidovali plochy cladding s members
            double additionalOffsetRoof = 0.010; // Aby nekolidovali plochy cladding s members (cross-bracing) na streche

            // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
            double column_crsc_y_minus_temp = column_crsc_y_minus - additionalOffset;
            double column_crsc_y_plus_temp = column_crsc_y_plus + additionalOffset;
            double column_crsc_z_plus_temp = column_crsc_z_plus + additionalOffset;

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

            if (bConsiderRoofCladdingFor_FB_WallHeight)
            {
                height_1_final_edge_FB_Wall = height_1_final_edge_FB_Wall + claddingHeight_Roof * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                height_2_final_edge_FB_Wall = height_2_final_edge_FB_Wall + claddingHeight_Roof * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                    height_2_final_edge_FB_Wall = height_2_final + (column_crsc_z_plus_temp + claddingHeight_Roof) * Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            }

            // Wall Cladding Edges

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, bottomEdge_z);
            Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, column_crsc_y_minus_temp, bottomEdge_z);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, bottomEdge_z);
            Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp, bottomEdge_z);

            Brush solidBrushFront = new SolidColorBrush(m_ColorWall);
            Brush solidBrushSide = new SolidColorBrush(m_ColorWall);
            Brush solidBrushRoof = new SolidColorBrush(m_ColorRoof);

            solidBrushFront.Opacity = options.fFrontCladdingOpacity;
            solidBrushSide.Opacity = options.fLeftCladdingOpacity;
            solidBrushRoof.Opacity = options.fRoofCladdingOpacity;

            DiffuseMaterial material_SideWall = new DiffuseMaterial(solidBrushSide); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_FrontBackWall = new DiffuseMaterial(solidBrushFront); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_Roof = new DiffuseMaterial(solidBrushRoof); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export

            ImageBrush brushFront = null;
            ImageBrush brushSide = null;
            ImageBrush brushRoof = null;
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
                model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pFBWall_front2_heightright, pFBWall_front3_heightleft }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));
                // Back Wall
                model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pFBWall_back3_heightleft, pFBWall_back2_heightright }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                    wpHeight = claddingWidthRibModular_Wall / (pLRWall_back2_heightright.Z - pback0_baseleft.Z);
                    brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_SideWall = new DiffuseMaterial(brushSide);
                }

                // Left Wall
                model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pLRWall_front3_heightleft, pLRWall_back3_heightleft }, 0).CreateArea(options.bUseTextures, material_SideWall));
                // Right Wall
                model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pLRWall_back2_heightright, pLRWall_front2_heightright }, 0).CreateArea(options.bUseTextures, material_SideWall));

                if (options.bUseTextures)
                {
                    double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pRoof_front2_heightright, pRoof_front3_heightleft); // Rovina XZ
                    wpWidth = claddingWidthRibModular_Roof / (pRoof_back2_heightright.Y - pRoof_front2_heightright.Y);
                    wpHeight = claddingWidthRibModular_Roof / poinstsDist;
                    brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_Roof = new DiffuseMaterial(brushRoof);
                }

                // Roof
                model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pRoof_front2_heightright, pRoof_back2_heightright, pRoof_back3_heightleft, pRoof_front3_heightleft }, 0).CreateArea(options.bUseTextures, material_Roof));

                // Canopies

                //----------------------------------------------------------------------------------
                // Todo 691
                // TODO - napojit suradnice zaciatku a konca bay v smere GCS Y
                // To Ondrej - potrebujem tu nejako elegantne dosta+t a pracovat bay start a bay end coordinate v smere Y
                //Docasny kod

                int iBayIndex = 0;
                //----------------------------------------------------------------------------------
                foreach (CCanopiesInfo canopy in canopyCollection)
                {
                    int iAreaIndex = 5;

                    float fOverhangOffset_x = 0.05f; // TODO - zadavat v GUI ako cladding property pre roof
                    float fOverhangOffset_y = (float)roofEdgeOverhang_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

                    float fBayWidth = bayWidthCollection[canopy.BayIndex].Width;
                    float fBayStartCoordinate_Y = (iBayIndex * fBayWidth) - fOverhangOffset_y + (float)column_crsc_y_minus;
                    float fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + fOverhangOffset_y + (float)column_crsc_y_plus;

                    if (canopy.BayIndex == 0) // First bay
                        fBayStartCoordinate_Y = (iBayIndex * fBayWidth) + (float)column_crsc_y_minus_temp - (float)roofEdgeOverhang_Y;
                    else if (canopy.BayIndex == canopyCollection.Count - 1) // Last bay
                        fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + (float)column_crsc_y_plus_temp + (float)roofEdgeOverhang_Y;

                    //TODO - tu treba oddelit fBayStartCoordinate_Y a fBayEndCoordinate_Y pre lavu a pravu stranu
                    // Zistit ci je na lavej ,resp pravej strane canopy napojena na inu canopy vedla nej a ak ano tak je potrebne nastavit tieto hodnoty tak, aby sa canopies neprekryvali

                    iBayIndex++; // Docasne // Todo 691 - zmazat

                    float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y - (float)column_crsc_y_minus_temp + (float)roofEdgeOverhang_Y;
                    int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                    double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                    double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                    if (canopy.Left)
                    {
                        // 2 ______ 1
                        //  |      |
                        //  |      |
                        //  |______|
                        // 3        0

                        float fCanopyCladdingWidth = (float)canopy.WidthLeft + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                        float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y, height_1_final_edge_Roof);
                        Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y, height_1_final_edge_Roof);

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

                        model_gr.Children.Add(new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0).CreateArea(options.bUseTextures, material_Roof));
                        iAreaIndex++;
                    }

                    if (canopy.Right)
                    {
                        float fCanopyCladdingWidth = (float)canopy.WidthRight + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                        float fCanopy_EdgeCoordinate_z = (float)height_2_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y, height_2_final_edge_Roof);
                        Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y, height_2_final_edge_Roof);
                        Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);

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

                        model_gr.Children.Add(new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0).CreateArea(options.bUseTextures, material_Roof));
                        iAreaIndex++;
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
                model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pFBWall_front2_heightright, pFBWall_front4_top, pFBWall_front3_heightleft }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));
                // Back Wall
                model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pFBWall_back3_heightleft, pFBWall_back4_top, pFBWall_back2_heightright }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                    wpHeight = claddingWidthRibModular_Wall / (pLRWall_back3_heightleft.Z - pback0_baseleft.Z);
                    brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_SideWall = new DiffuseMaterial(brushSide);
                }

                // Left Wall
                model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pLRWall_front3_heightleft, pLRWall_back3_heightleft }, 0).CreateArea(options.bUseTextures, material_SideWall));
                // Right Wall
                model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pLRWall_back2_heightright, pLRWall_front2_heightright }, 0).CreateArea(options.bUseTextures, material_SideWall));

                if (options.bUseTextures)
                {
                    double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pRoof_front4_top, pRoof_front3_heightleft); // Rovina XZ
                    wpWidth = claddingWidthRibModular_Roof / (pRoof_back4_top.Y - pRoof_front4_top.Y);
                    wpHeight = claddingWidthRibModular_Roof / poinstsDist;
                    brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_Roof = new DiffuseMaterial(brushRoof);
                }

                // Roof - Left Side
                model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pRoof_front4_top, pRoof_back4_top, pRoof_back3_heightleft, pRoof_front3_heightleft }, 0).CreateArea(options.bUseTextures, material_Roof));
                // Roof - Right Side
                model_gr.Children.Add(new CAreaPolygonal(5, new List<Point3D>() { pRoof_front2_heightright, pRoof_back2_heightright, pRoof_back4_top, pRoof_front4_top }, 0).CreateArea(options.bUseTextures, material_Roof));

                // Canopies

                //----------------------------------------------------------------------------------
                // Todo 691
                // TODO - napojit suradnice zaciatku a konca bay v smere GCS Y
                // To Ondrej - potrebujem tu nejako elegantne dosta+t a pracovat bay start a bay end coordinate v smere Y
                //Docasny kod
                int iBayIndex = 0;
                //----------------------------------------------------------------------------------

                foreach (CCanopiesInfo canopy in canopyCollection)
                {
                    int iAreaIndex = 6;

                    float fOverhangOffset_x = 0.05f; // TODO - zadavat v GUI ako cladding property pre roof
                    float fOverhangOffset_y = (float)roofEdgeOverhang_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

                    float fBayWidth = bayWidthCollection[canopy.BayIndex].Width;
                    float fBayStartCoordinate_Y = (iBayIndex * fBayWidth) - fOverhangOffset_y + (float)column_crsc_y_minus;
                    float fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + fOverhangOffset_y + (float)column_crsc_y_plus;

                    if (canopy.BayIndex == 0) // First bay
                        fBayStartCoordinate_Y = (iBayIndex * fBayWidth) + (float)column_crsc_y_minus_temp - (float)roofEdgeOverhang_Y;
                    else if (canopy.BayIndex == canopyCollection.Count - 1) // Last bay
                        fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + (float)column_crsc_y_plus_temp + (float)roofEdgeOverhang_Y;

                    //TODO - tu treba oddelit fBayStartCoordinate_Y a fBayEndCoordinate_Y pre lavu a pravu stranu
                    // Zistit ci je na lavej ,resp pravej strane canopy napojena na inu canopy vedla nej a ak ano tak je potrebne nastavit tieto hodnoty tak, aby sa canopies neprekryvali

                    iBayIndex++; // Docasne // Todo 691 - zmazat

                    float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y - (float)column_crsc_y_minus_temp + (float)roofEdgeOverhang_Y;
                    int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                    double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                    double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                    if (canopy.Left)
                    {
                        float fCanopyCladdingWidth = (float)canopy.WidthLeft + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                        float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X - fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y, height_1_final_edge_Roof);
                        Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y, height_1_final_edge_Roof);

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

                        model_gr.Children.Add(new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0).CreateArea(options.bUseTextures, material_Roof));
                        iAreaIndex++;
                    }

                    if (canopy.Right)
                    {
                        float fCanopyCladdingWidth = (float)canopy.WidthRight + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;
                        float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y, height_1_final_edge_Roof);
                        Point3D pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y, height_1_final_edge_Roof);
                        Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + (float)column_crsc_z_plus_temp + (float)roofEdgeOverhang_X + fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);

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

                        model_gr.Children.Add(new CAreaPolygonal(iAreaIndex, new List<Point3D>() { pfront_right, pback_right, pback_left, pfront_left }, 0).CreateArea(options.bUseTextures, material_Roof));
                        iAreaIndex++;
                    }
                }
            }
            else
            {
                throw new Exception("Not implemented kitset type.");
            }






            bool bIndividualCladdingSheets = true; // TODO 719 - Option - Model Options
            // Ak to bude false, zostane vacsina veci ako doposial
            // Zobrazime len jednoliatu plochu s farbou alebo texturou, nad nou mozeme zobrazit fibreglass sheet (to treba dorobit aby sa dalo zavolat samostatne)
            // Bude to podobne ako door a window, takze sa nebudu kreslit realne otvory len sa nad plochu strechy dokresli fibreglass sheet
            // Nebudeme generovat cladding sheet material list ani cladding sheet layout pattern
            // Len spocitame plochu otvorov a odratame ju z celkovej plochy cladding a to bude v Quotation

            if (bIndividualCladdingSheets)
            {
                //------------------------------------------------------------------------------------------------------
                //------------------------------------------------------------------------------------------------------
                //------------------------------------------------------------------------------------------------------

                // Gewnerujeme jednotlive plechy, tie rozdelime podla toho ako koliduju s otvormi
                // V mieste otvorov pre fibreglass, door, windows sa nebudú generovat cladding sheets, ktore su otvorom rozdelene po celej sirke
                // Cladding sheets, ktore sa s otvorom prekryvaju len ciastocne vykreslime v 3D cele

                // IN WORK
                // Particular Cladding Sheet Model

                // Prva uroven, stany budovy alebo strechy, left, right, front, back, roof left roof right
                // Druha uroven jednotlive sheet nachadzajuce sa v jednej rovine
                //List<List<CCladdingOrFibreGlassSheet>> listOfCladdingSheets = new List<List<CCladdingOrFibreGlassSheet>>();

                // TODO - color list ma 141 poloziek, ale na jednej strane moze byt az 500 sheets, preto nakopirujem polozky viac krat
                // TO Ondrej - asi sa to da nejako krajsie programovo urobit aby sa pri vycerpani listu zacalo znova od zaciatku
                // TODO Ondrej - chcel by som ten list upravit tak ze vybrieme napriklad 20 peknych farieb a tie sa budu striedat, aby tam nebola cierna alebo 3 odtiene bielej za sebou
                // Povodny list so vsetkymi farbami mi nezmaz

                List<Color> colorsTemp_1410_items = new List<Color>();
                for (int k = 0; k < 10; k++)
                {
                    for (int l = 0; l < ColorList.Count; l++)
                    {
                        colorsTemp_1410_items.Add(ColorList[l]);
                    }
                }

                // Nahradime povodne farby 141 rozsirenym zoznamom 1410
                ColorList = colorsTemp_1410_items;


                //--------------------------------------------------------------------------------------------------------------------------------
                // Left Wall
                // Docasne - napojit vytvorenie FG Sheets na GUI
                // Fibreglass - docasne - malo byt zadavane v datagride v Tabe Cladding

                int iSheet_FG_Index = 0;
                // FG Sheet 1
                float fPosition_x = 2 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                float fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                float fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet
                List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallLeft = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassSheetsWallLeft.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pback0_baseleft.X, pback0_baseleft.Y, pback0_baseleft.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;
                //--------------------------------------------------------------------------------------------------------------------------------

                // Left Wall
                // Total Wall Width
                double width = pback0_baseleft.Y - pfront0_baseleft.Y;
                double height_left_basic = -bottomEdge_z + height_1_final_edge_LR_Wall;

                int iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
                double dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
                double dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                int iNumberOfSheets = iNumberOfWholeSheets + 1;

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsLeftWall = new List<CCladdingOrFibreGlassSheet>();

                int iSheetIndex = 0;

                // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                for (int i = 0; i < iNumberOfSheets; i++)
                {
                    if (bDistinguishedSheetColor)
                        m_ColorWall = ColorList[i];

                    listOfCladdingSheetsLeftWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4, i * claddingWidthModular_Wall, 0,
                    new Point3D(pback0_baseleft.X, pback0_baseleft.Y, pback0_baseleft.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left_basic, height_left_basic, 0.5 * (i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall), height_left_basic,
                    m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                    iSheetIndex++;
                }

                // Vytvorime zoznam otvorov na left wall
                // Moze sa refaktorovat, ale pozor na controlPoint_GCS, je iny pre kazdu stranu
                List<CCladdingOrFibreGlassSheet> listOfOpeningsLeftWall = new List<CCladdingOrFibreGlassSheet>();
                foreach (DoorProperties door in doorPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru dveri od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fdoorPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (float)claddingHeight_Wall + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * bayWidthCollection[door.iBayNumber - 1].Width);
                    float fdoorPosition_x = fdoorPosition_x_Input_GUI;

                    if (door.sBuildingSide == "Left" || door.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fdoorPosition_x = (float)width - fdoorPosition_x_Input_GUI - door.fDoorsWidth;

                    if (door.sBuildingSide == "Left")
                    {
                        listOfOpeningsLeftWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fdoorPosition_x, 0,
                        new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), door.fDoorsWidth, door.fDoorsHeight, door.fDoorsHeight, 0, 0,
                        m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                foreach (WindowProperties window in windowPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru okna od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fwindowPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (float)claddingHeight_Wall + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * bayWidthCollection[window.iBayNumber - 1].Width);
                    float fwindowPosition_x = fwindowPosition_x_Input_GUI;

                    if (window.sBuildingSide == "Left" || window.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fwindowPosition_x = (float)width - fwindowPosition_x_Input_GUI - window.fWindowsWidth;

                    if (window.sBuildingSide == "Left")
                    {
                        listOfOpeningsLeftWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fwindowPosition_x, window.fWindowCoordinateZinBay,
                        new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), window.fWindowsWidth, window.fWindowsHeight, window.fWindowsHeight, 0, 0,
                        m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                // Modifikujeme sheets
                // Odstranime plechy cladding, ktore su v kolizii s otvormi (FibreGlass, Doors, Windows)
                // a vytvorime novu sadu plechov ktora zmazany plech nahradi

                // TO Ondrej - vyrábam uplne nový zoznam a don pridavam povodny sheet alebo ho delim a vytvaram nove
                // Mozno by sa to dalo urobit uz na prvykrat aby som nerobil zoznam s sheets bez otvorov a potom s otvormi
                // Uvazujem ci by sa nedal vytvorit originalSheet len "naoko" vo vnutri SplitSheets a tym padom by cele naplnenie povodneho zoznamu odpadlo

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsLeftWallNew = null;

                // Kedze mame otvory pre fibreglass sheets a otvory pre doors/windows v dvoch samostatnych zoznamoch, zlucime ich do jedneho
                // Pouzivame pre vsetky otvory jeden typ objektu
                // TODO - v podstate by sme mohli pouzivat len jeden zoznam a vsetko doň priamo vkladať, ale kedže chceme pre fibreglass robiť material list zatiaľ to nechame oddelene.
                List<CCladdingOrFibreGlassSheet> listOfOpeningsLeftWall_All = listOfFibreGlassSheetsWallLeft.Concat(listOfOpeningsLeftWall).ToList();

                SplitSheets(listOfCladdingSheetsLeftWall, listOfOpeningsLeftWall_All,
                    ref iSheetIndex, out listOfCladdingSheetsLeftWallNew);

                listOfCladdingSheetsLeftWall = listOfCladdingSheetsLeftWallNew; // Nastavime novy zoznam

                //--------------------------------------------------------------------------------------------------------------------------------
                // Front Wall
                // Docasne - napojit vytvorenie FG Sheets na GUI
                // Fibreglass - docasne - malo byt zadavane v datagride v Tabe Cladding

                // FG Sheet 1
                fPosition_x = 2 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet
                List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallFront = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassSheetsWallFront.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 2
                fPosition_x = 4 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet
                listOfFibreGlassSheetsWallFront.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 3
                fPosition_x = 6 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                fPosition_y = 0.0f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                fFBSheetLength = 2.0f; // TODO Input - docasne - dlzka fibreglass sheet
                listOfFibreGlassSheetsWallFront.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 4
                fPosition_x = 6 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                fPosition_y = 2.6f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                fFBSheetLength = 0.6f; // TODO Input - docasne - dlzka fibreglass sheet
                listOfFibreGlassSheetsWallFront.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;
                //--------------------------------------------------------------------------------------------------------------------------------

                // Front Wall
                // Total Wall Width
                width = pfront1_baseright.X - pfront0_baseleft.X;
                height_left_basic = -bottomEdge_z + height_1_final_edge_FB_Wall;

                iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
                dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
                dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                iNumberOfSheets = iNumberOfWholeSheets + 1;

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsFrontWall = new List<CCladdingOrFibreGlassSheet>();

                // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                for (int i = 0; i < iNumberOfSheets; i++)
                {
                    if (bDistinguishedSheetColor)
                        m_ColorWall = ColorList[i];

                    double height_left = GetVerticalCoordinate("Front", eModelType, width, height_left_basic, i * claddingWidthModular_Wall);
                    double height_right = GetVerticalCoordinate("Front", eModelType, width, height_left_basic, (i + 1) * claddingWidthModular_Wall);
                    double height_toptip = 0.5 * (height_left + height_right);
                    double tipCoordinate_x = 0.5 * claddingWidthModular_Wall;

                    if (i == iNumberOfSheets - 1)
                    {
                        height_right = GetVerticalCoordinate("Front", eModelType, width, height_left_basic, (float)width);
                        height_toptip = 0.5 * (height_left + height_right);
                        tipCoordinate_x = 0.5 * dPartialSheet_End;
                    }

                    int iNumberOfEdges = 4;

                    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed &&
                       i * claddingWidthModular_Wall < 0.5 * width &&
                       (i + 1) * claddingWidthModular_Wall > 0.5 * width)
                    {
                        iNumberOfEdges = 5;
                        height_toptip = -bottomEdge_z + height_2_final_edge_FB_Wall;
                        tipCoordinate_x = 0.5 * width - i * claddingWidthModular_Wall;
                    }

                    listOfCladdingSheetsFrontWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, iNumberOfEdges, i * claddingWidthModular_Wall, 0,
                    new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left, height_right, tipCoordinate_x, height_toptip,
                    m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                    iSheetIndex++;
                }

                // Vytvorime zoznam otvorov na front wall
                // Moze sa refaktorovat, ale pozor na controlPoint_GCS, je iny pre kazdu stranu
                List<CCladdingOrFibreGlassSheet> listOfOpeningsFrontWall = new List<CCladdingOrFibreGlassSheet>();
                foreach (DoorProperties door in doorPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru dveri od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fdoorPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * m_fFrontColumnDistance);
                    float fdoorPosition_x = fdoorPosition_x_Input_GUI;

                    if (door.sBuildingSide == "Left" || door.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fdoorPosition_x = (float)width - fdoorPosition_x_Input_GUI - door.fDoorsWidth;

                    if (door.sBuildingSide == "Front")
                    {
                        listOfOpeningsFrontWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fdoorPosition_x, 0,
                            new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), door.fDoorsWidth, door.fDoorsHeight, door.fDoorsHeight, 0, 0,
                            m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                foreach (WindowProperties window in windowPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru okna od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fwindowPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * m_fFrontColumnDistance);
                    float fwindowPosition_x = fwindowPosition_x_Input_GUI;

                    if (window.sBuildingSide == "Left" || window.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fwindowPosition_x = (float)width - fwindowPosition_x_Input_GUI - window.fWindowsWidth;

                    if (window.sBuildingSide == "Front")
                    {
                        listOfOpeningsFrontWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fwindowPosition_x, window.fWindowCoordinateZinBay,
                        new Point3D(pfront0_baseleft.X, pfront0_baseleft.Y, pfront0_baseleft.Z), window.fWindowsWidth, window.fWindowsHeight, window.fWindowsHeight, 0, 0,
                        m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                // Modifikujeme sheets
                // Odstranime plechy cladding, ktore su v kolizii s otvormi (FibreGlass, Doors, Windows)
                // a vytvorime novu sadu plechov ktora zmazany plech nahradi

                // TO Ondrej - vyrábam uplne nový zoznam a don pridavam povodny sheet alebo ho delim a vytvaram nove
                // Mozno by sa to dalo urobit uz na prvykrat aby som nerobil zoznam s sheets bez otvorov a potom s otvormi
                // Uvazujem ci by sa nedal vytvorit originalSheet len "naoko" vo vnutri SplitSheets a tym padom by cele naplnenie povodneho zoznamu odpadlo

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsFrontWallNew = null;

                // Kedze mame otvory pre fibreglass sheets a otvory pre doors/windows v dvoch samostatnych zoznamoch, zlucime ich do jedneho
                // Pouzivame pre vsetky otvory jeden typ objektu
                // TODO - v podstate by sme mohli pouzivat len jeden zoznam a vsetko doň priamo vkladať, ale kedže chceme pre fibreglass robiť material list zatiaľ to nechame oddelene.
                List<CCladdingOrFibreGlassSheet> listOfOpeningsFrontWall_All = listOfFibreGlassSheetsWallFront.Concat(listOfOpeningsFrontWall).ToList();

                SplitSheets(listOfCladdingSheetsFrontWall, listOfOpeningsFrontWall_All,
                    ref iSheetIndex, out listOfCladdingSheetsFrontWallNew);

                listOfCladdingSheetsFrontWall = listOfCladdingSheetsFrontWallNew; // Nastavime novy zoznam

                //--------------------------------------------------------------------------------------------------------------------------------
                // Right Wall
                // Docasne - napojit vytvorenie FG Sheets na GUI
                // Fibreglass - docasne - malo byt zadavane v datagride v Tabe Cladding

                // FG Sheet 1
                fPosition_x = 3 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                fPosition_y = 0.2f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                fFBSheetLength = 2.0f; // TODO Input - docasne - dlzka fibreglass sheet
                List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallRight = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassSheetsWallRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;
                //--------------------------------------------------------------------------------------------------------------------------------

                // Right Wall
                // Total Wall Width
                width = pback1_baseright.Y - pfront1_baseright.Y;
                height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_LR_Wall : -bottomEdge_z + height_2_final_edge_LR_Wall;

                iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
                dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
                dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                iNumberOfSheets = iNumberOfWholeSheets + 1;

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRightWall = new List<CCladdingOrFibreGlassSheet>();

                iSheetIndex = 0;

                // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                for (int i = 0; i < iNumberOfSheets; i++)
                {
                    if (bDistinguishedSheetColor)
                        m_ColorWall = ColorList[i];

                    listOfCladdingSheetsRightWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4, i * claddingWidthModular_Wall, 0,
                    new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left_basic, height_left_basic, 0.5 * (i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall), height_left_basic,
                    m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                    iSheetIndex++;
                }

                // Vytvorime zoznam otvorov na right wall
                // Moze sa refaktorovat, ale pozor na controlPoint_GCS, je iny pre kazdu stranu
                List<CCladdingOrFibreGlassSheet> listOfOpeningsRightWall = new List<CCladdingOrFibreGlassSheet>();
                foreach (DoorProperties door in doorPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru dveri od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fdoorPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * bayWidthCollection[door.iBayNumber - 1].Width);
                    float fdoorPosition_x = fdoorPosition_x_Input_GUI;

                    if (door.sBuildingSide == "Left" || door.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fdoorPosition_x = (float)width - fdoorPosition_x_Input_GUI - door.fDoorsWidth;

                    if (door.sBuildingSide == "Right")
                    {
                        listOfOpeningsRightWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fdoorPosition_x, 0,
                            new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z), door.fDoorsWidth, door.fDoorsHeight, door.fDoorsHeight, 0, 0,
                            m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                foreach (WindowProperties window in windowPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru okna od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fwindowPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * bayWidthCollection[window.iBayNumber - 1].Width);
                    float fwindowPosition_x = fwindowPosition_x_Input_GUI;

                    if (window.sBuildingSide == "Left" || window.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fwindowPosition_x = (float)width - fwindowPosition_x_Input_GUI - window.fWindowsWidth;

                    if (window.sBuildingSide == "Right")
                    {
                        listOfOpeningsRightWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fwindowPosition_x, window.fWindowCoordinateZinBay,
                        new Point3D(pfront1_baseright.X, pfront1_baseright.Y, pfront1_baseright.Z), window.fWindowsWidth, window.fWindowsHeight, window.fWindowsHeight, 0, 0,
                        m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                // Modifikujeme sheets
                // Odstranime plechy cladding, ktore su v kolizii s otvormi (FibreGlass, Doors, Windows)
                // a vytvorime novu sadu plechov ktora zmazany plech nahradi

                // TO Ondrej - vyrábam uplne nový zoznam a don pridavam povodny sheet alebo ho delim a vytvaram nove
                // Mozno by sa to dalo urobit uz na prvykrat aby som nerobil zoznam s sheets bez otvorov a potom s otvormi
                // Uvazujem ci by sa nedal vytvorit originalSheet len "naoko" vo vnutri SplitSheets a tym padom by cele naplnenie povodneho zoznamu odpadlo

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRightWallNew = null;

                // Kedze mame otvory pre fibreglass sheets a otvory pre doors/windows v dvoch samostatnych zoznamoch, zlucime ich do jedneho
                // Pouzivame pre vsetky otvory jeden typ objektu
                // TODO - v podstate by sme mohli pouzivat len jeden zoznam a vsetko doň priamo vkladať, ale kedže chceme pre fibreglass robiť material list zatiaľ to nechame oddelene.
                List<CCladdingOrFibreGlassSheet> listOfOpeningsRightWall_All = listOfFibreGlassSheetsWallRight.Concat(listOfOpeningsRightWall).ToList();

                SplitSheets(listOfCladdingSheetsRightWall, listOfOpeningsRightWall_All,
                    ref iSheetIndex, out listOfCladdingSheetsRightWallNew);

                listOfCladdingSheetsRightWall = listOfCladdingSheetsRightWallNew; // Nastavime novy zoznam

                //--------------------------------------------------------------------------------------------------------------------------------
                // Back Wall
                // Docasne - napojit vytvorenie FG Sheets na GUI
                // Fibreglass - docasne - malo byt zadavane v datagride v Tabe Cladding

                // FG Sheet 1
                fPosition_x = 2 * claddingWidthModular_Wall_FG; // TODO Input - docasne
                fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany laveho okraja steny
                fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet
                List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsWallBack = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassSheetsWallBack.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, 4 /*iNumberOfEdges*/, fPosition_x, fPosition_y,
                    new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z), claddingWidthModular_Wall_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameWall_FG, m_claddingShape_Wall_FG, m_claddingCoatingType_Wall_FG, m_ColorWall_FG, fFibreGlassOpacity, claddingWidthRibModular_Wall_FG, true, 0));
                iSheet_FG_Index++;
                //--------------------------------------------------------------------------------------------------------------------------------

                // Back Wall
                // Total Wall Width
                width = pback1_baseright.X - pback0_baseleft.X;
                height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? -bottomEdge_z + height_1_final_edge_FB_Wall : -bottomEdge_z + height_2_final_edge_FB_Wall;

                iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
                dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
                dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                iNumberOfSheets = iNumberOfWholeSheets + 1;

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsBackWall = new List<CCladdingOrFibreGlassSheet>();

                // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                for (int i = 0; i < iNumberOfSheets; i++)
                {
                    if (bDistinguishedSheetColor)
                        m_ColorWall = ColorList[i];

                    double height_left = GetVerticalCoordinate("Back", eModelType, width, height_left_basic, i * claddingWidthModular_Wall);
                    double height_right = GetVerticalCoordinate("Back", eModelType, width, height_left_basic, (i + 1) * claddingWidthModular_Wall);
                    double height_toptip = 0.5 * (height_left + height_right);
                    double tipCoordinate_x = 0.5 * claddingWidthModular_Wall;

                    if (i == iNumberOfSheets - 1)
                    {
                        height_right = GetVerticalCoordinate("Back", eModelType, width, height_left_basic, (float)width);
                        height_toptip = 0.5 * (height_left + height_right);
                        tipCoordinate_x = 0.5 * dPartialSheet_End;
                    }

                    int iNumberOfEdges = 4;

                    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed &&
                       i * claddingWidthModular_Wall < 0.5 * width &&
                       (i + 1) * claddingWidthModular_Wall > 0.5 * width)
                    {
                        iNumberOfEdges = 5;
                        height_toptip = -bottomEdge_z + height_2_final_edge_FB_Wall;
                        tipCoordinate_x = 0.5 * width - i * claddingWidthModular_Wall;
                    }

                    listOfCladdingSheetsBackWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, iNumberOfEdges, i * claddingWidthModular_Wall, 0,
                    new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left, height_right, tipCoordinate_x, height_toptip,
                    m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                    iSheetIndex++;
                }

                // Vytvorime zoznam otvorov na back wall
                // Moze sa refaktorovat, ale pozor na controlPoint_GCS, je iny pre kazdu stranu
                List<CCladdingOrFibreGlassSheet> listOfOpeningsBackWall = new List<CCladdingOrFibreGlassSheet>();
                foreach (DoorProperties door in doorPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru dveri od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fdoorPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (door.fDoorCoordinateXinBlock + (door.iBayNumber - 1) * m_fBackColumnDistance);
                    float fdoorPosition_x = fdoorPosition_x_Input_GUI;

                    if (door.sBuildingSide == "Left" || door.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fdoorPosition_x = (float)width - fdoorPosition_x_Input_GUI - door.fDoorsWidth;

                    if (door.sBuildingSide == "Back")
                    {
                        listOfOpeningsBackWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fdoorPosition_x, 0,
                            new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z), door.fDoorsWidth, door.fDoorsHeight, door.fDoorsHeight, 0, 0,
                            m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                foreach (WindowProperties window in windowPropCollection)
                {
                    // TODO - vypocitat presnu poziciu otvoru okna od laveho okraja steny
                    // Moze sa menit podla strany a aj podla orientacie steny (left a back !!!)
                    float fwindowPosition_x_Input_GUI = -(float)column_crsc_y_minus_temp + (window.fWindowCoordinateXinBay + (window.iBayNumber - 1) * m_fBackColumnDistance);
                    float fwindowPosition_x = fwindowPosition_x_Input_GUI;

                    if (window.sBuildingSide == "Left" || window.sBuildingSide == "Back") // Reverse x-direction in GUI
                        fwindowPosition_x = (float)width - fwindowPosition_x_Input_GUI - window.fWindowsWidth;

                    if (window.sBuildingSide == "Back")
                    {
                        listOfOpeningsBackWall.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, 4 /*iNumberOfEdges*/, fwindowPosition_x, window.fWindowCoordinateZinBay,
                        new Point3D(pback1_baseright.X, pback1_baseright.Y, pback1_baseright.Z), window.fWindowsWidth, window.fWindowsHeight, window.fWindowsHeight, 0, 0,
                        m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, fOpeningOpacity, claddingWidthRibModular_Wall, true, 0));
                        iSheetIndex++;
                    }
                }

                // Modifikujeme sheets
                // Odstranime plechy cladding, ktore su v kolizii s otvormi (FibreGlass, Doors, Windows)
                // a vytvorime novu sadu plechov ktora zmazany plech nahradi

                // TO Ondrej - vyrábam uplne nový zoznam a don pridavam povodny sheet alebo ho delim a vytvaram nove
                // Mozno by sa to dalo urobit uz na prvykrat aby som nerobil zoznam s sheets bez otvorov a potom s otvormi
                // Uvazujem ci by sa nedal vytvorit originalSheet len "naoko" vo vnutri SplitSheets a tym padom by cele naplnenie povodneho zoznamu odpadlo

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsBackWallNew = null;

                // Kedze mame otvory pre fibreglass sheets a otvory pre doors/windows v dvoch samostatnych zoznamoch, zlucime ich do jedneho
                // Pouzivame pre vsetky otvory jeden typ objektu
                // TODO - v podstate by sme mohli pouzivat len jeden zoznam a vsetko doň priamo vkladať, ale kedže chceme pre fibreglass robiť material list zatiaľ to nechame oddelene.
                List<CCladdingOrFibreGlassSheet> listOfOpeningsBackWall_All = listOfFibreGlassSheetsWallBack.Concat(listOfOpeningsBackWall).ToList();

                SplitSheets(listOfCladdingSheetsBackWall, listOfOpeningsBackWall_All,
                    ref iSheetIndex, out listOfCladdingSheetsBackWallNew);

                listOfCladdingSheetsBackWall = listOfCladdingSheetsBackWallNew; // Nastavime novy zoznam

                // Roof
                //--------------------------------------------------------------------------------------------------------------------------------
                // Docasne - napojit vytvorenie FG Sheets na GUI
                // Fibreglass - docasne - malo byt zadavane v datagride v Tabe Cladding
                int iNumberOfEdges_FG_Roof = 4;

                // FG Sheet 1
                fPosition_x = 2 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet
                List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();
                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 2
                fPosition_x = 4 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet

                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 3
                fPosition_x = 8 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 1.0f; // TODO Input - docasne - dlzka fibreglass sheet

                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 4
                fPosition_x = 2 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 2.0f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 1.5f; // TODO Input - docasne - dlzka fibreglass sheet

                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 5
                fPosition_x = 6 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 2.1f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 1.5f; // TODO Input - docasne - dlzka fibreglass sheet

                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 6
                fPosition_x = 10 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 0.4f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 3.0f; // TODO Input - docasne - dlzka fibreglass sheet

                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;

                // FG Sheet 7
                fPosition_x = 11 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                fPosition_y = 0.4f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                fFBSheetLength = 3.0f; // TODO Input - docasne - dlzka fibreglass sheet

                listOfFibreGlassSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                    m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                iSheet_FG_Index++;
                //--------------------------------------------------------------------------------------------------------------------------------

                // Roof - Right Side
                // Total Width
                width = pRoof_back2_heightright.Y - pRoof_front2_heightright.Y;

                float length_left_basic;

                if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
                    length_left_basic = Drawing3D.GetPoint3DDistanceFloat(pRoof_front2_heightright, pRoof_front4_top);
                else
                    length_left_basic = Drawing3D.GetPoint3DDistanceFloat(pRoof_front3_heightleft, pRoof_front2_heightright);

                iNumberOfWholeSheets = (int)(width / claddingWidthModular_Roof);
                dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Roof;
                dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                iNumberOfSheets = iNumberOfWholeSheets + 1;

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRoofRight = new List<CCladdingOrFibreGlassSheet>();

                // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                for (int i = 0; i < iNumberOfSheets; i++)
                {
                    if (bDistinguishedSheetColor)
                        m_ColorRoof = ColorList[i];

                    double length = length_left_basic;

                    int iNumberOfEdges = 4;

                    listOfCladdingSheetsRoofRight.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, iNumberOfEdges, i * claddingWidthModular_Roof, 0,
                    new Point3D(pRoof_front2_heightright.X, pRoof_front2_heightright.Y, pRoof_front2_heightright.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Roof, length, length, 0, 0,
                    m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, claddingWidthRibModular_Roof, true, 0));
                    iSheetIndex++;
                }

                // Modifikujeme sheets
                // Odstranime plechy cladding, ktore su v kolizii s otvormi (FibreGlass, Doors, Windows)
                // a vytvorime novu sadu plechov ktora zmazany plech nahradi

                // TO Ondrej - vyrábam uplne nový zoznam a don pridavam povodny sheet alebo ho delim a vytvaram nove
                // Mozno by sa to dalo urobit uz na prvykrat aby som nerobil zoznam s sheets bez otvorov a potom s otvormi
                // Uvazujem ci by sa nedal vytvorit originalSheet len "naoko" vo vnutri SplitSheets a tym padom by cele naplnenie povodneho zoznamu odpadlo

                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRoofRightNew = null;

                SplitSheets(listOfCladdingSheetsRoofRight, listOfFibreGlassSheetsRoofRight,
                    ref iSheetIndex, out listOfCladdingSheetsRoofRightNew);

                listOfCladdingSheetsRoofRight = listOfCladdingSheetsRoofRightNew; // Nastavime novy zoznam

                // TODO - upravit plechy pre canopies
                if(canopyCollection != null)
                {
                    for (int i = 0; i < listOfCladdingSheetsRoofRight.Count; i++)
                    {
                        CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofRight[i];

                        // Zistime ake su suradnice canopy start a end pre smer Y !!! Je to nakopirovane z predchadzajuceho kodu,
                        // Najlepsie by asi bolo keby boli suradnice priamo property v CCanopiesInfo

                        int iBayIndex = 0;
                        //----------------------------------------------------------------------------------
                        foreach (CCanopiesInfo canopy in canopyCollection)
                        {
                            float fOverhangOffset_x = 0.05f; // // TODO 719 - zadavat v GUI ako cladding property pre roof
                            float fOverhangOffset_y = (float)roofEdgeOverhang_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

                            float fBayWidth = bayWidthCollection[canopy.BayIndex].Width;
                            float fBayStartCoordinate_Y = (iBayIndex * fBayWidth) - fOverhangOffset_y + (float)column_crsc_y_minus;
                            float fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + fOverhangOffset_y + (float)column_crsc_y_plus;

                            if (canopy.BayIndex == 0) // First bay
                                fBayStartCoordinate_Y = (iBayIndex * fBayWidth) + (float)column_crsc_y_minus_temp - (float)roofEdgeOverhang_Y;
                            else if (canopy.BayIndex == canopyCollection.Count - 1) // Last bay
                                fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + (float)column_crsc_y_plus_temp + (float)roofEdgeOverhang_Y;

                            iBayIndex++; // Docasne // Todo 691 - zmazat

                            // Musime menit len tie sheets ktore maju zaciatok na hrane strechy
                            if (MATH.MathF.d_equal(originalsheet.CoordinateInPlane_y, 0, 0.002))
                            {
                                // Zistime ci je plocha originalsheet v kolizii s nejakym canopy - right
                                // Myslim ze mame niekde uz funkcie ktore vedia skontrolovat ci sa dve plochy prekryvaju

                                // Zistime ci je canopy v kolizii s plechom
                                // Ak ano upravime koncove lokalne suradnice plechu y na suradnice canopy a nastavime nove dlzky plechu
                                if (canopy.Right && (
                                   (fBayStartCoordinate_Y <= originalsheet.CoordinateInPlane_x &&
                                   fBayEndCoordinate_Y >= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular)) ||
                                   (fBayStartCoordinate_Y >= originalsheet.CoordinateInPlane_x &&
                                   fBayStartCoordinate_Y <= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular)) ||
                                   (fBayEndCoordinate_Y >= originalsheet.CoordinateInPlane_x &&
                                   fBayEndCoordinate_Y <= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular))))
                                {
                                    float fCanopyCladdingWidth_Right = (float)canopy.WidthRight + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;

                                    originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth_Right;
                                    originalsheet.LengthTopLeft += fCanopyCladdingWidth_Right;
                                    originalsheet.LengthTopRight += fCanopyCladdingWidth_Right;
                                    //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                    originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);

                                    if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed || (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed && !canopy.Left))
                                        break;
                                }
                            }

                            // Pre monopitch upravujeme aj lavu stranu plechu
                            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
                            {
                                // Musime menit len tie sheets ktore maju koniec na hrane strechy
                                if (MATH.MathF.d_equal(originalsheet.CoordinateInPlane_y + originalsheet.LengthTotal, length_left_basic, 0.002))
                                {
                                    // Zistime ci je canopy v kolizii s plechom
                                    // Ak ano upravime koncove lokalne suradnice plechu y na suradnice canopy a nastavime nove dlzky plechu
                                    if (canopy.Left && (
                                       (fBayStartCoordinate_Y <= originalsheet.CoordinateInPlane_x &&
                                       fBayEndCoordinate_Y >= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular)) ||
                                       (fBayStartCoordinate_Y >= originalsheet.CoordinateInPlane_x &&
                                       fBayStartCoordinate_Y <= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular)) ||
                                       (fBayEndCoordinate_Y >= originalsheet.CoordinateInPlane_x &&
                                       fBayEndCoordinate_Y <= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular))))
                                    {
                                        float fCanopyCladdingWidth_Left = (float)canopy.WidthLeft + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;

                                        //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth; // Ostava povodne
                                        originalsheet.LengthTopLeft += fCanopyCladdingWidth_Left;
                                        originalsheet.LengthTopRight += fCanopyCladdingWidth_Left;
                                        //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                        originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Roof - Left Side
                List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRoofLeft = null;
                List<CCladdingOrFibreGlassSheet> listOfFibreGlassSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();

                if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
                {
                    // Roof - Left Side
                    length_left_basic = Drawing3D.GetPoint3DDistanceFloat(pRoof_front3_heightleft, pRoof_front4_top);

                    // FG Sheet 1
                    fPosition_x = 2 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                    fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                    fFBSheetLength = 2.0f; // TODO Input - docasne - dlzka fibreglass sheet

                    // Pre Left side prevratime suradnice v LCS y, aby boli vstupy na oboch stranach brane od spodnej hrany H1
                    fPosition_y = length_left_basic - fPosition_y - fFBSheetLength;

                    listOfFibreGlassSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                        new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                        m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                    iSheet_FG_Index++;

                    // FG Sheet 2
                    fPosition_x = 4 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                    fPosition_y = 0.5f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                    fFBSheetLength = 2.0f; // TODO Input - docasne - dlzka fibreglass sheet

                    // Pre Left side prevratime suradnice v LCS y, aby boli vstupy na oboch stranach brane od spodnej hrany H1
                    fPosition_y = length_left_basic - fPosition_y - fFBSheetLength;

                    listOfFibreGlassSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                        new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                        m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                    iSheet_FG_Index++;

                    // FG Sheet 3
                    fPosition_x = 8 * claddingWidthModular_Roof_FG; // TODO Input - docasne
                    fPosition_y = 0.8f; // TODO Input - docasne - pozicia od spodnej hrany praveho okraja strechy
                    fFBSheetLength = 1.6f; // TODO Input - docasne - dlzka fibreglass sheet

                    // Pre Left side prevratime suradnice v LCS y, aby boli vstupy na oboch stranach brane od spodnej hrany H1
                    fPosition_y = length_left_basic - fPosition_y - fFBSheetLength;

                    listOfFibreGlassSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iSheet_FG_Index + 1, iNumberOfEdges_FG_Roof, fPosition_x, fPosition_y,
                        new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z), claddingWidthModular_Roof_FG, fFBSheetLength, fFBSheetLength, 0, 0,
                        m_ColorNameRoof_FG, m_claddingShape_Roof_FG, m_claddingCoatingType_Roof_FG, m_ColorRoof_FG, fFibreGlassOpacity, claddingWidthRibModular_Roof_FG, true, 0));
                    iSheet_FG_Index++;

                    // Roof - Left Side
                    // Total Width
                    width = pRoof_back3_heightleft.Y - pRoof_front3_heightleft.Y;

                    iNumberOfWholeSheets = (int)(width / claddingWidthModular_Roof);
                    dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Roof;
                    dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
                    iNumberOfSheets = iNumberOfWholeSheets + 1;

                    listOfCladdingSheetsRoofLeft = new List<CCladdingOrFibreGlassSheet>();

                    // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < iNumberOfSheets; i++)
                    {
                        if (bDistinguishedSheetColor)
                            m_ColorRoof = ColorList[i];

                        double length = length_left_basic;

                        int iNumberOfEdges = 4;

                        listOfCladdingSheetsRoofLeft.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, iNumberOfEdges, i * claddingWidthModular_Roof, 0,
                        new Point3D(pRoof_front4_top.X, pRoof_front4_top.Y, pRoof_front4_top.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Roof, length, length, 0, 0,
                        m_ColorNameRoof, m_claddingShape_Roof, m_claddingCoatingType_Roof, m_ColorRoof, options.fRoofCladdingOpacity, claddingWidthRibModular_Roof, true, 0));
                        iSheetIndex++;
                    }

                    // Modifikujeme sheets
                    // Odstranime plechy cladding, ktore su v kolizii s otvormi (FibreGlass, Doors, Windows)
                    // a vytvorime novu sadu plechov ktora zmazany plech nahradi

                    // TO Ondrej - vyrábam uplne nový zoznam a don pridavam povodny sheet alebo ho delim a vytvaram nove
                    // Mozno by sa to dalo urobit uz na prvykrat aby som nerobil zoznam s sheets bez otvorov a potom s otvormi
                    // Uvazujem ci by sa nedal vytvorit originalSheet len "naoko" vo vnutri SplitSheets a tym padom by cele naplnenie povodneho zoznamu odpadlo

                    List<CCladdingOrFibreGlassSheet> listOfCladdingSheetsRoofLeftNew = null;

                    SplitSheets(listOfCladdingSheetsRoofLeft, listOfFibreGlassSheetsRoofLeft,
                        ref iSheetIndex, out listOfCladdingSheetsRoofLeftNew);

                    listOfCladdingSheetsRoofLeft = listOfCladdingSheetsRoofLeftNew; // Nastavime novy zoznam

                    // TODO - upravit plechy pre canopies
                    if (canopyCollection != null)
                    {
                        for (int i = 0; i < listOfCladdingSheetsRoofLeft.Count; i++)
                        {
                            CCladdingOrFibreGlassSheet originalsheet = listOfCladdingSheetsRoofLeft[i];

                            // Musime menit len tie sheets ktore maju koniec na hrane strechy
                            if(MATH.MathF.d_equal(originalsheet.CoordinateInPlane_y + originalsheet.LengthTotal, length_left_basic, 0.002))
                            {
                                // Zistime ake su suradnice canopy start a end pre smer Y !!! Je to nakopirovane z predchadzajuceho kodu,
                                // Najlepsie by asi bolo keby boli suradnice priamo property v CCanopiesInfo

                                int iBayIndex = 0;
                                //----------------------------------------------------------------------------------
                                foreach (CCanopiesInfo canopy in canopyCollection)
                                {
                                    float fOverhangOffset_x = 0.05f; // TODO 719 - zadavat v GUI ako cladding property pre roof
                                    float fOverhangOffset_y = (float)roofEdgeOverhang_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

                                    float fBayWidth = bayWidthCollection[canopy.BayIndex].Width;
                                    float fBayStartCoordinate_Y = (iBayIndex * fBayWidth) - fOverhangOffset_y + (float)column_crsc_y_minus;
                                    float fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + fOverhangOffset_y + (float)column_crsc_y_plus;

                                    if (canopy.BayIndex == 0) // First bay
                                        fBayStartCoordinate_Y = (iBayIndex * fBayWidth) + (float)column_crsc_y_minus_temp - (float)roofEdgeOverhang_Y;
                                    else if (canopy.BayIndex == canopyCollection.Count - 1) // Last bay
                                        fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + (float)column_crsc_y_plus_temp + (float)roofEdgeOverhang_Y;

                                    iBayIndex++; // Docasne // Todo 691 - zmazat

                                    // Zistime ci je plocha originalsheet v kolizii s nejakym canopy - right
                                    // Myslim ze mame niekde uz funkcie ktore vedia skontrolovat ci sa dve plochy prekryvaju

                                    // Zistime ci je canopy v kolizii s plechom
                                    // Ak ano upravime koncove lokalne suradnice plechu y na suradnice canopy a nastavime nove dlzky plechu
                                    if (canopy.Left && (
                                        (fBayStartCoordinate_Y <= originalsheet.CoordinateInPlane_x &&
                                        fBayEndCoordinate_Y >= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular)) ||
                                        (fBayStartCoordinate_Y >= originalsheet.CoordinateInPlane_x &&
                                        fBayStartCoordinate_Y <= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular)) ||
                                        (fBayEndCoordinate_Y >= originalsheet.CoordinateInPlane_x &&
                                        fBayEndCoordinate_Y <= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular))))
                                    {
                                        float fCanopyCladdingWidth_Left = (float)canopy.WidthLeft + fOverhangOffset_x - (float)column_crsc_z_plus_temp - (float)roofEdgeOverhang_X;

                                        //originalsheet.CoordinateInPlane_y -= fCanopyCladdingWidth_Left;
                                        originalsheet.LengthTopLeft += fCanopyCladdingWidth_Left;
                                        originalsheet.LengthTopRight += fCanopyCladdingWidth_Left;
                                        //originalsheet.LengthTopTip - vsetky plechy canopies maju len 4 hrany
                                        originalsheet.LengthTotal = Math.Max(originalsheet.LengthTopLeft, originalsheet.LengthTopRight);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Vytvorime geometry model

                // TODO - osetrit pripady ked moze byt list null

                model_gr = new Model3DGroup(); // Vypraznime model group

                for (int i = 0; i < listOfCladdingSheetsLeftWall.Count; i++)
                {
                    // Pridame sheet do model group
                    GeometryModel3D sheetModel = listOfCladdingSheetsLeftWall[i].GetCladdingSheetModel(options);
                    sheetModel.Transform = listOfCladdingSheetsLeftWall[i].GetTransformGroup(0, 0, -90);
                    model_gr.Children.Add(sheetModel);
                }

                if (listOfFibreGlassSheetsWallLeft != null)
                {
                    // Generujeme FG sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < listOfFibreGlassSheetsWallLeft.Count; i++)
                    {
                        // Pridame sheet do model group
                        GeometryModel3D sheetModel = listOfFibreGlassSheetsWallLeft[i].GetCladdingSheetModel(options);
                        sheetModel.Transform = listOfFibreGlassSheetsWallLeft[i].GetTransformGroup(0, 0, -90);
                        model_gr.Children.Add(sheetModel);
                    }
                }

                for (int i = 0; i < listOfCladdingSheetsFrontWall.Count; i++)
                {
                    // Pridame sheet do model group
                    GeometryModel3D sheetModel = listOfCladdingSheetsFrontWall[i].GetCladdingSheetModel(options);
                    sheetModel.Transform = listOfCladdingSheetsFrontWall[i].GetTransformGroup(0, 0, 0);
                    model_gr.Children.Add(sheetModel);
                }

                if (listOfFibreGlassSheetsWallFront != null)
                {
                    // Generujeme FG sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < listOfFibreGlassSheetsWallFront.Count; i++)
                    {
                        // Pridame sheet do model group
                        GeometryModel3D sheetModel = listOfFibreGlassSheetsWallFront[i].GetCladdingSheetModel(options);
                        sheetModel.Transform = listOfFibreGlassSheetsWallFront[i].GetTransformGroup(0, 0, 0);
                        model_gr.Children.Add(sheetModel);
                    }
                }

                for (int i = 0; i < listOfCladdingSheetsRightWall.Count; i++)
                {
                    // Pridame sheet do model group
                    GeometryModel3D sheetModel = listOfCladdingSheetsRightWall[i].GetCladdingSheetModel(options);
                    sheetModel.Transform = listOfCladdingSheetsRightWall[i].GetTransformGroup(0, 0, 90);
                    model_gr.Children.Add(sheetModel);
                }

                if (listOfFibreGlassSheetsWallRight != null)
                {
                    // Generujeme FG sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < listOfFibreGlassSheetsWallRight.Count; i++)
                    {
                        // Pridame sheet do model group
                        GeometryModel3D sheetModel = listOfFibreGlassSheetsWallRight[i].GetCladdingSheetModel(options);
                        sheetModel.Transform = listOfFibreGlassSheetsWallRight[i].GetTransformGroup(0, 0, 90);
                        model_gr.Children.Add(sheetModel);
                    }
                }

                for (int i = 0; i < listOfCladdingSheetsBackWall.Count; i++)
                {
                    // Pridame sheet do model group
                    GeometryModel3D sheetModel = listOfCladdingSheetsBackWall[i].GetCladdingSheetModel(options);
                    sheetModel.Transform = listOfCladdingSheetsBackWall[i].GetTransformGroup(0, 0, 180);
                    model_gr.Children.Add(sheetModel);
                }

                if (listOfFibreGlassSheetsWallBack != null)
                {
                    // Generujeme FG sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < listOfFibreGlassSheetsWallBack.Count; i++)
                    {
                        // Pridame sheet do model group
                        GeometryModel3D sheetModel = listOfFibreGlassSheetsWallBack[i].GetCladdingSheetModel(options);
                        sheetModel.Transform = listOfFibreGlassSheetsWallBack[i].GetTransformGroup(0, 0, 180);
                        model_gr.Children.Add(sheetModel);
                    }
                }

                float rotationAboutX;

                for (int i = 0; i < listOfCladdingSheetsRoofRight.Count; i++)
                {
                    // Pridame sheet do model group
                    rotationAboutX = -90f + (eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? sBuildingGeomInputData.fRoofPitch_deg : -sBuildingGeomInputData.fRoofPitch_deg);
                    GeometryModel3D sheetModel = listOfCladdingSheetsRoofRight[i].GetCladdingSheetModel(options);
                    sheetModel.Transform = listOfCladdingSheetsRoofRight[i].GetTransformGroup(rotationAboutX, 0, 90);
                    model_gr.Children.Add(sheetModel);
                }

                // Generujeme FG sheets pre jednu stranu, resp. jednu rovinu
                for (int i = 0; i < listOfFibreGlassSheetsRoofRight.Count; i++)
                {
                    // Pridame sheet do model group
                    rotationAboutX = -90f + (eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? sBuildingGeomInputData.fRoofPitch_deg : -sBuildingGeomInputData.fRoofPitch_deg);
                    GeometryModel3D sheetModel = listOfFibreGlassSheetsRoofRight[i].GetCladdingSheetModel(options);
                    sheetModel.Transform = listOfFibreGlassSheetsRoofRight[i].GetTransformGroup(rotationAboutX, 0, 90);
                    model_gr.Children.Add(sheetModel);
                }

                if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
                {
                    rotationAboutX = -90f - sBuildingGeomInputData.fRoofPitch_deg;

                    // Generujeme sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < listOfCladdingSheetsRoofLeft.Count; i++)
                    {
                        // Pridame sheet do model group
                        GeometryModel3D sheetModel = listOfCladdingSheetsRoofLeft[i].GetCladdingSheetModel(options);
                        sheetModel.Transform = listOfCladdingSheetsRoofLeft[i].GetTransformGroup(rotationAboutX, 0, 90);
                        model_gr.Children.Add(sheetModel);
                    }

                    // Generujeme FG sheets pre jednu stranu, resp. jednu rovinu
                    for (int i = 0; i < listOfFibreGlassSheetsRoofLeft.Count; i++)
                    {
                        // Pridame sheet do model group
                        GeometryModel3D sheetModel = listOfFibreGlassSheetsRoofLeft[i].GetCladdingSheetModel(options);
                        sheetModel.Transform = listOfFibreGlassSheetsRoofLeft[i].GetTransformGroup(rotationAboutX, 0, 90);
                        model_gr.Children.Add(sheetModel);
                    }
                }
            }

            return model_gr;
        }

        public double GetVerticalCoordinate(string sBuildingSide, EModelType_FS eKitset, double width, double leftHeight, float fx)
        {
            if (sBuildingSide == "Left" || sBuildingSide == "Right")
                return leftHeight;
            else //if(sBuildingSide == "Front" || sBuildingSide == "Back")
            {

                if (eKitset == EModelType_FS.eKitsetMonoRoofEnclosed)
                {
                    if (sBuildingSide == "Back")
                        return leftHeight + fx * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                    else
                        return leftHeight + fx * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                }
                else if (fx < 0.5f * width)
                    return leftHeight + fx * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
                else
                    return leftHeight + (width - fx) * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);
            }
        }

        public void SplitSheets(List<CCladdingOrFibreGlassSheet> originalListOfSheets,
            List<CCladdingOrFibreGlassSheet> listOfOpenings,
            ref int iSheetIndex, out List<CCladdingOrFibreGlassSheet> listOfSheetsNew)
        {
            listOfSheetsNew = new List<CCladdingOrFibreGlassSheet>(); // Nove pole kombinovane z povodnych aj novych nadelenych sheets

            for (int i = 0; i < originalListOfSheets.Count; i++)
            {
                CCladdingOrFibreGlassSheet originalsheet = originalListOfSheets[i];

                // listOfOpenings // TODO - zapracovat aj doors a windows, vyrobit vseobecneho predka pre fibreglassSheet, door, window, ktory bude reprezentovat otvor v cladding

                // 1. Zistime ci lezi originalsheet v rovine s nejakym objektom listOfOpenings
                // Bod 1 mozeme preskocit ak vieme na ktorej strane resp v ktorej stresnej rovine sa nachadzame

                // 2. Zistime ci je plocha originalsheet v kolizii s nejakym objektom listOfOpenings
                // Myslim ze mame niekde uz funkcie ktore vedia skontrolovat ci sa dve plochy prekryvaju

                // Zoznam objektov ktore su v kolizii
                List<CCladdingOrFibreGlassSheet> objectInColision_In_Local_x = listOfOpenings.Where(o => (o.CoordinateInPlane_x <= originalsheet.CoordinateInPlane_x && (o.CoordinateInPlane_x + o.WidthModular) >= (originalsheet.CoordinateInPlane_x + originalsheet.WidthModular))).ToList();

                // Ak neexistuju objekty v kolizii s originalsheet mozeme opustit funkciu
                if (objectInColision_In_Local_x == null || objectInColision_In_Local_x.Count == 0)
                {
                    listOfSheetsNew.Add(originalsheet);
                    iSheetIndex++;
                }
                else if (objectInColision_In_Local_x.Count == 1 &&
                         objectInColision_In_Local_x[0].CoordinateInPlane_y <= originalsheet.CoordinateInPlane_y &&
                         objectInColision_In_Local_x[0].LengthTotal >= originalsheet.LengthTotal) // Otvor je dlhsi ako cely povodny sheet, odstranime len originalsheet a nic nepridavame
                {
                    // TODO - este by sa mohlo stat ze openings je sice viac nadefinovanych priamo vedla seba, ale tvoria jeden velky otvor
                    // Asi je to nezmyselne zadanie, ale malo by to byt osetrene
                    // Do not add sheet to the list - whole original sheet is substituted by opening
                }
                else
                {
                    // Predpokladame ze samotne objekty v listOfOpenings sa neprekyvaju
                    // 3. Zoradime objekty podla lokalnej suradnice y
                    objectInColision_In_Local_x.OrderBy(o => originalsheet.CoordinateInPlane_y);

                    // 4. Podla poctu objektov v objectInColision_In_Local_x a ich suradnic vieme na kolko casti budeme originalsheet delit
                    int iNumberOfNewSheets = objectInColision_In_Local_x.Count + 1; // TODO skontrolovat podla suradnic ci objekt zacina na alebo konci priamo na hrane a podla toho upravit pocet novych, ktore treba vytvorit

                    // 5. Pridame nove sheets do zoznamu
                    for (int j = 0; j < iNumberOfNewSheets; j++)
                    {
                        if (j == iNumberOfNewSheets - 1) // Last segment of original sheet
                        {
                            listOfSheetsNew.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, originalsheet.NumberOfEdges,
                            originalsheet.CoordinateInPlane_x,
                            objectInColision_In_Local_x[j - 1].CoordinateInPlane_y + (float)objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheet.ControlPoint, originalsheet.WidthModular,
                            originalsheet.LengthTopLeft - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - (float)objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheet.LengthTopRight - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - (float)objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheet.TipCoordinate_x,
                            originalsheet.LengthTopTip - objectInColision_In_Local_x[j - 1].CoordinateInPlane_y - (float)objectInColision_In_Local_x[j - 1].LengthTotal,
                            originalsheet.ColorName, originalsheet.CladdingShape, originalsheet.CladdingCoatingType,
                            originalsheet.Color, originalsheet.Opacity, originalsheet.CladdingWidthRibModular, originalsheet.BIsDisplayed, originalsheet.FTime));
                            iSheetIndex++;

                        }
                        else
                        {
                            float coordinate_y = 0; // Zacat od okraja  !!! - je potrebne zmenit pre doors a zacat nad dverami

                            if (j > 0)
                                coordinate_y = objectInColision_In_Local_x[j-1].CoordinateInPlane_y + (float)objectInColision_In_Local_x[j-1].LengthTotal;

                            int iNumberOfEdges = 4;
                            listOfSheetsNew.Add(new CCladdingOrFibreGlassSheet(iSheetIndex + 1, iNumberOfEdges,
                            originalsheet.CoordinateInPlane_x,
                            coordinate_y,
                            originalsheet.ControlPoint, originalsheet.WidthModular,
                            objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinate_y,
                            objectInColision_In_Local_x[j].CoordinateInPlane_y - coordinate_y,
                            0,
                            0,
                            originalsheet.ColorName, originalsheet.CladdingShape, originalsheet.CladdingCoatingType,
                            originalsheet.Color, originalsheet.Opacity, originalsheet.CladdingWidthRibModular, originalsheet.BIsDisplayed, originalsheet.FTime));
                            iSheetIndex++;
                        }
                    }
                }
            }
        }

        // To Ondrej - toto som prevzal z CComboboxHelper.cs v projekte PFD. Tu na to nevidiet lebo BaseClasses su includovane v PFD.
        // Asi by sa mala vyvorit pomocna trieda pre farby v BaseClasses alebo este lepsie - zaviest kompletnu databazu farieb System.Windows.Media.Colors
        // a z nej vyberat

        public List<Color> ColorList = new List<Color>() {
                Colors.AliceBlue,
                Colors.AntiqueWhite,
                Colors.Aqua,
                Colors.Aquamarine,
                Colors.Azure,
                Colors.Beige,
                Colors.Bisque,
                Colors.Black,
                Colors.BlanchedAlmond,
                Colors.Blue,
                Colors.BlueViolet,
                Colors.Brown,
                Colors.BurlyWood,
                Colors.CadetBlue,
                Colors.Chartreuse,
                Colors.Chocolate,
                Colors.Coral,
                Colors.CornflowerBlue,
                Colors.Cornsilk,
                Colors.Crimson,
                Colors.Cyan,
                Colors.DarkBlue,
                Colors.DarkCyan,
                Colors.DarkGoldenrod,
                Colors.DarkGray,
                Colors.DarkGreen,
                Colors.DarkKhaki,
                Colors.DarkMagenta,
                Colors.DarkOliveGreen,
                Colors.DarkOrange,
                Colors.DarkOrchid,
                Colors.DarkRed,
                Colors.DarkSalmon,
                Colors.DarkSeaGreen,
                Colors.DarkSlateBlue,
                Colors.DarkSlateGray,
                Colors.DarkTurquoise,
                Colors.DarkViolet,
                Colors.DeepPink,
                Colors.DeepSkyBlue,
                Colors.DimGray,
                Colors.DodgerBlue,
                Colors.Firebrick,
                Colors.FloralWhite,
                Colors.ForestGreen,
                Colors.Fuchsia,
                Colors.Gainsboro,
                Colors.GhostWhite,
                Colors.Gold,
                Colors.Goldenrod,
                Colors.Gray,
                Colors.Green,
                Colors.GreenYellow,
                Colors.Honeydew,
                Colors.HotPink,
                Colors.IndianRed,
                Colors.Indigo,
                Colors.Ivory,
                Colors.Khaki,
                Colors.Lavender,
                Colors.LavenderBlush,
                Colors.LawnGreen,
                Colors.LemonChiffon,
                Colors.LightBlue,
                Colors.LightCoral,
                Colors.LightCyan,
                Colors.LightGoldenrodYellow,
                Colors.LightGray,
                Colors.LightGreen,
                Colors.LightPink,
                Colors.LightSalmon,
                Colors.LightSeaGreen,
                Colors.LightSkyBlue,
                Colors.LightSlateGray,
                Colors.LightSteelBlue,
                Colors.LightYellow,
                Colors.Lime,
                Colors.LimeGreen,
                Colors.Linen,
                Colors.Magenta,
                Colors.Maroon,
                Colors.MediumAquamarine,
                Colors.MediumBlue,
                Colors.MediumOrchid,
                Colors.MediumPurple,
                Colors.MediumSeaGreen,
                Colors.MediumSlateBlue,
                Colors.MediumSpringGreen,
                Colors.MediumTurquoise,
                Colors.MediumVioletRed,
                Colors.MidnightBlue,
                Colors.MintCream,
                Colors.MistyRose,
                Colors.Moccasin,
                Colors.NavajoWhite,
                Colors.Navy,
                Colors.OldLace,
                Colors.Olive,
                Colors.OliveDrab,
                Colors.Orange,
                Colors.OrangeRed,
                Colors.Orchid,
                Colors.PaleGoldenrod,
                Colors.PaleGreen,
                Colors.PaleTurquoise,
                Colors.PaleVioletRed,
                Colors.PapayaWhip,
                Colors.PeachPuff,
                Colors.Peru,
                Colors.Pink,
                Colors.Plum,
                Colors.PowderBlue,
                Colors.Purple,
                Colors.Red,
                Colors.RosyBrown,
                Colors.RoyalBlue,
                Colors.SaddleBrown,
                Colors.Salmon,
                Colors.SandyBrown,
                Colors.SeaGreen,
                Colors.SeaShell,
                Colors.Sienna,
                Colors.Silver,
                Colors.SkyBlue,
                Colors.SlateBlue,
                Colors.SlateGray,
                Colors.Snow,
                Colors.SpringGreen,
                Colors.SteelBlue,
                Colors.Tan,
                Colors.Teal,
                Colors.Thistle,
                Colors.Tomato,
                Colors.Transparent,
                Colors.Turquoise,
                Colors.Olive,
                Colors.Wheat,
                Colors.White,
                Colors.WhiteSmoke,
                Colors.Yellow,
                Colors.YellowGreen
        };
    }
}
