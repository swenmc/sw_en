using System;

namespace CRSC
{
    // TODO ZJEDNOTIT S CCRSC_TW

    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_GE : CCrSc
    {
        // General Thin-walled Cross-section
        // onlz c/t fields are available
        //----------------------------------------------------------------------------
        private float m_ft_max; // Maximum Thickness / Maximalna hrubka elementu
        private float m_ft_min; // Mimimum Thickness  / Minimalna hrubka elementu
        private float[,] m_CrScKeyPoint; // Array of Nodes / y-Coordinate, z-Coordinate, t-Thickness
        private float[,] m_CrScElem; // Array of Elements / Start Node, End Node, Thickness at the Start, Thickness at the End, Element Support Type (Both Side / One Side / Tube / Curved / Flat / Circle / None)
        private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing

        private float m_fc_El;   // Element Length / Delka Elementu
        public  float[][,] m_CrScPoint; // Array of Points and values for Drawing in 2D
        //----------------------------------------------------------------------------

        
        public float Ft_max
        {
            get { return m_ft_max; }
            set { m_ft_max = value; }
        }
        public float Ft_w
        {
            get { return m_ft_min; }
            set { m_ft_min = value; }
        }
        public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_GE()  {   }
        public CCrSc_GE(float[,] arrCrScKeyPoint, float[,] arrCrScElem)
        {
            m_CrScKeyPoint = arrCrScKeyPoint;
            m_CrScElem = arrCrScElem;

            // Create Array - allocate memory
            // m_CrScPoint = new float [][,];

            // Fill Array Data
            CalcCrSc_Coord();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)


            /*
            
              
                                  S1 [y,z]                                                  E1 [y,z]
             
             
                            _|/____     __________________________________________________        ____|/
                            /|         |                                                  |          /|         
                      t_S    | S [y,z] |___ _ ____ _ ____ _ ____ _ ____ _ ____ _ ____ _ __|       t_E |  E [y,z]
                             |         |                                                  |           |   
                            _|/____    |__________________________________________________|       ____|/
                            /|                                                                       /|
             
                                S2  [y,z]                                                   E2 [y,z]                                                                              
              
                                                          c - Length of Element
                                        |/_________________________________________________|/                                                                                
                                       /|                                                 /|
             
                  
                                                                                                
                                                                                            
                                                                                              

             Popis

             Kazdy element ma 2 riadiace body urcujuce zaciatok (S) a koniec (E), tieto uzly teda udavaju jeho dlzku a orientaciu v suradnicovom systeme prierezu (lokalny)
             Pre zohladnenie rozmeru elementu su definovane dalsie 4 body, 2 na zaciatku (S1, S2), 2 na konci (E1, E2). Umiestnene su symetricky voci ose elementu. 
             Ich vzialenost od riadiaceho uzla v suradnicovom systeme elementu je polovica hrubky na zaciatku resp. na konci.
             To znamena ze element moze mat pri konstantnej hrubke pravouhly tvar - stvorec / obdlznik, zakriveny tvar - vysec medzikruzia, pripadne tvar kruhovej trubky
             Element s rozdielnymi hodnotami hrubky na zaciatku a na konci moze mat tvar lichobeznika, trojuholnika, a vysece medzikruzia s meniacim sa priecnym rozmerom.
             
             Elementy so zakrivenim nebudeme momentalne riesit !!!
            
             Pre najjednoduchsie zobrazenie elementov prierezu je mozne uvazovat pole obdlznikov , nezohladnujuc prekryvanie sa hran na koncoch.
             Pre detailne zobrazenie je potrebne pre elementy, ku ktorym sa pripaju na oboch koncoch ine elemnty vykreslovat len pozdlzne hrany,
             pricom suradnice bodov je nutne upravit podla priesecnika s nadvazujucim elementom.
             Pre elementy ktore su na jednej strane nepodoprete / nenadvazuje na ne ziaden iny prvok sa vykreslia 3 hrany.
             
              
              
            
             
            */

















        }

		protected override void loadCrScIndices()
		{
			throw new NotImplementedException();
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
