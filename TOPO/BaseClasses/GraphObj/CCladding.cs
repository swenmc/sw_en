using System;
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
        BuildingGeometryDataInput sBuildingGeomInputData;

        // TODO Ondrej
        double claddingThickness_Wall = 0.030; // Dopracovat napojenie z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m v tabulkach tableSections_m alebo trapezoidalSheeting_m
        double claddingThickness_Roof = 0.060; // Dopracovat napojenie z databazy cladding MDBTrapezoidalSheeting - vlastnost height_m

        double column_crsc_z_plus;
        double column_crsc_y_minus;
        double column_crsc_y_plus;

        public CCladding()
        {

        }

        // Constructor 2
        public CCladding(int iCladding_ID, BuildingGeometryDataInput sGeometryInputData, CRSC.CCrSc_TW columnSection, bool bIsDisplayed, int fTime)
        {
            ID = iCladding_ID;
            sBuildingGeomInputData = sGeometryInputData;
            column_crsc_z_plus = columnSection.z_max;
            column_crsc_y_minus = columnSection.y_min;
            column_crsc_y_plus = columnSection.y_max;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        public Model3DGroup GetCladdingModel(Color colorWall, Color colorRoof, bool bUseTextures = false)
        {
            m_pControlPoint = new Point3D(0, 0, 0);

            Model3DGroup model_gr = new Model3DGroup();

            DiffuseMaterial material_Wall = new DiffuseMaterial(new SolidColorBrush(colorWall)); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export
            DiffuseMaterial material_Roof = new DiffuseMaterial(new SolidColorBrush(colorRoof)); // TODO Ondrej - nastavitelna farba pre zobrazenie v GUI a pre Export

            if (bUseTextures)
            {
                var image = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Textures/wallTexture_PermanentGreen.jpg", UriKind.RelativeOrAbsolute)) };
                RenderOptions.SetCachingHint(image, CachingHint.Cache);
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                material_Wall = new DiffuseMaterial(new VisualBrush(image));

                image = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Textures/roofTexture_Gold.jpg", UriKind.RelativeOrAbsolute)) };
                material_Roof = new DiffuseMaterial(new VisualBrush(image));

                //materialGroup.Children.Add(material);
                //model.BackMaterial = material;
            }


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

            // Monopitch

            // Gable

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, column_crsc_y_minus, bottomEdge_z);
            Point3D pfront1_baseright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, column_crsc_y_minus, bottomEdge_z);
            Point3D pfront2_heightright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, column_crsc_y_minus, height_1_final);
            Point3D pfront3_heightleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, column_crsc_y_minus, height_1_final);
            Point3D pfront4_top = new Point3D(0.5 * sBuildingGeomInputData.fW, column_crsc_y_minus, height_2_final);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, bottomEdge_z);
            Point3D pback1_baseright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, bottomEdge_z);
            Point3D pback2_heightright = new Point3D(sBuildingGeomInputData.fW + column_crsc_z_plus + claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, height_1_final);
            Point3D pback3_heightleft = new Point3D(-column_crsc_z_plus - claddingThickness_Wall, sBuildingGeomInputData.fL + column_crsc_y_plus, height_1_final);
            Point3D pback4_top = new Point3D(0.5 * sBuildingGeomInputData.fW, sBuildingGeomInputData.fL + column_crsc_y_plus, height_2_final);

            // Front Wall
            if (bUseTextures)
            model_gr.Children.Add(new CAreaPolygonal(0, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_heightright, pfront4_top, pfront3_heightleft}, 0).CreateArea(bUseTextures, material_Wall));
            // Back Wall
            model_gr.Children.Add(new CAreaPolygonal(1, new List<Point3D>() { pback1_baseright, pback0_baseleft, pback3_heightleft, pback4_top, pback2_heightright}, 0).CreateArea(bUseTextures, material_Wall));
            // Left Wall
            model_gr.Children.Add(new CAreaPolygonal(2, new List<Point3D>() { pback0_baseleft, pfront0_baseleft, pfront3_heightleft, pback3_heightleft}, 0).CreateArea(bUseTextures, material_Wall));
            // Right Wall
            model_gr.Children.Add(new CAreaPolygonal(3, new List<Point3D>() { pfront1_baseright, pback1_baseright, pback2_heightright, pfront2_heightright}, 0).CreateArea(bUseTextures, material_Wall));
            // Roof - Left Side
            model_gr.Children.Add(new CAreaPolygonal(4, new List<Point3D>() { pback3_heightleft, pfront3_heightleft, pfront4_top, pback4_top}, 0).CreateArea(bUseTextures, material_Roof));
            // Roof - Right Side
            model_gr.Children.Add(new CAreaPolygonal(5, new List<Point3D>() { pfront2_heightright, pback2_heightright, pback4_top, pfront4_top}, 0).CreateArea(bUseTextures, material_Roof));

            return model_gr;
        }
    }
}
