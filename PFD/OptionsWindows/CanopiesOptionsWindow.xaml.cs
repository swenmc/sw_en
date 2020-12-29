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
    public partial class CanopiesOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool CanopiesOptionsChanged = false;
        public CanopiesOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            CanopiesOptionsChanged = false;

            if (pfdVM._canopiesOptionsVM == null) pfdVM._canopiesOptionsVM = new CanopiesOptionsViewModel(pfdVM.Frames - 1); // Počet bays = počet frames - 1
            
            pfdVM._canopiesOptionsVM.PropertyChanged += HandleCanopiesOptionsPropertyChangedEvent;
            
            this.DataContext = pfdVM._canopiesOptionsVM;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        
        private void HandleCanopiesOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is CanopiesOptionsViewModel)
            {
                
            }
            if (sender is CCanopiesInfo)
            {
                CanopiesOptionsChanged = true;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (CanopiesOptionsChanged) _pfdVM.CanopiesOptionsChanged = true;
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

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            CanopiesOptionsViewModel vm = this.DataContext as CanopiesOptionsViewModel;
            if (vm.BayFrom > vm.BayTo) return;

            for (int i = vm.BayFrom; i <= vm.BayTo; i++)
            {
                vm.CanopiesList[i - 1].Left = vm.Left;
                vm.CanopiesList[i - 1].Right = vm.Right;
                vm.CanopiesList[i - 1].WidthLeft = vm.WidthLeft;
                vm.CanopiesList[i - 1].WidthRight = vm.WidthRight;
                vm.CanopiesList[i - 1].PurlinCountLeft = vm.PurlinCountLeft;
                vm.CanopiesList[i - 1].PurlinCountRight = vm.PurlinCountRight;
            }
        }
    }
}
