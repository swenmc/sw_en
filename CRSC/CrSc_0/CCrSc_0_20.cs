using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_20 : CCrSc
    {
        // Semicircle Curve / Polovica medzikruzia - polkruznica

        //----------------------------------------------------------------------------
        private float m_fd;   // Diameter/ Priemer
        private float m_ft;   // Thickness/ Hrubka
        //private short m_iNoPoints; // Number of Cross-section Points for Drawing in One Circle
        //public  float[,] m_CrScPointOut; // Array of Outside Points and values in 2D
        //public float[,] m_CrScPointIn; // Array of Inside Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }

        /*public short INoPoints
        {
            get { return m_iNoPoints; }
            set { m_iNoPoints = value; }
        }*/

        float m_fr_out;
        float m_fr_in;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_20()  {   }
        public CCrSc_0_20(float fd, float ft, short iNoPoints)
        {
            INoPointsIn = INoPointsOut = iNoPoints; // vykreslujeme ako n-uholnik, pocet bodov n, + 1 centroid
            m_fd = fd;
            m_ft = ft;

            float fd_in = m_fd - 2 * m_ft;

            // Radii
            m_fr_out = m_fd / 2f;
            m_fr_in = fd_in / 2f;

            if (iNoPoints < 2 || m_fr_in == m_fr_out)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        public CCrSc_0_20(float fd, float ft)
        {
            INoPointsIn = INoPointsOut = 37; // vykreslujeme ako n-uholnik, pocet bodov n, + 1 centroid 
            m_fd = fd;
            m_ft = ft;

            float fd_in = m_fd - 2 * m_ft;

            // Radii
            m_fr_out = m_fd / 2f;
            m_fr_in = fd_in / 2f;

            if (m_fr_in == m_fr_out)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

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
            CrScPointsOut = Geom2D.GetArcPointCoord(m_fr_out, 180, 360, INoPointsOut);

            // Inside Points
            CrScPointsIn = Geom2D.GetArcPointCoord(m_fr_in, 180, 360, INoPointsIn);
        }

		protected override void loadCrScIndices()
        {
            //const int secNum = 37;  // Number of points in section (2D) 36+1 -centroid
            int secNum = INoPointsOut;

            TriangleIndices = new Int32Collection((secNum - 1) * 12 + (secNum - 2) * 12 + 12);

            // Front Side / Forehead

            for (int i = 0; i < secNum - 1; i++)
            {
                if (i < secNum - 2)
                    AddRectangleIndices_CW_1234(TriangleIndices, i, i + 1, i + secNum + 1, i + secNum);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, i, 0, i + 1, i + secNum); // Last Element
            }

            // Back Side
            for (int i = 0; i < secNum - 1; i++)
            {
                if (i < secNum - 2)
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * secNum + i, 2 * secNum + i + secNum, 2 * secNum + i + secNum + 1, 2 * secNum + i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * secNum + i, 2 * secNum + i + secNum, 2 * secNum + i + 1, 2 * secNum + 0); // Last Element
            }

            //// Shell Surface OutSide
            for (int i = 0; i < secNum - 2; i++)
            {
                AddRectangleIndices_CW_1234(TriangleIndices, i, 2 * secNum + i, 2 * secNum + i + 1, i + 1);
            }
            // Shell Surface Inside
            for (int i = 0; i < secNum - 2; i++)
            {
                AddRectangleIndices_CW_1234(TriangleIndices, secNum + i, secNum + i + 1, 2 * secNum + i + secNum + 1, 2 * secNum + i + secNum);
            }

            // Base
            AddRectangleIndices_CW_1234(TriangleIndices, 0, secNum, 3 * secNum, 2 * secNum);
            AddRectangleIndices_CW_1234(TriangleIndices, secNum - 2, 3 * secNum - 2, 4 * secNum - 2, secNum + (secNum - 2));
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