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
    public partial class LayoutExportOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;

        public LayoutExportOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            if (pfdVM._layoutsExportOptionsVM == null) pfdVM._layoutsExportOptionsVM = new LayoutsExportOptionsViewModel();

            pfdVM._layoutsExportOptionsVM.PropertyChanged += HandleExportOptionsPropertyChangedEvent;
            this.DataContext = pfdVM._layoutsExportOptionsVM;
        }


        private void HandleExportOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is LayoutsExportOptionsViewModel)
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
