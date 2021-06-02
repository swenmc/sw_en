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
using BaseClasses;
using MATH;

namespace PFD
{
    public partial class QuotationExportOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;

        public QuotationExportOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            //pfdVM._quotationExportOptionsVM = pfdVM._quotationDisplayOptionsVM.Clone();

            if (!pfdVM._quotationDisplayOptionsVM.DisplayMembers) chckDisplayMembers.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayPlates) chckDisplayPlates.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayConnectors) chcDisplayConnectors.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayBoltNuts) chckDisplayBoltNuts.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayCladding) chckDisplayCladding.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayFibreglass) chckDisplayFibreglass.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayRoofNetting) chckDisplayRoofNetting.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayDoorsAndWindows) chckDisplayDoorsAndWindows.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayGutters) chckDisplayGutters.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayDownpipe) chckDisplayDownpipe.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayFlashing) chckDisplayFlashing.Visibility = Visibility.Collapsed;
            if (!pfdVM._quotationDisplayOptionsVM.DisplayPackers) chckDisplayPackers.Visibility = Visibility.Collapsed;

            //pfdVM._quotationExportOptionsVM = pfdVM._quotationDisplayOptionsVM.Clone();

            pfdVM._quotationExportOptionsVM.PropertyChanged += HandleExportOptionsPropertyChangedEvent;
            this.DataContext = pfdVM._quotationExportOptionsVM;
        }

        private void HandleExportOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is QuotationDisplayOptionsViewModel)
            {

            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}