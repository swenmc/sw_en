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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using BaseClasses;
using MATH;

namespace PFD
{
    /// <summary>
    /// Interaction logic for LoadCombinationsList.xaml
    /// </summary>
    public partial class UC_LoadCombinationList : UserControl
    {
        DataSet ds;

        List<int> listLoadCombinationID = new List<int>();
        List<string> listLoadCombinationName = new List<string>();
        //List<string> listLoadCombinationType = new List<string>();
        List<string> listLoadCombinationLimitState = new List<string>();
        List<string> listLoadCombinationLoadCases = new List<string>();
        List<string> listLoadCombinationKeys = new List<string>();
        List<string> listFormulas = new List<string>();

        public UC_LoadCombinationList(CModel_PFD model)
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            if (model is CModel_PFD_01_MR) // !!! Docasna vynimka - neviem ci je nieco zle alebo to tu spadlo z dovodu inej chyby alebo vynimky
            {
                CModel_PFD_01_MR m = (CModel_PFD_01_MR)model;
                if (m.m_arrLoadCombs == null) return; // TO Ondrej - tu mi to spadlo s tym ze zoznam kombinacii je null
            }
            else if (model is CModel_PFD_01_GR) // !!! Docasna vynimka - neviem ci je nieco zle alebo to tu spadlo z dovodu inej chyby alebo vynimky
            {
                CModel_PFD_01_GR m = (CModel_PFD_01_GR)model;
                if(m.m_arrLoadCombs == null) return; // TO Ondrej - tu mi to spadlo s tym ze zoznam kombinacii je null
            }
            else
            {
                throw new Exception("Kitset model is not implemented.");
            }

            // For each load combination add one row
            for (int i = 0; i < model.m_arrLoadCombs.Length; i++)
            {
                listLoadCombinationID.Add(model.m_arrLoadCombs[i].ID);
                listLoadCombinationName.Add(model.m_arrLoadCombs[i].Name);
                listLoadCombinationLimitState.Add(model.m_arrLoadCombs[i].eLComType == ELSType.eLS_ULS ? "ULS" : "SLS"); // ! Todo ak sa prida dalsi type LS je nutne upravit
                listLoadCombinationKeys.Add(model.m_arrLoadCombs[i].CombinationKey);
                listFormulas.Add(model.m_arrLoadCombs[i].Formula);

                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < model.m_arrLoadCombs[i].LoadCasesList.Count; j++)
                {
                    sb.AppendFormat("{0:F2} * LC{1}", Math.Round(model.m_arrLoadCombs[i].LoadCasesFactorsList[j], 2), model.m_arrLoadCombs[i].LoadCasesList[j].ID);
                    
                    if(j< model.m_arrLoadCombs[i].LoadCasesList.Count - 1) // All except the last LC
                       sb.Append(" + ");
                }
                listLoadCombinationLoadCases.Add(sb.ToString());
            }

            // Create Table
            DataTable table = new DataTable("Table");

            // Create Table Rows
            table.Columns.Add("ID", typeof(Int32));
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("LimitState", typeof(String));
            table.Columns.Add("LoadCases", typeof(String));
            table.Columns.Add("CombinationKey", typeof(String));
            table.Columns.Add("Formula", typeof(String));

            // Set Column Caption
            table.Columns["ID"].Caption = "ID";
            table.Columns["Name"].Caption = "Name";
            table.Columns["LimitState"].Caption = "Limit State";
            table.Columns["LoadCases"].Caption = "Load Cases";
            table.Columns["CombinationKey"].Caption = "Combination Key";
            table.Columns["Formula"].Caption = "Formula";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < listLoadCombinationID.Count; i++)
            {
                DataRow row = table.NewRow();

                try
                {
                    row["ID"] = listLoadCombinationID[i];
                    row["Name"] = listLoadCombinationName[i];
                    row["LimitState"] = listLoadCombinationLimitState[i];
                    row["LoadCases"] = listLoadCombinationLoadCases[i];
                    row["CombinationKey"] = listLoadCombinationKeys[i];
                    row["Formula"] = listFormulas[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_LoadCombinations.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_LoadCombinations.ItemsSource = null;
            Datagrid_LoadCombinations.Items.Clear();
            Datagrid_LoadCombinations.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listLoadCombinationID.Clear();
            listLoadCombinationName.Clear();
            listLoadCombinationLimitState.Clear();
            listLoadCombinationLoadCases.Clear();
        }
    }
}
