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
    public partial class DoorGeneratorWindow : Window
    {
        
        
        public DoorGeneratorWindow()
        {
            InitializeComponent();

            DoorGeneratorViewModel vm = new DoorGeneratorViewModel();
            vm.PropertyChanged += HandleDoorGeneratorPropertyChanged;
            this.DataContext = vm;
        }

        private void HandleDoorGeneratorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }
        
        

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
