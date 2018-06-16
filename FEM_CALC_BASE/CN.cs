using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using System.Collections;

namespace FEM_CALC_BASE
{
    public class CN
    {
        int m_ID; // Unique FEM node ID

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        float fTime;

        public float FTime
        {
            get { return fTime; }
            set { fTime = value; }
        }

        /*
        // Vector of node coordinates in carthesian coordinate system
        public CVector m_fVNodeCoordinates = new CVector(m_iNodeDOFNumber);
        public CVector m_VDisp = new CVector(m_iNodeDOFNumber);
        public int[] m_ArrNCodeNo = new int[m_iNodeDOFNumber];        // Array of global codes numbers
        public CVector m_VDirNodeLoad = new CVector(m_iNodeDOFNumber);  // Direct external nodal load vector
        public bool[] m_ArrNodeDOF = new bool[m_iNodeDOFNumber];        // Nodal Supports - Node DOF restraints
        public ArrayList m_iMemberCollection; // List / Collection of FEM members connected to the node
        */

        // Vector of node coordinates in carthesian coordinate system
        public CVector m_fVNodeCoordinates;
        public CVector m_VDisp;
        public int[] m_ArrNCodeNo;
        public CVector m_VDirNodeLoad;
        public bool[] m_ArrNodeDOF;
        public ArrayList m_iMemberCollection; // List / Collection of FEM members connected to the node

        public CN()
        {

        }

        /////////////////////////////////////////////////////////////////
        // Auxiliary operations

        // Detaulf values of Vectors (Array)

        // Nodal displacement
        public void Fill_VDisp_Init()
        {
            for (int i = 0; i < m_VDisp.FVectorItems.Length; i++)
                m_VDisp.FVectorItems[i] = float.PositiveInfinity;
        }

        public void Fill_NCode_Init()
        {
            foreach (int i in m_ArrNCodeNo)
              m_ArrNCodeNo[i] = int.MaxValue;
        }

        public void Fill_ArrNodeDOF_Init()
        {
            for (int i = 0; i < m_ArrNodeDOF.Length; i++)
              m_ArrNodeDOF[i] = false;
        }

        // Direct nodal loads in GCS
        public void Fill_VDirNodeLoad_Init()
        {
            for (int i = 0; i < m_VDirNodeLoad.FVectorItems.Length; i++)
              m_VDirNodeLoad.FVectorItems[i] = 0f;
        }

        public void Fill_Node_Init()
        {
            Fill_VDisp_Init();
            Fill_ArrNodeDOF_Init();
            Fill_VDirNodeLoad_Init();
        }
    }
}
