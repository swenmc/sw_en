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
    /// Interaction logic for UC_WindowsList.xaml
    /// </summary>
    public partial class UC_WindowsList : UserControl
    {
        DataSet ds;

        List<string> listBuildingSide = new List<string>();
        List<int> listBayNumber = new List<int>();
        List<float> listWindowHeight = new List<float>();
        List<float> listWindowWidth = new List<float>();
        List<float> listWindowCoordinateXinBay = new List<float>();
        List<float> listWindowCoordinateZinBay = new List<float>();
        List<int> listWindowNumberOfWindowColumns = new List<int>();

        public UC_WindowsList()
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            // For each component add one row
            listBuildingSide.Add("Back");
            listBuildingSide.Add("Right");
            //listBuildingSide.Add("Right");

            listBayNumber.Add(5);
            listBayNumber.Add(3);
            //listBayNumber.Add(3);

            listWindowHeight.Add(0.6f);
            listWindowHeight.Add(0.7f);
            //listWindowHeight.Add(1.4f);

            listWindowWidth.Add(0.6f);
            listWindowWidth.Add(2.2f);
            //listWindowWidth.Add(1.2f);

            listWindowCoordinateXinBay.Add(0.3f);
            listWindowCoordinateXinBay.Add(0.6f);
            //listWindowCoordinateXinBay.Add(0.7f);

            listWindowCoordinateZinBay.Add(0.8f);
            listWindowCoordinateZinBay.Add(1.5f);
            //listWindowCoordinateZinBay.Add(0.5f);

            listWindowNumberOfWindowColumns.Add(2); // Min 2
            listWindowNumberOfWindowColumns.Add(3);
            //listWindowNumberOfWindowColumns.Add(2);

            // Create Table
            DataTable table = new DataTable("Table");

            // Create Table Rows
            table.Columns.Add("BuildingSide", typeof(String));
            table.Columns.Add("BayNumber", typeof(Int32));
            table.Columns.Add("WindowHeight", typeof(Decimal));
            table.Columns.Add("WindowWidth", typeof(Decimal));
            table.Columns.Add("WindowPositionX", typeof(Decimal));
            table.Columns.Add("WindowPositionZ", typeof(Decimal));
            table.Columns.Add("NumberOfWindowColumns", typeof(Int32));

            // Set Column Caption
            table.Columns["BuildingSide"].Caption = "BuildingSide";
            table.Columns["BayNumber"].Caption = "BayNumber";
            table.Columns["WindowHeight"].Caption = "WindowHeight";
            table.Columns["WindowWidth"].Caption = "WindowWidth";
            table.Columns["WindowPositionX"].Caption = "WindowPositionX";
            table.Columns["WindowPositionZ"].Caption = "WindowPositionZ";
            table.Columns["NumberOfWindowColumns"].Caption = "NumberOfWindowColumns";

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
                    row["WindowHeight"] = listWindowHeight[i];
                    row["WindowWidth"] = listWindowWidth[i];
                    row["WindowPositionX"] = listWindowCoordinateXinBay[i];
                    row["WindowPositionZ"] = listWindowCoordinateZinBay[i];
                    row["NumberOfWindowColumns"] = listWindowNumberOfWindowColumns[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_Windows.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            // Set Column Header
            /*
            Datagrid_Members.Columns[0].Header = "BuildingSide";
            Datagrid_Members.Columns[1].Header = "BayNumber";
            Datagrid_Members.Columns[2].Header = "WindowHeight";
            Datagrid_Members.Columns[3].Header = "WindowWidth";
            Datagrid_Members.Columns[4].Header = "WindowPositionX";
            Datagrid_Members.Columns[5].Header = "WindowPositionZ";
            Datagrid_Members.Columns[6].Header = "NumberOfWindowColumns";
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
            */
        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_Windows.ItemsSource = null;
            Datagrid_Windows.Items.Clear();
            Datagrid_Windows.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listBuildingSide.Clear();
            listBayNumber.Clear();
            listWindowHeight.Clear();
            listWindowWidth.Clear();
            listWindowCoordinateXinBay.Clear();
            listWindowCoordinateZinBay.Clear();
            listWindowNumberOfWindowColumns.Clear();
        }
    }
}
