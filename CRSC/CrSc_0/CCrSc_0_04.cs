using MATH;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_04:CCrSc
    {
        // Triangular Prism / Equilateral

        //----------------------------------------------------------------------------
        private float m_fa;   // Length of Side
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] m_CrScPoint; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }

        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_04()  {   }
        public CCrSc_0_04(float fa)
        {
            IsShapeSolid = true;
            ITotNoPoints = 3;
            m_fa = fa;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            //CrScPointsOut = new List<Point>(ITotNoPoints);

            // Fill Array Data
            CrScPointsOut = Geom2D.GetTrianEqLatPointCoord1Array(m_fa);
            //CrScPointsOut = Geom2D.GetTrianEqLatPointCoord1(m_fa);

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        private float m_fh;  // Height
        private float m_fb;  // Base Length

        public CCrSc_0_04(float fh, float fb)
        {
            IsShapeSolid = true;
            ITotNoPoints = 3;
            m_fh = fh;
            m_fb = fb;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data

            // Isosceles
            CrScPointsOut = Geom2D.GetTrianIsosCelPointCoordArray(m_fh, m_fb);
            // Right - angled
            CrScPointsOut = Geom2D.GetTrianRightAngPointCoordArray(m_fh, m_fb);

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_0_04(float fN0y, float fN0z, float fN1y, float fN1z, float fN2y, float fN2z)
        {
            IsShapeSolid = true;
            ITotNoPoints = 3;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            // CalcCrSc_Coord_Scalene();

            // Point No. 1
            CrScPointsOut[0, 0] = fN0y;     // y
            CrScPointsOut[0, 1] = fN0z;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = fN1y;     // y
            CrScPointsOut[1, 1] = fN1z;     // z

            // Point No. 3
            CrScPointsOut[2, 0] = fN2y;     // y
            CrScPointsOut[2, 1] = fN2z;     // z

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        //public CCrSc_0_04(float fN0y, float fN0z, float fN1y, float fN1z, float fN2y, float fN2z)
        //{
        //    IsShapeSolid = true;
        //    ITotNoPoints = 3;

        //    // Create Array - allocate memory            
        //    CrScPointsOut = new List<Point>(ITotNoPoints);
        //    // Fill Array Data
        //    // CalcCrSc_Coord_Scalene();

        //    // Point No. 1            
        //    CrScPointsOut.Add(new Point(fN0y, fN0z));

        //    // Point No. 2            
        //    CrScPointsOut.Add(new Point(fN1y, fN1z));

        //    // Point No. 3            
        //    CrScPointsOut.Add(new Point(fN2y, fN2z));

        //    // Fill list of indices for drawing of surface - triangles edges
        //    loadCrScIndices();
        //}

        // Scalene - general
        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_Scalene()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
        }

		protected override void loadCrScIndices()
        {
            // const int secNum = 3;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(24);

            // Front Side / Forehead
            TriangleIndices.Add(0);
            TriangleIndices.Add(2);
            TriangleIndices.Add(1);

            // Back Side 
            TriangleIndices.Add(3);
            TriangleIndices.Add(4);
            TriangleIndices.Add(5);

            // Shell Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 3, 4, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 4, 5, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 5, 3, 0);
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
