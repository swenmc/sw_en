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
    /// Interaction logic for ComponentList.xaml
    /// </summary>
    public partial class UC_ComponentList : UserControl
    {
        DataSet ds;

        List<string> listMemberPrefix = new List<string>(0);
        List<string> listMemberComponentName = new List<string>();
        List<string> listMemberCrScName = new List<string>();
        List<string> listMemberMaterialName = new List<string>();

        List<bool> listMemberGenerate = new List<bool>();
        List<bool> listMemberDisplay = new List<bool>();
        List<bool> listMemberCalculate = new List<bool>();
        List<bool> listMemberDesign = new List<bool>();
        List<bool> listMemberMaterialList = new List<bool>();

        public UC_ComponentList()
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            // For each component add one row
            listMemberPrefix.Add("MC");
            listMemberPrefix.Add("R");
            listMemberPrefix.Add("EP");
            listMemberPrefix.Add("G");
            listMemberPrefix.Add("P");

            listMemberPrefix.Add("C");
            listMemberPrefix.Add("C");
            listMemberPrefix.Add("G");
            listMemberPrefix.Add("G");

            listMemberComponentName.Add("Main Column");
            listMemberComponentName.Add("Rafter");
            listMemberComponentName.Add("Eaves Purlin");
            listMemberComponentName.Add("Girt");
            listMemberComponentName.Add("Purlin");

            listMemberComponentName.Add("Column - Front Side");
            listMemberComponentName.Add("Column - Back Side");
            listMemberComponentName.Add("Girt - Front Side");
            listMemberComponentName.Add("Girt - Back Side");

            listMemberCrScName.Add("Box 63020");   // Main Column
            listMemberCrScName.Add("Box 63020");   // Rafter
            listMemberCrScName.Add("C 50020");     // Eaves Purlin
            listMemberCrScName.Add("C 27095");     // Girt - Wall
            listMemberCrScName.Add("C 270115");    // Purlin

            listMemberCrScName.Add("Box 10075");   // Front Column
            listMemberCrScName.Add("Box 10075");   // Back Column
            listMemberCrScName.Add("C 27095");     // Front Girt
            listMemberCrScName.Add("C 27095");     // Back Girt

            // Default - other properties
            foreach(string sprefix in listMemberPrefix)
            {
                listMemberMaterialName.Add("G550");
                listMemberGenerate.Add(true);
                listMemberDisplay.Add(true);
                listMemberCalculate.Add(true);
                listMemberDesign.Add(true);
                listMemberMaterialList.Add(true);
            }

            // Create Table
            DataTable table = new DataTable("Table");

            // Create Table Rows
            table.Columns.Add("Prefix", typeof(String));
            table.Columns.Add("ComponentName", typeof(String));
            table.Columns.Add("Section", typeof(String));
            table.Columns.Add("Material", typeof(String));
            table.Columns.Add("Generate", typeof(Boolean));
            table.Columns.Add("Display", typeof(Boolean));
            table.Columns.Add("Calculate", typeof(Boolean));
            table.Columns.Add("Design", typeof(Boolean));
            table.Columns.Add("MaterialList", typeof(Boolean));

            // Set Column Caption
            table.Columns["Prefix"].Caption = "Prefix";
            table.Columns["ComponentName"].Caption = "ComponentName";
            table.Columns["Section"].Caption = "Section";
            table.Columns["Material"].Caption = "Material";
            table.Columns["Generate"].Caption = "Generate";
            table.Columns["Display"].Caption = "Display";
            table.Columns["Calculate"].Caption = "Calculate";
            table.Columns["Design"].Caption = "Design";
            table.Columns["MaterialList"].Caption = "MaterialList";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < listMemberPrefix.Count; i++)
            {
                DataRow row = table.NewRow();

                try
                {
                    row["Prefix"] = listMemberPrefix[i];
                    row["ComponentName"] = listMemberComponentName[i];
                    row["Section"] = listMemberCrScName[i];
                    row["Material"] = listMemberMaterialName[i];
                    row["Generate"] = listMemberGenerate[i];
                    row["Display"] =  listMemberDisplay[i];
                    row["Calculate"] = listMemberCalculate[i];
                    row["Design"] = listMemberDesign[i];
                    row["MaterialList"] = listMemberMaterialList[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_Components.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            // Set Column Header
            /*
            Datagrid_Members.Columns[0].Header = "Prefix";
            Datagrid_Members.Columns[1].Header = "ComponentName";
            Datagrid_Members.Columns[2].Header = "Section";
            Datagrid_Members.Columns[3].Header = "Material";
            Datagrid_Members.Columns[4].Header = "Generate";
            Datagrid_Members.Columns[5].Header = "Display";
            Datagrid_Members.Columns[6].Header = "Calculate";
            Datagrid_Members.Columns[7].Header = "Design";
            Datagrid_Members.Columns[8].Header = "MaterialList";
            */

            // Set Column Width
            /*
            Datagrid_Members.Columns[0].Width = 100;
            Datagrid_Members.Columns[1].Width = 100;
            Datagrid_Members.Columns[2].Width = 100;
            Datagrid_Members.Columns[3].Width = 100;
            Datagrid_Members.Columns[4].Width = 100;
            Datagrid_Members.Columns[5].Width = 100;
            Datagrid_Members.Columns[6].Width = 100;
            Datagrid_Members.Columns[7].Width = 100;
            */


            // TODO Ondrej - zmenit bunky typu string v stlpci cross-secion na na comboboxy

            // Creating new DataGridComboBoxColumn
            DataGridComboBoxColumn myDGCBC_crsc = new DataGridComboBoxColumn();
            myDGCBC_crsc.Header = "crsc";
            // Binding DataGridComboBoxColumn.ItemsSource and DataGridComboBoxColumn.SelectedItem
            string [] s = new string [] { "Box 63020", "Box 63020s1", "Box 63020s2", "C 50020n", "C 270115n", "C 27095n" };
            myDGCBC_crsc.ItemsSource = s;
            myDGCBC_crsc.SelectedItemBinding = new Binding("SelectedValue");
            // Adding DataGridComboBoxColumn to the DataGrid
            Datagrid_Components.Columns.Add(myDGCBC_crsc);
            //Datagrid_Components.Columns[3] = myDGCBC;
        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_Components.ItemsSource = null;
            Datagrid_Components.Items.Clear();
            Datagrid_Components.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listMemberPrefix.Clear();
            listMemberComponentName.Clear();
            listMemberCrScName.Clear();
            listMemberMaterialName.Clear();
            listMemberGenerate.Clear();
            listMemberDisplay.Clear();
            listMemberCalculate.Clear();
            listMemberDesign.Clear();
            listMemberMaterialList.Clear();
        }
    }
}
