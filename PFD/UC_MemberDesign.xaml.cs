using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BaseClasses;
using M_AS4600;
using CRSC;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_MemberDesign.xaml
    /// </summary>
    public partial class UC_MemberDesign : UserControl
    {
        DataSet ds;
        List<string> zoznamMenuNazvy = new List<string>(4);          // premenne zobrazene v tabulke
        List<string> zoznamMenuHodnoty = new List<string>(4);        // hodnoty danych premennych
        List<string> zoznamMenuJednotky = new List<string>(4);       // jednotky danych premennych

        public UC_MemberDesign() { } // TODO - Refaktorovat, tento konstruktor je pouzity v projekte SBD

        public UC_MemberDesign(CModel model, UC_ComponentList components, List<CMemberLoadCombinationRatio> DesignResults)
        {
            InitializeComponent();

            // Add items into comboboxes
            FillComboboxValues(Combobox_LimitState, model.m_arrLimitStates);
            FillComboboxValues(Combobox_LoadCombination, model.m_arrLoadCombs);
            // TODO Ondrej - fill combobox with UC_ComponentList Names
            //FillComboboxValues(Combobox_ComponentType, components);

            // Set default values of combobox index
            Combobox_LimitState.SelectedIndex = 0;
            Combobox_LoadCombination.SelectedIndex = 0;

            // TODO - Ondrej zobrazit vysledky pre dany vyber v comboboxe, UC_MemberDesign a tabItem Member Design sa zobrazuje len ak su vysledky k dispozicii
            // DisplayDesignResultsInGridView(CCalcul obj_CalcDesign);

            //CMemberLoadCombinationRatio mlcr = DesignResults.Find(i => i.LoadCombination.ID == Combobox_LoadCombination.SelectedValue)

            CCalcul c = new CCalcul(false, DesignResults[0].DesignInternalForces, (CCrSc_TW)DesignResults[0].Member.CrScStart, DesignResults[0].Member.FLength, DesignResults[0].DesignMomentValuesForCb);
            DisplayDesignResultsInGridView(Results_GridView, c);
        }

        // TODO - Ondrej - zjednotit s UC_InternalForces
        public void FillComboboxValues(ComboBox combobox, CObject[] array)
        {
            if (array != null)
            {
                foreach (CObject obj in array)
                {
                    if (obj.Name != null || obj.Name != "")
                        combobox.Items.Add(obj.Name);
                    else
                    {
                        // Exception
                        MessageBox.Show("Object ID = " + obj.ID + "." + " Object name is not defined correctly.");
                    }
                }
            }
        }

        // TODO - Display Data in DataGrid Results_GridView

        public void DisplayDesignResultsInGridView(DataGrid dataGrid, CCalcul obj_CalcDesign)
        {
            DeleteLists();
            // Display results in datagrid
            // AS 4600 output variables

            // Tension
            zoznamMenuNazvy.Add("φ t");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fPhi_t.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("N t,min");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_t_min.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("η Nt");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_Nt.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Compression
            // Global Buckling
            zoznamMenuNazvy.Add("f oc");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_oc.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ c");
            zoznamMenuHodnoty.Add(obj_CalcDesign.flambda_c.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("f oz");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_oz.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("f ox");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_ox.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("f oy");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_oy.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("N y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_y.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N oc");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_oc.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N ce");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_ce.ToString());
            zoznamMenuJednotky.Add("[N]");

            // Local Buckling
            zoznamMenuNazvy.Add("f ol");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_oy.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ l");
            zoznamMenuHodnoty.Add(obj_CalcDesign.flambda_l.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("N ol");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_ol.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N cl");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_cl.ToString());
            zoznamMenuJednotky.Add("[N]");

            // Distorsial Buckling
            zoznamMenuNazvy.Add("f od");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_od.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("λ d");
            zoznamMenuHodnoty.Add(obj_CalcDesign.flambda_d.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("N od");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_od.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N cd");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_cd.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("N c,min");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fN_c_min.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("φ c");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fPhi_c.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η Nc");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_721_N.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Bending
            zoznamMenuNazvy.Add("M p,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_p_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_y_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M p,y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_p_yv.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_y_yv.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Bending about x/u axis
            zoznamMenuNazvy.Add("C b");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fC_b.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("M o,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_o_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M be,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_be_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Local Buckling
            zoznamMenuNazvy.Add("f ol,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_ol_bend.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("M ol,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_ol_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("λ l,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fLambda_l_xu.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("M bl,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_bl_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Distrosial buckling
            zoznamMenuNazvy.Add("f od,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.ff_od_bend.ToString());
            zoznamMenuJednotky.Add("[Pa]");

            zoznamMenuNazvy.Add("M od,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_od_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("λ d,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fLambda_d_xu.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("M bd,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_bd_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("φ b");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fPhi_b.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Shear
            zoznamMenuNazvy.Add("V y,y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fV_y_yv.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("V v,y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fV_v_yv.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("V cr,y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fV_cr_yv.ToString());
            zoznamMenuJednotky.Add("[N]");

            zoznamMenuNazvy.Add("λ v,y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fLambda_v_yv.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("φ v");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fPhi_v.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_723_9_xu_yv.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_723_10_xu.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_723_11_xu_yv.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_723_12_xu_yv_10.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Intercation of internal forces
            zoznamMenuNazvy.Add("M s,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_s_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M b,x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_b_xu.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M s,x,f");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_s_xu_f.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M s,y,f");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fM_s_yv_f.ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_724.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_725_1.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("η");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_725_2.ToString());
            zoznamMenuJednotky.Add("[-]");

            // Maximum design ratio
            zoznamMenuNazvy.Add("η max");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_max.ToString());
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

            dataGrid.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

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

        private void DeleteLists()
        {
            // Deleting lists for updating actual values
            zoznamMenuNazvy.Clear();
            zoznamMenuHodnoty.Clear();
            zoznamMenuJednotky.Clear();
        }
    }
}
