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
        //float fRollerDoorTrimmerFlashing_TotalLength = 0;
        //float fRollerDoorLintelFlashing_TotalLength = 0;
        //float fRollerDoorLintelCapFlashing_TotalLength = 0;
        //float fPADoorTrimmerFlashing_TotalLength = 0;
        //float fPADoorLintelFlashing_TotalLength = 0;
        //float fWindowFlashing_TotalLength = 0;

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
                //CreateTableFlashing(
                //    fRollerDoorTrimmerFlashing_TotalLength,
                //    fRollerDoorLintelFlashing_TotalLength,
                //    fRollerDoorLintelCapFlashing_TotalLength,
                //    fPADoorTrimmerFlashing_TotalLength,
                //    fPADoorLintelFlashing_TotalLength,
                //    fWindowFlashing_TotalLength);
                CreateTableFlashing();
            }
            else
            {
                TextBlock_Flashing.Visibility = Visibility.Collapsed;
                Datagrid_Flashing.Visibility = Visibility.Collapsed;
            }

            // Packers
            if (PartListHelper.DisplayPackersTable(pfdVM))
            {
                CreateTablePackers();
            }
            else
            {
                TextBlock_Packers.Visibility = Visibility.Collapsed;
                Datagrid_Packers.Visibility = Visibility.Collapsed;
            }

            // Cladding Accessories
            if (PartListHelper.DisplayCladdingAccesoriesTable(pfdVM))
            {
                CreateTableCladdingAccessories(pfdVM);
            }
            else
            {
                TxtCladdingAccessories.Visibility = Visibility.Collapsed;
                Datagrid_CladdingAccessories_Items_Length.Visibility = Visibility.Collapsed;
                Datagrid_CladdingAccessories_Items_Piece.Visibility = Visibility.Collapsed;
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
            DataSet ds = QuotationHelper.GetTableDoorsAndWindows(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST, out fTotalAreaOfOpennings);

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
        private void CreateTableFlashing()
        {
            DataSet ds = QuotationHelper.GetTableFlashing(_pfdVM, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Flashing.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Flashing.Loaded += Datagrid_Flashing_Loaded;
        }

        private void Datagrid_Flashing_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Flashing);
        }

        private void CreateTablePackers()
        {
            DataSet ds = QuotationHelper.GetTablePackers(_pfdVM.RollerDoorLintelFlashing_TotalLength, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds != null)
                Datagrid_Packers.ItemsSource = ds.Tables[0].AsDataView();
            else
            {
                TextBlock_Packers.Visibility = Visibility.Collapsed;
                Datagrid_Packers.Visibility = Visibility.Collapsed;
            }
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

        private void CreateTableCladdingAccessories(CPFDViewModel vm)
        {
            List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece = null;
            List<CCladdingAccessories_Item_Length> claddingAccessoriesItems_Length = null;
            PartListHelper.GetTableCladdingAccessoriesLists(vm, out claddingAccessoriesItems_Piece, out claddingAccessoriesItems_Length);

            // Items - Length
            DataSet ds1 = PartListHelper.GetTableCladdingAccessories_Items_Length(claddingAccessoriesItems_Length, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds1 != null)
            {
                Datagrid_CladdingAccessories_Items_Length.ItemsSource = ds1.Tables[0].AsDataView();  //draw the table to datagridview
                Datagrid_CladdingAccessories_Items_Length.Loaded += Datagrid_CladdingAccessories_Items_Length_Loaded;
            }
            else
            {
                Datagrid_CladdingAccessories_Items_Length.Visibility = Visibility.Collapsed;
            }

            // Items - Piece
            DataSet ds2 = PartListHelper.GetTableCladdingAccessories_Items_Piece(claddingAccessoriesItems_Piece, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds2 != null)
            {
                Datagrid_CladdingAccessories_Items_Piece.ItemsSource = ds2.Tables[0].AsDataView();  //draw the table to datagridview
                Datagrid_CladdingAccessories_Items_Piece.Loaded += Datagrid_CladdingAccessories_Items_Piece_Loaded;
            }
            else
            {
                Datagrid_CladdingAccessories_Items_Piece.Visibility = Visibility.Collapsed;
            }

            if(ds1 == null && ds2 == null) TxtCladdingAccessories.Visibility = Visibility.Collapsed;
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