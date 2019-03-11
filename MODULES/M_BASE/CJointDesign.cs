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
        public List<CCalculJoint> listOfJointDesignInStartandEndLocation;
        public int fDesignRatioLocationID_Start = 0;
        public float fDesignRatio_Start = 0;

        public int fDesignRatioLocationID_End = 0;
        public float fDesignRatio_End = 0;

        public bool BUseCRSCGeometricalAxes;

        public CJointDesign(bool bDebugging_temp = false)
        {
            bDebugging = bDebugging_temp;
        }

        // PFD
        public void SetDesignForcesAndJointDesign_PFD(int iNumberOfDesignSections, bool bUseCRSCGeometricalAxes, CModel model, CMember member, basicInternalForces[] sBIF_x, out CConnectionJointTypes jointStart, out CConnectionJointTypes jointEnd, out designInternalForces[] sDIF_x)
        {
            BUseCRSCGeometricalAxes = bUseCRSCGeometricalAxes;
            int iNumberOfJointSections = 2; // Start and end of member - design internal forces
            model.GetModelMemberStartEndConnectionJoints(member, out jointStart, out jointEnd);

            listOfJointDesignInStartandEndLocation = new List<CCalculJoint>(iNumberOfJointSections);

            // Design
            sDIF_x = new designInternalForces[iNumberOfDesignSections];

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                if (j == 0 || j == iNumberOfDesignSections - 1) // Start or end result section
                {
                    sDIF_x[j].fN = sBIF_x[j].fN;
                    sDIF_x[j].fN_c = sDIF_x[j].fN > 0 ? 0f : Math.Abs(sDIF_x[j].fN);
                    sDIF_x[j].fN_t = sDIF_x[j].fN < 0 ? 0f : sDIF_x[j].fN;
                    sDIF_x[j].fT = sBIF_x[j].fT;

                    sDIF_x[j].fV_yu = sBIF_x[j].fV_yu;
                    sDIF_x[j].fM_zv = sBIF_x[j].fM_zv;

                    sDIF_x[j].fV_zv = sBIF_x[j].fV_zv;
                    sDIF_x[j].fM_yu = sBIF_x[j].fM_yu;

                    CCalculJoint obj_CalcDesign;

                    if (j == 0) // Start Joint Design
                    {
                        obj_CalcDesign = new CCalculJoint(bDebugging, bUseCRSCGeometricalAxes, jointStart, sDIF_x[j]);

                        fDesignRatioLocationID_Start = j;
                        fDesignRatio_Start = obj_CalcDesign.fEta_max;

                        listOfJointDesignInStartandEndLocation.Add(obj_CalcDesign);
                    }
                    else // End Joint Design
                    {
                        obj_CalcDesign = new CCalculJoint(bDebugging, bUseCRSCGeometricalAxes, jointEnd, sDIF_x[j]);

                        fDesignRatioLocationID_End = j;
                        fDesignRatio_End = obj_CalcDesign.fEta_max;

                        listOfJointDesignInStartandEndLocation.Add(obj_CalcDesign);
                    }
                }
            }
        }
    }
}
