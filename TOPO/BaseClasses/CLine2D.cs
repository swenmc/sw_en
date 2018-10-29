using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class CLine2D : IComparable
    {
        //-------------------------------------------------------------------------
        private Point MP1;
        private Point MP2;
        private double MX1;
        private double MX2;
        private double MY1;
        private double MY2;

        //-------------------------------------------------------------------------
        public Point P1
        {
            get
            {
                return MP1;
            }

            set
            {
                MP1 = value;
            }
        }

        public Point P2
        {
            get
            {
                return MP2;
            }

            set
            {
                MP2 = value;
            }
        }

        public double X1
        {
            get
            {
                return MX1;
            }

            set
            {
                MX1 = value;
            }
        }

        public double X2
        {
            get
            {
                return MX2;
            }

            set
            {
                MX2 = value;
            }
        }

        public double Y1
        {
            get
            {
                return MY1;
            }

            set
            {
                MY1 = value;
            }
        }

        public double Y2
        {
            get
            {
                return MY2;
            }

            set
            {
                MY2 = value;
            }
        }

        //-------------------------------------------------------------------------
        //-------------------------------------------------------------------------
        public CLine2D(Point p1, Point p2)
        {
            MP1 = p1;
            MP2 = p2;

            MX1 = p1.X;
            MX2 = p2.X;
            MY1 = p1.Y;
            MY2 = p2.Y;
        }
        public CLine2D(float X1, float Y1, float X2, float Y2)
        {
            MX1 = X1;
            MX2 = X2;
            MY1 = Y1;
            MY2 = Y2;
        }
        public CLine2D(double X1, double Y1, double X2, double Y2)
        {
            MX1 = X1;
            MX2 = X2;
            MY1 = Y1;
            MY2 = Y2;
        }

        public bool IntersectsLine(CLine2D comparedLine)
        {
            if ((X2 == comparedLine.X1) && (Y2 == comparedLine.Y1)) return false;
            if ((X1 == comparedLine.X2) && (Y1 == comparedLine.Y2)) return false;
            
            double firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;

            firstLineSlopeX = X2 - X1;
            firstLineSlopeY = Y2 - Y1;

            secondLineSlopeX = comparedLine.X2 - comparedLine.X1;
            secondLineSlopeY = comparedLine.Y2 - comparedLine.Y1;

            double s, t;
            s = (-firstLineSlopeY * (X1 - comparedLine.X1) + firstLineSlopeX * (Y1 - comparedLine.Y1)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
            t = (secondLineSlopeX * (Y1 - comparedLine.Y1) - secondLineSlopeY * (X1 - comparedLine.X1)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return true;
            }

            return false; // No collision 
        }

        int IComparable.CompareTo(object obj)
        {
            //return Y1.GetHashCode();
            CLine2D o1 = this;
            CLine2D o2 = (CLine2D)obj;
            if (o1.Y1< o2.Y1)
            {
                return -1;
            }
            else if (o1.Y1 > o2.Y2)
            {
                return 1;
            }
            else
            {
                if (o1.Y2 < o2.Y2)
                {
                    return -1;
                }
                else if (o1.Y2 > o2.Y2)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double GetLineLength()
        {
            return Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2);
        }

        public void MirrorYCoordinates()
        {
            this.MY1 *= -1;
            this.MY2 *= -1;
        }
        public void MirrorXCoordinates()
        {
            this.MX1 *= -1;
            this.MX2 *= -1;
        }

        public void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
          P1 = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (P1.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (P1.Y - minY));
          P2 = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (P2.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (P2.Y - minY));

          MX1 = P1.X;
          MY1 = P1.Y;

          MX2 = P2.X;
          MY2 = P2.Y;
        }
    }
}
