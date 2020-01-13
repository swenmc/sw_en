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
    public partial class SolverOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool SolverOptionsChanged = false;
        public SolverOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            pfdVM._solverOptionsVM.PropertyChanged += _solverOptionsVM_PropertyChanged;

            this.DataContext = pfdVM._solverOptionsVM;
        }

        private void _solverOptionsVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is SolverOptionsViewModel)
            {
                SolverOptionsChanged = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (SolverOptionsChanged) _pfdVM.SolverOptionsChanged = true;
            this.Close();
        }
    }
}
