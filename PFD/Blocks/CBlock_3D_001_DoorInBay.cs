using BaseClasses;
using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace PFD
{
    public class CBlock_3D_001_DoorInBay : CBlock
    {
        public CBlock_3D_001_DoorInBay(
            DoorProperties prop,
            float fLimitDistanceFromColumn,       // Limit value to generate girts on the left or right side of opening
            float fBottomGirtPosition,            // Vertical position of first girt
            float fDist_Girt,                     // Vertical regular distance between girts
            CMember referenceGirt_temp,           // Reference girt object in bay
            CMember ColumnLeft,                   // Left column of bay
            CMember ColumnRight,                  // Right column of bay
            float fBayWidth,
            float fBayHeight,
            float fUpperGirtLimit,                // Vertical limit to generate last girt (cant' be too close or in colision with eave purlin or rafter)
            bool bIsReverseGirtSession = false,   // Front or back wall bay can have reverse direction of girts in X
            bool bIsFirstBayInFrontorBackSide = false,
            bool bIsLastBayInFrontorBackSide = false)
        {
            BuildingSide = prop.sBuildingSide;
            ReferenceGirt = referenceGirt_temp;

            // TODO napojit premennu na hlavny model a pripadne dat moznost uzivatelovi nastavit hodnotu 0 - 30 mm
            float fCutOffOneSide = 0.005f; // Cut 5 mm from each side of member

            // Basic validation
            if ((prop.fDoorsWidth + prop.fDoorCoordinateXinBlock) > fBayWidth)
                throw new Exception("Door is defined out of frame bay."); // Door is defined out of frame bay

            EMemberType_FS eTypeColumn;
            EMemberType_FS eTypeLintel;
            CCrSc crscColumn;
            CCrSc crscLintel;

            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[2];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00(0, "G550‡", 200e+9f, 0.3f);

            // Cross-sections
            // TODO - add to cross-section parameters

            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = ReferenceGirt.CrScStart; // Girts

            if (prop.sDoorType == "Personnel Door")
            {
                // Personnel door
                // One cross-section
                m_arrCrSc[1] = new CCrSc_3_10075_BOX(0, 0.1f, 0.1f, 0.00075f, Colors.Red); // Door frame
                m_arrCrSc[1].Name_short = "10075";
                m_arrCrSc[1].m_Mat = m_arrMat[0];
                m_arrCrSc[1].ID = (int)EMemberType_FS_Position.DoorFrame;

                eTypeColumn = EMemberType_FS.eDF;
                eTypeLintel = EMemberType_FS.eDF;

                crscColumn = m_arrCrSc[1];
                crscLintel = m_arrCrSc[1];
            }
            else // if (prop.sDoorType == "Roller Door")
            {
                // Roller door
                // Two cross-sections
                int arraysizeoriginal;

                // Cross-sections
                arraysizeoriginal = m_arrCrSc.Length;

                Array.Resize(ref m_arrCrSc, arraysizeoriginal + 1); // ( + one cross-section)

                // Trimmer
                m_arrCrSc[1] = new CCrSc_3_270XX_C_BACK_TO_BACK(0, 0.27f, 0.14f, 0.02f, 0.00115f, Colors.Beige); // Door trimmer
                m_arrCrSc[1].Name_short = "270115btb";
                m_arrCrSc[1].m_Mat = m_arrMat[0];
                m_arrCrSc[1].ID = (int)EMemberType_FS_Position.DoorTrimmer;

                eTypeColumn = EMemberType_FS.eDT;
                crscColumn = m_arrCrSc[1];

                // Lintel
                m_arrCrSc[2] = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00095f, Colors.Chocolate); // Door lintel
                m_arrCrSc[2].Name_short = "27095";
                m_arrCrSc[2].m_Mat = m_arrMat[0];
                m_arrCrSc[2].ID = (int)EMemberType_FS_Position.DoorLintel;

                eTypeLintel = EMemberType_FS.eDL;
                crscLintel = m_arrCrSc[2];
            }

            INumberOfGirtsToDeactivate = (int)((prop.fDoorsHeight - fBottomGirtPosition) / fDist_Girt) + 1; // Number of intermediate girts + Bottom Girt

            bool bDoorToCloseToLeftColumn = false; // true - generate girts only on one side, false - generate girts on both sides of door
            bool bDoorToCloseToRightColumn = false; // true - generate girts only on one side, false - generate girts on both sides of door

            if (prop.fDoorCoordinateXinBlock < fLimitDistanceFromColumn)
                bDoorToCloseToLeftColumn = true; // Door is to close to the left column

            if((fBayWidth - (prop.fDoorCoordinateXinBlock + prop.fDoorsWidth)) < fLimitDistanceFromColumn)
                bDoorToCloseToRightColumn = true; // Door is to close to the right column

            int iNumberOfGirtsSequences;

            if (bDoorToCloseToLeftColumn && bDoorToCloseToRightColumn || fBottomGirtPosition > prop.fDoorsHeight)
                iNumberOfGirtsSequences = 0;  // No girts (not generate girts, just door frame members)
            else if (bDoorToCloseToLeftColumn || bDoorToCloseToRightColumn)
                iNumberOfGirtsSequences = 1; // Girts only on one side of door
            else
                iNumberOfGirtsSequences = 2; // Girts on both sides of door

            int iNodesForGirts = INumberOfGirtsToDeactivate * iNumberOfGirtsSequences * 2;
            int iMembersGirts = INumberOfGirtsToDeactivate * iNumberOfGirtsSequences;
            int iNumberOfColumns = 2;
            int iNumberOfLintels = 1;

            float fLimitOfLintelAndGirtDistance = 0.2f;
            if ((fBottomGirtPosition + INumberOfGirtsToDeactivate * fDist_Girt) - prop.fDoorsHeight < fLimitOfLintelAndGirtDistance)
                iNumberOfLintels = 0; // Not generate lintel - girt is close to the top edge of door

            m_arrNodes = new CNode[iNodesForGirts + 2 * iNumberOfColumns + 2 * iNumberOfLintels];
            m_arrMembers = new CMember[iMembersGirts + iNumberOfColumns + iNumberOfLintels];

            // Block Nodes Coordinates
            // Coordinates of girt nodes

            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of door)
            {
                int iNumberOfNodesOnOneSide = INumberOfGirtsToDeactivate * 2;

                float fxcoordinate_start = i * (prop.fDoorCoordinateXinBlock + prop.fDoorsWidth);
                float fxcoordinate_end = i == 0 ? prop.fDoorCoordinateXinBlock : fBayWidth;

                if (bDoorToCloseToLeftColumn) // Generate only second sequence of girt nodes
                {
                    fxcoordinate_start = prop.fDoorCoordinateXinBlock + prop.fDoorsWidth;
                    fxcoordinate_end = fBayWidth;
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
                m_arrNodes[iNodesForGirts + i * iNumberOfColumns] = new CNode(iNodesForGirts + i * iNumberOfColumns + 1, prop.fDoorCoordinateXinBlock + i * prop.fDoorsWidth, 0, 0, 0);

                // TODO - Refaktorovat validaciu hornej suradnice stlpa s window
                // Vertical coordinate
                float fz = fBottomGirtPosition + INumberOfGirtsToDeactivate * fDist_Girt;
                fz = Math.Min(fz, fBayHeight); // Top node z-coordinate must be less or equal to the bay height

                // Ak nie je vygenerovany girt pretoze je velmi blizko eave purlin (left, right) alebo rafter (front, back)
                // napajame stlpiky priamo na eave purlin alebo rafter, tj. suradcnica horneho bodu stlpika je rovna fBayHeight
                if (fBayHeight - fz < fUpperGirtLimit)
                    fz = fBayHeight;

                m_arrNodes[iNodesForGirts + i * iNumberOfColumns + 1] = new CNode(iNodesForGirts + i * iNumberOfColumns + 1 + 1, prop.fDoorCoordinateXinBlock + i * prop.fDoorsWidth, 0, fz, 0);
            }

            // Coordinates of door lintel nodes
            if (iNumberOfLintels > 0)
            {
                m_arrNodes[iNodesForGirts + 2 * iNumberOfColumns] = new CNode(iNodesForGirts + 2 * iNumberOfColumns + 1, prop.fDoorCoordinateXinBlock, 0, prop.fDoorsHeight, 0);
                m_arrNodes[iNodesForGirts + 2 * iNumberOfColumns + 1] = new CNode(iNodesForGirts + 2 * iNumberOfColumns + 1 + 1, prop.fDoorCoordinateXinBlock + prop.fDoorsWidth, 0, prop.fDoorsHeight, 0);
            }

            // Block Members
            // TODO - add to block parameters

            float fGirtAllignmentStart = bIsReverseGirtSession ? ReferenceGirt.FAlignment_End : ReferenceGirt.FAlignment_Start; // Main column of a frame
            float fGirtAllignmentEnd = -0.5f * (float)crscColumn.b - fCutOffOneSide; // Door column
            CMemberEccentricity eccentricityGirtStart = bIsReverseGirtSession ? ReferenceGirt.EccentricityEnd : ReferenceGirt.EccentricityStart;
            CMemberEccentricity eccentricityGirtEnd = bIsReverseGirtSession ? ReferenceGirt.EccentricityStart : ReferenceGirt.EccentricityEnd;
            CMemberEccentricity eccentricityGirtStart_temp;
            CMemberEccentricity eccentricityGirtEnd_temp;
            float fGirtsRotation = bIsReverseGirtSession ? (float)(ReferenceGirt.DTheta_x + Math.PI) : (float)ReferenceGirt.DTheta_x;

            // Girt Members
            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of door)
            {
                int iNumberOfNodesOnOneSide = INumberOfGirtsToDeactivate * 2;

                //if (bDoorToCloseToLeftColumn) // Generate only second sequence of girt nodes
                //    i = 1;

                for (int j = 0; j < INumberOfGirtsToDeactivate; j++)
                {
                    // Alignment - switch start and end allignment for girts on the left side of door and the right side of door
                    float fGirtStartTemp = fGirtAllignmentStart;
                    float fGirtEndTemp = fGirtAllignmentEnd;

                    eccentricityGirtStart_temp = eccentricityGirtStart;
                    eccentricityGirtEnd_temp = eccentricityGirtEnd;

                    if (i == 1 || bDoorToCloseToLeftColumn) // If just right sequence of girts is generated switch allignment and eccentricity (???) need testing;
                    {
                        if (!bIsLastBayInFrontorBackSide) // Change allignment (different columns on bay sides)
                        {
                            fGirtStartTemp = fGirtAllignmentEnd;
                            fGirtEndTemp = fGirtAllignmentStart;

                            if (bIsFirstBayInFrontorBackSide) // First bay, right side, end connection to the intermediate column
                                fGirtEndTemp = ReferenceGirt.FAlignment_End;
                        }
                        else // Last bay - right side - end allignment to the main column
                        {
                            fGirtEndTemp = ReferenceGirt.FAlignment_Start;
                        }

                        eccentricityGirtStart_temp = eccentricityGirtEnd; // TODO - we need probably to change signs of values
                        eccentricityGirtEnd_temp = eccentricityGirtStart; // TODO - we need probably to change signs of values
                    }

                    m_arrMembers[i * INumberOfGirtsToDeactivate + j] = new CMember(i * INumberOfGirtsToDeactivate + j + 1, m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2], m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2 + 1], m_arrCrSc[0], EMemberType_FS.eG, eccentricityGirtStart_temp, eccentricityGirtEnd_temp, fGirtStartTemp, fGirtEndTemp, fGirtsRotation, 0);
                }
            }

            INumberOfGirtsGeneratedInBlock = iNumberOfGirtsSequences * INumberOfGirtsToDeactivate;

            // TODO - add to block parameters
            float fDoorColumnStart = 0.0f;
            float fDoorColumnEnd = (float)ReferenceGirt.CrScStart.y_min - fCutOffOneSide;
            float fDistanceBetweenGirtAndColumn_LCS_z_axis = 0.5f * (float)ReferenceGirt.CrScStart.h - 0.5f * (float)crscColumn.h;

            CMemberEccentricity feccentricityDoorColumnStart = new CMemberEccentricity(0f, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndColumn_LCS_z_axis : -eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndColumn_LCS_z_axis);
            CMemberEccentricity feccentricityDoorColumnEnd = new CMemberEccentricity(0f, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndColumn_LCS_z_axis : -eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndColumn_LCS_z_axis);
            float fDoorColumnRotation = (float)Math.PI / 2;

            // Rotate local axis about x
            if (BuildingSide == "Left" || BuildingSide == "Right")
            {
                fDoorColumnRotation += (float)Math.PI / 2;
            }

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (BuildingSide == "Left" || BuildingSide == "Back")
            {
                feccentricityDoorColumnStart.MFz_local *= -1.0f;
                feccentricityDoorColumnEnd.MFz_local *= -1.0f;
            }

            // Door columns
            m_arrMembers[iMembersGirts] = new CMember(iMembersGirts + 1, m_arrNodes[iNodesForGirts], m_arrNodes[iNodesForGirts + 1], crscColumn, eTypeColumn, feccentricityDoorColumnStart, feccentricityDoorColumnEnd, fDoorColumnStart, fDoorColumnEnd, fDoorColumnRotation, 0);
            m_arrMembers[iMembersGirts + 1] = new CMember(iMembersGirts + 1 + 1, m_arrNodes[iNodesForGirts + 2], m_arrNodes[iNodesForGirts + 2 + 1], crscColumn, eTypeColumn, feccentricityDoorColumnStart, feccentricityDoorColumnEnd, fDoorColumnStart, fDoorColumnEnd, fDoorColumnRotation, 0);

            // Door lintel (header)
            // TODO - add to block parameters
            float fDoorLintelStart = -0.5f * (float)crscColumn.b - fCutOffOneSide;
            float fDoorLintelEnd = -0.5f * (float)crscColumn.b - fCutOffOneSide;
            float fDistanceBetweenGirtAndLintel_LCS_z_axis = 0.5f * (float)ReferenceGirt.CrScStart.h - 0.5f * (float)crscLintel.h;

            CMemberEccentricity feccentricityDoorLintelStart = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndLintel_LCS_z_axis : -eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndLintel_LCS_z_axis);
            CMemberEccentricity feccentricityDoorLintelEnd = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndLintel_LCS_z_axis : -eccentricityGirtStart.MFz_local + fDistanceBetweenGirtAndLintel_LCS_z_axis);
            float fDoorLintelRotation = (float)Math.PI / 2;

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (BuildingSide == "Left" || BuildingSide == "Back")
            {
                feccentricityDoorLintelStart.MFz_local *= -1.0f;
                feccentricityDoorLintelEnd.MFz_local *= -1.0f;
            }

            if (iNumberOfLintels > 0)
            {
                m_arrMembers[iMembersGirts + iNumberOfColumns] = new CMember(iMembersGirts + iNumberOfColumns + 1, m_arrNodes[iNodesForGirts + iNumberOfColumns * 2], m_arrNodes[iNodesForGirts + iNumberOfColumns * 2 + 1], crscLintel, eTypeLintel, feccentricityDoorLintelStart, feccentricityDoorLintelEnd, fDoorLintelStart, fDoorLintelEnd, fDoorLintelRotation, 0);
            }

            // Connection Joints
            m_arrConnectionJoints = new List<CConnectionJointTypes>();

            // Girt Member Joints
            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of door)
            {
                for (int j = 0; j < INumberOfGirtsToDeactivate; j++)
                {
                    CMember currentColumnToConnectStart = ColumnLeft; // Column
                    CMember currentColumnToConnectEnd = m_arrMembers[iMembersGirts]; // Door Column
                    bool bConsiderMainMemberWidthStart = true;
                    bool bConsiderMainMemberWidthEnd = true;
                    CMember current_member = m_arrMembers[i * INumberOfGirtsToDeactivate + j]; // Girt

                    if (bIsFirstBayInFrontorBackSide)
                    {
                        bConsiderMainMemberWidthStart = false;
                    }

                    if (i == 1 || bDoorToCloseToLeftColumn) // If just right sequence of girts is generated
                    {
                        currentColumnToConnectStart = m_arrMembers[iMembersGirts + 1]; // Door Column
                        currentColumnToConnectEnd = ColumnRight;

                        if (bIsLastBayInFrontorBackSide) // Different columns on bay sides
                        {
                            bConsiderMainMemberWidthEnd = false;
                        }
                    }

                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, currentColumnToConnectStart, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, bConsiderMainMemberWidthStart, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, currentColumnToConnectEnd, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, bConsiderMainMemberWidthEnd, true));
                }
            }

            // Column Joints
            for (int i = 0; i < iNumberOfColumns; i++) // Each created column
            {
                CMember current_member = m_arrMembers[iMembersGirts + i];
                // TODO - dopracovat moznosti kedy je stlpik dveri pripojeny k eave purlin, main rafter a podobne (nemusi to byt vzdy girt)

                // Bottom - columns is connected to the concrete foundation (use different type of plate ???)

                // TODO - spoj nemoze mat MainMember = null, je potrebne pre alternativu T001 zapracovat aj tuto moznost
                //m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, null, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true));
                m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member, true));
                // Top
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, ReferenceGirt, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
            }

            // Lintel (header) Joint
            if (iNumberOfLintels > 0)
            {
                CMember current_member = m_arrMembers[iMembersGirts + iNumberOfColumns];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, m_arrMembers[iMembersGirts], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + 1], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
            }
        }
    }
}
