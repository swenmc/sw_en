using BaseClasses;
using M_AS4600;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PFD
{
    public static class DataGridHelper
    {
        static List<string> zoznamMenuNazvy = new List<string>(4);          // premenne zobrazene v tabulke
        static List<string> zoznamMenuHodnoty = new List<string>(4);        // hodnoty danych premennych
        static List<string> zoznamMenuJednotky = new List<string>(4);       // jednotky danych premennych
        public static void DisplayDesignResultsInGridView(this CCalculJoint calcul, DataGrid dataGrid)
        {
            DeleteLists();

            SetResultsDetailsFor_ULS(calcul);

            // Create Table
            DataTable table = new DataTable("Table");
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

            // Create Dataset
            DataSet ds = new DataSet();
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
                    if (i >= zoznamMenuNazvy.Count) break;
                    row["Symbol1"] = zoznamMenuNazvy[i];
                    row["Value1"] = zoznamMenuHodnoty[i];
                    row["Unit1"] = zoznamMenuJednotky[i];
                    i++;
                    if (i >= zoznamMenuNazvy.Count) break;
                    row["Symbol2"] = zoznamMenuNazvy[i];
                    row["Value2"] = zoznamMenuHodnoty[i];
                    row["Unit2"] = zoznamMenuJednotky[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            dataGrid.Columns.Clear();
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Binding = new Binding("Symbol") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Unit") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Binding = new Binding("Symbol1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Unit1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Binding = new Binding("Symbol2") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value2") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Unit2") });
            dataGrid.DataContext = ds.Tables[0];  //draw the table to datagridview

            // Styling datagrid
            //TODO - ponastavovat rozne Style property

            Style alignRight = new Style();
            alignRight.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            Style alignLeft = new Style();
            alignLeft.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
            //alignLeft.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));
            Style alignCenter = new Style();
            alignCenter.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            alignCenter.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.AliceBlue)));
            //alignCenter.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));

            dataGrid.Columns[0].CellStyle = alignLeft;
            dataGrid.Columns[1].CellStyle = alignRight;
            dataGrid.Columns[2].CellStyle = alignLeft;
            dataGrid.Columns[3].CellStyle = alignLeft;
            dataGrid.Columns[4].CellStyle = alignRight;
            dataGrid.Columns[5].CellStyle = alignLeft;
            dataGrid.Columns[6].CellStyle = alignLeft;
            dataGrid.Columns[7].CellStyle = alignRight;
            dataGrid.Columns[8].CellStyle = alignLeft;

            // Set Column Header
            dataGrid.Columns[0].Header = dataGrid.Columns[3].Header = dataGrid.Columns[6].Header = "Symbol";
            dataGrid.Columns[1].Header = dataGrid.Columns[4].Header = dataGrid.Columns[7].Header = "Value";
            dataGrid.Columns[2].Header = dataGrid.Columns[5].Header = dataGrid.Columns[8].Header = "Unit";

            //Set Column Width
            //Results_GridView.Columns[0].Width = Results_GridView.Columns[3].Width = Results_GridView.Columns[6].Width = 117;
            //Results_GridView.Columns[1].Width = Results_GridView.Columns[4].Width = Results_GridView.Columns[7].Width = 90;
            //Results_GridView.Columns[2].Width = Results_GridView.Columns[5].Width = Results_GridView.Columns[8].Width = 90;
        }

        private static void DeleteLists()
        {
            // Deleting lists for updating actual values
            zoznamMenuNazvy.Clear();
            zoznamMenuHodnoty.Clear();
            zoznamMenuJednotky.Clear();
        }

        private static void SetResultsDetailsFor_ULS(CCalculJoint calc)
        {
            float fUnitFactor_Force = 0.001f;     // from N to kN
            float fUnitFactor_Moment = 0.001f;    // from Nm to kNm
            float fUnitFactor_Stress = 0.000001f; // from Pa to MPa

            int iNumberOfDecimalPlaces = 3;


            // Display results in datagrid
            zoznamMenuNazvy.Add("fEta_max");
            zoznamMenuHodnoty.Add(calc.fEta_max.ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fM_yu");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fM_yu * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fM_yy");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fM_yy, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fM_zv");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fM_zv, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fM_zz");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fM_zz, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fN");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fN, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fN_c");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fN_c, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fN_t");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fN_t, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fT");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fT, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fV_yu");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fV_yu, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fV_yy");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fV_yy, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fV_zv");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fV_zv, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");

            zoznamMenuNazvy.Add("fV_zz");
            zoznamMenuHodnoty.Add(Math.Round(calc.sDIF.fV_zz, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");
            

            // Maximum design ratio
            zoznamMenuNazvy.Add("η max");
            zoznamMenuHodnoty.Add(Math.Round(calc.fEta_max, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");
        }

        

    }
}
