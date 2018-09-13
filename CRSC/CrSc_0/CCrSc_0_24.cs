using MATH;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_24:CCrSc
    {
        // Triangular Prism / Equilateral with Opening
        //----------------------------------------------------------------------------
        private float m_fa;   // Length of Side
        private float m_ft;   // Thickness
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] m_CrScPoint; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
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
        public CCrSc_0_24()  {   }
        public CCrSc_0_24(float fa, float ft)
        {
           IsShapeSolid = false;

           INoPointsIn = INoPointsOut = 3;
           m_fa = fa;
           m_ft = ft;

           // Create Array - allocate memory
           CrScPointsOut = new float[INoPointsOut, 2];
           CrScPointsIn = new float[INoPointsIn, 2];
            //CrScPointsOut = new List<Point>(INoPointsOut);
            //CrScPointsIn = new List<Point>(INoPointsIn);
            // Fill Array Data
            CalcCrSc_Coord_EqLat();

           // Fill list of indices for drawing of surface - triangles edges
           loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_EqLat()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Outside

            // Point No. 1
            CrScPointsOut[0, 0] = 0f;                                     // y
            CrScPointsOut[0, 1] = 2f / 3f * (m_fa/2f) * MathF.fSqrt3;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = m_fa / 2f;                              // y
            CrScPointsOut[1, 1] = -1f / 3f * (m_fa/2f) * MathF.fSqrt3;    // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[1, 0];                     // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1];                      // z

            // Inside

            // Point No. 4
            CrScPointsIn[0, 0] = 0f;                                                // y
            CrScPointsIn[0, 1] = 2f / 3f * (m_fa / 2f) * MathF.fSqrt3 - 2 * m_ft;   // z

            // Point No. 5
            CrScPointsIn[1, 0] = (m_fa / 2f) - (m_ft / (float)Math.Tan(0.523598775598299f)); // y // tan 0.5, resp. tan 30
            CrScPointsIn[1, 1] = -1f / 3f * (m_fa / 2f) * MathF.fSqrt3 + m_ft;               // z

            // Point No. 6
            CrScPointsIn[2, 0] = -CrScPointsIn[1, 0];                     // y
            CrScPointsIn[2, 1] = CrScPointsIn[1, 1];                      // z
        }

        //void CalcCrSc_Coord_EqLat()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Outside

        //    // Point No. 1            
        //    CrScPointsOut.Add(new Point(0, 2f / 3f * (m_fa / 2f) * MathF.fSqrt3));

        //    // Point No. 2            
        //    CrScPointsOut.Add(new Point(m_fa / 2f, -1f / 3f * (m_fa / 2f) * MathF.fSqrt3));

        //    // Point No. 3            
        //    CrScPointsOut.Add(new Point(-CrScPointsOut[1].X, CrScPointsOut[1].Y));
            
        //    // Inside
        //    // Point No. 4            
        //    CrScPointsIn.Add(new Point(0, 2f / 3f * (m_fa / 2f) * MathF.fSqrt3 - 2 * m_ft));
        //    // Point No. 5            
        //    CrScPointsIn.Add(new Point((m_fa / 2f) - (m_ft / (float)Math.Tan(0.523598775598299f)), -1f / 3f * (m_fa / 2f) * MathF.fSqrt3 + m_ft));
        //    // Point No. 6            
        //    CrScPointsIn.Add(new Point(-CrScPointsIn[1].X, CrScPointsIn[1].Y));
        //}

        protected override void loadCrScIndices()
        {
            // const int secNum = 3+3;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(12*6);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 4, 3);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 1, 2, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 3, 5, 2);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 8, 11, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 10, 11, 8, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 9, 10, 7);

            // Shell Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 6, 7, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 7, 8, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 8, 6, 0);

            AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 10, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 11, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 9, 11, 5);
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
