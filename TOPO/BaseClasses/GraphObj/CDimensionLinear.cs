﻿using System;
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

        private bool m_bIsTextAboveLine;

        public bool IsTextAboveLine
        {
            get
            {
                return m_bIsTextAboveLine;
            }

            set
            {
                m_bIsTextAboveLine = value;
            }
        }

        private bool m_bIsDimensionUnderLine;

        public bool IsDimensionUnderLine
        {
            get
            {
                return m_bIsDimensionUnderLine;
            }

            set
            {
                m_bIsDimensionUnderLine = value;
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


        public CDimensionLinear(Point pStart, Point pEnd) : this(pStart, pEnd, true, true)
        {}

        public CDimensionLinear(Point pStart, Point pEnd, bool isTextAboveLineBetweenExtensionLines, bool isDimensionUnderLine, double dOffsetFromOrigin_pxs = 30) : this("", pStart, pEnd, isTextAboveLineBetweenExtensionLines, isDimensionUnderLine, dOffsetFromOrigin_pxs)
        {}

        public CDimensionLinear(string text, Point pStart, Point pEnd, bool isTextAboveLineBetweenExtensionLines, bool isDimensionUnderLine, double dOffsetFromOrigin_pxs = 30) : base(text)
        {
            ControlPointStart = pStart;
            ControlPointEnd = pEnd;
            IsTextAboveLine = isTextAboveLineBetweenExtensionLines;
            IsDimensionUnderLine = isDimensionUnderLine;
            OffsetFromOrigin_pxs = dOffsetFromOrigin_pxs;
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
