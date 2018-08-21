namespace CRSC
{
    public class CCrSc_3_05 : CCrSc_0_22
    {
      // Circular Hollow Section / Tube / Pipe
      public CCrSc_3_05(float fd, float ft)
      {
          INoPointsIn = INoPointsOut = 72; // vykreslujeme ako n-uholnik, pocet bodov n

          Fd = fd;
          Ft = ft;

          Fd_in = Fd - 2 * Ft;

          // Radii
          Fr_out = Fd / 2f;
          Fr_in = Fd_in / 2f;

          if (Fr_in == Fr_out)
              return;

          // Create Array - allocate memory
          CrScPointsOut = new float[INoPointsOut, 2];
          CrScPointsIn = new float[INoPointsIn, 2];

          // Fill Array Data
          CalcCrSc_Coord();

          // Fill list of indices for drawing of surface - triangles edges
          loadCrScIndices();
      }

      protected override void loadCrScIndicesFrontSide()
      {
      }

      protected override void loadCrScIndicesShell()
      {
      }

      protected override void loadCrScIndicesBackSide()
      {
      }
    }
}
