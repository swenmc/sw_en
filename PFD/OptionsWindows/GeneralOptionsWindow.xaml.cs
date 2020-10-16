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
        private bool GeneralOptionsChanged = false;        
        public GeneralOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            pfdVM._generalOptionsVM.PropertyChanged += _generalOptionsVM_PropertyChanged;
            
            this.DataContext = pfdVM._generalOptionsVM;
        }

        private void _generalOptionsVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is GeneralOptionsViewModel)
            {
                if (e.PropertyName == "UseStraightReinforcementBars") _pfdVM.RecreateFoundations = true;
                if (e.PropertyName == "BracingEverySecondRowOfGirts") _pfdVM.RecreateJoints = true;
                if (e.PropertyName == "BracingEverySecondRowOfPurlins") _pfdVM.RecreateJoints = true;
                if (e.PropertyName == "WindPostUnderRafter") { _pfdVM.RecreateJoints = true; _pfdVM.RecreateFoundations = true; }

                GeneralOptionsChanged = true;
            }
        }
        

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (GeneralOptionsChanged)
            {
                if (_pfdVM._generalOptionsVM.SameColorsDoor) _pfdVM.SetAllDoorCoatingColorToSame();
                if (_pfdVM._generalOptionsVM.SameColorsFGD) _pfdVM.SetAll_FGD_CoatingColorToSame();
                else
                {
                    if (_pfdVM._generalOptionsVM.SameColorsFlashings) _pfdVM.SetAllFlashingsCoatingColorToSame();
                    if (_pfdVM._generalOptionsVM.SameColorsGutters) _pfdVM.SetAllGuttersCoatingColorToSame();
                    if (_pfdVM._generalOptionsVM.SameColorsDownpipes) _pfdVM.SetAllDownpipesCoatingColorToSame();
                }
                
                _pfdVM.GeneralOptionsChanged = true;
            }
            this.Close();
        }
    }
}
