using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.GraphObj
{
    public class PointF
    {
        //----------------------------------------------------------------------------
        float MX;
        float MY;

        //----------------------------------------------------------------------------
        public float X
        {
            get
            {
                return MX;
            }
            set
            {
                MX = value;
            }
        }

        public float Y
        {
            get
            {
                return MY;
            }

            set
            {
                MY = value;
            }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public PointF(float x, float y)
        {
            MX = x;
            MY = y;
        }

        
    }
}
