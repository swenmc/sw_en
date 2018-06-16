using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;
using MATH;
using FEM_CALC_BASE;

namespace FEM_CALC_1Din3D
{
    public class CFemNode : CN
    {
        // Settings
        static int m_iNodeDOFNo = (int)ENDOF.e3DEnv; // 6 DOF in 3D

        // Constructor 1
        public CFemNode()
        {
            m_fVNodeCoordinates = new CVector(m_iNodeDOFNo);
            m_VDisp = new CVector(m_iNodeDOFNo);
            m_ArrNCodeNo = new int[m_iNodeDOFNo];         // Array of global codes numbers
            m_VDirNodeLoad = new CVector(m_iNodeDOFNo);   // Direct external nodal load vector
            m_ArrNodeDOF = new bool[m_iNodeDOFNo];        // Nodal Supports - Node DOF restraints

            // Fill Arrays / Initialize
            Fill_Node_Init();
        }

        // Constructor 2
        public CFemNode(int iNNo)
        {
            m_fVNodeCoordinates = new CVector(m_iNodeDOFNo);
            m_VDisp = new CVector(m_iNodeDOFNo);
            m_ArrNCodeNo = new int[m_iNodeDOFNo];         // Array of global codes numbers
            m_VDirNodeLoad = new CVector(m_iNodeDOFNo);   // Direct external nodal load vector
            m_ArrNodeDOF = new bool[m_iNodeDOFNo];        // Nodal Supports - Node DOF restraints

            // Fill Arrays / Initialize
            Fill_Node_Init();

            ID = iNNo;
        }

        // Constructor 3
        public CFemNode(int iNNo, CVector ArrDisp, CVector VLoad)
        {
            m_fVNodeCoordinates = new CVector(m_iNodeDOFNo);
            m_VDisp = new CVector(m_iNodeDOFNo);
            m_ArrNCodeNo = new int[m_iNodeDOFNo];         // Array of global codes numbers
            m_VDirNodeLoad = new CVector(m_iNodeDOFNo);   // Direct external nodal load vector
            m_ArrNodeDOF = new bool[m_iNodeDOFNo];        // Nodal Supports - Node DOF restraints

            // Fill Arrays / Initialize
            Fill_Node_Init();

            ID = iNNo;
            m_VDisp = ArrDisp;
            m_VDirNodeLoad = VLoad;
        }

        // Constructor 4 - FEM node is copy of topological node
        public CFemNode(CNode TopoNode)
        {
            m_fVNodeCoordinates = new CVector(m_iNodeDOFNo);
            m_VDisp = new CVector(m_iNodeDOFNo);
            m_ArrNCodeNo = new int[m_iNodeDOFNo];         // Array of global codes numbers
            m_VDirNodeLoad = new CVector(m_iNodeDOFNo);   // Direct external nodal load vector
            m_ArrNodeDOF = new bool[m_iNodeDOFNo];        // Nodal Supports - Node DOF restraints

            // Fill Arrays / Initialize
            Fill_Node_Init();

            ID = TopoNode.ID;
            m_fVNodeCoordinates.FVectorItems[(int)e3D_DOF.eUX] = TopoNode.X;
            m_fVNodeCoordinates.FVectorItems[(int)e3D_DOF.eUY] = TopoNode.Y;
            m_fVNodeCoordinates.FVectorItems[(int)e3D_DOF.eUZ] = TopoNode.Z;
            FTime = TopoNode.FTime;
        }

        // Function returns list of FEM 1D elements which includes given node
        // Do we need to store whole elements object (array of elements indexes should be enough) !!!

        public ArrayList GetFoundedMembers(CFemNode Node, CE_1D[] ElemArray, int iElemNo)
        {
            int j = 0;
            ArrayList ElemTempList = new ArrayList();
            
            for (int i = 0; i < iElemNo; i++)
            {
                if ((ElemArray[i].NodeStart == Node) || (ElemArray[i].NodeEnd == Node))
                {
                    ElemTempList.Add(i); // Add Element to Element List
                    j++;
                }
            }

            return ElemTempList;
        }

        // Array of int values
        public ArrayList GetFoundedMembers_Index(CFemNode Node, CE_1D[] ElemArray)
        {
            ArrayList IndexList = new ArrayList();

            for (int i = 0; i < ElemArray.Length; i++) // Check for each member in array
            {
                if ((ElemArray[i].NodeStart == Node) || (ElemArray[i].NodeEnd == Node))
                {
                    IndexList.Add(i); // Add Element to Element Index Array
                }
            }

            return IndexList;
        }

        public ArrayList GetFoundedMembers_IndexOld(CFemNode Node, CE_1D[] ElemArray, int iElemNo)
        {
            int j = 0;
            ArrayList IndexList = new ArrayList();

            for (int i = 0; i < iElemNo; i++)
            {
                if ((ElemArray[i].NodeStart == Node) || (ElemArray[i].NodeEnd == Node))
                {
                    IndexList.Add(i); // Add Element to Element Index Array
                    j++;
                }
            }

            return IndexList;
        }






























    }
}
