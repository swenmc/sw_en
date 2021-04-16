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

            double fibreglassSheetsArea = 0;
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets())
            {
                fibreglassSheetsArea += sheet.Area_netto;
            }

            TxtTotalSheetsAreaCladding.Text = claddingSheetsArea.ToString("F3");
            TxtTotalSheetsAreaFibreglass.Text = fibreglassSheetsArea.ToString("F3");
            TxtTotalCladdingArea.Text = (_pfdVM.TotalWallArea + _pfdVM.TotalRoofAreaInclCanopies).ToString("F3");

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        private string GetLengthTopTip(CCladdingOrFibreGlassSheet sheet)
        {
            if (MathF.d_equal(sheet.LengthTopTip, sheet.LengthTopLeft) && MathF.d_equal(sheet.LengthTopTip, sheet.LengthTopRight)) return "";
            else return sheet.LengthTopTip.ToString("F3");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}