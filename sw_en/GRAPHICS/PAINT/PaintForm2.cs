//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using CRSC;









namespace CENEX
{
	public partial class PaintForm2 : System.Windows.Forms.Form
	{ }
}
//        private Bitmap bitmap = null;
//        private bool dragMode = false;
//        private int drawIndex = 0;
//        public Graphics curGraphics;
//        public Pen curPen;
//        public SolidBrush curBrush;
//        private Size fullSize;
//        public EDOF eDOF;


//        private int iFormSizeWidth = ActiveForm.Size.Width;
//        private int iFormSizeHeight = ActiveForm.Size.Height;

//        /*
//        iFormSizeWidth = PaintForm2.ActiveForm.Size.Width;
//        iFormSizeHeight = PaintForm2.ActiveForm.Size.Height;
//        */

//        /*
//        iFormSizeWidth = ActiveForm.Size.Width;
//        iFormSizeHeight = ActiveForm.Size.Height;
//        */

//        // Displayed Structure Dimensions

//        int iMin_X;
//        int iMax_X;
//        int iMin_Y;
//        int iMax_Y;

//        int iStructWidth;
//        int iStructHeight;

//        public int m_fNodeDrawSizeX = 4;
//        public int m_fNodeDrawSizeZ = 4;

//        // Items
//        public bool bDisplayNodes;
//        public bool bDisplayLines;
//        public bool bDisplayNSupports;
//        public bool bDisplayMembRel;
//        public bool bDisplayNForces;

//        // Items Numbering
//        public bool bDisplayNodesNumbering;
//        public bool bDisplayNodesCoordinates;
//        public bool bDisplayLinesNumbering;
//        public bool bDisplaySupportsNumbering;
//        public bool bDisplayMembRelsNumbering;
//        public bool bDisplayNForcesValues;

//        // Display Global Coordinates System
//        public bool bDisplayGCS;


//        // Definice objektu pro zobrazeni
//        CNode[] arrNodes;
//        CLine[] arrLines;
//        CNSupport[] arrSupports;
//        CNForce []arrForce;
//        //CCrSc_I objCrSc;

//        public PaintForm2()
//        {
//            InitializeComponent();

//            BackColor = Color.Black;

//        }

//        public void PaintForm2_Load(object sender, EventArgs e)
//        {
//            // Get the full size of the form
//            fullSize = SystemInformation
//              .PrimaryMonitorMaximizedWindowSize;
//            // Create a bitmap using full size
//            bitmap = new Bitmap(fullSize.Width,
//              fullSize.Height);
//            // Create a Graphics object from Bitmap
//            curGraphics = Graphics.FromImage(bitmap);

//            // Set background color as form's color
//            curGraphics.Clear(this.BackColor);

//            // Create a new pen and brush as
//            // default pen and brush
//            curPen = new Pen(Color.White);
//            curBrush = new SolidBrush(Color.SkyBlue);
//        }

//        public void GetStrucSize()
//        {
//            // Displayed Structure Dimensions

//            iMin_X = int.MaxValue;
//            iMax_X = int.MinValue;
//            iMin_Y = int.MaxValue;
//            iMax_Y = int.MinValue;

//            for (int i = 0; i < arrNodes.Length; i++)
//            {
//                if (arrNodes[i].m_fCoord_X < iMin_X)
//                    iMin_X = arrNodes[i].m_fCoord_X;

//                if (arrNodes[i].m_fCoord_X > iMax_X)
//                    iMax_X = arrNodes[i].m_fCoord_X;

//                if (arrNodes[i].m_fCoord_Z < iMin_Y)
//                    iMin_Y = arrNodes[i].m_fCoord_Z;

//                if (arrNodes[i].m_fCoord_Z > iMax_Y)
//                    iMax_Y = arrNodes[i].m_fCoord_Z;
//            }

//            iStructWidth = Math.Max(iMax_X - iMin_X, 0);
//            iStructHeight = Math.Max(iMax_Y - iMin_Y, 0);
        
//        }

//        public void CentreDrawing()
//        {
//            int iWindowCentre_X = iFormSizeWidth / 2;
//            int iWindowCentre_Y = iFormSizeHeight / 2;

//            int iStructCentre_X = iMin_X + iStructWidth / 2;
//            int iStructCentre_Y = iMin_Y + iStructHeight / 2;

//            for (int i = 0; i < arrNodes.Length; i++)
//            {
//                if (iStructCentre_X > iWindowCentre_X)
//                    arrNodes[i].m_fCoord_X -= iStructCentre_X - iWindowCentre_X;
//                else if (iStructCentre_X < iWindowCentre_X)
//                    arrNodes[i].m_fCoord_X += iWindowCentre_X - iStructCentre_X;
//                else
//                { 
//                    // no display move
//                }

//                if (iStructCentre_Y > iWindowCentre_Y)
//                    arrNodes[i].m_fCoord_Z -= iStructCentre_Y - iWindowCentre_Y;
//                else if (iStructCentre_Y < iWindowCentre_Y)
//                    arrNodes[i].m_fCoord_Z += iWindowCentre_Y - iStructCentre_Y;
//                else
//                {
//                    // no display move
//                }
//            }
//        }

//        public void ResizeDrawing()
//        {
//            // Get Scale / Structure size / Window Size

//            float iScale = Math.Max(((float)(iStructWidth + 100) / (float)iFormSizeWidth), (((float)iStructHeight + 100) / (float)iFormSizeHeight)); // 100 - distance from form border

//            for (int i = 0; i < arrNodes.Length; i++)
//            {
//                float fx = arrNodes[i].m_fCoord_X;
//                float fy = arrNodes[i].m_fCoord_Z;

//                    fx *= 1/iScale;
//                    fy *= 1/iScale;

//                    arrNodes[i].m_fCoord_X = (int)fx;
//                    arrNodes[i].m_fCoord_Z = (int)fy;
//            }
//        }

//        protected override void OnPaint(PaintEventArgs e)
//        {
//            Graphics grfx = e.Graphics;
//            Pen curPen = new Pen(Color.YellowGreen);
//            //curPen.Color = Color.Yellow;
//            //curPen.DashCap = DashCap.Round;
//            //curPen.DashStyle = DashStyle.Dash;
//            //curPen.DashStyle = DashStyle.Solid;
//            //curPen.DashCap = DashCap.Triangle;
//            //curPen.StartCap = LineCap.Triangle;
//            //curPen.EndCap = LineCap.ArrowAnchor;
//            //curPen.DashPattern 
//            curPen.Width = 2;
//            //curPen.Dispose();

//            Brush curBrush = new SolidBrush(Color.LightCyan);
//            grfx.SmoothingMode = SmoothingMode.AntiAlias;
//            grfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

//            // Draw items
//            if (bDisplayNodes && arrNodes != null) // Nodes
//            {
//                for (int i = 0; i < arrNodes.Length; i++)
//                    DrawNode(grfx, curBrush, arrNodes[i]);
//            }

//            if (bDisplayLines && arrLines != null)
//            {
//                for (int i = 0; i < arrLines.Length; i++)
//                    DrawLine(grfx, curPen, arrLines[i]);
//            }

//            if (bDisplayNSupports && arrSupports != null)
//            {
//                for (int i = 0; i < arrSupports.Length; i++)
//                    DrawNSupport(grfx, curPen,  arrSupports[i]);
//            }

//            if (bDisplayMembRel && arrLines != null)
//            {
//                for (int i = 0; i < arrLines.Length; i++)
//                {
//                    if(arrLines[i].m_iMR != null)
//                    DrawMemberRel(grfx, curPen, arrLines[i]);
//                }
//            }

//            if (bDisplayNForces && arrForce != null)
//            {
//                for (int i = 0; i < arrForce.Length; i++)
//                    DrawNForce(grfx, curPen, arrForce[i]);
//            }

//            // Draw items numbering

//            if (bDisplayNodesNumbering && arrNodes != null)
//            {
//                for (int i = 0; i < arrNodes.Length; i++)
//                    DrawNode_Numbering(grfx, arrNodes[i]);
//            }

//            if (bDisplayNodesCoordinates && arrNodes != null)
//            {
//                for (int i = 0; i < arrNodes.Length; i++)
//                    DrawNode_Coordinates(grfx, arrNodes[i]);
//            }

//            if (bDisplayLinesNumbering && arrLines != null)
//            {
//                for (int i = 0; i < arrLines.Length; i++)
//                    DrawLine_Numbering(grfx, arrLines[i]);
//            }

//            if (bDisplaySupportsNumbering && arrSupports != null)
//            {
//                for (int i = 0; i < arrSupports.Length; i++)
//                    DrawSupport_Numbering(grfx, arrSupports[i]);
//            }

//            if (bDisplayNForcesValues && arrForce != null)
//            {
//                for (int i = 0; i < arrForce.Length; i++)
//                    DrawForce_Values(grfx, arrForce[i]);
//            }

//           // Draw Global Coordinates System GCS
//            if(bDisplayGCS)
//            {
//                DrawGCS(grfx,0, iFormSizeHeight);
//            }
//        }


//        /// <summary>
//        /// Draw methods for each Draw Element Type
//        /// </summary>

//        public void DrawNode(Graphics g, Brush b , CNode node)
//        {
//            // Fill the rectangle
//            g.FillRectangle(b,
//                node.m_fCoord_X - (m_fNodeDrawSizeX/2),
//                node.m_fCoord_Z - (m_fNodeDrawSizeZ/2),
//                m_fNodeDrawSizeX,
//                m_fNodeDrawSizeZ);
//        }

//        public void DrawLine(Graphics g, Pen p, CLine line)
//        {
//            g.DrawLine(p,
//            line.m_iNode1.m_fCoord_X,
//            line.m_iNode1.m_fCoord_Z,
//            line.m_iNode2.m_fCoord_X,
//            line.m_iNode2.m_fCoord_Z);
//        }

//        public void DrawNSupport(Graphics g, Pen p, CNSupport support)
//        {
//            p.StartCap = LineCap.Flat;
//            p.EndCap = LineCap.Flat;
//            p.DashStyle = DashStyle.Solid;
//            p.Color = Color.Yellow;
//            p.Width = 1;
//            int iLineOffset = 3;
//            int iLineExtent = 2;

//            int iSHeight = 10;
//            int iSLengthSide = 2 * 6; // (int) iSHeight * Math.Tan((int)Math.PI/6); 0.57735 * 10

//            Brush b = new SolidBrush(Color.Yellow);

//            // Pole vrcholov trojuholnika
//            // Triangle peaks
//            Point[] arrPeaks = new Point[3];


//            // Fill main point
//            arrPeaks[0].X = support.m_iNode.m_fCoord_X;
//            arrPeaks[0].Y = support.m_iNode.m_fCoord_Z;

//            // Vykresli podporu pro X a Z (bez uvolenia - trojuholnik bez ciarky)
//            if (support.m_bRestrain[0] && support.m_bRestrain[2])
//            {
//                if (!support.m_bRestrain[4]) // Rotation is not restrained
//                {
//                    arrPeaks[1].X = arrPeaks[0].X - iSLengthSide / 2;
//                    arrPeaks[1].Y = arrPeaks[0].Y + iSHeight;

//                    arrPeaks[2].X = arrPeaks[0].X + iSLengthSide / 2;
//                    arrPeaks[2].Y = arrPeaks[0].Y + iSHeight;

//                    g.FillPolygon(b, arrPeaks, FillMode.Alternate);
//                }
//                else
//                {
//                    // Draw Square
//                    // Kresli stvorec pre votknutie
//                    Rectangle rect = new Rectangle();
//                    // Set corner point coordinate  - upper left corner
//                    rect.X = support.m_iNode.m_fCoord_X - iSHeight / 2;
//                    rect.Y = support.m_iNode.m_fCoord_Z - iSHeight / 2;

//                    rect.Width = rect.Height = iSHeight;

//                    // Brush bBrushRestr = new SolidBrush(Color.Beige);
//                    // g.FillRectangle(bBrushRestr, rect);

//                    p.Color = Color.OrangeRed;
//                    g.DrawRectangle(p, rect);
//                }
//            }

//            // Vykresli podporu pro X (uvolenie smeru Z - trojuholnik so zvislou ciarkou)
//            else if (support.m_bRestrain[0] && !support.m_bRestrain[2])
//            {
//                // Triangle and Square Base Points
//                arrPeaks[1].X = arrPeaks[0].X + iSHeight;
//                arrPeaks[1].Y = arrPeaks[0].Y + iSLengthSide / 2;

//                arrPeaks[2].X = arrPeaks[0].X + iSHeight;
//                arrPeaks[2].Y = arrPeaks[0].Y - iSLengthSide / 2;

//                if (!support.m_bRestrain[4]) // Rotation is not restrained
//                {
//                    // Draw Triangle
//                    g.FillPolygon(b, arrPeaks, FillMode.Alternate);
//                }
//                else
//                {
//                    // Draw Square
//                    // Kresli stvorec pre votknutie
//                    Rectangle rect = new Rectangle();
//                    // Set corner point coordinate  - upper left corner
//                    //rect.X = support.m_iNode.m_fCoord_X;
//                    rect.X = support.m_iNode.m_fCoord_X - iSHeight / 2;
//                    rect.Y = support.m_iNode.m_fCoord_Z - iSHeight / 2;

//                    rect.Width = rect.Height = iSHeight;

//                    //g.FillRectangle(b, rect);

//                    p.Color = Color.OrangeRed;
//                    g.DrawRectangle(p, rect);
//                }

//                // Vertical bar / pipe |

//                g.DrawLine(p, arrPeaks[1].X + iLineOffset, arrPeaks[1].Y + iLineExtent, arrPeaks[2].X + iLineOffset, arrPeaks[2].Y - iLineExtent);
//            }

//            // Vykresli podporu pro Z (uvolenie smeru X - trojuholnik s vodorovnou ciarkou)
//            else if (!support.m_bRestrain[0] && support.m_bRestrain[2])
//            {
//                // Triangle and Square Base Points
//                arrPeaks[1].X = arrPeaks[0].X - iSLengthSide / 2;
//                arrPeaks[1].Y = arrPeaks[0].Y + iSHeight;

//                arrPeaks[2].X = arrPeaks[0].X + iSLengthSide / 2;
//                arrPeaks[2].Y = arrPeaks[0].Y + iSHeight;

//                if (!support.m_bRestrain[4]) // Rotation is not restrained
//                {
//                    // Draw Triangle
//                    g.FillPolygon(b, arrPeaks, FillMode.Alternate);
//                }
//                else
//                {
//                    // Draw Square
//                    // Kresli stvorec pre votknutie
//                    Rectangle rect = new Rectangle();
//                    // Set corner point coordinate  - upper left corner
//                    rect.X = support.m_iNode.m_fCoord_X - iSHeight / 2;
//                    //rect.Y = support.m_iNode.m_fCoord_Z;
//                    rect.Y = support.m_iNode.m_fCoord_Z - iSHeight / 2;

//                    rect.Width = rect.Height = iSHeight;

//                    //g.FillRectangle(b, rect);

//                    p.Color = Color.OrangeRed;
//                    g.DrawRectangle(p, rect);
//                }

//                // Horizontal bar / underline _

//                g.DrawLine(p, arrPeaks[1].X - iLineExtent, arrPeaks[1].Y + iLineOffset, arrPeaks[2].X + iLineExtent, arrPeaks[2].Y + iLineOffset);
//            }
//            else
//            {
//                // restraint for Y - direction or any restraint
//            }
//        }

//        public void DrawMemberRel(Graphics g, Pen p, CLine iLine)
//        {
//            // Drawing points - if empty / do not draw release/hinge at member/line edge 
//            Point pNode1 = new Point();
//            Point pNode2 = new Point();

//            switch (iLine.m_iMR.m_iNodeCode)
//            {
//                case 0: // Both Nodes
//                    {
//                        pNode1.X = iLine.m_iNode1.m_fCoord_X;
//                        pNode1.Y = iLine.m_iNode1.m_fCoord_Z;

//                        pNode2.X = iLine.m_iNode2.m_fCoord_X;
//                        pNode2.Y = iLine.m_iNode2.m_fCoord_Z;
//                        break;
//                    }
//                case 1: // Start Node
//                    {
//                        pNode1.X = iLine.m_iNode1.m_fCoord_X;
//                        pNode1.Y = iLine.m_iNode1.m_fCoord_Z;
//                        break;
//                    }
//                case 2: // End Node
//                    {
//                        pNode2.X = iLine.m_iNode2.m_fCoord_X;
//                        pNode2.Y = iLine.m_iNode2.m_fCoord_Z;
//                        break;
//                    }
//                default: // No Member Release
//                    {
//                        break;
//                    }
//            }

//            // Draw Hinge
//            DrawRelease(g, iLine.m_iMR, pNode1, pNode2);
//        }

//        public void DrawCircleHinge(Graphics g, int iHingeDiameter, Point pNode)
//        {
//            Pen p = new Pen(Color.PeachPuff);
//            p.DashStyle = DashStyle.Solid;
//            p.Width = 1;

//            // pNode - Input is Centre of Circle
            
//            // Upper left Node
//            pNode.X -= iHingeDiameter / 2;
//            pNode.Y -= iHingeDiameter / 2;

//            g.DrawEllipse(p, pNode.X, pNode.Y, iHingeDiameter, iHingeDiameter);
//        }

//        public void DrawRelease(Graphics g, CMembRelease iHinge, Point pNode1, Point pNode2)
//        {

//            float fLengthX = pNode2.X - pNode1.X;
//            float fLengthZ = pNode2.Y - pNode1.Y;
//            float fLength = (float)Math.Sqrt(Math.Pow(fLengthX, 2) + Math.Pow(fLengthZ, 2));

//            float fAlpha;

//           /*
//            Length X	Length Z	Alpha Rad	    Alpha Deg
//                 5	        2,5 	 0,463647609	 26,56505118
//                -5	        2,5	    -0,463647609	-26,56505118
//                -5	       -2,5 	 0,463647609	 26,56505118
//                 5	       -2,5	    -0,463647609	-26,56505118
//           */

//            // 1st Quadrant 
//            // 0 < Alpha < 90 (PI/2)
//            if (fLengthX >= 0f && fLengthZ >= 0)
//            {
//                fAlpha = (float)Math.Atan(fLengthZ / fLengthX);
//            }
//            // 2nd Quadrant
//            //  90 (PI/2) < Alpha < 180 (PI)
//            else if (fLengthX <= 0f && fLengthZ >= 0)
//            {
//                fAlpha = (float)Math.PI + (float)Math.Atan(fLengthZ / fLengthX);
//            }
//            // 3rd Quadrant
//            //  180 (PI) < Alpha < 270 (3/2*PI)
//            else if (fLengthX <= 0f && fLengthZ <= 0)
//            {
//                fAlpha = (float)Math.PI + (float)Math.Atan(fLengthZ / fLengthX);
//            }
//            // 4th Quadrant
//            //  270 (3/2*PI) < Alpha < 360 (2*PI)
//            else if (fLengthX >= 0f && fLengthZ <= 0)
//            {
//                fAlpha = (float)(2 * Math.PI) + (float)Math.Atan(fLengthZ / fLengthX);
//            }
//            else // Exception
//            {
//                fAlpha = 0f;
//            }

//            int iHingeDiameter = 8;
//            int iX = (int)Math.Abs((float)Math.Cos(fAlpha) * iHingeDiameter);
//            int iY = (int)Math.Abs((float)Math.Sin(fAlpha) * iHingeDiameter);

            
//            // Centre Points
            
//            Point pNode1Centre = new Point();
//            Point pNode2Centre = new Point();


//            /*
//            1-----2
//            */
//            // Alpha = 0 (0)
//            if (fLengthX > 0 && fLengthZ == 0)
//            {
//                pNode1Centre.X = pNode1.X + iHingeDiameter / 2;
//                pNode1Centre.Y = pNode1.Y;

//                pNode2Centre.X = pNode2.X - iHingeDiameter / 2;
//                pNode2Centre.Y = pNode2.Y;
//            }
//            // Diagonal line Left->Right Slope descending
//            /*
//                1
//                 \ 
//                  \
//                   2
//            */
//            // 0 < Alpha < 90 (PI/2) 
//            else if ((pNode1.X < pNode2.X) && (pNode1.Y < pNode2.Y))
//            {
//                pNode1Centre.X = pNode1.X + iX;
//                pNode1Centre.Y = pNode1.Y + iY;

//                pNode2Centre.X = pNode2.X - iX;
//                pNode2Centre.Y = pNode2.Y - iY;
//            }
//            /*
//             1
//             |
//             |
//             2
//            */
//            // Alpha = 90 (PI/2)
//            else if (fLengthX == 0 && fLengthZ > 0)
//            {
//                pNode1Centre.X = pNode1.X;
//                pNode1Centre.Y = pNode1.Y + iHingeDiameter / 2;

//                pNode2Centre.X = pNode2.X;
//                pNode2Centre.Y = pNode2.Y - iHingeDiameter / 2;
//            }
//            /* 
//                 1
//                / 
//               /
//              2
//            */
//            //  90 (PI/2) < Alpha < 180 (PI)
//            else if ((pNode2.X < pNode1.X) && (pNode2.Y > pNode1.Y))
//            {
//                pNode1Centre.X = pNode1.X - iX;
//                pNode1Centre.Y = pNode1.Y + +iY;

//                pNode2Centre.X = pNode2.X + iX;
//                pNode2Centre.Y = pNode2.Y - iY;
//            }
//            /*
//            2-----1
//           */
//            // Alpha = 180 (PI)
//            if (fLengthX < 0 && fLengthZ == 0)
//            {
//                pNode1Centre.X = pNode1.X - iHingeDiameter / 2;
//                pNode1Centre.Y = pNode1.Y;

//                pNode2Centre.X = pNode2.X + iHingeDiameter / 2;
//                pNode2Centre.Y = pNode2.Y;
//            }
//            // Diagonal line Left->Right Slope descending
//            /* 
//                2
//                 \ 
//                  \
//                   1
//            */
//            //  180 (PI) < Alpha < 270 (3/2*PI)
//            else if ((pNode2.X < pNode1.X) && (pNode2.Y < pNode1.Y))
//            {
//                pNode1Centre.X = pNode1.X - iX;
//                pNode1Centre.Y = pNode1.Y - iY;

//                pNode2Centre.X = pNode2.X + iX;
//                pNode2Centre.Y = pNode2.Y + iY;
//            }
//            /*
//             2
//             |
//             |
//             1
//            */
//            // Alpha = 270 (3/2*PI)
//            else if (fLengthX == 0 && fLengthZ < 0)
//            {
//                pNode1Centre.X = pNode1.X;
//                pNode1Centre.Y = pNode1.Y - iHingeDiameter / 2;

//                pNode2Centre.X = pNode2.X;
//                pNode2Centre.Y = pNode2.Y + iHingeDiameter / 2;
//            }
//            // Diagonal line Left->Right Slope ascending
//            /*
//                  2
//                 / 
//                /
//               1
//            */
//            //  270 (3/2*PI) < Alpha < 360 (2*PI)
//            else if ((pNode1.X < pNode2.X) && (pNode1.Y > pNode2.Y))
//            {
//                pNode1Centre.X = pNode1.X + iX;
//                pNode1Centre.Y = pNode1.Y - iY;

//                pNode2Centre.X = pNode2.X - iX;
//                pNode2Centre.Y = pNode2.Y + iY;
//            }
//            else // Exception
//            {
//                pNode1Centre.X = pNode1.X;
//                pNode1Centre.Y = pNode1.Y;

//                pNode2Centre.X = pNode2.X;
//                pNode2Centre.Y = pNode2.Y;
//            }


//            // Draw Releases
//            // 0-5  // UX, UY, UZ, RX, RY, RZ

//            if (iHinge.m_bRestrain[0])  // Local UX / Displacement / Draw line pair parallel to the local x-Axis
//            {
//                if (!pNode1.IsEmpty)
//                    DrawStraigthRel(g, pNode1Centre, fAlpha);
//                if (!pNode2.IsEmpty)
//                    DrawStraigthRel(g, pNode2Centre, fAlpha);
//            }

//            if (iHinge.m_bRestrain[2])  // Local UZ / Displacement / Draw line pair transverse to the local x-Axis 
//            {
//                if (!pNode1.IsEmpty)

//                    DrawStraigthRel(g, pNode1Centre, fAlpha + (float)Math.PI / 2f);
//                if (!pNode2.IsEmpty)
//                    DrawStraigthRel(g, pNode2Centre, fAlpha + (float)Math.PI / 2f);
//            }

//            if (iHinge.m_bRestrain[4])  // Local RY - Rotation / Draw Circle
//            {
//                if (!pNode1.IsEmpty)
//                    DrawCircleHinge(g, iHingeDiameter, pNode1Centre);
//                if (!pNode2.IsEmpty)
//                    DrawCircleHinge(g, iHingeDiameter, pNode2Centre);
//            }
//        }

//        public void DrawNForce(Graphics g, Pen p, CNForce force)
//        {
//            p.StartCap = LineCap.ArrowAnchor;
//            p.EndCap   = LineCap.Flat;
//            p.Color = Color.Red;
//            p.Width = 6;

//            // Vykresli kladnou silu ve smeru X smerem doprava
//            if (force.m_fValueX > 0.0f)
//            {
//                g.DrawLine(p,
//                force.m_iNode.m_fCoord_X,
//                force.m_iNode.m_fCoord_Z,
//                force.m_iNode.m_fCoord_X - force.m_fValueX,
//                force.m_iNode.m_fCoord_Z);
//            }

//            // Vykresli zapornou silu ve smeru X smerem doleva
//            if (force.m_fValueX < 0.0f)
//            {
//                g.DrawLine(p,
//                force.m_iNode.m_fCoord_X,
//                force.m_iNode.m_fCoord_Z,
//                force.m_iNode.m_fCoord_X - force.m_fValueX,
//                force.m_iNode.m_fCoord_Z);
//            }

//            // Vykresli kladnou silu ve smeru Z - smerem nahor
//            if (force.m_fValueZ > 0.0f)
//            {
//                g.DrawLine(p,
//                force.m_iNode.m_fCoord_X,
//                force.m_iNode.m_fCoord_Z,
//                force.m_iNode.m_fCoord_X,
//                force.m_iNode.m_fCoord_Z + force.m_fValueZ);
//            }

//            // Vykresli zapornou silu ve smeru Z - smerem nadol
//            if (force.m_fValueZ < 0.0f)
//            {
//                g.DrawLine(p,
//                force.m_iNode.m_fCoord_X,
//                force.m_iNode.m_fCoord_Z,
//                force.m_iNode.m_fCoord_X,
//                force.m_iNode.m_fCoord_Z + force.m_fValueZ);
//            }
//        }


















//        // Draw Numbering / coordinates / values

//        public void DrawNode_Numbering(Graphics g, CNode node)
//        {
//            Font  font = new Font("Courier new", 10);
//            Brush brush = new SolidBrush(Color.Aquamarine);

//            g.DrawString(node.m_iNode_ID.ToString(), font, brush, (int)node.m_fCoord_X - 20, (int)node.m_fCoord_Z - 20);
//        }

//        public void DrawNode_Coordinates(Graphics g, CNode node)
//        {
//            Font  font = new Font("Courier new", 8);
//            Brush brush = new SolidBrush(Color.Azure);

//            g.DrawString("[" + (int)node.m_fCoord_X + ","
//                             + (int)node.m_fCoord_Y + ","
//                             + (int)node.m_fCoord_Z + "]",
//                            font,
//                            brush,
//                            (int)node.m_fCoord_X + 20, (int)node.m_fCoord_Z - 20);
//        }

//        public void DrawLine_Numbering(Graphics g, CLine line)
//        {
//            Font font = new Font("Courier new", 10);
//            Brush brush = new SolidBrush(Color.Crimson);

//            int iLineCentreCoord_X = Math.Min((int)line.m_iNode2.m_fCoord_X , (int)line.m_iNode1.m_fCoord_X) + (Math.Abs((int)line.m_iNode2.m_fCoord_X - (int)line.m_iNode1.m_fCoord_X) / 2);
//            int iLineCentreCoord_Z = Math.Min((int)line.m_iNode2.m_fCoord_Z, (int)line.m_iNode1.m_fCoord_Z) + (Math.Abs((int)line.m_iNode2.m_fCoord_Z - (int)line.m_iNode1.m_fCoord_Z) / 2);

//            // Same Z coordinates - horizontal line
//            if (Math.Abs(line.m_iNode2.m_fCoord_Z - line.m_iNode1.m_fCoord_Z) < 5)
//                g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X, iLineCentreCoord_Z - 20);
//            // Same X coordinates - vertical line
//            else if (Math.Abs(line.m_iNode2.m_fCoord_X - line.m_iNode1.m_fCoord_X) < 5)
//                g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X + 10 , iLineCentreCoord_Z);
//            // Diagonal line Left->Right Slope descending
//            else if (((line.m_iNode1.m_fCoord_X < line.m_iNode2.m_fCoord_X) && (line.m_iNode1.m_fCoord_Z < line.m_iNode2.m_fCoord_Z)) ||
//                     ((line.m_iNode2.m_fCoord_X < line.m_iNode1.m_fCoord_X) && (line.m_iNode2.m_fCoord_Z < line.m_iNode1.m_fCoord_Z)))
//                g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X + 10, iLineCentreCoord_Z - 10);
//            // Diagonal line Left->Right Slope ascending
//            else if (((line.m_iNode1.m_fCoord_X < line.m_iNode2.m_fCoord_X) && (line.m_iNode1.m_fCoord_Z > line.m_iNode2.m_fCoord_Z)) ||
//                     ((line.m_iNode2.m_fCoord_X < line.m_iNode1.m_fCoord_X) && (line.m_iNode2.m_fCoord_Z > line.m_iNode1.m_fCoord_Z)))
//                g.DrawString(line.m_iLine_ID.ToString(), font, brush, iLineCentreCoord_X + 10, iLineCentreCoord_Z + 10);
//            else
//            {
//                // Exception
//            }
//        }

//        public void DrawSupport_Numbering(Graphics g, CNSupport support)
//        {
//            Font font = new Font("Courier new", 10);
//            Brush brush = new SolidBrush(Color.Yellow);

//            g.DrawString(support.m_iSupport_ID.ToString(), font, brush, support.m_iNode.m_fCoord_X - 20, support.m_iNode.m_fCoord_Z + 10);
//        }

//        public void DrawForce_Values(Graphics g, CNForce force)
//        {
//            Font font = new Font("Courier new", 8);
//            Brush brush = new SolidBrush(Color.DarkOrange);

//            // Force in X direction
//            if (force.m_fValueX > 0)
//                g.DrawString("Fx = " + Math.Round(force.m_fValueX, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X - (int)force.m_fValueX - 80, (int)force.m_iNode.m_fCoord_Z + 10);
//            else if (force.m_fValueX < 0)
//                g.DrawString("Fx = " + Math.Round(force.m_fValueX, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X - (int)force.m_fValueX + 10, (int)force.m_iNode.m_fCoord_Z + 10);
//            else
//            {
//                // Exception
//            }

//            // Force in Z direction
//            if (force.m_fValueZ > 0)
//                g.DrawString("Fz = " + Math.Round(force.m_fValueZ, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X + 10, (int)force.m_iNode.m_fCoord_Z + (int)force.m_fValueZ + 10);
//            else if (force.m_fValueZ < 0)
//                g.DrawString("Fz = " + Math.Round(force.m_fValueZ, 2) + " kN", font, brush, (int)force.m_iNode.m_fCoord_X + 10, (int)force.m_iNode.m_fCoord_Z + (int)force.m_fValueZ - 10);
//            else
//            {
//                // Exception
//            }
//        }

//        // Draw Global Coordinates System Symbol

//        public void DrawGCS(Graphics g, int iXmin, int iZmax)
//        {
//            Font font = new Font("Courier new", 12);
//            Brush brushX = new SolidBrush(Color.Red);
//            Brush brushY = new SolidBrush(Color.LightGreen);
//            Brush brushZ = new SolidBrush(Color.Blue);
//            Pen pen = new Pen(Color.PeachPuff);
//            pen.StartCap = LineCap.Round;
//            pen.EndCap = LineCap.ArrowAnchor;
//            pen.Width = 4;

//            // Lines
//            // X-Axis
//            pen.Color = Color.Red;
//            g.DrawLine(pen, iXmin + 20, iZmax - 20, iXmin + 60, iZmax -20);
//            g.DrawString("X", font, brushX, iXmin + 65, iZmax - 20);
//            // Y-Axis
//            pen.Color = Color.LightGreen;
//            g.DrawLine(pen, iXmin + 20, iZmax -20, iXmin + 50, iZmax - 50);
//            g.DrawString("Y", font, brushY, iXmin + 55, iZmax - 55);
//            // Z-Axis
//            pen.Color = Color.Blue;
//            g.DrawLine(pen, iXmin + 20, iZmax - 20, iXmin + 20, iZmax - 60);
//            g.DrawString("Z", font, brushZ, iXmin + 20, iZmax - 65);
//        }

//        // Temporary
//        // Draw Cross-section in 2D

//        public void DrawCrSc2D(Graphics g, CCrSc_0_50 objCrSc)
//        { 
//           Brush b  = new SolidBrush(Color.Yellow);
//           Pen p = new Pen(Color.Cornsilk,1);

//           int iSize = 2; // Size of point

//           // Points
//           for (short i = 0; i < objCrSc.ITotNoPoints; i++)
//           {

//               g.FillRectangle(b,
//               objCrSc.m_CrScPoint[i, 0] - (iSize / 2),
//               objCrSc.m_CrScPoint[i, 1] - (iSize / 2),
//               iSize,
//               iSize);
//           }

//           // Change Color
//           p.Color = Color.DarkCyan;

//           // Liness
//           for (short i = 0; i < objCrSc.ITotNoPoints - 1; i++)
//           {
//               g.DrawLine(p,
//               objCrSc.m_CrScPoint[i, 0],
//               objCrSc.m_CrScPoint[i, 1],
//               objCrSc.m_CrScPoint[i + 1, 0],
//               objCrSc.m_CrScPoint[i + 1, 1]);
//           }
//        }

//        ///////////////////////////////////////////////////////////////////////////////////
//        // Transformation Matrices 
//        ///////////////////////////////////////////////////////////////////////////////////

//        // Transformation matrix [A] = [AX] * [AY] * [AAlpha]
//        float[,] A_RH(float fAlpha, float fx, float fy)
//        {
//            return new float[3, 3] 
//    {
//     {(float)Math.Cos(fAlpha), - (float)Math.Sin(fAlpha), fx},
//     {(float)Math.Sin(fAlpha),   (float)Math.Cos(fAlpha), fy},
//     {                     0f,                        0f, 1f}
//    };
//        }

//        // Transformation of x coordinate
//        float[,] AX(float fx)
//        {
//            return new float[3, 3] 
//    {
//     {0f, 0f, fx},
//     {0f, 0f, 0f},
//     {0f, 0f, 1f}
//    };
//        }

//        // Transformation of y coordinate
//        float[,] AY(float fy)
//        {
//            return new float[3, 3] 
//    {
//     {0f, 0f, 0f},
//     {0f, 0f, fy},
//     {0f, 0f, 1f}
//    };
//        }

//        // Transformation of rotation degree
//        float[,] AAlpha_RH(float fAlpha)
//        {
//            return new float[3, 3] 
//    {
//     {(float)Math.Cos(fAlpha), - (float)Math.Sin(fAlpha), 0f},
//     {(float)Math.Sin(fAlpha),   (float)Math.Cos(fAlpha), 0f},
//     {                     0f,                        0f, 1f}
//    };
//        }

//        // Return Node Coordinates Matrix in Global Coordinate System
//        float[] fPointG_RH(float fx, float fy, float fx0, float fy0, float fAlpha)
//        {
//            return new float[] 
//    {
//     fx*(float)Math.Cos(fAlpha) - fy*(float)Math.Sin(fAlpha) + fx0,
//     fx*(float)Math.Sin(fAlpha) + fy*(float)Math.Cos(fAlpha) + fy0,
//                                                                1f
//    };
//        }

//        int iGetPointG_RH_X(int fx, int fy, float fAlpha, int fx0)
//        {
//            return (int) (fx * (float)Math.Cos(fAlpha) - fy * (float)Math.Sin(fAlpha) + fx0);
//        }

//        int iGetPointG_RH_Y(int fx, int fy, float fAlpha, int fy0)
//        {
//            return (int) (fx * (float)Math.Sin(fAlpha) + fy * (float)Math.Cos(fAlpha) + fy0);
//        }

//        //http://int21h.ic.cz/?id=53
//        float[] fPointL;
//        float[] fPointG;

//        void DrawStraigthRel(Graphics g, Point pCentre, float fAlpha)
//        {
//            // Length of line
//            int iLength = 10;
//            // InterSpace / gap
//            int iGap = 4;

//            Point pL1_S = new Point();
//            Point pL1_E = new Point();
//            Point pL2_S = new Point();
//            Point pL2_E = new Point();
            
//            // Coordinates in LCS - Cnetre point local coordinates are [0,0]
//            pL1_S.X = -iLength / 2;
//            pL1_S.Y = -iGap / 2;

//            pL1_E.X = iLength / 2;
//            pL1_E.Y = -iGap / 2;

//            pL2_S.X = -iLength / 2;
//            pL2_S.Y = iGap / 2;

//            pL2_E.X = iLength / 2;
//            pL2_E.Y = iGap / 2;

//            // Transformation to GCS
//            // Temporary
//            int pL1_S_X = iGetPointG_RH_X(pL1_S.X, pL1_S.Y, fAlpha, pCentre.X);
//            int pL1_S_Y = iGetPointG_RH_Y(pL1_S.X, pL1_S.Y, fAlpha, pCentre.Y);
 
//            int pL1_E_X = iGetPointG_RH_X(pL1_E.X, pL1_E.Y, fAlpha, pCentre.X);
//            int pL1_E_Y = iGetPointG_RH_Y(pL1_E.X, pL1_E.Y, fAlpha, pCentre.Y);

//            int pL2_S_X = iGetPointG_RH_X(pL2_S.X, pL2_S.Y, fAlpha, pCentre.X);
//            int pL2_S_Y = iGetPointG_RH_Y(pL2_S.X, pL2_S.Y, fAlpha, pCentre.Y);

//            int pL2_E_X = iGetPointG_RH_X(pL2_E.X, pL2_E.Y, fAlpha, pCentre.X);
//            int pL2_E_Y = iGetPointG_RH_Y(pL2_E.X, pL2_E.Y, fAlpha, pCentre.Y);

//            // Coordinates in GCS

//            pL1_S.X = pL1_S_X;
//            pL1_S.Y = pL1_S_Y;

//            pL1_E.X = pL1_E_X;
//            pL1_E.Y = pL1_E_Y;

//            pL2_S.X = pL2_S_X;
//            pL2_S.Y = pL2_S_Y;

//            pL2_E.X = pL2_E_X;
//            pL2_E.Y = pL2_E_Y;

//            // Draw Lines
//            // Create pen
//            Pen p = new Pen(Color.Azure);
//            p.DashStyle = DashStyle.Solid;
//            //p.Color = Color.Azure;
//            p.Width = 1;
            
//            g.DrawLine(p, pL1_S, pL1_E);
//            g.DrawLine(p, pL2_S, pL2_E);
//        }









//        // TEMPORARY

//        ///////////////////////////////////////////////////////////////////////////////////
 
//        private float ftemp(float a)
//        { return a*a; }

//        private float ftemp(float a, float b)
//        { return a + b; }

































//        ///////////////////////////////////////////////////////////////////////////////////

//        // Menu Events
   
//        private void nodesToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (nodesToolStripMenuItem.Checked == true)
//            {
//                nodesToolStripMenuItem.Checked = false;
//                    bDisplayNodes = false;
//            }
//            else
//            {
//                nodesToolStripMenuItem.Checked = true;
//                bDisplayNodes = true;
//            }
//            Refresh();
//        }

//        private void linesToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (linesToolStripMenuItem.Checked == true)
//            {
//                linesToolStripMenuItem.Checked = false;
//                bDisplayLines = false;
//            }
//            else
//            {
//                linesToolStripMenuItem.Checked = true;
//                bDisplayLines = true;
//            }
//            Refresh();
//        }

//        private void supportsToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (supportsToolStripMenuItem.Checked == true)
//            {
//                supportsToolStripMenuItem.Checked = false;
//                bDisplayNSupports = false;
//            }
//            else
//            {
//                supportsToolStripMenuItem.Checked = true;
//                bDisplayNSupports = true;
//            }
//            Refresh();
//        }

//        private void membReltoolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (membReltoolStripMenuItem.Checked == true)
//            {
//                membReltoolStripMenuItem.Checked = false;
//                bDisplayMembRel = false;
//            }
//            else
//            {
//                membReltoolStripMenuItem.Checked = true;
//                bDisplayMembRel = true;
//            }
//            Refresh();
//        }

//        private void forcesToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (forcesToolStripMenuItem.Checked == true)
//            {
//                forcesToolStripMenuItem.Checked = false;
//                bDisplayNForces = false;
//            }
//            else
//            {
//                forcesToolStripMenuItem.Checked = true;
//                bDisplayNForces = true;
//            }
//            Refresh();
//        }

//        private void numberingToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (numberingToolStripMenuItem.Checked == true)
//            {
//                numberingToolStripMenuItem.Checked = false;
//                bDisplayNodesNumbering = false;
//            }
//            else
//            {
//                numberingToolStripMenuItem.Checked = true;
//                bDisplayNodesNumbering = true;
//            }
//            Refresh();
//        }

//        private void coordinatesToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (coordinatesToolStripMenuItem.Checked == true)
//            {
//                coordinatesToolStripMenuItem.Checked = false;
//                bDisplayNodesCoordinates = false;
//            }
//            else
//            {
//                coordinatesToolStripMenuItem.Checked = true;
//                bDisplayNodesCoordinates = true;
//            }
//            Refresh();
//        }

//        private void numberingToolStripMenuItem1_Click(object sender, EventArgs e)
//        {
//            if (numberingToolStripMenuItem1.Checked == true)
//            {
//                numberingToolStripMenuItem1.Checked = false;
//                bDisplayLinesNumbering = false;
//            }
//            else
//            {
//                numberingToolStripMenuItem1.Checked = true;
//                bDisplayLinesNumbering = true;
//            }
//            Refresh();
//        }

//        private void valuesToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (valuesToolStripMenuItem.Checked == true)
//            {
//                valuesToolStripMenuItem.Checked = false;
//                bDisplayNForcesValues = false;
//            }
//            else
//            {
//                valuesToolStripMenuItem.Checked = true;
//                bDisplayNForcesValues = true;
//            }
//            Refresh();
//        }

//        private void gCSToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (bDisplayGCS == true)
//            {
//                bDisplayGCS = false;
//            }
//            else
//            {
//                bDisplayGCS = true;
//            }
//            Refresh();
//        }

//        // Supports
//        private void numberingToolStripMenuItem2_Click(object sender, EventArgs e)
//        {
//            if (numberingToolStripMenuItem2.Checked == true)
//            {
//                numberingToolStripMenuItem2.Checked = false;
//                bDisplaySupportsNumbering = false;
//            }
//            else
//            {
//                numberingToolStripMenuItem2.Checked = true;
//                bDisplaySupportsNumbering = true;
//            }
//            Refresh();
//        }

//        // Member Releases
//        private void numberingToolStripMenuItem3_Click(object sender, EventArgs e)
//        {
//            if (numberingToolStripMenuItem3.Checked == true)
//            {
//                numberingToolStripMenuItem3.Checked = false;
//                bDisplayMembRelsNumbering = false;
//            }
//            else
//            {
//                numberingToolStripMenuItem3.Checked = true;
//                bDisplayMembRelsNumbering = true;
//            }
//            Refresh();
//        }

//        private void drawTest1ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest1 objCTest1 = new CTest1();

//            DeleteArrays();

//            /*
//            // Allocate Memory
//            arrNodes = new BaseClasses.CNode[objCTest1.arrNodes.Length];
//            arrLines = new BaseClasses.CMember[objCTest1.arrMembers.Length];
//            arrSupports = new BaseClasses.CNSupport[objCTest1.arrSupports.Length];
//            arrForce = new BaseClasses.CNLoad[objCTest1.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest1.arrNodes;
//            arrLines = objCTest1.arrMembers;
//            arrSupports = objCTest1.arrSupports;
//            arrForce = objCTest1.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//            */
//        }

//        private void drawTest2ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest2 objCTest2 = new CTest2();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest2.arrNodes.Length];
//            arrLines = new CLine[objCTest2.arrLines.Length];
//            arrSupports = new CNSupport[objCTest2.arrSupports.Length];
//            arrForce = new CNForce[objCTest2.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest2.arrNodes;
//            arrLines = objCTest2.arrLines;
//            arrSupports = objCTest2.arrSupports;
//            arrForce = objCTest2.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest3ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest3 objCTest3 = new CTest3();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest3.arrNodes.Length];
//            arrLines = new CLine[objCTest3.arrLines.Length];
//            arrSupports = new CNSupport[objCTest3.arrSupports.Length];
//            arrForce = new CNForce[objCTest3.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest3.arrNodes;
//            arrLines = objCTest3.arrLines;
//            arrSupports = objCTest3.arrSupports;
//            arrForce = objCTest3.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }
//        private void DeleteArrays()
//        {
//            // Release Memory
//            arrNodes = null;
//            arrLines = null;
//            arrSupports = null;
//            arrForce = null;
//        }

//        private void drawTest4ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest4 objCTest4 = new CTest4();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest4.arrNodes.Length];
//            arrLines = new CLine[objCTest4.arrLines.Length];
//            arrSupports = new CNSupport[objCTest4.arrSupports.Length];
//            arrForce = new CNForce[objCTest4.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest4.arrNodes;
//            arrLines = objCTest4.arrLines;
//            arrSupports = objCTest4.arrSupports;
//            arrForce = objCTest4.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest5ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest5 objCTest5 = new CTest5();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest5.arrNodes.Length];
//            arrLines = new CLine[objCTest5.arrLines.Length];
//            arrSupports = new CNSupport[objCTest5.arrSupports.Length];
//            arrForce = new CNForce[objCTest5.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest5.arrNodes;
//            arrLines = objCTest5.arrLines;
//            arrSupports = objCTest5.arrSupports;
//            arrForce = objCTest5.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest6ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest6 objCTest6 = new CTest6();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest6.arrNodes.Length];
//            arrLines = new CLine[objCTest6.arrLines.Length];
//            arrSupports = new CNSupport[objCTest6.arrSupports.Length];
//            arrForce = new CNForce[objCTest6.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest6.arrNodes;
//            arrLines = objCTest6.arrLines;
//            arrSupports = objCTest6.arrSupports;
//            arrForce = objCTest6.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest7ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest7 objCTest7 = new CTest7();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest7.arrNodes.Length];
//            arrLines = new CLine[objCTest7.arrLines.Length];
//            arrSupports = new CNSupport[objCTest7.arrSupports.Length];
//            arrForce = new CNForce[objCTest7.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest7.arrNodes;
//            arrLines = objCTest7.arrLines;
//            arrSupports = objCTest7.arrSupports;
//            arrForce = objCTest7.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest8ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest8 objCTest8 = new CTest8();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest8.arrNodes.Length];
//            arrLines = new CLine[objCTest8.arrLines.Length];
//            arrSupports = new CNSupport[objCTest8.arrSupports.Length];
//            arrForce = new CNForce[objCTest8.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest8.arrNodes;
//            arrLines = objCTest8.arrLines;
//            arrSupports = objCTest8.arrSupports;
//            arrForce = objCTest8.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest9ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest9 objCTest9 = new CTest9();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest9.arrNodes.Length];
//            arrLines = new CLine[objCTest9.arrLines.Length];
//            arrSupports = new CNSupport[objCTest9.arrSupports.Length];
//            arrForce = new CNForce[objCTest9.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest9.arrNodes;
//            arrLines = objCTest9.arrLines;
//            arrSupports = objCTest9.arrSupports;
//            arrForce = objCTest9.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }

//        private void drawTest10ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CTest10 objCTest10 = new CTest10();

//            DeleteArrays();

//            // Allocate Memory
//            arrNodes = new CNode[objCTest10.arrNodes.Length];
//            arrLines = new CLine[objCTest10.arrLines.Length];
//            arrSupports = new CNSupport[objCTest10.arrSupports.Length];
//            arrForce = new CNForce[objCTest10.arrForces.Length];

//            // Fill Data
//            arrNodes = objCTest10.arrNodes;
//            arrLines = objCTest10.arrLines;
//            arrSupports = objCTest10.arrSupports;
//            arrForce = objCTest10.arrForces;

//            // Center drawing of structure in window
//            if (arrNodes != null && arrLines != null) // if some relevant data to centre and re-draw exist
//            {
//                GetStrucSize();
//                ResizeDrawing();
//                GetStrucSize();
//                CentreDrawing();
//                Refresh();
//            }
//        }
//     }
//}