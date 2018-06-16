//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Collections;
//using CRSC;

//namespace CENEX
//{
//    // Class CLine
//    [Serializable]
//    public class CLine
//    {

//        public int m_iLine_ID,
//                     m_fTime;
//        public CNode m_iNode1 = new CNode();
//        public CNode m_iNode2 = new CNode();

//        public CCrSc m_CrSc = new CCrSc();

//        public CMembRelease m_iMR;

//        // Konstruktor 1 CLine 
//        public CLine(){ }
//        // Konstruktor 2 CLine
//        public CLine(
//            int iLine_ID,
//            CNode iNode1,
//            CNode iNode2,
//            int fTime
//            )
//        {
//            m_iLine_ID = iLine_ID;
//            m_iNode1 = iNode1;
//            m_iNode2 = iNode2;
//            m_fTime = fTime;
//        }
//        // Konstruktor 3 CLine
//        public CLine(
//    int iLine_ID,
//    CNode iNode1,
//    CNode iNode2,
//    CCrSc objCrSc,
//    int fTime
//    )
//        {
//            m_iLine_ID = iLine_ID;
//            m_iNode1 = iNode1;
//            m_iNode2 = iNode2;
//            m_CrSc = objCrSc;
//            m_fTime = fTime;
//        }


//    } // End of Class CLine

//    // Objekt, ktery porovnava Lines podle ID
//    public class CCompare_LineID : IComparer
//    {
//        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
//        public int Compare(object x, object y)
//        {
//            return ((CLine)x).m_iLine_ID - ((CLine)y).m_iLine_ID;
//        }
//    }
//}
