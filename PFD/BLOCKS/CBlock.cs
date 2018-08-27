using BaseClasses.GraphObj;
using CRSC;
using MATERIAL;
using System;
using BaseClasses;

namespace PFD
{
    // Main model class

    // List of block objects is included

    [Serializable]
    public class CBlock : CModel_PFD
    {
        // General project data
        public string m_sBlockName;

        private int m_iNumberOfGirtsToDeactivate;

        public int INumberOfGirtsToDeactivate
        {
            get
            {
                return m_iNumberOfGirtsToDeactivate;
            }

            set
            {
                m_iNumberOfGirtsToDeactivate = value;
            }
        }

        private int m_iNumberOfGirtsGeneratedInBlock;

        public int INumberOfGirtsGeneratedInBlock
        {
            get
            {
                return m_iNumberOfGirtsGeneratedInBlock;
            }

            set
            {
                m_iNumberOfGirtsGeneratedInBlock = value;
            }
        }

        private CMember m_ReferenceGirt;

        public CMember ReferenceGirt
        {
            get
            {
                return m_ReferenceGirt;
            }

            set
            {
                m_ReferenceGirt = value;
            }
        }

        private string m_sBuildingSide;

        public string BuildingSide
        {
            get
            {
                return m_sBuildingSide;
            }

            set
            {
                m_sBuildingSide = value;
            }
        }

        // Physical Model / Structural data
        // Collection of references to objects

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CBlock() { }

        public CBlock(string sBlockName)
        {
            m_sBlockName = sBlockName;
        }

        // Alokuje velkost poli zoznamov, malo by to byt dymamicke
        public CBlock(string sBlockName,
            int iMatNum, int iCrScNum, int iNodeNum,
            int iMemNum, int iNSupNum, int iNRelNum, int iNLoadNum,
            int iMLoadNum)
        {
            m_sBlockName = sBlockName;
            m_arrMat = new CMat[iMatNum];
            m_arrCrSc = new CCrSc[iCrScNum];
            m_arrNodes = new CNode[iNodeNum];
            m_arrMembers = new CMember[iMemNum];
            m_arrNSupports = new CNSupport[iNSupNum];
            m_arrNReleases = new CNRelease[iNRelNum];
            m_arrNLoads = new CNLoadAll[iNLoadNum];
            m_arrMLoads = new CMLoad[iMLoadNum];
        }

        // Geometrical model
        public CBlock(string sBlockName, 
            int iMatNum, /*int iCrScNum,*/ int iPointNum,
            /*int iMemNum,*/ int iLineNum, int iAreaNum, int iVolumeNum, int iWindNum)
        {
            m_arrMat = new CMat[iMatNum];
            //m_arrCrSc = new CCrSc[iCrScNum];
            m_arrGOPoints = new CPoint[iPointNum];
            //m_arrMembers = new CMember[iMemNum];
            m_arrGOAreas = new CArea[iAreaNum];
            m_arrGOVolumes = new CVolume[iVolumeNum];
            m_arrGOStrWindows = new CStructure_Window[iWindNum];
        }
    }
}
