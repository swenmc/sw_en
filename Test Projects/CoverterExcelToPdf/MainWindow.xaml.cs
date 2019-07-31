using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace CoverterExcelToPdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnExportToPDFFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WaitWindow ww = new WaitWindow("PDF");
                    ww.Show();

                    string folder = dialog.SelectedPath;
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    FileInfo[] files = dirInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
                    if (files.Length == 0) { ww.Close(); MessageBox.Show("No .xlsx files in the directory.");  return; }



                    using (ExcelPackage resultPackage = new ExcelPackage())
                    {
                        //ExcelWorksheet resultSheet = resultPackage.Workbook.Worksheets.Add("Sheet 1");
                        int count = 1;

                        foreach (FileInfo fi in files)
                        {
                            using (ExcelPackage pckg = new ExcelPackage(fi))
                            {
                                var sheet = pckg.Workbook.Worksheets.FirstOrDefault();
                                if (sheet == null) continue;

                                //add new sheet
                                resultPackage.Workbook.Worksheets.Add("Sheet " + count, sheet);
                            }



                            //for (var r = 1; r <= sheet.Dimension.Rows; r++)
                            //{
                            //    for (var c = 1; c <= sheet.Dimension.Columns; c++)
                            //    {
                            //        var cellSrc = sheet.Cells[r, c];
                            //        var cellDest = resultSheet.Cells[r, c];

                            //        // Copy value
                            //        cellDest.Value = cellSrc.Value;

                            //        // Copy cell properties
                            //        cellDest.Style.Numberformat = cellSrc.Style.Numberformat;
                            //        cellDest.Style.Font.Bold = cellSrc.Style.Font.Bold;
                            //        // TODO... Add any additional properties that you may want to copy over
                            //    }
                            //}


                            //foreach (var sheet in pckg.Workbook.Worksheets)
                            //{
                            //    //check name of worksheet, in case that worksheet with same name already exist exception will be thrown by EPPlus
                            //    string workSheetName = sheet.Name;
                            //    foreach (var masterSheet in resultPackage.Workbook.Worksheets)
                            //    {
                            //        if (sheet.Name == masterSheet.Name)
                            //        {
                            //            workSheetName = string.Format("{0}_{1}", workSheetName, DateTime.Now.ToString("yyyyMMddhhssmmm"));
                            //        }
                            //    }

                            //    //add new sheet
                            //    masterPackage.Workbook.Worksheets.Add(workSheetName, sheet);
                            //}
                            count++;

                        }


                        string fileName = string.Format("{0}\\{1}", folder, "ExportAll.pdf");
                        string fileNameXLSX = string.Format("{0}\\{1}", folder, "ExportAll.xlsx");
                        //ExportToExcelDocument.ExportToExcel(fileNameXLSX, tableParams, "plates");
                        //CExportToPDF.SavePDFDocument(fileName);

                        try
                        {
                            using (FileStream fs = File.OpenWrite(fileNameXLSX))
                            {
                                resultPackage.SaveAs(fs);
                            }
                        }
                        catch (IOException ex)
                        {
                            // The process cannot access the file 'filename' because it is being used by another process
                            MessageBox.Show("The process cannot access the file because it is being used by another process.");
                            return;
                        }

                    }
                        
                }
            }
        }


        
        

    }
}
