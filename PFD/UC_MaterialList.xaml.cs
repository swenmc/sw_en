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
    public partial class UC_MaterialList : UserControl
    {
        DataSet ds;

        List<string> listMemberPrefix = new List<string>(1);
        List<string> listMemberCrScName = new List<string>(1);
        List<int>    listMemberQuantity = new List<int>(1);
        List<string> listMemberMaterialName = new List<string>(1);
        List<double> dlistMemberLength = new List<double>(1);
        List<double> dlistMemberMassPerLength = new List<double>(1);
        List<double> dlistMemberMassPerPiece = new List<double>(1);
        List<double> listMemberTotalLength = new List<double>(1);
        List<double> listMemberTotalMass = new List<double>(1);
        List<double> listMemberTotalPrice = new List<double>(1);

        // For output
        List<string> listMemberLength = new List<string>(1);
        List<string> listMemberMassPerLength = new List<string>(1);
        List<string> listMemberMassPerPiece = new List<string>(1);

        List<string> listPlatePrefix = new List<string>(1);
        List<int> listPlateQuantity = new List<int>(1);
        List<string> listPlateMaterialName = new List<string>(1);
        List<double> dlistPlateWidth_bx = new List<double>(1);
        List<double> dlistPlateHeight_hy = new List<double>(1);
        List<double> dlistPlateThickness_tz = new List<double>(1);
        List<double> dlistPlateArea = new List<double>(1);
        List<double> dlistPlateMassPerPiece = new List<double>(1);
        List<double> listPlateTotalArea = new List<double>(1);
        List<double> listPlateTotalMass = new List<double>(1);
        List<double> listPlateTotalPrice = new List<double>(1);

        // For output
        List<string> listPlateWidth_bx = new List<string>(1);
        List<string> listPlateHeight_hy = new List<string>(1);
        List<string> listPlateThickness_tz = new List<string>(1);
        List<string> listPlateArea = new List<string>(1);
        List<string> listPlateMassPerPiece = new List<string>(1);

        List<string> listConnectorPrefix = new List<string>(1);
        List<int> listConnectorQuantity = new List<int>(1);
        List<string> listConnectorMaterialName = new List<string>(1);
        List<string> listConnectorSize = new List<string>(1);
        List<double> dlistConnectorMassPerPiece = new List<double>(1);
        List<double> listConnectorTotalMass = new List<double>(1);
        List<double> listConnectorTotalPrice = new List<double>(1);

        // For output
        List<string> listConnectorMassPerPiece = new List<string>(1);

        CDatabaseComponents databaseCopm = new CDatabaseComponents();

        public UC_MaterialList(CModel_PFD model)
        {
            InitializeComponent();

            // Clear all lists
            DeleteAllLists();

            int iNumberOfDecimalPlacesLength = 2;
            int iNumberOfDecimalPlacesPlateDim = 3;
            int iNumberOfDecimalPlacesArea = 3;
            int iNumberOfDecimalPlacesVolume = 3;
            int iNumberOfDecimalPlacesMass = 3;
            int iNumberOfDecimalPlacesPrice = 3;

            float fCFS_PricePerKg_Members_Material = 0.3f;     // NZD / kg
            float fCFS_PricePerKg_Plates_Material = 0.4f;      // NZD / kg
            float fCFS_PricePerKg_Members_Manufacture = 0.2f;  // NZD / kg
            float fCFS_PricePerKg_Plates_Manufacture = 0.3f;   // NZD / kg
            float fCFS_PricePerKg_Members_Total = fCFS_PricePerKg_Members_Material + fCFS_PricePerKg_Members_Manufacture;        // NZD / kg
            float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg
            float fTEK_PricePerPiece_Screws_Total = 0.05f;         // NZD / piece

            int iLastItemIndex = 0; // Index of last row for previous cross-section

            // For each cross-section shape / size add one row
            for (int i = 0; i < model.m_arrCrSc.Length; i++)
            {
                List<CMember> assignedMembersList = model.GetListOfMembersWithCrsc(model.m_arrCrSc[i]);
                if (assignedMembersList.Count > 0) // Cross-section is assigned (to the one or more members)
                {
                    List<CMember> ListOfMemberGroups = new List<CMember>();

                    for (int j = 0; j < assignedMembersList.Count; j++) // Each member in the list
                    {
                        if (assignedMembersList[j].BIsDisplayed)
                        {
                            // Define current member properties
                            string sPrefix = databaseCopm.arr_Member_Types_Prefix[(int)assignedMembersList[j].EMemberType, 0];
                            string sCrScName = model.m_arrCrSc[i].Name;
                            int iQuantity = 1;
                            string sMaterialName = model.m_arrCrSc[i].m_Mat.Name;
                            float fLength = assignedMembersList[j].FLength_real;
                            float fMassPerLength = (float)(model.m_arrCrSc[i].A_g * model.m_arrCrSc[i].m_Mat.m_fRho);
                            float fMassPerPiece = fLength * fMassPerLength;
                            float fTotalLength = iQuantity * fLength;
                            float fTotalMass = fTotalLength * fMassPerLength;
                            float fTotalPrice = fTotalMass * fCFS_PricePerKg_Members_Total;

                            bool bMemberwasAdded = false; // Member was added to the group

                            if (j > 0) // If it not first item
                            {
                                for (int k = 0; k < ListOfMemberGroups.Count; k++) // For each group of members check if current member has same prefix and same length as some already created -  // Add Member to the group or create new one
                                {
                                    if ((databaseCopm.arr_Member_Types_Prefix[(int)ListOfMemberGroups[k].EMemberType, 0] == databaseCopm.arr_Member_Types_Prefix[(int)assignedMembersList[j].EMemberType, 0]) &&
                                    (MathF.d_equal(ListOfMemberGroups[k].FLength_real, assignedMembersList[j].FLength_real)))
                                    {
                                        // Add member to the one from already created groups

                                        listMemberQuantity[iLastItemIndex + k] += 1; // Add one member (piece) to the quantity
                                        listMemberTotalLength[iLastItemIndex + k] = Math.Round(listMemberQuantity[iLastItemIndex + k] * dlistMemberLength[iLastItemIndex + k], iNumberOfDecimalPlacesLength); // Recalculate total length of all members in the group
                                        listMemberTotalMass[iLastItemIndex + k] = Math.Round(listMemberTotalLength[iLastItemIndex + k] * dlistMemberMassPerLength[iLastItemIndex + k], iNumberOfDecimalPlacesMass); // Recalculate total weight of all members in the group
                                        listMemberTotalPrice[iLastItemIndex + k] = Math.Round(listMemberTotalMass[iLastItemIndex + k] * fCFS_PricePerKg_Members_Total, iNumberOfDecimalPlacesPrice); // Recalculate total price of all members in the group

                                        bMemberwasAdded = true;
                                    }
                                    // TODO - po pridani pruta by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                                }
                            }

                            if (j == 0 || !bMemberwasAdded) // Create new group (new row) (different length /prefix of member or first item in list of members assigned to the cross-section)
                            {
                                listMemberPrefix.Add(sPrefix);
                                listMemberCrScName.Add(sCrScName);
                                listMemberQuantity.Add(iQuantity);
                                listMemberMaterialName.Add(sMaterialName);
                                dlistMemberLength.Add(Math.Round(fLength, iNumberOfDecimalPlacesLength));
                                dlistMemberMassPerLength.Add(Math.Round(fMassPerLength, iNumberOfDecimalPlacesMass));
                                dlistMemberMassPerPiece.Add(Math.Round(fMassPerPiece, iNumberOfDecimalPlacesMass));
                                listMemberTotalLength.Add(Math.Round(fTotalLength, iNumberOfDecimalPlacesLength));
                                listMemberTotalMass.Add(Math.Round(fTotalMass, iNumberOfDecimalPlacesMass));
                                listMemberTotalPrice.Add(Math.Round(fTotalPrice, iNumberOfDecimalPlacesPrice));

                                // Add first member in the group to the list of member groups
                                ListOfMemberGroups.Add(assignedMembersList[j]);
                            }
                        }
                    }

                    iLastItemIndex += ListOfMemberGroups.Count; // Index of last row for previous cross-section
                }

                //if (model.m_arrCrSc[i].AssignedMembersList.Count > 0) // Cross-section is assigned (to the one or more members)
                //{
                //    List<CMember> ListOfMemberGroups = new List<CMember>();

                //    for (int j = 0; j < model.m_arrCrSc[i].AssignedMembersList.Count; j++) // Each member in the list
                //    {
                //        if (model.m_arrCrSc[i].AssignedMembersList[j].BIsDisplayed)
                //        {
                //            // Define current member properties
                //            string sPrefix = databaseCopm.arr_Member_Types_Prefix[(int)model.m_arrCrSc[i].AssignedMembersList[j].EMemberType, 0];
                //            string sCrScName = model.m_arrCrSc[i].Name;
                //            int iQuantity = 1;
                //            string sMaterialName = model.m_arrCrSc[i].m_Mat.Name;
                //            float fLength = model.m_arrCrSc[i].AssignedMembersList[j].FLength_real;
                //            float fMassPerLength = (float)(model.m_arrCrSc[i].A_g * model.m_arrCrSc[i].m_Mat.m_fRho);
                //            float fMassPerPiece = fLength * fMassPerLength;
                //            float fTotalLength = iQuantity * fLength;
                //            float fTotalMass = fTotalLength * fMassPerLength;
                //            float fTotalPrice = fTotalMass * fCFS_PricePerKg_Members_Total;

                //            bool bMemberwasAdded = false; // Member was added to the group

                //            if (j > 0) // If it not first item
                //            {
                //                for (int k = 0; k < ListOfMemberGroups.Count; k++) // For each group of members check if current member has same prefix and same length as some already created -  // Add Member to the group or create new one
                //                {
                //                    if ((databaseCopm.arr_Member_Types_Prefix[(int)ListOfMemberGroups[k].EMemberType, 0] == databaseCopm.arr_Member_Types_Prefix[(int)model.m_arrCrSc[i].AssignedMembersList[j].EMemberType, 0]) &&
                //                    (MathF.d_equal(ListOfMemberGroups[k].FLength_real, model.m_arrCrSc[i].AssignedMembersList[j].FLength_real)))
                //                    {
                //                        // Add member to the one from already created groups

                //                        listMemberQuantity[iLastItemIndex + k] += 1; // Add one member (piece) to the quantity
                //                        listMemberTotalLength[iLastItemIndex + k] = Math.Round(listMemberQuantity[iLastItemIndex + k] * dlistMemberLength[iLastItemIndex + k], iNumberOfDecimalPlacesLength); // Recalculate total length of all members in the group
                //                        listMemberTotalMass[iLastItemIndex + k] = Math.Round(listMemberTotalLength[iLastItemIndex + k] * dlistMemberMassPerLength[iLastItemIndex + k], iNumberOfDecimalPlacesMass); // Recalculate total weight of all members in the group
                //                        listMemberTotalPrice[iLastItemIndex + k] = Math.Round(listMemberTotalMass[iLastItemIndex + k] * fCFS_PricePerKg_Members_Total, iNumberOfDecimalPlacesPrice); // Recalculate total price of all members in the group

                //                        bMemberwasAdded = true;
                //                    }
                //                    // TODO - po pridani pruta by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                //                }
                //            }

                //            if (j == 0 || !bMemberwasAdded) // Create new group (new row) (different length /prefix of member or first item in list of members assigned to the cross-section)
                //            {
                //                listMemberPrefix.Add(sPrefix);
                //                listMemberCrScName.Add(sCrScName);
                //                listMemberQuantity.Add(iQuantity);
                //                listMemberMaterialName.Add(sMaterialName);
                //                dlistMemberLength.Add(Math.Round(fLength, iNumberOfDecimalPlacesLength));
                //                dlistMemberMassPerLength.Add(Math.Round(fMassPerLength, iNumberOfDecimalPlacesMass));
                //                dlistMemberMassPerPiece.Add(Math.Round(fMassPerPiece, iNumberOfDecimalPlacesMass));
                //                listMemberTotalLength.Add(Math.Round(fTotalLength, iNumberOfDecimalPlacesLength));
                //                listMemberTotalMass.Add(Math.Round(fTotalMass, iNumberOfDecimalPlacesMass));
                //                listMemberTotalPrice.Add(Math.Round(fTotalPrice, iNumberOfDecimalPlacesPrice));

                //                // Add first member in the group to the list of member groups
                //                ListOfMemberGroups.Add(model.m_arrCrSc[i].AssignedMembersList[j]);
                //            }
                //        }
                //    }

                //    iLastItemIndex += ListOfMemberGroups.Count; // Index of last row for previous cross-section
                //}
            }

            // Check Data
            double dTotalMembersLength_Model = 0, dTotalMembersLength_Table = 0;
            double dTotalMembersVolume_Model = 0, dTotalMembersVolume_Table = 0;
            double dTotalMembersMass_Model = 0, dTotalMembersMass_Table = 0;
            double dTotalMembersPrice_Model = 0, dTotalMembersPrice_Table = 0;
            int iTotalMembersNumber_Model = 0, iTotalMembersNumber_Table = 0;

            foreach (CMember member in model.m_arrMembers)
            {
                if (member.BIsDisplayed)
                {
                    dTotalMembersLength_Model += member.FLength_real;
                    dTotalMembersVolume_Model += member.CrScStart.A_g * member.FLength_real;
                    dTotalMembersMass_Model += member.CrScStart.A_g * member.FLength_real * member.CrScStart.m_Mat.m_fRho;
                    dTotalMembersPrice_Model += member.CrScStart.A_g * member.FLength_real * member.CrScStart.m_Mat.m_fRho * fCFS_PricePerKg_Members_Total;
                    iTotalMembersNumber_Model += 1;
                }
            }

            for (int i = 0; i < listMemberPrefix.Count; i++)
            {
                dTotalMembersLength_Table += dlistMemberLength[i] * listMemberQuantity[i];
                //dTotalMembersVolume_Table += member.CrScStart.A_g * dlistMemberLength[i]; // TODO - pridat funkciu, ktora podla nazvu prierezu vrati jeho parametre
                dTotalMembersMass_Table += listMemberTotalMass[i];
                dTotalMembersPrice_Table += listMemberTotalPrice[i];
                iTotalMembersNumber_Table += listMemberQuantity[i];
            }

            dTotalMembersLength_Model = Math.Round(dTotalMembersLength_Model, iNumberOfDecimalPlacesLength);
            dTotalMembersLength_Table = Math.Round(dTotalMembersLength_Table, iNumberOfDecimalPlacesLength);
            dTotalMembersMass_Model = Math.Round(dTotalMembersMass_Model, iNumberOfDecimalPlacesMass);
            dTotalMembersMass_Table = Math.Round(dTotalMembersMass_Table, iNumberOfDecimalPlacesMass);

            if (!MathF.d_equal(dTotalMembersLength_Model, dTotalMembersLength_Table) ||
                !MathF.d_equal(dTotalMembersMass_Model, dTotalMembersMass_Table) ||
                (iTotalMembersNumber_Model != iTotalMembersNumber_Table)) // Error
                MessageBox.Show(
                "Total length of members in model " + dTotalMembersLength_Model + " m" + "\n" +
                "Total length of members in table " + dTotalMembersLength_Table + " m" + "\n" +
                "Total weight of members in model " + dTotalMembersMass_Model + " kg" + "\n" +
                "Total weight of members in table " + dTotalMembersMass_Table + " kg" + "\n" +
                "Total number of members in model " + iTotalMembersNumber_Model + "\n" +
                "Total number of members in table " + iTotalMembersNumber_Table + "\n");

            // Prepare output format (last row is empty)
            for (int i = 0; i < listMemberPrefix.Count; i++)
            {
                // Change output data format
                listMemberLength.Add(dlistMemberLength[i].ToString());
                listMemberMassPerLength.Add(dlistMemberMassPerLength[i].ToString());
                listMemberMassPerPiece.Add(dlistMemberMassPerPiece[i].ToString());
            }

            // Add Sum
            listMemberPrefix.Add("Total:");
            listMemberCrScName.Add("");
            listMemberQuantity.Add(iTotalMembersNumber_Table);
            listMemberMaterialName.Add("");
            listMemberLength.Add(""); // Empty cell
            listMemberMassPerLength.Add(""); // Empty cell
            listMemberMassPerPiece.Add(""); // Empty cell
            listMemberTotalLength.Add(dTotalMembersLength_Table);
            listMemberTotalMass.Add(dTotalMembersMass_Table);
            listMemberTotalPrice.Add(dTotalMembersPrice_Table);

            // Create Table
            DataTable table = new DataTable("Table");
            // Create Table Rows

            table.Columns.Add("Prefix", typeof(String));
            table.Columns.Add("Section", typeof(String));
            table.Columns.Add("Quantity", typeof(Int32));
            table.Columns.Add("Material", typeof(String));
            table.Columns.Add("Length", typeof(String));
            table.Columns.Add("Mass_per_m", typeof(String));
            table.Columns.Add("Mass_per_Piece", typeof(String));
            table.Columns.Add("Total_Length", typeof(Decimal));
            table.Columns.Add("Total_Mass", typeof(Decimal));
            table.Columns.Add("Total_Price", typeof(Decimal));

            // Set Column Caption
            table.Columns["Prefix"].Caption = "Prefix";
            table.Columns["Section"].Caption = "Section";
            table.Columns["Quantity"].Caption = "Quantity";
            table.Columns["Material"].Caption = "Material";
            table.Columns["Length"].Caption = "Length";
            table.Columns["Mass_per_m"].Caption = "Mass_per_m";
            table.Columns["Mass_per_Piece"].Caption = "Mass_per_Piece";
            table.Columns["Total_Length"].Caption = "Total_Length";
            table.Columns["Total_Mass"].Caption = "Total_Mass";
            table.Columns["Total_Price"].Caption = "Total_Price";

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
                    row["Mass_per_m"] = listMemberMassPerLength[i];
                    row["Mass_per_Piece"] = listMemberMassPerPiece[i];
                    row["Total_Length"] = listMemberTotalLength[i];
                    row["Total_Mass"] = listMemberTotalMass[i];
                    row["Total_Price"] = listMemberTotalPrice[i];
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
            Datagrid_Members.Columns[5].Header = "Mass / m";
            Datagrid_Members.Columns[6].Header = "Mass / piece";
            Datagrid_Members.Columns[7].Header = "Total Length";
            Datagrid_Members.Columns[8].Header = "Total Mass";
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
                    float Ft = model.m_arrConnectionJoints[i].m_arrPlates[j].Ft;
                    float fArea = model.m_arrConnectionJoints[i].m_arrPlates[j].PolygonArea();
                    float fMassPerPiece = fArea * Ft * model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.m_fRho;
                    float fTotalArea = iQuantity * fArea;
                    float fTotalMass = iQuantity * fMassPerPiece;
                    float fTotalPrice = fTotalMass * fCFS_PricePerKg_Plates_Total;

                    bool bPlatewasAdded = false; // Plate was added to the group

                    if (i > 0 || (i == 0 && j > 0)) // If it not first item
                    {
                        for (int k = 0; k < ListOfPlateGroups.Count; k++) // For each group of plates check if current plate has same prefix and same dimensions as some already created -  // Add plate to the group or create new one
                        {
                            if (ListOfPlateGroups[k].Name == model.m_arrConnectionJoints[i].m_arrPlates[j].Name &&
                            MathF.d_equal(ListOfPlateGroups[k].fWidth_bx, model.m_arrConnectionJoints[i].m_arrPlates[j].fWidth_bx) &&
                            MathF.d_equal(ListOfPlateGroups[k].fHeight_hy, model.m_arrConnectionJoints[i].m_arrPlates[j].fHeight_hy) &&
                            MathF.d_equal(ListOfPlateGroups[k].Ft, model.m_arrConnectionJoints[i].m_arrPlates[j].Ft) &&
                            MathF.d_equal(ListOfPlateGroups[k].fArea, model.m_arrConnectionJoints[i].m_arrPlates[j].fArea))
                            {
                                // Add plate to the one from already created groups

                                listPlateQuantity[k] += 1; // Add one plate (piece) to the quantity
                                listPlateTotalArea[k] = Math.Round(listPlateQuantity[k] * dlistPlateArea[k], iNumberOfDecimalPlacesArea);
                                listPlateTotalMass[k] = Math.Round(listPlateQuantity[k] * dlistPlateMassPerPiece[k], iNumberOfDecimalPlacesMass); // Recalculate total weight of all plates in the group
                                listPlateTotalPrice[k] = Math.Round(listPlateTotalMass[k] * fCFS_PricePerKg_Plates_Total, iNumberOfDecimalPlacesPrice); // Recalculate total price of all plates in the group

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
                        dlistPlateWidth_bx.Add(Math.Round(fWidth_bx, iNumberOfDecimalPlacesPlateDim));
                        dlistPlateHeight_hy.Add(Math.Round(fHeight_hy, iNumberOfDecimalPlacesPlateDim));
                        dlistPlateThickness_tz.Add(Math.Round(Ft, iNumberOfDecimalPlacesPlateDim));
                        dlistPlateArea.Add(Math.Round(fArea, iNumberOfDecimalPlacesArea));
                        dlistPlateMassPerPiece.Add(Math.Round(fMassPerPiece, iNumberOfDecimalPlacesMass));
                        listPlateTotalArea.Add(Math.Round(fTotalArea, iNumberOfDecimalPlacesArea));
                        listPlateTotalMass.Add(Math.Round(fTotalMass, iNumberOfDecimalPlacesMass));
                        listPlateTotalPrice.Add(Math.Round(fTotalPrice, iNumberOfDecimalPlacesPrice));

                        // Add first plate in the group to the list of plate groups
                        ListOfPlateGroups.Add(model.m_arrConnectionJoints[i].m_arrPlates[j]);
                    }
                }
            }

            // Check Data
            double dTotalPlatesArea_Model = 0, dTotalPlatesArea_Table = 0;
            double dTotalPlatesVolume_Model = 0, dTotalPlatesVolume_Table = 0;
            double dTotalPlatesMass_Model = 0, dTotalPlatesMass_Table = 0;
            double dTotalPlatesPrice_Model = 0, dTotalPlatesPrice_Table = 0;
            int iTotalPlatesNumber_Model = 0, iTotalPlatesNumber_Table = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            {
                // Set plates and connectors data
                foreach (CPlate plate in joint.m_arrPlates)
                {
                    dTotalPlatesArea_Model += plate.fArea;
                    dTotalPlatesVolume_Model += plate.fArea * plate.Ft;
                    dTotalPlatesMass_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho;
                    dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;
                    iTotalPlatesNumber_Model += 1;
                }
            }

            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                dTotalPlatesArea_Table += (dlistPlateArea[i] * listPlateQuantity[i]);
                dTotalPlatesVolume_Table += (dlistPlateArea[i] * dlistPlateThickness_tz[i]);
                dTotalPlatesMass_Table += listPlateTotalMass[i];
                dTotalPlatesPrice_Table += listPlateTotalPrice[i];
                iTotalPlatesNumber_Table += listPlateQuantity[i];
            }

            dTotalPlatesArea_Model = Math.Round(dTotalPlatesArea_Model, iNumberOfDecimalPlacesArea);
            dTotalPlatesVolume_Model = Math.Round(dTotalPlatesVolume_Model, iNumberOfDecimalPlacesVolume);
            dTotalPlatesMass_Model = Math.Round(dTotalPlatesMass_Model, iNumberOfDecimalPlacesMass);
            dTotalPlatesPrice_Model = Math.Round(dTotalPlatesPrice_Model, iNumberOfDecimalPlacesPrice);

            if (!MathF.d_equal(dTotalPlatesArea_Model, dTotalPlatesArea_Table) ||
                !MathF.d_equal(dTotalPlatesMass_Model, dTotalPlatesMass_Table) ||
                (iTotalPlatesNumber_Model != iTotalPlatesNumber_Table)) // Error
                MessageBox.Show(
                "Total area of plates in model " + dTotalPlatesArea_Model + " m^2" + "\n" +
                "Total area of plates in table " + dTotalPlatesArea_Table + " m^2" + "\n" +
                "Total volume of plates in model " + dTotalPlatesVolume_Model + " m^3" + "\n" +
                "Total volume of plates in table " + dTotalPlatesVolume_Table + " m^3" + "\n" +
                "Total weight of plates in model " + dTotalPlatesMass_Model + " kg" + "\n" +
                "Total weight of plates in table " + dTotalPlatesMass_Table + " kg" + "\n" +
                "Total number of plates in model " + iTotalPlatesNumber_Model + "\n" +
                "Total number of plates in table " + iTotalPlatesNumber_Table + "\n");

            // Prepare output format (last row is empty)
            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                // Change output data format
                listPlateWidth_bx.Add(dlistPlateWidth_bx[i].ToString());
                listPlateHeight_hy.Add(dlistPlateHeight_hy[i].ToString());
                listPlateThickness_tz.Add(dlistPlateThickness_tz[i].ToString());
                listPlateArea.Add(dlistPlateArea[i].ToString());
                listPlateMassPerPiece.Add(dlistPlateMassPerPiece[i].ToString());
            }

            // Add Sum
            listPlatePrefix.Add("Total:");
            listPlateQuantity.Add(iTotalPlatesNumber_Table);
            listPlateMaterialName.Add("");
            listPlateWidth_bx.Add(""); // Empty cell
            listPlateHeight_hy.Add(""); // Empty cell
            listPlateThickness_tz.Add(""); // Empty cell
            listPlateArea.Add(""); // Empty cell
            listPlateMassPerPiece.Add(""); // Empty cell
            listPlateTotalArea.Add(dTotalPlatesArea_Table);
            listPlateTotalMass.Add(dTotalPlatesMass_Table);
            listPlateTotalPrice.Add(dTotalPlatesPrice_Table);

            // Create Table
            DataTable table2 = new DataTable("Table2");
            // Create Table Rows

            table2.Columns.Add("Prefix", typeof(String));
            table2.Columns.Add("Quantity", typeof(Int32));
            table2.Columns.Add("Material", typeof(String));
            table2.Columns.Add("Width", typeof(String));
            table2.Columns.Add("Height", typeof(String));
            table2.Columns.Add("Thickness", typeof(String));
            table2.Columns.Add("Area", typeof(String));
            table2.Columns.Add("Mass_per_Piece", typeof(String));
            table2.Columns.Add("Total_Area", typeof(Decimal));
            table2.Columns.Add("Total_Mass", typeof(Decimal));
            table2.Columns.Add("Total_Price", typeof(Decimal));

            // Set Column Caption
            table2.Columns["Prefix"].Caption = "Prefix1";
            table2.Columns["Quantity"].Caption = "Quantity";
            table2.Columns["Material"].Caption = "Material";
            table2.Columns["Width"].Caption = "Width";
            table2.Columns["Height"].Caption = "Height";
            table2.Columns["Thickness"].Caption = "Thickness";
            table2.Columns["Area"].Caption = "Area";
            table2.Columns["Mass_per_Piece"].Caption = "Mass_per_Piece";
            table2.Columns["Total_Area"].Caption = "Total_Area";
            table2.Columns["Total_Mass"].Caption = "Total_Mass";
            table2.Columns["Total_Price"].Caption = "Total_Price";

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
                    row["Mass_per_Piece"] = listPlateMassPerPiece[i];
                    row["Total_Area"] = listPlateTotalArea[i];
                    row["Total_Mass"] = listPlateTotalMass[i];
                    row["Total_Price"] = listPlateTotalPrice[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table2.Rows.Add(row);
            }

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview

            // Connectors

            List<CConnector> ListOfConnectorGroups = new List<CConnector>();

            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
                {
                    if (model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws != null)
                    {
                        for (int k = 0; k < model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws.Length; k++) // For each connector in plate
                        {
                            string sPrefix = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Name;
                            int iQuantity = 1;
                            string sMaterialName = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].m_Mat.Name;
                            int iGauge = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Gauge;
                            float fDiameter = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Diameter_thread;
                            float fLength = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Length;
                            string size = iGauge.ToString() + "g" + " x " + Math.Round(fLength * 1000, 0).ToString(); // Display in [mm] (value * 1000)
                            float fMassPerPiece = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Mass;
                            float fTotalMass = iQuantity * fMassPerPiece;
                            float fTotalPrice = iQuantity * fTEK_PricePerPiece_Screws_Total;

                            bool bConnectorwasAdded = false; // Connector was added to the group

                            if (ListOfConnectorGroups.Count > 0) // If it not first item
                            {
                                for (int m = 0; m < ListOfConnectorGroups.Count; m++) // For each group of connectors check if current connector has same prefix and same dimensions as some already created -  // Add connector to the group or create new one
                                {
                                    if (ListOfConnectorGroups[m].Name == model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Name &&
                                    MathF.d_equal(ListOfConnectorGroups[m].Diameter_thread, model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Diameter_thread) &&
                                    MathF.d_equal(ListOfConnectorGroups[m].Length, model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Length) &&
                                    MathF.d_equal(ListOfConnectorGroups[m].Mass, model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Mass))
                                    {
                                        // Add connector to the one from already created groups

                                        listConnectorQuantity[m] += 1; // Add one connector (piece) to the quantity
                                        listConnectorTotalMass[m] = Math.Round(listConnectorQuantity[m] * dlistConnectorMassPerPiece[m], iNumberOfDecimalPlacesMass); // Recalculate total mass of all connectors in the group
                                        listConnectorTotalPrice[m] = Math.Round(listConnectorQuantity[m] * fTEK_PricePerPiece_Screws_Total, iNumberOfDecimalPlacesPrice); // Recalculate total price of all connectors in the group

                                        bConnectorwasAdded = true;
                                    }
                                    // TODO - po pridani spojovacieho prostriedku by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                                }
                            }

                            if ((i == 0 && j == 0 && k == 0) || !bConnectorwasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
                            {
                                listConnectorPrefix.Add(sPrefix);
                                listConnectorQuantity.Add(iQuantity);
                                listConnectorMaterialName.Add(sMaterialName);
                                listConnectorSize.Add(size);
                                dlistConnectorMassPerPiece.Add(Math.Round(fMassPerPiece, iNumberOfDecimalPlacesMass));
                                listConnectorTotalMass.Add(Math.Round(fTotalMass, iNumberOfDecimalPlacesMass));
                                listConnectorTotalPrice.Add(Math.Round(fTotalPrice, iNumberOfDecimalPlacesPrice));

                                // Add first plate in the group to the list of plate groups
                                ListOfConnectorGroups.Add(model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k]);
                            }
                        }
                    }
                }
            }

            // Check Data
            double dTotalConnectorsMass_Model = 0, dTotalConnectorsMass_Table = 0;
            double dTotalConnectorsPrice_Model = 0, dTotalConnectorsPrice_Table = 0;
            int iTotalConnectorsNumber_Model = 0, iTotalConnectorsNumber_Table = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            {
                foreach (CPlate plate in joint.m_arrPlates)
                {
                    // Set connectors data
                    if (plate.ScrewArrangement.Screws != null)
                    {
                        foreach (CConnector connector in plate.ScrewArrangement.Screws)
                        {
                            dTotalConnectorsMass_Model += connector.Mass;
                            dTotalConnectorsPrice_Model += fTEK_PricePerPiece_Screws_Total;
                            iTotalConnectorsNumber_Model += 1;
                        }
                    }
                }
            }

            for (int i = 0; i < listConnectorPrefix.Count; i++)
            {
                dTotalConnectorsMass_Table += listConnectorTotalMass[i];
                dTotalConnectorsPrice_Table += listConnectorTotalPrice[i];
                iTotalConnectorsNumber_Table += listConnectorQuantity[i];
            }

            dTotalConnectorsMass_Model = Math.Round(dTotalConnectorsMass_Model, iNumberOfDecimalPlacesMass);
            dTotalConnectorsPrice_Model = Math.Round(dTotalConnectorsPrice_Model, iNumberOfDecimalPlacesPrice);

            if (!MathF.d_equal(dTotalConnectorsMass_Model, dTotalConnectorsMass_Table) ||
                    (iTotalConnectorsNumber_Model != iTotalConnectorsNumber_Table)) // Error
                MessageBox.Show(
                "Total weight of connectors in model " + dTotalConnectorsMass_Model + " kg" + "\n" +
                "Total weight of connectors in table " + dTotalConnectorsMass_Table + " kg" + "\n" +
                "Total number of connectors in model " + iTotalConnectorsNumber_Model + "\n" +
                "Total number of connectors in table " + iTotalConnectorsNumber_Table + "\n");

            // Prepare output format (last row is empty)
            for (int i = 0; i < listConnectorPrefix.Count; i++)
            {
                // Change output data format
                listConnectorMassPerPiece.Add(dlistConnectorMassPerPiece[i].ToString());
            }

            // Add Sum
            listConnectorPrefix.Add("Total:");
            listConnectorQuantity.Add(iTotalConnectorsNumber_Table);
            listConnectorMaterialName.Add(""); // Empty cell
            listConnectorSize.Add(""); // Empty cell
            listConnectorMassPerPiece.Add(""); // Empty cell
            listConnectorTotalMass.Add(dTotalConnectorsMass_Table);
            listConnectorTotalPrice.Add(dTotalConnectorsPrice_Table);

            // Create Table
            DataTable table3 = new DataTable("Table3");
            // Create Table Rows

            table3.Columns.Add("Prefix", typeof(String));
            table3.Columns.Add("Quantity", typeof(Int32));
            table3.Columns.Add("Material", typeof(String));
            table3.Columns.Add("Size", typeof(String));
            table3.Columns.Add("Mass_per_Piece", typeof(String));
            table3.Columns.Add("Total_Mass", typeof(Decimal));
            table3.Columns.Add("Total_Price", typeof(Decimal));

            // Set Column Caption
            table3.Columns["Prefix"].Caption = "Prefix1";
            table3.Columns["Quantity"].Caption = "Quantity";
            table3.Columns["Material"].Caption = "Material";
            table3.Columns["Size"].Caption = "Size";
            table3.Columns["Mass_per_Piece"].Caption = "Mass_per_Piece";
            table3.Columns["Total_Mass"].Caption = "Total_Mass";
            table3.Columns["Total_Price"].Caption = "Total_Price";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table3);

            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                DataRow row = table3.NewRow();

                try
                {
                    row["Prefix"] = listConnectorPrefix[i];
                    row["Quantity"] = listConnectorQuantity[i];
                    row["Material"] = listConnectorMaterialName[i];
                    row["Size"] = listConnectorSize[i];
                    row["Mass_per_Piece"] = listConnectorMassPerPiece[i];
                    row["Total_Mass"] = listConnectorTotalMass[i];
                    row["Total_Price"] = listConnectorTotalPrice[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table3.Rows.Add(row);
            }

            Datagrid_Screws.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
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
            dlistMemberLength.Clear();
            dlistMemberMassPerLength.Clear();
            dlistMemberMassPerPiece.Clear();
            listMemberTotalLength.Clear();
            listMemberTotalMass.Clear();

            listPlatePrefix.Clear();
            listPlateQuantity.Clear();
            listPlateMaterialName.Clear();
            dlistPlateWidth_bx.Clear();
            dlistPlateHeight_hy.Clear();
            dlistPlateArea.Clear();
            dlistPlateMassPerPiece.Clear();
            listPlateTotalMass.Clear();

            listConnectorPrefix.Clear();
            listConnectorQuantity.Clear();
            listConnectorMaterialName.Clear();
            listConnectorSize.Clear();
            dlistConnectorMassPerPiece.Clear();
            listConnectorTotalMass.Clear();
            listConnectorTotalPrice.Clear();
        }
    }
}
