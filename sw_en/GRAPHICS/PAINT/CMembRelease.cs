//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace CENEX
//{
//    // Class CMembRelease
//    [Serializable]
//    public class CMembRelease
//    {
//        /*
//         0 - release at both nodes of member,
//         1 - release at start node, 
//         2 - release at end node, 
//         3 - no release at member
//        */
//        public int m_iNodeCode; 
//        public bool[] m_bRestrain; // DOF
//        public int m_fTime;



//        // Konstruktor CMembRelease
//        public CMembRelease(
//              int iNodeCode,
//              bool[] bRestrain,
//              int fTime)
//        {
//            m_iNodeCode = iNodeCode;
//            m_bRestrain = bRestrain;
//            m_fTime = fTime;
//        }
//    }
//}
