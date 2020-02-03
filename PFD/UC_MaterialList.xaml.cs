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

namespace PFD
{
    /// <summary>
    /// Interaction logic for MaterialList.xaml
    /// </summary>
    public partial class UC_MaterialList : UserControl
    {        
        double dBuildingMass = 0;
        double dBuildingNetPrice_WithoutMargin_WithoutGST = 0;

        public UC_MaterialList(CModel_PFD model)
        {
            //DateTime start = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("UC_MaterialList");
            InitializeComponent();
            //System.Diagnostics.Trace.WriteLine("after InitializeComponent: " + (DateTime.Now - start).TotalMilliseconds);

            CMaterialListViewModel vm = new CMaterialListViewModel(model);
            vm.PropertyChanged += MaterialListViewModel_PropertyChanged;
            this.DataContext = vm;
            //System.Diagnostics.Trace.WriteLine("after CMaterialListViewModel: " + (DateTime.Now - start).TotalMilliseconds) ;
                        
            // Plates
            CreateTablePlates(model);

            // Screws
            // Bolts
            // Anchors
            CreateTableConnectors(model);
        }

        private void MaterialListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
        
        private void CreateTablePlates(CModel model)
        {
            DataSet ds = QuotationHelper.GetTablePlates(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

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

            Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        }

        private void Datagrid_Connectors_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Connectors);
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

    }
}
