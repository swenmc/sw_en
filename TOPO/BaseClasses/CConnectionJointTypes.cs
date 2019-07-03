using System;
using MATERIAL;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConnectionJointTypes : CEntity3D
    {
        private EJointType m_JointType;

        public CPlate[] m_arrPlates;
        public CConnector[] m_arrConnectors;
        public CWeld[] m_arrWelds;
        public CMember m_MainMember; // hlavny prvok (spravidla najmasivnejsi) ku ktoremu sa pripajaju jeden alebo viacero dalsich sekundarnych
        public CMember[] m_SecondaryMembers; // zoznam sekundarnych prvkov, TODO - asi bude potrebne upravit podla jednotlivych typov prvkov v spoji (nosnik / diagonala / stlp)
        public CNode m_Node;
        public Point3D m_ControlPoint;
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

            // Joint is generated
            BIsGenerated = true;
            // Set as default property that joint should be displayed
            BIsDisplayed = true;
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

            // Joint is generated
            BIsGenerated = true;
            // Set as default property that joint should be displayed
            BIsDisplayed = true;
        }


        // Pomocna funkcia pre base plates - nastavenie typu plechu podla prierezu a nastavenie screwArrangement
        protected void SetPlateTypeAndScrewArrangement(string sSectionNameDatabase, CScrew referenceScrew, float fh_plate, out string platePrefix, out CScrewArrangement screwArrangement)
        {
            // TODO - Urobit nastavovanie rozmerov dynamicky podla velkosti prierezu / vyztuh a plechu
            // Nacitat velkosti vyztuh z parametrov prierezu, medzery a polohu skrutiek urcovat dynamicky
            CScrewArrangement_BX_2 screwArrangement1_10075 = new CScrewArrangement_BX_2(referenceScrew, fh_plate, fh_plate - 2 * 0.006f - 2 * 0.002f, 0.023f,
                    2, 1, 0.02f, 0.015f, 0.03f, 0.03f,
                    2, 1, 0.02f, 0.050f, 0.03f, 0.03f,
                    2, 1, 0.02f, 0.085f, 0.03f, 0.03f);
            CScrewArrangement_BX_1 screwArrangement1_63020 = new CScrewArrangement_BX_1(referenceScrew, fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,
                    3, 5, 0.05f, 0.029f, 0.05f, 0.05f,
                    3, 5, 0.05f, 0.401f, 0.05f, 0.05f);
            CScrewArrangement_BX_1 screwArrangement1_50020 = new CScrewArrangement_BX_1(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.132f,
                    3, 5, 0.05f, 0.029f, 0.05f, 0.05f,
                    3, 5, 0.05f, 0.330f, 0.05f, 0.05f);
            CScrewArrangement_BX_2 screwArrangement2 = new CScrewArrangement_BX_2(referenceScrew, fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,
                    3, 1, 0.025f, 0.03f, 0.03f, 0.05f,
                    3, 1, 0.025f, 0.14f, 0.03f, 0.05f,
                    3, 1, 0.025f, 0.26f, 0.03f, 0.05f);

            if (sSectionNameDatabase == "10075")
            {
                platePrefix = "BI";
                screwArrangement = screwArrangement1_10075; // TODO - definovat iny typ
            }
            else if (sSectionNameDatabase == "27055")
            {
                platePrefix = "BG";
                screwArrangement = screwArrangement2;
            }
            else if (sSectionNameDatabase == "27095")
            {
                platePrefix = "BG";
                screwArrangement = screwArrangement2;
            }
            else if (sSectionNameDatabase == "27095n")
            {
                platePrefix = "BB";
                screwArrangement = screwArrangement2;
            }
            else if (sSectionNameDatabase == "270115")
            {
                platePrefix = "BG";
                screwArrangement = screwArrangement2;
            }
            else if (sSectionNameDatabase == "270115btb")
            {
                platePrefix = "BA";
                screwArrangement = screwArrangement2;
            }
            else if (sSectionNameDatabase == "270115n")
            {
                platePrefix = "BB";
                screwArrangement = screwArrangement2;
            }
            else if (sSectionNameDatabase == "50020")
            {
                platePrefix = "BD";
                screwArrangement = screwArrangement1_50020;
            }
            else if (sSectionNameDatabase == "50020n")
            {
                platePrefix = "BE";
                screwArrangement = screwArrangement1_50020;
            }
            else if (sSectionNameDatabase == "63020")
            {
                platePrefix = "BF";
                screwArrangement = screwArrangement1_63020;
            }
            else if (sSectionNameDatabase == "63020s1")
            {
                platePrefix = "BF";
                screwArrangement = screwArrangement1_63020;
            }
            else if (sSectionNameDatabase == "63020s2")
            {
                platePrefix = "BF";
                screwArrangement = screwArrangement1_63020;
            }
            else
            {
                platePrefix = "";
                screwArrangement = null;
                throw new NotImplementedException("Invalid cross-section name: " + sSectionNameDatabase + ". \n" +
                                                  "Base plate of cross-section with this name is not implemented");
            }
        }

        public virtual CConnectionJointTypes RecreateJoint()
        {
            return null;
        }
    }
}
