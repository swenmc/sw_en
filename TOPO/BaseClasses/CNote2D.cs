using System;
using System.Windows;

namespace BaseClasses
{
    public class CNote2D
    {
        private Point MNoteTextPoint;
        private string MText;
        private double MDistanceX;
        private double MDistanceY;
        private bool MDrawArrow;
        private bool MDrawLineUnderText;
        private Point MArrowPoint1;
        private Point MArrowPoint2;
        private Point MLineEndPoint;
        private Point MRefPoint;

        public Point NoteTextPoint
        {
            get
            {
                return MNoteTextPoint;
            }

            set
            {
                MNoteTextPoint = value;
            }
        }

        public double DistanceX
        {
            get
            {
                return MDistanceX;
            }

            set
            {
                MDistanceX = value;
            }
        }

        public double DistanceY
        {
            get
            {
                return MDistanceY;
            }

            set
            {
                MDistanceY = value;
            }
        }

        public bool DrawArrow
        {
            get
            {
                return MDrawArrow;
            }

            set
            {
                MDrawArrow = value;
            }
        }

        public bool DrawLineUnderText
        {
            get
            {
                return MDrawLineUnderText;
            }

            set
            {
                MDrawLineUnderText = value;
            }
        }

        public Point ArrowPoint1
        {
            get
            {
                return MArrowPoint1;
            }

            set
            {
                MArrowPoint1 = value;
            }
        }

        public Point ArrowPoint2
        {
            get
            {
                return MArrowPoint2;
            }

            set
            {
                MArrowPoint2 = value;
            }
        }

        public Point LineEndPoint
        {
            get
            {
                return MLineEndPoint;
            }

            set
            {
                MLineEndPoint = value;
            }
        }

        public string Text
        {
            get
            {
                return MText;
            }

            set
            {
                MText = value;
            }
        }

        public Point RefPoint
        {
            get
            {
                return MRefPoint;
            }

            set
            {
                MRefPoint = value;
            }
        }

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CNote2D() { }
        public CNote2D(Point noteTextPoint, string text, double distanceX, double distanceY, bool drawArrow, Point arrowPoint1, Point arrowPoint2, Point refPoint, bool bDrawLineUnderText = false)
        {
            MNoteTextPoint = noteTextPoint;
            MText = text;
            MDistanceX = distanceX; // TO Ondrej - co nastavuju tieto dve hodnoty ? Offset stredu textu od bodu?
            MDistanceY = distanceY; // TO Ondrej - co nastavuju tieto dve hodnoty ? Offset stredu textu od bodu?
            MDrawArrow = drawArrow;
            MArrowPoint1 = arrowPoint1;
            MArrowPoint2 = arrowPoint2;
            MRefPoint = RefPoint;
            MDrawLineUnderText = bDrawLineUnderText;
        }

        public void MirrorYCoordinates()
        {
            MNoteTextPoint.Y *= -1;
            MRefPoint.Y *= -1;
            MArrowPoint1.Y *= -1;
            MArrowPoint2.Y *= -1;
            MLineEndPoint.Y *= -1;
        }

        public void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            MNoteTextPoint = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MNoteTextPoint.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MNoteTextPoint.Y - minY));
            MRefPoint = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MRefPoint.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MRefPoint.Y - minY));
            MArrowPoint1 = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MArrowPoint1.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MArrowPoint1.Y - minY));
            MArrowPoint2 = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MArrowPoint2.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MArrowPoint2.Y - minY));
            MLineEndPoint = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MLineEndPoint.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MLineEndPoint.Y - minY));
        }
    }
}
