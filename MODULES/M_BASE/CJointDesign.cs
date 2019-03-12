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

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                if (j == 0 || j == iNumberOfDesignSections - 1) // Start or end result section
                {
                    CCalculJoint obj_CalcDesign;

                    if (j == 0) // Start Joint Design
                    {
                        // Internal forces in joint
                        sjointStartDIF_x.fN = sBIF_x[j].fN;
                        sjointStartDIF_x.fN_c = sjointStartDIF_x.fN > 0 ? 0f : Math.Abs(sjointStartDIF_x.fN);
                        sjointStartDIF_x.fN_t = sjointStartDIF_x.fN < 0 ? 0f : sjointStartDIF_x.fN;
                        sjointStartDIF_x.fT = sBIF_x[j].fT;

                        if (bUseCRSCGeometricalAxes)
                        {
                            sjointStartDIF_x.fV_yy = sBIF_x[j].fV_yy;
                            sjointStartDIF_x.fV_zz = sBIF_x[j].fV_zz;

                            sjointStartDIF_x.fM_yy = sBIF_x[j].fM_yy;
                            sjointStartDIF_x.fM_zz = sBIF_x[j].fM_zz;
                        }
                        else
                        {
                            sjointStartDIF_x.fV_yu = sBIF_x[j].fV_yu;
                            sjointStartDIF_x.fV_zv = sBIF_x[j].fV_zv;

                            sjointStartDIF_x.fM_yu = sBIF_x[j].fM_yu;
                            sjointStartDIF_x.fM_zv = sBIF_x[j].fM_zv;
                        }

                        // Design
                        obj_CalcDesign = new CCalculJoint(bDebugging, bUseCRSCGeometricalAxes, jointStart, sjointStartDIF_x);
                        fDesignRatio_Start = obj_CalcDesign.fEta_max;
                    }
                    else // End Joint Design
                    {
                        // Internal forces in joint
                        sjointEndDIF_x.fN = sBIF_x[j].fN;
                        sjointEndDIF_x.fN_c = sjointEndDIF_x.fN > 0 ? 0f : Math.Abs(sjointEndDIF_x.fN);
                        sjointEndDIF_x.fN_t = sjointEndDIF_x.fN < 0 ? 0f : sjointEndDIF_x.fN;
                        sjointEndDIF_x.fT = sBIF_x[j].fT;

                        if (bUseCRSCGeometricalAxes)
                        {
                            sjointEndDIF_x.fV_yy = sBIF_x[j].fV_yy;
                            sjointEndDIF_x.fV_zz = sBIF_x[j].fV_zz;

                            sjointEndDIF_x.fM_yy = sBIF_x[j].fM_yy;
                            sjointEndDIF_x.fM_zz = sBIF_x[j].fM_zz;
                        }
                        else
                        {
                            sjointEndDIF_x.fV_yu = sBIF_x[j].fV_yu;
                            sjointEndDIF_x.fV_zv = sBIF_x[j].fV_zv;

                            sjointEndDIF_x.fM_yu = sBIF_x[j].fM_yu;
                            sjointEndDIF_x.fM_zv = sBIF_x[j].fM_zv;
                        }

                        // Design
                        obj_CalcDesign = new CCalculJoint(bDebugging, bUseCRSCGeometricalAxes, jointEnd, sjointEndDIF_x);
                        fDesignRatio_End = obj_CalcDesign.fEta_max;
                    }
                }
            }
        }
    }
}
