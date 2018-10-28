using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
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
        public static void ExportCanvasToPDF(Canvas canvas, List<string[]> tableParams)
        {
            CreatePDFFile(canvas, tableParams);
        }

        public static void CreatePDFFile(Canvas canvas, List<string[]> tableParams)
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
            DrawCanvas_PDF(canvas, gfx);
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
            double scaleFactor = gfx.PageSize.Width / canvas.RenderSize.Width;

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
                    gfx.DrawRectangle(solidBrush, x * scaleFactor, y * scaleFactor, winRect.Width * scaleFactor, winRect.Height * scaleFactor);
                }
                else if (o is Polyline)
                {
                    Polyline winPol = o as Polyline;
                                        
                    System.Windows.Media.Color c = ((SolidColorBrush)winPol.Stroke).Color;
                    XPen pen = new XPen(XColor.FromArgb(c.A, c.R, c.G, c.B), winPol.StrokeThickness * scaleFactor);
                    
                    List<XPoint> points = new List<XPoint>();
                    foreach (Point p in winPol.Points)
                    {
                        points.Add(new XPoint(p.X * scaleFactor, p.Y * scaleFactor));                        
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

                    gfx.DrawEllipse(pen, x * scaleFactor, y * scaleFactor, winElipse.Width * scaleFactor, winElipse.Height * scaleFactor);
                }
                else if (o is Line)
                {
                    Line winLine = o as Line;
                    
                    System.Windows.Media.Color c = ((SolidColorBrush)winLine.Stroke).Color;                    
                    XPen pen = new XPen(XColor.FromArgb(c.A, c.R, c.G, c.B), winLine.StrokeThickness * scaleFactor);

                    if(winLine.StrokeDashArray.Count > 0) pen.DashStyle = XDashStyle.Dash;

                    gfx.DrawLine(pen, winLine.X1 * scaleFactor, winLine.Y1 * scaleFactor, winLine.X2 * scaleFactor, winLine.Y2 * scaleFactor);
                }
                else if (o is TextBlock)
                {
                    TextBlock winText = o as TextBlock;

                    double x = Canvas.GetLeft(winText);
                    //x += winText.ActualWidth / 2;
                    double y = Canvas.GetTop(winText);                    
                    y += winText.FontSize;
                                        
                    System.Windows.Media.Color c = ((SolidColorBrush)winText.Foreground).Color;
                    XSolidBrush solidBrush = new XSolidBrush(XColor.FromArgb(c.A, c.R, c.G, c.B));

                    XFont f = new XFont(winText.FontFamily.ToString(), winText.FontSize * scaleFactor);
                    
                    gfx.DrawString(winText.Text, f, solidBrush, new XPoint(x * scaleFactor, y * scaleFactor));
                }
            }
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
