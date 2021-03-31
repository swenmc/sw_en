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

namespace PFD
{
    public partial class UC_DoorsAndWindows : UserControl
    {
        private CPFDViewModel _pfdVM;
        private bool DoorsAndWindowsOptionsChanged = false;
        private bool ErrorDetected = false;

        //private bool RecreateModelRequired = false;
        public UC_DoorsAndWindows(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            DoorsAndWindowsOptionsChanged = false;

            if (_pfdVM._doorsAndWindowsVM == null)
            {
                _pfdVM._doorsAndWindowsVM = new DoorsAndWindowsViewModel();
                SetDefaultFlashings();
                SetDefaultDownpipes();
                SetFlashingsNames();
            }
            

            _pfdVM._doorsAndWindowsVM.PropertyChanged += HandleDoorsAndWindowsOptionsPropertyChangedEvent;

            this.DataContext = _pfdVM._doorsAndWindowsVM;
        }

        private void HandleDoorsAndWindowsOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is DoorsAndWindowsViewModel)
            {
                if (e.PropertyName == "DoorBlocksProperties")
                {
                    //_pfdVM.RecreateModel = true;
                    //_pfdVM.RecreateJoints = true;
                    //_pfdVM.RecreateFloorSlab = true;
                }
                else if (e.PropertyName == "DoorBlocksProperties_CollectionChanged")
                {
                    //_pfdVM.RecreateModel = true;
                    //_pfdVM.RecreateJoints = true;
                    //_pfdVM.RecreateFloorSlab = true;
                }
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
                    //_pfdVM.RecreateModel = true;
                    if (_pfdVM._modelOptionsVM.SameColorsDoor) _pfdVM._doorsAndWindowsVM.SetAllDoorCoatingColorAccordingTo(doorProperties);
                }
                else
                {
                    Datagrid_DoorsAndGates_SelectionChanged(null, null);
                    //_pfdVM.RecreateModel = true;
                    //_pfdVM.RecreateJoints = true;
                    //_pfdVM.RecreateFloorSlab = true;
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

                DoorsAndWindowsOptionsChanged = true;
            }
        }

        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DoorsAndWindowsOptionsChanged)
            {
                _pfdVM.DoorsAndWindowsChanged = true;
            }
            DoorsAndWindowsOptionsChanged = false;
        }

        


        
        private void BtnApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorDetected) { return; }

            if (DoorsAndWindowsOptionsChanged)
            {
                _pfdVM.DoorsAndWindowsChanged = true;
            }
            DoorsAndWindowsOptionsChanged = false;
        }


        public void SetDefaultFlashings()
        {
            float fRoofSideLength = 0;

            if (_pfdVM.Model is CModel_PFD_01_MR)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(_pfdVM.Height_H2_Overall - _pfdVM.WallHeightOverall) + MathF.Pow2(_pfdVM.WidthOverall)); // Dlzka hrany strechy
            }
            else if (_pfdVM.Model is CModel_PFD_01_GR)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(_pfdVM.Height_H2_Overall - _pfdVM.WallHeightOverall) + MathF.Pow2(0.5f * _pfdVM.WidthOverall)); // Dlzka hrany strechy
            }
            else
            {
                // Exception - not implemented
                fRoofSideLength = 0;
            }

            float fRoofRidgeFlashing_TotalLength = 0;
            float fWallCornerFlashing_TotalLength = 0;
            float fBargeFlashing_TotalLength = 0;

            if (_pfdVM.Model is CModel_PFD_01_MR)
            {
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 2 * _pfdVM.WallHeightOverall + 2 * _pfdVM.Height_H2_Overall;
                fBargeFlashing_TotalLength = 2 * fRoofSideLength;
            }
            else if (_pfdVM.Model is CModel_PFD_01_GR)
            {
                fRoofRidgeFlashing_TotalLength = _pfdVM.LengthOverall;
                fWallCornerFlashing_TotalLength = 4 * _pfdVM.WallHeightOverall;
                fBargeFlashing_TotalLength = 4 * fRoofSideLength;
            }
            else
            {
                // Exception - not implemented
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 0;
                fBargeFlashing_TotalLength = 0;
            }

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            float fPADoorTrimmerFlashing_TotalLength = 0;
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

            ObservableCollection<CAccessories_LengthItemProperties> flashings = new ObservableCollection<CAccessories_LengthItemProperties>();

            if (_pfdVM.KitsetTypeIndex != 0)
            {
                flashings.Add(new CAccessories_LengthItemProperties("Roof Ridge", "Flashings", fRoofRidgeFlashing_TotalLength, 2));
            }

            flashings.Add(new CAccessories_LengthItemProperties("Wall Corner", "Flashings", fWallCornerFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Barge", "Flashings", fBargeFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Trimmer", "Flashings", fRollerDoorTrimmerFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header", "Flashings", fRollerDoorLintelFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header Cap", "Flashings", fRollerDoorLintelCapFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("PA Door Trimmer", "Flashings", fPADoorTrimmerFlashing_TotalLength, 18));
            flashings.Add(new CAccessories_LengthItemProperties("PA Door Header", "Flashings", fPADoorLintelFlashing_TotalLength, 18));
            flashings.Add(new CAccessories_LengthItemProperties("Window", "Flashings", fWindowFlashing_TotalLength, 9));
            _pfdVM._doorsAndWindowsVM.Flashings = flashings;

            SetFlashingsNames();
        }

        public void SetDefaultDownpipes()
        {
            // Zatial bude natvrdo jeden riadok s poctom zvodov, prednastavenou dlzkou ako vyskou steny a farbou, rovnaky default ako gutter
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

            CAccessories_DownpipeProperties downpipe = new CAccessories_DownpipeProperties("RP80®", iCountOfDownpipePoints, fDownpipesTotalLength, 2);
            //downpipe.PropertyChanged += AccessoriesItem_PropertyChanged;
            _pfdVM._doorsAndWindowsVM.Downpipes = new ObservableCollection<CAccessories_DownpipeProperties>() { downpipe };
        }

        


        int actualPreview = 0;
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

            DisplayOptions displayOptions = _pfdVM.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            displayOptions.bDisplayGlobalAxis = false;
            displayOptions.RotateModelX = -90;
            displayOptions.RotateModelY = 20;
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
            }
            RedrawWindowPreview();
        }

        private void RedrawWindowPreview()
        {
            ////Mato??? - tieto komenty dole su aktualne? Lebo task 266 je uzavrety.
            //// TO Ondrej - no mne sa zda ze preberame z bloku WindowProperties props
            //// ale dalsie parametre samotneho bay, objekty stlpov, girts, odstup od stlpov atd tu nastavujem natvrdo,
            //// Mali by sa tiez dat urcit z parametrov globalneho modelu
            //// Tieto parametre sa totiz nastavuju pri vytvarani objektu bloku v CModel_01_PFD
            //// vid metoda DeterminateBasicPropertiesToInsertBlock

            ////------------------------------------------------
            //// TODO 266 - prevziat parametre girt a columns zo skutocneho modelu, resp. zvoleneho bloku
            ////
            //// Tento kod by mal byt zmazany
            //// Girt
            //CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(1, 0.27f, 0.07f, 0.00115f, Colors.Orange);
            //CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
            //CMember refgirt = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, 1, 0, 0), crsc, 0);
            //refgirt.EccentricityStart = eccentricity;
            //refgirt.EccentricityEnd = eccentricity;
            //refgirt.DTheta_x = Math.PI / 2;

            //CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0, Colors.Green);
            //CMember mColumnLeft = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);
            //CMember mColumnRight = new CMember(0, new CNode(0, 1, 0, 0, 0), new CNode(1, 1, 0, 5, 0), crscColumn, 0);
            ////------------------------------------------------

            //WindowProperties props = null;
            //if (Datagrid_Windows.SelectedIndex != -1) props = _pfdVM._doorsAndWindowsVM.WindowBlocksProperties.ElementAtOrDefault(Datagrid_Windows.SelectedIndex);
            //if (props == null) props = _pfdVM._doorsAndWindowsVM.WindowBlocksProperties.FirstOrDefault();

            //// TODO 266 - vsetky vstupne parametre konstruktora CBlock_3D_002_WindowInBay by sa mali prevziat z existujuceho bloku podla toho ktory riadok datagridu je selektovany
            //// V podstate by sme nemali tento blok vytvarat nanovo, ale len prevziat parametre bloku z hlavneho modelu (to asi teraz nie je dostupne)
            //// Prva moznost je ze si budeme bloky ukladat niekam do CModel_PFD_01_GR a potom ich tu len zobrazime podla vybraneho riadku v datagride.

            //// Druha moznost je vytvorit konrektny zobrazovany blok znova.
            //// V tom pripade by sme potrebovali zavolat cast metody CModel_PFD_01_GR, AddWindowBlock, tj. 
            //// 1. Nastavia sa vstupne parametre podla polohy bloku DeterminateBasicPropertiesToInsertBlock
            //// 2. Vyrobi sa blok window = new CBlock_3D_001_WindowInBay(....)

            //CModel model;
            //if (props == null) model = new CModel();
            //else model = new CBlock_3D_002_WindowInBay(props, 0.5f, 0.3f, 0.8f, refgirt, mColumnLeft, mColumnRight, 6.0f, 2.8f, 0.3f);

            CModel_PFD modelPFD = _pfdVM.Model as CModel_PFD;
            if (modelPFD.WindowsModels == null) return;

            int index = Datagrid_Windows.SelectedIndex;
            if (index < 0) index = 0;
            CModel model = modelPFD.WindowsModels.ElementAtOrDefault(index);
            if (model == null) model = modelPFD.WindowsModels.FirstOrDefault();
            if (model == null) model = new CModel();


            DisplayOptions displayOptions = _pfdVM.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            displayOptions.bDisplayGlobalAxis = false;
            displayOptions.RotateModelX = -90;
            displayOptions.RotateModelY = 20;
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

        //private void Datagrid_DoorsAndGates_AddingNewItem(object sender, AddingNewItemEventArgs e)
        //{
        //    Frame1.UpdateLayout();  // Nutne kvôli pridaniu riadku a update v GUI
        //}

        //private void Datagrid_Windows_AddingNewItem(object sender, AddingNewItemEventArgs e)
        //{
        //    Frame1.UpdateLayout(); // Nutne kvôli pridaniu riadku a update v GUI
        //}

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
            _pfdVM.RecreateQuotation = true;
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
            _pfdVM.RecreateQuotation = true;
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
            _pfdVM.RecreateQuotation = true;
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

        private void SetFlashingsNames()
        {
            if (_pfdVM.KitsetTypeIndex == 0)
            {
                _pfdVM._doorsAndWindowsVM.FlashingsNames = new List<string>() { "Wall Corner", "Barge", "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        "PA Door Trimmer",  "PA Door Header", "Window"};
            }
            else
            {
                _pfdVM._doorsAndWindowsVM.FlashingsNames = new List<string>() { "Roof Ridge", "Roof Ridge (Soft Edge)", "Wall Corner", "Barge", "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        "PA Door Trimmer",  "PA Door Header", "Window"};
            }
        }
    }
}
