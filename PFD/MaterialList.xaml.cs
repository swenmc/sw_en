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
    /// Interaction logic for MaterialList.xaml
    /// </summary>
    public partial class MaterialList : Window
    {
        DataSet ds;

        List<string> listMemberPrefix = new List<string>(1);
        List<string> listMemberCrScName = new List<string>(1);
        List<int>    listMemberQuantity = new List<int>(1);
        List<string> listMemberMaterialName = new List<string>(1);
        List<double>  listMemberLength = new List<double>(1);
        List<double>  listMemberWeightPerLength = new List<double>(1);
        List<double>  listMemberWeightPerPiece = new List<double>(1);
        List<double>  listMemberTotalLength = new List<double>(1);
        List<double>  listMemberTotalWeight = new List<double>(1);

        List<string> listPlatePrefix = new List<string>(1);
        List<string> listPlateQuantity = new List<string>(1);
        List<string> listMaterialName = new List<string>(1);
        List<string> listPlateWidth_bx = new List<string>(1);
        List<string> listPlateHeight_hy = new List<string>(1);
        List<string> listPlateArea = new List<string>(1);
        List<string> listPlateWeightPerPiece = new List<string>(1);
        List<string> listPlateTotalWeight = new List<string>(1);

        DatabaseComponents databaseCopm = new DatabaseComponents();

        public MaterialList()
        {
            InitializeComponent();
        }

        public MaterialList(CModel model)
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            int iNumberOfDecimalPlaces = 3;
            int iLastItemIndex = 0; // Index of last row for previous cross-section

            // For each cross-section shape / size add one row
            for (int i = 0; i < model.m_arrCrSc.Length; i++)
            {
                if (model.m_arrCrSc[i].AssignedMembersList.Count > 0) // Cross-section is assigned (to the one or more members)
                {
                    List<CMember> ListOfGroups = new List<CMember>();

                    for (int j = 0; j < model.m_arrCrSc[i].AssignedMembersList.Count; j++) // Each member in the list
                    {
                        string sPrefix = databaseCopm.arr_Member_Types_Prefix[(int)model.m_arrCrSc[i].AssignedMembersList[j].eMemberType_FS, 0];
                        string sCrScName = model.m_arrCrSc[i].Name;
                        int iQuantity = 1;
                        string sMaterialName = model.m_arrCrSc[i].m_Mat.Name;
                        float fLength = model.m_arrCrSc[i].AssignedMembersList[j].FLength_real;
                        float fWeightPerLength = (float)(model.m_arrCrSc[i].A_g * model.m_arrCrSc[i].m_Mat.m_fRho);
                        float fWeightPerPiece = fLength * fWeightPerLength;
                        float fTotalLength = iQuantity * fLength;
                        float fTotalWeight = fTotalLength * fWeightPerLength;

                        bool bMemberwasAdded = false; // Member was added to the group

                        if (j > 0) // If it not first item
                        {
                            for (int k = 0; k < ListOfGroups.Count; k++) // For each group of members check if current member has same prefix and same length as some already created -  // Add Member to the group or create new one
                            {
                                if ((databaseCopm.arr_Member_Types_Prefix[(int)ListOfGroups[k].eMemberType_FS, 0] == databaseCopm.arr_Member_Types_Prefix[(int)model.m_arrCrSc[i].AssignedMembersList[j].eMemberType_FS, 0]) &&
                                (MathF.d_equal(ListOfGroups[k].FLength_real, model.m_arrCrSc[i].AssignedMembersList[j].FLength_real)))
                                {
                                    // Add member to the one from already created groups

                                    listMemberQuantity[iLastItemIndex + k] += 1; // Add one member (piece) to the quantity
                                    listMemberTotalLength[iLastItemIndex + k] = Math.Round(listMemberQuantity[iLastItemIndex + k] * listMemberLength[iLastItemIndex + k], iNumberOfDecimalPlaces); // Recalculate total length of all members in the group
                                    listMemberTotalWeight[iLastItemIndex + k] = Math.Round(listMemberTotalLength[iLastItemIndex + k] * listMemberWeightPerLength[iLastItemIndex + k], iNumberOfDecimalPlaces); // Recalculate total weight of all members in the group

                                    bMemberwasAdded = true;
                                }
                            }
                        }

                        if(j == 0 || !bMemberwasAdded) // Create new group (new row) (different length /prefix of member or first item in list of members assigned to the cross-section)
                        {
                            listMemberPrefix.Add(sPrefix);
                            listMemberCrScName.Add(sCrScName);
                            listMemberQuantity.Add(iQuantity);
                            listMemberMaterialName.Add(sMaterialName);
                            listMemberLength.Add(Math.Round(fLength, iNumberOfDecimalPlaces));
                            listMemberWeightPerLength.Add(Math.Round(fWeightPerLength, iNumberOfDecimalPlaces));
                            listMemberWeightPerPiece.Add(Math.Round(fWeightPerPiece, iNumberOfDecimalPlaces));
                            listMemberTotalLength.Add(Math.Round(fTotalLength, iNumberOfDecimalPlaces));
                            listMemberTotalWeight.Add(Math.Round(fTotalWeight, iNumberOfDecimalPlaces));

                            // Add first member in the group to the list of member groups
                            ListOfGroups.Add(model.m_arrCrSc[i].AssignedMembersList[j]);
                        }
                    }

                    iLastItemIndex += ListOfGroups.Count; // Index of last row for previous cross-section
                }
            }

            // Check Data
            double dTotalMembersLength_Model = 0, dTotalMembersLength_Table = 0;
            double dTotalMembersVolume_Model = 0, dTotalMembersVolume_Table = 0;
            double dTotalMembersWeight_Model = 0, dTotalMembersWeight_Table = 0;
            int iTotalMembersNumber_Model = 0, iTotalMembersNumber_Table = 0;

            foreach(CMember member in model.m_arrMembers)
            {
                dTotalMembersLength_Model += member.FLength_real;
                dTotalMembersVolume_Model += member.CrScStart.A_g * member.FLength_real;
                dTotalMembersWeight_Model += dTotalMembersVolume_Model * member.CrScStart.m_Mat.m_fRho;
                iTotalMembersNumber_Model += 1;
            }

            for(int i = 0; i < listMemberPrefix.Count; i++)
            {
                dTotalMembersLength_Table += listMemberLength[i] * listMemberQuantity[i];
                //dTotalMembersVolume_Table += member.CrScStart.A_g * listMemberLength[i]; // TODO - pridat funkciu, ktora podla nazvu prierezu vrati jeho parametre
                dTotalMembersWeight_Table += listMemberTotalWeight[i];
                iTotalMembersNumber_Table += listMemberQuantity[i];
            }

            dTotalMembersLength_Model = Math.Round(dTotalMembersLength_Model, iNumberOfDecimalPlaces);
            dTotalMembersLength_Table = Math.Round(dTotalMembersLength_Table, iNumberOfDecimalPlaces);
            dTotalMembersWeight_Model = Math.Round(dTotalMembersWeight_Model, iNumberOfDecimalPlaces);
            dTotalMembersWeight_Table = Math.Round(dTotalMembersWeight_Table, iNumberOfDecimalPlaces);

            if (!MathF.d_equal(dTotalMembersLength_Model, dTotalMembersLength_Table) ||
                !MathF.d_equal(dTotalMembersWeight_Model, dTotalMembersWeight_Table) ||
                (iTotalMembersNumber_Model != iTotalMembersNumber_Table)) // Error
                MessageBox.Show(
                "Total length of members in model " + dTotalMembersLength_Model + " m" + "\n" +
                "Total length of members in table " + dTotalMembersLength_Table + " m" + "\n" +
                "Total weight of members in model " + dTotalMembersWeight_Model + " kg" + "\n" +
                "Total weight of members in table " + dTotalMembersWeight_Table + " kg" + "\n" +
                "Total number of members in model " + iTotalMembersNumber_Model + "\n" +
                "Total number of members in table " + iTotalMembersNumber_Table + "\n");

            // Add Sum
            listMemberPrefix.Add("");
            listMemberCrScName.Add("");
            listMemberQuantity.Add(iTotalMembersNumber_Table);
            listMemberMaterialName.Add("");
            //listMemberLength.Add();
            //listMemberWeightPerLength.Add();
            //listMemberWeightPerPiece.Add();
            listMemberTotalLength.Add(dTotalMembersLength_Table);
            listMemberTotalWeight.Add(dTotalMembersWeight_Table);


            // Create Table
            DataTable table = new DataTable("Table");
            // Create Table Rows

            table.Columns.Add("Prefix", typeof(String));
            table.Columns.Add("Section", typeof(String));
            table.Columns.Add("Quantity", typeof(Int32));
            table.Columns.Add("Material", typeof(String));
            table.Columns.Add("Length", typeof(Decimal));
            table.Columns.Add("Weight_per_m", typeof(Decimal));
            table.Columns.Add("Weight_per_piece", typeof(Decimal));
            table.Columns.Add("Total_Length", typeof(Decimal));
            table.Columns.Add("Total_Weight", typeof(Decimal));

            // Set Column Caption
            table.Columns["Prefix"].Caption = "Prefix1";
            table.Columns["Section"].Caption = "Section1";
            table.Columns["Quantity"].Caption = "Quantity";
            table.Columns["Material"].Caption = "Material";
            table.Columns["Length"].Caption = "Length";
            table.Columns["Weight_per_m"].Caption = "Weight / m";
            table.Columns["Weight_per_piece"].Caption = "Weight / piece";
            table.Columns["Total_Length"].Caption = "Total Length";
            table.Columns["Total_Weight"].Caption = "Total Weight";

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
                    row["Section"] = listMemberCrScName[i];
                    row["Quantity"] = listMemberQuantity[i];
                    row["Material"] = listMemberMaterialName[i];
                    row["Length"] = listMemberLength[i];
                    row["Weight_per_m"] = listMemberWeightPerLength[i];
                    row["Weight_per_piece"] = listMemberWeightPerPiece[i];
                    row["Total_Length"] = listMemberTotalLength[i];
                    row["Total_Weight"] = listMemberTotalWeight[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_Members.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            // TODO - temporary - tento column nema byt v tabulke, ale inak sa do Datagrid_Members.ItemsSource neprevezme pocet columns

            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "ID", Width = 30 });

            /*
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Prefix" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Section" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Quantity" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Material" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Length" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Weight / m" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Weight / piece" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Total Length" });
            Datagrid_Members.Columns.Add(new DataGridTextColumn() { Header = "Total Weight" });
            */

            // Set Column Header
            /*
            Datagrid_Members.Columns[0].Header = "Prefix";
            Datagrid_Members.Columns[1].Header = "Section";
            Datagrid_Members.Columns[2].Header = "Quantity";
            Datagrid_Members.Columns[3].Header = "Material";
            Datagrid_Members.Columns[4].Header = "Length";
            Datagrid_Members.Columns[5].Header = "Weight / m";
            Datagrid_Members.Columns[6].Header = "Weight / piece";
            Datagrid_Members.Columns[7].Header = "Total Length";
            Datagrid_Members.Columns[8].Header = "Total Weight";
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
            Datagrid_Members.Columns[8].Width = 100;
            */



        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_Members.ItemsSource = null;
            Datagrid_Members.Items.Clear();
            Datagrid_Members.Items.Refresh();

            Datagrid_Plates.ItemsSource = null;
            Datagrid_Plates.Items.Clear();
            Datagrid_Plates.Items.Refresh();

            Datagrid_Screws.ItemsSource = null;
            Datagrid_Screws.Items.Clear();
            Datagrid_Screws.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listMemberPrefix.Clear();
            listMemberCrScName.Clear();
            listMemberQuantity.Clear();
            listMemberMaterialName.Clear();
            listMemberLength.Clear();
            listMemberWeightPerLength.Clear();
            listMemberWeightPerPiece.Clear();
            listMemberTotalLength.Clear();
            listMemberTotalWeight.Clear();

            listPlatePrefix.Clear();
            listPlateQuantity.Clear();
            listMaterialName.Clear();
            listPlateWidth_bx.Clear();
            listPlateHeight_hy.Clear();
            listPlateArea.Clear();
            listPlateWeightPerPiece.Clear();
            listPlateTotalWeight.Clear();
        }
    }
}
