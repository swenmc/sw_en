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
    public class CCrSc_0_00 : CCrSc
    {
        // Solid Half Circle / Semicircle shape / Polkruh

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
        public CCrSc_0_00()  {   }
        public CCrSc_0_00(float fd, short iTotNoPoints)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 19+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            m_fd = fd;
            ITotNoPoints = iTotNoPoints; // + 1 auxialiary node in centroid / stredovy bod v tazisku

            m_fr_out = m_fd / 2f;

            if (iTotNoPoints < 2 || m_fr_out <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        public CCrSc_0_00(float fd)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 19+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            m_fd = fd;
            ITotNoPoints = 20; // 1 auxialiary node in centroid / stredovy bod v tazisku

            m_fr_out = m_fd / 2f;

            if (m_fr_out <= 0f)
                return;

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
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Outside Points Coordinates
            CrScPointsOut = Geom2D.GetArcPointCoord(m_fr_out, 180, 360, ITotNoPoints);

            // Centroid
            CrScPointsOut[ITotNoPoints - 1, 0] = 0f;
            CrScPointsOut[ITotNoPoints - 1, 1] = 0f;
        }

        protected override void loadCrScIndices()
        {
            loadCrScIndices_00_01(ITotNoPoints);
        }

        // Auxiliary, used for also other cross-sections
        public void loadCrScIndices_00_01(int iTotNoPoints)
		{
           TriangleIndices = new Int32Collection();

           // Front Side / Forehead
           for (int i = 0; i < iTotNoPoints - 1; i++)
           {
               if (i < iTotNoPoints - 2)
            {
             TriangleIndices.Add(i);
             TriangleIndices.Add(iTotNoPoints - 1);
             TriangleIndices.Add(i + 1);
            }
            else // Last Element
            {
             TriangleIndices.Add(i);
             TriangleIndices.Add(iTotNoPoints - 1);
             TriangleIndices.Add(0);
            }
           }

           // Back Side
           for (int i = 0; i < iTotNoPoints - 1; i++)
           {
            if (i < iTotNoPoints - 2)
            {
             TriangleIndices.Add(iTotNoPoints + i);
             TriangleIndices.Add(iTotNoPoints + i + 1);
             TriangleIndices.Add(iTotNoPoints + iTotNoPoints - 1);
            }
            else // Last Element
            {
             TriangleIndices.Add(iTotNoPoints + i);
             TriangleIndices.Add(iTotNoPoints);
             TriangleIndices.Add(iTotNoPoints + iTotNoPoints - 1);
            }
           }

           // Shell Surface Arc
           for (int i = 0; i < iTotNoPoints - 2; i++)
            AddRectangleIndices_CW_1234(TriangleIndices, i, iTotNoPoints + i, iTotNoPoints + i + 1, i + 1);

           // Flat Sides - !!! Clock-wise points of arc generation
           AddRectangleIndices_CW_1234(TriangleIndices, 0, iTotNoPoints - 1, 2 * iTotNoPoints - 1, iTotNoPoints);
           AddRectangleIndices_CW_1234(TriangleIndices, iTotNoPoints - 1, iTotNoPoints - 2, 2 * iTotNoPoints - 2, 2 * iTotNoPoints - 1);
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
