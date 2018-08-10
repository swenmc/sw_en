using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using CRSC;
using M_AS4600;

namespace M_BASE
{
    public class CMemberDesign
    {
        bool bDebugging = false;

        public CMemberDesign() { }
        public void SetDesignForcesAndMemberDesign(int iNumberOfLoadCombinations, int iNumberOfDesignSections, CCrSc_TW section, float fTheoreticalLengthOfMember, basicInternalForces[,] sBIF_x, designMomentValuesForCb[] sMomentValuesforCb, out designInternalForces[,] sDIF_x)
        {
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

                    CCalcul obj_CalcDesign = new CCalcul(bDebugging, sDIF_x[i, j], section, fTheoreticalLengthOfMember, sMomentValuesforCb[i]);
                }
            }
        }
    }
}
