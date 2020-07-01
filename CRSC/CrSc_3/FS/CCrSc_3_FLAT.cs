using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_FLAT : CSO
    {
        // Thin-walled flat bar / strip

        //private float m_ft_f; // Flange Thickness / Hrubka pasnice
        //private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;
        private int m_iNumberOfStrips;

        /*
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
        */

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public int iNumberOfStrips
        {
            get { return m_iNumberOfStrips; }
            set { m_iNumberOfStrips = value; }
        }

        public CCrSc_3_FLAT() { }

        public CCrSc_3_FLAT(int iNoStrips, float fh, float ft) : this(0, iNoStrips, fh, ft, Colors.Olive) { }

        public CCrSc_3_FLAT(int iID_temp, int iNoStrips, float fh, float ft, Color color_temp)
        {
            ID = iID_temp;

            Name_long = iNoStrips +"x" + (fh * 1000).ToString("F0") + "x" + (ft * 1000).ToString("F0");
            Name_short = iNoStrips + "x" + (fh * 1000).ToString("F0") + "x" + (ft * 1000).ToString("F0");

            CSColor = color_temp;  // Set cross-section color
            //ITotNoPoints = 4;
            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 4;

            h = fh;
            b = iNoStrips * ft;
            t_min = b;
            t_max = b;

            //m_ft_f = iNoStrips * ft;
            //m_ft_w = iNoStrips * ft;
            m_fd = fh;

            // Load Cross-section Database Values - based on cross-section Name_short
            FillCrScPropertiesByTableData();

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(ITotNoPoints);

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
            float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
            // Point No. 1
            CrScPointsOutArr[0, 0] = -(float)b / 2f;     // y
            CrScPointsOutArr[0, 1] =  (float)h / 2f;     // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = -CrScPointsOutArr[0, 0];           // y
            CrScPointsOutArr[1, 1] =  CrScPointsOutArr[0, 1];           // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = -CrScPointsOutArr[0, 0];           // y
            CrScPointsOutArr[2, 1] = -CrScPointsOutArr[0, 1];           // z

            // Point No. 4
            CrScPointsOutArr[3, 0] =  CrScPointsOutArr[0, 0];           // y
            CrScPointsOutArr[3, 1] = -CrScPointsOutArr[0, 1];           // z

            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
        }

        public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        {
            // Temporary - odstranit po implementacii vypoctu

            y_min = -b / 2;
            y_max = b / 2;

            z_min = -h / 2;
            z_max = h / 2;

            /*
            for (int i = 0; i < ITotNoPoints; i++)
            {
                Point p = CrScPointsOut[i];
                p.X += y_min;
                p.Y += 0;
                CrScPointsOut[i] = p;
            }*/
        }
    }
}
