﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CCladding:CEntity3D
    {
        EModelType_FS eModelType;
        BuildingGeometryDataInput sBuildingGeomInputData;

        // TODO Ondrej
        double claddingThickness_Wall = 0.030; // Dopracovat napojenie z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m v tabulkach tableSections_m alebo trapezoidalSheeting_m
        double claddingThickness_Roof = 0.060; // Dopracovat napojenie z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m

        double claddingWidthRibModular_Wall = 0.190; // m // Dopracovat napojenie z databazy cladding MDBTrapezoidalSheeting widthRib_m
        double claddingWidthRibModular_Roof = 0.300; // m // Dopracovat napojenie z databazy cladding MDBTrapezoidalSheeting widthRib_m

        double column_crsc_z_plus;
        double column_crsc_y_minus;
        double column_crsc_y_plus;

        Color m_ColorWall;
        Color m_ColorRoof;

        public CCladding()
        {

        }

        // Constructor 2
        public CCladding(int iCladding_ID, EModelType_FS modelType_FS, BuildingGeometryDataInput sGeometryInputData, CRSC.CCrSc_TW columnSection,
            Color colorWall, Color colorRoof, bool bIsDisplayed, int fTime)
        {
            ID = iCladding_ID;
            eModelType = modelType_FS;
            sBuildingGeomInputData = sGeometryInputData;
            column_crsc_z_plus = columnSection.z_max;
            column_crsc_y_minus = columnSection.y_min;
            column_crsc_y_plus = columnSection.y_max;
            m_ColorWall = colorWall;
            m_ColorRoof = colorRoof;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        public Model3DGroup GetCladdingModel(bool bUseTextures = false)
        {
            m_pControlPoint = new Point3D(0, 0, 0);

            Model3DGroup model_gr = new Model3DGroup();

            // Vytvorime model v GCS [0,0,0] je uvazovana v bode m_ControlPoint

            double bottomEdge_z = 0;

            double height_1_final = sBuildingGeomInputData.fH_1 + column_crsc_z_plus + claddingThickness_Roof; // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
            double height_2_final = sBuildingGeomInputData.fH_2 + column_crsc_z_plus + claddingThickness_Roof; // TODO - dopocitat presne, zohladnit edge purlin a sklon

            double additionalOffset = 0.010;  // 10 mm

            // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
            column_crsc_y_minus -= additionalOffset;
            column_crsc_y_plus += additionalOffset;
            column_crsc_z_plus += additionalOffset;

            // Cladding Edges

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, column_crsc_y_minus, bottomEdge_z);
            Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, column_crsc_y_minus, bottomEdge_z);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, bottomEdge_z);
            Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, bottomEdge_z);

            DiffuseMaterial material_SideWall = new DiffuseMaterial(new SolidColorBrush(m_ColorWall)); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_FrontBackWall = new DiffuseMaterial(new SolidColorBrush(m_ColorWall)); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_Roof = new DiffuseMaterial(new SolidColorBrush(m_ColorRoof)); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export

            if (bUseTextures)
            {
                // TODO Ondrej - potrebujeme nejako prepocitat faktor pre pomer absolutnej a relativnej velkosti
                // Nejako som to urobil prosim o kontrolu (pripadne dorobit alternativu pre absolutne zadavanie)

                // Obrazok textury ma a = 213.9 x 213.9 mm. velkost v pixeloch je c = 800 x 800 pxs, 3.74 pxs / mm
                // Povedzme ze osova vzialenost rebier plechu je b = 130 mm
                // Ak chceme na budove dlzky 10000 mm vykreslit 77 vln (textur), tak bude velkost strany Rect 130/10000
                // Ak chceme mat vlny (sekvencie textur) rovnako velke bez ohladu na rozmery musime pouzit absolutne vykreslovanie alebo pre kazdu plochu (ktora ma inak dlhu zakladnu pre kreslenie textury)
                // vypocitat specificky pomer

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Textures/wallTexture_PermanentGreen.jpg", UriKind.RelativeOrAbsolute));
                brush.TileMode = TileMode.Tile;
                brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                brush.Stretch = Stretch.UniformToFill;
                double rectSize = claddingWidthRibModular_Wall / (pfront1_baseright.X - pfront0_baseleft.X);
                brush.Viewport = new System.Windows.Rect(0, 0, rectSize, rectSize);
                material_FrontBackWall = new DiffuseMaterial(brush);

                brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Textures/wallTexture_PermanentGreen.jpg", UriKind.RelativeOrAbsolute));
                brush.TileMode = TileMode.Tile;
                brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                brush.Stretch = Stretch.UniformToFill;
                rectSize = claddingWidthRibModular_Wall / (pback0_baseleft.Y - pfront0_baseleft.Y);
                brush.Viewport = new System.Windows.Rect(0, 0, rectSize, rectSize);
                material_SideWall = new DiffuseMaterial(brush);

                brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Textures/roofTexture_Gold.jpg", UriKind.RelativeOrAbsolute));
                brush.TileMode = TileMode.Tile;
                brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                brush.Stretch = Stretch.UniformToFill;
                rectSize = claddingWidthRibModular_Roof / (pback0_baseleft.Y - pfront0_baseleft.Y);
                brush.Viewport = new System.Windows.Rect(0, 0, rectSize, rectSize); // Rozmer v smere Y pre side wall a strechu nemusi byt rovnaky ak sa pouzije iny plech
                material_Roof = new DiffuseMaterial(brush);
            }

            if (eModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                // Monopitch Roof

                Point3D pfront2_heightright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, column_crsc_y_minus, height_2_final);
                Point3D pfront3_heightleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, column_crsc_y_minus, height_1_final);

                Point3D pback2_heightright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, height_2_final);
                Point3D pback3_heightleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, height_1_final);

                if (bUseTextures)
                {
                    // Front Wall
                    model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_heightright, pfront3_heightleft }, 0).CreateArea(bUseTextures, material_FrontBackWall));
                    // Back Wall
                    model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pback3_heightleft, pback2_heightright }, 0).CreateArea(bUseTextures, material_FrontBackWall));
                    // Left Wall
                    model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pfront3_heightleft, pback3_heightleft }, 0).CreateArea(bUseTextures, material_SideWall));
                    // Right Wall
                    model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pback2_heightright, pfront2_heightright }, 0).CreateArea(bUseTextures, material_SideWall));
                    // Roof
                    model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pfront2_heightright, pback2_heightright, pback3_heightleft, pfront3_heightleft }, 0).CreateArea(bUseTextures, material_Roof));
                }
            }
            else if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Gable Roof

                Point3D pfront2_heightright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, column_crsc_y_minus, height_1_final);
                Point3D pfront3_heightleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, column_crsc_y_minus, height_1_final);
                Point3D pfront4_top = new Point3D(0.5 * sBuildingGeomInputData.fW, column_crsc_y_minus, height_2_final);

                Point3D pback2_heightright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, height_1_final);
                Point3D pback3_heightleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, height_1_final);
                Point3D pback4_top = new Point3D(0.5 * sBuildingGeomInputData.fW, sBuildingGeomInputData.fL + column_crsc_y_plus, height_2_final);

                if (bUseTextures)
                {
                    // Front Wall
                    model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_heightright, pfront4_top, pfront3_heightleft }, 0).CreateArea(bUseTextures, material_FrontBackWall));
                    // Back Wall
                    model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pback3_heightleft, pback4_top, pback2_heightright }, 0).CreateArea(bUseTextures, material_FrontBackWall));
                    // Left Wall
                    model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pfront3_heightleft, pback3_heightleft }, 0).CreateArea(bUseTextures, material_SideWall));
                    // Right Wall
                    model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pback2_heightright, pfront2_heightright }, 0).CreateArea(bUseTextures, material_SideWall));
                    // Roof - Left Side
                    model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pfront4_top, pback4_top, pback3_heightleft, pfront3_heightleft }, 0).CreateArea(bUseTextures, material_Roof));
                    // Roof - Right Side
                    model_gr.Children.Add(new CAreaPolygonal(5, new List<Point3D>() { pfront2_heightright, pback2_heightright, pback4_top, pfront4_top }, 0).CreateArea(bUseTextures, material_Roof));
                }
            }
            else
            {
                throw new Exception("Not implemented kitset type.");
            }

            return model_gr;
        }
    }
}
