using BaseClasses;
using CRSC;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using M_EC1.AS_NZS;
using MATERIAL;
using MATH;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Linq;
using System.Collections.ObjectModel;

namespace PFD
{
    [Serializable]
    public class CModel_PFD : CExample
    {
        public float fRoofPitch_rad;
        public float fSlopeFactor; // Snow load
        public int iFrameNo;
        public int iFrameNodesNo;
        public int iFrameMembersNo;

        public float fUpperGirtLimit;
        public float fDist_FrontGirts;
        public float fDist_BackGirts;
        public float fz_UpperLimitForFrontGirts;
        public float fz_UpperLimitForBackGirts;

        public int iMainColumnNo;
        public int iRafterNo;
        public int iEavesPurlinNo;
        public int iEavesPurlinNoInOneFrame;
        public int iPurlinNoInOneFrame;
        public int iGirtNoInOneFrame;
        public int iFrontColumnNoInOneFrame;
        public int iBackColumnNoInOneFrame;
        public int iFrontGirtsNoInOneFrame;
        public int iBackGirtsNoInOneFrame;
        public int iGBSideWallsMembersNo = 0;
        public int iPBMembersNo = 0;
        public int iNumberOfGB_FSMembersInOneFrame = 0;
        public int iNumberOfGB_BSMembersInOneFrame = 0;

        protected int[] iArrNumberOfNodesPerFrontColumnFromLeft;
        protected int[] iArrNumberOfNodesPerBackColumnFromLeft;

        protected int[] iArrNumberOfGirtsPerFrontColumnFromLeft;
        protected int[] iArrNumberOfGirtsPerBackColumnFromLeft;

        protected float fFrontFrameRakeAngle_temp_rad;
        protected float fBackFrameRakeAngle_temp_rad;
        protected float fFrontFrameRakeAngle_deg;
        protected float fBackFrameRakeAngle_deg;
        protected int iMainColumnFlyBracing_EveryXXGirt;
        protected int iOneRafterBackColumnNo = 0;
        protected int iOneRafterFrontColumnNo = 0;
        protected int iRafterFlyBracing_EveryXXPurlin;

        protected CMemberEccentricity eccentricityColumnFront_Z;
        protected CMemberEccentricity eccentricityColumnBack_Z;

        protected CMemberEccentricity eccentricityGirtFront_Y0;
        protected CMemberEccentricity eccentricityGirtBack_YL;

        protected ObservableCollection<DoorProperties> DoorBlocksProperties; // Pridane kvoli Rebates

        protected List<CNode> listOfSupportedNodes_S1;
        protected List<CNode> listOfSupportedNodes_S2;

        public List<CEntity3D> componentList;
        public List<CBlock_3D_001_DoorInBay> DoorsModels;
        public List<CBlock_3D_002_WindowInBay> WindowsModels;

        public virtual void CalculateLoadValuesAndGenerateLoads(
                CCalcul_1170_1 generalLoad,
                CCalcul_1170_2 wind,
                CCalcul_1170_3 snow,
                CCalcul_1170_5 eq,
                bool bGenerateNodalLoads,
                bool bGenerateLoadsOnGirts,
                bool bGenerateLoadsOnPurlins,
                bool bGenerateLoadsOnColumns,
                bool bGenerateLoadsOnFrameMembers,
                bool bGenerateSurfaceLoads) { }









        protected void CreateJoints(bool bGenerateGirts, bool bUseMainColumnFlyBracingPlates, bool bGeneratePurlins, bool bUseRafterFlyBracingPlates,
        bool bGenerateFrontColumns, bool bGenerateBackColumns, bool bGenerateFrontGirts, bool bGenerateBackGirts,
        bool bGenerateGirtBracingSideWalls, bool bGeneratePurlinBracing, bool bGenerateGirtBracingFrontSide, bool bGenerateGirtBracingBackSide, bool bWindPostUnderRafter, int iOneColumnGirtNo)
        {
            bool bIsGableRoof = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

            // Connection Joints
            m_arrConnectionJoints = new List<CConnectionJointTypes>();

            // Frame Main Column Joints to Foundation
            for (int i = 0; i < iFrameNo; i++)
            {
                m_arrConnectionJoints.Add(new CConnectionJoint_TA01(m_arrNodes[i * iFrameNodesNo + 0], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0]));
                m_arrConnectionJoints.Add(new CConnectionJoint_TA01(m_arrNodes[i * iFrameNodesNo + (iFrameNodesNo - 1)], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + (iFrameMembersNo - 1)]));
            }

            float ft_rafter_joint_plate = 0.003f; // m
            // Frame Rafter Joints
            if (bIsGableRoof)
            {
                for (int i = 0; i < iFrameNo; i++)
                {
                    float fRafterDepth = (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1].CrScStart.h;
                    float fa = fRafterDepth * (float)Math.Tan(fRoofPitch_rad);
                    float fRafterPart = fRafterDepth + fa;
                    float fbPlateHalf = fRafterPart * (float)Math.Cos(fRoofPitch_rad);
                    float fPlateWidth = 2 * fbPlateHalf;

                    if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                        m_arrConnectionJoints.Add(new CConnectionJoint_A001(m_arrNodes[i * iFrameNodesNo + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 2], fRoofPitch_rad, fPlateWidth, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg));
                    else //if(i< (iFrameNo - 1) // Intermediate frame
                        m_arrConnectionJoints.Add(new CConnectionJoint_A001(m_arrNodes[i * iFrameNodesNo + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 2], fRoofPitch_rad, fPlateWidth, ft_rafter_joint_plate, 0));
                }
            }

            float ft_knee_joint_plate = 0.003f; // m

            // Knee Joints 1
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * iFrameNodesNo + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * iFrameNodesNo + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg));
            }

            // Knee Joints 2
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * iFrameNodesNo + (iFrameNodesNo - 1 - 1)], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1)], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1 - 1)], bIsGableRoof ? fRoofPitch_rad : -fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1)].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1 - 1)].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * iFrameNodesNo + (iFrameNodesNo - 1 - 1)], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1)], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1 - 1)], bIsGableRoof ? fRoofPitch_rad : -fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1)].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + (iFrameMembersNo - 1 - 1)].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg));
            }

            // Eaves Purlin Joints
            if (iEavesPurlinNo > 0)
            {
                for (int i = 0; i < iEavesPurlinNo / iEavesPurlinNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[(i * iEavesPurlinNoInOneFrame) + (i + 1) * iFrameMembersNo];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_C001(current_member.NodeStart, m_arrMembers[0], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_C001(current_member.NodeEnd, m_arrMembers[0], current_member, true));

                    // Eave purlin is connected to the rafter - index i+1 and i+2
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[((i + 1) * iEavesPurlinNoInOneFrame) + (i + 1) * iFrameMembersNo + 1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate));

                    current_member = m_arrMembers[(i * iEavesPurlinNoInOneFrame) + (i + 1) * iFrameMembersNo + 1];
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * iFrameMembersNo + 2], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[((i + 1) * iEavesPurlinNoInOneFrame) + (i + 1) * iFrameMembersNo + 2], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate));
                }
            }

            // Girt Joints
            if (bGenerateGirts)
            {
                int iNumberOfGirtsPerLeftColumnInOneFrame = iGirtNoInOneFrame / 2;
                int iNumberOfGirtsPerRightColumnInOneFrame = iNumberOfGirtsPerLeftColumnInOneFrame;

                if (!bIsGableRoof)
                {
                    // Cesty a necesty ako pekne previest float na int :)
                    //iNumberOfGirtsPerLeftColumnInOneFrame = (int)((fH1_frame - fBottomGirtPosition) / fDist_Girt);
                    //iNumberOfGirtsPerLeftColumnInOneFrame = (int)Math.Ceiling((fH1_frame - fBottomGirtPosition) / fDist_Girt);
                    //iNumberOfGirtsPerLeftColumnInOneFrame = (int)Math.Floor((fH1_frame - fBottomGirtPosition) / fDist_Girt);
                    iNumberOfGirtsPerLeftColumnInOneFrame = (int)Math.Round((fH1_frame - fBottomGirtPosition) / fDist_Girt);
                    iNumberOfGirtsPerRightColumnInOneFrame = iGirtNoInOneFrame - iNumberOfGirtsPerLeftColumnInOneFrame;
                }

                for (int i = 0; i < (iFrameNo - 1) * iGirtNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + i];
                    CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                    CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));

                    int iCurrentFrameIndex = i / iGirtNoInOneFrame;
                    int iFirstGirtInFrameLeftSide = iMainColumnNo + iRafterNo + iEavesPurlinNo + iCurrentFrameIndex * iGirtNoInOneFrame;
                    int iFirstGirtInFrameRightSide = iFirstGirtInFrameLeftSide + iNumberOfGirtsPerLeftColumnInOneFrame;
                    int iCurrentMemberIndex = iMainColumnNo + iRafterNo + iEavesPurlinNo + i;

                    int iFirstGirtOnCurrentSideIndex;
                    if (iCurrentMemberIndex < iFirstGirtInFrameRightSide)
                        iFirstGirtOnCurrentSideIndex = iFirstGirtInFrameLeftSide;
                    else
                        iFirstGirtOnCurrentSideIndex = iFirstGirtInFrameRightSide;

                    if (bUseMainColumnFlyBracingPlates && iMainColumnFlyBracing_EveryXXGirt > 0 && (iCurrentMemberIndex - iFirstGirtOnCurrentSideIndex + 1) % iMainColumnFlyBracing_EveryXXGirt == 0)
                    {
                        bool bTopOfPlateInCrscVerticalAxisPlusDirection = false;

                        if (iFirstGirtInFrameRightSide <= iCurrentMemberIndex && iCurrentMemberIndex < iFirstGirtOnCurrentSideIndex + iNumberOfGirtsPerRightColumnInOneFrame)
                            bTopOfPlateInCrscVerticalAxisPlusDirection = true;

                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", "FB - RH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, bTopOfPlateInCrscVerticalAxisPlusDirection));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", "FB - RH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, bTopOfPlateInCrscVerticalAxisPlusDirection));
                    }
                    else
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }
                }
            }

            // Purlin Joints
            if (bGeneratePurlins)
            {
                for (int i = 0; i < (iFrameNo - 1) * iPurlinNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + i];
                    CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                    CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));

                    int iCurrentFrameIndex = i / iPurlinNoInOneFrame;
                    int iFirstPurlinInFrameLeftSide = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + iCurrentFrameIndex * iPurlinNoInOneFrame;
                    int iFirstPurlinInFrameRightSide = iFirstPurlinInFrameLeftSide + iPurlinNoInOneFrame / 2;
                    int iCurrentMemberIndex = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + i;

                    int iFirstPurlinOnCurrentSideIndex;
                    if (iCurrentMemberIndex < iFirstPurlinInFrameRightSide)
                        iFirstPurlinOnCurrentSideIndex = iFirstPurlinInFrameLeftSide;
                    else
                        iFirstPurlinOnCurrentSideIndex = iFirstPurlinInFrameRightSide;

                    if (bUseRafterFlyBracingPlates && iRafterFlyBracing_EveryXXPurlin > 0 && (iCurrentMemberIndex - iFirstPurlinOnCurrentSideIndex + 1) % iRafterFlyBracing_EveryXXPurlin == 0)
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", "FB - RH", current_member.NodeStart, mainMemberForStartJoint, current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", "FB - RH", current_member.NodeEnd, mainMemberForEndJoint, current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true));
                    }
                    else
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }
                }
            }

            // Front Wind Posts Foundation Joints / Top Joint to the rafter
            if (bGenerateFrontColumns)
            {
                for (int i = 0; i < iFrontColumnNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member));

                    int iSides = bIsGableRoof ? 2 : 1;

                    if (i < (int)(iFrontColumnNoInOneFrame / iSides))
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[1], current_member, -fRoofPitch_rad, bWindPostUnderRafter, true)); // Front Left Main Rafter (0 to 0.5*W)
                    else
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[2], current_member, fRoofPitch_rad, bWindPostUnderRafter, true)); // Front Right Main Rafter(0.5*W to W)
                }
            }

            // Back Wind Posts Foundation Joints / Top Joint to the rafter
            if (bGenerateBackColumns)
            {
                for (int i = 0; i < iBackColumnNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member));

                    int iSides = bIsGableRoof ? 2 : 1;

                    if (i < (int)(iBackColumnNoInOneFrame / iSides))
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[(iFrameNo - 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame) + 1], current_member, fRoofPitch_rad, bWindPostUnderRafter, false)); // Back Left Main Rafter (0 to 0.5*W)
                    else
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[(iFrameNo - 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame) + 2], current_member, -fRoofPitch_rad, bWindPostUnderRafter, false)); // Back Right Main Rafter(0.5*W to W)
                }
            }

            // Front Girt Joints
            if (bGenerateFrontGirts)
            {
                for (int i = 0; i < iFrontGirtsNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + i];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_J001(current_member.NodeStart, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_J001(current_member.NodeEnd, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame], current_member, true));

                    // Number of girts in the middle
                    int iNumberOfGirtsInMiddle = iArrNumberOfNodesPerFrontColumnFromLeft[iOneRafterFrontColumnNo - 1];
                    // Number of girts without middle
                    int iNumberOfSymmetricalGirts = iFrontGirtsNoInOneFrame - iNumberOfGirtsInMiddle;
                    // Number of girts in the half of frame (without middle)
                    int iNumberOfSymmetricalGirtsHalf = iNumberOfSymmetricalGirts / 2;

                    // Joint at member start - connected to the first main column
                    if (i < iGirtNoInOneFrame / 2) // First column of girts are connected to the first main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates)); // Use height (z dimension)
                    }
                    else if ((iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle - 1) < i && i < (iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle + iOneColumnGirtNo)) // Joint at member start - connected to the second main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[3], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates)); // Use height (z dimension)
                    }
                    else
                    {
                        CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }

                    // Joint at member end
                    CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                }
            }

            // Back Girt Joints
            if (bGenerateBackGirts)
            {
                for (int i = 0; i < iBackGirtsNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + i];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_L001(current_member.NodeStart, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_L001(current_member.NodeEnd, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame], current_member, true));

                    // Number of girts in the middle
                    int iNumberOfGirtsInMiddle = iArrNumberOfNodesPerBackColumnFromLeft[iOneRafterBackColumnNo - 1];
                    // Number of girts without middle
                    int iNumberOfSymmetricalGirts = iBackGirtsNoInOneFrame - iNumberOfGirtsInMiddle;
                    // Number of girts in the half of frame (without middle)
                    int iNumberOfSymmetricalGirtsHalf = iNumberOfSymmetricalGirts / 2;

                    // Joint at member start - connected to the first main column
                    if (i < iGirtNoInOneFrame / 2) // First column of girts are connected to the first main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[((iFrameNo - 1) * iEavesPurlinNoInOneFrame) + (iFrameNo - 1) * (iFrameNodesNo - 1)], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates)); // Use height (z dimension)
                    }
                    else if ((iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle - 1) < i && i < (iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle + iOneColumnGirtNo)) // Joint at member start - connected to the second main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[((iFrameNo - 1) * iEavesPurlinNoInOneFrame) + (iFrameNo - 1) * (iFrameNodesNo - 1) + 3], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates)); // Use height (z dimension)
                    }
                    else
                    {
                        CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }

                    CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                }
            }

            // Girt Bracing - Side walls - Joints
            if (bGenerateGirtBracingSideWalls)
            {
                for (int i = 0; i < iGBSideWallsMembersNo; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame + i];

                    //To Mato - toto je podla mna problem preco niekedy pada metoda na disablovanie member Joints - lebo sa proste tie joints pre vypnuty member proste vobec negeneruju
                    // vo vysledku teda musime stale pregenerovat joints, aby to sedelo
                    if (current_member.BIsGenerated)
                    {
                        // Joint at member start
                        CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));

                        // Joint at member end
                        CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }
                }
            }

            // Purlin Bracing - Joints
            if (bGeneratePurlinBracing)
            {
                for (int i = 0; i < iPBMembersNo; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame + iGBSideWallsMembersNo + i];

                    //To Mato - toto je podla mna problem preco niekedy pada metoda na disablovanie member Joints - lebo sa proste tie joints pre vypnuty member proste vobec negeneruju
                    // vo vysledku teda musime stale pregenerovat joints, aby to sedelo
                    if (current_member.BIsGenerated)
                    {
                        // Joint at member start
                        CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));

                        // Joint at member end
                        CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }
                }
            }

            // Girt Bracing - Front Side - Joints
            if (bGenerateGirtBracingFrontSide)
            {
                for (int i = 0; i < iNumberOfGB_FSMembersInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame + iGBSideWallsMembersNo + iPBMembersNo + i];

                    //To Mato - toto je podla mna problem preco niekedy pada metoda na disablovanie member Joints - lebo sa proste tie joints pre vypnuty member proste vobec negeneruju
                    // vo vysledku teda musime stale pregenerovat joints, aby to sedelo
                    if (current_member.BIsGenerated)
                    {
                        // Joint at member start
                        CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));

                        // Joint at member end
                        CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }
                }
            }

            // Girt Bracing - Back Side - Joints
            if (bGenerateGirtBracingBackSide)
            {
                for (int i = 0; i < iNumberOfGB_BSMembersInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame + iGBSideWallsMembersNo + iPBMembersNo + iNumberOfGB_FSMembersInOneFrame + i];

                    //To Mato - toto je podla mna problem preco niekedy pada metoda na disablovanie member Joints - lebo sa proste tie joints pre vypnuty member proste vobec negeneruju
                    // vo vysledku teda musime stale pregenerovat joints, aby to sedelo
                    if (current_member.BIsGenerated)
                    {
                        // Joint at member start
                        CMember mainMemberForStartJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeStart));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, mainMemberForStartJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));

                        // Joint at member end
                        CMember mainMemberForEndJoint = m_arrMembers.FirstOrDefault(m => m.IntermediateNodes.Contains(current_member.NodeEnd));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, mainMemberForEndJoint, current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates));
                    }
                }
            }

            // Validation - check that all created joints have assigned Main Member
            // Check all joints before definition of doors and windows members and joints
            for (int i = 0; i < m_arrConnectionJoints.Count; i++)
            {
                if (m_arrConnectionJoints[i].m_MainMember == null)
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:" + m_arrConnectionJoints[i].ID.ToString() + " Joint index in the list: " + i);
            }

            // Validation - duplicity of node ID
            for (int i = 0; i < m_arrNodes.Length; i++)
            {
                for (int j = 0; j < m_arrNodes.Length; j++)
                {
                    if ((m_arrNodes[i] != m_arrNodes[j]) && (m_arrNodes[i].ID == m_arrNodes[j].ID))
                        throw new ArgumentNullException("Duplicity in Node ID.\nNode index: " + i + " and Node index: " + j);
                }
            }
        }

        public void AddColumnsNodes(bool bConsiderAbsoluteValueOfRoofPitch, int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, float fHeight, float fDist_Columns, float fy_Global_Coord)
        {
            float z_glob;

            // Bottom nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + i] = new CNode(i_temp_numberofNodes + i + 1, (i + 1) * fDist_Columns, fy_Global_Coord, 0, 0);
                listOfSupportedNodes_S2.Add(m_arrNodes[i_temp_numberofNodes + i]);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i]);
            }

            bool bIsGableRoof = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

            int iSideNo = bIsGableRoof ? 2 : 1;

            if (bIsGableRoof)
            {
                // Bottom nodes
                for (int i = 0; i < iOneRafterColumnNo; i++)
                {
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (i + 1) * fDist_Columns, out z_glob);
                    m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + iOneRafterColumnNo + i + 1, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, 0, 0);
                    listOfSupportedNodes_S2.Add(m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i]);
                    RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i]);
                }
            }

            // Top nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i] = new CNode(i_temp_numberofNodes + iSideNo * iOneRafterColumnNo + i + 1, (i + 1) * fDist_Columns, fy_Global_Coord, z_glob, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i]);
            }

            if (bIsGableRoof)
            {
                // Top nodes
                for (int i = 0; i < iOneRafterColumnNo; i++)
                {
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (i + 1) * fDist_Columns, out z_glob);
                    m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + (iSideNo + 1) * iOneRafterColumnNo + i + 1, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, z_glob, 0);
                    RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i]);
                }
            }
        }

        public void AddColumnsMembers(
            int i_temp_numberofNodes,
            int i_temp_numberofMembers,
            int iOneRafterColumnNo,
            int iColumnNoInOneFrame,
            CMemberEccentricity eccentricityColumn,
            float fColumnAlignmentStart,
            float fColumnAlignmentEnd,
            CCrSc section,
            float fMemberRotation,
            bool bUseFlyBracing,
            int iFlyBracing_Every_XXSupportingMember,
            float fFirstSupportingMemberPositionAbsolute,
            float fSupportingMembersDistance)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, EMemberType_FS.eWP, EMemberType_FS_Position.WindPostFrontSide, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseFlyBracing, iFlyBracing_Every_XXSupportingMember, fFirstSupportingMemberPositionAbsolute, fSupportingMembersDistance, ref m_arrMembers[i_temp_numberofMembers + i]);
            }

            bool bIsGableRoof = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

            if (bIsGableRoof)
            {
                for (int i = 0; i < iOneRafterColumnNo; i++)
                {
                    m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, EMemberType_FS.eWP, EMemberType_FS_Position.WindPostBackSide, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
                    CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseFlyBracing, iFlyBracing_Every_XXSupportingMember, fFirstSupportingMemberPositionAbsolute, fSupportingMembersDistance, ref m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i]);
                }
            }
        }

        public int GetNumberofIntermediateNodesInOneColumnForGirts(bool bConsiderAbsoluteValueOfRoofPitch, float fHeight, float fBottomGirtPosition_temp, float fDistBetweenColumns, float fz_UpperLimitForGirts, int iColumnIndex)
        {
            float fz_gcs_column_temp;

            CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (iColumnIndex + 1) * fDistBetweenColumns, out fz_gcs_column_temp);
            int iNumber_of_segments = (int)((fz_gcs_column_temp - fz_UpperLimitForGirts - fBottomGirtPosition_temp) / fDist_Girt);
            return iNumber_of_segments + 1;
        }

        public int GetNumberofIntermediateNodesInColumnsForOneFrame(bool bConsiderAbsoluteValueOfRoofPitch, int iOneRafterColumnNo, float fHeight, float fBottomGirtPosition_temp, float fDistBetweenColumns, float fz_UpperLimitForGirts)
        {
            int iNo_temp = 0;
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                iNo_temp += GetNumberofIntermediateNodesInOneColumnForGirts(bConsiderAbsoluteValueOfRoofPitch, fHeight, fBottomGirtPosition_temp, fDistBetweenColumns, fz_UpperLimitForGirts, i);
            }

            return iNo_temp;
        }

        public void AddFrontOrBackGirtsNodes(bool bConsiderAbsoluteValueOfRoofPitch, int iOneRafterColumnNo, int[] iArrNumberOfNodesPerColumn, int i_temp_numberofNodes, int iIntermediateColumnNodesForGirtsOneRafterNo, float fHeight, float fDist_Girts, float fDist_Columns, float fy_Global_Coord)
        {
            int iTemp = 0;

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                float z_glob;
                CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (i + 1) * fDist_Columns, out z_glob);

                for (int j = 0; j < iArrNumberOfNodesPerColumn[i]; j++)
                {
                    m_arrNodes[i_temp_numberofNodes + iTemp + j] = new CNode(i_temp_numberofNodes + iTemp + j + 1, (i + 1) * fDist_Columns, fy_Global_Coord, fBottomGirtPosition + j * fDist_Girts, 0);
                    RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iTemp + j]);
                }

                iTemp += iArrNumberOfNodesPerColumn[i];
            }

            bool bIsGableRoof = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

            if (bIsGableRoof)
            {
                iTemp = 0;

                for (int i = 0; i < iOneRafterColumnNo; i++)
                {
                    float z_glob;
                    CalcColumnNodeCoord_Z(bConsiderAbsoluteValueOfRoofPitch, fHeight, (i + 1) * fDist_Columns, out z_glob);

                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i]; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j] = new CNode(i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j + 1, fW_frame - (i + 1) * fDist_Columns, fy_Global_Coord, fBottomGirtPosition + j * fDist_Girts, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j]);
                    }

                    iTemp += iArrNumberOfNodesPerColumn[i];
                }
            }
        }

        public void AddFrontOrBackGirtsMembers(int iFrameNodesNo, int iOneRafterColumnNo, int iLeftColumnGirtNo, int[] iArrNumberOfNodesPerColumn, int[] iArrNumberOfGirtsPerColumn, int i_temp_numberofNodes, int i_temp_numberofMembers,
            int iIntermediateColumnNodesForGirtsOneRafterNo, int iIntermediateColumnNodesForGirtsOneFrameNo, int iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection,
            float fDist_Girts, CMemberEccentricity eGirtEccentricity, float fGirtStart_MC, float fGirtStart, float fGirtEnd, CCrSc section, EMemberType_FS_Position eMemberType_FS_Position, float fMemberRotation, int iNumberOfTransverseSupports)
        {
            bool bIsGableRoof = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

            int iTemp = 0; // Docasny pocet prutov
            int iTemp2 = 0; // Docasny pocet uzlov

            for (int i = 0; i < iOneRafterColumnNo + 1; i++)
            {
                if (i == 0) // First session depends on number of girts at main frame column
                {
                    for (int j = 0; j < iArrNumberOfGirtsPerColumn[0]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + j] = new CMember(i_temp_numberofMembers + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + j], m_arrNodes[i_temp_numberofNodes + j], section, EMemberType_FS.eG, eMemberType_FS_Position, eGirtEccentricity, eGirtEccentricity, fGirtStart_MC, fGirtEnd, fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + j], iNumberOfTransverseSupports);
                    }

                    iTemp += iArrNumberOfGirtsPerColumn[0];
                }
                else if (i < iOneRafterColumnNo) // Other sessions
                {
                    for (int j = 0; j < iArrNumberOfGirtsPerColumn[i]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FS.eG, eMemberType_FS_Position, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iTemp + j], iNumberOfTransverseSupports);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfGirtsPerColumn[i];
                }
                else // Last session - prechadza cez stred budovy
                {
                    int iNumberOfGirtsInLastSession = iArrNumberOfGirtsPerColumn[i];

                    for (int j = 0; j < iNumberOfGirtsInLastSession; j++) // Ak je uhol sklonu zaporny tak rozhoduje pocet uzlov na pravej strane, ktory moze byt mensi ako je na lavej
                    {
                        if (!bIsGableRoof)
                            m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + iLeftColumnGirtNo + j], section, EMemberType_FS.eG, eMemberType_FS_Position, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtStart_MC, fMemberRotation, 0);
                        else
                            m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerColumn[iOneRafterColumnNo - 1] + j], section, EMemberType_FS.eG, eMemberType_FS_Position, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);

                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iTemp + j], iNumberOfTransverseSupports);
                    }

                    iTemp += iNumberOfGirtsInLastSession;
                }
            }

            if (bIsGableRoof)
            {
                iTemp = 0;
                iTemp2 = 0;

                for (int i = 0; i < iOneRafterColumnNo; i++)
                {
                    int iNumberOfMembers_temp = iArrNumberOfGirtsPerColumn[0] + iIntermediateColumnNodesForGirtsOneRafterNo;

                    CMemberEccentricity eGirtEccentricity_temp = new CMemberEccentricity(eGirtEccentricity.MFy_local, -eGirtEccentricity.MFz_local);

                    if (i == 0) // First session depends on number of girts at main frame column
                    {
                        for (int j = 0; j < iArrNumberOfGirtsPerColumn[0]; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + iArrNumberOfGirtsPerColumn[0] + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + j], section, EMemberType_FS.eG, eMemberType_FS_Position, eGirtEccentricity_temp, eGirtEccentricity_temp, fGirtStart_MC, fGirtEnd, -fMemberRotation, 0);
                            CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j], iNumberOfTransverseSupports);
                        }

                        iTemp += iArrNumberOfGirtsPerColumn[0];
                    }
                    else // Other sessions (not in the middle)
                    {
                        for (int j = 0; j < iArrNumberOfGirtsPerColumn[i]; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FS.eG, eMemberType_FS_Position, eGirtEccentricity_temp, eGirtEccentricity_temp, fGirtStart, fGirtEnd, -fMemberRotation, 0);
                            CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j], iNumberOfTransverseSupports);
                        }

                        iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                        iTemp += iArrNumberOfGirtsPerColumn[i];
                    }
                }
            }
        }

        protected void FillIntermediateNodesForMembers()
        {
            foreach (CMember m in this.m_arrMembers)
            {
                foreach (CNode n in this.m_arrNodes)
                {
                    if (m.IsIntermediateNode(n)) m.IntermediateNodes.Add(n);
                }
            }

            bool debugging = false;
            if (debugging)
            {
                foreach (CMember m in this.m_arrMembers)
                {
                    if (m.IntermediateNodes.Count > 0) System.Diagnostics.Trace.WriteLine($"ID:{m.ID} Name:{m.Name} IntNodesCount: {m.IntermediateNodes.Count}");
                }
            }
        }

        public void CalcPurlinNodeCoord(float x_rel, out float x_global, out float z_global)
        {
            x_global = (float)Math.Cos(fRoofPitch_rad) * x_rel;
            z_global = fH1_frame + (float)Math.Sin(fRoofPitch_rad) * x_rel;
        }

        public void CalcColumnNodeCoord_Z(bool bConsiderAbsoluteValueOfRoofPitch, float fHeight, float x, out float z_global)
        {
            bool bIsGableRoof = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

            float fRoofPitch_temp = fRoofPitch_rad;

            if(bConsiderAbsoluteValueOfRoofPitch)
                fRoofPitch_temp = Math.Abs(fRoofPitch_rad);

            // Ocakava sa ze vyska je mensia z oboch stran a uhol je vzdy brany ako kladny
            if (x<= 0.5f * fW_frame || !bIsGableRoof)
                z_global = fHeight + (float)Math.Tan(fRoofPitch_temp) * x;
            else
                z_global = fHeight + (float)Math.Tan(fRoofPitch_temp) * (fW_frame - x);
        }

        // Rotate Node in Front or Back Frame about Z (angle between X and Front or Back Frame
        public void RotateFrontOrBackFrameNodeAboutZ(CNode node_temp)
        {
            // Rake Angles - Front and Back Frame
            // Upravi suradnice Y vsetkych uzlov s Y = 0 a s Y = L
            // Prepocita suradnicu podla hodnoty X a uhlu, ktory je nastaveny medzi globalnou osou X a prednym (prvym) alebo zadnym (poslednym) ramom
            // Tato uprava umoznuje zosikmenie prveho / posledneho ramu voci mezilahlym

            if (node_temp != null)
            {
                if (MathF.d_equal(node_temp.Y, 0) && !MathF.d_equal(fFrontFrameRakeAngle_temp_rad, 0)) // Front Frame
                {
                    node_temp.Y += node_temp.X * (float)Math.Tan(fFrontFrameRakeAngle_temp_rad);
                }

                if (MathF.d_equal(node_temp.Y, fL_tot) && !MathF.d_equal(fBackFrameRakeAngle_temp_rad, 0)) // Back Frame
                {
                    node_temp.Y += node_temp.X * (float)Math.Tan(fBackFrameRakeAngle_temp_rad);
                }
            }
        }

        public void RotateAndTranslateNodeAboutZ_CCW(Point3D pControlPoint, ref CNode node, float fAngle_rad)
        {
            Point3D p = node.GetPoint3D();
            RotateAndTranslatePointAboutZ_CCW(pControlPoint, ref p, fAngle_rad);

            node.X = (float)p.X;
            node.Y = (float)p.Y;
        }

        public void RotateAndTranslatePointAboutZ_CCW(Point3D pControlPoint, ref Point3D point, float fAngle_rad)
        {
            // Rotate node
            float fx = (float)Geom2D.GetRotatedPosition_x_CCW_rad(point.X, point.Y, fAngle_rad);
            float fy = (float)Geom2D.GetRotatedPosition_y_CCW_rad(point.X, point.Y, fAngle_rad);

            // Set rotated coordinates
            point.X = fx;
            point.Y = fy;

            // Translate node
            point.X += (float)pControlPoint.X;
            point.Y += (float)pControlPoint.Y;
        }

        protected void DeactivateMemberBracingBlocks(CMember m, CBlock block, List<Point3D> openningPointsInGCS)
        {
            float fAdditionalOffset = 0.2f; // Ak nechceme aby brace (bracing blok) bol hned vela door alebo window
            IEnumerable<CMember> bracingBlocks = m_arrMembers.Where(mGB => mGB.EMemberType == EMemberType_FS.eGB && (m.IntermediateNodes.Contains(mGB.NodeStart) || m.IntermediateNodes.Contains(mGB.NodeEnd)));
            foreach (CMember mBB in bracingBlocks)
            {
                if (block.BuildingSide == "Left" || block.BuildingSide == "Right") // Blok je v lavej alebo pravej stene, LCS x bloku odpoveda smer v GCS Y // Porovnavame suradnice GCS Y
                {
                    if (mBB.NodeStart.Y >= openningPointsInGCS[0].Y - fAdditionalOffset && mBB.NodeStart.Y <= openningPointsInGCS[1].Y + fAdditionalOffset)
                    {
                        DeactivateMemberAndItsJoints(mBB);
                    }
                }
                else // Blok je v prednej alebo zadnej stene, LCS x bloku odpoveda smer v GCS X // Porovnavame suradnice GCS X
                {
                    if (mBB.NodeStart.X >= openningPointsInGCS[0].X - fAdditionalOffset && mBB.NodeStart.X <= openningPointsInGCS[1].X + fAdditionalOffset)
                    {
                        DeactivateMemberAndItsJoints(mBB);
                    }
                }
            }
        }

        protected void DeactivateBracingBlocksThroughtBlock(CBlock block, List<Point3D> openningPointsInGCS)
        {
            float fAdditionalOffset = 0.2f; // Ak nechceme aby brace (bracing blok) bol hned vela door alebo window
            //vyberieme podla Z suradnice tie,ktore pretinaju okno
            IEnumerable<CMember> bracingBlocks = m_arrMembers.Where(mGB => mGB.EMemberType == EMemberType_FS.eGB &&
                ((mGB.NodeStart.Z <= openningPointsInGCS[0].Z && mGB.NodeEnd.Z >= openningPointsInGCS[3].Z)
                || (mGB.NodeStart.Z >= openningPointsInGCS[0].Z && mGB.NodeEnd.Z <= openningPointsInGCS[3].Z)));
            foreach (CMember mBB in bracingBlocks)
            {
                if (block.BuildingSide == "Left" || block.BuildingSide == "Right") // Blok je v lavej alebo pravej stene, LCS x bloku odpoveda smer v GCS Y // Porovnavame suradnice GCS Y
                {
                    // Bug 411 - pridana prva podmienka - musime kontrolovat aj to na ktorej strane budovy sme
                    if (MathF.d_equal(mBB.NodeStart.X, openningPointsInGCS[0].X) && (mBB.NodeStart.Y >= openningPointsInGCS[0].Y - fAdditionalOffset && mBB.NodeStart.Y <= openningPointsInGCS[1].Y + fAdditionalOffset))
                    {
                        DeactivateMemberAndItsJoints(mBB);
                    }
                }
                else // Blok je v prednej alebo zadnej stene, LCS x bloku odpoveda smer v GCS X // Porovnavame suradnice GCS X
                {
                    // Bug 411 - pridana prva podmienka - musime kontrolovat aj to na ktorej strane budovy sme
                    if (MathF.d_equal(mBB.NodeStart.Y, openningPointsInGCS[0].Y) && (mBB.NodeStart.X >= openningPointsInGCS[0].X - fAdditionalOffset && mBB.NodeStart.X <= openningPointsInGCS[1].X + fAdditionalOffset))
                    {
                        DeactivateMemberAndItsJoints(mBB);
                    }
                }
            }
        }

        // Load model component cross-sections
        public void SetCrossSectionsFromDatabase()
        {
            // Todo - Ondrej
            // Chcel som napojit obsah m_arrCrSc podla 
            // MDBModels tabulka KitsetGableRoofEnclosed alebo KitsetGableRoofEnclosedCrscID
            // Skusis mi aspon jeden prierez napojit prosim ?
            // pole m_arrCrSc by sa malo naplnit prierezmi podla mena prierezu (270115, 27095, 63020, ...) v databaze pre jednotlive typy members (purlin, girt, main column, ...)
            // kazdemu menu prierezu zodpoveda iny objekt z tried, ktore su v CRSC / CrSc_3 / FS
        }

        // Add members to the member group list
        // Base on cross-section ID 
        // TODO - spravnejsie by bolo pridavat member do zoznamu priamo pri vytvoreni
        public void AddMembersToMemberGroupsLists()
        {
            int i = 0;

            // Opravene ID prierezu sa bralo z poradia v databaze a prepisovalo ID prierezu z modelu
            foreach (CMember member in m_arrMembers)
            {
                foreach (CMemberGroup group in listOfModelMemberGroups) // TODO - dalo by sa nahradit napriklad switchom ak pozname presne typy
                {
                    if (member.BIsGenerated && member.CrScStart.ID == group.CrossSection.ID) // In case that cross-section ID is same add member to the list
                    {
                        member.EMemberTypePosition = group.MemberType_FS_Position; // TODO - docasne riesenie, nastavime prutu typ podla pozicie (referencny typ pruta nastaveny v skupine), spravne by sme ho mohli nastavit uz v konstruktore pruta pri jeho vytvoreni
                        group.ListOfMembers.Add(member);
                        //listOfModelMemberGroups[group.CrossSection.ICrSc_ID].ListOfMembers.Add(member);
                        i++;
                        break;
                    }
                }
            }

            // Check
            // TODO - aktivovat po vyrieseni mazania nevygenerovanych prutov zo zoznamu a pridani prutov tvoriacich bloky (dvere, okna, ...)
            /*
            if (i != m_arrMembers.Length)
                throw new Exception("Not all members were added.");
            */
        }

        protected List<CSegment_LTB> GenerateIntermediateLTBSegmentsOnMember(List<CIntermediateTransverseSupport> lTransverseSupportGroup, bool bIsRelativeCoordinate_x, float fMemberLength)
        {
            List<CSegment_LTB> LTB_segment_group = null; // TODO - rozhodnut co sa ma generovat ak na prute nie su medzilahle podpory, nic alebo vzdy jeden segment ???

            if (lTransverseSupportGroup != null && lTransverseSupportGroup.Count > 0)
            {
                LTB_segment_group = new List<CSegment_LTB>();

                // Create lateral-torsional buckling segments
                for (int j = 0; j < lTransverseSupportGroup.Count + 1; j++)
                {
                    // Number of segments = number of intermediate supports + 1 - type BothFlanges
                    // TODO - doriesit ako generovat segmenty ak nie su vsetky lateral supports typu BothFlanges
                    // Najprv by sa mal najst pocet podpor s BothFlanges, z toho urcit pocet segmentov a zohladnovat len tie coordinates x,
                    // ktore sa vztahuju na podpory s BothFlanges

                    float fSegmentStart_abs = 0;
                    float fSegmentEnd_abs = fMemberLength;

                    if (j == 0) // First Segment
                    {
                        fSegmentStart_abs = 0f;
                        fSegmentEnd_abs = lTransverseSupportGroup[j].Fx_position_abs;
                    }
                    else if (j < lTransverseSupportGroup.Count)
                    {
                        fSegmentStart_abs = lTransverseSupportGroup[j - 1].Fx_position_abs;
                        fSegmentEnd_abs = lTransverseSupportGroup[j].Fx_position_abs;
                    }
                    else // Last
                    {
                        fSegmentStart_abs = lTransverseSupportGroup[j - 1].Fx_position_abs;
                        fSegmentEnd_abs = fMemberLength;
                    }

                    CSegment_LTB segment = new CSegment_LTB(j + 1, bIsRelativeCoordinate_x, fSegmentStart_abs, fSegmentEnd_abs, fMemberLength);

                    LTB_segment_group.Add(segment);
                }
            }

            return LTB_segment_group;
        }

        protected List<CIntermediateTransverseSupport> GenerateRegularIntermediateTransverseSupports(float fMemberLength, int iNumberOfTransverseSupports)
        {
            List<CIntermediateTransverseSupport> TransverseSupportGroup = null;

            float fFirstSupportPosition = fMemberLength / (iNumberOfTransverseSupports + 1); // number of LTB segments = number of support + 1
            float fDistOfSupports = fFirstSupportPosition;

            if (iNumberOfTransverseSupports > 0)
            {
                TransverseSupportGroup = new List<CIntermediateTransverseSupport>();
                for (int j = 0; j < iNumberOfTransverseSupports; j++) // Each suport
                {
                    float fxLocationOfSupport = Math.Min(fFirstSupportPosition + j * fDistOfSupports, fMemberLength);

                    if (fxLocationOfSupport < fMemberLength)
                        TransverseSupportGroup.Add(new CIntermediateTransverseSupport(j + 1, EITSType.eBothFlanges, fxLocationOfSupport / fMemberLength, fxLocationOfSupport, 0));
                    // TODO - To Ondrej, nie som si isty ci mam v kazdej podpore CIntermediateTransverseSupport ukladat jej poziciu (aktualny stav) alebo ma CMember nie list CIntermediateTransverseSupport ale list nejakych struktur (x, CIntermediateTransverseSupport), takze x miesta budu definovane v tejto strukture v objekte CMember a samotny objekt CIntermediateTransverseSupport nebude vediet kde je
                }
            }

            return TransverseSupportGroup;
        }

        protected void CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(CMember member, int iNumberOfTransverseSupports)
        {
            List<CIntermediateTransverseSupport> TransverseSupportGroup = GenerateRegularIntermediateTransverseSupports(member.FLength, iNumberOfTransverseSupports);
            List<CSegment_LTB> LTB_segment_group = GenerateIntermediateLTBSegmentsOnMember(TransverseSupportGroup, false, member.FLength);

            // Assign transverse support group and LTB segment group to the member
            member.IntermediateTransverseSupportGroup = TransverseSupportGroup;
            member.LTBSegmentGroup = LTB_segment_group;
        }

        protected void CreateIrregularTransverseSupportGroupAndLTBsegmentGroup(
        bool bUseFlyBracing,
        int iFlyBracing_Every_XXSupportingMember,
        float fFirstSupportingMemberPositionAbsolute,
        float fSupportingMembersDistance,
        ref CMember member,
        out List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member,
        out List<CSegment_LTB> LTB_segment_group_Member)
        {
            lTransverseSupportGroup_Member = null;
            LTB_segment_group_Member = null;

            // Define fly bracing positions and LTB segments on member
            float fMemberLength = member.FLength;

            if (bUseFlyBracing && iFlyBracing_Every_XXSupportingMember > 0)
            {
                lTransverseSupportGroup_Member = new List<CIntermediateTransverseSupport>();
                float fFirstFlyBracePosition = fFirstSupportingMemberPositionAbsolute + (iFlyBracing_Every_XXSupportingMember - 1) * fSupportingMembersDistance;
                int iNumberOfFlyBracesOnMember = fFirstFlyBracePosition < fMemberLength ? (int)((fMemberLength - fFirstFlyBracePosition) / (iFlyBracing_Every_XXSupportingMember * fSupportingMembersDistance)) + 1 : 0;

                for (int j = 0; j < iNumberOfFlyBracesOnMember; j++) // Each fly brace
                {
                    float fxLocationOfFlyBrace = fFirstFlyBracePosition + (j * iFlyBracing_Every_XXSupportingMember) * fSupportingMembersDistance;

                    if (fxLocationOfFlyBrace < fMemberLength)
                        lTransverseSupportGroup_Member.Add(new CIntermediateTransverseSupport(j + 1, EITSType.eBothFlanges, fxLocationOfFlyBrace / fMemberLength, fxLocationOfFlyBrace, 0));
                    // TODO - To Ondrej, nie som si isty ci mam v kazdej podpore CIntermediateTransverseSupport ukladat jej poziciu (aktualny stav) alebo ma CMember nie list CIntermediateTransverseSupport ale list nejakych struktur (x, CIntermediateTransverseSupport), takze x miesta budu definovane v tejto strukture v objekte CMember a samotny objekt CIntermediateTransverseSupport nebude vediet kde je
                }

                if (lTransverseSupportGroup_Member.Count > 0)
                {
                    LTB_segment_group_Member = GenerateIntermediateLTBSegmentsOnMember(lTransverseSupportGroup_Member, false, fMemberLength);
                }
            }
        }

        protected void CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(
            bool bUseFlyBracing,
            int iFlyBracing_Every_XXSupportingMember,
            float fFirstSupportingMemberPositionAbsolute,
            float fSupportingMembersDistance,
            ref CMember member)
        {
            List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member;
            List<CSegment_LTB> LTB_segment_group_Member;

            CreateIrregularTransverseSupportGroupAndLTBsegmentGroup(
                bUseFlyBracing,
                iFlyBracing_Every_XXSupportingMember,
                fFirstSupportingMemberPositionAbsolute,
                fSupportingMembersDistance,
                ref member,
                out lTransverseSupportGroup_Member,
                out LTB_segment_group_Member);

            if (lTransverseSupportGroup_Member != null && lTransverseSupportGroup_Member.Count > 0)
            {
                // Assign transverse support group to the member
                member.IntermediateTransverseSupportGroup = lTransverseSupportGroup_Member;
                member.LTBSegmentGroup = LTB_segment_group_Member;
            }
        }

        protected void CreateAndAssignReversedIrregularTransverseSupportGroupAndLTBsegmentGroup(
            bool bUseFlyBracing,
            int iFlyBracing_Every_XXSupportingMember,
            float fFirstSupportingMemberPositionAbsolute,
            float fSupportingMembersDistance,
            ref CMember member)
        {
            // Suradnice pre member opacne orientovane, takze pouzit L - x
            float fmemberLength = member.FLength;
            List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member = new List<CIntermediateTransverseSupport>();
            List<CSegment_LTB> LTB_segment_group_Member = new List<CSegment_LTB>();
            CreateIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseFlyBracing, iFlyBracing_Every_XXSupportingMember, fBottomGirtPosition, fDist_Girt, ref member, out lTransverseSupportGroup_Member, out LTB_segment_group_Member);

            // Reverse ILS and LTB segments
            List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member_Reverse = new List<CIntermediateTransverseSupport>();
            List<CSegment_LTB> LTB_segment_group_Member_Reverse = new List<CSegment_LTB>();

            if (lTransverseSupportGroup_Member != null && lTransverseSupportGroup_Member.Count != 0) // List is initialized and some intermediate supports exists
            {
                for (int j = 0; j < lTransverseSupportGroup_Member.Count; j++)
                {
                    int iSupportIndex = lTransverseSupportGroup_Member.Count - j - 1;
                    CIntermediateTransverseSupport lTransverseSupport = new CIntermediateTransverseSupport(j + 1, lTransverseSupportGroup_Member[iSupportIndex].Type, 1f - lTransverseSupportGroup_Member[iSupportIndex].Fx_position_rel, fmemberLength - lTransverseSupportGroup_Member[iSupportIndex].Fx_position_abs);
                    lTransverseSupportGroup_Member_Reverse.Add(lTransverseSupport);
                }

                LTB_segment_group_Member_Reverse = GenerateIntermediateLTBSegmentsOnMember(lTransverseSupportGroup_Member_Reverse, false, fmemberLength);

                member.IntermediateTransverseSupportGroup = lTransverseSupportGroup_Member_Reverse;
                member.LTBSegmentGroup = LTB_segment_group_Member_Reverse;
            }
        }

        public void DeactivateMember(ref CMember m)
        {
            if (m != null)
            {
                m.BIsGenerated = false;
                m.BIsDisplayed = false;
                m.BIsSelectedForIFCalculation = false;
                m.BIsSelectedForDesign = false;
                m.BIsSelectedForMaterialList = false;
            }
        }

        public void DeactivateJoint(ref CConnectionJointTypes j)
        {
            // Deaktivujeme parametre spoje aj vsetkych jeho component
            if (j != null)
            {
                j.BIsGenerated = false;
                j.BIsDisplayed = false;
                j.BIsSelectedForDesign = false;
                j.BIsSelectedForMaterialList = false;

                // Deactivate joint components

                if (j.m_arrConnectors != null && j.m_arrConnectors.Length > 0)
                {
                    foreach (CConnector connector in j.m_arrConnectors)
                    {
                        connector.BIsGenerated = false;
                        connector.BIsDisplayed = false;
                        connector.BIsSelectedForDesign = false;
                        connector.BIsSelectedForMaterialList = false;
                    }
                }

                if (j.m_arrWelds != null && j.m_arrWelds.Length > 0)
                {
                    foreach (CWeld weld in j.m_arrWelds)
                    {
                        weld.BIsGenerated = false;
                        weld.BIsDisplayed = false;
                        weld.BIsSelectedForDesign = false;
                        weld.BIsSelectedForMaterialList = false;
                    }
                }

                if (j.m_arrPlates != null && j.m_arrPlates.Length > 0)
                {
                    foreach (CPlate plate in j.m_arrPlates)
                    {
                        plate.BIsGenerated = false;
                        plate.BIsDisplayed = false;
                        plate.BIsSelectedForDesign = false;
                        plate.BIsSelectedForMaterialList = false;

                        if (plate.ScrewArrangement != null)
                        {
                            plate.ScrewArrangement.referenceScrew.BIsGenerated = false;
                            plate.ScrewArrangement.referenceScrew.BIsDisplayed = false;
                            plate.ScrewArrangement.referenceScrew.BIsSelectedForDesign = false;
                            plate.ScrewArrangement.referenceScrew.BIsSelectedForMaterialList = false;

                            if (plate.ScrewArrangement.Screws != null && plate.ScrewArrangement.Screws.Length > 0)
                            {
                                foreach (CScrew screw in plate.ScrewArrangement.Screws)
                                {
                                    screw.BIsGenerated = false;
                                    screw.BIsDisplayed = false;
                                    screw.BIsSelectedForDesign = false;
                                    screw.BIsSelectedForMaterialList = false;
                                }
                            }
                        }

                        // Base Joint
                        if (j is CConnectionJoint_TA01 || j is CConnectionJoint_TB01 || j is CConnectionJoint_TC01 || j is CConnectionJoint_TD01) // Tato podmienka je zbytocna
                        {
                            if (plate is CConCom_Plate_B_basic) // Base plate
                            {
                                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

                                basePlate.AnchorArrangement.referenceAnchor.BIsGenerated = false;
                                basePlate.AnchorArrangement.referenceAnchor.BIsDisplayed = false;
                                basePlate.AnchorArrangement.referenceAnchor.BIsSelectedForDesign = false;
                                basePlate.AnchorArrangement.referenceAnchor.BIsSelectedForMaterialList = false;

                                if (basePlate.AnchorArrangement != null && basePlate.AnchorArrangement.Anchors.Length > 0)
                                {
                                    foreach (CAnchor anchor in basePlate.AnchorArrangement.Anchors)
                                    {
                                        anchor.BIsGenerated = false;
                                        anchor.BIsDisplayed = false;
                                        anchor.BIsSelectedForDesign = false;
                                        anchor.BIsSelectedForMaterialList = false;

                                        anchor.WasherBearing.BIsGenerated = false;
                                        anchor.WasherBearing.BIsDisplayed = false;
                                        anchor.WasherBearing.BIsSelectedForDesign = false;
                                        anchor.WasherBearing.BIsSelectedForMaterialList = false;

                                        anchor.WasherPlateTop.BIsGenerated = false;
                                        anchor.WasherPlateTop.BIsDisplayed = false;
                                        anchor.WasherPlateTop.BIsSelectedForDesign = false;
                                        anchor.WasherPlateTop.BIsSelectedForMaterialList = false;

                                        foreach (CNut nut in anchor.Nuts)
                                        {
                                            nut.BIsGenerated = false;
                                            nut.BIsDisplayed = false;
                                            nut.BIsSelectedForDesign = false;
                                            nut.BIsSelectedForMaterialList = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DeactivateMemberAndItsJoints(ref CMember m)
        {
            DeactivateMember(ref m);

            try
            {
                CConnectionJointTypes jStart;
                CConnectionJointTypes jEnd;
                GetModelMemberStartEndConnectionJoints(m, out jStart, out jEnd);

                // Tu stoji za zvazenie ci sa maju spoje deaktivovat alebo aj zmazat
                DeactivateJoint(ref jStart);
                DeactivateJoint(ref jEnd);
            }
            catch (Exception ex)
            {
                bool debugging = true;
                if (debugging) System.Diagnostics.Trace.WriteLine($"DeactivateMemberAndItsJoints: Error: [{ex.Message}] m.ID:{m.ID}  Prefix: {m.Prefix}");

            };
        }

        protected void ValidateIDs()
        {
            // Nodes and Members

            // Validacia generovanych prutov a uzlov (overime ci vsetky generovane nodes a members maju ID o 1 vacsie ako je index v poli
            bool bValidateIDs = false;

            if (bValidateIDs)
            {
                for (int i = 0; i < m_arrNodes.Length; i++)
                {
                    if (m_arrNodes[i].ID != i + 1)
                        throw new Exception("Invalid ID - Node Index:" + i.ToString() + " Node ID: " + m_arrNodes[i].ID);
                }

                for (int i = 0; i < m_arrMembers.Length; i++)
                {
                    if (m_arrMembers[i].ID != i + 1)
                        throw new Exception("Invalid ID - Member Index:" + i.ToString() + " Member ID: " + m_arrMembers[i].ID);
                }
            }
        }

        public void DeactivateMemberAndItsJoints(CMember m)
        {
            DeactivateMemberAndItsJoints(ref m);
        }

        protected void CreateFoundations(bool bGenerateFrontColumns, bool bGenerateBackColumns, bool bIsReinforcementBarStraight)
        {
            bool bGenerateFoundations = true;
            //bool bIsReinforcementBarStraight = false; // Nastavime bool, aky typ vyztuze chceme vytvorit

            if (bGenerateFoundations)
            {
                // Docasne
                CMat_02_00 materialConcrete = new CMat_02_00();
                materialConcrete.Name = "25";
                materialConcrete.Fck = 25e+6f;
                materialConcrete.m_fRho = 2300f;
                materialConcrete.m_fE = 30e+9f;

                DATABASE.DTO.CMatPropertiesRC concreteProperties = DATABASE.CMaterialManager.LoadMaterialPropertiesRC(materialConcrete.Name);
                materialConcrete.Fck = concreteProperties.Fc;
                materialConcrete.m_fRho = (float)concreteProperties.Rho;
                materialConcrete.m_fE = (float)concreteProperties.E;

                // Foundations
                // Footings
                m_arrFoundations = new List<CFoundation>(iMainColumnNo + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame);

                // Main Column - Footings
                // TODO - Predbezne doporucene hodnoty velkosti zakladov vypocitane z rozmerov budovy
                // UC - Footings
                float fTributaryArea_Wall = MathF.Average(fH1_frame, fH2_frame) * fL1_frame;
                float fTributaryArea_Roof = 0.5f * fW_frame * fL1_frame;
                float fMainColumnFooting_aX = (float)Math.Round(MathF.Max(0.7f, 0.014f * (fTributaryArea_Wall + fTributaryArea_Roof)), 1);
                float fMainColumnFooting_bY = (float)Math.Round(MathF.Max(0.8f, 0.015f * (fTributaryArea_Wall + fTributaryArea_Roof)), 1);
                float fMainColumnFooting_h = 0.45f; // "AS 2870 - Footing pad size must be between 0.45 and 2 [m]" // TODO napojit na tabulku normy
                float fConcreteCover = 0.075f; // Concrete Cover - UC - Footings

                CReinforcementBar MainColumnFootingReference_Top_Bar_x;
                CReinforcementBar MainColumnFootingReference_Top_Bar_y;
                CReinforcementBar MainColumnFootingReference_Bottom_Bar_x;
                CReinforcementBar MainColumnFootingReference_Bottom_Bar_y;

                int iMainColumnFootingNumberOfBarsTop_x;
                int iMainColumnFootingNumberOfBarsTop_y;
                int iMainColumnFootingNumberOfBarsBottom_x;
                int iMainColumnFootingNumberOfBarsBottom_y;

                CreateReferenceReinforcementBars(
                    bIsReinforcementBarStraight,
                    fMainColumnFooting_aX,
                    fMainColumnFooting_bY,
                    fMainColumnFooting_h,
                    out MainColumnFootingReference_Top_Bar_x,
                    out MainColumnFootingReference_Top_Bar_y,
                    out MainColumnFootingReference_Bottom_Bar_x,
                    out MainColumnFootingReference_Bottom_Bar_y,
                    out iMainColumnFootingNumberOfBarsTop_x,
                    out iMainColumnFootingNumberOfBarsTop_y,
                    out iMainColumnFootingNumberOfBarsBottom_x,
                    out iMainColumnFootingNumberOfBarsBottom_y
                    );

                for (int i = 0; i < iFrameNo; i++)
                {
                    float fMainColumnFooting_Eccentricity_x = 0f;
                    float fMainColumnFooting_Eccentricity_y = 0.5f * (fMainColumnFooting_bY - (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h);

                    EMemberType_FS_Position columnTypePosition = EMemberType_FS_Position.MainColumn;
                    string sName = "A";
                    string sDescriptionText = "PAD TYPE A [MC]";
                    //Color color = Colors.LightSkyBlue;

                    if (i == 0 || i == (iFrameNo - 1)) // First or last frame
                    {
                        columnTypePosition = EMemberType_FS_Position.EdgeColumn;
                        sName = "B";
                        sDescriptionText = "PAD TYPE B [EC]";
                        fMainColumnFooting_Eccentricity_y = 0.5f * (fMainColumnFooting_bY - (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].h);

                        // Zistenie hrubky base plate v spoji
                        float fBasePlateThickness = 0.003f;
                        if (m_arrConnectionJoints != null)
                            fBasePlateThickness = (m_arrConnectionJoints.Find(x => x.m_MainMember.EMemberTypePosition == columnTypePosition)).m_arrPlates[0].Ft;

                        // Kedze kotevny plech sa je pripojeny z vonkajsej strany stlpa je potrebne pocitat pri excentricite s tymto rozmerom a zmmensit ju

                        // Front side edge frame columns
                        fMainColumnFooting_Eccentricity_x = 0.5f * (fMainColumnFooting_aX - (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].b - 2 * fBasePlateThickness);

                        // Back side edge frame columns
                        if (i == (iFrameNo - 1))
                            fMainColumnFooting_Eccentricity_x *= -1;

                        //color = Colors.LightSteelBlue;
                    }

                    // Left
                    CNode node_left = m_arrNodes[i * iFrameNodesNo + 0];
                    //Point3D controlPoint_left = new Point3D(i * 2 + 1, node_left.X - 0.5f * fMainColumnFooting_aX, node_left.Y - 0.5f * fMainColumnFooting_bY, node_left.Z - fMainColumnFooting_h, 0);
                    m_arrFoundations.Add(new CFoundation(i * 2 + 1,
                        EFoundationType.ePad,
                        node_left,
                        materialConcrete,
                        columnTypePosition,
                        sName,
                        sDescriptionText,
                        //controlPoint_left,
                        fMainColumnFooting_aX,
                        fMainColumnFooting_bY,
                        fMainColumnFooting_h,
                        0,
                        0,
                        fMainColumnFooting_Eccentricity_x,
                        -fMainColumnFooting_Eccentricity_y,
                        90,
                        fConcreteCover,
                        MainColumnFootingReference_Top_Bar_x,
                        MainColumnFootingReference_Top_Bar_y,
                        MainColumnFootingReference_Bottom_Bar_x,
                        MainColumnFootingReference_Bottom_Bar_y,
                        iMainColumnFootingNumberOfBarsTop_x,
                        iMainColumnFootingNumberOfBarsTop_y,
                        iMainColumnFootingNumberOfBarsBottom_x,
                        iMainColumnFootingNumberOfBarsBottom_y,
                        //color,
                        0.5f,
                        true,
                        0));

                    // Right
                    CNode node_right = m_arrNodes[i * iFrameNodesNo + (iFrameNodesNo - 1)];
                    //Point3D controlPoint_right = new Point3D(i * 2 + 2, node_right.X - 0.5f * fMainColumnFooting_aX, node_right.Y - 0.5f * fMainColumnFooting_bY, node_right.Z - fMainColumnFooting_h, 0);
                    m_arrFoundations.Add(new CFoundation(i * 2 + 2,
                        EFoundationType.ePad,
                        node_right,
                        materialConcrete,
                        columnTypePosition,
                        sName,
                        sDescriptionText,
                        //controlPoint_right,
                        fMainColumnFooting_aX,
                        fMainColumnFooting_bY,
                        fMainColumnFooting_h,
                        0,
                        0,
                        -fMainColumnFooting_Eccentricity_x,
                        -fMainColumnFooting_Eccentricity_y,
                        270,
                        fConcreteCover,
                        MainColumnFootingReference_Top_Bar_x,
                        MainColumnFootingReference_Top_Bar_y,
                        MainColumnFootingReference_Bottom_Bar_x,
                        MainColumnFootingReference_Bottom_Bar_y,
                        iMainColumnFootingNumberOfBarsTop_x,
                        iMainColumnFootingNumberOfBarsTop_y,
                        iMainColumnFootingNumberOfBarsBottom_x,
                        iMainColumnFootingNumberOfBarsBottom_y,
                        //color,
                        0.5f,
                        true,
                        0));
                }

                int iLastFoundationIndex = iMainColumnNo;

                // Front and Back Wall Columns - Footings
                if (bGenerateFrontColumns)
                {
                    float fFrontColumnFootingTributaryArea = MathF.Average(fH1_frame, fH2_frame) * fDist_FrontColumns;
                    float fFrontColumnFootingSizeFactor = 0.045f;
                    float fFrontColumnFooting_aX = (float)Math.Round(MathF.Max(0.6f, fFrontColumnFootingTributaryArea * fFrontColumnFootingSizeFactor), 1);
                    float fFrontColumnFooting_bY = (float)Math.Round(MathF.Max(0.7f, fFrontColumnFootingTributaryArea * fFrontColumnFootingSizeFactor), 1);
                    float fFrontColumnFooting_h = 0.45f; // "AS 2870 - Footing pad size must be between 0.45 and 2 [m]" // TODO napojit na tabulku normy

                    float fFrontColumnFooting_Eccentricity_y = 0.5f * (fFrontColumnFooting_bY - (float)m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].h);

                    CReinforcementBar FrontColumnFootingReference_Top_Bar_x;
                    CReinforcementBar FrontColumnFootingReference_Top_Bar_y;
                    CReinforcementBar FrontColumnFootingReference_Bottom_Bar_x;
                    CReinforcementBar FrontColumnFootingReference_Bottom_Bar_y;

                    int iFrontColumnFootingNumberOfBarsTop_x;
                    int iFrontColumnFootingNumberOfBarsTop_y;
                    int iFrontColumnFootingNumberOfBarsBottom_x;
                    int iFrontColumnFootingNumberOfBarsBottom_y;

                    CreateReferenceReinforcementBars(
                        bIsReinforcementBarStraight,
                        fFrontColumnFooting_aX,
                        fFrontColumnFooting_bY,
                        fFrontColumnFooting_h,
                        out FrontColumnFootingReference_Top_Bar_x,
                        out FrontColumnFootingReference_Top_Bar_y,
                        out FrontColumnFootingReference_Bottom_Bar_x,
                        out FrontColumnFootingReference_Bottom_Bar_y,
                        out iFrontColumnFootingNumberOfBarsTop_x,
                        out iFrontColumnFootingNumberOfBarsTop_y,
                        out iFrontColumnFootingNumberOfBarsBottom_x,
                        out iFrontColumnFootingNumberOfBarsBottom_y
                        );

                    EMemberType_FS_Position columnTypePosition = EMemberType_FS_Position.WindPostFrontSide;
                    string sName = "C-FRONT";
                    string sDescriptionText = "PAD TYPE C [WP-FRONT]";

                    // Search footings control points
                    List<CNode> listOfControlPoints = new List<CNode>();
                    for (int i = 0; i < m_arrMembers.Length; i++)
                    {
                        // Find foundation definition nodes
                        if (MathF.d_equal(m_arrMembers[i].NodeStart.Z, 0) &&
                            m_arrMembers[i].EMemberType == EMemberType_FS.eWP &&
                            m_arrMembers[i].CrScStart.Equals(listOfModelMemberGroups[(int)EMemberGroupNames.eFrontWindPost].CrossSection))
                            listOfControlPoints.Add(m_arrMembers[i].NodeStart);
                    }

                    for (int i = 0; i < listOfControlPoints.Count; i++)
                    {
                        //Point3D controlPoint = new Point3D(iLastFoundationIndex + i + 1, listOfControlPoints[i].X - 0.5f * fFrontColumnFooting_aX, listOfControlPoints[i].Y - 0.5f * fFrontColumnFooting_bY, listOfControlPoints[i].Z - fFrontColumnFooting_h, 0);
                        m_arrFoundations.Add(new CFoundation(iLastFoundationIndex + i + 1,
                            EFoundationType.ePad,
                            listOfControlPoints[i],
                            materialConcrete,
                            columnTypePosition,
                            sName,
                            sDescriptionText,
                            //controlPoint,
                            fFrontColumnFooting_aX,
                            fFrontColumnFooting_bY,
                            fFrontColumnFooting_h,
                            0,
                            eccentricityColumnFront_Z.MFz_local,
                            0,
                            -fFrontColumnFooting_Eccentricity_y,
                            180,
                            fConcreteCover,
                            FrontColumnFootingReference_Top_Bar_x,
                            FrontColumnFootingReference_Top_Bar_y,
                            FrontColumnFootingReference_Bottom_Bar_x,
                            FrontColumnFootingReference_Bottom_Bar_y,
                            iFrontColumnFootingNumberOfBarsTop_x,
                            iFrontColumnFootingNumberOfBarsTop_y,
                            iFrontColumnFootingNumberOfBarsBottom_x,
                            iFrontColumnFootingNumberOfBarsBottom_y,
                            //Colors.LightSeaGreen,
                            0.5f,
                            true,
                            0));
                    }

                    iLastFoundationIndex += listOfControlPoints.Count;
                }

                if (bGenerateBackColumns)
                {
                    float fBackColumnFootingTributaryArea = MathF.Average(fH1_frame, fH2_frame) * fDist_BackColumns;
                    float fBackColumnFootingSizeFactor = 0.045f;
                    float fBackColumnFooting_aX = (float)Math.Round(MathF.Max(0.6f, fBackColumnFootingTributaryArea * fBackColumnFootingSizeFactor), 1);
                    float fBackColumnFooting_bY = (float)Math.Round(MathF.Max(0.7f, fBackColumnFootingTributaryArea * fBackColumnFootingSizeFactor), 1);
                    float fBackColumnFooting_h = 0.45f; // "AS 2870 - Footing pad size must be between 0.45 and 2 [m]" // TODO napojit na tabulku normy

                    float fBackColumnFooting_Eccentricity_y = 0.5f * (fBackColumnFooting_bY - (float)m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].h);

                    CReinforcementBar BackColumnFootingReference_Top_Bar_x;
                    CReinforcementBar BackColumnFootingReference_Top_Bar_y;
                    CReinforcementBar BackColumnFootingReference_Bottom_Bar_x;
                    CReinforcementBar BackColumnFootingReference_Bottom_Bar_y;

                    int iBackColumnFootingNumberOfBarsTop_x;
                    int iBackColumnFootingNumberOfBarsTop_y;
                    int iBackColumnFootingNumberOfBarsBottom_x;
                    int iBackColumnFootingNumberOfBarsBottom_y;

                    CreateReferenceReinforcementBars(
                        bIsReinforcementBarStraight,
                        fBackColumnFooting_aX,
                        fBackColumnFooting_bY,
                        fBackColumnFooting_h,
                        out BackColumnFootingReference_Top_Bar_x,
                        out BackColumnFootingReference_Top_Bar_y,
                        out BackColumnFootingReference_Bottom_Bar_x,
                        out BackColumnFootingReference_Bottom_Bar_y,
                        out iBackColumnFootingNumberOfBarsTop_x,
                        out iBackColumnFootingNumberOfBarsTop_y,
                        out iBackColumnFootingNumberOfBarsBottom_x,
                        out iBackColumnFootingNumberOfBarsBottom_y
                        );

                    EMemberType_FS_Position columnTypePosition = EMemberType_FS_Position.WindPostBackSide;
                    string sName = "C-BACK";
                    string sDescriptionText = "PAD TYPE C [WP-BACK]";

                    // Search footings control points
                    List<CNode> listOfControlPoints = new List<CNode>();
                    for (int i = 0; i < m_arrMembers.Length; i++)
                    {
                        // Find foundation definition nodes
                        if (MathF.d_equal(m_arrMembers[i].NodeStart.Z, 0) &&
                            m_arrMembers[i].EMemberType == EMemberType_FS.eWP &&
                            m_arrMembers[i].CrScStart.Equals(listOfModelMemberGroups[(int)EMemberGroupNames.eBackWindPost].CrossSection))
                            listOfControlPoints.Add(m_arrMembers[i].NodeStart);
                    }

                    for (int i = 0; i < listOfControlPoints.Count; i++)
                    {
                        //Point3D controlPoint = new Point3D(iLastFoundationIndex + i + 1, listOfControlPoints[i].X - 0.5f * fBackColumnFooting_aX, listOfControlPoints[i].Y - 0.5f * fBackColumnFooting_bY, listOfControlPoints[i].Z - fBackColumnFooting_h, 0);
                        m_arrFoundations.Add(new CFoundation(iLastFoundationIndex + i + 1,
                            EFoundationType.ePad,
                            listOfControlPoints[i],
                            materialConcrete,
                            columnTypePosition,
                            sName,
                            sDescriptionText,
                            //controlPoint,
                            fBackColumnFooting_aX,
                            fBackColumnFooting_bY,
                            fBackColumnFooting_h,
                            0,
                            eccentricityColumnBack_Z.MFz_local,
                            0,
                            fBackColumnFooting_Eccentricity_y,
                            180,
                            fConcreteCover,
                            BackColumnFootingReference_Top_Bar_x,
                            BackColumnFootingReference_Top_Bar_y,
                            BackColumnFootingReference_Bottom_Bar_x,
                            BackColumnFootingReference_Bottom_Bar_y,
                            iBackColumnFootingNumberOfBarsTop_x,
                            iBackColumnFootingNumberOfBarsTop_y,
                            iBackColumnFootingNumberOfBarsBottom_x,
                            iBackColumnFootingNumberOfBarsBottom_y,
                            //Colors.Coral,
                            0.5f,
                            true,
                            0));
                    }

                    iLastFoundationIndex += listOfControlPoints.Count;
                }

                // Validation - skontroluje ci je velkost pola zhodna s poctom vygenerovanych prvkov
                if (m_arrFoundations.Count != iLastFoundationIndex)
                    throw new Exception("Incorrect number of generated foundations");
            }
        }

        protected void CreateFloorSlab(bool bGenerateFrontColumns, bool bGenerateBackColumns, bool bGenerateFrontGirts, bool bGenerateBackGirts, bool bWindPostUnderRafter)
        {
            bool bGenerateSlabs = true;

            if (bGenerateSlabs)
            {
                // Docasne
                CMat_02_00 materialConcrete = new CMat_02_00();
                materialConcrete.Name = "25";
                materialConcrete.Fck = 25e+6f;
                materialConcrete.m_fRho = 2300f;
                materialConcrete.m_fE = 30e+9f;

                // Ground Floor Slab
                float fFloorSlab_AdditionalOffset_X = 0.005f; // Rozmer o ktory doska presahuje od hrany stlpa
                float fFloorSlab_AdditionalOffset_Y = 0.005f; // Rozmer o ktory doska presahuje od hrany stlpa

                float fFloorSlabOffset_x = -(float)m_arrCrSc[0].z_max - fFloorSlab_AdditionalOffset_X;
                float fFloorSlabOffset_y_Front = (float)m_arrCrSc[0].y_min - fFloorSlab_AdditionalOffset_Y;
                float fFloorSlabOffset_y_Back = (float)m_arrCrSc[0].y_max + fFloorSlab_AdditionalOffset_Y;

                float fFloorSlab_aX = fW_frame + 2 * (-fFloorSlabOffset_x);
                float fFloorSlab_bY = fL_tot + (-fFloorSlabOffset_y_Front) + fFloorSlabOffset_y_Back;
                float fFloorSlab_h = 0.125f;
                float fFloorSlab_eX = fFloorSlabOffset_x;
                float fFloorSlab_eY = fFloorSlabOffset_y_Front;

                float fConcreteCoverTop = 0.05f; // 50 mm
                string sMeshGradeName = "SE92DE";

                CReinforcementMesh mesh = new CReinforcementMesh(sMeshGradeName);

                // Saw Cuts
                CSawCut refSawCut = new CSawCut(0, new Point3D(0, 0, 0), new Point3D(1, 0, 0), 0.01f, 0.03f, true, 0);

                // Create raster of lines in XY-plane
                float fFirstSawCutPositionInDirectionX = 0;
                float fFirstSawCutPositionInDirectionY = 0;
                int iNumberOfSawCutsInDirectionX = 0;
                int iNumberOfSawCutsInDirectionY = 0;
                float fSawCutsSpacingInDirectionX = 0;
                float fSawCutsSpacingInDirectionY = 0;

                float fSawCutMaximumDistanceY = Math.Min(6, fL1_frame); // 6 m // V kazdej bay alebo maximalne 6 metrov od seba
                float fSawCutMaximumDistanceX = 0.5f * fSawCutMaximumDistanceY; // Pomer Y : X = 2:1

                if (fSawCutMaximumDistanceY < fFloorSlab_bY)
                {
                    iNumberOfSawCutsInDirectionY = (int)(fFloorSlab_bY / fSawCutMaximumDistanceY);
                    // Predpoklada sa, ze posledny saw cut je rovnako vzdialeny od konca ako prvy od zaciatku
                    fSawCutsSpacingInDirectionY = fFloorSlab_bY / (iNumberOfSawCutsInDirectionY);
                    fFirstSawCutPositionInDirectionY = fSawCutsSpacingInDirectionY / 2f;
                }

                if (fSawCutMaximumDistanceX < fFloorSlab_aX)
                {
                    iNumberOfSawCutsInDirectionX = (int)(fFloorSlab_aX / fSawCutMaximumDistanceX);
                    // Predpoklada sa, ze posledny saw cut je rovnako vzdialeny od konca ako prvy od zaciatku
                    fSawCutsSpacingInDirectionX = fFloorSlab_aX / (iNumberOfSawCutsInDirectionX);
                    fFirstSawCutPositionInDirectionX = fSawCutsSpacingInDirectionX / 2f;
                }

                // ControlJoints
                CDowel refDowel = new CDowel(new Point3D(0, 0, 0), 0.016f, 0.6f, 0.947f);
                CControlJoint refControlJoint = new CControlJoint(0, new Point3D(0, 0, 0), new Point3D(1, 0, 0), refDowel, 0.4f, true, 0);
                // Create raster of lines in XY-plane
                float fControlJointMaximumDistance = Math.Min(24, 0.5f * fFloorSlab_bY); // 24 m alebo polovica budovy (vzdy aspon jeden joint v strede)

                float fFirstControlJointPositionInDirectionX = 0;
                float fFirstControlJointPositionInDirectionY = 0;
                int iNumberOfControlJointsInDirectionX = 0;
                int iNumberOfControlJointsInDirectionY = 0;
                float fControlJointsSpacingInDirectionX = 0;
                float fControlJointsSpacingInDirectionY = 0;

                if (fControlJointMaximumDistance < fFloorSlab_bY)
                {
                    iNumberOfControlJointsInDirectionY = (int)(fFloorSlab_bY / fControlJointMaximumDistance);
                    // Predpoklada sa, ze posledny control joint je rovnako vzdialeny od konca ako prvy od zaciatku
                    fControlJointsSpacingInDirectionY = fFloorSlab_bY / (iNumberOfControlJointsInDirectionY);
                    fFirstControlJointPositionInDirectionY = fControlJointsSpacingInDirectionY / 2f;
                }

                if (fControlJointMaximumDistance < fFloorSlab_aX)
                {
                    iNumberOfControlJointsInDirectionX = (int)(fFloorSlab_aX / fControlJointMaximumDistance);
                    // Predpoklada sa, ze posledny control joint je rovnako vzdialeny od konca ako prvy od zaciatku
                    fControlJointsSpacingInDirectionX = fFloorSlab_aX / (iNumberOfControlJointsInDirectionX);
                    fFirstControlJointPositionInDirectionX = fControlJointsSpacingInDirectionX / 2f;
                }

                float fPerimeterDepth_LRSide = 0.45f; // "AS 2870 - Size must be between 0.45 and 2 [m]"; // TODO napojit na tabulku normy
                float fPerimeterWidth_LRSide = 0.25f;
                float fStartersLapLength_LRSide = 0.6f;
                float fStartersSpacing_LRSide = 0.6f;
                float fStarters_Phi_LRSide = 0.012f;
                float fLongitud_Reinf_TopAndBotom_Phi_LRSide = 0.016f;
                float fLongitud_Reinf_Intermediate_Phi_LRSide = 0.012f;
                int fLongitud_Reinf_Intermediate_Count_LRSide = 1;

                float fRebateWidth_LRSide = 0.5f;

                float fPerimeterDepth_FBSide = 0.45f; // "AS 2870 - Size must be between 0.45 and 2 [m]"; // TODO napojit na tabulku normy
                float fPerimeterWidth_FBSide = 0.25f;
                float fStartersLapLength_FBSide = 0.6f;
                float fStartersSpacing_FBSide = 0.6f;
                float fStarters_Phi_FBSide = 0.012f;
                float fLongitud_Reinf_TopAndBotom_Phi_FBSide = 0.016f;
                float fLongitud_Reinf_Intermediate_Phi_FBSide = 0.012f;
                int fLongitud_Reinf_Intermediate_Count_FBSide = 1;

                float fRebateWidth_FBSide = 0.5f;

                m_arrSlabs = new List<CSlab>();
                m_arrSlabs.Add(new CSlab(1,
                            materialConcrete,
                            fFloorSlab_aX,
                            fFloorSlab_bY,
                            fFloorSlab_h,
                            fFloorSlab_eX,
                            fFloorSlab_eY, 0,
                            fConcreteCoverTop,
                            sMeshGradeName,
                            iNumberOfSawCutsInDirectionX,
                            iNumberOfSawCutsInDirectionY,
                            fFirstSawCutPositionInDirectionX,
                            fFirstSawCutPositionInDirectionY,
                            fSawCutsSpacingInDirectionX,
                            fSawCutsSpacingInDirectionY,
                            refSawCut,
                            iNumberOfControlJointsInDirectionX,
                            iNumberOfControlJointsInDirectionY,
                            fFirstControlJointPositionInDirectionX,
                            fFirstControlJointPositionInDirectionY,
                            fControlJointsSpacingInDirectionX,
                            fControlJointsSpacingInDirectionY,
                            refControlJoint,
                            fPerimeterDepth_LRSide,
                            fPerimeterWidth_LRSide,
                            fStartersLapLength_LRSide,
                            fStartersSpacing_LRSide,
                            fStarters_Phi_LRSide,
                            fLongitud_Reinf_TopAndBotom_Phi_LRSide,
                            fLongitud_Reinf_Intermediate_Phi_LRSide,
                            fLongitud_Reinf_Intermediate_Count_LRSide,
                            fRebateWidth_LRSide,
                            fPerimeterDepth_FBSide,
                            fPerimeterWidth_FBSide,
                            fStartersLapLength_FBSide,
                            fStartersSpacing_FBSide,
                            fStarters_Phi_FBSide,
                            fLongitud_Reinf_TopAndBotom_Phi_FBSide,
                            fLongitud_Reinf_Intermediate_Phi_FBSide,
                            fLongitud_Reinf_Intermediate_Count_FBSide,
                            fRebateWidth_FBSide,
                            DoorBlocksProperties,

                            // Temp
                            // TODO Ondrej - tieto parametre sa mi nepacia, sluzia na vypocet polohy zaciatku rebate a dlzky rebate, malo by to podla mna prist do CSlab uz nejako v ramci door properties
                            //fL1_frame, // Vzdialenost ramov
                            //fDist_FrontColumns, // Vzdialenost wind posts (stlpov v prednej stene)
                            //fDist_BackColumns, // Vzdialenost wind posts (stlpov v zadnej stene)
                            //0.14f, // Sirka cross-section typu roller door trimmer

                            //BackColumnFootingReference_Top_Bar_x,
                            //BackColumnFootingReference_Top_Bar_y,
                            //BackColumnFootingReference_Bottom_Bar_x,
                            //BackColumnFootingReference_Bottom_Bar_y,
                            //iBackColumnFootingNumberOfBarsTop_x,
                            //iBackColumnFootingNumberOfBarsTop_y,
                            //iBackColumnFootingNumberOfBarsBottom_x,
                            //iBackColumnFootingNumberOfBarsBottom_y,
                            //Colors.LightGray,
                            0.3f,
                            true,
                            0));
            }
        }

        protected int GetDefaultNumberOfReinforcingBars(float footingPadWidth, float fBarDiameter, float fConcreteCover)
        {
            float fDefaultDistanceBetweenReinforcementBars = 0.15f; // 150 mm

            // Number of spacings + 1
            return (int)((footingPadWidth - 2 * fConcreteCover - 3 * fBarDiameter) / fDefaultDistanceBetweenReinforcementBars) + 1;
        }

        protected void CreateReferenceReinforcementBars(
            bool bIsReinforcementBarStraight,
            float faX,
            float fbY,
            float fhZ,
            out CReinforcementBar reference_Top_Bar_x,
            out CReinforcementBar reference_Top_Bar_y,
            out CReinforcementBar reference_Bottom_Bar_x,
            out CReinforcementBar reference_Bottom_Bar_y,
            out int iNumberOfBarsTop_x,
            out int iNumberOfBarsTop_y,
            out int iNumberOfBarsBottom_x,
            out int iNumberOfBarsBottom_y,
            float fDiameterTop_Bar_x = 0.012f,
            float fDiameterTop_Bar_y = 0.012f,
            float fDiameterBottom_Bar_x = 0.012f,
            float fDiameterBottom_Bar_y = 0.012f,
            float fConcreteCover = 0.075f
            )
        {
            // For each pad recalculate lengths of reference bars
            float fLengthTop_Bar_x = bIsReinforcementBarStraight ? faX - 2 * fConcreteCover : faX - 2 * fConcreteCover - fDiameterTop_Bar_x;
            float fLengthBottom_Bar_x = bIsReinforcementBarStraight ? faX - 2 * fConcreteCover : faX - 2 * fConcreteCover - fDiameterBottom_Bar_x;
            float fLengthTop_Bar_y = bIsReinforcementBarStraight ? fbY - 2 * fConcreteCover : fbY - 2 * fConcreteCover - fDiameterTop_Bar_y;
            float fLengthBottom_Bar_y = bIsReinforcementBarStraight ? fbY - 2 * fConcreteCover : fbY - 2 * fConcreteCover - fDiameterBottom_Bar_y;

            iNumberOfBarsTop_x = GetDefaultNumberOfReinforcingBars(fbY, fDiameterTop_Bar_x, fConcreteCover);
            iNumberOfBarsTop_y = GetDefaultNumberOfReinforcingBars(faX, fDiameterTop_Bar_y, fConcreteCover);
            iNumberOfBarsBottom_x = GetDefaultNumberOfReinforcingBars(fbY, fDiameterBottom_Bar_x, fConcreteCover);
            iNumberOfBarsBottom_y = GetDefaultNumberOfReinforcingBars(faX, fDiameterBottom_Bar_y, fConcreteCover);

            // Reference / first bar coordinates
            double cp_Top_x_coordX = bIsReinforcementBarStraight ? fConcreteCover : fConcreteCover + 0.5f * fDiameterTop_Bar_x;
            double cp_Top_x_coordY = bIsReinforcementBarStraight ? fConcreteCover + 0.5f * fDiameterTop_Bar_x : fConcreteCover + fDiameterTop_Bar_y + 0.5f * fDiameterTop_Bar_x;
            double cp_Top_y_coordX = bIsReinforcementBarStraight ? fConcreteCover + 0.5f * fDiameterTop_Bar_y : fConcreteCover + fDiameterTop_Bar_x + 0.5f * fDiameterTop_Bar_y;
            double cp_Top_y_coordY = bIsReinforcementBarStraight ? fConcreteCover : fConcreteCover + 0.5f * fDiameterTop_Bar_y;
            double cp_Bottom_x_coordX = bIsReinforcementBarStraight ? fConcreteCover : fConcreteCover + 0.5f * fDiameterBottom_Bar_x;
            double cp_Bottom_x_coordY = bIsReinforcementBarStraight ? fConcreteCover + 0.5f * fDiameterBottom_Bar_x : fConcreteCover + fDiameterBottom_Bar_y + 0.5f * fDiameterBottom_Bar_x;
            double cp_Bottom_y_coordX = bIsReinforcementBarStraight ? fConcreteCover + 0.5f * fDiameterBottom_Bar_y : fConcreteCover + fDiameterBottom_Bar_x + 0.5f * fDiameterBottom_Bar_y;
            double cp_Bottom_y_coordY = bIsReinforcementBarStraight ? fConcreteCover : fConcreteCover + 0.5f * fDiameterBottom_Bar_y;

            Point3D cp_Top_x = new Point3D(cp_Top_x_coordX, cp_Top_x_coordY, fhZ - fConcreteCover - fDiameterTop_Bar_y - 0.5f * fDiameterTop_Bar_x);
            Point3D cp_Top_y = new Point3D(cp_Top_y_coordX, cp_Top_y_coordY, fhZ - fConcreteCover - 0.5f * fDiameterTop_Bar_y);
            Point3D cp_Bottom_x = new Point3D(cp_Bottom_x_coordX, cp_Bottom_x_coordY, fConcreteCover + fDiameterBottom_Bar_y + 0.5f * fDiameterBottom_Bar_x);
            Point3D cp_Bottom_y = new Point3D(cp_Bottom_y_coordX, cp_Bottom_y_coordY, fConcreteCover + 0.5f * fDiameterBottom_Bar_y);

            if (!bIsReinforcementBarStraight)
            {
                cp_Top_x = new Point3D(cp_Top_x_coordX, cp_Top_x_coordY, fConcreteCover + fDiameterTop_Bar_y);
                cp_Top_y = new Point3D(cp_Top_y_coordX, cp_Top_y_coordY, fConcreteCover);

                // Kedze sa vertikalne casti hornych a spodnych prutov prekryvaju posunieme horne pruty o sucet polovic priemeru
                cp_Top_x.Y = cp_Top_x_coordY + 0.5 * fDiameterTop_Bar_x + 0.5 * fDiameterBottom_Bar_x;
                cp_Top_y.X = cp_Top_y_coordX + 0.5 * fDiameterTop_Bar_y + 0.5 * fDiameterBottom_Bar_y;

                cp_Bottom_x = new Point3D(cp_Bottom_x_coordX, cp_Bottom_x_coordY, fhZ - fConcreteCover - fDiameterBottom_Bar_y);
                cp_Bottom_y = new Point3D(cp_Bottom_y_coordX, cp_Bottom_y_coordY, fhZ - fConcreteCover);
            }

            if (bIsReinforcementBarStraight)
            {
                reference_Top_Bar_x = new CReinforcementBarStraight(1, "500E", "Top x", true, cp_Top_x, fLengthTop_Bar_x, fDiameterTop_Bar_x, /*Colors.CadetBlue,*/ 0.5f, true, 0);
                reference_Top_Bar_y = new CReinforcementBarStraight(2, "500E", "Top y", false, cp_Top_y, fLengthTop_Bar_y, fDiameterTop_Bar_y, /*Colors.Coral,*/ 0.5f, true, 0);
                reference_Bottom_Bar_x = new CReinforcementBarStraight(3, "500E", "Bottom x", true, cp_Bottom_x, fLengthBottom_Bar_x, fDiameterBottom_Bar_x, /*Colors.YellowGreen,*/ 0.5f, true, 0);
                reference_Bottom_Bar_y = new CReinforcementBarStraight(4, "500E", "Bottom y", false, cp_Bottom_y, fLengthBottom_Bar_y, fDiameterBottom_Bar_y, /*Colors.Purple,*/ 0.5f, true, 0);
            }
            else
            {
                float fArcRadiusNetTop_Bar_x = 3f * fDiameterTop_Bar_x;
                float fArcRadiusNetTop_Bar_y = 3f * fDiameterTop_Bar_y;
                float fArcRadiusNetBottom_Bar_x = 3f * fDiameterBottom_Bar_x;
                float fArcRadiusNetBottom_Bar_y = 3f * fDiameterBottom_Bar_y;

                reference_Top_Bar_x = new CReinforcementBar_U(1, "500E", "Top x", true, cp_Top_x, fLengthTop_Bar_x, fArcRadiusNetTop_Bar_x, fDiameterTop_Bar_x, /*Colors.CadetBlue,*/ 0.5f, true, true, 0);
                reference_Top_Bar_y = new CReinforcementBar_U(2, "500E", "Top y", false, cp_Top_y, fLengthTop_Bar_y, fArcRadiusNetTop_Bar_y, fDiameterTop_Bar_y, /*Colors.Coral,*/ 0.5f, true, true, 0);
                reference_Bottom_Bar_x = new CReinforcementBar_U(3, "500E", "Bottom x", true, cp_Bottom_x, fLengthBottom_Bar_x, fArcRadiusNetBottom_Bar_x, fDiameterBottom_Bar_x, /*Colors.YellowGreen,*/ 0.5f, false, true, 0);
                reference_Bottom_Bar_y = new CReinforcementBar_U(4, "500E", "Bottom y", false, cp_Bottom_y, fLengthBottom_Bar_y, fArcRadiusNetBottom_Bar_y, fDiameterBottom_Bar_y, /*Colors.Purple,*/ 0.5f, false, true, 0);
            }
        }

        protected void CountPlates_ValidationPurpose(bool bIsDebbuging)
        {
            if (bIsDebbuging)
            {
                int iNumberOfJoints = 0;
                int iNumberOfJointsGenerateTrue = 0;
                int iNumberOfJointsGenerateFalse = 0;

                int iNumberOfPlates = 0;
                int iNumberOfPlateGenerateTrue = 0;
                int iNumberOfPlateGenerateFalse = 0;
                int iNumberOfPlateMatListTrue = 0;
                int iNumberOfPlateMatlistFalse = 0;

                foreach (CConnectionJointTypes joint in m_arrConnectionJoints)
                {
                    iNumberOfJoints++;
                    if (joint.BIsGenerated) iNumberOfJointsGenerateTrue++; else iNumberOfJointsGenerateFalse++;

                    foreach (CPlate plate in joint.m_arrPlates)
                    {
                        iNumberOfPlates++;

                        if (plate.BIsGenerated) iNumberOfPlateGenerateTrue++; else iNumberOfPlateGenerateFalse++;
                        if (plate.BIsSelectedForMaterialList) iNumberOfPlateMatListTrue++; else iNumberOfPlateMatlistFalse++;

                        if (plate is CConCom_Plate_B_basic)
                        {
                            CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

                            foreach (CAnchor anchor in basePlate.AnchorArrangement.Anchors)
                            {
                                iNumberOfPlates++; // anchor.WasherBearing
                                iNumberOfPlates++; // anchor.WasherPlateTop

                                if (anchor.WasherBearing.BIsGenerated) iNumberOfPlateGenerateTrue++; else iNumberOfPlateGenerateFalse++;
                                if (anchor.WasherBearing.BIsSelectedForMaterialList) iNumberOfPlateMatListTrue++; else iNumberOfPlateMatlistFalse++;

                                if (anchor.WasherPlateTop.BIsGenerated) iNumberOfPlateGenerateTrue++; else iNumberOfPlateGenerateFalse++;
                                if (anchor.WasherPlateTop.BIsSelectedForMaterialList) iNumberOfPlateMatListTrue++; else iNumberOfPlateMatlistFalse++;
                            }
                        }
                    }
                }

                System.Diagnostics.Trace.WriteLine(
                    "Total number of joints: " + iNumberOfJoints.ToString() + "\n" +
                    "Number of joints - Generate - True: " + iNumberOfJointsGenerateTrue.ToString() + "\n" +
                    "Number of joints - Generate - False: " + iNumberOfJointsGenerateFalse.ToString() + "\n" +
                    "\n" +
                    "Total number of plates: " + iNumberOfPlates.ToString() + "\n" +
                    "Number of plates - Generate - True: " + iNumberOfPlateGenerateTrue.ToString() + "\n" +
                    "Number of plates - Generate - False: " + iNumberOfPlateGenerateFalse.ToString() + "\n" +
                    "Number of plates - Material List - True: " + iNumberOfPlateMatListTrue.ToString() + "\n" +
                    "Number of plates - Material List - False: " + iNumberOfPlateMatlistFalse.ToString() + "\n");
            }
        }

        public void GetJointalignments(float fh_column, float fh_rafter, out float alignment_column, out float alignment_knee_rafter, out float alignment_apex_rafter)
        {
            float cosAlpha = (float)Math.Cos(Math.Abs(fRoofPitch_rad));
            float sinAlpha = (float)Math.Sin(Math.Abs(fRoofPitch_rad));
            float tanAlpha = (float)Math.Tan(Math.Abs(fRoofPitch_rad));

            /*
            float y = fh_rafter / cosAlpha;
            float a = sinAlpha * 0.5f * y;
            float x = cosAlpha * 2f * a;
            float x2 = 0.5f * fh_column - x;
            float y2 = tanAlpha * x2;
            alignment_column = 0.5f * y + y2;

            float x3 = 0.5f * x;
            float x4 = 0.5f * fh_column - x3;
            alignment_knee_rafter = x4 / cosAlpha;

            alignment_apex_rafter = a;
            */

            float y = fh_rafter / cosAlpha;
            alignment_apex_rafter = sinAlpha * 0.5f * y;
            float x = cosAlpha * 2f * alignment_apex_rafter;
            alignment_column = 0.5f * y + (tanAlpha * (0.5f * fh_column - x));
            alignment_knee_rafter = (0.5f * fh_column - (0.5f * x)) / cosAlpha;
        }

        public void AddDoorBlock(DoorProperties prop, int iSideWallLeftColumnGirtNoInOneFrame, int iSideWallRightColumnGirtNoInOneFrame, float fLimitDistanceFromColumn, float fSideWallHeight, bool addJoints)
        {
            CMember mReferenceGirt;
            CMember mColumnLeft;
            CMember mColumnRight;
            CMember mEavesPurlin;
            CMember mEdgeRafter;
            CBlock_3D_001_DoorInBay door;
            Point3D pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fSideWallHeight; //fH1_frame; // TODO - spocitat vysku bay v mieste bloku (pre front a back budu dve vysky v mieste vlozenia stlpov bloku
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(prop.sBuildingSide, prop.iBayNumber, iSideWallLeftColumnGirtNoInOneFrame, iSideWallRightColumnGirtNoInOneFrame, out mReferenceGirt, out mColumnLeft, out mColumnRight, out mEavesPurlin, out mEdgeRafter, out pControlPointBlock, out fBayWidth, out iFirstMemberToDeactivate, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            // Set girt to connect columns / trimmers
            int iNumberOfGirtsToDeactivate = (int)((prop.fDoorsHeight - fBottomGirtPosition) / fDist_Girt) + 1; // Number of intermediate girts + Bottom Girt (prevzate z CBlock_3D_001_DoorInBay)
            CMember mGirtToConnectDoorTrimmers = m_arrMembers[(mReferenceGirt.ID - 1) + iNumberOfGirtsToDeactivate]; // Toto je girt, ku ktoremu sa pripoja stlpy dveri (len v pripade ze sa nepripoja k eave purlin alebo edge rafter) - 1 -index reference girt

            door = new CBlock_3D_001_DoorInBay(
                prop,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                fDist_FrontColumns,
                fDist_BackColumns,
                fH1_frame,
                fW_frame,
                fRoofPitch_rad,
                eKitset,
                mReferenceGirt,
                mGirtToConnectDoorTrimmers,
                mColumnLeft,
                mColumnRight,
                mEavesPurlin,
                mEdgeRafter,
                fBayWidth,
                fBayHeight,
                fUpperGirtLimit,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, door, addJoints);

            DoorsModels.Add(door);
        }

        public void AddWindowBlock(WindowProperties prop, int iSideWallLeftColumnGirtNoInOneFrame, int iSideWallRightColumnGirtNoInOneFrame, float fLimitDistanceFromColumn, float fSideWallHeight, bool addJoints)
        {
            CMember mReferenceGirt;
            CMember mColumnLeft;
            CMember mColumnRight;
            CMember mEavesPurlin;
            CMember mEdgeRafter;
            CBlock_3D_002_WindowInBay window;
            Point3D pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fSideWallHeight; //fH1_frame; // TODO - spocitat vysku bay v mieste bloku (pre front a back budu dve vysky v mieste vlozenia stlpov bloku
            int iFirstGirtInBay;
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(prop.sBuildingSide, prop.iBayNumber, iSideWallLeftColumnGirtNoInOneFrame, iSideWallRightColumnGirtNoInOneFrame, out mReferenceGirt, out mColumnLeft, out mColumnRight, out mEavesPurlin, out mEdgeRafter, out pControlPointBlock, out fBayWidth, out iFirstGirtInBay, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            // Prevzate z CBlock_3D_002_WindowInBay
            int iNumberOfGirtsUnderWindow = (int)((prop.fWindowCoordinateZinBay - fBottomGirtPosition) / fDist_Girt) + 1;
            float fCoordinateZOfGirtUnderWindow = (iNumberOfGirtsUnderWindow - 1) * fDist_Girt + fBottomGirtPosition;

            if (prop.fWindowCoordinateZinBay <= fBottomGirtPosition)
            {
                iNumberOfGirtsUnderWindow = 0;
                fCoordinateZOfGirtUnderWindow = 0f;
            }

            int iNumberOfGirtsToDeactivate = (int)((prop.fWindowsHeight + prop.fWindowCoordinateZinBay - fCoordinateZOfGirtUnderWindow) / fDist_Girt); // Number of intermediate girts to deactivate

            CMember mGirtToConnectWindowColumns_Bottom = null;

            if (iNumberOfGirtsUnderWindow > 0)
                mGirtToConnectWindowColumns_Bottom = m_arrMembers[(mReferenceGirt.ID - 1) + (iNumberOfGirtsUnderWindow - 1)]; // Toto je girt, ku ktoremu sa pripoja stlpiky okna v dolnej casti

            CMember mGirtToConnectWindowColumns_Top = m_arrMembers[(mReferenceGirt.ID - 1) + (iNumberOfGirtsUnderWindow - 1) + iNumberOfGirtsToDeactivate + 1]; // Toto je girt, ku ktoremu sa pripoja stlpiky okna v hornej casti (len v pripade ze sa nepripoja k eave purlin alebo edge rafter)

            window = new CBlock_3D_002_WindowInBay(
                prop,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                fDist_FrontColumns,
                fDist_BackColumns,
                fH1_frame,
                fW_frame,
                fRoofPitch_rad,
                eKitset,
                mReferenceGirt,
                mGirtToConnectWindowColumns_Bottom,
                mGirtToConnectWindowColumns_Top,
                mColumnLeft,
                mColumnRight,
                mEavesPurlin,
                mEdgeRafter,
                fBayWidth,
                fBayHeight,
                fUpperGirtLimit,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            iFirstMemberToDeactivate = iFirstGirtInBay + window.iNumberOfGirtsUnderWindow;

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, window, addJoints);

            WindowsModels.Add(window);
        }

        public virtual void DeterminateBasicPropertiesToInsertBlock(
            string sBuildingSide,                     // Identification of building side (left, right, front, back)
            int iBayNumber,                           // Bay number (1-n) in positive X or Y direction
            int iSideWallLeftColumnGirtNoInOneFrame,  // Number of girts in the left side wall per one column (one frame)
            int iSideWallRightColumnGirtNoInOneFrame, // Number of girts in the right side wall per one column (one frame)
            out CMember mReferenceGirt,               // Reference girt - first girts that needs to be deactivated and replaced by new member (some parameters are same for deactivated and new member)
            out CMember mColumnLeft,                  // Left column of bay
            out CMember mColumnRight,                 // Right column of bay
            out CMember mEavesPurlin,                 // Eave purlin for left and right side
            out CMember mEdgeRafter,                  // Edge rafter - reference for front and back side (trimmer to rafter connection)
            out Point3D pControlPointBlock,           // Conctrol point to insert block - defined as left column base point
            out float fBayWidth,                      // Width of bay (distance between bay columns)
            out int iFirstMemberToDeactivate,         // Index of first girt in the bay which is in collision with the block and must be deactivated
            out bool bIsReverseSession,               // Front or back wall bay can have reverse direction of girts in X
            out bool bIsFirstBayInFrontorBackSide,
            out bool bIsLastBayInFrontorBackSide
            )
        {
            bIsReverseSession = false;            // Set to true value just for front or back wall (right part of wall)
            bIsFirstBayInFrontorBackSide = false; // Set to true value just for front or back wall (first bay)
            bIsLastBayInFrontorBackSide = false;  // Set to true value just for front or back wall (last bay)

            if (sBuildingSide == "Left" || sBuildingSide == "Right")
            {
                // Left side X = 0, Right Side X = GableWidth
                // Insert after frame ID
                int iSideMultiplier = sBuildingSide == "Left" ? 0 : 1; // 0 left side X = 0, 1 - right side X = Gable Width
                int iBlockFrame = iBayNumber - 1; // ID of frame in the bay, starts with zero

                int iBayColumnLeft = (iBlockFrame * (iFrameMembersNo + iEavesPurlinNoInOneFrame)) + (iSideMultiplier == 0 ? 0 : (iFrameMembersNo - 1)); // (2 columns + 2 rafters + 2 eaves purlins) = 6, For Y = GableWidth + 4 number of members in one frame - 1 (index)
                int iBayColumnRight = ((iBlockFrame + 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame)) + (iSideMultiplier == 0 ? 0 : (iFrameMembersNo - 1));
                fBayWidth = fL1_frame;

                int iLeftGirtNoInOneFrame = 0;
                if (sBuildingSide == "Right") iLeftGirtNoInOneFrame = iSideWallLeftColumnGirtNoInOneFrame;

                iFirstMemberToDeactivate = iMainColumnNo + iRafterNo + iEavesPurlinNo + iBlockFrame * iGirtNoInOneFrame + iLeftGirtNoInOneFrame /*iSideMultiplier * (iGirtNoInOneFrame / 2)*/;

                mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
                mColumnLeft = m_arrMembers[iBayColumnLeft];
                mColumnRight = m_arrMembers[iBayColumnRight];

                if (sBuildingSide == "Left")
                    mEavesPurlin = m_arrMembers[(iBlockFrame * iEavesPurlinNoInOneFrame) + iBlockFrame * (iFrameNodesNo - 1) + iFrameMembersNo];
                else
                    mEavesPurlin = m_arrMembers[(iBlockFrame * iEavesPurlinNoInOneFrame) + iBlockFrame * (iFrameNodesNo - 1) + (iFrameMembersNo + 1)];

                mEdgeRafter = null; // Not defined for the left and right side
            }
            else // Front or Back Side
            {
                // Insert after sequence ID
                int iNumberOfIntermediateColumns;
                int[] iArrayOfGirtsPerColumnCount;
                //int iNumberOfGirtsInWall;

                if (sBuildingSide == "Front")  // Front side properties
                {
                    iNumberOfIntermediateColumns = iFrontColumnNoInOneFrame;
                    iArrayOfGirtsPerColumnCount = iArrNumberOfGirtsPerFrontColumnFromLeft; //iArrNumberOfNodesPerFrontColumnFromLeft;
                    //iNumberOfGirtsInWall = iFrontGirtsNoInOneFrame;
                    fBayWidth = fDist_FrontColumns;
                }
                else // Back side properties
                {
                    iNumberOfIntermediateColumns = iBackColumnNoInOneFrame;
                    iArrayOfGirtsPerColumnCount = iArrNumberOfGirtsPerBackColumnFromLeft; //iArrNumberOfNodesPerBackColumnFromLeft;
                    //iNumberOfGirtsInWall = iBackGirtsNoInOneFrame;
                    fBayWidth = fDist_BackColumns;
                }

                int iSideMultiplier = sBuildingSide == "Front" ? 0 : 1; // 0 front side Y = 0, 1 - back side Y = Length
                int iBlockSequence = iBayNumber - 1; // ID of sequence, starts with zero
                int iColumnNumberLeft;
                int iColumnNumberRight;
                int iRafterNumber;
                int iNumberOfFirstGirtInWallToDeactivate = 0;
                int iNumberOfMembers_tempForGirts = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iSideMultiplier * iFrontGirtsNoInOneFrame;
                int iNumberOfMembers_tempForColumns = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iSideMultiplier * iFrontColumnNoInOneFrame;

                if (iBlockSequence == 0) // Main Column - first bay
                {
                    if (sBuildingSide == "Front")
                    {
                        iColumnNumberLeft = 0;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + iBlockSequence;
                        iRafterNumber = iColumnNumberLeft + 1;
                    }
                    else
                    {
                        iColumnNumberLeft = (iFrameNo - 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame);
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + iBlockSequence;
                        iRafterNumber = iColumnNumberLeft + 1;
                    }

                    iFirstMemberToDeactivate = iNumberOfMembers_tempForGirts + iNumberOfFirstGirtInWallToDeactivate;

                    bIsFirstBayInFrontorBackSide = true; // First bay
                }
                else
                {
                    bool bIsGable = eKitset == EModelType_FS.eKitsetGableRoofEnclosed;

                    if (iBlockSequence < (int)(iNumberOfIntermediateColumns / 2) + 1 || !bIsGable) // Left session
                    {
                        iColumnNumberLeft = iNumberOfMembers_tempForColumns + iBlockSequence - 1;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + iBlockSequence;

                        iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[0]; // iLeftColumnGirtNo;

                        for (int i = 0; i < iBlockSequence - 1; i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i + 1];

                        if(!bIsGable && iBlockSequence == iNumberOfIntermediateColumns) // Monopitch - posledna bay
                            bIsLastBayInFrontorBackSide = true;

                        if (sBuildingSide == "Front")
                            iRafterNumber = 1;
                        else
                            iRafterNumber = (iFrameNo - 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame) + 1;
                    }
                    else // Right session
                    {
                        bIsReverseSession = true; // Nodes and members are numbered from right to the left

                        iColumnNumberLeft = iNumberOfMembers_tempForColumns + (int)(iNumberOfIntermediateColumns / 2) + iNumberOfIntermediateColumns - iBlockSequence;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + (int)(iNumberOfIntermediateColumns / 2) + iNumberOfIntermediateColumns - iBlockSequence - 1;

                        // Number of girts in left session
                        iNumberOfFirstGirtInWallToDeactivate += iSideWallRightColumnGirtNoInOneFrame;

                        for (int i = 0; i < (int)(iNumberOfIntermediateColumns / 2); i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i+1];

                        if (iBlockSequence < iNumberOfIntermediateColumns)
                            iNumberOfFirstGirtInWallToDeactivate += iSideWallRightColumnGirtNoInOneFrame;

                        for (int i = 0; i < iNumberOfIntermediateColumns - iBlockSequence - 1; i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i+1];

                        if (iBlockSequence == iNumberOfIntermediateColumns) // Last bay
                        {
                            bIsLastBayInFrontorBackSide = true;
                            iColumnNumberRight = iSideMultiplier == 0 ? (iFrameMembersNo - 1) : (iFrameNo - 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame) + (iFrameMembersNo - 1);
                        }

                        if (sBuildingSide == "Front")
                            iRafterNumber = iFrameMembersNo - 1 - 1; // Minus right column, minus index 1
                        else
                            iRafterNumber = (iFrameNo - 1) * (iFrameMembersNo + iEavesPurlinNoInOneFrame) + (iFrameMembersNo - 1 - 1);
                    }

                    iFirstMemberToDeactivate = iNumberOfMembers_tempForGirts + iNumberOfFirstGirtInWallToDeactivate;
                }

                mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
                mColumnLeft = m_arrMembers[iColumnNumberLeft];
                mColumnRight = m_arrMembers[iColumnNumberRight];
                mEdgeRafter = m_arrMembers[iRafterNumber];
                mEavesPurlin = null; // Not defined for the front and back side
            }

            pControlPointBlock = new Point3D(mColumnLeft.NodeStart.X, mColumnLeft.NodeStart.Y, mColumnLeft.NodeStart.Z);
        }

        //Tuto funkciu mam pozriet - Mato chce:
        //rozsirujem tam velkosti poli a take veci CModel_PFD_01_GR - riadok 1751
        //vlastne tie objekty z objektu CBlock pridavam do celkoveho zoznamu, ale napriklad prerez pre girts som ignoroval aby tam nebol 2x
        //Chce to vymysliet nejaky koncept ako to ma fungovat a chce to programatorsku hlavu 🙂
        //tie moje "patlacky" ako sa to tam dolepuje do poli atd by som nebral velmi vazne
        //Malo by ty to fungovat tak, ze ked pridam prve dvere tak sa tie prierezy pridaju a ked pridavam dalsie, tak uz sa pridavaju len uzly a pruty a prierez sa len nastavi
        //uz by sa nemal vytvarat novy
        public void AddDoorOrWindowBlockProperties(Point3D pControlPointBlock, int iFirstMemberToDeactivate, CBlock block, bool addJoints = true)
        {
            float fBlockRotationAboutZaxis_rad = 0;

            if (block.BuildingSide == "Left" || block.BuildingSide == "Right")
                fBlockRotationAboutZaxis_rad = MathF.fPI / 2.0f; // Parameter of block - depending on side of building (front, back (0 deg), left, right (90 deg))

            //----------------------------------------------------------------------------------------------------------------------------------------------------
            // TODO 405 - TO Ondrej - tu sa znazim pripravit obrysove body otvoru v 3D - GCS
            // Opening definition points
            // Transformation from LCS of block to GCS // Create definition points in 3D

            List<Point3D> openningPointsInGCS = new List<Point3D>();

            foreach (System.Windows.Point p2D in block.openningPoints)
            {
                Point3D p3D = new Point3D(p2D.X, 0, p2D.Y);
                RotateAndTranslatePointAboutZ_CCW(pControlPointBlock, ref p3D, fBlockRotationAboutZaxis_rad);
                openningPointsInGCS.Add(p3D); // Output - s tymito suradnicami by sa mala porovnavat pozicia girt bracing na jednotlivych stranach budovy
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------------

            int arraysizeoriginal;

            // Cross-sections

            // Copy block cross-sections into the model
            for (int i = 1; i < block.m_arrCrSc.Length; i++) // Zacina sa od i = 1 - preskocit prvy prvok v poli doors, pretoze odkaz na girt section uz existuje, nie je potrebne prierez kopirovat znova
            {
                CCrSc foundCrsc = m_arrCrSc.FirstOrDefault(c => c.ID == block.m_arrCrSc[i].ID);
                if (foundCrsc != null) continue;

                arraysizeoriginal = m_arrCrSc.Length;
                Array.Resize(ref m_arrCrSc, arraysizeoriginal + 1); // ( - 1) Prvy prvok v poli blocks crsc ignorujeme
                // Preskocit prvy prvok v poli block crsc, pretoze odkaz na girt section uz existuje, nie je potrebne prierez kopirovat znova
                m_arrCrSc[arraysizeoriginal] = block.m_arrCrSc[i];
                //m_arrCrSc[arraysizeoriginal + i - 1].ID = arraysizeoriginal + i/* -1 + 1*/; // Odcitat index pretoze prvy prierez ignorujeme a pridat 1 pre ID (+ 1)
            }

            //task 405 - je to hotove, ale chcelo by to mozno aj zistit ako je to narocne na pamat, lebo su tam vyhladavacky
            DeactivateBracingBlocksThroughtBlock(block, openningPointsInGCS);

            // Nodes
            arraysizeoriginal = m_arrNodes.Length;
            Array.Resize(ref m_arrNodes, m_arrNodes.Length + block.m_arrNodes.Length);

            int iNumberofMembersToDeactivate = block.INumberOfGirtsToDeactivate;

            // Deactivate already generated members in the bay (space between frames) where is the block inserted
            for (int i = 0; i < iNumberofMembersToDeactivate; i++)
            {
                // Deactivate Members
                // Deactivate Member Joints
                CMember m = m_arrMembers[iFirstMemberToDeactivate + i];
                DeactivateMemberAndItsJoints(ref m);

                // -------------------------------------------------------------------------------------------------
                // Deactivate bracing blocks and joints
                // Find bracing blocks for deactivated girt
                DeactivateMemberBracingBlocks(m, block, openningPointsInGCS);
            }

            // Copy block nodes into the model
            for (int i = 0; i < block.m_arrNodes.Length; i++)
            {
                RotateAndTranslateNodeAboutZ_CCW(pControlPointBlock, ref block.m_arrNodes[i], fBlockRotationAboutZaxis_rad);
                m_arrNodes[arraysizeoriginal + i] = block.m_arrNodes[i];
                m_arrNodes[arraysizeoriginal + i].ID = arraysizeoriginal + i + 1;
            }

            // Members
            arraysizeoriginal = m_arrMembers.Length;
            Array.Resize(ref m_arrMembers, m_arrMembers.Length + block.m_arrMembers.Length);

            // Copy block members into the model
            for (int i = 0; i < block.m_arrMembers.Length; i++)
            {
                // Position of definition nodes was already changed, we dont need to rotate member definition nodes NodeStart and NodeEnd
                // Recalculate basic member data (PointA, PointB, delta projection length)
                block.m_arrMembers[i].Fill_Basic();

                m_arrMembers[arraysizeoriginal + i] = block.m_arrMembers[i];
                m_arrMembers[arraysizeoriginal + i].ID = arraysizeoriginal + i + 1;
            }

            // Add block member connections to the main model connections
            if (addJoints)
            {
                foreach (CConnectionJointTypes joint in block.m_arrConnectionJoints)
                    m_arrConnectionJoints.Add(joint); // Add joint
            }

            // Validation

            //// Number of added joints
            //int iNumberOfAddedJoints = 0;
            //// Number of added plates
            //int iNumberOfAddedPlates = 0;
            //
            //if (addJoints)
            //{
            //    foreach (CConnectionJointTypes joint in block.m_arrConnectionJoints)
            //    {
            //        m_arrConnectionJoints.Add(joint); // Add joint
            //
            //        iNumberOfAddedJoints++;
            //
            //        foreach (CPlate plate in joint.m_arrPlates)
            //        {
            //            iNumberOfAddedPlates++;
            //
            //            if (plate is CConCom_Plate_B_basic)
            //            {
            //                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
            //
            //                foreach (CAnchor anchor in basePlate.AnchorArrangement.Anchors)
            //                {
            //                    iNumberOfAddedPlates++; // anchor.WasherBearing
            //                    iNumberOfAddedPlates++; // anchor.WasherPlateTop
            //                }
            //            }
            //        }
            //    }
            //}
            //
            //System.Diagnostics.Trace.WriteLine(
            //    "Number of added joints: " + iNumberOfAddedJoints + "\n" +
            //    "Number of added plates and washers: " + iNumberOfAddedPlates);
        }
    }
}
