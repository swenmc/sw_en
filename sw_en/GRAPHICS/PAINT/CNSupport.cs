//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections;

//namespace CENEX
//{
//    // Degrees of freedom 0-5 (6 - warping)
//    public enum EDOF
//    {
//        eUX = 0, // Displacement in X-Direction
//        eUY = 1, // Displacement in Y-Direction
//        eUZ = 2, // Displacement in Z-Direction
//        eRX = 3, // Rotation around X-Axis
//        eRY = 4, // Rotation around Y-Axis
//        eRZ = 5, // Rotation around Z-Axis
//        eW = 6  // Warping (not implemented yet)
//    }

//    // Class CNSupport
//    [Serializable]
//    public class CNSupport
//    {
//        public int   m_iSupport_ID;
//        public CNode m_iNode = new CNode();
//        public bool  [] m_bRestrain;
//        public int   m_fTime;
 


                       

//        // Konstruktor CNSupport
//        public CNSupport(
//              int iSupport_ID, 
//              CNode iNode,
//              bool  [] bRestrain,
//              int fTime
//            )
//        {
//            m_iSupport_ID = iSupport_ID;
//            m_iNode = iNode;
//            m_bRestrain = bRestrain;
//            m_fTime = fTime;
//        }
//    } // End of Class CNSupport

//    // Objekt, ktery porovnava Supports podle ID
//    public class CCompare_NSupportID : IComparer
//    {
//        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
//        public int Compare(object x, object y)
//        {
//            return ((CNSupport)x).m_iSupport_ID - ((CNSupport)y).m_iSupport_ID;
//        }
//    }
//}
