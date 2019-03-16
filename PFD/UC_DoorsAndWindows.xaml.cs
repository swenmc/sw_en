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
using System.Data.SqlClient;
using System.Data;
using BaseClasses;
using MATH;
using System.ComponentModel;

namespace PFD
{
    public partial class UC_DoorsAndWindows : UserControl
    {
        public UC_DoorsAndWindows(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            CDoorsWindowsVM vm = new CDoorsWindowsVM();
            vm.Doors = pfdVM.DoorBlocksProperties;

            vm.PropertyChanged += HandleComponentListPropertyChangedEvent;
            this.DataContext = vm;
        }

        protected void HandleComponentListPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }



    }

    
}
