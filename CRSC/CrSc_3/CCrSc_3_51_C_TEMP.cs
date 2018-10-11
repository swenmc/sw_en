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

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;
            CSColor = color_temp;  // Set cross-section color

            Fill_Basic();
        }

        public CCrSc_3_51_C_TEMP(int iID_temp, float fh, float fb, float ft)
        {
            ID = iID_temp;

            Name = "C " + (fh * 1000).ToString() + (ft * 1000).ToString();
            NameDatabase = (fh * 1000).ToString() + (ft * 1000).ToString();

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            Fill_Basic();
        }

        private void Fill_Basic()
        {
            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 8; // Total number of points per section

            m_fd = (float)(h - 2 * m_ft_f);

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

            // Defined Clockwise CW
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

            // zadavane v CCW - jednotlive trojuholniky
            TriangleIndicesFrontSide = new Int32Collection() { 0, 1, 6, 0, 6, 7, 1, 2, 6, 6, 2, 5, 5, 2, 4, 2, 3, 4 };

            // zadavane v CCW - obdlzniky
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 1, 6, 7);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 1, 2, 5, 6);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 2, 3, 4, 5);
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection(3 * 6);

            // zadavane v CCW - jednotlive trojuholniky
            TriangleIndicesBackSide = new Int32Collection() { ITotNoPoints + 0, ITotNoPoints + 6, ITotNoPoints + 1,
                ITotNoPoints + 0, ITotNoPoints + 7, ITotNoPoints + 6,
                ITotNoPoints + 1, ITotNoPoints + 6, ITotNoPoints + 2,
                ITotNoPoints + 6, ITotNoPoints + 5, ITotNoPoints + 2,
                ITotNoPoints + 5, ITotNoPoints + 4, ITotNoPoints + 2,
                ITotNoPoints + 2, ITotNoPoints + 4, ITotNoPoints + 3 };

            // zadavane v CW - obdlzniky (normaly sa vo funkcii zmenia)
            AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 6, ITotNoPoints + 7);
            AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 5, ITotNoPoints + 6);
            AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 2, ITotNoPoints + 3, ITotNoPoints + 4, ITotNoPoints + 5);
        }

        protected override void loadCrScIndicesShell()
        {
            TriangleIndicesShell = new Int32Collection();

            // zadavane v CCW - jednotlive trojuholniky
            TriangleIndicesShell = new Int32Collection() { 0,9,1, 0,8,9,
                                                           1,9,10, 1,10,2,
                                                           2,10,11, 2,11,3,
                                                           3,12,4, 3,11,12,
                                                           4,12,13, 4,13,5,
                                                           5,13,14, 5,14,6,
                                                           6,14,15, 6,15,7,
                                                           7,8,0, 7,15,8};

            // zadavane v CCW - obdlzniky
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 8, 9, 1);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 1, 9, 10, 2);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 2, 10, 11, 3);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 3, 11, 12, 4);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 4, 12, 13, 5);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 5, 13, 14, 6);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 6, 14, 15, 7);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 7, 15, 8, 0);
        }
    }
}