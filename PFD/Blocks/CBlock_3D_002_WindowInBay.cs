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
            float fLimitDistanceFromColumn,            // Limit value to generate girts on the left or right side of opening
            float fBottomGirtPosition,                 // Vertical position of first girt
            float fDist_Girt,                          // Vertical regular distance between girts
            float fDist_FrontColumns_temp,             // Pridane kvoli pripoju trimmer to rafter - Horizontal regular distance between columns in the front wall / side
            float fDist_BackColumns_temp,              // Pridane kvoli pripoju trimmer to rafter - Horizontal regular distance between columns in the front wall / side
            float fH1_frame_Left,                      // Pridane kvoli pripoju trimmer to rafter - Height of frame column on the left side
            float fW_frame_temp,                       // Pridane kvoli pripoju trimmer to rafter - Frame Width
            float fRoofPitch_rad_temp,                 // Pridane kvoli pripoju trimmer to rafter - Roof Pitch
            EModelType_FS eKitset_temp,                // Pridane kvoli pripoju trimmer to rafter - Building shape 0 - monopitch roof, 1 - gable roof
            CMember referenceGirt_temp,                // Reference girt object in bay
            CMember GirtToConnectWindowColumns_Bottom, // Bottom window column to girt connection
            CMember GirtToConnectWindowColumns_Top,    // Top window column to girt connection (just in case that it is not connected to the eave purlin or edge rafter)
            CMember ColumnLeft,                        // Left column of bay
            CMember ColumnRight,                       // Right column of bay
            CMember referenceEavePurlin,               // Reference Eave Purlin
            CMember referenceRafter,                   // Pridane kvoli pripoju trimmer to rafter - Reference Edge Rafter
            float fBayWidth,
            float fBayHeight,
            float fUpperGirtLimit,                     // Vertical limit to generate last girt (cant' be too close or in colision with eave purlin or rafter)
            bool bIsReverseGirtSession = false,        // Front or back wall bay can have reverse direction of girts in X
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
                throw new Exception("Window is defined out of frame height."); // Window is defined out of frame height

            float fDistanceBetweenWindowColumns = prop.fWindowsWidth / (prop.iNumberOfWindowColumns - 1);

            m_arrMat = new System.Collections.Generic.Dictionary<EMemberGroupNames, CMat>();
            m_arrCrSc = new Dictionary<EMemberGroupNames, CCrSc>();// CCrSc[2];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00(0, "G550‡", 200e+9f, 0.3f);

            // Cross-sections
            // TODO - add to cross-section parameters

            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = ReferenceGirt.CrScStart; // Girts
            m_arrCrSc[(EMemberGroupNames)1] = new CCrSc_3_10075_BOX(0, 0.1f, 0.1f, 0.00075f, Colors.Red); // Window frame
            m_arrCrSc[(EMemberGroupNames)1].Name_short = "10075";
            m_arrCrSc[(EMemberGroupNames)1].m_Mat = m_arrMat[0];
            m_arrCrSc[(EMemberGroupNames)1].ID = (int)EMemberType_FS_Position.WindowFrame;

            iNumberOfGirtsUnderWindow = (int)((prop.fWindowCoordinateZinBay - fBottomGirtPosition) / fDist_Girt) + 1;
            float fCoordinateZOfGirtUnderWindow = (iNumberOfGirtsUnderWindow - 1) * fDist_Girt + fBottomGirtPosition;

            if (prop.fWindowCoordinateZinBay <= fBottomGirtPosition)
            {
                iNumberOfGirtsUnderWindow = 0;
                fCoordinateZOfGirtUnderWindow = 0f;
            }

            INumberOfGirtsToDeactivate = (int)((prop.fWindowsHeight + prop.fWindowCoordinateZinBay - fCoordinateZOfGirtUnderWindow) / fDist_Girt); // Number of intermediate girts to deactivate

            bool bWindowColumnIsConnectedtoEavePurlin = false;
            bool bWindowColumnIsConnectedtoRafter = false;
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
                float fz_0 = fBottomGirtPosition + iNumberOfGirtsUnderWindow * fDist_Girt + INumberOfGirtsToDeactivate * fDist_Girt;
                float fz_1 = fz_0;

                // Ak nie je vygenerovany girt pretoze je velmi blizko eave purlin (left, right) alebo rafter (front, back)
                // napajame stlpiky priamo na eave purlin alebo rafter, tj. suradcnica horneho bodu stlpika je rovna fBayHeight
                // TODO - zatial to plati len pre lavu a pravu stranu, pretoze nemam spocitanu maximalnu volnu vysku pre jednotlive bays na prednej a zadnej strane
                // Ak bude spravne urcena fBayHeight, tak (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right") zmazat
                if ((fBayHeight - fz_0 < fUpperGirtLimit) && (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right"))
                {
                    fz_0 = fz_1 = fBayHeight;
                    bWindowColumnIsConnectedtoEavePurlin = true;
                }

                if (prop.sBuildingSide == "Front" || prop.sBuildingSide == "Back")
                {
                    // TODO - cely blok refaktorovat s Door

                    float fDist_Columns;

                    if (prop.sBuildingSide == "Front")
                        fDist_Columns = fDist_FrontColumns_temp;
                    else
                        fDist_Columns = fDist_BackColumns_temp;

                    bool bConsiderAbsoluteValueOfRoofPitch = true;

                    if (eKitset == EModelType_FS.eKitsetMonoRoofEnclosed)
                        bConsiderAbsoluteValueOfRoofPitch = false;

                    // Urcime absolutne suradnice x pre laveho a praveho okraja bay
                    float fx_abs_Bay_Left = (prop.iBayNumber - 1) * fDist_Columns;
                    float fx_abs_Bay_Right = (prop.iBayNumber) * fDist_Columns;

                    // Urcime suradnice teoretickeho priesecnika medzi rafter a lavym, resp. pravym okrajom bay v ramci celej steny
                    float fz_abs_LeftBay = 0;
                    float fz_abs_RightBay = 0;

                    fW_frame_centerline = fW_frame_temp;
                    fRoofPitch_rad = fRoofPitch_rad_temp;
                    eKitset = eKitset_temp; // Nastavime tvar budovy do modelu bloku
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_Bay_Left, out fz_abs_LeftBay);
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_Bay_Right, out fz_abs_RightBay);
                    float fz_abs_Bay_Min = Math.Min(fz_abs_LeftBay, fz_abs_RightBay);
                    fBayHeight = fz_abs_Bay_Min; // Nastavime bay heigth na minimum (TODO - dalo by sa s tym hrat lebo teoreticky mozu dvere do bay vojst aj ked su mensie ako minimalna vyska v bay, zavisi to od pozicie dveri v ramci bay a od uhla sklonu)

                    if ((prop.fWindowsHeight + prop.fWindowCoordinateZinBay) > fBayHeight)
                        throw new Exception("Window is defined out of bay height."); // Window is defined out of bay height

                    // Urcime absolutne suradnice x pre poziciu laveho a praveho stlpika dveri v ramci celej steny
                    float fx_abs_LeftColumn = (prop.iBayNumber - 1) * fDist_Columns + prop.fWindowCoordinateXinBay;
                    float fx_abs_RightColumn = fx_abs_LeftColumn + prop.fWindowsWidth;

                    // Urcime suradnice teoretickeho priesecnika medzi rafter a column pre pozicie x laveho a praveho stlpika dveri v ramci celej steny
                    float fz_abs_LeftColumn = 0;
                    float fz_abs_RightColumn = 0;

                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_LeftColumn, out fz_abs_LeftColumn);
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fH1_frame_Left, fx_abs_RightColumn, out fz_abs_RightColumn);

                    // TODO - tento limit by chcelo este nejako poladit, resp presne spocitat ale to sa musim poriadne zamysliet :)
                    float fUpperGirtLimit_temp = 0.5f * fUpperGirtLimit;  // Pre edge rafter nemozeme pouzit limit stanoveny pre eave purlin

                    if (fBayHeight - fz_0 < fUpperGirtLimit_temp)
                    {
                        bWindowColumnIsConnectedtoRafter = true;
                        fz_0 = fz_abs_LeftColumn;
                        fz_1 = fz_abs_RightColumn;
                    }
                }

                m_arrNodes[iNodesForGirts + i * 2 + 1] = new CNode(iNodesForGirts + i * 2 + 1 + 1, prop.fWindowCoordinateXinBay + i * fDistanceBetweenWindowColumns, 0, i == 0 ? fz_0 : fz_1, 0);
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

            float fGirtAlignmentStart = bIsReverseGirtSession ? ReferenceGirt.FAlignment_End : ReferenceGirt.FAlignment_Start; // Main column of a frame
            float fGirtAlignmentEnd = -0.5f * (float)m_arrCrSc[(EMemberGroupNames)1].b - fCutOffOneSide; // Window column
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
                    // Alignment - switch start and end alignment for girts on the left side of window and the right side of window
                    float fGirtStartTemp = fGirtAlignmentStart;
                    float fGirtEndTemp = fGirtAlignmentEnd;

                    eccentricityGirtStart_temp = new CMemberEccentricity(eccentricityGirtStart.MFy_local, eccentricityGirtStart.MFz_local);
                    eccentricityGirtEnd_temp = new CMemberEccentricity(eccentricityGirtEnd.MFy_local, eccentricityGirtEnd.MFz_local);

                    if (bIsReverseGirtSession) // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                    {
                        eccentricityGirtStart_temp = new CMemberEccentricity(-eccentricityGirtStart.MFy_local, -eccentricityGirtStart.MFz_local);
                        eccentricityGirtEnd_temp = new CMemberEccentricity(-eccentricityGirtEnd.MFy_local, -eccentricityGirtEnd.MFz_local);
                    }

                    if (i == 1 || bWindowToCloseToLeftColumn) // If just right sequence of girts is generated switch alignment and eccentricity (???) need testing;
                    {
                        if (!bIsLastBayInFrontorBackSide) // Change alignment (different columns on bay sides)
                        {
                            fGirtStartTemp = fGirtAlignmentEnd;
                            fGirtEndTemp = fGirtAlignmentStart;

                            if (prop.sBuildingSide == "Left" || prop.sBuildingSide == "Right")
                                fGirtEndTemp = ReferenceGirt.FAlignment_End; // Ak su girt len na pravej strane od okna, nastavime koncove odsadenie podla povodneho koncoveho odsadenia referencneho girt

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

                            // Ak je girt v obratenom poradi a len na pravej strane v ramci bay, tak zaciatocne alignment ma byt nastavene podla edge column a koncove podla window column
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
                }
            }

            INumberOfGirtsGeneratedInBlock = iNumberOfGirtsSequences * INumberOfGirtsToDeactivate;

            float fWindowColumnStart = 0.0f;

            if(fBottomGirtPosition >= fCoordinateZOfGirtUnderWindow) // Window column is connected to the girt
                fWindowColumnStart = -(float)ReferenceGirt.CrScStart.y_max - fCutOffOneSide;

            float fWindowColumnEnd = (float)ReferenceGirt.CrScStart.y_min - fCutOffOneSide;

            if (bWindowColumnIsConnectedtoEavePurlin)
            {
                float referenceEavePurlinEcc_Local_z = -referenceEavePurlin.EccentricityStart.MFz_local; // Left side (+z downward)

                if (BuildingSide == "Right")
                    referenceEavePurlinEcc_Local_z = referenceEavePurlin.EccentricityStart.MFz_local; // Right side (+z upward)

                fWindowColumnEnd = referenceEavePurlinEcc_Local_z + (float)referenceEavePurlin.CrScStart.z_min - fCutOffOneSide;
            }

            if (bWindowColumnIsConnectedtoRafter)
            {
                float referenceEdgeRafter_alignment_temp = (float)referenceRafter.CrScStart.z_min / (float)Math.Cos(Math.Abs(fRoofPitch_rad)) + (float)m_arrCrSc[(EMemberGroupNames)1].y_min * (float)Math.Tan(Math.Abs(fRoofPitch_rad));
                fWindowColumnEnd = referenceEdgeRafter_alignment_temp - fCutOffOneSide;
            }

            float fOffsetBetweenGirtAndColumn_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_max - (float)m_arrCrSc[(EMemberGroupNames)1].z_max;
            float feccentricityWindowColumnStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
            float feccentricityWindowColumnEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;

            float fOffsetBetweenGirtAndHeader_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_max - (float)m_arrCrSc[(EMemberGroupNames)1].z_max;
            float feccentricityWindowHeaderStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndHeader_LCS_z_axis;
            float feccentricityWindowHeaderEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndHeader_LCS_z_axis;

            float fOffsetBetweenGirtAndSill_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_max - (float)m_arrCrSc[(EMemberGroupNames)1].z_max;
            float feccentricityWindowSillStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndSill_LCS_z_axis;
            float feccentricityWindowSillEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndSill_LCS_z_axis;

            if (BuildingSide == "Left" || BuildingSide == "Back") // Align the to bottom edge of cross-section
            {
                fOffsetBetweenGirtAndColumn_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_min - (float)m_arrCrSc[(EMemberGroupNames)1].z_min;
                feccentricityWindowColumnStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
                feccentricityWindowColumnEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;

                fOffsetBetweenGirtAndHeader_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_min - (float)m_arrCrSc[(EMemberGroupNames)1].z_min;
                feccentricityWindowHeaderStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndHeader_LCS_z_axis;
                feccentricityWindowHeaderEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndHeader_LCS_z_axis;

                fOffsetBetweenGirtAndSill_LCS_z_axis = (float)ReferenceGirt.CrScStart.z_min - (float)m_arrCrSc[(EMemberGroupNames)1].z_min;
                feccentricityWindowSillStart_LCS_z = eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndSill_LCS_z_axis;
                feccentricityWindowSillEnd_LCS_z = eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndSill_LCS_z_axis;
            }

            // Window columns
            // Set eccentricity sign of columns in front / back side depending on girt LCS (reverse session)
            if ((BuildingSide == "Front" || BuildingSide == "Back") && bIsReverseGirtSession)
            {
                // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                feccentricityWindowColumnStart_LCS_z = -eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
                feccentricityWindowColumnEnd_LCS_z = -eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndColumn_LCS_z_axis;
            }

            CMemberEccentricity feccentricityWindowColumnStart = new CMemberEccentricity(0f, feccentricityWindowColumnStart_LCS_z);
            CMemberEccentricity feccentricityWindowColumnEnd = new CMemberEccentricity(0f, feccentricityWindowColumnEnd_LCS_z);

            float fWindowColumnRotation = (float)Math.PI / 2;

            // Rotate local axis about x
            if (BuildingSide == "Left" || BuildingSide == "Right")
            {
                fWindowColumnRotation += (float)Math.PI / 2;
            }

            // Window columns
            for (int i = 0; i < prop.iNumberOfWindowColumns; i++)
            {
                m_arrMembers[iMembersGirts + i] = new CMember(iMembersGirts + i + 1, m_arrNodes[iNodesForGirts + i * 2], m_arrNodes[iNodesForGirts + i * 2 + 1], m_arrCrSc[(EMemberGroupNames)1], EMemberType_FS.eWF, EMemberType_FS_Position.WindowFrame, feccentricityWindowColumnStart, feccentricityWindowColumnEnd, fWindowColumnStart, fWindowColumnEnd, fWindowColumnRotation, 0);
            }

            // Window (header)
            // TODO - add to block parameters
            float fWindowHeaderStart = -0.5f * (float)m_arrCrSc[(EMemberGroupNames)1].h - fCutOffOneSide;
            float fWindowHeaderEnd = -0.5f * (float)m_arrCrSc[(EMemberGroupNames)1].h - fCutOffOneSide;
            float fWindowHeaderRotation = (float)Math.PI / 2;

            // Set eccentricity sign of header in front / back side depending on girt LCS (reverse session)
            if ((BuildingSide == "Front" || BuildingSide == "Back") && bIsReverseGirtSession)
            {
                // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                feccentricityWindowHeaderStart_LCS_z = -eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndHeader_LCS_z_axis;
                feccentricityWindowHeaderEnd_LCS_z = -eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndHeader_LCS_z_axis;
            }

            CMemberEccentricity feccentricityWindowHeaderStart = new CMemberEccentricity(0, feccentricityWindowHeaderStart_LCS_z);
            CMemberEccentricity feccentricityWindowHeaderEnd = new CMemberEccentricity(0, feccentricityWindowHeaderEnd_LCS_z);

            for (int i = 0; i < iNumberOfHeaders; i++)
            {
                m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + i] = new CMember(iMembersGirts + prop.iNumberOfWindowColumns + i + 1, m_arrNodes[iNodesForGirts + iNodesForWindowColumns + i], m_arrNodes[iNodesForGirts + iNodesForWindowColumns + i + 1], m_arrCrSc[(EMemberGroupNames)1], EMemberType_FS.eWF, EMemberType_FS_Position.WindowFrame, feccentricityWindowHeaderStart, feccentricityWindowHeaderEnd, fWindowHeaderStart, fWindowHeaderEnd, fWindowHeaderRotation, 0);
            }

            // Window (Sills)
            // TODO - add to block parameters
            float fWindowSillStart = -0.5f * (float)m_arrCrSc[(EMemberGroupNames)1].h - fCutOffOneSide;
            float fWindowSillEnd = -0.5f * (float)m_arrCrSc[(EMemberGroupNames)1].h - fCutOffOneSide;
            float fWindowSillRotation = (float)Math.PI / 2;

            // Set eccentricity sign of sill in front / back side depending on girt LCS (reverse session)
            if ((BuildingSide == "Front" || BuildingSide == "Back") && bIsReverseGirtSession)
            {
                // Zmenime znamienko pre excentricitu, lebo girts v bloku maju inu orientaciu osi ako girts v session
                feccentricityWindowSillStart_LCS_z = -eccentricityGirtStart.MFz_local + fOffsetBetweenGirtAndSill_LCS_z_axis;
                feccentricityWindowSillEnd_LCS_z = -eccentricityGirtEnd.MFz_local + fOffsetBetweenGirtAndSill_LCS_z_axis;
            }

            CMemberEccentricity feccentricityWindowSillStart = new CMemberEccentricity(0, feccentricityWindowSillStart_LCS_z);
            CMemberEccentricity feccentricityWindowSillEnd = new CMemberEccentricity(0, feccentricityWindowSillEnd_LCS_z);

            for (int i = 0; i < iNumberOfSills; i++)
            {
                m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i] = new CMember(iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i + 1, m_arrNodes[iNodesForGirts + iNodesForWindowColumns + iNodesForWindowHeaders + i], m_arrNodes[iNodesForGirts + iNodesForWindowColumns + iNodesForWindowHeaders + i + 1], m_arrCrSc[(EMemberGroupNames)1], EMemberType_FS.eWF, EMemberType_FS_Position.WindowFrame, feccentricityWindowSillStart, feccentricityWindowSillEnd, fWindowSillStart, fWindowSillEnd, fWindowSillRotation, 0);
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

                    m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eGirt_EdgeColumn, "LH", current_member.NodeStart, currentColumnToConnectStart, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eGirt_DoorTrimmer, "LH", current_member.NodeEnd, currentColumnToConnectEnd, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                }
            }

            // Column Joints
            for (int i = 0; i < prop.iNumberOfWindowColumns; i++) // Each created column
            {
                CMember current_member = m_arrMembers[iMembersGirts + i];
                // TODO - dopracovat moznosti kedy je stlpik okna pripojeny k eave purlin, main rafter a podobne (nemusi to byt vzdy girt)

                // Bottom - columns is connected to the concrete foundation or girt (use different type of plate ???)
                CMember mainMemberForColumnJoint_Bottom = GirtToConnectWindowColumns_Bottom;
                m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eWindowFrame_Girt, "LJ", current_member.NodeStart, mainMemberForColumnJoint_Bottom, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));

                // Top
                CMember mainMemberForColumnJoint_Top = GirtToConnectWindowColumns_Top;

                if (bWindowColumnIsConnectedtoEavePurlin && (BuildingSide == "Left" || BuildingSide == "Right")) // Connection to the eave purlin Only Left and Right Side
                {
                    mainMemberForColumnJoint_Top = referenceEavePurlin;
                }

                if (bWindowColumnIsConnectedtoRafter && (BuildingSide == "Front" || BuildingSide == "Back")) // Connection to the edge rafter Only Front and Back Side
                {
                    mainMemberForColumnJoint_Top = referenceRafter;
                }

                m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eWindowFrame_Girt, "LJ", current_member.NodeEnd, mainMemberForColumnJoint_Top, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
            }

            // Window Header Joint
            for (int i = 0; i < iNumberOfHeaders; i++) // Each created header
            {
                CMember current_member = m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + i];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eWindowFrame_Header_Sill_WindowFrameColumn, "LJ", current_member.NodeStart, m_arrMembers[iMembersGirts + i], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eWindowFrame_Header_Sill_WindowFrameColumn, "LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + i + 1], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
            }

            // Window Sill Joint
            for (int i = 0; i < iNumberOfSills; i++) // Each created sill
            {
                CMember current_member = m_arrMembers[iMembersGirts + prop.iNumberOfWindowColumns + iNumberOfHeaders + i];
                m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eWindowFrame_Header_Sill_WindowFrameColumn, "LJ", current_member.NodeStart, m_arrMembers[iMembersGirts + i], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                m_arrConnectionJoints.Add(new CConnectionJoint_T001(EJointType.eWindowFrame_Header_Sill_WindowFrameColumn, "LJ", current_member.NodeEnd, m_arrMembers[iMembersGirts + i + 1], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
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
