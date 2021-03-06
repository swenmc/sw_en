﻿
using System;
using System.Collections.Generic;

namespace BaseClasses.Helpers
{
    public static class CJointHelper
    {
        public static void SetJoinModelRotationDisplayOptions(CConnectionJointTypes joint, ref DisplayOptions opt)
        {
            switch (joint.JointType)
            {
                case EJointType.eBase_MainColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eKnee_MainRafter_Column:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eApex_MainRafters:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eBase_EdgeColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eKnee_EgdeRafter_Column:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eApex_Edge_Rafters:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafter_FlyBracing:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafter_FlyBracing:
                    opt.CO_RotateModelX =  -100; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eEdgePurlin_MainRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225-90; opt.CO_RotateModelZ = 0; break;
                case EJointType.eEdgePurlin_EdgeRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_MainColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_MainColumn_FlyBracing:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn_FlyBracing:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eBase_WindPost_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eBase_WindPost_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindPost_EdgeRafter_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindPost_EdgeRafter_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_WindPost_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_WindPost_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eBase_DoorTrimmer:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_Girt:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_GirtFront:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_Girt_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_DoorTrimmer:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_DoorTrimmer_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_DoorTrimmer_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_EdgePurlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_EdgeRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorLintel_Trimmer:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eBase_DoorFrame:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorFrame_Girt:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorFrame_Girt_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorFrame_Girt_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_DoorFrame:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_DoorFrame_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirt_DoorFrame_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorFrame_EdgePurlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorFrame_EdgeRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eDoorFrame:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Girt:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Girt_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Girt_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindowFrame_EdgePurlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindowFrame_EdgeRafter:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Header_Sill_WindowFrameColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirtbracing_Girt:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirtBracing_EdgePurlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirtBracing_Girt_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirtBracing_EdgeRafter_Front:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirtBracing_Girt_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eGirtBracing_EdgeRafter_Back:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlinBracing_Purlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlinBracing_EdgePurlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.eCrossBracing_MainColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eCrossBracing_EdgeColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eCrossBracing_MainRafter:
                    opt.CO_RotateModelX = 45; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 45; break;
                case EJointType.eCrossBracing_EdgeRafter:
                    opt.CO_RotateModelX = -45; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = -45; break;
                // Canopy
                case EJointType.eKnee_MainRafterCanopy_Column:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eKnee_EdgeRafterCanopy_Column:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eKnee_EdgeRafterCanopy_EdgeColumn:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafterCanopy:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafterCanopy:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafterCanopy_FlyBracing:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafterCanopy_FlyBracing:
                    opt.CO_RotateModelX = -100; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafterCanopyEdge_FlyBracing:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafterCanopyEdge_FlyBracing:
                    opt.CO_RotateModelX = -100; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlinBracingCanopy_PurlinCanopy: // TODO - je potrebné dostat sem nastavenie every second row  purlin bracing a zmenit otocenie 
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlinBracingCanopy_EdgePurlin:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 225; opt.CO_RotateModelZ = 0; break;
                case EJointType.ePurlinBracingCanopy_EdgePurlinCanopy:
                    opt.CO_RotateModelX = -90; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 0; break;
                case EJointType.eCrossBracing_MainRafterCanopy:
                    opt.CO_RotateModelX = 45; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = 45; break;
                case EJointType.eCrossBracing_EdgeRafterCanopy:
                    opt.CO_RotateModelX = -45; opt.CO_RotateModelY = 45; opt.CO_RotateModelZ = -45; break;

                default:
                    System.Diagnostics.Trace.WriteLine("ERROR!!! Joint type: " + joint.JointType + " not recognized." + joint.GetType().ToString()); break;
            }
        }

        public static CScrewArrangement GetBasePlateArrangement(string platePrefix, CScrew referenceScrew/*, float fh_plate*/)
        {
            // TODO - Urobit nastavovanie rozmerov dynamicky podla velkosti prierezu / vyztuh a plechu
            // Nacitat velkosti vyztuh z parametrov prierezu, medzery a polohu skrutiek urcovat dynamicky
            // BX1 - 2 rectangular sequencies
            // BX2 - 3 rectangular sequencies

            // BX2 - 3 rectangular sequencies - nahradime jednou sekvenciou s roznymi rozostupmi

            //CScrewArrangement_BX_2
            //CScrewArrangement_BX screwArrangement2_10075_92 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.006f - 2 * 0.002f, 0.023f,*/
            //        2, 1, 0.020f, 0.012f, 0.030f, 0.03f,
            //        2, 1, 0.020f, 0.050f, 0.030f, 0.03f,
            //        2, 1, 0.020f, 0.085f, 0.030f, 0.03f);
            //CScrewArrangement_BX_2
            //CScrewArrangement_BX screwArrangement2_270XXX_180 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,*/
            //        3, 1, 0.030f, 0.030f, 0.060f, 0.050f,
            //        3, 1, 0.030f, 0.135f, 0.060f, 0.050f,
            //        3, 1, 0.030f, 0.240f, 0.060f, 0.050f);
            //CScrewArrangement_BX_2
            //CScrewArrangement_BX screwArrangement2_270XXX_100 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,*/
            //        2, 1, 0.025f, 0.030f, 0.050f, 0.050f,
            //        2, 1, 0.025f, 0.135f, 0.050f, 0.050f,
            //        2, 1, 0.025f, 0.240f, 0.050f, 0.050f);
            //CScrewArrangement_BX_2
            //CScrewArrangement_BX screwArrangement2_270XXXn_180 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.058f,*/
            //        3, 1, 0.030f, 0.030f, 0.060f, 0.050f,
            //        3, 1, 0.030f, 0.135f, 0.060f, 0.050f,
            //        3, 1, 0.030f, 0.260f, 0.060f, 0.050f);

            if (platePrefix == "BA")
            {
                CScrewArrangement_BX screwArrangement2_270XXX_180 = new CScrewArrangement_BX(referenceScrew,
                    3, 3, 0.030f, 0.030f, 0.060f, 0.105f);
                return screwArrangement2_270XXX_180;
            }
            else if (platePrefix == "BG")
            {
                CScrewArrangement_BX screwArrangement2_270XXX_100 = new CScrewArrangement_BX(referenceScrew,
                   2, 3, 0.025f, 0.030f, 0.050f, 0.105f);
                return screwArrangement2_270XXX_100;
            }
            else if (platePrefix == "BB" || platePrefix == "BC")
            {
                CScrewArrangement_BX screwArrangement2_270XXXn_180 = new CScrewArrangement_BX(referenceScrew,
                   3, 3, 0.030f, 0.030f, new List<float>(1) { 0.060f }, new List<float>(2) { 0.105f, 0.135f });
                return screwArrangement2_270XXXn_180;

            }
            else if (platePrefix == "BD")
            {
                CScrewArrangement_BX screwArrangement1_50020_154 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.132f,*/
                      3, 3, 0.027f, 0.030f, 0.050f, 0.060f,
                      3, 3, 0.027f, 0.345f, 0.050f, 0.060f);
                return screwArrangement1_50020_154;
            }
            else if (platePrefix == "BE-2 holes" || platePrefix == "BE-3 holes" || platePrefix == "BE-4 holes")
            {
                CScrewArrangement_BX screwArrangement1_50020n_154 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.182f,*/
                    3, 3, 0.027f, 0.030f, 0.050f, 0.060f,
                    3, 3, 0.027f, 0.395f, 0.050f, 0.060f);
                return screwArrangement1_50020n_154;
            }
            else if (platePrefix == "BF-4 holes" || platePrefix == "BF-6 holes")
            {
                CScrewArrangement_BX screwArrangement1_63020_400 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,*/
                    3, 3, 0.020f, 0.020f, 0.150f, 0.080f,
                    3, 3, 0.020f, 0.400f, 0.150f, 0.080f);
                return screwArrangement1_63020_400;
            }
            else if (platePrefix == "BH" || platePrefix == "BI")
            {
                CScrewArrangement_BX screwArrangement2_10075_92 = new CScrewArrangement_BX(referenceScrew,
                    2, 3, 0.020f, 0.012f, 0.030f, 0.035f);
                return screwArrangement2_10075_92;
            }
            else if (platePrefix == "BJ-2 holes" || platePrefix == "BJ-3 holes")
            {
                CScrewArrangement_BX screwArrangement1_63020_180 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,*/
                    3, 5, 0.030f, 0.029f, 0.060f, 0.035f,
                    3, 5, 0.030f, 0.404f, 0.060f, 0.035f);
                return screwArrangement1_63020_180;
            }
            else
                return null; // Exception - not defined plate prefix
        }

        public static CScrewArrangementCircleApexOrKnee GetDefaultCircleScrewArrangement(
            float fCrscDepth,
            float fCrscWebEndArcExternalRadius,
            float fCrscWebStraightDepth,
            float fCrscWebMiddlePart,
            CScrew referenceScrew)
        {
            // Minimalna vzdialenost skrutky od hrany ohybu pozdlzneho rebra / vyztuhy na priereze (hrana zakrivenej casti)
            float fMinimumStraightEdgeDistance = 0.010f;

            // Recalculate default radius and number of screws depending on cross-section depth
            float fMinimumDistanceBetweenScrews = 0.02f;

            // Circle arrangement - parameters
            bool bUseAdditionalCornerScrews = true;
            int iAdditionalConnectorInCornerNumber = 4; // 4 screws in each corner
            float fAdditionalConnectorDistance = Math.Max(fMinimumDistanceBetweenScrews, 0.05f * fCrscWebStraightDepth);
            float fConnectorRadiusInCircleSequence = 0.5f * (fCrscWebStraightDepth - 2 * fMinimumStraightEdgeDistance);
            float fDistanceBetweenScrewsInCircle = 0.05f;

            if (fCrscDepth < 0.5f) // Zmenseny default - TODO mohol by byt urceny podla tvaru prierezu
            {
                fDistanceBetweenScrewsInCircle = 0.030f;
            }

            

            // http://www.ambrsoft.com/TrigoCalc/Sphere/Arc_.htm
            float fAngle = 2f * (float)Math.Acos((0.5f * (fCrscWebMiddlePart + 2f * fMinimumDistanceBetweenScrews)) / fConnectorRadiusInCircleSequence);
            int iConnectorNumberInCircleSequence = (int)((fAngle * fConnectorRadiusInCircleSequence) / fDistanceBetweenScrewsInCircle) + 1; // Pocet medzier + 1

            List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
            gr1.NumberOfHalfCircleSequences = 2;
            gr1.NumberOfRectangularSequences = 4;
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr1);
            CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
            gr2.NumberOfHalfCircleSequences = 2;
            gr2.NumberOfRectangularSequences = 4;
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr2);

            bool bUseExtraScrews = false;
            int iExtraScrewsRows = 1;
            int iExtraScrewsInRow = 8;
            float positionOfExtraScrews_y = fConnectorRadiusInCircleSequence + 0.02f;

            return new CScrewArrangementCircleApexOrKnee(referenceScrew, fCrscDepth, fCrscWebStraightDepth, fCrscWebMiddlePart, 1, screwSeqGroups, bUseAdditionalCornerScrews, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iAdditionalConnectorInCornerNumber, fAdditionalConnectorDistance, fAdditionalConnectorDistance, bUseExtraScrews, iExtraScrewsRows, iExtraScrewsInRow, positionOfExtraScrews_y, 0.05f, 0.03f);
        }

        public static CJointLoadCombinationRatio_ULS FindResultWithMaximumDesignRatio(IEnumerable<CJointLoadCombinationRatio_ULS> results)
        {
            CJointLoadCombinationRatio_ULS result = null;
            float maxDesignRatio = float.MinValue;
            foreach (CJointLoadCombinationRatio_ULS r in results)
            {
                if (r.MaximumDesignRatio > maxDesignRatio) { result = r; maxDesignRatio = r.MaximumDesignRatio; }

            }
            return result;
        }
        public static CFootingLoadCombinationRatio_ULS FindResultWithMaximumDesignRatio(IEnumerable<CFootingLoadCombinationRatio_ULS> results)
        {
            CFootingLoadCombinationRatio_ULS result = null;
            float maxDesignRatio = float.MinValue;
            foreach (CFootingLoadCombinationRatio_ULS r in results)
            {
                if (r.MaximumDesignRatio > maxDesignRatio) { result = r; maxDesignRatio = r.MaximumDesignRatio; }

            }
            return result;
        }
    }
}
