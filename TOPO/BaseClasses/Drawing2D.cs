using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using MATH;
using CRSC;
using System.Globalization;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using System.IO;
using System.Windows.Media.Imaging;
using Petzold.Media2D;

namespace BaseClasses
{
    public static class Drawing2D
    {
        public static void DrawCrscToCanvas(CCrSc crsc, double width, double height, ref Canvas canvasForImage,
            bool bDrawPoints, bool bDrawOutLine, bool bDrawPointNumbers, bool bDrawDimensions)
        {
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
            double dPointInOutDistance_x_real = 0;
            double dPointInOutDistance_y_real = 0;

            // Fill arrays of points
            if (crsc.CrScPointsOut != null && crsc.CrScPointsOut.Count > 1)
            {
                CalculateModelLimits(crsc.CrScPointsOut, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            }

            if (crsc.CrScPointsIn != null && crsc.CrScPointsIn.Count > 1)
            {
                double fTempMax_X_IN = 0, fTempMin_X_IN = 0, fTempMax_Y_IN = 0, fTempMin_Y_IN = 0;

                CalculateModelLimits(crsc.CrScPointsIn, out fTempMax_X_IN, out fTempMin_X_IN, out fTempMax_Y_IN, out fTempMin_Y_IN);

                dPointInOutDistance_x_real = fTempMax_X - fTempMax_X_IN;
                dPointInOutDistance_y_real = fTempMax_Y - fTempMax_Y_IN;
            }

            int scale_unit = 1000; // mm

            double fModel_Length_x_real;
            double fModel_Length_y_real;
            double fModel_Length_x_page;
            double fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            double dReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginTop_y;
            double dPointInOutDistance_x_page;
            double dPointInOutDistance_y_page;

            CalculateBasicValue(
                    fTempMax_X,
                    fTempMin_X,
                    fTempMax_Y,
                    fTempMin_Y,
                    0.8f,
                    scale_unit,
                    width,
                    height,
                    crsc.CrScPointsIn,
                    dPointInOutDistance_x_real,
                    dPointInOutDistance_y_real,
                    out fModel_Length_x_real,
                    out fModel_Length_y_real,
                    out fModel_Length_x_page,
                    out fModel_Length_y_page,
                    out dFactor_x,
                    out dFactor_y,
                    out dReal_Model_Zoom_Factor,
                    out fmodelMarginLeft_x,
                    out fmodelMarginTop_y,
                    out dPointInOutDistance_x_page,
                    out dPointInOutDistance_y_page);

            canvasForImage.Children.Clear();
            if (crsc != null)
                DrawComponent(
                     bDrawPoints,
                     bDrawOutLine,
                     bDrawPointNumbers,
                     false,
                     false,
                     false,
                     bDrawDimensions,
                     false,
                     false,
                     crsc.CrScPointsOut,
                     crsc.CrScPointsIn,
                     null,
                     null,
                     null,
                     null, // TODO - dodefinovat koty i pre prierezy
                     null,
                     null,
                     null,
                     0,
                     0,
                     fmodelMarginLeft_x,
                     fmodelMarginTop_y,
                     dReal_Model_Zoom_Factor,
                     fModel_Length_y_page,
                     dPointInOutDistance_y_page,
                     dPointInOutDistance_x_page,
                     true,
                     canvasForImage);
        }

        public static void DrawPlateToCanvas(CPlate plate,
            double width,
            double height,
            ref Canvas canvasForImage,
            bool bDrawPoints,
            bool bDrawOutLine,
            bool bDrawPointNumbers,
            bool bDrawHoles,
            bool bDrawHoleCentreSymbols,
            bool bDrawDrillingRoute,
            bool bDrawDimensions,
            bool bDrawMemberOutline,
            bool bDrawBendLines)
        {
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;

            // Fill arrays of points
            if (plate.PointsOut2D != null && plate.PointsOut2D.Length > 1)
            {
                CalculateModelLimits(plate.PointsOut2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            }

            int scale_unit = 1000; // mm

            double fModel_Length_x_real;
            double fModel_Length_y_real;
            double fModel_Length_x_page;
            double fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            double dReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginBottom_y;
            double dPointInOutDistance_x_page;
            double dPointInOutDistance_y_page;

            CalculateBasicValue(
                    fTempMax_X,
                    fTempMin_X,
                    fTempMax_Y,
                    fTempMin_Y,
                    0.8f,
                    scale_unit,
                    width,
                    height,
                    null,
                    0,
                    0,
                    out fModel_Length_x_real,
                    out fModel_Length_y_real,
                    out fModel_Length_x_page,
                    out fModel_Length_y_page,
                    out dFactor_x,
                    out dFactor_y,
                    out dReal_Model_Zoom_Factor,
                    out fmodelMarginLeft_x,
                    out fmodelMarginBottom_y,
                    out dPointInOutDistance_x_page,
                    out dPointInOutDistance_y_page);

            // Holes center points
            Point[] pHolesCentersPointsScrews2D = null;

            // Check that object of screw arrangement is not null and set array items to the temporary array
            if (plate.ScrewArrangement != null && plate.ScrewArrangement.HolesCentersPoints2D != null)
                pHolesCentersPointsScrews2D = plate.ScrewArrangement.HolesCentersPoints2D;

            Point[] pHolesCentersPointsAnchors2D = null;

            // Check that object of screw arrangement is not null and set array items to the temporary array
            if (plate is CConCom_Plate_B_basic) // Ak je plech typu base plate "B" mozu sa vykreslovat objekty typu anchors
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
                if (basePlate.AnchorArrangement != null && basePlate.AnchorArrangement.HolesCentersPoints2D != null)
                    pHolesCentersPointsAnchors2D = basePlate.AnchorArrangement.HolesCentersPoints2D;
            }

            float fDiameter_screwPreDrilledHole = 0;
            float fDiameter_anchorPreDrilledHole = 0;

            // Holes diameters
            if (plate.ScrewArrangement != null && plate.ScrewArrangement.referenceScrew != null)
                fDiameter_screwPreDrilledHole = plate.ScrewArrangement.referenceScrew.D_holediameter;

            if (plate is CConCom_Plate_B_basic) // Ak je plech typu base plate "B" mozu sa vykreslovat objekty typu anchors alebo screws
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
                if (basePlate.AnchorArrangement != null && basePlate.AnchorArrangement.referenceAnchor != null)
                    fDiameter_anchorPreDrilledHole = basePlate.AnchorArrangement.referenceAnchor.Diameter_thread; // TODO - Doplnit do databazy velkost otvorov pre bolts a anchors

            }

            canvasForImage.Children.Clear();
            if (plate != null)
            {
                CNote2D note2D = GetNoteForPlate(plate);

                DrawComponent(
                    bDrawPoints,
                    bDrawOutLine,
                    bDrawPointNumbers,
                    bDrawHoles,
                    bDrawHoleCentreSymbols,
                    bDrawDrillingRoute,
                    bDrawDimensions,
                    bDrawMemberOutline,
                    bDrawBendLines,
                    Geom2D.TransformArrayToList(plate.PointsOut2D),
                    null,
                    pHolesCentersPointsScrews2D,
                    pHolesCentersPointsAnchors2D,
                    plate.DrillingRoutePoints,
                    plate.Dimensions,
                    plate.MemberOutlines,
                    plate.BendLines,
                    note2D,
                    fDiameter_screwPreDrilledHole * scale_unit,
                    fDiameter_anchorPreDrilledHole * scale_unit,
                    fmodelMarginLeft_x,
                    fmodelMarginBottom_y,
                    dReal_Model_Zoom_Factor,
                    fModel_Length_y_page,
                    dPointInOutDistance_y_page,
                    dPointInOutDistance_x_page,
                    true,
                    canvasForImage);
            }
        }

        public static Canvas DrawRealPlateToCanvas(CPlate plate,
            bool bDrawPoints,
            bool bDrawOutLine,
            bool bDrawPointNumbers,
            bool bDrawHoles,
            bool bDrawHoleCentreSymbols,
            bool bDrawDrillingRoute,
            bool bDrawDimensions,
            bool bDrawMemberOutline,
            bool bDrawBendLines)
        {
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;

            // Fill arrays of points
            if (plate.PointsOut2D != null && plate.PointsOut2D.Length > 1)
            {
                CalculateModelLimits(plate.PointsOut2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            }

            int scale_unit = 1000; // mm

            double dModel_Length_x_real = (fTempMax_X - fTempMin_X) * scale_unit;
            double dModel_Length_y_real = (fTempMax_Y - fTempMin_Y) * scale_unit;

            // Holes center points
            Point[] pHolesCentersPointsScrews2D = null;

            // Check that object of screw arrangement is not null and set array items to the temporary array
            if (plate.ScrewArrangement != null && plate.ScrewArrangement.HolesCentersPoints2D != null)
                pHolesCentersPointsScrews2D = plate.ScrewArrangement.HolesCentersPoints2D;

            Point[] pHolesCentersPointsAnchors2D = null;

            // Check that object of screw arrangement is not null and set array items to the temporary array
            if (plate is CConCom_Plate_B_basic) // Ak je plech typu base plate "B" mozu sa vykreslovat objekty typu anchors
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
                if (basePlate.AnchorArrangement != null && basePlate.AnchorArrangement.HolesCentersPoints2D != null)
                    pHolesCentersPointsAnchors2D = basePlate.AnchorArrangement.HolesCentersPoints2D;
            }

            float fDiameter_screwPreDrilledHoles = 0;
            float fDiameter_anchorPreDrilledHoles = 0;

            // Holes diameters
            if (plate.ScrewArrangement != null && plate.ScrewArrangement.referenceScrew != null)
                fDiameter_screwPreDrilledHoles = plate.ScrewArrangement.referenceScrew.D_holediameter;

            if (plate is CConCom_Plate_B_basic) // Ak je plech typu base plate "B" mozu sa vykreslovat objekty typu anchors alebo screws
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
                if (basePlate.AnchorArrangement != null && basePlate.AnchorArrangement.referenceAnchor != null)
                    fDiameter_anchorPreDrilledHoles = basePlate.AnchorArrangement.referenceAnchor.Diameter_thread; // TODO - doplnit do databazy priemer otvorov pre bolts a anchors
            }

            Canvas canvasForImage = new Canvas();
            canvasForImage.RenderSize = new Size(dModel_Length_x_real, dModel_Length_y_real);

            if (plate != null)
            {
                //CNote2D note2D = GetNoteForPlate(plate);

                List<Point> canvasPointsOut = null;
                List<Point> canvasPointsOut_Mirror = null;
                List<Point> canvasPointsIn = null;
                List<Point> canvasPointsHolesScrews = null;
                List<Point> canvasPointsHolesAnchors = null;
                List<Point> canvasPointsDrillingRoute = null;
                List<CDimension> canvasDimensions = null;
                List<CLine2D> canvasMemberOutline = null;
                List<CLine2D> canvasBendLines = null;
                //CNote2D canvasNote2D = null;

                bool bPointsHaveYinUpDirection = false;
                if (bPointsHaveYinUpDirection)
                {
                    canvasPointsOut = Geom2D.MirrorAboutX_ChangeYCoordinates(plate.PointsOut2D);
                    //canvasPointsIn = Geom2D.MirrorAboutX_ChangeYCoordinates(null);
                    canvasPointsHolesScrews = Geom2D.MirrorAboutX_ChangeYCoordinates(pHolesCentersPointsScrews2D);
                    canvasPointsHolesAnchors = Geom2D.MirrorAboutX_ChangeYCoordinates(pHolesCentersPointsAnchors2D);
                    canvasPointsDrillingRoute = Geom2D.MirrorAboutX_ChangeYCoordinates(plate.DrillingRoutePoints);
                    canvasDimensions = MirrorAboutX_ChangeYCoordinates(plate.Dimensions, false);
                    canvasMemberOutline = MirrorAboutX_ChangeYCoordinates(plate.MemberOutlines, false);
                    canvasBendLines = MirrorAboutX_ChangeYCoordinates(plate.BendLines, false);
                    //if (note2D != null) note2D.MirrorYCoordinates();
                }
                else
                {
                    if (plate.PointsOut2D != null) canvasPointsOut = new List<Point>(plate.PointsOut2D);
                    canvasPointsOut_Mirror = Geom2D.MirrorAboutX_ChangeYCoordinates(plate.PointsOut2D);
                    //canvasPointsIn = new List<Point>(PointsIn);
                    if (pHolesCentersPointsScrews2D != null) canvasPointsHolesScrews = new List<Point>(pHolesCentersPointsScrews2D);
                    if (pHolesCentersPointsAnchors2D != null) canvasPointsHolesAnchors = new List<Point>(pHolesCentersPointsAnchors2D);
                    if (plate.DrillingRoutePoints != null) canvasPointsDrillingRoute = new List<Point>(plate.DrillingRoutePoints);

                    // Oprava prepisu dat pre plate ak spustim export opakovane                
                    canvasDimensions = ModelHelper.GetClonedDimensions(plate.Dimensions);
                    canvasMemberOutline = ModelHelper.GetClonedLines(plate.MemberOutlines);
                    canvasBendLines = ModelHelper.GetClonedLines(plate.BendLines);
                }

                double minX = canvasPointsOut.Min(p => p.X);
                double minY = canvasPointsOut.Min(p => p.Y);

                float fmodelMarginLeft_x = 0;
                float fmodelMarginTop_y = 0;
                float dReal_Model_Zoom_Factor = 1;
                canvasPointsOut = ConvertRealPointsToCanvasDrawingPoints(canvasPointsOut, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasPointsOut_Mirror = ConvertRealPointsToCanvasDrawingPoints(canvasPointsOut_Mirror, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasPointsIn = ConvertRealPointsToCanvasDrawingPoints(canvasPointsIn, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasPointsHolesScrews = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHolesScrews, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasPointsHolesAnchors = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHolesAnchors, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasPointsDrillingRoute = ConvertRealPointsToCanvasDrawingPoints(canvasPointsDrillingRoute, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);

                canvasDimensions = ConvertRealPointsToCanvasDrawingPoints(canvasDimensions, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasMemberOutline = ConvertRealPointsToCanvasDrawingPoints(canvasMemberOutline, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                canvasBendLines = ConvertRealPointsToCanvasDrawingPoints(canvasBendLines, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);
                //canvasNote2D = ConvertRealPointsToCanvasDrawingPoints(note2D, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor * scale_unit);

                // Definition Points
                //vobec nechapem preco treba spravit mirror pre samotne body, ale body ktore sa pouziju pre nakreslenie ciar uz mirorovane netreba
                DrawComponentPoints(bDrawPoints, canvasPointsOut_Mirror, canvasPointsIn, canvasForImage, "Points");

                // Outlines
                DrawOutlines(bDrawOutLine, canvasPointsOut, canvasPointsIn, canvasForImage, "Outlines");

                // Definition Point Numbers
                DrawPointNumbers(bDrawPointNumbers, canvasPointsOut, canvasPointsIn, canvasForImage, "Point numbers");

                // Holes
                if (pHolesCentersPointsScrews2D != null)
                {
                    DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesScrews, Brushes.Black, Brushes.Red, 1, 1, fDiameter_screwPreDrilledHoles * scale_unit, canvasForImage, "Holes");
                    DrawDrillingRoute(bDrawDrillingRoute, canvasPointsDrillingRoute, canvasForImage, "Drilling route");
                }

                if (pHolesCentersPointsAnchors2D != null)
                {
                    DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesAnchors, Brushes.Black, Brushes.Red, 1, 1, fDiameter_anchorPreDrilledHoles * scale_unit, canvasForImage, "Holes");
                }

                // Dimensions
                DrawDimensions(bDrawDimensions, canvasDimensions, canvasForImage, Brushes.DarkGreen, Brushes.DarkGreen, 1, "Dimensions");

                // Member Outline
                DrawSeparateLines(bDrawMemberOutline, canvasMemberOutline, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, "Member outline");
                
                // Bend Lines
                DrawSeparateLines(bDrawBendLines, canvasBendLines, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, "Bend lines");
            }

            canvasForImage.UpdateLayout();
            RenderVisual(canvasForImage);

            //using (Stream stream = GetCanvasStream(canvasForImage))
            //{
            //    using (var fileStream = File.Create("canvas.png"))
            //    {
            //        stream.Seek(0, SeekOrigin.Begin);
            //        stream.CopyTo(fileStream);
            //    }
            //}

            return canvasForImage;
        }

        public static void DrawFootingPadSideElevationToCanvas(CFoundation pad,
            CConnectionJointTypes joint,
            CSlab floorSlab,
            double width,
            double height,
            ref Canvas canvasForImage,
            DisplayOptionsFootingPad2D opts)
        {
            // TODO Ondrej

            // Tuto funckiu potrebujem zrefaktorovat a precistit tak aby bol system vykreslovania podobny ako je pre DrawPlateToCanvas

            // 1. Potrebujem zjednotit tento system podla toho co je lepsie
            // - najprv pripravit vsetky realne suradnice, potom ich previest na canvas units a potom kreslit objekty // DrawPlateToCanvas
            // - pripravit realne suradnice pre dany bool (typ objektov ktore kreslime) , previest na canvas units, kreslit objekty a potom pokracovat pre dalsi bool // DrawFootingPadSideElevationToCanvas

            // 3. Vypocitat scalovaci faktor fReal_Model_Zoom_Factor z rozmerov canvas a toho co sa kresli - vid  DrawPlateToCanvas
            // Nieco som tu uz nadhodil

            CConCom_Plate_B_basic basePlate = null;

            double crscDepth = 0; // zobrazovana sirka prierezu
            double horizontalOffsetColumn = 0; //
            float fVerticalOffsetLeft = 0; // hrana pruta nad plechom spoja vlavo
            float fVerticalOffsetRight = 0; // hrana pruta na plechom spoja vpravo
            float fTopLineSlope_rad = 0; // Sklon pomocnej ciary ktora ukoncuje stlp

            Point bottomLeft_ColumnEdge = new Point();
            Point topLeft_ColumnEdge = new Point();
            Point bottomRight_ColumnEdge = new Point();
            Point topRight_ColumnEdge = new Point();

            Point bottomReinforcementLeftPointForDimensions = new Point();
            Point bottomReinforcementRightPointForDimensions = new Point();

            Point bottomReinforcementLeftBottomPointForDimensions = new Point(); // Lavy spodny okraj vyztuze pre kotovanie concrete cover od spodneho okraja (default 75 mm)

            Point FloorMeshReinforcementRightPointForDimensions = new Point();
            Point FloorMeshNotePoint = new Point();

            Point dpc_dpm_NotePoint = new Point();

            Point columnNotePoint = new Point();
            Point anchorNotePoint = new Point();

            float fPlateWasherWidth_x = 0;
            float fPlateWasherWidth_y = 0;
            float fPlateWasherThickness = 0;

            Point plateWasherNotePoint = new Point();

            float fBearingWasherWidth_x = 0;
            float fBearingWasherWidth_y = 0;
            float fBearingWasherThickness = 0;

            Point bearingWasherNotePoint = new Point();

            List<CAnchor> anchorsToDraw = null;
            List<Point> anchorControlPointsForDimensions = null;

            Point reinforcement_bottom_x_NotePoint = new Point();
            Point reinforcement_top_x_NotePoint = new Point();
            Point reinforcement_bottom_y_NotePoint = new Point();
            Point reinforcement_top_y_NotePoint = new Point();

            Point starter_NotePoint = new Point();
            float fStarterBarDiameter = 0; // TODO Bolo by lepsie keby sa objekt starter implementoval priamo do floorSlab
            float startersSpacing = 0;

            if (joint != null)
            {
                if (joint.m_arrPlates.FirstOrDefault() is CConCom_Plate_B_basic)
                    basePlate = (CConCom_Plate_B_basic)joint.m_arrPlates.FirstOrDefault();

                if (opts.bDrawColumnOutline)
                {
                    crscDepth = joint.m_MainMember.CrScStart.h;
                    horizontalOffsetColumn = -0.5 * crscDepth; // Column // Pozicia lavej hrany stlpa na obrazku

                    if (joint.m_MainMember.EccentricityStart != null)
                    {
                        if (pad.m_ColumnMemberTypePosition != EMemberType_FS_Position.ColumnBackSide)
                            horizontalOffsetColumn -= joint.m_MainMember.EccentricityStart.MFz_local; // Odpocitavame, pretoze lokalny smer z pruta smeruje v opacnom smere ako x osa v canvas
                        else // Pre wind post na zadnej strane je excentricita definovana zaporna v GCS, preto ju musime otocit
                            horizontalOffsetColumn += joint.m_MainMember.EccentricityStart.MFz_local;
                    }

                    fVerticalOffsetLeft = 0.3f * (float)crscDepth; // TODO - urobit nastavitelne odsadenie podla toho aku velku cast chceme kreslit
                    fTopLineSlope_rad = 15f * MathF.fPI / 180f; // slope in radians
                    fVerticalOffsetRight = fVerticalOffsetLeft + (float)crscDepth * (float)Math.Tan(fTopLineSlope_rad);

                    bottomLeft_ColumnEdge = new Point(horizontalOffsetColumn + 0, basePlate.Ft);
                    topLeft_ColumnEdge = new Point(horizontalOffsetColumn + 0, basePlate.Fl_Z + fVerticalOffsetLeft);

                    bottomRight_ColumnEdge = new Point(horizontalOffsetColumn + crscDepth, basePlate.Ft);
                    topRight_ColumnEdge = new Point(horizontalOffsetColumn + crscDepth, basePlate.Fl_Z + fVerticalOffsetRight);
                }
            }

            // Draw footing pad outline

            float fFloorWidthPart = 0.5f; // 0.5 m // Sirka vykreslovanej casti floor slab - aproximovana skutocna vzdialenost, aby to bolo dobre na obrazku
            float fFloorEdge = 0.03f; // 0.03 m // Horizontalne / Vertikalne skosenie hrany
            float floorThickness = floorSlab.m_fDim3;
            float fPadWidth_y = pad.m_fDim2;
            float fPadDepth_z = pad.m_fDim3;

            float fRealOffset_DPC_DPM = 0.02f; // m // Offset vrstvy DPC / DPM od floor slab alebo footing pad - aproximovana skutocna vzdialenost, aby to bolo dobre na obrazku

            // Suradnica x = 0 je v polovici rozmeru patky 0.5f * fPadWidth_y
            // Suradnica y = 0 je v urovni hornej hrany floor slab

            double horizontalOffset = -0.5 * fPadWidth_y - pad.Eccentricity_y; // Opacne znamienko pre excentricity lebo LCS y v 3D smeruje tam, kam x v 2D

            // Pre wind post na zadnej strane je excentricita definovana zaporna v GCS, preto ju musime otocit
            if (pad.m_ColumnMemberTypePosition == EMemberType_FS_Position.ColumnBackSide)
                horizontalOffset = -0.5 * fPadWidth_y + pad.Eccentricity_y;

            List<Point> PointsFootingPad_real = new List<Point>
                {
                    new Point(horizontalOffset + fFloorWidthPart + fPadWidth_y, -floorThickness),
                    new Point(horizontalOffset + fPadWidth_y + fFloorEdge, -floorThickness),
                    new Point(horizontalOffset + fPadWidth_y, -floorThickness - fFloorEdge),
                    new Point(horizontalOffset + fPadWidth_y, -fPadDepth_z),
                    new Point(horizontalOffset + 0, -fPadDepth_z),
                    new Point(horizontalOffset + 0, 0),
                    new Point(horizontalOffset + fFloorWidthPart + fPadWidth_y, 0)
                };

            // Vytvorime pole bodov v ktorom budu vsetky relevantne krajne body potrebne pre urcenie velkosti vykreslovaneho obrazku
            List<Point> PointsForEdgeCoord_real = new List<Point>
            {
                new Point(PointsFootingPad_real[0].X, PointsFootingPad_real[0].Y - fRealOffset_DPC_DPM), // Right
                new Point(PointsFootingPad_real[4].X, PointsFootingPad_real[4].Y - fRealOffset_DPC_DPM),  // Left Bottom point
                new Point(horizontalOffset + 0, basePlate.Fl_Z + fVerticalOffsetLeft), // Top Left Column Point
                new Point(horizontalOffset + crscDepth, basePlate.Fl_Z + fVerticalOffsetRight), // Top Right Column Point
                new Point(PointsFootingPad_real[1].X, PointsFootingPad_real[1].Y - fRealOffset_DPC_DPM) // pravy okraj patky (bez floor slab)
            };

            // Vypocitame zoom faktor v prvej iteracii
            double fTempMax_X_F1 = 0, fTempMin_X_F1 = 0, fTempMax_Y_F1 = 0, fTempMin_Y_F1 = 0;
            List<Point> PointsForEdgeCoord_real_Copy = PointsFootingPad_real.ToList();

            Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsForEdgeCoord_real_Copy);
            CalculateModelLimits(PointsForEdgeCoord_real_Copy, out fTempMax_X_F1, out fTempMin_X_F1, out fTempMax_Y_F1, out fTempMin_Y_F1);

            double dModel_Length_x_real_F1 = fTempMax_X_F1 - fTempMin_X_F1;
            double dModel_Length_y_real_F1 = fTempMax_X_F1 - fTempMin_Y_F1;

            double dscaleFactor = 0.8;
            int scale_unit = 1000; // mm

            double fModel_Length_x_page_F1;
            double fModel_Length_y_page_F1;
            double dFactor_x_F1;
            double dFactor_y_F1;
            double dReal_Model_Zoom_Factor_F1;

            CalculateBasicValue_ZoomFactor(dModel_Length_x_real_F1,
                dModel_Length_y_real_F1,
                dscaleFactor,
                scale_unit,
                width,
                height,
                out fModel_Length_x_page_F1,
                out fModel_Length_y_page_F1,
                out dFactor_x_F1,
                out dFactor_y_F1,
                out dReal_Model_Zoom_Factor_F1);

            // Maximalna dlzka textbloku pre vypisovanie poznamok (canvas page units - points)
            double dNoteTextMaximumLength_Points = 232 + 18; // Dlzka je 232 bodov + rezerva 18 // PRI ZMENE TEXTOV JE POTREBNE PRENASTAVIT
            // Vypocitame dlzku textu v realnych suradniciach [m]
            double dNoteTextMaximumLength_meters = dNoteTextMaximumLength_Points / dReal_Model_Zoom_Factor_F1;
            // Upravime pole bodov pre vypocet finalneho zoom faktora
            // Pridame pravy koncovy bod textu najdlhsej poznamky
            PointsForEdgeCoord_real.Add(new Point(PointsFootingPad_real[1].X + dNoteTextMaximumLength_meters, PointsFootingPad_real[1].Y - fRealOffset_DPC_DPM));

            // Vypocitame finalny zoom faktor a nastavime dalsie parametre
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
            Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsForEdgeCoord_real);

            CalculateModelLimits(PointsForEdgeCoord_real, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            //PointsForEdgeCoord_real = MovePointsToCenterCSAndCalculateModelLimits(PointsForEdgeCoord_real, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);

            double fModel_Length_x_real;
            double fModel_Length_y_real;
            double fModel_Length_x_page;
            double fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            double dReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginTop_y;
            double dPointInOutDistance_x_page;
            double dPointInOutDistance_y_page;

            CalculateBasicValue(
                    fTempMax_X,
                    fTempMin_X,
                    fTempMax_Y,
                    fTempMin_Y,
                    dscaleFactor,
                    scale_unit,
                    width,
                    height,
                    null,
                    0,
                    0,
                    out fModel_Length_x_real,
                    out fModel_Length_y_real,
                    out fModel_Length_x_page,
                    out fModel_Length_y_page,
                    out dFactor_x,
                    out dFactor_y,
                    out dReal_Model_Zoom_Factor,
                    out fmodelMarginLeft_x,
                    out fmodelMarginTop_y,
                    out dPointInOutDistance_x_page,
                    out dPointInOutDistance_y_page);

            if (opts.bDrawFootingPad)
            {
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsFootingPad_real);

                List<Point> PointsFootingPad_canvas = ConvertRealPointsToCanvasDrawingPoints(PointsFootingPad_real, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                DrawPolyLine(false, PointsFootingPad_canvas, opts.FootingPadColor, PenLineCap.Flat, PenLineCap.Flat, opts.FootingPadThickness, canvasForImage);

                if (opts.bDrawDPC_DPM)
                {
                    float fLineSlope_rad = 45f * MathF.fPI / 180f; // slope in radians
                    float fAngleAux_rad = 0.5f * fLineSlope_rad;
                    double fAux = fRealOffset_DPC_DPM * Math.Tan(fAngleAux_rad);

                    List<Point> PointsDPC_DPM = new List<Point>
                    {
                        new Point(PointsFootingPad_real[0].X, PointsFootingPad_real[0].Y + fRealOffset_DPC_DPM),
                        new Point(PointsFootingPad_real[1].X + fAux, PointsFootingPad_real[1].Y + fRealOffset_DPC_DPM),
                        new Point(PointsFootingPad_real[2].X + fRealOffset_DPC_DPM, PointsFootingPad_real[2].Y + fAux),
                        new Point(PointsFootingPad_real[3].X + fRealOffset_DPC_DPM, PointsFootingPad_real[3].Y + fRealOffset_DPC_DPM),
                        new Point(PointsFootingPad_real[4].X, PointsFootingPad_real[4].Y + fRealOffset_DPC_DPM)
                    };

                    dpc_dpm_NotePoint = new Point(PointsDPC_DPM[1].X + 0.05 * (PointsDPC_DPM[0].X - PointsDPC_DPM[1].X), PointsDPC_DPM[0].Y); // 5% od zlomu

                    PointsDPC_DPM = ConvertRealPointsToCanvasDrawingPoints(PointsDPC_DPM, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                    DoubleCollection dashes = new DoubleCollection();
                    dashes.Add(10); dashes.Add(10);

                    DrawPolyLine(false, PointsDPC_DPM, opts.DPC_DPMColor, PenLineCap.Flat, PenLineCap.Flat, opts.DPC_DPMThickness, canvasForImage, "", opts.DPC_DPMLineStyle, dashes);
                }

                if (opts.bDrawPerimeter)
                {
                    // TODO - sem potrebujeme dostat rozmery perimeter pre danu stranu floor slab kde sa nachadza patka (left/right, front/back)

                    // Left or right side of building
                    float fPerimeterWidth = floorSlab.PerimeterWidth_LRSide; // TODO - napojit
                    float fPerimeterDepth = floorSlab.PerimeterDepth_LRSide; // TODO - napojit

                    // Front or back side
                    if (pad.m_ColumnMemberTypePosition == EMemberType_FS_Position.ColumnFrontSide || pad.m_ColumnMemberTypePosition == EMemberType_FS_Position.ColumnBackSide)
                    {
                        fPerimeterWidth = floorSlab.PerimeterWidth_FBSide;
                        fPerimeterDepth = floorSlab.PerimeterDepth_FBSide;
                    }

                    List<Point> PointsPerimeter = new List<Point>
                    {
                        new Point(PointsFootingPad_real[1].X, PointsFootingPad_real[1].Y),
                        new Point(PointsFootingPad_real[4].X + fPerimeterWidth + fFloorEdge, PointsFootingPad_real[1].Y),
                        new Point(PointsFootingPad_real[4].X + fPerimeterWidth, PointsFootingPad_real[1].Y + fFloorEdge),
                        new Point(PointsFootingPad_real[4].X + fPerimeterWidth, PointsFootingPad_real[5].Y + fPerimeterDepth),
                        new Point(PointsFootingPad_real[5].X, PointsFootingPad_real[5].Y + fPerimeterDepth)
                    };

                    PointsPerimeter = ConvertRealPointsToCanvasDrawingPoints(PointsPerimeter, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                    DoubleCollection dashes = new DoubleCollection();
                    dashes.Add(10); dashes.Add(10);

                    DrawPolyLine(false, PointsPerimeter, opts.PerimeterColor, PenLineCap.Flat, PenLineCap.Flat, opts.PerimeterThickness, canvasForImage,"", opts.PerimeterLineStyle, dashes);
                }

                if (opts.bDrawReinforcement)
                {
                    // Vyztuz v smere x kreslime ako kruhy (v reze)
                    // Vyztuz v smere y kreslime ako ciary (v pohlade z boku)

                    double dCircleScaleFactorFor2D = 1.3; // Faktor ktory zvacsi priemer vyztuze pre zobrazenie aby bola lepsie viditelna

                    bool bIsReinforcementBarStraight = pad.Reference_Top_Bar_x is CReinforcementBarStraight || pad.Reference_Top_Bar_y is CReinforcementBarStraight ||
                        pad.Reference_Bottom_Bar_x is CReinforcementBarStraight || pad.Reference_Bottom_Bar_y is CReinforcementBarStraight; // TODO - tento bool z UC Footing Input by sme sem potrebovali dostat

                    // Reinforcement in LCS x direction - circles
                    if (pad.Top_Bars_x != null && pad.Top_Bars_x.Count > 0)
                    {
                        float fFirstPosition_y = (float)pad.Reference_Top_Bar_x.m_pControlPoint.Y;
                        float fDistanceInLCS_y = pad.DistanceOfBars_Top_x_SpacingInyDirection;
                        double VerticalPosition = pad.Top_Bars_x.First().m_pControlPoint.Z; // Priama vyztuz - kreslime podla suradnic skutocneho bodu vlozenia

                        if (!bIsReinforcementBarStraight) // Tvar U - vkladaci bod je dole, vyztuz kreslime k hornemu okraju
                            VerticalPosition = -pad.ConcreteCover - pad.Reference_Top_Bar_y.Diameter - 0.5 * pad.Reference_Top_Bar_x.Diameter;

                        for (int i = 0; i < pad.Top_Bars_x.Count; i++)
                        {
                            Point p = new Point(horizontalOffset + fFirstPosition_y + i * fDistanceInLCS_y, VerticalPosition);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref p);

                            if (i == pad.Top_Bars_x.Count - 1) // Consider only last bar
                                reinforcement_top_x_NotePoint = new Point(p.X, p.Y); // Nastavime bod pre poznamku // uvazujeme otocene suradnice poslednej tyce

                            p = ConvertRealPointToCanvasDrawingPoint(p, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            DrawCircle(p, dCircleScaleFactorFor2D * dReal_Model_Zoom_Factor * pad.Top_Bars_x[i].Diameter /*pad.Reference_Top_Bar_x.Diameter*/, opts.ReinforcementInSectionStrokeColor, opts.ReinforcementInSectionFillColor, opts.ReinforcementInSectionThickness, canvasForImage);
                        }
                    }

                    if (pad.Bottom_Bars_x != null && pad.Bottom_Bars_x.Count > 0)
                    {
                        float fFirstPosition_y = (float)pad.Reference_Bottom_Bar_x.m_pControlPoint.Y;
                        float fDistanceInLCS_y = pad.DistanceOfBars_Bottom_x_SpacingInyDirection;
                        double VerticalPosition = pad.Bottom_Bars_x.First().m_pControlPoint.Z; // Priama vyztuz - kreslime podla suradnic skutocneho bodu vlozenia

                        if (!bIsReinforcementBarStraight) // Tvar U - vkladaci bod je dole, vyztuz kreslime k hornemu okraju
                            VerticalPosition = -pad.m_fDim3 + pad.ConcreteCover + pad.Reference_Bottom_Bar_y.Diameter + 0.5 * pad.Reference_Bottom_Bar_x.Diameter;

                        for (int i = 0; i < pad.Bottom_Bars_x.Count; i++)
                        {
                            Point p = new Point(horizontalOffset + fFirstPosition_y + i * fDistanceInLCS_y, VerticalPosition);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref p);

                            if (i == pad.Bottom_Bars_x.Count - 1) // Consider only last bar
                                reinforcement_bottom_x_NotePoint = new Point(p.X, p.Y); // Nastavime bod pre poznamku // uvazujeme otocene suradnice poslednej tyce

                            p = ConvertRealPointToCanvasDrawingPoint(p, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            DrawCircle(p, dCircleScaleFactorFor2D * dReal_Model_Zoom_Factor * pad.Bottom_Bars_x[i].Diameter /*pad.Reference_Bottom_Bar_x.Diameter*/, opts.ReinforcementInSectionStrokeColor, opts.ReinforcementInSectionFillColor, opts.ReinforcementInSectionThickness, canvasForImage);
                        }
                    }

                    double dLineThicknessFactor = dReal_Model_Zoom_Factor; // TODO Ondrej - Vhodne nastavit zavislost hrubky ciary a priemeru vyztuze

                    // Reinforcement in LCS y direction - lines
                    if (pad.Top_Bars_y != null && pad.Top_Bars_y.Count > 0)
                    {
                        // Kreslime len prvy prut

                        if (pad.Top_Bars_y.First() is CReinforcementBarStraight)
                        {
                            Point pStart = new Point(horizontalOffset + pad.ConcreteCover, pad.Top_Bars_y.First().StartPoint.Z);
                            Point pEnd = new Point(horizontalOffset + pad.ConcreteCover + pad.Top_Bars_y.First().TotalLength, pad.Top_Bars_y.First().EndPoint.Z);

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref pStart);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref pEnd);

                            reinforcement_top_y_NotePoint = new Point(pEnd.X - 0.05, pEnd.Y); // Nastavime bod pre poznamku // uvazujeme otocene suradnice // suradnica x je znizena o 0.05 metra, aby nebola poznamku uplne na konci

                            pStart = ConvertRealPointToCanvasDrawingPoint(pStart, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                            pEnd = ConvertRealPointToCanvasDrawingPoint(pEnd, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            DrawPolyLine(false, new List<Point> { pStart, pEnd }, opts.ReinforcementInWiewColorTop, PenLineCap.Flat, PenLineCap.Flat, dLineThicknessFactor * pad.Top_Bars_y.First().Diameter, canvasForImage,"", DashStyles.Solid, null);
                        }
                        else
                        {
                            // Vyztuz v tvare U

                            // Posun aby boli viditelne zvisle ciary
                            float fCenterLineHorizontalOffsetFromBottom = 0.5f * pad.Reference_Top_Bar_y.Diameter + 0.5f * pad.Reference_Bottom_Bar_y.Diameter;
                            // Pridanie medzery medzi ciarami
                            fCenterLineHorizontalOffsetFromBottom += 0.5f * fCenterLineHorizontalOffsetFromBottom;

                            DrawReincementBar_U_Shape(
                                /*pad.Reference_Top_Bar_y*/ (CReinforcementBar_U)pad.Top_Bars_y.First(), // TODO - Ondrej - neupdatuju sa parametre reference bar len objektov v poli, mali sa updatovat aj vlastnosti reference bar podla GUI
                                pad,
                                new Point(horizontalOffset + fCenterLineHorizontalOffsetFromBottom, 0), // Vkladaci bod vyztuze (lavy horny roh patky) // posun v smere x aby bolo vidno zvisle ciary spodneho aj horneho pruta vyztuze
                                0.03f, // Netto polomer zakrivenia rohu
                                true,
                                opts.ReinforcementInWiewColorTop,
                                fTempMin_X,
                                fTempMin_Y,
                                fmodelMarginLeft_x,
                                fmodelMarginTop_y,
                                dReal_Model_Zoom_Factor,
                                canvasForImage,
                                out reinforcement_top_y_NotePoint);
                        }
                    }

                    if (pad.Bottom_Bars_y != null && pad.Bottom_Bars_y.Count > 0)
                    {
                        // Toto su vypocty pre rovnu vyztuz ale vybral som to pred if, aby sa nastavili suradnice bodov pre koty
                        // TODO - Trebalo by sa s tym pohrat a oddelit urcenie tychto suradnic pre priame pruty a pre tvar U

                        Point pStart = new Point(horizontalOffset + pad.ConcreteCover, pad.Bottom_Bars_y.First().StartPoint.Z);

                        // Nezohladnujeme dlzku pruta kedze chceme kotovat sirku bez net cover (bY - 2 * c)
                        // Pre pruty priame a U je dlzka horizontalneho priemetu ina, takze by sme to museli implementovat oddelene viz todo vyssie
                        Point pEnd = new Point(horizontalOffset + pad.m_fDim2 - pad.ConcreteCover /*pad.ConcreteCover + pad.Bottom_Bars_y.First().TotalLength*/, pad.Bottom_Bars_y.First().EndPoint.Z);

                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref pStart);
                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref pEnd);

                        bottomReinforcementLeftPointForDimensions = new Point(pStart.X, pStart.Y); // Nastavime suradnice bodov pouzite pre kotovanie
                        bottomReinforcementRightPointForDimensions = new Point(pEnd.X, pEnd.Y);

                        bottomReinforcementLeftBottomPointForDimensions = new Point(pStart.X, pStart.Y + 0.5f * pad.Reference_Bottom_Bar_y.Diameter); // Pripocitana vzdialenost od stredu tyce smerom nadol (polovica priemeru), aby sme ziskali spodny okraj

                        // Kreslime len prvy prut
                        if (pad.Bottom_Bars_y.First() is CReinforcementBarStraight)
                        {
                            reinforcement_bottom_y_NotePoint = new Point(pEnd.X - 0.05, pEnd.Y); // Nastavime bod pre poznamku // uvazujeme otocene suradnice // suradnica x je znizena o 0.05 metra, aby nebola poznamku uplne na konci

                            pStart = ConvertRealPointToCanvasDrawingPoint(pStart, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                            pEnd = ConvertRealPointToCanvasDrawingPoint(pEnd, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            DrawPolyLine(false, new List<Point> { pStart, pEnd }, opts.ReinforcementInWiewColorBottom, PenLineCap.Flat, PenLineCap.Flat, dLineThicknessFactor * pad.Bottom_Bars_y.First().Diameter, canvasForImage,"", DashStyles.Solid, null);
                        }
                        else
                        {
                            // Vyztuz v tvare U
                            DrawReincementBar_U_Shape(
                                 /*pad.Reference_Bottom_Bar_y */ (CReinforcementBar_U)pad.Bottom_Bars_y.First(), // TODO - Ondrej - neupdatuju sa parametre reference bar len objektov v poli, mali sa updatovat aj vlastnosti reference bar podla GUI
                                 pad,
                                 new Point(horizontalOffset, 0), // Vkladaci bod vyztuze (lavy horny roh patky)
                                 0.03f, // Netto polomer zakrivenia rohu
                                 false,
                                 opts.ReinforcementInWiewColorBottom,
                                 fTempMin_X,
                                 fTempMin_Y,
                                 fmodelMarginLeft_x,
                                 fmodelMarginTop_y,
                                 dReal_Model_Zoom_Factor,
                                 canvasForImage,
                                 out reinforcement_bottom_y_NotePoint);
                        }
                    }

                    // Vyztuz v podlahovej doske
                    // Kreslime len prvy prut
                    if (opts.bDrawReinforcementInSlab)
                    {
                        double horizontalOffsetReinfocementInSlab = horizontalOffset + floorSlab.ConcreteCover;

                        float reinfocementDiameter = 0.008f; // m // TODO zapracovat priemer prutov siete do databazy a vykreslovat

                        Point pStart = new Point(horizontalOffsetReinfocementInSlab, PointsFootingPad_real.Last().Y + floorSlab.ConcreteCover + 0.5 * reinfocementDiameter);
                        Point pEnd = new Point(PointsFootingPad_real.Last().X, PointsFootingPad_real.Last().Y + floorSlab.ConcreteCover + 0.5 * reinfocementDiameter);

                        //Geom2D.MirrorAboutX_ChangeYCoordinates(ref pStart); // Netransformujeme - body PointsFootingPad_real uz boli transformovane
                        //Geom2D.MirrorAboutX_ChangeYCoordinates(ref pEnd);

                        FloorMeshReinforcementRightPointForDimensions = new Point(pEnd.X, pEnd.Y - 0.5 * reinfocementDiameter); // Odpocitame polomer vystuze a dostaneme hodnotu cisteho krytia pre y // Nastavime suradnice bodov pouzite pre kotovanie
                        FloorMeshNotePoint = new Point(PointsFootingPad_real[1].X, FloorMeshReinforcementRightPointForDimensions.Y); // Bod pre note - zaciatok sipky poznamky

                        pStart = ConvertRealPointToCanvasDrawingPoint(pStart, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                        pEnd = ConvertRealPointToCanvasDrawingPoint(pEnd, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                        DoubleCollection dashes = new DoubleCollection();
                        dashes.Add(3); dashes.Add(3); // Kratsie a hustejsie ciarky

                        //opts.ReinforcementInSlabThickness = dLineThicknessFactor * reinfocementDiameter;
                        DrawPolyLine(false, new List<Point> { pStart, pEnd }, opts.ReinforcementInSlabColor, PenLineCap.Flat, PenLineCap.Flat, dLineThicknessFactor * reinfocementDiameter, canvasForImage,"", opts.ReinforcementInSlabLineStyle, dashes);

                        // Starter - reinforcement bar to connect with mesh in floor slab
                        bool bDrawReinforcement_Starter = true;

                        if (bDrawReinforcement_Starter)
                            DrawReincementBar_Starter_Shape(
                            pad,
                            floorSlab,
                            new Point(horizontalOffset - 0.01, -0.01), // Vkladaci bod vyztuze (lavy horny roh patky) // Posunieme o trosku nahor, aby sa neprekvyvali
                            0.03f, // m
                            opts.ReinforcementInWiewColorStarter,
                            fTempMin_X,
                            fTempMin_Y,
                            fmodelMarginLeft_x,
                            fmodelMarginTop_y,
                            dReal_Model_Zoom_Factor,
                            canvasForImage,
                            out starter_NotePoint,
                            out fStarterBarDiameter, // TODO Bolo by lepsie keby sa objekt starter implementoval priamo do floorSlab
                            out startersSpacing
                            );
                    }
                }
            }

            if (opts.bDrawColumnOutline)
            {
                int iSectionDatabaseID = joint.m_MainMember.CrScStart.DatabaseID;
                //string sSectionName = joint.m_MainMember.CrScStart.Name_short;

                // TODO Ondrej - potreboval by som toto pridat do databazy - je to pole medzier medi zlomami hran / rebrami prierezu, mal by to byt string s ciarkami alebo bodkociarkami ktory potom prevedieme na cisla ???
                // Kazdy prierez moze mat iny pocet hran / rebier
                /*
                1  10075
                2  27055
                3  27095
                4  27095n
                5  270115
                6  270115btb
                7  270115n
                8  50020
                9  50020n
                10 63020
                11 63020s1
                12 63020s2
                */

                List<double> crscWebSegmentsWidths = null;

                // Hodnoty su v [mm]
                switch (iSectionDatabaseID)
                {
                    case 1:
                        crscWebSegmentsWidths = new List<double>() { 6, 14, 4.5, 14, 4.5, 14, 4.5, 14, 4.5, 14, 6 };
                        break;
                    case 2:
                    case 3:
                    case 5:
                    case 6: // Neplati pri pohlade z boku ale mne to nevadi :)
                        crscWebSegmentsWidths = new List<double>() { 8, 40.8, 20, 17.5, 20, 57.4, 20, 17.5, 20, 40.8, 8 };
                        break;
                    case 4:
                    case 7:
                        crscWebSegmentsWidths = new List<double>() { 8, 40.8, 20, 17.5, 20, 57.4, 20, 17.5, 20, 40.8, 8, 5, 7, 8 };
                        break;
                    case 8:
                        crscWebSegmentsWidths = new List<double>() { 8, 176.5, 131, 176.5, 8 };
                        break;
                    case 9:
                        crscWebSegmentsWidths = new List<double>() { 8, 176.5, 131, 176.5, 8, 8, 34, 8 };
                        break;
                    case 10:
                    case 11:
                    case 12:
                        crscWebSegmentsWidths = new List<double>() { 25, 51.8, 16, 14.9, 16, 98, 46.8, 93, 46.8, 98, 16, 14.9, 16, 51.8, 25 };
                        break;
                    default:
                        new Exception("Cross section is not implemented");
                        break;
                }

                List<double> stiffenersHorizontalPositions = null;
                if (crscWebSegmentsWidths != null) // Konverzia na suradnice positions a z mm na m
                {
                    stiffenersHorizontalPositions = new List<double>();

                    double dCurrentCoordinate = 0;

                    for (int i = 0; i < crscWebSegmentsWidths.Count - 1; i++) // Pocet pozicii vnutornych vyztuh je pocet segmentov - 1
                    {
                        stiffenersHorizontalPositions.Add((dCurrentCoordinate + crscWebSegmentsWidths[i]) / 1000); // Pridanie absolutnej pozicie hrany a konverzia na mm
                        dCurrentCoordinate += crscWebSegmentsWidths[i]; // nastavenie novej hodnoty pre hranu
                    }
                }

                List<Point> PointsStiffenersBottom = new List<Point>();
                List<Point> PointsStiffenersIntermediate = new List<Point>();
                List<Point> PointsStiffenersTop = new List<Point>();

                // Sfiffeners Edges
                if (stiffenersHorizontalPositions != null)
                {
                    for (int i = 0; i < stiffenersHorizontalPositions.Count; i++)
                    {
                        PointsStiffenersBottom.Add(new Point(horizontalOffsetColumn + stiffenersHorizontalPositions[i], basePlate.Ft));
                        PointsStiffenersIntermediate.Add(new Point(horizontalOffsetColumn + stiffenersHorizontalPositions[i], basePlate.Fl_Z));
                        //double fVerticalOffset_x = fVerticalOffsetRight + ((float)crscDepth - stiffenersHorizontalPositions[i]) * (float)Math.Tan(fTopLineSlope_rad); // pozicie zadane zdola (kreslene zprava)
                        double fVerticalOffset_x = fVerticalOffsetLeft + (stiffenersHorizontalPositions[i]) * (float)Math.Tan(fTopLineSlope_rad); // Pozicie zadane zhora (kreslene zlava)
                        PointsStiffenersTop.Add(new Point(horizontalOffsetColumn + stiffenersHorizontalPositions[i], basePlate.Fl_Z + fVerticalOffset_x));

                        // Draw Lines

                        bool bBottomPartBehindPlate = true;

                        if (bBottomPartBehindPlate)
                        {
                            Point bottom = new Point(PointsStiffenersBottom[i].X, PointsStiffenersBottom[i].Y);
                            Point top = new Point(PointsStiffenersIntermediate[i].X, PointsStiffenersIntermediate[i].Y);

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref bottom);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref top);

                            List<Point> PointsLine = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { bottom, top }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            Line l = new Line();
                            l.X1 = PointsLine[0].X;
                            l.Y1 = PointsLine[0].Y;

                            l.X2 = PointsLine[1].X;
                            l.Y2 = PointsLine[1].Y;

                            DoubleCollection dashes = new DoubleCollection();
                            dashes.Add(10); dashes.Add(10);

                            DrawLine(l, opts.ColumnOutlineBehindColor, PenLineCap.Flat, PenLineCap.Flat, opts.ColumnOutlineThickness, canvasForImage, opts.ColumnOutlineBehindLineStyle, dashes);
                        }

                        bool bTopPartAbovePlate = true;

                        if (bTopPartAbovePlate)
                        {
                            Point bottom = new Point(PointsStiffenersIntermediate[i].X, PointsStiffenersIntermediate[i].Y);
                            Point top = new Point(PointsStiffenersTop[i].X, PointsStiffenersTop[i].Y);

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref bottom);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref top);

                            List<Point> PointsLine = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { bottom, top }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            Line l = new Line();
                            l.X1 = PointsLine[0].X;
                            l.Y1 = PointsLine[0].Y;

                            l.X2 = PointsLine[1].X;
                            l.Y2 = PointsLine[1].Y;

                            DrawLine(l, opts.ColumnOutlineAboveColor, PenLineCap.Flat, PenLineCap.Flat, opts.ColumnOutlineThickness, canvasForImage, DashStyles.Solid);
                        }
                    }
                }

                // Outlines

                // Left Line
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref bottomLeft_ColumnEdge);
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref topLeft_ColumnEdge);

                List<Point> PointsLineLeft = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { bottomLeft_ColumnEdge, topLeft_ColumnEdge }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                Line l_Left = new Line();
                l_Left.X1 = PointsLineLeft[0].X;
                l_Left.Y1 = PointsLineLeft[0].Y;

                l_Left.X2 = PointsLineLeft[1].X;
                l_Left.Y2 = PointsLineLeft[1].Y;

                DrawLine(l_Left, opts.ColumnOutlineColor, PenLineCap.Flat, PenLineCap.Flat, opts.ColumnOutlineThickness, canvasForImage, DashStyles.Solid);

                // Right Line
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref bottomRight_ColumnEdge);
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref topRight_ColumnEdge);

                columnNotePoint = new Point(topRight_ColumnEdge.X, topRight_ColumnEdge.Y + 0.5 * fVerticalOffsetRight);

                List<Point> PointsLineRight = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { bottomRight_ColumnEdge, topRight_ColumnEdge }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                Line l_Right = new Line();
                l_Right.X1 = PointsLineRight[0].X;
                l_Right.Y1 = PointsLineRight[0].Y;

                l_Right.X2 = PointsLineRight[1].X;
                l_Right.Y2 = PointsLineRight[1].Y;

                DrawLine(l_Right, opts.ColumnOutlineColor, PenLineCap.Flat, PenLineCap.Flat, opts.ColumnOutlineThickness, canvasForImage, DashStyles.Solid);

                // Top Line
                Line l_Top = new Line();
                l_Top.X1 = PointsLineLeft[1].X;
                l_Top.Y1 = PointsLineLeft[1].Y;

                l_Top.X2 = PointsLineRight[1].X;
                l_Top.Y2 = PointsLineRight[1].Y;

                DrawLine(l_Top, opts.ColumnOutlineColor, PenLineCap.Flat, PenLineCap.Flat, opts.ColumnOutlineThickness, canvasForImage, DashStyles.Solid);
            }

            if (basePlate != null)
            {
                Point insertingPoint_Plate = new Point(0, 0); // Stred plechu

                if (joint.m_MainMember.EccentricityStart != null) // Napojenie na excentricity - ak je prutu nastavena excentricita, nastavime rovnaky posun stredu plate
                {
                    if (pad.m_ColumnMemberTypePosition != EMemberType_FS_Position.ColumnBackSide)
                        insertingPoint_Plate.X = -joint.m_MainMember.EccentricityStart.MFz_local; // Odpocitavame, pretoze lokalny smer z pruta smeruje v opacnom smere ako x osa v canvas
                    else
                        insertingPoint_Plate.X = +joint.m_MainMember.EccentricityStart.MFz_local;
                }

                Point lt_Plate = new Point(insertingPoint_Plate.X - basePlate.Fh_Y * 0.5, insertingPoint_Plate.Y + basePlate.Fl_Z); // lavy horny bod plechu v realnych suradniciach - preklopena
                Point br_Plate = new Point(insertingPoint_Plate.X + basePlate.Fh_Y * 0.5, insertingPoint_Plate.Y);  // pravy spodny bod plechu v realnych suradniciach - preklopena

                // Draw Plate Outline
                if (opts.bDrawBasePlate)
                {
                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_Plate);
                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_Plate);

                    List<Point> PointsPlate = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_Plate, br_Plate }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                    DrawRectangle(opts.BasePlateColor, null, opts.BasePlateThickness, canvasForImage, PointsPlate[0], PointsPlate[1]);

                    // Obrys vnutornej hrany

                    Point left = new Point(lt_Plate.X, lt_Plate.Y + basePlate.Ft);
                    Point right = new Point(br_Plate.X, lt_Plate.Y + basePlate.Ft);

                    List<Point> PointsPlateLine = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { left, right }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsPlateLine);
                    Line l = new Line();
                    l.X1 = PointsPlateLine[0].X;
                    l.Y1 = PointsPlateLine[0].Y;

                    l.X2 = PointsPlateLine[1].X;
                    l.Y2 = PointsPlateLine[1].Y;

                    // Spodna neviditelna hrana
                    DrawLine(l, opts.BasePlateColor, PenLineCap.Flat, PenLineCap.Flat, opts.BasePlateThickness, canvasForImage, DashStyles.Dash);

                    if (opts.bDrawScrews)
                    {
                        List<Point> PointsHolesScrews = basePlate.ScrewArrangement.HolesCentersPoints2D.ToList();

                        List<Point> canvasPointsHolesScrews = new List<Point>();

                        for (int i = 0; i < PointsHolesScrews.Count / 2; i++) // Kreslime len polovicu bodov
                        {
                            // Potrebujeme zamenit suradnice x a y
                            // x - sme na lavej hrane, pripocitame sirku plechu, takze sa dostaneme na pravu hranu plechu a potom odpocitavame suradnici skrutiek (su definovane zprava smerom dolava)
                            canvasPointsHolesScrews.Add(new Point(lt_Plate.X + basePlate.Fh_Y - PointsHolesScrews[i].Y, -lt_Plate.Y - PointsHolesScrews[i].X)); // -lt_Plate.Y - uz bolo preklopene uvazujem kladnu hodnotu
                        }

                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref canvasPointsHolesScrews);
                        canvasPointsHolesScrews = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHolesScrews, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                        double dHolesDiameterScrews = basePlate.ScrewArrangement.referenceScrew.Diameter_shank * dReal_Model_Zoom_Factor;

                        DrawHoles(opts.bDrawHoles, opts.bDrawHoleCentreSymbols, canvasPointsHolesScrews, opts.bHoleColor, opts.bHoleCenterSymbolColor, opts.HoleLineThickness, opts.HoleCenterSymbolLineThickness, dHolesDiameterScrews, canvasForImage, "", "", 1);
                    }
                }

                // Draw Anchors
                if (basePlate.AnchorArrangement.Anchors != null && basePlate.AnchorArrangement.Anchors.Length > 0)
                {
                    // Filter anchors in one row // Nechceme zobrazovat kotvy ktore su za sebou a prekryvaju sa

                    anchorsToDraw = new List<CAnchor>(); // Kotvy, ktore sa budu vykreslovat. Suradnice Y su dane v ramci plate pricom y smeruje zprava dolava od pravej hrany plechu

                    foreach (CAnchor anchor in basePlate.AnchorArrangement.Anchors)
                    {
                        if (MathF.d_equal(anchor.m_pControlPoint.X, basePlate.AnchorArrangement.Anchors[0].m_pControlPoint.X)) // Pridame do zoznamu len kotvy, ktore maju rovnaku suradnicu X (kolmo na zobrazovanu rovinu YZ) ako prva kotva
                            anchorsToDraw.Add(anchor);
                    }

                    anchorControlPointsForDimensions = new List<Point>(); // Inicializujeme zoznam bodov pre koty

                    // Anchor Bar
                    if (opts.bDrawAnchors)
                    {
                        // Zoradim kotvy od najvacsej suradnice control point Y, to znamena ze ta ktora je s najvacsim Y je uplne vlavo, najmensie x v canvas
                        // Robim to preto aby sa z poslednej kotvy upne napravo nastavili suradnice bodov pre poznamky a koty
                        anchorsToDraw = anchorsToDraw.OrderByDescending(anchor => anchor.m_pControlPoint.Y).ToList();

                        foreach (CAnchor anchor in anchorsToDraw) // Kreslime vyfiltrovane kotvy
                        {
                            // TODO Implementovat funckiu ktora vykresli jednu anchors + washers

                            float fAnchorDiameter = anchor.Diameter_shank;
                            float fAnchorLength = anchor.Length;
                            Point insertingPoint = new Point(lt_Plate.X + basePlate.Fh_Y - anchor.m_pControlPoint.Y, anchor.m_pControlPoint.Z); // Kotvy kreslime zlava doprava(os +x v canvas smeruje doprava) ale ich suradnice v plate su definovane zprava dolava (os +Y smeruje nalavo), preto musime prepocitat suradnice v horizontalnom smere

                            // Pridame bod do zoznamu bodov pre kotovanie
                            Point insertingPointForDimesnions = new Point(); // Vytvorime nezavisly objekt pre bod - klon :)
                            insertingPointForDimesnions.X = insertingPoint.X;
                            insertingPointForDimesnions.Y = insertingPoint.Y;
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref insertingPointForDimesnions); // Bod transformujeme
                            anchorControlPointsForDimensions.Add(insertingPointForDimesnions); // Bod pridame do zoznamu bodov pre koty

                            Point lt = new Point(insertingPoint.X - fAnchorDiameter * 0.5, insertingPoint.Y);
                            Point br = new Point(insertingPoint.X + fAnchorDiameter * 0.5, insertingPoint.Y - fAnchorLength);
                            anchorNotePoint = new Point(insertingPoint.X + fAnchorDiameter * 0.5, insertingPoint.Y - 0.7 * fAnchorLength);

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref br);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref anchorNotePoint);

                            List<Point> PointsAnchor = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt, br }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                            DrawRectangle(opts.AnchorStrokeColor, null, opts.AnchorLineThickness, canvasForImage, PointsAnchor[0], PointsAnchor[1]);

                            // Washers & Nuts
                            CNut nut = new CNut(anchor.Name, "8.8", new System.Windows.Media.Media3D.Point3D(0, 0, 0), 0, 0, 0, true); // Reference Nut // TODO - prerobit na skutocne nuts - 1x washer pre plate top a 2x bearing washer
                            float fNutWidth = nut.SizeAcrossCorners;
                            float fNutHeight = nut.Thickness_max;

                            // Washer - Plate
                            if (opts.bDrawWashers)
                            {
                                fPlateWasherWidth_x = anchor.WasherPlateTop.Width_bx; // Kolmo na rovinu
                                fPlateWasherWidth_y = anchor.WasherPlateTop.Height_hy;
                                fPlateWasherThickness = anchor.WasherPlateTop.Ft;

                                float fPlateWasherOffsetFromTop = (float)anchor.m_pControlPoint.Z - fPlateWasherThickness; // TO napojit na GUI ???

                                Point lt_WasherPlate = new Point(insertingPoint.X - fPlateWasherWidth_y * 0.5, insertingPoint.Y - fPlateWasherOffsetFromTop);
                                Point br_WasherPlate = new Point(insertingPoint.X + fPlateWasherWidth_y * 0.5, insertingPoint.Y - fPlateWasherOffsetFromTop - fPlateWasherThickness); // TODO - ??? Toto by y malo byt zaporne a potom sa preklopit

                                Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_WasherPlate);
                                Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_WasherPlate);

                                plateWasherNotePoint = new Point(br_WasherPlate.X, lt_WasherPlate.Y);

                                List<Point> PointsPlateWasher = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_WasherPlate, br_WasherPlate }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                                DrawRectangle(opts.WasherStrokeColor, null, opts.WasherLineThickness, canvasForImage, PointsPlateWasher[0], PointsPlateWasher[1]);

                                // Nut - Plate
                                if (opts.bDrawNuts)
                                {
                                    float fPlateWasherNutOffsetFromTop = (float)anchor.m_pControlPoint.Z - fPlateWasherThickness - fNutHeight;

                                    Point lt_WasherPlateNut = new Point(insertingPoint.X - fNutWidth * 0.5, insertingPoint.Y - fPlateWasherNutOffsetFromTop);
                                    Point br_WasherPlateNut = new Point(insertingPoint.X + fNutWidth * 0.5, insertingPoint.Y - fPlateWasherNutOffsetFromTop - fNutHeight);

                                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_WasherPlateNut);
                                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_WasherPlateNut);

                                    List<Point> PointsPlateWasherNut = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_WasherPlateNut, br_WasherPlateNut }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                                    DrawRectangle(opts.NutStrokeColor, null, opts.NutLineThickness, canvasForImage, PointsPlateWasherNut[0], PointsPlateWasherNut[1]);
                                }
                            }

                            // Washer - Bearing
                            if (opts.bDrawWashers)
                            {
                                fBearingWasherWidth_x = anchor.WasherBearing.Width_bx; // Kolmo na rovinu
                                fBearingWasherWidth_y = anchor.WasherBearing.Height_hy;
                                fBearingWasherThickness = anchor.WasherBearing.Ft;

                                float fBearingWasherOffsetFromTop = fAnchorLength - 0.03f; // TO napojit na GUI ??? // Vzdialenost od hornej hrany kotvy po hornu hranu bearing washer

                                Point lt_BearingWasher = new Point(insertingPoint.X - fBearingWasherWidth_y * 0.5, insertingPoint.Y - fBearingWasherOffsetFromTop);
                                Point br_BearingWasher = new Point(insertingPoint.X + fBearingWasherWidth_y * 0.5, insertingPoint.Y - fBearingWasherOffsetFromTop - fBearingWasherThickness); // TODO - ??? Toto by y malo byt zaporne a potom sa preklopit

                                Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_BearingWasher);
                                Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_BearingWasher);

                                bearingWasherNotePoint = new Point(br_BearingWasher.X, lt_BearingWasher.Y);

                                List<Point> PointsBearingWasher = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_BearingWasher, br_BearingWasher }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                                DrawRectangle(opts.WasherStrokeColor, null, opts.WasherLineThickness, canvasForImage, PointsBearingWasher[0], PointsBearingWasher[1]);

                                // Nuts - Bearing
                                if (opts.bDrawNuts)
                                {
                                    // Top nut

                                    float fBearingWasherNut_Top_OffsetFromTop = fBearingWasherOffsetFromTop - fNutHeight;

                                    Point lt_WasherBearingNut_Top = new Point(insertingPoint.X - fNutWidth * 0.5, insertingPoint.Y - fBearingWasherNut_Top_OffsetFromTop);
                                    Point br_WasherBearingNut_Top = new Point(insertingPoint.X + fNutWidth * 0.5, insertingPoint.Y - fBearingWasherNut_Top_OffsetFromTop - fNutHeight);

                                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_WasherBearingNut_Top);
                                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_WasherBearingNut_Top);

                                    List<Point> PointsBearingWasherNut_Top = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_WasherBearingNut_Top, br_WasherBearingNut_Top }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                                    DrawRectangle(opts.NutStrokeColor, null, opts.NutLineThickness, canvasForImage, PointsBearingWasherNut_Top[0], PointsBearingWasherNut_Top[1]);

                                    // Bottom nut
                                    float fBearingWasherNut_Bottom_OffsetFromTop = fBearingWasherOffsetFromTop + fBearingWasherThickness;

                                    Point lt_WasherBearingNut_Bottom = new Point(insertingPoint.X - fNutWidth * 0.5, insertingPoint.Y - fBearingWasherNut_Bottom_OffsetFromTop);
                                    Point br_WasherBearingNut_Bottom = new Point(insertingPoint.X + fNutWidth * 0.5, insertingPoint.Y - fBearingWasherNut_Bottom_OffsetFromTop - fNutHeight);

                                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_WasherBearingNut_Bottom);
                                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_WasherBearingNut_Bottom);

                                    List<Point> PointsBearingWasherNut_Bottom = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_WasherBearingNut_Bottom, br_WasherBearingNut_Bottom }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                                    DrawRectangle(opts.NutStrokeColor, null, opts.NutLineThickness, canvasForImage, PointsBearingWasherNut_Bottom[0], PointsBearingWasherNut_Bottom[1]);
                                }
                            }
                        }
                    }
                }
            }

            if (opts.bDrawDimensions)
            {
                List<CDimension> Dimensions = new List<CDimension>(); // Real

                Point center = new Point(0, 0); // TO Ondrej - toto by mal byt asi stred obrazku

                // Vertical Dimensions
                Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[4], PointsFootingPad_real[5], true, true, 40)); // Vertical Dimension - footing pad depth
                Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[0], PointsFootingPad_real[6], false, true, 50)); // Vertical Dimension - floor slab thickness
                Dimensions.Add(new CDimensionLinear(center, new Point(PointsFootingPad_real[0].X, PointsFootingPad_real[3].Y), PointsFootingPad_real[0], false, true, 50)); // Vertical Dimension - footing pad to floor slab bottom surface

                // Reinforcement
                if (opts.bDrawReinforcement)
                    Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[4], new Point(PointsFootingPad_real[4].X, bottomReinforcementLeftBottomPointForDimensions.Y), true, true, 20)); // Vertical Dimension - reinforcement cover bottom

                if (opts.bDrawReinforcement && opts.bDrawReinforcementInSlab)
                    Dimensions.Add(new CDimensionLinear(center, new Point(PointsFootingPad_real[0].X, FloorMeshReinforcementRightPointForDimensions.Y), PointsFootingPad_real[6], false, true)); // Vertical Dimension - footing pad top surface to mesh - floor mesh cover

                if (opts.bDrawAnchors && anchorsToDraw != null && anchorControlPointsForDimensions != null && anchorControlPointsForDimensions.Count > 0) // Pridame koty pre anchors
                {
                    CAnchor firstAnchor = anchorsToDraw.First(); // First from right
                    Point cPoint = anchorControlPointsForDimensions.Last(); // Last from left
                    Point conctactWithFloorSlab = new Point(PointsFootingPad_real.Last().X, PointsFootingPad_real.Last().Y); // Posledny bod obrysu

                    Dimensions.Add(new CDimensionLinear(center, new Point(cPoint.X, cPoint.Y + firstAnchor.Length), new Point(cPoint.X, conctactWithFloorSlab.Y), false, true, 50)); // Vertical Dimension - first anchor
                    Dimensions.Add(new CDimensionLinear(center, new Point(cPoint.X, conctactWithFloorSlab.Y), cPoint, false, true, 50)); // Vertical Dimension - first anchor
                }

                // Horizontal Dimensions

                // Anchors
                if (opts.bDrawAnchors && anchorControlPointsForDimensions != null && anchorControlPointsForDimensions.Count > 0) // Pridame koty pre anchors
                {
                    for (int i = 0; i < anchorControlPointsForDimensions.Count; i++)
                    {
                        if (i == 0) // First dimension
                            Dimensions.Add(new CDimensionLinear(center, new Point(bottomLeft_ColumnEdge.X, anchorControlPointsForDimensions[i].Y), anchorControlPointsForDimensions[i], true, true, 20));
                        else
                            Dimensions.Add(new CDimensionLinear(center, anchorControlPointsForDimensions[i - 1], anchorControlPointsForDimensions[i], true, true, 20));
                    }
                }

                // Footing pad to column edge
                if (!MathF.d_equal(PointsFootingPad_real[5].X, bottomLeft_ColumnEdge.X)) // Ak hrana betonovej patky a oceloveho stlpa na lavej strane spolu licuju, tak tuto kotu nezobrazujeme
                {
                    if (opts.bDrawAnchors && anchorControlPointsForDimensions != null && anchorControlPointsForDimensions.Count > 0) // Zarovnanie kot do roviny
                        Dimensions.Add(new CDimensionLinear(center, new Point(PointsFootingPad_real[5].X, anchorControlPointsForDimensions[0].Y), new Point(bottomLeft_ColumnEdge.X, anchorControlPointsForDimensions[0].Y), true, true, 20)); // Horizontal Dimension - footing pad edge to column
                    else // Kota v pripade ze nekotujeme ziadne anchors
                        Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[5], new Point(bottomLeft_ColumnEdge.X, PointsFootingPad_real[5].Y), true, false, 20)); // Horizontal Dimension - footing pad edge to column
                }

                // Reinforcement
                if (opts.bDrawReinforcement)
                {
                    Dimensions.Add(new CDimensionLinear(center, new Point(PointsFootingPad_real[4].X, bottomReinforcementLeftPointForDimensions.Y), bottomReinforcementLeftPointForDimensions, false, true, 53)); // Horizontal Dimension - reinforcement cover left
                    Dimensions.Add(new CDimensionLinear(center, bottomReinforcementLeftPointForDimensions, bottomReinforcementRightPointForDimensions, false, true, 53)); // Horizontal Dimension - reinforcement length
                    Dimensions.Add(new CDimensionLinear(center, bottomReinforcementRightPointForDimensions, new Point(PointsFootingPad_real[3].X, bottomReinforcementRightPointForDimensions.Y), false, true, 53)); // Horizontal Dimension - reinforcement cover right
                }

                Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[4], PointsFootingPad_real[3], false, true, 50)); // Horizontal Dimension - footing pad width

                //canvasDimensions = MirrorYCoordinates(Dimensions.ToArray()); // Nezrkadlime body lebo uz boli zrkadlene pre vykreslenie patky atd
                List<CDimension> canvasDimensions = Dimensions;

                canvasDimensions = ConvertRealPointsToCanvasDrawingPoints(canvasDimensions, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                // Dimensions
                DrawDimensions(opts.bDrawDimensions, canvasDimensions, canvasForImage, opts.DimensionsLinesColor, opts.DimensionsTextColor, opts.DimensionsThickness, "Dimensions");
            }

            if (opts.bDrawNotes)
            {
                List<CNote2D> notes2D = new List<CNote2D>(); // Real

                Point center = new Point(0, 0); // TO Ondrej - toto by mal byt asi stred obrazku

                bool bDrawArrows = true;
                bool bDrawUnderLineBelowText = true;

                // TODO - zatial natvrdo nastavene parametre pre odkazovu ciaru
                double dVerticalOffsetOfText = 0.03; // m // Vertikalne odsadenie textu od ciary
                double dHorizontalProjectionOfArrow = 0.20; // m // TODO Ondrej - S tymto sa treba pohrat a urobit to rozne nastavitelne aby napriklad zacinali texty pekne pod sebou alebo sa neprekryvali vzajomne ani s nicim co je uz nakreslene
                double dVerticalProjectionOfArrow = 0.25; // m // TODO Ondrej - S tymto sa treba pohrat

                // Pozicia textu poznamok na pravej strane obrazku, hrana zalomenia floorSlab smerom do footing pad + 0.1 m
                double dNoteTextHorizontalPosition_x = PointsFootingPad_real[1].X + 0.1;  //0.4; // Absolutna pozicia konca sipky alebo bodu textu // Ak chceme zaciatky poznamok / textov pekne pod sebou
                //double dNoteTextHorizontalPosition_x_rel = 0.56; // To Ondrej tu nie je uplne dobre mat napevno relativnu poziciu, ale malo by tu byt nieco co sa viaze na pravu hranu patky (chcel by som aby boli poznamky vzdy napravo od tohto bodu a radsej urobit sirsi obrazok, aby donho vosli texty
                double dNoteTextHorizontalPosition_x_rel = (dNoteTextHorizontalPosition_x - fTempMin_X) / fModel_Length_x_real;

                bool bUseSameHorizontalPositions = true; // Vsetky notes u ktorych to bude zapnute mat rovnaku suradnicu x

                bool bUseEquallySpacedVerticalPositions = true; // Vytvorime zoznam pozicii pre poznamky nad a pod floor slab a postupne do nich umiestnujeme poznamky

                double dNotesVerticalOffset = 0.1f;
                double dNotesVerticalOffset_rel = 0.05f;

                // TODO Ondrej - skusam tu vyrobit pozicie pre poznamky v smere y, akokeby tabulku nad a pod floor slab
                // potom na zaklade indexov vkladam na jednotlive pozicie poznamky

                // TODO Ondrej - mozno by bolo lepsie nahradit (dNoteTextHorizontalPosition_x a notesVerticalPositions... priamo zoznamom Points)
                List<double> notesVerticalPositionsAboveFloor = new List<double>();
                List<double> notesVerticalPositionsBelowFloor = new List<double>();

                List<double> notesVerticalPositionsAboveFloor_rel = new List<double>();
                List<double> notesVerticalPositionsBelowFloor_rel = new List<double>();

                double dFirstNoteVerticalPositionsAboveFloor = PointsFootingPad_real[6].Y + 0.1f;
                double dFirstNoteVerticalPositionsBelowFloor = PointsFootingPad_real[0].Y - fRealOffset_DPC_DPM - 0.55f;

                double dFirstNoteVerticalPositionsAboveFloor_rel = 0.40f;
                double dFirstNoteVerticalPositionsBelowFloor_rel = 0.80f;

                // Naplnime zoznamy pozicii
                short numberOfNotePositions = 10;

                for (int i = 0; i < numberOfNotePositions; i++)
                {
                    notesVerticalPositionsAboveFloor.Add(dFirstNoteVerticalPositionsAboveFloor + i * dNotesVerticalOffset);
                    notesVerticalPositionsBelowFloor.Add(dFirstNoteVerticalPositionsBelowFloor - i * dNotesVerticalOffset);

                    // Pripravime si preklopene suradnice 
                    // MirrorAboutX_ChangeYCoordinates
                    notesVerticalPositionsAboveFloor[i] *= -1;
                    notesVerticalPositionsBelowFloor[i] *= -1;

                    notesVerticalPositionsAboveFloor_rel.Add(dFirstNoteVerticalPositionsAboveFloor_rel - i * dNotesVerticalOffset_rel);
                    notesVerticalPositionsBelowFloor_rel.Add(dFirstNoteVerticalPositionsBelowFloor_rel + i * dNotesVerticalOffset_rel);
                }

                // Tu sa nastavuje relativna pozicia textu v percentach (0- 1) 1.00 = 100%

                Point relativePoint;
                // Column Description
                bool bDrawColumnDescription = true;
                if (opts.bDrawColumnOutline && bDrawColumnDescription)
                {
                    Point pArrowStart = columnNotePoint;
                    double pTextPosition_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                    double pTextPosition_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[6] : pArrowStart.Y - dVerticalProjectionOfArrow;

                    relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[6]);

                    Point pArrowEnd = new Point(pTextPosition_x, pTextPosition_y);
                    Point pTextNote = new Point(pArrowEnd.X, pArrowEnd.Y - dVerticalOffsetOfText);
                    //notes2D.Add(new CNote2D(pTextNote, joint.m_MainMember.CrScStart.Name_short, bDrawArrows, pArrowStart, pArrowEnd, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                    notes2D.Add(new CNote2D(relativePoint, joint.m_MainMember.CrScStart.Name_short, bDrawArrows, pArrowStart, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                }

                // Anchors Description
                bool bDrawAnchorDescription = true;
                if (opts.bDrawAnchors && bDrawAnchorDescription)
                {
                    dVerticalProjectionOfArrow = 0.52; // m // TODO Ondrej - S tymto sa treba pohrat

                    Point pArrowStart = anchorNotePoint;
                    double pTextPosition_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                    double pTextPosition_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[4] : pArrowStart.Y - dVerticalProjectionOfArrow;

                    relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[4]);

                    Point pArrowEnd = new Point(pTextPosition_x, pTextPosition_y);
                    Point pTextNote = new Point(pArrowEnd.X, pArrowEnd.Y - dVerticalOffsetOfText);
                    // Sample text: 4 x M16 HD bolts 500 mm long
                    string sText = basePlate.AnchorArrangement.Anchors.Length.ToString() + " x M" + (anchorsToDraw.First().Diameter_shank * 1000).ToString("F0") + " HD bolts - " +
                                   (anchorsToDraw.First().Length * 1000).ToString("F0") + " mm long";

                    //notes2D.Add(new CNote2D(pTextNote, sText, bDrawArrows, pArrowStart, pArrowEnd, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));

                    notes2D.Add(new CNote2D(relativePoint, sText, bDrawArrows, pArrowStart, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));

                    bool bDrawAnchorTopWasherDescription = true;

                    if (bDrawAnchorTopWasherDescription)
                    {
                        dVerticalProjectionOfArrow = 0.40; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_AnchorTopWasher = plateWasherNotePoint;
                        double pTextPosition_AnchorTopWasher_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_AnchorTopWasher.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_AnchorTopWasher_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[5] : pArrowStart_AnchorTopWasher.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[5]);

                        Point pArrowEnd_AnchorTopWasher = new Point(pTextPosition_AnchorTopWasher_x, pTextPosition_AnchorTopWasher_y);
                        Point pTextNote_AnchorTopWasher = new Point(pArrowEnd_AnchorTopWasher.X, pArrowEnd_AnchorTopWasher.Y - dVerticalOffsetOfText);
                        // Sample text: M16 HT nut & 80sq x 12mm washer on top

                        string sText_AnchorTopWasher = "M" + (anchorsToDraw.First().Diameter_shank * 1000).ToString("F0") + " HT nut & " +
                            (fPlateWasherWidth_x * 1000).ToString("F0") + "x" +
                            (fPlateWasherWidth_y * 1000).ToString("F0") + "x" +
                            (fPlateWasherThickness * 1000).ToString("F0") + " mm washer on top";

                        //notes2D.Add(new CNote2D(pTextNote_AnchorTopWasher, sText_AnchorTopWasher,bDrawAnchorTopWasherDescription, pArrowStart_AnchorTopWasher, pArrowEnd_AnchorTopWasher, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));

                        notes2D.Add(new CNote2D(relativePoint, sText_AnchorTopWasher, bDrawAnchorTopWasherDescription, pArrowStart_AnchorTopWasher, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }

                    bool bDrawAnchorBottomWasherDescription = true;

                    if (bDrawAnchorBottomWasherDescription)
                    {
                        dHorizontalProjectionOfArrow = 0.30; // m // TODO Ondrej - S tymto sa treba pohrat a urobit to rozne nastavitelne aby napriklad zacinali texty pekne pod sebou alebo sa neprekryvali vzajomne ani s nicim co je uz nakreslene
                        dVerticalProjectionOfArrow = -0.50; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_AnchorBottomWasher = bearingWasherNotePoint;
                        double pTextPosition_AnchorBottomWasher_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_AnchorBottomWasher.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_AnchorBottomWasher_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsBelowFloor[3] : pArrowStart_AnchorBottomWasher.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsBelowFloor_rel[3]);

                        Point pArrowEnd_AnchorBottomWasher = new Point(pTextPosition_AnchorBottomWasher_x, pTextPosition_AnchorBottomWasher_y);
                        Point pTextNote_AnchorBottomWasher = new Point(pArrowEnd_AnchorBottomWasher.X, pArrowEnd_AnchorBottomWasher.Y - dVerticalOffsetOfText);
                        // Sample text: M16 Nuts & 60sq x 6mm washer at base

                        string sText_AnchorBottomWasher = "M" + (anchorsToDraw.First().Diameter_shank * 1000).ToString("F0") + " HT nut & " +
                            (fBearingWasherWidth_x * 1000).ToString("F0") + "x" +
                            (fBearingWasherWidth_y * 1000).ToString("F0") + "x" +
                            (fBearingWasherThickness * 1000).ToString("F0") + " mm washer at base";

                        //notes2D.Add(new CNote2D(pTextNote_AnchorBottomWasher, sText_AnchorBottomWasher, bDrawAnchorBottomWasherDescription, pArrowStart_AnchorBottomWasher, pArrowEnd_AnchorBottomWasher, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                        notes2D.Add(new CNote2D(relativePoint, sText_AnchorBottomWasher, bDrawAnchorBottomWasherDescription, pArrowStart_AnchorBottomWasher, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }
                }

                bool bDrawMeshDescription = true; // Kreslit poznamku
                bool bDrawArrowMeshDescription = true; // Kreslit sipku

                if (opts.bDrawReinforcementInSlab && bDrawMeshDescription)
                {
                    dHorizontalProjectionOfArrow = 0.20; // m // TODO Ondrej - S tymto sa treba pohrat a urobit to rozne nastavitelne aby napriklad zacinali texty pekne pod sebou alebo sa neprekryvali vzajomne ani s nicim co je uz nakreslene
                    dVerticalProjectionOfArrow = 0.10; // m // TODO Ondrej - S tymto sa treba pohrat

                    Point pArrowStart_Mesh = FloorMeshNotePoint;
                    double pTextPosition_Mesh_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_Mesh.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                    double pTextPosition_Mesh_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[0] : pArrowStart_Mesh.Y - dVerticalProjectionOfArrow;

                    relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[0]);

                    Point pArrowEnd_Mesh = new Point(pTextPosition_Mesh_x, pTextPosition_Mesh_y);
                    Point pTextNote_Mesh = new Point(pArrowEnd_Mesh.X, pArrowEnd_Mesh.Y - dVerticalOffsetOfText);
                    // Sample text: SE92 [500 Grade] Mesh

                    string sText_Mesh = floorSlab.MeshGradeName + " [" + floorSlab.RCMesh.m_Mat.Name + " Grade]" + " Mesh";

                    //notes2D.Add(new CNote2D(pTextNote_Mesh, sText_Mesh, bDrawArrowMeshDescription, pArrowStart_Mesh, pArrowEnd_Mesh, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                    notes2D.Add(new CNote2D(relativePoint, sText_Mesh, bDrawArrowMeshDescription, pArrowStart_Mesh, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    // Starter
                    bool bDrawStarterDescription = true; // Kreslit poznamku
                    bool bDrawArrowStarterDescription = true; // Kreslit sipku

                    if (bDrawStarterDescription)
                    {
                        dHorizontalProjectionOfArrow = 0.20; // m // TODO Ondrej - S tymto sa treba pohrat a urobit to rozne nastavitelne aby napriklad zacinali texty pekne pod sebou alebo sa neprekryvali vzajomne ani s nicim co je uz nakreslene
                        dVerticalProjectionOfArrow = 0.10; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_Starter = starter_NotePoint;
                        double pTextPosition_Starter_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_Starter.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_Starter_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[1] : pArrowStart_Starter.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[1]);

                        Point pArrowEnd_Starter = new Point(pTextPosition_Starter_x, pTextPosition_Starter_y);
                        Point pTextNote_Starter = new Point(pArrowEnd_Starter.X, pArrowEnd_Starter.Y - dVerticalOffsetOfText);
                        // Sample text: HD12 Starters / 600 mm crs.
                        string sText_Starter = "HD" + (/*floorSlab.fStartersDiameter*/ fStarterBarDiameter * 1000).ToString("F0") + " Starters / " + (/*floorSlab.stafStartersSpacing*/ startersSpacing * 1000).ToString("F0") + " mm crs";

                        //notes2D.Add(new CNote2D(pTextNote_Starter, sText_Starter, bDrawArrowStarterDescription, pArrowStart_Starter, pArrowEnd_Starter, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                        notes2D.Add(new CNote2D(relativePoint, sText_Starter, bDrawArrowStarterDescription, pArrowStart_Starter, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }
                }

                bool bDraw_dpc_dpm_Description = true; // Kreslit poznamku
                bool bDrawArrow_dpc_dpm_Description = true; // Kreslit sipku

                if (opts.bDrawDPC_DPM && bDraw_dpc_dpm_Description)
                {
                    dHorizontalProjectionOfArrow = 0.25; // m // TODO Ondrej - S tymto sa treba pohrat a urobit to rozne nastavitelne aby napriklad zacinali texty pekne pod sebou alebo sa neprekryvali vzajomne ani s nicim co je uz nakreslene
                    dVerticalProjectionOfArrow = -0.10; // m // TODO Ondrej - S tymto sa treba pohrat

                    Point pArrowStart_dpc_dpm = dpc_dpm_NotePoint;
                    double pTextPosition_dpc_dpm_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_dpc_dpm.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                    double pTextPosition_dpc_dpm_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsBelowFloor[0] : pArrowStart_dpc_dpm.Y - dVerticalProjectionOfArrow;

                    relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsBelowFloor_rel[0]);

                    Point pArrowEnd_dpc_dpm = new Point(pTextPosition_dpc_dpm_x, pTextPosition_dpc_dpm_y);
                    Point pTextNote_dpc_dpm = new Point(pArrowEnd_dpc_dpm.X, pArrowEnd_dpc_dpm.Y - dVerticalOffsetOfText);
                    // Sample text: DPC to underside

                    string sText_dpc_dpm = "DPC to underside";

                    //notes2D.Add(new CNote2D(pTextNote_dpc_dpm, sText_dpc_dpm, bDrawArrow_dpc_dpm_Description, pArrowStart_dpc_dpm, pArrowEnd_dpc_dpm, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                    notes2D.Add(new CNote2D(relativePoint, sText_dpc_dpm, bDrawArrow_dpc_dpm_Description, pArrowStart_dpc_dpm, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                }

                bool bDraw_reinforcement_Description = true; // Kreslit poznamku
                bool bDrawArrow_reinforcement_Description = true; // Kreslit sipku

                if (opts.bDrawReinforcement && bDraw_reinforcement_Description)
                {
                    dHorizontalProjectionOfArrow = 0.25; // m // TODO Ondrej - S tymto sa treba pohrat a urobit to rozne nastavitelne aby napriklad zacinali texty pekne pod sebou alebo sa neprekryvali vzajomne ani s nicim co je uz nakreslene

                    // TODO Ondrej - toto vykreslovanie poznamok by sa dalo refaktorovat pre tieto 4 pozicie vyztuze
                    // Navrhujem vytovrit jednu funkciu, ktora vykresli poznamku pre vyztuz, poslat jej objekt vyztuze, polohu NotePoint a odsadenia ciary

                    // Top_Bar_x
                    if (pad.Top_Bars_x != null && pad.Top_Bars_x.Count > 0)
                    {
                        dVerticalProjectionOfArrow = 0.22; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_RC_Top_x = reinforcement_top_x_NotePoint;
                        double pTextPosition_RC_Top_x_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_RC_Top_x.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_RC_Top_x_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[2] : pArrowStart_RC_Top_x.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[2]);

                        Point pArrowEnd_RC_Top_x = new Point(pTextPosition_RC_Top_x_x, pTextPosition_RC_Top_x_y);
                        Point pTextNote_RC_Top_x = new Point(pArrowEnd_RC_Top_x.X, pArrowEnd_RC_Top_x.Y - dVerticalOffsetOfText);
                        // Sample text: 5 x HD16 bars with standard hook each end

                        string sText_RC_Top_x = pad.Count_Top_Bars_x.ToString() + " x HD" + (pad.Top_Bars_x.First().Diameter * 1000).ToString("F0") + " bars with standard hook each end";

                        //notes2D.Add(new CNote2D(pTextNote_RC_Top_x, sText_RC_Top_x, bDrawArrow_reinforcement_Description, pArrowStart_RC_Top_x, pArrowEnd_RC_Top_x, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                        notes2D.Add(new CNote2D(relativePoint, sText_RC_Top_x, bDrawArrow_reinforcement_Description, pArrowStart_RC_Top_x, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }

                    // Top_Bar_y
                    if (pad.Top_Bars_y != null && pad.Top_Bars_y.Count > 0)
                    {
                        dVerticalProjectionOfArrow = 0.30; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_RC_Top_y = reinforcement_top_y_NotePoint;
                        double pTextPosition_RC_Top_y_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_RC_Top_y.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_RC_Top_y_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsAboveFloor[3] : pArrowStart_RC_Top_y.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsAboveFloor_rel[3]);

                        Point pArrowEnd_RC_Top_y = new Point(pTextPosition_RC_Top_y_x, pTextPosition_RC_Top_y_y);
                        Point pTextNote_RC_Top_y = new Point(pArrowEnd_RC_Top_y.X, pArrowEnd_RC_Top_y.Y - dVerticalOffsetOfText);
                        // Sample text: 5 x HD16 bars with standard hook each end

                        string sText_RC_Top_y = pad.Count_Top_Bars_y.ToString() + " x HD" + (pad.Top_Bars_y.First().Diameter * 1000).ToString("F0") + " bars with standard hook each end";

                        //notes2D.Add(new CNote2D(pTextNote_RC_Top_y, sText_RC_Top_y, bDrawArrow_reinforcement_Description, pArrowStart_RC_Top_y, pArrowEnd_RC_Top_y, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                        notes2D.Add(new CNote2D(relativePoint, sText_RC_Top_y, bDrawArrow_reinforcement_Description, pArrowStart_RC_Top_y, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }

                    // Bottom_Bar_x
                    if (pad.Bottom_Bars_x != null && pad.Bottom_Bars_x.Count > 0)
                    {
                        dVerticalProjectionOfArrow = -0.20; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_RC_Bottom_x = reinforcement_bottom_x_NotePoint;
                        double pTextPosition_RC_Bottom_x_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_RC_Bottom_x.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_RC_Bottom_x_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsBelowFloor[1] : pArrowStart_RC_Bottom_x.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsBelowFloor_rel[1]);

                        Point pArrowEnd_RC_Bottom_x = new Point(pTextPosition_RC_Bottom_x_x, pTextPosition_RC_Bottom_x_y);
                        Point pTextNote_RC_Bottom_x = new Point(pArrowEnd_RC_Bottom_x.X, pArrowEnd_RC_Bottom_x.Y - dVerticalOffsetOfText);
                        // Sample text: 5 x HD16 bars with standard hook each end

                        string sText_RC_Bottom_x = pad.Count_Bottom_Bars_x.ToString() + " x HD" + (pad.Bottom_Bars_x.First().Diameter * 1000).ToString("F0") + " bars with standard hook each end";

                        //notes2D.Add(new CNote2D(pTextNote_RC_Bottom_x, sText_RC_Bottom_x, bDrawArrow_reinforcement_Description, pArrowStart_RC_Bottom_x, pArrowEnd_RC_Bottom_x, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                        notes2D.Add(new CNote2D(relativePoint, sText_RC_Bottom_x, bDrawArrow_reinforcement_Description, pArrowStart_RC_Bottom_x, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }

                    // Bottom_Bar_y
                    if (pad.Bottom_Bars_y != null && pad.Bottom_Bars_y.Count > 0)
                    {
                        dVerticalProjectionOfArrow = -0.27; // m // TODO Ondrej - S tymto sa treba pohrat

                        Point pArrowStart_RC_Bottom_y = reinforcement_bottom_y_NotePoint;
                        double pTextPosition_RC_Bottom_y_x = bUseSameHorizontalPositions ? dNoteTextHorizontalPosition_x : pArrowStart_RC_Bottom_y.X + dHorizontalProjectionOfArrow;  // Pozicia konca sipky, resp bodu textu
                        double pTextPosition_RC_Bottom_y_y = bUseEquallySpacedVerticalPositions ? notesVerticalPositionsBelowFloor[2] : pArrowStart_RC_Bottom_y.Y - dVerticalProjectionOfArrow;

                        relativePoint = new Point(dNoteTextHorizontalPosition_x_rel, notesVerticalPositionsBelowFloor_rel[2]);

                        Point pArrowEnd_RC_Bottom_y = new Point(pTextPosition_RC_Bottom_y_x, pTextPosition_RC_Bottom_y_y);
                        Point pTextNote_RC_Bottom_y = new Point(pArrowEnd_RC_Bottom_y.X, pArrowEnd_RC_Bottom_y.Y - dVerticalOffsetOfText);
                        // Sample text: 5 x HD16 bars with standard hook each end

                        string sText_RC_Bottom_y = pad.Count_Bottom_Bars_y.ToString() + " x HD" + (pad.Bottom_Bars_y.First().Diameter * 1000).ToString("F0") + " bars with standard hook each end";

                        //notes2D.Add(new CNote2D(pTextNote_RC_Bottom_y, sText_RC_Bottom_y, bDrawArrow_reinforcement_Description, pArrowStart_RC_Bottom_y, pArrowEnd_RC_Bottom_y, center, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right));
                        notes2D.Add(new CNote2D(relativePoint, sText_RC_Bottom_y, bDrawArrow_reinforcement_Description, pArrowStart_RC_Bottom_y, relativePoint, center, opts.NotesArrowFillColor, opts.NotesArrowStrokeColor, bDrawUnderLineBelowText, VerticalAlignment.Center, HorizontalAlignment.Right, 12, true, opts.NotesThickness));
                    }
                }

                List<CNote2D> canvasNotes2D = notes2D;
                canvasNotes2D = ConvertRealPointsToCanvasDrawingPoints(canvasNotes2D, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor, width, height);

                // Draw Notes
                DrawNotes(opts.bDrawNotes, canvasNotes2D, canvasForImage);
            }
        }

        public static Stream GetCanvasStream(Canvas canvas)
        {
            RenderTargetBitmap bmp = RenderVisual(canvas);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));

            MemoryStream stm = new MemoryStream();
            png.Save(stm);
            stm.Position = 0;

            return stm;
        }
        private static RenderTargetBitmap RenderVisual(UIElement elt)
        {
            Size size = new Size(elt.RenderSize.Width, elt.RenderSize.Height);
            elt.Measure(size);
            elt.Arrange(new Rect(size));
            elt.UpdateLayout();

            var bitmap = new RenderTargetBitmap(
                (int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);

            bitmap.Render(elt);
            return bitmap;
        }

        private static CNote2D GetNoteForPlate(CPlate plate)
        {
            CNote2D note2D = null;
            if (plate is CConCom_Plate_KB || plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KB kb = (CConCom_Plate_KB)plate;
                Point plateCenter = Drawing2D.CalculateModelCenter(kb.PointsOut2D);
                if (kb.pTip != null)
                {
                    double moveX = Math.Abs(kb.pTip.X) < 1 ? kb.pTip.X / 10 : kb.pTip.X / 18;
                    double moveY = Math.Abs(kb.pTip.Y) < 1 ? kb.pTip.Y / 10 : kb.pTip.Y / 25;
                    note2D = new CNote2D(new Point(kb.pTip.X + moveX, kb.pTip.Y + moveY), "Trim Off", false, kb.pTip, new Point(kb.pTip.X + 40, kb.pTip.Y + 40), plateCenter, Brushes.Black, Brushes.Black);
                }
            }
            else if (plate is CConCom_Plate_KC || plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KC kc = (CConCom_Plate_KC)plate;
                Point plateCenter = Drawing2D.CalculateModelCenter(kc.PointsOut2D);
                if (kc.pTip != null)
                {
                    double moveX = Math.Abs(kc.pTip.X) < 1 ? kc.pTip.X / 10 : kc.pTip.X / 18;
                    double moveY = Math.Abs(kc.pTip.Y) < 1 ? kc.pTip.Y / 10 : kc.pTip.Y / 25;
                    note2D = new CNote2D(new Point(kc.pTip.X + moveX, kc.pTip.Y + moveY), "Trim Off", false, kc.pTip, new Point(kc.pTip.X + 40, kc.pTip.Y + 40), plateCenter, Brushes.Black, Brushes.Black);
                }
            }
            else if (plate is CConCom_Plate_KD || plate is CConCom_Plate_KDS)
            {
                CConCom_Plate_KD kd = (CConCom_Plate_KD)plate;
                Point plateCenter = Drawing2D.CalculateModelCenter(kd.PointsOut2D);
                if (kd.pTip != null)
                {
                    double moveX = Math.Abs(kd.pTip.X) < 1 ? kd.pTip.X / 10 : kd.pTip.X / 18;
                    double moveY = Math.Abs(kd.pTip.Y) < 1 ? kd.pTip.Y / 10 : kd.pTip.Y / 25;
                    note2D = new CNote2D(new Point(kd.pTip.X + moveX, kd.pTip.Y + moveY), "Trim Off", false, kd.pTip, new Point(kd.pTip.X + 40, kd.pTip.Y + 40), plateCenter, Brushes.Black, Brushes.Black);
                }
            }
            else if (plate is CConCom_Plate_KES)
            {
                CConCom_Plate_KES ke = (CConCom_Plate_KES)plate;
                Point plateCenter = Drawing2D.CalculateModelCenter(ke.PointsOut2D);
                if (ke.pTip != null && ke.FSlope_rad < 0) // Display note only for falling knee
                {
                    double moveX = Math.Abs(ke.pTip.X) < 1 ? ke.pTip.X / 10 : ke.pTip.X / 18;
                    double moveY = Math.Abs(ke.pTip.Y) < 1 ? ke.pTip.Y / 10 : ke.pTip.Y / 25;
                    note2D = new CNote2D(new Point(ke.pTip.X + moveX, ke.pTip.Y + moveY), "Trim Off", false, ke.pTip, new Point(ke.pTip.X + 40, ke.pTip.Y + 40), plateCenter, Brushes.Black, Brushes.Black);
                }
            }
            return note2D;
        }

        public static void DrawScrewToCanvas(CScrew screw, double width, double height, ref Canvas canvasForImage, bool bDrawCentreSymbol)
        {
            float fScaleFactor = 0.5f; // 50% of canvas
            int scale_unit = 1000; // mm

            double dModel_Length_x_page;
            double dModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            double dReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginTop_y;
            float fmodelMarginBottom_y;

            CalculateBasicValue(
                screw.D_w_washerdiameter,
                screw.D_w_washerdiameter,
                fScaleFactor,
                scale_unit,
                width,
                height,
                out dModel_Length_x_page,
                out dModel_Length_y_page,
                out dFactor_x,
                out dFactor_y,
                out dReal_Model_Zoom_Factor,
                out fmodelMarginLeft_x,
                out fmodelMarginTop_y,
                out fmodelMarginBottom_y
                );

            Point pCenterPoint = new Point(width / 2, height / 2);

            // Head Inside Circle
            DrawCircle(pCenterPoint, dReal_Model_Zoom_Factor * screw.D_h_headdiameter, Brushes.Black, null, 1, canvasForImage);

            // Head Hexagon
            float a = (0.5f * screw.D_h_headdiameter) / (float)Math.Cos(30f / 180f * Math.PI);
            List<Point> headpoints = Geom2D.GetHexagonPointCoord(a); // Diameter of outside circle

            // TODO - upravit podla toho ci bude v databaze vnutorny alebo vonkajsi rozmer sesthrannej hlavy (opisana alebo vpisana kruznica)
            float fInsideDiameterFactor = 0.5f / (float)Math.Tan(30f / 180f * Math.PI); // Radius of inside circle of hexagon

            double dCanvasTop = (height - (dReal_Model_Zoom_Factor * screw.D_h_headdiameter)) / 2;
            double dCanvasLeft = (width - (dReal_Model_Zoom_Factor * 2 * a/* fInsideDiameterFactor * screw.D_h_headdiameter*/)) / 2;
            DrawPolyLine(true, headpoints, dCanvasTop, dCanvasLeft, fmodelMarginLeft_x, fmodelMarginBottom_y, dReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);

            // Washer Circle
            DrawCircle(pCenterPoint, dReal_Model_Zoom_Factor * screw.D_w_washerdiameter, Brushes.Black, null, 1, canvasForImage);

            // Draw Symbol of Center
            if (bDrawCentreSymbol)
                DrawSymbol_Cross(pCenterPoint, dReal_Model_Zoom_Factor * screw.D_w_washerdiameter + 20, Brushes.Red, 1, canvasForImage);
        }

        public static void DrawComponent(bool bDrawPoints,
            bool bDrawOutLine,
            bool bDrawPointNumbers,
            bool bDrawHoles,
            bool bDrawHoleCentreSymbols,
            bool bDrawDrillingRoute,
            bool bDrawDimensions,
            bool bDrawMemberOutline,
            bool bDrawBendLines,
            List<Point> PointsOut,
            List<Point> PointsIn,
            Point[] PointsHolesScrews,
            Point[] PointsHolesAnchors,
            List<Point> PointsDrillingRoute,
            CDimension[] Dimensions,
            CLine2D[] MemberOutline,
            CLine2D[] BendLines,
            CNote2D note2D,
            double dHolesDiameterScrews,
            double dHolesDiameterAnchors,
            float fmodelMarginLeft_x,
            float fmodelMarginTop_y,
            double dReal_Model_Zoom_Factor,
            double fModel_Length_y_page,
            double dPointInOutDistance_y_page,
            double dPointInOutDistance_x_page,
            bool bPointsHaveYinUpDirection,
            Canvas canvasForImage)
        {
            List<Point> canvasPointsOut = null;
            List<Point> canvasPointsIn = null;
            List<Point> canvasPointsHolesScrews = null;
            List<Point> canvasPointsHolesAnchors = null;
            List<Point> canvasPointsDrillingRoute = null;
            List<CDimension> canvasDimensions = null;
            List<CLine2D> canvasMemberOutline = null;
            List<CLine2D> canvasBendLines = null;
            CNote2D canvasNote2D = null;

            if (bPointsHaveYinUpDirection)
            {
                canvasPointsOut = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsOut);
                canvasPointsIn = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsIn);
                canvasPointsHolesScrews = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsHolesScrews);
                canvasPointsHolesAnchors = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsHolesAnchors);
                canvasPointsDrillingRoute = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsDrillingRoute);
                canvasDimensions = MirrorAboutX_ChangeYCoordinates(Dimensions, false);
                canvasMemberOutline = MirrorAboutX_ChangeYCoordinates(MemberOutline, false);
                canvasBendLines = MirrorAboutX_ChangeYCoordinates(BendLines, false);
                if (note2D != null) note2D.MirrorAboutX_ChangeYCoordinates();
            }
            else
            {
                canvasPointsOut = new List<Point>(PointsOut);
                canvasPointsIn = new List<Point>(PointsIn);
                canvasPointsHolesScrews = new List<Point>(PointsHolesScrews);
                canvasPointsHolesAnchors = new List<Point>(PointsHolesAnchors);
                canvasPointsDrillingRoute = new List<Point>(PointsDrillingRoute);

                // Oprava prepisu dat pre plate ak spustim export opakovane                
                canvasDimensions = ModelHelper.GetClonedDimensions(Dimensions);
                canvasMemberOutline = ModelHelper.GetClonedLines(MemberOutline);
                canvasBendLines = ModelHelper.GetClonedLines(BendLines);                
            }

            double minX = canvasPointsOut.Min(p => p.X);
            double minY = canvasPointsOut.Min(p => p.Y);

            canvasPointsOut = ConvertRealPointsToCanvasDrawingPoints(canvasPointsOut, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsIn = ConvertRealPointsToCanvasDrawingPoints(canvasPointsIn, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsHolesScrews = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHolesScrews, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsHolesAnchors = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHolesAnchors, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsDrillingRoute = ConvertRealPointsToCanvasDrawingPoints(canvasPointsDrillingRoute, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasDimensions = ConvertRealPointsToCanvasDrawingPoints(canvasDimensions, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasMemberOutline = ConvertRealPointsToCanvasDrawingPoints(canvasMemberOutline, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasBendLines = ConvertRealPointsToCanvasDrawingPoints(canvasBendLines, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasNote2D = ConvertRealPointsToCanvasDrawingPoints(note2D, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

            // Definition Points
            DrawComponentPoints(bDrawPoints, canvasPointsOut, canvasPointsIn, canvasForImage, "Points");

            // Outlines
            DrawOutlines(bDrawOutLine, canvasPointsOut, canvasPointsIn, canvasForImage, "Outlines");

            // Definition Point Numbers
            DrawPointNumbers(bDrawPointNumbers, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Holes
            if (PointsHolesScrews != null)
            {
                DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesScrews, Brushes.Black, Brushes.Red, 1, 1, dHolesDiameterScrews, canvasForImage, "Holes", "Hole Center Symbols");
                DrawDrillingRoute(bDrawDrillingRoute, canvasPointsDrillingRoute, canvasForImage);
            }

            if (PointsHolesAnchors != null)
            {
                DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesAnchors, Brushes.Black, Brushes.Red, 1, 1, dHolesDiameterAnchors, canvasForImage, "Anchor Holes", "Anchor Hole Center Symbols");
            }

            // Dimensions
            DrawDimensions(bDrawDimensions, canvasDimensions, canvasForImage, Brushes.DarkGreen, Brushes.DarkGreen, 1, "Dimensions");

            // Member Outline
            DrawSeparateLines(bDrawMemberOutline, canvasMemberOutline, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, "Member Outline");

            // Bend Lines
            DrawSeparateLines(bDrawBendLines, canvasBendLines, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, "Bend Lines");

            //Notes
            if (note2D != null) DrawNote(canvasNote2D, canvasForImage);
        }

        private static List<CDimension> MirrorAboutX_ChangeYCoordinates(CDimension[] Dimensions, bool changeOriginalObject = true)
        {
            if (Dimensions == null) return new List<CDimension>();
            List<CDimension> listDimensions = new List<CDimension>();

            if (changeOriginalObject)
            {
                listDimensions = new List<CDimension>(Dimensions);
                foreach (CDimension d in listDimensions)
                {
                    d.MirrorYCoordinates();
                }
            }
            else
            {
                foreach (CDimension d in Dimensions)
                {
                    CDimension dimensionClone = d.GetClonedDimension();
                    dimensionClone.MirrorYCoordinates();
                    listDimensions.Add(dimensionClone);
                }
            }

            return listDimensions;
        }

        private static List<CLine2D> MirrorAboutX_ChangeYCoordinates(CLine2D[] lines, bool changeOriginalObject = true)
        {
            if (lines == null) return new List<CLine2D>();
            List<CLine2D> listLines = new List<CLine2D>();
            if (changeOriginalObject)
            {
                listLines = new List<CLine2D>(lines);
                foreach (CLine2D l in listLines)
                {
                    l.MirrorYCoordinates();
                }
            }
            else
            {
                foreach (CLine2D l in lines)
                {
                    CLine2D lineClone = l.Clone();
                    lineClone.MirrorYCoordinates();
                    listLines.Add(lineClone);
                }
            }
            return listLines;
        }

        private static List<Point> ConvertRealPointsToCanvasDrawingPoints(List<Point> points, double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            if (points == null) return new List<Point>();

            List<Point> updatedPoints = new List<Point>(points.Count);
            foreach (Point p in points)
            {
                Point point = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (p.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (p.Y - minY));
                updatedPoints.Add(point);
            }
            return updatedPoints;
        }
        private static Point ConvertRealPointToCanvasDrawingPoint(Point point, double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            return new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (point.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (point.Y - minY));
        }
        private static List<CDimension> ConvertRealPointsToCanvasDrawingPoints(List<CDimension> dimensions, double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            if (dimensions == null) return new List<CDimension>();

            List<CDimension> updatedDimensions = new List<CDimension>(dimensions);
            foreach (CDimension d in updatedDimensions)
            {
                d.UpdatePoints(minX, minY, modelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            }
            return updatedDimensions;
        }
        private static CNote2D ConvertRealPointsToCanvasDrawingPoints(CNote2D note2D, double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            if (note2D == null) return note2D;

            note2D.UpdatePoints(minX, minY, modelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            return note2D;
        }
        private static List<CNote2D> ConvertRealPointsToCanvasDrawingPoints(List<CNote2D> notes, double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor,
            double canvasWidth, double canvasHeight)
        {
            if (notes == null) return new List<CNote2D>();

            List<CNote2D> updatedNotes = new List<CNote2D>(notes);
            foreach (CNote2D n in updatedNotes)
            {
                n.UpdatePoints(minX, minY, modelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                n.SetRelativePoints(canvasWidth, canvasHeight);
            }
            return updatedNotes;
        }
        private static List<CLine2D> ConvertRealPointsToCanvasDrawingPoints(List<CLine2D> lines, double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            if (lines == null) return new List<CLine2D>();

            List<CLine2D> updatedLines = new List<CLine2D>(lines);
            foreach (CLine2D l in updatedLines)
            {
                l.UpdatePoints(minX, minY, modelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            }
            return updatedLines;
        }

        public static void CalculateBasicValue(
            double fTempMax_X,
            double fTempMin_X,
            double fTempMax_Y,
            double fTempMin_Y,
            double dScale_Factor, // 0-1
            int scale_unit,
            double dPageWidth,
            double dPageHeight,
            List<Point> PointsIn,
            double dPointInOutDistance_x_real,
            double dPointInOutDistance_y_real,
            out double dModel_Length_x_real,
            out double dModel_Length_y_real,
            out double dModel_Length_x_page,
            out double dModel_Length_y_page,
            out double dFactor_x,
            out double dFactor_y,
            out double dReal_Model_Zoom_Factor,
            out float fmodelMarginLeft_x,
            out float fmodelMarginTop_y,
            out double dPointInOutDistance_x_page,
            out double dPointInOutDistance_y_page
            )
        {
            dModel_Length_x_real = fTempMax_X - fTempMin_X;
            dModel_Length_y_real = fTempMax_Y - fTempMin_Y;

            CalculateBasicValue_ZoomFactor(
                dModel_Length_x_real,
                dModel_Length_y_real,
                dScale_Factor, // zoom ratio 0-1 (zoom of 2D view), default 0.8 (80%)
                scale_unit,
                dPageWidth,
                dPageHeight,
                out dModel_Length_x_page,
                out dModel_Length_y_page,
                out dFactor_x,
                out dFactor_y,
                out dReal_Model_Zoom_Factor);

            // Set new size of model on the page
            dModel_Length_x_page = dReal_Model_Zoom_Factor * dModel_Length_x_real;
            dModel_Length_y_page = dReal_Model_Zoom_Factor * dModel_Length_y_real;

            fmodelMarginLeft_x = (float)(0.5 * (dPageWidth - dModel_Length_x_page));
            fmodelMarginTop_y = (float)(0.5 * (dPageHeight - dModel_Length_y_page));

            dPointInOutDistance_x_page = 0;
            dPointInOutDistance_y_page = 0;

            if (PointsIn != null && PointsIn.Count > 0)
            {
                dPointInOutDistance_x_page = dPointInOutDistance_x_real * dReal_Model_Zoom_Factor;
                dPointInOutDistance_y_page = dPointInOutDistance_y_real * dReal_Model_Zoom_Factor;
            }
        }

        // TODO Ondrej - tento variant funkcie sa pouziva len na vykreslenie skrutky, chcelo by to prerobit a zjednotit
        // je potrebne zrusit fmodelMarginBottom_y a prerobit funkciu na vykreslovanie skrutky tak aby sa zadavalo fmodelMarginTop_y
        public static void CalculateBasicValue(
            float fModel_Length_x_real,
            float fModel_Length_y_real,
            double dScale_Factor, // zoom ratio 0-1 (zoom of 2D view), default 0.8 (80%)
            int scale_unit,
            double dPageWidth,
            double dPageHeight,
            out double dModel_Length_x_page,
            out double dModel_Length_y_page,
            out double dFactor_x,
            out double dFactor_y,
            out double dReal_Model_Zoom_Factor,
            out float fmodelMarginLeft_x,
            out float fmodelMarginTop_y,
            out float fmodelMarginBottom_y  // Treba rorzlisovat fmodelMarginBottom_y a fmodelBottomPosition_y a vycistit to
            )
        {
            CalculateBasicValue_ZoomFactor(
                fModel_Length_x_real,
                fModel_Length_y_real,
                dScale_Factor, // zoom ratio 0-1 (zoom of 2D view), default 0.8 (80%)
                scale_unit,
                dPageWidth,
                dPageHeight,
                out dModel_Length_x_page,
                out dModel_Length_y_page,
                out dFactor_x,
                out dFactor_y,
                out dReal_Model_Zoom_Factor);

            // Set new size of model on the page
            dModel_Length_x_page = dReal_Model_Zoom_Factor * fModel_Length_x_real;
            dModel_Length_y_page = dReal_Model_Zoom_Factor * fModel_Length_y_real;

            fmodelMarginLeft_x = (float)(0.5 * (dPageWidth - dModel_Length_x_page));
            fmodelMarginTop_y = (float)(0.5 * (dPageHeight - dModel_Length_y_page));

            // TODO Ondrej - zrusit toto odsadenie a prerobit vsade na top margin
            fmodelMarginBottom_y = (float)(dModel_Length_y_page + 0.5 * (dPageHeight - dModel_Length_y_page));
        }

        public static void CalculateBasicValue_ZoomFactor(
        double dModel_Length_x_real,
        double dModel_Length_y_real,
        double dScale_Factor, // zoom ratio 0-1 (zoom of 2D view), default 0.8 (80%)
        int scale_unit,
        double dPageWidth,
        double dPageHeight,
        out double dModel_Length_x_page,
        out double dModel_Length_y_page,
        out double dFactor_x,
        out double dFactor_y,
        out double dReal_Model_Zoom_Factor)
        {
            dModel_Length_x_page = scale_unit * dModel_Length_x_real;
            dModel_Length_y_page = scale_unit * dModel_Length_y_real;

            // Calculate maximum zoom factor
            // Original ratio
            dFactor_x = dModel_Length_x_page / dPageWidth;
            dFactor_y = dModel_Length_y_page / dPageHeight;

            // Calculate new model dimensions (zoom of model is defined by factor 0.0 - 1.0), default 0.8 (80%)
            dReal_Model_Zoom_Factor = dScale_Factor / (float)Math.Max(dFactor_x, dFactor_y) * scale_unit;
        }

        //public static void DrawPoints(bool bDrawPoints, List<Point> PointsOut, List<Point> PointsIn, float modelMarginLeft_x, float modelMarginBottom_y, float fReal_Model_Zoom_Factor, Canvas canvasForImage)
        //{
        //    if (bDrawPoints)
        //    {
        //        // Outer outline points
        //        if (PointsOut != null) // If is array of points not empty
        //        {
        //            for (int i = 0; i < PointsOut.Count; i++)
        //            {
        //                DrawPoint(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i].X, modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i].Y), Brushes.Red, Brushes.Red, 4, canvasForImage);
        //            }
        //        }

        //        // Internal outline points
        //        if (PointsIn != null) // If is array of points not empty
        //        {
        //            for (int i = 0; i < PointsIn.Count; i++)
        //            {
        //                DrawPoint(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i].X, modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i].Y), Brushes.Red, Brushes.Red, 4, canvasForImage);
        //            }
        //        }
        //    }
        //}


        //refaktoring metody hore, spravi sa kopia poli, suradnice sa upravia na kladne
        //public static void DrawPoints(bool bDrawPoints, List<Point> PointsOut, List<Point> PointsIn, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, Canvas canvasForImage)
        //{
        //    if (bDrawPoints)
        //    {
        //        List<Point> points = new List<Point>();                
        //        if (PointsOut != null) points.AddRange(PointsOut);
        //        if (PointsIn != null) points.AddRange(PointsIn);

        //        double minX = points.Min(p => p.X);
        //        double minY = points.Min(p => p.Y);
        //        foreach (Point p in points)
        //        {
        //            Point point = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (p.X - minX), modelMarginBottom_y - dReal_Model_Zoom_Factor * (p.Y - minY));
        //            DrawPoint(point, Brushes.Red, Brushes.Red, 4, canvasForImage);
        //        }
        //    }
        //}
        public static void DrawComponentPoints(bool bDrawPoints, List<Point> PointsOut, List<Point> PointsIn, Canvas canvasForImage, string name = "")
        {
            if (bDrawPoints)
            {
                DrawPoints(bDrawPoints, PointsOut, canvasForImage, name);
                DrawPoints(bDrawPoints, PointsIn, canvasForImage, name);
            }
        }
        public static void DrawPoints(bool bDrawPoints, List<Point> points, Canvas canvasForImage, string name)
        {
            foreach (Point p in points)
            {
                DrawPoint(p, Brushes.Red, Brushes.Red, 4, canvasForImage, name);
            }

        }



        //public static void DrawOutlines(bool bDrawOutLine, bool bUsePolylineforDrawing, List<Point> PointsOut, List<Point> PointsIn, double dReal_Model_Zoom_Factor, float modelMarginBottom_y, float modelMarginLeft_x, double fModel_Length_y_page, double dPointInOutDistance_y_page, double dPointInOutDistance_x_page, Canvas canvasForImage)
        //{
        //    if (bDrawOutLine)
        //    {
        //        if (bUsePolylineforDrawing)
        //        {
        //            // Outer outline lines
        //            if (PointsOut != null) // If is array of points not empty
        //            {
        //                // TODO - Ondrej - bug No. 113, ked sa pouzije zakomentovany kod line 469 a 470, + odkomentuje sa to co je na 473, tak sa to v niektorych pripadoch upravi, ale nie je to dokonale, napr. pre KE plate
        //                // Bolo by super aby si si nasiel cas a to vykreslovanie nejako prerobil, aby to bolo univerzalnejsie bez ohladu na to ci su suradnice v bodov plechu len kladne alebo aj zaporne a podobne
        //                // Pripadne vyrobit kopiu poli bodov, ktore sa pouziju pre vykreslovanie, vsetko posunut tak, aby minimum bolo [0,0], pripadne rovno aj posunut aby bolo minimum vlavo hore a sedelo to so systemom pre vykreslovanie
        //                // Ten chaos vznikol tak ze som chcel mat povodne lavy spodny roh plechu v [0,0] ale neuvedomil som si ze to moze mat potom aj zaporne suradnice a teda ze [0,0] nebude minimum a odvtedy sa v tom vykreslovani trosku patlam :)
        //                // a lepim blbe na blbsie :)

        //                /*
        //                double fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y;
        //                CalculateModelLimits(PointsOut, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
        //                */
        //                double fCanvasTop = modelMarginBottom_y - fModel_Length_y_page;
        //                double fCanvasLeft = modelMarginLeft_x /*+ fTempMin_X * fReal_Model_Zoom_Factor*/;

        //                DrawPolyLine(true, PointsOut, fCanvasTop, fCanvasLeft, modelMarginLeft_x, modelMarginBottom_y, dReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
        //            }

        //            // Internal outline lines
        //            if (PointsIn != null) // If is array of points not empty
        //            {
        //                double fCanvasTop = modelMarginBottom_y - fModel_Length_y_page + dPointInOutDistance_y_page;
        //                double fCanvasLeft = modelMarginLeft_x + dPointInOutDistance_x_page;
        //                DrawPolyLine(true, PointsIn, fCanvasTop, fCanvasLeft, modelMarginLeft_x, modelMarginBottom_y, dReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
        //            }
        //        }
        //        else
        //        {
        //            // ToDo kreslenie po liniach nefunguje spravne, asi je problem s tymito canvas vo funkcii DrawLine
        //            // Canvas.SetTop(myLine, line.Y1);
        //            // Canvas.SetLeft(myLine, line.X1);

        //            // Outer outline lines
        //            if (PointsOut != null) // If is array of points not empty
        //            {
        //                for (int i = 0; i < PointsOut.Count; i++)
        //                {
        //                    // Add a Line
        //                    Line l = new Line();

        //                    l.X1 = modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsOut[i].X;
        //                    l.Y1 = modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsOut[i].Y;

        //                    if (i < (PointsOut.Count - 1))
        //                    {
        //                        l.X2 = modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsOut[i + 1].X;
        //                        l.Y2 = modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsOut[i + 1].Y;
        //                    }
        //                    else
        //                    {
        //                        l.X2 = modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsOut[0].X;
        //                        l.Y2 = modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsOut[0].Y;
        //                    }

        //                    DrawLine(l, Brushes.Black, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
        //                }
        //            }

        //            // Internal outline lines
        //            if (PointsIn != null) // If is array of points not empty
        //            {
        //                for (int i = 0; i < PointsIn.Count; i++)
        //                {
        //                    // Add a Line
        //                    Line l = new Line();
        //                    l.X1 = modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsIn[i].X;
        //                    l.Y1 = modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsIn[i].Y;

        //                    if (i < (PointsIn.Count - 1))
        //                    {
        //                        l.X2 = modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsIn[i + 1].X;
        //                        l.Y2 = modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsIn[i + 1].Y;
        //                    }
        //                    else
        //                    {
        //                        l.X2 = modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsIn[0].X;
        //                        l.Y2 = modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsIn[0].Y;
        //                    }

        //                    DrawLine(l, Brushes.Black, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void DrawOutlines(bool bDrawOutLine, List<Point> PointsOut, List<Point> PointsIn, Canvas canvasForImage, string name)
        {
            if (bDrawOutLine)
            {
                DrawPolyLine(true, PointsOut, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, name);
                DrawPolyLine(true, PointsIn, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, name);
            }
        }

        //public static void DrawPointNumbers(bool bDrawPointNumbers, List<Point> PointsOut, List<Point> PointsIn, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, Canvas canvasForImage)
        //{
        //    if (bDrawPointNumbers)
        //    {
        //        // Outer outline points
        //        if (PointsOut != null) // If is array of points not empty
        //        {
        //            for (int i = 0; i < PointsOut.Count; i++)
        //            {
        //                DrawText((i + 1).ToString(), modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsOut[i].X, modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsOut[i].Y,0, 16, false, Brushes.Blue, canvasForImage);
        //            }
        //        }

        //        // Internal outline points
        //        if (PointsIn != null && PointsOut != null) // If is array of points not empty
        //        {
        //            for (int i = 0; i < PointsIn.Count; i++)
        //            {
        //                DrawText((/*crsc.INoPointsOut +*/ i + 1).ToString(), modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsIn[i].X, modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsIn[i].Y,0, 16, false, Brushes.Green, canvasForImage);
        //            }
        //        }
        //    }
        //}

        public static void DrawPointNumbers(bool bDrawPointNumbers, List<Point> PointsOut, List<Point> PointsIn, Canvas canvasForImage, string name = "")
        {
            if (bDrawPointNumbers)
            {
                // Outer outline points
                if (PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsOut.Count; i++)
                    {
                        DrawText((i + 1).ToString(), PointsOut[i].X, PointsOut[i].Y, 0, 16, false, Brushes.Blue, canvasForImage, name);
                    }
                }

                // Internal outline points
                if (PointsIn != null && PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsIn.Count; i++)
                    {
                        DrawText((i + 1).ToString(), PointsIn[i].X, PointsIn[i].Y, 0, 16, false, Brushes.Green, canvasForImage, name);
                    }
                }
            }
        }

        //public static void DrawHoles(bool bDrawHoles, bool bDrawHoleCentreSymbols, List<Point> PointsHoles, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, float scale_unit, double DHolesDiameter, Canvas canvasForImage)
        //{
        //    if (bDrawHoles)
        //    {
        //        // Holes
        //        if (PointsHoles != null) // If is array of holes centers is not empty
        //        {
        //            for (int i = 0; i < PointsHoles.Count; i++)
        //            {
        //                // Draw Hole
        //                DrawCircle(new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsHoles[i].X, modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsHoles[i].Y), scale_unit * DHolesDiameter, Brushes.Black, 1, canvasForImage);

        //                // Draw Symbol of Center
        //                if (bDrawHoleCentreSymbols)
        //                    DrawSymbol_Cross(new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * PointsHoles[i].X, modelMarginBottom_y - dReal_Model_Zoom_Factor * PointsHoles[i].Y), scale_unit * DHolesDiameter + 10, Brushes.Red, 1, canvasForImage);
        //            }
        //        }
        //    }
        //}

        public static void DrawHoles(bool bDrawHoles, bool bDrawHoleCentreSymbols, List<Point> PointsHoles, SolidColorBrush ColorHoles, SolidColorBrush ColorHoleCenterSymbols, double ThicknessHoleLine, double ThicknessHoleCenterSymbolLine, double dHolesDiameter, Canvas canvasForImage, string nameHoles = "", string nameHoleCentreSymbols = "", double SymbolOffsetFromHoleCircle = 5)
        {
            if (bDrawHoles)
            {
                // Holes
                if (PointsHoles != null) // If is array of holes centers is not empty
                {
                    for (int i = 0; i < PointsHoles.Count; i++)
                    {
                        // Draw Hole
                        DrawCircle(PointsHoles[i], dHolesDiameter, Brushes.Black, null, ThicknessHoleLine, canvasForImage, nameHoles);

                        // Draw Symbol of Center
                        if (bDrawHoleCentreSymbols) DrawSymbol_Cross(PointsHoles[i], dHolesDiameter + 2 * SymbolOffsetFromHoleCircle, ColorHoleCenterSymbols, ThicknessHoleCenterSymbolLine, canvasForImage, nameHoleCentreSymbols);
                    }
                }
            }
        }

        //public static void DrawDrillingRoute(bool bDrawDrillingRoute, List<Point> PointsDrillingRoute, double dReal_Model_Zoom_Factor, float modelMarginLeft_x, float modelMarginBottom_y, Canvas canvasForImage)
        //{
        //    if (!bDrawDrillingRoute || PointsDrillingRoute == null) return;
        //    // ??? TODO upravit odsadenie

        //    double fx_min = double.MaxValue;
        //    double fy_min = double.MaxValue;
        //    double fx_max = double.MinValue;
        //    double fy_max = double.MinValue;

        //    for (int i = 0; i < PointsDrillingRoute.Count; i++)
        //    {
        //        if (PointsDrillingRoute[i].X < fx_min)
        //            fx_min = PointsDrillingRoute[i].X;

        //        if (PointsDrillingRoute[i].Y < fy_min)
        //            fy_min = PointsDrillingRoute[i].Y;

        //        if (PointsDrillingRoute[i].X > fx_max)
        //            fx_max = PointsDrillingRoute[i].X;

        //        if (PointsDrillingRoute[i].Y > fy_max)
        //            fy_max = PointsDrillingRoute[i].Y;
        //    }

        //    fx_min *= dReal_Model_Zoom_Factor;
        //    fy_min *= dReal_Model_Zoom_Factor;
        //    fx_max *= dReal_Model_Zoom_Factor;
        //    fy_max *= dReal_Model_Zoom_Factor;

        //    double fCanvasTop = modelMarginBottom_y - fy_max;
        //    double fCanvasLeft = modelMarginLeft_x + fx_min;

        //    if (PointsDrillingRoute != null)
        //        DrawPolyLine(false, PointsDrillingRoute, fCanvasTop, fCanvasLeft, modelMarginLeft_x, modelMarginBottom_y, dReal_Model_Zoom_Factor, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
        //}

        public static void DrawDrillingRoute(bool bDrawDrillingRoute, List<Point> PointsDrillingRoute, Canvas canvasForImage, string name = "")
        {
            if (!bDrawDrillingRoute || PointsDrillingRoute == null) return;

            DrawPolyLine(false, PointsDrillingRoute, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage, name);
        }

        //public static void DrawDimensions(bool bDrawDimensions, CDimension[] Dimensions, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, Canvas canvasForImage)
        //{
        //    if (bDrawDimensions && Dimensions != null && Dimensions.Length > 0)
        //    {
        //        for (int i = 0; i < Dimensions.Length; i++) // Pole kot
        //        {
        //            if (Dimensions[i] is CDimensionLinear)
        //            {
        //                CDimensionLinear dim = (CDimensionLinear)Dimensions[i];
        //                DrawSimpleLinearDimension(dim.ControlPointStart, dim.ControlPointEnd, 30, true, dim.IsTextAboveLineBetweenExtensionLines, modelMarginLeft_x, modelMarginBottom_y, dReal_Model_Zoom_Factor, canvasForImage);
        //            }
        //            else if(Dimensions[i] is CDimensionArc)
        //            {
        //                CDimensionArc dim = (CDimensionArc)Dimensions[i];
        //                DrawArcDimension(dim.ControlPointStart, dim.ControlPointEnd, dim.ControlPointCenter, modelMarginLeft_x, modelMarginBottom_y, dReal_Model_Zoom_Factor, canvasForImage);
        //            }
        //            else
        //            {
        //                // Not defined drawing function
        //            }
        //        }
        //    }
        //}

        public static void DrawReincementBar_U_Shape(
            CReinforcementBar_U bar,
            CFoundation pad,
            Point pControlPoint, // Left top point of footing pad
            float fArcNetRadius, // m
            bool bIsTopBar, // true - horna vystuz (preklapame U)
            SolidColorBrush colorBrush,
            double fTempMin_X,
            double fTempMin_Y,
            float fmodelMarginLeft_x,
            float fmodelMarginTop_y,
            double dReal_Model_Zoom_Factor,
            Canvas canvasForImage,
            out Point reinforcement_NotePoint)
        {
            float fBarDiameter = bar.Diameter;
            float fArcRadius = fArcNetRadius + 0.5f * fBarDiameter;

            float fHorizontalStraightPartLength = pad.m_fDim2 - 2 * (pad.ConcreteCover + 0.5f * fBarDiameter + fArcRadius);

            Point start = new Point(pad.ConcreteCover + 0.5f * fBarDiameter, -pad.ConcreteCover);
            Point leftArcStart = new Point(start.X, start.Y - (pad.m_fDim3 - pad.ConcreteCover - pad.ConcreteCover - 0.5f * fBarDiameter - fArcRadius));
            Point leftArcEnd = new Point(start.X + fArcRadius, leftArcStart.Y - fArcRadius);
            Point rightArcStart = new Point(leftArcEnd.X + fHorizontalStraightPartLength, leftArcEnd.Y);
            Point rightArcEnd = new Point(rightArcStart.X + fArcRadius, rightArcStart.Y + fArcRadius);
            Point end = new Point(pad.m_fDim2 - pad.ConcreteCover - 0.5 * fBarDiameter, start.Y);

            // Presunut body
            List<Point> listOfPoints = new List<Point>() { start, leftArcStart, leftArcEnd, rightArcStart, rightArcEnd, end };

            //Point pControlPoint = new Point(horizontalOffset, 0); // Vkladaci bod vyztuze (lavy horny roh patky)
            for (int i = 0; i < listOfPoints.Count; i++)
            {
                Point aux = new Point(listOfPoints[i].X + pControlPoint.X, listOfPoints[i].Y + pControlPoint.Y); // Presun

                if (bIsTopBar) // Preklopenie pre hornu vystuz
                {
                    Point aux2 = new Point(aux.X, aux.Y);
                    aux = new Point(aux.X, -(pad.m_fDim3 + aux2.Y));
                }

                Geom2D.MirrorAboutX_ChangeYCoordinates(ref aux); // Odzrkadlenie
                aux = ConvertRealPointToCanvasDrawingPoint(aux, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor); // Konverzia na zobrazovacie jednotky
                listOfPoints[i] = aux;
            }

            double reinforcement_NotePoint_Position_x = pControlPoint.X + rightArcStart.X - 0.05 * fHorizontalStraightPartLength;
            double reinforcement_NotePoint_Position_y = pControlPoint.Y + rightArcStart.Y;

            reinforcement_NotePoint = new Point(reinforcement_NotePoint_Position_x, reinforcement_NotePoint_Position_y);

            if (bIsTopBar)
                reinforcement_NotePoint = new Point(reinforcement_NotePoint_Position_x, -(pad.m_fDim3 + reinforcement_NotePoint_Position_y));

            Geom2D.MirrorAboutX_ChangeYCoordinates(ref reinforcement_NotePoint);

            double dArcRadiusInCanvas = fArcRadius * dReal_Model_Zoom_Factor;
            double dBarDiameterInCanvas = fBarDiameter * dReal_Model_Zoom_Factor;

            PathSegmentCollection listOfSegments = new PathSegmentCollection();

            Point startPoint = listOfPoints[0];

            SweepDirection sDirection = SweepDirection.Counterclockwise;

            if (bIsTopBar)
                sDirection = SweepDirection.Clockwise;

            LineSegment verticalSegment_Left = new LineSegment(listOfPoints[1], true);
            ArcSegment leftArc = new ArcSegment(listOfPoints[2], new Size(dArcRadiusInCanvas, dArcRadiusInCanvas), 90, false, sDirection, true);
            LineSegment horizontalSegment = new LineSegment(listOfPoints[3], true);
            ArcSegment rightArc = new ArcSegment(listOfPoints[4], new Size(dArcRadiusInCanvas, dArcRadiusInCanvas), 90, false, sDirection, true);
            LineSegment verticalSegment_Right = new LineSegment(listOfPoints[5], true);

            listOfSegments.Add(verticalSegment_Left);
            listOfSegments.Add(leftArc);
            listOfSegments.Add(horizontalSegment);
            listOfSegments.Add(rightArc);
            listOfSegments.Add(verticalSegment_Right);

            DrawPathFigure(startPoint, listOfSegments, false, colorBrush, dBarDiameterInCanvas, canvasForImage);
        }

        public static void DrawReincementBar_Starter_Shape(
            CFoundation pad,
            CSlab slab,
            Point pControlPoint, // Left top point of footing pad
            float fArcNetRadius, // m
            SolidColorBrush colorBrush,
            double fTempMin_X,
            double fTempMin_Y,
            float fmodelMarginLeft_x,
            float fmodelMarginTop_y,
            double dReal_Model_Zoom_Factor,
            Canvas canvasForImage,
            out Point starterNotePoint,
            out float fBarDiameter,
            out float startersSpacing)
        {
            // Reinforcement bar type starter
            // Kreslime od spodneho haku smerom nahor a doprava, takze v smere hodinovych ruciciek

            // TODO - sem potrebujeme dostat rozmery starter pre danu stranu floor slab kde sa nachadza patka (left/right, front/back)

            // Left or right side of building
            fBarDiameter = slab.Starters_Phi_LRSide;
            float fTopPartProjectionLength = slab.StartersLapLength_LRSide;
            startersSpacing = slab.StartersSpacing_LRSide;

            // Front or back side
            if (pad.m_ColumnMemberTypePosition == EMemberType_FS_Position.ColumnFrontSide || pad.m_ColumnMemberTypePosition == EMemberType_FS_Position.ColumnBackSide)
            {
                fBarDiameter = slab.Starters_Phi_FBSide;
                fTopPartProjectionLength = slab.StartersLapLength_FBSide;
                startersSpacing = slab.StartersSpacing_FBSide;
            }

            float fArcRadius = fArcNetRadius + 0.5f * fBarDiameter;

            // Horna priama cast
            float fTopStraightPartLength = fTopPartProjectionLength - fArcRadius;

            // Spodny hak - zahnutie
            float fHookStraightPartLength = 5 * fBarDiameter;
            float fHookAngleFromVerticalAxis_deg = 30;
            float fHookAngleFromVerticalAxis_rad = fHookAngleFromVerticalAxis_deg / 180f * MathF.fPI;

            float fHookStraightPart_Projection_x = fHookStraightPartLength * (float)Math.Sin(fHookAngleFromVerticalAxis_rad);
            float fHookStraightPart_Projection_y = fHookStraightPartLength * (float)Math.Cos(fHookAngleFromVerticalAxis_rad);

            Point pHookCenterPoint = new Point(pad.ConcreteCover + fBarDiameter + fArcNetRadius, -(pad.m_fDim3 - pad.ConcreteCover - fBarDiameter - fArcNetRadius));

            Point bottomArcStart = new Point(pHookCenterPoint.X + fArcRadius * (float)Math.Cos(fHookAngleFromVerticalAxis_rad), pHookCenterPoint.Y - fArcRadius * (float)Math.Sin(fHookAngleFromVerticalAxis_rad));
            Point start = new Point(bottomArcStart.X + fHookStraightPart_Projection_x, bottomArcStart.Y + fHookStraightPart_Projection_y);
            Point bottomArcEnd = new Point(pHookCenterPoint.X - fArcRadius, pHookCenterPoint.Y);
            Point topArcStart = new Point(bottomArcEnd.X, -slab.ConcreteCover - fBarDiameter - fArcNetRadius);
            Point toptArcEnd = new Point(topArcStart.X + fArcRadius, topArcStart.Y + fArcRadius);
            Point end = new Point(toptArcEnd.X + fTopStraightPartLength, toptArcEnd.Y);

            // Presunut body
            List<Point> listOfPoints = new List<Point>() { start, bottomArcStart, bottomArcEnd, topArcStart, toptArcEnd, end };

            //Point pControlPoint = new Point(horizontalOffset, 0); // Vkladaci bod vyztuze (lavy horny roh patky)
            for (int i = 0; i < listOfPoints.Count; i++)
            {
                Point aux = new Point(listOfPoints[i].X + pControlPoint.X, listOfPoints[i].Y + pControlPoint.Y); // Presun

                Geom2D.MirrorAboutX_ChangeYCoordinates(ref aux); // Odzrkadlenie
                aux = ConvertRealPointToCanvasDrawingPoint(aux, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor); // Konverzia na zobrazovacie jednotky
                listOfPoints[i] = aux;
            }

            starterNotePoint = new Point(pControlPoint.X + end.X - 0.01 * fTopStraightPartLength, pControlPoint.Y + end.Y);
            Geom2D.MirrorAboutX_ChangeYCoordinates(ref starterNotePoint);

            double dArcRadiusInCanvas = fArcRadius * dReal_Model_Zoom_Factor;
            double dBarDiameterInCanvas = fBarDiameter * dReal_Model_Zoom_Factor;

            PathSegmentCollection listOfSegments = new PathSegmentCollection();

            Point startPoint = listOfPoints[0];

            SweepDirection sDirection = SweepDirection.Clockwise;

            LineSegment hookSegment = new LineSegment(listOfPoints[1], true);
            ArcSegment bottomArc = new ArcSegment(listOfPoints[2], new Size(dArcRadiusInCanvas, dArcRadiusInCanvas), 90 + (90 - fHookAngleFromVerticalAxis_deg), false, sDirection, true);
            LineSegment verticalSegment = new LineSegment(listOfPoints[3], true);
            ArcSegment topArc = new ArcSegment(listOfPoints[4], new Size(dArcRadiusInCanvas, dArcRadiusInCanvas), 90, false, sDirection, true);
            LineSegment horizontalSegment = new LineSegment(listOfPoints[5], true);

            listOfSegments.Add(hookSegment);
            listOfSegments.Add(bottomArc);
            listOfSegments.Add(verticalSegment);
            listOfSegments.Add(topArc);
            listOfSegments.Add(horizontalSegment);

            DrawPathFigure(startPoint, listOfSegments, false, colorBrush, dBarDiameterInCanvas, canvasForImage);
        }

        public static void DrawPathFigure(Point start, PathSegmentCollection listOfSegments, bool bIsPathClosed, SolidColorBrush brush, double strokeThickness, Canvas canvasForImage)
        {
            PathFigure spline = new PathFigure(start, listOfSegments, bIsPathClosed);

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(spline);

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path();
            myPath.Stroke = brush;
            myPath.StrokeThickness = strokeThickness;
            myPath.Data = myPathGeometry;

            canvasForImage.Children.Add(myPath);
        }

        public static void DrawDimensions(bool bDrawDimensions, List<CDimension> Dimensions, Canvas canvasForImage, SolidColorBrush linesColor, SolidColorBrush textColor, double thickness, string name = "")
        {
            if (bDrawDimensions && Dimensions != null && Dimensions.Count > 0)
            {
                for (int i = 0; i < Dimensions.Count; i++) // Pole kot
                {
                    if (Dimensions[i] is CDimensionLinear)
                    {
                        CDimensionLinear dim = (CDimensionLinear)Dimensions[i];
                        DrawSimpleLinearDimension(dim, true, canvasForImage, linesColor, textColor, thickness, name);
                    }
                    else if (Dimensions[i] is CDimensionArc)
                    {
                        CDimensionArc dim = (CDimensionArc)Dimensions[i];
                        DrawArcDimension(dim.ControlPointStart, dim.ControlPointEnd, dim.ControlPointCenter, canvasForImage, name);
                    }
                    else
                    {
                        // Not defined drawing function
                    }
                }
            }
        }

        public static void DrawNote(CNote2D note, Canvas canvasForImage)
        {
            double lineThickness = 1;

            double textWidth;
            DrawText(note.Text, note.NoteTextPoint.X, note.NoteTextPoint.Y, note.FontSize, note.Valign, note.Halign, Brushes.Black, canvasForImage, out textWidth);

            if (note.DrawArrow)
            {
                PointCollection points = new PointCollection() { new Point(note.ArrowPoint1.X, note.ArrowPoint1.Y), new Point(note.ArrowPoint2.X, note.ArrowPoint2.Y) };
                DrawArrow(points, canvasForImage, new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black), lineThickness, ArrowEnds.Begin, 30);
            }
            if (note.DrawLineUnderText)
            {
                PointCollection points = new PointCollection() { new Point(note.ArrowPoint2.X, note.ArrowPoint2.Y), new Point(note.ArrowPoint2.X + textWidth, note.ArrowPoint2.Y) };
                DrawArrow(points, canvasForImage, new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black), lineThickness, ArrowEnds.None, 30);
            }
        }

        public static void DrawNotes(bool bDrawNotes, List<CNote2D> Notes, Canvas canvasForImage)
        {
            if (bDrawNotes && Notes != null && Notes.Count > 0)
            {
                for (int i = 0; i < Notes.Count; i++) // Zoznam poznamok
                {
                    CNote2D note = Notes[i];
                    DrawNote(note, canvasForImage);
                }
            }
        }

        public static void DrawSeparateLines(bool bDrawLines, List<CLine2D> lines, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas canvasForImage, string name = "")
        {
            if (bDrawLines && lines != null && lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++) // Pole ciar
                {
                    if (!Double.IsNaN(lines[i].X1) && !Double.IsNaN(lines[i].Y1) && !Double.IsNaN(lines[i].X2) && !Double.IsNaN(lines[i].Y2))
                    {
                        Line l = new Line();

                        l.X1 = lines[i].X1;
                        l.Y1 = lines[i].Y1;

                        l.X2 = lines[i].X2;
                        l.Y2 = lines[i].Y2;

                        DoubleCollection dashes = new DoubleCollection();
                        dashes.Add(10); dashes.Add(10);
                        DrawLine(l, color, startCap, endCap, thickness, canvasForImage, DashStyles.Dash, dashes, name);
                    }
                }
            }
        }

        public static void DrawPoint(Point point, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas, string name = "")
        {
            DrawRectangle(strokeColor, fillColor, thickness, imageCanvas, new Point(point.X - 0.5 * thickness, point.Y - 0.5 * thickness), new Point(point.X + 0.5 * thickness, point.Y + 0.5 * thickness), name);
        }

        public static void DrawLine(Line line, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas, DashStyle dashStyle, DoubleCollection dashArray = null, string name = "")
        {
            //Random r = new Random();
            //Color randomcolor = Color.FromArgb((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
            //SolidColorBrush b = new SolidColorBrush(randomcolor);

            Line myLine = new Line();
            myLine.Tag = name;
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.X1 = line.X1;
            myLine.X2 = line.X2;
            myLine.Y1 = line.Y1;
            myLine.Y2 = line.Y2;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;

            if (dashStyle != null)
            {
                if (dashArray != null) myLine.StrokeDashArray = dashArray;
                else myLine.StrokeDashArray = dashStyle.Dashes;
            }

            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(myLine, Math.Min(line.Y1, line.Y2));
            Canvas.SetLeft(myLine, Math.Min(line.X1, line.X2));
            imageCanvas.Children.Add(myLine);
        }

        //public static void DrawPolyLine(bool bIsClosed, float[,] arrPoints, double dCanvasTopTemp, double dCanvasLeftTemp, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        //{
        //    PointCollection points = new PointCollection();

        //    int iNumberOfLineSegments = arrPoints.Length / 2 + (bIsClosed ? 1 : 0);

        //    for (int i = 0; i < iNumberOfLineSegments; i++)
        //    {
        //        if (i < ((arrPoints.Length / 2)))
        //            points.Add(new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * arrPoints[i, 0], modelMarginBottom_y - dReal_Model_Zoom_Factor * arrPoints[i, 1]));
        //        else
        //            points.Add(new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * arrPoints[0, 0], modelMarginBottom_y - dReal_Model_Zoom_Factor * arrPoints[0, 1])); // Last point is same as first one
        //    }

        //    Polyline myLine = new Polyline();
        //    myLine.Stretch = Stretch.Fill;
        //    myLine.Stroke = color;
        //    myLine.Points = points;
        //    myLine.StrokeThickness = thickness;
        //    myLine.StrokeStartLineCap = startCap;
        //    myLine.StrokeEndLineCap = endCap;
        //    //myLine.HorizontalAlignment = HorizontalAlignment.Left;
        //    //myLine.VerticalAlignment = VerticalAlignment.Center;
        //    Canvas.SetTop(myLine, dCanvasTopTemp);
        //    Canvas.SetLeft(myLine, dCanvasLeftTemp);
        //    imageCanvas.Children.Add(myLine);
        //}
        public static void DrawPolyLine(bool bIsClosed, List<Point> listPoints, double dCanvasTopTemp, double dCanvasLeftTemp, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor,
            SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas, string name = "")
        {
            if (listPoints == null) return;
            if (listPoints.Count < 2) return;

            List<Point> points = new List<Point>();
            int iNumberOfLineSegments = listPoints.Count + (bIsClosed ? 1 : 0);

            for (int i = 0; i < iNumberOfLineSegments; i++)
            {
                if (i < ((listPoints.Count)))
                    points.Add(new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * listPoints[i].X, modelMarginBottom_y - dReal_Model_Zoom_Factor * listPoints[i].Y));
                else
                    points.Add(new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * listPoints[0].X, modelMarginBottom_y - dReal_Model_Zoom_Factor * listPoints[0].Y)); // Last point is same as first one
            }

            Polyline myLine = new Polyline();
            myLine.Tag = name;
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = new PointCollection(points);
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;

            Canvas.SetTop(myLine, dCanvasTopTemp);
            Canvas.SetLeft(myLine, dCanvasLeftTemp);
            imageCanvas.Children.Add(myLine);
        }
        public static void DrawPolyLine(bool bIsClosed, List<Point> listPoints, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas, string name = "", DashStyle dashStyle = null, DoubleCollection dashArray = null)
        {
            if (listPoints == null) return;
            if (listPoints.Count < 2) return;

            double canvasLeft = listPoints.Min(p => p.X);
            double canvasTop = listPoints.Min(p => p.Y);

            PointCollection points = new PointCollection(listPoints);
            if (bIsClosed) points.Add(listPoints.ElementAt(0));

            Polyline myLine = new Polyline();
            myLine.Tag = name;
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;

            if (dashStyle != null)
            {
                if (dashArray != null) myLine.StrokeDashArray = dashArray;
                else myLine.StrokeDashArray = dashStyle.Dashes;
            }

            Canvas.SetTop(myLine, canvasTop);
            Canvas.SetLeft(myLine, canvasLeft);
            imageCanvas.Children.Add(myLine);
        }

        public static void DrawArrow(PointCollection points, Canvas imageCanvas, SolidColorBrush fillColor, SolidColorBrush strokeColor, double thickness = 2, ArrowEnds arrowEnds = ArrowEnds.Start, double arrowAngle = 45)
        {
            ArrowPolyline arrow = new ArrowPolyline();
            arrow.Stroke = strokeColor;
            arrow.StrokeThickness = thickness;
            arrow.Fill = fillColor;
            arrow.ArrowEnds = arrowEnds;
            arrow.ArrowAngle = arrowAngle;
            arrow.Points = points;
            imageCanvas.Children.Add(arrow);
        }

        public static void DrawPolygon(List<Point> listPoints, SolidColorBrush fill_color, SolidColorBrush stroke_color, PenLineCap startCap, PenLineCap endCap, double thickness, double opacity, Canvas imageCanvas)
        {
            if (listPoints == null) return;
            if (listPoints.Count < 2) return;

            double canvasLeft = listPoints.Min(p => p.X);
            double canvasTop = listPoints.Min(p => p.Y);

            Polygon polygon = new Polygon();
            polygon.Stretch = Stretch.Fill;
            polygon.Stroke = stroke_color;
            polygon.Fill = fill_color;
            polygon.Points = new PointCollection(listPoints);
            polygon.StrokeThickness = thickness;
            polygon.StrokeStartLineCap = startCap;
            polygon.StrokeEndLineCap = endCap;
            polygon.Opacity = opacity;

            Canvas.SetTop(polygon, canvasTop);
            Canvas.SetLeft(polygon, canvasLeft);
            imageCanvas.Children.Add(polygon);
        }
        public static void DrawCircle(Point center, double diameter, SolidColorBrush colorStroke, SolidColorBrush colorFill, double thickness, Canvas imageCanvas, string name = "")
        {
            if (!Double.IsNaN(center.X))
            {
                Ellipse circle = new Ellipse();
                circle.Tag = name;
                circle.Height = diameter;
                circle.Width = diameter;
                circle.StrokeThickness = thickness;
                circle.Stroke = colorStroke;
                if (colorFill != null)
                    circle.Fill = colorFill;

                double left = center.X - (diameter / 2) + thickness / 2;
                double top = center.Y - (diameter / 2) + thickness / 2;
                Canvas.SetLeft(circle, left);
                Canvas.SetTop(circle, top);
                imageCanvas.Children.Add(circle);
            }
        }

        public static void DrawSymbol_Cross(Point center, double size, SolidColorBrush color, double thickness, Canvas imageCanvas, string name = "")
        {
            if (!Double.IsNaN(center.X)) // Check that value is not "NaN" - TODO - Ondrej Bug No. 109
            {
                Line l = new Line();
                l.Tag = name;

                double fSideLength = 0.5f * size;

                // Horizontal
                l.X1 = center.X - fSideLength;
                l.Y1 = center.Y;

                l.X2 = center.X + fSideLength;
                l.Y2 = center.Y;

                DrawLine(l, color, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas, DashStyles.Solid, null, name);

                l.X1 = center.X;
                l.Y1 = center.Y - fSideLength;

                l.X2 = center.X;
                l.Y2 = center.Y + fSideLength;

                DrawLine(l, color, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas, DashStyles.Solid, null, name);
            }
        }

        //public static void DrawSimpleLinearDimension(Point pStart, Point pEnd, float fOffsetFromOrigin, bool bDrawExtensionLines, bool bIsTextAboveControlPoint, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, Canvas imageCanvas)
        //{
        //    bool bBasicDimensionIsDefinedInPlusY = true;

        //    // kontrolne body su v y = 0, kota je v smere +x, kladna osa y smeruje dole
        //    // TRUE
        //    //
        //    //   |                                |
        //    //   |              Text              |
        //    //   /--------------------------------/

        //    // FALSE
        //    //
        //    //                  Text
        //    //   /--------------------------------/
        //    //   |                                |
        //    //   |                                |

        //    float fDirectionFactor = bBasicDimensionIsDefinedInPlusY ? 1f : -1f;

        //    double dRotation_rad = Math.Atan((pEnd.Y - pStart.Y) / (pEnd.X - pStart.X));
        //    double dRotation_deg = dRotation_rad / MathF.fPI * 180;

        //    double fBasicLength_m = Math.Sqrt(MathF.Pow2(pEnd.X - pStart.X) + MathF.Pow2(pEnd.Y - pStart.Y)); // Real length between points
        //    float fUnitFactor_mTomm = 1000;
        //    int iNumberOfDecimalPlaces = 0;
        //    string sText = (Math.Round(fBasicLength_m * fUnitFactor_mTomm, iNumberOfDecimalPlaces)).ToString();

        //    double fBasicLength_DisplayUnits = dReal_Model_Zoom_Factor * fBasicLength_m; // Length of displayed primary line

        //    double dLengtOfExtensionLineStartToPrimary = 0.8 * fOffsetFromOrigin;
        //    double dLengtOfExtensionLinePrimaryToEnd = 5; // Points
        //    double dOffsetOfExtensionLineFromPoint = 0.2 * fOffsetFromOrigin;

        //    double dLengtOfExtensionLineTotal = dLengtOfExtensionLineStartToPrimary + dLengtOfExtensionLinePrimaryToEnd;

        //    double dPrimaryLineThickness = 1;
        //    Line lPrimaryLine = new Line();
        //    lPrimaryLine.X1 = 0;
        //    lPrimaryLine.Y1 = fOffsetFromOrigin * fDirectionFactor;
        //    lPrimaryLine.X2 = fBasicLength_DisplayUnits;
        //    lPrimaryLine.Y2 = fOffsetFromOrigin * fDirectionFactor;

        //    // Extension lines
        //    double dExtensionLineThickness = 1;

        //    Line lExtensionLine1 = new Line();
        //    lExtensionLine1.X1 = 0;
        //    lExtensionLine1.Y1 = dOffsetOfExtensionLineFromPoint * fDirectionFactor;
        //    lExtensionLine1.X2 = 0;
        //    lExtensionLine1.Y2 = (dOffsetOfExtensionLineFromPoint + dLengtOfExtensionLineTotal) * fDirectionFactor;

        //    Line lExtensionLine2 = new Line();
        //    lExtensionLine2.X1 = fBasicLength_DisplayUnits;
        //    lExtensionLine2.Y1 = dOffsetOfExtensionLineFromPoint * fDirectionFactor;
        //    lExtensionLine2.X2 = fBasicLength_DisplayUnits;
        //    lExtensionLine2.Y2 = (dOffsetOfExtensionLineFromPoint + dLengtOfExtensionLineTotal) * fDirectionFactor;

        //    // Slope Symbol Lines
        //    double dSlopeLineLength = 10;
        //    double dSlopeLineThickness = 1;

        //    double coord = 0.5 * dSlopeLineLength / Math.Sqrt(2);

        //    Line lSlopeLine1 = new Line();
        //    lSlopeLine1.X1 = -coord;
        //    lSlopeLine1.Y1 = (fOffsetFromOrigin + coord) * fDirectionFactor;
        //    lSlopeLine1.X2 = coord;
        //    lSlopeLine1.Y2 = (fOffsetFromOrigin - coord) * fDirectionFactor;

        //    Line lSlopeLine2 = new Line();
        //    lSlopeLine2.X1 = fBasicLength_DisplayUnits - coord;
        //    lSlopeLine2.Y1 = (fOffsetFromOrigin + coord) * fDirectionFactor;
        //    lSlopeLine2.X2 = fBasicLength_DisplayUnits + coord;
        //    lSlopeLine2.Y2 = (fOffsetFromOrigin - coord) * fDirectionFactor;

        //    // Text transformation
        //    // ScaleTransform
        //    // SkewTransform
        //    // TranslateTransform
        //    // DropShadowBitmapEffect

        //    /* // TODO - tieto transformacie mi akosi nefunguju

        //    // Rotate dimension
        //    RotateTransform r1 = new RotateTransform(45);
        //    // Translate dimension
        //    TranslateTransform t1 = new TranslateTransform(pStart.X * fReal_Model_Zoom_Factor, pStart.Y * fReal_Model_Zoom_Factor);

        //    TransformGroup group = new TransformGroup();
        //    group.Children.Add(r1);
        //    group.Children.Add(t1);

        //    // Transform each part of dimension
        //    lPrimaryLine.RenderTransform = group;
        //    lExtensionLine1.RenderTransform = group;
        //    lExtensionLine2.RenderTransform = group;
        //    lSlopeLine1.RenderTransform = group;
        //    lSlopeLine2.RenderTransform = group;
        //    //Text.RenderTransform = group;
        //    */

        //    RotateAndTranslateDimension(pStart, pEnd, modelMarginLeft_x, modelMarginBottom_y, dReal_Model_Zoom_Factor, dRotation_rad, ref lPrimaryLine, ref lExtensionLine1, ref lExtensionLine2, ref lSlopeLine1, ref lSlopeLine2);

        //    // Urcuje sa z uz transformovanych suradnice lPrimaryLine
        //    double fTextPositionx = lPrimaryLine.X1 + 0.5 * (lPrimaryLine.X2 - lPrimaryLine.X1); // TODO - osetrit znamienka
        //    double fTextPositiony = lPrimaryLine.Y1 + 0.5 * (lPrimaryLine.Y2 - lPrimaryLine.Y1);

        //    // Draw dimension line
        //    DrawLine(lPrimaryLine, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dPrimaryLineThickness, imageCanvas);
        //    // Draw extension line - start
        //    DrawLine(lExtensionLine1, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas);
        //    // Draw extension line - end
        //    DrawLine(lExtensionLine2, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas);
        //    // Draw slope line - start
        //    DrawLine(lSlopeLine1, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas);
        //    // Draw slope line - end
        //    DrawLine(lSlopeLine2, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas);
        //    // Draw text
        //    DrawText(sText, fTextPositionx, fTextPositiony, -dRotation_deg, 12, bIsTextAboveControlPoint, Brushes.DarkGreen, imageCanvas);
        //}

        //public static void RotateAndTranslateDimension(Point pStart, Point pEnd, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, double dRotation_rad, ref Line lPrimaryLine, ref Line lExtensionLine1, ref Line lExtensionLine2, ref Line lSlopeLine1, ref Line lSlopeLine2)
        //{
        //    //double dRotation_rad = Math.Atan((pEnd.Y - pStart.Y) / (pEnd.X - pStart.X));
        //    float fOffset_x = (float)(modelMarginLeft_x + dReal_Model_Zoom_Factor * Math.Min(pStart.X, pEnd.X));
        //    float fOffset_y = (float)(modelMarginBottom_y - dReal_Model_Zoom_Factor * Math.Min(pStart.Y, pEnd.Y));

        //    RotateAndTranslateLine_CW(fOffset_x, fOffset_y, dRotation_rad, ref lPrimaryLine);
        //    RotateAndTranslateLine_CW(fOffset_x, fOffset_y, dRotation_rad, ref lExtensionLine1);
        //    RotateAndTranslateLine_CW(fOffset_x, fOffset_y, dRotation_rad, ref lExtensionLine2);
        //    RotateAndTranslateLine_CW(fOffset_x, fOffset_y, dRotation_rad, ref lSlopeLine1);
        //    RotateAndTranslateLine_CW(fOffset_x, fOffset_y, dRotation_rad, ref lSlopeLine2);
        //}

        public static void DrawSimpleLinearDimension(CDimensionLinear dim, bool bDrawExtensionLines, Canvas imageCanvas, SolidColorBrush linesColor, SolidColorBrush textColor, double thickness, string name = "")
        {
            double dRotation_rad = Math.Atan((dim.ControlPointEnd.Y - dim.ControlPointStart.Y) / (dim.ControlPointEnd.X - dim.ControlPointStart.X));
            double dRotation_deg = Geom2D.RadiansToDegrees(dRotation_rad);

            float fUnitFactor_mTomm = 1000;
            int iNumberOfDecimalPlaces = 0;
            string sText = (Math.Round(dim.BasicLength_m * fUnitFactor_mTomm, iNumberOfDecimalPlaces)).ToString();

            double dLengtOfExtensionLineStartToPrimary = 0.8 * dim.OffsetFromOrigin_pxs;
            double dLengtOfExtensionLinePrimaryToEnd = 5; // Points
            double dOffsetOfExtensionLineFromPoint = 0.2 * dim.OffsetFromOrigin_pxs;

            double dLengtOfExtensionLineTotal = dLengtOfExtensionLineStartToPrimary + dLengtOfExtensionLinePrimaryToEnd;

            double dPrimaryLineThickness = thickness;
            double lPrimaryLinelength = Math.Sqrt(Math.Pow(dim.ControlPointEnd.X - dim.ControlPointStart.X, 2) + Math.Pow(dim.ControlPointEnd.Y - dim.ControlPointStart.Y, 2));

            if (dim.ControlPointStart.X > dim.ControlPointEnd.X) lPrimaryLinelength *= -1; //opposite direction

            if (dim.IsDimensionOutSide)
            {
                if (dim.ControlPointRef.Y > dim.ControlPointStart.Y) //over center point
                {
                    if (dim.ControlPointRef.X > dim.ControlPointStart.X) //on the left
                    {
                        if (dRotation_deg < 45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                    else //on the right
                    {
                        if (dRotation_deg > -45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                }
                else //under center point
                {
                    if (dim.ControlPointRef.X > dim.ControlPointStart.X) //on the left
                    {
                        if (dRotation_deg < -45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                    else //on the right
                    {
                        if (dRotation_deg > 45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                }
            }
            else
            {
                if (dim.ControlPointRef.Y > dim.ControlPointStart.Y) //over center point
                {
                    if (dim.ControlPointRef.X > dim.ControlPointStart.X) //on the left
                    {
                        if (dRotation_deg > 45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                    else //on the right
                    {
                        if (dRotation_deg < -45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                }
                else //under center point
                {
                    if (dim.ControlPointRef.X > dim.ControlPointStart.X) //on the left
                    {
                        if (dRotation_deg > -45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                    else //on the right
                    {
                        if (dRotation_deg < 45) dim.OffsetFromOrigin_pxs *= -1;
                    }
                }
            }

            Line lPrimaryLine = new Line();
            lPrimaryLine.X1 = dim.ControlPointStart.X;
            lPrimaryLine.Y1 = dim.ControlPointStart.Y + dim.OffsetFromOrigin_pxs;
            lPrimaryLine.X2 = dim.ControlPointStart.X + lPrimaryLinelength;
            lPrimaryLine.Y2 = lPrimaryLine.Y1;

            // Extension lines
            double dExtensionLineThickness = thickness;

            Line lExtensionLine1 = new Line();
            lExtensionLine1.X1 = lPrimaryLine.X1;
            lExtensionLine1.Y1 = lPrimaryLine.Y1 - dim.OffsetFromOrigin_pxs + dOffsetOfExtensionLineFromPoint;
            lExtensionLine1.X2 = lPrimaryLine.X1;
            lExtensionLine1.Y2 = lPrimaryLine.Y1;

            Line lExtensionLine2 = new Line();
            lExtensionLine2.X1 = lPrimaryLine.X2;
            lExtensionLine2.Y1 = lPrimaryLine.Y2 - dim.OffsetFromOrigin_pxs + dOffsetOfExtensionLineFromPoint;
            lExtensionLine2.X2 = lPrimaryLine.X2;
            lExtensionLine2.Y2 = lPrimaryLine.Y2;

            // Slope Symbol Lines
            double dSlopeLineLength = 10;
            double dSlopeLineThickness = thickness;

            double coord = 0.5 * dSlopeLineLength / Math.Sqrt(2);

            Line lSlopeLine1 = new Line();
            lSlopeLine1.X1 = lPrimaryLine.X1 - coord;
            lSlopeLine1.Y1 = lPrimaryLine.Y1 + coord;
            lSlopeLine1.X2 = lPrimaryLine.X1 + coord;
            lSlopeLine1.Y2 = lPrimaryLine.Y1 - coord;

            Line lSlopeLine2 = new Line();
            lSlopeLine2.X1 = lPrimaryLine.X2 - coord;
            lSlopeLine2.Y1 = lPrimaryLine.Y2 + coord;
            lSlopeLine2.X2 = lPrimaryLine.X2 + coord;
            lSlopeLine2.Y2 = lPrimaryLine.Y2 - coord;

            RotateDimension(dim.ControlPointStart, dRotation_deg, ref lPrimaryLine, ref lExtensionLine1, ref lExtensionLine2, ref lSlopeLine1, ref lSlopeLine2);

            // Urcuje sa z uz transformovanych suradnice lPrimaryLine
            double textPositionx = lPrimaryLine.X1 + 0.5 * (lPrimaryLine.X2 - lPrimaryLine.X1);
            double textPositiony = lPrimaryLine.Y1 + 0.5 * (lPrimaryLine.Y2 - lPrimaryLine.Y1);

            // Draw dimension line
            DrawLine(lPrimaryLine, linesColor, PenLineCap.Flat, PenLineCap.Flat, dPrimaryLineThickness, imageCanvas, DashStyles.Solid, null, name);
            // Draw extension line - start
            DrawLine(lExtensionLine1, linesColor, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas, DashStyles.Solid, null, name);
            // Draw extension line - end
            DrawLine(lExtensionLine2, linesColor, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas, DashStyles.Solid, null, name);
            // Draw slope line - start
            DrawLine(lSlopeLine1, linesColor, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas, DashStyles.Solid, null, name);
            // Draw slope line - end
            DrawLine(lSlopeLine2, linesColor, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas, DashStyles.Solid, null, name);
            // Draw text            
            DrawText(sText, textPositionx, textPositiony, dRotation_deg, 12, dim.ControlPointRef, dim.IsTextOutSide, textColor, imageCanvas, name);
        }

        public static void RotateDimension(Point centerRotation, double dRotationDegrees, ref Line lPrimaryLine, ref Line lExtensionLine1, ref Line lExtensionLine2, ref Line lSlopeLine1, ref Line lSlopeLine2)
        {
            RotateLine_CW(centerRotation, dRotationDegrees, ref lPrimaryLine);
            RotateLine_CW(centerRotation, dRotationDegrees, ref lExtensionLine1);
            RotateLine_CW(centerRotation, dRotationDegrees, ref lExtensionLine2);
            RotateLine_CW(centerRotation, dRotationDegrees, ref lSlopeLine1);
            RotateLine_CW(centerRotation, dRotationDegrees, ref lSlopeLine2);
        }
        public static void RotateLine_CW(Point rotationCenter, double dRotationDegrees, ref Line l)
        {
            Point pLineStart = new Point(l.X1, l.Y1);
            Point pLineEnd = new Point(l.X2, l.Y2);

            pLineStart = Geom2D.RotatePoint(pLineStart, rotationCenter, dRotationDegrees);
            pLineEnd = Geom2D.RotatePoint(pLineEnd, rotationCenter, dRotationDegrees);
            //    TransformPositions_CW_rad(l.X1, l.Y1, dRotation_rad, ref pLineStart);
            //Geom2D.TransformPositions_CW_rad(l.X1, l.Y1, dRotation_rad, ref pLineEnd);

            l.X1 = pLineStart.X;
            l.Y1 = pLineStart.Y;

            l.X2 = pLineEnd.X;
            l.Y2 = pLineEnd.Y;
        }

        //public static void RotateAndTranslateLine_CW(float fOffset_x, float fOffset_y, double dRotation_rad, ref Line l)
        //{
        //    Point pLineStart = new Point(l.X1, l.Y1);
        //    Point pLineEnd = new Point(l.X2, l.Y2);

        //    Geom2D.TransformPositions_CW_rad(0, 0, dRotation_rad, ref pLineStart);
        //    Geom2D.TransformPositions_CW_rad(0, 0, dRotation_rad, ref pLineEnd);

        //    Geom2D.TransformPositions_CW_rad(fOffset_x, fOffset_y, 0, ref pLineStart);
        //    Geom2D.TransformPositions_CW_rad(fOffset_x, fOffset_y, 0, ref pLineEnd);

        //    l.X1 = pLineStart.X;
        //    l.Y1 = pLineStart.Y;

        //    l.X2 = pLineEnd.X;
        //    l.Y2 = pLineEnd.Y;
        //}

        //public static void DrawArcDimension(Point pStart, Point pEnd, Point pCenter, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, Canvas imageCanvas)
        //{
        //    float fPositionOfArcFactor = 0.45f;

        //    double slope = Geom2D.GetAngle_rad(pStart, pEnd, pCenter);
        //    double radius = fPositionOfArcFactor * (pStart.X * dReal_Model_Zoom_Factor);

        //    Point p2 = new Point(); // 2nd point of arc
        //    p2.X = Geom2D.GetPositionX_deg((float)radius, (float)slope / MathF.fPI * 180f);  // y
        //    p2.Y = Geom2D.GetPositionY_CCW_deg((float)radius, (float)slope / MathF.fPI * 180f);  // z

        //    Size size = new Size(radius, radius);

        //    ArcSegment arc = new ArcSegment(
        //    new Point(modelMarginLeft_x + pCenter.X * dReal_Model_Zoom_Factor + p2.X, modelMarginBottom_y - (pCenter.Y * dReal_Model_Zoom_Factor + p2.Y)),
        //    size,
        //    slope / MathF.fPI * 180,
        //    false,
        //    SweepDirection.Counterclockwise,
        //    true
        //    );

        //    PathGeometry pathGeometry = new PathGeometry();
        //    PathFigure figure = new PathFigure();
        //    figure.StartPoint = new Point(modelMarginLeft_x + fPositionOfArcFactor * (pStart.X * dReal_Model_Zoom_Factor), modelMarginBottom_y - pStart.Y * dReal_Model_Zoom_Factor);
        //    figure.Segments.Add(arc);

        //    pathGeometry.Figures.Add(figure);
        //    Path path = new Path();
        //    path.Data = pathGeometry;
        //    //path.Fill = Brushes.Gray;
        //    path.Stroke = Brushes.Black;
        //    imageCanvas.Children.Add(path);

        //    // Lines
        //    Line l1 = new Line();
        //    l1.X1 = modelMarginLeft_x + pCenter.X * dReal_Model_Zoom_Factor;
        //    l1.Y1 = modelMarginBottom_y - pCenter.Y * dReal_Model_Zoom_Factor;

        //    l1.X2 = modelMarginLeft_x + pStart.X * dReal_Model_Zoom_Factor;
        //    l1.Y2 = modelMarginBottom_y - pStart.Y * dReal_Model_Zoom_Factor;

        //    DrawLine(l1, Brushes.Black, DashStyles.Dash, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas);

        //    Line l2 = new Line();
        //    l2.X1 = modelMarginLeft_x + pCenter.X * dReal_Model_Zoom_Factor;
        //    l2.Y1 = modelMarginBottom_y - pCenter.Y * dReal_Model_Zoom_Factor;

        //    l2.X2 = modelMarginLeft_x + pEnd.X * dReal_Model_Zoom_Factor;
        //    l2.Y2 = modelMarginBottom_y - pEnd.Y * dReal_Model_Zoom_Factor;

        //    DrawLine(l2, Brushes.Black, DashStyles.Dash, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas);

        //    // Draw text
        //    // Draw text in the middle of the arc
        //    float fFactorTextPosition = 0.5f;
        //    float fTextPositionx = Geom2D.GetPositionX_deg((float)radius, fFactorTextPosition * (float)slope / MathF.fPI * 180f);  // y
        //    float fTextPositiony = Geom2D.GetPositionY_CCW_deg((float)radius, fFactorTextPosition * (float)slope / MathF.fPI * 180f);  // z
        //    string sText = Math.Round(slope / MathF.fPI * 180, 1).ToString() + " °";

        //    DrawText(sText, modelMarginLeft_x + pCenter.X * dReal_Model_Zoom_Factor +  fTextPositionx, modelMarginBottom_y - (pCenter.Y * dReal_Model_Zoom_Factor + fTextPositiony), 0, 12, false, Brushes.Black, imageCanvas);
        //}

        public static void DrawArcDimension(Point pStart, Point pEnd, Point pCenter, Canvas imageCanvas, string name = "")
        {
            float fPositionOfArcFactor = 0.3f;

            double slope = Geom2D.GetAngle_rad(pStart, pEnd, pCenter);
            double slopeDeg = Geom2D.RadiansToDegrees(slope);
            double radius = Math.Abs(pCenter.X - pStart.X) * fPositionOfArcFactor;
            Size size = new Size(radius, radius);


            SweepDirection direction = SweepDirection.Clockwise;

            if (pCenter.X < pStart.X && pCenter.X < pEnd.X) //center on left
            {
                if (pStart.Y < pEnd.Y) direction = SweepDirection.Clockwise;
                else direction = SweepDirection.Counterclockwise;
            }
            else if (pCenter.X > pStart.X && pCenter.X > pEnd.X) //center on right
            {
                if (pStart.Y < pEnd.Y) direction = SweepDirection.Counterclockwise;
                else direction = SweepDirection.Clockwise;
            }
            else //start and center equal
            {
                if (pCenter.Y > pStart.Y && pCenter.Y > pEnd.Y) //center on the bottom
                {
                    if (pStart.X > pEnd.X) direction = SweepDirection.Counterclockwise;
                    else direction = SweepDirection.Clockwise;
                }
                else
                {
                    if (pStart.X > pEnd.X) direction = SweepDirection.Clockwise;
                    else direction = SweepDirection.Counterclockwise;
                }
            }


            Point pathStartPoint = new Point(pCenter.X + (pStart.X - pCenter.X) * fPositionOfArcFactor, pCenter.Y + (pStart.Y - pCenter.Y) * fPositionOfArcFactor);
            Point pathTowardsPoint = Geom2D.RotatePoint(pathStartPoint, pCenter, direction == SweepDirection.Counterclockwise ? -slopeDeg : slopeDeg);

            ArcSegment arc = new ArcSegment(pathTowardsPoint, size, slopeDeg, false, direction, true);

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = pathStartPoint;
            figure.Segments.Add(arc);

            pathGeometry.Figures.Add(figure);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = pathGeometry;
            //path.Fill = Brushes.Gray;
            path.Stroke = Brushes.Black;
            imageCanvas.Children.Add(path);

            DoubleCollection dashArray = new DoubleCollection();
            dashArray.Add(10); dashArray.Add(10);
            // Lines
            Line l1 = new Line();
            l1.X1 = pCenter.X;
            l1.Y1 = pCenter.Y;
            l1.X2 = pStart.X;
            l1.Y2 = pStart.Y;
            DrawLine(l1, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas, DashStyles.Dash, dashArray, name);

            Line l2 = new Line();
            l2.X1 = pCenter.X;
            l2.Y1 = pCenter.Y;
            l2.X2 = pEnd.X;
            l2.Y2 = pEnd.Y;
            DrawLine(l2, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas, DashStyles.Dash, dashArray, name);

            // Draw text
            // Draw text in the middle of the arc
            //double fTextPositionx = pathStartPoint.X + (pathTowardsPoint.X - pathStartPoint.X) / 2;
            //double fTextPositiony = pathStartPoint.Y + (pathTowardsPoint.Y - pathStartPoint.Y) / 2;
            //double fTextPositionx = (pathStartPoint.X + pathTowardsPoint.X + pCenter.X) / 3;
            //double fTextPositiony = (pathStartPoint.Y + pathTowardsPoint.Y + pCenter.Y) / 3;
            double fTextPositionx = (pathStartPoint.X * 3 + pathTowardsPoint.X * 3 + pCenter.X) / 7;
            double fTextPositiony = (pathStartPoint.Y * 3 + pathTowardsPoint.Y * 3 + pCenter.Y) / 7;
            string sText = Math.Round(slopeDeg, 1).ToString() + " °";

            double dTextWidth;
            DrawText(sText, fTextPositionx, fTextPositiony, 0, 12, Brushes.Black, imageCanvas, out dTextWidth, name);
        }

        public static void CalculateModelLimits(List<Point> Points_temp, out double fTempMax_X, out double fTempMin_X, out double fTempMax_Y, out double fTempMin_Y)
        {
            fTempMax_X = double.MinValue;
            fTempMin_X = double.MaxValue;
            fTempMax_Y = double.MinValue;
            fTempMin_Y = double.MaxValue;

            if (Points_temp != null) // Some points exist
            {
                for (int i = 0; i < Points_temp.Count; i++)
                {
                    // Maximum X - coordinate
                    if (Points_temp[i].X > fTempMax_X)
                        fTempMax_X = Points_temp[i].X;

                    // Minimum X - coordinate
                    if (Points_temp[i].X < fTempMin_X)
                        fTempMin_X = Points_temp[i].X;

                    // Maximum Y - coordinate
                    if (Points_temp[i].Y > fTempMax_Y)
                        fTempMax_Y = Points_temp[i].Y;

                    // Minimum Y - coordinate
                    if (Points_temp[i].Y < fTempMin_Y)
                        fTempMin_Y = Points_temp[i].Y;
                }
            }
        }
        public static void CalculateModelLimits(Point[] Points_temp, out double fTempMax_X, out double fTempMin_X, out double fTempMax_Y, out double fTempMin_Y)
        {
            fTempMax_X = double.MinValue;
            fTempMin_X = double.MaxValue;
            fTempMax_Y = double.MinValue;
            fTempMin_Y = double.MaxValue;

            if (Points_temp != null) // Some points exist
            {
                for (int i = 0; i < Points_temp.Length; i++)
                {
                    // Maximum X - coordinate
                    if (Points_temp[i].X > fTempMax_X)
                        fTempMax_X = Points_temp[i].X;

                    // Minimum X - coordinate
                    if (Points_temp[i].X < fTempMin_X)
                        fTempMin_X = Points_temp[i].X;

                    // Maximum Y - coordinate
                    if (Points_temp[i].Y > fTempMax_Y)
                        fTempMax_Y = Points_temp[i].Y;

                    // Minimum Y - coordinate
                    if (Points_temp[i].Y < fTempMin_Y)
                        fTempMin_Y = Points_temp[i].Y;
                }
            }
        }

        public static List<Point> MovePointsToCenterCSAndCalculateModelLimits(List<Point> points, out double max_X, out double min_X, out double max_Y, out double min_Y)
        {
            CalculateModelLimits(points, out max_X, out min_X, out max_Y, out min_Y);

            List<Point> updatedPoints = MovePointsToCenterOfCoordinateSystem(points, min_X, min_Y);
            CalculateModelLimits(updatedPoints, out max_X, out min_X, out max_Y, out min_Y);
            return updatedPoints;
        }
        //Moves model points left bottom to[0,0]
        public static List<Point> MovePointsToCenterOfCoordinateSystem(List<Point> points, double min_X, double min_Y)
        {
            List<Point> updatedPoints = new List<Point>();
            foreach (Point p in points)
            {
                updatedPoints.Add(new Point(p.X - min_X, p.Y - min_Y));
            }
            return updatedPoints;
        }

        public static Point CalculateModelCenter(Point[] Points)
        {
            double fTempMax_X = double.MinValue;
            double fTempMin_X = double.MaxValue;
            double fTempMax_Y = double.MinValue;
            double fTempMin_Y = double.MaxValue;
            CalculateModelLimits(Points, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);

            return new Point((fTempMax_X + fTempMin_X) / 2, (fTempMax_Y + fTempMin_Y) / 2);
        }

        // POVODNE FUNKCIE

        //LINES
        public static void DrawLine(CMember member, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas canvas,
            HorizontalAlignment alignHorizontal = HorizontalAlignment.Left, VerticalAlignment alignVertical = VerticalAlignment.Top)
        {
            Line myLine = new Line();
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.X1 = member.NodeStart.X;
            myLine.X2 = member.NodeEnd.X;
            myLine.Y1 = member.NodeStart.Z;
            myLine.Y2 = member.NodeEnd.Z;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            myLine.HorizontalAlignment = alignHorizontal;
            myLine.VerticalAlignment = alignVertical;
            Canvas.SetTop(myLine, member.NodeStart.Z);
            Canvas.SetLeft(myLine, member.NodeStart.X);
            canvas.Children.Add(myLine);
        }

        /// <summary>
        /// Draw Poliline to Canvas
        /// </summary>
        /// <param name="arrPointsCoordX"></param>
        /// <param name="arrPointsCoordY"></param>
        /// <param name="dCanvasTop"></param>
        /// <param name="dCanvasLeft"></param>
        /// <param name="fFactorX"></param>
        /// <param name="fFactorY"></param>
        /// <param name="marginLeft_x"></param>
        /// <param name="bottomPosition_y">Tento parameter by mal byt marginTop_y </param> 
        /// <param name="color"></param>
        /// <param name="startCap"></param>
        /// <param name="endCap"></param>
        /// <param name="thickness"></param>
        /// <param name="canvas"></param>
        public static PointCollection DrawPolyLine(float[] arrPointsCoordX, float[] arrPointsCoordY, double dCanvasTop, double dCanvasLeft, float fFactorX, float fFactorY,
            float marginLeft_x, float bottomPosition_y, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas canvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < arrPointsCoordX.Length; i++)
            {
                //bottomPosition_y tu by som nechcel mat poziciu bottomPosition ale MarginTop_y
                points.Add(new Point(marginLeft_x + fFactorX * arrPointsCoordX[i], bottomPosition_y - fFactorY * arrPointsCoordY[i]));
            }

            Polyline myLine = new Polyline();
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            Canvas.SetTop(myLine, dCanvasTop);
            Canvas.SetLeft(myLine, dCanvasLeft);
            canvas.Children.Add(myLine);

            return points;
        }

        public static void DrawRectangle(SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas canvas, Point lt, Point br, string name = "")
        {
            Rectangle rect = new Rectangle();
            rect.Tag = name;
            rect.Stretch = Stretch.Fill;
            rect.Fill = fillColor;
            rect.Stroke = strokeColor;
            rect.Width = br.X - lt.X;
            rect.Height = br.Y - lt.Y;
            Canvas.SetTop(rect, lt.Y);
            Canvas.SetLeft(rect, lt.X);
            canvas.Children.Add(rect);
        }

        //TEXT
        public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, bool bIsTextAboveControlPoint, SolidColorBrush color, Canvas canvas, string name = "")
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Tag = name;
            textBlock.Text = text;
            textBlock.Foreground = color;
            //textBlock.Background = new SolidColorBrush(Colors.Red);
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);

            // TODO Ondrej - upravit - Umiestnenie textu uprostred hlavnej kotovacej ciary (ak je kota velmi kratka,
            // tak by to malo byt presne v strede a ak to nevojde medzi extension lines,
            // tak sa to robi tak ze sa text posunie uplne vedla koty nalavo / vpravo / alebo pod kotu (na opacnu stranu hlavnej kotovacej ciary)
            // My by sme si mali vystacit s tym, ze to bude presne v strede, tie "uzke pasiky" nebyvaju sirsie ako 100 mm, takze tam budu len 2 cislice
            // upravene v metode dole - tuto nechavam pre pouzitie pre textAboveControlPoint

            Canvas.SetLeft(textBlock, posx);

            if (bIsTextAboveControlPoint) // Text nad liniou koty
            {
                Canvas.SetTop(textBlock, posy - txtSize.Height);
                if (Math.Abs(rotationAngle_CW_deg) / 90 < 0.2)
                {
                    Canvas.SetTop(textBlock, posy - txtSize.Height);
                }
                else if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.80)
                {
                    if (rotationAngle_CW_deg < 0) Canvas.SetTop(textBlock, posy + txtSize.Width / 2);
                    else Canvas.SetTop(textBlock, posy - txtSize.Width / 2);

                    Canvas.SetLeft(textBlock, posx - txtSize.Height);
                }
                else
                {
                    if (rotationAngle_CW_deg < 0) Canvas.SetTop(textBlock, posy + txtSize.Width);
                    else Canvas.SetTop(textBlock, posy - txtSize.Width);

                    Canvas.SetLeft(textBlock, posx - txtSize.Width);
                }

                //textBlock.Margin = new Thickness(2, 0, 0, 0);
            }
            else // text pod liniou koty
            {
                if (Math.Abs(rotationAngle_CW_deg) / 90 < 0.2)
                {
                    Canvas.SetTop(textBlock, posy);
                }
                else if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.80)
                {
                    if (rotationAngle_CW_deg < 0) Canvas.SetTop(textBlock, posy + txtSize.Width / 2);
                    else Canvas.SetTop(textBlock, posy - txtSize.Width / 2);
                }
                else
                {
                    if (rotationAngle_CW_deg < 0) Canvas.SetTop(textBlock, posy + txtSize.Width);
                    else Canvas.SetTop(textBlock, posy - txtSize.Width);
                }

                //textBlock.Margin = new Thickness(2, 2, 0, 0);
            }

            textBlock.RenderTransform = new RotateTransform(rotationAngle_CW_deg);
            canvas.Children.Add(textBlock);
        }

        public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, SolidColorBrush color, Canvas canvas, out double txtWidth, string name = "")
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Tag = name;
            textBlock.Text = text;
            textBlock.Foreground = color;
            //textBlock.Background = new SolidColorBrush(Colors.Red);
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);

            txtWidth = txtSize.Width; // Dlzka texboxu - pre potreby vykreslenia ciary pod textom

            // NOTE - docasne vykreslujeme body na ktore sa viaze text
            DrawPoint(new Point(posx, posy), Brushes.DarkCyan, Brushes.DarkCyan, 2, canvas);
            //DrawPoint(new Point(posx, posy), Brushes.Red, Brushes.Red, 2, canvas);

            Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
            Canvas.SetTop(textBlock, posy - txtSize.Height / 2);

            textBlock.RenderTransform = new RotateTransform(rotationAngle_CW_deg);
            canvas.Children.Add(textBlock);
        }
        public static void DrawText(string text, double posx, double posy, double fontSize, bool bYOrientationIsUp, VerticalAlignment valign, SolidColorBrush color, Canvas canvas)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            //textBlock.Background = new SolidColorBrush(Colors.Red);
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);

            // NOTE - docasne vykreslujeme body na ktore sa viaze text
            //DrawPoint(new Point(posx, posy), Brushes.DarkCyan, Brushes.DarkCyan, 2, canvas);

            Canvas.SetLeft(textBlock, posx);
            if (valign == VerticalAlignment.Center) Canvas.SetTop(textBlock, posy - txtSize.Height / 2);
            else if (valign == VerticalAlignment.Top) Canvas.SetTop(textBlock, posy - txtSize.Height);
            else Canvas.SetTop(textBlock, posy);

            canvas.Children.Add(textBlock);
        }
        //public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, SolidColorBrush color, Canvas canvas, out double txtWidth)
        public static void DrawText(string text, double posx, double posy, double fontSize, VerticalAlignment valign, HorizontalAlignment halign, SolidColorBrush color, Canvas canvas, out double txtWidth)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            //textBlock.Background = new SolidColorBrush(Colors.Red);
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);

            txtWidth = txtSize.Width; // Dlzka texboxu - pre potreby vykreslenia ciary pod textom

            // NOTE - docasne vykreslujeme body na ktore sa viaze text
            bool drawTextPoint = false;
            if (drawTextPoint) DrawPoint(new Point(posx, posy), Brushes.DarkCyan, Brushes.DarkCyan, 2, canvas);

            //Canvas.SetLeft(textBlock, posx);
            if (halign == HorizontalAlignment.Center) Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
            else if (halign == HorizontalAlignment.Left) Canvas.SetLeft(textBlock, posx - txtSize.Width);
            else Canvas.SetLeft(textBlock, posx);

            if (valign == VerticalAlignment.Center) Canvas.SetTop(textBlock, posy - txtSize.Height / 2);
            else if (valign == VerticalAlignment.Top) Canvas.SetTop(textBlock, posy - txtSize.Height);
            else Canvas.SetTop(textBlock, posy);

            canvas.Children.Add(textBlock);
        }

        public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, Point refPoint, bool bIsTextOutSide, SolidColorBrush color, Canvas canvas, string name = "")
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Tag = name;
            textBlock.Text = text;
            textBlock.Foreground = color;
            //textBlock.Background = new SolidColorBrush(Colors.Red);
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);

            if (bIsTextOutSide)
            {
                if (refPoint.Y > posy) //top
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetTop(textBlock, posy - txtSize.Height / 2);
                    }
                    else Canvas.SetTop(textBlock, posy - txtSize.Height);
                }
                else //bottom
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetTop(textBlock, posy - txtSize.Height / 2);
                    }
                    else Canvas.SetTop(textBlock, posy);
                }

                if (refPoint.X > posx) //left
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetLeft(textBlock, posx - txtSize.Width);
                    }
                    else Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
                }
                else //right
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetLeft(textBlock, posx);
                    }
                    else Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
                }
            }
            else //INSIDE
            {
                if (refPoint.Y > posy) //top
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetTop(textBlock, posy - txtSize.Height / 2);
                    }
                    else Canvas.SetTop(textBlock, posy);
                }
                else //bottom
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetTop(textBlock, posy - txtSize.Height / 2);
                    }
                    else Canvas.SetTop(textBlock, posy - txtSize.Height);
                }

                if (refPoint.X > posx) //left
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetLeft(textBlock, posx);
                    }
                    else Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
                }
                else //right
                {
                    if (Math.Abs(rotationAngle_CW_deg) / 90 > 0.5)
                    {
                        Canvas.SetLeft(textBlock, posx - txtSize.Width);
                    }
                    else Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
                }
            }

            //center of the dimension should be
            //Canvas.SetLeft(textBlock, posx - txtSize.Width / 2);
            //Canvas.SetTop(textBlock, posy - txtSize.Height / 2);

            textBlock.RenderTransform = new RotateTransform(rotationAngle_CW_deg, txtSize.Width / 2, txtSize.Height / 2);
            canvas.Children.Add(textBlock);
        }

        private static Size MeasureString(TextBlock textBlock, string text)
        {
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }
        public static double GetTextWidth(string text, double fontSize)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);
            return txtSize.Width;
        }
        public static double GetTextHeight(string text, double fontSize)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);
            return txtSize.Height;
        }

        public static void DrawTexts(bool bUseZoomFactor, string[] array_text, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight,
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, bool isTextAboveCtrlPoint, SolidColorBrush color, Canvas canvas)
        {
            float fFactorX = 1.0f;
            float fFactorY = 1.0f;

            if (bUseZoomFactor)
            {
                float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
                fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

                float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
                fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);
            }

            for (int i = 0; i < array_text.Length; i++)
            {
                DrawText(array_text[i], modelMarginLeft_x + fFactorX * arrPointsCoordX[i], modelBottomPosition_y - fFactorY * arrPointsCoordY[i], 0, 12, isTextAboveCtrlPoint, color, canvas);
            }
        }

        public static void DrawTexts(Point p, bool bYOrientationIsUp, bool bUseZoomFactor, string[] array_text, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight,
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, SolidColorBrush color, Canvas canvas)
        {
            if (!bYOrientationIsUp) // Draw positive values below x-axis
            {
                for (int i = 0; i < arrPointsCoordY.Length; i++)
                    arrPointsCoordY[i] *= -1f;
            }

            float fFactorX = 1.0f;
            float fFactorY = 1.0f;

            if (bUseZoomFactor)
            {
                float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
                fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

                float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
                fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);
                if (float.IsInfinity(fFactorY))
                {
                    fFactorY = 1.0f;
                }
            }

            for (int i = 0; i < array_text.Length; i++)
            {
                VerticalAlignment va = VerticalAlignment.Bottom;
                if (bYOrientationIsUp)
                {
                    if (arrPointsCoordY[i] > 0) va = VerticalAlignment.Top;
                    else va = VerticalAlignment.Bottom;

                    DrawPoint(new Point(p.X + fFactorX * arrPointsCoordX[i], p.Y - fFactorY * arrPointsCoordY[i]), Brushes.Red, Brushes.Red, 2, canvas);
                    DrawText(array_text[i], (p.X + fFactorX * arrPointsCoordX[i]), (p.Y - fFactorY * arrPointsCoordY[i]), 12, bYOrientationIsUp, va, color, canvas);
                }
                else
                {
                    if (arrPointsCoordY[i] < 0) va = VerticalAlignment.Top;
                    else va = VerticalAlignment.Bottom;

                    DrawPoint(new Point(p.X + fFactorX * arrPointsCoordX[i], p.Y + fFactorY * arrPointsCoordY[i]), Brushes.Red, Brushes.Red, 2, canvas);
                    DrawText(array_text[i], (p.X + fFactorX * arrPointsCoordX[i]), (p.Y + fFactorY * arrPointsCoordY[i]), 12, bYOrientationIsUp, va, color, canvas);
                }
            }
        }

        public static float CalculateZoomFactor(float[] arrPointsCoord, float fCanvasDimension, float fMarginValue1, float fMarginValue2, out float fValueMin, out float fValueMax, out float fRangeOfValues, out float fAxisLength)
        {
            fValueMin = arrPointsCoord.Min();
            fValueMax = arrPointsCoord.Max();

            if (fValueMin * fValueMax < 0)
                fAxisLength = fRangeOfValues = Math.Abs(fValueMin) + Math.Abs(fValueMax);
            else if (fValueMax > 0)
            {
                fAxisLength = fValueMax;
                fRangeOfValues = fValueMax - fValueMin;
            }
            else
            {
                fAxisLength = Math.Abs(fValueMin);
                fRangeOfValues = fValueMax - fValueMin;
            }

            return (float)((fCanvasDimension - fMarginValue1 - fMarginValue2) / fAxisLength);
        }


        public static void DrawXYDiagramToCanvas(bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight,
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, Canvas canvas)
        {
            Point p = Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, canvas);

            // TODO
            // Vysledky by mali byt v N a Nm (pocitame v zakladnych jednotkach SI), pre zobrazenie prekonvertovat na kN a kNm, pripadne pridat nastavenie jednotiek do GUI
            bool drawPolygons = true;
            if (!drawPolygons)
            {
                Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, canvas);
            }
            else
            {
                Drawing2D.DrawYValuesPolygonInCanvas(p, true, arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, canvas);

            }
            // Draw values description
            int iNumberOfDecimalPlaces = 2;
            Drawing2D.DrawTexts(p, true, true, ConversionsHelper.ConvertArrayFloatToString(arrPointsCoordY, iNumberOfDecimalPlaces), arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, canvas);
        }

        // TODO No 44 Ondrej
        // Temporary - TODO Ondrej zjednotit metody pre vykreslovanie v 2D do nejakej zakladnej triedy (mozno uz nejaka aj existuje v inom projekte "SW_EN\GRAPHICS\PAINT" alebo swen_GUI\WindowPaint)
        public static Point DrawAxisInCanvas(bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight,
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, Canvas canvas)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            if (float.IsInfinity(fFactorY))
            {
                fFactorY = fCanvasHeight - modelMarginTop_y - modelMarginBottom_y;
                yValueMin = -0.5f;
                yValueMax = 0.5f;
                yRangeOfValues = yAxisLength = 1;
            }

            float fPositionOfXaxisToTheEndOfYAxis = 0;

            if (bYOrientationIsUp) // Up (Forces N, Vx, Vy) // Tu sa urcuje ci sa kladne hodnoty veliciny vykresluju smerom hore alebo dole, tj kam smeruje osa y
            {
                //To Mato: to je co za cislo 1.02f ???
                // To Ondrej: To je moja "konstanta" - os x je o 2% dlhsia nez maximalna x suradnica diagramu, blbe je ze pre y som to neurobil :)
                // Asi by bolo lepsie ak by boli osy dhsie o nejaku fixnu hodnotu

                // x-axis (middle)
                fPositionOfXaxisToTheEndOfYAxis = yValueMax < 0 ? 0 : yValueMax;

                Drawing2D.DrawPolyLine(new float[2] { 0, 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis, modelMarginLeft_x,
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);

                // y-axis (oriented upwards)
                Drawing2D.DrawPolyLine(new float[2] { 0, 0 }, new float[2] { yValueMin < 0 ? yValueMin : 0, yValueMax < 0 ? 0 : yValueMin + yRangeOfValues }, modelMarginTop_y, modelMarginLeft_x,
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);
            }
            else // Down (Torsion and bending moments T, Mx, My and deflections delta)
            {
                fPositionOfXaxisToTheEndOfYAxis = yValueMin < 0 ? Math.Abs(yValueMin) : 0;

                // x-axis (middle)
                Drawing2D.DrawPolyLine(new float[2] { 0, 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis, modelMarginLeft_x,
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);

                // y-axis (oriented downwards)
                Drawing2D.DrawPolyLine(new float[2] { 0, 0 }, new float[2] { yValueMin < 0 ? yValueMin : 0, yValueMax < 0 ? 0 : yValueMin + yRangeOfValues }, modelMarginTop_y, modelMarginLeft_x,
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);
            }
            double AxisIntX = modelMarginLeft_x;
            double AxisIntY = modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis;
            Point AxisIntersection = new Point(AxisIntX, AxisIntY);
            return AxisIntersection;
        }

        public static void DrawYValuesCurveInCanvas(bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight,
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, Canvas canvas)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            if (arrPointsCoordY != null)
            {
                if (!bYOrientationIsUp) // Draw positive values below x-axis
                {
                    for (int i = 0; i < arrPointsCoordY.Length; i++)
                        arrPointsCoordY[i] *= -1f;
                }

                Drawing2D.DrawPolyLine(arrPointsCoordX, arrPointsCoordY, modelMarginTop_y, modelMarginLeft_x, fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.SlateGray, new PenLineCap(), PenLineCap.Triangle, 1, canvas);
            }
        }

        //p is Axis intersection
        public static void DrawYValuesPolygonInCanvas(Point p, bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight,
        float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, Canvas canvas)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            if (arrPointsCoordY == null || arrPointsCoordX == null) return;

            if (!bYOrientationIsUp) // Draw positive values below x-axis
            {
                for (int i = 0; i < arrPointsCoordY.Length; i++)
                    arrPointsCoordY[i] *= -1f;
            }

            List<Point> listPoints = new List<Point>();
            listPoints.Add(new Point(p.X, p.Y));
            for (int i = 0; i < arrPointsCoordY.Length; i++)
            {
                listPoints.Add(new Point(modelMarginLeft_x + arrPointsCoordX[i] * fFactorX, p.Y - arrPointsCoordY[i] * fFactorY));
            }
            listPoints.Add(new Point(modelMarginLeft_x + xValueMax * fFactorX, p.Y));

            Drawing2D.DrawPolygon(listPoints, Brushes.LightSlateGray, Brushes.SlateGray, PenLineCap.Flat, PenLineCap.Flat, 1, 0.3f, canvas);
        }


        public static void DetectAndResolveTextColisions(Canvas canvas)
        {
            canvas.UpdateLayout();

            foreach (UIElement elem in canvas.Children)
            {
                if (elem.Visibility == Visibility.Hidden) continue;
                //if (!elem.IsVisible) continue;
                if (!(elem is TextBlock)) continue;

                TextBlock tb = elem as TextBlock;
                Size s1 = MeasureString(tb, tb.Text);
                double top1 = Canvas.GetTop(elem);
                double left1 = Canvas.GetLeft(elem);
                //Rect r = new Rect(new Point(left1, top1), tb.RenderSize);  
                Rect r = new Rect(new Point(left1, top1), s1);

                foreach (UIElement elem2 in canvas.Children)
                {
                    if (elem2.Visibility == Visibility.Hidden) continue;
                    //if (!elem2.IsVisible) continue;
                    if (elem2 == elem) continue;
                    if (!(elem2 is TextBlock)) continue;

                    TextBlock tb2 = elem2 as TextBlock;
                    Size s2 = MeasureString(tb2, tb2.Text);
                    double top2 = Canvas.GetTop(elem2);
                    double left2 = Canvas.GetLeft(elem2);
                    //Rect r2 = new Rect(new Point(left2, top2), tb2.RenderSize);
                    Rect r2 = new Rect(new Point(left2, top2), s2);
                    if (r.IntersectsWith(r2))
                    {
                        double v1 = ConversionsHelper.GetDoubleFromText(tb.Text);
                        double v2 = ConversionsHelper.GetDoubleFromText(tb2.Text);
                        if (Math.Abs(v1) >= Math.Abs(v2)) tb2.Visibility = Visibility.Hidden;
                        else { tb.Visibility = Visibility.Hidden; break; }
                    }
                } //inner foreach
            } //outer foreach
        }
    }
}
