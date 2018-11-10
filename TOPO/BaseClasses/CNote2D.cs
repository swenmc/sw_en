using System;
using System.Windows;

namespace BaseClasses
{
    public class CNote2D
    {
        private Point MNotePoint;
        private string MText;
        private double MDistanceX;
        private double MDistanceY;
        private bool MDrawArrow;
        private Point MArrowPoint1;
        private Point MArrowPoint2;
        private Point MRefPoint;

        public Point NotePoint
        {
            get
            {
                return MNotePoint;
            }

            set
            {
                MNotePoint = value;
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
        public CNote2D(Point notePoint, string text, double distanceX, double distanceY, bool drawArrow, Point arrowPoint1, Point arrowPoint2, Point refPoint)
        {
            MNotePoint = notePoint;
            MText = text;
            MDistanceX = distanceX;
            MDistanceY = distanceY;
            MDrawArrow = drawArrow;
            MArrowPoint1 = arrowPoint1;
            MArrowPoint2 = arrowPoint2;
            MRefPoint = RefPoint;
        }


        public void MirrorYCoordinates()
        {
            MNotePoint.Y *= -1;
            MRefPoint.Y *= -1;
            MArrowPoint1.Y *= -1;
            MArrowPoint2.Y *= -1;
        }

        public void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            MNotePoint = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MNotePoint.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MNotePoint.Y - minY));
            MRefPoint = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MRefPoint.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MRefPoint.Y - minY));
            MArrowPoint1 = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MArrowPoint1.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MArrowPoint1.Y - minY));
            MArrowPoint2 = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (MArrowPoint2.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (MArrowPoint2.Y - minY));
        }

    }
}
