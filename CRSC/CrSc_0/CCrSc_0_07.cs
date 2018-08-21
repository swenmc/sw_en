using MATH;
using System;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_07 : CCrSc
    {
        // Solid Hexagon

        //----------------------------------------------------------------------------
        private float m_fa;   // Side
        private float m_fd;   // Circumscribed Circle Diameter / Polygon is Inscribed in Circle
        //private short ITotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] CrScPointsOut; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
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
        public CCrSc_0_07()  {   }
        public CCrSc_0_07(float fa)
        {
            IsShapeSolid = true;
            ITotNoPoints = 6+1; // Total Number of Points in Section (1 point for Centroid)
            m_fa = fa;

            // Calculate Diameter of Circumscribed Circle
            m_fd = Geom2D.GetRadiusfromSideLength(m_fa, 6);

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord()
        {
            // Fill Edge Points Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            CrScPointsOut = Geom2D.AddCentroidPosition_Zero(Geom2D.GetHexagonPointCoord(m_fa));
        }

        protected override void loadCrScIndices()
        {
            CCrSc_0_02 oTemp = new CCrSc_0_02();
            oTemp.loadCrScIndices_02_03(ITotNoPoints);            
            TriangleIndices = oTemp.TriangleIndices;
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
