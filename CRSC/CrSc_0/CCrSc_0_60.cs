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
    public class CCrSc_0_60 : CCrSc
    {
        // Doubly symmetric Cruciform

        /*


               1  _   2              ____|/
                 | |                    /|
              12 | |  3                  |
       11  ______| |_____  4    t     h  |
          |______ * _____|               |
       10      9 | |  6    5             |
                 | |                     |
                 |_|                 ____|/
               8      7                 /|

                  b
          |/_____________|/
         /|             /|


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
        private float m_fb;   // Width  / Sirka
        private float m_ft;   // Thickness / Hrubka 
        //private short ITotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] CrScPointsOut; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
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
        public CCrSc_0_60()  {   }
        public CCrSc_0_60(float fh, float fb, float ft)
        {
            IsShapeSolid = true;
            ITotNoPoints = 12;
            m_fh = fh;
            m_fb = fb;
            m_ft = ft;
 
            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0,0] = -m_ft / 2f;    // y
            CrScPointsOut[0,1] = m_fh / 2f;     // z

            // Point No. 2
            CrScPointsOut[1,0] = -CrScPointsOut[0,0];    // y
            CrScPointsOut[1,1] = CrScPointsOut[0,1];     // z

            // Point No. 3
            CrScPointsOut[2,0] = -CrScPointsOut[0,0];    // y
            CrScPointsOut[2,1] = m_ft /2f;             // z

            // Point No. 4
            CrScPointsOut[3,0] = m_fb/2f;              // y
            CrScPointsOut[3,1] = CrScPointsOut[2,1];     // z

            // Point No. 5
            CrScPointsOut[4,0] = CrScPointsOut[3,0];      // y
            CrScPointsOut[4,1] = -CrScPointsOut[3,1];     // z

            // Point No. 6
            CrScPointsOut[5,0] = CrScPointsOut[2,0];      // y
            CrScPointsOut[5,1] = -CrScPointsOut[2,1];     // z

            // Point No. 7
            CrScPointsOut[6,0] = CrScPointsOut[1,0];      // y
            CrScPointsOut[6,1] = -CrScPointsOut[1,1];     // z

            // Point No. 8
            CrScPointsOut[7,0] = CrScPointsOut[0,0];      // y
            CrScPointsOut[7,1] = -CrScPointsOut[0,1];     // z

            // Point No. 9
            CrScPointsOut[8,0] = CrScPointsOut[0,0];     // y
            CrScPointsOut[8,1] = -CrScPointsOut[2,1];     // z

            // Point No. 10
            CrScPointsOut[9,0] = -CrScPointsOut[4,0];     // y
            CrScPointsOut[9,1] =  CrScPointsOut[4,1];     // z

            // Point No. 11
            CrScPointsOut[10,0] = -CrScPointsOut[3,0];    // y
            CrScPointsOut[10,1] = CrScPointsOut[3,1];     // z

            // Point No. 12
            CrScPointsOut[11,0] = CrScPointsOut[0,0];     // y
            CrScPointsOut[11,1] = CrScPointsOut[2,1];     // z
        }

		protected override void loadCrScIndices()
        {
            // const int secNum = 12;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 11);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 5, 6, 7, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 8, 9, 10, 11);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 5, 8, 11);

            // Back Side 
            AddRectangleIndices_CW_1234(TriangleIndices, 23, 14, 13, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 17, 16, 15, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 20, 19, 18, 17);
            AddRectangleIndices_CW_1234(TriangleIndices, 23, 22, 21, 20);
            AddRectangleIndices_CW_1234(TriangleIndices, 23, 20, 17, 14);

            // Shell
            DrawCaraLaterals(12, TriangleIndices);
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
