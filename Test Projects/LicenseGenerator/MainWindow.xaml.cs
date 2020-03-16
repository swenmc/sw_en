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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LicenseGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel vm;
        public MainWindow()
        {
            InitializeComponent();

            vm = new MainViewModel();
            vm.PropertyChanged += Vm_PropertyChanged;
            this.DataContext = vm;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            vm.Key = vm.GetLicenseKey();
        }

        private void Button(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(vm.Key);
        }
    }
}
