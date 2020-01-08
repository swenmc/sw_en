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
        // To Ondrej - tu by som chcel pre stlpce, ktore sa casto opakuju ale najlepsie pre vsetky
        // vyrobit defaultne, ktore sa budu vsade pouzivat, aby sa nestalo ze niekde nieco zmenime a niekde zabudneme

        string sEP_Width = "Width";
        string sEP_Unit = "Unit";
        string sEP_Align = "Align";

        // Bez jednotiek
        ColumnProperties colProp_Prefix = new ColumnProperties(typeof(String), "Prefix", "Prefix", 7.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Name = new ColumnProperties(typeof(String), "Name", "Name", 10f, null, AlignmentX.Left);
        ColumnProperties colProp_Item = new ColumnProperties(typeof(String), "Item", "Item", 10f, null, AlignmentX.Left);
        ColumnProperties colProp_Component = new ColumnProperties(typeof(String), "Component", "Component", 15f, null, AlignmentX.Left);
        ColumnProperties colProp_Size = new ColumnProperties(typeof(String), "Size", "Size", 10f, null, AlignmentX.Left);
        ColumnProperties colProp_Description = new ColumnProperties(typeof(String), "Description", "Description", 15f, null, AlignmentX.Left);

        // Specificke nazvy (mozno by sa to dalo nahradit ako item / description alebo nieco podobne)
        ColumnProperties colProp_CrossSection = new ColumnProperties(typeof(String), "Crsc", "Cross-section", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Cladding = new ColumnProperties(typeof(String), "Cladding", "Cladding", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Fibreglass = new ColumnProperties(typeof(String), "Fibreglass", "Fibreglass", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Opening = new ColumnProperties(typeof(String), "Opening", "Opening", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Gutter = new ColumnProperties(typeof(String), "Gutter", "Gutter", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Downpipe = new ColumnProperties(typeof(String), "Downpipe", "Downpipe", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Flashing = new ColumnProperties(typeof(String), "Flashing", "Flashing", 20f, null, AlignmentX.Left);

        ColumnProperties colProp_Material = new ColumnProperties(typeof(String), "Material", "Material", 12.5f, null, AlignmentX.Left);
        ColumnProperties colProp_Coating = new ColumnProperties(typeof(String), "Coating", "Coating", 10f, null, AlignmentX.Left);
        ColumnProperties colProp_Color = new ColumnProperties(typeof(String), "Color", "Colour", 10f, null, AlignmentX.Left);
        ColumnProperties colProp_ColorName = new ColumnProperties(typeof(String), "ColorName", "Colour Name", 12.5f, null, AlignmentX.Left);

        // S jednotkami
        ColumnProperties colProp_Count = new ColumnProperties(typeof(Int32), "Count", "Count", 7.5f, "[-]", AlignmentX.Right);
        ColumnProperties colProp_Width_m = new ColumnProperties(typeof(String), "Width_m", "Width", 7.5f, "[m]", AlignmentX.Right);
        ColumnProperties colProp_Height_m = new ColumnProperties(typeof(String), "Height_m", "Height", 7.5f, "[m]", AlignmentX.Right);
        ColumnProperties colProp_Thickness_m = new ColumnProperties(typeof(String), "Thickness_m", "Thickness", 7.5f, "[m]", AlignmentX.Right);
        ColumnProperties colProp_Thickness_mm = new ColumnProperties(typeof(String), "Thickness_mm", "Thickness", 7.5f, "[mm]", AlignmentX.Right);
        ColumnProperties colProp_Diameter_mm = new ColumnProperties(typeof(String), "Diameter_mm", "Diameter", 7.5f, "[mm]", AlignmentX.Right);
        ColumnProperties colProp_Area_m2 = new ColumnProperties(typeof(String), "Area_m2", "Area", 10f, "[m²]", AlignmentX.Right);
        ColumnProperties colProp_UnitMass_LM = new ColumnProperties(typeof(String), "UnitMass_LM", "Unit Mass", 10f, "[kg/m]", AlignmentX.Right);
        ColumnProperties colProp_UnitMass_SM = new ColumnProperties(typeof(String), "UnitMass_SM", "Unit Mass", 10f, "[kg/m²]", AlignmentX.Right);
        ColumnProperties colProp_UnitMass_P = new ColumnProperties(typeof(String), "UnitMass_P", "Unit Mass", 10f, "[kg/piece]", AlignmentX.Right);
        ColumnProperties colProp_UnitPrice_LM_NZD = new ColumnProperties(typeof(String), "UnitPrice_LM_NZD", "Unit Price", 0f, "[NZD/m]", AlignmentX.Right);
        ColumnProperties colProp_UnitPrice_SM_NZD = new ColumnProperties(typeof(String), "UnitPrice_SM_NZD", "Unit Price", 0f, "[NZD/m²]", AlignmentX.Right);
        ColumnProperties colProp_UnitPrice_P_NZD = new ColumnProperties(typeof(String), "UnitPrice_P_NZD", "Unit Price", 0f, "[NZD/piece]", AlignmentX.Right);
        ColumnProperties colProp_TotalLength_m = new ColumnProperties(typeof(Decimal), "TotalLength_m", "Total Length", 10f, "[m]", AlignmentX.Right);
        ColumnProperties colProp_TotalArea_m2 = new ColumnProperties(typeof(Decimal), "TotalArea_m2", "Total Area", 10f, "[m²]", AlignmentX.Right);
        ColumnProperties colProp_TotalMass = new ColumnProperties(typeof(String), "TotalMass", "Total Mass", 10f, "[kg]", AlignmentX.Right);
        ColumnProperties colProp_TotalPrice_NZD = new ColumnProperties(typeof(String), "TotalPrice_NZD", "Price", 0f, "[NZD]", AlignmentX.Right);

        List<ColumnProperties> colPropList;

        double dBuildingMass = 0;
        double dBuildingNetPrice_WithoutMargin_WithoutGST = 0;

        const float fCFS_PricePerKg_Plates_Material = 2.8f;      // NZD / kg
        const float fCFS_PricePerKg_Plates_Manufacture = 2.0f;   // NZD / kg

        const float fTEK_PricePerPiece_Screws_Total = 0.15f;     // NZD / piece / !!! priblizna cena - nezohladnuje priemer skrutky
        const float fAnchor_PricePerLength = 30; // NZD / m - !!! priblizna cena - nezohladnuje priemer tyce
        const float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

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

            colPropList = new List<ColumnProperties>()
            {
                colProp_Prefix,
                colProp_Name,
                colProp_Component,
                colProp_Size,
                colProp_Description,

                colProp_CrossSection,
                colProp_Cladding,
                colProp_Fibreglass,
                colProp_Opening,
                colProp_Gutter,
                colProp_Downpipe,
                colProp_Flashing,

                colProp_Material,
                colProp_Coating,
                colProp_Color,
                colProp_ColorName,

                colProp_Count,
                colProp_Width_m,
                colProp_Height_m,
                colProp_Thickness_m,
                colProp_Thickness_mm,
                colProp_Diameter_mm,
                colProp_Area_m2,
                colProp_UnitMass_LM,
                colProp_UnitMass_SM,
                colProp_UnitMass_P,
                colProp_UnitPrice_LM_NZD,
                colProp_UnitPrice_SM_NZD,
                colProp_UnitPrice_P_NZD,
                colProp_TotalLength_m,
                colProp_TotalArea_m2,
                colProp_TotalMass,
                colProp_TotalPrice_NZD
            };

            CModel model = vm.Model;

            List<Point> fWallDefinitionPoints_Left = new List<Point>(4) { new Point(0, 0), new Point(model.fL_tot, 0), new Point(model.fL_tot, model.fH1_frame), new Point(0, model.fH1_frame) };
            List<Point> fWallDefinitionPoints_Front = new List<Point>(5) { new Point(0, 0), new Point(model.fW_frame, 0), new Point(model.fW_frame, model.fH1_frame), new Point(0.5 * model.fW_frame, model.fH2_frame), new Point(0, model.fH1_frame) };

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
            float fWallArea_Total = fWallArea_Left + fWallArea_Right + fWallArea_Front + fWallArea_Back;
            float fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame - model.fH1_frame) + MathF.Pow2(0.5f * model.fW_frame)); // Dlzka hrany strechy

            float fRoofArea = 0;
            if (vm.ComponentList[(int)EMemberType_FS_Position.Purlin].Generate == true)
                fRoofArea = 2 * fRoofSideLength * model.fL_tot;

            float fFibreGlassArea_Roof = vm.FibreglassAreaRoof / 100f * fRoofArea; // Priesvitna cast strechy TODO Percento pre fibre glass zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul
            float fFibreGlassArea_Walls = vm.FibreglassAreaWall / 100f * fWallArea_Total; // Priesvitna cast strechy TODO Percento zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul

            if (vm._quotationDisplayOptionsVM.DisplayCladding)
            {
                // TODO Ondrej - refaktoring - funckia CreateTableCladding
                CreateTableCladding(vm,
                    fWallArea_Total,
                    fTotalAreaOfOpennings,
                    fFibreGlassArea_Walls,
                    fRoofArea,
                    fFibreGlassArea_Roof
                   );
            }
            else
            {
                TextBlock_Cladding.Visibility = Visibility.Collapsed;
                Datagrid_Cladding.Visibility = Visibility.Collapsed;
            }

            // DG 7
            // Gutters
            if (vm._quotationDisplayOptionsVM.DisplayGutters && _pfdVM.Gutters.Count > 0) CreateTableGutters(model);
            else
            {
                TextBlock_Gutters.Visibility = Visibility.Collapsed;
                Datagrid_Gutters.Visibility = Visibility.Collapsed;
            }

            // DG 8
            // Downpipes
            if (vm._quotationDisplayOptionsVM.DisplayDownpipe) CreateTableDownpipes(model);
            else
            {
                TextBlock_Downpipes.Visibility = Visibility.Collapsed;
                Datagrid_Downpipes.Visibility = Visibility.Collapsed;
            }

            // DG 9
            // FibreGlass
            if (vm._quotationDisplayOptionsVM.DisplayFibreglass) CreateTableFibreglass(vm, fFibreGlassArea_Roof, fFibreGlassArea_Walls);
            else
            {
                TextBlock_Fibreglass.Visibility = Visibility.Collapsed;
                Datagrid_Fibreglass.Visibility = Visibility.Collapsed;
            }

            // DG 10
            // Roof Netting
            if (vm._quotationDisplayOptionsVM.DisplayRoofNetting) CreateTableRoofNetting(fRoofArea);
            else
            {
                TextBlock_RoofNetting.Visibility = Visibility.Collapsed;
                Datagrid_RoofNetting.Visibility = Visibility.Collapsed;
            }

            // DG 11
            // Flashing and Packers
            if (vm._quotationDisplayOptionsVM.DisplayFlashing && _pfdVM.Flashings.Count > 0)
            {
                CreateTableFlashing(model,
                fRoofSideLength,
                fRollerDoorTrimmerFlashing_TotalLength,
                fRollerDoorLintelFlashing_TotalLength,
                fRollerDoorLintelCapFlashing_TotalLength,
                fPADoorTrimmerFlashing_TotalLength,
                fPADoorLintelFlashing_TotalLength,
                fWindowFlashing_TotalLength);
            }
            else
            {
                TextBlock_Flashing.Visibility = Visibility.Collapsed;
                Datagrid_Flashing.Visibility = Visibility.Collapsed;
            }

            // Vysledne hodnoty a sumy spolu s plochou, objemom a celkovou cenou zobrazime v tabe

            double dMarginPercentage = 40; // Default
            double dGST_Percentage = 15;   // Default

            double dMarginAbsolute,
                   buildingPrice_WithMargin_WithoutGST,
                   buildingPrice_PSM,
                   buildingPrice_PCM,
                   buildingPrice_PPKG,
                   dGST_Absolute,
                   dTotalBuildingPrice_IncludingGST;

            // Calculate prices
            CalculatePrices(
                   fBuildingArea_Gross,
                   fBuildingVolume_Gross,
                   dBuildingMass,
                   dMarginPercentage,
                   dGST_Percentage,
                   out dMarginAbsolute,
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
                BuildingPrice_WithMargin_WithoutGST = buildingPrice_WithMargin_WithoutGST,
                GST_Percentage = dGST_Percentage,
                GST_Absolute = dGST_Absolute,
                TotalBuildingPrice_IncludingGST = dTotalBuildingPrice_IncludingGST,
                BuildingArea_Gross = fBuildingArea_Gross,
                BuildingVolume_Gross = fBuildingVolume_Gross,
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
            if (e.PropertyName == "Margin_Percentage" || e.PropertyName == "GST_Percentage")
            {
                QuotationViewModel vm = sender as QuotationViewModel;

                double dMarginAbsolute,
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
                       out dMarginAbsolute,
                       out buildingPrice_WithMargin_WithoutGST,
                       out buildingPrice_PSM,
                       out buildingPrice_PCM,
                       out buildingPrice_PPKG,
                       out dGST_Absolute,
                       out dTotalBuildingPrice_IncludingGST
                       );

                // Set view model output values
                vm.Margin_Absolute = dMarginAbsolute;
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
            out double dMarginAbsolute,
            out double buildingPrice_WithMargin_WithoutGST,
            out double buildingPrice_PSM,
            out double buildingPrice_PCM,
            out double buildingPrice_PPKG,
            out double dGST_Absolute,
            out double dTotalBuildingPrice_IncludingGST
            )
        {
            // Margin
            dMarginAbsolute = dBuildingNetPrice_WithoutMargin_WithoutGST * dMarginPercentage / 100f;
            buildingPrice_WithMargin_WithoutGST = dBuildingNetPrice_WithoutMargin_WithoutGST + dMarginAbsolute;

            // Building Unit Price
            buildingPrice_PSM = fBuildingArea_Gross > 0 ? buildingPrice_WithMargin_WithoutGST / fBuildingArea_Gross : 0;
            buildingPrice_PCM = fBuildingVolume_Gross > 0 ? buildingPrice_WithMargin_WithoutGST / fBuildingVolume_Gross : 0;
            buildingPrice_PPKG = dBuildingMass > 0 ? buildingPrice_WithMargin_WithoutGST / dBuildingMass : 0;

            dGST_Absolute = dGST_Percentage / 100f * buildingPrice_WithMargin_WithoutGST;
            dTotalBuildingPrice_IncludingGST = buildingPrice_WithMargin_WithoutGST + dGST_Absolute;
        }

        private void CreateTableMembers(CModel model)
        {
            // Create Table
            DataTable dt = new DataTable("Members");
            // Create Table Rows
            dt.Columns.Add(colProp_CrossSection.ColumnName, colProp_CrossSection.DataType);
            dt.Columns.Add(colProp_Count.ColumnName, colProp_Count.DataType);
            dt.Columns.Add(colProp_TotalLength_m.ColumnName, colProp_TotalLength_m.DataType);
            dt.Columns.Add(colProp_UnitMass_LM.ColumnName, colProp_UnitMass_LM.DataType);
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_LM_NZD.ColumnName, colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

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
                    row[colProp_CrossSection.ColumnName] = crsc.Name_short;

                    row[colProp_Count.ColumnName] = count.ToString();
                    SumCount += count;

                    row[colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[colProp_UnitMass_LM.ColumnName] = (crsc.A_g * GlobalConstants.MATERIAL_DENSITY_STEEL).ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_LM_NZD.ColumnName] = crsc.price_PPLM_NZD.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = totalPrice.ToString("F2");
                    SumTotalPrice += totalPrice;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            // Last row
            row = dt.NewRow();
            row[colProp_CrossSection.ColumnName] = "Total:";
            row[colProp_Count.ColumnName] = SumCount.ToString();
            row[colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[colProp_UnitMass_LM.ColumnName] = "";
            row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
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

            List<QuotationItem> quotation = new List<QuotationItem>();
            // Plates
            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                //toto by podla mna uz malo byt nastavene a nie tu to checkovat
                joint.BIsSelectedForMaterialList = CJointHelper.IsJointSelectedForMaterialList(joint);

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

                    AddPlateToQuotation(plate, quotation, 1);


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

                            AddPlateToQuotation(anchor.WasherPlateTop, quotation, anchorsNum);


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

                            AddPlateToQuotation(anchor.WasherBearing, quotation, anchorsNum);

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
                            dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;

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
            table.Columns.Add(colProp_Prefix.ColumnName, colProp_Prefix.DataType);
            table.Columns.Add(colProp_Count.ColumnName, colProp_Count.DataType);
            table.Columns.Add(colProp_Material.ColumnName, colProp_Material.DataType);
            table.Columns.Add(colProp_Width_m.ColumnName, colProp_Width_m.DataType);
            table.Columns.Add(colProp_Height_m.ColumnName, colProp_Height_m.DataType);
            table.Columns.Add(colProp_Thickness_m.ColumnName, colProp_Thickness_m.DataType);
            table.Columns.Add(colProp_Area_m2.ColumnName, colProp_Area_m2.DataType);
            table.Columns.Add(colProp_UnitMass_P.ColumnName, colProp_UnitMass_P.DataType);
            table.Columns.Add(colProp_TotalArea_m2.ColumnName, colProp_TotalArea_m2.DataType);
            table.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            table.Columns.Add(colProp_UnitPrice_P_NZD.ColumnName, colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(table);

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
                    row[colProp_Prefix.ColumnName] = item.Prefix;
                    row[colProp_Count.ColumnName] = item.Quantity;
                    row[colProp_Material.ColumnName] = item.MaterialName;
                    row[colProp_Width_m.ColumnName] = item.Width_bx.ToString("F2");
                    row[colProp_Height_m.ColumnName] = item.Height_hy.ToString("F2");
                    row[colProp_Thickness_m.ColumnName] = item.Ft.ToString("F3"); // meters
                    row[colProp_Area_m2.ColumnName] = item.Area.ToString("F2");
                    row[colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[colProp_TotalArea_m2.ColumnName] = item.TotalArea.ToString("F2");
                    row[colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[colProp_Prefix.ColumnName] = "Total:";
            row[colProp_Count.ColumnName] = iTotalPlatesNumber_Table;
            row[colProp_Material.ColumnName] = "";
            row[colProp_Width_m.ColumnName] = "";
            row[colProp_Height_m.ColumnName] = "";
            row[colProp_Thickness_m.ColumnName] = "";
            row[colProp_Area_m2.ColumnName] = "";
            row[colProp_UnitMass_P.ColumnName] = "";
            row[colProp_TotalArea_m2.ColumnName] = dTotalPlatesArea_Table.ToString("F2");
            row[colProp_TotalMass.ColumnName] = dTotalPlatesMass_Table.ToString("F2");
            row[colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = dTotalPlatesPrice_Table.ToString("F2");
            table.Rows.Add(row);

            Datagrid_Plates.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            Datagrid_Plates.Loaded += Datagrid_Plates_Loaded;
        }

        private void Datagrid_Plates_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Plates);
        }

        private void AddPlateToQuotation(CPlate plate, List<QuotationItem> quotation, int iQuantity)
        {
            float fMassPerPiece = plate.fArea * plate.Ft * plate.m_Mat.m_fRho;
            float fPricePerPiece = plate.Price_PPKG_NZD > 0 ? (float)plate.Price_PPKG_NZD * fMassPerPiece : fCFS_PricePerKg_Plates_Total * fMassPerPiece;

            QuotationItem qItem = quotation.FirstOrDefault(q => q.Prefix == plate.Name && MathF.d_equal(q.Width_bx, plate.Width_bx) &&
                    MathF.d_equal(q.Height_hy, plate.Height_hy) &&
                    MathF.d_equal(q.Ft, plate.Ft) &&
                    MathF.d_equal(q.Area, plate.fArea));
            if (qItem != null) //this quotation exists
            {
                qItem.Quantity += iQuantity;
                qItem.TotalArea = qItem.Quantity * qItem.Area;
                qItem.TotalMass = qItem.Quantity * qItem.MassPerPiece;
                qItem.TotalPrice = qItem.Quantity * qItem.PricePerPiece;
            }
            else //quotation item does not exist = add to collection
            {
                QuotationItem item = new QuotationItem
                {
                    Prefix = plate.Name,
                    Quantity = iQuantity,
                    Width_bx = plate.Width_bx,
                    Height_hy = plate.Height_hy,
                    Ft = plate.Ft,
                    MaterialName = plate.m_Mat.Name,
                    Area = plate.fArea,
                    MassPerPiece = fMassPerPiece,
                    PricePerPiece = fPricePerPiece,
                    TotalArea = iQuantity * plate.fArea,
                    TotalMass = iQuantity * fMassPerPiece,
                    TotalPrice = iQuantity * fPricePerPiece
                };
                quotation.Add(item);
            }
        }


        private void CreateTableConnectors(CModel model)
        {
            // Connectors
            // TASK 422
            // Neviem ci je to stastne ale chcel som usetrit datagridy a dat vsetky spojovacie prostriedky (rozne typy) do jednej tabulky            
            // Anchors + screws

            List<QuotationItem> quotation = new List<QuotationItem>();

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                if (!joint.BIsSelectedForMaterialList) continue;

                foreach (CPlate plate in joint.m_arrPlates) // For each plate
                {
                    // Screws
                    if (plate.ScrewArrangement.Screws != null)
                    {
                        AddConnector(plate.ScrewArrangement.Screws.FirstOrDefault(), quotation, plate.ScrewArrangement.Screws.Length);
                    }

                    // Anchors
                    if (plate is CConCom_Plate_B_basic)
                    {
                        CConCom_Plate_B_basic plateB = (CConCom_Plate_B_basic)plate;

                        if (plateB.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                        {
                            // TASK 422
                            // Doplnit data pre anchors
                            // Refaktorovat anchors a screws
                            // Pre Quantity asi zaviest Count a zjednotit nazov stlpca pre pocet vsade

                            // Pre screws - gauge + dlzka (14g - 38)
                            // Pre anchors  - name + dlzka (M16 - 330)

                            // Prefix | Quantity |     Material     | Size    |   Mass per Piece [kg] | Total Mass [kg] | Unit Price [NZD / piece] | Total Price [NZD]
                            // TEK    |     1515 | Class 3 / 4 / B8 |  14g-38 |                 0.052 |
                            // Anchor |       65 |              8.8 | M16-330 |                 2.241 |

                            AddConnector(plateB.AnchorArrangement.Anchors.FirstOrDefault(), quotation, plateB.AnchorArrangement.Anchors.Length);
                        }
                    }
                }
            }

            // Check Data
            //double dTotalConnectorsMass_Model = 0, dTotalConnectorsMass_Table = 0;
            //double dTotalConnectorsPrice_Model = 0, dTotalConnectorsPrice_Table = 0;
            //int iTotalConnectorsNumber_Model = 0, iTotalConnectorsNumber_Table = 0;

            ////toto sa mi zda,ze netreba
            //foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            //{
            //    if (!joint.BIsSelectedForMaterialList) continue;

            //    foreach (CPlate plate in joint.m_arrPlates)
            //    {
            //        // Set connectors data
            //        if (plate.ScrewArrangement.Screws != null)
            //        {
            //            foreach (CConnector connector in plate.ScrewArrangement.Screws)
            //            {
            //                dTotalConnectorsMass_Model += connector.Mass;

            //                if (connector.Price_PPP_NZD > 0)
            //                    dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
            //                else
            //                    dTotalConnectorsPrice_Model += fTEK_PricePerPiece_Screws_Total;

            //                iTotalConnectorsNumber_Model += 1;
            //            }
            //        }

            //        if (plate is CConCom_Plate_B_basic)
            //        {
            //            CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

            //            if (basePlate.AnchorArrangement.Anchors != null)
            //            {
            //                foreach (CConnector connector in basePlate.AnchorArrangement.Anchors)
            //                {
            //                    dTotalConnectorsMass_Model += connector.Mass;

            //                    if (connector.Price_PPP_NZD > 0)
            //                        dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
            //                    else
            //                        dTotalConnectorsPrice_Model += (fAnchor_PricePerLength * connector.Length);

            //                    iTotalConnectorsNumber_Model += 1;
            //                }
            //            }
            //        }
            //    }
            //}

            double dTotalConnectorsMass_Table = 0;
            double dTotalConnectorsPrice_Table = 0;
            int iTotalConnectorsNumber_Table = 0;
            foreach (QuotationItem item in quotation)
            {
                dTotalConnectorsMass_Table += item.TotalMass;
                dTotalConnectorsPrice_Table += item.TotalPrice;
                iTotalConnectorsNumber_Table += item.Quantity;
            }

            //To Mato...toto tu treba???
            // TO Ondrej
            // Tie kontroly znikli pre to aby som mal istotu ze som vsetko spravne pridal z modelu do zoznamov
            // V debugu by sa nam to mohlo zist aby sme nic nevynechali ani neodfiltrovali

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
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalConnectorsPrice_Table;

            // Create Table
            DataTable dt = new DataTable("Connectors (Anchors, Bolts, Screws, Rivets)");

            // Create Table Rows
            dt.Columns.Add(colProp_Prefix.ColumnName, colProp_Prefix.DataType);
            dt.Columns.Add(colProp_Count.ColumnName, colProp_Count.DataType);
            dt.Columns.Add(colProp_Material.ColumnName, colProp_Material.DataType);
            dt.Columns.Add(colProp_Size.ColumnName, colProp_Size.DataType);
            dt.Columns.Add(colProp_UnitMass_P.ColumnName, colProp_UnitMass_P.DataType);
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_P_NZD.ColumnName, colProp_UnitPrice_P_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

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
                    row[colProp_Prefix.ColumnName] = item.Prefix;
                    row[colProp_Count.ColumnName] = item.Quantity;
                    row[colProp_Material.ColumnName] = item.MaterialName;
                    row[colProp_Size.ColumnName] = item.Name;
                    row[colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            // Add Sum
            row = dt.NewRow();
            row[colProp_Prefix.ColumnName] = "Total:";
            row[colProp_Count.ColumnName] = iTotalConnectorsNumber_Table;
            row[colProp_Material.ColumnName] = "";
            row[colProp_Size.ColumnName] = "";
            row[colProp_UnitMass_P.ColumnName] = "";
            row[colProp_TotalMass.ColumnName] = dTotalConnectorsMass_Table.ToString("F2");
            row[colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = dTotalConnectorsPrice_Table.ToString("F2");
            dt.Rows.Add(row);

            Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        }

        private void Datagrid_Connectors_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Connectors);
        }

        private void AddConnector(CConnector connector, List<QuotationItem> quotation, int iQuantity)
        {
            string sPrefix = "";
            string name = "";
            float fTotalMass = iQuantity * connector.Mass;
            float fUnitPrice = connector.Price_PPP_NZD;

            if (connector.Price_PPP_NZD > 0)
                fUnitPrice = connector.Price_PPP_NZD;
            else
            {
                if (connector is CScrew) fUnitPrice = fTEK_PricePerPiece_Screws_Total;
                else if (connector is CAnchor) fUnitPrice = (fAnchor_PricePerLength * connector.Length);
            }

            if (connector is CScrew)
            {
                sPrefix = connector.Name;
                int iGauge = ((CScrew)connector).Gauge;
                name = $"{iGauge}g x {Math.Round(connector.Length * 1000, 0)}"; // Display in [mm] (value * 1000)
            }
            else if (connector is CAnchor)
            {
                sPrefix = connector.Prefix;
                name = $"{connector.Name} x {Math.Round(connector.Length * 1000, 0)}"; // Display in [mm] (value * 1000)
            }

            float fTotalPrice = iQuantity * fUnitPrice;

            QuotationItem item = quotation.FirstOrDefault(q => q.Prefix == sPrefix &&
                        MathF.d_equal(q.Diameter_thread, connector.Diameter_thread) &&
                        MathF.d_equal(q.Length, connector.Length) &&
                        MathF.d_equal(q.MassPerPiece, connector.Mass));

            if (item != null)
            {
                item.Quantity += iQuantity; // Add one connector (piece) to the quantity
                item.TotalMass = item.Quantity * item.MassPerPiece;// Recalculate total mass of all connectors in the group
                item.TotalPrice = item.Quantity * item.PricePerPiece;// Recalculate total price of all connectors in the group
            }
            else
            {
                QuotationItem newItem = new QuotationItem
                {
                    Prefix = sPrefix,
                    Quantity = iQuantity,
                    MaterialName = connector.m_Mat.Name,
                    Diameter_thread = connector.Diameter_thread,
                    Length = connector.Length,
                    Name = name,
                    MassPerPiece = connector.Mass,
                    TotalMass = fTotalMass,
                    PricePerPiece = fUnitPrice,
                    TotalPrice = fTotalPrice
                };
                quotation.Add(newItem);
            }
        }

        private void CreateTableBoltNuts(CModel model)
        {
            // Bolt nuts
            List<QuotationItem> quotation = new List<QuotationItem>();

            for (int i = 0; i < model.m_arrConnectionJoints.Count; i++) // For each joint
            {
                if (!model.m_arrConnectionJoints[i].BIsSelectedForMaterialList) continue;

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
                            // TO Ondrej  - na 90 % su rovnake, teoereticky by mohol niekto mat hornu maticu nejaku specialnu inu ako spodne zabetonovane v betone, priemer musi byt rovnaky
                            foreach (CNut nut in anchor.Nuts)
                            {
                                AddBoltNutToQuotation(nut, quotation, anchorsNum);
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
            dt.Columns.Add(colProp_Prefix.ColumnName, colProp_Prefix.DataType);
            dt.Columns.Add(colProp_Count.ColumnName, colProp_Count.DataType);
            dt.Columns.Add(colProp_Material.ColumnName, colProp_Material.DataType);
            dt.Columns.Add(colProp_Size.ColumnName, colProp_Size.DataType);
            dt.Columns.Add(colProp_UnitMass_P.ColumnName, colProp_UnitMass_P.DataType);
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_P_NZD.ColumnName, colProp_UnitPrice_P_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

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
                    row[colProp_Prefix.ColumnName] = item.Prefix;
                    row[colProp_Count.ColumnName] = item.Quantity;
                    row[colProp_Material.ColumnName] = item.MaterialName;
                    row[colProp_Size.ColumnName] = item.Name;
                    row[colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            // Add Sum
            row = dt.NewRow();
            row[colProp_Prefix.ColumnName] = "Total:";
            row[colProp_Count.ColumnName] = iTotalNutsNumber_Table;
            row[colProp_Material.ColumnName] = "";
            row[colProp_Size.ColumnName] = "";
            row[colProp_UnitMass_P.ColumnName] = "";
            row[colProp_TotalMass.ColumnName] = dTotalNutsMass_Table.ToString("F2");
            row[colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = dTotalNutsPrice_Table.ToString("F2");
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

            //-----------------------------------------------------------------------------
            // TODO Ondrej - Toto treba refaktorovat s MainWindow.xaml.cs
            // TODO 438
            List<CTS_CoatingProperties> coatingsProperties = CTrapezoidalSheetingManager.LoadCoatingPropertiesList();

            CTS_CrscProperties prop_RoofCladding = new CTS_CrscProperties();
            prop_RoofCladding = CTrapezoidalSheetingManager.GetSectionProperties($"{vm.RoofCladding}-{vm.RoofCladdingThickness}");

            CTS_CrscProperties prop_WallCladding = new CTS_CrscProperties();
            prop_WallCladding = CTrapezoidalSheetingManager.GetSectionProperties($"{vm.WallCladding}-{vm.WallCladdingThickness}");

            CTS_CoatingProperties prop_RoofCladdingCoating = new CTS_CoatingProperties();
            prop_RoofCladdingCoating = CTrapezoidalSheetingManager.LoadCoatingProperties(vm.RoofCladdingCoating);

            CTS_CoatingProperties prop_WallCladdingCoating = new CTS_CoatingProperties();
            prop_WallCladdingCoating = CTrapezoidalSheetingManager.LoadCoatingProperties(vm.WallCladdingCoating);

            CoatingColour prop_RoofCladdingColor = vm.RoofCladdingColors.ElementAtOrDefault(vm.RoofCladdingColorIndex); // TODO Ondrej - pre Formclad a vyber color Zinc potrebujem vratit spravnu farbu odpovedajuce ID = 18 v databaze
            CoatingColour prop_WallCladdingColor = vm.WallCladdingColors.ElementAtOrDefault(vm.WallCladdingColorIndex);

            CTS_CoilProperties prop_RoofCladdingCoil = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(vm.RoofCladdingCoatingIndex), prop_RoofCladdingColor, prop_RoofCladding); // Ceny urcujeme podla coating a color
            CTS_CoilProperties prop_WallCladdingCoil = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(vm.WallCladdingCoatingIndex), prop_WallCladdingColor, prop_WallCladding); // Ceny urcujeme podla coating a color

            float fRoofCladdingUnitMass_kg_m2 = (float)(prop_RoofCladdingCoil.mass_kg_lm / prop_RoofCladding.widthModular_m);
            float fWallCladdingUnitMass_kg_m2 = (float)(prop_WallCladdingCoil.mass_kg_lm / prop_WallCladding.widthModular_m);
            //-----------------------------------------------------------------------------

            float fRoofCladdingPrice_PSM_NZD = (float)(prop_RoofCladdingCoil.price_PPLM_NZD / prop_RoofCladding.widthModular_m);
            float fWallCladdingPrice_PSM_NZD = (float)(prop_WallCladdingCoil.price_PPLM_NZD / prop_WallCladding.widthModular_m);

            float fRoofCladdingPrice_Total_NZD = fRoofArea_Total_Netto * fRoofCladdingPrice_PSM_NZD;
            float fWallCladdingPrice_Total_NZD = fWallArea_Total_Netto * fWallCladdingPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("Cladding");
            // Create Table Rows
            dt.Columns.Add(colProp_Cladding.ColumnName, colProp_Cladding.DataType);
            dt.Columns.Add(colProp_Thickness_mm.ColumnName, colProp_Thickness_mm.DataType);
            dt.Columns.Add(colProp_Coating.ColumnName, colProp_Coating.DataType);
            dt.Columns.Add(colProp_Color.ColumnName, colProp_Color.DataType);
            dt.Columns.Add(colProp_ColorName.ColumnName, colProp_ColorName.DataType);
            //dt.Columns.Add("TotalLength", typeof(String)); // Dalo by sa spocitat ak podelime plochu sirkou profilu
            dt.Columns.Add(colProp_TotalArea_m2.ColumnName, colProp_TotalArea_m2.DataType);
            dt.Columns.Add(colProp_UnitMass_SM.ColumnName, colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_SM_NZD.ColumnName, colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

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

                float fUnitMass = fRoofCladdingUnitMass_kg_m2;
                float totalMass = fRoofArea_Total_Netto * fUnitMass;
                try
                {
                    row[colProp_Cladding.ColumnName] = vm.RoofCladding;
                    row[colProp_Thickness_mm.ColumnName] = (prop_RoofCladding.thicknessCore_m * 1000).ToString("F2"); // mm
                    row[colProp_Coating.ColumnName] = vm.RoofCladdingCoating;
                    row[colProp_Color.ColumnName] = prop_RoofCladdingColor.CodeHEX;
                    row[colProp_ColorName.ColumnName] = prop_RoofCladdingColor.Name;
                    row[colProp_TotalArea_m2.ColumnName] = fRoofArea_Total_Netto.ToString("F2");
                    SumTotalArea += fRoofArea_Total_Netto;

                    row[colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_SM_NZD.ColumnName] = fRoofCladdingPrice_PSM_NZD.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = fRoofCladdingPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fRoofCladdingPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (fWallArea_Total_Netto > 0) // Wall Cladding
            {
                row = dt.NewRow();

                float fUnitMass = fWallCladdingUnitMass_kg_m2;
                float totalMass = fWallArea_Total_Netto * fUnitMass;
                try
                {
                    row[colProp_Cladding.ColumnName] = vm.WallCladding;
                    row[colProp_Thickness_mm.ColumnName] = (prop_WallCladding.thicknessCore_m * 1000).ToString("F2"); // mm
                    row[colProp_Coating.ColumnName] = vm.WallCladdingCoating;
                    row[colProp_Color.ColumnName] = prop_WallCladdingColor.CodeHEX;
                    row[colProp_ColorName.ColumnName] = prop_WallCladdingColor.Name;

                    row[colProp_TotalArea_m2.ColumnName] = fWallArea_Total_Netto.ToString("F2");
                    SumTotalArea += fWallArea_Total_Netto;

                    row[colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_SM_NZD.ColumnName] = fWallCladdingPrice_PSM_NZD.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = fWallCladdingPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fWallCladdingPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[colProp_Cladding.ColumnName] = "Total:";
                row[colProp_Thickness_mm.ColumnName] = "";
                row[colProp_Coating.ColumnName] = "";
                row[colProp_Color.ColumnName] = "";
                row[colProp_ColorName.ColumnName] = "";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row[colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[colProp_UnitMass_SM.ColumnName] = "";
                row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
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
            if (dtrow == null) return;
            Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);
            Style newStyle = new Style(dtrow.GetType());

            newStyle.Setters.Add(bold);
            dtrow.Style = newStyle;
        }

        private void CreateTableDoorsAndWindows(CPFDViewModel vm)
        {
            fTotalAreaOfOpennings = 0;

            fRollerDoorTrimmerFlashing_TotalLength = 0;
            fRollerDoorLintelFlashing_TotalLength = 0;
            fRollerDoorLintelCapFlashing_TotalLength = 0;
            fPADoorTrimmerFlashing_TotalLength = 0;
            fPADoorLintelFlashing_TotalLength = 0;
            fWindowFlashing_TotalLength = 0;

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

                listOfOpenings.Add(new COpeningProperties(dp.sDoorType, dp.fDoorsWidth, dp.fDoorsHeight, dp.CoatingColor.ID, dp.Serie));
            }

            foreach (WindowProperties wp in vm.WindowBlocksProperties)
            {
                fTotalAreaOfOpennings += wp.fWindowsWidth * wp.fWindowsHeight;

                fWindowFlashing_TotalLength += (2 * wp.fWindowsWidth + 2 * wp.fWindowsHeight);

                listOfOpenings.Add(new COpeningProperties("Window", wp.fWindowsWidth, wp.fWindowsHeight, wp.CoatingColor.ID, null));
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
                    grOP.Count++;
                }
                else groupedOpenings.Add(op);
            }

            // Create Table
            DataTable dt = new DataTable("Doors and Windows");
            // Create Table Rows
            dt.Columns.Add(colProp_Opening.ColumnName, colProp_Opening.DataType);
            dt.Columns.Add(colProp_Width_m.ColumnName, colProp_Width_m.DataType);
            dt.Columns.Add(colProp_Height_m.ColumnName, colProp_Height_m.DataType);
            dt.Columns.Add(colProp_Count.ColumnName, colProp_Count.DataType);
            dt.Columns.Add(colProp_Area_m2.ColumnName, colProp_Area_m2.DataType);
            dt.Columns.Add(colProp_TotalArea_m2.ColumnName, colProp_TotalArea_m2.DataType);
            dt.Columns.Add(colProp_UnitMass_SM.ColumnName, colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_SM_NZD.ColumnName, colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(colProp_UnitPrice_P_NZD.ColumnName, colProp_UnitPrice_P_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            int SumCount = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (COpeningProperties prop in groupedOpenings)
            {
                AddOpeningItemRow(dt,
                            colProp_Opening.ColumnName,
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
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[colProp_Opening.ColumnName] = "Total:";
                row[colProp_Width_m.ColumnName] = "";
                row[colProp_Height_m.ColumnName] = "";
                row[colProp_Count.ColumnName] = SumCount.ToString();
                row[colProp_Area_m2.ColumnName] = "";
                row[colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[colProp_UnitMass_SM.ColumnName] = "";
                row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[colProp_UnitPrice_P_NZD.ColumnName] = "";
                row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_DoorsAndWindows.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_DoorsAndWindows.Loaded += Datagrid_DoorsAndWindows_Loaded;
            }
            else // TODO Ondrej - Tabulka je prazdna - nezobrazime ju
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

        private void CreateTableFibreglass(CPFDViewModel vm,
            float fFibreGlassArea_Roof,
            float fFibreGlassArea_Walls)
        {
            string roofFibreglassThickness = vm.RoofFibreglassThicknessTypes.ElementAtOrDefault(vm.RoofFibreglassThicknessIndex);
            string wallFibreglassThickness = vm.WallFibreglassThicknessTypes.ElementAtOrDefault(vm.WallFibreglassThicknessIndex);

            CFibreglassProperties prop_RoofFibreglass = new CFibreglassProperties();
            prop_RoofFibreglass = CFibreglassManager.GetFibreglassProperties($"{vm.RoofCladding}-{roofFibreglassThickness}");

            CFibreglassProperties prop_WallFibreglass = new CFibreglassProperties();
            prop_WallFibreglass = CFibreglassManager.GetFibreglassProperties($"{vm.WallCladding}-{wallFibreglassThickness}");

            float fRoofFibreGlassPrice_PSM_NZD = (float)prop_RoofFibreglass.price_PPSM_NZD; // Cena roof fibreglass za 1 m^2
            float fWallFibreGlassPrice_PSM_NZD = (float)prop_WallFibreglass.price_PPSM_NZD; // Cena wall fibreglass za 1 m^2

            float fRoofFibreGlassUnitMass_SM = (float)prop_RoofFibreglass.mass_kg_m2;
            float fWallFibreGlassUnitMass_SM = (float)prop_WallFibreglass.mass_kg_m2;

            float fRoofFibreGlassPrice_Total_NZD = fFibreGlassArea_Roof * fRoofFibreGlassPrice_PSM_NZD;
            float fWallFibreGlassPrice_Total_NZD = fFibreGlassArea_Walls * fWallFibreGlassPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("Fibreglass");
            // Create Table Rows
            dt.Columns.Add(colProp_Fibreglass.ColumnName, colProp_Fibreglass.DataType);
            dt.Columns.Add(colProp_Thickness_mm.ColumnName, colProp_Thickness_mm.DataType);
            dt.Columns.Add(colProp_Width_m.ColumnName, colProp_Width_m.DataType);
            dt.Columns.Add(colProp_TotalLength_m.ColumnName, colProp_TotalLength_m.DataType);
            dt.Columns.Add(colProp_TotalArea_m2.ColumnName, colProp_TotalArea_m2.DataType);
            dt.Columns.Add(colProp_UnitMass_SM.ColumnName, colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_SM_NZD.ColumnName, colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;

            if (fFibreGlassArea_Roof > 0) // Roof Cladding
            {
                row = dt.NewRow();

                float totalLength = fFibreGlassArea_Roof / (float)prop_RoofFibreglass.widthModular_m;
                float fUnitMass = fRoofFibreGlassUnitMass_SM;
                float totalMass = fFibreGlassArea_Roof * fUnitMass;
                try
                {
                    row[colProp_Fibreglass.ColumnName] = vm.RoofCladding;
                    row[colProp_Thickness_mm.ColumnName] = (prop_RoofFibreglass.thickness_m * 1000).ToString("F2"); // mm
                    row[colProp_Width_m.ColumnName] = prop_RoofFibreglass.widthModular_m.ToString("F2");
                    row[colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[colProp_TotalArea_m2.ColumnName] = fFibreGlassArea_Roof.ToString("F2");
                    SumTotalArea += fFibreGlassArea_Roof;

                    row[colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_SM_NZD.ColumnName] = fRoofFibreGlassPrice_PSM_NZD.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = fRoofFibreGlassPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fRoofFibreGlassPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (fFibreGlassArea_Walls > 0) // Wall Cladding
            {
                row = dt.NewRow();

                float totalLength = fFibreGlassArea_Walls / (float)prop_WallFibreglass.widthModular_m;
                float fUnitMass = fWallFibreGlassUnitMass_SM;
                float totalMass = fFibreGlassArea_Walls * fUnitMass;
                try
                {
                    row[colProp_Fibreglass.ColumnName] = vm.WallCladding;
                    row[colProp_Thickness_mm.ColumnName] = (prop_WallFibreglass.thickness_m * 1000).ToString("F2"); // mm
                    row[colProp_Width_m.ColumnName] = prop_WallFibreglass.widthModular_m.ToString("F2");
                    row[colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[colProp_TotalArea_m2.ColumnName] = fFibreGlassArea_Walls.ToString("F2");
                    SumTotalArea += fFibreGlassArea_Walls;

                    row[colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_SM_NZD.ColumnName] = fWallFibreGlassPrice_PSM_NZD.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = fWallFibreGlassPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fWallFibreGlassPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[colProp_Fibreglass.ColumnName] = "Total:";
                row[colProp_Thickness_mm.ColumnName] = "";
                row[colProp_Width_m.ColumnName] = "";
                row[colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
                row[colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[colProp_UnitMass_SM.ColumnName] = "";
                row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                Datagrid_Fibreglass.ItemsSource = ds.Tables[0].AsDataView();
                Datagrid_Fibreglass.Loaded += Datagrid_Fibreglass_Loaded;
            }
            else // TODO Ondrej - Tabulka je prazdna - nezobrazime ju
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
            List<CPlaneItemProperties> listOfProperties = CPlaneItemManager.LoadPlaneItemsProperties("RoofNetting");

            // Roof Netting and Sisalation
            // Roof Sisalation Foil
            // Roof Safe Net
            float fRoofSisalationFoilPrice_PSM_NZD = (float)listOfProperties[0].Price1_PPSM_NZD; // Cena roof foil za 1 m^2
            float fRoofSafeNetPrice_PSM_NZD = (float)listOfProperties[1].Price1_PPSM_NZD; // Cena roof net za 1 m^2

            float fRoofSisalationFoilUnitMass_SM = (float)listOfProperties[0].Mass_kg_m2;
            float fRoofSafeNetUnitMass_SM = (float)listOfProperties[1].Mass_kg_m2;

            float fRoofSisalationFoilPrice_Total_NZD = fRoofArea * fRoofSisalationFoilPrice_PSM_NZD;
            float fRoofSafeNetPrice_Total_NZD = fRoofArea * fRoofSafeNetPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("Roof Netting");
            // Create Table Rows
            dt.Columns.Add(colProp_Component.ColumnName, colProp_Component.DataType);
            dt.Columns.Add(colProp_TotalArea_m2.ColumnName, colProp_TotalArea_m2.DataType);
            dt.Columns.Add(colProp_UnitMass_SM.ColumnName, colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_SM_NZD.ColumnName, colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            AddSurfaceItemRow(dt,
                        colProp_Component.ColumnName,
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
                        colProp_Component.ColumnName,
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
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[colProp_Component.ColumnName] = "Total:";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row[colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[colProp_UnitMass_SM.ColumnName] = "";
                row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
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
            float fRoofRidgeFlashing_TotalLength = model.fL_tot;
            float fWallCornerFlashing_TotalLength = 4 * model.fH1_frame;
            float fBargeFlashing_TotalLength = 4 * fRoofSideLength;

            //To Mato - nie som si uplne isty, kde chceme toto nastavovat,ci tu, alebo vseobecne pri zmene modelu
            CAccessories_LengthItemProperties flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[0]);
            if(flashing != null) flashing.Length_total = fRoofRidgeFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[1]);
            if (flashing != null) flashing.Length_total = fRoofRidgeFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[2]);
            if (flashing != null) flashing.Length_total = fWallCornerFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[3]);
            if (flashing != null) flashing.Length_total = fBargeFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[4]);
            if (flashing != null) flashing.Length_total = fRollerDoorTrimmerFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[5]);
            if (flashing != null) flashing.Length_total = fRollerDoorLintelFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[6]);
            if (flashing != null) flashing.Length_total = fRollerDoorLintelCapFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[7]);
            if (flashing != null) flashing.Length_total = fPADoorTrimmerFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[8]);
            if (flashing != null) flashing.Length_total = fPADoorLintelFlashing_TotalLength;

            flashing = _pfdVM.Flashings.FirstOrDefault(f => f.Name == _pfdVM.FlashingsNames[9]);
            if (flashing != null) flashing.Length_total = fWindowFlashing_TotalLength;
            
            // Create Table
            DataTable dt = new DataTable("Flashings");
            // Create Table Rows
            dt.Columns.Add(colProp_Flashing.ColumnName, colProp_Flashing.DataType);
            dt.Columns.Add(colProp_Thickness_mm.ColumnName, colProp_Thickness_mm.DataType);
            dt.Columns.Add(colProp_Width_m.ColumnName, colProp_Width_m.DataType);
            dt.Columns.Add(colProp_Color.ColumnName, colProp_Color.DataType);
            dt.Columns.Add(colProp_ColorName.ColumnName, colProp_ColorName.DataType);
            dt.Columns.Add(colProp_TotalLength_m.ColumnName, colProp_TotalLength_m.DataType);
            dt.Columns.Add(colProp_UnitMass_LM.ColumnName, colProp_UnitMass_LM.DataType);
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_LM_NZD.ColumnName, colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (CAccessories_LengthItemProperties fl in _pfdVM.Flashings)
            {
                AddLengthItemRow(dt,
                            colProp_Flashing.ColumnName,
                            fl.Name,
                            fl.Thickness / 1000, //from [mm] to [m]
                            fl.Width_total,
                            fl.CoatingColor,
                            fl.Length_total,
                            fl.Mass_kg_lm,
                            fl.Mass_kg_lm * fl.Length_total,
                            fl.Price_PPLM_NZD,
                            fl.Price_PPLM_NZD * fl.Length_total,
                            ref SumTotalLength,
                            ref SumTotalMass,
                            ref SumTotalPrice);
            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            // Last row
            DataRow row;
            row = dt.NewRow();
            row[colProp_Flashing.ColumnName] = "Total:";
            row[colProp_Thickness_mm.ColumnName] = "";
            row[colProp_Width_m.ColumnName] = "";
            row[colProp_Color.ColumnName] = "";
            row[colProp_ColorName.ColumnName] = "";
            row[colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[colProp_UnitMass_LM.ColumnName] = "";
            row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
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
            float fGuttersTotalLength = 2 * model.fL_tot; // na dvoch okrajoch strechy
            
            //toto tu je len preto ak by sa nahodou neupdatoval gutters total length pri zmene modelu (mozno je aj lepsie to mat az tu)
            //_pfdVM.Gutters[0].Length_total = fGuttersTotalLength;            

            // Create Table
            DataTable dt = new DataTable("Gutters");
            // Create Table Rows
            dt.Columns.Add(colProp_Gutter.ColumnName, colProp_Gutter.DataType);
            dt.Columns.Add(colProp_Thickness_mm.ColumnName, colProp_Thickness_mm.DataType);
            dt.Columns.Add(colProp_Width_m.ColumnName, colProp_Width_m.DataType);
            dt.Columns.Add(colProp_Color.ColumnName, colProp_Color.DataType);
            dt.Columns.Add(colProp_ColorName.ColumnName, colProp_ColorName.DataType);
            dt.Columns.Add(colProp_TotalLength_m.ColumnName, colProp_TotalLength_m.DataType);
            dt.Columns.Add(colProp_UnitMass_LM.ColumnName, colProp_UnitMass_LM.DataType);
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_LM_NZD.ColumnName, colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (CAccessories_LengthItemProperties gutter in _pfdVM.Gutters)
            {
                //TO Mato - tu neviem co s tymto
                gutter.Length_total = fGuttersTotalLength;

                AddLengthItemRow(dt,
                        colProp_Gutter.ColumnName,
                        gutter.Name,
                        gutter.Thickness / 1000, // from [mm] to [m]
                        gutter.Width_total,
                        gutter.CoatingColor,
                        gutter.Length_total,
                        gutter.Mass_kg_lm,
                        gutter.Mass_kg_lm * gutter.Length_total,
                        gutter.Price_PPLM_NZD,
                        gutter.Price_PPLM_NZD * gutter.Length_total,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            }


            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            //if (dt.Rows.Count > 1) // Len ak su v tabulke rozne typy gutters // Zatial komentujem, dal by sa tym usetrit jeden riadok
            //{
            // Last row
            DataRow row;
            row = dt.NewRow();
            row[colProp_Gutter.ColumnName] = "Total:";
            row[colProp_Thickness_mm.ColumnName] = "";
            row[colProp_Width_m.ColumnName] = "";
            row[colProp_Color.ColumnName] = "";
            row[colProp_ColorName.ColumnName] = "";
            row[colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[colProp_UnitMass_LM.ColumnName] = "";
            row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);
            //}

            Datagrid_Gutters.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Gutters.Loaded += Datagrid_Gutters_Loaded;
        }

        private void Datagrid_Gutters_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Gutters);
        }

        private void CreateTableDownpipes(CModel model)
        {
            // Zatial bude natvrdo jeden riadok s poctom zvodov, prednastavenou dlzkou ako vyskou steny a farbou, rovnaky default ako gutter
            CAccessories_DownpipeProperties downpipe = _pfdVM.Downpipes[0];
            float fDownpipesTotalLength = downpipe.CountOfDownpipePoints * model.fH1_frame; // Pocet zvodov krat vyska steny
            downpipe.Length_total = fDownpipesTotalLength;
            
            double fDownpipesTotalMass = fDownpipesTotalLength * downpipe.Mass_kg_lm;
            double fDownpipesTotalPrice = fDownpipesTotalLength * downpipe.Price_PPLM_NZD;

            // Create Table
            DataTable dt = new DataTable("Downpipes");
            // Create Table Rows
            dt.Columns.Add(colProp_Downpipe.ColumnName, colProp_Downpipe.DataType);
            dt.Columns.Add(colProp_Diameter_mm.ColumnName, colProp_Diameter_mm.DataType);
            dt.Columns.Add(colProp_Color.ColumnName, colProp_Color.DataType);
            dt.Columns.Add(colProp_ColorName.ColumnName, colProp_ColorName.DataType);
            dt.Columns.Add(colProp_TotalLength_m.ColumnName, colProp_TotalLength_m.DataType);
            dt.Columns.Add(colProp_UnitMass_LM.ColumnName, colProp_UnitMass_LM.DataType);
            dt.Columns.Add(colProp_TotalMass.ColumnName, colProp_TotalMass.DataType);
            dt.Columns.Add(colProp_UnitPrice_LM_NZD.ColumnName, colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(colProp_TotalPrice_NZD.ColumnName, colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;

            if (fDownpipesTotalLength > 0 && fDownpipesTotalPrice > 0) // Add new row only if length and price are more than zero
            {
                row = dt.NewRow();

                try
                {
                    row[colProp_Downpipe.ColumnName] = downpipe.Name;
                    row[colProp_Diameter_mm.ColumnName] = (downpipe.Diameter * 1000f).ToString("F2"); // mm
                    row[colProp_Color.ColumnName] = downpipe.CoatingColor.CodeHEX;
                    row[colProp_ColorName.ColumnName] = downpipe.CoatingColor.Name;
                    row[colProp_TotalLength_m.ColumnName] = fDownpipesTotalLength.ToString("F2");
                    SumTotalLength += fDownpipesTotalLength;

                    row[colProp_UnitMass_LM.ColumnName] = downpipe.Mass_kg_lm.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = fDownpipesTotalMass.ToString("F2");
                    SumTotalMass += fDownpipesTotalMass;

                    row[colProp_UnitPrice_LM_NZD.ColumnName] = downpipe.Price_PPLM_NZD.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = fDownpipesTotalPrice.ToString("F2");
                    SumTotalPrice += fDownpipesTotalPrice;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            //if (dt.Rows.Count > 1) // Len ak su v tabulke rozne typy downpipes // Zatial komentujem, dal by sa tym usetrit jeden riadok
            //{
            // Last row
            row = dt.NewRow();
            row[colProp_Downpipe.ColumnName] = "Total:";
            row[colProp_Diameter_mm.ColumnName] = "";
            row[colProp_Color.ColumnName] = "";
            row[colProp_ColorName.ColumnName] = "";
            row[colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[colProp_UnitMass_LM.ColumnName] = "";
            row[colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);
            //}

            Datagrid_Downpipes.ItemsSource = ds.Tables[0].AsDataView();
            Datagrid_Downpipes.Loaded += Datagrid_Downpipes_Loaded;
        }

        private void Datagrid_Downpipes_Loaded(object sender, RoutedEventArgs e)
        {
            SetLastRowBold(Datagrid_Downpipes);
        }

        private void AddLengthItemRow(DataTable dt,
            string itemColumnName,
            string itemName,
            double thickness_m,
            double width,
            CoatingColour coatingColor,
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
                    row[colProp_Thickness_mm.ColumnName] = (thickness_m * 1000).ToString("F2"); // mm
                    row[colProp_Width_m.ColumnName] = width.ToString("F2");
                    row[colProp_Color.ColumnName] = coatingColor.CodeHEX;
                    row[colProp_ColorName.ColumnName] = coatingColor.Name;
                    row[colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[colProp_UnitMass_LM.ColumnName] = unitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_LM_NZD.ColumnName] = unitPrice.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = price.ToString("F2");
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

                    row[colProp_TotalArea_m2.ColumnName] = totalArea.ToString("F2");
                    SumTotalArea += totalArea;

                    row[colProp_UnitMass_SM.ColumnName] = unitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_SM_NZD.ColumnName] = unitPrice.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = price.ToString("F2");
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

                    row[colProp_Width_m.ColumnName] = width.ToString("F2");
                    row[colProp_Height_m.ColumnName] = height.ToString("F2");
                    row[colProp_Count.ColumnName] = count.ToString();
                    SumCount += count;

                    row[colProp_Area_m2.ColumnName] = area.ToString("F2");

                    row[colProp_TotalArea_m2.ColumnName] = totalArea.ToString("F2");
                    SumTotalArea += totalArea;

                    row[colProp_UnitMass_SM.ColumnName] = unitMass.ToString("F2");

                    row[colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[colProp_UnitPrice_SM_NZD.ColumnName] = unitPrice_PPSM.ToString("F2");
                    row[colProp_UnitPrice_P_NZD.ColumnName] = unitPrice_PPP.ToString("F2");

                    row[colProp_TotalPrice_NZD.ColumnName] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }

        private void SetColumnProperties(DataTable dt, ColumnProperties properties)
        {
            // Set Column Caption
            dt.Columns[properties.ColumnName].Caption = properties.Caption;

            // Set Extended properties
            if (properties.EP_Unit != null && properties.EP_Unit.Length > 1)
                dt.Columns[properties.ColumnName].ExtendedProperties.Add(sEP_Unit, properties.EP_Unit);

            dt.Columns[properties.ColumnName].ExtendedProperties.Add(sEP_Width, properties.EP_Width);
            dt.Columns[properties.ColumnName].ExtendedProperties.Add(sEP_Align, properties.EP_Alignment);
        }

        private void SetDataTableColumnProperties(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
                SetColumnProperties(dt, GetColumnProperties(c));
        }

        private ColumnProperties GetColumnProperties(DataColumn c)
        {
            ColumnProperties prop = null;

            if (colPropList != null)
            {
                prop = new ColumnProperties();
                prop = colPropList.Find(x => x.ColumnName == c.ColumnName);
            }

            return prop;
        }

        private void BtnDisplayOptions_Click(object sender, RoutedEventArgs e)
        {
            QuotationDisplayOptionsWindow window = new QuotationDisplayOptionsWindow(_pfdVM);
            window.ShowDialog();

            

        }

        private void ChangeGUIAccordingToDisplayOptions()
        {

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
        //                // Nastavime parametre plechu z databazy - TO Ondrej - toto by sa malo diat uz asi pri vytvarani plechov
        //                // Nie vsetky plechy budu mat parametre definovane v databaze
        //                // !!!! Treba doriesit presne rozmery pri vytvarani plates a zaokruhlovanie

        //                #region Base Plate
        //                // TO Ondrej Blok1 Plate START
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

        //                // TO Ondrej Blok1 Plate END
        //                // ----------------------------------------------------------------------------------------------------------------------------------------
        //                #endregion

        //                //temp
        //                // Anchors - WASHERS
        //                // TO Mato - nieco som skusal... chcelo by to asi mat jeden objekt na tieto veci a nie zoznamy kade tade
        //                //rovnako je asi problem,ze to nijako negrupujem...ale tak potreboval by som vediet na zaklade coho sa to bude grupovat

        //                // To Ondrej - K prvej vete nemam vyhrady. Urob ako sa to ma.
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
        //                        // TO Ondrej Blok2 Washer Plate Top START
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
        //                        // TO Ondrej Blok2 Washer Plate Top END
        //                        // ----------------------------------------------------------------------------------------------------------------------------------------
        //                        #endregion

        //                        #region Washer Bearing 
        //                        // TO Ondrej Blok3 Washer Bearing START
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
        //                        // TO Ondrej Blok3 Washer Bearing END
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
        //    // TO Ondrej
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

    class ColumnProperties : DataColumn
    {
        public float EP_Width;
        public string EP_Unit;
        public object EP_Alignment;

        public ColumnProperties() { }
        public ColumnProperties(Type cType, string sName, string sCaption, float fWidth, string sUnit, object eAlignment)
        {
            DataType = cType;
            ColumnName = sName;
            Caption = sCaption;

            EP_Width = fWidth;
            EP_Unit = sUnit;
            EP_Alignment = eAlignment;
        }
    }
}
