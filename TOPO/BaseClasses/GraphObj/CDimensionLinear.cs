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

        public bool m_bIsTextAboveLineBetweenExtensionLines;

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

        public CDimensionLinear(Point pstart_temp, Point pend_temp) : base()
        {
            DisplayedText = "";
            ControlPointStart = pstart_temp;
            ControlPointEnd = pend_temp;
            IsTextAboveLineBetweenExtensionLines = true;
        }

        public CDimensionLinear(Point pstart_temp, Point pend_temp, bool bIsTextAboveLineBetweenExtensionLines_temp) : base()
        {
            DisplayedText = "";
            ControlPointStart = pstart_temp;
            ControlPointEnd = pend_temp;
            IsTextAboveLineBetweenExtensionLines = bIsTextAboveLineBetweenExtensionLines_temp;
        }

        public CDimensionLinear(string text_temp, Point pstart_temp, Point pend_temp, bool bIsTextAboveLineBetweenExtensionLines_temp) : base(text_temp)
        {
            DisplayedText = text_temp;
            ControlPointStart = pstart_temp;
            ControlPointEnd = pend_temp;
            IsTextAboveLineBetweenExtensionLines = bIsTextAboveLineBetweenExtensionLines_temp;
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
