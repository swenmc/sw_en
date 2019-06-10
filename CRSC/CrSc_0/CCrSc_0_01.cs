using MATH;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    [Serializable]
    public class CCrSc_0_01:CCrSc
    {
        // Solid Quater Circle / Stvrtkruh

        //----------------------------------------------------------------------------
        private float m_fd;   // Diameter/ Priemer
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing (withCentroid Point)
        //public float[,] m_CrScPoint; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        float m_fr_out; // Radius

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_01()  {   }
        public CCrSc_0_01(float fd, short iTotNoPoints)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 19+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            m_fd = fd;
            ITotNoPoints = iTotNoPoints; // + 1 auxialiary node in centroid / stredovy bod v tazisku

            m_fr_out = m_fd / 2f;

            if (iTotNoPoints < 2 || m_fr_out <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(ITotNoPoints);
            //CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        public CCrSc_0_01(float fd)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 19+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            m_fd = fd;
            ITotNoPoints = 20; // + 1 auxialiary node in centroid / stredovy bod v tazisku

            m_fr_out = m_fd / 2f;

            if (m_fr_out <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(ITotNoPoints);
            //CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        //void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Outside Points Coordinates
        //    CrScPointsOut = Geom2D.GetArcPointCoordArray_CW_deg(m_fr_out, 180, 270, ITotNoPoints);

        //    // Centroid
        //    //Point p = CrScPointsOut[ITotNoPoints - 1];
        //    //p.X = 0.0;
        //    //p.Y = 0.0;
        //    CrScPointsOut[ITotNoPoints - 1, 0] = 0f;
        //    CrScPointsOut[ITotNoPoints - 1, 1] = 0f;
        //}

        void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Outside Points Coordinates
            CrScPointsOut = Geom2D.GetArcPointCoord_CW_deg(m_fr_out, 180, 270, ITotNoPoints);

            // Centroid
            CrScPointsOut.Add(new Point(0, 0));
            //Point p = CrScPointsOut[ITotNoPoints - 1];
            //p.X = 0.0;
            //p.Y = 0.0;
        }

        protected override void loadCrScIndices()
		{
            CCrSc_0_00 oTemp = new CCrSc_0_00();
            oTemp.loadCrScIndices_00_01(ITotNoPoints);
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
