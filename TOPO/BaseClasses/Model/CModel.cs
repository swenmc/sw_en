using _3DTools;
using BaseClasses.GraphObj;
using BriefFiniteElementNet;
using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseClasses
{
    // Main model class

    // List of model objects is included

    [Serializable]
    public class CModel
    {
        // General project data

        public string m_sProjectName;
        public string m_sConstObjectName;
        public string m_sFileName;

        public ESLN m_eSLN; // Model/solution Type
        public int m_eNDOF; // DOF (3 in 2D, 6 in 3D)
        public EGCS m_eGCS; // Global coordinate system (Left-handed or Right-handed)

        // Physical Model / Structural data
        // Collection of references to objects

        // Materials used/defined in current model
        public CMat[] m_arrMat;
        // Cross-sections used/ defined in current model
        public CCrSc[] m_arrCrSc;

        // Topological nodes (not FEM)
        // Note !!!
        // Type of object collections - some dynamically allocated which can be resized - stack, queue, vector ????

        public CNode[] m_arrNodes;
        // 1D Elements (not FEM)
        public CMember[] m_arrMembers;
        // Nodal Supports
        public CNSupport[] m_arrNSupports;
        // Member Releases
        public CNRelease[] m_arrNReleases;
        // Member Releases
        public CIntermediateTransverseSupport[] m_arrIntermediateTransverseSupports;
        // Connections
        public List<CConnectionJointTypes> m_arrConnectionJoints;
        // Foundations
        public List<CFoundation> m_arrFoundations;

        // Loading
        // Nodal Loads
        public CNLoad[] m_arrNLoads;
        // Member Loads
        public CMLoad[] m_arrMLoads;
        // Surface Loads
        public CSLoad_Free[] m_arrSLoads;
        // Load Cases
        public CLoadCase[] m_arrLoadCases;
        // Load Case Groups
        public CLoadCaseGroup[] m_arrLoadCaseGroups;
        // Load Combinations
        public CLoadCombination[] m_arrLoadCombs;
        // Limit States
        public CLimitState[] m_arrLimitStates;

        // Geometrical graphical model objects
        // Points
        //public CPoint[] m_arrGOPoints;
        public CPoint[] m_arrGOPoints;
        // Lines
        public CLine[] m_arrGOLines;
        // Areas
        public CArea[] m_arrGOAreas;
        // Volumes
        public CVolume[] m_arrGOVolumes;

        // 3D Objects
        public CStructure_Window[] m_arrGOStrWindows;

        // Group of structure parts / components - each of them has its own member list
        public List<CMemberGroup> listOfModelMemberGroups;

        //Grouped Members
        Dictionary<Tuple<float, string, string>, List<CMember>> GroupedMembers;

        public LoadCombinationsInternalForces LoadCombInternalForcesResults { get; set; }

        [NonSerialized]
        public Model BFEMNetModel;

        //Visuals
        public ScreenSpaceLines3D AxisX;
        public ScreenSpaceLines3D AxisY;
        public ScreenSpaceLines3D AxisZ;
        public ScreenSpaceLines3D WireFrameJoints;
        public ScreenSpaceLines3D WireFrameMembers;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CModel()
        {
            m_arrNodes = new CNode[] { new CNode() };
        }
        public CModel(string sFileName)
        {
            m_sFileName = sFileName;
        }
        // Alokuje velkost poli zoznamov, malo by to byt dymamicke
        public CModel(string sFileName, ESLN eSLN, int eNDOF, EGCS eGCS,
            int iMatNum, int iCrScNum, int iNodeNum,
            int iMemNum, int iNSupNum, int iNRelNum, int iITSNum, int iNLoadNum,
            int iMLoadNum, int iLoadCaseNum, int iLoadComNum)
        {
            m_eSLN = eSLN;
            m_eNDOF = eNDOF;
            m_eGCS = eGCS;
            m_arrMat = new CMat[iMatNum];
            m_arrCrSc = new CCrSc[iCrScNum];
            m_arrNodes = new CNode[iNodeNum];
            m_arrMembers = new CMember[iMemNum];
            m_arrNSupports = new CNSupport[iNSupNum];
            m_arrNReleases = new CNRelease[iNRelNum];
            m_arrIntermediateTransverseSupports = new CIntermediateTransverseSupport[iITSNum];
            m_arrNLoads = new CNLoadAll[iNLoadNum];
            m_arrMLoads = new CMLoad[iMLoadNum];
            m_arrLoadCases = new CLoadCase[iLoadCaseNum];
            m_arrLoadCombs = new CLoadCombination[iLoadComNum];
        }
        public CModel(string sProjectName, string sConstObjectName, string sFileName)
        {
            m_sProjectName = sProjectName;
            m_sConstObjectName = sConstObjectName;
            m_sFileName = sFileName;
        }

        // Geometrical model
        public CModel(string sFileName, ESLN eSLN, int eNDOF, EGCS eGCS,
            int iMatNum, /*int iCrScNum,*/ int iPointNum,
            /*int iMemNum,*/ int iLineNum, int iAreaNum, int iVolumeNum, int iWindNum)
        {
            m_eSLN = eSLN;
            m_eNDOF = eNDOF;
            m_eGCS = eGCS;
            m_arrMat = new CMat[iMatNum];
            //m_arrCrSc = new CCrSc[iCrScNum];
            m_arrGOPoints = new CPoint[iPointNum];
            //m_arrMembers = new CMember[iMemNum];
            m_arrGOAreas = new CArea[iAreaNum];
            m_arrGOVolumes = new CVolume[iVolumeNum];
            m_arrGOStrWindows = new CStructure_Window[iWindNum];
        }

        //Funkcia vytvori Dictionary z rovnakych Members - kriterium je tu (FLength, CrScStart.GetType().Name, CrScEnd.GetType().Name)
        public void GroupModelMembers()
        {
            GroupedMembers = new Dictionary<Tuple<float, string, string>, List<CMember>>();

            //pre kazdy member v poli members
            foreach (CMember m in m_arrMembers)
            {
                string startCrscName = m.CrScStart != null ? m.CrScStart.GetType().Name : string.Empty;
                string endCrscName = m.CrScEnd != null ? m.CrScEnd.GetType().Name : string.Empty;
                //vytvori n-ticu 3 property
                Tuple<float, string, string> t = Tuple.Create(m.FLength, startCrscName, endCrscName);
                //ak sa dana n-tica v dictionary nachadza, tak pridaj member do kolekcie
                //inak vytvor novy zaznam v dictionary kde Key je n-tica a Value je list s tymto objektom member
                if (GroupedMembers.ContainsKey(t)) GroupedMembers[t].Add(m);
                else GroupedMembers.Add(t, new List<CMember> { m });
            }

            //Takto sa kolekciou prechadza. Stale je to par Key a Value
            //v nasom pripade je Key n-tica (FLength, CrScStart.GetType().Name, CrScEnd.GetType().Name) a pristupuje sa k tymto properties pair.Key.Item1, pair.Key.Item2, pair.Key.Item3
            //value je List<Member> vypiseme pocet rovnakych prvkov Members
            foreach (KeyValuePair<Tuple<float, string, string>, List<CMember>> pair in GroupedMembers)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}", string.Format("{0}_{1}_{2}", pair.Key.Item1, pair.Key.Item2, pair.Key.Item3), pair.Value.Count));
            }
        }


        //public List<CMember> GetListOfMembersWithCrsc(CCrSc crsc)
        //{
        //    List<CMember> members = new List<CMember>();
        //    foreach (CMember m in m_arrMembers)
        //    {
        //        if (m.CrScStart.Equals(crsc) || (m.CrScEnd != null && m.CrScEnd.Equals(crsc))) members.Add(m);
        //    }
        //    return members;
        //}
        public List<CMember> GetListOfMembersWithCrsc(CCrSc crsc)
        {
            return m_arrMembers.Where(m => m.CrScStart.ID == crsc.ID || (m.CrScEnd != null && m.CrScEnd.ID == crsc.ID)).ToList();            
        }

        public void GetModelMemberStartEndConnectionJoints(CMember m, out CConnectionJointTypes jStart, out CConnectionJointTypes jEnd)
        {
            jStart = null;
            jEnd = null;
            foreach (CConnectionJointTypes cj in m_arrConnectionJoints)
            {
                CMember[] secondary_members = cj.m_SecondaryMembers;
                if (secondary_members == null) continue;
                foreach (CMember secMem in secondary_members)
                {
                    if (secMem.ID == m.ID)
                    {
                        if (secMem.NodeStart.ID == cj.m_Node.ID) jStart = cj; // Zatial je lepsie porovnavat len ID a nie cele objekty Node, pretoze pri vytvarani dielcich modelov sa objekty Node modifikuju
                        if (secMem.NodeEnd.ID == cj.m_Node.ID) jEnd = cj;
                    }
                }
            }
            if (jStart != null && jEnd != null) return;

            foreach (CConnectionJointTypes cj in m_arrConnectionJoints)
            {
                if (cj.m_MainMember == null)
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:"+ cj.ID.ToString());

                if (cj.m_MainMember != null && cj.m_MainMember.ID == m.ID)
                {
                    if (cj.m_MainMember.NodeStart.ID == cj.m_Node.ID) jStart = cj; // Zatial je lepsie porovnavat len ID a nie cele objekty Node, pretoze pri vytvarani dielcich modelov sa objekty Node modifikuju
                    if (cj.m_MainMember.NodeEnd.ID == cj.m_Node.ID) jEnd = cj;
                }
            }

            // Validation - start or end joint wasn't found.
            if (jStart == null || jEnd == null)
                throw new Exception("Start or end connection joint not found.\n" +
                    "Member ID: " + m.ID + "\n"+
                    "Member Start Node ID: " + m.NodeStart.ID +"\n" +
                    "Member End Node ID: " + m.NodeEnd.ID);
        }

        public void GetModelMemberStartConnectionJoint(CMember m, out CConnectionJointTypes jStart)
        {
            CConnectionJointTypes jEnd_temp;
            GetModelMemberStartEndConnectionJoints(m, out jStart, out jEnd_temp);
        }

        public void GetModelMemberEndConnectionJoint(CMember m, out CConnectionJointTypes jEnd)
        {
            CConnectionJointTypes jStart_temp;
            GetModelMemberStartEndConnectionJoints(m, out jStart_temp, out jEnd);
        }

        public CFoundation GetFoundationForJointFromModel(CConnectionJointTypes joint)
        {
            foreach (CFoundation f in m_arrFoundations)
            {
                if (joint.m_Node.ID == f.m_Node.ID) return f;
            }
            return null;
        }
    }
}
