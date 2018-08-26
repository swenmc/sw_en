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

namespace PFD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        ////////////////////////////////////////////////////////////////////////
        // PORTAL FRAME DESIGNER
        ////////////////////////////////////////////////////////////////////////

        bool bDebugging = false;

        public CModel model;
        public CDatabaseModels dmodels; // Todo nahradit databazov modelov
        public List<PropertiesToInsertOpening> DoorBlocksToInsertProperties;
        public List<PropertiesToInsertOpening> WindowBlocksToInsertProperties;
        public List<DoorProperties> DoorBlocksProperties;
        public List<WindowProperties> WindowBlocksProperties;
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

        // TODO - Ondrej zaviest staticku triedu pre fyzikalne konstanty, prevody jednotiek a podobne
        public const float fg_acceleration = 9.80665f; // gravitational acceleration [m/s^2]
        public float fMaterial_density = 7850f; //  [kg /m^3] (malo by byt zadane v databaze materialov)

        //int selected_Model_Index;
        //float fb; // 3 - 100 m
        //float fL; // 3 - 150 m
        //float fh; // 2 -  30 m (h1)
        //float fL1;
        //int iFrNo; // 2 - 30
        //float fRoofPitch_radians; // (zadavane v stupnoch - limity stupne 3 - 35 deg)
        //float fh2;
        //float fdist_girt; // 0.5 - 5 m
        //float fdist_purlin; // 0.5 - 5 m
        //float fdist_frontcolumn; // 1 - 10 m
        //float fdist_girt_bottom; // 1 - 10 m

        public MainWindow()
        {
            dmodels = new CDatabaseModels();

            // Initial Screen
            SplashScreen splashScreen = new SplashScreen("formsteel-screen.jpg");
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

            // TODO Ondrej - Bug No 43 - polozky farieb v comboboxe stratia farbu, ak boli vybrane

            // Model Geometry
            vm = new CPFDViewModel(1);
            vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            this.DataContext = vm;

            FillComboboxTrapezoidalSheetingThickness(Combobox_RoofCladding.Items[vm.RoofCladdingIndex].ToString(), Combobox_RoofCladdingThickness);
            FillComboboxTrapezoidalSheetingThickness(Combobox_WallCladding.Items[vm.WallCladdingIndex].ToString(), Combobox_WallCladdingThickness);

            // Set default values for cladding
            Combobox_RoofCladding.SelectedIndex = 0;
            Combobox_WallCladding.SelectedIndex = 0;
            Combobox_RoofCladdingThickness.SelectedIndex = 0;
            Combobox_WallCladdingThickness.SelectedIndex = 0;

            sGeometryInputData.fH_2 = vm.fh2;
            sGeometryInputData.fH_1 = vm.WallHeight;
            sGeometryInputData.fW = vm.GableWidth;
            sGeometryInputData.fL = vm.Length;
            sGeometryInputData.fRoofPitch_deg = vm.RoofPitch_deg;

            DataTable dt;
            // Prepare data for generating of door blocks
            dt = ((DataView)UC_doors.Datagrid_DoorsAndGates.ItemsSource).ToTable();

            DoorBlocksToInsertProperties = new List<PropertiesToInsertOpening>();
            DoorBlocksProperties = new List<DoorProperties>();
            // Fill list of door blocks
            PropertiesToInsertOpening insertOpeningDoors_temp;
            DoorProperties dp_temp;
            foreach (DataRow row in dt.Rows) // Create block for each not empty row in datatable
            {
                if (row.ItemArray != null && (string)row.ItemArray[0] != "") // Check that row is not empty and data are valid
                {
                    insertOpeningDoors_temp = new PropertiesToInsertOpening();
                    insertOpeningDoors_temp.sBuildingSide = (string)row.ItemArray[0];
                    insertOpeningDoors_temp.iBayNumber = (int)row.ItemArray[1];

                    DoorBlocksToInsertProperties.Add(insertOpeningDoors_temp);

                    dp_temp = new DoorProperties();
                    dp_temp.fDoorsHeight = float.Parse(row.ItemArray[2].ToString());
                    dp_temp.fDoorsWidth = float.Parse(row.ItemArray[3].ToString());
                    dp_temp.fDoorCoordinateXinBlock = float.Parse(row.ItemArray[4].ToString());

                    DoorBlocksProperties.Add(dp_temp);
                }
            }

            // Prepare data for generating of window blocks
            dt = ((DataView)UC_windows.Datagrid_Windows.ItemsSource).ToTable();

            WindowBlocksToInsertProperties = new List<PropertiesToInsertOpening>();
            WindowBlocksProperties = new List<WindowProperties>();
            // Fill list of window blocks
            PropertiesToInsertOpening insertOpeningWindows_temp;
            WindowProperties wp_temp;
            foreach (DataRow row in dt.Rows) // Create block for each not empty row in datatable
            {
                if (row.ItemArray != null && (string)row.ItemArray[0] != "") // Check that row is not empty and data are valid
                {
                    insertOpeningWindows_temp = new PropertiesToInsertOpening();
                    insertOpeningWindows_temp.sBuildingSide = (string)row.ItemArray[0];
                    insertOpeningWindows_temp.iBayNumber = (int)row.ItemArray[1];

                    WindowBlocksToInsertProperties.Add(insertOpeningWindows_temp);

                    wp_temp = new WindowProperties();
                    wp_temp.fWindowsHeight = float.Parse(row.ItemArray[2].ToString());
                    wp_temp.fWindowsWidth = float.Parse(row.ItemArray[3].ToString());
                    wp_temp.fWindowCoordinateXinBay = float.Parse(row.ItemArray[4].ToString());
                    wp_temp.fWindowCoordinateZinBay = float.Parse(row.ItemArray[5].ToString());
                    wp_temp.iNumberOfWindowColumns = int.Parse(row.ItemArray[6].ToString());

                    WindowBlocksProperties.Add(wp_temp);
                }
            }

            // Create Model
            // Kitset Steel Gable Enclosed Buildings

            // TODO Ondrej - zaroven by sa malo vygenerovat a nastavit aj zatazenie v modeli, pripadne sa to ma urobit po vygenerovani zakladnej geometrie
            // vid 3 nove parametre na konci
            // polozky z vm by asi bolo lepsie predavat ako nejaku strukturu zakladnej geometrie
            // vid public BuildingGeometryDataInput sGeometryInputData;

            // Calculate loading values as an input to draw loads in 3D
            CalculateLoadingValues();

            // Create 3D model of structure including loading
            model = new CExample_3D_901_PF(
                    vm.WallHeight,
                    vm.GableWidth,
                    vm.fL1, vm.Frames,
                    vm.fh2,
                    vm.GirtDistance,
                    vm.PurlinDistance,
                    vm.ColumnDistance,
                    vm.BottomGirtPosition,
                    vm.FrontFrameRakeAngle,
                    vm.BackFrameRakeAngle,
                    DoorBlocksToInsertProperties,
                    WindowBlocksToInsertProperties,
                    DoorBlocksProperties,
                    WindowBlocksProperties,
                    generalLoad,
                    wind,
                    snow,
                    eq);

            // Load cases
            // Fill combobox items
            foreach (CLoadCase loadcase in model.m_arrLoadCases)
                Combobox_LoadCase.Items.Add(loadcase.Name);

            //Combobox_LoadCase.SelectedIndex = 0; // Selected load case  - TOto spusti UpdateAll a model sa vytvara znovu

            // Update display options
            UpdateDisplayOptions();

            // Create 3D window
            Page3Dmodel page1 = new Page3Dmodel(model, sDisplayOptions, model.m_arrLoadCases[0]);

            // Display model in 3D preview frame
            Frame1.Content = page1;

            model.GroupModelMembers();
        }

        protected void HandleViewModelPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDViewModel viewModel = sender as CPFDViewModel;
            if (viewModel != null && viewModel.IsSetFromCode) return; //ak je to property nastavena v kode napr. pri zmene typu modelu tak nic netreba robit

            //tu sa da spracovat  e.PropertyName a reagovat konkretne na to,ze ktora property bola zmenena vo view modeli
            if (e.PropertyName == "RoofCladdingIndex")
            {
                FillComboboxTrapezoidalSheetingThickness(Combobox_RoofCladding.Items[viewModel.RoofCladdingIndex].ToString(), Combobox_RoofCladdingThickness);
            }
            else if (e.PropertyName == "WallCladdingIndex")
            {
                FillComboboxTrapezoidalSheetingThickness(Combobox_WallCladding.Items[viewModel.WallCladdingIndex].ToString(), Combobox_WallCladdingThickness);
            }


            //waiting = true;
            //BackgroundWorker bckWrk = new BackgroundWorker();
            //bckWrk.DoWork += new DoWorkEventHandler(bckWrk_DoWork);
            //bckWrk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bckWrk_RunWorkerCompleted);
            //bckWrk.RunWorkerAsync();

            //load the popup
            SplashScreen splashScreen = new SplashScreen("loading2.gif");
            splashScreen.Show(false);

            progressBar.Visibility = Visibility.Visible;
            Thread.Sleep(300);

            DeleteCalculationResults();
            UpdateAll();

            splashScreen.Close(TimeSpan.FromSeconds(0.1));
            progressBar.Value = 100;

            progressBar.Visibility = Visibility.Hidden;

            //waiting = false;
            //Thread.Sleep(2000);
            //bckWrk.Dispose();
            //MessageBox.Show("OK");
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

            CExample_2D_13_PF temp2Dmodel = new CExample_2D_13_PF(model.m_arrMat[0], model.m_arrCrSc[0], model.m_arrCrSc[1], vm.GableWidth, vm.WallHeight, vm.fh2, 1000, 1000, 1000, 1000);
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
            if (Model_Component.Content == null) Model_Component.Content = new UC_ComponentList();
            UC_ComponentList componentList_UC = (UC_ComponentList)Model_Component.Content;
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

            float fMass_Roof = CComboBoxHelper.GetValueFromDatabasebyRowID("TrapezoidalSheetingSQLiteDB", (string)Combobox_RoofCladding.SelectedItem, "mass_kg_m2", Combobox_RoofCladdingThickness.SelectedIndex);
            float fMass_Wall = CComboBoxHelper.GetValueFromDatabasebyRowID("TrapezoidalSheetingSQLiteDB", (string)Combobox_WallCladding.SelectedItem, "mass_kg_m2", Combobox_WallCladdingThickness.SelectedIndex);

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

            float fMass_Wall_x_kg = 2 * vm.WallHeight * fLoadingWidth_Frame_x * (fMass_Wall + (loadinput.AdditionalDeadActionWall * 1000) / fg_acceleration); // NZS 1170.5, cl. 4.2
            float fMass_Roof_x_kg = 2 * fRafterLength * fLoadingWidth_Frame_x * (fMass_Roof + (loadinput.AdditionalDeadActionRoof * 1000) / fg_acceleration); // NZS 1170.5, cl. 4.2

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

            float fMass_Wall_y_kg = vm.Length * vm.WallHeight * (fMass_Wall + (loadinput.AdditionalDeadActionWall * 1000) / fg_acceleration); // NZS 1170.5, cl. 4.2
            float fMass_Roof_y_kg = vm.Length * fRafterLength * (fMass_Roof + (loadinput.AdditionalDeadActionRoof * 1000) / fg_acceleration); // NZS 1170.5, cl. 4.2

            float fMass_Total_y = fMass_Frame_y + fMass_Girts_y + fMass_Wall_y_kg + fMass_EavePurlins_y + fMass_Purlins_y + fMass_Roof_y_kg;

            float fT_1y = GetPeriod(iNumberOfMainColumns_y, vm.WallHeight, fMainColumnMomentOfInteria_zv, fMainColumnMaterial_E, fMass_Total_y);  //  TODO  napojit Column Iz (AS 4600 - Iy)

            // NZS 1170.5, cl. 4.1.2.1 Rayleigh method, eq. 4.1(1)
            float fT_1x_RM_411 = GetPeriod_RM_NZS1107_5_Eq411(iNumberOfMainColumns_x, vm.WallHeight, fMainColumnMomentOfInteria_yu, fMainColumnMaterial_E, fMass_Total_x);
            float fT_1y_RM_411 = GetPeriod_RM_NZS1107_5_Eq411(iNumberOfMainColumns_y, vm.WallHeight, fMainColumnMomentOfInteria_zv, fMainColumnMaterial_E, fMass_Total_y);

            // Validation - compare calculated periods
            // Kontrola vypoctu frekvencii, assert v pripade ze rozdiel je viac nez 20% mensej z hodnot (TODO - idealne je spocitat z globalneho modelu alebo aspon 2D modelu ramu so zohladnenim poddajnosti prievlaku / edge purlin)
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
            // Clear results of previous calculation
            DeleteCalculationResults();

            // TODO  - toto je potrebne presunut niekam k materialom / prierezom, moze sa nacitat pred vypoctom
            SetMaterialValuesFromDatabase();
            SetCrossSectionValuesFromDatabase();

            // Temporary solution
            // Purlin

            // Loading Width
            // Dead Load

            // TODO Ondrej - ziskat z gridview components typ prierezu (polozka v stlpci cross-section
            // grid

            // TODO Ondrej - ziskat hodnotu z databazy
            //float fA_g = DatabaseManager.GetValueFromDatabasebyRowID("MDBSections", "tableSections_m", "A_g", 1, "section");

            float fA_g = (float)model.m_arrCrSc[4].A_g;
            float fPurlinSelfWeight = fA_g * fMaterial_density * fg_acceleration;
            float fPurlinDeadLoadLinear = generalLoad.fDeadLoadTotal_Roof * vm.PurlinDistance + fPurlinSelfWeight;
            float fPurlinImposedLoadLinear = loadinput.ImposedActionRoof * 1000 * vm.PurlinDistance;
            float fsnowValue = snow.fs_ULS_Nu_1 * ((0.5f * vm.GableWidth) / ((0.5f * vm.GableWidth) / (float)Math.Cos(vm.fRoofPitch_radians))); // Consider projection acc. to Figure 4.1
            float fPurlinSnowLoadLinear = fsnowValue * vm.PurlinDistance;

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TEMPORARY - vypocet na modeli jedneho pruta
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            float fPurlinWindLoadLinear = wind.fp_e_max_D_roof_ULS_Theta_4[0,0];

            float fp_i_min_min;
            float fp_i_min_max;
            float fp_i_max_min;
            float fp_i_max_max;

            GetMinAndMaxValueInTheArray(wind.fp_i_min_ULS_Theta_4, out fp_i_min_min, out fp_i_min_max);
            GetMinAndMaxValueInTheArray(wind.fp_i_max_ULS_Theta_4, out fp_i_max_min, out fp_i_max_max);

            float[] fp_e_min_min = new float[3];
            float[] fp_e_min_max = new float[3];
            float[] fp_e_max_min = new float[3];
            float[] fp_e_max_max = new float[3];

            GetMinAndMaxValueInTheArray(wind.fp_e_min_D_roof_ULS_Theta_4, out fp_e_min_min[0], out fp_e_min_max[0]);
            GetMinAndMaxValueInTheArray(wind.fp_e_min_U_roof_ULS_Theta_4, out fp_e_min_min[1], out fp_e_min_max[1]);
            GetMinAndMaxValueInTheArray(wind.fp_e_min_R_roof_ULS_Theta_4, out fp_e_min_min[2], out fp_e_min_max[2]);

            GetMinAndMaxValueInTheArray(wind.fp_e_max_D_roof_ULS_Theta_4, out fp_e_max_min[0], out fp_e_max_max[0]);
            GetMinAndMaxValueInTheArray(wind.fp_e_max_U_roof_ULS_Theta_4, out fp_e_max_min[1], out fp_e_max_max[1]);
            GetMinAndMaxValueInTheArray(wind.fp_e_max_R_roof_ULS_Theta_4, out fp_e_max_min[2], out fp_e_max_max[2]);

            float fp_e_min_min_value;
            float fp_e_min_max_value;
            float fp_e_max_min_value;
            float fp_e_max_max_value;

            GetMinAndMaxValueInTheArray(fp_e_min_min, out fp_e_min_min_value, out fp_e_min_max_value);
            GetMinAndMaxValueInTheArray(fp_e_max_max, out fp_e_max_min_value, out fp_e_max_max_value);

            float fp_min = fp_i_min_min + fp_e_min_min_value;
            float fp_max = fp_i_max_max + fp_e_max_max_value;

            float fWu_min_linear = fp_min * vm.PurlinDistance;
            float fWu_max_linear = fp_max * vm.PurlinDistance;

            // Transform loads from global coordinate system to the purlin coordinate system
            float fSinAlpha = (float)Math.Sin((vm.RoofPitch_deg / 180f) * MathF.fPI);
            float fCosAlpha = (float)Math.Cos((vm.RoofPitch_deg / 180f) * MathF.fPI);

            float fPurlinDeadLoadLinear_LCS_y = fPurlinDeadLoadLinear * fSinAlpha;
            float fPurlinDeadLoadLinear_LCS_z = fPurlinDeadLoadLinear * fCosAlpha;

            float fPurlinImposedLoadLinear_LCS_y = fPurlinImposedLoadLinear * fSinAlpha;
            float fPurlinImposedLoadLinear_LCS_z = fPurlinImposedLoadLinear * fCosAlpha;

            float fPurlinSnowLoadLinear_LCS_y = fPurlinSnowLoadLinear * fSinAlpha;
            float fPurlinSnowLoadLinear_LCS_z = fPurlinSnowLoadLinear * fCosAlpha;

            // Combinations of action
            // 4.2.2 Strength
            // Purlin (a) (b) (d) (e) (g)
            /*
            int iNumberOfLoadCombinations = 5;
            float[] fE_d_load_values_LCS_y = new float[iNumberOfLoadCombinations];

            // Ukazka generovania kombinacii

            fE_d_load_values_LCS_y[0] = 1.35f * fPurlinDeadLoadLinear_LCS_y;                                              // 4.2.2 (a)
            fE_d_load_values_LCS_y[1] = 1.20f * fPurlinDeadLoadLinear_LCS_y + 1.50f * fPurlinImposedLoadLinear_LCS_y;     // 4.2.2 (b)
            fE_d_load_values_LCS_y[2] = 1.20f * fPurlinDeadLoadLinear_LCS_y;                                              // 4.2.2 (d)
            fE_d_load_values_LCS_y[3] = 0.90f * fPurlinDeadLoadLinear_LCS_y;                                              // 4.2.2 (e)
            fE_d_load_values_LCS_y[4] = 1.20f * fPurlinDeadLoadLinear_LCS_y + fPurlinSnowLoadLinear_LCS_y;                // 4.2.2 (g)

            float[] fE_d_load_values_LCS_z = new float[iNumberOfLoadCombinations];

            fE_d_load_values_LCS_z[0] = 1.35f * fPurlinDeadLoadLinear_LCS_z;                                              // 4.2.2 (a)
            fE_d_load_values_LCS_z[1] = 1.20f * fPurlinDeadLoadLinear_LCS_z + 1.50f * fPurlinImposedLoadLinear_LCS_z;     // 4.2.2 (b)
            fE_d_load_values_LCS_z[2] = 1.20f * fPurlinDeadLoadLinear_LCS_z + fWu_max_linear;                             // 4.2.2 (d)
            fE_d_load_values_LCS_z[3] = 0.90f * fPurlinDeadLoadLinear_LCS_z + Math.Abs(fWu_min_linear);                   // 4.2.2 (e)
            fE_d_load_values_LCS_z[4] = 1.20f * fPurlinDeadLoadLinear_LCS_z + fPurlinSnowLoadLinear_LCS_z;                // 4.2.2 (g)
            */
            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;

            float[] fx_positions = new float[iNumberOfDesignSections];
            designMomentValuesForCb sMomentValuesforCb = new designMomentValuesForCb();
            basicInternalForces[] sBIF_x = null;

            // Tu by sa mal napojit FEM vypocet
            //RunFEMSOlver();
            float fMaximumDesignRatioWholeStructure = 0;
            float fMaximumDesignRatioGirts = 0;
            float fMaximumDesignRatioPurlins = 0;
            float fMaximumDesignRatioColumns = 0;

            CMember MaximumDesignRatioWholeStructureMember = new CMember();
            CMember MaximumDesignRatioGirt = new CMember();
            CMember MaximumDesignRatioPurlin = new CMember();
            CMember MaximumDesignRatioColumn = new CMember();

            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();
            List<CMemberLoadForces> listMemberLoadForces = new List<CMemberLoadForces>();

            // Calculate Internal Forces For Load Cases
            foreach (CMember m in model.m_arrMembers)
            {
                for (int i = 0; i < iNumberOfDesignSections; i++)
                    fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                m.MMomentValuesforCb = new List<designMomentValuesForCb>();
                m.MBIF_x = new List<basicInternalForces[]>();

                foreach (CLoadCase lc in model.m_arrLoadCases)
                {
                    // Calculate Internal forces just for Load Cases that are included in ULS
                    if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                    {
                        foreach (CMLoad cmload in lc.MemberLoadsList)
                        {
                            if (cmload.Member.ID == m.ID) // TODO - Zatial pocitat len pre zatazenia, ktore lezia priamo skumanom na prute, po zavedeni 3D solveru upravit
                            {
                                calcModel.CalculateInternalForcesOnSimpleBeam_PFD(iNumberOfDesignSections, fx_positions, m, (CMLoad_21)cmload, out sBIF_x, out sMomentValuesforCb);
                            }
                        }
                    }

                    listMemberLoadForces.Add(new CMemberLoadForces(m, lc, sBIF_x));
                    m.MMomentValuesforCb.Add(sMomentValuesforCb);
                    m.MBIF_x.Add(sBIF_x);
                }
            }

            // Design of members
            // Calculate Internal Forces For Load Cases
            foreach (CMember m in model.m_arrMembers)
            {
                for (int i = 0; i < iNumberOfDesignSections; i++)
                    fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                foreach (CLoadCombination lcomb in model.m_arrLoadCombs)
                {
                    // TODO - nacitat internal forces z obsahu LC ktore patria kombinacii a prenasobit faktormi
                    designMomentValuesForCb sMomentValuesforCb_design = new designMomentValuesForCb();
                    basicInternalForces[] sBIF_x_design = m.MBIF_x[0];

                    foreach (CLoadCase lc in lcomb.LoadCasesList)
                    {
                        CMemberLoadForces mlf = listMemberLoadForces.Find(i => i.Member.ID == m.ID && i.LoadCase.ID == lc.ID);
                        if (mlf != null)
                        {
                            int i = 0;
                            foreach (basicInternalForces bif in mlf.Forces)
                            {
                                sBIF_x_design[i].fV_yy = lc.Factor * bif.fV_yy;
                                i++;
                            }
                        }
                    }

                    // Design
                    designInternalForces[] sDIF_x;
                    CMemberDesign designModel = new CMemberDesign();
                    designModel.SetDesignForcesAndMemberDesign_PFD(iNumberOfDesignSections, m, sBIF_x_design, sMomentValuesforCb_design, out sDIF_x);

                    // Set maximum design ratio of whole structure
                    if (designModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                    {
                        fMaximumDesignRatioWholeStructure = designModel.fMaximumDesignRatio;
                        MaximumDesignRatioWholeStructureMember = m;
                    }

                    // Output (for debugging)
                    bDebugging = true; // Testovacie ucely
                    if (bDebugging)
                        Console.WriteLine("Member ID: " + m.ID + "\t | " +
                                          "Load Combination ID: " + lcomb.ID + "\t | " +
                                          "Design Ratio: " + Math.Round(designModel.fMaximumDesignRatio, 3).ToString());

                    // Output - set maximum design ratio by component Type
                    if (bDebugging)
                    {

                        switch (m.EMemberType)
                        {
                            case EMemberType_FormSteel.eG: // Girt
                                {
                                    if (designModel.fMaximumDesignRatio > fMaximumDesignRatioGirts)
                                    {
                                        fMaximumDesignRatioGirts = designModel.fMaximumDesignRatio;
                                        MaximumDesignRatioGirt = m;
                                    }
                                    break;
                                }
                            case EMemberType_FormSteel.eP: // Purlin
                                {
                                    if (designModel.fMaximumDesignRatio > fMaximumDesignRatioPurlins)
                                    {
                                        fMaximumDesignRatioPurlins = designModel.fMaximumDesignRatio;
                                        MaximumDesignRatioPurlin = m;
                                    }
                                    break;
                                }
                            case EMemberType_FormSteel.eC: // Column
                                {
                                    if (designModel.fMaximumDesignRatio > fMaximumDesignRatioColumns)
                                    {
                                        fMaximumDesignRatioColumns = designModel.fMaximumDesignRatio;
                                        MaximumDesignRatioColumn = m;
                                    }
                                    break;
                                }
                            default:
                                // TODO - modifikovat podla potrieb pre ukladanie - doplnit vsetky typy
                                break;
                        }
                    }
                }
            }

            // TODO Ondrej, zostavovat modely a pocitat vn. sily by malo stacit len pre load cases
            // Pre Load Combinations by sme mali len poprenasobovat hodnoty z load cases faktormi a spocitat ich hodnoty ako jednoduchy sucet, nemusi sa vytvarat nahradny vypoctovy model
            // Potom by mal prebehnut cyklus pre design (vsetky pruty a vsetky load combination, ale uz len pre designModel s hodnotami vn sil v rezoch)

            MessageBox.Show("Calculation Results \n" +
                    "Maximum design ratio \n" +
                    "Member ID: " + MaximumDesignRatioWholeStructureMember.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioWholeStructure, 3).ToString() + "\n\n" +

                    "Maximum design ratio - girts\n" +
                    "Member ID: " + MaximumDesignRatioGirt.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioGirts, 3).ToString() + "\n\n" +
                    "Maximum design ratio - purlins\n" +
                    "Member ID: " + MaximumDesignRatioPurlin.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioPurlins, 3).ToString() + "\n\n" +
                    "Maximum design ratio - columns\n" +
                    "Member ID: " + MaximumDesignRatioColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioColumns, 3).ToString() + "\n\n"
                    );
        }

        public void CalculateBasicLoad(float fMass_Roof, float fMass_Wall)
        {
            generalLoad = new CCalcul_1170_1(
                fMass_Roof,
                fMass_Wall,
                loadinput.AdditionalDeadActionRoof,
                loadinput.AdditionalDeadActionWall,
                loadinput.ImposedActionRoof,
                fg_acceleration);
        }

        public void CalculateSnowLoad()
        {
            sSnowInputData.eCountry = ECountry.eNewZealand; // Temporary - zatial nie je implementovana Australia
            sSnowInputData.eSnowRegion = (ESnowRegion) loadinput.SnowRegionIndex; // indexovane od 0, takze postacuje len previest na enum
            sSnowInputData.eExposureCategory = (ERoofExposureCategory) loadinput.ExposureCategoryIndex;
            sSnowInputData.fh_0_SiteElevation_meters = loadinput.SiteElevation;
            snow = new CCalcul_1170_3(sBuildingInputData, sGeometryInputData, sSnowInputData);
        }

        public void CalculateWindLoad()
        {
            sWindInputData.eWindRegion = loadinput.WindRegion;
            sWindInputData.iAngleWindDirection = loadinput.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = loadinput.TerrainCategoryIndex;

            wind = new CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData);
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
            return (float)(2 * MathF.fPI * Math.Sqrt((fMass_Total * fg_acceleration * MathF.Pow2(fDelta_x)) / (fg_acceleration * fP * fDelta_x))); // Eq. 4.1(1)
        }

        public void FillComboboxTrapezoidalSheetingThickness(string sTableName, ComboBox combobox)
        {
            CComboBoxHelper.FillComboboxValues("TrapezoidalSheetingSQLiteDB", sTableName, "name", combobox);
        }
        
        public void SetMaterialValuesFromDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command;
                SQLiteDataReader reader;

                foreach (CMat_03_00 mat in model.m_arrMat)
                {
                    int ID = 1;
                    string stringcommand = "Select * from materialSteelAS4600 where ID = '" + ID + "'";

                    command = new SQLiteCommand(stringcommand, conn);
                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mat.Standard = reader["Standard"].ToString();
                            mat.Name = /*mat.Grade =*/ reader["Grade"].ToString();
                            int intervals = reader.GetInt32(reader.GetOrdinal("iNumberOfIntervals"));
                            mat.Note = reader["note"].ToString();

                            if (intervals == 1)
                            {
                                mat.m_ft_interval = new float[intervals];
                                mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
                                mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                            }
                            else if (intervals == 2)
                            {
                                mat.m_ft_interval = new float[intervals];
                                mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                            }
                            else if (intervals == 3)
                            {
                                mat.m_ft_interval = new float[intervals];
                                mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                mat.m_ft_interval[2] = reader.GetFloat(reader.GetOrdinal("t3"));
                                mat.m_ff_yk[2] = reader.GetFloat(reader.GetOrdinal("f_y3")) * 1e+6f;
                                mat.m_ff_u[2] = reader.GetFloat(reader.GetOrdinal("f_u3")) * 1e+6f;
                            }
                            else if (intervals == 4)
                            {
                                mat.m_ft_interval = new float[intervals];
                                mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                mat.m_ft_interval[2] = reader.GetFloat(reader.GetOrdinal("t3"));
                                mat.m_ff_yk[2] = reader.GetFloat(reader.GetOrdinal("f_y3")) * 1e+6f;
                                mat.m_ff_u[2] = reader.GetFloat(reader.GetOrdinal("f_u3")) * 1e+6f;
                                mat.m_ft_interval[3] = reader.GetFloat(reader.GetOrdinal("t4"));
                                mat.m_ff_yk[3] = reader.GetFloat(reader.GetOrdinal("f_y4")) * 1e+6f;
                                mat.m_ff_u[3] = reader.GetFloat(reader.GetOrdinal("f_u4")) * 1e+6f;
                            }

                            model.m_arrMat[0] = mat;
                        }
                    }
                    reader.Close();
                }
            }
        }

        public void SetCrossSectionValuesFromDatabase()
        {
            foreach (CCrSc_TW crsc in model.m_arrCrSc)
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

                CSectionManager.LoadCrossSectionProperties_meters(crsc, crsc.NameDatabase);

                // Docasne hodnoty
                crsc.A_vy = crsc.h * crsc.t_min; // TODO - len priblizne Temp
                crsc.A_vz = crsc.b * crsc.t_min; // Temp
                crsc.W_y_pl = 1.1 * crsc.W_y_el;
                crsc.W_z_pl = 1.1 * crsc.W_z_el;

                // TODO - doplnit vypocet
                crsc.i_y_rg = 0.102f;
                crsc.i_z_rg = 0.052f;
                crsc.i_yz_rg = 0.102f;
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

            sDisplayOptions.bDisplayMemberDescription = chbDisplayMemberDescription.IsChecked == true;

            sDisplayOptions.bDisplayMembersCenterLines = chbDisplayMembersCenterLines.IsChecked == true;
            sDisplayOptions.bDisplaySolidModel = chbDisplaySolidModel.IsChecked == true;
            sDisplayOptions.bDisplayWireFrameModel = chbDisplayWireFrameModel.IsChecked == true;

            sDisplayOptions.bDistinguishedColor = chbDisplayDistinguishedColorMember.IsChecked == true;
            sDisplayOptions.bTransparentMemberModel = chbDisplayTransparentModelMember.IsChecked == true;

            sDisplayOptions.bDisplayGlobalAxis = chbDisplayGlobalAxis.IsChecked == true;

            sDisplayOptions.bDisplayLoads = chbDisplayLoads.IsChecked == true;
        }

        private void UpdateAll()
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;
            // Create Model
            // Kitset Steel Gable Enclosed Buildings

            // Set current geometry data to calculate loads
            sGeometryInputData.fH_2 = vm.fh2;
            sGeometryInputData.fH_1 = vm.WallHeight;
            sGeometryInputData.fW = vm.GableWidth;
            sGeometryInputData.fL = vm.Length;
            sGeometryInputData.fRoofPitch_deg = vm.RoofPitch_deg;

            CalculateLoadingValues();

            // TODO - nove parametre pre nastavenie hodnot zatazenia
            model = new CExample_3D_901_PF(
                vm.WallHeight,
                vm.GableWidth,
                vm.fL1,
                vm.Frames,
                vm.fh2,
                vm.GirtDistance,
                vm.PurlinDistance,
                vm.ColumnDistance,
                vm.BottomGirtPosition,
                vm.FrontFrameRakeAngle,
                vm.BackFrameRakeAngle,
                DoorBlocksToInsertProperties,
                WindowBlocksToInsertProperties,
                DoorBlocksProperties,
                WindowBlocksProperties,
                generalLoad,
                wind,
                snow,
                eq);

            // Create 3D window
            UpdateDisplayOptions();

            Page3Dmodel page1 = new Page3Dmodel(model, sDisplayOptions, model.m_arrLoadCases[Combobox_LoadCase.SelectedIndex]);

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
            Page3Dmodel page3D = (Page3Dmodel) Frame1.Content;
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
            Pokus2DView win = new Pokus2DView(model);
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

            if (MainTabControl.SelectedIndex == 1)
            {
                if(Model_Component.Content == null) Model_Component.Content = new UC_ComponentList();
            }                
            else if (MainTabControl.SelectedIndex == 2)
            {
                if(Loads.Content == null) Loads.Content = new UC_Loads(sGeometryInputData);
            }                
            else if (MainTabControl.SelectedIndex == 3)
                Load_Cases.Content = new UC_LoadCaseList(model).Content;
            else if (MainTabControl.SelectedIndex == 4)
                Load_Combinations.Content = new UC_LoadCombinationList(model).Content;
            else if (MainTabControl.SelectedIndex == 5)
            {
                if (Model_Component.Content == null) Model_Component.Content = new UC_ComponentList();                
                UC_ComponentList component = Model_Component.Content as UC_ComponentList;
                
                // TODO - napojit ako vstup type prvkov pre combobox - zobrazovanie vysledkov podla typu pruta
                Internal_Forces.Content = new UC_InternalForces(model, component).Content;
            }
            else if (MainTabControl.SelectedIndex == 6)
            {
                if (Model_Component.Content == null) Model_Component.Content = new UC_ComponentList();
                UC_ComponentList component = Model_Component.Content as UC_ComponentList;
                // TODO - napojit ako vstup type prvkov pre combobox - zobrazovanie vysledkov podla typu pruta
                Member_Design.Content = new UC_MemberDesign(model, component).Content;
            }
            else if (MainTabControl.SelectedIndex == 7)
                Joint_Design.Content = new UC_JointDesign().Content;
            else if (MainTabControl.SelectedIndex == 8)
                Part_List.Content = new UC_MaterialList(model).Content;
            else
            {
                // Not implemented like UC;
            };
        }

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

        private void chbDisplayMemberDescription_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void chbDisplayMemberDescription_Unchecked(object sender, RoutedEventArgs e)
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
                // TODO Ondrej - odstranit zavislost na solid model
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

        private void chbDisplayGlobalAxis_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Add(model.AxisX);
                ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Add(model.AxisY);
                ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.Add(model.AxisZ);


                //neviem refreshnut viewport
                //((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Focus();
                //((Page3Dmodel)Frame1.Content)._trackport.ViewPort.UpdateLayout();
                //((Page3Dmodel)Frame1.Content)._trackport.SetupScene();
                //Frame1.UpdateLayout();
                //UpdateAll();
            }
        }
        private void chbDisplayGlobalAxis_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                int index = 0;
                foreach (Visual3D visual in ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children)
                {
                    if (visual is ScreenSpaceLines3D)
                    {
                        if(((ScreenSpaceLines3D)visual).Name != null && ((ScreenSpaceLines3D)visual).Name.Equals("AxisX")) break;
                    }

                    index++;
                }
                ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.RemoveAt(index);
                ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.RemoveAt(index);
                ((Page3Dmodel)Frame1.Content)._trackport.ViewPort.Children.RemoveAt(index);
                
                //UpdateAll();
            }
        }

        private void chbDisplayLoads_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }
        private void chbDisplayLoads_UnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsInitialized)
            {
                UpdateAll();
            }
        }

        private void Combobox_LoadCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAll();
        }

        //private void WindSpeedChart_Click(object sender, RoutedEventArgs e)
        //{
        //    CalculateWindLoad();

        //    WindSpeedChart wind_chart = new WindSpeedChart(wind);
        //    wind_chart.Show();
        //}
    }
}
