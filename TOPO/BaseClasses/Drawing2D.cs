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
                    canvasDimensions = MirrorYCoordinates(plate.Dimensions);
                    canvasMemberOutline = MirrorYCoordinates(plate.MemberOutlines);
                    canvasBendLines = MirrorYCoordinates(plate.BendLines);
                    //if (note2D != null) note2D.MirrorYCoordinates();
                }
                else
                {
                    if(plate.PointsOut2D != null) canvasPointsOut = new List<Point>(plate.PointsOut2D);
                    canvasPointsOut_Mirror = Geom2D.MirrorAboutX_ChangeYCoordinates(plate.PointsOut2D);
                    //canvasPointsIn = new List<Point>(PointsIn);
                    if (pHolesCentersPointsScrews2D != null) canvasPointsHolesScrews = new List<Point>(pHolesCentersPointsScrews2D);
                    if (pHolesCentersPointsAnchors2D != null) canvasPointsHolesAnchors = new List<Point>(pHolesCentersPointsAnchors2D);
                    if (plate.DrillingRoutePoints != null) canvasPointsDrillingRoute = new List<Point>(plate.DrillingRoutePoints);
                    if (plate.Dimensions != null) canvasDimensions = new List<CDimension>(plate.Dimensions);
                    if (plate.MemberOutlines != null) canvasMemberOutline = new List<CLine2D>(plate.MemberOutlines);
                    if (plate.BendLines != null) canvasBendLines = new List<CLine2D>(plate.BendLines);
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

                canvasDimensions = ConvertRealPointsToCanvasDrawingPoints(canvasDimensions, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                canvasMemberOutline = ConvertRealPointsToCanvasDrawingPoints(canvasMemberOutline, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                canvasBendLines = ConvertRealPointsToCanvasDrawingPoints(canvasBendLines, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                //canvasNote2D = ConvertRealPointsToCanvasDrawingPoints(note2D, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                // Definition Points
                //vobec nechapem preco treba spravit mirror pre samotne body, ale body ktore sa pouziju pre nakreslenie ciar uz mirorovane netreba
                DrawComponentPoints(bDrawPoints, canvasPointsOut_Mirror, canvasPointsIn, canvasForImage);

                // Outlines
                DrawOutlines(bDrawOutLine, canvasPointsOut, canvasPointsIn, canvasForImage);

                // Definition Point Numbers
                DrawPointNumbers(bDrawPointNumbers, canvasPointsOut, canvasPointsIn, canvasForImage);

                // Holes
                if (pHolesCentersPointsScrews2D != null)
                {
                    DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesScrews, fDiameter_screwPreDrilledHoles * scale_unit, canvasForImage);
                    DrawDrillingRoute(bDrawDrillingRoute, canvasPointsDrillingRoute, canvasForImage);
                }

                if (pHolesCentersPointsAnchors2D != null)
                {
                    DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesAnchors, fDiameter_anchorPreDrilledHoles * scale_unit, canvasForImage);
                }

                // Dimensions
                DrawDimensions(bDrawDimensions, canvasDimensions, canvasForImage);

                // Member Outline
                DrawSeparateLines(bDrawMemberOutline, canvasMemberOutline, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);

                // Bend Lines
                DrawSeparateLines(bDrawBendLines, canvasBendLines, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
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
            double width,
            double height,
            ref Canvas canvasForImage,
            bool bDrawFootingPad = true,
            bool bDrawColumnOutline = true,
            bool bDrawAnchors = true,
            bool bDrawBasePlate = true,
            bool bDrawScrews = true,
            bool bDrawPerimeter = true,
            bool bDrawReinforcement = true,
            bool bDrawDPC_DPM = true,
            bool bDrawDimensions = true,
            bool bDrawNotes = true
            )
        {
            // TODO Ondrej

            // Tuto funckiu potrebujem zrefaktorovat a precistit tak aby bol system vykreslovania podobny ako je pre DrawPlateToCanvas

            // 1. Potrebujem zjednotit tento system podla toho co je lepsie
            // - najprv pripravit vsetky realne suradnice, potom ich previest na canvas units a potom kreslit objekty // DrawPlateToCanvas
            // - pripravit realne suradnice pre dany bool (typ objektov ktore kreslime) , previest na canvas units, kreslit objekty a potom pokracovat pre dalsi bool // DrawFootingPadSideElevationToCanvas

            // To Mato - je to asi jedno, ja by som mozno isiel podla bodu 2 - stale podla bool podla toho co prave ides kreslis

            // 2. Prepocitat vertikalne suradnice -y za +y a opacne - urobit to analogicky ako je v DrawPlateToCanvas

            // 3. Vypocitat scalovaci faktor fReal_Model_Zoom_Factor z rozmerov canvas a toho co sa kresli - vid  DrawPlateToCanvas
            // Nieco som tu uz nadhodil

            // 4. Urcit spravne hodnotu top margin a odsadenie modelu fmodelMarginTop_y
            // Obrazok sa kresli na spodny okraj a nie do stredu, nepredpoklada tato funkcia ConvertRealPointsToCanvasDrawingPoints ze vsetky realne points su rovnakeho znamienka alebo nieco take?

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

            if (joint != null)
            {
                if (joint.m_arrPlates.FirstOrDefault() is CConCom_Plate_B_basic)
                    basePlate = (CConCom_Plate_B_basic)joint.m_arrPlates.FirstOrDefault();

                if (bDrawColumnOutline)
                {
                    crscDepth = joint.m_MainMember.CrScStart.h;
                    horizontalOffsetColumn = -0.5 * crscDepth; // Column

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
            float floorThickness = 0.15f; // TODO - napojit na GUI
            float fPadWidth_y = pad.m_fDim2;
            float fPadDepth_z = pad.m_fDim3;

            float fRealOffset_DPC_DPM = 0.02f; // m // Offset vrstvy DPC / DPM od floor slab alebo footing pad - aproximovana skutocna vzdialenost, aby to bolo dobre na obrazku

            // Suradnica x = 0 je v polovici rozmeru patky 0.5f * fPadWidth_y
            // Suradnica y = 0 je v urovni hornej hrany floor slab

            double horizontalOffset = -0.5 * fPadWidth_y;
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
                new Point(PointsFootingPad_real[0].X, PointsFootingPad_real[0].Y + fRealOffset_DPC_DPM), // Right
                new Point(PointsFootingPad_real[4].X, PointsFootingPad_real[4].Y + fRealOffset_DPC_DPM),  // Left Bottom point
                new Point(horizontalOffset + 0, basePlate.Fl_Z + fVerticalOffsetLeft), // Top Left Column Point
                new Point(horizontalOffset + crscDepth, basePlate.Fl_Z + fVerticalOffsetRight) // Top Right Column Point
            };

            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;

            CalculateModelLimits(PointsForEdgeCoord_real, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);

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

            if (bDrawFootingPad)
            {
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsFootingPad_real);

                List<Point> PointsFootingPad_canvas = ConvertRealPointsToCanvasDrawingPoints(PointsFootingPad_real, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                DrawPolyLine(false, PointsFootingPad_canvas, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);

                if (bDrawDPC_DPM)
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

                    PointsDPC_DPM = ConvertRealPointsToCanvasDrawingPoints(PointsDPC_DPM, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                    DoubleCollection dashes = new DoubleCollection();
                    dashes.Add(10); dashes.Add(10);

                    DrawPolyLine(false, PointsDPC_DPM, Brushes.DarkGreen, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Dash, dashes);
                }

                if (bDrawPerimeter)
                {
                    // TODO - sem potrebujeme dostat rozmery perimeter pre danu stranu floor slab kde sa nachadza patka (left/right, front/back)

                    float fPerimeterWidth = 0.2f; // TODO - napojit
                    float fPerimeterDepth = fPadDepth_z; // TODO - napojit

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

                    DrawPolyLine(false, PointsPerimeter, Brushes.DarkOrange, PenLineCap.Flat, PenLineCap.Flat, 0.8, canvasForImage, DashStyles.Dash, dashes);
                }

                if (bDrawReinforcement)
                {
                    // Vyztuz v smere x kreslime ako kruhy (v reze)
                    // Vyztuz v smere y kreslime ako ciary (v pohlade z boku)

                    double horizontalOffsetReinfocement = 3 * 0.075f; // !!!!! Je potrebne doriesit co tu ma byt - 3x concrete cover ?????????

                    // Reinforcement in LCS x direction - circles
                    if (pad.Top_Bars_x != null && pad.Top_Bars_x.Count > 0)
                    {
                        for (int i = 0; i < pad.Top_Bars_x.Count; i++)
                        {
                            Point p = new Point(horizontalOffsetReinfocement + pad.Top_Bars_x[i].m_pControlPoint.Y, pad.Top_Bars_x[i].m_pControlPoint.Z);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref p);
                            p = ConvertRealPointToCanvasDrawingPoint(p, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            DrawCircle(p, dReal_Model_Zoom_Factor * pad.Top_Bars_x[i].Diameter /*pad.Reference_Top_Bar_x.Diameter*/, Brushes.Black, Brushes.LightGray, 1, canvasForImage);
                        }
                    }

                    if (pad.Bottom_Bars_x != null && pad.Bottom_Bars_x.Count > 0)
                    {
                        for (int i = 0; i < pad.Bottom_Bars_x.Count; i++)
                        {
                            Point p = new Point(horizontalOffsetReinfocement + pad.Bottom_Bars_x[i].m_pControlPoint.Y, pad.Bottom_Bars_x[i].m_pControlPoint.Z);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref p);
                            p = ConvertRealPointToCanvasDrawingPoint(p, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                            DrawCircle(p, dReal_Model_Zoom_Factor * pad.Bottom_Bars_x[i].Diameter /*pad.Reference_Bottom_Bar_x.Diameter*/, Brushes.Black, Brushes.LightGray, 1, canvasForImage);
                        }
                    }

                    double dLineThicknessFactor = dReal_Model_Zoom_Factor; //  0.4 * 1000; // TODO Ondrej - Vhodne nastavit zavislost hrubky ciary a priemeru vyztuze

                    // Reinforcement in LCS y direction - lines
                    if (pad.Top_Bars_y != null && pad.Top_Bars_y.Count > 0)
                    {
                        // Kreslime len prvy prut
                        // TODO - potrebujeme prerobit z jednoduchej ciary na tvar U, alebo obecny tvar spline (striedanie oblucikov a rovnych segmentov)
                        Point pStart = new Point(horizontalOffsetReinfocement + pad.Top_Bars_y[0].StartPoint.Y, pad.Top_Bars_y[0].StartPoint.Z);
                        Point pEnd = new Point(horizontalOffsetReinfocement + pad.Top_Bars_y[0].EndPoint.Y, pad.Top_Bars_y[0].EndPoint.Z);

                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref pStart);
                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref pEnd);

                        pStart = ConvertRealPointToCanvasDrawingPoint(pStart, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                        pEnd = ConvertRealPointToCanvasDrawingPoint(pEnd, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                        DrawPolyLine(false, new List<Point> { pStart, pEnd }, Brushes.DarkSeaGreen, PenLineCap.Flat, PenLineCap.Flat, dLineThicknessFactor * pad.Top_Bars_y[0].Diameter, canvasForImage, DashStyles.Solid, null);
                    }

                    if (pad.Bottom_Bars_y != null && pad.Bottom_Bars_y.Count > 0)
                    {
                        // Kreslime len prvy prut
                        // TODO - potrebujeme prerobit z jednoduchej ciary na tvar U, alebo obecny tvar spline (striedanie oblucikov a rovnych segmentov)
                        Point pStart = new Point(horizontalOffsetReinfocement + pad.Bottom_Bars_y[0].StartPoint.Y, pad.Bottom_Bars_y[0].StartPoint.Z);
                        Point pEnd = new Point(horizontalOffsetReinfocement + pad.Bottom_Bars_y[0].EndPoint.Y, pad.Bottom_Bars_y[0].EndPoint.Z);

                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref pStart);
                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref pEnd);

                        pStart = ConvertRealPointToCanvasDrawingPoint(pStart, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                        pEnd = ConvertRealPointToCanvasDrawingPoint(pEnd, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                        DrawPolyLine(false, new List<Point> { pStart, pEnd }, Brushes.DarkTurquoise, PenLineCap.Flat, PenLineCap.Flat, dLineThicknessFactor * pad.Bottom_Bars_y[0].Diameter, canvasForImage, DashStyles.Solid, null);
                    }
                }
            }

            if (bDrawColumnOutline)
            {
                const short numberOfStiffeners = 2; // TODO napojit na parametre a pozicie prierezu
                double[] stiffenersHorizontalPositions = new double[numberOfStiffeners] { 0.4 * crscDepth, 0.6 * crscDepth  }; // TODO - napojit na pole pozicii hran alebo vyztuh prierezu

                List<Point> PointsStiffenersBottom = new List<Point>();
                List<Point> PointsStiffenersIntermediate = new List<Point>();
                List<Point> PointsStiffenersTop = new List<Point>();

                // Sfiffeners Edges
                for(int i = 0; i < stiffenersHorizontalPositions.Length; i++)
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

                        DrawLine(l, Brushes.Tomato, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Dash, dashes);
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

                        DrawLine(l, Brushes.Turquoise, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Solid);
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

                DrawLine(l_Left, Brushes.Tomato, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Solid);

                // Right Line
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref bottomRight_ColumnEdge);
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref topRight_ColumnEdge);

                List<Point> PointsLineRight = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { bottomRight_ColumnEdge, topRight_ColumnEdge }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                Line l_Right = new Line();
                l_Right.X1 = PointsLineRight[0].X;
                l_Right.Y1 = PointsLineRight[0].Y;

                l_Right.X2 = PointsLineRight[1].X;
                l_Right.Y2 = PointsLineRight[1].Y;

                DrawLine(l_Right, Brushes.Tomato, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Solid);

                // Top Line
                Line l_Top = new Line();
                l_Top.X1 = PointsLineLeft[1].X;
                l_Top.Y1 = PointsLineLeft[1].Y;

                l_Top.X2 = PointsLineRight[1].X;
                l_Top.Y2 = PointsLineRight[1].Y;

                DrawLine(l_Top, Brushes.Tomato, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Solid);
            }

            if (basePlate != null)
            {
                // Draw Plate Outline
                if (bDrawBasePlate)
                {
                    Point insertingPoint_Plate = new Point(0, 0); // TODO - doplnit napojenie na excentricity

                    Point lt_Plate = new Point(insertingPoint_Plate.X - basePlate.Fh_Y * 0.5, insertingPoint_Plate.Y + basePlate.Fl_Z);
                    Point br_Plate = new Point(insertingPoint_Plate.X + basePlate.Fh_Y * 0.5, insertingPoint_Plate.Y); // TODO - ??? Toto by y malo byt zaporne a potom sa preklopit

                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_Plate);
                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_Plate);

                    List<Point> PointsPlate = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_Plate, br_Plate }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                    DrawRectangle(Brushes.DarkGreen, null, 1, canvasForImage, PointsPlate[0], PointsPlate[1]);

                    // Obrys vnutornej hrany

                    Point left = new Point(lt_Plate.X, lt_Plate.Y + basePlate.Ft);
                    Point right = new Point(br_Plate.X, lt_Plate.Y + basePlate.Ft);

                    List <Point> PointsPlateLine = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { left, right }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                    Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsPlateLine);
                    Line l = new Line();
                    l.X1 = PointsPlateLine[0].X;
                    l.Y1 = PointsPlateLine[0].Y;

                    l.X2 = PointsPlateLine[1].X;
                    l.Y2 = PointsPlateLine[1].Y;

                    DrawLine(l, Brushes.Brown, PenLineCap.Flat, PenLineCap.Flat, 0.7, canvasForImage, DashStyles.Dash);

                    if(bDrawScrews)
                    {
                        bool bDrawHoles = true;
                        bool bDrawHoleCentreSymbols = true;

                        List<Point> PointsHolesScrews = basePlate.ScrewArrangement.HolesCentersPoints2D.ToList();

                        List<Point> canvasPointsHolesScrews = new List<Point>();

                        for(int i = 0; i<PointsHolesScrews.Count / 2; i++) // Kreslime len polovicu bodov
                        {
                            // Potrebujeme zamenit suradnice x a y
                            canvasPointsHolesScrews.Add(new Point(-lt_Plate.X - PointsHolesScrews[i].Y, -lt_Plate.Y - PointsHolesScrews[i].X)); // -lt_Plate.Y - uz bolo preklopene uvazujem kladnu hodnotu
                        }

                        Geom2D.MirrorAboutX_ChangeYCoordinates(ref canvasPointsHolesScrews);
                        canvasPointsHolesScrews = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHolesScrews, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                        double dHolesDiameterScrews = basePlate.ScrewArrangement.referenceScrew.Diameter_shank * dReal_Model_Zoom_Factor;

                        DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesScrews, dHolesDiameterScrews, canvasForImage, 2);
                    }
                }

                // Draw Anchors
                if (basePlate.AnchorArrangement.Anchors != null && basePlate.AnchorArrangement.Anchors.Length > 0)
                {
                    foreach (CAnchor anchor in basePlate.AnchorArrangement.Anchors)
                    {
                        // TODO Implementovat funckiu ktora vykresli jednu anchors + washers

                        // Anchor Bar
                        if (bDrawAnchors)
                        {
                            float fAnchorDiameter = anchor.Diameter_shank;
                            float fAnchorLength = anchor.Length;
                            Point insertingPoint = new Point(-basePlate.Fh_Y * 0.5 + anchor.m_pControlPoint.Y, anchor.m_pControlPoint.Z);

                            Point lt = new Point(insertingPoint.X - fAnchorDiameter * 0.5, insertingPoint.Y);
                            Point br = new Point(insertingPoint.X + fAnchorDiameter * 0.5, insertingPoint.Y - fAnchorLength); // TODO - ??? Toto by y malo byt zaporne a potom sa preklopit

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref br);

                            List<Point> PointsAnchor = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt, br }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                            DrawRectangle(Brushes.DarkGreen, null, 1, canvasForImage, PointsAnchor[0], PointsAnchor[1]);

                            // Washers

                            // Washer - Plate
                            float fPlateWasherWidth = anchor.y_washer_plate;
                            float fPlateWasherThickness = 0.008f; // TO napojit na GUI ???
                            float fPlateWasherOffsetFromTop = (float)anchor.m_pControlPoint.Z - fPlateWasherThickness; // TO napojit na GUI ???

                            Point lt_WasherPlate = new Point(insertingPoint.X - fPlateWasherWidth * 0.5, insertingPoint.Y - fPlateWasherOffsetFromTop);
                            Point br_WasherPlate = new Point(insertingPoint.X + fPlateWasherWidth * 0.5, insertingPoint.Y - fPlateWasherOffsetFromTop - fPlateWasherThickness); // TODO - ??? Toto by y malo byt zaporne a potom sa preklopit

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_WasherPlate);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_WasherPlate);

                            List<Point> PointsPlateWasher = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_WasherPlate, br_WasherPlate }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                            DrawRectangle(Brushes.DarkMagenta, null, 1, canvasForImage, PointsPlateWasher[0], PointsPlateWasher[1]);

                            // Washer - Bearing
                            float fBearingWasherWidth = anchor.y_washer_bearing;
                            float fBearingWasherThickness = 0.006f; // TO napojit na GUI ???
                            float fBearingWasherOffsetFromTop = fAnchorLength - 0.05f; // TO napojit na GUI ???

                            Point lt_BearingWasher = new Point(insertingPoint.X - fBearingWasherWidth * 0.5, insertingPoint.Y - fBearingWasherOffsetFromTop);
                            Point br_BearingWasher = new Point(insertingPoint.X + fBearingWasherWidth * 0.5, insertingPoint.Y - fBearingWasherOffsetFromTop - fBearingWasherThickness); // TODO - ??? Toto by y malo byt zaporne a potom sa preklopit

                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref lt_BearingWasher);
                            Geom2D.MirrorAboutX_ChangeYCoordinates(ref br_BearingWasher);

                            List<Point> PointsBearingWasher = ConvertRealPointsToCanvasDrawingPoints(new List<Point> { lt_BearingWasher, br_BearingWasher }, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
                            DrawRectangle(Brushes.DarkMagenta, null, 1, canvasForImage, PointsBearingWasher[0], PointsBearingWasher[1]);
                        }
                    }
                }
            }

            if(bDrawDimensions)
            {
                List<CDimension> Dimensions = new List<CDimension>(); // Real

                Point center = new Point(0,0); // TO Ondrej - toto by mal byt asi stred obrazku

                // Vertical Dimensions
                Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[4], PointsFootingPad_real[5], true, true)); // Vertical Dimension - footing pad
                Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[0], PointsFootingPad_real[6], false, true)); // Vertical Dimension - floor slab
                Dimensions.Add(new CDimensionLinear(center, new Point(PointsFootingPad_real[0].X, PointsFootingPad_real[3].Y), PointsFootingPad_real[0], false, true)); // Vertical Dimension - footing pad to floor slab bottom surface

                // Horizontal Dimensions
                Dimensions.Add(new CDimensionLinear(center, PointsFootingPad_real[5], bottomLeft_ColumnEdge, true, false)); // Horizontal Dimension - footing pad edge to column

                //canvasDimensions = MirrorYCoordinates(Dimensions.ToArray()); // Nezrkadlime body lebo uz boli zrkadlene pre vykreslenie patky atd
                List<CDimension> canvasDimensions = Dimensions;

                canvasDimensions = ConvertRealPointsToCanvasDrawingPoints(canvasDimensions, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                // Dimensions
                DrawDimensions(bDrawDimensions, canvasDimensions, canvasForImage);
            }

            if (bDrawNotes)
            {
                CNote2D[] notes2D = null; 
                List<CNote2D> canvasNotes2D = null;
                //canvasNotes2D = MirrorYCoordinates(notes2D);
                //canvasNotes2D = ConvertRealPointsToCanvasDrawingPoints(notes2D, fTempMin_X, fTempMin_Y, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

                //Notes
                //if (notes2D != null) DrawNote(canvasNotes2D, canvasForImage);
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
                    note2D = new CNote2D(new Point(kb.pTip.X + moveX, kb.pTip.Y + moveY), "Trim Off", 0, 0, true, kb.pTip, new Point(kb.pTip.X + 40, kb.pTip.Y + 40), plateCenter);
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
                    note2D = new CNote2D(new Point(kc.pTip.X + moveX, kc.pTip.Y + moveY), "Trim Off", 0, 0, true, kc.pTip, new Point(kc.pTip.X + 40, kc.pTip.Y + 40), plateCenter);
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
                    note2D = new CNote2D(new Point(kd.pTip.X + moveX, kd.pTip.Y + moveY), "Trim Off", 0, 0, true, kd.pTip, new Point(kd.pTip.X + 40, kd.pTip.Y + 40), plateCenter);
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
                    note2D = new CNote2D(new Point(ke.pTip.X + moveX, ke.pTip.Y + moveY), "Trim Off", 0, 0, true, ke.pTip, new Point(ke.pTip.X + 40, ke.pTip.Y + 40), plateCenter);
                }
            }
            return note2D;
        }

        public static void DrawScrewToCanvas(CScrew screw, double width, double height, ref Canvas canvasForImage, bool bDrawCentreSymbol)
        {
            float fScaleFactor = 0.5f; // 50% of canvas
            int scale_unit = 1000; // mm

            float fModel_Length_x_page;
            float fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            float fReal_Model_Zoom_Factor;
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
                out fModel_Length_x_page,
                out fModel_Length_y_page,
                out dFactor_x,
                out dFactor_y,
                out fReal_Model_Zoom_Factor,
                out fmodelMarginLeft_x,
                out fmodelMarginTop_y,
                out fmodelMarginBottom_y
                );

            Point pCenterPoint = new Point(width / 2, height / 2);

            // Head Inside Circle
            DrawCircle(pCenterPoint, fReal_Model_Zoom_Factor * screw.D_h_headdiameter, Brushes.Black, null, 1, canvasForImage);

            // Head Hexagon
            float a = (0.5f * screw.D_h_headdiameter) / (float)Math.Cos(30f / 180f * Math.PI);
            List<Point> headpoints = Geom2D.GetHexagonPointCoord(a); // Diameter of outside circle

            // TODO - upravit podla toho ci bude v databaze vnutorny alebo vonkajsi rozmer sesthrannej hlavy (opisana alebo vpisana kruznica)
            float fInsideDiameterFactor = 0.5f / (float)Math.Tan(30f / 180f * Math.PI); // Radius of inside circle of hexagon

            double dCanvasTop = (height - (fReal_Model_Zoom_Factor * screw.D_h_headdiameter)) / 2;
            double dCanvasLeft = (width - (fReal_Model_Zoom_Factor * 2 * a/* fInsideDiameterFactor * screw.D_h_headdiameter*/)) / 2;
            DrawPolyLine(true, headpoints, dCanvasTop, dCanvasLeft, fmodelMarginLeft_x, fmodelMarginBottom_y, fReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);

            // Washer Circle
            DrawCircle(pCenterPoint, fReal_Model_Zoom_Factor * screw.D_w_washerdiameter, Brushes.Black, null, 1, canvasForImage);

            // Draw Symbol of Center
            if (bDrawCentreSymbol)
                DrawSymbol_Cross(pCenterPoint, fReal_Model_Zoom_Factor * screw.D_w_washerdiameter + 20, Brushes.Red, 1, canvasForImage);
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
                canvasDimensions = MirrorYCoordinates(Dimensions);
                canvasMemberOutline = MirrorYCoordinates(MemberOutline);
                canvasBendLines = MirrorYCoordinates(BendLines);
                if (note2D != null) note2D.MirrorYCoordinates();
            }
            else
            {
                canvasPointsOut = new List<Point>(PointsOut);
                canvasPointsIn = new List<Point>(PointsIn);
                canvasPointsHolesScrews = new List<Point>(PointsHolesScrews);
                canvasPointsHolesAnchors = new List<Point>(PointsHolesAnchors);
                canvasPointsDrillingRoute = new List<Point>(PointsDrillingRoute);
                canvasDimensions = new List<CDimension>(Dimensions);
                canvasMemberOutline = new List<CLine2D>(MemberOutline);
                canvasBendLines = new List<CLine2D>(BendLines);
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
            DrawComponentPoints(bDrawPoints, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Outlines
            DrawOutlines(bDrawOutLine, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Definition Point Numbers
            DrawPointNumbers(bDrawPointNumbers, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Holes
            if (PointsHolesScrews != null)
            {
                DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesScrews, dHolesDiameterScrews, canvasForImage);
                DrawDrillingRoute(bDrawDrillingRoute, canvasPointsDrillingRoute, canvasForImage);
            }

            if (PointsHolesAnchors != null)
            {
                DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHolesAnchors, dHolesDiameterAnchors, canvasForImage);
            }

            // Dimensions
            DrawDimensions(bDrawDimensions, canvasDimensions, canvasForImage);

            // Member Outline
            DrawSeparateLines(bDrawMemberOutline, canvasMemberOutline, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);

            // Bend Lines
            DrawSeparateLines(bDrawBendLines, canvasBendLines, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);

            //Notes
            if (note2D != null) DrawNote(canvasNote2D, canvasForImage);
        }
        
        private static List<CDimension> MirrorYCoordinates(CDimension[] Dimensions)
        {
            if (Dimensions == null) return new List<CDimension>();
            List<CDimension> listDimensions = new List<CDimension>(Dimensions);
            foreach (CDimension d in listDimensions)
            {
                d.MirrorYCoordinates();
            }
            return listDimensions;
        }

        private static List<CLine2D> MirrorYCoordinates(CLine2D[] lines)
        {
            if (lines == null) return new List<CLine2D>();
            List<CLine2D> listLines = new List<CLine2D>(lines);
            foreach (CLine2D l in listLines)
            {
                l.MirrorYCoordinates();
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
            double fScale_Factor, // 0-1
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

            dModel_Length_x_page = scale_unit * dModel_Length_x_real;
            dModel_Length_y_page = scale_unit * dModel_Length_y_real;

            // Calculate maximum zoom factor
            // Original ratio
            dFactor_x = dModel_Length_x_page / dPageWidth;
            dFactor_y = dModel_Length_y_page / dPageHeight;

            // Calculate new model dimensions (zoom of model size is 80%)
            dReal_Model_Zoom_Factor = fScale_Factor / Math.Max(dFactor_x, dFactor_y) * scale_unit;

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

        public static void CalculateBasicValue(
            float fModel_Length_x_real,
            float fModel_Length_y_real,
            float fScale_Factor, // zoom ratio 0-1 (zoom of 2D view), default 0.8 (80%)
            int scale_unit,
            double dPageWidth,
            double dPageHeight,
            out float fModel_Length_x_page,
            out float fModel_Length_y_page,
            out double dFactor_x,
            out double dFactor_y,
            out float fReal_Model_Zoom_Factor,
            out float fmodelMarginLeft_x,
            out float fmodelMarginTop_y,
            out float fmodelMarginBottom_y
            )
        {
            fModel_Length_x_page = scale_unit * fModel_Length_x_real;
            fModel_Length_y_page = scale_unit * fModel_Length_y_real;

            // Calculate maximum zoom factor
            // Original ratio
            dFactor_x = fModel_Length_x_page / dPageWidth;
            dFactor_y = fModel_Length_y_page / dPageHeight;

            // Calculate new model dimensions (zoom of model is defined by factor 0.0 - 1.0)
            fReal_Model_Zoom_Factor = fScale_Factor / (float)MathF.Max(dFactor_x, dFactor_y) * scale_unit;

            // Set new size of model on the page
            fModel_Length_x_page = fReal_Model_Zoom_Factor * fModel_Length_x_real;
            fModel_Length_y_page = fReal_Model_Zoom_Factor * fModel_Length_y_real;

            fmodelMarginLeft_x = (float)(0.5 * (dPageWidth - fModel_Length_x_page));
            fmodelMarginTop_y = (float)(0.5 * (dPageHeight - fModel_Length_y_page));

            fmodelMarginBottom_y = (float)(fModel_Length_y_page + 0.5 * (dPageHeight - fModel_Length_y_page));
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
        public static void DrawComponentPoints(bool bDrawPoints, List<Point> PointsOut, List<Point> PointsIn, Canvas canvasForImage)
        {
            if (bDrawPoints)
            {
                DrawPoints(bDrawPoints, PointsOut, canvasForImage);
                DrawPoints(bDrawPoints, PointsIn, canvasForImage);
            }
        }
        public static void DrawPoints(bool bDrawPoints, List<Point> points, Canvas canvasForImage)
        {
            foreach (Point p in points)
            {
                DrawPoint(p, Brushes.Red, Brushes.Red, 4, canvasForImage);
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

        public static void DrawOutlines(bool bDrawOutLine, List<Point> PointsOut, List<Point> PointsIn, Canvas canvasForImage)
        {
            if (bDrawOutLine)
            {
                DrawPolyLine(true, PointsOut, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
                DrawPolyLine(true, PointsIn, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
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

        public static void DrawPointNumbers(bool bDrawPointNumbers, List<Point> PointsOut, List<Point> PointsIn, Canvas canvasForImage)
        {
            if (bDrawPointNumbers)
            {
                // Outer outline points
                if (PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsOut.Count; i++)
                    {
                        DrawText((i + 1).ToString(), PointsOut[i].X, PointsOut[i].Y, 0, 16, false, Brushes.Blue, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null && PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsIn.Count; i++)
                    {
                        DrawText((i + 1).ToString(), PointsIn[i].X, PointsIn[i].Y, 0, 16, false, Brushes.Green, canvasForImage);
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

        public static void DrawHoles(bool bDrawHoles, bool bDrawHoleCentreSymbols, List<Point> PointsHoles, double dHolesDiameter, Canvas canvasForImage, double SymbolOffsetFromHoleCircle = 5)
        {
            if (bDrawHoles)
            {
                // Holes
                if (PointsHoles != null) // If is array of holes centers is not empty
                {
                    for (int i = 0; i < PointsHoles.Count; i++)
                    {
                        // Draw Hole
                        DrawCircle(PointsHoles[i], dHolesDiameter, Brushes.Black, null, 1, canvasForImage);

                        // Draw Symbol of Center
                        if (bDrawHoleCentreSymbols) DrawSymbol_Cross(PointsHoles[i], dHolesDiameter + 2 * SymbolOffsetFromHoleCircle, Brushes.Red, 1, canvasForImage);
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

        public static void DrawDrillingRoute(bool bDrawDrillingRoute, List<Point> PointsDrillingRoute, Canvas canvasForImage)
        {
            if (!bDrawDrillingRoute || PointsDrillingRoute == null) return;

            DrawPolyLine(false, PointsDrillingRoute, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);
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

        public static void DrawDimensions(bool bDrawDimensions, List<CDimension> Dimensions, Canvas canvasForImage)
        {
            if (bDrawDimensions && Dimensions != null && Dimensions.Count > 0)
            {
                for (int i = 0; i < Dimensions.Count; i++) // Pole kot
                {
                    if (Dimensions[i] is CDimensionLinear)
                    {
                        CDimensionLinear dim = (CDimensionLinear)Dimensions[i];
                        DrawSimpleLinearDimension(dim, true, canvasForImage);
                    }
                    else if (Dimensions[i] is CDimensionArc)
                    {
                        CDimensionArc dim = (CDimensionArc)Dimensions[i];
                        DrawArcDimension(dim.ControlPointStart, dim.ControlPointEnd, dim.ControlPointCenter, canvasForImage);
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
            //todo implementacion
            DrawText(note.Text, note.NotePoint.X, note.NotePoint.Y, 0, 12, Brushes.Black, canvasForImage);

            //if (note.DrawArrow)
            //{
            //    Line line = new Line();
            //    line.X1 = note.ArrowPoint1.X;
            //    line.Y1 = note.ArrowPoint1.Y;
            //    line.X2 = note.ArrowPoint2.X;
            //    line.Y2 = note.ArrowPoint2.Y;
            //    DrawLine(line, new SolidColorBrush(Colors.Red), PenLineCap.Flat, PenLineCap.Triangle, 3, canvasForImage, null);
            //}
        }

        public static void DrawSeparateLines(bool bDrawLines, List<CLine2D> lines, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas canvasForImage)
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
                        DrawLine(l, color, startCap, endCap, thickness, canvasForImage, DashStyles.Dash, dashes);
                    }
                }
            }
        }

        public static void DrawPoint(Point point, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas)
        {
            DrawRectangle(strokeColor, fillColor, thickness, imageCanvas, new Point(point.X - 0.5 * thickness, point.Y - 0.5 * thickness), new Point(point.X + 0.5 * thickness, point.Y + 0.5 * thickness));
        }

        public static void DrawLine(Line line, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas, DashStyle dashStyle, DoubleCollection dashArray = null)
        {
            //Random r = new Random();
            //Color randomcolor = Color.FromArgb((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
            //SolidColorBrush b = new SolidColorBrush(randomcolor);

            Line myLine = new Line();
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
            SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
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
        public static void DrawPolyLine(bool bIsClosed, List<Point> listPoints, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas, DashStyle dashStyle = null, DoubleCollection dashArray = null)
        {
            if (listPoints == null) return;
            if (listPoints.Count < 2) return;

            double canvasLeft = listPoints.Min(p => p.X);
            double canvasTop = listPoints.Min(p => p.Y);

            PointCollection points = new PointCollection(listPoints);
            if (bIsClosed) points.Add(listPoints.ElementAt(0));

            Polyline myLine = new Polyline();
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
        public static void DrawCircle(Point center, double diameter, SolidColorBrush colorStroke, SolidColorBrush colorFill, double thickness, Canvas imageCanvas)
        {
            if (!Double.IsNaN(center.X))
            {
                Ellipse circle = new Ellipse();
                circle.Height = diameter;
                circle.Width = diameter;
                circle.StrokeThickness = thickness;
                circle.Stroke = colorStroke;
                if(colorFill != null)
                  circle.Fill = colorFill;

                double left = center.X - (diameter / 2) + thickness / 2;
                double top = center.Y - (diameter / 2) + thickness / 2;
                Canvas.SetLeft(circle, left);
                Canvas.SetTop(circle, top);
                imageCanvas.Children.Add(circle);
            }
        }

        public static void DrawSymbol_Cross(Point center, double size, SolidColorBrush color, double thickness, Canvas imageCanvas)
        {
            if (!Double.IsNaN(center.X)) // Check that value is not "NaN" - TODO - Ondrej Bug No. 109
            {
                Line l = new Line();

                double fSideLength = 0.5f * size;

                // Horizontal
                l.X1 = center.X - fSideLength;
                l.Y1 = center.Y;

                l.X2 = center.X + fSideLength;
                l.Y2 = center.Y;

                DrawLine(l, color, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas, DashStyles.Solid);

                l.X1 = center.X;
                l.Y1 = center.Y - fSideLength;

                l.X2 = center.X;
                l.Y2 = center.Y + fSideLength;

                DrawLine(l, color, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas, DashStyles.Solid);
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

        public static void DrawSimpleLinearDimension(CDimensionLinear dim, bool bDrawExtensionLines, Canvas imageCanvas)
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

            double dPrimaryLineThickness = 1;
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
            double dExtensionLineThickness = 1;

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
            double dSlopeLineThickness = 1;

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
            DrawLine(lPrimaryLine, Brushes.DarkGreen, PenLineCap.Flat, PenLineCap.Flat, dPrimaryLineThickness, imageCanvas, DashStyles.Solid);
            // Draw extension line - start
            DrawLine(lExtensionLine1, Brushes.DarkGreen, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas, DashStyles.Solid);
            // Draw extension line - end
            DrawLine(lExtensionLine2, Brushes.DarkGreen, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas, DashStyles.Solid);
            // Draw slope line - start
            DrawLine(lSlopeLine1, Brushes.DarkGreen, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas, DashStyles.Solid);
            // Draw slope line - end
            DrawLine(lSlopeLine2, Brushes.DarkGreen, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas, DashStyles.Solid);
            // Draw text            
            DrawText(sText, textPositionx, textPositiony, dRotation_deg, 12, dim.ControlPointRef, dim.IsTextOutSide, Brushes.DarkGreen, imageCanvas);
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

        public static void DrawArcDimension(Point pStart, Point pEnd, Point pCenter, Canvas imageCanvas)
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
            DrawLine(l1, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas, DashStyles.Dash, dashArray);

            Line l2 = new Line();
            l2.X1 = pCenter.X;
            l2.Y1 = pCenter.Y;
            l2.X2 = pEnd.X;
            l2.Y2 = pEnd.Y;
            DrawLine(l2, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas, DashStyles.Dash, dashArray);

            // Draw text
            // Draw text in the middle of the arc
            //double fTextPositionx = pathStartPoint.X + (pathTowardsPoint.X - pathStartPoint.X) / 2;
            //double fTextPositiony = pathStartPoint.Y + (pathTowardsPoint.Y - pathStartPoint.Y) / 2;
            //double fTextPositionx = (pathStartPoint.X + pathTowardsPoint.X + pCenter.X) / 3;
            //double fTextPositiony = (pathStartPoint.Y + pathTowardsPoint.Y + pCenter.Y) / 3;
            double fTextPositionx = (pathStartPoint.X * 3 + pathTowardsPoint.X * 3 + pCenter.X) / 7;
            double fTextPositiony = (pathStartPoint.Y * 3 + pathTowardsPoint.Y * 3 + pCenter.Y) / 7;
            string sText = Math.Round(slopeDeg, 1).ToString() + " °";

            DrawText(sText, fTextPositionx, fTextPositiony, 0, 12, Brushes.Black, imageCanvas);
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

        public static void DrawRectangle(SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas canvas, Point lt, Point br)
        {
            Rectangle rect = new Rectangle();
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
        public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, bool bIsTextAboveControlPoint, SolidColorBrush color, Canvas canvas)
        {
            TextBlock textBlock = new TextBlock();
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

        public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, SolidColorBrush color, Canvas canvas)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            //textBlock.Background = new SolidColorBrush(Colors.Red);
            textBlock.FontSize = fontSize;
            Size txtSize = MeasureString(textBlock, text);

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

        public static void DrawText(string text, double posx, double posy, double rotationAngle_CW_deg, double fontSize, Point refPoint, bool bIsTextOutSide, SolidColorBrush color, Canvas canvas)
        {
            TextBlock textBlock = new TextBlock();
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
