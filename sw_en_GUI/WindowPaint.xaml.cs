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
using CENEX;
using BaseClasses;



namespace sw_en_GUI
{
	/// <summary>
	/// Interaction logic for WindowPaint.xaml
	/// </summary>
	public partial class WindowPaint : Window
	{
		public WindowPaint()
		{
			InitializeComponent();
			//myDrawing.Geometry.

			//            DrawingVisual ghostVisual = new DrawingVisual();
			//            using (DrawingContext dc = ghostVisual.RenderOpen())
			//            {
			//                // The body
			//                dc.DrawGeometry(Brushes.Blue, null, Geometry.Parse(
			//                @"M 240,250
			//				C 200,375 200,250 175,200
			//				C 100,400 100,250 100,200
			//					C 0,350 0,250 30,130
			//				C 75,0 100,0 150,0
			//				C 200,0 250,0 250,150 Z"));
			//                // Left eye
			//                dc.DrawEllipse(Brushes.Black, new Pen(Brushes.White, 10),
			//                new Point(95, 95), 15, 15);
			//                // Right eye
			//                dc.DrawEllipse(Brushes.Black, new Pen(Brushes.White, 10),
			//                new Point(170, 105), 15, 15);
			//                // The mouth
			//                Pen p = new Pen(Brushes.Black, 10);
			//                p.StartLineCap = PenLineCap.Round;
			//                p.EndLineCap = PenLineCap.Round;
			//                dc.DrawLine(p, new Point(75, 160), new Point(175, 150));
			//            }
			//            AddVisualChild(ghostVisual);
			//            AddLogicalChild(ghostVisual);
			//            //myDrawing.Geometry = ghostVisual.Ge;

			//StreamGeometry g = new StreamGeometry();
			//using (StreamGeometryContext context = g.Open())
			//{
			//    // Triangle #1
			//    context.BeginFigure(new Point(0, 0), true /*isFilled*/, true /*isClosed*/);
			//    context.LineTo(new Point(0, 100), true /*isStroked*/, true /*isSmoothJoin*/);
			//    context.LineTo(new Point(100, 100), true /*isStroked*/, true /*isSmoothJoin*/);
			//    // Triangle #2
			//    context.BeginFigure(new Point(70, 0), true /*isFilled*/, true /*isClosed*/);
			//    context.LineTo(new Point(0, 100), true /*isStroked*/, true /*isSmoothJoin*/);
			//    context.LineTo(new Point(100, 100), true /*isStroked*/, true /*isSmoothJoin*/);
			//}
			//// Apply this Geometry to an existing GeometryDrawing:
			//myDrawing.Geometry = g;

			//// Add a Line Element
			//Line myLine = new Line();
			//myLine.Stretch = Stretch.Uniform;
			//myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
			//myLine.X1 = 1;
			//myLine.X2 = 50;
			//myLine.Y1 = 1;
			//myLine.Y2 = 50;
			//myLine.HorizontalAlignment = HorizontalAlignment.Left;
			//myLine.VerticalAlignment = VerticalAlignment.Center;
			//myLine.StrokeThickness = 2;
			//myLine.StrokeStartLineCap = PenLineCap.Round;
			//canvasForImage.Children.Add(myLine);

			//PointCollection myPointCollection = new PointCollection();
			//myPointCollection.Add(new Point(0, 0));
			//myPointCollection.Add(new Point(0, 1));
			//myPointCollection.Add(new Point(1, 1));

			//Polygon myPolygon = new Polygon();
			//myPolygon.Points = myPointCollection;
			//myPolygon.Fill = Brushes.Blue;
			//myPolygon.Width = 100;
			//myPolygon.Height = 100;
			//myPolygon.Stretch = Stretch.Fill;
			//myPolygon.Stroke = Brushes.Black;
			//myPolygon.StrokeThickness = 2;

			//canvasForImage.Children.Add(myPolygon);

			//Ellipse myEllipse = new Ellipse();

			// Create a SolidColorBrush with a red color to fill the 
			// Ellipse with.
			//SolidColorBrush mySolidColorBrush = new SolidColorBrush();

			//// Describes the brush's color using RGB values. 
			//// Each value has a range of 0-255.
			//mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
			//myEllipse.Fill = mySolidColorBrush;
			//myEllipse.StrokeThickness = 2;
			//myEllipse.Stroke = Brushes.Black;

			//// Set the width and height of the Ellipse.
			//myEllipse.Width = 200;
			//myEllipse.Height = 100;
			//Canvas.SetTop(myEllipse, 100);
			//Canvas.SetLeft(myEllipse, 100);
			//// Add the Ellipse to the StackPanel.
			//canvasForImage.Children.Add(myEllipse);


			//// Create a path to draw a geometry with.
			//System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path();
			//myPath.Stroke = Brushes.Black;
			//myPath.StrokeThickness = 5;

			//// Create a StreamGeometry to use to specify myPath.
			//StreamGeometry geometry = new StreamGeometry();
			//geometry.FillRule = FillRule.EvenOdd;


			//// Open a StreamGeometryContext that can be used to describe this StreamGeometry 
			//// object's contents.
			//using (StreamGeometryContext ctx = geometry.Open())
			//{
			//    DrawRectangle(ctx, new Point(0, 0), new Point(1, 1));

			//    //// Begin the triangle at the point specified. Notice that the shape is set to 
			//    //// be closed so only two lines need to be specified below to make the triangle.
			//    //ctx.BeginFigure(new Point(10, 100), true /* is filled */, true /* is closed */);

			//    //// Draw a line to the next specified point.
			//    //ctx.LineTo(new Point(100, 100), true /* is stroked */, false /* is smooth join */);

			//    //// Draw another line to the next specified point.
			//    //ctx.LineTo(new Point(100, 50), true /* is stroked */, false /* is smooth join */);
			//}

			//// Freeze the geometry (make it unmodifiable)
			//// for additional performance benefits.
			//geometry.Freeze();

			//// Specify the shape (triangle) of the Path using the StreamGeometry.
			//myPath.Data = geometry;

			//// Add path shape to the UI.
			////canvasForImage.Children.Add(myPath);
			//myDrawing.Geometry = geometry;
			////this.Content = mainPanel;
			CNode n = new CNode(1, 10, 10, 10, 10);

			DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
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
				//CTest1 objCTest1 = new CTest1();
				//canvasForImage.Children.Clear();
				//foreach (CNode n in objCTest1.arrNodes)
				//{

				//}
			canvasForImage.Children.Clear();
			Line myLine = new Line();
			myLine.Stretch = Stretch.Fill;
			myLine.Stroke = Brushes.Aquamarine;
			myLine.X1 = 0;
			myLine.X2 = 1;
			myLine.Y1 = 0;
			myLine.Y2 =	1;
			myLine.StrokeThickness = 3;
			myLine.StrokeStartLineCap = PenLineCap.Round;
			myLine.StrokeEndLineCap = PenLineCap.Triangle;
			myLine.HorizontalAlignment = HorizontalAlignment.Left;
			myLine.VerticalAlignment = VerticalAlignment.Center;
			Canvas.SetTop(myLine, 2);
			Canvas.SetLeft(myLine, 4);
			canvasForImage.Children.Add(myLine);
			
			
			
		}

		private void menuItemTest2_Click(object sender, RoutedEventArgs e)
		{
            //CTest2 objCTest2 = new CTest2();
            //canvasForImage.Children.Clear();
			
            //foreach (CMember l in objCTest2.arrLines)
            //{
            //    // Add a Line Element
            //    DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
            //}
            //foreach (CNode n in objCTest2.arrNodes)
            //{
            //    DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
            //}
		}

		private void menuItemTest3_Click(object sender, RoutedEventArgs e)
		{
            
            canvasForImage.Children.Clear();
			
            //CTest3 objCTest3 = new CTest3();
            //canvasForImage.Children.Clear();
			
            //foreach (CMember l in objCTest3.arrLines)
            //{
            //    // Add a Line Element
            //    DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
            //}
            //foreach (CNode n in objCTest3.arrNodes)
            //{
            //    DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
            //}
		}

		private void menuItemTest4_Click(object sender, RoutedEventArgs e)
		{
			CTest4 objCTest4 = new CTest4();
			canvasForImage.Children.Clear();
			
			foreach (CMember l in objCTest4.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest4.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
			}
		}

		private void menuItemTest5_Click(object sender, RoutedEventArgs e)
		{
			CTest5 objCTest5 = new CTest5();
			canvasForImage.Children.Clear();
			
			foreach (CMember l in objCTest5.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest5.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);

			}
		}

		private void menuItemTest6_Click(object sender, RoutedEventArgs e)
		{
			CTest6 objCTest6 = new CTest6();
			canvasForImage.Children.Clear();
			
			foreach (CMember l in objCTest6.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest6.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
			}
		}

		private void menuItemTest7_Click(object sender, RoutedEventArgs e)
		{
			CTest7 objCTest7 = new CTest7();
			canvasForImage.Children.Clear();
			
			foreach (CMember l in objCTest7.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest7.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
			}
		}

		private void menuItemTest8_Click(object sender, RoutedEventArgs e)
		{
			CTest8 objCTest8 = new CTest8();
			canvasForImage.Children.Clear();
			
			foreach (CMember l in objCTest8.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest8.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
			}
		}

		private void menuItemTest9_Click(object sender, RoutedEventArgs e)
		{
			CTest9 objCTest9 = new CTest9();
			canvasForImage.Children.Clear();
			
			foreach (CMember l in objCTest9.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest9.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
			}
		}

		private void menuItemTest10_Click(object sender, RoutedEventArgs e)
		{
			CTest10 objCTest10 = new CTest10();
			canvasForImage.Children.Clear();
			foreach (CMember l in objCTest10.arrLines)
			{
				// Add a Line Element
				DrawLine(l, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
			}
			foreach (CNode n in objCTest10.arrNodes)
			{
				DrawNode(n, Brushes.Red, Brushes.Red, 4, canvasForImage);
			}
		}

		public void DrawNode(CNode node, SolidColorBrush strokeColor, SolidColorBrush fillColor, double thickness, Canvas imageCanvas)
		{
			DrawRectangle(strokeColor,fillColor, thickness, imageCanvas, new Point(node.X, node.Z), new Point(node.X + 4, node.Z + 4));
		}

		public void DrawLine(CMember line, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
		{
			Line myLine = new Line();
			myLine.Stretch = Stretch.Fill;
			myLine.Stroke = color;
			myLine.X1 = line.NodeStart.X;
			myLine.X2 = line.NodeEnd.X;
			myLine.Y1 = line.NodeStart.Z;
			myLine.Y2 = line.NodeEnd.Z;
			myLine.StrokeThickness = thickness;
			myLine.StrokeStartLineCap = startCap;
			myLine.StrokeEndLineCap = endCap;
			myLine.HorizontalAlignment = HorizontalAlignment.Left;
			myLine.VerticalAlignment = VerticalAlignment.Center;
			Canvas.SetTop(myLine, line.NodeStart.Z);
			Canvas.SetLeft(myLine, line.NodeStart.X);
			imageCanvas.Children.Add(myLine);
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


		//public void DrawNSupport(Graphics g, Pen p, CNSupport support)
		//{
		//    p.StartCap = LineCap.Flat;
		//    p.EndCap = LineCap.Flat;
		//    p.DashStyle = DashStyle.Solid;
		//    p.Color = Color.Yellow;
		//    p.Width = 1;
		//    int iLineOffset = 3;
		//    int iLineExtent = 2;

		//    int iSHeight = 10;
		//    int iSLengthSide = 2 * 6; // (int) iSHeight * Math.Tan((int)Math.PI/6); 0.57735 * 10

		//    Brush b = new SolidBrush(Color.Yellow);

		//    // Pole vrcholov trojuholnika
		//    // Triangle peaks
		//    Point[] arrPeaks = new Point[3];


		//    // Fill main point
		//    arrPeaks[0].X = support.m_iNode.m_fCoord_X;
		//    arrPeaks[0].Y = support.m_iNode.m_fCoord_Z;

		//    // Vykresli podporu pro X a Z (bez uvolenia - trojuholnik bez ciarky)
		//    if (support.m_bRestrain[0] && support.m_bRestrain[2])
		//    {
		//        if (!support.m_bRestrain[4]) // Rotation is not restrained
		//        {
		//            arrPeaks[1].X = arrPeaks[0].X - iSLengthSide / 2;
		//            arrPeaks[1].Y = arrPeaks[0].Y + iSHeight;

		//            arrPeaks[2].X = arrPeaks[0].X + iSLengthSide / 2;
		//            arrPeaks[2].Y = arrPeaks[0].Y + iSHeight;

		//            g.FillPolygon(b, arrPeaks, FillMode.Alternate);
		//        }
		//        else
		//        {
		//            // Draw Square
		//            // Kresli stvorec pre votknutie
		//            Rectangle rect = new Rectangle();
		//            // Set corner point coordinate  - upper left corner
		//            rect.X = support.m_iNode.m_fCoord_X - iSHeight / 2;
		//            rect.Y = support.m_iNode.m_fCoord_Z - iSHeight / 2;

		//            rect.Width = rect.Height = iSHeight;

		//            // Brush bBrushRestr = new SolidBrush(Color.Beige);
		//            // g.FillRectangle(bBrushRestr, rect);

		//            p.Color = Color.OrangeRed;
		//            g.DrawRectangle(p, rect);
		//        }
		//    }

		//    // Vykresli podporu pro X (uvolenie smeru Z - trojuholnik so zvislou ciarkou)
		//    else if (support.m_bRestrain[0] && !support.m_bRestrain[2])
		//    {
		//        // Triangle and Square Base Points
		//        arrPeaks[1].X = arrPeaks[0].X + iSHeight;
		//        arrPeaks[1].Y = arrPeaks[0].Y + iSLengthSide / 2;

		//        arrPeaks[2].X = arrPeaks[0].X + iSHeight;
		//        arrPeaks[2].Y = arrPeaks[0].Y - iSLengthSide / 2;

		//        if (!support.m_bRestrain[4]) // Rotation is not restrained
		//        {
		//            // Draw Triangle
		//            g.FillPolygon(b, arrPeaks, FillMode.Alternate);
		//        }
		//        else
		//        {
		//            // Draw Square
		//            // Kresli stvorec pre votknutie
		//            Rectangle rect = new Rectangle();
		//            // Set corner point coordinate  - upper left corner
		//            //rect.X = support.m_iNode.m_fCoord_X;
		//            rect.X = support.m_iNode.m_fCoord_X - iSHeight / 2;
		//            rect.Y = support.m_iNode.m_fCoord_Z - iSHeight / 2;

		//            rect.Width = rect.Height = iSHeight;

		//            //g.FillRectangle(b, rect);

		//            p.Color = Color.OrangeRed;
		//            g.DrawRectangle(p, rect);
		//        }

		//        // Vertical bar / pipe |

		//        g.DrawLine(p, arrPeaks[1].X + iLineOffset, arrPeaks[1].Y + iLineExtent, arrPeaks[2].X + iLineOffset, arrPeaks[2].Y - iLineExtent);
		//    }

		//    // Vykresli podporu pro Z (uvolenie smeru X - trojuholnik s vodorovnou ciarkou)
		//    else if (!support.m_bRestrain[0] && support.m_bRestrain[2])
		//    {
		//        // Triangle and Square Base Points
		//        arrPeaks[1].X = arrPeaks[0].X - iSLengthSide / 2;
		//        arrPeaks[1].Y = arrPeaks[0].Y + iSHeight;

		//        arrPeaks[2].X = arrPeaks[0].X + iSLengthSide / 2;
		//        arrPeaks[2].Y = arrPeaks[0].Y + iSHeight;

		//        if (!support.m_bRestrain[4]) // Rotation is not restrained
		//        {
		//            // Draw Triangle
		//            g.FillPolygon(b, arrPeaks, FillMode.Alternate);
		//        }
		//        else
		//        {
		//            // Draw Square
		//            // Kresli stvorec pre votknutie
		//            Rectangle rect = new Rectangle();
		//            // Set corner point coordinate  - upper left corner
		//            rect.X = support.m_iNode.m_fCoord_X - iSHeight / 2;
		//            //rect.Y = support.m_iNode.m_fCoord_Z;
		//            rect.Y = support.m_iNode.m_fCoord_Z - iSHeight / 2;

		//            rect.Width = rect.Height = iSHeight;

		//            //g.FillRectangle(b, rect);

		//            p.Color = Color.OrangeRed;
		//            g.DrawRectangle(p, rect);
		//        }

		//        // Horizontal bar / underline _

		//        g.DrawLine(p, arrPeaks[1].X - iLineExtent, arrPeaks[1].Y + iLineOffset, arrPeaks[2].X + iLineExtent, arrPeaks[2].Y + iLineOffset);
		//    }
		//    else
		//    {
		//        // restraint for Y - direction or any restraint
		//    }
		//}

		//public void DrawMemberRel(Graphics g, Pen p, CMember iLine)
		//{
		//    // Drawing points - if empty / do not draw release/hinge at member/line edge 
		//    Point pNode1 = new Point();
		//    Point pNode2 = new Point();

		//    switch (iLine.m_iMR.m_iNodeCode)
		//    {
		//        case 0: // Both Nodes
		//            {
		//                pNode1.X = iLine.m_iNode1.m_fCoord_X;
		//                pNode1.Y = iLine.m_iNode1.m_fCoord_Z;

		//                pNode2.X = iLine.m_iNode2.m_fCoord_X;
		//                pNode2.Y = iLine.m_iNode2.m_fCoord_Z;
		//                break;
		//            }
		//        case 1: // Start Node
		//            {
		//                pNode1.X = iLine.m_iNode1.m_fCoord_X;
		//                pNode1.Y = iLine.m_iNode1.m_fCoord_Z;
		//                break;
		//            }
		//        case 2: // End Node
		//            {
		//                pNode2.X = iLine.m_iNode2.m_fCoord_X;
		//                pNode2.Y = iLine.m_iNode2.m_fCoord_Z;
		//                break;
		//            }
		//        default: // No Member Release
		//            {
		//                break;
		//            }
		//    }

		//    // Draw Hinge
		//    DrawRelease(g, iLine.m_iMR, pNode1, pNode2);
		//}

		//public void DrawCircleHinge(Graphics g, int iHingeDiameter, Point pNode)
		//{
		//    Pen p = new Pen(Color.PeachPuff);
		//    p.DashStyle = DashStyle.Solid;
		//    p.Width = 1;

		//    // pNode - Input is Centre of Circle

		//    // Upper left Node
		//    pNode.X -= iHingeDiameter / 2;
		//    pNode.Y -= iHingeDiameter / 2;

		//    g.DrawEllipse(p, pNode.X, pNode.Y, iHingeDiameter, iHingeDiameter);
		//}

		//public void DrawRelease(Graphics g, CMembRelease iHinge, Point pNode1, Point pNode2)
		//{

		//    float fLengthX = pNode2.X - pNode1.X;
		//    float fLengthZ = pNode2.Y - pNode1.Y;
		//    float fLength = (float)Math.Sqrt(Math.Pow(fLengthX, 2) + Math.Pow(fLengthZ, 2));

		//    float fAlpha;

		//    /*
		//     Length X	Length Z	Alpha Rad	    Alpha Deg
		//          5	        2,5 	 0,463647609	 26,56505118
		//         -5	        2,5	    -0,463647609	-26,56505118
		//         -5	       -2,5 	 0,463647609	 26,56505118
		//          5	       -2,5	    -0,463647609	-26,56505118
		//    */

		//    // 1st Quadrant 
		//    // 0 < Alpha < 90 (PI/2)
		//    if (fLengthX >= 0f && fLengthZ >= 0)
		//    {
		//        fAlpha = (float)Math.Atan(fLengthZ / fLengthX);
		//    }
		//    // 2nd Quadrant
		//    //  90 (PI/2) < Alpha < 180 (PI)
		//    else if (fLengthX <= 0f && fLengthZ >= 0)
		//    {
		//        fAlpha = (float)Math.PI + (float)Math.Atan(fLengthZ / fLengthX);
		//    }
		//    // 3rd Quadrant
		//    //  180 (PI) < Alpha < 270 (3/2*PI)
		//    else if (fLengthX <= 0f && fLengthZ <= 0)
		//    {
		//        fAlpha = (float)Math.PI + (float)Math.Atan(fLengthZ / fLengthX);
		//    }
		//    // 4th Quadrant
		//    //  270 (3/2*PI) < Alpha < 360 (2*PI)
		//    else if (fLengthX >= 0f && fLengthZ <= 0)
		//    {
		//        fAlpha = (float)(2 * Math.PI) + (float)Math.Atan(fLengthZ / fLengthX);
		//    }
		//    else // Exception
		//    {
		//        fAlpha = 0f;
		//    }

		//    int iHingeDiameter = 8;
		//    int iX = (int)Math.Abs((float)Math.Cos(fAlpha) * iHingeDiameter);
		//    int iY = (int)Math.Abs((float)Math.Sin(fAlpha) * iHingeDiameter);


		//    // Centre Points

		//    Point pNode1Centre = new Point();
		//    Point pNode2Centre = new Point();


		//    /*
		//    1-----2
		//    */
		//    // Alpha = 0 (0)
		//    if (fLengthX > 0 && fLengthZ == 0)
		//    {
		//        pNode1Centre.X = pNode1.X + iHingeDiameter / 2;
		//        pNode1Centre.Y = pNode1.Y;

		//        pNode2Centre.X = pNode2.X - iHingeDiameter / 2;
		//        pNode2Centre.Y = pNode2.Y;
		//    }
		//    // Diagonal line Left->Right Slope descending
		//    /*
		//        1
		//         \ 
		//          \
		//           2
		//    */
		//    // 0 < Alpha < 90 (PI/2) 
		//    else if ((pNode1.X < pNode2.X) && (pNode1.Y < pNode2.Y))
		//    {
		//        pNode1Centre.X = pNode1.X + iX;
		//        pNode1Centre.Y = pNode1.Y + iY;

		//        pNode2Centre.X = pNode2.X - iX;
		//        pNode2Centre.Y = pNode2.Y - iY;
		//    }
		//    /*
		//     1
		//     |
		//     |
		//     2
		//    */
		//    // Alpha = 90 (PI/2)
		//    else if (fLengthX == 0 && fLengthZ > 0)
		//    {
		//        pNode1Centre.X = pNode1.X;
		//        pNode1Centre.Y = pNode1.Y + iHingeDiameter / 2;

		//        pNode2Centre.X = pNode2.X;
		//        pNode2Centre.Y = pNode2.Y - iHingeDiameter / 2;
		//    }
		//    /* 
		//         1
		//        / 
		//       /
		//      2
		//    */
		//    //  90 (PI/2) < Alpha < 180 (PI)
		//    else if ((pNode2.X < pNode1.X) && (pNode2.Y > pNode1.Y))
		//    {
		//        pNode1Centre.X = pNode1.X - iX;
		//        pNode1Centre.Y = pNode1.Y + +iY;

		//        pNode2Centre.X = pNode2.X + iX;
		//        pNode2Centre.Y = pNode2.Y - iY;
		//    }
		//    /*
		//    2-----1
		//   */
		//    // Alpha = 180 (PI)
		//    if (fLengthX < 0 && fLengthZ == 0)
		//    {
		//        pNode1Centre.X = pNode1.X - iHingeDiameter / 2;
		//        pNode1Centre.Y = pNode1.Y;

		//        pNode2Centre.X = pNode2.X + iHingeDiameter / 2;
		//        pNode2Centre.Y = pNode2.Y;
		//    }
		//    // Diagonal line Left->Right Slope descending
		//    /* 
		//        2
		//         \ 
		//          \
		//           1
		//    */
		//    //  180 (PI) < Alpha < 270 (3/2*PI)
		//    else if ((pNode2.X < pNode1.X) && (pNode2.Y < pNode1.Y))
		//    {
		//        pNode1Centre.X = pNode1.X - iX;
		//        pNode1Centre.Y = pNode1.Y - iY;

		//        pNode2Centre.X = pNode2.X + iX;
		//        pNode2Centre.Y = pNode2.Y + iY;
		//    }
		//    /*
		//     2
		//     |
		//     |
		//     1
		//    */
		//    // Alpha = 270 (3/2*PI)
		//    else if (fLengthX == 0 && fLengthZ < 0)
		//    {
		//        pNode1Centre.X = pNode1.X;
		//        pNode1Centre.Y = pNode1.Y - iHingeDiameter / 2;

		//        pNode2Centre.X = pNode2.X;
		//        pNode2Centre.Y = pNode2.Y + iHingeDiameter / 2;
		//    }
		//    // Diagonal line Left->Right Slope ascending
		//    /*
		//          2
		//         / 
		//        /
		//       1
		//    */
		//    //  270 (3/2*PI) < Alpha < 360 (2*PI)
		//    else if ((pNode1.X < pNode2.X) && (pNode1.Y > pNode2.Y))
		//    {
		//        pNode1Centre.X = pNode1.X + iX;
		//        pNode1Centre.Y = pNode1.Y - iY;

		//        pNode2Centre.X = pNode2.X - iX;
		//        pNode2Centre.Y = pNode2.Y + iY;
		//    }
		//    else // Exception
		//    {
		//        pNode1Centre.X = pNode1.X;
		//        pNode1Centre.Y = pNode1.Y;

		//        pNode2Centre.X = pNode2.X;
		//        pNode2Centre.Y = pNode2.Y;
		//    }


		//    // Draw Releases
		//    // 0-5  // UX, UY, UZ, RX, RY, RZ

		//    if (iHinge.m_bRestrain[0])  // Local UX / Displacement / Draw line pair parallel to the local x-Axis
		//    {
		//        if (!pNode1.IsEmpty)
		//            DrawStraigthRel(g, pNode1Centre, fAlpha);
		//        if (!pNode2.IsEmpty)
		//            DrawStraigthRel(g, pNode2Centre, fAlpha);
		//    }

		//    if (iHinge.m_bRestrain[2])  // Local UZ / Displacement / Draw line pair transverse to the local x-Axis 
		//    {
		//        if (!pNode1.IsEmpty)

		//            DrawStraigthRel(g, pNode1Centre, fAlpha + (float)Math.PI / 2f);
		//        if (!pNode2.IsEmpty)
		//            DrawStraigthRel(g, pNode2Centre, fAlpha + (float)Math.PI / 2f);
		//    }

		//    if (iHinge.m_bRestrain[4])  // Local RY - Rotation / Draw Circle
		//    {
		//        if (!pNode1.IsEmpty)
		//            DrawCircleHinge(g, iHingeDiameter, pNode1Centre);
		//        if (!pNode2.IsEmpty)
		//            DrawCircleHinge(g, iHingeDiameter, pNode2Centre);
		//    }
		//}

		//public void DrawNForce(Graphics g, Pen p, CNForce force)
		//{
		//    p.StartCap = LineCap.ArrowAnchor;
		//    p.EndCap = LineCap.Flat;
		//    p.Color = Color.Red;
		//    p.Width = 6;

		//    // Vykresli kladnou silu ve smeru X smerem doprava
		//    if (force.m_fValueX > 0.0f)
		//    {
		//        g.DrawLine(p,
		//        force.m_iNode.m_fCoord_X,
		//        force.m_iNode.m_fCoord_Z,
		//        force.m_iNode.m_fCoord_X - force.m_fValueX,
		//        force.m_iNode.m_fCoord_Z);
		//    }

		//    // Vykresli zapornou silu ve smeru X smerem doleva
		//    if (force.m_fValueX < 0.0f)
		//    {
		//        g.DrawLine(p,
		//        force.m_iNode.m_fCoord_X,
		//        force.m_iNode.m_fCoord_Z,
		//        force.m_iNode.m_fCoord_X - force.m_fValueX,
		//        force.m_iNode.m_fCoord_Z);
		//    }

		//    // Vykresli kladnou silu ve smeru Z - smerem nahor
		//    if (force.m_fValueZ > 0.0f)
		//    {
		//        g.DrawLine(p,
		//        force.m_iNode.m_fCoord_X,
		//        force.m_iNode.m_fCoord_Z,
		//        force.m_iNode.m_fCoord_X,
		//        force.m_iNode.m_fCoord_Z + force.m_fValueZ);
		//    }

		//    // Vykresli zapornou silu ve smeru Z - smerem nadol
		//    if (force.m_fValueZ < 0.0f)
		//    {
		//        g.DrawLine(p,
		//        force.m_iNode.m_fCoord_X,
		//        force.m_iNode.m_fCoord_Z,
		//        force.m_iNode.m_fCoord_X,
		//        force.m_iNode.m_fCoord_Z + force.m_fValueZ);
		//    }
		//}


















		//// Draw Numbering / coordinates / values

		//public void DrawNode_Numbering(Graphics g, CNode node)
		//{
		//    Font font = new Font("Courier new", 10);
		//    Brush brush = new SolidBrush(Color.Aquamarine);

		//    g.DrawString(node.m_iNode_ID.ToString(), font, brush, (int)node.m_fCoord_X - 20, (int)node.m_fCoord_Z - 20);
		//}

		//public void DrawNode_Coordinates(Graphics g, CNode node)
		//{
		//    Font font = new Font("Courier new", 8);
		//    Brush brush = new SolidBrush(Color.Azure);

		//    g.DrawString("[" + (int)node.m_fCoord_X + ","
		//                     + (int)node.m_fCoord_Y + ","
		//                     + (int)node.m_fCoord_Z + "]",
		//                    font,
		//                    brush,
		//                    (int)node.m_fCoord_X + 20, (int)node.m_fCoord_Z - 20);
		//}

		//public void DrawLine_Numbering(Graphics g, CMember line)
		//{
		//    Font font = new Font("Courier new", 10);
		//    Brush brush = new SolidBrush(Color.Crimson);

		//    int iLineCentreCoord_X = Math.Min((int)line.m_iNode2.m_fCoord_X, (int)line.m_iNode1.m_fCoord_X) + (Math.Abs((int)line.m_iNode2.m_fCoord_X - (int)line.m_iNode1.m_fCoord_X) / 2);
		//    int iLineCentreCoord_Z = Math.Min((int)line.m_iNode2.m_fCoord_Z, (int)line.m_iNode1.m_fCoord_Z) + (Math.Abs((int)line.m_iNode2.m_fCoord_Z - (int)line.m_iNode1.m_fCoord_Z) / 2);

		//    // Same Z coordinates - horizontal line
		//    if (Math.Abs(line.m_iNode2.m_fCoord_Z - line.m_iNode1.m_fCoord_Z) < 5)
		//        g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X, iLineCentreCoord_Z - 20);
		//    // Same X coordinates - vertical line
		//    else if (Math.Abs(line.m_iNode2.m_fCoord_X - line.m_iNode1.m_fCoord_X) < 5)
		//        g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X + 10, iLineCentreCoord_Z);
		//    // Diagonal line Left->Right Slope descending
		//    else if (((line.m_iNode1.m_fCoord_X < line.m_iNode2.m_fCoord_X) && (line.m_iNode1.m_fCoord_Z < line.m_iNode2.m_fCoord_Z)) ||
		//             ((line.m_iNode2.m_fCoord_X < line.m_iNode1.m_fCoord_X) && (line.m_iNode2.m_fCoord_Z < line.m_iNode1.m_fCoord_Z)))
		//        g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X + 10, iLineCentreCoord_Z - 10);
		//    // Diagonal line Left->Right Slope ascending
		//    else if (((line.m_iNode1.m_fCoord_X < line.m_iNode2.m_fCoord_X) && (line.m_iNode1.m_fCoord_Z > line.m_iNode2.m_fCoord_Z)) ||
		//             ((line.m_iNode2.m_fCoord_X < line.m_iNode1.m_fCoord_X) && (line.m_iNode2.m_fCoord_Z > line.m_iNode1.m_fCoord_Z)))
		//        g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X + 10, iLineCentreCoord_Z + 10);
		//    else
		//    {
		//        // Exception
		//    }
		//}

		//public void DrawSupport_Numbering(Graphics g, CNSupport support)
		//{
		//    Font font = new Font("Courier new", 10);
		//    Brush brush = new SolidBrush(Color.Yellow);

		//    g.DrawString(support.m_iSupport_ID.ToString(), font, brush, support.m_iNode.m_fCoord_X - 20, support.m_iNode.m_fCoord_Z + 10);
		//}

		//public void DrawForce_Values(Graphics g, CNForce force)
		//{
		//    Font font = new Font("Courier new", 8);
		//    Brush brush = new SolidBrush(Color.DarkOrange);

		//    // Force in X direction
		//    if (force.m_fValueX > 0)
		//        g.DrawString("Fx = " + Math.Round(force.m_fValueX, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X - (int)force.m_fValueX - 80, (int)force.m_iNode.m_fCoord_Z + 10);
		//    else if (force.m_fValueX < 0)
		//        g.DrawString("Fx = " + Math.Round(force.m_fValueX, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X - (int)force.m_fValueX + 10, (int)force.m_iNode.m_fCoord_Z + 10);
		//    else
		//    {
		//        // Exception
		//    }

		//    // Force in Z direction
		//    if (force.m_fValueZ > 0)
		//        g.DrawString("Fz = " + Math.Round(force.m_fValueZ, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X + 10, (int)force.m_iNode.m_fCoord_Z + (int)force.m_fValueZ + 10);
		//    else if (force.m_fValueZ < 0)
		//        g.DrawString("Fz = " + Math.Round(force.m_fValueZ, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X + 10, (int)force.m_iNode.m_fCoord_Z + (int)force.m_fValueZ - 10);
		//    else
		//    {
		//        // Exception
		//    }
		//}

		//// Draw Global Coordinates System Symbol

		//public void DrawGCS(Graphics g, int iXmin, int iZmax)
		//{
		//    Font font = new Font("Courier new", 12);
		//    Brush brushX = new SolidBrush(Color.Red);
		//    Brush brushY = new SolidBrush(Color.LightGreen);
		//    Brush brushZ = new SolidBrush(Color.Blue);
		//    Pen pen = new Pen(Color.PeachPuff);
		//    pen.StartCap = LineCap.Round;
		//    pen.EndCap = LineCap.ArrowAnchor;
		//    pen.Width = 4;

		//    // Lines
		//    // X-Axis
		//    pen.Color = Color.Red;
		//    g.DrawLine(pen, iXmin + 20, iZmax - 20, iXmin + 60, iZmax - 20);
		//    g.DrawString("X", font, brushX, iXmin + 65, iZmax - 20);
		//    // Y-Axis
		//    pen.Color = Color.LightGreen;
		//    g.DrawLine(pen, iXmin + 20, iZmax - 20, iXmin + 50, iZmax - 50);
		//    g.DrawString("Y", font, brushY, iXmin + 55, iZmax - 55);
		//    // Z-Axis
		//    pen.Color = Color.Blue;
		//    g.DrawLine(pen, iXmin + 20, iZmax - 20, iXmin + 20, iZmax - 60);
		//    g.DrawString("Z", font, brushZ, iXmin + 20, iZmax - 65);
		//}

		//// Temporary
		//// Draw Cross-section in 2D

		//public void DrawCrSc2D(Graphics g, CCrSc_0_50 objCrSc)
		//{
		//    Brush b = new SolidBrush(Color.Yellow);
		//    Pen p = new Pen(Color.Cornsilk, 1);

		//    int iSize = 2; // Size of point

		//    // Points
		//    for (short i = 0; i < objCrSc.ITotNoPoints; i++)
		//    {

		//        g.FillRectangle(b,
		//        objCrSc.m_CrScPoint[i, 0] - (iSize / 2),
		//        objCrSc.m_CrScPoint[i, 1] - (iSize / 2),
		//        iSize,
		//        iSize);
		//    }

		//    // Change Color
		//    p.Color = Color.DarkCyan;

		//    // Liness
		//    for (short i = 0; i < objCrSc.ITotNoPoints - 1; i++)
		//    {
		//        g.DrawLine(p,
		//        objCrSc.m_CrScPoint[i, 0],
		//        objCrSc.m_CrScPoint[i, 1],
		//        objCrSc.m_CrScPoint[i + 1, 0],
		//        objCrSc.m_CrScPoint[i + 1, 1]);
		//    }
		//}
	}
}
