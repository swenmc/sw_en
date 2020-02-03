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
        double dBuildingMass = 0;
        double dBuildingNetPrice_WithoutMargin_WithoutGST = 0;

        DataSet ds;

        //List<string> listPlatePrefix = new List<string>(1);
        //List<int> listPlateQuantity = new List<int>(1);
        //List<string> listPlateMaterialName = new List<string>(1);
        //List<double> dlistPlateWidth_bx = new List<double>(1);
        //List<double> dlistPlateHeight_hy = new List<double>(1);
        //List<double> dlistPlateThickness_tz = new List<double>(1);
        //List<double> dlistPlateArea = new List<double>(1);
        //List<double> dlistPlateMassPerPiece = new List<double>(1);
        //List<double> listPlateTotalArea = new List<double>(1);
        //List<double> listPlateTotalMass = new List<double>(1);
        //List<double> listPlateTotalPrice = new List<double>(1);

        // For output
        //List<string> listPlateWidth_bx = new List<string>(1);
        //List<string> listPlateHeight_hy = new List<string>(1);
        //List<string> listPlateThickness_tz = new List<string>(1);
        //List<string> listPlateArea = new List<string>(1);
        //List<string> listPlateMassPerPiece = new List<string>(1);

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

            //int iNumberOfDecimalPlacesPlateDim = 3;
            //int iNumberOfDecimalPlacesArea = 5;
            //int iNumberOfDecimalPlacesVolume = 5;
            //int iNumberOfDecimalPlacesMass = 4;
            //int iNumberOfDecimalPlacesPrice = 3;

            //const float fCFS_PricePerKg_Plates_Material = 1.698f;    // NZD / kg
            //const float fCFS_PricePerKg_Plates_Manufacture = 0.0f;   // NZD / kg

            float fTEK_PricePerPiece_Screws_Total = 0.15f;     // NZD / piece / !!! priblizna cena - nezohladnuje priemer skrutky
            float fAnchor_PricePerLength = 30; // NZD / m - !!! priblizna cena - nezohladnuje priemer tyce
            //float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

            // Plates
            CreateTablePlates(model);
            
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
            //listPlatePrefix.Clear();
            //listPlateQuantity.Clear();
            //listPlateMaterialName.Clear();
            //dlistPlateWidth_bx.Clear();
            //dlistPlateHeight_hy.Clear();
            //dlistPlateArea.Clear();
            //dlistPlateMassPerPiece.Clear();
            //listPlateTotalMass.Clear();

            listConnectorPrefix.Clear();
            listConnectorQuantity.Clear();
            listConnectorMaterialName.Clear();
            listConnectorSize.Clear();
            dlistConnectorMassPerPiece.Clear();
            listConnectorTotalMass.Clear();
            listConnectorTotalPrice.Clear();
        }

        private void CreateTablePlates(CModel model)
        {
            const float fCFS_PricePerKg_Plates_Material = 1.698f;    // NZD / kg
            const float fCFS_PricePerKg_Plates_Manufacture = 0.0f;   // NZD / kg
            float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

            List<QuotationItem> quotation = new List<QuotationItem>();
            // Plates
            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                if (!joint.BIsSelectedForMaterialList) continue;

                foreach (CPlate plate in joint.m_arrPlates) // For each plate
                {
                    // Nastavime parametre plechu z databazy - TO Ondrej - toto by sa malo diat uz asi pri vytvarani plechov
                    // Nie vsetky plechy budu mat parametre definovane v databaze
                    // !!!! Treba doriesit presne rozmery pri vytvarani plates a zaokruhlovanie

                    #region Base Plate
                    // TO Ondrej Blok1 Plate START
                    // ----------------------------------------------------------------------------------------------------------------------------------------
                    try
                    {
                        plate.SetParams(plate.Name, plate.m_ePlateSerieType_FS);
                    }
                    catch { };

                    QuotationHelper.AddPlateToQuotation(plate, quotation, 1, fCFS_PricePerKg_Plates_Total);


                    // TO Ondrej Blok1 Plate END
                    // ----------------------------------------------------------------------------------------------------------------------------------------
                    #endregion

                    //temp
                    // Anchors - WASHERS
                    // TO Mato - nieco som skusal... chcelo by to asi mat jeden objekt na tieto veci a nie zoznamy kade tade
                    //rovnako je asi problem,ze to nijako negrupujem...ale tak potreboval by som vediet na zaklade coho sa to bude grupovat

                    // To Ondrej - K prvej vete nemam vyhrady. Urob ako sa to ma.
                    // Zgrupovat to treba podla prefixu, ale kedze to este nie je dotiahnute tak porovnavam aj rozmery a plochu uz pridanych plates alebo washers s aktualnym
                    // Vyrobil som 3 bloky kodu, resp. regiony
                    // Jeden pre base plate, jeden washer plate top a jeden pre washer bearing
                    // Funguje to tak ze sa v bloku nastavia parametre aktualnej plate / washer (pocet, rozmery cena, celkove pocty a cena atd)
                    // Potom sa prechadza cyklus cez vsetky uz vytvorene riadky, resp ListOfPlateGroups a porovnava sa ci je aktualny objekt rovnaky ako niektory uz pridany do skupiny
                    // Porovnava sa prefix, rozmery a plocha (ak by sme boli dosledni tak pre plates by sa este malo porovnat screw arrangement, anchor arrangement)
                    // Ak sa zisti ze rovnaky plate/ washer uz bol pridany tak sa aktualizuju celkove parametre, celkovy pocet, celkova plocha, celkova hmotnost
                    // Ak sa zisti ze taky plech v skupine este nie je alebo je to uplne prvy plech v cykle tak sa vyrobi novy zaznam

                    // Dalo by sa to napriklad refaktorovat a urobit z toho jedna funkcia
                    // ListOfPlateGroups by som asi zrusil, lebo tam nemame moznost nastavit pocet plechov v ramci skupiny
                    // Ak tomu rozumiem spravne chces na to pouzit List<PlateView> a odstranit jednotlive zoznamy podla stplcov
                    // Kazdopadne zase sa dostavame k tomu, ze to mame vselijako, niekde samostatne zoznamy pre jednotlive stlpce, inde zoznam objektov s properties, ktore odpovedaju jednemu riadku.

                    if (plate is CConCom_Plate_B_basic)
                    {
                        CConCom_Plate_B_basic plateB = (CConCom_Plate_B_basic)plate;

                        if (plateB.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                        {
                            CAnchor anchor = plateB.AnchorArrangement.Anchors.FirstOrDefault();
                            int anchorsNum = plateB.AnchorArrangement.Anchors.Length;

                            #region Washer Plate Top
                            // TO Ondrej Blok2 Washer Plate Top START
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            // Plate Top Washer
                            try
                            {
                                anchor.WasherPlateTop.SetParams(anchor.WasherPlateTop.Name, anchor.WasherPlateTop.m_ePlateSerieType_FS);
                            }
                            catch { };

                            QuotationHelper.AddPlateToQuotation(anchor.WasherPlateTop, quotation, anchorsNum, fCFS_PricePerKg_Plates_Total);


                            // TO Ondrej Blok2 Washer Plate Top END
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            #endregion

                            #region Washer Bearing 
                            // TO Ondrej Blok3 Washer Bearing START
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            // Bearing Washer
                            try
                            {
                                anchor.WasherBearing.SetParams(anchor.WasherBearing.Name, anchor.WasherBearing.m_ePlateSerieType_FS);
                            }
                            catch { };

                            QuotationHelper.AddPlateToQuotation(anchor.WasherBearing, quotation, anchorsNum, fCFS_PricePerKg_Plates_Total);

                            // TO Ondrej Blok3 Washer Bearing END
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            #endregion
                        }
                    }
                    //end temp
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
                        {
                            dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;

                            if (plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_J || plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_K) // Consider overal rectangular dimensions for knee and apex plates
                            {
                                dTotalPlatesPrice_Model += plate.Width_bx_Stretched * plate.Height_hy_Stretched * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;
                            }
                        }

                        iTotalPlatesNumber_Model += 1;
                    }
                }
            }

            foreach (QuotationItem item in quotation)
            {
                dTotalPlatesArea_Table += item.Area * item.Quantity;
                dTotalPlatesVolume_Table += item.Area * item.Quantity * item.Ft;
                dTotalPlatesMass_Table += item.TotalMass;
                dTotalPlatesPrice_Table += item.TotalPrice;
                iTotalPlatesNumber_Table += item.Quantity;
            }
            dBuildingMass += dTotalPlatesMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalPlatesPrice_Table;

            // Create Table
            DataTable table = new DataTable("Plates");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Prefix.ColumnName, QuotationHelper.colProp_Prefix.DataType);
            table.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            table.Columns.Add(QuotationHelper.colProp_Material.ColumnName, QuotationHelper.colProp_Material.DataType);
            table.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Height_m.ColumnName, QuotationHelper.colProp_Height_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Thickness_m.ColumnName, QuotationHelper.colProp_Thickness_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Area_m2.ColumnName, QuotationHelper.colProp_Area_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Prefix.ColumnName] = item.Prefix;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_Material.ColumnName] = item.MaterialName;
                    row[QuotationHelper.colProp_Width_m.ColumnName] = item.Width_bx.ToString("F2");
                    row[QuotationHelper.colProp_Height_m.ColumnName] = item.Height_hy.ToString("F2");
                    row[QuotationHelper.colProp_Thickness_m.ColumnName] = item.Ft.ToString("F3"); // meters
                    row[QuotationHelper.colProp_Area_m2.ColumnName] = item.Area.ToString("F2");
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = item.TotalArea.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }
            
            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Prefix.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalPlatesNumber_Table;
            row[QuotationHelper.colProp_Material.ColumnName] = "";
            row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            row[QuotationHelper.colProp_Height_m.ColumnName] = "";
            row[QuotationHelper.colProp_Thickness_m.ColumnName] = "";
            row[QuotationHelper.colProp_Area_m2.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = dTotalPlatesArea_Table.ToString("F2");
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalPlatesMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalPlatesPrice_Table.ToString("F2");
            table.Rows.Add(row);

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_Plates.Loaded += Datagrid_Plates_Loaded;
        }

        private void Datagrid_Plates_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Plates);
        }
        private void SetLastRowBold(DataGrid datagrid)
        {
            DataGridRow dtrow = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(datagrid.Items.Count - 1);
            if (dtrow == null) return;
            Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);
            Style newStyle = new Style(dtrow.GetType());

            newStyle.Setters.Add(bold);
            dtrow.Style = newStyle;
        }

    }
}
