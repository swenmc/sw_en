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
        static List<string> listPhysicalQuantity_Values = new List<string>(4);        // hodnoty danych premennych
        static List<string> listPhysicalQuantity_Units = new List<string>(4);       // jednotky danych premennych
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
            listPhysicalQuantity_Symbols.Clear();
            listPhysicalQuantity_Values.Clear();
            listPhysicalQuantity_Units.Clear();
        }

        private static void SetResultsDetailsFor_ULS(CCalculJoint calc)
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

            // Internal forces in joint
            listPhysicalQuantity_Symbols.Add("N");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fN * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            listPhysicalQuantity_Symbols.Add("Vx");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fV_yu * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            listPhysicalQuantity_Symbols.Add("Vy");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fV_zv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Force);

            //listPhysicalQuantity_Symbols.Add("T");
            //listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fT * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            //listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("Mx");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fM_yu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_Moment);

            listPhysicalQuantity_Symbols.Add("My");
            listPhysicalQuantity_Values.Add(Math.Round(calc.sDIF.fM_zv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
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

            // Maximum design ratio
            listPhysicalQuantity_Symbols.Add("η max");
            listPhysicalQuantity_Values.Add(Math.Round(calc.fEta_max, iNumberOfDecimalPlaces).ToString());
            listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

            // Knee and Apex Joint
            if (calc.joint is CConnectionJoint_A001 || calc.joint is CConnectionJoint_B001)
            {
                float fPhi_Plate = 0;
                float fA_n_plate = 0;
                float fN_t_plate = 0;
                float fEta_N_t_5423_plate = 0;
                float fA_vn_yv_plate = 0;
                float fV_y_yv_plate = 0;
                float fEta_V_yv_3341_plate = 0;
                float fM_xu_resitance_plate = 0;
                float fEta_Mb_plate = 0;

                // Shear in connection
                float fPhi_shear_screw = 0;
                float fVb_MainMember = 0;
                float fVb_SecondaryMember = 0;
                int iNumberOfScrewsInShear = 0;
                float fEta_MainMember = 0;
                float fEta_SecondaryMember = 0;

                // Plastic Design
                float fMb_MainMember_oneside_plastic = 0;
                float fMb_SecondaryMember_oneside_plastic = 0;
                float fEta_Mb_MainMember_oneside_plastic = 0;
                float fEta_Mb_SecondaryMember_oneside_plastic = 0;

                // Elastic Design
                float fV_asterix_b_max_screw_Mxu = 0;
                float fV_asterix_b_max_screw_Vyv = 0;
                float fV_asterix_b_max_screw_N = 0;
                float fV_asterix_b_max_screw = 0;

                float fEta_Vb_5424_MainMember = 0;
                float fEta_Vb_5424_SecondaryMember = 0;

                listPhysicalQuantity_Symbols.Add("Φplate");
                listPhysicalQuantity_Values.Add(Math.Round(fPhi_Plate, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("An.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fA_n_plate * fUnitFactor_CrSc_Area, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_CrSc_Area);

                listPhysicalQuantity_Symbols.Add("Nt.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fN_t_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Nt.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_N_t_5423_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("Av.y.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fA_vn_yv_plate * fUnitFactor_CrSc_Area, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_CrSc_Area);

                listPhysicalQuantity_Symbols.Add("Vy.y.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fV_y_yv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vy.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_V_yv_3341_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("M.b.x.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fM_xu_resitance_plate * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η Mx.plate");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_Mb_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Shear in connection
                listPhysicalQuantity_Symbols.Add("Φv.screw");
                listPhysicalQuantity_Values.Add(Math.Round(fPhi_shear_screw, iNumberOfDecimalPlaces_Factor).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Factor);

                listPhysicalQuantity_Symbols.Add("Vb.screw.m1");
                listPhysicalQuantity_Values.Add(Math.Round(fVb_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Vb.screw.m2");
                listPhysicalQuantity_Values.Add(Math.Round(fVb_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("Number of screws in shear");
                listPhysicalQuantity_Values.Add(iNumberOfScrewsInShear.ToString());
                listPhysicalQuantity_Units.Add("");

                listPhysicalQuantity_Symbols.Add("η screw.m1");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η screw.m2");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Plastic Design
                listPhysicalQuantity_Symbols.Add("M.b.x.m1");
                listPhysicalQuantity_Values.Add(Math.Round(fMb_MainMember_oneside_plastic * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("M.b.x.m2");
                listPhysicalQuantity_Values.Add(Math.Round(fMb_SecondaryMember_oneside_plastic * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Moment);

                listPhysicalQuantity_Symbols.Add("η Mb.m1");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_Mb_MainMember_oneside_plastic, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Mb.m2");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_Mb_SecondaryMember_oneside_plastic, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                // Elastic Design
                listPhysicalQuantity_Symbols.Add("V*b.screw.Mx");
                listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_max_screw_Mxu * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.Vy");
                listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_max_screw_Vyv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.N");
                listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_max_screw_N * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("V*b.screw.res");
                listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_max_screw * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                listPhysicalQuantity_Units.Add(sUnit_Force);

                listPhysicalQuantity_Symbols.Add("η Vb.m1");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_Vb_5424_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                listPhysicalQuantity_Symbols.Add("η Vb.m2");
                listPhysicalQuantity_Values.Add(Math.Round(fEta_Vb_5424_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
            }
            else if (calc.joint.m_SecondaryMembers != null)
            {
                // Purlins, girts, ...
                if (calc.joint is CConnectionJoint_T001 || calc.joint is CConnectionJoint_T002 || calc.joint is CConnectionJoint_T003)
                {
                    // 5.4.3 Screwed connections in tension
                    // 5.4.3.2 Pull-out and pull-over (pull-through)
                    int iNumberOfScrewsInTension = 0;
                    float fPhi_N_screw = 0;
                    float fN_t_5432_MainMember = 0;
                    float fEta_N_t_5432_MainMember = 0;

                    // Pripoj plechu k hlavnemu prutu
                    // Tension and shear
                    float fPhi_shear_Vb_Nov = 0;
                    float fC_for5434_MainMember = 0;
                    float fV_b_for5434_MainMember = 0;
                    float fd_w_for5434_plate = 0;
                    float fN_ov_for5434_plate = 0;
                    float fV_asterix_b_for5434_MainMember = 0;
                    float fEta_5434_MainMember = 0;

                    float fV_b_for5435_MainMember = 0;
                    float fN_ou_for5435_MainMember = 0;
                    float fV_asterix_b_for5435_MainMember = 0;
                    float fEta_5435_MainMember = 0;

                    // 5.4.2.5 Connection shear as limited by end distance
                    float fe_Plate = 0;
                    float fV_asterix_fv_plate = 0;
                    float fV_fv_Plate = 0;
                    float fEta_V_fv_5425_Plate = 0;

                    // 5.4.2.6 Screws in shear
                    float fV_w_nom_screw_5426 = 0;
                    float fEta_V_w_5426 = 0;

                    // 5.4.3.3 Screws in tension
                    float fPhi_N_t_screw = 0;
                    float fN_t_nom_screw_5433 = 0;
                    float fEta_N_t_screw_5433 = 0;

                    // 5.4.3.6 Screws subject to combined shear and tension
                    float fEta_V_N_t_screw_5436 = 0;

                    // Plate design
                    float fPhi_plate = 0;
                    float fA_n_plate = 0;
                    float fN_t_plate = 0;
                    float fEta_N_t_5423_plate = 0;

                    float fA_vn_yv_plate = 0;
                    float fV_y_yv_plate = 0;
                    float fEta_V_yv_3341_plate = 0;

                    // Pripoj plechu sekundarneho pruta
                    int iNumberOfScrewsInConnectionOfSecondaryMember = 0;

                    float fV_asterix_b_SecondaryMember = 0;
                    float fVb_SecondaryMember = 0;
                    float fEta_Vb_5424_SecondaryMember = 0;

                    float fe_SecondaryMember = 0;
                    float fV_asterix_fv_SecondaryMember = 0;
                    float fV_fv_SecondaryMember = 0;
                    float fEta_V_fv_5425_SecondaryMember = 0;

                    // 5.4.3 Screwed connections in tension
                    // 5.4.3.2 Pull-out and pull-over (pull-through)

                    listPhysicalQuantity_Symbols.Add("Number of screws in tension");
                    listPhysicalQuantity_Values.Add(iNumberOfScrewsInTension.ToString());
                    listPhysicalQuantity_Units.Add("");

                    listPhysicalQuantity_Symbols.Add("Φn.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fPhi_N_screw, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Nt.screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fN_t_5432_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Nt.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_N_t_5432_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Pripoj plechu k hlavnemu prutu
                    // Tension and shear
                    listPhysicalQuantity_Symbols.Add("Φv.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fPhi_shear_Vb_Nov, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("C.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fC_for5434_MainMember, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Vb.screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_b_for5434_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("dw");
                    listPhysicalQuantity_Values.Add(Math.Round(fd_w_for5434_plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("Nov.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fN_ov_for5434_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("V*b.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_for5434_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_5434_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("Vb.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_b_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Nou.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fN_ou_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("V*b.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_for5435_MainMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.m1");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_5435_MainMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.2.5 Connection shear as limited by end distance
                    listPhysicalQuantity_Symbols.Add("e");
                    listPhysicalQuantity_Values.Add(Math.Round(fe_Plate * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("V*fv.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_fv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vfv.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_fv_Plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_V_fv_5425_Plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.2.6 Screws in shear
                    listPhysicalQuantity_Symbols.Add("Vw.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_w_nom_screw_5426 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_V_w_5426, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // 5.4.3.3 Screws in tension
                    listPhysicalQuantity_Symbols.Add("Φn.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fPhi_N_t_screw, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("Nt.screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fN_t_nom_screw_5433 * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_N_t_screw_5433, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("η screw");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_V_N_t_screw_5436, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Plate design
                    listPhysicalQuantity_Symbols.Add("Φplate");
                    listPhysicalQuantity_Values.Add(Math.Round(fPhi_plate, iNumberOfDecimalPlaces_Factor).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Factor);

                    listPhysicalQuantity_Symbols.Add("An.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fA_n_plate * fUnitFactor_CrSc_Area, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_CrSc_Area);

                    listPhysicalQuantity_Symbols.Add("Nt.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fN_t_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Nt.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_N_t_5423_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("Avn.y.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fA_vn_yv_plate * fUnitFactor_CrSc_Area, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_CrSc_Area);

                    listPhysicalQuantity_Symbols.Add("Vy.y.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_y_yv_plate * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vy.plate");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_V_yv_3341_plate, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    // Pripoj plechu sekundarneho pruta
                    listPhysicalQuantity_Symbols.Add("Number of screws in connection m2");
                    listPhysicalQuantity_Values.Add(iNumberOfScrewsInConnectionOfSecondaryMember.ToString());
                    listPhysicalQuantity_Units.Add("");

                    listPhysicalQuantity_Symbols.Add("V*b.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_b_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vb.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fVb_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vb.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_Vb_5424_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);

                    listPhysicalQuantity_Symbols.Add("e.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fe_SecondaryMember * fUnitFactor_ComponentDimension, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_ComponentDimension);

                    listPhysicalQuantity_Symbols.Add("V*fv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_asterix_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("Vfv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fV_fv_SecondaryMember * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_Force);

                    listPhysicalQuantity_Symbols.Add("η Vfv.m2");
                    listPhysicalQuantity_Values.Add(Math.Round(fEta_V_fv_5425_SecondaryMember, iNumberOfDecimalPlaces_DesignRatio).ToString());
                    listPhysicalQuantity_Units.Add(sUnit_DesignRatio);
                }
                else if (calc.joint is CConnectionJoint_S001) // Front / back column connection to the main rafter
                {
                    //sJointDesignDetails_FrontOrBackColumnToMainRafterJoint sDesignDetails;
                }
                else
                {
                    // Exception - not defined type
                    throw new Exception("Joint type design is not implemented!");
                }
            }
            else if (calc.joint is CConnectionJoint_TA01 || calc.joint is CConnectionJoint_TB01)
            {
                //sJointDesignDetails_BaseJoint sDesignDetails;
            }
            else
            {
                // Exception - not defined type
                throw new Exception("Joint type design is not implemented!");
            }
        }
    }
}
