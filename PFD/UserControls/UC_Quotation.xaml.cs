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
        double dBuildingNetPrice_WithoutMargin_WithoutGST = 0;

        const float fCFS_PricePerKg_Plates_Material = 1.698f;    // NZD / kg
        const float fCFS_PricePerKg_Plates_Manufacture = 0.0f;   // NZD / kg
        const float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

        // Plate price - general plate price

        // 3 mm - 40.0 NZD / m^2
        // 2 mm - 27.0 NZD / m^2
        // 1 mm - 14.0 NZD / m^2

        float fTotalAreaOfOpennings = 0;

        float fRollerDoorTrimmerFlashing_TotalLength = 0;
        float fRollerDoorLintelFlashing_TotalLength = 0;
        float fRollerDoorLintelCapFlashing_TotalLength = 0;
        float fPADoorTrimmerFlashing_TotalLength = 0;
        float fPADoorLintelFlashing_TotalLength = 0;
        float fWindowFlashing_TotalLength = 0;

        private CPFDViewModel _pfdVM;

        public UC_Quotation(CPFDViewModel vm)
        {
            InitializeComponent();
            _pfdVM = vm;

            CModel model = vm.Model;

            // DG 1
            // Members
            if (vm._quotationDisplayOptionsVM.DisplayMembers) CreateTableMembers(model);
            else
            {
                TextBlock_Members.Visibility = Visibility.Collapsed;
                Datagrid_Members.Visibility = Visibility.Collapsed;
            }

            // DG 2
            // Plates
            // Washers
            if (vm._quotationDisplayOptionsVM.DisplayPlates) CreateTablePlates(model);
            else
            {
                TextBlock_Plates.Visibility = Visibility.Collapsed;
                Datagrid_Plates.Visibility = Visibility.Collapsed;
            }
            // TODO - dopracovat apex brace plates

            // DG 3
            // Screws
            // Bolts
            // Anchors
            if (vm._quotationDisplayOptionsVM.DisplayConnectors) CreateTableConnectors(model);
            else
            {
                TextBlock_Connectors.Visibility = Visibility.Collapsed;
                Datagrid_Connectors.Visibility = Visibility.Collapsed;
            }

            // DG 4
            // Bolt Nuts
            if (vm._quotationDisplayOptionsVM.DisplayBoltNuts) CreateTableBoltNuts(model);
            else
            {
                TextBlock_BoltNuts.Visibility = Visibility.Collapsed;
                Datagrid_BoltNuts.Visibility = Visibility.Collapsed;
            }

            // DG 5
            // Doors and windows
            
            if (vm._quotationDisplayOptionsVM.DisplayDoorsAndWindows) CreateTableDoorsAndWindows(vm);
            else
            {
                TextBlock_DoorsAndWindows.Visibility = Visibility.Collapsed;
                Datagrid_DoorsAndWindows.Visibility = Visibility.Collapsed;
            }

            // DG 6
            // Cladding
            // Canopies
            float fFibreGlassArea_Roof = vm._claddingOptionsVM.FibreglassAreaRoofRatio / 100f * vm.TotalRoofArea; // Priesvitna cast strechy (bez canopies)
            float fFibreGlassArea_Walls = vm._claddingOptionsVM.FibreglassAreaWallRatio / 100f * vm.TotalWallArea; // Priesvitna cast stien

            if (QuotationHelper.DisplayCladdingTable(vm)) //iba ak je nejaky cladding
            {
                // TODO Ondrej - refaktoring - funckia CreateTableCladding
                //TO Mato - je ten koment hore aktualny?
                // TODO - rozdelit riadky pre basic roof a canopies ???

                // fTotalAreaOfOpennings - TODO - Treba zarucit ze tato premenna sa naplni plochou vsetkych doors a windows na stenach skor nez sa zavola CreateTableCladding

                CreateTableCladding(vm, fTotalAreaOfOpennings, fFibreGlassArea_Walls, fFibreGlassArea_Roof);
            }
            else
            {
                TextBlock_Cladding.Visibility = Visibility.Collapsed;
                Datagrid_Cladding.Visibility = Visibility.Collapsed;
            }

            // DG 7
            // Gutters
            if (vm._quotationDisplayOptionsVM.DisplayGutters && QuotationHelper.DisplayGuttersTable(vm) &&
                _pfdVM._doorsAndWindowsVM != null && _pfdVM._doorsAndWindowsVM.Gutters.Count > 0) CreateTableGutters(vm);
            else
            {
                TextBlock_Gutters.Visibility = Visibility.Collapsed;
                Datagrid_Gutters.Visibility = Visibility.Collapsed;
            }

            // DG 8
            // Downpipes
            if (vm._quotationDisplayOptionsVM.DisplayDownpipe && QuotationHelper.DisplayDownpipesTable(vm) &&
                vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.Downpipes.Count > 0) CreateTableDownpipes(vm);
            else
            {
                TextBlock_Downpipes.Visibility = Visibility.Collapsed;
                Datagrid_Downpipes.Visibility = Visibility.Collapsed;
            }

            // DG 9
            // FibreGlass
            if (QuotationHelper.DisplayFibreglassTable(vm))
                CreateTableFibreglass(vm, fFibreGlassArea_Roof, fFibreGlassArea_Walls);
            else
            {
                TextBlock_Fibreglass.Visibility = Visibility.Collapsed;
                Datagrid_Fibreglass.Visibility = Visibility.Collapsed;
            }

            // DG 10
            // Roof Netting
            if (vm._quotationDisplayOptionsVM.DisplayRoofNetting && QuotationHelper.DisplayRoofNettingTable(vm)) CreateTableRoofNetting(vm.TotalRoofAreaInclCanopies);
            else
            {
                TextBlock_RoofNetting.Visibility = Visibility.Collapsed;
                Datagrid_RoofNetting.Visibility = Visibility.Collapsed;
            }

            // DG 11
            // Flashing and Packers
            if (vm._quotationDisplayOptionsVM.DisplayFlashing && QuotationHelper.DisplayFlashingsTable(vm) && vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.Flashings.Count > 0)
            {
                //CreateTableFlashing(
                //    fRollerDoorTrimmerFlashing_TotalLength,
                //    fRollerDoorLintelFlashing_TotalLength,
                //    fRollerDoorLintelCapFlashing_TotalLength,
                //    fPADoorTrimmerFlashing_TotalLength,
                //    fPADoorLintelFlashing_TotalLength,
                //    fWindowFlashing_TotalLength);
                CreateTableFlashing();
            }
            else
            {
                TextBlock_Flashing.Visibility = Visibility.Collapsed;
                Datagrid_Flashing.Visibility = Visibility.Collapsed;
            }

            // DG 12
            // Packers
            if (vm._quotationDisplayOptionsVM.DisplayPackers && QuotationHelper.DisplayPackersTable(vm))
            {
                CreateTablePackers(fRollerDoorLintelFlashing_TotalLength);
            }
            else
            {
                TextBlock_Packers.Visibility = Visibility.Collapsed;
                Datagrid_Packers.Visibility = Visibility.Collapsed;
            }

            // Cladding Accessories
            if (vm._quotationDisplayOptionsVM.DisplayCladdingAccesories && QuotationHelper.DisplayCladdingAccesoriesTable(vm))
            {
                CreateTableCladdingAccessories(vm);
            }
            else
            {
                TxtCladdingAccessories.Visibility = Visibility.Collapsed;
                Datagrid_CladdingAccessories_Items_Length.Visibility = Visibility.Collapsed;
                Datagrid_CladdingAccessories_Items_Piece.Visibility = Visibility.Collapsed;
            }

            // Vysledne hodnoty a sumy spolu s plochou, objemom a celkovou cenou zobrazime v tabe

            double dMarginPercentage = 40; // Default
            double dGST_Percentage = 15;   // Default
            double dFreight = 0;           // Default

            double dMarginAbsolute,
                   dMarkupAbsolute,
                   dMarkupPercentage,
                   buildingPrice_WithMargin_WithoutGST,
                   buildingPrice_PSM,
                   buildingPrice_PCM,
                   buildingPrice_PPKG,
                   dGST_Absolute,
                   dTotalBuildingPrice_IncludingGST;

            // Calculate prices
            CalculatePrices(
                   vm.BuildingArea_Gross, // fBuildingArea_Gross,
                   vm.BuildingVolume_Gross, // fBuildingVolume_Gross,
                   dBuildingMass,
                   dMarginPercentage,
                   dGST_Percentage,
                   dFreight,
                   out dMarginAbsolute,
                   out dMarkupAbsolute,
                   out dMarkupPercentage,
                   out buildingPrice_WithMargin_WithoutGST,
                   out buildingPrice_PSM,
                   out buildingPrice_PCM,
                   out buildingPrice_PPKG,
                   out dGST_Absolute,
                   out dTotalBuildingPrice_IncludingGST
                   );

            // Set view model values
            QuotationViewModel qVM = new QuotationViewModel
            {
                BuildingNetPrice_WithoutMargin_WithoutGST = dBuildingNetPrice_WithoutMargin_WithoutGST,
                Margin_Percentage = dMarginPercentage,
                Margin_Absolute = dMarginAbsolute,
                Markup_Percentage = dMarkupPercentage,
                Markup_Absolute = dMarkupAbsolute,
                BuildingPrice_WithMargin_WithoutGST = buildingPrice_WithMargin_WithoutGST,
                GST_Percentage = dGST_Percentage,
                GST_Absolute = dGST_Absolute,
                TotalBuildingPrice_IncludingGST = dTotalBuildingPrice_IncludingGST,
                BuildingArea_Gross = vm.BuildingArea_Gross, // fBuildingArea_Gross,
                BuildingVolume_Gross = vm.BuildingVolume_Gross, // fBuildingVolume_Gross,
                BuildingMass = dBuildingMass,
                BuildingPrice_PSM = buildingPrice_PSM,
                BuildingPrice_PCM = buildingPrice_PCM,
                BuildingPrice_PPKG = buildingPrice_PPKG
            };

            qVM.PropertyChanged += QVM_PropertyChanged;
            this.DataContext = qVM;

            vm._quotationViewModel = qVM;

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

        private void QVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Margin_Percentage" || e.PropertyName == "GST_Percentage" || e.PropertyName == "Freight")
            {
                QuotationViewModel vm = sender as QuotationViewModel;

                double dMarginAbsolute,
                       dMarkupAbsolute,
                       dMarkupPercentage,
                       buildingPrice_WithMargin_WithoutGST,
                       buildingPrice_PSM,
                       buildingPrice_PCM,
                       buildingPrice_PPKG,
                       dGST_Absolute,
                       dTotalBuildingPrice_IncludingGST;

                // Calculate prices
                CalculatePrices(
                       vm.BuildingArea_Gross,
                       vm.BuildingVolume_Gross,
                       vm.BuildingMass,
                       vm.Margin_Percentage,
                       vm.GST_Percentage,
                       vm.Freight,
                       out dMarginAbsolute,
                       out dMarkupAbsolute,
                       out dMarkupPercentage,
                       out buildingPrice_WithMargin_WithoutGST,
                       out buildingPrice_PSM,
                       out buildingPrice_PCM,
                       out buildingPrice_PPKG,
                       out dGST_Absolute,
                       out dTotalBuildingPrice_IncludingGST
                       );

                // Set view model output values
                vm.Margin_Absolute = dMarginAbsolute;
                vm.Markup_Absolute = dMarkupAbsolute;
                vm.Markup_Percentage = dMarkupPercentage;
                vm.BuildingPrice_WithMargin_WithoutGST = buildingPrice_WithMargin_WithoutGST;
                vm.GST_Absolute = dGST_Absolute;
                vm.TotalBuildingPrice_IncludingGST = dTotalBuildingPrice_IncludingGST;
                vm.BuildingPrice_PSM = buildingPrice_PSM;
                vm.BuildingPrice_PCM = buildingPrice_PCM;
                vm.BuildingPrice_PPKG = buildingPrice_PPKG;
            }
        }

        private void CalculatePrices(
            float fBuildingArea_Gross,
            float fBuildingVolume_Gross,
            double dBuildingMass,
            double dMarginPercentage,
            double dGST_Percentage,
            double dFreight,
            out double dMarginAbsolute,
            out double dMarkupAbsolute,
            out double dMarkupPercentage,
            out double buildingPrice_WithMargin_WithoutGST,
            out double buildingPrice_PSM,
            out double buildingPrice_PCM,
            out double buildingPrice_PPKG,
            out double dGST_Absolute,
            out double dTotalBuildingPrice_IncludingGST
            )
        {
            // Margin
            // Marža = (predajná cena – nákupná cena) / predajná cena * 100(vyjadrené v %)
            buildingPrice_WithMargin_WithoutGST = dBuildingNetPrice_WithoutMargin_WithoutGST / ((100f - dMarginPercentage) / 100f);
            dMarginAbsolute = buildingPrice_WithMargin_WithoutGST - dBuildingNetPrice_WithoutMargin_WithoutGST; // Podiel z koncovej (predajnej) ceny - marza

            // Markup
            // Prirážka = (predajná cena – nákupná cena) / nákupná cena * 100 (vyjadrené v %)
            dMarkupAbsolute = buildingPrice_WithMargin_WithoutGST - dBuildingNetPrice_WithoutMargin_WithoutGST;
            dMarkupPercentage = dMarkupAbsolute / dBuildingNetPrice_WithoutMargin_WithoutGST * 100f; // Podiel voci zakladnej (nakupnej) cene - prirazka
            double dMarkupPercentage_check = (dMarginPercentage / (100f - dMarginPercentage)) * 100f;

            if (!MathF.d_equal(dMarkupPercentage, dMarkupPercentage_check))
                throw new Exception("Error in the markup calculation.");

            // Building Unit Price
            buildingPrice_PSM = fBuildingArea_Gross > 0 ? buildingPrice_WithMargin_WithoutGST / fBuildingArea_Gross : 0;
            buildingPrice_PCM = fBuildingVolume_Gross > 0 ? buildingPrice_WithMargin_WithoutGST / fBuildingVolume_Gross : 0;
            buildingPrice_PPKG = dBuildingMass > 0 ? buildingPrice_WithMargin_WithoutGST / dBuildingMass : 0;

            //predpokladam,ze cenu za dopravu treba priratat az nakoniec a neriesit tam ani Margin ani Markup
            buildingPrice_WithMargin_WithoutGST += dFreight;

            dGST_Absolute = dGST_Percentage / 100f * buildingPrice_WithMargin_WithoutGST;
            dTotalBuildingPrice_IncludingGST = buildingPrice_WithMargin_WithoutGST + dGST_Absolute;
        }

        private void CreateTableMembers(CModel model)
        {
            // Create Table
            DataTable dt = new DataTable("Members");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_CrossSection.ColumnName, QuotationHelper.colProp_CrossSection.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalLength_m.ColumnName, QuotationHelper.colProp_TotalLength_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            int SumCount = 0;
            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;
            foreach (CCrSc crsc in model.m_arrCrSc.Values.GroupBy(m => m.Name_short).Select(g => g.First()))
            {   
                List<CMember> membersList = model.GetListOfMembersWithCrscDatabaseID(crsc.DatabaseID);
                int count = membersList.Where(m => m.BIsGenerated && m.BIsSelectedForMaterialList).Count();

                if (count > 0) row = dt.NewRow();
                else continue;

                double totalLength = membersList.Where(m => m.BIsGenerated && m.BIsSelectedForMaterialList).Sum(m => m.FLength_real);
                double totalMass = (crsc.A_g * GlobalConstants.MATERIAL_DENSITY_STEEL * totalLength);
                double totalPrice = totalLength * crsc.price_PPLM_NZD;

                try
                {
                    row[QuotationHelper.colProp_CrossSection.ColumnName] = crsc.Name_short;

                    row[QuotationHelper.colProp_Count.ColumnName] = count.ToString();
                    SumCount += count;

                    row[QuotationHelper.colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = (crsc.A_g * GlobalConstants.MATERIAL_DENSITY_STEEL).ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = crsc.price_PPLM_NZD.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = totalPrice.ToString("F2");
                    SumTotalPrice += totalPrice;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            // Last row
            row = dt.NewRow();
            row[QuotationHelper.colProp_CrossSection.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = SumCount.ToString();
            row[QuotationHelper.colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
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
            DataSet ds = QuotationHelper.GetTablePlates(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_Plates.Loaded += Datagrid_Plates_Loaded;
        }

        private void Datagrid_Plates_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Plates);
        }

        private void CreateTableConnectors(CModel model)
        {
            DataSet ds = QuotationHelper.GetTableConnectors(model, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        }

        private void Datagrid_Connectors_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Connectors);
        }

        private void CreateTableBoltNuts(CModel model)
        {
            // Bolt nuts
            List<QuotationItem> quotation = new List<QuotationItem>();

            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                model.m_arrConnectionJoints[i].SetJointIsSelectedForMaterialListAccordingToMember();
                if (!model.m_arrConnectionJoints[i].BIsSelectedForMaterialList) continue;
                if (model.m_arrConnectionJoints[i].m_arrPlates == null || model.m_arrConnectionJoints[i].m_arrPlates.Length == 0) continue; // TODO - vylepsit... Zatial nemame ine spoje, ktore obsahuju bolts alebo anchors a neobsahuju plates

                for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
                {
                    // Anchors
                    if (model.m_arrConnectionJoints[i].m_arrPlates[j] is CConCom_Plate_B_basic)
                    {
                        CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)model.m_arrConnectionJoints[i].m_arrPlates[j];

                        if (plate.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                        {
                            CAnchor anchor = plate.AnchorArrangement.Anchors.FirstOrDefault();
                            int anchorsNum = plate.AnchorArrangement.Anchors.Length;
                            //v pripade ak su anchor.Nuts stale rovnake tak netreba foreach ale len quantity = anchorsNum * anchor.Nuts.Count
                            // TO Ondrej  - na 90 % su rovnake, teoreticky by mohol niekto mat hornu maticu nejaku specialnu inu ako spodne zabetonovane v betone, priemer musi byt rovnaky
                            if (anchor.Nuts != null)
                            {
                                foreach (CNut nut in anchor.Nuts)
                                {
                                    AddBoltNutToQuotation(nut, quotation, anchorsNum);
                                }
                            }
                        }
                    }
                }
            }

            double dTotalNutsMass_Table = 0;
            double dTotalNutsPrice_Table = 0;
            int iTotalNutsNumber_Table = 0;
            foreach (QuotationItem item in quotation)
            {
                dTotalNutsMass_Table += item.TotalMass;
                dTotalNutsPrice_Table += item.TotalPrice;
                iTotalNutsNumber_Table += item.Quantity;
            }

            dBuildingMass += dTotalNutsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalNutsPrice_Table;

            // Create Table
            DataTable dt = new DataTable("Bolt Nuts");

            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Prefix.ColumnName, QuotationHelper.colProp_Prefix.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Material.ColumnName, QuotationHelper.colProp_Material.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Size.ColumnName, QuotationHelper.colProp_Size.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);
            DataRow row;
            foreach (QuotationItem item in quotation)
            {
                row = dt.NewRow();
                try
                {
                    row[QuotationHelper.colProp_Prefix.ColumnName] = item.Prefix;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_Material.ColumnName] = item.MaterialName;
                    row[QuotationHelper.colProp_Size.ColumnName] = item.Name;
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            // Add Sum
            row = dt.NewRow();
            row[QuotationHelper.colProp_Prefix.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalNutsNumber_Table;
            row[QuotationHelper.colProp_Material.ColumnName] = "";
            row[QuotationHelper.colProp_Size.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalNutsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalNutsPrice_Table.ToString("F2");
            dt.Rows.Add(row);

            Datagrid_BoltNuts.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_BoltNuts.Loaded += Datagrid_BoltNuts_Loaded;
        }

        private void Datagrid_BoltNuts_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_BoltNuts);
        }

        private void AddBoltNutToQuotation(CNut nut, List<QuotationItem> quotation, int iQuantity)
        {
            QuotationItem qItem = quotation.FirstOrDefault(q => q.Name == nut.Name &&
                    MathF.d_equal(q.MassPerPiece, nut.Mass));
            //TO Mato - neviem na zaklade coho vsetkeho to treba groupovat???
            // TO Ondrej - podla priemeru (prefix M16)
            // Este ako tak na to pozeram, tak mozno zjednotime s Connectors, aby boli vsetky stlpce rovnake
            // Vymyslim nejaky prefix, z prefixu urobim size a doplnim ostatne stlpce aby boli rovnake ako ma tabulka Connectors

            if (qItem != null) //this quotation exists
            {
                qItem.Quantity += iQuantity;
                qItem.TotalMass = qItem.Quantity * qItem.MassPerPiece;
                qItem.TotalPrice = qItem.Quantity * qItem.PricePerPiece;
            }
            else //quotation item does not exist = add to collection
            {
                QuotationItem item = new QuotationItem
                {
                    Prefix = nut.Prefix,
                    Name = nut.Name,
                    Quantity = iQuantity,
                    MaterialName = nut.m_Mat.Name,
                    MassPerPiece = nut.Mass,
                    PricePerPiece = nut.Price_PPKG_NZD * nut.Mass,
                    TotalMass = iQuantity * nut.Mass,
                    TotalPrice = iQuantity * nut.Price_PPKG_NZD * nut.Mass
                };
                quotation.Add(item);
            }
        }

        private void CreateTableCladding(CPFDViewModel vm, float fTotalAreaOfOpennings, float fFibreGlassArea_Walls, float fFibreGlassArea_Roof)
        {
            DataSet ds = QuotationHelper.GetTableCladding(vm, fTotalAreaOfOpennings, fFibreGlassArea_Walls, fFibreGlassArea_Roof, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Cladding.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Cladding.Loaded += Datagrid_Cladding_Loaded;            
        }

        private void Datagrid_Cladding_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Cladding);
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

        private void CreateTableDoorsAndWindows(CPFDViewModel vm)
        {
            DataSet ds = QuotationHelper.GetTableDoorsAndWindows(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST, out fTotalAreaOfOpennings, 
                out fRollerDoorTrimmerFlashing_TotalLength, out fRollerDoorLintelFlashing_TotalLength, out fRollerDoorLintelCapFlashing_TotalLength, 
                out fPADoorTrimmerFlashing_TotalLength, out fPADoorLintelFlashing_TotalLength, out fWindowFlashing_TotalLength);

            if (ds != null)
            {
                Datagrid_DoorsAndWindows.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_DoorsAndWindows.Loaded += Datagrid_DoorsAndWindows_Loaded;
            }
            else // Tabulka je prazdna - nezobrazime ju
            {
                TextBlock_DoorsAndWindows.IsEnabled = false;
                TextBlock_DoorsAndWindows.Visibility = Visibility.Collapsed;

                Datagrid_DoorsAndWindows.IsEnabled = false;
                Datagrid_DoorsAndWindows.Visibility = Visibility.Collapsed;
            }
        }

        private void Datagrid_DoorsAndWindows_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_DoorsAndWindows);
        }

        private void CreateTableFibreglass(CPFDViewModel vm, float fFibreGlassArea_Roof, float fFibreGlassArea_Walls)
        {
            DataSet ds = QuotationHelper.GetTableFibreglass(vm, fFibreGlassArea_Roof, fFibreGlassArea_Walls, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            if (ds != null)
            {                
                Datagrid_Fibreglass.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_Fibreglass.Loaded += Datagrid_Fibreglass_Loaded;
            }
            else // Tabulka je prazdna - nezobrazime ju
            {
                TextBlock_Fibreglass.IsEnabled = false;
                TextBlock_Fibreglass.Visibility = Visibility.Collapsed;

                Datagrid_Fibreglass.IsEnabled = false;
                Datagrid_Fibreglass.Visibility = Visibility.Collapsed;
            }
        }
        private void Datagrid_Fibreglass_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Fibreglass);
        }

        private void CreateTableRoofNetting(float fRoofArea)
        {
            DataSet ds = QuotationHelper.GetTableRoofNetting(fRoofArea, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_RoofNetting.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_RoofNetting.Loaded += Datagrid_RoofNetting_Loaded;            
        }
        private void Datagrid_RoofNetting_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_RoofNetting);
        }

        private void CreateTableFlashing()
        {
            DataSet ds = QuotationHelper.GetTableFlashing(_pfdVM, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Flashing.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Flashing.Loaded += Datagrid_Flashing_Loaded;
        }

        private void Datagrid_Flashing_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Flashing);
        }

        private void CreateTablePackers(float fRollerDoorLintelFlashing_TotalLength)
        {
            DataSet ds = QuotationHelper.GetTablePackers(fRollerDoorLintelFlashing_TotalLength, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds != null)
                Datagrid_Packers.ItemsSource = ds.Tables[0].AsDataView();
            else
            {
                TextBlock_Packers.Visibility = Visibility.Collapsed;
                Datagrid_Packers.Visibility = Visibility.Collapsed;
            }
        }

        //private void Datagrid_Packers_Loaded(object sender, RoutedEventArgs e)
        //{
        //    SetLastRowBold(Datagrid_Flashing);
        //}

        private void CreateTableGutters(CPFDViewModel vm)
        {
            DataSet ds = QuotationHelper.GetTableGutters(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);

            Datagrid_Gutters.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Gutters.Loaded += Datagrid_Gutters_Loaded;
        }

        private void Datagrid_Gutters_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Gutters);
        }

        private void CreateTableDownpipes(CPFDViewModel vm)
        {
            DataSet ds = QuotationHelper.GetTableDownpipes(vm, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            
            Datagrid_Downpipes.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Downpipes.Loaded += Datagrid_Downpipes_Loaded;
        }

        private void Datagrid_Downpipes_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Downpipes);
        }

        private void CreateTableCladdingAccessories(CPFDViewModel vm)
        {
            List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece = null;
            List<CCladdingAccessories_Item_Length> claddingAccessoriesItems_Length = null;
            PartListHelper.GetTableCladdingAccessoriesLists(vm, out claddingAccessoriesItems_Piece, out claddingAccessoriesItems_Length);

            List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece_Quotation = QuotationHelper.GetQuotationCladdingAccessoriesItems_Piece(claddingAccessoriesItems_Piece);

            // Items - Length
            DataSet ds1 = PartListHelper.GetTableCladdingAccessories_Items_Length(claddingAccessoriesItems_Length, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds1 != null)
            {
                Datagrid_CladdingAccessories_Items_Length.ItemsSource = ds1.Tables[0].AsDataView();  //draw the table to datagridview
                Datagrid_CladdingAccessories_Items_Length.Loaded += Datagrid_CladdingAccessories_Items_Length_Loaded;
            }
            else
            {
                Datagrid_CladdingAccessories_Items_Length.Visibility = Visibility.Collapsed;
            }

            // Items - Piece            
            DataSet ds2 = PartListHelper.GetTableCladdingAccessories_Items_Piece(claddingAccessoriesItems_Piece_Quotation, ref dBuildingMass, ref dBuildingNetPrice_WithoutMargin_WithoutGST);
            if (ds2 != null)
            {
                Datagrid_CladdingAccessories_Items_Piece.ItemsSource = ds2.Tables[0].AsDataView();  //draw the table to datagridview
                Datagrid_CladdingAccessories_Items_Piece.Loaded += Datagrid_CladdingAccessories_Items_Piece_Loaded;
            }
            else
            {
                Datagrid_CladdingAccessories_Items_Piece.Visibility = Visibility.Collapsed;
            }

            if (ds1 == null && ds2 == null) TxtCladdingAccessories.Visibility = Visibility.Collapsed;
        }

        private void Datagrid_CladdingAccessories_Items_Length_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_CladdingAccessories_Items_Length);
        }
        private void Datagrid_CladdingAccessories_Items_Piece_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_CladdingAccessories_Items_Piece);
        }


        private void BtnDisplayOptions_Click(object sender, RoutedEventArgs e)
        {
            QuotationDisplayOptionsWindow window = new QuotationDisplayOptionsWindow(_pfdVM);
            window.ShowDialog();
        }

        private void ChangeGUIAccordingToDisplayOptions()
        {

        }

        private void btnRefreshQuotation_Click(object sender, RoutedEventArgs e)
        {
            _pfdVM.RecreateQuotation = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnFreightDetails_Click(object sender, RoutedEventArgs e)
        {
            //TODO open window

        }




































        //ODKALDAM SI PLATES METODU AK BY SOM V REFAKTORINGU NIECO DOBABRAL TAK odtialto sa vezme
        //private void CreateTablePlates(CModel model)
        //{
        //    float fCFS_PricePerKg_Plates_Material = 2.8f;      // NZD / kg
        //    float fCFS_PricePerKg_Plates_Manufacture = 2.0f;   // NZD / kg
        //    float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

        //    List<string> listPlatePrefix = new List<string>(1);
        //    List<int> listPlateCount = new List<int>(1);
        //    List<string> listPlateMaterialName = new List<string>(1);
        //    List<double> dlistPlateWidth_bx = new List<double>(1);
        //    List<double> dlistPlateHeight_hy = new List<double>(1);
        //    List<double> dlistPlateThickness_tz = new List<double>(1);
        //    List<double> dlistPlateArea = new List<double>(1);
        //    List<double> dlistPlateUnitMass = new List<double>(1);
        //    List<double> listPlateTotalArea = new List<double>(1);
        //    List<double> listPlateTotalMass = new List<double>(1);
        //    List<double> dlistPlatePricePerPiece = new List<double>(1);
        //    List<double> listPlateTotalPrice = new List<double>(1);

        //    List<string> listPlateWidth_bx = new List<string>(1);
        //    List<string> listPlateHeight_hy = new List<string>(1);
        //    List<string> listPlateThickness_tz = new List<string>(1);
        //    List<string> listPlateArea = new List<string>(1);
        //    List<string> listPlateMassPerPiece = new List<string>(1);
        //    List<string> listPlatePricePerPiece = new List<string>(1);
        //    // Plates

        //    List<CPlate> ListOfPlateGroups = new List<CPlate>();
        //    //System.Diagnostics.Trace.WriteLine("model.m_arrConnectionJoints.Count: " + model.m_arrConnectionJoints.Count);
        //    int count = 0;
        //    for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
        //    {
        //        model.m_arrConnectionJoints[i].BIsSelectedForMaterialList = CJointHelper.IsJointSelectedForMaterialList(model.m_arrConnectionJoints[i]);

        //        if (model.m_arrConnectionJoints[i].BIsSelectedForMaterialList)
        //        {
        //            count++;
        //            for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
        //            {
        //                // Nastavime parametre plechu z databazy - toto by sa malo diat uz asi pri vytvarani plechov
        //                // Nie vsetky plechy budu mat parametre definovane v databaze
        //                // !!!! Treba doriesit presne rozmery pri vytvarani plates a zaokruhlovanie

        //                #region Base Plate
        //                // Blok1 Plate START
        //                // ----------------------------------------------------------------------------------------------------------------------------------------
        //                try
        //                {
        //                    model.m_arrConnectionJoints[i].m_arrPlates[j].SetParams(model.m_arrConnectionJoints[i].m_arrPlates[j].Name, model.m_arrConnectionJoints[i].m_arrPlates[j].m_ePlateSerieType_FS);
        //                }
        //                catch { };

        //                string sPrefix = model.m_arrConnectionJoints[i].m_arrPlates[j].Name;
        //                int iQuantity = 1;
        //                string sMaterialName = model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.Name;

        //                float fWidth_bx = model.m_arrConnectionJoints[i].m_arrPlates[j].Width_bx;
        //                float fHeight_hy = model.m_arrConnectionJoints[i].m_arrPlates[j].Height_hy;
        //                float Ft = model.m_arrConnectionJoints[i].m_arrPlates[j].Ft;
        //                float fArea = model.m_arrConnectionJoints[i].m_arrPlates[j].fArea;
        //                float fMassPerPiece = fArea * Ft * model.m_arrConnectionJoints[i].m_arrPlates[j].m_Mat.m_fRho;

        //                float fPricePerPiece;
        //                if (model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD > 0)
        //                    fPricePerPiece = (float)model.m_arrConnectionJoints[i].m_arrPlates[j].Price_PPKG_NZD * fMassPerPiece;
        //                else
        //                    fPricePerPiece = fCFS_PricePerKg_Plates_Total * fMassPerPiece;

        //                float fTotalArea = iQuantity * fArea;
        //                float fTotalMass = iQuantity * fMassPerPiece;
        //                float fTotalPrice = iQuantity * fPricePerPiece;

        //                bool bPlatewasAdded = false; // Plate was added to the group

        //                if (i > 0 || (i == 0 && j > 0)) // If it not first item
        //                {
        //                    for (int k = 0; k < ListOfPlateGroups.Count; k++) // For each group of plates check if current plate has same prefix and same dimensions as some already created -  // Add plate to the group or create new one
        //                    {
        //                        if (ListOfPlateGroups[k].Name == model.m_arrConnectionJoints[i].m_arrPlates[j].Name &&
        //                        MathF.d_equal(ListOfPlateGroups[k].Width_bx, model.m_arrConnectionJoints[i].m_arrPlates[j].Width_bx) &&
        //                        MathF.d_equal(ListOfPlateGroups[k].Height_hy, model.m_arrConnectionJoints[i].m_arrPlates[j].Height_hy) &&
        //                        MathF.d_equal(ListOfPlateGroups[k].Ft, model.m_arrConnectionJoints[i].m_arrPlates[j].Ft) &&
        //                        MathF.d_equal(ListOfPlateGroups[k].fArea, model.m_arrConnectionJoints[i].m_arrPlates[j].fArea))
        //                        {
        //                            // Add plate to the one from already created groups

        //                            listPlateCount[k] += 1; // Add one plate (piece) to the quantity
        //                            listPlateTotalArea[k] = listPlateCount[k] * dlistPlateArea[k];
        //                            listPlateTotalMass[k] = listPlateCount[k] * dlistPlateUnitMass[k]; // Recalculate total weight of all plates in the group
        //                            listPlateTotalPrice[k] = listPlateCount[k] * dlistPlatePricePerPiece[k]; // Recalculate total price of all plates in the group

        //                            bPlatewasAdded = true;
        //                        }
        //                        // TODO - po pridani plechu by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
        //                    }
        //                }

        //                if ((i == 0 && j == 0) || !bPlatewasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
        //                {
        //                    //TODO - radsej refaktorovat s triedou PlateView
        //                    listPlatePrefix.Add(sPrefix);
        //                    listPlateCount.Add(iQuantity);
        //                    listPlateMaterialName.Add(sMaterialName);
        //                    dlistPlateWidth_bx.Add(fWidth_bx);
        //                    dlistPlateHeight_hy.Add(fHeight_hy);
        //                    dlistPlateThickness_tz.Add(Ft);
        //                    dlistPlateArea.Add(fArea);
        //                    dlistPlateUnitMass.Add(fMassPerPiece);
        //                    listPlateTotalArea.Add(fTotalArea);
        //                    listPlateTotalMass.Add(fTotalMass);
        //                    dlistPlatePricePerPiece.Add(fPricePerPiece);
        //                    listPlateTotalPrice.Add(fTotalPrice);

        //                    // Add first plate in the group to the list of plate groups
        //                    ListOfPlateGroups.Add(model.m_arrConnectionJoints[i].m_arrPlates[j]);
        //                }

        //                // Blok1 Plate END
        //                // ----------------------------------------------------------------------------------------------------------------------------------------
        //                #endregion

        //                //temp
        //                // Anchors - WASHERS
        //                // TO Mato - nieco som skusal... chcelo by to asi mat jeden objekt na tieto veci a nie zoznamy kade tade
        //                //rovnako je asi problem,ze to nijako negrupujem...ale tak potreboval by som vediet na zaklade coho sa to bude grupovat

        //                // K prvej vete nemam vyhrady. Urob ako sa to ma.
        //                // Zgrupovat to treba podla prefixu, ale kedze to este nie je dotiahnute tak porovnavam aj rozmery a plochu uz pridanych plates alebo washers s aktualnym
        //                // Vyrobil som 3 bloky kodu, resp. regiony
        //                // Jeden pre base plate, jeden washer plate top a jeden pre washer bearing
        //                // Funguje to tak ze sa v bloku nastavia parametre aktualnej plate / washer (pocet, rozmery cena, celkove pocty a cena atd)
        //                // Potom sa prechadza cyklus cez vsetky uz vytvorene riadky, resp ListOfPlateGroups a porovnava sa ci je aktualny objekt rovnaky ako niektory uz pridany do skupiny
        //                // Porovnava sa prefix, rozmery a plocha (ak by sme boli dosledni tak pre plates by sa este malo porovnat screw arrangement, anchor arrangement)
        //                // Ak sa zisti ze rovnaky plate/ washer uz bol pridany tak sa aktualizuju celkove parametre, celkovy pocet, celkova plocha, celkova hmotnost
        //                // Ak sa zisti ze taky plech v skupine este nie je alebo je to uplne prvy plech v cykle tak sa vyrobi novy zaznam

        //                // Dalo by sa to napriklad refaktorovat a urobit z toho jedna funkcia
        //                // ListOfPlateGroups by som asi zrusil, lebo tam nemame moznost nastavit pocet plechov v ramci skupiny
        //                // Ak tomu rozumiem spravne chces na to pouzit List<PlateView> a odstranit jednotlive zoznamy podla stplcov
        //                // Kazdopadne zase sa dostavame k tomu, ze to mame vselijako, niekde samostatne zoznamy pre jednotlive stlpce, inde zoznam objektov s properties, ktore odpovedaju jednemu riadku.

        //                if (model.m_arrConnectionJoints[i].m_arrPlates[j] is CConCom_Plate_B_basic)
        //                {
        //                    CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)model.m_arrConnectionJoints[i].m_arrPlates[j];

        //                    if (plate.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
        //                    {
        //                        CAnchor anchor = plate.AnchorArrangement.Anchors.FirstOrDefault();
        //                        int anchorsNum = plate.AnchorArrangement.Anchors.Length;

        //                        #region Washer Plate Top
        //                        // Blok2 Washer Plate Top START
        //                        // ----------------------------------------------------------------------------------------------------------------------------------------
        //                        // Plate Top Washer
        //                        try
        //                        {
        //                            anchor.WasherPlateTop.SetParams(anchor.WasherPlateTop.Name, anchor.WasherPlateTop.m_ePlateSerieType_FS);
        //                        }
        //                        catch { };

        //                        sPrefix = anchor.WasherPlateTop.Name;
        //                        iQuantity = anchorsNum; // One plate washer per anchor
        //                        sMaterialName = anchor.WasherPlateTop.m_Mat.Name;

        //                        fWidth_bx = anchor.WasherPlateTop.Width_bx;
        //                        fHeight_hy = anchor.WasherPlateTop.Height_hy;
        //                        Ft = anchor.WasherPlateTop.Ft;
        //                        fArea = anchor.WasherPlateTop.fArea;
        //                        fMassPerPiece = fArea * Ft * anchor.WasherPlateTop.m_Mat.m_fRho;

        //                        if (anchor.WasherPlateTop.Price_PPKG_NZD > 0)
        //                            fPricePerPiece = (float)anchor.WasherPlateTop.Price_PPKG_NZD * fMassPerPiece;
        //                        else
        //                            fPricePerPiece = fCFS_PricePerKg_Plates_Total * fMassPerPiece;

        //                        fTotalArea = iQuantity * anchor.WasherPlateTop.fArea;
        //                        fTotalMass = iQuantity * fMassPerPiece;
        //                        fTotalPrice = iQuantity * fPricePerPiece;

        //                        bPlatewasAdded = false; // Plate was added to the group

        //                        if (i > 0 || (i == 0 && j > 0)) // If it not first item
        //                        {
        //                            for (int k = 0; k < ListOfPlateGroups.Count; k++) // For each group of plates check if current plate has same prefix and same dimensions as some already created -  // Add plate to the group or create new one
        //                            {
        //                                if (ListOfPlateGroups[k].Name == anchor.WasherPlateTop.Name &&
        //                                MathF.d_equal(ListOfPlateGroups[k].Width_bx, anchor.WasherPlateTop.Width_bx) &&
        //                                MathF.d_equal(ListOfPlateGroups[k].Height_hy, anchor.WasherPlateTop.Height_hy) &&
        //                                MathF.d_equal(ListOfPlateGroups[k].Ft, anchor.WasherPlateTop.Ft) &&
        //                                MathF.d_equal(ListOfPlateGroups[k].fArea, anchor.WasherPlateTop.fArea))
        //                                {
        //                                    // Add plate to the one from already created groups

        //                                    listPlateCount[k] += iQuantity; // Add one washers to the quantity
        //                                    listPlateTotalArea[k] = listPlateCount[k] * dlistPlateArea[k];
        //                                    listPlateTotalMass[k] = listPlateCount[k] * dlistPlateUnitMass[k]; // Recalculate total weight of all plates in the group
        //                                    listPlateTotalPrice[k] = listPlateCount[k] * dlistPlatePricePerPiece[k]; // Recalculate total price of all plates in the group

        //                                    bPlatewasAdded = true;
        //                                }

        //                                // TODO - po pridani plechu by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
        //                            }
        //                        }

        //                        if ((i == 0 && j == 0) || !bPlatewasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
        //                        {
        //                            //TODO - radsej refaktorovat s triedou PlateView
        //                            listPlatePrefix.Add(sPrefix);
        //                            listPlateCount.Add(iQuantity);
        //                            listPlateMaterialName.Add(sMaterialName);
        //                            dlistPlateWidth_bx.Add(fWidth_bx);
        //                            dlistPlateHeight_hy.Add(fHeight_hy);
        //                            dlistPlateThickness_tz.Add(Ft);
        //                            dlistPlateArea.Add(fArea);
        //                            dlistPlateUnitMass.Add(fMassPerPiece);
        //                            listPlateTotalArea.Add(fTotalArea);
        //                            listPlateTotalMass.Add(fTotalMass);
        //                            dlistPlatePricePerPiece.Add(fPricePerPiece);
        //                            listPlateTotalPrice.Add(fTotalPrice);

        //                            // Add first plate in the group to the list of plate groups
        //                            ListOfPlateGroups.Add(anchor.WasherPlateTop);
        //                        }
        //                        // Blok2 Washer Plate Top END
        //                        // ----------------------------------------------------------------------------------------------------------------------------------------
        //                        #endregion

        //                        #region Washer Bearing 
        //                        // Blok3 Washer Bearing START
        //                        // ----------------------------------------------------------------------------------------------------------------------------------------
        //                        // Bearing Washer
        //                        try
        //                        {
        //                            anchor.WasherBearing.SetParams(anchor.WasherBearing.Name, anchor.WasherBearing.m_ePlateSerieType_FS);
        //                        }
        //                        catch { };

        //                        sPrefix = anchor.WasherBearing.Name;
        //                        iQuantity = 2 * anchorsNum; // Two bearing washers per anchor
        //                        sMaterialName = anchor.WasherBearing.m_Mat.Name;

        //                        fWidth_bx = anchor.WasherBearing.Width_bx;
        //                        fHeight_hy = anchor.WasherBearing.Height_hy;
        //                        Ft = anchor.WasherBearing.Ft;
        //                        fArea = anchor.WasherBearing.fArea;
        //                        fMassPerPiece = fArea * Ft * anchor.WasherBearing.m_Mat.m_fRho;

        //                        if (anchor.WasherBearing.Price_PPKG_NZD > 0)
        //                            fPricePerPiece = (float)anchor.WasherBearing.Price_PPKG_NZD * fMassPerPiece;
        //                        else
        //                            fPricePerPiece = fCFS_PricePerKg_Plates_Total * fMassPerPiece;

        //                        fTotalArea = iQuantity * anchor.WasherPlateTop.fArea;
        //                        fTotalMass = iQuantity * fMassPerPiece;
        //                        fTotalPrice = iQuantity * fPricePerPiece;

        //                        bPlatewasAdded = false; // Plate was added to the group

        //                        if (i > 0 || (i == 0 && j > 0)) // If it not first item
        //                        {
        //                            for (int k = 0; k < ListOfPlateGroups.Count; k++) // For each group of plates check if current plate has same prefix and same dimensions as some already created -  // Add plate to the group or create new one
        //                            {
        //                                if (ListOfPlateGroups[k].Name == anchor.WasherBearing.Name &&
        //                                MathF.d_equal(ListOfPlateGroups[k].Width_bx, anchor.WasherBearing.Width_bx) &&
        //                                MathF.d_equal(ListOfPlateGroups[k].Height_hy, anchor.WasherBearing.Height_hy) &&
        //                                MathF.d_equal(ListOfPlateGroups[k].Ft, anchor.WasherBearing.Ft) &&
        //                                MathF.d_equal(ListOfPlateGroups[k].fArea, anchor.WasherBearing.fArea))
        //                                {
        //                                    // Add plate to the one from already created groups

        //                                    listPlateCount[k] += iQuantity; // Add one washers to the quantity
        //                                    listPlateTotalArea[k] = listPlateCount[k] * dlistPlateArea[k];
        //                                    listPlateTotalMass[k] = listPlateCount[k] * dlistPlateUnitMass[k]; // Recalculate total weight of all plates in the group
        //                                    listPlateTotalPrice[k] = listPlateCount[k] * dlistPlatePricePerPiece[k]; // Recalculate total price of all plates in the group

        //                                    bPlatewasAdded = true;
        //                                }

        //                                // TODO - po pridani plechu by sme mohli tento cyklus prerusit, pokracovat dalej nema zmysel
        //                            }
        //                        }

        //                        if ((i == 0 && j == 0) || !bPlatewasAdded) // Create new group (new row) (different length / prefix of plates or first item in list of plates assigned to the cross-section)
        //                        {
        //                            //TODO - radsej refaktorovat s triedou PlateView
        //                            listPlatePrefix.Add(sPrefix);
        //                            listPlateCount.Add(iQuantity);
        //                            listPlateMaterialName.Add(sMaterialName);
        //                            dlistPlateWidth_bx.Add(fWidth_bx);
        //                            dlistPlateHeight_hy.Add(fHeight_hy);
        //                            dlistPlateThickness_tz.Add(Ft);
        //                            dlistPlateArea.Add(fArea);
        //                            dlistPlateUnitMass.Add(fMassPerPiece);
        //                            listPlateTotalArea.Add(fTotalArea);
        //                            listPlateTotalMass.Add(fTotalMass);
        //                            dlistPlatePricePerPiece.Add(fPricePerPiece);
        //                            listPlateTotalPrice.Add(fTotalPrice);

        //                            // Add first plate in the group to the list of plate groups
        //                            ListOfPlateGroups.Add(anchor.WasherBearing);
        //                        }
        //                        // Blok3 Washer Bearing END
        //                        // ----------------------------------------------------------------------------------------------------------------------------------------
        //                        #endregion
        //                    }
        //                }
        //                //end temp
        //            }
        //        }
        //    }
        //    //System.Diagnostics.Trace.WriteLine("Joints SelectedForMaterialList count: " + count);

        //    // Check Data
        //    double dTotalPlatesArea_Model = 0, dTotalPlatesArea_Table = 0;
        //    double dTotalPlatesVolume_Model = 0, dTotalPlatesVolume_Table = 0;
        //    double dTotalPlatesMass_Model = 0, dTotalPlatesMass_Table = 0;
        //    double dTotalPlatesPrice_Model = 0, dTotalPlatesPrice_Table = 0;
        //    int iTotalPlatesNumber_Model = 0, iTotalPlatesNumber_Table = 0;

        //    foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
        //    {
        //        if (joint.BIsSelectedForMaterialList)
        //        {
        //            // Set plates and connectors data
        //            foreach (CPlate plate in joint.m_arrPlates)
        //            {
        //                dTotalPlatesArea_Model += plate.fArea;
        //                dTotalPlatesVolume_Model += plate.fArea * plate.Ft;
        //                dTotalPlatesMass_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho;

        //                if (plate.Price_PPKG_NZD > 0)
        //                    dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * plate.Price_PPKG_NZD;
        //                else
        //                    dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;

        //                iTotalPlatesNumber_Model += 1;
        //            }
        //        }
        //    }

        //    for (int i = 0; i < listPlatePrefix.Count; i++)
        //    {
        //        dTotalPlatesArea_Table += (dlistPlateArea[i] * listPlateCount[i]);
        //        dTotalPlatesVolume_Table += (dlistPlateArea[i] * listPlateCount[i] * dlistPlateThickness_tz[i]);
        //        dTotalPlatesMass_Table += listPlateTotalMass[i];
        //        dTotalPlatesPrice_Table += listPlateTotalPrice[i];
        //        iTotalPlatesNumber_Table += listPlateCount[i];
        //    }

        //    //dTotalPlatesArea_Model = Math.Round(dTotalPlatesArea_Model, iNumberOfDecimalPlacesArea);
        //    //dTotalPlatesVolume_Model = Math.Round(dTotalPlatesVolume_Model, iNumberOfDecimalPlacesVolume);
        //    //dTotalPlatesMass_Model = Math.Round(dTotalPlatesMass_Model, iNumberOfDecimalPlacesMass);
        //    //dTotalPlatesPrice_Model = Math.Round(dTotalPlatesPrice_Model, iNumberOfDecimalPlacesPrice);

        //    //if (!MathF.d_equal(dTotalPlatesArea_Model, dTotalPlatesArea_Table) ||
        //    //    !MathF.d_equal(dTotalPlatesVolume_Model, dTotalPlatesVolume_Table) ||
        //    //    !MathF.d_equal(dTotalPlatesMass_Model, dTotalPlatesMass_Table) ||
        //    //    (iTotalPlatesNumber_Model != iTotalPlatesNumber_Table)) // Error
        //    //    MessageBox.Show(
        //    //    "Total area of plates in model " + dTotalPlatesArea_Model + " m^2" + "\n" +
        //    //    "Total area of plates in table " + dTotalPlatesArea_Table + " m^2" + "\n" +
        //    //    "Total volume of plates in model " + dTotalPlatesVolume_Model + " m^3" + "\n" +
        //    //    "Total volume of plates in table " + dTotalPlatesVolume_Table + " m^3" + "\n" +
        //    //    "Total weight of plates in model " + dTotalPlatesMass_Model + " kg" + "\n" +
        //    //    "Total weight of plates in table " + dTotalPlatesMass_Table + " kg" + "\n" +
        //    //    "Total number of plates in model " + iTotalPlatesNumber_Model + "\n" +
        //    //    "Total number of plates in table " + iTotalPlatesNumber_Table + "\n");

        //    // Prepare output format (last row is empty)
        //    for (int i = 0; i < listPlatePrefix.Count; i++)
        //    {
        //        // Change output data format
        //        listPlateWidth_bx.Add(dlistPlateWidth_bx[i].ToString("F3"));
        //        listPlateHeight_hy.Add(dlistPlateHeight_hy[i].ToString("F3"));
        //        listPlateThickness_tz.Add(dlistPlateThickness_tz[i].ToString("F3"));
        //        listPlateArea.Add(dlistPlateArea[i].ToString("F3"));
        //        listPlateMassPerPiece.Add(dlistPlateUnitMass[i].ToString("F3"));
        //        listPlatePricePerPiece.Add(dlistPlatePricePerPiece[i].ToString("F3"));
        //    }

        //    dBuildingMass += dTotalPlatesMass_Table;
        //    dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalPlatesPrice_Table;

        //    // Add Sum
        //    listPlatePrefix.Add("Total:");
        //    listPlateCount.Add(iTotalPlatesNumber_Table);
        //    listPlateMaterialName.Add("");
        //    listPlateWidth_bx.Add(""); // Empty cell
        //    listPlateHeight_hy.Add(""); // Empty cell
        //    listPlateThickness_tz.Add(""); // Empty cell
        //    listPlateArea.Add(""); // Empty cell
        //    listPlateMassPerPiece.Add(""); // Empty cell
        //    listPlateTotalArea.Add(dTotalPlatesArea_Table);
        //    listPlateTotalMass.Add(dTotalPlatesMass_Table);
        //    listPlatePricePerPiece.Add("");
        //    listPlateTotalPrice.Add(dTotalPlatesPrice_Table);

        //    // Create Table
        //    DataTable table = new DataTable("TablePlates");
        //    // Create Table Rows
        //    table.Columns.Add("Prefix", typeof(String));
        //    table.Columns.Add("Count", typeof(Int32));
        //    table.Columns.Add("Material", typeof(String));
        //    table.Columns.Add(sEP_Width, typeof(String));
        //    table.Columns.Add("Height", typeof(String));
        //    table.Columns.Add("Thickness", typeof(String));
        //    table.Columns.Add("Area", typeof(String));
        //    table.Columns.Add("UnitMass", typeof(String));
        //    table.Columns.Add("TotalArea", typeof(Decimal));
        //    table.Columns.Add("TotalMass", typeof(Decimal));
        //    table.Columns.Add("UnitPrice", typeof(String));
        //    table.Columns.Add("Price", typeof(Decimal));

        //    // Set Column Caption
        //    table.Columns["Prefix"].Caption = "Prefix";
        //    table.Columns["Count"].Caption = "Count [-]";
        //    table.Columns["Material"].Caption = "Material";
        //    table.Columns["Width"].Caption = "Width [m]";
        //    table.Columns["Height"].Caption = "Height [m]";
        //    table.Columns["Thickness"].Caption = "Thickness [m]";
        //    table.Columns["Area"].Caption = "Area [m2]";
        //    table.Columns["UnitMass"].Caption = "Unit Mass [kg/piece]";
        //    table.Columns["TotalArea"].Caption = "Total Area [m2]";
        //    table.Columns["TotalMass"].Caption = "Total Mass [kg]";
        //    table.Columns["UnitPrice"].Caption = "Unit Price [NZD/piece]";
        //    table.Columns["Price"].Caption = "Price [NZD]";

        //    table.Columns["Prefix"].ExtendedProperties.Add(sEP_Width, 7f);
        //    table.Columns["Count"].ExtendedProperties.Add(sEP_Width, 7f);
        //    table.Columns["Material"].ExtendedProperties.Add(sEP_Width, 8.5f);
        //    table.Columns["Width"].ExtendedProperties.Add(sEP_Width, 7f);
        //    table.Columns["Height"].ExtendedProperties.Add(sEP_Width, 7f);
        //    table.Columns["Thickness"].ExtendedProperties.Add(sEP_Width, 8.5f);
        //    table.Columns["Area"].ExtendedProperties.Add(sEP_Width, 7f);
        //    table.Columns["UnitMass"].ExtendedProperties.Add(sEP_Width, 9f);
        //    table.Columns["TotalArea"].ExtendedProperties.Add(sEP_Width, 10f);
        //    table.Columns["TotalMass"].ExtendedProperties.Add(sEP_Width, 10f);
        //    table.Columns["UnitPrice"].ExtendedProperties.Add(sEP_Width, 11f);
        //    table.Columns["Price"].ExtendedProperties.Add(sEP_Width, 8f);

        //    table.Columns["Prefix"].ExtendedProperties.Add(sEP_Align, AlignmentX.Left);
        //    table.Columns["Count"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["Material"].ExtendedProperties.Add(sEP_Align, AlignmentX.Left);
        //    table.Columns["Width"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["Height"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["Thickness"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["Area"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["UnitMass"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["TotalArea"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["TotalMass"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["UnitPrice"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    table.Columns["Price"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);

        //    // Create Datases
        //    DataSet ds = new DataSet();
        //    // Add Table to Dataset
        //    ds.Tables.Add(table);

        //    for (int i = 0; i < listPlatePrefix.Count; i++)
        //    {
        //        DataRow row = table.NewRow();

        //        try
        //        {
        //            row["Prefix"] = listPlatePrefix[i];
        //            row["Count"] = listPlateCount[i];
        //            row["Material"] = listPlateMaterialName[i];
        //            row["Width"] = listPlateWidth_bx[i];
        //            row["Height"] = listPlateHeight_hy[i];
        //            row["Thickness"] = listPlateThickness_tz[i];
        //            row["Area"] = listPlateArea[i];
        //            row["UnitMass"] = listPlateMassPerPiece[i];
        //            row["TotalArea"] = listPlateTotalArea[i].ToString("F2");
        //            row["TotalMass"] = listPlateTotalMass[i].ToString("F2");
        //            row["UnitPrice"] = listPlatePricePerPiece[i];
        //            row["Price"] = listPlateTotalPrice[i].ToString("F2");
        //        }
        //        catch (ArgumentOutOfRangeException) { }
        //        table.Rows.Add(row);
        //    }

        //    Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
        //    Datagrid_Plates.Loaded += Datagrid_Plates_Loaded;
        //}

        //zaloha
        //private void CreateTableConnectors(CModel model)
        //{
        //    //float fTEK_PricePerPiece_Screws_Total = 0.15f;     // NZD / piece / !!! priblizna cena - nezohladnuje priemer skrutky
        //    //float fAnchor_PricePerLength = 30; // NZD / m - !!! priblizna cena - nezohladnuje priemer tyce
        //    //float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

        //    //listConnectorPrefix = new List<string>(1);
        //    //listConnectorCount = new List<int>(1);
        //    //listConnectorMaterialName = new List<string>(1);
        //    //listConnectorSize = new List<string>(1);
        //    //listConnectorUnitMass = new List<double>(1);
        //    //listConnectorTotalMass = new List<double>(1);
        //    //listConnectorUnitPrice = new List<double>(1);
        //    //listConnectorTotalPrice = new List<double>(1);

        //    // Connectors
        //    // TASK 422
        //    // Neviem ci je to stastne ale chcel som usetrit datagridy a dat vsetky spojovacie prostriedky (rozne typy) do jednej tabulky
        //    // Vsetky by mali mat nejaky prefix, material, popis velkosti (priemer, dlzka), vaha / kus, cena / kus
        //    // Prosim pozri sa na to a skus to povylepsovat
        //    // Blok pre screws a pre anchors maju velmi vela spolocneho, mozes to skusit refaktorovat

        //    // Anchors + screws
        //    //List<CConnector> ListOfConnectorGroups = new List<CConnector>();
        //    List<QuotationItem> quotation = new List<QuotationItem>();

        //    for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
        //    {
        //        if (!model.m_arrConnectionJoints[i].BIsSelectedForMaterialList) continue;

        //        for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++) // For each plate
        //        {
        //            // Screws
        //            if (model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws != null)
        //            {
        //                AddConnector(model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws.FirstOrDefault(), ListOfConnectorGroups, model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws.Length);
        //            }

        //            // Anchors
        //            if (model.m_arrConnectionJoints[i].m_arrPlates[j] is CConCom_Plate_B_basic)
        //            {
        //                CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)model.m_arrConnectionJoints[i].m_arrPlates[j];

        //                if (plate.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
        //                {
        //                    // TASK 422
        //                    // Doplnit data pre anchors
        //                    // Refaktorovat anchors a screws
        //                    // Pre Quantity asi zaviest Count a zjednotit nazov stlpca pre pocet vsade
        //                    // Size

        //                    // Pre screws - gauge + dlzka (14g - 38)
        //                    // Pre anchors  - name + dlzka (M16 - 330)

        //                    // Prefix | Quantity |     Material     | Size    |   Mass per Piece [kg] | Total Mass [kg] | Unit Price [NZD / piece] | Total Price [NZD]
        //                    // TEK    |     1515 | Class 3 / 4 / B8 |  14g-38 |                 0.052 |
        //                    // Anchor |       65 |              8.8 | M16-330 |                 2.241 |

        //                    AddConnector(plate.AnchorArrangement.Anchors.FirstOrDefault(), ListOfConnectorGroups, plate.AnchorArrangement.Anchors.Length);
        //                }
        //            }
        //        }
        //    }

        //    // Check Data
        //    //double dTotalConnectorsMass_Model = 0, dTotalConnectorsMass_Table = 0;
        //    //double dTotalConnectorsPrice_Model = 0, dTotalConnectorsPrice_Table = 0;
        //    //int iTotalConnectorsNumber_Model = 0, iTotalConnectorsNumber_Table = 0;

        //    ////toto sa mi zda,ze netreba
        //    //foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
        //    //{
        //    //    if (!joint.BIsSelectedForMaterialList) continue;

        //    //    foreach (CPlate plate in joint.m_arrPlates)
        //    //    {
        //    //        // Set connectors data
        //    //        if (plate.ScrewArrangement.Screws != null)
        //    //        {
        //    //            foreach (CConnector connector in plate.ScrewArrangement.Screws)
        //    //            {
        //    //                dTotalConnectorsMass_Model += connector.Mass;

        //    //                if (connector.Price_PPP_NZD > 0)
        //    //                    dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
        //    //                else
        //    //                    dTotalConnectorsPrice_Model += fTEK_PricePerPiece_Screws_Total;

        //    //                iTotalConnectorsNumber_Model += 1;
        //    //            }
        //    //        }

        //    //        if (plate is CConCom_Plate_B_basic)
        //    //        {
        //    //            CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

        //    //            if (basePlate.AnchorArrangement.Anchors != null)
        //    //            {
        //    //                foreach (CConnector connector in basePlate.AnchorArrangement.Anchors)
        //    //                {
        //    //                    dTotalConnectorsMass_Model += connector.Mass;

        //    //                    if (connector.Price_PPP_NZD > 0)
        //    //                        dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
        //    //                    else
        //    //                        dTotalConnectorsPrice_Model += (fAnchor_PricePerLength * connector.Length);

        //    //                    iTotalConnectorsNumber_Model += 1;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    double dTotalConnectorsMass_Table = 0;
        //    double dTotalConnectorsPrice_Table = 0;
        //    int iTotalConnectorsNumber_Table = 0;
        //    for (int i = 0; i < listConnectorPrefix.Count; i++)
        //    {
        //        dTotalConnectorsMass_Table += listConnectorTotalMass[i];
        //        dTotalConnectorsPrice_Table += listConnectorTotalPrice[i];
        //        iTotalConnectorsNumber_Table += listConnectorCount[i];
        //    }

        //    //To Mato...toto tu treba???
        //    // 
        //    // Tie kontroly znikli pre to aby som mal istotu ze som vsetko spravne pridal z modelu do zoznamov
        //    // V debugu by sa nam to mohlo zist aby sme nic nevynechali ani neodfiltrovali

        //    //if (!MathF.d_equal(dTotalConnectorsMass_Model, dTotalConnectorsMass_Table) ||
        //    //        (iTotalConnectorsNumber_Model != iTotalConnectorsNumber_Table)) // Error
        //    //    MessageBox.Show(
        //    //    "Total weight of connectors in model " + dTotalConnectorsMass_Model + " kg" + "\n" +
        //    //    "Total weight of connectors in table " + dTotalConnectorsMass_Table + " kg" + "\n" +
        //    //    "Total number of connectors in model " + iTotalConnectorsNumber_Model + "\n" +
        //    //    "Total number of connectors in table " + iTotalConnectorsNumber_Table + "\n");

        //    //// Prepare output format (last row is empty)
        //    //for (int i = 0; i < listConnectorPrefix.Count; i++)
        //    //{
        //    //    // Change output data format
        //    //    listConnectorMassPerPiece.Add(dlistConnectorMassPerPiece[i].ToString("F2"));
        //    //}

        //    dBuildingMass += dTotalConnectorsMass_Table;
        //    dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalConnectorsPrice_Table;

        //    // Create Table
        //    DataTable dt = new DataTable("Table3");
        //    // Create Table Rows

        //    dt.Columns.Add("Prefix", typeof(String));
        //    dt.Columns.Add("Count", typeof(Int32));
        //    dt.Columns.Add("Material", typeof(String));
        //    dt.Columns.Add("Size", typeof(String));
        //    dt.Columns.Add("UnitMass", typeof(String));
        //    dt.Columns.Add("TotalMass", typeof(Decimal));
        //    dt.Columns.Add("UnitPrice", typeof(string));
        //    dt.Columns.Add("Price", typeof(Decimal));

        //    // Prefix | Quantity |     Material     | Size    |   Mass per Piece [kg] | Total Mass [kg] | Unit Price [NZD / piece] | Total Price [NZD]
        //    // Set Column Caption
        //    dt.Columns["Prefix"].Caption = "Prefix";
        //    dt.Columns["Count"].Caption = "Count [-]";
        //    dt.Columns["Material"].Caption = "Material";
        //    dt.Columns["Size"].Caption = "Size";
        //    dt.Columns["UnitMass"].Caption = "Unit Mass [kg/piece]";
        //    dt.Columns["TotalMass"].Caption = "Total Mass [kg]";
        //    dt.Columns["UnitPrice"].Caption = "Unit Price [NZD/piece]";
        //    dt.Columns["Price"].Caption = "Price [NZD]";

        //    dt.Columns["Prefix"].ExtendedProperties.Add(sEP_Width, 25f);
        //    dt.Columns["Count"].ExtendedProperties.Add(sEP_Width, 7f);
        //    dt.Columns["Material"].ExtendedProperties.Add(sEP_Width, 20f);
        //    dt.Columns["Size"].ExtendedProperties.Add(sEP_Width, 10f);
        //    dt.Columns["UnitMass"].ExtendedProperties.Add(sEP_Width, 10f);
        //    dt.Columns["TotalMass"].ExtendedProperties.Add(sEP_Width, 10f);
        //    dt.Columns["UnitPrice"].ExtendedProperties.Add(sEP_Width, 10f);
        //    dt.Columns["Price"].ExtendedProperties.Add(sEP_Width, 8f);

        //    dt.Columns["Prefix"].ExtendedProperties.Add(sEP_Align, AlignmentX.Left);
        //    dt.Columns["Count"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    dt.Columns["Material"].ExtendedProperties.Add(sEP_Align, AlignmentX.Left);
        //    dt.Columns["Size"].ExtendedProperties.Add(sEP_Align, AlignmentX.Left);
        //    dt.Columns["UnitMass"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    dt.Columns["TotalMass"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    dt.Columns["UnitPrice"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);
        //    dt.Columns["Price"].ExtendedProperties.Add(sEP_Align, AlignmentX.Right);

        //    // Create Datases
        //    DataSet ds = new DataSet();
        //    // Add Table to Dataset
        //    ds.Tables.Add(dt);
        //    DataRow row;
        //    for (int i = 0; i < listConnectorPrefix.Count; i++)
        //    {
        //        row = dt.NewRow();
        //        try
        //        {
        //            row["Prefix"] = listConnectorPrefix[i];
        //            row["Count"] = listConnectorCount[i];
        //            row["Material"] = listConnectorMaterialName[i];
        //            row["Size"] = listConnectorSize[i];
        //            row["UnitMass"] = listConnectorUnitMass[i].ToString("F2");
        //            row["TotalMass"] = listConnectorTotalMass[i].ToString("F2");
        //            row["UnitPrice"] = listConnectorUnitPrice[i].ToString("F2");
        //            row["Price"] = listConnectorTotalPrice[i].ToString("F2");
        //        }
        //        catch (ArgumentOutOfRangeException) { }
        //        dt.Rows.Add(row);
        //    }

        //    // Add Sum
        //    row = dt.NewRow();
        //    row["Prefix"] = "Total:";
        //    row["Count"] = iTotalConnectorsNumber_Table;
        //    row["Material"] = "";
        //    row["Size"] = "";
        //    row["UnitPrice"] = "";
        //    row["TotalMass"] = dTotalConnectorsMass_Table.ToString("F2");
        //    row["UnitPrice"] = "";
        //    row["Price"] = dTotalConnectorsPrice_Table.ToString("F2");
        //    dt.Rows.Add(row);

        //    Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
        //    Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        //}

    }

    
}
