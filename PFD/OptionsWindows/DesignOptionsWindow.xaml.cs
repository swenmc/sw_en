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
    public partial class DesignOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool DesignOptionsChanged = false;
        public DesignOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            DesignOptionsChanged = false;

            pfdVM._designOptionsVM.PropertyChanged += HandleDesignOptionsPropertyChangedEvent;
            this.DataContext = pfdVM._designOptionsVM;
        }


        private void HandleDesignOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DesignOptionsViewModel)
            {
                DesignOptionsChanged = true;
            }
        }


        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (DesignOptionsChanged) _pfdVM.DesignOptionsChanged = true;
            this.Close();
        }
    }
}
