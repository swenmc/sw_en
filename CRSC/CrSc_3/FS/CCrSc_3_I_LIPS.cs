using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_I_LIPS : CSO
    {
        // Thin-walled doubly symmetrical I-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;
        private float m_fc_lip;

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
        public float Fc_lip
        {
            get { return m_fc_lip; }
            set { m_fc_lip = value; }
        }

        public CCrSc_3_I_LIPS(int iID_temp, float fh, float fb, float fc_lip, float ft, Color color_temp)
        {
            ID = iID_temp;

            Name = "I " + (fh * 1000).ToString() + (ft * 1000).ToString();
            NameDatabase = (fh * 1000).ToString() + (ft * 1000).ToString();

            CSColor = color_temp;  // Set cross-section color

            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 20;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;
            Fc_lip = fc_lip;

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

            // Point No. 1
            CrScPointsOut.Add(new Point(b / 2.0, h / 2.0));
            // Point No. 2
            CrScPointsOut.Add(new Point(-CrScPointsOut[0].X, CrScPointsOut[0].Y));
            // Point No. 3
            CrScPointsOut.Add(new Point(CrScPointsOut[1].X, CrScPointsOut[1].Y - Fc_lip));
            // Point No. 4
            CrScPointsOut.Add(new Point(CrScPointsOut[2].X - m_ft_f, CrScPointsOut[2].Y));
            // Point No. 5
            CrScPointsOut.Add(new Point(CrScPointsOut[3].X, CrScPointsOut[1].Y - m_ft_f));
            // Point No. 6
            CrScPointsOut.Add(new Point(m_ft_w / 2.0, CrScPointsOut[4].Y));
            // Point No. 7
            CrScPointsOut.Add(new Point(CrScPointsOut[5].X, -CrScPointsOut[5].Y));
            // Point No. 8
            CrScPointsOut.Add(new Point(-CrScPointsOut[4].X, -CrScPointsOut[4].Y));
            // Point No. 9
            CrScPointsOut.Add(new Point(-CrScPointsOut[3].X, -CrScPointsOut[3].Y));
            // Point No. 10
            CrScPointsOut.Add(new Point(CrScPointsOut[2].X, -CrScPointsOut[2].Y));
            // Point No. 11
            CrScPointsOut.Add(new Point(CrScPointsOut[1].X, -CrScPointsOut[1].Y));
            // Point No. 12
            CrScPointsOut.Add(new Point(-CrScPointsOut[10].X, CrScPointsOut[10].Y));
            // Point No. 13
            CrScPointsOut.Add(new Point(-CrScPointsOut[9].X, CrScPointsOut[9].Y));
            // Point No. 14
            CrScPointsOut.Add(new Point(-CrScPointsOut[8].X, CrScPointsOut[8].Y));
            // Point No. 15
            CrScPointsOut.Add(new Point(-CrScPointsOut[7].X, CrScPointsOut[7].Y));
            // Point No. 16
            CrScPointsOut.Add(new Point(-CrScPointsOut[6].X, CrScPointsOut[6].Y));
            // Point No. 17
            CrScPointsOut.Add(new Point(-CrScPointsOut[5].X, CrScPointsOut[5].Y));
            // Point No. 18
            CrScPointsOut.Add(new Point(-CrScPointsOut[4].X, CrScPointsOut[4].Y));
            // Point No. 19
            CrScPointsOut.Add(new Point(-CrScPointsOut[3].X, CrScPointsOut[3].Y));
            // Point No. 20
            CrScPointsOut.Add(new Point(-CrScPointsOut[2].X, CrScPointsOut[2].Y));
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection(7*6);

            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 1, 4, 17);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 17, 18, 19);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 1, 2, 3, 4);

            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 5, 6, 15, 16);

            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 7, 8, 9, 10);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 11, 12, 13, 14);
            AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 7, 10, 11, 14);
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection(7*6);

            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 4, ITotNoPoints + 17);
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 17, ITotNoPoints + 18, ITotNoPoints + 19);
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 3, ITotNoPoints + 4);

            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 5, ITotNoPoints + 6, ITotNoPoints + 15, ITotNoPoints + 16);

            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 7, ITotNoPoints + 8, ITotNoPoints + 9, ITotNoPoints + 10);
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 11, ITotNoPoints + 12, ITotNoPoints + 13, ITotNoPoints + 14);
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 7, ITotNoPoints + 10, ITotNoPoints + 11, ITotNoPoints + 14);
        }
    }
}


