using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_50020_C_NESTED : CSC
    {
        /// <summary>
        /// TODO - prepracovat - je to len kopia C 50020
        /// </summary>

        // Thin-walled mono-symmetrical C-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;
        private float m_fc_lip1;
        private float m_fc_lip2;
        private float fr_1_out;
        private float fr_1_in;
        private float fz_stif;
        private float fy_stif;

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
        public float FC_lip1
        {
            get { return m_fc_lip1; }
            set { m_fc_lip1 = value; }
        }
        public float FC_lip2
        {
            get { return m_fc_lip2; }
            set { m_fc_lip2 = value; }
        }
        public CCrSc_3_50020_C_NESTED() { }
        public CCrSc_3_50020_C_NESTED(float fh, float fb, float ft) : this(0, fh, fb, ft, Colors.Red) { }
        public CCrSc_3_50020_C_NESTED(int iID_temp, float fh, float fb, float ft, Color color_temp)
        {
            ID = iID_temp;

            Name_long = "C " + (fh * 1000).ToString() + (ft * 1000 * 10).ToString() + " nested"; // Original Description
            Name_long = "C " + (fh * 1000).ToString() + (20).ToString() + " nested"; // FS Description
            Name_short = (fh * 1000).ToString() + (20).ToString() + "n";

            // Temporary
            Name_long = "C " + "500" + (20).ToString() + " nested"; // FS Description
            Name_short = "500" + (20).ToString() + "n";

            CSColor = color_temp;  // Set cross-section color

            //ITotNoPoints = 28;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 14;
            ITotNoPoints = INoPointsIn + INoPointsOut;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;

            m_fd = fh - 2 * ft;
            //m_fc_lip1 = 0.012f; // Horizontal
            //m_fc_lip2 = 0.034f; // Vertical

            fr_1_in = 0.006f;
            fr_1_out = fr_1_in + m_ft_f;

            fz_stif = 0.13f;
            fy_stif = 0.04f;

            b_in = b - 2 * fy_stif - 2 * m_ft_w;
            h_in = h - 2 * m_fc_lip2 - 2 * m_ft_f; // TODO - skontrolovat

            // Load Cross-section Database Values - based on cross-section Name_short
            FillCrScPropertiesByTableData();

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(INoPointsOut);
            CrScPointsIn = new List<Point>(INoPointsIn);

            // Fill Array Data
            CalcCrSc_Coord();

            ChangeCoordToCentroid(); // Temp - TODO doriesit zadavanie bodov (CW, CCW), osove systemy, orientaciu os a zjednotit zadanie pre vsetky prierezy

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
            Point[] CrScPointsOutArr = new Point[INoPointsOut];
            Point[] CrScPointsInArr = new Point[INoPointsIn];

            // Outside
            // Point No. 1
            CrScPointsOutArr[0] = new Point(b / 2f - fr_1_out, h / 2f);

            // Point No. 2
            CrScPointsOutArr[1] = new Point(b / 2f, CrScPointsOutArr[0].Y - fr_1_out);

            // Point No. 3
            CrScPointsOutArr[2] = new Point(CrScPointsOutArr[1].X, fz_stif / 2f);

            // Point No. 4
            CrScPointsOutArr[3] = new Point(CrScPointsOutArr[1].X - fy_stif, 0);

            // Point No. 5
            CrScPointsOutArr[4] = new Point(CrScPointsOutArr[2].X, -CrScPointsOutArr[2].Y);

            // Point No. 6
            CrScPointsOutArr[5] = new Point(CrScPointsOutArr[1].X, -CrScPointsOutArr[1].Y);

            // Point No. 7
            CrScPointsOutArr[6] = new Point(CrScPointsOutArr[0].X, -CrScPointsOutArr[0].Y);

            // Mirror about y-y
            for (int i = 0; i < INoPointsOut; i++)
            {
                CrScPointsOutArr[INoPointsOut - i - 1].X = -CrScPointsOutArr[i].X;
                CrScPointsOutArr[INoPointsOut - i - 1].Y = CrScPointsOutArr[i].Y;
            }

            // Inside
            // Point No. 1
            CrScPointsInArr[0] = new Point(b / 2f - fr_1_out, h / 2f - m_ft_f);

            // Point No. 2
            CrScPointsInArr[1] = new Point(b / 2f - m_ft_w, CrScPointsOutArr[0].Y - fr_1_out);

            // Point No. 3
            CrScPointsInArr[2] = new Point(CrScPointsInArr[1].X, fz_stif / 2f);

            // Point No. 4
            CrScPointsInArr[3] = new Point(CrScPointsInArr[1].X - fy_stif, 0);

            // Point No. 5
            CrScPointsInArr[4] = new Point(CrScPointsInArr[2].X, -CrScPointsInArr[2].Y);

            // Point No. 6
            CrScPointsInArr[5] = new Point(CrScPointsInArr[1].X, -CrScPointsInArr[1].Y);

            // Point No. 7
            CrScPointsInArr[6] = new Point(CrScPointsInArr[0].X, -CrScPointsInArr[0].Y);

            // Mirror about y-y
            for (int i = 0; i < INoPointsIn; i++)
            {
                CrScPointsInArr[INoPointsIn - i - 1].X = -CrScPointsInArr[i].X;
                CrScPointsInArr[INoPointsIn - i - 1].Y = CrScPointsInArr[i].Y;
            }

            for (int i = 0; i < CrScPointsOutArr.Length; i++)
            {
                CrScPointsOut.Add(CrScPointsOutArr[i]);
            }

            for (int i = 0; i < CrScPointsInArr.Length; i++)
            {
                CrScPointsIn.Add(CrScPointsInArr[i]);
            }
        }

        public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        {
            // Temporary - odstranit po implementacii vypoctu

            y_min = -b / 2;
            y_max = b / 2;
            z_min = -h / 2;
            z_max = h / 2;

            for (int i = 0; i < INoPointsOut; i++)
            {
                Point p = CrScPointsOut[i];
                p.X += 0;
                p.Y += 0;
                CrScPointsOut[i] = p;
            }

            for (int i = 0; i < INoPointsIn; i++)
            {
                Point p = CrScPointsIn[i];
                p.X += 0;
                p.Y += 0;
                CrScPointsIn[i] = p;
            }
        }
    }
}
