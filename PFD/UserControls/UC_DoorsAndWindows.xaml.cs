using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DATABASE.DTO;
using MATH;
using PFD.Infrastructure;

namespace PFD
{
    public partial class UC_DoorsAndWindows : UserControl
    {
        private CPFDViewModel _pfdVM;
        private bool DoorsAndWindowsOptionsChanged = false;        

        public UC_DoorsAndWindows(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            DoorsAndWindowsOptionsChanged = false;

            _pfdVM._doorsAndWindowsVM.PropertyChanged += HandleDoorsAndWindowsOptionsPropertyChangedEvent;

            this.DataContext = _pfdVM._doorsAndWindowsVM;

            RedrawDoorOrWindowPreview();
        }

        private void HandleDoorsAndWindowsOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DoorsAndWindowsViewModel)
            {
                if (e.PropertyName == "Flashings_CollectionChanged")
                {
                    SetAccessoriesButtonsVisibility();                    
                }
                if (e.PropertyName == "Gutters_CollectionChanged") SetAccessoriesButtonsVisibility();
                if (e.PropertyName == "Downpipes_CollectionChanged") SetAccessoriesButtonsVisibility();

                DoorsAndWindowsOptionsChanged = true;
            }
            else if (sender is DoorProperties)
            {
                if (e.PropertyName == "Bays") return;
                if (e.PropertyName == "Series") return;
                if (e.PropertyName == "Serie") return;
                if (e.PropertyName == "SerieEnabled") return;

                DoorProperties doorProperties = sender as DoorProperties;
                if (doorProperties.IsSetFromCode) return;

                if (e.PropertyName == "CoatingColor")
                {
                    //recreate model after color changed                    
                    if (_pfdVM._modelOptionsVM.SameColorsDoor) _pfdVM._doorsAndWindowsVM.SetAllDoorCoatingColorAccordingTo(doorProperties);
                }
                else
                {
                    Datagrid_DoorsAndGates_SelectionChanged(null, null);
                }

                List<FibreglassProperties> collisions = CDoorsAndWindowsHelper.GetFibreglassCollisionsWithDoors(_pfdVM, doorProperties);
                if (collisions.Count > 0)
                {
                    MessageBox.Show($"We found {collisions.Count} collisions with doors. Fibreglass in collision will be removed.", "Attention");
                    foreach (FibreglassProperties collision_fp in collisions) _pfdVM._claddingOptionsVM.FibreglassProperties.Remove(collision_fp);
                }

                DoorsAndWindowsOptionsChanged = true;
            }
            else if (sender is WindowProperties)
            {
                if (e.PropertyName == "Bays") return;
                WindowProperties wProperties = sender as WindowProperties;
                if (wProperties.IsSetFromCode) return;

                Datagrid_Windows_SelectionChanged(null, null);
                //_pfdVM.RecreateModel = true;
                //_pfdVM.RecreateJoints = true;

                List<FibreglassProperties> collisions = CDoorsAndWindowsHelper.GetFibreglassCollisionsWithWindows(_pfdVM, wProperties);
                if (collisions.Count > 0)
                {
                    MessageBox.Show($"We found {collisions.Count} collisions with windows. Fibreglass in collision will be removed.", "Attention");
                    foreach (FibreglassProperties collision_fp in collisions) _pfdVM._claddingOptionsVM.FibreglassProperties.Remove(collision_fp);
                }

                DoorsAndWindowsOptionsChanged = true;
            }
            else if (sender is CAccessories_LengthItemProperties)
            {
                if (e.PropertyName == "CoatingColor")
                {
                    CAccessories_LengthItemProperties flashing = sender as CAccessories_LengthItemProperties;
                    _pfdVM._doorsAndWindowsVM.SetFlashingsCoatingColorAccordingTo(flashing);
                }
                DoorsAndWindowsOptionsChanged = true;
            }
            else if (sender is CAccessories_DownpipeProperties)
            {
                DoorsAndWindowsOptionsChanged = true;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            BtnApplyChanges_Click(sender, e);            
        }
        
        private void BtnApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (DoorsAndWindowsOptionsChanged)
            {
                _pfdVM.DoorsAndWindowsChanged = true;
            }
            DoorsAndWindowsOptionsChanged = false;
            previewDoorIndex = int.MinValue;
            previewWindowIndex = int.MinValue;
            RedrawDoorOrWindowPreview();
        }


        int actualPreview = 0;
        int previewDoorIndex = int.MinValue;
        int previewWindowIndex = int.MinValue;
        private void RedrawDoorOrWindowPreview()
        {
            if (actualPreview == 2) RedrawWindowPreview();
            else RedrawDoorPreview();
        }

        private void Datagrid_DoorsAndGates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (!Datagrid_DoorsAndGates.IsLoaded) return;
            if (e != null && e.Source != null)
            {
                DataGrid dg = e.Source as DataGrid;
                if (!dg.IsLoaded) return;
                if (!dg.IsMouseOver) return;
                if (dg.SelectedIndex == previewDoorIndex) return;
                else previewDoorIndex = dg.SelectedIndex;
            }
            RedrawDoorPreview();
        }
        private void RedrawDoorPreview()
        {
            CModel_PFD modelPFD = _pfdVM.Model as CModel_PFD;

            if (modelPFD.DoorsModels == null) return;
            //if (modelPFD.DoorsModels.Count == 0) return;

            int index = Datagrid_DoorsAndGates.SelectedIndex;
            if (index < 0) index = 0;
            CModel doorModel = modelPFD.DoorsModels.ElementAtOrDefault(index);
            if (doorModel == null) doorModel = modelPFD.DoorsModels.FirstOrDefault();
            if (doorModel == null) doorModel = new CModel();

            DisplayOptions displayOptions = _pfdVM.GetDisplayOptions(EDisplayOptionsTypes.GUI_Accessories_Preview);
            
            //Page3Dmodel page3D = new Page3Dmodel(doorModel, displayOptions, null);
            Page3Dmodel page3D = new Page3Dmodel(doorModel, displayOptions, EModelType.eJoint);

            // Display model in 3D preview frame
            FrameDoorWindowPreview3D.Content = page3D;
            FrameDoorWindowPreview3D.UpdateLayout();
            actualPreview = 1;
        }

        private void Datagrid_Windows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && e.Source != null)
            {
                DataGrid dg = e.Source as DataGrid;
                if (!dg.IsLoaded) return;
                if (!dg.IsMouseOver) return;
                if (dg.SelectedIndex == previewWindowIndex) return;
                else previewWindowIndex = dg.SelectedIndex;
            }
            RedrawWindowPreview();
        }
        private void RedrawWindowPreview()
        {
            CModel_PFD modelPFD = _pfdVM.Model as CModel_PFD;
            if (modelPFD.WindowsModels == null) return;

            int index = Datagrid_Windows.SelectedIndex;
            if (index < 0) index = 0;
            CModel model = modelPFD.WindowsModels.ElementAtOrDefault(index);
            if (model == null) model = modelPFD.WindowsModels.FirstOrDefault();
            if (model == null) model = new CModel();

            DisplayOptions displayOptions = _pfdVM.GetDisplayOptions(EDisplayOptionsTypes.GUI_Accessories_Preview);
            
            //Page3Dmodel page3D = new Page3Dmodel(model, displayOptions, null);
            Page3Dmodel page3D = new Page3Dmodel(model, displayOptions, EModelType.eJoint);

            // Display model in 3D preview frame
            FrameDoorWindowPreview3D.Content = page3D;
            FrameDoorWindowPreview3D.UpdateLayout();
            actualPreview = 2;
        }

        private void Datagrid_DoorsAndGates_GotFocus(object sender, RoutedEventArgs e)
        {
            if (actualPreview != 1) Datagrid_DoorsAndGates_SelectionChanged(sender, null);
        }
        private void Datagrid_Windows_GotFocus(object sender, RoutedEventArgs e)
        {
            if (actualPreview != 2) Datagrid_Windows_SelectionChanged(sender, null);
        }
        private void FrameDoorWindowPreview3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        //GENERATORS
        private void BtnDoorGenerator_Click(object sender, RoutedEventArgs e)
        {
            DoorGeneratorWindow generatorWindow = new DoorGeneratorWindow(_pfdVM.Frames - 1, _pfdVM.IFrontColumnNoInOneFrame + 1);
            generatorWindow.ShowDialog();

            DoorGeneratorViewModel doorGeneratorViewModel = generatorWindow.DataContext as DoorGeneratorViewModel;
            if (doorGeneratorViewModel == null) return;
            if (doorGeneratorViewModel.AddDoors)
            {
                List<DoorProperties> doorProperties = generatorWindow.GetDoorProperties();
                List<DoorProperties> validatedDoorProperties = new List<DoorProperties>();
                bool errorOccurs = false;
                foreach (DoorProperties dp in doorProperties)
                {
                    //task 600
                    //dp.SetValidationValues(_pfdVM.WallHeight, _pfdVM.Model.fL1_frame, _pfdVM.Model.fDist_FrontColumns, _pfdVM.Model.fDist_BackColumns);
                    dp.SetValidationValues(_pfdVM.WallHeight, _pfdVM.Model.GetBayWidth(dp.iBayNumber), _pfdVM.Model.fDist_FrontColumns, _pfdVM.Model.fDist_BackColumns);
                    if (!dp.ValidateDoorInsideBay())
                    {
                        if (!errorOccurs) MessageBox.Show("Door is defined out of frame bay.");
                        errorOccurs = true;
                        continue;
                    }

                    validatedDoorProperties.Add(dp);
                }
                if (validatedDoorProperties.Count != doorProperties.Count) doorProperties = validatedDoorProperties;

                if (doorProperties.Count == 0) return;


                foreach (DoorProperties dp in _pfdVM._doorsAndWindowsVM.DoorBlocksProperties)
                {
                    //dp.PropertyChanged -= null;
                    bool existsSameItem = doorProperties.Exists(p => p.iBayNumber == dp.iBayNumber && p.sBuildingSide == dp.sBuildingSide);
                    if (!existsSameItem) doorProperties.Add(dp);
                }

                //door and windows collision detection
                List<WindowProperties> windowProperties = new List<WindowProperties>();
                bool doorWindowColision = false;
                foreach (WindowProperties wp in _pfdVM._doorsAndWindowsVM.WindowBlocksProperties)
                {
                    bool existsSameItem = doorProperties.Exists(p => p.iBayNumber == wp.iBayNumber && p.sBuildingSide == wp.sBuildingSide);
                    if (!existsSameItem) windowProperties.Add(wp);
                    else doorWindowColision = true;
                }
                if (doorWindowColision)
                {
                    _pfdVM.IsSetFromCode = true;
                    _pfdVM._doorsAndWindowsVM.WindowBlocksProperties = new ObservableCollection<WindowProperties>(windowProperties);
                    _pfdVM.IsSetFromCode = false;
                }
                
                _pfdVM._doorsAndWindowsVM.DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorProperties);
                if (_pfdVM._modelOptionsVM.SameColorsDoor) _pfdVM._doorsAndWindowsVM.SetAllDoorCoatingColorToSame();

                List<FibreglassProperties> collisionsWithFG = CDoorsAndWindowsHelper.GetCollisionsWithDoorsOrWindows(_pfdVM);                
                if (collisionsWithFG.Count > 0)
                {
                    MessageBox.Show($"We found {collisionsWithFG.Count} collisions with doors or windows. Fibreglass in collision will be removed.", "Attention");
                    foreach (FibreglassProperties collision_fp in collisionsWithFG) _pfdVM._claddingOptionsVM.FibreglassProperties.Remove(collision_fp);
                }
            }
            else if (doorGeneratorViewModel.DeleteDoors)
            {
                List<DoorProperties> doorsToDelete = generatorWindow.GetDoorsToDelete();
                if (doorsToDelete.Count == 0) return;

                List<DoorProperties> doorProperties = new List<DoorProperties>();

                foreach (DoorProperties dp in _pfdVM._doorsAndWindowsVM.DoorBlocksProperties)
                {
                    bool isTheItemToDelete = doorsToDelete.Exists(p => p.iBayNumber == dp.iBayNumber && p.sBuildingSide == dp.sBuildingSide);
                    if (!isTheItemToDelete) doorProperties.Add(dp);
                }
                _pfdVM._doorsAndWindowsVM.DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorProperties);
            }
        }
        private void BtnWindowsGenerator_Click(object sender, RoutedEventArgs e)
        {
            WindowsGeneratorWindow generatorWindow = new WindowsGeneratorWindow(_pfdVM.Frames - 1, _pfdVM.IFrontColumnNoInOneFrame + 1, _pfdVM.WallHeight, _pfdVM.BayWidth, _pfdVM.ColumnDistance);
            generatorWindow.ShowDialog();

            WindowGeneratorViewModel windowGeneratorViewModel = generatorWindow.DataContext as WindowGeneratorViewModel;
            if (windowGeneratorViewModel == null) return;

            if (windowGeneratorViewModel.AddWindows)
            {
                List<WindowProperties> windowProperties = generatorWindow.GetWindowsProperties();
                List<WindowProperties> validatedWindowProperties = new List<WindowProperties>();
                bool errorOccurs = false;
                foreach (WindowProperties wp in windowProperties)
                {
                    //task 600
                    //wp.SetValidationValues(_pfdVM.WallHeight, _pfdVM.Model.fL1_frame, _pfdVM.Model.fDist_FrontColumns, _pfdVM.Model.fDist_BackColumns);
                    wp.SetValidationValues(_pfdVM.WallHeight, _pfdVM.Model.GetBayWidth(wp.iBayNumber), _pfdVM.Model.fDist_FrontColumns, _pfdVM.Model.fDist_BackColumns);
                    if (!wp.ValidateWindowInsideBay())
                    {
                        if (!errorOccurs) MessageBox.Show("Window is defined out of frame bay.");
                        errorOccurs = true;
                        continue;
                    }
                    validatedWindowProperties.Add(wp);
                }
                if (validatedWindowProperties.Count != windowProperties.Count) windowProperties = validatedWindowProperties;

                if (windowProperties.Count == 0) return;

                foreach (WindowProperties dp in _pfdVM._doorsAndWindowsVM.WindowBlocksProperties)
                {
                    bool existsSameItem = windowProperties.Exists(p => p.iBayNumber == dp.iBayNumber && p.sBuildingSide == dp.sBuildingSide);
                    if (!existsSameItem) windowProperties.Add(dp);
                }

                //door and windows collision detection
                List<DoorProperties> doorProperties = new List<DoorProperties>();
                bool doorWindowColision = false;
                foreach (DoorProperties dp in _pfdVM._doorsAndWindowsVM.DoorBlocksProperties)
                {
                    bool existsSameItem = windowProperties.Exists(p => p.iBayNumber == dp.iBayNumber && p.sBuildingSide == dp.sBuildingSide);
                    if (!existsSameItem) doorProperties.Add(dp);
                    else doorWindowColision = true;
                }
                if (doorWindowColision)
                {
                    _pfdVM.IsSetFromCode = true;
                    _pfdVM._doorsAndWindowsVM.DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorProperties);
                    _pfdVM.IsSetFromCode = false;
                }

                _pfdVM._doorsAndWindowsVM.WindowBlocksProperties = new ObservableCollection<WindowProperties>(windowProperties);

                List<FibreglassProperties> collisionsWithFG = CDoorsAndWindowsHelper.GetCollisionsWithDoorsOrWindows(_pfdVM);
                if (collisionsWithFG.Count > 0)
                {
                    MessageBox.Show($"We found {collisionsWithFG.Count} collisions with doors or windows. Fibreglass in collision will be removed.", "Attention");
                    foreach (FibreglassProperties collision_fp in collisionsWithFG) _pfdVM._claddingOptionsVM.FibreglassProperties.Remove(collision_fp);
                }
            }
            else if (windowGeneratorViewModel.DeleteWindows)
            {
                List<WindowProperties> windowsToDelete = generatorWindow.GetWindowsToDelete();
                if (windowsToDelete.Count == 0) return;

                List<WindowProperties> windowProperties = new List<WindowProperties>();

                foreach (WindowProperties wp in _pfdVM._doorsAndWindowsVM.WindowBlocksProperties)
                {
                    bool isTheItemToDelete = windowsToDelete.Exists(p => p.iBayNumber == wp.iBayNumber && p.sBuildingSide == wp.sBuildingSide);
                    if (!isTheItemToDelete) windowProperties.Add(wp);
                }
                _pfdVM._doorsAndWindowsVM.WindowBlocksProperties = new ObservableCollection<WindowProperties>(windowProperties);
            }
        }
        private void btnAddFlashing_Click(object sender, RoutedEventArgs e)
        {
            string flashingName = FindNotUsedFlashingName();
            if (string.IsNullOrEmpty(flashingName)) return;

            int colorIndex = 2;

            if (_pfdVM._modelOptionsVM.SameColorsFGD)
            {
                CoatingColour colour = _pfdVM._doorsAndWindowsVM.GetActual_FGD_Color();
                if (colour != null)
                {
                    CAccessories_LengthItemProperties p = new CAccessories_LengthItemProperties();
                    colorIndex = p.CoatingColors.IndexOf(colour);
                }
            }
            else if (_pfdVM._modelOptionsVM.SameColorsFlashings)
            {
                CAccessories_LengthItemProperties prop = _pfdVM._doorsAndWindowsVM.Flashings.FirstOrDefault();
                if (prop != null) colorIndex = prop.CoatingColors.IndexOf(prop.CoatingColor);
            }
            CAccessories_LengthItemProperties item = new CAccessories_LengthItemProperties(flashingName, "Flashings", 0, colorIndex);
            item.PropertyChanged += _pfdVM._doorsAndWindowsVM.FlashingsItem_PropertyChanged;
            _pfdVM._doorsAndWindowsVM.Flashings.Add(item);
            _pfdVM._doorsAndWindowsVM.CheckFlashingsColors();
        }
        private string FindNotUsedFlashingName()
        {
            foreach (string s in _pfdVM._doorsAndWindowsVM.FlashingsNames)
            {
                if (_pfdVM._doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.Name == s) != null) continue;

                if (s == "Roof Ridge")
                {
                    if (_pfdVM._doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.Name == "Roof Ridge (Soft Edge)") != null) continue;
                }

                if (s == "Roof Ridge (Soft Edge)")
                {
                    if (_pfdVM._doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.Name == "Roof Ridge") != null) continue;
                }

                return s;
            }
            return null;
        }
        private void btnAddGutter_Click(object sender, RoutedEventArgs e)
        {
            float fGuttersTotalLength = 2 * _pfdVM.LengthOverall; // na dvoch okrajoch strechy

            int colorIndex = 2;
            if (_pfdVM._modelOptionsVM.SameColorsFGD)
            {
                CoatingColour colour = _pfdVM._doorsAndWindowsVM.GetActual_FGD_Color();
                if (colour != null)
                {
                    CAccessories_LengthItemProperties p = new CAccessories_LengthItemProperties();
                    colorIndex = p.CoatingColors.IndexOf(colour);
                }
            }
            else if (_pfdVM._modelOptionsVM.SameColorsGutters)
            {
                CAccessories_LengthItemProperties prop = _pfdVM._doorsAndWindowsVM.Gutters.FirstOrDefault();
                if (prop != null) colorIndex = prop.CoatingColors.IndexOf(prop.CoatingColor);
            }

            CAccessories_LengthItemProperties item = new CAccessories_LengthItemProperties("Roof Gutter 430", "Gutters", fGuttersTotalLength, colorIndex);
            item.PropertyChanged += _pfdVM._doorsAndWindowsVM.AccessoriesItem_PropertyChanged;
            _pfdVM._doorsAndWindowsVM.Gutters.Add(item);
        }
        private void BtnAddDownpipe_Click(object sender, RoutedEventArgs e)
        {
            int iCountOfDownpipePoints = 0;
            float fDownpipesTotalLength = 0;

            if (_pfdVM.Model is CModel_PFD_01_MR)
            {
                iCountOfDownpipePoints = 2; // TODO - prevziat z GUI - 2 rohy budovy kde je nizsia vyska steny (H1 alebo H2)
                fDownpipesTotalLength = iCountOfDownpipePoints * Math.Min(_pfdVM.WallHeightOverall, _pfdVM.Height_H2_Overall); // Pocet zvodov krat vyska steny
            }
            else if (_pfdVM.Model is CModel_PFD_01_GR)
            {
                iCountOfDownpipePoints = 4; // TODO - prevziat z GUI - 4 rohy strechy
                fDownpipesTotalLength = iCountOfDownpipePoints * _pfdVM.WallHeightOverall; // Pocet zvodov krat vyska steny
            }
            else
            {
                // Exception - not implemented
                iCountOfDownpipePoints = 0;
                fDownpipesTotalLength = 0;
            }

            int colorIndex = 2;
            if (_pfdVM._modelOptionsVM.SameColorsFGD)
            {
                CoatingColour colour = _pfdVM._doorsAndWindowsVM.GetActual_FGD_Color();
                if (colour != null)
                {
                    CAccessories_DownpipeProperties p = new CAccessories_DownpipeProperties();
                    colorIndex = p.CoatingColors.IndexOf(colour);
                }
            }
            else if (_pfdVM._modelOptionsVM.SameColorsDownpipes)
            {
                CAccessories_DownpipeProperties prop = _pfdVM._doorsAndWindowsVM.Downpipes.FirstOrDefault();
                if (prop != null) colorIndex = prop.CoatingColors.IndexOf(prop.CoatingColor);
            }

            CAccessories_DownpipeProperties downpipe = new CAccessories_DownpipeProperties("RP80®", iCountOfDownpipePoints, fDownpipesTotalLength, colorIndex);
            downpipe.PropertyChanged += _pfdVM._doorsAndWindowsVM.AccessoriesItem_PropertyChanged;

            _pfdVM._doorsAndWindowsVM.Downpipes.Add(downpipe);
        }

        private void SetAccessoriesButtonsVisibility()
        {
            //if (_pfdVM.Flashings.Count >= 9) btnAddFlashing.Visibility = Visibility.Hidden;
            //else btnAddFlashing.Visibility = Visibility.Visible;

            //if (_pfdVM.Gutters.Count >= 1) btnAddGutter.Visibility = Visibility.Hidden;
            //else btnAddGutter.Visibility = Visibility.Visible;

            //if (_pfdVM.Downpipes.Count >= 1) btnAddDownpipe.Visibility = Visibility.Hidden;
            //else btnAddDownpipe.Visibility = Visibility.Visible;

            //2.moznost
            if (_pfdVM._doorsAndWindowsVM.Flashings.Count >= 9) btnAddFlashing.IsEnabled = false;
            else btnAddFlashing.IsEnabled = true;

            if (_pfdVM._doorsAndWindowsVM.Gutters.Count >= 1) btnAddGutter.IsEnabled = false;
            else btnAddGutter.IsEnabled = true;

            if (_pfdVM._doorsAndWindowsVM.Downpipes.Count >= 1) btnAddDownpipe.IsEnabled = false;
            else btnAddDownpipe.IsEnabled = true;
        }

        private void Datagrid_Flashings_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!_pfdVM._modelOptionsVM.EnableCladding) return; //ak nie je zapnute Cladding, tak nie je nutne mazat dvere po zmazani Flashing (814)

            if (e.Key == Key.Delete)
            {
                var grid = (DataGrid)sender;

                if (grid.SelectedItems.Count > 0)
                {
                    CAccessories_LengthItemProperties item = grid.SelectedItems[0] as CAccessories_LengthItemProperties;
                    DoorsAndWindowsViewModel vm = this.DataContext as DoorsAndWindowsViewModel;
                    if (vm.AreAnyDoorsOrWindowsWith(item))
                    {
                        var result = MessageBox.Show($"Do you really want to delete {item.Name}? Doors or windows with this flashing will be deleted from model.", "Warning", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.OK)
                        {
                            vm.DeleteDoorsOrWindowsWith(item);
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }

                    
                }
            }
        }
    }
}
