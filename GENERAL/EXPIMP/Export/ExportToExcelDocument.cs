using OfficeOpenXml;
using System;
using System.Data;
using System.IO;

namespace EXPIMP
{
    public class ExportToExcelDocument
    {
        public static void Export_to_Excel(string filename, DataTable dt, string workSheetName, bool withTimeStamp)
        {
            if (withTimeStamp)
            {
                DateTime d = DateTime.Now;
                filename = string.Format("{0}_{1}{2:00}{3:00}T{4:00}{5:00}{6:00}.xlsx", filename, d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
            }

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(workSheetName);
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

            workSheet.Row(1).Style.Font.Bold = true;
            for (int i = 1; i <= dt.Columns.Count; i++)
            {
                workSheet.Column(i).AutoFit();
            }

            //if (workSheet.Dimension != null) //not empty
            //{
            //    int rozdielColumn = workSheet.Dimension.Columns - COLUMNS_FROM_ROZDIEL_TO_END;
            //    int rozdiel = 0;
            //    int rozdielPlusSklad = 0;
            //    for (var i = 2; i <= workSheet.Dimension.End.Row; i++)
            //    {
            //        int.TryParse(workSheet.Cells[i, rozdielColumn].Text, out rozdiel);
            //        if (rozdiel < 0)
            //        {
            //            workSheet.Cells[i, rozdielColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //            workSheet.Cells[i, rozdielColumn].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
            //        }
            //        int.TryParse(workSheet.Cells[i, rozdielColumn + 1].Text, out rozdielPlusSklad);
            //        if (rozdielPlusSklad < 0)
            //        {
            //            workSheet.Cells[i, rozdielColumn + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //            workSheet.Cells[i, rozdielColumn + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
            //        }
            //    }
            //}

            using (FileStream fs = File.OpenWrite(filename))
            {
                excel.SaveAs(fs);
            }                
        }


    }
}

