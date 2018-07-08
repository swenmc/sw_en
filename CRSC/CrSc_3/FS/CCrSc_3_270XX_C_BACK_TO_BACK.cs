using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    public class CCrSc_3_270XX_C_BACK_TO_BACK : CCrSc_3_I_LIPS
    {
        // Thin-walled -back to back template // TODO - zapracovat system vykreslovania zlozenych prierezov (napr. 2 x C - prierez))

        public CCrSc_3_270XX_C_BACK_TO_BACK(float fh, float fb, float fc_lip, float ft, Color color_temp) : base(fh, fb, fc_lip, ft, color_temp)
        {
            Name = "C " + (fh * 1000).ToString() + (ft * 1000 * 100).ToString() + " back to back";
            CSColor = color_temp;  // Set cross-section color

            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 20;

            h = fh;
            b = fb;
            Ft_f = ft;
            Ft_w = ft;
            Fc_lip = fc_lip;

            CSColor = color_temp;
            Fd = (float)h - 2 * ft;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            FillCrScPropertiesByTableData();
        }

        public void FillCrScPropertiesByTableData()
        {
            // Do not calculate but set table data
            A_g = 1057 / 1e+6;
        }
    }
}
