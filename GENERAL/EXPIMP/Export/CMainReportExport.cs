using BaseClasses;
using MATH;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EXPIMP
{
    public class CMainReportExport
    {

        //private const string fontFamily = "Verdana";
        //private const string fontFamily = "Times New Roman";
        private const string fontFamily = "Calibri";

        //private static PdfDocument document = null;

        private static RenderTargetBitmap SaveViewPortContentAsImage(Viewport3D viewPort)
        {
            
            // Scale dimensions from 96 dpi to 600 dpi.
            double scale = 300 / 96;
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(scale * viewPort.ActualWidth),
                                                            (int)(scale * viewPort.ActualHeight),
                                                            scale * 96,
                                                            scale * 96, PixelFormats.Default);
            viewPort.InvalidateVisual();
            bmp.Render(viewPort);
            bmp.Freeze();
            SaveBitmapImage(bmp);
            return bmp;
            
        }

        private static void SaveBitmapImage(RenderTargetBitmap bmp)
        {
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create("ViewPort.png"))
            {
                png.Save(stm);
            }
        }

        /// <summary>
        /// Draw scaled 3Model to PDF
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="viewPort"></param>
        private static void DrawModel3D(XGraphics gfx, Viewport3D viewPort)
        {
            XImage image = XImage.FromBitmapSource(SaveViewPortContentAsImage(viewPort));
            //XImage image = XImage.FromFile("ViewPort.png");
            double scaleFactor = gfx.PageSize.Width / image.PointWidth;
            double scaledImageWidth = gfx.PageSize.Width;
            double scaledImageHeight = image.PointHeight * scaleFactor;

            gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);

            //gfx.DrawImage(image, image.Size.Width, image.Size.Height);
        }
                

        private static string GetReportPDFName()
        {
            int count = 0;
            string fileName = null;
            bool nameOK = false;
            while (!nameOK)
            {
                fileName = $"Report_{++count}.pdf";

                if (!System.IO.File.Exists(fileName)) nameOK = true;
            }
            return fileName;
        }
        

        public static void ReportAllDataToPDFFile(Viewport3D viewPort, List<string[]> tableParams)
        {
            PdfDocument s_document = new PdfDocument();
            s_document.Info.Title = "Export from software";
            //s_document.Info.Author = "";
            //s_document.Info.Subject = "Created with code snippets that show the use of graphical functions";
            //s_document.Info.Keywords = "PDFsharp, XGraphics";
            PdfPage page = s_document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            
            // Vykreslenie zobrazovanych textov a objektov do PDF - zoradene z hora
            DrawModel3D(gfx, viewPort);
            

            gfx.Dispose();

            //DrawCanvas_PDF(canvas, page, canvas.RenderSize.Width);

            //double height = DrawCanvasImage(gfx, canvas);
            //DrawImage(gfx);

            // Create demonstration pages
            //new LinesAndCurves().DrawPage(s_document.AddPage());
            //new Shapes().DrawPage(s_document.AddPage());
            //new Paths().DrawPage(s_document.AddPage());
            //new Text().DrawPage(s_document.AddPage());
            //new Images().DrawPage(s_document.AddPage());

            PdfPage page2 = s_document.AddPage();
            XGraphics gfx2 = XGraphics.FromPdfPage(page2);
            AddTableToDocument(gfx2, 50, tableParams);

            string fileName = GetReportPDFName();
            // Save the s_document...
            s_document.Save(fileName);
            // ...and start a viewer
            Process.Start(fileName);
        }



























        


        
        

        

       

        

        

        
        

        private static void DrawFSAddress(XGraphics gfx)
        {
            XFont font = new XFont(fontFamily, 6, XFontStyle.Regular);

            string sLine1 = "Enquires to:";
            string sLine2 = "FS Technologies Ltd";
            string sLine3 = "2-4 Waokauri Pl, Mangere";
            string sLine4 = "P.O.Box 23-718, Auckland";
            string sLine5 = "Telephone 09 275 0089";

            double dposition_x = 100;
            double dposition_y = 755;
            double drowheight = 9.5;

            gfx.DrawString(sLine1, font, XBrushes.Black, dposition_x, dposition_y);
            gfx.DrawString(sLine2, font, XBrushes.Black, dposition_x, dposition_y + 1 * drowheight);
            gfx.DrawString(sLine3, font, XBrushes.Black, dposition_x, dposition_y + 2 * drowheight);
            gfx.DrawString(sLine4, font, XBrushes.Black, dposition_x, dposition_y + 3 * drowheight);
            gfx.DrawString(sLine5, font, XBrushes.Black, dposition_x, dposition_y + 4 * drowheight);
        }

        private static void DrawProductionInfo(XGraphics gfx, CProductionInfo pInfo, CPlate plate)
        {
            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            XFont font = new XFont(fontFamily, 12, XFontStyle.Regular, options);
            XFont font2 = new XFont(fontFamily, 12, XFontStyle.Underline, options);

            gfx.DrawString("Job Number: ", font, XBrushes.Black, 10, 20);
            if (pInfo.JobNumber != null) gfx.DrawString(pInfo.JobNumber, font, XBrushes.Black, 100, 20);
            gfx.DrawString("Customer: ", font, XBrushes.Black, 10, 40);
            if (pInfo.Customer != null) gfx.DrawString(pInfo.Customer, font, XBrushes.Black, 100, 40);
            gfx.DrawString("Amount: ", font, XBrushes.Black, 10, 60);
            gfx.DrawString(pInfo.Amount.ToString(), font, XBrushes.Black, 100, 60);

            if (!plate.IsSymmetric())
            {
                gfx.DrawString("RH: ", font, XBrushes.Black, 40, 80);
                gfx.DrawString(pInfo.AmountRH.ToString(), font, XBrushes.Black, 100, 80);
                gfx.DrawString("LH: ", font, XBrushes.Black, 40, 100);
                gfx.DrawString(pInfo.AmountLH.ToString(), font, XBrushes.Black, 100, 100);
            }
        }

        private static void DrawProductionNotes(XGraphics gfx)
        {
            XFont font = new XFont(fontFamily, 12, XFontStyle.Regular);

            string sNote1 = "Screw Holes - Pre Drill to Ø = 5.7 mm";
            gfx.DrawString(sNote1, font, XBrushes.Black, 200, 700);
        }

        private static void DrawPlateInfo(XGraphics gfx, CPlate plate)
        {
            string plateNamePrefix = plate.Name;
            string plateName = "";
            decimal plateThickness = (decimal)plate.Ft * 1000; // Convert to mm
            float platePitch_rad = 0;

            if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;
                plateName = "Apex Plate";
                platePitch_rad = plateTemp.FSlope_rad;
            }
            else if (plate is CConCom_Plate_JB)
            {
                CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;
                plateName = "Apex Plate";
                platePitch_rad = plateTemp.FSlope_rad;
            }
            else if (plate is CConCom_Plate_JBS)
            {
                CConCom_Plate_JBS plateTemp = (CConCom_Plate_JBS)plate;
                plateName = "Apex Plate";
                platePitch_rad = plateTemp.FSlope_rad;
            }
            else if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KB)
            {
                CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KBS plateTemp = (CConCom_Plate_KBS)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KC)
            {
                CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KCS plateTemp = (CConCom_Plate_KCS)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KD)
            {
                CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KDS)
            {
                CConCom_Plate_KDS plateTemp = (CConCom_Plate_KDS)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KE)
            {
                CConCom_Plate_KE plateTemp = (CConCom_Plate_KE)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else
            {
                // Not defined
                platePitch_rad = 0;
                plateName = "";
            }

            decimal platePitch = (decimal)Math.Round(Geom2D.RadiansToDegrees(Math.Abs(platePitch_rad)), 1); // Display absolute value in deg, 1 decimal place

            XFont font1 = new XFont(fontFamily, 14, XFontStyle.Bold);
            XFont font2 = new XFont(fontFamily, 12, XFontStyle.Regular);

            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(plateNamePrefix + " (" + plateName + ")", font1, XBrushes.Black, new XRect(0, 20, gfx.PageSize.Width, 40));

            gfx.DrawString(Math.Round(plateThickness, 2).ToString(), font2, XBrushes.Black, 50, 730);
            gfx.DrawString("mm Plate", font2, XBrushes.Black, 80, 730);
            gfx.DrawString(platePitch.ToString(), font2, XBrushes.Black, 485, 730);
            gfx.DrawString("° Pitch", font2, XBrushes.Black, 513, 730);
        }

        private static double DrawCanvasImage(XGraphics gfx, Canvas canvas)
        {
            try
            {
                XImage image = XImage.FromBitmapSource(GetBitmapSourceFromCanvas(canvas));
                double scaleFactor = gfx.PageSize.Width / image.PointWidth;
                double scaledImageWidth = gfx.PageSize.Width;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
                return scaledImageHeight;
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        private static BitmapSource GetBitmapSourceFromCanvas(Canvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);
            return rtb;
        }

        private static void SaveImageFromCanvas(Canvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(50, 50, 250, 250));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = System.IO.File.OpenWrite("ImageFromCanvas.png"))
            {
                pngEncoder.Save(fs);
            }
        }


        private static void DrawImage(XGraphics gfx)
        {
            try
            {
                string jpegSamplePath = "fs-screen.jpg";
                XImage image = XImage.FromFile(jpegSamplePath);
                // Left position in point
                double x = (250 - image.PixelWidth * 72 / image.HorizontalResolution) / 2;
                gfx.DrawImage(image, x, 0);
            }
            catch (Exception ex)
            {

            }
        }

        private static void AddTableToDocument(XGraphics gfx, double offsetY, List<string[]> tableParams)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = GetSimpleTable(doc, tableParams);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(40), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        private static void AddPlatesTableToDocument(XGraphics gfx, double offsetY, List<string[]> tableParams)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = GetPlatesParamsTable(doc, gfx, tableParams);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(40), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        public static Table GetSimpleTable(Document document, List<string[]> tableParams)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.Borders.Width = 0.75;
            table.Format.Font.Name = fontFamily;

            Column column1 = table.AddColumn(Unit.FromCentimeter(7));
            column1.Format.Alignment = ParagraphAlignment.Left;
            Column column2 = table.AddColumn(Unit.FromCentimeter(2));
            column2.Format.Alignment = ParagraphAlignment.Left;
            Column column3 = table.AddColumn(Unit.FromCentimeter(4));
            column3.Format.Alignment = ParagraphAlignment.Right;
            Column column4 = table.AddColumn(Unit.FromCentimeter(2));
            column4.Format.Alignment = ParagraphAlignment.Left;

            foreach (string[] strParams in tableParams)
            {
                Row row = table.AddRow();
                //row.Shading.Color = Colors.PaleGoldenrod;
                Cell cell = row.Cells[0];
                cell.Shading.Color = MigraDoc.DocumentObjectModel.Colors.PaleGoldenrod;
                cell.AddParagraph(strParams[0]);
                cell = row.Cells[1];
                cell.AddParagraph(strParams[1]);
                cell = row.Cells[2];
                cell.AddParagraph(strParams[2]);
                cell = row.Cells[3];
                cell.AddParagraph(strParams[3]);
            }

            table.SetEdge(0, 0, 4, tableParams.Count, Edge.Box, BorderStyle.Single, 1.5, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            return table;
        }
        public static Table GetPlatesParamsTable(Document document, XGraphics gfx, List<string[]> tableParams)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.Borders.Width = 0.75;
            table.Format.Font.Name = fontFamily;

            //{ "ID", "Name", "Width", "Height", "Thickness", "Area", "Volume", "Mass", "Amount", "Amount Left", "Amount Right", "Mass Total", "Screws Plate", "Screws Total" };
            Column columnID = table.AddColumn(Unit.FromCentimeter(0.7));
            columnID.Format.Alignment = ParagraphAlignment.Center;
            Column columnName = table.AddColumn(Unit.FromCentimeter(1.1));
            columnName.Format.Alignment = ParagraphAlignment.Center;
            Column columnWidth = table.AddColumn(Unit.FromCentimeter(1.2));
            columnWidth.Format.Alignment = ParagraphAlignment.Center;
            Column columnHeight = table.AddColumn(Unit.FromCentimeter(1.2));
            columnHeight.Format.Alignment = ParagraphAlignment.Center;
            Column columnThickness = table.AddColumn(Unit.FromCentimeter(1.6));
            columnThickness.Format.Alignment = ParagraphAlignment.Center;
            Column columnArea = table.AddColumn(Unit.FromCentimeter(1.1));
            columnArea.Format.Alignment = ParagraphAlignment.Center;
            Column columnVolume = table.AddColumn(Unit.FromCentimeter(1.3));
            columnVolume.Format.Alignment = ParagraphAlignment.Center;
            Column columnMass = table.AddColumn(Unit.FromCentimeter(1.1));
            columnMass.Format.Alignment = ParagraphAlignment.Center;
            Column columnAmount = table.AddColumn(Unit.FromCentimeter(1.5));
            columnAmount.Format.Alignment = ParagraphAlignment.Center;
            Column columnAmountL = table.AddColumn(Unit.FromCentimeter(1.5));
            columnAmountL.Format.Alignment = ParagraphAlignment.Center;
            Column columnAmountR = table.AddColumn(Unit.FromCentimeter(1.5));
            columnAmountR.Format.Alignment = ParagraphAlignment.Center;
            Column columnMassTotal = table.AddColumn(Unit.FromCentimeter(1.8));
            columnMassTotal.Format.Alignment = ParagraphAlignment.Center;

            Column columnAmountScrewPlate = table.AddColumn(Unit.FromCentimeter(1.25));
            columnAmountScrewPlate.Format.Alignment = ParagraphAlignment.Center;
            Column columnAmountScrewTotal = table.AddColumn(Unit.FromCentimeter(1.25));
            columnAmountScrewTotal.Format.Alignment = ParagraphAlignment.Center;

            int columns = 0;
            int count = 0;
            foreach (string[] strParams in tableParams)
            {
                count++;
                Row row = table.AddRow();
                if (count == 1 || count == tableParams.Count) row.Format.Font.Bold = true;

                if (count > 1)
                {
                    row.Format.Alignment = ParagraphAlignment.Right;
                    row.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                }

                //row.Shading.Color = Colors.PaleGoldenrod;
                Cell cell = row.Cells[0];
                cell.Shading.Color = MigraDoc.DocumentObjectModel.Colors.PaleGoldenrod;
                cell.AddParagraph(strParams[0]);
                cell = row.Cells[1];
                cell.Shading.Color = MigraDoc.DocumentObjectModel.Colors.PaleGoldenrod;
                cell.AddParagraph(strParams[1]);

                // Insert column data ID 2 - 10
                for (int i = 2; i < strParams.Length - 3; i++)
                {
                    cell = row.Cells[i];

                    cell.AddParagraph(strParams[i]);
                }

                cell = row.Cells[11];
                cell.Shading.Color = MigraDoc.DocumentObjectModel.Colors.LightCyan;
                cell.AddParagraph(strParams[11]);

                cell = row.Cells[12];
                cell.AddParagraph(strParams[12]);

                cell = row.Cells[13];
                cell.Shading.Color = MigraDoc.DocumentObjectModel.Colors.LightCyan;
                cell.AddParagraph(strParams[13]);

                columns = strParams.Length;
            }

            table.SetEdge(0, 0, columns, tableParams.Count, Edge.Box, BorderStyle.Single, 1.5, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            return table;
        }
    }
}
