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
using BaseClasses;
using MATH;
using CRSC;
using sw_en_GUI;
using sw_en_GUI.EXAMPLES._3D;
using M_AS4600;
using M_EC1.AS_NZS;
using System.Windows.Media.Media3D;

namespace PFD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection cn;
        SqlDataAdapter da;
        DataSet ds;
        public CModel model;
        CultureInfo ci;
        public DatabaseModels dmodels; // Todo nahradit databazov modelov
        public DatabaseLocations dlocations; // Todo nahradit databazov miest - pokial mozno skusit pripravit mapu ktora by bola schopna identifikovat polohu podla kliknutia

        int selected_Model_Index;
        float fb; // 3000 - 100000 mm
        float fL; // 3000 - 150000 mm
        float fh; // 2000 -  50000 mm (h1)
        float fL1;
        int iFrNo; // 2 - 30
        float fRoofPitch_radians; // (zadavane v stupnoch - limity stupne 3 - 50 deg)
        float fh2;
        float fdist_girt; // 500 - 5000 mm
        float fdist_purlin; // 500 - 5000 mm
        float fdist_frontcolumn; // 1000 - 10000 mm
        float fdist_girt_bottom; // 1000 - 10000 mm

        List<string> zoznamMenuNazvy = new List<string>(4);          // premenne zobrazene v tabulke
        List<string> zoznamMenuHodnoty = new List<string>(4);        // hodnoty danych premennych
        List<string> zoznamMenuJednotky = new List<string>(4);       // jednotky danych premennych

        public MainWindow()
        {
            ci = new CultureInfo("en-us");

            // Database Connection
            // TODO Pripojit DATABASE\bin\Debug\MDB.db
            //cn = new SqlConnection(@"data source"); // Data Source
            //cn.Open();
            dmodels = new DatabaseModels();
            dlocations = new DatabaseLocations();

            InitializeComponent();
            this.DataContext = new CPFDViewModel();

            foreach (string modelname in dmodels.arr_ModelNames)
              Combobox_Models.Items.Add(modelname);

            foreach (string locationname in dlocations.arr_LocationNames)
                Combobox_Location.Items.Add(locationname);

            // Set data from database in to the Window textboxes
            SetDataFromDatabasetoWindow();

            // Load data from window

            LoadDataFromWindow();

            // Create Model
            // Kitset Steel Gable Enclosed Buildings

            model = new CExample_3D_901_PF(fh, fb, fL1, iFrNo, fh2, fdist_girt, fdist_purlin, fdist_frontcolumn, fdist_girt_bottom);
            //model = new CExample_3D_902_OM();

            // Create 3D window
            Page3Dmodel page1 = new Page3Dmodel(model);

            // Display model in 3D preview frame
            Frame1.Content = page1;
        }

        private void SetDataFromDatabasetoWindow()
        {
            DatabaseModels dmodel = new DatabaseModels(Combobox_Models.SelectedIndex);

            TextBox_Gable_Width.Text = dmodel.fb_mm.ToString();
            TextBox_Length.Text = dmodel.fL_mm.ToString();
            TextBox_Wall_Height.Text = dmodel.fh_mm.ToString();
            TextBox_Frames_No.Text = dmodel.iFrNo.ToString();
            TextBox_Roof_Pitch.Text = dmodel.fRoof_Pitch_deg.ToString();
            TextBox_Girt_Distance.Text = dmodel.fdist_girt_mm.ToString();
            TextBox_Purlin_Distance.Text = dmodel.fdist_purlin_mm.ToString();
            TextBox_Column_Distance.Text = dmodel.fdist_frontcolumn_mm.ToString();
        }

        private void LoadDataFromWindow()
        {
            selected_Model_Index = Combobox_Models.SelectedIndex;

            fb = (float)Convert.ToDecimal(TextBox_Gable_Width.Text, ci) / 1000f; // From milimeters to meters
            fL = (float)Convert.ToDecimal(TextBox_Length.Text, ci) / 1000f;
            fh = (float)Convert.ToDecimal(TextBox_Wall_Height.Text, ci) / 1000f;
            iFrNo = (int)Convert.ToInt64(TextBox_Frames_No.Text, ci);
            fL1 = fL / (iFrNo - 1);
            fRoofPitch_radians = (float)Convert.ToDecimal(TextBox_Roof_Pitch.Text, ci) * MathF.fPI / 180f;
            fh2 = fh + 0.5f * fb * (float)Math.Tan(fRoofPitch_radians);

            fdist_girt = (float)Convert.ToDecimal(TextBox_Girt_Distance.Text, ci) / 1000f;
            fdist_purlin = (float)Convert.ToDecimal(TextBox_Purlin_Distance.Text, ci) / 1000f;
            fdist_frontcolumn = (float)Convert.ToDecimal(TextBox_Column_Distance.Text, ci) / 1000f;

            // Temporary
            fdist_girt_bottom = dmodels.fdist_girt_bottom_mm / 1000f;
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
            model = new CExample_3D_901_PF(fh, fb, fL1, iFrNo, fh2, fdist_girt, fdist_purlin, fdist_frontcolumn, fdist_girt_bottom); // create calculation model // TODO - set purlin distances

            // Clear results of previous calculation
            DeleteCalculationResults();

            // Load Generation
            // Self-weigth (1170.1)

            // Wind
            CCalcul_1170_2 wind = new CCalcul_1170_2();

            // Snow
            CCalcul_1170_3 snow = new CCalcul_1170_3();

            // Earthquake
            CCalcul_1170_5 eq = new CCalcul_1170_5();









            //Calculate Internal Forces
            // Todo - napojit FEM vypocet

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
            CCrSc_TW cso = new CSO();

            //cso = model.m_arrMembers[i].CrScStart;

            cso.A_g = 2100;
            cso.I_y = 11200;
            cso.I_z = 55406;
            cso.I_yz = 12;
            cso.I_t = 5887878;
            cso.I_w = 5277778;
            cso.A_vy = 6546;
            cso.A_vz = 65465;
            cso.W_y_el = 556;
            cso.W_z_el = 564;
            cso.W_y_pl = 742;
            cso.W_z_pl = 545;

            cso.h = 0.27f;
            cso.b = 0.09f;
            cso.t_min = 0.00115;
            cso.t_max = 0.00115;

            cso.m_Mat.m_ff_yk_0 = 5e+8f;
            cso.m_Mat.m_fE = 2.1e+8f;
            cso.m_Mat.m_fG = 8.1e+7f;
            cso.m_Mat.m_fNu = 0.297f;

            cso.i_y_rg = 0.102f;
            cso.i_z_rg = 0.052f;
            cso.i_yz_rg = 0.102f;

            cso.D_y_s = 0.043f;
            cso.D_z_s = 0.0f;

            CCalcul obj_calculate = new CCalcul(fN, fN_c, fN_t, fV_xu, fV_yv, fT, fM_xu, fM_yv, cso);
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

            // Set Column Header
            Results_GridView.Columns[0].Header = Results_GridView.Columns[3].Header = Results_GridView.Columns[6].Header = "Symbol";
            Results_GridView.Columns[1].Header = Results_GridView.Columns[4].Header = Results_GridView.Columns[7].Header = "Value";
            Results_GridView.Columns[2].Header = Results_GridView.Columns[5].Header = Results_GridView.Columns[8].Header = "Unit";

            // Set Column Width
            Results_GridView.Columns[0].Width = Results_GridView.Columns[3].Width = Results_GridView.Columns[6].Width = 117;
            Results_GridView.Columns[1].Width = Results_GridView.Columns[4].Width = Results_GridView.Columns[7].Width = 90;
            Results_GridView.Columns[2].Width = Results_GridView.Columns[5].Width = Results_GridView.Columns[8].Width = 90;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Material - pokus TODO
            da = new SqlDataAdapter("SELECT * FROM STEEL", cn); // SQL Query
            ds = new DataSet();
            da.Fill(ds);

            //Results_GridView.ItemsSource = ds.Tables[0].DefaultView;
        }

        //deleting lists for updating actual values
        private void DeleteLists()
        {
            zoznamMenuNazvy.Clear();
            zoznamMenuNazvy.Clear();
            zoznamMenuJednotky.Clear();
        }

        private void ComboBox_Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteCalculationResults();

            SetDataFromDatabasetoWindow();

            // Load data from window
            LoadDataFromWindow();

            // Create Model
            // Kitset Steel Gable Enclosed Buildings
            model = new CExample_3D_901_PF(fh, fb, fL1, iFrNo, fh2, fdist_girt, fdist_purlin,fdist_frontcolumn,fdist_girt_bottom);

            // Create 3D window
            Page3Dmodel page1 = new Page3Dmodel(model);

            // Display model in 3D preview frame
            Frame1.Content = page1;
            this.UpdateLayout();
        }

        private void UpdateAll()
        {
            // Create Model
            // Kitset Steel Gable Enclosed Buildings
            model = new CExample_3D_901_PF(fh, fb, fL1, iFrNo, fh2, fdist_girt, fdist_purlin,fdist_frontcolumn, fdist_girt_bottom);

            // Create 3D window
            Page3Dmodel page1 = new Page3Dmodel(model);

            // Display model in 3D preview frame
            Frame1.Content = page1;
            Frame1.UpdateLayout();
        }

        private void Results_GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_Gable_Width_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fb = (float)Convert.ToDecimal(TextBox_Gable_Width.Text, ci) / 1000f;
                    // Recalculate roof pitch
                    fRoofPitch_radians = (float)Math.Atan((fh2 - fh) / (0.5f * fb));
                    // Set new value in GUI
                    TextBox_Roof_Pitch.Text = (fRoofPitch_radians * 180f / MathF.fPI).ToString();

                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void TextBox_Length_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fL = (float)Convert.ToDecimal(TextBox_Length.Text, ci) / 1000f;
                    // Recalculate fL1
                    fL1 = fL / (iFrNo - 1);
                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void TextBox_Wall_Height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fh = (float)Convert.ToDecimal(TextBox_Wall_Height.Text, ci) / 1000f;
                    // Recalculate roof heigth
                    fh2 = fh + 0.5f * fb * (float)Math.Tan(fRoofPitch_radians);
                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void TextBox_Frames_No_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    iFrNo = (int)Convert.ToInt64(TextBox_Frames_No.Text, ci);
                    // Recalculate L1
                    fL1 = fL / (iFrNo - 1);
                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void TextBox_Roof_Pitch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fRoofPitch_radians = (float)Convert.ToDecimal(TextBox_Roof_Pitch.Text, ci) * MathF.fPI / 180f;
                    // Recalculate h2
                    fh2 = fh + 0.5f * fb * (float)Math.Tan(fRoofPitch_radians);
                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void ComboBox_Location_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_Girt_Distance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fdist_girt = (float)Convert.ToDecimal(TextBox_Girt_Distance.Text, ci) / 1000f;
                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void TextBox_Purlin_Distance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fdist_purlin = (float)Convert.ToDecimal(TextBox_Purlin_Distance.Text, ci) / 1000f;
                    DeleteCalculationResults();
                    UpdateAll();
                }
                catch
                { }
            }
        }

        private void TextBox_Column_Distance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selected_Model_Index != 0)
            {
                try
                {
                    fdist_frontcolumn = (float)Convert.ToDecimal(TextBox_Column_Distance.Text, ci) / 1000f;
                    DeleteCalculationResults();

                    // Re-calculate value of distance between columns (number of columns per frame is always even
                    int iOneRafterFrontColumnNo = (int)((0.5f * fb) / fdist_frontcolumn);
                    int iFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                    // Update value of distance between columns
                    fdist_frontcolumn = (fb / (iFrontColumnNoInOneFrame + 1));
                    //Calculate value in displayed units
                    float fdist_frontcolumn_mm = 1000f * fdist_frontcolumn;
                    // Set value in textbox
                    TextBox_Column_Distance.Text = fdist_frontcolumn_mm.ToString();

                    UpdateAll();
                }
                catch
                { }
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

        //float fb; // 3000 - 100000 mm
        //float fL; // 3000 - 150000 mm
        //float fh; // 2000 -  50000 mm (h1)
        //float fL1;
        //int iFrNo; // 2 - 30
        //float fRoofPitch_radians; // (zadavane v stupnoch - limity stupne 3 - 50 deg)
        //float fh2;
        //float fdist_girt; // 500 - 5000 mm
        //float fdist_purlin; // 500 - 5000 mm
        //float fdist_frontcolumn; // 1000 - 10000 mm
        //float fdist_girt_bottom; // 1000 - 10000 mm

        private void TextBox_Gable_Width_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValidGableWidth(((TextBox)sender).Text + e.Text);
        }
        private static bool IsValidGableWidth(string str)
        {
            int i;
            return int.TryParse(str, out i) && i >= 3000 && i <= 100000;
        }

        private void TextBox_Length_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValidLength(((TextBox)sender).Text + e.Text);
        }
        private static bool IsValidLength(string str)
        {
            int i;
            return int.TryParse(str, out i) && i >= 3000 && i <= 150000;
        }

        private void TextBox_Rangetextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool valid = IsValidMyRange(((TextBox)sender).Text);
            e.Handled = !valid;
        }
        private static bool IsValidMyRange(string str)
        {
            int i;
            return int.TryParse(str, out i) && i >= 5 && i <= 100;
        }

        private void SystemComponentViewer_Click(object sender, RoutedEventArgs e)
        {
            SystemComponentViewer win = new SystemComponentViewer();
            win.Show();
        }
    }
}
