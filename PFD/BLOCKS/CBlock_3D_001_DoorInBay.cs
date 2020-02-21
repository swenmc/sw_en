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
            float fDist_FrontColumns_temp,        // Pridane kvoli pripoju trimmer to rafter - Horizontal regular distance between columns in the front wall / side
            float fDist_BackColumns_temp,         // Pridane kvoli pripoju trimmer to rafter - Horizontal regular distance between columns in the front wall / side
            float fH1_frame_Left,                 // Pridane kvoli pripoju trimmer to rafter - Height of frame column on the left side
            float fW_rame_temp,                   // Pridane kvoli pripoju trimmer to rafter - Frame Width
            float fRoofPitch_rad_temp,            // Pridane kvoli pripoju trimmer to rafter - Roof Pitch
            EModelType_FS eKitset_temp,           // Pridane kvoli pripoju trimmer to rafter - Building shape 0 - monopitch roof, 1 - gable roof
            CMember referenceGirt_temp,           // Reference girt object in bay
            CMember GirtToConnectDoorTrimmers,    // Girt to connect door trimmers on the top (not used in case that door trimmers are connected to the eave purlin or edge rafter)
            CMember ColumnLeft,                   // Left column of bay
            CMember ColumnRight,                  // Right column of bay
            CMember referenceEavePurlin,          // Reference Eave Purlin
            CMember referenceRafter,              // Pridane kvoli pripoju trimmer to rafter - Reference Edge Rafter
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
                throw new Exception("Door is defined out of frame bay."); // Door is defined out of frame bay // Bug 500

            EMemberType_FS eTypeColumn;
            EMemberType_FS eTypeLintel;

            EMemberType_FS_Position eTypePositionColumn;
            EMemberType_FS_Position eTypePositionLintel;

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

                eTypePositionColumn = EMemberType_FS_Position.DoorFrame;
                eTypePositionLintel = EMemberType_FS_Position.DoorFrame;

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
                eTypePositionColumn = EMemberType_FS_Position.DoorTrimmer;
                crscColumn = m_arrCrSc[1];

                // Lintel
                m_arrCrSc[2] = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00095f, Colors.Chocolate); // Door lintel
                m_arrCrSc[2].Name_short = "27095";
                m_arrCrSc[2].m_Mat = m_arrMat[0];
                m_arrCrSc[2].ID = (int)EMemberType_FS_Position.DoorLintel;

                eTypeLintel = EMemberType_FS.eDL;
                eTypePositionLintel = EMemberType_FS_Position.DoorLintel;
                crscLintel = m_arrCrSc[2];
            }

            INumberOfGirtsToDeactivate = (int)((prop.fDoorsHeight - fBottomGirtPosition) / fDist_Girt) + 1; // Number of intermediate girts + Bottom Girt

            bool bDoorColumnIsConnectedtoEavePurlin = false;
            bool bDoorColumnIsConnectedtoRafter = false;
            bool bDoorToCloseToLeftColumn = false; // true - generate girts only on one side, false - generate girts on both sides of door
            bool bDoorToCloseToRightColumn = false; // true - generate girts only on one side, false - generate girts on both sides of door

            if (prop.fDoorCoordinateXinBlock < fLimitDistanceFromColumn)
                bDoorToCloseToLeftColumn = true; // Door is to close to the left column

            if ((fBayWidth - (prop.fDoorCoordinateXinBlock + prop.fDoorsWidth)) < fLimitDistanceFromColumn)
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
                float fz_0 = fBottomGirtPosition + INumberOfGirtsToDeactivate * fDist_Girt;
                float fz_1 = fz_0;

                // Ak nie je vygenerovany girt pretoze je velmi blizko eave purlin (left, right) alebo rafter (front, back)
                // napajame stlpiky priamo na eave purlin alebo rafter, tj. suradcnica horneho bodu stlpika je rovna fBayHeight
                // TODO - zatial to plati len pre lavu a pravu stranu, pretoze nemam spocitanu maximalnu volnu vysku pre jednotlive bays na prednej a zadnej strane
                // Ak bude spravne urcena fBayHeight, tak (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right") zmazat
                if ((fBayHeight - fz_0 < fUpperGirtLimit) && (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right"))
                {
                    fz_0 = fz_1 = fBayHeight;
                    bDoorColumnIsConnectedtoEavePurlin = true;
                }

                if(prop.sBuildingSide == "Front" || prop.sBuildingSide == "Back")
                {
                    // TODO - cely blok refaktorovat s Window

                    float fDist_Columns;

                    if (prop.sBuildingSide == "Front")
                        fDist_Columns = fDist_FrontColumns_temp;
                    else
                        fDist_Columns = fDist_BackColumns_temp;

                    bool bConsiderAbsoluteValueOfRoofPitch = true;

                    if(eKitset == EModelType_FS.eKitsetMonoRoofEnclosed)
                        bConsiderAbsoluteValueOfRoofPitch = false;

                    // Urcime absolutne suradnice x pre laveho a praveho okraja bay
                    float fx_abs_Bay_Left = (prop.iBayNumber - 1) * fDist_Columns;
                    float fx_abs_Bay_Right = (prop.iBayNumber) * fDist_Columns;

                    // Urcime suradnice teoretickeho priesecnika medzi rafter a lavym, resp. pravym okrajom bay v ramci celej steny
                    float fz_abs_LeftBay = 0;
                    float fz_abs_RightBay = 0;

                    fW_frame = fW_rame_temp;
                    fRoofPitch_rad = fRoofPitch_rad_temp;
                    eKitset = eKitset_temp; // Nastavime tvar budovy do modelu bloku
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_Bay_Left, out fz_abs_LeftBay);
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_Bay_Right, out fz_abs_RightBay);
                    float fz_abs_Bay_Min = Math.Min(fz_abs_LeftBay, fz_abs_RightBay);
                    fBayHeight = fz_abs_Bay_Min; // Nastavime bay heigth na minimum (TODO - dalo by sa s tym hrat lebo teoreticky mozu dvere do bay vojst aj ked su mensie ako minimalna vyska v bay, zavisi to od pozicie dveri v ramci bay a od uhla sklonu)

                    if (prop.fDoorsHeight > fBayHeight)
                        throw new Exception("Door is defined out of bay height."); // Door is defined out of bay height

                    // Urcime absolutne suradnice x pre poziciu laveho a praveho stlpika dveri v ramci celej steny
                    float fx_abs_LeftColumn = (prop.iBayNumber - 1) * fDist_Columns + prop.fDoorCoordinateXinBlock;
                    float fx_abs_RightColumn = fx_abs_LeftColumn + prop.fDoorsWidth;

                    // Urcime suradnice teoretickeho priesecnika medzi rafter a column pre pozicie x laveho a praveho stlpika dveri v ramci celej steny
                    float fz_abs_LeftColumn = 0;
                    float fz_abs_RightColumn = 0;

                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_LeftColumn, out fz_abs_LeftColumn);
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_RightColumn, out fz_abs_RightColumn);

                    // TODO - tento limit by chcelo este nejako poladit, resp presne spocitat ale to sa musim poriadne zamysliet :)
                    float fUpperGirtLimit_temp = 0.5f * fUpperGirtLimit;  // Pre edge rafter nemozeme pouzit limit stanoveny pre eave purlin

                    if (fBayHeight - fz_0 < fUpperGirtLimit_temp)
                    {
                        bDoorColumnIsConnectedtoRafter = true;
                        fz_0 = fz_abs_LeftColumn;
                        fz_1 = fz_abs_RightColumn;
                    }
                }

                m_arrNodes[iNodesForGirts + i * iNumberOfColumns + 1] = new CNode(iNodesForGirts + i * iNumberOfColumns + 1 + 1, prop.fDoorCoordinateXinBlock + i * prop.fDoorsWidth, 0, i == 0 ? fz_0 : fz_1, 0);
            }

            // Coordinates of door lintel nodes
            if (iNumberOfLintels > 0)
            {
                m_arrNodes[iNodesForGirts + 2 * iNumberOfColumns] = new CNode(iNodesForGirts + 2 * iNumberOfColumns + 1, prop.fDoorCoordinateXinBlock, 0, prop.fDoorsHeight, 0);
                m_arrNodes[iNodesForGirts + 2 * iNumberOfColumns + 1] = new CNode(iNodesForGirts + 2 * iNumberOfColumns + 1 + 1, prop.fDoorCoordinateXinBlock + prop.fDoorsWidth, 0, prop.fDoorsHeight, 0);
            }

            // Block Members
            // TODO - add to block parameters

            float fGirtAlignmentStart = bIsReverseGirtSession ? ReferenceGirt.FAlignment_End : ReferenceGirt.FAlignment_Start; // Main column of a frame
            float fGirtAlignmentEnd = -0.5f * (float)crscColumn.b - fCutOffOneSide; // Door column

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
                    // Alignment - switch start and end alignment for girts on the left side of door and the right side of door
                    float fGirtStartTemp = fGirtAlignmentStart;
                    float fGirtEndTemp = fGirtAlignmentEnd;

                    eccentricityGirtStart_temp = new CMemberEccentricity(eccentricityGirtStart.MFy_local, eccentricityGirtStart.MFz_local);
                    eccentricityGirtEnd_temp = new CMemberEccentricity(eccentricityGirtEnd.MFy_local, eccentricityGirtEnd.MFz_local);

                    if (bIsReverseGirtSession) // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                    {
                        eccentricityGirtStart_temp = new CMemberEccentricity(-eccentricityGirtStart.MFy_local, -eccentricityGirtStart.MFz_local);
                        eccentricityGirtEnd_temp = new CMemberEccentricity(-eccentricityGirtEnd.MFy_local, -eccentricityGirtEnd.MFz_local);
                    }

                    if (i == 1 || bDoorToCloseToLeftColumn) // If just right sequence of girts is generated switch alignment and eccentricity (???) need testing;
                    {
                        if (!bIsLastBayInFrontorBackSide) // Change alignment (different columns on bay sides)
                        {
                            fGirtStartTemp = fGirtAlignmentEnd;
                            fGirtEndTemp = fGirtAlignmentStart;

                            if (bIsFirstBayInFrontorBackSide) // First bay, right side, end connection to the intermediate column
                                fGirtEndTemp = ReferenceGirt.FAlignment_End;
                        }
                        else // Last bay - right side - end alignment to the main column
                        {
                            fGirtEndTemp = ReferenceGirt.FAlignment_Start;

                            if (!bIsReverseGirtSession && bIsLastBayInFrontorBackSide) // Posledna bay, ale nie reverse session, girt ma odsadenie podla frame edge column
                            {
                                fGirtStartTemp = fGirtAlignmentEnd;
                                fGirtEndTemp = ReferenceGirt.FAlignment_End;
                            }

                            // Ak je girt v obratenom poradi a len na pravej strane v ramci bay, tak zaciatocne alignment ma byt nastavene podla edge column a koncove podla door trimmer
                            if (bIsReverseGirtSession) // Obratene poradie girts (Gable roof predna a zadna strana napravo)
                                fGirtStartTemp = fGirtAlignmentEnd;
                        }

                        eccentricityGirtStart_temp = new CMemberEccentricity(eccentricityGirtEnd.MFy_local, eccentricityGirtEnd.MFz_local); // TODO - we need probably to change signs of values
                        eccentricityGirtEnd_temp = new CMemberEccentricity(eccentricityGirtStart.MFy_local, eccentricityGirtStart.MFz_local); // TODO - we need probably to change signs of values

                        if (bIsReverseGirtSession) // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                        {
                            eccentricityGirtStart_temp = new CMemberEccentricity(-eccentricityGirtEnd.MFy_local, -eccentricityGirtEnd.MFz_local);
                            eccentricityGirtEnd_temp = new CMemberEccentricity(-eccentricityGirtStart.MFy_local, -eccentricityGirtStart.MFz_local);
                        }
                    }

                    m_arrMembers[i * INumberOfGirtsToDeactivate + j] = new CMember(i * INumberOfGirtsToDeactivate + j + 1, m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2], m_arrNodes[i * iNumberOfNodesOnOneSide + j * 2 + 1], m_arrCrSc[0], ReferenceGirt.EMemberType, ReferenceGirt.EMemberTypePosition, eccentricityGirtStart_temp, eccentricityGirtEnd_temp, fGirtStartTemp, fGirtEndTemp, fGirtsRotation, 0);

                    // Set position type (same as reference girt)
                    m_arrMembers[i * INumberOfGirtsToDeactivate + j].EMemberTypePosition = ReferenceGirt.EMemberTypePosition;
                }
            }

            INumberOfGirtsGeneratedInBlock = iNumberOfGirtsSequences * INumberOfGirtsToDeactivate;

            // TODO - add to block parameters
            float fDoorColumnStart = 0.0f;
            float fDoorColumnEnd = (float)ReferenceGirt.CrScStart.y_min - fCutOffOneSide;

            if (bDoorColumnIsConnectedtoEavePurlin)
            {
                float referenceEavePurlinEcc_Local_z = -referenceEavePurlin.EccentricityStart.MFz_local; // Left side (+z downward)

                if(BuildingSide == "Right")
                    referenceEavePurlinEcc_Local_z = referenceEavePurlin.EccentricityStart.MFz_local; // Right side (+z upward)

                fDoorColumnEnd = referenceEavePurlinEcc_Local_z + (float)referenceEavePurlin.CrScStart.z_min - fCutOffOneSide;
            }

            if(bDoorColumnIsConnectedtoRafter)
            {
                float referenceEdgeRafter_alignment_temp = (float)referenceRafter.CrScStart.z_min / (float)Math.Cos(Math.Abs(fRoofPitch_rad)) + (float)crscColumn.y_min * (float)Math.Tan(Math.Abs(fRoofPitch_rad));
                fDoorColumnEnd = referenceEdgeRafter_alignment_temp - fCutOffOneSide;
            }

            float fOffsetBetweenGirtAndColumn_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_max - (float)crscColumn.z_max;
            float feccentricityDoorColumnStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
            float feccentricityDoorColumnEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;

            float fOffsetBetweenGirtAndLintel_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_max - (float)crscLintel.z_max;
            float feccentricityDoorLintelStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndLintel_LCS_z_axis;
            float feccentricityDoorLintelEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndLintel_LCS_z_axis;

            if (BuildingSide == "Left" || BuildingSide == "Back") // Align the to bottom edge of cross-section
            {
                fOffsetBetweenGirtAndColumn_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_min - (float)crscColumn.z_min;
                feccentricityDoorColumnStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
                feccentricityDoorColumnEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;

                fOffsetBetweenGirtAndLintel_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_min - (float)crscLintel.z_min;
                feccentricityDoorLintelStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndLintel_LCS_z_axis;
                feccentricityDoorLintelEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndLintel_LCS_z_axis;
            }

            // Door columns / trimmers
            // Set eccentricity sign of columns in front / back side depending on girt LCS (reverse session)
            if ((BuildingSide == "Front" || BuildingSide == "Back") && bIsReverseGirtSession)
            {
                // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                feccentricityDoorColumnStart_LCS_z = -eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
                feccentricityDoorColumnEnd_LCS_z = -eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
            }

            CMemberEccentricity feccentricityDoorColumnStart = new CMemberEccentricity(0f, feccentricityDoorColumnStart_LCS_z);
            CMemberEccentricity feccentricityDoorColumnEnd = new CMemberEccentricity(0f, feccentricityDoorColumnEnd_LCS_z);

            float fDoorColumnRotation = (float)Math.PI / 2;

            // Rotate local axis about x
            if (BuildingSide == "Left" || BuildingSide == "Right")
            {
                fDoorColumnRotation += (float)Math.PI / 2;
            }

            m_arrMembers[iMembersGirts] = new CMember(iMembersGirts + 1, m_arrNodes[iNodesForGirts], m_arrNodes[iNodesForGirts + 1], crscColumn, eTypeColumn, eTypePositionColumn, feccentricityDoorColumnStart, feccentricityDoorColumnEnd, fDoorColumnStart, fDoorColumnEnd, fDoorColumnRotation, 0);
            m_arrMembers[iMembersGirts + 1] = new CMember(iMembersGirts + 1 + 1, m_arrNodes[iNodesForGirts + 2], m_arrNodes[iNodesForGirts + 2 + 1], crscColumn, eTypeColumn, eTypePositionColumn, feccentricityDoorColumnStart, feccentricityDoorColumnEnd, fDoorColumnStart, fDoorColumnEnd, fDoorColumnRotation, 0);

            // Set position type
            //m_arrMembers[iMembersGirts].EMemberTypePosition = eTypePositionColumn;
            //m_arrMembers[iMembersGirts + 1].EMemberTypePosition = eTypePositionColumn;

            // Door lintel (header)
            // TODO - add to block parameters
            float fDoorLintelStart = -0.5f * (float)crscColumn.b - fCutOffOneSide;
            float fDoorLintelEnd = -0.5f * (float)crscColumn.b - fCutOffOneSide;

            // Set eccentricity sign of lintel in front / back side depending on girt LCS (reverse session)
            if ((BuildingSide == "Front" || BuildingSide == "Back") && bIsReverseGirtSession)
            {
                // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                feccentricityDoorLintelStart_LCS_z = -eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndLintel_LCS_z_axis;
                feccentricityDoorLintelEnd_LCS_z = -eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndLintel_LCS_z_axis;
            }

            CMemberEccentricity feccentricityDoorLintelStart = new CMemberEccentricity(0, feccentricityDoorLintelStart_LCS_z);
            CMemberEccentricity feccentricityDoorLintelEnd = new CMemberEccentricity(0, feccentricityDoorLintelEnd_LCS_z);
            float fDoorLintelRotation = (float)Math.PI / 2;

            if (iNumberOfLintels > 0)
            {
                m_arrMembers[iMembersGirts + iNumberOfColumns] = new CMember(iMembersGirts + iNumberOfColumns + 1, m_arrNodes[iNodesForGirts + iNumberOfColumns * 2], m_arrNodes[iNodesForGirts + iNumberOfColumns * 2 + 1], crscLintel, eTypeLintel, eTypePositionLintel, feccentricityDoorLintelStart, feccentricityDoorLintelEnd, fDoorLintelStart, fDoorLintelEnd, fDoorLintelRotation, 0);

                // Set position type
                //m_arrMembers[iMembersGirts + iNumberOfColumns].EMemberTypePosition = eTypePositionLintel;
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

                    if (i == 1 || bDoorToCloseToLeftColumn) // If just right sequence of girts is generated
                    {
                        currentColumnToConnectStart = m_arrMembers[iMembersGirts + 1]; // Door Column
                        currentColumnToConnectEnd = ColumnRight;
                    }

                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, currentColumnToConnectStart, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, currentColumnToConnectEnd, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                }
            }

            // Column Joints
            for (int i = 0; i < iNumberOfColumns; i++) // Each created column
            {
                CMember current_member = m_arrMembers[iMembersGirts + i];
                // TODO - dopracovat moznosti kedy je stlpik dveri pripojeny k eave purlin, main rafter a podobne (nemusi to byt vzdy girt)

                // Bottom - columns is connected to the concrete foundation (use different type of plate ???)

                // TODO - spoj nemoze mat MainMember = null, je potrebne pre alternativu podobnu ako je T001 zapracovat aj tuto moznost, takze stlp ramu bude main member a plechy atd budu analogicke ako su spoje prvkov ramu T001
                //m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, null, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true));

                if (prop.sDoorType == "Personnel Door")
                    m_arrConnectionJoints.Add(new CConnectionJoint_TD01(current_member.NodeStart, current_member)); // Personnel door frame base joint
                else
                    m_arrConnectionJoints.Add(new CConnectionJoint_TC01(current_member.NodeStart, current_member)); // Roller door trimmer base joint

                // Top
                CMember mainMemberForColumnJoint = GirtToConnectDoorTrimmers;

                if (bDoorColumnIsConnectedtoEavePurlin && (BuildingSide == "Left" || BuildingSide == "Right")) // Connection to the eave purlin Only Left and Right Side
                {
                    mainMemberForColumnJoint = referenceEavePurlin;
                }

                if (bDoorColumnIsConnectedtoRafter && (BuildingSide == "Front" || BuildingSide == "Back")) // Connection to the edge rafter Only Front and Back Side
                {
                    mainMemberForColumnJoint = referenceRafter;
                }

                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, mainMemberForColumnJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
            }

            // Lintel (header) Joint
            if (iNumberOfLintels > 0)
            {
                CMember current_member = m_arrMembers[iMembersGirts + iNumberOfColumns];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeStart, m_arrMembers[iMembersGirts], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001("LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + 1], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
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
                new System.Windows.Point(prop.fDoorCoordinateXinBlock, 0),
                new System.Windows.Point(prop.fDoorCoordinateXinBlock + prop.fDoorsWidth, 0),
                new System.Windows.Point(prop.fDoorCoordinateXinBlock + prop.fDoorsWidth, 0 + prop.fDoorsHeight),
                new System.Windows.Point(prop.fDoorCoordinateXinBlock, 0 + prop.fDoorsHeight)
            };
        }
    }
}