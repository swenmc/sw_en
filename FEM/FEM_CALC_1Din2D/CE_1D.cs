using System;
using System.Collections;
using BaseClasses;
using FEM_CALC_BASE;
using MATH;
using CRSC;

namespace FEM_CALC_1Din2D
{
    public class CE_1D : CE_1D_BASE
    {
        // Geometrical properties of Element
        public float m_fLength_X, m_fLength_Y, m_frotation_angle = 0f;

        // Primary End Forces Vectors
        // Vector of member nodes (ends) primary forces in Local Coordinate System (LCS) due to the transverse member load
        public CVector m_VElemPEF_LCS_StNode = new CVector(Constants.i2D_DOFNo);  // Start Node
        public CVector m_VElemPEF_LCS_EnNode = new CVector(Constants.i2D_DOFNo);  // End Node

        // Vector of member nodes (ends) primary forces in Global Coordinate System due to the transverse member load
        public CVector m_VElemPEF_GCS_StNode = new CVector(Constants.i2D_DOFNo);  // Start Node
        public CVector m_VElemPEF_GCS_EnNode = new CVector(Constants.i2D_DOFNo);  // End Node

        // Results End Forces Vectors
        // Vector of member nodes (ends) forces in GCS
        public CVector m_VElemEF_GCS_StNode = new CVector(Constants.i2D_DOFNo);  // Start Node
        public CVector m_VElemEF_GCS_EnNode = new CVector(Constants.i2D_DOFNo);  // End Node

        // Vector of member nodes (ends) forces in LCS
        public CVector m_VElemEF_LCS_StNode = new CVector(Constants.i2D_DOFNo);  // Start Node
        public CVector m_VElemEF_LCS_EnNode = new CVector(Constants.i2D_DOFNo);  // End Node

        // Vector of member nodes (ends) internal forces in LCS
        public CVector m_VElemIF_LCS_StNode = new CVector(Constants.i2D_DOFNo);  // Start Node
        public CVector m_VElemIF_LCS_EnNode = new CVector(Constants.i2D_DOFNo);  // End Node

        // 2D Matrices 
        public CMatrix m_fkLocMatr = new CMatrix(Constants.i2D_DOFNo);   // 3x3
        public CMatrix m_fATRMatr2D = new CMatrix(Constants.i2D_DOFNo);  // 3x3
        public CMatrix m_fBTTMatr2D = new CMatrix(Constants.i2D_DOFNo);  // 3x3
        public CMatrix m_fKGlobM;     // (2x3)*(2x3)

        public CCrSc m_CrSc /*= new CCrSc()*/; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public CMLoad m_ELoad;
        // 2D support Types - 3-dimensional array for UXRX, UYRZ, UZRY
        public FEM_CALC_BASE.Enums.EElemSuppType2D [] m_eSuppType;

        float m_GCS_X = 0f;
        float m_GCS_Y = 0f;

        float m_fAlpha = 0f;
        float m_fSinAlpha, m_fCosAlpha;

        // Constructor

        public CE_1D()
        {
            // Fill Arrays / Initialize
            Fill_EEndsLoad_Init();
        }
        public CE_1D(CMember mMember, CFemNode NStart, CFemNode NEnd)
        {
            // Main member or segment
            Member = mMember; 

            // Nodes
            NodeStart = NStart;
            NodeEnd   = NEnd;

            // Cross-section
            m_CrSc = Member.CrScStart;

            FillBasic2_GeomMatrices();

        } // End of constructor

        // Constructor - FEM Member is copy of topological member or segment
        public CE_1D(CMember TopoMember, CFemNode[] arrFemNodes)
        {
            // Main member or segment
            Member = TopoMember;
            ID = TopoMember.ID; // Temporary - TopoMember ID is same as FemMember
            // Nodes
            //m_NodeStart.CopyTopoNodetoFemNode(m_Member.INode1);
            //m_NodeEnd.CopyTopoNodetoFemNode(m_Member.INode2);

            // Nodes - temporary - nodes of Topo and Fem model are same
            // Search FEM nodes
            for (int i = 0; i < arrFemNodes.Length; i++)
            {
                if( Member.NodeStart.ID == arrFemNodes[i].ID)
                NodeStart = arrFemNodes[i];
            }

            for (int i = 0; i < arrFemNodes.Length; i++)
            {
                if (Member.NodeEnd.ID == arrFemNodes[i].ID)
                    NodeEnd = arrFemNodes[i];
            }

            // Cross-section
            m_CrSc = TopoMember.CrScStart;

            FillBasic2_GeomMatrices();
        } // End of constructor

        public void FillBasic2_GeomMatrices()
        {
            // Element / Member load

 

            // Lengths in Global Coordinates
            m_fLength_X = GetGCSLengh(0);
            m_fLength_Y = GetGCSLengh(1);

            // FEM Element Length
            FLength = MathF.Sqrt(MathF.Pow2(m_fLength_X) + MathF.Pow2(m_fLength_Y));

            // Temporary !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Member.FLength = FLength;

            // Calculate rotation of meber - clockwise system
            m_fAlpha = GetGCSAlpha();

            //m_fAlpha = GetGCSAlpha(1);
            m_fSinAlpha = (float)Math.Sin(m_fAlpha);
            m_fCosAlpha = (float)Math.Cos(m_fAlpha);

            // 2D
            // Transformation Matrix of Element Rotation - 2D
            m_fATRMatr2D.m_fArrMembers = new float[Constants.i2D_DOFNo, Constants.i2D_DOFNo]
            {
            {  m_fCosAlpha,     m_fSinAlpha,    0f },
            { -m_fSinAlpha,     m_fCosAlpha,    0f },
            {           0f,              0f,    1f }
            };


            // Transformation Transfer Matrix - 2D
            m_fBTTMatr2D.m_fArrMembers = new float[Constants.i2D_DOFNo, Constants.i2D_DOFNo]
            {
            {           -1f,              0f,    0f },
            {            0f,             -1f,    0f },
            {  -m_fLength_Y,     m_fLength_X,   -1f }
            };
        }

        public void FillBasic3_StiffMatrices()
        {
            // Get Element support type
            // Depends on nodal support and element releases
            m_eSuppType = Get_iElemSuppType();

            // Get local matrix acc. to end support/restraint of element
            GetLocMatrix_2D();

            // Check of partial matrices members

            // Partial matrices of global matrix of member 3 x 3
            //if(bDebug)
            //Console.WriteLine(m_fkLocMatr.Print2DMatrix());

            // Return partial matrixes and global matrix of FEM element 6 x 6 (2*3x2*3) 2D

            m_fKGlobM = new CMatrix(
            GetPartM_k11(m_fkLocMatr, m_fATRMatr2D),
            GetPartM_k12(m_fkLocMatr, m_fATRMatr2D, m_fBTTMatr2D),
            GetPartM_k21(m_fkLocMatr, m_fATRMatr2D, m_fBTTMatr2D),
            GetPartM_k22(m_fkLocMatr, m_fATRMatr2D, m_fBTTMatr2D)
            );
        }

        public void Fill_ECode_Init()
        {
           // Start Node
           NodeStart.m_ArrNCodeNo[(int)e2D_DOF.eUX] = int.MaxValue;
           NodeStart.m_ArrNCodeNo[(int)e2D_DOF.eUY] = int.MaxValue;
           NodeStart.m_ArrNCodeNo[(int)e2D_DOF.eRZ] = int.MaxValue;

           // End Node
           NodeEnd.m_ArrNCodeNo[(int)e2D_DOF.eUX] = int.MaxValue;
           NodeEnd.m_ArrNCodeNo[(int)e2D_DOF.eUY] = int.MaxValue;
           NodeEnd.m_ArrNCodeNo[(int)e2D_DOF.eRZ] = int.MaxValue;
        }

        public void Fill_EEndsLoad_Init()
        {
            // Start Node
            m_VElemPEF_LCS_StNode.FVectorItems[(int)e2D_E_F.eFX] = 0f;
            m_VElemPEF_LCS_StNode.FVectorItems[(int)e2D_E_F.eFY] = 0f;
            m_VElemPEF_LCS_StNode.FVectorItems[(int)e2D_E_F.eMZ] = 0f;

            // End Node
            m_VElemPEF_LCS_EnNode.FVectorItems[(int)e2D_E_F.eFX] = 0f;
            m_VElemPEF_LCS_EnNode.FVectorItems[(int)e2D_E_F.eFY] = 0f;
            m_VElemPEF_LCS_EnNode.FVectorItems[(int)e2D_E_F.eMZ] = 0f;
        }

        public float GetGCSLengh(float fCoordStart, float fCoordEnd, float fGCSCoord)
        {
            // Priebezne
            if (
                ((fCoordEnd >= fGCSCoord) && (fCoordStart >= fGCSCoord)) || // Both positive
                 (fCoordEnd <= fGCSCoord && fCoordStart <= fGCSCoord) // Both negative
               )
            {
                return fCoordEnd - fCoordStart;  // if (fCoordEnd > fCoordStart) Positive length / if (fCoordStart > fCoordEnd) Negative length
            }
            else if (fCoordEnd <= fGCSCoord && fCoordStart >= fGCSCoord) // Start positive / End negative
            {
                return fCoordEnd - fCoordStart; // Negative length
            }
            else if (fCoordStart <= fGCSCoord && fCoordEnd >= fGCSCoord) // Start negative / End positive
            {
                return fCoordEnd - fCoordStart; // Positive length
            }
            else // Exception
            { 
                return 0.0f;
            }
        }

        public float GetGCSLengh(int i)
        {
            // global coordinate system direction
            // 0 - global x-axis
            // 1 - global y-axis
            switch (i)
            {
                case 0:
                    return GetGCSLengh(NodeStart.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUX], NodeEnd.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUX], m_GCS_X);
                case 1:
                    return GetGCSLengh(NodeStart.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUY], NodeEnd.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUY], m_GCS_Y);
                default:
                    return 0f;
            }
        }

        public float GetGCSAlpha2D_CW(float fCoordStart1, float fCoordEnd1, float fCoordStart2, float fCoordEnd2)
        {
            // Clockwise rotation in 2D environment

            ///////////////////////////////////////////////////////////////////
            // len rozpracovane , nutne skontrolovat znamienka a vylepsit 
            ///////////////////////////////////////////////////////////////////

            if ((fCoordEnd1 >= fCoordStart1) && (fCoordEnd2 >= fCoordStart2))
            {
               // 1st Quadrant (0-90 degrees / resp. 0 - 0.5PI)
                return (float)Math.Atan((fCoordEnd2 - fCoordStart2) / (fCoordEnd1 - fCoordStart1));
            }
            else if ((fCoordEnd1 <= fCoordStart1) && (fCoordEnd2 >= fCoordStart2))
            {
               // 2nd Quadrant (90-180 degrees / resp. 0.5PI - PI)
                return (float)Math.PI/2 + (float)Math.Atan((fCoordStart1 - fCoordEnd1) / (fCoordEnd2 - fCoordStart2));
            }
            else if ((fCoordEnd1 <= fCoordStart1) && (fCoordEnd2 <= fCoordStart2))
            {
                // 3rd Quadrant (180-270 degrees / resp. PI - 1.5PI)
                return (float)Math.PI + (float)Math.Atan((fCoordStart2 - fCoordEnd2) / (fCoordStart1 - fCoordEnd1));
            }
            else /*((fCoordEnd1 >= fCoordStart1) && (fCoordEnd2 <= fCoordStart2))*/
            {
                // 4th Quadrant (270-360 degrees / resp. 1.5PI - 2PI)
                return (1.5f * (float)Math.PI) + (float)Math.Atan((fCoordEnd1 - fCoordStart1) / (fCoordStart2 - fCoordEnd2));
            }
        }

        public float GetGCSAlpha()
        {
            // GLOBAL COORDINATE SYSTEM direction
            // Rotation about Global Z-Axis

            return GetGCSAlpha2D_CW(NodeStart.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUX], NodeEnd.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUX], NodeStart.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUY], NodeEnd.m_fVNodeCoordinates.FVectorItems[(int)e2D_DOF.eUY]);
        }

        private bool IsMemberDOFRigid(bool[] bNodeDOF, ArrayList iMemberCollection, CNRelease NRelease, e2D_DOF eDOF)
        {
            if (NRelease == null) // No release at point - support restraints are governing
                if (iMemberCollection == null || iMemberCollection.Count == 1) // None or just one FEM Member is connected, means free end - support DOF are governing
                    return bNodeDOF[(int)eDOF];
                else // Node is connected to two or more members, releases are necessary to define DOF
                    return true; // Two members connection is rigid as default if no release exists
            else
            {
                // Some release exists
                if (iMemberCollection == null || iMemberCollection.Count == 1) // None or just one FEM Member is connected, means free end
                {
                    // default Node DOF are false, therefore it is always false if no support exist in node
                    if (bNodeDOF[(int)eDOF] == true && NRelease.m_bRestrain[(int)eDOF] == true)
                        return true;
                    else
                        return false;
                }
                else
                {
                    // More members is connected, do not take into account nodal support, just member releases
                    if (NRelease.m_bRestrain[(int)eDOF] == true) // Release DOF rigid restraint exist and is ridig 
                        return true;
                    else
                        return false;
                }
            }
         }



        private FEM_CALC_BASE.Enums.EElemSuppType2D [] Get_iElemSuppType()
        {
            FEM_CALC_BASE.Enums.EElemSuppType2D[] eArrSuppType = new FEM_CALC_BASE.Enums.EElemSuppType2D[3];

            // Is DOF rigid?
            // true - 1 - yes, it is
            // false - 0 - no, it isnt
            // true - 1 restraint (infinity rigidity) / false - 0 - free (zero rigidity)

            if (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 000_000
                eArrSuppType[(int) EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int) EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int) EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 000____
                //eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType.eEl_00__0; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00___; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00___; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // ____000
                //eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType.eEl__0_00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl____00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl____00; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
               )
            {
                // 000_00_
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_0_; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 00__000
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_0__00; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 00___0_
                //eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType.eEl_00__0; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00___; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_0__0_; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // _0__00_
                //eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType.eEl__0_00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl____00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_0__0_; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 00__00_
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_0__0_; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 00__0__
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in x-axis and rotation about x-axis in PCS
                //eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType.eEl_0____; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl______; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else if
                (
                IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUX) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeStart.m_ArrNodeDOF, NodeStart.m_iMemberCollection, Member.CnRelease1, e2D_DOF.eRZ) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUX) &&
                IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eUY) &&
                !IsMemberDOFRigid(NodeEnd.m_ArrNodeDOF, NodeEnd.m_iMemberCollection, Member.CnRelease2, e2D_DOF.eRZ)
                )
            {
                // 0___00_
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in x-axis and rotation about x-axis in PCS
                //eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType.eEl____0_; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
            else
            {
                // _______  // Not supported member !!!  or other not implemented restraint conditions
                eArrSuppType[(int)EM_PCS_DIR1.eUXRX] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl______; // Displacement in x-axis and rotation about x-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUYRZ] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl______; // Displacement in y-axis and rotation about z-axis in PCS
                eArrSuppType[(int)EM_PCS_DIR1.eUZRY] = FEM_CALC_BASE.Enums.EElemSuppType2D.eEl______; // Displacement in z-axis and rotation about y-axis in PCS
                return eArrSuppType;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Definition of local stiffeness matrixes depending on loading and restraints
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region 2D_00_00
        // 00_00
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // votknutie - votknutie 2D
        // votknutie - vidlicove ulozenie 2D
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private float[,] GetLocMatrix_2D_00_00()
        {
            // Local Stiffeness Matrix Members
            float fEA_len = Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.A_g / FLength;
            float f_EIy = Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.I_y;
            float f12EIy_len3 = (12f * f_EIy) / (float)Math.Pow(FLength, 3f);
            float f06EIy_len2 = (6f * f_EIy) / (float)Math.Pow(FLength, 2f);
            float f04EIy_len1 = (4f * f_EIy) / FLength;

            // Local Stiffeness Matrix
            return new float[3, 3]
            {
            {fEA_len,               0f,            0f },
            {       0f,      f12EIy_len3,   f06EIy_len2 },
            {       0f,      f06EIy_len2,   f04EIy_len1 }
            };
        }
        #endregion
        #region 2D_00_0_
        // 00_0_

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // votknutie na zaciatku - posuvne ulozenie / valcovy klb na konci - 2D
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private float[,] GetLocMatrix_2D_00_0_()
        {
            // Local Stiffeness Matrix Members
            float fEA_len = (Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.A_g) / FLength;
            float f_EIy = Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.I_y;
            float f3EIy_len3 = (3f * f_EIy) / (float)Math.Pow(FLength, 3f);
            float f3EIy_len2 = (3f * f_EIy) / (float)Math.Pow(FLength, 2f);
            float f3EIy_len1 = (3f * f_EIy) / FLength;

            // Local Stiffeness Matrix
            return new float[3, 3]
            {
            {fEA_len,           0f,         0f },
            {       0f, f3EIy_len3, f3EIy_len2 },
            {       0f, f3EIy_len2, f3EIy_len1 }
            };
        }
        #endregion
        #region 2D_0__00
        // 0__00

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // posuvne ulozenie / valcovy klb na zaciatku  - votknutie na konci - 2D
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private float[,] GetLocMatrix_2D_0__00()
        {
            // Local Stiffeness Matrix Members
            float fEA_len = Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.A_g / FLength;
            float f3EIy_len3 = (3f * Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.I_y) / (float)Math.Pow(FLength, 3f);

            // Local Stiffeness Matrix
            return new float[3, 3]
            {
            {fEA_len,         0f,    0f },
            {     0f, f3EIy_len3,    0f },
            {     0f,         0f,    0f }
            };
        }
        #endregion
        #region 2D_0__0_
        // 0__0_
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // posuvne ulozenie / valcovy klb na zaciatku  - posuvne ulozenie / valcovy klb na konci - 2D
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private float[,] GetLocMatrix_2D_0__0_()
        {
            // Local Stiffeness Matrix Members
            float fEA_len = Member.CrScStart.m_Mat.m_fE * (float)Member.CrScStart.A_g / FLength;

            // Local Stiffeness Matrix
            return new float[3, 3]  
            {
            {fEA_len,         0f,    0f },
            {     0f,         0f,    0f },
            {     0f,         0f,    0f }
            };
        }
        #endregion

        #region Local stiffeness matrix of member in 2D
        private void GetLocMatrix_2D()
        {
            switch (m_eSuppType[(int)EM_PCS_DIR1.eUYRZ]) // !!!!
            {
                case FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_00:
                    m_fkLocMatr.m_fArrMembers = GetLocMatrix_2D_00_00();
                    break;
                case FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_00_0_:
                    m_fkLocMatr.m_fArrMembers = GetLocMatrix_2D_00_0_();
                    break;
                case FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_0__00:
                    m_fkLocMatr.m_fArrMembers = GetLocMatrix_2D_0__00();
                    break;
                case FEM_CALC_BASE.Enums.EElemSuppType2D.eEl_0__0_:
                  m_fkLocMatr.m_fArrMembers = GetLocMatrix_2D_0__0_();
                    break;
                default:
                    // Error or unsupported element - exception
                    m_fkLocMatr.m_fArrMembers = null;
                    break;
            }
        }
        #endregion



















































        // GENERAL FEM OPERATIONS

        // Return partial matrix k11 of global matrix of FEM 1D element
        CMatrix GetPartM_k11(CMatrix fMk_0, CMatrix fMA)
        {
            // [fMA]T * [fMk_0] * [fMA] 

            // Output Matrix
            return MatrixF.fMultiplyMatr(MatrixF.fMultiplyMatr(MatrixF.GetTransMatrix(fMA), fMk_0), fMA);
        }

        // Return partial matrix k12 of global matrix of FEM 1D element
        CMatrix GetPartM_k12(CMatrix fMk_0, CMatrix fMA, CMatrix fMB)
        {
            // Output Matrix
            return MatrixF.GetTransMatrix(MatrixF.fMultiplyMatr(MatrixF.fMultiplyMatr(fMB, MatrixF.GetTransMatrix(fMA)), MatrixF.fMultiplyMatr(fMk_0, fMA)));
        }

        // Return partial matrix k21 of global matrix of FEM 1D element
        CMatrix GetPartM_k21(CMatrix fMk_0, CMatrix fMA, CMatrix fMB)
        {
            // Output Matrix
            return MatrixF.fMultiplyMatr(MatrixF.fMultiplyMatr(fMB, MatrixF.GetTransMatrix(fMA)), MatrixF.fMultiplyMatr(fMk_0, fMA));
        }

        // Return partial matrix k22 of global matrix of FEM 1D element
        CMatrix GetPartM_k22(CMatrix fMk_0, CMatrix fMA, CMatrix fMB)
        {
            // Output Matrix
            return MatrixF.fMultiplyMatr(fMB, MatrixF.GetTransMatrix(MatrixF.fMultiplyMatr(MatrixF.fMultiplyMatr(fMB, MatrixF.GetTransMatrix(fMA)), MatrixF.fMultiplyMatr(fMk_0, fMA))));
        }

        CMatrix fGetGlobM(CMatrix fMk11, CMatrix fMk12, CMatrix fMk21, CMatrix fMk22)
        {
            // Number of Matrix M rows and columns
            // Output Matrix
            return new CMatrix( fMk11, fMk12, fMk21,fMk22);
        }






        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Transformation of element primary end forces in LCS to GCS
        // Set vector of ends  primary forces in global coordinate system
        public void SetGetPEF_GCS()
        {
            // Start Node
            // [PEF_GCS i] = [A0T] * [PEF_LCS i]
            m_VElemPEF_GCS_StNode = VectorF.fMultiplyMatrVectr(MatrixF.GetTransMatrix(m_fATRMatr2D), m_VElemPEF_LCS_StNode);

            // End Node
            // [PEF_GCS j] = [A0T] * [PEF_LCS j]
            m_VElemPEF_GCS_EnNode = VectorF.fMultiplyMatrVectr(MatrixF.GetTransMatrix(m_fATRMatr2D), m_VElemPEF_LCS_EnNode);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Results
        // Get internal forces in global and local coordinate system
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Element Final End Forces GCS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Start Node Vector - 1 x 3
        // [EF_GCS i] = [ELoad i LCS] + [Kii] * [delta i] + [Kij] * [delta j]
        public void GetArrElemEF_GCS_StNode()
        {
            m_VElemEF_GCS_StNode =
                VectorF.fGetSum(
                m_VElemPEF_GCS_StNode,
                VectorF.fGetSum(
                VectorF.fMultiplyMatrVectr(GetPartM_k11(m_fkLocMatr, m_fATRMatr2D), NodeStart.m_VDisp),
                VectorF.fMultiplyMatrVectr(GetPartM_k12(m_fkLocMatr, m_fATRMatr2D, m_fBTTMatr2D), NodeEnd.m_VDisp)
                )
                );
        }

        // End Node Vector - 1 x 3
        // [EF_GCS j] = [ELoad j LCS] + [Kji] * [delta i] + [Kjj] * [delta j]
        public void GetArrElemEF_GCS_EnNode()
        {
            m_VElemEF_GCS_EnNode =
                VectorF.fGetSum(
                m_VElemPEF_GCS_EnNode,
                VectorF.fGetSum(
                VectorF.fMultiplyMatrVectr(GetPartM_k21(m_fkLocMatr, m_fATRMatr2D, m_fBTTMatr2D), NodeStart.m_VDisp),
                VectorF.fMultiplyMatrVectr(GetPartM_k22(m_fkLocMatr, m_fATRMatr2D, m_fBTTMatr2D), NodeEnd.m_VDisp)
                )
                );
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Element Final End forces LCS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Start Node Vector - 1 x 3
        //  [EF_LCS i] = [A0] * [EF_GCS i]
        public void GetArrElemEF_LCS_StNode()
        {
            m_VElemEF_LCS_StNode = VectorF.fMultiplyMatrVectr(m_fATRMatr2D, m_VElemEF_GCS_StNode);
        }

        // End Node Vector - 1 x 3
        // [EF_LCS j] = [A0] * [EF_GCS j]
        public void GetArrElemEF_LCS_EnNode()
        {
            m_VElemEF_LCS_EnNode = VectorF.fMultiplyMatrVectr(m_fATRMatr2D, m_VElemEF_GCS_EnNode);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Element final internal forces in LCS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Start Node Vector - 1 x 3
        //  [IF_LCS i] = [-1,-1,-1] * [EF_LCS i]
        public void GetArrElemIF_LCS_StNode()
        {
            CVector fTempSignTransf = new CVector(-1, -1, 1 );
            m_VElemIF_LCS_StNode = VectorF.fMultiplyVectors(fTempSignTransf, m_VElemEF_LCS_StNode);
        }

        // End Node Vector - 1 x 3
        // [IF_LCS j]  = [1, 1,-1] * [EF_LCS j]
        public void GetArrElemIF_LCS_EnNode()
        {
            CVector fTempSignTransf = new CVector(1, 1, -1 );
            m_VElemIF_LCS_EnNode = VectorF.fMultiplyVectors(fTempSignTransf, m_VElemEF_LCS_EnNode);
        }



    }
}
