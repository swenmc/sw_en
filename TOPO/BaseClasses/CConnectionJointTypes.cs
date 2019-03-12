using MATERIAL;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConnectionJointTypes : CEntity3D
    {
        public CPlate[] m_arrPlates;
        public CConnector[] m_arrConnectors;
        public CWeld[] m_arrWelds;
        public CMember m_MainMember; // hlavny prvok (spravidla najmasivnejsi) ku ktoremu sa pripajaju jeden alebo viacero dalsich sekundarnych
        public CMember[] m_SecondaryMembers; // zoznam sekundarnych prvkov, TODO - asi bude potrebne upravit podla jednotlivych typov prvkov v spoji (nosnik / diagonala / stlp)
        public CNode m_Node;
        public Point3D m_ControlPoint;
        public CNode[] m_arrAssignedNodesWithJointType;
        public bool bIsJointDefinedinGCS;
        public Model3DGroup Visual_ConnectionJoint;

        private CJointDesignDetails m_DesignDetails;
        public CJointDesignDetails DesignDetails
        {
            get { return m_DesignDetails; }
            set { m_DesignDetails = value; }
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
    }
}
