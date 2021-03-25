using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BaseClasses;
using PFD.Infrastructure;

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

            if (_pfdVM._baysWidthOptionsVM == null) _pfdVM._baysWidthOptionsVM = new BayWidthOptionsViewModel(_pfdVM.Frames - 1, _pfdVM.BayWidth);

            _pfdVM._baysWidthOptionsVM.PropertyChanged += HandleBayWidthsOptionsPropertyChangedEvent;
            
            this.DataContext = _pfdVM._baysWidthOptionsVM;

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
                if (e.PropertyName == "Width")
                {
                    CBayInfo bi = sender as CBayInfo;
                    if (!bi.ValidateWidth(_pfdVM.LengthOverall))
                    {
                        MessageBox.Show("Wrong width value.");
                        bi.UndoWidth();
                    }
                    //To Mato zvladnes check, co tam vojde okno, dvere?
                    //uz implementovane, staci ak skontrolujes/otestujes a mozu sa komenty zmazat
                    else if (!CDoorsAndWindowsHelper.IsEnoughtPlaceForDoors(bi.BayNumber, bi.Width, _pfdVM.DoorBlocksProperties))
                    {
                        MessageBox.Show("Not enought space for doors.");
                        bi.UndoWidth();
                    }
                    else if (!CDoorsAndWindowsHelper.IsEnoughtPlaceForWindows(bi.BayNumber, bi.Width, _pfdVM.WindowBlocksProperties))
                    {
                        MessageBox.Show("Not enought space for windows.");
                        bi.UndoWidth();
                    }
                }
                BaysWidthOptionsChanged = true;
            }
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
            if (BaysWidthOptionsChanged)
            {
                _pfdVM.BaysWidthOptionsChanged = true;
                
            }

            BaysWidthOptionsChanged = false;
        }

        //toto som zapinal, lebo inak sa neupdatuje tabulka s bay widths 
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _pfdVM._baysWidthOptionsVM.PropertyChanged -= HandleBayWidthsOptionsPropertyChangedEvent; //reregister events
            _pfdVM._baysWidthOptionsVM.PropertyChanged += HandleBayWidthsOptionsPropertyChangedEvent;
            this.DataContext = _pfdVM._baysWidthOptionsVM;
        }

        private void Datagrid_BayWidths_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDataValid(e.Text);
        }

        bool IsDataValid(string data)
        {
            //allow commas and dots
            if (data == "," || data == ".") return true;

            try
            {
                float.Parse(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void TextBox_Width_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDataValid(e.Text);
        }

        private void BtnApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (BaysWidthOptionsChanged)
            {
                _pfdVM.BaysWidthOptionsChanged = true;

            }

            BaysWidthOptionsChanged = false;
        }
    }
}
