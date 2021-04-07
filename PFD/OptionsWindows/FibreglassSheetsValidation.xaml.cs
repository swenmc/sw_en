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
    public partial class FibreglassSheetsValidation : Window
    {
        private CPFDViewModel _pfdVM;

        public FibreglassSheetsValidation(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            // Create Table
            DataTable dt = new DataTable("FibreglassSheets");
            // Create Table Rows
            dt.Columns.Add("ID");
            dt.Columns.Add("Prefix");
            dt.Columns.Add("Name");
            //dt.Columns.Add("CladdingCoatingType");
            dt.Columns.Add("CladdingShape");
            //dt.Columns.Add("CladdingWidthRibModular");
            dt.Columns.Add("x");
            dt.Columns.Add("y");
            dt.Columns.Add("Area_brutto");
            dt.Columns.Add("Area_netto");
            //dt.Columns.Add("FTime");
            dt.Columns.Add("Opacity");
            dt.Columns.Add("LengthTotal");
            dt.Columns.Add("Width");
            //dt.Columns.Add("LengthTopLeft");
            //dt.Columns.Add("LengthTopRight");
            //dt.Columns.Add("LengthTopTip");
            //dt.Columns.Add("IsCanopy");
            dt.Columns.Add("ColorName");
            dt.Columns.Add("ControlPoint");
            dt.Columns.Add("PointText");
            //dt.Columns.Add("Mat");
            dt.Columns.Add("IsGenerated");
            dt.Columns.Add("IsDisplayed");
            //dt.Columns.Add("IsSelectedForDesign");
            dt.Columns.Add("IsSelectedForMaterialList");

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            DataRow row;

            if (_pfdVM.Model.m_arrGOCladding == null) return;
            CCladding cladding = _pfdVM.Model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return;

            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets())
            {
                row = dt.NewRow();
                row["ID"] = sheet.ID;
                row["Prefix"] = sheet.Prefix;
                row["Name"] = sheet.Name;
                //row["CladdingCoatingType"] = sheet.CladdingCoatingType;
                row["CladdingShape"] = sheet.CladdingShape;
                row["x"] = sheet.CoordinateInPlane_x.ToString("F3");
                row["y"] = sheet.CoordinateInPlane_y.ToString("F3");
                //row["CladdingWidthRibModular"] = sheet.CladdingWidthRibModular;
                row["Area_brutto"] = sheet.Area_brutto.ToString("F3");
                row["Area_netto"] = sheet.Area_netto.ToString("F3");
                //row["FTime"] = sheet.FTime;
                row["Opacity"] = sheet.Opacity;
                row["LengthTotal"] = sheet.LengthTotal.ToString("F3");
                row["Width"] = sheet.Width.ToString("F3");
                //row["LengthTopLeft"] = sheet.LengthTopLeft;
                //row["LengthTopRight"] = sheet.LengthTopRight;
                //row["LengthTopTip"] = sheet.LengthTopTip;
                //row["IsCanopy"] = sheet.IsCanopy;
                row["ColorName"] = sheet.ColorName;
                row["ControlPoint"] = sheet.ControlPoint.ToString(3);
                row["PointText"] = sheet.PointText.ToString(3);
                //row["Mat"] = sheet.m_Mat != null ? sheet.m_Mat.Name : "-"; ;
                row["IsGenerated"] = sheet.BIsGenerated;
                row["IsDisplayed"] = sheet.BIsDisplayed;
                //row["IsSelectedForDesign"] = sheet.BIsSelectedForDesign;
                row["IsSelectedForMaterialList"] = sheet.BIsSelectedForMaterialList;

                dt.Rows.Add(row);
            }

            Datagrid_FibreglassSheets.ItemsSource = ds.Tables[0].AsDataView();

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}