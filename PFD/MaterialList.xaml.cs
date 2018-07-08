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
        List<int> listPlateQuantity = new List<int>(1);
        List<string> listPlateMaterialName = new List<string>(1);
        List<double> listPlateWidth_bx = new List<double>(1);
        List<double> listPlateHeight_hy = new List<double>(1);
        List<double> listPlateThickness_tz = new List<double>(1);
        List<double> listPlateArea = new List<double>(1);
        List<double> listPlateWeightPerPiece = new List<double>(1);
        List<double> listPlateTotalArea = new List<double>(1);
        List<double> listPlateTotalWeight = new List<double>(1);

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
                    List<CMember> ListOfMemberGroups = new List<CMember>();

                    for (int j = 0; j < model.m_arrCrSc[i].AssignedMembersList.Count; j++) // Each member in the list
                    {
                        // Define current member properties
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
                            for (int k = 0; k < ListOfMemberGroups.Count; k++) // For each group of members check if current member has same prefix and same length as some already created -  // Add Member to the group or create new one
                            {
                                if ((databaseCopm.arr_Member_Types_Prefix[(int)ListOfMemberGroups[k].eMemberType_FS, 0] == databaseCopm.arr_Member_Types_Prefix[(int)model.m_arrCrSc[i].AssignedMembersList[j].eMemberType_FS, 0]) &&
                                (MathF.d_equal(ListOfMemberGroups[k].FLength_real, model.m_arrCrSc[i].AssignedMembersList[j].FLength_real)))
                                {
                                    // Add member to the one from already created groups

                                    listMemberQuantity[iLastItemIndex + k] += 1; // Add one member (piece) to the quantity
                                    listMemberTotalLength[iLastItemIndex + k] = Math.Round(listMemberQuantity[iLastItemIndex + k] * listMemberLength[iLastItemIndex + k], iNumberOfDecimalPlaces); // Recalculate total length of all members in the group
                                    listMemberTotalWeight[iLastItemIndex + k] = Math.Round(listMemberTotalLength[iLastItemIndex + k] * listMemberWeightPerLength[iLastItemIndex + k], iNumberOfDecimalPlaces); // Recalculate total weight of all members in the group

                                    bMemberwasAdded = true;
                                }
                                // TOO - po pridani pruta by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                            }
                        }

                        if (j == 0 || !bMemberwasAdded) // Create new group (new row) (different length /prefix of member or first item in list of members assigned to the cross-section)
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
                            ListOfMemberGroups.Add(model.m_arrCrSc[i].AssignedMembersList[j]);
                        }
                    }

                    iLastItemIndex += ListOfMemberGroups.Count; // Index of last row for previous cross-section
                }
            }

            // Check Data
            double dTotalMembersLength_Model = 0, dTotalMembersLength_Table = 0;
            double dTotalMembersVolume_Model = 0, dTotalMembersVolume_Table = 0;
            double dTotalMembersWeight_Model = 0, dTotalMembersWeight_Table = 0;
            int iTotalMembersNumber_Model = 0, iTotalMembersNumber_Table = 0;

            foreach (CMember member in model.m_arrMembers)
            {
                dTotalMembersLength_Model += member.FLength_real;
                dTotalMembersVolume_Model += member.CrScStart.A_g * member.FLength_real;
                dTotalMembersWeight_Model += member.CrScStart.A_g * member.FLength_real * member.CrScStart.m_Mat.m_fRho;
                iTotalMembersNumber_Model += 1;
            }

            for (int i = 0; i < listMemberPrefix.Count; i++)
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
            listMemberPrefix.Add("Total:");
            listMemberCrScName.Add("");
            listMemberQuantity.Add(iTotalMembersNumber_Table);
            listMemberMaterialName.Add("");
            listMemberLength.Add(0); // Empty cell
            listMemberWeightPerLength.Add(0); // Empty cell
            listMemberWeightPerPiece.Add(0); // Empty cell
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
            table.Columns.Add("Weight_per_Piece", typeof(Decimal));
            table.Columns.Add("Total_Length", typeof(Decimal));
            table.Columns.Add("Total_Weight", typeof(Decimal));

            // Set Column Caption
            table.Columns["Prefix"].Caption = "Prefix";
            table.Columns["Section"].Caption = "Section";
            table.Columns["Quantity"].Caption = "Quantity";
            table.Columns["Material"].Caption = "Material";
            table.Columns["Length"].Caption = "Length";
            table.Columns["Weight_per_m"].Caption = "Weight_per_m";
            table.Columns["Weight_per_Piece"].Caption = "Weight_per_Piece";
            table.Columns["Total_Length"].Caption = "Total_Length";
            table.Columns["Total_Weight"].Caption = "Total_Weight";

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
                    row["Weight_per_Piece"] = listMemberWeightPerPiece[i];
                    row["Total_Length"] = listMemberTotalLength[i];
                    row["Total_Weight"] = listMemberTotalWeight[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_Members.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

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

            // Plates

            List<CPlate> ListOfPlateGroups = new List<CPlate>();

            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
                {
                    // Define current plate properties
                    // Not used - could be used to compare names in database with user-defined in the future

                    string[] sPlateNames;
                    ESerieTypePlate ePlateSerieType_FS = model.m_arrConnectionJoints[i].m_arrPlates[j].m_ePlateSerieType_FS;
                    switch (ePlateSerieType_FS)
                    {
                        case ESerieTypePlate.eSerie_B:
                            {
                                sPlateNames = databaseCopm.arr_Serie_B_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_L:
                            {
                                sPlateNames = databaseCopm.arr_Serie_L_Names;
                                break;
                            }
                        case ESerieTypePlate.eSerie_LL:
                            {
                                sPlateNames = databaseCopm.arr_Serie_LL_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_F:
                            {
                                sPlateNames = databaseCopm.arr_Serie_F_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_Q:
                            {
                                sPlateNames = databaseCopm.arr_Serie_Q_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_S:
                            {
                                sPlateNames = databaseCopm.arr_Serie_S_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_T:
                            {
                                sPlateNames = databaseCopm.arr_Serie_T_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_X:
                            {
                                sPlateNames = databaseCopm.arr_Serie_X_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_Y:
                            {
                                sPlateNames = databaseCopm.arr_Serie_Y_Names;

                                break;
                            }
                        case ESerieTypePlate.eSerie_J:
                            {
                                sPlateNames = databaseCopm.arr_Serie_J_Names;
                                break;
                            }
                        case ESerieTypePlate.eSerie_K:
                            {
                                sPlateNames = databaseCopm.arr_Serie_K_Names;
                                break;
                            }
                        default:
                            {
                                // Not implemented
                                break;
                            }
                    }

                    string sPrefix = model.m_arrConnectionJoints[i].m_arrPlates[j].Name;
                    int iQuantity = 1;
                    string sMaterialName = model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.Name;

                    float fWidth_bx = model.m_arrConnectionJoints[i].m_arrPlates[j].fWidth_bx;
                    float fHeight_hy = model.m_arrConnectionJoints[i].m_arrPlates[j].fHeight_hy;
                    float fThickness_tz = model.m_arrConnectionJoints[i].m_arrPlates[j].fThickness_tz;
                    float fArea = model.m_arrConnectionJoints[i].m_arrPlates[j].PolygonArea();
                    float fWeightPerPiece = fArea * fThickness_tz * model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.m_fRho;
                    float fTotalArea = iQuantity * fArea;
                    float fTotalWeight = iQuantity * fWeightPerPiece;

                    bool bPlatewasAdded = false; // Plate was added to the group

                    if (i > 0 || (i == 0 && j > 0)) // If it not first item
                    {
                        for (int k = 0; k < ListOfPlateGroups.Count; k++) // For each group of plates check if current plate has same prefix and same dimensions as some already created -  // Add plate to the group or create new one
                        {
                            if (ListOfPlateGroups[k].Name == model.m_arrConnectionJoints[i].m_arrPlates[j].Name &&
                            MathF.d_equal(ListOfPlateGroups[k].fWidth_bx, model.m_arrConnectionJoints[i].m_arrPlates[j].fWidth_bx) &&
                            MathF.d_equal(ListOfPlateGroups[k].fHeight_hy, model.m_arrConnectionJoints[i].m_arrPlates[j].fHeight_hy) &&
                            MathF.d_equal(ListOfPlateGroups[k].fThickness_tz, model.m_arrConnectionJoints[i].m_arrPlates[j].fThickness_tz) &&
                            MathF.d_equal(ListOfPlateGroups[k].fArea, model.m_arrConnectionJoints[i].m_arrPlates[j].fArea))
                            {
                                // Add member to the one from already created groups

                                listPlateQuantity[k] += 1; // Add one plate (piece) to the quantity
                                listPlateTotalArea[k] = Math.Round(listPlateQuantity[k] * listPlateArea[k], iNumberOfDecimalPlaces);
                                listPlateTotalWeight[k] = Math.Round(listPlateQuantity[k] * listPlateWeightPerPiece[k], iNumberOfDecimalPlaces); // Recalculate total weight of all plates in the group

                                bPlatewasAdded = true;
                            }
                            // TOO - po pridani plechu by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                        }
                    }

                    if ((i == 0 && j == 0) || !bPlatewasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
                    {
                        listPlatePrefix.Add(sPrefix);
                        listPlateQuantity.Add(iQuantity);
                        listPlateMaterialName.Add(sMaterialName);
                        listPlateWidth_bx.Add(Math.Round(fWidth_bx, iNumberOfDecimalPlaces));
                        listPlateHeight_hy.Add(Math.Round(fHeight_hy, iNumberOfDecimalPlaces));
                        listPlateThickness_tz.Add(Math.Round(fThickness_tz, iNumberOfDecimalPlaces));
                        listPlateArea.Add(Math.Round(fArea, iNumberOfDecimalPlaces));
                        listPlateWeightPerPiece.Add(Math.Round(fWeightPerPiece, iNumberOfDecimalPlaces));
                        listPlateTotalArea.Add(Math.Round(fTotalArea, iNumberOfDecimalPlaces));
                        listPlateTotalWeight.Add(Math.Round(fTotalWeight, iNumberOfDecimalPlaces));

                        // Add first plate in the group to the list of plate groups
                        ListOfPlateGroups.Add(model.m_arrConnectionJoints[i].m_arrPlates[j]);
                    }
                }
            }

            // Check Data
            double dTotalPlatesArea_Model = 0, dTotalPlatesArea_Table = 0;
            double dTotalPlatesVolume_Model = 0, dTotalPlatesVolume_Table = 0;
            double dTotalPlatesWeight_Model = 0, dTotalPlatesWeight_Table = 0;
            int iTotalPlatesNumber_Model = 0, iTotalPlatesNumber_Table = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            {
                foreach (CPlate plate in joint.m_arrPlates)
                {
                    dTotalPlatesArea_Model += plate.fArea;
                    dTotalPlatesVolume_Model += plate.fArea * plate.fThickness_tz;
                    dTotalPlatesWeight_Model += plate.fArea * plate.fThickness_tz * plate.m_Mat.m_fRho;
                    iTotalPlatesNumber_Model += 1;
                }
            }

            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                dTotalPlatesArea_Table += (listPlateArea[i] * listPlateQuantity[i]);
                dTotalPlatesVolume_Table += (listPlateArea[i] * listPlateThickness_tz[i]);
                dTotalPlatesWeight_Table += listPlateTotalWeight[i];
                iTotalPlatesNumber_Table += listPlateQuantity[i];
            }

            dTotalPlatesArea_Model = Math.Round(dTotalPlatesArea_Model, iNumberOfDecimalPlaces);
            dTotalPlatesVolume_Model = Math.Round(dTotalPlatesVolume_Model, iNumberOfDecimalPlaces);
            dTotalPlatesWeight_Model = Math.Round(dTotalPlatesWeight_Model, iNumberOfDecimalPlaces);

            if (!MathF.d_equal(dTotalPlatesArea_Model, dTotalPlatesArea_Table) ||
                !MathF.d_equal(dTotalPlatesWeight_Model, dTotalPlatesWeight_Table) ||
                (iTotalPlatesNumber_Model != iTotalPlatesNumber_Table)) // Error
                MessageBox.Show(
                "Total area of plates in model " + dTotalPlatesArea_Model + " m^2" + "\n" +
                "Total area of plates in table " + dTotalPlatesArea_Table + " m^2" + "\n" +
                "Total volume of plates in model " + dTotalPlatesVolume_Model + " m^3" + "\n" +
                "Total volume of plates in table " + dTotalPlatesVolume_Table + " m^3" + "\n" +
                "Total weight of plates in model " + dTotalPlatesWeight_Model + " kg" + "\n" +
                "Total weight of plates in table " + dTotalPlatesWeight_Table + " kg" + "\n" +
                "Total number of plates in model " + iTotalPlatesNumber_Model + "\n" +
                "Total number of plates in table " + iTotalPlatesNumber_Table + "\n");

            // Add Sum
            listPlatePrefix.Add("Total:");
            listPlateQuantity.Add(iTotalPlatesNumber_Table);
            listPlateMaterialName.Add("");
            listPlateWidth_bx.Add(0); // Empty cell
            listPlateHeight_hy.Add(0); // Empty cell
            listPlateThickness_tz.Add(0); // Empty cell
            listPlateArea.Add(0); // Empty cell
            listPlateWeightPerPiece.Add(0); // Empty cell
            listPlateTotalArea.Add(dTotalPlatesArea_Table);
            listPlateTotalWeight.Add(dTotalPlatesWeight_Table);

            // Create Table
            DataTable table2 = new DataTable("Table2");
            // Create Table Rows

            table2.Columns.Add("Prefix", typeof(String));
            table2.Columns.Add("Quantity", typeof(Int32));
            table2.Columns.Add("Material", typeof(String));
            table2.Columns.Add("Width", typeof(Decimal));
            table2.Columns.Add("Height", typeof(Decimal));
            table2.Columns.Add("Thickness", typeof(Decimal));
            table2.Columns.Add("Area", typeof(Decimal));
            table2.Columns.Add("Weight_per_Piece", typeof(Decimal));
            table2.Columns.Add("Total_Area", typeof(Decimal));
            table2.Columns.Add("Total_Weight", typeof(Decimal));

            // Set Column Caption
            table2.Columns["Prefix"].Caption = "Prefix1";
            table2.Columns["Quantity"].Caption = "Quantity";
            table2.Columns["Material"].Caption = "Material";
            table2.Columns["Width"].Caption = "Width";
            table2.Columns["Height"].Caption = "Height";
            table2.Columns["Thickness"].Caption = "Thickness";
            table2.Columns["Area"].Caption = "Area";
            table2.Columns["Weight_per_Piece"].Caption = "Weight_per_Piece";
            table2.Columns["Total_Area"].Caption = "Total_Area";
            table2.Columns["Total_Weight"].Caption = "Total_Weight";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table2);

            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                DataRow row = table2.NewRow();

                try
                {
                    row["Prefix"] = listPlatePrefix[i];
                    row["Quantity"] = listPlateQuantity[i];
                    row["Material"] = listPlateMaterialName[i];
                    row["Width"] = listPlateWidth_bx[i];
                    row["Height"] = listPlateHeight_hy[i];
                    row["Thickness"] = listPlateThickness_tz[i];
                    row["Area"] = listPlateArea[i];
                    row["Weight_per_Piece"] = listPlateWeightPerPiece[i];
                    row["Total_Area"] = listPlateTotalArea[i];
                    row["Total_Weight"] = listPlateTotalWeight[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table2.Rows.Add(row);
            }

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
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
            listPlateMaterialName.Clear();
            listPlateWidth_bx.Clear();
            listPlateHeight_hy.Clear();
            listPlateArea.Clear();
            listPlateWeightPerPiece.Clear();
            listPlateTotalWeight.Clear();
        }
    }
}
