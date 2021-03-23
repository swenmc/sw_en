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
        private bool ModelOptionsChanged = false;        
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

                if (e.PropertyName == "VariousBayWidths")
                {
                    //clear bay widths
                    if (_pfdVM._modelOptionsVM.VariousBayWidths == false)
                    {
                        _pfdVM._baysWidthOptionsVM.ResetBaysWidths(_pfdVM.Frames - 1, _pfdVM.BayWidth);
                    }
                    else
                    {
                        _pfdVM._baysWidthOptionsVM = new BayWidthOptionsViewModel(_pfdVM.Frames - 1, _pfdVM.BayWidth);
                    }
                }

                if (e.PropertyName == "EnableCrossBracing")
                {
                    //clear cross bracing
                    if (_pfdVM._modelOptionsVM.EnableCrossBracing == false)
                    {
                        _pfdVM._crossBracingOptionsVM.ClearCrossBracing();
                    }
                    else
                    {
                        _pfdVM._crossBracingOptionsVM.Update(_pfdVM.Frames - 1, _pfdVM.OneRafterPurlinNo);
                    }
                }
                if (e.PropertyName == "EnableCanopies")
                {
                    //clear canopies
                    if (_pfdVM._modelOptionsVM.EnableCanopies == false)
                    {
                        _pfdVM._canopiesOptionsVM.ClearCanopies();
                    }
                    else
                    {
                        _pfdVM._canopiesOptionsVM.Update(_pfdVM.Frames - 1, _pfdVM.Width);
                    }
                }
                if (e.PropertyName == "EnableCladding")
                {
                    //reset cladding
                }
                
                ModelOptionsChanged = true;
            }
        }
        

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (ModelOptionsChanged)
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
