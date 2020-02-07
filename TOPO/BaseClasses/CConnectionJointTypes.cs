using System;
using MATERIAL;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConnectionJointTypes : CEntity3D
    {
        private EJointType m_JointType;

        public CPlate[] m_arrPlates;
        public CConnector[] m_arrConnectors;
        public CWeld[] m_arrWelds;
        public CMember m_MainMember; // hlavny prvok (spravidla najmasivnejsi) ku ktoremu sa pripajaju jeden alebo viacero dalsich sekundarnych
        public CMember[] m_SecondaryMembers; // zoznam sekundarnych prvkov, TODO - asi bude potrebne upravit podla jednotlivych typov prvkov v spoji (nosnik / diagonala / stlp)
        public CNode m_Node;
        public CNode[] m_arrAssignedNodesWithJointType;
        public bool bIsJointDefinedinGCS;

        [NonSerialized]
        public Model3DGroup Visual_ConnectionJoint;

        private CJointDesignDetails m_DesignDetails;
        public CJointDesignDetails DesignDetails
        {
            get { return m_DesignDetails; }
            set { m_DesignDetails = value; }
        }

        public EJointType JointType
        {
            get
            {
                return m_JointType;
            }

            set
            {
                m_JointType = value;
            }
        }

        public CConnectionJointTypes() { }

        public CConnectionJointTypes(int iNumberOfAssignedNodes, int arrPlatesSize, int arrConnectorsSize, int arrWeldsSize)
        {
            m_arrAssignedNodesWithJointType = new CNode[iNumberOfAssignedNodes];

            m_arrPlates = arrPlatesSize != 0 ? new CPlate[arrPlatesSize] : null;
            m_arrConnectors = arrConnectorsSize != 0 ? new CConnector[arrConnectorsSize] : null;
            m_arrWelds = arrWeldsSize != 0 ? new CWeld[arrWeldsSize] : null;

            m_Mat = new CMat();
        }

        public CConnectionJointTypes(CPlate[] arrPlatesTemp, CBolt[] arrBoltsTemp, CWeld[] arrWeldsTemp)
        {
            m_arrPlates = new CPlate[arrPlatesTemp.Length];

            for (int i = 0; i < arrPlatesTemp.Length; i++)
            {
                m_arrPlates[i] = arrPlatesTemp[i];
            }

            m_arrConnectors = new CConnector[arrBoltsTemp.Length];

            for (int i = 0; i < arrBoltsTemp.Length; i++)
            {
                m_arrConnectors[i] = arrBoltsTemp[i];
            }

            m_arrWelds = new CWeld[arrWeldsTemp.Length];

            for (int i = 0; i < arrWeldsTemp.Length; i++)
            {
                m_arrWelds[i] = arrWeldsTemp[i];
            }
        }

        // Pomocna funkcia pre base plates - nastavenie typu plechu podla prierezu a nastavenie screwArrangement
        protected void SetBasePlateTypeAndScrewArrangement(string sSectionNameDatabase, CScrew referenceScrew,
            out string platePrefix, out DATABASE.DTO.CPlate_B_Properties plateProp, out CScrewArrangement screwArrangement, out CWasher_W washerPlateTop, out CWasher_W washerBearing)
        {
            screwArrangement = null;
            washerPlateTop = null;
            plateProp = null;

            string washerPlateTopName;
            washerBearing = new CWasher_W("WA", new Point3D(0,0,0), 0, -90, 0); // Opacny uhol otocenia okolo y ako ma anchor aby sme sa dostali naspat do roviny XY a t je v smere Z

            if (sSectionNameDatabase == "10075")
            {
                //platePrefix = "BI";
                platePrefix = "BH";
                washerPlateTopName = "";
            }
            else if (sSectionNameDatabase == "27055")
            {
                platePrefix = "BG";
                washerPlateTopName = "WB";
            }
            else if (sSectionNameDatabase == "27095")
            {
                platePrefix = "BG";
                washerPlateTopName = "WB";
            }
            else if (sSectionNameDatabase == "27095n")
            {
                platePrefix = "BB";
                washerPlateTopName = "WB";
            }
            else if (sSectionNameDatabase == "270115")
            {
                platePrefix = "BG";
                washerPlateTopName = "WB";
            }
            else if (sSectionNameDatabase == "270115btb")
            {
                platePrefix = "BA";
                washerPlateTopName = "WB";
            }
            else if (sSectionNameDatabase == "270115n")
            {
                platePrefix = "BB";
                washerPlateTopName = "WB";
            }
            else if (sSectionNameDatabase == "50020")
            {
                platePrefix = "BD";
                washerPlateTopName = "WD";
            }
            else if (sSectionNameDatabase == "50020n")
            {
                platePrefix = "BE-3 holes";
                washerPlateTopName = "WD";
            }
            else if (sSectionNameDatabase == "63020")
            {
                platePrefix = "BF-4 holes";
                washerPlateTopName = "WF";
            }
            else if (sSectionNameDatabase == "63020s1")
            {
                platePrefix = "BF-6 holes";
                washerPlateTopName = "WF";
            }
            else if (sSectionNameDatabase == "63020s2")
            {
                platePrefix = "BF-6 holes";
                washerPlateTopName = "WF";
            }
            else
            {
                platePrefix = "";
                washerPlateTopName = "";
                throw new NotImplementedException("Invalid cross-section name: " + sSectionNameDatabase + ". \n" +
                                                  "Base plate of cross-section with this name is not implemented");
            }

            if (platePrefix != "")
            {
                plateProp = DATABASE.CJointsManager.GetPlate_B_Properties(platePrefix);
                screwArrangement = GetBasePlateArrangement(platePrefix, referenceScrew, (float)plateProp.dim2y); // Set base plate screw arrangement
                washerPlateTop = new CWasher_W(washerPlateTopName, new Point3D(0, 0, 0), 0, -90, 0); // Opacny uhol otocenia okolo y ako ma anchor aby sme sa dostali naspat do roviny XY a t je v smere Z
            }
        }

        public CScrewArrangement GetBasePlateArrangement(string platePrefix, CScrew referenceScrew, float fh_plate)
        {
            // TODO - Urobit nastavovanie rozmerov dynamicky podla velkosti prierezu / vyztuh a plechu
            // Nacitat velkosti vyztuh z parametrov prierezu, medzery a polohu skrutiek urcovat dynamicky
            // BX1 - 2 rectangular sequencies
            // BX2 - 3 rectangular sequencies

            CScrewArrangement_BX_2 screwArrangement2_10075_92 = new CScrewArrangement_BX_2(referenceScrew, fh_plate, fh_plate - 2 * 0.006f - 2 * 0.002f, 0.023f,
                    2, 1, 0.020f, 0.015f, 0.030f, 0.03f,
                    2, 1, 0.020f, 0.050f, 0.030f, 0.03f,
                    2, 1, 0.020f, 0.085f, 0.030f, 0.03f);
            CScrewArrangement_BX_2 screwArrangement2_270XXX_180 = new CScrewArrangement_BX_2(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,
                    3, 1, 0.030f, 0.030f, 0.060f, 0.050f,
                    3, 1, 0.030f, 0.135f, 0.060f, 0.050f,
                    3, 1, 0.030f, 0.240f, 0.060f, 0.050f);
            CScrewArrangement_BX_2 screwArrangement2_270XXX_100 = new CScrewArrangement_BX_2(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,
                    2, 1, 0.025f, 0.030f, 0.050f, 0.050f,
                    2, 1, 0.025f, 0.135f, 0.050f, 0.050f,
                    2, 1, 0.025f, 0.240f, 0.050f, 0.050f);
            CScrewArrangement_BX_2 screwArrangement2_270XXXn_180 = new CScrewArrangement_BX_2(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,
                    3, 1, 0.030f, 0.030f, 0.060f, 0.050f,
                    3, 1, 0.030f, 0.135f, 0.060f, 0.050f,
                    3, 1, 0.030f, 0.260f, 0.060f, 0.050f);
            CScrewArrangement_BX_1 screwArrangement1_50020_154 = new CScrewArrangement_BX_1(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.132f,
                    3, 3, 0.027f, 0.030f, 0.050f, 0.060f,
                    3, 3, 0.027f, 0.345f, 0.050f, 0.060f);
            CScrewArrangement_BX_1 screwArrangement1_50020n_154 = new CScrewArrangement_BX_1(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.182f,
                    3, 3, 0.027f, 0.030f, 0.050f, 0.060f,
                    3, 3, 0.027f, 0.395f, 0.050f, 0.060f);
            CScrewArrangement_BX_1 screwArrangement1_63020_180 = new CScrewArrangement_BX_1(referenceScrew, fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,
                    3, 5, 0.030f, 0.029f, 0.060f, 0.035f,
                    3, 5, 0.030f, 0.404f, 0.060f, 0.035f);
            CScrewArrangement_BX_1 screwArrangement1_63020_400 = new CScrewArrangement_BX_1(referenceScrew, fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,
                    7, 5, 0.050f, 0.029f, 0.050f, 0.035f,
                    7, 5, 0.050f, 0.401f, 0.050f, 0.035f);

            if (platePrefix == "BA")
                return screwArrangement2_270XXX_180;
            if (platePrefix == "BG")
                return screwArrangement2_270XXX_100;
            else if (platePrefix == "BB" || platePrefix == "BC")
                return screwArrangement2_270XXXn_180;
            else if (platePrefix == "BD")
                return screwArrangement1_50020_154;
            else if (platePrefix == "BE-2 holes" || platePrefix == "BE-3 holes")
                return screwArrangement1_50020n_154;
            else if (platePrefix == "BF-4 holes" || platePrefix == "BF-6 holes")
                return screwArrangement1_63020_400;
            else if (platePrefix == "BH" || platePrefix == "BI")
                return screwArrangement2_10075_92;
            else if (platePrefix == "BJ-2 holes" || platePrefix == "BJ-3 holes")
                return screwArrangement1_63020_180;
            else
                return null; // Exception - not defined plate prefix
        }

        public void SetBaseJointEdgeDistances(CFoundation pad)
        {
            // Joint with base plate and anchors
            if (m_arrPlates != null && m_arrPlates[0] is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)m_arrPlates[0];
                float feccentricity_x = pad.Eccentricity_x;
                float feccentricity_y = pad.Eccentricity_y;
                float fpad_x = pad.m_fDim1;
                float fpad_y = pad.m_fDim2;

                float x_plateEdge_to_pad_basic = 0.5f * (fpad_x - basePlate.Fb_X);
                float y_plateEdge_to_pad_basic = 0.5f * (fpad_y - basePlate.Fh_Y);

                basePlate.x_minus_plateEdge_to_pad = x_plateEdge_to_pad_basic - feccentricity_x;
                basePlate.y_minus_plateEdge_to_pad = y_plateEdge_to_pad_basic - feccentricity_y;

                basePlate.x_plus_plateEdge_to_pad = x_plateEdge_to_pad_basic + feccentricity_x;
                basePlate.y_plus_plateEdge_to_pad = y_plateEdge_to_pad_basic + feccentricity_y;

                basePlate.AnchorArrangement.SetEdgeDistances(basePlate, pad);
            }
        }

        //To Mato - prosim o review
        public void SetJointIsSelectedForMaterialListAccordingToMember()
        {
            if (m_MainMember != null)
            {
                if (m_MainMember.BIsSelectedForMaterialList) { BIsSelectedForMaterialList = true; return; }
            }
            if (m_SecondaryMembers != null)
            {
                foreach (CMember m in m_SecondaryMembers)
                {
                    if (m.BIsSelectedForMaterialList) { BIsSelectedForMaterialList = true; return; }
                }
            }

            BIsSelectedForMaterialList = false;
        }

        public virtual CConnectionJointTypes RecreateJoint()
        {
            return null;
        }
    }
}
