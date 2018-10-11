using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_51_C_TEMP : CSO
    {
        // Thin-walled symmetrical simple C-section

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        public float Ft_f
        {
            get { return m_ft_f; }
            set { m_ft_f = value; }
        }
        public float Ft_w
        {
            get { return m_ft_w; }
            set { m_ft_w = value; }
        }
        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public CCrSc_3_51_C_TEMP(int iID_temp, float fh, float fb, float ft, Color color_temp)
        {
            ID = iID_temp;

            Name = "C " + (fh * 1000).ToString() + (ft * 1000).ToString();
            NameDatabase = (fh * 1000).ToString() + (ft * 1000).ToString();

            CSColor = color_temp;  // Set cross-section color

            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 8; // Total number of points per section

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;
            m_fd = fh - 2 * ft;

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(ITotNoPoints);
            // Fill Array Data
            CalcCrSc_Coord();

            // SOLID MODEL
            // Fill list of indices for drawing of surface - triangles edges
            // Particular indices - distinguished colors of member surfaces
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color of member
            loadCrScIndices();

            // WIREFRAME MODEL
            // Complex indices
            loadCrScWireFrameIndices();
        }

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Defined Clockwise
            // Point No. 1
            CrScPointsOut.Add(new Point(b / 2.0, h / 2.0 - m_ft_f));
            // Point No. 2
            CrScPointsOut.Add(new Point(-CrScPointsOut[0].X + m_ft_w, CrScPointsOut[0].Y));
            // Point No. 3
            CrScPointsOut.Add(new Point(CrScPointsOut[1].X, -CrScPointsOut[1].Y));
            // Point No. 4
            CrScPointsOut.Add(new Point(CrScPointsOut[0].X, -CrScPointsOut[0].Y));
            // Point No. 5
            CrScPointsOut.Add(new Point(CrScPointsOut[3].X, CrScPointsOut[3].Y - m_ft_f));
            // Point No. 6
            CrScPointsOut.Add(new Point(-b / 2.0, CrScPointsOut[4].Y));
            // Point No. 7
            CrScPointsOut.Add(new Point(CrScPointsOut[5].X, -CrScPointsOut[5].Y));
            // Point No. 8
            CrScPointsOut.Add(new Point(CrScPointsOut[4].X, -CrScPointsOut[4].Y));
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection(3 * 6);

            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 1, 6, 7);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 1, 2, 5, 6);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 2, 3, 4, 5);
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection(3 * 6);

            AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 6, ITotNoPoints + 7);
            AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 5, ITotNoPoints + 6);
            AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 2, ITotNoPoints + 3, ITotNoPoints + 4, ITotNoPoints + 5);
        }
    }
}