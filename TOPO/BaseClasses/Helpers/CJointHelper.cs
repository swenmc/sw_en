
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
    }
}
