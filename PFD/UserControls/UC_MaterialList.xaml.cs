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
                item = new CCladdingAccessories_Item("Fibreglass protection strip 80 mm wide", (int)fLengthProtectionstrip); // DOCASNE INT
                claddingAccessoriesItems.Add(item);

                // 13 - Rooflite support bracket

                // Support bracket
                item = new CCladdingAccessories_Item("Fibreglass support bracket 30x40x1400x1 mm", (int)supportBracketBetweenPurlinsLengthTotal); // DOCASNE INT
                claddingAccessoriesItems.Add(item);

                // Support bracket Fixing
                item = new CCladdingAccessories_Item("TEK screw 14gx115 (platic profile washer and galvanized cap)", iNumberOfSupportBracketBetweenPurlinsFixingPoints);
                claddingAccessoriesItems.Add(item);








                // 21 - Cladding
                ribWidth = _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m;
                float profileFactor = 1; // 1 - Smartdek, 2 - Purlindek and Speedclad
                fixingPointTributaryArea = (ribWidth / profileFactor) * model.fDist_Girt;

                double fWallCladdingArea_WithoutFibreglassAndOpenings = 200; // Todo napojit
                iNumberOfFixingPoints = (int)(fWallCladdingArea_WithoutFibreglassAndOpenings / fixingPointTributaryArea);

                item = new CCladdingAccessories_Item("Smartdek TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints);
                claddingAccessoriesItems.Add(item);
            }

        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}