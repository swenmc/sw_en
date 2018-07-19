using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BaseClasses;
using MATERIAL;
using CRSC;

namespace sw_en_GUI.EXAMPLES._3D
{
    public class CBlock_3D_001_DoorInBay : CBlock
    {
        public CBlock_3D_001_DoorInBay(string sBuildingSide, float fDoorHeight, float fDoorWidth, float fDoorCoordinateXinBlock, float fLimitDistanceFromColumn , float fBottomGirtPosition, float fDist_Girt, CMember referenceGirt, CMember Colummn, float fL1_bayofframe)
        {
            // TODO napojit premennu na hlavny model a pripadne dat moznost uzivatelovi nastavit hodnotu 0 - 30 mm
            float fCutOffOneSide = 0.005f; // Cut 5 mm from each side of member
            /*
            fDoorsHeight = 2.1f;
            fDoorsWidth = 1.1f;
            fDoorCoordinateXinBlock = 0.6f; // From 0 to Bay Width (distance L1) - DoorsWidth // Position of Door in Bay
            */

            // Basic validation
            if ((fDoorWidth + fDoorCoordinateXinBlock) > fL1_bayofframe)
                throw new Exception(); // Door is defined out of frame bay

            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[2];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // TODO - add to cross-section parameters

            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = referenceGirt.CrScStart; // Girts
            m_arrCrSc[1] = new CCrSc_3_10075_BOX(0.1f, 0.1f, 0.00075f, Colors.Red); // Door frame
            m_arrCrSc[1].Name = "Box 10075";

            INumberOfGirtsToDeactivate = (int)((fDoorHeight - fBottomGirtPosition) / fDist_Girt) + 1; // Number of intermediate girts + Bottom Girt

            bool bDoorToCloseToLeftColumn = false; // true - generate girts only on one side, false - generate girts on both sides of door
            bool bDoorToCloseToRightColumn = false; // true - generate girts only on one side, false - generate girts on both sides of door

            if (fDoorCoordinateXinBlock < fLimitDistanceFromColumn)
                bDoorToCloseToLeftColumn = true; // Door are to close to the left column

            if((fL1_bayofframe - (fDoorCoordinateXinBlock + fDoorWidth)) < fLimitDistanceFromColumn)
                bDoorToCloseToRightColumn = true; // Door are to close to the right column

            int iNumberOfGirtsSequences;

            if (bDoorToCloseToLeftColumn && bDoorToCloseToRightColumn || fBottomGirtPosition > fDoorHeight)
                iNumberOfGirtsSequences = 0;  // No girts (not generate girts, just door frame members)
            else if (bDoorToCloseToLeftColumn || bDoorToCloseToRightColumn)
                iNumberOfGirtsSequences = 1; // Girts only on one side of doors
            else
                iNumberOfGirtsSequences = 2; // Girts on both sides of doors

            int iNodesForGirts = INumberOfGirtsToDeactivate * iNumberOfGirtsSequences * 2;
            int iMembersGirts = INumberOfGirtsToDeactivate * iNumberOfGirtsSequences;
            int iNumberOfColumns = 2;
            int iNumberOfLintels = 1;

            float fLimitOfLintelAndGirtDistance = 0.2f;
            if ((fBottomGirtPosition + INumberOfGirtsToDeactivate * fDist_Girt) - fDoorHeight < fLimitOfLintelAndGirtDistance)
                iNumberOfLintels = 0; // Not generate lintel - girt is close to the top edge of doors

            m_arrNodes = new CNode[iNodesForGirts + 2 * iNumberOfColumns + 2 * iNumberOfLintels];
            m_arrMembers = new CMember[iMembersGirts + iNumberOfColumns + iNumberOfLintels];

            // Block Nodes Coordinates
            // Coordinates of girt nodes

            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of door)
            {
                int iNumberOfNodesOnOneSide = INumberOfGirtsToDeactivate * 2;

                float fxcoordinate_start = i * (fDoorCoordinateXinBlock + fDoorWidth);
                float fxcoordinate_end = i == 0 ? fDoorCoordinateXinBlock : fL1_bayofframe;

                if (bDoorToCloseToLeftColumn) // Generate only second sequence of girt nodes
                {
                    fxcoordinate_start = fDoorCoordinateXinBlock + fDoorWidth;
                    fxcoordinate_end = fL1_bayofframe;
                }

                for (int j = 0; j < INumberOfGirtsToDeactivate; j++)
                {
                    // Start node of member
                    m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2] = new CNode(i * iNumberOfNodesOnOneSide + j * 2 + 1, fxcoordinate_start, 0, fBottomGirtPosition + j * fDist_Girt, 0);

                    // End node of member
                    m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2 + 1] = new CNode(i * iNumberOfNodesOnOneSide + j * 2 + 1 + 1, fxcoordinate_end, 0, fBottomGirtPosition + j * fDist_Girt, 0);
                }
            }

            // Coordinates of door columns nodes

            for (int i = 0; i < iNumberOfColumns; i++) // (Column on the left side and the right side of door)
            {
                m_arrNodes[iNodesForGirts + i * iNumberOfColumns] = new CNode(iNodesForGirts + i * iNumberOfColumns + 1, fDoorCoordinateXinBlock + i * fDoorWidth, 0, 0, 0);
                m_arrNodes[iNodesForGirts + i * iNumberOfColumns + 1] = new CNode(iNodesForGirts + i * iNumberOfColumns + 1 + 1, fDoorCoordinateXinBlock + i * fDoorWidth, 0, fBottomGirtPosition + INumberOfGirtsToDeactivate * fDist_Girt, 0);
            }

            // Coordinates of door lintel nodes
            if (iNumberOfLintels > 0)
            {
                m_arrNodes[iNodesForGirts + 2 * iNumberOfColumns] = new CNode(iNodesForGirts + 2 * iNumberOfColumns + 1, fDoorCoordinateXinBlock, 0, fDoorHeight, 0);
                m_arrNodes[iNodesForGirts + 2 * iNumberOfColumns + 1] = new CNode(iNodesForGirts + 2 * iNumberOfColumns + 1 + 1, fDoorCoordinateXinBlock + fDoorWidth, 0, fDoorHeight, 0);
            }

            // Block Members
            // TODO - add to block parameters

            float fGirtAllignmentStart = referenceGirt.FAlignment_Start; // Main column of a frame
            float fGirtAllignmentEnd = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide; // Door column
            CMemberEccentricity eccentricityGirtStart = referenceGirt.EccentricityStart;
            CMemberEccentricity eccentricityGirtEnd = referenceGirt.EccentricityEnd;
            CMemberEccentricity eccentricityGirtStart_temp;
            CMemberEccentricity eccentricityGirtEnd_temp;
            float fGirtsRotation = (float)referenceGirt.DTheta_x;

            // Girt Members
            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of door)
            {
                int iNumberOfNodesOnOneSide = INumberOfGirtsToDeactivate * 2;

                //if (bDoorToCloseToLeftColumn) // Generate only second sequence of girt nodes
                //    i = 1;

                for (int j = 0; j < INumberOfGirtsToDeactivate; j++)
                {
                    // Alignment - switch stanrt and end allignment for girts on the left side of door and the right side of door
                    float fGirtStartTemp = fGirtAllignmentStart;
                    float fGirtEndTemp = fGirtAllignmentEnd;
                    eccentricityGirtStart_temp = eccentricityGirtStart;
                    eccentricityGirtEnd_temp = eccentricityGirtEnd;

                    if (i == 1 || bDoorToCloseToLeftColumn) // If just right sequence of girts is generated switch allignment and eccentricity (???) need testing;
                    {
                        fGirtStartTemp = fGirtAllignmentEnd;
                        fGirtEndTemp = fGirtAllignmentStart;
                        eccentricityGirtStart_temp = eccentricityGirtEnd; // TODO - we need probably to change signs of values
                        eccentricityGirtEnd_temp = eccentricityGirtStart; // TODO - we need probably to change signs of values
                    }

                    m_arrMembers[i * INumberOfGirtsToDeactivate + j] = new CMember(i * INumberOfGirtsToDeactivate + j + 1, m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2], m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2 + 1], m_arrCrSc[0], EMemberType_FormSteel.eG, eccentricityGirtStart_temp, eccentricityGirtEnd_temp, fGirtStartTemp, fGirtEndTemp, fGirtsRotation, 0);
                    m_arrMembers[i * INumberOfGirtsToDeactivate + j].BIsDisplayed = true;
                }
            }

            // TODO - add to block parameters
            float fDoorColumnStart = 0.0f;
            float fDoorColumnEnd = -0.5f * (float)referenceGirt.CrScStart.b - fCutOffOneSide;
            CMemberEccentricity feccentricityDoorColumnStart = new CMemberEccentricity(0f, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local - 0.5f * (float)m_arrCrSc[1].h);
            CMemberEccentricity feccentricityDoorColumnEnd = new CMemberEccentricity(0f, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local - 0.5f * (float)m_arrCrSc[1].h);
            float fDoorColumnRotation = (float)Math.PI;

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (sBuildingSide == "Left")
            {
                feccentricityDoorColumnStart.MFz_local *= -1.0f;
                feccentricityDoorColumnEnd.MFz_local *= -1.0f;
            }

            // Door columns
            m_arrMembers[iMembersGirts] = new CMember(iMembersGirts + 1, m_arrNodes[iNodesForGirts], m_arrNodes[iNodesForGirts + 1], m_arrCrSc[1], EMemberType_FormSteel.eDF, feccentricityDoorColumnStart, feccentricityDoorColumnEnd, fDoorColumnStart, fDoorColumnEnd, fDoorColumnRotation, 0);
            m_arrMembers[iMembersGirts + 1] = new CMember(iMembersGirts + 1 + 1, m_arrNodes[iNodesForGirts + 2], m_arrNodes[iNodesForGirts + 2 + 1], m_arrCrSc[1], EMemberType_FormSteel.eDF, feccentricityDoorColumnStart, feccentricityDoorColumnEnd, fDoorColumnStart, fDoorColumnEnd, fDoorColumnRotation, 0);

            m_arrMembers[iMembersGirts].BIsDisplayed = true;
            m_arrMembers[iMembersGirts + 1].BIsDisplayed = true;

            // Door lintel (header)
            // TODO - add to block parameters
            float fDoorLintelStart = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;
            float fDoorLintelEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;
            CMemberEccentricity feccentricityDoorLintelStart = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local - 0.5f * (float)m_arrCrSc[1].h);
            CMemberEccentricity feccentricityDoorLintelEnd = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local - 0.5f * (float)m_arrCrSc[1].h);
            float fDoorLintelRotation = (float)Math.PI / 2;

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (sBuildingSide == "Left")
            {
                feccentricityDoorLintelStart.MFz_local *= -1.0f;
                feccentricityDoorLintelEnd.MFz_local *= -1.0f;
            }

            if (iNumberOfLintels > 0)
            {
                m_arrMembers[iMembersGirts + iNumberOfColumns] = new CMember(iMembersGirts + iNumberOfColumns + 1, m_arrNodes[iNodesForGirts + iNumberOfColumns * 2], m_arrNodes[iNodesForGirts + iNumberOfColumns * 2 + 1], m_arrCrSc[1], EMemberType_FormSteel.eDF, feccentricityDoorLintelStart, feccentricityDoorLintelEnd, fDoorLintelStart, fDoorLintelEnd, fDoorLintelRotation, 0);
                m_arrMembers[iMembersGirts + iNumberOfColumns].BIsDisplayed = true;
            }

            // Connection Joints
            m_arrConnectionJoints = new List<CConnectionJointTypes>();

            // Girt Joints
            if (iMembersGirts > 0)
            {
                for (int i = 0; i < iMembersGirts; i++) // Each created girt
                {
                    CMember current_member = m_arrMembers[i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, Colummn, current_member, true, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, Colummn, current_member, true, true));
                }
            }

            // Column Joints (Stile)
            for (int i = 0; i < iNumberOfColumns; i++) // Each created girt
            {
                CMember current_member = m_arrMembers[iMembersGirts + i];
                // TODO - dopracovat moznosti kedy je stlpik dveri pripojeny k eave purlin, main rafter a podobne (nemusi to byt vzdy girt)

                // Bottom - columns is connected to the concrete foundation (use different type of plate ???)
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, null, current_member, true, true));
                // Top
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, referenceGirt, current_member, true, true));
            }

            // Lintel (header) Joint
            if (iNumberOfLintels > 0)
            {
                CMember current_member = m_arrMembers[iMembersGirts + iNumberOfColumns];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, m_arrMembers[iMembersGirts], current_member, true, true));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + 1], current_member, true, true));
            }
        }
    }
}
