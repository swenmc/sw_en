using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    public class CCrSc_2_00 : CCrSc_0_05
    {
        // Solid rectangle

        public CCrSc_2_00()
      {
      }
          public CCrSc_2_00(float fh, float fb/*, float ft*/)
      {
            //ITotNoPoints = 4;
          IsShapeSolid = true;
          ITotNoPoints = 4;
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

          // Fill list of indices for drawing of surface - triangles edges
          loadCrScIndices();
      }


    }
}
