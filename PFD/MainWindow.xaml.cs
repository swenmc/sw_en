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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;
using DATABASE;
using MATH;
using MATERIAL;
using M_EC1.AS_NZS;
using _3DTools;
using FEM_CALC_BASE;
using M_BASE;
using CRSC;
using EXPIMP;
using Examples;
using DATABASE.DTO;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Controls;
using PFD.Infrastructure;
using System.Collections.ObjectModel;
using System.IO;
using BaseClasses.Helpers;

namespace PFD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        // Names of tabs in Main Window
        private enum ETabNames
        {
            eGeneral = 0,
            eMember_Input = 1,
            eDoorsAndWindows = 2,
            eJoint_Input = 3,
            eFooting_Input = 4,
            eLoads = 5,

            eLoadCases = 6,
            eLoadCombinations = 7,

            eInternalForces = 8,
            eMemberDesign = 9,
            eJointDesign = 10,
            eFootingDesign = 11,

            ePartList = 12,
            eQuoation = 13

        }

        ////////////////////////////////////////////////////////////////////////
        // PORTAL FRAME DESIGNER
        ////////////////////////////////////////////////////////////////////////

        bool bDebugging = false;

        public ObservableCollection<DoorProperties> DoorBlocksProperties;
        public ObservableCollection<WindowProperties> WindowBlocksProperties;
        public CPFDViewModel vm;
        public DisplayOptions sDisplayOptions;
        public BuildingDataInput sBuildingInputData;
        public BuildingGeometryDataInput sGeometryInputData;
        public CPFDLoadInput loadInput;
        public SnowLoadDataInput sSnowInputData;
        public WindLoadDataInput sWindInputData;
        public SeisLoadDataInput sSeisInputData;

        private CProjectInfoVM projectInfoVM;

        public MainWindow()
        {
            // Initial Screen
            SplashScreen splashScreen = new SplashScreen("Resources/fs-screen.jpg");
            splashScreen.Show(false);
            InitializeComponent();
            splashScreen.Close(TimeSpan.FromMilliseconds(1000));

            // Set items in comboboxes and default values
            SetInitialItemsInComboboxes();

            // Prepare data for generating of door blocks
            DoorBlocksProperties = CDoorsAndWindowsHelper.GetDefaultDoorProperties();

            // Prepare data for generating of window blocks
            WindowBlocksProperties = CDoorsAndWindowsHelper.GetDefaultWindowsProperties();

            CComponentListVM compListVM = uc_ComponentList.DataContext as CComponentListVM;
            SetLoadInput();

            projectInfoVM = new CProjectInfoVM();

            // Model Geometry
            vm = new CPFDViewModel(1, DoorBlocksProperties, WindowBlocksProperties, compListVM, loadInput, projectInfoVM);
            vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            this.DataContext = vm;
            vm.PFDMainWindow = this;

            Combobox_RoofCladding.SelectedIndex = 1; //toto len kvoli nasledujucej metode,ktora sa inak zrube
            Combobox_WallCladding.SelectedIndex = 1; //toto len kvoli nasledujucej metode,ktora sa inak zrube

            FillComboboxTrapezoidalSheetingThickness(Combobox_RoofCladding.Items[vm.RoofCladdingIndex].ToString(), Combobox_RoofCladdingThickness);
            FillComboboxTrapezoidalSheetingThickness(Combobox_WallCladding.Items[vm.WallCladdingIndex].ToString(), Combobox_WallCladdingThickness);
            
            UpdateAll();

            vm.Model.GroupModelMembers();
            vm.RecreateJoints = false;

            
        }

        //tu sa da spracovat  e.PropertyName a reagovat konkretne na to,ze ktora property bola zmenena vo view modeli
        protected void HandleViewModelPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (vm.IsSetFromCode) return;
            if (sender is CPFDViewModel)
            {
                CPFDViewModel viewModel = sender as CPFDViewModel;
                if (viewModel == null) return;
                if (viewModel.IsSetFromCode) return; //ak je to property nastavena v kode napr. pri zmene typu modelu tak nic netreba robit

                if (e.PropertyName == "RoofCladdingIndex")
                {
                    FillComboboxTrapezoidalSheetingThickness(Combobox_RoofCladding.Items[viewModel.RoofCladdingIndex].ToString(), Combobox_RoofCladdingThickness);
                }
                else if (e.PropertyName == "WallCladdingIndex")
                {
                    FillComboboxTrapezoidalSheetingThickness(Combobox_WallCladding.Items[viewModel.WallCladdingIndex].ToString(), Combobox_WallCladdingThickness);
                }

                if (e.PropertyName == "Bays") return;
                if (e.PropertyName == "IsEnabledLocalMembersAxis") return;
                if (e.PropertyName == "IsEnabledSurfaceLoadsAxis") return;
                if (e.PropertyName == "ModelCalculatedResultsValid") return;

                //if (e.PropertyName == "DoorBlocksProperties_Add") { vm.RecreateJoints = true; }
                //if (e.PropertyName == "DoorBlocksProperties_CollectionChanged") { vm.RecreateJoints = true; }
                //if (e.PropertyName == "WindowBlocksProperties_Add") { vm.RecreateJoints = true; }
                //if (e.PropertyName == "WindowBlocksProperties_CollectionChanged") { vm.RecreateJoints = true; }
            }
            else if (sender is CComponentListVM)
            {
                CComponentListVM clVM = sender as CComponentListVM;
                //if (e.PropertyName == "SelectedComponentIndex") return;  //osetrene uz v CPFDViewModel
                if (e.PropertyName == "ColumnFlyBracingPosition_Items") return;
                if (e.PropertyName == "RafterFlyBracingPosition_Items") return;
            }
            else if (sender is CPFDLoadInput)
            {
                //CPFDLoadInput vm = sender as CPFDLoadInput;
                //if (e.PropertyName != "ModelCalculatedResultsValid") return;  //osetrene uz v CPFDViewModel                
                vm.RecreateJoints = false;
            }
            else if (sender is CJointsVM)
            {
                //CJointsVM vm = sender as CJointsVM;
                vm.RecreateJoints = false;
            }
            else if (sender is DoorProperties)
            {
                Datagrid_DoorsAndGates_SelectionChanged(null, null);
                vm.RecreateJoints = true;
            }
            else if (sender is WindowProperties)
            {
                Datagrid_Windows_SelectionChanged(null, null);
                vm.RecreateJoints = true;
            }
            else if (sender is CComponentInfo)
            {   
                CComponentInfo cInfo = sender as CComponentInfo;
                if (cInfo.IsSetFromCode) return;
                if (e.PropertyName == "IsSetFromCode") return;
                if (e.PropertyName == "GenerateIsEnabled") return;
                if (e.PropertyName == "GenerateIsReadonly") return;
                if (e.PropertyName == "ILS_Items") return;
                if (e.PropertyName == "ILS") return;
                if (e.PropertyName == "Section")
                {
                    vm.RecreateJoints = true; //need to recreate joint when Section was changed
                }

                if (e.PropertyName == "Generate" && cInfo.ComponentName == "Girt - Front Side" && cInfo.Generate == false && AreDoorsOrWindowsOnBuildingSide("Front"))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete doors and windows in the front wall?", "Warning", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        RemoveDoorsAndWindowsBuildingSide("Front");
                    }
                    else
                    {
                        cInfo.IsSetFromCode = true;
                        cInfo.Generate = true; //vsetky ostatne property sa same oznacia (len aby to nemalo dosah inde)
                        cInfo.IsSetFromCode = false;
                        return;
                    }
                }

                if (e.PropertyName == "Generate" && cInfo.ComponentName == "Girt - Back Side" && cInfo.Generate == false && AreDoorsOrWindowsOnBuildingSide("Back"))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete doors and windows in the back wall?", "Warning", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        RemoveDoorsAndWindowsBuildingSide("Back");
                    }
                    else
                    {
                        cInfo.Generate = true;
                        return;
                    }
                }
            }

            SetUIElementsVisibility();

            //load the popup
            SplashScreen splashScreen = new SplashScreen("loading2.gif");
            splashScreen.Show(false);

            DeleteCalculationResults();
            UpdateAll();


            splashScreen.Close(TimeSpan.FromSeconds(0.1));
            
            //kvoli Doors Models,  najprv musi byt update
            if (sender is DoorProperties)
            {
                Datagrid_DoorsAndGates_SelectionChanged(null, null);
            }
            else if (sender is WindowProperties)
            {
                Datagrid_Windows_SelectionChanged(null, null);
            }

            if(vm.RecreateJoints) vm.RecreateJoints = false;
        }

        private void RemoveDoorsAndWindowsBuildingSide(string sBuildingSide)
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;

            List<DoorProperties> doorsToRemove = new List<DoorProperties>();
            foreach (DoorProperties d in vm.DoorBlocksProperties)
            {
                if (d.sBuildingSide == sBuildingSide) doorsToRemove.Add(d);
            }
            foreach (DoorProperties d in doorsToRemove)
            {
                vm.DoorBlocksProperties.Remove(d);
            }

            List<WindowProperties> windowsToRemove = new List<WindowProperties>();
            foreach (WindowProperties w in vm.WindowBlocksProperties)
            {
                if (w.sBuildingSide == sBuildingSide) windowsToRemove.Add(w);
            }
            foreach (WindowProperties w in windowsToRemove)
            {
                vm.WindowBlocksProperties.Remove(w);
            }
        }

        private bool AreDoorsOrWindowsOnBuildingSide(string sBuildingSide)
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;

            foreach (DoorProperties d in vm.DoorBlocksProperties)
            {
                if (d.sBuildingSide == sBuildingSide) return true;
            }

            foreach (WindowProperties w in vm.WindowBlocksProperties)
            {
                if (w.sBuildingSide == sBuildingSide) return true;
            }
            return false;
        }

        //SplashScreen splashScreen = null;
        //bool waiting = true;
        public void bckWrk_DoWork(object sender, DoWorkEventArgs e)
        {
            //while (waiting) { Thread.Sleep(1000); }
        }

        public void bckWrk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //hide the popup
        }

        private void DeleteCalculationResults()
        {
            // TODO - Ondrej - je potrebne zmazat vysledky a updatovat UC_InternalForces, UC_MemberDesign, UC_JointDesign (tieto UC by sa nemali zobrazovat pokial nie su k dispozicii vysledky)
            // tj. nebola spustena metoda Calculate_Click, vysledky boli z dovodu zmeny topologickeho 3D modelu zmazane a pod

            //Todo - asi sa to da jednoduchsie
            /*
            DeleteLists();
            Results_GridView.ItemsSource = null;
            Results_GridView.Items.Clear();
            Results_GridView.Items.Refresh();
            */
        }

        private void RunFEMSOlver()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Calculate Internal Forces
            // Todo - napojit FEM vypocet

            // TODO - Ondrej Task No 41
            // Todo - nefunguje implementovany 2D solver, asi je chybna detekcia zakladnej tuhostnej matici pruta
            // Treba sa na to pozriet podrobnejsie
            // Navrhujem napojit nejaky externy solver

            CExample_2D_13_PF temp2Dmodel = new CExample_2D_13_PF(vm.Model.m_arrMat[0], vm.Model.m_arrCrSc[0], vm.Model.m_arrCrSc[1], vm.GableWidth, vm.WallHeight, vm.fh2, 1000, 0, 1000, 1000, 1000);
            FEM_CALC_1Din2D.CFEM_CALC obj_Calc = new FEM_CALC_1Din2D.CFEM_CALC(temp2Dmodel, bDebugging);

            // Auxialiary string - result data
            int iDispDecPrecision = 3; // Precision of numerical values of displacement and rotations
            string sDOFResults = null;

            for (int i = 0; i < obj_Calc.m_V_Displ.FVectorItems.Length; i++)
            {
                int iNodeNumber = obj_Calc.m_fDisp_Vector_CN[i, 1] + 1; // Increase index (1st member "0" to "1"
                int iNodeDOFNumber = obj_Calc.m_fDisp_Vector_CN[i, 2] + 1;

                sDOFResults += "Node No:" + "\t" + iNodeNumber + "\t" +
                               "Node DOF No:" + "\t" + iNodeDOFNumber + "\t" +
                               "Value:" + "\t" + String.Format("{0:0.000}", Math.Round(obj_Calc.m_V_Displ.FVectorItems[i], iDispDecPrecision))
                               + "\n";
            }

            // Main String
            string sMessageCalc =
                "Calculation was successful!" + "\n\n" +
                "Result - vector of calculated values of unrestraint DOF displacement or rotation" + "\n\n" + sDOFResults;

            // Display Message
            MessageBox.Show(sMessageCalc, "Solver Message", MessageBoxButton.OK);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            List<ModelValidationError> errors = ValidationHelper.ValidateModel(vm.Model);
            if (errors.Count > 0)
            {
                MessageBox.Show(ValidationHelper.GetErrorsString(errors));
                return;
            }
            
            //DateTime start = DateTime.Now;
            // Clear results of previous calculation
            DeleteCalculationResults();

            // TODO  - toto je potrebne presunut niekam k materialom / prierezom, moze sa nacitat pred vypoctom
            SetMaterialValuesFromDatabase();
            SetCrossSectionValuesFromDatabase();

            //System.Diagnostics.Trace.WriteLine("After loading from DB : " + (DateTime.Now - start).TotalMilliseconds);

            vm.GenerateMemberLoadsIfNotGenerated();

            Solver solver = new Solver(vm.UseFEMSolverCalculationForSimpleBeam);
            vm.SolverWindow = solver;

            vm.Run();
            solver.ShowDialog();

            vm.ModelCalculatedResultsValid = true;
            SetUIElementsVisibility();
            // TODO - implementovat vypocet
        }

        private CPFDLoadInput SetLoadInput()
        {
            if (loadInput == null)
            {
                if (Loads.Content == null) Loads.Content = new UC_Loads(sGeometryInputData);
                UC_Loads loadInput_UC = (UC_Loads)Loads.Content;
                loadInput = loadInput_UC.DataContext as CPFDLoadInput;
            }

            return loadInput;
        }

        private void CalculateLoadingValues(CModel_PFD_01_GR model)
        {
            // Basic data
            sBuildingInputData.location = (ELocation)loadInput.LocationIndex;                    // locations (cities) enum
            sBuildingInputData.fDesignLife_Value = loadInput.DesignLife_Value;                   // Database value in years
            sBuildingInputData.iImportanceClass = loadInput.ImportanceClassIndex + 1;            // Importance Level (index + 1)

            sBuildingInputData.fAnnualProbabilityULS_Snow = loadInput.AnnualProbabilityULS_Snow; // Annual Probability of Exceedence ULS - Snow
            sBuildingInputData.fAnnualProbabilityULS_Wind = loadInput.AnnualProbabilityULS_Wind; // Annual Probability of Exceedence ULS - Wind
            sBuildingInputData.fAnnualProbabilityULS_EQ = loadInput.AnnualProbabilityULS_EQ;     // Annual Probability of Exceedence ULS - EQ
            sBuildingInputData.fAnnualProbabilitySLS = loadInput.AnnualProbabilitySLS;           // Annual Probability of Exceedence SLS

            sBuildingInputData.fR_ULS_Snow = loadInput.R_ULS_Snow;
            sBuildingInputData.fR_ULS_Wind = loadInput.R_ULS_Wind;
            sBuildingInputData.fR_ULS_EQ = loadInput.R_ULS_EQ;
            sBuildingInputData.fR_SLS = loadInput.R_SLS;

            // Load Generation
            // General loading
            // toto tu tu proste nemoze byt, je nemozne volat tuto metodu skor ako je v combe nastavene Combobox_RoofCladding.SelectedItem
            // TO Ondrej - suvisi to s tym ze potrebujeme oddelit vypocty hodnot zatazeni od generovania 3D geometrie a od GUI
            float fMass_Roof = CComboBoxHelper.GetValueFromDatabasebyRowID("TrapezoidalSheetingSQLiteDB", (string)Combobox_RoofCladding.SelectedItem, "mass_kg_m2", vm.RoofCladdingThicknessIndex);
            float fMass_Wall = CComboBoxHelper.GetValueFromDatabasebyRowID("TrapezoidalSheetingSQLiteDB", (string)Combobox_WallCladding.SelectedItem, "mass_kg_m2", vm.WallCladdingThicknessIndex);

            // General Load (AS / NZS 1170.1)
            CalculateBasicLoad(fMass_Roof, fMass_Wall);

            // Wind  (AS / NZS 1170.2)
            CalculateWindLoad();

            // Snow  (AS / NZS 1170.3)
            CalculateSnowLoad();

            // TODO - refaktorovat vypocet dlzkovej hmotnosti prutov - dlzkove hmotnosti sa pouziju aj v Material List a aj pre vypocet Dead Loads
            float fPurlinMassPerMeter = (float)(model.m_arrCrSc[(int)EMemberGroupNames.ePurlin].A_g * model.m_arrMat[(int)EMemberGroupNames.ePurlin].m_fRho); // [kg] A_g * Rho
            float fEdgePurlinMassPerMeter = (float)(model.m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].A_g * model.m_arrMat[(int)EMemberGroupNames.eEavesPurlin].m_fRho); // [kg] A_g * Rho
            float fGirtMassPerMeter = (float)(model.m_arrCrSc[(int)EMemberGroupNames.eGirtWall].A_g * model.m_arrMat[(int)EMemberGroupNames.eGirtWall].m_fRho); // [kg] A_g * Rho
            float fMainColumnMassPerMeter = (float)(model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].A_g * model.m_arrMat[(int)EMemberGroupNames.eMainColumn].m_fRho); // [kg] A_g * Rho
            float fMainRafterMassPerMeter = (float)(model.m_arrCrSc[(int)EMemberGroupNames.eRafter].A_g * model.m_arrMat[(int)EMemberGroupNames.eRafter].m_fRho); // [kg] A_g * Rho

            float fMainColumnMomentOfInteria_yu = (float)(model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].I_y); // m^4
            float fMainColumnMomentOfInteria_zv = (float)(model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].I_z); // m^4
            float fMainColumnMaterial_E = model.m_arrMat[(int)EMemberGroupNames.eMainColumn].m_fE; // Pa // Material Young's modulus

            // TODO - do buducna napojit na parametre v CModel_PFD (dedi od obecneho modelu CExample)
            // CModel_PFD by mal byt obecny predok pre rozne tvary budov systemu FS

            int iNumberOfEavePurlins_x = model.iEavesPurlinNoInOneFrame;
            int iNumberOfPurlins_x = model.iPurlinNoInOneFrame;
            int iNumberOfGirts_x = model.iGirtNoInOneFrame;
            int iNumberOfMainColumns_x = 2; // TODO - napojit na model
            int iNumberOfMainRafters_x = 2; // TODO - napojit na model

            float fLoadingWidth_Frame_x = vm.fL1; // Zatazovacia sirka ramu
            float fRafterLength = vm.GableWidth / (float)Math.Cos(vm.fRoofPitch_radians);

            float fMass_Purlins_x = iNumberOfPurlins_x * fPurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_EavePurlins_x = iNumberOfEavePurlins_x * fEdgePurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Girts_x = iNumberOfGirts_x * fGirtMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Frame_x = iNumberOfMainColumns_x * fMainColumnMassPerMeter * vm.WallHeight + iNumberOfMainRafters_x * fMainRafterMassPerMeter * fRafterLength;

            // Pre stlpy uvazujeme polovicu vysky
            float fMass_Wall_x_kg = 0.5f * iNumberOfMainColumns_x * vm.WallHeight * fLoadingWidth_Frame_x * (fMass_Wall + (loadInput.AdditionalDeadActionWall * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2
            float fMass_Roof_x_kg = iNumberOfMainRafters_x * fRafterLength * fLoadingWidth_Frame_x * (fMass_Roof + (loadInput.AdditionalDeadActionRoof * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2

            float fMass_Total_x = fMass_Frame_x + fMass_Girts_x + fMass_Wall_x_kg + fMass_EavePurlins_x + fMass_Purlins_x + fMass_Roof_x_kg;

            float fT_1x = GetPeriod(iNumberOfMainColumns_x, vm.WallHeight, fMainColumnMomentOfInteria_yu, fMainColumnMaterial_E, fMass_Total_x);  // TODO  napojit Column Iy (AS 4600 - Ix)

            int iNumberOfMainColumns_y = model.iFrameNo;
            int iNumberOfMainRafters_y = iNumberOfMainColumns_y; // Pocet rafters je rovnaky ako pocet stlpov
            int iNumberOfPurlins_y = (iNumberOfMainColumns_y - 1) * (iNumberOfPurlins_x / 2); // Number of bays (number of frames - 1) * Number of purlins per half of building width
            int iNumberOfEavePurlins_y = (iNumberOfMainColumns_y - 1);
            int iNumberOfGirtsInWallPerMainColumn = model.iGirtNoInOneFrame / iNumberOfMainColumns_x; // Pocet girts na vysku stlpa
            int iNumberOfGirts_y = (iNumberOfMainColumns_x - 1) * iNumberOfGirtsInWallPerMainColumn;

            float fMass_Purlins_y = iNumberOfPurlins_y * fPurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_EavePurlins_y = iNumberOfEavePurlins_y * fEdgePurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Girts_y = iNumberOfGirts_y * fGirtMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Frame_y = iNumberOfMainColumns_y * fMainColumnMassPerMeter * 0.5f * vm.WallHeight + iNumberOfMainRafters_y * fMainRafterMassPerMeter * fRafterLength;
            // TODO - pre smer Y pripocitat vahu polovice sirky * polovice vysky prednej a zadnej steny

            float fLoadingWidth_Frame_y = 0.5f * vm.GableWidth; // Zatazovacia sirka ramu v smere Y - polovica budovy

            float fMass_Wall_y_kg = vm.Length * 0.5f * vm.WallHeight * (fMass_Wall + (loadInput.AdditionalDeadActionWall * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2
            float fMass_Roof_y_kg = vm.Length * fRafterLength * (fMass_Roof + (loadInput.AdditionalDeadActionRoof * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2

            float fMass_Total_y = fMass_Frame_y + fMass_Girts_y + fMass_Wall_y_kg + fMass_EavePurlins_y + fMass_Purlins_y + fMass_Roof_y_kg;

            float fT_1y = GetPeriod(iNumberOfMainColumns_y, vm.WallHeight, fMainColumnMomentOfInteria_zv, fMainColumnMaterial_E, fMass_Total_y);  //  TODO  napojit Column Iz (AS 4600 - Iy)

            // NZS 1170.5, cl. 4.1.2.1 Rayleigh method, eq. 4.1(1)
            float fT_1x_RM_411 = GetPeriod_RM_NZS1107_5_Eq411(iNumberOfMainColumns_x, vm.WallHeight, fMainColumnMomentOfInteria_yu, fMainColumnMaterial_E, fMass_Total_x);
            float fT_1y_RM_411 = GetPeriod_RM_NZS1107_5_Eq411(iNumberOfMainColumns_y, vm.WallHeight, fMainColumnMomentOfInteria_zv, fMainColumnMaterial_E, fMass_Total_y);

            // Validation - compare calculated periods
            // Kontrola vypoctu frekvencii, assert v pripade ze rozdiel je viac nez 20% mensej z hodnot (TODO - idealne je spocitat z globalneho modelu alebo aspon 2D modelu ramu so zohladnenim poddajnosti prievlaku (rafter) pre smer x / krajnej vaznice (edge purlin) pre smer y)
            if (!MathF.d_equal(fT_1x, fT_1x_RM_411, 0.2 * Math.Min(fT_1x, fT_1x_RM_411)) || !MathF.d_equal(fT_1y, fT_1y_RM_411, 0.2 * Math.Min(fT_1y, fT_1y_RM_411)))
                throw new ArgumentException("Period values are different. \n" +
                    "T1.x = " + Math.Round(fT_1x, 5).ToString() + " s\n" +
                    "T1.x.rm = " + Math.Round(fT_1x_RM_411, 5).ToString() + " s" + " Eq. (4.1(1))" + "\n" +
                    "T1.y = " + Math.Round(fT_1y, 5).ToString() + " s\n" +
                    "T1.y.rm = " + Math.Round(fT_1y_RM_411, 5).ToString() + " s" + " Eq. (4.1(1))");

            // Earthquake / Seismic Design  (NZS 1170.5)
            CalculateEQParameters(fT_1x, fT_1y, fMass_Total_x, fMass_Total_y);
        }

        public void CalculateBasicLoad(float fMass_Roof, float fMass_Wall)
        {
            vm.GeneralLoad = new CCalcul_1170_1(
                  fMass_Roof,
                  fMass_Wall,
                  loadInput.AdditionalDeadActionRoof,
                  loadInput.AdditionalDeadActionWall,
                  loadInput.ImposedActionRoof,
                  GlobalConstants.G_ACCELERATION);
        }

        public void CalculateSnowLoad()
        {
            sSnowInputData.eCountry = ECountry.eNewZealand; // Temporary - zatial nie je implementovana Australia
            sSnowInputData.eSnowRegion = (ESnowRegion)loadInput.SnowRegionIndex; // indexovane od 0, takze postacuje len previest na enum
            sSnowInputData.eExposureCategory = (ERoofExposureCategory)loadInput.ExposureCategoryIndex;
            sSnowInputData.fh_0_SiteElevation_meters = loadInput.SiteElevation;
            vm.Snow = new CCalcul_1170_3(sBuildingInputData, sGeometryInputData, sSnowInputData);
        }

        public void CalculateWindLoad()
        {
            sWindInputData.eWindRegion = loadInput.WindRegion;
            sWindInputData.iAngleWindDirection = loadInput.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = GetTerrainCategory(loadInput.TerrainCategoryIndex);

            vm.Wind = new CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData);
        }

        // TODO - refaktorovat tuto funkciu s UC_Load
        private float GetTerrainCategory(int iCategoryIndex) // TODO - prerobit na nacitanie z databazy
        {
            if (iCategoryIndex == 0)
                return 1.0f;
            else if (iCategoryIndex == 1)
                return 1.5f;
            else if (iCategoryIndex == 2)
                return 2.0f;
            else if (iCategoryIndex == 3)
                return 2.5f;
            else if (iCategoryIndex == 4)
                return 3.0f;
            else if (iCategoryIndex == 5)
                return 4.0f;
            else
            {
                // Invalid index
                return -1;
            }
        }

        public void CalculateEQParameters(float fT_1x_param, float fT_1y_param, float fMass_Total_x_param, float fMass_Total_y_param)
        {
            sSeisInputData.eSiteSubsoilClass = loadInput.SiteSubSoilClass;
            sSeisInputData.fProximityToFault_D_km = loadInput.FaultDistanceDmin; // km
            sSeisInputData.fZoneFactor_Z = loadInput.ZoneFactorZ;
            //sSeisInputData.fPeriodAlongXDirection_Tx = loadInput.PeriodAlongXDirectionTx; //sec
            //sSeisInputData.fPeriodAlongYDirection_Ty = loadInput.PeriodAlongYDirectionTy; //sec
            //sSeisInputData.fSpectralShapeFactor_Ch_Tx = loadInput.SpectralShapeFactorChTx;
            //sSeisInputData.fSpectralShapeFactor_Ch_Ty = loadInput.SpectralShapeFactorChTy;

            vm.Eq = new CCalcul_1170_5(fT_1x_param, fT_1y_param, fMass_Total_x_param, fMass_Total_y_param, sBuildingInputData, sSeisInputData);
        }

        // Priblizne riesenie (tuhy prievlak)
        public float GetPeriod(int iNumberOfColumns, float fL, float fI_column, float fE, float fMass_Total)
        {
            // Equivalent Stiffness
            float fk_e = iNumberOfColumns * (3 * fE * fI_column / MathF.Pow3(fL));

            // Natural circular frequency
            float fOmega = MathF.Sqrt(fk_e / fMass_Total);

            // Natural frequency
            float fFrequency = fOmega / (2f * MathF.fPI);

            // Natural period
            float fPeriod_T = 1f / fFrequency;

            return fPeriod_T;
        }

        public float GetPeriod_RM_NZS1107_5_Eq411(int iNumberOfColumns, float fL, float fI_column, float fE, float fMass_Total)
        {
            double fP = 1; // Unit Force
            double fDelta_x = fP * MathF.Pow3(vm.WallHeight) / (iNumberOfColumns * 3 * fI_column * fE); // Cantilever deflection (rafter is considered as rigid member)
            return (float)(2 * MathF.fPI * Math.Sqrt((fMass_Total * GlobalConstants.G_ACCELERATION * MathF.Pow2(fDelta_x)) / (GlobalConstants.G_ACCELERATION * fP * fDelta_x))); // Eq. 4.1(1)
        }

        public void FillComboboxTrapezoidalSheetingThickness(string sTableName, ComboBox combobox)
        {
            CComboBoxHelper.FillComboboxValues("TrapezoidalSheetingSQLiteDB", sTableName, "name", combobox);
        }

        public void SetMaterialValuesFromDatabase()
        {
            foreach (CMat m in vm.Model.m_arrMat)
            {
                if (m is CMat_03_00)
                {
                    CMaterialManager.LoadMaterialProperties((CMat_03_00)m, m.Name);
                }
            }
        }

        public void SetCrossSectionValuesFromDatabase()
        {
            foreach (CCrSc_TW crsc in vm.Model.m_arrCrSc)
            {
                // TODO - zjednotit nazvy prierezov v database a v GUI programu
                // TODO - zaviest v databaze meno prierezu ktore sa ma zobrazovat a meno pouzite pre identifikaciu (mozno enum)
                /*
                10075
                27095
                270115
                270115btb
                270115n
                50020
                50020n
                63020
                63020s1
                63020s2
                */

                // Ak sa funkcia LoadCrossSectionProperties presunie do konstruktora pre CrSc tak toto je mozno uz navyse
                CrScProperties dto = CSectionManager.LoadCrossSectionProperties_meters(crsc.Name_short);
                crsc.SetParams(dto);

                // Docasne hodnoty
                crsc.W_y_pl = 1.001 * crsc.W_y_el;
                crsc.W_z_pl = 1.001 * crsc.W_z_el;

                // TODO - doplnit vypocet polomerov zotrvacnosti do prierezov // Doriesit jednoznacnost indexov
                crsc.i_y_rg = Math.Sqrt(crsc.I_y / crsc.A_g);
                crsc.i_z_rg = Math.Sqrt(crsc.I_z / crsc.A_g);
                crsc.i_yz_rg = Math.Sqrt(crsc.I_yz / crsc.A_g);
            }
        }



        //private void UpdateDisplayOptions()
        //{
        //    // Get display options from GUI
        //    sDisplayOptions.bUseLightDirectional = vm.LightDirectional;
        //    sDisplayOptions.bUseLightPoint = vm.LightPoint;
        //    sDisplayOptions.bUseLightSpot = vm.LightSpot;
        //    sDisplayOptions.bUseLightAmbient = vm.LightAmbient;

        //    sDisplayOptions.bUseDiffuseMaterial = vm.MaterialDiffuse;
        //    sDisplayOptions.bUseEmissiveMaterial = vm.MaterialEmissive;

        //    sDisplayOptions.bDisplayMembers = vm.DisplayMembers;
        //    sDisplayOptions.bDisplayJoints = vm.DisplayJoints;
        //    sDisplayOptions.bDisplayPlates = vm.DisplayPlates;
        //    sDisplayOptions.bDisplayConnectors = vm.DisplayConnectors;
        //    sDisplayOptions.bDisplayNodes = vm.DisplayNodes;

        //    sDisplayOptions.bDisplayFoundations = vm.DisplayFoundations;
        //    sDisplayOptions.bDisplayFloorSlab = vm.DisplayFloorSlab;
        //    sDisplayOptions.bDisplayNodalSupports = vm.DisplayNodalSupports;

        //    sDisplayOptions.bDisplayMemberDescription = vm.ShowMemberDescription;
        //    sDisplayOptions.bDisplayMemberID = vm.ShowMemberID;
        //    sDisplayOptions.bDisplayMemberPrefix = vm.ShowMemberPrefix;
        //    sDisplayOptions.bDisplayMemberCrossSectionStartName = vm.ShowMemberCrossSectionStartName;
        //    sDisplayOptions.bDisplayMemberRealLength = vm.ShowMemberRealLength;
        //    sDisplayOptions.bDisplayNodesDescription = vm.ShowNodesDescription;

        //    sDisplayOptions.bDisplayMembersCenterLines = vm.DisplayMembersCenterLines;
        //    sDisplayOptions.bDisplaySolidModel = vm.DisplaySolidModel;
        //    sDisplayOptions.bDisplayWireFrameModel = vm.DisplayWireFrameModel;

        //    sDisplayOptions.bDistinguishedColor = vm.DisplayDistinguishedColorMember;
        //    sDisplayOptions.bTransparentMemberModel = vm.DisplayTransparentModelMember;

        //    sDisplayOptions.bDisplayGlobalAxis = vm.ShowGlobalAxis;
        //    sDisplayOptions.bDisplayLocalMembersAxis = vm.ShowLocalMembersAxis;
        //    sDisplayOptions.bDisplaySurfaceLoadAxis = vm.ShowSurfaceLoadsAxis;

        //    sDisplayOptions.bDisplayLoads = vm.ShowLoads;
        //    sDisplayOptions.bDisplayNodalLoads = vm.ShowNodalLoads;
        //    sDisplayOptions.bDisplayMemberLoads = vm.ShowLoadsOnMembers;
        //    sDisplayOptions.bDisplayMemberLoads_Girts = vm.ShowLoadsOnGirts;
        //    sDisplayOptions.bDisplayMemberLoads_Purlins = vm.ShowLoadsOnPurlins;
        //    sDisplayOptions.bDisplayMemberLoads_Columns = vm.ShowLoadsOnColumns;
        //    sDisplayOptions.bDisplayMemberLoads_Frames = vm.ShowLoadsOnFrameMembers;
        //    sDisplayOptions.bDisplaySurfaceLoads = vm.ShowSurfaceLoads;

        //    sDisplayOptions.bDisplayLoadsLabels = vm.ShowLoadsLabels;
        //    sDisplayOptions.bDisplayLoadsLabelsUnits = vm.ShowLoadsLabelsUnits;

        //    sDisplayOptions.DisplayIn3DRatio = vm.DisplayIn3DRatio;
        //    sDisplayOptions.bColorsAccordingToMembers = vm.ColorsAccordingToMembers;
        //    sDisplayOptions.bColorsAccordingToSections = vm.ColorsAccordingToSections;
        //}

        private void UpdateAll()
        {
            CComponentListVM compList = (CComponentListVM)uc_ComponentList.DataContext;

            UpdateGeometryInputData();

            List<CConnectionJointTypes> joints = null;
            if (!vm.RecreateJoints) joints = vm.Model.m_arrConnectionJoints;
            else if(vm.Model != null) MessageBox.Show("Joints will be recreated and changed to defaults.");

            if (vm.RecreateModel)
            {
                // Create Model
                // Kitset Steel Gable Enclosed Buildings
                // TODO - nove parametre pre nastavenie hodnot zatazenia
                vm.Model = new CModel_PFD_01_GR(
                    sGeometryInputData,
                    vm.Frames,
                    vm.GirtDistance,
                    vm.PurlinDistance,
                    vm.ColumnDistance,
                    vm.BottomGirtPosition,
                    vm.FrontFrameRakeAngle,
                    vm.BackFrameRakeAngle,
                    DoorBlocksProperties,
                    WindowBlocksProperties,
                    compList,
                    joints);

                UpdateUC_Joints();
            }            

            bool generateSurfaceLoads = vm.ShowSurfaceLoadsAxis ||
                                        vm.GenerateSurfaceLoads ||
                                        vm.GenerateLoadsOnGirts ||
                                        vm.GenerateLoadsOnPurlins ||
                                        vm.GenerateLoadsOnColumns;

            // Calculate load values
            CalculateLoadingValues((CModel_PFD_01_GR)vm.Model);

            vm.Model.CalculateLoadValuesAndGenerateLoads(vm.GeneralLoad,
                vm.Wind,
                vm.Snow,
                vm.Eq,
                vm.GenerateNodalLoads,
                vm.GenerateLoadsOnGirts,
                vm.GenerateLoadsOnPurlins,
                vm.GenerateLoadsOnColumns,
                vm.GenerateLoadsOnFrameMembers,
                generateSurfaceLoads);

            if (vm.SynchronizeGUI)
            {
                // Create 3D window
                //UpdateDisplayOptions();
                sDisplayOptions = vm.GetDisplayOptions();

                Page3Dmodel page1 = new Page3Dmodel(vm.Model, sDisplayOptions, vm.Model.m_arrLoadCases[vm.LoadCaseIndex]);

                // Display model in 3D preview frame
                Frame1.Content = page1;
                Frame1.UpdateLayout();
            }
        }


        private void UpdateUC_Joints()
        {
            if (Joint_Input.Content == null)
            {
                UC_Joints uc_joints = new UC_Joints(vm);
                Joint_Input.Content = uc_joints;
                vm.JointsVM = uc_joints.DataContext as CJointsVM;
            }

            if (vm.RecreateJoints)
            {
                UC_Joints uc_joints = Joint_Input.Content as UC_Joints;
                uc_joints.ArrangeConnectionJoints();
                vm.JointsVM.JointTypeIndex = vm.JointsVM.JointTypeIndex; //redraw same selected joint
            }
        }

        private void UpdateGeometryInputData()
        {
            CComponentListVM compList = (CComponentListVM)uc_ComponentList.DataContext;
            // Set current geometry data to calculate loads
            sGeometryInputData.fH_2 = vm.fh2;
            sGeometryInputData.fH_1 = vm.WallHeight;
            sGeometryInputData.fW = vm.GableWidth;
            sGeometryInputData.fL = vm.Length;
            sGeometryInputData.fRoofPitch_deg = vm.RoofPitch_deg;

            CComponentInfo ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainColumn);
            if (ci != null) sGeometryInputData.iMainColumnFlyBracingEveryXXGirt = ci.ILS_Items.IndexOf(ci.ILS);

            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainRafter);
            if (ci != null) sGeometryInputData.iRafterFlyBracingEveryXXPurlin = ci.ILS_Items.IndexOf(ci.ILS);

            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgePurlin);
            if (ci != null) sGeometryInputData.iEdgePurlin_ILS_Number = ci.ILS_Items.IndexOf(ci.ILS);

            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (ci != null) sGeometryInputData.iGirt_ILS_Number = ci.ILS_Items.IndexOf(ci.ILS);

            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
            if (ci != null) sGeometryInputData.iPurlin_ILS_Number = ci.ILS_Items.IndexOf(ci.ILS);

            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.ColumnFrontSide);
            if (ci != null) sGeometryInputData.iFrontColumnFlyBracingEveryXXGirt = ci.ILS_Items.IndexOf(ci.ILS);
            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.ColumnBackSide);
            if (ci != null) sGeometryInputData.iBackColumnFlyBracingEveryXXGirt = ci.ILS_Items.IndexOf(ci.ILS);

            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide);
            if (ci != null) sGeometryInputData.iGirtFrontSide_ILS_Number = ci.ILS_Items.IndexOf(ci.ILS);
            ci = compList.ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtBackSide);
            if (ci != null) sGeometryInputData.iGirtBackSide_ILS_Number = ci.ILS_Items.IndexOf(ci.ILS);

        }

        private void SetUIElementsVisibility()
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;
            if (vm.ModelCalculatedResultsValid)
            {
                Internal_Forces.IsEnabled = true;
                Member_Design.IsEnabled = true;
                Joint_Design.IsEnabled = true;
                Footing_Design.IsEnabled = true;
                ButtonCalculateForces.IsEnabled = false;
            }
            else
            {
                Internal_Forces.IsEnabled = false;
                Member_Design.IsEnabled = false;
                Joint_Design.IsEnabled = false;
                Footing_Design.IsEnabled = false;
                ButtonCalculateForces.IsEnabled = true;
            }

            CComponentListVM compListVM = (CComponentListVM)uc_ComponentList.DataContext;
            if (compListVM.NoCompomentsForCalculate())
            {
                Internal_Forces.IsEnabled = false;
                ButtonCalculateForces.IsEnabled = false;
            }
            if (compListVM.NoCompomentsForDesign())
            {
                Member_Design.IsEnabled = false;
                Joint_Design.IsEnabled = false;
                Footing_Design.IsEnabled = false;
            }

            if (compListVM.NoCompomentsForMaterialList()) Part_List.IsEnabled = false;
            else Part_List.IsEnabled = true;
        }

        private void Clear3DModel_Click(object sender, RoutedEventArgs e)
        {
            Page3Dmodel page3D = (Page3Dmodel)Frame1.Content;
            ClearViewPort(page3D._trackport.ViewPort);
        }

        private void ClearViewPort(Viewport3D viewPort)
        {
            if (viewPort == null) return;
            if (viewPort.Children.Count == 0) return;

            Visual3DCollection.Enumerator myEnumer = viewPort.Children.GetEnumerator();
            myEnumer.Reset();
            while (myEnumer.MoveNext())
            {
                if (myEnumer.Current is _3DTools.ScreenSpaceLines3D)
                {
                    _3DTools.ScreenSpaceLines3D curLine = myEnumer.Current as _3DTools.ScreenSpaceLines3D;
                    curLine.Points.Clear(); //important
                }
            }
            viewPort.Children.Clear();
        }

        private void SystemComponentViewer_Click(object sender, RoutedEventArgs e)
        {
            SystemComponentViewer win = new SystemComponentViewer();
            win.Show();
        }

        private void View_2D_Click(object sender, RoutedEventArgs e)
        {
            Pokus2DView win = new Pokus2DView(vm.Model);
            win.Show();
        }

        private void ExportDXF_3D_Click(object sender, RoutedEventArgs e)
        {
            Page3Dmodel model3D = Frame1.Content as Page3Dmodel;
            CExportToDXF.ExportViewPort_DXF_Test(model3D._trackport.ViewPort);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl)) return; //pozor selection changed event buble smerom nahor

            if (MainTabControl.SelectedIndex == (int)ETabNames.eMember_Input)
            {
                //defined in xaml
                //if (Member_Input.Content == null) Member_Input.Content = new UC_ComponentList();
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eDoorsAndWindows)
            {
                if (Datagrid_DoorsAndGates.Items.Count > 0 && Datagrid_DoorsAndGates.SelectedIndex == -1) { Datagrid_DoorsAndGates.SelectedIndex = 0; Datagrid_DoorsAndGates_SelectionChanged(null, null); }
                else RedrawDoorOrWindowPreview();

                FrameDoorWindowPreview3D.Focus(); //asi nefunguje :-( stve ma ten focus na prvom riadku v prvom gride
                LabelDoors.Focus();
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eFooting_Input)
            {
                if (Footing_Input.Content == null) Footing_Input.Content = new UC_FootingInput();
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eLoads)
            {
                //if (Loads.Content == null) Loads.Content = new UC_Loads(sGeometryInputData);
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eLoadCases)
            {
                Load_Cases.Content = new UC_LoadCaseList(vm);
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eLoadCombinations)
            {
                Load_Combinations.Content = new UC_LoadCombinationList(vm.Model);
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eInternalForces)
            {
                //if (Member_Input.Content == null) Member_Input.Content = new UC_ComponentList();
                //UC_ComponentList component = Member_Input.Content as UC_ComponentList;
                CComponentListVM compList = (CComponentListVM)uc_ComponentList.DataContext;

                if (Internal_Forces.Content == null)
                {
                    Internal_Forces.Content = new UC_InternalForces(
                    vm.UseCRSCGeometricalAxes,
                    vm.Model,
                    compList,
                    vm.MemberInternalForcesInLoadCombinations,
                    vm.MemberDeflectionsInLoadCombinations,
                    vm.MemberDesignResults_ULS,
                    vm.MemberDesignResults_SLS,
                    vm.frameModels
                    );
                }
                else
                {
                    //setuje sa v public void UpdateResults()
                    //UC_InternalForces uc_intForces = Internal_Forces.Content as UC_InternalForces;
                    //uc_intForces.MemberDesignResults_SLS = vm.MemberDesignResults_SLS;
                    //uc_intForces.MemberDesignResults_ULS = vm.MemberDesignResults_ULS;
                    //uc_intForces.ListMemberInternalForcesInLoadCombinations = vm.MemberInternalForcesInLoadCombinations;
                }
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eMemberDesign)
            {
                CComponentListVM compListVM = (CComponentListVM)uc_ComponentList.DataContext;

                if (Member_Design.Content == null)
                {
                    Member_Design.Content = new UC_MemberDesign(vm.UseCRSCGeometricalAxes, vm.Model, compListVM, vm.MemberDesignResults_ULS, vm.MemberDesignResults_SLS); ;
                }
                else
                {
                    ////setuje sa v public void UpdateResults()
                    //UC_MemberDesign uc_memberDesign = Member_Design.Content as UC_MemberDesign;
                    //uc_memberDesign.DesignResults_SLS = vm.MemberDesignResults_SLS;
                    //uc_memberDesign.DesignResults_ULS = vm.MemberDesignResults_ULS;

                    //CPFDMemberDesign memberDesignVM = uc_memberDesign.DataContext as CPFDMemberDesign;
                    //memberDesignVM.SetComponentList(compListVM.ComponentList);
                }

            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eJointDesign)
            {
                //if (Member_Input.Content == null) Member_Input.Content = new UC_ComponentList();
                //UC_ComponentList component = Member_Input.Content as UC_ComponentList;
                CComponentListVM compListVM = (CComponentListVM)uc_ComponentList.DataContext;
                if (Joint_Design.Content == null) Joint_Design.Content = new UC_JointDesign(vm.UseCRSCGeometricalAxes, vm.Model, compListVM, vm.JointDesignResults_ULS);
                else
                {
                    ////setuje sa v public void UpdateResults()
                    //UC_JointDesign uc_jointDesign = Joint_Design.Content as UC_JointDesign;
                    //uc_jointDesign.DesignResults_ULS = vm.JointDesignResults_ULS;

                    //CPFDJointsDesign jointsDesignVM = uc_jointDesign.DataContext as CPFDJointsDesign;
                    //jointsDesignVM.SetComponentList(compListVM.ComponentList);                    
                }
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eJoint_Input)
            {
                //if (Joint_Input.Content == null) Joint_Input.Content = new UC_Joints(vm);
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.ePartList)
            {
                Part_List.Content = new UC_MaterialList(vm.Model);
            }
            else
            {
                // Not implemented like UC;
            };
        }

        public void ShowMessageBoxInPFDWindow(string text)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(text);
            });
        }

        public void UpdateResults()
        {
            Dispatcher.Invoke(() =>
            {
                ObservableCollection<CComponentInfo> componentsList = new ObservableCollection<CComponentInfo>();

                if (uc_ComponentList.DataContext != null)
                {
                    CComponentListVM compListVM = uc_ComponentList.DataContext as CComponentListVM;
                    componentsList = compListVM.ComponentList;
                }

                if (Internal_Forces.Content != null)
                {
                    UC_InternalForces uc_intForces = Internal_Forces.Content as UC_InternalForces;
                    uc_intForces.MemberDesignResults_SLS = vm.MemberDesignResults_SLS;
                    uc_intForces.MemberDesignResults_ULS = vm.MemberDesignResults_ULS;
                    uc_intForces.ListMemberInternalForcesInLoadCombinations = vm.MemberInternalForcesInLoadCombinations;
                    uc_intForces.FrameModels = vm.frameModels;

                    CPFDMemberInternalForces vmIF = uc_intForces.DataContext as CPFDMemberInternalForces;
                    vmIF.IsSetFromCode = true;
                    vmIF.LimitStateIndex = 0;
                    vmIF.SetComponentList(componentsList);
                    vmIF.IsSetFromCode = false;
                    vmIF.ComponentTypeIndex = 0;
                }

                if (Member_Design.Content != null)
                {
                    UC_MemberDesign uc_memberDesign = Member_Design.Content as UC_MemberDesign;
                    uc_memberDesign.DesignResults_SLS = vm.MemberDesignResults_SLS;
                    uc_memberDesign.DesignResults_ULS = vm.MemberDesignResults_ULS;
                    CPFDMemberDesign vmMD = uc_memberDesign.DataContext as CPFDMemberDesign;
                    vmMD.IsSetFromCode = true;
                    vmMD.LimitStateIndex = 0;
                    vmMD.SetComponentList(componentsList);
                    vmMD.IsSetFromCode = false;
                    vmMD.ComponentTypeIndex = 0;
                }

                if (Joint_Design.Content != null)
                {
                    UC_JointDesign uc_jointDesign = Joint_Design.Content as UC_JointDesign;
                    uc_jointDesign.DesignResults_ULS = vm.JointDesignResults_ULS;
                    CPFDJointsDesign vmJD = uc_jointDesign.DataContext as CPFDJointsDesign;
                    vmJD.IsSetFromCode = true;
                    vmJD.LimitStateIndex = 0;
                    vmJD.SetComponentList(componentsList);
                    vmJD.IsSetFromCode = false;
                    vmJD.ComponentTypeIndex = 0;
                }
            });
        }

        private void WindPressureButton_Click(object sender, RoutedEventArgs e)
        {
            WindPressureCalculator win = new WindPressureCalculator();
            win.Show();
        }

        private void PurlinDesignerButton_Click(object sender, RoutedEventArgs e)
        {
            PurlinDesigner win = new PurlinDesigner();
            win.Show();
        }
        

        private void chbDisplayLoadsOnFrames_Checked(object sender, RoutedEventArgs e)
        {
            // To Ondrej - popis

            // 1. nacitat objekt CModel_PFD_01_GR

            CPFDViewModel vm = this.DataContext as CPFDViewModel;
            CModel_PFD_01_GR model = vm.Model as CModel_PFD_01_GR;
            double limit = 0.0000001;

            // 2. Najst pruty ramu
            // 2.(a) prva moznost - najst members v rovinach s Y = i * fL1_frame, ktore su typu Main Column (EMemberGroupNames.eMainColumn) alebo Rafter (EMemberGroupNames.eRafter) vid listOfModelMemberGroups
            // i je index ramu 0 - iFrameNo
            // 2.(b) druha moznost - pruty ramu sa generuju ako prve spolu s eaves purlins, vieme ze ram ma 2 stlpy a 2 raftery, ramy su spojene na konci dvomi eave purlins takze v jednom cykle je vyrobenych 6 prutov
            // vid CModel_PFD_01_GR line 338 - 354

            List<List<CMember>> frames = new List<List<CMember>>();

            for (int i = 0; i < model.iFrameNo; i++)
            {
                List<CMember> frameMembers = new List<CMember>();

                foreach (CMemberGroup gr in model.listOfModelMemberGroups)
                {
                    foreach (CMember m in gr.ListOfMembers)
                    {
                        //it is not Main/End Column and it is not Main/End rafter
                        if (m.EMemberType != EMemberType_FS.eMC && m.EMemberType != EMemberType_FS.eMR &&
                            m.EMemberType != EMemberType_FS.eEC && m.EMemberType != EMemberType_FS.eER) continue;

                        if (MathF.d_equal(m.PointStart.Y, i * model.fL1_frame, limit))
                        {
                            frameMembers.Add(m);

                            if (bDebugging)
                                System.Diagnostics.Trace.WriteLine($"ID: {m.ID}, Name: {m.Name}, {m.PointStart.Y}");
                        }
                    }
                }

                frames.Add(frameMembers);
            }

            // Frame 1
            // Member ID 1 - Main Column
            // Member ID 2 - Rafter
            // Member ID 3 - Rafter
            // Member ID 4 - Main Column
            // Member ID 5 - Eave Purlin
            // Member ID 6 - Eave Purlin
            // Frame 2
            // Member ID 1+6 - Main Column
            // Member ID 2+6 - Rafter
            // Member ID 3+6 - Rafter
            // Member ID 4+6 - Main Column
            // Member ID 5+6 - Eave Purlin
            // Member ID 6+6 - Eave Purlin

            // a takto sa to opakuje s tym ze posledne 2 eave purlin sa nevytvoria lebo ramov je o jeden viac nez bays

            // 3. Ked mame uspesne identifikovane pruty ramu na ktore chceme generovat zatazenie
            // tak pre kazdy load case a kazdy objekt SurfaceLoad zo zoznamu zatazeni v danom load case
            // aplikujeme funkciu ktora zisti ci sa plocha zatazenia alebo jej cast nachadza v takzvanej zatazovacej ploche pruta L_member * fL1_frame (resp. 0.5 fL1_frame pre krajne ramy)
            // To znamena ze niektory z definicnych bodov Surface Load ma globalnu suradnicu Y z intervalu <i*Y - 0.5 * L1; i*Y + 0.5 * L1>

            // Zistime ci je prut ramu v danej ploche SurfaceLoad len z casti alebo cely (podla suradnice x pruta)
            // Podla toho sa potom vygeneruje objekt CMLoad_21 alebo CMLoad_24
            // 4. Hodnota zatazenia sa urci zo zatazovacej sirky ramu fL1_frame, pripadne 0.5 * fL1_frame ak sa v oblasti <i*Y - 0.5 * L1; i*Y + 0.5 * L1> v lokacii x pruta nachadza len jedna plocha (vid obrazok 10)
            // 5. Hodnota zatazenia sa urci zo sumy zatazovacich sirok * SurfaceLoad fValue, pre vsetky surface loads, ktore spadaju do oblasti <i*Y - 0.5 * L1; i*Y + 0.5 * L1> v lokacii x pruta (vid obrazok 11)

            // Do hodnoty zatazenia sa zohladnia vsetky objekty SurfaceLoad ktore sa nachadzaju v danej zatazovacej sirke pruta
            // Je potrebne prepocitat smer a urcit znamienko zatazenia medzi SurfaceLoad a novym CM_Load. Zatazenie mozeme generovat v LCS pruta alebo v GCS.
            // Asi bude lepsie pouzit vzdy LCS.

            //Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001)

            int frameCount = 0;
            foreach (List<CMember> frame in frames)
            {
                foreach (CMember m in frame)
                {
                    frameCount++;
                    bool isOuterFrame = (frameCount == 1 || frameCount == frames.Count);
                    foreach (CLoadCase loadCase in model.m_arrLoadCases)
                    {
                        foreach (CSLoad_Free load in loadCase.SurfaceLoadsList)
                        {
                            //musis najst CSLoad_Free ktorych niektory bod ma suradnicu Y do v intervale < i * Y - 0.5L1, i* Y +0.5L1 >
                            if (load is CSLoad_FreeUniformGroup)
                            {
                                foreach (CSLoad_FreeUniform l in ((CSLoad_FreeUniformGroup)load).LoadList)
                                {
                                    SetLoadGCSCoordinates(l);

                                    if (l.pSurfacePoints == null || l.pSurfacePoints.Count == 0 ||
                                        l.PointsGCS == null || l.PointsGCS.Count == 0)
                                    {
                                        /*
                                        // Model group sa nevyrobi ak je hodnota zatazenia nulova, mali by sme to vsetko preskakovat ak zatazenie neexistuje alebo je zatazenie nulove
                                        Model3DGroup gr = load.CreateM_3D_G_Load();
                                        // Catch null or empty list of definition points
                                        throw new ArgumentNullException("Load Case Name: " + loadCase.Name + "\n" +
                                           "Load Geometry 3D Model Items: " + gr.Children.Count.ToString());
                                        */
                                    }

                                    GetTributaryWidth_B(l, m, model.fL1_frame);

                                    //if (IsLoadForMember(l, m, model.fL1_frame)) CreateLoadOnMember(loadCase, l, m, model.fL1_frame, isOuterFrame);
                                }
                            }
                            else if (load is CSLoad_FreeUniform)
                            {
                                SetLoadGCSCoordinates((CSLoad_FreeUniform)load);

                                if (load.pSurfacePoints == null || load.pSurfacePoints.Count == 0 ||
                                    load.PointsGCS == null || load.PointsGCS.Count == 0)
                                {
                                    /*
                                    // Model group sa nevyrobi ak je hodnota zatazenia nulova, mali by sme to vsetko preskakovat ak zatazenie neexistuje alebo je zatazenie nulove
                                    Model3DGroup gr = load.CreateM_3D_G_Load();
                                    // Catch null or empty list of definition points
                                    throw new ArgumentNullException("Load Case Name: " + loadCase.Name + "\n" +
                                       "Load Geometry 3D Model Items: " + gr.Children.Count.ToString());
                                    */
                                }

                                GetTributaryWidth_B((CSLoad_FreeUniform)load, m, model.fL1_frame);
                                //if (IsLoadForMember((CSLoad_FreeUniform)load, m, model.fL1_frame)) CreateLoadOnMember(loadCase, (CSLoad_FreeUniform)load, m, model.fL1_frame, isOuterFrame);
                            }
                            else throw new Exception("Load type not known.");


                        }
                    }
                }
            }

            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                //chbDisplayLoadsOnPurlinsAndGirts.IsChecked = false;




                //foreach (CLoadCase load in model.m_arrLoadCases)
                //{
                //    System.Diagnostics.Trace.WriteLine(load.Name);
                //    List<CSLoad_Free> loadList = load.SurfaceLoadsList;
                //    foreach (CSLoad_Free l_free in load.SurfaceLoadsList)
                //    {
                //        if(l_free is CSLoad_FreeUniform)
                //            System.Diagnostics.Trace.WriteLine($"CSLoad_Free: {l_free.Name}, Points: {l_free.pSurfacePoints.ToString()}");

                //    }

                //}

                //UpdateAll();
            }
        }

        private void SetLoadGCSCoordinates(CSLoad_FreeUniform load)
        {
            load.PointsGCS = Drawing3D.GetLoadCoordinates_GCS(load, null, 0.001f);
        }

        private bool IsLoadForMember(CSLoad_FreeUniform load, CMember m, float fL1_frame)
        {
            foreach (Point3D p in load.PointsGCS)
            {
                if (m.NodeStart.Y - 0.5 * fL1_frame <= p.Y && m.NodeStart.Y + 0.5 * fL1_frame >= p.Y
                    || m.NodeEnd.Y - 0.5 * fL1_frame <= p.Y && m.NodeEnd.Y + 0.5 * fL1_frame >= p.Y)
                {
                    if (bDebugging)
                        System.Diagnostics.Trace.WriteLine($"found load: {load.fValue}_{load.ELoadType_FMTS} for member {m.Name} ID: {m.ID}");
                    return true;
                }
            }
            return false;
        }

        // Returns tributary width "b" which is representing a portion of surface load area transferred to the member
        // "b" is a dimension perpendicular to the member local x-axis
        private double GetTributaryWidth_B(CSLoad_FreeUniform load, CMember m, float fL1_frame)
        {
            // TO Ondrej, odpoved na tento problem je v podmienke if (Math.Abs(fValue) > 0) public class CSLoad_FreeUniform, line 257
            // Ak je hodnota zatazenia nulova tak sa ModelGroup nevyrobi a vracia to ModelGroup = null;

            if (load.PointsGCS.Count == 0) return 0; //toto by podla mna nemalo nastavat a just nastane // TO Ondrej - nastava pre nulovu hodnotu zatazenia, nevyrobi sa ModelGroup3D

            double MinLoadY = load.PointsGCS.Min(p => p.Y);
            double MaxLoadY = load.PointsGCS.Max(p => p.Y);

            double minY = m.NodeStart.Y - 0.5 * fL1_frame;
            double maxY = m.NodeStart.Y + 0.5 * fL1_frame;

            if (MaxLoadY < minY) return 0;
            else if (MinLoadY > maxY) return 0;

            if (MinLoadY < minY) MinLoadY = minY;
            if (MaxLoadY > maxY) MaxLoadY = maxY;

            double b = (MaxLoadY - MinLoadY) / (maxY - minY);
            if (bDebugging)
                System.Diagnostics.Trace.WriteLine($"found load: {load.fValue}_{load.ELoadType_FMTS} for member {m.Name} ID: {m.ID} b: {b}");
            return b;
        }

        private List<double> GetMemberX1X2(CSLoad_FreeUniform load, CMember m, float fL1_frame)
        {
            double x1 = 0;
            double x2 = 0;

            if (load.PointsGCS.Count == 0) return null; //toto by podla mna nemalo nastavat a just nastane // To Ondrej - vid komentar vyssie GetTributaryWidth_B

            if (load.ELoadDir == ELoadDirection.eLD_Z)
            {
                if (m.EMemberType == EMemberType_FS.eMR)
                {
                    //urcit x1,x2 pre member v LCS

                    //double minLoadY = load.PointsGCS
                    //m.NodeStart.Y

                }
            }
            else
            {
                //todo
            }

            return new List<double> { x1, x2 };
        }

        private void CreateLoadOnMember(CLoadCase loadCase, CSLoad_FreeUniform load, CMember m, float fL1_frame, bool isOuterFrame)
        {
            if (isOuterFrame)
            {
                CMLoad_21 l_21 = new CMLoad_21();
                l_21.Fq = load.fValue * fL1_frame * 0.5f;
                l_21.Member = m;
                //loadCase.MemberLoadsList.Add(l_21);
            }
            else
            {
                CMLoad_24 l_24 = new CMLoad_24();
                l_24.Fq = load.fValue * fL1_frame * 0.5f;
                l_24.Member = m;
                //loadCase.MemberLoadsList.Add(l_24);
            }
        }

        public void ShowBFEMNetModel(Model model)
        {
            //tu by som chcel zobrazit BFEMNEt model
            // stiahol som do projektu PFD HelixToolkit aj DynamicDataDisplay nuget packages...ale aj tak sa to nerozbehlo

            /*
            Dispatcher.Invoke(() =>
            {
                var wnd = new Window();
                var ctrl = new ModelVisualizerControl();
                ctrl.ModelToVisualize = model;

                wnd.Content = ctrl;

                wnd.ShowDialog();

            });*/
        }

        //private void btnDisplayDoorBlock_Click(object sender, RoutedEventArgs e)
        //{
        //    // Girt
        //    CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(1, 0.27f, 0.07f, 0.00115f, Colors.Orange);
        //    CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
        //    CMember refgirt = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, 1, 0, 0), crsc, 0);
        //    refgirt.EccentricityStart = eccentricity;
        //    refgirt.EccentricityEnd = eccentricity;
        //    refgirt.DTheta_x = Math.PI / 2;

        //    CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0, Colors.Green);
        //    CMember mColumnLeft = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);
        //    CMember mColumnRight = new CMember(0, new CNode(0, 1, 0, 0, 0), new CNode(1, 1, 0, 5, 0), crscColumn, 0);

        //    DoorProperties props = new DoorProperties();
        //    props.sBuildingSide = "Left";
        //    props.fDoorsHeight = 2.1f;
        //    props.fDoorsWidth = 0.9f;
        //    props.fDoorCoordinateXinBlock = 0.6f;
        //    CModel model = new CBlock_3D_001_DoorInBay(props, 0.5f, 0.3f, 0.9f, refgirt, mColumnLeft, mColumnRight, 4.5f, 4f, 0.3f);

        //    DisplayOptions sDisplayOptions = new DisplayOptions();
        //    sDisplayOptions.bUseDiffuseMaterial = true;
        //    sDisplayOptions.bUseEmissiveMaterial = true;
        //    sDisplayOptions.bUseLightAmbient = true;
        //    sDisplayOptions.bDisplaySolidModel = true;
        //    sDisplayOptions.bDisplayMembers = true;
        //    sDisplayOptions.bDisplayJoints = true;

        //    Page3Dmodel page1 = new Page3Dmodel(model, sDisplayOptions, null);

        //    // Display model in 3D preview frame
        //    Frame1.Content = page1;
        //    Frame1.UpdateLayout();

        //    /*
        //    Window2 win = new Window2(model, sDisplayOptions, m_bDebugging);
        //    list_trackports.Add(win._trackport);
        //    Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Block 3D 001 Door in bay" + " - Window " + (Container.Children.Count + 1) });
        //    win.Close();
        //    */
        //}

        //private void btnDisplayWindowBlock_Click(object sender, RoutedEventArgs e)
        //{
        //    // Girt
        //    CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(1, 0.27f, 0.07f, 0.00115f, Colors.Orange);
        //    CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
        //    CMember refgirt = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, 1, 0, 0), crsc, 0);
        //    refgirt.EccentricityStart = eccentricity;
        //    refgirt.EccentricityEnd = eccentricity;
        //    refgirt.DTheta_x = Math.PI / 2;

        //    CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0, Colors.Green);
        //    CMember mColumnLeft = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);
        //    CMember mColumnRight = new CMember(0, new CNode(0, 1, 0, 0, 0), new CNode(1, 1, 0, 5, 0), crscColumn, 0);

        //    WindowProperties prop = new WindowProperties();
        //    prop.sBuildingSide = "Left";
        //    prop.fWindowsHeight = 1.0f;
        //    prop.fWindowsWidth = 3.0f;
        //    prop.fWindowCoordinateXinBay = 0.9f;
        //    prop.fWindowCoordinateZinBay = 1.4f;
        //    prop.iNumberOfWindowColumns = 3;

        //    CModel model = new CBlock_3D_002_WindowInBay(prop, 0.5f, 0.3f, 0.8f, refgirt, mColumnLeft, mColumnRight, 6.0f, 2.8f, 0.3f);

        //    DisplayOptions sDisplayOptions = new DisplayOptions();
        //    sDisplayOptions.bUseDiffuseMaterial = true;
        //    sDisplayOptions.bUseEmissiveMaterial = true;
        //    sDisplayOptions.bUseLightAmbient = true;
        //    sDisplayOptions.bDisplaySolidModel = true;
        //    sDisplayOptions.bDisplayMembers = true;
        //    sDisplayOptions.bDisplayJoints = true;

        //    Page3Dmodel page1 = new Page3Dmodel(model, sDisplayOptions, null);

        //    // Display model in 3D preview frame
        //    Frame1.Content = page1;
        //    Frame1.UpdateLayout();

        //    /*
        //    Window2 win = new Window2(model, sDisplayOptions, m_bDebugging);
        //    list_trackports.Add(win._trackport);
        //    Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Block 3D 002 Window in bay" + " - Window " + (Container.Children.Count + 1) });
        //    win.Close();
        //    */
        //}

        private void Datagrid_DoorsAndGates_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Frame1.UpdateLayout();  // Nutne kvôli pridaniu riadku a update v GUI
        }

        private void Datagrid_Windows_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Frame1.UpdateLayout(); // Nutne kvôli pridaniu riadku a update v GUI
        }


        //temp To Mato - Som zakomentoval, ale pokial z toho nic nepotrebujeme,tak to treba zmazat

        //private void btnDisplayDoorBlock_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (btnDisplayWindowBlock.IsChecked == true) btnDisplayWindowBlock.IsChecked = false;

        //    CModel_PFD_01_GR modelPFD = vm.Model as CModel_PFD_01_GR;

        //    if (modelPFD.DoorsModels == null) return;
        //    if (modelPFD.DoorsModels.Count == 0) return;

        //    int index = Datagrid_DoorsAndGates.SelectedIndex;
        //    if (index < 0) index = 0;
        //    CModel doorModel = modelPFD.DoorsModels.ElementAtOrDefault(index);
        //    if (doorModel == null) return;

        //    //// Girt
        //    ////------------------------------------------------
        //    //// TODO 266 - prevziat parametre girt a columns zo skutocneho modelu, resp. zvoleneho bloku
        //    //// Tento kod by mal byt zmazany
        //    //CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(1, 0.27f, 0.07f, 0.00115f, Colors.Orange);
        //    //CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
        //    //CMember refgirt = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, 1, 0, 0), crsc, 0);
        //    //refgirt.EccentricityStart = eccentricity;
        //    //refgirt.EccentricityEnd = eccentricity;
        //    //refgirt.DTheta_x = Math.PI / 2;

        //    //CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0, Colors.Green);
        //    //CMember mColumnLeft = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);
        //    //CMember mColumnRight = new CMember(0, new CNode(0, 1, 0, 0, 0), new CNode(1, 1, 0, 5, 0), crscColumn, 0);
        //    ////------------------------------------------------



        //    //DoorProperties props = null;
        //    //if (Datagrid_DoorsAndGates.SelectedIndex != -1) props = vm.DoorBlocksProperties.ElementAtOrDefault(Datagrid_DoorsAndGates.SelectedIndex);
        //    //else props = vm.DoorBlocksProperties.FirstOrDefault();
        //    //if (props == null) return;

        //    //// TODO 266 - vsetky vstupne parametre konstruktora CBlock_3D_001_DoorInBay by sa mali prevziat z existujuceho bloku podla toho ktory riadok datagridu je selektovany
        //    //// V podstate by sme nemali tento blok vytvarat nanovo, ale len prevziat parametre bloku z hlavneho modelu (to asi teraz nie je dostupne)
        //    //// Prva moznost je ze si budeme bloky ukladat niekam do CModel_PFD_01_GR a potom ich tu len zobrazime podla vybraneho riadku v datagride.

        //    //// Druha moznost je vytvorit konrektny zobrazovany blok znova.
        //    //// V tom pripade by sme potrebovali zavolat cast metody CModel_PFD_01_GR, AddDoorBlock, tj. 
        //    //// 1. Nastavia sa vstupne parametre podla polohy bloku DeterminateBasicPropertiesToInsertBlock
        //    //// 2. Vyrobi sa blok door = new CBlock_3D_001_DoorInBay(....)

        //    //CModel model = new CBlock_3D_001_DoorInBay(props, 0.5f, 0.3f, 0.9f, refgirt, mColumnLeft, mColumnRight, 4.5f, 4f, 0.3f);

        //    DisplayOptions displayOptions = new DisplayOptions();
        //    displayOptions.bUseDiffuseMaterial = true;
        //    displayOptions.bUseEmissiveMaterial = true;
        //    displayOptions.bUseLightAmbient = true;
        //    displayOptions.bDisplaySolidModel = true;
        //    displayOptions.bDisplayMembers = true;
        //    displayOptions.bDisplayJoints = true;

        //    Page3Dmodel page1 = new Page3Dmodel(doorModel, displayOptions, null);

        //    // Display model in 3D preview frame
        //    Frame1.Content = page1;
        //    Frame1.UpdateLayout();
        //}

        //private void btnDisplayDoorBlock_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Page3Dmodel page1 = new Page3Dmodel(vm.Model, sDisplayOptions, vm.Model.m_arrLoadCases[vm.LoadCaseIndex]);

        //    // Display model in 3D preview frame
        //    Frame1.Content = page1;
        //    Frame1.UpdateLayout();
        //}

        //private void btnDisplayWindowBlock_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (btnDisplayDoorBlock.IsChecked == true) btnDisplayDoorBlock.IsChecked = false;

        //    //------------------------------------------------
        //    // TODO 266 - prevziat parametre girt a columns zo skutocneho modelu, resp. zvoleneho bloku
        //    // Tento kod by mal byt zmazany
        //    // Girt
        //    CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(1, 0.27f, 0.07f, 0.00115f, Colors.Orange);
        //    CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
        //    CMember refgirt = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, 1, 0, 0), crsc, 0);
        //    refgirt.EccentricityStart = eccentricity;
        //    refgirt.EccentricityEnd = eccentricity;
        //    refgirt.DTheta_x = Math.PI / 2;

        //    CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0, Colors.Green);
        //    CMember mColumnLeft = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);
        //    CMember mColumnRight = new CMember(0, new CNode(0, 1, 0, 0, 0), new CNode(1, 1, 0, 5, 0), crscColumn, 0);
        //    //------------------------------------------------



        //    WindowProperties props = null;
        //    if (Datagrid_Windows.SelectedIndex != -1) props = vm.WindowBlocksProperties.ElementAtOrDefault(Datagrid_Windows.SelectedIndex);
        //    else props = vm.WindowBlocksProperties.FirstOrDefault();
        //    if (props == null) return;

        //    // TODO 266 - vsetky vstupne parametre konstruktora CBlock_3D_002_WindowInBay by sa mali prevziat z existujuceho bloku podla toho ktory riadok datagridu je selektovany
        //    // V podstate by sme nemali tento blok vytvarat nanovo, ale len prevziat parametre bloku z hlavneho modelu (to asi teraz nie je dostupne)
        //    // Prva moznost je ze si budeme bloky ukladat niekam do CModel_PFD_01_GR a potom ich tu len zobrazime podla vybraneho riadku v datagride.

        //    // Druha moznost je vytvorit konrektny zobrazovany blok znova.
        //    // V tom pripade by sme potrebovali zavolat cast metody CModel_PFD_01_GR, AddWindowBlock, tj. 
        //    // 1. Nastavia sa vstupne parametre podla polohy bloku DeterminateBasicPropertiesToInsertBlock
        //    // 2. Vyrobi sa blok window = new CBlock_3D_001_WindowInBay(....)

        //    CModel model = new CBlock_3D_002_WindowInBay(props, 0.5f, 0.3f, 0.8f, refgirt, mColumnLeft, mColumnRight, 6.0f, 2.8f, 0.3f);

        //    DisplayOptions displayOptions = new DisplayOptions();
        //    displayOptions.bUseDiffuseMaterial = true;
        //    displayOptions.bUseEmissiveMaterial = true;
        //    displayOptions.bUseLightAmbient = true;
        //    displayOptions.bDisplaySolidModel = true;
        //    displayOptions.bDisplayMembers = true;
        //    displayOptions.bDisplayJoints = true;

        //    Page3Dmodel page1 = new Page3Dmodel(model, displayOptions, null);

        //    // Display model in 3D preview frame
        //    Frame1.Content = page1;
        //    Frame1.UpdateLayout();
        //}

        //private void btnDisplayWindowBlock_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Page3Dmodel page1 = new Page3Dmodel(vm.Model, sDisplayOptions, vm.Model.m_arrLoadCases[vm.LoadCaseIndex]);

        //    // Display model in 3D preview frame
        //    Frame1.Content = page1;
        //    Frame1.UpdateLayout();
        //}

        private void SetInitialItemsInComboboxes()
        {
            // Fill model combobox items
            CComboBoxHelper.FillComboboxValues("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "modelName", Combobox_Models);
            // Cladding (type and colors)
            CComboBoxHelper.FillComboboxValues("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting", "name", Combobox_RoofCladding);
            CComboBoxHelper.FillComboboxValues("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting", "name", Combobox_WallCladding);
            CComboBoxHelper.FillComboboxWithColors(Combobox_RoofCladdingColor);
            CComboBoxHelper.FillComboboxWithColors(Combobox_WallCladdingColor);

            Combobox_RoofCladdingColor.SelectedIndex = 8; // Default Permanent Green
            Combobox_WallCladdingColor.SelectedIndex = 8; // Default Permanent Green

            //// TODO - pocet poloziek by mohol byt zavisly na tom kolko purlins sa vygenerovalo, aby nebolo mozne nastavit vaznicu s vyssim poradim nez existuju na jednej priecli (rafter)
            //string[] sColumnFlyBracingPosition_Items = new string[10] {
            //"None",
            //"Every girt",
            //"Every 2nd girt",
            //"Every 3rd girt",
            //"Every 4th girt",
            //"Every 5th girt",
            //"Every 6th girt",
            //"Every 7th girt",
            //"Every 8th girt",
            //"Every 9th girt"};

            //Combobox_MainColumnFlyBracingPosition.ItemsSource = sColumnFlyBracingPosition_Items;
            //Combobox_FrontColumnFlyBracingPosition.ItemsSource = sColumnFlyBracingPosition_Items;
            //Combobox_BackColumnFlyBracingPosition.ItemsSource = sColumnFlyBracingPosition_Items;

            //Combobox_RafterFlyBracingPosition.ItemsSource = new string[10] {
            //"None",
            //"Every purlin",
            //"Every 2nd purlin",
            //"Every 3rd purlin",
            //"Every 4th purlin",
            //"Every 5th purlin",
            //"Every 6th purlin",
            //"Every 7th purlin",
            //"Every 8th purlin",
            //"Every 9th purlin"};

            //string[] sILS_Items = new string[]
            //{
            //    "None", "1", "2", "3", "4", "5"
            //};

            //Combobox_EdgePurlin_ILS_Number.ItemsSource = sILS_Items;
            //Combobox_Girt_ILS_Number.ItemsSource = sILS_Items;
            //Combobox_Purlin_ILS_Number.ItemsSource = sILS_Items;
            //Combobox_GirtFrontSide_ILS_Number.ItemsSource = sILS_Items;
            //Combobox_GirtBackSide_ILS_Number.ItemsSource = sILS_Items;
        }

        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            WaitWindow ww = new WaitWindow("XLS");
            ww.Show();

            CPFDViewModel vmPFD = this.DataContext as CPFDViewModel;
            CModelData modelData = vmPFD.GetModelData();

            List<string[]> list = new List<string[]>();

            try
            {
                Viewport3D viewPort = ((Page3Dmodel)Frame1.Content)._trackport.ViewPort;
                CMainReportExport.ReportAllDataToPDFFile(viewPort, modelData, list);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ww.Close();
            }
        }

        private void ExportWord_Click(object sender, RoutedEventArgs e)
        {
            CPFDViewModel vmPFD = this.DataContext as CPFDViewModel;
            //TODO tlacidlo povolit len vtedy ak su spocitane vysledky
            //zatial to nechcem pouzivat,aby som nestracal cas
            //if (!vmPFD.ModelCalculatedResultsValid) { MessageBox.Show("Please click Calculate to get valid results for report."); return; }

            WaitWindow ww = new WaitWindow("DOC");
            ww.Show();


            try
            {
                //vmPFD._loadInput = loadInput;
                CModelData modelData = vmPFD.GetModelData();

                //UC_InternalForces uc_intForces = Internal_Forces.Content as UC_InternalForces;
                Viewport3D viewPort = ((Page3Dmodel)Frame1.Content)._trackport.ViewPort;
                ExportToWordDocument.ReportAllDataToWordDoc(viewPort, modelData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ww.Close();
            }
        }

        private void btnProjectInfo_Click(object sender, RoutedEventArgs e)
        {
            ProjectInfo pi = new ProjectInfo(projectInfoVM);
            pi.ShowDialog();
        }

        int actualPreview = 0;
        private void RedrawDoorOrWindowPreview()
        {
            if (actualPreview == 2) RedrawWidowPreview();
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
            CModel_PFD_01_GR modelPFD = vm.Model as CModel_PFD_01_GR;

            if (modelPFD.DoorsModels == null) return;
            //if (modelPFD.DoorsModels.Count == 0) return;

            int index = Datagrid_DoorsAndGates.SelectedIndex;
            if (index < 0) index = 0;
            CModel doorModel = modelPFD.DoorsModels.ElementAtOrDefault(index);
            if (doorModel == null) doorModel = modelPFD.DoorsModels.FirstOrDefault();
            if (doorModel == null) doorModel = new CModel();

            DisplayOptions displayOptions = vm.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            displayOptions.bDisplayGlobalAxis = false;
            //Page3Dmodel page3D = new Page3Dmodel(doorModel, displayOptions, null);
            Page3Dmodel page3D = new Page3Dmodel(doorModel, displayOptions);

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
            RedrawWidowPreview();
        }

        private void RedrawWidowPreview()
        {
            //Mato??? - tieto komenty dole su aktualne? Lebo task 266 je uzavrety.
            // TO Ondrej - no mne sa zda ze preberame z bloku WindowProperties props
            // ale dalsie parametre samotneho bay, objekty stlpov, girts, odstup od stlpov atd tu nastavujem natvrdo,
            // Mali by sa tiez dat urcit z parametrov globalneho modelu
            // Tieto parametre sa totiz nastavuju pri vytvarani objektu bloku v CModel_01_PFD
            // vid metoda DeterminateBasicPropertiesToInsertBlock

            //------------------------------------------------
            // TODO 266 - prevziat parametre girt a columns zo skutocneho modelu, resp. zvoleneho bloku
            //
            // Tento kod by mal byt zmazany
            // Girt
            CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(1, 0.27f, 0.07f, 0.00115f, Colors.Orange);
            CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
            CMember refgirt = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, 1, 0, 0), crsc, 0);
            refgirt.EccentricityStart = eccentricity;
            refgirt.EccentricityEnd = eccentricity;
            refgirt.DTheta_x = Math.PI / 2;

            CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0, Colors.Green);
            CMember mColumnLeft = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);
            CMember mColumnRight = new CMember(0, new CNode(0, 1, 0, 0, 0), new CNode(1, 1, 0, 5, 0), crscColumn, 0);
            //------------------------------------------------

            WindowProperties props = null;
            if (Datagrid_Windows.SelectedIndex != -1) props = vm.WindowBlocksProperties.ElementAtOrDefault(Datagrid_Windows.SelectedIndex);
            if (props == null) props = vm.WindowBlocksProperties.FirstOrDefault();

            // TODO 266 - vsetky vstupne parametre konstruktora CBlock_3D_002_WindowInBay by sa mali prevziat z existujuceho bloku podla toho ktory riadok datagridu je selektovany
            // V podstate by sme nemali tento blok vytvarat nanovo, ale len prevziat parametre bloku z hlavneho modelu (to asi teraz nie je dostupne)
            // Prva moznost je ze si budeme bloky ukladat niekam do CModel_PFD_01_GR a potom ich tu len zobrazime podla vybraneho riadku v datagride.

            // Druha moznost je vytvorit konrektny zobrazovany blok znova.
            // V tom pripade by sme potrebovali zavolat cast metody CModel_PFD_01_GR, AddWindowBlock, tj. 
            // 1. Nastavia sa vstupne parametre podla polohy bloku DeterminateBasicPropertiesToInsertBlock
            // 2. Vyrobi sa blok window = new CBlock_3D_001_WindowInBay(....)

            CModel model;
            if (props == null) model = new CModel();
            else model = new CBlock_3D_002_WindowInBay(props, 0.5f, 0.3f, 0.8f, refgirt, mColumnLeft, mColumnRight, 6.0f, 2.8f, 0.3f);

            DisplayOptions displayOptions = vm.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            displayOptions.bDisplayGlobalAxis = false;
            //Page3Dmodel page3D = new Page3Dmodel(model, displayOptions, null);
            Page3Dmodel page3D = new Page3Dmodel(model, displayOptions);

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
    }
}
