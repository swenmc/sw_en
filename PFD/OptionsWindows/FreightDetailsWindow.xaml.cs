using BaseClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace PFD
{
    /// <summary>
    /// Interaction logic for ProjectInfo.xaml
    /// </summary>
    public partial class FreightDetailsWindow : Window
    {
        private CPFDViewModel _pfdVM;

        private bool m_FreightDetailsChanged;

        public bool FreightDetailsChanged
        {
            get
            {
                return m_FreightDetailsChanged;
            }

            set
            {
                m_FreightDetailsChanged = value;
            }
        }

        public FreightDetailsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            if (pfdVM._freightDetailsVM == null) return;

            pfdVM._freightDetailsVM.PropertyChanged -= HandleFreightDetails_PropertyChanged;
            pfdVM._freightDetailsVM.PropertyChanged += HandleFreightDetails_PropertyChanged;

            this.DataContext = pfdVM._freightDetailsVM;

            CreateTableRouteSegments();

            Text_RoadUnitPriceBasic.Text = $"Road unit price (≤{pfdVM._freightDetailsVM.MaxItemLengthBasic} m):";
            Text_RoadUnitPriceOversize.Text = $"Road unit price (>{pfdVM._freightDetailsVM.MaxItemLengthBasic} m):";
        }

        private void CreateTableRouteSegments()
        {
            DataSet ds = GetTableRouteSegments();

            Datagrid_RouteSegments.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_RouteSegments.Loaded += Datagrid_RouteSegments_Loaded;
        }

        public DataSet GetTableRouteSegments()
        {
            DataTable dt = new DataTable("RouteSegments");
            // Create Table Rows
            dt.Columns.Add("ID");
            dt.Columns.Add("TransportType");
            dt.Columns.Add("Distance");
            dt.Columns.Add("Time");
            dt.Columns.Add("UnitPrice_NZD");
            dt.Columns.Add("TotalPrice_NZD");

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // To Ondrej - docasne, ak nie co nie je naplnene opustam funkciu
            if (_pfdVM._freightDetailsVM == null || _pfdVM._freightDetailsVM.RouteSegments == null || _pfdVM._freightDetailsVM.RouteSegments.Count == 0)
                return null;

            int totalDistance = 0;
            int totalTime = 0;
            float totalPrice = 0f;

            DataRow row;
            foreach (RouteSegmentsViewModel rs in _pfdVM._freightDetailsVM.RouteSegments)
            {
                row = dt.NewRow();

                try
                {
                    row["ID"] = rs.ID;
                    row["TransportType"] = rs.TransportType;
                    row["Distance"] = rs.Distance;
                    row["Time"] = GetRouteDuration(rs.Time);
                    row["UnitPrice_NZD"] = rs.UnitPrice;
                    row["TotalPrice_NZD"] = rs.Price;

                    totalDistance += rs.Distance;
                    totalTime += rs.Time;
                    totalPrice += rs.Price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            // Last row
            row = dt.NewRow();
            row["ID"] = "Total:";
            row["TransportType"] = "";
            row["Distance"] = totalDistance;
            row["Time"] = GetRouteDuration(totalTime);
            row["UnitPrice_NZD"] = "";
            row["TotalPrice_NZD"] = totalPrice;
            dt.Rows.Add(row);

            return ds;
        }

        private void Datagrid_RouteSegments_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_RouteSegments);
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

        private string GetRouteDuration(int duration)
        {
            int hours = duration / 3600;
            int min = (duration % 3600) / 60;
            return $"{hours} h {min} min.";
        }

        private void HandleFreightDetails_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is FreightDetailsViewModel)
            {
                FreightDetailsViewModel vm = sender as FreightDetailsViewModel;
                if (e.PropertyName == "MaxItemLengthBasic")
                {
                    Text_RoadUnitPriceBasic.Text = $"Road unit price (≤{vm.MaxItemLengthBasic} m):";
                    Text_RoadUnitPriceOversize.Text = $"Road unit price (>{vm.MaxItemLengthBasic} m):";
                }
                FreightDetailsChanged = true;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}