using _3DTools;
using BaseClasses.GraphObj;
using BriefFiniteElementNet;
using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    // Main model class

    // List of model objects is included

    [Serializable]
    public class CModel
    {
        public bool debugging = false;
        private List<float> m_L1_Bays;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // TO Ondrej: Docasne su tieto parametre tu, ale nemali byt v CModel ale niekde v potomkoch, napriklad CModel_PFD
        // Netykaju sa totiz uplne vseobecneho modelu - trieda CModel ale specifickeho modelu, ktory sme vytvorili pre projekt PFD (portal frame designer), teda CModel_PFD
        public EModelType_FS eKitset;

        public float fL_tot_centerline;
        public float fW_frame_centerline;
        public float fH1_frame_centerline;
        public float fH2_frame_centerline;

        public float fDist_Girt;
        public float fDist_Purlin;

        public float fBottomGirtPosition;

        public float fDist_FrontColumns;
        public float fDist_BackColumns;

        public int iOneRafterPurlinNo;

        public float fL_tot_overall;
        public float fW_frame_overall;
        public float fH1_frame_overall;
        public float fH2_frame_overall;
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        // General project data

        public string m_sProjectName;
        public string m_sConstObjectName;
        public string m_sFileName;

        public ESLN m_eSLN; // Model/solution Type
        public int m_eNDOF; // DOF (3 in 2D, 6 in 3D)
        public EGCS m_eGCS; // Global coordinate system (Left-handed or Right-handed)

        // Physical Model / Structural data
        // Collection of references to objects

        //// Materials used/defined in current model
        //public CMat[] m_arrMat;
        //// Cross-sections used/ defined in current model
        //public CCrSc[] m_arrCrSc;

        // Materials used/defined in current model
        public Dictionary<EMemberType_FS_Position, CMat> m_arrMat; //ja by som to mal radsej na iny enum viazane
        //public Dictionary<EMemberGroupNames, CMat> m_arrMat;

        // Cross-sections used/ defined in current model
        public Dictionary<EMemberType_FS_Position, CCrSc> m_arrCrSc; //ja by som to mal radsej na iny enum viazane
        //public Dictionary<EMemberGroupNames, CCrSc> m_arrCrSc;

        // Topological nodes (not FEM)
        // Note !!!
        // Type of object collections - some dynamically allocated which can be resized - stack, queue, vector ????

        public CNode[] m_arrNodes;
        // 1D Elements (not FEM)
        public CMember[] m_arrMembers;
        //public List<CMember> m_arrMembers; takto by to bolo lepsie ale vzhladom na 27682 pouziti, to by bolo tazke refaktorovat :-)
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
        // Slabs
        public List<CSlab> m_arrSlabs;
        // Saw Cuts
        //public List<CSawCut> m_arrSawCuts;
        // Control Joints
        //public List<CControlJoint> m_arrControlJoints;

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
        public Point3D[] m_arrGOPoints;
        // Lines
        public CLine[] m_arrGOLines;
        // Areas
        public CArea[] m_arrGOAreas;
        // Volumes
        public CVolume[] m_arrGOVolumes;

        // 3D Objects
        public List<CCladding> m_arrGOCladding;
        public List<CStructure_Window> m_arrGOStrWindows;
        public List<CStructure_Door> m_arrGOStrDoors;

        // Group of structure parts / components - each of them has its own member list
        public List<CMemberGroup> listOfModelMemberGroups;

        //Grouped Members
        Dictionary<Tuple<float, string, string>, List<CMember>> GroupedMembers;

        public LoadCombinationsInternalForces LoadCombInternalForcesResults { get; set; }

        public List<float> L1_Bays
        {
            get
            {
                return m_L1_Bays;
            }

            set
            {
                m_L1_Bays = value;
            }
        }

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
            m_arrCrSc = new Dictionary<EMemberType_FS_Position, CCrSc>();
            m_arrMat = new Dictionary<EMemberType_FS_Position, CMat>();
        }
        public CModel(string sFileName)
        {
            m_sFileName = sFileName;
            m_arrCrSc = new Dictionary<EMemberType_FS_Position, CCrSc>();
            m_arrMat = new Dictionary<EMemberType_FS_Position, CMat>();
        }
        //komentujem nepouzivane konstruktory
        //// Alokuje velkost poli zoznamov, malo by to byt dymamicke
        //public CModel(string sFileName, ESLN eSLN, int eNDOF, EGCS eGCS,
        //    int iMatNum, int iCrScNum, int iNodeNum,
        //    int iMemNum, int iNSupNum, int iNRelNum, int iITSNum, int iNLoadNum,
        //    int iMLoadNum, int iLoadCaseNum, int iLoadComNum)
        //{
        //    m_eSLN = eSLN;
        //    m_eNDOF = eNDOF;
        //    m_eGCS = eGCS;
        //    m_arrMat = new CMat[iMatNum];
        //    m_arrCrSc = new CCrSc[iCrScNum];
        //    m_arrNodes = new CNode[iNodeNum];
        //    m_arrMembers = new CMember[iMemNum];
        //    m_arrNSupports = new CNSupport[iNSupNum];
        //    m_arrNReleases = new CNRelease[iNRelNum];
        //    m_arrIntermediateTransverseSupports = new CIntermediateTransverseSupport[iITSNum];
        //    m_arrNLoads = new CNLoadAll[iNLoadNum];
        //    m_arrMLoads = new CMLoad[iMLoadNum];
        //    m_arrLoadCases = new CLoadCase[iLoadCaseNum];
        //    m_arrLoadCombs = new CLoadCombination[iLoadComNum];
        //}
        //public CModel(string sProjectName, string sConstObjectName, string sFileName)
        //{
        //    m_sProjectName = sProjectName;
        //    m_sConstObjectName = sConstObjectName;
        //    m_sFileName = sFileName;
        //}

        //// Geometrical model
        //public CModel(string sFileName, ESLN eSLN, int eNDOF, EGCS eGCS,
        //    int iMatNum, /*int iCrScNum,*/ int iPointNum,
        //    /*int iMemNum,*/ int iLineNum, int iAreaNum, int iVolumeNum, int iWindNum)
        //{
        //    m_eSLN = eSLN;
        //    m_eNDOF = eNDOF;
        //    m_eGCS = eGCS;
        //    m_arrMat = new CMat[iMatNum];
        //    //m_arrCrSc = new CCrSc[iCrScNum];
        //    m_arrGOPoints = new Point3D[iPointNum];
        //    //m_arrMembers = new CMember[iMemNum];
        //    m_arrGOAreas = new CArea[iAreaNum];
        //    m_arrGOVolumes = new CVolume[iVolumeNum];
        //    m_arrGOStrWindows = new List<CStructure_Window>(iWindNum);
        //}

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
                if(debugging)
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
        public List<CMember> GetListOfMembersWithCrscDatabaseID(int databaseID)
        {
            return m_arrMembers.Where(m => m.CrScStart.DatabaseID == databaseID || (m.CrScEnd != null && m.CrScEnd.DatabaseID == databaseID)).ToList();
        }


        //Pozor tu som nasiel ze
        //m_arrConnectionJoints s m_Node.ID == 6 najde 2 jeden je TA01 a duhy T001
        //skusit treba m_arrConnectionJoints.Where(c=> c.m_Node.ID == 6).ElementAt(1)
        public void GetModelMemberStartEndConnectionJoints(CMember m, out CConnectionJointTypes jStart, out CConnectionJointTypes jEnd)
        {
            jStart = null;
            jEnd = null;
            if (m_arrConnectionJoints == null) return;
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
                if (jStart != null && jEnd != null) return;
            }
            

            foreach (CConnectionJointTypes cj in m_arrConnectionJoints)
            {
                if (cj.m_MainMember == null)
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:"+ cj.ID.ToString());

                if (cj.m_MainMember != null && cj.m_MainMember.ID == m.ID)
                {
                    if (cj.m_MainMember.NodeStart.ID == cj.m_Node.ID) jStart = cj; // Zatial je lepsie porovnavat len ID a nie cele objekty Node, pretoze pri vytvarani dielcich modelov sa objekty Node modifikuju
                    if (cj.m_MainMember.NodeEnd.ID == cj.m_Node.ID) jEnd = cj;
                }
                if (jStart != null && jEnd != null) return;
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

        public float GetBayWidth(int bayID)
        {
            return L1_Bays.ElementAtOrDefault(bayID - 1);
        }

        public float GetBaysWidthUntilFrameIndex(int frameIndex)
        {
            float w = 0;
            for (int i = 0; i < frameIndex; i++)
            {
                if (i >= L1_Bays.Count) continue;
                w += L1_Bays[i];
            }
            return w;
        }

        public float GetBayWidthPrevious(int frameIndex)
        {
            if (frameIndex > 0) return L1_Bays[frameIndex - 1];
            else return 0;
        }

        public float GetBayWidthNext(int frameIndex)
        {
            if (frameIndex >= L1_Bays.Count) return 0;
            else return L1_Bays[frameIndex];
        }

        public float GetTributaryWidth(int frameIndex)
        {
            return 0.5f * GetBayWidthPrevious(frameIndex) + 0.5f * GetBayWidthNext(frameIndex);
        }

        public CMat GetMaterial(EMemberType_FS_Position memberType)
        {
            CMat mat = null;
            m_arrMat.TryGetValue(memberType, out mat);
            return mat;
        }
        public void AddMaterial(EMemberType_FS_Position memberType, CMat mat)
        {
            if (!m_arrMat.ContainsKey(memberType)) m_arrMat.Add(memberType, mat);
            else m_arrMat[memberType] = mat;
        }
        public CCrSc GetCrSc(EMemberType_FS_Position memberType)
        {
            CCrSc crsc = null;
            m_arrCrSc.TryGetValue(memberType, out crsc);
            return crsc;
        }
        public void AddCRSC(EMemberType_FS_Position memberType, CCrSc crsc)
        {
            if (!m_arrCrSc.ContainsKey(memberType)) m_arrCrSc.Add(memberType, crsc);
            else m_arrCrSc[memberType] = crsc;
        }
    }
}
