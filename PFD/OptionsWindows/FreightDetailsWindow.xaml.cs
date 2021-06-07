using BaseClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        private void HandleFreightDetails_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is FreightDetailsViewModel)
            {
                FreightDetailsViewModel vm = sender as FreightDetailsViewModel;
                if (e.PropertyName == "RoadUnitPrice1")
                {
                    Text_RoadUnitPriceBasic.Text = $"Road unit price (≤{vm.RoadUnitPrice1} m):";
                    Text_RoadUnitPriceOversize.Text = $"Road unit price (>{vm.RoadUnitPrice1} m):";
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
