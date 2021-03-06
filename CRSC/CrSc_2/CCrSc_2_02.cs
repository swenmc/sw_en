﻿using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_2_02 : CCrSc_0_02
    {
      // Solid circle

      public CCrSc_2_02(float fd)
      {
          // m_iTotNoPoints = 72+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
          Fd = fd;
          ITotNoPoints = 73; // 1 auxialiary node in centroid / stredovy bod v tazisku

          Fr_out = Fd / 2f;

          if (Fr_out <= 0f)
              return;

          // Create Array - allocate memory
          //CrScPointsOut = new float[ITotNoPoints, 2];
          CrScPointsOut = new List<Point>(ITotNoPoints);
            
            // Fill Array Data
            CalcCrSc_Coord();

          // Fill list of indices for drawing of surface - triangles edges
          loadCrScIndices();
      }
    }
}
