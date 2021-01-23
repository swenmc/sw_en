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
    public partial class DisplayOptionsWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private bool DisplayOptionsChanged = false;
        private bool MemberOptionsChanged = false;
        private bool RecreateModelRequired = false;
        public DisplayOptionsWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            DisplayOptionsChanged = false;
            
            pfdVM._displayOptionsVM.PropertyChanged += HandleDisplayOptionsPropertyChangedEvent;
            
            this.DataContext = pfdVM._displayOptionsVM;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        
        private void HandleDisplayOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DisplayOptionsViewModel)
            {
                if (e.PropertyName == "DisplayCladding" || e.PropertyName == "DisplayDoors" || e.PropertyName == "DisplayWindows" ||
                    e.PropertyName == "FrontCladdingOpacity" || e.PropertyName == "LeftCladdingOpacity" || e.PropertyName == "RoofCladdingOpacity" ||
                    e.PropertyName == "DoorPanelOpacity" || e.PropertyName == "WindowPanelOpacity")
                    RecreateModelRequired = true;

                if (e.PropertyName == "ColorsAccordingToMembersPrefix" || e.PropertyName == "ColorsAccordingToMembersPosition")
                {
                    MemberOptionsChanged = true;
                    
                }

                DisplayOptionsChanged = true;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayOptionsChanged)
            {
                if (MemberOptionsChanged)
                {
                    if(_pfdVM._displayOptionsVM.ColorsAccordingToMembersPosition) _pfdVM._componentVM.SetColorsAccordingToPosition();
                    else if(_pfdVM._displayOptionsVM.ColorsAccordingToMembersPrefix) _pfdVM._componentVM.SetColorsAccordingToPrefixes();
                }

                _pfdVM.RecreateModel = RecreateModelRequired;
                _pfdVM.SynchronizeGUI = true;                
            }
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
          
            //radioColorsIn3DMembers.IsChecked = _pfdVM._displayOptionsVM.ColorsAccordingToMembers;


        }
    }
}
