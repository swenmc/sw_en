﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_CB:CScrewArrangement
    {
        // TODO 624
        // Tento objekt moze byt samostatny alebo ho mozeme zlucit s CScrewArrangementRect_PlateType_JKL
        // Podla mna by mal byt radsej samostatny kedze tento objekt ma byt nezavisly na plates, ale obsah Calc_HolesCentersCoord2D by sa refaktoroval
        public CScrewArrangement_CB() { }

        public CScrewArrangement_CB(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fhY, float fx_edge, float fy_edge, float fx/*, float fy*/)
        {
            /*
            float fx_edge = 0.03f;  // x-direction
            float fy_edge = 0.02f;  // y-direction
            float fx = 0.06f;  // x-direction
            float fy = 0.06f;  // y-direction
            */

            // TODO 624 - urobit toto dynamicke, aby sa generovalo podla rect screw arrangement - rovnake alebo rozne vzdialenosti medzi jednotlivymi radmi alebo stlpcami skrutiek
            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge);

                HolesCentersPoints2D[1] = new Point(fx_edge, fhY - fy_edge);

                HolesCentersPoints2D[2] = new Point(fx_edge + fx, fy_edge);

                HolesCentersPoints2D[3] = new Point(fx_edge + fx, fhY - fy_edge);

                HolesCentersPoints2D[4] = new Point(fx_edge + 2*fx, fy_edge);

                HolesCentersPoints2D[5] = new Point(fx_edge + 2*fx, fhY - fy_edge);
            }
        }
    }
}
