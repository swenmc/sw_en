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
    public partial class DisplayOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool DisplayOptionsChanged = false;
        public DisplayOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            DisplayOptionsChanged = false;
            
            pfdVM._displayOptionsVM.PropertyChanged += HandleDisplayOptionsPropertyChangedEvent;
            this.DataContext = pfdVM._displayOptionsVM;
        }

        
        private void HandleDisplayOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DisplayOptionsViewModel)
            {
                DisplayOptionsChanged = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            DisplayOptionsChanged = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayOptionsChanged) _pfdVM.SynchronizeGUI = true;
            this.Close();
        }
    }
}
