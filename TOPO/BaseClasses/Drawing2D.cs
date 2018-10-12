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
                    scale_unit,
                    width,
                    height,
                    crsc.CrScPointsOut,
                    crsc.CrScPointsIn,
                    null,
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
                    crsc.CrScPointsOut,
                    crsc.CrScPointsIn,
                    null,
                    null,
                    null, // TODO - dodefinovat i pre prierezy                    
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

        public static void DrawPlateToCanvas(CPlate plate, double width, double height, ref Canvas canvasForImage,
            bool bDrawPoints, bool bDrawOutLine, bool bDrawPointNumbers, bool bDrawHoles, bool bDrawHoleCentreSymbols, bool bDrawDrillingRoute, bool bDrawDimensions)
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

            Point[] pHolesCentersPoints2D = null;

            // Check that object of screw arrangement is not null and set array items to the temporary array
            if (plate.ScrewArrangement != null && plate.ScrewArrangement.HolesCentersPoints2D != null)
                pHolesCentersPoints2D = plate.ScrewArrangement.HolesCentersPoints2D;

            CalculateBasicValue(
                    fTempMax_X,
                    fTempMin_X,
                    fTempMax_Y,
                    fTempMin_Y,
                    scale_unit,
                    width,
                    height,
                    Geom2D.TransformArrayToList(plate.PointsOut2D),
                    null,
                    pHolesCentersPoints2D,
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

            float fDiameter = 0;

            if(plate.ScrewArrangement != null && plate.ScrewArrangement.referenceScrew != null)
                fDiameter = plate.ScrewArrangement.referenceScrew.Diameter_thread;

            if (plate is CConCom_Plate_BB_BG) // Ak je plech totoho typu mozu sa vykreslovat objekty typu anchors alebo screws (screws som zatial nezadefinoval)
            {
                // TODO - prepracovat na Anchor Arrangement

                // Ak je plech typu B - zakladova patka, vykreslit priemer z anchor
                CConCom_Plate_BB_BG temp_plate = plate as CConCom_Plate_BB_BG;
                fDiameter = temp_plate.referenceAnchor.Diameter_thread;
            }

            canvasForImage.Children.Clear();
            if (plate != null)
                DrawComponent(
                    bDrawPoints,
                    bDrawOutLine,
                    bDrawPointNumbers,
                    bDrawHoles,
                    bDrawHoleCentreSymbols,
                    bDrawDrillingRoute,
                    bDrawDimensions,
                    Geom2D.TransformArrayToList(plate.PointsOut2D),
                    null,
                    pHolesCentersPoints2D,
                    plate.DrillingRoutePoints,
                    plate.Dimensions,
                    fDiameter * scale_unit,
                    fmodelMarginLeft_x,
                    fmodelMarginBottom_y,
                    dReal_Model_Zoom_Factor,
                    fModel_Length_y_page,
                    dPointInOutDistance_y_page,
                    dPointInOutDistance_x_page,
                    true,
                    canvasForImage);
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
                out fmodelMarginBottom_y
                );

            Point pCenterPoint = new Point(width / 2, height / 2);

            // Head Inside Circle
            DrawCircle(pCenterPoint, fReal_Model_Zoom_Factor * screw.D_h_headdiameter, Brushes.Black, 1, canvasForImage);

            // Head Hexagon            
            float a = (0.5f * screw.D_h_headdiameter) / (float)Math.Cos(30f / 180f * Math.PI);
            List<Point> headpoints = Geom2D.GetHexagonPointCoord(a); // Diameter of outside circle

            // TODO - upravit podla toho ci bude v databaze vnutorny alebo vonkajsi rozmer sesthrannej hlavy (opisana alebo vpisana kruznica)
            float fInsideDiameterFactor = 0.5f / (float)Math.Tan(30f / 180f * Math.PI); // Radius of inside circle of hexagon

            double dCanvasTop = (height - (fReal_Model_Zoom_Factor * screw.D_h_headdiameter)) / 2;
            double dCanvasLeft = (width - (fReal_Model_Zoom_Factor * 2* a/* fInsideDiameterFactor * screw.D_h_headdiameter*/)) / 2;
            DrawPolyLine(true, headpoints, dCanvasTop, dCanvasLeft, fmodelMarginLeft_x, fmodelMarginBottom_y, fReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 1, canvasForImage);

            // Washer Circle
            DrawCircle(pCenterPoint, fReal_Model_Zoom_Factor * screw.D_w_washerdiameter, Brushes.Black, 1, canvasForImage);

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
            List<Point> PointsOut,
            List<Point> PointsIn,
            Point[] PointsHoles,
            List<Point> PointsDrillingRoute,
            CDimension[] Dimensions,
            double dHolesDiameter,
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
            List<Point> canvasPointsHoles = null;
            List<Point> canvasPointsDrillingRoute = null;
            List<CDimension> canvasDimensions = null;

            if (bPointsHaveYinUpDirection)
            {
                canvasPointsOut = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsOut);
                canvasPointsIn = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsIn);
                canvasPointsHoles = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsHoles);
                canvasPointsDrillingRoute = Geom2D.MirrorAboutX_ChangeYCoordinates(PointsDrillingRoute);
                canvasDimensions = MirrorYCoordinates(Dimensions);
            }
            else
            {
                canvasPointsOut = new List<Point>(PointsOut);
                canvasPointsIn = new List<Point>(PointsIn);
                canvasPointsHoles = new List<Point>(PointsHoles);
                canvasPointsDrillingRoute = new List<Point>(PointsDrillingRoute);
                canvasDimensions = new List<CDimension>(Dimensions);

            }
            double minX = canvasPointsOut.Min(p => p.X);
            double minY = canvasPointsOut.Min(p => p.Y);

            canvasPointsOut = ConvertRealPointsToCanvasDrawingPoints(canvasPointsOut, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsIn = ConvertRealPointsToCanvasDrawingPoints(canvasPointsIn, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsHoles = ConvertRealPointsToCanvasDrawingPoints(canvasPointsHoles, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasPointsDrillingRoute = ConvertRealPointsToCanvasDrawingPoints(canvasPointsDrillingRoute, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);
            canvasDimensions = ConvertRealPointsToCanvasDrawingPoints(canvasDimensions, minX, minY, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor);

            // Definition Points
            //DrawPoints(bDrawPoints, canvasPointsOut, canvasPointsIn, fmodelMarginLeft_x, fmodelMarginTop_y, dReal_Model_Zoom_Factor, canvasForImage);
            DrawComponentPoints(bDrawPoints, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Outlines
            DrawOutlines(bDrawOutLine, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Definition Point Numbers
            DrawPointNumbers(bDrawPointNumbers, canvasPointsOut, canvasPointsIn, canvasForImage);

            // Holes
            if (PointsHoles != null)
            {
                DrawHoles(bDrawHoles, bDrawHoleCentreSymbols, canvasPointsHoles, dHolesDiameter, canvasForImage);
                DrawDrillingRoute(bDrawDrillingRoute, canvasPointsDrillingRoute, canvasForImage);
            }

            // Dimensions
            DrawDimensions(bDrawDimensions, canvasDimensions, canvasForImage);
        }

        private static List<CDimension> MirrorYCoordinates(CDimension[] Dimensions)
        {
            if(Dimensions == null) return new List<CDimension>();
            List<CDimension> listDimensions = new List<CDimension>(Dimensions);
            foreach (CDimension d in listDimensions)
            {
                d.MirrorYCoordinates();
            }
            return listDimensions;
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

        public static void CalculateBasicValue(
            double fTempMax_X,
            double fTempMin_X,
            double fTempMax_Y,
            double fTempMin_Y,
            int scale_unit,
            double dPageWidth,
            double dPageHeight,
            List<Point> PointsOut,
            List<Point> PointsIn,
            Point[] PointsHoles,
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

            // Recalculate model coordinates and set minimum point coordinates to [0,0]

            //if (PointsOut != null && PointsOut.Count > 0) // It should exist
            //{
            //    for (int i = 0; i < PointsOut.Count; i++)
            //    {
            //        Point p = PointsOut[i];
            //        p.X -= fTempMin_X;
            //        p.Y -= fTempMin_Y;
            //        PointsOut[i] = p;
            //    }
            //}
            //else
            //{
            //    // Error - Invalid data
            //    MessageBox.Show("Invalid component outline");
            //}

            //if (PointsIn != null && PointsIn.Count > 0)
            //{
            //    for (int i = 0; i < PointsIn.Count; i++)
            //    {
            //        Point p = PointsIn[i];
            //        p.X -= fTempMin_X;
            //        p.Y -= fTempMin_Y;
            //        PointsIn[i] = p;
            //    }
            //}

            //if (PointsHoles != null && PointsHoles.Length > 0)
            //{
            //    for (int i = 0; i < PointsHoles.Length; i++)
            //    {
            //        PointsHoles[i].X -= fTempMin_X;
            //        PointsHoles[i].Y -= fTempMin_Y;
            //    }
            //}

            // Calculate new model dimensions (zoom of model size is 80%)
            dReal_Model_Zoom_Factor = 0.8 / Math.Max(dFactor_x, dFactor_y) * scale_unit;

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

        public static void DrawHoles(bool bDrawHoles, bool bDrawHoleCentreSymbols, List<Point> PointsHoles, double dHolesDiameter, Canvas canvasForImage)
        {
            if (bDrawHoles)
            {
                // Holes
                if (PointsHoles != null) // If is array of holes centers is not empty
                {
                    for (int i = 0; i < PointsHoles.Count; i++)
                    {
                        // Draw Hole
                        DrawCircle(PointsHoles[i], dHolesDiameter, Brushes.Black, 1, canvasForImage);

                        // Draw Symbol of Center
                        if (bDrawHoleCentreSymbols) DrawSymbol_Cross(PointsHoles[i], dHolesDiameter + 10, Brushes.Red, 1, canvasForImage);
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

        public static void DrawPoint(Point point, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas)
        {
            DrawRectangle(strokeColor, fillColor, thickness, imageCanvas, new Point(point.X - 0.5 * thickness, point.Y - 0.5 * thickness), new Point(point.X + 0.5 * thickness, point.Y + 0.5 * thickness));
        }

        public static void DrawLine(Line line, SolidColorBrush color, DashStyle dashStyle, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
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

            myLine.StrokeDashArray =  dashStyle.Dashes;

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
        public static void DrawPolyLine(bool bIsClosed, List<Point> listPoints, double dCanvasTopTemp, double dCanvasLeftTemp, float modelMarginLeft_x, float modelMarginBottom_y, double dReal_Model_Zoom_Factor, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            if (listPoints == null) return;
            if (listPoints.Count < 2) return;

            PointCollection points = new PointCollection();
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
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(myLine, dCanvasTopTemp);
            Canvas.SetLeft(myLine, dCanvasLeftTemp);
            imageCanvas.Children.Add(myLine);
        }
        public static void DrawPolyLine(bool bIsClosed, List<Point> listPoints, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
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
            Canvas.SetTop(myLine, canvasTop);
            Canvas.SetLeft(myLine, canvasLeft);
            imageCanvas.Children.Add(myLine);
        }

        public static void DrawCircle(Point center, double diameter, SolidColorBrush color, double thickness, Canvas imageCanvas)
        {
            if (!Double.IsNaN(center.X)) // Check that value is not "NaN" - TODO - Ondrej Bug No. 109
            {
                Ellipse circle = new Ellipse();
                circle.Height = diameter;
                circle.Width = diameter;
                circle.StrokeThickness = thickness;
                circle.Stroke = color;

                double left = center.X - (diameter / 2);
                double top = center.Y - (diameter / 2);
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

                DrawLine(l, color, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas);

                l.X1 = center.X;
                l.Y1 = center.Y - fSideLength;

                l.X2 = center.X;
                l.Y2 = center.Y + fSideLength;

                DrawLine(l, color, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas);
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
            if (!dim.IsDimensionOutSide) dim.OffsetFromOrigin_pxs *= -1; 
            
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
            double fTextPositionx = lPrimaryLine.X1 + 0.5 * (lPrimaryLine.X2 - lPrimaryLine.X1); // TODO - osetrit znamienka
            double fTextPositiony = lPrimaryLine.Y1 + 0.5 * (lPrimaryLine.Y2 - lPrimaryLine.Y1);

            // Draw dimension line
            DrawLine(lPrimaryLine, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dPrimaryLineThickness, imageCanvas);
            // Draw extension line - start
            DrawLine(lExtensionLine1, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas);
            // Draw extension line - end
            DrawLine(lExtensionLine2, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dExtensionLineThickness, imageCanvas);
            // Draw slope line - start
            DrawLine(lSlopeLine1, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas);
            // Draw slope line - end
            DrawLine(lSlopeLine2, Brushes.DarkGreen, DashStyles.Solid, PenLineCap.Flat, PenLineCap.Flat, dSlopeLineThickness, imageCanvas);
            // Draw text
            DrawText(sText, fTextPositionx, fTextPositiony, dRotation_deg, 12, dim.IsTextAboveLine, Brushes.DarkGreen, imageCanvas);
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
            return;

            float fPositionOfArcFactor = 0.45f;

            double slope = Geom2D.GetAngle_rad(pStart, pEnd, pCenter);
            double radius = fPositionOfArcFactor * (pStart.X);

            Point p2 = new Point(); // 2nd point of arc
            p2.X = Geom2D.GetPositionX_deg((float)radius, (float)slope / MathF.fPI * 180f);  // y
            p2.Y = Geom2D.GetPositionY_CCW_deg((float)radius, (float)slope / MathF.fPI * 180f);  // z

            Size size = new Size(radius, radius);

            ArcSegment arc = new ArcSegment(new Point(pCenter.X + p2.X, pCenter.Y * + p2.Y),
            size,
            slope / MathF.fPI * 180,
            false,
            SweepDirection.Counterclockwise,
            true
            );

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(pStart.X, pStart.Y);
            figure.Segments.Add(arc);

            pathGeometry.Figures.Add(figure);
            Path path = new Path();
            path.Data = pathGeometry;
            //path.Fill = Brushes.Gray;
            path.Stroke = Brushes.Black;
            imageCanvas.Children.Add(path);

            // Lines
            Line l1 = new Line();
            l1.X1 = pCenter.X;
            l1.Y1 = pCenter.Y;
            l1.X2 = pStart.X;
            l1.Y2 = pStart.Y;
            DrawLine(l1, Brushes.Black, DashStyles.Dash, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas);

            Line l2 = new Line();
            l2.X1 = pCenter.X;
            l2.Y1 = pCenter.Y;
            l2.X2 = pEnd.X;
            l2.Y2 = pEnd.Y;
            DrawLine(l2, Brushes.Black, DashStyles.Dash, PenLineCap.Flat, PenLineCap.Flat, 1, imageCanvas);

            // Draw text
            // Draw text in the middle of the arc
            float fFactorTextPosition = 0.5f;
            float fTextPositionx = Geom2D.GetPositionX_deg((float)radius, fFactorTextPosition * (float)slope / MathF.fPI * 180f);  // y
            float fTextPositiony = Geom2D.GetPositionY_CCW_deg((float)radius, fFactorTextPosition * (float)slope / MathF.fPI * 180f);  // z
            string sText = Math.Round(slope / MathF.fPI * 180, 1).ToString() + " °";

            DrawText(sText, pCenter.X + fTextPositionx, pCenter.Y + fTextPositiony, 0, 12, false, Brushes.Black, imageCanvas);
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
        public static  void DrawLine(CMember member, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas canvas, 
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
        public static void DrawPolyLine(float[] arrPointsCoordX, float[] arrPointsCoordY, double dCanvasTop, double dCanvasLeft, float fFactorX, float fFactorY,
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
                    if(rotationAngle_CW_deg < 0) Canvas.SetTop(textBlock, posy + txtSize.Width / 2);
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

        public static void DrawTexts(string[] array_text, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight, 
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, SolidColorBrush color, Canvas canvas)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            for (int i = 0; i < array_text.Length; i++)
            {
                DrawText(array_text[i], modelMarginLeft_x + fFactorX * arrPointsCoordX[i], modelBottomPosition_y - fFactorY * arrPointsCoordY[i],0, 12, false, color, canvas);
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

        // TODO No 44 Ondrej
        // Temporary - TODO Ondrej zjednotit metody pre vykreslovanie v 2D do nejakej zakladnej triedy (mozno uz nejaka aj existuje v inom projekte "SW_EN\GRAPHICS\PAINT" alebo swen_GUI\WindowPaint)
        public static void DrawAxisInCanvas(bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, float fCanvasWidth, float fCanvasHeight, 
            float modelMarginLeft_x, float modelMarginRight_x, float modelMarginTop_y, float modelMarginBottom_y, float modelBottomPosition_y, Canvas canvas)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            float fPositionOfXaxisToTheEndOfYAxis = 0;
            
            if (bYOrientationIsUp) // Up (Forces N, Vx, Vy)
            {
                // x-axis (middle)
                fPositionOfXaxisToTheEndOfYAxis = yValueMax < 0 ? 0 : yValueMax;
                Drawing2D.DrawPolyLine(new float[2] { 0, 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis, modelMarginLeft_x, 
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);

                // y-axis (oriented upwards)
                Drawing2D.DrawPolyLine(new float[2] { 0, 0 }, new float[2] { yValueMin < 0 ? yValueMin : 0, yValueMax < 0 ? 0 : yValueMin + yRangeOfValues }, modelMarginTop_y, modelMarginLeft_x, 
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);
            }
            else // Down (Torsion and bending moments T, Mx, My)
            {
                fPositionOfXaxisToTheEndOfYAxis = yValueMin < 0 ? Math.Abs(yValueMin) : 0;
                // x-axis (middle)
                Drawing2D.DrawPolyLine(new float[2] { 0, 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis, modelMarginLeft_x, 
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);

                // y-axis (oriented downwards)
                Drawing2D.DrawPolyLine(new float[2] { 0, 0 }, new float[2] { yValueMin < 0 ? yValueMin : 0, yValueMax < 0 ? 0 : yValueMin + yRangeOfValues }, modelMarginTop_y, modelMarginLeft_x, 
                    fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvas);
            }
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
                if (!bYOrientationIsUp) // Draw positive values bellow x-axis
                {
                    for (int i = 0; i < arrPointsCoordY.Length; i++)
                        arrPointsCoordY[i] *= -1f;
                }

                Drawing2D.DrawPolyLine(arrPointsCoordX, arrPointsCoordY, modelMarginTop_y, modelMarginLeft_x, fFactorX, fFactorY, modelMarginLeft_x, modelBottomPosition_y, Brushes.Blue, new PenLineCap(), PenLineCap.Triangle, 1, canvas);
            }
        }
    }
}
