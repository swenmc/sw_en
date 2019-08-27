using _3DTools;
using BaseClasses;
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
        private const int fontSizeDetailTable = 8; // Details description tables text
        private static int sheetNo;

        private static XPdfFontOptions options;
        //private static PdfDocument document = null;
        private static List<string[]> contents = new List<string[]>();

        public static void ReportAllDataToPDFFile(CModelData modelData)
        {
            sheetNo = 1;
            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            PdfDocument s_document = new PdfDocument();

            CProjectInfo projectInfo = modelData.ProjectInfo; // GetProjectInfo();

            s_document.Info.Title = projectInfo.ProjectName;
            s_document.Info.Author = "Formsteel Technologies";
            s_document.Info.Subject = "No " + projectInfo.ProjectNumber;
            s_document.Info.Keywords = projectInfo.ProjectNumber + ", " +
                                       "Formsteel Technologies" + ", " +
                                       "cold-formed steel" + ", " +
                                       "portal frame";

            // Vykreslenie zobrazovanych textov a objektov do PDF - zoradene z hora
            //DrawLogo(gfx);
            //DrawProjectInfo(gfx,GetProjectInfo());

            // TODO Ondrej - toto ma byt dynamicke - to znamena ze by sme do tohto zoznamu mali pridavat polozky pocas generovania stranok a az na konci vlozit tabulku na prvu stranku podla toho co sme vygenerovali
            // Podobne ako ked sa vytvaraju obsahy v dokumentoch

            // Tabulka so zoznamom vykresov
            // TODO
            // TO Ondrej - vyrobil by som nejaku triedu s nazvom pageDetails alebo layoutDetails
            // Vyrobil by som nejaky enum s moznymi nazvami vykresov
            // To tej triedy by som daj ako property ten enum, nazov vykresu, mozno enum pre filter pre vykresy ktore sa tvoria z 3D pohladu filtra
            // A potom by som podla enumu pre nazov layoutu alebo enumu pre nazov view vkladal riadky do zoznamu pre tabulku na prvej stranke
            // fs01-fs12 by sa mali generovat automaticky podla toho kolko stranok vygenerujem

            //string[] row1 = new string[2] { "fs01", "Isometric View" };
            //string[] row2 = new string[2] { "fs02", "Front Elevation" };
            //string[] row3 = new string[2] { "fs03", "Back Elevation" };
            //string[] row4 = new string[2] { "fs04", "Left Elevation" };
            //string[] row5 = new string[2] { "fs05", "Right Elevation" };
            //string[] row6 = new string[2] { "fs06", "Roof Layout" };
            //string[] row7 = new string[2] { "fs07", "Middle Frame" };
            //string[] row8 = new string[2] { "fs08", "Columns" };
            //string[] row9 = new string[2] { "fs09", "Foundation Pads" };
            //string[] row10 = new string[2] { "fs10", "Floor Plan" };
            //string[] row11 = new string[2] { "fs11", "Details - Standard 1" };
            //string[] row12 = new string[2] { "fs12", "Details - Standard 2" };
            //string[] row13 = new string[2] { "fs13", "Details - Joints" };
            //string[] row14 = new string[2] { "fs14", "Details - Footing Pads" };

            //List<string[]> tableParams = new List<string[]>() { row1, row2, row3, row4, row5, row6, row7, row8, row9, row10, row11, row12, row13, row14 };

            XGraphics TitlePage_gfx = DrawTitlePage(s_document, projectInfo, modelData); // To Ondrej - vykreslit titulnu stranku so zoznamom vykresov, asi sa musi generovat az na konci podobne ako obsah

            DrawModel3D(s_document, modelData);

            DrawModelViews(s_document, modelData);
            
            DrawJointTypes(s_document, modelData);

            DrawFootingTypes(s_document, modelData);

            DrawFloorDetails(s_document, modelData);

            DrawStandardDetails(s_document, modelData); // To Ondrej - for review


            AddTitlePageContentTableToDocument(TitlePage_gfx, contents);

            //page = s_document.AddPage();
            //gfx = XGraphics.FromPdfPage(page);
            //DrawBasicGeometry(gfx, modelData);

            //page = s_document.AddPage();
            //gfx = XGraphics.FromPdfPage(page);
            //AddBasicGeometryToDocument(gfx, modelData, 10);

            //DrawCanvas_PDF(canvas, page, canvas.RenderSize.Width);

            //double height = DrawCanvasImage(gfx, canvas);
            //DrawImage(gfx);

            // Create demonstration pages
            //new LinesAndCurves().DrawPage(s_document.AddPage());
            //new Shapes().DrawPage(s_document.AddPage());
            //new Paths().DrawPage(s_document.AddPage());
            //new Text().DrawPage(s_document.AddPage());
            //new Images().DrawPage(s_document.AddPage());

            //PdfPage page2 = s_document.AddPage();
            //XGraphics gfx2 = XGraphics.FromPdfPage(page2);
            //AddTableToDocument(gfx2, 50, tableParams);

            string fileName = GetReportPDFName();
            // Save the s_document...
            s_document.Save(fileName);
            // ...and start a viewer
            Process.Start(fileName);
        }

        /// <summary>
        /// Draw scaled 3Model to PDF
        /// </summary>
        private static void DrawModel3D(PdfDocument s_document, CModelData data)
        {
            // TO Ondrej - pre export 3D sceny implementovat samostatne display options podobne ako to mame pre pohlady ModelViews
            XGraphics gfx;
            PdfPage page;
            page = s_document.AddPage();
            page.Size = PageSize.A3;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

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

            opts.bDisplayMembers = true;
            opts.bDisplayJoints = true;
            opts.bDisplayPlates = true;

            opts.bDisplayFoundations = false;
            opts.bDisplayFloorSlab = false;
            opts.bDisplaySawCuts = false;
            opts.bDisplayControlJoints = false;

            opts.bDisplayFoundationsDescription = false;
            opts.bDisplayFloorSlabDescription = false;
            opts.bDisplaySawCutsDescription = false;
            opts.bDisplayControlJointsDescription = false;

            CModel filteredModel = null;
            Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data.Model, out filteredModel);
            viewPort.UpdateLayout();

            XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
            gfx.DrawString("Model in 3D environment: ", fontBold, XBrushes.Black, 20, 20);
                        
            DrawTitleBlock(gfx, data.ProjectInfo, EPDFPageContentType.Isometric_View.GetFriendlyName(), sheetNo, 0);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Isometric_View.GetFriendlyName() });

            int legendImgWidth = 100;
            int legendTextWidth = 60;
            DrawCrscLegend(gfx, filteredModel, (int)page.Width.Point - legendImgWidth, legendTextWidth);

            XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort));

            double scaleFactor = (gfx.PageSize.Width - legendImgWidth - legendTextWidth) / image.PointWidth;
            double scaledImageWidth = gfx.PageSize.Width - legendImgWidth - legendTextWidth;
            double scaledImageHeight = image.PointHeight * scaleFactor;

            gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
            viewPort = null;
            image.Dispose();
            gfx.Dispose();
        }

        private static void DrawModelViews(PdfDocument s_document, CModelData data)
        {
            XGraphics gfx;
            PdfPage page;
            double scale = 1;
            DisplayOptions opts = data.DisplayOptions; // Display properties pre export do PDF - TO Ondrej - mohla by to byt samostatna sada nastaveni nezavisla na 3D scene
            opts.bUseOrtographicCamera = true;
            opts.bColorsAccordingToMembers = false;
            opts.bColorsAccordingToSections = true;
            opts.bDisplayGlobalAxis = false;
            opts.bDisplaySolidModel = true;
            opts.bDisplayMembersCenterLines = false;
            opts.bDisplayWireFrameModel = false;   //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            opts.bTransformScreenLines3DToCylinders3D = true;

            opts.bDisplayMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta

            opts.bDisplayMembers = true;
            //opts.bDisplayJoints = true;
            //opts.bDisplayPlates = true;

            // TO Ondrej - Tu by to chcelo vymysliet nejaky mechanizmus, ktory na zaklade rozmerov vykresu a velkosti obrazku modelu urci aka ma byt vyska textu v jednotlivych pohladoch, na papieri by to malo byt cca - 2-2.5 mm, pripadne do 3 mm (6 - 8 PT)
            // Vysku textu mozeme nastavovat ako velkost fontu ale pre export do 2D je lepsie uzivatelsky nastavovat velkost v mm lebo stavbari nevedia aky velky je font c. 8, pripadne tam bude prepocet z bodov na mm

            /*
            7 PT    9 PX    2.5 MM  0.6 EM   60 %
            7 PT    10 PX   2.5 MM  0.6 EM   60 %
            8 PT    11 PX   2.8 MM  0.7 EM   70 %
            9 PT    12 PX   3.4 MM  0.8 EM   80 %
            9 PT    13 PX   3.4 MM  0.8 EM   80 %
            10 PT   13 PX   3.4 MM  0.8 EM   80 %
            10.5 PT 14 PX   3.6 MM  0.85 EM  85 %
            11 PT   15 PX   3.9 MM  0.95 EM  95 %
            12 PT   16 PX   4.2 MM  1.05 EM 105 %
            12 PT   17 PX   4.2 MM  1.05 EM 105 %
            13 PT   17 PX   4.2 MM  1.1 EM  110 %
            13 PT   18 PX   4.8 MM  1.1 EM  110 %
            14 PT   19 PX   5 MM    1.2 EM  120 %
            15 PT   20 PX   5.4 MM  1.33 EM 133 %
            16 PT   21 PX   5.8 MM  1.4 EM  140 %
            16 PT   22 PX   5.8 MM  1.4 EM  140 %
            17 PT   23 PX   6.2 MM  1.5 EM  150 %
            */

            opts.fMemberDescriptionTextFontSize = 14; // Font 14 znamena 0.14 m v 3D grafike, takze hodnota / 100f
            opts.MemberDescriptionTextColor = System.Windows.Media.Colors.DarkGreen;

            opts.fDimensionTextFontSize = 14;
            opts.DimensionTextColor = System.Windows.Media.Colors.DarkBlue;
            opts.DimensionLineColor = System.Windows.Media.Colors.Black;

            opts.fSawCutTextFontSize = 14;
            opts.SawCutTextColor = System.Windows.Media.Colors.DarkGoldenrod;
            opts.SawCutLineColor = System.Windows.Media.Colors.DarkGoldenrod;

            opts.fControlJointTextFontSize = 14;
            opts.ControlJointTextColor = System.Windows.Media.Colors.DarkMagenta;
            opts.ControlJointLineColor = System.Windows.Media.Colors.DarkMagenta;

            opts.fFoundationTextFontSize = 14;
            opts.FoundationTextColor = System.Windows.Media.Colors.Black;

            opts.fFloorSlabTextFontSize = 14;
            opts.FloorSlabTextColor = System.Windows.Media.Colors.Black;

            List<EViewModelMemberFilters> list_views = new List<EViewModelMemberFilters>()
             { EViewModelMemberFilters.FRONT, EViewModelMemberFilters.BACK, EViewModelMemberFilters.LEFT, EViewModelMemberFilters.RIGHT, EViewModelMemberFilters.ROOF, /*EViewModelMemberFilters.BOTTOM,*/ EViewModelMemberFilters.MIDDLE_FRAME, EViewModelMemberFilters.COLUMNS, EViewModelMemberFilters.FOUNDATIONS, EViewModelMemberFilters.FLOOR};

            int legendImgWidth = 100;
            int legendTextWidth = 60;

            foreach (EViewModelMemberFilters viewMembers in list_views)
            {
                sheetNo++;
                page = s_document.AddPage();
                page.Size = PageSize.A3;
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                gfx = XGraphics.FromPdfPage(page);
                //DrawImage(gfx, ConfigurationManager.AppSettings["logoAndDetails"], 0, (int)page.Height.Point - 80, 320, 75);
                DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
                DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

                DrawTitleBlock(gfx, data.ProjectInfo, ((EPDFPageContentType)viewMembers).GetFriendlyName(), sheetNo, 0);
                contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", ((EPDFPageContentType)viewMembers).GetFriendlyName() });

                opts.ModelView = GetView(viewMembers);
                opts.ViewModelMembers = (int)viewMembers;

                // Defaultne hodnoty pre vsetky pohlady
                opts.bTransformScreenLines3DToCylinders3D = false;  // Do not convert lines (v PDF sa teda nezobrazia)
                opts.wireFrameColor = System.Windows.Media.Colors.Black; // Nastavenie farby wireframe pre export (ina farba ako je v 3D scene)
                opts.fWireFrameLineThickness = 0.015f; // Priemer valca v 3D ktory reprezentuje ciaru // TO Ondrej - Tu by to chcelo vymysliet nejaky mechanizmus, ktory na zaklade rozmerov vykresu a velkosti obrazku modelu urci aky priemer maju mat valce pre ciary aby bola hrubka ciary na vykrese konstantna, vo vysledku maju byt ciary na vykrese cca 0.15 - 0.25 mm hrube

                // Mozeme nastavit pre ktory view chceme kreslit wireframe a konvertovat ciary, farbu a hrubku ciary
                if (viewMembers == EViewModelMemberFilters.COLUMNS)
                {
                    // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                    opts.bDisplayWireFrameModel = true;
                    opts.bDisplayFloorSlabWireFrame = true;
                    opts.bTransformScreenLines3DToCylinders3D = true;
                    //opts.fWireFrameLineThickness = 0.001f; //MAto - tu stoji za uvahu skontrolova/nastavit hrubku pre wireframe

                    opts.bDisplayFoundations = false;
                    opts.bDisplayReinforcementBars = false;
                    opts.bDisplayFloorSlab = true;
                    opts.bDisplayFloorSlabDescription = false;
                }

                // Toto je len pokus ako to vyzera :)
                if (viewMembers == EViewModelMemberFilters.MIDDLE_FRAME)
                {
                    // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                    opts.bDisplayWireFrameModel = true;
                    opts.bTransformScreenLines3DToCylinders3D = true;
                    //opts.fWireFrameLineThickness = 0.001f; //MAto - tu stoji za uvahu skontrolova/nastavit hrubku pre wireframe
                }

                if (viewMembers == EViewModelMemberFilters.FOUNDATIONS)
                {
                    // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                    opts.bDisplayWireFrameModel = true;
                    opts.bDisplayFoundationsWireFrame = true;
                    opts.bDisplayFloorSlabWireFrame = true;
                    opts.bTransformScreenLines3DToCylinders3D = true;
                    //opts.fWireFrameLineThickness = 0.001f; //MAto - tu stoji za uvahu skontrolova/nastavit hrubku pre wireframe

                    opts.bDisplayFoundations = true;
                    opts.bDisplayReinforcementBars = true;
                    opts.bDisplayFloorSlab = true;
                    opts.bDisplayFloorSlabDescription = false;
                    opts.bDisplayFoundationsDescription = true;
                    opts.bDisplayMemberDescription = false;
                }

                if (viewMembers == EViewModelMemberFilters.FLOOR)
                {
                    // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
                    opts.bDisplayWireFrameModel = true;
                    opts.bDisplayFoundationsWireFrame = true;
                    opts.bDisplayFloorSlabWireFrame = true;
                    opts.bTransformScreenLines3DToCylinders3D = true;
                    //opts.fWireFrameLineThickness = 0.001f; //MAto - tu stoji za uvahu skontrolova/nastavit hrubku pre wireframe

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
                }

                CModel filteredModel = null;
                Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data.Model, out filteredModel);
                viewPort.UpdateLayout();

                DrawCrscLegend(gfx, filteredModel, (int)page.Width.Point - legendImgWidth, legendTextWidth);

                XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
                gfx.DrawString($"{(viewMembers).ToString()}:", fontBold, XBrushes.Black, 20, 20);

                XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort, scale));

                double scaleFactor = (gfx.PageSize.Width - legendImgWidth - legendTextWidth) / image.PointWidth;
                double scaledImageWidth = gfx.PageSize.Width - legendImgWidth - legendTextWidth;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                gfx.DrawImage(image, 0, 0, scaledImageWidth, scaledImageHeight);
                image.Dispose();
                viewPort = null;
                gfx.Dispose();
            }
        }

        private static void DrawJointTypes(PdfDocument s_document, CModelData data)
        {
            XGraphics gfx;
            PdfPage page;
            double scale = 1;
            DisplayOptions opts = data.DisplayOptions; // Display properties pre export do PDF - TO Ondrej - mohla by to byt samostatna sada nastaveni nezavisla na 3D scene
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

            // Do dokumentu exporujeme aj s wireframe
            opts.bDisplayWireFrameModel = true; //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            opts.fWireFrameLineThickness = 2;
            opts.bTransformScreenLines3DToCylinders3D = false;
            opts.bDisplayJointsWireFrame = true;
            opts.bDisplayPlatesWireFrame = true;
            opts.bDisplayConnectorsWireFrame = false;
            opts.wireFrameColor = System.Windows.Media.Colors.Black;

            sheetNo++;
            AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Joints.GetFriendlyName());
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Joints.GetFriendlyName() });

            //XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
            //gfx.DrawString("JDetails - Joints:", fontBold, XBrushes.Black, 20, 20);

            XFont font = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);            

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

            foreach (KeyValuePair<CConnectionDescription, CConnectionJointTypes> kvp in data.JointsDict)
            {
                //add new page when whole page is used
                if (numInColumn == maxInColumn)
                {
                    numInColumn = 0;
                    moveY = 40;
                    gfx.Dispose();

                    sheetNo++;
                    AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Joints.GetFriendlyName());
                    contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Joints.GetFriendlyName() });
                    tf = new XTextFormatter(gfx);
                }

                numInRow++;
                CConnectionJointTypes joint = kvp.Value;

                Viewport3D viewPort = ExportHelper.GetJointViewPort(joint, opts, data.Model);
                foreach (Visual3D obj3D in viewPort.Children)
                {
                    if(obj3D is ScreenSpaceLines3D) ((ScreenSpaceLines3D)obj3D).Rescale();  //the only way to draw line in 3D perspective, offline viewport
                }
                viewPort.UpdateLayout();

                XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort, scale));

                viewPort.Children.Clear();
                viewPort = null;

                double scaleFactor = (gfx.PageSize.Width) / (image.PointWidth) / maxInRow;
                double scaledImageWidth = gfx.PageSize.Width / maxInRow;
                double scaledImageHeight = image.PointHeight * scaleFactor;

                tf.DrawString($"{kvp.Key.Name} [{kvp.Key.JoinType}]", font, XBrushes.Black, new Rect(moveX + 10, moveY - 25, scaledImageWidth - 20, scaledImageHeight), format);
                gfx.DrawImage(image, moveX, moveY, scaledImageWidth, scaledImageHeight);
                image.Dispose();

                DrawJointTableToDocument(gfx, moveX, moveY + scaledImageHeight + 4, joint);

                moveX += scaledImageWidth;
                
                if (numInRow == maxInRow) { numInRow = 0; moveX = 5; moveY += scaledImageHeight + 130; numInColumn++; }
            }
            
            gfx.Dispose();
        }

        private static void DrawFootingTypes(PdfDocument s_document, CModelData data)
        {
            XGraphics gfx;
            PdfPage page;
            double scale = 1;
            DisplayOptions opts = data.DisplayOptions; // Display properties pre export do PDF - TO Ondrej - mohla by to byt samostatna sada nastaveni nezavisla na 3D scene
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

            // Do dokumentu exporujeme aj s wireframe
            opts.bDisplayWireFrameModel = true; //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            opts.fWireFrameLineThickness = 2;
            opts.bTransformScreenLines3DToCylinders3D = false;
            opts.bDisplayJointsWireFrame = true;
            opts.bDisplayPlatesWireFrame = true;
            opts.bDisplayConnectorsWireFrame = false;
            opts.wireFrameColor = System.Windows.Media.Colors.Black;

            // Foundations
            opts.bDisplayFoundations = true;
            opts.bDisplayReinforcementBars = true;
            opts.bDisplayFoundationsWireFrame = true;
            opts.bDisplayReinforcementBarsWireFrame = true;
            opts.RotateModelX = -80;
            opts.RotateModelY = 45;
            opts.RotateModelZ = 5;

            sheetNo++;
            AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Footing_Pads.GetFriendlyName());
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Footing_Pads.GetFriendlyName() });

            //XFont fontBold = new XFont(fontFamily, fontSizeTitle, XFontStyle.Bold, options);
            //gfx.DrawString("Footing Pads:", fontBold, XBrushes.Black, 20, 20);

            XFont font = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);

            double moveX = -50;
            double moveY = 40;
            int maxInRow = 2;
            int maxInColumn = 2;
            int numInRow = 0;
            int numInColumn = 0;
            //var tf = new XTextFormatter(gfx);
            //XStringFormat format = new XStringFormat();
            //format.LineAlignment = XLineAlignment.Near;
            //format.Alignment = XStringAlignment.Near;

            foreach (KeyValuePair<string, Tuple<CFoundation, CConnectionJointTypes>> kvp in data.FootingsDict)
            {
                //add new page when whole page is used
                if (numInColumn > maxInColumn)
                {
                    numInColumn = 0;
                    moveY = 40;
                    gfx.Dispose();

                    sheetNo++;
                    AddPageToDocument(s_document, data.ProjectInfo, out page, out gfx, EPDFPageContentType.Details_Footing_Pads.GetFriendlyName());
                    contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Footing_Pads.GetFriendlyName() });
                }

                numInRow++;
                CFoundation pad = kvp.Value.Item1;
                CConnectionJointTypes joint = kvp.Value.Item2;

                Viewport3D viewPort = ExportHelper.GetFootingViewPort(joint, pad, opts);
                foreach (Visual3D obj3D in viewPort.Children)
                {
                    if (obj3D is ScreenSpaceLines3D) ((ScreenSpaceLines3D)obj3D).Rescale();  //the only way to draw line in 3D perspective, offline viewport
                }
                viewPort.UpdateLayout();

                XImage image = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort, scale));                
                
                //double scaleFactor = (gfx.PageSize.Width) / image.PointWidth / maxInRow;
                //double scaledImageWidth = gfx.PageSize.Width / maxInRow;
                //double scaledImageHeight = image.PointHeight * scaleFactor;

                double scaleFactor = image.PointWidth / 500;
                double scaledImageWidth = 500;
                double scaledImageHeight = image.PointHeight * scaleFactor;
                
                gfx.DrawString($"{kvp.Key}", font, XBrushes.Black, new Rect(moveX, moveY - 15, scaledImageWidth, scaledImageHeight), XStringFormats.TopCenter);
                gfx.DrawImage(image, moveX, moveY, scaledImageWidth, scaledImageHeight);
                image.Dispose();
                viewPort = null;
                //DrawFootingTableToDocument(gfx, moveX, moveY + scaledImageHeight + 4, pad);
                DrawFootingTableToDocument(gfx, moveX + scaledImageWidth - 100, moveY, pad);

                moveX += scaledImageWidth + 90;
                if (numInRow == maxInRow) { numInRow = 0; moveX = -50; moveY += scaledImageHeight + 80; numInColumn++; }
            }

            gfx.Dispose();
        }

        private static void AddPageToDocument(PdfDocument s_document, CProjectInfo projectInfo, out PdfPage page, out XGraphics gfx, string pageDetails)
        {
            page = s_document.AddPage();
            page.Size = PageSize.A3;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            DrawTitleBlock(gfx, projectInfo, pageDetails, sheetNo, 0);
        }

        private static void DrawStandardDetails(PdfDocument s_document, CModelData data)
        {
            XGraphics gfx;
            PdfPage page;
            page = s_document.AddPage();
            page.Size = PageSize.A3;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            sheetNo++;            
            DrawTitleBlock(gfx, data.ProjectInfo, EPDFPageContentType.Details_Standard_1.GetFriendlyName(), sheetNo, 0);
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
                DrawTitleBlock(gfx2, data.ProjectInfo, EPDFPageContentType.Details_Standard_2.GetFriendlyName(), sheetNo, 0);
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
            }
        }

        private static void DrawFloorDetails(PdfDocument s_document, CModelData data)
        {
            XGraphics gfx;
            PdfPage page;
            page = s_document.AddPage();
            page.Size = PageSize.A3;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            gfx = XGraphics.FromPdfPage(page);

            DrawPDFLogo(gfx, 0, (int)page.Height.Point - 90);
            DrawCopyRightNote(gfx, 400, (int)page.Height.Point - 15);

            sheetNo++;
            
            DrawTitleBlock(gfx, data.ProjectInfo, EPDFPageContentType.Details_Floor.GetFriendlyName(), sheetNo, 0);
            contents.Add(new string[] { $"fs{sheetNo.ToString("D2")}", EPDFPageContentType.Details_Floor.GetFriendlyName() });

            double scale = 0.2; // 20% of original file dimensions in pixels
            double dImagePosition_x = 2;
            double dImagePosition_y = 2;
            double dRowPosition = 0;

            // 1st row
            XImage image = XImage.FromFile(ConfigurationManager.AppSettings["SawCutDetail"]);
            double imageWidthOriginal = image.PixelWidth;
            double imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);

            XFont fontDimension = new XFont(fontFamily, fontSizeNormal, XFontStyle.Regular, options);
            XBrush brushDimension = XBrushes.DarkOrange;

            XFont fontNote = new XFont(fontFamily, fontSizeDetailTable, XFontStyle.Bold, options);
            XBrush brushNote = XBrushes.Black;

            if (data.Model.m_arrSawCuts != null && data.Model.m_arrSawCuts.Count > 0)
            {
                string sCutWidth = (data.Model.m_arrSawCuts[0].CutWidth * 1000).ToString("F0");
                gfx.DrawString(sCutWidth, fontDimension, brushDimension, 115, 17);

                string sCutDepth = (data.Model.m_arrSawCuts[0].CutDepth * 1000).ToString("F0");
                gfx.DrawString(sCutDepth, fontDimension, brushDimension, 60, 40);
            }

            image = XImage.FromFile(ConfigurationManager.AppSettings["ControlJointDetail"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dImagePosition_y, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dImagePosition_y + imageHeightOriginal * scale);

            if (data.Model.m_arrControlJoints != null && data.Model.m_arrControlJoints.Count > 0)
            {
                /*
                string sText = "D"+(data.Model.m_arrControlJoints[0].ReferenceDowel.Diameter_shank*1000).ToString("F0") + " GALVANISED DOWEL"+
                    " ("+ (data.Model.m_arrControlJoints[0].ReferenceDowel.Length * 1000).ToString("F0") + " mm LONG) / "+
                    (data.Model.m_arrControlJoints[0].DowelSpacing * 1000).ToString("F0") + " CENTRES \n (WRAP ONE SIDE WITH DENSO TAPE)";
                */

                string sText1 = "D" + (data.Model.m_arrControlJoints[0].ReferenceDowel.Diameter_shank * 1000).ToString("F0") + " GALVANISED DOWEL";
                string sText2 = "(" + (data.Model.m_arrControlJoints[0].ReferenceDowel.Length * 1000).ToString("F0") + " mm LONG) / " +
                    (data.Model.m_arrControlJoints[0].DowelSpacing * 1000).ToString("F0") + " CENTRES";
                string sText3 = "WRAP ONE SIDE WITH DENSO TAPE";

                gfx.DrawString(sText1, fontNote, brushNote, 315, 125);
                gfx.DrawString(sText2, fontNote, brushNote, 315, 135);
                gfx.DrawString(sText3, fontNote, brushNote, 315, 145);
            }

            // TODO - skontrolovat ci sa dalsi obrazok vojde do sirky stranky, ak nie pridat novy rad (len ak sa vojde na vysku) alebo novu stranku
            // 2nd row
            dImagePosition_x = 2; // Zaciname znova od laveho okraja
            double dRowPosition2 = dRowPosition;

            image = XImage.FromFile(ConfigurationManager.AppSettings["PerimeterDetail"]);
            imageWidthOriginal = image.PixelWidth;
            imageHeightOriginal = image.PixelHeight;
            gfx.DrawImage(image, dImagePosition_x, dRowPosition2, imageWidthOriginal * scale, imageHeightOriginal * scale);
            image.Dispose();
            dImagePosition_x += imageWidthOriginal * scale;
            dRowPosition = Math.Max(dRowPosition, dRowPosition2 + dImagePosition_y + imageHeightOriginal * scale);

            float fPerimeterDepth = 0.55f;
            float fPerimeterBottomWidth = 0.25f;
            float fMeshAndStartersOverlapping = 0.6f;

            CSlab slab = data.Model.m_arrSlabs.FirstOrDefault();
            float fFloorSlabTopCover = slab.ConcreteCover;
            CFoundation f = data.Model.m_arrFoundations.FirstOrDefault(); // ???? Budeme zadavat samostatne pre perimeter, vytvorit objekt perimeter ????
            float fPerimeterCover = f.ConcreteCover;

            float fStarterTopPosition = fFloorSlabTopCover + 0.02f; // Mesh position + 20 mm
            float fMiddleDimension = fPerimeterDepth - fPerimeterCover;

            string sTextP1 = (fPerimeterDepth * 1000).ToString("F0");
            string sTextP2 = (fPerimeterCover * 1000).ToString("F0");
            string sTextP3 = (fMiddleDimension * 1000).ToString("F0");
            string sTextP4 = (fStarterTopPosition * 1000).ToString("F0");

            string sTextP5 = (fPerimeterBottomWidth * 1000).ToString("F0");
            string sTextP6 = (fMeshAndStartersOverlapping * 1000).ToString("F0") + " lap with mesh";

            string sTextP7 = "HD12 Starters";
            string sTextP8 = "600 mm crs";

            // IN WORK 26.8.2019
            // TO ONDREJ - ako otocim text o 90 stupnov ??? aby bol rovnobezne so zvislou kotou???
            // TO Ondrej - zistil som ze to otocim vid nizsie, aj to funguje, ale musim vytvorit novu XGraphics pre kazdy text
            // Rozumiem tomu spravne ze mam vykreslit najprv vsetko, co je horizontalne, potom gfx.Dispose(); a potom vytvorit pre kazdy rotovany text novu XGraphics?

            /*
            gfx.Dispose();
            XGraphics gfxRotate = XGraphics.FromPdfPage(page);
            gfxRotate.RotateAtTransform(-90, new XPoint(200, 300));
            gfxRotate.DrawString("Text Here", fontDimension, XBrushes.Black, new XPoint(200, 300));
            */

            gfx.DrawString(sTextP1, fontDimension, brushDimension, 17, 295);
            gfx.DrawString(sTextP2, fontDimension, brushDimension, 45, 380);
            gfx.DrawString(sTextP3, fontDimension, brushDimension, 43, 295);
            gfx.DrawString(sTextP4, fontDimension, brushDimension, 45, 225);
            gfx.DrawString(sTextP5, fontDimension, brushDimension, 90, 380);
            gfx.DrawString(sTextP6, fontDimension, brushDimension, 100, 210);
            gfx.DrawString(sTextP7, fontNote, brushNote, 180, 290);
            gfx.DrawString(sTextP8, fontNote, brushNote, 180, 300);

            if (data.DoorBlocksProperties != null && data.DoorBlocksProperties.Count > 0) // Some door exists
            {
                bool bAddRollerDoorDetail = false;
                foreach (DoorProperties prop in data.DoorBlocksProperties)
                {
                    if (prop.sDoorType == "Roller Door")
                        bAddRollerDoorDetail = true;
                }

                if (bAddRollerDoorDetail) // Add roller door rebate detail
                {
                    int iPictureTextOffset = 240;

                    image = XImage.FromFile(ConfigurationManager.AppSettings["RollerDoorRebateDetail"]);
                    imageWidthOriginal = image.PixelWidth;
                    imageHeightOriginal = image.PixelHeight;
                    gfx.DrawImage(image, dImagePosition_x, dRowPosition2, imageWidthOriginal * scale, imageHeightOriginal * scale);
                    image.Dispose();
                    dImagePosition_x += imageWidthOriginal * scale;
                    dRowPosition = Math.Max(dRowPosition, dRowPosition2 + dImagePosition_y + imageHeightOriginal * scale);

                    float fPerimeterDepthRebate = fPerimeterDepth - 0.02f; // 10 + 10 mm
                    sTextP1 = (fPerimeterDepthRebate * 1000).ToString("F0");

                    float fRollerDoorRebate = 0.5f;
                    sTextP6 = (fRollerDoorRebate * 1000).ToString("F0");

                    gfx.DrawString(sTextP1, fontDimension, brushDimension, iPictureTextOffset + 17, 295);
                    gfx.DrawString(sTextP2, fontDimension, brushDimension, iPictureTextOffset + 45, 380);
                    //gfx.DrawString(sTextP3, fontDimension, brushDimension, iPictureTextOffset + 43, 295);
                    //gfx.DrawString(sTextP4, fontDimension, brushDimension, iPictureTextOffset + 45, 225);
                    gfx.DrawString(sTextP5, fontDimension, brushDimension, iPictureTextOffset + 90, 380);
                    gfx.DrawString(sTextP6, fontDimension, brushDimension, iPictureTextOffset + 130, 210);
                    //gfx.DrawString(sTextP7, fontNote, brushNote, iPictureTextOffset + 180, 290);
                }
            }
        }

        private static XGraphics DrawTitlePage(PdfDocument s_document, CProjectInfo pInfo, CModelData data) // TODO Ondrej - Titulna stranka s dynamickou tabulkou, v ktorej je zoznam vykresov (mozno sa musi vlozit az uplne posledna, podobne ako vkladame obsah
        {
            PdfPage page = s_document.AddPage();
            page.Size = PageSize.A3;
            page.Orientation = PdfSharp.PageOrientation.Landscape;
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

            //gfx.DrawString("TITLE PAGE", font, XBrushes.Black, 500, 400);

            // Do stredu by sa mozno mohol vlozit malicky preview isometricky pohlad na konstrukciu, aby to nebolo take prazdne :)
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
            opts.fWireFrameLineThickness = 0.001f;

            opts.bDisplayMembers = true;
            opts.bDisplayJoints = true;
            opts.bDisplayPlates = true;

            opts.bDisplayFoundations = false;
            opts.bDisplayFloorSlab = false;
            opts.bDisplaySawCuts = false;
            opts.bDisplayControlJoints = false;

            opts.bDisplayFoundationsDescription = false;
            opts.bDisplayFloorSlabDescription = false;
            opts.bDisplaySawCutsDescription = false;
            opts.bDisplayControlJointsDescription = false;

            CModel filteredModel = null;
            Viewport3D viewPort = ExportHelper.GetBaseModelViewPort(opts, data.Model, out filteredModel);
            viewPort.UpdateLayout();

            XImage imageModel = XImage.FromBitmapSource(ExportHelper.RenderVisual(viewPort));

            double scaleFactor = (gfx.PageSize.Width / 2) / imageModel.PointWidth;
            double scaledImageWidth = gfx.PageSize.Width / 2;
            double scaledImageHeight = imageModel.PointHeight * scaleFactor;

            gfx.DrawImage(imageModel, gfx.PageSize.Width / 4, gfx.PageSize.Height / 4 - 100, scaledImageWidth, scaledImageHeight);
            imageModel.Dispose();

            

            // Logo
            XImage image = XImage.FromFile(ConfigurationManager.AppSettings["logo2"]);
            gfx.DrawImage(image, gfx.PageSize.Width - 240 - 50, 630, 240, 75);
            image.Dispose();
            viewPort = null;

            gfx.DrawString("TO BE READ IN CONJUCTION WITH", fontBold, XBrushes.Black, 900, 730);
            gfx.DrawString("ARCHITECTURAL PLAN SET", fontBold, XBrushes.Black, 947, 750);
            gfx.DrawString("ENGINEERING PLAN SET", fontBoltTitle, XBrushes.Black, 530, 800);
            return gfx;
        }

        // TO Ondrej - ma zmysel mat tieto vnorene metody ak maju rovnake parametre, neviem ci je opodstatnene - ja som to urobil len aby malo vsetko nazov Draw
        private static void DrawTable(XGraphics gfx, int x, int y, List<string[]> tableParams)
        {
            AddTableToDocument(gfx, x, y, tableParams);
        }

        // TO Ondrej - ma zmysel mat tieto vnorene metody ak maju rovnake parametre, neviem ci je opodstatnene - ja som to urobil len aby malo vsetko nazov Draw
        private static void DrawTitleBlock(XGraphics gfx, CProjectInfo pInfo, string contents, int sheetNo, int issue) // TODO Ondrej - Tabulka s rozpiskou
        {
            // TODO - Onddrej - sem treba vykreslit tabulku podla vzoru co som Ti poslal (nemusis vsetko, len zhruba :) aby som si to vedel podoplnat)
            // Velkost pisma mozes nastavit tak, aby bolo zhruba 2.5-3 mm velke, aby nam ta tabulka nezaberal prilis vela miesta, nazov projektu moze byt 5 mm pismom

            AddPageTitleBlockTableToDocument(gfx, pInfo, contents, sheetNo, issue);
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

        private static void DrawCrscLegend(XGraphics gfx, CModel model, int x, int textWidth)
        {
            int width = 100;
            int height = 76;
            int y = 20;
            int font_y = 20;

            XFont font = new XFont(fontFamily, fontSizeLegend, XFontStyle.Regular, options);
            List<string> list_crsc = GetCrscFromModel(model);

            foreach (string crsc in list_crsc)
            {
                DrawImage(gfx, ConfigurationManager.AppSettings[crsc], x, y, width, height);

                // List of member types
                List<string> list_memberTypes = GetMemberTypesWithCrscFromModel(model, crsc);
                font_y = 20;
                foreach (string s in list_memberTypes)
                {
                    gfx.DrawString($"[{s}]", font, XBrushes.Black, x - textWidth, y + font_y);
                    font_y += 15;
                }

                // Cross-section name
                gfx.DrawString($"{crsc}", font, XBrushes.Black, x - textWidth, y + font_y); // cross-section name
                font_y += 15;

                // TEK screws number, gauge and distance - TO Ondrej - mozem to nacitavat tu z databazy znova alebo je lepsie dostat sem nie len crsc string ale cely objekt a necitat to z neho
                DATABASE.DTO.CrScProperties crscProp = DATABASE.CSectionManager.GetSectionProperties(crsc); // Load cross-section properties

                if (crscProp.IsBuiltUp == true)
                {
                    string sScrewsDescrtiption = crscProp.iScrewsNumber + "/" + crscProp.iScrewsGauge + "g" + " teks@" + (crscProp.dScrewDistance * 1000).ToString("F0") + "c/c";
                    gfx.DrawString(sScrewsDescrtiption, font, XBrushes.Black, x - textWidth, y + font_y); // built-up cross-section number of screws and distance
                }

                y += height;
            }
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
            gfx.DrawString(data.GableWidth.ToString(), font, XBrushes.Black, offsetX3, 60);
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
            para.AddFormattedText(data.GableWidth.ToString());
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

        private static void AddPageTitleBlockTableToDocument(XGraphics gfx, CProjectInfo projectInfo, string contents, int sheetNo, int issue)
        {
            gfx.MUH = PdfFontEncoding.Unicode;
            //gfx.MFEH = PdfFontEmbedding.Always;

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Table t = GetPageTitleBlockTable(doc, projectInfo, contents, sheetNo, issue);

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

        private static Table GetPageTitleBlockTable(Document document, CProjectInfo projectInfo, string contents, int sheetNo, int issue)
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
            cell.AddParagraph("420x297 - A3");

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
            if(joint.m_MainMember != null)
                tableParams.Add(new string[2] { "Main Member", joint.m_MainMember.Prefix + " - " + joint.m_MainMember.EMemberTypePosition.GetFriendlyName() });
            if (joint.m_SecondaryMembers != null && joint.m_SecondaryMembers.Length > 0)
                tableParams.Add(new string[2] { "Secondary Member", joint.m_SecondaryMembers[0].Prefix + " - " + joint.m_SecondaryMembers[0].EMemberTypePosition.GetFriendlyName() });
            //if (joint.m_arrPlates != null)
            //    tableParams.Add(new string[2] { "Plates count", joint.m_arrPlates.Length.ToString() });
            if (joint.m_arrPlates != null && joint.m_arrPlates.Length > 0)
            {
                //tableParams.Add(new string[2] { "Plate name", joint.m_arrPlates.FirstOrDefault().Name.ToString() });
                //tableParams.Add(new string[2] { "Plate thickness", (joint.m_arrPlates.FirstOrDefault().Ft * 1000).ToString() + " [mm]" });
                tableParams.Add(new string[2] { "Plates ", joint.m_arrPlates.Length.ToString() + " x " + joint.m_arrPlates.FirstOrDefault().Name.ToString() + " - " + "thickness " + (joint.m_arrPlates.FirstOrDefault().Ft * 1000).ToString() + " [mm]" });
                //tableParams.Add(new string[2] { "Screws count in plate", joint.m_arrPlates.FirstOrDefault().ScrewArrangement.IHolesNumber.ToString() });
                //tableParams.Add(new string[2] { "Screw", "TEK " + (joint.m_arrPlates.FirstOrDefault().ScrewArrangement.referenceScrew.Gauge +"g").ToString() });
                tableParams.Add(new string[2] { "Screws", joint.m_arrPlates.FirstOrDefault().ScrewArrangement.IHolesNumber.ToString() + " x " + "TEKs " + (joint.m_arrPlates.FirstOrDefault().ScrewArrangement.referenceScrew.Gauge + "g").ToString() });

                if (joint.m_arrPlates.FirstOrDefault().ScrewArrangement is CScrewArrangementCircleApexOrKnee) // Knee or apex with circle screw arrangement
                {
                    CScrewArrangementCircleApexOrKnee circleArrangement = (CScrewArrangementCircleApexOrKnee)joint.m_arrPlates.FirstOrDefault().ScrewArrangement;

                    tableParams.Add(new string[2] { "Number of circles", circleArrangement.INumberOfCirclesInGroup.ToString() });

                    if (circleArrangement.ListOfSequenceGroups[0].ListSequence[0] is CScrewHalfCircleSequence)
                    {
                        CScrewHalfCircleSequence seq = (CScrewHalfCircleSequence)circleArrangement.ListOfSequenceGroups[0].ListSequence[0];
                        tableParams.Add(new string[2] { "Radius", (seq.Radius * 1000).ToString("F0") + " [mm]"  });
                    }
                }

                if(joint.m_arrPlates.FirstOrDefault() is CConCom_Plate_B_basic) // Base plate
                {
                    CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)joint.m_arrPlates.FirstOrDefault();
                    CAnchorArrangement anchorArrangement = plate.AnchorArrangement;

                    //tableParams.Add(new string[2] { "Number of anchors", anchorArrangement.IHolesNumber.ToString() });
                    //tableParams.Add(new string[2] { "Anchor", "M"+(anchorArrangement.referenceAnchor.Diameter_shank*1000).ToString() + " HD bolts - " + (anchorArrangement.referenceAnchor.Length * 1000).ToString() + " [mm]" });
                    tableParams.Add(new string[2] { "Anchors", anchorArrangement.IHolesNumber.ToString() + " x " + "M" + (anchorArrangement.referenceAnchor.Diameter_shank * 1000).ToString() + " HD bolts - " + (anchorArrangement.referenceAnchor.Length * 1000).ToString() + " [mm]" });
                    tableParams.Add(new string[2] { "Note", "Drypack between plate and floor to suit" });
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

            //tableParams.Add(new string[2] { "Count Bottom Bars x", pad.Count_Bottom_Bars_x.ToString() });
            //tableParams.Add(new string[2] { "Count Bottom Bars y", pad.Count_Bottom_Bars_y.ToString() });
            //tableParams.Add(new string[2] { "Count Top Bars x", pad.Count_Top_Bars_x.ToString() });
            //tableParams.Add(new string[2] { "Count Top Bars y", pad.Count_Top_Bars_y.ToString() });
            tableParams.Add(new string[2] { "Bottom Bars x", pad.Count_Bottom_Bars_x.ToString() + " x " + " HD" + (pad.Reference_Bottom_Bar_x.Diameter * 1000).ToString("F0") });
            tableParams.Add(new string[2] { "Bottom Bars y", pad.Count_Bottom_Bars_y.ToString() + " x " + " HD" + (pad.Reference_Bottom_Bar_y.Diameter * 1000).ToString("F0") });
            tableParams.Add(new string[2] { "Top Bars x", pad.Count_Top_Bars_x.ToString() + " x " + " HD" + (pad.Reference_Top_Bar_x.Diameter * 1000).ToString("F0") });
            tableParams.Add(new string[2] { "Top Bars y", pad.Count_Top_Bars_y.ToString() + " x " + " HD" + (pad.Reference_Top_Bar_y.Diameter * 1000).ToString("F0") });

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
