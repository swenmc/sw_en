using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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
        public CMember [] m_arrMembers;
        // Nodal Supports
        public CNSupport[] m_arrNSupports;
        // Member Releases
        public CNRelease[] m_arrNReleases;
        // Connections
        public List <CConnectionJointTypes> m_arrConnectionJoints;
        // Loading
        // Nodal Loads
        public CNLoad[] m_arrNLoads;
        // Member Loads
        public CMLoad[] m_arrMLoads;
        // Load Cases
        public CLoadCase[] m_arrLoadCases;
        // Load Combinations
        public CLoadCombination[] m_arrLoadCombs;
        // Limit States
        public CLimitState[] m_arrLimitStates;

        // Geometrical graphical model objects
        // Points
        //public BaseClasses.GraphObj.CPoint[] m_arrGOPoints;
        public BaseClasses.GraphObj.CPoint[] m_arrGOPoints;
        // Lines
        public BaseClasses.GraphObj.CLine[] m_arrGOLines;
        // Areas
        public BaseClasses.GraphObj.CArea[] m_arrGOAreas;
        // Volumes
        public BaseClasses.GraphObj.CVolume[] m_arrGOVolumes;

        // 3D Objects
        public BaseClasses.GraphObj.CStructure_Window[] m_arrGOStrWindows;

        //Grouped Members
        Dictionary<Tuple<float, string, string>, List<CMember>> GroupedMembers;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CModel() { }
        public CModel(string sFileName)
        {
            m_sFileName = sFileName;
        }
        // Alokuje velkost poli zoznamov, malo by to byt dymamicke
        public CModel(string sFileName, ESLN eSLN, int eNDOF, EGCS eGCS,
            int iMatNum, int iCrScNum, int iNodeNum,
            int iMemNum, int iNSupNum, int iNRelNum, int iNLoadNum,
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
            m_arrGOPoints = new BaseClasses.GraphObj.CPoint[iPointNum];
            //m_arrMembers = new CMember[iMemNum];
            m_arrGOAreas = new BaseClasses.GraphObj.CArea[iAreaNum];
            m_arrGOVolumes = new BaseClasses.GraphObj.CVolume[iVolumeNum];
            m_arrGOStrWindows = new BaseClasses.GraphObj.CStructure_Window[iWindNum];
        }


        //Funkcia vytvori Dictionary z rovnakych Members - kriterium je tu (FLength, CrScStart.GetType().Name, CrScEnd.GetType().Name)
        public void GroupModelMembers()
        {
            GroupedMembers = new Dictionary<Tuple<float, string, string>, List<CMember>>();
            
            //pre kazdy member v poli members
            foreach (CMember m in m_arrMembers)
            {
                string startCrscName = m.CrScStart != null ?  m.CrScStart.GetType().Name : string.Empty;
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
    }
}
