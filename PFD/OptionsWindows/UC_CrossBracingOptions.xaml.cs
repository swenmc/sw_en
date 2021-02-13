using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BaseClasses;

namespace PFD
{
    public partial class UC_CrossBracingOptions : UserControl
    {
        private CPFDViewModel _pfdVM;
        private bool m_CrossBracingOptionsChanged;

        public bool CrossBracingOptionsChanged
        {
            get
            {
                return m_CrossBracingOptionsChanged;
            }

            set
            {
                m_CrossBracingOptionsChanged = value;
            }
        }

        public UC_CrossBracingOptions(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            CrossBracingOptionsChanged = false;

            if (pfdVM._crossBracingOptionsVM == null) pfdVM._crossBracingOptionsVM = new CrossBracingOptionsViewModel(pfdVM.Frames - 1, pfdVM.OneRafterPurlinNo); // Počet bays = počet frames - 1


            pfdVM._crossBracingOptionsVM.PropertyChanged += HandleCrossBracingOptionsPropertyChangedEvent;
            
            this.DataContext = pfdVM._crossBracingOptionsVM;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        
        private void HandleCrossBracingOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is CrossBracingOptionsViewModel)
            {
                //if(e.PropertyName == "CrossBracingItem_PropertyChanged") CrossBracingOptionsChanged = true;
            }
            if (sender is CCrossBracingInfo)
            {
                CrossBracingOptionsChanged = true;
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
            CrossBracingOptionsViewModel vm = this.DataContext as CrossBracingOptionsViewModel;
            if (vm.BayFrom > vm.BayTo) return;

            if (vm.Roof && vm.RoofPosition == "None") { MessageBox.Show("None is not a valid value when roof is selected."); return; }

            for (int i = vm.BayFrom; i <= vm.BayTo; i++)
            {
                vm.CrossBracingList[i - 1].WallLeft = vm.WallLeft;
                vm.CrossBracingList[i - 1].WallRight = vm.WallRight;
                vm.CrossBracingList[i - 1].Roof = vm.Roof;
                vm.CrossBracingList[i - 1].RoofPosition = vm.RoofPosition;
                vm.CrossBracingList[i - 1].FirstCrossOnRafter = vm.FirstCrossOnRafter;
                vm.CrossBracingList[i - 1].LastCrossOnRafter = vm.LastCrossOnRafter;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (CrossBracingOptionsChanged) _pfdVM.CrossBracingOptionsChanged = true;

            CrossBracingOptionsChanged = false;
        }
    }
}
