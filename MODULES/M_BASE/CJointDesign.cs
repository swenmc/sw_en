using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using M_AS4600;
using CRSC;

namespace M_BASE
{
    public class CJointDesign
    {
        bool bDebugging;
        public float fDesignRatio_Start = 0;
        public float fDesignRatio_End = 0;
        public float fDesignRatio_Footing = 0;
        public bool BUseCRSCGeometricalAxes;

        public CJointDesign(bool bDebugging_temp = false)
        {
            bDebugging = bDebugging_temp;
        }

        // PFD
        public void SetDesignForcesAndJointDesign_PFD(int iNumberOfDesignSections, bool bUseCRSCGeometricalAxes, CModel model, CMember member, basicInternalForces[] sBIF_x, out CConnectionJointTypes jointStart, out CConnectionJointTypes jointEnd, out designInternalForces sjointStartDIF_x, out designInternalForces sjointEndDIF_x)
        {
            BUseCRSCGeometricalAxes = bUseCRSCGeometricalAxes;
            model.GetModelMemberStartEndConnectionJoints(member, out jointStart, out jointEnd);

            // Design
            sjointStartDIF_x = new designInternalForces();
            sjointEndDIF_x = new designInternalForces();

            //-------------------------------------------------------------------------------------------------------------
            // TODO Ondrej, tu asi musime posielat do vypoctu nastavenia z UC_Footings a nie objekt ako null
            // TODO Ondrej - potrebujem sem dostat nastavenia vypoctu z UC_FootingInput a nahradit tieto konstanty
            CalculationSettingsFoundation FootingCalcSettings = new CalculationSettingsFoundation();
            FootingCalcSettings.ConcreteGrade = "30";
            FootingCalcSettings.AggregateSize = 0.02f;
            FootingCalcSettings.ConcreteDensity = 2300f;
            FootingCalcSettings.ReinforcementGrade = "500E";
            FootingCalcSettings.SoilReductionFactor_Phi = 0.5f;
            FootingCalcSettings.SoilReductionFactorEQ_Phi = 0.8f;
            FootingCalcSettings.SoilBearingCapacity = 100e+3f;
            FootingCalcSettings.FloorSlabThickness = 0.125f;
            //-------------------------------------------------------------------------------------------------------------

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                if (j == 0 || j == iNumberOfDesignSections - 1) // Start or end result section
                {
                    CCalculJoint obj_CalcDesign;

                    if (j == 0) // Start Joint Design
                    {
                        // Set design internal forces in joint
                        SetDesignInternalForces(bUseCRSCGeometricalAxes, sBIF_x[j], ref sjointStartDIF_x);

                        // Design joint
                        obj_CalcDesign = new CCalculJoint(bDebugging, bUseCRSCGeometricalAxes, jointStart, model, /*FootingCalcSettings*/ null, sjointStartDIF_x);
                        fDesignRatio_Start = obj_CalcDesign.fEta_max;
                    }
                    else // End Joint Design
                    {
                        // Set design internal forces in joint
                        SetDesignInternalForces(bUseCRSCGeometricalAxes, sBIF_x[j], ref sjointEndDIF_x);

                        // Design joint
                        obj_CalcDesign = new CCalculJoint(bDebugging, bUseCRSCGeometricalAxes, jointEnd, model, /*FootingCalcSettings*/ null, sjointEndDIF_x);
                        fDesignRatio_End = obj_CalcDesign.fEta_max;
                    }
                }
            }
        }

        private void SetDesignInternalForces(bool bUseCRSCGeometricalAxes, basicInternalForces sBIF_x, ref designInternalForces sDIF_x)
        {
            // Internal forces
            sDIF_x.fN = sBIF_x.fN;
            sDIF_x.fN_c = sDIF_x.fN > 0 ? 0f : Math.Abs(sDIF_x.fN);
            sDIF_x.fN_t = sDIF_x.fN < 0 ? 0f : sDIF_x.fN;
            sDIF_x.fT = sBIF_x.fT;

            if (bUseCRSCGeometricalAxes)
            {
                sDIF_x.fV_yy = sBIF_x.fV_yy;
                sDIF_x.fV_zz = sBIF_x.fV_zz;

                sDIF_x.fM_yy = sBIF_x.fM_yy;
                sDIF_x.fM_zz = sBIF_x.fM_zz;
            }
            else
            {
                sDIF_x.fV_yu = sBIF_x.fV_yu;
                sDIF_x.fV_zv = sBIF_x.fV_zv;

                sDIF_x.fM_yu = sBIF_x.fM_yu;
                sDIF_x.fM_zv = sBIF_x.fM_zv;
            }
        }
    }
}
