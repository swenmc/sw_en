using BaseClasses;
using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace PFD
{
    public class CBlock_3D_002_WindowInBay : CBlock
    {
        public int iNumberOfGirtsUnderWindow;

        public CBlock_3D_002_WindowInBay(
            WindowProperties prop,
            float fLimitDistanceFromColumn,       // Limit value to generate girts on the left or right side of opening
            float fBottomGirtPosition,            // Vertical position of first girt
            float fDist_Girt,                     // Vertical regular distance between girts
            CMember referenceGirt_temp,           // Reference girt object in bay
            CMember GirtToConnectWindowColumns_Bottom, // Bottom window column to girt connection
            CMember GirtToConnectWindowColumns_Top,    // Top window column to girt connection (just in case that it is not connected to the eave purlin or edge rafter)
            CMember ColumnLeft,                   // Left column of bay
            CMember ColumnRight,                  // Right column of bay
            CMember referenceEavePurlin,          // Reference Eave Purlin
            float fBayWidth,
            float fBayHeight,
            float fUpperGirtLimit,                // Vertical limit to generate last girt (cant' be too close or in colision with eave purlin or rafter)
            bool bIsReverseGirtSession = false,   // Front or back wall bay can have reverse direction of girts in X
            bool bIsFirstBayInFrontorBackSide = false,
            bool bIsLastBayInFrontorBackSide = false)
        {
            BuildingSide = prop.sBuildingSide;
            ReferenceGirt = referenceGirt_temp;

            //prop.iNumberOfWindowColumns = 2; // Minimum is 2
            int iNumberOfHeaders = prop.iNumberOfWindowColumns - 1;
            int iNumberOfSills = prop.iNumberOfWindowColumns - 1;

            // TODO napojit premennu na hlavny model a pripadne dat moznost uzivatelovi nastavit hodnotu 0 - 30 mm
            float fCutOffOneSide = 0.005f; // Cut 5 mm from each side of member

            // Basic validation
            if ((prop.fWindowsWidth + prop.fWindowCoordinateXinBay) > fBayWidth)
                throw new Exception("Window is defined out of frame bay."); // Window is defined out of frame bay

            if ((prop.fWindowsHeight + prop.fWindowCoordinateZinBay) > fBayHeight)
                throw new Exception("Window is defined out of frame height"); // Window is defined out of frame height

            float fDistanceBetweenWindowColumns = prop.fWindowsWidth / (prop.iNumberOfWindowColumns - 1);

            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[2];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00(0, "G550‡", 200e+9f, 0.3f);

            // Cross-sections
            // TODO - add to cross-section parameters

            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = ReferenceGirt.CrScStart; // Girts
            m_arrCrSc[1] = new CCrSc_3_10075_BOX(0, 0.1f, 0.1f, 0.00075f, Colors.Red); // Window frame
            m_arrCrSc[1].Name_short = "10075";
            m_arrCrSc[1].m_Mat = m_arrMat[0];
            m_arrCrSc[1].ID = (int)EMemberType_FS_Position.WindowFrame;

            iNumberOfGirtsUnderWindow = (int)((prop.fWindowCoordinateZinBay - fBottomGirtPosition) / fDist_Girt) + 1;
            float fCoordinateZOfGirtUnderWindow = (iNumberOfGirtsUnderWindow - 1) * fDist_Girt + fBottomGirtPosition;

            if (prop.fWindowCoordinateZinBay <= fBottomGirtPosition)
            {
                iNumberOfGirtsUnderWindow = 0;
                fCoordinateZOfGirtUnderWindow = 0f;
            }

            INumberOfGirtsToDeactivate = (int)((prop.fWindowsHeight + prop.fWindowCoordinateZinBay - fCoordinateZOfGirtUnderWindow) / fDist_Girt); // Number of intermediate girts to deactivate

            bool bWindowColumnIsConnectedtoEavePurlin = false;
            bool bWindowToCloseToLeftColumn = false; // true - generate girts only on one side, false - generate girts on both sides of window
            bool bWindowToCloseToRightColumn = false; // true - generate girts only on one side, false - generate girts on both sides of window

            if (prop.fWindowCoordinateXinBay < fLimitDistanceFromColumn)
                bWindowToCloseToLeftColumn = true; // Window is to close to the left column

            if((fBayWidth - (prop.fWindowCoordinateXinBay + prop.fWindowsWidth)) < fLimitDistanceFromColumn)
                bWindowToCloseToRightColumn = true; // Window is to close to the right column

            int iNumberOfGirtsSequences;

            if (bWindowToCloseToLeftColumn && bWindowToCloseToRightColumn || fBottomGirtPosition > (prop.fWindowCoordinateZinBay + prop.fWindowsHeight))
                iNumberOfGirtsSequences = 0;  // No girts (not generate girts, just window frame members)
            else if (bWindowToCloseToLeftColumn || bWindowToCloseToRightColumn)
                iNumberOfGirtsSequences = 1; // Girts only on one side of window
            else
                iNumberOfGirtsSequences = 2; // Girts on both sides of window

            int iNodesForGirts = INumberOfGirtsToDeactivate * iNumberOfGirtsSequences * 2;
            int iMembersGirts = INumberOfGirtsToDeactivate * iNumberOfGirtsSequences;
            int iNodesForWindowColumns = prop.iNumberOfWindowColumns * 2;
            int iNodesForWindowHeaders = iNumberOfHeaders + 1;
            int iNodesForWindowSills = iNumberOfSills + 1;

            float fLimitOfHeaderOrSillAndGirtDistance = 0.1f;

            if ((fBottomGirtPosition + (INumberOfGirtsToDeactivate + iNumberOfGirtsUnderWindow) * fDist_Girt) - (prop.fWindowCoordinateZinBay + prop.fWindowsHeight) < fLimitOfHeaderOrSillAndGirtDistance)
            {
                iNumberOfHeaders = 0; // Not generate header - girt is close to the top edge of window
                iNodesForWindowHeaders = 0;
            }

            if (prop.fWindowCoordinateZinBay - (fBottomGirtPosition + ((iNumberOfGirtsUnderWindow - 1) * fDist_Girt)) < fLimitOfHeaderOrSillAndGirtDistance)
            {
                iNumberOfSills = 0; // Not generate sill - girt is close to the bottom edge of window
                iNodesForWindowSills = 0;
            }

            m_arrNodes = new CNode[iNodesForGirts + 2 * prop.iNumberOfWindowColumns + iNodesForWindowHeaders + iNodesForWindowSills];
            m_arrMembers = new CMember[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + iNumberOfSills];

            // Block Nodes Coordinates
            // Coordinates of girt nodes

            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of window)
            {
                int iNumberOfNodesOnOneSide = INumberOfGirtsToDeactivate * 2;

                float fxcoordinate_start = i * (prop.fWindowCoordinateXinBay + prop.fWindowsWidth);
                float fxcoordinate_end = i == 0 ? prop.fWindowCoordinateXinBay : fBayWidth;

                if (bWindowToCloseToLeftColumn) // Generate only second sequence of girt nodes
                {
                    fxcoordinate_start = prop.fWindowCoordinateXinBay + prop.fWindowsWidth;
                    fxcoordinate_end = fBayWidth;
                }

                for (int j = 0; j < INumberOfGirtsToDeactivate; j++)
                {
                    // Start node of member
                    m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2] = new CNode(i * iNumberOfNodesOnOneSide + j * 2 + 1, fxcoordinate_start, 0, fBottomGirtPosition + iNumberOfGirtsUnderWindow * fDist_Girt + j * fDist_Girt, 0);

                    // End node of member
                    m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2 + 1] = new CNode(i * iNumberOfNodesOnOneSide + j * 2 + 1 + 1, fxcoordinate_end, 0, fBottomGirtPosition + iNumberOfGirtsUnderWindow * fDist_Girt + j * fDist_Girt, 0);
                }
            }

            // Coordinates of window columns nodes
            for (int i = 0; i < prop.iNumberOfWindowColumns; i++) // (Column on the left side and the right side of window and also intermediate columns if necessary)
            {
                m_arrNodes[iNodesForGirts + i * 2] = new CNode(iNodesForGirts + i * 2 + 1, prop.fWindowCoordinateXinBay + i * fDistanceBetweenWindowColumns, 0, fCoordinateZOfGirtUnderWindow, 0);

                // TODO - Refaktorovat validaciu hornej suradnice stlpa s door
                // Vertical coordinate
                float fz = fBottomGirtPosition + iNumberOfGirtsUnderWindow * fDist_Girt + INumberOfGirtsToDeactivate * fDist_Girt;
                fz = Math.Min(fz, fBayHeight); // Top node z-coordinate must be less or equal to the bay height

                // Ak nie je vygenerovany girt pretoze je velmi blizko eave purlin (left, right) alebo rafter (front, back)
                // napajame stlpiky priamo na eave purlin alebo rafter, tj. suradcnica horneho bodu stlpika je rovna fBayHeight
                // TODO - zatial to plati len pre lavu a pravu stranu, pretoze nemam spocitanu maximalnu volnu vysku pre jednotlive bays na prednej a zadnej strane
                // Ak bude spravne urcena fBayHeight, tak (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right") zmazat
                if ((fBayHeight - fz < fUpperGirtLimit) && (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right"))
                {
                    fz = fBayHeight;
                    bWindowColumnIsConnectedtoEavePurlin = true;
                }

                m_arrNodes[iNodesForGirts + i * 2 + 1] = new CNode(iNodesForGirts + i * 2 + 1 + 1, prop.fWindowCoordinateXinBay + i * fDistanceBetweenWindowColumns, 0, fz, 0);
            }

            // Coordinates of window header nodes
            for (int i = 0; i < iNodesForWindowHeaders; i++) // (Headers between columns)
            {
                m_arrNodes[iNodesForGirts + iNodesForWindowColumns + i] = new CNode(iNodesForGirts + iNodesForWindowColumns + i + 1, prop.fWindowCoordinateXinBay + i * fDistanceBetweenWindowColumns, 0, prop.fWindowCoordinateZinBay + prop.fWindowsHeight, 0);
            }

            // Coordinates of window sill nodes
            for (int i = 0; i < iNodesForWindowSills; i++) // (Sills between columns)
            {
                m_arrNodes[iNodesForGirts + iNodesForWindowColumns + iNodesForWindowHeaders + i] = new CNode(iNodesForGirts + iNodesForWindowColumns + iNodesForWindowHeaders + i + 1, prop.fWindowCoordinateXinBay + i * fDistanceBetweenWindowColumns, 0, prop.fWindowCoordinateZinBay, 0);
            }

            // Block Members
            // TODO - add to block parameters

            float fGirtAllignmentStart = bIsReverseGirtSession ? ReferenceGirt.FAlignment_End : ReferenceGirt.FAlignment_Start; // Main column of a frame
            float fGirtAllignmentEnd = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide; // Window column
            CMemberEccentricity eccentricityGirtStart = bIsReverseGirtSession ? ReferenceGirt.EccentricityEnd : ReferenceGirt.EccentricityStart;
            CMemberEccentricity eccentricityGirtEnd = bIsReverseGirtSession ? ReferenceGirt.EccentricityStart : ReferenceGirt.EccentricityEnd;
            CMemberEccentricity eccentricityGirtStart_temp;
            CMemberEccentricity eccentricityGirtEnd_temp;
            float fGirtsRotation = bIsReverseGirtSession ? (float)(ReferenceGirt.DTheta_x + Math.PI) : (float)ReferenceGirt.DTheta_x;

            // Girt Members
            for (int i = 0; i < iNumberOfGirtsSequences; i++) // (Girts on the left side and the right side of window)
            {
                int iNumberOfNodesOnOneSide = INumberOfGirtsToDeactivate * 2;

                //if (bWindowToCloseToLeftColumn) // Generate only second sequence of girt nodes
                //    i = 1;

                for (int j = 0; j < INumberOfGirtsToDeactivate; j++)
                {
                    // Alignment - switch start and end allignment for girts on the left side of window and the right side of window
                    float fGirtStartTemp = fGirtAllignmentStart;
                    float fGirtEndTemp = fGirtAllignmentEnd;

                    eccentricityGirtStart_temp = eccentricityGirtStart;
                    eccentricityGirtEnd_temp = eccentricityGirtEnd;

                    if (i == 1 || bWindowToCloseToLeftColumn) // If just right sequence of girts is generated switch allignment and eccentricity (???) need testing;
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

                    m_arrMembers[i * INumberOfGirtsToDeactivate + j] = new CMember(i * INumberOfGirtsToDeactivate + j + 1, m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2], m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2 + 1], m_arrCrSc[0], ReferenceGirt.EMemberType, ReferenceGirt.EMemberTypePosition, eccentricityGirtStart_temp, eccentricityGirtEnd_temp, fGirtStartTemp, fGirtEndTemp, fGirtsRotation, 0);

                    // Set position type (same as reference girt)
                    m_arrMembers[i * INumberOfGirtsToDeactivate + j].EMemberTypePosition = ReferenceGirt.EMemberTypePosition;

                    //m_arrMembers[i * INumberOfGirtsToDeactivate + j].BIsDisplayed = true;
                }
            }

            INumberOfGirtsGeneratedInBlock = iNumberOfGirtsSequences * INumberOfGirtsToDeactivate;

            // TODO - add to block parameters
            float fWindowColumnStart = 0.0f;

            if(fBottomGirtPosition >= fCoordinateZOfGirtUnderWindow) // Window column is connected to the girt
                fWindowColumnStart = -(float)ReferenceGirt.CrScStart.y_max - fCutOffOneSide;

            float fWindowColumnEnd = (float)ReferenceGirt.CrScStart.y_min - fCutOffOneSide;

            if (bWindowColumnIsConnectedtoEavePurlin)
            {
                fWindowColumnEnd = (float)referenceEavePurlin.CrScStart.z_min - fCutOffOneSide;
            }

            CMemberEccentricity feccentricityWindowColumnStart = new CMemberEccentricity(0f, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h);
            CMemberEccentricity feccentricityWindowColumnEnd = new CMemberEccentricity(0f, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h);
            float fWindowColumnRotation = (float)Math.PI / 2;

            // Rotate local axis about x
            if (BuildingSide == "Left" || BuildingSide == "Right")
            {
                fWindowColumnRotation += (float)Math.PI / 2;
            }

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (BuildingSide == "Left" || BuildingSide == "Back")
            {
                feccentricityWindowColumnStart.MFz_local *= -1.0f;
                feccentricityWindowColumnEnd.MFz_local *= -1.0f;
            }

            // Window columns
            for (int i = 0; i < prop.iNumberOfWindowColumns; i++)
            {
                m_arrMembers[iMembersGirts + i] = new CMember(iMembersGirts + i + 1, m_arrNodes[iNodesForGirts + i * 2], m_arrNodes[iNodesForGirts + i * 2 + 1], m_arrCrSc[1], EMemberType_FS.eWF, EMemberType_FS_Position.WindowFrame, feccentricityWindowColumnStart, feccentricityWindowColumnEnd, fWindowColumnStart, fWindowColumnEnd, fWindowColumnRotation, 0);

                // Set position type
                //m_arrMembers[iMembersGirts + i].EMemberTypePosition = EMemberType_FS_Position.WindowFrame;
                //m_arrMembers[iMembersGirts + i].BIsDisplayed = true;
            }

            // Window (header)
            // TODO - add to block parameters
            float fWindowHeaderStart = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;
            float fWindowHeaderEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;
            CMemberEccentricity feccentricityWindowHeaderStart = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h);
            CMemberEccentricity feccentricityWindowHeaderEnd = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h);
            float fWindowHeaderRotation = (float)Math.PI / 2;

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (BuildingSide == "Left" || BuildingSide == "Back")
            {
                feccentricityWindowHeaderStart.MFz_local *= -1.0f;
                feccentricityWindowHeaderEnd.MFz_local *= -1.0f;
            }

            for (int i = 0; i < iNumberOfHeaders; i++)
            {
                m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + i] = new CMember(iMembersGirts + prop.iNumberOfWindowColumns + i + 1, m_arrNodes[iNodesForGirts + iNodesForWindowColumns + i], m_arrNodes[iNodesForGirts + iNodesForWindowColumns + i + 1], m_arrCrSc[1], EMemberType_FS.eWF, EMemberType_FS_Position.WindowFrame, feccentricityWindowHeaderStart, feccentricityWindowHeaderEnd, fWindowHeaderStart, fWindowHeaderEnd, fWindowHeaderRotation, 0);

                // Set position type
                //m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + i].EMemberTypePosition = EMemberType_FS_Position.WindowFrame;

                //m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + i].BIsDisplayed = true;
            }

            // Window (Sills)
            // TODO - add to block parameters
            float fWindowSillStart = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;
            float fWindowSillEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;
            CMemberEccentricity feccentricityWindowSillStart = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h);
            CMemberEccentricity feccentricityWindowSillEnd = new CMemberEccentricity(0, eccentricityGirtStart.MFz_local > 0 ? eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h : -eccentricityGirtStart.MFz_local + 0.5f * (float)m_arrCrSc[1].h);
            float fWindowSillRotation = (float)Math.PI / 2;

            // Set eccentricity sign depending on global rotation angle and building side (left / right)
            if (BuildingSide == "Left" || BuildingSide == "Back")
            {
                feccentricityWindowSillStart.MFz_local *= -1.0f;
                feccentricityWindowSillEnd.MFz_local *= -1.0f;
            }

            for (int i = 0; i < iNumberOfSills; i++)
            {
                m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i] = new CMember(iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i + 1, m_arrNodes[iNodesForGirts + iNodesForWindowColumns + iNodesForWindowHeaders + i], m_arrNodes[iNodesForGirts + iNodesForWindowColumns + iNodesForWindowHeaders + i + 1], m_arrCrSc[1], EMemberType_FS.eWF, EMemberType_FS_Position.WindowFrame, feccentricityWindowSillStart, feccentricityWindowSillEnd, fWindowSillStart, fWindowSillEnd, fWindowSillRotation, 0);

                // Set position type
                //m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i].EMemberTypePosition = EMemberType_FS_Position.WindowFrame;

                //m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i].BIsDisplayed = true;
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
                    CMember current_member = m_arrMembers[i * INumberOfGirtsToDeactivate + j]; // Girt

                    if (i == 1 || bWindowToCloseToLeftColumn) // If just right sequence of girts is generated
                    {
                        currentColumnToConnectStart = m_arrMembers[iMembersGirts + 1]; // Door Column
                        currentColumnToConnectEnd = ColumnRight;
                    }

                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, currentColumnToConnectStart, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, currentColumnToConnectEnd, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                }
            }

            // Column Joints
            for (int i = 0; i < prop.iNumberOfWindowColumns; i++) // Each created column
            {
                CMember current_member = m_arrMembers[iMembersGirts + i];
                // TODO - dopracovat moznosti kedy je stlpik okna pripojeny k eave purlin, main rafter a podobne (nemusi to byt vzdy girt)

                // Bottom - columns is connected to the concrete foundation or girt (use different type of plate ???)
                CMember mainMemberForColumnJoint_Bottom = GirtToConnectWindowColumns_Bottom;
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, mainMemberForColumnJoint_Bottom, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                
                // Top
                CMember mainMemberForColumnJoint_Top = GirtToConnectWindowColumns_Top;

                if (bWindowColumnIsConnectedtoEavePurlin && (BuildingSide == "Left" || BuildingSide == "Right")) // Connection to the eave purlin Only Left and Right Side
                {
                    mainMemberForColumnJoint_Top = referenceEavePurlin;
                }

                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, mainMemberForColumnJoint_Top, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
            }

            // Window Header Joint
            for (int i = 0; i < iNumberOfHeaders; i++) // Each created header
            {
                CMember current_member = m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + i];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, m_arrMembers[iMembersGirts + i], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + i + 1], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
            }

            // Window Sill Joint
            for (int i = 0; i < iNumberOfSills; i++) // Each created sill
            {
                CMember current_member = m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, m_arrMembers[iMembersGirts + i], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + i + 1], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true));
            }

            /*

            4------------3
            |            |
            |            |
            |            |
            |            |
            |            |
            1------------2

            */

            // x horizontalna os, y-zvisla os (z)

            openningPoints = new List<System.Windows.Point>(4) {
                new System.Windows.Point(prop.fWindowCoordinateXinBay, prop.fWindowCoordinateZinBay),
                new System.Windows.Point(prop.fWindowCoordinateXinBay + prop.fWindowsWidth, prop.fWindowCoordinateZinBay),
                new System.Windows.Point(prop.fWindowCoordinateXinBay + prop.fWindowsWidth, prop.fWindowCoordinateZinBay + prop.fWindowsHeight),
                new System.Windows.Point(prop.fWindowCoordinateXinBay, prop.fWindowCoordinateZinBay + prop.fWindowsHeight)
            };
        }
    }
}
