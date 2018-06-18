using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConnectionJointTypes : CEntity
    {
        public CPlate[] m_arrPlates;
        public CBolt[] m_arrBolts;
        public CWeld[] m_arrWelds;
        public CMember m_MainMember; // hlavny prvok (spravidla najmasivnejsi) ku ktoremu sa pripajaju jeden alebo viacero dalsich sekundarnych
        public CMember[] m_SecondaryMembers; // zoznam sekundarnych prvkov, TODO - asi bude potrebne upravit podla jednotlivych typov prvkov v spoji (nosnik / diagonala / stlp)
        public CNode m_Node;
        public Point3D m_ControlPoint;
        public CNode[] m_arrAssignedNodesWithJointType;

        public CConnectionJointTypes() { }

        public CConnectionJointTypes(int iNumberOfAssignedNodes, int arrPlatesSize, int arrBoltsSize, int arrWeldsSize)
        {
            m_arrAssignedNodesWithJointType = new CNode[iNumberOfAssignedNodes];

            m_arrPlates = arrPlatesSize != 0 ? new CPlate[arrPlatesSize] : null;
            m_arrBolts = arrBoltsSize != 0 ? new CBolt[arrBoltsSize] : null;
            m_arrWelds = arrWeldsSize != 0 ? new CWeld[arrWeldsSize] : null;
        }

        public CConnectionJointTypes(CPlate[] arrPlatesTemp, CBolt[] arrBoltsTemp, CWeld[] arrWeldsTemp)
        {
            m_arrPlates = new CPlate[arrPlatesTemp.Length];

            for (int i = 0; i < arrPlatesTemp.Length; i++)
            {
                m_arrPlates[i] = arrPlatesTemp[i];
            }

            m_arrBolts = new CBolt[arrBoltsTemp.Length];

            for (int i = 0; i < arrBoltsTemp.Length; i++)
            {
                m_arrBolts[i] = arrBoltsTemp[i];
            }

            m_arrWelds = new CWeld[arrWeldsTemp.Length];

            for (int i = 0; i < arrWeldsTemp.Length; i++)
            {
                m_arrWelds[i] = arrWeldsTemp[i];
            }
        }
    }
}
