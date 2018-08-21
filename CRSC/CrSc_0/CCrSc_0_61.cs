using MATH;
using System;
using System.Windows.Media;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_61 : CCrSc
    {
        // Y-section

        /*


          9   1    3   4
             _      _
             \ \ 2 / /
              \ \ / /    t
               \ * /
              8 | |  5
                | |    b
                |_|
              7      6



         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */


        //----------------------------------------------------------------------------
        private float m_fb;   // Width  / Sirka
        private float m_ft;   // Thickness / Hrubka 
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] CrScPointsOut; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fb
        {
            get { return m_fb; }
            set { m_fb = value; }
        }
        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }
        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_61()  {   }
        public CCrSc_0_61(float fb, float ft)
        {
            IsShapeSolid = true;
            ITotNoPoints = 9;
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

            // Polar Coordinates of Points 1,3,4,6,7,9
            // Auxiliary Angle
            float fAlpha_Aux = (float)Math.Atan(m_ft / 2 / (m_fb + m_ft / 3));
            // Calculate Radius
            float fr = (m_ft / 2f) / (float)Math.Sin(fAlpha_Aux);

            // Calculate coordinates of 2, 5, 8 - Equilateral Triangle
            float[,] fArrTemp = new float[3, 2];
            fArrTemp = Geom2D.GetTrianEqLatPointCoord1(m_ft);

            // Transform Radians to Degrees - input to GetPositionX/Y functions
            fAlpha_Aux = 180f / MathF.fPI * fAlpha_Aux;
            // Transform Degrees to Radians
            // fAlpha_Aux = MathF.fPI / 180f * fAlpha_Aux;

            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = Geom2D.GetPositionX(fr, 210f + fAlpha_Aux);    // y
            CrScPointsOut[0, 1] = Geom2D.GetPositionY_CW(fr, 210f + fAlpha_Aux);    // z

            // Point No. 2
            CrScPointsOut[1, 0] = fArrTemp[0, 0];     // y
            CrScPointsOut[1, 1] = fArrTemp[0, 1];     // z

            // Point No. 3
            CrScPointsOut[2, 0] = Geom2D.GetPositionX(fr, 330f - fAlpha_Aux);    // y
            CrScPointsOut[2, 1] = Geom2D.GetPositionY_CW(fr, 330f - fAlpha_Aux);    // z

            // Point No. 4
            CrScPointsOut[3, 0] = Geom2D.GetPositionX(fr, 330f + fAlpha_Aux);    // y
            CrScPointsOut[3, 1] = Geom2D.GetPositionY_CW(fr, 330f + fAlpha_Aux);    // z

            // Point No. 5
            CrScPointsOut[4, 0] = fArrTemp[1, 0];      // y
            CrScPointsOut[4, 1] = fArrTemp[1, 1];      // z

            // Point No. 6
            CrScPointsOut[5, 0] = Geom2D.GetPositionX(fr, 90f - fAlpha_Aux);    // y
            CrScPointsOut[5, 1] = Geom2D.GetPositionY_CW(fr, 90f - fAlpha_Aux);    // z

            // Point No. 7
            CrScPointsOut[6, 0] = Geom2D.GetPositionX(fr, 90f + fAlpha_Aux);    // y
            CrScPointsOut[6, 1] = Geom2D.GetPositionY_CW(fr, 90f + fAlpha_Aux);    // z

            // Point No. 8
            CrScPointsOut[7, 0] = fArrTemp[2, 0];      // y
            CrScPointsOut[7, 1] = fArrTemp[2, 1];      // z

            // Point No. 9
            CrScPointsOut[8, 0] = Geom2D.GetPositionX(fr, 210f - fAlpha_Aux);    // y
            CrScPointsOut[8, 1] = Geom2D.GetPositionY_CW(fr, 210f - fAlpha_Aux);    // z
        }

		protected override void loadCrScIndices()
        {
            // const int secnum = 9;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(42 + 9 * 6);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 7, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 3, 4);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 6, 7);
            // Front Side / Forehead
            TriangleIndices.Add(1);
            TriangleIndices.Add(7);
            TriangleIndices.Add(4);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 17, 16, 10, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 12, 11, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 16, 15, 14, 13);
            // Back Side
            TriangleIndices.Add(10);
            TriangleIndices.Add(13);
            TriangleIndices.Add(16);

            // Shell
            DrawCaraLaterals(9, TriangleIndices);
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
