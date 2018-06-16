using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    class CH_EC5_1101
    {
        // vstup   - (trieda, struktura ??? ) potrebne nacitat len vstupne parametre (prierez, material atd)
        // vypocet - zavolat funkcie z _1_EQ resp aj _2_CL a naplnit ich premennymi
        // vystup  - vlozit vypocitane data do triedy (struktury ???) ktora sa bude spolu so vstupmi zobrazovat v tabulke dialogu

//----------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------

        double dsigma;
        double dN_Ed;
        double dA;

        

        public double Dsigma
        {
            get { return dsigma; }
        }
        
        public double DN_Ed
        {
            get { return dN_Ed; }
        }
        public double DA
        {
            get { return dA; }
            set { dA = value; }
        }

        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        public CH_EC5_1101() 
        {
            dN_Ed = 7; //nacitanie z databazy

            dA = 4;

            CL_61 obCL_61 = new CL_61();
            dsigma = obCL_61.Eq_61______(dN_Ed, dA);
            
            
        }

        public override string ToString()
        {
            return "dsigma: " + dsigma + ", dN_Ed: " + dN_Ed + ", dA: " + dA;
        }
        
    }
}
