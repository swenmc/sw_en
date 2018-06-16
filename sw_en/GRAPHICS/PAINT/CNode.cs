//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Collections;

//namespace CENEX
//{
//    // Class CNode
//    [Serializable]
//    public class CNode
//    {
//        public int m_iNode_ID,
//                     m_fCoord_X,
//                     m_fCoord_Y,
//                     m_fCoord_Z,
//                     m_fTime;

//        // Konstruktor1 CNode
//        public CNode()
//        {

//        }
//        // Konstruktor2 CNode
//        public CNode(
//            int iNode_ID,
//            int fCoord_X,
//            int fCoord_Y,
//            int fCoord_Z,
//            int fTime
//            //,
//            //int fNodeSizeX,
//            //int fNodeSizeZ
//            )
//        {
//            m_iNode_ID = iNode_ID;
//            m_fCoord_X = fCoord_X;
//            m_fCoord_Y = fCoord_Y;
//            m_fCoord_Z = fCoord_Z;
//            m_fTime = fTime;
//            //m_fNodeSizeX  =  fNodeSizeX;
//            //m_fNodeSizeZ  =  fNodeSizeZ;
//        }

//    } // End of Class CNode

//    // Objekt, ktery porovnava Nodes podle ID
//    public class CCompare_NodeID : IComparer
//    {
//        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
//        public int Compare(object x, object y)
//        {
//            return ((CNode)x).m_iNode_ID - ((CNode)y).m_iNode_ID;
//        }
//    }
//}
