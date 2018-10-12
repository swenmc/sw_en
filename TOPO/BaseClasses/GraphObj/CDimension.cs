using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses.GraphObj
{
    public class CDimension
    {
        private Point m_controlPointRef;

        public Point ControlPointRef
        {
            get
            {
                return m_controlPointRef;
            }

            set
            {
                m_controlPointRef = value;
            }
        }

        private string m_DisplayedText;

        public string DisplayedText
        {
            get
            {
                return m_DisplayedText;
            }

            set
            {
                m_DisplayedText = value;
            }
        }

        public CDimension(Point refPoint) : this(refPoint, "") { }

        public CDimension(Point refPoint, string text)
        {
            m_controlPointRef = refPoint;
            DisplayedText = text;

        }

        public virtual void MirrorYCoordinates() { }
        public virtual void MirrorXCoordinates() { }
        public virtual void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor) { }
    }
}
