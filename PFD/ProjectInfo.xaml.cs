using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PFD
{
    /// <summary>
    /// Interaction logic for ProjectInfo.xaml
    /// </summary>
    public partial class ProjectInfo : Window
    {
        public ProjectInfo(CProjectInfoVM vm)
        {
            InitializeComponent();
            
            vm.PropertyChanged += HandleProjectInfoPropertyChangedEvent;
            this.DataContext = vm;
        }

        protected void HandleProjectInfoPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            //CProjectInfoVM vm = sender as CProjectInfoVM;
            

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
