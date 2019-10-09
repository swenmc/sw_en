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
    /// <summary>
    /// Interaction logic for WindPressureCalculator.xaml
    /// </summary>
    public partial class DisplayOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;

        public DisplayOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            DisplayOptionsViewModel vm = new DisplayOptionsViewModel(pfdVM);
            vm.PropertyChanged += HandleDisplayOptionsPropertyChangedEvent;
            this.DataContext = vm;
        }

        
        private void HandleDisplayOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DisplayOptionsViewModel)
            {
               
            }
        }

        

       
    }
}
