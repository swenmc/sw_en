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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MATH;
using DATABASE;
using DATABASE.DTO;
using BaseClasses;
using System.Data;
using CRSC;
using BaseClasses.Helpers;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_Quotation.xaml
    /// </summary>
    public partial class UC_Quotation : UserControl
    {
        double dBuildingMass = 0;
        double dBuildingPrice_WithoutGST = 0;

        public UC_Quotation(CPFDViewModel vm)
        {
            InitializeComponent();

            CModel model = vm.Model;

            List<Point> fWallDefinitionPoints_Left = new List<Point>(4) { new Point(0, 0), new Point(model.fL_tot, 0), new Point(model.fL_tot, model.fH1_frame), new Point(0, model.fH1_frame) };
            List<Point> fWallDefinitionPoints_Front = new List<Point>(5) { new Point(0, 0), new Point(model.fW_frame, 0), new Point(model.fW_frame, model.fH1_frame), new Point(0.5 * model.fW_frame, model.fH2_frame), new Point(0, model.fH1_frame) };

            // TODO Ondrej - refaktoring - funckia CreateTableCladding

            float fWallArea_Left = 0; float fWallArea_Right = 0;
            if (vm.ComponentList[(int)EMemberType_FS_Position.Girt].Generate == true)
            {
                fWallArea_Left = Geom2D.PolygonArea(fWallDefinitionPoints_Left.ToArray());
                fWallArea_Right = fWallArea_Left;
            }

            float fWallArea_Front = 0;
            if (vm.ComponentList[(int)EMemberType_FS_Position.GirtFrontSide].Generate == true)
                fWallArea_Front = Geom2D.PolygonArea(fWallDefinitionPoints_Front.ToArray());

            float fWallArea_Back = 0;
            if (vm.ComponentList[(int)EMemberType_FS_Position.GirtBackSide].Generate == true)
                fWallArea_Back = Geom2D.PolygonArea(fWallDefinitionPoints_Front.ToArray());

            float fBuildingArea_Gross = model.fW_frame * model.fL_tot;
            float fBuildingVolume_Gross = Geom2D.PolygonArea(fWallDefinitionPoints_Front.ToArray()) * model.fL_tot;

            // DG 1
            // Members
            CreateTableMembers(model);

            // DG 2
            // Plates
            CreateTablePlates(model);

            // TODO - dopracovat apex brace plates

            // DG 3
            // Screws
            // Bolts
            // Anchors

            // TODO Ondrej - zobrazit zoznam screws a anchors - vid Material List
            // Treba spravne naformatovat stlpce a pocty desatinnych miest
            // Pridat vsetky anchors zo vsetkych base plate "plate typu B" Plate AnchorArrangement

            CreateTableConnectors(model);

            // DG 4
            // Bolt Nuts

            // TODO

            // DG 5
            // Washers

            // TODO

            // DG 7
            // Doors and windows
            float fTotalAreaOfOpennings = 0;

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            float fPADoorTrimmerFlashing_TotalLength = 0;
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

            List<COpeningProperties> listOfOpenings = new List<COpeningProperties>();

            foreach (DoorProperties dp in vm.DoorBlocksProperties)
            {
                fTotalAreaOfOpennings += dp.fDoorsWidth * dp.fDoorsHeight;

                if (dp.sDoorType == "Roller Door")
                {
                    fRollerDoorTrimmerFlashing_TotalLength += (dp.fDoorsHeight * 2);
                    fRollerDoorLintelFlashing_TotalLength += dp.fDoorsWidth;
                    fRollerDoorLintelCapFlashing_TotalLength = dp.fDoorsWidth;
                }
                else
                {
                    fPADoorTrimmerFlashing_TotalLength += (dp.fDoorsHeight * 2);
                    fPADoorLintelFlashing_TotalLength += dp.fDoorsWidth;
                }

                listOfOpenings.Add(new COpeningProperties(dp.sDoorType, dp.fDoorsWidth, dp.fDoorsHeight));
            }

            foreach (WindowProperties wp in vm.WindowBlocksProperties)
            {
                fTotalAreaOfOpennings += wp.fWindowsWidth * wp.fWindowsHeight;

                fWindowFlashing_TotalLength += (2 * wp.fWindowsWidth + 2 * wp.fWindowsHeight);

                listOfOpenings.Add(new COpeningProperties("Window", wp.fWindowsWidth, wp.fWindowsHeight));
            }

            // TODO Ondrej

            // Refaktorovat kody
            // Skus to popozerat a pripadne nejako zautomatizovat
            // V principe mame 2 typy poloziek
            // 1 - definovane dlzkou (flashings, gutters, mozno sa da uvazovat aj fibreglass)
            // 2 - definovene plochou (doors, windows, roof netting)

            List<COpeningProperties> groupedOpenings = new List<COpeningProperties>();
            foreach (COpeningProperties op in listOfOpenings)
            {
                if (groupedOpenings.Contains(op))
                {
                    COpeningProperties grOP = groupedOpenings[groupedOpenings.IndexOf(op)];
                    //grOP.Area += op.Area;
                    //grOP.Perimeter += op.Perimeter;
                    grOP.Count++;
                }
                else groupedOpenings.Add(op);
            }

            CreateTableDoorsAndWindows(groupedOpenings);

            // DG 9
            // Cladding
            float fWallArea_Total = fWallArea_Left + fWallArea_Right + fWallArea_Front + fWallArea_Back;
            float fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame - model.fH1_frame) + MathF.Pow2(0.5f * model.fW_frame)); // Dlzka hrany strechy

            float fRoofArea = 0;
            if (vm.ComponentList[(int)EMemberType_FS_Position.Purlin].Generate == true)
                fRoofArea = 2 * fRoofSideLength * model.fL_tot;

            float fFibreGlassArea_Roof = vm.FibreglassAreaRoof / 100f * fRoofArea; // Priesvitna cast strechy TODO Percento pre fibre glass zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul
            float fFibreGlassArea_Walls = vm.FibreglassAreaWall / 100f * fWallArea_Total; // Priesvitna cast strechy TODO Percento zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul

            CreateTableCladding(vm,
                fWallArea_Total,
                fTotalAreaOfOpennings,
                fFibreGlassArea_Walls,
                fRoofArea,
                fFibreGlassArea_Roof
               );

            // DG 10
            // Gutters
            CreateTableGutters(model);

            // DG 11
            // FibreGlass
            CreateTableFibreglass(vm, fFibreGlassArea_Roof, fFibreGlassArea_Walls);

            // DG 12
            // Roof Netting
            CreateTableRoofNetting(fRoofArea);

            // DG 13
            // Flashing and Packers
            CreateTableFlashing(model,
                fRoofSideLength,
                fRollerDoorTrimmerFlashing_TotalLength,
                fRollerDoorLintelFlashing_TotalLength,
                fRollerDoorLintelCapFlashing_TotalLength,
                fPADoorTrimmerFlashing_TotalLength,
                fPADoorLintelFlashing_TotalLength,
                fWindowFlashing_TotalLength);

            // Vysledne hodnoty a sumy spolu s plochou, objemom a celkovou cenou zobrazime v tabe
            double buildingPrice_PSM = dBuildingPrice_WithoutGST / fBuildingArea_Gross;
            double buildingPrice_PCM = dBuildingPrice_WithoutGST / fBuildingVolume_Gross;
            double buildingPrice_PPKG = dBuildingPrice_WithoutGST / dBuildingMass;

            double dGST_Percentage = 15f; // TODO - urobit nastavitelne v GUI ???
            double dGST_Absolute = dGST_Percentage / 100f * dBuildingPrice_WithoutGST;
            double dTotalBuildingPrice_IncludingGST = dBuildingPrice_WithoutGST + dGST_Absolute;

            // Vypiseme celkovu cenu a dalsie parametre
            SubTotalPrice.Text = dBuildingPrice_WithoutGST.ToString("F2");
            GST_Absolute.Text = dGST_Absolute.ToString("F2");
            TotalPrice.Text = dTotalBuildingPrice_IncludingGST.ToString("F2");

            BuildingArea.Text = fBuildingArea_Gross.ToString("F2");
            BuildingVolume.Text = fBuildingVolume_Gross.ToString("F2");
            BuildingMass.Text = dBuildingMass.ToString("F2");

            UnitPricePerBuildingArea.Text = buildingPrice_PSM.ToString("F2");
            UnitPricePerBuildingVolume.Text = buildingPrice_PCM.ToString("F2");
            UnitPricePerBuildingMass.Text = buildingPrice_PPKG.ToString("F2");

            // TODO - for later

            // DG 14
            // Footing pads

            // DG 15
            // Reinforcement

            // DG 16
            // Floor Slab

            // DG 17
            // Floor Slab Mesh

            // DG 18
            // Perimeters

            // DG 19
            // Perimeter Reinforcement

            // DG 20
            // Rebates (area)

            // DG 21
            // Saw Cuts

            // DG 22
            // Control Joints
        }

        private void CreateTableMembers(CModel model)
        {
            // Create Table
            DataTable dt = new DataTable("TableMembers");
            // Create Table Rows
            dt.Columns.Add("Crsc", typeof(String));
            dt.Columns.Add("Count", typeof(String));
            dt.Columns.Add("TotalLength", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String));
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            dt.Columns["Crsc"].Caption = "Cross-section";
            dt.Columns["Count"].Caption = "Count [-]";
            dt.Columns["TotalLength"].Caption = "Length [m]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/m]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Crsc"].ExtendedProperties.Add("Width", 30f);
            dt.Columns["Count"].ExtendedProperties.Add("Width", 20f);
            dt.Columns["TotalLength"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Crsc"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["Count"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalLength"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            int SumCount = 0;
            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;
            foreach (CCrSc crsc in model.m_arrCrSc.GroupBy(m => m.Name_short).Select(g => g.First()))
            {
                row = dt.NewRow();
                List<CMember> membersList = model.GetListOfMembersWithCrscDatabaseID(crsc.DatabaseID);

                int count = membersList.Where(m => m.BIsGenerated && m.BIsSelectedForMaterialList).Count();
                double totalLength = membersList.Where(m => m.BIsGenerated && m.BIsSelectedForMaterialList).Sum(m => m.FLength_real);
                double totalMass = (crsc.A_g * GlobalConstants.MATERIAL_DENSITY_STEEL * totalLength);
                double totalPrice = totalLength * crsc.price_PPLM_NZD;

                try
                {
                    row["Crsc"] = crsc.Name_short;

                    row["Count"] = count.ToString();
                    SumCount += count;

                    row["TotalLength"] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row["UnitMass"] = (crsc.A_g * GlobalConstants.MATERIAL_DENSITY_STEEL).ToString("F2");

                    row["TotalMass"] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row["UnitPrice"] = crsc.price_PPLM_NZD.ToString("F2");

                    row["Price"] = totalPrice.ToString("F2");
                    SumTotalPrice += totalPrice;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            dBuildingMass += SumTotalMass;
            dBuildingPrice_WithoutGST += SumTotalPrice;

            // Last row
            row = dt.NewRow();
            row["Crsc"] = "Total:";
            row["Count"] = SumCount.ToString();
            row["TotalLength"] = SumTotalLength.ToString("F2");
            row["UnitMass"] = "";
            row["TotalMass"] = SumTotalMass.ToString("F2");
            row["UnitPrice"] = "";
            row["Price"] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);

            Datagrid_Members.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Members.Loaded += Datagrid_Members_Loaded;

        }

        private void Datagrid_Members_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Members);
        }

        private void CreateTablePlates(CModel model)
        {
            float fCFS_PricePerKg_Plates_Material = 2.8f;      // NZD / kg
            float fCFS_PricePerKg_Plates_Manufacture = 2.0f;   // NZD / kg
            float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

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


            List<string> listPlateWidth_bx = new List<string>(1);
            List<string> listPlateHeight_hy = new List<string>(1);
            List<string> listPlateThickness_tz = new List<string>(1);
            List<string> listPlateArea = new List<string>(1);
            List<string> listPlateMassPerPiece = new List<string>(1);
            // Plates

            List<CPlate> ListOfPlateGroups = new List<CPlate>();
            //System.Diagnostics.Trace.WriteLine("model.m_arrConnectionJoints.Count: " + model.m_arrConnectionJoints.Count);
            int count = 0;
            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                model.m_arrConnectionJoints[i].BIsSelectedForMaterialList = CJointHelper.IsJointSelectedForMaterialList(model.m_arrConnectionJoints[i]);

                if (model.m_arrConnectionJoints[i].BIsSelectedForMaterialList)
                {
                    count++;
                    for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
                    {

                        // Nastavime parametre plechu z databazy - TO Ondrej - toto by sa malo diat uz asi pri vytvarani plechov
                        // Nie vsetky plechy budu mat parametre definovane v databaze
                        // !!!! Treba doriesit presne rozmery pri vytvarani plates a zaokruhlovanie

                        try
                        {
                            model.m_arrConnectionJoints[i].m_arrPlates[j].SetParams(model.m_arrConnectionJoints[i].m_arrPlates[j].Name, model.m_arrConnectionJoints[i].m_arrPlates[j].m_ePlateSerieType_FS);
                        }
                        catch { };

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
                                    listPlateTotalArea[k] = listPlateQuantity[k] * dlistPlateArea[k];
                                    listPlateTotalMass[k] = listPlateQuantity[k] * dlistPlateMassPerPiece[k]; // Recalculate total weight of all plates in the group

                                    // Recalculate total price of all plates in the group

                                    if (model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD > 0)
                                        listPlateTotalPrice[k] = listPlateTotalMass[k] * (float)model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD;
                                    else
                                        listPlateTotalPrice[k] = listPlateTotalMass[k] * fCFS_PricePerKg_Plates_Total;

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
                            dlistPlateWidth_bx.Add(fWidth_bx);
                            dlistPlateHeight_hy.Add(fHeight_hy);
                            dlistPlateThickness_tz.Add(Ft);
                            dlistPlateArea.Add(fArea);
                            dlistPlateMassPerPiece.Add(fMassPerPiece);
                            listPlateTotalArea.Add(fTotalArea);
                            listPlateTotalMass.Add(fTotalMass);
                            listPlateTotalPrice.Add(fTotalPrice);

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

            //dTotalPlatesArea_Model = Math.Round(dTotalPlatesArea_Model, iNumberOfDecimalPlacesArea);
            //dTotalPlatesVolume_Model = Math.Round(dTotalPlatesVolume_Model, iNumberOfDecimalPlacesVolume);
            //dTotalPlatesMass_Model = Math.Round(dTotalPlatesMass_Model, iNumberOfDecimalPlacesMass);
            //dTotalPlatesPrice_Model = Math.Round(dTotalPlatesPrice_Model, iNumberOfDecimalPlacesPrice);

            //if (!MathF.d_equal(dTotalPlatesArea_Model, dTotalPlatesArea_Table) ||
            //    !MathF.d_equal(dTotalPlatesVolume_Model, dTotalPlatesVolume_Table) ||
            //    !MathF.d_equal(dTotalPlatesMass_Model, dTotalPlatesMass_Table) ||
            //    (iTotalPlatesNumber_Model != iTotalPlatesNumber_Table)) // Error
            //    MessageBox.Show(
            //    "Total area of plates in model " + dTotalPlatesArea_Model + " m^2" + "\n" +
            //    "Total area of plates in table " + dTotalPlatesArea_Table + " m^2" + "\n" +
            //    "Total volume of plates in model " + dTotalPlatesVolume_Model + " m^3" + "\n" +
            //    "Total volume of plates in table " + dTotalPlatesVolume_Table + " m^3" + "\n" +
            //    "Total weight of plates in model " + dTotalPlatesMass_Model + " kg" + "\n" +
            //    "Total weight of plates in table " + dTotalPlatesMass_Table + " kg" + "\n" +
            //    "Total number of plates in model " + iTotalPlatesNumber_Model + "\n" +
            //    "Total number of plates in table " + iTotalPlatesNumber_Table + "\n");

            // Prepare output format (last row is empty)
            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                // Change output data format
                listPlateWidth_bx.Add(dlistPlateWidth_bx[i].ToString("F3"));
                listPlateHeight_hy.Add(dlistPlateHeight_hy[i].ToString("F3"));
                listPlateThickness_tz.Add(dlistPlateThickness_tz[i].ToString("F3"));
                listPlateArea.Add(dlistPlateArea[i].ToString("F3"));
                listPlateMassPerPiece.Add(dlistPlateMassPerPiece[i].ToString("F3"));
            }

            dBuildingMass += dTotalPlatesMass_Table;
            dBuildingPrice_WithoutGST += dTotalPlatesPrice_Table;

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
            DataTable table = new DataTable("TablePlates");
            // Create Table Rows
            table.Columns.Add("Prefix", typeof(String));
            table.Columns.Add("Quantity", typeof(Int32));
            table.Columns.Add("Material", typeof(String));
            table.Columns.Add("Width", typeof(String));
            table.Columns.Add("Height", typeof(String));
            table.Columns.Add("Thickness", typeof(String));
            table.Columns.Add("Area", typeof(String));
            table.Columns.Add("Mass_per_Piece", typeof(String));
            table.Columns.Add("Total_Area", typeof(Decimal));
            table.Columns.Add("Total_Mass", typeof(Decimal));
            table.Columns.Add("Total_Price", typeof(Decimal));

            // Set Column Caption
            table.Columns["Prefix"].Caption = "Prefix";
            table.Columns["Quantity"].Caption = "Quantity [-]";
            table.Columns["Material"].Caption = "Material";
            table.Columns["Width"].Caption = "Width [m]";
            table.Columns["Height"].Caption = "Height [m]";
            table.Columns["Thickness"].Caption = "Thickness [m]";
            table.Columns["Area"].Caption = "Area [m2]";
            table.Columns["Mass_per_Piece"].Caption = "Mass per Piece [kg]";
            table.Columns["Total_Area"].Caption = "Total Area [m2]";
            table.Columns["Total_Mass"].Caption = "Total Mass [kg]";
            table.Columns["Total_Price"].Caption = "Total Price [NZD]";

            table.Columns["Prefix"].ExtendedProperties.Add("Width", 7.5f);
            table.Columns["Quantity"].ExtendedProperties.Add("Width", 10.0f);
            table.Columns["Material"].ExtendedProperties.Add("Width", 10.0f);
            table.Columns["Width"].ExtendedProperties.Add("Width", 7.5f);
            table.Columns["Height"].ExtendedProperties.Add("Width", 7.5f);
            table.Columns["Thickness"].ExtendedProperties.Add("Width", 10.0f);
            table.Columns["Area"].ExtendedProperties.Add("Width", 7.5f);
            table.Columns["Mass_per_Piece"].ExtendedProperties.Add("Width", 10f);
            table.Columns["Total_Area"].ExtendedProperties.Add("Width", 10f);
            table.Columns["Total_Mass"].ExtendedProperties.Add("Width", 10f);
            table.Columns["Total_Price"].ExtendedProperties.Add("Width", 10f);

            table.Columns["Prefix"].ExtendedProperties.Add("Align", AlignmentX.Left);
            table.Columns["Quantity"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Material"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Width"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Height"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Thickness"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Area"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Mass_per_Piece"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Total_Area"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Total_Mass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            table.Columns["Total_Price"].ExtendedProperties.Add("Align", AlignmentX.Right);


            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < listPlatePrefix.Count; i++)
            {
                DataRow row = table.NewRow();

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
                    row["Total_Area"] = listPlateTotalArea[i].ToString("F3");
                    row["Total_Mass"] = listPlateTotalMass[i].ToString("F3");
                    row["Total_Price"] = listPlateTotalPrice[i].ToString("F3");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_Plates.Loaded += Datagrid_Plates_Loaded;
        }

        private void Datagrid_Plates_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Plates);
        }


        private void CreateTableConnectors(CModel model)
        {
            float fCFS_PricePerKg_Plates_Material = 2.8f;      // NZD / kg
            float fCFS_PricePerKg_Plates_Manufacture = 2.0f;   // NZD / kg

            float fTEK_PricePerPiece_Screws_Total = 0.15f;     // NZD / piece / !!! priblizna cena - nezohladnuje priemer skrutky
            float fAnchor_PricePerLength = 30; // NZD / m - !!! priblizna cena - nezohladnuje priemer tyce
            float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

            List<string> listConnectorPrefix = new List<string>(1);
            List<int> listConnectorQuantity = new List<int>(1);
            List<string> listConnectorMaterialName = new List<string>(1);
            List<string> listConnectorSize = new List<string>(1);
            List<double> listConnectorMassPerPiece = new List<double>(1);
            List<double> listConnectorTotalMass = new List<double>(1);
            List<double> listConnectorTotalPrice = new List<double>(1);

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
                if (!model.m_arrConnectionJoints[i].BIsSelectedForMaterialList) continue;

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
                                        listConnectorTotalMass[m] = listConnectorQuantity[m] * listConnectorMassPerPiece[m]; // Recalculate total mass of all connectors in the group

                                        if (model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Price_PPP_NZD > 0)
                                            listConnectorTotalPrice[m] = listConnectorQuantity[m] * model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k].Price_PPP_NZD; // Recalculate total price of all connectors in the group
                                        else
                                            listConnectorTotalPrice[m] = listConnectorQuantity[m] * fTEK_PricePerPiece_Screws_Total;

                                        bConnectorwasAdded = true;
                                        // TODO - po pridani spojovacieho prostriedku by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                                        break;
                                    }
                                }
                            }

                            if ((i == 0 && j == 0 && k == 0) || !bConnectorwasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
                            {
                                listConnectorPrefix.Add(sPrefix);
                                listConnectorQuantity.Add(iQuantity);
                                listConnectorMaterialName.Add(sMaterialName);
                                listConnectorSize.Add(size);
                                listConnectorMassPerPiece.Add(fMassPerPiece);
                                listConnectorTotalMass.Add(fTotalMass);
                                listConnectorTotalPrice.Add(fTotalPrice);

                                // Add first plate in the group to the list of plate groups
                                ListOfConnectorGroups.Add(model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[k]);
                            }
                        }
                    }

                    // Anchors
                    if (model.m_arrConnectionJoints[i].m_arrPlates[j] is CConCom_Plate_B_basic)
                    {
                        CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)model.m_arrConnectionJoints[i].m_arrPlates[j];

                        if (plate.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
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
                                            listConnectorTotalMass[m] = listConnectorQuantity[m] * listConnectorMassPerPiece[m]; // Recalculate total mass of all connectors in the group

                                            if (plate.AnchorArrangement.Anchors[k].Price_PPP_NZD > 0)
                                                listConnectorTotalPrice[m] = listConnectorQuantity[m] * plate.AnchorArrangement.Anchors[k].Price_PPP_NZD; // Recalculate total price of all connectors in the group
                                            else
                                                listConnectorTotalPrice[m] = listConnectorQuantity[m] * (fAnchor_PricePerLength * plate.AnchorArrangement.Anchors[k].Length);

                                            bConnectorwasAdded = true;
                                            // TODO - po pridani spojovacieho prostriedku by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
                                            break;
                                        }
                                    }
                                }

                                if ((i == 0 && j == 0 && k == 0) || !bConnectorwasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
                                {
                                    listConnectorPrefix.Add(sPrefix);
                                    listConnectorQuantity.Add(iQuantity);
                                    listConnectorMaterialName.Add(sMaterialName);
                                    listConnectorSize.Add(size);
                                    listConnectorMassPerPiece.Add(fMassPerPiece);
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

            // Check Data
            double dTotalConnectorsMass_Model = 0, dTotalConnectorsMass_Table = 0;
            double dTotalConnectorsPrice_Model = 0, dTotalConnectorsPrice_Table = 0;
            int iTotalConnectorsNumber_Model = 0, iTotalConnectorsNumber_Table = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            {
                if (!joint.BIsSelectedForMaterialList) continue;

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

            for (int i = 0; i < listConnectorPrefix.Count; i++)
            {
                dTotalConnectorsMass_Table += listConnectorTotalMass[i];
                dTotalConnectorsPrice_Table += listConnectorTotalPrice[i];
                iTotalConnectorsNumber_Table += listConnectorQuantity[i];
            }

            //To Mato...toto tu treba???
            //if (!MathF.d_equal(dTotalConnectorsMass_Model, dTotalConnectorsMass_Table) ||
            //        (iTotalConnectorsNumber_Model != iTotalConnectorsNumber_Table)) // Error
            //    MessageBox.Show(
            //    "Total weight of connectors in model " + dTotalConnectorsMass_Model + " kg" + "\n" +
            //    "Total weight of connectors in table " + dTotalConnectorsMass_Table + " kg" + "\n" +
            //    "Total number of connectors in model " + iTotalConnectorsNumber_Model + "\n" +
            //    "Total number of connectors in table " + iTotalConnectorsNumber_Table + "\n");

            //// Prepare output format (last row is empty)
            //for (int i = 0; i < listConnectorPrefix.Count; i++)
            //{
            //    // Change output data format
            //    listConnectorMassPerPiece.Add(dlistConnectorMassPerPiece[i].ToString("F2"));
            //}

            dBuildingMass += dTotalConnectorsMass_Table;
            dBuildingPrice_WithoutGST += dTotalConnectorsPrice_Table;

            // Create Table
            DataTable dt = new DataTable("Table3");
            // Create Table Rows

            dt.Columns.Add("Prefix", typeof(String));
            dt.Columns.Add("Quantity", typeof(Int32));
            dt.Columns.Add("Material", typeof(String));
            dt.Columns.Add("Size", typeof(String));
            dt.Columns.Add("Mass_per_Piece", typeof(String));
            dt.Columns.Add("Total_Mass", typeof(Decimal));
            dt.Columns.Add("Total_Price", typeof(Decimal));

            // Prefix | Quantity |     Material     | Size    |   Mass per Piece [kg] | Total Mass [kg] | Unit Price [NZD / piece] | Total Price [NZD]
            // Set Column Caption
            dt.Columns["Prefix"].Caption = "Prefix";
            dt.Columns["Quantity"].Caption = "Quantity";
            dt.Columns["Material"].Caption = "Material";
            dt.Columns["Size"].Caption = "Size";
            dt.Columns["Mass_per_Piece"].Caption = "Mass per Piece [kg]";
            dt.Columns["Total_Mass"].Caption = "Total Mass [kg]";
            dt.Columns["Total_Price"].Caption = "Total Price [NZD]";

            dt.Columns["Prefix"].ExtendedProperties.Add("Width", 30f);
            dt.Columns["Quantity"].ExtendedProperties.Add("Width", 20f);
            dt.Columns["Material"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Size"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Mass_per_Piece"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Total_Mass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Total_Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Prefix"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["Quantity"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Material"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Size"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Mass_per_Piece"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Total_Mass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Total_Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);
            DataRow row;
            for (int i = 0; i < listConnectorPrefix.Count; i++)
            {
                row = dt.NewRow();
                try
                {
                    row["Prefix"] = listConnectorPrefix[i];
                    row["Quantity"] = listConnectorQuantity[i];
                    row["Material"] = listConnectorMaterialName[i];
                    row["Size"] = listConnectorSize[i];
                    row["Mass_per_Piece"] = listConnectorMassPerPiece[i].ToString("F2");
                    row["Total_Mass"] = listConnectorTotalMass[i].ToString("F3");
                    row["Total_Price"] = listConnectorTotalPrice[i].ToString("F3");
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            // Add Sum
            row = dt.NewRow();
            row["Prefix"] = "Total:";
            row["Quantity"] = iTotalConnectorsNumber_Table;
            row["Material"] = "";
            row["Size"] = "";
            row["Mass_per_Piece"] = "";
            row["Total_Mass"] = dTotalConnectorsMass_Table.ToString("F3");
            row["Total_Price"] = dTotalConnectorsPrice_Table.ToString("F3");
            dt.Rows.Add(row);

            Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        }

        private void Datagrid_Connectors_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Connectors);
        }

        private void CreateTableCladding(CPFDViewModel vm,
             float fWallArea_Total,
             float fTotalAreaOfOpennings,
             float fFibreGlassArea_Walls,
             float fRoofArea,
             float fFibreGlassArea_Roof
            )
        {
            // Plocha stien bez otvorov a fibre glass
            float fWallArea_Total_Netto = fWallArea_Total - fTotalAreaOfOpennings - fFibreGlassArea_Walls;

            // Plocha strechy bez fibre glass
            float fRoofArea_Total_Netto = fRoofArea - fFibreGlassArea_Roof;

            List<string> claddings = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting_m", "name");
            string roofCladding = claddings.ElementAtOrDefault(vm.RoofCladdingIndex);
            string wallCladding = claddings.ElementAtOrDefault(vm.WallCladdingIndex);

            List<string> list_roofCladdingThickness = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", roofCladding, "name");
            List<string> list_wallCladdingThickness = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", wallCladding, "name");

            string roofCladdingThickness = list_roofCladdingThickness.ElementAtOrDefault(vm.RoofCladdingThicknessIndex);
            string wallCladdingThickness = list_wallCladdingThickness.ElementAtOrDefault(vm.WallCladdingThicknessIndex);

            List<CTrapezoidalSheetingColours> colours = CTrapezoidalSheetingManager.LoadTrapezoidalSheetingColours();

            CTS_CrscProperties prop_RoofCladding = new CTS_CrscProperties();
            prop_RoofCladding = CTrapezoidalSheetingManager.GetSectionProperties($"{roofCladding}-{roofCladdingThickness}");

            CTS_CrscProperties prop_WallCladding = new CTS_CrscProperties();
            prop_WallCladding = CTrapezoidalSheetingManager.GetSectionProperties($"{wallCladding}-{wallCladdingThickness}");

            float fRoofCladdingPrice_PSM_NZD = (float)prop_RoofCladding.price_PPSM_NZD; // Cena roof cladding za 1 m^2
            float fWallCladdingPrice_PSM_NZD = (float)prop_WallCladding.price_PPSM_NZD; ; // Cena wall cladding za 1 m^2

            float fRoofCladdingPrice_Total_NZD = fRoofArea_Total_Netto * fRoofCladdingPrice_PSM_NZD;
            float fWallCladdingPrice_Total_NZD = fWallArea_Total_Netto * fWallCladdingPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("TableCladding");
            // Create Table Rows
            dt.Columns.Add("Cladding", typeof(String));
            dt.Columns.Add("Thickness", typeof(String));
            dt.Columns.Add("Color", typeof(String));
            dt.Columns.Add("ColorName", typeof(String));
            //dt.Columns.Add("TotalLength", typeof(String)); // Dalo by sa spocitat ak podelime plochu sirkou profilu
            dt.Columns.Add("TotalArea", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String)); // kg / m^2
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            dt.Columns["Cladding"].Caption = "Cladding";
            dt.Columns["Thickness"].Caption = "Thickness [mm]";
            dt.Columns["Color"].Caption = "Color";
            dt.Columns["ColorName"].Caption = "Color Name";
            //dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            dt.Columns["TotalArea"].Caption = "Total Area [m2]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m2]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/m2]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Cladding"].ExtendedProperties.Add("Width", 20f);
            dt.Columns["Thickness"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Color"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["ColorName"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Cladding"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["Thickness"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Color"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["ColorName"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;

            if (fRoofArea_Total_Netto > 0) // Roof Cladding
            {
                row = dt.NewRow();

                float fUnitMass = (float)prop_RoofCladding.mass_kg_m2;
                float totalMass = fRoofArea_Total_Netto * fUnitMass;
                try
                {
                    row["Cladding"] = roofCladding;
                    row["Thickness"] = roofCladdingThickness;
                    row["Color"] = colours.ElementAtOrDefault(vm.RoofCladdingColorIndex).CodeHEX;
                    row["ColorName"] = colours.ElementAtOrDefault(vm.RoofCladdingColorIndex).Name;
                    row["TotalArea"] = fRoofArea_Total_Netto.ToString("F2");
                    SumTotalArea += fRoofArea_Total_Netto;

                    row["UnitMass"] = fUnitMass.ToString("F2");

                    row["TotalMass"] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row["UnitPrice"] = fRoofCladdingPrice_PSM_NZD.ToString("F2");

                    row["Price"] = fRoofCladdingPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fRoofCladdingPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (fWallArea_Total_Netto > 0) // Wall Cladding
            {
                row = dt.NewRow();

                float fUnitMass = (float)prop_WallCladding.mass_kg_m2;
                float totalMass = fWallArea_Total_Netto * fUnitMass;
                try
                {
                    row["Cladding"] = wallCladding;
                    row["Thickness"] = wallCladdingThickness;
                    row["Color"] = colours.ElementAtOrDefault(vm.WallCladdingColorIndex).CodeHEX;
                    row["ColorName"] = colours.ElementAtOrDefault(vm.WallCladdingColorIndex).Name;

                    row["TotalArea"] = fWallArea_Total_Netto.ToString("F2");
                    SumTotalArea += fWallArea_Total_Netto;

                    row["UnitMass"] = fUnitMass.ToString("F2");

                    row["TotalMass"] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row["UnitPrice"] = fWallCladdingPrice_PSM_NZD.ToString("F2");

                    row["Price"] = fWallCladdingPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fWallCladdingPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingPrice_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row["Cladding"] = "Total:";
                row["Thickness"] = "";
                row["Color"] = "";
                row["ColorName"] = "";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row["TotalArea"] = SumTotalArea.ToString("F2");
                row["UnitMass"] = "";
                row["TotalMass"] = SumTotalMass.ToString("F2");
                row["UnitPrice"] = "";
                row["Price"] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_Cladding.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_Cladding.Loaded += Datagrid_Cladding_Loaded;
            }
        }

        private void Datagrid_Cladding_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Cladding);
        }

        private void SetLastRowBold(DataGrid datagrid)
        {
            DataGridRow dtrow = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(datagrid.Items.Count - 1);
            Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);
            Style newStyle = new Style(dtrow.GetType());

            newStyle.Setters.Add(bold);
            dtrow.Style = newStyle;
        }

        private void CreateTableDoorsAndWindows(List<COpeningProperties> list)
        {
            // Create Table
            DataTable dt = new DataTable("TableDoorsAndWindows");
            // Create Table Rows
            dt.Columns.Add("Opening", typeof(String));
            dt.Columns.Add("Width", typeof(String));
            dt.Columns.Add("Height", typeof(String));
            dt.Columns.Add("Count", typeof(String));
            dt.Columns.Add("Area", typeof(String));
            dt.Columns.Add("TotalArea", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String)); // kg / m^2
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice_PPSM", typeof(String));
            dt.Columns.Add("UnitPrice_PPP", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            dt.Columns["Opening"].Caption = "Opening";
            dt.Columns["Width"].Caption = "Width [m]";
            dt.Columns["Height"].Caption = "Height [m]";
            dt.Columns["Count"].Caption = "Count [-]";
            dt.Columns["Area"].Caption = "Area [m2]";
            dt.Columns["TotalArea"].Caption = "Total Area [m2]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m2]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice_PPSM"].Caption = "Unit Price [NZD/m2]";
            dt.Columns["UnitPrice_PPP"].Caption = "Unit Price [NZD/piece]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Opening"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Width"].ExtendedProperties.Add("Width", 7.5f);
            dt.Columns["Height"].ExtendedProperties.Add("Width", 7.5f);
            dt.Columns["Count"].ExtendedProperties.Add("Width", 7.5f);
            dt.Columns["Area"].ExtendedProperties.Add("Width", 7.5f);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice_PPSM"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice_PPP"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Opening"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["Width"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Height"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Count"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Area"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice_PPSM"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice_PPP"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            int SumCount = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (COpeningProperties prop in list)
            {
                AddOpeningItemRow(dt,
                            "Opening",
                            prop.Type,
                            prop.fWidth,
                            prop.fHeight,
                            prop.Count,
                            prop.Area,
                            prop.Area * prop.Count,
                            prop.UnitMass_SM,
                            prop.UnitMass_SM * prop.Area,
                            prop.Price_PPSM_NZD,
                            prop.Price_PPP_NZD,
                            prop.Price_PPSM_NZD * prop.Area * prop.Count,
                            ref SumCount,
                            ref SumTotalArea,
                            ref SumTotalMass,
                            ref SumTotalPrice);
            }

            DataRow row;
            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingPrice_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row["Opening"] = "Total:";
                row["Width"] = "";
                row["Height"] = "";
                row["Count"] = SumCount.ToString();
                row["Area"] = "";
                row["TotalArea"] = SumTotalArea.ToString("F2");
                row["UnitMass"] = "";
                row["TotalMass"] = SumTotalMass.ToString("F2");
                row["UnitPrice_PPSM"] = "";
                row["UnitPrice_PPP"] = "";
                row["Price"] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_DoorsAndWindows.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_DoorsAndWindows.Loaded += Datagrid_DoorsAndWindows_Loaded;
            }
            else // TODO Ondrej - Tabulka je prazdna - nezobrazime ju
            {
                DoorsAndWindowsLabel.IsEnabled = false;
                DoorsAndWindowsLabel.Visibility = Visibility.Hidden;

                Datagrid_DoorsAndWindows.IsEnabled = false;
                Datagrid_DoorsAndWindows.Visibility = Visibility.Hidden;
            }
        }

        private void Datagrid_DoorsAndWindows_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_DoorsAndWindows);
        }

        private void CreateTableFibreglass(CPFDViewModel vm,
            float fFibreGlassArea_Roof,
            float fFibreGlassArea_Walls)
        {
            List<CLengthItemProperties> listOfProperties = CLengthItemManager.LoadLengthItemsProperties("Fibreglass");

            float fRoofFibreGlassPrice_PSM_NZD = (float)listOfProperties[0].Price_PPSM_NZD; // Cena roof fibreglass za 1 m^2
            float fWallFibreGlassPrice_PSM_NZD = (float)listOfProperties[1].Price_PPSM_NZD; ; // Cena wall fibreglass za 1 m^2

            float fRoofFibreGlassUnitMass_SM = (float)listOfProperties[0].Mass_kg_m2;
            float fWallFibreGlassUnitMass_SM = (float)listOfProperties[1].Mass_kg_m2;

            float fRoofFibreGlassPrice_Total_NZD = fFibreGlassArea_Roof * fRoofFibreGlassPrice_PSM_NZD;
            float fWallFibreGlassPrice_Total_NZD = fFibreGlassArea_Walls * fWallFibreGlassPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("TableFibreglass");
            // Create Table Rows
            dt.Columns.Add("Fibreglass", typeof(String));
            //dt.Columns.Add("TotalLength", typeof(String)); // Dalo by sa spocitat ak podelime plochu sirkou profilu
            dt.Columns.Add("TotalArea", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String)); // kg / m^2
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            dt.Columns["Fibreglass"].Caption = "Fibreglass";
            //dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            dt.Columns["TotalArea"].Caption = "Total Area [m2]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m2]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/m2]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Fibreglass"].ExtendedProperties.Add("Width", 50f);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Fibreglass"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            AddSurfaceItemRow(dt,
                        "Fibreglass",
                        "Roof Fibreglass",
                        fFibreGlassArea_Roof,
                        fRoofFibreGlassUnitMass_SM,
                        fRoofFibreGlassUnitMass_SM * fFibreGlassArea_Roof,
                        fRoofFibreGlassPrice_PSM_NZD,
                        fRoofFibreGlassPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddSurfaceItemRow(dt,
                        "Fibreglass",
                        "Wall Fibreglass",
                        fFibreGlassArea_Walls,
                        fWallFibreGlassUnitMass_SM,
                        fWallFibreGlassUnitMass_SM * fFibreGlassArea_Walls,
                        fWallFibreGlassPrice_PSM_NZD,
                        fWallFibreGlassPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            DataRow row;
            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingPrice_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row["Fibreglass"] = "Total:";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row["TotalArea"] = SumTotalArea.ToString("F2");
                row["UnitMass"] = "";
                row["TotalMass"] = SumTotalMass.ToString("F2");
                row["UnitPrice"] = "";
                row["Price"] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_Fibreglass.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_Fibreglass.Loaded += Datagrid_Fibreglass_Loaded;
            }
            else // TODO Ondrej - Tabulka je prazdna - nezobrazime ju
            {
                FibreglassLabel.IsEnabled = false;
                FibreglassLabel.Visibility = Visibility.Hidden;

                Datagrid_Fibreglass.IsEnabled = false;
                Datagrid_Fibreglass.Visibility = Visibility.Hidden;
            }
        }

        private void Datagrid_Fibreglass_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Fibreglass);
        }

        private void CreateTableRoofNetting(float fRoofArea)
        {
            List<CPlaneItemProperties> listOfProperties = CPlaneItemManager.LoadPlaneItemsProperties("RoofNetting");

            // Roof Netting and Sisalation
            // Roof Sisalation Foil
            // Roof Safe Net
            float fRoofSisalationFoilPrice_PSM_NZD = (float)listOfProperties[0].Price_PPSM_NZD; // Cena roof foil za 1 m^2
            float fRoofSafeNetPrice_PSM_NZD = (float)listOfProperties[1].Price_PPSM_NZD; // Cena roof net za 1 m^2

            float fRoofSisalationFoilUnitMass_SM = (float)listOfProperties[0].Mass_kg_m2;
            float fRoofSafeNetUnitMass_SM = (float)listOfProperties[1].Mass_kg_m2;

            float fRoofSisalationFoilPrice_Total_NZD = fRoofArea * fRoofSisalationFoilPrice_PSM_NZD;
            float fRoofSafeNetPrice_Total_NZD = fRoofArea * fRoofSafeNetPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("TableRoofNetting");
            // Create Table Rows
            dt.Columns.Add("Component", typeof(String));
            dt.Columns.Add("TotalArea", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String)); // kg / m^2
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            dt.Columns["Component"].Caption = "Component";
            dt.Columns["TotalArea"].Caption = "Total Area [m2]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m2]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/m2]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Component"].ExtendedProperties.Add("Width", 50f);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Component"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["TotalArea"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            AddSurfaceItemRow(dt,
                        "Component",
                        listOfProperties[0].Name,
                        fRoofArea,
                        fRoofSisalationFoilUnitMass_SM,
                        fRoofSisalationFoilUnitMass_SM * fRoofArea,
                        fRoofSisalationFoilPrice_PSM_NZD,
                        fRoofSisalationFoilPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddSurfaceItemRow(dt,
                        "Component",
                        listOfProperties[1].Name,
                        fRoofArea,
                        fRoofSafeNetUnitMass_SM,
                        fRoofSafeNetUnitMass_SM * fRoofArea,
                        fRoofSafeNetPrice_PSM_NZD,
                        fRoofSafeNetPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            DataRow row;
            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingPrice_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row["Component"] = "Total:";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row["TotalArea"] = SumTotalArea.ToString("F2");
                row["UnitMass"] = "";
                row["TotalMass"] = SumTotalMass.ToString("F2");
                row["UnitPrice"] = "";
                row["Price"] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_RoofNetting.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_RoofNetting.Loaded += Datagrid_RoofNetting_Loaded;
            }
        }

        private void Datagrid_RoofNetting_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_RoofNetting);
        }

        private void CreateTableFlashing(CModel model,
        float fRoofSideLength,
        float fRollerDoorTrimmerFlashing_TotalLength,
        float fRollerDoorLintelFlashing_TotalLength,
        float fRollerDoorLintelCapFlashing_TotalLength,
        float fPADoorTrimmerFlashing_TotalLength,
        float fPADoorLintelFlashing_TotalLength,
        float fWindowFlashing_TotalLength)
        {
            List<CLengthItemProperties> listOfProperties = CLengthItemManager.LoadLengthItemsProperties("Flashing");
            //CFlashingsManager.LoadFlashingsPropertiesDictionary();
            //Dictionary<string, CLengthItemProperties> dict = CFlashingsManager.DictFlashingProperties;

            // TODO Ondrej - toto by chcelo naplnat a pouzivat nejako krajsie, mozno by sa to dalo cele zabalit do nejakeho systemu, kde zadam len flashing name a dlzku a vsetko ostatne
            // sa udeje automaticky v cykle cez jednotlive polozky, teraz nizsie 9 krat naplnam parametre funkcie AddLengthItemRow, co je tiez dost skarede

            // Price LM
            float fRoofRidgeFlashingPrice_PLM_NZD = (float)listOfProperties[0].Price_PPLM_NZD; // Cena roof ridge flashing za 1 m dlzky 
            float fWallCornerFlashingPrice_PLM_NZD = (float)listOfProperties[1].Price_PPLM_NZD; // Cena corner flashing za 1 m dlzky
            float fBargeFlashingPrice_PLM_NZD = (float)listOfProperties[2].Price_PPLM_NZD; // Cena barge flashing za 1 m dlzky

            float fRollerDoorTrimmerFlashingPrice_PLM_NZD = (float)listOfProperties[3].Price_PPLM_NZD; // Cena roller door trimmer flashing za 1 m dlzky
            float fRollerDoorLintelFlashingPrice_PLM_NZD = (float)listOfProperties[4].Price_PPLM_NZD; // Cena roller door lintel flashing za 1 m dlzky
            float fRollerDoorLintelCapFlashingPrice_PLM_NZD = (float)listOfProperties[5].Price_PPLM_NZD; // Cena cap flashing za 1 m dlzky
            float fPADoorTrimmerFlashingPrice_PLM_NZD = (float)listOfProperties[6].Price_PPLM_NZD; // Cena PA door trimmer flashing za 1 m dlzky
            float fPADoorLintelFlashingPrice_PLM_NZD = (float)listOfProperties[7].Price_PPLM_NZD; // Cena PA door lintel flashing za 1 m dlzky
            float fWindowFlashingPrice_PLM_NZD = (float)listOfProperties[8].Price_PPLM_NZD; // Cena window flashing za 1 m dlzky

            // Mass LM
            float fRoofRidgeFlashingUnitMass_LM = (float)listOfProperties[0].Mass_kg_lm;
            float fWallCornerFlashingUnitMass_LM = (float)listOfProperties[1].Mass_kg_lm;
            float fBargeFlashingUnitMass_LM = (float)listOfProperties[2].Mass_kg_lm;

            float fRollerDoorTrimmerFlashingUnitMass_LM = (float)listOfProperties[3].Mass_kg_lm;
            float fRollerDoorLintelFlashingUnitMass_LM = (float)listOfProperties[4].Mass_kg_lm;
            float fRollerDoorLintelCapFlashingUnitMass_LM = (float)listOfProperties[5].Mass_kg_lm;
            float fPADoorTrimmerFlashingUnitMass_LM = (float)listOfProperties[6].Mass_kg_lm;
            float fPADoorLintelFlashingUnitMass_LM = (float)listOfProperties[7].Mass_kg_lm;
            float fWindowFlashingUnitMass_LM = (float)listOfProperties[8].Mass_kg_lm;

            // TODO Ondrej

            // Refaktorovat kody
            // Skus to popozerat a pripadne nejako zautomatizovat
            // V principe mame 2 typy poloziek
            // 1 - definovane dlzkou (flashings, gutters, mozno sa da uvazovat aj fibreglass)
            // 2 - definovene plochou (doors, windows, roof netting)

            float fRoofRidgeFlashingPrice_Total_NZD = model.fL_tot * fRoofRidgeFlashingPrice_PLM_NZD;
            float fWallCornerFlashingPrice_Total_NZD = 4 * model.fH1_frame * fWallCornerFlashingPrice_PLM_NZD;
            float fBargeFlashingPrice_Total_NZD = 4 * fRoofSideLength * fBargeFlashingPrice_PLM_NZD;

            float fRollerDoorTrimmerFlashingPrice_Total_NZD = fRollerDoorTrimmerFlashing_TotalLength * fRollerDoorTrimmerFlashingPrice_PLM_NZD;
            float fRollerDoorLintelFlashingPrice_Total_NZD = fRollerDoorLintelFlashing_TotalLength * fRollerDoorLintelFlashingPrice_PLM_NZD;
            float fRollerDoorLintelCapFlashingPrice_Total_NZD = fRollerDoorLintelCapFlashing_TotalLength * fRollerDoorLintelCapFlashingPrice_PLM_NZD;
            float fPADoorTrimmerFlashingPrice_Total_NZD = fPADoorTrimmerFlashing_TotalLength * fPADoorTrimmerFlashingPrice_PLM_NZD;
            float fPADoorLintelFlashingPrice_Total_NZD = fPADoorLintelFlashing_TotalLength * fPADoorLintelFlashingPrice_PLM_NZD;
            float fWindowFlashingPrice_Total_NZD = fWindowFlashing_TotalLength * fWindowFlashingPrice_PLM_NZD;

            // Create Table
            DataTable dt = new DataTable("TableFlashing");
            // Create Table Rows
            dt.Columns.Add("Flashing", typeof(String));
            dt.Columns.Add("TotalLength", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String));
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            dt.Columns["Flashing"].Caption = "Flashing";
            dt.Columns["TotalLength"].Caption = "Total Length [m]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/m]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Flashing"].ExtendedProperties.Add("Width", 50f);
            dt.Columns["TotalLength"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Flashing"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["TotalLength"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roof Ridge Flashing",
                        model.fL_tot,
                        fRoofRidgeFlashingUnitMass_LM,
                        fRoofRidgeFlashingUnitMass_LM * model.fL_tot,
                        fRoofRidgeFlashingPrice_PLM_NZD,
                        fRoofRidgeFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Wall Corner Flashing",
                        4 * model.fH1_frame,
                        fWallCornerFlashingUnitMass_LM,
                        fWallCornerFlashingUnitMass_LM * 4 * model.fH1_frame,
                        fWallCornerFlashingPrice_PLM_NZD,
                        fWallCornerFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Barge Flashing",
                        4 * fRoofSideLength,
                        fBargeFlashingUnitMass_LM,
                        fBargeFlashingUnitMass_LM * 4 * fRoofSideLength,
                        fBargeFlashingPrice_PLM_NZD,
                        fBargeFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roller Door Trimmer Flashing",
                        fRollerDoorTrimmerFlashing_TotalLength,
                        fRollerDoorTrimmerFlashingUnitMass_LM,
                        fRollerDoorTrimmerFlashingUnitMass_LM * fRollerDoorTrimmerFlashing_TotalLength,
                        fRollerDoorTrimmerFlashingPrice_PLM_NZD,
                        fRollerDoorTrimmerFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roller Door Lintel Flashing",
                        fRollerDoorLintelFlashing_TotalLength,
                        fRollerDoorLintelFlashingUnitMass_LM,
                        fRollerDoorLintelFlashingUnitMass_LM * fRollerDoorLintelFlashing_TotalLength,
                        fRollerDoorLintelFlashingPrice_PLM_NZD,
                        fRollerDoorLintelFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roller Door Lintel Cap Flashing",
                        fRollerDoorLintelCapFlashing_TotalLength,
                        fRollerDoorLintelCapFlashingUnitMass_LM,
                        fRollerDoorLintelCapFlashingUnitMass_LM * fRollerDoorLintelCapFlashing_TotalLength,
                        fRollerDoorLintelCapFlashingPrice_PLM_NZD,
                        fRollerDoorLintelCapFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "PA Door Trimmer Flashing",
                        fPADoorTrimmerFlashing_TotalLength,
                        fPADoorTrimmerFlashingUnitMass_LM,
                        fPADoorTrimmerFlashingUnitMass_LM * fPADoorTrimmerFlashing_TotalLength,
                        fPADoorTrimmerFlashingPrice_PLM_NZD,
                        fPADoorTrimmerFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "PA Door Lintel Flashing",
                        fPADoorLintelFlashing_TotalLength,
                        fPADoorLintelFlashingUnitMass_LM,
                        fPADoorLintelFlashingUnitMass_LM * fPADoorLintelFlashing_TotalLength,
                        fPADoorLintelFlashingPrice_PLM_NZD,
                        fPADoorLintelFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Window Flashing",
                        fWindowFlashing_TotalLength,
                        fWindowFlashingUnitMass_LM,
                        fWindowFlashingUnitMass_LM * fWindowFlashing_TotalLength,
                        fWindowFlashingPrice_PLM_NZD,
                        fWindowFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            dBuildingMass += SumTotalMass;
            dBuildingPrice_WithoutGST += SumTotalPrice;

            // Last row
            DataRow row;
            row = dt.NewRow();
            row["Flashing"] = "Total:";
            row["TotalLength"] = SumTotalLength.ToString("F2");
            row["UnitMass"] = "";
            row["TotalMass"] = SumTotalMass.ToString("F2");
            row["UnitPrice"] = "";
            row["Price"] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);

            Datagrid_Flashing.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Flashing.Loaded += Datagrid_Flashing_Loaded;
        }

        private void Datagrid_Flashing_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Flashing);
        }

        private void CreateTableGutters(CModel model)
        {
            List<CLengthItemProperties> listOfProperties = CLengthItemManager.LoadLengthItemsProperties("Gutters");

            float fGuttersTotalLength = 2 * model.fL_tot; // na dvoch okrajoch strechy
            float fRoofGutterPrice_PLM_NZD = (float)listOfProperties[0].Price_PPLM_NZD; // Cena roof gutter za 1 m dlzky

            float fRoofGutterUnitMass_LM = (float)listOfProperties[0].Mass_kg_lm;
            float fGuttersPrice_Total_NZD = fGuttersTotalLength * fRoofGutterPrice_PLM_NZD;

            // Create Table
            DataTable dt = new DataTable("TableGutter");
            // Create Table Rows
            dt.Columns.Add("Gutter", typeof(String));
            dt.Columns.Add("TotalLength", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String));
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            dt.Columns["Gutter"].Caption = "Gutter";
            dt.Columns["TotalLength"].Caption = "Total Length [m]";
            dt.Columns["UnitMass"].Caption = "Unit Mass [kg/m]";
            dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/m]";
            dt.Columns["Price"].Caption = "Price [NZD]";

            dt.Columns["Gutter"].ExtendedProperties.Add("Width", 50f);
            dt.Columns["TotalLength"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Width", 10f);
            dt.Columns["Price"].ExtendedProperties.Add("Width", 10f);

            dt.Columns["Gutter"].ExtendedProperties.Add("Align", AlignmentX.Left);
            dt.Columns["TotalLength"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["TotalMass"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["UnitPrice"].ExtendedProperties.Add("Align", AlignmentX.Right);
            dt.Columns["Price"].ExtendedProperties.Add("Align", AlignmentX.Right);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            AddLengthItemRow(dt,
                        "Gutter",
                        "Drip Edge Gutter",
                        fGuttersTotalLength,
                        fRoofGutterUnitMass_LM,
                        fRoofGutterUnitMass_LM * fGuttersTotalLength,
                        fRoofGutterPrice_PLM_NZD,
                        fGuttersPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            dBuildingMass += SumTotalMass;
            dBuildingPrice_WithoutGST += SumTotalPrice;

            //if (dt.Rows.Count > 1) // Len ak su v tabulke rozne typy gutters // Zatial komentujem, dal by sa tym usetrit jeden riadok
            //{
            // Last row
            DataRow row;
            row = dt.NewRow();
            row["Gutter"] = "Total:";
            row["TotalLength"] = SumTotalLength.ToString("F2");
            row["UnitMass"] = "";
            row["TotalMass"] = SumTotalMass.ToString("F2");
            row["UnitPrice"] = "";
            row["Price"] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);
            //}

            Datagrid_Gutters.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Gutters.Loaded += Datagrid_Gutters_Loaded;
        }

        private void Datagrid_Gutters_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Gutters);
        }

        private void AddLengthItemRow(DataTable dt,
            string itemColumnName,
            string itemName,
            double totalLength,
            double unitMass,
            double totalMass,
            double unitPrice,
            double price,
            ref double SumTotalLength,
            ref double SumTotalMass,
            ref double SumTotalPrice)
        {
            if (totalLength > 0 && price > 0) // Add new row only if length and price are more than zero
            {
                DataRow row;

                row = dt.NewRow();

                try
                {
                    row[itemColumnName] = itemName;

                    row["TotalLength"] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row["UnitMass"] = unitMass.ToString("F2");

                    row["TotalMass"] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row["UnitPrice"] = unitPrice.ToString("F2");

                    row["Price"] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }

        private void AddSurfaceItemRow(DataTable dt,
            string itemColumnName,
            string itemName,
            double totalArea,
            double unitMass,
            double totalMass,
            double unitPrice,
            double price,
            ref double SumTotalArea,
            ref double SumTotalMass,
            ref double SumTotalPrice)
        {
            if (totalArea > 0 && price > 0) // Add new row only if area and price are more than zero
            {
                DataRow row;

                row = dt.NewRow();

                try
                {
                    row[itemColumnName] = itemName;

                    row["TotalArea"] = totalArea.ToString("F2");
                    SumTotalArea += totalArea;

                    row["UnitMass"] = unitMass.ToString("F2");

                    row["TotalMass"] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row["UnitPrice"] = unitPrice.ToString("F2");

                    row["Price"] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }

        private void AddOpeningItemRow(DataTable dt,
                string itemColumnName,
                string itemName,
                double width,
                double height,
                int count,
                double area,
                double totalArea,
                double unitMass,
                double totalMass,
                double unitPrice_PPSM,
                double unitPrice_PPP,
                double price,
                ref int SumCount,
                ref double SumTotalArea,
                ref double SumTotalMass,
                ref double SumTotalPrice)
        {
            if (totalArea > 0 && price > 0) // Add new row only if area and price are more than zero
            {
                DataRow row;

                row = dt.NewRow();

                try
                {
                    row[itemColumnName] = itemName;

                    row["Width"] = width.ToString("F2");
                    row["Height"] = height.ToString("F2");
                    row["Count"] = count.ToString();
                    SumCount += count;

                    row["Area"] = area.ToString("F2");

                    row["TotalArea"] = totalArea.ToString("F2");
                    SumTotalArea += totalArea;

                    row["UnitMass"] = unitMass.ToString("F2");

                    row["TotalMass"] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row["UnitPrice_PPSM"] = unitPrice_PPSM.ToString("F2");
                    row["UnitPrice_PPP"] = unitPrice_PPP.ToString("F2");

                    row["Price"] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }
    }



}
