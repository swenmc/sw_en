using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;

namespace FEM_CALC_BASE
{
    public static class CInternalForcesManager
    {
        public static void SetMemberInternalForcesInLoadCombination(CMember m, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCases> listMemberLoadForces, int iNumberOfMemberResultsSections, out designMomentValuesForCb sMomentValuesforCb_output, out basicInternalForces[] sBIF_x_output)
        {
            sMomentValuesforCb_output = new designMomentValuesForCb();
            sBIF_x_output = new basicInternalForces[iNumberOfMemberResultsSections];

            if (listMemberLoadForces != null) // If some results data exist
            {
                foreach (CLoadCase lc in lcomb.LoadCasesList)
                {
                    CMemberInternalForcesInLoadCases mlf = listMemberLoadForces.Find(i => i.Member.ID == m.ID && i.LoadCase.ID == lc.ID);
                    if (mlf != null)
                    {
                        sMomentValuesforCb_output.fM_14 += lc.Factor * mlf.BendingMomentValues.fM_14;
                        sMomentValuesforCb_output.fM_24 += lc.Factor * mlf.BendingMomentValues.fM_24;
                        sMomentValuesforCb_output.fM_34 += lc.Factor * mlf.BendingMomentValues.fM_34;
                        sMomentValuesforCb_output.fM_max += lc.Factor * mlf.BendingMomentValues.fM_max;

                        int j = 0;
                        foreach (basicInternalForces bif in mlf.InternalForces)
                        {
                            sBIF_x_output[j].fN += lc.Factor * bif.fN;
                            sBIF_x_output[j].fV_yu += lc.Factor * bif.fV_yu;
                            sBIF_x_output[j].fV_yy += lc.Factor * bif.fV_yy;
                            sBIF_x_output[j].fV_zv += lc.Factor * bif.fV_zv;
                            sBIF_x_output[j].fV_zz += lc.Factor * bif.fV_zz;
                            sBIF_x_output[j].fT += lc.Factor * bif.fT;
                            sBIF_x_output[j].fM_yu += lc.Factor * bif.fM_yu;
                            sBIF_x_output[j].fM_yy += lc.Factor * bif.fM_yy;
                            sBIF_x_output[j].fM_zv += lc.Factor * bif.fM_zv;
                            sBIF_x_output[j].fM_zz += lc.Factor * bif.fM_zz;

                            j++;
                        }
                    }
                }
            }
        }


    }
}
