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
    public partial class GeneralOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        
        public GeneralOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
                        
            this.DataContext = pfdVM;
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
