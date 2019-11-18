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
    public partial class SCVDisplayOptionsWindow : Window
    {
        public SCVDisplayOptionsWindow()
        {
            InitializeComponent();

            bool drawOutLine = true; // Default
            bool drawHoles = true; // Default

            SCVDisplayOptionsViewModel vm = new SCVDisplayOptionsViewModel(false, drawOutLine, false, drawHoles, false, false, false, false, false);
            vm.PropertyChanged += HandleDisplayOptionsPropertyChangedEvent;
            this.DataContext = vm;
        }

        public SCVDisplayOptionsWindow(bool drawPoints, bool drawOutLine, bool drawPointNumbers, bool drawHoles, bool drawHoleCentreSymbol, bool drawDrillingRoute,
                                bool drawDimensions, bool drawMemberOutline, bool drawBendLines)
        {
            InitializeComponent();

            SCVDisplayOptionsViewModel vm = new SCVDisplayOptionsViewModel(drawPoints, drawOutLine, drawPointNumbers, drawHoles, drawHoleCentreSymbol, drawDrillingRoute,
                                drawDimensions, drawMemberOutline, drawBendLines);
            vm.PropertyChanged += HandleDisplayOptionsPropertyChangedEvent;
            this.DataContext = vm;
        }

        private void HandleDisplayOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is SCVDisplayOptionsViewModel)
            {
                
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            SCVDisplayOptionsViewModel vm = this.DataContext as SCVDisplayOptionsViewModel;
            if (vm != null) vm.ExportRequired = true;
            this.Close();
        }
    }
}
