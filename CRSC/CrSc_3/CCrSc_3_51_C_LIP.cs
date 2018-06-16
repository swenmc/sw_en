using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;

namespace CRSC
{
    public class CCrSc_3_51_C_LIP : CCrSc_TW
    {
        // Thin-walled mono-symmetrical C-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        public float Ft_f
        {
            get { return m_ft_f; }
            set { m_ft_f = value; }
        }
        public float Ft_w
        {
            get { return m_ft_w; }
            set { m_ft_w = value; }
        }
        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public CCrSc_3_51_C_LIP(float fh, float fb, float fc_lip, float ft, Color color_temp)
      {
            //ITotNoPoints = 12;
            IsShapeSolid = true;
            ITotNoPoints = 12;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;
            m_fd = fh - 2 * ft;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();
        }

      public void CalcCrSc_Coord()
      {
      // Todo
      }
    }
}
