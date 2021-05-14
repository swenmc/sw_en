using System;
using System.Collections.Generic;
using System.Data;
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
using BaseClasses;
using BaseClasses.GraphObj;
using MATH;

namespace PFD
{
    public partial class CladdingSheetsValidation : Window
    {
        private CPFDViewModel _pfdVM;

        public CladdingSheetsValidation(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;



            // Create Table
            DataTable dt = new DataTable("CladdingSheets");
            // Create Table Rows
            dt.Columns.Add("ID");
            dt.Columns.Add("Prefix");
            dt.Columns.Add("Name");
            dt.Columns.Add("CladdingCoatingType");
            dt.Columns.Add("CladdingShape");
            dt.Columns.Add("CladdingWidthRibModular");
            dt.Columns.Add("Area_brutto");
            dt.Columns.Add("Area_netto");
            //dt.Columns.Add("FTime");
            dt.Columns.Add("Opacity");
            dt.Columns.Add("Width");
            dt.Columns.Add("LengthTotal");
            dt.Columns.Add("LengthTopLeft");
            dt.Columns.Add("LengthTopRight");
            dt.Columns.Add("LengthTopTip");
            dt.Columns.Add("NumberOfEdges");
            //dt.Columns.Add("IsCanopy");
            dt.Columns.Add("ColorName");
            dt.Columns.Add("ControlPoint");
            dt.Columns.Add("PointText");
            dt.Columns.Add("Mat");
            dt.Columns.Add("IsGenerated");
            dt.Columns.Add("IsDisplayed");
            dt.Columns.Add("IsSelectedForDesign");
            dt.Columns.Add("IsSelectedForMaterialList");

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            DataRow row;
            if (_pfdVM.Model.m_arrGOCladding == null) return;
            CCladding cladding = _pfdVM.Model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return;

            double claddingSheetsArea = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets())
            {
                claddingSheetsArea += sheet.Area_netto;

                row = dt.NewRow();
                row["ID"] = sheet.ID;
                row["Prefix"] = sheet.Prefix;
                row["Name"] = sheet.Name;
                row["CladdingCoatingType"] = sheet.CladdingCoatingType;
                row["CladdingShape"] = sheet.CladdingShape;
                row["CladdingWidthRibModular"] = sheet.CladdingWidthRibModular;
                row["Area_brutto"] = sheet.Area_brutto.ToString("F3");
                row["Area_netto"] = sheet.Area_netto.ToString("F3");
                //row["FTime"] = sheet.FTime;
                row["Opacity"] = sheet.Opacity;
                row["Width"] = sheet.Width.ToString("F3");
                row["LengthTotal"] = sheet.LengthTotal.ToString("F3");
                row["LengthTopLeft"] = sheet.LengthTopLeft.ToString("F3");
                row["LengthTopRight"] = sheet.LengthTopRight.ToString("F3");
                row["LengthTopTip"] = GetLengthTopTip(sheet);
                row["NumberOfEdges"] = sheet.NumberOfEdges;
                //row["IsCanopy"] = sheet.IsCanopy;
                row["ColorName"] = sheet.ColorName;
                row["ControlPoint"] = sheet.ControlPoint.ToString(3);
                row["PointText"] = sheet.PointText.ToString(3);
                row["Mat"] = sheet.m_Mat != null ? sheet.m_Mat.Name : "-"; ;
                row["IsGenerated"] = sheet.BIsGenerated;
                row["IsDisplayed"] = sheet.BIsDisplayed;
                row["IsSelectedForDesign"] = sheet.BIsSelectedForDesign;
                row["IsSelectedForMaterialList"] = sheet.BIsSelectedForMaterialList;

                dt.Rows.Add(row);
            }

            Datagrid_CladdingSheets.ItemsSource = ds.Tables[0].AsDataView();

            double claddingSheetsArea_Roof = 0;
            double claddingSheetsArea_Walls = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets_Roof())
            {
                claddingSheetsArea_Roof += sheet.Area_netto;
            }
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets_Wall())
            {
                claddingSheetsArea_Walls += sheet.Area_netto;
            }

            double fibreglassSheetsArea = 0;
            double fibreglassSheetsArea_Roof = 0;
            double fibreglassSheetsArea_Wall = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets())
            {
                fibreglassSheetsArea += sheet.Area_netto;
            }
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets_Roof())
            {
                fibreglassSheetsArea_Roof += sheet.Area_netto;
            }
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets_Wall())
            {
                fibreglassSheetsArea_Wall += sheet.Area_netto;
            }

            TxtTotalSheetsAreaCladding.Text = claddingSheetsArea.ToString("F3");
            TxtTotalSheetsAreaFibreglass.Text = fibreglassSheetsArea.ToString("F3");
            
            
            float fFibreGlassArea_Roof = _pfdVM._claddingOptionsVM.FibreglassAreaRoofRatio / 100f * _pfdVM.TotalRoofArea; // Priesvitna cast strechy (bez canopies)
            float fFibreGlassArea_Walls = _pfdVM._claddingOptionsVM.FibreglassAreaWallRatio / 100f * _pfdVM.TotalWallArea; // Priesvitna cast stien

            // Plocha stien bez otvorov a fibre glass
            float fWallArea_Total_Netto = _pfdVM.TotalWallArea - GetTotalAreaOpeningsDoorsAnWindows() - fFibreGlassArea_Walls;  //float fWallArea_Total,
            // Plocha strechy bez fibre glass
            float fRoofArea_Total_Netto = _pfdVM.TotalRoofAreaInclCanopies - fFibreGlassArea_Roof;    //float fRoofArea,
            
            TxtTotalCladdingArea.Text = (fWallArea_Total_Netto + fRoofArea_Total_Netto).ToString("F3");
            TxtTotalFibreglassArea.Text = (fFibreGlassArea_Roof + fFibreGlassArea_Walls).ToString("F3");
            
            TxtSheetsAreaCladding_Roof.Text = claddingSheetsArea_Roof.ToString("F3");
            TxtSheetsAreaCladding_Walls.Text = claddingSheetsArea_Walls.ToString("F3");
            TxtTotalCladdingArea_Roof.Text = fRoofArea_Total_Netto.ToString("F3");
            TxtTotalCladdingArea_Walls.Text = fWallArea_Total_Netto.ToString("F3");

            TxtSheetsAreaFibreglass_Roof.Text = fibreglassSheetsArea_Roof.ToString("F3");
            TxtSheetsAreaFibreglass_Walls.Text = fibreglassSheetsArea_Wall.ToString("F3");
            TxtTotalFibreglassArea_Roof.Text = fFibreGlassArea_Roof.ToString("F3");
            TxtTotalFibreglassArea_Walls.Text = fFibreGlassArea_Walls.ToString("F3");
            
            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }


        private float GetTotalAreaOpeningsDoorsAnWindows()
        {
            float fTotalAreaOfOpennings = 0;
            foreach (DoorProperties dp in _pfdVM._doorsAndWindowsVM.DoorBlocksProperties)
            {
                fTotalAreaOfOpennings += dp.fDoorsWidth * dp.fDoorsHeight;
            }

            foreach (WindowProperties wp in _pfdVM._doorsAndWindowsVM.WindowBlocksProperties)
            {
                fTotalAreaOfOpennings += wp.fWindowsWidth * wp.fWindowsHeight;
            }
            return fTotalAreaOfOpennings;
        }

        private string GetLengthTopTip(CCladdingOrFibreGlassSheet sheet)
        {
            if (sheet.NumberOfEdges == 4) return "";
            else if (MathF.d_equal(sheet.LengthTopTip, sheet.LengthTopLeft) && MathF.d_equal(sheet.LengthTopTip, sheet.LengthTopRight)) return "";
            else return sheet.LengthTopTip.ToString("F3");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}