using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using BaseClasses;
using MATH;
using CRSC;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowCrossSection2D.xaml
    /// </summary>
    public partial class WindowCrossSection2D : Window
    {
        public Canvas CanvasSection2D = null;
        int scale_unit = 1000; // mm

        double modelMarginLeft_x;
        double modelMarginBottom_y;
        double fReal_Model_Zoom_Factor;

        double fModel_Length_x_page;
        double fModel_Length_y_page;

        double dPageWidth;
        double dPageHeight;

        float fTempMax_X;
        float fTempMin_X;
        float fTempMax_Y;
        float fTempMin_Y;

        bool bDrawPoints = true;
        bool bDrawOutLine = true;
        bool bUsePolylineforDrawing = true;
        bool bDrawPointNumbers = true;
        bool bDrawHoles = true;

        float[,] PointsOut;
        float[,] PointsIn;
        float[,] PointsHoles;
        float[,] PointsDrillingRoute;

        double dPointInOutDistance_x_real;
        double dPointInOutDistance_y_real;

        double dPointInOutDistance_x_page;
        double dPointInOutDistance_y_page;

        int INoPointsOut;
        int INoPointsIn;
        int INoHoles;

        double DHolesDiameter;

        public WindowCrossSection2D()
        {
            InitializeComponent();
            CanvasSection2D = canvasForImage;
        }

        public WindowCrossSection2D(CPlate plate, double dPageWidth_temp, double dPageHeight_temp)
        {
            dPageWidth = dPageWidth_temp;
            dPageHeight = dPageHeight_temp;

            InitializeComponent();

            // Fill arrays of points
            if (plate.PointsOut2D != null && plate.PointsOut2D.Length > 1)
            {
                INoPointsOut = plate.ITotNoPointsin2D;
                PointsOut = plate.PointsOut2D;

                CalculateModelLimits(plate.PointsOut2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            }

            PointsIn = null; // Todo - dopracovat (pole viacerych poli bodov pre definovanie otvorov), teoreticky moze mat plech okrem dier pre skrutky aj vacsie vyrezy a rozne otvory

            PointsHoles = plate.HolesCentersPoints2D;

            if (plate.HolesCentersPoints2D != null)
            {
                INoHoles = plate.screwArrangement.IHolesNumber;
                PointsHoles = plate.HolesCentersPoints2D;
                DHolesDiameter = plate.screwArrangement.referenceScrew.Diameter_thread;
            }

            if(plate.DrillingRoutePoints != null)
            {
                PointsDrillingRoute = new float[plate.DrillingRoutePoints.Count, 2];

                // Fill array of drilling points route
                for (int i = 0; i < plate.DrillingRoutePoints.Count; i++)
                {
                    PointsDrillingRoute[i, 0] = (float)plate.DrillingRoutePoints[i].X;
                    PointsDrillingRoute[i, 1] = (float)plate.DrillingRoutePoints[i].Y;
                }
            }

            CaclulateBasicValue();

            canvasForImage.Children.Clear();
            if (plate != null)
                DrawComponent();

            CanvasSection2D = canvasForImage;
        }

        public WindowCrossSection2D(CCrSc crsc, double dPageWidth_temp, double dPageHeight_temp)
        {
            InitializeComponent();

            dPageWidth = dPageWidth_temp;
            dPageHeight = dPageHeight_temp;

            // Fill arrays of points
            if (crsc.CrScPointsOut != null && crsc.CrScPointsOut.Length > 1)
            {
                INoPointsOut = crsc.INoPointsOut;
                PointsOut = crsc.CrScPointsOut;

                CalculateModelLimits(crsc.CrScPointsOut, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);

            }

            PointsHoles = null;

            if (crsc.CrScPointsIn != null && crsc.CrScPointsIn.Length > 1)
            {
                INoPointsIn = crsc.INoPointsIn;
                PointsIn = crsc.CrScPointsIn;

                float fTempMax_X_IN, fTempMin_X_IN, fTempMax_Y_IN, fTempMin_Y_IN;

                CalculateModelLimits(crsc.CrScPointsIn, out fTempMax_X_IN, out fTempMin_X_IN, out fTempMax_Y_IN, out fTempMin_Y_IN);

                dPointInOutDistance_x_real = fTempMax_X - fTempMax_X_IN;
                dPointInOutDistance_y_real = fTempMax_Y - fTempMax_Y_IN;
            }


            CaclulateBasicValue();

            canvasForImage.Children.Clear();
            if (crsc != null)
                DrawComponent();

            CanvasSection2D = canvasForImage;
        }

        public void SaveImage(Visual visual, int width, int height, string filePath)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, 96, 96,
                                                         PixelFormats.Pbgra32);
            bitmap.Render(visual);

            PngBitmapEncoder image = new PngBitmapEncoder();
            image.Frames.Add(BitmapFrame.Create(bitmap));
            using (Stream fs = File.Create(filePath))
            {
                image.Save(fs);
            }
        }

        private void menuItemTest1_Click(object sender, RoutedEventArgs e)
        {
            canvasForImage.Children.Clear();

            CCrSc_3_10075_BOX crsc = new CCrSc_3_10075_BOX(0.1f, 0.1f, 0.0075f, Colors.LawnGreen);
            dPageWidth = this.Width;
            dPageHeight = this.Height;

            // Fill arrays of points
            PointsOut = crsc.CrScPointsOut;
            PointsIn = crsc.CrScPointsIn;
            PointsHoles = null;

            if (crsc.CrScPointsIn != null && crsc.CrScPointsIn.Length > 1)
            {
                INoPointsIn = crsc.CrScPointsIn.Length / 2;
                PointsIn = new float[INoPointsIn, 2];   //zbytocny riadok
                PointsIn = crsc.CrScPointsIn;
            }

            CalculateModelLimits(PointsOut = crsc.CrScPointsOut, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);

            CaclulateBasicValue();

            canvasForImage.Children.Clear();
            if (crsc != null)
                DrawComponent();
        }

        public void DrawComponent()
        {
            // Definition Points
            DrawPoints();

            // Outlines
            DrawOutlines();

            // Definition Point Numbers
            DrawPointNumbers();

            // Holes
            if (PointsHoles != null)
            {
                DrawHoles();

                DrawDrillingRoute();
            }
        }

        public void DrawPoints()
        {   
            if (bDrawPoints)
            {
                // Outer outline points
                if (PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < INoPointsOut; i++)
                    {
                        DrawPoint(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i, 1]), Brushes.Red, Brushes.Red, 4, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null) // If is array of points not empty
                {
                    for (int i = 0; i < INoPointsIn; i++)
                    {
                        DrawPoint(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i, 1]), Brushes.Red, Brushes.Red, 4, canvasForImage);
                    }
                }
            }
        }

        public void DrawOutlines()
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
                        DrawPolyLine(true, PointsOut, fCanvasTop, fCanvasLeft, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
                    }

                    // Internal outline lines
                    if (PointsIn != null) // If is array of points not empty
                    {
                        double fCanvasTop = modelMarginBottom_y - fModel_Length_y_page + dPointInOutDistance_y_page;
                        double fCanvasLeft = modelMarginLeft_x + dPointInOutDistance_x_page;
                        DrawPolyLine(true, PointsIn, fCanvasTop, fCanvasLeft, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
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
                        for (int i = 0; i < INoPointsOut; i++)
                        {
                            // Add a Line
                            Line l = new Line();

                            l.X1 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i, 0];
                            l.Y1 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i, 1];

                            if (i < (INoPointsOut - 1))
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
                        for (int i = 0; i < INoPointsIn; i++)
                        {
                            // Add a Line
                            Line l = new Line();
                            l.X1 = modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i, 0];
                            l.Y1 = modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i, 1];

                            if (i < (INoPointsIn - 1))
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

        public void DrawPointNumbers()
        {
            if (bDrawPointNumbers)
            {
                // Outer outline points
                if (PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < INoPointsOut; i++)
                    {
                        DrawText((i + 1).ToString(), modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsOut[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsOut[i, 1], 16, Brushes.Blue, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null && PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < INoPointsIn; i++)
                    {
                        DrawText((/*crsc.INoPointsOut +*/ i + 1).ToString(), modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsIn[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsIn[i, 1], 16, Brushes.Green, canvasForImage);
                    }
                }
            }
        }

        public void DrawHoles()
        {
            if (bDrawHoles)
            {
                // Holes
                if (PointsHoles != null) // If is array of holes centers is not empty
                {
                    for (int i = 0; i < INoHoles; i++)
                    {
                        // Draw Hole
                        DrawCircle(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsHoles[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsHoles[i, 1]), scale_unit * DHolesDiameter, Brushes.Black, 2, canvasForImage);
                        // Draw Symbol of Center
                        DrawSymbol_Cross(new Point(modelMarginLeft_x + fReal_Model_Zoom_Factor * PointsHoles[i, 0], modelMarginBottom_y - fReal_Model_Zoom_Factor * PointsHoles[i, 1]), scale_unit * DHolesDiameter + 10, Brushes.Red, 2, canvasForImage);
                    }
                }
            }
        }

        public void DrawDrillingRoute()
        {
            if (PointsDrillingRoute == null) return;
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
                DrawPolyLine(false, PointsDrillingRoute, fCanvasTop, fCanvasLeft, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
        }

        public void DrawPoint(Point point, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas)
        {
            DrawRectangle(strokeColor, fillColor, thickness, imageCanvas, new Point(point.X - 0.5 * thickness, point.Y - 0.5 * thickness), new Point(point.X + 0.5 * thickness, point.Y + 0.5 * thickness));
        }

        public void DrawLine(Line line, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
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

        public void DrawPolyLine(bool bIsClosed, float [,] arrPoints,double dCanvasTopTemp, double dCanvasLeftTemp, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            int iNumberOfLineSegments = arrPoints.Length / 2 + (bIsClosed? 1:0);

            for (int i = 0; i < iNumberOfLineSegments; i++)
            {
                if(i < ((arrPoints.Length / 2)))
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

        public void DrawText(string text, double posx, double posy, double fontSize, SolidColorBrush color, Canvas imageCanvas)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            Canvas.SetLeft(textBlock, posx);
            Canvas.SetTop(textBlock, posy);
            textBlock.Margin = new Thickness(2, 2, 0, 0);
            textBlock.FontSize = fontSize;
            imageCanvas.Children.Add(textBlock);
        }

        public void DrawCircle(Point center, double diameter, SolidColorBrush color, double thickness, Canvas imageCanvas)
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

        public void DrawSymbol_Cross(Point center, double size, SolidColorBrush color, double thickness, Canvas imageCanvas)
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
        /// <summary>
        /// Draw methods for each Draw Element Type
        /// </summary>
        public void DrawRectangle(SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas, Point lt, Point br)
        {
            Rectangle rect = new Rectangle();
            rect.Stretch = Stretch.Fill;
            rect.Fill = fillColor;
            rect.Stroke = strokeColor;
            rect.Width = br.X - lt.X;
            rect.Height = br.Y - lt.Y;
            Canvas.SetTop(rect, lt.Y);
            Canvas.SetLeft(rect, lt.X);
            imageCanvas.Children.Add(rect);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                zoomIn2D();
                Console.WriteLine("zoomIn");
            }
            else if (e.Key == Key.OemMinus)
            {
                zoomOut2D();
                Console.WriteLine("zoomOut");
            }
            //else MessageBox.Show("else");
        }

        private void zoomIn2D()
        {
            Line l = (Line)canvasForImage.Children[0];
            l.X2 *= 2;
            l.Y2 *= 2;
        }

        private void zoomOut2D()
        {
            Line l = (Line)canvasForImage.Children[0];
            l.X2 /= 2;
            l.Y2 /= 2;
        }

        public void CalculateModelLimits(float[,] Points_temp, out float fTempMax_X, out float fTempMin_X, out float fTempMax_Y, out float fTempMin_Y)
        {
            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;

            if (PointsOut != null) // Some points exist
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

        public void CaclulateBasicValue()
        {
            float fModel_Length_x_real = fTempMax_X - fTempMin_X;
            float fModel_Length_y_real = fTempMax_Y - fTempMin_Y;

            fModel_Length_x_page = scale_unit * fModel_Length_x_real;
            fModel_Length_y_page = scale_unit * fModel_Length_y_real;

            // Calculate maximum zoom factor
            // Original ratio
            double dFactor_x = fModel_Length_x_page / dPageWidth;
            double dFactor_y = fModel_Length_y_page / dPageHeight;

            // Recalculate model coordinates and set minimum point coordinates to [0,0]

            if (PointsOut != null && INoPointsOut > 0) // It should exist
            {
                for (int i = 0; i < INoPointsOut; i++)
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

            if (PointsIn != null && INoPointsIn > 0)
            {
                for (int i = 0; i < INoPointsIn; i++)
                {
                    PointsIn[i, 0] -= fTempMin_X;
                    PointsIn[i, 1] -= fTempMin_Y;
                }
            }

            if (PointsHoles != null && INoHoles > 0)
            {
                for (int i = 0; i < INoHoles; i++)
                {
                    PointsHoles[i, 0] -= fTempMin_X;
                    PointsHoles[i, 1] -= fTempMin_Y;
                }
            }

            // Calculate new model dimensions (zoom of model size is 90%)
            fReal_Model_Zoom_Factor = 0.9f / MathF.Max(dFactor_x, dFactor_y) * scale_unit;

            // Set new size of model on the page
            fModel_Length_x_page = fReal_Model_Zoom_Factor * fModel_Length_x_real;
            fModel_Length_y_page = fReal_Model_Zoom_Factor * fModel_Length_y_real;

            modelMarginLeft_x = 0.5 * (dPageWidth - fModel_Length_x_page);

            modelMarginBottom_y = fModel_Length_y_page + 0.5 * (dPageHeight - fModel_Length_y_page);

            if(PointsIn != null && INoPointsIn > 0)
            {
                dPointInOutDistance_x_page = dPointInOutDistance_x_real * fReal_Model_Zoom_Factor;
                dPointInOutDistance_y_page = dPointInOutDistance_y_real * fReal_Model_Zoom_Factor;
            }
        }
    }
}
