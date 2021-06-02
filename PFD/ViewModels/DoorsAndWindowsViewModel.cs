using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using CRSC;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;

namespace PFD
{
    [Serializable]
    public class DoorsAndWindowsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region private fields
        private ObservableCollection<DoorProperties> MDoorBlocksProperties;
        private ObservableCollection<WindowProperties> MWindowBlocksProperties;
        private List<string> MBuildingSides;
        private List<string> MDoorsTypes;
        
        private ObservableCollection<CAccessories_LengthItemProperties> m_Flashings;
        private List<string> m_FlashingsNames;

        private ObservableCollection<CAccessories_LengthItemProperties> m_Gutters;
        private List<string> m_GuttersNames;

        private ObservableCollection<CAccessories_DownpipeProperties> m_Downpipes;

        private bool m_IsCladdingEnabled;
        //private CAccessories_LengthItemProperties m_RemovedFlashing;
        #endregion private fields

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region Properties
        //-------------------------------------------------------------------------------------------------------------
        public ObservableCollection<DoorProperties> DoorBlocksProperties
        {
            get
            {
                if (MDoorBlocksProperties == null) MDoorBlocksProperties = new ObservableCollection<DoorProperties>();
                return MDoorBlocksProperties;
            }

            set
            {
                MDoorBlocksProperties = value;
                if (MDoorBlocksProperties == null) return;
                MDoorBlocksProperties.CollectionChanged += DoorBlocksProperties_CollectionChanged;
                foreach (DoorProperties d in MDoorBlocksProperties)
                {
                    d.PropertyChanged -= HandleDoorPropertiesPropertyChangedEvent;
                    d.PropertyChanged += HandleDoorPropertiesPropertyChangedEvent;
                }
                CheckFlashings();
                NotifyPropertyChanged("DoorBlocksProperties");
            }
        }

        public ObservableCollection<WindowProperties> WindowBlocksProperties
        {
            get
            {
                if (MWindowBlocksProperties == null) MWindowBlocksProperties = new ObservableCollection<WindowProperties>();
                return MWindowBlocksProperties;
            }

            set
            {
                MWindowBlocksProperties = value;
                if (MWindowBlocksProperties == null) return;
                MWindowBlocksProperties.CollectionChanged += WindowBlocksProperties_CollectionChanged;
                foreach (WindowProperties w in MWindowBlocksProperties)
                {
                    w.PropertyChanged -= HandleWindowPropertiesPropertyChangedEvent;
                    w.PropertyChanged += HandleWindowPropertiesPropertyChangedEvent;
                }
                //RecreateModel = true;
                //RecreateJoints = true;
                CheckFlashings();
                NotifyPropertyChanged("WindowBlocksProperties");
            }
        }

        public List<string> BuildingSides
        {
            get
            {
                if (MBuildingSides == null) MBuildingSides = new List<string>() { "Left", "Right", "Front", "Back" };
                return MBuildingSides;
            }
        }

        public List<string> DoorsTypes
        {
            get
            {
                if (MDoorsTypes == null) MDoorsTypes = new List<string>() { "Personnel Door", "Roller Door" };
                return MDoorsTypes;
            }
        }

        

        public ObservableCollection<CAccessories_LengthItemProperties> Flashings
        {
            get
            {                
                return m_Flashings;
            }

            set
            {
                if (value == null) return;
                m_Flashings = value;
                m_Flashings.CollectionChanged += Flashings_CollectionChanged;
                foreach (CAccessories_LengthItemProperties item in Flashings)
                {
                    item.PropertyChanged -= FlashingsItem_PropertyChanged;
                    item.PropertyChanged += FlashingsItem_PropertyChanged;
                }

                CheckFlashingsColors();
                NotifyPropertyChanged("Flashings");
            }
        }

        public ObservableCollection<CAccessories_LengthItemProperties> Gutters
        {
            get
            {                
                return m_Gutters;
            }

            set
            {
                if (value == null) return;
                m_Gutters = value;
                m_Gutters.CollectionChanged += Gutters_CollectionChanged;

                foreach (CAccessories_LengthItemProperties item in Gutters)
                {
                    item.PropertyChanged -= AccessoriesItem_PropertyChanged;
                    item.PropertyChanged += AccessoriesItem_PropertyChanged;
                }

                NotifyPropertyChanged("Gutters");
            }
        }

        public ObservableCollection<CAccessories_DownpipeProperties> Downpipes
        {
            get
            {                
                return m_Downpipes;
            }

            set
            {
                if (value == null) return;
                m_Downpipes = value;
                m_Downpipes.CollectionChanged += Downpipes_CollectionChanged;

                foreach (CAccessories_DownpipeProperties item in Downpipes)
                {
                    item.PropertyChanged -= DownpipeItem_PropertyChanged;
                    item.PropertyChanged += DownpipeItem_PropertyChanged;
                }

                NotifyPropertyChanged("Downpipes");
            }
        }

        public List<string> FlashingsNames
        {
            get
            {
                return m_FlashingsNames;
            }
            set
            {
                m_FlashingsNames = value;
                NotifyPropertyChanged("FlashingsNames");
            }
        }
        public List<string> AllFlashingsNames
        {
            get
            {
                return new List<string>() { "Roof Ridge", "Roof Ridge (Soft Edge)", "Wall Corner", "Barge", "Barge Birdproof", "Eave Purlin Birdproof Strip",
                    "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        /*"PA Door Trimmer",*/  "PA Door Header Cap", "Window", "Fibreglass Roof Ridge Cap"};
            }
        }
        public List<string> GuttersNames
        {
            get
            {
                if (m_GuttersNames == null) m_GuttersNames = new List<string>() { "Roof Gutter 430", "Roof Gutter 520", "Roof Gutter 550"/*, "Internal Gutter"*/ };
                return m_GuttersNames;
            }
        }

        public bool IsCladdingEnabled
        {
            get
            {
                return m_IsCladdingEnabled;
            }

            set
            {
                m_IsCladdingEnabled = value;
            }
        }

        //public CAccessories_LengthItemProperties RemovedFlashing
        //{
        //    get
        //    {
        //        return m_RemovedFlashing;
        //    }

        //    set
        //    {
        //        m_RemovedFlashing = value;
        //    }
        //}

        #endregion Properties

        CPFDViewModel _pfdVM;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DoorsAndWindowsViewModel()
        {
            IsSetFromCode = true;

            
            IsSetFromCode = false;
        }
        public DoorsAndWindowsViewModel(ObservableCollection<DoorProperties> doorBlocksProperties, ObservableCollection<WindowProperties> windowBlocksProperties)            
        {
            DoorBlocksProperties = doorBlocksProperties;
            WindowBlocksProperties = windowBlocksProperties;
            //Flashings, Downpipes a Gutters sa nastavia po zmene/nastaveni ModelTypeIndex
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(DoorsAndWindowsViewModel newVM)
        {
            IsSetFromCode = true;

            //Not implemented
            DoorBlocksProperties = newVM.DoorBlocksProperties;
            WindowBlocksProperties = newVM.WindowBlocksProperties;

            Flashings = newVM.Flashings;
            Gutters = newVM.Gutters;
            Downpipes = newVM.Downpipes;



            IsSetFromCode = false;
        }

        private void HandleDoorPropertiesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (IsSetFromCode) return;

                if (e.PropertyName == "sBuildingSide")
                {
                    //SetResultsAreNotValid();
                    if (sender is DoorProperties) { if (!SetDoorsBays(sender as DoorProperties)) return; }
                    if (sender is WindowProperties) { if (!SetWindowsBays(sender as WindowProperties)) return; }
                }
                else if (e.PropertyName == "iBayNumber")
                {
                    //SetResultsAreNotValid();
                    if (sender is DoorProperties)
                    {
                        DoorProperties d = sender as DoorProperties;
                        if (!CheckDoorsBays(d)) { IsSetFromCode = true; d.iBayNumber = d.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                    if (sender is WindowProperties)
                    {
                        WindowProperties w = sender as WindowProperties;
                        if (!CheckWindowsBays(w)) { IsSetFromCode = true; w.iBayNumber = w.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                }
                else if (e.PropertyName == "sDoorType")
                {
                    //SetResultsAreNotValid();
                    //SetComponentListAccordingToDoors();
                }
                else if (e.PropertyName == "CoatingColor")
                {
                    //SetResultsAreNotValid(); //regenerate after change
                }
                else if (e.PropertyName == "Series" || e.PropertyName == "Series")
                {
                    return;
                }

                if (e.PropertyName == "fDoorsHeight" || e.PropertyName == "fDoorsWidth" ||
                    e.PropertyName == "fDoorCoordinateXinBlock")
                {
                    //SetResultsAreNotValid();
                }
                //RecreateFloorSlab = true;
                this.PropertyChanged(sender, e);
            }
            catch (Exception ex)
            {
                //task 551
                //toto este prerobit tak,ze zdetekuje koliziu dveri a okna
                //PFDMainWindow.ShowMessageBoxInPFDWindow(ex.Message);
                MessageBox.Show(ex.Message);
                //bug 436
                //tu by som chcel reagovat na to,ze neexistuje volna bay, zistit koliziu = ze su rovnake objekty a jeden surovo zmazat
                var duplicates = DoorBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() > 1).Select(g => g.FirstOrDefault());
                if (duplicates.Count() > 0)
                {
                    var doorProps = DoorBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() == 1).Select(g => g.FirstOrDefault()).ToList();
                    doorProps.AddRange(duplicates);
                    DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorProps);
                }
            }
        }

        private void DoorBlocksProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //    DoorProperties d = MDoorBlocksProperties.LastOrDefault();
            //    if (d != null)
            //    {
            //        CDoorsAndWindowsHelper.SetDefaultDoorParams(d);
            //        d.PropertyChanged += HandleDoorPropertiesPropertyChangedEvent;
            //        NotifyPropertyChanged("DoorBlocksProperties_Add");
            //        SetResultsAreNotValid();
            //    }
            //}
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                //RecreateModel = true;
                //RecreateJoints = true;
                //RecreateFloorSlab = true;
                NotifyPropertyChanged("DoorBlocksProperties_CollectionChanged");
                //SetResultsAreNotValid();
            }
            //SetComponentListAccordingToDoors();
        }

        private void HandleWindowPropertiesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (IsSetFromCode) return;

                if (e.PropertyName == "sBuildingSide")
                {
                    //SetResultsAreNotValid();
                    if (sender is DoorProperties) { if (!SetDoorsBays(sender as DoorProperties)) return; }
                    if (sender is WindowProperties) { if (!SetWindowsBays(sender as WindowProperties)) return; }
                }
                else if (e.PropertyName == "iBayNumber")
                {
                    //SetResultsAreNotValid();
                    if (sender is DoorProperties)
                    {
                        DoorProperties d = sender as DoorProperties;
                        if (!CheckDoorsBays(d)) { IsSetFromCode = true; d.iBayNumber = d.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                    if (sender is WindowProperties)
                    {
                        WindowProperties w = sender as WindowProperties;
                        if (!CheckWindowsBays(w)) { IsSetFromCode = true; w.iBayNumber = w.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                }
                else if (e.PropertyName == "fWindowsHeight" || e.PropertyName == "fWindowsWidth" || e.PropertyName == "fWindowCoordinateXinBay" || e.PropertyName == "fWindowCoordinateZinBay")
                {
                    //SetResultsAreNotValid();
                }
                this.PropertyChanged(sender, e);
            }
            catch (Exception ex)
            {
                //task 551
                //toto este prerobit tak,ze zdetekuje koliziu dveri a okna
                //PFDMainWindow.ShowMessageBoxInPFDWindow(ex.Message);
                MessageBox.Show(ex.Message);
                //bug 436
                //tu by som chcel reagovat na to,ze neexistuje volna bay, zistit koliziu = ze su rovnake objekty a jeden surovo zmazat
                var duplicates = WindowBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() > 1).Select(g => g.FirstOrDefault());
                if (duplicates.Count() > 0)
                {
                    var windowsProps = WindowBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() == 1).Select(g => g.FirstOrDefault()).ToList();
                    windowsProps.AddRange(duplicates);
                    WindowBlocksProperties = new ObservableCollection<WindowProperties>(windowsProps);
                }
            }


        }
        private void WindowBlocksProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //    WindowProperties w = MWindowBlocksProperties.LastOrDefault();
            //    if (w != null)
            //    {
            //        CDoorsAndWindowsHelper.SetDefaultWindowParams(w);
            //        w.PropertyChanged += HandleWindowPropertiesPropertyChangedEvent;
            //        NotifyPropertyChanged("WindowBlocksProperties_Add");
            //        SetResultsAreNotValid();
            //    }
            //}
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                //RecreateModel = true;
                //RecreateJoints = true;
                NotifyPropertyChanged("WindowBlocksProperties_CollectionChanged");
                //SetResultsAreNotValid();
            }
            //SetComponentListAccordingToWindows();
        }

        private void Flashings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {            
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            //{
            //    //e.OldItems
            //    if (e.OldItems.Count > 0)
            //    {
            //        CAccessories_LengthItemProperties item = (CAccessories_LengthItemProperties)e.OldItems[0];
            //        m_RemovedFlashing = item;
            //    }
            //}
            NotifyPropertyChanged("Flashings_CollectionChanged");
        }
        public void FlashingsItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (!ValidateFlashings())
                {
                    MessageBox.Show("ERROR.\nDuplicated definition of flashing type.\nChoose a unique type, please.");
                    //PFDMainWindow.ShowMessageBoxInPFDWindow("ERROR.\nDuplicated definition of flashing type.\nChoose a unique type, please.");
                    CAccessories_LengthItemProperties item = sender as CAccessories_LengthItemProperties;
                    if (item != null) item.Name = item.NameOld;
                    //PFDMainWindow.Datagrid_Flashings.ItemsSource = null;
                    //PFDMainWindow.Datagrid_Flashings.ItemsSource = Flashings;
                }
            }
            if (e.PropertyName == "Thickness") return;
            if (e.PropertyName == "Width_total") return;
            PropertyChanged(sender, e);
        }
        private void Gutters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Gutters_CollectionChanged");

            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            //{                
            //}
        }
        private void DownpipeItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(sender, e);
        }
        private void Downpipes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Downpipes_CollectionChanged");
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            //{
                
            //}
        }
        public void AccessoriesItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Thickness") return;
            if (e.PropertyName == "Width_total") return;
            PropertyChanged(sender, e);
        }



        private ObservableCollection<int> frontBays;
        private ObservableCollection<int> backBays;
        private ObservableCollection<int> leftRightBays;

        public void SetModelBays(CPFDViewModel pfdVM)
        {
            _pfdVM = pfdVM;
            CModel_PFD model;

            if (_pfdVM.Model is CModel_PFD_01_MR)
                model = (CModel_PFD_01_MR)_pfdVM.Model;
            else if (_pfdVM.Model is CModel_PFD_01_GR)
                model = (CModel_PFD_01_GR)_pfdVM.Model;
            else
            {
                model = null;
                throw new Exception("Kitset model is not implemented.");
            }

            frontBays = new ObservableCollection<int>();
            backBays = new ObservableCollection<int>();
            leftRightBays = new ObservableCollection<int>();

            int iFrameNo = model != null ? model.iFrameNo : 4;
            int i = 0;
            while (i < iFrameNo - 1)
            {
                leftRightBays.Add((++i));
            }
            i = 0;
            while (i < _pfdVM.IFrontColumnNoInOneFrame + 1)
            {
                frontBays.Add((++i));
            }
            i = 0;
            while (i < _pfdVM.IFrontColumnNoInOneFrame + 1)
            {
                backBays.Add((++i));
            }

            SetDoorsBays();
            SetWindowsBays();
            SetDoorsWindowsValidationProperties();
        }

        public void SetModelBays(int iFrameNo, int frontColumnNoInOneFrame)
        {            
            frontBays = new ObservableCollection<int>();
            backBays = new ObservableCollection<int>();
            leftRightBays = new ObservableCollection<int>();

            int i = 0;
            while (i < iFrameNo - 1)
            {
                leftRightBays.Add((++i));
            }
            i = 0;
            while (i < frontColumnNoInOneFrame + 1)
            {
                frontBays.Add((++i));
            }
            i = 0;
            while (i < frontColumnNoInOneFrame + 1)
            {
                backBays.Add((++i));
            }

            SetDoorsBays(false);
            SetWindowsBays(false);
        }

        private int GetFreeBayFor(WindowProperties win)
        {
            foreach (int bayNum in win.Bays)
            {
                if (WindowBlocksProperties.Where(x => x.iBayNumber == bayNum && x.sBuildingSide == win.sBuildingSide).Count() == 0) return bayNum;
            }
            return -1;
        }

        private int GetFreeBayFor(DoorProperties d)
        {
            foreach (int bayNum in d.Bays)
            {
                if (DoorBlocksProperties.Where(x => x.iBayNumber == bayNum && x.sBuildingSide == d.sBuildingSide).Count() == 0) return bayNum;
            }
            return -1;
        }

        private void SetDoorsWindowsValidationProperties()
        {
            SetDoorsValidationProperties();
            SetWindowsValidationProperties();
        }

        private void SetDoorsValidationProperties()
        {
            CModel_PFD model;

            if (_pfdVM.Model is CModel_PFD_01_MR)
                model = (CModel_PFD_01_MR)_pfdVM.Model;
            else if (_pfdVM.Model is CModel_PFD_01_GR)
                model = (CModel_PFD_01_GR)_pfdVM.Model;
            else
            {
                model = null;
                throw new Exception("Kitset model is not implemented.");
            }

            foreach (DoorProperties d in DoorBlocksProperties)
            {
                //task 600
                //d.SetValidationValues(MWallHeight, model.fL1_frame, model.fDist_FrontColumns, model.fDist_BackColumns);
                d.SetValidationValues(_pfdVM.WallHeight, model.GetBayWidth(d.iBayNumber), model.fDist_FrontColumns, model.fDist_BackColumns);
            }
        }

        private void SetWindowsValidationProperties()
        {
            CModel_PFD model;

            if (_pfdVM.Model is CModel_PFD_01_MR)
                model = (CModel_PFD_01_MR)_pfdVM.Model;
            else if (_pfdVM.Model is CModel_PFD_01_GR)
                model = (CModel_PFD_01_GR)_pfdVM.Model;
            else
            {
                model = null;
                throw new Exception("Kitset model is not implemented.");
            }

            foreach (WindowProperties w in WindowBlocksProperties)
            {
                //task 600
                //w.SetValidationValues(MWallHeight, model.fL1_frame, model.fDist_FrontColumns, model.fDist_BackColumns);
                w.SetValidationValues(_pfdVM.WallHeight, model.GetBayWidth(w.iBayNumber), model.fDist_FrontColumns, model.fDist_BackColumns);
            }
        }

        public void SetDoorsBays(bool check = true)
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sBuildingSide == "Front" && !d.Bays.SequenceEqual(frontBays)) d.Bays = frontBays;
                else if (d.sBuildingSide == "Back" && !d.Bays.SequenceEqual(backBays)) d.Bays = backBays;
                else if (d.sBuildingSide == "Left" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;
                else if (d.sBuildingSide == "Right" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;
            }
            if (check) CheckDoorsBays();
        }

        private bool SetDoorsBays(DoorProperties d)
        {
            if (d.sBuildingSide == "Front" && !d.Bays.SequenceEqual(frontBays)) d.Bays = frontBays;
            else if (d.sBuildingSide == "Back" && !d.Bays.SequenceEqual(backBays)) d.Bays = backBays;
            else if (d.sBuildingSide == "Left" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;
            else if (d.sBuildingSide == "Right" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;

            if (!CheckDoorsBays(d))
            {
                this.IsSetFromCode = true;
                d.sBuildingSide = d.sBuildingSide_old;
                this.IsSetFromCode = false;
                return false;
            }
            return true;
        }

        private void CheckDoorsBays()
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.iBayNumber > d.Bays.Count) d.iBayNumber = 1;
                if (DoorBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() > 1)
                {
                    //d.iBayNumber++; //tu by sa dala napisat funkcia na najdenie volneho bay na umiesnenie dveri
                    int bayNum = GetFreeBayFor(d);
                    if (bayNum == -1) //PFDMainWindow.ShowMessageBoxInPFDWindow($"Not possible to find free bay on this side. [{d.sBuildingSide}]");
                        MessageBox.Show($"Not possible to find free bay on this side. [{d.sBuildingSide}]");
                    else d.iBayNumber = bayNum;
                }
            }
        }

        private bool CheckDoorsBays(DoorProperties d)
        {
            bool isValid = true;
            if (d.iBayNumber > d.Bays.Count) d.iBayNumber = 1;

            if (DoorBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() > 1)
            {
                MessageBox.Show("This bay is already occupied with a door.");
                isValid = false;
                //throw new Exception($"This bay is already occupied with a door.");
            }
            if (WindowBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() == 1)
            {
                MessageBox.Show("This bay is already occupied with a window.");
                isValid = false;
                //throw new Exception($"This bay is already occupied with a window.");
            }
            return isValid;
        }

        private void SetWindowsBays(bool check = true)
        {
            foreach (WindowProperties w in WindowBlocksProperties)
            {
                if (w.sBuildingSide == "Front" && !w.Bays.SequenceEqual(frontBays)) w.Bays = frontBays;
                else if (w.sBuildingSide == "Back" && !w.Bays.SequenceEqual(backBays)) w.Bays = backBays;
                else if (w.sBuildingSide == "Left" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;
                else if (w.sBuildingSide == "Right" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;
            }
            if (check)
            {
                CheckWindowsBays();
            }
        }

        private bool SetWindowsBays(WindowProperties w)
        {
            if (w.sBuildingSide == "Front" && !w.Bays.SequenceEqual(frontBays)) w.Bays = frontBays;
            else if (w.sBuildingSide == "Back" && !w.Bays.SequenceEqual(backBays)) w.Bays = backBays;
            else if (w.sBuildingSide == "Left" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;
            else if (w.sBuildingSide == "Right" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;

            if (!CheckWindowsBays(w))
            {
                this.IsSetFromCode = true;
                w.sBuildingSide = w.sBuildingSide_old;
                this.IsSetFromCode = false;
                return false;
            }
            return true;
        }

        private void CheckWindowsBays()
        {
            foreach (WindowProperties w in WindowBlocksProperties)
            {
                if (w.iBayNumber > w.Bays.Count) w.iBayNumber = 1;
                if (WindowBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() > 1)
                {
                    //w.iBayNumber++; //tu by sa dala napisat funkcia na najdenie volneho bay na umiesnenie okna
                    int bayNum = GetFreeBayFor(w);
                    if (bayNum == -1) MessageBox.Show($"Not possible to find free bay on this side. [{w.sBuildingSide}]");
                    else
                    {
                        w.IsSetFromCode = true;
                        w.iBayNumber = bayNum;
                        w.IsSetFromCode = false;
                    }
                }
            }
        }

        private bool CheckWindowsBays(WindowProperties w)
        {
            bool isValid = true;
            if (w.iBayNumber > w.Bays.Count) w.iBayNumber = 1;
            if (WindowBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() > 1)
            {
                MessageBox.Show("The position is already occupied with a window.");
                isValid = false;
            }
            if (DoorBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() == 1)
            {
                MessageBox.Show("The position is already occupied with a door.");
                isValid = false;
            }
            return isValid;
        }


        

        public bool ValidateFlashings()
        {
            foreach (CAccessories_LengthItemProperties item in Flashings)
            {
                int count = Flashings.Where(f => f.Name == item.Name).Count();
                if (count > 1) return false;

                if (item.Name == "Roof Ridge")
                {
                    if (Flashings.FirstOrDefault(f => f.Name == "Roof Ridge (Soft Edge)") != null) return false;
                }

                if (item.Name == "Roof Ridge (Soft Edge)")
                {
                    if (Flashings.FirstOrDefault(f => f.Name == "Roof Ridge") != null) return false;
                }
            }
            return true;
        }

        public void SetAllDoorCoatingColorAccordingTo(DoorProperties doorProperties)
        {
            if (DoorBlocksProperties == null) return;
            if (doorProperties == null) return;

            foreach (DoorProperties dp in DoorBlocksProperties)
            {
                if (dp.CoatingColor.ID != doorProperties.CoatingColor.ID)
                {
                    dp.IsSetFromCode = true;
                    dp.CoatingColor = dp.CoatingColors.FirstOrDefault(c => c.ID == doorProperties.CoatingColor.ID);
                    dp.IsSetFromCode = false;
                }
            }
        }
        public void SetAllDoorCoatingColorToSame()
        {
            if (DoorBlocksProperties == null) return;

            SetAllDoorCoatingColorAccordingTo(DoorBlocksProperties.FirstOrDefault());
        }

        public void SetAllFlashingsCoatingColorAccordingTo(CoatingColour colour)
        {
            if (Flashings == null) return;

            IsSetFromCode = true;
            foreach (CAccessories_LengthItemProperties p in Flashings)
            {
                if (p.CoatingColor.ID != colour.ID)
                {
                    //p.IsSetFromCode = true;
                    p.CoatingColor = p.CoatingColors.FirstOrDefault(c => c.ID == colour.ID);
                    //p.IsSetFromCode = false;
                }
            }
            IsSetFromCode = false;
        }
        public void SetAllGuttersCoatingColorAccordingTo(CoatingColour colour)
        {
            if (Gutters == null) return;

            IsSetFromCode = true;
            foreach (CAccessories_LengthItemProperties p in Gutters)
            {
                if (p.CoatingColor.ID != colour.ID)
                {
                    //p.IsSetFromCode = true;
                    p.CoatingColor = p.CoatingColors.FirstOrDefault(c => c.ID == colour.ID);
                    //p.IsSetFromCode = false;
                }
            }
            IsSetFromCode = false;
        }

        public void SetAllDownpipeCoatingColorAccordingTo(CoatingColour colour)
        {
            if (Downpipes == null) return;

            IsSetFromCode = true;
            foreach (CAccessories_DownpipeProperties p in Downpipes)
            {
                if (p.CoatingColor.ID != colour.ID)
                {
                    //p.IsSetFromCode = true;
                    p.CoatingColor = p.CoatingColors.FirstOrDefault(c => c.ID == colour.ID);
                    //p.IsSetFromCode = false;
                }
            }
            IsSetFromCode = false;
        }

        public void SetAll_FGD_CoatingColorAccordingTo(CoatingColour colour)
        {
            SetAllFlashingsCoatingColorAccordingTo(colour);
            SetAllGuttersCoatingColorAccordingTo(colour);
            SetAllDownpipeCoatingColorAccordingTo(colour);
        }

        public void SetAllFlashingsCoatingColorToSame()
        {
            CoatingColour col = null;
            if (Flashings != null) col = Flashings.FirstOrDefault().CoatingColor;

            if (col != null) SetAllFlashingsCoatingColorAccordingTo(col);
        }
        public void SetAllGuttersCoatingColorToSame()
        {
            CoatingColour col = null;
            if (Gutters != null) col = Gutters.FirstOrDefault().CoatingColor;

            if (col != null) SetAllGuttersCoatingColorAccordingTo(col);
        }
        public void SetAllDownpipesCoatingColorToSame()
        {
            CoatingColour col = null;
            if (Downpipes != null) col = Downpipes.FirstOrDefault().CoatingColor;

            if (col != null) SetAllDownpipeCoatingColorAccordingTo(col);
        }

        public void SetAll_FGD_CoatingColorToSame()
        {
            CoatingColour col = GetActual_FGD_Color();

            if (col != null) SetAll_FGD_CoatingColorAccordingTo(col);
        }

        public CoatingColour GetActual_FGD_Color()
        {
            CoatingColour col = null;
            if (Flashings != null) col = Flashings.FirstOrDefault().CoatingColor;
            else if (Gutters != null) col = Gutters.FirstOrDefault().CoatingColor;
            else if (Downpipes != null) col = Downpipes.FirstOrDefault().CoatingColor;

            return col;
        }

        public bool ModelHasDoor()
        {
            return (ModelHasRollerDoor() || ModelHasPersonelDoor());
        }

        public bool ModelHasPersonelDoor()
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sDoorType == "Personnel Door") return true;
            }
            return false;
        }

        public bool ModelHasRollerDoor()
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sDoorType == "Roller Door") return true;
            }
            return false;
        }

        public bool ModelHasWindow()
        {
            if (WindowBlocksProperties == null) return false;

            return WindowBlocksProperties.Count > 0;
        }

        public void RemoveDoorsAndWindowsBuildingSide(string sBuildingSide)
        {
            int doorsToRemoveCount = 0;
            List<DoorProperties> doorsProps = new List<DoorProperties>();

            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sBuildingSide == sBuildingSide) doorsToRemoveCount++;
                else doorsProps.Add(d);
            }

            int windowsToRemoveCount = 0;
            List<WindowProperties> windowsProps = new List<WindowProperties>();
            foreach (WindowProperties w in WindowBlocksProperties)
            {
                if (w.sBuildingSide == sBuildingSide) windowsToRemoveCount++;
                else windowsProps.Add(w);
            }

            if (doorsToRemoveCount > 0)
            {
                IsSetFromCode = true;
                DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorsProps);
                IsSetFromCode = false;
            }
            if (windowsToRemoveCount > 0)
            {
                IsSetFromCode = true;
                WindowBlocksProperties = new ObservableCollection<WindowProperties>(windowsProps);
                IsSetFromCode = false;
            }
        }

        public bool AreDoorsOrWindowsOnBuildingSide(string sBuildingSide)
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sBuildingSide == sBuildingSide) return true;
            }

            foreach (WindowProperties w in WindowBlocksProperties)
            {
                if (w.sBuildingSide == sBuildingSide) return true;
            }
            return false;
        }

        public void RemoveRollerDoors()
        {
            int doorsToRemoveCount = 0;
            List<DoorProperties> doorsProps = new List<DoorProperties>();

            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sDoorType == "Roller Door") doorsToRemoveCount++;
                else doorsProps.Add(d);
            }

            if (doorsToRemoveCount > 0)
            {
                IsSetFromCode = true;
                DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorsProps);
                IsSetFromCode = false;
            }            
        }
        public void RemovePersonelDoors()
        {
            int doorsToRemoveCount = 0;
            List<DoorProperties> doorsProps = new List<DoorProperties>();

            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sDoorType == "Personnel Door") doorsToRemoveCount++;
                else doorsProps.Add(d);
            }

            if (doorsToRemoveCount > 0)
            {
                IsSetFromCode = true;
                DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorsProps);
                IsSetFromCode = false;
            }
        }
        public void RemoveWindows()
        {
            IsSetFromCode = true;
            WindowBlocksProperties = new ObservableCollection<WindowProperties>();
            IsSetFromCode = false;
        }

        public bool AreAnyDoorsOrWindowsWith(CAccessories_LengthItemProperties flashing)
        {
            if (flashing.Name.Contains("Roller Door") && ModelHasRollerDoor()) return true;

            if (flashing.Name.Contains("PA Door") && ModelHasPersonelDoor()) return true;

            if (flashing.Name.Contains("Window") && ModelHasWindow()) return true;

            return false;
        }

        public void DeleteDoorsOrWindowsWith(CAccessories_LengthItemProperties flashing)
        {
            if (flashing.Name.Contains("Roller Door") && ModelHasRollerDoor()) RemoveRollerDoors();
            else if (flashing.Name.Contains("PA Door") && ModelHasPersonelDoor()) RemovePersonelDoors();
            else if (flashing.Name.Contains("Window") && ModelHasWindow()) RemoveWindows();
        }


        public bool CheckFlashings()
        {
            bool changed = false;
            if (_pfdVM != null)
            {
                if (!_pfdVM._modelOptionsVM.EnableCladding) return changed; //ak nie je zapnuty cladding, netreba doplnat naspat Flashings
            }
            List<CAccessories_LengthItemProperties> flashingsToAdd = new List<CAccessories_LengthItemProperties>();
            if (ModelHasRollerDoor())
            {
                if (!Flashings.Any(f => f.Name == "Roller Door Trimmer")) flashingsToAdd.Add(new CAccessories_LengthItemProperties("Roller Door Trimmer", "Flashings", 0, 4));
                if (!Flashings.Any(f => f.Name == "Roller Door Header")) flashingsToAdd.Add(new CAccessories_LengthItemProperties("Roller Door Header", "Flashings", 0, 4));
                if (!Flashings.Any(f => f.Name == "Roller Door Header Cap")) flashingsToAdd.Add(new CAccessories_LengthItemProperties("Roller Door Header Cap", "Flashings", 0, 4));
            }

            if (ModelHasPersonelDoor())
            {
                //if (!Flashings.Any(f => f.Name == "PA Door Trimmer")) flashingsToAdd.Add(new CAccessories_LengthItemProperties("PA Door Trimmer", "Flashings", 0, 18));
                if (!Flashings.Any(f => f.Name == "PA Door Header Cap")) flashingsToAdd.Add(new CAccessories_LengthItemProperties("PA Door Header Cap", "Flashings", 0, 18));
            }

            if (ModelHasWindow())
            {
                if (!Flashings.Any(f => f.Name == "Window")) flashingsToAdd.Add(new CAccessories_LengthItemProperties("Window", "Flashings", 0, 9));
            }

            if (flashingsToAdd.Count > 0)
            {
                flashingsToAdd.InsertRange(0, Flashings);
                Flashings = new ObservableCollection<CAccessories_LengthItemProperties>(flashingsToAdd.OrderBy(f=>f.ID));
                changed = true;
            }

            return changed;
        }

        public void CheckFlashingsColors()
        {
            IEnumerable<int> rollerDoorColors = Flashings.Where(f => f.Name.Contains("Roller Door")).Select(f=>f.CoatingColor.ID).Distinct();
            if (rollerDoorColors.Count() > 1)
            {
                CoatingColour coatingColour = Flashings.Where(f => f.Name.Contains("Roller Door")).Select(f => f.CoatingColor).FirstOrDefault();
                foreach (CAccessories_LengthItemProperties item in Flashings.Where(f => f.Name.Contains("Roller Door")))
                {
                    if(item.CoatingColor != coatingColour) item.CoatingColor = coatingColour;
                }
            }

            IEnumerable<int> personelDoorColors = Flashings.Where(f => f.Name.Contains("PA Door")).Select(f => f.CoatingColor.ID).Distinct();
            if (personelDoorColors.Count() > 1)
            {
                CoatingColour coatingColour = Flashings.Where(f => f.Name.Contains("PA Door")).Select(f => f.CoatingColor).FirstOrDefault();
                foreach (CAccessories_LengthItemProperties item in Flashings.Where(f => f.Name.Contains("PA Door")))
                {
                    if(item.CoatingColor != coatingColour) item.CoatingColor = coatingColour;
                }
            }
        }

        public void SetFlashingsCoatingColorAccordingTo(CAccessories_LengthItemProperties item)
        {
            if (item.Name.Contains("Roller Door"))
            {
                foreach (CAccessories_LengthItemProperties flashingRD in Flashings.Where(f => f.Name.Contains("Roller Door")))
                {
                    if(flashingRD.CoatingColor != item.CoatingColor) flashingRD.CoatingColor = item.CoatingColor;
                }
            }
            if (item.Name.Contains("PA Door"))
            {
                foreach (CAccessories_LengthItemProperties flashingPD in Flashings.Where(f => f.Name.Contains("PA Door")))
                {
                    if(flashingPD.CoatingColor != item.CoatingColor) flashingPD.CoatingColor = item.CoatingColor;
                }
            }
        }

        public bool AreBothRollerDoorHeaderFlashings()
        {
            if (Flashings == null) return false;

            if (Flashings.Any(f => f.Name == "Roller Door Header") && Flashings.Any(f => f.Name == "Roller Door Header Cap")) return true;
            else return false;            
        }


        public bool HasFlashing(EFlashingType flashingType)
        {
            if (Flashings == null) return false;

            return Flashings.Any(f => f.ID == (int)flashingType);
        }

        public bool HasGutter()
        {
            if (Gutters == null) return false;

            return Gutters.Count > 0;
        }



        public float GetTotalModelDoorsWidth()
        {
            float totalW = 0f;
            foreach (DoorProperties door in DoorBlocksProperties)
            {
                totalW += door.fDoorsWidth;
            }
            return totalW;
        }

        public double GetRollerDoorTrimmerLengh()
        {
            double dRollerDoorTrimmerLengh = 0;
            foreach (DoorProperties door in DoorBlocksProperties)
            {                
                if (door.sDoorType == "Roller Door")
                {
                    dRollerDoorTrimmerLengh += door.fDoorsHeight * 2;                    
                }                
            }
            return dRollerDoorTrimmerLengh;
        }

        public double GetRollerDoorHeaderLengh()
        {
            double dRollerDoorHeaderLengh = 0;
            foreach (DoorProperties door in DoorBlocksProperties)
            {

                if (door.sDoorType == "Roller Door")
                {                    
                    dRollerDoorHeaderLengh += door.fDoorsWidth;                    
                }                
            }
            return dRollerDoorHeaderLengh;
        }

        public double GetNumberOfRollerDoorTrimmers()
        {
            int iNumberOfRollerDoorTrimmers = 0;
            foreach (DoorProperties door in DoorBlocksProperties)
            {
                if (door.sDoorType == "Roller Door")
                {
                    
                    iNumberOfRollerDoorTrimmers += 2;
                }                
            }
            return iNumberOfRollerDoorTrimmers;
        }


    }
}