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
    public partial class UC_CladdingOptions : UserControl
    {
        private CPFDViewModel _pfdVM;
        private bool CladdingOptionsChanged = false;
        
        //private bool RecreateModelRequired = false;
        public UC_CladdingOptions(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            CladdingOptionsChanged = false;
            
            pfdVM._claddingOptionsVM.PropertyChanged += HandleCladdingOptionsPropertyChangedEvent;
            
            this.DataContext = pfdVM._claddingOptionsVM;
        }

        
        private void HandleCladdingOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is CladdingOptionsViewModel)
            {
                CladdingOptionsChanged = true;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (CladdingOptionsChanged)
            {
                _pfdVM.CladdingOptionsChanged = true;                
            }
            CladdingOptionsChanged = false;
        }

        private void BtnFiberglassGenerator_Click(object sender, RoutedEventArgs e)
        {
            FibreglassGeneratorWindow w = new FibreglassGeneratorWindow(_pfdVM);
            w.ShowDialog();
        }
    }
}
