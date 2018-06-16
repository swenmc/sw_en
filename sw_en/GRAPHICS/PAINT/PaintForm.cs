using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CENEX
{
    public partial class PaintForm : Form
    {
        private Bitmap bitmap = null;
        private Bitmap curBitmap = null;
        private bool dragMode = false;
        private int drawIndex = 1;
        private int curX, curY, x, y;
        private int diffX, diffY;
        private Graphics curGraphics;
        private Pen curPen;
        private SolidBrush curBrush;
        private Size fullSize;


        public PaintForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Get the full size of the form
            fullSize = SystemInformation
              .PrimaryMonitorMaximizedWindowSize;
            // Create a bitmap using full size
            bitmap = new Bitmap(fullSize.Width,
              fullSize.Height);
            // Create a Graphics object from Bitmap
            curGraphics = Graphics.FromImage(bitmap);
            // Set background color as form's color
            curGraphics.Clear(this.BackColor);
            // Create a new pen and brush as
            // default pen and brush
            curPen = new Pen(Color.White);
            curBrush = new SolidBrush(Color.SkyBlue);

            /*
            curHatchBrush = new HatchBrush();
            curLinearGradientBrush = new LinearGradientBrush ();
            curPathGradientBrush  = new PathGradientBrush ();
            curTextureBrush = new TextureBrush();
             */

        }

        private void LineDraw_Click(object sender, System.EventArgs e)
        {
            drawIndex = 1;
        }
        private void RectDraw_Click(object sender, System.EventArgs e)
        {
            drawIndex = 2;
        }
        private void EllipseDraw_Click(object sender, System.EventArgs e)
        {
            drawIndex = 3;
        }
        private void FilledRectangle_Click(object sender, System.EventArgs e)
        {
            drawIndex = 4;
        }
        private void FilledEllipse_Click(object sender, System.EventArgs e)
        {
            drawIndex = 5;
        }

        



        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Store the starting point of
            // the rectangle and set the drag mode
            // to true
            curX = e.X;
            curY = e.Y;
            dragMode = true;

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // Find out the ending point of
            // the rectangle and calculate the
            // difference between starting and ending
            // points to find out the height and width
            // of the rectangle
            x = e.X;
            y = e.Y;
            diffX = e.X - curX;
            diffY = e.Y - curY;
            // If dragMode is true, call refresh
            // to force the window to repaint
            if (dragMode)
            {
                this.Refresh();
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            diffX = x - curX;
            diffY = y - curY;
            switch (drawIndex)
            {
                case 1:
                    {
                        // Draw a line
                        curGraphics.DrawLine(curPen,
                          curX, curY, x, y);
                        break;
                    }
                case 2:
                    {
                        // Draw a rectangle
                        curGraphics.DrawRectangle(curPen,
                          curX, curY, diffX, diffY);
                        break;
                    }
                case 3:
                    {

                        // Draw an ellipse
                        curGraphics.DrawEllipse(curPen,
                          curX, curY, diffX, diffY);
                        break;
                    }
                case 4:
                    {
                        // Fill the rectangle
                        curGraphics.FillRectangle(curBrush,
                          curX, curY, diffX, diffY);
                        break;
                    }
                case 5:
                    {
                        // Fill the ellipse
                        curGraphics.FillEllipse(curBrush,
                          curX, curY, diffX, diffY);
                        break;
                    }
            }
            // Refresh
            RefreshFormBackground();
            // Set drag mode to false
            dragMode = false;

        }
        private void RefreshFormBackground()
        {
            curBitmap = bitmap.Clone(
              new Rectangle(0, 0, this.Width, this.Height),
              bitmap.PixelFormat);
            this.BackgroundImage = curBitmap;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // If dragMode is true, draw the selected
            // graphics shape
            if (dragMode)
            {
                switch (drawIndex)
                {
                    case 1:
                        {
                            g.DrawLine(curPen, curX, curY, x, y);
                            break;
                        }
                    case 2:
                        {
                            g.DrawEllipse(curPen,
                              curX, curY, diffX, diffY);
                            break;
                        }
                    case 3:
                        {
                            g.DrawRectangle(curPen,
                              curX, curY, diffX, diffY);
                            break;
                        }
                    case 4:
                        {
                            g.FillRectangle(curBrush,
                              curX, curY, diffX, diffY);
                            break;
                        }
                    case 5:
                        {
                            g.FillEllipse(curBrush,
                              curX, curY, diffX, diffY);
                            break;
                        }
                }
            }

        }
       
        
        
        private void specimen1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // draws shapes with different brushes
            // draw various shapes on Form

            // references to object we will use
            Graphics graphicsObject = this.curGraphics;//e.Graphics;

            // ellipse rectangle and gradient brush
            Rectangle drawArea1 = new Rectangle(5, 35, 30, 100);
            LinearGradientBrush linearBrush =
               new LinearGradientBrush(drawArea1, Color.Blue,
                  Color.Yellow, LinearGradientMode.ForwardDiagonal);

            // draw ellipse filled with a blue-yellow gradient
            graphicsObject.FillEllipse(linearBrush, 5, 30, 65, 100);

            // pen and location for red outline rectangle
            Pen thickRedPen = new Pen(Color.Red, 10);
            Rectangle drawArea2 = new Rectangle(80, 30, 65, 100);

            // draw thick rectangle outline in red
            graphicsObject.DrawRectangle(thickRedPen, drawArea2);

            // bitmap texture
            Bitmap textureBitmap = new Bitmap(10, 10);

            // get bitmap graphics
            Graphics graphicsObject2 =
               Graphics.FromImage(textureBitmap);

            // brush and pen used throughout program
            SolidBrush solidColorBrush =
               new SolidBrush(Color.Red);
            Pen coloredPen = new Pen(solidColorBrush);

            // fill textureBitmap with yellow
            solidColorBrush.Color = Color.Yellow;
            graphicsObject2.FillRectangle(solidColorBrush, 0, 0, 10, 10);

            // draw small black rectangle in textureBitmap
            coloredPen.Color = Color.Black;
            graphicsObject2.DrawRectangle(coloredPen, 1, 1, 6, 6);

            // draw small blue rectangle in textureBitmap
            solidColorBrush.Color = Color.Blue;
            graphicsObject2.FillRectangle(solidColorBrush, 1, 1, 3, 3);

            // draw small red square in textureBitmap
            solidColorBrush.Color = Color.Red;
            graphicsObject2.FillRectangle(solidColorBrush, 4, 4, 3, 3);

            // create textured brush and 
            // display textured rectangle
            TextureBrush texturedBrush =
               new TextureBrush(textureBitmap);
            graphicsObject.FillRectangle(texturedBrush, 155, 30, 75, 100);

            // draw pie-shaped arc in white
            coloredPen.Color = Color.White;
            coloredPen.Width = 6;
            graphicsObject.DrawPie(coloredPen, 240, 30, 75, 100, 0, 270);

            // draw lines in green and yellow
            coloredPen.Color = Color.Green;
            coloredPen.Width = 5;
            graphicsObject.DrawLine(coloredPen, 395, 30, 320, 150);

            // draw a rounded, dashed yellow line
            coloredPen.Color = Color.Yellow;
            coloredPen.DashCap = DashCap.Round;
            coloredPen.DashStyle = DashStyle.Dash;
            graphicsObject.DrawLine(coloredPen, 320, 30, 395, 150);
            // end method
        }

        // create path and draw stars along it
        private void tutorial2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics graphicsObject = curGraphics;
            Random random = new Random();
            SolidBrush brush =
               new SolidBrush(Color.DarkMagenta);

            // x and y points of the path
            int[] xPoints = { 55, 67, 109, 73, 83, 55, 27, 37, 1, 43 };
            int[] yPoints = { 0, 36, 36, 54, 96, 72, 96, 54, 36, 36 };

            // create graphics path for star;
            GraphicsPath star = new GraphicsPath();

            // create star from series of points
            for (int i = 0; i <= 8; i += 2)
                star.AddLine(xPoints[i], yPoints[i],
                   xPoints[i + 1], yPoints[i + 1]);

            // close the shape
            star.CloseFigure();

            // translate the origin to (150, 150)
            graphicsObject.TranslateTransform(150, 150);

            // rotate the origin and draw stars in random colors
            for (int i = 1; i <= 18; i++)
            {
                graphicsObject.RotateTransform(20);

                brush.Color = Color.FromArgb(
                   random.Next(200, 255), random.Next(255),
                   random.Next(255), random.Next(255));

                graphicsObject.FillPath(brush, star);
            } // end for
        }

        

        

        }
}