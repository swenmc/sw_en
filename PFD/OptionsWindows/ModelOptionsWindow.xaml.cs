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
    public partial class ModelOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool GeneralOptionsChanged = false;        
        public ModelOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            pfdVM._modelOptionsVM.PropertyChanged += _modelOptionsVM_PropertyChanged;
            
            this.DataContext = pfdVM._modelOptionsVM;
        }

        private void _modelOptionsVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is ModelOptionsViewModel)
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
                if (_pfdVM._modelOptionsVM.SameColorsDoor) _pfdVM.SetAllDoorCoatingColorToSame();
                if (_pfdVM._modelOptionsVM.SameColorsFGD) _pfdVM.SetAll_FGD_CoatingColorToSame();
                else
                {
                    if (_pfdVM._modelOptionsVM.SameColorsFlashings) _pfdVM.SetAllFlashingsCoatingColorToSame();
                    if (_pfdVM._modelOptionsVM.SameColorsGutters) _pfdVM.SetAllGuttersCoatingColorToSame();
                    if (_pfdVM._modelOptionsVM.SameColorsDownpipes) _pfdVM.SetAllDownpipesCoatingColorToSame();
                }
                
                _pfdVM.ModelOptionsChanged = true;
            }
            this.Close();
        }
    }
}
