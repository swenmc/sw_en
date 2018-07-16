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
    /// Interaction logic for LoadCasesList.xaml
    /// </summary>
    public partial class UC_LoadCaseList : UserControl
    {
        DataSet ds;

        List<int> listLoadCaseID = new List<int>();
        List<string> listLoadCaseName = new List<string>();
        List<string> listLoadCaseType = new List<string>();

        public UC_LoadCaseList(CModel model)
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            // For each load case add one row
            for (int i = 0; i < model.m_arrLoadCases.Length; i++)
            {
                listLoadCaseID.Add(model.m_arrLoadCases[i].ID);
                listLoadCaseName.Add(model.m_arrLoadCases[i].Name);
                listLoadCaseType.Add(model.m_arrLoadCases[i].Type);
            }

            // Create Table
            DataTable table = new DataTable("Table");

            // Create Table Rows
            table.Columns.Add("ID", typeof(Int32));
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Type", typeof(String));

            // Set Column Caption
            table.Columns["ID"].Caption = "ID";
            table.Columns["Name"].Caption = "Name";
            table.Columns["Type"].Caption = "Type";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < listLoadCaseID.Count; i++)
            {
                DataRow row = table.NewRow();

                try
                {
                    row["ID"] = listLoadCaseID[i];
                    row["Name"] = listLoadCaseName[i];
                    row["Type"] = listLoadCaseType[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_LoadCases.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            // Set Column Header
            /*
            Datagrid_Members.Columns[0].Header = "ID";
            Datagrid_Members.Columns[1].Header = "Name";
            Datagrid_Members.Columns[2].Header = "Type";
            */

            // Set Column Width
            /*
            Datagrid_Members.Columns[0].Width = 100;
            Datagrid_Members.Columns[1].Width = 100;
            Datagrid_Members.Columns[2].Width = 100;
            */
        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_LoadCases.ItemsSource = null;
            Datagrid_LoadCases.Items.Clear();
            Datagrid_LoadCases.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listLoadCaseID.Clear();
            listLoadCaseName.Clear();
            listLoadCaseType.Clear();
        }
    }
}
