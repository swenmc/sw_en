using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_58 : CCrSc
    {
        // Welded centrally symmetric Z section

        /*




        1   ________  2              ____|/
     t_f   |______  |                   /|
        8      7  | |                    |
                  | |                    |
                  | |                    |
           t_w    | |                 h  |
                  |*|                    |
                  | |                    |
                  | | 3       4          |
                  | |______              |
                  |________|         ____|/
                6            5          /|
                      b
                  |/_______|/
                 /|       /|


         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */


        //----------------------------------------------------------------------------
        private float m_fh;   // Height / Vyska
        private float m_fb;   // Flange Width  / Sirka Pasnice
        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        //private short ITotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] CrScPointsOut; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

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
        /*public short ITotNoPoints
        {
            get { return ITotNoPoints; }
            set { ITotNoPoints = value; }
        }*/
        /*
        public float[,] CrScPoint
        {
            get { return CrScPointsOut; }
            set { CrScPointsOut = value; }
        }
        */

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_58()  {   }
        public CCrSc_0_58(float fh, float fb, float ft_f, float ft_w)
        {
            IsShapeSolid = true;
            ITotNoPoints = 8;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data
            CalcCrSc_Coord_Z_CS();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_Z_CS()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb + m_ft_w / 2f;       // y
            CrScPointsOut[0, 1] = m_fh / 2f;                 // z

            // Point No. 2
            CrScPointsOut[1, 0] = m_ft_w / 2f;              // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];       // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];                    // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1] - m_fh + m_ft_f;    // z

            // Point No. 4
            CrScPointsOut[3, 0] = -CrScPointsOut[0, 0];                   // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                    // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];              // y
            CrScPointsOut[4, 1] = CrScPointsOut[3, 1] - m_ft_f;     // z

            // Point No. 6
            CrScPointsOut[5, 0] = -m_ft_w / 2f;           // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];      // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];      // y
            CrScPointsOut[6, 1] = -CrScPointsOut[2, 1];     // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[0, 0];      // y
            CrScPointsOut[7, 1] = CrScPointsOut[6, 1];      // z
        }

        // Welded asymmetric Z section

        /*

              b_fu
           |/_______|/
          /|       /|

        1   ________  2                  ____|/
    t_fu   |______  |                       /|
        8      7  | |                        |
                  | |                        |
                  | |                        |
           t_w    | |                     h  |
                  | |                        |
                  | |  *                     |
                  | | 3         4            |
                  | |__________              |
                  |____________|         ____|/
                6               5           /|

                   y_c
                  |/__|/
                 /|  /|

                        b_fb
                  |/___________|/
                 /|           /|


         y_c - from left bottom corner
         z_c - from bottom

         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */

        private float m_fb_fu;   // Width of Upper Flange / Sirka hornej pasnice
        private float m_ft_fu;   // Upper Flange Thickness / Hrubka hornej pasnice
        private float m_fb_fb;   // Width of Bottom Flange / Sirka spodnej pasnice
        private float m_ft_fb;   // Bottom Flange Thickness / Hrubka spodnej pasnice
        private float m_fy_c;    // Centroid coordinate / Suradnica tažiska
        private float m_fz_c;    // Centroid coordinate / Suradnica tažiska / Absolute value

        public CCrSc_0_58(float fh, float fb_fu, float fb_fb, float ft_fu, float ft_fb, float ft_w, float fy_c, float fz_c)
        {
            IsShapeSolid = true;
            ITotNoPoints = 8;
            m_fh    = fh;
            m_fb_fu = fb_fu;
            m_fb_fb = fb_fb;
            m_ft_fu = ft_fu;
            m_ft_fb = ft_fb;
            m_ft_w = ft_w;
            m_fy_c = fy_c; 
            m_fz_c = Math.Abs(fz_c); // Absolute value

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord_Z_AS();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_Z_AS()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb_fu - m_fy_c;           // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;               // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0] + m_fb_fu;     // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];               // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];                     // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1] - m_fh + m_ft_fb;    // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0] + m_fb_fb - m_ft_w;    // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                       // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];        // y
            CrScPointsOut[4, 1] = -m_fz_c;                  // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[4, 0] - m_fb_fb;        // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];                  // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];                    // y
            CrScPointsOut[6, 1] = CrScPointsOut[5, 1] + m_fh - m_ft_fu;   // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[0, 0];        // y
            CrScPointsOut[7, 1] = CrScPointsOut[6, 1];        // z
        }

        // Welded centrally symmetric Z section with overhangs

        /*



                       t_f
      _|/____  1   ____________  2              ____|/
      /|          |  ________  |                   /|
  h_o  |          | | 10   9 | |                    |
      _|/____     |_|        | |                    |
      /|       12     11     | |                    |
                   t_o       | |                 h  |
                             |*|      5   6         |
                             | |        _           |
                      t_w    | | 3   4 | |          |
                             | |_______| |          |
                             |___________|      ____|/
                            8              7       /|
                                   b
                             |/__________|/
                            /|          /|

         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */





		protected override void loadCrScIndices()
        {
            // const int secNum = 8;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(36 + 8*6);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 6, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 1, 2, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);

            // Back Side 
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 8, 15, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 14, 13, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 10, 13, 12);

            // Shell Surface
            DrawCaraLaterals(8, TriangleIndices);
        }

        protected override void loadCrScIndicesFrontSide()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesShell()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesBackSide()
        {
            throw new NotImplementedException();
        }

        public override void CalculateSectionProperties()
        {
            throw new NotImplementedException();
        }
    }
}
