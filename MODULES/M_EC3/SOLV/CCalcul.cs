using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    public class CCalcul
    {
        // Global Model Data
        // posunut este vyssie

        C_MAT m_objMat = new C_MAT();
        C_IFO m_objIFO = new C_IFO();
        C_NAD m_objNAD = new C_NAD(); // doriesit konstruktor a naplnenie Mat
        CCrSc m_objCrSc = new CCrSc();

        ECrScShType1 m_eCrScType;
        ECrScSymmetry1 m_eSym;
        ECrScPrType1 m_eProd;

        public CCalcul()
        {
            // Basic Cross-section Data and Classification
            // s niektorymi objektami potrebujem pracovat aj dalej, takze by to malo byt inak :-/
            // Mat jednu triedu, vytvarat jej objekt ako clensky v tejto triede a volat rozne konštruktory podla ECrScShType ???- zbytocne vela premennych - rozne typy maju rozne dáta


            switch (m_eCrScType)
            {
                case ECrScShType1.eCrScType_I: // I and H - section
                    {
                        C_GEO___I objGeo = new C_GEO___I(m_eSym, m_eProd);
                        C_MAT___I objMat = new C_MAT___I(objGeo, m_eSym);
                        C_ADD___I objAdd = new C_ADD___I(m_objNAD, objGeo, objMat, m_objCrSc, m_eSym, m_eProd);
                        C_STR___I objStr = new C_STR___I(m_objIFO, objGeo, m_objCrSc, m_eSym);
                        C___I objClass = new C___I(objGeo, objMat,m_objIFO,m_objNAD,objStr,m_eSym,m_eProd);

                        if (objClass.m_iClass == 4)
                        {
                            C___I4 objC___I4 = new C___I4(m_objCrSc, objGeo, m_objIFO, objStr, objClass, objMat, objAdd, m_eSym);
                        }

                        break;
                    }
                case ECrScShType1.eCrScType_C:  // C and U (channel) - section
                    {
                        C_GEO___U objGeo = new C_GEO___U(m_eProd);
                        C_MAT___U objMat = new C_MAT___U(objGeo, m_eSym);
                        C_ADD___U objAdd = new C_ADD___U(m_objNAD, objGeo, objMat, m_objCrSc, m_eProd);
                        C_STR___U objStr = new C_STR___U(m_objIFO, objGeo, m_objCrSc);
                        C___U objClass = new C___U(objGeo, objMat, m_objIFO, m_objNAD, objStr);

                        if (objClass.m_iClass == 4)
                        {
                            C___U4 objC___U4 = new C___U4(m_objCrSc, objGeo, m_objIFO, objStr, objClass, objMat, objAdd);
                        }

                        break;
                    }
                case ECrScShType1.eCrScType_L:   // L (angle) - section , equal and unequal
                    {
                        C_GEO___L objGeo = new C_GEO___L(m_eSym, m_eProd);
                        C_MAT___L objMat = new C_MAT___L(objGeo, m_eSym);
                        C_ADD___L objAdd = new C_ADD___L(m_objNAD, objGeo, objMat, m_objCrSc, m_eSym, m_eProd);
                        C_STR___L objStr = new C_STR___L(m_objIFO, objGeo, m_objCrSc, m_eSym);
                        C___L objClass = new C___L(objGeo, objMat, objStr);

                        if (objClass.m_iClass == 4)
                        {
                            C___L4 objC___L4 = new C___L4(m_objCrSc, objGeo, m_objIFO, objStr, objClass, objMat, objAdd, m_eSym);
                        }

                        break;
                    }
                case ECrScShType1.eCrScType_T:   // T - section
                    {
                        /*
                        C_GEO___T objGeo = new C_GEO___I(m_eSym, m_eProd);
                        C_MAT___T objMat = new C_MAT___I(objGeo, m_eSym);
                        C_ADD___I objAdd = new C_ADD___I(m_objNAD, objGeo, objMat, m_objCrSc, m_eSym, m_eProd);
                        C_STR___I objStr = new C_STR___I(m_objIFO, objGeo, m_objCrSc, m_eSym);
                        C___I objC___I = new C___I(objGeo, objMat, m_objIFO, m_objNAD, objStr, m_eSym, m_eProd);

                        if (objC___I.m_iClass == 4)
                        {
                            C___I4 objC___I4 = new C___I4(m_objCrSc, objGeo, m_objIFO, objStr, objC___I, objMat, objAdd, m_eSym);
                        }
                        */

                        break;
                    }
                case ECrScShType1.eCrScType_HL:   // HoLLow / box - section, hollow - section (square and rectangular) 
                    {
                        C_GEO__HL objGeo = new C_GEO__HL(m_eProd);
                        C_MAT__HL objMat = new C_MAT__HL(objGeo, m_eProd);
                        C_ADD__HL objAdd = new C_ADD__HL(m_objNAD, objGeo, objMat, m_objCrSc, m_eProd);
                        C_STR__HL objStr = new C_STR__HL(m_objIFO, objGeo, m_objCrSc, m_eProd);
                        C__HL objClass = new C__HL(objGeo, objMat, m_objIFO, m_objNAD, objStr, m_eSym, m_eProd);

                        if (objClass.m_iClass == 4)
                        {
                            C__HL4 objC__HL4 = new C__HL4(m_objCrSc, objGeo, m_objIFO, objStr, objClass, objMat, objAdd, m_eProd);
                        }

                        break;
                    }
                case ECrScShType1.eCrScType_FB:   // Flat Bar
                    {
                        /*
                        C_GEO___I objGeo = new C_GEO___I(m_eSym, m_eProd);
                        C_MAT___I objMat = new C_MAT___I(objGeo, m_eSym);
                        C_ADD___I objAdd = new C_ADD___I(m_objNAD, objGeo, objMat, m_objCrSc, m_eSym, m_eProd);
                        C_STR___I objStr = new C_STR___I(m_objIFO, objGeo, m_objCrSc, m_eSym);
                        C___I objC___I = new C___I(objGeo, objMat, m_objIFO, m_objNAD, objStr, m_eSym, m_eProd);

                        if (objC___I.m_iClass == 4)
                        {
                            C___I4 objC___I4 = new C___I4(m_objCrSc, objGeo, m_objIFO, objStr, objC___I, objMat, objAdd, m_eSym);
                        }
                         */
                        break;
                    }
                case ECrScShType1.eCrScType_RB:   // Round Bar
                    {
                        /*
                        C_GEO___I objGeo = new C_GEO___I(m_eSym, m_eProd);
                        C_MAT___I objMat = new C_MAT___I(objGeo, m_eSym);
                        C_ADD___I objAdd = new C_ADD___I(m_objNAD, objGeo, objMat, m_objCrSc, m_eSym, m_eProd);
                        C_STR___I objStr = new C_STR___I(m_objIFO, objGeo, m_objCrSc, m_eSym);
                        C___I objC___I = new C___I(objGeo, objMat, m_objIFO, m_objNAD, objStr, m_eSym, m_eProd);

                        if (objC___I.m_iClass == 4)
                        {
                            C___I4 objC___I4 = new C___I4(m_objCrSc, objGeo, m_objIFO, objStr, objC___I, objMat, objAdd, m_eSym);
                        }*/

                        break;
                    }
                case ECrScShType1.eCrScType_TU:   // TUbe
                    {
                        C_GEO__TU objGeo = new C_GEO__TU(m_eProd);
                        C_MAT__TU objMat = new C_MAT__TU(objGeo);
                        C_ADD__TU objAdd = new C_ADD__TU(m_objNAD, objGeo, objMat, m_objCrSc, m_eProd);
                        C_STR__TU objStr = new C_STR__TU(m_objIFO, objGeo, m_objCrSc);
                        C__TU objC__TU = new C__TU(objGeo, objMat, m_objIFO, m_objNAD, objStr);

                        if (objC__TU.m_iClass == 4)
                        {
                          //Error
                        }

                        break;
                    }
                default:
                    {


                        break;
                    }
            }


            // Cross-section Design



            // Stability Design


            // Nasleduje if / else pre urcenie spravneho posudku ktory sa ma vytvorit
            // objekty CH_10000 - CH_90000






        }
    }
}
