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
using System.ComponentModel;
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
    public class CMainReportExport
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

        private static XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode); // Set font encoding to unicode
        //private static PdfDocument document = null;
        private static List<string[]> contents = new List<string[]>();

        public static void ReportAllDataToPDFFile(CModelData modelData, LayoutsExportOptionsViewModel exportOpts)
        {
            sheetNo = 1;
            PdfDocument s_document = new PdfDocument();

            CProjectInfo projectInfo = modelData.ProjectInfo; // GetProjectInfo();

            s_document.Info.Title = projectInfo.ProjectName;
            s_document.Info.Author = "Formsteel Technologies";
            s_document.Info.Subject = "No " + projectInfo.ProjectNumber;
            s_document.Info.Keywords = projectInfo.ProjectNumber + ", " +
                                       "Formsteel Technologies" + ", " +
                                       "cold-formed steel" + ", " +
                                       "portal frame";

            contents = new List<string[]>();

            XGraphics TitlePage_gfx = DrawTitlePage(s_document, projectInfo, modelData, exportOpts);

            if(exportOpts.ExportModel3D)
                DrawModel3D(s_document, modelData, exportOpts);

            if (exportOpts.ExportModelViews)
                DrawModelViews(s_document, modelData, exportOpts);

            if (exportOpts.ExportModelCladdingLayingSchemeViews)
                DrawCladdingViews(s_document, modelData, exportOpts);

            if (exportOpts.ExportJointTypes)
                DrawJointTypes(s_document, modelData, exportOpts);

            if (exportOpts.ExportFootingTypes)
                DrawFootingTypes(s_document, modelData, exportOpts);

            if (exportOpts.ExportFloorDetails)
                DrawFloorDetails(s_document, modelData, exportOpts);

            if (exportOpts.ExportStandardDetails)
                DrawStandardDetails(s_document, modelData, exportOpts);

            AddTitlePageContentTableToDocument(TitlePage_gfx, contents);

            string fileName = GetReportPDFName();
            // Save the s_document...
            s_document.Save(fileName);
            s_document.Close();
            s_document.Dispose();

            // ...and start a viewer
            Process.Start(fileName);
        }


        //toto boli pokusy asi treba zmazat
        //static CModelData _modelData = null;
        //static string _fileName = null;
        //public static void ReportAllDataToPDFFiles_New(CModelData modelData)
        //{
        //    _modelData = modelData;

        //    sheetNo = 1;
        //    // Set font encoding to unicode
        //    XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
        //    string fileName = GetReportPDFName();
        //    _fileName = fileName;

        //    PdfDocument s_document = new PdfDocument();

        //    CProjectInfo projectInfo = modelData.ProjectInfo; // GetProjectInfo();

        //    s_document.Info.Title = projectInfo.ProjectName;
        //    s_document.Info.Author = "Formsteel Technologies";
        //    s_document.Info.Subject = "No " + projectInfo.ProjectNumber;
        //    s_document.Info.Keywords = projectInfo.ProjectNumber + ", " +
        //                               "Formsteel Technologies" + ", " +
        //                               "cold-formed steel" + ", " +
        //                               "portal frame";

        //    contents = new List<string[]>();

        //    DateTime start = DateTime.Now;
        //    System.Diagnostics.Trace.WriteLine("Beginning: " + (DateTime.Now - start).TotalMilliseconds);
        //    XGraphics TitlePage_gfx = DrawTitlePage(s_document, projectInfo, modelData);
        //    System.Diagnostics.Trace.WriteLine("After DrawTitlePage: " + (DateTime.Now - start).TotalMilliseconds);

        //    PdfDocument s_document2 = new PdfDocument();
        //    DrawModel3D(s_document2, modelData);
        //    s_document2.Save($"EXP2_{fileName}");
        //    s_document2.Close();
        //    s_document2.Dispose();
        //    System.Diagnostics.Trace.WriteLine("After DrawModel3D: " + (DateTime.Now - start).TotalMilliseconds);


        //    PdfDocument s_document3 = new PdfDocument();
        //    DrawModelViews(s_document3, modelData);
        //    s_document3.Save($"EXP3_{fileName}");
        //    s_document3.Close();
        //    s_document3.Dispose();
        //    System.Diagnostics.Trace.WriteLine("After DrawModelViews: " + (DateTime.Now - start).TotalMilliseconds);

        //    PdfDocument s_document4 = new PdfDocument();
        //    DrawJointTypes(s_document4, modelData);
        //    s_document4.Save($"EXP4_{fileName}");
        //    s_document4.Close();
        //    s_document4.Dispose();
        //    System.Diagnostics.Trace.WriteLine("After DrawJointTypes: " + (DateTime.Now - start).TotalMilliseconds);

        //    PdfDocument s_document5 = new PdfDocument();
        //    DrawFootingTypes(s_document5, modelData);
        //    s_document5.Save($"EXP5_{fileName}");
        //    s_document5.Close();
        //    s_document5.Dispose();
        //    System.Diagnostics.Trace.WriteLine("After DrawFootingTypes: " + (DateTime.Now - start).TotalMilliseconds);

        //    PdfDocument s_document6 = new PdfDocument();
        //    DrawFloorDetails(s_document6, modelData);
        //    s_document6.Save($"EXP6_{fileName}");
        //    s_document6.Close();
        //    s_document6.Dispose();
        //    System.Diagnostics.Trace.WriteLine("After DrawFloorDetails: " + (DateTime.Now - start).TotalMilliseconds);

        //    PdfDocument s_document7 = new PdfDocument();
        //    DrawStandardDetails(s_document7, modelData);
        //    s_document7.Save($"EXP7_{fileName}");
        //    s_document7.Close();
        //    s_document7.Dispose();
        //    System.Diagnostics.Trace.WriteLine("After DrawStandardDetails: " + (DateTime.Now - start).TotalMilliseconds);

        //    AddTitlePageContentTableToDocument(TitlePage_gfx, contents);
        //    System.Diagnostics.Trace.WriteLine("After AddTitlePageContentTableToDocument: " + (DateTime.Now - start).TotalMilliseconds);

        //    // Save the s_document...
        //    s_document.Save($"EXP1_{fileName}");
        //    s_document.Close();
        //    s_document.Dispose();

        //    // ...and start a viewer
        //    //Process.Start(fileName);
        //}

        //public static void Export3DModel(CModelData modelData, Trackport3D trackport)
        //{
        //    PdfDocument s_document2 = new PdfDocument();
        //    DrawModel3D_Async(s_document2, modelData, trackport);
        //    s_document2.Save($"EXP2_{_fileName}");
        //    s_document2.Close();
        //    s_document2.Dispose();
        //}



        private static PageSize GetPageSize(EPageSizes pageSize)
        {
            switch (pageSize)
            {
                case EPageSizes.A0: return PageSize.A0;
                case EPageSizes.A1: return PageSize.A1;
                case EPageSizes.A2: return PageSize.A2;
                case EPageSizes.A3: return PageSize.A3;
                case EPageSizes.A4: return PageSize.A4;
                default:
                    return PageSize.A3;
            }
        }
        private static int GetCanvasSizeFactorAcordingToPageSize(EPageSizes pageSize)
        {
            int factor = 1;
            switch (pageSize)
            {
                case EPageSizes.A0: return 4;
                case EPageSizes.A1: return 3;
                case EPageSizes.A2: return 2;
                case EPageSizes.A3: return factor;
                case EPageSizes.A4: return factor;
                default:
                    return factor;
            }

        }
        private static double GetCanvasWidthAcordingToPageSize(EPageSizes pageSize, EImagesQuality quality)
        {
            int width = 1191;
            switch (pageSize)
            {
                case EPageSizes.A0: width = 3370; break;
                case EPageSizes.A1: width = 2384; break;
                case EPageSizes.A2: width = 1684; break;
            }

            double factor = 1;
            switch (quality)
            {
                case EImagesQuality.Low: factor = 0.5; break;
                case EImagesQuality.Hight: factor = 2; break;
                case EImagesQuality.Best: factor = 4; break;
                default: factor = 1; break;
            }

            return width * factor;
        }
        private static double GetCanvasHeightAcordingToPageSize(EPageSizes pageSize, EImagesQuality quality)
        {
            int height = 842;
            switch (pageSize)
            {
                case EPageSizes.A0: height = 2384; break;
                case EPageSizes.A1: height = 1684; break;
                case EPageSizes.A2: height = 1191; break;
            }

            double factor = 1;
            switch (quality)
            {
                case EImagesQuality.Low: factor = 0.5; break;
                case EImagesQuality.Hight: factor = 2; break;
                case EImagesQuality.Best: factor = 4; break;
                default: factor = 1; break;
            }

            return height * factor;
        }

        private static PageOrientation GetPageOrientation(EPageOrientation pageOrientation)
        {
            switch (pageOrientation)
            {
                case EPageOrientation.Portrait: return PageOrientation.Portrait;
                default:
                    return PageOrientation.Landscape;
            }
        }

        /// <summary>
        /// Draw scaled 3Model to PDF
        /// </summary>        
        private static void DrawModel3D(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            // TO Ondrej - pre export 3D sceny implementovat samostatne display options podobne ako to mame pre pohlady ModelViews
            XGraphics gfx;
            PdfPage page;
            page = s_document.AddPage();
            page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSize);
            page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientation);
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            DisplayOptions opts = ExportHelper.GetDisplayOptionsForMainModelExport(data);

            CModel filteredModel = null;
            Trackport3D trackport = null;

            double width = GetCanvasWidthAcordingToPageSize((EPageSizes)exportOpts.ExportPageSize, opts.ExportImagesQuality);
            double height = GetCanvasHeightAcordingToPageSize((EPageSizes)exportOpts.ExportPageSize, opts.ExportImagesQuality);

            Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data, 1f, out filteredModel, out trackport, width, height);
            viewPort.UpdateLayout();

            XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
            gfx.DrawString("Model in 3D environment: ", fontBold, XBrushes.Black, 20, 20);

            DrawTitleBlock(gfx, data.ProjectInfo, page, EPDFPageContentType.Isometric_View.GetFriendlyName(), sheetNo, 0);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Isometric_View.GetFriendlyName() });

            int legendImgWidth = 100;
            int legendTextWidth = 80;
            DrawCrscLegendTable(gfx, filteredModel, (int)page.Width.Point, legendTextWidth, legendImgWidth);

            XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort));

            double scaleFactor = (gfx.PageSize.Width - legendImgWidth - legendTextWidth) / image.PointWidth;
            double scaledImageWidth = gfx.PageSize.Width - legendImgWidth - legendTextWidth;
            double scaledImageHeight = image.PointHeight * scaleFactor;

            gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
            image.Dispose();
            viewPort.Dispose();
            trackport.Dispose();
            gfx.Dispose();
            page.Close();
        }

        //private static void DrawModel3D_Async(PdfDocument s_document, CModelData data, Trackport3D trackport, LayoutsExportOptionsViewModel exportOpts)
        //{
        //    // TO Ondrej - pre export 3D sceny implementovat samostatne display options podobne ako to mame pre pohlady ModelViews
        //    XGraphics gfx;
        //    PdfPage page;
        //    page = s_document.AddPage();
        //    //page.Size = PageSize.A3;
        //    //page.Orientation = PdfSharp.PageOrientation.Landscape;
        //    page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSize);
        //    page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientation);
        //    gfx = XGraphics.FromPdfPage(page);

        //    DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
        //    DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

        //    DisplayOptions opts = ExportHelper.GetDisplayOptionsForMainModelExport(data);

        //    CModel filteredModel = null;            

        //    Viewport3D viewPort = ExportHelper.GetBaseModelViewPortAsync(opts, data, 1f, trackport, out filteredModel);
        //    viewPort.UpdateLayout();

        //    XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
        //    gfx.DrawString("Model in 3D environment: ", fontBold, XBrushes.Black, 20, 20);

        //    DrawTitleBlock(gfx, data.ProjectInfo, EPDFPageContentType.Isometric_View.GetFriendlyName(), sheetNo, 0);
        //    contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Isometric_View.GetFriendlyName() });

        //    int legendImgWidth = 100;
        //    int legendTextWidth = 80;
        //    DrawCrscLegendTable(gfx, filteredModel, (int)page.Width.Point, legendTextWidth, legendImgWidth);

        //    XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort));

        //    double scaleFactor = (gfx.PageSize.Width - legendImgWidth - legendTextWidth) / image.PointWidth;
        //    double scaledImageWidth = gfx.PageSize.Width - legendImgWidth - legendTextWidth;
        //    double scaledImageHeight = image.PointHeight * scaleFactor;

        //    gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
        //    image.Dispose();
        //    viewPort.Dispose();
        //    trackport.Dispose();
        //    gfx.Dispose();
        //    page.Close();
        //}

        private static void DrawModelViews(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            XGraphics gfx;
            PdfPage page;
            double scale = 1;
            DisplayOptions opts = GetModelViewsDisplayOptions(data);
            opts.ViewsPageSize = (EPageSizes) exportOpts.ExportPageSizeViews;
            opts.ExportImagesQuality = (EImagesQuality)exportOpts.ExportImagesQuality;
            opts.IsExport = true;

            opts.SameScaleForViews = true;

            List<EViewModelMemberFilters> list_views = GetModelViewsFromExportOptions(exportOpts);
            
            int legendImgWidth = 100;
            int legendTextWidth = 70;
            //float modelMaxLength = ModelHelper.GetModelMaxLength(data.Model, data.DisplayOptions);  //toto nic nerobi, tak som zakomentoval

            DateTime start = DateTime.Now;
            System.Diagnostics.Trace.WriteLine("DrawModelViews Beginning: " + (DateTime.Now - start).TotalMilliseconds);

            foreach (EViewModelMemberFilters viewMembers in list_views)
            {
                sheetNo++;
                Trace.WriteLine(sheetNo + ". " + viewMembers.ToString());
                page = s_document.AddPage();
                //page.Size = PageSize.A3;
                //page.Orientation = PdfSharp.PageOrientation.Landscape;
                page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSizeViews);
                page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientationViews);

                gfx = XGraphics.FromPdfPage(page);
                DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
                DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

                DrawTitleBlock(gfx, data.ProjectInfo, page,((EPDFPageContentType)viewMembers).GetFriendlyName(), sheetNo, 0);
                contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", ((EPDFPageContentType)viewMembers).GetFriendlyName() });

                opts.ModelView = GetView(viewMembers);
                opts.ViewModelMembers = (int)viewMembers;

                // Defaultne hodnoty pre vsetky pohlady
                opts.bTransformScreenLines3DToCylinders3D = false;  // Do not convert lines (v PDF sa teda nezobrazia)
                opts.wireFrameColor = System.Windows.Media.Colors.Black; // Nastavenie farby wireframe pre export (ina farba ako je v 3D scene)
                // Mozeme nastavit pre ktory view chceme kreslit wireframe a konvertovat ciary, farbu a hrubku ciary

                // TO Ondrej - tu je trosku problem ze mame jedny 
                // DisplayOptions opts pre vsetky pohlady a podorysy (mame to nazvane a pouzivame to ako filters), takze ked nieco zobrazim v jednom pohlade a nechcem to v inych ,tak to musim vsade inde povypinat
                // To moze byt dost komplikovane na spravu, mozno by bolo lepsie mat pre export - pre kazdy pohlad/podorys/elevation/floor plan/layout samostatne display options
                // a zapnut v nich na true len to co chcem vidiet v danom pohlade nezavisle na ostatnych pohladoch
                /*
                All = 0,
                FRONT = 1,
                BACK = 2,
                LEFT = 3,
                RIGHT = 4,
                ROOF = 5,
                MIDDLE_FRAME = 6,
                COLUMNS = 7,
                FOUNDATIONS = 8,
                FLOOR = 9
                */

                opts.bCreateHorizontalGridlines = true;
                opts.bCreateVerticalGridlinesFront = false;
                opts.bCreateVerticalGridlinesBack = false;
                opts.bCreateVerticalGridlinesLeft = false;
                opts.bCreateVerticalGridlinesRight = false;

                /*
                // Bug 477 - Refactoring
                float fWireFrameLineThickness_Basic = 2f; // Default value same as in GUI - zakladna hrubka ciar wireframe, ktoru chceme na vykresoch
                float fWireFrameLineThickness_Factor = 1.05f; //  Faktor ktory zohladnuje vztah medzi hodnotou basic v "bodoch" a model size factor pre velkost modelu v metroch
                float fWireFrameLineThickness_ModelSize_Factor = modelMaxLength / 1000.0f;
                float fZoomFactor = 1f;

                float fWireFrameLineThickness_Final = fWireFrameLineThickness_Basic * fWireFrameLineThickness_Factor * fWireFrameLineThickness_ModelSize_Factor * fZoomFactor;
                */

                ChangeDisplayOptionsAcordingToView(viewMembers, ref opts);

                if (viewMembers == EViewModelMemberFilters.FOUNDATIONS)
                {
                    // Table - footing pads list
                    DrawFootingPadList(gfx, data, (int)page.Width.Point - 275, (int)page.Height.Point - 250);
                }

                if (viewMembers == EViewModelMemberFilters.FLOOR)
                {  
                    // Notes - floor
                    DrawNotes_Floor(gfx, data, (int)page.Width.Point - 275, (int)page.Height.Point - 250);
                }

                CModel filteredModel = null;
                Trackport3D trackport = null;
                System.Diagnostics.Trace.WriteLine("DrawModelViews before GetBaseModelViewPort: " + (DateTime.Now - start).TotalMilliseconds);

                //int factor = GetCanvasSizeFactorAcordingToPageSize((EPageSizes)exportOpts.ExportPageSizeViews);
                double width = GetCanvasWidthAcordingToPageSize((EPageSizes)exportOpts.ExportPageSizeViews, opts.ExportImagesQuality);
                double height = GetCanvasHeightAcordingToPageSize((EPageSizes)exportOpts.ExportPageSizeViews, opts.ExportImagesQuality);

                //Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data, 1f, out filteredModel, out trackport, 1400 * factor, 1000 * factor);
                Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data, 1f, out filteredModel, out trackport, width, height);
                System.Windows.Media.RenderOptions.SetEdgeMode((DependencyObject)viewPort, System.Windows.Media.EdgeMode.Aliased);
                viewPort.UpdateLayout();
                System.Diagnostics.Trace.WriteLine("DrawModelViews after GetBaseModelViewPort: " + (DateTime.Now - start).TotalMilliseconds);
                DrawCrscLegendTable(gfx, filteredModel, (int)page.Width.Point, legendTextWidth);
                filteredModel = null;
                //System.Diagnostics.Trace.WriteLine("DrawModelViews after DrawCrscLegendTable: " + (DateTime.Now - start).TotalMilliseconds);

                XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
                gfx.DrawString($"{(viewMembers).ToString()}:", fontBold, XBrushes.Black, 20, 20);

                System.Diagnostics.Trace.WriteLine("DrawModelViews before RenderVisual: " + (DateTime.Now - start).TotalMilliseconds);

                //XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort, scale));
                BitmapSource bs = ExportHelper.RenderVisual(viewPort);
                XImage image = XImage.FromBitmapSource(bs);
                bs = null;
                System.Diagnostics.Trace.WriteLine("DrawModelViews after RenderVisual: " + (DateTime.Now - start).TotalMilliseconds);

                double scaleFactor = (gfx.PageSize.Width - legendImgWidth - legendTextWidth) / image.PointWidth;
                double scaledImageWidth = gfx.PageSize.Width - legendImgWidth - legendTextWidth;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
                image.Dispose();
                viewPort.Dispose();
                trackport.Dispose();
                gfx.Dispose();
                page.Close();
            }
        }

        private static void DrawCladdingViews(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            XGraphics gfx;
            PdfPage page;
            DisplayOptions opts = GetModelViewsDisplayOptions(data);
            opts.ViewsPageSize = (EPageSizes)exportOpts.ExportPageSizeViewsCladding;
            opts.ExportImagesQuality = (EImagesQuality)exportOpts.ExportImagesQuality;
            opts.IsExport = true;
            opts.SameScaleForViews = true;

            List<EViewCladdingFilters> list_views = GetCladdingViewsFromExportOptions(exportOpts);

            int legendImgWidth = 100;
            int legendTextWidth = 70;

            //DateTime start = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("DrawCladdingViews Beginning: " + (DateTime.Now - start).TotalMilliseconds);

            foreach (EViewCladdingFilters view in list_views)
            {
                sheetNo++;
                Trace.WriteLine(sheetNo + ". " + view.ToString());
                page = s_document.AddPage();
                page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSizeViewsCladding);
                page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientationViewsCladding);

                gfx = XGraphics.FromPdfPage(page);
                DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
                DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

                //to pretypovanie na EPDFPageContentType sa mi vobec nepaci, to je nejake divne
                DrawTitleBlock(gfx, data.ProjectInfo, page, ((EPDFPageContentType)view).GetFriendlyName(), sheetNo, 0);
                contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", ((EPDFPageContentType)view).GetFriendlyName() });

                opts.ModelView = GetView(view);
                opts.ViewModelMembers = -1;
                opts.ViewCladding = (int)view;

                ChangeDisplayOptionsAcordingToView(view, ref opts);

                CModel filteredModel = null;
                Trackport3D trackport = null;
                double width = GetCanvasWidthAcordingToPageSize((EPageSizes)exportOpts.ExportPageSizeViewsCladding, opts.ExportImagesQuality);
                double height = GetCanvasHeightAcordingToPageSize((EPageSizes)exportOpts.ExportPageSizeViewsCladding, opts.ExportImagesQuality);

                Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data, 1f, out filteredModel, out trackport, width, height);
                System.Windows.Media.RenderOptions.SetEdgeMode((DependencyObject)viewPort, System.Windows.Media.EdgeMode.Aliased);
                viewPort.UpdateLayout();
                
                filteredModel = null;
                //System.Diagnostics.Trace.WriteLine("DrawCladdingViews after DrawCrscLegendTable: " + (DateTime.Now - start).TotalMilliseconds);

                XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
                gfx.DrawString($"{(view).ToString()}:", fontBold, XBrushes.Black, 20, 20);

                //System.Diagnostics.Trace.WriteLine("DrawCladdingViews before RenderVisual: " + (DateTime.Now - start).TotalMilliseconds);

                BitmapSource bs = ExportHelper.RenderVisual(viewPort);
                XImage image = XImage.FromBitmapSource(bs);
                bs = null;
                //System.Diagnostics.Trace.WriteLine("DrawCladdingViews after RenderVisual: " + (DateTime.Now - start).TotalMilliseconds);

                double scaleFactor = (gfx.PageSize.Width - legendImgWidth - legendTextWidth) / image.PointWidth;
                double scaledImageWidth = gfx.PageSize.Width - legendImgWidth - legendTextWidth;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
                image.Dispose();
                viewPort.Dispose();
                trackport.Dispose();
                gfx.Dispose();
                page.Close();
            }
        }

        private static List<EViewModelMemberFilters> GetModelViewsFromExportOptions(LayoutsExportOptionsViewModel exportOpts)
        {
            List<EViewModelMemberFilters> list_views = new List<EViewModelMemberFilters>();
            if (exportOpts.ExportModelViewsFront) list_views.Add(EViewModelMemberFilters.FRONT);
            if (exportOpts.ExportModelViewsBack) list_views.Add(EViewModelMemberFilters.BACK);
            if (exportOpts.ExportModelViewsLeft) list_views.Add(EViewModelMemberFilters.LEFT);
            if (exportOpts.ExportModelViewsRight) list_views.Add(EViewModelMemberFilters.RIGHT);
            if (exportOpts.ExportModelViewsRoof) list_views.Add(EViewModelMemberFilters.ROOF);
            if (exportOpts.ExportModelViewsMiddleFrame) list_views.Add(EViewModelMemberFilters.MIDDLE_FRAME);
            if (exportOpts.ExportModelViewsColumns) list_views.Add(EViewModelMemberFilters.COLUMNS);
            if (exportOpts.ExportModelViewsFoundations) list_views.Add(EViewModelMemberFilters.FOUNDATIONS);
            if (exportOpts.ExportModelViewsFloor) list_views.Add(EViewModelMemberFilters.FLOOR);

            return list_views;
        }
        private static List<EViewCladdingFilters> GetCladdingViewsFromExportOptions(LayoutsExportOptionsViewModel exportOpts)
        {
            List<EViewCladdingFilters> list_views = new List<EViewCladdingFilters>();

            if (exportOpts.ExportModelCladdingLayingSchemeViewsFront) list_views.Add(EViewCladdingFilters.CLADDING_FRONT);
            if (exportOpts.ExportModelCladdingLayingSchemeViewsBack) list_views.Add(EViewCladdingFilters.CLADDING_BACK);
            if (exportOpts.ExportModelCladdingLayingSchemeViewsLeft) list_views.Add(EViewCladdingFilters.CLADDING_LEFT);
            if (exportOpts.ExportModelCladdingLayingSchemeViewsRight) list_views.Add(EViewCladdingFilters.CLADDING_RIGHT);
            if (exportOpts.ExportModelCladdingLayingSchemeViewsRoof) list_views.Add(EViewCladdingFilters.CLADDING_ROOF);
            return list_views;
        }

        private static DisplayOptions GetModelViewsDisplayOptions(CModelData data)
        {
            DisplayOptions opts = data.DisplayOptions;
            opts.bUseOrtographicCamera = true;
            opts.bColorsAccordingToMembers = false;
            opts.bColorsAccordingToSections = true;
            opts.bDisplayGlobalAxis = false;
            opts.bDisplaySolidModel = true;
            opts.bDisplayMembersCenterLines = false;
            opts.bDisplayWireFrameModel = false;   //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            opts.bTransformScreenLines3DToCylinders3D = true;

            opts.bDisplayMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta

            opts.bDisplayNodes = false;
            opts.bDisplayNodesDescription = false;
            opts.bDisplayNodalSupports = false;

            opts.bDisplayMembers = true;
            //opts.bDisplayJoints = true;
            //opts.bDisplayPlates = true;

            opts.bDisplaySawCuts = false;
            opts.bDisplayControlJoints = false;
            opts.bDisplayGridlines = false;
            opts.bDisplaySectionSymbols = false;
            opts.bDisplayDetailSymbols = false;

            opts.bDisplayCladding = false;

            // TO Ondrej - Tu by to chcelo vymysliet nejaky mechanizmus, ktory na zaklade rozmerov vykresu a velkosti obrazku modelu urci aka ma byt vyska textu v jednotlivych pohladoch, na papieri by to malo byt cca - 2-2.5 mm, pripadne do 3 mm (6 - 8 PT)
            // Vysku textu mozeme nastavovat ako velkost fontu ale pre export do 2D je lepsie uzivatelsky nastavovat velkost v mm lebo stavbari nevedia aky velky je font c. 8, pripadne tam bude prepocet z bodov na mm

            /*
            07.0 PT     09 PX     2.5 MM     0.60 EM     060 %
            07.0 PT     10 PX     2.5 MM     0.60 EM     060 %
            08.0 PT     11 PX     2.8 MM     0.70 EM     070 %
            09.0 PT     12 PX     3.4 MM     0.80 EM     080 %
            09.0 PT     13 PX     3.4 MM     0.80 EM     080 %
            10.0 PT     13 PX     3.4 MM     0.80 EM     080 %
            10.5 PT     14 PX     3.6 MM     0.85 EM     085 %
            11.0 PT     15 PX     3.9 MM     0.95 EM     095 %
            12.0 PT     16 PX     4.2 MM     1.05 EM     105 %
            12.0 PT     17 PX     4.2 MM     1.05 EM     105 %
            13.0 PT     17 PX     4.2 MM     1.10 EM     110 %
            13.0 PT     18 PX     4.8 MM     1.10 EM     110 %
            14.0 PT     19 PX     5.0 MM     1.20 EM     120 %
            15.0 PT     20 PX     5.4 MM     1.33 EM     133 %
            16.0 PT     21 PX     5.8 MM     1.40 EM     140 %
            16.0 PT     22 PX     5.8 MM     1.40 EM     140 %
            17.0 PT     23 PX     6.2 MM     1.50 EM     150 %
            */

            opts.fMemberDescriptionTextFontSize = 14; // Font 14 znamena 0.14 m v 3D grafike, takze hodnota / 100f
            opts.MemberDescriptionTextColor = System.Windows.Media.Colors.DarkGreen;

            opts.fDimensionTextFontSize = 14;
            opts.DimensionTextColor = System.Windows.Media.Colors.DarkBlue;
            opts.DimensionLineColor = System.Windows.Media.Colors.Black;

            opts.fGridLineLabelTextFontSize = 30;
            opts.GridLineLabelTextColor = System.Windows.Media.Colors.Black;
            opts.GridLineColor = System.Windows.Media.Colors.Black;
            opts.GridLinePatternType = ELinePatternType.DASHDOTTED;

            opts.fSawCutTextFontSize = 14;
            opts.SawCutTextColor = System.Windows.Media.Colors.Black;
            opts.SawCutLineColor = System.Windows.Media.Colors.Black;
            opts.SawCutLinePatternType = ELinePatternType.DOTTED;

            opts.fControlJointTextFontSize = 14;
            opts.ControlJointTextColor = System.Windows.Media.Colors.Black;
            opts.ControlJointLineColor = System.Windows.Media.Colors.Black;
            opts.ControlJointLinePatternType = ELinePatternType.DIVIDE;

            opts.fSectionSymbolLabelTextFontSize = 30;
            opts.SectionSymbolLabelTextColor = System.Windows.Media.Colors.Black;
            opts.SectionSymbolColor = System.Windows.Media.Colors.Black;
            //opts.SectionSymbolLinePatternType = ELinePatternType.DASHDOTTED;

            opts.fDetailSymbolLabelTextFontSize = 30;
            opts.DetailSymbolLabelTextColor = System.Windows.Media.Colors.Black;
            opts.DetailSymbolLabelBackColor = System.Windows.Media.Colors.White;
            opts.DetailSymbolColor = System.Windows.Media.Colors.Black;
            //opts.DetailSymbolLinePatternType = ELinePatternType.CONTINUOUS;

            opts.fFoundationTextFontSize = 14;
            opts.FoundationColor = System.Windows.Media.Colors.White;
            opts.FoundationTextColor = System.Windows.Media.Colors.Black;

            opts.fFloorSlabTextFontSize = 14;
            opts.FloorSlabColor = System.Windows.Media.Colors.White;
            opts.FloorSlabTextColor = System.Windows.Media.Colors.Black;

            opts.SlabRebateColor = System.Windows.Media.Colors.White;

            opts.ReinforcementBarColor_Top_x = System.Windows.Media.Colors.Black;
            opts.ReinforcementBarColor_Top_y = System.Windows.Media.Colors.Black;
            opts.ReinforcementBarColor_Bottom_x = System.Windows.Media.Colors.Black;
            opts.ReinforcementBarColor_Bottom_y = System.Windows.Media.Colors.Black;

            //opts.PlateColor = System.Windows.Media.Colors.Gray;
            //opts.ScrewColor = System.Windows.Media.Colors.Black;
            //opts.AnchorColor = System.Windows.Media.Colors.Black;
            //opts.WasherColor = System.Windows.Media.Colors.Gray;
            //opts.NutColor = System.Windows.Media.Colors.Black;

            opts.fFoundationSolidModelOpacity = 0;
            opts.fFloorSlabSolidModelOpacity = 0;
            opts.fSlabRebateSolidModelOpacity = 0;
            opts.fReinforcementBarSolidModelOpacity = 1;

            return opts;
        }

        private static void ChangeDisplayOptionsAcordingToView(EViewModelMemberFilters viewMembers, ref DisplayOptions opts)
        {
            if (viewMembers == EViewModelMemberFilters.FRONT)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;
                opts.bDisplayMemberRealLengthInMM = true;

                // opts.bDisplayJoints = true; // Ak chceme zobrazovat znacky detailov, musime do filtrovaneho modelu exportovat aj spoje, bude to zavisiet na tom ci je zapnute ich zobrazenie, alebo to budeme robit vzdy
                opts.bDisplayGridlines = true; // Vertical
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = true;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = false;
                opts.bCreateVerticalGridlinesFront = true;
            }

            if (viewMembers == EViewModelMemberFilters.BACK)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;
                opts.bDisplayMemberRealLengthInMM = true;

                opts.bDisplayGridlines = true; // Vertical
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = true;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = false;
                opts.bCreateVerticalGridlinesBack = true;
            }

            if (viewMembers == EViewModelMemberFilters.LEFT)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;
                opts.bDisplayMemberRealLengthInMM = true;

                opts.bDisplayGridlines = true;// Vertical
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = true;
                opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = false;
                opts.bCreateVerticalGridlinesLeft = true;
            }

            if (viewMembers == EViewModelMemberFilters.RIGHT)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;
                opts.bDisplayMemberRealLengthInMM = true;

                opts.bDisplayGridlines = true; // Vertical
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = true;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = false;
                opts.bCreateVerticalGridlinesRight = true;
            }

            if (viewMembers == EViewModelMemberFilters.ROOF)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;
                opts.bDisplayMemberRealLengthInMM = true;

                opts.bDisplayGridlines = true; // Horizontal
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = true;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = true;
            }

            if (viewMembers == EViewModelMemberFilters.MIDDLE_FRAME)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;
                opts.bDisplayMemberRealLengthInMM = true;

                // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                //opts.bDisplayWireFrameModel = true;
                //opts.bDisplayMembersWireFrame = true;
                //opts.bTransformScreenLines3DToCylinders3D = true;

                opts.bDisplayGridlines = true; // Vertical
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = false;
                //opts.bDisplayDimensions = true;

                opts.bCreateVerticalGridlinesFront = true;
            }

            if (viewMembers == EViewModelMemberFilters.COLUMNS)
            {
                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberPrefix = true;

                // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                opts.bDisplayWireFrameModel = true;
                opts.bDisplayFloorSlabWireFrame = true;
                opts.bDisplayMembersWireFrame = true;
                opts.bTransformScreenLines3DToCylinders3D = true;
                //opts.fWireFrameLineThickness = fWireFrameLineThickness_Final;

                opts.bDisplayFoundations = false;
                opts.bDisplayReinforcementBars = false;
                opts.bDisplayFloorSlab = true;
                opts.bDisplayFloorSlabDescription = false;
                opts.bDisplayGridlines = true; // Horizontal
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = false;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = true;
            }

            if (viewMembers == EViewModelMemberFilters.FOUNDATIONS)
            {
                // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                opts.bDisplayWireFrameModel = true;
                opts.bDisplayFoundationsWireFrame = true;
                opts.bDisplayFloorSlabWireFrame = true;
                opts.bTransformScreenLines3DToCylinders3D = true;
                //opts.fWireFrameLineThickness = fWireFrameLineThickness_Final;

                opts.bDisplayFoundations = true;
                opts.bDisplayReinforcementBars = true;
                opts.bDisplayFloorSlab = true;
                opts.bDisplayFloorSlabDescription = false;
                opts.bDisplayFoundationsDescription = true;
                opts.bDisplayMemberDescription = false;
                opts.bDisplayGridlines = true; // Horizontal
                opts.bDisplaySectionSymbols = false;
                opts.bDisplayDetailSymbols = false;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = true;
            }

            if (viewMembers == EViewModelMemberFilters.FLOOR)
            {
                // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                opts.bDisplayWireFrameModel = true;
                opts.bDisplayFoundationsWireFrame = true;
                opts.bDisplayFloorSlabWireFrame = true;
                opts.bTransformScreenLines3DToCylinders3D = true;
                //opts.fWireFrameLineThickness = fWireFrameLineThickness_Final;

                opts.bDisplayFoundations = true;
                opts.bDisplayReinforcementBars = false;
                opts.bDisplayFloorSlab = true;
                opts.bDisplayFloorSlabDescription = true;
                opts.bDisplayFoundationsDescription = false;
                opts.bDisplayMemberDescription = false;

                opts.bDisplaySawCuts = true;
                opts.bDisplaySawCutsDescription = true;
                opts.bDisplayControlJoints = true;
                opts.bDisplayControlJointsDescription = true;
                opts.bDisplayGridlines = true; // Horizontal
                opts.bDisplaySectionSymbols = true;
                opts.bDisplayDetailSymbols = false;
                //opts.bDisplayDimensions = true;

                opts.bCreateHorizontalGridlines = true;
            }
        }

        private static void ChangeDisplayOptionsAcordingToView(EViewCladdingFilters viewMembers, ref DisplayOptions opts)
        {
            //spolocne Display Options
            if (viewMembers == EViewCladdingFilters.CLADDING_FRONT ||
                viewMembers == EViewCladdingFilters.CLADDING_BACK ||
                viewMembers == EViewCladdingFilters.CLADDING_LEFT ||
                viewMembers == EViewCladdingFilters.CLADDING_RIGHT ||
                viewMembers == EViewCladdingFilters.CLADDING_ROOF)
            {
                // Defaultne hodnoty pre vsetky pohlady
                opts.bTransformScreenLines3DToCylinders3D = true;
                opts.wireFrameColor = System.Windows.Media.Colors.Red;

                opts.bDisplayMembers = false;
                opts.bDisplayJoints = false;
                opts.bDisplayFoundations = false;
                opts.bDisplayFloorSlab = false;

                opts.bDisplayLocalMembersAxis = false;
                opts.bDisplayDimensions = false;

                opts.bDisplayCladding = true;
                opts.bDisplayCladdingLeftWall = true;
                opts.bDisplayCladdingRightWall = true;
                opts.bDisplayCladdingFrontWall = true;
                opts.bDisplayCladdingBackWall = true;
                opts.bDisplayCladdingRoof = true;

                opts.bDisplayFibreglass = true;
                opts.bDisplayDoors = true;
                opts.bDisplayWindows = true;

                opts.bDisplayWireFrameModel = true;
                opts.bDisplayCladdingWireFrame = true;
                opts.bDisplayFibreglassWireFrame = true;
                opts.bDisplayDoorsWireFrame = true;
                opts.bDisplayWindowsWireFrame = true;

                opts.bDisplayCladdingDescription = true;
                opts.bDisplayCladdingID = true;
                opts.bDisplayCladdingPrefix = true;
                opts.bDisplayCladdingLengthWidth = true;

                opts.bDisplayFibreglassDescription = true;
                opts.bDisplayFibreglassID = true;
                opts.bDisplayFibreglassPrefix = true;
                opts.bDisplayFibreglassLengthWidth = true;

                opts.bDisplayDoorDescription = true;
                opts.bDisplayDoorID = true;
                opts.bDisplayDoorType = true;
                opts.bDisplayDoorHeightWidth = true;

                opts.bDisplayWindowDescription = true;
                opts.bDisplayWindowID = true;
                opts.bDisplayWindowHeightWidth = true;
            }
            
            if (viewMembers == EViewCladdingFilters.CLADDING_FRONT)
            {
                opts.bDisplayCladdingFrontWall = true;
            }
            else if (viewMembers == EViewCladdingFilters.CLADDING_BACK)
            { 
                opts.bDisplayCladdingBackWall = true;
            }
            else if (viewMembers == EViewCladdingFilters.CLADDING_LEFT)
            {
                opts.bDisplayCladdingLeftWall = true;
            }
            else if (viewMembers == EViewCladdingFilters.CLADDING_RIGHT)
            {
                opts.bDisplayCladdingRightWall = true;
            }
            else if (viewMembers == EViewCladdingFilters.CLADDING_ROOF)
            {
                opts.bDisplayCladdingRoof = true;
            }
        }

        private static void DrawJointTypes(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            XGraphics gfx;
            PdfPage page;
            double scale = 1;
            DisplayOptions opts = GetJointTypesDisplayOptions(data);
            opts.IsExport = true;

            sheetNo++;
            AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Joints.GetFriendlyName(), exportOpts);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Joints.GetFriendlyName() });

            //XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
            //gfx.DrawString("JDetails - Joints:", fontBold, XBrushes.Black, 20, 20);

            XFont font = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);
            XFont font2 = new XFont(fontFamily, 18, XFontStyle.Bold, options);
            XPen pen = new XPen(XColors.Black, 2);

            double moveX = 5; // Odsadenie od laveho okraja vykresu
            double moveY = 40;
            int maxInRow = 4;
            int maxInColumn = 2;
            int numInRow = 0;
            int numInColumn = 0;
            int tableWidth = 100;
            var tf = new XTextFormatter(gfx);
            XStringFormat format = new XStringFormat();
            format.LineAlignment = XLineAlignment.Near;
            format.Alignment = XStringAlignment.Near;
            XStringFormat formatCenter = new XStringFormat();
            formatCenter.LineAlignment = XLineAlignment.Center;
            formatCenter.Alignment = XStringAlignment.Center;
            int counter = 1;

            foreach (KeyValuePair<CConnectionDescription, CConnectionJointTypes> kvp in data.JointsDict)
            {
                //add new page when whole page is used
                if (numInColumn == maxInColumn)
                {
                    numInColumn = 0;
                    moveY = 40;
                    gfx.Dispose();
                    page.Close();

                    sheetNo++;
                    AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Joints.GetFriendlyName(), exportOpts);
                    contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Joints.GetFriendlyName() });
                    tf = new XTextFormatter(gfx);
                }

                numInRow++;
                CConnectionJointTypes joint = kvp.Value;

                Trackport3D trackport = null;

                Viewport3D viewPort = ExportHelper.GetJointViewPort(joint, opts, data.Model, 1f, out trackport);
                foreach (Visual3D obj3D in viewPort.Children)
                {
                    if (obj3D is ScreenSpaceLines3D) ((ScreenSpaceLines3D)obj3D).Rescale();  //the only way to draw line in 3D perspective, offline viewport
                }
                viewPort.UpdateLayout();

                XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort, scale));

                viewPort.Dispose();
                trackport.Dispose();

                double scaleFactor = (gfx.PageSize.Width) / (image.PointWidth) / maxInRow;
                double scaledImageWidth = gfx.PageSize.Width / maxInRow;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                // Nezobrazujeme nazov triedy / typu spoja
                tf.DrawString(/*$"{kvp.Key.Name} [{kvp.Key.JoinType}]"*/ $"{kvp.Key.Name}", font, XBrushes.Black, new Rect(moveX + 35, moveY - 25, scaledImageWidth - 20, scaledImageHeight), format);
                gfx.DrawEllipse(pen, new Rect(moveX, moveY - 30, 30, 30));
                gfx.DrawString($"{counter++}", font2, XBrushes.Black, moveX + (counter > 10 ? 5 : 10), moveY - 9);

                gfx.DrawImage(image, moveX, moveY, scaledImageWidth, scaledImageHeight);
                image.Dispose();

                DrawJointTableToDocument(gfx, moveX, moveY + scaledImageHeight + 4, joint);

                moveX += scaledImageWidth;

                if (numInRow == maxInRow) { numInRow = 0; moveX = 5; moveY += scaledImageHeight + 130; numInColumn++; }
            }

            gfx.Dispose();
            page.Close();
        }

        private static DisplayOptions GetJointTypesDisplayOptions(CModelData data)
        {
            DisplayOptions opts = data.DisplayOptions;
            opts.bUseOrtographicCamera = false;
            opts.bColorsAccordingToMembers = false;
            opts.bColorsAccordingToSections = true;

            opts.bDisplayGlobalAxis = false;
            opts.bDisplaySolidModel = true;
            opts.bDisplayMembersCenterLines = false;
            opts.bDisplayMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta
            opts.bDisplayMemberDescription = false;

            opts.bDisplayMembers = true;
            opts.bDisplayJoints = true;
            opts.bDisplayPlates = true;
            opts.bDisplayConnectors = true;

            opts.bDisplayNodes = false;
            opts.bDisplayNodesDescription = false;
            opts.bDisplayNodalSupports = false;

            opts.bDisplayGridlines = false;
            opts.bDisplaySectionSymbols = false;
            opts.bDisplayDetailSymbols = false;

            // Do dokumentu exporujeme aj s wireframe
            opts.bDisplayWireFrameModel = true; //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            opts.fWireFrameLineThickness = 2;
            opts.bTransformScreenLines3DToCylinders3D = false;
            opts.bDisplayMembersWireFrame = true;
            opts.bDisplayJointsWireFrame = true;
            opts.bDisplayPlatesWireFrame = true;
            opts.bDisplayConnectorsWireFrame = true;
            opts.wireFrameColor = System.Windows.Media.Colors.Black;

            return opts;
        }

        private static void DrawFootingTypes(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            XGraphics gfx;
            PdfPage page;
            double scale = 1;
            DisplayOptions opts = GetFootingTypesDisplayOptions(data);
            opts.IsExport = true;

            DisplayOptionsFootingPad2D opts2D = DisplayOptionsHelper.GetDefaultForExport();

            sheetNo++;
            AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Footing_Pads.GetFriendlyName(), exportOpts);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Footing_Pads.GetFriendlyName() });

            //XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
            //gfx.DrawString("Footing Pads:", fontBold, XBrushes.Black, 20, 20);

            XFont font = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);

            double moveX = 0;
            double moveY = 30;
            int maxInRow = 2;
            int maxInColumn = 1;
            int numInRow = 0;
            int numInColumn = 0;
            //var tf = new XTextFormatter(gfx);
            //XStringFormat format = new XStringFormat();
            //format.LineAlignment = XLineAlignment.Near;
            //format.Alignment = XStringAlignment.Near;

            foreach (KeyValuePair<string, Tuple<CFoundation, CConnectionJointTypes>> kvp in data.FootingsDict)
            {
                //add new page when whole page is used
                if (numInColumn == maxInColumn)
                {
                    numInColumn = 0;
                    moveY = 30; // Toto je rovnaka hodnota ako ma default
                    gfx.Dispose();
                    page.Close();

                    sheetNo++;
                    AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Footing_Pads.GetFriendlyName(), exportOpts);
                    contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Footing_Pads.GetFriendlyName() });
                }

                numInRow++;
                CFoundation pad = kvp.Value.Item1;
                CConnectionJointTypes joint = kvp.Value.Item2;

                Trackport3D trackport = null;
                Viewport3D viewPort = ExportHelper.GetFootingViewPort(joint, pad, opts, data.Model, 1f, out trackport/*, 1140, 800*/);
                foreach (Visual3D obj3D in viewPort.Children)
                {
                    if (obj3D is ScreenSpaceLines3D) ((ScreenSpaceLines3D)obj3D).Rescale();  //the only way to draw line in 3D perspective, offline viewport
                }
                viewPort.UpdateLayout();

                XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort, scale));

                //double scaleFactor = (gfx.PageSize.Width) / image.PointWidth / maxInRow;
                //double scaledImageWidth = gfx.PageSize.Width / maxInRow;
                //double scaledImageHeight = image.PointHeight * scaleFactor;

                // TO Ondrej - neda sa sem nejako prepasovat tie konstanty pre velkosti frames, aby sme to nemuseli rucne menit ked sa nejako zmeni GUI ???

                double scaledImageWidth = 430;
                double scaleFactor = scaledImageWidth / image.PointWidth;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                //gfx.DrawString($"{kvp.Key}", font, XBrushes.Black, new Rect(moveX, moveY - 15, scaledImageWidth, scaledImageHeight), XStringFormats.TopCenter);
                string columnTypeName = $"{kvp.Key}";
                gfx.DrawString("Pad Type " + pad.Name + " - " + columnTypeName, font, XBrushes.Black, new Rect(moveX, moveY - 15, scaledImageWidth, scaledImageHeight), XStringFormats.TopCenter);
                gfx.DrawImage(image, moveX, moveY, scaledImageWidth, scaledImageHeight);
                image.Dispose();
                viewPort.Dispose();
                trackport.Dispose();
                //DrawFootingTableToDocument(gfx, moveX, moveY + scaledImageHeight + 4, pad);
                DrawFootingTableToDocument(gfx, moveX + scaledImageWidth - 100, moveY, pad);
                
                double Frame2DWidth = 579;
                double Frame2DHeight = 397;
                double scaleFor2D = 0.85;
                Canvas page2D = new Canvas();
                page2D.RenderSize = new Size(Frame2DWidth, Frame2DHeight);
                int margin = 2;
                CSlab floorSlab = data.Model.m_arrSlabs.FirstOrDefault();

                Drawing2D.DrawFootingPadSideElevationToCanvas(pad, joint, floorSlab, Frame2DWidth, Frame2DHeight, ref page2D, opts2D);
                XImage image2 = XImage.FromBitmapSource(ExportHelper.RenderVisual(page2D, scaleFor2D));
                gfx.DrawImage(image2, moveX, moveY + margin + Frame2DHeight * scaleFor2D, Frame2DWidth * scaleFor2D, Frame2DHeight * scaleFor2D);
                image2.Dispose();

                moveX += scaledImageWidth + 130; // Posun medzi stlpcami
                if (numInRow == maxInRow) { numInRow = 0; moveX = 0; moveY += scaledImageHeight + 5; numInColumn++; }
            }
            
            gfx.Dispose();
            page.Close();
        }

        private static DisplayOptions GetFootingTypesDisplayOptions(CModelData data)
        {
            DisplayOptions opts = data.DisplayOptions;
            opts.bUseOrtographicCamera = false;
            opts.bColorsAccordingToMembers = false;
            opts.bColorsAccordingToSections = true;

            opts.bDisplayGlobalAxis = false;
            opts.bDisplaySolidModel = true;
            opts.bDisplayMembersCenterLines = false;
            opts.bDisplayMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta
            opts.bDisplayMemberDescription = false;

            opts.bDisplayMembers = true;
            opts.bDisplayJoints = true;
            opts.bDisplayPlates = true;
            opts.bDisplayConnectors = true;

            opts.bDisplayNodes = false;
            opts.bDisplayNodesDescription = false;
            opts.bDisplayNodalSupports = false;

            opts.bDisplayGridlines = false;
            opts.bDisplaySectionSymbols = false;
            opts.bDisplayDetailSymbols = false;

            // Do dokumentu exporujeme aj s wireframe
            opts.bDisplayWireFrameModel = true; //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            opts.fWireFrameLineThickness = 2;
            opts.bTransformScreenLines3DToCylinders3D = false;
            opts.bDisplayMembersWireFrame = true;
            opts.bDisplayJointsWireFrame = true;
            opts.bDisplayPlatesWireFrame = true;
            opts.bDisplayConnectorsWireFrame = true;
            opts.wireFrameColor = System.Windows.Media.Colors.Black;

            // Foundations
            opts.bDisplayFoundations = true;
            opts.bDisplayReinforcementBars = true;
            opts.bDisplayFoundationsWireFrame = true;
            opts.bDisplayReinforcementBarsWireFrame = true;
            opts.RotateModelX = -80;
            opts.RotateModelY = 45;
            opts.RotateModelZ = 5;

            return opts;
        }

        private static void AddPageToDocument(PdfDocument s_document, CProjectInfo projectInfo, out PdfPage page, out XGraphics gfx, string pageDetails, LayoutsExportOptionsViewModel exportOpts)
        {
            page = s_document.AddPage();
            //page.Size = PageSize.A3;
            //page.Orientation = PdfSharp.PageOrientation.Landscape;
            page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSize);
            page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientation);

            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            DrawTitleBlock(gfx, projectInfo, page, pageDetails, sheetNo, 0);
        }

        private static void DrawStandardDetails(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            XGraphics gfx;
            PdfPage page;
            page = s_document.AddPage();
            //page.Size = PageSize.A3;
            //page.Orientation = PdfSharp.PageOrientation.Landscape;
            page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSize);
            page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientation);
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            sheetNo++;
            DrawTitleBlock(gfx, data.ProjectInfo, page, EPDFPageContentType.Details_Standard_1.GetFriendlyName(), sheetNo, 0);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Standard_1.GetFriendlyName() });

            double scale = 0.2; // 20% of original file dimensions in pixels
            double dImagePosition_x = 2;
            double dImagePosition_y = 2;
            double dRowPosition = 0;

            // 1st row
            XImage image = XImage.FromFile(ConfigurationManager.AppSettings["SD_03_ridge"]);
            double imageWidthOriginal = image.PixelWidth;
            double imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);

            image = XImage.FromFile(ConfigurationManager.AppSettings["SD_04_cladding"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);

            // TODO - skontrolovat ci sa dalsi obrazok vojde do sirky stranky, ak nie pridat novy rad (len ak sa vojde na vysku) alebo novu stranku
            // 2nd row
            dImagePosition_x = 2; // Zaciname znova od laveho okraja
            double dRowPosition2 = dRowPosition;

            image = XImage.FromFile(ConfigurationManager.AppSettings["SD_02_barge"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dRowPosition2, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dRowPosition2 + dImagePosition_y + imageHeightOriginal * scale);

            image = XImage.FromFile(ConfigurationManager.AppSettings["SD_05_cladding_corner"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dRowPosition2, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dRowPosition2 + dImagePosition_y + imageHeightOriginal * scale);

            image = XImage.FromFile(ConfigurationManager.AppSettings["SD_06_gutter"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dRowPosition2, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dRowPosition2 + dImagePosition_y + imageHeightOriginal * scale);

            // TODO - skontrolovat ci sa dalsi obrazok vojde do sirky stranky, ak nie pridat novy rad (len ak sa vojde na vysku) alebo novu stranku
            // 3rd row
            dImagePosition_x = 2; // Zaciname znova od laveho okraja
            double dRowPosition3 = dRowPosition;

            image = XImage.FromFile(ConfigurationManager.AppSettings["SD_01_roof"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dRowPosition3, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dRowPosition3 + dImagePosition_y + imageHeightOriginal * scale);

            gfx.Dispose();
            page.Close();

            if (data.DoorBlocksProperties != null && data.DoorBlocksProperties.Count > 0) // Some door exists
            {
                bool bAddRollerDoorDetail = false;
                bool bAddPersonnelAccessDoorDetail = false;
                foreach (DoorProperties prop in data.DoorBlocksProperties)
                {
                    if (prop.sDoorType == "Roller Door")
                        bAddRollerDoorDetail = true;

                    if (prop.sDoorType == "Personnel Door")
                        bAddPersonnelAccessDoorDetail = true;
                }

                XGraphics gfx2;
                PdfPage page2;
                page2 = s_document.AddPage();
                page2.Size = PageSize.A3;
                page2.Orientation = PdfSharp.PageOrientation.Landscape;
                gfx2 = XGraphics.FromPdfPage(page2);

                DrawPDFLogo(gfx2, 0, (int)page.Height.Point - 90);
                DrawCopyRightNote(gfx2, 400, (int)page.Height.Point - 15);

                sheetNo++;
                DrawTitleBlock(gfx2, data.ProjectInfo, page, EPDFPageContentType.Details_Standard_2.GetFriendlyName(), sheetNo, 0);
                contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Standard_2.GetFriendlyName() });

                dImagePosition_x = 2;
                // 1st row
                if ((bAddRollerDoorDetail && !bAddPersonnelAccessDoorDetail) || (!bAddRollerDoorDetail && bAddPersonnelAccessDoorDetail)) // Add roller or personnel door detail
                {
                    string fileName = (bAddRollerDoorDetail == true) ? "SD_07_roller_door_surround" : "SD_08_pa_door_surround";
                    image = XImage.FromFile(ConfigurationManager.AppSettings[fileName]);
                    imageWidthOriginal = image.PixelWidth;
                    imageHeightOriginal = image.PixelHeight;
                    gfx2.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
                    dImagePosition_x += imageWidthOriginal * scale;
                    dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);
                }
                else // Add both details
                {
                    image = XImage.FromFile(ConfigurationManager.AppSettings["SD_07_roller_door_surround"]);
                    imageWidthOriginal = image.PixelWidth;
                    imageHeightOriginal = image.PixelHeight;
                    gfx2.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
                    dImagePosition_x += imageWidthOriginal * scale;
                    dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);

                    image = XImage.FromFile(ConfigurationManager.AppSettings["SD_08_pa_door_surround"]);
                    imageWidthOriginal = image.PixelWidth;
                    imageHeightOriginal = image.PixelHeight;
                    gfx2.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
                    dImagePosition_x += imageWidthOriginal * scale;
                    dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);
                }
                gfx2.Dispose();
                page2.Close();
            }
        }

        private static void DrawFloorDetails(PdfDocument s_document, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            XGraphics gfx;
            PdfPage page;
            page = s_document.AddPage();
            //page.Size = PageSize.A3;
            //page.Orientation = PdfSharp.PageOrientation.Landscape;
            page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSize);
            page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientation);
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            sheetNo++;

            DrawTitleBlock(gfx, data.ProjectInfo, page, EPDFPageContentType.Details_Floor.GetFriendlyName(), sheetNo, 0);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Floor.GetFriendlyName() });

            double scale = 0.2; // 20% of original file dimensions in pixels
            double dImagePosition_x_First = 2;
            double dImagePosition_x = dImagePosition_x_First;
            double dImagePosition_y = 0;

            double dTitlePositionOffset_x = 20; // Medzi lavym okrajom obrazka a nadpisom
            double dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x; // Pozicia textu nadpisu
            double dTitleLineHeight_1 = 20; // Vyska riadku nadpisu
            double dTitleLineHeight_2 = 15; // Vyska riadku nadpisu
            double dTitleBottomOffset = 5; // Medzera medzi spodnou hranou textu a obrazkom
            double dTitlePosition_y = dTitleLineHeight_1; // Pozicia textu nadpisu
            double dTitleSpaceHeight = dTitlePosition_y + dTitleBottomOffset; // Celkova vyska nadpisu vratane odstupu - rozne pre jednotlive nadpisy / obrazky
            double dCurrentRowPosition = 0;
            double dNextRowPosition = 0;

            XFont fontDimension = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);
            XBrush brushDimension = XBrushes.DarkOrange;

            XFont fontNote = new XFont(fontFamily, fontSizeDetailTable, XFontStyle.Bold, options);
            XBrush brushNote = XBrushes.Black;

            XFont fontBold_Title1 = new XFont("Verdana", 17, XFontStyle.Bold, options);
            XFont fontBold_Title2 = new XFont("Verdana", 12, XFontStyle.Bold, options);

            CSlab slab = data.Model.m_arrSlabs.FirstOrDefault();
            if (slab == null) return;
            // To Mato - je to takto dobre? ze ak je null tak nemame co robit? 
            // To Ondrej - v podstate je to chyba alebo do znacnej miery vynimka
            // Mohlo by sa stat ze niekto bude chciet budovu bez betonovej podlahy napriklad nejaky pristresok pre zvierata alebo na nejake stroje
            // Mozno by stalo za to mat v GUI nejaky checkbox  v UC FootingInput ci sa ma vobec floor generovat
            // Ak by bol unchecked, tak by sa podlaha ani dalsie objekty v nej (saw cuts, control joints, perimeters, rebates, description text, ...)
            // vobec negenerovala a ani sa nevytvaralo v PDF layout pre floor a floor details
            // Asi to nie je extra dolezite, ale mozes sa s tym pohrat v ramci review "floor slab" :)
            // Aspon budes mat lepsi prehlad toho ako to teraz funguje, tipujem ze pri tom odhalis ine suvislosti, slabiny a nedorobky

            // 1st row
            if (slab.SawCuts.Count > 0)
            {
                // Vlozime nadpis
                gfx.DrawString("SAW CUT", fontBold_Title1, XBrushes.Black, dTitlePosition_x, dTitlePosition_y);

                dTitleSpaceHeight = dTitleLineHeight_1 + dTitleBottomOffset;
                dImagePosition_y = dCurrentRowPosition + dTitleSpaceHeight;

                // Vlozime obrazok
                XImage imageSC = XImage.FromFile(ConfigurationManager.AppSettings["SawCutDetail"]);
                double imageWidthOriginalSC = imageSC.PixelWidth;
                double imageHeightOriginalSC = imageSC.PixelHeight;
                gfx.DrawImage(imageSC, dImagePosition_x, dImagePosition_y, imageWidthOriginalSC * scale, imageHeightOriginalSC * scale);
                imageSC.Dispose();

                // Vlozime popisy do obrazku
                if (data.Model.m_arrSlabs != null && data.Model.m_arrSlabs.Count > 0)
                {
                    if (data.Model.m_arrSlabs.FirstOrDefault().ReferenceSawCut != null)
                    {
                        string sCutWidth = (data.Model.m_arrSlabs.FirstOrDefault().ReferenceSawCut.CutWidth * 1000).ToString("F0");
                        gfx.DrawString(sCutWidth, fontDimension, brushDimension, dImagePosition_x + 113, dImagePosition_y + 15);

                        string sCutDepth = (data.Model.m_arrSlabs.FirstOrDefault().ReferenceSawCut.CutDepth * 1000).ToString("F0");
                        gfx.DrawString(sCutDepth, fontDimension, brushDimension, dImagePosition_x + 58, dImagePosition_y + 38);
                    }
                }

                // Nastavime pozicie pre vlozenie dalsieho nadpisu a obrazku
                dImagePosition_x += imageWidthOriginalSC * scale;
                dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
                dNextRowPosition = Math.Max(dNextRowPosition, dImagePosition_y + imageHeightOriginalSC * scale);
            }

            if (slab.ControlJoints.Count > 0)
            {
                // Vlozime nadpis
                gfx.DrawString("CONTROL JOINT", fontBold_Title1, XBrushes.Black, dTitlePosition_x, dTitlePosition_y);

                dTitleSpaceHeight = dTitleLineHeight_1 + dTitleBottomOffset;
                dImagePosition_y = dCurrentRowPosition + dTitleSpaceHeight;

                // Vlozime obrazok
                XImage imageJD = XImage.FromFile(ConfigurationManager.AppSettings["ControlJointDetail"]);
                double imageWidthOriginalJD = imageJD.PixelWidth;
                double imageHeightOriginalJD = imageJD.PixelHeight;
                gfx.DrawImage(imageJD, dImagePosition_x, dImagePosition_y, imageWidthOriginalJD * scale, imageHeightOriginalJD * scale);
                imageJD.Dispose();

                // Vlozime popisy do obrazku
                if (data.Model.m_arrSlabs != null && data.Model.m_arrSlabs.Count > 0)
                {
                    if (data.Model.m_arrSlabs.FirstOrDefault().ReferenceControlJoint != null)
                    {
                        /*
                        string sText = "D"+(data.Model.m_arrControlJoints[0].ReferenceDowel.Diameter_shank*1000).ToString("F0") + " GALVANISED DOWEL"+
                            " ("+ (data.Model.m_arrControlJoints[0].ReferenceDowel.Length * 1000).ToString("F0") + " mm LONG) / "+
                            (data.Model.m_arrControlJoints[0].DowelSpacing * 1000).ToString("F0") + " CENTRES \n (WRAP ONE SIDE WITH DENSO TAPE)";
                        */

                        string sText1 = "D" + (data.Model.m_arrSlabs.FirstOrDefault().ReferenceControlJoint.ReferenceDowel.Diameter_shank * 1000).ToString("F0") + " GALVANISED DOWEL";
                        string sText2 = "(" + (data.Model.m_arrSlabs.FirstOrDefault().ReferenceControlJoint.ReferenceDowel.Length * 1000).ToString("F0") + " mm LONG) / " +
                            (data.Model.m_arrSlabs.FirstOrDefault().ReferenceControlJoint.DowelSpacing * 1000).ToString("F0") + " CENTRES";
                        string sText3 = "WRAP ONE SIDE WITH DENSO TAPE";

                        gfx.DrawString(sText1, fontNote, brushNote, dImagePosition_x + 112, dImagePosition_y + 115);
                        gfx.DrawString(sText2, fontNote, brushNote, dImagePosition_x + 112, dImagePosition_y + 125);
                        gfx.DrawString(sText3, fontNote, brushNote, dImagePosition_x + 112, dImagePosition_y + 135);
                    }
                }

                // Nastavime pozicie pre vlozenie dalsieho nadpisu a obrazku
                dImagePosition_x += imageWidthOriginalJD * scale;
                dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
                dNextRowPosition = Math.Max(dNextRowPosition, dImagePosition_y + imageHeightOriginalJD * scale);
            }

            // TODO - skontrolovat ci sa dalsi obrazok vojde do sirky stranky, ak nie pridat novy rad (len ak sa vojde na vysku) alebo novu stranku
            // 2nd row
            dImagePosition_x = dImagePosition_x_First; // Zaciname znova od laveho okraja
            dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
            dCurrentRowPosition = dNextRowPosition;

            List<string> sectionDetailsLetters = new List<string>() { "A", "B", "C", "D", "E", "F" };
            int sectionDetailsLetterIndex = 0;
            string letter;
            List<CSlabPerimeter> diff_perimetersWithoutRebates = ModelHelper.GetDifferentPerimetersWithoutRebates(slab.PerimeterBeams);
            List<CSlabPerimeter> diff_perimetersWithRebates = ModelHelper.GetDifferentPerimetersWithRebates(slab.PerimeterBeams);
            
            foreach (CSlabPerimeter perimeter in diff_perimetersWithoutRebates)
            {
                // Nacitame obrazok
                XImage image = XImage.FromFile(ConfigurationManager.AppSettings["PerimeterDetail"]);
                double imageWidthOriginal = image.PixelWidth;
                double imageHeightOriginal = image.PixelHeight;

                // Skontrolujeme ci obrazok vojde do riadku, inak vkladame na novy riadok
                if (dImagePosition_x + imageWidthOriginal * scale > gfx.PageSize.Width)
                {
                     dImagePosition_x = dImagePosition_x_First;
                     dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
                     dCurrentRowPosition = dNextRowPosition;
                }

                // Vlozime nadpis
                letter = sectionDetailsLetters[sectionDetailsLetterIndex];
                sectionDetailsLetterIndex++;
                gfx.DrawString($"SECTION {letter}-{letter}", fontBold_Title1, XBrushes.Black, dTitlePosition_x, dCurrentRowPosition + dTitleLineHeight_1);
                gfx.DrawString("PERIMETER", fontBold_Title1, XBrushes.Black, dTitlePosition_x, dCurrentRowPosition + 2 * dTitleLineHeight_1);

                dTitleSpaceHeight = 2 * dTitleLineHeight_1 + dTitleLineHeight_2 + dTitleBottomOffset; // Rovnake ako rebate, lebo chceme aby boli obrazky pekne vedla seba
                dImagePosition_y = dCurrentRowPosition + dTitleSpaceHeight;

                // Vlozime obrazok
                gfx.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
                image.Dispose();

                // Vlozime popisy do obrazku
                float fPerimeterDepth = perimeter.PerimeterDepth;
                float fPerimeterBottomWidth = perimeter.PerimeterWidth;
                float fMeshAndStartersOverlapping = perimeter.StartersLapLength;
                float fStartersDiameter = perimeter.Starters_Phi;
                float fStartersSpacing = perimeter.StartersSpacing;
                float fLongitud_Reinf_TopAndBotom_Phi = perimeter.Longitud_Reinf_TopAndBotom_Phi;
                float fLongitud_Reinf_Intermediate_Phi = perimeter.Longitud_Reinf_Intermediate_Phi;
                int iLongitud_Reinf_Intermediate_Count = perimeter.Longitud_Reinf_Intermediate_Count;

                float fFloorSlabTopCover = slab.ConcreteCover;
                CFoundation f = data.Model.m_arrFoundations.FirstOrDefault();
                float fPerimeterCover = f.ConcreteCover; // TODO - asi by to mala byt samostatna polozka - property v CPerimeter

                float fStarterTopPosition = fFloorSlabTopCover + 0.02f; // Mesh position + 20 mm
                float fMiddleDimension = fPerimeterDepth - fPerimeterCover;

                string sTextP1 = (fPerimeterDepth * 1000).ToString("F0");
                string sTextP2 = (fPerimeterCover * 1000).ToString("F0");
                string sTextP3 = (fMiddleDimension * 1000).ToString("F0");
                string sTextP4 = (fStarterTopPosition * 1000).ToString("F0");

                string sTextP5 = (fPerimeterBottomWidth * 1000).ToString("F0");
                string sTextP6 = (fMeshAndStartersOverlapping * 1000).ToString("F0") + " lap with mesh";

                string sTextP7 = "HD" + (fStartersDiameter * 1000).ToString("F0") + " Starters";
                string sTextP8 = (fStartersSpacing * 1000).ToString("F0") + " mm crs";

                string sTextP9 = "HD" + (fLongitud_Reinf_TopAndBotom_Phi * 1000).ToString("F0");
                string sTextP10 = iLongitud_Reinf_Intermediate_Count.ToString() + "x" + "HD" + (fLongitud_Reinf_Intermediate_Phi * 1000).ToString("F0");
                string sTextP11 = sTextP9;

                gfx.DrawString(sTextP2, fontDimension, brushDimension, dImagePosition_x + 45, dImagePosition_y + 176);
                gfx.DrawString(sTextP4, fontDimension, brushDimension, dImagePosition_x + 45, dImagePosition_y + 20);
                gfx.DrawString(sTextP5, fontDimension, brushDimension, dImagePosition_x + 90, dImagePosition_y + 172);
                gfx.DrawString(sTextP6, fontDimension, brushDimension, dImagePosition_x + 96, dImagePosition_y + 9);
                gfx.DrawString(sTextP7, fontNote, brushNote, dImagePosition_x + 180, dImagePosition_y + 86);
                gfx.DrawString(sTextP8, fontNote, brushNote, dImagePosition_x + 180, dImagePosition_y + 96);
                gfx.DrawString(sTextP9, fontNote, brushNote, dImagePosition_x + 100, dImagePosition_y + 61);
                gfx.DrawString(sTextP10, fontNote, brushNote, dImagePosition_x + 93, dImagePosition_y + 101);
                gfx.DrawString(sTextP11, fontNote, brushNote, dImagePosition_x + 100, dImagePosition_y + 146);

                // Rotacia textu
                XGraphicsState state = gfx.Save();
                gfx.RotateAtTransform(-90, new XPoint(dImagePosition_x + 35, dImagePosition_y + 96));
                gfx.DrawString(sTextP1, fontDimension, brushDimension, dImagePosition_x + 35, dImagePosition_y + 96);
                gfx.Restore(state);
                state = gfx.Save();
                gfx.RotateAtTransform(-90, new XPoint(dImagePosition_x + 59, dImagePosition_y + 96));
                gfx.DrawString(sTextP3, fontDimension, brushDimension, dImagePosition_x + 59, dImagePosition_y + 96);
                gfx.Restore(state);

                // Updatujeme poziciu pre dalsi pripadny riadok
                dNextRowPosition = Math.Max(dNextRowPosition, dImagePosition_y + imageHeightOriginal * scale);
                // Nastavime pozicie pre vlozenie dalsieho nadpisu a obrazku
                dImagePosition_x += imageWidthOriginal * scale;
                dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
            }

            // Pre kazdy perimeter mozeme vlozit maximalne jeden detail roller door rebate, nezavisle na tom kolko doors v danom perimeter existuje
            // Vsetky detaily rebates pre jeden perimeter budu rovnake, takze detail vlozime len raz
            
            if (data.DoorBlocksProperties != null && data.DoorBlocksProperties.Count > 0) // Some door exists
            {
                foreach (CSlabPerimeter perimeterDiffRebates in diff_perimetersWithRebates)
                {
                    bool bAddRollerDoorDetail = false;
                    foreach (DoorProperties prop in data.DoorBlocksProperties)
                    {
                        // Musime najst konkretny perimeter (left, right, front, back), v ktorom je rebate pre dane roller doors a pouzit hodnoty z konkretneho perimeter
                        if (prop.sDoorType == "Roller Door" && perimeterDiffRebates.BuildingSide == prop.sBuildingSide)
                            bAddRollerDoorDetail = true;
                    }

                    if (bAddRollerDoorDetail) // Add roller door rebate detail // Vsetky detaily rebates pre jeden perimeter budu rovnake, takze detail vlozime len raz
                    {
                        // Nacitame obrazok
                        XImage image = XImage.FromFile(ConfigurationManager.AppSettings["RollerDoorRebateDetail"]);
                        double imageWidthOriginal = image.PixelWidth;
                        double imageHeightOriginal = image.PixelHeight;

                        // Skontrolujeme ci obrazok vojde do riadku, inak vkladame na novy riadok
                        if (dImagePosition_x + imageWidthOriginal * scale > gfx.PageSize.Width)
                        {
                            dImagePosition_x = dImagePosition_x_First;
                            dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
                            dCurrentRowPosition = dNextRowPosition;
                        }

                        // Vlozime nadpis
                        letter = sectionDetailsLetters[sectionDetailsLetterIndex];
                        sectionDetailsLetterIndex++;
                        gfx.DrawString($"SECTION {letter}-{letter}", fontBold_Title1, XBrushes.Black, dTitlePosition_x, dCurrentRowPosition + dTitleLineHeight_1);
                        gfx.DrawString("ROLLER DOOR REBATE", fontBold_Title1, XBrushes.Black, dTitlePosition_x, dCurrentRowPosition + 2 * dTitleLineHeight_1);
                        gfx.DrawString("[Confirm with door manufacture]", fontBold_Title2, XBrushes.Black, dTitlePosition_x, dCurrentRowPosition + 2 * dTitleLineHeight_1 + dTitleLineHeight_2);

                        dTitleSpaceHeight = 2 * dTitleLineHeight_1 + dTitleLineHeight_2 + dTitleBottomOffset;
                        dImagePosition_y = dCurrentRowPosition + dTitleSpaceHeight;

                        // Vlozime obrazok
                        gfx.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
                        image.Dispose();

                        // Vlozime popisy do obrazku
                        float fRebateDepth_Step = 0;
                        float fRebateDepth_Edge = 0;
                        float fRollerDoorRebate = 0f;

                        if (perimeterDiffRebates.SlabRebates != null)
                        {
                            // Tu by sa mali nacitat data pre rebate, v pripade ze su v perimeter viacere rebates s roznymi parametrami mal by sa pridat detail pre kazdy z nich, to sa ale nestane
                            // Neumoznujeme, aby mal kazdy rebate v jednom perimeter inu sirku alebo sklon

                            fRebateDepth_Step = perimeterDiffRebates.SlabRebates.First().RebateDepth_Step; // 0.01f;
                            fRebateDepth_Edge = perimeterDiffRebates.SlabRebates.First().RebateDepth_Edge; // 0.02f;
                            fRollerDoorRebate = perimeterDiffRebates.SlabRebates.First().RebateWidth; // 0.5f;
                        }

                        float fPerimeterDepth = perimeterDiffRebates.PerimeterDepth;
                        float fPerimeterBottomWidth = perimeterDiffRebates.PerimeterWidth;
                        float fMeshAndStartersOverlapping = perimeterDiffRebates.StartersLapLength;
                        float fStartersDiameter = perimeterDiffRebates.Starters_Phi;
                        float fStartersSpacing = perimeterDiffRebates.StartersSpacing;
                        float fLongitud_Reinf_TopAndBotom_Phi = perimeterDiffRebates.Longitud_Reinf_TopAndBotom_Phi;
                        float fLongitud_Reinf_Intermediate_Phi = perimeterDiffRebates.Longitud_Reinf_Intermediate_Phi;
                        int iLongitud_Reinf_Intermediate_Count = perimeterDiffRebates.Longitud_Reinf_Intermediate_Count;

                        float fPerimeterDepthRebate = fPerimeterDepth - fRebateDepth_Edge; // Default (10 + 10 mm)

                        string sTextP1 = (fPerimeterDepthRebate * 1000).ToString("F0");
                        string sTextP3 = (fRebateDepth_Step * 1000).ToString("F0"); // Step
                        string sTextP4 = ((fRebateDepth_Edge - fRebateDepth_Step) * 1000).ToString("F0"); // Slope between the edge and step
                        string sTextP6 = (fRollerDoorRebate * 1000).ToString("F0");

                        CFoundation f = data.Model.m_arrFoundations.FirstOrDefault();
                        float fPerimeterCover = f.ConcreteCover; // TODO - asi by to mala byt samostatna polozka - property v CPerimeter

                        string sTextP2 = (fPerimeterCover * 1000).ToString("F0");
                        string sTextP5 = (fPerimeterBottomWidth * 1000).ToString("F0");
                        string sTextP7 = "HD" + (fStartersDiameter * 1000).ToString("F0") + " Starters";
                        string sTextP8 = (fStartersSpacing * 1000).ToString("F0") + " mm crs";
                        string sTextP9 = "HD" + (fLongitud_Reinf_TopAndBotom_Phi * 1000).ToString("F0");
                        string sTextP10 = iLongitud_Reinf_Intermediate_Count.ToString() + "x" + "HD" + (fLongitud_Reinf_Intermediate_Phi * 1000).ToString("F0");
                        string sTextP11 = sTextP9;

                        //Rotacia textu
                        XGraphicsState state = gfx.Save();
                        gfx.RotateAtTransform(-90, new XPoint(dImagePosition_x + 35, dImagePosition_y + 96));
                        gfx.DrawString(sTextP1, fontDimension, brushDimension, dImagePosition_x + 35, dImagePosition_y + 96);
                        gfx.Restore(state);

                        gfx.DrawString(sTextP2, fontDimension, brushDimension, dImagePosition_x + 45, dImagePosition_y + 176);
                        gfx.DrawString(sTextP3, fontDimension, brushDimension, dImagePosition_x + 25, dImagePosition_y + 26);
                        gfx.DrawString(sTextP4, fontDimension, brushDimension, dImagePosition_x + 25, dImagePosition_y + 51);
                        gfx.DrawString(sTextP5, fontDimension, brushDimension, dImagePosition_x + 90, dImagePosition_y + 172);
                        gfx.DrawString(sTextP6, fontDimension, brushDimension, dImagePosition_x + 130, dImagePosition_y + 9);

                        // Longitudinal reinforcement - Toto je asi zbytocna duplicita, uz je to oznacene v perimeter, ale zatial to tak necham.
                        gfx.DrawString(sTextP9, fontNote, brushNote, dImagePosition_x + 100, dImagePosition_y + 61);
                        gfx.DrawString(sTextP10, fontNote, brushNote, dImagePosition_x + 93, dImagePosition_y + 101);
                        gfx.DrawString(sTextP11, fontNote, brushNote, dImagePosition_x + 100, dImagePosition_y + 146);

                        // Updatujeme poziciu pre dalsi pripadny riadok
                        dNextRowPosition = Math.Max(dNextRowPosition, dImagePosition_y + imageHeightOriginal * scale);
                        // Nastavime pozicie pre vlozenie dalsieho nadpisu a obrazku
                        dImagePosition_x += imageWidthOriginal * scale;
                        dTitlePosition_x = dImagePosition_x + dTitlePositionOffset_x;
                    }
                }
            }

            gfx.Dispose();
            page.Close();
        }

        private static XGraphics DrawTitlePage(PdfDocument s_document, CProjectInfo pInfo, CModelData data, LayoutsExportOptionsViewModel exportOpts)
        {
            PdfPage page = s_document.AddPage();
            //page.Size = PageSize.A3;
            //page.Orientation = PdfSharp.PageOrientation.Landscape;
            page.Size = GetPageSize((EPageSizes)exportOpts.ExportPageSize);
            page.Orientation = GetPageOrientation((EPageOrientation)exportOpts.ExportPageOrientation);
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont(fontFamily, fontSizeTitle, XFontStyle.Regular, options);
            XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);

            XFont fontProjectInfo = new XFont(fontFamily, 20, XFontStyle.Regular, options);
            XFont fontBoltProjectInfo = new XFont(fontFamily, 20, XFontStyle.Bold, options);

            XFont fontBoltTitle = new XFont(fontFamily, 50, XFontStyle.Bold, options);

            // Project info
            gfx.DrawString("Project Name: ", fontProjectInfo, XBrushes.Black, 30, 30);
            if (pInfo.ProjectName != null) gfx.DrawString(pInfo.ProjectName, fontBoltProjectInfo, XBrushes.Black, 30 + 150, 30);

            gfx.DrawString("Project Site: ", fontProjectInfo, XBrushes.Black, 30, 60);
            if (pInfo.ProjectName != null) gfx.DrawString(pInfo.Site, fontBoltProjectInfo, XBrushes.Black, 30 + 150, 60);

            gfx.DrawString("Project Part: ", fontProjectInfo, XBrushes.Black, 30, 90);
            if (pInfo.ProjectName != null) gfx.DrawString(pInfo.ProjectPart, fontBoltProjectInfo, XBrushes.Black, 30 + 150, 90);

            // Preview isometricky pohlad na konstrukciu
            // Bez kot, bez popisov
            DisplayOptions opts = data.DisplayOptions; // Display properties pre export do PDF - TO Ondrej - mohla by to byt samostatna sada nastaveni nezavisla na 3D scene
            opts.bUseOrtographicCamera = false;
            opts.bColorsAccordingToMembers = false;
            opts.bColorsAccordingToSections = true;
            opts.bDisplayGlobalAxis = false;
            opts.bDisplayMemberDescription = false;
            opts.ModelView = (int)EModelViews.ISO_FRONT_RIGHT;
            opts.ViewModelMembers = (int)EViewModelMemberFilters.All;
            opts.bDisplaySolidModel = true;
            opts.bDisplayMembersCenterLines = false;
            opts.bDisplayWireFrameModel = false; //musi byt false, lebo to je neskutocne vela dat a potom OutOfMemory Exception
            opts.bTransformScreenLines3DToCylinders3D = true;
            opts.fWireFrameLineThickness = 2f;

            opts.bDisplayMembers = true;
            opts.bDisplayJoints = false;
            opts.bDisplayPlates = false;

            opts.bDisplayNodes = false;
            opts.bDisplayNodesDescription = false;
            opts.bDisplayNodalSupports = false;

            opts.bDisplayFoundations = false;
            opts.bDisplayFloorSlab = false;
            opts.bDisplaySawCuts = false;
            opts.bDisplayControlJoints = false;

            opts.bDisplayFoundationsDescription = false;
            opts.bDisplayFloorSlabDescription = false;
            opts.bDisplaySawCutsDescription = false;
            opts.bDisplayControlJointsDescription = false;

            opts.bDisplayGridlines = false;
            opts.bDisplaySectionSymbols = false;
            opts.bDisplayDetailSymbols = false;
            opts.bDisplayDimensions = false;

            opts.bCreateHorizontalGridlines = true;
            opts.bCreateVerticalGridlinesFront = false;
            opts.bCreateVerticalGridlinesBack = false;
            opts.bCreateVerticalGridlinesLeft = false;
            opts.bCreateVerticalGridlinesRight = false;

            CModel filteredModel = null;
            Trackport3D trackport = null;
            Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data, 1f, out filteredModel, out trackport);
            viewPort.UpdateLayout();

            XImage imageModel = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort));

            double scaleFactor = (gfx.PageSize.Width / 2) / imageModel.PointWidth;
            double scaledImageWidth = gfx.PageSize.Width / 2;
            double scaledImageHeight = imageModel.PointHeight * scaleFactor;

            gfx.DrawImage(imageModel, gfx.PageSize.Width / 4, gfx.PageSize.Height / 4 - 100, scaledImageWidth, scaledImageHeight);
            imageModel.Dispose();
            viewPort.Dispose();
            trackport.Dispose();

            // Logo
            XImage image = XImage.FromFile(ConfigurationManager.AppSettings["logo2"]);
            //gfx.DrawImage(image, gfx.PageSize.Width - 240 - 50, 630, 240, 75);
            gfx.DrawImage(image, gfx.PageSize.Width - 240 - 50, gfx.PageSize.Height - 212, 240, 75);
            image.Dispose();

            //gfx.DrawString("TO BE READ IN CONJUCTION WITH", fontBold, XBrushes.Black, 900, 730);
            //gfx.DrawString("ARCHITECTURAL PLAN SET", fontBold, XBrushes.Black, 947, 750);
            //gfx.DrawString("ENGINEERING PLAN SET", fontBoltTitle, XBrushes.Black, 530, 800);

            gfx.DrawString("TO BE READ IN CONJUCTION WITH", fontBold, XBrushes.Black, gfx.PageSize.Width - 291, gfx.PageSize.Height - 112);
            gfx.DrawString("ARCHITECTURAL PLAN SET", fontBold, XBrushes.Black, gfx.PageSize.Width - 244, gfx.PageSize.Height - 92);
            gfx.DrawString("ENGINEERING PLAN SET", fontBoltTitle, XBrushes.Black, gfx.PageSize.Width - 661 , gfx.PageSize.Height - 42);

            return gfx;
        }

        // TO Ondrej - ma zmysel mat tieto vnorene metody ak maju rovnake parametre, neviem ci je opodstatnene - ja som to urobil len aby malo vsetko nazov Draw
        private static void DrawTable(XGraphics gfx, int x, int y, List<string[]> tableParams)
        {
            AddTableToDocument(gfx, x, y, tableParams);
        }

        private static void DrawTable(XGraphics gfx, int x, int y, Table t)
        {
            AddTableToDocument(gfx, x, y, t);
        }

        // TO Ondrej - ma zmysel mat tieto vnorene metody ak maju rovnake parametre, neviem ci je opodstatnene - ja som to urobil len aby malo vsetko nazov Draw
        private static void DrawTitleBlock(XGraphics gfx, CProjectInfo pInfo, PdfPage pdfPage, string contents, int sheetNo, int issue) // Tabulka s rozpiskou
        {
            // Velkost pisma mozes nastavit tak, aby bolo zhruba 2.5-3 mm velke, aby nam ta tabulka nezaberal prilis vela miesta, nazov projektu moze byt 5 mm pismom

            AddPageTitleBlockTableToDocument(gfx, pInfo, pdfPage, contents, sheetNo, issue);
        }

        private static void DrawFootingPadList(XGraphics gfx, CModelData data, int x, int y)
        {
            // Title
            XFont font = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);
            gfx.DrawString("Footing Pads", font, XBrushes.Black, x, y);

            // Table
            AddFootingPadListTableToDocument(gfx, data, x, y + 20, 220);
        }

        private static int GetView(EViewModelMemberFilters viewModelMembers)
        {
            if (viewModelMembers == EViewModelMemberFilters.FRONT) return (int)EModelViews.FRONT;
            else if (viewModelMembers == EViewModelMemberFilters.BACK) return (int)EModelViews.BACK;
            else if (viewModelMembers == EViewModelMemberFilters.LEFT) return (int)EModelViews.LEFT;
            else if (viewModelMembers == EViewModelMemberFilters.RIGHT) return (int)EModelViews.RIGHT;
            else if (viewModelMembers == EViewModelMemberFilters.ROOF) return (int)EModelViews.TOP;
            else if (viewModelMembers == EViewModelMemberFilters.MIDDLE_FRAME) return (int)EModelViews.FRONT;
            else if (viewModelMembers == EViewModelMemberFilters.COLUMNS) return (int)EModelViews.TOP;
            else if (viewModelMembers == EViewModelMemberFilters.FOUNDATIONS) return (int)EModelViews.TOP;
            else if (viewModelMembers == EViewModelMemberFilters.FLOOR) return (int)EModelViews.TOP;

            else return (int)EModelViews.ISO_FRONT_RIGHT;
        }
        private static int GetView(EViewCladdingFilters view)
        {
            if (view == EViewCladdingFilters.CLADDING_FRONT) return (int)EModelViews.FRONT;
            else if (view == EViewCladdingFilters.CLADDING_BACK) return (int)EModelViews.BACK;
            else if (view == EViewCladdingFilters.CLADDING_LEFT) return (int)EModelViews.LEFT;
            else if (view == EViewCladdingFilters.CLADDING_RIGHT) return (int)EModelViews.RIGHT;
            else if (view == EViewCladdingFilters.CLADDING_ROOF) return (int)EModelViews.TOP;

            else return (int)EModelViews.ISO_FRONT_RIGHT;
        }

        private static void DrawLogo(XGraphics gfx)
        {
            XImage image = XImage.FromFile(ConfigurationManager.AppSettings["logoForPDF"]);
            gfx.DrawImage(image, 10, 10, 300, 200);
            image.Dispose();
        }

        private static void DrawPDFLogo(XGraphics gfx, int x, int y)
        {
            int width = 240;
            int height = 75;
            XFont font = new XFont(fontFamily, fontSizeLegend, XFontStyle.Regular, options);

            DrawImage(gfx, ConfigurationManager.AppSettings["logo2"], x, y, width, height);

            int textX = x + width;
            int textY = y + 6;
            int lineHeight = 11;

            gfx.DrawString("Formsteel Technologies", font, XBrushes.Black, textX, textY);
            textY += lineHeight;
            gfx.DrawString("2 - 4 Waokauri Place", font, XBrushes.Black, textX, textY);
            textY += lineHeight;
            gfx.DrawString("Mangere, Auckland", font, XBrushes.Black, textX, textY);
            textY += lineHeight;
            gfx.DrawString("tel 09 275 0089", font, XBrushes.Black, textX, textY);
            textY += lineHeight;
            gfx.DrawString("fax 09 257 2650", font, XBrushes.Black, textX, textY);
            textY += lineHeight;
            gfx.DrawString("free 0800 800 003", font, XBrushes.Black, textX, textY);
            textY += lineHeight;
            gfx.DrawString("sales@formsteel.co.nz", font, XBrushes.Blue, textX, textY);
            textY += lineHeight;
            gfx.DrawString("www.formsteel.co.nz", font, XBrushes.Blue, textX, textY);
            //Adding web link in older versions
            //PdfSharp.Drawing.XRect rect2 = gfx.Transformer.WorldToDefaultPage(new PdfSharp.Drawing.XRect(new PdfSharp.Drawing.XPoint(textX, textY), new XSize(100, 12)));
            //PdfSharp.Pdf.PdfRectangle rc2 = new PdfSharp.Pdf.PdfRectangle(rect2);
            //gfx.PdfPage.AddWebLink(rc2, "www.formsteel.co.nz");
        }

        private static void DrawCopyRightNote(XGraphics gfx, int x, int y)
        {
            string s = "© Formsteel Technologies 2019";
            XFont font = new XFont(fontFamily, fontSizeLegend, XFontStyle.Bold, options);
            gfx.DrawString(s, font, XBrushes.Red, x, y);
        }

        private static void DrawImage(XGraphics gfx, string path, int x, int y, int width, int height)
        {
            XImage image = XImage.FromFile(path);
            gfx.DrawImage(image, x, y, width, height);
            image.Dispose();
        }

        private static void DrawCrscLegendTable(XGraphics gfx, CModel model, int x, int textWidth = 80, int imgWidth = 100)
        {
            List<string> list_crsc = GetCrscFromModel(model);
            int margins = 12;
            Document doc = new Document();
            DateTime start = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("Beginning: DrawCrscLegendTable" + (DateTime.Now - start).TotalMilliseconds);
            AddTableToDocument(doc, gfx, x - imgWidth - textWidth - margins, 20, GetCRSC_Table(doc, model, list_crsc, textWidth, imgWidth));
            //System.Diagnostics.Trace.WriteLine("End: DrawCrscLegendTable " + (DateTime.Now - start).TotalMilliseconds);
        }
        
        private static Table GetCRSC_Table(Document document, CModel model, List<string> list_crsc, int textWidth, int imgWidth = 100, int imgHeight = 76)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.LeftPadding = 0;
            table.RightPadding = 0;
            table.TopPadding = 0;
            table.BottomPadding = 0;
            table.Borders.Width = 0.5;
            table.Format.Font.Name = fontFamily;
            table.Format.Font.Size = fontSizeLegend;

            Column column1 = table.AddColumn(Unit.FromCentimeter(2));
            column1.Format.Alignment = ParagraphAlignment.Left;
            column1.Width = textWidth;
            column1.LeftPadding = 2;
            //column1.Format.Font.Bold = true;
            Column column2 = table.AddColumn(Unit.FromCentimeter(2));
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Width = imgWidth + 4;
            column2.LeftPadding = 2;

            DateTime start = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("Beginning: GetCRSC_Table" + (DateTime.Now - start).TotalMilliseconds);

            foreach (string crsc in list_crsc)
            {
                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                cell.AddParagraph("");
                // List of member types
                List<string> list_memberTypes = GetMemberTypesWithCrscFromModel(model, crsc);
                foreach (string s in list_memberTypes)
                {
                    cell.AddParagraph($"[{s}]");
                }
                // Cross-section name
                cell.AddParagraph(crsc);

                //System.Diagnostics.Trace.WriteLine(crsc + " before DATABASE.CSectionManager.GetSectionProperties(crsc)" + (DateTime.Now - start).TotalMilliseconds);
                // TEK screws number, gauge and distance - TO Ondrej - mozem to nacitavat tu z databazy znova alebo je lepsie dostat sem nie len crsc string ale cely objekt a necitat to z neho
                DATABASE.DTO.CrScProperties crscProp = DATABASE.CSectionManager.GetSectionProperties(crsc); // Load cross-section properties
                //System.Diagnostics.Trace.WriteLine(crsc + " after DATABASE.CSectionManager.GetSectionProperties(crsc)" + (DateTime.Now - start).TotalMilliseconds);

                if (crscProp.IsBuiltUp == true)
                {
                    string sScrewsDescrtiption = crscProp.iScrewsNumber + "/" + crscProp.iScrewsGauge + "g" + " teks@" + (crscProp.dScrewDistance * 1000).ToString("F0") + "c/c";
                    cell.AddParagraph(sScrewsDescrtiption);
                }

                //System.Diagnostics.Trace.WriteLine(crsc + " before cell.AddImage" + (DateTime.Now - start).TotalMilliseconds);
                cell = row.Cells[1];
                MigraDoc.DocumentObjectModel.Shapes.Image img = cell.AddImage(ConfigurationManager.AppSettings[crsc]);
                img.Width = imgWidth;
                img.Height = imgHeight;

                
                //System.Diagnostics.Trace.WriteLine(crsc + "end for" + (DateTime.Now - start).TotalMilliseconds);
            }

            table.SetEdge(0, 0, 2, list_crsc.Count, Edge.Box, BorderStyle.Single, 1, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            //System.Diagnostics.Trace.WriteLine("End: GetCRSC_Table" + (DateTime.Now - start).TotalMilliseconds);
            return table;
        }
        //private static void DrawCrscLegend(XGraphics gfx, CModel model, int x, int textWidth)
        //{
        //    List<string> list_crsc = GetCrscFromModel(model);

        //    int width = 100;
        //    int height = 76;
        //    int y = 20;
        //    int font_y = 20;

        //    XFont font = new XFont(fontFamily, fontSizeLegend, XFontStyle.Regular, options);

        //    foreach (string crsc in list_crsc)
        //    {
        //        DrawImage(gfx, ConfigurationManager.AppSettings[crsc], x, y, width, height);

        //        // List of member types
        //        List<string> list_memberTypes = GetMemberTypesWithCrscFromModel(model, crsc);
        //        font_y = 20;
        //        foreach (string s in list_memberTypes)
        //        {
        //            gfx.DrawString($"[{s}]", font, XBrushes.Black, x - textWidth, y + font_y);
        //            font_y += 15;
        //        }

        //        // Cross-section name
        //        gfx.DrawString($"{crsc}", font, XBrushes.Black, x - textWidth, y + font_y); // cross-section name
        //        font_y += 15;

        //        // TEK screws number, gauge and distance - TO Ondrej - mozem to nacitavat tu z databazy znova alebo je lepsie dostat sem nie len crsc string ale cely objekt a necitat to z neho
        //        DATABASE.DTO.CrScProperties crscProp = DATABASE.CSectionManager.GetSectionProperties(crsc); // Load cross-section properties

        //        if (crscProp.IsBuiltUp == true)
        //        {
        //            string sScrewsDescrtiption = crscProp.iScrewsNumber + "/" + crscProp.iScrewsGauge + "g" + " teks@" + (crscProp.dScrewDistance * 1000).ToString("F0") + "c/c";
        //            gfx.DrawString(sScrewsDescrtiption, font, XBrushes.Black, x - textWidth, y + font_y); // built-up cross-section number of screws and distance
        //        }

        //        y += height;
        //    }
        //}

        private static void DrawNotes_Floor(XGraphics gfx, CModelData data, int x, int y)
        {
            int iVerticalOffset_y = 10;
            int yPosition = y;
            XFont fontTitle = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);
            XFont font = new XFont(fontFamily, fontSizeNotes, XFontStyle.Regular, options);

            string sNoteTitle = "Notes";

            // TODO Ondrej - sem by som potreboval dostat data z UC_FootingInput, resp z view modelu
            // TODO Ondrej - parametre soil capacity, concrete strength atd by mali byt spolocne pre vsetky foundation pads
            
            //TODO Mato - ukazku som spravil...tak mozes dostat dalsie parametre...vsetko treba preniest do CModelData a potom to tu kade tade vyuzivat
            // ja mam problem identifikovat premmenne ktore chces preniest, tak preto mas len ukazku ako na to
            //Task 366
            string sNote_1 = "1) Minimum ultimate ground bearing capacity " + data.SoilBearingCapacity.ToString("F0") + " kPa."; // Ukazka ako to dostat z Footing Input
            string sNote_2 = "2) Concrete grade " + data.Model.m_arrFoundations.FirstOrDefault().m_Mat.Name + " MPa for footing pads."; // TODO - dostat sem vstup z UC_Footing pad a ponapajat na GUI, resp zabezpecit update materialu v objekte CFoundation pre zmene v GUI
            string sNote_3 = "3) Concrete grade " + data.Model.m_arrSlabs.FirstOrDefault().m_Mat.Name + " MPa for floor slab."; // TODO - dostat sem vstup z UC_Footing pad a ponapajat na GUI, resp zabezpecit update materialu v objekte CFoundation pre zmene v GUI
            string sNote_41 = "4) If top soil encountered on site that should be removed";
            string sNote_42 = "   and replaced with compacted engineered soil.";

            gfx.DrawString(sNoteTitle, fontTitle, XBrushes.Black, x, yPosition);
            yPosition += 20;

            gfx.DrawString(sNote_1, font, XBrushes.Black, x, yPosition);
            yPosition += iVerticalOffset_y;
            gfx.DrawString(sNote_2, font, XBrushes.Black, x, yPosition);
            yPosition += iVerticalOffset_y;
            gfx.DrawString(sNote_3, font, XBrushes.Black, x, yPosition);
            yPosition += iVerticalOffset_y;
            gfx.DrawString(sNote_41, font, XBrushes.Black, x, yPosition);
            yPosition += iVerticalOffset_y;
            gfx.DrawString(sNote_42, font, XBrushes.Black, x, yPosition);
            //yPosition += iVerticalOffset_y;
        }

        private static List<string> GetCrscFromModel(CModel model)
        {
            List<string> list_crsc = new List<string>();
            foreach (CMember m in model.m_arrMembers)
            {
                if (m.CrScStart != null)
                {
                    if (!list_crsc.Contains(m.CrScStart.Name_short)) list_crsc.Add(m.CrScStart.Name_short);
                }
                if (m.CrScEnd != null)
                {
                    if (!list_crsc.Contains(m.CrScEnd.Name_short)) list_crsc.Add(m.CrScEnd.Name_short);
                }
            }
            return list_crsc;
        }

        private static List<string> GetMemberTypesWithCrscFromModel(CModel model, string crscName)
        {
            List<string> list_memberTypes = new List<string>();
            foreach (CMember m in model.m_arrMembers)
            {
                if (m.CrScStart != null && m.CrScStart.Name_short == crscName)
                {
                    if (!list_memberTypes.Contains(m.EMemberTypePosition.GetFriendlyName())) list_memberTypes.Add(m.EMemberTypePosition.GetFriendlyName());
                }
                if (m.CrScEnd != null && m.CrScEnd.Name_short == crscName)
                {
                    if (!list_memberTypes.Contains(m.EMemberTypePosition.GetFriendlyName())) list_memberTypes.Add(m.EMemberTypePosition.GetFriendlyName());
                }
            }
            return list_memberTypes;
        }

        /*
        private static CProjectInfo GetProjectInfo()
        {
            CProjectInfo pInfo = new CProjectInfo("New self storage", "8 Forest Road, Stoke", "B6351", "Building 1", DateTime.Now);
            return pInfo;
        }
        */

        private static void DrawProjectInfo(XGraphics gfx, CProjectInfo pInfo)
        {
            XFont font = new XFont(fontFamily, fontSizeTitle, XFontStyle.Regular, options);
            XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);

            int offsetX1 = 320;
            int offsetX2 = 510;
            gfx.DrawString("Project Name: ", font, XBrushes.Black, offsetX1, 20);
            if (pInfo.ProjectName != null) gfx.DrawString(pInfo.ProjectName, fontBold, XBrushes.Black, offsetX1, 40);

            gfx.DrawString("Site: ", font, XBrushes.Black, offsetX1, 60);
            if (pInfo.Site != null) gfx.DrawString(pInfo.Site, fontBold, XBrushes.Black, offsetX1, 80);

            gfx.DrawString("Project Number: ", font, XBrushes.Black, offsetX1, 140);
            if (pInfo.ProjectNumber != null) gfx.DrawString(pInfo.ProjectNumber, fontBold, XBrushes.Black, offsetX1, 160);

            gfx.DrawString("Project Part: ", font, XBrushes.Black, offsetX2, 20);
            gfx.DrawString(pInfo.ProjectPart, fontBold, XBrushes.Black, offsetX2, 40);

            gfx.DrawString("Date: ", font, XBrushes.Black, offsetX2, 140);
            gfx.DrawString(pInfo.Date.ToShortDateString(), fontBold, XBrushes.Black, offsetX2, 160);

        }

        private static void DrawBasicGeometry(XGraphics gfx, CModelData data)
        {
            XFont fontTitle = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);

            XFont font = new XFont(fontFamily, fontSizeTitle, XFontStyle.Regular, options);
            XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);

            int offsetX1 = 20;
            int offsetX2 = 120;
            int offsetX3 = 220;
            int offsetX4 = 320;

            gfx.DrawString("Basic Geometry - Building", fontTitle, XBrushes.Black, offsetX1, 20);


            gfx.DrawString("Width", font, XBrushes.Black, offsetX1, 60);
            gfx.DrawString("B = ", font, XBrushes.Black, offsetX2, 60);
            gfx.DrawString(data.Width.ToString(), font, XBrushes.Black, offsetX3, 60);
            gfx.DrawString("m", font, XBrushes.Black, offsetX4, 60);

            gfx.DrawString("Length", font, XBrushes.Black, offsetX1, 80);
            gfx.DrawString("L", font, XBrushes.Black, offsetX2, 80);
            gfx.DrawString(data.Length.ToString(), font, XBrushes.Black, offsetX3, 80);
            gfx.DrawString("m", font, XBrushes.Black, offsetX4, 80);

            gfx.DrawString("Height", font, XBrushes.Black, offsetX1, 100);
            gfx.DrawString("H1", font, XBrushes.Black, offsetX2, 100);
            gfx.DrawString(data.WallHeight.ToString(), font, XBrushes.Black, offsetX3, 100);
            gfx.DrawString("m", font, XBrushes.Black, offsetX4, 100);

            gfx.DrawString("Height", font, XBrushes.Black, offsetX1, 120);
            gfx.DrawString("???", font, XBrushes.Black, offsetX2, 120);
            gfx.DrawString("H", font, XBrushes.Black, offsetX2, 100);
            gfx.DrawString(data.WallHeight.ToString(), font, XBrushes.Black, offsetX3, 100);
            gfx.DrawString("m", font, XBrushes.Black, offsetX4, 100);

            gfx.DrawString("Pitch", font, XBrushes.Black, offsetX1, 140);
            gfx.DrawString("α", font, XBrushes.Black, offsetX2, 140);
            gfx.DrawString(data.RoofPitch_deg.ToString(), font, XBrushes.Black, offsetX3, 140);
            gfx.DrawString("deg", font, XBrushes.Black, offsetX4, 140);
        }

        private static void AddBasicGeometryToDocument(XGraphics gfx, CModelData data, double offsetY)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Section sec = doc.AddSection();
            Paragraph para = sec.AddParagraph();
            para.Format.FirstLineIndent = 0;
            para.Format.LeftIndent = 0;

            para.AddFormattedText("Width\t\t");
            //para.Format.SpaceAfter = "3cm";
            FormattedText t = para.AddFormattedText("B =", TextFormat.Bold);
            t.AddTab();
            para.AddFormattedText(data.Width.ToString());
            para.AddText("m");
            t = para.AddFormattedText("2");
            t.Superscript = true;
            para.AddLineBreak();
            para.AddFormattedText("Pitch\t\t");
            para.AddFormattedText("α\t");
            para.AddFormattedText(data.RoofPitch_deg.ToString());
            para.AddFormattedText("\tdeg");

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(40), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), para);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
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
            else if (plate is CConCom_Plate_KES)
            {
                CConCom_Plate_KES plateTemp = (CConCom_Plate_KES)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KFS)
            {
                CConCom_Plate_KFS plateTemp = (CConCom_Plate_KFS)plate;
                platePitch_rad = plateTemp.FSlope_rad;

                if (plateTemp.FSlope_rad > 0)
                    plateName = "Knee Plate - rising";
                else
                    plateName = "Knee Plate - falling";
            }
            else if (plate is CConCom_Plate_KK)
            {
                CConCom_Plate_KK plateTemp = (CConCom_Plate_KK)plate;
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
                image.Dispose();
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

        private static void AddTableToDocument(XGraphics gfx, double offsetX, double offsetY, List<string[]> tableParams)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

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
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        private static void AddTableToDocument(XGraphics gfx, double offsetX, double offsetY, Table t)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }
        private static void AddTableToDocument(Document doc, XGraphics gfx, double offsetX, double offsetY, Table t)
        {
            DateTime start = DateTime.Now;
            System.Diagnostics.Trace.WriteLine("Beginning: AddTableToDocument" + (DateTime.Now - start).TotalMilliseconds);
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            //PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            //pdfRenderer.Document = doc;
            //pdfRenderer.RenderDocument();
            //System.Diagnostics.Trace.WriteLine("after: pdfRenderer.RenderDocument()" + (DateTime.Now - start).TotalMilliseconds);
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);

            docRenderer.PrepareDocument();
            System.Diagnostics.Trace.WriteLine("after: docRenderer.PrepareDocument();" + (DateTime.Now - start).TotalMilliseconds);
            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            System.Diagnostics.Trace.WriteLine("after: docRenderer.RenderObject();" + (DateTime.Now - start).TotalMilliseconds);

            //try
            //{
            //    // Render the paragraph. You can render tables or shapes the same way.
            //    System.Diagnostics.Trace.WriteLine("before: docRenderer.RenderObject();" + (DateTime.Now - start).TotalMilliseconds);
            //    docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //    System.Diagnostics.Trace.WriteLine("after: docRenderer.RenderObject();" + (DateTime.Now - start).TotalMilliseconds);
            //}
            //catch
            //{
            //    docRenderer.PrepareDocument();
            //    System.Diagnostics.Trace.WriteLine("after: docRenderer.PrepareDocument();" + (DateTime.Now - start).TotalMilliseconds);
            //    // Render the paragraph. You can render tables or shapes the same way.
            //    docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(gfx.PageSize.Width * 0.8), t);
            //    System.Diagnostics.Trace.WriteLine("after: docRenderer.RenderObject();" + (DateTime.Now - start).TotalMilliseconds);
            //}

            System.Diagnostics.Trace.WriteLine("ENd: AddTableToDocument" + (DateTime.Now - start).TotalMilliseconds);
        }

        private static void AddTitlePageContentTableToDocument(XGraphics gfx, List<string[]> tableParams)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = GetTitlePageTable(doc, tableParams);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            double dTableRowHeight = 20;

            XFont font1 = new XFont(fontFamily, 20, XFontStyle.Bold);
            gfx.DrawString("Contents:", font1, XBrushes.Black, 30, gfx.PageSize.Height - t.Rows.Count * dTableRowHeight - 20);

            double offsetX = 30;
            double offsetY = gfx.PageSize.Height - t.Rows.Count * dTableRowHeight; // TO Ondrej - Pozicia by sa mohla nastavovat podla toho ze vieme, aky vysoky je riadok tabulky a kolko riadkov tabulka ma
            double width = 300;
            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(width), t);
            gfx.Dispose();
            if (gfx.PdfPage != null) gfx.PdfPage.Close();
        }

        private static Table GetTitlePageTable(Document document, List<string[]> tableParams)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.LeftPadding = 5;
            table.RightPadding = 5;
            table.TopPadding = 1;
            table.BottomPadding = 1;
            table.Borders.Width = 0.75;
            table.Format.Font.Name = fontFamily;
            table.Format.Font.Size = fontSizeNormal;

            Column column1 = table.AddColumn(Unit.FromCentimeter(2));
            column1.Format.Alignment = ParagraphAlignment.Center;
            column1.Format.Font.Bold = true;
            Column column2 = table.AddColumn(Unit.FromCentimeter(8));
            column2.Format.Alignment = ParagraphAlignment.Left;

            foreach (string[] strParams in tableParams)
            {
                Row row = table.AddRow();
                //row.Shading.Color = Colors.PaleGoldenrod;
                Cell cell = row.Cells[0];
                //cell.Shading.Color = MigraDoc.DocumentObjectModel.Colors.PaleGoldenrod;
                cell.AddParagraph(strParams[0]);
                cell = row.Cells[1];
                cell.AddParagraph(strParams[1]);
            }

            table.SetEdge(0, 0, 2, tableParams.Count, Edge.Box, BorderStyle.Single, 1, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            return table;
        }




        private static void AddPageTitleBlockTableToDocument(XGraphics gfx, CProjectInfo projectInfo, PdfPage pdfPage, string contents, int sheetNo, int issue)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = GetPageTitleBlockTable(doc, projectInfo, pdfPage, contents, sheetNo, issue);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            double width = 410;
            double offsetX = gfx.PageSize.Width - width;
            double offsetY = gfx.PageSize.Height - 120;

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(width), t);
        }

        private static Table GetPageTitleBlockTable(Document document, CProjectInfo projectInfo, PdfPage pdfPage, string contents, int sheetNo, int issue)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.LeftPadding = 3;
            table.RightPadding = 1;
            table.TopPadding = 1;
            table.BottomPadding = 1;
            table.Borders.Width = 0.75;
            table.Format.Font.Name = fontFamily;
            table.Format.Font.Size = fontSizeNormal;

            Column column1 = table.AddColumn(Unit.FromCentimeter(3));
            column1.Format.Alignment = ParagraphAlignment.Left;
            Column column2 = table.AddColumn(Unit.FromCentimeter(4));
            column2.Format.Alignment = ParagraphAlignment.Left;
            Column column3 = table.AddColumn(Unit.FromCentimeter(3));
            column3.Format.Alignment = ParagraphAlignment.Left;
            Column column4 = table.AddColumn(Unit.FromCentimeter(4));
            column4.Format.Alignment = ParagraphAlignment.Left;

            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            cell.AddParagraph("Project Title:");
            cell.MergeDown = 1;
            cell = row.Cells[1];
            cell.MergeRight = 2;
            cell.Format.Borders.Bottom.Width = 0;
            cell.Format.Borders.Bottom = null;
            cell.AddParagraph(projectInfo.ProjectName);
            cell.Format.Font.Size = 16;
            cell.Format.Font.Bold = true;

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("");
            cell = row.Cells[1];
            cell.Format.Borders.Top.Width = 0;
            cell.Format.Borders.Top = null;
            cell.MergeRight = 2;
            cell.AddParagraph(projectInfo.Site);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Contents:");
            cell = row.Cells[1];
            cell.MergeRight = 2;
            cell.AddParagraph(contents);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Job No.:");
            cell = row.Cells[1];
            cell.AddParagraph(projectInfo.ProjectNumber);
            cell = row.Cells[2];
            cell.AddParagraph("Date:");
            cell = row.Cells[3];
            cell.AddParagraph(projectInfo.Date.ToShortDateString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Sheet No.:");
            cell = row.Cells[1];
            cell.AddParagraph($"fs {sheetNo.ToString("D2")}");
            cell = row.Cells[2];
            cell.AddParagraph("Scale:");
            cell = row.Cells[3];
            cell.AddParagraph("Not to scale");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Issue:");
            cell = row.Cells[1];
            cell.AddParagraph(issue.ToString());
            cell = row.Cells[2];
            cell.AddParagraph("Size:");
            cell = row.Cells[3];
            //cell.AddParagraph("420x297 - A3");
            //cell.AddParagraph($"{pdfPage.Width.Point}x{pdfPage.Height.Point} - {pdfPage.Size.ToString()}");
            cell.AddParagraph($"{(int)pdfPage.Width.Millimeter}x{(int)pdfPage.Height.Millimeter} - {pdfPage.Size.ToString()}");



            table.SetEdge(0, 0, 4, 6, Edge.Box, BorderStyle.Single, 1.5, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            return table;
        }

        private static void DrawJointTableToDocument(XGraphics gfx, double offsetX, double offsetY, CConnectionJointTypes joint)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            List<string[]> tableParams = new List<string[]>();
            //tableParams.Add(new string[2] { "ID", joint.ID.ToString() });
            if (joint.m_MainMember != null)
                tableParams.Add(new string[2] { "Main Member", joint.m_MainMember.Prefix + " - " + joint.m_MainMember.EMemberTypePosition.GetFriendlyName() });
            if (joint.m_SecondaryMembers != null && joint.m_SecondaryMembers.Length > 0)
                tableParams.Add(new string[2] { "Secondary Member", joint.m_SecondaryMembers[0].Prefix + " - " + joint.m_SecondaryMembers[0].EMemberTypePosition.GetFriendlyName() });
            //if (joint.m_arrPlates != null)
            //    tableParams.Add(new string[2] { "Plates count", joint.m_arrPlates.Length.ToString() });
            if (joint.m_arrPlates != null && joint.m_arrPlates.Length > 0)
            {
                CPlate plate = joint.m_arrPlates.FirstOrDefault();
                if (plate != null)
                {
                    //tableParams.Add(new string[2] { "Plate name", plate.Name.ToString() });
                    //tableParams.Add(new string[2] { "Plate thickness", (plate.Ft * 1000).ToString() + " [mm]" });
                    tableParams.Add(new string[2] { "Plates ", joint.m_arrPlates.Length.ToString() + " x " + plate.Name.ToString() + " - " + "thickness " + (plate.Ft * 1000).ToString() + " [mm]" });
                    //tableParams.Add(new string[2] { "Screws count in plate", plate.ScrewArrangement.IHolesNumber.ToString() });
                    //tableParams.Add(new string[2] { "Screw", "TEK " + (plate.ScrewArrangement.referenceScrew.Gauge +"g").ToString() });

                    CScrewArrangement screwArrangement = plate.ScrewArrangement;
                    if (screwArrangement != null)
                    {
                        tableParams.Add(new string[2] { "Screws", screwArrangement.IHolesNumber.ToString() + " x " +
                    "TEKs " + (screwArrangement.referenceScrew.Gauge + "g").ToString() + " each plate" });

                        if (screwArrangement is CScrewArrangementCircleApexOrKnee) // Knee or apex with circle screw arrangement
                        {
                            CScrewArrangementCircleApexOrKnee circleArrangement = (CScrewArrangementCircleApexOrKnee)screwArrangement;

                            //tableParams.Add(new string[2] { "Number of circles", circleArrangement.INumberOfCirclesInGroup.ToString() });

                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // TODO 370
                            // TODO - Ondrej toto by chcelo nejako zautomatizovat a zjednotit s tym ako vypisujeme v System Component Viewer parametre pre plate a pre screw arrangement
                            // Chcel by som to urobit co najhutnejsie aby sme mali co najmenej riadkov v tabulke
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            // Screws in circles
                            // First circle
                            if (circleArrangement.ListOfSequenceGroups[0].ListSequence[0] is CScrewHalfCircleSequence) // Radius parameters
                            {
                                CScrewHalfCircleSequence seq = (CScrewHalfCircleSequence)circleArrangement.ListOfSequenceGroups[0].ListSequence[0];
                                tableParams.Add(new string[2] { "Circle screws", "4 x " + seq.INumberOfConnectors + " " +
                            "TEKs " + (plate.ScrewArrangement.referenceScrew.Gauge + "g").ToString() +
                            "\nradius: " + (seq.Radius * 1000).ToString("F0") + " [mm]" });
                            }

                            // Second circle
                            if (circleArrangement.ListOfSequenceGroups[0].ListSequence[2] != null && circleArrangement.ListOfSequenceGroups[0].ListSequence[2] is CScrewHalfCircleSequence) // Radius parameters
                            {
                                CScrewHalfCircleSequence seq = (CScrewHalfCircleSequence)circleArrangement.ListOfSequenceGroups[0].ListSequence[2];
                                tableParams.Add(new string[2] { "Circle screws", "4 x " + seq.INumberOfConnectors + " " +
                            "TEKs " + (screwArrangement.referenceScrew.Gauge + "g").ToString() +
                            "\nradius: " + (seq.Radius * 1000).ToString("F0") + " [mm]" });
                            }

                            // Screws in corners
                            if (circleArrangement.BUseAdditionalCornerScrews)
                            {
                                tableParams.Add(new string[2] { "Corner screws", "2 x 4 x " + circleArrangement.IAdditionalConnectorInCornerNumber + " " +
                            "TEKs " + (screwArrangement.referenceScrew.Gauge + "g").ToString() +
                            "\nspacing: "+
                              (circleArrangement.FAdditionalCornerScrewsDistance_x * 1000).ToString("F0") + " x "
                            + (circleArrangement.FAdditionalCornerScrewsDistance_y * 1000).ToString("F0") + " [mm]" });
                            }
                        }
                    }
                    if (plate is CConCom_Plate_B_basic) // Base plate
                    {
                        CConCom_Plate_B_basic plate_B = (CConCom_Plate_B_basic)plate;
                        CAnchorArrangement anchorArrangement = plate_B.AnchorArrangement;
                        if (anchorArrangement != null)
                        {
                            //tableParams.Add(new string[2] { "Number of anchors", anchorArrangement.IHolesNumber.ToString() });
                            //tableParams.Add(new string[2] { "Anchor", "M"+(anchorArrangement.referenceAnchor.Diameter_shank*1000).ToString() + " HD bolts - " + (anchorArrangement.referenceAnchor.Length * 1000).ToString() + " [mm]" });
                            tableParams.Add(new string[2] { "Anchors", anchorArrangement.IHolesNumber.ToString() + " x " + "M" + (anchorArrangement.referenceAnchor.Diameter_shank * 1000).ToString() + " HD bolts - " + (anchorArrangement.referenceAnchor.Length * 1000).ToString() + " [mm]" });
                            tableParams.Add(new string[2] { "Note", "Drypack between plate and floor to suit" });
                        }
                    }
                }
            }

            if(joint.ConnectorGroups != null && joint.ConnectorGroups.Count > 0)
            {
                // TODO - zobecnit - Nemusia byt vsetky CScrew ani nemusi byt prvy typ CScrew ale zatial nemame ine
                if (joint.ConnectorGroups.First().Connectors.FirstOrDefault() is CScrew) 
                {
                    int connectorsCount = joint.ConnectorGroups.Sum(g => g.Connectors.Count);
                    tableParams.Add(new string[2] { "Screws", connectorsCount + " x " +
                    "TEKs " + (((CScrew)joint.ConnectorGroups.First().Connectors.FirstOrDefault()).Gauge + "g").ToString() + " one side" });
                }
            }

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = Get2ColumnTable(doc, tableParams);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(100), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        private static void DrawFootingTableToDocument(XGraphics gfx, double offsetX, double offsetY, CFoundation pad)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            List<string[]> tableParams = new List<string[]>();
            //tableParams.Add(new string[2] { "ID", pad.ID.ToString() });
            //tableParams.Add(new string[2] { "Name", pad.Name });
            //tableParams.Add(new string[2] { "Prefix", pad.Prefix });
            tableParams.Add(new string[2] { "Name", pad.Text });
            //tableParams.Add(new string[2] { "L", pad.m_fDim1.ToString() });
            //tableParams.Add(new string[2] { "W", pad.m_fDim2.ToString() });
            //tableParams.Add(new string[2] { "H", pad.m_fDim3.ToString() });
            tableParams.Add(new string[2] { "Dimensions L x W x H", pad.m_fDim1.ToString() + " x " + pad.m_fDim2.ToString() + " x " + pad.m_fDim3.ToString() + " [m]" });
            if (pad.m_Mat != null) tableParams.Add(new string[2] { "Concrete Grade", pad.m_Mat.Name + " [MPa]" }); // !!! vzdy by mal byt priradeny material
            tableParams.Add(new string[2] { "Reinforcement Grade", pad.Reference_Bottom_Bar_x.m_Mat.Name });

            // Popis vyztuze nezobrazujeme, lebo je v 2D obrazku

            //tableParams.Add(new string[2] { "Count Bottom Bars x", pad.Count_Bottom_Bars_x.ToString() });
            //tableParams.Add(new string[2] { "Count Bottom Bars y", pad.Count_Bottom_Bars_y.ToString() });
            //tableParams.Add(new string[2] { "Count Top Bars x", pad.Count_Top_Bars_x.ToString() });
            //tableParams.Add(new string[2] { "Count Top Bars y", pad.Count_Top_Bars_y.ToString() });

            //tableParams.Add(new string[2] { "Bottom Bars x", pad.Count_Bottom_Bars_x.ToString() + " x " + " HD" + (pad.Reference_Bottom_Bar_x.Diameter * 1000).ToString("F0") });
            //tableParams.Add(new string[2] { "Bottom Bars y", pad.Count_Bottom_Bars_y.ToString() + " x " + " HD" + (pad.Reference_Bottom_Bar_y.Diameter * 1000).ToString("F0") });
            //tableParams.Add(new string[2] { "Top Bars x", pad.Count_Top_Bars_x.ToString() + " x " + " HD" + (pad.Reference_Top_Bar_x.Diameter * 1000).ToString("F0") });
            //tableParams.Add(new string[2] { "Top Bars y", pad.Count_Top_Bars_y.ToString() + " x " + " HD" + (pad.Reference_Top_Bar_y.Diameter * 1000).ToString("F0") });

            tableParams.Add(new string[2] { "Concrete Cover", (pad.ConcreteCover * 1000).ToString("F0") + " [mm]" });

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = Get2ColumnTable(doc, tableParams);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(100), t);
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", para);
        }

        private static void AddFootingPadListTableToDocument(XGraphics gfx, CModelData data, int x, int y, int width)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = GetFootingPadListTable(doc, data);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            //double width = 410;
            double offsetX = x;
            double offsetY = y;

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromPoint(offsetX), XUnit.FromPoint(offsetY), XUnit.FromPoint(width), t);

            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;
        }

        private static Table GetFootingPadListTable(Document document, CModelData data)
        {
            List<string[]> tableParams = new List<string[]>();

            //tableParams.Add(new string[6] { "Type", "Width L [m]", "Width W [m]", "Height H [m]", "Count", "Volume [m³]" });
            tableParams.Add(new string[6] { "Type", "L [m]", "W [m]", "H [m]", "Count [-]", "Volume [m³]" });
            
            foreach (KeyValuePair<string, Tuple<CFoundation, CConnectionJointTypes>> kvp in data.FootingsDict)
            {
                CFoundation pad = kvp.Value.Item1;
                int numberOfPads = data.Model.m_arrFoundations.Where(f => f.Text == pad.Text).Count();
                
                tableParams.Add(new string[6] { /*pad.Text*/ pad.Name, pad.m_fDim1.ToString(), pad.m_fDim2.ToString(), pad.m_fDim3.ToString(), numberOfPads.ToString(), pad.m_fVolume.ToString() });
            }

            Table table = Get6ColumnTable(document, tableParams, 9);
            return table;
        }

        private static void AddPlatesTableToDocument(XGraphics gfx, double offsetY, List<string[]> tableParams)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

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

        // To Ondrej - Refaktoring 
        // Tu by bolo elegantne urobit len jednu funkciu, ktora by podla poctu stlpcov poloziek v tableParams generovala tabulku

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
        public static Table Get6ColumnTable(Document document, List<string[]> tableParams, int iFontSize)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.Borders.Width = 0.75;
            table.Format.Font.Name = fontFamily;
            table.Format.Font.Size = iFontSize;

            Column column1 = table.AddColumn(Unit.FromCentimeter(2));
            column1.Format.Alignment = ParagraphAlignment.Left;
            Column column2 = table.AddColumn(Unit.FromCentimeter(1.2));
            column2.Format.Alignment = ParagraphAlignment.Right;
            Column column3 = table.AddColumn(Unit.FromCentimeter(1.2));
            column3.Format.Alignment = ParagraphAlignment.Right;
            Column column4 = table.AddColumn(Unit.FromCentimeter(1.2));
            column4.Format.Alignment = ParagraphAlignment.Right;
            Column column5 = table.AddColumn(Unit.FromCentimeter(1.5));
            column4.Format.Alignment = ParagraphAlignment.Right;
            Column column6 = table.AddColumn(Unit.FromCentimeter(2));
            column4.Format.Alignment = ParagraphAlignment.Right;

            foreach (string[] strParams in tableParams)
            {
                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                cell.AddParagraph(strParams[0]);
                cell = row.Cells[1];
                cell.AddParagraph(strParams[1]);
                cell = row.Cells[2];
                cell.AddParagraph(strParams[2]);
                cell = row.Cells[3];
                cell.AddParagraph(strParams[3]);
                cell = row.Cells[4];
                cell.AddParagraph(strParams[4]);
                cell = row.Cells[5];
                cell.AddParagraph(strParams[5]);
            }

            table.SetEdge(0, 0, 6, tableParams.Count, Edge.Box, BorderStyle.Single, 1.0, MigraDoc.DocumentObjectModel.Colors.Black);
            sec.Add(table);
            return table;
        }
        public static Table Get2ColumnTable(Document document, List<string[]> tableParams)
        {
            Section sec = document.AddSection();
            Table table = new Table();
            table.Borders.Width = 0.75;
            table.Format.Font.Name = fontFamily;
            table.Format.Font.Size = fontSizeDetailTable;

            Column column1 = table.AddColumn(Unit.FromCentimeter(4));
            column1.Format.Alignment = ParagraphAlignment.Left;
            Column column2 = table.AddColumn(Unit.FromCentimeter(4));
            column2.Format.Alignment = ParagraphAlignment.Left;

            foreach (string[] strParams in tableParams)
            {
                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                if (strParams[0] != null) cell.AddParagraph(strParams[0]);
                cell = row.Cells[1];
                if (strParams[1] != null) cell.AddParagraph(strParams[1]);
            }

            table.SetEdge(0, 0, 2, tableParams.Count, Edge.Box, BorderStyle.Single, 1.0, MigraDoc.DocumentObjectModel.Colors.Black);
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
