using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using MATH;
using CRSC;

namespace BaseClasses
{
    public static class Drawing2D
    {
        // TODO No 44 Ondrej
        // Temporary - TODO Ondrej zjednotit metody pre vykreslovanie v 2D do nejakej zakladnej triedy 
        //(mozno uz nejaka aj existuje v inom projekte "SW_EN\GRAPHICS\PAINT" alebo swen_GUI\WindowPaint)


        public static void DrawCrscToCanvas(CCrSc crsc, double width, double height, ref Canvas canvasForImage)
        {
                float fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
                double dPointInOutDistance_x_real = 0;
                double dPointInOutDistance_y_real = 0;

                // Fill arrays of points
                if (crsc.CrScPointsOut != null && crsc.CrScPointsOut.Length > 1)
                {
                    CalculateModelLimits(crsc.CrScPointsOut, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
                }

                if (crsc.CrScPointsIn != null && crsc.CrScPointsIn.Length > 1)
                {
                    float fTempMax_X_IN = 0, fTempMin_X_IN = 0, fTempMax_Y_IN = 0, fTempMin_Y_IN = 0;

                    CalculateModelLimits(crsc.CrScPointsIn, out fTempMax_X_IN, out fTempMin_X_IN, out fTempMax_Y_IN, out fTempMin_Y_IN);

                    dPointInOutDistance_x_real = fTempMax_X - fTempMax_X_IN;
                    dPointInOutDistance_y_real = fTempMax_Y - fTempMax_Y_IN;
                }

            int scale_unit = 1000; // mm

            float fModel_Length_x_real;
            float fModel_Length_y_real;
            float fModel_Length_x_page;
            float fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            float fReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginBottom_y;
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
                    out fReal_Model_Zoom_Factor,
                    out fmodelMarginLeft_x,
                    out fmodelMarginBottom_y,
                    out dPointInOutDistance_x_page,
                    out dPointInOutDistance_y_page);

            canvasForImage.Children.Clear();
                if (crsc != null)
                    DrawComponent(
                    true, //bDrawPoints,   // NAPOJIT booly na checkboxy v GUI
                    true, // bDrawOutLine,
                    true, // bDrawPointNumbers,
                    false,
                    false,
                    crsc.CrScPointsOut,
                    crsc.CrScPointsIn,
                    null,
                    null,
                    scale_unit,
                    0,
                    fmodelMarginLeft_x,
                    fmodelMarginBottom_y,
                    fReal_Model_Zoom_Factor,
                    fModel_Length_y_page,
                    dPointInOutDistance_y_page,
                    dPointInOutDistance_x_page,
                    canvasForImage);
        }

        public static void DrawPlateToCanvas(CPlate plate, double width, double height, ref Canvas canvasForImage)
        {
            float fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;

            // Fill arrays of points
            if (plate.PointsOut2D != null && plate.PointsOut2D.Length > 1)
            {
                CalculateModelLimits(plate.PointsOut2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            }

            if (plate.DrillingRoutePoints != null)
            {
                float[,] PointsDrillingRoute = new float[plate.DrillingRoutePoints.Count, 2];

                // Fill array of drilling points route
                for (int i = 0; i < plate.DrillingRoutePoints.Count; i++)
                {
                    PointsDrillingRoute[i, 0] = (float)plate.DrillingRoutePoints[i].X;
                    PointsDrillingRoute[i, 1] = (float)plate.DrillingRoutePoints[i].Y;
                }
            }

            int scale_unit = 1000; // mm

            float fModel_Length_x_real;
            float fModel_Length_y_real;
            float fModel_Length_x_page;
            float fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            float fReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginBottom_y;
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
                    plate.PointsOut2D,
                    null,
                    plate.HolesCentersPoints2D,
                    0,
                    0,
                    out fModel_Length_x_real,
                    out fModel_Length_y_real,
                    out fModel_Length_x_page,
                    out fModel_Length_y_page,
                    out dFactor_x,
                    out dFactor_y,
                    out fReal_Model_Zoom_Factor,
                    out fmodelMarginLeft_x,
                    out fmodelMarginBottom_y,
                    out dPointInOutDistance_x_page,
                    out dPointInOutDistance_y_page);

            canvasForImage.Children.Clear();
            if (plate != null)
                DrawComponent(
                    true,
                    true,
                    true,
                    true,
                    true,
                    plate.PointsOut2D,
                    null,
                    plate.HolesCentersPoints2D,
                    plate.DrillingRoutePoints2D, // TODO Ondrej - Prepracovat vsetky polia bodov na objekt Point alebo opacne, toto pole je potrebne naplnit z poloziek zoznamu List<Point> DrillingRoutePoints
                    scale_unit,
                    plate.FHoleDiameter,
                    fmodelMarginLeft_x,
                    fmodelMarginBottom_y,
                    fReal_Model_Zoom_Factor,
                    fModel_Length_y_page,
                    dPointInOutDistance_y_page,
                    dPointInOutDistance_x_page,
                    canvasForImage);
        }

        public static void DrawComponent(bool bDrawPoints,
            bool bDrawOutLine,
            bool bDrawPointNumbers,
            bool bDrawHoles,
            bool bDrawDrillingRoute,
            float[,] PointsOut,
            float[,] PointsIn,
            float[,] PointsHoles,
            float[,] PointsDrillingRoute,
            float scale_unit,
            double DHolesDiameter,
            float fmodelMarginLeft_x,
            float fmodelMarginBottom_y,
            float fReal_Model_Zoom_Factor,
            float fModel_Length_y_page,
            double dPointInOutDistance_y_page,
            double dPointInOutDistance_x_page,
            Canvas canvasForImage)
        {
            // Definition Points
            DrawPoints(bDrawPoints, PointsOut, PointsIn, fmodelMarginLeft_x, fmodelMarginBottom_y, fReal_Model_Zoom_Factor, canvasForImage);

            // Outlines
            DrawOutlines(bDrawOutLine, true, PointsOut, PointsIn, fReal_Model_Zoom_Factor, fmodelMarginBottom_y, fmodelMarginLeft_x, fModel_Length_y_page, dPointInOutDistance_y_page, dPointInOutDistance_x_page, canvasForImage);

            // Definition Point Numbers
            DrawPointNumbers(bDrawPointNumbers, PointsOut, PointsIn, fmodelMarginLeft_x, fmodelMarginBottom_y, fReal_Model_Zoom_Factor, canvasForImage);

            // Holes
            if (PointsHoles != null)
            {
                DrawHoles(bDrawHoles, PointsHoles, fmodelMarginLeft_x, fmodelMarginBottom_y, fReal_Model_Zoom_Factor, scale_unit, DHolesDiameter, canvasForImage);

                DrawDrillingRoute(bDrawDrillingRoute, PointsDrillingRoute, fReal_Model_Zoom_Factor, fmodelMarginLeft_x, fmodelMarginBottom_y, canvasForImage);
            }
        }
        
        public static void CalculateBasicValue(
            float fTempMax_X,
            float fTempMin_X,
            float fTempMax_Y,
            float fTempMin_Y,
            int scale_unit,
            double dPageWidth,
            double dPageHeight,
            float[,] PointsOut,
            float[,] PointsIn,
            float[,] PointsHoles,
            double dPointInOutDistance_x_real,
            double dPointInOutDistance_y_real,
            out float fModel_Length_x_real,
            out float fModel_Length_y_real,
            out float fModel_Length_x_page,
            out float fModel_Length_y_page,
            out double dFactor_x,
            out double dFactor_y,
            out float fReal_Model_Zoom_Factor,
            out float fmodelMarginLeft_x,
            out float fmodelMarginBottom_y,
            out double dPointInOutDistance_x_page,
            out double dPointInOutDistance_y_page
            )
        {
            fModel_Length_x_real = fTempMax_X - fTempMin_X;
            fModel_Length_y_real = fTempMax_Y - fTempMin_Y;

            fModel_Length_x_page = scale_unit * fModel_Length_x_real;
            fModel_Length_y_page = scale_unit * fModel_Length_y_real;

            // Calculate maximum zoom factor
            // Original ratio
            dFactor_x = fModel_Length_x_page / dPageWidth;
            dFactor_y = fModel_Length_y_page / dPageHeight;

            // Recalculate model coordinates and set minimum point coordinates to [0,0]

            if (PointsOut != null && PointsOut.Length / 2 > 0) // It should exist
            {
                for (int i = 0; i < PointsOut.Length / 2; i++)
                {
                    PointsOut[i, 0] -= fTempMin_X;
                    PointsOut[i, 1] -= fTempMin_Y;
                }
            }
            else
            {
                // Error - Invalid data
                MessageBox.Show("Invalid component outline");
            }

            if (PointsIn != null && PointsIn.Length /2 > 0)
            {
                for (int i = 0; i < PointsIn.Length / 2; i++)
                {
                    PointsIn[i, 0] -= fTempMin_X;
                    PointsIn[i, 1] -= fTempMin_Y;
                }
            }

            if (PointsHoles != null && PointsHoles.Length / 2 > 0)
            {
                for (int i = 0; i < PointsHoles.Length / 2; i++)
                {
                    PointsHoles[i, 0] -= fTempMin_X;
                    PointsHoles[i, 1] -= fTempMin_Y;
                }
            }

            // Calculate new model dimensions (zoom of model size is 90%)
            fReal_Model_Zoom_Factor = 0.9f / (float)MathF.Max(dFactor_x, dFactor_y) * scale_unit;

            // Set new size of model on the page
            fModel_Length_x_page = fReal_Model_Zoom_Factor * fModel_Length_x_real;
            fModel_Length_y_page = fReal_Model_Zoom_Factor * fModel_Length_y_real;

            fmodelMarginLeft_x = (float)(0.5 * (dPageWidth - fModel_Length_x_page));

            fmodelMarginBottom_y = (float)(fModel_Length_y_page + 0.5 * (dPageHeight - fModel_Length_y_page));

            dPointInOutDistance_x_page = 0;
            dPointInOutDistance_y_page = 0;

            if (PointsIn != null && PointsIn.Length / 2 > 0)
            {
                dPointInOutDistance_x_page = dPointInOutDistance_x_real * fReal_Model_Zoom_Factor;
                dPointInOutDistance_y_page = dPointInOutDistance_y_real * fReal_Model_Zoom_Factor;
            }
        }

        public static void DrawPoints(bool bDrawPoints, float[,] PointsOut, float [,] PointsIn, float modelMarginLeft_x, float modelMarginBottom_y, float fReal_Model_Zoom_Factor, Canvas canvasForImage)
        {
            if (bDrawPoints)
            {
                // Outer outline points
                if (PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsOut.Length / 2; i++)
                    {
                        DrawPoint(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i, 1]), Brushes.Red, Brushes.Red, 4, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsIn.Length / 2; i++)
                    {
                        DrawPoint(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i, 1]), Brushes.Red, Brushes.Red, 4, canvasForImage);
                    }
                }
            }
        }

        public static void DrawOutlines(bool bDrawOutLine, bool bUsePolylineforDrawing, float [,] PointsOut, float[,] PointsIn, float fReal_Model_Zoom_Factor, float modelMarginBottom_y, float modelMarginLeft_x, float fModel_Length_y_page, double dPointInOutDistance_y_page, double dPointInOutDistance_x_page, Canvas canvasForImage)
        {
            if (bDrawOutLine)
            {
                if (bUsePolylineforDrawing)
                {
                    // Outer outline lines
                    if (PointsOut != null) // If is array of points not empty
                    {
                        double fCanvasTop = modelMarginBottom_y - fModel_Length_y_page;
                        double fCanvasLeft = modelMarginLeft_x;
                        DrawPolyLine(true, PointsOut, fCanvasTop, fCanvasLeft, modelMarginLeft_x, modelMarginBottom_y, fReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
                    }

                    // Internal outline lines
                    if (PointsIn != null) // If is array of points not empty
                    {
                        double fCanvasTop = modelMarginBottom_y - fModel_Length_y_page + dPointInOutDistance_y_page;
                        double fCanvasLeft = modelMarginLeft_x + dPointInOutDistance_x_page;
                        DrawPolyLine(true, PointsIn, fCanvasTop, fCanvasLeft, modelMarginLeft_x, modelMarginBottom_y, fReal_Model_Zoom_Factor, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
                    }
                }
                else
                {
                    // ToDo kreslenie po liniach nefunguje spravne, asi je problem s tymito canvas vo funkcii DrawLine
                    // Canvas.SetTop(myLine, line.Y1);
                    // Canvas.SetLeft(myLine, line.X1);

                    // Outer outline lines
                    if (PointsOut != null) // If is array of points not empty
                    {
                        for (int i = 0; i < PointsOut.Length / 2; i++)
                        {
                            // Add a Line
                            Line l = new Line();

                            l.X1 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i, 0];
                            l.Y1 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i, 1];

                            if (i < (PointsOut.Length / 2 - 1))
                            {
                                l.X2 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i + 1, 0];
                                l.Y2 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i + 1, 1];
                            }
                            else
                            {
                                l.X2 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[0, 0];
                                l.Y2 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[0, 1];
                            }

                            DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
                        }
                    }

                    // Internal outline lines
                    if (PointsIn != null) // If is array of points not empty
                    {
                        for (int i = 0; i < PointsIn.Length / 2; i++)
                        {
                            // Add a Line
                            Line l = new Line();
                            l.X1 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i, 0];
                            l.Y1 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i, 1];

                            if (i < (PointsIn.Length / 2 - 1))
                            {
                                l.X2 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i + 1, 0];
                                l.Y2 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i + 1, 1];
                            }
                            else
                            {
                                l.X2 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[0, 0];
                                l.Y2 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[0, 1];
                            }

                            DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
                        }
                    }
                }
            }
        }

        public static void DrawPointNumbers(bool bDrawPointNumbers, float [,] PointsOut, float [,] PointsIn, float modelMarginLeft_x, float modelMarginBottom_y, float fReal_Model_Zoom_Factor, Canvas canvasForImage)
        {
            if (bDrawPointNumbers)
            {
                // Outer outline points
                if (PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsOut.Length /2; i++)
                    {
                        DrawText((i + 1).ToString(), modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i, 1], 16, Brushes.Blue, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null && PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < PointsIn.Length / 2; i++)
                    {
                        DrawText((/*crsc.INoPointsOut +*/ i + 1).ToString(), modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i, 1], 16, Brushes.Green, canvasForImage);
                    }
                }
            }
        }

        public static void DrawHoles(bool bDrawHoles, float [,] PointsHoles, float modelMarginLeft_x, float modelMarginBottom_y, float fReal_Model_Zoom_Factor, float scale_unit, double DHolesDiameter, Canvas canvasForImage)
        {
            if (bDrawHoles)
            {
                // Holes
                if (PointsHoles != null) // If is array of holes centers is not empty
                {
                    for (int i = 0; i < PointsHoles.Length / 2; i++)
                    {
                        // Draw Hole
                        DrawCircle(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsHoles[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsHoles[i, 1]), scale_unit * DHolesDiameter, Brushes.Black, 2, canvasForImage);
                        // Draw Symbol of Center
                        DrawSymbol_Cross(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsHoles[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsHoles[i, 1]), scale_unit * DHolesDiameter + 10, Brushes.Red, 2, canvasForImage);
                    }
                }
            }
        }

        public static void DrawDrillingRoute(bool bDrawDrillingRoute, float [,] PointsDrillingRoute, float fReal_Model_Zoom_Factor, float modelMarginLeft_x, float modelMarginBottom_y, Canvas canvasForImage)
        {
            if (!bDrawDrillingRoute || PointsDrillingRoute == null) return;
            // ??? TODO upravit odsadenie

            double fx_min = double.MaxValue;
            double fy_min = double.MaxValue;
            double fx_max = double.MinValue;
            double fy_max = double.MinValue;

            for (int i = 0; i < PointsDrillingRoute.Length / 2; i++)
            {
                if (PointsDrillingRoute[i, 0] < fx_min)
                    fx_min = PointsDrillingRoute[i, 0];

                if (PointsDrillingRoute[i, 1] < fy_min)
                    fy_min = PointsDrillingRoute[i, 1];

                if (PointsDrillingRoute[i, 0] > fx_max)
                    fx_max = PointsDrillingRoute[i, 0];

                if (PointsDrillingRoute[i, 1] > fy_max)
                    fy_max = PointsDrillingRoute[i, 1];
            }

            fx_min *= fReal_Model_Zoom_Factor;
            fy_min *= fReal_Model_Zoom_Factor;
            fx_max *= fReal_Model_Zoom_Factor;
            fy_max *= fReal_Model_Zoom_Factor;

            double fCanvasTop = modelMarginBottom_y - fy_max;
            double fCanvasLeft = modelMarginLeft_x + fx_min;

            if (PointsDrillingRoute != null)
                DrawPolyLine(false, PointsDrillingRoute, fCanvasTop, fCanvasLeft, modelMarginLeft_x, modelMarginBottom_y, fReal_Model_Zoom_Factor, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
        }

        public static void DrawPoint(Point point, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas)
        {
            DrawRectangle(strokeColor, fillColor, thickness, imageCanvas, new Point(point.X - 0.5 * thickness, point.Y - 0.5 * thickness), new Point(point.X + 0.5 * thickness, point.Y + 0.5 * thickness));
        }

        public static void DrawLine(Line line, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            Random r = new Random();
            Color randomcolor = Color.FromArgb((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
            SolidColorBrush b = new SolidColorBrush(randomcolor);

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
            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(myLine, line.Y1);
            Canvas.SetLeft(myLine, line.X1);
            imageCanvas.Children.Add(myLine);
        }

        public static void DrawPolyLine(bool bIsClosed, float[,] arrPoints, double dCanvasTopTemp, double dCanvasLeftTemp, float modelMarginLeft_x, float modelMarginBottom_y, float fReal_Model_Zoom_Factor, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            int iNumberOfLineSegments = arrPoints.Length / 2 + (bIsClosed ? 1 : 0);

            for (int i = 0; i < iNumberOfLineSegments; i++)
            {
                if (i < ((arrPoints.Length / 2)))
                    points.Add(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * arrPoints[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * arrPoints[i, 1]));
                else
                    points.Add(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * arrPoints[0, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * arrPoints[0, 1])); // Last point is same as first one
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

        public static void DrawCircle(Point center, double diameter, SolidColorBrush color, double thickness, Canvas imageCanvas)
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

        public static void DrawSymbol_Cross(Point center, double size, SolidColorBrush color, double thickness, Canvas imageCanvas)
        {
            Line l = new Line();

            double fSideLength = 0.5f * size;

            // Horizontal
            l.X1 = center.X - fSideLength;
            l.Y1 = center.Y;

            l.X2 = center.X + fSideLength;
            l.Y2 = center.Y;

            DrawLine(l, color, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas);

            l.X1 = center.X;
            l.Y1 = center.Y - fSideLength;

            l.X2 = center.X;
            l.Y2 = center.Y + fSideLength;

            DrawLine(l, color, PenLineCap.Flat, PenLineCap.Flat, thickness, imageCanvas);
        }

        public static void CalculateModelLimits(float[,] Points_temp, out float fTempMax_X, out float fTempMin_X, out float fTempMax_Y, out float fTempMin_Y)
        {
            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;

            if (Points_temp != null) // Some points exist
            {
                for (int i = 0; i < Points_temp.Length / 2; i++)
                {
                    // Maximum X - coordinate
                    if (Points_temp[i, 0] > fTempMax_X)
                        fTempMax_X = Points_temp[i, 0];

                    // Minimum X - coordinate
                    if (Points_temp[i, 0] < fTempMin_X)
                        fTempMin_X = Points_temp[i, 0];

                    // Maximum Y - coordinate
                    if (Points_temp[i, 1] > fTempMax_Y)
                        fTempMax_Y = Points_temp[i, 1];

                    // Minimum Y - coordinate
                    if (Points_temp[i, 1] < fTempMin_Y)
                        fTempMin_Y = Points_temp[i, 1];
                }
            }
        }

        public static float [,] TransformPointToArrayCoord(List<Point> Points_input)
        {
            if (Points_input == null)
                throw new ArgumentNullException("Not inicialized list of points!");

            float [,] array = new float[Points_input.Count, 2];

            for(int i = 0; i < Points_input.Count; i++)
            {
                array[i, 0] = (float)Points_input[i].X;
                array[i, 1] = (float)Points_input[i].Y;
            }

            return array;
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
        public static void DrawText(string text, double posx, double posy, double fontSize, SolidColorBrush color, Canvas canvas)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            Canvas.SetLeft(textBlock, posx);
            Canvas.SetTop(textBlock, posy);
            textBlock.Margin = new Thickness(2, 2, 0, 0);
            textBlock.FontSize = fontSize;
            canvas.Children.Add(textBlock);
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
                DrawText(array_text[i], modelMarginLeft_x + fFactorX * arrPointsCoordX[i], modelBottomPosition_y - fFactorY * arrPointsCoordY[i], 12, color, canvas);
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
