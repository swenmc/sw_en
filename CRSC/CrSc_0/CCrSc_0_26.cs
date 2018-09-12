using MATH;
using System;
using System.Windows.Media;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_26 : CCrSc
    {
        // Empty (Hollow) Penthagon / Duty Patuholnik

        //----------------------------------------------------------------------------
        private float m_fa_out;   // OutSide Length / Dlzka vonkajsej strany
        private float m_fa_in;    // InSide Length / Dlzka vnutornej strany
        private float m_fd1_out;   // Circumscribed Circle Diameter / Outside Polygon is Inscribed in Circle / Priemer opisanej kruznice vonkajsieho polygonu
        private float m_fd1_in;    // Circumscribed Circle Diameter / Inside Polygon is Inscribed in Circle / Priemer opisanej kruznice vnutorneho polygonu
        private float m_fd2_out;   // Inscribed Circle Diameter / Circle circumscribed Outside Polygon / Priemer vpisanej kruznice vonkajsieho polygonu
        private float m_fd2_in;    // Inscribed Circle Diameter / Circle circumscribed Inside Polygon / Priemer vpisanej kruznice vnutorneho polygonu 
        private float m_ft;       // Thickness/ Hrubka
        //----------------------------------------------------------------------------

        public float Fa_out
        {
            get { return m_fa_out; }
            set { m_fa_out = value; }
        }

        public float Fa_in
        {
            get { return m_fa_in; }
            set { m_fa_in = value; }
        }

        public float Fd1_out
        {
            get { return m_fd1_out; }
            set { m_fd1_out = value; }
        }

        public float Fd1_in
        {
            get { return m_fd1_in; }
            set { m_fd1_in = value; }
        }
        
        public float Fd2_out
        {
            get { return m_fd2_out; }
            set { m_fd2_out = value; }
        }

        public float Fd2_in
        {
            get { return m_fd2_in; }
            set { m_fd2_in = value; }
        }

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }

        float m_fr1_out;
        float m_fr1_in;
        float m_fr2_out;
        float m_fr2_in;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_26()  {   }
        public CCrSc_0_26(float fa, float ft)
        {
            INoPointsIn = INoPointsOut = 5; // vykreslujeme ako n-uholnik, pocet bodov n
            m_fa_out = fa;
            m_ft = ft;

            // Calculate Diameter of Circumscribed Circle of Outside Polygon
            m_fd1_out = 2 * Geom2D.GetRadiusfromSideLength(m_fa_out, INoPointsOut);
            // Calculate Diameter of Inscribed Circle of Outside Polygon
            m_fd2_out = 2 * Geom2D.GetIntRadiusfromSideLength(m_fa_out, INoPointsOut);

            m_fr1_out = m_fd1_out / 2f;
            m_fr2_out = m_fd2_out / 2f;
            
            // Calculate Side Length of Inside Polygon
            m_fa_in = Geom2D.GetRegularPolygonInternalSideLength(m_fa_out, m_fd2_out / 2, m_fd2_out / 2 - m_ft);
            // Calculate Diameter of Circumscribed Circle of Inside Polygon
            m_fd1_in = 2 * Geom2D.GetRadiusfromSideLength(m_fa_in, INoPointsOut);
            // Calculate Diameter of Inscribed Circle of Inside Polygon
            m_fd2_in = m_fd2_out - 2 *  m_ft;

            m_fr1_in = m_fd1_in / 2f;
            m_fr2_in = m_fd2_in / 2f;

            if (m_fr1_out == m_fr1_in)
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
            CrScPointsOut = Geom2D.GetPentagonPointCoordArray(m_fa_out);

            // Inside Points
            CrScPointsIn = Geom2D.GetPentagonPointCoordArray(m_fa_in);
        }

        protected override void loadCrScIndices()
        {
            int iAux = 0;
            loadCrScIndices_26_28(INoPointsOut, iAux);
        }

        // Auxiliary, used for also other cross-sections
        public void loadCrScIndices_26_28(int iNoPoints, int iAux)
        {
            // iAux - number of auxiliary points in inside/outside collection of points
            // iNoPoints - numer of real points in inside/outside collection of points
            // iAux + iNoPoints - total number of points in inside/outside collection of section

            TriangleIndices = new Int32Collection(iNoPoints * 24);

            // Front Side / Forehead

            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, iAux + i + 1, iAux + i + (iAux + iNoPoints) + 1, iAux + i + (iAux + iNoPoints));
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, iAux + 0, iAux + i + iAux + 1, iAux + i + (iAux + iNoPoints)); // Last Element
            }

            // Back Side
            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints + 1, 2 * (iAux + iNoPoints) + iAux + i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints, 2 * (iAux + iNoPoints) + i + 2 * iAux + 1, 2 * (iAux + iNoPoints) + iAux + 0); // Last Element
            }

            // Shell Surface OutSide
            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + iAux + i + 1, iAux + i + 1);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + i, 2 * (iAux + iNoPoints) + iAux + i, 2 * (iAux + iNoPoints) + iAux, iAux + 0); // Last Element
            }

            // Shell Surface Inside
            for (int i = 0; i < iNoPoints; i++)
            {
                if (i < iNoPoints - 1)
                    AddRectangleIndices_CW_1234(TriangleIndices, iAux + iNoPoints + iAux + i, iAux + iNoPoints + iAux + i + 1, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints + 1, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints);
                else
                    AddRectangleIndices_CW_1234(TriangleIndices, 2 * (iAux + iNoPoints) + 2 * iAux + i + 1, 2 * (iAux + iNoPoints) + i + 2 * iAux + iNoPoints, iAux + iNoPoints + iAux + i, 2 * iAux + iNoPoints); // Last Element
            }
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




