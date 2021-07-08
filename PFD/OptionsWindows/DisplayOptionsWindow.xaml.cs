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
            _pfdVM = pfdVM;

            InitializeComponent();

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
                _pfdVM.DisplayOptionsChanged = DisplayOptionsChanged;
            }
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
          
            //radioColorsIn3DMembers.IsChecked = _pfdVM._displayOptionsVM.ColorsAccordingToMembers;


        }

        private void MainTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //MessageBox.Show(((TreeViewItem)e.NewValue).Header.ToString());

            int selectedIndex = -1;            

            switch (((TreeViewItem)e.NewValue).Name.ToString())
            {
                case "twiGUI_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.GUI_3D_Scene; Title = "Display Options - GUI 3D scene"; break;
                case "twiGUI_Joint_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Joint_Preview; Title = "Display Options - GUI - Joint Preview"; break;
                case "twiGUI_Foundation_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Foundation_Preview; Title = "Display Options - GUI - Foundation Preview"; break;
                case "twiGUI_Accessories_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Accessories_Preview; Title = "Display Options - GUI Accessories Preview"; break;
                case "twiReport_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Report_3D_Scene; Title = "Display Options - Report 3D scene"; break;
                case "twiReport_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Report_FW_Elevations; Title = "Display Options - Report - Frame Views - Elevations"; break;
                case "twiReport_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Report_FW_Roof; Title = "Display Options - Report - Frame Views - Roof"; break;
                case "twiReport_Joints": selectedIndex = (int)EDisplayOptionsTypes.Report_Joints; Title = "Display Options - Report - Joints"; break;
                case "twiReport_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Report_Foundations; Title = "Display Options - Report - Foundations"; break;
                case "twiLayouts_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Layouts_3D_Scene; Title = "Display Options - Layouts 3D scene"; break;
                case "twiLayouts_FW_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_3D_Scene; Title = "Display Options - Layouts - Frame Views - 3D scene"; break;
                case "twiLayouts_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Elevations; Title = "Display Options - Layouts - Frame Views - Elevations"; break;
                case "twiLayouts_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Roof; Title = "Display Options - Layouts - Frame Views - Roof"; break;
                case "twiLayouts_FW_Frames": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Frames; Title = "Display Options - Layouts - Frame Views - Frames"; break;
                case "twiLayouts_FW_Columns": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Columns; Title = "Display Options - Layouts - Frame Views - Columns"; break;
                case "twiLayouts_FW_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Foundations; Title = "Display Options - Layouts - Frame Views - Foundations"; break;
                case "twiLayouts_FW_Floor": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Floor; Title = "Display Options - Layouts - Frame Views - Floor"; break;
                case "twiLayouts_CW_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_3D_Scene; Title = "Display Options - Layouts - Cladding Views - 3D scene"; break;
                case "twiLayouts_CW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Elevations; Title = "Display Options - Layouts - Cladding Views - Elevations"; break;
                case "twiLayouts_CW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Roof; Title = "Display Options - Layouts - Cladding Views - Roof"; break;
                case "twiLayouts_Joints": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Joints; Title = "Display Options - Layouts - Joints"; break;
                case "twiLayouts_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Foundations; Title = "Display Options - Layouts - Foundations"; break;
            }

            if (selectedIndex != -1)
            {
                this.DataContext = _pfdVM._displayOptionsVM.DisplayOptionsList[selectedIndex];
                ChangeControlsVisibilityBasedOnSelectedTreeViewItem((EDisplayOptionsTypes)selectedIndex);
                ChangeControlsIsEnabledBasedOnSelectedTreeViewItem((EDisplayOptionsTypes)selectedIndex);
            }
        }

        private void ChangeControlsVisibilityBasedOnSelectedTreeViewItem(EDisplayOptionsTypes optsType)
        {
            if (!IsLoaded) return;

            if (optsType != EDisplayOptionsTypes.GUI_3D_Scene) Loads.Visibility = Visibility.Collapsed;
            else Loads.Visibility = Visibility.Visible;
        }
        private void ChangeControlsIsEnabledBasedOnSelectedTreeViewItem(EDisplayOptionsTypes optsType)
        {
            if (!IsLoaded) return;

            if (optsType == EDisplayOptionsTypes.Layouts_3D_Scene || optsType == EDisplayOptionsTypes.Report_3D_Scene 
                || optsType == EDisplayOptionsTypes.Layouts_FW_3D_Scene || optsType == EDisplayOptionsTypes.Layouts_CW_3D_Scene)
            {
                chbDisplayWireFrameModel.IsEnabled = false;
                chbDisplayMembersWireFrame.IsEnabled = false;
                chbDisplayJointsWireFrame.IsEnabled = false;
                chbDisplayPlatesWireFrame.IsEnabled = false;
                chbDisplayConnectorsWireFrame.IsEnabled = false;
                chbDisplayFoundationsWireFrame.IsEnabled = false;
                chbDisplayReinforcementBarsWireFrame.IsEnabled = false;
                chbDisplayFloorSlabWireFrame.IsEnabled = false;
                chbDisplayCladdingWireFrame.IsEnabled = false;
                chbDisplayFibreglassWireFrame.IsEnabled = false;
                chbDisplayDoorsWireFrame.IsEnabled = false;
                chbDisplayWindowsWireFrame.IsEnabled = false;
            }
            else
            {
                chbDisplayWireFrameModel.IsEnabled = true;
                chbDisplayMembersWireFrame.IsEnabled = true;
                chbDisplayJointsWireFrame.IsEnabled = true;
                chbDisplayPlatesWireFrame.IsEnabled = true;
                chbDisplayConnectorsWireFrame.IsEnabled = true;
                chbDisplayFoundationsWireFrame.IsEnabled = true;
                chbDisplayReinforcementBarsWireFrame.IsEnabled = true;
                chbDisplayFloorSlabWireFrame.IsEnabled = true;
                chbDisplayCladdingWireFrame.IsEnabled = true;
                chbDisplayFibreglassWireFrame.IsEnabled = true;
                chbDisplayDoorsWireFrame.IsEnabled = true;
                chbDisplayWindowsWireFrame.IsEnabled = true;
            }

            if (optsType == EDisplayOptionsTypes.GUI_Joint_Preview || optsType == EDisplayOptionsTypes.GUI_Accessories_Preview || optsType == EDisplayOptionsTypes.GUI_Foundation_Preview
                || optsType == EDisplayOptionsTypes.Report_Joints || optsType == EDisplayOptionsTypes.Report_Foundations
                || optsType == EDisplayOptionsTypes.Layouts_Joints || optsType == EDisplayOptionsTypes.Layouts_Foundations)
            {
                chbDisplayGlobalAxis.IsEnabled = false;
            }
            else chbDisplayGlobalAxis.IsEnabled = true;


        }

        private void BtnCopySettings_Click(object sender, RoutedEventArgs e)
        {
            DisplayOptionsCopyWindow copyWindow = new DisplayOptionsCopyWindow(_pfdVM);
            copyWindow.ShowDialog();
            if (copyWindow.OptionsChanged) DisplayOptionsChanged = true;
            if (copyWindow.GUI3DSceneOptionsChanged) { MemberOptionsChanged = true; RecreateModelRequired = true; }
        }
    }
}
