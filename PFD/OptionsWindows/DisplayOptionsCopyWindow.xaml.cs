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
    public partial class DisplayOptionsCopyWindow : Window
    {
        private CPFDViewModel _pfdVM;
        private string selectedSource;
        public bool OptionsChanged;
        public bool GUI3DSceneOptionsChanged;
        public DisplayOptionsCopyWindow(CPFDViewModel pfdVM)
        {
            _pfdVM = pfdVM;

            OptionsChanged = false;
            GUI3DSceneOptionsChanged = false;

            InitializeComponent();

            chGUI_3D_scene.IsChecked = false;
            chGUI_3D_scene.IsEnabled = false;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private int GetSelectedSourceOptions()
        {
            int selectedIndex = -1;
            selectedSource = "";

            TreeViewItem item = TreeViewSource.SelectedItem as TreeViewItem;
            if (item == null) return selectedIndex;

            switch ((item).Name.ToString())
            {
                case "twiSourceGUI_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.GUI_3D_Scene; selectedSource = "GUI 3D scene"; break;
                case "twiSourceGUI_Joint_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Joint_Preview; selectedSource = "GUI - Joint Preview"; break;
                case "twiSourceGUI_Foundation_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Foundation_Preview; selectedSource = "GUI - Foundation Preview"; break;
                case "twiSourceGUI_Accessories_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Accessories_Preview; selectedSource = "GUI Accessories Preview"; break;
                case "twiSourceReport_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Report_3D_Scene; selectedSource = "Report 3D scene"; break;
                case "twiSourceReport_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Report_3D_Scene; selectedSource = "Report - Frame Views - Elevations"; break;
                case "twiSourceReport_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Report_3D_Scene; selectedSource = "Report - Frame Views - Roof"; break;
                case "twiSourceReport_Joints": selectedIndex = (int)EDisplayOptionsTypes.Report_Joints; selectedSource = "Report - Joints"; break;
                case "twiSourceReport_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Report_Foundations; selectedSource = "Report - Foundations"; break;
                case "twiSourceLayouts_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Layouts_3D_Scene; selectedSource = "Layouts 3D scene"; break;
                case "twiSourceLayouts_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Elevations; selectedSource = "Layouts - Frame Views - Elevations"; break;
                case "twiSourceLayouts_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Roof; selectedSource = "Layouts - Frame Views - Roof"; break;
                case "twiSourceLayouts_FW_Frames": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Frames; selectedSource = "Layouts - Frame Views - Frames"; break;
                case "twiSourceLayouts_FW_Columns": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Columns; selectedSource = "Layouts - Frame Views - Columns"; break;
                case "twiSourceLayouts_FW_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Foundations; selectedSource = "Layouts - Frame Views - Foundations"; break;
                case "twiSourceLayouts_FW_Floor": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Floor; selectedSource = "Layouts - Frame Views - Floor"; break;
                case "twiSourceLayouts_CW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Elevations; selectedSource = "Layouts - Cladding Views - Elevations"; break;
                case "twiSourceLayouts_CW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Roof; selectedSource = "Layouts - Cladding Views - Roof"; break;
                case "twiSourceLayouts_Joints": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Joints; selectedSource = "Layouts - Joints"; break;
                case "twiSourceLayouts_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Foundations; selectedSource = "Layouts - Foundations"; break;
            }

            return selectedIndex;
        }

        private List<int> GetSelectedDestinationIndexes()
        {
            List<int> destIndexes = new List<int>();

            if (chGUI_3D_scene.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.GUI_3D_Scene);
            if (chGUI_Joint_Preview.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.GUI_Joint_Preview);
            if (chGUI_Foundation_Preview.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.GUI_Foundation_Preview);
            if (chGUI_Accessories_Preview.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.GUI_Accessories_Preview);
            if (chReport_3D_scene.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Report_3D_Scene);
            if (chReport_FW_Elevations.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Report_FW_Elevations);
            if (chReport_FW_Roof.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Report_FW_Roof);
            if (chReport_Joints.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Report_Joints);
            if (chReport_Foundations.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Report_Foundations);
            if (chLayouts_3D_scene.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_3D_Scene);
            if (chLayouts_FW_Elevations.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_FW_Elevations);
            if (chLayouts_FW_Roof.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_FW_Roof);
            if (chLayouts_FW_Frames.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_FW_Frames);
            if (chLayouts_FW_Columns.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_FW_Columns);
            if (chLayouts_FW_Foundations.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_FW_Foundations);
            if (chLayouts_FW_Floor.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_FW_Floor);
            if (chLayouts_CW_Elevations.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_CW_Elevations);
            if (chLayouts_CW_Roof.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_CW_Roof);
            if (chLayouts_Joints.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_Joints);
            if (chLayouts_Foundations.IsChecked == true) destIndexes.Add((int)EDisplayOptionsTypes.Layouts_Foundations);
            
            return destIndexes;
        }


        private void SetDestDisabled(EDisplayOptionsTypes optsType)
        {
            if (!IsLoaded) return;

            SetAllEnabled();

            if (optsType == EDisplayOptionsTypes.GUI_3D_Scene)
            {
                chGUI_3D_scene.IsChecked = false;
                chGUI_3D_scene.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.GUI_Joint_Preview)
            {
                chGUI_Joint_Preview.IsChecked = false;
                chGUI_Joint_Preview.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.GUI_Foundation_Preview)
            {
                chGUI_Foundation_Preview.IsChecked = false;
                chGUI_Foundation_Preview.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.GUI_Accessories_Preview)
            {
                chGUI_Accessories_Preview.IsChecked = false;
                chGUI_Accessories_Preview.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Report_3D_Scene)
            {
                chReport_3D_scene.IsChecked = false;
                chReport_3D_scene.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Report_FW_Elevations)
            {
                chReport_FW_Elevations.IsChecked = false;
                chReport_FW_Elevations.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Report_FW_Roof)
            {
                chReport_FW_Roof.IsChecked = false;
                chReport_FW_Roof.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Report_Joints)
            {
                chReport_Joints.IsChecked = false;
                chReport_Joints.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Report_Foundations)
            {
                chReport_Foundations.IsChecked = false;
                chReport_Foundations.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_3D_Scene)
            {
                chLayouts_3D_scene.IsChecked = false;
                chLayouts_3D_scene.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_FW_Elevations)
            {
                chLayouts_FW_Elevations.IsChecked = false;
                chLayouts_FW_Elevations.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_FW_Roof)
            {
                chLayouts_FW_Roof.IsChecked = false;
                chLayouts_FW_Roof.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_FW_Frames)
            {
                chLayouts_FW_Frames.IsChecked = false;
                chLayouts_FW_Frames.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_FW_Columns)
            {
                chLayouts_FW_Columns.IsChecked = false;
                chLayouts_FW_Columns.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_FW_Foundations)
            {
                chLayouts_FW_Foundations.IsChecked = false;
                chLayouts_FW_Foundations.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_FW_Floor)
            {
                chLayouts_FW_Floor.IsChecked = false;
                chLayouts_FW_Floor.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_CW_Elevations)
            {
                chLayouts_CW_Elevations.IsChecked = false;
                chLayouts_CW_Elevations.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_CW_Roof)
            {
                chLayouts_CW_Roof.IsChecked = false;
                chLayouts_CW_Roof.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_Joints)
            {
                chLayouts_Joints.IsChecked = false;
                chLayouts_Joints.IsEnabled = false;
            }
            else if (optsType == EDisplayOptionsTypes.Layouts_Foundations)
            {
                chLayouts_Foundations.IsChecked = false;
                chLayouts_Foundations.IsEnabled = false;
            }
        }

        private void SetAllEnabled()
        {
            chGUI_3D_scene.IsEnabled = true;
            chGUI_Joint_Preview.IsEnabled = true;
            chGUI_Foundation_Preview.IsEnabled = true;
            chGUI_Accessories_Preview.IsEnabled = true;
            chReport_3D_scene.IsEnabled = true;
            chReport_FW_Elevations.IsEnabled = true;
            chReport_FW_Roof.IsEnabled = true;
            chReport_Joints.IsEnabled = true;
            chReport_Foundations.IsEnabled = true;
            chLayouts_3D_scene.IsEnabled = true;
            chLayouts_FW_Elevations.IsEnabled = true;
            chLayouts_FW_Roof.IsEnabled = true;
            chLayouts_FW_Frames.IsEnabled = true;
            chLayouts_FW_Columns.IsEnabled = true;
            chLayouts_FW_Foundations.IsEnabled = true;
            chLayouts_FW_Floor.IsEnabled = true;
            chLayouts_CW_Elevations.IsEnabled = true;
            chLayouts_CW_Roof.IsEnabled = true;
            chLayouts_Joints.IsEnabled = true;
            chLayouts_Foundations.IsEnabled = true;
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            int sourceIndex = GetSelectedSourceOptions();
            if (sourceIndex == -1) { MessageBox.Show("Select source please."); return; }
            List<int> destIndexes = GetSelectedDestinationIndexes();
            if (destIndexes.Count == 0) { MessageBox.Show("Select at least one destination."); return; }

            DisplayOptionsViewModel sourceVM = _pfdVM._displayOptionsVM.DisplayOptionsList.ElementAtOrDefault(sourceIndex);

            List<DisplayOptionsViewModel> destVMs = new List<DisplayOptionsViewModel>();
            foreach (int index in destIndexes)
                destVMs.Add(_pfdVM._displayOptionsVM.DisplayOptionsList.ElementAtOrDefault(index));

            MessageBoxResult result = MessageBox.Show($"Do you really want to copy settings from [{selectedSource}] to the selected items on the right?", "Information", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                foreach (DisplayOptionsViewModel destVM in destVMs)
                {
                    //tu sa treba trosku zamysliet, lebo pokial setnem viewmodel pre GUI na IsExport = true a neda sa to nastavit potom inak, tak je to vlastne pruser
                    destVM.SetViewModel(sourceVM);

                    OptionsChanged = true;
                }
                if (OptionsChanged && destIndexes.Contains((int)EDisplayOptionsTypes.GUI_3D_Scene)) GUI3DSceneOptionsChanged = true;

                MessageBox.Show("Settings were succesfully copied.");
            }
        }

        private void TreeViewSource_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int selectedIndex = -1;

            switch (((TreeViewItem)e.NewValue).Name.ToString())
            {
                case "twiSourceGUI_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.GUI_3D_Scene; Title = "Copying from: GUI 3D scene"; break;
                case "twiSourceGUI_Joint_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Joint_Preview; Title = "Copying from: GUI - Joint Preview"; break;
                case "twiSourceGUI_Foundation_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Foundation_Preview; Title = "Copying from: GUI - Foundation Preview"; break;
                case "twiSourceGUI_Accessories_Preview": selectedIndex = (int)EDisplayOptionsTypes.GUI_Accessories_Preview; Title = "Copying from: GUI Accessories Preview"; break;
                case "twiSourceReport_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Report_3D_Scene; Title = "Copying from: Report 3D scene"; break;
                case "twiSourceReport_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Report_FW_Elevations; Title = "Copying from: Report - Frame Views - Elevations"; break;
                case "twiSourceReport_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Report_FW_Roof; Title = "Copying from: Report - Frame Views - Roof"; break;
                case "twiSourceReport_Joints": selectedIndex = (int)EDisplayOptionsTypes.Report_Joints; Title = "Copying from: Report - Joints"; break;
                case "twiSourceReport_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Report_Foundations; Title = "Copying from: Report - Foundations"; break;
                case "twiSourceLayouts_3D_scene": selectedIndex = (int)EDisplayOptionsTypes.Layouts_3D_Scene; Title = "Copying from: Layouts 3D scene"; break;
                case "twiSourceLayouts_FW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Elevations; Title = "Copying from: Layouts - Frame Views - Elevations"; break;
                case "twiSourceLayouts_FW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Roof; Title = "Copying from: Layouts - Frame Views - Roof"; break;
                case "twiSourceLayouts_FW_Frames": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Frames; Title = "Copying from: Layouts - Frame Views - Frames"; break;
                case "twiSourceLayouts_FW_Columns": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Columns; Title = "Copying from: Layouts - Frame Views - Columns"; break;
                case "twiSourceLayouts_FW_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Foundations; Title = "Copying from: Layouts - Frame Views - Foundations"; break;
                case "twiSourceLayouts_FW_Floor": selectedIndex = (int)EDisplayOptionsTypes.Layouts_FW_Floor; Title = "Copying from: Layouts - Frame Views - Floor"; break;
                case "twiSourceLayouts_CW_Elevations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Elevations; Title = "Copying from: Layouts - Cladding Views - Elevations"; break;
                case "twiSourceLayouts_CW_Roof": selectedIndex = (int)EDisplayOptionsTypes.Layouts_CW_Roof; Title = "Copying from: Layouts - Cladding Views - Roof"; break;
                case "twiSourceLayouts_Joints": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Joints; Title = "Copying from: Layouts - Joints"; break;
                case "twiSourceLayouts_Foundations": selectedIndex = (int)EDisplayOptionsTypes.Layouts_Foundations; Title = "Copying from: Layouts - Foundations"; break;
            }

            if(selectedIndex != -1) SetDestDisabled((EDisplayOptionsTypes)selectedIndex);
        }
    }
}
