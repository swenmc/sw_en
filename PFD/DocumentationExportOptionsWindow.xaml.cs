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
    public partial class DocumentationExportOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;

        public DocumentationExportOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            if (pfdVM._documentationExportOptionsVM == null) pfdVM._documentationExportOptionsVM = new DocumentationSettingsViewModel();


            //To Mato - toto mi nedava zmysel, lebo ked zaskrtnem false tak na 2.krat to zmizne
            //if (!pfdVM._documentationExportOptionsVM.ExportMembersXLS) chckExportMembersXLS.Visibility = Visibility.Collapsed;
            //if (!pfdVM._documentationExportOptionsVM.ExportPlatesPDF) chckExportPlatesPDF.Visibility = Visibility.Collapsed;
            //if (!pfdVM._documentationExportOptionsVM.ExportCNCSetup) chckExportCNCSetup.Visibility = Visibility.Collapsed;
            //if (!pfdVM._documentationExportOptionsVM.ExportCNCDrilling) chckExportCNCDrilling.Visibility = Visibility.Collapsed;
            //if (!pfdVM._documentationExportOptionsVM.Export2D_DXF) chckExport2D_DXF.Visibility = Visibility.Collapsed;
            //if (!pfdVM._documentationExportOptionsVM.Export3D_DXF) chckExport3D_DXF.Visibility = Visibility.Collapsed;
            //if (!pfdVM._documentationExportOptionsVM.ExportSCV) chckExportSCV.Visibility = Visibility.Collapsed;
            
            pfdVM._documentationExportOptionsVM.PropertyChanged += HandleExportOptionsPropertyChangedEvent;            
            this.DataContext = pfdVM._documentationExportOptionsVM;
        }


        private void HandleExportOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DocumentationSettingsViewModel)
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
