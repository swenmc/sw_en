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
        static List<string> listPhysicalQuantity_Symbols = new List<string>(4);          // premenne zobrazene v tabulke
        static List<string> listPhysicalQuantity_Values = new List<string>(4);           // hodnoty danych premennych
        static List<string> listPhysicalQuantity_Units = new List<string>(4);            // jednotky danych premennych

        public static void DisplayDesignResultsInGridView(this CCalculMember calcul, ELSType eCombinationType, DataGrid dataGrid)
        {
            DeleteLists();

            if (eCombinationType == ELSType.eLS_ULS)
                SetResultsDetailsFor_ULS(calcul);
            else
                SetResultsDetailsFor_SLS(calcul);

            CreateTableInDataGrid(ref dataGrid);
        }

        public static void DisplayDesignResultsInGridView(this CCalculJoint calcul, DataGrid dataGrid)
        {
            DeleteLists();

            SetResultsDetailsFor_ULS(calcul);

            CreateTableInDataGrid(ref dataGrid);
        }

        private static void DeleteLists()
        {
            // Deleting lists for updating actual values
            listPhysicalQuantity_Symbols.Clear();
            listPhysicalQuantity_Values.Clear();
            listPhysicalQuantity_Units.Clear();
        }

        private static void CreateTableInDataGrid(ref DataGrid dataGrid)
        {
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

            for (int i = 0; i < listPhysicalQuantity_Symbols.Count; i++)
            {
                DataRow row = table.NewRow();
                try
                {
                    row["Symbol"] = listPhysicalQuantity_Symbols[i];
                    row["Value"] = listPhysicalQuantity_Values[i];
                    row["Unit"] = listPhysicalQuantity_Units[i];
                    i++;
                    if (i >= listPhysicalQuantity_Symbols.Count) break;
                    row["Symbol1"] = listPhysicalQuantity_Symbols[i];
                    row["Value1"] = listPhysicalQuantity_Values[i];
                    row["Unit1"] = listPhysicalQuantity_Units[i];
                    i++;
                    if (i >= listPhysicalQuantity_Symbols.Count) break;
                    row["Symbol2"] = listPhysicalQuantity_Symbols[i];
                    row["Value2"] = listPhysicalQuantity_Values[i];
                    row["Unit2"] = listPhysicalQuantity_Units[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            int iColumnWidth_Symbol = 120;
            int iColumnWidth_Value = 80;
            int iColumnWidth_Unit = 55;

            dataGrid.Columns.Clear();
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Width = iColumnWidth_Symbol, Binding = new Binding("Symbol") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Width = iColumnWidth_Value, Binding = new Binding("Value") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Width = iColumnWidth_Unit, Binding = new Binding("Unit") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Width = iColumnWidth_Symbol, Binding = new Binding("Symbol1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Width = iColumnWidth_Value, Binding = new Binding("Value1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Width = iColumnWidth_Unit, Binding = new Binding("Unit1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Width = iColumnWidth_Symbol, Binding = new Binding("Symbol2") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Width = iColumnWidth_Value, Binding = new Binding("Value2") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Width = iColumnWidth_Unit, Binding = new Binding("Unit2") });
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

        // Member Design

        private static void SetResultsDetailsFor_ULS(CCalculMember obj_CalcDesign)
        {
            float fUnitFactor_Force = 0.001f;     // from N to kN
            float fUnitFactor_Moment = 0.001f;    // from Nm to kNm
            float fUnitFactor_Stress = 0.000001f; // from Pa to MPa

            float fUnitFactor_ComponentDimension = 1000f; // m to mm
            float fUnitFactor_CrSc_Area = 1000000f; // m^2 to mm^2

            int iNumberOfDecimalPlaces = 3;
            int iNumberOfDecimalPlaces_Factor = 3;
            int iNumberOfDecimalPlaces_DesignRatio = 3;

            string sUnit_Force = "[kN]";
            string sUnit_Moment = "[kNm]";
            string sUnit_Stress = "[MPa]";

            string sUnit_ComponentDimension = "[mm]";
            string sUnit_CrSc_Area = "[mm²]";

            string sUnit_Factor = "[-]";
            string sUnit_DesignRatio = "[-]";

            // Display results in datagrid
            // Design Internal Forces

            // Internal forces in joint
            listPhysicalQuantity_Symbols.Add("N");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.sDIF.fN * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            listPhysicalQuantity_Symbols.Add("Vx");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.sDIF.fV_xu_xx * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            listPhysicalQuantity_Symbols.Add("Vy");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.sDIF.fV_yv_yy * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            //listPhysicalQuantity_Symbols.Add("T");
            //listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fT * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            //listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("Mx");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.sDIF.fM_xu_xx * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("My");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.sDIF.fM_yv_yy * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            // AS 4600 output variables
            // Tension
            if (obj_CalcDesign.fEta_Nt > 0)
            {
                listPhysicalQuantity_Symbols.Add("Φ t");
                listPhysicalQuantity_Values.Add(obj_CalcDesign.fPhi_t.ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("N t,min");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_t_min * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_Nt, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }

            // Compression
            // Global Buckling
            if (obj_CalcDesign.fEta_721_N > 0 || obj_CalcDesign.fEta_722_M_xu > 0)
            {
                listPhysicalQuantity_Symbols.Add("f ox");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_ox * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("f oy");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_oy * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("f oz");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_oz * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                if (obj_CalcDesign.fEta_721_N > 0)
                {
                    listPhysicalQuantity_Symbols.Add("f oc");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_oc * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Stress);

                    listPhysicalQuantity_Symbols.Add("λ c");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.flambda_c, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("N y");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_y * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("N oc");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_oc * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("N ce");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_ce * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    // Local Buckling
                    listPhysicalQuantity_Symbols.Add("f ol");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_oy * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Stress);

                    listPhysicalQuantity_Symbols.Add("λ l");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.flambda_l, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("N ol");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_ol * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("N cl");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_cl * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    // Distorsial Buckling
                    listPhysicalQuantity_Symbols.Add("f od");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_od * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Stress);

                    listPhysicalQuantity_Symbols.Add("λ d");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.flambda_d, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("N od");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_od * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("N cd");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_cd * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("N c,min");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fN_c_min * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Φ c");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fPhi_c, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("η Nc");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_721_N, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
                }
            }

            // Bending

            listPhysicalQuantity_Symbols.Add("M p,x");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_p_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("M y,x");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_y_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("M p,y");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_p_yv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("M y,y");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_y_yv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            if (obj_CalcDesign.fEta_722_M_xu > 0)
            {
                // Bending about x/u axis
                listPhysicalQuantity_Symbols.Add("C b");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fC_b, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("M o,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_o_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("M be,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_be_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                // Local Buckling
                listPhysicalQuantity_Symbols.Add("f ol,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_ol_bend * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("M ol,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_ol_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("λ l,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fLambda_l_xu, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("M bl,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_bl_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                // Distrosial buckling
                listPhysicalQuantity_Symbols.Add("f od,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.ff_od_bend * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("M od,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_od_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("λ d,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fLambda_d_xu, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("M bd,x");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_bd_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("Φ b");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fPhi_b, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_722_M_xu, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }

            // Shear
            if (obj_CalcDesign.fEta_723_9_xu_yv > 0)
            {
                listPhysicalQuantity_Symbols.Add("V y,y");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fV_y_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V v,y");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fV_v_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V cr,y");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fV_cr_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("λ v,y");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fLambda_v_yv, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Φ v");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fPhi_v, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_723_9_xu_yv, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_723_11_V_yv, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                if (obj_CalcDesign.fEta_723_10_xu > 0)
                {
                    listPhysicalQuantity_Symbols.Add("η");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_723_10_xu, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
                }

                if (obj_CalcDesign.fEta_723_12_xu_yv_10 > 0)
                {
                    listPhysicalQuantity_Symbols.Add("η");
                    listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_723_12_xu_yv_10, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
                }
            }

            // Interation of internal forces
            listPhysicalQuantity_Symbols.Add("M s,x");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_s_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("M b,x");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_b_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            // Compression and bending
            if (obj_CalcDesign.fEta_721_N > 0 && obj_CalcDesign.fEta_724 > 0)
            {
                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_724, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }

            // Tension and bending
            if (obj_CalcDesign.fEta_Nt > 0 && obj_CalcDesign.fEta_725_1 > 0)
            {
                listPhysicalQuantity_Symbols.Add("M s,x,f");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_s_xu_f * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("M s,y,f");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fM_s_yv_f * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_725_1, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_725_2, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }

            // Maximum design ratio
            listPhysicalQuantity_Symbols.Add("η max");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_max, iNumberOfDecimalPlaces_DesignRatio).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
        }

        private static void SetResultsDetailsFor_SLS(CCalculMember obj_CalcDesign)
        {
            float fUnitFactor_Deflection = 1000f; // m to mm

            int iNumberOfDecimalPlaces = 3;
            int iNumberOfDecimalPlaces_Factor = 3;
            int iNumberOfDecimalPlaces_DesignRatio = 3;

            int iNumberOfDecimalPlaces_Deflection = 3;

            string sUnit_Deflection = "[mm]";

            string sUnit_Factor = "[-]";
            string sUnit_DesignRatio = "[-]";

            // Display results in datagrid

            // Deflection
            // δ

            listPhysicalQuantity_Symbols.Add("δ lim");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fLimitDeflection * fUnitFactor_Deflection, iNumberOfDecimalPlaces_Deflection).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Deflection);

            // Design ratio
            listPhysicalQuantity_Symbols.Add("η x/u");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_defl_yu, iNumberOfDecimalPlaces_DesignRatio).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

            listPhysicalQuantity_Symbols.Add("η y/v");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_defl_zv, iNumberOfDecimalPlaces_DesignRatio).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

            listPhysicalQuantity_Symbols.Add("η x");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_defl_yy, iNumberOfDecimalPlaces_DesignRatio).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

            listPhysicalQuantity_Symbols.Add("η y");
            listPhysicalQuantity_Values.Add(Math.Round(obj_CalcDesign.fEta_defl_zz, iNumberOfDecimalPlaces_DesignRatio).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
        }

        // Joint Design

        private static void SetResultsDetailsFor_ULS(CCalculJoint calc)
        {
            float fUnitFactor_Force = 0.001f;     // from N to kN
            float fUnitFactor_Moment = 0.001f;    // from Nm to kNm
            float fUnitFactor_Stress = 0.000001f; // from Pa to MPa

            float fUnitFactor_ComponentDimension = 1000f; // m to mm
            float fUnitFactor_ComponentArea = 1000000f; // m^2 to mm^2

            float fUnitFactor_Density = 1f; // kg/m^3 to kg/m^3

            int iNumberOfDecimalPlaces = 3;
            int iNumberOfDecimalPlaces_Factor = 3;
            int iNumberOfDecimalPlaces_DesignRatio = 3;

            string sUnit_Force = "[kN]";
            string sUnit_Moment = "[kNm]";
            string sUnit_Stress = "[MPa]";
            string sUnit_Density = "[kg/m³]";

            string sUnit_ComponentDimension = "[mm]";
            string sUnit_ComponentArea = "[mm²]";

            string sUnit_Factor = "[-]";
            string sUnit_DesignRatio = "[-]";

            // Display results in datagrid

            // Knee and Apex Joint
            if (calc.joint is CConnectionJoint_A001 || calc.joint is CConnectionJoint_B001)
            {
                CJointDesignDetails_ApexOrKnee det = (CJointDesignDetails_ApexOrKnee)calc.joint.DesignDetails;

                if (calc.joint is CConnectionJoint_A001)
                    listPhysicalQuantity_Symbols.Add("Apex connection");
                else
                    listPhysicalQuantity_Symbols.Add("Knee connection");
                listPhysicalQuantity_Values.Add("");
                listPhysicalQuantity_Units.Add("");

                DisplayBasicValues(calc);

                listPhysicalQuantity_Symbols.Add("Φplate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_Plate, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("An.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Nt.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Av.y.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_vn_yv_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Vy.y.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_y_yv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vy.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_yv_3341_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("M.b.x.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fM_xu_resitance_plate * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η Mx.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Mb_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Shear in connection
                listPhysicalQuantity_Symbols.Add("Φv.screw");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_shear_screw, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Vb.screw.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fVb_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Vb.screw.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fVb_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Number of screws in shear");
                listPhysicalQuantity_Values.Add(det.iNumberOfScrewsInShear.ToString());
                listPhysicalQuantity_Units.Add("");

                listPhysicalQuantity_Symbols.Add("η screw.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η screw.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Plastic Design
                listPhysicalQuantity_Symbols.Add("M.b.x.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fMb_MainMember_oneside_plastic * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("M.b.x.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fMb_SecondaryMember_oneside_plastic * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η Mb.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Mb_MainMember_oneside_plastic, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Mb.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Mb_SecondaryMember_oneside_plastic, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Elastic Design
                listPhysicalQuantity_Symbols.Add("V*b.screw.Mx");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw_Mxu * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.Vy");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw_Vyv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.N");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw_N * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.res");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vb.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Vb_5424_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vb.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Vb_5424_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vb.min");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Vb_5424, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 5.4.2.5 Connection shear as limited by end distance
                listPhysicalQuantity_Symbols.Add("e");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("Vfv.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Vfv.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Vfv.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_Plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*fv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_fv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vfv.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vfv.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vfv.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_Plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vfv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 5.4.2.6 Screws in shear
                listPhysicalQuantity_Symbols.Add("Vw.screw");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_w_nom_screw_5426 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vw");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_w_5426, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 5.4.2.3 Tension in the connected part
                listPhysicalQuantity_Symbols.Add("Φsection");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_CrSc, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("An.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_MainMember * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Nt.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_section_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("An.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_SecondaryMember * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Nt.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_section_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt.m2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }
            else if (calc.joint.m_SecondaryMembers != null)
            {
                // Purlins, girts, ...
                if (calc.joint is CConnectionJoint_T001 || calc.joint is CConnectionJoint_T002 || calc.joint is CConnectionJoint_T003)
                {
                    DisplayBasicValues(calc);

                    CJointDesignDetails_GirtOrPurlin det = (CJointDesignDetails_GirtOrPurlin)calc.joint.DesignDetails;

                    // 5.4.3 Screwed connections in tension
                    // 5.4.3.2 Pull-out and pull-over (pull-through)

                    listPhysicalQuantity_Symbols.Add("Number of screws in tension");
                    listPhysicalQuantity_Values.Add(det.iNumberOfScrewsInTension.ToString());
                    listPhysicalQuantity_Units.Add("");

                    listPhysicalQuantity_Symbols.Add("Φn.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_N_screw, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Nt.screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_5432_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Nt.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5432_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Pripoj plechu k hlavnemu prutu
                    // Tension and shear
                    listPhysicalQuantity_Symbols.Add("Φv.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_shear_Vb_Nov, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("C.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fC_for5434_MainMember, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Vb.screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_b_for5434_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("dw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fd_w_for5434_plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("Nov.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_ov_for5434_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("V*b.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_for5434_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_5434_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("Vb.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_b_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Nou.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_ou_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("V*b.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_5435_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.2.5 Connection shear as limited by end distance
                    listPhysicalQuantity_Symbols.Add("e");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fe_Plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("V*fv.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_fv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vfv.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_Plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425_Plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.2.6 Screws in shear
                    listPhysicalQuantity_Symbols.Add("Vw.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_w_nom_screw_5426 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_w_5426, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.3.3 Screws in tension
                    listPhysicalQuantity_Symbols.Add("Φn.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_N_t_screw, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Nt.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_nom_screw_5433 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_screw_5433, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_N_t_screw_5436, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Plate design
                    listPhysicalQuantity_Symbols.Add("Φplate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_plate, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("An.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                    listPhysicalQuantity_Symbols.Add("Nt.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Nt.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("Avn.y.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fA_vn_yv_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                    listPhysicalQuantity_Symbols.Add("Vy.y.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_y_yv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vy.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_yv_3341_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Pripoj plechu sekundarneho pruta
                    listPhysicalQuantity_Symbols.Add("Number of screws in connection m2");
                    listPhysicalQuantity_Values.Add(det.iNumberOfScrewsInConnectionOfSecondaryMember.ToString());
                    listPhysicalQuantity_Units.Add("");

                    listPhysicalQuantity_Symbols.Add("V*b.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vb.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fVb_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vb.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Vb_5424_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("e.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fe_SecondaryMember * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("V*fv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vfv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vfv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
                }
                else if (calc.joint is CConnectionJoint_S001) // Front / back column connection to the main rafter
                {
                    listPhysicalQuantity_Symbols.Add("Connection to the rafter");
                    listPhysicalQuantity_Values.Add("");
                    listPhysicalQuantity_Units.Add("");

                    DisplayBasicValues(calc);

                   CJointDesignDetails_FrontOrBackColumnToMainRafterJoint det = (CJointDesignDetails_FrontOrBackColumnToMainRafterJoint)calc.joint.DesignDetails;

                    // 5.4.3 Screwed connections in tension
                    // 5.4.3.2 Pull-out and pull-over (pull-through)

                    listPhysicalQuantity_Symbols.Add("Number of screws in tension");
                    listPhysicalQuantity_Values.Add(det.iNumberOfScrewsInTension.ToString());
                    listPhysicalQuantity_Units.Add("");

                    listPhysicalQuantity_Symbols.Add("Φn.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_N_screw, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Nt.screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_5432_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Nt.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5432_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Pripoj plechu k hlavnemu prutu
                    // Tension and shear
                    listPhysicalQuantity_Symbols.Add("Φv.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_shear_Vb_Nov, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("C.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fC_for5434_MainMember, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Vb.screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_b_for5434_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("dw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fd_w_for5434_plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("Nov.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_ov_for5434_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("V*b.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_for5434_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_5434_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("Vb.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_b_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Nou.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_ou_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("V*b.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_5435_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.2.5 Connection shear as limited by end distance
                    listPhysicalQuantity_Symbols.Add("e");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fe_Plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("V*fv.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_fv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vfv.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_Plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425_Plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.2.6 Screws in shear
                    listPhysicalQuantity_Symbols.Add("Vw.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_w_nom_screw_5426 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_w_5426, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.3.3 Screws in tension
                    listPhysicalQuantity_Symbols.Add("Φn.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_N_t_screw, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Nt.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_nom_screw_5433 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_screw_5433, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_N_t_screw_5436, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Plate design
                    listPhysicalQuantity_Symbols.Add("Φplate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_plate, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("An.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                    listPhysicalQuantity_Symbols.Add("Nt.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Nt.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("Avn.y.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fA_vn_yv_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                    listPhysicalQuantity_Symbols.Add("Vy.y.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_y_yv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vy.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_yv_3341_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Pripoj plechu sekundarneho pruta
                    listPhysicalQuantity_Symbols.Add("Number of screws in connection m2");
                    listPhysicalQuantity_Values.Add(det.iNumberOfScrewsInConnectionOfSecondaryMember.ToString());
                    listPhysicalQuantity_Units.Add("");

                    listPhysicalQuantity_Symbols.Add("V*b.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vb.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fVb_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vb.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Vb_5424_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("e.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fe_SecondaryMember * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("V*fv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vfv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vfv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
                }
                else
                {
                    // Exception - not defined type
                    throw new Exception("Joint type design is not implemented!");
                }
            }
            else if (calc.joint is CConnectionJoint_TA01 || calc.joint is CConnectionJoint_TB01)
            {
                CJointDesignDetails_BaseJoint det = (CJointDesignDetails_BaseJoint)calc.joint.DesignDetails;

                listPhysicalQuantity_Symbols.Add("Footing Base");
                listPhysicalQuantity_Values.Add("");
                listPhysicalQuantity_Units.Add("");

                DisplayBasicValues(calc);

                listPhysicalQuantity_Symbols.Add("Φplate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_plate, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("An.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Nt.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Av.y.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_vn_yv_plate * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Vy.y.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_y_yv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vy.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_yv_3341_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("M.b.x.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fM_xu_resistance_plate * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η Mx.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Mb_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Shear in connection
                listPhysicalQuantity_Symbols.Add("Φv.screw");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_shear_screw, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Vb.screw.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fVb_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Number of screws in shear");
                listPhysicalQuantity_Values.Add(det.iNumberOfScrewsInShear.ToString());
                listPhysicalQuantity_Units.Add("");

                listPhysicalQuantity_Symbols.Add("η screw.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Plastic Design
                listPhysicalQuantity_Symbols.Add("M.b.x.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fMb_MainMember_oneside_plastic * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η Mb.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Mb_MainMember_oneside_plastic, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Elastic Design
                listPhysicalQuantity_Symbols.Add("V*b.screw.Mx");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw_Mxu * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.Vy");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw_Vyv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.N");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw_N * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.res");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_b_max_screw * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vb.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_Vb_5424_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 5.4.2.5 Connection shear as limited by end distance
                listPhysicalQuantity_Symbols.Add("e");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("Vfv.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Vfv.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_Plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*fv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_fv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vfv.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vfv.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_Plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vfv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_fv_5425, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 5.4.2.6 Screws in shear
                listPhysicalQuantity_Symbols.Add("Vw.screw");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_w_nom_screw_5426 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vw");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_V_w_5426, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 5.4.2.3 Tension in the connected part
                listPhysicalQuantity_Symbols.Add("Φsection");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_CrSc, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("An.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_MainMember * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Nt.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_t_section_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt.m1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fEta_N_t_5423_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);






                // Anchors
                listPhysicalQuantity_Symbols.Add("N*joint.up");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_asterix_joint_uplif * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("N*joint.brg");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_asterix_joint_bearing * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*x.joint");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_x_joint * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*y.joint");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_y_joint * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*joint");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_res_joint * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("No anchors");
                listPhysicalQuantity_Values.Add(det.iNumberAnchors.ToString());
                listPhysicalQuantity_Units.Add("");

                listPhysicalQuantity_Symbols.Add("No anchors.t");
                listPhysicalQuantity_Values.Add(det.iNumberAnchors_t.ToString());
                listPhysicalQuantity_Units.Add("");

                listPhysicalQuantity_Symbols.Add("No anchors.v");
                listPhysicalQuantity_Values.Add(det.iNumberAnchors_t.ToString());
                listPhysicalQuantity_Units.Add("");

                listPhysicalQuantity_Symbols.Add("N*anchor.up");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_asterix_anchor_uplif * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*anchor");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_asterix_anchor * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("wx.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fplateWidth_x * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("wy.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fplateWidth_y * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("bx.footing");
                listPhysicalQuantity_Values.Add(Math.Round(det.fFootingDimension_x * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("by.footing");
                listPhysicalQuantity_Values.Add(Math.Round(det.fFootingDimension_y * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("hfooting");
                listPhysicalQuantity_Values.Add(Math.Round(det.fFootingHeight * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("ex.a.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe_x_AnchorToPlateEdge * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("ey.a.plate");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe_y_AnchorToPlateEdge * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("ex.p.footing");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe_x_BasePlateToFootingEdge * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("ey.p.footing");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe_y_BasePlateToFootingEdge * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("bx.washer");
                listPhysicalQuantity_Values.Add(Math.Round(det.fu_x_Washer * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("by.washer");
                listPhysicalQuantity_Values.Add(Math.Round(det.fu_y_Washer * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("f'c");
                listPhysicalQuantity_Values.Add(Math.Round(det.ff_apostrophe_c * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("ρc");
                listPhysicalQuantity_Values.Add(Math.Round(det.fRho_c * fUnitFactor_Density, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Density);

                listPhysicalQuantity_Symbols.Add("ds"); // thread
                listPhysicalQuantity_Values.Add(Math.Round(det.fd_s * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("df"); // shank
                listPhysicalQuantity_Values.Add(Math.Round(det.fd_f * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("Ac");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_c * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Ao");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_o * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("fy.b");
                listPhysicalQuantity_Values.Add(Math.Round(det.ff_y_anchor * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("fu.b");
                listPhysicalQuantity_Values.Add(Math.Round(det.ff_u_anchor * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                // AS / NZS 4600:2018 - 5.3 Bolted connections
                listPhysicalQuantity_Symbols.Add("Φv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_v_532, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Vf");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_f_532 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vf");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_532_1, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Φv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_v_534, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("α");
                listPhysicalQuantity_Values.Add(Math.Round(det.fAlpha_5342, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("C");
                listPhysicalQuantity_Values.Add(Math.Round(det.fC_5342, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Vb");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_b_5342 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vb");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_5342, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Vb");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_b_5343 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vb");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_5343, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Φ");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_535, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Vfv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fV_fv_5351_2_anchor * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vfv");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_5351_2, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Nft");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_ft_5352_1 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nft");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_5352_1, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_5353, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // NZS 3101.1 - 2006

                listPhysicalQuantity_Symbols.Add("Φelasticity");
                listPhysicalQuantity_Values.Add(Math.Round(det.fElasticityFactor_1764, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Φt.a");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_anchor_tension_173, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Φv.a");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_anchor_shear_174, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Φt.c");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_concrete_tension_174a, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Φv.c");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPhi_concrete_shear_174b, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                // 17.5.7.1  Steel strength of anchor in tension
                listPhysicalQuantity_Symbols.Add("Ase");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_se * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Ns.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_s_176_group * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Ns.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_17571_group, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // 17.5.7.2  Strength of concrete breakout of anchor

                listPhysicalQuantity_Symbols.Add("e'n");
                listPhysicalQuantity_Values.Add(Math.Round(det.fe_apostrophe_n * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("c");
                listPhysicalQuantity_Values.Add(Math.Round(det.fConcreteCover * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("hef");
                listPhysicalQuantity_Values.Add(Math.Round(det.fh_ef * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("s2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fs_2_x * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("s1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fs_1_y * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("smin");
                listPhysicalQuantity_Values.Add(Math.Round(det.fs_min * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("c2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fc_2_x * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("c1");
                listPhysicalQuantity_Values.Add(Math.Round(det.fc_1_y * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("cmin");
                listPhysicalQuantity_Values.Add(Math.Round(det.fs_min * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("k");
                listPhysicalQuantity_Values.Add(Math.Round(det.fk, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("λ");
                listPhysicalQuantity_Values.Add(Math.Round(det.fLambda_53, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("ψ1.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPsi_1_group, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("ψ2");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPsi_2, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("ψ3");
                listPhysicalQuantity_Values.Add(Math.Round(det.fPsi_3, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Ano.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_no_group * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("An.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fA_n_group * fUnitFactor_ComponentArea, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentArea);

                listPhysicalQuantity_Symbols.Add("Nb.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_b_179_group * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Nb.g(a)");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_b_179a_group * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Ncb.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fN_cb_177_group * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Ncb.g");
                listPhysicalQuantity_Values.Add(Math.Round(det.fDesignRatio_17572_group, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }
            else
            {
                // Exception - not defined type
                throw new Exception("Joint type design is not implemented!");
            }

            // Maximum design ratio
            listPhysicalQuantity_Symbols.Add("η max");
            listPhysicalQuantity_Values.Add(Math.Round(calc.fEta_max, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
        }

        private static void DisplayBasicValues(CCalculJoint calc)
        {
            // TODO - refaktorovat nastavenie jednotiek pre zobrazenie

            float fUnitFactor_Force = 0.001f;     // from N to kN
            float fUnitFactor_Moment = 0.001f;    // from Nm to kNm
            float fUnitFactor_Stress = 0.000001f; // from Pa to MPa

            float fUnitFactor_ComponentDimension = 1000f; // m to mm
            float fUnitFactor_ComponentArea = 1000000f; // m^2 to mm^2

            int iNumberOfDecimalPlaces = 3;
            int iNumberOfDecimalPlaces_Factor = 3;
            int iNumberOfDecimalPlaces_DesignRatio = 3;

            string sUnit_Force = "[kN]";
            string sUnit_Moment = "[kNm]";
            string sUnit_Stress = "[MPa]";

            string sUnit_ComponentDimension = "[mm]";
            string sUnit_ComponentArea = "[mm²]";

            string sUnit_Factor = "[-]";
            string sUnit_DesignRatio = "[-]";

            // Internal forces in joint
            listPhysicalQuantity_Symbols.Add("N");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF_AS4600.fN * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            listPhysicalQuantity_Symbols.Add("Vx");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF_AS4600.fV_xu_xx * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            listPhysicalQuantity_Symbols.Add("Vy");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF_AS4600.fV_yv_yy * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            //listPhysicalQuantity_Symbols.Add("T");
            //listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fT * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            //listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("Mx");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF_AS4600.fM_xu_xx * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("My");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF_AS4600.fM_yv_yy * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            // Component properties
            listPhysicalQuantity_Symbols.Add("t1");
            listPhysicalQuantity_Values.Add(Math.Round(calc.ft_1_plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

            listPhysicalQuantity_Symbols.Add("fy.1");
            listPhysicalQuantity_Values.Add(Math.Round(calc.ff_yk_1_plate * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Stress);

            listPhysicalQuantity_Symbols.Add("fu.1");
            listPhysicalQuantity_Values.Add(Math.Round(calc.ff_uk_1_plate * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Stress);

            if (calc.ft_2_crscmainMember > 0.0001f)
            {
                listPhysicalQuantity_Symbols.Add("t2.m1");
                listPhysicalQuantity_Values.Add(Math.Round(calc.ft_2_crscmainMember * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("fy.2.m1");
                listPhysicalQuantity_Values.Add(Math.Round(calc.ff_yk_2_MainMember * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("fu.2.m1");
                listPhysicalQuantity_Values.Add(Math.Round(calc.ff_uk_2_MainMember * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);
            }

            if (calc.ft_2_crscsecMember > 0.0001f)
            {
                listPhysicalQuantity_Symbols.Add("t2.m2");
                listPhysicalQuantity_Values.Add(Math.Round(calc.ft_2_crscsecMember * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                listPhysicalQuantity_Symbols.Add("fy.2.m2");
                listPhysicalQuantity_Values.Add(Math.Round(calc.ff_yk_2_SecondaryMember * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);

                listPhysicalQuantity_Symbols.Add("fu.2.m2");
                listPhysicalQuantity_Values.Add(Math.Round(calc.ff_uk_2_SecondaryMember * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Stress);
            }
        }
    }
}
