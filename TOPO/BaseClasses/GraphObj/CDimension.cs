using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.GraphObj
{
    public class CDimension
    {
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

        public CDimension() { }

        public CDimension(string text)
        {
            DisplayedText = text;
        }

        public virtual void MirrorYCoordinates() { }
        public virtual void UpdatePoints(double minX, double minY, float modelMarginLeft_x, float fmodelMarginTop_y, double dReal_Model_Zoom_Factor) { }
    }
}
