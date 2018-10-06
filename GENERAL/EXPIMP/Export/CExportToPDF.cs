using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;

namespace EXPIMP
{
    public static class CExportToPDF
    {
        public static void ExportCanvasToPDF()
        {
            CreatePDFFile();
        }

        public static void CreatePDFFile()
        {
            // Create a temporary file
            string filename = String.Format("{0}_tempfile.pdf", Guid.NewGuid().ToString("D").ToUpper());
            PdfDocument s_document = new PdfDocument();
            //s_document.Info.Title = "Export ";
            //s_document.Info.Author = "";
            //s_document.Info.Subject = "Created with code snippets that show the use of graphical functions";
            //s_document.Info.Keywords = "PDFsharp, XGraphics";
            PdfPage page = s_document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage(gfx);

            // Create demonstration pages
            //new LinesAndCurves().DrawPage(s_document.AddPage());
            //new Shapes().DrawPage(s_document.AddPage());
            //new Paths().DrawPage(s_document.AddPage());
            //new Text().DrawPage(s_document.AddPage());
            //new Images().DrawPage(s_document.AddPage());

            // Save the s_document...
            s_document.Save(filename);
            // ...and start a viewer
            Process.Start(filename);
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
