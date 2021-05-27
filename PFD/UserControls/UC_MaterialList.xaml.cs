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

        float fTotalAreaOfOpennings = 0;
        float fRollerDoorTrimmerFlashing_TotalLength = 0;
        float fRollerDoorLintelFlashing_TotalLength = 0;
        float fRollerDoorLintelCapFlashing_TotalLength = 0;
        float fPADoorTrimmerFlashing_TotalLength = 0;
        float fPADoorLintelFlashing_TotalLength = 0;
        float fWindowFlashing_TotalLength = 0;

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
            if (PartListHelper.DisplayCladdingSheetsTable(pfdVM))
                CreateTableCladdingSheets(pfdVM.Model);

            // Fibreglass Sheets
            if (PartListHelper.DisplayFibreglassSheetsTable(pfdVM))
                CreateTableFibreglassSheets(pfdVM.Model);

            // Cladding            
            float fFibreGlassArea_Roof = pfdVM._claddingOptionsVM.FibreglassAreaRoofRatio / 100f * pfdVM.TotalRoofArea; // Priesvitna cast strechy (bez canopies)
            float fFibreGlassArea_Walls = pfdVM._claddingOptionsVM.FibreglassAreaWallRatio / 100f * pfdVM.TotalWallArea; // Priesvitna cast stien

            if (PartListHelper.DisplayCladdingTable(pfdVM)) //iba ak je nejaky cladding
            {
                CreateTableCladding(pfdVM, fTotalAreaOfOpennings, fFibreGlassArea_Walls, fFibreGlassArea_Roof);
            }
            else
            {
                TextBlock_Cladding.Visibility = Visibility.Collapsed;
                Datagrid_Cladding.Visibility = Visibility.Collapsed;
            }

            // FibreGlass
            if (PartListHelper.DisplayFibreglassTable(pfdVM))
                CreateTableFibreglass(pfdVM, fFibreGlassArea_Roof, fFibreGlassArea_Walls);
            else
            {
                TextBlock_Fibreglass.Visibility = Visibility.Collapsed;
                Datagrid_Fibreglass.Visibility = Visibility.Collapsed;
            }


            // Cladding Accessories
            // IN WORK
            //CreateTableCladdingAccessories(pfdVM.Model);


            // Roof Netting
            if (PartListHelper.DisplayRoofNettingTable(pfdVM)) CreateTableRoofNetting(pfdVM.TotalRoofAreaInclCanopies);
            else
            {
                TextBlock_RoofNetting.Visibility = Visibility.Collapsed;
                Datagrid_RoofNetting.Visibility = Visibility.Collapsed;
            }

            // Doors and windows
            CreateTableDoorsAndWindows(pfdVM);            

            // Gutters
            if (PartListHelper.DisplayGuttersTable(pfdVM)) CreateTableGutters(pfdVM);
            else
            {
                TextBlock_Gutters.Visibility = Visibility.Collapsed;
                Datagrid_Gutters.Visibility = Visibility.Collapsed;
            }

            // Downpipes
            if (PartListHelper.DisplayDownpipesTable(pfdVM)) CreateTableDownpipes(pfdVM);
            else
            {
                TextBlock_Downpipes.Visibility = Visibility.Collapsed;
                Datagrid_Downpipes.Visibility = Visibility.Collapsed;
            }

            // Flashing
            if (PartListHelper.DisplayFlashingsTable(pfdVM))
            {
                CreateTableFlashing(
                    fRollerDoorTrimmerFlashing_TotalLength,
                    fRollerDoorLintelFlashing_TotalLength,
                    fRollerDoorLintelCapFlashing_TotalLength,
                    fPADoorTrimmerFlashing_TotalLength,
                    fPADoorLintelFlashing_TotalLength,
                    fWindowFlashing_TotalLength);
            }
            else
            {
                TextBlock_Flashing.Visibility = Visibility.Collapsed;
                Datagrid_Flashing.Visibility = Visibility.Collapsed;
            }

            // Packers
            if (PartListHelper.DisplayPackersTable(pfdVM))
            {
                CreateTablePackers(fRollerDoorLintelFlashing_TotalLength);
            }
            else
            {
                TextBlock_Packers.Visibility = Visibility.Collapsed;
                Datagrid_Packers.Visibility = Visibility.Collapsed;
            }

            SetControlsVisibility();
        }

        private void SetControlsVisibility()
        {
            if (PartListHelper.DisplayCladdingSheetsTable(_pfdVM)) 
            {
                TxtCladdingSheets.Visibility = Visibility.Visible;
                Datagrid_CladdingSheets.Visibility = Visibility.Visible;
            }
            else
            {
                TxtCladdingSheets.Visibility = Visibility.Collapsed;
                Datagrid_CladdingSheets.Visibility = Visibility.Collapsed;
            }

            if (PartListHelper.DisplayFibreglassSheetsTable(_pfdVM)) 
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

        private void CreateTableCladding(CPFDViewModel vm, float fTotalAreaOfOpennings, float fFibreGlassArea_Walls, float fFibreGlassArea_Roof)
        {
            DataSet ds = QuotationHelper.GetTableCladding(vm, fTotalAreaOfOpennings, fFibreGlassArea_Walls, fFibreGlassArea_Roof, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Cladding.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Cladding.Loaded += Datagrid_Cladding_Loaded;
        }

        private void Datagrid_Cladding_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Cladding);
        }

        private void CreateTableFibreglass(CPFDViewModel vm, float fFibreGlassArea_Roof, float fFibreGlassArea_Walls)
        {
            DataSet ds = QuotationHelper.GetTableFibreglass(vm, fFibreGlassArea_Roof, fFibreGlassArea_Walls, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            if (ds != null)
            {
                Datagrid_Fibreglass.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_Fibreglass.Loaded += Datagrid_Fibreglass_Loaded;
            }
            else // Tabulka je prazdna - nezobrazime ju
            {
                TextBlock_Fibreglass.IsEnabled = false;
                TextBlock_Fibreglass.Visibility = Visibility.Collapsed;

                Datagrid_Fibreglass.IsEnabled = false;
                Datagrid_Fibreglass.Visibility = Visibility.Collapsed;
            }
        }
        private void Datagrid_Fibreglass_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Fibreglass);
        }


        private void CreateTableRoofNetting(float fRoofArea)
        {
            DataSet ds = QuotationHelper.GetTableRoofNetting(fRoofArea, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_RoofNetting.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_RoofNetting.Loaded += Datagrid_RoofNetting_Loaded;
        }
        private void Datagrid_RoofNetting_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_RoofNetting);
        }

        private void CreateTableDoorsAndWindows(CPFDViewModel vm)
        {
            DataSet ds = QuotationHelper.GetTableDoorsAndWindows(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST, out fTotalAreaOfOpennings,
                out fRollerDoorTrimmerFlashing_TotalLength, out fRollerDoorLintelFlashing_TotalLength, out fRollerDoorLintelCapFlashing_TotalLength,
                out fPADoorTrimmerFlashing_TotalLength, out fPADoorLintelFlashing_TotalLength, out fWindowFlashing_TotalLength);

            if (ds != null)
            {
                Datagrid_DoorsAndWindows.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_DoorsAndWindows.Loaded += Datagrid_DoorsAndWindows_Loaded;
            }
            else // Tabulka je prazdna - nezobrazime ju
            {
                TextBlock_DoorsAndWindows.IsEnabled = false;
                TextBlock_DoorsAndWindows.Visibility = Visibility.Collapsed;

                Datagrid_DoorsAndWindows.IsEnabled = false;
                Datagrid_DoorsAndWindows.Visibility = Visibility.Collapsed;
            }
        }
        private void Datagrid_DoorsAndWindows_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_DoorsAndWindows);
        }

        private void CreateTableGutters(CPFDViewModel vm)
        {
            DataSet ds = QuotationHelper.GetTableGutters(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Gutters.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Gutters.Loaded += Datagrid_Gutters_Loaded;
        }
        private void Datagrid_Gutters_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Gutters);
        }

        private void CreateTableDownpipes(CPFDViewModel vm)
        {
            DataSet ds = QuotationHelper.GetTableDownpipes(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Downpipes.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Downpipes.Loaded += Datagrid_Downpipes_Loaded;
        }
        private void Datagrid_Downpipes_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Downpipes);
        }
        private void CreateTableFlashing(
            float fRollerDoorTrimmerFlashing_TotalLength,
            float fRollerDoorLintelFlashing_TotalLength,
            float fRollerDoorLintelCapFlashing_TotalLength,
            float fPADoorTrimmerFlashing_TotalLength,
            float fPADoorLintelFlashing_TotalLength,
            float fWindowFlashing_TotalLength)
        {
            DataSet ds = QuotationHelper.GetTableFlashing(_pfdVM, fRollerDoorTrimmerFlashing_TotalLength, fRollerDoorLintelFlashing_TotalLength, fRollerDoorLintelCapFlashing_TotalLength,
                fPADoorTrimmerFlashing_TotalLength, fPADoorLintelFlashing_TotalLength, fWindowFlashing_TotalLength,
                ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Flashing.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Flashing.Loaded += Datagrid_Flashing_Loaded;
        }

        private void Datagrid_Flashing_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Flashing);
        }

        private void CreateTablePackers(float fRollerDoorLintelFlashing_TotalLength)
        {
            DataSet ds = QuotationHelper.GetTablePackers(fRollerDoorLintelFlashing_TotalLength, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Packers.ItemsSource = ds.Tables[0].AsDataView();
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
            List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece = new List<CCladdingAccessories_Item_Piece>();
            List<CCladdingAccessories_Item_Length> claddingAccessoriesItems_Length = new List<CCladdingAccessories_Item_Length>();

            // Tests
            DATABASE.DTO.CCladdingAccessories_Fixing_Properties fixingProp = new DATABASE.DTO.CCladdingAccessories_Fixing_Properties();
            fixingProp = DATABASE.CCladdingAccessoriesManager.GetFixingProperties("Galvanized cap assembly (for TEK screw 14gx115)");

            CCladdingAccessories_Item_Piece itemPiece;
            CCladdingAccessories_Item_Length itemLength;

            if (model.m_arrGOCladding != null && model.m_arrGOCladding.Count > 0)
            {
                int iNumberOfFixingPoints = 0;
                double fixingPointTributaryArea = 0;

                int iNumberOfFixingPoints2 = 0;

                int iNumberLapstitchFixingPoints = 0;
                double dLapstitchFixingPointsSpacing = 0;

                if (model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight != null && model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight.Count > 0)
                {
                    // 11 - Standard Roofing
                    // Sposob A
                    double ribWidthRoof = _pfdVM._claddingOptionsVM.RoofCladdingProps.widthRib_m;
                    fixingPointTributaryArea = ribWidthRoof * model.fDist_Purlin;

                    double fRoofCladdingAreaFibreglass = _pfdVM._claddingOptionsVM.FibreglassAreaRoofRatio / 100 * _pfdVM.TotalRoofArea; // Todo skontrolovat
                    double fRoofCladdingArea_WithoutFibreglass = _pfdVM.TotalRoofAreaInclCanopies - fRoofCladdingAreaFibreglass; // Todo skontrolovat

                    iNumberOfFixingPoints = (int)(fRoofCladdingArea_WithoutFibreglass / fixingPointTributaryArea);

                    // Pridavok
                    iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 1.1478f); // Navysime pocet o rezervu

                    // Sposob B

                    if (model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight != null)
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight)
                        {
                            iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                        }
                    }

                    if (model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft != null)
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft)
                        {
                            iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                        }
                    }

                    // Kontrola - priblizna
                    if(!MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 15))
                    {
                        // Exception
                        throw new Exception("Algorithm error. Different count of items!");
                    }

                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfFixingPoints2, "Roof Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    if ((model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofRight != null && model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofRight.Count > 0) ||
                        (model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofLeft != null && model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofLeft.Count > 0))
                    {
                        // 12 - Fibreglass rooflites

                        // Todo napojit
                        // Zistit kolko FG sheets (resp ich sirku) konci na hrane alebo tesne pod hranou strechy pre gable roof (napriklad < 0.3 m
                        // Podla suradnice y a hodnoty length v porovnani s length_left_basic (right side) a hodnoty y = 0 (left side)

                        int iNumberOfFGSheetsRidge = 5; // Todo napojit
                        double dTotalLengthFGSheetsRidge = 20.15; // Todo napojit

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
                        fixingPointTributaryArea = ribWidthRoof * model.fDist_Purlin;

                        iNumberOfFixingPoints = (int)(fRoofCladdingAreaFibreglass / fixingPointTributaryArea);

                        // Sposob B

                        iNumberOfFixingPoints2 = 0;
                        iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, TODO doriesit ak su 2 fibreglass sheets vedla seba
                        dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

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
                        itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfFixingPoints, "Roof Fibreglass");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Lapstitch fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("Lapstitch with TEK screw 12gx20 (neo washer)", iNumberLapstitchFixingPoints, "Roof Fibreglass");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Protection strip
                        double fLengthProtectionstrip = iNumberOfFixingPoints2 * ribWidthRoof;

                        // CAccessories_LengthItemProperties - asi by bolo dobre pouzit
                        itemLength = new CCladdingAccessories_Item_Length("Fibreglass protection strip 80 mm wide", fLengthProtectionstrip);
                        claddingAccessoriesItems_Length.Add(itemLength);

                        // Plastic blocks - Ridge - Fibreglass edge cap
                        int iNumberOfRidgePlasticBlocks = (int)(dTotalLengthFGSheetsRidge / ribWidthRoof);
                        itemPiece = new CCladdingAccessories_Item_Piece("Plastic ridge block - fibreglass", iNumberOfRidgePlasticBlocks);
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // 13 - Rooflite support bracket

                        // Support bracket
                        itemPiece = new CCladdingAccessories_Item_Piece("Fibreglass support bracket 30x40x1400-1 mm", (int)(supportBracketBetweenPurlinsLengthTotal / 1.4f) + 1, "Roof Fibreglass Support Bracket");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Support bracket fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfSupportBracketBetweenPurlinsFixingPoints, "Roof Fibreglass Support Bracket");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // 14 - Roofing lap

                        // Continuous closed cell foam packer
                        itemLength = new CCladdingAccessories_Item_Length("Continuous closed cell foam packer 10x12 mm", dLapFoamPacker_TotalLength/*, "Roof Fibreglass"*/);
                        claddingAccessoriesItems_Length.Add(itemLength);

                        if (_pfdVM.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed) // Gable Roof Only
                        {
                            // Roof ridge length
                            // Rovnake ako ridge flashing length

                            iNumberOfFixingPoints = 2 * ((int)(_pfdVM.RoofLength_Y / ribWidthRoof) + 1);

                            bool bStandardRidge = true; // TODO - napojit - accessories flashings

                            if (bStandardRidge)
                            {
                                // 15 - Standard ridge

                                // Apex ridge flashing rivets
                                itemPiece = new CCladdingAccessories_Item_Piece("Apex ridge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                                claddingAccessoriesItems_Piece.Add(itemPiece);
                            }
                            else
                            {
                                // 16 - Infill Ridge

                                // TEK screws 14gx115
                                itemPiece = new CCladdingAccessories_Item_Piece("Apex ridge flashing TEK screw 14gx115  (neo washer)", iNumberOfFixingPoints);
                                claddingAccessoriesItems_Piece.Add(itemPiece);

                                // Plastic ridge blocks
                                itemPiece = new CCladdingAccessories_Item_Piece("Plastic ridge block", iNumberOfFixingPoints);
                                claddingAccessoriesItems_Piece.Add(itemPiece);

                                // TEK screws 12gx20
                                itemPiece = new CCladdingAccessories_Item_Piece("Ridge TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints);
                                claddingAccessoriesItems_Piece.Add(itemPiece);
                            }

                            // TODO
                            // Apex brace - malo by to byt samostatne pri plates
                            double dApexBraceSpacing = 1; // 1 m???

                            int iNumberOfRidgeApexBracePoints = (int)(_pfdVM.Length / dApexBraceSpacing) + 1;
                            int iNumberOfRidgeApexBracePlates = 2 * iNumberOfRidgeApexBracePoints;
                            iNumberOfFixingPoints = 4 * iNumberOfRidgeApexBracePlates; // 4 TEK screws per brace

                            // Apex brace
                            itemPiece = new CCladdingAccessories_Item_Piece("Apex brace Angle L30/1-650", iNumberOfRidgeApexBracePoints);
                            claddingAccessoriesItems_Piece.Add(itemPiece);

                            // Apex brace TEK screws 10g
                            itemPiece = new CCladdingAccessories_Item_Piece("Apex brace wafer TEK screw 10g", iNumberOfFixingPoints);
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }
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

                // TODO - dopracovat podmienky
                // Pouzit ak su front a back wall

                // Barge flashing fixing - Rivets
                itemPiece = new CCladdingAccessories_Item_Piece("Barge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // TODO - dopracovat podmienky
                // Pouzit ak su front a back wall
                // Bird proof flashing fixing - Rivets
                itemPiece = new CCladdingAccessories_Item_Piece("Birdgproof flashing rivet 73AS6.4", iNumberOfFixingPointsBirdProofFlashing, "Barge");
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // Barge cladding sheet edge fixing - TEK screws 12gx42
                itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx42 (bonded washer)", iNumberOfFixingPointsBirdProofFlashing, "Barge");
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // 18 - Gutter

                // Eave purlin bird proof flashing fixing
                itemPiece = new CCladdingAccessories_Item_Piece("Birdproof strip wafer TEK screw 10g", iNumberEavePurlinBirdProofFixingPoints, "Eave purlin");
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // Eave purlin bird proof plastic blocks
                itemPiece = new CCladdingAccessories_Item_Piece("Plastic gutter block", iNumberEavePurlinBirdProofFixingPoints, "Eave purlin");
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // Gutter brackets
                itemPiece = new CCladdingAccessories_Item_Piece("Gutter bracket 300x26x15 mm", iNumberOfGutterBrackets);
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // Gutter bracket fixing
                itemPiece = new CCladdingAccessories_Item_Piece("Gutter TEK screw 12gx20(neo washer)", iNumberOfGutterBracketFixingPoints);
                claddingAccessoriesItems_Piece.Add(itemPiece);

                // Gutter fixing
                itemPiece = new CCladdingAccessories_Item_Piece("Gutter rivet 73AS6.4", iNumberOfGutterFixingPoints);
                claddingAccessoriesItems_Piece.Add(itemPiece);

                if ((model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall != null && model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall.Count > 0) ||
                    (model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall != null && model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall.Count > 0) ||
                    (model.m_arrGOCladding[0].listOfCladdingSheetsRightWall != null && model.m_arrGOCladding[0].listOfCladdingSheetsRightWall.Count > 0) ||
                    (model.m_arrGOCladding[0].listOfCladdingSheetsBackWall != null && model.m_arrGOCladding[0].listOfCladdingSheetsBackWall.Count > 0))
                {
                    // Openings
                    // Urcime zakladne parametre, mohli by sem uz prist pripravene

                    double dBuildingPerimeter = 2 * (_pfdVM.LengthOverall + _pfdVM.WidthOverall);
                    double dBuildingCladdingPerimeterWithoutDoors = dBuildingPerimeter; // Obvod budovy bez sirky dveri

                    // Wall Fibreglass Area
                    double fWallCladdingAreaFibreglass = _pfdVM._claddingOptionsVM.FibreglassAreaWallRatio / 100 * _pfdVM.TotalWallArea; // Todo skontrolovat

                    double dRollerDoorTrimmerLengh = 0;
                    double dRollerDoorHeaderLengh = 0;
                    int iNumberOfRollerDoorTrimmers = 0; // Trimmers or extension plates

                    double dPADoorHeaderLengh = 0;

                    bool bAnyRollerDoorExists = false;
                    bool bAnyPADoorExists = false;

                    // Wall Doors and Windows Area
                    double dDoorsAndWindowsOpeningArea = 0; // !!!! Tu su dvere a okna zo vsetkych stien, je potrebne doriesit ak sa niektora stena nerata, aby sa neuvazovali doors a windows z danej steny

                    foreach (DoorProperties door in _pfdVM._doorsAndWindowsVM.DoorBlocksProperties)
                    {
                        dBuildingCladdingPerimeterWithoutDoors -= door.fDoorsWidth;

                        if (door.sDoorType == "Roller Door")
                        {
                            dRollerDoorTrimmerLengh += door.fDoorsHeight * 2;
                            dRollerDoorHeaderLengh += door.fDoorsWidth;
                            iNumberOfRollerDoorTrimmers += 2;

                            bAnyRollerDoorExists = true;
                        }
                        else
                        {
                            dPADoorHeaderLengh += door.fDoorsWidth;

                            bAnyPADoorExists = true;
                        }

                        dDoorsAndWindowsOpeningArea += door.fDoorsWidth * door.fDoorsHeight;
                    }

                    foreach (WindowProperties window in _pfdVM._doorsAndWindowsVM.WindowBlocksProperties)
                    {
                        dDoorsAndWindowsOpeningArea += window.fWindowsWidth * window.fWindowsHeight;
                    }

                    // 21 - Cladding

                    int profileFactor = 1; // 1 - Smartdek, 2 - Purlindek and Speedclad

                    double dCladdingSeamFixingSpacing = 0.6; // DB
                    int iNumberCladdingSeamFixingPoints = 0;

                    // Sposob A
                    double ribWidthWall = _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m;
                    fixingPointTributaryArea = (ribWidthWall / (float)profileFactor) * model.fDist_Girt;

                    double fWallCladdingArea_WithoutFibreglassAndOpenings = _pfdVM.TotalWallArea - fWallCladdingAreaFibreglass - dDoorsAndWindowsOpeningArea; // Todo skontrolovat ak nie su aktivne vsetky steny !!!
                    iNumberOfFixingPoints = (int)(fWallCladdingArea_WithoutFibreglassAndOpenings / fixingPointTributaryArea);

                    // Pridavok
                    iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 1.2645f); // Navysime pocet o rezervu

                    // Sposob B

                    iNumberOfFixingPoints2 = 0;

                    // TO Ondrej - mam podozrenie ze tieto zoznamy obsahuju sheet pred nadelenim !!!!!!!
                    // TODO Ondrej - potrebujeme zaistit aby to sem voslo az ked je vsetko nadelene !!!!!

                    if (model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall != null)
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall)
                        {
                            if (!_pfdVM._modelOptionsVM.IndividualCladdingSheets)
                                iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet

                            iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                            iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                        }
                    }

                    if (model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall != null)
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall)
                        {
                            double dSheetLength = sheet.LengthTotal_Real;
                            if (!_pfdVM._modelOptionsVM.IndividualCladdingSheets)
                            {
                                if (sheet.NumberOfEdges == 5)
                                    dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopTip_Real);
                                else
                                    dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopRight_Real);

                                iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet
                            }

                            iNumberOfFixingPoints2 += profileFactor * ((int)(dSheetLength / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                            iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(dSheetLength / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                        }
                    }

                    if (model.m_arrGOCladding[0].listOfCladdingSheetsRightWall != null)
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsRightWall)
                        {
                            if (!_pfdVM._modelOptionsVM.IndividualCladdingSheets)
                                iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet

                            iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                            iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing)/* + 1*/); // One sheet side only
                        }
                    }

                    if (model.m_arrGOCladding[0].listOfCladdingSheetsBackWall != null)
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfCladdingSheetsBackWall)
                        {
                            double dSheetLength = sheet.LengthTotal_Real;
                            if (!_pfdVM._modelOptionsVM.IndividualCladdingSheets)
                            {
                                if (sheet.NumberOfEdges == 5)
                                    dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopTip_Real);
                                else
                                    dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopRight_Real);

                                iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet
                            }

                            iNumberOfFixingPoints2 += profileFactor * ((int)(dSheetLength / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                            iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(dSheetLength / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                        }
                    }

                    // Kontrola - priblizna
                    if (!MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 15))
                    {
                        // Exception
                        throw new Exception("Algorithm error. Different count of items!");
                    }

                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints2, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Fixing between wall cladding sheets
                    itemPiece = new CCladdingAccessories_Item_Piece("Seam fix cladding rivet 73AS6.4", iNumberCladdingSeamFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Damp proof course
                    itemLength = new CCladdingAccessories_Item_Length("Damp proof course beneath angle", dBuildingCladdingPerimeterWithoutDoors/*, "Wall Cladding"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    // Angle
                    itemLength = new CCladdingAccessories_Item_Length("Angle 50x50x1 mm", dBuildingCladdingPerimeterWithoutDoors/*, "Wall Cladding"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    // Foam bird proof strip
                    itemLength = new CCladdingAccessories_Item_Length("Foam birdproof strip", dBuildingCladdingPerimeterWithoutDoors/*, "Wall Cladding"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    // Angle fixing
                    double dAngleFixingPointsSpacing = 0.6; // DB
                    iNumberOfFixingPoints = (int)(dBuildingCladdingPerimeterWithoutDoors / dAngleFixingPointsSpacing) + 4; // 4 strany - len priblizne
                    itemPiece = new CCladdingAccessories_Item_Piece("Angle suredrive concrete anchor 6.5x50", iNumberOfFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // 22 - Cladding corner
                    double dCornerFlashingLength = 4 * _pfdVM.WallHeightOverall; // TODO napojit, zohladnit ktore steny su zapnute a ktore vypnute
                    double dCornerFlashingFixingPointsSpacing = 0.3; // DB (kotvenie dvoch stran flashing)
                    iNumberOfFixingPoints = 2 * ((int)(dCornerFlashingLength / dCornerFlashingFixingPointsSpacing) + 4); // 4 rohy - len priblizne

                    itemPiece = new CCladdingAccessories_Item_Piece("Corner flashing rivet 73AS6.4", iNumberOfFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    if ((model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft != null && model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft.Count > 0) ||
                        (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront != null && model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront.Count > 0) ||
                        (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight != null && model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight.Count > 0) ||
                        (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack != null && model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack.Count > 0))
                    {
                        // 23 - Fibreglass walllite

                        int iNumberOfSupportBracketBetweenGirts;
                        double supportBracketBetweenGirtsLengthTotal = 0;
                        int iNumberOfSupportBracketBetweenGirtsFixingPoints = 0;
                        int iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints = 0; // 12gx20 - 4 pcs per bracket

                        if (model.fDist_Purlin <= 1.8)
                            iNumberOfSupportBracketBetweenGirts = 0;
                        if (model.fDist_Purlin <= 5.4)
                            iNumberOfSupportBracketBetweenGirts = 1;
                        else
                            iNumberOfSupportBracketBetweenGirts = 2;

                        double dLapSealantBead_TotalLength = 0;

                        // Sposob A
                        ribWidthWall = _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m;
                        fixingPointTributaryArea = (ribWidthWall / (float)profileFactor) * model.fDist_Girt;

                        iNumberOfFixingPoints = (int)(fWallCladdingAreaFibreglass / fixingPointTributaryArea);

                        // Sposob B

                        iNumberOfFixingPoints2 = 0;
                        iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, TODO doriesit ak su 2 fibreglass sheets vedla seba
                        dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

                        if (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft != null)
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft)
                            {
                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1);
                                iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                            }
                        }

                        if (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront != null)
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront)
                            {
                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1);
                                iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                            }
                        }

                        if (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight != null)
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight)
                            {
                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1);
                                iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                            }
                        }

                        if (model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack != null)
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack)
                            {
                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / model.fDist_Girt) + 1);
                                iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                            }
                        }

                        // Pan fibreglass sheet fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo and bonded washer)", iNumberOfFixingPoints2, "Wall Fibreglass");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Lapstitch fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("Lap stitching TEK screw 12gx20 (neo washer)", iNumberLapstitchFixingPoints, "Wall Fibreglass");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Support bracket
                        itemPiece = new CCladdingAccessories_Item_Piece("U bracket 40x30x1400 - 1 mm", (int)(supportBracketBetweenGirtsLengthTotal / 1.4) + 1, "Wall Fibreglass Support Bracket");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Support bracket fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo and bonded washer)", iNumberOfSupportBracketBetweenGirtsFixingPoints, "Wall Fibreglass Support Bracket");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Support bracket fixing to cladding
                        itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo washer)", iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints, "Wall Fibreglass Support Bracket");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // 24 - Cladding lap

                        // Silicone sealant bead
                        itemLength = new CCladdingAccessories_Item_Length("Silicone sealant bead", dLapSealantBead_TotalLength);
                        claddingAccessoriesItems_Length.Add(itemLength);
                    }

                    if (bAnyRollerDoorExists)
                    {
                        // 26 - Roller door trim

                        // TODO Header packer - pridat ku packer list // Vytvorit novu tabulku

                        // Roller door trim flashing fixing

                        double dRollerDoorflashingFixingSpacing = 0.3f; // DB
                        iNumberOfFixingPoints = 2 * (int)(dRollerDoorTrimmerLengh / dRollerDoorflashingFixingSpacing); // 2 sides resp. top and bottom
                        iNumberOfFixingPoints += 5 * (int)(dRollerDoorHeaderLengh / dRollerDoorflashingFixingSpacing);

                        itemPiece = new CCladdingAccessories_Item_Piece("Flashing rivet 73AS6.4", iNumberOfFixingPoints, "Roller Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // 27 - Roller door mounting
                        // Roller door extension plate
                        itemPiece = new CCladdingAccessories_Item_Piece("Roller door extension plate", iNumberOfRollerDoorTrimmers, "Roller Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Roller door extension plate fixing
                        int iNumberOfFixingPointsPerPlate = 2 * 6; // DB
                        iNumberOfFixingPoints = iNumberOfRollerDoorTrimmers * iNumberOfFixingPointsPerPlate;
                        itemPiece = new CCladdingAccessories_Item_Piece("Roller door extension plate TEK screw 14gx22", iNumberOfFixingPoints, "Roller Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }

                    if (bAnyPADoorExists)
                    {
                        // 29 - PA door trim
                        // PA door header cap flashing fixing

                        double dPADoorflashingFixingSpacing = 0.3f; // DB
                        iNumberOfFixingPoints = 2 * (int)(dPADoorHeaderLengh / dPADoorflashingFixingSpacing);
                        itemPiece = new CCladdingAccessories_Item_Piece("Flashing rivet 73AS6.4", iNumberOfFixingPoints, "Personnel Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }
                }

                // TODO - zobrazit zoznamy items v tabulkach

                // Items - Length
                DataSet ds = GetTableCladdingAccessories_Items_Length(claddingAccessoriesItems_Length, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
                if (ds == null) return;

                Datagrid_CladdingAccessories_Items_Length.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
                Datagrid_CladdingAccessories_Items_Length.Loaded += Datagrid_CladdingAccessories_Items_Length_Loaded;

                // Items - Piece
                ds = null;
                ds = GetTableCladdingAccessories_Items_Piece(claddingAccessoriesItems_Piece, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
                if (ds == null) return;

                Datagrid_CladdingAccessories_Items_Piece.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
                Datagrid_CladdingAccessories_Items_Piece.Loaded += Datagrid_CladdingAccessories_Items_Piece_Loaded;
            }
        }

        public static DataSet GetTableCladdingAccessories_Items_Length(List<CCladdingAccessories_Item_Length> claddingAccessoriesItems_Length, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            // IN WORK - rozpracovane

            // TODO - Dopracovat
            double dTotalItemsMass_Table = 0;
            double dTotalItemsPrice_Table = 0;

            List<QuotationItem> quotation = new List<QuotationItem>(); // TODO Docanse - upravit 
            foreach(CCladdingAccessories_Item_Length item in claddingAccessoriesItems_Length)
            {
                QuotationItem qitem = new QuotationItem();

                qitem.Name = item.Name;
                qitem.Length = (float)item.m_length;

                quotation.Add(qitem);

                //dTotalItemsMass_Table += item.Mass;
                //dTotalItemsPrice_Table += item.Price_NZD;
            }

            dBuildingMass += dTotalItemsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalItemsPrice_Table;

            // Create Table
            DataTable table = new DataTable("CladdingAccessories_Items_Length");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Name.ColumnName, QuotationHelper.colProp_Name.DataType);
            table.Columns.Add(QuotationHelper.colProp_Length_m.ColumnName, QuotationHelper.colProp_Length_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Name.ColumnName] = item.Name;
                    row[QuotationHelper.colProp_Length_m.ColumnName] = item.Length.ToString("F2");
                    row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = 0; // TODO //item.UnitMassLength.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = 0; // TODO //item.UnitPrice_Length_NZD.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Name.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Length_m.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalItemsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalItemsPrice_Table.ToString("F2");
            table.Rows.Add(row);

            return ds;
        }

        public static DataSet GetTableCladdingAccessories_Items_Piece(List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            // IN WORK - rozpracovane

            // TODO - Dopracovat
            double dTotalItemsMass_Table = 0;
            double dTotalItemsPrice_Table = 0;
            int iTotalItemsNumber_Table = 0;

            List<QuotationItem> quotation = new List<QuotationItem>(); // TODO Docanse - upravit 
            foreach (CCladdingAccessories_Item_Piece item in claddingAccessoriesItems_Piece)
            {
                QuotationItem qitem = new QuotationItem();

                // Nastavime parametre z CCladdingAccessories_Item_Piece do QuotationItem (TO Ondrej, toto je asi zbytocny krok ???)
                qitem.Name = item.Name;
                qitem.Quantity = item.Count;
                qitem.Note = item.Note;

                quotation.Add(qitem);

                //dTotalItemsMass_Table += item.Mass_per_piece;
                //dTotalItemsPrice_Table += item.Price_PPP_NZD;
                iTotalItemsNumber_Table += item.Count;
            }

            dBuildingMass += dTotalItemsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalItemsPrice_Table;

            // Create Table
            DataTable table = new DataTable("CladdingAccessories_Items_Piece");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Name.ColumnName, QuotationHelper.colProp_Name.DataType);
            table.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_Note.ColumnName, QuotationHelper.colProp_Note.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Name.ColumnName] = item.Name;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                    row[QuotationHelper.colProp_Note.ColumnName] = item.Note;
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Name.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalItemsNumber_Table;
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalItemsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalItemsPrice_Table.ToString("F2");
            row[QuotationHelper.colProp_Note.ColumnName] = "";
            table.Rows.Add(row);

            return ds;
        }

        private void Datagrid_CladdingAccessories_Items_Length_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_CladdingAccessories_Items_Length);
        }

        private void Datagrid_CladdingAccessories_Items_Piece_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_CladdingAccessories_Items_Piece);
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}