using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace CRSC
{
    public class CCrSc_3_09 : CCrSc_0_02
    {
        // Solid round bar

        /*
        //----------------------------------------------------------------------------
        private float m_fd;   // Diameter/ Priemer
        private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing (withCentroid Point)
        public float[,] m_CrScPoint; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }

        float m_fr_out; // Radius
        */

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_3_09(float fd, short iTotNoPoints)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 72+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            Fd = fd;
            ITotNoPoints = iTotNoPoints; // + 1 auxialiary node in centroid / stredovy bod v tazisku

            Fr_out = Fd / 2f;

            if (iTotNoPoints < 2 || Fr_out <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // All indices together
            loadCrScIndices();
        }

        public CCrSc_3_09(float fd)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 72+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            Fd = fd;
            ITotNoPoints = 73; // 1 auxialiary node in centroid / stredovy bod v tazisku

            Fr_out = Fd / 2f;

            if (Fr_out <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // All indices together
            loadCrScIndices();
        }
    }
}
