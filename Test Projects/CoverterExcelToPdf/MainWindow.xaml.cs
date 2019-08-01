using GemBox.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using PdfRpt.Core.Contracts;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace CoverterExcelToPdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        double Progress;
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        FileInfo databaseFile;
        FileInfo[] files;
        double step;
        string folder;

        Stopwatch stopWatch = new Stopwatch();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            _worker.DoWork += Export;
        }

        public void Run()
        {
            if (!_worker.IsBusy) _worker.RunWorkerAsync();
        }

        private void BtnExportToPDFFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            
            Progress = 0;
            UpdateProgress();
            step = 0;
            UpdateText("Export is starting...");

            // Show progress time
            DisplayCalculationTime();

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WaitWindow ww = new WaitWindow("PDF");
                    ww.Show();

                    folder = dialog.SelectedPath;
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    files = dirInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
                    if (files.Length == 0) { ww.Close(); MessageBox.Show("No .xlsx files in the directory."); return; }
                    
                    databaseFile = files.FirstOrDefault(f => f.Name.Contains("DATABASE.xlsx"));
                    //if (databaseFile != null) Process.Start(databaseFile.FullName); // Otvaram Satabase file ako workbook v Exceli, preto som toto zakomentoval
                    
                    int totalFilesToExportCount = GetFilesToExportCount(files, databaseFile);
                    step = 100 / totalFilesToExportCount;

                    Run();

                    ww.Close();
                }
            }
        }

        private void Export(object sender, DoWorkEventArgs e)
        {
            ExportToPDFFromDirectory();
        }

        [STAThread]
        private void ExportToPDFFromDirectory()
        {
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

                try
                {
                    Progress += step;
                    UpdateProgress();
                    UpdateText($"Converting {fi.Name} to PDF");

                    Workbook wkb = app.Workbooks.Open(fi.FullName);
                    var firstSheeet = wkb.Sheets[1];

                    // Export only first worrksheet, not whole workbook
                    firstSheeet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, fi.FullName.Substring(0, fi.FullName.Length - 5) + ".pdf");

                    wkb.Close(false);
                }
                catch (Exception ex) { }
            }

            // To Ondrej - tu by som subor DATABASE.xlsx zavrel
            // Close database file
            if (wkbDatabase != null)
                wkbDatabase.Close(false); // Neukladat zmeny - TO Ondrej - toto false akosi nefunguje a subor sa nezavrie

            // Close Excel
            app.Quit();

            UpdateText("Merging documents into one PDF");
            MergePDFDocuments(folder);

            Progress = 100;
            UpdateProgress();
            UpdateText("Done.");

            // Stop timer
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                stopWatch.Reset(); // Set to 0:0:0
            }
        }


        public void UpdateProgress()
        {
            Dispatcher.Invoke(() =>
            {
                myProgressBar.Value = Progress;
            });
        }
        public void UpdateText(string text)
        {
            Dispatcher.Invoke(() =>
            {
               //LabelProgress.Content = text;
            });
        }

        private static int GetFilesToExportCount(FileInfo[] files, FileInfo databaseFile)
        {
            int totalFilesToExportCount = 0;
            foreach (FileInfo fi in files)
            {
                if (!fi.Extension.Equals(".xlsx")) continue;
                if (fi.Name.Equals(databaseFile.Name)) continue;
                totalFilesToExportCount++;
            }
            return totalFilesToExportCount;
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
            string filename = directory + "\\Result.pdf";
            outputDocument.Save(filename);
            
            // ...and start a viewer.
            Process.Start(filename);
        }

        public void DisplayCalculationTime()
        {
            dispatcherTimer.Tick += new EventHandler(dt_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);

            stopWatch.Start();
            dispatcherTimer.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                string currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                LabelTimer.Text = currentTime;
            }
        }





        //private void Export(object sender, DoWorkEventArgs e)
        //{
        //    ExportToPDFFromDirectory();


        //    Progress = 0;
        //    UpdateProgress();
        //    int step = 0;
        //    UpdateText("Export is starting...");


        //    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
        //    {
        //        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            WaitWindow ww = new WaitWindow("PDF");
        //            ww.Show();

        //            string folder = dialog.SelectedPath;
        //            DirectoryInfo dirInfo = new DirectoryInfo(folder);
        //            FileInfo[] files = dirInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
        //            if (files.Length == 0) { ww.Close(); MessageBox.Show("No .xlsx files in the directory."); return; }

        //            //SpreadsheetInfo.SetLicense("FREE - LIMITED - KEY");

        //            FileInfo databaseFile = files.FirstOrDefault(f => f.Name.Contains("DATABASE.xlsx"));
        //            //if (databaseFile != null) Process.Start(databaseFile.FullName); // Otvaram Satabase file ako workbook v Exceli, preto som toto zakomentoval

        //            // Open Excel
        //            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();

        //            // Open database file
        //            Workbook wkbDatabase = null;
        //            if (databaseFile != null)
        //                wkbDatabase = app.Workbooks.Open(databaseFile.FullName);

        //            int totalFilesToExportCount = GetFilesToExportCount(files, databaseFile);
        //            step = 100 / totalFilesToExportCount;

        //            foreach (FileInfo fi in files)
        //            {
        //                if (!fi.Extension.Equals(".xlsx")) continue;
        //                if (fi.Name.Equals(databaseFile.Name)) continue;

        //                try
        //                {
        //                    Progress += step;
        //                    UpdateProgress();
        //                    UpdateText($"Converting {fi.Name} to PDF");

        //                    Workbook wkb = app.Workbooks.Open(fi.FullName);
        //                    var firstSheeet = wkb.Sheets[1];

        //                    // Export only first worrksheet, not whole workbook
        //                    firstSheeet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, fi.FullName.Substring(0, fi.FullName.Length - 5) + ".pdf");

        //                    wkb.Close(false);
        //                }
        //                catch (Exception ex) { }
        //            }

        //            // To Ondrej - tu by som subor DATABASE.xlsx zavrel
        //            // Close database file
        //            if (wkbDatabase != null)
        //                wkbDatabase.Close(false); // Neukladat zmeny - TO Ondrej - toto false akosi nefunguje a subor sa nezavrie

        //            // Close Excel
        //            app.Quit();

        //            UpdateText("Merging documents into one PDF");
        //            MergePDFDocuments(folder);

        //            ww.Close();
        //            Progress = 100;
        //            UpdateProgress();
        //            UpdateText("Done.");
        //        }
        //    }
        //}





    }
}
