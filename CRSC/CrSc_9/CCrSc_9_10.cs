using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace CRSC
{
    public class CCrSc_9_10 : CCrSc_0_05
    {
      // Solid rectangle

      public CCrSc_9_10(float fh, float fb, float ft)
      {
          //ITotNoPoints = 4;
          h = fh;
          b = fb;

          // Create Array - allocate memory
          CrScPointsOut = new float[ITotNoPoints, 2];
          // Fill Array Data
          CalcCrSc_Coord();
      }
    }
}
