using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
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

        CModel_PFD Model;
        List<CMemberLoadCombinationRatio_ULS> DesignResults_ULS;
        List<CMemberLoadCombinationRatio_SLS> DesignResults_SLS;

        public UC_MemberDesign() { } // TODO - Refaktorovat, tento konstruktor je pouzity v projekte SBD

        public UC_MemberDesign(CModel_PFD model, CComponentListVM compList, List<CMemberLoadCombinationRatio_ULS> designResults_ULS, List<CMemberLoadCombinationRatio_SLS> designResults_SLS)
        {
            InitializeComponent();

            Model = model;
            DesignResults_ULS = designResults_ULS;
            DesignResults_SLS = designResults_SLS;

            // TODO - Ondrej zobrazit vysledky pre dany vyber v comboboxe, UC_MemberDesign a 
            //tabItem Member Design sa zobrazuje len ak su vysledky k dispozicii

            
            // Member Design
            CPFDMemberDesign mdinput = new CPFDMemberDesign(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            mdinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = mdinput;
        }
        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDMemberDesign mdinput = sender as CPFDMemberDesign;
            if (mdinput != null && mdinput.IsSetFromCode) return;

            CMemberGroup GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups[mdinput.ComponentTypeIndex];

            // Calculate governing member design ratio in member group
            CCalcul cGoverningMemberResults;

            if (Model.m_arrLoadCombs[mdinput.LoadCombinationIndex].eLComType == ELSType.eLS_ULS)
                CalculateGoverningMemberDesignDetails(DesignResults_ULS, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
            else
                CalculateGoverningMemberDesignDetails(DesignResults_SLS, GroupOfMembersWithSelectedType, out cGoverningMemberResults);

        }
        
        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(List<CMemberLoadCombinationRatio_ULS> DesignResults, CMemberGroup GroupOfMembersWithSelectedType, out CCalcul cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {


                    // TODO - Ondrej, vieme member ale potrebujeme sa dostat v zozname DesignResults na riadok ktory odpoveda uvedenemu member
                    // hodnota ID - 1 je nespolahlive pretoze pocet zaznamov v DesignResults nemusi byt rovnaky ako pocet prutov v modeli, nemusia sa pocitat vsetky

                    //tu sa vyberie zo zoznamu taky prvok ktory ma zhodne Member.ID
                    CMemberLoadCombinationRatio_ULS res = DesignResults.Find(i => i.Member.ID == m.ID);
                    if (res == null) continue;
                    CCalcul c = new CCalcul(false, res.DesignInternalForces, m, res.DesignMomentValuesForCb);

                    if (c.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = c.fEta_max;
                        cGoverningMemberResults = c;
                    }
                }

                if (cGoverningMemberResults != null)
                    DisplayDesignResultsInGridView(ELSType.eLS_ULS, Results_GridView, cGoverningMemberResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }

        }

        public void CalculateGoverningMemberDesignDetails(List<CMemberLoadCombinationRatio_SLS> DesignResults, CMemberGroup GroupOfMembersWithSelectedType, out CCalcul cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;

                //Mato mam otazku: Preco tu neprejdeme zoznam DesignResults a vypocet spravime pre vsetky Members z DesignResults, ktore maju dany typ vybraty v kombe ComponentType???

                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    // TODO - Ondrej, vieme member ale potrebujeme sa dostat v zozname DesignResults na riadok ktory odpoveda uvedenemu member
                    // hodnota ID - 1 je nespolahlive pretoze pocet zaznamov v DesignResults nemusi byt rovnaky ako pocet prutov v modeli, nemusia sa pocitat vsetky

                    //tu sa vyberie zo zoznamu taky prvok ktory ma zhodne Member.ID
                    CMemberLoadCombinationRatio_SLS res = DesignResults.Find(i => i.Member.ID == m.ID);
                    if (res == null) continue;
                    CCalcul c = new CCalcul(false, res.DesignDeflections, m);

                    if (c.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = c.fEta_max;
                        cGoverningMemberResults = c;
                    }
                }

                if (cGoverningMemberResults != null)
                    DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView, cGoverningMemberResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }
        }

        // TODO - Display Data in DataGrid Results_GridView

        public void DisplayDesignResultsInGridView(ELSType eCombinationType, DataGrid dataGrid, CCalcul obj_CalcDesign)
        {
            DeleteLists();

            if (eCombinationType == ELSType.eLS_ULS)
                SetResultsDetailsFor_ULS(obj_CalcDesign);
            else
                SetResultsDetailsFor_SLS(obj_CalcDesign);

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

        private void SetResultsDetailsFor_ULS(CCalcul obj_CalcDesign)
        {
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
        }

        private void SetResultsDetailsFor_SLS(CCalcul obj_CalcDesign)
        {
            // Display results in datagrid

            // Deflection
            // δ

            zoznamMenuNazvy.Add("δ lim");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fLimitDeflection.ToString());
            zoznamMenuJednotky.Add("[mm]");

            // Design ratio
            zoznamMenuNazvy.Add("η x/u");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_yu.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η y/v");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_zv.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_yy.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_zz.ToString());
            zoznamMenuJednotky.Add("[mm]");

        }
    }
}
