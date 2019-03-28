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
            eJoint_Input = 2,
            eFooting_Input = 3,
            eLoads = 4,

            eLoadCases = 5,
            eLoadCombinations = 6,

            eInternalForces = 7,
            eMemberDesign = 8,
            eJointDesign = 9,
            eFootingDesign = 10,

            ePartList = 11,
            eQuoation = 12,
            eDoorsAndWindows = 13
        }

        ////////////////////////////////////////////////////////////////////////
        // PORTAL FRAME DESIGNER
        ////////////////////////////////////////////////////////////////////////

        bool bDebugging = false;

        //public CModel_PFD model;
        public CDatabaseModels dmodels; // Todo nahradit databazov modelov
        //public List<PropertiesToInsertOpening> DoorBlocksToInsertProperties;
        //public List<PropertiesToInsertOpening> WindowBlocksToInsertProperties;
        public ObservableCollection<DoorProperties> DoorBlocksProperties;
        public ObservableCollection<WindowProperties> WindowBlocksProperties;
        public CPFDViewModel vm;
        public CPFDLoadInput loadinput;
        public CCalcul_1170_1 generalLoad;
        public CCalcul_1170_2 wind;
        public CCalcul_1170_3 snow;
        public CCalcul_1170_5 eq;

        public DisplayOptions sDisplayOptions;
        public BuildingDataInput sBuildingInputData;
        public BuildingGeometryDataInput sGeometryInputData;
        public SnowLoadDataInput sSnowInputData;
        public WindLoadDataInput sWindInputData;
        public SeisLoadDataInput sSeisInputData;
        
        // TODO Ondrej pouzit na zobrazovanie jednotlivych zaloziek (tabitems) aktivitu tlacitok a podobne podla toho ci existuju vysledky pre vnutorne sily, posudenie prutov, posudenie spojov
        bool bInternalForcesResultsExists = false;
        bool bMemberDesignResultsExists = false;
        bool bJointDesignResultsExists = false;

        public MainWindow()
        {
            dmodels = new CDatabaseModels();

            // Initial Screen
            SplashScreen splashScreen = new SplashScreen("Resources/fs-screen.jpg");
            splashScreen.Show(false);
            InitializeComponent();
            splashScreen.Close(TimeSpan.FromMilliseconds(1000));

            // Fill model combobox items
            CComboBoxHelper.FillComboboxValues("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "modelName", Combobox_Models);
            // Cladding (type and colors)
            CComboBoxHelper.FillComboboxValues("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting", "name", Combobox_RoofCladding);
            CComboBoxHelper.FillComboboxValues("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting", "name", Combobox_WallCladding);
            CComboBoxHelper.FillComboboxWithColors(Combobox_RoofCladdingColor);
            CComboBoxHelper.FillComboboxWithColors(Combobox_WallCladdingColor);

            Combobox_RoofCladdingColor.SelectedIndex = 8; // Default Permanent Green
            Combobox_WallCladdingColor.SelectedIndex = 8; // Default Permanent Green

            
            // Prepare data for generating of door blocks
            DoorBlocksProperties = CDoorsAndWindowsHelper.GetDefaultDoorProperties();

            // Prepare data for generating of window blocks
            WindowBlocksProperties = CDoorsAndWindowsHelper.GetDefaultWindowsProperties();

            CComponentListVM compListVM = uc_ComponentList.DataContext as CComponentListVM;

            // Model Geometry
            vm = new CPFDViewModel(1, DoorBlocksProperties, WindowBlocksProperties, compListVM);
            vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            this.DataContext = vm;
            vm.PFDMainWindow = this;
            

            Combobox_RoofCladding.SelectedIndex = 1; //toto len kvoli nasledujucej metode,ktora sa inak zrube
            Combobox_WallCladding.SelectedIndex = 1; //toto len kvoli nasledujucej metode,ktora sa inak zrube

            // Calculate loading values as an input to draw loads in 3D
            CalculateLoadingValues();

            // TODO No. 199 - Ondrej - tu potrebujeme vyriesit zavislosti, pocitame zatazenie, ale to zavisi na geometrii modelu, takze sa to pocita s nulovymi rozmermi
            // Najprv by sme mali vyrobit len geometricky model a potom pocitat zatazenia, problem je ze tie sa teraz vyrabaju v spolocnej triede spolu s modelom
            // Takze v CModel_PFD_01_GR by trebalo oddelit fyzicku konstrukciu a load cases, load combinations, loads ....

            vm.GeneralLoad = generalLoad;
            vm.Wind = wind;
            vm.Snow = snow;
            vm.Eq = eq;
            vm.Loadinput = loadinput;

            //vm.CreateModel();

            FillComboboxTrapezoidalSheetingThickness(Combobox_RoofCladding.Items[vm.RoofCladdingIndex].ToString(), Combobox_RoofCladdingThickness);
            FillComboboxTrapezoidalSheetingThickness(Combobox_WallCladding.Items[vm.WallCladdingIndex].ToString(), Combobox_WallCladdingThickness);

            if (vm.MemberDesignResults_ULS == null)
            {
                //Internal_Forces.IsEnabled = false;
                //Member_Design.IsEnabled = false;
                //Joint_Design.IsEnabled = false;

                //Member_Design.Visibility = Visibility.Hidden;
                //Internal_Forces.Visibility = Visibility.Hidden;
            }
            else
            {
                //Internal_Forces.IsEnabled = true;
                //Member_Design.IsEnabled = true;
                //Joint_Design.IsEnabled = true;

                //Member_Design.Visibility = Visibility.Visible;
                //Internal_Forces.Visibility = Visibility.Visible;
            }

            //// Update display options
            //UpdateDisplayOptions();

            //// Create 3D window
            //Page3Dmodel page1 = new Page3Dmodel(vm.Model, sDisplayOptions, vm.Model.m_arrLoadCases[0]);

            //// Display model in 3D preview frame
            //Frame1.Content = page1;

            UpdateAll();

            vm.Model.GroupModelMembers();
        }

        //tu sa da spracovat  e.PropertyName a reagovat konkretne na to,ze ktora property bola zmenena vo view modeli
        protected void HandleViewModelPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is CPFDViewModel)
            {
                CPFDViewModel viewModel = sender as CPFDViewModel;
                if (viewModel != null && viewModel.IsSetFromCode) return; //ak je to property nastavena v kode napr. pri zmene typu modelu tak nic netreba robit

                if (e.PropertyName == "RoofCladdingIndex")
                {
                    FillComboboxTrapezoidalSheetingThickness(Combobox_RoofCladding.Items[viewModel.RoofCladdingIndex].ToString(), Combobox_RoofCladdingThickness);
                }
                else if (e.PropertyName == "WallCladdingIndex")
                {
                    FillComboboxTrapezoidalSheetingThickness(Combobox_WallCladding.Items[viewModel.WallCladdingIndex].ToString(), Combobox_WallCladdingThickness);
                }

                if (e.PropertyName == "Bays") return;
            }
            else if (sender is CComponentListVM)
            {
                CComponentListVM vm = sender as CComponentListVM;
                //if (e.PropertyName == "SelectedComponentIndex") return;  //osetrene uz v CPFDViewModel
                //else if (e.PropertyName == "ComponentDetailsList") return;
            }
            else if (sender is CComponentInfo)
            {
                CComponentInfo cInfo = sender as CComponentInfo;
                if (e.PropertyName == "Generate" && cInfo.ComponentName == "Girt - Front Side" && cInfo.Generate == false && AreDoorsOrWindowsOnBuildingSide("Front"))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete doors and windows in the front wall?", "Warning", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        RemoveDoorsAndWindowsBuildingSide("Front");
                    }
                    else
                    {
                        cInfo.Generate = true;
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

            //load the popup
            SplashScreen splashScreen = new SplashScreen("loading2.gif");
            splashScreen.Show(false);

            DeleteCalculationResults();
            UpdateAll();

            splashScreen.Close(TimeSpan.FromSeconds(0.1));
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

        private void CalculateLoadingValues()
        {
            // Input - TabItem Components
            //if (Member_Input.Content == null) Member_Input.Content = new UC_ComponentList();
            //UC_ComponentList componentList_UC = (UC_ComponentList)Member_Input.Content;
            //tu som nenasiel ziaden ViewModel napojeny na dany User Control
            //DataGrid grid = componentList_UC.Datagrid_Components;

            // Input - TabItem Loads
            UC_Loads loadInput_UC = null;
            if (Loads.Content == null) loadInput_UC = new UC_Loads(sGeometryInputData);
            else loadInput_UC = (UC_Loads)Loads.Content;
            loadinput = loadInput_UC.DataContext as CPFDLoadInput;

            // Basic data
            sBuildingInputData.location = (ELocation)loadinput.LocationIndex;                    // locations (cities) enum
            sBuildingInputData.fDesignLife_Value = loadinput.DesignLife_Value;                   // Database value in years
            sBuildingInputData.iImportanceClass = loadinput.ImportanceClassIndex + 1;            // Importance Level (index + 1)

            sBuildingInputData.fAnnualProbabilityULS_Snow = loadinput.AnnualProbabilityULS_Snow; // Annual Probability of Exceedence ULS - Snow
            sBuildingInputData.fAnnualProbabilityULS_Wind = loadinput.AnnualProbabilityULS_Wind; // Annual Probability of Exceedence ULS - Wind
            sBuildingInputData.fAnnualProbabilityULS_EQ = loadinput.AnnualProbabilityULS_EQ;     // Annual Probability of Exceedence ULS - EQ
            sBuildingInputData.fAnnualProbabilitySLS = loadinput.AnnualProbabilitySLS;           // Annual Probability of Exceedence SLS

            sBuildingInputData.fR_ULS_Snow = loadinput.R_ULS_Snow;
            sBuildingInputData.fR_ULS_Wind = loadinput.R_ULS_Wind;
            sBuildingInputData.fR_ULS_EQ = loadinput.R_ULS_EQ;
            sBuildingInputData.fR_SLS = loadinput.R_SLS;

            // Load Generation
            // General loading
            //toto tu tu proste nemoze byt, je nemozne volat tuto metodu skor ako je v combe nastavene Combobox_RoofCladding.SelectedItem
            float fMass_Roof = CComboBoxHelper.GetValueFromDatabasebyRowID("TrapezoidalSheetingSQLiteDB", (string)Combobox_RoofCladding.SelectedItem, "mass_kg_m2", vm.RoofCladdingThicknessIndex);
            float fMass_Wall = CComboBoxHelper.GetValueFromDatabasebyRowID("TrapezoidalSheetingSQLiteDB", (string)Combobox_WallCladding.SelectedItem, "mass_kg_m2", vm.WallCladdingThicknessIndex);

            // General Load (AS / NZS 1170.1)
            CalculateBasicLoad(fMass_Roof, fMass_Wall);

            // Wind  (AS / NZS 1170.2)
            CalculateWindLoad();

            // Snow  (AS / NZS 1170.3)
            CalculateSnowLoad();

            // TODO - napojit vstupy z TabItem Components a TabItem Main (rozmery, hmotnosti, pocet prvkov)
            // 30.7.2018

            // Temporary values - napojit na model a spocitat presne hmotnost ramu a zatazenie
            // Napojit na tab Compoment

            float fPurlinMassPerMeter = 20; // kg // TODO napojit na ComponentTab - zvoleny prierez pre Purlin a jeho hodnota A_g * rho_steel (prevziat z materialu prierezu)
            float fEdgePurlinMassPerMeter = 30; // kg // TODO napojit na ComponentTab - zvoleny prierez pre Eave Purlin a jeho hodnota A_g * rho_steel (prevziat z materialu prierezu)
            float fGirtMassPerMeter = 15; // kg // TODO napojit na ComponentTab - zvoleny prierez pre Girt a jeho hodnota A_g * rho_steel (prevziat z materialu prierezu)
            float fMainColumnMassPerMeter = 75; // kg // TODO napojit na ComponentTab - zvoleny prierez pre Main Column a jeho hodnota A_g * rho_steel (prevziat z materialu prierezu)
            float fMainRafterMassPerMeter = 85; // kg // TODO napojit na ComponentTab - zvoleny prierez pre Main Rafter a jeho hodnota A_g * rho_steel (prevziat z materialu prierezu)

            float fMainColumnMomentOfInteria_yu = 1.48e-4f; // m^4 // Box 63020, nacitat z databazy prierezov
            float fMainColumnMomentOfInteria_zv = 1.86e-5f; // m^4
            float fMainColumnMaterial_E = 2.1e+11f; // Pa

            // Napojit na parametre v CExample_3D_901_PF (dedi od obecneho modelu, mozno je spravnejsie pouzivat v projekte PFD CExample_3D_901_PF nez obecny CModel)
            // Obecny CModel tieto parametre nema a asi by ani nemal mat, mozeme vytvorit potomka CModel specialne pre projekt PFD

            int iNumberOfEavePurlins_x = 2; // TODO - napojit na model alebo priamo na example CExample_3D_901
            int iNumberOfPurlins_x = 10; // TODO - napojit na model alebo priamo na example CExample_3D_901
            int iNumberOfGirts_x = 8; // TODO - napojit na model alebo priamo na example CExample_3D_901
            int iNumberOfMainColumns_x = 2; // TODO - napojit na model alebo priamo na example CExample_3D_901
            int iNumberOfMainRafters_x = 2; // TODO - napojit na model alebo priamo na example CExample_3D_901

            float fLoadingWidth_Frame_x = vm.fL1; // Zatazovacia sirka ramu
            float fRafterLength = vm.GableWidth / (float)Math.Cos(vm.fRoofPitch_radians);

            float fMass_Purlins_x = iNumberOfPurlins_x * fPurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_EavePurlins_x = iNumberOfEavePurlins_x * fEdgePurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Girts_x = iNumberOfGirts_x * fGirtMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Frame_x = iNumberOfMainColumns_x * fMainColumnMassPerMeter * vm.WallHeight + iNumberOfMainRafters_x * fMainRafterMassPerMeter * fRafterLength;

            float fMass_Wall_x_kg = 2 * vm.WallHeight * fLoadingWidth_Frame_x * (fMass_Wall + (loadinput.AdditionalDeadActionWall * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2
            float fMass_Roof_x_kg = 2 * fRafterLength * fLoadingWidth_Frame_x * (fMass_Roof + (loadinput.AdditionalDeadActionRoof * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2

            float fMass_Total_x = fMass_Frame_x + fMass_Girts_x + fMass_Wall_x_kg + fMass_EavePurlins_x + fMass_Purlins_x + fMass_Roof_x_kg;

            float fT_1x = GetPeriod(iNumberOfMainColumns_x, vm.WallHeight, fMainColumnMomentOfInteria_yu, fMainColumnMaterial_E, fMass_Total_x);  // TODO  napojit Column Iy (AS 4600 - Ix)

            int iNumberOfMainColumns_y = 5; //  // TODO - napojit na model alebo priamo na example CExample_3D_901, pocet ramov (da sa pouzit aj vm z GUI)
            int iNumberOfMainRafters_y = iNumberOfMainColumns_y; // Pocet ramov je rovnaky ako pocet stlpov
            int iNumberOfPurlins_y = (iNumberOfMainColumns_y - 1) * (iNumberOfPurlins_x / 2); // Number of bays (number of frames - 1) * Number of purlins per half of building width
            int iNumberOfEavePurlins_y = (iNumberOfMainColumns_y - 1);
            int iNumberOfGirtsInWallPerMainColumn = 4; // TODO - napojit na model alebo priamo na example CExample_3D_901 (pocet girts na vysku stlpa)
            int iNumberOfGirts_y = (iNumberOfMainColumns_x - 1) * iNumberOfGirtsInWallPerMainColumn;

            float fMass_Purlins_y = iNumberOfPurlins_y * fPurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_EavePurlins_y = iNumberOfEavePurlins_y * fEdgePurlinMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Girts_y = iNumberOfGirts_y * fGirtMassPerMeter * fLoadingWidth_Frame_x;
            float fMass_Frame_y = iNumberOfMainColumns_y * fMainColumnMassPerMeter * vm.WallHeight + iNumberOfMainRafters_y * fMainRafterMassPerMeter * fRafterLength;

            float fLoadingWidth_Frame_y = 0.5f * vm.GableWidth; // Zatazovacia sirka ramu

            float fMass_Wall_y_kg = vm.Length * vm.WallHeight * (fMass_Wall + (loadinput.AdditionalDeadActionWall * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2
            float fMass_Roof_y_kg = vm.Length * fRafterLength * (fMass_Roof + (loadinput.AdditionalDeadActionRoof * 1000) / GlobalConstants.G_ACCELERATION); // NZS 1170.5, cl. 4.2

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

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            // Clear results of previous calculation
            DeleteCalculationResults();

            // TODO  - toto je potrebne presunut niekam k materialom / prierezom, moze sa nacitat pred vypoctom
            SetMaterialValuesFromDatabase();
            SetCrossSectionValuesFromDatabase();

            System.Diagnostics.Trace.WriteLine("After loading from DB : " + (DateTime.Now - start).TotalMilliseconds);

            vm.GenerateMemberLoadsIfNotGenerated();

            Solver solver = new Solver(vm.UseFEMSolverCalculationForSimpleBeam);
            vm.SolverWindow = solver;
            
            vm.Run();
            solver.ShowDialog();

            // TODO - implementovat vypocet
        }

        public void CalculateBasicLoad(float fMass_Roof, float fMass_Wall)
        {
            generalLoad = new CCalcul_1170_1(
                fMass_Roof,
                fMass_Wall,
                loadinput.AdditionalDeadActionRoof,
                loadinput.AdditionalDeadActionWall,
                loadinput.ImposedActionRoof,
                GlobalConstants.G_ACCELERATION);
        }

        public void CalculateSnowLoad()
        {
            sSnowInputData.eCountry = ECountry.eNewZealand; // Temporary - zatial nie je implementovana Australia
            sSnowInputData.eSnowRegion = (ESnowRegion)loadinput.SnowRegionIndex; // indexovane od 0, takze postacuje len previest na enum
            sSnowInputData.eExposureCategory = (ERoofExposureCategory)loadinput.ExposureCategoryIndex;
            sSnowInputData.fh_0_SiteElevation_meters = loadinput.SiteElevation;
            snow = new CCalcul_1170_3(sBuildingInputData, sGeometryInputData, sSnowInputData);
        }

        public void CalculateWindLoad()
        {
            sWindInputData.eWindRegion = loadinput.WindRegion;
            sWindInputData.iAngleWindDirection = loadinput.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = GetTerrainCategory(loadinput.TerrainCategoryIndex);

            wind = new CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData);
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
            sSeisInputData.eSiteSubsoilClass = loadinput.SiteSubSoilClass;
            sSeisInputData.fProximityToFault_D_km = loadinput.FaultDistanceDmin; // km
            sSeisInputData.fZoneFactor_Z = loadinput.ZoneFactorZ;
            sSeisInputData.fPeriodAlongXDirection_Tx = loadinput.PeriodAlongXDirectionTx; //sec
            sSeisInputData.fPeriodAlongYDirection_Ty = loadinput.PeriodAlongYDirectionTy; //sec
            sSeisInputData.fSpectralShapeFactor_Ch_Tx = loadinput.SpectralShapeFactorChTx;
            sSeisInputData.fSpectralShapeFactor_Ch_Ty = loadinput.SpectralShapeFactorChTy;

            eq = new CCalcul_1170_5(fT_1x_param, fT_1y_param, fMass_Total_x_param, fMass_Total_y_param, sBuildingInputData, sSeisInputData);
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

        private void UpdateDisplayOptions()
        {
            // Get display options from GUI

            sDisplayOptions.bUseLightDirectional = chbLightDirectional.IsChecked == true;
            sDisplayOptions.bUseLightPoint = chbLightPoint.IsChecked == true;
            sDisplayOptions.bUseLightSpot = chbLightSpot.IsChecked == true;
            sDisplayOptions.bUseLightAmbient = chbLightAmbient.IsChecked == true;

            sDisplayOptions.bUseDiffuseMaterial = chbMaterialDiffuse.IsChecked == true;
            sDisplayOptions.bUseEmissiveMaterial = chbMaterialEmissive.IsChecked == true;

            sDisplayOptions.bDisplayMembers = chbDisplayMembers.IsChecked == true;
            sDisplayOptions.bDisplayJoints = chbDisplayJoints.IsChecked == true;
            sDisplayOptions.bDisplayPlates = chbDisplayPlates.IsChecked == true;
            sDisplayOptions.bDisplayConnectors = chbDisplayConnectors.IsChecked == true;

            sDisplayOptions.bDisplayFoundations = chbDisplayFoundations.IsChecked == true;
            sDisplayOptions.bDisplayFloorSlab = chbDisplayFloorSlab.IsChecked == true;
            sDisplayOptions.bDisplayNodalSupports = chbDisplayNodalSupports.IsChecked == true;

            sDisplayOptions.bDisplayMemberDescription = vm.ShowMemberDescription;
            sDisplayOptions.bDisplayMemberID = vm.ShowMemberID;
            sDisplayOptions.bDisplayMemberPrefix = vm.ShowMemberPrefix;
            sDisplayOptions.bDisplayMemberCrossSectionStartName = vm.ShowMemberCrossSectionStartName;
            sDisplayOptions.bDisplayMemberRealLength = vm.ShowMemberRealLength;

            sDisplayOptions.bDisplayMembersCenterLines = chbDisplayMembersCenterLines.IsChecked == true;
            sDisplayOptions.bDisplaySolidModel = chbDisplaySolidModel.IsChecked == true;
            sDisplayOptions.bDisplayWireFrameModel = chbDisplayWireFrameModel.IsChecked == true;

            sDisplayOptions.bDistinguishedColor = chbDisplayDistinguishedColorMember.IsChecked == true;
            sDisplayOptions.bTransparentMemberModel = chbDisplayTransparentModelMember.IsChecked == true;

            sDisplayOptions.bDisplayGlobalAxis = vm.ShowGlobalAxis;
            sDisplayOptions.bDisplayLocalMembersAxis = vm.ShowLocalMembersAxis;
            sDisplayOptions.bDisplaySurfaceLoadAxis = vm.ShowSurfaceLoadsAxis;

            sDisplayOptions.bDisplayLoads = vm.ShowLoads;
            sDisplayOptions.bDisplayNodalLoads = vm.ShowNodalLoads;
            sDisplayOptions.bDisplayMemberLoads = vm.ShowLoadsOnMembers;
            sDisplayOptions.bDisplayMemberLoads_Girts = vm.ShowLoadsOnGirts;
            sDisplayOptions.bDisplayMemberLoads_Purlins = vm.ShowLoadsOnPurlins;
            sDisplayOptions.bDisplayMemberLoads_Columns = vm.ShowLoadsOnColumns;
            sDisplayOptions.bDisplayMemberLoads_Frames = vm.ShowLoadsOnFrameMembers;
            sDisplayOptions.bDisplaySurfaceLoads = vm.ShowSurfaceLoads;

            sDisplayOptions.bDisplayLoadsLabels = vm.ShowLoadsLabels;
            sDisplayOptions.bDisplayLoadsLabelsUnits = vm.ShowLoadsLabelsUnits;

            sDisplayOptions.DisplayIn3DRatio = vm.DisplayIn3DRatio;
        }

        private void UpdateAll()
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;
            
            CComponentListVM compList = (CComponentListVM)uc_ComponentList.DataContext;

            // Create Model
            // Kitset Steel Gable Enclosed Buildings

            // Set current geometry data to calculate loads
            sGeometryInputData.fH_2 = vm.fh2;
            sGeometryInputData.fH_1 = vm.WallHeight;
            sGeometryInputData.fW = vm.GableWidth;
            sGeometryInputData.fL = vm.Length;
            sGeometryInputData.fRoofPitch_deg = vm.RoofPitch_deg;
            
            CalculateLoadingValues();

            bool generateSurfaceLoads = vm.ShowSurfaceLoadsAxis ||
                vm.GenerateSurfaceLoads ||
                vm.GenerateLoadsOnGirts ||
                vm.GenerateLoadsOnPurlins ||
                vm.GenerateLoadsOnColumns;

            // TODO - nove parametre pre nastavenie hodnot zatazenia
            vm.Model = new CModel_PFD_01_GR(
                sGeometryInputData,
                //vm.WallHeight,
                //vm.GableWidth,
                //vm.fL1,
                vm.Frames,
                //vm.fh2,
                vm.GirtDistance,
                vm.PurlinDistance,
                vm.ColumnDistance,
                vm.BottomGirtPosition,
                vm.FrontFrameRakeAngle,
                vm.BackFrameRakeAngle,                
                DoorBlocksProperties,
                WindowBlocksProperties,
                compList?.ComponentList,
                generalLoad,
                wind,
                snow,
                eq,
                vm.GenerateNodalLoads,
                vm.GenerateLoadsOnGirts,
                vm.GenerateLoadsOnPurlins,
                vm.GenerateLoadsOnColumns,
                vm.GenerateLoadsOnFrameMembers,
                generateSurfaceLoads
                );

            // Create 3D window
            UpdateDisplayOptions();

            Page3Dmodel page1 = new Page3Dmodel(vm.Model, sDisplayOptions, vm.Model.m_arrLoadCases[vm.LoadCaseIndex]);

            // Display model in 3D preview frame
            Frame1.Content = page1;
            Frame1.UpdateLayout();
        }

        private void GetMinAndMaxValueInTheArray(float[,] array, out float min, out float max)
        {
            if (array != null)
            {
                min = max = array[0, 0];

                foreach (float f in array)
                {
                    if (Math.Abs(f) > Math.Abs(min))
                        min = f;

                    if (Math.Abs(f) > Math.Abs(max))
                        max = f;
                }
            }
            else // Exception
            {
                min = max = float.MaxValue;
            }
        }

        private void GetMinAndMaxValueInTheArray(float[] array, out float min, out float max)
        {
            if (array != null)
            {
                min = max = array[0];

                foreach (float f in array)
                {
                    if (Math.Abs(f) > Math.Abs(min))
                        min = f;

                    if (Math.Abs(f) > Math.Abs(max))
                        max = f;
                }
            }
            else // Exception
            {
                min = max = float.MaxValue;
            }
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
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eFooting_Input)
            {
                if (Footing_Input.Content == null) Footing_Input.Content = new UC_FootingInput();
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eLoads)
            {
                if (Loads.Content == null) Loads.Content = new UC_Loads(sGeometryInputData);
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
                    vm.MemberDesignResults_ULS,
                    vm.MemberDesignResults_SLS,
                    vm.frameModels
                    );
                }
                else
                {
                    UC_InternalForces uc_intForces = Internal_Forces.Content as UC_InternalForces;
                    uc_intForces.MemberDesignResults_SLS = vm.MemberDesignResults_SLS;
                    uc_intForces.MemberDesignResults_ULS = vm.MemberDesignResults_ULS;
                    uc_intForces.ListMemberInternalForcesInLoadCombinations = vm.MemberInternalForcesInLoadCombinations;
                }
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eMemberDesign)
            {
                //if (Member_Input.Content == null) Member_Input.Content = new UC_ComponentList();
                //UC_ComponentList component = Member_Input.Content as UC_ComponentList;
                CComponentListVM compList = (CComponentListVM)uc_ComponentList.DataContext;

                if (Member_Design.Content == null) Member_Design.Content = new UC_MemberDesign(vm.UseCRSCGeometricalAxes, vm.Model, compList, vm.MemberDesignResults_ULS, vm.MemberDesignResults_SLS);
                else
                {
                    UC_MemberDesign uc_memberDesign = Member_Design.Content as UC_MemberDesign;
                    uc_memberDesign.DesignResults_SLS = vm.MemberDesignResults_SLS;
                    uc_memberDesign.DesignResults_ULS = vm.MemberDesignResults_ULS;
                }
            }
            else if (MainTabControl.SelectedIndex == (int)ETabNames.eJointDesign)
            {
                //if (Member_Input.Content == null) Member_Input.Content = new UC_ComponentList();
                //UC_ComponentList component = Member_Input.Content as UC_ComponentList;
                CComponentListVM compList = (CComponentListVM)uc_ComponentList.DataContext;
                if (Joint_Design.Content == null) Joint_Design.Content = new UC_JointDesign(vm.UseCRSCGeometricalAxes, vm.Model, compList, vm.JointDesignResults_ULS);
                else
                {
                    UC_JointDesign uc_jointDesign = Joint_Design.Content as UC_JointDesign;
                    uc_jointDesign.DesignResults_ULS = vm.JointDesignResults_ULS;
                }
            }
            //else if (MainTabControl.SelectedIndex == (int)ETabNames.eDoorsAndWindows)
            //{
            //    DoorsAndWindows.Content = new UC_DoorsAndWindows(vm);
            //}
            else if (MainTabControl.SelectedIndex == (int)ETabNames.ePartList)
            {
                Part_List.Content = new UC_MaterialList(vm.Model);
            }
            else
            {
                // Not implemented like UC;
            };
        }

        // TO Ondrej - TODO - nemali by sme tieto metody pre checkboxy tiez prerobit na ViewModel properties ???

        private void chbLightDirectional_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbLightpoint_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbLightSpot_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbLightAmbient_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbLightDirectional_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbLightpoint_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbLightSpot_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbLightAmbient_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbMaterialDiffuse_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbMaterialEmissive_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbMaterialDiffuse_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbMaterialEmissive.IsChecked == false)
                    chbMaterialEmissive.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }

        private void chbMaterialEmissive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbMaterialDiffuse.IsChecked == false)
                    chbMaterialDiffuse.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }

        private void chbDisplayMembers_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayJoints_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayPlates_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbDisplayJoints.IsChecked == false)
                    chbDisplayJoints.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }
        private void chbDisplayConnectors_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbDisplayJoints.IsChecked == false)
                    chbDisplayJoints.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }
        private void chbDisplayFoundations_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbDisplayFoundations.IsChecked == false)
                    chbDisplayFoundations.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }
        private void chbDisplayFloorSlab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbDisplayFloorSlab.IsChecked == false)
                    chbDisplayFloorSlab.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }
        private void chbDisplayNodalSupports_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                if (chbDisplayNodalSupports.IsChecked == false)
                    chbDisplayNodalSupports.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();
            }
        }

        private void chbDisplayMembers_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayJoints_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayPlates_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayConnectors_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayFoundations_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayFloorSlab_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayNodalSupports_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        
        private void chbDisplayMembersCenterLines_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplaySolidModel_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayWireFrameModel_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                // TODO Ondrej - odstranit zavislost na solid model, tak aby bolo mozne zobrazit samostany wireframe
                if (chbDisplaySolidModel.IsChecked == false)
                    chbDisplaySolidModel.SetCurrentValue(CheckBox.IsCheckedProperty, true);

                UpdateAll();


                //if (model.WireFrameJoints != null)
                //{
                //    //((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Add(model.WireFrameMembers);
                //    //((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Add(model.WireFrameJoints);
                //}
                //else
                //{

                //}
            }
        }

        private void chbDisplayMembersCenterLines_UnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplaySolidModel_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayWireFrameModel_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();

                //int index = 0;
                //foreach (Visual3D visual in ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children)
                //{
                //    if (visual is ScreenSpaceLines3D)
                //    {
                //        if (((ScreenSpaceLines3D)visual).Name != null && ((ScreenSpaceLines3D)visual).Name.StartsWith("WireFrame")) break;
                //    }

                //    index++;
                //}
                //if(index < ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Count) ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.RemoveAt(index);

                //index = 0;
                //foreach (Visual3D visual in ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children)
                //{
                //    if (visual is ScreenSpaceLines3D)
                //    {
                //        if (((ScreenSpaceLines3D)visual).Name != null && ((ScreenSpaceLines3D)visual).Name.StartsWith("WireFrame")) break;
                //    }

                //    index++;
                //}
                //if (index < ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Count) ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.RemoveAt(index);
            }
        }

        private void chbDisplayDistinguishedColorMember_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayTransparentModelMember_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayDistinguishedColorMember_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayTransparentModelMember_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        
        public void UpdateProgressBarValue(double progressValue, string labelText)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = progressValue;
                progressBarLabel.Content = labelText;
            });
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
                if (Internal_Forces.Content != null)
                {
                    UC_InternalForces uc_intForces = Internal_Forces.Content as UC_InternalForces;
                    uc_intForces.MemberDesignResults_SLS = vm.MemberDesignResults_SLS;
                    uc_intForces.MemberDesignResults_ULS = vm.MemberDesignResults_ULS;
                    uc_intForces.ListMemberInternalForcesInLoadCombinations = vm.MemberInternalForcesInLoadCombinations;

                    CPFDMemberInternalForces vmIF = uc_intForces.DataContext as CPFDMemberInternalForces;
                    vmIF.LimitStateIndex = 0;
                }

                if (Member_Design.Content != null)
                {
                    UC_MemberDesign uc_memberDesign = Member_Design.Content as UC_MemberDesign;
                    uc_memberDesign.DesignResults_SLS = vm.MemberDesignResults_SLS;
                    uc_memberDesign.DesignResults_ULS = vm.MemberDesignResults_ULS;
                    CPFDMemberDesign vmMD = uc_memberDesign.DataContext as CPFDMemberDesign;
                    vmMD.LimitStateIndex = 0;
                }

                if (Joint_Design.Content != null)
                {
                    UC_JointDesign uc_jointDesign = Joint_Design.Content as UC_JointDesign;
                    uc_jointDesign.DesignResults_ULS = vm.JointDesignResults_ULS;
                    CPFDJointsDesign vmJD = uc_jointDesign.DataContext as CPFDJointsDesign;
                    vmJD.LimitStateIndex = 0;
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

        private void chbDisplayLoadsOnPurlinsAndGirts_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                //UpdateAll();
            }
        }

        private void chbDisplayLoadsOnPurlinsAndGirts_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                //chbDisplayLoadsOnFrames.IsChecked = false;
                //UpdateAll();
            }
        }

        private void chbDisplayLoadsOnFrames_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                //UpdateAll();
            }
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

                            if(bDebugging)
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
                    if(bDebugging)
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
            if(bDebugging)
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

        private void btnDisplayDoorBlock_Click(object sender, RoutedEventArgs e)
        {
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

            DoorProperties props = new DoorProperties();
            props.sBuildingSide = "Left";
            props.fDoorsHeight = 2.1f;
            props.fDoorsWidth = 0.9f;
            props.fDoorCoordinateXinBlock = 0.6f;
            CModel model = new CBlock_3D_001_DoorInBay(props, 0.5f, 0.3f, 0.9f, refgirt, mColumnLeft, mColumnRight, 4.5f, 4f, 0.3f);

            DisplayOptions sDisplayOptions = new DisplayOptions();
            sDisplayOptions.bUseDiffuseMaterial = true;
            sDisplayOptions.bUseEmissiveMaterial = true;
            sDisplayOptions.bUseLightAmbient = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayMembers = true;
            sDisplayOptions.bDisplayJoints = true;

            Page3Dmodel page1 = new Page3Dmodel(model, sDisplayOptions, null);

            // Display model in 3D preview frame
            Frame1.Content = page1;
            Frame1.UpdateLayout();

            /*
            Window2 win = new Window2(model, sDisplayOptions, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Block 3D 001 Door in bay" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
            */
        }

        private void btnDisplayWindowBlock_Click(object sender, RoutedEventArgs e)
        {
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

            WindowProperties prop = new WindowProperties();
            prop.sBuildingSide = "Left";
            prop.fWindowsHeight = 1.0f;
            prop.fWindowsWidth = 3.0f;
            prop.fWindowCoordinateXinBay = 0.9f;
            prop.fWindowCoordinateZinBay = 1.4f;
            prop.iNumberOfWindowColumns = 3;

            CModel model = new CBlock_3D_002_WindowInBay(prop, 0.5f, 0.3f, 0.8f, refgirt, mColumnLeft, mColumnRight, 6.0f, 2.8f, 0.3f);

            DisplayOptions sDisplayOptions = new DisplayOptions();
            sDisplayOptions.bUseDiffuseMaterial = true;
            sDisplayOptions.bUseEmissiveMaterial = true;
            sDisplayOptions.bUseLightAmbient = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayMembers = true;
            sDisplayOptions.bDisplayJoints = true;

            Page3Dmodel page1 = new Page3Dmodel(model, sDisplayOptions, null);

            // Display model in 3D preview frame
            Frame1.Content = page1;
            Frame1.UpdateLayout();

            /*
            Window2 win = new Window2(model, sDisplayOptions, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Block 3D 002 Window in bay" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
            */
        }

        private void Datagrid_DoorsAndGates_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Frame1.UpdateLayout();  //nutne koli pridaniu riadku a update v GUI
        }

        private void Datagrid_Windows_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Frame1.UpdateLayout(); //nutne koli pridaniu riadku a update v GUI
        }
    }
}
