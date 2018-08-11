using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

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

        public CCrSc_3_I_LIPS(float fh, float fb, float fc_lip, float ft, Color color_temp)
        {
            Name = "I " + (fh * 1000).ToString() + (ft * 1000).ToString();
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
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices - distinguished colors of member surfaces
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color or member
            loadCrScIndices();

            // Wireframe Indices
            loadCrScWireFrameIndicesFrontSide();
            loadCrScWireFrameIndicesBackSide();
            loadCrScWireFrameIndicesLaterals();
        }

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -(float)b /2f;                                    // y
            CrScPointsOut[0, 1] = (float)h / 2f;                                    // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];                             // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];                              // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];                              // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1] - Fc_lip;                     // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0] - m_ft_f;                     // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                              // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];                              // y
            CrScPointsOut[4, 1] = CrScPointsOut[1, 1] - m_ft_f;                     // z

            // Point No. 6
            CrScPointsOut[5, 0] = m_ft_w / 2f;                                      // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];                              // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];                              // y
            CrScPointsOut[6, 1] = -CrScPointsOut[5, 1];                             // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[4, 0];                              // y
            CrScPointsOut[7, 1] = -CrScPointsOut[4, 1];                             // z

            // Point No. 9
            CrScPointsOut[8, 0] = CrScPointsOut[3, 0];                              // y
            CrScPointsOut[8, 1] = -CrScPointsOut[3, 1];                             // z

            // Point No. 10
            CrScPointsOut[9, 0] = CrScPointsOut[2, 0];                              // y
            CrScPointsOut[9, 1] = -CrScPointsOut[2, 1];                             // z

            // Point No. 11
            CrScPointsOut[10, 0] = CrScPointsOut[1, 0];                             // y
            CrScPointsOut[10, 1] = -CrScPointsOut[1, 1];                            // z

            // Point No. 12
            CrScPointsOut[11, 0] = -CrScPointsOut[10, 0];                           // y
            CrScPointsOut[11, 1] = CrScPointsOut[10, 1];                            // z

            // Point No. 13
            CrScPointsOut[12, 0] = -CrScPointsOut[9, 0];                           // y
            CrScPointsOut[12, 1] = CrScPointsOut[9, 1];                            // z

            // Point No. 14
            CrScPointsOut[13, 0] = -CrScPointsOut[8, 0];                           // y
            CrScPointsOut[13, 1] = CrScPointsOut[8, 1];                            // z

            // Point No. 15
            CrScPointsOut[14, 0] = -CrScPointsOut[7, 0];                           // y
            CrScPointsOut[14, 1] = CrScPointsOut[7, 1];                            // z

            // Point No. 16
            CrScPointsOut[15, 0] = -CrScPointsOut[6, 0];                           // y
            CrScPointsOut[15, 1] = CrScPointsOut[6, 1];                            // z

            // Point No. 17
            CrScPointsOut[16, 0] = -CrScPointsOut[5, 0];                           // y
            CrScPointsOut[16, 1] = CrScPointsOut[5, 1];                            // z

            // Point No. 18
            CrScPointsOut[17, 0] = -CrScPointsOut[4, 0];                           // y
            CrScPointsOut[17, 1] = CrScPointsOut[4, 1];                            // z

            // Point No. 19
            CrScPointsOut[18, 0] = -CrScPointsOut[3, 0];                           // y
            CrScPointsOut[18, 1] = CrScPointsOut[3, 1];                            // z

            // Point No. 20
            CrScPointsOut[19, 0] = -CrScPointsOut[2, 0];                           // y
            CrScPointsOut[19, 1] = CrScPointsOut[2, 1];                            // z
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection();

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
            TriangleIndicesBackSide = new Int32Collection();

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


