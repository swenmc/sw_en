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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EXPIMP
{
    public static class CExportToPDF
    {
        //public static void ExportCanvasToPDF(Canvas canvas, List<string[]> tableParams)
        //{
        //    CreatePDFFile(canvas, tableParams);
        //}

        public static void CreatePDFFile(Canvas canvas, List<string[]> tableParams, string jobNumber, string customer, int amount, string plateNamePrefix, string plateName, decimal thickness, decimal pitch)
        {
            // Create a temporary file
            DateTime d = DateTime.Now;
            string filename = string.Format("ExportPDF_{0}{1}{2}T{3}{4}{5}.pdf",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            //string filename = String.Format("{0}_tempfile.pdf", Guid.NewGuid().ToString("D").ToUpper());
            PdfDocument s_document = new PdfDocument();
            s_document.Info.Title = "Export from FormSteel software";
            //s_document.Info.Author = "";
            //s_document.Info.Subject = "Created with code snippets that show the use of graphical functions";
            //s_document.Info.Keywords = "PDFsharp, XGraphics";
            PdfPage page = s_document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Vykreslenie zobrazovanych textov a objektov do PDF - zoradene z hora
            DrawProductionInfo(gfx, jobNumber, customer, amount);
            DrawPlateInfo(gfx, plateNamePrefix, plateName, thickness, pitch);
            DrawCanvas_PDF(canvas, gfx);
            DrawProductionNotes(gfx);
            DrawLogo(gfx);
            DrawFSAddress(gfx);

            //double height = DrawCanvasImage(gfx, canvas);
            //DrawImage(gfx);

            // Create demonstration pages
            //new LinesAndCurves().DrawPage(s_document.AddPage());
            //new Shapes().DrawPage(s_document.AddPage());
            //new Paths().DrawPage(s_document.AddPage());
            //new Text().DrawPage(s_document.AddPage());
            //new Images().DrawPage(s_document.AddPage());

            gfx.Dispose();
            PdfPage page2 = s_document.AddPage();
            XGraphics gfx2 = XGraphics.FromPdfPage(page2);
            AddTableToDocument(gfx2, 50, tableParams);

            // Save the s_document...
            s_document.Save(filename);
            // ...and start a viewer
            Process.Start(filename);
        }

        public static void DrawCanvas_PDF(Canvas canvas, XGraphics gfx)
        {
            double scaleFactor = gfx.PageSize.Width / canvas.RenderSize.Width * 0.9; //90%
            double marginLeft = gfx.PageSize.Width * 0.1 / 2.0;
            double marginTop = gfx.PageSize.Height * 0.3 / 2.97;

            foreach (object o in canvas.Children)
            {
                System.Diagnostics.Trace.WriteLine(o.GetType());

                if (o is Rectangle)
                {
                    Rectangle winRect = o as Rectangle;
                    double x = Canvas.GetLeft(winRect);
                    double y = Canvas.GetTop(winRect);

                    System.Windows.Media.Color c = ((SolidColorBrush)winRect.Fill).Color;
                    XSolidBrush solidBrush = new XSolidBrush(XColor.FromArgb(c.A, c.R, c.G, c.B));
                    gfx.DrawRectangle(solidBrush, x * scaleFactor + marginLeft, y * scaleFactor + marginTop, winRect.Width * scaleFactor, winRect.Height * scaleFactor);
                }
                else if (o is Polyline)
                {
                    Polyline winPol = o as Polyline;

                    System.Windows.Media.Color c = ((SolidColorBrush)winPol.Stroke).Color;
                    XPen pen = new XPen(XColor.FromArgb(c.A, c.R, c.G, c.B), winPol.StrokeThickness * scaleFactor);

                    List<XPoint> points = new List<XPoint>();
                    foreach (Point p in winPol.Points)
                    {
                        points.Add(new XPoint(p.X * scaleFactor + marginLeft, p.Y * scaleFactor + marginTop));
                    }
                    gfx.DrawLines(pen, points.ToArray());
                }
                else if (o is Ellipse)
                {
                    Ellipse winElipse = o as Ellipse;
                    //double majorAxis = winElipse.Width;
                    //double minorAxis = winElipse.Height;

                    System.Windows.Media.Color c = ((SolidColorBrush)winElipse.Stroke).Color;
                    XPen pen = new XPen(XColor.FromArgb(c.A, c.R, c.G, c.B), winElipse.StrokeThickness);

                    double x = Canvas.GetLeft(winElipse) - winElipse.StrokeThickness / 2;
                    double y = Canvas.GetTop(winElipse) - winElipse.StrokeThickness / 2;

                    gfx.DrawEllipse(pen, x * scaleFactor + marginLeft, y * scaleFactor + marginTop, winElipse.Width * scaleFactor, winElipse.Height * scaleFactor);
                }
                else if (o is Line)
                {
                    Line winLine = o as Line;
                    
                    System.Windows.Media.Color c = ((SolidColorBrush)winLine.Stroke).Color; 
                    XPen pen = new XPen(XColor.FromArgb(c.A, c.R, c.G, c.B), winLine.StrokeThickness * scaleFactor);

                    if(winLine.StrokeDashArray.Count > 0) pen.DashStyle = XDashStyle.Dash;

                    gfx.DrawLine(pen, winLine.X1 * scaleFactor + marginLeft, winLine.Y1 * scaleFactor + marginTop, winLine.X2 * scaleFactor + marginLeft, winLine.Y2 * scaleFactor + marginTop);
                }
                else if (o is TextBlock)
                {
                    // TODO - Ondrej - rotacia textu v PDF, nasiel som toto vyjadrenie
                    /*
                    Rotation is not a property of text but of the XGraphics class.
                    Use the functions RotateTransform or RotateAtTransform.
                    More generally use the Transform property to set an arbitrary transformation matrix.
                    */

                    // TO Ondrej - Nieco som skusal vid tento riadok, ale otaca to celou skupinou objektov v gfx, neviem ci sa bude dat otocit len samotny jeden textblock
                    // gfx.RotateAtTransform(12, new XPoint(200, 200));

                    // To Ondrej - Pozri tieto odkazy:), mozno to tam najdes, ja si na to ani netrufam :)
                    // https://forum.pdfsharp.net/viewtopic.php?p=9591#p9591
                    // https://www.opten.ch/blog/2014/01/16/vertical-text-in-a-migradoc-table-cell-using-pdfsharp/
                    // https://csharp.hotexamples.com/examples/PdfSharp.Drawing/XGraphics/RotateTransform/php-xgraphics-rotatetransform-method-examples.html

                    TextBlock winText = o as TextBlock;

                    double x = Canvas.GetLeft(winText);
                    //x += winText.ActualWidth / 2;
                    double y = Canvas.GetTop(winText);
                    y += winText.FontSize;

                    System.Windows.Media.Color c = ((SolidColorBrush)winText.Foreground).Color;
                    XSolidBrush solidBrush = new XSolidBrush(XColor.FromArgb(c.A, c.R, c.G, c.B));

                    XFont f = new XFont(winText.FontFamily.ToString(), winText.FontSize * scaleFactor);

                    gfx.DrawString(winText.Text, f, solidBrush, new XPoint(x * scaleFactor + marginLeft, y * scaleFactor + marginTop));
                }
            }
        }

        private static void DrawLogo(XGraphics gfx)
        {
            XImage image = XImage.FromFile(ConfigurationManager.AppSettings["logoForPDF"]);
            gfx.DrawImage(image, 50, 750);

            XImage image2 = XImage.FromFile(ConfigurationManager.AppSettings["confStampForPDF"]);
            gfx.DrawImage(image2, 220, 750);
        }

        private static void DrawFSAddress(XGraphics gfx)
        {
            XFont font = new XFont("Verdana", 6, XFontStyle.Regular);

            string sLine1 = "Enquires to:";
            string sLine2 = "Formsteel Industries Ltd";
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

        private static void DrawProductionInfo(XGraphics gfx, string jobNumber, string customer, int amount)
        {
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            XFont font2 = new XFont("Verdana", 12, XFontStyle.Underline);

            gfx.DrawString("Job Number: ", font, XBrushes.Black, 10, 20);
            if (jobNumber != null) gfx.DrawString(jobNumber, font, XBrushes.Black, 100, 20);
            gfx.DrawString("Customer: ", font, XBrushes.Black, 10, 40);
            if(customer != null) gfx.DrawString(customer, font, XBrushes.Black, 100, 40);
            gfx.DrawString("Amount: ", font, XBrushes.Black, 10, 60);
            gfx.DrawString(amount.ToString(), font, XBrushes.Black, 100, 60);
        }

        private static void DrawProductionNotes(XGraphics gfx)
        {
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

            string sNote1 = "Screw Holes - Pre Drill to Ø = 5.7 mm";
            gfx.DrawString(sNote1, font, XBrushes.Black, 200, 700);
        }

        private static void DrawPlateInfo(XGraphics gfx, string plateNamePrefix, string plateName, decimal thickness, decimal pitch)
        {
            XFont font1 = new XFont("Verdana", 14, XFontStyle.Bold);
            XFont font2 = new XFont("Verdana", 12, XFontStyle.Regular);

            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(plateNamePrefix + " (" + plateName + ")", font1, XBrushes.Black, new XRect(0, 20, gfx.PageSize.Width, 40));

            gfx.DrawString(Math.Round(thickness, 2).ToString(), font2, XBrushes.Black, 50, 730);
            gfx.DrawString("mm Plate", font2, XBrushes.Black, 80, 730);
            gfx.DrawString(pitch.ToString(), font2, XBrushes.Black, 485, 730);
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
                string jpegSamplePath = "formsteel-screen.jpg";
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
            // You always need a MigraDoc document for rendering.
            Document doc = new Document();

            Table t = GetSimpleTable(doc, tableParams);
            //Image image = sec.AddImage()

            //Section sec = doc.AddSection();
            // Add a single paragraph with some text and format information.
            //Paragraph para = sec.AddParagraph();
            //para.Format.Alignment = ParagraphAlignment.Justify;
            //para.Format.Font.Name = "Times New Roman";
            //para.Format.Font.Size = 12;
            //para.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            //para.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            //para.AddText("Duisism odigna acipsum delesenisl ");
            //para.AddFormattedText("ullum in velenit", TextFormat.Bold);
            //para.AddText(" ipit iurero dolum zzriliquisis nit wis dolore vel et nonsequipit, velendigna " +
            //  "auguercilit lor se dipisl duismod tatem zzrit at laore magna feummod oloborting ea con vel " +
            //  "essit augiati onsequat luptat nos diatum vel ullum illummy nonsent nit ipis et nonsequis " +
            //  "niation utpat. Odolobor augait et non etueril landre min ut ulla feugiam commodo lortie ex " +
            //  "essent augait el ing eumsan hendre feugait prat augiatem amconul laoreet. ≤≥≈≠");
            //para.Format.Borders.Distance = "5pt";
            //para.Format.Borders.Color = Colors.Gold;

            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();
            
            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(40), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        public static Table DemonstrateSimpleTable(Document document)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            row.Shading.Color = MigraDoc.DocumentObjectModel.Colors.PaleGoldenrod;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph("ffassdasda");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph("dsadkja asklk daj a");

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            return table;
        }

        public static Table GetSimpleTable(Document document, List<string[]> tableParams)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.Borders.Width = 0.75;

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

        //private static void BeginBox(XGraphics gfx, int number, string title)
        //{
        //    const int dEllipse = 15;
        //    XRect rect = new XRect(0, 20, 300, 200);
        //    if (number % 2 == 0)
        //        rect.X = 300 - 5;
        //    rect.Y = 40 + ((number - 1) / 2) * (200 - 5);
        //    rect.Inflate(-10, -10);
        //    XRect rect2 = rect;
        //    rect2.Offset(this.borderWidth, this.borderWidth);
        //    gfx.DrawRoundedRectangle(new XSolidBrush(this.shadowColor), rect2, new XSize(dEllipse + 8, dEllipse + 8));
        //    XLinearGradientBrush brush = new XLinearGradientBrush(rect, this.backColor, this.backColor2, XLinearGradientMode.Vertical);
        //    gfx.DrawRoundedRectangle(this.borderPen, brush, rect, new XSize(dEllipse, dEllipse));
        //    rect.Inflate(-5, -5);

        //    XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
        //    gfx.DrawString(title, font, XBrushes.Navy, rect, XStringFormats.TopCenter);

        //    rect.Inflate(-10, -5);
        //    rect.Y += 20;
        //    rect.Height -= 20;

        //    this.state = gfx.Save();
        //    gfx.TranslateTransform(rect.X, rect.Y);
        //}

        //private void EndBox(XGraphics gfx)
        //{
        //    gfx.Restore(this.state);
        //}
    }
}
