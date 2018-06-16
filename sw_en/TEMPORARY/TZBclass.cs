using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CENEX
{
    class TZBclass
    {

        // TZB CALCULATION

        // Dimensions 

        // variables

        // Geometria

        double _A;

        public double A
        {
            get { return _A; }
            set { _A = value; }
        }
        double _B;

        public double B
        {
            get { return _B; }
            set { _B = value; }
        }
        double _C;

        public double C
        {
            get { return _C; }
            set { _C = value; }
        }
        double _D;

        public double D
        {
            get { return _D; }
            set { _D = value; }
        }
        double _E;

        public double E
        {
            get { return _E; }
            set { _E = value; }
        }
        double _F;

        public double F
        {
            get { return _F; }
            set { _F = value; }
        }
        double _G;

        public double G
        {
            get { return _G; }
            set { _G = value; }
        }
        double _R;

        public double R
        {
            get { return _R; }
            set { _R = value; }
        }
        double _Uhol;

        public double Uhol
        {
            get { return _Uhol; }
            set { _Uhol = value; }
        }
        double _O1; // obvod 1

        public double O1
        {
            get { return _O1; }
            set { _O1 = value; }
        }
        double _L; // dlzka

        public double L
        {
            get { return _L; }
            set { _L = value; }
        }

        // Pocet spojov

        double _p_Spoj;

        //Pridavok
        double _P;
        //Obvod 1 + Pridavok
        double _O1P;


        // CENNIK

        // cennik spravime v databaze.mdb, teraz len na ukazku
        double _PZ_0_6 = 5; // Pozinkovaný plech PZ 0,6
        double _PZ_0_8 = 7.3;//Pozinkovaný plech PZ 0,8
        double _PZ_1_0 = 9.15;//Pozinkovaný plech PZ 1,0
        double _PN_0_6 = 25.24;//Nerezový plech 0,6 

        double _PrLi_S20 = 0.9512; // Prirubova lista S20
        double _PrLi_S30 = 1.1488; // Prirubova lista S30
        double _Roh_P20_3 = 0.1565; // Rohovnik P20/3
        double _Roh_P30_3 = 0.3253; // Rohovnik P30/3

        double _r_chs_3_8inch = 2.1412; // Rurka 3/8"
        double _str_vyz = 0.7082; // stred vystuhy
        double _hmozdi = 0.6094; // hmozdinka
        double _tanier = 0.2841; // tanier
        double _skrutka = 0.0371; // skrutka

        double _vyztuha = 4.4306; // vyztuha 1 kpl
        double _prych_QUICK = 0.1606; // prichytka nabehovych plechov QUICK
        double _vyztuha_np = 1.6306; // vystuha nabehovych plechov
















        // premenne pocty
        double _p_S20; // počet S20
        double _p_S30; // počet S30
        int _p_P20_3; // počet P20/3
        int _p_P30_3; // počet P30/3
        int _p_V; // počet V

        // premenne ceny
        double _c_P; // cena plech

        double _c_S20; // cena S20
        double _c_S30; // cena S30
        double _c_P20_3; // cena P20/3
        double _c_P30_3; // cena P30/3
        double _c_V; // cena V

        double _c_Mat; // cena material
        double _c_Vyr; // cena vyroba
        double _c_Tot; // Cena spolu

        // Pracnost

        double _c_PJ; // Jednotkova pracnost




        

        // METODY VYPOCTU PRE JEDNOTLIVE TVARY

        public void RU (double _A, double _B, double _L)
        {                       
            if (_L <= 600)
            {
                // napisat vzorce co plati pre TVAR RU ak je L mensie ako 600
                // obvod s pridavkami
                _O1P = (_O1 + (_p_Spoj * 0.03)) * 1.2;
                _O1 = (_A * _B) * 2; // mm

                // pocet poloziek
                _p_S20 = _O1 * 2;
                _p_P20_3 = 8;

                //ceny poloziek
                _c_P = _O1P * _PZ_0_6 * _L;
                _c_P20_3 = _p_P20_3 * _Roh_P20_3;
                //_c_P30_3 = _p_P30_3 * _Roh_P30_3;
                _c_S20 = _p_S20 * _PrLi_S20;
                //_c_S30 = _p_S30 * _PrLi_S30;
                //_c_V = _p_V * _vyztuha;

                // Ceny vysledne
                _c_PJ = 1.33; // jednotkova cena pracnosti
                _c_Tot = _c_Mat + _c_Vyr;
                _c_Vyr = (1.5 + _p_Spoj) * _c_PJ;
                _c_Mat = _c_P + _c_P20_3 + _c_P30_3 + _c_S20 + _c_S30 + _c_V;
                                
            }
            else if (_L > 600 && _L <= 1000)
            {

                // napisat vzorce co plati pre TVAR RU ak je L je 600-1000 mm


            }
            else if (_L > 1000)
            {

                // napisat vzorce co plati pre TVAR RU ak je L >1000 mm


            }





        }
        public void OB(double _A, double _B, double _R, double _Uhol)
        {


        }

        public void OD(double _A, double _B, double _C, double _D, double _E, double _F, double _G, double _L)
        {


        }

        public void RK(double _A, double _B)
        {


        }




    }
}
