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

        //Mato - tento bool by som rozhodne vyhodil prec
        bool bIsDefinedCCW = true; // Pomocny bool  

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

            // Fill list of indices for drawing of surface - triangles edges

            //SOLID MODEL
            // Particular indices - distinguished colors of member surfaces
            //Mato - indexy tvorit tak aby normala smerovala vonku, na CW, CCW prepinac sa treba vykaslat
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color or member            
            // Mato - tato metoda nebude robit nic ine iba vytvori novu kolekciu ktora bude tvorena:
            //list.AddRange(TriangleIndicesFrontSide); list.AddRange(TriangleIndicesBackSide); list.AddRange(TriangleIndicesShell);
            loadCrScIndices(); 

            // Wireframe Indices
            // Mato - Naco potrebujeme wireframe pre Front, Back a Laterals?
            // Ja by som spravil jednu metodu a tieto 3 by som uplne vyhodil
            loadCrScWireFrameIndicesFrontSide();
            loadCrScWireFrameIndicesBackSide();
            loadCrScWireFrameIndicesLaterals();
        }

        //tato metoda by mohla byt pretazena "override" ak je to nutne a ak nie tak metoda v predkovi by na zaklade poctu bodov vytvorila presne tu kolekciu co som rucne tu napisal
        public override void loadCrScWireFrameIndices()
        {
            //kvoli prerformance by som pouzil takto, resp. zadefinovat pre list pocet prvkov, naplnit a nakoniec vytvorit z Listu Int32Collection
            List<int> indices = new List<int>() { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 0, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 8, 0, 8, 1, 9, 2, 10, 3, 11, 4, 12, 5, 13, 6, 14, 7, 15 };
            WireFrameIndices = new Int32Collection(indices);
            
            //WireframeIndices;     0,1,1,2,2,3,3,4,4,5,5,6,6,7,7,0 - front
            //                      8,9,9,10,10,11,11,12,12,13,13,14,14,15,15,8 - back
            //                      0,8,1,9,2,10,3,11,4,12,5,13,6,14,7,15 - shell
        }



        //TODO Mato:
        //Pozicie by som vzdy robil tak ze pre 3_51_C by to boli suradnice bodov 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15
        //TriangleIndices - I don't care, hocijak aby to dobre vykreslilo.

        //WireframeIndices;     0,1,1,2,2,3,3,4,4,5,5,6,6,7,7,0 - front
        //                      8,9,9,10,10,11,11,12,12,13,13,14,14,15,15,8 - back
        //                      0,8,1,9,2,10,3,11,4,12,5,13,6,14,7,15 - shell


        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            if (bIsDefinedCCW)
            {
                // Defined Counter-clockwise
                // Point No. 1
                CrScPointsOut.Add(new Point(b / 2.0, h / 2.0));
                // Point No. 2
                CrScPointsOut.Add(new Point(-CrScPointsOut[0].X, CrScPointsOut[0].Y));
                // Point No. 3
                CrScPointsOut.Add(new Point(CrScPointsOut[1].X, -CrScPointsOut[1].Y));
                // Point No. 4
                CrScPointsOut.Add(new Point(CrScPointsOut[0].X, -CrScPointsOut[0].Y));
                // Point No. 5
                CrScPointsOut.Add(new Point(CrScPointsOut[3].X, CrScPointsOut[3].Y + m_ft_f));
                // Point No. 6
                CrScPointsOut.Add(new Point(CrScPointsOut[1].X + m_ft_w, CrScPointsOut[4].Y));
                // Point No. 7
                CrScPointsOut.Add(new Point(CrScPointsOut[5].X, -CrScPointsOut[5].Y));
                // Point No. 8
                CrScPointsOut.Add(new Point(CrScPointsOut[4].X, -CrScPointsOut[4].Y));
            }
            else
            {
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
                CrScPointsOut.Add(new Point(- b / 2.0, CrScPointsOut[4].Y));
                // Point No. 7
                CrScPointsOut.Add(new Point(CrScPointsOut[5].X, -CrScPointsOut[5].Y));
                // Point No. 8
                CrScPointsOut.Add(new Point(CrScPointsOut[4].X, -CrScPointsOut[4].Y));
            }
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection(3*6);

            if (bIsDefinedCCW)
            {
                AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 0, 1, 6, 7);
                AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 1, 2, 5, 6);
                AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 2, 3, 4, 5);
            }
            else
            {
                AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 1, 6, 7);
                AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 1, 2, 5, 6);
                AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 2, 3, 4, 5);
            }
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection(3*6);

            if (bIsDefinedCCW)
            {
                AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 6, ITotNoPoints + 7);
                AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 5, ITotNoPoints + 6);
                AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 2, ITotNoPoints + 3, ITotNoPoints + 4, ITotNoPoints + 5);
            }
            else
            {
                AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 6, ITotNoPoints + 7);
                AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 5, ITotNoPoints + 6);
                AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 2, ITotNoPoints + 3, ITotNoPoints + 4, ITotNoPoints + 5);
            }
        }
    }
}