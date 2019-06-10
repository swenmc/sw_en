using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.GraphObj
{
    [Serializable]
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

        private bool m_bIsTextOutSide;

        public bool IsTextOutSide
        {
            get
            {
                return m_bIsTextOutSide;
            }

            set
            {
                m_bIsTextOutSide = value;
            }
        }

        private bool m_bIsDimensionOutSide;

        public bool IsDimensionOutSide
        {
            get
            {
                return m_bIsDimensionOutSide;
            }

            set
            {
                m_bIsDimensionOutSide = value;
            }
        }

        private double dBasicLength_m;

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

        private double m_dOffsetFromOrigin_pxs;

        public double OffsetFromOrigin_pxs
        {
            get
            {
                return m_dOffsetFromOrigin_pxs;
            }

            set
            {
                m_dOffsetFromOrigin_pxs = value;
            }
        }

        public CDimensionLinear() { }
        public CDimensionLinear(Point pRef, Point pStart, Point pEnd) : this(pRef, pStart, pEnd, true, true)
        {}

        public CDimensionLinear(Point pRef, Point pStart, Point pEnd, bool isTextOutSide, bool isDimensionOutSide, double dOffsetFromOrigin_pxs = 30) 
            : this(pRef, "", pStart, pEnd, isTextOutSide, isDimensionOutSide, dOffsetFromOrigin_pxs)
        {}

        public CDimensionLinear(Point pRef, string text, Point pStart, Point pEnd, bool isTextOutSide, bool isDimensionOutSide, double dOffsetFromOrigin_pxs = 30) : base(pRef, text)
        {
            ControlPointStart = pStart;
            ControlPointEnd = pEnd;
            IsTextOutSide = isTextOutSide;
            IsDimensionOutSide = isDimensionOutSide;
            OffsetFromOrigin_pxs = dOffsetFromOrigin_pxs;
            dBasicLength_m = Math.Sqrt(Math.Pow(pEnd.X - pStart.X, 2) + Math.Pow(pEnd.Y - pStart.Y, 2));
        }

        public override void MirrorYCoordinates()
        {
            this.m_controlPointStart.Y *= -1;
            this.m_controlPointEnd.Y *= -1;
            base.MirrorYCoordinates();
        }
        public override void MirrorXCoordinates()
        {
            this.m_controlPointStart.X *= -1;
            this.m_controlPointEnd.X *= -1;
            base.MirrorXCoordinates();
        }

        public override void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor)
        {
            m_controlPointStart = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointStart.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointStart.Y - minY));
            m_controlPointEnd = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (m_controlPointEnd.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (m_controlPointEnd.Y - minY));
            ControlPointRef = new Point(modelMarginLeft_x + dReal_Model_Zoom_Factor * (ControlPointRef.X - minX), fmodelMarginTop_y + dReal_Model_Zoom_Factor * (ControlPointRef.Y - minY));
        }
    }
}
