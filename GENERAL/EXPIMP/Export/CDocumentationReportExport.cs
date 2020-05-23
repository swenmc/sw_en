using _3DTools;
using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
using MATH;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
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

using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EXPIMP
{
    public class CDocumentationReportExport
    {
        //private const string fontFamily = "Verdana";
        //private const string fontFamily = "Times New Roman";
        private const string fontFamily = "Arial";
        private const int fontSizeTitle = 14;
        private const int fontSizeNormal = 12;
        private const int fontSizeLegend = 8; // Cross-section shape legend
        private const int fontSizeNotes = 10; // Drawing - notes
        private const int fontSizeDetailTable = 8; // Details description tables text
        private static int sheetNo;

        private static XPdfFontOptions options;
        //private static PdfDocument document = null;
        private static List<string[]> contents = new List<string[]>();

        public static void ReportPDFFile(string folder, List<CPlate> plates)
        {
            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            PdfDocument s_document = new PdfDocument();

            DrawPlates(s_document, plates);

            string fileName = GetReportPDFName(folder);
            // Save the s_document...
            s_document.Save(fileName);
            s_document.Close();
            s_document.Dispose();

            // ...and start a viewer
            Process.Start(fileName);
        }

        private static void DrawPlates(PdfDocument s_document, List<CPlate> plates)
        {
            XGraphics gfx;
            PdfPage page;

            double Frame2DWidth = 579;
            double Frame2DHeight = 397;
            double scaleFor2D = 0.85;
            Canvas page2D = null;

            foreach (CPlate plate in plates)
            {
                page = s_document.AddPage();
                gfx = XGraphics.FromPdfPage(page); 
                page2D = new Canvas();
                page2D.RenderSize = new Size(Frame2DWidth, Frame2DHeight);

                //if (useTransformOptions)
                //{
                //    if (vm.MirrorX) plate.MirrorPlateAboutX();
                //    if (vm.MirrorY) plate.MirrorPlateAboutY();
                //    if (vm.Rotate90CW) plate.RotatePlateAboutZ_CW(90);
                //    if (vm.Rotate90CCW) plate.RotatePlateAboutZ_CW(-90);
                //}
                //if (vm.DrillingRoutePoints != null) plate.DrillingRoutePoints = vm.DrillingRoutePoints;

                CExportToPDF.DrawPlateInfo(gfx, plate);
                string sFileName = CExportToPDF.Draw3DScheme(gfx, null, plate); //To Mato - production info???
                CExportToPDF.DrawProductionNotes(gfx);
                //CExportToPDF.DrawLogo_Old(gfx);
                CExportToPDF.DrawLogo_New(gfx);
                CExportToPDF.DrawFSAddress(gfx);

                Drawing2D.DrawPlateToCanvas(plate,
                   Frame2DWidth,
                   Frame2DHeight,
                   ref page2D,
                   false, true, false, true, true, true, true, true, true, true); // Pozn. Nekreslime body plechu a ich cislovanie

                XImage image2 = XImage.FromBitmapSource(ExportHelper.RenderVisual(page2D, scaleFor2D));
                double y = 280;
                if (string.IsNullOrEmpty(sFileName)) y = 100;
                gfx.DrawImage(image2, 0, y, Frame2DWidth * scaleFor2D, Frame2DHeight * scaleFor2D);
                image2.Dispose();

                gfx.Dispose();
                page.Close();
            }
        }

        private static string GetReportPDFName(string folder)
        {
            int count = 0;
            string fileName = null;
            bool nameOK = false;
            while (!nameOK)
            {
                fileName = $"{folder}\\Documentation_{++count}.pdf";

                if (!System.IO.File.Exists(fileName)) nameOK = true;
            }
            return fileName;
        }
    }
}
