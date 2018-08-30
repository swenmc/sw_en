using System.Collections.Generic;

namespace TwoOpt
{
   public interface IMainWindow
   {
      void UpdateIteration(double best, int iter, List<Pair> tourCoords);
      void PlotNodes(List<Pair> displayCoords);
  
      double GridHeight();
      double GridWidth();
   }
}