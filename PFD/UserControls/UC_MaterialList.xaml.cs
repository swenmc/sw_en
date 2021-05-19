using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using BaseClasses;
using MATH;
using BaseClasses.Results;
using PFD.ViewModels;
using BaseClasses.GraphObj;

namespace PFD
{
    /// <summary>
    /// Interaction logic for MaterialList.xaml
    /// </summary>
    public partial class UC_MaterialList : UserControl
    {        
        double dBuildingMass = 0;
        double dBuildingNetPrice_WithoutMargin_WithoutGST = 0;
        CPFDViewModel _pfdVM;

        public UC_MaterialList(CPFDViewModel pfdVM)
        {
            _pfdVM = pfdVM;
            //DateTime start = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("UC_MaterialList");
            InitializeComponent();
            //System.Diagnostics.Trace.WriteLine("after InitializeComponent: " + (DateTime.Now - start).TotalMilliseconds);

            CMaterialListViewModel vm = new CMaterialListViewModel(pfdVM.Model);
            vm.PropertyChanged += MaterialListViewModel_PropertyChanged;
            this.DataContext = vm;
            //System.Diagnostics.Trace.WriteLine("after CMaterialListViewModel: " + (DateTime.Now - start).TotalMilliseconds) ;

            // Plates
            CreateTablePlates(pfdVM.Model);

            // Screws
            // Bolts
            // Anchors
            CreateTableConnectors(pfdVM.Model);

            // Cladding Sheets
            if (PartListHelper.DisplayCladdingTable(pfdVM))
                CreateTableCladdingSheets(pfdVM.Model);

            // Fibreglass Sheets
            if (PartListHelper.DisplayFibreglassTable(pfdVM))
                CreateTableFibreglassSheets(pfdVM.Model);

            // Cladding Accessories
            // IN WORK
            CreateTableCladdingAccessories(pfdVM.Model);

            SetControlsVisibility();
        }

        private void SetControlsVisibility()
        {
            if (PartListHelper.DisplayCladdingTable(_pfdVM)) 
            {
                TxtCladdingSheets.Visibility = Visibility.Visible;
                Datagrid_CladdingSheets.Visibility = Visibility.Visible;
            }
            else
            {
                TxtCladdingSheets.Visibility = Visibility.Collapsed;
                Datagrid_CladdingSheets.Visibility = Visibility.Collapsed;
            }

            if (PartListHelper.DisplayFibreglassTable(_pfdVM)) 
            {
                TxtFibreglassSheets.Visibility = Visibility.Visible;
                Datagrid_FibreglassSheets.Visibility = Visibility.Visible;
            }
            else
            {
                TxtFibreglassSheets.Visibility = Visibility.Collapsed;
                Datagrid_FibreglassSheets.Visibility = Visibility.Collapsed;
            }
        }

        private void MaterialListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void CreateTablePlates(CModel model)
        {
            DataSet ds = QuotationHelper.GetTablePlates(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds == null) return;

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_Plates.Loaded += Datagrid_Plates_Loaded;
        }

        private void Datagrid_Plates_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Plates);
        }

        private void CreateTableConnectors(CModel model)
        {
            DataSet ds = QuotationHelper.GetTableConnectors(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds == null) return;

            Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        }

        private void Datagrid_Connectors_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Connectors);
        }

        private void CreateTableCladdingSheets(CModel model)
        {
            DataSet ds = QuotationHelper.GetTableCladdingSheets(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds == null) return;

            Datagrid_CladdingSheets.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_CladdingSheets.Loaded += Datagrid_CladdingSheets_Loaded;
        }

        private void Datagrid_CladdingSheets_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_CladdingSheets);
        }

        private void CreateTableFibreglassSheets(CModel model)
        {
            DataSet ds = QuotationHelper.GetTableFibreglassSheets(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds == null) return;

            Datagrid_FibreglassSheets.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_FibreglassSheets.Loaded += Datagrid_FibreglassSheets_Loaded;
        }

        private void Datagrid_FibreglassSheets_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_FibreglassSheets);
        }

        private void SetLastRowBold(DataGrid datagrid)
        {
            DataGridRow dtrow = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(datagrid.Items.Count - 1);
            if (dtrow == null) return;
            Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);
            Style newStyle = new Style(dtrow.GetType());

            newStyle.Setters.Add(bold);
            dtrow.Style = newStyle;
        }

        private void CreateTableCladdingAccessories(CModel model)
        {
            // Cladding Accessories Item and Fixing - IN WORK
            List<CCladdingAccessories_Item> claddingAccessoriesItems = new List<CCladdingAccessories_Item>();

            // Tests
            DATABASE.DTO.CCladdingAccessories_Fixing_Properties fixingProp = new DATABASE.DTO.CCladdingAccessories_Fixing_Properties();
            fixingProp = DATABASE.CCladdingAccessoriesManager.GetFixingProperties("Galvanized cap assembly (for TEK screw 14gx115)");

            CCladdingAccessories_Item item;

            if (model.m_arrGOCladding[0] != null)
            {
                // 11 - Standard Roofing
                // Sposob A
                double ribWidth = _pfdVM._claddingOptionsVM.RoofCladdingProps.widthRib_m;
                double fixingPointTributaryArea = ribWidth * model.fDist_Purlin;

                double fRoofCladdingArea_WithoutFibreglass = 200; // Todo napojit
                int iNumberOfFixingPoints = (int)(fRoofCladdingArea_WithoutFibreglass / fixingPointTributaryArea);

                // Sposob B

                int iNumberOfFixingPoints2 = 0;
                if (model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight)
                    {
                        iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                    }
                }

                if (model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft)
                    {
                        iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                    }
                }

                item = new CCladdingAccessories_Item("TEK screw 14gx115(platic profile washer and galvanized cap)", iNumberOfFixingPoints);
                claddingAccessoriesItems.Add(item);






                // 12 - Fibreglass rooflites

                // Todo napojit
                // Zistit kolko FG sheets (resp ich sirku) konci na hrane alebo tesne pod hranou strechy pre gable roof (napriklad < 0.3 m
                // Podla suradnice y a hodnoty length v porovnani s length_left_basic (right side) a hodnoty y = 0 (left side)

                int iNumberOfFGSheetsRidge = 5; // Todo napojit
                double fTotalLengthFGSheetsRidge = 20.15; // Todo napojit

                // Todo pridat do DB capFlashingFibreglass_Ridge
                // Todo pridat do DB plasticBlokFibreglass_Ridge

                int iNumberOfSupportBracketBetweenPurlins;
                double supportBracketBetweenPurlinsLengthTotal = 0;
                int iNumberOfSupportBracketBetweenPurlinsFixingPoints = 0;

                if (model.fDist_Purlin <= 2.5)
                    iNumberOfSupportBracketBetweenPurlins = 0;
                if (model.fDist_Purlin <= 4.5)
                    iNumberOfSupportBracketBetweenPurlins = 1;
                else
                    iNumberOfSupportBracketBetweenPurlins = 2;

                double dLapFoamPacker_TotalLength = 0;

                // Sposob A
                fixingPointTributaryArea = ribWidth * model.fDist_Purlin;

                double fRoofCladdingAreaFibreglass = 22; // Todo napojit
                iNumberOfFixingPoints = (int)(fRoofCladdingAreaFibreglass / fixingPointTributaryArea);

                // Sposob B

                iNumberOfFixingPoints2 = 0;
                int iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, TODO doriesit ak su 2 fibreglass sheets vedla seba
                double dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

                if (model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofRight != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofRight)
                    {
                        iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                        iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                        int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenPurlins * ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1);
                        supportBracketBetweenPurlinsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                        iNumberOfSupportBracketBetweenPurlinsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 + 2); // Pridany jeden bod pre koncove rebro FG + 2 pre rebra cladding sheet
                        dLapFoamPacker_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                    }
                }

                if (model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofLeft != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofLeft)
                    {
                        iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                        iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                        int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenPurlins * ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1);
                        supportBracketBetweenPurlinsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                        iNumberOfSupportBracketBetweenPurlinsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 + 2); // Pridany jeden bod pre koncove rebro FG + 2 pre rebra cladding sheet
                        dLapFoamPacker_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                    }
                }

                // Crown roof fixing
                item = new CCladdingAccessories_Item("TEK screw 14gx115 (platic profile washer and galvanized cap)", iNumberOfFixingPoints);
                claddingAccessoriesItems.Add(item);

                // Lapstitch fixing
                item = new CCladdingAccessories_Item("Lapstitch with TEK screw 12gx20 (neo washer)", iNumberLapstitchFixingPoints);
                claddingAccessoriesItems.Add(item);

                // Protection strip
                double fLengthProtectionstrip = iNumberOfFixingPoints2 * ribWidth;

                // CAccessories_LengthItemProperties - asi by bolo dobre pouzit
                item = new CCladdingAccessories_Item("Fibreglass protection strip 80 mm wide", fLengthProtectionstrip);
                claddingAccessoriesItems.Add(item);

                // 13 - Rooflite support bracket

                // Support bracket
                item = new CCladdingAccessories_Item("Fibreglass support bracket 30x40x1400x1 mm", supportBracketBetweenPurlinsLengthTotal);
                claddingAccessoriesItems.Add(item);

                // Support bracket Fixing
                item = new CCladdingAccessories_Item("TEK screw 14gx115 (platic profile washer and galvanized cap)", iNumberOfSupportBracketBetweenPurlinsFixingPoints);
                claddingAccessoriesItems.Add(item);

                // 14 - Roofing lap
                // Todo pridat do DB 10 mm x 12 mm closed cell foam packer

                //dLapFoamPacker_TotalLength

                if (_pfdVM.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed) // Gable Roof Only
                {
                    // Roof ridge length
                    // Rovnake ako ridge flashing length

                    iNumberOfFixingPoints = 2 * ((int)(_pfdVM.RoofLength_Y / ribWidth) + 1);

                    bool bStandardRidge = true; // TODO - napojit - accessories flashings

                    if (bStandardRidge)
                    {
                        // 15 - Standard ridge

                        // Apex ridge flashing rivets
                        item = new CCladdingAccessories_Item("Apex ridge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                        claddingAccessoriesItems.Add(item);
                    }
                    else
                    {
                        // 16 - Infill Ridge

                        // TEK screws 14gx115
                        item = new CCladdingAccessories_Item("Apex ridge flashing TEK screw 14gx115  (neo washer)", iNumberOfFixingPoints);
                        claddingAccessoriesItems.Add(item);

                        // Plastic ridge blocks
                        item = new CCladdingAccessories_Item("Plastic ridge block", iNumberOfFixingPoints);
                        claddingAccessoriesItems.Add(item);

                        // TEK screws 12gx20
                        item = new CCladdingAccessories_Item("Ridge TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints);
                        claddingAccessoriesItems.Add(item);
                    }
                }

                // 17 - Barge

                double dBargeFlashing_TotalLength = 0;
                double dBargeflashingFixingSpacing = 0.3f; // DB
                int iNumberOfFixingPointsBirdProofFlashing = 0;
                double dFixingPointsBargeCladdingSheetEdge = 2; // DB
                int iNumberOfFixingPointsBargeCladdingSheetEdge = 0;

                double dGutter_TotalLength = 0;
                double dGutterBracketSpacing = 2 * _pfdVM._claddingOptionsVM.RoofCladdingProps.widthRib_m; // DB
                int iNumberOfGutterBrackets = 0;
                int iNumberOfGutterBracketFixingPoints = 0;
                int iNumberOfGutterFixingPoints = 0;
                double dEavePurlinBirdProofFixingPointSpacing = 1; // DB
                int iNumberEavePurlinBirdProofFixingPoints = 0;

                // TODO  // pridat CANOPIES ???? !!!!!!!!!!!!!!
                // Asi bude potrebne prechadzat zoznam canopies ...

                if (_pfdVM.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
                {
                    dBargeFlashing_TotalLength = 2 * _pfdVM.RoofSideLength;
                    iNumberOfFixingPoints = 2 * (2 * ((int)(_pfdVM.RoofSideLength / dBargeflashingFixingSpacing) + 1)); // Top and bottom
                    iNumberOfFixingPointsBirdProofFlashing = 2 * ((int)(_pfdVM.RoofSideLength / _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m) + 1);
                    iNumberOfFixingPointsBargeCladdingSheetEdge = Math.Min(2, 2 * ((int)(_pfdVM.RoofSideLength / dFixingPointsBargeCladdingSheetEdge) + 1));

                    dGutter_TotalLength = _pfdVM.RoofSideLength;
                    iNumberOfGutterBrackets = (int)(_pfdVM.RoofSideLength / dGutterBracketSpacing) + 1;
                    iNumberOfGutterBracketFixingPoints = 2 * iNumberOfGutterBrackets;

                    iNumberOfGutterFixingPoints = (int)(_pfdVM.RoofSideLength / _pfdVM._claddingOptionsVM.RoofCladdingProps.widthRib_m) + 1; // Each pan
                    iNumberOfGutterFixingPoints += iNumberOfGutterBrackets;

                    iNumberEavePurlinBirdProofFixingPoints = (int)(_pfdVM.RoofSideLength / dEavePurlinBirdProofFixingPointSpacing) + 1;
                }
                else if (_pfdVM.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
                {
                    dBargeFlashing_TotalLength = 4 * _pfdVM.RoofSideLength;
                    iNumberOfFixingPoints = 4 * (2 * ((int)(_pfdVM.RoofSideLength / dBargeflashingFixingSpacing) + 1)); // Top and bottom
                    iNumberOfFixingPointsBirdProofFlashing = 4 * ((int)(_pfdVM.RoofSideLength / _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m) + 1);
                    iNumberOfFixingPointsBargeCladdingSheetEdge = Math.Min(2, 4 * ((int)(_pfdVM.RoofSideLength / dFixingPointsBargeCladdingSheetEdge) + 1));

                    dGutter_TotalLength = 2 * _pfdVM.RoofSideLength;
                    iNumberOfGutterBrackets = 2 * ((int)(_pfdVM.RoofSideLength / dGutterBracketSpacing) + 1);
                    iNumberOfGutterBracketFixingPoints = 2 * iNumberOfGutterBrackets;

                    iNumberOfGutterFixingPoints = 2 * ((int)(_pfdVM.RoofSideLength / _pfdVM._claddingOptionsVM.RoofCladdingProps.widthRib_m) + 1); // Each pan
                    iNumberOfGutterFixingPoints += iNumberOfGutterBrackets;

                    iNumberEavePurlinBirdProofFixingPoints = 2 * ((int)(_pfdVM.RoofSideLength / dEavePurlinBirdProofFixingPointSpacing) + 1);
                }

                // TODO Bird proof flashing - pridat ku flashing

                // Barge flashing fixing - Rivets
                item = new CCladdingAccessories_Item("Barge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                claddingAccessoriesItems.Add(item);

                // Bird proof flashing fixing - Rivets
                item = new CCladdingAccessories_Item("Birdgproof flashing rivet 73AS6.4", iNumberOfFixingPointsBirdProofFlashing);
                claddingAccessoriesItems.Add(item);

                // Barge cladding sheet edge fixing - TEK screws 12gx42
                item = new CCladdingAccessories_Item("TEK screw 14gx42 (bonded washer)", iNumberOfFixingPointsBirdProofFlashing);
                claddingAccessoriesItems.Add(item);

                // 18 - Gutter

                // TODO Bird proof flashing - pridat ku flashing .... Eave purlin birdproof flashing

                // Eave purlin bird proof flashing fixing
                item = new CCladdingAccessories_Item("Birdproof strip wafer TEK 10g", iNumberEavePurlinBirdProofFixingPoints);
                claddingAccessoriesItems.Add(item);

                // Eave purlin bird proof plastic blocks
                item = new CCladdingAccessories_Item("Plastic gutter block", iNumberEavePurlinBirdProofFixingPoints);
                claddingAccessoriesItems.Add(item);

                // Gutter brackets
                item = new CCladdingAccessories_Item("Gutter bracket 300x26x15", iNumberOfGutterBrackets);
                claddingAccessoriesItems.Add(item);

                // Gutter bracket fixing
                item = new CCladdingAccessories_Item("Gutter TEK screw 12gx20(neo washer)", iNumberOfGutterBracketFixingPoints);
                claddingAccessoriesItems.Add(item);

                // Gutter fixing
                item = new CCladdingAccessories_Item("Gutter rivet 73AS6.4", iNumberOfGutterFixingPoints);
                claddingAccessoriesItems.Add(item);

                // 21 - Cladding

                int profileFactor = 1; // 1 - Smartdek, 2 - Purlindek and Speedclad

                double dCladdingSeamFixingSpacing = 0.6; // DB
                int iNumberCladdingSeamFixingPoints = 0;

                // Sposob A
                ribWidth = _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m;
                fixingPointTributaryArea = (ribWidth / (float)profileFactor) * model.fDist_Girt;

                double fWallCladdingArea_WithoutFibreglassAndOpenings = 200; // Todo napojit
                iNumberOfFixingPoints = (int)(fWallCladdingArea_WithoutFibreglassAndOpenings / fixingPointTributaryArea);

                // Sposob B

                iNumberOfFixingPoints2 = 0;

                if (model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall)
                    {
                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                        iNumberCladdingSeamFixingPoints += ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                    }
                }

                if (model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall)
                    {
                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                        iNumberCladdingSeamFixingPoints += ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                    }
                }

                if (model.m_arrGOCladding[0].listOfCladdingSheetsRightWall != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsRightWall)
                    {
                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                        iNumberCladdingSeamFixingPoints += ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                    }
                }

                if (model.m_arrGOCladding[0].listOfCladdingSheetsBackWall != null)
                {
                    foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsBackWall)
                    {
                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                        iNumberCladdingSeamFixingPoints += ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                    }
                }

                item = new CCladdingAccessories_Item("Smartdek TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints2);
                claddingAccessoriesItems.Add(item);

                // Fixing between wall cladding sheets
                item = new CCladdingAccessories_Item("Seam fix cladding rivet 73AS6.4", iNumberCladdingSeamFixingPoints);
                claddingAccessoriesItems.Add(item);

                double dBuildingPerimeter = 2 * (_pfdVM.LengthOverall + _pfdVM.WidthOverall);
                double dBuildingCladdingPerimeterWithoutDoors = dBuildingPerimeter; // Obvod budovy bez sirky dveri

                foreach (DoorProperties door in _pfdVM._doorsAndWindowsVM.DoorBlocksProperties)
                    dBuildingCladdingPerimeterWithoutDoors -= door.fDoorsWidth;

                // Damp proof course
                item = new CCladdingAccessories_Item("Damp proof course beneath angle", dBuildingCladdingPerimeterWithoutDoors); // TODO - prerobit na double
                claddingAccessoriesItems.Add(item);

                // Angle
                item = new CCladdingAccessories_Item("Angle 50x50x1 mm", dBuildingCladdingPerimeterWithoutDoors); // TODO - prerobit na double
                claddingAccessoriesItems.Add(item);

                // Foam bird proof strip
                item = new CCladdingAccessories_Item("Foam birdproof strip", dBuildingCladdingPerimeterWithoutDoors); // TODO - prerobit na double
                claddingAccessoriesItems.Add(item);

                // Angle fixing
                double dAngleFixingPointsSpacing = 0.6; // DB
                iNumberOfFixingPoints = (int)(dBuildingCladdingPerimeterWithoutDoors / dAngleFixingPointsSpacing) + 4; // 4 strany - len priblizne
                item = new CCladdingAccessories_Item("Angle suredrive concrete anchor 6.5x50", iNumberOfFixingPoints);
                claddingAccessoriesItems.Add(item);

                // 22 - Cladding corner
                double dCornerFlashingLength = 4 * _pfdVM.WallHeightOverall; // TODO napojit, zohladnit ktore steny su zapnute a ktore vypnute
                double dCornerFlashingFixingPointsSpacing = 0.3; // DB (kotvenie dvoch stran flashing)
                iNumberOfFixingPoints = 2 * ((int)(dCornerFlashingLength / dCornerFlashingFixingPointsSpacing) + 4); // 4 rohy - len priblizne

                item = new CCladdingAccessories_Item("Corner flashing rivet 73AS6.4", iNumberOfFixingPoints);
                claddingAccessoriesItems.Add(item);

                // 23 - Fibreglass wall lite



            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}