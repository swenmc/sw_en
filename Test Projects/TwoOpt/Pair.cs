namespace TwoOpt
{
   public class Pair
   {
      private readonly double _x;
      private readonly double _y;

      public Pair(double x, double y)
      {
         _x = x;
         _y = y;
      }   

      public double X()
      {
         return _x;
      }

      public double Y()
      {
         return _y;
      }
   }
}