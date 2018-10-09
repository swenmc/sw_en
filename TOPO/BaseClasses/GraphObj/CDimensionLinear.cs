using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.GraphObj
{
    public class CDimensionLinear : CDimension
    {
        private Point m_controlPointStart;

        public Point ControlPointStart
        {
            get
            {
                return m_controlPointStart;
            }

            set
            {
                m_controlPointStart = value;
            }
        }

        private Point m_controlPointEnd;

        public Point ControlPointEnd
        {
            get
            {
                return m_controlPointEnd;
            }

            set
            {
                m_controlPointEnd = value;
            }
        }

        private bool m_bIsTextAboveLineBetweenExtensionLines;

        public bool IsTextAboveLineBetweenExtensionLines
        {
            get
            {
                return m_bIsTextAboveLineBetweenExtensionLines;
            }

            set
            {
                m_bIsTextAboveLineBetweenExtensionLines = value;
            }
        }

        public double BasicLength_m
        {
            get
            {
                return dBasicLength_m;
            }

            set
            {
                dBasicLength_m = value;
            }
        }

        private double dBasicLength_m;


        public CDimensionLinear(Point pStart, Point pEnd) : this(pStart, pEnd, true)
        {}

        public CDimensionLinear(Point pStart, Point pEnd, bool isTextAboveLineBetweenExtensionLines) : this("", pStart, pEnd, isTextAboveLineBetweenExtensionLines)
        {}

        public CDimensionLinear(string text, Point pStart, Point pEnd, bool isTextAboveLineBetweenExtensionLines) : base(text)
        {
            ControlPointStart = pStart;
            ControlPointEnd = pEnd;
            IsTextAboveLineBetweenExtensionLines = isTextAboveLineBetweenExtensionLines;
            dBasicLength_m = Math.Sqrt(Math.Pow(pEnd.X - pStart.X, 2) + Math.Pow(pEnd.Y - pStart.Y, 2));
        }

        public override void MirrorYCoordinates()
        {
            this.m_controlPointStart.Y *= -1;
            this.m_controlPointEnd.Y *= -1;
        }

        public override void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            m_controlPointStart = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointStart.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointStart.Y - minY));
            m_controlPointEnd = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointEnd.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointEnd.Y - minY));
        }
    }
}
