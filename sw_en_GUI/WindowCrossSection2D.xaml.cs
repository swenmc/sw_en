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
using CRSC;
using BaseClasses.GraphObj;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowCrossSection2D.xaml
    /// </summary>
    public partial class WindowCrossSection2D : Window
    {
        int scale_unit = 1000; // mm
        int modelposition_x = 200;
        int modelposition_y = 150;

        bool bDrawPoints = true;
        bool bDrawOutLine = true;
        bool bUsePolylineforDrawing = true;
        bool bDrawPointNumbers = true;
        bool bDrawHoles = true;

        float[,] PointsOut;
        float[,] PointsIn;
        float[,] HolesCoord;

        int INoPointsOut;
        int INoPointsIn;
        int INoHoles;

        double DHolesDiameter;

        public WindowCrossSection2D()
        {
            InitializeComponent();
            // Temporary
            //Point p = new Point(10, 10);
            //DrawPoint(p, Brushes.Red, Brushes.Red, 4, canvasForImage);
        }

        public WindowCrossSection2D(CPlate component)
        {
            InitializeComponent();
            canvasForImage.Children.Clear();
            if (component != null)
                DrawPlate(component);
        }

        public WindowCrossSection2D(CCrSc_TW crsc)
        {
            InitializeComponent();
            canvasForImage.Children.Clear();
            if (crsc != null)
                DrawCrSc(crsc);
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
            CCrSc_3_10075_BOX crsc_temp = new CCrSc_3_10075_BOX(0.1f, 0.1f, 0.0075f, Colors.LawnGreen);
            //CCrSc_3_51_TRIANGLE_TEMP crsc_temp = new CCrSc_3_51_TRIANGLE_TEMP(0.866025f * 0.5f, 0.5f, 0.01f);
            DrawCrSc(crsc_temp);

            //CConCom_Plate_BB_BG component_temp = new CConCom_Plate_BB_BG(new CPoint(0, 0, 0, 0, 0), 0.072f, 0.29f, 0.18f, 0.003f, 2, 0.012f, true);
            //DrawPlate(component_temp);
        }

        public void DrawPlate(CPlate component)
        {
            INoPointsOut = component.PointsOut2D.Length / 2;
            INoPointsIn = 0;


            PointsOut = new float [INoPointsOut, 2];
            PointsIn = null;

            PointsOut = component.PointsOut2D;
            PointsIn = null;

            if (component.HolesCentersPoints2D != null)
            {
                INoHoles = component.HolesCentersPoints2D.Length / 2;
                HolesCoord = new float[INoHoles, 2];
                HolesCoord = component.HolesCentersPoints2D;
                DHolesDiameter = component.FHoleDiameter;
            }


            // Definition Points
            DrawPoints();

            // Outlines
            DrawOutlines();

            // Definition Point Numbers
            DrawPointNumbers();

            // Holes
            if (component.HolesCentersPoints2D != null)
            {
                DrawHoles();
            }
        }

        public void DrawCrSc(CCrSc_TW crsc)
        {
            if (crsc.CrScPointsOut != null) // Draw just in case that crsc is valid
            {
                INoPointsOut = crsc.CrScPointsOut.Length / 2;
                PointsOut = new float[INoPointsOut, 2];
                PointsOut = crsc.CrScPointsOut;

                if (crsc.CrScPointsIn != null && crsc.CrScPointsIn.Length > 1)
                {
                    INoPointsIn = crsc.CrScPointsIn.Length / 2;
                    PointsIn = new float[INoPointsIn, 2];
                    PointsIn = crsc.CrScPointsIn;
                }

                // Definition Points
                DrawPoints();

                // Outlines
                DrawOutlines();

                // Definition Point Numbers
                DrawPointNumbers();
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
                        DrawPoint(new Point(modelposition_x + scale_unit * PointsOut[i, 0], modelposition_y + scale_unit * PointsOut[i, 1]), Brushes.Red, Brushes.Red, 4, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null) // If is array of points not empty
                {
                    for (int i = 0; i < INoPointsIn; i++)
                    {
                        DrawPoint(new Point(modelposition_x + scale_unit * PointsIn[i, 0], modelposition_y + scale_unit * PointsIn[i, 1]), Brushes.Red, Brushes.Red, 4, canvasForImage);
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
                        DrawPolyLine(PointsOut, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
                    }

                    // Internal outline lines
                    if (PointsIn != null) // If is array of points not empty
                    {
                        DrawPolyLine(PointsIn, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
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

                            l.X1 = modelposition_x + scale_unit * PointsOut[i, 0];
                            l.Y1 = modelposition_y + scale_unit * PointsOut[i, 1];

                            if (i < (INoPointsOut - 1))
                            {
                                l.X2 = modelposition_x + scale_unit * PointsOut[i + 1, 0];
                                l.Y2 = modelposition_y + scale_unit * PointsOut[i + 1, 1];
                            }
                            else
                            {
                                l.X2 = modelposition_x + scale_unit * PointsOut[0, 0];
                                l.Y2 = modelposition_y + scale_unit * PointsOut[0, 1];
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
                            l.X1 = modelposition_x + scale_unit * PointsIn[i, 0];
                            l.Y1 = modelposition_y + scale_unit * PointsIn[i, 1];

                            if (i < (INoPointsIn - 1))
                            {
                                l.X2 = modelposition_x + scale_unit * PointsIn[i + 1, 0];
                                l.Y2 = modelposition_y + scale_unit * PointsIn[i + 1, 1];
                            }
                            else
                            {
                                l.X2 = modelposition_x + scale_unit * PointsIn[0, 0];
                                l.Y2 = modelposition_y + scale_unit * PointsIn[0, 1];
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
                        DrawText((i + 1).ToString(), modelposition_x + scale_unit * PointsOut[i, 0], modelposition_x + scale_unit * PointsOut[i, 1], Brushes.Blue, canvasForImage);
                    }
                }

                // Internal outline points
                if (PointsIn != null && PointsOut != null) // If is array of points not empty
                {
                    for (int i = 0; i < INoPointsIn; i++)
                    {
                        DrawText((/*crsc.INoPointsOut +*/ i + 1).ToString(), modelposition_x + scale_unit * PointsIn[i, 0], modelposition_x + scale_unit * PointsIn[i, 1], Brushes.Green, canvasForImage);
                    }
                }
            }
        }

        public void DrawHoles()
        {
            if (bDrawHoles)
            {
                // Holes
                if (HolesCoord != null) // If is array of holes centers is not empty
                {
                    for (int i = 0; i < INoHoles; i++)
                    {
                        // Draw Hole
                        DrawCircle(new Point(modelposition_x + scale_unit * HolesCoord[i, 0], modelposition_y + scale_unit * HolesCoord[i, 1]), scale_unit * DHolesDiameter, Brushes.Black, 2, canvasForImage);
                        // Draw Symbol of Center
                        DrawSymbol_Cross(new Point(modelposition_x + scale_unit * HolesCoord[i, 0], modelposition_y + scale_unit * HolesCoord[i, 1]), scale_unit * DHolesDiameter + 10, Brushes.Red, 2, canvasForImage);
                    }
                }
            }
        }

        public void DrawPoint(Point point, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas)
		{
			DrawRectangle(strokeColor,fillColor, thickness, imageCanvas, new Point(point.X, point.Y), new Point(point.X + 4, point.Y + 4));
		}

		public void DrawLine(Line line, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
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
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(myLine, line.Y1);
            Canvas.SetLeft(myLine, line.X1);
            imageCanvas.Children.Add(myLine);
        }

        public void DrawPolyLine(float [,] arrPoints, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < arrPoints.Length / 2 + 1; i++)
            {
                if(i < ((arrPoints.Length / 2)))
                   points.Add(new Point(modelposition_x + scale_unit * arrPoints[i, 0], modelposition_y + scale_unit * arrPoints[i, 1]));
                else
                    points.Add(new Point(modelposition_x + scale_unit * arrPoints[0, 0], modelposition_y + scale_unit * arrPoints[0, 1])); // Last point is same as first one
            }

            Polyline myLine = new Polyline();
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(myLine, myLine.Points[0].Y);
            Canvas.SetLeft(myLine, myLine.Points[0].X);
            imageCanvas.Children.Add(myLine);
        }

        public void DrawText(string text, double posx, double posy, SolidColorBrush color, Canvas imageCanvas)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            Canvas.SetLeft(textBlock, posx);
            Canvas.SetTop(textBlock, posy);
            //textBlock.RenderTransform = new RotateTransform(0, 0, 0);
            textBlock.Margin = new Thickness(5, -50, 0, 0);
            textBlock.FontSize = 20;
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
	}
}
