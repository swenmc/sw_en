using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    public class CCrSc_2_00_AAC_Horizontal_Wall_Panel : CCrSc_2_00_AAC_Roof_Panel
    {
        // Solid AAC roof panel - grooved

        public CCrSc_2_00_AAC_Horizontal_Wall_Panel()
        {
        }
        public CCrSc_2_00_AAC_Horizontal_Wall_Panel(float fh, float fb)
        {
            IsShapeSolid = true;

            ITotNoPoints = 12;
            INoAuxPoints = 0;
            h = fh;
            b = fb;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // All indices together
            //loadCrScIndices();
        }
    }
}
