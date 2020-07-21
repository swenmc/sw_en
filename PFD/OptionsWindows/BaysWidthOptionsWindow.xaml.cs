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
    public partial class BaysWidthOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool BaysWidthOptionsChanged = false;
        public BaysWidthOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            BaysWidthOptionsChanged = false;

            if (pfdVM._baysWidthOptionsVM == null) pfdVM._baysWidthOptionsVM = new BayWidthOptionsViewModel(pfdVM.Frames - 1, pfdVM.fBayWidth);

            pfdVM._baysWidthOptionsVM.PropertyChanged += HandleBayWidthsOptionsPropertyChangedEvent;
            
            this.DataContext = pfdVM._baysWidthOptionsVM;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        private void HandleBayWidthsOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is BayWidthOptionsViewModel)
            {
                
            }
            if (sender is CBayInfo)
            {
                BaysWidthOptionsChanged = true;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (BaysWidthOptionsChanged) _pfdVM.BaysWidthOptionsChanged = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Datagrid_Components_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Datagrid_Components_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            BayWidthOptionsViewModel vm = this.DataContext as BayWidthOptionsViewModel;
            if (vm.BayFrom > vm.BayTo) return;

            for (int i = vm.BayFrom; i <= vm.BayTo; i++)
            {
                vm.BayWidthList[i - 1].Width = vm.Width;                
            }
        }
    }
}
