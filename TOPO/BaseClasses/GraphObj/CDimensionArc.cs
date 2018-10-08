using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.GraphObj
{
    public class CDimensionArc : CDimension
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

        private Point m_controlPointCenter;

        public Point ControlPointCenter
        {
            get
            {
                return m_controlPointCenter;
            }

            set
            {
                m_controlPointCenter = value;
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

        public CDimensionArc(Point pstart_temp, Point pend_temp) : base()
        {
            DisplayedText = "";
            ControlPointStart = pstart_temp;
            ControlPointEnd = pend_temp;
        }

        public CDimensionArc(Point pstart_temp, Point pend_temp, Point pcenter_temp) : base()
        {
            DisplayedText = "";
            ControlPointStart = pstart_temp;
            ControlPointEnd = pend_temp;
            ControlPointCenter = pcenter_temp;
        }

        public CDimensionArc(string text_temp, Point pstart_temp, Point pend_temp) : base(text_temp)
        {
            DisplayedText = text_temp;
            ControlPointStart = pstart_temp;
            ControlPointEnd = pend_temp;
        }
    }
}
