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
    public partial class CrossBracingOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool CrossBracingOptionsChanged = false;
        public CrossBracingOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            CrossBracingOptionsChanged = false;

            if (pfdVM._crossBracingOptionsVM == null) pfdVM._crossBracingOptionsVM = new CrossBracingOptionsViewModel(pfdVM.Frames - 1); // Počet bays = počet frames - 1


            pfdVM._crossBracingOptionsVM.PropertyChanged += HandleCrossBracingOptionsPropertyChangedEvent;
            
            this.DataContext = pfdVM._crossBracingOptionsVM;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        
        private void HandleCrossBracingOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is CrossBracingOptionsViewModel)
            {
                CrossBracingOptionsChanged = true;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (CrossBracingOptionsChanged) _pfdVM.SynchronizeGUI = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
          
            //radioColorsIn3DMembers.IsChecked = _pfdVM._displayOptionsVM.ColorsAccordingToMembers;


        }

        private void Datagrid_Components_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Datagrid_Components_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
