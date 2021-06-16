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
            
            this.DataContext = pfdVM._displayOptionsVM.DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene];

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        private void HandleDisplayOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DisplayOptionsAllViewModel)
            {

            }
            if (sender is DisplayOptionsViewModel)
            {
                if (e.PropertyName == "DisplayCladding" || e.PropertyName == "DisplayDoors" || e.PropertyName == "DisplayWindows" ||
                    e.PropertyName == "FrontCladdingOpacity" || e.PropertyName == "LeftCladdingOpacity" || e.PropertyName == "RoofCladdingOpacity" ||
                    e.PropertyName == "FlashingOpacity" || e.PropertyName == "DoorPanelOpacity" || e.PropertyName == "WindowPanelOpacity" || e.PropertyName == "FibreglassOpacity" || e.PropertyName == "DoorsSimpleSolidModel")
                    RecreateModelRequired = true;

                if (e.PropertyName == "ColorsAccordingToMembersPrefix" || e.PropertyName == "ColorsAccordingToMembersPosition")
                {
                    MemberOptionsChanged = true;
                    RecreateModelRequired = true;
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
                    if(_pfdVM._displayOptionsVM.DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ColorsAccordingToMembersPosition) _pfdVM._componentVM.SetColorsAccordingToPosition();
                    else if(_pfdVM._displayOptionsVM.DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ColorsAccordingToMembersPrefix) _pfdVM._componentVM.SetColorsAccordingToPrefixes();                    
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

        private void MainTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MessageBox.Show(((TreeViewItem)e.NewValue).Header.ToString());

            int selectedIndex = 0;            

            switch (((TreeViewItem)e.NewValue).Name.ToString())
            {
                case "twiGUI_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.GUI_3D_Scene; break;
                case "twiGUI_Joint_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Joint_Preview; break;
                case "twiGUI_Foundation_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Foundation_Preview; break;
                case "twiGUI_Accessories_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Accessories_Preview; break;
                case "twiReport_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Report_3DScene; break;
                case "twiReport_Joints": selectedIndex = (int)EDisplayOptionsTypes.Report_Joints; break;
                case "twiReport_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Report_Foundations; break;
                case "twiLayouts_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Layouts_3D_Scene; break;
                case "twiLayouts_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Elevations; break;
                case "twiLayouts_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Roof; break;
                case "twiLayouts_FW_Frames": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Frames; break;
                case "twiLayouts_FW_Columns": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Columns; break;
                case "twiLayouts_FW_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Foundations; break;
                case "twiLayouts_FW_Floor": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Floor; break;
                case "twiLayouts_CW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Elevations; break;
                case "twiLayouts_CW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Roof; break;
                case "twiLayouts_Joints": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Joints; break;
                case "twiLayouts_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Foundations; break;                
            }

            this.DataContext = _pfdVM._displayOptionsVM.DisplayOptionsList[selectedIndex];            
        }
    }
}
