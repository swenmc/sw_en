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
using BaseClasses.Results;
using PFD.ViewModels;

namespace PFD
{
    /// <summary>
    /// Interaction logic for MaterialList.xaml
    /// </summary>
    public partial class UC_MaterialList : UserControl
    {
        bool debugging = true;
        DataSet ds;

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
            //DateTime start = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("UC_MaterialList");
            InitializeComponent();

            //System.Diagnostics.Trace.WriteLine("after InitializeComponent: " + (DateTime.Now - start).TotalMilliseconds);

            CMaterialListViewModel vm = new CMaterialListViewModel(model);
            vm.PropertyChanged += MaterialListViewModel_PropertyChanged;
            this.DataContext = vm;

            //System.Diagnostics.Trace.WriteLine("after CMaterialListViewModel: " + (DateTime.Now - start).TotalMilliseconds) ;

            // Clear all lists
            DeleteAllLists();

            int iNumberOfDecimalPlacesPlateDim = 3;
            int iNumberOfDecimalPlacesArea = 5;
            int iNumberOfDecimalPlacesVolume = 5;
            int iNumberOfDecimalPlacesMass = 4;
            int iNumberOfDecimalPlacesPrice = 3;

            const float fCFS_PricePerKg_Plates_Material = 1.698f;    // NZD / kg
            const float fCFS_PricePerKg_Plates_Manufacture = 0.0f;   // NZD / kg

            float fTEK_PricePerPiece_Screws_Total = 0.15f;     // NZD / piece / !!! priblizna cena - nezohladnuje priemer skrutky
            float fAnchor_PricePerLength = 30; // NZD / m - !!! priblizna cena - nezohladnuje priemer tyce
            float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

            // Plates

            List<CPlate> ListOfPlateGroups = new List<CPlate>();
            //System.Diagnostics.Trace.WriteLine("model.m_arrConnectionJoints.Count: " + model.m_arrConnectionJoints.Count);
            int count = 0;
            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                if (model.m_arrConnectionJoints[i].BIsSelectedForMaterialList)
                {
                    count++;
                    for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
                    {
                        // Define current plate properties
                        // Not used - could be used to compare names in database with user-defined in the future

                        //string[] sPlateNames;
                        //ESerieTypePlate ePlateSerieType_FS = model.m_arrConnectionJoints[i].m_arrPlates[j].m_ePlateSerieType_FS;
                        //switch (ePlateSerieType_FS)
                        //{
                        //    case ESerieTypePlate.eSerie_B:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_B_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_L:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_L_Names;
                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_LL:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_LL_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_F:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_F_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_Q:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_Q_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_S:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_S_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_T:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_T_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_X:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_X_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_Y:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_Y_Names;

                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_J:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_J_Names;
                        //            break;
                        //        }
                        //    case ESerieTypePlate.eSerie_K:
                        //        {
                        //            sPlateNames = databaseCopm.arr_Serie_K_Names;
                        //            break;
                        //        }
                        //    default:
                        //        {
                        //            // Not implemented
                        //            break;
                        //        }
                        //}

                        // Nastavime parametre plechu z databazy - TO Ondrej - toto by sa malo diat uz asi pri vytvarani plechov
                        // Nie vsetky plechy budu mat parametre definovane v databaze
                        // !!!! Treba doriesit presne rozmery pri vytvarani plates a zaokruhlovanie
                        //TO Mato - a preco sa to nedeje uz pri vytvarani plechov? Nacitavanie dat z databazy stale je sprostost, lebo to spomaluje


                        //System.Diagnostics.Trace.WriteLine($"before model.m_arrConnectionJoints[{i}].m_arrPlates[{j}].SetParams: " + (DateTime.Now - start).TotalMilliseconds);
                        try
                        {
                            model.m_arrConnectionJoints[i].m_arrPlates[j].SetParams(model.m_arrConnectionJoints[i].m_arrPlates[j].Name, model.m_arrConnectionJoints[i].m_arrPlates[j].m_ePlateSerieType_FS);
                        }
                        catch (Exception ex)
                        {
                            //System.Diagnostics.Trace.WriteLine($"{ex.Message}" + (DateTime.Now - start).TotalMilliseconds);
                        };
                        //System.Diagnostics.Trace.WriteLine($"after model.m_arrConnectionJoints[{i}].m_arrPlates[{j}].SetParams: " + (DateTime.Now - start).TotalMilliseconds);

                        string sPrefix = model.m_arrConnectionJoints[i].m_arrPlates[j].Name;
                        int iQuantity = 1;
                        string sMaterialName = model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.Name;

                        float fWidth_bx = model.m_arrConnectionJoints[i].m_arrPlates[j].Width_bx;
                        float fHeight_hy = model.m_arrConnectionJoints[i].m_arrPlates[j].Height_hy;
                        float Ft = model.m_arrConnectionJoints[i].m_arrPlates[j].Ft;
                        float fArea = model.m_arrConnectionJoints[i].m_arrPlates[j].fArea;
                        float fMassPerPiece = fArea * Ft * model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.m_fRho;
                        float fTotalArea = iQuantity * fArea;
                        float fTotalMass = iQuantity * fMassPerPiece;

                        float fTotalPrice;
                        if (model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD > 0)
                             fTotalPrice = fTotalMass * (float)model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD;
                        else
                            fTotalPrice = fTotalMass * fCFS_PricePerKg_Plates_Total;

                        bool bPlatewasAdded = false; // Plate was added to the group

                        if (i > 0 || (i == 0 && j > 0)) // If it not first item
                        {
                            for (int k = 0; k < ListOfPlateGroups.Count; k++) // For each group of plates check if current plate has same prefix and same dimensions as some already created -  // Add plate to the group or create new one
                            {
                                if (ListOfPlateGroups[k].Name == model.m_arrConnectionJoints[i].m_arrPlates[j].Name &&
                                MathF.d_equal(ListOfPlateGroups[k].Width_bx, model.m_arrConnectionJoints[i].m_arrPlates[j].Width_bx) &&
                                MathF.d_equal(ListOfPlateGroups[k].Height_hy, model.m_arrConnectionJoints[i].m_arrPlates[j].Height_hy) &&
                                MathF.d_equal(ListOfPlateGroups[k].Ft, model.m_arrConnectionJoints[i].m_arrPlates[j].Ft) &&
                                MathF.d_equal(ListOfPlateGroups[k].fArea, model.m_arrConnectionJoints[i].m_arrPlates[j].fArea))
                                {
                                    // Add plate to the one from already created groups

                                    listPlateQuantity[k] += 1; // Add one plate (piece) to the quantity
                                    listPlateTotalArea[k] = Math.Round(listPlateQuantity[k] * dlistPlateArea[k], iNumberOfDecimalPlacesArea);
                                    listPlateTotalMass[k] = Math.Round(listPlateQuantity[k] * dlistPlateMassPerPiece[k], iNumberOfDecimalPlacesMass); // Recalculate total weight of all plates in the group

                                    // Recalculate total price of all plates in the group

                                    if (model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD > 0)
                                        listPlateTotalPrice[k] = Math.Round(listPlateTotalMass[k] * (float)model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD, iNumberOfDecimalPlacesPrice);
                                    else
                                        listPlateTotalPrice[k] = Math.Round(listPlateTotalMass[k] * fCFS_PricePerKg_Plates_Total, iNumberOfDecimalPlacesPrice);

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
            }
            //System.Diagnostics.Trace.WriteLine("Joints SelectedForMaterialList count: " + count);

            // Check Data
            double dTotalPlatesArea_Model = 0, dTotalPlatesArea_Table = 0;
            double dTotalPlatesVolume_Model = 0, dTotalPlatesVolume_Table = 0;
            double dTotalPlatesMass_Model = 0, dTotalPlatesMass_Table = 0;
            double dTotalPlatesPrice_Model = 0, dTotalPlatesPrice_Table = 0;
            int iTotalPlatesNumber_Model = 0, iTotalPlatesNumber_Table = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            {
                if (joint.BIsSelectedForMaterialList)
                {
                    // Set plates and connectors data
                    foreach (CPlate plate in joint.m_arrPlates)
                    {
                        dTotalPlatesArea_Model += plate.fArea;
                        dTotalPlatesVolume_Model += plate.fArea * plate.Ft;
                        dTotalPlatesMass_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho;

                        if (plate.Price_PPKG_NZD > 0)
                            dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * plate.Price_PPKG_NZD;
                        else
                            dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;

                        iTotalPlatesNumber_Model += 1;
                    }
                }
            }

            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                dTotalPlatesArea_Table += (dlistPlateArea[i] * listPlateQuantity[i]);
                dTotalPlatesVolume_Table += (dlistPlateArea[i] * listPlateQuantity[i] * dlistPlateThickness_tz[i]);
                dTotalPlatesMass_Table += listPlateTotalMass[i];
                dTotalPlatesPrice_Table += listPlateTotalPrice[i];
                iTotalPlatesNumber_Table += listPlateQuantity[i];
            }

            dTotalPlatesArea_Model = Math.Round(dTotalPlatesArea_Model, iNumberOfDecimalPlacesArea);
            dTotalPlatesVolume_Model = Math.Round(dTotalPlatesVolume_Model, iNumberOfDecimalPlacesVolume);
            dTotalPlatesMass_Model = Math.Round(dTotalPlatesMass_Model, iNumberOfDecimalPlacesMass);
            dTotalPlatesPrice_Model = Math.Round(dTotalPlatesPrice_Model, iNumberOfDecimalPlacesPrice);

            if (!MathF.d_equal(dTotalPlatesArea_Model, dTotalPlatesArea_Table) ||
                !MathF.d_equal(dTotalPlatesVolume_Model, dTotalPlatesVolume_Table) ||
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

            //System.Diagnostics.Trace.WriteLine("before plates: " + (DateTime.Now - start).TotalMilliseconds);
            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            //System.Diagnostics.Trace.WriteLine("after plates: " + (DateTime.Now - start).TotalMilliseconds);


            // Connectors
            // TASK 422
            // Neviem ci je to stastne ale chcel som usetrit datagridy a dat vsetky spojovacie prostriedky (rozne typy) do jednej tabulky
            // Vsetky by mali mat nejaky prefix, material, popis velkosti (priemer, dlzka), vaha / kus, cena / kus
            // Prosim pozri sa na to a skus to povylepsovat
            // Blok pre screws a pre anchors maju velmi vela spolocneho, mozes to skusit refaktorovat

            // Anchors + screws
            List<CConnector> ListOfConnectorGroups = new List<CConnector>();

            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                if (model.m_arrConnectionJoints[i].BIsSelectedForMaterialList)
                {
                    for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
                    {
                        // Screws
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

                                float fTotalPrice;
                                if (model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Price_PPP_NZD > 0)
                                    fTotalPrice = iQuantity * model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Price_PPP_NZD;
                                else
                                    fTotalPrice = iQuantity * fTEK_PricePerPiece_Screws_Total;

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
                                            listConnectorTotalMass[m] = listConnectorQuantity[m] * dlistConnectorMassPerPiece[m]; // Recalculate total mass of all connectors in the group

                                            if (model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Price_PPP_NZD > 0)
                                                listConnectorTotalPrice[m] = listConnectorQuantity[m] * model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Price_PPP_NZD; // Recalculate total price of all connectors in the group
                                            else
                                                listConnectorTotalPrice[m] = listConnectorQuantity[m] * fTEK_PricePerPiece_Screws_Total;

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
                                    dlistConnectorMassPerPiece.Add(fMassPerPiece);
                                    listConnectorTotalMass.Add(fTotalMass);
                                    listConnectorTotalPrice.Add(fTotalPrice);

                                    // Add first plate in the group to the list of plate groups
                                    ListOfConnectorGroups.Add(model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k]);
                                }
                            }
                        }

                        // Anchors
                        if(model.m_arrConnectionJoints[i].m_arrPlates[j] is CConCom_Plate_B_basic)
                        {
                            CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)model.m_arrConnectionJoints[i].m_arrPlates[j];

                            if(plate.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                            {
                                // TASK 422

                                // TODO Ondrej - doplnit data pre anchors
                                // Refaktorovat anchors a screws

                                // Pre Quantity asi zaviest Count a zjednotit nazov stlpca pre pocet vsade

                                // Size

                                // Pre screws - gauge + dlzka (14g - 38)
                                // Pre anchors  - name + dlzka (M16 - 330)

                                // Prefix | Quantity |     Material     | Size    |   Mass per Piece [kg] | Total Mass [kg] | Unit Price [NZD / piece] | Total Price [NZD]
                                // TEK    |     1515 | Class 3 / 4 / B8 |  14g-38 |                 0.052 |
                                // Anchor |       65 |              8.8 | M16-330 |                 2.241 |

                                for (int k = 0; k < plate.AnchorArrangement.Anchors.Length; k++) // For each connector in plate
                                {
                                    string sPrefix = plate.AnchorArrangement.Anchors[k].Prefix;
                                    int iQuantity = 1;
                                    string sMaterialName = plate.AnchorArrangement.Anchors[k].m_Mat.Name;
                                    string sName = plate.AnchorArrangement.Anchors[k].Name;
                                    float fDiameter = plate.AnchorArrangement.Anchors[k].Diameter_thread;
                                    float fLength = plate.AnchorArrangement.Anchors[k].Length;
                                    string size = sName + " x " + Math.Round(fLength * 1000, 0).ToString(); // Display in [mm] (value * 1000)
                                    float fMassPerPiece = plate.AnchorArrangement.Anchors[k].Mass;
                                    float fTotalMass = iQuantity * fMassPerPiece;

                                    float fTotalPrice;
                                    if (plate.AnchorArrangement.Anchors[k].Price_PPP_NZD > 0)
                                        fTotalPrice = iQuantity * plate.AnchorArrangement.Anchors[k].Price_PPP_NZD;
                                    else
                                        fTotalPrice = iQuantity * (fAnchor_PricePerLength * fLength);

                                    bool bConnectorwasAdded = false; // Connector was added to the group

                                    if (ListOfConnectorGroups.Count > 0) // If it not first item
                                    {
                                        for (int m = 0; m < ListOfConnectorGroups.Count; m++) // For each group of connectors check if current connector has same prefix and same dimensions as some already created -  // Add connector to the group or create new one
                                        {
                                            if (ListOfConnectorGroups[m].Name == plate.AnchorArrangement.Anchors[k].Name &&
                                            MathF.d_equal(ListOfConnectorGroups[m].Diameter_thread, plate.AnchorArrangement.Anchors[k].Diameter_thread) &&
                                            MathF.d_equal(ListOfConnectorGroups[m].Length, plate.AnchorArrangement.Anchors[k].Length) &&
                                            MathF.d_equal(ListOfConnectorGroups[m].Mass, plate.AnchorArrangement.Anchors[k].Mass))
                                            {
                                                // Add connector to the one from already created groups

                                                listConnectorQuantity[m] += 1; // Add one connector (piece) to the quantity
                                                listConnectorTotalMass[m] = listConnectorQuantity[m] * dlistConnectorMassPerPiece[m]; // Recalculate total mass of all connectors in the group

                                                if (plate.AnchorArrangement.Anchors[k].Price_PPP_NZD > 0)
                                                    listConnectorTotalPrice[m] = listConnectorQuantity[m] * plate.AnchorArrangement.Anchors[k].Price_PPP_NZD; // Recalculate total price of all connectors in the group
                                                else
                                                    listConnectorTotalPrice[m] = listConnectorQuantity[m] * (fAnchor_PricePerLength * plate.AnchorArrangement.Anchors[k].Length);

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
                                        dlistConnectorMassPerPiece.Add(fMassPerPiece);
                                        listConnectorTotalMass.Add(fTotalMass);
                                        listConnectorTotalPrice.Add(fTotalPrice);

                                        // Add first plate in the group to the list of plate groups
                                        ListOfConnectorGroups.Add(plate.AnchorArrangement.Anchors[k]);
                                    }
                                }
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
                if (joint.BIsSelectedForMaterialList)
                {
                    foreach (CPlate plate in joint.m_arrPlates)
                    {
                        // Set connectors data
                        if (plate.ScrewArrangement.Screws != null)
                        {
                            foreach (CConnector connector in plate.ScrewArrangement.Screws)
                            {
                                dTotalConnectorsMass_Model += connector.Mass;

                                if (connector.Price_PPP_NZD > 0)
                                    dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
                                else
                                    dTotalConnectorsPrice_Model += fTEK_PricePerPiece_Screws_Total;

                                iTotalConnectorsNumber_Model += 1;
                            }
                        }

                        if (plate is CConCom_Plate_B_basic)
                        {
                            CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

                            if (basePlate.AnchorArrangement.Anchors != null)
                            {
                                foreach (CConnector connector in basePlate.AnchorArrangement.Anchors)
                                {
                                    dTotalConnectorsMass_Model += connector.Mass;

                                    if (connector.Price_PPP_NZD > 0)
                                        dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
                                    else
                                        dTotalConnectorsPrice_Model += (fAnchor_PricePerLength * connector.Length);

                                    iTotalConnectorsNumber_Model += 1;
                                }
                            }
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
                listConnectorMassPerPiece.Add(dlistConnectorMassPerPiece[i].ToString("F2"));
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

            for (int i = 0; i < listConnectorPrefix.Count; i++)
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
            //System.Diagnostics.Trace.WriteLine("before connectors: " + (DateTime.Now - start).TotalMilliseconds);
            Datagrid_Screws.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            //System.Diagnostics.Trace.WriteLine("after connectors: " + (DateTime.Now - start).TotalMilliseconds);
        }

        private void MaterialListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void DeleteAllLists()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

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
