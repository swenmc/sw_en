using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.GraphObj
{
    [Serializable]
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

        //public CDimensionArc(Point pRef, Point pstart, Point pend) : base(pRef)
        //{
        //    DisplayedText = "";
        //    ControlPointStart = pstart;
        //    ControlPointEnd = pend;
        //}

        public CDimensionArc(Point pRef, Point pstart, Point pend, Point pcenter) : base(pRef)
        {
            DisplayedText = "";
            ControlPointStart = pstart;
            ControlPointEnd = pend;
            ControlPointCenter = pcenter;
        }

        public override void MirrorYCoordinates()
        {
            this.m_controlPointStart.Y *= -1;
            this.m_controlPointEnd.Y *= -1;
            this.m_controlPointCenter.Y *= -1;
            base.MirrorYCoordinates();
        }
        public override void MirrorXCoordinates()
        {
            this.m_controlPointStart.X *= -1;
            this.m_controlPointEnd.X *= -1;
            this.m_controlPointCenter.X *= -1;
            base.MirrorXCoordinates();
        }

        public override void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            m_controlPointStart = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointStart.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointStart.Y - minY));
            m_controlPointEnd = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointEnd.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointEnd.Y - minY));
            m_controlPointCenter = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointCenter.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointCenter.Y - minY));
            ControlPointRef = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (ControlPointRef.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (ControlPointRef.Y - minY));
        }
    }
}
