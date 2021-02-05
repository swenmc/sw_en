using BaseClasses.Helpers;
using System;
using System.Collections.Generic;
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

        double claddingHeight_Wall = 0.030; // z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m v tabulkach tableSections_m alebo trapezoidalSheeting_m
        double claddingHeight_Roof = 0.075; // z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m

        double claddingWidthRibModular_Wall = 0.190; // m // z databazy cladding MDBTrapezoidalSheeting widthRib_m
        double claddingWidthRibModular_Roof = 0.300; // m // z databazy cladding MDBTrapezoidalSheeting widthRib_m

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

        public CCladding()
        {

        }

        // Constructor 2
        public CCladding(int iCladding_ID, EModelType_FS modelType_FS, BuildingGeometryDataInput sGeometryInputData,
            System.Collections.ObjectModel.ObservableCollection<CCanopiesInfo> canopies,
            System.Collections.ObjectModel.ObservableCollection<CBayInfo> bayWidths,
            CRSC.CCrSc_TW columnSection,
            string colorName_Wall, string colorName_Roof, string claddingShape_Wall, string claddingCoatingType_Wall, string claddingShape_Roof, string claddingCoatingType_Roof,
            Color colorWall, Color colorRoof,
            bool bIsDisplayed, int fTime, double wallCladdingHeight, double roofCladdingHeight, double wallCladdingWidthRib, double roofCladdingWidthRib)
        {
            ID = iCladding_ID;
            eModelType = modelType_FS;
            sBuildingGeomInputData = sGeometryInputData;
            canopyCollection = canopies;
            bayWidthCollection = bayWidths;
            column_crsc_z_plus = columnSection.z_max;
            column_crsc_y_minus = columnSection.y_min;
            column_crsc_y_plus = columnSection.y_max;
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
        }

        public Model3DGroup GetCladdingModel(DisplayOptions options)
        {
            m_pControlPoint = new Point3D(0, 0, 0);

            Model3DGroup model_gr = new Model3DGroup();

            // Vytvorime model v GCS [0,0,0] je uvazovana v bode m_ControlPoint

            double bottomEdge_z = 0;

            double additionalOffset = 0.010;  // 10 mm

            double height_1_final = sBuildingGeomInputData.fH_1_centerline + column_crsc_z_plus + additionalOffset + claddingHeight_Roof; // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
            double height_2_final = sBuildingGeomInputData.fH_2_centerline + column_crsc_z_plus + additionalOffset + claddingHeight_Roof; // TODO - dopocitat presne, zohladnit edge purlin a sklon

            // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
            double column_crsc_y_minus_temp = column_crsc_y_minus - additionalOffset;
            double column_crsc_y_plus_temp = column_crsc_y_plus + additionalOffset;
            double column_crsc_z_plus_temp = column_crsc_z_plus + additionalOffset;

            // Cladding Edges

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus_temp - claddingHeight_Wall, column_crsc_y_minus_temp - claddingHeight_Wall, bottomEdge_z);
            Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + claddingHeight_Wall, column_crsc_y_minus_temp - claddingHeight_Wall, bottomEdge_z);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus_temp - claddingHeight_Wall, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, bottomEdge_z);
            Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + claddingHeight_Wall, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, bottomEdge_z);

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

            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                // Monopitch Roof

                Point3D pfront2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + claddingHeight_Wall, column_crsc_y_minus_temp - claddingHeight_Wall, height_2_final);
                Point3D pfront3_heightleft = new Point3D(-column_crsc_z_plus_temp - claddingHeight_Wall, column_crsc_y_minus_temp - claddingHeight_Wall, height_1_final);

                Point3D pback2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + claddingHeight_Wall, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, height_2_final);
                Point3D pback3_heightleft = new Point3D(-column_crsc_z_plus_temp - claddingHeight_Wall, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, height_1_final);

                if (options.bUseTextures) // Pouzijeme len ak vykreslujeme textury, inak sa pouzije material vytvoreny z SolidColorBrush podla vybranej farby cladding v GUI
                {
                    wpWidth = claddingWidthRibModular_Wall / (pfront1_baseright.X - pfront0_baseleft.X);
                    wpHeight = claddingWidthRibModular_Wall / (pfront2_heightright.Z - pfront1_baseright.Z);
                    brushFront.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_FrontBackWall = new DiffuseMaterial(brushFront);
                }

                // Front Wall
                model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_heightright, pfront3_heightleft }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));
                // Back Wall
                model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pback3_heightleft, pback2_heightright }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                    wpHeight = claddingWidthRibModular_Wall / (pback2_heightright.Z - pback0_baseleft.Z);
                    brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_SideWall = new DiffuseMaterial(brushSide);
                }

                // Left Wall
                model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pfront3_heightleft, pback3_heightleft }, 0).CreateArea(options.bUseTextures, material_SideWall));
                // Right Wall
                model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pback2_heightright, pfront2_heightright }, 0).CreateArea(options.bUseTextures, material_SideWall));

                if (options.bUseTextures)
                {
                    double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront2_heightright, pfront3_heightleft); // Rovina XZ
                    wpWidth = claddingWidthRibModular_Roof / (pback2_heightright.Y - pfront2_heightright.Y);
                    wpHeight = claddingWidthRibModular_Roof / poinstsDist;
                    brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_Roof = new DiffuseMaterial(brushRoof);
                }

                // Roof
                model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pfront2_heightright, pback2_heightright, pback3_heightleft, pfront3_heightleft }, 0).CreateArea(options.bUseTextures, material_Roof));

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
                    float fOverhangOffset_y = 0; // TODO - zadavat v GUI ako cladding property pre roof

                    float fBayWidth = bayWidthCollection[canopy.BayIndex].Width;
                    float fBayStartCoordinate_Y = (iBayIndex * fBayWidth) - fOverhangOffset_y + (float)column_crsc_y_minus;
                    float fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + fOverhangOffset_y + (float)column_crsc_y_plus;

                    if(canopy.BayIndex == 0) // First bay
                       fBayStartCoordinate_Y = (iBayIndex * fBayWidth) + (float)column_crsc_y_minus_temp - (float)claddingHeight_Wall;
                    else if(canopy.BayIndex == canopyCollection.Count - 1) // Last bay
                        fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + (float)column_crsc_y_plus_temp + (float)claddingHeight_Wall;

                    iBayIndex++; // Docasne // Todo 691 - zmazat

                    float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y - (float)column_crsc_y_minus_temp + (float)claddingHeight_Wall;
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

                        float fCanopyCladdingWidth = (float)canopy.WidthLeft + fOverhangOffset_x;
                        float fCanopy_EdgeCoordinate_z = (float)height_1_final + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(0 - fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_left = new Point3D(0 - fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pfront_right = new Point3D(pfront3_heightleft.X, fBayStartCoordinate_Y, height_1_final);
                        Point3D pback_right = new Point3D(pback3_heightleft.X, fBayEndCoordinate_Y, height_1_final);

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

                    if(canopy.Right)
                    {
                        float fCanopyCladdingWidth = (float)canopy.WidthRight + fOverhangOffset_x;
                        float fCanopy_EdgeCoordinate_z = (float)height_2_final + fCanopyCladdingWidth * (float)Math.Tan(sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(pfront2_heightright.X, fBayStartCoordinate_Y, height_2_final);
                        Point3D pback_left = new Point3D(pback2_heightright.X, fBayEndCoordinate_Y, height_2_final);
                        Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);

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
                // Gable Roof

                Point3D pfront2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + claddingHeight_Wall, column_crsc_y_minus_temp - claddingHeight_Wall, height_1_final);
                Point3D pfront3_heightleft = new Point3D(-column_crsc_z_plus_temp - claddingHeight_Wall, column_crsc_y_minus_temp - claddingHeight_Wall, height_1_final);
                Point3D pfront4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, column_crsc_y_minus_temp - claddingHeight_Wall, height_2_final);

                Point3D pback2_heightright = new Point3D(sBuildingGeomInputData.fW_centerline + column_crsc_z_plus_temp + claddingHeight_Wall, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, height_1_final);
                Point3D pback3_heightleft = new Point3D(-column_crsc_z_plus_temp - claddingHeight_Wall, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, height_1_final);
                Point3D pback4_top = new Point3D(0.5 * sBuildingGeomInputData.fW_centerline, sBuildingGeomInputData.fL_centerline + column_crsc_y_plus_temp + claddingHeight_Wall, height_2_final);

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pfront1_baseright.X - pfront0_baseleft.X);
                    wpHeight = claddingWidthRibModular_Wall / (pfront4_top.Z - pfront1_baseright.Z);
                    brushFront.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_FrontBackWall = new DiffuseMaterial(brushFront);
                }

                // Front Wall
                model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_heightright, pfront4_top, pfront3_heightleft }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));
                // Back Wall
                model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pback3_heightleft, pback4_top, pback2_heightright }, 0).CreateArea(options.bUseTextures, material_FrontBackWall));

                if (options.bUseTextures)
                {
                    wpWidth = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                    wpHeight = claddingWidthRibModular_Wall / (pback2_heightright.Z - pback0_baseleft.Z);
                    brushSide.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_SideWall = new DiffuseMaterial(brushSide);
                }

                // Left Wall
                model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pfront3_heightleft, pback3_heightleft }, 0).CreateArea(options.bUseTextures, material_SideWall));
                // Right Wall
                model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pback2_heightright, pfront2_heightright }, 0).CreateArea(options.bUseTextures, material_SideWall));

                if (options.bUseTextures)
                {
                    double poinstsDist = Drawing3D.GetPoint3DDistanceDouble(pfront4_top, pfront3_heightleft); // Rovina XZ
                    wpWidth = claddingWidthRibModular_Roof / (pback4_top.Y - pfront4_top.Y);
                    wpHeight = claddingWidthRibModular_Roof / poinstsDist;
                    brushRoof.Viewport = new System.Windows.Rect(0, 0, wpWidth, wpHeight);
                    material_Roof = new DiffuseMaterial(brushRoof);
                }

                // Roof - Left Side
                model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pfront4_top, pback4_top, pback3_heightleft, pfront3_heightleft }, 0).CreateArea(options.bUseTextures, material_Roof));
                // Roof - Right Side
                model_gr.Children.Add(new CAreaPolygonal(5, new List<Point3D>() { pfront2_heightright, pback2_heightright, pback4_top, pfront4_top }, 0).CreateArea(options.bUseTextures, material_Roof));

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

                    float fOverhangOffset_x = 0.15f; // TODO - zadavat v GUI ako cladding property pre roof
                    float fOverhangOffset_y = 0; // TODO - zadavat v GUI ako cladding property pre roof

                    float fBayWidth = bayWidthCollection[canopy.BayIndex].Width;
                    float fBayStartCoordinate_Y = (iBayIndex * fBayWidth) - fOverhangOffset_y + (float)column_crsc_y_minus;
                    float fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + fOverhangOffset_y + (float)column_crsc_y_plus;

                    if (canopy.BayIndex == 0) // First bay
                        fBayStartCoordinate_Y = (iBayIndex * fBayWidth) + (float)column_crsc_y_minus_temp - (float)claddingHeight_Wall;
                    else if (canopy.BayIndex == canopyCollection.Count - 1) // Last bay
                        fBayEndCoordinate_Y = ((iBayIndex + 1) * fBayWidth) + (float)column_crsc_y_plus_temp + (float)claddingHeight_Wall;

                    iBayIndex++; // Docasne // Todo 691 - zmazat

                    float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y - (float)column_crsc_y_minus_temp + (float)claddingHeight_Wall;
                    int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / claddingWidthRibModular_Roof);
                    double dWidthOfWholeRibs = iNumberOfWholeRibs * claddingWidthRibModular_Roof;
                    double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                    if (canopy.Left)
                    {
                        float fCanopyCladdingWidth = (float)canopy.WidthLeft + fOverhangOffset_x;
                        float fCanopy_EdgeCoordinate_z = (float)height_1_final + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(0 - fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_left = new Point3D(0 - fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pfront_right = new Point3D(pfront3_heightleft.X, fBayStartCoordinate_Y, height_1_final);
                        Point3D pback_right = new Point3D(pback3_heightleft.X, fBayEndCoordinate_Y, height_1_final);

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
                        float fCanopyCladdingWidth = (float)canopy.WidthRight + fOverhangOffset_x;
                        float fCanopy_EdgeCoordinate_z = (float)height_1_final + fCanopyCladdingWidth * (float)Math.Tan(-sBuildingGeomInputData.fRoofPitch_deg * Math.PI / 180);

                        Point3D pfront_left = new Point3D(pfront2_heightright.X, fBayStartCoordinate_Y, height_1_final);
                        Point3D pback_left = new Point3D(pback2_heightright.X, fBayEndCoordinate_Y, height_1_final);
                        Point3D pfront_right = new Point3D(sBuildingGeomInputData.fW_centerline + fCanopyCladdingWidth, fBayStartCoordinate_Y, fCanopy_EdgeCoordinate_z);
                        Point3D pback_right = new Point3D(sBuildingGeomInputData.fW_centerline + fCanopyCladdingWidth, fBayEndCoordinate_Y, fCanopy_EdgeCoordinate_z);

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







            //------------------------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------

            // IN WORK
            // Particular Cladding Sheet Model

            model_gr = new Model3DGroup(); // Vypraznime model group

            // Prva uroven, stany budovy alebo strechy, left, right, front, back, roof left roof right
            // Druha uroven jednotlive sheet nachadzajuce sa v jednej rovine
            List<List<CCladdingSheet>> listOfCladdingSheets = new List<List<CCladdingSheet>>();

            float claddingWidthModular_Wall = 0.6f; // TODO - napojit na DB

            bool bDistinguishedSheetColor = true;

            // Left Wall
            // Total Wall Width
            double width = pback0_baseleft.Y - pfront0_baseleft.Y;
            double height_left_basic = height_1_final;
            float rotationAboutZ = -90f;

            int iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            double dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            double dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            int iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<CCladdingSheet> listOfCladdingSheetsLeftWall = new List<CCladdingSheet>();

            int iSheetIndex = 0;

            // Generujeme sheets pre jednu stranu, resp. jednu rovinu
            for (int i = 0; i < iNumberOfSheets; i++)
            {
                if (bDistinguishedSheetColor)
                    m_ColorWall = ColorList[i];

                listOfCladdingSheetsLeftWall.Add(new CCladdingSheet(iSheetIndex + 1, 4, i * claddingWidthModular_Wall, 0,
                new Point3D(pback0_baseleft.X, pback0_baseleft.Y - i * claddingWidthModular_Wall, pback0_baseleft.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left_basic, height_left_basic, 0.5 *( i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall), height_left_basic,
                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                iSheetIndex++;

                // Pridame sheet do model group
                GeometryModel3D sheetModel = listOfCladdingSheetsLeftWall[i].GetCladdingSheetModel(options);
                sheetModel.Transform = listOfCladdingSheetsLeftWall[i].GetTransformGroup(0, 0, rotationAboutZ);
                model_gr.Children.Add(sheetModel);
            }

            // Front Wall
            // Total Wall Width
            width = pfront1_baseright.X - pfront0_baseleft.X;
            height_left_basic = height_1_final;
            rotationAboutZ = 0f;

            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<CCladdingSheet> listOfCladdingSheetsFrontWall = new List<CCladdingSheet>();

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

                if(eModelType == EModelType_FS.eKitsetGableRoofEnclosed &&
                   i * claddingWidthModular_Wall < 0.5 * width &&
                   (i+1) * claddingWidthModular_Wall > 0.5 * width)
                {
                    iNumberOfEdges = 5;
                    height_toptip = height_2_final;
                    tipCoordinate_x = 0.5 * width - i * claddingWidthModular_Wall;
                }

                listOfCladdingSheetsFrontWall.Add(new CCladdingSheet(iSheetIndex + 1, iNumberOfEdges, i * claddingWidthModular_Wall, 0,
                new Point3D(pfront0_baseleft.X + i * claddingWidthModular_Wall, pfront0_baseleft.Y, pfront0_baseleft.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left, height_right, tipCoordinate_x, height_toptip,
                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                iSheetIndex++;

                // Pridame sheet do model group
                GeometryModel3D sheetModel = listOfCladdingSheetsFrontWall[i].GetCladdingSheetModel(options);
                sheetModel.Transform = listOfCladdingSheetsFrontWall[i].GetTransformGroup(0, 0, rotationAboutZ);
                model_gr.Children.Add(sheetModel);
            }

            // Right Wall
            // Total Wall Width
            width = pback1_baseright.Y - pfront1_baseright.Y;
            height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? height_1_final : height_2_final;
            rotationAboutZ = 90f;

            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<CCladdingSheet> listOfCladdingSheetsRightWall = new List<CCladdingSheet>();

            iSheetIndex = 0;

            // Generujeme sheets pre jednu stranu, resp. jednu rovinu
            for (int i = 0; i < iNumberOfSheets; i++)
            {
                if (bDistinguishedSheetColor)
                    m_ColorWall = ColorList[i];

                listOfCladdingSheetsRightWall.Add(new CCladdingSheet(iSheetIndex + 1, 4, i * claddingWidthModular_Wall, 0,
                new Point3D(pfront1_baseright.X, pfront1_baseright.Y + i * claddingWidthModular_Wall, pfront1_baseright.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left_basic, height_left_basic, 0.5 * (i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall), height_left_basic,
                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fLeftCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                iSheetIndex++;

                // Pridame sheet do model group
                GeometryModel3D sheetModel = listOfCladdingSheetsRightWall[i].GetCladdingSheetModel(options);
                sheetModel.Transform = listOfCladdingSheetsRightWall[i].GetTransformGroup(0, 0, rotationAboutZ);
                model_gr.Children.Add(sheetModel);
            }

            // Back Wall
            // Total Wall Width
            width = pback1_baseright.X - pback0_baseleft.X;
            height_left_basic = eModelType == EModelType_FS.eKitsetGableRoofEnclosed ? height_1_final : height_2_final;
            rotationAboutZ = 180f;

            iNumberOfWholeSheets = (int)(width / claddingWidthModular_Wall);
            dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular_Wall;
            dPartialSheet_End = width - dWidthOfWholeSheets; // Last Sheet
            iNumberOfSheets = iNumberOfWholeSheets + 1;

            List<CCladdingSheet> listOfCladdingSheetsBackWall = new List<CCladdingSheet>();

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
                    height_toptip = height_2_final;
                    tipCoordinate_x = 0.5 * width - i * claddingWidthModular_Wall;
                }

                listOfCladdingSheetsBackWall.Add(new CCladdingSheet(iSheetIndex + 1, iNumberOfEdges, i * claddingWidthModular_Wall, 0,
                new Point3D(pback1_baseright.X - i * claddingWidthModular_Wall, pback1_baseright.Y, pback1_baseright.Z), i == iNumberOfSheets - 1 ? (float)dPartialSheet_End : claddingWidthModular_Wall, height_left, height_right, tipCoordinate_x, height_toptip,
                m_ColorNameWall, m_claddingShape_Wall, m_claddingCoatingType_Wall, m_ColorWall, options.fFrontCladdingOpacity, claddingWidthRibModular_Wall, true, 0));
                iSheetIndex++;

                // Pridame sheet do model group
                GeometryModel3D sheetModel = listOfCladdingSheetsBackWall[i].GetCladdingSheetModel(options);
                sheetModel.Transform = listOfCladdingSheetsBackWall[i].GetTransformGroup(0, 0, rotationAboutZ);
                model_gr.Children.Add(sheetModel);
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

        // To Ondrej - toto som prevzal z CComboboxHelper.cs v projekte PFD. Tu na to nevidiet lebo BaseClasses su includovane v PFD.
        // Asi by sa mala vyvorit pomocna trieda pre farby v BaseClasses alebo este lepsie - zaviest kompletnu databazu farieb System.Windows.Media.Colors
        // a z nej vyberat

        public static List<Color> ColorList = new List<Color>() {
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
