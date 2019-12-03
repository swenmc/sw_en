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

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_Quotation.xaml
    /// </summary>
    public partial class UC_Quotation : UserControl
    {
        double fBuildingPrice_WithoutGST = 0;

        public UC_Quotation(CPFDViewModel vm)
        {
            InitializeComponent();

            CModel model = vm.Model;

            List<Point> fWallDefinitionPoints_Left = new List<Point>(4) { new Point(0, 0), new Point(model.fL_tot, 0), new Point(model.fL_tot, model.fH1_frame), new Point(0, model.fH1_frame) };
            List<Point> fWallDefinitionPoints_Front = new List<Point>(5) { new Point(0, 0), new Point(model.fW_frame, 0), new Point(model.fW_frame, model.fH1_frame), new Point(0.5 * model.fW_frame, model.fH2_frame), new Point(0, model.fH1_frame) };

            // TO Ondrej - Tieto plochy by sa mali zohladnovat len ak su zapnute girt na prislusnych stranach - bGenerate
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

            // Sem dat members podla Crsc, alebo podla MemberType_FS alebo podla MemberType_FS_Position
            // V jednom riadku spocitat dlzku pre vsetky pruty daneho typu

            // Cross-section | Total Length [m] | Price PLM | Total Price
            // 270115 | 245.54 | 4.54 | Total Price
            //  !!!!! Cisla v datagridoch zarovnavat napravo (rovnaky pocet desatinnych miest aby boli desatinne  ciarky pod sebou
            CreateTableMembers(model);

            // DG 2
            // Plates

            // TODO Ondrej - sem dat Plates presne ako su v UC_Material

            // TODO - dopracovat apex brace plates

            // DG 3
            // Screws

            // DG 4
            // Anchors

            // DG 5
            // Washers

            // DG 6
            // Bolts

            // DG 7
            // Doors and windows
            float fTotalAreaOfOpennings = 0;

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            float fPADoorTrimmerFlashing_TotalLength = 0;
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

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
            }

            foreach (WindowProperties wp in vm.WindowBlocksProperties)
            {
                fTotalAreaOfOpennings += wp.fWindowsWidth * wp.fWindowsHeight;

                fWindowFlashing_TotalLength += (2 * wp.fWindowsWidth + 2 * wp.fWindowsHeight);
            }

            // TODO Ondrej
            // Zobrazit Datagrid
            // Rovnake dvere / okna budu v jednom riadku
            //  !!!!! Cisla v datagridoch zarovnavat napravo (rovnaky pocet desatinnych miest aby boli desatinne  ciarky pod sebou

            // Type           | Width | Height | Count | Area  | Total Area | Price PSM | Price PP  | Total Price
            // Roller Door    | 3.3   | 4.5    | 5     | 12.20 | 62.21      | 301       | 3000      | 18000
            // Personnel Door | 1.0   | 2.1    | 4     |  2.20 |  8.21      | 350       | 700       |  2800
            // Personnel Door | 0.8   | 2.0    | 2     |  1.80 |  3.60      | 350       | 650       |  1300

            // DG 9
            // Cladding
            float fWallArea_Total = fWallArea_Left + fWallArea_Right + fWallArea_Front + fWallArea_Back;

            // Dlzka hrany strechy
            float fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame - model.fH1_frame) + MathF.Pow2(0.5f * model.fW_frame));

            // Tato plocha by sa mala zohladnovat len ak su zapnute purlins - bGenerate

            float fRoofArea = 2 * fRoofSideLength * model.fL_tot;

            float fFibreGlassArea_Roof = 0.20f * fRoofArea; // Priesvitna cast strechy TODO Percento pre fibre glass zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul
            float fFibreGlassArea_Walls = 0.05f * fWallArea_Total; // Priesvitna cast strechy TODO Percento zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul

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
            // Roof Netting and Sisalation
            // Roof Sisalation Foil
            // Roof Safe Net

            float fRoofSisalationFoilPrice_PSM_NZD = 0.90f; // Cena roof foil za 1 m^2 // TODO - zapracovat do databazy
            float fRoofSafeNetPrice_PSM_NZD = 1.10f; // Cena roof net za 1 m^2 // TODO - zapracovat do databazy

            // TODO Ondrej
            // Zobrazit Datagrid s 2 riadkami
            // Roof Sisalation Foil | Total Area | Price PSM | Total Price
            // Roof Safe Net | Total Area | Price PSM | Total Price
            float fRoofSisalationFoilPrice_Total_NZD = fRoofArea * fRoofSisalationFoilPrice_PSM_NZD; // TODO Ondrej
            float fRoofSafeNetPrice_Total_NZD = fRoofArea * fRoofSafeNetPrice_PSM_NZD; // TODO Ondrej

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


            // TODO Ondrej
            // Na zaver potrebujeme spocitat vsetky sumy z datagridov a vypocitat cenu za meter stvorcovy a meter kubicky.

            // Tieto hodnoty spolu s plochou, objemom a celkovou cenou zobrazime v tabe
            double buildingPrice_PSM = fBuildingPrice_WithoutGST / fBuildingArea_Gross;
            double buildingPrice_PCM = fBuildingPrice_WithoutGST / fBuildingVolume_Gross;







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
            // TO Ondrej - myslim ze tieto captions sa z datatable nepreberaju do datagrid
            // Skusil som to nastavit priamo pre datagrid, ale neuspesne lebo sa to tam nastavuje ako itemsource takze samotny datagrid nema column
            // Tento problem mame skoro vo vsetkych tabulkach, nezobrazujeme pre nazvy stlpcov formatovane texty s medzerami, ale zdrojovy nazov stlpca z kodu

            // TODO Ondrej - zakazat sortovanie v stlpci gridu pre vsetky taketo datagridy s vysledkami a podobne.

            //dt.Columns["Crsc"].Caption = "Cross-section";
            //dt.Columns["Count"].Caption = "Count";
            //dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            //dt.Columns["UnitMass"].Caption = "Unit Mass\t [kg/m]";
            //dt.Columns["TotalMass"].Caption = "Total Mass\t [kg]";
            //dt.Columns["UnitPrice"].Caption = "Unit Price\t [NZD/m]";
            //dt.Columns["Price"].Caption = "Price\t [NZD]";

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

            fBuildingPrice_WithoutGST += SumTotalPrice;

            // Last row
            row = dt.NewRow();
            row["Crsc"] = "Total:";
            row["Count"] =SumCount.ToString();
            row["TotalLength"] = SumTotalLength.ToString("F2");
            row["UnitMass"] = "";
            row["TotalMass"] = SumTotalMass.ToString("F2");
            row["UnitPrice"] = "";
            row["Price"] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);

            Datagrid_Members.ItemsSource = ds.Tables[0].AsDataView();

            //styling
            
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

            // TODO Ondrej - potrebujeme nacitat parametre podla mena v comboboxe v Tabe General zluceneho s hrubkou

            // TO Ondrej - pre napojenie a nacitanie parametrov plechu cladding je mozne pouzit
            // DATABASE.CTrapezoidalSheetingManager
            // Bude potrebne nacitavat parametre pre farbu a potom pre nazov profilu a hrubku z inych tabuliek
            // K dispozicii su v databaze tabulka pre vsetky typy cladding a vsetky hrubky ale aj samostatne tabulky podla typov

            /*
            SmartDek-0.40 mm
            SmartDek-0.55 mm
            PurlinDek-0.40 mm
            PurlinDek-0.55 mm
            PurlinDek-0.75 mm
            SpeedClad-0.40 mm
            SpeedClad-0.55 mm
            SpeedClad-0.75 mm
            */

            CTS_CrscProperties prop_RoofCladding = new CTS_CrscProperties();
            prop_RoofCladding = CTrapezoidalSheetingManager.GetSectionProperties("PurlinDek-0.55 mm"); // TODO Ondrej

            CTS_CrscProperties prop_WallCladding = new CTS_CrscProperties();
            prop_WallCladding = CTrapezoidalSheetingManager.GetSectionProperties("SmartDek-0.40 mm"); // TODO Ondrej

            float fRoofCladdingPrice_PSM_NZD = (float)prop_RoofCladding.price_PPSM_NZD; // Cena roof cladding za 1 m^2 // TODO - zapracovat do databazy
            float fWallCladdingPrice_PSM_NZD = (float)prop_WallCladding.price_PPSM_NZD; ; // Cena wall cladding za 1 m^2 // TODO - zapracovat do databazy

            // TODO Ondrej
            // Zobrazit Datagrid s 2 riadkami - wall cladding a roof cladding - zobrazit nazov, hrubku, farbu (vid UC_General), celkovu plochu, cenu za meter stvorcovy a celkovu cenu
            //  !!!!! Cisla v datagridoch zarovnavat napravo (rovnaky pocet desatinnych miest aby boli desatinne  ciarky pod sebou

            // Cladding  | Thickness | Color   | Total Area | Price PSM | Total Price
            // PurlinDek | 0.75 mm   | Titania | 324.4      | 5.20      | Total Price
            // SmartDek  | 0.55 mm   | Black   | 245.9      | 4.20      | Total Price
            float fRoofCladdingPrice_Total_NZD = fRoofArea_Total_Netto * fRoofCladdingPrice_PSM_NZD; // TODO Ondrej
            float fWallCladdingPrice_Total_NZD = fWallArea_Total_Netto * fWallCladdingPrice_PSM_NZD; // TODO Ondrej

            // Create Table
            DataTable dt = new DataTable("TableCladding");
            // Create Table Rows
            dt.Columns.Add("Cladding", typeof(String));
            dt.Columns.Add("Thickness", typeof(String));
            dt.Columns.Add("Color", typeof(String));
            //dt.Columns.Add("TotalLength", typeof(String)); // Dalo by sa spocitat ak podelime plochu sirkou profilu
            dt.Columns.Add("TotalArea", typeof(String));
            dt.Columns.Add("UnitMass", typeof(String)); // kg / m^2
            dt.Columns.Add("TotalMass", typeof(String));
            dt.Columns.Add("UnitPrice", typeof(String));
            dt.Columns.Add("Price", typeof(String));

            // Set Column Caption
            // TO Ondrej - myslim ze tieto captions sa z datatable nepreberaju do datagrid
            // Skusil som to nastavit priamo pre datagrid, ale neuspesne lebo sa to tam nastavuje ako itemsource takze samotny datagrid nema column
            // Tento problem mame skoro vo vsetkych tabulkach, nezobrazujeme pre nazvy stlpcov formatovane texty s medzerami, ale zdrojovy nazov stlpca z kodu

            // TODO Ondrej - zakazat sortovanie v stlpci gridu pre vsetky taketo datagridy s vysledkami a podobne.

            dt.Columns["Cladding"].Caption = "Cladding";
            dt.Columns["Thickness"].Caption = "Thickness\t [mm]";
            dt.Columns["Color"].Caption = "Color";
            //dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            dt.Columns["TotalArea"].Caption = "Total Area\t [m2]";
            dt.Columns["UnitMass"].Caption = "Unit Mass\t [kg/m2]";
            dt.Columns["TotalMass"].Caption = "Total Mass\t [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price\t [NZD/m2]";
            dt.Columns["Price"].Caption = "Price\t [NZD]";

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
                    // TO Ondrej - pre napojenie a nacitanie parametrov plechu cladding je mozne pouzit
                    // DATABASE.CTrapezoidalSheetingManager
                    // Bude potrebne nacitavat parametre pre farbu a potom pre nazov profilu a hrubku z inych tabuliek
                    // K dispozicii su v databaze tabulka pre vsetky typy cladding a vsetky hrubky ale aj samostatne tabulky podla typov

                    row["Cladding"] = vm.RoofCladdingIndex; // TODO - napojit na GUI - Tab General
                    row["Thickness"] = vm.RoofCladdingThicknessIndex;
                    row["Color"] = vm.RoofCladdingColorIndex;

                    row["TotalArea"] = fRoofArea_Total_Netto.ToString("F2");
                    SumTotalArea += fRoofArea_Total_Netto;

                    row["UnitMass"] = fUnitMass.ToString("F2"); // Todo - napojit na databazu

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
                    // TO Ondrej - pre napojenie a nacitanie parametrov plechu cladding je mozne pouzit
                    // DATABASE.CTrapezoidalSheetingManager
                    // Bude potrebne nacitavat parametre pre farbu a potom pre nazov profilu a hrubku z inych tabuliek
                    // K dispozicii su v databaze tabulka pre vsetky typy cladding a vsetky hrubky ale aj samostatne tabulky podla typov

                    row["Cladding"] = vm.WallCladdingIndex; // TODO - napojit na GUI - Tab General
                    row["Thickness"] = vm.WallCladdingThicknessIndex;
                    row["Color"] = vm.WallCladdingColorIndex;

                    row["TotalArea"] = fWallArea_Total_Netto.ToString("F2");
                    SumTotalArea += fWallArea_Total_Netto;

                    row["UnitMass"] = fUnitMass.ToString("F2"); // Todo - napojit na databazu

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
                fBuildingPrice_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row["Cladding"] = "Total:";
                row["Thickness"] = "";
                row["Color"] = "";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row["TotalArea"] = SumTotalArea.ToString("F2");
                row["UnitMass"] = "";
                row["TotalMass"] = SumTotalMass.ToString("F2");
                row["UnitPrice"] = "";
                row["Price"] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_Cladding.ItemsSource = ds.Tables[0].AsDataView();
            }
        }

        private void CreateTableFibreglass(CPFDViewModel vm,
            float fFibreGlassArea_Roof,
            float fFibreGlassArea_Walls)
            {
            // Zobrazit Datagrid s 2 riadkami
            // FibreGlass Roof | Total Area | Price PSM | Total Price
            // FibreGlass Walls | Total Area | Price PSM | Total Price

            float fRoofFibreGlassPrice_PSM_NZD = 10.40f; // Cena roof fibreglass za 1 m^2 // TODO - zapracovat do databazy
            float fWallFibreGlassPrice_PSM_NZD = 9.10f; // Cena wall fibreglass za 1 m^2 // TODO - zapracovat do databazy

            float fRoofFibreGlassPrice_Total_NZD = fFibreGlassArea_Roof * fRoofFibreGlassPrice_PSM_NZD; // TODO Ondrej
            float fWallFibreGlassPrice_Total_NZD = fFibreGlassArea_Walls * fWallFibreGlassPrice_PSM_NZD; // TODO Ondrej

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
            // TO Ondrej - myslim ze tieto captions sa z datatable nepreberaju do datagrid
            // Skusil som to nastavit priamo pre datagrid, ale neuspesne lebo sa to tam nastavuje ako itemsource takze samotny datagrid nema column
            // Tento problem mame skoro vo vsetkych tabulkach, nezobrazujeme pre nazvy stlpcov formatovane texty s medzerami, ale zdrojovy nazov stlpca z kodu

            // TODO Ondrej - zakazat sortovanie v stlpci gridu pre vsetky taketo datagridy s vysledkami a podobne.

            dt.Columns["Fibreglass"].Caption = "Fibreglass";
            //dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            dt.Columns["TotalArea"].Caption = "Total Area\t [m2]";
            dt.Columns["UnitMass"].Caption = "Unit Mass\t [kg/m2]";
            dt.Columns["TotalMass"].Caption = "Total Mass\t [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price\t [NZD/m2]";
            dt.Columns["Price"].Caption = "Price\t [NZD]";

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
                        0.2, // TODO - database
                        0.2 * fFibreGlassArea_Roof,
                        fRoofFibreGlassPrice_PSM_NZD,
                        fRoofFibreGlassPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddSurfaceItemRow(dt,
                        "Fibreglass",
                        "Wall Fibreglass",
                        fFibreGlassArea_Walls,
                        0.2, // TODO - database
                        0.2 * fFibreGlassArea_Walls,
                        fWallFibreGlassPrice_PSM_NZD,
                        fWallFibreGlassPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            DataRow row;
            if (SumTotalPrice > 0)
            {
                fBuildingPrice_WithoutGST += SumTotalPrice;

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
            }
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
            float fRoofRidgeFlashingPrice_PLM_NZD = 3.90f; // Cena roof ridge flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fWallCornerFlashingPrice_PLM_NZD = 2.90f; // Cena corner flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fBargeFlashingPrice_PLM_NZD = 3.90f; // Cena barge flashing za 1 m dlzky // TODO - zapracovat do databazy

            float fRollerDoorTrimmerFlashingPrice_PLM_NZD = 3.90f; // Cena roller door trimmer flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fRollerDoorLintelFlashingPrice_PLM_NZD = 3.80f; // Cena roller door lintel flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fRollerDoorLintelCapFlashingPrice_PLM_NZD = 1.90f; // Cena cap flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fPADoorTrimmerFlashingPrice_PLM_NZD = 1.90f; // Cena PA door trimmer flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fPADoorLintelFlashingPrice_PLM_NZD = 1.80f; // Cena PA door lintel flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fWindowFlashingPrice_PLM_NZD = 1.90f; // Cena window flashing za 1 m dlzky // TODO - zapracovat do databazy

            // TODO Ondrej
            // Zobrazit Datagrid
            // Flashing | Total Length | Price PLM | Total Price
            // Roof Ridge Flashing | 41.12 | 3.90 | Total Price

            float fRoofRidgeFlashingPrice_Total_NZD = model.fL_tot * fRoofRidgeFlashingPrice_PLM_NZD; // TODO Ondrej
            float fWallCornerFlashingPrice_Total_NZD = 4 * model.fH1_frame * fWallCornerFlashingPrice_PLM_NZD; // TODO Ondrej
            float fBargeFlashingPrice_Total_NZD = 4 * fRoofSideLength * fBargeFlashingPrice_PLM_NZD; // TODO Ondrej

            float fRollerDoorTrimmerFlashingPrice_Total_NZD = fRollerDoorTrimmerFlashing_TotalLength * fRollerDoorTrimmerFlashingPrice_PLM_NZD; // TODO Ondrej
            float fRollerDoorLintelFlashingPrice_Total_NZD = fRollerDoorLintelFlashing_TotalLength * fRollerDoorLintelFlashingPrice_PLM_NZD; // TODO Ondrej
            float fRollerDoorLintelCapFlashingPrice_Total_NZD = fRollerDoorLintelCapFlashing_TotalLength * fRollerDoorLintelCapFlashingPrice_PLM_NZD; // TODO Ondrej
            float fPADoorTrimmerFlashingPrice_Total_NZD = fPADoorTrimmerFlashing_TotalLength * fPADoorTrimmerFlashingPrice_PLM_NZD; // TODO Ondrej
            float fPADoorLintelFlashingPrice_Total_NZD = fPADoorLintelFlashing_TotalLength * fPADoorLintelFlashingPrice_PLM_NZD; // TODO Ondrej
            float fWindowFlashingPrice_Total_NZD = fWindowFlashing_TotalLength * fWindowFlashingPrice_PLM_NZD; // TODO Ondrej

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
            // TO Ondrej - myslim ze tieto captions sa z datatable nepreberaju do datagrid
            // Skusil som to nastavit priamo pre datagrid, ale neuspesne lebo sa to tam nastavuje ako itemsource takze samotny datagrid nema column
            // Tento problem mame skoro vo vsetkych tabulkach, nezobrazujeme pre nazvy stlpcov formatovane texty s medzerami, ale zdrojovy nazov stlpca z kodu

            dt.Columns["Flashing"].Caption = "Flashing";
            dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            dt.Columns["UnitMass"].Caption = "Unit Mass\t [kg/m]";
            dt.Columns["TotalMass"].Caption = "Total Mass\t [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price\t [NZD/m]";
            dt.Columns["Price"].Caption = "Price\t [NZD]";

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
                        0.2, // TODO - database
                        0.2 * model.fL_tot,
                        fRoofRidgeFlashingPrice_PLM_NZD,
                        fRoofRidgeFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Wall Corner Flashing",
                        4 * model.fH1_frame,
                        0.2, // TODO - database
                        0.2 * 4 * model.fH1_frame,
                        fWallCornerFlashingPrice_PLM_NZD,
                        fWallCornerFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Barge Flashing",
                        4 * fRoofSideLength,
                        0.2, // TODO - database
                        0.2 * 4 * fRoofSideLength,
                        fBargeFlashingPrice_PLM_NZD,
                        fBargeFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roller Door Trimmer Flashing",
                        fRollerDoorTrimmerFlashing_TotalLength,
                        0.2, // TODO - database
                        0.2 * fRollerDoorTrimmerFlashing_TotalLength,
                        fRollerDoorTrimmerFlashingPrice_PLM_NZD,
                        fRollerDoorTrimmerFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roller Door Lintel Flashing",
                        fRollerDoorLintelFlashing_TotalLength,
                        0.2, // TODO - database
                        0.2 * fRollerDoorLintelFlashing_TotalLength,
                        fRollerDoorLintelFlashingPrice_PLM_NZD,
                        fRollerDoorLintelFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Roller Door Lintel Cap Flashing",
                        fRollerDoorLintelCapFlashing_TotalLength,
                        0.2, // TODO - database
                        0.2 * fRollerDoorLintelCapFlashing_TotalLength,
                        fRollerDoorLintelCapFlashingPrice_PLM_NZD,
                        fRollerDoorLintelCapFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "PA Door Trimmer Flashing",
                        fPADoorTrimmerFlashing_TotalLength,
                        0.2, // TODO - database
                        0.2 * fPADoorTrimmerFlashing_TotalLength,
                        fPADoorTrimmerFlashingPrice_PLM_NZD,
                        fPADoorTrimmerFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "PA Door Lintel Flashing",
                        fPADoorLintelFlashing_TotalLength,
                        0.2, // TODO - database
                        0.2 * fPADoorLintelFlashing_TotalLength,
                        fPADoorLintelFlashingPrice_PLM_NZD,
                        fPADoorLintelFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddLengthItemRow(dt,
                        "Flashing",
                        "Window Flashing",
                        fWindowFlashing_TotalLength,
                        0.2, // TODO - database
                        0.2 * fWindowFlashing_TotalLength,
                        fWindowFlashingPrice_PLM_NZD,
                        fWindowFlashingPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            fBuildingPrice_WithoutGST += SumTotalPrice;

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
        }

        private void CreateTableGutters(CModel model)
        {
            float fGuttersTotalLength = 2 * model.fL_tot; // na 2 okrajoch strechy
            float fRoofGutterPrice_PLM_NZD = 2.20f; // Cena roof gutter za 1 m dlzky // TODO - zapracovat do databazy podla sirok

            // TODO Ondrej
            // Zobrazit Datagrid
            // Roof Gutter | Total Length | Price PLM | Total Price
            float fGuttersPrice_Total_NZD = fGuttersTotalLength * fRoofGutterPrice_PLM_NZD; // TODO Ondrej

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
            // TO Ondrej - myslim ze tieto captions sa z datatable nepreberaju do datagrid
            // Skusil som to nastavit priamo pre datagrid, ale neuspesne lebo sa to tam nastavuje ako itemsource takze samotny datagrid nema column
            // Tento problem mame skoro vo vsetkych tabulkach, nezobrazujeme pre nazvy stlpcov formatovane texty s medzerami, ale zdrojovy nazov stlpca z kodu

            dt.Columns["Gutter"].Caption = "Gutter";
            dt.Columns["TotalLength"].Caption = "Total Length\t [m]";
            dt.Columns["UnitMass"].Caption = "Unit Mass\t [kg/m]";
            dt.Columns["TotalMass"].Caption = "Total Mass\t [kg]";
            dt.Columns["UnitPrice"].Caption = "Unit Price\t [NZD/m]";
            dt.Columns["Price"].Caption = "Price\t [NZD]";

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
                        0.2, // TODO - database
                        0.2 * fGuttersTotalLength,
                        fRoofGutterPrice_PLM_NZD,
                        fGuttersPrice_Total_NZD,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            fBuildingPrice_WithoutGST += SumTotalPrice;

            //if (dt.Rows.Count > 1) // Len ak su v tabulke rozne typy gutters // Komentujem, inak by bol problem spocitat celkovu sumu
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
    }
}
