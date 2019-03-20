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
    public class CMemberDesign
    {
        bool bDebugging;
        public List<CCalculMember> listOfMemberDesignInLocations;
        public int fMaximumDesignRatioLocationID = 0;
        public float fMaximumDesignRatio = 0;

        public CMemberDesign(bool bDebugging_temp = false)
        {
            bDebugging = bDebugging_temp;
        }

        // SBD
        public void SetDesignForcesAndMemberDesign_SBD(bool bUseCRSCGeometricalAxes, int iNumberOfDesignSections, CMember member, basicInternalForces[,] sBIF_x, designBucklingLengthFactors[] sBucklingLengthFactors, designMomentValuesForCb[] sMomentValuesforCb, out designInternalForces[,] sDIF_x)
        {
            SetDesignForcesAndMemberDesign_SBD(bUseCRSCGeometricalAxes, 1, iNumberOfDesignSections, member, sBIF_x, sBucklingLengthFactors, sMomentValuesforCb, out sDIF_x);
        }

        public void SetDesignForcesAndMemberDesign_SBD(bool bUseCRSCGeometricalAxes, int iNumberOfLoadCombinations, int iNumberOfDesignSections, CMember member, basicInternalForces[,] sBIF_x, designBucklingLengthFactors[] sBucklingLengthFactors, designMomentValuesForCb[] sMomentValuesforCb, out designInternalForces[,] sDIF_x)
        {
            listOfMemberDesignInLocations = new List<CCalculMember>(iNumberOfDesignSections);
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

                    if (bUseCRSCGeometricalAxes)
                    {
                        sDIF_x[i, j].fV_yy = sBIF_x[i, j].fV_yy;
                        sDIF_x[i, j].fV_zz = sBIF_x[i, j].fV_zz;
                        sDIF_x[i, j].fM_yy = sBIF_x[i, j].fM_yy;
                        sDIF_x[i, j].fM_zz = sBIF_x[i, j].fM_zz;
                    }
                    else
                    {
                        sDIF_x[i, j].fV_yu = sBIF_x[i, j].fV_yu;
                        sDIF_x[i, j].fV_zv = sBIF_x[i, j].fV_zv;
                        sDIF_x[i, j].fM_yu = sBIF_x[i, j].fM_yu;
                        sDIF_x[i, j].fM_zv = sBIF_x[i, j].fM_zv;
                    }

                    CCalculMember obj_CalcDesign = new CCalculMember(bDebugging, bUseCRSCGeometricalAxes, sDIF_x[i, j], member, sBucklingLengthFactors[i], sMomentValuesforCb[i]);

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
        public void SetDesignForcesAndMemberDesign_PFD(bool bUseCRSCGeometricalAxes, int iNumberOfDesignSections, CMember member, basicInternalForces[] sBIF_x,
            designBucklingLengthFactors[] sBucklingLengthFactors, designMomentValuesForCb[] sMomentValuesforCb, out designInternalForces[] sDIF_x)
        {
            listOfMemberDesignInLocations = new List<CCalculMember>(iNumberOfDesignSections);
            // Design
            sDIF_x = new designInternalForces[iNumberOfDesignSections];

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                sDIF_x[j].fN = sBIF_x[j].fN;
                sDIF_x[j].fN_c = sDIF_x[j].fN > 0 ? 0f : Math.Abs(sDIF_x[j].fN);
                sDIF_x[j].fN_t = sDIF_x[j].fN < 0 ? 0f : sDIF_x[j].fN;
                sDIF_x[j].fT = sBIF_x[j].fT;

                if (bUseCRSCGeometricalAxes)
                {
                    sDIF_x[j].fV_yy = sBIF_x[j].fV_yy;
                    sDIF_x[j].fV_zz = sBIF_x[j].fV_zz;
                    sDIF_x[j].fM_yy = sBIF_x[j].fM_yy;
                    sDIF_x[j].fM_zz = sBIF_x[j].fM_zz;
                }
                else
                {
                    sDIF_x[j].fV_yu = sBIF_x[j].fV_yu;
                    sDIF_x[j].fV_zv = sBIF_x[j].fV_zv;
                    sDIF_x[j].fM_yu = sBIF_x[j].fM_yu;
                    sDIF_x[j].fM_zv = sBIF_x[j].fM_zv;
                }

                CCalculMember obj_CalcDesign = new CCalculMember(bDebugging, bUseCRSCGeometricalAxes, sDIF_x[j], member, sBucklingLengthFactors[j], sMomentValuesforCb[j]);

                if (obj_CalcDesign.fEta_max > fMaximumDesignRatio)
                {
                    fMaximumDesignRatioLocationID = j;
                    fMaximumDesignRatio = obj_CalcDesign.fEta_max;
                }

                listOfMemberDesignInLocations.Add(obj_CalcDesign);
            }
        }

        public void SetDesignDeflections_PFD(bool bUseCRSCGeometricalAxes, int iNumberOfDesignSections, CMember member, float fDeflectionLimit, basicDeflections[] sBDeflections_x, out designDeflections[] sDDeflections_x)
        {
            listOfMemberDesignInLocations = new List<CCalculMember>(iNumberOfDesignSections);
            // Design
            sDDeflections_x = new designDeflections[iNumberOfDesignSections];

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                if (bUseCRSCGeometricalAxes)
                {
                    sDDeflections_x[j].fDelta_yy = sBDeflections_x[j].fDelta_yy;
                    sDDeflections_x[j].fDelta_zz = sBDeflections_x[j].fDelta_zz;
                }
                else
                {
                    sDDeflections_x[j].fDelta_yu = sBDeflections_x[j].fDelta_yu;
                    sDDeflections_x[j].fDelta_zv = sBDeflections_x[j].fDelta_zv;
                }

                sDDeflections_x[j].fDelta_tot = sBDeflections_x[j].fDelta_tot;

                CCalculMember obj_CalcDesign = new CCalculMember(bDebugging, bUseCRSCGeometricalAxes, sDDeflections_x[j], member, fDeflectionLimit);

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
