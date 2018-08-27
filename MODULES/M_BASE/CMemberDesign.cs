﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using M_AS4600;
using CRSC;

namespace M_BASE
{
    public class CMemberDesign
    {
        bool bDebugging;
        public List<CCalcul> listOfMemberDesignInLocations;
        public int fMaximumDesignRatioLocationID = 0;
        public float fMaximumDesignRatio = 0;

        public CMemberDesign(bool bDebugging_temp = false)
        {
            bDebugging = bDebugging_temp;
        }

        // SBD
        public void SetDesignForcesAndMemberDesign_SBD(int iNumberOfDesignSections, CMember member, basicInternalForces[,] sBIF_x, designMomentValuesForCb[] sMomentValuesforCb, out designInternalForces[,] sDIF_x)
        {
            SetDesignForcesAndMemberDesign_SBD(1, iNumberOfDesignSections, member, sBIF_x, sMomentValuesforCb, out sDIF_x);
        }

        public void SetDesignForcesAndMemberDesign_SBD(int iNumberOfLoadCombinations, int iNumberOfDesignSections, CMember member, basicInternalForces[,] sBIF_x, designMomentValuesForCb[] sMomentValuesforCb, out designInternalForces[,] sDIF_x)
        {
            listOfMemberDesignInLocations = new List<CCalcul>(iNumberOfDesignSections);
            // Design
            sDIF_x = new designInternalForces[iNumberOfLoadCombinations, iNumberOfDesignSections];

            for (int i = 0; i < iNumberOfLoadCombinations; i++)
            {
                for (int j = 0; j < iNumberOfDesignSections; j++)
                {
                    sDIF_x[i, j].fN = sBIF_x[i, j].fN;
                    sDIF_x[i, j].fN_c = sDIF_x[i, j].fN > 0 ? 0f : Math.Abs(sDIF_x[i, j].fN);
                    sDIF_x[i, j].fN_t = sDIF_x[i, j].fN < 0 ? 0f : sDIF_x[i, j].fN;
                    sDIF_x[i, j].fT = sBIF_x[i, j].fT;

                    sDIF_x[i, j].fV_yu = sBIF_x[i, j].fV_yu;
                    sDIF_x[i, j].fM_zv = sBIF_x[i, j].fM_zv;

                    sDIF_x[i, j].fV_zv = sBIF_x[i, j].fV_zv;
                    sDIF_x[i, j].fM_yu = sBIF_x[i, j].fM_yu;

                    CCalcul obj_CalcDesign = new CCalcul(bDebugging, sDIF_x[i, j], member, sMomentValuesforCb[i]);

                    if (obj_CalcDesign.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatioLocationID = j;
                        fMaximumDesignRatio = obj_CalcDesign.fEta_max;
                    }

                    listOfMemberDesignInLocations.Add(obj_CalcDesign);
                }
            }
        }

        // PFD
        public void SetDesignForcesAndMemberDesign_PFD(int iNumberOfDesignSections, CMember member, basicInternalForces[] sBIF_x, designMomentValuesForCb sMomentValuesforCb, out designInternalForces[] sDIF_x)
        {
            listOfMemberDesignInLocations = new List<CCalcul>(iNumberOfDesignSections);
            // Design
            sDIF_x = new designInternalForces[iNumberOfDesignSections];

                for (int j = 0; j < iNumberOfDesignSections; j++)
                {
                sDIF_x[j].fN = sBIF_x[j].fN;
                sDIF_x[j].fN_c = sDIF_x[j].fN > 0 ? 0f : Math.Abs(sDIF_x[j].fN);
                sDIF_x[j].fN_t = sDIF_x[j].fN < 0 ? 0f : sDIF_x[j].fN;
                sDIF_x[j].fT = sBIF_x[j].fT;

                sDIF_x[j].fV_yu = sBIF_x[j].fV_yu;
                sDIF_x[j].fM_zv = sBIF_x[j].fM_zv;

                sDIF_x[j].fV_zv = sBIF_x[j].fV_zv;
                sDIF_x[j].fM_yu = sBIF_x[j].fM_yu;

                CCalcul obj_CalcDesign = new CCalcul(bDebugging, sDIF_x[j], member, sMomentValuesforCb);

                    if (obj_CalcDesign.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatioLocationID = j;
                        fMaximumDesignRatio = obj_CalcDesign.fEta_max;
                    }

                    listOfMemberDesignInLocations.Add(obj_CalcDesign);
                }

        }
    }
}
