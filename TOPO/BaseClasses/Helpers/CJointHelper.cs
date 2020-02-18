
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
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eKnee_MainRafter_Column:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eApex_MainRafters:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eBase_EdgeColumn:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eKnee_EgdeRafter_Column:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eApex_Edge_Rafters:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.ePurlin_MainRafter_FlyBracing:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.ePurlin_EdgeRafter_FlyBracing:
                    opt.RotateModelX =  -100; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eEdgePurlin_MainRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 225-90; opt.RotateModelZ = 0; break;
                case EJointType.eEdgePurlin_EdgeRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_MainColumn:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eBase_WindPost_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eBase_WindPost_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindPost_EdgeRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eWindPost_EdgeRafter_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_EdgeColumn_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_WindPost_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_WindPost_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eBase_DoorTrimmer:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_Girt:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_GirtFront:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_Girt_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_DoorTrimmer:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_DoorTrimmer_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_DoorTrimmer_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_EdgePulin:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorTrimmer_EdgeRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorLintel_Trimmer:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eBase_DoorFrame:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorFrame_Girt:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorFrame_Girt_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorFrame_Girt_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_DoorFrame:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_DoorFrame_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eGirt_DoorFrame_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 225; opt.RotateModelZ = 0; break;
                case EJointType.eDoorFrame_EdgePulin:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorFrame_EdgeRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eDoorFrame:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Girt:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Girt_Front:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Girt_Back:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindowFrame_EdgePulin:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindowFrame_EdgeRafter:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
                case EJointType.eWindowFrame_Header_Sill_WindowFrameColumn:
                    opt.RotateModelX = -90; opt.RotateModelY = 45; opt.RotateModelZ = 0; break;
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

            CScrewArrangement_BX screwArrangement2_10075_92 = new CScrewArrangement_BX(referenceScrew,
                    2, 3, 0.020f, 0.012f, 0.030f, 0.035f);

            CScrewArrangement_BX screwArrangement2_270XXX_180 = new CScrewArrangement_BX(referenceScrew,
                    3, 3, 0.030f, 0.030f, 0.060f, 0.105f);

            CScrewArrangement_BX screwArrangement2_270XXX_100 = new CScrewArrangement_BX(referenceScrew,
                    2, 3, 0.025f, 0.030f, 0.050f, 0.105f);

            CScrewArrangement_BX screwArrangement2_270XXXn_180 = new CScrewArrangement_BX(referenceScrew,
                    3, 3, 0.030f, 0.030f, new List<float>(1) { 0.060f }, new List<float>(2) { 0.105f, 0.135f });

            //CScrewArrangement_BX_1
            CScrewArrangement_BX screwArrangement1_50020_154 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.132f,*/
                    3, 3, 0.027f, 0.030f, 0.050f, 0.060f,
                    3, 3, 0.027f, 0.345f, 0.050f, 0.060f);
            //CScrewArrangement_BX_1
            CScrewArrangement_BX screwArrangement1_50020n_154 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.008f - 2 * 0.002f, 0.182f,*/
                    3, 3, 0.027f, 0.030f, 0.050f, 0.060f,
                    3, 3, 0.027f, 0.395f, 0.050f, 0.060f);
            //CScrewArrangement_BX_1
            CScrewArrangement_BX screwArrangement1_63020_180 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,*/
                    3, 5, 0.030f, 0.029f, 0.060f, 0.035f,
                    3, 5, 0.030f, 0.404f, 0.060f, 0.035f);
            //CScrewArrangement_BX_1
            CScrewArrangement_BX screwArrangement1_63020_400 = new CScrewArrangement_BX(referenceScrew, /*fh_plate, fh_plate - 2 * 0.025f - 2 * 0.002f, 0.185f,*/
                    7, 5, 0.050f, 0.029f, 0.050f, 0.035f,
                    7, 5, 0.050f, 0.401f, 0.050f, 0.035f);

            if (platePrefix == "BA")
                return screwArrangement2_270XXX_180;
            if (platePrefix == "BG")
                return screwArrangement2_270XXX_100;
            else if (platePrefix == "BB" || platePrefix == "BC")
                return screwArrangement2_270XXXn_180;
            else if (platePrefix == "BD")
                return screwArrangement1_50020_154;
            else if (platePrefix == "BE-2 holes" || platePrefix == "BE-3 holes")
                return screwArrangement1_50020n_154;
            else if (platePrefix == "BF-4 holes" || platePrefix == "BF-6 holes")
                return screwArrangement1_63020_400;
            else if (platePrefix == "BH" || platePrefix == "BI")
                return screwArrangement2_10075_92;
            else if (platePrefix == "BJ-2 holes" || platePrefix == "BJ-3 holes")
                return screwArrangement1_63020_180;
            else
                return null; // Exception - not defined plate prefix
        }

    }
}
