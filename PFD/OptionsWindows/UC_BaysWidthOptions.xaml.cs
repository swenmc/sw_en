using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BaseClasses;

namespace PFD
{
    public partial class UC_BaysWidthOptions : UserControl
    {
        private CPFDViewModel _pfdVM;
        private bool m_BaysWidthOptionsChanged;

        public bool BaysWidthOptionsChanged
        {
            get
            {
                return m_BaysWidthOptionsChanged;
            }

            set
            {
                m_BaysWidthOptionsChanged = value;
            }
        }

        public UC_BaysWidthOptions(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            BaysWidthOptionsChanged = false;

            if (pfdVM._baysWidthOptionsVM == null) pfdVM._baysWidthOptionsVM = new BayWidthOptionsViewModel(pfdVM.Frames - 1, pfdVM.BayWidth);

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

        

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (BaysWidthOptionsChanged) _pfdVM.BaysWidthOptionsChanged = true;

            BaysWidthOptionsChanged = false;
        }
    }
}
