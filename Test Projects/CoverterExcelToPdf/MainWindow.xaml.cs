using GemBox.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using PdfRpt.Core.Contracts;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void BtnExportToPDFFromDirectory_Click(object sender, RoutedEventArgs e)
        //{
        //    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
        //    {
        //        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            WaitWindow ww = new WaitWindow("PDF");
        //            ww.Show();

        //            string folder = dialog.SelectedPath;
        //            DirectoryInfo dirInfo = new DirectoryInfo(folder);
        //            FileInfo[] files = dirInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
        //            if (files.Length == 0) { ww.Close(); MessageBox.Show("No .xlsx files in the directory.");  return; }



        //            using (ExcelPackage resultPackage = new ExcelPackage())
        //            {
        //                //ExcelWorksheet resultSheet = resultPackage.Workbook.Worksheets.Add("Sheet 1");
        //                int count = 1;

        //                foreach (FileInfo fi in files)
        //                {
        //                    using (ExcelPackage pckg = new ExcelPackage(fi))
        //                    {
        //                        var sheet = pckg.Workbook.Worksheets.FirstOrDefault();
        //                        if (sheet == null) continue;

        //                        //add new sheet
        //                        resultPackage.Workbook.Worksheets.Add("Sheet " + count, sheet);
        //                    }



        //                    //for (var r = 1; r <= sheet.Dimension.Rows; r++)
        //                    //{
        //                    //    for (var c = 1; c <= sheet.Dimension.Columns; c++)
        //                    //    {
        //                    //        var cellSrc = sheet.Cells[r, c];
        //                    //        var cellDest = resultSheet.Cells[r, c];

        //                    //        // Copy value
        //                    //        cellDest.Value = cellSrc.Value;

        //                    //        // Copy cell properties
        //                    //        cellDest.Style.Numberformat = cellSrc.Style.Numberformat;
        //                    //        cellDest.Style.Font.Bold = cellSrc.Style.Font.Bold;
        //                    //        // TODO... Add any additional properties that you may want to copy over
        //                    //    }
        //                    //}


        //                    //foreach (var sheet in pckg.Workbook.Worksheets)
        //                    //{
        //                    //    //check name of worksheet, in case that worksheet with same name already exist exception will be thrown by EPPlus
        //                    //    string workSheetName = sheet.Name;
        //                    //    foreach (var masterSheet in resultPackage.Workbook.Worksheets)
        //                    //    {
        //                    //        if (sheet.Name == masterSheet.Name)
        //                    //        {
        //                    //            workSheetName = string.Format("{0}_{1}", workSheetName, DateTime.Now.ToString("yyyyMMddhhssmmm"));
        //                    //        }
        //                    //    }

        //                    //    //add new sheet
        //                    //    masterPackage.Workbook.Worksheets.Add(workSheetName, sheet);
        //                    //}
        //                    count++;

        //                }


        //                string fileName = string.Format("{0}\\{1}", folder, "ExportAll.pdf");
        //                string fileNameXLSX = string.Format("{0}\\{1}", folder, "ExportAll.xlsx");
        //                //ExportToExcelDocument.ExportToExcel(fileNameXLSX, tableParams, "plates");
        //                //CExportToPDF.SavePDFDocument(fileName);

        //                try
        //                {
        //                    using (FileStream fs = File.OpenWrite(fileNameXLSX))
        //                    {
        //                        resultPackage.SaveAs(fs);
        //                    }
        //                }
        //                catch (IOException ex)
        //                {
        //                    // The process cannot access the file 'filename' because it is being used by another process
        //                    MessageBox.Show("The process cannot access the file because it is being used by another process.");
        //                    return;
        //                }

        //            }

        //        }
        //    }
        //}


        private void BtnExportToPDFFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage resultPackage = new ExcelPackage();
            


            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WaitWindow ww = new WaitWindow("PDF");
                    ww.Show();

                    string folder = dialog.SelectedPath;
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    FileInfo[] files = dirInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
                    if (files.Length == 0) { ww.Close(); MessageBox.Show("No .xlsx files in the directory."); return; }

                    SpreadsheetInfo.SetLicense("FREE - LIMITED - KEY");

                    FileInfo databaseFile = files.FirstOrDefault(f => f.Name.Contains("DATABASE.xlsx"));
                    //if (databaseFile != null) Process.Start(databaseFile.FullName); // Otvaram Satabase file ako workbook v Exceli, preto som toto zakomentoval

                    // Open Excel
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();

                    // Open database file
                    Workbook wkbDatabase = null;
                    if (databaseFile != null)
                        wkbDatabase = app.Workbooks.Open(databaseFile.FullName);

                    foreach (FileInfo fi in files)
                    {
                        if (!fi.Extension.Equals(".xlsx")) continue;
                        if (fi.Name.Equals(databaseFile.Name)) continue;

                        //1
                        //ExcelFile excel = ExcelFile.Load(fi.FullName);
                        //excel.Save(fi.FullName.Substring(0, fi.FullName.Length - 5) +".pdf");

                        //2
                        //tato kniznica vie zase exportovat len tabulky, pokial je tam prvy riadok prazdny,tak ma problem
                        //ExcelToPdfReport r = new ExcelToPdfReport();
                        //r.CreateExcelToPdfReport(fi.FullName, null);

                        //3
                        try
                        {
                            Workbook wkb = app.Workbooks.Open(fi.FullName);
                            var firstSheeet = wkb.Sheets[1];

                            //for (int i = 2; i < wkb.Sheets.Count; i++) // To Ondrej - podla mna nemozes zmazat ostatne zalozky, zmazes si tak data, na ktorych je zavisly obsah prvej, upravil som to tak ze sa exportuje len prvy zosit
                            //{
                            //    wkb.Sheets[i].Delete();
                            //}

                            // Export only first worrksheet, not whole workbook
                            firstSheeet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, fi.FullName.Substring(0, fi.FullName.Length - 5) + ".pdf");
                        }
                        catch (Exception ex) {}
                    }

                    // To Ondrej - tu by som subor DATABASE.xlsx zavrel
                    // Close database file
                    if(wkbDatabase != null)
                    wkbDatabase.Close(false); // Neukladat zmeny

                    // Close Excel
                    app.Quit();

                    MergePDFDocuments(folder);

                    ww.Close();
                }
            }
        }

        static void MergePDFDocuments(string directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            FileInfo[] files = dirInfo.GetFiles("*.pdf", SearchOption.TopDirectoryOnly);
            if (files.Length == 0) { MessageBox.Show("No .pdf files in the directory."); return; }
            
            // Create the output document
            PdfDocument outputDocument = new PdfDocument();
            // Show consecutive pages facing. Requires Acrobat 5 or higher.
            outputDocument.PageLayout = PdfPageLayout.OneColumn;

            XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Far;
            XGraphics gfx;
            XRect box;

            foreach (FileInfo fi in files)
            {
                if (!fi.Extension.Equals(".pdf")) continue;
                // Open the input files
                PdfDocument inputDocument = PdfReader.Open(fi.FullName, PdfDocumentOpenMode.Import);

                for (int idx = 0; idx < inputDocument.PageCount; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    page = outputDocument.AddPage(page);

                    bool debugDocumentsAndPages = false;
                    if (debugDocumentsAndPages)
                    {
                        // Write document file name and page number on each page
                        gfx = XGraphics.FromPdfPage(page);
                        box = page.MediaBox.ToXRect();
                        box.Inflate(0, -10);
                        gfx.DrawString(String.Format("{0} • {1}", fi.FullName, idx + 1),
                          font, XBrushes.Red, box, format);
                    }
                }
            }
            
            // Save the document...
            string filename = "Result.pdf";
            outputDocument.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }











        

        }
}
