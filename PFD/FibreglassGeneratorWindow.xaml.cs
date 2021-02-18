using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class FibreglassGeneratorWindow : Window
    {
        
        FibreglassGeneratorViewModel vm;

        public FibreglassGeneratorWindow(int lrBaysNum, int fbBaysNum)
        {
            InitializeComponent();
            
            

            //vm = new FibreglassGeneratorViewModel();
            vm.PropertyChanged += HandleFibreglassPropertyChanged;
            this.DataContext = vm;
        }

        private void HandleFibreglassPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }


        

        

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //vm.AddDoors = true;
            this.Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            //vm.DeleteDoors = true;
            this.Close();
        }
    }
}
