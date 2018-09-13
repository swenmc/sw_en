using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_5_01 : CCrSc_0_05
    {
        // Solid rectangle
      public CCrSc_5_01(float fh, float fb, float ft)
      {
          //ITotNoPoints = 4;
          h = fh;
          b = fb;

          // Create Array - allocate memory
          CrScPointsOut = new float[ITotNoPoints, 2];
            //CrScPointsOut = new List<Point>(ITotNoPoints);
            // Fill Array Data
            CalcCrSc_Coord();
      }
    }
}
