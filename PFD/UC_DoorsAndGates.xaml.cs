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
    /// Interaction logic for UC_DoorsAndGatesList.xaml
    /// </summary>
    public partial class UC_DoorsAndGatesList : UserControl
    {
        DataSet ds;

        List<string> listBuildingSide = new List<string>();
        List<int> listBayNumber = new List<int>();
        List<float> listDoorHeight = new List<float>();
        List<float> listDoorWidth = new List<float>();
        List<float> listDoorCoordinateXinBlock = new List<float>();

        public UC_DoorsAndGatesList()
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            // For each component add one row
            listBuildingSide.Add("Left");
            listBuildingSide.Add("Right");
            listBuildingSide.Add("Left");
            listBuildingSide.Add("Right");
            listBayNumber.Add(1);
            listBayNumber.Add(2);
            listBayNumber.Add(3);
            listBayNumber.Add(1);

            listDoorHeight.Add(2.1f);
            listDoorHeight.Add(2.2f);
            listDoorHeight.Add(2.4f);
            listDoorHeight.Add(2.6f);

            listDoorWidth.Add(0.9f);
            listDoorWidth.Add(1.5f);
            listDoorWidth.Add(1.2f);
            listDoorWidth.Add(2.5f);

            listDoorCoordinateXinBlock.Add(1.1f);
            listDoorCoordinateXinBlock.Add(1.6f);
            listDoorCoordinateXinBlock.Add(1.5f);
            listDoorCoordinateXinBlock.Add(0.6f);

            // Create Table
            DataTable table = new DataTable("Table");

            // Create Table Rows
            table.Columns.Add("BuildingSide", typeof(String));
            table.Columns.Add("BayNumber", typeof(Int32));
            table.Columns.Add("DoorHeight", typeof(Decimal));
            table.Columns.Add("DoorWidth", typeof(Decimal));
            table.Columns.Add("DoorPosition", typeof(Decimal));

            // Set Column Caption
            table.Columns["BuildingSide"].Caption = "BuildingSide";
            table.Columns["BayNumber"].Caption = "BayNumber";
            table.Columns["DoorHeight"].Caption = "DoorHeight";
            table.Columns["DoorWidth"].Caption = "DoorWidth";
            table.Columns["DoorPosition"].Caption = "DoorPosition";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < listBuildingSide.Count; i++)
            {
                DataRow row = table.NewRow();

                try
                {
                    row["BuildingSide"] = listBuildingSide[i];
                    row["BayNumber"] = listBayNumber[i];
                    row["DoorHeight"] = listDoorHeight[i];
                    row["DoorWidth"] = listDoorWidth[i];
                    row["DoorPosition"] = listDoorCoordinateXinBlock[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_DoorsAndGates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            // Set Column Header
            /*
            Datagrid_Members.Columns[0].Header = "BuildingSide";
            Datagrid_Members.Columns[1].Header = "BayNumber";
            Datagrid_Members.Columns[2].Header = "DoorHeight";
            Datagrid_Members.Columns[3].Header = "DoorWidth";
            Datagrid_Members.Columns[4].Header = "DoorPosition";
            */

            // Set Column Width
            /*
            Datagrid_Members.Columns[0].Width = 100;
            Datagrid_Members.Columns[1].Width = 100;
            Datagrid_Members.Columns[2].Width = 100;
            Datagrid_Members.Columns[3].Width = 100;
            Datagrid_Members.Columns[4].Width = 100;
            Datagrid_Members.Columns[5].Width = 100;
            */
        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_DoorsAndGates.ItemsSource = null;
            Datagrid_DoorsAndGates.Items.Clear();
            Datagrid_DoorsAndGates.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listBuildingSide.Clear();
            listBayNumber.Clear();
            listDoorHeight.Clear();
            listDoorWidth.Clear();
            listDoorCoordinateXinBlock.Clear();
        }
    }
}
