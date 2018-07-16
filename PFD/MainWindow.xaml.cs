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
using BaseClasses;
using MATH;
using MATERIAL;
using CRSC;
using sw_en_GUI;
using sw_en_GUI.EXAMPLES._2D;
using sw_en_GUI.EXAMPLES._3D;
using M_AS4600;
using M_EC1.AS_NZS;
using _3DTools;
using SharedLibraries.EXPIMP;
using System.Data.SQLite;
using System.Configuration;

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

        DataSet ds;

        public CModel model;
        public DatabaseModels dmodels; // Todo nahradit databazov modelov
        public DatabaseLocations dlocations; // Todo nahradit databazov miest - pokial mozno skusit pripravit mapu ktora by bola schopna identifikovat polohu podla kliknutia
        public CPFDViewModel vm;
        public CPFDLoadInput loadinput;

        public BuildingDataInput sBuildingInputData;
        public WindLoadDataInput sWindInputData;
        public SeisLoadDataInput sSeisInputData;

        //int selected_Model_Index;
        //float fb; // 3 - 100 m
        //float fL; // 3 - 150 m
        //float fh; // 2 -  50 m (h1)
        //float fL1;
        //int iFrNo; // 2 - 50
        //float fRoofPitch_radians; // (zadavane v stupnoch - limity stupne 3 - 50 deg)
        //float fh2;
        //float fdist_girt; // 0.5 - 5 m
        //float fdist_purlin; // 0.5 - 5 m
        //float fdist_frontcolumn; // 1 - 10 m
        //float fdist_girt_bottom; // 1 - 10 m

        List<string> zoznamMenuNazvy = new List<string>(4);          // premenne zobrazene v tabulke
        List<string> zoznamMenuHodnoty = new List<string>(4);        // hodnoty danych premennych
        List<string> zoznamMenuJednotky = new List<string>(4);       // jednotky danych premennych

        public MainWindow()
        {
            // Database Connection
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("Select * from corrosion_category", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        System.Diagnostics.Trace.Write(reader["ID"] + ", ");
                        System.Diagnostics.Trace.Write(reader["standard"] + ", ");
                        System.Diagnostics.Trace.Write(reader["corrosion_category"] + ", ");
                        System.Diagnostics.Trace.Write(reader["category_name"] + ", ");
                        System.Diagnostics.Trace.WriteLine(reader["description"] + ", ");
                    }
                    
                    reader.Close();
                }
            }




            dmodels = new DatabaseModels();
            dlocations = new DatabaseLocations();

            // Initial Screen
            SplashScreen splashScreen = new SplashScreen("formsteel-screen.jpg");
            splashScreen.Show(false);
            InitializeComponent();
            splashScreen.Close(TimeSpan.FromMilliseconds(1000));

            // Fill combobox items
            foreach (string modelname in dmodels.arr_ModelNames)
              Combobox_Models.Items.Add(modelname);

            foreach (string locationname in dlocations.arr_LocationNames)
                Combobox_Location.Items.Add(locationname);

            // Cladding
            Combobox_RoofCladding.Items.Add("SmartDek");
            Combobox_RoofCladding.Items.Add("PurlinDek");

            Combobox_WallCladding.Items.Add("SmartDek");
            Combobox_WallCladding.Items.Add("PurlinDek");

            // Colors
            // TODO - pridat do comboboxu len vybrane farby

            /*
            Desert Sand
            Bone White
            Titania
            Smooth Cream
            New Denim Blue
            Grey Friars
            Sandstone Grey
            Gull Grey
            Permanent Green
            Scoria
            Pioneer Red
            Mist Green
            Rivergum
            Lichen
            */

            Combobox_RoofCladdingColor.ItemsSource = typeof(Colors).GetProperties();
            Combobox_WallCladdingColor.ItemsSource = typeof(Colors).GetProperties();

            // Model Geometry
            vm = new CPFDViewModel(1);
            vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            this.DataContext = vm;
            
            // Create Model
            // Kitset Steel Gable Enclosed Buildings
            model = new CExample_3D_901_PF(vm.WallHeight, vm.GableWidth, vm.fL1, vm.Frames, vm.fh2, vm.GirtDistance, vm.PurlinDistance, vm.ColumnDistance, vm.BottomGirtPosition, vm.FrontFrameRakeAngle, vm.BackFrameRakeAngle);

            // Loading
            loadinput = new CPFDLoadInput(11); // Default - Auckland
            loadinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            //this.DataContext = loadinput; // Toto prepise aj predchadzajuce nastavenie z vm, musi to byt inak Todo Ondrej

            // Create 3D window
            Page3Dmodel page1 = new Page3Dmodel(model);

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

        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDLoadInput loadInput = sender as CPFDLoadInput;
            if (loadInput != null && loadInput.IsSetFromCode) return;
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
            //Todo - asi sa to da jednoduchsie
            DeleteLists();
            Results_GridView.ItemsSource = null;
            Results_GridView.Items.Clear();
            Results_GridView.Items.Refresh();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;
            model = new CExample_3D_901_PF(vm.WallHeight, vm.GableWidth, vm.fL1, vm.Frames, vm.fh2, vm.GirtDistance, vm.PurlinDistance, vm.ColumnDistance, vm.BottomGirtPosition, vm.FrontFrameRakeAngle, vm.BackFrameRakeAngle);

            // Clear results of previous calculation
            DeleteCalculationResults();

            // Load Generation
            // Self-weigth (1170.1)

            //Temporary
            sBuildingInputData.location = ELocation.eAuckland;
            sBuildingInputData.iDesignLife = 50; // years
            sBuildingInputData.iImportanceClass = 3; // Importance Level
            sBuildingInputData.fAnnualProbability_R_ULS = 1f / 500f; // Annual Probability of Exceedence R_ULS
            sBuildingInputData.fAnnualProbability_R_SLS = 1f / 25f; // Annual Probability of Exceedence R_SLS

            // General loading
            CCalcul_1170_1 generalLoad = new CCalcul_1170_1();
            float fLiveLoad_Roof = 250f; // N/m2
            float fSuperImposed_DeadLoad = 450f;

            // Wind
            //1.wind region
            //2.terrain roughness
            //3.topography
            //4.pressure coefficients
            sWindInputData.fh = vm.fh2;
            CCalcul_1170_2 wind = new CCalcul_1170_2(sBuildingInputData, sWindInputData);

            // Snow
            CCalcul_1170_3 snow = new CCalcul_1170_3();

            // Earthquake / Seismic Design
            sSeisInputData.sSiteSubsoilClass = "D";
            sSeisInputData.fProximityToFault = 20000; // m
            sSeisInputData.fZoneFactor_Z = 0.13f;
            sSeisInputData.fPeriodAlongXDirection_Tx = 0.4f; //sec
            sSeisInputData.fPeriodAlongYDirection_Ty = 0.4f; //sec
            sSeisInputData.fSpectralShapeFactor_Ch_T = 3.0f;

            CCalcul_1170_5 eq = new CCalcul_1170_5(vm.GableWidth, vm.fL1, vm.WallHeight, sBuildingInputData, sSeisInputData);

            // TODO  - toto je potrebne presunut niekam k prierezom
            // Nastavit hodnoty pre vypocet

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
                            else if(intervals == 2)
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

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command;
                SQLiteDataReader reader;

                foreach (CCrSc_TW crsc in model.m_arrCrSc)
                {
                    // TODO - zjednotit nazvy prierezov v database a v GUI programu
                    string stringcommand = "Select * from tableSections_m where section = '" + crsc.Name + "'";
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

                    command = new SQLiteCommand("Select * from tableSections_m where section = '27095'", conn); // Temp
                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            crsc.Name = reader["section"].ToString();
                            crsc.h = reader.GetDouble(reader.GetOrdinal("h"));
                            crsc.b = reader.GetDouble(reader.GetOrdinal("b"));
                            crsc.t_min = crsc.t_max = reader.GetDouble(reader.GetOrdinal("t")); // Max and min value is just for thin-walled cold formed cross-sections
                            crsc.A_g = reader.GetDouble(reader.GetOrdinal("A_g"));
                            crsc.I_y0 = reader.GetDouble(reader.GetOrdinal("I_y0"));
                            crsc.I_z0 = reader.GetDouble(reader.GetOrdinal("I_z0"));
                            //crsc.W_y_el0 = reader.GetDouble(reader.GetOrdinal("W_el_y0"));
                            //crsc.W_z_el0 = reader.GetDouble(reader.GetOrdinal("W_el_z0"));
                            crsc.I_yz0 = reader.GetDouble(reader.GetOrdinal("Iyz0"));
                            crsc.I_y = reader.GetDouble(reader.GetOrdinal("Iy"));
                            crsc.I_z = reader.GetDouble(reader.GetOrdinal("Iz"));
                            crsc.W_y_el = reader.GetDouble(reader.GetOrdinal("W_el_y"));
                            crsc.W_z_el = reader.GetDouble(reader.GetOrdinal("W_el_z"));
                            crsc.I_t = reader.GetDouble(reader.GetOrdinal("It"));
                            crsc.I_w = reader.GetDouble(reader.GetOrdinal("Iw"));
                            crsc.D_y_gc = reader.GetDouble(reader.GetOrdinal("yc")); // Poloha taziska v povodnom suradnicovom systeme
                            crsc.D_z_gc = reader.GetDouble(reader.GetOrdinal("zc"));
                            crsc.D_y_sc = reader.GetDouble(reader.GetOrdinal("ys")); // Poloha stredu smyku v povodnom suradnicovom systeme
                            crsc.D_z_sc = reader.GetDouble(reader.GetOrdinal("zs"));
                            crsc.D_y_s = reader.GetDouble(reader.GetOrdinal("ycs"));  // Vzdialenost medzi taziskom G a stredom smyku S
                            crsc.D_z_s = reader.GetDouble(reader.GetOrdinal("zcs"));
                            crsc.Beta_y = reader.GetDouble(reader.GetOrdinal("betay"));
                            crsc.Beta_z = reader.GetDouble(reader.GetOrdinal("betaz"));
                            crsc.Alpha_rad = (reader.GetDouble(reader.GetOrdinal("alpha_deg")) / 180 * MathF.dPI);
                            crsc.Bending_curve_stress_x1 = reader.GetDouble(reader.GetOrdinal("Bending_curve_x1"));
                            crsc.Bending_curve_stress_x2 = reader.GetDouble(reader.GetOrdinal("Bending_curve_x2"));
                            //crsc.Bending_curve_stress_x3 = reader.GetDouble(reader.GetOrdinal("Bending_curve_x3")); // TODO osetrit nacitanie hodnoty ak je bunka v databaze prazdna
                            //crsc.Bending_curve_stress_y = reader.GetDouble(reader.GetOrdinal("Bending_curve_y")); // TODO osetrit nacitanie hodnoty ak je bunka v databaze prazdna
                            crsc.Compression_curve_stress_1 = reader.GetDouble(reader.GetOrdinal("Compression_curve_1"));
                            //crsc.Compression_curve_stress_2 = reader.GetDouble(reader.GetOrdinal("Compression_curve_2")); // TODO osetrit nacitanie hodnoty ak je bunka v databaze prazdna
                            //crsc.Compression_curve_stress_3 = reader.GetDouble(reader.GetOrdinal("Compression_curve_3")); // TODO osetrit nacitanie hodnoty ak je bunka v databaze prazdna

                            crsc.A_vy = crsc.h * crsc.t_min; // TODO - len priblizne Temp
                            crsc.A_vz = crsc.b * crsc.t_min; // Temp
                            crsc.W_y_pl = 1.1 * crsc.W_y_el;
                            crsc.W_z_pl = 1.1 * crsc.W_z_el;
                            crsc.i_y_rg = 0.102f;
                            crsc.i_z_rg = 0.052f;
                            crsc.i_yz_rg = 0.102f;

                            /*
                            crsc.m_Mat.m_ff_yk_0 = 5e+8f;
                            crsc.m_Mat.m_fE = 2.1e+8f;
                            crsc.m_Mat.m_fG = 8.1e+7f;
                            crsc.m_Mat.m_fNu = 0.297f;
                            */
                        }
                    }
                    reader.Close();
                }
            }

            // Calculate Internal Forces
            // Todo - napojit FEM vypocet
            bool bDebugging = false;

            // Todo - nefunguje to, asi je chybna detekcia zakladnej tuhostnej matice pruta
            // Treba sa na to pozriet podrobnejsie

            // Navrhujem napojit nejaky externy solver

            CExample_2D_13_PF temp2Dmodel = new CExample_2D_13_PF(model.m_arrMat[0], model.m_arrCrSc[0], model.m_arrCrSc[1], vm.GableWidth, vm.WallHeight, vm.fh2,1000,1000,1000,1000);
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



            float fN = -5000f;
            float fN_c = fN > 0 ? 0f : Math.Abs(fN);
            float fN_t = fN < 0 ? 0f : fN;
            float fV_xu = 4100;
            float fV_yv = 564556;
            float fV_xx = 0;
            float fV_yy = 0;
            float fT = 0;
            float fM_xu = 54548;
            float fM_yv = 5454;
            float fM_xx = 0;
            float fM_yy = 0;

            // Design Members
            //for (int i = 0; i < model.m_arrMembers.Length; i++)
            //{
            // skontrolovat co sa pocita, ostatne nastavit





            CCalcul obj_calculate = new CCalcul(fN, fN_c, fN_t, fV_xu, fV_yv, fT, fM_xu, fM_yv, (CCrSc_TW) model.m_arrCrSc[0]);
            //}

            // Display results in datagrid
            // AS 4600 output variables

            // Compression
            // Global Buckling
            zoznamMenuNazvy.Add("f oc");
            zoznamMenuHodnoty.Add(obj_calculate.ff_oc.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ c");
            zoznamMenuHodnoty.Add(obj_calculate.flambda_c.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("f oz");
            zoznamMenuHodnoty.Add(obj_calculate.ff_oz.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("f ox");
            zoznamMenuHodnoty.Add(obj_calculate.ff_ox.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("f oy");
            zoznamMenuHodnoty.Add(obj_calculate.ff_oy.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("N y");
            zoznamMenuHodnoty.Add(obj_calculate.fN_y.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N oc");
            zoznamMenuHodnoty.Add(obj_calculate.fN_oc.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N ce");
            zoznamMenuHodnoty.Add(obj_calculate.fN_ce.ToString());
            zoznamMenuJednotky.Add("[N]");

            // Local Buckling
            zoznamMenuNazvy.Add("f ol");
            zoznamMenuHodnoty.Add(obj_calculate.ff_oy.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ l");
            zoznamMenuHodnoty.Add(obj_calculate.flambda_l.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("N ol");
            zoznamMenuHodnoty.Add(obj_calculate.fN_ol.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N cl");
            zoznamMenuHodnoty.Add(obj_calculate.fN_cl.ToString());
            zoznamMenuJednotky.Add("[N]");

            // Distorsial Buckling
            zoznamMenuNazvy.Add("f od");
            zoznamMenuHodnoty.Add(obj_calculate.ff_od.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ d");
            zoznamMenuHodnoty.Add(obj_calculate.flambda_d.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("N od");
            zoznamMenuHodnoty.Add(obj_calculate.fN_od.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N cd");
            zoznamMenuHodnoty.Add(obj_calculate.fN_cd.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N c,min");
            zoznamMenuHodnoty.Add(obj_calculate.fN_c_min.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("φ c");
            zoznamMenuHodnoty.Add(obj_calculate.fPhi_c.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("Eta max");
            zoznamMenuHodnoty.Add(obj_calculate.fDesignRatio_N.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Tension
            zoznamMenuNazvy.Add("φ t");
            zoznamMenuHodnoty.Add(obj_calculate.fPhi_t.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Bending
            zoznamMenuNazvy.Add("M p,x");
            zoznamMenuHodnoty.Add(obj_calculate.fM_p_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,x");
            zoznamMenuHodnoty.Add(obj_calculate.fM_y_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M p,y");
            zoznamMenuHodnoty.Add(obj_calculate.fM_p_yv.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,y");
            zoznamMenuHodnoty.Add(obj_calculate.fM_y_yv.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("C b");
            zoznamMenuHodnoty.Add(obj_calculate.fC_b.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("M be,x");
            zoznamMenuHodnoty.Add(obj_calculate.fM_be_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Local Buckling
            zoznamMenuNazvy.Add("f ol,x");
            zoznamMenuHodnoty.Add(obj_calculate.ff_ol_bend.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ l,x");
            zoznamMenuHodnoty.Add(obj_calculate.fLambda_l_xu.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("M bl,x");
            zoznamMenuHodnoty.Add(obj_calculate.fM_bl_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Distrosial buckling
            zoznamMenuNazvy.Add("f od,x");
            zoznamMenuHodnoty.Add(obj_calculate.ff_od_bend.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ d,x");
            zoznamMenuHodnoty.Add(obj_calculate.fLambda_d_xu.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("M bd,x");
            zoznamMenuHodnoty.Add(obj_calculate.fM_bd_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("φ b");
            zoznamMenuHodnoty.Add(obj_calculate.fPhi_b.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Shear
            zoznamMenuNazvy.Add("φ v");
            zoznamMenuHodnoty.Add(obj_calculate.fPhi_v.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("Eta max");
            zoznamMenuHodnoty.Add(obj_calculate.fEta_max.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Create Table
            DataTable table = new DataTable("Table");
            // Create Table Rows

            table.Columns.Add("Symbol", typeof(String));
            table.Columns.Add("Value", typeof(String));
            table.Columns.Add("Unit", typeof(String));

            table.Columns.Add("Symbol1", typeof(String));
            table.Columns.Add("Value1", typeof(String));
            table.Columns.Add("Unit1", typeof(String));

            table.Columns.Add("Symbol2", typeof(String));
            table.Columns.Add("Value2", typeof(String));
            table.Columns.Add("Unit2", typeof(String));

            // Set Column Caption
            table.Columns["Symbol1"].Caption = table.Columns["Symbol2"].Caption = "Symbol";
            table.Columns["Value1"].Caption = table.Columns["Value2"].Caption = "Value";
            table.Columns["Unit1"].Caption = table.Columns["Unit2"].Caption = "Unit";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < zoznamMenuNazvy.Count; i++)
            {
                DataRow row = table.NewRow();

                try
                {
                    row["Symbol"] = zoznamMenuNazvy[i];
                    row["Value"] = zoznamMenuHodnoty[i];
                    row["Unit"] = zoznamMenuJednotky[i];
                    i++;
                    row["Symbol1"] = zoznamMenuNazvy[i];
                    row["Value1"] = zoznamMenuHodnoty[i];
                    row["Unit1"] = zoznamMenuJednotky[i];
                    i++;
                    row["Symbol2"] = zoznamMenuNazvy[i];
                    row["Value2"] = zoznamMenuHodnoty[i];
                    row["Unit2"] = zoznamMenuJednotky[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Results_GridView.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            /*
            // Set Column Header
            Results_GridView.Columns[0].Header = Results_GridView.Columns[3].Header = Results_GridView.Columns[6].Header = "Symbol";
            Results_GridView.Columns[1].Header = Results_GridView.Columns[4].Header = Results_GridView.Columns[7].Header = "Value";
            Results_GridView.Columns[2].Header = Results_GridView.Columns[5].Header = Results_GridView.Columns[8].Header = "Unit";

            // Set Column Width
            Results_GridView.Columns[0].Width = Results_GridView.Columns[3].Width = Results_GridView.Columns[6].Width = 117;
            Results_GridView.Columns[1].Width = Results_GridView.Columns[4].Width = Results_GridView.Columns[7].Width = 90;
            Results_GridView.Columns[2].Width = Results_GridView.Columns[5].Width = Results_GridView.Columns[8].Width = 90;
            */
                }

                //deleting lists for updating actual values
                private void DeleteLists()
        {
            zoznamMenuNazvy.Clear();
            zoznamMenuHodnoty.Clear();
            zoznamMenuJednotky.Clear();
        }
        
        private void UpdateAll()
        {
            CPFDViewModel vm = this.DataContext as CPFDViewModel;
            // Create Model
            // Kitset Steel Gable Enclosed Buildings
            model = new CExample_3D_901_PF(vm.WallHeight, vm.GableWidth, vm.fL1, vm.Frames, vm.fh2, vm.GirtDistance, vm.PurlinDistance, vm.ColumnDistance, vm.BottomGirtPosition, vm.FrontFrameRakeAngle, vm.BackFrameRakeAngle);

            // Create 3D window
            Page3Dmodel page1 = new Page3Dmodel(model);

            // Display model in 3D preview frame
            Frame1.Content = page1;
            Frame1.UpdateLayout();
        }

        private void Results_GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void MaterialList_Click(object sender, RoutedEventArgs e)
        {
            MaterialList win = new MaterialList(model);
            win.Show();
        }

        private void ExportDXF_3D_Click(object sender, RoutedEventArgs e)
        {
            Page3Dmodel model3D = Frame1.Content as Page3Dmodel;
            CExportToDXF.ExportViewPort_DXF_Test(model3D._trackport.ViewPort);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedIndex == 1)
                Model_Component.Content = new UC_ComponentList().Content;
            else if (MainTabControl.SelectedIndex == 3)
                Load_Cases.Content = new UC_LoadCaseList(model).Content;
            else if (MainTabControl.SelectedIndex == 4)
                Load_Combinations.Content = new UC_LoadCombinationList(model).Content;
            else if (MainTabControl.SelectedIndex == 8)
                Part_List.Content = new UC_MaterialList(model).Content;
            else
            { 
                // Not implemented like UC;
            };
        }
    }
}
