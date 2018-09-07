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
        public int fMaximumDesignRatioLocationID = 0;
        public float fMaximumDesignRatio = 0;

        public CJointDesign(bool bDebugging_temp = false)
        {
            bDebugging = bDebugging_temp;
        }

        // PFD
        public void SetDesignForcesAndJointDesign_PFD(int iNumberOfDesignSections, CMember member, basicInternalForces[] sBIF_x, out CConnectionJointTypes joint, out designInternalForces[] sDIF_x)
        {
            int iNumberOfJointSections = 2; // Start and end of Member

            listOfJointDesignInStartandEndLocation = new List<CCalculJoint>(iNumberOfJointSections);
            // Design
            sDIF_x = new designInternalForces[iNumberOfDesignSections];
            joint = null; // Temporary

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                if (j == 0 || j == iNumberOfDesignSections - 1) // Start or end result section
                    sDIF_x[j].fN = sBIF_x[j].fN;
                sDIF_x[j].fN_c = sDIF_x[j].fN > 0 ? 0f : Math.Abs(sDIF_x[j].fN);
                sDIF_x[j].fN_t = sDIF_x[j].fN < 0 ? 0f : sDIF_x[j].fN;
                sDIF_x[j].fT = sBIF_x[j].fT;

                sDIF_x[j].fV_yu = sBIF_x[j].fV_yu;
                sDIF_x[j].fM_zv = sBIF_x[j].fM_zv;

                sDIF_x[j].fV_zv = sBIF_x[j].fV_zv;
                sDIF_x[j].fM_yu = sBIF_x[j].fM_yu;

                ////////////////////////////////
                // TODO - identifikovat spoj v zaciatocnom a koncovom uzle pruta, skontrolovat ci nejake spoj existuje

                // TODO Ondrej - najst spoj ktory patri k zaciatocnemu a koncovemu uzlu pruta;
                CConnectionJoint_T001 temporaryJoint = new CConnectionJoint_T001("LH", member.NodeStart, member, member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true);
                joint = temporaryJoint; // Temporary output

                CCalculJoint obj_CalcDesign = new CCalculJoint(bDebugging, temporaryJoint /*j == 0 ? member.NodeStart.Joint : member.NodeEnd.Joint*/, sDIF_x[j]);

                if (obj_CalcDesign.fEta_max > fMaximumDesignRatio)
                {
                    fMaximumDesignRatioLocationID = j;
                    fMaximumDesignRatio = obj_CalcDesign.fEta_max;
                }

                listOfJointDesignInStartandEndLocation.Add(obj_CalcDesign);
            }
        }
    }
}
